using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;

namespace D10.Norm
{
    public delegate T CacheRunDelegate<out T>();

    internal class CacheResult<T>
    {

        public CacheResult(T value, bool cached)
        {
            Value = value;
            WasCached = cached;
        }

        public T Value { get; set; }
        public bool WasCached { get; set; }
    }

    internal class CacheManager
    {
        private class CacheItem
        {
            private DateTime ExpirationDate { get; set; }
            public string Key { get; set; }
            public object Value { get; set; }
            public TimeSpan LifeSpan { private get; set; }

            public void ResetExpiration()
            {
                ExpirationDate = DateTime.Now + LifeSpan;
            }

            public bool IsExpired
            {
                get { return DateTime.Now > ExpirationDate; }
            }
        }

        private readonly object _locker = new object();
        private readonly Dictionary<string, CacheItem> _data = new Dictionary<string, CacheItem>();

        private void SetValue(string key, object value, TimeSpan lifeSpan)
        {
            CacheItem item;
            lock (_locker)
            {
                if (_data.ContainsKey(key))
                {
                    item = _data[key];
                    if (item.IsExpired)
                    {
                        _data.Remove(key);
                        item = null;
                    }
                }
                else
                {
                    item = new CacheItem() { Key = key, Value = value };
                    _data.Add(key, item);
                }
            }
            if (item != null)
            {
                item.LifeSpan = lifeSpan;
                item.ResetExpiration();
            }
        }

        private bool IsCached(string key)
        {
            lock (_locker)
            {
                if (_data.ContainsKey(key))
                {
                    var item = _data[key];
                    if (item.IsExpired)
                    {
                        _data.Remove(key);
                        return false;
                    }
                    return true;
                }
                return false;
            }
        }

        private T GetValue<T>(string key)
        {
            CacheItem item = null;
            lock (_locker)
            {
                if (_data.ContainsKey(key))
                {
                    item = _data[key];
                    if (item.IsExpired)
                    {
                        _data.Remove(key);
                        item = null;
                    }
                }
            }
            if (item != null)
            {
                item.ResetExpiration();
                return (T)item.Value;
            }
            else
                return default(T);
        }


        private const int LockTimeout = 30000;
        private readonly Dictionary<string, ReaderWriterLock> _lockers = new Dictionary<string, ReaderWriterLock>();
        private readonly object _lockersLocker = new object();

        private void AcquireLock(string key)
        {
            try
            {
                ReaderWriterLock locker = GetLocker(key);
                locker.AcquireWriterLock(LockTimeout);
            }
            catch
            {
            }
        }

        private void ReleaseLock(string key)
        {
            try
            {
                ReaderWriterLock locker = GetLocker(key);
                locker.ReleaseWriterLock();
            }
            catch
            {
            }
        }

        private ReaderWriterLock GetLocker(string key)
        {
            lock (_lockersLocker)
            {
                if (_lockers.ContainsKey(key))
                {
                    return _lockers[key];
                }
                else
                {
                    ReaderWriterLock locker = new ReaderWriterLock();
                    _lockers.Add(key, locker);
                    return locker;
                }
            }
        }

        private static string GenerateKey(string root, object[] parameters)
        {
            StringBuilder result = new StringBuilder(root);
            foreach (object o in parameters)
            {
                result.AppendFormat("-{0}", o ?? "");
            }
            return result.ToString();
        }

        private static string GenerateRegexKey(string root, object[] parameters)
        {
            StringBuilder result = new StringBuilder(String.Format(root, parameters));
            result.Append(".*");
            return result.ToString();
        }

        public CacheResult<T> CacheRun<T>(string cacheKey, object[] parameters, CacheRunDelegate<T> runner,
                                          TimeSpan lifeSpan)
        {
            if (cacheKey == null) return new CacheResult<T>(runner(), false);
            string fullKey = CacheManager.GenerateKey(cacheKey, parameters);
            if (IsCached(fullKey))
            {
                var value = GetValue<T>(fullKey);
                if (value != null)
                    return new CacheResult<T>(value, true);
            }
            try
            {
                AcquireLock(fullKey);
                T value = runner();
                SetValue(fullKey, value, lifeSpan);
                return new CacheResult<T>(value, false);
            }
            finally
            {
                ReleaseLock(fullKey);
            }
        }

        public void RemoveCachedValue(string cacheKey, params object[] parameters)
        {
            RemoveMultipleCachedValues(GenerateKey(cacheKey, parameters));
        }

        public void RemoveSingleCachedValue(string cacheKey)
        {
            lock (_locker)
            {
                _data.Remove(cacheKey);
            }
        }

        private IEnumerable<string> FindKeys(string regex)
        {
            List<String> res = new List<string>();
            lock (_locker)
            {
                foreach (string s in _data.Keys)
                {
                    if (Regex.IsMatch(s, regex))
                        res.Add(s);
                }
            }
            return res.ToArray();
        }

        public void RemoveMultipleCachedValues(string cacheKey, params object[] parameters)
        {
            string regex = GenerateRegexKey(cacheKey, parameters);
            IEnumerable<string> keys = FindKeys(regex);
            foreach (string s in keys)
            {
                RemoveSingleCachedValue(s);
            }
        }

    }
}

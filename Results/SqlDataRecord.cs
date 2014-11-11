using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using D10.Common;

namespace D10.Norm.Results
{
    public class SqlDataRecord : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public SqlDataRecord()
        {
        }

        public SqlDataRecord(IEnumerable<KeyValuePair<string, int>> mapping, IDataRecord record)
        {
            foreach (var item in mapping)
            {
                this[item.Key] = SqlUtils.ToValue(record[item.Value]);
            }
        }

        public object this[string key]
        {
            get
            {
                return _values.ContainsKey(key) ? _values[key] : null;
            }
            set
            {
                if (_values.ContainsKey(key))
                {
                    _values[key] = SqlUtils.ToValue(value);
                }
                else
                {
                    _values.Add(key, SqlUtils.ToValue(value));
                }
            }
        }

        public object this[int index]
        {
            get
            {
                if (index >= _values.Count) return null;
                return this[_values.Keys.Skip(index).First()];
            }
            set
            {
                if (index >= _values.Count) return;
                this[_values.Keys.Skip(index).First()] = value;
            }
        }

        public IList<string> Keys
        {
            get
            {
                return _values.Keys.ToList();
            }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _values.GetEnumerator();
        }
    }
}

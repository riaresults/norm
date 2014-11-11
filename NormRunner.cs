using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System.Data;
using D10.Norm.Results;
using D10.Norm.Persistance;

namespace D10.Norm
{
    public static class NormRunner
    {
        #region private
        private static readonly CacheManager _cacheManager = new CacheManager();

        private static NormOperationAttribute GetFirstAttributeFromStack(StackTrace stackTrace)
        {
            int i = 1;
            NormOperationAttribute operationDescription = null;
            while ((operationDescription == null) && (i < stackTrace.FrameCount))
            {
                MethodBase method = stackTrace.GetFrame(i).GetMethod();
                object[] attrs = method.GetCustomAttributes(typeof(NormOperationAttribute), false);
                if (attrs.Length > 0)
                {
                    operationDescription = attrs[0] as NormOperationAttribute;
                    if (operationDescription != null) operationDescription.Method = method;
                }
                i++;
            }
            return operationDescription;
        }

        private static IDictionary<string, object> GetParameterDictionary(NormOperationAttribute operationDescription, object[] parameters)
        {
            IDictionary<string, object> result = new Dictionary<string, object>();
            if (operationDescription.ParameterNames == null)
            {
                ParameterInfo[] pars = operationDescription.Method.GetParameters();
                for (int i = 0; i < Math.Min(parameters.Length, pars.Length); i++)
                {
                    result.Add(pars[i].Name, parameters[i]);
                }
                return result;
            }
            for (int pi = 0; pi < operationDescription.ParameterNames.Length; pi++)
            {
                result.Add(operationDescription.ParameterNames[pi], pi < parameters.Length ? parameters[pi] : null);
            }

            return result;
        }

        private static IList<T> PopulateFromReader<T>(IDataReader reader)
        {
            List<T> result = new List<T>();
            IDictionary<string, int> mapping = null;
            while (reader.Read())
            {
                if (mapping == null)
                {
                    mapping = new Dictionary<string, int>();
                    for (int fc = 0; fc < reader.FieldCount; fc++)
                    {
                        mapping.Add(reader.GetName(fc).ToLowerInvariant(), fc);
                    }
                }
                result.Add(DataRecordSourceBuilder.BuildObject<T>(new SqlDataRecord(mapping, reader)));
            }
            return result;
        }

        private static T PopulateFirstFromReader<T>(IDataReader reader)
        {
            T result = default(T);
            if (reader.Read())
            {
                IDictionary<string, int> mapping = new Dictionary<string, int>();
                for (int fc = 0; fc < reader.FieldCount; fc++)
                {
                    mapping.Add(reader.GetName(fc).ToLowerInvariant(), fc);
                }
                result = DataRecordSourceBuilder.BuildObject<T>(new SqlDataRecord(mapping, reader));
            }
            return result;
        }

        private static TimeSpan GetLifeSpan(NormOperationAttribute description)
        {
            return description.SecondsCached == 0 ? new TimeSpan(0, 0, 30) : new TimeSpan(0, 0, description.SecondsCached);
        }

        #endregion

        public static SingleSetResult<T> ReadSingleSet<T>(params object[] parameters)
        {
            StackTrace stackTrace = new StackTrace();
            NormOperationAttribute operationDescription = GetFirstAttributeFromStack(stackTrace);
            if (operationDescription == null)
                throw new ApplicationException("Operation Description Not Found");


            DateTime start = DateTime.Now;
            CacheResult<IList<T>> result = _cacheManager.CacheRun(operationDescription.CacheKey, parameters,
                delegate
                {
                    var connectionString = ConnectionStringHandler.GetDataSetConnectionString(operationDescription.DataSet);
                    IList<T> res = null;
                    PersistanceInterfaceHandler.GetInstance(connectionString.ProviderName).ExecuteReader(connectionString.ConnectionString, operationDescription.CommandName,
                        operationDescription.NullsParametersAsDefault, 0, GetParameterDictionary(operationDescription, parameters),
                        delegate(IDataReader reader) { res = PopulateFromReader<T>(reader); });
                    return res;
                }, GetLifeSpan(operationDescription));
            SingleSetResult<T> setResult = new SingleSetResult<T>(result.Value) { Duration = DateTime.Now - start };
            if (operationDescription.InvalidatesCache != null)
            {
                foreach (string cache in operationDescription.InvalidatesCache)
                    _cacheManager.RemoveMultipleCachedValues(cache, parameters);
            }
            return setResult;
        }

        public static FirstResult<T> ReadFirst<T>(params object[] parameters)
        {
            StackTrace stackTrace = new StackTrace();
            NormOperationAttribute operationDescription = GetFirstAttributeFromStack(stackTrace);

            if (operationDescription == null)
                throw new Exception("Description Not Found");

            DateTime start = DateTime.Now;
            CacheResult<T> result = _cacheManager.CacheRun(operationDescription.CacheKey, parameters,
                delegate
                {
                    var cs = ConnectionStringHandler.GetDataSetConnectionString(operationDescription.DataSet);
                    T res = default(T);
                    PersistanceInterfaceHandler.GetInstance(cs.ProviderName).ExecuteReader(cs.ConnectionString, operationDescription.CommandName,
                        operationDescription.NullsParametersAsDefault, 0, GetParameterDictionary(operationDescription, parameters),
                        delegate(IDataReader reader) { res = PopulateFirstFromReader<T>(reader); });
                    return res;
                }, GetLifeSpan(operationDescription));
            FirstResult<T> setResult = new FirstResult<T>(result.Value) { Duration = DateTime.Now - start };
            if (operationDescription.InvalidatesCache != null)
            {
                foreach (string cache in operationDescription.InvalidatesCache)
                    _cacheManager.RemoveMultipleCachedValues(cache, parameters);
            }
            return setResult;
        }

        public static RunCommandResult RunCommand(params object[] parameters)
        {
            DateTime start = DateTime.Now;
            StackTrace stackTrace = new StackTrace();
            NormOperationAttribute operationDescription = GetFirstAttributeFromStack(stackTrace);

            if (operationDescription == null)
                throw new Exception("Description Not Found");

            var cs = ConnectionStringHandler.GetDataSetConnectionString(operationDescription.DataSet);
            PersistanceInterfaceHandler.GetInstance(cs.ProviderName).ExecuteNonQuery(cs.ConnectionString, operationDescription.CommandName, operationDescription.NullsParametersAsDefault, 0, GetParameterDictionary(operationDescription, parameters));
            RunCommandResult setResult = new RunCommandResult { Duration = DateTime.Now - start };
            if (operationDescription.InvalidatesCache != null)
            {
                foreach (string cache in operationDescription.InvalidatesCache)
                    _cacheManager.RemoveMultipleCachedValues(cache, parameters);
            }
            return setResult;
        }

        public static ScalarResult<T> GetScalarResult<T>(params object[] parameters)
        {
            ScalarResult<T> result = new ScalarResult<T>();
            DateTime start = DateTime.Now;
            StackTrace stackTrace = new StackTrace();
            NormOperationAttribute operationDescription = GetFirstAttributeFromStack(stackTrace);

            if (operationDescription == null)
                throw new Exception("Description Not Found");

            var cs = ConnectionStringHandler.GetDataSetConnectionString(operationDescription.DataSet);
            object tempRes = PersistanceInterfaceHandler.GetInstance(cs.ProviderName).ExecuteScalar(
                cs.ConnectionString, operationDescription.CommandName,
                operationDescription.NullsParametersAsDefault, 0,
                GetParameterDictionary(operationDescription, parameters));            
            result.Value = ConvertUtils.ConvertOrCast<T>(tempRes);
            result.Duration = DateTime.Now - start;
            if (operationDescription.InvalidatesCache != null)
            {
                foreach (string cache in operationDescription.InvalidatesCache)
                    _cacheManager.RemoveMultipleCachedValues(cache, parameters);
            }
            return result;
        }

        public static IEnumerable<IDataRecord> GetDataRecords(params object[] parameters)
        {
            DateTime start = DateTime.Now;
            StackTrace stackTrace = new StackTrace();
            NormOperationAttribute operationDescription = GetFirstAttributeFromStack(stackTrace);

            if (operationDescription == null)
                throw new Exception("Description Not Found");

            var cs = ConnectionStringHandler.GetDataSetConnectionString(operationDescription.DataSet);
            var res = PersistanceInterfaceHandler.GetInstance(cs.ProviderName).EnumDataRecords(
                cs.ConnectionString, operationDescription.CommandName,
                operationDescription.NullsParametersAsDefault, 0,
                GetParameterDictionary(operationDescription, parameters));
            if (operationDescription.InvalidatesCache != null)
            {
                foreach (string cache in operationDescription.InvalidatesCache)
                    _cacheManager.RemoveMultipleCachedValues(cache, parameters);
            }
            return res;
        }


        public static BulkInsertResult BulkInsert(DataTable table, int batchSize)
        {
            DateTime start = DateTime.Now;
            StackTrace stackTrace = new StackTrace();
            NormOperationAttribute operationDescription = GetFirstAttributeFromStack(stackTrace);

            if (operationDescription == null)
                throw new Exception("Description Not Found");

            var cs = ConnectionStringHandler.GetDataSetConnectionString(operationDescription.DataSet);
            PersistanceInterfaceHandler.GetInstance(cs.ProviderName).BulkInsert(cs.ConnectionString, 0, batchSize, table);

            BulkInsertResult setResult = new BulkInsertResult();
            setResult.Duration = DateTime.Now - start;
            return setResult;
        }
    }
}

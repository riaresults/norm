using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Data;

namespace D10.Norm.Persistance
{
    public class SqlServerCommand: IPersistanceInterface
    {
        private static Dictionary<string, string> _commandCache = new Dictionary<string,string>();
        private static object _locker = new object();
        private static string _scriptFolder = ConfigurationManager.AppSettings["Norm.SqlCommandFolder"];

        private static string LoadCommand(string name)
        {
            lock(_locker)
            {
                if (!_commandCache.ContainsKey(name))
                {
                    string fileName = Path.Combine(_scriptFolder, name);;
                    if (!File.Exists(fileName))
                    {
                        throw new ApplicationException("Command File Not Found: " + fileName);
                    }
                    _commandCache.Add(name, File.ReadAllText(fileName));
                }
                return _commandCache[name];
            }
        }
        public void ExecuteReader(string connectionString, string command, bool nullsAsDefault, int timeout, IDictionary<string, object> parameters, DatabaseReaderCallback callback)
        {
            string cmd = LoadCommand(command);
            SqlServerPersistanceCommon.ExecuteReader(connectionString, cmd, false, nullsAsDefault, timeout, parameters, callback);
        }

        public int ExecuteNonQuery(string connectionString, string command, bool nullsAsDefault, int timeout, IDictionary<string, object> parameters)
        {
            string cmd = LoadCommand(command);
            return SqlServerPersistanceCommon.ExecuteNonQuery(connectionString, cmd, false, nullsAsDefault, timeout, parameters);
        }

        public object ExecuteScalar(string connectionString, string command, bool nullsAsDefault, int timeout, IDictionary<string, object> parameters)
        {
            string cmd = LoadCommand(command);
            return SqlServerPersistanceCommon.ExecuteScalar(connectionString, cmd, false, nullsAsDefault, timeout, parameters);
        }

        public IEnumerable<IDataRecord> EnumDataRecords(string connectionString, string command, bool nullsAsDefault, int timeout, IDictionary<string, object> parameters)
        {
            string cmd = LoadCommand(command);
            return SqlServerPersistanceCommon.EnumDataRecords(connectionString, cmd, true, nullsAsDefault, timeout, parameters);
        }


        public void BulkInsert(string connectionString, int timeout, int batchSize, DataTable table)
        {
            SqlServerPersistanceCommon.BulkInsert(connectionString, timeout, batchSize, table);
        }
    }
}

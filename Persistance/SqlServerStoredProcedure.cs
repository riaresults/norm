using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace D10.Norm.Persistance
{
    public class SqlServerStoredProcedure : IPersistanceInterface
    {
        public void ExecuteReader(string connectionString, string command, bool nullsAsDefault, int timeout, IDictionary<string, object> parameters, DatabaseReaderCallback callback)
        {
            SqlServerPersistanceCommon.ExecuteReader(connectionString, command, true, nullsAsDefault, timeout, parameters, callback);
        }

        public int ExecuteNonQuery(string connectionString, string command, bool nullsAsDefault, int timeout, IDictionary<string, object> parameters)
        {
            return SqlServerPersistanceCommon.ExecuteNonQuery(connectionString, command, true, nullsAsDefault, timeout, parameters);
        }

        public object ExecuteScalar(string connectionString, string command, bool nullsAsDefault, int timeout, IDictionary<string, object> parameters)
        {
            return SqlServerPersistanceCommon.ExecuteScalar(connectionString, command, true, nullsAsDefault, timeout, parameters);
        }

        public IEnumerable<IDataRecord> EnumDataRecords(string connectionString, string command, bool nullsAsDefault, int timeout, IDictionary<string, object> parameters)
        {
            return SqlServerPersistanceCommon.EnumDataRecords(connectionString, command, true, nullsAsDefault, timeout, parameters);
        }

        public void BulkInsert(string connectionString, int timeout, int batchSize, DataTable table)
        {
            SqlServerPersistanceCommon.BulkInsert(connectionString, timeout, batchSize, table);
        }
    }
}

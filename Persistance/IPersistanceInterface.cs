using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace D10.Norm.Persistance
{
    public delegate void DatabaseReaderCallback(IDataReader reader);

    public interface IPersistanceInterface
    {
        void ExecuteReader(string connectionString, string command, bool nullsAsDefault, int timeout, IDictionary<string, object> parameters, DatabaseReaderCallback callback);
        int ExecuteNonQuery(string connectionString, string command, bool nullsAsDefault, int timeout, IDictionary<string, object> parameters);
        object ExecuteScalar(string connectionString, string command, bool nullsAsDefault, int timeout, IDictionary<string, object> parameters);

        IEnumerable<IDataRecord> EnumDataRecords(string connectionString, string command, bool isStoredProcedure,
                                                 int timeout, IDictionary<string, object> parameters);

        void BulkInsert(string connectionString, int timeout, int batchSize, DataTable table);
    }
}

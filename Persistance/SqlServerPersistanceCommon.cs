using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace D10.Norm.Persistance
{
    internal class SqlServerPersistanceCommon
    {
        public static void ExecuteReader(string connectionString, string command, bool isStoredProcedure, bool nullsAsDefault, int timeout, IDictionary<string, object> parameters, DatabaseReaderCallback callback)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(command, con);
                    cmd.CommandTimeout = timeout;
                    cmd.CommandType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
                    foreach (var item in parameters)
                    {
                        if ((item.Value == null) && (!nullsAsDefault))
                            cmd.Parameters.Add(new SqlParameter(item.Key, DBNull.Value));
                        else
                            cmd.Parameters.Add(new SqlParameter(item.Key, item.Value));
                    }
                    SqlDataReader reader = cmd.ExecuteReader();
                    callback(reader);
                    reader.Close();
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public static int ExecuteNonQuery(string connectionString, string command, bool isStoredProcedure, bool nullsAsDefault, int timeout, IDictionary<string, object> parameters)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(command, con);
                    cmd.CommandTimeout = timeout;
                    cmd.CommandType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
                    foreach (var item in parameters)
                    {
                        if ((item.Value == null) && (!nullsAsDefault))
                            cmd.Parameters.Add(new SqlParameter(item.Key, DBNull.Value));
                        else
                            cmd.Parameters.Add(new SqlParameter(item.Key, item.Value));
                    }
                    return cmd.ExecuteNonQuery();
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public static object ExecuteScalar(string connectionString, string command, bool isStoredProcedure, bool nullsAsDefault, int timeout, IDictionary<string, object> parameters)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(command, con);
                    cmd.CommandTimeout = timeout;
                    cmd.CommandType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
                    foreach (var item in parameters)
                    {
                        if ((item.Value == null) && (!nullsAsDefault))
                            cmd.Parameters.Add(new SqlParameter(item.Key, DBNull.Value));
                        else
                            cmd.Parameters.Add(new SqlParameter(item.Key, item.Value));
                    }
                    return cmd.ExecuteScalar();
                }
                finally
                {
                    con.Close();
                }
            }
        }
        public static IEnumerable<IDataRecord> EnumDataRecords(string connectionString, string command, bool isStoredProcedure, bool nullsAsDefault, int timeout, IDictionary<string, object> parameters)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(command, con)
                    {
                        CommandTimeout = timeout,
                        CommandType =
                            isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text
                    };
                    foreach (var item in parameters)
                    {
                        if ((item.Value == null) && (!nullsAsDefault))
                            cmd.Parameters.Add(new SqlParameter(item.Key, DBNull.Value));
                        else
                            cmd.Parameters.Add(new SqlParameter(item.Key, item.Value));
                    }
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public static void BulkInsert(string connectionString, int timeout, int batchSize, DataTable table)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    SqlBulkCopy bcp = new SqlBulkCopy(con)
                    {
                        BatchSize = batchSize,
                        BulkCopyTimeout = timeout,
                        DestinationTableName = table.TableName
                    };
                    bcp.WriteToServer(table);
                }
                finally
                {
                    con.Close();
                }
            }
        }
    }
}

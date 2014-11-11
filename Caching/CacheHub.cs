using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace D10.Common.Caching
{
    internal class CacheHub
    {
        private static CacheHub _instance = new CacheHub();

      /*  private CacheHub()
        { 
            string cs = ConnectionStringHandler.GetDataSetConnectionString("CacheHub").ConnectionString;
            if (cs != null)
                SetupObjects(cs);
        }

        private void SetupObjects(string connectionString)
        {
            //DatabaseInterfaceHandler.Instance.ExecuteNonQuery(
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(Resources.SetupSql);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();


        }*/
    }
}

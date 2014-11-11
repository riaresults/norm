using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace D10.Norm.Persistance
{
    public static class ConnectionStringHandler
    {
        public static ConnectionStringSettings GetDataSetConnectionString(string dataset)
        {
            var item = ConfigurationManager.ConnectionStrings["Norm." + dataset];
            if (item == null)
                return null;
            else
                return item;
        }
        
    }
}

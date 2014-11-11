using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D10.Norm.Persistance
{
    public class PersistanceInterfaceHandler
    {
        private static readonly object _locker = new object();
        private static readonly Dictionary<string, IPersistanceInterface> _instances = new Dictionary<string, IPersistanceInterface>();

        public static IPersistanceInterface GetInstance(string type)
        {
            lock (_locker)
            {
                if (!_instances.ContainsKey(type))
                {
                    Type t = Type.GetType(type);
                    if (t != null)
                    {
                        IPersistanceInterface pi = (IPersistanceInterface)Activator.CreateInstance(t);
                        _instances.Add(type, pi);
                    }
                }
            }
            return _instances[type];
        }
    }
}

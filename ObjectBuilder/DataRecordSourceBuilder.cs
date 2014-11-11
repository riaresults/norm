using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;
using System.Reflection;
using D10.Norm.ObjectBuilder;
using D10.Norm.Results;
namespace D10.Norm
{
    internal class DataRecordSourceBuilder
    {
        private static readonly DataRecordSourceBuilder _instance = new DataRecordSourceBuilder();
        private DataRecordSourceBuilder()
        {

        }

        private readonly object _locker = new object();
        private readonly HashSet<Type> _inspectedTypes = new HashSet<Type>();
        
        private readonly Dictionary<Type, TypeDescription> _descriptions = new Dictionary<Type, TypeDescription>();
        private readonly Dictionary<Type, IPopulator> _populators = new Dictionary<Type, IPopulator>();

        private void InspectType(Type t)
        {
            lock (_locker)
            {
                if (_inspectedTypes.Contains(t)) return;
                _inspectedTypes.Add(t);
                IPopulator pop = PopulatorFactory.GetPopulator(t);
                _populators.Add(t, pop);
                if (pop.RequiresTypeDescription())
                {
                    _descriptions.Add(t, new TypeDescription(t));
                }
            }
        }

        public static T BuildObject<T>(SqlDataRecord values)
        {
            Type t = typeof(T);
            _instance.InspectType(t);
            return _instance._populators[t].BuildInstance<T>(values, _instance._descriptions.ContainsKey(t) ? _instance._descriptions[t] : null);
        }        
    }
}

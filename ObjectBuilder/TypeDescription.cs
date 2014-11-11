using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace D10.Norm.ObjectBuilder
{
    internal class TypeDescription
    {
        private readonly Dictionary<string, List<FastInvokeHandler>> _getters = new Dictionary<string, List<FastInvokeHandler>>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<KeyValuePair<FastInvokeHandler, Type>>> _setters = new Dictionary<string, List<KeyValuePair<FastInvokeHandler, Type>>>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<FieldInfo>> _fields = new Dictionary<string, List<FieldInfo>>(StringComparer.OrdinalIgnoreCase);
        private readonly List<ConstructorInfo> _constructors = new List<ConstructorInfo>();

        public TypeDescription(Type t)
        {
            PropertyInfo[] pis = t.GetProperties();
            foreach (PropertyInfo pi in pis)
            {

                string aliasBase = pi.Name.ToLowerInvariant();
                MethodInfo g = pi.GetGetMethod();
                MethodInfo s = pi.GetSetMethod();
                FastInvokeHandler fg = null;
                FastInvokeHandler fs = null;
                if (g != null)
                    fg = FastInvoke.GetMethodInvoker(g);

                if (s != null)
                    fs = FastInvoke.GetMethodInvoker(s);

                if (!_getters.ContainsKey(aliasBase)) _getters.Add(aliasBase, new List<FastInvokeHandler>());
                if (!_setters.ContainsKey(aliasBase)) _setters.Add(aliasBase, new List<KeyValuePair<FastInvokeHandler, Type>>());

                if (fg != null)
                    _getters[aliasBase].Add(fg);
                if (fs != null)
                {
                    KeyValuePair<FastInvokeHandler, Type> st = new KeyValuePair<FastInvokeHandler, Type>(fs, s.GetParameters()[0].ParameterType);
                    _setters[aliasBase].Add(st);
                }

                object[] attrs = pi.GetCustomAttributes(typeof(AliasAttribute), false);
                foreach (AliasAttribute attrib in attrs)
                {
                    foreach (string alias in attrib.Aliases.Select(x => x.ToLowerInvariant()))
                    {
                        if (!_getters.ContainsKey(alias)) _getters.Add(alias, new List<FastInvokeHandler>());
                        if (!_setters.ContainsKey(alias)) _setters.Add(alias, new List<KeyValuePair<FastInvokeHandler, Type>>());

                        if (fg != null)
                            _getters[alias].Add(fg);
                        if (fs != null)
                        {
                            KeyValuePair<FastInvokeHandler, Type> st = new KeyValuePair<FastInvokeHandler, Type>(fs, s.GetParameters()[0].ParameterType);
                            _setters[alias].Add(st);
                        }
                    }
                }
            }

            foreach (FieldInfo fi in t.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!_fields.ContainsKey(fi.Name))
                    _fields.Add(fi.Name, new List<FieldInfo>());
                _fields[fi.Name].Add(fi);

                object[] attrs = fi.GetCustomAttributes(typeof(AliasAttribute), false);
                foreach (AliasAttribute attrib in attrs)
                {
                    foreach (string alias in attrib.Aliases.Select(x => x.ToLowerInvariant()))
                    {
                        if (!_fields.ContainsKey(alias))
                            _fields.Add(alias, new List<FieldInfo>());
                        _fields[alias].Add(fi);
                    }
                }
            }

            _constructors.AddRange(t.GetConstructors().OrderBy(x => x.GetParameters().Length));
        }

        public IDictionary<string, List<FastInvokeHandler>> Getters { get { return _getters; } }
        public FastInvokeHandler Getter(string key)
        {
            var all = Getters[key];
            if (all != null && all.Count > 0)
                return all[0];
            return null;
        }

        public IDictionary<string, List<KeyValuePair<FastInvokeHandler, Type>>> Setters { get { return _setters; } }
        public IDictionary<string, List<FieldInfo>> Fields { get { return _fields; } }
        public IList<ConstructorInfo> Constructors
        {
            get { return _constructors; }
        }
    }
}

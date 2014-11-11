using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace D10.Norm
{
    public class NormOperationAttribute: Attribute
    {
        public string DataSet { get; set; }
        public string CommandName { get; set; }
        public string[] ParameterNames { get; set; }
        public string CacheKey { get; set; }
        public int SecondsCached { get; set; }
        public string[] InvalidatesCache { get; set; }
        public bool NullsParametersAsDefault { get; set; }
        public MethodBase Method { get; set; }

        public NormOperationAttribute(string dataSet, string commandName)
        {
            DataSet = dataSet;
            CommandName = commandName;            
        }
    }
}

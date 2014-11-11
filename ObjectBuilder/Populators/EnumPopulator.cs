using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using D10.Norm.Results;

namespace D10.Norm.ObjectBuilder
{
    internal class EnumPopulator : IPopulator
    {
        public bool RequiresTypeDescription()
        {
            return false;
        }

        public bool IsCompatible(Type t)
        {
            return t.IsEnum;
        }

        public T BuildInstance<T>(SqlDataRecord row, TypeDescription description)
        {
            object value = row[0];
            return ConvertUtils.EnumParse<T>(value);
        }       
    }
}

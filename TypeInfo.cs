using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D10.Norm
{
    internal static class TypeInfo
    {
        public static bool IsPrimitive(Type t)
        {
            return t.IsPrimitive;
        }

        public static bool IsStructure(Type t)
        {
            return t.IsValueType;
        }
    }
}

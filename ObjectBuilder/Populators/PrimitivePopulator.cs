using System;
using D10.Norm.Results;

// ReSharper disable CheckNamespace
namespace D10.Norm.ObjectBuilder
// ReSharper restore CheckNamespace
{
    internal class PrimitivePopulator : IPopulator
    {
        public bool IsCompatible(Type t)
        {
            return TypeInfo.IsPrimitive(t) 
                || (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>))
                || t.FullName == "System.String");
        }       

        public T BuildInstance<T>(SqlDataRecord row, TypeDescription description)
        {
            return ConvertUtils.ConvertOrCast<T>(row[0]);
        }

        public bool RequiresTypeDescription()
        {
            return false;
        }      
    }
}

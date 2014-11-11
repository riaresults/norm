using System;
using System.Data.SqlTypes;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

// ReSharper disable CheckNamespace
namespace D10.Norm
// ReSharper restore CheckNamespace
{
    internal static class SqlUtils
    {
        private static readonly IDictionary<Type, FastInvokeHandler> _mapping = new Dictionary<Type, FastInvokeHandler>();

        static SqlUtils()
        {
            foreach (MethodInfo mi in typeof(SqlUtils).GetMethods())
            {
                if (mi.Name != "ToValue") continue;

                Type t = mi.GetParameters()[0].ParameterType;
                if (t != typeof(object))
                {
                    _mapping.Add(t, FastInvoke.GetMethodInvoker(mi));
                }
            }
        }

        public static bool IsSupported(Type t)
        {
            return _mapping.ContainsKey(t);
        }

        public static object ToValue(object nullableValue)
        {
            if (DatabaseUtils.IsNull(nullableValue))
                return null;
            if (_mapping.ContainsKey(nullableValue.GetType()))
            {
                FastInvokeHandler fih = _mapping[nullableValue.GetType()];
                return fih.Invoke(null, new[] { nullableValue });
            }
            return nullableValue;
        }

        public static T ToValue<T>(object nullableValue)
        {
            return (T)ToValue(nullableValue);
        }

        public static byte[] ToValue(SqlBinary sqlValue)
        {
            if (DatabaseUtils.IsNull(sqlValue))
                return null;
            return sqlValue.Value;
        }
        public static byte? ToValue(SqlByte sqlValue)
        {
            if (DatabaseUtils.IsNull(sqlValue))
                return null;
            return sqlValue.Value;
        }
        public static byte[] ToValue(SqlBytes sqlValue)
        {
            if (DatabaseUtils.IsNull(sqlValue))
                return null;
            return sqlValue.Value;
        }
        public static char[] ToValue(SqlChars sqlValue)
        {
            if (DatabaseUtils.IsNull(sqlValue))
                return null;
            return sqlValue.Value;
        }
        public static decimal? ToValue(SqlDecimal sqlValue)
        {
            if (DatabaseUtils.IsNull(sqlValue))
                return null;
            return sqlValue.Value;
        }
        public static double? ToValue(SqlDouble sqlValue)
        {
            if (DatabaseUtils.IsNull(sqlValue))
                return null;
            return sqlValue.Value;
        }
        public static Stream ToValue(SqlFileStream sqlValue)
        {
            if (DatabaseUtils.IsNull(sqlValue))
                return null;
            return sqlValue;
        }
        public static Guid? ToValue(SqlGuid sqlValue)
        {
            if (DatabaseUtils.IsNull(sqlValue))
                return null;
            return sqlValue.Value;
        }
        public static short? ToValue(SqlInt16 sqlValue)
        {
            if (DatabaseUtils.IsNull(sqlValue))
                return null;
            return sqlValue.Value;
        }
        public static int? ToValue(SqlInt32 sqlValue)
        {
            if (DatabaseUtils.IsNull(sqlValue))
                return null;
            return sqlValue.Value;
        }
        public static long? ToValue(SqlInt64 sqlValue)
        {
            if (DatabaseUtils.IsNull(sqlValue))
                return null;
            return sqlValue.Value;
        }
        public static decimal? ToValue(SqlMoney sqlValue)
        {
            if (DatabaseUtils.IsNull(sqlValue))
                return null;
            return sqlValue.Value;
        }
        public static float? ToValue(SqlSingle sqlValue)
        {
            if (DatabaseUtils.IsNull(sqlValue))
                return null;
            return sqlValue.Value;
        }
        public static string ToValue(SqlString sqlValue)
        {
            if (DatabaseUtils.IsNull(sqlValue))
                return null;
            return sqlValue.Value;
        }
        public static DateTime? ToValue(SqlDateTime sqlValue)
        {
            if (DatabaseUtils.IsNull(sqlValue))
                return null;
            return sqlValue.Value;
        }
        public static bool? ToValue(SqlBoolean sqlValue)
        {
            if (DatabaseUtils.IsNull(sqlValue))
                return null;
            return sqlValue.Value;
        }
    }
}
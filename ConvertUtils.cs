using System;
using System.ComponentModel;
using System.Globalization;
using System.Data.SqlTypes;
namespace D10.Norm
{
    internal static class ConvertUtils
    {
        #region Convert
        public static T Convert<T>(object initialValue)
        {
            return Convert<T>(initialValue, CultureInfo.CurrentCulture);
        }

        public static T Convert<T>(object initialValue, CultureInfo culture)
        {
            return (T)Convert(initialValue, culture, typeof(T));
        }

        public static object Convert(object initialValue, CultureInfo culture, Type targetType)
        {
            if (initialValue == null)
                throw new ArgumentNullException("initialValue");

            Type initialType = initialValue.GetType();

            if (targetType == initialType)
                return initialValue;

            if (targetType.IsInterface || targetType.IsGenericTypeDefinition || targetType.IsAbstract)
                throw new ArgumentException("Can only convert to value types and concrete primitives.");


            // use Convert.ChangeType if both types are IConvertible
            if (initialValue is IConvertible && ReflectionUtils.IsSubClass(targetType, typeof(IConvertible)))
            {
                if (initialValue is string && targetType.IsEnum)
                    return Enum.Parse(targetType, initialValue.ToString(), true);
                else
                    return System.Convert.ChangeType(initialValue, targetType, culture);
            }


            // see if source or target types have a TypeConverter that converts between the two
            TypeConverter toConverter = TypeDescriptor.GetConverter(initialType);

            if (toConverter != null && toConverter.CanConvertTo(targetType))
                return toConverter.ConvertTo(null, culture, initialValue, targetType);

            TypeConverter fromConverter = TypeDescriptor.GetConverter(targetType);

            if (fromConverter != null && fromConverter.CanConvertFrom(initialType))
                return fromConverter.ConvertFrom(null, culture, initialValue);


            // handle DBNull and INullable
            if (initialValue == DBNull.Value)
            {
                if (ReflectionUtils.IsNullable(targetType))
                    return EnsureTypeAssignable(null, initialType, targetType);
                else
                    throw new Exception(string.Format("Can not convert null {0} into non-nullable {1}", initialType, targetType));
            }
            else if (initialValue is INullable)
            {
                return EnsureTypeAssignable(SqlUtils.ToValue((INullable)initialValue), initialType, targetType);
            }

            throw new Exception(string.Format("Could not find TypeConverter to convert from {0} to {1}.", initialType, targetType));
        }
        #endregion

        #region TryConvert
        public static bool TryConvert<T>(object initialValue, out T convertedValue)
        {
            return TryConvert(initialValue, CultureInfo.CurrentCulture, out convertedValue);
        }

        public static bool TryConvert<T>(object initialValue, CultureInfo culture, out T convertedValue)
        {
            return Actions.TryAction<T>(delegate
            {
                object tempConvertedValue;
                TryConvert(initialValue, CultureInfo.CurrentCulture, typeof(T), out tempConvertedValue);

                return (T)tempConvertedValue;
            }, out convertedValue);
        }

        public static bool TryConvert(object initialValue, CultureInfo culture, Type targetType, out object convertedValue)
        {
            return Actions.TryAction<object>(delegate { return Convert(initialValue, culture, targetType); }, out convertedValue);
        }
        #endregion

        #region ConvertOrCast
        public static T ConvertOrCast<T>(object initialValue)
        {
            return ConvertOrCast<T>(initialValue, CultureInfo.CurrentCulture);
        }

        public static T ConvertOrCast<T>(object initialValue, CultureInfo culture)
        {
            return (T)ConvertOrCast(initialValue, culture, typeof(T));
        }

        public static object ConvertOrCast(object initialValue, CultureInfo culture, Type targetType)
        {
            object convertedValue;
            if (TryConvert(initialValue, culture, targetType, out convertedValue))
                return convertedValue;

            return EnsureTypeAssignable(initialValue, ReflectionUtils.GetObjectType(initialValue), targetType);
        }
        #endregion

        #region TryConvertOrCast
        public static bool TryConvertOrCast<T>(object initialValue, out T convertedValue)
        {
            return TryConvertOrCast(initialValue, CultureInfo.CurrentCulture, out convertedValue);
        }

        public static bool TryConvertOrCast<T>(object initialValue, CultureInfo culture, out T convertedValue)
        {
            return Actions.TryAction<T>(delegate { return ConvertOrCast<T>(initialValue, culture); }, out convertedValue);
        }
        #endregion

        private static object EnsureTypeAssignable(object value, Type initialType, Type targetType)
        {
            Type valueType = (value != null) ? value.GetType() : null;

            if (value != null && targetType.IsAssignableFrom(valueType))
                return value;
            else if (value == null && ReflectionUtils.IsNullable(targetType))
                return null;
            else
                throw new Exception(string.Format("Could not cast or convert from {0} to {1}.",
                  (initialType != null) ? initialType.ToString() : "{null}", targetType));
        }

        public static T EnumParse<T>(object value)
        {
            if (value is String)
            {
                return (T)Enum.Parse(typeof(T), value as String);
            }
            return (T)Enum.ToObject(typeof(T), ConvertOrCast<long>(value));
        }
        public static object EnumParse(object value, Type t)
        {
            if (value is String)
            {
                return Enum.Parse(t, value as String);
            }
            return Enum.ToObject(t, ConvertOrCast<long>(value));
        }
    }
}

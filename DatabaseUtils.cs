using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Data.SqlTypes;

namespace D10.Norm
{
  internal static class DatabaseUtils
  {
    /// <summary>
    /// Converts specified value to nullable value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static T? ConvertToNullableValue<T>(object value) where T : struct
    {
      if (value == null)
      {
        return null;
      }
      else if (value == DBNull.Value)
      {
        return null;
      }
      else if (value is string && string.IsNullOrEmpty((string)value))
      {
        return null;
      }
      else
      {
        if (!(value is T))
        {
          try
          {
            value = Convert.ChangeType(value, typeof(T));
          }
          catch (Exception e)
          {
            throw new ArgumentException("Value is not a valid type.", "value", e);
          }
        }

        return new T?((T)value);
      }
    }

    /// <summary>
    /// Determines whether the specified value is null.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    /// 	<c>true</c> if the specified value is null; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNull(object value)
    {
      if (value == null)
        return true;

      if (value is INullable && ((INullable)value).IsNull)
        return true;

      if (value == DBNull.Value)
        return true;

      return false;
    }
  }
}

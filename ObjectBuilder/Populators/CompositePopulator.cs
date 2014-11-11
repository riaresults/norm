using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using D10.Norm.Results;

// ReSharper disable CheckNamespace
namespace D10.Norm.ObjectBuilder
// ReSharper restore CheckNamespace
{
    internal class CompositePopulator : IPopulator
    {
        public bool IsCompatible(Type t)
        {
            return true;
        }

        public bool RequiresTypeDescription()
        {
            return true;
        }

        public T BuildInstance<T>(SqlDataRecord row, TypeDescription typeDescription)
        {
            T result = default(T);
            IList<ConstructorInfo> cts = typeDescription.Constructors;
            foreach (ConstructorInfo ci in cts)
            {
                ParameterInfo[] pis = cts[0].GetParameters();
                object[] pars = new object[pis.Length];
                bool canInvoke = true;
                {
                    for (int i = 0; i < pars.Length; i++)
                    {
                        object val = null;
                        if (row.Keys.Contains(pis[i].Name))
                            val = row[pis[i].Name];
                        else
                            if (row.Keys.Count > i)
                                val = row[i];
                        bool assigned = ConvertUtils.TryConvertOrCast(val, CultureInfo.CurrentCulture, out pars[i]);
                        if (!assigned) canInvoke = false;
                    }
                }
                if (canInvoke)
                {
                    try
                    {
                        result = (T)ci.Invoke(pars);
                        break;
                    }
                    catch
                    {
                        
                    }
                }
            }
            
            foreach (string key in row.Keys)
            {
                object value = row[key];
                if (typeDescription.Setters.ContainsKey(key))
                {
                    foreach (KeyValuePair<FastInvokeHandler, Type> ih in typeDescription.Setters[key])
                    {
                        try
                        {
                            Type targetType = ih.Value;
                            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                            {

                                if (value != null)
                                {
                                    System.ComponentModel.NullableConverter nullableConverter
                                        = new System.ComponentModel.NullableConverter(targetType);

                                    targetType = nullableConverter.UnderlyingType;
                                }
                            }

                            ih.Key.Invoke(result,
                                         targetType.IsEnum
                                          ? new[] { value == null ? null : ConvertUtils.EnumParse(value, targetType) }
                                              : new[]
                                                    {
                                                        value == null ? null : ConvertUtils.ConvertOrCast(value,
                                                                                   CultureInfo.InvariantCulture,
                                                                                   targetType)
                                                    });
                        }
                        catch (Exception ex)
                        {
                            throw new ApplicationException(
                                String.Format("Error assigning value \"{0}\" of type {1} to {2}", value,
                                              value.GetType(), key), ex);
                        }
                    }
                }
                if (typeDescription.Fields.ContainsKey(key))
                {
                    foreach (FieldInfo fi in typeDescription.Fields[key])
                    {
                        Type fieldType = fi.FieldType;
                        try
                        {
                            fi.SetValue(result,
                                         fieldType.IsEnum
                                             ? ConvertUtils.EnumParse(value, fieldType)
                                             : ConvertUtils.ConvertOrCast(value,
                                                                                   CultureInfo.InvariantCulture,
                                                                                   fieldType)
                                                    );
                        }
                        catch (Exception ex)
                        {
                            throw new ApplicationException(
                                String.Format("Error assigning value \"{0}\" of type {1} to {2}", value,
                                              value.GetType(), key), ex);
                        }
                    }
                }
            }

            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using ZegaFinancials.Business.Support.Parser;

namespace ZegaFinancials.Business.Support.SpecificationsImport
{
    internal static class XmlParserHelper
    {
        public static void FillDataWithElements<T>(IDictionary<string, Mappings> propertyMapping, XElement element, T target)
        {
            foreach (var key in propertyMapping.Keys)
            {
                 var propertyInfo = typeof(T).GetProperty(key);
                var elementExist = false;
                foreach (var elementName in propertyMapping[key].XmlFields)
                {
                    var el = element.Element(elementName);
                    if (el == null)
                        continue;
                    elementExist = true;
                    
                    if (propertyInfo == null) continue;
                    try
                    {
                        propertyInfo.SetValue(target, ChangeType(el.Value, propertyInfo.PropertyType), null);
                    }
                    catch (Exception)
                    {
                        throw new XmlParserException(string.Format("The value \"{0}\" is unknown.", el.Value));
                    }
                    
                    break;
                }
                if (!elementExist)
                    throw new XmlParserException(string.Format("Does not exist data for field \"{0}\"", key));
            }
        }

        internal static object ChangeType(string value, Type type)
        {
            object val = null;
            if (type.IsSubclassOf(typeof(Enum)))
                val = Enum.Parse(type, value);
            else if (IsNullableType(type))
                val = ChangeType(value, GetNonNullableType(type));
            else if (type == typeof(bool))
            {
                int t;
                if (int.TryParse(value, out t))
                    val = Convert.ChangeType(t, typeof(bool));
                else
                    val = Convert.ChangeType(value, typeof(bool));
            }
            else if (type == typeof(int) || type == typeof(Int64))
            {
                string buf;
                if (value.IndexOf(",") > -1)
                {
                    var str = value.Split(',');
                    buf = str[0];
                }
                else
                    buf = value;

                if (string.IsNullOrEmpty(value))
                {
                    buf = "0";
                }

                val = Convert.ChangeType(buf, type);
            }
            else if (type == typeof(decimal))
            {
                var sep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                val = string.IsNullOrEmpty(value) ? decimal.Zero : Convert.ChangeType(value.Replace(".", sep).Replace(",", sep), type);
            }
            else if (type == typeof(DateTime))
            {
                if (!string.IsNullOrEmpty(value))
                    val = DateTime.Parse(value, CultureInfo.GetCultureInfo("en-US"));
            }
            else
            {
                val = Convert.ChangeType(value, type);
            }

            return val;
        }

        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        private static Type GetNonNullableType(Type type)
        {
            return IsNullableType(type) ? type.GetGenericArguments()[0] : type;
        }

    }
}

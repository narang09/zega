using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ZegaFinancials.Services.Shared.Utils
{
    public class EnumFunctions
    {
        public static string GetNameEnumByValue<K>(int value, bool isDescRequired = true)
        {
            if (typeof(K).BaseType != typeof(Enum))
            {
                throw new InvalidCastException();
            }

            string enumDesc = null;
            var memInfo = typeof(K).GetMember(Enum.GetName(typeof(K), value));
            var descriptionAttributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (descriptionAttributes.Length > 0 && isDescRequired)
            {
                // we're only getting the first description we find
                // others will be ignored
                enumDesc = ((DescriptionAttribute)descriptionAttributes[0]).Description;
            }

            return string.IsNullOrEmpty(enumDesc) ? Enum.GetName(typeof(K), value) : enumDesc;
        }

        public static List<Type> GetAllEnumTypes()
        {
            var enumTypes = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(o => o.FullName.StartsWith("Zega")))
            {
                enumTypes.AddRange(assembly.GetTypes()
                    .Where(t => t.IsEnum && t.IsPublic).ToList());
            }

            return enumTypes;
        }

        public static Type GetEnumTypeByName(string enumName, List<Type> allEnumTypes)
        {
            foreach (var enumType in allEnumTypes)
                if (enumType.Name == enumName)
                    return enumType;
            throw new Exception(string.Format("No such {0} enum found.", enumName));
        }

        public static List<int> GetEnumValuesBySearchVal(Type K, string searchVal, bool isDescRequired = true)
        {
            var enumValues = new List<int>();

            if (K.BaseType != typeof(Enum))
            {
                throw new InvalidCastException();
            }

            foreach (var memberName in Enum.GetNames(K))
            {
                string enumDesc = null, stringToValidate = "";
                int value = (int)Enum.Parse(K, memberName);

                var memInfo = K.GetMember(memberName);

                var descriptionAttributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (descriptionAttributes.Length > 0 && isDescRequired)
                {
                    // we're only getting the first description we find
                    // others will be ignored
                    enumDesc = ((DescriptionAttribute)descriptionAttributes[0]).Description;
                }

                if (enumDesc == null)
                    stringToValidate = memberName;
                else
                    stringToValidate = enumDesc;

                if (stringToValidate.ToLower().Contains(searchVal.ToLower()))
                    enumValues.Add(value);
            }
            return enumValues;
        }
        public static Dictionary<string, int> getEnumMembers(Type K)
        {

            bool isDescRequired = true;
            var enumValues = new Dictionary<string, int>();
            foreach (var memberName in Enum.GetNames(K))
            {
                string enumDesc = null, stringToValidate = "";
                int value = (int)Enum.Parse(K, memberName);
                var memInfo = K.GetMember(memberName);
                var descriptionAttributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (descriptionAttributes.Length > 0 && isDescRequired)
                {
                    enumDesc = ((DescriptionAttribute)descriptionAttributes[0]).Description;
                }
                if (enumDesc == null)
                    stringToValidate = memberName;
                else
                    stringToValidate = enumDesc;

                enumValues.Add(stringToValidate.ToLower(), value);
            }
            return enumValues;
        }

    }
}

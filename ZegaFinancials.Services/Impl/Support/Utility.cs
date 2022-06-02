using System;
using System.Collections.Generic;
using System.Linq;
using ZegaFinancials.Services.Shared.Utils;

namespace ZegaFinancials.Services.Impl.Support
{
   public class Utility
    {
        public static Dictionary<string, int> ConvertEnumToDictionary<K>(bool isDescRequired = true)
        {
            if (typeof(K).BaseType != typeof(Enum))
            {
                throw new InvalidCastException();
            }
            return Enum.GetValues(typeof(K)).Cast<int>().ToDictionary(currentItem => EnumFunctions.GetNameEnumByValue<K>(currentItem));
        }
    }
}

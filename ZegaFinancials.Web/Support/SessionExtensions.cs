using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;

namespace ZegaFinancials.Web.Support
{
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, String key, T Value)
        {
            session.SetString(key, JsonConvert.SerializeObject(Value));
        }

        public static T Get<T>(this ISession session, String key)
        {
            var value = session.GetString(key);
            return value == null ?  default : JsonConvert.DeserializeObject<T>(value);
        }
    }
}

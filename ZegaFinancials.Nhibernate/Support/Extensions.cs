using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace ZegaFinancials.Nhibernate.Support
{
    public static class Extensions
    {	
		public static global::NHibernate.IQuery SetParameters(this global::NHibernate.IQuery query, Dictionary<string, object> keyValues)
		{
			foreach (var paramName in keyValues.Keys)
			{
				var val = keyValues[paramName];	
				if (query.QueryString.Contains(paramName))
				{
					if (val is ICollection)
						query.SetParameterList(paramName, (ICollection)val);
					
					else
						query.SetParameter(paramName, val);
					//if (val is not ICollection)
					//	query.SetParameter(paramName, val);
					//else if(val)
					//query.SetParameterList(paramName, (ICollection)val);
					//else
					//	query.SetParameter(paramName, val);
				}
			}
			return query;
		}

		public static void Append<K, V>(this Dictionary<K, V> first, Dictionary<K, V> second)
		{
			foreach (KeyValuePair<K, V> item in second)
			{
				first[item.Key] = item.Value;
			}
		}

	}
}

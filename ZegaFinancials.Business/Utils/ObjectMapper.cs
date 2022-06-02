using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace ZegaFinancials.Business.Utils
{
	public static class ObjectMapper
    {
		public static void Map<S, D>(S s, D d, out PropertyChange[] changes)
		{
			ObjectMapperT<S, D>.Apply(s, d, out changes);
		}

		public static void Map<S, D>(S s, D d)
		{
			PropertyChange[] pcs;
			ObjectMapperT<S, D>.Apply(s, d, out pcs);
		}

		internal class ObjectMapperT<Source, Destination>
		{
			private static IList<PropertyMatch> propertyMatches = new List<PropertyMatch>();

			public static void FillPropertyMatches()
			{
				lock (propertyMatches)
				{
					propertyMatches.Clear();
					var sourceProps = typeof(Source).GetProperties();
					var destinationProps = typeof(Destination).GetProperties();

					foreach (var sourceProp in sourceProps)
					{
						var sourceType = sourceProp.PropertyType;
						var sourceTypeIsEnum = (sourceType.IsGenericType
												&& sourceType.GetGenericTypeDefinition() == typeof(Nullable<>)
												&& sourceType.GetGenericArguments()[0].IsEnum) || sourceType.IsEnum;

						foreach (var destinationProp in destinationProps)
						{
							var destinationType = destinationProp.PropertyType;
							var destinationTypeIsEnum = (destinationType.IsGenericType
												&& destinationType.GetGenericTypeDefinition() == typeof(Nullable<>)
												&& destinationType.GetGenericArguments()[0].IsEnum) || destinationType.IsEnum;

							if (sourceProp.Name.Equals(destinationProp.Name, StringComparison.InvariantCultureIgnoreCase)
								&& destinationProp.CanWrite
								&& (sourceType.Equals(destinationType)
								|| sourceTypeIsEnum && destinationTypeIsEnum)
								&& sourceProp.GetCustomAttributes(typeof(ReadOnlyAttribute), true).Length == 0
							/*and writeonly*/ //|| CheckNullableEnumType(sourceProp.PropertyType, destinationProp.PropertyType).HasValue)
							)
								propertyMatches.Add(new PropertyMatch(sourceProp, destinationProp));
						}
					}
				}
			}

			static ObjectMapperT()
			{
				FillPropertyMatches();
			}
			
			public static void Apply(Source source, Destination destination, out PropertyChange[] changes)
			{

				if (source == null)
					throw new ArgumentNullException("source", "source cannot be null");

				List<PropertyChange> pcs = new List<PropertyChange>();

				foreach (var propertyMatch in propertyMatches)
				{
					var sourceVal = propertyMatch.SourceProperty.GetValue(source, null);
					var destVal = propertyMatch.DestinationProperty.GetValue(destination, null);

					if (!object.Equals(sourceVal, destVal))
					{
						var destinationTypeIsEnum = (propertyMatch.DestinationProperty.PropertyType.IsGenericType
												&& propertyMatch.DestinationProperty.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
												&& propertyMatch.DestinationProperty.PropertyType.GetGenericArguments()[0].IsEnum) || propertyMatch.DestinationProperty.PropertyType.IsEnum;
						var destinationTypeIsNullable = propertyMatch.DestinationProperty.PropertyType.IsGenericType && propertyMatch.DestinationProperty.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);

						var sourceTypeIsEnum = (propertyMatch.SourceProperty.PropertyType.IsGenericType
												&& propertyMatch.SourceProperty.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
												&& propertyMatch.SourceProperty.PropertyType.GetGenericArguments()[0].IsEnum) || propertyMatch.SourceProperty.PropertyType.IsEnum;
						var sourceTypeIsNullable = propertyMatch.SourceProperty.PropertyType.IsGenericType && propertyMatch.SourceProperty.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);

						if (destinationTypeIsEnum && sourceTypeIsEnum)
						{
							if (sourceTypeIsNullable && !destinationTypeIsNullable)
							{
								if ((sourceVal == null || (int)sourceVal == 0))
									propertyMatch.DestinationProperty.SetValue(destination, 0, null);
								else
									propertyMatch.DestinationProperty.SetValue(destination, sourceVal, null);
							}
							else if (/*!sourceTypeIsNullable &&*/ destinationTypeIsNullable)
							{
								if ((sourceVal == null || (int)sourceVal == 0))
									propertyMatch.DestinationProperty.SetValue(destination, null, null);
								else
								{
									var value = Enum.ToObject(System.Nullable.GetUnderlyingType(propertyMatch.DestinationProperty.PropertyType), (int)sourceVal);
									var value1 = System.Activator.CreateInstance(propertyMatch.DestinationProperty.PropertyType, value);
									propertyMatch.DestinationProperty.SetValue(destination, value1, null);
								}
							}
							else
								propertyMatch.DestinationProperty.SetValue(destination, sourceVal, null);
						}
						else
							propertyMatch.DestinationProperty.SetValue(destination, sourceVal, null);

						if (!(destinationTypeIsEnum && sourceTypeIsEnum && Convert.ToInt32(sourceVal) == Convert.ToInt32(destVal)))
							pcs.Add(new PropertyChange() { Property = propertyMatch.DestinationProperty, PreviousValue = destVal, NewValue = sourceVal });
					}
				}
				changes = pcs.ToArray();

			}

			public struct PropertyMatch
			{
				public PropertyInfo SourceProperty;
				public PropertyInfo DestinationProperty;

				public PropertyMatch(PropertyInfo sourceProp, PropertyInfo destinationProp)
				{
					SourceProperty = sourceProp;
					DestinationProperty = destinationProp;
				}
			}
		}
	}
	public struct PropertyChange
	{
		public PropertyInfo Property { get; set; }
		public object PreviousValue { get; set; }
		public object NewValue { get; set; }
	}

}

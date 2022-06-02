using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using ZegaFinancials.Business.Support.Parser;

namespace ZegaFinancials.Business.Impl.SpecificationsImport
{
    public class ImportDecimalConverter : DecimalConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            decimal d;
            if (string.IsNullOrEmpty(text))
                return null;
            bool isParsed = Decimal.TryParse(text, out d);
            if (!isParsed)
                throw new CsvParserException(String.Format("Error : Decimal Conversion "+"{0}", text));
            return d;
        }
    }
    public class ImportDateTimeConverter : DateTimeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            DateTime dt;
            if (string.IsNullOrEmpty(text))
                return null;

            var isParsed = DateTime.TryParse(text, out dt);
            if (!isParsed)
                throw new CsvParserException(string.Format("Error : DateTime Conversion "+"{0}",text));
            return dt;
        }
    }
    public class ImportEnumConverter<T> : TypeConverter where T : struct
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrEmpty(text))
                return null;
           foreach (var field in typeof(T).GetFields())
           {
                var attribute= Attribute.GetCustomAttribute(field, typeof(System.ComponentModel.DescriptionAttribute)) as System.ComponentModel.DescriptionAttribute;
               
                  if (attribute!= null && attribute.Description == text)
                            return (T)field.GetValue(null);
                    else
                        if (field.Name == text)
                            return (T)field.GetValue(null);
                    
           }   
                throw new CsvParserException(string.Format("Error : Enum Conversion " + "{0}", text));
        }
        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            return string.Empty;
        }
    }
}

















 
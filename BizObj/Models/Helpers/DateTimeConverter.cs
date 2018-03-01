using System;
using System.Data.SqlTypes;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BizObj.Models.Helpers
{
    public class DateTimeConvertorCustome : DateTimeConverterBase
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            DateTime dateTime;

            if (String.IsNullOrWhiteSpace(reader.Value.ToString()))
                return DateTime.MinValue;
            if (DateTime.TryParse(reader.Value.ToString(), CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal, out dateTime))
                return dateTime;
            
            return DateTime.Parse(reader.Value.ToString());
        }
        
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((DateTime)value).ToString(CultureInfo.CurrentCulture));
        }
    }
    public class DateTimeConvertorSql : DateTimeConverterBase
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            DateTime dateTime;

            if (String.IsNullOrWhiteSpace(reader.Value.ToString()))
                return (DateTime) SqlDateTime.MinValue;
            if (DateTime.TryParse(reader.Value.ToString(), CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal, out dateTime))
                return dateTime;

            return DateTime.Parse(reader.Value.ToString());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((DateTime)value).ToString(CultureInfo.CurrentCulture));
        }
    }
}
using System;
using Newtonsoft.Json;



namespace HIS.Desktop.Plugins.Library.TwoIDStorageIntegration
{
    public class DateOnlyConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime) || objectType == typeof(DateTime?);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTime dt)
            {
                writer.WriteValue(dt.ToString("yyyy-MM-dd"));
            }
            else
            {
                writer.WriteNull();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null || string.IsNullOrEmpty(reader.Value.ToString()))
                return objectType == typeof(DateTime?) ? (DateTime?)null : default(DateTime);

            return DateTime.Parse(reader.Value.ToString());
        }
    }
}
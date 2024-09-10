using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.WCF.JsonConvert
{
    public class JsonConvert
    {
        public static string Serialize<T>(T data)
        {
            string result = null;
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(data.GetType());
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.WriteObject(stream, data);
                result = Encoding.Default.GetString(stream.ToArray());
            }
            return result;
        }

        public static T Deserialize<T>(string json)
        {
            T result = Activator.CreateInstance<T>();
            using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(result.GetType());
                result = (T)serializer.ReadObject(stream);
            }
            return result;
        }
    }
}

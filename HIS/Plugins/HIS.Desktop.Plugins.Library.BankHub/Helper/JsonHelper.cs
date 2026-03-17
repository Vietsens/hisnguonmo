using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace HIS.Desktop.Plugins.Library.BankHub.Helper
{
    /// <summary>
    /// Helper xử lý JSON sử dụng JavaScriptSerializer (.NET 4.5 built-in).
    /// Không cần cài thêm package bên ngoài.
    /// </summary>
    internal static class JsonHelper
    {
        private static readonly JavaScriptSerializer _serializer;

        static JsonHelper()
        {
            _serializer = new JavaScriptSerializer();
            _serializer.MaxJsonLength = int.MaxValue;
        }

        /// <summary>Serialize object thành JSON string</summary>
        public static string Serialize(object obj)
        {
            if (obj == null) return "null";
            return _serializer.Serialize(obj);
        }

        /// <summary>Deserialize JSON string thành object</summary>
        public static T Deserialize<T>(string json)
        {
            if (string.IsNullOrEmpty(json)) return default(T);
            return _serializer.Deserialize<T>(json);
        }

        /// <summary>Deserialize JSON string thành Dictionary</summary>
        public static Dictionary<string, object> DeserializeToDictionary(string json)
        {
            return _serializer.Deserialize<Dictionary<string, object>>(json);
        }
    }
}

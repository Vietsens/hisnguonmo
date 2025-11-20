using HIS.Desktop.LocalStorage.HisConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ConnectWhoCnd
{
    internal class Configs
    {
        private const string NCD_CONFIG_KEY = "HIS.WHO_NCD.CONNECTION_INFO";
        private const string VALIDATE_DATA_CFG = "HIS.WHO_NCD.VALIDATE_DATA";

        internal static List<string> ICD_HIGH_BLOOD_PRESSURE = new List<string> { "I10", "I11", "I12", "I13", "I14", "I15" };
        internal static List<string> ICD_DIABETES_MELLITUS = new List<string> { "E10", "E11", "E12", "E13", "E14" };
        internal static List<string> SERVICE_CODE_DIABETES_MELLITUS = new List<string> { "23.0075.1494" };
        internal static List<string> SERVICE_CODE_DVHBA1C = new List<string> { "23.0075.1494" };

        public static bool IS_CONNECT;
        public static string API_NCD;
        public static string PROGRAM;
        public static string USERNAME;
        public static string PASSWORD;

        internal static void LoadConfig()
        {
            try
            {
                string ncdConfig = HisConfigs.Get<string>(NCD_CONFIG_KEY);
                API_NCD = Get(ncdConfig, 0, '|');
                PROGRAM = Get(ncdConfig, 1, '|');
                USERNAME = Get(ncdConfig, 2, '|');
                PASSWORD = Base64Decode(Get(ncdConfig, 3, '|'));
                string validates = HisConfigs.Get<string>(VALIDATE_DATA_CFG);
                if (!String.IsNullOrWhiteSpace(validates))
                {
                    //không bắt exception để đảm bảo trạng thái kết nối
                    VALIDATE_DATA data = Newtonsoft.Json.JsonConvert.DeserializeObject<VALIDATE_DATA>(validates);
                    if (data != null)
                    {
                        if (!String.IsNullOrWhiteSpace(data.THA))
                        {
                            ICD_HIGH_BLOOD_PRESSURE = data.THA.Split('|').ToList();
                        }
                        if (!String.IsNullOrWhiteSpace(data.DTD))
                        {
                            ICD_DIABETES_MELLITUS = data.DTD.Split('|').ToList();
                        }
                        if (!String.IsNullOrWhiteSpace(data.DVDTD))
                        {
                            SERVICE_CODE_DIABETES_MELLITUS = data.DVDTD.Split('|').ToList();
                        }
                        if (!String.IsNullOrWhiteSpace(data.DVHBA1C))
                        {
                            SERVICE_CODE_DVHBA1C = data.DVHBA1C.Split('|').ToList();
                        }
                    }
                }

                if (Utilities.NCDToken == null || Utilities.NCDToken.response == null)
                {
                    var acc = new { program = PROGRAM, username = USERNAME, password = PASSWORD };
                    Utilities.NCDToken = ApiConsumers.CreateRequest<Model.OLogin>("POST", API_NCD, "/api/v1/auth", acc);
                }

                IS_CONNECT = Utilities.NCDToken != null && Utilities.NCDToken.response != null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static string Get(string value, int index, char splitChar)
        {
            string user = "";
            try
            {
                if (!String.IsNullOrEmpty(value))
                {
                    var data = value.Split(splitChar);
                    if (data != null && data.Length >= index)
                    {
                        user = data[index].Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                user = "";
            }
            return user;
        }

        static string Base64Decode(string base64EncodedData)
        {
            if (base64EncodedData != null)
            {
                int diff = base64EncodedData.Length % 4;
                if (diff != 0)
                {
                    for (int i = 0; i < diff; i++)
                    {
                        base64EncodedData += "=";
                    }
                }
            }
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }

    internal class VALIDATE_DATA
    {
        /// <summary>
        /// Đái tháo đường
        /// </summary>
        public string DTD { get; set; }
        /// <summary>
        /// Tăng Huyết Áp
        /// </summary>
        public string THA { get; set; }
        /// <summary>
        /// Dịch vụ Đái tháo đường
        /// </summary>
        public string DVDTD { get; set; }
        /// <summary>
        /// Dịch vụ Đái tháo đường
        /// </summary>
        public string DVHBA1C { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ConnectWhoCnd
{
    internal class Utilities
    {
        public static Model.OLogin NCDToken { get; set; }

        public static bool IsBATHA(List<string> data)
        {
            try
            {
                if (Configs.ICD_HIGH_BLOOD_PRESSURE == null || Configs.ICD_HIGH_BLOOD_PRESSURE.Count == 0) { throw new Exception("Chưa khai thông số ICD bệnh Tăng huyết áp"); }
                if (Configs.ICD_HIGH_BLOOD_PRESSURE.Exists(e => data.Exists(c => c.StartsWith(e)))) { return true; }

                return false;
            }
            catch { return false; }
        }

        public static bool IsBADTD(List<string> data)
        {
            try
            {
                if (Configs.ICD_DIABETES_MELLITUS == null || Configs.ICD_DIABETES_MELLITUS.Count == 0) { throw new Exception("Chưa khai thông số ICD bệnh Đái tháo đường"); }
                if (Configs.ICD_DIABETES_MELLITUS.Exists(e => data.Exists(c => c.StartsWith(e)))) { return true; }

                return false;
            }
            catch { return false; }
        }
    }
}

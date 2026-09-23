using HIS.Desktop.LocalStorage.HisConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Config
{
    public class HisConfigCFG
    {
        private const string KEY__DisablePartExamByExecutor = "HIS.Desktop.Plugins.EnterKskInfomantionVer2.DisablePartExamByExecutor";
        private const string KEY__CheckReq = "HIS.Desktop.Plugins.EnterKskInfomantionVer2.CheckReq";

        internal static string DisablePartExamByExecutor;
        /// <summary>"1" = khi kết thúc khám (bấm nút hoặc tự động sau Lưu) cảnh báo nếu còn dịch vụ có y lệnh chưa hoàn thành.</summary>
        internal static string CheckReq;

        internal static void LoadConfig()
        {
            try
            {
                DisablePartExamByExecutor = HisConfigs.Get<string>(KEY__DisablePartExamByExecutor);
                CheckReq = HisConfigs.Get<string>(KEY__CheckReq);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

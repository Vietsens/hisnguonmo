/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseSyncList
 * Cấu hình kết nối cổng ECDS — đọc từ HisConfigs (toàn viện). KHÔNG hardcode.
 */
using System;

namespace HIS.Desktop.Plugins.InfectiousDiseaseSyncList.Config
{
    internal class EcdsConfigCFG
    {
        internal static string BaseUrl;          // VD: https://daotao-gs.vadp.gov.vn
        internal static string Username;
        internal static string Password;
        internal static string MaDonVi;          // Mã đơn vị báo cáo (REPORTER_ORG)
        internal static string MaCoSoDieuTri;    // Mã cơ sở điều trị
        internal static int TimeoutSecond = 60;

        internal static void LoadConfig()
        {
            try
            {
                BaseUrl = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("ECDS.API.BASE_URL");
                Username = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("ECDS.API.USERNAME");
                Password = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("ECDS.API.PASSWORD");
                MaDonVi = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("ECDS.API.MA_DON_VI");
                MaCoSoDieuTri = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("ECDS.API.MA_CO_SO_DIEU_TRI");

                int t = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>("ECDS.API.TIMEOUT_SECOND");
                if (t > 0) TimeoutSecond = t;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(BaseUrl)
                && !string.IsNullOrWhiteSpace(Username)
                && !string.IsNullOrWhiteSpace(Password);
        }
    }
}

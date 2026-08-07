/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseReport
 * Cấu hình kết nối cổng ECDS — đọc từ HisConfigs (toàn viện). KHÔNG hardcode.
 * Dùng 1 key gộp (mẫu KSK 2062): MOS.HIS_ECDS_SYNC.ECDS_CONNECTION_INFO
 * Giá trị pipe 8 phần: MaDonVi|MaCoSoDieuTri|Username|Password|MaTinh|BaseUrl|LoginPath|PushPath
 * VD: 30045|30045001|bvdakhoa_tichhop|MatKhau@123|01|https://daotao-gs.vadp.gov.vn|/api/fast/v1/auth/login|/api/fast/v1/ca-benh/cap-nhat
 */
using System;

namespace HIS.Desktop.Plugins.InfectiousDiseaseReport.Config
{
    internal class EcdsConfigCFG
    {
        /// <summary>Key HIS_CONFIG gộp toàn bộ thông tin liên thông ECDS.</summary>
        internal const string CONFIG_KEY = "MOS.HIS_ECDS_SYNC.ECDS_CONNECTION_INFO";

        internal static string MaDonVi;          // [0] Mã đơn vị báo cáo
        internal static string MaCoSoDieuTri;    // [1] Mã cơ sở điều trị
        internal static string Username;         // [2]
        internal static string Password;         // [3]
        internal static string MaTinh;           // [4] Mã tỉnh
        internal static string BaseUrl;          // [5] VD: https://daotao-gs.vadp.gov.vn
        internal static string LoginPath;        // [6] VD: /api/fast/v1/auth/login
        internal static string PushPath;         // [7] VD: /api/fast/v1/ca-benh/cap-nhat
        internal static int TimeoutSecond = 60;

        internal static void LoadConfig()
        {
            try
            {
                string raw = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY);
                if (string.IsNullOrWhiteSpace(raw)) return;

                string[] p = raw.Split('|');
                MaDonVi = Part(p, 0);
                MaCoSoDieuTri = Part(p, 1);
                Username = Part(p, 2);
                Password = Part(p, 3);
                MaTinh = Part(p, 4);
                BaseUrl = Part(p, 5);
                LoginPath = Part(p, 6);
                PushPath = Part(p, 7);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private static string Part(string[] arr, int i)
        {
            return (arr != null && i < arr.Length && arr[i] != null) ? arr[i].Trim() : null;
        }

        internal static bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(BaseUrl)
                && !string.IsNullOrWhiteSpace(Username)
                && !string.IsNullOrWhiteSpace(Password);
        }
    }
}

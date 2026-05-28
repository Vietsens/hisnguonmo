using System;
using System.Resources;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute2.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage =
            new System.Resources.ResourceManager(
                "HIS.Desktop.Plugins.SurgServiceReqExecute2.Resources.Message.Lang",
                System.Reflection.Assembly.GetExecutingAssembly());

        internal static string BanCoMuonHuyBatDauKhong
        {
            get { return GetValue("BanCoMuonHuyBatDauKhong"); }
        }

        internal static string BanCoMuonHuyKetThucKhong
        {
            get { return GetValue("BanCoMuonHuyKetThucKhong"); }
        }

        internal static string YLenhDaTonTaiVanBanKy
        {
            get { return GetValue("YLenhDaTonTaiVanBanKy"); }
        }

        internal static string KhongCoNoiDungLuuMau
        {
            get { return GetValue("KhongCoNoiDungLuuMau"); }
        }

        internal static string ChuaChonYLenh
        {
            get { return GetValue("ChuaChonYLenh"); }
        }

        internal static string TaiKhoanKhongPhaiBacSi
        {
            get { return GetValue("TaiKhoanKhongPhaiBacSi"); }
        }

        internal static string TongSoBN
        {
            get { return GetValue("TongSoBN"); }
        }

        internal static string TongSoDichVu
        {
            get { return GetValue("TongSoDichVu"); }
        }

        internal static string HuyBatDau
        {
            get { return GetValue("HuyBatDau"); }
        }

        internal static string HuyKetThuc
        {
            get { return GetValue("HuyKetThuc"); }
        }

        internal static string ChucNangLuuMauChuaKhaDung
        {
            get { return GetValue("ChucNangLuuMauChuaKhaDung"); }
        }

        internal static string DichVuChuaThucHienKhongChoKetThuc
        {
            get { return GetValue("DichVuChuaThucHienKhongChoKetThuc"); }
        }

        private static string GetValue(string key)
        {
            try
            {
                return Inventec.Common.Resource.Get.Value(
                    key,
                    languageMessage,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }
    }
}

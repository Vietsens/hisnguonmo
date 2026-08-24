using System;

namespace HIS.Desktop.Plugins.HisImpMestMediMate.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager(
            "HIS.Desktop.Plugins.HisImpMestMediMate.Resources.Message.Lang",
            System.Reflection.Assembly.GetExecutingAssembly());

        private static string GetValue(string key)
        {
            try
            {
                return Inventec.Common.Resource.Get.Value(
                    key, languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }

        internal static string ThongBao
        {
            get { return GetValue("ThongBao"); }
        }

        /// <summary>Không tồn tại hóa đơn tương ứng</summary>
        internal static string KhongTonTaiHoaDonTuongUng
        {
            get { return GetValue("KhongTonTaiHoaDonTuongUng"); }
        }

        internal static string BanChuaChonThuoc
        {
            get { return GetValue("BanChuaChonThuoc"); }
        }

        internal static string BanChuaChonVatTu
        {
            get { return GetValue("BanChuaChonVatTu"); }
        }

        internal static string KhoangThoiGianKhongHopLe
        {
            get { return GetValue("KhoangThoiGianKhongHopLe"); }
        }

        /// <summary>Tổng số dòng tìm được: {0}</summary>
        internal static string TongSoDongTimDuoc
        {
            get { return GetValue("TongSoDongTimDuoc"); }
        }

        internal static string XuatFileThanhCongBanCoMuonMoFile
        {
            get { return GetValue("XuatFileThanhCongBanCoMuonMoFile"); }
        }

        internal static string XuLyThatBai
        {
            get { return GetValue("XuLyThatBai"); }
        }
    }
}

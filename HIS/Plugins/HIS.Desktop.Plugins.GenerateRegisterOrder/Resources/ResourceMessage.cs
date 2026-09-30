using System;

namespace HIS.Desktop.Plugins.GenerateRegisterOrder.Resources
{
    class ResourceMessage
    {
        internal static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.GenerateRegisterOrder.Resources.Message", System.Reflection.Assembly.GetExecutingAssembly());

        /// <summary>Doc mot khoa trong bo Message theo ngon ngu dang dung</summary>
        private static string GetValue(string key)
        {
            try
            {
                return Inventec.Common.Resource.Get.Value(key, languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }

        /// <summary>So thu tu bat dau khong duoc phep nho hon 0</summary>
        internal static string SoThuTuBatDauKhongDuocPhepNhoHonKhong
        {
            get { return GetValue("SoThuTuBatDauKhongDuocPhepNhoHonKhong"); }
        }

        /// <summary>Tieu de popup chon hinh thuc lay so</summary>
        internal static string ChonHinhThucLaySo
        {
            get { return GetValue("ChonHinhThucLaySo"); }
        }

        /// <summary>Nhan o quet ma QR the can cuoc</summary>
        internal static string QuetQrTheCccd
        {
            get { return GetValue("QuetQrTheCccd"); }
        }

        /// <summary>Nhan o quet ma QR tren ung dung VNeID</summary>
        internal static string QuetQrVneId
        {
            get { return GetValue("QuetQrVneId"); }
        }

        /// <summary>Nhan o nhap tay so CCCD</summary>
        internal static string NhapTaySoCccd
        {
            get { return GetValue("NhapTaySoCccd"); }
        }

        /// <summary>Nhan o the bao hiem y te</summary>
        internal static string TheBhyt
        {
            get { return GetValue("TheBhyt"); }
        }

        /// <summary>Nhan o khong co giay to</summary>
        internal static string KhongCoGiayTo
        {
            get { return GetValue("KhongCoGiayTo"); }
        }

        /// <summary>Nhan nut quay lai buoc chon hinh thuc</summary>
        internal static string QuayLai
        {
            get { return GetValue("QuayLai"); }
        }

        /// <summary>Nhan nut xac nhan thong tin dinh danh</summary>
        internal static string XacNhan
        {
            get { return GetValue("XacNhan"); }
        }

        /// <summary>Huong dan khi chon quet ma QR the can cuoc</summary>
        internal static string HuongDanQuetQrTheCccd
        {
            get { return GetValue("HuongDanQuetQrTheCccd"); }
        }

        /// <summary>Huong dan khi chon quet ma QR tren ung dung VNeID</summary>
        internal static string HuongDanQuetQrVneId
        {
            get { return GetValue("HuongDanQuetQrVneId"); }
        }

        /// <summary>Huong dan khi chon nhap tay so CCCD</summary>
        internal static string HuongDanNhapTaySoCccd
        {
            get { return GetValue("HuongDanNhapTaySoCccd"); }
        }

        /// <summary>Huong dan khi chon the bao hiem y te</summary>
        internal static string HuongDanTheBhyt
        {
            get { return GetValue("HuongDanTheBhyt"); }
        }

        /// <summary>So CCCD khong hop le, vui long quet hoac nhap lai</summary>
        internal static string SoCccdKhongHopLe
        {
            get { return GetValue("SoCccdKhongHopLe"); }
        }

        /// <summary>Ma the BHYT khong hop le, vui long quet hoac nhap lai</summary>
        internal static string MaTheBhytKhongHopLe
        {
            get { return GetValue("MaTheBhytKhongHopLe"); }
        }

        /// <summary>Tien to dai thong tin dang lay so cho ai</summary>
        internal static string DangLaySoCho
        {
            get { return GetValue("DangLaySoCho"); }
        }

        /// <summary>Dai thong tin khi nguoi benh chon khong co giay to</summary>
        internal static string LaySoKhongDinhDanh
        {
            get { return GetValue("LaySoKhongDinhDanh"); }
        }

        /// <summary>Nhan nut huy thong tin dinh danh dang giu</summary>
        internal static string HuyChonLai
        {
            get { return GetValue("HuyChonLai"); }
        }

        /// <summary>Nhan nut in lai phieu so thu tu da cap</summary>
        internal static string InLaiPhieu
        {
            get { return GetValue("InLaiPhieu"); }
        }

        /// <summary>
        /// Thong bao da lay du so trong ngay.
        /// Tham so 0 la so thu tu da cap, tham so 1 la ten day.
        /// </summary>
        internal static string BanDaLayDuSoTrongNgay
        {
            get { return GetValue("BanDaLayDuSoTrongNgay"); }
        }

        /// <summary>Nhac nguoi benh chon hinh thuc lay so truoc khi chon day</summary>
        internal static string VuiLongChonHinhThucLaySo
        {
            get { return GetValue("VuiLongChonHinhThucLaySo"); }
        }
    }
}

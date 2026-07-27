/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using System;
using System.Resources;

namespace HIS.Desktop.Plugins.CoordinationServiceReqCLS.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage =
            new System.Resources.ResourceManager(
                "HIS.Desktop.Plugins.CoordinationServiceReqCLS.Resources.Message.Lang",
                System.Reflection.Assembly.GetExecutingAssembly());

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

        /// <summary>Chưa xử lý</summary>
        internal static string ChuaXuLy { get { return GetValue("ChuaXuLy"); } }

        /// <summary>Đã xử lý</summary>
        internal static string DaXuLy { get { return GetValue("DaXuLy"); } }

        /// <summary>Đã xem</summary>
        internal static string DaXem { get { return GetValue("DaXem"); } }

        /// <summary>Thời gian từ không được bỏ trống</summary>
        internal static string ThoiGianTuKhongDuocBoTrong { get { return GetValue("ThoiGianTuKhongDuocBoTrong"); } }

        /// <summary>Thời gian từ phải nhỏ hơn hoặc bằng thời gian đến</summary>
        internal static string ThoiGianTuPhaiNhoHonDen { get { return GetValue("ThoiGianTuPhaiNhoHonDen"); } }

        /// <summary>Vui lòng nhập số giây tự động làm mới hợp lệ</summary>
        internal static string VuiLongNhapSoGiayHopLe { get { return GetValue("VuiLongNhapSoGiayHopLe"); } }

        /// <summary>Y lệnh không tồn tại hoặc đã bị xóa/khóa</summary>
        internal static string YLenhKhongTonTai { get { return GetValue("YLenhKhongTonTai"); } }

        /// <summary>Chưa có kết quả để xem cho dịch vụ này</summary>
        internal static string ChuaCoKetQuaDeXem { get { return GetValue("ChuaCoKetQuaDeXem"); } }

        /// <summary>Chưa thực hiện</summary>
        internal static string TrangThaiChuaThucHien { get { return GetValue("TrangThaiChuaThucHien"); } }

        /// <summary>Đang thực hiện</summary>
        internal static string TrangThaiDangThucHien { get { return GetValue("TrangThaiDangThucHien"); } }

        /// <summary>Đủ kết quả</summary>
        internal static string TrangThaiDuKetQua { get { return GetValue("TrangThaiDuKetQua"); } }

        /// <summary>Bình thường</summary>
        internal static string CanhBaoBinhThuong { get { return GetValue("CanhBaoBinhThuong"); } }

        /// <summary>Bất thường</summary>
        internal static string CanhBaoBatThuong { get { return GetValue("CanhBaoBatThuong"); } }

        /// <summary>Vượt ngưỡng</summary>
        internal static string CanhBaoVuotNguong { get { return GetValue("CanhBaoVuotNguong"); } }
    }
}

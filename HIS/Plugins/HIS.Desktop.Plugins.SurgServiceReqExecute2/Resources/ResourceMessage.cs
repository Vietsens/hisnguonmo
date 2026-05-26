/* IVT
 * @Project : hisnguonmo
 * Việc 45072 — Thông báo riêng plugin
 */
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

        /// <summary>Bạn có muốn hủy bắt đầu y lệnh này không?</summary>
        internal static string BanCoMuonHuyBatDauKhong
        {
            get { return GetValue("BanCoMuonHuyBatDauKhong"); }
        }

        /// <summary>Bạn có muốn hủy kết thúc y lệnh này không?</summary>
        internal static string BanCoMuonHuyKetThucKhong
        {
            get { return GetValue("BanCoMuonHuyKetThucKhong"); }
        }

        /// <summary>Y lệnh đã tồn tại văn bản ký số. Bạn có muốn xóa các văn bản đã ký để hủy kết thúc không?</summary>
        internal static string YLenhDaTonTaiVanBanKy
        {
            get { return GetValue("YLenhDaTonTaiVanBanKy"); }
        }

        /// <summary>Không có nội dung để lưu mẫu PTTT</summary>
        internal static string KhongCoNoiDungLuuMau
        {
            get { return GetValue("KhongCoNoiDungLuuMau"); }
        }

        /// <summary>Vui lòng chọn 1 y lệnh trước khi thực hiện</summary>
        internal static string ChuaChonYLenh
        {
            get { return GetValue("ChuaChonYLenh"); }
        }

        /// <summary>Tài khoản đăng nhập không phải bác sĩ, không cho phép kết thúc y lệnh</summary>
        internal static string TaiKhoanKhongPhaiBacSi
        {
            get { return GetValue("TaiKhoanKhongPhaiBacSi"); }
        }

        /// <summary>Tổng số BN: </summary>
        internal static string TongSoBN
        {
            get { return GetValue("TongSoBN"); }
        }

        /// <summary>Tổng số dịch vụ: </summary>
        internal static string TongSoDichVu
        {
            get { return GetValue("TongSoDichVu"); }
        }

        /// <summary>Hủy bắt đầu</summary>
        internal static string HuyBatDau
        {
            get { return GetValue("HuyBatDau"); }
        }

        /// <summary>Hủy kết thúc</summary>
        internal static string HuyKetThuc
        {
            get { return GetValue("HuyKetThuc"); }
        }

        /// <summary>Chức năng lưu mẫu chưa khả dụng (chưa nạp được FormPtttTemp).</summary>
        internal static string ChucNangLuuMauChuaKhaDung
        {
            get { return GetValue("ChucNangLuuMauChuaKhaDung"); }
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

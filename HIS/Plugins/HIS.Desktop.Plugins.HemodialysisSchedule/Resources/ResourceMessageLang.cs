/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using System;

namespace HIS.Desktop.Plugins.HemodialysisSchedule.Resources
{
    public class ResourceMessageLang
    {
        public static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.HemodialysisSchedule.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        private static string Get(string key)
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

        internal static string TatCa { get { return Get("TatCa"); } }

        /// <summary>Vui lòng chọn ngày xếp lịch.</summary>
        internal static string VuiLongChonNgayXepLich { get { return Get("VuiLongChonNgayXepLich"); } }

        /// <summary>Vui lòng chọn phòng chạy thận.</summary>
        internal static string VuiLongChonPhongChay { get { return Get("VuiLongChonPhongChay"); } }

        /// <summary>Vui lòng chọn ca chạy thận (1..5).</summary>
        internal static string VuiLongChonCa { get { return Get("VuiLongChonCa"); } }

        /// <summary>Vui lòng tích chọn ít nhất một bệnh nhân để đưa vào lịch.</summary>
        internal static string VuiLongChonItNhatMotBenhNhan { get { return Get("VuiLongChonItNhatMotBenhNhan"); } }

        /// <summary>Format: "Đã đưa {0} bệnh nhân vào lịch chạy thận."</summary>
        internal static string DuaVaoLichThanhCongFormat { get { return Get("DuaVaoLichThanhCongFormat"); } }

        /// <summary>Vui lòng chọn một slot lịch để xóa.</summary>
        internal static string VuiLongChonSlotDeXoa { get { return Get("VuiLongChonSlotDeXoa"); } }

        /// <summary>Bạn có chắc chắn muốn xóa slot lịch đã chọn?</summary>
        internal static string XacNhanXoaSlot { get { return Get("XacNhanXoaSlot"); } }

        /// <summary>Vui lòng chọn ngày nguồn để sao chép.</summary>
        internal static string VuiLongChonNgayNguon { get { return Get("VuiLongChonNgayNguon"); } }

        /// <summary>Ngày nguồn và ngày đích không được trùng nhau.</summary>
        internal static string NgayNguonVaDichTrung { get { return Get("NgayNguonVaDichTrung"); } }

        /// <summary>Không có bản ghi nào ở ngày nguồn để sao chép.</summary>
        internal static string KhongCoBanGhiDeSaoChep { get { return Get("KhongCoBanGhiDeSaoChep"); } }

        /// <summary>Format: "Đã sao chép {0} bệnh nhân mới sang ngày đích."</summary>
        internal static string SaoChepThanhCongFormat { get { return Get("SaoChepThanhCongFormat"); } }
    }
}

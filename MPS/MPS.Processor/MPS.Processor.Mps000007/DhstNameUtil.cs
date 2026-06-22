using System;

namespace MPS.Processor.Mps000007
{
    /// <summary>
    /// Mức độ ý thức (LOC) — ánh xạ với cột HIS_DHST.LOC.
    /// Giá trị lưu DB = SelectedIndex + 1 của combo nhập DHST (frmHisDhst.cboLoc).
    /// </summary>
    public enum EnumDhstLoc
    {
        /// <summary>Tỉnh táo</summary>
        Alert = 1,

        /// <summary>Lơ mơ</summary>
        Drowsy = 2,

        /// <summary>U ám</summary>
        Stupor = 3,

        /// <summary>Nửa hôn mê</summary>
        SemiComa = 4,

        /// <summary>Hôn mê</summary>
        Coma = 5
    }

    /// <summary>
    /// Thang điểm AVPU — ánh xạ với cột HIS_DHST.AVPU.
    /// Giá trị lưu DB = SelectedIndex + 1 của combo nhập DHST (frmHisDhst.cboAvpu).
    /// </summary>
    public enum EnumDhstAvpu
    {
        /// <summary>A - Tỉnh táo</summary>
        Alert = 1,

        /// <summary>V - Đáp ứng lời nói</summary>
        Verbal = 2,

        /// <summary>P - Đáp ứng đau</summary>
        Pain = 3,

        /// <summary>U - Không đáp ứng</summary>
        Unresponsive = 4
    }

    /// <summary>
    /// Tiện ích chuyển giá trị số LOC/AVPU của DHST sang tên hiển thị để in.
    /// Tên hiển thị phải khớp với danh sách combo trong frmHisDhst.
    /// </summary>
    internal static class DhstNameUtil
    {
        /// <summary>Tên hiển thị mức độ ý thức theo giá trị HIS_DHST.LOC.</summary>
        internal static string GetLocName(short? loc)
        {
            try
            {
                if (!loc.HasValue) return "";
                switch ((EnumDhstLoc)loc.Value)
                {
                    case EnumDhstLoc.Alert: return "Tỉnh táo";
                    case EnumDhstLoc.Drowsy: return "Lơ mơ";
                    case EnumDhstLoc.Stupor: return "U ám";
                    case EnumDhstLoc.SemiComa: return "Nửa hôn mê";
                    case EnumDhstLoc.Coma: return "Hôn mê";
                    default: return "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }

        /// <summary>Tên hiển thị thang AVPU theo giá trị HIS_DHST.AVPU.</summary>
        internal static string GetAvpuName(short? avpu)
        {
            try
            {
                if (!avpu.HasValue) return "";
                switch ((EnumDhstAvpu)avpu.Value)
                {
                    case EnumDhstAvpu.Alert: return "A - Tỉnh táo";
                    case EnumDhstAvpu.Verbal: return "V - Đáp ứng lời nói";
                    case EnumDhstAvpu.Pain: return "P - Đáp ứng đau";
                    case EnumDhstAvpu.Unresponsive: return "U - Không đáp ứng";
                    default: return "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }
    }
}

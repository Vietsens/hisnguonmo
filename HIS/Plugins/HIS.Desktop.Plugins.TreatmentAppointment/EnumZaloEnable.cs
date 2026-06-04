/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
namespace HIS.Desktop.Plugins.TreatmentAppointment
{
    /// <summary>
    /// Cấu hình MOS.SMS.ZALO_ENABLE — kiêm 2 vai trò: bật/tắt tính năng + chọn gateway.
    /// Mapping với key HIS_CONFIG.MOS.SMS.ZALO_ENABLE.
    /// </summary>
    internal enum EnumZaloEnable
    {
        /// <summary>TẮT tính năng gửi Zalo nhắc tái khám</summary>
        Disabled = 0,

        /// <summary>BẬT gateway OneSMS (CONEK)</summary>
        OneSms = 1,

        /// <summary>BẬT gateway FNS ZNS (FPT)</summary>
        FnsZns = 2
    }
}

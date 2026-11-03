/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.Library.KhoDuLieuYTe.ADO
{
    /// <summary>Dữ liệu 1 trẻ để gửi Giấy chứng sinh mẫu 01 (cùng bộ tham số với GenXmlGCSData).</summary>
    public class KdlBirthInputADO
    {
        public HIS_BRANCH Branch { get; set; }
        public V_HIS_BABY Baby { get; set; }
        public V_HIS_TREATMENT Treatment { get; set; }
    }

    /// <summary>Dữ liệu 1 hồ sơ tử vong để gửi Giấy báo tử (cùng bộ tham số với GenXmlGBTData).</summary>
    public class KdlDeathInputADO
    {
        public HIS_BRANCH Branch { get; set; }
        public HIS_PATIENT Patient { get; set; }
        public V_HIS_TREATMENT Treatment { get; set; }
        public HIS_SEVERE_ILLNESS_INFO DeathData { get; set; }
    }
}

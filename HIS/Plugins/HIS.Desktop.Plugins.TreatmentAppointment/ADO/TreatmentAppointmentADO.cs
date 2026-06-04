/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.TreatmentAppointment.ADO
{
    /// <summary>
    /// Mở rộng HIS_TREATMENT thêm trường IsSelected để bind checkbox multi-select trên grid.
    /// </summary>
    public class TreatmentAppointmentADO : HIS_TREATMENT
    {
        /// <summary>Đánh dấu chọn dòng để gửi tin nhắn Zalo nhắc tái khám</summary>
        public bool IsSelected { get; set; }
    }
}

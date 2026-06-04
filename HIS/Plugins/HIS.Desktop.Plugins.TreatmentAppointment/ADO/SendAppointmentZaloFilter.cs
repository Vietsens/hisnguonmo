/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.TreatmentAppointment.ADO
{
    /// <summary>
    /// Request body cho API POST /api/HisTreatment/SendAppointmentZalo.
    /// </summary>
    public class SendAppointmentZaloFilter
    {
        /// <summary>Danh sách ID điều trị muốn gửi tin nhắn Zalo nhắc tái khám</summary>
        public List<long> TreatmentIds { get; set; }

        /// <summary>Template Zalo được user chọn (TemplateId từ gateway)</summary>
        public string TemplateId { get; set; }
    }
}

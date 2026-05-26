/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Việc 45072 — Bổ sung property hiển thị cho 5 cột grid trái:
 *  - PATIENT_TYPE_NAME: ĐTTT (tên đối tượng thanh toán)
 *  - REQUEST_DOCTOR_DISPLAY: Bác sĩ chỉ định ghép từ TDL_REQUEST_USERNAME + TDL_REQUEST_LOGINNAME
 *  - BEGIN_TIME_STR: Thời gian bắt đầu format dd/MM/yyyy HH:mm
 *  - END_TIME_STR: Thời gian kết thúc format dd/MM/yyyy HH:mm
 * Giữ V_HIS_SERE_SERV_1 làm base (không đổi sang V8) — các field bổ sung được populate khi click row.
 */
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute2.ADO
{
    public class SereServView1ADO : V_HIS_SERE_SERV_1
    {
        public string GroupFieldName { get; set; }

        /// <summary>ĐTTT — Tên đối tượng thanh toán (lookup từ HIS_PATIENT_TYPE.PATIENT_TYPE_NAME).</summary>
        public string PATIENT_TYPE_NAME { get; set; }

        /// <summary>Bác sĩ chỉ định: "Họ tên (Loginname)" — ghép từ TDL_REQUEST_USERNAME + TDL_REQUEST_LOGINNAME.</summary>
        public string REQUEST_DOCTOR_DISPLAY { get; set; }

        /// <summary>Thời gian bắt đầu format dd/MM/yyyy HH:mm — từ BEGIN_TIME (long yyyyMMddHHmmss).</summary>
        public string BEGIN_TIME_STR { get; set; }

        /// <summary>Thời gian kết thúc format dd/MM/yyyy HH:mm — từ END_TIME.</summary>
        public string END_TIME_STR { get; set; }

        /// <summary>Đơn giá hiển thị — từ PRICE.</summary>
        public decimal? PRICE_V45072 { get; set; }

        public SereServView1ADO() { }

        public SereServView1ADO(V_HIS_SERE_SERV_1 data)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<SereServView1ADO>(this, data);
            GroupFieldName = string.Format("{0}: {1}", this.TDL_PATIENT_NAME, this.TDL_PATIENT_CODE);
        }
    }
}

/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * vCong60263 - Một dòng nhật ký gửi XML 130 đã định dạng sẵn để hiển thị và kết xuất.
 */
using System;
using System.Globalization;

namespace HIS.Desktop.Plugins.ExportXmlQD130.ADO
{
    public class Xml130SendLogADO
    {
        //Quy ước lưu trong bảng nhật ký
        public const short SEND_MODE_AUTO = 1;
        public const short SEND_MODE_MANUAL = 2;
        public const short SEND_RESULT_SUCCESS = 1;

        public long ID { get; set; }
        public int STT { get; set; }
        public long TREATMENT_ID { get; set; }
        public string TREATMENT_CODE { get; set; }
        public string PATIENT_CODE { get; set; }
        /// <summary>Thời điểm gửi dạng số yyyyMMddHHmmss - dùng để sắp xếp.</summary>
        public long SEND_TIME { get; set; }
        /// <summary>Thời điểm gửi đã định dạng để hiển thị.</summary>
        public string SEND_TIME_TEXT { get; set; }
        public string SEND_MODE_TEXT { get; set; }
        public string SEND_RESULT_TEXT { get; set; }
        public bool IS_SUCCESS { get; set; }
        public string SEND_DESC { get; set; }
        public string CHECK_CODE { get; set; }
        public string MACHINE_NAME { get; set; }
        public string SEND_ROUND_CODE { get; set; }
        public string CREATOR { get; set; }

        public static Xml130SendLogADO From(MOS.EFMODEL.DataModels.HIS_TREATMENT_XML_LOG raw, int stt)
        {
            Xml130SendLogADO ado = new Xml130SendLogADO();
            if (raw == null)
            {
                return ado;
            }
            ado.ID = raw.ID;
            ado.STT = stt;
            ado.TREATMENT_ID = raw.TREATMENT_ID;
            ado.TREATMENT_CODE = raw.TREATMENT_CODE;
            ado.PATIENT_CODE = raw.PATIENT_CODE;
            ado.SEND_TIME = raw.SEND_TIME;
            ado.SEND_TIME_TEXT = FormatTime(raw.SEND_TIME);
            ado.SEND_MODE_TEXT = raw.SEND_MODE.HasValue && raw.SEND_MODE.Value == SEND_MODE_AUTO
                ? "Tự động theo lịch"
                : "Gửi tay";
            ado.IS_SUCCESS = raw.SEND_RESULT.HasValue && raw.SEND_RESULT.Value == SEND_RESULT_SUCCESS;
            ado.SEND_RESULT_TEXT = ado.IS_SUCCESS ? "Thành công" : "Thất bại";
            ado.SEND_DESC = raw.SEND_DESC;
            ado.CHECK_CODE = raw.CHECK_CODE;
            ado.MACHINE_NAME = raw.MACHINE_NAME;
            ado.SEND_ROUND_CODE = raw.SEND_ROUND_CODE;
            ado.CREATOR = raw.CREATOR;
            return ado;
        }

        private static string FormatTime(long value)
        {
            try
            {
                if (value <= 0)
                {
                    return String.Empty;
                }
                DateTime? time = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(value);
                return time.HasValue
                    ? time.Value.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                    : String.Empty;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return String.Empty;
            }
        }
    }
}

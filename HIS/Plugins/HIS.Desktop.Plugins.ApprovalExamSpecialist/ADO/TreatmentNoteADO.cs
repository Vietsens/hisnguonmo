using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MOS.EFMODEL.DataModels;
using MOS.SDO;

namespace HIS.Desktop.Plugins.ApprovalExamSpecialist.ADO
{
    public class TreatmentNoteADO
    {
        public long Datetime { get; set; }
        public string DatetimeFormatted { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
        public string Medical_order { get; set; }
        public string ICD_CODE { get; set; }
        public string ICD_NAME { get; set; }
        public string ICD_SUB_CODE { get; set; }
        public string ICD_TEXT { get; set; }

        public TreatmentNoteADO(HIS_TRACKING tracking, V_HIS_EMPLOYEE employee, List<DHisSereServ2> allSereServ2, Dictionary<long, HIS_SERVICE_REQ> dicServiceReq)
        {
            try
            {
                if (tracking == null) return;

                Datetime = tracking.TRACKING_TIME;
                DateTime? trackingDateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(tracking.TRACKING_TIME);
                DatetimeFormatted = trackingDateTime.HasValue
                    ? trackingDateTime.Value.ToString("dd/MM/yyyy HH:mm")
                    : string.Empty;

                Content = tracking.CONTENT;

                if (employee != null && employee.LOGINNAME == tracking.CREATOR)
                {
                    UserName = employee.TDL_USERNAME + " - " + employee.DIPLOMA;
                }

                //Y lenh: thuoc, chi dinh can lam sang, y lenh chu cua to (cuoi o); khong hien ma y lenh, vat tu
                string medicalOrder = Tracking.TrackingOrderText.Build(allSereServ2, dicServiceReq, tracking.MEDICAL_INSTRUCTION);
                if (!string.IsNullOrEmpty(medicalOrder))
                {
                    Medical_order = medicalOrder;
                }

                ICD_CODE = tracking.ICD_CODE;
                ICD_NAME = tracking.ICD_NAME;
                ICD_SUB_CODE = tracking.ICD_SUB_CODE;
                ICD_TEXT = tracking.ICD_TEXT;

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

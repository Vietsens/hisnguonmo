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

        public TreatmentNoteADO(HIS_TRACKING tracking, V_HIS_EMPLOYEE employee, List<DHisSereServ2> allSereServ2)
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
                    UserName = employee.DIPLOMA + " - " + employee.TDL_USERNAME;
                }

                if (allSereServ2 != null && allSereServ2.Count > 0)
                {
                    var groupServiceReq = allSereServ2.GroupBy(o => o.SERVICE_REQ_CODE).ToList();
                    List<string> lst = new List<string>();
                    foreach (var item in groupServiceReq)
                    {
                        lst.Add(string.Format("{0}:\r\n{1}", item.Key,string.Join("\r\n",item.ToList().Select(o=>string.Format("- {0} x{1} {2}",o.SERVICE_NAME, o.AMOUNT, o.SERVICE_UNIT_NAME)))));

                    }
                    Medical_order = string.Join("\r\n", lst);

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

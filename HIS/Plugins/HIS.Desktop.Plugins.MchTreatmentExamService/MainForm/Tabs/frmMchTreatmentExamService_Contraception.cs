using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraEditors;
using MCH.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.MainForm
{
    public partial class UCMchTreatmentExamService : HIS.Desktop.Utility.FormBase
    {
        #region Tab 4: Tránh thai

        private void GetDataFromTab4()
        {
            try
            {
                if (_examService == null) _examService = new MCH_EXAM_SERVICE();
                if (_contraception == null) _contraception = new MCH_CONTRACEPTION();
                // MCH_EXAM_SERVICE
                _examService.EXECUTE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteExam4.DateTime) ?? 0;
                _examService.EXECUTE_LOGINNAME = GetComboValue(cboUser4);
                _examService.EXECUTE_USERNAME = GetUserNameByLoginName(_examService.EXECUTE_LOGINNAME);
                _examService.EXECUTE_TYPE = GetComboValue(cboDiploma4);

                // MCH_CONTRACEPTION
                _contraception.CONTRACEPTION_METHOD = GetComboValue(cboContraceptionMethod4);
                _contraception.CONTRACEPTION_COMPLICATION = GetComboValue(cboContraceptionComplication4);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToTab4()
        {
            try
            {
                if (_examService != null)
                {
                    SetComboValue(cboUser4, _examService.EXECUTE_LOGINNAME);
                    SetComboValue(cboDiploma4, _examService.EXECUTE_TYPE);
                }

                if (_contraception != null)
                {
                    SetComboValue(cboContraceptionMethod4, _contraception.CONTRACEPTION_METHOD);
                    SetComboValue(cboContraceptionComplication4, _contraception.CONTRACEPTION_COMPLICATION);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion
    }
}

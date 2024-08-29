using Inventec.UC.CreateReport.Base;
using Inventec.UC.CreateReport.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.CreateReport.Design.TemplateBetweenTimeFilterOnly.TemplateBetweenTimeFilterOnlyValidation
{
    internal class TimeToDateValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.DateEdit deTimeToDate;
        internal DevExpress.XtraEditors.DateEdit deTimeFromDate;
        public override bool Validate(Control control, object value)
        {
            bool valid = true;
            try
            {
                valid = valid && (deTimeToDate != null) && (deTimeFromDate != null);
                if (valid)
                {
                    var dateTimeTo = GlobalStore.ConvertDateStringToSystemDate(deTimeToDate.Text);
                    var dataTimeFrom = GlobalStore.ConvertDateStringToSystemDate(deTimeFromDate.Text);

                    //if (dateTimeTo == null || dataTimeFrom == null)
                    //{
                    //    valid = false;
                    //}
                    if (dateTimeTo.Value == DateTime.MinValue || dataTimeFrom.Value == DateTime.MinValue)
                    {
                        valid = true;
                    }
                    else
                    {
                        valid = valid && (dateTimeTo != null && dateTimeTo.Value != DateTime.MinValue);
                        if (valid && dateTimeTo.Value < dataTimeFrom.Value)
                        {
                            valid = false;
                            this.ErrorText = Base.MessageUtil.GetMessage(Inventec.UC.CreateReport.MessageLang.Message.Enum.HeThongThongBaoThoiGianDenBeHonThoiGianTu);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
    }
}

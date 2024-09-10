using Inventec.UC.CreateReport.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.CreateReport.Design.TemplateBetweenTimeFilterOnly2.Validate
{
    class TimeFromValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.DateEdit deTimeFromDate;
        internal DevExpress.XtraEditors.DateEdit deTimeToDate;
        public override bool Validate(Control control, object value)
        {
            bool valid = true;
            try
            {
                valid = valid && (deTimeFromDate != null) && (deTimeToDate != null);
                if (valid)
                {
                    var dateTimeFrom = GlobalStore.ConvertDateStringToSystemDate(deTimeFromDate.Text);
                    var dataTimeTo = GlobalStore.ConvertDateStringToSystemDate(deTimeToDate.Text);

                    if (dateTimeFrom.Value == DateTime.MinValue || dataTimeTo.Value == DateTime.MinValue)
                    {
                        valid = true;
                    }
                    else
                    {
                        valid = valid && (dateTimeFrom != null && dateTimeFrom.Value != DateTime.MinValue);
                        if (valid && dateTimeFrom.Value > dataTimeTo.Value)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboNational.Design.Template1
{
    public partial class Template1
    {
        internal bool SetDelegateFocusNext(FocusNextControl FocusNext)
        {
            bool result = false;
            try
            {
                this._FocusNext = FocusNext;
                if (_FocusNext != null)
                {
                    result = true;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => FocusNext), FocusNext));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        internal void SetFocus()
        {
            try
            {
                txtMaQuocTich.Focus();
                txtMaQuocTich.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void SetTextLabel(string textLabel)
        {
            try
            {
                lblQuocTich.Text = textLabel;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void SetDefaultNational(SDA.EFMODEL.DataModels.SDA_NATIONAL National)
        {
            try
            {
                this.defaultNational = National;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => National), National));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void ResetValue()
        {
            try
            {
                if (defaultNational != null)
                {
                    cboQuocTich.EditValue = defaultNational.NATIONAL_CODE;
                    txtMaQuocTich.Text = defaultNational.NATIONAL_CODE;
                }
                else
                {
                    cboQuocTich.EditValue = null;
                    txtMaQuocTich.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

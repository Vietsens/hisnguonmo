using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboEthnic.Design.Template1
{
    public partial class Template1
    {
        internal bool SetDelegateFocusNext(FocusNextControl Focus)
        {
            bool result = false;
            try
            {
                this._FocusNext = Focus;
                if (_FocusNext != null)
                {
                    result = true;
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
                txtMaDanToc.Focus();
                txtMaDanToc.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void SetDefaultEthnic(SDA.EFMODEL.DataModels.SDA_ETHNIC Ethnic)
        {
            try
            {
                this.defaultEthnic = Ethnic;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Ethnic), Ethnic));
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
                if (defaultEthnic != null)
                {
                    cboDanToc.EditValue = defaultEthnic.ETHNIC_CODE;
                    txtMaDanToc.Text = defaultEthnic.ETHNIC_CODE;
                }
                else
                {
                    txtMaDanToc.Text = "";
                    cboDanToc.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);        
            }
        }
    }
}

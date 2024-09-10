using Inventec.UC.CreateReport.Init;
using Inventec.UC.CreateReport.Set.Delegate;
using Inventec.UC.CreateReport.Set.Delegate.SetGetObjectFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.CreateReport
{
    public partial class MainCreateReport
    {
        public enum EnumTemplate
        {
            TEMPLATE1,
            TEMPLATE2
        }

        public UserControl Init(EnumTemplate Template, Inventec.UC.CreateReport.Data.InitData Data)
        {
            UserControl result = null;
            try
            {
                result = InitFactory.MakeIInit().InitUC(Template, Data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public bool SetDelegateProcessHasException(UserControl UC, ProcessHasException hasException)
        {
            bool result = false;
            try
            {
                result = SetDelegateHasExceptionFactory.MakeISetDelegateHasException().SetDelegate(UC, hasException);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public void CloseFormDelegate(UserControl UC, CloseContainerForm closeForm)
        {
            try
            {
                SetDelegateCloseContainerFormFactory.MakeISetDelegateCloseContainerFormCreateReport().SetDelegateClose(UC, closeForm);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public bool GetFilterDelegate(UserControl UC, GetObjectFilter getFilter)
        {
            bool result = false;
            try
            {
                result = SetDelegateGetObjectFilterFactory.MakeISetDelegateGetObjectFilter().SetDelegate(UC, getFilter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result = false;
        }

        public void ButtonSearchClick(UserControl UC)
        {
            try
            {
                //SetShortcutButtonSearchFactory.MakeISetShortcutButtonSearch().SearchClick(UC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void ButtonRefreshClick(UserControl UC)
        {
            try
            {
                //SetShortcutButtonRefreshFactory.MakeISetShortcutButtonRefresh().RefreshClick(UC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

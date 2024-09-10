using Inventec.UC.ComboNational.Data;
using Inventec.UC.ComboNational.Get.GetValueNationalCode;
using Inventec.UC.ComboNational.Get.GetValueNationalName;
using Inventec.UC.ComboNational.Init;
using Inventec.UC.ComboNational.Set.Delegate.SetDelegateFocusNextControl;
using Inventec.UC.ComboNational.Set.SetFocusComboNational;
using Inventec.UC.ComboNational.Set.Value.ResetValueControl;
using Inventec.UC.ComboNational.Set.Value.SetNationalDefaultCombo;
using Inventec.UC.ComboNational.Set.Value.SetTextLabelLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ComboNational
{
    public partial class MainComboNational
    {
        public static string TEMPLATE1 = "Template1";
        public static string TEMPLATE2 = "Template2";

        #region Init User Control
        public UserControl Init(string template, DataInitNational Data)
        {
            UserControl result = null;
            try
            {
                result = InitFactory.MakeIInit().InitCombo(template, Data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
        #endregion

        #region Set Delegate
        public bool SetDelegateFocusNext(UserControl UC, FocusNextControl FocusNext)
        {
            bool result = false;
            try
            {
                result = SetDelegateFocusNextControlFactory.MakeISetDelegateFocusNextControl().SetDelegateFocucNext(UC, FocusNext);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
        #endregion

        #region Set Focus
        public void SetFocus(UserControl UC)
        {
            try
            {
                SetFocusComboNationalFactory.MakeISetFocusComboNational().SetFocus(UC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Set Value National Default
        public void SetDefault(UserControl UC, SDA.EFMODEL.DataModels.SDA_NATIONAL National)
        {
            try
            {
                SetNationalDefaultComboFactory.MakeISetNationalDefaultCombo().SetDefaultNational(UC, National);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Set Text Label Language
        public void SetTextLabel(UserControl UC, string textLabel)
        {
            try
            {
                SetTextLabelLanguageFactory.MakeISetTextLabelLanguage().SetTextLabel(UC, textLabel);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Reset Value
        public void ResetValue(UserControl UC)
        {
            try
            {
                ResetValueControlFactory.MakeIResetValueControl().ResetValue(UC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region  Get National Code
        public string GetNationalCode(UserControl UC)
        {
            string result = null;
            try
            {
                GetValueNationalCodeFactory.MakeIGetValueNationalCode().GetNationalCode(UC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
        #endregion

        #region Get National Name
        public string GetNationalName(UserControl UC)
        {
            string result = null;
            try
            {
                GetValueNationalNameFactory.MakeIGetValueNationalName().GetNationalName(UC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
        #endregion
    }
}

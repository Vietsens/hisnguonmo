using Inventec.UC.ComboProvince.Data;
using Inventec.UC.ComboProvince.Get.GetEditValueComboProvince;
using Inventec.UC.ComboProvince.Get.GetValueProvinceName;
using Inventec.UC.ComboProvince.Get.GetValueProvinceCode;
using Inventec.UC.ComboProvince.Init;
using Inventec.UC.ComboProvince.Set.ResetValueControl;
using Inventec.UC.ComboProvince.Set.SetDataInit;
using Inventec.UC.ComboProvince.Set.SetDelegateFocus;
using Inventec.UC.ComboProvince.Set.SetDelegateLoadCbDistrictFromProvince;
using Inventec.UC.ComboProvince.Set.SetDelegateValueCboDistrictAndCommune;
using Inventec.UC.ComboProvince.Set.SetFocusComboProvince;
using Inventec.UC.ComboProvince.Set.SetLabelLanguage;
using Inventec.UC.ComboProvince.Set.SetValueComboProvince;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ComboProvince
{
    public partial class MainComboProvince
    {
        public static string TEMPLATE1 = "Template1";
        public static string TEMPLATE2 = "Template2";

        #region Init UserControl

        public System.Windows.Forms.UserControl Init(string template, DataInitProcinve Data)
        {
            System.Windows.Forms.UserControl result = null;
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

        public bool SetDelegateLoadDistrict(UserControl UC, LoadComboDistrictFromProvince LoadCombo)
        {
            bool result = false;
            try
            {
                result = SetDelegateLoadCbDistrictFromProvinceFactory.MakeISetDelegateLoadCbDistrictFromProvince().SetDelegateLoadDistrictFromProvince(UC, LoadCombo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public bool SetDelegateValueDistrictCommune(UserControl UC, SetValueCboDistrictAndCboCommune SetValue)
        {
            bool result = false;
            try
            {
                result = SetDelegateValueCboDistrictAndCommuneFactory.MakeISetDelegateValueCboDistrictAndCommune().SetDelegateDistrictAndCommune(UC, SetValue);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public bool SetDelegateFocusDistrict(UserControl UC, SetFocusCboDistrict Focus)
        {
            bool result = false;
            try
            {
                result = SetDelegateFocusFactory.MakeISetDelegateFocus().SetDelegateLoadFocus(UC, Focus);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        #endregion

        #region Set Focus ComboTinh
        public void SetFocus(UserControl UC)
        {
            try
            {
                SetFocusComboProvinceFactory.MakeISetFocusComboProvince().SetFocus(UC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Set Text Label
        public void SetTextLabel(UserControl UC, string textLabel)
        {
            try
            {
                SetLabelLanguageFactory.MakeISetLabelLanguage().SetLanguageLabel(UC, textLabel);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Set Value Combo Tinh
        public void SetValue(UserControl UC, string ProvinceCODE)
        {
            try
            {
                SetValueComboProvinceFactory.MakeISetValueComboProvince().SetValue(UC, ProvinceCODE);
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

        #region Set Data Init
        public void SetDataTinh(UserControl UC, DataInitProcinve Data)
        {
            try
            {
                SetDataInitFactory.MakeISetDataInit().SetData(UC, Data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Get Value
        public string GetProvinceName(UserControl UC)
        {
            string result = null;
            try
            {
                result = GetValueProvinceNameFactory.MakeIGetValueProvinceName().GetProvinceName(UC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public string GetProvinceCode(UserControl UC)
        {
            string result = null;
            try
            {
                result = GetValueProvinceCodeFactory.MakeIGetValueProvinceCode().GetProvinceCode(UC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public object GetEditValue(UserControl UC)
        {
            object result = null;
            try
            {
                result = GetEditValueComboProvinceFactory.MakeIGetEditValueComboProvince().GetEditValue(UC);
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

/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using Inventec.UC.ComboCommune.Data;
using Inventec.UC.ComboCommune.Get.GetEditValueComboCommune;
using Inventec.UC.ComboCommune.Get.GetValueCommuneCode;
using Inventec.UC.ComboCommune.Get.GetValueCommuneName;
using Inventec.UC.ComboCommune.Init;
using Inventec.UC.ComboCommune.Set.ResetValueControl;
using Inventec.UC.ComboCommune.Set.SetDelegateGetValueComboDistrict;
using Inventec.UC.ComboCommune.Set.SetDelegateGetValueComboProvince;
using Inventec.UC.ComboCommune.Set.SetDelegateSetFocusControl;
using Inventec.UC.ComboCommune.Set.SetDelegateSetValueComboTHX;
using Inventec.UC.ComboCommune.Set.SetFocusComboCommune;
using Inventec.UC.ComboCommune.Set.SetLoadComboCommune;
using Inventec.UC.ComboCommune.Set.SetTextLabelLanguage;
using Inventec.UC.ComboCommune.Set.SetValueComboCommune;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ComboCommune
{
    public partial class MainComboCommune
    {
        public static string TEMPLATE1 = "Template1";
        public static string TEMPLATE2 = "Template2";

        #region Init UserControl Phuong Xa
        public UserControl Init(string template, DataInitCommune Data)
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
        public bool SetDelegateSetValueTHX(UserControl UC, SetValueComboTHX SetValue)
        {
            bool result = false;
            try
            {
                result = SetDelegateSetValueComboTHXFactory.MakeISetDelegateSetValueComboTHX().SetDelegateSetValueTHX(UC, SetValue);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public bool SetDelegateSetFocusNext(UserControl UC, SetFocusNextControl FocusNext)
        {
            bool result = false;
            try
            {
                result = SetDelegateSetFocusControlFactory.MakeISetDelegateSetFocusControl().SetDelegateSetFocusNext(UC, FocusNext);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public bool SetDelegateGetValueDistrict(UserControl UC, GetValueComboDistrict GetValue)
        {
            bool result = false;
            try
            {
                result = SetDelegateGetValueComboDistrictFactory.MakeISetDelegateGetValueComboDistrict().SetDelegateValueDistrict(UC, GetValue);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public bool SetDelegateGetValueProvince(UserControl UC, GetValueComboProvince GetValue)
        {
            bool result = false;
            try
            {
                result = SetDelegateGetValueComboProvinceFactory.MakeISetDelegateGetValueComboProvince().SetDelegateGetValueProvince(UC, GetValue);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
        #endregion

        #region Set Focus ComboBox Phuong Xa
        public void SetFocus(UserControl UC)
        {
            try
            {
                SetFocusComboCommuneFactory.MakeISetFocusComboCommune().SetFocus(UC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Set Load ComboBox Phuong Xa
        public void SetLoadValue(UserControl UC, string DistrictCODE)
        {
            try
            {
                SetLoadComboCommuneFactory.MakeISetLoadComboCommune().SetValue(UC, DistrictCODE);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Set Value
        public void SetValueCommune(UserControl UC, string CommuneCODE)
        {
            try
            {
                SetValueComboCommuneFactory.MakeISetValueComboCommune().SetValue(UC, CommuneCODE);
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

        #region Get Value
        public string GetCommuneName(UserControl UC)
        {
            string result = null;
            try
            {
                result = GetValueCommuneNameFactory.MakeIGetValueCommuneName().GetCommuneName(UC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public string GetCommuneCode(UserControl UC)
        {
            string result = null;
            try
            {
                result = GetValueCommuneCodeFactory.MakeIGetValueCommuneCode().GetCommuneCode(UC);
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
                result = GetEditValueComboCommuneFactory.MakeIGetEditValueComboCommune().GetValue(UC);
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

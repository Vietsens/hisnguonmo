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
using Inventec.UC.ComboDistrict.Data;
using Inventec.UC.ComboDistrict.Get.GetValueDistrictName;
using Inventec.UC.ComboDistrict.Get.GetValueDistrictCode;
using Inventec.UC.ComboDistrict.Init;
using Inventec.UC.ComboDistrict.Set.ResetValueControl;
using Inventec.UC.ComboDistrict.Set.SetDelegateGetValueProvince;
using Inventec.UC.ComboDistrict.Set.SetDelegateLoadComboCommune;
using Inventec.UC.ComboDistrict.Set.SetDelegateSetFocusComboCommune;
using Inventec.UC.ComboDistrict.Set.SetDelegateSetValueComboCommune;
using Inventec.UC.ComboDistrict.Set.SetFocusComboDistrict;
using Inventec.UC.ComboDistrict.Set.SetLoadComboDistrict;
using Inventec.UC.ComboDistrict.Set.SetTextLabelLanguage;
using Inventec.UC.ComboDistrict.Set.SetValueComboDistrict;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.UC.ComboDistrict.Get.GetEditValueDistrict;

namespace Inventec.UC.ComboDistrict
{
    public partial class MainComboDistrict
    {
        public static string TEMPLATE1 = "Template1";
        public static string TEMPLATE2 = "Template2";

        #region Init
        public UserControl Init(string template, DataInitDistrict Data)
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
        public bool SetDelegateLoadComboCommune(UserControl UC, LoadComboCommuneFromDistrict LoadComboCommune)
        {
            bool result = false;
            try
            {
                result = SetDelegateLoadComboCommuneFactory.MakeISetDelegateLoadComboCommune().SetDelegateLoadCommune(UC, LoadComboCommune);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public bool SetDelegateSetValueCommune(UserControl UC, SetValueComboCommune setValue)
        {
            bool result = false;
            try
            {
                result = SetDelegateSetValueComboCommuneFactory.MakeISetDelegateSetValueComboCommune().SetDelegateSetValueCommune(UC, setValue);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public bool SetDelegateSetFocusCommune(UserControl UC, SetFocusComboCommune Focus)
        {
            bool result = false;
            try
            {
                SetDelegateSetFocusComboCommuneFactory.MakeISetDelegateSetFocusComboCommune().SetDelegateFocusCommune(UC, Focus);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public bool SetDelegateGetValueProvince(UserControl UC, GetValueComboProvince getValue)
        {
            bool result = false;
            try
            {
                result = SetDelegateGetValueProvinceFactory.MakeISetDelegateGetValueProvince().SetDelegateGetProvince(UC, getValue);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
        #endregion

        #region Set Load Combo Huyen
        public void SetLoadDistrict(UserControl UC, string ProvinceCODE)
        {
            try
            {
                SetLoadComboDistrictFactory.MakeISetLoadComboDistrict().SetLoadDistrict(UC, ProvinceCODE);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Set Value
        public void SetValue(UserControl UC, string DistrictCODE)
        {
            try
            {
                SetValueComboDistrictFactory.MakeISetValueComboDistrict().SetValue(UC, DistrictCODE);
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

        #region Set Focus Combo Huyen
        public void SetFocusHuyen(UserControl UC)
        {
            try
            {
                SetFocusComboDistrictFactory.MakeISetFocusComboDistrict().SetFocus(UC);
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
        public string GetDistrictName(UserControl UC)
        {
            string result = null;
            try
            {
                result = GetValueDistrictNameFactory.MakeIGetValueDistrictName().GetDistrictName(UC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public string GetDistrictCode(UserControl UC)
        {
            string result = null;
            try
            {
                result = GetValueDistrictCodeFactory.MakeIGetValueDistrictCode().GetDistrictCode(UC);
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
                result = GetEditValueDistrictFactory.MakeIGetEditValueDistrict().GetEditValue(UC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        #endregion
    }
}

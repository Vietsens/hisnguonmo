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
using Inventec.UC.ComboTHX.Init;
using Inventec.UC.ComboTHX.Set.ResetValueControl;
using Inventec.UC.ComboTHX.Set.SetDelegate.SetDelegateFocusComboProvince;
using Inventec.UC.ComboTHX.Set.SetDelegate.SetDelegateFocusControlNext;
using Inventec.UC.ComboTHX.Set.SetDelegate.SetDelegateLoadComboDistrict;
using Inventec.UC.ComboTHX.Set.SetDelegate.SetDelegateLoadComboCommune;
using Inventec.UC.ComboTHX.Set.SetDelegate.SetDelegateSetValueComboDistrict;
using Inventec.UC.ComboTHX.Set.SetDelegate.SetDelegateSetValueComboCommune;
using Inventec.UC.ComboTHX.Set.SetDelegate.SetDelegateSetValueComboProvince;
using Inventec.UC.ComboTHX.Set.SetFocusComboTHX;
using Inventec.UC.ComboTHX.Set.SetValueComboTHX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ComboTHX
{
    public partial class MainComboTHX
    {
        public static string TEMPLATE1 = "Template1";
        public static string TEMPLATE2 = "Template2";

        #region Init UserControl
        public UserControl Init(string template, Data.DataInitTHX Data)
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

        public bool SetDelegateLoadCboDistrict(UserControl UC, LoadComboDistrict LoadHuyen)
        {
            bool result = false;
            try
            {
                result = SetDelegateLoadComboDistrictFactory.MakeISetDelegateLoadComboDistrict().SetDelegateLoadDistrict(UC, LoadHuyen);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public bool SetDelegateLoadCboCommune(UserControl UC, LoadComboCommune LoadPhuongXa)
        {
            bool result = false;
            try
            {
                result = SetDelegateLoadComboCommuneFactory.MakeISetDelegateLoadComboCommune().SetDelegateLoadCommune(UC, LoadPhuongXa);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public bool SetDelegateSetValueProvince(UserControl UC, SetValueComboProvince ValueTinh)
        {
            bool result = false;
            try
            {
                result = SetDelegateSetValueComboProvinceFactory.MakeISetDelegateSetValueComboProvince().SetDelegateValueProvince(UC, ValueTinh);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public bool SetDelegateSetValueDistrict(UserControl UC, SetValueComboDistrict ValueHuyen)
        {
            bool result = false;
            try
            {
                result = SetDelegateSetValueComboDistrictFactory.MakeISetDelegateSetValueComboDistrict().SetDelegateSetValueDistrict(UC, ValueHuyen);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public bool SetDelegateSetValueCommune(UserControl UC, SetValueComboCommune ValuePhuongXa)
        {
            bool result = false;
            try
            {
                result = SetDelegateSetValueComboCommuneFactory.MakeISetDelegateSetValueComboCommune().SetDelegateSetValueCommune(UC, ValuePhuongXa);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public bool SetDelegateFocusProvince(UserControl UC, FocusComboProvince FocusTinh)
        {
            bool result = false;
            try
            {
                result = SetDelegateFocusComboProvinceFactory.MakeISetDelegateFocusComboProvince().SetDelegateFocusProvince(UC, FocusTinh);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public bool SetDelegateFocusNext(UserControl UC, FocusNextControl FocusNext)
        {
            bool result = false;
            try
            {
                result = SetDelegateFocusControlNextFactory.MakeISetDelegateFocusControlNext().SetDelegateFocusNext(UC, FocusNext);
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
                SetFocusComboTHXFactory.MakeISetFocusComboTHX().SetFocus(UC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Set Value
        public void SetValue(UserControl UC, SDA.EFMODEL.DataModels.V_SDA_COMMUNE Commune)
        {
            try
            {
                SetValueComboTHXFactory.MakeISetValueComboTHX().SetValue(UC, Commune);
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
    }
}

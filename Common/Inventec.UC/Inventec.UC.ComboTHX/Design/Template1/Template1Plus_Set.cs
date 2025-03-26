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
using Inventec.UC.ComboTHX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboTHX.Design.Template1
{
    public partial class Template1
    {
        #region Set Delegate
        internal bool SetDelegateLoadComboDistrict(LoadComboDistrict LoadHuyen)
        {
            bool result = false;
            try
            {
                this._LoadHuyen = LoadHuyen;
                if (_LoadHuyen != null)
                {
                    result = true;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => LoadHuyen), LoadHuyen));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        internal bool SetDelegateLoadComboCommune(LoadComboCommune LoadPhuongXa)
        {
            bool result = false;
            try
            {
                this._LoadPhuongXa = LoadPhuongXa;
                if (_LoadPhuongXa != null)
                {
                    result = true;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => LoadPhuongXa), LoadPhuongXa));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        internal bool SetDelegateSetValueProvince(SetValueComboProvince ValueTinh)
        {
            bool result = false;
            try
            {
                this._SetValueTinh = ValueTinh;
                if (_SetValueTinh != null)
                {
                    result = true;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ValueTinh), ValueTinh));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        internal bool SetDelegateSetValueDistrict(SetValueComboDistrict ValueHuyen)
        {
            bool result = false;
            try
            {
                this._SetValueHuyen = ValueHuyen;
                if (_SetValueHuyen != null)
                {
                    result = true;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ValueHuyen), ValueHuyen));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        internal bool SetDelegateSetValueCommune(SetValueComboCommune ValuePhuongXa)
        {
            bool result = false;
            try
            {
                this._SetValuePhuongXa = ValuePhuongXa;
                if (_SetValuePhuongXa != null)
                {
                    result = true;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ValuePhuongXa), ValuePhuongXa));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        internal bool SetDelegateFocusComboProvince(FocusComboProvince FocusTinh)
        {
            bool result = false;
            try
            {
                this._FocusTinh = FocusTinh;
                if (_FocusTinh != null)
                {
                    result = true;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => FocusTinh), FocusTinh));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        internal bool SetDelegateFocusNextControl(FocusNextControl FocusNext)
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
        #endregion

        internal void SetValueComboTHX(SDA.EFMODEL.DataModels.V_SDA_COMMUNE Commune)
        {
            try
            {
                cboTHX.Properties.DataSource = listDataTHX;
                List<ViewSdaCommuneModel> listResult = listData.Where(o => o.ID == Commune.ID).ToList();
                if (listResult.Count == 1)
                {
                    cboTHX.EditValue = listResult[0].ID;
                    txtMaTHX.Text = listResult[0].SEARCH_CODE;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Commune), Commune));
                }
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void SetFocus()
        {
            try
            {
                txtMaTHX.Focus();
                txtMaTHX.SelectAll();
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
                cboTHX.EditValue = null;
                txtMaTHX.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

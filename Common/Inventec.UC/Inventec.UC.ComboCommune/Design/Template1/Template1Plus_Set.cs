﻿using Inventec.UC.ComboCommune.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboCommune.Design.Template1
{
    public partial class Template1
    {
        #region Set Delegate

        internal bool SetDelegateSetValueTHX(SetValueComboTHX Value)
        {
            bool result = false;
            try
            {
                this._SetValueTHX = Value;
                if (_SetValueTHX != null)
                {
                    result = true;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Value), Value));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        internal bool SetDelegateSetFocusNext(SetFocusNextControl Focus)
        {
            bool result = false;
            try
            {
                this._FocusControlNext = Focus;
                if (_FocusControlNext != null)
                {
                    result = true;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Focus), Focus));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        internal bool SetDelegateGetValueProvince(GetValueComboProvince getValue)
        {
            bool result = false;
            try
            {
                this._ValueProvince = getValue;
                if (_ValueProvince != null)
                {
                    result = true;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => getValue), getValue));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        internal bool SetDelegateGetValueDistrict(GetValueComboDistrict getValue)
        {
            bool result = false;
            try
            {
                this._ValueDistrict = getValue;
                if (_ValueDistrict != null)
                {
                    result = true;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => getValue), getValue));
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

        internal void SetLoadComboCommune(string DistrictCode)
        {
            try
            {
                LoadXaCombo("", DistrictCode, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void SetValueCombo(string CommuneCODE)
        {
            try
            {
                if (_ValueDistrict != null)
                    LoadXaCombo(CommuneCODE, _ValueDistrict().ToString(), true);
                cboPhuongXa.EditValue = CommuneCODE;
                txtMaPhuongXa.Text = CommuneCODE;
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
                txtMaPhuongXa.Focus();
                txtMaPhuongXa.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void SetDataInit(DataInitCommune data)
        {
            try
            {
                this.DataCommune = data;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
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
                lblPhuongXa.Text = textLabel;
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
                cboPhuongXa.EditValue = null;
                txtMaPhuongXa.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

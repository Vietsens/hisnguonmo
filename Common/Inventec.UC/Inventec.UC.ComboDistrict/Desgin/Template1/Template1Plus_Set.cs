﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.UC.ComboDistrict.Data;

namespace Inventec.UC.ComboDistrict.Desgin.Template1
{
    public partial class Template1
    {
        #region Set Delegate
        internal bool SetDelegateLoadCommune(LoadComboCommuneFromDistrict LoadCombo)
        {
            bool result = false;
            try
            {
                this._LoadComboCommune = LoadCombo;
                if (_LoadComboCommune != null)
                {
                    result = true;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => LoadCombo), LoadCombo));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        internal bool SetDelegateSetValue(SetValueComboCommune SetCombo)
        {
            bool result = false;
            try
            {
                this._SetValueCommune = SetCombo;
                if (_SetValueCommune != null)
                {
                    result = true;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => SetCombo), SetCombo));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        internal bool SetDelegateFocusCommune(SetFocusComboCommune Focus)
        {
            bool result = false;
            try
            {
                this._FocusComboCommune = Focus;
                if (_FocusComboCommune != null)
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

        internal bool SetdelegateGetValueProvince(GetValueComboProvince getValue)
        {
            bool result = false;
            try
            {
                this._GetValueProvince = getValue;
                if (_GetValueProvince != null)
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

        internal void SetLoadComboDistrict(string ProvinceCODE)
        {
            try
            {
                LoadHuyenCombo("", ProvinceCODE, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void SetValueDistrict(string DistrictCODE)
        {
            try
            {
                cboHuyen.EditValue = DistrictCODE;
                txtMaHuyen.Text = DistrictCODE;
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
                lblHuyen.Text = textLabel;
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
                txtMaHuyen.Focus();
                txtMaHuyen.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void SetDataInit(DataInitDistrict data)
        {
            try
            {
                this.DataDistrict = data;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                //this.listData = data.listDistrict;
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
                cboHuyen.EditValue = null;
                txtMaHuyen.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

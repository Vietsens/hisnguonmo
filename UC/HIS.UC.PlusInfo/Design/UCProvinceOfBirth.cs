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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors;
using Inventec.Common.Controls.PopupLoader;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.DelegateRegister;
using HIS.Desktop.Utility;
using SDA.EFMODEL.DataModels;
using HIS.UC.PlusInfo.ShareMethod;
using HIS.UC.PlusInfo.Config;
using HIS.UC.PlusInfo.Validate;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.UC.PlusInfo.ADO;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;

namespace HIS.UC.PlusInfo.Design
{
    public partial class UCProvinceOfBirth : UserControlBase
    {
        #region Declare

        IShareMethodInit _shareMethod = new ShareMethodDetail();
        DelegateSetValueForUCPlusInfo dlgLoadDistrict;
        DelegateNextControl dlgFocusNextUserControl;
        DelegateReloadData dlgReloadData;

        #endregion

        #region Contructor - Load

        public UCProvinceOfBirth()
            : base("UCPlusInfo", "UCProvinceOfBirth")
        {
            try
            {
                InitializeComponent();

                this.txtProvinceOfBirthCode.TabIndex = this.TabIndex;
                this.SetCaptionByLanguageKeyNew();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCProvinceOfBirth
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.PlusInfo.Resources.Lang", typeof(UCProvinceOfBirth).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCProvinceOfBirth.layoutControl1.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboProvinceOfBirth.Properties.NullText = Inventec.Common.Resource.Get.Value("UCProvinceOfBirth.cboProvinceOfBirth.Properties.NullText", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciProvinceOfBirth.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCProvinceOfBirth.lciProvinceOfBirth.OptionsToolTip.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciProvinceOfBirth.Text = Inventec.Common.Resource.Get.Value("UCProvinceOfBirth.lciProvinceOfBirth.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void UCProvinceOfBirth_Load(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void UCProvinceOfBirthInit()
        {
            try
            {
                _shareMethod.InitComboCommon(this.cboProvinceOfBirth, BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>(), "PROVINCE_CODE", "PROVINCE_NAME", "SEARCH_CODE");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                this.lciProvinceOfBirth.Text = Inventec.Common.Resource.Get.Value("UCPlusInfo.UCProvinceOfBirth", HIS.UC.PlusInfo.ShareMethod.ResourceLanguageManager.ResourceUCPlusInfo, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void SetDelegateLoadDistrictByProvince(DelegateSetValueForUCPlusInfo _dlgLoadDistrict)
        {
            try
            {
                if (_dlgLoadDistrict != null)
                    this.dlgLoadDistrict = _dlgLoadDistrict;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void ReloadDataDistrictAndCommune(DelegateReloadData _dlgReloadData)
        {
            try
            {
                if (_dlgReloadData != null)
                    this.dlgReloadData = _dlgReloadData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Tiep don: UCPlusInfo/UCProvinceNow/ReloadDistrictAndCommune:\n" + ex);
            }
        }
        #endregion

        #region Event Control

        private void cboProvinceOfBirth_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboProvinceOfBirth.EditValue != null
                        && this.cboProvinceOfBirth.EditValue != this.cboProvinceOfBirth.OldEditValue)
                    {
                        if (this.dlgReloadData != null)
                            this.dlgReloadData(false);
                        var pro = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_CODE == this.cboProvinceOfBirth.EditValue.ToString());
                        if (pro != null)
                        {
                            this.txtProvinceOfBirthCode.Text = pro.PROVINCE_CODE;
                            if (this.dlgLoadDistrict != null)
                                this.dlgLoadDistrict(pro, false);
                        }
                    }
                    this.dlgFocusNextUserControl(this.TabIndex, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProvinceOfBirth_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboProvinceOfBirth.EditValue != null)
                    {
                        if (this.dlgReloadData != null)
                            this.dlgReloadData(false);

                        var pro = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_CODE == this.cboProvinceOfBirth.EditValue.ToString());
                        if (pro != null)
                        {
                            this.txtProvinceOfBirthCode.Text = pro.PROVINCE_CODE;
                            if (this.dlgLoadDistrict != null)
                                this.dlgLoadDistrict(pro, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtProvinceOfBirthCode_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.txtProvinceOfBirthCode.Text))
                {
                    this.cboProvinceOfBirth.Properties.DataSource = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtProvinceOfBirthCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = txtProvinceOfBirthCode.Text;
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        this.cboProvinceOfBirth.EditValue = null;
                        _shareMethod.FocusShowpopup(this.cboProvinceOfBirth, false);
                        e.Handled = true;
                    }
                    else
                    {
                        List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                        listResult = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.SEARCH_CODE.Contains(searchCode)).ToList();
                        if (listResult.Count == 1)
                        {
                            this.cboProvinceOfBirth.EditValue = listResult[0].PROVINCE_CODE;
                            this.txtProvinceOfBirthCode.Text = listResult[0].SEARCH_CODE;
                            if (this.dlgLoadDistrict != null)
                                this.dlgLoadDistrict(listResult[0], false);

                            if (this.dlgReloadData != null)
                                this.dlgReloadData(false);

                            this.dlgFocusNextUserControl(this.TabIndex, null);
                        }
                        else
                        {
                            //this.cboProvinceOfBirth.EditValue = null;
                            _shareMethod.FocusShowpopup(this.cboProvinceOfBirth, false);
                            e.Handled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Data

        internal UCPlusInfoADO GetValue()
        {
            UCPlusInfoADO dataGet = new UCPlusInfoADO();
            try
            {
                if (this.cboProvinceOfBirth.EditValue != null)
                {
                    dataGet.PROVINCE_OfBIRTH_NAME = this.cboProvinceOfBirth.Text;
                    dataGet.PROVINCE_OfBIRTH_CODE = (this.cboProvinceOfBirth.EditValue ?? "0").ToString();
                }
                else
                {
                    dataGet.PROVINCE_OfBIRTH_NAME = "";
                    dataGet.PROVINCE_OfBIRTH_CODE = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                dataGet = null;
            }
            return dataGet;
        }

        internal void SetValue(string provinceCODE)
        {
            try
            {
                if (!String.IsNullOrEmpty(provinceCODE))
                {

                    List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                    string proCode = this._shareMethod.GenerateProvinceCode(provinceCODE);
                    listResult = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.PROVINCE_CODE == proCode).ToList();
                    if (listResult.Count == 1)
                    {
                        this.cboProvinceOfBirth.EditValue = listResult[0].PROVINCE_CODE;
                        this.txtProvinceOfBirthCode.Text = listResult[0].SEARCH_CODE;
                    }
                }
                else
                {
                    this.txtProvinceOfBirthCode.Text = "";
                    this.cboProvinceOfBirth.EditValue = null;
                }
                //this.txtProvinceOfBirthCode.TabIndex = this.TabIndex;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void SetValueFromUCAddress(object dataSet)//
        {
            try
            {
                if (dataSet != null)
                {
                    if (dataSet is SDA.EFMODEL.DataModels.V_SDA_DISTRICT)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT data = (SDA.EFMODEL.DataModels.V_SDA_DISTRICT)dataSet;
                        if (!String.IsNullOrEmpty(data.PROVINCE_CODE) && !String.IsNullOrEmpty(data.PROVINCE_NAME))
                        {
                            this.txtProvinceOfBirthCode.Text = data.PROVINCE_CODE;
                            this.cboProvinceOfBirth.EditValue = data.PROVINCE_CODE;
                        }
                        else
                        {
                            SDA.EFMODEL.DataModels.V_SDA_DISTRICT pro = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>()
                            .SingleOrDefault(o => o.DISTRICT_CODE == data.DISTRICT_CODE);
                            if (pro != null)
                            {
                                this.txtProvinceOfBirthCode.Text = pro.PROVINCE_CODE;
                                this.cboProvinceOfBirth.EditValue = pro.PROVINCE_CODE;
                            }
                        }

                    }
                    if (dataSet is SDA.EFMODEL.DataModels.V_SDA_PROVINCE)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE data = (SDA.EFMODEL.DataModels.V_SDA_PROVINCE)dataSet;
                        this.txtProvinceOfBirthCode.Text = data.PROVINCE_CODE;
                        this.cboProvinceOfBirth.EditValue = data.PROVINCE_CODE;

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Validate

        internal void SetValidateControl(bool _isObligatory)
        {
            try
            {
                if (_isObligatory == true)
                {
                    this.lciProvinceOfBirth.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    Valid_ProvinceOfBirth_Control oProvinceOfBirth = new Valid_ProvinceOfBirth_Control();
                    oProvinceOfBirth.txtProvinceOfBirth = this.txtProvinceOfBirthCode;
                    oProvinceOfBirth.cboProvinceOfBirth = this.cboProvinceOfBirth;
                    oProvinceOfBirth.ErrorType = ErrorType.Warning;
                    this.dxValidationProviderControl.SetValidationRule(this.txtProvinceOfBirthCode, oProvinceOfBirth);
                }
                else
                {
                    this.lciProvinceOfBirth.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                    this.dxValidationProviderControl.SetValidationRule(this.txtProvinceOfBirthCode, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal bool ValidateRequiredField()
        {
            bool valid = true;
            try
            {
                if (!this.dxValidationProviderControl.Validate())
                {
                    IList<Control> invalidControls = this.dxValidationProviderControl.GetInvalidControls();
                    for (int i = invalidControls.Count - 1; i >= 0; i--)
                    {
                        Inventec.Common.Logging.LogSystem.Debug((i == 0 ? "InvalidControls:" : "") + "" + invalidControls[i].Name + ",");
                    }
                    valid = false;
                }

            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return valid;
        }

        #endregion

        #region Focus

        internal void FocusNextControl(DelegateNextControl _dlgFocusNextControl)
        {
            try
            {
                if (_dlgFocusNextControl != null)
                    this.dlgFocusNextUserControl = _dlgFocusNextControl;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
        internal void DisposeControl()
        {
            try
            {
                dlgReloadData = null;
                dlgFocusNextUserControl = null;
                dlgLoadDistrict = null;
                _shareMethod = null;
                this.txtProvinceOfBirthCode.EditValueChanged -= new System.EventHandler(this.txtProvinceOfBirthCode_EditValueChanged);
                this.txtProvinceOfBirthCode.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtProvinceOfBirthCode_KeyDown);
                this.cboProvinceOfBirth.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboProvinceOfBirth_Closed);
                this.cboProvinceOfBirth.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboProvinceOfBirth_KeyUp);
                this.Load -= new System.EventHandler(this.UCProvinceOfBirth_Load);
                dxErrorProviderControl = null;
                dxValidationProviderControl = null;
                lciProvinceOfBirth = null;
                layoutControlItem1 = null;
                txtProvinceOfBirthCode = null;
                cboProvinceOfBirth = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

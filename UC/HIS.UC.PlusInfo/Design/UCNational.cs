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
using HIS.UC.PlusInfo.ShareMethod;
using HIS.Desktop.DelegateRegister;
using HIS.UC.PlusInfo.ADO;
using HIS.UC.PlusInfo.Config;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;

namespace HIS.UC.PlusInfo.Design
{
    public partial class UCNational : UserControl
    {
        #region Declare

        IShareMethodInit _shareMethod = new ShareMethodDetail();
        DelegateNextControl dlgFocusNextUserControl;
        long positionHandle = -1;
        #endregion

        #region Constructor - Load

        public UCNational()
        {
            try
            {
                InitializeComponent(); 
                
                this.txtNationalCode.TabIndex = this.TabIndex;
                this.SetCaptionByLanguageKeyNew();
                SetValidRule();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetValidRule()
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.LookupEditWithTextEditValidationRule validate = new Inventec.Desktop.Common.Controls.ValidationRule.LookupEditWithTextEditValidationRule();
                validate.txtTextEdit = txtNationalCode;
                validate.cbo = cboNational;
                validate.ErrorText = "Trường dữ liệu bắt buộc";
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtNationalCode, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        internal void ResetRequiredField()
        {
            try
            {
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProvider1, this.dxErrorProvider1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        internal bool ValidateRequiredField()
        {
            bool result = true;
            try
            {
                positionHandle = -1;
                result = dxValidationProvider1.Validate();
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCNational
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.PlusInfo.Resources.Lang", typeof(UCNational).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCNational.layoutControl1.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboNational.Properties.NullText = Inventec.Common.Resource.Get.Value("UCNational.cboNational.Properties.NullText", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNational.Text = Inventec.Common.Resource.Get.Value("UCNational.lciNational.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCNational_Load(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public async Task InitNational()
        {
            try
            {
                List<SDA.EFMODEL.DataModels.SDA_NATIONAL> datas = null;
                if (BackendDataWorker.IsExistsKey<SDA.EFMODEL.DataModels.SDA_NATIONAL>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>();
                }
                else
                {
                    Inventec.Core.CommonParam paramCommon = new Inventec.Core.CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<SDA.EFMODEL.DataModels.SDA_NATIONAL>>("api/SdaNational/Get", HIS.Desktop.ApiConsumer.ApiConsumers.SdaConsumer, filter, paramCommon);

                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(SDA.EFMODEL.DataModels.SDA_NATIONAL), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                _shareMethod.InitComboCommon(this.cboNational, datas, "NATIONAL_NAME", "NATIONAL_NAME", "NATIONAL_CODE");

                this.LoadNationalBase();
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
                this.lciNational.Text = Inventec.Common.Resource.Get.Value("UCPlusInfo.UCNational", HIS.UC.PlusInfo.ShareMethod.ResourceLanguageManager.ResourceUCPlusInfo,LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event UC

        private void txtNationalCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        txtNationalCode.Text = txtNationalCode.OldEditValue.ToString();
                        //this.cboNational.EditValue = null;
                        //_shareMethod.FocusShowpopup(this.cboNational, true);
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().Where(o => o.NATIONAL_CODE.ToLower().Contains(searchCode.ToLower())).ToList();
                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.NATIONAL_CODE.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboNational.EditValue = searchResult[0].NATIONAL_NAME;
                            this.txtNationalCode.Text = searchResult[0].NATIONAL_CODE;
                            try
                            {
                                this.dlgFocusNextUserControl(this.TabIndex, null);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn(ex);
                                SendKeys.Send("{TAB}");
                            }
                        }
                        else
                        {
                            txtNationalCode.Text = txtNationalCode.OldEditValue.ToString();
                            //this.cboNational.EditValue = null;
                            //_shareMethod.FocusShowpopup(this.cboNational, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNational_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    SDA.EFMODEL.DataModels.SDA_NATIONAL ethnic = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().SingleOrDefault(o => o.NATIONAL_NAME == (this.cboNational.EditValue ?? "").ToString());
                    if (ethnic != null)
                    {
                        this.txtNationalCode.Text = ethnic.NATIONAL_CODE;
                    }
                    try
                    {
                        this.dlgFocusNextUserControl(this.txtNationalCode, null);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                        SendKeys.Send("{TAB}");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNational_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SDA.EFMODEL.DataModels.SDA_NATIONAL data = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().SingleOrDefault(o => o.NATIONAL_NAME == (this.cboNational.EditValue ?? "").ToString());
                    if (data != null)
                    {
                        this.txtNationalCode.Text = data.NATIONAL_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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

        #region Data

        internal void SetValue(UCPlusInfoADO dataSet)
        {
            try
            {
                SDA.EFMODEL.DataModels.SDA_NATIONAL national = null;
                if (!String.IsNullOrEmpty(dataSet.NATIONAL_CODE))
                {
                    national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().SingleOrDefault(o => o.NATIONAL_CODE == dataSet.NATIONAL_CODE);
                    if (national != null)
                    {
                        this.cboNational.EditValue = national.NATIONAL_NAME;
                        this.txtNationalCode.Text = national.NATIONAL_CODE;
                    }
                }
                else if (!String.IsNullOrEmpty(dataSet.NATIONAL_NAME))
                {
                    national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().SingleOrDefault(o => o.NATIONAL_NAME == dataSet.NATIONAL_NAME);
                    if (national != null)
                    {
                        this.cboNational.EditValue = national.NATIONAL_NAME;
                        this.txtNationalCode.Text = national.NATIONAL_CODE;
                    }
                }
                else
                {
                    LoadNationalBase();
                }
                //this.txtNationalCode.TabIndex = this.TabIndex;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal UCPlusInfoADO GetValue()
        {
            UCPlusInfoADO dataGet = new UCPlusInfoADO();
            try
            {
                if (this.cboNational.EditValue != null)
                {
                    dataGet.NATIONAL_NAME = this.cboNational.Text;
                    
                    var national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().FirstOrDefault(o => o.NATIONAL_NAME == cboNational.Text);
                    if (national != null)
                    {
                        dataGet.MPS_NATIONAL_CODE = national.MPS_NATIONAL_CODE;
                        dataGet.NATIONAL_CODE = national.NATIONAL_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return dataGet;
        }

        internal void LoadNationalBase()
        {
            try
            {
                var nationalBase = HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.NationalBase;
                if (nationalBase != null)
                {
                    this.txtNationalCode.Text = nationalBase.NATIONAL_CODE;
                    this.cboNational.EditValue = nationalBase.NATIONAL_NAME;
                }
                this.txtNationalCode.TabIndex = this.TabIndex;
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
                dlgFocusNextUserControl = null;
                _shareMethod = null;
                this.cboNational.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboNational_Closed);
                this.cboNational.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboNational_KeyUp);
                this.txtNationalCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtNationalCode_PreviewKeyDown);
                this.Load -= new System.EventHandler(this.UCNational_Load);
                lciNational = null;
                layoutControlItem1 = null;
                txtNationalCode = null;
                cboNational = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}

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
using HIS.UC.PlusInfo.ADO;
using HIS.UC.PlusInfo.Config;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.UC.PlusInfo.Validate;
using Inventec.Core;
using System.Resources;

namespace HIS.UC.PlusInfo.Design
{
    public partial class UCEthnic : UserControlBase
    {
        #region Declare

        IShareMethodInit _shareMethod = new ShareMethodDetail();
        DelegateNextControl dlgFocusNextUserControl;

        #endregion

        #region Contructor - Load

        public UCEthnic()
            : base("UCPlusInfo", "UCEthnic")
        {
            try
            {
                InitializeComponent();
                SetCaptionByLanguageKeyNew();
                //this.SetCaptionByLanguageKey();
                IsValidateCombo(HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.IsValidate__Ethnic);
                this.txtEthnicCode.TabIndex = this.cboEthnic.TabIndex = this.TabIndex;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCEthnic_Load(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public async Task InitEthnic()
        {
            try
            {
                List<SDA.EFMODEL.DataModels.SDA_ETHNIC> datas = null;
                if (BackendDataWorker.IsExistsKey<SDA.EFMODEL.DataModels.SDA_ETHNIC>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<SDA.EFMODEL.DataModels.SDA_ETHNIC>>("api/SdaEthnic/Get", HIS.Desktop.ApiConsumer.ApiConsumers.SdaConsumer, filter, paramCommon);

                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(SDA.EFMODEL.DataModels.SDA_ETHNIC), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
                datas = datas.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                _shareMethod.InitComboCommon(this.cboEthnic, datas, "ETHNIC_NAME", "ETHNIC_NAME", "ETHNIC_CODE");

                this.LoadEthnicBase();
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
                this.lciEthnic.Text = Inventec.Common.Resource.Get.Value("UCPlusInfo.UCEthnic", HIS.UC.PlusInfo.ShareMethod.ResourceLanguageManager.ResourceUCPlusInfo, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }




        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCEthnic
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.PlusInfo.Resources.Lang", typeof(UCEthnic).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCEthnic.layoutControl1.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboEthnic.Properties.NullText = Inventec.Common.Resource.Get.Value("UCEthnic.cboEthnic.Properties.NullText", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEthnic.Text = Inventec.Common.Resource.Get.Value("UCEthnic.lciEthnic.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        #endregion

        #region Event Control

        private void txtEthnicCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = ((sender as DevExpress.XtraEditors.TextEdit).Text);
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        this.cboEthnic.EditValue = null;
                        _shareMethod.FocusShowpopup(this.cboEthnic, false);
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Where(o => o.ETHNIC_CODE.ToLower().Contains(searchCode.ToLower())).ToList();
                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.ETHNIC_CODE.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboEthnic.EditValue = searchResult[0].ETHNIC_NAME;
                            this.txtEthnicCode.Text = searchResult[0].ETHNIC_CODE;
                            if (this.dlgFocusNextUserControl != null)
                                this.dlgFocusNextUserControl(this.TabIndex, null);
                        }
                        else
                        {
                            this.cboEthnic.EditValue = null;
                            _shareMethod.FocusShowpopup(this.cboEthnic, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEthnic_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    SDA.EFMODEL.DataModels.SDA_ETHNIC ethnic = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).SingleOrDefault(o => o.ETHNIC_NAME == (this.cboEthnic.EditValue ?? "").ToString());
                    if (ethnic != null)
                    {
                        this.txtEthnicCode.Text = ethnic.ETHNIC_CODE;
                    }
                    if (this.dlgFocusNextUserControl != null)
                        this.dlgFocusNextUserControl(this.TabIndex, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEthnic_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboEthnic.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.SDA_ETHNIC data = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).SingleOrDefault(o => o.ETHNIC_NAME == (this.cboEthnic.EditValue ?? "").ToString());
                        if (data != null)
                        {
                            this.txtEthnicCode.Text = data.ETHNIC_CODE;
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

        internal void LoadEthnicBase()
        {
            try
            {
                var ethnicDefault = HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.EthinicBase;
                if (ethnicDefault != null)
                {
                    this.cboEthnic.EditValue = ethnicDefault.ETHNIC_NAME;
                    this.txtEthnicCode.Text = ethnicDefault.ETHNIC_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void SetValue(UCPlusInfoADO dataSet)
        {
            try
            {
                if (!String.IsNullOrEmpty(dataSet.ETHNIC_CODE))
                {
                    var ethnic = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).SingleOrDefault(o => o.ETHNIC_CODE == dataSet.ETHNIC_CODE);
                    if (ethnic != null)
                    {
                        this.cboEthnic.EditValue = ethnic.ETHNIC_NAME;
                        this.txtEthnicCode.Text = ethnic.ETHNIC_CODE;
                    }
                }
                else if (!String.IsNullOrEmpty(dataSet.ETHNIC_NAME))
                {
                    var ethnic = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).SingleOrDefault(o => o.ETHNIC_NAME == dataSet.ETHNIC_NAME);
                    if (ethnic != null)
                    {
                        this.cboEthnic.EditValue = ethnic.ETHNIC_NAME;
                        this.txtEthnicCode.Text = ethnic.ETHNIC_CODE;
                    }
                }
                else
                {
                    LoadEthnicBase();
                }
                //this.txtEthnicCode.TabIndex = this.cboEthnic.TabIndex = this.TabIndex;
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
                if (this.cboEthnic.EditValue != null)
                {
                    dataGet.ETHNIC_NAME = this.cboEthnic.Text;
                    dataGet.ETHNIC_CODE = this.txtEthnicCode.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return dataGet;
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

        public void IsValidateCombo(bool _isValidate)
        {
            try
            {
                if (_isValidate == true)
                {
                    lciEthnic.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidateEthnic();
                }
                else
                {
                    lciEthnic.AppearanceItemCaption.ForeColor = Color.Black;
                    ResetRequiredField();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ResetRequiredField()
        {
            try
            {
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProvider1, this.dxErrorProvider1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateEthnic()
        {
            Valid_Ethnic_Control validateProvince = new Valid_Ethnic_Control();
            validateProvince.cboEthnic = this.cboEthnic;
            validateProvince.txtEthnic = this.txtEthnicCode;
            validateProvince.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
            validateProvince.ErrorType = ErrorType.Warning;
            this.dxValidationProvider1.SetValidationRule(txtEthnicCode, validateProvince);
        }

        internal bool ValidateRequiredField()
        {
            bool valid = true;
            try
            {
                if (!this.dxValidationProvider1.Validate())
                {
                    IList<Control> invalidControls = this.dxValidationProvider1.GetInvalidControls();
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
        internal void DisposeControl()
        {
            try
            {
                dlgFocusNextUserControl = null;
                _shareMethod = null;
                this.txtEthnicCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtEthnicCode_PreviewKeyDown);
                this.cboEthnic.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboEthnic_Closed);
                this.cboEthnic.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.cboEthnic_KeyUp);
                this.Load -= new System.EventHandler(this.UCEthnic_Load);
                dxErrorProvider1 = null;
                dxValidationProvider1 = null;
                lciEthnic = null;
                layoutControlItem1 = null;
                txtEthnicCode = null;
                cboEthnic = null;
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

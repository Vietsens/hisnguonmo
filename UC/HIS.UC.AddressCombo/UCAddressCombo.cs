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
using HIS.UC.AddressCombo.ADO;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors;
using Inventec.Common.Controls.PopupLoader;
using Inventec.Common.Logging;
using DevExpress.XtraEditors.Controls;
using SDA.EFMODEL.DataModels;
//using HIS.UC.AddressCombo.ADO;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.DelegateRegister;
using HIS.Desktop.Utility;
using HIS.UC.UCAddressCombo.Valid;
using DevExpress.XtraEditors.DXErrorProvider;
using MOS.SDO;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.Library.RegisterConfig;

namespace HIS.UC.AddressCombo
{
    public partial class UCAddressCombo : UserControlBase
    {
        #region Declare

        DelegateFocusNextUserControl dlgFocusNextUserControl;
        DelegateSetAddressUCHein dlgSetAddressUCHein;
        DelegateSetAddressUCPlusInfo dlgSetAddressUCProvinceOfBirth;
        DelegateSendCodeProvince dlgSendCodeProvince;
        DelegateSendCardSDO dlgSendCardSDO;
        public bool isReadCard = false;
        public bool isOldPatient = false;
        public bool isPatientBHYT = false;

        int positionHandleControl = -1;

        bool isSearchOrderByXHT = false;
        List<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO> workingCommuneADO;
        HisPatientSDO currentPatientSDO { get; set; }
        bool IsPressEnter { get; set; }
        #endregion

        #region Contructor -Load

        public UCAddressCombo()
            : base("HIS.Desktop.Plugins.RegisterV2", "UCAddressCombo")
        {
            Inventec.Common.Logging.LogSystem.Debug("UCAddressCombo.InitializeComponent------1-");
            InitializeComponent();
            Inventec.Common.Logging.LogSystem.Debug("UCAddressCombo.InitializeComponent------2-");
        }

        private void UCAddressCombo_Load(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("UCAddressCombo_Load------1-");
                this.workingCommuneADO = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>();
                //SetCaptionByLanguageKey();
                SetCaptionByLanguageKeyNew();
                ShowHideControlAddress();
                SetDefaultDataToControlV2();
                this.IsValidateAddressCombo(HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.Validate__T_H_X);      

                Inventec.Common.Logging.LogSystem.Debug("UCAddressCombo_Load------2-");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    
        private void ValidControl(DevExpress.XtraEditors.TextEdit txtEdit, int maxlength, bool isVali)
        {
            try
            {
                TextEditMaxLengthValidationRule _Rule = new TextEditMaxLengthValidationRule();
                _Rule.txtEdit = txtEdit;
                _Rule.maxlength = maxlength;
                _Rule.isVali = isVali;
                _Rule.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                _Rule.ErrorType = ErrorType.Warning;
                this.dxValidationProviderControl.SetValidationRule(txtEdit, _Rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ShowHideControlAddress()
        {
            try
            {
                isSearchOrderByXHT = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS_DESKTOP_REGISTER__SEARCH_CODE__X/H/T") == "1" ? true : false;
                if (isSearchOrderByXHT)
                {
                    lciTHX.Text = "X/H/T:";
                }
                else
                {
                    lciTHX.Text = "T/H/X:";
                }

                // nếu hiển thị địa chỉ ở trên đầu thì biến địa chỉ dưới thành số điện thoại, ẩn số điện thoại đi.
                if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.IsShowLineFirstAddress == 2)
                {
                    this.lciAddress2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.lciAddress.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.lciPhone.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                    //ẩn control thì đổi sang tên của phone để ẩn.
                    string phoneName = this.lciPhone.Name;
                    this.lciPhone.Name += "1";
                    this.lciAddress.Name = phoneName;

                    emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    ValidateMaxlengthAddress(this.txtAddress2, 200);
                    ValidateMaxlengthAddress(this.txtAddress, 12);
                }
                else
                {
                    this.lciAddress.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.lciAddress2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    ValidateMaxlengthAddress(this.txtAddress, 200);

                    bool validPhone = false;
                    if (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PhoneRequired == "1")
                    {
                        lciPhone.AppearanceItemCaption.ForeColor = Color.Maroon;
                        validPhone = true;
                    }

                    ValidateMaxlengthAddress(this.txtPhone, 12, validPhone);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.ResourceUCAddressCombo = new ResourceManager("HIS.UC.AddressCombo.Resources.Lang", typeof(HIS.UC.AddressCombo.UCAddressCombo).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.lcUCAddressCombo.Text = Inventec.Common.Resource.Get.Value("UCAddressCombo.lcUCAddressCombo.Text", Resources.ResourceLanguageManager.ResourceUCAddressCombo, LanguageManager.GetCulture());
                this.cboTHX.Properties.NullText = Inventec.Common.Resource.Get.Value("UCAddressCombo.cboTHX.Properties.NullText", Resources.ResourceLanguageManager.ResourceUCAddressCombo, LanguageManager.GetCulture());
                this.cboDistrict.Properties.NullText = Inventec.Common.Resource.Get.Value("UCAddressCombo.cboDistrict.Properties.NullText", Resources.ResourceLanguageManager.ResourceUCAddressCombo, LanguageManager.GetCulture());
                this.cboCommune.Properties.NullText = Inventec.Common.Resource.Get.Value("UCAddressCombo.cboCommune.Properties.NullText", Resources.ResourceLanguageManager.ResourceUCAddressCombo, LanguageManager.GetCulture());
                this.cboProvince.Properties.NullText = Inventec.Common.Resource.Get.Value("UCAddressCombo.cboProvince.Properties.NullText", Resources.ResourceLanguageManager.ResourceUCAddressCombo, LanguageManager.GetCulture());
                this.lcgAddress.Text = Inventec.Common.Resource.Get.Value("UCAddressCombo.lcgAddress.Text", Resources.ResourceLanguageManager.ResourceUCAddressCombo, LanguageManager.GetCulture());
                this.lciTHX.Text = Inventec.Common.Resource.Get.Value("UCAddressCombo.lciTHX.Text", Resources.ResourceLanguageManager.ResourceUCAddressCombo, LanguageManager.GetCulture());
                this.lciProvince.Text = Inventec.Common.Resource.Get.Value("UCAddressCombo.lciProvince.Text", Resources.ResourceLanguageManager.ResourceUCAddressCombo, LanguageManager.GetCulture());
                this.lciCommune.Text = Inventec.Common.Resource.Get.Value("UCAddressCombo.lciCommune.Text", Resources.ResourceLanguageManager.ResourceUCAddressCombo, LanguageManager.GetCulture());
                this.lciAddress2.Text = Inventec.Common.Resource.Get.Value("UCAddressCombo.lciAddress2.Text", Resources.ResourceLanguageManager.ResourceUCAddressCombo, LanguageManager.GetCulture());
                this.lciDistrict.Text = Inventec.Common.Resource.Get.Value("UCAddressCombo.lciDistrict.Text", Resources.ResourceLanguageManager.ResourceUCAddressCombo, LanguageManager.GetCulture());
                this.lciAddress.Text = Inventec.Common.Resource.Get.Value("UCAddressCombo.lciAddress.Text", Resources.ResourceLanguageManager.ResourceUCAddressCombo, LanguageManager.GetCulture());
                this.lciPhone.Text = Inventec.Common.Resource.Get.Value("UCAddressCombo.lciPhone.Text", Resources.ResourceLanguageManager.ResourceUCAddressCombo, LanguageManager.GetCulture());

                if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.IsShowLineFirstAddress == 2)
                {
                    this.lciAddress.Text = Inventec.Common.Resource.Get.Value("UCAddressCombo.lciAddress.Phone.Text", Resources.ResourceLanguageManager.ResourceUCAddressCombo, LanguageManager.GetCulture());
                    this.lciAddress.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCAddressCombo.lciAddress.Phone.Tooltip.Text", Resources.ResourceLanguageManager.ResourceUCAddressCombo, LanguageManager.GetCulture());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCAddressCombo
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.ResourceUCAddressCombo = new ResourceManager("HIS.UC.AddressCombo.Resources.Lang", typeof(UCAddressCombo).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.lcUCAddressCombo.Text = Inventec.Common.Resource.Get.Value("UCAddressCombo.lcUCAddressCombo.Text", Resources.ResourceLanguageManager.ResourceUCAddressCombo, LanguageManager.GetCulture());
                this.cboTHX.Properties.NullText = Inventec.Common.Resource.Get.Value("UCAddressCombo.cboTHX.Properties.NullText", Resources.ResourceLanguageManager.ResourceUCAddressCombo, LanguageManager.GetCulture());
                this.cboDistrict.Properties.NullText = Inventec.Common.Resource.Get.Value("UCAddressCombo.cboDistrict.Properties.NullText", Resources.ResourceLanguageManager.ResourceUCAddressCombo, LanguageManager.GetCulture());
                this.cboCommune.Properties.NullText = Inventec.Common.Resource.Get.Value("UCAddressCombo.cboCommune.Properties.NullText", Resources.ResourceLanguageManager.ResourceUCAddressCombo, LanguageManager.GetCulture());
                this.cboProvince.Properties.NullText = Inventec.Common.Resource.Get.Value("UCAddressCombo.cboProvince.Properties.NullText", Resources.ResourceLanguageManager.ResourceUCAddressCombo, LanguageManager.GetCulture());
                this.lcgAddress.Text = Inventec.Common.Resource.Get.Value("UCAddressCombo.lcgAddress.Text", Resources.ResourceLanguageManager.ResourceUCAddressCombo, LanguageManager.GetCulture());
                this.lciTHX.Text = Inventec.Common.Resource.Get.Value("UCAddressCombo.lciTHX.Text", Resources.ResourceLanguageManager.ResourceUCAddressCombo, LanguageManager.GetCulture());
                this.lciProvince.Text = Inventec.Common.Resource.Get.Value("UCAddressCombo.lciProvince.Text", Resources.ResourceLanguageManager.ResourceUCAddressCombo, LanguageManager.GetCulture());
                this.lciCommune.Text = Inventec.Common.Resource.Get.Value("UCAddressCombo.lciCommune.Text", Resources.ResourceLanguageManager.ResourceUCAddressCombo, LanguageManager.GetCulture());
                this.lciAddress2.Text = Inventec.Common.Resource.Get.Value("UCAddressCombo.lciAddress2.Text", Resources.ResourceLanguageManager.ResourceUCAddressCombo, LanguageManager.GetCulture());
                this.lciDistrict.Text = Inventec.Common.Resource.Get.Value("UCAddressCombo.lciDistrict.Text", Resources.ResourceLanguageManager.ResourceUCAddressCombo, LanguageManager.GetCulture());
                this.lciAddress.Text = Inventec.Common.Resource.Get.Value("UCAddressCombo.lciAddress.Text", Resources.ResourceLanguageManager.ResourceUCAddressCombo, LanguageManager.GetCulture());
                this.lciPhone.Text = Inventec.Common.Resource.Get.Value("UCAddressCombo.lciPhone.Text", Resources.ResourceLanguageManager.ResourceUCAddressCombo, LanguageManager.GetCulture());

                if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.IsShowLineFirstAddress == 2)
                {
                    this.lciAddress.Text = Inventec.Common.Resource.Get.Value("UCAddressCombo.lciAddress.Phone.Text", Resources.ResourceLanguageManager.ResourceUCAddressCombo, LanguageManager.GetCulture());
                    this.lciAddress.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCAddressCombo.lciAddress.Phone.Tooltip.Text", Resources.ResourceLanguageManager.ResourceUCAddressCombo, LanguageManager.GetCulture());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        #endregion

        #region Event Control

        private void cboProvince_Properties_GetNotInListValue(object sender, DevExpress.XtraEditors.Controls.GetNotInListValueEventArgs e)
        {
            try
            {
                if (e.FieldName == "RENDERER_PROVINCE_NAME")
                {
                    var item = ((List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>)this.cboProvince.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0} {1}", "", item.PROVINCE_NAME);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCommune_Properties_GetNotInListValue(object sender, DevExpress.XtraEditors.Controls.GetNotInListValueEventArgs e)
        {
            try
            {
                if (e.FieldName == "RENDERER_COMMUNE_NAME")
                {
                    var item = ((List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>)this.cboCommune.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0} {1}", item.INITIAL_NAME, item.COMMUNE_NAME);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistrict_Properties_GetNotInListValue(object sender, DevExpress.XtraEditors.Controls.GetNotInListValueEventArgs e)
        {
            try
            {
                if (e.FieldName == "RENDERER_DISTRICT_NAME")
                {
                    var item = ((List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>)this.cboDistrict.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0} {1}", item.INITIAL_NAME, item.DISTRICT_NAME);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetSourceValueTHX(List<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO> communeADOs)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("UCAddressCombo--------SetSourceValueTHX------1-");
                if (communeADOs != null)
                {
                    this.InitComboCommonUtil(this.cboTHX, communeADOs, "ID_RAW", "RENDERER_PDC_NAME", 650, "SEARCH_CODE_COMMUNE", 150, "RENDERER_PDC_NAME_UNSIGNED", 5, 0);
                }
                this.cboTHX.EditValue = null;
                this.cboTHX.Properties.Buttons[1].Visible = false;
                this.FocusShowpopupExt(this.cboTHX, false);
                Inventec.Common.Logging.LogSystem.Debug("UCAddressCombo--------SetSourceValueTHX------2-");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMaTHX_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string maTHX = (sender as DevExpress.XtraEditors.TextEdit).Text.Trim();
                    if (String.IsNullOrEmpty(maTHX))
                    {
                        this.SetSourceValueTHX(this.workingCommuneADO);
                        return;
                    }
                    this.SetSourceValueTHX(this.workingCommuneADO);//Load lai trong TH cbo bi set lai dataSource
                    this.cboTHX.EditValue = null;
                    List<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO> listResult = this.workingCommuneADO
                        .Where(o => (o.SEARCH_CODE_COMMUNE != null
                            && o.SEARCH_CODE_COMMUNE.ToUpper().StartsWith(maTHX.ToUpper()))).ToList();
                    if (listResult != null && listResult.Count >= 1)
                    {
                        var dataNoCommunes = listResult.Where(o => o.ID < 0).ToList();
                        if (dataNoCommunes != null && dataNoCommunes.Count > 1)
                        {
                            this.SetSourceValueTHX(listResult);
                        }
                        else if (dataNoCommunes != null && dataNoCommunes.Count == 1)
                        {
                            this.cboTHX.Properties.Buttons[1].Visible = true;
                            this.cboTHX.EditValue = dataNoCommunes[0].ID_RAW;
                            this.txtMaTHX.Text = dataNoCommunes[0].SEARCH_CODE_COMMUNE;

                            var districtDTO = (cboDistrict.Properties.DataSource as List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>).ToList().FirstOrDefault(o => o.DISTRICT_CODE == dataNoCommunes[0].DISTRICT_CODE);
                            if (districtDTO != null)
                            {
                                this.LoadHuyenCombo("", districtDTO.PROVINCE_CODE, false);
                                var provinceDTO = (cboProvince.Properties.DataSource as List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>).ToList().FirstOrDefault(o => o.PROVINCE_CODE == districtDTO.PROVINCE_CODE);
                                if (provinceDTO != null)
                                {
                                    this.txtProvinceCode.Text = provinceDTO.PROVINCE_CODE;
                                    this.cboProvince.EditValue = provinceDTO.PROVINCE_CODE;
                                }
                                //this.dlgSetAddressUCProvinceOfBirth(districtDTO, true);
                            }
                            this.LoadXaCombo("", dataNoCommunes[0].DISTRICT_CODE, false);

                            this.txtDistrictCode.Text = dataNoCommunes[0].DISTRICT_CODE;
                            this.cboDistrict.EditValue = dataNoCommunes[0].DISTRICT_CODE;

                            this.cboCommune.Focus();
                            this.cboCommune.ShowPopup();
                        }
                        else if (listResult.Count == 1)
                        {
                            this.SetSourceValueTHX(this.workingCommuneADO);
                            this.cboTHX.Properties.Buttons[1].Visible = true;
                            this.cboTHX.EditValue = listResult[0].ID_RAW;
                            this.txtMaTHX.Text = listResult[0].SEARCH_CODE_COMMUNE;

                            var districtDTO = (cboDistrict.Properties.DataSource as List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>).Where(o => o.ID == listResult[0].DISTRICT_ID).FirstOrDefault();
                            if (districtDTO != null)
                            {
                                this.LoadHuyenCombo("", districtDTO.PROVINCE_CODE, false); 
                                var provinceDTO = (cboProvince.Properties.DataSource as List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>).ToList().FirstOrDefault(o => o.PROVINCE_CODE == districtDTO.PROVINCE_CODE);
                                if (provinceDTO != null)
                                {
                                    this.txtProvinceCode.Text = provinceDTO.PROVINCE_CODE;
                                    this.cboProvince.EditValue = provinceDTO.PROVINCE_CODE;
                                }
                                //this.dlgSetAddressUCProvinceOfBirth(districtDTO, true);
                            }
                            this.LoadXaCombo("", listResult[0].DISTRICT_CODE, false);
                            if ((cboDistrict.Properties.DataSource as List<V_SDA_DISTRICT>).Exists(o => o.DISTRICT_CODE == listResult[0].DISTRICT_CODE))
                            {
                                this.txtDistrictCode.Text = listResult[0].DISTRICT_CODE;
                                this.cboDistrict.EditValue = listResult[0].DISTRICT_CODE;
                            }
                            else
                            {
                                this.cboDistrict.EditValue = null;
                                this.txtDistrictCode.Text = null;
                            }

                            if ((cboCommune.Properties.DataSource as List<V_SDA_COMMUNE>).Exists(o => o.COMMUNE_CODE == listResult[0].COMMUNE_CODE))
                            {
                                this.txtCommuneCode.Text = listResult[0].COMMUNE_CODE;
                                this.cboCommune.EditValue = listResult[0].COMMUNE_CODE;
                            }
                            else
                            {
                                this.cboCommune.EditValue = null;
                                this.txtCommuneCode.Text = null;
                            }

                            if (this.cboProvince.EditValue != null
                                && this.cboDistrict.EditValue != null
                                && this.cboCommune.EditValue != null)
                            {
                                FocusToAddress();
                            }
                            else
                            {
                                FocusToCommune();
                            }
                        }
                        else
                        {
                            this.SetSourceValueTHX(listResult);
                        }
                    }
                    else
                    {
                        //this.SetSourceValueTHX(null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTHX_Properties_GetNotInListValue(object sender, DevExpress.XtraEditors.Controls.GetNotInListValueEventArgs e)
        {
            try
            {
                if (e.FieldName == "RENDERER_PDC_NAME")
                {
                    var item = ((List<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>)this.cboTHX.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                    //e.Value = string.Format("{0} - {1} {2}{3}", item.PROVINCE_NAME, item.DISTRICT_INITIAL_NAME, item.DISTRICT_NAME, (String.IsNullOrEmpty(item.COMMUNE_NAME) ? "" : " - " + item.INITIAL_NAME + " " + item.COMMUNE_NAME));
                    {
                        if (isSearchOrderByXHT)
                        {
                            string x1 = (String.IsNullOrEmpty(item.COMMUNE_NAME) ? "" : "" + item.INITIAL_NAME + " " + item.COMMUNE_NAME);
                            string h1 = (String.IsNullOrEmpty(item.DISTRICT_INITIAL_NAME) ? "" : (String.IsNullOrEmpty(x1) ? "" : " - ") + item.DISTRICT_INITIAL_NAME) + (String.IsNullOrEmpty(item.DISTRICT_NAME) ? "" : " " + item.DISTRICT_NAME);
                            string t1 = (String.IsNullOrEmpty(item.PROVINCE_NAME) ? "" : " - " + item.PROVINCE_NAME);
                            e.Value = string.Format("{0}{1}{2}", x1, h1, t1);
                        }
                        else
                        {
                            string t1 = item.PROVINCE_NAME;

                            string h1 = (String.IsNullOrEmpty(item.DISTRICT_INITIAL_NAME) ? "" : " - " + item.DISTRICT_INITIAL_NAME);
                            string h2 = !String.IsNullOrEmpty(item.DISTRICT_NAME) ?
                                String.IsNullOrEmpty(h1) ? "- " + item.DISTRICT_NAME : item.DISTRICT_NAME : "";

                            string x1 = (String.IsNullOrEmpty(item.COMMUNE_NAME) ? "" : " - " + item.INITIAL_NAME + " " + item.COMMUNE_NAME);

                            e.Value = string.Format("{0}{1} {2}{3}", t1, h1, h2, x1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboTHX_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.cboTHX.EditValue = null;
                    this.cboTHX.Properties.Buttons[1].Visible = false;
                    this.txtMaTHX.Text = "";

                    this.SetValueHeinAddressByAddressOfPatient();
                    this.SetValueForUCPlusInfo();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTHX_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboTHX.EditValue != null)
                    {
                        this.cboTHX.Properties.Buttons[1].Visible = true;
                        HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO commune = workingCommuneADO.SingleOrDefault(o => o.ID_RAW == (this.cboTHX.EditValue ?? "").ToString());
                        if (commune != null)
                        {
                            //Trường hợp chọn huyện/xã sẽ tự động điền thông tin vào ô tỉnh/huyện/xã & focus xuống ô địa chỉ
                            this.txtMaTHX.Text = commune.SEARCH_CODE_COMMUNE;
                            if (!String.IsNullOrEmpty(commune.DISTRICT_CODE))
                            {
                                var districtDTO = (cboDistrict.Properties.DataSource as List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>).ToList().FirstOrDefault(o => o.DISTRICT_CODE == commune.DISTRICT_CODE);
                                if (districtDTO != null)
                                {
                                    this.LoadHuyenCombo("", districtDTO.PROVINCE_CODE, false);
                                    var provinceDTO = (cboProvince.Properties.DataSource as List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>).ToList().FirstOrDefault(o => o.PROVINCE_CODE == districtDTO.PROVINCE_CODE);
                                    if (provinceDTO != null)
                                    {
                                        this.txtProvinceCode.Text = provinceDTO.PROVINCE_CODE;
                                        this.cboProvince.EditValue = provinceDTO.PROVINCE_CODE;
                                    }
                                    //this.dlgSetAddressUCProvinceOfBirth(districtDTO, true);
                                }
                                this.LoadXaCombo("", commune.DISTRICT_CODE, false);
                                if ((cboDistrict.Properties.DataSource as List<V_SDA_DISTRICT>).Exists(o => o.DISTRICT_CODE == commune.DISTRICT_CODE))
                                {
                                    this.txtDistrictCode.Text = commune.DISTRICT_CODE;
                                    this.cboDistrict.EditValue = commune.DISTRICT_CODE;
                                }
                                else
                                {
                                    this.cboDistrict.EditValue = null;
                                    this.txtDistrictCode.Text = null;
                                }
                                if (commune.ID < 0)
                                {
                                    FocusToAddress();
                                }
                                else
                                {
                                    if ((cboCommune.Properties.DataSource as List<V_SDA_COMMUNE>).Exists(o => o.COMMUNE_CODE == commune.COMMUNE_CODE))
                                    {
                                        this.txtCommuneCode.Text = commune.COMMUNE_CODE;
                                        this.cboCommune.EditValue = commune.COMMUNE_CODE;
                                    }
                                    else
                                    {
                                        this.cboCommune.EditValue = null;
                                        this.txtCommuneCode.Text = null;
                                    }
                                    if (this.cboProvince.EditValue != null
                                        && this.cboDistrict.EditValue != null
                                        && this.cboCommune.EditValue != null)
                                    {
                                        FocusToAddress();
                                    }
                                    else
                                    {
                                        FocusToCommune();
                                    }
                                }
                            }
                            //Trường hợp chọn 1 dòng là tỉnh => chỉ điền giá trị vào ô tỉnh & focus xuống ô địa chỉ
                            else
                            {
                                this.LoadHuyenCombo("", commune.PROVINCE_CODE, false);
                                var provinceDTO = (cboProvince.Properties.DataSource as List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>).ToList().FirstOrDefault(o => o.PROVINCE_CODE == commune.PROVINCE_CODE);
                                if (provinceDTO != null)
                                {
                                    this.txtProvinceCode.Text = provinceDTO.PROVINCE_CODE;
                                    this.cboProvince.EditValue = provinceDTO.PROVINCE_CODE;
                                }
                                FocusToAddress();
                            }
                        }
                    }
                    else
                    {
                        if (this.cboProvince.EditValue != null
                            && this.cboDistrict.EditValue != null
                            && this.cboCommune.EditValue != null)
                        {
                            FocusToAddress();
                        }
                        else
                        {
                            FocusToCommune();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTHX_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboTHX.EditValue != null)
                    {
                        this.cboTHX.Properties.Buttons[1].Visible = true;
                        HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO commune = workingCommuneADO.SingleOrDefault(o => o.ID_RAW == (this.cboTHX.EditValue ?? "").ToString());
                        if (commune != null)
                        {
                            this.txtMaTHX.Text = commune.SEARCH_CODE_COMMUNE;
                            if (!String.IsNullOrEmpty(commune.DISTRICT_CODE))
                            {
                                var districtDTO = (cboDistrict.Properties.DataSource as List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>).ToList().FirstOrDefault(o => o.DISTRICT_CODE == commune.DISTRICT_CODE);
                                if (districtDTO != null)
                                {
                                    this.LoadHuyenCombo("", districtDTO.PROVINCE_CODE, false);
                                    var provinceDTO = (cboProvince.Properties.DataSource as List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>).ToList().FirstOrDefault(o => o.PROVINCE_CODE == districtDTO.PROVINCE_CODE);
                                    if (provinceDTO != null)
                                    {
                                        this.txtProvinceCode.Text = provinceDTO.PROVINCE_CODE;
                                        this.cboProvince.EditValue = provinceDTO.PROVINCE_CODE;
                                    }
                                    //this.dlgSetAddressUCProvinceOfBirth(districtDTO, true);
                                }
                                this.LoadXaCombo("", commune.DISTRICT_CODE, false);
                                if ((cboDistrict.Properties.DataSource as List<V_SDA_DISTRICT>).Exists(o => o.DISTRICT_CODE == commune.DISTRICT_CODE))
                                {
                                    this.txtDistrictCode.Text = commune.DISTRICT_CODE;
                                    this.cboDistrict.EditValue = commune.DISTRICT_CODE;
                                }
                                else
                                {
                                    this.cboDistrict.EditValue = null;
                                    this.txtDistrictCode.Text = null;
                                }
                                if (commune.ID < 0)
                                {
                                    this.txtAddress.Focus();
                                    this.txtAddress.SelectAll();
                                }
                                else
                                {
                                    if ((cboCommune.Properties.DataSource as List<V_SDA_COMMUNE>).Exists(o => o.COMMUNE_CODE == commune.COMMUNE_CODE))
                                    {
                                        this.txtCommuneCode.Text = commune.COMMUNE_CODE;
                                        this.cboCommune.EditValue = commune.COMMUNE_CODE;
                                    }
                                    else
                                    {
                                        this.cboCommune.EditValue = null;
                                        this.txtCommuneCode.Text = null;
                                    }
                                    if (this.cboProvince.EditValue != null
                                        && this.cboDistrict.EditValue != null
                                        && this.cboCommune.EditValue != null)
                                    {
                                        FocusToAddress();
                                    }
                                    else
                                    {
                                        FocusToCommune();
                                    }
                                }
                            }
                            //Trường hợp chọn 1 dòng là tỉnh => chỉ điền giá trị vào ô tỉnh & focus xuống ô địa chỉ
                            else
                            {
                                this.LoadHuyenCombo("", commune.PROVINCE_CODE, false);
                                var provinceDTO = (cboProvince.Properties.DataSource as List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>).ToList().FirstOrDefault(o => o.PROVINCE_CODE == commune.PROVINCE_CODE);
                                if (provinceDTO != null)
                                {
                                    this.txtProvinceCode.Text = provinceDTO.PROVINCE_CODE;
                                    this.cboProvince.EditValue = provinceDTO.PROVINCE_CODE;
                                }
                                FocusToAddress();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTHXFilter_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.A)
                {
                    this.cboTHX.Focus();
                    this.cboTHX.SelectAll();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboTHX.EditValue != null)
                    {
                        this.cboTHX.Properties.Buttons[1].Visible = true;
                        HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO commune = workingCommuneADO.SingleOrDefault(o => o.ID_RAW == (this.cboTHX.EditValue ?? "").ToString());
                        if (commune != null)
                        {
                            this.txtMaTHX.Text = commune.SEARCH_CODE_COMMUNE;
                            if (!String.IsNullOrEmpty(commune.DISTRICT_CODE))
                            {
                                var districtDTO = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().SingleOrDefault(o => o.DISTRICT_CODE == commune.DISTRICT_CODE);
                                if (districtDTO != null)
                                {
                                    this.LoadHuyenCombo("", districtDTO.PROVINCE_CODE, false);
                                    this.txtProvinceCode.Text = districtDTO.PROVINCE_CODE;
                                    this.cboProvince.EditValue = districtDTO.PROVINCE_CODE;
                                    //this.dlgSetAddressUCProvinceOfBirth(districtDTO, true);
                                }
                                this.LoadXaCombo("", commune.DISTRICT_CODE, false);

                                this.txtDistrictCode.Text = commune.DISTRICT_CODE;
                                this.cboDistrict.EditValue = commune.DISTRICT_CODE;

                                if (commune.ID < 0)
                                {
                                    this.txtAddress.Focus();
                                    this.txtAddress.SelectAll();
                                }
                                else
                                {
                                    this.txtCommuneCode.Text = commune.COMMUNE_CODE;
                                    this.cboCommune.EditValue = commune.COMMUNE_CODE;
                                    if (this.cboProvince.EditValue != null
                                        && this.cboDistrict.EditValue != null
                                        && this.cboCommune.EditValue != null)
                                    {
                                        FocusToAddress();
                                    }
                                    else
                                    {
                                        FocusToCommune();
                                    }
                                }
                            }
                            //Trường hợp chọn 1 dòng là tỉnh => chỉ điền giá trị vào ô tỉnh & focus xuống ô địa chỉ
                            else
                            {
                                this.LoadHuyenCombo("", commune.PROVINCE_CODE, false);

                                this.txtProvinceCode.Text = commune.PROVINCE_CODE;
                                this.cboProvince.EditValue = commune.PROVINCE_CODE;

                                FocusToAddress();
                            }
                        }
                    }
                }
                else
                {
                    this.cboTHX.ShowPopup();
                    //PopupProcess.SelectFirstRowPopup(this.cboTHX);
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTHXFilter_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboTHX.EditValue != null)
                    {
                        this.cboTHX.Properties.Buttons[1].Visible = true;
                        HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO commune = workingCommuneADO.SingleOrDefault(o => o.ID_RAW == (this.cboTHX.EditValue ?? "").ToString());
                        if (commune != null)
                        {
                            //Trường hợp chọn huyện/xã sẽ tự động điền thông tin vào ô tỉnh/huyện/xã & focus xuống ô địa chỉ
                            this.txtMaTHX.Text = commune.SEARCH_CODE_COMMUNE;
                            if (!String.IsNullOrEmpty(commune.DISTRICT_CODE))
                            {
                                var districtDTO = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().SingleOrDefault(o => o.DISTRICT_CODE == commune.DISTRICT_CODE);
                                if (districtDTO != null)
                                {
                                    this.LoadHuyenCombo("", districtDTO.PROVINCE_CODE, false);
                                    this.txtProvinceCode.Text = districtDTO.PROVINCE_CODE;
                                    this.cboProvince.EditValue = districtDTO.PROVINCE_CODE;
                                    //this.dlgSetAddressUCProvinceOfBirth(districtDTO, true);
                                }
                                this.LoadXaCombo("", commune.DISTRICT_CODE, false);

                                this.txtDistrictCode.Text = commune.DISTRICT_CODE;
                                this.cboDistrict.EditValue = commune.DISTRICT_CODE;

                                if (commune.ID < 0)
                                {
                                    FocusToAddress();
                                }
                                else
                                {
                                    this.txtCommuneCode.Text = commune.COMMUNE_CODE;
                                    this.cboCommune.EditValue = commune.COMMUNE_CODE;
                                    if (this.cboProvince.EditValue != null
                                        && this.cboDistrict.EditValue != null
                                        && this.cboCommune.EditValue != null)
                                    {
                                        FocusToAddress();
                                    }
                                    else
                                    {
                                        FocusToCommune();
                                    }
                                }
                            }
                            //Trường hợp chọn 1 dòng là tỉnh => chỉ điền giá trị vào ô tỉnh & focus xuống ô địa chỉ
                            else
                            {
                                this.LoadHuyenCombo("", commune.PROVINCE_CODE, false);

                                this.txtProvinceCode.Text = commune.PROVINCE_CODE;
                                this.cboProvince.EditValue = commune.PROVINCE_CODE;

                                FocusToAddress();
                            }
                        }
                    }
                    else
                    {
                        if (this.cboProvince.EditValue != null
                            && this.cboDistrict.EditValue != null
                            && this.cboCommune.EditValue != null)
                        {
                            FocusToAddress();
                        }
                        else
                        {
                            FocusToCommune();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTHXFilter_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.cboTHX.EditValue = null;
                    this.cboTHX.Properties.Buttons[1].Visible = false;
                    this.txtMaTHX.Text = "";

                    this.SetValueHeinAddressByAddressOfPatient();
                    this.SetValueForUCPlusInfo();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtProvinceCode_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.txtProvinceCode.Text))
                {
                    this.cboProvince.Properties.DataSource = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtProvinceCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.LoadTinhThanhCombo((sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper(), true);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboProvince_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboProvince.EditValue != null
                        && this.cboProvince.EditValue != this.cboProvince.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().SingleOrDefault(o => o.PROVINCE_CODE == cboProvince.EditValue.ToString());
                        if (province != null)
                        {
                            this.LoadHuyenCombo("", province.PROVINCE_CODE, false);
                            this.txtProvinceCode.Text = province.SEARCH_CODE;
                            //this.dlgSetAddressUCProvinceOfBirth(province, true);
                        }
                    }
                    this.txtDistrictCode.Text = "";
                    FocusToDistrict();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProvince_EditValueChanged(object sender, EventArgs e)
        {
            try
            {

                if (this.cboProvince.EditValue != null && !(cboProvince.EditValue.Equals(this.cboProvince.OldEditValue == null ? "" : this.cboProvince.OldEditValue)))
                {
                    this.cboCommune.Properties.DataSource = null;
                    this.cboCommune.EditValue = null;
                    this.txtCommuneCode.Text = "";
                    this.txtDistrictCode.Text = "";
                    this.cboDistrict.EditValue = null;

                    this.SetValueHeinAddressByAddressOfPatient();
                    this.SetValueForUCPlusInfo();
                }
                if (cboProvince.EditValue == null || (cboProvince.EditValue != null && !(cboProvince.Properties.DataSource as List<V_SDA_PROVINCE>).Exists(o => o.PROVINCE_CODE == cboProvince.EditValue.ToString())))
                {
                    this.cboProvince.EditValue = null;
                    this.txtDistrictCode.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Goi ham gan du lieu thong tinh hanh chinh tu su kien cboProvince_EditValuechanged khong thanh cong : \n" + ex);
            }
        }

        private void cboProvince_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboProvince.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().SingleOrDefault(o => o.PROVINCE_CODE == this.cboProvince.EditValue.ToString());
                        if (province != null)
                        {
                            this.LoadHuyenCombo("", province.PROVINCE_CODE, false);
                            this.txtProvinceCode.Text = province.SEARCH_CODE;
                            this.txtDistrictCode.Text = "";
                            //this.dlgSetAddressUCProvinceOfBirth(province,true);
                            FocusToDistrict();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDistrictCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string provinceCode = "";
                    if (this.cboProvince.EditValue != null)
                    {
                        provinceCode = this.cboProvince.EditValue.ToString();
                    }
                    this.LoadHuyenCombo((sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper(), provinceCode, true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistrict_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboDistrict.EditValue != null
                        && this.cboDistrict.EditValue != this.cboDistrict.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList()
                            .SingleOrDefault(o => o.DISTRICT_CODE == this.cboDistrict.EditValue.ToString()
                                && (String.IsNullOrEmpty((this.cboProvince.EditValue ?? "").ToString()) || o.PROVINCE_CODE == (this.cboProvince.EditValue ?? "").ToString()));
                        if (district != null)
                        {
                            if (String.IsNullOrEmpty((this.cboProvince.EditValue ?? "").ToString()))
                            {
                                this.txtProvinceCode.Text = district.PROVINCE_CODE;
                                this.cboProvince.EditValue = district.PROVINCE_CODE;
                            }
                            this.LoadXaCombo("", district.DISTRICT_CODE, false);
                            this.txtDistrictCode.Text = district.SEARCH_CODE;
                            this.cboDistrict.EditValue = district.DISTRICT_CODE;
                            this.cboCommune.EditValue = null;
                            this.txtCommuneCode.Text = "";
                        }
                    }
                    FocusToCommune();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistrict_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.cboDistrict.EditValue != null && !(this.cboDistrict.EditValue.Equals(this.cboDistrict.OldEditValue == null ? "" : this.cboDistrict.OldEditValue)))
                {
                    this.SetValueHeinAddressByAddressOfPatient();
                    this.SetValueForUCPlusInfo();
                }

                if (cboDistrict.EditValue == null || (cboDistrict.EditValue != null && !(cboDistrict.Properties.DataSource as List<V_SDA_DISTRICT>).Exists(o => o.DISTRICT_CODE == cboDistrict.EditValue.ToString())))
                {
                    this.cboDistrict.EditValue = null;
                    this.txtDistrictCode.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDistrict_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboDistrict.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList()
                            .SingleOrDefault(o => o.DISTRICT_CODE == this.cboDistrict.EditValue.ToString()
                               && (String.IsNullOrEmpty((this.cboProvince.EditValue ?? "").ToString()) || o.PROVINCE_CODE == (this.cboProvince.EditValue ?? "").ToString()));
                        if (district != null)
                        {
                            if (String.IsNullOrEmpty((this.cboProvince.EditValue ?? "").ToString()))
                            {
                                this.cboProvince.EditValue = district.PROVINCE_CODE;
                            }
                            this.LoadXaCombo("", district.DISTRICT_CODE, false);
                            this.txtDistrictCode.Text = district.SEARCH_CODE;
                            this.cboCommune.EditValue = null;
                            this.txtCommuneCode.Text = "";
                            FocusToCommune();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCommuneCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string districtCode = "";
                    if (this.cboDistrict.EditValue != null)
                    {
                        districtCode = this.cboDistrict.EditValue.ToString();
                    }
                    this.LoadXaCombo((sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper(), districtCode, true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCommune_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboCommune.EditValue != null
                        && this.cboCommune.EditValue != cboCommune.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList()
                            .SingleOrDefault(o =>
                                o.COMMUNE_CODE == this.cboCommune.EditValue.ToString()
                                    && (String.IsNullOrEmpty((this.cboDistrict.EditValue ?? "").ToString()) || o.DISTRICT_CODE == (this.cboDistrict.EditValue ?? "").ToString())
                                );
                        if (commune != null)
                        {
                            this.txtCommuneCode.Text = commune.SEARCH_CODE;
                            if (String.IsNullOrEmpty((this.cboProvince.EditValue ?? "").ToString()) && String.IsNullOrEmpty((this.cboDistrict.EditValue ?? "").ToString()))
                            {
                                this.cboDistrict.EditValue = commune.DISTRICT_CODE;
                                SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().Where(o => o.ID == commune.DISTRICT_ID).FirstOrDefault();
                                if (district != null)
                                {
                                    this.cboProvince.EditValue = district.PROVINCE_CODE;
                                }
                            }
                        }
                    }
                    FocusToAddress();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCommune_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.cboCommune.EditValue != null && !(this.cboCommune.EditValue.Equals(this.cboCommune.OldEditValue == null ? "" : this.cboCommune.OldEditValue)))
                {
                    this.SetValueHeinAddressByAddressOfPatient();
                    this.SetValueForUCPlusInfo();
                }
                if (cboCommune.EditValue == null || (cboCommune.EditValue != null && !(cboCommune.Properties.DataSource as List<V_SDA_COMMUNE>).Exists(o => o.COMMUNE_CODE == cboCommune.EditValue.ToString())))
                {
                    this.cboCommune.EditValue = null;
                    this.txtCommuneCode.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("cboCommune_EditValueChanged:\n" + ex);
            }
        }

        private void cboCommune_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboCommune.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList()
                            .SingleOrDefault(o =>
                                o.COMMUNE_CODE == this.cboCommune.EditValue.ToString()
                                && (String.IsNullOrEmpty((this.cboDistrict.EditValue ?? "").ToString()) || o.DISTRICT_CODE == (this.cboDistrict.EditValue ?? "").ToString()));
                        if (commune != null)
                        {
                            this.txtCommuneCode.Text = commune.SEARCH_CODE;
                            if (String.IsNullOrEmpty((this.cboProvince.EditValue ?? "").ToString()) && String.IsNullOrEmpty((this.cboDistrict.EditValue ?? "").ToString()))
                            {
                                this.cboDistrict.EditValue = commune.DISTRICT_CODE;
                                SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().Where(o => o.ID == commune.DISTRICT_ID).FirstOrDefault();
                                if (district != null)
                                {
                                    this.cboProvince.EditValue = district.PROVINCE_CODE;
                                }
                            }
                            FocusToAddress();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAddress_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                string address = (String.IsNullOrEmpty(txtAddress.Text) == true ? "" : this.txtAddress.Text);
                if(!IsNotLoadChangeAddressTxt)
                {
                    this.SetValueHeinAddressByAddressOfPatient();
                    this.SetValueForUCPlusInfo();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Goi ham gan du lieu thong tin hanh chinh tu event txtAddress_EditValueChanged khong thanh cong:\n" + ex);
            }
        }

        private void txtAddress_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                //if (e.KeyCode == Keys.Enter && dlgFocusNextUserControl != null)
                //{
                //    this.dlgFocusNextUserControl();
                //}

                if (e.KeyCode == Keys.Enter)
                {
                    if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.IsShowLineFirstAddress == 2)
                    {
                        if (dlgFocusNextUserControl != null)
                        {
                            this.dlgFocusNextUserControl();
                        }
                    }
                    else if (lciPhone.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never && dlgFocusNextUserControl != null)
                    {
                        this.dlgFocusNextUserControl();
                    }
                    else
                    {
                        txtPhone.Focus();
                        txtPhone.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangeReplaceAddress(string cmd, string lever, ref string address)
        {
            try
            {
                if (!string.IsNullOrEmpty(address) && !string.IsNullOrEmpty(cmd))
                {
                    string[] addressSplit = address.Split(new string[] { "," },StringSplitOptions.RemoveEmptyEntries);
                    var datas = addressSplit.Where(p => p.ToLower().Contains(cmd.ToLower()) || cmd.ToLower().Contains(p.ToLower())).ToList();
                    if (datas != null && datas.Count > 0)
                    {
                        if (datas.Count == 1)
                        {
                            string addressNew = "," + datas[0];
                            if (address.Contains(addressNew))
                            {
                                address = address.Replace(addressNew, "");
                            }
                            else
                            {
                                address = address.Replace(datas[0], "");
                            }
                        }
                        else
                        {
                            string addressV2 = lever + " " + cmd;
                            var data = datas.FirstOrDefault(p => p.ToLower().Contains(addressV2.ToLower()));
                            if (data != null)
                            {
                                string addressNew = "," + data;
                                if (address.Contains(addressNew))
                                {
                                    address = address.Replace(addressNew, "");
                                }
                                else
                                {
                                    address = address.Replace(data, "");
                                }
                            }
                            var dataEquals = datas.FirstOrDefault(p => p.Trim().ToLower().Equals(cmd.ToLower()));
                            if (dataEquals != null)
                            {
                                string addressNew = "," + dataEquals;
                                if (address.Contains(addressNew))
                                {
                                    address = address.Replace(addressNew, "");
                                }
                                else
                                {
                                    address = address.Replace(dataEquals, "");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                IsNotLoadChangeAddressTxt = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        bool IsNotLoadChangeAddressTxt = false;
        private void txtAddress2_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                string address = (String.IsNullOrEmpty(txtAddress2.Text) == true ? "" : this.txtAddress2.Text);
                if (!IsNotLoadChangeAddressTxt)
                {
                    this.SetValueHeinAddressByAddressOfPatient();
                    this.SetValueForUCPlusInfo();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Goi ham gan du lieu thong tin hanh chinh tu event txtAddress2_EditValueChanged khong thanh cong:\n" + ex);
            }
        }

        private void txtAddress2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.txtMaTHX.Focus();
                    this.txtMaTHX.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void txtPhone_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    IsPressEnter = true;
                    GetCardSdoByPhone();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void txtPhone_Leave(object sender, EventArgs e)
        {
            try
            {
                if(!IsPressEnter)
                    GetCardSdoByPhone();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void GetCardSdoByPhone()
        {
            try
            {
                if (dlgFocusNextUserControl != null)
                    this.dlgFocusNextUserControl();
                if ((!string.IsNullOrEmpty(txtPhone.Text.Trim()) && txtPhone.Text.Trim().Length == 10) && HisConfigCFG.IsSuggestCardHolderInformationByUsingPhoneNumber /*&& (currentPatientSDO == null || currentPatientSDO.HAS_CARD != 1)*/)
                {
                    CommonParam param = new CommonParam();
                    string phone = txtPhone.Text.Trim();
                    var cardSdoList = new BackendAdapter(param).Get<List<HisCardSDO>>("api/HisCard/GetCardSdoByPhone", ApiConsumers.MosConsumer, phone, param);
                    if (cardSdoList != null && cardSdoList.Count > 0)
                    {
                        frmCard frm = new frmCard(cardSdoList, CardChoice);
                        frm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CardChoice(HisCardSDO obj)
        {
            try
            {
                dlgSendCardSDO(obj);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        private void cboTHX_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.cboTHX.Properties.Buttons[1].Visible = this.cboTHX.EditValue != null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                 Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

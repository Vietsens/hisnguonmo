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
using Inventec.Desktop.Common.Controls.ValidationRule;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Controls.EditorLoader;
using SDA.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors;
using Inventec.Common.Controls.PopupLoader;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.ContactDeclaration.Choice;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.ContactDeclaration.Validate;
using DevExpress.XtraLayout;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using DevExpress.Utils;
using Inventec.Common.Address;

namespace HIS.Desktop.Plugins.ContactDeclaration.UcObject
{
    public partial class UcOrther : UserControl
    {
        private Inventec.Desktop.Common.Modules.Module moduleData;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        string moduleLink = "HIS.Desktop.Plugins.ContactDeclaration";
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        public UpdateVContactPoint updateVContactPoint;
        V_HIS_CONTACT_POINT CurrentContactPoint = new V_HIS_CONTACT_POINT();
        private AddressADO currentAddressADO;
        HIS_CONTACT_POINT ContactPoint = new HIS_CONTACT_POINT();
        List<HIS_GENDER> lstGender = new List<HIS_GENDER>();
        List<HIS.Desktop.LocalStorage.BackendData.ADO.AgeADO> lstAgeADO = new List<LocalStorage.BackendData.ADO.AgeADO>();
        List<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO> lstCommuneADO = new List<LocalStorage.BackendData.ADO.CommuneADO>();
        List<V_SDA_PROVINCE> lstSdaProvince = new List<V_SDA_PROVINCE>();
        List<V_SDA_DISTRICT> lstSdaDistrict = new List<V_SDA_DISTRICT>();
        List<V_SDA_COMMUNE> lstSdaCommune = new List<V_SDA_COMMUNE>();
        int positionHandle = -1;
        bool check = true;
        private bool useNewAddressStructurePatient;
        private bool useNewAddressStructureContact;

        public UcOrther() { }

        public UcOrther(List<HIS_GENDER> Genders, List<HIS.Desktop.LocalStorage.BackendData.ADO.AgeADO> AgeADOs,
            List<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO> CommuneADOs, List<V_SDA_PROVINCE> SdaProvinces,
            List<V_SDA_DISTRICT> SdaDistricts, List<V_SDA_COMMUNE> SdaCommunes,
            UpdateVContactPoint _updateVContactPoint,
            V_HIS_CONTACT_POINT ContactPoint)
        {
            this.lstGender = Genders;
            this.lstAgeADO = AgeADOs;
            this.lstCommuneADO = CommuneADOs;
            this.lstSdaProvince = SdaProvinces;
            this.lstSdaDistrict = SdaDistricts;
            this.lstSdaCommune = SdaCommunes;
            this.CurrentContactPoint = ContactPoint;

            this.updateVContactPoint = _updateVContactPoint;
            InitializeComponent();
        }

        private void UcOrther_Load(object sender, EventArgs e)
        {
            try
            {
                IsChangeStrucAddress = toggleSwitchDataChecked.IsOn;
                IsNotCheckToggleAddress = false;

                // Gọi lại hàm toggle như thể người dùng vừa click
                toggleSwitchDataChecked_Toggled(toggleSwitchDataChecked, EventArgs.Empty);
                LoadComboboxGender(this.lstGender);
                LoadComboboxAgeName(this.lstAgeADO);
                LoadComboboxTHX(this.lstCommuneADO);
                LoadComboboxProvince(this.lstSdaProvince);
                LoadComboboxDistrict(this.lstSdaDistrict);
                LoadComboboxCommune(this.lstSdaCommune);
                if (this.CurrentContactPoint != null && this.CurrentContactPoint.ID > 0)
                {
                    check = false;
                    dataContactPoint(this.CurrentContactPoint.ID);

                    SetDefautValueControl(this.ContactPoint);
                }
                else
                {
                    check = true;
                }

                SetValidate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void dataContactPoint(long? dataId)
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                HisContactPointFilter filter = new HisContactPointFilter();

                filter.ID = dataId;
                var ContactPointdata = new BackendAdapter(paramCommon).Get<List<HIS_CONTACT_POINT>>("/api/HisContactPoint/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                if (ContactPointdata != null && ContactPointdata.Count > 0)
                {
                    ContactPoint = ContactPointdata.FirstOrDefault();
                }
                else
                {
                    ContactPoint = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetDefautValueControl(HIS_CONTACT_POINT data)
        {
            try
            {              
                //this.CurrentContactPoint = data;
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_CONTACT_POINT>(CurrentContactPoint, data);
                if (data != null)
                {
                    this.txtPatientName.Text = data.VIR_FULL_NAME;
                    this.cboGender.EditValue = data.GENDER_ID;
                    this.dtPatientDob.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.DOB.Value).Value;
                    this.txtAddress.Text = data.ADDRESS;
                    this.txtPhone.Text = data.PHONE;

                    this.cboProvince.EditValue = data.PROVINCE_CODE;
                    this.cboDistrict.EditValue = data.DISTRICT_CODE;
                    this.cboCommune.EditValue = data.COMMUNE_CODE;
                }
                else
                {
                    this.txtPatientName.Text = "";
                    this.cboGender.EditValue = null;
                    this.dtPatientDob.EditValue = null;
                    this.txtAddress.Text = "";
                    this.txtPhone.Text = "";
                    this.txtProvinceCode.Text = "";
                    this.txtDistrictCode.Text = "";
                    this.txtCommuneCode.Text = "";
                    this.cboProvince.EditValue = null;
                    this.cboDistrict.EditValue = null;
                    this.cboCommune.EditValue = null;
                    this.toggleSwitchDataChecked.EditValue = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public bool ValidateForm()
        {
            positionHandle = -1;
            return dxValidationProvider1.Validate();
        }

        public void SetValidate()
        {
            try
            {
                ValidateSingleControl(dxValidationProvider1, txtPatientName);

                ValidateSingleControl(dxValidationProvider1, cboGender);

                ValidationSingleControl(dxValidationProvider1, dtPatientDob);

                ValidateSingleControl(dxValidationProvider1, txtAddress);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationSingleControl(DXValidationProvider validate, DateEdit control)
        {
            try
            {
                if (control.Enabled)
                {
                    ValidateDateTime validRule = new ValidateDateTime();
                    validRule.dateEdit = control;
                    //validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    validRule.ErrorType = ErrorType.Warning;
                    validate.SetValidationRule(control, validRule);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateSingleControl(DXValidationProvider validate, Control control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                validate.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboGender.Focus();
                    cboGender.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Load combobox
        public void LoadComboboxGender(List<HIS_GENDER> data)
        {
            try
            {
                this.lstGender = data;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("GENDER_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("GENDER_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("GENDER_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboGender, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void LoadComboboxAgeName(List<HIS.Desktop.LocalStorage.BackendData.ADO.AgeADO> data)
        {
            try
            {

                this.lstAgeADO = data;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("Id", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MoTa", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MoTa", "Id", columnInfos, false, 250);
                ControlEditorLoader.Load(cboAge, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void LoadComboboxTHX(List<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO> data)
        {
            try
            {
                this.lstCommuneADO = data;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SEARCH_CODE_COMMUNE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("RENDERER_PDC_NAME", "", 450, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("RENDERER_PDC_NAME", "ID", columnInfos, false, 550);
                ControlEditorLoader.Load(cboTHX, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void LoadComboboxProvince(List<V_SDA_PROVINCE> data)
        {
            try
            {
                this.lstSdaProvince = data;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SEARCH_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PROVINCE_NAME", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PROVINCE_NAME", "PROVINCE_CODE", columnInfos, false, 350);
                ControlEditorLoader.Load(cboProvince, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void LoadComboboxDistrict(List<V_SDA_DISTRICT> data)
        {
            try
            {
                this.lstSdaDistrict = data;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SEARCH_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("RENDERER_DISTRICT_NAME", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("RENDERER_DISTRICT_NAME", "DISTRICT_CODE", columnInfos, false, 350);
                ControlEditorLoader.Load(cboDistrict, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void LoadComboboxCommune(List<V_SDA_COMMUNE> data)
        {
            try
            {
                this.lstSdaCommune = data;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SEARCH_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("RENDERER_COMMUNE_NAME", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("RENDERER_COMMUNE_NAME", "COMMUNE_CODE", columnInfos, false, 350);
                ControlEditorLoader.Load(cboCommune, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        private void FocusShowpopup(LookUpEdit cboEditor, bool isSelectFirstRow)
        {
            try
            {
                cboEditor.Focus();
                cboEditor.ShowPopup();
                if (isSelectFirstRow)
                    PopupLoader.SelectFirstRowPopup(cboEditor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadTinhThanhCombo(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    this.cboCommune.Properties.DataSource = null;
                    this.cboCommune.EditValue = null;
                    this.txtCommuneCode.Text = "";
                    this.cboDistrict.Properties.DataSource = null;
                    this.cboDistrict.EditValue = null;
                    this.txtDistrictCode.Text = "";
                    this.cboProvince.EditValue = null;
                    this.FocusShowpopup(this.cboProvince, false);
                }
                else
                {
                    List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                    listResult = SdaProvinces.Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).Where(o => o.SEARCH_CODE.Contains(searchCode)).ToList();
                    if (listResult.Count == 1)
                    {
                        this.cboProvince.EditValue = listResult[0].PROVINCE_CODE;
                        this.txtProvinceCode.Text = listResult[0].SEARCH_CODE;
                        if (IsChangeStrucAddress)
                        {
                            LoadXaComboNoDistrict("", listResult[0].PROVINCE_CODE, false);
                        }
                        else
                            this.LoadHuyenCombo("", listResult[0].PROVINCE_CODE, false);
                        if (isExpand)
                        {
                            this.txtDistrictCode.Focus();
                            this.txtDistrictCode.SelectAll();
                        }
                    }
                    else
                    {
                        this.cboCommune.Properties.DataSource = null;
                        this.cboCommune.EditValue = null;
                        this.txtCommuneCode.Text = "";
                        this.cboDistrict.Properties.DataSource = null;
                        this.cboDistrict.EditValue = null;
                        this.txtDistrictCode.Text = "";
                        this.cboProvince.EditValue = null;
                        if (isExpand)
                        {
                            this.cboProvince.Properties.DataSource = listResult;
                            this.FocusShowpopup(this.cboProvince, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadHuyenCombo(string searchCode, string provinceCode, bool isExpand)
        {
            try
            {
                List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
                listResult = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).Where(o => (o.SEARCH_CODE ?? "").ToUpper().Contains(searchCode.ToUpper()) && (provinceCode == "" || o.PROVINCE_CODE == provinceCode)).ToList();
                this.InitComboCommon(this.cboDistrict, listResult, "DISTRICT_CODE", "RENDERER_DISTRICT_NAME", "SEARCH_CODE");
                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(provinceCode) && listResult.Count > 0)
                {
                    this.cboCommune.Properties.DataSource = null;
                    this.cboCommune.EditValue = null;
                    this.txtCommuneCode.Text = "";
                    this.txtDistrictCode.Text = "";
                    this.cboDistrict.EditValue = null;
                    this.FocusShowpopup(this.cboDistrict, false);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        this.cboDistrict.EditValue = listResult[0].DISTRICT_CODE;
                        this.txtDistrictCode.Text = listResult[0].SEARCH_CODE;
                        if (String.IsNullOrEmpty(this.cboProvince.Text))
                        {
                            this.cboProvince.EditValue = listResult[0].PROVINCE_CODE;
                            this.txtProvinceCode.Text = listResult[0].PROVINCE_CODE;
                        }

                        this.LoadXaCombo("", listResult[0].DISTRICT_CODE, false);
                        if (isExpand)
                        {
                            this.txtCommuneCode.Focus();
                            this.txtCommuneCode.SelectAll();
                        }
                    }
                    else
                    {
                        this.cboCommune.Properties.DataSource = null;
                        this.cboCommune.EditValue = null;
                        this.txtCommuneCode.Text = "";
                        this.cboDistrict.EditValue = null;
                        if (isExpand)
                        {
                            this.FocusShowpopup(this.cboDistrict, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadXaComboNoDistrict(string searchCode, string provinceCode, bool isExpand)
        {
            try
            {
                List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE> listResult = SdaCommunes.Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE)
                    .Where(o => ((o.SEARCH_CODE ?? "").Contains(searchCode ?? "") || (o.COMMUNE_CODE == (searchCode ?? "") || string.IsNullOrEmpty(searchCode)))
                        && (String.IsNullOrEmpty(provinceCode) || o.PROVINCE_CODE == provinceCode) && o.IS_NO_DISTRICT == 1).ToList();
                this.InitComboCommon(this.cboCommune, listResult, "COMMUNE_CODE", "RENDERER_COMMUNE_NAME", "SEARCH_CODE");
                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(provinceCode) && listResult.Count > 0)
                {
                    this.cboCommune.EditValue = null;
                    this.txtCommuneCode.Text = "";
                    this.FocusShowpopup(this.cboCommune, false);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        this.cboCommune.EditValue = listResult[0].COMMUNE_CODE;
                        this.txtCommuneCode.Text = listResult[0].COMMUNE_CODE;

                        this.cboProvince.EditValue = listResult[0].PROVINCE_CODE;
                        this.txtProvinceCode.Text = listResult[0].PROVINCE_CODE;
                        if (isExpand)
                        {
                            this.txtAddress.Focus();
                            this.txtAddress.SelectAll();
                        }
                    }
                    else
                    {
                        this.cboCommune.EditValue = null;
                        this.txtCommuneCode.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadXaCombo(string searchCode, string districtCode, bool isExpand)
        {
            try
            {
                if (IsChangeStrucAddress)
                {
                    LoadXaComboNoDistrict(searchCode, districtCode, isExpand);
                    return;
                }

                List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE> listResult = SdaCommunes.Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE)
                    .Where(o => (o.SEARCH_CODE ?? "").Contains(searchCode ?? "")
                        && (String.IsNullOrEmpty(districtCode) || o.DISTRICT_CODE == districtCode)).ToList();
                this.InitComboCommon(this.cboCommune, listResult, "COMMUNE_CODE", "RENDERER_COMMUNE_NAME", "SEARCH_CODE");
                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(districtCode) && listResult.Count > 0)
                {
                    this.cboCommune.EditValue = null;
                    this.txtCommuneCode.Text = "";
                    this.FocusShowpopup(this.cboCommune, false);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        this.cboCommune.EditValue = listResult[0].COMMUNE_CODE;
                        this.txtCommuneCode.Text = listResult[0].SEARCH_CODE;
                        this.cboDistrict.EditValue = listResult[0].DISTRICT_CODE;
                        this.txtDistrictCode.Text = listResult[0].DISTRICT_CODE;
                        if (String.IsNullOrEmpty(this.cboProvince.Text))
                        {
                            SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).Where(o => o.ID == listResult[0].DISTRICT_ID).FirstOrDefault();
                            if (district != null)
                            {
                                this.cboProvince.EditValue = district.PROVINCE_CODE;
                                this.txtProvinceCode.Text = district.PROVINCE_CODE;
                            }
                        }

                        if (isExpand)
                        {
                            this.txtAddress.Focus();
                            this.txtAddress.SelectAll();
                        }
                    }
                    else if (isExpand)
                    {
                        this.cboCommune.EditValue = null;
                        this.FocusShowpopup(this.cboCommune, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusShowpopup(GridLookUpEdit cboEditor, bool isSelectFirstRow)
        {
            try
            {
                cboEditor.Focus();
                cboEditor.ShowPopup();
                if (isSelectFirstRow)
                    PopupLoader.SelectFirstRowPopup(cboEditor);
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
                if (communeADOs != null)
                    this.InitComboCommon(this.cboTHX, communeADOs, "ID", "RENDERER_PDC_NAME", "SEARCH_CODE_COMMUNE");
                this.cboTHX.EditValue = null;
                this.cboTHX.Properties.Buttons[1].Visible = false;
                //this.FocusShowpopup(this.cboTHX, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetValueHeinAddressByAddressOfPatient()
        {
            try
            {
                SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = null;
                SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = null;
                SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = null;
                if (cboProvince.EditValue != null)
                {
                    province = SdaProvinces.Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).FirstOrDefault(o => o.PROVINCE_CODE == (string)this.cboProvince.EditValue);
                    this.ChangeReplaceAddress(cboProvince.Text, "Tỉnh");
                }
                if (this.cboDistrict.EditValue != null)
                {
                    district = BackendDataWorker.Get<V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).FirstOrDefault(o => o.DISTRICT_CODE == (string)this.cboDistrict.EditValue);
                    this.ChangeReplaceAddress(cboDistrict.Text, "Huyện");
                }

                if (this.cboCommune.EditValue != null)
                {
                    commune = SdaCommunes.Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).FirstOrDefault(o => o.COMMUNE_CODE == (string)this.cboCommune.EditValue);
                    this.ChangeReplaceAddress(cboCommune.Text, "Xã");
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void ChangeReplaceAddress(string cmd, string lever)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.txtAddress.Text) && !string.IsNullOrEmpty(cmd))
                {
                    string address = this.txtAddress.Text;
                    string[] addressSplit = address.Split(',');
                    var datas = addressSplit.Where(p => p.Contains(cmd)).ToList();
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
                            var data = datas.FirstOrDefault(p => p.Contains(addressV2));
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
                        }
                        this.txtAddress.Text = address;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Hàm tính tuổi
        /// </summary>
        /// <param name="strDob"></param>
        /// <param name="isHasReset"></param>
        private void CalulatePatientAge()
        {
            try
            {
                if (this.dtPatientDob.EditValue != null && this.dtPatientDob.DateTime != DateTime.MinValue)
                {
                    bool isGKS = true;
                    DateTime dtNgSinh = this.dtPatientDob.DateTime;
                    TimeSpan diff = DateTime.Now - dtNgSinh;
                    long tongsogiay = diff.Ticks;
                    if (tongsogiay < 0)
                    {
                        this.txtAge.EditValue = "";
                        this.cboAge.EditValue = 4;
                        return;
                    }
                    DateTime newDate = new DateTime(tongsogiay);

                    int nam = newDate.Year - 1;
                    int thang = newDate.Month - 1;
                    int ngay = newDate.Day - 1;
                    int gio = newDate.Hour;
                    int phut = newDate.Minute;
                    int giay = newDate.Second;

                    isGKS = MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(dtNgSinh);

                    if (nam >= 7)
                    {
                        this.cboAge.EditValue = 1;
                        this.txtAge.Enabled = false;
                        this.cboAge.Enabled = false;
                        if (!isGKS)
                        {
                            this.txtAge.EditValue = DateTime.Now.Year - dtNgSinh.Year;
                        }
                        else
                        {
                            this.txtAge.EditValue = nam.ToString();
                        }
                    }
                    else if (nam > 0 && nam < 7)
                    {
                        if (nam == 6)
                        {
                            if (thang > 0 || ngay > 0)
                            {
                                this.cboAge.EditValue = 1;
                                this.txtAge.Enabled = false;
                                this.cboAge.Enabled = false;
                                if (!isGKS)
                                {
                                    this.txtAge.EditValue = DateTime.Now.Year - dtNgSinh.Year;
                                }
                                else
                                {
                                    this.txtAge.EditValue = nam.ToString();
                                }
                            }
                            else
                            {
                                this.txtAge.EditValue = nam * 12 - 1;
                                this.cboAge.EditValue = 2;
                                this.txtAge.Enabled = false;
                                this.cboAge.Enabled = false;
                            }

                        }
                        else
                        {
                            this.txtAge.EditValue = nam * 12 + thang;
                            this.cboAge.EditValue = 2;
                            this.txtAge.Enabled = false;
                            this.cboAge.Enabled = false;
                        }

                    }
                    else
                    {
                        if (thang > 0)
                        {
                            this.txtAge.EditValue = thang.ToString();
                            this.cboAge.EditValue = 2;
                            this.txtAge.Enabled = false;
                            this.cboAge.Enabled = false;
                        }
                        else
                        {
                            if (ngay > 0)
                            {
                                this.txtAge.EditValue = ngay.ToString();
                                this.cboAge.EditValue = 3;
                                this.txtAge.Enabled = false;
                                this.cboAge.Enabled = false;
                            }
                            else
                            {
                                this.txtAge.EditValue = "";
                                this.cboAge.EditValue = 4;
                                this.txtAge.Enabled = false;
                                this.cboAge.Enabled = false;
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

        private void txtTHX_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string maTHX = (sender as DevExpress.XtraEditors.TextEdit).Text.Trim();
                    if (String.IsNullOrEmpty(maTHX))
                    {
                        this.SetSourceValueTHX(workingCommuneADO);
                        return;
                    }
                    this.cboTHX.EditValue = null;
                    //Tìm dữ liệu xã theo startwith với mã đang tìm kiếm
                    List<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO> listResult = workingCommuneADO
                        .Where(o => (o.SEARCH_CODE_COMMUNE != null
                            && o.SEARCH_CODE_COMMUNE.ToUpper().StartsWith(maTHX.ToUpper()))).ToList();
                    //Nếu tìm thấy nhiều hơn 1 kết quả có startwith theo mã vừa nhập
                    if (listResult != null && listResult.Count >= 1)
                    {
                        //Kiểm tra nếu dữ liệu tìm kiếm được là dữ liệu tự động add thêm vào là ghép của 2 mã tìm kiếm tỉnh + huyện với nhau (đánh dấu bằng ID < 0)
                        var dataNoCommunes = listResult.Where(o => o.ID < 0).ToList();
                        //Nếu tìm ra nhiều hơn 1 thằng add thêm -> gán lại datasource của combo THX bằng kết quả tìm kiếm theo startwith ở trên
                        if (dataNoCommunes != null && dataNoCommunes.Count > 1)
                        {
                            this.SetSourceValueTHX(listResult);
                        }
                        //Nếu tìm ra chỉ 1 dòng duy nhất -> gán giá trị cho combo THX, tự động gán các combo huyện, combo xã
                        else if (dataNoCommunes != null && dataNoCommunes.Count == 1)
                        {
                            this.cboTHX.Properties.Buttons[1].Visible = true;
                            this.cboTHX.EditValue = dataNoCommunes[0].ID;
                            this.txtTHX.Text = dataNoCommunes[0].SEARCH_CODE_COMMUNE;

                            var districtDTO = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).SingleOrDefault(o => o.DISTRICT_CODE == dataNoCommunes[0].DISTRICT_CODE);
                            if (districtDTO != null)
                            {
                                this.LoadHuyenCombo("", districtDTO.PROVINCE_CODE, false);
                                this.cboProvince.EditValue = districtDTO.PROVINCE_CODE;
                                this.txtProvinceCode.Text = districtDTO.PROVINCE_CODE;
                            }
                            this.LoadXaCombo("", dataNoCommunes[0].DISTRICT_CODE, false);
                            this.cboDistrict.EditValue = dataNoCommunes[0].DISTRICT_CODE;
                            this.txtDistrictCode.Text = dataNoCommunes[0].DISTRICT_CODE;

                            this.cboCommune.Focus();
                            this.cboCommune.ShowPopup();
                        }
                        //Nếu kết quả tìm kiếm theo startwith tìm ra 1 dòng dữ liệu
                        //--> gán giá trị combo THX, combo Tỉnh, combo huyện, combo xã
                        else if (listResult.Count == 1 && !IsChangeStrucAddress)
                        {
                            this.SetSourceValueTHX(workingCommuneADO);
                            this.cboTHX.Properties.Buttons[1].Visible = true;
                            this.cboTHX.EditValue = listResult[0].ID;
                            this.txtTHX.Text = listResult[0].SEARCH_CODE_COMMUNE;

                            var districtDTO = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).Where(o => o.ID == listResult[0].DISTRICT_ID).SingleOrDefault();
                            if (districtDTO != null)
                            {
                                this.LoadHuyenCombo("", districtDTO.PROVINCE_CODE, false);
                                this.cboProvince.EditValue = districtDTO.PROVINCE_CODE;
                                this.txtProvinceCode.Text = districtDTO.PROVINCE_CODE;
                            }
                            this.LoadXaCombo("", listResult[0].DISTRICT_CODE, false);
                            this.cboDistrict.EditValue = listResult[0].DISTRICT_CODE;
                            this.txtDistrictCode.Text = listResult[0].DISTRICT_CODE;
                            this.cboCommune.EditValue = listResult[0].COMMUNE_CODE;
                            this.txtCommuneCode.Text = listResult[0].COMMUNE_CODE;

                            if (this.cboProvince.EditValue != null
                                && this.cboDistrict.EditValue != null
                                && this.cboCommune.EditValue != null)
                            {
                                this.txtAddress.Focus();
                                this.txtAddress.SelectAll();
                            }
                            else
                            {
                                this.txtCommuneCode.Focus();
                                this.txtCommuneCode.SelectAll();
                            }
                        }
                        else if (listResult.Count == 1 && IsChangeStrucAddress)
                        {
                            this.SetSourceValueTHX(this.workingCommuneADO);
                            this.cboTHX.Properties.Buttons[1].Visible = true;
                            this.cboTHX.EditValue = listResult[0].ID;
                            this.txtTHX.Text = listResult[0].SEARCH_CODE_COMMUNE;
                            var provinceDTO = (cboProvince.Properties.DataSource as List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>).ToList().FirstOrDefault(o => o.PROVINCE_CODE == listResult[0].PROVINCE_CODE);
                            if (provinceDTO != null)
                            {
                                this.txtProvinceCode.Text = provinceDTO.PROVINCE_CODE;
                                this.cboProvince.EditValue = provinceDTO.PROVINCE_CODE;
                            }
                            this.LoadXaCombo("", listResult[0].PROVINCE_CODE, false);

                            this.cboCommune.EditValue = listResult[0].COMMUNE_CODE;
                            this.txtCommuneCode.Text = listResult[0].COMMUNE_CODE;
                        }
                        //Ngược lại gán lại datasource của combo THX bằng kết quả vừa tìm đc
                        else
                        {
                            this.SetSourceValueTHX(listResult);
                        }
                    }
                    //Nếu không tìm thấy kết quả nào -> reset giá trị combo THX
                    else
                    {
                        this.SetSourceValueTHX(null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTHX_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboTHX.EditValue != null)
                    {
                        this.cboTHX.Properties.Buttons[1].Visible = true;
                        HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO commune = workingCommuneADO.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboTHX.EditValue ?? 0).ToString()));
                        if (commune != null)
                        {
                            var districtDTO = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).SingleOrDefault(o => o.DISTRICT_CODE == commune.DISTRICT_CODE);
                            if (districtDTO != null)
                            {
                                this.LoadHuyenCombo("", districtDTO.PROVINCE_CODE, false);
                                this.cboProvince.EditValue = districtDTO.PROVINCE_CODE;
                                this.txtProvinceCode.Text = districtDTO.PROVINCE_CODE;
                            }
                            if (IsChangeStrucAddress)
                            {
                                this.LoadXaCombo("", commune.PROVINCE_CODE, false);
                                cboCommune.EditValue = commune.COMMUNE_CODE;
                                txtCommuneCode.Text = commune.COMMUNE_CODE;
                                this.cboProvince.EditValue = commune.PROVINCE_CODE;
                                this.txtProvinceCode.Text = commune.PROVINCE_CODE;
                            }
                            else
                            {
                                this.LoadXaCombo("", commune.DISTRICT_CODE, false);
                            }
                            this.txtTHX.Text = commune.SEARCH_CODE_COMMUNE;
                            this.cboDistrict.EditValue = commune.DISTRICT_CODE;
                            this.txtDistrictCode.Text = commune.DISTRICT_CODE;

                            if (commune.ID < 0)
                            {
                                this.txtAddress.Focus();
                                this.txtAddress.SelectAll();
                            }
                            else
                            {
                                this.cboCommune.EditValue = commune.COMMUNE_CODE;
                                this.txtCommuneCode.Text = commune.COMMUNE_CODE;
                                if (this.cboProvince.EditValue != null
                                    && this.cboDistrict.EditValue != null
                                    && this.cboCommune.EditValue != null)
                                {
                                    this.txtAddress.Focus();
                                    this.txtAddress.SelectAll();
                                }
                                else
                                {
                                    this.txtCommuneCode.Focus();
                                    this.txtCommuneCode.SelectAll();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (this.cboProvince.EditValue != null
                            && this.cboDistrict.EditValue != null
                            && this.cboCommune.EditValue != null)
                        {
                            this.txtAddress.Focus();
                            this.txtAddress.SelectAll();
                        }
                        else
                        {
                            this.txtCommuneCode.Focus();
                            this.txtCommuneCode.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                    this.txtTHX.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = SdaProvinces.Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).FirstOrDefault(o => o.PROVINCE_CODE == cboProvince.EditValue.ToString());
                        if (province != null)
                        {
                            if (IsChangeStrucAddress)
                            {
                                this.LoadXaCombo("", province.PROVINCE_CODE, false);
                            }
                            else
                                this.LoadHuyenCombo("", province.PROVINCE_CODE, false);
                            this.txtProvinceCode.Text = province.SEARCH_CODE;
                        }
                    }
                    this.txtDistrictCode.Text = "";
                    this.txtDistrictCode.Focus();
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
                this.SetValueHeinAddressByAddressOfPatient();
                this.SetRelativeAddress(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            //    try
            //    {
            //        this.SetValueHeinAddressByAddressOfPatient();
            //        if (cboProvince.EditValue != null)
            //        {
            //            SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().SingleOrDefault(o => o.PROVINCE_CODE == cboProvince.EditValue.ToString());
            //            if (province != null)
            //            {
            //                this.txtProvinceCode.Text = province.SEARCH_CODE;
            //            }

            //            this.cboProvince.Properties.Buttons[1].Visible = true;
            //        }
            //        else
            //        {
            //            this.txtProvinceCode.Text = "";
            //            this.cboProvince.Properties.Buttons[1].Visible = false;
            //        }

            //    }
            //    catch (Exception ex)
            //    {
            //        Inventec.Common.Logging.LogSystem.Error(ex);
            //    }
            //}
        }

        private void cboProvince_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboProvince.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = SdaProvinces.Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).SingleOrDefault(o => o.PROVINCE_CODE == this.cboProvince.EditValue.ToString());
                        if (province != null)
                        {
                            this.txtProvinceCode.Text = province.SEARCH_CODE;
                            if (IsChangeStrucAddress)
                            {
                                LoadXaComboNoDistrict("", province.PROVINCE_CODE, false);
                            }
                            else
                            {
                                this.LoadHuyenCombo("", province.PROVINCE_CODE, false);
                                this.txtDistrictCode.Text = "";
                                this.txtDistrictCode.Focus();
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
        private void SetRelativeAddress(bool value)
        {
            try
            {
                //value   false la tick KS
                #region ------
                string adress = "";
                    if (!string.IsNullOrEmpty(this.txtAddress.Text))
                    {
                        adress = this.txtAddress.Text + ", ";
                    }
                    if (this.cboCommune.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = SdaCommunes.Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE)
                        .SingleOrDefault(o =>
                            o.COMMUNE_CODE == this.cboCommune.EditValue.ToString()
                                && (String.IsNullOrEmpty((this.cboDistrict.EditValue ?? "").ToString()) || o.DISTRICT_CODE == (this.cboDistrict.EditValue ?? "").ToString())
                            );
                        if (commune != null)
                        {
                            adress += "Xã " + commune.COMMUNE_NAME + ", ";
                        }
                    }

                    if (this.cboDistrict.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE)
                       .SingleOrDefault(o => o.DISTRICT_CODE == this.cboDistrict.EditValue.ToString()
                           && (String.IsNullOrEmpty((this.cboProvince.EditValue ?? "").ToString()) || o.PROVINCE_CODE == (this.cboProvince.EditValue ?? "").ToString()));
                        if (district != null)
                        {
                            adress += "Huyện " + district.DISTRICT_NAME + ", ";
                        }
                    }
                    if (this.cboProvince.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = SdaProvinces.Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).SingleOrDefault(o => o.PROVINCE_CODE == cboProvince.EditValue.ToString());
                        if (province != null)
                        {
                            adress += province.PROVINCE_NAME;
                        }
                    }

                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void cboProvince_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.cboProvince.EditValue = null;
                    this.cboProvince.Properties.Buttons[1].Visible = false;
                    this.txtProvinceCode.Text = "";
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
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE)
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
                        }
                    }
                    this.txtCommuneCode.Focus();
                    this.txtCommuneCode.SelectAll();
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
                this.SetValueHeinAddressByAddressOfPatient();
                this.SetRelativeAddress(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            //try
            //{
            //    this.SetValueHeinAddressByAddressOfPatient();
            //    if (cboDistrict.EditValue != null)
            //    {
            //        SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>()
            //                .SingleOrDefault(o => o.DISTRICT_CODE == this.cboDistrict.EditValue.ToString()
            //                    && (String.IsNullOrEmpty((this.cboProvince.EditValue ?? "").ToString()) || o.PROVINCE_CODE == (this.cboProvince.EditValue ?? "").ToString()));
            //        if (district != null)
            //        {
            //            if (String.IsNullOrEmpty((this.cboProvince.EditValue ?? "").ToString()))
            //            {
            //                this.cboProvince.EditValue = district.PROVINCE_CODE;
            //            }
            //            this.txtDistrictCode.Text = district.SEARCH_CODE;
            //        }
            //        this.cboDistrict.Properties.Buttons[1].Visible = true;
            //    }
            //    else
            //    {
            //        this.txtDistrictCode.Text = "";
            //        this.cboDistrict.Properties.Buttons[1].Visible = false;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void cboDistrict_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboDistrict.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE)
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
                            this.txtCommuneCode.Focus();
                            this.txtCommuneCode.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistrict_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.cboDistrict.EditValue = null;
                    this.cboDistrict.Properties.Buttons[1].Visible = false;
                    this.txtDistrictCode.Text = "";
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
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = SdaCommunes.Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE)
                            .SingleOrDefault(o =>
                                o.COMMUNE_CODE == this.cboCommune.EditValue.ToString()
                                    //&& o.PROVINCE_CODE == this.cboProvince.EditValue.ToString() 
                                    && (String.IsNullOrEmpty((this.cboDistrict.EditValue ?? "").ToString())
                                    || o.DISTRICT_CODE == (this.cboDistrict.EditValue ?? "").ToString())
                                );
                        if (commune != null)
                        {
                            this.txtCommuneCode.Text = commune.SEARCH_CODE;
                            if (String.IsNullOrEmpty((this.cboProvince.EditValue ?? "").ToString()) && String.IsNullOrEmpty((this.cboDistrict.EditValue ?? "").ToString()))
                            {
                                this.cboDistrict.EditValue = commune.DISTRICT_CODE;
                                this.txtDistrictCode.Text = commune.DISTRICT_CODE;
                                SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).Where(o => o.ID == commune.DISTRICT_ID).FirstOrDefault();
                                if (district != null)
                                {
                                    this.cboProvince.EditValue = district.PROVINCE_CODE;
                                    this.txtProvinceCode.Text = district.PROVINCE_CODE;
                                }
                            }
                        }
                    }
                    this.txtAddress.Focus();
                    this.txtAddress.SelectAll();
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
                this.SetValueHeinAddressByAddressOfPatient();
                this.SetRelativeAddress(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            //try
            //{
            //    this.SetValueHeinAddressByAddressOfPatient();
            //    if (cboCommune.EditValue != null)
            //    {
            //        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>()
            //                .SingleOrDefault(o =>
            //                    o.COMMUNE_CODE == this.cboCommune.EditValue.ToString()
            //                        //&& o.PROVINCE_CODE == this.cboProvince.EditValue.ToString() 
            //                        && (String.IsNullOrEmpty((this.cboDistrict.EditValue ?? "").ToString())
            //                        || o.DISTRICT_CODE == (this.cboDistrict.EditValue ?? "").ToString())
            //                    );
            //        if (commune != null)
            //        {
            //            this.txtCommuneCode.Text = commune.SEARCH_CODE;
            //            if (String.IsNullOrEmpty((this.cboProvince.EditValue ?? "").ToString()) && String.IsNullOrEmpty((this.cboDistrict.EditValue ?? "").ToString()))
            //            {
            //                this.cboDistrict.EditValue = commune.DISTRICT_CODE;
            //                this.txtDistrictCode.Text = commune.DISTRICT_CODE;
            //                SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.ID == commune.DISTRICT_ID).FirstOrDefault();
            //                if (district != null)
            //                {
            //                    this.cboProvince.EditValue = district.PROVINCE_CODE;
            //                    this.txtProvinceCode.Text = district.PROVINCE_CODE;
            //                }
            //            }
            //        }
            //        this.cboCommune.Properties.Buttons[1].Visible = true;
            //    }
            //    else
            //    {
            //        this.txtCommuneCode.Text = "";
            //        this.cboCommune.Properties.Buttons[1].Visible = false;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }
        private void cboCommune_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboCommune.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = SdaCommunes.Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE)
                            .SingleOrDefault(o =>
                                o.COMMUNE_CODE == this.cboCommune.EditValue.ToString()
                                //&& o.PROVINCE_CODE == cboProvince.EditValue.ToString() 
                                && (String.IsNullOrEmpty((this.cboDistrict.EditValue ?? "").ToString()) || o.DISTRICT_CODE == (this.cboDistrict.EditValue ?? "").ToString()));
                        if (commune != null)
                        {
                            this.txtCommuneCode.Text = commune.SEARCH_CODE;
                            if (String.IsNullOrEmpty((this.cboProvince.EditValue ?? "").ToString()) && String.IsNullOrEmpty((this.cboDistrict.EditValue ?? "").ToString()))
                            {
                                this.cboDistrict.EditValue = commune.DISTRICT_CODE;
                                SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>()
                                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).Where(o => o.ID == commune.DISTRICT_ID).FirstOrDefault();
                                if (district != null)
                                {
                                    this.cboProvince.EditValue = district.PROVINCE_CODE;
                                    this.txtProvinceCode.Text = district.PROVINCE_CODE;
                                }
                                //Mô tả lỗi đi
                            }
                            this.txtAddress.Focus();
                            this.txtAddress.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCommune_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.cboCommune.EditValue = null;
                    this.cboCommune.Properties.Buttons[1].Visible = false;
                    this.txtCommuneCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAddress_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPhone.Focus();
                    txtPhone.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboGender_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    dtPatientDob.Focus();
                    dtPatientDob.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtDOB_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.dtPatientDob.EditValue = null;
                    this.dtPatientDob.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtDOB_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtPatientDob.EditValue != null)
                {
                    CalulatePatientAge();
                    this.dtPatientDob.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    this.dtPatientDob.Properties.Buttons[1].Visible = false;
                }

                if (!String.IsNullOrEmpty(txtPatientName.Text) && cboGender.EditValue != null && dtPatientDob.EditValue != null && check)
                {
                    loadDataPatientInformation();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtPatientDob_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTHX.Focus();
                    txtTHX.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FocustxtPatientName()
        {
            try
            {
                txtPatientName.Focus();
                txtPatientName.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public string GetValuePatientName()
        {
            try
            {
                return txtPatientName.Text;
            }
            catch (Exception ex)
            {
                return "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public long? GetValueGender()
        {
            try
            {
                return (long)cboGender.EditValue;
            }
            catch (Exception ex)
            {
                return null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public long? GetValuePatientDob()
        {
            try
            {
                return Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtPatientDob.DateTime);
            }
            catch (Exception ex)
            {
                return null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public string GetValueAddress()
        {
            try
            {
                return txtAddress.Text;
            }
            catch (Exception ex)
            {
                return "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public string GetValuePhone()
        {
            try
            {
                return txtPhone.Text;
            }
            catch (Exception ex)
            {
                return "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public V_SDA_PROVINCE GetValueProvince()
        {
            try
            {
                V_SDA_PROVINCE province = new V_SDA_PROVINCE();
                if (!String.IsNullOrEmpty(this.cboProvince.EditValue.ToString()))
                {
                    province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().SingleOrDefault(o => o.PROVINCE_CODE == this.cboProvince.EditValue.ToString());
                }
                return province;
            }
            catch (Exception ex)
            {
                return new V_SDA_PROVINCE();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public V_SDA_DISTRICT GetValueDistrict()
        {
            try
            {
                V_SDA_DISTRICT district = new V_SDA_DISTRICT();
                if (!String.IsNullOrEmpty(this.cboDistrict.EditValue.ToString()))
                {
                    district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>()
                            .SingleOrDefault(o => o.DISTRICT_CODE == this.cboDistrict.EditValue.ToString()
                               && (String.IsNullOrEmpty((this.cboProvince.EditValue ?? "").ToString()) || o.PROVINCE_CODE == (this.cboProvince.EditValue ?? "").ToString()));

                }
                return district;
            }
            catch (Exception ex)
            {
                return new V_SDA_DISTRICT();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public V_SDA_COMMUNE GetValueCommune()
        {
            try
            {
                V_SDA_COMMUNE commune = new V_SDA_COMMUNE();
                if (!String.IsNullOrEmpty(this.cboCommune.EditValue.ToString()))
                {
                    commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>()
                                .SingleOrDefault(o =>
                                    o.COMMUNE_CODE == this.cboCommune.EditValue.ToString()
                                    //&& o.PROVINCE_CODE == cboProvince.EditValue.ToString() 
                                    && (String.IsNullOrEmpty((this.cboDistrict.EditValue ?? "").ToString()) || o.DISTRICT_CODE == (this.cboDistrict.EditValue ?? "").ToString()));
                }
                return commune;
            }
            catch (Exception ex)
            {
                return new V_SDA_COMMUNE();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        public V_HIS_CONTACT_POINT GetCurrentContactPoint()
        {
            try
            {
                if (this.CurrentContactPoint != null && this.CurrentContactPoint.ID > 0)
                {
                    return this.CurrentContactPoint;
                }
                else
                {
                    return null;
                }
                //return CurrentContactPoint;

            }
            catch (Exception ex)
            {
                return new V_HIS_CONTACT_POINT();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void loadDataPatientInformation()
        {
            try
            {
                WaitingManager.Show();
                CommonParam paramCommon = new CommonParam();
                HisContactPointFilter filter = new HisContactPointFilter();

                filter.FULL_NAME_EXACT = txtPatientName.Text.ToUpper();
                filter.GENDER_ID = (long)cboGender.EditValue;
                filter.DOB = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtPatientDob.DateTime);

                var ContactPoint = new BackendAdapter(paramCommon).Get<List<HIS_CONTACT_POINT>>("/api/HisContactPoint/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (ContactPoint != null && ContactPoint.Count > 0)
                {
                    //CurrentContactPoint = ContactPoint.FirstOrDefault();
                    SetCurrentContactPoint(ref this.CurrentContactPoint, ContactPoint.FirstOrDefault());

                    frmOtherChoice frmOtherChoice = new frmOtherChoice(ContactPoint, this.SetValueControl);

                    frmOtherChoice.ShowDialog();

                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCurrentContactPoint(ref V_HIS_CONTACT_POINT VContactPoint, HIS_CONTACT_POINT ConTactPoint)
        {
            try
            {
                if (ConTactPoint != null)
                {
                    VContactPoint = new V_HIS_CONTACT_POINT();
                    VContactPoint.CONTACT_LEVEL = ConTactPoint.CONTACT_LEVEL;
                    VContactPoint.CONTACT_POINT_OTHER_TYPE_NAME = ConTactPoint.CONTACT_POINT_OTHER_TYPE_NAME;
                    VContactPoint.CONTACT_TYPE = ConTactPoint.CONTACT_TYPE;
                    VContactPoint.CREATE_TIME = ConTactPoint.CREATE_TIME;
                    VContactPoint.CREATOR = ConTactPoint.CREATOR;
                    VContactPoint.DOB = ConTactPoint.DOB;
                    VContactPoint.EMPLOYEE_ID = ConTactPoint.EMPLOYEE_ID;
                    VContactPoint.FIRST_NAME = ConTactPoint.FIRST_NAME;
                    VContactPoint.FULL_NAME = ConTactPoint.VIR_FULL_NAME;
                    VContactPoint.GENDER_ID = ConTactPoint.GENDER_ID;
                    VContactPoint.ID = ConTactPoint.ID;
                    VContactPoint.LAST_NAME = ConTactPoint.LAST_NAME;
                    VContactPoint.MODIFIER = ConTactPoint.MODIFIER;
                    VContactPoint.MODIFY_TIME = ConTactPoint.MODIFY_TIME;
                    VContactPoint.NOTE = ConTactPoint.NOTE;
                    VContactPoint.PATIENT_ID = ConTactPoint.PATIENT_ID;
                    VContactPoint.PHONE = ConTactPoint.PHONE;
                    VContactPoint.TEST_RESULT_1 = ConTactPoint.TEST_RESULT_1;
                    VContactPoint.TEST_RESULT_2 = ConTactPoint.TEST_RESULT_2;
                    VContactPoint.TEST_RESULT_3 = ConTactPoint.TEST_RESULT_3;
                    VContactPoint.VIR_ADDRESS = ConTactPoint.VIR_ADDRESS;
                }
                else
                {
                    VContactPoint = new V_HIS_CONTACT_POINT();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void SetValueControl(HIS_CONTACT_POINT data)
        {
            try
            {
                if (data != null)
                {

                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_CONTACT_POINT>(CurrentContactPoint, data);
                    //CurrentContactPoint = data;
                    this.ChangeDataSourceAddress();
                    this.cboProvince.EditValue = data.PROVINCE_CODE;
                    this.cboDistrict.EditValue = data.DISTRICT_CODE;
                    this.cboCommune.EditValue = data.COMMUNE_CODE;
                    this.txtAddress.Text = data.ADDRESS;
                    this.txtPhone.Text = data.PHONE;

                    this.updateVContactPoint(CurrentContactPoint);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientName_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtPatientName.Text) && cboGender.EditValue != null && dtPatientDob.EditValue != null && check)
                {
                    loadDataPatientInformation();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboGender_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtPatientName.Text) && cboGender.EditValue != null && dtPatientDob.EditValue != null && check)
                {
                    loadDataPatientInformation();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTHX_Properties_GetNotInListValue(object sender, GetNotInListValueEventArgs e)
        {
            try
            {
                if (e.FieldName == "RENDERER_PDC_NAME")
                {
                    var item = ((List<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>)this.cboTHX.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0} - {1} {2}{3}", item.PROVINCE_NAME, item.DISTRICT_INITIAL_NAME, item.DISTRICT_NAME, (String.IsNullOrEmpty(item.COMMUNE_NAME) ? "" : " - " + item.INITIAL_NAME + " " + item.COMMUNE_NAME));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDistrict_Properties_GetNotInListValue(object sender, GetNotInListValueEventArgs e)
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

        private void cboCommune_Properties_GetNotInListValue(object sender, GetNotInListValueEventArgs e)
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

        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
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

        public void reset()
        {
            try
            {
                this.txtPatientName.Text = "";
                this.cboGender.EditValue = null;
                this.dtPatientDob.EditValue = null;
                this.txtAddress.Text = "";
                this.txtPhone.Text = "";
                this.txtProvinceCode.Text = "";
                this.txtDistrictCode.Text = "";
                this.txtCommuneCode.Text = "";
                this.cboProvince.EditValue = null;
                this.cboDistrict.EditValue = null;
                this.cboCommune.EditValue = null;
                this.txtAge.Text = "";
                this.cboAge.EditValue = null;
                this.txtTHX.Text = "";
                this.cboTHX.EditValue = null;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetToolTipTog(object sender)
        {

            try
            {
                var toggleSwitch = sender as DevExpress.XtraEditors.ToggleSwitch;
                if (toggleSwitch != null)
                {
                    string tooltipText = toggleSwitch.IsOn
                        ? "Sử dụng cấu trúc địa chỉ mới Xã - Tỉnh (không có Huyện)"
                        : "Sử dụng cấu trúc địa chỉ mới Xã - Huyện - Tỉnh";
                    toggleSwitch.ToolTip = tooltipText;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private bool IsChangeStrucAddress { get; set; }
        public bool IsNotCheckToggleAddress { get; private set; }

        private bool IsChangeValueMember = false;
        private void toggleSwitchDataChecked_Toggled(object sender, EventArgs e)
        {
            try
            {
                SetToolTipTog(sender);
                IsChangeStrucAddress = toggleSwitchDataChecked.IsOn;
                if (IsNotCheckToggleAddress)
                {
                    IsNotCheckToggleAddress = false;
                    return;
                }
                ChangeComponentDistrict();
                ChangeDataSourceAddress();

                IsNotCheckToggleAddress = false;
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == toggleSwitchDataChecked.Name && o.MODULE_LINK == this.moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (toggleSwitchDataChecked.IsOn ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = toggleSwitchDataChecked.Name;
                    csAddOrUpdate.VALUE = (toggleSwitchDataChecked.IsOn ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = this.moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetDataAddress(AddressADO data)
        {
            try
            {
                IsNotCheckToggleAddress = true;

                if (data.IsNoDistrict)
                {
                    toggleSwitchDataChecked.IsOn = true;
                }
                else
                {
                    toggleSwitchDataChecked.IsOn = false;
                }
                ChangeComponentDistrict();
                ChangeDataSourceAddress();
                cboProvince.EditValue = data.ProvinceCode;
                txtProvinceCode.EditValue = data.ProvinceCode;
                cboDistrict.EditValue = data.DistrictCode;
                txtDistrictCode.EditValue = data.DistrictCode;
                cboCommune.EditValue = data.CommuneCode;
                txtCommuneCode.EditValue = data.CommuneCode;
                if (!string.IsNullOrEmpty(data.CommuneCode))
                {
                    var commune = ((List<V_SDA_COMMUNE>)cboCommune.Properties.DataSource).FirstOrDefault(o => o.COMMUNE_CODE == data.CommuneCode);
                    if (commune != null)
                    {
                        this.cboTHX.EditValue = commune.ID;//ID_RAW
                        bool isSearchOrderByXHT = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS_DESKTOP_REGISTER__SEARCH_CODE__X/H/T") == "1" ? true : false;
                        this.cboTHX.Properties.Buttons[1].Visible = true;
                        var thx = workingCommuneADO.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboTHX.EditValue ?? 0).ToString()));
                        this.txtTHX.Text = thx != null ? thx.SEARCH_CODE_COMMUNE : null;

                        cboTHX_Closed(cboTHX, new DevExpress.XtraEditors.Controls.ClosedEventArgs(DevExpress.XtraEditors.PopupCloseMode.Normal));

                    }
                }

                IsNotCheckToggleAddress = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void ChangeComponentDistrict()
        {
            try
            {
                if (IsChangeStrucAddress)
                {
                    layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem12.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                }
                else
                {
                    layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem12.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                cboProvince.EditValue = null;
                txtProvinceCode.Text = null;
                cboDistrict.EditValue = null;
                txtDistrictCode.Text = null;
                cboCommune.EditValue = null;
                txtCommuneCode.EditValue = null;
                txtTHX.EditValue = null;
                cboTHX.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        List<V_SDA_COMMUNE> SdaCommunes { get; set; }
        List<V_SDA_PROVINCE> SdaProvinces { get; set; }
        List<CommuneADO> workingCommuneADO { get; set; }

        private void ChangeDataSourceAddress()
        {
            try
            {

                if (IsChangeStrucAddress)
                {
                    SdaCommunes = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE && o.IS_NO_DISTRICT == 1 && o.PROVINCE_ID > 0).ToList();
                    SdaProvinces = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE && o.IS_NO_DISTRICT == 1).ToList();
                    workingCommuneADO = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>().Where(o => o.IS_NO_DISTRICT == 1).ToList();
                }
                else
                {

                    SdaCommunes = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE && o.IS_NO_DISTRICT != 1).ToList();
                    SdaProvinces = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE && o.IS_NO_DISTRICT != 1).ToList();
                    workingCommuneADO = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>().Where(o => o.IS_NO_DISTRICT != 1).ToList();
                }
                this.InitComboCommon(this.cboProvince, SdaProvinces, "PROVINCE_CODE", "PROVINCE_NAME", "SEARCH_CODE");
                this.InitComboCommon(this.cboDistrict, BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList(), "DISTRICT_CODE", "RENDERER_DISTRICT_NAME", "SEARCH_CODE");
                this.InitComboCommon(this.cboCommune, SdaCommunes, "COMMUNE_CODE", "RENDERER_COMMUNE_NAME", "SEARCH_CODE");
                this.SetSourceValueTHX(workingCommuneADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, string displayMemberCode)
        {
            try
            {
                InitComboCommon(cboEditor, data, valueMember, displayMember, 0, displayMemberCode, 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, int displayMemberWidth, string displayMemberCode, int displayMemberCodeWidth)
        {
            try
            {
                int popupWidth = 0;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                if (!String.IsNullOrEmpty(displayMemberCode))
                {
                    columnInfos.Add(new ColumnInfo(displayMemberCode, "", (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100), 1));
                    popupWidth += (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100);
                }
                if (!String.IsNullOrEmpty(displayMember))
                {
                    columnInfos.Add(new ColumnInfo(displayMember, "", (displayMemberWidth > 0 ? displayMemberWidth : 250), 2));
                    popupWidth += (displayMemberWidth > 0 ? displayMemberWidth : 250);
                }
                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, popupWidth);
                ControlEditorLoader.Load(cboEditor, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        bool isNotLoadWhileChangeControlStateInFirst;
        void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == toggleSwitchDataChecked.Name)
                        {
                            toggleSwitchDataChecked.IsOn = item.VALUE == "1";
                        }
                    }
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
                    this.cboProvince.Properties.DataSource = SdaProvinces.Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProvince_Properties_GetNotInListValue(object sender, GetNotInListValueEventArgs e)
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

        private void cboTHX_EditValueChanged(object sender, EventArgs e)
        {
            {
                try
                {
                    if (this.cboTHX.EditValue != null)
                    {
                        this.cboTHX.Properties.Buttons[1].Visible = true;
                        //HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO commune = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>().SingleOrDefault(o => o.ID_RAW == (this.cboVirAddress.EditValue ?? "").ToString());
                        var communeList = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>();
                        //Inventec.Common.Logging.LogSystem.Debug("COMMUNE LISTTTTTT: " + Inventec.Common.Logging.LogUtil.TraceData("DataA", communeList));
                        if (communeList == null) return;
                        var commune = communeList.SingleOrDefault(o => o.ID_RAW == (this.cboTHX.EditValue ?? "").ToString());
                        if (commune != null)
                        {
                            this.txtTHX.Text = commune.SEARCH_CODE_COMMUNE;

                            var district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_DISTRICT>().SingleOrDefault(o => o.DISTRICT_CODE == commune.DISTRICT_CODE);

                            if (toggleSwitchDataChecked.IsOn == false)
                            {
                                if (district != null)
                                {
                                    var province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_PROVINCE>().SingleOrDefault(o => o.ID == district.PROVINCE_ID);
                                    cboProvince.EditValue = province != null ? (long?)province.ID : null;
                                    cboDistrict.EditValue = district.ID;
                                    cboCommune.EditValue = commune.ID;
                                }
                            }
                            else
                            {
                                var province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_PROVINCE>().SingleOrDefault(o => o.ID == commune.PROVINCE_ID);
                                cboProvince.EditValue = province != null ? (long?)province.ID : null;
                                //.EditValue = district.ID;
                                cboCommune.EditValue = commune.ID;
                            }
                            //Cái này a k code ntn nh
                            //Không hiểu biến Is_No_District là gì à???? Trong việc thấy ghi mà trong này k thấy sử dụng nó là không huyện còn gì a
                        }
                    }
                    else
                    {
                        this.cboTHX.Properties.Buttons[1].Visible = false;
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
        }
    }
}

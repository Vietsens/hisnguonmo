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

namespace HIS.Desktop.Plugins.ContactDeclaration.UcObject
{
    public partial class UcOrther : UserControl
    {
        public UpdateVContactPoint updateVContactPoint;
        V_HIS_CONTACT_POINT CurrentContactPoint = new V_HIS_CONTACT_POINT();

        HIS_CONTACT_POINT ContactPoint = new HIS_CONTACT_POINT();

        List<HIS_GENDER> lstGender = new List<HIS_GENDER>();
        List<HIS.Desktop.LocalStorage.BackendData.ADO.AgeADO> lstAgeADO = new List<LocalStorage.BackendData.ADO.AgeADO>();
        List<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO> lstCommuneADO = new List<LocalStorage.BackendData.ADO.CommuneADO>();
        List<V_SDA_PROVINCE> lstSdaProvince = new List<V_SDA_PROVINCE>();
        List<V_SDA_DISTRICT> lstSdaDistrict = new List<V_SDA_DISTRICT>();
        List<V_SDA_COMMUNE> lstSdaCommune = new List<V_SDA_COMMUNE>();
        int positionHandle = -1;
        bool check = true;

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
                    listResult = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.SEARCH_CODE.Contains(searchCode)).ToList();
                    if (listResult.Count == 1)
                    {
                        this.cboProvince.EditValue = listResult[0].PROVINCE_CODE;
                        this.txtProvinceCode.Text = listResult[0].SEARCH_CODE;
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
                listResult = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.SEARCH_CODE.ToUpper().Contains(searchCode.ToUpper()) && (provinceCode == "" || o.PROVINCE_CODE == provinceCode)).ToList();
                LoadComboboxDistrict(listResult);
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

        private void LoadXaCombo(string searchCode, string districtCode, bool isExpand)
        {
            try
            {
                List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE> listResult = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>()
                    .Where(o => (o.SEARCH_CODE ?? "").Contains(searchCode ?? "")
                        && (String.IsNullOrEmpty(districtCode) || o.DISTRICT_CODE == districtCode)).ToList();
                LoadComboboxCommune(listResult);

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
                            SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.ID == listResult[0].DISTRICT_ID).FirstOrDefault();
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
                    LoadComboboxTHX(communeADOs);
                this.cboTHX.EditValue = null;
                this.cboTHX.Properties.Buttons[1].Visible = false;
                this.FocusShowpopup(this.cboTHX, false);
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
                    province = BackendDataWorker.Get<V_SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_CODE == (string)this.cboProvince.EditValue);
                    this.ChangeReplaceAddress(cboProvince.Text, "Tỉnh");
                }
                if (this.cboDistrict.EditValue != null)
                {
                    district = BackendDataWorker.Get<V_SDA_DISTRICT>().FirstOrDefault(o => o.DISTRICT_CODE == (string)this.cboDistrict.EditValue);
                    this.ChangeReplaceAddress(cboDistrict.Text, "Huyện");
                }

                if (this.cboCommune.EditValue != null)
                {
                    commune = BackendDataWorker.Get<V_SDA_COMMUNE>().FirstOrDefault(o => o.COMMUNE_CODE == (string)this.cboCommune.EditValue);
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
                        this.SetSourceValueTHX(BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>());
                        return;
                    }
                    this.cboTHX.EditValue = null;
                    //Tìm dữ liệu xã theo startwith với mã đang tìm kiếm
                    List<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO> listResult = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>()
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

                            var districtDTO = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().SingleOrDefault(o => o.DISTRICT_CODE == dataNoCommunes[0].DISTRICT_CODE);
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
                        else if (listResult.Count == 1)
                        {
                            this.SetSourceValueTHX(BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>());
                            this.cboTHX.Properties.Buttons[1].Visible = true;
                            this.cboTHX.EditValue = listResult[0].ID;
                            this.txtTHX.Text = listResult[0].SEARCH_CODE_COMMUNE;

                            var districtDTO = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.ID == listResult[0].DISTRICT_ID).SingleOrDefault();
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
                        HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO commune = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboTHX.EditValue ?? 0).ToString()));
                        if (commune != null)
                        {
                            var districtDTO = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().SingleOrDefault(o => o.DISTRICT_CODE == commune.DISTRICT_CODE);
                            if (districtDTO != null)
                            {
                                this.LoadHuyenCombo("", districtDTO.PROVINCE_CODE, false);
                                this.cboProvince.EditValue = districtDTO.PROVINCE_CODE;
                                this.txtProvinceCode.Text = districtDTO.PROVINCE_CODE;
                            }
                            this.LoadXaCombo("", commune.DISTRICT_CODE, false);
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
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().SingleOrDefault(o => o.PROVINCE_CODE == cboProvince.EditValue.ToString());
                        if (province != null)
                        {
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
                if (cboProvince.EditValue != null)
                {
                    SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().SingleOrDefault(o => o.PROVINCE_CODE == cboProvince.EditValue.ToString());
                    if (province != null)
                    {
                        this.txtProvinceCode.Text = province.SEARCH_CODE;
                    }

                    this.cboProvince.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    this.txtProvinceCode.Text = "";
                    this.cboProvince.Properties.Buttons[1].Visible = false;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().SingleOrDefault(o => o.PROVINCE_CODE == this.cboProvince.EditValue.ToString());
                        if (province != null)
                        {
                            this.LoadHuyenCombo("", province.PROVINCE_CODE, false);
                            this.txtProvinceCode.Text = province.SEARCH_CODE;
                            this.txtDistrictCode.Text = "";
                            this.txtDistrictCode.Focus();
                        }
                    }
                }
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
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>()
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
                if (cboDistrict.EditValue != null)
                {
                    SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>()
                            .SingleOrDefault(o => o.DISTRICT_CODE == this.cboDistrict.EditValue.ToString()
                                && (String.IsNullOrEmpty((this.cboProvince.EditValue ?? "").ToString()) || o.PROVINCE_CODE == (this.cboProvince.EditValue ?? "").ToString()));
                    if (district != null)
                    {
                        if (String.IsNullOrEmpty((this.cboProvince.EditValue ?? "").ToString()))
                        {
                            this.cboProvince.EditValue = district.PROVINCE_CODE;
                        }
                        this.txtDistrictCode.Text = district.SEARCH_CODE;
                    }
                    this.cboDistrict.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    this.txtDistrictCode.Text = "";
                    this.cboDistrict.Properties.Buttons[1].Visible = false;
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
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>()
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
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>()
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
                                SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.ID == commune.DISTRICT_ID).FirstOrDefault();
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
                if (cboCommune.EditValue != null)
                {
                    SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>()
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
                            SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.ID == commune.DISTRICT_ID).FirstOrDefault();
                            if (district != null)
                            {
                                this.cboProvince.EditValue = district.PROVINCE_CODE;
                                this.txtProvinceCode.Text = district.PROVINCE_CODE;
                            }
                        }
                    }
                    this.cboCommune.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    this.txtCommuneCode.Text = "";
                    this.cboCommune.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>()
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
                                SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.ID == commune.DISTRICT_ID).FirstOrDefault();
                                if (district != null)
                                {
                                    this.cboProvince.EditValue = district.PROVINCE_CODE;
                                    this.txtProvinceCode.Text = district.PROVINCE_CODE;
                                }
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistrict_Properties_GetNotInListValue(object sender, GetNotInListValueEventArgs e)
        {
            try
            {
                if (e.FieldName == "RENDERER_DISTRICT_NAME")
                {
                    var item = ((List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>)cboDistrict.Properties.DataSource)[e.RecordIndex];
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
    }
}

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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.PatientInfo;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using HIS.UC.WorkPlace;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Desktop.Common.LocalStorage.Location;
using System.Configuration;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Plugins.PatientInfo.Resources;
using HIS.Desktop.Common;
using SDA.EFMODEL.DataModels;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Plugins.PatientInfo.MoRong;
using HID.EFMODEL.DataModels;
using HID.Filter;

namespace HIS.Desktop.Plugins.PatientInfo
{
    public partial class frmPatientInfo : HIS.Desktop.Utility.FormBase
    {
        #region Declaration
        bool isGKS = false;
        bool isTxtPatientDobPreviewKeyDown = false;
        internal bool isNotPatientDayDob = false;
        internal int ActionType = 0;// No action    
        V_HIS_PATIENT currentPatient;
        V_HIS_PATIENT delegatePatient;
        HisTreatmentPatientInfoSDO treatmentPatientInfoSdo;
        internal MOS.EFMODEL.DataModels.V_HIS_PATIENT currentVHisPatientDTO = null;
        internal int PatientRowCount = 0;
        int positionHandleControlPatientInfo = -1;
        RefeshReference refeshReference;
        internal HIS.UC.WorkPlace.UCWorkPlaceCombo workPlacecbo;
        internal HIS.UC.WorkPlace.WorkPlaceProcessor workPlaceProcessor;
        internal HIS.UC.WorkPlace.WorkPlaceProcessor.Template workPlaceTemplate;
        UserControl ucWorkPlace;
        long CheDoHienThiNoiLamViecManHinhDangKyTiepDon;
        List<HisPatientSDO> currentSearchedPatients;
        List<HID_HOUSEHOLD_RELATION> houseHoldRelation;
        //bool validate;
        #endregion

        #region Load

        public frmPatientInfo(V_HIS_PATIENT _currentPatient, RefeshReference _refeshReference)
        {
            try
            {
                InitializeComponent();
                this.CheDoHienThiNoiLamViecManHinhDangKyTiepDon = ConfigApplicationWorker.Get<long>("CONFIG_KEY__HIEN_THI_NOI_LAM_VIEC_THEO_DINH_DANG_MAN_HINH_DANG_KY");
                this.currentPatient = _currentPatient;
                this.currentVHisPatientDTO = _currentPatient;
                this.refeshReference = _refeshReference;
                InitWorkPlaceControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmPatientInfo()
        {
            try
            {
                InitializeComponent();
                this.CheDoHienThiNoiLamViecManHinhDangKyTiepDon = ConfigApplicationWorker.Get<long>("CONFIG_KEY__HIEN_THI_NOI_LAM_VIEC_THEO_DINH_DANG_MAN_HINH_DANG_KY");
                InitWorkPlaceControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmPatientInfo_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                houseHoldRelation = GetHidHouseHoldRelation();
                SetDefaultData();
                SetCaptionByLanguageKey();
                FillDataToControlsForm();
                SetIcon();
                FillDataPatientToControl(this.currentVHisPatientDTO);
                if (MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(dtDOB.DateTime))
                {
                    isGKS = true;
                    if (this.lciCMNDRelative.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                        this.lciCMNDRelative.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
                ValidateForm(isGKS);

                //#2173
                // SetEditInfo();

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.PatientInfo.Resources.Lang", typeof(HIS.Desktop.Plugins.PatientInfo.frmPatientInfo).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmPatientInfo.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupControl1.Text = Inventec.Common.Resource.Get.Value("frmPatientInfo.groupControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmPatientInfo.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboEthnic.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientInfo.cboEthnic.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmPatientInfo.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMilitaryRank.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientInfo.cboMilitaryRank.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboNation.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientInfo.cboNation.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboCommune.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientInfo.cboCommune.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDistricts.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientInfo.cboDistricts.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboProvince.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientInfo.cboProvince.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboCareer.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientInfo.cboCareer.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboGender1.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientInfo.cboGender1.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientName.Text = Inventec.Common.Resource.Get.Value("frmPatientInfo.lciPatientName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEthnic.Text = Inventec.Common.Resource.Get.Value("frmPatientInfo.lciEthnic.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciGender.Text = Inventec.Common.Resource.Get.Value("frmPatientInfo.lciGender.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCareer.Text = Inventec.Common.Resource.Get.Value("frmPatientInfo.lciCareer.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAdress.Text = Inventec.Common.Resource.Get.Value("frmPatientInfo.lciAdress.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCommune.Text = Inventec.Common.Resource.Get.Value("frmPatientInfo.lciCommune.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDistricts.Text = Inventec.Common.Resource.Get.Value("frmPatientInfo.lciDistricts.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciProvince.Text = Inventec.Common.Resource.Get.Value("frmPatientInfo.lciProvince.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPhone.Text = Inventec.Common.Resource.Get.Value("frmPatientInfo.lciPhone.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem24.Text = Inventec.Common.Resource.Get.Value("frmPatientInfo.layoutControlItem24.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.lciPersonFamily.Text = Inventec.Common.Resource.Get.Value("frmPatientInfo.lciPersonFamily.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRelation.Text = Inventec.Common.Resource.Get.Value("frmPatientInfo.lciRelation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lctContact.Text = Inventec.Common.Resource.Get.Value("frmPatientInfo.lctContact.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNation.Text = Inventec.Common.Resource.Get.Value("frmPatientInfo.lciNation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMilitaryRank.Text = Inventec.Common.Resource.Get.Value("frmPatientInfo.lciMilitaryRank.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDOB.Text = Inventec.Common.Resource.Get.Value("frmPatientInfo.lciDOB.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmPatientInfo.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemSave.Caption = Inventec.Common.Resource.Get.Value("frmPatientInfo.barButtonItemSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmPatientInfo.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemSave.Caption = Inventec.Common.Resource.Get.Value("frmPatientInfo.barButtonItemSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<HID_HOUSEHOLD_RELATION> GetHidHouseHoldRelation()
        {
            List<HID_HOUSEHOLD_RELATION> result = null;
            try
            {
                CommonParam param = new CommonParam();
                HidHouseholdRelationFilter filter = new HidHouseholdRelationFilter();
                filter.IS_ACTIVE = 1;
                result = new BackendAdapter(param).Get<List<HID_HOUSEHOLD_RELATION>>("api/HidHouseHoldRelation/Get", ApiConsumers.HidConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = null;
            }
            return result;
        }

        private void SetDefaultData()
        {
            try
            {
                ActionType = GlobalVariables.ActionEdit;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            try
            {
                txtPatientName.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataPatientToControl(MOS.EFMODEL.DataModels.V_HIS_PATIENT patientDto)
        {
            try
            {
                txtCMNDRelative.Text = patientDto.RELATIVE_CMND_NUMBER;
                txtPatientName.Text = patientDto.VIR_PATIENT_NAME;
                if (patientDto.DOB > 0)
                {
                    if (patientDto.IS_HAS_NOT_DAY_DOB == 1)
                        txtPatientDOB.Text = patientDto.DOB.ToString().Substring(0, 4);
                    else
                    {
                        LoadDobPatientToForm(patientDto);
                    }
                    //dtDOB.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientDto.DOB);
                }
                var gender = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().FirstOrDefault(o => o.ID == patientDto.GENDER_ID);
                if (gender != null)
                {
                    cboGender1.EditValue = gender.ID;
                    txtGender.Text = gender.GENDER_CODE;
                }
                txtAddress.Text = patientDto.ADDRESS;
                var national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().FirstOrDefault(o => o.NATIONAL_NAME == patientDto.NATIONAL_NAME);
                if (national != null)
                {
                    cboNation.EditValue = national.NATIONAL_CODE;
                    txtNation.Text = national.NATIONAL_CODE;
                }
                var ethnic = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>().FirstOrDefault(o => o.ETHNIC_NAME == patientDto.ETHNIC_NAME);
                if (ethnic != null)
                {
                    cboEthnic.EditValue = ethnic.ETHNIC_CODE;
                    txtEthnic.Text = ethnic.ETHNIC_CODE;
                }


                var career = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().FirstOrDefault(o => o.ID == patientDto.CAREER_ID);
                if (career != null)
                {
                    cboCareer.EditValue = patientDto.CAREER_ID;
                    txtCareer.Text = career.CAREER_CODE;
                }

                var province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_NAME == patientDto.PROVINCE_NAME);
                if (province != null)
                {
                    cboProvince.EditValue = province.PROVINCE_CODE;
                    txtProvince.Text = province.PROVINCE_CODE;
                    LoadDistrictsCombo("", province.PROVINCE_CODE, false);
                }
                var district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().FirstOrDefault(o => (o.INITIAL_NAME + " " + o.DISTRICT_NAME) == patientDto.DISTRICT_NAME && o.PROVINCE_NAME == patientDto.PROVINCE_NAME);
                if (district != null)
                {
                    cboDistricts.EditValue = district.DISTRICT_CODE;
                    txtDistricts.Text = district.DISTRICT_CODE;
                    LoadCommuneCombo("", district.DISTRICT_CODE, false);
                }
                var commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().FirstOrDefault(o => (o.INITIAL_NAME + " " + o.COMMUNE_NAME) == patientDto.COMMUNE_NAME && (o.DISTRICT_INITIAL_NAME + " " + o.DISTRICT_NAME) == patientDto.DISTRICT_NAME);
                if (commune != null)
                {
                    cboCommune.EditValue = commune.COMMUNE_CODE;
                    txtCommune.Text = commune.COMMUNE_CODE;
                }

                var provinceKS = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_NAME == patientDto.BORN_PROVINCE_NAME);
                if (provinceKS != null)
                {
                    cboTinhKhaiSinh.EditValue = provinceKS.PROVINCE_CODE;
                    txtTinhKhaiSinh.Text = provinceKS.PROVINCE_CODE;
                    //LoadDistrictsCombo("", province.PROVINCE_CODE, false);
                }

                var provinceHT = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_NAME == patientDto.HT_PROVINCE_NAME);
                if (provinceHT != null)
                {
                    cboTinhHienTai.EditValue = provinceHT.PROVINCE_NAME;
                    txtTinhHienTai.Text = provinceHT.PROVINCE_CODE;
                    LoadDistrictsComboHT("", provinceHT.PROVINCE_CODE, false);
                }
                var districtHT = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().FirstOrDefault(o => o.DISTRICT_NAME == patientDto.HT_DISTRICT_NAME && provinceHT != null && o.PROVINCE_CODE == provinceHT.PROVINCE_CODE);
                if (districtHT != null)
                {
                    cboHuyenHienTai.EditValue = districtHT.DISTRICT_NAME;
                    txtHuyenHienTai.Text = districtHT.DISTRICT_CODE;
                    LoadCommuneComboHT("", districtHT.DISTRICT_CODE, false);
                }
                var communeHT = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().FirstOrDefault(o => o.COMMUNE_NAME == patientDto.HT_COMMUNE_NAME && districtHT != null && o.DISTRICT_CODE == districtHT.DISTRICT_CODE);
                if (communeHT != null)
                {
                    cboXaHienTai.EditValue = communeHT.COMMUNE_CODE;
                    txtXaHienTai.Text = communeHT.COMMUNE_CODE;
                }

                txtMilitaryRankCode.Text = patientDto.MILITARY_RANK_CODE;
                cboMilitaryRank.EditValue = patientDto.MILITARY_RANK_ID;
                txtPhone.Text = patientDto.PHONE;
                txtRelation.Text = patientDto.RELATIVE_TYPE;
                txtPersonFamily.Text = patientDto.RELATIVE_NAME;
                txtContact.Text = patientDto.RELATIVE_ADDRESS;
                txtEmail.Text = patientDto.EMAIL;


                if (this.currentPatient != null)
                {
                    chkBNManTinh.Checked = (patientDto.IS_CHRONIC == 1 ? true : false);
                }

                if (this.CheDoHienThiNoiLamViecManHinhDangKyTiepDon != 1
                    && workPlaceProcessor != null
                    && ucWorkPlace != null)
                {
                    if (patientDto.WORK_PLACE_ID > 0)
                    {
                        var workPlace = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_WORK_PLACE>().FirstOrDefault(o => o.ID == patientDto.WORK_PLACE_ID);
                        if (workPlace != null)
                            workPlaceProcessor.SetValue(ucWorkPlace, workPlace);
                        else
                            workPlaceProcessor.SetValue(ucWorkPlace, null);
                    }
                }
                else
                {
                    workPlaceProcessor.SetValue(ucWorkPlace, patientDto.WORK_PLACE);
                }

                //cboTinhKhaiSinh.EditValue = patientDto.BORN_PROVINCE_CODE;
                //txtTinhKhaiSinh.Text = patientDto.BORN_PROVINCE_CODE;

                //cboTinhHienTai.EditValue = patientDto.HT_PROVINCE_NAME;
                //var tinhHienTai = BackendDataWorker.Get<SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_NAME == patientDto.HT_PROVINCE_NAME);
                //if (tinhHienTai != null)
                //{
                //    txtTinhHienTai.Text = tinhHienTai.PROVINCE_CODE;
                //}

                //cboHuyenHienTai.EditValue = patientDto.HT_DISTRICT_NAME;
                //var huyenHienTai = BackendDataWorker.Get<SDA_DISTRICT>().FirstOrDefault(o => o.DISTRICT_NAME == patientDto.HT_DISTRICT_NAME);
                //if (huyenHienTai != null)
                //{
                //    txtHuyenHienTai.Text = huyenHienTai.DISTRICT_CODE;
                //}

                //cboXaHienTai.EditValue = patientDto.HT_COMMUNE_NAME;
                //var xaHienTai = BackendDataWorker.Get<SDA_COMMUNE>().FirstOrDefault(o => o.COMMUNE_NAME == patientDto.HT_COMMUNE_NAME);
                //if (xaHienTai != null)
                //{
                //    txtXaHienTai.Text = xaHienTai.COMMUNE_CODE;
                //}

                txtDiaChiHienTai.Text = patientDto.HT_ADDRESS;
                if (!string.IsNullOrEmpty(patientDto.CMND_NUMBER))
                {
                    txtCMND.Text = patientDto.CMND_NUMBER;
                    txtNoiCap.Text = patientDto.CMND_PLACE;
                    dtNgayCap.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientDto.CMND_DATE ?? 0);
                }
                else
                {
                    txtCMND.Text = patientDto.CCCD_NUMBER;
                    txtNoiCap.Text = patientDto.CCCD_PLACE;
                    dtNgayCap.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientDto.CCCD_DATE ?? 0);
                }
                cboNhomMau.EditValue = patientDto.BLOOD_ABO_CODE;
                cboRh.EditValue = patientDto.BLOOD_RH_CODE;
                txtSoHoKhau.Text = patientDto.HOUSEHOLD_CODE;
                txtHoTenBo.Text = patientDto.FATHER_NAME;
                txtHoTenMe.Text = patientDto.MOTHER_NAME;
                cboQuanHeChuHo.EditValue = patientDto.HOUSEHOLD_RELATION_NAME;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FillDataToControlsForm()
        {
            try
            {
                FillDataToLookupedit(this.cboGender1, "GENDER_NAME", "ID", "GENDER_CODE", BackendDataWorker.Get<HIS_GENDER>());
                FillDataToLookupedit(this.cboProvince, "PROVINCE_NAME", "PROVINCE_CODE", "PROVINCE_CODE", BackendDataWorker.Get<V_SDA_PROVINCE>());
                FillDataToLookupedit(this.cboTinhKhaiSinh, "PROVINCE_NAME", "PROVINCE_CODE", "PROVINCE_CODE", BackendDataWorker.Get<V_SDA_PROVINCE>());
                FillDataToLookupedit(this.cboTinhHienTai, "PROVINCE_NAME", "PROVINCE_NAME", "PROVINCE_CODE", BackendDataWorker.Get<V_SDA_PROVINCE>());
                FillDataToLookupedit(this.cboCareer, "CAREER_NAME", "ID", "CAREER_CODE", BackendDataWorker.Get<HIS_CAREER>());
                FillDataToLookupedit(this.cboEthnic, "ETHNIC_NAME", "ETHNIC_CODE", "ETHNIC_CODE", BackendDataWorker.Get<SDA_ETHNIC>());
                FillDataToLookupedit(this.cboNation, "NATIONAL_NAME", "NATIONAL_CODE", "NATIONAL_CODE", BackendDataWorker.Get<SDA_NATIONAL>());
                FillDataToLookupedit(this.cboMilitaryRank, "MILITARY_RANK_NAME", "ID", "MILITARY_RANK_CODE", BackendDataWorker.Get<HIS_MILITARY_RANK>());
                FillDataToLookupedit(this.cboQuanHeChuHo, "HOUSEHOLD_RELATION_NAME", "HOUSEHOLD_RELATION_NAME", "HOUSEHOLD_RELATION_CODE", houseHoldRelation);
                FillDataToLookupedit1(this.cboNhomMau, "BLOOD_ABO_CODE", "BLOOD_ABO_CODE", BackendDataWorker.Get<HIS_BLOOD_ABO>());
                FillDataToLookupedit1(this.cboRh, "BLOOD_RH_CODE", "BLOOD_RH_CODE", BackendDataWorker.Get<HIS_BLOOD_RH>());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void LoadDobPatientToForm(MOS.EFMODEL.DataModels.V_HIS_PATIENT patientDTO)
        {
            try
            {
                if (patientDTO.DOB != null)
                {
                    string nthnm = patientDTO.DOB.ToString();
                    DateTime dtNgSinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientDTO.DOB) ?? DateTime.MinValue;
                    dtDOB.DateTime = dtNgSinh;
                    txtPatientDOB.Text = dtNgSinh.ToString("dd/MM/yyyy");
                    int age = Inventec.Common.TypeConvert.Parse.ToInt32(nthnm.Substring(8, 2));
                    //bool isGKS = MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(dtNgSinh);
                    //ValidateForm(isGKS);
                    //CalulatePatientAge(dtNgSinh);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void LoadCurrentPatient(long patientId, ref MOS.EFMODEL.DataModels.HIS_PATIENT currentVHisPatientDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientFilter filter = new MOS.Filter.HisPatientFilter();
                filter.ID = patientId;
                currentVHisPatientDTO = new BackendAdapter(param).Get<List<HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GET, ApiConsumers.MosConsumer, filter, null).ToList().First();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdatePatientDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_PATIENT patientDTO)
        {
            try
            {
                int idx = txtPatientName.Text.Trim().LastIndexOf(" ");
                if (idx > -1)
                {
                    patientDTO.FIRST_NAME = txtPatientName.Text.Trim().Substring(idx).Trim();
                    patientDTO.LAST_NAME = txtPatientName.Text.Trim().Substring(0, idx).Trim();
                }
                else
                {
                    patientDTO.FIRST_NAME = txtPatientName.Text.Trim();
                    patientDTO.LAST_NAME = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Loi xu ly cat chuoi ho ten benh nhan: ", ex);
            }
            try
            {
                if (dtDOB.DateTime != null)
                {
                    if (txtPatientDOB.Text.Length == 4)
                    {
                        patientDTO.IS_HAS_NOT_DAY_DOB = 1;
                        string dateDob = txtPatientDOB.Text.Substring(0, 4) + "0101";
                        string timeDob = "00";
                        patientDTO.DOB = Inventec.Common.TypeConvert.Parse.ToInt64(dateDob + timeDob + "0000");
                    }
                    else
                    {
                        txtPatientDOB.Text = dtDOB.Text;
                        string dateDob = dtDOB.DateTime.ToString("yyyyMMdd");
                        string timeDob = "00";
                        patientDTO.DOB = Inventec.Common.TypeConvert.Parse.ToInt64(dateDob + timeDob + "0000");
                        patientDTO.IS_HAS_NOT_DAY_DOB = 0;
                    }
                }


                if (cboGender1.EditValue != null)
                    patientDTO.GENDER_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboGender1.EditValue ?? "0").ToString());
                patientDTO.ADDRESS = txtAddress.Text;
                patientDTO.PROVINCE_NAME = ((cboProvince.Text ?? "").ToString());
                patientDTO.PROVINCE_CODE = txtProvince.Text;
                patientDTO.DISTRICT_NAME = ((cboDistricts.Text ?? "").ToString());
                patientDTO.COMMUNE_NAME = ((cboCommune.Text ?? "").ToString());
                if (cboCareer.EditValue != null)
                    patientDTO.CAREER_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboCareer.EditValue ?? "").ToString());
                else
                    patientDTO.CAREER_ID = null;
                patientDTO.ETHNIC_NAME = ((cboEthnic.Text ?? "").ToString());
                patientDTO.NATIONAL_NAME = ((cboNation.Text ?? "").ToString());
                patientDTO.PHONE = txtPhone.Text;
                patientDTO.RELATIVE_NAME = txtPersonFamily.Text;
                patientDTO.RELATIVE_TYPE = txtRelation.Text;
                patientDTO.EMAIL = txtEmail.Text;
                patientDTO.RELATIVE_ADDRESS = txtContact.Text;

                patientDTO.IS_CHRONIC = (short)(chkBNManTinh.Checked ? 1 : 0);
                patientDTO.RELATIVE_CMND_NUMBER = txtCMNDRelative.Text;

                if (cboMilitaryRank.EditValue != null)
                    patientDTO.MILITARY_RANK_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboMilitaryRank.EditValue ?? "").ToString());
                else
                    patientDTO.MILITARY_RANK_ID = null;
                if (workPlaceTemplate == WorkPlaceProcessor.Template.Combo)
                {
                    patientDTO.WORK_PLACE_ID = (long?)workPlaceProcessor.GetValue(ucWorkPlace, workPlaceTemplate);
                    //patientDTO.WORK_PLACE = "";
                }
                else
                {
                    patientDTO.WORK_PLACE = (string)workPlaceProcessor.GetValue(ucWorkPlace, workPlaceTemplate);
                    //patientDTO.WORK_PLACE_ID = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        private void TimBenhNhanTheoDieuKien(bool isShowMessage)
        {
            try
            {
                string strDob = "";
                if (txtPatientDOB.Text.Length == 4)
                    strDob = "01/01/" + txtPatientDOB.Text;
                else if (txtPatientDOB.Text.Length == 8)
                {
                    strDob = txtPatientDOB.Text.Substring(0, 2) + "/" + txtPatientDOB.Text.Substring(2, 2) + "/" + txtPatientDOB.Text.Substring(4, 4);
                }
                else
                    strDob = txtPatientDOB.Text;
                dtDOB.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(strDob);
                dtDOB.Update();
                if ((dtDOB.EditValue == null
                    || dtDOB.DateTime == DateTime.MinValue)
                    || cboGender == null
                    || String.IsNullOrEmpty(txtPatientName.Text.Trim()))
                {
                    return;
                }
                LogSystem.Debug("Bat dau tim kiem benh nhan theo filter.");
                string dateDob = dtDOB.DateTime.ToString("yyyyMMdd");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PatientDobLeaveProcess(bool showMessage)
        {
            try
            {
                string strDob = "";
                if (txtPatientDOB.Text.Length == 2 || txtPatientDOB.Text.Length == 1)
                {
                    int patientDob = Int32.Parse(txtPatientDOB.Text);
                    if (patientDob < 7)
                    {
                        if (showMessage)
                            MessageBox.Show(ResourceLanguageManager.NgaySinhKhongDuocNhoHon7);
                        FocusMoveText(this.txtPatientDOB);
                        return;
                    }
                    else
                    {
                        txtPatientDOB.Text = (DateTime.Now.Year - patientDob).ToString();
                    }
                }
                else if (txtPatientDOB.Text.Length == 4)
                {
                    if (Inventec.Common.TypeConvert.Parse.ToInt64(txtPatientDOB.Text) <= DateTime.Now.Year)
                    {
                        strDob = "01/01/" + txtPatientDOB.Text;
                        isNotPatientDayDob = true;
                    }
                    else
                    {
                        if (showMessage)
                            MessageBox.Show(ResourceLanguageManager.NhapNgaySinhKhongDungDinhDang);
                        FocusMoveText(this.txtPatientDOB);
                        return;
                    }
                }
                else if (txtPatientDOB.Text.Length < 4)
                {
                    if (showMessage)
                        MessageBox.Show(ResourceLanguageManager.NhapNgaySinhKhongDungDinhDang);
                    FocusMoveText(this.txtPatientDOB);
                    return;
                }
                else if (txtPatientDOB.Text.Length == 8)
                {
                    strDob = txtPatientDOB.Text.Substring(0, 2) + "/" + txtPatientDOB.Text.Substring(2, 2) + "/" + txtPatientDOB.Text.Substring(4, 4);
                    if (HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(strDob).Value.Date <= DateTime.Now.Date)
                    {
                        txtPatientDOB.Text = strDob;
                        isNotPatientDayDob = false;
                        dtDOB.EditValue = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(strDob);
                        dtDOB.Update();
                    }
                    else
                    {
                        if (showMessage)
                        {
                            txtPatientDOB.Text = strDob;
                            return;
                        }
                    }
                }
                else if (txtPatientDOB.Text.Length == 10)
                {
                    if (HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(txtPatientDOB.Text).Value.Date <= DateTime.Now.Date)
                    {
                        isNotPatientDayDob = false;
                        dtDOB.EditValue = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(strDob);
                        dtDOB.Update();
                    }
                    else
                    {
                        if (showMessage)
                            MessageBox.Show(ResourceLanguageManager.NhapNgaySinhKhongDungDinhDang);
                        FocusMoveText(this.txtPatientDOB);
                        return;
                    }
                }
                else
                {
                    if (showMessage)
                        MessageBox.Show(ResourceLanguageManager.NhapNgaySinhKhongDungDinhDang);
                    FocusMoveText(this.txtPatientDOB);
                    return;
                }

                if (String.IsNullOrWhiteSpace(strDob))
                {
                    strDob = txtPatientDOB.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkUpdateNew_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboEthnic_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboEthnic.EditValue = null;
                    txtEthnic.Text = "";
                    cboEthnic.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboProvince_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (!cboProvince.ReadOnly && e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboProvince.EditValue = null;
                    txtProvince.Text = "";
                    cboDistricts.EditValue = null;
                    txtDistricts.Text = "";
                    cboCommune.EditValue = null;
                    txtCommune.Text = "";
                    LoadDistrictsCombo("", null, false);
                    LoadCommuneCombo("", null, false);
                    cboProvince.Properties.Buttons[1].Visible = false;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistricts_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (!cboDistricts.ReadOnly && e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDistricts.EditValue = null;
                    txtDistricts.Text = "";
                    cboCommune.EditValue = null;
                    txtCommune.Text = "";
                    LoadCommuneCombo("", null, false);
                    cboDistricts.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEthnic_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboEthnic.Properties.Buttons[1].Visible = false;
                if (cboEthnic.EditValue != null)
                {
                    cboEthnic.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCareer_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboCareer.Properties.Buttons[1].Visible = false;
                if (cboCareer.EditValue != null)
                {
                    cboCareer.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboProvince_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboProvince.Properties.Buttons[1].Visible = false;
                if (cboProvince.EditValue != null)
                {
                    cboProvince.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDistricts_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboDistricts.Properties.Buttons[1].Visible = false;
                if (cboDistricts.EditValue != null)
                {
                    cboDistricts.Properties.Buttons[1].Visible = true;
                }
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCommune_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (!cboCommune.ReadOnly && e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboCommune.EditValue = null;
                    txtCommune.Text = "";
                    cboCommune.Properties.Buttons[1].Visible = false;
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
                cboCommune.Properties.Buttons[1].Visible = false;
                if (cboCommune.EditValue != null)
                {
                    cboCommune.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboNation_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboNation.EditValue = null;
                    txtNation.Text = "";
                    cboNation.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNation_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboNation.Properties.Buttons[1].Visible = false;
                if (cboNation.EditValue != null)
                {
                    cboNation.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMilitaryRank_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMilitaryRank.EditValue = null;
                    txtMilitaryRankCode.Text = "";
                    cboMilitaryRank.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMilitaryRank_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboMilitaryRank.Properties.Buttons[1].Visible = false;
                if (cboMilitaryRank.EditValue != null)
                {
                    cboMilitaryRank.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBloodAbo_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCareer_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboCareer.EditValue = null;
                    txtCareer.Text = "";
                    cboCareer.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtPatientDOB_Leave(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtPatientDOB_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                string birthDay = txtPatientDOB.Text;
                if (!String.IsNullOrEmpty(birthDay))
                {
                    int lengthBirthDay = birthDay.Length;
                    if (lengthBirthDay == 4)
                    {
                        int age = DateTime.Now.Year - int.Parse(birthDay);
                        if (age == 6)
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            //try
            //{
            //    if (!String.IsNullOrEmpty(txtPatientDOB.Text))
            //    {
            //        string strDob = "";

            //        if (txtPatientDOB.Text.Length == 2 || txtPatientDOB.Text.Length == 1)
            //        {
            //            int patientDob = Int32.Parse(txtPatientDOB.Text);
            //            if (patientDob < 7)
            //            {
            //                MessageBox.Show(ResourceLanguageManager.NgaySinhKhongDuocNhoHon7);
            //                FocusMoveText(this.txtPatientDOB);
            //                return;
            //            }
            //            else
            //            {
            //                txtPatientDOB.Text = (DateTime.Now.Year - patientDob).ToString();
            //            }
            //        }
            //        else if (txtPatientDOB.Text.Length == 4)
            //        {
            //            if (Inventec.Common.TypeConvert.Parse.ToInt64(txtPatientDOB.Text) <= DateTime.Now.Year)
            //            {
            //                strDob = "01/01/" + txtPatientDOB.Text;
            //                isNotPatientDayDob = true;
            //            }
            //            else
            //            {
            //                MessageBox.Show(ResourceLanguageManager.NhapNgaySinhKhongDungDinhDang);
            //                FocusMoveText(this.txtPatientDOB);
            //                return;
            //            }

            //        }
            //        else if (txtPatientDOB.Text.Length == 8)
            //        {
            //            strDob = txtPatientDOB.Text.Substring(0, 2) + "/" + txtPatientDOB.Text.Substring(2, 2) + "/" + txtPatientDOB.Text.Substring(4, 4);
            //            if (HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(strDob).Value.Date <= DateTime.Now.Date)
            //            {
            //                strDob = txtPatientDOB.Text.Substring(0, 2) + "/" + txtPatientDOB.Text.Substring(2, 2) + "/" + txtPatientDOB.Text.Substring(4, 4);
            //                txtPatientDOB.Text = strDob;
            //                isNotPatientDayDob = false;
            //            }
            //            else
            //            {
            //                MessageBox.Show(ResourceLanguageManager.ThongTinNgaySinhPhaiNhoHonNgayHienTai);
            //                FocusMoveText(this.txtPatientDOB);
            //                return;
            //            }
            //        }
            //        else
            //        {
            //            MessageBox.Show(ResourceLanguageManager.NhapNgaySinhKhongDungDinhDang);
            //            FocusMoveText(this.txtPatientDOB);
            //            return;
            //        }


            //        if (String.IsNullOrWhiteSpace(strDob))
            //        {
            //            strDob = txtPatientDOB.Text;
            //        }
            //        TimBenhNhanTheoDieuKien(true);
            //        isTxtPatientDobPreviewKeyDown = true;

            //        FocusMoveText(this.txtEthnic);
            //    }
            //    else
            //    {
            //        FocusMoveText(this.txtPatientDOB);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}

        }

        private void CalulatePatientAge(DateTime ngaySinh)
        {
            try
            {
                if (ngaySinh != null && ngaySinh != DateTime.MinValue)
                {
                    isGKS = true;
                    DateTime dtNgSinh = ngaySinh;
                    TimeSpan diff = DateTime.Now - dtNgSinh;
                    long tongsogiay = diff.Ticks;
                    if (tongsogiay < 0)
                    {
                        if (this.lciCMNDRelative.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                            this.lciCMNDRelative.AppearanceItemCaption.ForeColor = Color.Maroon;
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



                    if (nam >= 0 && nam <= 6)
                    {
                        if (nam == 6)
                        {
                            if (thang <= 0 && ngay <= 0)
                            {
                                if (this.lciCMNDRelative.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                                    this.lciCMNDRelative.AppearanceItemCaption.ForeColor = Color.Maroon;
                            }
                        }
                        else
                        {
                            if (this.lciCMNDRelative.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                                this.lciCMNDRelative.AppearanceItemCaption.ForeColor = Color.Maroon;
                        }

                    }
                    LogSystem.Info("p4. Ket thuc ham tinh tuoi benh nhan.");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientDOB_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPatientDOB_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void txtCMNDRelative_Leave(object sender, EventArgs e)
        {
        }

        private void txtCMNDRelative_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkBNManTinh.Properties.FullFocusRect = true;
                    chkBNManTinh.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtPatientDOB_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            //try
            //{
            //    HIS.Desktop.Plugins.PatientInfo.DateValidObject dateValidObject = DateValidObject.ValidPatientDob(this.txtPatientDOB.Text);
            //    if (dateValidObject != null)
            //    {
            //        e.ErrorText = dateValidObject.Message;
            //    }

            //    AutoValidate = AutoValidate.EnableAllowFocusChange;
            //    e.ExceptionMode = ExceptionMode.DisplayError;
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void txtTinhKhaiSinh_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadProvinceComboKS(strValue.ToUpper(), true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboTinhKhaiSinh_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboTinhKhaiSinh.EditValue != null && cboTinhKhaiSinh.EditValue != cboTinhKhaiSinh.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().SingleOrDefault(o => o.PROVINCE_CODE == cboTinhKhaiSinh.EditValue.ToString());
                        if (province != null)
                        {
                            //LoadDistrictsCombo("", province.PROVINCE_CODE, false);
                            txtTinhKhaiSinh.Text = province.PROVINCE_CODE;
                        }
                    }
                    txtChuongTrinh.Focus();
                    txtChuongTrinh.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTinhKhaiSinh_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboTinhKhaiSinh.Properties.Buttons[1].Visible = false;
                if (cboTinhKhaiSinh.EditValue != null)
                {
                    cboTinhKhaiSinh.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtChuongTrinh_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void cboChuongTrinh_Closed(object sender, ClosedEventArgs e)
        {

        }

        private void cboChuongTrinh_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboChuongTrinh.Properties.Buttons[1].Visible = false;
                if (cboChuongTrinh.EditValue != null)
                {
                    cboChuongTrinh.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTinhHienTai_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtTinhHienTai.Text))
                    {
                        string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                        LoadProvinceComboHT(strValue.ToUpper(), true);
                    }
                    else
                    {
                        cboTinhHienTai.Focus();
                        cboTinhHienTai.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboTinhHienTai_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboTinhHienTai.EditValue != null && cboTinhHienTai.EditValue != cboProvince.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().SingleOrDefault(o => o.PROVINCE_NAME == cboTinhHienTai.EditValue.ToString());
                        if (province != null)
                        {
                            LoadDistrictsComboHT("", province.PROVINCE_CODE, false);
                            txtTinhHienTai.Text = province.PROVINCE_CODE;
                        }
                    }
                    txtHuyenHienTai.Text = "";
                    txtHuyenHienTai.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTinhHienTai_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboTinhHienTai.Properties.Buttons[1].Visible = false;
                if (cboTinhHienTai.EditValue != null)
                {
                    cboTinhHienTai.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtHuyenHienTai_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtHuyenHienTai.Text))
                    {
                        string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                        string provinceCode = "";
                        if (cboTinhHienTai.EditValue != null)
                        {
                            SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().SingleOrDefault(o => o.PROVINCE_NAME == cboTinhHienTai.EditValue.ToString());
                            if (province != null)
                            {
                                provinceCode = province.PROVINCE_CODE;
                                LoadDistrictsComboHT(strValue.ToUpper(), provinceCode, true);
                            }
                            else {
                                DevExpress.XtraEditors.XtraMessageBox.Show("Không có thông tin tỉnh hiện tại", "Thông báo");
                            }
                        }
                        
                    }
                    else
                    {
                        cboHuyenHienTai.Focus();
                        cboHuyenHienTai.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboHuyenHienTai_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboHuyenHienTai.EditValue != null && cboHuyenHienTai.EditValue != cboDistricts.OldEditValue)
                    {
                        //string str = cboDistricts.EditValue.ToString();
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>()
                            .FirstOrDefault(o => o.DISTRICT_NAME == cboHuyenHienTai.EditValue.ToString()
                                && (String.IsNullOrEmpty((cboTinhHienTai.EditValue ?? "").ToString()) || o.PROVINCE_NAME == (cboTinhHienTai.EditValue ?? "").ToString()));
                        if (district != null)
                        {
                            if (String.IsNullOrEmpty((cboTinhHienTai.EditValue ?? "").ToString()))
                            {
                                cboTinhHienTai.EditValue = district.PROVINCE_CODE;
                            }
                            LoadCommuneComboHT("", district.DISTRICT_CODE, false);
                            txtHuyenHienTai.Text = district.DISTRICT_CODE;
                            cboXaHienTai.EditValue = null;
                            txtXaHienTai.Text = "";
                        }
                    }
                    FocusMoveText(this.txtXaHienTai);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHuyenHienTai_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboHuyenHienTai.Properties.Buttons[1].Visible = false;
                if (cboHuyenHienTai.EditValue != null)
                {
                    cboHuyenHienTai.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtXaHienTai_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtXaHienTai.Text))
                    {
                        string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                        string districtCode = "";
                        if (cboHuyenHienTai.EditValue != null)
                        {
                            SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>()
                            .SingleOrDefault(o => o.DISTRICT_NAME == cboHuyenHienTai.EditValue.ToString());
                            districtCode = district.DISTRICT_CODE;
                            LoadCommuneComboHT(strValue.ToUpper(), districtCode, true);
                        }
                        else
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Không có thông tin huyện hiện tại", "Thông báo");
                        }
                        //string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                        //string districtCode = "";
                        //if (cboHuyenHienTai.EditValue != null && cboTinhHienTai.EditValue != null)
                        //{
                        //    SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>()
                        //    .SingleOrDefault(o => o.DISTRICT_NAME == cboHuyenHienTai.EditValue.ToString()
                        //        && (String.IsNullOrEmpty((cboTinhHienTai.EditValue ?? "").ToString()) || o.PROVINCE_NAME == (cboTinhHienTai.EditValue ?? "").ToString()));
                        //    if (district != null)
                        //    {
                        //        districtCode = district.DISTRICT_CODE;
                        //        LoadCommuneComboHT(strValue.ToUpper(), districtCode, true);
                        //    }
                        //    else
                        //    {
                        //        DevExpress.XtraEditors.XtraMessageBox.Show("Không có thông tin huyện hiện tại", "Thông báo");
                        //    }
                        //}
                        
                    }
                    else
                    {
                        cboXaHienTai.Focus();
                        cboXaHienTai.ShowPopup();
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void cboXaHienTai_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboXaHienTai.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>()
                            .FirstOrDefault(o =>
                                o.COMMUNE_CODE == cboXaHienTai.EditValue.ToString()
                                    && (String.IsNullOrEmpty((cboHuyenHienTai.EditValue ?? "").ToString()) || o.DISTRICT_NAME == (cboHuyenHienTai.EditValue ?? "").ToString())
                                );
                        if (commune != null)
                        {
                            txtXaHienTai.Text = commune.COMMUNE_CODE;
                            if (String.IsNullOrEmpty((cboTinhHienTai.EditValue ?? "").ToString()) && String.IsNullOrEmpty((cboHuyenHienTai.EditValue ?? "").ToString()))
                            {
                                cboHuyenHienTai.EditValue = commune.DISTRICT_CODE;
                                SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.ID == commune.DISTRICT_ID).FirstOrDefault();
                                if (district != null)
                                {
                                    cboTinhHienTai.EditValue = district.PROVINCE_CODE;
                                }
                            }
                        }
                    }
                    FocusMoveText(this.txtDiaChiHienTai);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboXaHienTai_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboXaHienTai.Properties.Buttons[1].Visible = false;
                if (cboXaHienTai.EditValue != null)
                {
                    cboXaHienTai.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDiaChiHienTai_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtCMND.Focus();
                    txtCMND.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCMND_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtNgayCap.Focus();
                    dtNgayCap.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtNgayCap_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNoiCap.Focus();
                    txtNoiCap.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNoiCap_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboNhomMau.Focus();
                    cboNhomMau.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void cboNhomMau_KeyDown(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            cboRh.Focus();
        //            cboRh.ShowPopup();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void cboRh_KeyDown(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            txtSoHoKhau.Focus();
        //            txtSoHoKhau.SelectAll();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void txtSoHoKhau_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtHoTenBo.Focus();
                    txtHoTenBo.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHoTenBo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtHoTenMe.Focus();
                    txtHoTenMe.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHoTenMe_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboQuanHeChuHo.Focus();
                    cboQuanHeChuHo.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboQuanHeChuHo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //if (e.KeyCode == Keys.Enter)
                //{
                //    txtSoTheBHYT.Focus();
                //    txtSoTheBHYT.SelectAll();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSoTheBHYT_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboQuanHeChuHo.Focus();
                    cboQuanHeChuHo.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboNhomMau_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboNhomMau.Properties.Buttons[1].Visible = false;
                if (cboNhomMau.EditValue != null)
                {
                    cboNhomMau.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRh_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboRh.Properties.Buttons[1].Visible = false;
                if (cboRh.EditValue != null)
                {
                    cboRh.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboQuanHeChuHo_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboQuanHeChuHo.Properties.Buttons[1].Visible = false;
                if (cboQuanHeChuHo.EditValue != null)
                {
                    cboQuanHeChuHo.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTinhKhaiSinh_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTinhKhaiSinh.EditValue = null;
                    txtTinhKhaiSinh.Text = "";
                    cboTinhKhaiSinh.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboChuongTrinh_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboChuongTrinh.EditValue = null;
                    txtChuongTrinh.Text = "";
                    cboChuongTrinh.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboTinhHienTai_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTinhHienTai.EditValue = null;
                    txtTinhHienTai.Text = "";
                    cboHuyenHienTai.EditValue = null;
                    txtHuyenHienTai.Text = "";
                    cboXaHienTai.EditValue = null;
                    txtXaHienTai.Text = "";
                    LoadDistrictsComboHT("", null, false);
                    LoadCommuneComboHT("", null, false);
                    cboTinhHienTai.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboHuyenHienTai_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboHuyenHienTai.EditValue = null;
                    txtHuyenHienTai.Text = "";
                    cboXaHienTai.EditValue = null;
                    txtXaHienTai.Text = "";
                    LoadCommuneComboHT("", null, false);
                    cboHuyenHienTai.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboXaHienTai_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboXaHienTai.EditValue = null;
                    txtXaHienTai.Text = "";
                    cboXaHienTai.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboNhomMau_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboNhomMau.EditValue = null;
                    //txtXaHienTai.Text = "";
                    cboNhomMau.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboRh_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboRh.EditValue = null;
                    //txtXaHienTai.Text = "";
                    cboRh.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboQuanHeChuHo_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboQuanHeChuHo.EditValue = null;
                    //txtXaHienTai.Text = "";
                    cboQuanHeChuHo.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboNhomMau_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    cboRh.Focus();
                    cboRh.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboRh_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    txtSoHoKhau.Focus();
                    txtSoHoKhau.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboQuanHeChuHo_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtSoTheBHYT_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = "Số thẻ BHYT không hợp lệ";
                AutoValidate = AutoValidate.EnableAllowFocusChange;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SearchPatientByFilterCombo()
        {
            try
            {
                string strDob = "";
                if (this.txtPatientDOB.Text.Length == 4)
                    strDob = "01/01/" + this.txtPatientDOB.Text;
                else if (this.txtPatientDOB.Text.Length == 8)
                {
                    strDob = this.txtPatientDOB.Text.Substring(0, 2) + "/" + this.txtPatientDOB.Text.Substring(2, 2) + "/" + this.txtPatientDOB.Text.Substring(4, 4);
                }
                else
                    strDob = this.txtPatientDOB.Text;
                this.dtDOB.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(strDob);
                this.dtDOB.Update();

                //Trường hợp chưa nhập đủ 3 thông tin: hộ tên, ngày sinh, giới tính thì bỏ qua không thưc hiện tìm kiếm
                if ((this.dtDOB.EditValue == null
                    || this.dtDOB.DateTime == DateTime.MinValue)
                    || this.cboGender1.EditValue == null
                    || String.IsNullOrEmpty(this.txtPatientName.Text.Trim()))
                {
                    return;
                }

                LogSystem.Debug("Bat dau tim kiem benh nhan theo filter.");
                string dateDob = this.dtDOB.DateTime.ToString("yyyyMMdd");
                string timeDob = "00";

                long dob = Inventec.Common.TypeConvert.Parse.ToInt64(dateDob + timeDob + "0000");
                short ismale = Convert.ToInt16(this.cboGender1.EditValue);
                this.LoadDataSearchPatient("", this.txtPatientName.Text, dob, ismale, true);
                LogSystem.Debug("Ket thuc tim kiem benh nhan theo filter.");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataSearchPatient(string maBN, string hoten, long? dob, short? isMale, bool isSearchData)
        {
            try
            {
                LogSystem.Debug("LoadDataSearchPatient => t1");
                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientAdvanceFilter hisPatientFilter = new MOS.Filter.HisPatientAdvanceFilter();
                hisPatientFilter.DOB = dob;
                hisPatientFilter.VIR_PATIENT_NAME__EXACT = hoten;
                if (!String.IsNullOrEmpty(maBN))
                {
                    hisPatientFilter.PATIENT_CODE__EXACT = string.Format("{0:0000000000}", Inventec.Common.TypeConvert.Parse.ToInt64(maBN));
                }
                hisPatientFilter.GENDER_ID = isMale;
                this.currentSearchedPatients = new BackendAdapter(param).Get<List<HisPatientSDO>>("api/HisPatient/GetSdoAdvance", ApiConsumers.MosConsumer, hisPatientFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (this.currentSearchedPatients != null && this.currentSearchedPatients.Count > 0)
                {
                    //if (this.currentSearchedPatients.Count == 1)
                    //{
                    //    this.SelectOnePatientProcess(this.currentSearchedPatients[0]);
                    //}
                    //else
                    //{
                    LogSystem.Debug("LoadDataSearchPatient => t1.1. Tim thay benh nhan cu, hien thi cua so chon benh nhan");
                    frmPatientChoice frm = new frmPatientChoice(this.currentSearchedPatients, this.SelectOnePatientProcess);
                    frm.ShowDialog();
                    //}                    
                }
                LogSystem.Debug("LoadDataSearchPatient => t2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SelectOnePatientProcess(object data)
        {
            try
            {
                if (data != null)
                {
                    var patient = (HisPatientSDO)data;
                    FillDataToControl(patient);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void LoadDobPatientToForm(HisPatientSDO patientDTO)
        {
            try
            {
                if (patientDTO.DOB != null)
                {
                    string nthnm = patientDTO.DOB.ToString();
                    DateTime dtNgSinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientDTO.DOB) ?? DateTime.MinValue;
                    dtDOB.DateTime = dtNgSinh;
                    txtPatientDOB.Text = dtNgSinh.ToString("dd/MM/yyyy");
                    int age = Inventec.Common.TypeConvert.Parse.ToInt32(nthnm.Substring(8, 2));
                    //bool isGKS = MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(dtNgSinh);
                    //ValidateForm(isGKS);
                    //CalulatePatientAge(dtNgSinh);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetFormData()
        {
            try
            {
                if (!layoutControl2.IsInitialized) return;
                layoutControl2.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControl2.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            fomatFrm.ResetText();
                            fomatFrm.Text = "";
                            fomatFrm.EditValue = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    layoutControl2.EndUpdate();
                }

                txtPatientDOB.Text = "";
                txtPatientDOB.EditValue = null;
                dtDOB.EditValue = null;

                if (this.CheDoHienThiNoiLamViecManHinhDangKyTiepDon != 1
                    && workPlaceProcessor != null
                    && ucWorkPlace != null)
                {
                    workPlaceProcessor.SetValue(ucWorkPlace, null);
                }
                else
                {
                    workPlaceProcessor.SetValue(ucWorkPlace, "");
                }

                //btnSave.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToControl(HisPatientSDO patientDto)
        {
            try
            {
                ResetFormData();
                txtCMNDRelative.Text = patientDto.RELATIVE_CMND_NUMBER;
                txtPatientName.Text = patientDto.VIR_PATIENT_NAME;
                if (patientDto.DOB > 0)
                {
                    if (patientDto.IS_HAS_NOT_DAY_DOB == 1)
                    {
                        txtPatientDOB.Text = patientDto.DOB.ToString().Substring(0, 4);
                        DateTime dtNgSinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientDto.DOB) ?? DateTime.MinValue;
                        dtDOB.DateTime = dtNgSinh;
                    }
                    else
                    {
                        LoadDobPatientToForm(patientDto);
                    }
                    //dtDOB.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientDto.DOB);
                }
                var gender = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().FirstOrDefault(o => o.ID == patientDto.GENDER_ID);
                if (gender != null)
                {
                    cboGender1.EditValue = gender.ID;
                    txtGender.Text = gender.GENDER_CODE;
                }
                txtAddress.Text = patientDto.ADDRESS;
                var national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().FirstOrDefault(o => o.NATIONAL_NAME == patientDto.NATIONAL_NAME);
                if (national != null)
                {
                    cboNation.EditValue = national.NATIONAL_CODE;
                    txtNation.Text = national.NATIONAL_CODE;
                }
                var ethnic = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>().FirstOrDefault(o => o.ETHNIC_NAME == patientDto.ETHNIC_NAME);
                if (ethnic != null)
                {
                    cboEthnic.EditValue = ethnic.ETHNIC_CODE;
                    txtEthnic.Text = ethnic.ETHNIC_CODE;
                }


                var career = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().FirstOrDefault(o => o.ID == patientDto.CAREER_ID);
                if (career != null)
                {
                    cboCareer.EditValue = patientDto.CAREER_ID;
                    txtCareer.Text = career.CAREER_CODE;
                }

                var province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_NAME == patientDto.PROVINCE_NAME);
                if (province != null)
                {
                    cboProvince.EditValue = province.PROVINCE_CODE;
                    txtProvince.Text = province.PROVINCE_CODE;
                    LoadDistrictsCombo("", province.PROVINCE_CODE, false);
                }
                var district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().FirstOrDefault(o => (o.INITIAL_NAME + " " + o.DISTRICT_NAME) == patientDto.DISTRICT_NAME && o.PROVINCE_NAME == patientDto.PROVINCE_NAME);
                if (district != null)
                {
                    cboDistricts.EditValue = district.DISTRICT_CODE;
                    txtDistricts.Text = district.DISTRICT_CODE;
                    LoadCommuneCombo("", district.DISTRICT_CODE, false);
                }
                var commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().FirstOrDefault(o => (o.INITIAL_NAME + " " + o.COMMUNE_NAME) == patientDto.COMMUNE_NAME && (o.DISTRICT_INITIAL_NAME + " " + o.DISTRICT_NAME) == patientDto.DISTRICT_NAME);
                if (commune != null)
                {
                    cboCommune.EditValue = commune.COMMUNE_CODE;
                    txtCommune.Text = commune.COMMUNE_CODE;
                }

                var militaryRank = BackendDataWorker.Get<HIS_MILITARY_RANK>().FirstOrDefault(o => o.ID == patientDto.MILITARY_RANK_ID);
                if (militaryRank != null)
                {
                    txtMilitaryRankCode.Text = militaryRank.MILITARY_RANK_CODE;
                    cboMilitaryRank.EditValue = militaryRank.ID;
                }
                txtPhone.Text = patientDto.PHONE;
                txtRelation.Text = patientDto.RELATIVE_TYPE;
                txtPersonFamily.Text = patientDto.RELATIVE_NAME;
                txtContact.Text = patientDto.RELATIVE_ADDRESS;
                txtEmail.Text = patientDto.EMAIL;
                chkBNManTinh.Checked = (patientDto.IS_CHRONIC == 1 ? true : false);

                if (this.CheDoHienThiNoiLamViecManHinhDangKyTiepDon != 1
                    && workPlaceProcessor != null
                    && ucWorkPlace != null)
                {
                    if (patientDto.WORK_PLACE_ID > 0)
                    {
                        var workPlace = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_WORK_PLACE>().FirstOrDefault(o => o.ID == patientDto.WORK_PLACE_ID);
                        if (workPlace != null)
                            workPlaceProcessor.SetValue(ucWorkPlace, workPlace);
                        else
                            workPlaceProcessor.SetValue(ucWorkPlace, null);
                    }
                }
                else
                {
                    workPlaceProcessor.SetValue(ucWorkPlace, patientDto.WORK_PLACE);
                }

                txtTinhKhaiSinh.Text = patientDto.BORN_PROVINCE_CODE;
                cboTinhKhaiSinh.EditValue = patientDto.BORN_PROVINCE_CODE;

                if (BackendDataWorker.Get<HIS_PROGRAM>() != null)
                {
                    var program = BackendDataWorker.Get<HIS_PROGRAM>().FirstOrDefault(o => o.ID == patientDto.ProgramId);
                    if (program != null)
                    {
                        txtChuongTrinh.Text = program.PROGRAM_CODE;
                        cboChuongTrinh.EditValue = program.ID;
                    }
                }

                var provineHt = BackendDataWorker.Get<V_SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_NAME == patientDto.HT_PROVINCE_NAME);
                if (provineHt != null)
                {
                    txtTinhHienTai.Text = provineHt.PROVINCE_CODE;
                    cboTinhHienTai.EditValue = provineHt.PROVINCE_CODE;
                }

                var districtHt = BackendDataWorker.Get<V_SDA_DISTRICT>().FirstOrDefault(o => o.PROVINCE_NAME == patientDto.HT_PROVINCE_NAME && o.DISTRICT_NAME == patientDto.HT_DISTRICT_NAME);
                if (districtHt != null)
                {
                    txtHuyenHienTai.Text = districtHt.DISTRICT_CODE;
                    cboHuyenHienTai.EditValue = districtHt.DISTRICT_CODE;
                }

                var communeHt = BackendDataWorker.Get<V_SDA_COMMUNE>().FirstOrDefault(o => o.DISTRICT_NAME == patientDto.HT_DISTRICT_NAME && o.COMMUNE_NAME == patientDto.HT_COMMUNE_NAME);
                if (communeHt != null)
                {
                    txtXaHienTai.Text = communeHt.COMMUNE_CODE;
                    cboXaHienTai.EditValue = communeHt.COMMUNE_CODE;
                }

                txtDiaChiHienTai.Text = patientDto.HT_ADDRESS;
                if (!string.IsNullOrEmpty(patientDto.CMND_NUMBER))
                {
                    txtCMND.Text = patientDto.CMND_NUMBER;
                    dtNgayCap.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientDto.CMND_DATE ?? 0) ?? new DateTime();
                    txtNoiCap.Text = patientDto.CMND_PLACE;
                }
                else if (!string.IsNullOrEmpty(patientDto.CCCD_NUMBER))
                {
                    txtCMND.Text = patientDto.CCCD_NUMBER;
                    dtNgayCap.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientDto.CCCD_DATE ?? 0) ?? new DateTime();
                    txtNoiCap.Text = patientDto.CCCD_PLACE;
                }
                else
                {

                }

                cboRh.EditValue = patientDto.BLOOD_RH_CODE;
                cboNhomMau.EditValue = patientDto.BLOOD_ABO_CODE;
                txtSoHoKhau.Text = patientDto.HOUSEHOLD_CODE;
                txtHoTenBo.Text = patientDto.FATHER_NAME;
                txtHoTenMe.Text = patientDto.MOTHER_NAME;
                cboQuanHeChuHo.EditValue = patientDto.HOUSEHOLD_RELATION_NAME;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                ResetFormData();
                btnSave.Enabled = true;
                txtPatientName.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtXaHienTai_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                //if (e.KeyCode == Keys.Enter)
                //{
                //    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                //    string districtCode = "";
                //    if (cboHuyenHienTai.EditValue != null)
                //    {
                //        districtCode = cboHuyenHienTai.EditValue.ToString();
                //        LoadCommuneComboHT(strValue.ToUpper(), districtCode, true);
                //    }
                //    else
                //    {
                //        DevExpress.XtraEditors.XtraMessageBox.Show("Không có thông tin huyện hiện tại", "Thông báo");
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        

        

        //private void ResetFormData1()
        //{
        //    try
        //    {
        //        if (!layoutControl2.IsInitialized) return;
        //        layoutControl2.BeginUpdate();
        //        try
        //        {
        //            foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControl2.Items)
        //            {
        //                DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
        //                if (lci != null && lci.Control != null && lci.Control is BaseEdit)
        //                {
        //                    DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
        //                    fomatFrm.ResetText();
        //                    fomatFrm.EditValue = null;
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Inventec.Common.Logging.LogSystem.Warn(ex);
        //        }
        //        finally
        //        {
        //            layoutControl2.EndUpdate();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
    }
}

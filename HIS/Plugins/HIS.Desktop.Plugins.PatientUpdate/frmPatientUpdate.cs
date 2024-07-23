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
using MOS.Filter;
using EMR.SDO;
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
using HIS.Desktop.Plugins.PatientUpdate;
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
using HIS.Desktop.Plugins.PatientUpdate.Resources;
using HIS.Desktop.Common;
using SDA.EFMODEL.DataModels;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using System.IO;
using Inventec.Common.Address;

namespace HIS.Desktop.Plugins.PatientUpdate
{
    public partial class frmPatientUpdate : HIS.Desktop.Utility.FormBase
    {
        #region Declaration
        int demAvatar = 0;
        int demBHYT = 0;
        bool isGKS = false;
        bool isTxtPatientDobPreviewKeyDown = false;
        internal bool isNotPatientDayDob = false;
        internal int ActionType = 0;// No action    
        bool activeDCFirtTime = false;
        bool activeDCHTFirtTime = false;
        //V_HIS_TREATMENT currentTreatment;
        V_HIS_PATIENT currentPatient;
        HisPatientUpdateSDO patientUpdateSdo;
        internal MOS.EFMODEL.DataModels.HIS_SERVICE_REQ EVHisServiceReqDTO = null;
        internal MOS.EFMODEL.DataModels.V_HIS_PATIENT currentVHisPatientDTO = null;
        internal MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER currentHisPatientTypeAlter = new MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER();
        internal HisServiceReqUpdateSDO ServiceReqUpdateSDO { get; set; }
        internal int PatientRowCount = 0;
        int positionHandleControlPatientInfo = -1;
        HisCardSDO hisCard;
        List<HIS_CARD> listCard;
        DelegateSelectData refeshReference;
        internal HIS.UC.WorkPlace.UCWorkPlaceCombo workPlacecbo;
        internal HIS.UC.WorkPlace.WorkPlaceProcessor workPlaceProcessor;
        internal HIS.UC.WorkPlace.WorkPlaceProcessor.Template workPlaceTemplate;
        UserControl ucWorkPlace;
        long CheDoHienThiNoiLamViecManHinhDangKyTiepDon;
        byte[] byteImgChanDungs;
        byte[] byteImgTheBHYTs;
        internal bool isDobTextEditKeyEnter;
        //bool validate;
        Inventec.Desktop.Common.Modules.Module currentModule;
        long? TreatmentId = null;
        long PatientId = 0;
        #endregion

        #region Load

        public frmPatientUpdate(Inventec.Desktop.Common.Modules.Module _Module, V_HIS_PATIENT _currentPatient, DelegateSelectData _refeshReference)
            : base(_Module)
        {
            try
            {
                InitializeComponent();
                this.CheDoHienThiNoiLamViecManHinhDangKyTiepDon = ConfigApplicationWorker.Get<long>("CONFIG_KEY__HIEN_THI_NOI_LAM_VIEC_THEO_DINH_DANG_MAN_HINH_DANG_KY");
                this.currentPatient = _currentPatient;
                this.PatientId = _currentPatient.ID;
                this.refeshReference = _refeshReference;
                this.currentModule = _Module;
                InitWorkPlaceControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmPatientUpdate(Inventec.Desktop.Common.Modules.Module _Module, long patientId, long treatmentId, DelegateSelectData _refeshReference)
            : base(_Module)
        {
            try
            {
                InitializeComponent();
                this.CheDoHienThiNoiLamViecManHinhDangKyTiepDon = ConfigApplicationWorker.Get<long>("CONFIG_KEY__HIEN_THI_NOI_LAM_VIEC_THEO_DINH_DANG_MAN_HINH_DANG_KY");
                this.TreatmentId = treatmentId;
                this.PatientId = patientId;
                this.refeshReference = _refeshReference;
                this.currentModule = _Module;
                InitWorkPlaceControl();
                //LoadDefaultWhenOpenFromHSDT();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void getPatientFromTreatment(long patientId)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientViewFilter hisPatientFilter = new MOS.Filter.HisPatientViewFilter();
                hisPatientFilter.ID = patientId;
                currentVHisPatientDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, hisPatientFilter, param).SingleOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmPatientUpdate_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetDefaultData();
                SetDefaultControlsProperties();
                SetCaptionByLanguageKey();
                FillDataToControlsForm();
                SetIcon();

                if (this.TreatmentId != null)
                {
                    chkUpdateNew.Enabled = false;
                    chkUpdateNew.Checked = false;
                    chkEmrUpdate.Enabled = true;
                    getPatientFromTreatment(this.PatientId);


                }

                if (this.currentPatient != null)
                {
                    chkUpdateNew.Enabled = true;
                    chkUpdateNew.Checked = true;
                    if (chkUpdateNew.Checked == true)
                    {
                        chkEmrUpdate.Enabled = true;
                    }
                    else
                    {
                        chkEmrUpdate.Enabled = false;
                        chkEmrUpdate.Checked = false;
                    }
                    this.currentVHisPatientDTO = this.currentPatient;
                }
                FillDataPatientToControl(this.currentVHisPatientDTO);
                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT.MUST_HAVE_NCS_INFO_FOR_CHILD") == "1" && MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(dtPatientDob.DateTime))
                {
                    isGKS = true;

                }
                ValidateForm(isGKS);

                //#2173
                SetEditInfo();

                LoadConfigHisAcc();
                ValidationClassify();
                ValidationBHXH(txtBhxhFather, 10, 10);
                ValidationBHXH(txtBhxhMother, 10, 10);
                ValidationProvince();
                ValidateDistricts();
                ValidateCommune();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateDistricts()
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validate = new GridLookupEditWithTextEditValidationRule();
                validate.cbo = this.cboDistricts;
                validate.txtTextEdit = this.txtDistricts;
                validate.ErrorText = "Trường dữ liệu bắt buộc";
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(this.txtDistricts, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateCommune()
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validate = new GridLookupEditWithTextEditValidationRule();
                validate.cbo = this.cboCommune;
                validate.txtTextEdit = this.txtCommune;
                validate.ErrorText = "Trường dữ liệu bắt buộc";
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(this.txtCommune, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationProvince()
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validate = new GridLookupEditWithTextEditValidationRule();
                validate.cbo = this.cboProvince;
                validate.txtTextEdit = this.txtProvince;
                validate.ErrorText = "Trường dữ liệu bắt buộc";
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(this.txtProvince, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationClassify()
        {
            try
            {
                if (Config.IsPatientClassify)
                {
                    Valid_GenderCode_Control valid = new Valid_GenderCode_Control();
                    valid.cboGenderCode = cboClassify;
                    valid.ErrorText = "Trường dữ liệu bắt buộc";
                    valid.ErrorType = ErrorType.Warning;
                    dxValidationProvider1.SetValidationRule(cboClassify, valid);
                    layoutControlItem1.AppearanceItemCaption.ForeColor = Color.Maroon;

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidationBHXH(BaseControl control, int? maxLength, int minLength)
        {
            try
            {
                ValidateBHXHCode valid = new ValidateBHXHCode();
                valid.txtControl = control;
                valid.maxLength = maxLength;
                valid.minLength = minLength;
                dxValidationProvider1.SetValidationRule(control, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetDefaultControlsProperties()
        {
            try
            {
                emptySpaceItemBNManTinh.Width = lcFatherName.Width - lcBNManTinh.Width;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetEditInfo()
        {
            try
            {
                V_HIS_TREATMENT _Treatment = new V_HIS_TREATMENT();
                if (this.currentPatient != null)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTreatmentViewFilter treatmentFilter = new MOS.Filter.HisTreatmentViewFilter();
                    treatmentFilter.PATIENT_ID = this.currentPatient.ID;
                    treatmentFilter.ORDER_FIELD = "CREATE_TIME";
                    treatmentFilter.ORDER_DIRECTION = "DESC";
                    _Treatment = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, treatmentFilter, param).SingleOrDefault();
                }
                if (_Treatment != null && _Treatment.IS_PAUSE == 1)
                {
                    ReadOnly(true);
                }
                else
                {
                    ReadOnly(false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReadOnly(bool isKey)
        {
            try
            {
                txtPatientName.ReadOnly = isKey;
                txtGender.ReadOnly = isKey;
                cboGender1.ReadOnly = isKey;
                txtPatientDob.ReadOnly = isKey;
                txtNation.ReadOnly = isKey;
                cboNation.ReadOnly = isKey;
                txtAddress.ReadOnly = isKey;

                txtProvince.ReadOnly = isKey;
                cboProvince.ReadOnly = isKey;
                txtDistricts.ReadOnly = isKey;
                cboDistricts.ReadOnly = isKey;
                txtCommune.ReadOnly = isKey;
                cboCommune.ReadOnly = isKey;
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.PatientUpdate.Resources.Lang", typeof(HIS.Desktop.Plugins.PatientUpdate.frmPatientUpdate).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.Text = (currentModule != null ? currentModule.text : "");
                this.IciCardCode.Text = SetKey("frmPatientUpdate.IciCardCode.Text");
                this.lcPatientCode.Text = SetKey("frmPatientUpdate.lcPatientCode.Text");
                this.lciPatientName.Text = SetKey("frmPatientUpdate.lciPatientName.Text");
                this.lciGender.Text = SetKey("frmPatientUpdate.lciGender.Text");
                this.lciDOB.Text = SetKey("frmPatientUpdate.lciDOB.Text");
                this.lciNation.Text = SetKey("frmPatientUpdate.lciNation.Text");
                this.lciProvince.Text = SetKey("frmPatientUpdate.lciProvince.Text");
                this.lcProvince.Text = SetKey("frmPatientUpdate.lcProvince.Text");
                this.lcProvince.OptionsToolTip.ToolTip = SetKey("frmPatientUpdate.lcProvince.OptionsToolTip.ToolTip");
                this.lcCCCD_CMTNumber.Text = SetKey("frmPatientUpdate.lcCCCD_CMTNumber.Text");
                this.lcCCCD_CMTNumber.OptionsToolTip.ToolTip = SetKey("frmPatientUpdate.OptionsToolTip.ToolTip");
                this.lciPhone.Text = SetKey("frmPatientUpdate.lciPhone.Text");
                this.lcTheBHYT.Text = SetKey("frmPatientUpdate.lcTheBHYT.Text");
                this.lcPatientStoreCode.Text = SetKey("frmPatientUpdate.lcPatientStoreCode.Text");
                this.groupBox1.Text = SetKey("frmPatientUpdate.groupBox1.Text");
                this.lciDistricts.Text = SetKey("frmPatientUpdate.lciDistricts.Text");
                this.lcHTDistrict.Text = SetKey("frmPatientUpdate.lcHTDistrict.Text");
                this.lcHTDistrict.OptionsToolTip.ToolTip = SetKey("frmPatientUpdate.lcHTDistrict.OptionsToolTip.ToolTip");
                this.lcCCCD_CMTPlace.Text = SetKey("frmPatientUpdate.lcCCCD_CMTPlace.Text");
                this.lcEmail.Text = SetKey("frmPatientUpdate.lcEmail.Text");
                this.lcAccountNumber.Text = SetKey("frmPatientUpdate.lcAccountNumber.Text");
                this.lcAccountNumber.OptionsToolTip.ToolTip = SetKey("frmPatientUpdate.lcAccountNumber.OptionsToolTip.ToolTip");
                //this.lcUUID.Text = SetKey("frmPatientUpdate.lcUUID.Text");
                this.lciCommune.Text = SetKey("frmPatientUpdate.lciCommune.Text");
                this.lcHTCommune.Text = SetKey("frmPatientUpdate.lcHTCommune.Text");
                this.lcHTCommune.OptionsToolTip.ToolTip = SetKey("frmPatientUpdate.lcHTCommune.OptionsToolTip.ToolTip");
                this.lcCCCD_CMTDate.Text = SetKey("frmPatientUpdate.lcCCCD_CMTDate.Text");
                this.lciCareer.Text = SetKey("frmPatientUpdate.lciCareer.Text");
                this.lcTaxCode.Text = SetKey("frmPatientUpdate.lcTaxCode.Text");
                this.lcTaxCode.OptionsToolTip.ToolTip = SetKey("frmPatientUpdate.lcTaxCode.OptionsToolTip.ToolTip");
                this.lciAdress.Text = SetKey("frmPatientUpdate.lciAdress.Text");
                this.lcHtAddress.Text = SetKey("frmPatientUpdate.lcHtAddress.Text");
                this.lcHtAddress.OptionsToolTip.ToolTip = SetKey("frmPatientUpdate.lcHtAddress.OptionsToolTip.ToolTip");
                this.lciEthnic.Text = SetKey("frmPatientUpdate.lciEthnic.Text");
                this.lcBloodABOCode.Text = SetKey("frmPatientUpdate.lcBloodABOCode.Text");
                this.lcTonGiao.Text = SetKey("frmPatientUpdate.lcTonGiao.Text");
                this.lciMilitaryRank.Text = SetKey("frmPatientUpdate.lciMilitaryRank.Text");
                this.lcBloodRHCode.Text = SetKey("frmPatientUpdate.lcBloodRHCode.Text");
                this.groupBox2.Text = SetKey("frmPatientUpdate.groupBox2.Text");
                this.lcFatherName.Text = SetKey("frmPatientUpdate.lcFatherName.Text");
                this.lciFatherCareer.Text = SetKey("frmPatientUpdate.lciFatherCareer.Text");
                this.lciFatherEducationalLevel.Text = SetKey("frmPatientUpdate.lciFatherEducationalLevel.Text");
                this.lciFatherEducationalLevel.OptionsToolTip.ToolTip = SetKey("frmPatientUpdate.lciFatherEducationalLevel.ToolTip");
                this.lciMotherCareer.Text = SetKey("frmPatientUpdate.lciMotherCareer.Text");
                this.lciMotherEducationalLevel.Text = SetKey("frmPatientUpdate.lciMotherEducationalLevel.Text");
                this.lciMotherEducationalLevel.OptionsToolTip.ToolTip = SetKey("frmPatientUpdate.lciMotherEducationalLevel.ToolTip");
                this.lcRelativePhone.Text = SetKey("frmPatientUpdate.lcRelativePhone.Text");
                this.lcMotherName.Text = SetKey("frmPatientUpdate.lcMotherName.Text");
                this.lcRelativeMobile.Text = SetKey("frmPatientUpdate.lcRelativeMobile.Text");
                this.lcPersonFamily.Text = SetKey("frmPatientUpdate.lcPersonFamily.Text");
                this.lctContact.Text = SetKey("frmPatientUpdate.lctContact.Text");
                this.lciRelation.Text = SetKey("frmPatientUpdate.lciRelation.Text");
                this.lciCMNDRelative.Text = SetKey("frmPatientUpdate.lciCMNDRelative.Text");
                this.groupBox3.Text = SetKey("frmPatientUpdate.groupBox3.Text");
                this.lcBNManTinh.Text = SetKey("frmPatientUpdate.lcBNManTinh.Text");
                this.lcTienSuBenh.Text = SetKey("frmPatientUpdate.lcTienSuBenh.Text");
                this.lcTienSuGiaDinh.Text = SetKey("frmPatientUpdate.lcTienSuGiaDinh.Text");
                this.chkinTemBarcode.Text = SetKey("frmPatientUpdate.chkinTemBarcode.Text");
                this.chkInPhieuYCKham.Text = SetKey("frmPatientUpdate.chkInPhieuYCKham.Text");
                this.layoutControlItem61.Text = SetKey("frmPatientUpdate.layoutControlItem61.Text");
                this.lciEmrUpdate.Text = SetKey("frmPatientUpdate.lciEmrUpdate.Text");
                this.lciChucVu.Text = SetKey("frmPatientUpdate.lciChucVu.Text");
                this.btnUploadChanDung.Text = SetKey("frmPatientUpdate.btnUploadChanDung.Text");
                this.btnUploadTheBHYT.Text = SetKey("frmPatientUpdate.btnUploadTheBHYT.Text");
                this.btnSave.Text = SetKey("frmPatientUpdate.btnSave.Text");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string SetKey(string key)
        {
            string result = "";
            try
            {
                result = Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                result = "";
                LogSystem.Error(ex);
            }
            return result;
        }

        private void SetDefaultData()
        {
            try
            {
                ActionType = GlobalVariables.ActionEdit;
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
                if (!string.IsNullOrEmpty(patientDto.AVATAR_URL))
                {
                    Inventec.Common.Logging.LogSystem.Warn("AVATAR_URL: " + patientDto.AVATAR_URL);

                    try
                    {
                        MemoryStream ms = Inventec.Fss.Client.FileDownload.GetFile(patientDto.AVATAR_URL);
                        if (ms != null)
                        {
                            var img = Image.FromStream(ms);
                            SetImageDefaultForPictureEdit(pictureBox1, img);
                        }
                    }
                    catch (Exception ex)
                    {
                        pictureBox1.Tag = "NoImage";
                        Inventec.Common.Logging.LogSystem.Error("Loi convert va luu tam file pdf tu server fss ve may tram");
                    }
                }
                else
                {
                    pictureBox1.Tag = "NoImage";
                }

                if (!string.IsNullOrEmpty(patientDto.BHYT_URL))
                {
                    Inventec.Common.Logging.LogSystem.Warn("BHYT_URL: " + patientDto.BHYT_URL);
                    try
                    {
                        MemoryStream ms = Inventec.Fss.Client.FileDownload.GetFile(patientDto.BHYT_URL);
                        if (ms != null)
                        {
                            var img = Image.FromStream(ms);
                            SetImageDefaultForPictureEdit(pictureBox2, img);
                        }
                    }
                    catch (Exception ex)
                    {
                        pictureBox2.Tag = "NoImage";
                        Inventec.Common.Logging.LogSystem.Error("Loi convert va luu tam file pdf tu server fss ve may tram");
                    }
                }
                else
                {
                    pictureBox2.Tag = "NoImage";
                }
                txtTaxCode.Text = patientDto.TAX_CODE;
                txtPatientStoreCode.Text = patientDto.PATIENT_STORE_CODE;
                txtCMNDRelative.Text = patientDto.RELATIVE_CMND_NUMBER;
                txtPatientName.Text = patientDto.VIR_PATIENT_NAME;
                if (patientDto.DOB > 0)
                {
                    if (patientDto.IS_HAS_NOT_DAY_DOB == 1)
                    {
                        dtPatientDob.Text = patientDto.DOB.ToString().Substring(0, 4);
                        txtPatientDob.Text = patientDto.DOB.ToString().Substring(0, 4);
                        DateTime dtNgSinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientDto.DOB) ?? DateTime.MinValue;
                        dtPatientDob.DateTime = dtNgSinh;
                        this.isNotPatientDayDob = true;
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
                var career = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.ID == patientDto.CAREER_ID);
                if (career != null)
                {
                    cboCareer.EditValue = patientDto.CAREER_ID;
                    txtCareer.Text = career.CAREER_CODE;
                }

                //var province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_NAME == patientDto.PROVINCE_NAME);
                var province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().FirstOrDefault(o => o.PROVINCE_CODE == patientDto.PROVINCE_CODE);
                if (province != null)
                {
                    cboProvince.EditValue = province.PROVINCE_CODE;
                    txtProvince.Text = province.PROVINCE_CODE;
                    LoadDistrictsCombo("", province.PROVINCE_CODE, false);
                }
                V_SDA_PROVINCE provice = new V_SDA_PROVINCE();
                if (!String.IsNullOrEmpty(patientDto.HT_PROVINCE_NAME))
                {

                    var provinceHT = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().FirstOrDefault(o => o.PROVINCE_NAME.ToUpper().Trim() == patientDto.HT_PROVINCE_NAME.ToUpper().Trim());
                    if (provinceHT != null)
                    {
                        provice = provinceHT;
                        cboHTProvinceName.EditValue = provinceHT.PROVINCE_CODE;
                        txtHTProvinceCode.Text = provinceHT.PROVINCE_CODE;
                        LoadHTDistrictsCombo("", provinceHT.PROVINCE_CODE, false);
                    }
                }
                //var district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().FirstOrDefault(o => (o.INITIAL_NAME + " " + o.DISTRICT_NAME) == patientDto.DISTRICT_NAME && o.PROVINCE_NAME == patientDto.PROVINCE_NAME);

                var district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().FirstOrDefault(o => o.DISTRICT_CODE == patientDto.DISTRICT_CODE);
                if (district != null)
                {
                    cboDistricts.EditValue = district.DISTRICT_CODE;
                    txtDistricts.Text = district.DISTRICT_CODE;
                    LoadCommuneCombo("", district.DISTRICT_CODE, false);
                }
                V_SDA_DISTRICT districts = new V_SDA_DISTRICT();
                if (!String.IsNullOrEmpty(patientDto.HT_DISTRICT_NAME))
                {

                    var districtHT = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().FirstOrDefault(o => o.DISTRICT_NAME.ToUpper().Trim() == patientDto.HT_DISTRICT_NAME.ToUpper().Trim() && o.PROVINCE_ID == provice.ID);
                    if (districtHT != null)
                    {
                        districts = districtHT;
                        cboHTDistrictName.EditValue = districtHT.DISTRICT_CODE;
                        txtHTDistrictCode.Text = districtHT.DISTRICT_CODE;
                        LoadHTCommuneCombo("", districtHT.DISTRICT_CODE, false);
                    }
                }
                var commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().FirstOrDefault(o => o.COMMUNE_CODE == patientDto.COMMUNE_CODE);

                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("commune____", commune));

                if (commune != null)
                {
                    cboCommune.EditValue = commune.COMMUNE_CODE;
                    txtCommune.Text = commune.COMMUNE_CODE;
                }
                if (!String.IsNullOrEmpty(patientDto.HT_COMMUNE_NAME))
                {
                    var communeHT = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().FirstOrDefault(o => o.COMMUNE_NAME.ToUpper().Trim() == patientDto.HT_COMMUNE_NAME.ToUpper().Trim() && o.DISTRICT_NAME.ToUpper().Trim() == patientDto.HT_DISTRICT_NAME.ToUpper().Trim());
                    if (communeHT != null)
                    {
                        cboHTCommuneName.EditValue = communeHT.COMMUNE_CODE;
                        txtHTCommuneCode.Text = communeHT.COMMUNE_CODE;
                    }
                }
                Inventec.Common.Logging.LogSystem.Info("Ma xax" + txtCommune.Text);
                cboMilitaryRank.EditValue = patientDto.MILITARY_RANK_ID;
                txtPhone.Text = patientDto.PHONE;
                txtRelation.Text = patientDto.RELATIVE_TYPE;
                txtContact.Text = patientDto.RELATIVE_ADDRESS;
                txtEmail.Text = patientDto.EMAIL;
                txtTienSuBenh.Text = patientDto.PT_PATHOLOGICAL_HISTORY;
                txtTienSuGiaDinh.Text = patientDto.PT_PATHOLOGICAL_HISTORY_FAMILY;
                txtTheBHYT.Text = patientDto.UUID_BHYT_NUMBER;
                txtPersonFamily.Text = patientDto.RELATIVE_NAME;
                cboClassify.EditValue = patientDto.PATIENT_CLASSIFY_ID;
                cboPosition.EditValue = patientDto.POSITION_ID;

                chkBNManTinh.Checked = (patientDto.IS_CHRONIC == 1 ? true : false);
                chkIsTuberculosis.Checked = (patientDto.IS_TUBERCULOSIS == 1 ? true : false);
                chkIsHiv.Checked = (patientDto.IS_HIV == 1 ? true : false);


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
                txtOtherAddress.Text = patientDto.WORK_PLACE;
                txtPatientCode.Text = patientDto.PATIENT_CODE;
                txtAccountNumber.Text = patientDto.ACCOUNT_NUMBER;
                List<HIS_TEST_INDEX> lstIndexRh = null;
                if (!string.IsNullOrEmpty(patientDto.BLOOD_RH_CODE))
                    cboBloodRHCode.EditValue = patientDto.BLOOD_RH_CODE;
                else
                {
                    lstIndexRh = BackendDataWorker.Get<HIS_TEST_INDEX>().Where(o => o.IS_BLOOD_RH == 1 && o.RESULT_BLOOD_RH_PLUS != null && o.RESULT_BLOOD_RH_MINUS != null).ToList();
                }

                List<HIS_TEST_INDEX> lstIndexAbo = null;
                if (!string.IsNullOrEmpty(patientDto.BLOOD_ABO_CODE))
                    cboBloodABOCode.EditValue = patientDto.BLOOD_ABO_CODE;
                else
                {
                    lstIndexAbo = BackendDataWorker.Get<HIS_TEST_INDEX>().Where(o => o.IS_BLOOD_ABO == 1 && o.RESULT_BLOOD_A != null && o.RESULT_BLOOD_B != null && o.RESULT_BLOOD_AB != null && o.RESULT_BLOOD_O != null).ToList();

                }
                if ((lstIndexAbo != null && lstIndexAbo.Count > 0) || (lstIndexRh != null && lstIndexRh.Count > 0))
                {
                    CommonParam param = new CommonParam();
                    HisSereServTeinFilter filter = new HisSereServTeinFilter();
                    filter.TDL_TREATMENT_ID = TreatmentId;
                    List<long> lst = new List<long>();
                    if (lstIndexAbo != null && lstIndexAbo.Count > 0)
                        lst.AddRange(lstIndexAbo.Select(o => o.ID).ToList());
                    if (lstIndexRh != null && lstIndexRh.Count > 0)
                    {
                        lst.AddRange(lstIndexRh.Select(o => o.ID).ToList());
                    }
                    filter.TEST_INDEX_IDs = lst.Distinct().ToList();
                    var SereServTeinData = new BackendAdapter(param).Get<List<HIS_SERE_SERV_TEIN>>("/api/HisSereServTein/Get", ApiConsumers.MosConsumer, filter, param);
                    if (SereServTeinData != null && SereServTeinData.Count > 0)
                    {
                        var teinValue = SereServTeinData.Where(o => !string.IsNullOrEmpty(o.VALUE)).ToList();

                        if (teinValue != null && teinValue.Count > 0)
                        {
                            if (lstIndexAbo != null && lstIndexAbo.Count > 0)
                            {
                                var teinAbo = teinValue.Where(o => lstIndexAbo.Exists(p => p.ID == o.TEST_INDEX_ID));
                                if (teinAbo != null && teinAbo.Count() > 0)
                                {
                                    var tein = teinAbo.OrderByDescending(o => o.MODIFY_TIME).ThenByDescending(o => o.ID).First();
                                    var bloodABO = BackendDataWorker.Get<HIS_BLOOD_ABO>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                                    var indexByTeinA = lstIndexAbo.FirstOrDefault(o => o.ID == tein.TEST_INDEX_ID && ("," + o.RESULT_BLOOD_A + ",").Contains("," + tein.VALUE + ","));
                                    if (indexByTeinA != null)
                                        cboBloodABOCode.EditValue = bloodABO.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_BLOOD_ABO.ID__A).BLOOD_ABO_CODE;
                                    var indexByTeinB = lstIndexAbo.FirstOrDefault(o => o.ID == tein.TEST_INDEX_ID && ("," + o.RESULT_BLOOD_B + ",").Contains("," + tein.VALUE + ","));
                                    if (indexByTeinB != null)
                                        cboBloodABOCode.EditValue = bloodABO.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_BLOOD_ABO.ID__B).BLOOD_ABO_CODE;
                                    var indexByTeinAB = lstIndexAbo.FirstOrDefault(o => o.ID == tein.TEST_INDEX_ID && ("," + o.RESULT_BLOOD_AB + ",").Contains("," + tein.VALUE + ","));
                                    if (indexByTeinAB != null)
                                        cboBloodABOCode.EditValue = bloodABO.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_BLOOD_ABO.ID__AB).BLOOD_ABO_CODE;
                                    var indexByTeinO = lstIndexAbo.FirstOrDefault(o => o.ID == tein.TEST_INDEX_ID && ("," + o.RESULT_BLOOD_O + ",").Contains("," + tein.VALUE + ","));
                                    if (indexByTeinO != null)
                                        cboBloodABOCode.EditValue = bloodABO.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_BLOOD_ABO.ID__O).BLOOD_ABO_CODE;
                                }
                            }

                            if (lstIndexRh != null && lstIndexRh.Count > 0)
                            {

                                var teinRh = teinValue.Where(o => lstIndexRh.Exists(p => p.ID == o.TEST_INDEX_ID));
                                if (teinRh != null && teinRh.Count() > 0)
                                {
                                    var tein = teinRh.OrderByDescending(o => o.MODIFY_TIME).ThenByDescending(o => o.ID).First();
                                    var bloodRH = BackendDataWorker.Get<HIS_BLOOD_RH>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                                    var indexByTeinPlus = lstIndexRh.FirstOrDefault(o => o.ID == tein.TEST_INDEX_ID && ("," + o.RESULT_BLOOD_RH_PLUS + ",").Contains("," + tein.VALUE + ","));
                                    if (indexByTeinPlus != null)
                                        cboBloodRHCode.EditValue = bloodRH.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_BLOOD_RH.ID__CONG).BLOOD_RH_CODE;
                                    var indexByTeinMinus = lstIndexRh.FirstOrDefault(o => o.ID == tein.TEST_INDEX_ID && ("," + o.RESULT_BLOOD_RH_MINUS + ",").Contains("," + tein.VALUE + ","));
                                    if (indexByTeinMinus != null)
                                        cboBloodRHCode.EditValue = bloodRH.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_BLOOD_RH.ID__TRU).BLOOD_RH_CODE;
                                }
                            }
                        }
                    }
                }
                txtHTAddress.Text = patientDto.HT_ADDRESS;
                txtFatherName.Text = patientDto.FATHER_NAME;
                txtMotherCareer.Text = patientDto.MOTHER_CAREER;
                txtMotherEducationalLevel.Text = patientDto.MOTHER_EDUCATIIONAL_LEVEL;
                txtFatherCareer.Text = patientDto.FATHER_CAREER;
                txtBhxhFather.Text = patientDto.FATHER_SOCIAL_INSURANCE_NUMBER;
                txtBhxhMother.Text = patientDto.MOTHER_SOCIAL_INSURANCE_NUMBER;
                txtFatherEducationalLevel.Text = patientDto.FATHER_EDUCATIIONAL_LEVEL;
                txtMotherName.Text = patientDto.MOTHER_NAME;
                txtRelativeMobile.Text = patientDto.RELATIVE_MOBILE;
                txtRelativePhone.Text = patientDto.RELATIVE_PHONE;
                //txtUUID.Text = patientDto.UUID;
                txtSocialInsuranceNumber.Text = patientDto.SOCIAL_INSURANCE_NUMBER;
                txtNote.Text = patientDto.NOTE;
                if (!String.IsNullOrEmpty(patientDto.CCCD_NUMBER))
                {
                    txtCCCD_CMTNumber.Text = patientDto.CCCD_NUMBER;
                    txtCCCD_CMTPlace.Text = patientDto.CCCD_PLACE;
                    txtCCCD_CMTDate.Text = patientDto.CCCD_DATE.ToString().Substring(0, 4);
                    DateTime dtCccD = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientDto.CCCD_DATE ?? 0) ?? DateTime.MinValue;
                    txtCCCD_CMTDate.DateTime = dtCccD;
                }
                else if (!String.IsNullOrEmpty(patientDto.CMND_NUMBER))
                {
                    txtCCCD_CMTNumber.Text = patientDto.CMND_NUMBER;
                    txtCCCD_CMTPlace.Text = patientDto.CMND_PLACE;
                    txtCCCD_CMTDate.Text = patientDto.CMND_DATE.ToString().Substring(0, 4);
                    DateTime dtCMT = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientDto.CMND_DATE ?? 0) ?? DateTime.MinValue;
                    txtCCCD_CMTDate.DateTime = dtCMT;
                }
                else if (!String.IsNullOrEmpty(patientDto.PASSPORT_NUMBER))
                {
                    txtCCCD_CMTNumber.Text = patientDto.PASSPORT_NUMBER;
                    txtCCCD_CMTPlace.Text = patientDto.PASSPORT_PLACE;
                    txtCCCD_CMTDate.Text = patientDto.PASSPORT_DATE.ToString().Substring(0, 4);
                    DateTime dtCMT = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientDto.PASSPORT_DATE ?? 0) ?? DateTime.MinValue;
                    txtCCCD_CMTDate.DateTime = dtCMT;

                }
                if (!String.IsNullOrEmpty(patientDto.RELIGION_NAME))
                {
                    var itemdata = BackendDataWorker.Get<SDA_RELIGION>().FirstOrDefault(o => o.RELIGION_NAME.ToUpper().Trim() == patientDto.RELIGION_NAME.ToUpper().Trim());
                    if (itemdata != null)
                    {
                        cboTonGiao.EditValue = itemdata.ID;
                    }
                }
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
                FillDataToGridLookupedit(this.cboProvince, "PROVINCE_NAME", "PROVINCE_CODE", "SEARCH_CODE", BackendDataWorker.Get<V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                FillDataToGridLookupedit(this.cboHTProvinceName, "PROVINCE_NAME", "PROVINCE_CODE", "SEARCH_CODE", BackendDataWorker.Get<V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                FillDataToGridLookupedit(this.cboCareer, "CAREER_NAME", "ID", "CAREER_CODE", BackendDataWorker.Get<HIS_CAREER>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                FillDataToGridLookupedit(this.cboEthnic, "ETHNIC_NAME", "ETHNIC_CODE", "ETHNIC_CODE", BackendDataWorker.Get<SDA_ETHNIC>());
                FillDataToGridLookupedit(this.cboNation, "NATIONAL_NAME", "NATIONAL_CODE", "NATIONAL_CODE", BackendDataWorker.Get<SDA_NATIONAL>());
                FillDataToGridLookupedit(this.cboMilitaryRank, "MILITARY_RANK_NAME", "ID", "MILITARY_RANK_CODE", BackendDataWorker.Get<HIS_MILITARY_RANK>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                FillDataToLookupedit(this.cboBloodABOCode, "BLOOD_ABO_CODE", "BLOOD_ABO_CODE", BackendDataWorker.Get<HIS_BLOOD_ABO>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList(), 100);
                FillDataToLookupedit(this.cboBloodRHCode, "BLOOD_RH_CODE", "BLOOD_RH_CODE", BackendDataWorker.Get<HIS_BLOOD_RH>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList(), 100);


                FillDataToLookupedit(this.cboTonGiao, "RELIGION_NAME", "ID", BackendDataWorker.Get<SDA_RELIGION>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList(), 100);

                List<HIS_PATIENT_CLASSIFY> data = BackendDataWorker.Get<HIS_PATIENT_CLASSIFY>().ToList();
                MOS.EFMODEL.DataModels.HIS_TREATMENT tm = new MOS.EFMODEL.DataModels.HIS_TREATMENT();
                if (this.TreatmentId > 0)
                {
                    LoadCurrentNewTreatment(this.TreatmentId, ref tm);
                    data = data.Where(o => o.IS_ACTIVE == 1 && (o.PATIENT_TYPE_ID == null || o.PATIENT_TYPE_ID == tm.TDL_PATIENT_TYPE_ID)).ToList();
                }
                else
                {
                    LoadTreatmentLast(PatientId, ref tm);
                    if (tm.ID > 0)
                    {
                        data = data.Where(o => o.IS_ACTIVE == 1 && (o.PATIENT_TYPE_ID == null || o.PATIENT_TYPE_ID == tm.TDL_PATIENT_TYPE_ID)).ToList();
                    }
                }
                FillDataToGridLookupedit(this.cboClassify, "PATIENT_CLASSIFY_NAME", "ID", "PATIENT_CLASSIFY_CODE", data);
                FillDataToGridLookupedit(this.cboPosition, "POSITION_NAME", "ID", "POSITION_CODE", BackendDataWorker.Get<HIS_POSITION>());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadTreatmentLast(long PatientId, ref HIS_TREATMENT tm)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentFilter filter = new MOS.Filter.HisTreatmentFilter();
                filter.PATIENT_ID = PatientId;
                tm = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, filter, null).ToList().OrderByDescending(o => o.TREATMENT_CODE).First();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrentNewTreatment(long? treatmentId, ref HIS_TREATMENT tm)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentFilter filter = new MOS.Filter.HisTreatmentFilter();
                filter.ID = treatmentId;
                tm = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, filter, null).First();
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
                    dtPatientDob.DateTime = dtNgSinh;
                    dtPatientDob.Text = dtNgSinh.ToString("dd/MM/yyyy HH:mm");
                    txtPatientDob.Text = dtNgSinh.ToString("dd/MM/yyyy HH:mm");
                    int age = Inventec.Common.TypeConvert.Parse.ToInt32(nthnm.Substring(8, 2));
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
                if (!string.IsNullOrEmpty(this.txtPatientDob.Text))
                {
                    var dt = this.txtPatientDob.Text;
                    dt = dt.Replace("/", "");
                    dt = dt.Replace(":", "");
                    dt = dt.Replace(" ", "");

                    if (dt.Length == 12)
                    {
                        var hour = dt.Substring(8, 2);
                        var minutes = dt.Substring(10, 2);
                        patientDTO.DOB = Inventec.Common.TypeConvert.Parse.ToInt64(dt.Substring(4, 4) + dt.Substring(2, 2) + dt.Substring(0, 2) + hour + minutes + "00");
                    }
                    else if (dt.Length == 8)
                    {
                        patientDTO.DOB = Inventec.Common.TypeConvert.Parse.ToInt64(dt.Substring(4, 4) + dt.Substring(2, 2) + dt.Substring(0, 2) + "000000");
                    }
                    else if (dt.Length == 4)
                    {
                        patientDTO.DOB = Inventec.Common.TypeConvert.Parse.ToInt64(dt + "0101000000");
                    }

                }
                else if (this.dtPatientDob.EditValue != null)
                    patientDTO.DOB = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(this.dtPatientDob.DateTime) ?? 0;
                else
                {
                    DateUtil.DateValidObject dateValidObject = DateUtil.ValidPatientDob(this.txtPatientDob.Text);
                    if (dateValidObject != null && dateValidObject.HasNotDayDob)
                    {
                        this.dtPatientDob.EditValue = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(dateValidObject.OutDate);
                        this.dtPatientDob.Update();
                    }
                }

                if (this.isNotPatientDayDob)
                {
                    patientDTO.IS_HAS_NOT_DAY_DOB = 1;
                }
                else
                {
                    patientDTO.IS_HAS_NOT_DAY_DOB = null;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Loi xu ly cat chuoi ho ten benh nhan: ", ex);
            }
            try
            {
                if (cboGender1.EditValue != null)
                    patientDTO.GENDER_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboGender1.EditValue ?? "0").ToString());
                patientDTO.PATIENT_STORE_CODE = txtPatientStoreCode.Text.Trim();
                patientDTO.TAX_CODE = txtTaxCode.Text.Trim();
                patientDTO.ADDRESS = txtAddress.Text;
                if (cboCareer.EditValue != null)
                {
                    var careerId = Inventec.Common.TypeConvert.Parse.ToInt64((cboCareer.EditValue ?? "").ToString());
                    var career = BackendDataWorker.Get<HIS_CAREER>().FirstOrDefault(o => o.ID == careerId);
                    patientDTO.CAREER_ID = careerId;
                    patientDTO.CAREER_CODE = career.CAREER_CODE;
                    patientDTO.CAREER_NAME = career.CAREER_NAME;
                }
                else
                {
                    patientDTO.CAREER_ID = null;
                    patientDTO.CAREER_CODE = null;
                    patientDTO.CAREER_NAME = null;
                }

                if (cboEthnic.EditValue != null)
                {
                    var ethnic = BackendDataWorker.Get<SDA_ETHNIC>().FirstOrDefault(o => o.ETHNIC_CODE == cboEthnic.EditValue.ToString());
                    if (ethnic != null)
                    {
                        patientDTO.ETHNIC_NAME = ethnic.ETHNIC_NAME;
                        patientDTO.ETHNIC_CODE = ethnic.ETHNIC_CODE;
                    }
                }
                else
                {
                    patientDTO.ETHNIC_NAME = "";
                    patientDTO.ETHNIC_CODE = "";
                }

                if (cboNation.EditValue != null)
                {
                    var nation = BackendDataWorker.Get<SDA_NATIONAL>().FirstOrDefault(o => o.NATIONAL_CODE == cboNation.EditValue.ToString());
                    if (nation != null)
                    {
                        patientDTO.NATIONAL_CODE = nation.NATIONAL_CODE;
                        patientDTO.NATIONAL_NAME = nation.NATIONAL_NAME;
                        patientDTO.MPS_NATIONAL_CODE = nation.MPS_NATIONAL_CODE;
                    }
                }
                else
                {
                    patientDTO.NATIONAL_CODE = "";
                    patientDTO.NATIONAL_NAME = "";
                    patientDTO.MPS_NATIONAL_CODE = "";
                }

                if (cboProvince.EditValue != null)
                {
                    var province = BackendDataWorker.Get<V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().FirstOrDefault(o => o.PROVINCE_CODE == cboProvince.EditValue.ToString());
                    if (province != null)
                    {
                        patientDTO.PROVINCE_NAME = province.PROVINCE_NAME;
                        patientDTO.PROVINCE_CODE = province.PROVINCE_CODE;
                    }
                }
                else
                {
                    patientDTO.PROVINCE_NAME = "";
                    patientDTO.PROVINCE_CODE = "";
                }

                if (cboDistricts.EditValue != null)
                {
                    var district = BackendDataWorker.Get<V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().FirstOrDefault(o => o.DISTRICT_CODE == cboDistricts.EditValue.ToString());
                    if (district != null)
                    {
                        patientDTO.DISTRICT_CODE = district.DISTRICT_CODE;
                        patientDTO.DISTRICT_NAME = district.INITIAL_NAME + " " + district.DISTRICT_NAME;
                    }
                }
                else
                {
                    patientDTO.DISTRICT_CODE = txtDistricts.Text;
                    patientDTO.DISTRICT_NAME = ((cboDistricts.Text ?? "").ToString());
                }

                if (cboCommune.EditValue != null)
                {
                    var commune = BackendDataWorker.Get<V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().FirstOrDefault(o => o.COMMUNE_CODE == cboCommune.EditValue.ToString());
                    if (commune != null)
                    {
                        patientDTO.COMMUNE_NAME = commune.INITIAL_NAME + " " + commune.COMMUNE_NAME;
                        patientDTO.COMMUNE_CODE = commune.COMMUNE_CODE;
                    }
                }
                else
                {
                    patientDTO.COMMUNE_NAME = "";
                    patientDTO.COMMUNE_CODE = "";
                }

                patientDTO.PHONE = txtPhone.Text;
                patientDTO.RELATIVE_TYPE = txtRelation.Text;
                patientDTO.EMAIL = txtEmail.Text;
                patientDTO.RELATIVE_ADDRESS = txtContact.Text;
                patientDTO.RELATIVE_NAME = txtPersonFamily.Text;
                patientDTO.IS_CHRONIC = (short)(chkBNManTinh.Checked ? 1 : 0);
                patientDTO.IS_TUBERCULOSIS = (short)(chkIsTuberculosis.Checked ? 1 : 0);
                patientDTO.IS_HIV = (short)(chkIsHiv.Checked ? 1 : 0);
                patientDTO.RELATIVE_CMND_NUMBER = txtCMNDRelative.Text;
                patientDTO.PT_PATHOLOGICAL_HISTORY = txtTienSuBenh.Text;
                patientDTO.PT_PATHOLOGICAL_HISTORY_FAMILY = txtTienSuGiaDinh.Text;

                if (cboMilitaryRank.EditValue != null)
                    patientDTO.MILITARY_RANK_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboMilitaryRank.EditValue ?? "").ToString());
                else
                    patientDTO.MILITARY_RANK_ID = null;
                if (workPlaceTemplate == WorkPlaceProcessor.Template.Combo)
                {
                    patientDTO.WORK_PLACE_ID = (long?)workPlaceProcessor.GetValue(ucWorkPlace, workPlaceTemplate);
                    //patientDTO.WORK_PLACE = "";
                }

                else if (workPlaceTemplate == WorkPlaceProcessor.Template.Textbox)
                {
                    patientDTO.WORK_PLACE = (string)workPlaceProcessor.GetValue(ucWorkPlace, workPlaceTemplate);
                    //patientDTO.WORK_PLACE_ID = null;
                }
                if (!String.IsNullOrEmpty(txtOtherAddress.Text))
                {
                    patientDTO.WORK_PLACE = txtOtherAddress.Text;
                }
                else
                {
                    patientDTO.WORK_PLACE = null;
                }

                patientDTO.ACCOUNT_NUMBER = txtAccountNumber.Text;
                if (cboBloodABOCode.EditValue != null)
                {
                    patientDTO.BLOOD_ABO_CODE = cboBloodABOCode.EditValue.ToString();
                }
                else
                    patientDTO.BLOOD_ABO_CODE = "";
                if (cboBloodRHCode.EditValue != null)
                {
                    patientDTO.BLOOD_RH_CODE = cboBloodRHCode.EditValue.ToString();
                }
                else
                    patientDTO.BLOOD_RH_CODE = "";
                patientDTO.HT_ADDRESS = txtHTAddress.Text;
                if (cboHTProvinceName.EditValue != null)
                {
                    var province = BackendDataWorker.Get<V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().FirstOrDefault(o => o.PROVINCE_CODE == cboHTProvinceName.EditValue.ToString());
                    if (province != null)
                    {
                        patientDTO.HT_PROVINCE_NAME = province.PROVINCE_NAME;
                        patientDTO.HT_PROVINCE_CODE = province.PROVINCE_CODE;
                    }
                }
                else
                {
                    patientDTO.HT_PROVINCE_NAME = "";
                    patientDTO.HT_PROVINCE_CODE = "";
                }

                if (cboHTDistrictName.EditValue != null)
                {
                    var district = BackendDataWorker.Get<V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().FirstOrDefault(o => o.DISTRICT_CODE == cboHTDistrictName.EditValue.ToString());
                    if (district != null)
                    {
                        patientDTO.HT_DISTRICT_NAME = district.DISTRICT_NAME;
                        patientDTO.HT_DISTRICT_CODE = district.DISTRICT_CODE;
                    }
                }
                else
                {
                    patientDTO.HT_DISTRICT_NAME = "";
                    patientDTO.HT_DISTRICT_CODE = "";

                }

                if (cboHTCommuneName.EditValue != null)
                {
                    var commune = BackendDataWorker.Get<V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().FirstOrDefault(o => o.COMMUNE_CODE == cboHTCommuneName.EditValue.ToString());
                    if (commune != null)
                    {
                        patientDTO.HT_COMMUNE_NAME = commune.COMMUNE_NAME;
                        patientDTO.HT_COMMUNE_CODE = commune.COMMUNE_CODE; 
                    }
                }
                else
                {
                    patientDTO.HT_COMMUNE_NAME = "";
                    patientDTO.HT_COMMUNE_CODE = "";
                }

                patientDTO.FATHER_NAME = txtFatherName.Text.Trim();
                patientDTO.MOTHER_CAREER = txtMotherCareer.Text.Trim();
                patientDTO.MOTHER_EDUCATIIONAL_LEVEL = txtMotherEducationalLevel.Text.Trim();
                patientDTO.FATHER_CAREER = txtFatherCareer.Text.Trim();
                patientDTO.FATHER_EDUCATIIONAL_LEVEL = txtFatherEducationalLevel.Text.Trim();
                patientDTO.MOTHER_NAME = txtMotherName.Text.Trim();
                patientDTO.RELATIVE_MOBILE = txtRelativeMobile.Text.Trim();
                patientDTO.RELATIVE_PHONE = txtRelativePhone.Text.Trim();
                patientDTO.SOCIAL_INSURANCE_NUMBER = txtSocialInsuranceNumber.Text.Trim();
                patientDTO.NOTE = txtNote.Text.Trim();
                patientDTO.FATHER_SOCIAL_INSURANCE_NUMBER = txtBhxhFather.Text.Trim();
                patientDTO.MOTHER_SOCIAL_INSURANCE_NUMBER = txtBhxhMother.Text.Trim();
                //patientDTO.UUID = txtUUID.Text.Trim();
                if (!String.IsNullOrEmpty(txtCCCD_CMTNumber.Text))
                {
                    if (Inventec.Common.String.CountVi.Count(txtCCCD_CMTNumber.Text.Trim()) == 12)
                    {
                        Int64 k;
                        bool isNumeric = Int64.TryParse(txtCCCD_CMTNumber.Text, out k);
                        if (isNumeric)
                        {
                            if (txtCCCD_CMTDate.EditValue != null)
                                patientDTO.CCCD_DATE = Convert.ToInt64(txtCCCD_CMTDate.DateTime.ToString("yyyyMMdd") + "000000");
                            else
                                patientDTO.CCCD_DATE = null;
                            patientDTO.CCCD_NUMBER = txtCCCD_CMTNumber.Text.Trim();
                            patientDTO.CCCD_PLACE = txtCCCD_CMTPlace.Text.Trim();
                            patientDTO.CMND_DATE = null;
                            patientDTO.CMND_NUMBER = "";
                            patientDTO.CMND_PLACE = "";

                            patientDTO.PASSPORT_DATE = null;
                            patientDTO.PASSPORT_NUMBER = "";
                            patientDTO.PASSPORT_PLACE = "";
                        }
                    }
                    else
                    {
                        Int64 n;
                        bool isNumeric = Int64.TryParse(txtCCCD_CMTNumber.Text, out n);
                        if (isNumeric)
                        {
                            patientDTO.CCCD_DATE = null;
                            patientDTO.CCCD_NUMBER = "";
                            patientDTO.CCCD_PLACE = "";

                            patientDTO.PASSPORT_DATE = null;
                            patientDTO.PASSPORT_NUMBER = "";
                            patientDTO.PASSPORT_PLACE = "";

                            if (txtCCCD_CMTDate.EditValue != null)
                                patientDTO.CMND_DATE = Convert.ToInt64(txtCCCD_CMTDate.DateTime.ToString("yyyyMMdd") + "000000");
                            else
                                patientDTO.CMND_DATE = null;
                            patientDTO.CMND_NUMBER = txtCCCD_CMTNumber.Text.Trim();
                            patientDTO.CMND_PLACE = txtCCCD_CMTPlace.Text.Trim();
                        }
                        else
                        {
                            patientDTO.CCCD_DATE = null;
                            patientDTO.CCCD_NUMBER = "";
                            patientDTO.CCCD_PLACE = "";

                            patientDTO.CMND_DATE = null;
                            patientDTO.CMND_NUMBER = "";
                            patientDTO.CMND_PLACE = "";

                            if (txtCCCD_CMTDate.EditValue != null)
                                patientDTO.PASSPORT_DATE = Convert.ToInt64(txtCCCD_CMTDate.DateTime.ToString("yyyyMMdd") + "000000");
                            else
                                patientDTO.PASSPORT_DATE = null;
                            patientDTO.PASSPORT_NUMBER = txtCCCD_CMTNumber.Text.Trim();
                            patientDTO.PASSPORT_PLACE = txtCCCD_CMTPlace.Text.Trim();
                        }
                    }
                }
                else
                {
                    patientDTO.CCCD_DATE = null;
                    patientDTO.CCCD_NUMBER = "";
                    patientDTO.CCCD_PLACE = "";
                    patientDTO.CMND_DATE = null;
                    patientDTO.CMND_NUMBER = "";
                    patientDTO.CMND_PLACE = "";

                    patientDTO.PASSPORT_DATE = null;
                    patientDTO.PASSPORT_NUMBER = "";
                    patientDTO.PASSPORT_PLACE = "";
                }
                patientDTO.UUID_BHYT_NUMBER = txtTheBHYT.Text.Trim();
                if (cboTonGiao.EditValue != null)
                {
                    var itemdata = BackendDataWorker.Get<SDA_RELIGION>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboTonGiao.EditValue));
                    if (itemdata != null)
                        patientDTO.RELIGION_NAME = itemdata.RELIGION_NAME;
                }

                if (cboClassify.EditValue != null && cboClassify.EditValue.ToString() != "")
                {
                    patientDTO.PATIENT_CLASSIFY_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboClassify.EditValue ?? "").ToString());
                }
                else
                {
                    patientDTO.PATIENT_CLASSIFY_ID = null;
                }

                if (cboPosition.EditValue != null)
                    patientDTO.POSITION_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboPosition.EditValue ?? "").ToString());
                else
                    patientDTO.POSITION_ID = null;
                if (pictureBox1.Tag == "NoImage")
                    patientDTO.AVATAR_URL = null;

                if (pictureBox2.Tag == "NoImage")
                    patientDTO.BHYT_URL = null;
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

        #region ---ButtonClick---

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

        private void cboTonGiao_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTonGiao.EditValue = null;
                    cboTonGiao.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region ---EditValueChanged---

        private void dtDOB_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT.MUST_HAVE_NCS_INFO_FOR_CHILD") == "1" && MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(dtPatientDob.DateTime))
                {
                    isGKS = true;

                }
                else
                {
                    isGKS = false;

                }

                ValidateForm(isGKS);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        #endregion

        #region ---KeyDown---
        private void txtPatientDOB_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void txtCMNDRelative_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtContact.Focus();
                    txtContact.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region ---Leave---
        private void txtCMNDRelative_Leave(object sender, EventArgs e)
        {
        }

        private void txtPatientDOB_Leave(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(dtPatientDob.Text))
                {
                    dtPatientDob.Text = PatientDobUtil.PatientDobToDobRaw(dtPatientDob.Text);
                }

                if (!String.IsNullOrEmpty(dtPatientDob.Text))
                {
                    string strDob = "";

                    if (dtPatientDob.Text.Length == 2 || dtPatientDob.Text.Length == 1)
                    {
                        int patientDob = Int32.Parse(dtPatientDob.Text);
                        if (patientDob < 7)
                        {
                            MessageBox.Show(ResourceLanguageManager.NgaySinhKhongDuocNhoHon7);
                            FocusMoveText(this.dtPatientDob);
                            return;
                        }
                        else
                        {
                            dtPatientDob.Text = (DateTime.Now.Year - patientDob).ToString();
                        }
                    }
                    else if (dtPatientDob.Text.Length == 4)
                    {
                        if (Inventec.Common.TypeConvert.Parse.ToInt64(dtPatientDob.Text) <= DateTime.Now.Year)
                        {
                            strDob = "01/01/" + dtPatientDob.Text;
                            isNotPatientDayDob = true;
                        }
                        else
                        {
                            MessageBox.Show(ResourceLanguageManager.NhapNgaySinhKhongDungDinhDang);
                            FocusMoveText(this.dtPatientDob);
                            return;
                        }

                    }
                    else if (dtPatientDob.Text.Length == 8)
                    {
                        strDob = dtPatientDob.Text.Substring(0, 2) + "/" + dtPatientDob.Text.Substring(2, 2) + "/" + dtPatientDob.Text.Substring(4, 4);
                        if (HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(strDob).Value.Date <= DateTime.Now.Date)
                        {
                            strDob = dtPatientDob.Text.Substring(0, 2) + "/" + dtPatientDob.Text.Substring(2, 2) + "/" + dtPatientDob.Text.Substring(4, 4);
                            dtPatientDob.Text = strDob;
                            isNotPatientDayDob = false;
                        }
                        else
                        {
                            MessageBox.Show(ResourceLanguageManager.ThongTinNgaySinhPhaiNhoHonNgayHienTai);
                            FocusMoveText(this.dtPatientDob);
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show(ResourceLanguageManager.NhapNgaySinhKhongDungDinhDang);
                        FocusMoveText(this.dtPatientDob);
                        return;
                    }


                    if (String.IsNullOrWhiteSpace(strDob))
                    {
                        strDob = dtPatientDob.Text;
                    }
                    TimBenhNhanTheoDieuKien(true);
                    isTxtPatientDobPreviewKeyDown = true;

                    FocusMoveText(this.txtNation);
                }
                else
                {
                    FocusMoveText(this.dtPatientDob);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        #endregion

        #region ---PreviewKeyDown---
        private void txtTienSuBenh_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTienSuGiaDinh.Focus();
                    txtTienSuGiaDinh.SelectAll();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTienSuGiaDinh_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Tab)
                {
                    if (chkUpdateNew.Enabled)
                        chkUpdateNew.Focus();
                    else
                        btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtCCCD_CMTDate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    workPlaceProcessor.FocusControl(workPlaceTemplate);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtTheBHYT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSocialInsuranceNumber.Focus();
                    txtSocialInsuranceNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtUUID_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtFatherName.Focus();
                    txtFatherName.SelectAll();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void cboTonGiao_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboTonGiao.EditValue != null)
                    {
                        txtTheBHYT.Focus();
                        txtTheBHYT.SelectAll();
                    }
                    else
                    {
                        cboTonGiao.Focus();
                        cboTonGiao.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void cboMilitaryRank_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboMilitaryRank.EditValue != null)
                    {
                        cboPosition.Focus();
                        cboPosition.SelectAll();
                    }
                    else
                    {
                        cboMilitaryRank.Focus();
                        cboMilitaryRank.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void cboBloodABOCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboBloodABOCode.EditValue != null)
                    {
                        cboBloodRHCode.Focus();
                        cboBloodRHCode.SelectAll();
                    }
                    else
                    {
                        cboBloodABOCode.Focus();
                        cboBloodABOCode.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void cboBloodRHCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboBloodRHCode.EditValue != null)
                    {
                        txtPatientStoreCode.Focus();
                        txtPatientStoreCode.SelectAll();
                    }
                    else
                    {
                        cboBloodRHCode.Focus();
                        cboBloodRHCode.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void chkUpdateNew_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkEmrUpdate.Focus();
                    //btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtPatientStoreCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtFatherName.Focus();
                    txtFatherName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTaxCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboMilitaryRank.Focus();
                    cboMilitaryRank.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region ---Method private---
        private void CallModuleCamera(DelegateSelectData delegateSelect)
        {
            try
            {
                List<object> listArgs = new List<object>();
                listArgs.Add(delegateSelect);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.Camera", 0, 0, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TimBenhNhanTheoDieuKien(bool isShowMessage)
        {
            try
            {
                string strDob = "";
                if (dtPatientDob.Text.Length == 4)
                    strDob = "01/01/" + dtPatientDob.Text;
                else if (dtPatientDob.Text.Length == 8)
                {
                    strDob = dtPatientDob.Text.Substring(0, 2) + "/" + dtPatientDob.Text.Substring(2, 2) + "/" + dtPatientDob.Text.Substring(4, 4);
                }
                else
                    strDob = dtPatientDob.Text;
                dtPatientDob.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(strDob);
                dtPatientDob.Update();
                if ((dtPatientDob.EditValue == null
                    || dtPatientDob.DateTime == DateTime.MinValue)
                    || String.IsNullOrEmpty(txtPatientName.Text.Trim()))
                {
                    return;
                }
                LogSystem.Debug("Bat dau tim kiem benh nhan theo filter.");
                string dateDob = dtPatientDob.DateTime.ToString("yyyyMMdd");
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
                if (dtPatientDob.Text.Length == 2 || dtPatientDob.Text.Length == 1)
                {
                    int patientDob = Int32.Parse(dtPatientDob.Text);
                    if (patientDob < 7)
                    {
                        if (showMessage)
                            MessageBox.Show(ResourceLanguageManager.NgaySinhKhongDuocNhoHon7);
                        FocusMoveText(this.dtPatientDob);
                        return;
                    }
                    else
                    {
                        dtPatientDob.Text = (DateTime.Now.Year - patientDob).ToString();
                    }
                }
                else if (dtPatientDob.Text.Length == 4)
                {
                    if (Inventec.Common.TypeConvert.Parse.ToInt64(dtPatientDob.Text) <= DateTime.Now.Year)
                    {
                        strDob = "01/01/" + dtPatientDob.Text;
                        isNotPatientDayDob = true;
                    }
                    else
                    {
                        if (showMessage)
                            MessageBox.Show(ResourceLanguageManager.NhapNgaySinhKhongDungDinhDang);
                        FocusMoveText(this.dtPatientDob);
                        return;
                    }
                }
                else if (dtPatientDob.Text.Length < 4)
                {
                    if (showMessage)
                        MessageBox.Show(ResourceLanguageManager.NhapNgaySinhKhongDungDinhDang);
                    FocusMoveText(this.dtPatientDob);
                    return;
                }
                else if (dtPatientDob.Text.Length == 8)
                {
                    strDob = dtPatientDob.Text.Substring(0, 2) + "/" + dtPatientDob.Text.Substring(2, 2) + "/" + dtPatientDob.Text.Substring(4, 4);
                    if (HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(strDob).Value.Date <= DateTime.Now.Date)
                    {
                        dtPatientDob.Text = strDob;
                        isNotPatientDayDob = false;
                        dtPatientDob.EditValue = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(strDob);
                        dtPatientDob.Update();
                    }
                    else
                    {
                        if (showMessage)
                        {
                            dtPatientDob.Text = strDob;
                            return;
                        }
                    }
                }
                else if (dtPatientDob.Text.Length == 10)
                {
                    if (HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(dtPatientDob.Text).Value.Date <= DateTime.Now.Date)
                    {
                        isNotPatientDayDob = false;
                        dtPatientDob.EditValue = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(strDob);
                        dtPatientDob.Update();
                    }
                    else
                    {
                        if (showMessage)
                            MessageBox.Show(ResourceLanguageManager.NhapNgaySinhKhongDungDinhDang);
                        FocusMoveText(this.dtPatientDob);
                        return;
                    }
                }
                else
                {
                    if (showMessage)
                        MessageBox.Show(ResourceLanguageManager.NhapNgaySinhKhongDungDinhDang);
                    FocusMoveText(this.dtPatientDob);
                    return;
                }

                if (String.IsNullOrWhiteSpace(strDob))
                {
                    strDob = dtPatientDob.Text;
                }
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
                string birthDay = dtPatientDob.Text;
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

        #endregion

        #region ---internal---
        internal void SetImageDefaultForPictureEdit(PictureEdit pictureEdit, Image imageData)
        {
            try
            {
                if (imageData != null)
                {
                    pictureEdit.Image = (Image)imageData;
                    pictureEdit.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
                    pictureEdit.Tag = "Image";
                }
                else
                {
                    System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPatientUpdate));
                    pictureEdit.EditValue = ((object)(resources.GetObject("pictureBox1.EditValue")));
                    pictureEdit.Tag = "NoImage";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        internal void FillImageAvatarFromModuleCamereToUC(object dataImage)
        {
            try
            {
                if (dataImage != null)
                {
                    pictureBox1.Image = (Image)dataImage;
                    pictureBox1.Tag = ((Image)dataImage).Tag;
                    pictureBox1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
                    this.pictureBox1.Tag = "Image";
                }
                else
                {
                    System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPatientUpdate));
                    this.pictureBox1.EditValue = ((object)(resources.GetObject("pictureBox1.EditValue")));
                    this.pictureBox1.Tag = "NoImage";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void FillImageBHYTFromModuleCamereToUC(object dataImage)
        {
            try
            {
                if (dataImage != null)
                {
                    pictureBox2.Image = (Image)dataImage;
                    pictureBox2.Tag = ((Image)dataImage).Tag;
                    pictureBox2.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
                    this.pictureBox2.Tag = "Image";
                }
                else
                {
                    System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPatientUpdate));
                    this.pictureBox1.EditValue = ((object)(resources.GetObject("pictureBox1.EditValue")));
                    this.pictureBox2.Tag = "NoImage";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region ---Click---
        private void btnUploadChanDung_Click(object sender, EventArgs e)
        {
            try
            {
                CallModuleCamera((DelegateSelectData)FillImageAvatarFromModuleCamereToUC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnUploadTheBHYT_Click(object sender, EventArgs e)
        {
            try
            {
                CallModuleCamera((DelegateSelectData)FillImageBHYTFromModuleCamereToUC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region ---ImageChanged---
        private void pictureBox1_ImageChanged(object sender, EventArgs e)
        {
            try
            {
                this.demAvatar++;
                if (this.demAvatar != 0 && pictureBox1.Image != null)
                {
                    this.pictureBox1.Tag = "Image";
                }
                else
                {
                    this.pictureBox1.Tag = "NoImage";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void pictureBox2_ImageChanged(object sender, EventArgs e)
        {
            try
            {
                this.demBHYT++;
                if (this.demBHYT != 0 && pictureBox2.Image != null)
                {
                    this.pictureBox2.Tag = "Image";
                }
                else
                {
                    this.pictureBox2.Tag = "NoImage";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        #endregion

        #region ---Closed---
        private void txtProvinceName_Closed(object sender, ClosedEventArgs e)
        {

        }

        private void cboHTDistrictName_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboHTDistrictName.EditValue != null)
                    {
                        string str = cboHTDistrictName.EditValue.ToString();
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList()
                            .SingleOrDefault(o => o.DISTRICT_CODE == cboHTDistrictName.EditValue.ToString()
                                && (String.IsNullOrEmpty((cboHTProvinceName.EditValue ?? "").ToString()) || o.PROVINCE_CODE == (cboHTProvinceName.EditValue ?? "").ToString()));
                        if (district != null)
                        {

                            if (String.IsNullOrEmpty((cboHTProvinceName.EditValue ?? "").ToString()))
                            {
                                cboHTProvinceName.EditValue = district.PROVINCE_CODE;
                            }
                            LoadHTCommuneCombo("", district.DISTRICT_CODE, false);
                            txtHTDistrictCode.Text = district.SEARCH_CODE;
                            var data = (List<V_SDA_COMMUNE>)cboHTCommuneName.Properties.DataSource;
                            cboHTCommuneName.EditValue = null;
                            txtHTCommuneCode.Text = "";
                        }
                    }
                    FocusMoveText(this.txtHTCommuneCode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHTCommuneName_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboHTCommuneName.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList()
                            .SingleOrDefault(o =>
                                o.COMMUNE_CODE == cboHTCommuneName.EditValue.ToString()
                                    && (String.IsNullOrEmpty((cboHTDistrictName.EditValue ?? "").ToString()) || o.DISTRICT_CODE == (cboHTDistrictName.EditValue ?? "").ToString())
                                );
                        if (commune != null)
                        {
                            txtHTCommuneCode.Text = commune.SEARCH_CODE;
                            if (String.IsNullOrEmpty((cboHTProvinceName.EditValue ?? "").ToString()) && String.IsNullOrEmpty((cboHTDistrictName.EditValue ?? "").ToString()))
                            {
                                cboHTDistrictName.EditValue = commune.DISTRICT_CODE;
                                SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().Where(o => o.ID == commune.DISTRICT_ID).FirstOrDefault();
                                if (district != null)
                                {
                                    cboHTProvinceName.EditValue = district.PROVINCE_CODE;
                                }
                            }
                        }
                    }
                    FocusMoveText(this.txtHTAddress);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBloodRHCode_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboBloodRHCode.EditValue != null)
                    {
                        FocusMoveText(this.txtPatientStoreCode);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBloodABOCode_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboBloodRHCode.EditValue != null)
                    {
                        FocusMoveText(this.cboBloodRHCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTonGiao_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    txtNation.Focus();
                    txtNation.SelectAll();
                }
            }
            catch (Exception ex)
            {

                LogSession.Warn(ex);
            }
        }
        #endregion

        #region ---KeyPress---
        private void txtRelativePhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '/'))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtRelativeMobile_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '/'))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCCCD_CMTNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                //if (!(Char.IsDigit(e.KeyChar) || Char.IsControl(e.KeyChar)))
                //{
                //    e.Handled = true;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtCMNDRelative_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!(Char.IsDigit(e.KeyChar) || Char.IsControl(e.KeyChar)))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '/'))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void cboHTProvinceName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboHTProvinceName.EditValue != null)
                    {
                        string str = cboHTProvinceName.EditValue.ToString();
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().SingleOrDefault(o => o.PROVINCE_CODE == cboHTProvinceName.EditValue.ToString());
                        if (province != null)
                        {
                            LoadHTDistrictsCombo("", province.PROVINCE_CODE, false);
                            txtHTProvinceCode.Text = province.SEARCH_CODE;
                            txtHTDistrictCode.Text = "";
                            txtHTDistrictCode.Focus();
                        }
                    }
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

        private void txtPatientDOB_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            //try
            //{
            //    HIS.Desktop.Plugins.PatientUpdate.DateValidObject dateValidObject = DateValidObject.ValidPatientDob(this.txtPatientDOB.Text);
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

        private void chkEmrUpdate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void chkUpdateNew_CheckedChanged(object sender, EventArgs e)
        {
            if (this.currentPatient != null)
            {

                if (chkUpdateNew.Checked == true)
                {
                    chkEmrUpdate.Enabled = true;
                }
                else
                {
                    chkEmrUpdate.Enabled = false;
                    chkEmrUpdate.Checked = false;
                }
            }
        }

        private void txtFatherEducationalLevel_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtFatherCareer.Focus();
                    txtFatherCareer.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtFatherCareer_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBhxhFather.Focus();
                    txtBhxhFather.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMotherEducationalLevel_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMotherCareer.Focus();
                    txtMotherCareer.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMotherCareer_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBhxhMother.Focus();
                    txtBhxhMother.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtRelativeMobile_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkBNManTinh.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHTProvinceName_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboHTProvinceName.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().SingleOrDefault(o => o.PROVINCE_CODE == cboHTProvinceName.EditValue.ToString());
                        if (province != null)
                        {
                            LoadHTDistrictsCombo("", province.PROVINCE_CODE, false);
                            txtHTProvinceCode.Text = province.SEARCH_CODE;
                        }
                    }
                    //txtDistricts.Text = "";
                    txtHTDistrictCode.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientDob_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    DateTime? dt = null;
                    if (txtPatientDob.Text.Length == 4)
                    {
                        dt = DateTimeHelper.ConvertDateStringToSystemDate(this.txtPatientDob.Text);
                    }
                    else
                    {
                        int day = Int16.Parse(this.txtPatientDob.Text.Substring(0, 2));
                        int month = Int16.Parse(this.txtPatientDob.Text.Substring(2, 2));
                        int year = Int16.Parse(this.txtPatientDob.Text.Substring(4, 4));
                        dt = new DateTime(year, month, day);
                    }

                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtPatientDob.EditValue = dt;
                        this.dtPatientDob.Update();
                    }
                    this.dtPatientDob.Visible = true;
                    this.dtPatientDob.ShowPopup();
                    this.dtPatientDob.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientDob_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.txtPatientDob.Text)) return;

                string dob = "";
                Inventec.Common.Logging.LogSystem.Debug("this.txtPatientDob.Text__" + txtPatientDob.Text);
                if (this.txtPatientDob.Text.Contains("/") && this.txtPatientDob.Text.Contains(":"))
                {
                    Inventec.Common.Logging.LogSystem.Debug("this.txtPatientDob.Text__True       " + txtPatientDob.Text);
                    var dt = this.txtPatientDob.Text;
                    dt = dt.Replace(":", "");
                    dt = dt.Replace("/", "");
                    dt = dt.Replace(" ", "").Trim();
                    dob = dt;
                    Inventec.Common.Logging.LogSystem.Debug("dob__" + dob);

                }
                if (!String.IsNullOrEmpty(dob))
                {
                    this.txtPatientDob.Text = dob;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientDob_InvalidValue_1(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                DateUtil.DateValidObject dateValidObject = DateUtil.ValidPatientDob(this.txtPatientDob.Text);
                if (dateValidObject != null)
                {
                    e.ErrorText = dateValidObject.Message;
                }

                AutoValidate = AutoValidate.EnableAllowFocusChange;
                e.ExceptionMode = ExceptionMode.DisplayError;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientDob_KeyPress(object sender, KeyPressEventArgs e)
        {
            //try
            //{
            //    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '/'))
            //    {
            //        e.Handled = true;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void dtPatientDob_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    this.dtPatientDob.Visible = false;

                    this.txtPatientDob.Text = dtPatientDob.DateTime.ToString("dd/MM/yyyy HH:mm");
                    string strDob = this.txtPatientDob.Text;
                    this.CalulatePatientAge(strDob);
                    //this.SetValueCareerComboByCondition();
                    this.SearchPatientByFilterCombo();
                    //this.dxValidationProvider1.RemoveControlError(this.txtAge);
                    //if (this.txtAge.Enabled)
                    //{
                    //    this.txtAge.Focus();
                    //    this.txtAge.SelectAll();
                    //    this.ValidateTextAge();
                    //}
                    //else
                    //{
                    //    this.txtCareerCode.Focus();
                    //    this.txtCareerCode.SelectAll();
                    //}

                    Inventec.Common.Logging.LogSystem.Debug("Bat dau CheckTT tu dtPatientDob_Close");
                    //this.CheckTheBaoHiem();

                    Inventec.Common.Logging.LogSystem.Debug("dtPatientDob.Text.Length" + dtPatientDob.Text.Length);
                    if (dtPatientDob.Text.Length == 4 && Inventec.Common.TypeConvert.Parse.ToInt64(dtPatientDob.Text) <= DateTime.Now.Year)
                    {
                        isNotPatientDayDob = true;

                    }
                    else if (dtPatientDob.Text.Length == 8 || dtPatientDob.Text.Length == 10 || dtPatientDob.Text.Length == 16)
                    {
                        if (dtPatientDob.DateTime <= DateTime.Now.Date)
                            isNotPatientDayDob = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SearchPatientByFilterCombo()
        {
            try
            {
                string strDob = "";
                if (this.txtPatientDob.Text.Length == 4)
                    strDob = "01/01/" + this.txtPatientDob.Text;
                else if (this.txtPatientDob.Text.Length == 8)
                {
                    strDob = this.txtPatientDob.Text.Substring(0, 2) + "/" + this.txtPatientDob.Text.Substring(2, 2) + "/" + this.txtPatientDob.Text.Substring(4, 4);
                }
                else
                    strDob = this.txtPatientDob.Text;
                this.dtPatientDob.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(strDob);
                this.dtPatientDob.Update();

                //Trường hợp chưa nhập đủ 3 thông tin: hộ tên, ngày sinh, giới tính thì bỏ qua không thưc hiện tìm kiếm
                if ((this.dtPatientDob.EditValue == null
                    || this.dtPatientDob.DateTime == DateTime.MinValue)
                    //|| this.cboGender.EditValue == null
                    || String.IsNullOrEmpty(this.txtPatientName.Text.Trim()))
                {
                    return;
                }

                LogSystem.Debug("Bat dau tim kiem benh nhan theo filter.");
                string dateDob = this.dtPatientDob.DateTime.ToString("yyyyMMdd");
                string timeDob = "00";
                //if (this.txtAge.Enabled && this.cboAge.Enabled)
                //    timeDob = string.Format("{0:00}", DateTime.Now.Hour - Inventec.Common.TypeConvert.Parse.ToInt32(this.txtAge.Text));

                //long dob = Inventec.Common.TypeConvert.Parse.ToInt64(dateDob + timeDob + "0000");
                //short ismale = Convert.ToInt16(this.cboGender.EditValue);
                //this.LoadDataSearchPatient("", this.txtPatientName.Text, dob, ismale, true);
                //this.cardSearch = null;
                LogSystem.Debug("Ket thuc tim kiem benh nhan theo filter.");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CalulatePatientAge(string strDob)
        {
            try
            {
                this.dtPatientDob.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(strDob);
                if (this.dtPatientDob.EditValue != null && this.dtPatientDob.DateTime != DateTime.MinValue)
                {
                    isGKS = true;
                    DateTime dtNgSinh = this.dtPatientDob.DateTime;
                    TimeSpan diff = DateTime.Now - dtNgSinh;
                    long tongsogiay = diff.Ticks;
                    if (tongsogiay < 0)
                    {
                        //this.txtAge.EditValue = "";
                        //this.cboAge.EditValue = 4;
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
                        //this.cboAge.EditValue = 1;
                        //this.txtAge.Enabled = false;
                        //this.cboAge.Enabled = false;
                        //if (!isGKS)
                        //{
                        //    this.txtAge.EditValue = DateTime.Now.Year - dtNgSinh.Year;
                        //}
                        //else
                        //{
                        //    this.txtAge.EditValue = nam.ToString();
                        //}
                    }
                    //else if (nam > 0 && nam < 7)
                    //{
                    //    if (nam == 6)
                    //    {
                    //        if (thang > 0 || ngay > 0)
                    //        {
                    //            this.cboAge.EditValue = 1;
                    //            this.txtAge.Enabled = false;
                    //            this.cboAge.Enabled = false;
                    //            if (!isGKS)
                    //            {
                    //                this.txtAge.EditValue = DateTime.Now.Year - dtNgSinh.Year;
                    //            }
                    //            else
                    //            {
                    //                this.txtAge.EditValue = nam.ToString();
                    //            }
                    //        }
                    //        else
                    //        {
                    //            this.txtAge.EditValue = nam * 12 - 1;
                    //            this.cboAge.EditValue = 2;
                    //            this.txtAge.Enabled = false;
                    //            this.cboAge.Enabled = false;
                    //        }

                    //    }
                    //    else
                    //    {
                    //        this.txtAge.EditValue = nam * 12 + thang;
                    //        this.cboAge.EditValue = 2;
                    //        this.txtAge.Enabled = false;
                    //        this.cboAge.Enabled = false;
                    //    }

                    //}
                    //else
                    //{
                    //    if (thang > 0)
                    //    {
                    //        this.txtAge.EditValue = thang.ToString();
                    //        this.cboAge.EditValue = 2;
                    //        this.txtAge.Enabled = false;
                    //        this.cboAge.Enabled = false;
                    //    }
                    //    else
                    //    {
                    //        if (ngay > 0)
                    //        {
                    //            this.txtAge.EditValue = ngay.ToString();
                    //            this.cboAge.EditValue = 3;
                    //            this.txtAge.Enabled = false;
                    //            this.cboAge.Enabled = false;
                    //        }
                    //        else
                    //        {
                    //            this.txtAge.EditValue = "";
                    //            this.cboAge.EditValue = 4;
                    //            this.txtAge.Enabled = true;
                    //            this.cboAge.Enabled = false;
                    //        }
                    //    }
                    //}
                    //if (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.MustHaveNCSInfoForChild == true)
                    //{
                    //    this.dlgSetValidation(isGKS);
                    //    Inventec.Common.Logging.LogSystem.Debug("Da bat Validate thong tin nguoi nha");
                    //}
                    //if (this.dlgSetFocusWhenPatientIsChild != null)
                    //    this.dlgSetFocusWhenPatientIsChild(isGKS);
                    //if (this.dlgProcessChangePatientDob != null)
                    //    this.dlgProcessChangePatientDob();
                    //if (this.isTemp_QN == true || (this.isTemp_QN == true && this.isGKS == true))
                    //    this.isEnable(null, true);
                    //else if (this.isGKS == true)
                    //    this.isEnable(true, null);
                    //else
                    //{
                    //    this.isEnable(null, false);
                    //    Inventec.Common.Logging.LogSystem.Debug("Da an checkbox the tam : isTemp_QN = " + isTemp_QN);
                    //}
                    //TODO
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtPatientDob_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.dtPatientDob.Visible = true;
                    this.dtPatientDob.Update();
                    this.txtPatientDob.Text = this.dtPatientDob.DateTime.ToString("dd/MM/yyyy HH:mm");

                    this.CalulatePatientAge(this.txtPatientDob.Text);
                    //this.SetValueCareerComboByCondition();
                    this.SearchPatientByFilterCombo();

                    System.Threading.Thread.Sleep(100);
                    //this.dxValidationProviderControl.RemoveControlError(this.txtAge);
                    //if (this.txtAge.Enabled)
                    //{
                    //    this.txtAge.Focus();
                    //    this.txtAge.SelectAll();
                    //    this.ValidateTextAge();
                    //}
                    //else
                    //{
                    //    this.txtCareerCode.Focus();
                    //    this.txtCareerCode.SelectAll();
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientDob_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.isDobTextEditKeyEnter = true;

                    DateUtil.DateValidObject dateValidObject = DateUtil.ValidPatientDob(this.txtPatientDob.Text);
                    if (dateValidObject.Age > 0 && String.IsNullOrEmpty(dateValidObject.Message))
                    {
                        //this.txtAge.Text = this.txtPatientDob.Text;
                        //this.cboAge.EditValue = 1;
                        this.txtPatientDob.Text = dateValidObject.Age.ToString();
                    }
                    else if (String.IsNullOrEmpty(dateValidObject.Message))
                    {
                        if (!dateValidObject.HasNotDayDob)
                        {
                            this.txtPatientDob.Text = dateValidObject.OutDate;
                            this.dtPatientDob.EditValue = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(dateValidObject.OutDate);
                            this.dtPatientDob.Update();
                        }
                    }

                    Inventec.Common.Logging.LogSystem.Debug("Bat dau CheckTT tu txtPatientDob_PreviewKeyDown");
                    //this.CheckTheBaoHiem();
                    //if (this.dlgProcessChangePatientDob != null)
                    //    this.dlgProcessChangePatientDob();

                    this.txtProvince.SelectAll();
                    this.txtProvince.Focus();

                    //string dob = this.txtPatientDob.Text.Trim();
                    //if (dob.Length == 4)
                    //    this._HeinCardData.Dob = string.Format(dob, "yyyy");
                    //else if (dob.Length >= 10)
                    //    this._HeinCardData.Dob = string.Format(dob, "dd/MM/yyyy");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientDob_Validating_1(object sender, CancelEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.txtPatientDob.Text.Trim()))
                    return;
                DateUtil.DateValidObject dateValidObject = DateUtil.ValidPatientDob(this.txtPatientDob.Text);
                if (dateValidObject.Age > 0 && String.IsNullOrEmpty(dateValidObject.Message))
                {
                    //this.txtAge.Text = this.txtPatientDob.Text;
                    //this.cboAge.EditValue = 1;
                    this.txtPatientDob.Text = dateValidObject.Age.ToString();
                }
                else if (String.IsNullOrEmpty(dateValidObject.Message))
                {
                    if (!dateValidObject.HasNotDayDob)
                    {
                        this.txtPatientDob.Text = dateValidObject.OutDate;
                        this.dtPatientDob.EditValue = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(dateValidObject.OutDate);
                        this.dtPatientDob.Update();
                    }
                }
                else
                {
                    e.Cancel = true;
                    return;
                }

                this.isNotPatientDayDob = dateValidObject.HasNotDayDob;
                if (
                    ((this.txtPatientDob.EditValue ?? "").ToString() != (this.txtPatientDob.OldEditValue ?? "").ToString())
                    && (!String.IsNullOrEmpty(dateValidObject.OutDate))
                    )
                {
                    this.dxValidationProvider1.RemoveControlError(this.txtPatientDob);
                    this.txtPatientDob.ErrorText = "";
                    this.CalulatePatientAge(dateValidObject.OutDate);
                    //this.SetValueCareerComboByCondition();
                    this.SearchPatientByFilterCombo();
                }
                //if (this.isDobTextEditKeyEnter && this.txtAge.Enabled)
                //{
                //    this.txtAge.Focus();
                //    this.txtAge.SelectAll();
                //    this.ValidateTextAge();
                //}
                //else
                //{
                //    this.dxValidationProviderControl.RemoveControlError(this.txtAge);
                //}
                this.isDobTextEditKeyEnter = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtOtherAddress_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void txtCareer_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadNgheNghiepCombo(strValue, false, cboCareer, txtCareer);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSocialInsuranceNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAccountNumber.Focus();
                    txtAccountNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboClassify_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboBloodABOCode.Focus();
                    cboBloodABOCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboClassify_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    cboBloodABOCode.Focus();
                    cboBloodABOCode.SelectAll();
                    if (cboClassify.EditValue != null)
                    {
                        cboClassify.Properties.Buttons[1].Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// + TH 1: Số thẻ chưa được gán với bệnh nhân nào ( PATIENT_ID = null trong HIS_CARD) thì thực hiện gọi lên server thẻ để check thông tin bệnh nhân bên thẻ và bên his có trùng họ tên, năm sinh, giới tính không
        ///* Nếu trùng thông tin thì Lưu (Ctrl S) báo xử lý thành công đồng thời gán bệnh nhân vs số thẻ đó
        ///* Nếu khác 1 trong 3 thông tin: họ tên, ngày sinh, giới tính thì Lưu (Ctrl S) hiển thị thông báo "bệnh nhân XXX không trùng thông tin với số thẻ"
        ///+ TH 2: Số thẻ được gán với bệnh nhân ( PATIENT_ID <> null trong HIS_CARD) -> Lưu (Ctrl S) hiển thị thông báo " Thẻ đã được gán bệnh nhân XXXX. Bạn có muốn thực hện không.
        ///"Có" thì thực hiện cập nhật số thẻ vs bệnh nhân .
        ///"Không" không thực hiện
        ///Mặc định focus vào nút không
        ///+ TH 3: TH his_card chưa có bản ghi nào khi nhập số thẻ nhấn enter sẽ gọi lên server thẻ để check thông tin bệnh nhân bên thẻ có trùng với bên his không ( họ tên, ngày sinh , giới tính) :
        ///- Nếu trùng thông tin lưu sẽ thêm mới số thẻ vào his_card tương ứng với patient_id
        ///- Nếu khác 1 trong 3 thì hiển thị thông báo: "Thông tin bệnh nhân trên hệ thống thẻ không khớp. (Họ tên: xxx, Giới tính: yyy, năm sinh: zzzz)".Người dùng đồng ý thì focus chuột vào ô số thẻ, tự động bôi đen, để cho phép họ nhập luôn số khác
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSoThe_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                    this.hisCard = null;
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper().Trim();

                    if (!String.IsNullOrEmpty(strValue))
                    {
                        if (this.currentPatient != null)
                        {
                            HisCardFilter cardFilter = new HisCardFilter();
                            HisPatientUpdateCardSDO hisPatientUpdateCardSDO = new HisPatientUpdateCardSDO();
                            hisPatientUpdateCardSDO.CardCode = strValue;
                            hisPatientUpdateCardSDO.PatientId = this.currentPatient.ID;

                            cardFilter.CARD_CODE__EXACT = strValue;
                            var cards = new BackendAdapter(new CommonParam()).Get<List<HIS_CARD>>("api/HisCard/Get", ApiConsumers.MosConsumer, cardFilter, null);
                            if (cards != null && cards.Count > 0)
                            {
                                string messageShow = "";
                                var card = cards.FirstOrDefault();
                                if (card.PATIENT_ID != null)
                                {
                                    if (card.PATIENT_ID != this.currentPatient.ID)
                                    {
                                        this.hisCard = new BackendAdapter(new CommonParam()).Get<HisCardSDO>("api/HisCard/GetCardSdoByCode", ApiConsumers.MosConsumer, strValue, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                                        if (this.hisCard != null)
                                        {
                                            messageShow = "Thẻ đã được gán bệnh nhân " + hisCard.LastName + " " + hisCard.FirstName + ". Bạn có muốn thực hiện không?";
                                        }
                                        else
                                        {
                                            MessageBox.Show("Không truy vấn được tên bệnh nhân trên thẻ.");
                                        }
                                        if (XtraMessageBox.Show(messageShow, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                                        {
                                            CommonParam commonParam = new CommonParam();
                                            bool success = new BackendAdapter(commonParam).Post<bool>("api/HisPatient/UpdateCard", ApiConsumers.MosConsumer, hisPatientUpdateCardSDO, commonParam);
                                            MessageManager.Show(this, commonParam, success);
                                        }
                                        else
                                        {
                                            this.txtSoThe.Focus();
                                            this.txtSoThe.SelectAll();
                                        }
                                    }
                                    else
                                    {
                                        XtraMessageBox.Show("Thẻ đã được gán bệnh nhân này.");
                                    }
                                }
                                else
                                {
                                    CommonParam commonParam = new CommonParam();
                                    LogSystem.Debug("The chua duoc gan cho benh nhan nao ca ");
                                    bool success = new BackendAdapter(commonParam).Post<bool>("api/HisPatient/UpdateCard", ApiConsumers.MosConsumer, hisPatientUpdateCardSDO, commonParam);
                                    MessageManager.Show(this, commonParam, success);
                                }
                            }
                            else
                            {
                                CommonParam commonParam = new CommonParam();
                                LogSystem.Debug("The chua duoctao tren he thong, se tao mot the moi va gan cho benh nhan hien tai.");
                                bool success = new BackendAdapter(commonParam).Post<bool>("api/HisPatient/UpdateCard", ApiConsumers.MosConsumer, hisPatientUpdateCardSDO, commonParam);
                                MessageManager.Show(this, commonParam, success);
                            }
                        }
                        else
                        {
                            HisCardFilter cardFilter = new HisCardFilter();
                            HisPatientUpdateCardSDO hisPatientUpdateCardSDO = new HisPatientUpdateCardSDO();
                            hisPatientUpdateCardSDO.CardCode = strValue;
                            hisPatientUpdateCardSDO.PatientId = this.PatientId;

                            cardFilter.CARD_CODE__EXACT = strValue;
                            var cards = new BackendAdapter(new CommonParam()).Get<List<HIS_CARD>>("api/HisCard/Get", ApiConsumers.MosConsumer, cardFilter, null);
                            if (cards != null && cards.Count > 0)
                            {
                                string messageShow = "";
                                var card = cards.FirstOrDefault();
                                if (card.PATIENT_ID != null)
                                {
                                    if (card.PATIENT_ID != this.PatientId)
                                    {
                                        this.hisCard = new BackendAdapter(new CommonParam()).Get<HisCardSDO>("api/HisCard/GetCardSdoByCode", ApiConsumers.MosConsumer, strValue, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                                        if (this.hisCard != null)
                                        {
                                            messageShow = "Thẻ đã được gán bệnh nhân " + hisCard.LastName + " " + hisCard.FirstName + ". Bạn có muốn thực hiện không?";
                                        }
                                        else
                                        {
                                            MessageBox.Show("Không truy vấn được tên bệnh nhân trên thẻ.");
                                        }

                                        if (XtraMessageBox.Show(messageShow, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                                        {
                                            CommonParam commonParam = new CommonParam();
                                            bool success = new BackendAdapter(commonParam).Post<bool>("api/HisPatient/UpdateCard", ApiConsumers.MosConsumer, hisPatientUpdateCardSDO, commonParam);
                                            MessageManager.Show(this, commonParam, success);
                                        }
                                        else
                                        {
                                            this.txtSoThe.Focus();
                                            this.txtSoThe.SelectAll();
                                        }
                                    }
                                    else
                                    {
                                        XtraMessageBox.Show("Thẻ đã được gán bệnh nhân này.");
                                    }
                                }
                                else
                                {
                                    CommonParam commonParam = new CommonParam();
                                    LogSystem.Debug("The chua duoc gan cho benh nhan nao ca ");
                                    bool success = new BackendAdapter(commonParam).Post<bool>("api/HisPatient/UpdateCard", ApiConsumers.MosConsumer, hisPatientUpdateCardSDO, commonParam);
                                    MessageManager.Show(this, commonParam, success);
                                }
                            }
                            else
                            {
                                CommonParam commonParam = new CommonParam();
                                LogSystem.Debug("The chua duoctao tren he thong, se tao mot the moi va gan cho benh nhan hien tai.");
                                bool success = new BackendAdapter(commonParam).Post<bool>("api/HisPatient/UpdateCard", ApiConsumers.MosConsumer, hisPatientUpdateCardSDO, commonParam);
                                MessageManager.Show(this, commonParam, success);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Warn(ex);
            }
        }

        private void cboPosition_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboPosition.EditValue != null)
                    {
                        cboClassify.Focus();
                        cboClassify.SelectAll();
                    }
                    else
                    {
                        cboPosition.Focus();
                        cboPosition.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void cboProvince_ButtonClick(object sender, ButtonPressedEventArgs e)
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

        private void cboDistricts_ButtonClick(object sender, ButtonPressedEventArgs e)
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

        private void cboCommune_ButtonClick(object sender, ButtonPressedEventArgs e)
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

        private void cboHTProvinceName_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (!cboHTProvinceName.ReadOnly && e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboHTProvinceName.EditValue = null;
                    txtHTProvinceCode.Text = "";
                    cboHTDistrictName.EditValue = null;
                    txtHTDistrictCode.Text = "";
                    cboHTCommuneName.EditValue = null;
                    txtHTCommuneCode.Text = "";
                    LoadHTDistrictsCombo("", null, false);
                    LoadHTCommuneCombo("", null, false);
                    cboHTProvinceName.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHTDistrictName_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (!cboHTDistrictName.ReadOnly && e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboHTDistrictName.EditValue = null;
                    txtHTDistrictCode.Text = "";
                    cboHTCommuneName.EditValue = null;
                    txtHTCommuneCode.Text = "";
                    LoadHTCommuneCombo("", null, false);
                    cboHTDistrictName.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHTCommuneName_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (!cboHTCommuneName.ReadOnly && e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboHTCommuneName.EditValue = null;
                    txtHTCommuneCode.Text = "";
                    cboHTCommuneName.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCareer_ButtonClick(object sender, ButtonPressedEventArgs e)
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

        private void cboEthnic_ButtonClick(object sender, ButtonPressedEventArgs e)
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

        private void cboNation_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboNation.EditValue = null;
                    txtNation.Text = "";
                    txtNation.Focus();
                    cboNation.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMilitaryRank_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMilitaryRank.EditValue = null;
                    cboMilitaryRank.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPosition_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPosition.EditValue = null;
                    cboPosition.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboClassify_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboClassify.EditValue = null;
                    cboClassify.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProvince_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboProvince.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().SingleOrDefault(o => o.PROVINCE_CODE == cboProvince.EditValue.ToString());
                        if (province != null)
                        {
                            LoadDistrictsCombo("", province.PROVINCE_CODE, false);
                            txtProvince.Text = province.SEARCH_CODE;
                            txtDistricts.Text = "";
                            txtDistricts.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistricts_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboDistricts.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT disTricts = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().SingleOrDefault(o => o.DISTRICT_CODE == cboDistricts.EditValue.ToString());
                        if (disTricts != null)
                        {
                            LoadCommuneCombo("", disTricts.DISTRICT_CODE, false);
                            txtDistricts.Text = disTricts.SEARCH_CODE;
                            txtCommune.Text = "";
                            txtCommune.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHTDistrictName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboHTDistrictName.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT disTricts = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().SingleOrDefault(o => o.DISTRICT_CODE == cboHTDistrictName.EditValue.ToString());
                        if (disTricts != null)
                        {
                            LoadHTCommuneCombo("", disTricts.PROVINCE_CODE, false);
                            txtDistricts.Text = disTricts.SEARCH_CODE;
                            txtCommune.Text = "";
                            txtCommune.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtProvince_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(txtProvince.Text))
                {
                    cboProvince.Properties.DataSource = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDistricts_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(txtDistricts.Text) && cboProvince.EditValue != null)
                {
                    LoadDistrictsCombo("", cboProvince.EditValue.ToString(), true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCommune_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(txtCommune.Text) && cboDistricts.EditValue != null)
                {
                    LoadCommuneCombo("", cboDistricts.EditValue.ToString(), true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHTProvinceCode_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(txtHTProvinceCode.Text))
                {
                    cboHTProvinceName.Properties.DataSource = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                }
                cboHTDistrictName.EditValue = null;
                txtHTDistrictCode.Text = "";
                cboHTCommuneName.EditValue = null;
                txtHTCommuneCode.Text = "";
               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHTDistrictCode_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(txtHTDistrictCode.Text) && cboHTProvinceName.EditValue != null)
                {
                    LoadHTDistrictsCombo("", cboHTProvinceName.EditValue.ToString(), true);
                }
                cboHTCommuneName.EditValue = null;
                txtHTCommuneCode.Text = "";
                         
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHTCommuneCode_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(txtHTCommuneCode.Text) && cboHTDistrictName.EditValue != null)
                {
                    LoadHTCommuneCombo("", cboHTDistrictName.EditValue.ToString(), true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtBhxhFather_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMotherName.Focus();
                    txtMotherName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtBhxhMother_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPersonFamily.Focus();
                    txtPersonFamily.SelectAll();
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
                if (activeDCFirtTime == true)
                {
                    Inventec.Common.Address.AddressProcessor adProc = new Inventec.Common.Address.AddressProcessor(BackendDataWorker.Get<V_SDA_PROVINCE>(), BackendDataWorker.Get<V_SDA_DISTRICT>(), BackendDataWorker.Get<V_SDA_COMMUNE>());
                    string adress = txtAddress.Text;
                    AddressADO splitAdress = adProc.SplitFromFullAddress(adress);
                    if (!string.IsNullOrEmpty(splitAdress.ProvinceName) && !string.IsNullOrEmpty(splitAdress.DistrictName) && !string.IsNullOrEmpty(splitAdress.CommuneName))
                    {
                        if (cboCommune.Text != "" && cboDistricts.Text != "" && cboProvince.Text != "" && txtProvince.Text != "" && txtDistricts.Text != "" && txtCommune.Text != "")
                        {
                            if (splitAdress.ProvinceCode != txtCommune.Text || splitAdress.DistrictCode != txtDistricts.Text || splitAdress.ProvinceCode != txtProvince.Text)
                            {
                                if (MessageBox.Show("Địa chỉ nhập khác với địa chỉ cũ. Bạn có muốn sử dụng thông tin địa chỉ này không?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                {
                                    cboProvince.EditValue = splitAdress.ProvinceCode;
                                    LoadDistrictsCombo("", splitAdress.ProvinceCode, false);
                                    txtProvince.Text = splitAdress.ProvinceCode; // search code

                                    cboDistricts.EditValue = splitAdress.DistrictCode;
                                    LoadCommuneCombo("", splitAdress.DistrictCode, false);
                                    txtDistricts.Text = splitAdress.DistrictCode; // search code

                                    cboCommune.EditValue = splitAdress.CommuneCode;
                                    //LoadCommuneCombo("", splitAdress.CommuneCode, false);
                                    txtCommune.Text = splitAdress.CommuneCode; // search code
                                }

                            }
                        }
                        else
                        {
                            cboProvince.EditValue = splitAdress.ProvinceCode;
                            LoadDistrictsCombo("", splitAdress.ProvinceCode, false);
                            txtProvince.Text = splitAdress.ProvinceCode; // search code

                            cboDistricts.EditValue = splitAdress.DistrictCode;
                            LoadCommuneCombo("", splitAdress.DistrictCode, false);
                            txtDistricts.Text = splitAdress.DistrictCode; // search code

                            cboCommune.EditValue = splitAdress.CommuneCode;
                            //LoadCommuneCombo("", splitAdress.CommuneCode, false);
                            txtCommune.Text = splitAdress.CommuneCode; // search code
                        }

                    }
                }
                activeDCFirtTime = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHTAddress_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (activeDCHTFirtTime == true)
                {
                    Inventec.Common.Address.AddressProcessor adProc = new Inventec.Common.Address.AddressProcessor(BackendDataWorker.Get<V_SDA_PROVINCE>(), BackendDataWorker.Get<V_SDA_DISTRICT>(), BackendDataWorker.Get<V_SDA_COMMUNE>());
                    string adress = txtHTAddress.Text;
                    AddressADO splitAdress = adProc.SplitFromFullAddress(adress);
                    if (!string.IsNullOrEmpty(splitAdress.ProvinceName) && !string.IsNullOrEmpty(splitAdress.DistrictName) && !string.IsNullOrEmpty(splitAdress.CommuneName))
                    {
                        if (cboHTCommuneName.Text != "" && cboHTDistrictName.Text != "" && cboHTProvinceName.Text != "" && txtHTProvinceCode.Text != "" && txtHTDistrictCode.Text != "" && txtHTCommuneCode.Text != "")
                        {
                            if (splitAdress.ProvinceCode != txtCommune.Text || splitAdress.DistrictCode != txtDistricts.Text || splitAdress.ProvinceCode != txtProvince.Text)
                            {
                                if (MessageBox.Show("Địa chỉ nhập khác với địa chỉ cũ. Bạn có muốn sử dụng thông tin địa chỉ này không?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                {
                                    cboHTProvinceName.EditValue = splitAdress.ProvinceCode;
                                    LoadHTDistrictsCombo("", splitAdress.ProvinceCode, false);
                                    txtHTProvinceCode.Text = splitAdress.ProvinceCode; // search code

                                    cboHTDistrictName.EditValue = splitAdress.DistrictCode;
                                    LoadHTCommuneCombo("", splitAdress.DistrictCode, false);
                                    txtHTDistrictCode.Text = splitAdress.DistrictCode; // search code

                                    cboHTCommuneName.EditValue = splitAdress.CommuneCode;
                                    //LoadCommuneCombo("", splitAdress.CommuneCode, false);
                                    txtHTCommuneCode.Text = splitAdress.CommuneCode; // search code
                                }

                            }

                        }
                        else
                        {
                            cboHTProvinceName.EditValue = splitAdress.ProvinceCode;
                            LoadHTDistrictsCombo("", splitAdress.ProvinceCode, false);
                            txtHTProvinceCode.Text = splitAdress.ProvinceCode; // search code

                            cboHTDistrictName.EditValue = splitAdress.DistrictCode;
                            LoadHTCommuneCombo("", splitAdress.DistrictCode, false);
                            txtHTDistrictCode.Text = splitAdress.DistrictCode; // search code

                            cboHTCommuneName.EditValue = splitAdress.CommuneCode;
                            //LoadCommuneCombo("", splitAdress.CommuneCode, false);
                            txtHTCommuneCode.Text = splitAdress.CommuneCode; // search code
                        }

                    }
                }
                activeDCHTFirtTime = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}

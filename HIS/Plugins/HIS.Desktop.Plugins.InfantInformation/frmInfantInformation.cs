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
using ACS.EFMODEL.DataModels;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.InfantInformation.ADO;
using HIS.Desktop.Plugins.InfantInformation.Config;
using HIS.Desktop.Plugins.InfantInformation.Validate;
using HIS.Desktop.Plugins.InfantInformation.Validation;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Common.RichEditor.Base;
using Inventec.Core;

using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.InfantInformation
{
    public partial class frmInfantInformation : HIS.Desktop.Utility.FormBase
    {
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        internal HIS_BABY babyResult { get; set; }
        internal Inventec.Desktop.Common.Modules.Module Module;
        internal long treatmentId;
        long babyid;
        V_HIS_BABY hisBaby { get; set; }
        HIS_BABY getBaby;
        V_HIS_BABY currentData;
        HIS_TREATMENT treatment = null;
        List<HIS_TREATMENT> treatmentLoad;
        List<HIS_PATIENT> patientload;
        HIS_PATIENT patient = null;
        private string LoggingName = "";

        public bool isOldPatient = false;
        public bool isUpdatedPatient = false;

        private bool isBornedAtHospital = false;
        private HIS_BRANCH hisBranch = new HIS_BRANCH();
        private List<HIS_MEDI_ORG> listHisMediOrg = new List<HIS_MEDI_ORG>();
        private List<DistrictADO> lstDistrictADO = new List<DistrictADO>();
        private List<CommuneADO> lstCommuneADO = new List<CommuneADO>();

        private List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> listProvince = new List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
        private List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> listProvinceTemp = new List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
        private List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> listDistrict = new List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
        private List<DistrictADO> listDistrictTemp = new List<DistrictADO>();
        private List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE> listCommune = new List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>();
        private List<CommuneADO> listCommuneTemp = new List<CommuneADO>();
        private List<HIS_EMPLOYEE> listHisEmpLoyee = new List<HIS_EMPLOYEE>();
        private List<HIS_BABY> listHisBaby = new List<HIS_BABY>();
        public frmInfantInformation()
        {
            InitializeComponent();
        }

        public frmInfantInformation(long treatmentId)
        {
            InitializeComponent();
            try
            {
                this.treatmentId = treatmentId;
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public frmInfantInformation(Inventec.Desktop.Common.Modules.Module moduleData, long treatmentId)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.Module = moduleData;
                this.treatmentId = treatmentId;
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void frmInfantInformation_Load(object sender, EventArgs e)
        {
            try
            {
                HisConfigCFG.LoadConfig();
                SetIcon();
                LoadCombo();

                SetDefaultDataToControl();

                SetDefaultValue();

                this.LoadTreatment();
                //Load du lieu
                FillDataToGridControl();

                ValidControls();

                loadInfoMother();

                LoadInfoComplementFromTreatment(this.treatment);

                //if (HisConfigCFG.IsConfigKeyExportOption == "1")
                //{
                ValidControlBelongConfig();
                //}
                //else
                //{
                //    ValidControlWithoutConfig();
                //}
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadInfoComplementFromTreatment(HIS_TREATMENT treatment)
        {
            try
            {
                if (treatment != null)
                {
                    txtNumOfFullTermBirth.Text = treatment.NUMBER_OF_FULL_TERM_BIRTH.ToString();
                    txtNumOfPrematureBirth.Text = treatment.NUMBER_OF_PREMATURE_BIRTH.ToString();
                    txtNumOfMiscarriage.Text = treatment.NUMBER_OF_MISCARRIAGE.ToString();
                    cboNumOfTest.EditValue = treatment.NUMBER_OF_TESTS;
                    cboTestHiv.EditValue = treatment.TEST_HIV;
                    cboTestSyphilis.EditValue = treatment.TEST_SYPHILIS;
                    cboTestHepatitisB.EditValue = treatment.TEST_HEPATITIS_B;
                    chkIsTestBloodSugar.Checked = treatment.IS_TEST_BLOOD_SUGAR == 1;
                    chkIsEarlyNewBornCare.Checked = treatment.IS_EARLY_NEWBORN_CARE == 1;
                    cboNewBornCareAtHome.EditValue = treatment.NEWBORN_CARE_AT_HOME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCombo()
        {
            listHisEmpLoyee = BackendDataWorker.Get<HIS_EMPLOYEE>();
            listHisBaby = BackendDataWorker.Get<HIS_BABY>();
            listHisEmpLoyee = listHisEmpLoyee.Where(o => o.IS_ACTIVE == 1).ToList();
            listProvince = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
            listDistrict = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
            listCommune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>();
            this.hisBranch = BranchDataWorker.Branch;
            this.listHisMediOrg = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDI_ORG>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

            LoadComboEthnic();
            GetDataHisBirthCertBook();
            LoadComboHisBirthCertBook();

            LoadComboBornType();

            LoadComboBornPosition();

            LoadComboBornResult();

            loadComboGender();

            LoadComboUserGCS();

            LoadComboNumOfTest();

            LoadComboTestHiv();

            LoadComboTestSyphilis();

            LoadComboTestHepatitisB();

            LoadComboNewbornCareAtHome();

            LoadComboBirthPlaceType();

            LoadComboDirectorUser();
        }

        private void LoadComboNewbornCareAtHome()
        {
            try
            {
                List<ComboADO> lstCombo = new List<ComboADO>() {
                    new ComboADO(){Name="Tuần đầu",Id=1 },
                    new ComboADO(){Name="Từ tuần 2 đến hết 6 tuần",Id=2 }
                };
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Name", "", 100, 2, true));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Name", "Id", columnInfos, false, 150);
                controlEditorADO.DropDownRows = 100;
                ControlEditorLoader.Load(cboNewBornCareAtHome, lstCombo, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboTestHepatitisB()
        {
            try
            {
                List<ComboADO> lstCombo = new List<ComboADO>() {
                    new ComboADO(){Name="Trong mang thai",Id=1 },
                    new ComboADO(){Name="Trong chuyển dạ",Id=2 }
                };
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Name", "", 100, 2, true));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Name", "Id", columnInfos, false, 150);
                controlEditorADO.DropDownRows = 100;
                ControlEditorLoader.Load(cboTestHepatitisB, lstCombo, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboTestSyphilis()
        {
            try
            {
                List<ComboADO> lstCombo = new List<ComboADO>() {
                    new ComboADO(){Name="Trong mang thai",Id=1 },
                    new ComboADO(){Name="Trong chuyển dạ",Id=2 }
                };
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Name", "", 100, 2, true));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Name", "Id", columnInfos, false, 150);
                controlEditorADO.DropDownRows = 100;
                ControlEditorLoader.Load(cboTestSyphilis, lstCombo, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboTestHiv()
        {
            try
            {
                List<ComboADO> lstCombo = new List<ComboADO>() {
                    new ComboADO(){Name="Trước và trong mang thai",Id=1 },
                    new ComboADO(){Name="Trong chuyển dạ",Id=2 }
                };
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Name", "", 100, 2, true));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Name", "Id", columnInfos, false, 150);
                controlEditorADO.DropDownRows = 100;
                ControlEditorLoader.Load(cboTestHiv, lstCombo, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboNumOfTest()
        {
            try
            {
                List<ComboADO> lstCombo = new List<ComboADO>() {
                    new ComboADO(){Name="3 lần /3 kỳ",Id=1 },
                    new ComboADO(){Name="≥ 4 lần / 3 kỳ",Id=2 }
                };
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Name", "", 100, 2, true));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Name", "Id", columnInfos, false, 150);
                controlEditorADO.DropDownRows = 100;
                ControlEditorLoader.Load(cboNumOfTest, lstCombo, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadComboBirthPlaceType()
        {
            try
            {
                List<ComboADO> lstCombo = new List<ComboADO>() {
                    new ComboADO(){Name = "Sinh tại viện", Id = 1 },
                    new ComboADO(){Name = "Sinh tại cơ sở y tế khác", Id = 2 },
                    new ComboADO(){Name = "Sinh tại nhà", Id = 3 },
                    new ComboADO(){Name = "Đẻ trên đường đi", Id = 4 },
                    new ComboADO(){Name = "Trẻ bị bỏ rơi", Id = 5 }
                };
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Name", "", 150, 2, true));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Name", "Id", columnInfos, false, 150);
                controlEditorADO.DropDownRows = 100;
                ControlEditorLoader.Load(cboBirthPlaceType, lstCombo, controlEditorADO);
                cboBirthPlaceType.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControlBelongConfig()
        {
            ValidationSingleControl(cboInfantGendercode, dxValidationProviderEditorInfo);
            ValidationSingleControl(dtdInfantdate, dxValidationProviderEditorInfo);
            ValidationSingleControl(cboInfantResult, dxValidationProviderEditorInfo);
            ValidationcboHisBirthSertBook(cboHisBirthSertBook, dxValidationProviderEditorInfo);
            ValidationSingleControl(txtInfantBorntime, dxValidationProviderEditorInfo);
            ValidationSingleControl(cboInfantTybe, dxValidationProviderEditorInfo);
            ValidationSingleControl(cboInfantPosition, dxValidationProviderEditorInfo);
            ValidationSingleControl(spnInfantMonth, dxValidationProviderEditorInfo);
            ValidationSingleControl(txtInfantWeek, dxValidationProviderEditorInfo);
            ValidationSingleControl(cboEthnic, dxValidationProviderEditorInfo);
            ValidationSingleControl(spnInfantWeight, dxValidationProviderEditorInfo);
            ValidationSingleControl(dteIssue, dxValidationProviderEditorInfo);
            ValidationSingleControl(txtInfantName, dxValidationProviderEditorInfo);

            ValidationSingleControl(spnChildLive, dxValidationProviderEditorInfo);
            ValidationSingleControl(txtNumberChildrenBirth, dxValidationProviderEditorInfo);
            ValidationSingleControl(txtNumberOfBirth, dxValidationProviderEditorInfo);

            ValidationSingleControl(cboBirthPlaceType, dxValidationProviderEditorInfo);
            ValidationSingleControl(txtNoicap, dxValidationProviderEditorInfo);
            ValidationSingleControl(txtNgaycap, dxValidationProviderEditorInfo);
            ValidationSingleControl(txtAddress, dxValidationProviderEditorInfo);
            ValidateGridLookupWithTextEdit(cboProvinceName, txtProvinceCode, dxValidationProviderEditorInfo);
            ValidateGridLookupWithTextEdit(cboDistrictName, txtDistrictCode, dxValidationProviderEditorInfo);
            ValidateGridLookupWithTextEdit(cboCommuneName, txtCommuneCode, dxValidationProviderEditorInfo);
            ValidateGridLookupWithTextEdit(cboProvinceNameHospital, txtProvinceCodeHospital, dxValidationProviderEditorInfo);
            ValidateGridLookupWithTextEdit(cboDistrictNameHospital, txtDistrictCodeHospital, dxValidationProviderEditorInfo);
            ValidateGridLookupWithTextEdit(cboCommuneNameHospital, txtCommuneCodeHospital, dxValidationProviderEditorInfo);
            Validate.InfantValidationRule docDateRule = new Validate.InfantValidationRule();
            docDateRule.txtInfantMidwife1 = txtInfantMidwife1;
            docDateRule.txtInfantMidwife2 = txtInfantMidwife2;
            docDateRule.txtInfantMidwife3 = txtInfantMidwife3;
            dxValidationProviderEditorInfo.SetValidationRule(txtInfantMidwife1, docDateRule);

            ValidateGridLookupWithTextEdit(cboUserGCS, txtUserGCS, dxValidationProviderEditorInfo);
            CheckValidateMaxlengthCMNDCCCD(txtCMT, dxValidationProviderEditorInfo, true);

            CheckValidateMaxlength(this.txtAddress, 200, dxValidationProviderEditorInfo, true);
            CheckValidateMaxlength(txtNote, 4000, dxValidationProviderEditorInfo, true);
            CheckValidateTxtHeinCardTmp(this.txtHeinCardTmp, 15, dxValidationProviderEditorInfo);

            if (this.treatment.TDL_PATIENT_TYPE_ID == Config.HisConfigCFG.PatientTypeId__BHYT)
            {
                layoutControlItem64.AppearanceItemCaption.ForeColor = Color.Maroon;
                CheckValidateTxtHeinCardTmp(this.txtHeinCardTmp, 15, dxValidationProviderEditorInfo, true);
            }
        }
        private void CheckValidateMaxlengthCMNDCCCD(TextEdit txtCMT, DXValidationProvider dxValidationProviderEditor, bool isValid)
        {
            try
            {
                ValidateMaxlengthCMNDCCCD validRule = new ValidateMaxlengthCMNDCCCD();
                validRule.textEdit = txtCMT;
                validRule.ErrorType = ErrorType.Warning;
                validRule.isValid = isValid;
                dxValidationProviderEditor.SetValidationRule(txtCMT, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void CheckValidateMaxlengthCMNDCCCD(TextEdit txtCMT, DXValidationProvider dxValidationProviderEditor)
        //{
        //    try
        //    {
        //        ValidateMaxlengthCMNDCCCD validRule = new ValidateMaxlengthCMNDCCCD();
        //        validRule.textEdit = txtCMT;
        //        validRule.ErrorType = ErrorType.Warning;
        //        dxValidationProviderEditor.SetValidationRule(txtCMT, validRule);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}


        private void CheckValidateMaxlength(TextEdit txt, int maxLength, DXValidationProvider dxValidationProviderEditor, bool IsValid = false)
        {
            try
            {
                ValidateMaxlength validRule = new ValidateMaxlength();
                validRule.textEdit = txt;
                validRule.maxLength = maxLength;
                validRule.ErrorType = ErrorType.Warning;
                validRule.isValid = IsValid;
                dxValidationProviderEditor.SetValidationRule(txt, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void CheckValidateTxtHeinCardTmp(TextEdit txt, int maxLength, DXValidationProvider dxValidationProviderEditor, bool IsValid = false)
        {
            try
            {
                txtHeinCardTmpValidationRule validRule = new txtHeinCardTmpValidationRule();
                validRule.txtHeinCardTmp = txt;
                validRule.maxLength = maxLength;
                validRule.ErrorType = ErrorType.Warning;
                validRule.isValid = IsValid;
                dxValidationProviderEditor.SetValidationRule(txt, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        protected void ValidationcboHisBirthSertBook(BaseEdit control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                CboHisBirthSertBookValidationRule validRule = new CboHisBirthSertBookValidationRule();
                validRule.gridlookup = cboHisBirthSertBook;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void loadInfoMother()
        {
            try
            {
                if (patient != null)
                {
                    if (!String.IsNullOrWhiteSpace(patient.CMND_NUMBER))
                    {
                        txtCMT.Text = patient.CMND_NUMBER;
                        txtNgaycap.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patient.CMND_DATE ?? 0);
                        txtNoicap.Text = patient.CMND_PLACE;
                    }
                    else if (!String.IsNullOrWhiteSpace(patient.PASSPORT_NUMBER))
                    {
                        txtCMT.Text = patient.PASSPORT_NUMBER;
                        txtNgaycap.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patient.PASSPORT_DATE ?? 0);
                        txtNoicap.Text = patient.PASSPORT_PLACE;
                    }
                    else if (!String.IsNullOrWhiteSpace(patient.CCCD_NUMBER))
                    {
                        txtCMT.Text = patient.CCCD_NUMBER;
                        txtNgaycap.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patient.CCCD_DATE ?? 0);
                        txtNoicap.Text = patient.CCCD_PLACE;
                    }

                    HisPatientTypeAlterFilter filter = new HisPatientTypeAlterFilter();
                    filter.TDL_PATIENT_ID = patient.ID;
                    List<HIS_PATIENT_TYPE_ALTER> listPtTypeAlter = new BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/Get", ApiConsumers.MosConsumer, filter, null);
                    if (listPtTypeAlter != null && listPtTypeAlter.Count > 0)
                    {
                        txtAddress.Text = listPtTypeAlter.FirstOrDefault().ADDRESS;
                    }

                    LoadMotherAddress(patient);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                //this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(Inventec.Desktop.Common.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;

                ResetFormData();

                EnableControlChanged(this.ActionType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableControlChanged(int action)
        {
            try
            {
                btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetFormData()
        {
            if (!lcEditorInfo.IsInitialized) return;
            lcEditorInfo.BeginUpdate();
            try
            {
                foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditorInfo.Items)
                {
                    DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                    if (lci != layoutControlItem10 && lci != layoutControlItem26 && lci != layoutControlItem27 && lci != lciProvince && lci != lciProvinceName && lci != lciDistrict && lci != lciDistrictName && lci != lciCommune && lci != lciCommuneName && lci != lctAddress && lci != null && lci.Control != null && lci.Control is BaseEdit)
                    {
                        DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;

                        fomatFrm.ResetText();
                        fomatFrm.EditValue = null;
                        fomatFrm.Text = "";
                        //txtCMT.Text = "";
                        //txtNoicap.Text = "";
                        //txtNgaycap.EditValue = null;
                        spnInfantMonth.EditValue = null;
                        spnInfantHeight.EditValue = null;
                        spnInfantWeight.EditValue = null;
                        spnInfanthead.EditValue = null;
                        cboEthnic.EditValue = null;
                        spnChildLive.EditValue = null;
                        cboInfantPosition.EditValue = null;
                        cboInfantGendercode.Select();
                        cboInfantPosition.Select();
                        cboInfantResult.Select();
                        cboInfantTybe.Select();
                        cboInfantPosition.Select();
                        //if (patient != null)
                        //{
                        //    SetValueForMotherAddress(this.patient);
                        //}
                        //txtKey.Focus();
                    }
                }
                cboHisBirthSertBook.EditValue = null;
                lblHisBirthCertNum.Text = null;
                txtInfantName.Text = null;
                cboInfantGendercode.EditValue = null;
                cboInfantResult.EditValue = null;
                dtdInfantdate.EditValue = null;
                lciDeathDate.Enabled = false;
                dtDeathDate.EditValue = null;
                txtInfantBorntime.Text = null;
                cboInfantTybe.Text = null;
                txtInfantMonth.Text = null;
                txtInfantWeek.Text = null;
                txtFather.Text = null;
                txtInfantMidwife1.Text = null;
                txtInfantMidwife2.Text = null;
                txtInfantMidwife3.Text = null;
                txtUserGCS.Text = null;
                cboUserGCS.EditValue = null;
                txtNote.Text = null;
                cboDirectorUsername.EditValue = null;
                txtDirectorLoginname.Text = null;

                spnInfantMonth.EditValue = null;
                spnInfantHeight.EditValue = null;
                spnInfantWeight.EditValue = null;
                spnInfanthead.EditValue = null;
                cboEthnic.EditValue = null;
                spnChildLive.EditValue = null;
                cboInfantPosition.EditValue = null;


                var cboUser = BackendDataWorker.Get<ACS_USER>().Where(o => o.LOGINNAME == LoggingName).FirstOrDefault();
                cboUserGCS.EditValue = cboUser.LOGINNAME;
                txtUserGCS.Text = cboUser.LOGINNAME;
                //var cboDirectorUser = BackendDataWorker.Get<HIS_BRANCH>().Where(e => e.DIRECTOR_USERNAME != null && e.DIRECTOR_LOGINNAME != null).FirstOrDefault();
                cboDirectorUsername.EditValue = hisBranch.DIRECTOR_LOGINNAME;
                txtDirectorLoginname.Text = hisBranch.DIRECTOR_LOGINNAME;
                chkInfantcheck.CheckState = CheckState.Unchecked;
                chkIsBacterialContamination.CheckState = CheckState.Unchecked;
                chkIsDifficultBirth.CheckState = CheckState.Unchecked;
                chkIsFetalDeath22Weeks.CheckState = CheckState.Unchecked;
                ChkIsHaemorrhage.CheckState = CheckState.Unchecked;
                chkIsInjeckB.CheckState = CheckState.Unchecked;
                chkIsInjeckK1.CheckState = CheckState.Unchecked;
                chkIsMotherDeath.CheckState = CheckState.Unchecked;
                chkIsPuerperal.CheckState = CheckState.Unchecked;
                chkIsTetanus.CheckState = CheckState.Unchecked;
                chkIsUterineRupture.CheckState = CheckState.Unchecked;
                chkPostpartumCare2.CheckState = CheckState.Unchecked;
                chkPostpartumCare6.CheckState = CheckState.Unchecked;
                txtNumberChildrenBirth.EditValue = null;
                txtNumberOfPregnancies.Text = null;
                txtNumberOfBirth.EditValue = null;
                txtNumOfFullTermBirth.Text = null;
                txtNumOfPrematureBirth.Text = null;
                txtNumOfMiscarriage.Text = null;
                cboNumOfTest.EditValue = null;
                cboTestHiv.EditValue = null;
                cboTestSyphilis.EditValue = null;
                cboTestHepatitisB.EditValue = null;
                chkIsTestBloodSugar.CheckState = CheckState.Unchecked;
                chkIsEarlyNewBornCare.CheckState = CheckState.Unchecked;
                cboNewBornCareAtHome.EditValue = null;

                txtInfantName.Focus();

                chkIsSurgery.CheckState = CheckState.Unchecked;
                txtHeinCardTmp.Text = null;
                dteIssue.DateTime = DateTime.Now;
                cboBirthPlaceType.EditValue = null;
                cboBirthHospital.EditValue = null;
                txtProvinceCodeHospital.Text = null;
                cboProvinceNameHospital.EditValue = null;
                txtDistrictCodeHospital.Text = null;
                cboDistrictNameHospital.EditValue = null;
                txtCommuneCodeHospital.Text = null;
                cboCommuneNameHospital.EditValue = null;
                txtProvinceCodeHospital.Enabled = true;
                cboProvinceNameHospital.Enabled = true;
                txtDistrictCodeHospital.Enabled = true;
                cboDistrictNameHospital.Enabled = true;
                txtCommuneCodeHospital.Enabled = true;
                cboCommuneNameHospital.Enabled = true;
                lciBirthHospital.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                txtBirthPlace.Text = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                lcEditorInfo.EndUpdate();
            }
        }
        private void LoadTreatment()
        {
            try
            {
                if (this.treatmentId > 0)
                {
                    HisTreatmentFilter tFilter = new HisTreatmentFilter();
                    tFilter.ID = treatmentId;
                    List<HIS_TREATMENT> listTreatment = new BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, tFilter, null);
                    this.treatment = listTreatment != null ? listTreatment.FirstOrDefault() : null;
                    if (this.treatment != null)
                    {
                        HisPatientFilter pFilter = new HisPatientFilter();
                        pFilter.ID = this.treatment.PATIENT_ID;
                        List<HIS_PATIENT> listPatient = new BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, pFilter, null);
                        this.patient = listPatient != null ? listPatient.FirstOrDefault() : null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FillDataToGridControl()
        {
            try
            {
                WaitingManager.Show();
                int numPageSize;
                if (ucPaging.pagingGrid != null)
                {
                    numPageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param, numPageSize, this.gridControlFormList);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        /// <summary>
        /// Ham goi api lay du lieu phan trang
        /// </summary>
        /// <param name="param"></param>
        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_BABY>> apiResult = null;
                // Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT>> apiResult1 = null;

                HisBabyViewFilter filter = new HisBabyViewFilter();
                SetFilterNavBar(ref filter);
                gridviewFormList.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_BABY>>("api/HisBaby/GetView", ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.V_HIS_BABY>)apiResult.Data;
                    if (data != null)
                    {
                        gridviewFormList.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridviewFormList.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisBabyViewFilter filter)
        {
            try
            {
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.TREATMENT_ID = treatmentId;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.InfantInformation.Resources.Lang", typeof(HIS.Desktop.Plugins.InfantInformation.frmInfantInformation).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.chkInfantcheck.Properties.Caption = Inventec.Common.Resource.Get.Value("frmInfantInformation.chkInfantcheck.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.btndelete.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.btnsave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboInfantPosition.Properties.NullText = Inventec.Common.Resource.Get.Value("frmInfantInformation.cboInfantPosition.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboInfantTybe.Properties.NullText = Inventec.Common.Resource.Get.Value("frmInfantInformation.cboInfantTybe.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboInfantGendercode.Properties.NullText = Inventec.Common.Resource.Get.Value("frmInfantInformation.cboInfantGendercode.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboInfantResult.Properties.NullText = Inventec.Common.Resource.Get.Value("frmInfantInformation.cboInfantResult.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.cboHisBirthSertBook.Properties.NullText = Inventec.Common.Resource.Get.Value("frmInfantInformation.cboHisBirthSertBook.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHisBirthSertBook.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.lciHisBirthSertBook.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHisBirthSertBookID.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.lciHisBirthSertBookID.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());


                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.layoutControlItem18.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem19.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.layoutControlItem19.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem22.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.layoutControlItem22.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem26.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.layoutControlItem26.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem33.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.layoutControlItem33.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem34.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.layoutControlItem34.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem27.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.layoutControlItem27.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmInfantInformation.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("frmInfantInformation.barButtonItem2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciProvince.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.lctProvince.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDistrict.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.lctDistrict.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCommune.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.lctCommune.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lctAddress.Text = Inventec.Common.Resource.Get.Value("frmInfantInformation.lctAddress.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_BABY pData = (MOS.EFMODEL.DataModels.V_HIS_BABY)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    //MOS.EFMODEL.DataModels.V_HIS_PATIENT cData = ( MOS.EFMODEL.DataModels.V_HIS_PATIENT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    //var cData = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_PATIENT>()
                    //    .Where(o => o.).FirstOrDefault();
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        try
                        {
                            if (status == 1)
                                e.Value = "Hoạt động";
                            else
                                e.Value = "Tạm khóa";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "IsEnough")
                    {
                        try
                        {
                            if (pData.MONTH_COUNT >= 9 && pData.WEEK_COUNT >= 40)
                                e.Value = "x";
                            else
                                e.Value = "";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "BORN_DATE_STR" && pData.BORN_TIME.HasValue)
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(pData.BORN_TIME ?? 0);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "BORN_TIME_STR" && pData.BORN_TIME.HasValue)
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.BORN_TIME ?? 0).Substring(11, 8);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    //else if (e.Column.FieldName == "CMND_DATE_CMND" && cData.CMND_DATE.HasValue)
                    //{
                    //    try
                    //    {
                    //        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(cData.CMND_DATE ?? 0);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Inventec.Common.Logging.LogSystem.Error(ex);
                    //    }

                    //}
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.CREATE_TIME);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    //HIS_CERT_BOOK_STR
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.MODIFY_TIME);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "gridColumnUserGCS")
                    {
                        try
                        {
                            e.Value = pData.ISSUER_LOGINNAME + " - " + pData.ISSUER_USERNAME;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }

                gridControlFormList.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    V_HIS_BABY data = (V_HIS_BABY)gridviewFormList.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "isLock")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? btnGLock : btnGunLock);
                    }
                    if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGEdit : repositoryItemButtonEdit1);

                    }
                    if (e.Column.FieldName == "PRINT")
                    {
                        e.RepositoryItem = repositoryItemButtonPrint;

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void gridControlFormList_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_BABY)gridviewFormList.GetFocusedRow();

                if (rowData != null)
                {
                    currentData = rowData;
                    loadInfoMother();

                    ChangedDataRow(rowData);
                    //Set focus vào control editor đầu tiên
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangedDataRow(MOS.EFMODEL.DataModels.V_HIS_BABY data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    LoadInfoComplementFromTreatment(this.treatment);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            CommonParam param = new CommonParam();
            HIS_BABY success = new HIS_BABY();
            bool rs = false;
            //bool notHandler = false;
            try
            {

                V_HIS_BABY data = (V_HIS_BABY)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_BABY data1 = new HIS_BABY();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_BABY>("api/HisBaby/ChangeLock", ApiConsumers.MosConsumer, data1.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        FillDataToGridControl();
                        rs = true;
                    }
                    MessageManager.Show(this, param, rs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnGunLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_BABY success = new HIS_BABY();
            bool rs = false;
            //bool notHandler = false;
            try
            {

                V_HIS_BABY data = (V_HIS_BABY)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_BABY data1 = new HIS_BABY();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_BABY>("api/HisBaby/ChangeLock", ApiConsumers.MosConsumer, data1.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        FillDataToGridControl();
                        rs = true;
                    }
                    MessageManager.Show(this, param, rs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    CommonParam param = new CommonParam();
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_BABY)gridviewFormList.GetFocusedRow();
                    HisBabyViewFilter filter = new HisBabyViewFilter();
                    filter.ID = rowData.ID;
                    var data = new BackendAdapter(param).Get<List<HIS_BABY>>("api/HisBaby/GetView", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();

                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>("api/HisBaby/Delete", ApiConsumers.MosConsumer, data.ID, param);
                        if (success)
                        {
                            FillDataToGridControl();
                            currentData = ((List<V_HIS_BABY>)gridControlFormList.DataSource).FirstOrDefault();
                            SetDefaultValue();
                        }
                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientgioitinh_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                //    if (cboInfantGendercode.EditValue != null)
                //    {
                //        HIS_GENDER gender = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_GENDER>()
                //            .Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboInfantGendercode.EditValue.ToString())).FirstOrDefault();
                //        if (gender != null)
                //        {
                //            txtInfantGendercode.Text = gender.GENDER_CODE;
                // cboInfantResult.Focus();
                // cboInfantResult.SelectAll();
                //        }
                //    }
                //    //else
                //    //{
                //    //    cboInfantResult.Focus();
                //    //    //cboInfantResult.ShowPopup();
                //    //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public bool IsNumber(string pText)
        {
            Regex regex = new Regex(@"^\d+$");
            Inventec.Common.Logging.LogSystem.Debug("regex.IsMatch(pText)_" + regex.IsMatch(pText));
            return regex.IsMatch(pText);
        }

        private void SaveProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                this.isUpdatedPatient = true;
                bool success = false;
                //if (!btnEdit.Enabled)
                //    return;
                if (lciDeathDate.Enabled)
                {
                    ValidationSingleControl(dtDeathDate, dxValidationProviderEditorInfo);
                }
                else
                {
                    dxValidationProviderEditorInfo.SetValidationRule(dtDeathDate, null);
                }
                if (this.lciBirthHospital.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    ValidationSingleControl(cboBirthHospital, dxValidationProviderEditorInfo);
                }
                else
                {
                    dxValidationProviderEditorInfo.SetValidationRule(cboBirthHospital, null);
                }
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                if (!dxValidationProviderEditorInfo.Validate())
                    return;

               
                HisBabySDO updateDTO = new HisBabySDO();

                UpdateDTOFromDataForm(ref updateDTO);
                if (dxErrorProvider1.HasErrors) return;
                WaitingManager.Show();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => updateDTO), updateDTO));
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    Inventec.Common.Logging.LogSystem.Debug("gọi Create");
                    var resultData = new BackendAdapter(param).Post<HisBabySDO>("api/HisBaby/Create", ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        LoadTreatment();
                        FillDataToGridControl();
                        btnxoa_Click(null, null);
                    }
                }
                else
                {
                    if (babyid > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("gọi Update");
                        updateDTO.Id = babyid;
                        var resultData = new BackendAdapter(param).Post<HisBabySDO>("api/HisBaby/Update", ApiConsumers.MosConsumer, updateDTO, param);
                        if (resultData != null)
                        {
                            success = true;
                            LoadTreatment();
                            FillDataToGridControl();
                        }
                    }


                }

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion



                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool IsValid(string txtCMND)
        {
            bool valid = false;
            var txt = txtCMND;
            int countNumber = 0;
            int total = txt.Length;
            for (int i = 0; i < txt.Length; i++)
            {
                if (IsNumber(txt[i].ToString()))
                {
                    countNumber++;
                }
            }
            if (countNumber == 0)
            {
                valid = false;
            }
            else if (countNumber != 0 && countNumber < total)
            {
                valid = true;
            }
            return valid;
        }

        private bool CheckIsNumber(string pValue)
        {
            foreach (Char c in pValue)
            {
                if (!Char.IsDigit(c))
                    return false;
            }
            return true;
        }

        private void LoadCurrent(long currentId, MOS.EFMODEL.DataModels.HIS_BABY currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();

                HisBabyFilter filter = new HisBabyFilter();

                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_BABY>>("api/HisBaby/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void getInfo(long treatmentID)
        {
            try
            {
                //getBaby = new HIS_BABY();
                //CommonParam param = new CommonParam();
                //HisBabyFilter filter = new HisBabyFilter();
                //filter.TREATMENT_ID = treatmentID;

                //var baby = new BackendAdapter(new CommonParam())
                //    .Get<List<MOS.EFMODEL.DataModels.HIS_BABY>>("api/HisBaby/Get", ApiConsumers.MosConsumer, filter, new CommonParam()).FirstOrDefault();
                //if (baby != null)
                //    getBaby = baby;

                getBaby = new HIS_BABY();
                CommonParam param = new CommonParam();
                HisBabyFilter filter = new HisBabyFilter();
                filter.TREATMENT_ID = treatmentID;

                var baby = new BackendAdapter(new CommonParam())
                    .Get<List<MOS.EFMODEL.DataModels.HIS_BABY>>("api/HisBaby/Get", ApiConsumers.MosConsumer, filter, new CommonParam()).FirstOrDefault();
                if (baby != null)
                    getBaby = baby;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnxoa_Click(object sender, EventArgs e)
        {
            try
            {
                currentData = null;
                dxErrorProvider1.ClearErrors();
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                if (isUpdatedPatient)
                {
                    HisPatientFilter patientFilter = new HisPatientFilter();
                    patientFilter.ID = patient.ID;
                    var listRefreshPatient = new BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, patientFilter, null);
                    if (listRefreshPatient != null && listRefreshPatient.Count > 0)
                    {
                        this.patient = listRefreshPatient.FirstOrDefault();
                    }
                }
                LoadMotherAddress(this.patient);
                loadInfoMother();
                ResetFormData();
                LoadInfoComplementFromTreatment(this.treatment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbocachsinh_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            //try
            //{
            //    if (e.CloseMode == PopupCloseMode.Normal)
            //    {
            //        cboInfantPosition.Focus();
            //    }
            //}

            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void cbongoithai_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    this.spnInfantMonth.Focus();
                    this.spnInfantMonth.SelectAll();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (!this.btnCancel.Enabled)
                    return;
                btnxoa_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void gridLookUpEdit1_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                    cboEthnic.EditValue = null;
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
                cboEthnic.Properties.Buttons[1].Visible = cboEthnic.EditValue != null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridviewFormList.Focus();
                    gridviewFormList.FocusedRowHandle = 0;
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_BABY)gridviewFormList.GetFocusedRow();

                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnAdd_Click(null, null);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
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
                    //if (String.IsNullOrEmpty(cboEthnic.Text))
                    //    cboEthnic.ShowPopup();
                    //else
                    {
                        spnInfantWeight.Focus();
                        spnInfantWeight.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonPrint_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                V_HIS_BABY printBaby = (V_HIS_BABY)gridviewFormList.GetFocusedRow();
                if (printBaby != null)
                {
                    Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    store.RunPrintTemplate("Mps000308", InPhieuChungSinh);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private bool InPhieuChungSinh(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                V_HIS_BABY printBaby = (V_HIS_BABY)gridviewFormList.GetFocusedRow();

                HisTreatmentViewFilter tFilter = new HisTreatmentViewFilter();
                tFilter.ID = printBaby.TREATMENT_ID;
                var printTreatment = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, tFilter, null).FirstOrDefault();

                HisPatientFilter pFilter = new HisPatientFilter();
                pFilter.ID = printTreatment.PATIENT_ID;
                List<HIS_PATIENT> listPatient = new BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, pFilter, null);
                this.patient = listPatient != null ? listPatient.FirstOrDefault() : null;

                if (printBaby != null)
                {
                    WaitingManager.Show();
                    HIS_PATIENT_TYPE_ALTER patientTypeAlter = null;
                    if (this.treatment != null)
                    {
                        CommonParam param = new CommonParam();
                        HisPatientTypeAlterFilter filter = new HisPatientTypeAlterFilter();
                        filter.IS_ACTIVE = 1;
                        filter.TREATMENT_ID = this.treatment.ID;
                        var lstPatientTypeAlter = new BackendAdapter(param).Get<List<HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/Get", ApiConsumers.MosConsumer, filter, param);
                        if (lstPatientTypeAlter != null && lstPatientTypeAlter.Count > 0)
                        {
                            patientTypeAlter = lstPatientTypeAlter.FirstOrDefault();
                        }
                    }
                    MPS.Processor.MPS000308.PDO.Mps000308PDO pdo = new MPS.Processor.MPS000308.PDO.Mps000308PDO(printTreatment, printBaby, this.patient, dataTotal) { _PatientTypeAlter = patientTypeAlter };

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : ""), printTypeCode, currentModuleBase.RoomId);

                    MPS.ProcessorBase.Core.PrintData printdata = null;
                    if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        printdata = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        printdata = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                    }
                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(printdata);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }


        private void cboHisBirthSertBook_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                LoadComboHisBirthCertBook();
                if (currentData != null && currentData.BIRTH_CERT_BOOK_ID.HasValue && cboHisBirthSertBook.EditValue == null && lstBirthCertBook.FirstOrDefault(o => o.ID == (currentData.BIRTH_CERT_BOOK_ID ?? 0)) != null)
                    cboHisBirthSertBook.EditValue = currentData.BIRTH_CERT_BOOK_ID;
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboHisBirthSertBook.EditValue != null)
                    {
                        txtInfantName.Focus();
                        txtInfantName.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboUserGCS_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboUserGCS.EditValue != null)
                    {
                        var dataCboGCS = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                        dataCboGCS = dataCboGCS.Where(o => o.IS_ACTIVE == 1).ToList();
                        var data = dataCboGCS.FirstOrDefault(o => o.LOGINNAME == cboUserGCS.EditValue.ToString());
                        if (data != null)
                        {
                            txtUserGCS.Text = data.LOGINNAME;
                        }
                        txtHeinCardTmp.Focus();
                        txtHeinCardTmp.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboHisBirthSertBook_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtInfantName.Focus();
                    txtInfantName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtInfantName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboInfantGendercode.Focus();
                    cboInfantGendercode.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboInfantGendercode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboInfantResult.Focus();
                    cboInfantResult.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboInfantResult_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtdInfantdate.Focus();
                    dtdInfantdate.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtdInfantdate_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //if (dtdInfantdate.EditValue != null)
                    //{
                    //    txtInfantBorntime.Focus();
                    //    txtInfantBorntime.SelectAll();
                    //}
                    txtInfantBorntime.Focus();
                    txtInfantBorntime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtInfantBorntime_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboInfantTybe.Focus();
                    cboInfantTybe.SelectAll();
                    //if (lciDeathDate.Enabled)
                    //{
                    //    dtDeathDate.Focus();
                    //    dtDeathDate.ShowPopup();
                    //}
                    //else
                    //{
                    //    cboInfantTybe.Focus();
                    //    cboInfantTybe.SelectAll();
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboInfantTybe_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //if (String.IsNullOrEmpty(cboInfantTybe.Text))

                    //    cboInfantTybe.ShowPopup();
                    //else
                    cboInfantPosition.Focus();
                    cboInfantPosition.SelectAll();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboInfantPosition_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //if (String.IsNullOrEmpty(cboInfantPosition.Text))
                    //    cboInfantPosition.ShowPopup();
                    //else
                    {
                        this.spnInfantMonth.Focus();
                        spnInfantMonth.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spnInfantMonth_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnChildLive.Focus();
                    spnChildLive.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void txtInfantMonth_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtInfantWeek.Focus();
                    txtInfantWeek.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtInfantWeek_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNote.Focus();
                    txtNote.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtFather_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtHeinCardTmp.Focus();
                    txtHeinCardTmp.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtInfantMidwife1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtInfantMidwife2.Focus();
                    txtInfantMidwife2.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtInfantMidwife2_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtInfantMidwife3.Focus();
                    txtInfantMidwife3.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtInfantMidwife3_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtUserGCS.Focus();
                    txtUserGCS.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtUserGCS_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void cboUserGCS_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboUserGCS.EditValue != null)
                    {
                        var dataCboGCS = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                        var data = dataCboGCS.FirstOrDefault(o => o.ID == ((long)cboUserGCS.EditValue));
                        if (data != null)
                        {
                            txtUserGCS.Text = data.LOGINNAME;
                         
                        }
                    }
                    dteIssue.Focus();
                    dteIssue.ShowPopup();
                    
                }
                else
                {
                    cboUserGCS.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtCMT_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNgaycap.Focus();
                    txtNgaycap.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtUserGCS_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtUserGCS.Text.Trim()))
                    {
                        string code = txtUserGCS.Text.Trim().ToLower();

                        var dataCboGCS = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                        var data = dataCboGCS.Where(o => o.LOGINNAME.ToLower().Contains(code)).ToList();
                        var result = dataCboGCS != null ? (dataCboGCS.Count > 1 ? dataCboGCS.Where(o => o.LOGINNAME.ToLower() == code).ToList() : dataCboGCS) : null;
                        if (result != null && result.Count > 0)
                        {
                            showCbo = false;
                            txtUserGCS.Text = result.First().LOGINNAME;
                            cboUserGCS.Text = result.Select(o => o.USERNAME).FirstOrDefault();
                            cboUserGCS.EditValue = result.Select(o => o.LOGINNAME).FirstOrDefault();
                            //cboUserGCS.Properties.Buttons[1].Visible = true;
                            //txtCMT.Focus();
                            //txtCMT.SelectAll();
                        }
                    }
                    if (showCbo)
                    {
                        cboUserGCS.Focus();
                        cboUserGCS.ShowPopup();
                    }
                    dteIssue.Focus();
                    dteIssue.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNgaycap_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNoicap.Focus();
                    txtNoicap.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNoicap_KeyUp(object sender, KeyEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //if (btnAdd.Enabled)
                    //{
                    //    btnAdd.Focus();
                    //    btnAdd.Select();
                    //}
                    //else
                    //{
                    //    btnEdit.Focus();
                    //    btnEdit.Select();
                    //}
                    txtProvinceCode.Focus();
                    txtProvinceCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spInfantWeight_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnInfanthead.Focus();
                    spnInfanthead.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spInfanthead_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    spnInfantHeight.Focus();
                    spnInfantHeight.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spInfantHeight_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtFather.Focus();
                    txtFather.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkInfantcheck_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsSurgery.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spInfantMonth_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnChildLive.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkInfantcheck_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkInfantcheck.Checked == true)
                {
                    txtInfantMonth.Text = "9";
                    txtInfantWeek.Text = "40";
                    txtInfantWeek.Enabled = false;
                    txtInfantMonth.Enabled = false;
                    cboEthnic.Focus();
                    cboEthnic.SelectAll();
                }
                else
                {
                    txtInfantWeek.Enabled = true;
                    txtInfantMonth.Enabled = true;
                    txtInfantMonth.Text = "";
                    txtInfantWeek.Text = "";
                    txtInfantMonth.Focus();
                    txtInfantMonth.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinChildLive_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //chkInfantcheck.Properties.FullFocusRect = true;
                    //chkInfantcheck.Focus();
                    txtNumberChildrenBirth.Focus();
                    txtNumberChildrenBirth.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinChildLive_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == '\r')
                {
                    spnChildLive.Focus();
                    spnChildLive.SelectAll();
                }
                else if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtCMT_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                //if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                //{
                //    e.Handled = true;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNgaycap_KeyPress(object sender, KeyPressEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNgaycap_EditValueChanged(object sender, EventArgs e)
        {

            try
            {
                if (txtNgaycap.DateTime < DateTime.Now.AddYears(-2000))
                {
                    txtNgaycap.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void txtProvinceCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.LoadComboTinhThanh((sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper(), true);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboProvinceName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboProvinceName.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = listProvince.SingleOrDefault(o => o.PROVINCE_CODE == this.cboProvinceName.EditValue.ToString());
                        if (province != null)
                        {
                            this.LoadComboHuyen("", province.PROVINCE_CODE, false);
                            this.txtProvinceCode.Text = province.SEARCH_CODE;
                            this.txtDistrictCode.Text = "";
                        }
                    }
                    FocusToDistrict();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProvinceName_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                cboProvinceName.Properties.DataSource = listProvinceTemp;
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboProvinceName.EditValue != null
                        && this.cboProvinceName.EditValue != this.cboProvinceName.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = listProvince.SingleOrDefault(o => o.PROVINCE_CODE == cboProvinceName.EditValue.ToString());
                        if (province != null)
                        {
                            this.LoadComboHuyen("", province.PROVINCE_CODE, false);
                            this.txtProvinceCode.Text = province.SEARCH_CODE;
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

        private void txtDistrictCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string provinceCode = "";
                    if (this.cboProvinceName.EditValue != null)
                    {
                        provinceCode = this.cboProvinceName.EditValue.ToString();
                    }
                    this.LoadComboHuyen((sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper(), provinceCode, true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistrictName_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                cboDistrictName.Properties.DataSource = listDistrictTemp;
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboDistrictName.EditValue != null
                        && this.cboDistrictName.EditValue != this.cboDistrictName.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = listDistrict.SingleOrDefault(o => o.DISTRICT_CODE == this.cboDistrictName.EditValue.ToString()
                                && (String.IsNullOrEmpty((this.cboProvinceName.EditValue ?? "").ToString()) || o.PROVINCE_CODE == (this.cboProvinceName.EditValue ?? "").ToString()));
                        if (district != null)
                        {
                            if (String.IsNullOrEmpty((this.cboProvinceName.EditValue ?? "").ToString()))
                            {
                                this.cboProvinceName.EditValue = district.PROVINCE_CODE;
                                SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = listProvince.SingleOrDefault(o => o.PROVINCE_CODE == district.PROVINCE_CODE.ToString());
                                if (province != null)
                                {
                                    this.txtProvinceCode.Text = province.SEARCH_CODE;
                                }
                                else
                                {
                                    this.txtProvinceCode.Text = district.PROVINCE_CODE;
                                }
                            }
                            this.LoadComboXa("", district.DISTRICT_CODE, false);
                            this.txtDistrictCode.Text = district.SEARCH_CODE;
                            this.cboDistrictName.EditValue = district.DISTRICT_CODE;
                            this.cboCommuneName.EditValue = null;
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

        private void cboDistrictName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboDistrictName.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = listDistrict.SingleOrDefault(o => o.DISTRICT_CODE == this.cboDistrictName.EditValue.ToString()
                               && (String.IsNullOrEmpty((this.cboProvinceName.EditValue ?? "").ToString()) || o.PROVINCE_CODE == (this.cboProvinceName.EditValue ?? "").ToString()));
                        if (district != null)
                        {
                            if (String.IsNullOrEmpty((this.cboProvinceName.EditValue ?? "").ToString()))
                            {
                                this.cboProvinceName.EditValue = district.PROVINCE_CODE;
                            }
                            this.LoadComboXa("", district.DISTRICT_CODE, false);
                            this.txtDistrictCode.Text = district.SEARCH_CODE;
                            this.cboCommuneName.EditValue = null;
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
                    if (this.cboDistrictName.EditValue != null)
                    {
                        districtCode = this.cboDistrictName.EditValue.ToString();
                    }
                    this.LoadComboXa((sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper(), districtCode, true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCommuneName_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                cboCommuneName.Properties.DataSource = listCommuneTemp;
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboCommuneName.EditValue != null
                        && this.cboCommuneName.EditValue != cboCommuneName.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = listCommune.SingleOrDefault(o => o.COMMUNE_CODE == this.cboCommuneName.EditValue.ToString()
                                    && (String.IsNullOrEmpty((this.cboDistrictName.EditValue ?? "").ToString()) || o.DISTRICT_CODE == (this.cboDistrictName.EditValue ?? "").ToString())
                                );
                        if (commune != null)
                        {
                            this.txtCommuneCode.Text = commune.SEARCH_CODE;
                            if (String.IsNullOrEmpty((this.cboProvinceName.EditValue ?? "").ToString()) && String.IsNullOrEmpty((this.cboDistrictName.EditValue ?? "").ToString()))
                            {
                                this.cboDistrictName.EditValue = commune.DISTRICT_CODE;
                                SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = listDistrict.Where(o => o.ID == commune.DISTRICT_ID).FirstOrDefault();
                                if (district != null)
                                {
                                    this.txtDistrictCode.Text = district.SEARCH_CODE;
                                    this.cboProvinceName.EditValue = district.PROVINCE_CODE;
                                    SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = listProvince.SingleOrDefault(o => o.PROVINCE_CODE == district.PROVINCE_CODE.ToString());
                                    if (province != null)
                                    {
                                        this.txtProvinceCode.Text = province.SEARCH_CODE;
                                    }
                                    else
                                    {
                                        this.txtProvinceCode.Text = district.PROVINCE_CODE;
                                    }
                                    this.cboProvinceName.EditValue = district.PROVINCE_CODE;
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

        private void cboCommuneName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboCommuneName.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = listCommune.SingleOrDefault(o => o.COMMUNE_CODE == this.cboCommuneName.EditValue.ToString()
                                && (String.IsNullOrEmpty((this.cboDistrictName.EditValue ?? "").ToString()) || o.DISTRICT_CODE == (this.cboDistrictName.EditValue ?? "").ToString()));
                        if (commune != null)
                        {
                            this.txtCommuneCode.Text = commune.SEARCH_CODE;
                            if (String.IsNullOrEmpty((this.cboProvinceName.EditValue ?? "").ToString()) && String.IsNullOrEmpty((this.cboDistrictName.EditValue ?? "").ToString()))
                            {
                                this.cboDistrictName.EditValue = commune.DISTRICT_CODE;
                                SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = listDistrict.Where(o => o.ID == commune.DISTRICT_ID).FirstOrDefault();
                                if (district != null)
                                {
                                    this.cboProvinceName.EditValue = district.PROVINCE_CODE;
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

        private void lookUpEdit1_Properties_GetNotInListValue(object sender, GetNotInListValueEventArgs e)
        {
            try
            {
                if (e.FieldName == "RENDERER_PROVINCE_NAME")
                {
                    var item = ((List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>)this.cboProvinceName.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0} {1}", "", item.PROVINCE_NAME);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCommuneName_Properties_GetNotInListValue(object sender, GetNotInListValueEventArgs e)
        {
            try
            {
                if (e.FieldName == "RENDERER_COMMUNE_NAME")
                {
                    var item = ((List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>)this.cboCommuneName.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0} {1}", item.INITIAL_NAME, item.COMMUNE_NAME);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistrictName_Properties_GetNotInListValue(object sender, GetNotInListValueEventArgs e)
        {
            try
            {
                if (e.FieldName == "RENDERER_DISTRICT_NAME")
                {
                    var item = ((List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>)this.cboDistrictName.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0} {1}", item.INITIAL_NAME, item.DISTRICT_NAME);
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
                    this.cboProvinceName.Properties.DataSource = listProvince;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtNumberOfPregnancies_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPostpartumCare2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkPostpartumCare2.Checked)
                    chkPostpartumCare6.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPostpartumCare6_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkPostpartumCare6.Checked)
                    chkPostpartumCare2.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNumOfFullTermBirth_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNumOfPrematureBirth_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNumOfMiscarriage_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNumOfTest_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboNumOfTest.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTestHiv_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTestHiv.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTestSyphilis_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTestSyphilis.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTestHepatitisB_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTestHepatitisB.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboNewBornCareAtHome_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboNewBornCareAtHome.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtHTProvinceCode_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(this.txtHTProvinceCode.Text))
                {
                    this.cboHTProvinceName.Properties.DataSource = listProvince;
                    this.cboHTProvinceName.Text = "";
                    this.cboHTDistrictName.Text = "";
                    this.txtHTDistrictCode.Text = "";
                    this.cboHTCommuneName.Text = "";
                    this.txtHTCommuneCode.Text = "";
                    this.cboHTProvinceName.EditValue = null;
                    this.cboHTDistrictName.Properties.DataSource = null;
                    this.cboHTCommuneName.Properties.DataSource = null;
                }

            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtHTProvinceCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtHTProvinceCode.Text))
                {
                    cboHTProvinceName.Text = "";
                }
                if (e.KeyCode == Keys.Enter)
                {
                    this.LoadComboTinhThanh_HT((sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper(), true);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtHTDistrictCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string provinceCode = "";
                    if (this.cboHTProvinceName.EditValue != null)
                    {
                        provinceCode = this.cboHTProvinceName.EditValue.ToString();
                    }
                    this.LoadComboHuyen_HT((sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper(), provinceCode, true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHTCommuneCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string districtCode = "";
                    if (this.cboHTDistrictName.EditValue != null)
                    {
                        districtCode = this.cboHTDistrictName.EditValue.ToString();
                    }
                    this.LoadComboXa_HT((sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper(), districtCode, true);
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
                cboHTProvinceName.Properties.DataSource = listProvinceTemp;
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboHTProvinceName.EditValue != null
                        && this.cboHTProvinceName.EditValue != this.cboHTProvinceName.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = listProvince.SingleOrDefault(o => o.PROVINCE_CODE == cboHTProvinceName.EditValue.ToString());
                        if (province != null)
                        {
                            this.LoadComboHuyen_HT("", province.PROVINCE_CODE, false);
                            this.txtHTProvinceCode.Text = province.SEARCH_CODE;
                        }
                    }
                    this.txtHTDistrictCode.Text = "";
                    FocusToDistrict_HT();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHTProvinceName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboHTProvinceName.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = listProvince.SingleOrDefault(o => o.PROVINCE_CODE == this.cboHTProvinceName.EditValue.ToString());
                        if (province != null)
                        {
                            this.LoadComboHuyen_HT("", province.PROVINCE_CODE, false);
                            this.txtHTProvinceCode.Text = province.SEARCH_CODE;
                            this.txtHTDistrictCode.Text = "";
                        }
                    }
                    FocusToDistrict_HT();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHTDistrictName_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                cboHTDistrictName.Properties.DataSource = listDistrictTemp;
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboHTDistrictName.EditValue != null
                        && this.cboHTDistrictName.EditValue != this.cboHTDistrictName.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = listDistrict.SingleOrDefault(o => o.DISTRICT_CODE == this.cboHTDistrictName.EditValue.ToString()
                                && (String.IsNullOrEmpty((this.cboHTProvinceName.EditValue ?? "").ToString()) || o.PROVINCE_CODE == (this.cboHTProvinceName.EditValue ?? "").ToString()));
                        if (district != null)
                        {
                            if (String.IsNullOrEmpty((this.cboHTProvinceName.EditValue ?? "").ToString()))
                            {
                                this.cboHTProvinceName.EditValue = district.PROVINCE_CODE;
                                SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = listProvince.SingleOrDefault(o => o.PROVINCE_CODE == district.PROVINCE_CODE.ToString());
                                if (province != null)
                                {
                                    this.txtHTProvinceCode.Text = province.SEARCH_CODE;
                                }
                                else
                                {
                                    this.txtHTProvinceCode.Text = district.PROVINCE_CODE;
                                }
                            }
                            this.LoadComboXa_HT("", district.DISTRICT_CODE, false);
                            this.txtHTDistrictCode.Text = district.SEARCH_CODE;
                            this.cboHTDistrictName.EditValue = district.DISTRICT_CODE;
                            this.cboHTCommuneName.EditValue = null;
                            this.txtHTCommuneCode.Text = "";
                        }
                    }
                    FocusToCommune_HT();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHTDistrictName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboHTDistrictName.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = listDistrict.SingleOrDefault(o => o.DISTRICT_CODE == this.cboHTDistrictName.EditValue.ToString()
                               && (String.IsNullOrEmpty((this.cboHTProvinceName.EditValue ?? "").ToString()) || o.PROVINCE_CODE == (this.cboHTProvinceName.EditValue ?? "").ToString()));
                        if (district != null)
                        {
                            if (String.IsNullOrEmpty((this.cboHTProvinceName.EditValue ?? "").ToString()))
                            {
                                this.cboHTProvinceName.EditValue = district.PROVINCE_CODE;
                            }
                            this.LoadComboXa_HT("", district.DISTRICT_CODE, false);
                            this.txtHTDistrictCode.Text = district.SEARCH_CODE;
                            this.cboHTCommuneName.EditValue = null;
                            this.txtHTCommuneCode.Text = "";
                        }
                    }
                    FocusToCommune_HT();
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
                cboHTCommuneName.Properties.DataSource = listCommuneTemp;
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboHTCommuneName.EditValue != null
                        && this.cboHTCommuneName.EditValue != cboHTCommuneName.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = listCommune.SingleOrDefault(o => o.COMMUNE_CODE == this.cboHTCommuneName.EditValue.ToString()
                                    && (String.IsNullOrEmpty((this.cboHTDistrictName.EditValue ?? "").ToString()) || o.DISTRICT_CODE == (this.cboHTDistrictName.EditValue ?? "").ToString())
                                );
                        if (commune != null)
                        {
                            this.txtHTCommuneCode.Text = commune.SEARCH_CODE;
                            if (String.IsNullOrEmpty((this.cboHTProvinceName.EditValue ?? "").ToString()) && String.IsNullOrEmpty((this.cboHTDistrictName.EditValue ?? "").ToString()))
                            {
                                this.cboHTDistrictName.EditValue = commune.DISTRICT_CODE;
                                SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = listDistrict.Where(o => o.ID == commune.DISTRICT_ID).FirstOrDefault();
                                if (district != null)
                                {
                                    this.txtHTDistrictCode.Text = district.SEARCH_CODE;
                                    this.cboHTProvinceName.EditValue = district.PROVINCE_CODE;
                                    SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = listProvince.SingleOrDefault(o => o.PROVINCE_CODE == district.PROVINCE_CODE.ToString());
                                    if (province != null)
                                    {
                                        this.txtHTProvinceCode.Text = province.SEARCH_CODE;
                                    }
                                    else
                                    {
                                        this.txtHTProvinceCode.Text = district.PROVINCE_CODE;
                                    }
                                    this.cboHTProvinceName.EditValue = district.PROVINCE_CODE;
                                }
                            }
                        }
                    }
                    FocusToAddress_HT();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHTCommuneName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboHTCommuneName.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = listCommune.SingleOrDefault(o => o.COMMUNE_CODE == this.cboHTCommuneName.EditValue.ToString()
                                && (String.IsNullOrEmpty((this.cboHTDistrictName.EditValue ?? "").ToString()) || o.DISTRICT_CODE == (this.cboHTDistrictName.EditValue ?? "").ToString()));
                        if (commune != null)
                        {
                            this.txtHTCommuneCode.Text = commune.SEARCH_CODE;
                            if (String.IsNullOrEmpty((this.cboHTProvinceName.EditValue ?? "").ToString()) && String.IsNullOrEmpty((this.cboHTDistrictName.EditValue ?? "").ToString()))
                            {
                                this.cboHTDistrictName.EditValue = commune.DISTRICT_CODE;
                                SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = listDistrict.Where(o => o.ID == commune.DISTRICT_ID).FirstOrDefault();
                                if (district != null)
                                {
                                    this.cboHTProvinceName.EditValue = district.PROVINCE_CODE;
                                }
                            }
                        }
                    }
                    FocusToAddress_HT();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHTDistrictName_Properties_GetNotInListValue(object sender, GetNotInListValueEventArgs e)
        {
            try
            {
                if (e.FieldName == "RENDERER_DISTRICT_NAME")
                {
                    var item = ((List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>)this.cboHTDistrictName.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0} {1}", item.INITIAL_NAME, item.DISTRICT_NAME);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHTCommuneName_Properties_GetNotInListValue(object sender, GetNotInListValueEventArgs e)
        {
            try
            {
                if (e.FieldName == "RENDERER_COMMUNE_NAME")
                {
                    var item = ((List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>)this.cboHTCommuneName.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0} {1}", item.INITIAL_NAME, item.COMMUNE_NAME);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboInfantResult_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboInfantResult.EditValue != null)
                {
                    if ((long)cboInfantResult.EditValue == 2)
                    {
                        lciDeathDate.Enabled = true;
                    }
                    else
                    {
                        lciDeathDate.Enabled = false;
                        dtDeathDate.EditValue = null;
                    }
                }
                else
                {
                    lciDeathDate.Enabled = false;
                    dtDeathDate.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNumberChildrenBirth_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsSurgery_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (chkInfantcheck.Checked == false)
                    {
                        txtInfantMonth.Focus();
                        txtInfantMonth.SelectAll();
                    }
                    else
                    {
                        cboEthnic.Focus();
                        cboEthnic.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinCardTmp_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtInfantMidwife1.Focus();
                    txtInfantMidwife1.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtDeathDate_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtdInfantdate.Focus();
                    dtdInfantdate.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNumberOfBirth_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHisBirthSertBook_QueryPopUp(object sender, CancelEventArgs e)
        {
            try
            {
                GridLookUpEdit editor = sender as GridLookUpEdit;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BIRTH_CERT_BOOK_CODE", "", 10, 1, true));
                columnInfos.Add(new ColumnInfo("BIRTH_CERT_BOOK_NAME", "", 100, 2, true));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BIRTH_CERT_BOOK_NAME", "ID", columnInfos, false, 110);
                ControlEditorLoader.Load(editor, lstBirthCertBook != null && lstBirthCertBook.Count > 0 ? lstBirthCertBook.Where(o => o.CURRENT_BIRTH_CERT_NUM < (o.FROM_NUM_ORDER + o.TOTAL - 1)).ToList() : null, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBirthPlaceType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboBirthPlaceType.EditValue != null)
                {
                    var birthplaceTypeId = Inventec.Common.TypeConvert.Parse.ToInt16(cboBirthPlaceType.EditValue.ToString());
                    if (birthplaceTypeId == 1)
                    {
                        this.lciBirthHospital.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        isBornedAtHospital = true;
                        LoadComboBirthHospital();
                        cboBirthHospital.EditValue = hisBranch.BRANCH_CODE;
                    }
                    else if (birthplaceTypeId == 2)
                    {
                        this.lciBirthHospital.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        isBornedAtHospital = false;
                        LoadComboBirthHospital();
                        cboBirthHospital.EditValue = null;
                    }
                    else
                    {
                        this.lciBirthHospital.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        cboBirthHospital.EditValue = null;
                    }
                    dxErrorProvider1.SetError(dtdInfantdate, "", ErrorType.None);
                    dxErrorProvider1.SetError(txtInfantBorntime, "", ErrorType.None);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBirthHospital_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboBirthHospital.EditValue != null)
                {
                    this.cboProvinceNameHospital.Properties.DataSource = listProvince;
                    this.cboDistrictNameHospital.Properties.DataSource = lstDistrictADO;
                    this.cboCommuneNameHospital.Properties.DataSource = lstCommuneADO;
                    if (this.isBornedAtHospital)
                    {
                        txtProvinceCodeHospital.Enabled = false;
                        cboProvinceNameHospital.Enabled = false;
                        cboProvinceNameHospital.EditValue = this.hisBranch.PROVINCE_CODE;
                        txtProvinceCodeHospital.Text = listProvince.FirstOrDefault(o => o.PROVINCE_CODE == this.hisBranch.PROVINCE_CODE).SEARCH_CODE;

                        txtDistrictCodeHospital.Enabled = false;
                        cboDistrictNameHospital.Enabled = false;
                        cboDistrictNameHospital.EditValue = this.hisBranch.DISTRICT_CODE;
                        txtDistrictCodeHospital.Text = lstDistrictADO.FirstOrDefault(o => o.DISTRICT_CODE == this.hisBranch.DISTRICT_CODE).SEARCH_CODE;

                        txtCommuneCodeHospital.Enabled = false;
                        cboCommuneNameHospital.Enabled = false;
                        cboCommuneNameHospital.EditValue = this.hisBranch.COMMUNE_CODE;
                        txtCommuneCodeHospital.Text = lstCommuneADO.FirstOrDefault(o => o.COMMUNE_CODE == this.hisBranch.COMMUNE_CODE).SEARCH_CODE;
                    }
                    else
                    {
                        var mediOrg = listHisMediOrg.FirstOrDefault(o => o.MEDI_ORG_CODE == cboBirthHospital.EditValue.ToString());
                        txtProvinceCodeHospital.Enabled = true;
                        cboProvinceNameHospital.Enabled = true;
                        txtDistrictCodeHospital.Enabled = true;
                        cboDistrictNameHospital.Enabled = true;
                        txtCommuneCodeHospital.Enabled = true;
                        cboCommuneNameHospital.Enabled = true;
                        if (mediOrg != null)
                        {
                            if (!String.IsNullOrEmpty(mediOrg.PROVINCE_CODE))
                            {
                                cboProvinceNameHospital.EditValue = mediOrg.PROVINCE_CODE;
                                txtProvinceCodeHospital.Text = listProvince.FirstOrDefault(o => o.PROVINCE_CODE == mediOrg.PROVINCE_CODE).SEARCH_CODE;
                            }
                            else
                            {
                                cboProvinceNameHospital.EditValue = null;
                                txtProvinceCodeHospital.Text = null;
                            }
                            if (!String.IsNullOrEmpty(mediOrg.DISTRICT_CODE))
                            {
                                txtDistrictCodeHospital.Text = lstDistrictADO.FirstOrDefault(o => o.DISTRICT_CODE == mediOrg.DISTRICT_CODE).SEARCH_CODE;
                                cboDistrictNameHospital.EditValue = mediOrg.DISTRICT_CODE;
                            }
                            else
                            {
                                cboDistrictNameHospital.EditValue = null;
                                txtDistrictCodeHospital.Text = null;
                            }
                            if (!String.IsNullOrEmpty(mediOrg.COMMUNE_CODE))
                            {
                                txtCommuneCodeHospital.Text = lstCommuneADO.FirstOrDefault(o => o.COMMUNE_CODE == mediOrg.COMMUNE_CODE).SEARCH_CODE;
                                cboCommuneNameHospital.EditValue = mediOrg.COMMUNE_CODE;
                            }
                            else
                            {
                                cboCommuneNameHospital.EditValue = null;
                                txtCommuneCodeHospital.Text = null;
                            }
                        }
                    }
                }
                else
                {
                    txtProvinceCodeHospital.Enabled = true;
                    cboProvinceNameHospital.Enabled = true;
                    cboProvinceNameHospital.EditValue = null;
                    txtProvinceCodeHospital.Text = null;

                    txtDistrictCodeHospital.Enabled = true;
                    cboDistrictNameHospital.Enabled = true;
                    cboDistrictNameHospital.EditValue = null;
                    txtDistrictCodeHospital.Text = null;

                    txtCommuneCodeHospital.Enabled = true;
                    cboCommuneNameHospital.Enabled = true;
                    cboCommuneNameHospital.EditValue = null;
                    txtCommuneCodeHospital.Text = null;

                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProvinceNameHospital_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboProvinceNameHospital.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistrictNameHospital_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                    cboDistrictNameHospital.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCommuneNameHospital_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                    cboCommuneNameHospital.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtProvinceCodeHospital_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            {
                try
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        this.LoadComboTinhThanh_BV((sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper(), true);
                    }
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
        }

        private void txtProvinceCodeHospital_EditValueChanged(object sender, EventArgs e)
        {
            {
                try
                {
                    if (String.IsNullOrEmpty(this.txtProvinceCode.Text))
                    {
                        this.cboProvinceNameHospital.Properties.DataSource = listProvince;
                    }
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
        }

        private void cboProvinceNameHospital_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                cboProvinceNameHospital.Properties.DataSource = listProvinceTemp;
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboProvinceNameHospital.EditValue != null
                        && this.cboProvinceNameHospital.EditValue != this.cboProvinceNameHospital.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = listProvince.SingleOrDefault(o => o.PROVINCE_CODE == cboProvinceNameHospital.EditValue.ToString());
                        if (province != null)
                        {
                            this.LoadComboHuyen_BV("", province.PROVINCE_CODE, false);
                            this.txtProvinceCodeHospital.Text = province.SEARCH_CODE;
                        }
                    }
                    this.txtDistrictCodeHospital.Text = "";
                    this.txtDistrictCodeHospital.Focus();
                    this.txtDistrictCodeHospital.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProvinceNameHospital_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboProvinceNameHospital.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = listProvince.SingleOrDefault(o => o.PROVINCE_CODE == this.cboProvinceNameHospital.EditValue.ToString());
                        if (province != null)
                        {
                            this.LoadComboHuyen_BV("", province.PROVINCE_CODE, false);
                            this.txtProvinceCodeHospital.Text = province.SEARCH_CODE;
                            this.txtDistrictCodeHospital.Text = "";
                           
                        }
                    }
                    this.txtDistrictCodeHospital.Focus();
                    this.txtDistrictCodeHospital.SelectAll();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistrictNameHospital_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                cboDistrictNameHospital.Properties.DataSource = listDistrictTemp;
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboDistrictNameHospital.EditValue != null
                        && this.cboDistrictNameHospital.EditValue != this.cboDistrictNameHospital.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = listDistrict.SingleOrDefault(o => o.DISTRICT_CODE == this.cboDistrictNameHospital.EditValue.ToString()
                                && (String.IsNullOrEmpty((this.cboProvinceNameHospital.EditValue ?? "").ToString()) || o.PROVINCE_CODE == (this.cboProvinceNameHospital.EditValue ?? "").ToString()));
                        if (district != null)
                        {
                            if (String.IsNullOrEmpty((this.cboProvinceNameHospital.EditValue ?? "").ToString()))
                            {
                                this.cboProvinceNameHospital.EditValue = district.PROVINCE_CODE;
                                SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = listProvince.SingleOrDefault(o => o.PROVINCE_CODE == district.PROVINCE_CODE.ToString());
                                if (province != null)
                                {
                                    this.txtProvinceCodeHospital.Text = province.SEARCH_CODE;
                                }
                                else
                                {
                                    this.txtProvinceCodeHospital.Text = district.PROVINCE_CODE;
                                }
                            }
                            this.LoadComboXa_BV("", district.DISTRICT_CODE, false);
                            this.txtDistrictCodeHospital.Text = district.SEARCH_CODE;
                            this.cboDistrictNameHospital.EditValue = district.DISTRICT_CODE;
                            this.cboCommuneNameHospital.EditValue = null;
                            this.txtCommuneCodeHospital.Text = "";
                        }
                    }
                    this.txtCommuneCodeHospital.Focus();
                    this.txtCommuneCodeHospital.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistrictNameHospital_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboDistrictNameHospital.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = listDistrict.SingleOrDefault(o => o.DISTRICT_CODE == this.cboDistrictNameHospital.EditValue.ToString()
                               && (String.IsNullOrEmpty((this.cboProvinceNameHospital.EditValue ?? "").ToString()) || o.PROVINCE_CODE == (this.cboProvinceNameHospital.EditValue ?? "").ToString()));
                        if (district != null)
                        {
                            if (String.IsNullOrEmpty((this.cboProvinceNameHospital.EditValue ?? "").ToString()))
                            {
                                this.cboProvinceNameHospital.EditValue = district.PROVINCE_CODE;
                            }
                            this.LoadComboXa_BV("", district.DISTRICT_CODE, false);
                            this.txtDistrictCodeHospital.Text = district.SEARCH_CODE;
                            this.cboCommuneNameHospital.EditValue = null;
                            this.txtCommuneCodeHospital.Text = "";
                           
                        }
                    }
                    this.txtCommuneCodeHospital.Focus();
                    this.txtCommuneCodeHospital.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCommuneCodeHospital_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string districtCode = "";
                    if (this.cboDistrictNameHospital.EditValue != null)
                    {
                        districtCode = this.cboDistrictNameHospital.EditValue.ToString();
                    }
                    this.LoadComboXa_BV((sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper(), districtCode, true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCommuneNameHospital_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                cboCommuneNameHospital.Properties.DataSource = listCommuneTemp;
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboCommuneNameHospital.EditValue != null
                        && this.cboCommuneNameHospital.EditValue != cboCommuneNameHospital.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = listCommune.SingleOrDefault(o => o.COMMUNE_CODE == this.cboCommuneNameHospital.EditValue.ToString()
                                    && (String.IsNullOrEmpty((this.cboDistrictNameHospital.EditValue ?? "").ToString()) || o.DISTRICT_CODE == (this.cboDistrictNameHospital.EditValue ?? "").ToString())
                                );
                        if (commune != null)
                        {
                            this.txtCommuneCodeHospital.Text = commune.SEARCH_CODE;
                            if (String.IsNullOrEmpty((this.cboProvinceNameHospital.EditValue ?? "").ToString()) && String.IsNullOrEmpty((this.cboDistrictNameHospital.EditValue ?? "").ToString()))
                            {
                                this.cboDistrictNameHospital.EditValue = commune.DISTRICT_CODE;
                                SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = listDistrict.Where(o => o.ID == commune.DISTRICT_ID).FirstOrDefault();
                                if (district != null)
                                {
                                    this.txtDistrictCodeHospital.Text = district.SEARCH_CODE;
                                    this.cboProvinceNameHospital.EditValue = district.PROVINCE_CODE;
                                    SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = listProvince.SingleOrDefault(o => o.PROVINCE_CODE == district.PROVINCE_CODE.ToString());
                                    if (province != null)
                                    {
                                        this.txtProvinceCodeHospital.Text = province.SEARCH_CODE;
                                    }
                                    else
                                    {
                                        this.txtProvinceCodeHospital.Text = district.PROVINCE_CODE;
                                    }
                                    this.cboProvinceNameHospital.EditValue = district.PROVINCE_CODE;
                                }
                            }
                        }
                    }
                    this.txtBirthPlace.Focus();
                    this.txtBirthPlace.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCommuneNameHospital_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (this.cboCommuneNameHospital.EditValue != null)
                {
                    SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = listCommune.SingleOrDefault(o => o.COMMUNE_CODE == this.cboCommuneNameHospital.EditValue.ToString()
                            && (String.IsNullOrEmpty((this.cboCommuneNameHospital.EditValue ?? "").ToString()) || o.DISTRICT_CODE == (this.cboCommuneNameHospital.EditValue ?? "").ToString()));
                    if (commune != null)
                    {
                        this.txtCommuneCodeHospital.Text = commune.SEARCH_CODE;
                        if (String.IsNullOrEmpty((this.cboProvinceNameHospital.EditValue ?? "").ToString()) && String.IsNullOrEmpty((this.cboDistrictNameHospital.EditValue ?? "").ToString()))
                        {
                            this.cboDistrictNameHospital.EditValue = commune.DISTRICT_CODE;
                            SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = listDistrict.Where(o => o.ID == commune.DISTRICT_ID).FirstOrDefault();
                            if (district != null)
                            {
                                this.cboProvinceNameHospital.EditValue = district.PROVINCE_CODE;
                            }
                        }
                        this.txtBirthPlace.Focus();
                        this.txtBirthPlace.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDistrictCodeHospital_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string provinceCode = "";
                    if (this.cboProvinceName.EditValue != null)
                    {
                        provinceCode = this.cboProvinceName.EditValue.ToString();
                    }
                    this.LoadComboHuyen_BV((sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper(), provinceCode, true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProvinceNameHospital_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboProvinceNameHospital.Properties.Buttons[1].Visible = (cboProvinceNameHospital.EditValue != null && cboProvinceNameHospital.Enabled);
                if (cboProvinceNameHospital.EditValue == null)
                {
                    this.cboProvinceNameHospital.Properties.DataSource = listProvince;
                    txtProvinceCodeHospital.Text = null;
                    cboDistrictNameHospital.EditValue = null;
                    
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistrictNameHospital_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboDistrictNameHospital.Properties.Buttons[1].Visible = (cboDistrictNameHospital.EditValue != null && cboDistrictNameHospital.Enabled);
                if (cboDistrictNameHospital.EditValue == null)
                {
                    txtDistrictCodeHospital.Text = null;
                    cboCommuneNameHospital.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void cboCommuneNameHospital_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboCommuneNameHospital.Properties.Buttons[1].Visible = (cboCommuneNameHospital.EditValue != null && cboCommuneNameHospital.Enabled);
                if (cboCommuneNameHospital.EditValue == null)
                {
                    txtCommuneCodeHospital.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProvinceName_QueryPopUp(object sender, CancelEventArgs e)
        {
            try
            {
                listProvinceTemp = new List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>(cboProvinceName.Properties.DataSource as List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>);
                cboProvinceName.Properties.DataSource = listProvinceTemp.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProvinceNameHospital_QueryPopUp(object sender, CancelEventArgs e)
        {
            try
            {
                listProvinceTemp = new List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>(cboProvinceNameHospital.Properties.DataSource as List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>);
                cboProvinceNameHospital.Properties.DataSource = listProvinceTemp.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHTProvinceName_QueryPopUp(object sender, CancelEventArgs e)
        {
            try
            {
                listProvinceTemp = new List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>(cboHTProvinceName.Properties.DataSource as List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>);
                cboHTProvinceName.Properties.DataSource = listProvinceTemp.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        //cboDistricts.EditValue = null;
        //           txtDistricts.Text = "";
        //           cboCommune.EditValue = null;
        //           txtCommune.Text = "";
        //           LoadDistrictsCombo("", null, false);
        //           LoadCommuneCombo("", null, false);
        //           cboProvince.Properties.Buttons[1].Visible = false;

        private void cboDistrictName_QueryPopUp(object sender, CancelEventArgs e)
        {
            try
            {
                listDistrictTemp = new List<DistrictADO>(cboDistrictName.Properties.DataSource as List<DistrictADO>);
                cboDistrictName.Properties.DataSource = listDistrictTemp.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHTDistrictName_QueryPopUp(object sender, CancelEventArgs e)
        {
            try
            {
                listDistrictTemp = new List<DistrictADO>(cboHTDistrictName.Properties.DataSource as List<DistrictADO>);
                cboHTDistrictName.Properties.DataSource = listDistrictTemp.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistrictNameHospital_QueryPopUp(object sender, CancelEventArgs e)
        {
            try
            {
                listDistrictTemp = new List<DistrictADO>(cboDistrictNameHospital.Properties.DataSource as List<DistrictADO>);
                cboDistrictNameHospital.Properties.DataSource = listDistrictTemp.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCommuneNameHospital_QueryPopUp(object sender, CancelEventArgs e)
        {
            try
            {
                listCommuneTemp = new List<CommuneADO>(cboCommuneNameHospital.Properties.DataSource as List<CommuneADO>);
                cboCommuneNameHospital.Properties.DataSource = listCommuneTemp.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCommuneName_QueryPopUp(object sender, CancelEventArgs e)
        {
            try
            {
                listCommuneTemp = new List<CommuneADO>(cboCommuneName.Properties.DataSource as List<CommuneADO>);
                cboCommuneName.Properties.DataSource = listCommuneTemp.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHTCommuneName_QueryPopUp(object sender, CancelEventArgs e)
        {
            try
            {
                listCommuneTemp = new List<CommuneADO>(cboHTCommuneName.Properties.DataSource as List<CommuneADO>);
                cboHTCommuneName.Properties.DataSource = listCommuneTemp.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsSurgery_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtInfantWeek.Text.Trim()))
                {
                    if (chkIsSurgery.Checked)
                    {
                        if (Inventec.Common.TypeConvert.Parse.ToInt64(txtInfantWeek.Text.Trim()) > 32)
                        {
                            txtNote.Text = "Sinh con phải phẫu thuật";
                        }
                        else if (Inventec.Common.TypeConvert.Parse.ToInt64(txtInfantWeek.Text.Trim()) < 32)
                        {
                            txtNote.Text = "Phẫu thuật, sinh con dưới 32  tuần tuổi";
                        }
                        else
                        {
                            txtNote.Text = null;
                        }
                    }
                    else if (Inventec.Common.TypeConvert.Parse.ToInt64(txtInfantWeek.Text.Trim()) < 32)
                    {
                        txtNote.Text = "Sinh con dưới 32 tuần tuổi";
                    }
                    else
                    {
                        txtNote.Text = null;
                    }
                }
                else
                {
                    txtNote.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtInfantWeek_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtInfantWeek.Text.Trim()))
                {
                    if (chkIsSurgery.Checked)
                    {
                        if (Inventec.Common.TypeConvert.Parse.ToInt64(txtInfantWeek.Text.Trim()) >= 32)
                        {
                            txtNote.Text = "Sinh con phải phẫu thuật";
                        }
                        else if (Inventec.Common.TypeConvert.Parse.ToInt64(txtInfantWeek.Text.Trim()) < 32)
                        {
                            txtNote.Text = "Phẫu thuật, sinh con dưới 32  tuần tuổi";
                        }
                        else
                        {
                            txtNote.Text = null;
                        }
                    }
                    else if (Inventec.Common.TypeConvert.Parse.ToInt64(txtInfantWeek.Text.Trim()) < 32)
                    {
                        txtNote.Text = "Sinh con dưới 32 tuần tuổi";
                    }
                    else
                    {
                        txtNote.Text = null;
                    }
                }
                else
                {
                    txtNote.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtInfantBorntime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                dxErrorProvider1.SetError(txtInfantBorntime, "", ErrorType.None);
                dxErrorProvider1.SetError(dtdInfantdate, "", ErrorType.None);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void dtdInfantdate_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                dxErrorProvider1.SetError(txtInfantBorntime, "", ErrorType.None);
                dxErrorProvider1.SetError(dtdInfantdate, "", ErrorType.None);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        bool isNullDistrict = false;
        private void txtHTDistrictCode_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(this.txtHTDistrictCode.Text))
                {
                    this.cboHTDistrictName.Text = "";
                    this.cboHTCommuneName.Text = "";
                    this.txtHTCommuneCode.Text = "";
                    cboHTCommuneName.Properties.DataSource = null;
                }
                if (isNullDistrict == true)
                {
                    if (String.IsNullOrWhiteSpace(cboHTProvinceName.Text) && (!String.IsNullOrWhiteSpace(cboHTDistrictName.Text) || !String.IsNullOrWhiteSpace(cboHTCommuneName.Text)))
                    {
                        MessageBox.Show("Vui lòng chọn thông tin tỉnh trước...");
                        txtHTDistrictCode.Text = "";
                        cboHTDistrictName.Text = "";
                        txtHTCommuneCode.Text = "";
                        cboHTCommuneName.Text = "";
                    }
                }
                isNullDistrict = true;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        bool isNullCommune= false;
        private void txtHTCommuneCode_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(this.txtHTCommuneCode.Text))
                {
                    this.cboHTCommuneName.Text = "";
                }
                if (isNullCommune == true)
                {
                    if (String.IsNullOrWhiteSpace(cboHTProvinceName.Text) || String.IsNullOrWhiteSpace(cboHTDistrictName.Text))
                    {
                        MessageBox.Show("Vui lòng chọn thông tin tỉnh và huyện trước...");
                        txtHTCommuneCode.Text = "";
                        cboHTCommuneName.Text = "";
                    }
                }
                isNullCommune = true;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void lblHisBirthCertNum_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtInfantName.Focus();
                    txtInfantName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNumberChildrenBirth_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //chkInfantcheck.Properties.FullFocusRect = true;
                    //chkInfantcheck.Focus();
                    txtNumberOfBirth.Focus();
                    txtNumberOfBirth.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNumberOfBirth_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkInfantcheck.Properties.FullFocusRect = true;
                    chkInfantcheck.Focus();
                    //txtNumberOfBirth.Focus();
                    //txtNumberOfBirth.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNote_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //chkInfantcheck.Properties.FullFocusRect = true;
                    //chkInfantcheck.Focus();
                    cboEthnic.Focus();
                    cboEthnic.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDirectorUsername_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboDirectorUsername.EditValue != null)
                    {
                        txtDirectorLoginname.Text = (cboDirectorUsername.Properties.DataSource as List<V_HIS_EMPLOYEE>).FirstOrDefault(o => o.LOGINNAME == cboDirectorUsername.EditValue.ToString()).LOGINNAME;
                        txtCMT.Focus();
                        txtCMT.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDirectorUsername_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtCMT.Focus();
                    txtCMT.SelectAll();
                }
                else
                {
                    cboDirectorUsername.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDirectorUsername_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDirectorUsername.EditValue = null;
                    txtDirectorLoginname.Text = null;
                    //GridCheckMark
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDirectorUsername_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboDirectorUsername.EditValue != null)
                {
                    cboDirectorUsername.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboDirectorUsername.Properties.Buttons[1].Visible = false;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dteIssue_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboBirthPlaceType.Focus();
                    cboBirthPlaceType.SelectAll();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBirthPlaceType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboBirthHospital.Focus();
                    cboBirthHospital.SelectAll();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBirthHospital_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtProvinceCodeHospital.Focus();
                    txtProvinceCodeHospital.SelectAll();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtBirthPlace_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDirectorLoginname.Focus();
                    txtDirectorLoginname.SelectAll();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDistrictCodeHospital_KeyUp(object sender, KeyEventArgs e)
        {
    
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.cboDistrictNameHospital.Focus();
                    this.cboDistrictNameHospital.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCommuneCodeHospital_KeyUp(object sender, KeyEventArgs e)
        {
         
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.cboCommuneNameHospital.Focus();
                    this.cboCommuneNameHospital.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDirectorLoginname_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtDirectorLoginname.Text.Trim()))
                    {
                        var employee = (cboDirectorUsername.Properties.DataSource as List<V_HIS_EMPLOYEE>).FirstOrDefault(o => o.LOGINNAME == txtDirectorLoginname.Text.Trim());
                        if (employee != null)
                        {
                            cboDirectorUsername.EditValue = employee.LOGINNAME;
                            txtCMT.Focus();
                            txtCMT.SelectAll();
                        }
                        else
                        {
                            cboDirectorUsername.Focus();
                            cboDirectorUsername.ShowPopup();
                        }
                    }
                    else
                    {
                        cboDirectorUsername.Focus();
                        cboDirectorUsername.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtProvinceCodeHospital_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboProvinceNameHospital.Focus();
                    cboProvinceNameHospital.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtProvinceCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboProvinceName.Focus();
                    cboProvinceName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDistrictCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtCommuneCode.Focus();
                    txtCommuneCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCommuneCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboCommuneName.Focus();
                    cboCommuneName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAddress_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtHTProvinceCode.Focus();
                    txtHTProvinceCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHTProvinceCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboHTProvinceName.Focus();
                    cboHTProvinceName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHTDistrictCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboHTDistrictName.Focus();
                    cboHTDistrictName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHTCommuneCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboHTCommuneName.Focus();
                    cboHTCommuneName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHTAddress_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                        btnAdd.Select();
                    }
                    else
                    {
                        btnEdit.Focus();
                        btnEdit.Select();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }

}

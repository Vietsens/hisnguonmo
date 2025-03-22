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
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.CheckHeinGOV;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using HIS.Desktop.Plugins.Register.ADO;
using HIS.Desktop.Plugins.Register.PatientExtend;
using HIS.Desktop.Plugins.Register.Process;
using HIS.Desktop.Plugins.Register.Valid;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using HIS.UC.KskContract;
using HIS.UC.RoomExamService;
using HIS.UC.WorkPlace;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Controls.PopupLoader;
using Inventec.Common.Logging;
using Inventec.Common.QrCodeBHYT;
using Inventec.Common.QrCodeCCCD;
using Inventec.Core;
using Inventec.Desktop.Common.LibraryMessage;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Register.Run
{
    public partial class UCRegister : UserControlBase
    {
        #region Reclare
        internal UserControl ucKskContract;
        internal KskContractProcessor kskContractProcessor;
        internal HisCardSDO cardSearch = null;
        internal UserControl ucHeinBHYT;
        internal His.UC.UCHein.MainHisHeinBhyt mainHeinProcessor;
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        internal UserControl ucWorkPlace;
        internal WorkPlaceProcessor workPlaceProcessor;
        internal WorkPlaceProcessor.Template workPlaceTemplate;
        internal HisPatientSDO currentPatientSDO { get; set; }
        internal PatientInformationADO patientInformation;
        internal List<ServiceReqDetailSDO> serviceReqDetailSDOs;
        internal int registerNumber = 0;
        internal long? typeReceptionForm = null;
        internal string typeCodeFind = "Mã BN";//Set lại giá trị trong resource
        internal string typeCodeFind__MaBN = "Mã BN";//Set lại giá trị trong resource
        internal string typeCodeFind__MaHK = "Mã HK";//Set lại giá trị trong resource
        internal string typeCodeFind__MaCT = "Mã CT";//Set lại giá trị trong resource
        internal string typeCodeFind__SoThe = "Số thẻ";//Set lại giá trị trong resource
        internal string typeCodeFind__MaNV = "Mã NV";
        internal string typeCodeFind__CCCDCMND = "CCCD/CMND";
        internal long programId = 0;
        internal string appointmentCode = "";
        internal long _TreatmnetIdByAppointmentCode = 0;
        internal bool isNotPatientDayDob = false;
        internal List<long> serviceReqPrintIds { get; set; }
        internal long departmentId = 0;
        internal bool isShowMess;
        internal string HospitalizeReasonCode;
        internal string HospitalizeReasonName;
        internal string HospitalizeReason;
        List<HIS_PATIENT_TYPE> currentPatientTypeAllowByPatientType;
        List<HisPatientSDO> currentSearchedPatients;
        HisServiceReqExamRegisterResultSDO currentHisExamServiceReqResultSDO { get; set; }
        HisPatientProfileSDO resultHisPatientProfileSDO = null;
        UCServiceRequestRegisterFactorySaveType currentFactorySaveType;
        HeinCardData _HeinCardData { get; set; }
        ResultDataADO ResultDataADO { get; set; }
        RoomExamServiceProcessor roomExamServiceProcessor;
        UserControl ucRoomExamService = null;
        LoadExecuteRoomProcess loadExecuteRoomProcess;
        CPA.WCFClient.CallPatientClient.CallPatientClientManager clienttManager = null;
        int positionHandleControl = -1;
        int positionHandlePlusInfoControl = -1;
        int actionType = 0;
        long serviceReqIdForCreated = 0;
        const string IsDefaultRightRouteType__True = "1";
        const int Gio = 4;
        bool isReadQrCode;
        int roomExamServiceNumber = 0;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        bool isPrintNow;
        bool isDobTextEditKeyEnter;
        List<HIS_BRANCH_TIME> _BranchTimes { get; set; }
        bool _IsUserBranchTime = false;
        bool _IsDungTuyenCapCuuByTime = false;
        bool isAlertTreatmentEndInDay { get; set; }
        bool isNotCheckTT = false;
        internal bool isReadQrCccdData = false;

        internal string CccdReadFromQr = "";
        internal long? dateReleaseFromQr = 0;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.Register";
        public string mpsNationalCode = "";
        public string oldValue = "";
        #endregion

        #region Construct
        public UCRegister(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            this.currentModule = module;
        }
        #endregion

        #region Private method
        private void UCRegister_Load(object sender, EventArgs e)
        {
            try
            {
                LogSystem.Debug("Starting...");
                WaitingManager.Show();
                this.LoadBranch();
                LogSystem.Debug("Loaded LoadBranch");
                InitControlState();
                His.UC.UCHein.Base.ResouceManager.ResourceLanguageManager();
                this.SetCaptionByLanguageKey();
                HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.LoadConfig();
                HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.LoadConfig();

                this.CreateExamServiceRoomInfoPanel();

                this.SetDefaultData();
                LogSystem.Debug("Loaded SetDefaultData");

                this.loadExecuteRoomProcess = new LoadExecuteRoomProcess(this.dtIntructionTime.DateTime);
                this.loadExecuteRoomProcess.LoadDataExecuteRoomInfo(FillDataToGirdExecuteRoomInfo);

                this.SetTimer();

                this.FillDataToControlsForm();
                LogSystem.Debug("Loaded FillDataToControlsForm");

                this.SetDefaultRegisterForm();
                LogSystem.Debug("Loaded SetDefaultRegisterForm");

                this.InitTabIndex();
                LogSystem.Debug("Loaded InitTabIndex");
                this.ValidateForm();
                this.SetValidationByChildrenUnder6Years(false, false);
                LogSystem.Debug("Loaded ValidateForm");

                this.FillDataToGirdExecuteRoomInfo();
                LogSystem.Debug("Loaded FillDataToGirdExecuteRoomInfo");

                this.CreateThreadInitWCFReadCard();
                LogSystem.Debug("Loaded CreateThreadInitWCFReadCard");

                this.SetDefaultGateAndStep();
                LogSystem.Debug("Loaded SetDefaultGateAndStep");

                if (this.mainHeinProcessor != null && this.ucHeinBHYT != null)
                {
                    LogSystem.Debug("Bat dau goi ham : GoiUCTuManHinhTiepDon");
                    this.mainHeinProcessor.GoiUCTuManHinhTiepDon(this.ucHeinBHYT, true);
                    LogSystem.Debug("Ket thuc goi ham : GoiUCTuManHinhTiepDon");
                }

                //this.InitExamServiceRoom(true, null);//xuandv  bỏ đi vì khi gán dữ liệu đối tượng BN đã khởi tạo rồi
                //LogSystem.Debug("Loaded InitExamServiceRoom");

                this.InitPopupMenuOther();
                this.loadHosReason();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void IsValidateAddressCombo(string _isValidate)
        {
            try
            {
                if (_isValidate == "1")
                {
                    lcitxtProvince.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciDistrict.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciCommune.AppearanceItemCaption.ForeColor = Color.Maroon;
                    SetValidate();
                }
                else if (_isValidate == "2")
                {
                    lcitxtProvince.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciDistrict.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidateProvince();
                    ValidateDistrict();
                }
                else
                {
                    lcitxtProvince.AppearanceItemCaption.ForeColor = Color.Black;
                    lciDistrict.AppearanceItemCaption.ForeColor = Color.Black;
                    lciCommune.AppearanceItemCaption.ForeColor = Color.Black;
                    this.dxValidationProviderControl.SetValidationRule(txtProvinceCode, null);
                    this.dxValidationProviderControl.SetValidationRule(txtDistrictCode, null);
                    this.dxValidationProviderControl.SetValidationRule(txtCommuneCode, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetValidate()
        {
            try
            {
                ValidateProvince();
                ValidateDistrict();
                ValidateCommune();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateProvince()
        {
            Valid_Province_Control validateProvince = new Valid_Province_Control();
            validateProvince.cboProvince = this.cboProvince;
            validateProvince.txtProvince = this.txtProvinceCode;
            validateProvince.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
            validateProvince.ErrorType = ErrorType.Warning;
            this.dxValidationProviderControl.SetValidationRule(txtProvinceCode, validateProvince);
        }

        private void ValidateDistrict()
        {
            Valid_District_Control validateProvince = new Valid_District_Control();
            validateProvince.cboDistrict = this.cboDistrict;
            validateProvince.txtDistrict = this.txtDistrictCode;
            validateProvince.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
            validateProvince.ErrorType = ErrorType.Warning;
            this.dxValidationProviderControl.SetValidationRule(txtDistrictCode, validateProvince);
        }

        private void ValidateCommune()
        {
            Valid_Commune_Control validateProvince = new Valid_Commune_Control();
            validateProvince.cboCommune = this.cboCommune;
            validateProvince.txtCommune = this.txtCommuneCode;
            validateProvince.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
            validateProvince.ErrorType = ErrorType.Warning;
            this.dxValidationProviderControl.SetValidationRule(txtCommuneCode, validateProvince);
        }
        private void SetDefaultGateAndStep()
        {
            try
            {
                this.txtGateNumber.Text = HIS.Desktop.Plugins.Library.RegisterConfig.GateAndStepCFG.GateNumber;
                this.txtStepNumber.Text = HIS.Desktop.Plugins.Library.RegisterConfig.GateAndStepCFG.StepNumber;
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
                Resources.ResourceLanguageManager.LanguageUCRegister = new ResourceManager("HIS.Desktop.Plugins.Register.Resources.Lang", typeof(HIS.Desktop.Plugins.Register.Run.UCRegister).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.lciRegisterEditor.Text = Inventec.Common.Resource.Get.Value("UCRegister.lciRegisterEditor.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnBill.Text = Inventec.Common.Resource.Get.Value("UCRegister.btnBill.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboCashierRoom.Properties.NullText = Inventec.Common.Resource.Get.Value("UCRegister.cboCashierRoom.Properties.NullText", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnSaveAndPrint.Text = Inventec.Common.Resource.Get.Value("UCRegister.btnSaveAndPrint.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboTHX.Properties.NullText = Inventec.Common.Resource.Get.Value("UCRegister.cboTHX.Properties.NullText", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnPatientNew.Text = Inventec.Common.Resource.Get.Value("UCRegister.btnPatientNew.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnPatientExtend.Text = Inventec.Common.Resource.Get.Value("UCRegister.btnPatientExtend.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.dtIntructionTime.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCRegister.dtIntructionTime.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboMilitaryRank.Properties.NullText = Inventec.Common.Resource.Get.Value("UCRegister.cboMilitaryRank.Properties.NullText", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboNational.Properties.NullText = Inventec.Common.Resource.Get.Value("UCRegister.cboNational.Properties.NullText", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboEthnic.Properties.NullText = Inventec.Common.Resource.Get.Value("UCRegister.cboEthnic.Properties.NullText", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboCareer.Properties.NullText = Inventec.Common.Resource.Get.Value("UCRegister.cboCareer.Properties.NullText", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboCommune.Properties.NullText = Inventec.Common.Resource.Get.Value("UCRegister.cboCommune.Properties.NullText", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboDistrict.Properties.NullText = Inventec.Common.Resource.Get.Value("UCRegister.cboDistrict.Properties.NullText", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboProvince.Properties.NullText = Inventec.Common.Resource.Get.Value("UCRegister.cboProvince.Properties.NullText", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciOweType.Text = Inventec.Common.Resource.Get.Value("UCRegister.lciOweType.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnSaveAndAssain.Text = Inventec.Common.Resource.Get.Value("UCRegister.btnSaveAndAssain.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnDepositDetail.Text = Inventec.Common.Resource.Get.Value("UCRegister.btnDepositDetail.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnTreatmentBedRoom.Text = Inventec.Common.Resource.Get.Value("UCRegister.btnTreatmentBedRoom.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnNewContinue.Text = Inventec.Common.Resource.Get.Value("UCRegister.btnNewContinue.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("UCRegister.btnPrint.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UCRegister.btnSave.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboGender.Properties.NullText = Inventec.Common.Resource.Get.Value("UCRegister.cboGender.Properties.NullText", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.dtPatientDob.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCRegister.dtPatientDob.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboAge.Properties.NullText = Inventec.Common.Resource.Get.Value("UCRegister.cboAge.Properties.NullText", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnCodeFind.Text = Inventec.Common.Resource.Get.Value("UCRegister.btnCodeFind.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.chkIsNotRequireFee.Properties.Caption = Inventec.Common.Resource.Get.Value("UCRegister.chkIsNotRequireFee.Properties.Caption", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.chkPriority.Properties.Caption = Inventec.Common.Resource.Get.Value("UCRegister.chkPriority.Properties.Caption", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.chkEmergency.Properties.Caption = Inventec.Common.Resource.Get.Value("UCRegister.chkEmergency.Properties.Caption", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboEmergencyTime.Properties.NullText = Inventec.Common.Resource.Get.Value("UCRegister.cboEmergencyTime.Properties.NullText", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCRegister.cboPatientType.Properties.NullText", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gboxPatientInformation.Text = Inventec.Common.Resource.Get.Value("UCRegister.gboxPatientInformation.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciPatientCode.Text = Inventec.Common.Resource.Get.Value("UCRegister.lciPatientCode.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciPatientName.Text = Inventec.Common.Resource.Get.Value("UCRegister.lciPatientName.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciAge.Text = Inventec.Common.Resource.Get.Value("UCRegister.lciAge.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciGender.Text = Inventec.Common.Resource.Get.Value("UCRegister.lciGender.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciPatientDob.Text = Inventec.Common.Resource.Get.Value("UCRegister.lciPatientDob.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciPatientType.Text = Inventec.Common.Resource.Get.Value("UCRegister.lciPatientType.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciAddress.Text = Inventec.Common.Resource.Get.Value("UCRegister.lciAddress.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciCommune.Text = Inventec.Common.Resource.Get.Value("UCRegister.lciCommune.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciDistrict.Text = Inventec.Common.Resource.Get.Value("UCRegister.lciDistrict.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lcitxtProvince.Text = Inventec.Common.Resource.Get.Value("UCRegister.lcitxtProvince.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("UCRegister.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("UCRegister.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gboxHeinCardInformation.Text = Inventec.Common.Resource.Get.Value("UCRegister.gboxHeinCardInformation.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gboxRequestOtherInformation.Text = Inventec.Common.Resource.Get.Value("UCRegister.gboxRequestOtherInformation.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciIsNotRequireFee.Text = Inventec.Common.Resource.Get.Value("UCRegister.lciIsNotRequireFee.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciPriority.Text = Inventec.Common.Resource.Get.Value("UCRegister.lciPriority.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciIsEmergency.Text = Inventec.Common.Resource.Get.Value("UCRegister.lciIsEmergency.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciEmergencyTime.Text = Inventec.Common.Resource.Get.Value("UCRegister.lciEmergencyTime.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciIntructionTime.Text = Inventec.Common.Resource.Get.Value("UCRegister.lciIntructionTime.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gboxServiceRoomInformation.Text = Inventec.Common.Resource.Get.Value("UCRegister.gboxServiceRoomInformation.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("UCRegister.layoutControlItem21.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gboxMoreInformation.Text = Inventec.Common.Resource.Get.Value("UCRegister.gboxMoreInformation.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciEthnic.Text = Inventec.Common.Resource.Get.Value("UCRegister.lciEthnic.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciNational.Text = Inventec.Common.Resource.Get.Value("UCRegister.lciNational.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciPhone.Text = Inventec.Common.Resource.Get.Value("UCRegister.lciPhone.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciCareer.Text = Inventec.Common.Resource.Get.Value("UCRegister.lciCareer.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lcitxtMilitaryRankCode.Text = Inventec.Common.Resource.Get.Value("UCRegister.lcitxtMilitaryRankCode.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lcitxtHomePerson.Text = Inventec.Common.Resource.Get.Value("UCRegister.lcitxtHomePerson.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lcitxtCorrelated.Text = Inventec.Common.Resource.Get.Value("UCRegister.lcitxtCorrelated.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lcitxtRelativeAddress.Text = Inventec.Common.Resource.Get.Value("UCRegister.lcitxtRelativeAddress.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lcicboCashierRoom.Text = Inventec.Common.Resource.Get.Value("UCRegister.lcicboCashierRoom.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciTreatmentType.Text = Inventec.Common.Resource.Get.Value("UCRegister.lciTreatmentType.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.grboxExecuteRoomInfo.Text = Inventec.Common.Resource.Get.Value("UCRegister.grboxExecuteRoomInfo.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnRecallPatient.Text = Inventec.Common.Resource.Get.Value("UCRegister.btnRecallPatient.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnCallPatient.Text = Inventec.Common.Resource.Get.Value("UCRegister.btnCallPatient.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.txtStepNumber.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCRegister.txtStepNumber.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.txtGateNumber.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCRegister.txtGateNumber.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExecuteRoomInfo_ExcuteRoom1.Caption = Inventec.Common.Resource.Get.Value("UCRegister.gridColumn_ExecuteRoomInfo_ExcuteRoom1.Caption", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExecuteRoomInfo_Amount1.Caption = Inventec.Common.Resource.Get.Value("UCRegister.gridColumn_ExecuteRoomInfo_Amount1.Caption", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExecuteRoomInfo_ExecuteRoom2.Caption = Inventec.Common.Resource.Get.Value("UCRegister.gridColumn_ExecuteRoomInfo_ExecuteRoom2.Caption", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExecuteRoomInfo_Amount2.Caption = Inventec.Common.Resource.Get.Value("UCRegister.gridColumn_ExecuteRoomInfo_Amount2.Caption", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExecuteRoomInfo_ExecuteRoom3.Caption = Inventec.Common.Resource.Get.Value("UCRegister.gridColumn_ExecuteRoomInfo_ExecuteRoom3.Caption", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExecuteRoomInfo_Amount3.Caption = Inventec.Common.Resource.Get.Value("UCRegister.gridColumn_ExecuteRoomInfo_Amount3.Caption", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExecuteRoomInfo_ExecuteRoom4.Caption = Inventec.Common.Resource.Get.Value("UCRegister.gridColumn_ExecuteRoomInfo_ExecuteRoom4.Caption", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExecuteRoomInfo_Amount4.Caption = Inventec.Common.Resource.Get.Value("UCRegister.gridColumn_ExecuteRoomInfo_Amount4.Caption", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExecuteRoomInfo_ExecuteRoom5.Caption = Inventec.Common.Resource.Get.Value("UCRegister.gridColumn_ExecuteRoomInfo_ExecuteRoom5.Caption", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_ExecuteRoomInfo_Amount5.Caption = Inventec.Common.Resource.Get.Value("UCRegister.gridColumn_ExecuteRoomInfo_Amount5.Caption", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                typeCodeFind__MaBN = Inventec.Common.Resource.Get.Value("UCRegister.btnCodeFind.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                typeCodeFind__MaHK = Inventec.Common.Resource.Get.Value("UCRegister.btnCodeFind.Text__MaHK", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                typeCodeFind__MaCT = Inventec.Common.Resource.Get.Value("UCRegister.btnCodeFind.Text__MaCT", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                typeCodeFind__SoThe = Inventec.Common.Resource.Get.Value("UCRegister.btnCodeFind.Text__SoThe", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                typeCodeFind__MaNV = Inventec.Common.Resource.Get.Value("UCRegister.btnCodeFind.Text__MaNV", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                typeCodeFind__CCCDCMND = Inventec.Common.Resource.Get.Value("UCRegister.btnCodeFind.Text__CCCD", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                typeCodeFind = typeCodeFind__MaBN;
                lciBNManTinh.Text = Inventec.Common.Resource.Get.Value("UCRegister.lciBNManTinh.lciBNManTinh.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetPatientInfo()
        {
            try
            {
                if (!this.lciRegisterEditor.IsInitialized) return;
                this.lciRegisterEditor.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in lciRegisterEditor.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            if (lci.Name == "lciGateNumber" || lci.Name == "lciStepNumber" || lci.Name == "lcicboCashierRoom" || lci.Name == "layoutControlItem14" || lci.Name == "layoutControlItem15" || lci.Name == "layoutControlItem17")
                            {
                                continue;
                            }
                            fomatFrm.ResetText();
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
                    this.lciRegisterEditor.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetPatientSearchPanel(bool isFinded)
        {
            try
            {
                if (isFinded)
                {
                    this.lcibtnPatientNewInfo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    this.currentPatientSDO = null;
                    this.lcibtnPatientNewInfo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetEnableButtonControl(int actionType)
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionAdd)
                {
                    this.btnSave.Enabled = btnSaveAndPrint.Enabled = btnSaveAndAssain.Enabled = true;
                    this.btnPrint.Enabled = btnBill.Enabled = btnSaveAndAssain.Enabled = btnTreatmentBedRoom.Enabled = btnDepositDetail.Enabled = dropDownButton_Other.Enabled = false;
                }
                else
                {
                    this.btnSave.Enabled = btnSaveAndPrint.Enabled = btnSaveAndAssain.Enabled = false;
                    this.btnPrint.Enabled = btnBill.Enabled = btnSaveAndAssain.Enabled = btnTreatmentBedRoom.Enabled = btnDepositDetail.Enabled = dropDownButton_Other.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultRegisterForm()
        {
            try
            {
                LogSystem.Debug("SetDefaultRegisterForm. 1");
                this.IsValidateAddressCombo(HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.Validate__T_H_X);
                this._HeinCardData = new HeinCardData();
                this.ResultDataADO = new ResultDataADO();
                this.actionType = GlobalVariables.ActionAdd;
                this.isReadQrCccdData = false;
                this.CccdReadFromQr = null;
                this.dateReleaseFromQr = null;
                this.isReadQrCode = false;
                this.cardSearch = null;
                this.isNotPatientDayDob = false;
                this.txtPatientDob.Text = "";
                this.dtPatientDob.EditValue = null;
                this.chkPriority.Checked = false;
                this.cboTHX.Properties.Buttons[1].Visible = false;
                this.txtAge.Enabled = false;
                this.cboAge.Enabled = false;
                this.cboGender.Enabled = true;
                this.serviceReqPrintIds = new List<long>();
                this.lblDescriptionForHID.Text = "";
                this.mpsNationalCode = "";
                var genderDefault = HisConfigCFG.GenderBase;
                if (genderDefault != null && genderDefault.ID > 0)
                {
                    this.cboGender.EditValue = genderDefault.ID;
                    this.txtGenderCode.Text = genderDefault.GENDER_CODE;
                }
                var nationalDefault = HisConfigCFG.NationalBase;
                if (nationalDefault != null)
                {
                    this.cboNational.EditValue = nationalDefault.NATIONAL_NAME;
                    this.txtNationalCode.Text = nationalDefault.NATIONAL_CODE;
                    this.mpsNationalCode = nationalDefault.MPS_NATIONAL_CODE;
                }
                var ethnicDefault = HisConfigCFG.EthinicBase;
                if (ethnicDefault != null)
                {
                    this.cboEthnic.EditValue = ethnicDefault.ETHNIC_NAME;
                    this.txtEthnicCode.Text = ethnicDefault.ETHNIC_CODE;
                }
                var careerDefault = HisConfigCFG.CareerBase;
                if (careerDefault != null && careerDefault.ID > 0)
                {
                    this.cboCareer.EditValue = careerDefault.ID;
                    this.txtCareerCode.Text = careerDefault.CAREER_CODE;
                }
                //xuandv add lai dien dieu tri sau khi reset
                this.cboTreatmentType.EditValue = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM;

                MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType = null;
                if (!String.IsNullOrEmpty(AppConfigs.PatientTypeCodeDefault))
                {
                    patientType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == AppConfigs.PatientTypeCodeDefault);
                    if (patientType == null)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Phan mem RAE da duoc cau hinh doi tuong benh nhan mac dinh, tuy nhien ma doi tuong cau hinh khong ton tai trong danh muc doi tuong benh nhan, he thong tu dong lay doi tuong mac dinh la doi tuong BHYT. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => AppConfigs.PatientTypeCodeDefault), AppConfigs.PatientTypeCodeDefault));
                        patientType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == HisConfigCFG.PatientTypeCode__BHYT);
                    }
                }
                else
                {
                    patientType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == HisConfigCFG.PatientTypeCode__BHYT);
                }

                LogSystem.Debug("t3: set default patientType and generate uc hein usercontrol into groupbox");
                if (patientType != null)
                {
                    if (patientType.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboPatientType.EditValue ?? "").ToString()))
                    {
                        if (this.mainHeinProcessor != null && this.ucHeinBHYT != null)
                        {
                            this.mainHeinProcessor.ResetValue(this.ucHeinBHYT);
                        }

                        if (this.kskContractProcessor != null && this.ucKskContract != null)
                        {
                            this.kskContractProcessor.ResetValue(this.ucKskContract);
                        }

                    }
                    else
                    {
                        this.cboPatientType.EditValue = patientType.ID;
                        this.txtPatientTypeCode.Text = patientType.PATIENT_TYPE_CODE;

                        //Lay danh sach doi tuong kham theo doi tuong benh nhan da chon, 
                        //sau do set gia tri mac dinh cho doi tuong kham
                        this.currentPatientTypeAllowByPatientType = this.LoadPatientTypeExamByPatientType(patientType.ID);

                        //Goi ham xu ly load dong vung thong tin bhyt theo doi tuong benh nhan,ung
                        // voi tung doi tuong se goi den thu vien His.UC.UCHein thuc hien load dong giao dien
                        this.ChoiceTemplateHeinCard(patientType.PATIENT_TYPE_CODE, false);
                    }
                }
                else
                {
                    this.cboPatientType.EditValue = null;
                    this.txtPatientTypeCode.Text = "";
                    this.ChoiceTemplateHeinCard("", false);
                    Inventec.Common.Logging.LogSystem.Debug("Khong lay duoc doi tuong benh nhan mac dinh");
                }

                this.chkEmergency.Checked = false;
                this.cboHosReason.EditValue = null;
                this.dtIntructionTime.EditValue = DateTime.Now;
                this.dtIntructionTime.Properties.VistaDisplayMode = DefaultBoolean.True;
                this.dtIntructionTime.Properties.VistaEditTime = DefaultBoolean.True;
                this.txtIntructionTime.Text = dtIntructionTime.DateTime.ToString("dd/MM/yyyy HH:mm");
                LogSystem.Debug("t2: set default data into control");

                LogSystem.Debug("t5: end");
                this.EnableButton(this.actionType, false);

                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProviderControl, this.dxErrorProviderControl);
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProviderPlusInfomation, this.dxErrorProviderControl);
                txtPatientDob.ErrorText = "";
                this.SetValidationByChildrenUnder6Years(false, true);

                this.patientInformation = null;

                this._TreatmnetIdByAppointmentCode = 0;
                this.isAlertTreatmentEndInDay = false;
                this.chkIsChronic.Checked = false;
                this.chkIsChronic.ReadOnly = false;
                this.chkNoCCCD.Checked = false;
                if (GlobalVariables.AcsAuthorizeSDO != null)
                {
                    var controlAcs = GlobalVariables.AcsAuthorizeSDO.ControlInRoles;
                    chkNoCCCD.Enabled = controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == "HIS000046") != null;
                }
                this.workPlaceProcessor.SetValue(this.ucWorkPlace, "");
                this.txtPatientCode.Focus();
                this.txtPatientCode.SelectAll();
                LogSystem.Debug("SetDefaultRegisterForm. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetPatientForm()
        {
            try
            {
                this.ResetPatientInfo();
                this.SetPatientSearchPanel(false);

                this.pnlServiceRoomInfomation.Controls.Clear();
                this.InitExamServiceRoom(true, null);

                this.SetDefaultRegisterForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PatientInfoResult(object data)
        {
            try
            {
                this.patientInformation = data as PatientInformationADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckCashierRoom()
        {
            bool valid = true;
            try
            {
                if (Inventec.Common.TypeConvert.Parse.ToInt64((this.cboCashierRoom.EditValue ?? "0").ToString()) == 0)
                {
                    valid = false;
                    MessageManager.Show(ResourceMessage.ChonPhongThuNganTruocKhiMoTinhNangNay);
                    cboCashierRoom.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private void CreateThreadCallPatient()
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(CallPatientNewThread));
            try
            {
                thread.Start();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                thread.Abort();
            }
        }

        private void CallPatientNewThread()
        {
            try
            {
                //if (this.InvokeRequired)
                //{
                //    this.Invoke(new MethodInvoker(delegate { this.CallPatient(); }));
                //}
                //else
                //{
                this.CallPatient();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CallPatient()
        {
            try
            {
                if (!this.btnCallPatient.Enabled)
                    return;
                if (String.IsNullOrEmpty(this.txtGateNumber.Text) || String.IsNullOrEmpty(this.txtStepNumber.Text))
                    return;
                if (AppConfigs.DangKyTiepDonGoiBenhNhanBangCPA == "1")
                {
                    if (this.clienttManager == null)
                        this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager();
                    this.clienttManager.CallNumOrder(int.Parse(txtGateNumber.Text), int.Parse(this.txtStepNumber.Text));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Control editor
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


        private void txtPatientCode_KeyDown(object sender, KeyEventArgs e)
        {
            string OldTypeFind = this.typeCodeFind;
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    isReadQrCccdData = false;
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    this.chkIsChronic.Checked = false;
                    this.chkIsChronic.ReadOnly = false;
                    this.oldValue = strValue;
                    IsReadCardTheViet = false;
                    SearchPatientByCodeOrQrCode(strValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                this.typeCodeFind = OldTypeFind;
            }
        }

        private void SearchPatientByCodeOrQrCode(string strValue)
        {

            try
            {
                if (!String.IsNullOrEmpty(strValue))
                {
                    LogSystem.Debug("txtPatientCode_KeyDown");

                    if (strValue.Contains("|"))
                    {
                        var dataFirst = strValue.Split('|')[0];
                        if (dataFirst.Length == 10 || dataFirst.Length == 15)
                        {
                            this.typeCodeFind = typeCodeFind__MaBN;

                        }
                        else if (dataFirst.Length == 12)
                        {
                            this.typeCodeFind = typeCodeFind__CCCDCMND;
                        }
                    }

                    CommonParam param = new CommonParam();
                    WaitingManager.Show();
                    //Trường hợp tìm kiếm BN theo mã BN hoặc theo qrcode
                    if (this.typeCodeFind == typeCodeFind__MaBN)
                    {
                        WaitingManager.Hide();
                        this.typeReceptionForm = Base.ReceptionForm.MaBN;
                        this.ProcessSearchByCode(strValue);
                    }
                    //Trường hợp tìm kiếm BN theo số thẻ thông minh
                    else if (this.typeCodeFind == typeCodeFind__SoThe)
                    {
                        var patientInRegisterSearchByCard = new BackendAdapter(param).Get<HisCardSDO>(RequestUriStore.HIS_CARD_GETVIEWBYSERVICECODE, ApiConsumers.MosConsumer, strValue, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        WaitingManager.Hide();
                        if (patientInRegisterSearchByCard != null)
                        {
                            this.typeReceptionForm = Base.ReceptionForm.TheKcbThongminh;
                            this.actionType = GlobalVariables.ActionAdd;
                            this.cardSearch = patientInRegisterSearchByCard;
                            string heinAddressOfPatient = "";
                            var data = SearchByCode(patientInRegisterSearchByCard.PatientCode);
                            IsReadCardTheViet = true;
                            if (data != null && data.Result != null && data.Result is HisPatientSDO)
                            {
                                currentPatientSDO = data.Result as HisPatientSDO;
                                currentPatientSDO.HT_COMMUNE_NAME = HtCommuneName = patientInRegisterSearchByCard.HtCommuneName;
                                currentPatientSDO.HT_DISTRICT_NAME = HtDistrictName = patientInRegisterSearchByCard.HtDistrictName;
                                currentPatientSDO.HT_PROVINCE_NAME = HtProvinceName = patientInRegisterSearchByCard.HtProvinceName;
                                currentPatientSDO.HT_COMMUNE_CODE = HtCommuneCode = patientInRegisterSearchByCard.HtCommuneCode;
                                currentPatientSDO.HT_DISTRICT_CODE = HtDistrictCode = patientInRegisterSearchByCard.HtDistrictCode;
                                currentPatientSDO.HT_PROVINCE_CODE = HtProvinceCode = patientInRegisterSearchByCard.HtProvinceCode;
                                //Benh nhan da dang ky tren he thong benh vien, da co thong tin ho so
                                this.SetPatientSearchPanel(true);
                                heinAddressOfPatient = ((HisPatientSDO)data.Result).HeinAddress;
                                this.Invoke(new MethodInvoker(delegate ()
                                {
                                    this.ProcessPatientCodeKeydown(data.Result);
                                }));
                            }
                            else
                            {
                                this.Invoke(new MethodInvoker(delegate ()
                                {
                                    //An button lam moi khi co du lieu benh nhan cu
                                    this.SetPatientSearchPanel(false);

                                    //Benh nhan chua dang ky tren he thong benh vien, chua co thong tin ho so
                                    HisPatientSDO patientByCard = new HisPatientSDO();
                                    this.SetPatientDTOFromCardSDO(patientInRegisterSearchByCard, patientByCard);
                                    this.FillDataPatientToControl(patientByCard);
                                    this.FillDataToHeinCardControlByCardSDO(patientInRegisterSearchByCard);
                                }));
                            }

                            HeinGOVManager heinGOVManager = new HeinGOVManager(ResourceMessage.GoiSangCongBHXHTraVeMaLoi);
                            this.ResultDataADO = heinGOVManager.Check(this._HeinCardData, null, false, heinAddressOfPatient, dtIntructionTime.DateTime, this.isReadQrCode).Result;
                            mainHeinProcessor.SetResultDataADOBhyt(ucHeinBHYT, this.ResultDataADO);
                            if (this.ResultDataADO != null)
                            {
                                //Trường hợp tìm kiếm BN theo qrocde & BN có số thẻ bhyt mới, cần tìm kiếm BN theo số thẻ mới này & người dùng chọn lấy thông tin thẻ mới => tìm kiếm Bn theo số thẻ mới
                                if (!String.IsNullOrEmpty(this._HeinCardData.HeinCardNumber))
                                {
                                    if (this.ResultDataADO.IsShowQuestionWhileChangeHeinTime__Choose)
                                    {
                                        this._HeinCardData.HeinCardNumber = this.ResultDataADO.ResultHistoryLDO.maTheMoi;
                                    }
                                }
                            }

                            this.CheckTTProcessResultData(this._HeinCardData);
                            this.UCHeinProcessFillDataCareerUnder6AgeByHeinCardNumber(this._HeinCardData, true);
                        }
                        else
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.SoTheKhongTontai + " '" + strValue + "'", MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                            this.txtPatientCode.Focus();
                            this.txtPatientCode.SelectAll();
                            return;
                        }
                    }
                    //Trường hợp tìm kiếm Bn theo mã chương trình
                    else if (this.typeCodeFind == typeCodeFind__MaCT)
                    {
                        //Review .FirstOrDefault(); Exception
                        var _PatientProgram = new BackendAdapter(param).Get<V_HIS_PATIENT_PROGRAM>(RequestUriStore.HIS_PATIEN_PROGRAM_GET, ApiConsumers.MosConsumer, strValue.Trim(), HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);//.FirstOrDefault();
                        WaitingManager.Hide();
                        if (_PatientProgram != null)
                        {
                            this.typeReceptionForm = Base.ReceptionForm.MaChuongTrinh;
                            this.ResetPatientForm();
                            this.txtPatientCode.Text = strValue;
                            this.programId = _PatientProgram.PROGRAM_ID;

                            this.ProcessSearchByCode(_PatientProgram.PATIENT_CODE);
                        }
                        else
                        {
                            // WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.MaChuongTrinhKhongTontai + " '" + strValue + "'", MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                            this.txtPatientCode.Focus();
                            this.txtPatientCode.SelectAll();
                            return;
                        }
                    }
                    //Trường hợp tìm kiếm Bn theo mã hẹn khám
                    else if (this.typeCodeFind == typeCodeFind__MaHK)
                    {
                        int n;
                        bool isNumeric = int.TryParse(this.txtPatientCode.Text, out n);
                        if (!isNumeric)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.MaHenKhamKhongTontai + " '" + this.txtPatientCode.Text + "'", MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                            this.txtPatientCode.Focus();
                            this.txtPatientCode.SelectAll();
                            return;
                        }
                        var codeFind = string.Format("{0:000000000000}", Convert.ToInt64(strValue));
                        this.txtPatientCode.Text = codeFind;
                        param = new CommonParam();
                        HisPatientAdvanceFilter filter = new HisPatientAdvanceFilter();
                        filter.APPOINTMENT_CODE__EXACT = codeFind;
                        var data = (new BackendAdapter(param).Get<List<HisPatientSDO>>(RequestUriStore.HIS_PATIENT_GETSDOADVANCE, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param));//.SingleOrDefault();
                        WaitingManager.Hide();
                        if (data != null && data.Count > 0)
                        {
                            this.typeReceptionForm = Base.ReceptionForm.MaHenKham;
                            HisPatientSDO _PatientSDO = data.SingleOrDefault();
                            this.ProcessPatientCodeKeydown(_PatientSDO);
                            this.appointmentCode = _PatientSDO.AppointmentCode;
                            this._TreatmnetIdByAppointmentCode = _PatientSDO.TreatmentId ?? 0;

                            HeinCardData heinCard = new HeinCardData();
                            heinCard.HeinCardNumber = _PatientSDO.HeinCardNumber;
                            this.UCHeinProcessFillDataCareerUnder6AgeByHeinCardNumber(heinCard, true);
                        }
                        else
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.MaHenKhamKhongTontai + " '" + codeFind + "'", MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                            this.txtPatientCode.Focus();
                            this.txtPatientCode.SelectAll();
                            return;
                        }
                    }

                    else if (this.typeCodeFind == typeCodeFind__MaNV)
                    {
                        param = new CommonParam();
                        if (string.IsNullOrEmpty(strValue))
                            return;
                        HisPatientFilter filter = new HisPatientFilter();
                        filter.HRM_EMPLOYEE_CODE__EXACT = strValue.Trim();
                        var data = (new BackendAdapter(param).Get<List<HIS_PATIENT>>("/api/HisPatient/Get", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param));

                        if (data != null && data.Count > 0)
                        {
                            this.typeReceptionForm = Base.ReceptionForm.MaNV;
                            HisPatientSDO _PatientSDOByHrm = new HisPatientSDO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HisPatientSDO>(_PatientSDOByHrm, data.FirstOrDefault());
                            this.ProcessPatientCodeKeydown(_PatientSDOByHrm);
                            HeinCardData heinCard = new HeinCardData();
                            heinCard.HeinCardNumber = _PatientSDOByHrm.HeinCardNumber;
                            this.UCHeinProcessFillDataCareerUnder6AgeByHeinCardNumber(heinCard, true);
                        }
                        else
                        {

                            ProcessGetDataHrm(strValue.Trim());//GetDataHrm return _PatientSDOByHrm
                        }
                    }

                    else if (this.typeCodeFind == typeCodeFind__CCCDCMND)
                    {
                        if ((strValue.Trim().Length > 12 && strValue.Trim().Contains('|')) || (strValue.Trim().Length == 12 && !string.IsNullOrEmpty(txtPatientName.Text) && (!string.IsNullOrEmpty(txtPatientDob.Text) || dtPatientDob.EditValue != null)) || ((strValue.Trim().Length == 12 || strValue.Trim().Length == 9) && !strValue.Trim().Contains("|")))
                        {
                            isReadQrCccdData = true;
                            WaitingManager.Show();
                            this.ProcessSearchByCode(strValue);
                            if (strValue.Trim().Length > 12 && strValue.Trim().Contains('|'))
                            {
                                CccdReadFromQr = strValue.Split('|')[0];
                                dateReleaseFromQr = Int64.Parse(strValue.Split('|')[6].Substring(4, 4) + strValue.Split('|')[6].Substring(2, 2) + strValue.Split('|')[6].Substring(0, 2) + "000000");
                            }
                            else if (strValue.Trim().Length == 12 && !string.IsNullOrEmpty(txtPatientName.Text) && (!string.IsNullOrEmpty(txtPatientDob.Text) || dtPatientDob.EditValue != null))
                            {
                                CccdReadFromQr = strValue.Trim();
                            }
                        }
                        else
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.MaCmndCccdKhongTontai + " '" + strValue + "'", MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                            this.txtPatientCode.Focus();
                            this.txtPatientCode.SelectAll();
                            return;
                        }
                    }

                    WaitingManager.Hide();
                }
                else
                {
                    this.txtPatientName.Focus();
                    this.txtPatientName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private string ProcessDate(string date)
        {
            string result = "";
            try
            {
                if (!string.IsNullOrEmpty(date))
                {
                    if (date.Length == 4)
                    {
                        result = date;
                    }
                    else if (date.Length == 6)
                    {
                        result = new StringBuilder().Append(date.Substring(0, 2)).Append("/").Append(date.Substring(2, 4))
                            .ToString();
                    }
                    else if (date.Length == 8)
                    {
                        result = new StringBuilder().Append(date.Substring(0, 2)).Append("/").Append(date.Substring(2, 2))
                            .Append("/")
                            .Append(date.Substring(4, 4))
                            .ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }

            return result;
        }

        private CccdCardData GetDataQrCodeCccdCard(string qrCode)
        {
            CccdCardData dataCccd = null;
            try
            {
                dataCccd = ReadQrCodeCCCD.ReadDataQrCode(qrCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return dataCccd;
        }

        private void txtPatientName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtPatientCode.Text.Trim().Length == 12 && !string.IsNullOrEmpty(txtPatientName.Text) && (!string.IsNullOrEmpty(txtPatientDob.Text) || dtPatientDob.EditValue != null) && typeCodeFind == typeCodeFind__CCCDCMND)
                    {
                        isReadQrCccdData = true;
                        WaitingManager.Hide();
                        this.ProcessSearchByCode(txtPatientCode.Text);
                    }
                    else
                    {
                        this.SearchPatientByFilterCombo();
                    }
                    this.txtGenderCode.Focus();
                    this.txtGenderCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtGenderCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    if (String.IsNullOrEmpty(strValue))
                    {
                        this.cboGender.EditValue = null;
                        this.FocusShowpopup(this.cboGender, true);
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().Where(o => o.GENDER_CODE.Contains(strValue)).ToList();
                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.GENDER_CODE.ToUpper() == strValue.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboGender.EditValue = searchResult[0].ID;
                            this.txtGenderCode.Text = searchResult[0].GENDER_CODE;
                            this.txtPatientDob.Focus();

                            //CheckTT(2, searchResult[0].ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE ? "1" : "2");
                        }
                        else
                        {
                            this.cboGender.EditValue = null;
                            this.FocusShowpopup(this.cboGender, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtGenderCode_Validated(object sender, EventArgs e)
        {
            try
            {
                CheckTTFull(_HeinCardData);

                //MOS.EFMODEL.DataModels.HIS_GENDER gt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboGender.EditValue ?? "0").ToString()));
                //if (gt != null)
                //{
                //    //xuandv
                //    CheckTT(2, gt.ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE ? "1" : "2");
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboGender_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboGender.EditValue != null && this.cboGender.EditValue != this.cboGender.OldEditValue)
                    {
                        MOS.EFMODEL.DataModels.HIS_GENDER gt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboGender.EditValue ?? "0").ToString()));
                        if (gt != null)
                        {
                            this.txtGenderCode.Text = gt.GENDER_CODE;
                            this.SearchPatientByFilterCombo();
                            CheckTTFull(_HeinCardData);
                        }
                    }
                    this.txtPatientDob.Focus();
                    this.txtPatientDob.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboGender_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboGender.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_GENDER gt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboGender.EditValue ?? "0").ToString()));
                        if (gt != null)
                        {
                            this.txtGenderCode.Text = gt.GENDER_CODE;
                            this.SearchPatientByFilterCombo();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientDob_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(this.txtPatientDob.Text);
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientDob_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                HIS.Desktop.Plugins.Register.DateUtil.DateValidObject dateValidObject = DateUtil.ValidPatientDob(this.txtPatientDob.Text);
                if (dateValidObject != null)
                {
                    e.ErrorText = dateValidObject.Message;
                }

                AutoValidate = AutoValidate.EnableAllowFocusChange;
                e.ExceptionMode = ExceptionMode.DisplayError;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientDob_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                LogSystem.Debug("txtPatientDob_Validating => begin");
                if (String.IsNullOrEmpty(this.txtPatientDob.Text.Trim()))
                    return;

                WaitingManager.Show();
                HIS.Desktop.Plugins.Register.DateUtil.DateValidObject dateValidObject = DateUtil.ValidPatientDob(this.txtPatientDob.Text);
                if (dateValidObject.Age > 0 && String.IsNullOrEmpty(dateValidObject.Message))
                {
                    this.txtAge.Text = this.txtPatientDob.Text;
                    this.cboAge.EditValue = 1;
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
                    WaitingManager.Hide();
                    return;
                }

                this.isNotPatientDayDob = dateValidObject.HasNotDayDob;
                if (
                    ((this.txtPatientDob.EditValue ?? "").ToString() != (this.txtPatientDob.OldEditValue ?? "").ToString())
                    && (!String.IsNullOrEmpty(dateValidObject.OutDate))
                    )
                {
                    LogSystem.Debug("txtPatientDob_Validating => RemoveControlError");
                    this.dxValidationProviderControl.RemoveControlError(this.txtPatientDob);
                    this.txtPatientDob.ErrorText = "";
                    this.CalulatePatientAge(dateValidObject.OutDate, true);
                    this.SetValueCareerComboByCondition();
                    this.ProcessWhileChangeDOb();
                    LogSystem.Debug("txtPatientDob_Validating => TinhTuoiBenhNhan");
                    if (!(txtPatientCode.Text.Trim().Length == 12 && !string.IsNullOrEmpty(txtPatientName.Text) && (!string.IsNullOrEmpty(txtPatientDob.Text) || dtPatientDob.EditValue != null) && typeCodeFind == typeCodeFind__CCCDCMND))
                        this.SearchPatientByFilterCombo();
                    LogSystem.Debug("txtPatientDob_Validating => SearchPatientByFilterCombo");
                }
                if (this.isDobTextEditKeyEnter && this.txtAge.Enabled)
                {
                    this.txtAge.Focus();
                    this.txtAge.SelectAll();
                }
                this.isDobTextEditKeyEnter = false;
                WaitingManager.Hide();
                LogSystem.Debug("txtPatientDob_Validating => end");
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientDob_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtPatientDob_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.txtPatientDob.Text)) return;

                string dob = "";
                if (this.txtPatientDob.Text.Contains("/"))
                    dob = PatientDobUtil.PatientDobToDobRaw(this.txtPatientDob.Text);

                if (!String.IsNullOrEmpty(dob))
                {
                    this.txtPatientDob.Text = dob;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientDob_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtPatientCode.Text.Trim().Length == 12 && !string.IsNullOrEmpty(txtPatientName.Text) && (!string.IsNullOrEmpty(txtPatientDob.Text) || dtPatientDob.EditValue != null) && typeCodeFind == typeCodeFind__CCCDCMND)
                    {
                        isReadQrCccdData = true;
                        WaitingManager.Hide();
                        this.ProcessSearchByCode(txtPatientCode.Text);
                    }
                    else
                    {
                        this.isDobTextEditKeyEnter = true;
                    }
                    this.txtPatientTypeCode.Focus();
                    this.txtPatientTypeCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtPatientDob_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    this.dtPatientDob.Visible = false;

                    this.txtPatientDob.Text = dtPatientDob.DateTime.ToString("dd/MM/yyyy");
                    string strDob = this.txtPatientDob.Text;
                    this.CalulatePatientAge(strDob, true);
                    this.SetValueCareerComboByCondition();
                    if (txtPatientCode.Text.Trim().Length == 12 && !string.IsNullOrEmpty(txtPatientName.Text) && (!string.IsNullOrEmpty(txtPatientDob.Text) || dtPatientDob.EditValue != null) && typeCodeFind == typeCodeFind__CCCDCMND)
                    {
                        isReadQrCccdData = true;
                        WaitingManager.Hide();
                        this.ProcessSearchByCode(txtPatientCode.Text);
                    }
                    else
                    {
                        this.SearchPatientByFilterCombo();
                    }
                    this.ProcessWhileChangeDOb();
                    if (this.txtAge.Enabled)
                    {
                        this.txtAge.Focus();
                        this.txtAge.SelectAll();
                    }
                    else
                    {
                        this.txtPatientTypeCode.Focus();
                        this.txtPatientTypeCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                    this.txtPatientDob.Text = this.dtPatientDob.DateTime.ToString("dd/MM/yyyy");

                    this.CalulatePatientAge(this.txtPatientDob.Text, true);
                    this.SetValueCareerComboByCondition();
                    if (txtPatientCode.Text.Trim().Length == 12 && !string.IsNullOrEmpty(txtPatientName.Text) && (!string.IsNullOrEmpty(txtPatientDob.Text) || dtPatientDob.EditValue != null) && typeCodeFind == typeCodeFind__CCCDCMND)
                    {
                        isReadQrCccdData = true;
                        WaitingManager.Hide();
                        this.ProcessSearchByCode(txtPatientCode.Text);
                    }
                    else
                    {
                        this.SearchPatientByFilterCombo();
                    }
                    this.ProcessWhileChangeDOb();
                    System.Threading.Thread.Sleep(100);
                    if (this.txtAge.Enabled)
                    {
                        this.txtAge.Focus();
                        this.txtAge.SelectAll();
                    }
                    else
                    {
                        this.txtPatientTypeCode.Focus();
                        this.txtPatientTypeCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAge_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //Nếu đơn vị tính là giờ
                    if (Inventec.Common.TypeConvert.Parse.ToInt64(cboAge.EditValue.ToString()) == Gio)
                    {
                        //So gio kha dung
                        int hour = DateTime.Now.Hour;
                        if (Inventec.Common.TypeConvert.Parse.ToInt64(txtAge.Text) > hour)
                        {
                            MessageBox.Show(ResourceMessage.NhapNgaySinhKhongDungDinhDang);
                            this.txtAge.Focus();
                            this.txtAge.SelectAll();
                            return;
                        }
                    }

                    this.txtPatientTypeCode.Focus();
                    this.txtPatientTypeCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAge_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    this.txtPatientTypeCode.Focus();
                    this.txtPatientTypeCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboAge_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboAge != null)
                    {
                        this.txtPatientTypeCode.Focus();
                        this.txtPatientTypeCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtPatientTypeCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!this.PatientCodeKeyDown())
                    {
                        string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                        this.LoadDoiTuongCombo(strValue);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboPatientType.EditValue != null)
                    {
                        this.PatientTypeComboSelected();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    //if (this.cboPatientType.EditValue != null)
                    //{
                    //    //xuandv
                    //    var type = Inventec.Common.TypeConvert.Parse.ToInt64((this.cboPatientType.EditValue ?? "0").ToString());
                    //    if (type != HisConfigCFG.PatientTypeId__BHYT)
                    //        this._HeinCardData = new HeinCardData();
                    //    this.PatientTypeComboSelected();
                    //}
                    //else
                    //    this.FocusInServiceRoomInfo();
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
                        e.Value = string.Format("{0} - {1} {2}{3}", item.PROVINCE_NAME, item.DISTRICT_INITIAL_NAME, item.DISTRICT_NAME, (String.IsNullOrEmpty(item.COMMUNE_NAME) ? "" : " - " + item.INITIAL_NAME + " " + item.COMMUNE_NAME));
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
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
                            this.txtMaTHX.Text = dataNoCommunes[0].SEARCH_CODE_COMMUNE;

                            var districtDTO = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).SingleOrDefault(o => o.DISTRICT_CODE == dataNoCommunes[0].DISTRICT_CODE);
                            if (districtDTO != null)
                            {
                                this.LoadHuyenCombo("", districtDTO.PROVINCE_CODE, false);
                                this.cboProvince.EditValue = districtDTO.PROVINCE_CODE;
                                this.txtProvinceCode.Text = districtDTO.PROVINCE_CODE;
                                this.cboProvinceKS.EditValue = districtDTO.PROVINCE_CODE;
                                this.txtProvinceCodeKS.Text = districtDTO.PROVINCE_CODE;
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
                            this.txtMaTHX.Text = listResult[0].SEARCH_CODE_COMMUNE;

                            var districtDTO = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).Where(o => o.ID == listResult[0].DISTRICT_ID).SingleOrDefault();
                            if (districtDTO != null)
                            {
                                this.LoadHuyenCombo("", districtDTO.PROVINCE_CODE, false);
                                this.cboProvince.EditValue = districtDTO.PROVINCE_CODE;
                                this.txtProvinceCode.Text = districtDTO.PROVINCE_CODE;
                                this.cboProvinceKS.EditValue = districtDTO.PROVINCE_CODE;
                                this.txtProvinceCodeKS.Text = districtDTO.PROVINCE_CODE;
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

        private void SetSourceValueTHX(List<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO> communeADOs)
        {
            try
            {
                if (communeADOs != null)
                    this.InitComboCommon(this.cboTHX, communeADOs, "ID", "RENDERER_PDC_NAME", "SEARCH_CODE_COMMUNE");
                this.cboTHX.EditValue = null;
                this.cboTHX.Properties.Buttons[1].Visible = false;
                this.FocusShowpopup(this.cboTHX, false);
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
                        HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO commune = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboTHX.EditValue ?? 0).ToString()));
                        if (commune != null)
                        {
                            var districtDTO = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).SingleOrDefault(o => o.DISTRICT_CODE == commune.DISTRICT_CODE);
                            if (districtDTO != null)
                            {
                                this.LoadHuyenCombo("", districtDTO.PROVINCE_CODE, false);
                                this.cboProvince.EditValue = districtDTO.PROVINCE_CODE;
                                this.txtProvinceCode.Text = districtDTO.PROVINCE_CODE;
                                this.cboProvinceKS.EditValue = districtDTO.PROVINCE_CODE;
                                this.txtProvinceCodeKS.Text = districtDTO.PROVINCE_CODE;
                            }
                            this.LoadXaCombo("", commune.DISTRICT_CODE, false);
                            this.txtMaTHX.Text = commune.SEARCH_CODE_COMMUNE;
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
                        HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO commune = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboTHX.EditValue ?? 0).ToString()));
                        if (commune != null)
                        {
                            var districtDTO = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).SingleOrDefault(o => o.DISTRICT_CODE == commune.DISTRICT_CODE);
                            if (districtDTO != null)
                            {
                                this.LoadHuyenCombo("", districtDTO.PROVINCE_CODE, false);
                                this.cboProvince.EditValue = districtDTO.PROVINCE_CODE;
                                this.txtProvinceCode.Text = districtDTO.PROVINCE_CODE;
                                this.cboProvinceKS.EditValue = districtDTO.PROVINCE_CODE;
                                this.txtProvinceCodeKS.Text = districtDTO.PROVINCE_CODE;
                            }
                            this.LoadXaCombo("", commune.DISTRICT_CODE, false);
                            this.txtMaTHX.Text = commune.SEARCH_CODE_COMMUNE;
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

        private void txtHNCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.btnSave.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTHX_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.cboTHX.EditValue = null;
                    this.cboTHX.Properties.Buttons[1].Visible = false;
                    this.txtMaTHX.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkEmergency_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.chkEmergency.Checked)
                    this.EnableEmergencyWtime(true);
                else
                    this.EnableEmergencyWtime(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableEmergencyWtime(bool isCkhEmergency)
        {
            try
            {
                this.cboEmergencyTime.Enabled = isCkhEmergency;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkEmergency_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.chkEmergency.Checked == true)
                    {
                        this.EnableEmergencyWtime(true);
                        this.FocusShowpopup(this.cboEmergencyTime, false);
                    }
                    else
                    {
                        this.chkIsChronic.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtEmergency_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        this.cboEmergencyTime.EditValue = null;
                        this.FocusShowpopup(this.cboEmergencyTime, false);
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EMERGENCY_WTIME>().Where(o => o.EMERGENCY_WTIME_CODE.Contains(searchCode)).ToList();
                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.EMERGENCY_WTIME_CODE.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboEmergencyTime.EditValue = searchResult[0].ID;
                            this.txtAddress.Focus();
                            this.txtAddress.SelectAll();
                        }
                        else
                        {
                            this.cboEmergencyTime.EditValue = null;
                            this.FocusShowpopup(this.cboEmergencyTime, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEmergency_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboEmergencyTime.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_EMERGENCY_WTIME emergencyWTime = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EMERGENCY_WTIME>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(this.cboEmergencyTime.EditValue.ToString()));
                        if (emergencyWTime != null)
                        {
                            this.FocusShowpopup(this.cboTreatmentType, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEmergency_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboEmergencyTime.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_EMERGENCY_WTIME emergency = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EMERGENCY_WTIME>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(this.cboEmergencyTime.EditValue.ToString()));
                        if (emergency != null)
                        {
                            this.FocusShowpopup(this.cboTreatmentType, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPriority_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.cboOweType.Focus();
                    this.cboOweType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIntructionTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.chkIsNotRequireFee.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIntructionTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Down)
                {
                    this.dtIntructionTime.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsNotRequireFee_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.chkPriority.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtIntructionTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    this.dtIntructionTime.Update();
                    this.dtIntructionTime.Visible = false;
                    this.txtIntructionTime.Text = this.dtIntructionTime.DateTime.ToString("dd/MM/yyyy HH:mm");

                    this.chkIsNotRequireFee.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtIntructionTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.dtIntructionTime.Visible = true;
                    this.dtIntructionTime.Update();
                    this.txtIntructionTime.Text = this.dtPatientDob.DateTime.ToString("dd/MM/yyyy");

                    System.Threading.Thread.Sleep(100);
                    this.chkIsNotRequireFee.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtIntructionTime_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtIntructionTime_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    DateTime? dt = DateTimeHelper.ConvertDateTimeStringToSystemTime(this.txtIntructionTime.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtIntructionTime.EditValue = dt;
                        this.dtIntructionTime.Update();
                    }

                    this.dtIntructionTime.Visible = true;
                    this.dtIntructionTime.ShowPopup();
                    this.dtIntructionTime.Focus();
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
                    if (Inventec.Common.TypeConvert.Parse.ToInt64((this.cboPatientType.EditValue ?? "0").ToString()) == HisConfigCFG.PatientTypeId__BHYT
                        || Inventec.Common.TypeConvert.Parse.ToInt64((this.cboPatientType.EditValue ?? "0").ToString()) == HisConfigCFG.PatientTypeId__KSK
                        )
                        this.SetDefaultFocusUserControl();
                    else
                        this.FocusInServiceRoomInfo();
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
                this.SetValueHeinAddressByAddressOfPatient();
                this.SetRelativeAddress(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboProvince_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboProvince.EditValue != null
                        && this.cboProvince.EditValue != this.cboProvince.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).SingleOrDefault(o => o.PROVINCE_CODE == cboProvince.EditValue.ToString());
                        if (province != null)
                        {
                            this.LoadHuyenCombo("", province.PROVINCE_CODE, false);
                            this.txtProvinceCode.Text = province.SEARCH_CODE;
                        }
                        this.cboProvinceKS.EditValue = this.cboProvince.EditValue;
                        this.txtProvinceCodeKS.Text = txtProvinceCode.Text;
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

        private void cboProvince_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboProvince.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).SingleOrDefault(o => o.PROVINCE_CODE == this.cboProvince.EditValue.ToString());
                        if (province != null)
                        {
                            this.cboProvinceKS.EditValue = this.cboProvince.EditValue;
                            this.txtProvinceCodeKS.Text = txtProvinceCode.Text;

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

        private void txtProvinceCode_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.txtProvinceCode.Text))
                {
                    this.cboProvince.Properties.DataSource = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE);
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
                LogSystem.Warn(ex);
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
                                this.cboProvinceKS.EditValue = district.PROVINCE_CODE;
                                this.txtProvinceCodeKS.Text = district.PROVINCE_CODE;
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

        private void cboDistrict_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
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
                                this.cboProvinceKS.EditValue = district.PROVINCE_CODE;
                                this.txtProvinceCodeKS.Text = district.PROVINCE_CODE;
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

        private void cboCommune_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboCommune.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE)
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
                                SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).Where(o => o.ID == commune.DISTRICT_ID).FirstOrDefault();
                                if (district != null)
                                {
                                    this.cboProvince.EditValue = district.PROVINCE_CODE;
                                    this.txtProvinceCode.Text = district.PROVINCE_CODE;
                                    this.cboProvinceKS.EditValue = district.PROVINCE_CODE;
                                    this.txtProvinceCodeKS.Text = district.PROVINCE_CODE;
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

        private void cboCommune_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboCommune.EditValue != null
                        && this.cboCommune.EditValue != cboCommune.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE)
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
                                    this.cboProvinceKS.EditValue = district.PROVINCE_CODE;
                                    this.txtProvinceCodeKS.Text = district.PROVINCE_CODE;
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
        }

        private void txtNational_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        this.cboNational.EditValue = null;
                        this.FocusShowpopup(this.cboNational, true);
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().Where(o => o.NATIONAL_CODE.ToLower().Contains(searchCode.ToLower())).ToList();
                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.NATIONAL_CODE.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboNational.EditValue = searchResult[0].NATIONAL_NAME;
                            this.txtNationalCode.Text = searchResult[0].NATIONAL_CODE;
                            this.mpsNationalCode = searchResult[0].MPS_NATIONAL_CODE;
                            SendKeys.Send("{TAB}");
                        }
                        else
                        {
                            this.cboNational.EditValue = null;
                            this.FocusShowpopup(this.cboNational, true);
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
                    if (this.cboNational.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.SDA_NATIONAL ethnic = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().SingleOrDefault(o => o.NATIONAL_NAME == (this.cboNational.EditValue ?? "").ToString());
                        if (ethnic != null)
                        {
                            this.txtNationalCode.Text = ethnic.NATIONAL_CODE;
                            this.mpsNationalCode = ethnic.MPS_NATIONAL_CODE;
                        }
                    }
                    SendKeys.Send("{TAB}");
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
                    if (this.cboNational.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.SDA_NATIONAL data = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().SingleOrDefault(o => o.NATIONAL_NAME == (this.cboNational.EditValue ?? "").ToString());
                        if (data != null)
                        {
                            this.txtNationalCode.Text = data.NATIONAL_CODE;
                            SendKeys.Send("{TAB}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCareerCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = ((sender as DevExpress.XtraEditors.TextEdit).Text);
                    if (System.String.IsNullOrEmpty(searchCode))
                    {
                        this.cboCareer.EditValue = null;
                        this.FocusShowpopup(this.cboCareer, false);
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.CAREER_CODE.Contains(searchCode)).ToList();
                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.CAREER_CODE.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboCareer.EditValue = searchResult[0].ID;
                            this.txtCareerCode.Text = searchResult[0].CAREER_CODE;
                            if (this.workPlaceProcessor != null)
                                this.workPlaceProcessor.FocusControl(this.workPlaceTemplate);
                        }
                        else
                        {
                            this.cboCareer.EditValue = null;
                            this.FocusShowpopup(this.cboCareer, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCareer_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboCareer.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_CAREER career = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().SingleOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboCareer.EditValue ?? 0).ToString()));
                        if (career != null)
                        {
                            this.txtCareerCode.Text = career.CAREER_CODE;
                            if (this.workPlaceProcessor != null)
                                this.workPlaceProcessor.FocusControl(this.workPlaceTemplate);
                        }
                    }
                    else
                    {
                        if (this.workPlaceProcessor != null)
                            this.workPlaceProcessor.FocusControl(this.workPlaceTemplate);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCareer_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboCareer.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_CAREER data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().SingleOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboCareer.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            this.txtCareerCode.Text = data.CAREER_CODE;
                            if (this.workPlaceProcessor != null)
                                this.workPlaceProcessor.FocusControl(this.workPlaceTemplate);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtEthnic_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = ((sender as DevExpress.XtraEditors.TextEdit).Text);
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        this.cboEthnic.EditValue = null;
                        this.FocusShowpopup(this.cboEthnic, false);
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>().Where(o => o.ETHNIC_CODE.ToLower().Contains(searchCode.ToLower())).ToList();
                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.ETHNIC_CODE.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboEthnic.EditValue = searchResult[0].ETHNIC_NAME;
                            this.txtEthnicCode.Text = searchResult[0].ETHNIC_CODE;
                            this.txtNationalCode.Focus();
                            this.txtNationalCode.SelectAll();
                        }
                        else
                        {
                            this.cboEthnic.EditValue = null;
                            this.FocusShowpopup(this.cboEthnic, false);
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
                    if (this.cboEthnic.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.SDA_ETHNIC ethnic = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>().SingleOrDefault(o => o.ETHNIC_NAME == (this.cboEthnic.EditValue ?? "").ToString());
                        if (ethnic != null)
                        {
                            this.txtEthnicCode.Text = ethnic.ETHNIC_CODE;
                            this.txtNationalCode.Focus();
                            this.txtNationalCode.SelectAll();
                        }
                    }
                    else
                    {
                        this.txtNationalCode.Focus();
                        this.txtNationalCode.SelectAll();
                    }
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
                        SDA.EFMODEL.DataModels.SDA_ETHNIC data = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>().SingleOrDefault(o => o.ETHNIC_NAME == (this.cboEthnic.EditValue ?? "").ToString());
                        if (data != null)
                        {
                            this.txtEthnicCode.Text = data.ETHNIC_CODE;
                            this.txtNationalCode.Focus();
                            this.txtNationalCode.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTreatmentType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboTreatmentType.EditValue != null)
                    {
                        this.txtEthnicCode.Focus();
                        this.txtEthnicCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTreatmentType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.txtEthnicCode.Focus();
                    this.txtEthnicCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMilitaryRankCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        this.cboMilitaryRank.EditValue = null;
                        this.FocusShowpopup(this.cboMilitaryRank, false);
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>().Where(o => o.MILITARY_RANK_CODE.Contains(searchCode)).ToList();
                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.MILITARY_RANK_CODE.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboMilitaryRank.EditValue = searchResult[0].ID;
                            this.txtMilitaryRankCode.Text = searchResult[0].MILITARY_RANK_CODE;
                            this.txtPhone.Focus();
                            this.txtPhone.SelectAll();
                        }
                        else
                        {
                            this.cboMilitaryRank.EditValue = null;
                            this.FocusShowpopup(this.cboMilitaryRank, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMilitaryRank_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMilitaryRank.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_MILITARY_RANK commune = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(this.cboMilitaryRank.EditValue.ToString()));
                        if (commune != null)
                        {
                            this.txtMilitaryRankCode.Text = commune.MILITARY_RANK_CODE;
                        }
                    }
                    this.txtPhone.Focus();
                    this.txtPhone.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMilitaryRank_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboMilitaryRank.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_MILITARY_RANK commune = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(this.cboMilitaryRank.EditValue.ToString()));
                        if (commune != null)
                        {
                            this.txtMilitaryRankCode.Text = commune.MILITARY_RANK_CODE;
                            this.txtPhone.Focus();
                            this.txtPhone.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusAfterSelectedProgram(KeyEventArgs e)
        {
            try
            {
                if (AppConfigs.TiepDon_HienThiMotSoThongTinThemBenhNhan == 1 && e != null)
                {
                    this.btnSave.Focus();
                    if (e != null)
                    {
                        e.Handled = true;
                    }
                }
                else
                {
                    SendKeys.Send("{TAB}");
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
                    this.txtHomePerson.Focus();
                    this.txtHomePerson.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHomePerson_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.txtCorrelated.Focus();
                    this.txtCorrelated.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCorrelated_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.txtRelativeAddress.Focus();
                    this.txtRelativeAddress.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtRelativeAddress_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtRelativeCMNDNumber.Focus();
                    txtRelativeCMNDNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtGateNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (Char.IsDigit(e.KeyChar) || Char.IsControl(e.KeyChar))
                {
                }
                else
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtGateNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.txtStepNumber.Focus();
                    this.txtStepNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtStepNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (Char.IsDigit(e.KeyChar) || Char.IsControl(e.KeyChar))
                {
                }
                else
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtStepNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboOweType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.chkEmergency.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsChronic_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.cboTreatmentType.Focus();
                    this.cboTreatmentType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Button click
        private void btnPatientNew_Click(object sender, EventArgs e)
        {
            try
            {
                var strFullName = this.txtPatientName.Text;
                DateTime dob = this.dtPatientDob.DateTime;
                string txtdod = this.txtPatientDob.Text;
                var strAge = this.txtAge.Text;
                var iAge = this.cboAge.EditValue;
                var strGender = this.txtGenderCode.Text;
                var iGender = this.cboGender.EditValue;
                bool hasBobCertificate = false;
                var chkDobCertificate = this.mainHeinProcessor.GetchkHasDobCertificate(this.ucHeinBHYT);
                if (chkDobCertificate != null && chkDobCertificate.Checked)
                {
                    hasBobCertificate = true;
                }

                this.actionType = GlobalVariables.ActionAdd;
                this.ResetPatientForm();

                if (hasBobCertificate)
                {
                    this.mainHeinProcessor.UpdateHasDobCertificateEnable(this.ucHeinBHYT, hasBobCertificate);
                }

                MOS.EFMODEL.DataModels.HIS_CAREER career = null;
                if (hasBobCertificate)
                {
                    career = HisConfigCFG.CareerHS;
                }
                else
                {
                    career = HisConfigCFG.CareerBase;
                }

                if (career != null && career.ID > 0)
                {
                    this.cboCareer.EditValue = career.ID;
                    this.txtCareerCode.Text = career.CAREER_CODE;
                }

                this.txtPatientName.Text = strFullName;
                this.dtPatientDob.DateTime = dob;
                this.txtPatientDob.Text = txtdod;
                this.txtAge.Text = strAge;
                this.cboAge.EditValue = iAge;
                this.txtGenderCode.Text = strGender;
                this.cboGender.EditValue = iGender;
                this.txtPatientTypeCode.Focus();
                this.txtPatientTypeCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAddRow_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.btnAddRow.Enabled)
                    return;
                this.roomExamServiceNumber += 1;
                this.InitExamServiceRoom(false, null);
                this.roomExamServiceProcessor.FocusAndShow(this.ucRoomExamService);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnNewContinue_Click(object sender, EventArgs e)
        {
            try
            {
                this.IsReadCardTheViet = false;
                this.currentHisExamServiceReqResultSDO = null;
                this.resultHisPatientProfileSDO = null;
                this.serviceReqDetailSDOs = null;
                this.InitComboCommon(this.cboProvince, BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList(), "PROVINCE_CODE", "PROVINCE_NAME", "SEARCH_CODE");
                this.InitComboCommon(this.cboDistrict, BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList(), "DISTRICT_CODE", "RENDERER_DISTRICT_NAME", "SEARCH_CODE");
                this.InitComboCommon(this.cboCommune, BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList(), "COMMUNE_CODE", "RENDERER_COMMUNE_NAME", "SEARCH_CODE");

                this.isNotCheckTT = true;
                this.ResetPatientForm();
                this.LoadConfigOweTypeDefault(BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_OWE_TYPE>());
                this.isNotCheckTT = false;
                this.typeReceptionForm = null;
                this.txtReasonVV.Text = "";
                this.cboHosReason.EditValue = null;
                this.txtHosReason.Text = "";
                this.HospitalizeReason = "";
                this.HospitalizeReasonCode = "";
                this.HospitalizeReasonName = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //this._HeinCardData = new HeinCardData();
                this.SaveProcess(this.currentPatientSDO, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSaveAndPrint_Click(object sender, EventArgs e)
        {
            try
            {
                this.SaveProcess(this.currentPatientSDO, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnTreatmentBedRoom_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisBedRoomIn").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisBedRoomIn");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(GetTreatmentIdFromResultData());
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                //MessageBox.Show(ResourceMessage.ChucNangNayChuaDuocHoTroTrongPhienBanNay, ResourceMessage.TieuDeCuaSoThongBaoLaThongBao, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAssignService_Click(object sender, EventArgs e)
        {
            try
            {
                //if (new HIS.Desktop.Plugins.Library.AlertHospitalFeeNotBHYT.AlertHospitalFeeNotBHYTManager()
                //    .Run(
                //        GetTreatmentIdFromResultData(),
                //        Inventec.Common.TypeConvert.Parse.ToInt64((cboPatientType.EditValue ?? "0").ToString()),
                //        currentModule.RoomId
                //        ))
                //{
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignService").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.AssignService'");
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.AssignService' is not plugins");

                List<object> listArgs = new List<object>();
                AssignServiceADO assignServiceADO = new AssignServiceADO(GetTreatmentIdFromResultData(), 0, 0, null);
                if (this.typeCodeFind == this.typeCodeFind__MaHK && !String.IsNullOrEmpty(this.appointmentCode) && this._TreatmnetIdByAppointmentCode > 0)
                {
                    assignServiceADO.PreviusTreatmentId = this._TreatmnetIdByAppointmentCode;
                }
                assignServiceADO.IsAutoEnableEmergency = true;
                this.GetPatientInfoFromResultData(ref assignServiceADO);
                listArgs.Add(assignServiceADO);
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("HIS.Desktop.Plugins.AssignService INPUT assignServiceADO ", assignServiceADO));
                Inventec.Desktop.Common.Modules.Module module = PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId);
                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(module, listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                ((Form)extenceInstance).ShowDialog();
                //}
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnBill_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.CheckCashierRoom())
                {
                    if (this.GetTreatmentIdFromResultData() > 0)
                    {
                        TransactionBillADO transactionBillADO = new TransactionBillADO(this.GetTreatmentIdFromResultData(), this.currentModule.RoomId);

                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionBill").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TransactionBill'");
                        if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.TransactionBill' is not plugins");

                        List<object> listArgs = new List<object>();
                        transactionBillADO.CashierRoomId = (long)(this.cboCashierRoom.EditValue);
                        listArgs.Add(transactionBillADO);
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                // MessageBox.Show(ResourceMessage.ChucNangNayChuaDuocHoTroTrongPhienBanNay, ResourceMessage.TieuDeCuaSoThongBaoLaThongBao, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDepositDetail_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.CheckCashierRoom())
                {
                    //#15524
                    if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.IsShowDepositService == 1)
                    {
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionDeposit").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TransactionDeposit'");
                        if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                        {
                            List<object> listArgs = new List<object>();
                            TransactionDepositADO ado = new TransactionDepositADO(this.GetTreatmentFeeViewByResult(), (long)(this.cboCashierRoom.EditValue ?? 0));
                            listArgs.Add(ado);
                            listArgs.Add(moduleData);
                            var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                            if (extenceInstance == null)
                            {
                                throw new ArgumentNullException("moduleData is null");
                            }

                            ((Form)extenceInstance).ShowDialog();
                        }
                    }
                    else
                    {
                        //Review
                        DepositServiceADO depositServiceADO = new DepositServiceADO();
                        depositServiceADO.hisTreatmentId = this.GetTreatmentViewByResult().ID;

                        if (depositServiceADO.hisTreatmentId == 0)
                            throw new ArgumentNullException("hisTreatmentId is null");

                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.DepositService").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.DepositService'");
                        if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.DepositService' is not plugins");

                        List<object> listArgs = new List<object>();
                        depositServiceADO.BRANCH_ID = WorkPlace.GetBranchId();
                        depositServiceADO.CashierRoomId = (long)(this.cboCashierRoom.EditValue ?? 0);
                        listArgs.Add(depositServiceADO);
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                //MessageBox.Show(ResourceMessage.ChucNangNayChuaDuocHoTroTrongPhienBanNay, ResourceMessage.TieuDeCuaSoThongBaoLaThongBao, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private bool IsActionBtnPrint { get; set; }
        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.isPrintNow)
                {
                    if (this.currentHisExamServiceReqResultSDO == null
                    || this.currentHisExamServiceReqResultSDO.ServiceReqs == null
                    || this.currentHisExamServiceReqResultSDO.ServiceReqs.Count == 0
                    || this.actionType == GlobalVariables.ActionAdd)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.NguoiDungInPhieuYeCauKhamKhongCoDuLieuDangKyKham, ResourceMessage.TieuDeCuaSoThongBaoLaThongBao, DefaultBoolean.True);
                        return;
                    }

                    this.currentHisExamServiceReqResultSDO.ServiceReqs = this.currentHisExamServiceReqResultSDO.ServiceReqs.Where(o => this.serviceReqPrintIds.Contains(o.ID)).ToList();

                    this.PrintProcess(this.currentHisExamServiceReqResultSDO);
                }
                else
                {
                    IsActionBtnPrint = true;
                    this.InitMenuPrint();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPatientExtend_Click(object sender, EventArgs e)
        {
            try
            {
                PatientInformationADO patientTemp = null;
                if (this.currentPatientSDO != null && this.currentPatientSDO.ID > 0)
                {
                    patientTemp = new PatientInformationADO();
                    //this.patientInformation = this.patientInformation ?? new PatientInformationADO();
                    if (this.patientInformation == null || (this.patientInformation != null && !this.patientInformation.IsEdited))
                    {
                        patientTemp.BLOOD_ABO_CODE = this.currentPatientSDO.BLOOD_ABO_CODE;
                        patientTemp.BLOOD_RH_CODE = this.currentPatientSDO.BLOOD_RH_CODE;
                        patientTemp.BORN_PROVINCE_NAME = this.currentPatientSDO.BORN_PROVINCE_NAME;
                        patientTemp.BORN_PROVINCE_CODE = this.currentPatientSDO.BORN_PROVINCE_CODE;
                        if (!String.IsNullOrEmpty(this.currentPatientSDO.HT_PROVINCE_NAME))
                        {
                            patientTemp.HT_PROVINCE_NAME = this.currentPatientSDO.HT_PROVINCE_NAME;
                            var provinceHKTT = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).FirstOrDefault(o => o.PROVINCE_NAME == this.currentPatientSDO.HT_PROVINCE_NAME);
                            if (provinceHKTT != null)
                            {
                                patientTemp.HT_PROVINCE_CODE = provinceHKTT.PROVINCE_CODE;
                                if (!String.IsNullOrEmpty(this.currentPatientSDO.HT_DISTRICT_NAME))
                                {
                                    patientTemp.HT_DISTRICT_NAME = this.currentPatientSDO.HT_DISTRICT_NAME;
                                    var districtHKTT = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).FirstOrDefault(o => o.PROVINCE_ID == provinceHKTT.ID && o.DISTRICT_NAME == this.currentPatientSDO.HT_DISTRICT_NAME);
                                    if (districtHKTT != null)
                                    {
                                        patientTemp.HT_DISTRICT_CODE = districtHKTT.DISTRICT_CODE;
                                        if (!String.IsNullOrEmpty(this.currentPatientSDO.HT_COMMUNE_NAME))
                                        {
                                            patientTemp.HT_COMMUNE_NAME = this.currentPatientSDO.HT_COMMUNE_NAME;
                                            var communeHKTT = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).FirstOrDefault(o => o.DISTRICT_CODE == districtHKTT.DISTRICT_CODE && o.COMMUNE_NAME.Trim() == this.currentPatientSDO.HT_COMMUNE_NAME.Trim());
                                            if (communeHKTT != null)
                                            {
                                                patientTemp.HT_COMMUNE_CODE = communeHKTT.COMMUNE_CODE;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        patientTemp.ETHNIC_NAME = this.currentPatientSDO.ETHNIC_NAME;
                        patientTemp.NATIONAL_NAME = this.currentPatientSDO.NATIONAL_NAME;
                        patientTemp.HT_ADDRESS = this.currentPatientSDO.HT_ADDRESS;
                        patientTemp.FATHER_NAME = this.currentPatientSDO.FATHER_NAME;
                        patientTemp.MOTHER_NAME = this.currentPatientSDO.MOTHER_NAME;
                        patientTemp.RELATIVE_NAME = this.currentPatientSDO.RELATIVE_NAME;
                        patientTemp.RELATIVE_TYPE = this.currentPatientSDO.RELATIVE_TYPE;
                        patientTemp.RELATIVE_MOBILE = this.currentPatientSDO.RELATIVE_MOBILE;
                        if (!String.IsNullOrEmpty(this.currentPatientSDO.CMND_NUMBER))
                        {
                            patientTemp.CMND_NUMBER = this.currentPatientSDO.CMND_NUMBER;
                            if (this.currentPatientSDO.CMND_DATE != null)
                                patientTemp.CMND_DATE = this.currentPatientSDO.CMND_DATE ?? 0;
                            patientTemp.CMND_PLACE = this.currentPatientSDO.CMND_PLACE;
                        }
                        else if (!String.IsNullOrEmpty(this.currentPatientSDO.CCCD_NUMBER))
                        {
                            patientTemp.CMND_NUMBER = this.currentPatientSDO.CCCD_NUMBER;
                            if (this.currentPatientSDO.CCCD_DATE != null)
                                patientTemp.CMND_DATE = this.currentPatientSDO.CCCD_DATE ?? 0;
                            patientTemp.CMND_PLACE = this.currentPatientSDO.CCCD_PLACE;
                        }
                        patientTemp.EMAIL = this.currentPatientSDO.EMAIL;
                        patientTemp.MOBILE = this.currentPatientSDO.MOBILE;
                        patientTemp.HOUSEHOLD_CODE = this.currentPatientSDO.HOUSEHOLD_CODE;
                        patientTemp.HOUSEHOLD_RELATION_NAME = this.currentPatientSDO.HOUSEHOLD_RELATION_NAME;
                        patientTemp.IsPatientOld = true;
                    }
                    else if (this.patientInformation != null)
                    {
                        Inventec.Common.Mapper.DataObjectMapper.Map<PatientInformationADO>(patientTemp, this.patientInformation);
                    }
                }
                else if (this.actionType == GlobalVariables.ActionView)
                {
                    MOS.EFMODEL.DataModels.HIS_PATIENT patient = null;
                    if (this.currentHisExamServiceReqResultSDO != null)
                    {
                        patient = this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisPatient;
                    }
                    else if (this.resultHisPatientProfileSDO != null)
                    {
                        patient = this.resultHisPatientProfileSDO.HisPatient;
                    }

                    if (patient != null)
                    {
                        patientTemp = new PatientInformationADO();
                        patientTemp.BLOOD_ABO_CODE = patient.BLOOD_ABO_CODE;
                        patientTemp.BLOOD_RH_CODE = patient.BLOOD_RH_CODE;
                        patientTemp.BORN_PROVINCE_NAME = patient.BORN_PROVINCE_NAME;
                        patientTemp.BORN_PROVINCE_CODE = patient.BORN_PROVINCE_CODE;
                        if (!String.IsNullOrEmpty(patient.HT_PROVINCE_NAME))
                        {
                            patientTemp.HT_PROVINCE_NAME = patient.HT_PROVINCE_NAME;
                            var provinceHKTT = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).FirstOrDefault(o => o.PROVINCE_NAME == patient.HT_PROVINCE_NAME);
                            if (provinceHKTT != null)
                            {
                                patientTemp.HT_PROVINCE_CODE = provinceHKTT.PROVINCE_CODE;
                                if (!String.IsNullOrEmpty(patient.HT_DISTRICT_NAME))
                                {
                                    patientTemp.HT_DISTRICT_NAME = patient.HT_DISTRICT_NAME;
                                    var districtHKTT = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).FirstOrDefault(o => o.PROVINCE_ID == provinceHKTT.ID && o.DISTRICT_NAME == patient.HT_DISTRICT_NAME);
                                    if (districtHKTT != null)
                                    {
                                        patientTemp.HT_DISTRICT_CODE = districtHKTT.DISTRICT_CODE;
                                        if (!String.IsNullOrEmpty(patient.HT_COMMUNE_NAME))
                                        {
                                            patientTemp.HT_COMMUNE_NAME = patient.HT_COMMUNE_NAME;
                                            var communeHKTT = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).FirstOrDefault(o => o.DISTRICT_CODE == districtHKTT.DISTRICT_CODE && o.COMMUNE_NAME.Trim() == patient.HT_COMMUNE_NAME.Trim());
                                            if (communeHKTT != null)
                                            {
                                                patientTemp.HT_COMMUNE_CODE = communeHKTT.COMMUNE_CODE;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        patientTemp.ETHNIC_NAME = this.currentPatientSDO.ETHNIC_NAME;
                        patientTemp.NATIONAL_NAME = this.currentPatientSDO.NATIONAL_NAME;
                        patientTemp.HT_ADDRESS = this.currentPatientSDO.HT_ADDRESS;
                        patientTemp.FATHER_NAME = this.currentPatientSDO.FATHER_NAME;
                        patientTemp.MOTHER_NAME = this.currentPatientSDO.MOTHER_NAME;
                        patientTemp.RELATIVE_NAME = this.currentPatientSDO.RELATIVE_NAME;
                        patientTemp.RELATIVE_TYPE = this.currentPatientSDO.RELATIVE_TYPE;
                        patientTemp.RELATIVE_MOBILE = this.currentPatientSDO.RELATIVE_MOBILE;
                        patientTemp.CMND_NUMBER = this.currentPatientSDO.CMND_NUMBER;
                        if (this.currentPatientSDO.CMND_DATE != null)
                            patientTemp.CMND_DATE = this.currentPatientSDO.CMND_DATE ?? 0;
                        patientTemp.CMND_PLACE = this.currentPatientSDO.CMND_PLACE;
                        patientTemp.EMAIL = this.currentPatientSDO.EMAIL;
                        patientTemp.MOBILE = this.currentPatientSDO.MOBILE;
                        patientTemp.HOUSEHOLD_CODE = this.currentPatientSDO.HOUSEHOLD_CODE;
                        patientTemp.HOUSEHOLD_RELATION_NAME = this.currentPatientSDO.HOUSEHOLD_RELATION_NAME;
                        patientTemp.IsPatientOld = true;
                    }
                }
                else if (this.patientInformation != null)
                {

                    patientTemp = new PatientInformationADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<PatientInformationADO>(patientTemp, this.patientInformation);
                }

                if (patientTemp == null)
                {
                    patientTemp = new PatientInformationADO();

                }

                if (isReadQrCccdData)
                {
                    patientTemp.CMND_NUMBER = CccdReadFromQr;
                    patientTemp.CMND_DATE = dateReleaseFromQr;
                }

                patientTemp.HT_PROVINCE_CODE = txtProvinceCode.Text;
                patientTemp.HT_DISTRICT_CODE = txtDistrictCode.Text;
                patientTemp.HT_COMMUNE_CODE = txtCommuneCode.Text;
                patientTemp.HT_ADDRESS = txtAddress.Text;
                patientTemp.BORN_PROVINCE_NAME = cboProvinceKS.Text;
                patientTemp.BORN_PROVINCE_CODE = (cboProvinceKS.EditValue ?? "").ToString();
                patientTemp.ETHNIC_NAME = cboEthnic.Text;
                patientTemp.CAREER_NAME = cboCareer.Text;
                if (cboCareer.EditValue != null)
                    patientTemp.CAREER_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboCareer.EditValue.ToString());
                patientTemp.NATIONAL_NAME = cboNational.Text;
                patientTemp.RELATIVE_NAME = txtHomePerson.Text;
                patientTemp.RELATIVE_TYPE = txtCorrelated.Text;
                patientTemp.RELATIVE_ADDRESS = txtRelativeAddress.Text;
                patientTemp.RELATIVE_CMND_NUMBER = txtRelativeCMNDNumber.Text;
                patientTemp.IsNoCCCD = chkNoCCCD.Checked;
                frmPatientExtend frmPatientExtend = new frmPatientExtend(patientTemp, PatientInfoResult);
                frmPatientExtend.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCallPatient_Click(object sender, EventArgs e)
        {
            try
            {
                btnCallPatient.Focus();
                this.CreateThreadCallPatient();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRecallPatient_Click(object sender, EventArgs e)
        {
            try
            {
                btnRecallPatient.Focus();
                if (!btnCallPatient.Enabled)
                    return;
                if (String.IsNullOrEmpty(txtGateNumber.Text) || String.IsNullOrEmpty(txtStepNumber.Text))
                    return;
                if (AppConfigs.DangKyTiepDonGoiBenhNhanBangCPA == "1")
                {
                    if (this.clienttManager == null)
                        this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager();
                    this.clienttManager.RecallNumOrder(int.Parse(txtGateNumber.Text), int.Parse(txtStepNumber.Text));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #endregion

        #region Public method
        public void Save()
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionAdd && this.btnSave.Enabled == true)
                    this.btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SaveAndPrint()
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionAdd)
                    this.btnSaveAndPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void New()
        {
            try
            {
                this.btnNewContinue_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void PatientNew()
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionAdd && this.lcibtnPatientNewInfo.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    this.btnPatientNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void AssignService()
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionView)
                    this.btnAssignService_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void InBed()
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionView)
                {
                    this.btnTreatmentBedRoom_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Bill()
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionView && this.btnBill.Enabled && lcibtnBill.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    this.btnBill_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Deposit()
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionView && this.btnDepositDetail.Enabled && lcibtnDeposit.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    this.btnDepositDetail_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Print()
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionView)
                {
                    this.btnPrint_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ClickF2()
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionAdd)
                {
                    this.txtPatientCode.Focus();
                    this.txtPatientCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ClickF3()
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionAdd)
                {
                    this.FocusInServiceRoomInfo();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ClickF4()
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionAdd)
                {
                    this.txtEthnicCode.Focus();
                    this.txtEthnicCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ClickF5()
        {
            try
            {
                this.btnCallPatient_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void ClickF6()
        {
            try
            {
                this.btnRecallPatient_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void ClickF7()
        {
            try
            {
                if (cboPatientType.EditValue != null)
                {
                    long patientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString());

                    if (patientTypeId == HisConfigCFG.PatientTypeId__BHYT)
                    {
                        if (this.mainHeinProcessor != null && this.ucHeinBHYT != null)
                        {
                            this.mainHeinProcessor.DefaultFocusUserControl(this.ucHeinBHYT);
                        }
                    }
                    else if (patientTypeId == HisConfigCFG.PatientTypeId__KSK)
                    {
                        if (this.kskContractProcessor != null && this.ucKskContract != null)
                        {
                            this.kskContractProcessor.InFocus(this.ucKskContract);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void txtPatientName_Validated(object sender, EventArgs e)
        {
            try
            {
                CheckTTFull(_HeinCardData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientDob_Validated(object sender, EventArgs e)
        {
            try
            {
                CheckTTFull(_HeinCardData);
                //CheckTT(3, txtPatientDob.Text);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtRelativeCMNDNumber_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtProvinceCodeKS.Focus();
                    txtProvinceCodeKS.SelectAll();
                    //this.btnSave.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProvinceKS_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboProvinceKS.EditValue != null
                        && this.cboProvinceKS.EditValue != this.cboProvinceKS.OldEditValue)
                    {
                        var pro = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).FirstOrDefault(o => o.PROVINCE_CODE == this.cboProvinceKS.EditValue.ToString());
                        if (pro != null)
                        {
                            txtProvinceCodeKS.Text = pro.PROVINCE_CODE;
                        }
                    }

                    this.cboPatientClassify.Focus();
                    //this.btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProvinceKS_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboProvinceKS.EditValue != null)
                    {
                        var pro = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).FirstOrDefault(o => o.PROVINCE_CODE == this.cboProvinceKS.EditValue.ToString());
                        if (pro != null)
                        {
                            txtProvinceCodeKS.Text = pro.PROVINCE_CODE;
                        }

                        this.cboPatientClassify.Focus();
                        //this.btnSave.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtProvinceCodeKS_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.txtProvinceCode.Text))
                {
                    this.cboProvinceKS.Properties.DataSource = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtProvinceKH_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = txtProvinceCodeKS.Text;
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        this.cboProvinceKS.EditValue = null;
                        this.FocusShowpopup(this.cboProvinceKS, false);
                    }
                    else
                    {
                        List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                        listResult = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).Where(o => o.SEARCH_CODE.Contains(searchCode)).ToList();
                        if (listResult.Count == 1)
                        {
                            this.cboProvinceKS.EditValue = listResult[0].PROVINCE_CODE;
                            this.txtProvinceCodeKS.Text = listResult[0].SEARCH_CODE;
                            this.btnSave.Focus();
                            e.Handled = true;
                        }
                        else
                        {
                            this.cboProvinceKS.EditValue = null;
                            this.btnSave.Focus();
                            e.Handled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtIntructionTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!ValidIntructionTimeInput())
                    return;

                DateTime? dt = DateTimeHelper.ConvertDateTimeStringToSystemTime(this.txtIntructionTime.Text);
                if (dt != null && dt.Value != DateTime.MinValue)
                {
                    this.dtIntructionTime.EditValue = dt;
                    this.dtIntructionTime.Update();

                    if (dtIntructionTime.EditValue != null)
                    {
                        if (this.loadExecuteRoomProcess != null)
                            this.loadExecuteRoomProcess.TimerStop();

                        this.loadExecuteRoomProcess = new LoadExecuteRoomProcess(this.dtIntructionTime.DateTime);
                        this.loadExecuteRoomProcess.LoadDataExecuteRoomInfo();

                        bool IsCapCuuByBranchTime = true;
                        if (this._IsUserBranchTime)
                        {
                            if (this._BranchTimes != null && this._BranchTimes.Count > 0)
                            {
                                int day = (int)dtIntructionTime.DateTime.DayOfWeek;
                                var timeOfDay = dtIntructionTime.DateTime.ToString("HHmmss");
                                var dataTimes = this._BranchTimes.Where(p => p.DAY == (day + 1)).ToList();
                                if (dataTimes != null && dataTimes.Count > 0)
                                {
                                    foreach (var item in dataTimes)
                                    {
                                        long timeFrom = Inventec.Common.TypeConvert.Parse.ToInt64(item.FROM_TIME);
                                        long timeTo = Inventec.Common.TypeConvert.Parse.ToInt64(item.TO_TIME);
                                        if (timeFrom <= Inventec.Common.TypeConvert.Parse.ToInt64(timeOfDay)
                                            && Inventec.Common.TypeConvert.Parse.ToInt64(timeOfDay) <= timeTo)
                                        {
                                            IsCapCuuByBranchTime = false;
                                            break;
                                        }

                                    }
                                }
                            }

                            this._IsDungTuyenCapCuuByTime = IsCapCuuByBranchTime;

                            if (IsCapCuuByBranchTime)
                            {
                                chkEmergency.Checked = true;
                                if (this.ucHeinBHYT != null && this.mainHeinProcessor != null)
                                {
                                    this.mainHeinProcessor.AutoCheckRightRoute(this.ucHeinBHYT, true);
                                }
                            }
                            else
                            {
                                chkEmergency.Checked = false;
                                if (this.ucHeinBHYT != null && this.mainHeinProcessor != null)
                                {
                                    this.mainHeinProcessor.AutoCheckRightRoute(this.ucHeinBHYT, false);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.cboPatientType.EditValue != null)
                {
                    //xuandv
                    var type = Inventec.Common.TypeConvert.Parse.ToInt64((this.cboPatientType.EditValue ?? "0").ToString());
                    if (type != HisConfigCFG.PatientTypeId__BHYT)
                        this._HeinCardData = new HeinCardData();
                    this.PatientTypeComboSelected();
                }
                else
                    this.FocusInServiceRoomInfo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        private void InitPopupMenuOther()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("UCRegister.PopupMenuOther.btnDHST", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickDHST)));

                menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("UCRegister.PopupMenuOther.btnTaiNanThuongTich", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickTaiNanThuongTich)));

                this.dropDownButton_Other.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickDHST(object sender, EventArgs e)
        {
            try
            {
                long _treatmentId = 0;
                if (this.resultHisPatientProfileSDO != null)
                {
                    _treatmentId = this.resultHisPatientProfileSDO.HisTreatment.ID;
                }
                else if (this.currentHisExamServiceReqResultSDO != null)
                {
                    _treatmentId = this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisTreatment.ID;
                }

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisDhst").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisDhst");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(_treatmentId);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }

                #region -----------------------------
                //HIS_TREATMENT _treatment = new HIS_TREATMENT();
                //HIS_PATIENT_TYPE_ALTER _alter = new HIS_PATIENT_TYPE_ALTER();
                //if (this.resultHisPatientProfileSDO != null)
                //{
                //    _treatment = this.resultHisPatientProfileSDO.HisTreatment;
                //    _alter = this.resultHisPatientProfileSDO.HisPatientTypeAlter;
                //}
                //else if (this.currentHisExamServiceReqResultSDO != null)
                //{
                //    _treatment = this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisTreatment;
                //    _alter = this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisPatientTypeAlter;
                //}

                //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisDHSTInfo").FirstOrDefault();
                //if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.HisDHSTInfo'");
                //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                //{
                //    List<object> listArgs = new List<object>();

                //    listArgs.Add(_treatment);
                //    listArgs.Add(_alter);
                //    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                //    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                //    if (extenceInstance == null)
                //    {
                //        throw new ArgumentNullException("moduleData is null");
                //    }

                //    ((Form)extenceInstance).ShowDialog();
                //}
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickTaiNanThuongTich(object sender, EventArgs e)
        {
            try
            {
                long _treatmentId = 0;
                if (this.resultHisPatientProfileSDO != null)
                {
                    _treatmentId = this.resultHisPatientProfileSDO.HisTreatment.ID;
                }
                else if (this.currentHisExamServiceReqResultSDO != null)
                {
                    _treatmentId = this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisTreatment.ID;
                }
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AccidentHurt").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AccidentHurt");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(_treatmentId);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dropDownButton_Other_Click(object sender, EventArgs e)
        {
            try
            {
                this.dropDownButton_Other.ShowDropDown();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ValidIntructionTimeInput()
        {
            bool valid = false;
            try
            {
                if (string.IsNullOrEmpty(this.txtIntructionTime.Text))
                {
                    return false;
                }
                else
                {
                    try
                    {
                        DateTime.ParseExact(this.txtIntructionTime.Text, "dd/MM/yyyy HH:mm", null);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                        return false;
                    }
                }
                valid = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return valid;
        }

        private void txtPatientName_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtPatientName.Text))
                {
                    long genderId = GenderHelper.GenerateGenderByPatientName(txtPatientName.Text.Trim());
                    if (genderId > 0)
                    {
                        var searchResult = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().Where(o => o.ID == genderId).FirstOrDefault();
                        if (searchResult != null)
                        {
                            this.cboGender.EditValue = searchResult.ID;
                            this.txtGenderCode.Text = searchResult.GENDER_CODE;
                        }
                    }
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

        private void cboPatientClassify_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    this.btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientClassify_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.btnSave.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientClassify_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPatientClassify.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkWNext_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkWNext.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkWNext.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkWNext.Name;
                    csAddOrUpdate.VALUE = (chkWNext.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
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
                        if (item.KEY == chkWNext.Name)
                        {
                            chkWNext.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkSignExam.Name)
                        {
                            chkSignExam.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkSignExam_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkSignExam.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkSignExam.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkSignExam.Name;
                    csAddOrUpdate.VALUE = (chkSignExam.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTreatmentType_EditValueChanged(object sender, EventArgs e)
        {

            try
            {
                cboHosReason.EditValue = null;
                dxValidationProviderControl.SetValidationRule(cboHosReason, null);
                panelLayoutHosReason.Visible = false;
                layoutControlItem16.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlReasonVV.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                if (cboTreatmentType.EditValue != null)
                {
                    var type = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == Int64.Parse(cboTreatmentType.EditValue.ToString()));
                    panelLayoutHosReason.Visible = type != null && type.HEIN_TREATMENT_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinTreatmentType.HeinTreatmentTypeCode.TREAT ? true : false;
                    layoutControlReasonVV.Visibility = type != null && type.HEIN_TREATMENT_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinTreatmentType.HeinTreatmentTypeCode.TREAT ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem16.Visibility = type != null && type.HEIN_TREATMENT_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinTreatmentType.HeinTreatmentTypeCode.TREAT ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                    if (HisConfigCFG.InHospitalizationReasonRequired && panelLayoutHosReason.Visible)
                    {

                        layoutControlItem16.AppearanceItemCaption.ForeColor = Color.Maroon;
                        ValidateComboHosspitalizeReason();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }


        }





        /// <summary>
        /// chon ly do vao noi tru tu viec double click_)
        /// </summary>
        /// 
        List<HIS_HOSPITALIZE_REASON> listHospitalReason = new List<HIS_HOSPITALIZE_REASON>();
        private void loadHosReason()
        {
            try
            {
                HisHospitalizeReasonFilter reasonFilter = new HisHospitalizeReasonFilter();
                reasonFilter.IS_ACTIVE = 1;
                reasonFilter.ORDER_DIRECTION = "DESC";
                reasonFilter.ORDER_FIELD = "MODIFY_TIME";
                listHospitalReason = new BackendAdapter(new CommonParam()).Get<List<HIS_HOSPITALIZE_REASON>>("/api/HisHospitalizeReason/Get", ApiConsumers.MosConsumer, reasonFilter, new CommonParam());
                if (listHospitalReason != null)
                {
                    List<ColumnInfo> columnInfo = new List<ColumnInfo>();
                    columnInfo.Add(new ColumnInfo("HOSPITALIZE_REASON_CODE", "", 50, 1));
                    columnInfo.Add(new ColumnInfo("HOSPITALIZE_REASON_NAME", "", 150, 1));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("HOSPITALIZE_REASON_NAME", "ID", columnInfo, false, 100);
                    ControlEditorLoader.Load(cboHosReason, listHospitalReason, controlEditorADO);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void txtReasonVV_Validated(object sender, EventArgs e)
        {
            try
            {
                HospitalizeReason = txtReasonVV.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtHosReason_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
                {
                    cboHosReason.ShowPopup();
                }
                else if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboHosReason.EditValue = null;
                    GridCheckMarksSelection gridCheckMark = cboHosReason.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboHosReason.Properties.View);
                    }
                    txtHosReason.Text = "";
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void setDataHosReasonSelected(HIS_HOSPITALIZE_REASON data)
        {
            try
            {
                this.HospitalizeReasonCode = data.HOSPITALIZE_REASON_CODE ?? "";
                this.HospitalizeReasonName = data.HOSPITALIZE_REASON_NAME ?? "";

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void cboHosReason_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboHosReason.EditValue != null)
                {
                    txtHosReason.Text = this.listHospitalReason.Where(s => s.ID == Convert.ToInt64(cboHosReason.EditValue)).Select(o => o.HOSPITALIZE_REASON_NAME).FirstOrDefault();
                    setDataHosReasonSelected(this.listHospitalReason.Where(s => s.ID == Convert.ToInt64(cboHosReason.EditValue)).FirstOrDefault());
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtHosReason_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                cboHosReason.ShowPopup();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtHosReason_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtCareerCode.Focus();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtHosReason_Validated(object sender, EventArgs e)
        {
            try
            {
                var data = this.listHospitalReason.FirstOrDefault(o => o.HOSPITALIZE_REASON_NAME == txtHosReason.Text);
                if (data != null)
                {
                    cboHosReason.EditValue = data.ID;
                    setDataHosReasonSelected(data);
                }
                else
                {
                    var newHosReason = new HIS_HOSPITALIZE_REASON();
                    newHosReason.HOSPITALIZE_REASON_NAME = txtHosReason.Text;
                    setDataHosReasonSelected(newHosReason);

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkNoCCCD_CheckStateChanged(object sender, EventArgs e)
        {
        }

        private void chkNoCCCD_CheckedChanged(object sender, EventArgs e)
        {
        }
    }
}

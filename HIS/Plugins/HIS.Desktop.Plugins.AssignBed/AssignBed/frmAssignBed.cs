using ACS.EFMODEL.DataModels;
using DevExpress.Data;
using DevExpress.Office.Crypto.Agile;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.ApplicationFont;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignBed.ADO;
using HIS.Desktop.Plugins.AssignBed.Config;
using HIS.Desktop.Plugins.AssignBed.Resources;
using HIS.Desktop.Plugins.AssignBed.Validation;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ChooseICD;
using HIS.Desktop.Plugins.Library.AlertWarningFee;
using HIS.Desktop.Plugins.Library.CheckIcd;
using HIS.Desktop.Plugins.Library.PrintBordereau;
using HIS.Desktop.Plugins.Library.PrintBordereau.ADO;
using HIS.Desktop.Plugins.Library.PrintBordereau.Base;
using HIS.Desktop.Plugins.Library.PrintServiceReq;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using HIS.UC.DateEditor.ADO;
using HIS.UC.Icd;
using HIS.UC.PatientSelect;
using HIS.UC.SecondaryIcd;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Common.RichEditor.DAL;
using Inventec.Common.SignLibrary.DTO;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.LibraryMessage;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using MPS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignBed.AssignBed
{
    public partial class frmAssignBed : HIS.Desktop.Utility.FormBase
    {
        internal IcdProcessor icdYhctProcessor;
        internal UserControl ucIcdYhct;
        internal SecondaryIcdProcessor subIcdYhctProcessor;
        internal UserControl ucSecondaryIcdYhct;

        DateTime dteCommonParam { get; set; }
        HIS_TREATMENT currentTreatment = new HIS_TREATMENT();
        long ContructorIntructionTime;
        string provisionalDiagnosis;
        MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER currentHisPatientTypeAlter = null;
        Dictionary<long, List<V_HIS_SERVICE_PATY>> servicePatyInBranchs;
        List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> currentExecuteRooms;
        MOS.EFMODEL.DataModels.V_HIS_ROOM requestRoom;
        List<HIS.Desktop.LocalStorage.BackendData.ADO.SereServADO> ServiceIsleafADOs = null;
        List<DataGridAdo> DataGridAdo = null;
        List<ServiceADO> ServiceAllADOs;
        List<ServiceADO> ServiceParentADOs;
        List<ServiceADO> ServiceParentADOForGridServices;
        List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> currentPatientTypeWithPatientTypeAlter = null;
        string PatientKskCode = null;
        List<MOS.EFMODEL.DataModels.HIS_EXRO_ROOM> exroRooms;
        #region Dic phục vụ lấy combo giường
        private Dictionary<long, List<V_HIS_BED>> dicBedByServiceId;
        private List<HIS_BED_BSTY> allHisBedBstys;
        private List<V_HIS_BED_ROOM> allBedRooms;
        private List<V_HIS_BED> allBeds;
        #endregion
        HisTreatmentWithPatientTypeInfoSDO currentHisTreatment { get; set; }
        HIS.Desktop.ADO.AssignServiceADO workingAssignServiceADO;
        long treatmentId = 0;
        decimal transferTreatmentFeeBK = 0;
        decimal transferTreatmentFee = 0;
        HIS_PATIENT_TYPE patientTypeByPT;
        Inventec.Desktop.Common.Modules.Module currentModule;
        internal List<long> intructionTimeSelecteds = new List<long>();
        List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> allDataExecuteRooms;
        private List<HisBedADO> dataBedADOs;

        List<HIS_PATIENT_TYPE> currentPatientTypes;
        List<V_HIS_PATIENT_TYPE_ALLOW> currentPatientTypeAllows;
        List<HIS_PATIENT_TYPE_ROOM> PatientTypeRooms { get; set; }
        private List<HIS_DEPARTMENT_TRAN> ListDepartmentTranCheckTime = null;
        private List<HIS_CO_TREATMENT> ListCoTreatmentCheckTime = null;

        DateTime timeSelested;
        internal long InstructionTime { get; set; }
        internal List<DateTime?> intructionTimeSelected = new List<DateTime?>();
        internal PatientSelectProcessor patientSelectProcessor;
        internal UserControl ucPatientSelect;

        HIS_PATIENT currentPatient;
        V_HIS_TREATMENT_FEE treatmentPrint;
        V_HIS_PATIENT patientPrint;

        List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> sereServsInTreatmentRaw = new List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>();
        List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> sereServsInTreatment = new List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>();

        decimal totalHeinByTreatment = 0;
        decimal totalHeinPriceByTreatment = 0;

        bool isNotProcessWhileChangedTextSubIcd;
        MOS.EFMODEL.DataModels.HIS_TRACKING tracking { get; set; }
        MOS.EFMODEL.DataModels.HIS_SERVICE_REQ icdExam;
        List<HIS_ICD> currentIcds;
        HIS_ICD icdMain = null;
        bool IsObligatoryTranferMediOrg = false;
        bool isAutoCheckIcd;
        List<HIS_ICD_SERVICE> icdServicePhacDos { get; set; }
        List<HIS_SERE_SERV> lstSereServExist = new List<HIS_SERE_SERV>();
        List<long> ServicePDDTIds { get; set; }
        int actionType = 0;
        bool isCheckAssignServiceSimultaneityOption = false;
        internal bool isMultiDateState = false;

        Dictionary<long, List<HIS_PATIENT_TYPE>> dicPatientType = new Dictionary<long, List<HIS_PATIENT_TYPE>>();
        List<long> patientTypeIdAls;
        Dictionary<long, V_HIS_SERVICE> dicServices;
        List<V_HIS_SERVICE> lstService = null;
        long patientDob;
        HIS_DHST currentDhst;
        bool assignMulti = false;

        bool isNotHandlerWhileChangeToggetSwith;
        List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> sereServWithTreatment = new List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>();
        HIS_DEPARTMENT currentDepartment = null;
        string _TextIcdName = "";
        string _TextIcdNameCause = "";
        const int MaxReq = 500;
        bool IsTreatmentInBedRoom;
        HisServiceReqListResultSDO serviceReqComboResultSDO { get; set; }
        V_HIS_ROOM currentWorkingRoom = null;

        long previusTreatmentId = 0;
        List<LoaiPhieuInADO> lstLoaiPhieu;
        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.AssignBed";
        List<HIS_ROOM_TIME> roomTimes;

        HIS.Desktop.ADO.AssignServiceADO.DelegateProcessDataResult processDataResult;
        HIS.Desktop.ADO.AssignServiceADO.DelegateProcessRefeshIcd processRefeshIcd;
        long? serviceReqParentId;
        V_HIS_SERE_SERV currentSereServ { get; set; }
        string genderName;
        string patientName;
        bool isAutoEnableEmergency;
        bool isPriority;
        long? examRegisterRoomId;
        bool isNotUseBhyt;
        bool isNotLoadWhileChangeInstructionTimeInFirst;
        MOS.EFMODEL.DataModels.HIS_SERVICE_REQ serviceReqMain { get; set; }
        List<L_HIS_ROOM_COUNTER_1> hisRoomCounters;
        List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_SAME> currentServiceSames;
        private bool IsFirstLoad = false;
        ToolTipControlInfo lastInfo = null;
        int lastRowHandle = -1;
        GridColumn lastColumn = null;
        HIS_SERE_SERV hisSereServForGetPatientType = null;

        bool notSearch;

        int groupType__ServiceTypeName = 1;
        int groupType__PtttGroupName = 2;

        List<long> serviceTypeIdSplitReq { get; set; }
        List<long> serviceTypeIdRequired { get; set; }

        long serviceIdClick;
        long testSampleTypeId;
        private List<HIS_TEST_SAMPLE_TYPE> dataListTestSampleType;
        string[] periodSeparators = new string[] { "," };
        bool IsClosingForm = false;

        private Dictionary<string, string> dicValidIcd = new Dictionary<string, string>();
        List<string> ListMessError = new List<string>();
        CheckIcdManager checkIcdManager { get; set; }
        List<HIS.Desktop.Plugins.AssignBed.ADO.IcdADO> icdSubcodeAdoChecks;
        int positionHandleControl = -1;
        bool isYes = false;
        List<HIS_SERVICE_CONDITION> lstConditionService;
        bool IsFirstloadConditionService;
        bool IsActionKey = false;
        V_HIS_SERE_SERV currentSereServInEkip { get; set; }
        Dictionary<long, string> dicSessionCode = new Dictionary<long, string>();
        bool isInKip;
        decimal totalGuaranteeOriginal = 0;
        decimal totalGuaranteePrice_1 = 0;
        Dictionary<long, HisServiceReqListResultSDO> dicServiceReqList = new Dictionary<long, HisServiceReqListResultSDO>();

        bool isSaveAndPrint = false;
        bool isPrinted = false;
        bool IsSaveAndShowMps000102 = true;
        MPS.ProcessorBase.PrintConfig.PreviewType? PreviewTypeMps000102 = null;
        DataGridAdo currentRowSereServADO;
        public enum TypeButton
        {
            SAVE,
            SAVE_AND_PRINT,
            EDIT
        }

        public frmAssignBed(Inventec.Desktop.Common.Modules.Module module, HIS.Desktop.ADO.AssignServiceADO dataADO)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
                this.workingAssignServiceADO = dataADO;
                this.InitUC();
                if (dataADO != null)
                {
                    this.treatmentId = dataADO.TreatmentId;
                    this.tracking = dataADO.Tracking;
                    this.icdExam = dataADO.IcdExam;
                    this.ContructorIntructionTime = dataADO.IntructionTime;
                    this.provisionalDiagnosis = dataADO.ProvisionalDiagnosis;
                    this.previusTreatmentId = dataADO.PreviusTreatmentId;

                    this.processDataResult = dataADO.DgProcessDataResult;
                    this.processRefeshIcd = dataADO.DgProcessRefeshIcd;
                    this.serviceReqParentId = dataADO.ServiceReqId;
                    this.isInKip = dataADO.IsInKip;
                    //this.isAssignInPttt = dataADO.IsAssignInPttt;
                    if (this.isInKip)
                        this.currentSereServInEkip = dataADO.SereServ;
                    else
                        this.currentSereServ = dataADO.SereServ;

                    this.patientName = dataADO.PatientName;
                    this.patientDob = dataADO.PatientDob;
                    this.genderName = dataADO.GenderName;
                    this.currentDhst = dataADO.Dhst;
                    this.isAutoEnableEmergency = dataADO.IsAutoEnableEmergency;
                    this.isPriority = dataADO.IsPriority;
                    this.examRegisterRoomId = dataADO.ExamRegisterRoomId;
                    this.isNotUseBhyt = dataADO.IsNotUseBhyt.HasValue && dataADO.IsNotUseBhyt.Value;
                    this.GetExroRoom();
                }

                if (this.currentModule != null)
                {
                    currentWorkingRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmAssignBed_Load(object sender, EventArgs e)
        {
            try
            {
                this.LoadHisServiceFromRam();
                this.requestRoom = GetRequestRoom(this.currentModule.RoomId);
                this.currentPatientTypes = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                this.currentPatientTypeAllows = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                this.allDataExecuteRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                this.IsFirstloadConditionService = true;
                this.SetDefaultData(true);
                if (this.treatmentId > 0)
                {
                    this.LoadTotalSereServByHeinWithTreatmentAsync(this.treatmentId);
                    this.FillAllPatientInfoSelectedInForm();
                    var patientTypePrimary = this.currentPatientTypeWithPatientTypeAlter.Where(o => o.IS_ADDITION == (short)1).ToList();
                    this.InitComboPrimaryPatientType(patientTypePrimary);
                    this.InitComboExecuteRoom();
                    this.LoadTreatmentInfo__PatientType();
                    this.LoadDataDhst();
                    this.LoadDataToGridParticipants();
                }

                this.InitComboUser();
                this.currentTreatment = this.GetTreatment(this.treatmentId);
                this.LoadAllBedData();
                this.InitUCPatientSelect();
                this.InitConfig();
                this.CheckOverTotalPatientPrice();
                this.InitUcIcdYhct();
                this.InitUcSecondaryIcdYhct();
                this.BindTree();
                IsFirstLoad = true;
                this.InitComboRepositoryPatientType(this.currentPatientTypeWithPatientTypeAlter);
                //this.InitGridLookUpEditBed();
                CheckEnableBtnQR();
                InitGridLookUpEditBed();
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
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.AssignBed.Resources.Lang", typeof(frmAssignBed).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.barButtonItem2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem3.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.barButtonItem3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem4.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.barButtonItem4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem5.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.barButtonItem5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboIcdsCause.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignBed.cboIcdsCause.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboIcds.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignBed.cboIcds.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewServiceProcess.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("frmAssignBed.gridViewServiceProcess.OptionsFind.FindNullPrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcServiceCode_TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.grcServiceCode_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnExecuteRoomName__TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.gridColumnExecuteRoomName__TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnPatientTypeName__TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.gridColumnPatientTypeName__TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Service_PrimaryPatientType.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.gridColumn_Service_PrimaryPatientType.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemGridLookUpEditBed.NullText = Inventec.Common.Resource.Get.Value("frmAssignBed.repositoryItemGridLookUpEditBed.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemGridLookUpEditPatientType.NullText = Inventec.Common.Resource.Get.Value("frmAssignBed.repositoryItemGridLookUpEditPatientType.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemcboExcuteRoom_TabService.NullText = Inventec.Common.Resource.Get.Value("frmAssignBed.repositoryItemcboExcuteRoom_TabService.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemcboShareCount.NullText = Inventec.Common.Resource.Get.Value("frmAssignBed.repositoryItemcboShareCount.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemCboPrimaryPatientType.NullText = Inventec.Common.Resource.Get.Value("frmAssignBed.repositoryItemCboPrimaryPatientType.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemcboExcuteRoomPlus_TabService.NullText = Inventec.Common.Resource.Get.Value("frmAssignBed.repositoryItemcboExcuteRoomPlus_TabService.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnServiceReqList.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.btnServiceReqList.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSaveAndPrint.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.btnSaveAndPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.btnNew.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnQRPay.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.btnQRPay.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDepositService.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.btnDepositService.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCreateBill.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.btnCreateBill.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboCashierRoom.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignBed.cboCashierRoom.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPrintDocumentSigned.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.chkPrintDocumentSigned.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPrint.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.chkPrint.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSign.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.chkSign.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblCaptionFortoggleSwitchDataChecked.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.lblCaptionFortoggleSwitchDataChecked.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.toggleSwitchDataChecked.Properties.OffText = Inventec.Common.Resource.Get.Value("frmAssignBed.toggleSwitchDataChecked.Properties.OffText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.toggleSwitchDataChecked.Properties.OnText = Inventec.Common.Resource.Get.Value("frmAssignBed.toggleSwitchDataChecked.Properties.OnText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem20.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignBed.layoutControlItem20.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.layoutControlItem20.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.layoutControlItem21.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem22.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.layoutControlItem22.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem23.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.layoutControlItem23.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem24.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.layoutControlItem24.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem26.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.layoutControlItem26.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem33.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.layoutControlItem33.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem34.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.layoutControlItem34.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem35.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.layoutControlItem35.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciForlblConThua.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.lciForlblConThua.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciForlblSoDuTaiKhoan.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignBed.lciForlblSoDuTaiKhoan.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciForlblSoDuTaiKhoan.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.lciForlblSoDuTaiKhoan.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem28.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignBed.layoutControlItem28.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem28.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.layoutControlItem28.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem29.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.layoutControlItem29.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem36.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.layoutControlItem36.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem30.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.layoutControlItem30.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem31.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.layoutControlItem31.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkEditIcdCause.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.chkEditIcdCause.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboUser.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignBed.cboUser.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtIcdText.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmAssignBed.txtIcdText.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkEditIcd.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.chkEditIcd.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIcdText.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.lciIcdText.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIcdTextCause.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.lciIcdTextCause.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.gridColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn18.Caption = Inventec.Common.Resource.Get.Value("frmAssignBed.gridColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmAssignBed.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadHisServiceFromRam()
        {
            try
            {
                lstService = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool IsAllowShowEditPrice(int rowHandle)
        {
            bool valid = true;
            try
            {
                long patientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewServiceProcess.GetRowCellValue(rowHandle, "PATIENT_TYPE_ID") ?? "0").ToString());
                long primaryPatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewServiceProcess.GetRowCellValue(rowHandle, "PRIMARY_PATIENT_TYPE_ID") ?? "0").ToString());
                if (patientTypeId == HisConfigCFG.PatientTypeId__BHYT || primaryPatientTypeId == HisConfigCFG.PatientTypeId__BHYT)
                {
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

        private void gridViewServiceProcess_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    DataGridAdo data = (DataGridAdo)gridViewServiceProcess.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "PATIENT_TYPE_ID")
                    {
                        e.RepositoryItem = this.repositoryItemGridLookUpEditPatientType;
                    }
                    else if (e.Column.FieldName == "PRIMARY_PATIENT_TYPE_ID")
                    {
                        e.RepositoryItem = this.repositoryItemCboPrimaryPatientType;
                    }
                    else if (e.Column.FieldName == "PRICE_DISPLAY")
                    {
                        //- Chỉ hiển thị icon này, nếu dịch vụ là phẫu thuật, hoặc dịch vụ có cấu hình "Gói dịch vụ" (có package_id), và:
                        //+ Khoa người dùng làm việc có cấu hình "Cho phép chỉ định giá phẫu thuật" --> hiển thị icon "sửa" ở ô "Giá" 
                        //+ Khoa người dùng làm việc có cấu hình "Cho phép chỉ định giá gói" --> hiển thị icon "sửa" ở ô "Giá gói" 
                        //Lưu ý: chỉ cho sửa 1 trong 2 trường ("giá" hoặc "giá gói"), chứ ko cho phép sửa cả 2. Ưu tiên "giá gói"
                        //Lưu ý: Ko cho sửa nếu ĐTTT hoặc đối tượng phụ thu là BHYT

                        if (data.PATIENT_TYPE_CODE == HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.KSK"))
                        {
                            e.RepositoryItem = repositoryItembtnEditDonGia_TextDisable;
                        }
                        else
                        {
                            bool isEditCtrol = data.IS_ENABLE_ASSIGN_PRICE == 1;
                            isEditCtrol = isEditCtrol && IsAllowShowEditPrice(e.RowHandle);
                            if (data != null && !isEditCtrol)
                                e.RepositoryItem = repositoryItemTxtReadOnly;
                            else
                                e.RepositoryItem = repositoryItembtnEditDonGia_TextDisable;
                        }
                    }
                    else if (e.Column.FieldName == "TIME_FROM")
                    {
                        // Kiểm tra row có được tích chọn không (MultiSelect)
                        bool isSelected = view.IsRowSelected(e.RowHandle);

                        if (isSelected)
                        {
                            // Lấy giá trị TIME_FROM hiện tại của row
                            var timeFromValue = view.GetRowCellValue(e.RowHandle, "TIME_FROM");

                            // Nếu chưa có giá trị, set giá trị mặc định
                            if (timeFromValue == null || timeFromValue == DBNull.Value)
                            {
                                DateTime valueToSet;

                                // Kiểm tra currentTreatment.IN_TIME
                                if (currentTreatment != null && currentTreatment.IN_TIME != null)
                                {
                                    DateTime inTime = Inventec.Common.DateTime.Convert
                                        .TimeNumberToSystemDateTime(currentTreatment.IN_TIME) ?? DateTime.Now;

                                    // Kiểm tra IN_TIME có phải ngày hôm nay không
                                    if (inTime.Date == DateTime.Now.Date)
                                    {
                                        valueToSet = inTime;
                                    }
                                    else
                                    {
                                        valueToSet = DateTime.Now;
                                    }
                                }
                                else
                                {
                                    valueToSet = DateTime.Now;
                                }
                                repositoryItemDateEditTimeFrom.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                                repositoryItemDateEditTimeFrom.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";

                                repositoryItemDateEditTimeFrom.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                                repositoryItemDateEditTimeFrom.EditFormat.FormatString = "dd/MM/yyyy HH:mm";

                                // QUAN TRỌNG NHẤT
                                repositoryItemDateEditTimeFrom.Mask.EditMask = "dd/MM/yyyy HH:mm";
                                repositoryItemDateEditTimeFrom.Mask.UseMaskAsDisplayFormat = true;

                                // Set giá trị cho cell
                                view.SetRowCellValue(e.RowHandle, "TIME_FROM", valueToSet);
                            }

                            // Sử dụng repository mặc định
                            e.RepositoryItem = repositoryItemDateEditTimeFrom;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemDateEditTimeFrom;
                        }
                    }
                    else if (e.Column.FieldName == "QUANTITY")
                    {
                        // Kiểm tra xem dòng có được tích chọn không
                        bool isSelected = view.IsRowSelected(e.RowHandle);

                        if (isSelected)
                        {
                            // Lấy giá trị QUANTITY hiện tại
                            var quantityValue = view.GetRowCellValue(e.RowHandle, "QUANTITY");

                            // Nếu chưa có giá trị, set giá trị mặc định là 1
                            if (quantityValue == null || quantityValue == DBNull.Value)
                            {
                                view.SetRowCellValue(e.RowHandle, "QUANTITY", 1);
                            }

                            e.RepositoryItem = repositoryItemSpinEditQuantity;
                        }
                        else
                        {
                            e.RepositoryItem = null; // Hoặc repository mặc định
                        }
                    }
                    else if (e.Column.FieldName == "TIME_TO")
                    {
                        // Kiểm tra xem dòng có được tích chọn không
                        bool isSelected = view.IsRowSelected(e.RowHandle);

                        if (isSelected)
                        {
                            // Lấy giá trị TIME_FROM
                            var timeFromValue = view.GetRowCellValue(e.RowHandle, "TIME_FROM");
                            // Lấy giá trị QUANTITY
                            var quantityValue = view.GetRowCellValue(e.RowHandle, "QUANTITY");

                            if (timeFromValue != null && timeFromValue != DBNull.Value &&
                                quantityValue != null && quantityValue != DBNull.Value)
                            {
                                DateTime timeFrom;
                                decimal quantity;

                                if (timeFromValue is DateTime)
                                {
                                    timeFrom = (DateTime)timeFromValue;
                                }
                                else
                                {
                                    timeFrom = DateTime.Now;
                                }

                                if (decimal.TryParse(quantityValue.ToString(), out quantity))
                                {
                                    // Tính TIME_TO = TIME_FROM + QUANTITY (số ngày)
                                    int daysToAdd = (int)quantity;
                                    DateTime timeTo = timeFrom.Date.AddDays(daysToAdd).AddHours(23).AddMinutes(59);

                                    // Cập nhật giá trị TIME_TO vào cell
                                    view.SetRowCellValue(e.RowHandle, "TIME_TO", timeTo);
                                }
                            }

                            repositoryItemDateEditTimeTo.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                            repositoryItemDateEditTimeTo.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";

                            repositoryItemDateEditTimeTo.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                            repositoryItemDateEditTimeTo.EditFormat.FormatString = "dd/MM/yyyy HH:mm";

                            repositoryItemDateEditTimeTo.Mask.EditMask = "dd/MM/yyyy HH:mm";
                            repositoryItemDateEditTimeTo.Mask.UseMaskAsDisplayFormat = true;

                            e.RepositoryItem = repositoryItemDateEditTimeTo;
                        }
                    }
                    else if (e.Column.FieldName == "BED_CODE")
                    {
                        var selectedServiceId = data.SERVICE_ID;

                        if (selectedServiceId != null)
                        {
                            DateTime timeFrom = new DateTime();
                            DateTime timeTo = new DateTime();

                            // Kiểm tra xem dòng có được tích chọn không
                            bool isSelected = view.IsRowSelected(e.RowHandle);

                            if (isSelected)
                            {
                                // Lấy giá trị từ cột TIME_FROM
                                var timeFromValue = view.GetRowCellValue(e.RowHandle, "TIME_FROM");
                                if (timeFromValue != null && timeFromValue != DBNull.Value)
                                {
                                    if (timeFromValue is DateTime)
                                    {
                                        timeFrom = (DateTime)timeFromValue;
                                    }
                                }

                                // Lấy giá trị từ cột TIME_TO 
                                var timeToValue = view.GetRowCellValue(e.RowHandle, "TIME_TO");
                                if (timeToValue != null && timeToValue != DBNull.Value)
                                {
                                    if (timeToValue is DateTime)
                                    {
                                        timeTo = (DateTime)timeToValue;
                                    }
                                }

                                LoadBedDataByServiceId(Convert.ToInt64(selectedServiceId), timeFrom, timeTo);

                                e.RepositoryItem = repositoryItemGridLookUpEditBed;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemGridLookUpEditBed = null;
                            }
                        }

                    }
                    else if (e.Column.FieldName == "TDL_EXECUTE_ROOM_ID")
                    {
                        if (this.IsTreatmentInBedRoom && data != null && data.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G && data.TDL_EXECUTE_ROOM_ID > 0)
                        {
                            var room = currentExecuteRooms.FirstOrDefault(o => o.ROOM_ID == data.TDL_EXECUTE_ROOM_ID);
                            if (room != null)
                            {
                                e.RepositoryItem = this.repositoryItemcboExcuteRoom_TabService;
                            }
                            else
                            {
                                e.RepositoryItem = this.repositoryItemcboExcuteRoomPlus_TabService;
                            }
                        }
                        else
                        {
                            e.RepositoryItem = this.repositoryItemcboExcuteRoom_TabService;
                        }
                    }
                    else if (e.Column.FieldName == "ShareCount")
                    {
                        long serviceReqTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewServiceProcess.GetRowCellValue(e.RowHandle, "TDL_SERVICE_TYPE_ID") ?? "").ToString());
                        e.RepositoryItem = (serviceReqTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G ? this.repositoryItemcboShareCount : this.repositoryItemTxtReadOnly);
                        //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqTypeId), serviceReqTypeId) + "____" + Inventec.Common.Logging.LogUtil.TraceData("IMSys.DbConfig.HIS_RS.TDL_SERVICE_TYPE_ID.ID__G", IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task CheckOverTotalPatientPrice()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentFeeViewFilter hisTreatmentFeeViewFilter = new HisTreatmentFeeViewFilter();
                hisTreatmentFeeViewFilter.IS_ACTIVE = 1;
                hisTreatmentFeeViewFilter.ID = this.treatmentId;
                Inventec.Common.Logging.LogSystem.Debug("begin call HisTreatment/GetFeeView");
                var treatmentFees = await new BackendAdapter(param).GetAsync<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumer.ApiConsumers.MosConsumer, hisTreatmentFeeViewFilter, param);

                if (treatmentFees != null && treatmentFees.Count > 0)
                {
                    var treatmentFee = treatmentFees.FirstOrDefault();
                    //decimal totalReceiveMore = (treatmentFee.TOTAL_PATIENT_PRICE ?? 0) - (treatmentFee.TOTAL_DEPOSIT_AMOUNT ?? 0) - (treatmentFee.TOTAL_BILL_AMOUNT ?? 0) + (treatmentFee.TOTAL_BILL_TRANSFER_AMOUNT ?? 0) + (treatmentFee.TOTAL_REPAY_AMOUNT ?? 0);
                    decimal totalPrice = 0;
                    decimal totalHeinPrice = 0;
                    decimal totalPatientPrice = 0;
                    decimal totalDeposit = 0;
                    decimal totalBill = 0;
                    decimal totalBillTransferAmount = 0;
                    decimal totalRepay = 0;
                    decimal exemption = 0;
                    decimal total_obtained_price = 0;
                    totalPrice = treatmentFees[0].TOTAL_PRICE ?? 0; // tong tien
                    totalHeinPrice = treatmentFees[0].TOTAL_HEIN_PRICE ?? 0;
                    totalPatientPrice = treatmentFees[0].TOTAL_PATIENT_PRICE ?? 0; // bn tra
                    totalDeposit = treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0;
                    totalBill = treatmentFees[0].TOTAL_BILL_AMOUNT ?? 0;
                    totalBillTransferAmount = treatmentFees[0].TOTAL_BILL_TRANSFER_AMOUNT ?? 0;
                    exemption = treatmentFees[0].TOTAL_BILL_EXEMPTION ?? 0;// HospitalFeeSum[0].TOTAL_EXEMPTION ?? 0;
                    totalRepay = treatmentFees[0].TOTAL_REPAY_AMOUNT ?? 0;
                    total_obtained_price = (totalDeposit + totalBill - totalBillTransferAmount - totalRepay + exemption);//Da thu benh nhan
                    this.transferTreatmentFee = totalPatientPrice - total_obtained_price;//Phai thu benh nhan


                    lblChiPhiBNPhaiTra.Text = Inventec.Common.Number.Convert.NumberToString(totalPatientPrice, ConfigApplications.NumberSeperator);
                    lblDaDong.Text = Inventec.Common.Number.Convert.NumberToString(total_obtained_price, ConfigApplications.NumberSeperator);
                    if (this.transferTreatmentFee > 0)
                    {
                        lblConThua.Text = Inventec.Common.Number.Convert.NumberToString(Math.Abs(this.transferTreatmentFee), ConfigApplications.NumberSeperator);
                        lciForlblConThua.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciForlblConThieu.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                        lciForlblConThua.AppearanceItemCaption.ForeColor = System.Drawing.Color.Red;
                    }
                    else
                    {
                        lblConThua.Text = Inventec.Common.Number.Convert.NumberToString(Math.Abs(this.transferTreatmentFee), ConfigApplications.NumberSeperator);
                        this.lciForlblConThua.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciForlblConThua.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    }

                    this.patientTypeByPT = (currentHisPatientTypeAlter != null && currentHisPatientTypeAlter.PATIENT_TYPE_ID > 0) ? BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.ID == currentHisPatientTypeAlter.PATIENT_TYPE_ID).FirstOrDefault() : null;

                    // - Trong trường hợp ĐỐI TƯỢNG BỆNH NHÂN được check "Không cho phép chỉ định dịch vụ nếu thiếu tiền" (HIS_PATIENT_TYPE có IS_CHECK_FEE_WHEN_ASSIGN = 1) và hồ sơ là "Khám" (HIS_TREATMENT có TDL_TREATMENT_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM) thì kiểm tra:
                    //+ Nếu hồ sơ đang không thừa tiền "Còn thừa" = 0 hoặc hiển thị "Còn thiếu" thì hiển thị thông báo "Bệnh nhân đang nợ tiền, không cho phép chỉ định dịch vụ", người dùng nhấn "Đồng ý" thì tắt form chỉ định.
                    //+ Nếu hồ sơ đang thừa tiền ("Còn thừa" > 0), thì khi người dùng check chọn dịch vụ, nếu số tiền "Phát sinh" > "Còn thừa" thì hiển thị cảnh báo: "Không cho phép chỉ định dịch vụ vượt quá số tiền còn thừa" và không cho phép người dùng check chọn dịch vụ đó.
                    //+ Bỏ qua kiểm tra nợ tiền nếu bệnh nhân là bệnh nhân bảo lãnh
                    var SereSerView = new List<HIS_SERE_SERV>();
                    if (this.transferTreatmentFee > 0)
                    {

                        HisSereServFilter filterSs = new HisSereServFilter();
                        filterSs.TREATMENT_ID = treatmentId;
                        SereSerView = await new BackendAdapter(param).GetAsync<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumer.ApiConsumers.MosConsumer, filterSs, param);
                        //SereSerView = SereSerView.Where(o => o.IS_GUARANTEED != 1).ToList();
                        transferTreatmentFee = transferTreatmentFee - SereSerView.Where(o => o.IS_GUARANTEED == 1).Sum(o => o.VIR_TOTAL_PATIENT_PRICE ?? 0);
                    }

                    if (this.patientTypeByPT != null && this.patientTypeByPT.IS_CHECK_FEE_WHEN_ASSIGN == 1
                            && this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM
                            && this.transferTreatmentFee >= 0 && this.currentModule.RoomTypeId != IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__TD
                            && (this.currentHisTreatment != null && string.IsNullOrEmpty(this.currentHisTreatment.GUARANTEE_CODE))
                        )
                    {
                        SereSerView = SereSerView.Where(o => o.IS_GUARANTEED != 1).ToList();
                        if (SereSerView.Any())
                        {
                            frmDetailsSereServ frm = new frmDetailsSereServ(SereSerView.ToList(), (HIS.Desktop.Common.RefeshReference)this.Close);
                            frm.ShowDialog();
                            return;
                        }

                    }


                    if (treatmentFee.TDL_TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                    {
                        return;
                    }

                    if ((HisConfigCFG.WarningOverTotalPatientPrice__IsCheck == "1" || HisConfigCFG.WarningOverTotalPatientPrice__IsCheck == "3") && this.currentHisPatientTypeAlter != null && this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU && !string.IsNullOrEmpty(HisConfigCFG.WarningOverTotalPatientPrice) && (this.currentHisTreatment != null && string.IsNullOrEmpty(this.currentHisTreatment.GUARANTEE_CODE)))
                    {
                        decimal warningOverTotalCGF = Convert.ToInt64(HisConfigCFG.WarningOverTotalPatientPrice);

                        if (transferTreatmentFee > warningOverTotalCGF && this.transferTreatmentFeeBK != this.transferTreatmentFee)
                        {
                            if (MessageBox.Show(String.Format(ResourceMessage.BenhNhanDangThieuVienPhi, Inventec.Common.Number.Convert.NumberToString(transferTreatmentFee, ConfigApplications.NumberSeperator)), "Cảnh báo",
        MessageBoxButtons.YesNo, MessageBoxIcon.Question,
        MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.No)
                            {
                                this.Close();
                            }
                        }
                    }

                    this.transferTreatmentFeeBK = this.transferTreatmentFee;//Phai thu benh nhan
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async void InitConfig()
        {
            try
            {
                if (HisConfigCFG.IsUsingExecuteRoomPayment)
                {
                    lciForlblSoDuTaiKhoan.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.LoadUsingExecuteRoomPaymentProcess();
                }
                else
                {
                    lciForlblSoDuTaiKhoan.Text = "  ";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadUsingExecuteRoomPaymentProcess()
        {
            CommonParam param = new CommonParam();
            Inventec.Common.Logging.LogSystem.Debug("begin call HisPatient/GetCardBalance");
            var balance = await new BackendAdapter(param).GetAsync<decimal?>("api/HisPatient/GetCardBalance", ApiConsumers.MosConsumer, this.currentHisTreatment.PATIENT_ID, ProcessLostToken, param);
            Inventec.Common.Logging.LogSystem.Debug("end call HisPatient/GetCardBalance");
            lblSoDuTaiKhoan.Text = (balance.HasValue ? Inventec.Common.Number.Convert.NumberToString(balance.Value, ConfigApplications.NumberSeperator) : "0");
        }

        private void SetDefaultSerServTotalPrice()
        {
            try
            {
                decimal totalPrimary = 0, tmp = 0;
                decimal totalPrice = GetDefaultSerServTotalPrice(ref totalPrimary);
                this.lblTotalServicePrice.Text = Inventec.Common.Number.Convert.NumberToString(totalPrice, ConfigApplications.NumberSeperator);
                decimal totalPriceBhyt = GetDefaultSerServTotalPrice(ref tmp, HisConfigCFG.PatientTypeId__BHYT);
                decimal totalChecnhBhyt = 0;
                if (totalPrimary > 0 && totalPriceBhyt > 0)
                {
                    totalChecnhBhyt = totalPrimary - totalPriceBhyt;
                }
                this.lblChenhBHYT.Text = Inventec.Common.Number.Convert.NumberToString(totalChecnhBhyt, ConfigApplications.NumberSeperator);
                decimal totalPriceOther = totalPrice - totalPriceBhyt - totalChecnhBhyt;

                this.lblTotalServicePriceBhyt.Text = Inventec.Common.Number.Convert.NumberToString(totalPriceBhyt, ConfigApplications.NumberSeperator);
                this.lblTotalServicePriceOther.Text = Inventec.Common.Number.Convert.NumberToString(totalPriceOther, ConfigApplications.NumberSeperator);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private decimal GetDefaultSerServTotalPrice(ref decimal totalPrimaryPatientType, long? patientTypeId = null)
        {
            decimal totalPrice = 0;
            try
            {
                long instructionTime = this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0 ? this.intructionTimeSelecteds.FirstOrDefault() : 0;

                if (this.DataGridAdo != null && this.DataGridAdo.Count > 0)
                {
                    var dataCheckeds = this.DataGridAdo.Where(o => o.IsChecked).ToList();
                    if (patientTypeId.HasValue && patientTypeId.Value > 0)
                        dataCheckeds = dataCheckeds.Where(o => o.PATIENT_TYPE_ID == patientTypeId.Value).ToList();
                    if (dataCheckeds != null && dataCheckeds.Count > 0)
                    {
                        var serviceRoomViews = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>();
                        foreach (var item in dataCheckeds)
                        {
                            if (item.IsChecked && item.PATIENT_TYPE_ID != 0 && (item.IsExpend ?? false) == false)
                            {
                                var servicePaties = HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.ServicePatyWithListPatientType(item.SERVICE_ID, this.patientTypeIdAls);
                                V_HIS_SERVICE_PATY data_ServicePrice = null;
                                if (servicePaties != null && servicePaties.Count > 0 && this.requestRoom != null)
                                {
                                    List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> dataCombo = new List<V_HIS_EXECUTE_ROOM>();

                                    if (this.allDataExecuteRooms != null && this.allDataExecuteRooms.Count > 0 && serviceRoomViews != null && serviceRoomViews.Count > 0)
                                    {
                                        var arrExcuteRoom = serviceRoomViews.Where(o => item != null && o.SERVICE_ID == item.SERVICE_ID);

                                        if (HisConfigCFG.IsAssignRoomByPatientType && PatientTypeRooms != null && PatientTypeRooms.Count > 0 && PatientTypeRooms.Exists(o => o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID))
                                        {
                                            var RoomIds = PatientTypeRooms.Where(o => o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID).Select(o => o.ROOM_ID).ToList();
                                            arrExcuteRoom = arrExcuteRoom.Where(o => RoomIds.Contains(o.ROOM_ID)).ToList();
                                        }
                                        var arrExcuteRoomCode = arrExcuteRoom.Select(o => o.ROOM_ID).ToArray();
                                        dataCombo = ((arrExcuteRoomCode != null && arrExcuteRoomCode.Count() > 0 && this.allDataExecuteRooms != null) ? this.allDataExecuteRooms.Where(o => arrExcuteRoomCode.Contains(o.ROOM_ID)).ToList() : null);
                                    }
                                    var checkExecuteRoom = dataCombo != null && dataCombo.Count > 0 ? dataCombo.FirstOrDefault(o => o.BRANCH_ID == this.requestRoom.BRANCH_ID) : null;
                                    if (checkExecuteRoom != null)
                                    {
                                        item.TDL_EXECUTE_BRANCH_ID = checkExecuteRoom.BRANCH_ID;
                                    }
                                    else
                                    {
                                        item.TDL_EXECUTE_BRANCH_ID = dataCombo != null && dataCombo.Count > 0 ? dataCombo.FirstOrDefault().BRANCH_ID : 0;
                                        item.TDL_EXECUTE_BRANCH_ID = item.TDL_EXECUTE_BRANCH_ID == 0 ? HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.GetCurrentBranchId() : item.TDL_EXECUTE_BRANCH_ID;
                                    }

                                    List<HIS_SERE_SERV> sameServiceType = this.sereServWithTreatment != null ? this.sereServWithTreatment.Where(o => o.TDL_SERVICE_TYPE_ID == item.SERVICE_TYPE_ID).ToList() : null;
                                    List<HIS_SERE_SERV> sameService = this.sereServWithTreatment != null ? this.sereServWithTreatment.Where(o => o.SERVICE_ID == item.SERVICE_ID).ToList() : null;
                                    var intructionNumByType = sameServiceType != null ? (long)sameServiceType.Count() + 1 : 1;
                                    var intructionNum = sameService != null ? (long)sameService.Count() + 1 : 1;
                                    if (HisConfigCFG.IsSetPrimaryPatientType != "0"
                                        && item.PRIMARY_PATIENT_TYPE_ID.HasValue && !patientTypeId.HasValue)
                                    {
                                        data_ServicePrice = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePaties, item.TDL_EXECUTE_BRANCH_ID, null, this.requestRoom.ID, this.requestRoom.DEPARTMENT_ID, instructionTime, this.currentHisTreatment.IN_TIME, item.SERVICE_ID, item.PRIMARY_PATIENT_TYPE_ID.Value, intructionNum, intructionNumByType, item.PackagePriceId, item.SERVICE_CONDITION_ID, this.currentHisTreatment.TDL_PATIENT_CLASSIFY_ID, null);
                                        if (item.HEIN_LIMIT_RATIO.HasValue
                                            && item.HEIN_LIMIT_RATIO.Value > 0
                                            && data_ServicePrice != null)
                                        {
                                            totalPrimaryPatientType += (item.AMOUNT * data_ServicePrice.PRICE * (1 + data_ServicePrice.VAT_RATIO) * item.HEIN_LIMIT_RATIO.Value);
                                        }
                                        else if (data_ServicePrice != null)
                                        {
                                            totalPrimaryPatientType += (item.AMOUNT * data_ServicePrice.PRICE * (1 + data_ServicePrice.VAT_RATIO));
                                        }
                                    }
                                    else
                                    {
                                        data_ServicePrice = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePaties, item.TDL_EXECUTE_BRANCH_ID, null, this.requestRoom.ID, this.requestRoom.DEPARTMENT_ID, instructionTime, this.currentHisTreatment.IN_TIME, item.SERVICE_ID, item.PATIENT_TYPE_ID, intructionNum, intructionNumByType, item.PackagePriceId, item.SERVICE_CONDITION_ID, this.currentHisTreatment.TDL_PATIENT_CLASSIFY_ID, null);

                                    }
                                }

                                if (item.AssignSurgPriceEdit.HasValue && item.AssignSurgPriceEdit > 0)
                                {
                                    totalPrice += item.AssignSurgPriceEdit.Value;
                                }
                                else
                                {
                                    if (item.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
                                            && item.IsNoDifference.HasValue
                                            && item.IsNoDifference.Value)
                                    {
                                        if (item.HEIN_LIMIT_PRICE.HasValue
                                            && item.HEIN_LIMIT_PRICE.Value > 0)
                                        {
                                            totalPrice += item.AMOUNT * item.HEIN_LIMIT_PRICE.Value;
                                        }
                                        else if (item.HEIN_LIMIT_RATIO.HasValue
                                            && item.HEIN_LIMIT_RATIO.Value > 0
                                            && data_ServicePrice != null)
                                        {
                                            totalPrice += (item.AMOUNT * data_ServicePrice.PRICE * (1 + data_ServicePrice.VAT_RATIO) * item.HEIN_LIMIT_RATIO.Value);
                                        }
                                        else if (data_ServicePrice != null)
                                        {
                                            totalPrice += (item.AMOUNT * data_ServicePrice.PRICE * (1 + data_ServicePrice.VAT_RATIO));
                                        }
                                    }
                                    else if (data_ServicePrice != null)
                                    {

                                        totalPrice += (item.AMOUNT * data_ServicePrice.PRICE * (1 + data_ServicePrice.VAT_RATIO));
                                    }
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
            return totalPrice;
        }

        private HIS_TREATMENT GetTreatment(long treatmentId)
        {
            HIS_TREATMENT data = null;
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFilter filter = new HisTreatmentFilter();
                filter.ID = treatmentId;
                data = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return data;
        }

        private void PatientSelectedChange(V_HIS_TREATMENT_BED_ROOM data)
        {
            try
            {
                if (this.treatmentId == data.TREATMENT_ID)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Goi ham thay doi benh nhan nhung kiem tra ma dieu tri cu van nhu ma dieu tri hien tai____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data.TREATMENT_ID), data.TREATMENT_ID));
                    return;
                }
                this.treatmentId = data.TREATMENT_ID;
                this.LoadDataToCurrentTreatmentData(treatmentId, this.intructionTimeSelecteds.FirstOrDefault());
                this.SetDateUc();
                this.ProcessDataWithTreatmentWithPatientTypeInfo();
                this.CreateThreadLoadDataForPrint();
                this.LoadCurrentPatient();
                //this.InitComboKsk();
                DateTime intructTime = (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.intructionTimeSelecteds.First()) ?? DateTime.Now);
                this.LoadTotalSereServByHeinWithTreatmentAsync(this.treatmentId);
                ProcessPatientSelecttWithPatientTypeInfo();
                this.LoadServicePaty();
                this.InitComboRepositoryPatientType(this.currentPatientTypeWithPatientTypeAlter);
                var patientTypePrimary = this.currentPatientTypeWithPatientTypeAlter.Where(o => o.IS_ADDITION == (short)1).ToList();
                this.InitComboPrimaryPatientType(patientTypePrimary);
                //this.InitComboUser();
                //this.InitComboServiceGroup();
                //this.InitComboExecuteRoom();
                this.LoadTreatmentInfo__PatientType();
                this.BindTree();
                this.LoadDataDhst();
                this.InitDefaultFocus();
                assignMulti = false;
                if (workingAssignServiceADO.OpenFromBedRoomPartial && this.patientSelectProcessor != null && this.ucPatientSelect != null)
                {
                    var lstPatientSelect = this.patientSelectProcessor.GetSelectedRows(this.ucPatientSelect);
                    if (lstPatientSelect != null && lstPatientSelect.Count > 1)
                    {
                        assignMulti = true;
                    }
                }
                CheckAssignServiceSimultaneityOption();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCurrentTreatmentData(long treatmentId, long intructionTime)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentWithPatientTypeInfoFilter filter = new MOS.Filter.HisTreatmentWithPatientTypeInfoFilter();
                filter.TREATMENT_ID = treatmentId;
                if (HisConfigCFG.IsUsingServerTime == "1")
                {
                    filter.INTRUCTION_TIME = null;
                }
                else
                {
                    filter.INTRUCTION_TIME = intructionTime;
                }
                var hisTreatments = new BackendAdapter(param).Get<List<HisTreatmentWithPatientTypeInfoSDO>>("api/HisTreatment/GetTreatmentWithPatientTypeInfoSdo", ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
                this.currentHisTreatment = hisTreatments != null && hisTreatments.Count > 0 ? hisTreatments.FirstOrDefault() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDateUc()
        {
            try
            {
                if (this.currentHisTreatment != null && HisConfigCFG.IsUsingServerTime == "1"
                   && this.currentHisTreatment.SERVER_TIME > 0)
                {
                    DateInputADO ip = new DateInputADO();
                    ip.Time = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentHisTreatment.SERVER_TIME).Value;
                    ip.Dates = new List<DateTime?>() { ip.Time.Date };
                    UcDateSetValue(ip);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void UcDateSetValue(DateInputADO input)
        {
            try
            {
                if (input != null)
                {
                    //if (input.Time != null && input.Time != DateTime.MinValue)
                    //{
                    //    this.timeIntruction.EditValue = input.Time.ToString("HH:mm");
                    //}
                    //if (input.Dates != null && input.Dates.Count > 0)
                    //{
                    //    this.dtInstructionTime.EditValue = input.Dates[0];
                    this.intructionTimeSelected = new List<DateTime?>();
                    this.intructionTimeSelected.AddRange(input.Dates);
                    //}
                    this.intructionTimeSelecteds = this.intructionTimeSelected.Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o.Value.ToString("yyyyMMdd") + timeSelested.ToString("HHmm") + "00")).OrderByDescending(o => o).ToList();
                    this.InstructionTime = intructionTimeSelecteds.First();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessDataWithTreatmentWithPatientTypeInfo()
        {
            try
            {
                if (this.currentPatientTypeAllows != null && this.currentPatientTypes != null)
                {
                    if (this.currentHisTreatment != null && !String.IsNullOrEmpty(this.currentHisTreatment.PATIENT_TYPE_CODE))
                    {
                        var patientType = this.currentPatientTypes.FirstOrDefault(o => o.PATIENT_TYPE_CODE == this.currentHisTreatment.PATIENT_TYPE_CODE);
                        if (patientType == null) throw new AggregateException("Khong lay duoc thong tin PatientType theo ma doi tuong (PATIENT_TYPE trong HisTreatmentWithPatientTypeInfoSDO).");

                        this.currentHisPatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                        this.currentHisPatientTypeAlter.PATIENT_TYPE_ID = patientType.ID;
                        this.currentHisPatientTypeAlter.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                        this.currentHisPatientTypeAlter.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                        this.currentHisPatientTypeAlter.TREATMENT_TYPE_CODE = this.currentHisTreatment.TREATMENT_TYPE_CODE;
                        this.currentHisPatientTypeAlter.HEIN_MEDI_ORG_CODE = this.currentHisTreatment.HEIN_MEDI_ORG_CODE;
                        this.currentHisPatientTypeAlter.HEIN_CARD_FROM_TIME = this.currentHisTreatment.HEIN_CARD_FROM_TIME;
                        this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME = this.currentHisTreatment.HEIN_CARD_TO_TIME;
                        this.currentHisPatientTypeAlter.HEIN_CARD_NUMBER = this.currentHisTreatment.HEIN_CARD_NUMBER;
                        this.currentHisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE = this.currentHisTreatment.RIGHT_ROUTE_TYPE_CODE;
                        this.currentHisPatientTypeAlter.LEVEL_CODE = this.currentHisTreatment.LEVEL_CODE;
                        this.currentHisPatientTypeAlter.RIGHT_ROUTE_CODE = this.currentHisTreatment.RIGHT_ROUTE_CODE;
                        var tt = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.TREATMENT_TYPE_CODE == this.currentHisTreatment.TREATMENT_TYPE_CODE);
                        this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID = (tt != null ? tt.ID : 0);
                        this.currentHisPatientTypeAlter.TREATMENT_TYPE_NAME = (tt != null ? tt.TREATMENT_TYPE_NAME : "");

                        var patientTypeAllow = this.currentPatientTypeAllows.Where(o => o.PATIENT_TYPE_ID == patientType.ID).Select(m => m.PATIENT_TYPE_ALLOW_ID).Distinct().ToList();

                        this.currentPatientTypeWithPatientTypeAlter = ((patientTypeAllow != null && patientTypeAllow.Count > 0) ? currentPatientTypes.Where(o => patientTypeAllow.Contains(o.ID)).OrderBy(o => o.PRIORITY).ToList() : new List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>());
                        if (HisConfigCFG.IsAssignRoomByPatientType && currentPatientTypeWithPatientTypeAlter != null && currentPatientTypeWithPatientTypeAlter.Count > 0)
                        {
                            MOS.Filter.HisPatientTypeRoomFilter _patienttypeRoomFIlter = new MOS.Filter.HisPatientTypeRoomFilter();
                            _patienttypeRoomFIlter.PATIENT_TYPE_IDs = currentPatientTypeWithPatientTypeAlter.Select(o => o.ID).ToList();
                            _patienttypeRoomFIlter.IS_ACTIVE = (short)1;
                            PatientTypeRooms = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT_TYPE_ROOM>>("api/HisPatientTypeRoom/Get", ApiConsumers.MosConsumer, _patienttypeRoomFIlter, null);
                        }
                    }
                    else
                        throw new AggregateException("currentHisTreatment.PATIENT_TYPE_CODE is null");
                }
                else
                    throw new AggregateException("patientTypeAllows is null");
            }
            catch (AggregateException ex)
            {
                this.currentHisPatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                this.currentPatientTypeWithPatientTypeAlter = new List<HIS_PATIENT_TYPE>();
                WaitingManager.Hide();
                MessageManager.Show(ResourceMessage.KhongTimThayDoiTuongThanhToanTrongThoiGianYLenh);
                Inventec.Common.Logging.LogSystem.Warn("LoadDataToCurrentTreatmentData => khong lay duoc doi tuong benh nhan. Dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => treatmentId), treatmentId) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => intructionTimeSelecteds), intructionTimeSelecteds) + "____Dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentHisTreatment), currentHisTreatment));
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CreateThreadLoadDataForPrint()
        {
            System.Threading.Thread thread = new System.Threading.Thread(LoadDataForPrint);
            System.Threading.Thread thread2 = new System.Threading.Thread(ProcessGetDataDepartment);
            try
            {
                thread.Start();
                thread2.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                thread.Abort();
            }
        }

        private void LoadDataForPrint()
        {
            try
            {
                if (this.currentHisTreatment != null)
                {
                    MOS.Filter.HisPatientViewFilter patientViewFilter = new MOS.Filter.HisPatientViewFilter();
                    patientViewFilter.ID = this.currentHisTreatment.PATIENT_ID;
                    var patients = new BackendAdapter(null).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientViewFilter, null);
                    if (patients != null && patients.Count > 0)
                    {
                        this.patientPrint = patients.FirstOrDefault();
                    }

                    MOS.Filter.HisTreatmentFeeViewFilter filterTreatmentFee = new MOS.Filter.HisTreatmentFeeViewFilter();
                    filterTreatmentFee.ID = this.currentHisTreatment.ID;
                    var listTreatment = new BackendAdapter(null)
                      .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumer.ApiConsumers.MosConsumer, filterTreatmentFee, null);
                    if (listTreatment != null && listTreatment.Count > 0)
                    {
                        this.treatmentPrint = listTreatment.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessGetDataDepartment()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ProcessGetDataDepartment.Begin");
                CommonParam paramGet = new CommonParam();
                if (this.ListDepartmentTranCheckTime == null)
                {
                    HisDepartmentTranFilter filter = new HisDepartmentTranFilter();
                    filter.TREATMENT_ID = this.treatmentId;
                    this.ListDepartmentTranCheckTime = new BackendAdapter(paramGet).Get<List<HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                }

                if (this.ListCoTreatmentCheckTime == null)
                {
                    HisCoTreatmentFilter filter = new HisCoTreatmentFilter();
                    filter.TDL_TREATMENT_ID = this.treatmentId;
                    this.ListCoTreatmentCheckTime = new BackendAdapter(paramGet).Get<List<HIS_CO_TREATMENT>>("api/HisCoTreatment/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                }

                Inventec.Common.Logging.LogSystem.Debug("ProcessGetDataDepartment.End");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCurrentPatient()
        {
            try
            {
                if (treatmentId > 0)
                {
                    currentTreatment = GetTreatment(this.treatmentId);
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisPatientViewFilter patientViewFilter = new MOS.Filter.HisPatientViewFilter();
                    patientViewFilter.ID = currentTreatment.PATIENT_ID;
                    var patients = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumer.ApiConsumers.MosConsumer, patientViewFilter, param);
                    if (patients != null && patients.Count > 0)
                    {
                        this.currentPatient = patients.FirstOrDefault();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadTotalSereServByHeinWithTreatmentAsync(long treatmentId)
        {
            try
            {
                DateTime intructTime = (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.intructionTimeSelecteds.First()) ?? DateTime.Now);

                CommonParam param = new CommonParam();
                HisSereServFilter hisSereServFilter = new HisSereServFilter();
                hisSereServFilter.TREATMENT_ID = treatmentId;
                //hisSereServFilter.PATIENT_TYPE_ID = HisConfigCFG.PatientTypeId__BHYT;
                this.sereServsInTreatmentRaw = await new BackendAdapter(param).GetAsync<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, hisSereServFilter, param);
                this.sereServsInTreatment = this.sereServsInTreatmentRaw != null ? this.sereServsInTreatmentRaw.Where(o => o.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT).ToList() : null;

                this.totalHeinByTreatment = this.sereServsInTreatment != null && this.sereServsInTreatment.Count > 0 ? this.sereServsInTreatment.Sum(o => o.VIR_TOTAL_PRICE_NO_ADD_PRICE ?? 0) : 0;
                this.totalHeinPriceByTreatment = this.sereServsInTreatment.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);

                this.LoadDataSereServWithTreatment(this.currentHisTreatment, intructTime);
                this.LoadIcdDefault();

                if (this.totalHeinPriceByTreatment > 0)
                {
                    string messageErr = "";
                    AlertWarningFeeManager alertWarningFeeManager = new AlertWarningFeeManager();
                    if (!alertWarningFeeManager.RunOption(treatmentId, currentHisPatientTypeAlter.PATIENT_TYPE_ID, currentHisPatientTypeAlter.TREATMENT_TYPE_ID, currentHisPatientTypeAlter.HEIN_MEDI_ORG_CODE, HisConfigCFG.PatientTypeId__BHYT, totalHeinPriceByTreatment, HisConfigCFG.IsUsingWarningHeinFee, 0, ref messageErr, true))
                    {
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataSereServWithTreatment(HisTreatmentWithPatientTypeInfoSDO treatment, DateTime? intructionTime)
        {
            try
            {
                if (treatment != null)
                {
                    this.RefeshSereServInTreatmentData();
                    this.isNotHandlerWhileChangeToggetSwith = true;
                    //if (!HisConfigCFG.IsNotAutoLoadServiceOpenAssignService)
                    //{
                    //    this.LoadDataToGrid(true);//TODO
                    //}

                    this.isNotHandlerWhileChangeToggetSwith = false;
                    //this.FillDataToControlsForm();
                    //this.InitDefaultDataService();
                    //this.LoadDataToTrackingCombo();
                    this.CheckOverTotalPatientPrice();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadIcdDefault()
        {
            try
            {
                this.isNotProcessWhileChangedTextSubIcd = true;
                Inventec.Common.Logging.LogSystem.Debug("LoadIcdDefault. 1");
                if (tracking != null && !String.IsNullOrEmpty(tracking.TRADITIONAL_ICD_CODE) && HisConfigCFG.TrackingCreate__UpdateTreatmentIcd == "1")
                {
                    this.LoadIcdTranditionalToControl(tracking.TRADITIONAL_ICD_CODE, tracking.TRADITIONAL_ICD_NAME);
                    this.LoadIcdSubTranditionalToControl(tracking.TRADITIONAL_ICD_SUB_CODE, tracking.TRADITIONAL_ICD_TEXT);
                }
                else if ((HisConfigCFG.IsloadIcdFromExamServiceExecute || (currentHisTreatment != null && String.IsNullOrEmpty(currentHisTreatment.TRADITIONAL_ICD_CODE))) && this.icdExam != null)
                {
                    this.LoadIcdTranditionalToControl(icdExam.TRADITIONAL_ICD_CODE, icdExam.TRADITIONAL_ICD_NAME);
                    this.LoadIcdSubTranditionalToControl(icdExam.TRADITIONAL_ICD_SUB_CODE, icdExam.TRADITIONAL_ICD_TEXT);
                }
                else if (this.currentHisTreatment != null)
                {
                    this.LoadIcdTranditionalToControl(currentHisTreatment.TRADITIONAL_ICD_CODE, currentHisTreatment.TRADITIONAL_ICD_NAME);
                    this.LoadIcdSubTranditionalToControl(currentHisTreatment.TRADITIONAL_ICD_SUB_CODE, currentHisTreatment.TRADITIONAL_ICD_TEXT);
                }


                if (this.tracking != null && !String.IsNullOrEmpty(this.tracking.ICD_CODE) && HisConfigCFG.TrackingCreate__UpdateTreatmentIcd == "1")
                {
                    this.LoadIcdToControl(this.tracking.ICD_CODE, this.tracking.ICD_NAME);

                    if ((HisConfigCFG.IsloadIcdFromExamServiceExecute || (currentHisTreatment != null && String.IsNullOrEmpty(currentHisTreatment.ICD_CODE))) && this.icdExam != null && !String.IsNullOrEmpty(this.icdExam.ICD_CODE))
                    {
                        this.LoadIcdCauseToControl(this.icdExam.ICD_CAUSE_CODE, this.icdExam.ICD_CAUSE_NAME);
                    }
                    else if (this.currentHisTreatment != null)
                    {
                        //Nếu hồ sơ chưa có thông tin ICD, và là hồ sơ đến khám theo loại là hẹn khám thì khi chỉ định dịch vụ, tự động hiển thị ICD của đợt điều trị trước, tương ứng với mã hẹn khám
                        if (string.IsNullOrEmpty(this.currentHisTreatment.ICD_CODE)
                            && !String.IsNullOrEmpty(this.currentHisTreatment.PREVIOUS_ICD_CODE))
                        {

                        }
                        else
                        {
                            LoadIcdCauseToControl(currentHisTreatment.ICD_CAUSE_CODE, this.currentHisTreatment.ICD_CAUSE_NAME);
                        }
                    }

                    icdMain = this.currentIcds.FirstOrDefault(o => o.ICD_CODE == this.tracking.ICD_CODE);
                    if (icdMain != null)
                    {
                        LoadRequiredCause((icdMain.IS_REQUIRE_CAUSE == 1));
                    }

                    this.LoadDataToIcdSub(this.tracking.ICD_SUB_CODE, this.tracking.ICD_TEXT);

                    Inventec.Common.Logging.LogSystem.Debug("LoadIcdDefault. 2");
                }
                else if ((HisConfigCFG.IsloadIcdFromExamServiceExecute || (currentHisTreatment != null && String.IsNullOrEmpty(currentHisTreatment.ICD_CODE))) && this.icdExam != null && !String.IsNullOrEmpty(this.icdExam.ICD_CODE))
                {
                    this.LoadIcdToControl(this.icdExam.ICD_CODE, this.icdExam.ICD_NAME);
                    this.LoadIcdCauseToControl(this.icdExam.ICD_CAUSE_CODE, this.icdExam.ICD_CAUSE_NAME);

                    icdMain = this.currentIcds.FirstOrDefault(o => o.ICD_CODE == this.icdExam.ICD_CODE);
                    if (icdMain != null)
                    {
                        LoadRequiredCause((icdMain.IS_REQUIRE_CAUSE == 1));
                    }

                    this.LoadDataToIcdSub(this.icdExam.ICD_SUB_CODE, this.icdExam.ICD_TEXT);

                    Inventec.Common.Logging.LogSystem.Debug("LoadIcdDefault. 3");
                }
                else if (this.currentHisTreatment != null)
                {
                    //Nếu hồ sơ chưa có thông tin ICD, và là hồ sơ đến khám theo loại là hẹn khám thì khi chỉ định dịch vụ, tự động hiển thị ICD của đợt điều trị trước, tương ứng với mã hẹn khám
                    if (string.IsNullOrEmpty(this.currentHisTreatment.ICD_CODE)
                        && !String.IsNullOrEmpty(this.currentHisTreatment.PREVIOUS_ICD_CODE))
                    {
                        HIS.UC.Icd.ADO.IcdInputADO icd = new HIS.UC.Icd.ADO.IcdInputADO();
                        icd.ICD_CODE = currentHisTreatment.PREVIOUS_ICD_CODE;
                        icd.ICD_NAME = this.currentHisTreatment.PREVIOUS_ICD_NAME;
                        icdMain = this.currentIcds.FirstOrDefault(o => o.ICD_CODE == currentHisTreatment.PREVIOUS_ICD_CODE);

                        LoadIcdToControl(currentHisTreatment.PREVIOUS_ICD_CODE, this.currentHisTreatment.PREVIOUS_ICD_NAME);
                        if (icdMain != null)
                        {
                            LoadRequiredCause((icdMain.IS_REQUIRE_CAUSE == 1));
                        }

                        LoadIcdToControlIcdSub(this.currentHisTreatment.PREVIOUS_ICD_SUB_CODE, this.currentHisTreatment.PREVIOUS_ICD_TEXT);
                    }
                    else
                    {
                        HIS.UC.Icd.ADO.IcdInputADO icd = new HIS.UC.Icd.ADO.IcdInputADO();
                        icd.ICD_CODE = currentHisTreatment.ICD_CODE;
                        icd.ICD_NAME = this.currentHisTreatment.ICD_NAME;
                        icdMain = this.currentIcds.FirstOrDefault(o => o.ICD_CODE == currentHisTreatment.ICD_CODE);
                        LoadIcdToControl(currentHisTreatment.ICD_CODE, this.currentHisTreatment.ICD_NAME);
                        if (icdMain != null)
                        {
                            LoadRequiredCause((icdMain.IS_REQUIRE_CAUSE == 1));
                        }
                        LoadIcdCauseToControl(currentHisTreatment.ICD_CAUSE_CODE, this.currentHisTreatment.ICD_CAUSE_NAME);
                        LoadIcdToControlIcdSub(this.currentHisTreatment.ICD_SUB_CODE, this.currentHisTreatment.ICD_TEXT);
                    }
                    Inventec.Common.Logging.LogSystem.Debug("LoadIcdDefault. 4");
                }

                if (icdMain != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("LoadIcdDefault. 5");
                    DelegateSelectedIcd(icdMain);
                }

                //string[] codes = this.txtIcdSubCode.Text.Split(IcdUtil.seperator.ToCharArray());
                //this.icdSubcodeAdoChecks = (from m in this.currentIcds.Where(o => o.IS_TRADITIONAL != 1).ToList() select new ADO.IcdADO(m, codes)).ToList();

                //customGridViewSubIcdName.BeginUpdate();
                //customGridViewSubIcdName.GridControl.DataSource = this.icdSubcodeAdoChecks;
                //customGridViewSubIcdName.EndUpdate();
                this.isNotProcessWhileChangedTextSubIcd = false;
                Inventec.Common.Logging.LogSystem.Debug("LoadIcdDefault. 6");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadIcdTranditionalToControl(string icdCode, string icdName)
        {
            try
            {
                if (icdYhctProcessor != null)
                {
                    UC.Icd.ADO.IcdInputADO icdYhct = new UC.Icd.ADO.IcdInputADO();
                    icdYhct.ICD_CODE = icdCode;
                    icdYhct.ICD_NAME = icdName;
                    if (ucIcdYhct != null)
                    {
                        this.icdYhctProcessor.Reload(ucIcdYhct, icdYhct);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadIcdSubTranditionalToControl(string icdCode, string icdName)
        {
            try
            {
                SecondaryIcdDataADO subYhctIcd = new SecondaryIcdDataADO();
                subYhctIcd.ICD_SUB_CODE = icdCode;
                subYhctIcd.ICD_TEXT = icdName;
                if (ucSecondaryIcdYhct != null)
                {
                    subIcdYhctProcessor.Reload(ucSecondaryIcdYhct, subYhctIcd);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadIcdCauseToControl(string icdCode, string icdName)
        {
            try
            {
                if (!string.IsNullOrEmpty(icdCode))
                {
                    var icd = this.currentIcds.Where(p => p.ICD_CODE == (icdCode)).FirstOrDefault();
                    if (icd != null)
                    {
                        txtIcdCodeCause.Text = icd.ICD_CODE;
                        cboIcdsCause.EditValue = icd.ID;
                        if ((isAutoCheckIcd) || (!String.IsNullOrEmpty(icdName) && (icdName ?? "").Trim().ToLower() != (icd.ICD_NAME ?? "").Trim().ToLower()))
                        {
                            chkEditIcdCause.Checked = (HisConfigCFG.AutoCheckIcd != "2");
                            txtIcdMainTextCause.Text = icdName;
                        }
                        else
                        {
                            chkEditIcdCause.Checked = false;
                            txtIcdMainTextCause.Text = icd.ICD_NAME;
                        }
                    }
                    else
                    {
                        txtIcdCodeCause.Text = null;
                        cboIcdsCause.EditValue = null;
                        txtIcdMainTextCause.Text = null;
                        chkEditIcdCause.Checked = false;
                    }
                }
                else if (!string.IsNullOrEmpty(icdName))
                {
                    chkEditIcdCause.Checked = (HisConfigCFG.AutoCheckIcd != "2");
                    txtIcdMainTextCause.Text = icdName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadRequiredCause(bool isRequired)
        {
            try
            {
                ValidationICDCause(10, 500, isRequired);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationICDCause(int? maxLengthCode, int? maxLengthText, bool isRequired)
        {
            try
            {
                if (isRequired)
                {
                    lciIcdTextCause.AppearanceItemCaption.ForeColor = Color.Maroon;

                    IcdValidationRuleControl icdMainRule = new IcdValidationRuleControl();
                    icdMainRule.txtIcdCode = txtIcdCodeCause;
                    icdMainRule.btnBenhChinh = cboIcdsCause;
                    icdMainRule.txtMainText = txtIcdMainTextCause;
                    icdMainRule.chkCheck = chkEditIcdCause;
                    icdMainRule.maxLengthCode = maxLengthCode;
                    icdMainRule.maxLengthText = maxLengthText;
                    icdMainRule.IsObligatoryTranferMediOrg = this.IsObligatoryTranferMediOrg;
                    icdMainRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    icdMainRule.ErrorType = ErrorType.Warning;
                    dxValidationProviderControl.SetValidationRule(txtIcdCodeCause, icdMainRule);
                }
                else
                {
                    lciIcdTextCause.AppearanceItemCaption.ForeColor = new System.Drawing.Color();
                    txtIcdCodeCause.ErrorText = "";
                    dxValidationProviderControl.RemoveControlError(txtIcdCodeCause);
                    dxValidationProviderControl.SetValidationRule(txtIcdCodeCause, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadIcdToControlIcdSub(string icdSubCode, string icdText)
        {
            try
            {
                this.txtIcdSubCode.Text = icdSubCode;
                this.txtIcdText.Text = icdText;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void DelegateSelectedIcd(HIS_ICD icdSelected)
        {
            try
            {
                //Bổ sung key cấu hình: 
                //"0 (hoặc ko khai báo): Không kiểm tra 
                //+ 1: Có kiểm tra dịch vụ đã yêu cầu có nằm trong danh sách đã được cấu hình tương ứng với ICD của bệnh nhân hay không. Nếu tồn tại dịch vụ không được cấu hình thì hiển thị thông báo và không cho lưu.
                //+ 2: Có kiểm tra, nhưng chỉ hiển thị cảnh báo, và hỏi "Bạn có muốn tiếp tục không". Nếu người dùng chọn "OK" thì vẫn cho phép lưu"


                if (HisConfigCFG.IcdServiceHasCheck != "1" && HisConfigCFG.IcdServiceHasCheck != "2" && HisConfigCFG.IcdServiceHasCheck != "3" && HisConfigCFG.IcdServiceHasCheck != "4" && HisConfigCFG.IcdServiceHasCheck != "5")
                    return;

                List<HIS_ICD> icdFromUc = new List<HIS_ICD>();
                if (icdSelected != null)
                {
                    icdFromUc.Add(icdSelected);
                }


                var subIcd = UcSecondaryIcdGetValue() as HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO;
                if (subIcd != null)
                {
                    string icd_sub_code = subIcd.ICD_SUB_CODE;
                    if (!string.IsNullOrEmpty(icd_sub_code))
                    {
                        String[] icdCodes = icd_sub_code.Split(';');
                        foreach (var item in icdCodes)
                        {
                            var icd = this.currentIcds.Where(o => o.IS_TRADITIONAL != 1 && o.ICD_CODE == item).FirstOrDefault();
                            if (icd != null && (icdSelected == null || (icdSelected != null && icd.ICD_CODE != icdSelected.ICD_CODE)))
                            {
                                HIS_ICD icdSub = new HIS_ICD();
                                icdSub.ID = icd != null ? icd.ID : 0;
                                icdSub.ICD_NAME = icd != null ? icd.ICD_NAME : "";
                                icdSub.ICD_CODE = icd != null ? icd.ICD_CODE : "";
                                icdFromUc.Add(icdSub);
                            }
                        }
                    }
                }

                if (icdFromUc != null && icdFromUc.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisIcdServiceFilter filter = new HisIcdServiceFilter();
                    filter.ICD_CODE__EXACTs = icdFromUc.Select(o => o.ICD_CODE).Distinct().ToList();
                    this.icdServicePhacDos = new BackendAdapter(param).Get<List<HIS_ICD_SERVICE>>("api/HisIcdService/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                }
                else
                {
                    this.icdServicePhacDos = null;
                }

                if (this.icdServicePhacDos != null && this.icdServicePhacDos.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("DelegateSelectedIcd. 1. this.icdServicePhacDos.count=" + this.icdServicePhacDos.Count);
                    //if ((bool)this.toggleSwitchDataChecked.EditValue == false)
                    //{
                    //    this.toggleSwitchDataChecked.EditValue = true;
                    //}
                    ProcessChoiceIcdPhacDo(this.icdServicePhacDos);
                }
                else
                {
                    //if ((bool)this.toggleSwitchDataChecked.EditValue == true)
                    //{
                    //    this.toggleSwitchDataChecked.EditValue = false;
                    //}
                    this.ResetDefaultGridData();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        internal object UcSecondaryIcdGetValue()
        {
            object result = null;
            try
            {
                SecondaryIcdDataADO outPut = new SecondaryIcdDataADO();

                if (!String.IsNullOrEmpty(txtIcdSubCode.Text))
                {
                    outPut.ICD_SUB_CODE = txtIcdSubCode.Text;
                }
                if (!String.IsNullOrEmpty(txtIcdText.Text))
                {
                    outPut.ICD_TEXT = txtIcdText.Text;
                }
                result = outPut;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void ProcessChoiceIcdPhacDo(List<HIS_ICD_SERVICE> serviceIcds)
        {
            try
            {
                lstSereServExist = new List<HIS_SERE_SERV>();
                ServicePDDTIds = new List<long>();
                var allDatas = this.DataGridAdo.AsQueryable();
                var serviceChecked = allDatas.Where(o => o.IsChecked).ToList();
                var icdServiceIds = serviceIcds.Where(o => o.IS_CONTRAINDICATION == 1 || o.IS_WARNING == 1).Select(o => o.SERVICE_ID).ToList();
                serviceIcds = serviceIcds.Where(o => !icdServiceIds.Exists(p => p == o.SERVICE_ID)).ToList();
                if (serviceIcds != null && serviceIcds.Count > 0)
                {
                    List<DataGridAdo> SereServICD = null;
                    List<HIS_SERE_SERV> sereServMinDurations = null;
                    var serviceIds = serviceIcds.Select(o => o.SERVICE_ID).Distinct().ToArray();
                    SereServICD = allDatas.Where(o => serviceIds.Contains(o.SERVICE_ID)).ToList();
                    if (SereServICD != null && SereServICD.Count > 0)
                    {
                        #region GET MIN_DURATION
                        var icdServiceMinDuration = serviceIcds.Where(p => p.MIN_DURATION > 0).ToList();
                        var SereServICDMinDuration = allDatas.Where(o => icdServiceMinDuration.Select(p => p.SERVICE_ID).Distinct().ToArray().Contains(o.SERVICE_ID)).ToList();
                        if (SereServICDMinDuration != null && SereServICDMinDuration.Count > 0)
                        {
                            sereServMinDurations = getSereServWithMinDuration(SereServICDMinDuration, this.currentTreatment.PATIENT_ID, icdServiceMinDuration);
                        }
                        #endregion


                        List<DataGridAdo> lstSereServResult = new List<DataGridAdo>();
                        if (sereServMinDurations != null && sereServMinDurations.Count > 0)
                        {
                            var serviceIcdIds = SereServICD.Select(o => o.SERVICE_ID).Distinct().ToArray();
                            var serviceMinDurationIds = sereServMinDurations.Select(p => p.SERVICE_ID).ToArray();
                            var svNotExist = serviceIcdIds.Where(o => !serviceMinDurationIds.ToList().Exists(p => p == o)).ToList();
                            if (svNotExist != null && svNotExist.Count > 0)
                                lstSereServResult = allDatas.Where(o => svNotExist.Contains(o.SERVICE_ID)).ToList();


                            var svExist = serviceIcdIds.Where(o => serviceMinDurationIds.ToList().Exists(p => p == o)).ToList();
                            if (svExist != null && svExist.Count > 0)
                            {
                                lstSereServExist = sereServMinDurations.Where(o => svExist.Contains(o.SERVICE_ID)).ToList();
                            }

                        }
                        else
                        {
                            lstSereServResult = SereServICD;
                        }

                        if (lstSereServResult != null && lstSereServResult.Count > 0)
                        {
                            //foreach (var sereServADO in lstSereServResult)
                            //{
                            //    var ssData = this.ServiceIsleafADOs.Where(o => o.SERVICE_ID == sereServADO.SERVICE_ID).FirstOrDefault();
                            //    if (ssData != null)
                            //    {
                            //        if (!chkAutoCheckPDDT.Checked)
                            //        {
                            //            if (!serviceChecked.Exists(o => o.SERVICE_ID == ssData.SERVICE_ID))
                            //            {
                            //                this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, ssData.SERVICE_ID, ssData);
                            //                this.FillDataOtherPaySourceDataRow(ssData);
                            //                this.ValidServiceDetailProcessing(ssData);
                            //            }
                            //            ssData.IsChecked = true;
                            //            ServicePDDTIds.Add(ssData.SERVICE_ID);
                            //        }
                            //        else
                            //        {
                            //            ssData.IsChecked = false;
                            //        }
                            //    }
                            //}
                            foreach (var item in ServiceIsleafADOs)
                            {
                                if (!ServicePDDTIds.Exists(o => o == item.SERVICE_ID))
                                    item.IsChecked = false;
                            }
                        }
                        else
                        {
                            this.ResetDefaultGridData();
                        }

                        this.gridControlServiceProcess.DataSource = null;
                        List<DataGridAdo> gData = new List<DataGridAdo>();
                        //if (chkAutoCheckPDDT.Checked)
                        //{
                        //    this.ResetDefaultGridData();
                        //    gData = this.ServiceIsleafADOs.OrderBy(o => o.SERVICE_TYPE_ID).ThenByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.TDL_SERVICE_NAME).ToList();
                        //}
                        //else
                        //{
                        this.toggleSwitchDataChecked.EditValue = true;
                        gData = this.DataGridAdo.Where(o => o.IsChecked).OrderBy(o => o.SERVICE_TYPE_ID).ThenByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.TDL_SERVICE_NAME).ToList();
                        //}
                        this.gridControlServiceProcess.DataSource = gData;
                        this.SetEnableButtonControl(this.actionType);
                        VerifyWarningOverCeiling();
                        this.SetDefaultSerServTotalPrice();
                    }
                    else
                    {
                        this.ResetDefaultGridData();
                    }
                }
                else
                {
                    this.ResetDefaultGridData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<HIS_SERE_SERV> getSereServWithMinDuration(List<DataGridAdo> serviceCheckeds, long patientId, List<HIS_ICD_SERVICE> icdServiceDuration = null)
        {
            List<HIS_SERE_SERV> listSereServResult = null;
            try
            {
                if (HisConfigCFG.IsSereServMinDurationAlert == 1 || HisConfigCFG.IsSereServMinDurationAlert == 2)
                {
                    if (serviceCheckeds != null && serviceCheckeds.Count > 0)
                    {
                        var bhytServices = serviceCheckeds.Where(o => o.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT).ToList();
                        List<DataGridAdo> sereServADOExistMinDUration = bhytServices.Where(o => o.MIN_DURATION != null).ToList();
                        if (icdServiceDuration != null && icdServiceDuration.Count > 0)
                            sereServADOExistMinDUration = bhytServices;
                        if (sereServADOExistMinDUration != null && sereServADOExistMinDUration.Count > 0)
                        {
                            List<ServiceDuration> serviceDurations = new List<ServiceDuration>();
                            foreach (var item in sereServADOExistMinDUration)
                            {
                                ServiceDuration serviceDuration = new ServiceDuration();
                                serviceDuration.ServiceId = item.SERVICE_ID;
                                if (icdServiceDuration != null && icdServiceDuration.Count > 0)
                                    serviceDuration.MinDuration = icdServiceDuration.Where(o => o.SERVICE_ID == item.SERVICE_ID).Min(o => o.MIN_DURATION ?? 0);
                                else
                                    serviceDuration.MinDuration = (item.MIN_DURATION ?? 0);
                                serviceDurations.Add(serviceDuration);
                            }
                            CommonParam param = new CommonParam();
                            HisSereServMinDurationFilter filter = new HisSereServMinDurationFilter
                            {
                                ServiceDurations = serviceDurations,
                                InstructionTime = this.isMultiDateState ? intructionTimeSelecteds.First() : intructionTimeSelecteds.First(),
                                PatientId = patientId
                            };
                            var result = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/GetExceedMinDuration", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                            if (result != null && result.Any())
                            {
                                listSereServResult = result.GroupBy(o => o.SERVICE_ID).Select(g => g.OrderByDescending(x => x.TDL_INTRUCTION_TIME).First()).ToList();
                            }
                        }
                        else
                        {
                            listSereServResult = null;
                        }
                    }
                    else
                    {
                        listSereServResult = null;
                    }
                }
                if (HisConfigCFG.IsSereServMinDurationAlert == 0 || (HisConfigCFG.IsSereServMinDurationAlert != 1 && HisConfigCFG.IsSereServMinDurationAlert != 2))
                {
                    var allServices = serviceCheckeds.Where(o => o.MIN_DURATION != null).ToList();
                    if (allServices.Count > 0)
                    {
                        List<ServiceDuration> serviceDurations = new List<ServiceDuration>();
                        foreach (var item in allServices)
                        {
                            ServiceDuration serviceDuration = new ServiceDuration();
                            serviceDuration.ServiceId = item.SERVICE_ID;
                            if (icdServiceDuration != null && icdServiceDuration.Count > 0)
                                serviceDuration.MinDuration = icdServiceDuration.Where(o => o.SERVICE_ID == item.SERVICE_ID).Min(o => o.MIN_DURATION ?? 0);
                            else
                                serviceDuration.MinDuration = (item.MIN_DURATION ?? 0);
                            serviceDurations.Add(serviceDuration);
                        }
                        CommonParam param = new CommonParam();
                        HisSereServMinDurationFilter filter = new HisSereServMinDurationFilter
                        {
                            ServiceDurations = serviceDurations,
                            InstructionTime = this.isMultiDateState ? intructionTimeSelecteds.First() : intructionTimeSelecteds.First(),
                            PatientId = patientId
                        };
                        var result = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/GetExceedMinDuration", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                        if (result != null && result.Any())
                        {
                            listSereServResult = result.GroupBy(o => o.SERVICE_ID).Select(g => g.OrderByDescending(x => x.TDL_INTRUCTION_TIME).First()).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                listSereServResult = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return listSereServResult;
        }

        private void VerifyWarningOverCeiling()
        {

            try
            {
                decimal totalPriceSum = totalHeinByTreatment + GetTotalPriceServiceSelected(HisConfigCFG.PatientTypeId__BHYT);

                decimal warningOverCeiling = (this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM ? HisConfigCFG.WarningOverCeiling__Exam : (this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU ? HisConfigCFG.WarningOverCeiling__Out : HisConfigCFG.WarningOverCeiling__In));

                bool inValid = (warningOverCeiling > 0 && totalPriceSum > warningOverCeiling);
                if (inValid)
                {
                    MessageManager.Show(String.Format(ResourceMessage.TongTienTheoDoiTuongDieuTriChoBHYTDaVuotquaMucGioiHan, GetTreatmentTypeNameByCode(this.currentHisPatientTypeAlter.TREATMENT_TYPE_CODE), Inventec.Common.Number.Convert.NumberToString(totalPriceSum, 0), Inventec.Common.Number.Convert.NumberToString(warningOverCeiling, 0)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private decimal GetTotalPriceServiceSelected(long patientTypeId)
        {
            decimal totalPrice = 0;
            try
            {
                List<DataGridAdo> serviceCheckeds__Send = this.DataGridAdo.FindAll(o => o.IsChecked);
                foreach (var item in serviceCheckeds__Send)
                {
                    if (item.IsChecked
                        && ((patientTypeId > 0 && item.PATIENT_TYPE_ID == patientTypeId) || (patientTypeId <= 0 && item.PATIENT_TYPE_ID > 0))
                        && (item.IsExpend ?? false) == false)
                    {
                        if (BranchDataWorker.DicServicePatyInBranch.ContainsKey(item.SERVICE_ID))
                        {
                            var data_ServicePrice = BranchDataWorker.ServicePatyWithPatientType(item.SERVICE_ID, item.PATIENT_TYPE_ID).OrderByDescending(m => m.PRIORITY).ThenByDescending(m => m.ID).ToList();
                            if (data_ServicePrice != null && data_ServicePrice.Count > 0)
                            {
                                totalPrice += item.AMOUNT * (data_ServicePrice[0].PRICE * (1 + data_ServicePrice[0].VAT_RATIO));
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return totalPrice;
        }

        private string GetTreatmentTypeNameByCode(string code)
        {
            string name = "";
            try
            {
                name = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.TREATMENT_TYPE_CODE == code).TREATMENT_TYPE_NAME;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return name;
        }

        private void ResetDefaultGridData()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ResetDefaultGridData. 1");
                this.gridViewServiceProcess.ActiveFilter.Clear();
                this.gridViewServiceProcess.ClearColumnsFilter();
                this.gridControlServiceProcess.DataSource = null;
                Inventec.Common.Logging.LogSystem.Debug("ResetDefaultGridData. 2");
                foreach (var item in this.DataGridAdo)
                {
                    item.AMOUNT = 1;
                    item.IsChecked = false;
                    item.ShareCount = null;
                    item.PATIENT_TYPE_ID = 0;
                    item.PATIENT_TYPE_CODE = "";
                    item.PATIENT_TYPE_NAME = "";
                    item.PRICE = 0;
                    item.TDL_EXECUTE_ROOM_ID = 0;
                    item.IsExpend = false;
                    item.IsOutKtcFee = false;
                    item.IsKHBHYT = false;
                    item.InstructionNote = "";
                    item.SERVICE_GROUP_ID_SELECTEDs = null;
                    item.IsNoDifference = false;
                    item.ErrorMessageAmount = "";
                    item.ErrorMessageIsAssignDay = "";
                    item.ErrorMessagePatientTypeId = "";
                    item.AssignPackagePriceEdit = null;
                    item.AssignSurgPriceEdit = null;
                    item.ErrorTypeAmount = ErrorType.None;
                    item.ErrorTypeIsAssignDay = ErrorType.None;
                    item.ErrorTypePatientTypeId = ErrorType.None;
                    item.PRIMARY_PATIENT_TYPE_ID = null;
                    item.IsNotChangePrimaryPaty = false;
                    item.BedFinishTime = null;
                    item.BedId = null;
                    item.BedStartTime = null;
                    item.SereServEkipADO = null;
                    item.NumberOfTimes = 1;
                }

                var allDatas = this.DataGridAdo != null && this.DataGridAdo.Count > 0 ? this.DataGridAdo.AsQueryable() : null;
                this.gridControlServiceProcess.DataSource = allDatas.ToList();
                this.toggleSwitchDataChecked.EditValue = false;
                //isCheckAssignServiceSimultaneityOption = false;
                this.SetEnableButtonControl(this.actionType);
                Inventec.Common.Logging.LogSystem.Debug("ResetDefaultGridData. 3");
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SetEnableButtonControl(int actionType)
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionAdd)
                {
                    List<DataGridAdo> serviceCheckeds__Send = this.DataGridAdo.FindAll(o => o.IsChecked);
                    this.btnSave.Enabled = this.btnSaveAndPrint.Enabled = isCheckAssignServiceSimultaneityOption ? false : (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0);
                    this.btnCreateBill.Enabled = this.btnDepositService.Enabled = false;

                }
                else
                {
                    this.btnSave.Enabled = this.btnSaveAndPrint.Enabled = false;
                    this.btnCreateBill.Enabled = this.btnDepositService.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessPatientSelecttWithPatientTypeInfo()
        {
            try
            {
                dicPatientType = new Dictionary<long, List<HIS_PATIENT_TYPE>>();
                var lstPatientSelect = this.patientSelectProcessor.GetSelectedRows(this.ucPatientSelect);
                var lstPatientType = lstPatientSelect.Select(o => new { o.TDL_PATIENT_TYPE_ID, o.PATIENT_TYPE_CODE }).Distinct().ToList();
                if (this.currentPatientTypeAllows != null && this.currentPatientTypes != null)
                {
                    foreach (var item in lstPatientType)
                    {
                        if (dicPatientType.ContainsKey(item.TDL_PATIENT_TYPE_ID ?? 0))
                            continue;
                        var patientType = this.currentPatientTypes.FirstOrDefault(o => o.PATIENT_TYPE_CODE == item.PATIENT_TYPE_CODE);
                        if (patientType == null) throw new AggregateException("Khong lay duoc thong tin PatientType theo ma doi tuong (PATIENT_TYPE trong HisTreatmentWithPatientTypeInfoSDO).");
                        var patientTypeAllow = this.currentPatientTypeAllows.Where(o => o.PATIENT_TYPE_ID == patientType.ID).Select(m => m.PATIENT_TYPE_ALLOW_ID).Distinct().ToList();
                        var dt = ((patientTypeAllow != null && patientTypeAllow.Count > 0) ? currentPatientTypes.Where(o => patientTypeAllow.Contains(o.ID)).OrderBy(o => o.PRIORITY).ToList() : new List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>());
                        dicPatientType[item.TDL_PATIENT_TYPE_ID ?? 0] = dt;
                    }
                }
                else
                    throw new AggregateException("patientTypeAllows is null");
            }
            catch (AggregateException ex)
            {
                WaitingManager.Hide();
                MessageManager.Show(ResourceMessage.KhongTimThayDoiTuongThanhToanTrongThoiGianYLenh);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadServicePaty(bool IsPatientSelect = false)
        {
            try
            {
                long[] serviceTypeIdAllows = null;

                serviceTypeIdAllows = new long[12]{IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
                                                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G,
                                                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH,
                                                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC,
                                                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
                                                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN,
                                                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT,
                                                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
                                                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN,
                                                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT,
                                                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN,
                                                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL
                };



                var patientTypeAll = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<long> patientTypeIds = new List<long>();
                if (IsPatientSelect)
                {
                    foreach (var item in this.dicPatientType.Values.Select(o => o.Select(p => p.ID).ToList()).ToList())
                    {
                        patientTypeIds.AddRange(item);

                    }
                    patientTypeIds = patientTypeIds.Distinct().ToList();
                }
                else
                {
                    patientTypeIds = this.currentPatientTypeWithPatientTypeAlter.Select(o => o.ID).ToList();
                }

                long intructionTime = this.intructionTimeSelecteds.FirstOrDefault();
                long treatmentTime = this.currentHisTreatment.IN_TIME;

                //Lọc các đối tượng thanh toán không có chính sách giá
                var sety = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>()
                                        .Where(t => (patientTypeIds.Contains(t.PATIENT_TYPE_ID) || BranchDataWorker.CheckPatientTypeInherit(t.INHERIT_PATIENT_TYPE_IDS, patientTypeIds.ToList()))
                                            && t.IS_ACTIVE == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CommonNumberTrue
                                            && t.BRANCH_ID == BranchDataWorker.GetCurrentBranchId()
                                            && serviceTypeIdAllows.Contains(t.SERVICE_TYPE_ID)
                                            && ((!t.TREATMENT_TO_TIME.HasValue || t.TREATMENT_TO_TIME.Value >= treatmentTime) || (!t.TO_TIME.HasValue || t.TO_TIME.Value >= intructionTime))).ToList();

                this.patientTypeIdAls = sety != null ? sety.Select(o => o.PATIENT_TYPE_ID).Distinct().ToList() : null;//TODO
                var patientTypeIdPlusAfterFilter = patientTypeAll.Where(k => k.BASE_PATIENT_TYPE_ID != null && this.patientTypeIdAls.Contains(k.BASE_PATIENT_TYPE_ID.Value)).ToList();
                if (patientTypeIdPlusAfterFilter != null && patientTypeIdPlusAfterFilter.Count > 0)
                {
                    patientTypeIdAls.AddRange(patientTypeIdPlusAfterFilter.Select(o => o.ID));
                }
                if (patientTypeIdAls != null)
                    patientTypeIdAls = patientTypeIdAls.Distinct().ToList();

                this.servicePatyInBranchs = sety
                            .GroupBy(o => o.SERVICE_ID)
                            .ToDictionary(o => o.Key, o => o.ToList());

                //Inventec.Common.Logging.LogSystem.Debug("LoadServicePaty____1:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentPatientTypeWithPatientTypeAlter), currentPatientTypeWithPatientTypeAlter)
                //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientTypeIdAls), patientTypeIdAls));

                //this.currentPatientTypeWithPatientTypeAlter = patientTypeAll.Where(o => this.patientTypeIdAls.Contains(o.ID)).OrderBy(o => o.PRIORITY).ToList();

                //Inventec.Common.Logging.LogSystem.Debug("LoadServicePaty____2:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentPatientTypeWithPatientTypeAlter), currentPatientTypeWithPatientTypeAlter)
                //    );

                this.dicServices = lstService
                    .ToDictionary(o => o.ID);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadTreatmentInfo__PatientType()
        {
            try
            {
                //decimal totalPrice = 0;
                //if (this.dSereServ1WithTreatment != null && this.dSereServ1WithTreatment.Count > 0)
                //{
                //    totalPrice = this.dSereServ1WithTreatment.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);              
                //}
                string patientInfo = "";
                patientInfo += this.currentHisTreatment.TDL_PATIENT_NAME;
                if (this.patientDob > 0)
                    patientInfo += "    -    " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentHisTreatment.TDL_PATIENT_DOB) + " (" + MPS.AgeUtil.CalculateFullAge(currentHisTreatment.TDL_PATIENT_DOB) + ") ";
                patientInfo += "    -    " + this.currentHisTreatment.TDL_PATIENT_GENDER_NAME;

                if (this.currentHisPatientTypeAlter != null)
                {
                    patientInfo += "    -    " + this.currentHisPatientTypeAlter.PATIENT_TYPE_NAME;
                    patientInfo += "    -    " + this.currentHisPatientTypeAlter.TREATMENT_TYPE_NAME;
                }
                this.lblPatientName.Text = patientInfo;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataDhst()
        {
            try
            {
                if (this.currentDhst == null)
                {
                    CommonParam param = new CommonParam();
                    HisDhstFilter dhstFilter = new HisDhstFilter();
                    dhstFilter.TREATMENT_ID = this.treatmentId;
                    dhstFilter.ORDER_FIELD = "EXECUTE_TIME";
                    dhstFilter.ORDER_DIRECTION = "DESC";
                    currentDhst = new HIS_DHST();
                    var listDHST = await new BackendAdapter(param)
                                    .GetAsync<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDHST/Get", ApiConsumers.MosConsumer, dhstFilter, param);
                    currentDhst = listDHST != null ? listDHST.FirstOrDefault() : null;
                }

                lblWeight.Text = currentDhst != null && currentDhst.WEIGHT.HasValue ? currentDhst.WEIGHT + "" : "";
                lblHeight.Text = currentDhst != null && currentDhst.HEIGHT.HasValue ? currentDhst.HEIGHT + "" : "";
                lblBMI.Text = currentDhst != null ? FillDataToBmiArea(currentDhst) : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string FillDataToBmiArea(HIS_DHST currentDhst)
        {
            string rs = "";
            try
            {
                if (currentDhst != null && currentDhst.WEIGHT.HasValue && currentDhst.HEIGHT.HasValue)
                {
                    string s = "", bmiDisplay = "";
                    decimal bmi = 0;
                    if (currentDhst.WEIGHT != null && currentDhst.HEIGHT != 0)
                    {
                        bmi = (currentDhst.WEIGHT.Value) / ((currentDhst.HEIGHT.Value / 100) * (currentDhst.HEIGHT.Value / 100));
                    }
                    //double leatherArea = 0.007184 * Math.Pow((double)currentDhst.HEIGHT.Value, 0.725) * Math.Pow((double)currentDhst.WEIGHT.Value, 0.425);
                    s = Math.Round(bmi, 2) + "";
                    //lblLeatherArea.Text = Math.Round(leatherArea, 2) + "";
                    if (bmi < 16)
                    {
                        bmiDisplay = Inventec.Common.Resource.Get.Value("UCDHST.SKINNY.III", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    }
                    else if (16 <= bmi && bmi < 17)
                    {
                        bmiDisplay = Inventec.Common.Resource.Get.Value("UCDHST.SKINNY.II", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    }
                    else if (17 <= bmi && bmi < (decimal)18.5)
                    {
                        bmiDisplay = Inventec.Common.Resource.Get.Value("UCDHST.SKINNY.I", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    }
                    else if ((decimal)18.5 <= bmi && bmi < 25)
                    {
                        bmiDisplay = Inventec.Common.Resource.Get.Value("UCDHST.BMIDISPLAY.NORMAL", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    }
                    else if (25 <= bmi && bmi < 30)
                    {
                        bmiDisplay = Inventec.Common.Resource.Get.Value("UCDHST.BMIDISPLAY.OVERWEIGHT", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    }
                    else if (30 <= bmi && bmi < 35)
                    {
                        bmiDisplay = Inventec.Common.Resource.Get.Value("UCDHST.BMIDISPLAY.OBESITY.I", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    }
                    else if (35 <= bmi && bmi < 40)
                    {
                        bmiDisplay = Inventec.Common.Resource.Get.Value("UCDHST.BMIDISPLAY.OBESITY.II", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    }
                    else if (40 < bmi)
                    {
                        bmiDisplay = Inventec.Common.Resource.Get.Value("UCDHST.BMIDISPLAY.OBESITY.III", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    }
                    rs = s + "  " + bmiDisplay;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return rs;
        }

        private void InitDefaultFocus()
        {
            try
            {
                UcIcdFocusComtrol();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UcIcdFocusComtrol()
        {
            try
            {
                txtIcdCode.Focus();
                txtIcdCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckAssignServiceSimultaneityOption()
        {
            try
            {
                isCheckAssignServiceSimultaneityOption = false;
                //if ((HisConfigCFG.ASSIGN_SERVICE_SIMULTANEITY_OPTION != "1" && HisConfigCFG.ASSIGN_SERVICE_SIMULTANEITY_OPTION != "2") || cboUser.EditValue == null || intructionTimeSelecteds == null || intructionTimeSelecteds.Count == 0)
                //    return;
                //CommonParam param = new CommonParam();
                //HisServiceReqCheckSereTimesSDO sdo = new HisServiceReqCheckSereTimesSDO();
                //sdo.TreatmentId = treatmentId;
                //sdo.Loginnames = new List<string> { cboUser.EditValue.ToString() };
                //sdo.SereTimes = intructionTimeSelecteds;
                //var CheckSereTimes = new BackendAdapter(param).Post<bool>("api/HisServiceReq/CheckSereTimes", ApiConsumers.MosConsumer, sdo, ProcessLostToken, param);
                //if (!CheckSereTimes)
                //{
                //    if (HisConfigCFG.ASSIGN_SERVICE_SIMULTANEITY_OPTION == "1")
                //    {
                //        isCheckAssignServiceSimultaneityOption = true;
                //        btnSave.Enabled = btnSaveAndPrint.Enabled = btnEdit.Enabled = false;
                //        MessageManager.Show(this, param, CheckSereTimes);
                //    }
                //    else if (HisConfigCFG.ASSIGN_SERVICE_SIMULTANEITY_OPTION == "2")
                //    {
                //        if (XtraMessageBox.Show(param.GetMessage() + " Bạn có muốn tiếp tục?", "Thông báo", MessageBoxButtons.YesNo) != DialogResult.Yes)
                //        {
                //            isCheckAssignServiceSimultaneityOption = true;
                //            btnSave.Enabled = btnSaveAndPrint.Enabled = btnEdit.Enabled = false;
                //        }
                //    }
                //}

                if ((HisConfigCFG.ASSIGN_SERVICE_SIMULTANEITY_OPTION != "1" && HisConfigCFG.ASSIGN_SERVICE_SIMULTANEITY_OPTION != "2") ||
            cboUser.EditValue == null || intructionTimeSelecteds == null || intructionTimeSelecteds.Count == 0)
                    return;


                bool hasError = false;

                if (HisConfigCFG.ASSIGN_SERVICE_SIMULTANEITY_OPTION == "1" || HisConfigCFG.ASSIGN_SERVICE_SIMULTANEITY_OPTION == "2")
                {
                    CommonParam param = new CommonParam();
                    HisServiceReqCheckSereTimesSDO sdo = new HisServiceReqCheckSereTimesSDO();
                    sdo.TreatmentId = treatmentId;
                    sdo.Loginnames = new List<string> { cboUser.EditValue.ToString() };
                    sdo.SereTimes = intructionTimeSelecteds;

                    var checkSereTimes = new BackendAdapter(param)
                        .Post<bool>("api/HisServiceReq/CheckSereTimes", ApiConsumers.MosConsumer, sdo, ProcessLostToken, param);

                    if (!checkSereTimes)
                    {
                        hasError = true;
                        if (HisConfigCFG.ASSIGN_SERVICE_SIMULTANEITY_OPTION == "1")
                        {
                            isCheckAssignServiceSimultaneityOption = true;
                            btnSave.Enabled = btnSaveAndPrint.Enabled = false;
                            MessageManager.Show(this, param, false);
                            Inventec.Common.Logging.LogSystem.Debug("API Create Result: " + Inventec.Common.Logging.LogUtil.TraceData("DataA1 key ban đầu", param));
                            return; // Dừng lại nếu option 1 bị chặn
                        }
                        else if (HisConfigCFG.ASSIGN_SERVICE_SIMULTANEITY_OPTION == "2")
                        {
                            if (XtraMessageBox.Show(param.GetMessage() + " Bạn có muốn tiếp tục?", "Thông báo", MessageBoxButtons.YesNo) != DialogResult.Yes)
                            {
                                isCheckAssignServiceSimultaneityOption = true;
                                btnSave.Enabled = btnSaveAndPrint.Enabled = false;
                                Inventec.Common.Logging.LogSystem.Debug("API Create Result: " + Inventec.Common.Logging.LogUtil.TraceData("DataA2 key ban đầu", param));
                            }
                        }
                    }
                }

                if (HisConfigCFG.ASSIGN_SIMULTANEITY_OPTION == "1" || HisConfigCFG.ASSIGN_SIMULTANEITY_OPTION == "2")
                {
                    CommonParam param2 = new CommonParam();
                    var assignSdo = new HisServiceReqCheckAssignSimultaneitySDO();
                    assignSdo.TreatmentId = treatmentId;
                    assignSdo.CheckInfos = new List<HisServiceReqCheckAssignSimultaneityCheckInfosSDO>
                                        {
                                            new HisServiceReqCheckAssignSimultaneityCheckInfosSDO
                                            {
                                                LoginName = cboUser.EditValue.ToString(),
                                                CheckTimes = intructionTimeSelecteds
                                            }
                                        };
                    Inventec.Common.Logging.LogSystem.Debug("Input api: " + Inventec.Common.Logging.LogUtil.TraceData("Data:", assignSdo));
                    var checkAssignResult = new BackendAdapter(param2)
                        .Post<bool>("api/HisServiceReq/CheckAssignSimultaneity", ApiConsumers.MosConsumer, assignSdo, ProcessLostToken, param2);
                    Inventec.Common.Logging.LogSystem.Debug("Kết quả gọi api: " + Inventec.Common.Logging.LogUtil.TraceData("Data:", checkAssignResult));
                    if (!checkAssignResult)
                    {
                        hasError = true;
                        if (HisConfigCFG.ASSIGN_SIMULTANEITY_OPTION == "1")
                        {
                            isCheckAssignServiceSimultaneityOption = true;
                            btnSave.Enabled = btnSaveAndPrint.Enabled = false;
                            XtraMessageBox.Show(param2.GetMessage(), "Thông báo");
                            Inventec.Common.Logging.LogSystem.Debug("param: " + Inventec.Common.Logging.LogUtil.TraceData("Data:", param2));
                        }
                        else if (HisConfigCFG.ASSIGN_SIMULTANEITY_OPTION == "2")
                        {
                            if (XtraMessageBox.Show(param2.GetMessage() + " Bạn có muốn tiếp tục?", "Thông báo", MessageBoxButtons.YesNo) != DialogResult.Yes)
                            {
                                isCheckAssignServiceSimultaneityOption = true;
                                btnSave.Enabled = btnSaveAndPrint.Enabled = false;
                            }
                            else
                            {
                                isCheckAssignServiceSimultaneityOption = false;
                                btnSave.Enabled = btnSaveAndPrint.Enabled = true;
                            }
                        }
                    }
                }

                if (!hasError)
                {
                    isCheckAssignServiceSimultaneityOption = false;
                    btnSave.Enabled = btnSaveAndPrint.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void RefeshSereServInTreatmentData()
        {
            try
            {
                DateTime intructionTime = (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.intructionTimeSelecteds.First()) ?? DateTime.Now);

                List<long> setyAllowsIds = new List<long>();
                setyAllowsIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU);
                setyAllowsIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC);
                setyAllowsIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT);
                long? INTRUCTION_TIME_FROM = null, INTRUCTION_TIME_TO = null;
                var existServiceByType = BackendDataWorker.Get<V_HIS_SERVICE_PATY>().Where(o => (o.INSTR_NUM_BY_TYPE_FROM.HasValue && o.INSTR_NUM_BY_TYPE_FROM.Value > 0) || (o.INSTR_NUM_BY_TYPE_TO.HasValue && o.INSTR_NUM_BY_TYPE_TO.Value > 0)).ToList();
                if (existServiceByType == null || existServiceByType.Count() == 0)
                {
                    if (intructionTime != null && intructionTime != DateTime.MinValue)
                    {
                        INTRUCTION_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(intructionTime.ToString("yyyyMMdd") + "000000");
                        INTRUCTION_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(intructionTime.ToString("yyyyMMdd") + "235959");
                    }
                    else
                    {
                        INTRUCTION_TIME_FROM = Inventec.Common.DateTime.Get.StartDay();
                        INTRUCTION_TIME_TO = Inventec.Common.DateTime.Get.EndDay();
                    }
                }

                if (this.sereServsInTreatmentRaw == null || this.sereServsInTreatmentRaw.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    HisSereServView1Filter hisSereServFilter = new HisSereServView1Filter();
                    hisSereServFilter.TREATMENT_ID = treatmentId;
                    hisSereServFilter.INTRUCTION_TIME_FROM = INTRUCTION_TIME_FROM;
                    hisSereServFilter.INTRUCTION_TIME_TO = INTRUCTION_TIME_TO;
                    hisSereServFilter.NOT_IN_SERVICE_TYPE_IDs = setyAllowsIds;
                    this.sereServWithTreatment = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>("api/HisSereServ/GetView1", ApiConsumers.MosConsumer, hisSereServFilter, ProcessLostToken, param);
                }
                else
                {
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => INTRUCTION_TIME_FROM), INTRUCTION_TIME_FROM)
                    //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => INTRUCTION_TIME_TO), INTRUCTION_TIME_TO)
                    //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServWithTreatment), sereServWithTreatment)
                    //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServsInTreatmentRaw), sereServsInTreatmentRaw));
                    this.sereServWithTreatment = this.sereServsInTreatmentRaw.Where(o =>
                        o.TDL_TREATMENT_ID == treatmentId
                        && (INTRUCTION_TIME_FROM == null || (INTRUCTION_TIME_FROM.HasValue && o.TDL_INTRUCTION_TIME >= INTRUCTION_TIME_FROM.Value))
                        && (INTRUCTION_TIME_TO == null || (INTRUCTION_TIME_TO.HasValue && o.TDL_INTRUCTION_TIME <= INTRUCTION_TIME_TO.Value))
                        && !setyAllowsIds.Contains(o.TDL_SERVICE_TYPE_ID)).ToList();
                }

                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => INTRUCTION_TIME_FROM), INTRUCTION_TIME_FROM)
                //   + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => INTRUCTION_TIME_TO), INTRUCTION_TIME_TO)
                //   + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServWithTreatment), sereServWithTreatment)
                //   + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServsInTreatmentRaw), sereServsInTreatmentRaw));
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        V_HIS_ROOM GetRequestRoom(long requestRoomId)
        {
            V_HIS_ROOM result = new V_HIS_ROOM();
            try
            {
                if (requestRoomId > 0)
                {
                    result = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == requestRoomId);
                    this.currentDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == result.DEPARTMENT_ID);
                }
            }
            catch (Exception ex)
            {
                result = new V_HIS_ROOM();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void FillAllPatientInfoSelectedInForm()
        {
            try
            {
                LogSystem.Debug("FillAllPatientInfoSelectedInForm => 1");
                if (HisConfigCFG.IsUsingServerTime == "1"
                    && this.currentHisTreatment != null)
                {
                    return;
                }
                //this.intructionTimeSelecteds = this.ucDateProcessor.GetValue(ucDate);
                //this.isMultiDateState = this.ucDateProcessor.GetChkMultiDateState(ucDate);
                this.LoadDataToCurrentTreatmentData(treatmentId, this.intructionTimeSelecteds.FirstOrDefault());
                this.SetDateUc();
                this.ProcessDataWithTreatmentWithPatientTypeInfo();
                LogSystem.Debug("FillAllPatientInfoSelectedInForm => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultData(bool isInit = false)
        {
            try
            {
                lstSereServExist = new List<HIS_SERE_SERV>();
                this.gridViewServiceProcess.ActiveFilter.Clear();
                this.gridViewServiceProcess.ClearColumnsFilter();
                //this.dicServiceReqList = new Dictionary<long, HisServiceReqListResultSDO>();
                //this.serviceReqComboResultSDO = null;
                //this.repositoryItemchkIsCheckedDisable.ReadOnly = true;
                //this.repositoryItemchkIsCheckedDisable.Enabled = false;
                //this.ButtonEdit_IsExpenDisable.ReadOnly = true;
                //this.ButtonEdit_IsExpenDisable.Enabled = false;
                //this.repositoryItemSpinAmount__Disable_TabService.ReadOnly = true;
                //this.repositoryItemSpinAmount__Disable_TabService.Enabled = false;
                //this.btnCreateServiceGroup.Enabled = false;
                this.btnSave.Enabled = false;
                this.btnSaveAndPrint.Enabled = false;
                //this.btnShowDetail.Enabled = false;
                this.btnCreateBill.Enabled = false;
                this.btnDepositService.Enabled = false;
                //this.btnPrintPhieuHuongDanBN.Enabled = false;
                //this.pnlPrintAssignService.Enabled = false;
                //this.chkPriority.Checked = false;
                //this.chkIsNotRequireFee.Checked = false;
                //this.selectedSeviceGroups = null;
                //if (this.workingServiceGroupADOs != null && this.workingServiceGroupADOs.Count > 0)
                //    this.workingServiceGroupADOs.ForEach(o => o.IsChecked = false);
                //this.beditRoom.EditValue = null;
                //this.beditRoom.Properties.Buttons[1].Visible = false;

                //this.cboPackage.EditValue = null;
                //this.txtDescription.Text = "";
                //this.cboExecuteGroup.EditValue = null;
                //this.cboExecuteGroup.Properties.Buttons[1].Visible = false;
                //this.chkExpendAll.Checked = false;
                this.lblTotalServicePrice.Text = "0";
                this.lblTotalServicePriceBhyt.Text = "0";
                this.lblTotalServicePriceOther.Text = "0";
                this.lblChenhBHYT.Text = "0";
                this.actionType = GlobalVariables.ActionAdd;
                //this.btnBoSungPhacDo.Enabled = (HisConfigCFG.IcdServiceAllowUpdate == GlobalVariables.CommonStringTrue);
                //this.chkIsInformResultBySms.CheckState = CheckState.Unchecked;
                //this.chkIsEmergency.CheckState = CheckState.Unchecked;
                //this.chkIsNotRequireFee.Enabled = false;
                //this.chkIsNotRequireFee.CheckState = CheckState.Unchecked;
                this.txtProvisionalDiagnosis.Text = this.provisionalDiagnosis;
                //this.dSignedList = new Dictionary<long, List<Inventec.Common.SignLibrary.DTO.DocumentSignedUpdateIGSysResultDTO>>();
                //this.repositoryItemSpinNumberOfTimes__Disable_TabService.ReadOnly = true;
                //this.repositoryItemSpinNumberOfTimes__Disable_TabService.Enabled = false;
                //this.txtAssignRoomCode.Text = "";
                //this.cboAssignRoom.EditValue = null;

                //this.lblChiPhiBNPhaiTra.Text = "";
                //this.lblDaDong.Text = "";
                //this.lciForlblConThua.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                //this.lblConThua.Text = "";
                //this.lciForlblConThua.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciForlblConThua.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                if (isInit || HisConfigCFG.IsUsingServerTime != "1")
                {
                    UC.DateEditor.ADO.DateInputADO dateInputADO = new UC.DateEditor.ADO.DateInputADO();
                    if (HisConfigCFG.IsShowServerTimeByDefault)
                    {
                        dateInputADO.Time = dteCommonParam;
                        dateInputADO.Dates = new List<DateTime?>();
                        dateInputADO.Dates.Add(dateInputADO.Time);
                    }
                    dateInputADO.IsVisibleMultiDate = true;
                    UcDateReload(dateInputADO);
                    //ucDateProcessor.Reload(ucDate, dateInputADO);
                    //this.intructionTimeSelecteds = ucDateProcessor.GetValue(ucDate);
                }
                //if (!isInit)
                //{
                //    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                //    var data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => o.LOGINNAME.ToUpper().Equals(loginName.ToUpper())).FirstOrDefault();
                //    if (data != null)
                //    {
                //        this.cboConsultantUser.EditValue = data.LOGINNAME;
                //        this.txtConsultantLoginname.Text = data.LOGINNAME;
                //    }
                //}
                //this.isMultiDateState = false;

                //GridCheckMarksSelection gridCheckMark = cboServiceGroup.Properties.Tag as GridCheckMarksSelection;
                //if (gridCheckMark != null)
                //    gridCheckMark.ClearSelection(cboServiceGroup.Properties.View);

                //if (HisConfigCFG.SetRequestRoomByBedRoomWhenBeingInSurgery == "1")
                //{
                //    txtAssignRoomCode.Enabled = cboAssignRoom.Enabled = true;
                //}

                //this.beditRoom.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void UcDateReload(HIS.UC.DateEditor.ADO.DateInputADO input)
        {
            try
            {
                DateTime now = DateTime.Now;
                if (ContructorIntructionTime > 0)
                {
                    now = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ContructorIntructionTime) ?? DateTime.Now;
                }
                DateTime nowTmp = DateTime.Now;
                if (input != null && input.Time != DateTime.MinValue && input.Dates != null && input.Dates.Count > 0)
                {
                    //this.timeIntruction.EditValue = input.Time.ToString("HH:mm");
                    nowTmp = input.Time;
                    this.intructionTimeSelected = new List<DateTime?>();
                    this.intructionTimeSelected.AddRange(input.Dates);
                }
                else
                {
                    //this.txtInstructionTime.Visible = false;
                    //this.dtInstructionTime.Visible = true;
                    //this.timeIntruction.EditValue = now.ToString("HH:mm");
                    nowTmp = now;
                    this.intructionTimeSelected = new List<DateTime?>();
                    this.intructionTimeSelected.Add(now);
                }

                System.DateTime today = new DateTime(now.Year, now.Month, now.Day, 0, 0, 1);
                //this.timeSelested = today.Add(timeIntruction.TimeSpan);
                //this.dtInstructionTime.EditValue = nowTmp;
                DateTime date = DateTime.Now;
                //string time = DateTime.Now.ToString("hh:mm:ss");
                string dateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                long time = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                this.intructionTimeSelecteds.Add(time);
                this.InstructionTime = intructionTimeSelecteds.First();

                //this.chkMultiIntructionTime.Checked = false;

                //if (input != null && input.IsVisibleMultiDate.HasValue)
                //{
                //    this.lcichkMultiDate.Visibility = (input.IsVisibleMultiDate.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                //}
                //this.isMultiDateState = chkMultiIntructionTime.Checked;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetExroRoom()
        {
            try
            {
                CommonParam param = new CommonParam();
                V_HIS_ROOM currentWorkingRoom = null;
                currentWorkingRoom = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().First(o => o.ID == this.currentModule.RoomId);
                if (currentWorkingRoom != null)
                {
                    CommonParam paramGet = new CommonParam();
                    MOS.Filter.HisExroRoomFilter exroRoomFilter = new MOS.Filter.HisExroRoomFilter();
                    exroRoomFilter.ROOM_ID = currentWorkingRoom.ID;
                    exroRoomFilter.IS_ACTIVE = 1;
                    this.exroRooms = new BackendAdapter(paramGet).Get<List<HIS_EXRO_ROOM>>("api/HisExroRoom/Get", ApiConsumer.ApiConsumers.MosConsumer, exroRoomFilter, paramGet);

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => paramGet), paramGet));
                    dteCommonParam = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(paramGet.Now) ?? DateTime.Now;
                    //this.exroRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXRO_ROOM>().Where(o => o.ROOM_ID == currentWorkingRoom.ID).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkEditIcd_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkEditIcd.Checked == true)
                {
                    cboIcds.Visible = false;
                    txtIcdMainText.Visible = true;
                    if (this.IsObligatoryTranferMediOrg)
                        txtIcdMainText.Text = this._TextIcdName;
                    else
                        txtIcdMainText.Text = cboIcds.Text;
                    txtIcdMainText.Focus();
                    txtIcdMainText.SelectAll();
                }
                else if (chkEditIcd.Checked == false)
                {
                    txtIcdMainText.Visible = false;
                    cboIcds.Visible = true;
                    txtIcdMainText.Text = cboIcds.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkEditIcdCause_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkEditIcdCause.Checked == true)
                {
                    cboIcdsCause.Visible = false;
                    txtIcdMainTextCause.Visible = true;
                    if (this.IsObligatoryTranferMediOrg)
                        txtIcdMainTextCause.Text = this._TextIcdName;
                    else
                        txtIcdMainTextCause.Text = cboIcds.Text;
                    txtIcdMainTextCause.Focus();
                    txtIcdMainTextCause.SelectAll();
                }
                else if (chkEditIcdCause.Checked == false)
                {
                    txtIcdMainTextCause.Visible = false;
                    cboIcdsCause.Visible = true;
                    txtIcdMainTextCause.Text = cboIcds.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemcboExcuteRoom_TabService_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.GridLookUpEdit edit = sender as DevExpress.XtraEditors.GridLookUpEdit;
                if (edit == null) return;
                if (edit.EditValue != null)
                {
                    this.gridViewServiceProcess.FocusedColumn = this.grcServiceCode_TabService;
                    this.gridViewServiceProcess.FocusedColumn = this.gridColumnExecuteRoomName__TabService;
                    gridViewServiceProcess.SetRowCellValue(gridViewServiceProcess.FocusedRowHandle, gridColumnExecuteRoomName__TabService, edit.EditValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void popupControlContainer1_CloseUp(object sender, EventArgs e)
        {
            if (this.gridView7.IsEditing)
                this.gridView7.CloseEditor();

            if (this.gridView7.FocusedRowModified)
                this.gridView7.UpdateCurrentRow();
        }

        private void gridView7_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                WaitingManager.Show();

                if (e.Column.FieldName == "Check")
                {
                    var changedItem = gridView7.GetRow(e.RowHandle) as LoaiPhieuInADO;
                    if (changedItem != null && changedItem.Check)
                    {
                        if (changedItem.ID == "gridView7_1")
                        {
                            var tongHop = lstLoaiPhieu.FirstOrDefault(x => x.ID == "gridView7_4");
                            if (tongHop != null) tongHop.Check = false;
                        }
                        else if (changedItem.ID == "gridView7_4")
                        {
                            var dichVu = lstLoaiPhieu.FirstOrDefault(x => x.ID == "gridView7_1");
                            if (dichVu != null) dichVu.Check = false;
                        }

                        gridView7.RefreshData();
                    }
                }
                foreach (var item in lstLoaiPhieu)
                {
                    //if (item.ID == "gridView7_3") continue;
                    HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == item.ID && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                    if (csAddOrUpdate != null)
                    {
                        csAddOrUpdate.VALUE = (item.Check ? "1" : "");
                    }
                    else
                    {
                        csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                        csAddOrUpdate.KEY = item.ID;
                        csAddOrUpdate.VALUE = (item.Check ? "1" : "");
                        csAddOrUpdate.MODULE_LINK = moduleLink;
                        if (this.currentControlStateRDO == null)
                            this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                        this.currentControlStateRDO.Add(csAddOrUpdate);
                    }
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataServiceReqById(long id)
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                HisServiceReqFilter filter = new HisServiceReqFilter();
                filter.ID = id;
                filter.ColumnParams = new List<string>()
                    {
                        "ID",
                        "IS_EMERGENCY",
                        "IS_NOT_USE_BHYT",
                        "IS_NOT_REQUIRE_FEE",
                        "SERVICE_REQ_CODE",
                        "IS_MAIN_EXAM",
                        "IS_ANTIBIOTIC_RESISTANCE",
                        "TDL_PATIENT_TYPE_ID"
                    };
                filter.ColumnParams = filter.ColumnParams.Distinct().ToList();
                var serviceReqs = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/GetDynamic", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (!this.workingAssignServiceADO.IsNotUseBhyt.HasValue)
                {
                    if (serviceReqs != null && serviceReqs.Count > 0)
                    {
                        this.serviceReqMain = serviceReqs.FirstOrDefault();

                        this.isNotUseBhyt = this.serviceReqMain != null ? (this.serviceReqMain.IS_NOT_USE_BHYT.HasValue && this.serviceReqMain.IS_NOT_USE_BHYT == 1) : false;

                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("LoadDataServiceReqById. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task GetLCounter1Async()
        {
            try
            {
                HisRoomCounterLView1Filter exetuteFilter = new HisRoomCounterLView1Filter();
                exetuteFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                exetuteFilter.BRANCH_ID = WorkPlace.GetBranchId();
                this.hisRoomCounters = await new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).GetAsync<List<L_HIS_ROOM_COUNTER_1>>("api/HisRoom/GetCounterLView1", ApiConsumers.MosConsumer, exetuteFilter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TimerGetDataGetLCounter1()
        {
            try
            {
                if (HisConfigCFG.MaxPatientByDay == 1)
                {
                    var startTimeSpan = TimeSpan.Zero;
                    var periodTimeSpan = TimeSpan.FromSeconds(30);
                    //var Timer = new System.Threading.Timer(GetLCounter1Async() , null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
                    var timer = new System.Threading.Timer((e) =>
                    {
                        GetLCounter1Async();
                    }, null, startTimeSpan, periodTimeSpan);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void ProcessInitEventForGridServieProcess()
        {
            try
            {
                //if (ApplicationFontWorker.GetFontSize() != ApplicationFontConfig.FontSize825)
                //{
                this.gridViewServiceProcess.CalcRowHeight += new DevExpress.XtraGrid.Views.Grid.RowHeightEventHandler(this.gridViewServiceProcess_CalcRowHeight);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_CalcRowHeight(object sender, RowHeightEventArgs e)
        {
            try
            {
                if (gridViewServiceProcess.IsFilterRow(e.RowHandle))
                {
                    var fontSize = ApplicationFontWorker.GetFontSize();
                    if (fontSize == ApplicationFontConfig.FontSize825)
                    {
                        e.RowHeight = 23;
                    }
                    else if (fontSize == ApplicationFontConfig.FontSize875)
                    {
                        e.RowHeight = 23;
                        //txtServiceName_Search.Location = new Point(180, 23);
                        //txtServiceCode_Search.Location = new Point(31, 23);
                        //txtServiceBhytCode_Search.Location = new Point(107, 23);
                    }
                    else if (fontSize == ApplicationFontConfig.FontSize925)
                    {
                        e.RowHeight = 25;
                        //txtServiceName_Search.Location = new Point(180, 25);
                        //txtServiceCode_Search.Location = new Point(31, 25);
                        //txtServiceBhytCode_Search.Location = new Point(107, 23);
                    }
                    else if (fontSize == ApplicationFontConfig.FontSize975)
                    {
                        e.RowHeight = 27;
                        //txtServiceName_Search.Location = new Point(180, 27);
                        //txtServiceCode_Search.Location = new Point(31, 27);
                        //txtServiceBhytCode_Search.Location = new Point(107, 23);
                    }
                    else if (fontSize == ApplicationFontConfig.FontSize1025)
                    {
                        //txtServiceName_Search.Location = new Point(180, 29);
                        //txtServiceCode_Search.Location = new Point(31, 29);
                        //txtServiceBhytCode_Search.Location = new Point(107, 23);
                        e.RowHeight = 30;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemCboPrimaryPatientType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    DataGridAdo ssADO = (DataGridAdo)gridViewServiceProcess.GetFocusedRow();
                    if (ssADO != null)
                    {
                        ssADO.PRIMARY_PATIENT_TYPE_ID = null;
                        this.gridControlServiceProcess.RefreshDataSource();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void tooltipService_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == this.gridControlServiceProcess)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = this.gridControlServiceProcess.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell && (info.Column.FieldName == "SERVICE_CONDITION_NAME" || info.Column.FieldName == "TDL_SERVICE_NAME" || info.Column.FieldName == "TDL_HEIN_SERVICE_BHYT_CODE"))
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;
                            string text = "";
                            if (info.Column.FieldName == "SERVICE_CONDITION_NAME")
                            {
                                text = (view.GetRowCellValue(lastRowHandle, "SERVICE_REQ_STT_NAME") ?? "").ToString();
                            }
                            if (info.Column.FieldName == "TDL_SERVICE_NAME")
                            {
                                text = (view.GetRowCellValue(lastRowHandle, "TDL_SERVICE_NAME") ?? "").ToString();
                            }
                            if (info.Column.FieldName == "TDL_HEIN_SERVICE_BHYT_CODE")
                            {
                                text = (view.GetRowCellValue(lastRowHandle, "TDL_HEIN_SERVICE_BHYT_CODE") ?? "").ToString();
                            }
                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataSereServToGetPatientType()
        {
            try
            {
                if (serviceReqParentId != null && serviceReqParentId > 0)
                {
                    CommonParam param = new CommonParam();
                    HisSereServFilter fl = new HisSereServFilter();
                    fl.SERVICE_REQ_ID = serviceReqParentId;
                    var datas = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumer.ApiConsumers.MosConsumer, fl, param);
                    if (datas != null && datas.Count > 0)
                        hisSereServForGetPatientType = datas[0];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemcboExcuteRoomPlus_TabService_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.GridLookUpEdit edit = sender as DevExpress.XtraEditors.GridLookUpEdit;
                if (edit == null) return;
                if (edit.EditValue != null)
                {
                    this.gridViewServiceProcess.FocusedColumn = this.grcServiceCode_TabService;
                    this.gridViewServiceProcess.FocusedColumn = this.gridColumnExecuteRoomName__TabService;
                    gridViewServiceProcess.SetRowCellValue(gridViewServiceProcess.FocusedRowHandle, gridColumnExecuteRoomName__TabService, edit.EditValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void toggleSwitchDataChecked_Toggled(object sender, EventArgs e)
        {
            try
            {
                if (toggleSwitchDataChecked.IsOn)
                {
                    // BẬT → Chỉ hiển thị đã tích
                    var filteredData = this.DataGridAdo.Where(x => x.IsChecked == true).ToList();
                    gridControlServiceProcess.DataSource = filteredData;
                }
                else
                {
                    // TẮT → Hiển thị tất cả
                    gridControlServiceProcess.DataSource = this.DataGridAdo;
                }

                gridViewServiceProcess.RefreshData();
                RestoreSelection();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RestoreSelection()
        {
            try
            {
                gridViewServiceProcess.ClearSelection();

                for (int i = 0; i < gridViewServiceProcess.DataRowCount; i++)
                {
                    var item = gridViewServiceProcess.GetRow(i) as DataGridAdo;
                    if (item != null && item.IsChecked)
                    {
                        gridViewServiceProcess.SelectRow(i);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToGrid(bool isResetSearchtext)
        {
            try
            {
                this.gridViewServiceProcess.ClearGrouping();
                var allDatas = this.DataGridAdo != null && this.DataGridAdo.Count > 0 ? this.DataGridAdo.AsQueryable() : null;
                List<DataGridAdo> listSereServADO = null;
                List<long> serviceTypeIds = new List<long>();
                if (isResetSearchtext)
                {
                    this.notSearch = true;
                    this.notSearch = false;
                }

                if (this.toggleSwitchDataChecked.EditValue != null && (this.toggleSwitchDataChecked.EditValue ?? "").ToString().ToLower() == "true" && allDatas != null && allDatas.Count() > 0)
                {
                    serviceTypeIds.AddRange(ServiceParentADOs.Select(o => o.SERVICE_TYPE_ID).Distinct());
                    listSereServADO = allDatas.Where(o => o.IsChecked).ToList();
                    this.ChangeStateGroupInGrid(groupType__ServiceTypeName);
                }
                else
                {
                    this.ChangeStateGroupInGrid(groupType__ServiceTypeName);
                    listSereServADO = allDatas != null && allDatas.Count() > 0 ? allDatas.ToList() : null;
                    serviceTypeIds.AddRange(ServiceParentADOs.Select(p => p.SERVICE_TYPE_ID).Distinct());

                }
                
                this.gridControlServiceProcess.DataSource = null;
                //if (!String.IsNullOrWhiteSpace(txtServiceName_Search.Text) && listSereServADO != null && listSereServADO.Count() > 0)
                //{
                //    listSereServADO = listSereServADO.Where(o => o.SERVICE_NAME_HIDDEN.ToLower().Contains(txtServiceName_Search.Text.ToLower().Trim())).ToList();
                //}
                //if (!String.IsNullOrWhiteSpace(txtServiceCode_Search.Text) && listSereServADO != null && listSereServADO.Count() > 0)
                //{
                //    listSereServADO = listSereServADO.Where(o => o.SERVICE_CODE_HIDDEN.ToLower().Contains(txtServiceCode_Search.Text.ToLower().Trim())).ToList();
                //}
                //if (!String.IsNullOrWhiteSpace(txtServiceBhytCode_Search.Text) && listSereServADO != null && listSereServADO.Count() > 0)
                //{
                //    listSereServADO = listSereServADO.Where(o => o.TDL_HEIN_SERVICE_BHYT_CODE != null && o.TDL_HEIN_SERVICE_BHYT_CODE.ToLower().Contains(txtServiceBhytCode_Search.Text.ToLower().Trim())).ToList();
                //}
                //grcSampleType.VisibleIndex = -1;
                serviceTypeIdSplitReq = new List<long>();
                serviceTypeIdRequired = new List<long>();
                if (serviceTypeIds != null && serviceTypeIds.Count > 0)
                {
                    var serviceTypeSereS = BackendDataWorker.Get<HIS_SERVICE_TYPE>().Where(o => serviceTypeIds.Exists(p => p == o.ID)).ToList();
                    bool IsExistSplitReqBySampleType = serviceTypeSereS.Exists(o => o.IS_SPLIT_REQ_BY_SAMPLE_TYPE == 1);
                    if (((HisConfigCFG.IntegrationVersionValue == "1" && HisConfigCFG.IntegrationOptionValue != "1") || (HisConfigCFG.IntegrationVersionValue == "2" && HisConfigCFG.IntegrationTypeValue != "1")) && IsExistSplitReqBySampleType)
                    {
                        //grcSampleType.VisibleIndex = 21;
                        serviceTypeIdSplitReq.AddRange(serviceTypeSereS.Where(o => o.IS_SPLIT_REQ_BY_SAMPLE_TYPE == 1).Select(o => o.ID));
                    }
                    foreach (var item in serviceTypeIdSplitReq)
                    {
                        var serviceType = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(o => o.ID == item);
                        if (serviceType != null && serviceType.IS_REQUIRED_SAMPLE_TYPE == 1)
                        {
                            serviceTypeIdRequired.Add(serviceType.ID);
                        }
                    }
                }
                this.gridControlServiceProcess.DataSource =
                    listSereServADO != null && listSereServADO.Count > 0 ?
                    listSereServADO
                        .OrderBy(o => o.SERVICE_TYPE_ID)
                        .ThenByDescending(o => o.SERVICE_NUM_ORDER)
                        .ThenBy(o => o.TDL_SERVICE_NAME).ToList()
                    : null;
                this.SetEnableButtonControl(this.actionType);
            }
            catch (Exception ex)
            {
                this.gridViewServiceProcess.EndUpdate();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangeStateGroupInGrid(int type)
        {
            try
            {
                if (type == groupType__ServiceTypeName)
                {
                    gridViewServiceProcess.Columns["SERVICE_TYPE_NAME"].GroupIndex = 0;
                    gridViewServiceProcess.Columns["SERVICE_TYPE_NAME"].SortOrder = ColumnSortOrder.Ascending;
                    gridViewServiceProcess.Columns["SERVICE_TYPE_NAME"].Visible = true;

                    gridViewServiceProcess.Columns["PTTT_GROUP_NAME"].GroupIndex = -1;
                    gridViewServiceProcess.Columns["PTTT_GROUP_NAME"].SortOrder = ColumnSortOrder.None;
                    gridViewServiceProcess.Columns["PTTT_GROUP_NAME"].Visible = false;
                }
                else if (type == groupType__PtttGroupName)
                {
                    gridViewServiceProcess.Columns["SERVICE_TYPE_NAME"].GroupIndex = -1;
                    gridViewServiceProcess.Columns["SERVICE_TYPE_NAME"].SortOrder = ColumnSortOrder.None;
                    gridViewServiceProcess.Columns["SERVICE_TYPE_NAME"].Visible = false;

                    gridViewServiceProcess.Columns["PTTT_GROUP_NAME"].GroupIndex = 0;
                    gridViewServiceProcess.Columns["PTTT_GROUP_NAME"].SortOrder = ColumnSortOrder.Ascending;
                    gridViewServiceProcess.Columns["PTTT_GROUP_NAME"].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToGridParticipants()
        {
            try
            {


                var datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().ToList();
                //var datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ROOM>().Select(o => o.DEPARTMENT_ID).ToList();


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                var sereServADO = (DataGridAdo)this.gridViewServiceProcess.GetFocusedRow();
                var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                serviceIdClick = sereServADO.SERVICE_ID;
                if (sereServADO != null)
                {
                    bool isSelected = view.IsRowSelected(e.RowHandle);
                    if (isSelected)
                    {
                        if (lstSereServExist != null && lstSereServExist.FirstOrDefault(o => o.SERVICE_ID == sereServADO.SERVICE_ID) != null && DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Dịch vụ có thời gian chỉ định nằm trong khoảng thời gian thiết lập của phác đồ điều trị. Thời gian chỉ định {0} (mã y lệnh: {1}). Bạn có muốn tiếp tục?", Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(lstSereServExist.FirstOrDefault(o => o.SERVICE_ID == sereServADO.SERVICE_ID).TDL_INTRUCTION_TIME), lstSereServExist.FirstOrDefault(o => o.SERVICE_ID == sereServADO.SERVICE_ID).TDL_SERVICE_REQ_CODE), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) != DialogResult.Yes)
                        {
                            sereServADO.IsChecked = false;
                        }
                        if (sereServADO.IsChecked)
                        {
                            if (CheckExistServicePaymentLimit(sereServADO.TDL_SERVICE_CODE))
                            {
                                MessageBox.Show(ResourceMessage.DichVuCLSCoGioiHanChiDinhThanhToanBHYT_DeNghiBSXemXetTruocKhiChiDinh, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            ValidOnlyShowNoticeService(sereServADO);

                            if (((HisConfigCFG.IntegrationVersionValue == "1" && HisConfigCFG.IntegrationOptionValue != "1") || (HisConfigCFG.IntegrationVersionValue == "2" && HisConfigCFG.IntegrationTypeValue != "1")) && sereServADO.SERVICE_TYPE_ID > 0 && serviceTypeIdSplitReq != null && serviceTypeIdSplitReq.Count > 0 && serviceTypeIdSplitReq.Exists(o => o == sereServADO.SERVICE_TYPE_ID))
                            {
                                if (this.testSampleTypeId > 0)
                                {
                                    sereServADO.TEST_SAMPLE_TYPE_ID = this.testSampleTypeId;
                                    var sampleType = dataListTestSampleType.FirstOrDefault(o => o.ID == sereServADO.TEST_SAMPLE_TYPE_ID);
                                    if (sampleType != null)
                                    {
                                        sereServADO.TEST_SAMPLE_TYPE_CODE = sampleType.TEST_SAMPLE_TYPE_CODE;
                                        sereServADO.TEST_SAMPLE_TYPE_NAME = sampleType.TEST_SAMPLE_TYPE_NAME;
                                    }
                                }
                            }
                        }
                    }
                    
                    if (isSelected)
                    {
                        this.SetAssignNumOrder(sereServADO, isSelected);
                    }
                    if (//e.Column.FieldName == this.grcAmount_TabService.FieldName || 
                        e.Column.FieldName == this.gridColumnPatientTypeName__TabService.FieldName
                        || e.Column.FieldName == this.gridColumnExecuteRoomName__TabService.FieldName
                        )
                    {
                        if (isSelected)
                        {
                            //Phân biệt giá trị TEST_SAMPLE_TYPE_CODE mặc định bởi TEST_SAMPLE_TYPE_ID = 0;
                            if (((HisConfigCFG.IntegrationVersionValue == "1" && HisConfigCFG.IntegrationOptionValue != "1") || (HisConfigCFG.IntegrationVersionValue == "2" && HisConfigCFG.IntegrationTypeValue != "1")) && sereServADO.SERVICE_TYPE_ID > 0 && serviceTypeIdSplitReq != null && serviceTypeIdSplitReq.Count > 0 && serviceTypeIdSplitReq.Exists(o => o == sereServADO.SERVICE_TYPE_ID))
                            {
                                if (dataListTestSampleType != null && dataListTestSampleType.Count > 0 && sereServADO.TEST_SAMPLE_TYPE_ID == 0 && !string.IsNullOrEmpty(sereServADO.TEST_SAMPLE_TYPE_CODE_DEFAULT))
                                {
                                    var sampleType = dataListTestSampleType.FirstOrDefault(o => o.TEST_SAMPLE_TYPE_CODE == sereServADO.TEST_SAMPLE_TYPE_CODE_DEFAULT);
                                    if (sampleType != null)
                                    {
                                        sereServADO.TEST_SAMPLE_TYPE_ID = sampleType.ID;
                                        sereServADO.TEST_SAMPLE_TYPE_CODE = sereServADO.TEST_SAMPLE_TYPE_CODE_DEFAULT;
                                        sereServADO.TEST_SAMPLE_TYPE_NAME = sampleType.TEST_SAMPLE_TYPE_NAME;
                                    }
                                }
                            }
                            bool isNotChange = (e.Column.FieldName == this.gridColumnExecuteRoomName__TabService.FieldName);
                            if (sereServADO.PATIENT_TYPE_ID > 0)
                            {
                                this.ChoosePatientTypeDefaultlService(sereServADO.PATIENT_TYPE_ID, sereServADO.SERVICE_ID, sereServADO, isNotChange, null, true);
                            }
                            else
                            {
                                this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, sereServADO.SERVICE_ID, sereServADO, isNotChange);
                            }

                            if (sereServADO.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT && sereServADO.IsNotUseBhyt)
                            {
                                if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn đã tích chọn \"Không hưởng BHYT\", nếu đổi đối tượng sang BHYT, phần mềm sẽ tự động bỏ chọn. Bạn có muốn thực hiện không?", HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    sereServADO.IsNotUseBhyt = false;
                                }
                                else
                                {
                                    sereServADO.PATIENT_TYPE_ID = sereServADO.OldPatientType;
                                    gridViewServiceProcess.FocusedColumn = gridColumnPatientTypeName__TabService;
                                }
                            }
                            sereServADO.OldPatientType = sereServADO.PATIENT_TYPE_ID;
                            this.FillDataOtherPaySourceDataRow(sereServADO);

                            if (e.Column.FieldName == this.gridColumnPatientTypeName__TabService.FieldName && isSelected)
                            {
                                List<V_HIS_EXECUTE_ROOM> executeRoomList = null;
                                FilterExecuteRoom(sereServADO, ref executeRoomList);
                                long executeRoomId = this.SetPriorityRequired(executeRoomList);
                                if (executeRoomId <= 0)
                                    executeRoomId = this.SetDefaultExcuteRoom(executeRoomList);
                                //if (sereServADO.TDL_EXECUTE_ROOM_ID <= 0 && executeRoomId > 0)
                                //{
                                sereServADO.TDL_EXECUTE_ROOM_ID = executeRoomId;
                                //}
                                sereServADO.SERVICE_CONDITION_ID = null;
                                sereServADO.SERVICE_CONDITION_NAME = null;

                            }
                            this.VerifyWarningOverCeiling();
                            this.ValidServiceDetailProcessing(sereServADO);
                            this.ProcessNoDifferenceHeinServicePrice(sereServADO);

                            //if (this.selectedSeviceGroups != null && this.selectedSeviceGroups.Count > 0
                            //    && sereServADO.SERVICE_GROUP_ID_SELECTEDs != null && sereServADO.SERVICE_GROUP_ID_SELECTEDs.Count > 0
                            //    && this.serviceDeleteWhileSelectSeviceGroups != null && this.serviceDeleteWhileSelectSeviceGroups.Count > 0)
                            //{
                            //    var svRemove = this.serviceDeleteWhileSelectSeviceGroups.FirstOrDefault(k => k.SERVICE_ID == sereServADO.SERVICE_ID);
                            //    if (svRemove != null)
                            //    {
                            //        this.serviceDeleteWhileSelectSeviceGroups.Remove(svRemove);
                            //    }
                            //}
                            sereServADO.IsChecked = isSelected;
                            if (!VerifyCheckFeeWhileAssign())
                            {
                                this.ResetOneService(sereServADO);
                                sereServADO.IsChecked = false;
                                return;
                            }

                            if (sereServADO.IsAutoExpend == (short?)1 && sereServADO.IsAllowExpend == (short?)1 && !sereServADO.PackagePriceId.HasValue)
                                sereServADO.IsExpend = true;

                            //if (isSelected)
                            //{
                            //    try
                            //    {
                            //        var dataCondition = BranchDataWorker.ServicePatyWithListPatientType(sereServADO.SERVICE_ID, new List<long> { (sereServADO.PATIENT_TYPE_ID > 0 ? sereServADO.PATIENT_TYPE_ID : this.currentHisPatientTypeAlter != null ? this.currentHisPatientTypeAlter.PATIENT_TYPE_ID : 0) });

                            //        long instructionTime = this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0 ? this.intructionTimeSelecteds.FirstOrDefault() : 0;
                            //        List<V_HIS_SERVICE_PATY> dataSource = new List<V_HIS_SERVICE_PATY>();
                            //        long? intructionNumByType = null;
                            //        List<HIS_SERE_SERV> sameServiceType = this.sereServWithTreatment != null ? this.sereServWithTreatment.Where(o => o.TDL_SERVICE_TYPE_ID == sereServADO.SERVICE_TYPE_ID).ToList() : null;
                            //        List<HIS_SERE_SERV> sameService = this.sereServWithTreatment != null ? this.sereServWithTreatment.Where(o => o.SERVICE_ID == sereServADO.SERVICE_ID).ToList() : null;
                            //        intructionNumByType = sameServiceType != null ? (long)sameServiceType.Count() + 1 : 1;
                            //        var intructionNum = sameService != null ? (long)sameService.Count() + 1 : 1;
                            //        foreach (var con in dataCondition)
                            //        {
                            //            var dt = MOS.ServicePaty.ServicePatyUtil.GetApplied(new List<V_HIS_SERVICE_PATY>() { con }, sereServADO.TDL_EXECUTE_BRANCH_ID, sereServADO.TDL_EXECUTE_ROOM_ID, this.requestRoom.ID, this.requestRoom.DEPARTMENT_ID, instructionTime, this.currentHisTreatment.IN_TIME, sereServADO.SERVICE_ID, sereServADO.PATIENT_TYPE_ID, intructionNum, intructionNumByType, sereServADO.PackagePriceId, con.SERVICE_CONDITION_ID, this.currentHisTreatment.TDL_PATIENT_CLASSIFY_ID, null);
                            //            if (dt != null)
                            //                dataSource.Add(dt);
                            //        }
                            //        dataCondition = dataSource;
                            //        if (dataCondition != null && dataCondition.Count > 0)
                            //        {
                            //            dataCondition = dataCondition.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.SERVICE_CONDITION_ID.HasValue && o.SERVICE_CONDITION_ID > 0 && o.SERVICE_ID == sereServADO.SERVICE_ID).ToList();
                            //            if (dataCondition != null && dataCondition.Count > 0)
                            //            {
                            //                List<V_HIS_SERVICE_PATY> dataConditionTmps = new List<V_HIS_SERVICE_PATY>();
                            //                foreach (var item in dataCondition)
                            //                {
                            //                    if (dataConditionTmps.Count == 0 || !dataConditionTmps.Exists(t => t.SERVICE_CONDITION_NAME == item.SERVICE_CONDITION_NAME && t.HEIN_RATIO == item.HEIN_RATIO))
                            //                    {
                            //                        dataConditionTmps.Add(item);
                            //                    }
                            //                }
                            //                dataCondition.Clear();
                            //                dataCondition.AddRange(dataConditionTmps);
                            //                GridViewInfo info = gridViewServiceProcess.GetViewInfo() as GridViewInfo;
                            //                //GridCellInfo cellInfo = info.GetGridCellInfo(gridViewServiceProcess.FocusedRowHandle, gridColumnSERVICE_CONDITION_NAME);
                            //                //TODO
                            //                //Rectangle buttonPosition = cellInfo != null ? cellInfo.Bounds : default(Rectangle);
                            //                //popupControlContainerCondition.ShowPopup(new Point(buttonPosition.X + 532, buttonPosition.Bottom + 170));
                            //                if (dataCondition != null && dataCondition.Count > 0 && lstConditionService != null && lstConditionService.Count > 0)
                            //                {
                            //                    dataCondition = dataCondition.Where(o => lstConditionService.Exists(p => p.SERVICE_ID == sereServADO.SERVICE_ID && p.ID == o.SERVICE_CONDITION_ID)).ToList();
                            //                }
                            //                gridControlCondition.DataSource = null;
                            //                gridControlCondition.DataSource = dataCondition;
                            //                gridControlCondition.Focus();
                            //                gridViewCondition.FocusedRowHandle = 0;
                            //            }
                            //        }
                            //    }
                            //    catch (Exception exx)
                            //    {
                            //        Inventec.Common.Logging.LogSystem.Warn(exx);
                            //    }
                            //}
                        }
                        //else
                        //{
                        //    if (this.selectedSeviceGroups != null && this.selectedSeviceGroups.Count > 0 && sereServADO.SERVICE_GROUP_ID_SELECTEDs != null && sereServADO.SERVICE_GROUP_ID_SELECTEDs.Count > 0)
                        //    {
                        //        if (this.serviceDeleteWhileSelectSeviceGroups == null)
                        //        {
                        //            this.serviceDeleteWhileSelectSeviceGroups = new List<SereServADO>();
                        //        }
                        //        this.serviceDeleteWhileSelectSeviceGroups.Add(sereServADO);
                        //    }
                        //    else
                        //    {
                        //        this.serviceDeleteWhileSelectSeviceGroups = new List<SereServADO>();
                        //    }
                        //    this.ResetOneService(sereServADO);
                        //}
                        this.gridControlServiceProcess.RefreshDataSource();
                        this.SetEnableButtonControl(this.actionType);
                    }
                    else if (e.Column.FieldName == this.gridColumn_Service_PrimaryPatientType.FieldName)
                    {
                        sereServADO.IsChecked = isSelected;
                        if (!VerifyCheckFeeWhileAssign())
                        {
                            this.ResetOneService(sereServADO);
                            sereServADO.IsChecked = false;
                            return;
                        }
                    }

                    this.SetDefaultSerServTotalPrice();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckExistServicePaymentLimit(string ServiceCode)
        {
            bool result = false;
            try
            {
                string servicePaymentLimit = HisConfigCFG.ServiceHasPaymentLimitBHYT.ToLower();
                if (!String.IsNullOrEmpty(servicePaymentLimit))
                {
                    string[] serviceArr = servicePaymentLimit.Split(',');
                    if (serviceArr != null && serviceArr.Length > 0)
                    {
                        if (serviceArr.Contains(ServiceCode.ToLower()))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void SetAssignNumOrder(DataGridAdo sereServ, bool check)
        {
            try
            {
                if (!check)
                {
                    sereServ.AssignNumOrder = null;
                    return;
                }
                if (workingAssignServiceADO.OpenFromBedRoomPartial && this.patientSelectProcessor != null && this.ucPatientSelect != null)
                {
                    var lstPatientSelect = this.patientSelectProcessor.GetSelectedRows(this.ucPatientSelect);
                    if (lstPatientSelect != null && lstPatientSelect.Count > 1)
                    {
                        sereServ.AssignNumOrder = null;
                        return;
                    }
                }
                var ss = sereServsInTreatmentRaw.Where(o => o.SERVICE_ID == sereServ.SERVICE_ID && o.TDL_INTRUCTION_TIME <= InstructionTime).ToList();
                sereServ.AssignNumOrder = ss != null && ss.Count > 0 ? ss.Count + 1 : 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool ValidOnlyShowNoticeService(DataGridAdo serviceChecked__Send)
        {
            bool valid = true;
            try
            {
                List<DataGridAdo> SereServADOSelecteds = new List<DataGridAdo>();
                SereServADOSelecteds.Add(serviceChecked__Send);

                valid = ValidOnlyShowNoticeService(SereServADOSelecteds);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
        private bool ValidOnlyShowNoticeService(List<DataGridAdo> serviceCheckeds__Send)
        {
            bool valid = true;
            try
            {
                string messNotice = "";
                var svNotice = (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0) ? serviceCheckeds__Send.Where(o => !String.IsNullOrEmpty(o.NOTICE)).ToList() : null;
                messNotice = (svNotice != null && svNotice.Count > 0) ? String.Join(",", svNotice.Select(o => o.TDL_SERVICE_NAME + ":" + o.NOTICE).ToArray()) : "";

                if (!String.IsNullOrEmpty(messNotice))
                {
                    MessageManager.Show(messNotice);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private HIS_PATIENT_TYPE ChoosePatientTypeDefaultlService(long patientTypeId, long serviceId, DataGridAdo sereServADO, bool notChangePrimary = false, long? patientTypeAppointmentId = null, bool isChangingPatientType = false)
        {
            MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE result = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
            try
            {
                List<HIS_PATIENT_TYPE> listResult = new List<HIS_PATIENT_TYPE>();
                Inventec.Common.Logging.LogSystem.Debug("ChoosePatientTypeDefaultlService.1");
                if (this.currentPatientTypes != null && this.currentPatientTypes.Count > 0 && currentPatientTypeWithPatientTypeAlter != null && currentPatientTypeWithPatientTypeAlter.Count > 0)
                {
                    this.LoadAppliedPatientType(patientTypeId, sereServADO.SERVICE_ID, ref sereServADO);
                    bool LastOption = false;
                    long intructionTime = this.intructionTimeSelecteds.FirstOrDefault();
                    long treatmentTime = this.currentHisTreatment.IN_TIME;
                    var patientTypeIdInSePas = BranchDataWorker.ServicePatyWithListPatientType(serviceId, this.patientTypeIdAls).Where(o => ((!o.TREATMENT_TO_TIME.HasValue || o.TREATMENT_TO_TIME.Value >= treatmentTime) || (!o.TO_TIME.HasValue || o.TO_TIME.Value >= intructionTime)) && (HisConfigCFG.ServicePatyForServicePackage == "1" ? true : ((!sereServADO.PackagePriceId.HasValue && !o.PACKAGE_ID.HasValue) || (sereServADO.PackagePriceId.HasValue && sereServADO.PackagePriceId.Value == o.PACKAGE_ID)))).Select(o => o.PATIENT_TYPE_ID).ToList();
                    var patientTypeIdInSePasWithServices = BranchDataWorker.ServicePatyWithListPatientType(serviceId, this.patientTypeIdAls);
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientTypeIdInSePasWithServices), patientTypeIdInSePasWithServices));
                    //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServADO.PackagePriceId), sereServADO.PackagePriceId));
                    var patientTypeAll = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList();
                    if (patientTypeIdInSePas == null) patientTypeIdInSePas = new List<long>();
                    var patientTypeIdPlus = patientTypeAll.Where(k => k.BASE_PATIENT_TYPE_ID != null && patientTypeIdInSePas.Contains(k.BASE_PATIENT_TYPE_ID.Value)).ToList();
                    if (patientTypeIdPlus != null && patientTypeIdPlus.Count > 0)
                    {
                        patientTypeIdInSePas.AddRange(patientTypeIdPlus.Select(o => o.ID));
                    }
                    patientTypeIdInSePas = patientTypeIdInSePas.Distinct().ToList();
                    var currentPatientTypeTemps = patientTypeIdInSePas != null ? this.currentPatientTypeWithPatientTypeAlter.Where(o => patientTypeIdInSePas.Contains(o.ID)).OrderBy(o => o.PRIORITY).ToList() : null;
                    var primaryPatientTypeTemps = patientTypeIdInSePas != null ? this.currentPatientTypeWithPatientTypeAlter.Where(o => o.IS_ADDITION == (short)1 && patientTypeIdInSePas.Contains(o.ID)).OrderBy(o => o.PRIORITY).ToList() : null;

                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientTypeIdInSePas), patientTypeIdInSePas)
                    //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentPatientTypes), currentPatientTypes)
                    //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentPatientTypeWithPatientTypeAlter), currentPatientTypeWithPatientTypeAlter));
                    if (currentPatientTypeTemps != null && currentPatientTypeTemps.Count > 0)
                    {
                        if (isChangingPatientType)
                        {
                            Inventec.Common.Logging.LogSystem.Debug("ChoosePatientTypeDefaultlService.6");
                            listResult = currentPatientTypeTemps.Exists(t => t.ID == patientTypeId && (!this.isNotUseBhyt || (this.isNotUseBhyt && t.ID != HisConfigCFG.PatientTypeId__BHYT))) ? currentPatientTypeTemps.Where(o => o.ID == patientTypeId && (!this.isNotUseBhyt || (this.isNotUseBhyt && o.ID != HisConfigCFG.PatientTypeId__BHYT))).ToList() : null;
                        }
                        else
                        {
                            if (patientTypeAppointmentId.HasValue
                                && patientTypeAppointmentId.Value > 0
                                && currentPatientTypeTemps.Exists(e => e.ID == patientTypeAppointmentId.Value))
                            {
                                Inventec.Common.Logging.LogSystem.Debug("ChoosePatientTypeDefaultlService.2");
                                listResult = currentPatientTypeTemps.Where(o => (!this.isNotUseBhyt || (this.isNotUseBhyt && o.ID != HisConfigCFG.PatientTypeId__BHYT)) && o.ID == patientTypeAppointmentId.Value).ToList();
                            }
                            else if (HisConfigCFG.IsSetPrimaryPatientType != "1"
                               && this.requestRoom.DEFAULT_INSTR_PATIENT_TYPE_ID.HasValue
                               && this.requestRoom.DEFAULT_INSTR_PATIENT_TYPE_ID.Value != HisConfigCFG.PatientTypeId__BHYT
                               && currentPatientTypeTemps.Exists(e => e.ID == this.requestRoom.DEFAULT_INSTR_PATIENT_TYPE_ID.Value))
                            {
                                Inventec.Common.Logging.LogSystem.Debug("ChoosePatientTypeDefaultlService. 6 currentRoom Has default instr patient type");
                                listResult = currentPatientTypeTemps.Where(o => (!this.isNotUseBhyt || (this.isNotUseBhyt && o.ID != HisConfigCFG.PatientTypeId__BHYT)) && o.ID == this.requestRoom.DEFAULT_INSTR_PATIENT_TYPE_ID.Value).ToList();
                            }
                            else if (HisConfigCFG.IsSetPrimaryPatientType != "1"
                                && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.HasValue
                                && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value != HisConfigCFG.PatientTypeId__BHYT
                                && currentPatientTypeTemps.Exists(e => e.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value))
                            {
                                Inventec.Common.Logging.LogSystem.Debug("ChoosePatientTypeDefaultlService.3");
                                listResult = currentPatientTypeTemps.Where(o => (!this.isNotUseBhyt || (this.isNotUseBhyt && o.ID != HisConfigCFG.PatientTypeId__BHYT)) && o.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value).ToList();
                            }
                            else if (!IsValidBhytExceedDayAllowForInPatient())
                            {
                                Inventec.Common.Logging.LogSystem.Debug("ChoosePatientTypeDefaultlService.4");
                                listResult = currentPatientTypeTemps.Where(o => (!this.isNotUseBhyt || (this.isNotUseBhyt && o.ID != HisConfigCFG.PatientTypeId__BHYT)) && o.ID == HisConfigCFG.PatientTypeId__VP).ToList();
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Debug("ChoosePatientTypeDefaultlService.5");
                                listResult = ((currentPatientTypeTemps.Exists(t => t.ID == patientTypeId && (!this.isNotUseBhyt || (this.isNotUseBhyt && t.ID != HisConfigCFG.PatientTypeId__BHYT)))) ? (currentPatientTypeTemps.Where(o => o.ID == patientTypeId && (!this.isNotUseBhyt || (this.isNotUseBhyt && o.ID != HisConfigCFG.PatientTypeId__BHYT))).ToList() ?? currentPatientTypeTemps.Where(o => (!this.isNotUseBhyt || (this.isNotUseBhyt && o.ID != HisConfigCFG.PatientTypeId__BHYT))).ToList()) : currentPatientTypeTemps.Where(o => (!this.isNotUseBhyt || (this.isNotUseBhyt && o.ID != HisConfigCFG.PatientTypeId__BHYT))).ToList());
                                LastOption = true;
                            }
                        }

                        if (sereServADO != null && sereServADO.DEFAULT_PATIENT_TYPE_ID != null && currentPatientTypeTemps.Exists(e => e.ID == sereServADO.DEFAULT_PATIENT_TYPE_ID.Value) && !sereServADO.IsNotLoadDefaultPatientType)
                        {
                            listResult = currentPatientTypeTemps.Where(o => (!this.isNotUseBhyt || (this.isNotUseBhyt && o.ID != HisConfigCFG.PatientTypeId__BHYT)) && o.ID == sereServADO.DEFAULT_PATIENT_TYPE_ID.Value).ToList();
                            LastOption = false;
                        }
                        if (listResult != null && listResult.Count > 0 && sereServADO.DO_NOT_USE_BHYT == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && !CheckLoginAdmin.IsAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()))
                        {
                            if (LastOption)
                                listResult = currentPatientTypeTemps.Where(o => o.ID != HisConfigCFG.PatientTypeId__BHYT).ToList();
                            else
                                listResult = listResult.Where(o => o.ID != HisConfigCFG.PatientTypeId__BHYT).ToList();
                        }
                        result = (listResult != null && listResult.Count > 0) ? listResult.FirstOrDefault() : null;

                        #region ĐTTT
                        if (HisConfigCFG.DefaultPatientTypeOption && this.serviceReqParentId != null && this.hisSereServForGetPatientType != null && !sereServADO.IsNotLoadDefaultPatientType)
                        {
                            var lstPatientTypeIdInSePasWithServices = patientTypeIdInSePasWithServices.Where(o => o.PATIENT_TYPE_ID == this.hisSereServForGetPatientType.PATIENT_TYPE_ID).ToList();
                            if (lstPatientTypeIdInSePasWithServices != null && lstPatientTypeIdInSePasWithServices.Count > 0)
                            {
                                if (sereServADO.DO_NOT_USE_BHYT == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && !CheckLoginAdmin.IsAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()) && this.hisSereServForGetPatientType.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                                {
                                    var ptNotBhyt = patientTypeIdInSePasWithServices.FirstOrDefault(o => o.PATIENT_TYPE_ID != HisConfigCFG.PatientTypeId__BHYT);
                                    if (ptNotBhyt != null)
                                    {
                                        sereServADO.PATIENT_TYPE_ID = ptNotBhyt.PATIENT_TYPE_ID;
                                        sereServADO.PATIENT_TYPE_CODE = currentPatientTypes.First(o => o.ID == ptNotBhyt.PATIENT_TYPE_ID).PATIENT_TYPE_CODE;
                                        sereServADO.PATIENT_TYPE_NAME = currentPatientTypes.First(o => o.ID == ptNotBhyt.PATIENT_TYPE_ID).PATIENT_TYPE_NAME;
                                    }
                                }
                                else
                                {
                                    sereServADO.PATIENT_TYPE_ID = this.hisSereServForGetPatientType.PATIENT_TYPE_ID;
                                    sereServADO.PATIENT_TYPE_CODE = currentPatientTypes.First(o => o.ID == this.hisSereServForGetPatientType.PATIENT_TYPE_ID).PATIENT_TYPE_CODE;
                                    sereServADO.PATIENT_TYPE_NAME = currentPatientTypes.First(o => o.ID == this.hisSereServForGetPatientType.PATIENT_TYPE_ID).PATIENT_TYPE_NAME;
                                }
                            }
                            else if (patientTypeIdInSePasWithServices != null && patientTypeIdInSePasWithServices.Count > 0)
                            {
                                sereServADO.PATIENT_TYPE_ID = patientTypeIdInSePasWithServices.OrderBy(o => o.ID).ToList()[0].PATIENT_TYPE_ID;
                                sereServADO.PATIENT_TYPE_CODE = currentPatientTypes.First(o => o.ID == patientTypeIdInSePasWithServices.OrderBy(p => p.PATIENT_TYPE_ID).ToList()[0].PATIENT_TYPE_ID).PATIENT_TYPE_CODE;
                                sereServADO.PATIENT_TYPE_NAME = currentPatientTypes.First(o => o.ID == patientTypeIdInSePasWithServices.OrderBy(p => p.PATIENT_TYPE_ID).ToList()[0].PATIENT_TYPE_ID).PATIENT_TYPE_NAME;
                            }
                        }
                        else if (result != null && sereServADO != null)
                        {
                            sereServADO.PATIENT_TYPE_ID = result.ID;
                            sereServADO.PATIENT_TYPE_CODE = result.PATIENT_TYPE_CODE;
                            sereServADO.PATIENT_TYPE_NAME = result.PATIENT_TYPE_NAME;
                        }
                        #endregion
                        #region Phụ thu
                        if (HisConfigCFG.IsSetPrimaryPatientType == "2")
                        {
                            if (this.currentHisTreatment.PRIMARY_PATIENT_TYPE_ID <= 0 || notChangePrimary)
                            {
                                //sereServADO.PRIMARY_PATIENT_TYPE_ID = null;//TODO
                            }
                            else
                            {
                                if (sereServADO.PATIENT_TYPE_ID == this.currentHisTreatment.PRIMARY_PATIENT_TYPE_ID)
                                {
                                    sereServADO.PRIMARY_PATIENT_TYPE_ID = null;
                                }
                                else
                                {
                                    sereServADO.PRIMARY_PATIENT_TYPE_ID = this.currentHisTreatment.PRIMARY_PATIENT_TYPE_ID;
                                    if (primaryPatientTypeTemps.Exists(e => e.ID == this.currentHisTreatment.PRIMARY_PATIENT_TYPE_ID))
                                    {
                                        var priPaty = primaryPatientTypeTemps.FirstOrDefault(o => o.ID == this.currentHisTreatment.PRIMARY_PATIENT_TYPE_ID);
                                        sereServADO.PRIMARY_PATIENT_TYPE_ID = priPaty.ID;
                                    }
                                    else
                                    {
                                        try
                                        {
                                            var billPaty = this.currentPatientTypes.FirstOrDefault(o => o.ID == this.currentHisTreatment.PRIMARY_PATIENT_TYPE_ID);
                                            string patyName = billPaty != null ? billPaty.PATIENT_TYPE_NAME : "";
                                            sereServADO.ErrorMessagePatientTypeId = String.Format(ResourceMessage.DichVuCoDTPTBatBuocNhungKhongCoChinhSachGia, patyName);
                                            sereServADO.ErrorTypePatientTypeId = ErrorType.Warning;
                                        }
                                        catch (Exception ex)
                                        {
                                            Inventec.Common.Logging.LogSystem.Error(ex);
                                        }
                                    }
                                }
                            }
                        }
                        else if (!notChangePrimary
                            && HisConfigCFG.IsSetPrimaryPatientType == "1"
                            && sereServADO.BILL_PATIENT_TYPE_ID.HasValue
                            && sereServADO.PATIENT_TYPE_ID != sereServADO.BILL_PATIENT_TYPE_ID.Value
                            && BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == sereServADO.PATIENT_TYPE_ID).BASE_PATIENT_TYPE_ID != sereServADO.BILL_PATIENT_TYPE_ID.Value
                            && primaryPatientTypeTemps.Exists(e => e.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value)
                            && sereServADO.IsContainAppliedPatientType)
                        {
                            //if (primaryPatientTypeTemps.Exists(e => e.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value))
                            //{
                            var priPaty = primaryPatientTypeTemps.FirstOrDefault(o => o.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value);
                            sereServADO.PRIMARY_PATIENT_TYPE_ID = priPaty.ID;
                            sereServADO.IsNotChangePrimaryPaty = (sereServADO.IS_NOT_CHANGE_BILL_PATY == (short)1);
                            //LogSystem.Debug("sereServADO.IsNotChangePrimaryPaty: " + sereServADO.IsNotChangePrimaryPaty);
                            //}
                            //else
                            //{
                            //    try
                            //    {
                            //        var billPaty = this.currentPatientTypes.FirstOrDefault(o => o.ID == sereServADO.BILL_PATIENT_TYPE_ID.Value);
                            //        string patyName = billPaty != null ? billPaty.PATIENT_TYPE_NAME : "";
                            //        sereServADO.ErrorMessagePatientTypeId = String.Format(ResourceMessage.DichVuCoDTTTBatBuocNhungKhongCoChinhSachGia, patyName);
                            //        sereServADO.ErrorTypePatientTypeId = ErrorType.Warning;
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        Inventec.Common.Logging.LogSystem.Error(ex);
                            //    }
                            //}
                        }
                        else if (!notChangePrimary
                           && HisConfigCFG.IsSetPrimaryPatientType == "1"
                           && this.requestRoom.DEFAULT_INSTR_PATIENT_TYPE_ID.HasValue
                           && this.requestRoom.DEFAULT_INSTR_PATIENT_TYPE_ID.Value != HisConfigCFG.PatientTypeId__BHYT
                           && primaryPatientTypeTemps.Exists(e => e.ID == this.requestRoom.DEFAULT_INSTR_PATIENT_TYPE_ID.Value)
                           && result.ID != this.requestRoom.DEFAULT_INSTR_PATIENT_TYPE_ID.Value)
                        {
                            var priPaty = primaryPatientTypeTemps.FirstOrDefault(o => o.ID == this.requestRoom.DEFAULT_INSTR_PATIENT_TYPE_ID.Value);
                            sereServADO.PRIMARY_PATIENT_TYPE_ID = priPaty.ID;
                        }
                        else if (!notChangePrimary
                            && HisConfigCFG.IsSetPrimaryPatientType == "1"
                            && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.HasValue
                            && this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value != HisConfigCFG.PatientTypeId__BHYT
                            && primaryPatientTypeTemps.Exists(e => e.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value)
                            && result.ID != this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value)
                        {
                            var priPaty = primaryPatientTypeTemps.FirstOrDefault(o => o.ID == this.currentDepartment.DEFAULT_INSTR_PATIENT_TYPE_ID.Value);
                            sereServADO.PRIMARY_PATIENT_TYPE_ID = priPaty.ID;
                        }
                        else if (!notChangePrimary)
                        {
                            sereServADO.PRIMARY_PATIENT_TYPE_ID = null;//TODO
                        }
                        #endregion
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentPatientTypeTemps), currentPatientTypeTemps));
                    }
                }
                return (result ?? new HIS_PATIENT_TYPE());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void LoadAppliedPatientType(long patientTypeId, long serviceId, ref DataGridAdo sereServADO)
        {
            try
            {
                if (serviceId > 0)
                {
                    var checkService = lstService.Find(o => o.ID == serviceId);
                    if (checkService != null && (string.IsNullOrEmpty(checkService.APPLIED_PATIENT_TYPE_IDS) || IsContainString(checkService.APPLIED_PATIENT_TYPE_IDS, patientTypeId.ToString())) && (string.IsNullOrEmpty(checkService.APPLIED_PATIENT_CLASSIFY_IDS) || IsContainString(checkService.APPLIED_PATIENT_CLASSIFY_IDS, currentPatient.PATIENT_CLASSIFY_ID != null ? currentPatient.PATIENT_CLASSIFY_ID.ToString() : "-1")))
                    {
                        sereServADO.IsContainAppliedPatientType = true;
                    }
                    else
                    {
                        sereServADO.IsContainAppliedPatientType = false;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool IsContainString(string arrStr, string str)
        {
            bool result = false;
            try
            {
                if (arrStr.Contains(","))
                {
                    var arr = arrStr.Split(',');
                    for (int i = 0; i < arr.Length; i++)
                    {
                        result = arr[i] == str;
                        if (result) break;
                    }
                }
                else
                {
                    result = arrStr == str;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        bool IsValidBhytExceedDayAllowForInPatient()
        {
            bool result = true;
            try
            {
                if ((this.currentHisPatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0) == 0 && (this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0) == 0)
                    return result;
                DateTime dtHeinCardFromTimePlus = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentHisPatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0).Value.Date;
                DateTime dtHeinCardToTimePlus = HisConfigCFG.BhytExceedDayAllowForInPatient > 0 ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0).Value.AddDays(HisConfigCFG.BhytExceedDayAllowForInPatient).Date : Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0).Value.Date;

                if (this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
                        && (
                                ((dtHeinCardFromTimePlus > Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(intructionTimeSelecteds.OrderByDescending(o => o).First()).Value.Date
                                || dtHeinCardToTimePlus.Date < Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(intructionTimeSelecteds.OrderByDescending(o => o).First()).Value.Date
                                ))
                            )
                )
                {
                    Inventec.Common.Logging.LogSystem.Warn(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.BhytExceedDayAllowForInPatient), HisConfigCFG.BhytExceedDayAllowForInPatient)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentHisPatientTypeAlter.HEIN_CARD_FROM_TIME), this.currentHisPatientTypeAlter.HEIN_CARD_FROM_TIME)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME), this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME)
                        + Inventec.Common.Logging.LogUtil.TraceData("intructionTimeSelecteds.OrderByDescending(o => o).First()", intructionTimeSelecteds.OrderByDescending(o => o).First())
                        + Inventec.Common.Logging.LogUtil.TraceData("dtHeinCardToTimePlus", dtHeinCardToTimePlus)
                        + Inventec.Common.Logging.LogUtil.TraceData("dtHeinCardFromTimePlus", dtHeinCardFromTimePlus)
                        );
                    result = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void FillDataOtherPaySourceDataRow(DataGridAdo currentRowSereServADO)
        {
            try
            {
                if (currentRowSereServADO.IsChecked && currentRowSereServADO.PATIENT_TYPE_ID > 0)
                {
                    var dataOtherPaySources = BackendDataWorker.Get<HIS_OTHER_PAY_SOURCE>();
                    List<HIS_OTHER_PAY_SOURCE> dataOtherPaySourceTmps = new List<HIS_OTHER_PAY_SOURCE>();
                    dataOtherPaySources = (dataOtherPaySources != null && dataOtherPaySources.Count > 0) ? dataOtherPaySources.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;
                    if (dataOtherPaySources != null && dataOtherPaySources.Count > 0)
                    {
                        var workingPatientType = currentPatientTypes.Where(t => t.ID == currentRowSereServADO.PATIENT_TYPE_ID).FirstOrDefault();

                        if (workingPatientType != null && !String.IsNullOrEmpty(workingPatientType.OTHER_PAY_SOURCE_IDS))
                        {
                            dataOtherPaySourceTmps = dataOtherPaySources.Where(o => ("," + workingPatientType.OTHER_PAY_SOURCE_IDS + ",").Contains("," + o.ID + ",")).ToList();

                            if (currentRowSereServADO.OTHER_PAY_SOURCE_ID == null && dataOtherPaySourceTmps != null && dataOtherPaySourceTmps.Count == 1)
                            {
                                currentRowSereServADO.OTHER_PAY_SOURCE_ID = dataOtherPaySourceTmps[0].ID;
                                currentRowSereServADO.OTHER_PAY_SOURCE_CODE = dataOtherPaySourceTmps[0].OTHER_PAY_SOURCE_CODE;
                                currentRowSereServADO.OTHER_PAY_SOURCE_NAME = dataOtherPaySourceTmps[0].OTHER_PAY_SOURCE_NAME;
                            }
                        }
                        else
                        {
                            dataOtherPaySourceTmps.AddRange(dataOtherPaySources);
                        }

                        if (currentRowSereServADO.OTHER_PAY_SOURCE_ID == null
                            && currentHisTreatment != null && currentHisTreatment.OTHER_PAY_SOURCE_ID.HasValue && currentHisTreatment.OTHER_PAY_SOURCE_ID.Value > 0
                            && dataOtherPaySourceTmps != null && dataOtherPaySourceTmps.Exists(k => k.ID == currentHisTreatment.OTHER_PAY_SOURCE_ID.Value))
                        {
                            var otherPaysourceByTreatment = dataOtherPaySourceTmps.Where(k => k.ID == currentHisTreatment.OTHER_PAY_SOURCE_ID.Value).FirstOrDefault();
                            if (otherPaysourceByTreatment != null)
                            {
                                currentRowSereServADO.OTHER_PAY_SOURCE_ID = otherPaysourceByTreatment.ID;
                                currentRowSereServADO.OTHER_PAY_SOURCE_CODE = otherPaysourceByTreatment.OTHER_PAY_SOURCE_CODE;
                                currentRowSereServADO.OTHER_PAY_SOURCE_NAME = otherPaysourceByTreatment.OTHER_PAY_SOURCE_NAME;
                            }
                        }
                        else if (currentRowSereServADO.OTHER_PAY_SOURCE_ID == null)
                        {
                            HIS.UC.Icd.ADO.IcdInputADO icdData = UcIcdGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                            var serviceTemp = lstService.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.ID == currentRowSereServADO.SERVICE_ID).FirstOrDefault();
                            if (serviceTemp != null && serviceTemp.OTHER_PAY_SOURCE_ID.HasValue && dataOtherPaySourceTmps.Exists(k =>
                                k.ID == serviceTemp.OTHER_PAY_SOURCE_ID.Value)
                                && (String.IsNullOrEmpty(serviceTemp.OTHER_PAY_SOURCE_ICDS) || (icdData != null && !String.IsNullOrEmpty(serviceTemp.OTHER_PAY_SOURCE_ICDS) && !String.IsNullOrEmpty(icdData.ICD_CODE) && ("," + serviceTemp.OTHER_PAY_SOURCE_ICDS.ToLower() + ",").Contains("," + icdData.ICD_CODE.ToLower() + ","))))
                            {
                                var otherPaysourceByService = dataOtherPaySourceTmps.Where(k => k.ID == serviceTemp.OTHER_PAY_SOURCE_ID.Value).FirstOrDefault();
                                if (otherPaysourceByService != null)
                                {
                                    currentRowSereServADO.OTHER_PAY_SOURCE_ID = otherPaysourceByService.ID;
                                    currentRowSereServADO.OTHER_PAY_SOURCE_CODE = otherPaysourceByService.OTHER_PAY_SOURCE_CODE;
                                    currentRowSereServADO.OTHER_PAY_SOURCE_NAME = otherPaysourceByService.OTHER_PAY_SOURCE_NAME;
                                }
                            }
                            //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceTemp), serviceTemp)
                            //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => icdData), icdData));
                        }

                        //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => workingPatientType), workingPatientType)
                        //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataOtherPaySourceTmps), dataOtherPaySourceTmps)
                        //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentRowSereServADO.OTHER_PAY_SOURCE_ID), currentRowSereServADO.OTHER_PAY_SOURCE_ID)
                        //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentRowSereServADO.OTHER_PAY_SOURCE_NAME), currentRowSereServADO.OTHER_PAY_SOURCE_NAME));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FilterExecuteRoom(DataGridAdo data, ref List<V_HIS_EXECUTE_ROOM> executeRoomList)
        {
            try
            {
                var serviceRoomViews = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToArray();
                if (this.allDataExecuteRooms != null && serviceRoomViews != null && serviceRoomViews.Count() > 0)
                {
                    var arrExcuteRoom = serviceRoomViews.Where(o => data != null && o.SERVICE_ID == data.SERVICE_ID).ToList();
                    if (HisConfigCFG.IsAssignRoomByPatientType && PatientTypeRooms != null && PatientTypeRooms.Count > 0 && PatientTypeRooms.Exists(o => o.PATIENT_TYPE_ID == data.PATIENT_TYPE_ID))
                    {
                        var RoomIds = PatientTypeRooms.Where(o => o.PATIENT_TYPE_ID == data.PATIENT_TYPE_ID).Select(o => o.ROOM_ID).ToList();
                        arrExcuteRoom = arrExcuteRoom.Where(o => RoomIds.Contains(o.ROOM_ID)).ToList();
                    }
                    var arrExcuteRoomIds = arrExcuteRoom.Select(o => o.ROOM_ID).ToArray();
                    executeRoomList = ((arrExcuteRoomIds != null && arrExcuteRoomIds.Count() > 0 && this.allDataExecuteRooms != null) ? this.allDataExecuteRooms.Where(o => arrExcuteRoomIds.Contains(o.ROOM_ID)).ToList() : null);
                    List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> executeRoomFilters = ProcessExecuteRoom();
                    executeRoomList = (executeRoomFilters != null && executeRoomFilters.Count > 0 && executeRoomList != null && executeRoomList.Count > 0) ? executeRoomList.Where(p => executeRoomFilters.Select(o => o.ID).Distinct().Contains(p.ID)).ToList() : null;
                    if (this.IsTreatmentInBedRoom)
                    {
                        ProcessAddBedRoomToExecuteRoom(arrExcuteRoomIds.ToList(), ref executeRoomList);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private long SetPriorityRequired(List<V_HIS_EXECUTE_ROOM> excuteRoomList)
        {
            long roomId = 0;
            try
            {
                if (excuteRoomList != null && excuteRoomList.Count > 0)
                {
                    List<V_HIS_EXECUTE_ROOM> lstPriority = excuteRoomList.Where(o => this.exroRooms != null && this.exroRooms.Any(a => a.IS_PRIORITY_REQUIRE == (short)1 && a.EXECUTE_ROOM_ID == o.ID)).ToList();
                    if (lstPriority != null && lstPriority.Count == 1)
                    {
                        return lstPriority[0].ROOM_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return roomId;
        }

        private long SetDefaultExcuteRoom(List<V_HIS_EXECUTE_ROOM> excuteRoomList)
        {
            long sereServADO = 0;
            try
            {
                if (HisConfigCFG.ShowDefaultExecuteRoom == "2")
                {
                    gridColumnExecuteRoomName__TabService.OptionsColumn.AllowEdit = false;
                    return sereServADO;
                }
                if (HisConfigCFG.ShowDefaultExecuteRoom == "1" && excuteRoomList != null && excuteRoomList.Count > 0)
                {
                    V_HIS_EXECUTE_ROOM priority = excuteRoomList.Where(o => this.exroRooms != null && this.exroRooms.Any(a => a.IS_PRIORITY_REQUIRE == (short)1 && a.EXECUTE_ROOM_ID == o.ID)).FirstOrDefault();

                    if (priority != null)
                    {
                        sereServADO = priority.ROOM_ID;
                        return sereServADO;
                    }

                    // cùng phòng làm việc
                    var roomCheck = excuteRoomList.FirstOrDefault(o => o.ROOM_ID == this.currentModule.RoomId);
                    if (roomCheck != null)
                    {
                        sereServADO = roomCheck.ROOM_ID;
                    }
                    else
                    {
                        var currentRoomCheck = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                        if (currentRoomCheck != null)
                        {
                            // cùng khoa
                            var roomCheck1 = excuteRoomList.FirstOrDefault(o => o.DEPARTMENT_ID == currentRoomCheck.DEPARTMENT_ID);
                            if (roomCheck1 != null)
                            {
                                sereServADO = roomCheck1 != null ? roomCheck1.ROOM_ID : -1;
                            }
                            else
                            {
                                // cùng chi nhánh
                                var roomCheck2 = excuteRoomList.FirstOrDefault(o => o.BRANCH_ID == currentRoomCheck.BRANCH_ID);
                                sereServADO = roomCheck2 != null ? roomCheck2.ROOM_ID : -1;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return sereServADO;
        }

        private void ValidServiceDetailProcessing(DataGridAdo sereServADO)
        {
            try
            {
                this.ValidServiceDetailProcessing(sereServADO, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidServiceDetailProcessing(DataGridAdo sereServADO, bool isValidExecuteRoom)
        {
            try
            {
                if (sereServADO != null)
                {
                    if (HisConfigCFG.IsSetPrimaryPatientType != "2" || sereServADO.ErrorTypePatientTypeId == ErrorType.None)
                    {
                        bool vlPatientTypeId = (sereServADO.IsChecked && sereServADO.PATIENT_TYPE_ID <= 0);
                        sereServADO.ErrorMessagePatientTypeId = (vlPatientTypeId ? Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc) : "");
                        sereServADO.ErrorTypePatientTypeId = (vlPatientTypeId ? ErrorType.Warning : ErrorType.None);
                    }

                    bool vlAmount = (sereServADO.IsChecked && sereServADO.AMOUNT <= 0);
                    sereServADO.ErrorMessageAmount = (vlAmount ? Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc) : "");
                    sereServADO.ErrorTypeAmount = (vlAmount ? ErrorType.Warning : ErrorType.None);

                    List<HIS_SERE_SERV> serviceSames = null;
                    List<DataGridAdo> serviceSameResult = null;
                    CheckServiceSameByServiceId(sereServADO, this.currentServiceSames, ref serviceSames, ref serviceSameResult);
                    var existsSereServInDate = this.sereServWithTreatment.Any(o => o.SERVICE_ID == sereServADO.SERVICE_ID && o.TDL_INTRUCTION_TIME.ToString().Substring(0, 8) == intructionTimeSelecteds.First().ToString().Substring(0, 8));

                    if (existsSereServInDate && (serviceSames != null && serviceSames.Count > 0))
                    {
                        sereServADO.ErrorMessageIsAssignDay = String.Format(ResourceMessage.CanhBaoDichVuVaDichVuCungCoCheDaChiDinhTrongNgay, string.Join("; ", serviceSames.Select(o => o.TDL_SERVICE_NAME).ToArray()));
                        sereServADO.ErrorTypeIsAssignDay = ErrorType.Warning;
                    }
                    else if (existsSereServInDate)
                    {
                        sereServADO.ErrorMessageIsAssignDay = (existsSereServInDate ? ResourceMessage.CanhBaoDichVuDaChiDinhTrongNgay : "");
                        sereServADO.ErrorTypeIsAssignDay = (existsSereServInDate ? ErrorType.Warning : ErrorType.None);
                    }
                    else if (serviceSames != null && serviceSames.Count > 0)
                    {
                        sereServADO.ErrorMessageIsAssignDay = String.Format(ResourceMessage.CanhBaoDichVuCungCoCheDaChiDinhTrongNgay, string.Join("; ", serviceSames.Select(o => o.TDL_SERVICE_NAME).ToArray()));
                        sereServADO.ErrorTypeIsAssignDay = ErrorType.Warning;
                    }
                    else if (serviceSameResult != null && serviceSameResult.Count > 0)
                    {
                        sereServADO.ErrorMessageIsAssignDay = String.Format(ResourceMessage.CanhBaoDichVuCungCoChe, string.Join("; ", serviceSameResult.Select(o => o.TDL_SERVICE_NAME).ToArray()));
                        sereServADO.ErrorTypeIsAssignDay = ErrorType.Warning;
                    }
                    else
                    {
                        sereServADO.ErrorMessageIsAssignDay = "";
                        sereServADO.ErrorTypeIsAssignDay = ErrorType.None;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckServiceSameByServiceId(DataGridAdo sereServADO, List<V_HIS_SERVICE_SAME> serviceSameAll, ref List<HIS_SERE_SERV> result, ref List<DataGridAdo> resultSelect)
        {
            try
            {
                result = null;
                resultSelect = null;
                if (sereServADO != null && serviceSameAll != null && serviceSameAll.Count > 0)
                {
                    //Lay ra cac dich vu cung co che voi dich vu dang duoc chon

                    //Lay cac dich vu cung co che voi no
                    List<long> serviceSameId1s = serviceSameAll
                        .Where(o => o.SERVICE_ID == sereServADO.SERVICE_ID && o.SAME_ID != sereServADO.SERVICE_ID)
                        .Select(o => o.SAME_ID).ToList();
                    //Hoac cac dich vu ma no cung co che
                    List<long> serviceSameId2s = serviceSameAll
                        .Where(o => o.SAME_ID == sereServADO.SERVICE_ID && o.SERVICE_ID != sereServADO.SERVICE_ID)
                        .Select(o => o.SERVICE_ID).ToList();

                    List<long> serviceSameIds = new List<long>();

                    if (serviceSameId1s != null)
                    {
                        serviceSameIds.AddRange(serviceSameId1s);
                    }
                    if (serviceSameId2s != null)
                    {
                        serviceSameIds.AddRange(serviceSameId2s);
                    }
                    result = new List<HIS_SERE_SERV>();

                    if (this.sereServWithTreatment != null && this.sereServWithTreatment.Count() > 0)
                    {

                        long intructionTimeFrom = 0, intructionTimeTo = 0;
                        DateTime itime = (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.intructionTimeSelecteds.First()) ?? DateTime.Now);

                        if (itime != null && itime != DateTime.MinValue)
                        {
                            intructionTimeFrom = Inventec.Common.TypeConvert.Parse.ToInt64(itime.ToString("yyyyMMdd") + "000000");
                            intructionTimeTo = Inventec.Common.TypeConvert.Parse.ToInt64(itime.ToString("yyyyMMdd") + "235959");
                        }
                        else
                        {
                            intructionTimeFrom = (Inventec.Common.DateTime.Get.StartDay() ?? 0);
                            intructionTimeTo = (Inventec.Common.DateTime.Get.EndDay() ?? 0);
                        }

                        var checkServiceSame = this.sereServWithTreatment.Where(o => (intructionTimeFrom <= o.TDL_INTRUCTION_TIME && o.TDL_INTRUCTION_TIME <= intructionTimeTo) && serviceSameIds.Contains(o.SERVICE_ID));

                        if (checkServiceSame != null && checkServiceSame.Count() > 0)
                        {
                            var groupServiceSame = checkServiceSame.GroupBy(o => o.SERVICE_ID).ToList();
                            foreach (var serviceSameItems in groupServiceSame)
                            {
                                result.Add(serviceSameItems.FirstOrDefault());
                            }
                        }
                        else
                        {
                            result = null;
                        }
                    }

                    List<DataGridAdo> serviceCheckeds__Send = this.DataGridAdo.FindAll(o => o.IsChecked);
                    if (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0)
                    {
                        var checkServiceSame = serviceCheckeds__Send.Where(o => serviceSameIds.Contains(o.SERVICE_ID));
                        resultSelect = new List<DataGridAdo>();
                        if (checkServiceSame != null && checkServiceSame.Count() > 0)
                        {
                            var groupServiceSame = checkServiceSame.GroupBy(o => o.SERVICE_ID).ToList();
                            foreach (var serviceSameItems in groupServiceSame)
                            {
                                resultSelect.Add(serviceSameItems.FirstOrDefault());
                            }
                        }
                        else
                        {
                            resultSelect = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessNoDifferenceHeinServicePrice(DataGridAdo sereServADO)
        {
            try
            {

                bool finded = false;
                if (this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
                    && HisConfigCFG.NoDifference == "1")
                {
                    var headCards = !String.IsNullOrEmpty(HisConfigCFG.HeadCardNumberNoDifference) ? HisConfigCFG.HeadCardNumberNoDifference.Split(periodSeparators, StringSplitOptions.RemoveEmptyEntries).Where(o => !String.IsNullOrEmpty(o.Trim())).ToList() : null;
                    if ((headCards != null && !String.IsNullOrEmpty(this.currentHisPatientTypeAlter.HEIN_CARD_NUMBER) && headCards.Where(o => this.currentHisPatientTypeAlter.HEIN_CARD_NUMBER.StartsWith(o.Trim())).Any())
                        )
                    {
                        sereServADO.IsNoDifference = true;
                        finded = true;
                    }

                    var departmentCodes = !String.IsNullOrEmpty(HisConfigCFG.DepartmentCodeNoDifference) ? HisConfigCFG.DepartmentCodeNoDifference.Split(periodSeparators, StringSplitOptions.RemoveEmptyEntries).Where(o => !String.IsNullOrEmpty(o.Trim())).ToList() : null;
                    if (departmentCodes != null && departmentCodes.Contains(this.requestRoom.DEPARTMENT_CODE))
                    {
                        sereServADO.IsNoDifference = true;
                        finded = true;
                    }
                    //IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.
                    var heinService = lstService.FirstOrDefault(o => o.ID == sereServADO.SERVICE_ID);
                    if (heinService != null)
                    {
                        sereServADO.HEIN_LIMIT_PRICE = heinService.HEIN_LIMIT_PRICE;
                    }

                    if (!finded)
                    {
                        sereServADO.IsNoDifference = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool VerifyCheckFeeWhileAssign(List<ServiceReqDetailSDO> serviceReqDetails = null)
        {
            bool valid = true;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("VerifyCheckFeeWhileAssign.1");
                this.patientTypeByPT = (currentHisPatientTypeAlter != null && currentHisPatientTypeAlter.PATIENT_TYPE_ID > 0) ? BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.ID == currentHisPatientTypeAlter.PATIENT_TYPE_ID).FirstOrDefault() : null;
                if (this.patientTypeByPT != null && this.patientTypeByPT.IS_CHECK_FEE_WHEN_ASSIGN == 1
                        && this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                {
                    decimal totalPriceServiceSelected = GetFullTotalPriceServiceSelected();
                    if (serviceReqDetails != null && serviceReqDetails.Count > 0)
                        foreach (var item in serviceReqDetails)
                        {
                            if (item.ServiceId > 0 && item.PatientTypeId > 0)
                            {
                                if (BranchDataWorker.DicServicePatyInBranch.ContainsKey(item.ServiceId))
                                {
                                    var data_ServicePrice = BranchDataWorker.ServicePatyWithPatientType(item.ServiceId, item.PatientTypeId).OrderByDescending(m => m.PRIORITY).ThenByDescending(m => m.ID).ToList();
                                    if (data_ServicePrice != null && data_ServicePrice.Count > 0)
                                    {
                                        totalPriceServiceSelected += item.Amount * (data_ServicePrice[0].PRICE * (1 + data_ServicePrice[0].VAT_RATIO));
                                    }
                                }
                            }
                        }

                    if (this.isMultiDateState && intructionTimeSelecteds.Count() > 1)
                    {
                        totalPriceServiceSelected = totalPriceServiceSelected * intructionTimeSelecteds.Count();
                    }

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => transferTreatmentFee), transferTreatmentFee));


                    // - Trong trường hợp ĐỐI TƯỢNG BỆNH NHÂN được check "Không cho phép chỉ định dịch vụ nếu thiếu tiền" (HIS_PATIENT_TYPE có IS_CHECK_FEE_WHEN_ASSIGN = 1) và hồ sơ là "Khám" (HIS_TREATMENT có TDL_TREATMENT_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM) thì kiểm tra:
                    //+ Nếu hồ sơ đang không thừa tiền "Còn thừa" = 0 hoặc hiển thị "Còn thiếu" thì hiển thị thông báo "Bệnh nhân đang nợ tiền, không cho phép chỉ định dịch vụ", người dùng nhấn "Đồng ý" thì tắt form chỉ định.
                    //+ Nếu hồ sơ đang thừa tiền ("Còn thừa" > 0), thì khi người dùng check chọn dịch vụ, nếu số tiền "Phát sinh" > "Còn thừa" thì hiển thị cảnh báo: "Không cho phép chỉ định dịch vụ vượt quá số tiền còn thừa" và không cho phép người dùng check chọn dịch vụ đó.
                    //+ Bỏ qua kiểm tra nợ tiền nếu bệnh nhân là bệnh nhân bảo lãnh
                    if (this.patientTypeByPT != null && this.patientTypeByPT.IS_CHECK_FEE_WHEN_ASSIGN == 1
                            && this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM
                            && (
                            this.transferTreatmentFee > 0 ||
                            (this.transferTreatmentFee < 0 && totalPriceServiceSelected > Math.Abs(this.transferTreatmentFee))
                            )
                        && this.currentModule.RoomTypeId != IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__TD
                        )
                    {
                        //DialogResult myResult = MessageBox.Show(this, String.Format(ResourceMessage.BenhNhanDangNoTienKhogChoPhepChiDinhDV, Inventec.Common.Number.Convert.NumberToString(this.transferTreatmentFee, ConfigApplications.NumberSeperator)), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        MessageBox.Show(this, String.Format(ResourceMessage.KhongChoPhepChiDInhDVVuotQuaSoTienCOnThua), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                        Inventec.Common.Logging.LogSystem.Warn("co cau hinh IS_CHECK_FEE_WHEN_ASSIGN va ke don phong kham ==>" + ResourceMessage.KhongChoPhepChiDInhDVVuotQuaSoTienCOnThua);


                        //if (myResult == DialogResult.Yes)
                        //{

                        valid = false;
                        //}
                        Inventec.Common.Logging.LogSystem.Debug("VerifyCheckFeeWhileAssign.2");
                    }
                    Inventec.Common.Logging.LogSystem.Debug("VerifyCheckFeeWhileAssign.3");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private decimal GetFullTotalPriceServiceSelected()
        {
            decimal totalPrice = 0;
            try
            {
                List<DataGridAdo> serviceCheckeds__Send = this.DataGridAdo.FindAll(o => o.IsChecked);
                foreach (var item in serviceCheckeds__Send)
                {
                    if (item.IsChecked
                        && (item.IsExpend ?? false) == false)
                    {
                        if (BranchDataWorker.DicServicePatyInBranch.ContainsKey(item.SERVICE_ID))
                        {
                            var data_ServicePrice = BranchDataWorker.ServicePatyWithPatientType(item.SERVICE_ID, item.PRIMARY_PATIENT_TYPE_ID ?? item.PATIENT_TYPE_ID).OrderByDescending(m => m.PRIORITY).ThenByDescending(m => m.ID).ToList();
                            if (data_ServicePrice != null && data_ServicePrice.Count > 0)
                            {

                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data_ServicePrice), data_ServicePrice));
                                totalPrice += item.AMOUNT * (data_ServicePrice[0].PRICE * (1 + data_ServicePrice[0].VAT_RATIO));
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return totalPrice;
        }

        private void ResetOneService(DataGridAdo item)
        {
            try
            {
                item.PATIENT_TYPE_ID = 0;
                item.PATIENT_TYPE_CODE = null;
                item.PATIENT_TYPE_NAME = null;
                item.AssignNumOrder = null;
                item.TDL_EXECUTE_ROOM_ID = 0;
                item.IsNotLoadDefaultPatientType = false;
                item.IsContainAppliedPatientType = false;
                item.SERVICE_CONDITION_ID = null;
                item.SERVICE_CONDITION_NAME = null;
                item.OTHER_PAY_SOURCE_ID = null;
                item.OTHER_PAY_SOURCE_CODE = "";
                item.OTHER_PAY_SOURCE_NAME = "";
                item.IsNotChangePrimaryPaty = false;
                item.IsExpend = false;
                item.IsServiceKsk = false;
                item.IsNoDifference = false;
                item.PRIMARY_PATIENT_TYPE_ID = null;
                item.ErrorMessageAmount = "";
                item.ErrorTypeAmount = ErrorType.None;
                item.ErrorMessagePatientTypeId = "";
                item.ErrorTypePatientTypeId = ErrorType.None;
                item.ErrorMessageIsAssignDay = "";
                item.ErrorTypeIsAssignDay = ErrorType.None;
                item.IsNotUseBhyt = false;
                item.TEST_SAMPLE_TYPE_ID = 0;
                item.TEST_SAMPLE_TYPE_CODE = null;
                item.TEST_SAMPLE_TYPE_NAME = null;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewServiceProcess_ColumnFilterChanged(object sender, EventArgs e)
        {
            try
            {
                if (IsClosingForm)
                    return;
                if (gridViewServiceProcess.FocusedColumn == grcServiceCode_TabService
                   && !string.IsNullOrEmpty(gridViewServiceProcess.GetFocusedDisplayText())
                   && gridViewServiceProcess.FocusedRowHandle == DevExpress.XtraGrid.GridControl.AutoFilterRowHandle)
                {
                    toggleSwitchDataChecked.IsOn = false;
                }

                if (gridViewServiceProcess.RowCount == 2)
                {
                    var sereServADO = (DataGridAdo)this.gridViewServiceProcess.GetRow(0);
                    //var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    //bool isSelected = view.IsRowSelected(e.RowHandle);
                    if (sereServADO != null)
                    {
                        sereServADO.IsChecked = true;
                        if (sereServADO.IsChecked)
                        {
                            //Phân biệt giá trị TEST_SAMPLE_TYPE_CODE mặc định bởi TEST_SAMPLE_TYPE_ID = 0;
                            if (((HisConfigCFG.IntegrationVersionValue == "1" && HisConfigCFG.IntegrationOptionValue != "1") || (HisConfigCFG.IntegrationVersionValue == "2" && HisConfigCFG.IntegrationTypeValue != "1")) && sereServADO.SERVICE_TYPE_ID > 0 && serviceTypeIdSplitReq != null && serviceTypeIdSplitReq.Count > 0 && serviceTypeIdSplitReq.Exists(o => o == sereServADO.SERVICE_TYPE_ID))
                            {
                                if (testSampleTypeId > 0)
                                {
                                    sereServADO.TEST_SAMPLE_TYPE_ID = testSampleTypeId;
                                }
                                if (dataListTestSampleType != null && dataListTestSampleType.Count > 0 && sereServADO.TEST_SAMPLE_TYPE_ID == 0 && !string.IsNullOrEmpty(sereServADO.TEST_SAMPLE_TYPE_CODE_DEFAULT))
                                {
                                    var sampleType = dataListTestSampleType.FirstOrDefault(o => o.TEST_SAMPLE_TYPE_CODE == sereServADO.TEST_SAMPLE_TYPE_CODE_DEFAULT);
                                    if (sampleType != null)
                                    {
                                        sereServADO.TEST_SAMPLE_TYPE_ID = sampleType.ID;
                                        sereServADO.TEST_SAMPLE_TYPE_CODE = sereServADO.TEST_SAMPLE_TYPE_CODE_DEFAULT;
                                        sereServADO.TEST_SAMPLE_TYPE_NAME = sampleType.TEST_SAMPLE_TYPE_NAME;
                                    }
                                }
                            }
                            if (CheckExistServicePaymentLimit(sereServADO.TDL_SERVICE_CODE))
                            {
                                MessageBox.Show(ResourceMessage.DichVuCLSCoGioiHanChiDinhThanhToanBHYT_DeNghiBSXemXetTruocKhiChiDinh, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            ValidOnlyShowNoticeService(sereServADO);
                            //if (HisConfigCFG.DefaultPatientTypeOption && this.serviceReqParentId != null && this.hisSereServForGetPatientType != null && !sereServADO.IsNotLoadDefaultPatientType)
                            //{
                            //    sereServADO.PATIENT_TYPE_ID = this.hisSereServForGetPatientType.PATIENT_TYPE_ID;
                            //    sereServADO.PATIENT_TYPE_CODE = currentPatientTypes.First(o => o.ID == this.hisSereServForGetPatientType.PATIENT_TYPE_ID).PATIENT_TYPE_CODE;
                            //    sereServADO.PATIENT_TYPE_NAME = currentPatientTypes.First(o => o.ID == this.hisSereServForGetPatientType.PATIENT_TYPE_ID).PATIENT_TYPE_NAME;
                            //}
                            if (sereServADO.PATIENT_TYPE_ID > 0)
                            {
                                this.ChoosePatientTypeDefaultlService(sereServADO.PATIENT_TYPE_ID, sereServADO.SERVICE_ID, sereServADO, false, null, true);
                            }
                            else
                            {
                                this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, sereServADO.SERVICE_ID, sereServADO);
                            }
                            if (!VerifyCheckFeeWhileAssign())
                            {
                                this.ResetOneService(sereServADO);
                                sereServADO.IsChecked = false;
                            }
                            this.FillDataOtherPaySourceDataRow(sereServADO);

                            List<V_HIS_EXECUTE_ROOM> executeRoomList = null;
                            FilterExecuteRoom(sereServADO, ref executeRoomList);
                            long executeRoomId = this.SetPriorityRequired(executeRoomList);
                            if (executeRoomId <= 0)
                                executeRoomId = this.SetDefaultExcuteRoom(executeRoomList);
                            if (sereServADO.TDL_EXECUTE_ROOM_ID <= 0 && executeRoomId > 0)
                            {
                                sereServADO.TDL_EXECUTE_ROOM_ID = executeRoomId;
                            }
                            if (sereServADO.IsAutoExpend == (short?)1 && sereServADO.IsAllowExpend == (short?)1 && !sereServADO.PackagePriceId.HasValue)
                                sereServADO.IsExpend = true;
                            this.ValidServiceDetailProcessing(sereServADO);
                            this.ProcessNoDifferenceHeinServicePrice(sereServADO);
                            this.SetAssignNumOrder(sereServADO, sereServADO.IsChecked);
                            this.VerifyWarningOverCeiling();
                        }
                        else
                        {
                            this.ResetOneService(sereServADO);
                            sereServADO.IsNoDifference = false;
                        }

                        this.gridControlServiceProcess.RefreshDataSource();
                        this.SetEnableButtonControl(this.actionType);
                        this.SetDefaultSerServTotalPrice();

                        gridViewServiceProcess.ActiveEditor.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmAssignBed_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                IsClosingForm = true;
                this.lstSereServExist = new List<HIS_SERE_SERV>();
                gridViewServiceProcess.ActiveFilter.Clear();
                gridViewServiceProcess.ClearColumnsFilter();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            try
            {
                var info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
                string rowValue = Convert.ToString(this.gridViewServiceProcess.GetGroupRowValue(e.RowHandle, info.Column));
                info.GroupText = rowValue;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (((IList)((BaseView)sender).DataSource) != null && ((IList)((BaseView)sender).DataSource).Count > 0)
                    {
                        DataGridAdo oneServiceSDO = (DataGridAdo)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                        long instructionTime = this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0 ? this.intructionTimeSelecteds.FirstOrDefault() : 0;
                        if (oneServiceSDO != null)
                        {
                            if (e.Column.FieldName == "PRICE_DISPLAY" && oneServiceSDO.IsChecked)
                            {

                                if (oneServiceSDO.AssignSurgPriceEdit.HasValue && (oneServiceSDO.AssignSurgPriceEdit > 0 || oneServiceSDO.IsServiceKsk))
                                {
                                    e.Value = oneServiceSDO.AssignSurgPriceEdit;
                                }
                                else
                                {
                                    e.Value = GetPriceBySurg(oneServiceSDO);
                                }

                            }
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private decimal? GetPriceBySurg(DataGridAdo sereServADOOld)
        {
            decimal? resultData = null;
            decimal? heinLimitPrice = null;
            decimal? heinLimitRatio = null;
            try
            {
                long instructionTime = this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0 ? this.intructionTimeSelecteds.FirstOrDefault() : 0;
                if (sereServADOOld.PATIENT_TYPE_ID != 0 && BranchDataWorker.DicServicePatyInBranch.ContainsKey(sereServADOOld.SERVICE_ID) && instructionTime > 0)
                {
                    List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> dataCombo = new List<V_HIS_EXECUTE_ROOM>();
                    var serviceRoomViews = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>();
                    List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM> arrExcuteRoomCode = new List<V_HIS_SERVICE_ROOM>();
                    if (sereServADOOld.TDL_EXECUTE_ROOM_ID > 0)
                    {
                        dataCombo = this.allDataExecuteRooms.Where(o => sereServADOOld.TDL_EXECUTE_ROOM_ID == o.ROOM_ID).ToList();
                    }
                    else if (this.allDataExecuteRooms != null && this.allDataExecuteRooms.Count > 0 && serviceRoomViews != null && serviceRoomViews.Count > 0)
                    {
                        arrExcuteRoomCode = serviceRoomViews.Where(o => sereServADOOld != null && o.SERVICE_ID == sereServADOOld.SERVICE_ID).ToList();
                        if (HisConfigCFG.IsAssignRoomByPatientType && PatientTypeRooms != null && PatientTypeRooms.Count > 0 && PatientTypeRooms.Exists(o => o.PATIENT_TYPE_ID == sereServADOOld.PATIENT_TYPE_ID))
                        {
                            var RoomIds = PatientTypeRooms.Where(o => o.PATIENT_TYPE_ID == sereServADOOld.PATIENT_TYPE_ID).Select(o => o.ROOM_ID).ToList();
                            arrExcuteRoomCode = arrExcuteRoomCode.Where(o => RoomIds.Contains(o.ROOM_ID)).ToList();
                        }
                        dataCombo = ((arrExcuteRoomCode != null && arrExcuteRoomCode.Count > 0 && this.allDataExecuteRooms != null) ?
                            this.allDataExecuteRooms.Where(o => arrExcuteRoomCode.Select(p => p.ROOM_ID).Contains(o.ROOM_ID) && o.BRANCH_ID == this.requestRoom.BRANCH_ID).ToList()
                            : null);
                    }

                    var checkExecuteRoom = dataCombo != null && dataCombo.Count > 0 ? dataCombo.FirstOrDefault() : null;
                    if (checkExecuteRoom != null)
                    {
                        sereServADOOld.TDL_EXECUTE_BRANCH_ID = checkExecuteRoom.BRANCH_ID;
                    }
                    else
                    {
                        sereServADOOld.TDL_EXECUTE_BRANCH_ID = dataCombo != null && dataCombo.Count > 0 ? dataCombo.FirstOrDefault().BRANCH_ID : HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.GetCurrentBranchId();
                    }
                    long? intructionNumByType = null;

                    List<HIS_SERE_SERV> sameServiceType = this.sereServWithTreatment != null ? this.sereServWithTreatment.Where(o => o.TDL_SERVICE_TYPE_ID == sereServADOOld.SERVICE_TYPE_ID).ToList() : null;
                    List<HIS_SERE_SERV> sameService = this.sereServWithTreatment != null ? this.sereServWithTreatment.Where(o => o.SERVICE_ID == sereServADOOld.SERVICE_ID).ToList() : null;
                    intructionNumByType = sameServiceType != null ? (long)sameServiceType.Count() + 1 : 1;
                    var intructionNum = sameService != null ? (long)sameService.Count() + 1 : 1;

                    List<V_HIS_SERVICE_PATY> servicePaties = BranchDataWorker.ServicePatyWithListPatientType(sereServADOOld.SERVICE_ID, this.patientTypeIdAls);
                    V_HIS_SERVICE_PATY oneServicePatyPrice = new V_HIS_SERVICE_PATY();
                    if (HisConfigCFG.ServicePatyForServicePackage == "1")
                    {
                        //List<V_HIS_SERVICE_PATY> servicePatiesFirst = new List<V_HIS_SERVICE_PATY>();
                        //servicePatiesFirst.Add(this.GetServicePaties(servicePaties, sereServADOOld.TDL_EXECUTE_BRANCH_ID, (sereServADOOld.TDL_EXECUTE_ROOM_ID > 0 ? (long?)sereServADOOld.TDL_EXECUTE_ROOM_ID : null), this.requestRoom.ID, this.requestRoom.DEPARTMENT_ID, instructionTime, this.currentHisTreatment.IN_TIME, sereServADOOld.SERVICE_ID, sereServADOOld.PATIENT_TYPE_ID, intructionNum, intructionNumByType, sereServADOOld.SERVICE_CONDITION_ID, this.currentHisTreatment.TDL_PATIENT_CLASSIFY_ID, null));

                        oneServicePatyPrice = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePaties, sereServADOOld.TDL_EXECUTE_BRANCH_ID, (sereServADOOld.TDL_EXECUTE_ROOM_ID > 0 ? (long?)sereServADOOld.TDL_EXECUTE_ROOM_ID : null), this.requestRoom.ID, this.requestRoom.DEPARTMENT_ID, instructionTime, this.currentHisTreatment.IN_TIME, sereServADOOld.SERVICE_ID, sereServADOOld.PATIENT_TYPE_ID, intructionNum, intructionNumByType, null, sereServADOOld.SERVICE_CONDITION_ID, this.currentHisTreatment.TDL_PATIENT_CLASSIFY_ID, null);
                    }
                    else
                    {
                        oneServicePatyPrice = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePaties, sereServADOOld.TDL_EXECUTE_BRANCH_ID, (sereServADOOld.TDL_EXECUTE_ROOM_ID > 0 ? (long?)sereServADOOld.TDL_EXECUTE_ROOM_ID : null), this.requestRoom.ID, this.requestRoom.DEPARTMENT_ID, instructionTime, this.currentHisTreatment.IN_TIME, sereServADOOld.SERVICE_ID, sereServADOOld.PATIENT_TYPE_ID, intructionNum, intructionNumByType, sereServADOOld.PackagePriceId, sereServADOOld.SERVICE_CONDITION_ID, this.currentHisTreatment.TDL_PATIENT_CLASSIFY_ID, null);
                    }


                    if (sereServADOOld.PRIMARY_PATIENT_TYPE_ID.HasValue)
                    {
                        V_HIS_SERVICE_PATY primary = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePaties, sereServADOOld.TDL_EXECUTE_BRANCH_ID, (sereServADOOld.TDL_EXECUTE_ROOM_ID > 0 ? (long?)sereServADOOld.TDL_EXECUTE_ROOM_ID : null), this.requestRoom.ID, this.requestRoom.DEPARTMENT_ID, instructionTime, this.currentHisTreatment.IN_TIME, sereServADOOld.SERVICE_ID, sereServADOOld.PRIMARY_PATIENT_TYPE_ID.Value, intructionNum, intructionNumByType, sereServADOOld.PackagePriceId, sereServADOOld.SERVICE_CONDITION_ID, this.currentHisTreatment.TDL_PATIENT_CLASSIFY_ID, null);
                        if (oneServicePatyPrice == null || primary == null || (oneServicePatyPrice.PRICE * (1 + oneServicePatyPrice.VAT_RATIO)) >= (primary.PRICE * (1 + primary.VAT_RATIO)))
                        {
                            if (HisConfigCFG.IsSetPrimaryPatientType != "2")
                            {
                                sereServADOOld.PRIMARY_PATIENT_TYPE_ID = null;//TODO
                                sereServADOOld.IsNotChangePrimaryPaty = false;
                            }
                        }
                        oneServicePatyPrice = primary;
                    }

                    if (sereServADOOld.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
                        && sereServADOOld.IsNoDifference.HasValue
                        && sereServADOOld.IsNoDifference.Value)
                    {
                        this.GetHeinLimitPrice(sereServADOOld, instructionTime, this.currentHisTreatment.IN_TIME, ref heinLimitPrice, ref heinLimitRatio);

                        if (heinLimitPrice.HasValue && heinLimitPrice.Value > 0)
                        {
                            resultData = heinLimitPrice;
                        }
                        else if (heinLimitRatio.HasValue && heinLimitRatio.Value > 0 && oneServicePatyPrice != null)
                        {
                            resultData = (oneServicePatyPrice.PRICE * (1 + oneServicePatyPrice.VAT_RATIO) * heinLimitRatio.Value);
                        }
                        else if (oneServicePatyPrice != null)
                        {
                            resultData = (oneServicePatyPrice.PRICE * (1 + oneServicePatyPrice.VAT_RATIO));
                        }
                    }
                    else if (oneServicePatyPrice != null)
                    {
                        resultData = (oneServicePatyPrice.PRICE * (1 + oneServicePatyPrice.VAT_RATIO));
                    }
                }
                else
                {
                    resultData = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return resultData;
        }

        private void GetHeinLimitPrice(DataGridAdo hisService, long instructionTime, long inTime, ref decimal? heinLimitPrice, ref decimal? heinLimitRatio)
        {
            //neu dich vu khai bao gia tran
            if (hisService.HEIN_LIMIT_PRICE.HasValue || hisService.HEIN_LIMIT_PRICE_OLD.HasValue)
            {
                //neu gia ap dung theo ngay vao vien, thi cac benh nhan vao vien truoc ngay ap dung se lay gia cu
                if (hisService.HEIN_LIMIT_PRICE_IN_TIME.HasValue)
                {
                    heinLimitPrice = inTime < hisService.HEIN_LIMIT_PRICE_IN_TIME.Value ? hisService.HEIN_LIMIT_PRICE_OLD : hisService.HEIN_LIMIT_PRICE;
                }
                //neu ap dung theo ngay chi dinh, thi cac chi dinh truoc ngay ap dung se tinh gia cu
                else if (hisService.HEIN_LIMIT_PRICE_INTR_TIME.HasValue)
                {
                    heinLimitPrice = instructionTime < hisService.HEIN_LIMIT_PRICE_INTR_TIME.Value ? hisService.HEIN_LIMIT_PRICE_OLD : hisService.HEIN_LIMIT_PRICE;
                }
                //neu ca 2 truong ko co gia tri thi luon lay theo gia moi
                else
                {
                    heinLimitPrice = hisService.HEIN_LIMIT_PRICE;
                }
            }
        }

        private void gridViewServiceProcess_DataManagerReset(object sender, EventArgs e)
        {

        }

        private void gridViewServiceProcess_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (!this.ValidPatientTypeForAdd())
                    return;

                GridView view = (GridView)sender;
                Point pt = view.GridControl.PointToClient(Control.MousePosition);
                GridHitInfo info = view.CalcHitInfo(pt);
                if ((info.InRow || info.InRowCell)
                    && info.Column.FieldName != this.gridColumnPatientTypeName__TabService.FieldName)
                {
                    var sereServADO = (DataGridAdo)this.gridViewServiceProcess.GetFocusedRow();
                    if (sereServADO != null
                        //&& (sereServADO.IsAllowChecked == null || sereServADO.IsAllowChecked == true)
                        )
                    {
                        UpdateCurrentFocusRow(sereServADO);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool ValidPatientTypeForAdd()
        {
            bool valid = true;
            try
            {
                if (this.currentHisPatientTypeAlter == null || this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == 0)
                {
                    MessageManager.Show(String.Format(ResourceMessage.KhongTimThayDoiTuongThanhToanTrongThoiGianYLenh, Inventec.Common.DateTime.Convert.TimeNumberToDateString(intructionTimeSelecteds.First())));
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private void UpdateCurrentFocusRow(DataGridAdo sereServADO)
        {
            try
            {
                if (sereServADO == null || (sereServADO.IsChecked && sereServADO.PackagePriceId.HasValue))
                    return;

                sereServADO.IsChecked = !sereServADO.IsChecked;
                if (sereServADO.IsChecked)
                {
                    this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, sereServADO.SERVICE_ID, sereServADO);
                    this.FillDataOtherPaySourceDataRow(sereServADO);
                    if (!VerifyCheckFeeWhileAssign())
                    {
                        this.ResetOneService(sereServADO);
                        sereServADO.IsChecked = false;
                        return;
                    }


                    List<V_HIS_EXECUTE_ROOM> executeRoomList = null;
                    FilterExecuteRoom(sereServADO, ref executeRoomList);

                    long executeRoomId = this.SetPriorityRequired(executeRoomList);

                    if (executeRoomId <= 0)
                        executeRoomId = this.SetDefaultExcuteRoom(executeRoomList);

                    //data.TDL_EXECUTE_ROOM_ID = executeRoomDefault;
                    if (sereServADO.TDL_EXECUTE_ROOM_ID <= 0)
                    {
                        sereServADO.TDL_EXECUTE_ROOM_ID = executeRoomId;
                    }
                    if (sereServADO.IsAutoExpend == (short?)1 && sereServADO.IsAllowExpend == (short?)1 && !sereServADO.PackagePriceId.HasValue)
                        sereServADO.IsExpend = true;
                    this.ValidServiceDetailProcessing(sereServADO);
                    this.ProcessNoDifferenceHeinServicePrice(sereServADO);
                    //if (CheckExistServicePaymentLimit(sereServADO.TDL_SERVICE_CODE))
                    //{
                    //    MessageBox.Show(ResourceMessage.DichVuCLSCoGioiHanChiDinhThanhToanBHYT_DeNghiBSXemXetTruocKhiChiDinh, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //}
                }
                else
                {
                    this.ResetOneService(sereServADO);
                    sereServADO.IsNoDifference = false;
                }

                this.gridControlServiceProcess.RefreshDataSource();
                if (sereServADO.IsChecked)
                {
                    this.VerifyWarningOverCeiling();
                }
                this.SetEnableButtonControl(this.actionType);
                this.SetDefaultSerServTotalPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void IN_QR()
        {
            try
            {

                if (this.lstLoaiPhieu != null && this.lstLoaiPhieu.Count > 0)
                {
                    var checkHDBN = this.lstLoaiPhieu.FirstOrDefault(o => o.Check == true && o.ID == "gridView7_2");

                    var checkYCDV = this.lstLoaiPhieu.FirstOrDefault(o => o.Check == true && o.ID == "gridView7_1");

                    var checkQR = this.lstLoaiPhieu.FirstOrDefault(o => o.Check == true && o.ID == "gridView7_3");

                    if (checkHDBN != null)
                    {
                        InPhieuHuoangDanBenhNhan(true);
                    }

                    if (checkYCDV != null)
                    {
                        InPhieuYeuCauDichVu(true);
                    }

                    if (checkQR != null)
                    {
                        InYeuCauThanhToanQR(chkPrint.Checked, false, true);
                    }
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuHuoangDanBenhNhan(bool isSaveAndShow, MPS.ProcessorBase.PrintConfig.PreviewType? preview = null)
        {
            try
            {
                var PrintServiceReqProcessor = new HIS.Desktop.Plugins.Library.PrintServiceReqTreatment.PrintServiceReqTreatmentProcessor(this.serviceReqComboResultSDO.ServiceReqs, currentModule != null ? this.currentModule.RoomId : 0, preview);
                PrintServiceReqProcessor.DlgSendResultSigned = GetDocmentSigned;
                PrintServiceReqProcessor.Print("Mps000276", true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void InPhieuYeuCauDichVu(bool isSaveAndShow, MPS.ProcessorBase.PrintConfig.PreviewType? previewType = null)
        {
            try
            {
                string configValue = HisConfigCFG.IsAllowSignaturePrint;

                if (!string.IsNullOrWhiteSpace(configValue))
                {
                    var allowedModules = configValue
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .ToList();

                    if (allowedModules.Contains("HIS.Desktop.Plugins.AssignService"))
                    {
                        previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintPreview;
                    }
                    else
                    {
                        previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow;
                    }
                }
                if (serviceReqComboResultSDO != null)
                {
                    CommonParam param = new CommonParam();
                    List<V_HIS_BED_LOG> bedLogs = new List<V_HIS_BED_LOG>();
                    // get bedLog
                    if (this.currentHisTreatment != null && this.serviceReqComboResultSDO != null && this.serviceReqComboResultSDO.ServiceReqs != null && this.serviceReqComboResultSDO.ServiceReqs.Count > 0)
                    {
                        MOS.Filter.HisBedLogViewFilter bedLogViewFilter = new MOS.Filter.HisBedLogViewFilter();
                        bedLogViewFilter.TREATMENT_ID = currentHisTreatment.ID;
                        bedLogViewFilter.DEPARTMENT_IDs = this.serviceReqComboResultSDO.ServiceReqs.Select(o => o.REQUEST_DEPARTMENT_ID).Distinct().ToList();
                        bedLogs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumer.ApiConsumers.MosConsumer, bedLogViewFilter, param);
                    }
                    var PrintServiceReqProcessor = previewType != null ? new Library.PrintServiceReq.PrintServiceReqProcessor(serviceReqComboResultSDO, currentHisTreatment, bedLogs, (currentModule != null ? currentModule.RoomId : 0), previewType.Value, GetDocmentSigned)
                        : new Library.PrintServiceReq.PrintServiceReqProcessor(serviceReqComboResultSDO, currentHisTreatment, bedLogs, (currentModule != null ? currentModule.RoomId : 0));
                    PrintServiceReqProcessor.SaveNPrint(isSaveAndShow);

                    if (this.serviceReqComboResultSDO.SereServs != null)
                    {
                        ProcessOpenVoBenhAn(serviceReqComboResultSDO.SereServs);
                        Inventec.Common.Logging.LogSystem.Debug("PRINT NOW serviceReqComboResultSDO.SereServs: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.serviceReqComboResultSDO.SereServs), this.serviceReqComboResultSDO.SereServs));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void InYeuCauThanhToanQR(bool printTH, bool isSign, bool isPrintPreview)
        {
            try
            {

                if (serviceReqComboResultSDO != null)
                {
                    BordereauInitData data = new BordereauInitData();
                    HIS.Desktop.Plugins.Library.PrintBordereau.PrintBordereauProcessor processor = new PrintBordereauProcessor(this.currentModule.RoomId, this.currentModule.RoomTypeId, treatmentId, patientPrint.ID, null, null, GetDocmentSigned);
                    //if (IsActionButtonPrintBill)
                    //    processor.IsActionButtonPrintBill = true;
                    if (printTH && !isSign)
                    {
                        Inventec.Common.Logging.LogSystem.Error("Mps000446_____ PRINT_NOW");
                        processor.Print("Mps000446", PrintOption.Value.PRINT_NOW, null);
                    }
                    else if (printTH && isSign)
                    {
                        Inventec.Common.Logging.LogSystem.Error("Mps000446_____ PRINT_NOW_AND_EMR_SIGN_NOW");
                        processor.Print("Mps000446", PrintOption.Value.PRINT_NOW_AND_EMR_SIGN_NOW, null);
                    }
                    else if (isPrintPreview && isSign)
                    {
                        Inventec.Common.Logging.LogSystem.Error("Mps000446_____ EMR_SIGN_AND_PRINT_PREVIEW");
                        processor.Print("Mps000446", PrintOption.Value.EMR_SIGN_AND_PRINT_PREVIEW, null);
                    }
                    else if (!printTH && isSign)
                    {
                        Inventec.Common.Logging.LogSystem.Error("Mps000446_____ EMR_SIGN_NOW");
                        processor.Print("Mps000446", PrintOption.Value.EMR_SIGN_NOW, null);
                    }
                    else if (isPrintPreview)
                    {
                        Inventec.Common.Logging.LogSystem.Error("Mps000446_____ NULL");
                        processor.Print("Mps000446", null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        Dictionary<long, List<DocumentSignedUpdateIGSysResultDTO>> dSignedList = new Dictionary<long, List<DocumentSignedUpdateIGSysResultDTO>>();
        private void GetDocmentSigned(DocumentSignedUpdateIGSysResultDTO dTO)
        {
            try
            {
                if (!dSignedList.ContainsKey(this.serviceReqComboResultSDO.ServiceReqs[0].TREATMENT_ID))
                    dSignedList[this.serviceReqComboResultSDO.ServiceReqs[0].TREATMENT_ID] = new List<DocumentSignedUpdateIGSysResultDTO>();
                dSignedList[this.serviceReqComboResultSDO.ServiceReqs[0].TREATMENT_ID].Add(dTO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void ProcessOpenVoBenhAn(List<V_HIS_SERE_SERV> sereServs)
        {
            try
            {
                var emrFormsCodes = lstService.Where(o => sereServs.Exists(p => p.SERVICE_ID == o.ID) && !string.IsNullOrEmpty(o.EMR_FORM_CODES)).Select(o => o.EMR_FORM_CODES).ToList();
                if (emrFormsCodes != null && emrFormsCodes.Count > 0 && serviceReqComboResultSDO != null)
                {
                    HIS.Desktop.Plugins.Library.FormMedicalRecord.Base.EmrInputADO emrInputAdo = new Library.FormMedicalRecord.Base.EmrInputADO();
                    emrInputAdo.TreatmentId = serviceReqComboResultSDO.ServiceReqs.FirstOrDefault().TREATMENT_ID;
                    emrInputAdo.PatientId = serviceReqComboResultSDO.ServiceReqs.FirstOrDefault().TDL_PATIENT_ID;
                    emrInputAdo.roomId = this.currentModule.RoomId;
                    if (currentTreatment.EMR_COVER_TYPE_ID != null)
                    {
                        emrInputAdo.EmrCoverTypeId = currentTreatment.EMR_COVER_TYPE_ID;
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<HIS_EMR_COVER_CONFIG>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                            && o.ROOM_ID == this.currentModule.RoomId
                        && o.TREATMENT_TYPE_ID == currentTreatment.TDL_TREATMENT_TYPE_ID
                        ).ToList();
                        if (data != null && data.Count > 0)
                        {
                            if (data.Count == 1)
                            {
                                emrInputAdo.EmrCoverTypeId = data.FirstOrDefault().EMR_COVER_TYPE_ID;

                            }
                            else
                            {
                                emrInputAdo.lstEmrCoverTypeId = new List<long>();
                                emrInputAdo.lstEmrCoverTypeId = data.Select(o => o.EMR_COVER_TYPE_ID).ToList();
                            }
                        }
                        else
                        {
                            var DepartmentID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.currentModule.RoomId).DepartmentId;

                            var DataConfig = BackendDataWorker.Get<HIS_EMR_COVER_CONFIG>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                        && o.DEPARTMENT_ID == DepartmentID && o.TREATMENT_TYPE_ID == currentTreatment.TDL_TREATMENT_TYPE_ID).ToList();

                            if (DataConfig != null && DataConfig.Count > 0)
                            {
                                if (DataConfig.Count == 1)
                                {
                                    emrInputAdo.EmrCoverTypeId = DataConfig.FirstOrDefault().EMR_COVER_TYPE_ID;
                                }
                                else
                                {
                                    emrInputAdo.lstEmrCoverTypeId = new List<long>();
                                    emrInputAdo.lstEmrCoverTypeId = DataConfig.Select(o => o.EMR_COVER_TYPE_ID).ToList();
                                }
                            }
                        }
                    }

                    HIS.Desktop.Plugins.Library.FormMedicalRecord.MediRecordMenuPopupProcessor processor = new Library.FormMedicalRecord.MediRecordMenuPopupProcessor();

                    long EmrCoverTypeId_ = emrInputAdo.EmrCoverTypeId ?? 0;
                    long EmrCoverTypeId_Send;

                    if (EmrCoverTypeId_ <= 0)
                    {
                        EmrCoverTypeId_Send = 0;
                    }
                    else
                    {
                        EmrCoverTypeId_Send = EmrCoverTypeId_;
                    }

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => emrInputAdo), emrInputAdo));

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => emrFormsCodes), emrFormsCodes));
                    processor.FormOpenEmr(EmrCoverTypeId_Send, emrInputAdo, string.Join(",", emrFormsCodes));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //IsActionButtonPrintBill = false;
                SaveWithGridpatientSelect(TypeButton.SAVE, chkPrint.Checked, false, false, chkSign.Checked, chkPrintDocumentSigned.Checked);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveWithGridpatientSelect(TypeButton type, bool isSaveAndPrint, bool printTH, bool isSaveAndShow, bool isSign = false, bool isPrintDocumentSigned = false)
        {
            try
            {
                //kiểm tra cấu hình 
                if (!string.IsNullOrEmpty(HisConfigCFG.InstructionTimeServiceMustBeGreaterThanStartTimeExam))
                {
                    LoadVServiceReq();
                    if (vServiceReq != null && vServiceReq.START_TIME.HasValue && Inventec.Common.DateTime.Calculation.DifferenceTime(vServiceReq.START_TIME.Value, InstructionTime, Inventec.Common.DateTime.Calculation.UnitDifferenceTime.SECOND) < Int32.Parse(HisConfigCFG.InstructionTimeServiceMustBeGreaterThanStartTimeExam))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Thời gian chỉ định {0} phải cách thời gian bắt đầu khám {1} là {2} giây mới được phép chỉ định", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(InstructionTime), Inventec.Common.DateTime.Convert.TimeNumberToTimeString(vServiceReq.START_TIME.Value), HisConfigCFG.InstructionTimeServiceMustBeGreaterThanStartTimeExam), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                        return;
                    }
                }
                assignMulti = false;
                if (workingAssignServiceADO.OpenFromBedRoomPartial && this.patientSelectProcessor != null && this.ucPatientSelect != null)
                {
                    dicValidIcd = new Dictionary<string, string>();
                    ListMessError = new List<string>();
                    var lstPatientSelect = this.patientSelectProcessor.GetSelectedRows(this.ucPatientSelect);
                    if (lstPatientSelect != null && lstPatientSelect.Count > 1)
                    {
                        icdServicePhacDos = null;
                        assignMulti = true;
                        var actionTmp = actionType;
                        bool isValid = true;
                        List<DataGridAdo> serviceCheckeds__Send = this.DataGridAdo.FindAll(o => o.IsChecked);
                        if (serviceTypeIdRequired != null && serviceTypeIdRequired.Count > 0)
                        {
                            var serviceTypeInGrid = serviceCheckeds__Send.Select(o => new { o.TDL_SERVICE_NAME, o.SERVICE_TYPE_ID, o.TEST_SAMPLE_TYPE_ID }).ToList();
                            var lstServiceName = serviceTypeInGrid.Where(item => serviceTypeIdRequired.Exists(o => o == item.SERVICE_TYPE_ID) && item.TEST_SAMPLE_TYPE_ID <= 0).Select(o => o.TDL_SERVICE_NAME);
                            if (lstServiceName != null && lstServiceName.Count() > 0)
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Dịch vụ {0} bắt buộc chọn Loại mẫu bệnh phẩm xét nghiệm", String.Join(", ", lstServiceName.ToList())), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
                                return;
                            }
                        }
                        isValid = isValid && this.Valid(serviceCheckeds__Send);
                        isValid = isValid && this.CheckIcd(lstPatientSelect);
                        isValid = isValid && this.ValidICD();

                        if (!ValidForSaveGridPatientSelect(lstPatientSelect))
                        {
                            string message = "Các bệnh nhân sau có mã ICD không hợp lệ";
                            message += "<br>";
                            foreach (var item in dicValidIcd)
                            {
                                message += string.Format("Bệnh nhân {0} (mã điều trị: {1})", item.Value, item.Key);
                            }
                            XtraMessageBox.Show(message, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
                            return;
                        }


                        string MessGender = "";
                        string MessAge = "";
                        string MessType = "";
                        #region Valid ICD
                        bool isValidICD = true;
                        if (HisConfigCFG.IsIcdServiceHasRequireCheckPatientBHYT && !this.CheckPatientTypeBHYT(lstPatientSelect))
                        {
                            isValidICD = false;
                        }
                        if (isValidICD)
                        {
                            var icd = lstPatientSelect.Select(o => o.ICD_CODE).ToList();
                            var icdSub = lstPatientSelect.Where(o => !string.IsNullOrEmpty(o.ICD_SUB_CODE)).Select(o => o.ICD_SUB_CODE).ToList();
                            MOS.Filter.HisIcdFilter icdFilter = new HisIcdFilter();
                            icdFilter.ICD_CODEs = icd;
                            var icdData = new BackendAdapter(null).Get<List<HIS_ICD>>("api/HisIcd/Get", ApiConsumer.ApiConsumers.MosConsumer, icdFilter, null);
                            if (icdData != null && icdData.Count > 0)
                            {
                                MOS.Filter.HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
                                icdServiceFilter.ICD_CODE__EXACTs = icd;
                                icdServicePhacDos = new BackendAdapter(null).Get<List<HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumer.ApiConsumers.MosConsumer, icdServiceFilter, null);

                                //isValid = isValid && ValidServiceIcdForIcdSelected(icdServices, serviceCheckeds__Send);

                                isValid = isValid && ValidServiceIcdForServiceSelected(icdData, icdServicePhacDos, serviceCheckeds__Send);

                                if (!isValid && HisConfigCFG.IcdServiceHasCheck == "4")
                                    isValid = false;
                            }
                            else if (HisConfigCFG.IcdServiceHasCheck == "3" && serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0)
                            {
                                MOS.Filter.HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
                                icdServiceFilter.SERVICE_IDs = serviceCheckeds__Send.Select(o => o.SERVICE_ID).Distinct().ToList();
                                icdServicePhacDos = new BackendAdapter(new CommonParam()).Get<List<HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumer.ApiConsumers.MosConsumer, icdServiceFilter, null);

                                if (icdServicePhacDos != null && icdServicePhacDos.Count > 0 && icdData != null && icdData.Count > 0)
                                {
                                    icdServicePhacDos = icdServicePhacDos.Where(o => !icdData.Select(p => p.ICD_CODE).Contains(o.ICD_CODE)).ToList();
                                }
                                if (icdServicePhacDos != null && icdServicePhacDos.Count > 0)
                                {
                                    frmMissingIcd frmWaringConfigIcdService = new frmMissingIcd(icdData, serviceCheckeds__Send, this.currentModule, icdServicePhacDos, getDataFromMissingIcdDelegate);
                                    frmWaringConfigIcdService.ShowDialog();
                                }
                            }
                        }
                        #endregion
                        #region Valid ServiceAllow
                        ValidGenderServiceAllowGridpatientSelect(lstPatientSelect, serviceCheckeds__Send, ref MessGender, ref MessAge, ref MessType);
                        bool IsValidBed = ValidCheckTreatmentTypeBed(serviceCheckeds__Send, ref MessType, lstPatientSelect);
                        if (!string.IsNullOrEmpty(MessGender))
                        {
                            XtraMessageBox.Show(MessGender, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
                            return;
                        }
                        if (!string.IsNullOrEmpty(MessAge))
                        {
                            XtraMessageBox.Show(MessAge, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
                            return;
                        }
                        if (!string.IsNullOrEmpty(MessType))
                        {
                            if ((HisConfigCFG.BedServiceType_NotAllow_For_OutPatient == "1" && MessageBox.Show(MessType + ResourceMessage.KhongPhaiNoiTruChiDinhGiuong, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != System.Windows.Forms.DialogResult.Yes) || (HisConfigCFG.BedServiceType_NotAllow_For_OutPatient == "2" && MessageBox.Show(MessType + ResourceMessage.ChanKhongPhaiNoiTruChiDinhGiuong, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK))
                            {
                                return;
                            }
                            //if (MessageBox.Show(MessType + ResourceMessage._BanCoMuonChiDinhGiuong, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                            //    return;
                        }
                        #endregion
                        #region ValidSereServWithMinDuration
                        List<HIS_SERE_SERV> sereServMinDurations = new List<HIS_SERE_SERV>();
                        foreach (var item in lstPatientSelect)
                        {
                            var dt = getSereServWithMinDuration(serviceCheckeds__Send, item.PATIENT_ID);
                            if (dt != null)
                                sereServMinDurations.AddRange(dt);
                        }
                        if (sereServMinDurations != null && sereServMinDurations.Count > 0)
                        {
                            sereServMinDurations = sereServMinDurations.Distinct().ToList();
                            string sereServMinDurationStr = "";
                            foreach (var item in sereServMinDurations)
                            {
                                sereServMinDurationStr += item.TDL_SERVICE_CODE + " - " + item.TDL_SERVICE_NAME + " - " +
                                   Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(item.TDL_INTRUCTION_TIME) +
                                   " (" + item.TDL_SERVICE_REQ_CODE +
                                   "); ";
                            }

                            if (HisConfigCFG.IsSereServMinDurationAlert == 1)
                            {
                                if (MessageBox.Show(string.Format(ResourceMessage.SereServMinDurationAlert__BanCoMuonChuyenDoiDTTTSangVienPhi, string.Format(ResourceMessage.DichVuCoThoiGianChiDinhNamTrongKhoangThoiGianKhongChoPhep, sereServMinDurationStr)), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    foreach (var sv in serviceCheckeds__Send)
                                    {
                                        //Thực hiện tự động chuyển đổi đối tượng sang viện phí                     
                                        if (sereServMinDurations.Any(o => o.SERVICE_ID == sv.SERVICE_ID))
                                        {
                                            sv.PATIENT_TYPE_ID = HisConfigCFG.PatientTypeId__VP;
                                        }
                                    }
                                }
                                else
                                {
                                    return;
                                }
                            }
                            else if (HisConfigCFG.IsSereServMinDurationAlert == 2)
                            {
                                DialogResult result = MessageBox.Show(string.Format(ResourceMessage.DichVuCoThoiGianChiDinhNamTrongKhoangThoiGianKhongChoPhep, sereServMinDurationStr), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo);
                                if (result != DialogResult.Yes)
                                {
                                    return;
                                }
                            }
                            else
                            {
                                if (HisConfigCFG.IsSereServMinDurationAlert == 0 || (HisConfigCFG.IsSereServMinDurationAlert != 1 && HisConfigCFG.IsSereServMinDurationAlert != 2))
                                {
                                    DialogResult result = MessageBox.Show(string.Format(ResourceMessage.DichVuCoThoiGianChiDinhNamTrongKhoangThoiGianKhongChoPhep, sereServMinDurationStr), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo);
                                    if (result != DialogResult.Yes)
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                        #endregion
                        isValid = isValid && ValidSereServWithCondition(serviceCheckeds__Send);
                        isValid = isValid && CheckMaxPatientbyDayOption(serviceCheckeds__Send);
                        List<string> lstIcd = new List<string>();
                        if (!string.IsNullOrEmpty(txtIcdCode.Text))
                        {
                            var arrIcdCode = txtIcdCode.Text.Trim().Split(';');
                            foreach (var item in arrIcdCode)
                            {
                                if (!string.IsNullOrEmpty(item))
                                    lstIcd.Add(item);
                            }
                        }
                        List<string> lstSubIcd = new List<string>();
                        if (!string.IsNullOrEmpty(txtIcdSubCode.Text))
                        {
                            var arrIcdCode = txtIcdSubCode.Text.Trim().Split(';');
                            foreach (var item in arrIcdCode)
                            {
                                if (!string.IsNullOrEmpty(item))
                                    lstSubIcd.Add(item);
                            }
                        }
                        isValid = isValid && checkContraindicated(lstIcd, lstSubIcd, icdServicePhacDos, serviceCheckeds__Send);
                        isValid = isValid && ValidSereServWithOtherPaySource(serviceCheckeds__Send);
                        IsTreatmentInBedRoom = true;
                        isValid = isValid && ValidSereServWithBed(serviceCheckeds__Send);
                        //foreach (var item in lstPatientSelect)
                        //{
                        //    //ValidConsultationReqiured(serviceCheckeds__Send, item.TREATMENT_ID);
                        //    isValid = isValid && CheckMaxAmount(serviceCheckeds__Send, new List<long>() { item.TREATMENT_ID });
                        //}
                        if (isValid)
                        {
                            ChangeLockButtonWhileProcess(false);

                            foreach (var item in lstPatientSelect)
                            {
                                AssignServiceSDO serviceReqSDO = new AssignServiceSDO();
                                serviceReqSDO.ServiceReqDetails = new List<ServiceReqDetailSDO>();
                                bool isDupicate = false;
                                this.ProcessServiceReqSDO(serviceReqSDO, serviceCheckeds__Send, ref isDupicate, item.TREATMENT_ID, false);
                                //foreach (var detail in serviceReqSDO.ServiceReqDetails)
                                //{
                                //    var matchedService = serviceCheckeds__Send.FirstOrDefault(s => s.SERVICE_ID == detail.ServiceId);
                                //    if (matchedService != null)
                                //    {
                                //        detail.MultipleExecute = matchedService.NumberOfTimes;
                                //    }
                                //}
                                serviceReqSDO.IcdCode = item.ICD_CODE;
                                serviceReqSDO.IcdName = item.ICD_NAME;
                                serviceReqSDO.IcdCauseCode = item.ICD_CAUSE_CODE;
                                serviceReqSDO.IcdCauseName = item.ICD_CAUSE_NAME;
                                serviceReqSDO.IcdSubCode = item.ICD_SUB_CODE;
                                serviceReqSDO.IcdText = item.ICD_TEXT;
                                currentHisPatientTypeAlter.PATIENT_TYPE_ID = item.TDL_PATIENT_TYPE_ID ?? 0;
                                currentHisPatientTypeAlter.TREATMENT_TYPE_ID = item.TDL_TREATMENT_TYPE_ID ?? 0;
                                if (this.ServiceAttachForServicePrimary(ref serviceReqSDO, this.currentHisPatientTypeAlter.PATIENT_TYPE_ID))
                                {
                                    this.SaveServiceReqCombo(serviceReqSDO, isSaveAndPrint, printTH, isSaveAndShow, isSign, isPrintDocumentSigned, true, item.TDL_PATIENT_NAME, item.TREATMENT_CODE);
                                    this.actionType = actionTmp;
                                }
                            }
                            if (ListMessError != null && ListMessError.Count > 0)
                            {
                                string mess = "Các bệnh nhân sau chỉ định thất bại. \r\n";
                                mess += string.Join("\r\n", ListMessError);
                                DevExpress.XtraEditors.XtraMessageBox.Show(mess, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
                                this.ChangeLockButtonWhileProcess(true);
                                return;
                            }
                            else
                            {

                                MessageManager.Show(this, new CommonParam(), true);
                                this.actionType = GlobalVariables.ActionEdit;
                            }

                            if (isSaveAndPrint)
                            {
                                long isClosedForm = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY_HIS_DESKTOP_ASSIGN_SERVICE_CLOSED_FORM_AFTER_PRINT);
                                if (isClosedForm == 1)
                                {
                                    this.Dispose();
                                    this.Close();
                                }
                            }
                            this.ChangeLockButtonWhileProcess(true);
                        }

                    }
                }

                if (!assignMulti)
                {
                    switch (type)
                    {
                        case TypeButton.SAVE:
                            LogTheadInSessionInfo(() => ProcessSaveData(chkPrint.Checked, false, false, chkSign.Checked, chkPrintDocumentSigned.Checked), "SaveAssignServiceDefault");
                            break;
                        case TypeButton.SAVE_AND_PRINT:
                            LogTheadInSessionInfo(() => ProcessSaveData(true, false, false), "SaveAndPrintAssignServiceDefault");
                            break;
                        case TypeButton.EDIT:
                            LogTheadInSessionInfo(() => ProcessSaveData(chkPrint.Checked, false, false, chkSign.Checked, chkPrintDocumentSigned.Checked), "EditAssignServiceDefault");
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                this.ChangeLockButtonWhileProcess(true);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        V_HIS_SERVICE_REQ vServiceReq;
        private void LoadVServiceReq()
        {
            try
            {
                if (serviceReqParentId == null)
                    return;
                CommonParam param = new CommonParam();
                HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                filter.ID = serviceReqParentId;
                vServiceReq = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool Valid(List<DataGridAdo> serviceCheckeds__Send)
        {
            CommonParam param = new CommonParam();
            bool valid = true;
            try
            {
                string warning = "";
                this.txtIcdCode.ErrorText = "";
                this.dxValidationProviderControl.RemoveControlError(txtIcdCode);

                this.positionHandleControl = -1;
                valid = (this.dxValidationProviderControl.Validate()) && valid;
                Inventec.Common.Logging.LogSystem.Debug("Valid1:" + valid);
                valid = valid && this.CheckValidDataInGridService(param, serviceCheckeds__Send);
                Inventec.Common.Logging.LogSystem.Debug("Valid2:" + valid);
                if (!valid)
                {
                    if (this.ModuleControls == null || this.ModuleControls.Count == 0)
                    {
                        ModuleControlProcess controlProcess = new ModuleControlProcess(true);
                        this.ModuleControls = controlProcess.GetControls(this);
                    }

                    GetMessageErrorControlInvalidProcess getMessageErrorControlInvalidProcess = new Utility.GetMessageErrorControlInvalidProcess();
                    getMessageErrorControlInvalidProcess.Run(this, this.dxValidationProviderControl, this.ModuleControls, param);

                    warning = param.GetMessage();
                }

                if (!String.IsNullOrEmpty(warning))
                {
                    MessageBox.Show(warning, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                string WaringContinued = "";
                foreach (var item in serviceCheckeds__Send)
                {
                    if (item.ErrorTypeAmount == ErrorType.Warning)
                    {
                        WaringContinued += item.TDL_SERVICE_NAME + " " + item.ErrorMessageAmount + "; ";
                    }
                    if (item.ErrorTypeIsAssignDay == ErrorType.Warning)
                    {
                        WaringContinued += item.TDL_SERVICE_NAME + " " + item.ErrorMessageIsAssignDay + "; ";
                    }
                    if (item.ErrorTypePatientTypeId == ErrorType.Warning)
                    {
                        WaringContinued += item.TDL_SERVICE_NAME + " " + item.ErrorMessagePatientTypeId + "; ";
                    }
                }

                if (!String.IsNullOrEmpty(WaringContinued))
                {
                    WaringContinued += "\n" + ResourceMessage.BanCoMuonTiepTuc;
                    if (MessageBox.Show(WaringContinued, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                    {
                        valid = false;
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug("Chi dinh dich vụ -> luu: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => valid), valid) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => warning), warning));
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return valid;
        }

        private bool CheckIcd(List<V_HIS_TREATMENT_BED_ROOM> lst)
        {
            bool valid = true;
            try
            {
                string messErr = null;
                foreach (var item in lst)
                {
                    if (HisConfigCFG.CheckIcdWhenSave == "1" || HisConfigCFG.CheckIcdWhenSave == "2")
                    {
                        currentTreatment = GetTreatment(item.TREATMENT_ID);
                        InitCheckIcdManager();
                        //viec 178289 thinhdt1 them bien vao thu vien khi luu
                        if (!checkIcdManager.ProcessCheckIcd(item.ICD_CODE, item.ICD_SUB_CODE, ref messErr, HisConfigCFG.CheckIcdWhenSave == "1" || HisConfigCFG.CheckIcdWhenSave == "2", true))
                        {
                            if (HisConfigCFG.CheckIcdWhenSave == "1")
                            {
                                if (DevExpress.XtraEditors.XtraMessageBox.Show((!string.IsNullOrEmpty(item.TREATMENT_CODE) ? item.TREATMENT_CODE + ": " : null) + messErr + ". Bạn có muốn tiếp tục?",
                             HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                             MessageBoxButtons.YesNo) == DialogResult.No) valid = false;
                            }
                            else
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show((!string.IsNullOrEmpty(item.TREATMENT_CODE) ? item.TREATMENT_CODE + ": " : null) + messErr,
                             HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                             MessageBoxButtons.OK);
                                valid = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private void InitCheckIcdManager()
        {
            try
            {
                checkIcdManager = new CheckIcdManager(DlgIcdSubCode, currentTreatment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DlgIcdSubCode(string icdCodes, string icdNames)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("DlgIcdSubCode.1");
                this.isNotProcessWhileChangedTextSubIcd = true;
                ProcessIcdSub(icdCodes, icdNames);
                this.isNotProcessWhileChangedTextSubIcd = false;
                Inventec.Common.Logging.LogSystem.Debug("DlgIcdSubCode.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessIcdSub(string icdCodes, string icdNames)
        {
            try
            {
                var lstIcdCode = icdCodes.Split(IcdUtil.seperator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                var lstIcdName = icdNames.Split(IcdUtil.seperator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                var lstIcdCodeScreen = txtIcdSubCode.Text.Trim().Split(IcdUtil.seperator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                lstIcdCodeScreen.AddRange(lstIcdCode);
                lstIcdCodeScreen = lstIcdCodeScreen.Distinct().ToList();
                string icdCode = string.Join(";", lstIcdCodeScreen);

                var lstIcdNameScreen = txtIcdText.Text.Trim().Split(IcdUtil.seperator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                lstIcdNameScreen.AddRange(lstIcdName);
                lstIcdNameScreen = lstIcdNameScreen.Distinct().ToList();
                string icdName = string.Join(";", lstIcdNameScreen);
                if (!string.IsNullOrEmpty(icdCode))
                {
                    txtIcdSubCode.Text = icdCode;
                }
                else
                {
                    txtIcdSubCode.Text = "";
                }
                if (!string.IsNullOrEmpty(icdName))
                {
                    txtIcdText.Text = icdName;
                }
                else
                {
                    txtIcdText.Text = "";
                }
                ReloadIcdSubContainerByCodeChanged();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReloadIcdSubContainerByCodeChanged()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ReloadIcdSubContainerByCodeChanged.1");
                string[] codes = this.txtIcdSubCode.Text.Split(IcdUtil.seperator.ToCharArray());
                this.icdSubcodeAdoChecks = (from m in this.currentIcds.Where(o => o.IS_TRADITIONAL != 1).ToList() select new ADO.IcdADO(m, codes)).ToList();
                customGridControlSubIcdName.DataSource = null;
                customGridControlSubIcdName.DataSource = this.icdSubcodeAdoChecks;
                Inventec.Common.Logging.LogSystem.Debug("ReloadIcdSubContainerByCodeChanged.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ValidICD()
        {
            bool isValid = true;
            try
            {
                int icd_code = txtIcdCode.Text.Length;
                Inventec.Common.Logging.LogSystem.Debug("Do dai icd code: " + icd_code);
                int icd_name = Inventec.Common.String.CountVi.Count(cboIcds.Text) ?? 0;
                Inventec.Common.Logging.LogSystem.Debug("Do dai icd name: " + icd_name);
                int icd_sub_code = txtIcdSubCode.Text.Length;
                Inventec.Common.Logging.LogSystem.Debug("Do dai icd sub code: " + icd_sub_code);
                int icd_text = Inventec.Common.String.CountVi.Count(txtIcdText.Text) ?? 0;
                Inventec.Common.Logging.LogSystem.Debug("Do dai icd sub name: " + icd_text);
                int icd_yhct_len = 0;
                int icd_yhct_sub_len = 0;
                if (this.ucIcdYhct != null)
                {
                    var icdValue = this.icdYhctProcessor.GetValue(ucIcdYhct);
                    if (icdValue != null && icdValue is HIS.UC.Icd.ADO.IcdInputADO)
                    {
                        var rs = ((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                        if (rs != null) icd_yhct_len = rs.ToString().Length;
                        Inventec.Common.Logging.LogSystem.Debug("Do dai icd yhct code: " + icd_yhct_len);
                    }

                }
                if (this.ucSecondaryIcdYhct != null)
                {
                    var subIcd = this.subIcdYhctProcessor.GetValue(ucSecondaryIcdYhct);
                    if (subIcd != null)//&& subIcd is SecondaryIcdDataADO
                    {
                        var rs = ((HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                        if (rs != null) icd_yhct_sub_len = rs.ToString().Length;
                        Inventec.Common.Logging.LogSystem.Debug("Do dai icd yhct sub code: " + icd_yhct_sub_len);
                    }
                }

                string errror_string = "";
                if (icd_code + icd_sub_code > 100)
                {
                    errror_string = "Mã chẩn đoán phụ nhập quá 100 ký tự";
                }
                else if (icd_name + icd_text > 1500)
                {
                    errror_string = "Tên chẩn đoán phụ nhập quá 1500 ký tự";
                }
                else if (icd_yhct_len + icd_yhct_sub_len > 255)
                {
                    errror_string = "Mã chẩn đoán YHCT phụ nhập quá 255 ký tự";
                }
                if (!string.IsNullOrEmpty(errror_string))
                {
                    MessageBox.Show(this, errror_string, "Thông báo", MessageBoxButtons.OK);
                    isValid = false;
                }


            }
            catch (Exception ex)
            {
                isValid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return isValid;
        }

        private bool ValidForSaveGridPatientSelect(List<V_HIS_TREATMENT_BED_ROOM> lstTreatmentBedRoom)
        {
            bool valid = true;
            List<bool> lstValid = new List<bool>();
            try
            {
                foreach (var item in lstTreatmentBedRoom)
                {
                    if (!String.IsNullOrEmpty(item.ICD_CODE))
                    {
                        var listData = this.currentIcds.Where(o => o.ICD_CODE.Contains(item.ICD_CODE)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == item.ICD_CODE).ToList() : listData) : null;
                        if (result == null || result.Count <= 0)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("CASE 1");
                            if (!dicValidIcd.ContainsKey(item.TREATMENT_CODE))
                                dicValidIcd[item.TREATMENT_CODE] = item.TDL_PATIENT_NAME;
                            lstValid.Add(false);
                        }
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Warn("CASE 2");
                        if (!dicValidIcd.ContainsKey(item.TREATMENT_CODE))
                            dicValidIcd[item.TREATMENT_CODE] = item.TDL_PATIENT_NAME;
                        lstValid.Add(false);
                    }
                }
                this.positionHandleControl = -1;
                valid = lstValid != null && lstValid.Count > 0 ? (lstValid.IndexOf(false) > -1 ? false : true) : true;
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private bool CheckPatientTypeBHYT(List<V_HIS_TREATMENT_BED_ROOM> lst)
        {
            bool valid = false;
            try
            {

                foreach (var item in lst)
                {
                    if (item.TDL_PATIENT_TYPE_ID.HasValue && item.TDL_PATIENT_TYPE_ID.Value == HisConfigCFG.PatientTypeId__BHYT)
                    {
                        valid = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private bool ValidServiceIcdForServiceSelected(List<HIS_ICD> icdFromUc, List<HIS_ICD_SERVICE> icdServices, List<DataGridAdo> serviceCheckeds__Send)
        {
            bool valid = true;
            try
            {
                isYes = false;
                string serviceErrStr = "";
                //string icdServiceCFG = HisConfigCFG.IcdServiceHasCheck;
                List<DataGridAdo> sereServAdoResult = new List<DataGridAdo>();
                bool checkServiceIcd = this.CheckIcdServiceForService(icdServices, ref serviceErrStr, serviceCheckeds__Send, ref sereServAdoResult);
                if (HisConfigCFG.IcdServiceHasCheck == "1" && !checkServiceIcd)
                {
                    Inventec.Common.Logging.LogSystem.Debug(ResourceMessage.DichVuChuaDuocCauHinhICDDichVu + serviceErrStr);
                    MessageManager.Show(String.Format(ResourceMessage.DichVuChuaDuocCauHinhICDDichVu, serviceErrStr));
                    valid = false;
                }
                else if (HisConfigCFG.IcdServiceHasCheck == "2" && !checkServiceIcd)
                {
                    frmWaringConfigIcdService frmWaringConfigIcdService = new frmWaringConfigIcdService(icdFromUc, sereServAdoResult, this.currentModule, getDataFromOtherFormDelegate);
                    frmWaringConfigIcdService.ShowDialog();
                    if (!isYes)
                        valid = false;
                }
                else if ((HisConfigCFG.IcdServiceHasCheck == "4" || HisConfigCFG.IcdServiceHasCheck == "3" || HisConfigCFG.IcdServiceHasCheck == "5") && !checkServiceIcd
                    && sereServAdoResult != null && sereServAdoResult.Count > 0)
                {
                    MOS.Filter.HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
                    icdServiceFilter.SERVICE_IDs = sereServAdoResult.Select(o => o.SERVICE_ID).Distinct().ToList();
                    List<HIS_ICD_SERVICE> icdServiceByServices = new BackendAdapter(new CommonParam()).Get<List<HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumer.ApiConsumers.MosConsumer, icdServiceFilter, null);
                    if (HisConfigCFG.IcdServiceHasCheck == "4")
                        icdServiceByServices = icdServiceByServices.Where(o => o.IS_CONTRAINDICATION != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                    else if (HisConfigCFG.IcdServiceHasCheck == "5")
                        icdServiceByServices = icdServiceByServices.Where(o => o.IS_CONTRAINDICATION != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_WARNING != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                    if (icdServiceByServices != null && icdServiceByServices.Count > 0 && icdFromUc != null && icdFromUc.Count > 0)
                    {
                        icdServiceByServices = icdServiceByServices.Where(o => !icdFromUc.Select(p => p.ICD_CODE).Contains(o.ICD_CODE)).ToList();
                    }

                    if (icdServiceByServices != null && icdServiceByServices.Count > 0)
                    {
                        frmMissingIcd frmWaringConfigIcdService = new frmMissingIcd(icdFromUc, sereServAdoResult, this.currentModule, icdServiceByServices, getDataFromMissingIcdDelegate, HisConfigCFG.IcdServiceHasCheck == "5", SkipIcd);
                        frmWaringConfigIcdService.ShowDialog();
                        if (isYes && HisConfigCFG.IcdServiceHasCheck == "5")
                            valid = true;
                        else
                            valid = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        private bool CheckIcdServiceForService(List<HIS_ICD_SERVICE> icdServices, ref string messageErr, List<DataGridAdo> serviceCheckeds_Send, ref List<DataGridAdo> serviceNotConfigResult)
        {
            bool valid = true;
            try
            {
                serviceNotConfigResult = new List<DataGridAdo>();
                // kiểm tra dịch vụ theo cấu hình ICD - Dịch vụ                             

                List<long> serviceIdChecks = serviceCheckeds_Send.Select(o => o.SERVICE_ID).Distinct().ToList();

                if (HisConfigCFG.IcdServiceHasRequireCheck || (!HisConfigCFG.IcdServiceHasRequireCheck && icdServices != null && icdServices.Count > 0) && serviceCheckeds_Send != null && serviceCheckeds_Send.Count > 0)
                {
                    var icdServiceChecks = icdServices.Where(o => serviceIdChecks.Contains(o.SERVICE_ID ?? -1)).ToList();
                    foreach (var item in serviceCheckeds_Send)
                    {
                        if (item.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G || item.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC)
                            continue;

                        var checkIcdService = icdServiceChecks.FirstOrDefault(o => o.SERVICE_ID == item.SERVICE_ID);
                        if (checkIcdService == null)
                        {
                            valid = false;
                            serviceNotConfigResult.Add(item);
                            messageErr += item.TDL_SERVICE_CODE + " - " + item.TDL_SERVICE_NAME + "; ";
                            Inventec.Common.Logging.LogSystem.Debug("Dich vu (" + item.TDL_SERVICE_CODE + "-" + item.TDL_SERVICE_NAME + " chua duoc cau hinh ICD - Dich vu.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                valid = true;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        public void getDataFromOtherFormDelegate(object data)
        {
            try
            {
                if (data != null && data is bool)
                {
                    isYes = (bool)data;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void getDataFromMissingIcdDelegate(object data)
        {
            List<ADO.MissingIcdADO> missingIcdADOList = new List<ADO.MissingIcdADO>();
            try
            {
                isNotProcessWhileChangedTextSubIcd = true;
                if (data != null && data is List<ADO.MissingIcdADO>)
                {
                    missingIcdADOList = (List<ADO.MissingIcdADO>)data;
                    if (missingIcdADOList != null && missingIcdADOList.Count > 0)
                    {
                        this.isYes = true;
                        var icdMainCheck = missingIcdADOList.FirstOrDefault(o => o.ICD_MAIN_CHECK);

                        if (icdMainCheck != null)
                        {
                            var icdMainData = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).FirstOrDefault(o => o.ICD_CODE == icdMainCheck.ICD_CODE);
                            if (icdMainData != null)
                            {
                                cboIcds.EditValue = icdMainData.ID;
                                txtIcdCode.Text = icdMainData.ICD_CODE;
                                txtIcdMainText.Text = icdMainData.ICD_NAME;
                            }

                        }

                        var icdCauses = missingIcdADOList.Where(o => o.ICD_CAUSE_CHECK).ToList();
                        if (icdCauses != null && icdCauses.Count > 0)
                        {
                            var icdCauseDatas = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Where(o => icdCauses.Select(p => p.ICD_CODE).Contains(o.ICD_CODE)).ToList();

                            if (icdCauseDatas != null && icdCauseDatas.Count > 0)
                            {
                                icdCauseDatas = icdCauseDatas.GroupBy(o => o.ICD_CODE)
                                    .Select(p => p.FirstOrDefault())
                                    .OrderBy(k => k.ICD_CODE).ToList();

                                string icdCausesCodestr = String.Join(";", icdCauseDatas.Select(o => o.ICD_CODE).ToList());
                                string icdCausesstr = String.Join(";", icdCauseDatas.Select(o => o.ICD_NAME).ToList());
                                txtIcdSubCode.Text += !string.IsNullOrEmpty(txtIcdSubCode.Text) ? ";" + icdCausesCodestr : icdCausesCodestr;
                                txtIcdText.Text += !string.IsNullOrEmpty(txtIcdText.Text) ? ";" + icdCausesstr : icdCausesstr;
                            }
                        }
                    }
                    string[] codes = this.txtIcdSubCode.Text.Split(IcdUtil.seperator.ToCharArray());
                    this.icdSubcodeAdoChecks = (from m in this.currentIcds select new ADO.IcdADO(m, codes)).ToList();

                    customGridViewSubIcdName.BeginUpdate();
                    customGridViewSubIcdName.GridControl.DataSource = this.icdSubcodeAdoChecks;
                    customGridViewSubIcdName.EndUpdate();

                    if (HisConfigCFG.IcdServiceHasCheck == "3" || HisConfigCFG.IcdServiceHasCheck == "4")
                    {
                        List<HIS_ICD> icdFromUc = GetIcdCodeListFromUcIcd();
                        MOS.Filter.HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
                        icdServiceFilter.ICD_CODE__EXACTs = icdFromUc.Select(o => o.ICD_CODE).Distinct().ToList();
                        icdServicePhacDos = new BackendAdapter(null).Get<List<HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumer.ApiConsumers.MosConsumer, icdServiceFilter, null);
                        if (HisConfigCFG.IcdServiceHasCheck == "4")
                        {
                            if (icdServicePhacDos != null && icdServicePhacDos.Count > 0)
                                ProcessChoiceIcdPhacDo(icdServicePhacDos);
                            else
                            {
                                this.ResetDefaultGridData();
                            }
                        }
                    }

                }
                isNotProcessWhileChangedTextSubIcd = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<HIS_ICD> GetIcdCodeListFromUcIcd()
        {
            List<HIS_ICD> icdList = new List<HIS_ICD>();
            try
            {
                var icdValue = UcIcdGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                if (icdValue != null && !string.IsNullOrEmpty(icdValue.ICD_CODE))
                {
                    HIS_ICD icdMain = new HIS_ICD();

                    var icd = this.currentIcds.Where(o => o.ICD_CODE == icdValue.ICD_CODE).FirstOrDefault();
                    if (icd != null)
                    {
                        icdMain.ID = icd != null ? icd.ID : 0;
                        icdMain.ICD_NAME = icd != null ? icd.ICD_NAME : "";
                        icdMain.ICD_CODE = icd != null ? icd.ICD_CODE : "";
                        icdList.Add(icdMain);
                    }
                }

                var subIcd = UcSecondaryIcdGetValue() as HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO;
                if (subIcd != null)
                {
                    string icd_sub_code = subIcd.ICD_SUB_CODE;
                    if (!string.IsNullOrEmpty(icd_sub_code))
                    {
                        String[] icdCodes = icd_sub_code.Split(';');
                        foreach (var item in icdCodes)
                        {
                            var icd = this.currentIcds.Where(o => o.IS_TRADITIONAL != 1).ToList().FirstOrDefault(o => o.ICD_CODE == item);
                            if (icd != null)
                            {
                                HIS_ICD icdSub = new HIS_ICD();
                                icdSub.ID = icd != null ? icd.ID : 0;
                                icdSub.ICD_NAME = icd != null ? icd.ICD_NAME : "";
                                icdSub.ICD_CODE = icd != null ? icd.ICD_CODE : "";
                                icdList.Add(icdSub);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                icdList = new List<HIS_ICD>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return icdList;
        }

        private void SkipIcd(bool obj)
        {
            try
            {
                isYes = obj;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckValidDataInGridService(CommonParam param, List<DataGridAdo> serviceCheckeds__Send)
        {
            bool valid = true;
            try
            {
                if (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0)
                {
                    foreach (var item in serviceCheckeds__Send)
                    {
                        string messageErr = "";
                        messageErr = String.Format(ResourceMessage.CanhBaoDichVu, item.TDL_SERVICE_NAME);

                        if (item.PATIENT_TYPE_ID <= 0)
                        {
                            valid = false;
                            messageErr += ResourceMessage.KhongCoDoiTuongThanhToan;
                            Inventec.Common.Logging.LogSystem.Debug("Dich vu (" + item.TDL_SERVICE_CODE + "-" + item.TDL_SERVICE_NAME + " khong co doi tuong thanh toan.");
                        }
                        if (item.AMOUNT <= 0)
                        {
                            valid = false;
                            messageErr += ResourceMessage.KhongNhapSoLuong;
                            Inventec.Common.Logging.LogSystem.Debug("Dich vu (" + item.TDL_SERVICE_CODE + "-" + item.TDL_SERVICE_NAME + " khong co so luong.");
                        }

                        if (!valid)
                        {
                            param.Messages.Add(messageErr + ";");
                        }
                    }
                }
                else
                {
                    HIS.Desktop.LibraryMessage.MessageUtil.SetParam(param, HIS.Desktop.LibraryMessage.Message.Enum.ThongBaoDuLieuTrong);
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private void ValidGenderServiceAllowGridpatientSelect(List<V_HIS_TREATMENT_BED_ROOM> lst, List<DataGridAdo> serviceCheckeds__Send, ref string MessageGender, ref string MessageAge, ref string MessageType)
        {
            try
            {

                foreach (var item in lst.GroupBy(o => o.TREATMENT_ID))
                {
                    var genderCheck = GetDiffGender(serviceCheckeds__Send, item.First().TDL_PATIENT_GENDER_ID);
                    if (genderCheck != null && genderCheck.Count() > 0)
                    {
                        string gender = genderCheck.FirstOrDefault().GENDER_ID == 1 ? "nữ" : "nam";
                        MessageGender += "Dịch vụ không chỉ định cho giới tính " + gender + ": " + String.Join("; ", genderCheck.Select(o => o.TDL_SERVICE_NAME).ToArray()) + "\r\n";
                    }

                    // check tuổi từ - đến (DVKT)
                    var ageDate = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentHisTreatment.TDL_PATIENT_DOB);
                    //int age = DateTime.Now.Year - int.Parse(this.currentHisTreatment.TDL_PATIENT_DOB.ToString().Substring(0, 4));
                    TimeSpan timeSpan2 = System.DateTime.Now.Date - ageDate.Value.Date;
                    long ticks = timeSpan2.Ticks;
                    System.DateTime dateTime = new System.DateTime(ticks);
                    int ageMonth = (dateTime.Year - 1) * 12 + dateTime.Month - 1;
                    //int ageMonth = (DateTime.Now - (ageDate ?? DateTime.Now)).Days / 30;
                    Inventec.Common.Logging.LogSystem.Debug("age: " + ageMonth);

                    var checkAge = serviceCheckeds__Send.Where(o => (o.AGE_FROM.HasValue && o.AGE_FROM > ageMonth) || (o.AGE_TO.HasValue && o.AGE_TO < ageMonth));

                    if (checkAge != null && checkAge.Count() > 0)
                    {
                        MessageAge += "Độ tuổi của bệnh nhân " + item.First().TDL_PATIENT_NAME + " có mã điều trị " + item.First().TREATMENT_CODE + " không phù hợp với điều kiện của dịch vụ " + String.Join("; ", checkAge.Select(o => o.TDL_SERVICE_NAME).ToArray()) + "\r\n";
                    }

                    // check dịch vụ giường với diện điều trị là khám, điều trị ngoại trú, điều trị ban ngày
                    //if (item.First().TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM || item.First().TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY || item.First().TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
                    //{
                    //    var treatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == item.First().TDL_TREATMENT_TYPE_ID);
                    //    var dichVuGiuong = serviceCheckeds__Send.Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G).ToList();
                    //    if (dichVuGiuong != null && dichVuGiuong.Count() > 0 && treatmentType != null)
                    //    {
                    //        MessageType += "Diện điều trị của bệnh nhân " + item.First().TDL_PATIENT_NAME + " có mã điều trị " + item.First().TREATMENT_CODE + " là " + treatmentType.TREATMENT_TYPE_NAME;                          
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<DataGridAdo> GetDiffGender(List<DataGridAdo> serviceCheckeds__Send, long patientGenderId)
        {
            List<DataGridAdo> result = new List<DataGridAdo>();
            try
            {
                foreach (var item in serviceCheckeds__Send)
                {
                    if (item.GENDER_ID != null && patientGenderId != item.GENDER_ID)
                    {
                        result.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool ValidCheckTreatmentTypeBed(List<DataGridAdo> serviceCheckeds__Send, ref string MessageType, List<V_HIS_TREATMENT_BED_ROOM> lst = null)
        {
            bool result = true;
            try
            {
                List<DataGridAdo> listBed = serviceCheckeds__Send.Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G).ToList();
                if (listBed != null && listBed.Count > 0 && (HisConfigCFG.BedServiceType_NotAllow_For_OutPatient == "1" || HisConfigCFG.BedServiceType_NotAllow_For_OutPatient == "2"))
                {
                    if (lst != null && lst.Count > 0)
                    {
                        bool resultTemp = true;
                        foreach (var item in lst.GroupBy(o => o.TREATMENT_ID))
                        {
                            if (item.First().TDL_TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                            {
                                MessageType += "Bệnh nhân " + item.First().TDL_PATIENT_NAME + " có mã điều trị " + item.First().TREATMENT_CODE + ".\r\n";
                                result = false;
                                if (!result)
                                    resultTemp = result;
                            }
                        }
                        result = resultTemp;
                    }
                    else if (currentTreatment.TDL_TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                    {
                        if ((HisConfigCFG.BedServiceType_NotAllow_For_OutPatient == "1" && MessageBox.Show(ResourceMessage.KhongPhaiNoiTruChiDinhGiuong, MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != System.Windows.Forms.DialogResult.Yes) || (HisConfigCFG.BedServiceType_NotAllow_For_OutPatient == "2" && MessageBox.Show(ResourceMessage.ChanKhongPhaiNoiTruChiDinhGiuong, MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK))
                        {
                            result = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool ValidSereServWithCondition(List<DataGridAdo> serviceCheckeds__Send)
        {
            bool valid = true;
            try
            {
                if (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0)
                {
                    string sereServConditionStr = "";
                    foreach (var item in serviceCheckeds__Send)
                    {
                        var dataCondition = BranchDataWorker.ServicePatyWithListPatientType(item.SERVICE_ID, new List<long> { item.PATIENT_TYPE_ID });
                        List<V_HIS_SERVICE_PATY> dataSource = new List<V_HIS_SERVICE_PATY>();
                        long instructionTime = this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0 ? this.intructionTimeSelecteds.FirstOrDefault() : 0;
                        long? intructionNumByType = null;
                        List<HIS_SERE_SERV> sameServiceType = this.sereServWithTreatment != null ? this.sereServWithTreatment.Where(o => o.TDL_SERVICE_TYPE_ID == item.SERVICE_TYPE_ID).ToList() : null;
                        List<HIS_SERE_SERV> sameService = this.sereServWithTreatment != null ? this.sereServWithTreatment.Where(o => o.SERVICE_ID == item.SERVICE_ID).ToList() : null;
                        intructionNumByType = sameServiceType != null ? (long)sameServiceType.Count() + 1 : 1;
                        var intructionNum = sameService != null ? (long)sameService.Count() + 1 : 1;
                        foreach (var con in dataCondition)
                        {
                            var dt = MOS.ServicePaty.ServicePatyUtil.GetApplied(new List<V_HIS_SERVICE_PATY>() { con }, item.TDL_EXECUTE_BRANCH_ID, item.TDL_EXECUTE_ROOM_ID, this.requestRoom.ID, this.requestRoom.DEPARTMENT_ID, instructionTime, this.currentHisTreatment.IN_TIME, item.SERVICE_ID, item.PATIENT_TYPE_ID, intructionNum, intructionNumByType, item.PackagePriceId, con.SERVICE_CONDITION_ID, this.currentHisTreatment.TDL_PATIENT_CLASSIFY_ID, null);
                            if (dt != null)
                                dataSource.Add(dt);
                        }
                        dataCondition = dataSource;
                        if (dataCondition != null && dataCondition.Count > 0 && lstConditionService != null && lstConditionService.Count > 0)
                        {
                            dataCondition = dataCondition.Where(o => lstConditionService.Exists(p => p.SERVICE_ID == item.SERVICE_ID && p.ID == o.SERVICE_CONDITION_ID)).ToList();
                        }
                        if (dataCondition != null && dataCondition.Count > 0 && dataCondition.Exists(t => t.SERVICE_CONDITION_ID.HasValue && t.SERVICE_CONDITION_ID > 0) && !dataCondition.Exists(t => t.SERVICE_CONDITION_ID == null || t.SERVICE_CONDITION_ID == 0))
                        {
                            dataCondition = dataCondition.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.SERVICE_CONDITION_ID.HasValue && o.SERVICE_CONDITION_ID > 0 && o.SERVICE_ID == item.SERVICE_ID).ToList();
                            if (dataCondition != null && dataCondition.Count > 0)
                            {
                                List<V_HIS_SERVICE_PATY> dataConditionTmps = new List<V_HIS_SERVICE_PATY>();
                                foreach (var itemCon in dataCondition)
                                {
                                    if (dataConditionTmps.Count == 0 || !dataConditionTmps.Exists(t => t.SERVICE_CONDITION_NAME == itemCon.SERVICE_CONDITION_NAME && t.HEIN_RATIO == itemCon.HEIN_RATIO))
                                    {
                                        dataConditionTmps.Add(itemCon);
                                    }
                                }
                                dataCondition.Clear();
                                dataCondition.AddRange(dataConditionTmps);
                            }
                        }
                        else
                        {
                            dataCondition = null;
                        }
                        if (dataCondition != null && dataCondition.Count > 0 && (item.SERVICE_CONDITION_ID ?? 0) <= 0)
                        {
                            sereServConditionStr += item.TDL_SERVICE_NAME + ",";
                        }
                    }

                    if (!String.IsNullOrEmpty(sereServConditionStr))
                    {
                        sereServConditionStr = sereServConditionStr.TrimEnd(',');
                        MessageBox.Show(string.Format(ResourceMessage.SereServConditionAlert__DVChuaDuocNhapDieuKien, sereServConditionStr), MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                        Inventec.Common.Logging.LogSystem.Warn("ValidSereServWithCondition: valid = false_____" + sereServConditionStr);
                        valid = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        private bool CheckMaxPatientbyDayOption(List<DataGridAdo> serviceCheckeds__Send)
        {
            bool valid = true;
            try
            {
                if (HisConfigCFG.MaxPatientByDay == 1 && serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0)
                {
                    if (this.hisRoomCounters == null || this.hisRoomCounters.Count == 0)
                    {
                        this.hisRoomCounters = GetLCounter1();
                    }


                    var ProcessingRoom = this.hisRoomCounters != null ? this.hisRoomCounters.Where(p => p.MAX_PATIENT_BY_DAY > 0 && p.TOTAL_TODAY_PATIENT >= p.MAX_PATIENT_BY_DAY).ToList() : null;


                    if (ProcessingRoom != null && ProcessingRoom.Count > 0)
                    {
                        var serviceCheckeds__Send__Validmax = serviceCheckeds__Send.Where(k => k.TDL_EXECUTE_ROOM_ID > 0 && ProcessingRoom.Exists(t => t.ROOM_ID == k.TDL_EXECUTE_ROOM_ID)).Select(p => p.TDL_EXECUTE_ROOM_ID).Distinct().ToList();


                        List<string> txt_ = new List<string>();
                        string text;
                        if (serviceCheckeds__Send__Validmax != null && serviceCheckeds__Send__Validmax.Count > 0)
                        {
                            foreach (var item in serviceCheckeds__Send__Validmax)
                            {
                                Convert.ToInt32(ProcessingRoom.Where(p => p.ROOM_ID == item).FirstOrDefault().TOTAL_TODAY_PATIENT);
                                text = ProcessingRoom.Where(p => p.ROOM_ID == item).FirstOrDefault().EXECUTE_ROOM_NAME + ": Số lượng hiện tại :" + Convert.ToInt32(ProcessingRoom.Where(p => p.ROOM_ID == item).FirstOrDefault().TOTAL_TODAY_PATIENT) + ",Số lượng tối đa:" + ProcessingRoom.Where(p => p.ROOM_ID == item).FirstOrDefault().MAX_PATIENT_BY_DAY;
                                txt_.Add(text);
                                txt_.Distinct();
                            }

                            if (MessageBox.Show(string.Join("\r\n", txt_) + "\n\rBạn có muốn tiếp tục không?", Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                            {
                                valid = false;
                            }
                        }
                    }
                }
                GetLCounter1Async();
                return valid;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        private List<L_HIS_ROOM_COUNTER_1> GetLCounter1()
        {
            try
            {
                HisRoomCounterLView1Filter exetuteFilter = new HisRoomCounterLView1Filter();
                exetuteFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                exetuteFilter.BRANCH_ID = WorkPlace.GetBranchId();
                return new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<L_HIS_ROOM_COUNTER_1>>("api/HisRoom/GetCounterLView1", ApiConsumers.MosConsumer, exetuteFilter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private bool checkContraindicated(List<string> icd, List<string> icdSub, List<HIS_ICD_SERVICE> icdServices, List<DataGridAdo> serviceCheckeds__Send)
        {

            var icd_code = icd;
            var icd_sub_code = icdSub;
            bool valid = true;
            try
            {
                string serviceErrStr = "";
                long icdServiceCFG = HisConfigCFG.contraindicated;
                IsActionKey = false;
                if (icdServiceCFG == 1 || icdServiceCFG == 2)
                {
                    List<string> serviceCon = new List<string>();
                    List<string> serviceWar = new List<string>();

                    if (icd_sub_code != null && icd_sub_code.Count() > 0)
                    {
                        icd_code.AddRange(icd_sub_code);
                    }
                    icd_code = icd_code.Distinct().ToList();

                    if (icd_code != null && icd_code.Count > 0)
                    {
                        var is_condi = icdServices.Where(k => k.IS_CONTRAINDICATION == 1 && (icd_code.Contains(k.ICD_CODE)));
                        var is_warning = icdServices.Where(k => k.IS_WARNING == 1 && (icd_code.Contains(k.ICD_CODE)));
                        var serviceCheckeds = serviceCheckeds__Send.Select(p => p.SERVICE_ID);
                        if (icdServiceCFG == 2)
                        {
                            foreach (var item in serviceCheckeds)
                            {
                                string is_condiserid_name = null;
                                string is_warningid_name = null;
                                List<string> is_condiserCode = new List<string>();
                                List<string> is_warningCode = new List<string>();
                                if (is_condi != null && is_condi.Count() > 0)
                                {
                                    is_condiserCode = is_condi.Where(k => k.SERVICE_ID == item).Select(o => o.ICD_CODE).ToList();
                                    is_condiserCode.Distinct();
                                    //foreach (var item_ in is_condiserid)
                                    //{
                                    //    var is_sub_condiserid = icd_code.Where(k => k.ICD_CODE == item_).Select(o => o.ICD_SUB_CODE).ToList();
                                    //    is_sub_condiserid.Distinct();
                                    //     mess_is_sub_condiserid = string.Join(", ", is_sub_condiserid);
                                    //}
                                    is_condiserid_name = dicServices.Values.FirstOrDefault(p => p.ID == item).SERVICE_NAME;
                                }
                                if (is_warning != null && is_warning.Count() > 0)
                                {
                                    is_warningCode = is_warning.Where(k => k.SERVICE_ID == item).Select(o => o.ICD_CODE).ToList();
                                    is_warningCode.Distinct();
                                    //foreach (var item_ in is_condiserid)
                                    //{
                                    //    var is_sub_condiserid = icd_code.Where(k => k.ICD_CODE == item_).Select(o => o.ICD_SUB_CODE).ToList();
                                    //    is_sub_condiserid.Distinct();
                                    //     mess_is_sub_condiserid = string.Join(", ", is_sub_condiserid);
                                    //}
                                    is_warningid_name = dicServices.Values.FirstOrDefault(p => p.ID == item).SERVICE_NAME;
                                }

                                if (is_condiserCode != null && is_condiserCode.Count() > 0)
                                {
                                    string mess = string.Format("{0}: ", is_condiserid_name);
                                    foreach (var i in is_condi.Where(k => k.SERVICE_ID == item).ToList())
                                    {
                                        mess += String.Format("\r\n{0} - {1} - {2}", i.ICD_CODE, i.ICD_NAME, i.CONTRAINDICATION_CONTENT);
                                    }
                                    serviceCon.Add(mess);
                                    serviceCon.Distinct();
                                }

                                if (is_warningCode != null && is_warningCode.Count() > 0)
                                {
                                    string mess = string.Format("{0}: ", is_condiserid_name);
                                    foreach (var i in is_warning.Where(k => k.SERVICE_ID == item).ToList())
                                    {
                                        mess += String.Format("\r\n- {0} - {1} - {2}", i.ICD_CODE, i.ICD_NAME, i.CONTRAINDICATION_CONTENT);
                                    }
                                    serviceWar.Add(mess);
                                    serviceWar.Distinct();
                                }
                            }

                            if (serviceCon != null && serviceCon.Count > 0)
                            {
                                if (MessageBox.Show("Chặn báo chống chỉ định\r\n\r\n" + string.Join("\r\n\r\n", serviceCon), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.OK, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.OK)
                                {
                                    return valid = false;
                                }
                            }
                            if (serviceWar != null && serviceWar.Count > 0)
                            {
                                if (MessageBox.Show("Cảnh báo chống chỉ định\r\n\r\n" + string.Join("\r\n\r\n", serviceWar) + "\n\rBạn có muốn tiếp tục không?", Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                                {
                                    return valid = false;
                                }
                            }
                        }
                        if (icdServiceCFG == 1)
                        {
                            var is_condiKey1 = icdServices != null ? icdServices.Where(o => o.IS_CONTRAINDICATION == 1).ToList() : null;
                            var is_warKey1 = icdServices != null ? icdServices.Where(o => o.IS_WARNING == 1).ToList() : null;
                            if (is_warKey1 != null && is_warKey1.Count() > 0)
                            {
                                MOS.Filter.HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
                                icdServiceFilter.SERVICE_IDs = is_warKey1.Select(o => o.SERVICE_ID ?? 0).ToList();
                                var is_condi_allInService = new BackendAdapter(null).Get<List<HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumer.ApiConsumers.MosConsumer, icdServiceFilter, null);
                                var is_war_allInService = is_condi_allInService != null ? is_condi_allInService.Where(o => o.IS_WARNING == 1).ToList() : null;
                                List<HIS_ICD_SERVICE> chanChongChiDinhWar = is_war_allInService.Where(o => serviceCheckeds.Contains(o.SERVICE_ID ?? -1)).ToList();
                                if (chanChongChiDinhWar != null && chanChongChiDinhWar.Count() > 0)
                                {
                                    FormContraindicated.frmContraindicated form = new FormContraindicated.frmContraindicated(this.currentModule, chanChongChiDinhWar, CheckContinue);
                                    form.ShowDialog();
                                    valid = IsActionKey;
                                    if (!valid)
                                        return valid;
                                }
                            }
                            if (is_condiKey1 != null && is_condiKey1.Count() > 0)
                            {
                                MOS.Filter.HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
                                icdServiceFilter.SERVICE_IDs = is_condiKey1.Select(o => o.SERVICE_ID ?? 0).ToList();
                                var is_condi_allInService = new BackendAdapter(null).Get<List<HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumer.ApiConsumers.MosConsumer, icdServiceFilter, null);
                                is_condi_allInService = is_condi_allInService != null ? is_condi_allInService.Where(o => o.IS_CONTRAINDICATION == 1).ToList() : null;
                                List<HIS_ICD_SERVICE> chanChongChiDinhCon = is_condi_allInService.Where(o => serviceCheckeds.Contains(o.SERVICE_ID ?? -1)).ToList();
                                if (chanChongChiDinhCon != null && chanChongChiDinhCon.Count() > 0)
                                {
                                    FormContraindicated.frmContraindicated form = new FormContraindicated.frmContraindicated(this.currentModule, chanChongChiDinhCon);
                                    form.ShowDialog();
                                    valid = false;
                                }
                            }

                        }
                    }

                }
                return valid;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        private void CheckContinue(bool obj)
        {
            IsActionKey = obj;
        }

        private bool ValidSereServWithOtherPaySource(List<DataGridAdo> serviceCheckeds__Send)
        {
            bool valid = true;
            try
            {
                if (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0)
                {
                    string sereServOtherpaysourceStr = "";
                    foreach (var item in serviceCheckeds__Send)
                    {
                        var workingPatientType = currentPatientTypes.Where(t => t.ID == item.PATIENT_TYPE_ID).FirstOrDefault();
                        if (workingPatientType != null && !String.IsNullOrEmpty(workingPatientType.OTHER_PAY_SOURCE_IDS) && (item.OTHER_PAY_SOURCE_ID ?? 0) <= 0)
                        {
                            sereServOtherpaysourceStr += item.TDL_SERVICE_NAME + ",";
                        }
                    }

                    if (!String.IsNullOrEmpty(sereServOtherpaysourceStr))
                    {
                        sereServOtherpaysourceStr = sereServOtherpaysourceStr.TrimEnd(',');
                        MessageBox.Show(string.Format(ResourceMessage.SereServOtherpaySourceAlert__DVChuaDuocNhapNguonChiTra, sereServOtherpaysourceStr), MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                        Inventec.Common.Logging.LogSystem.Warn("ValidSereServWithOtherPaySource: valid = false_____" + sereServOtherpaysourceStr);
                        valid = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        private bool ValidSereServWithBed(List<DataGridAdo> serviceCheckeds__Send)
        {
            bool result = true;
            try
            {
                List<DataGridAdo> listBed = serviceCheckeds__Send.Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G).ToList();
                if (!HisConfigCFG.AssignBedServiceWithBedInfo)
                    return result;
                if (this.IsTreatmentInBedRoom)
                {
                    if (listBed != null && listBed.Count > 0)
                    {
                        List<DataGridAdo> listBedMissInfo = listBed.Where(o => !o.BedId.HasValue).ToList();
                        if ((listBedMissInfo == null || listBedMissInfo.Count <= 0) && intructionTimeSelecteds.Count > 1)
                        {
                            //this.txtInstructionTime.Focus();
                            //this.txtInstructionTime.SelectAll();
                            MessageBox.Show(ResourceMessage.DichVuCoThongTinGiuongChiDuocChiDinhTrongNgay, MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                            result = false;
                        }
                        else if (listBedMissInfo != null && listBedMissInfo.Count > 0 && MessageBox.Show(string.Format(ResourceMessage.DichVuThieuThongTinGiuong, string.Join(",", listBedMissInfo.Select(s => s.TDL_SERVICE_CODE))), MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                        {
                            result = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ChangeLockButtonWhileProcess(bool isLock)
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionEdit)
                    return;

                this.btnSave.Enabled = isCheckAssignServiceSimultaneityOption ? false : isLock;
                this.btnSaveAndPrint.Enabled = isCheckAssignServiceSimultaneityOption ? false : isLock;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessServiceReqSDO(AssignServiceSDO serviceReqSDO, List<DataGridAdo> dataSereServModel, ref bool isDupicate, long treatmentId, bool IsNeedTrackingCreate)
        {
            try
            {
                if (this.currentHisTreatment != null)
                    serviceReqSDO.TreatmentId = treatmentId;

                //if (this.chkPriority.Checked)
                //    serviceReqSDO.Priority = GlobalVariables.HAS_PRIORITY;
                //else
                    serviceReqSDO.Priority = null;

                //if (this.chkIsNotRequireFee.Checked)
                //    serviceReqSDO.IsNotRequireFee = 1;

                if (this.serviceReqParentId != 0)
                    serviceReqSDO.ParentServiceReqId = this.serviceReqParentId;

                //if (this.txtDescription.Text != "")
                //    serviceReqSDO.Description = this.txtDescription.Text.Trim();
                serviceReqSDO.ProvisionalDiagnosis = txtProvisionalDiagnosis.Text;
                ACS_USER acsUser = null;
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                if (cboUser.EditValue != null)
                {
                    acsUser = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME.Equals(cboUser.EditValue.ToString()));
                }

                if (acsUser == null)
                {
                    acsUser = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME.Equals(loginName));
                }

                if (acsUser != null)
                {
                    serviceReqSDO.RequestLoginName = acsUser.LOGINNAME;
                    serviceReqSDO.RequestUserName = acsUser.USERNAME;
                    txtLoginName.Text = acsUser.LOGINNAME;
                    cboUser.EditValue = acsUser.LOGINNAME;
                }

                //if (cboConsultantUser.EditValue != null)
                //{
                //    var conUser = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME.Equals(cboConsultantUser.EditValue.ToString()));

                //    if (conUser != null)
                //    {
                //        serviceReqSDO.ConsultantLoginName = conUser.LOGINNAME;
                //        serviceReqSDO.ConsultantUserName = conUser.USERNAME;
                //    }
                //}


                //if (this.cboExecuteGroup.EditValue != null)
                //    serviceReqSDO.ExecuteGroupId = Inventec.Common.TypeConvert.Parse.ToInt64(this.cboExecuteGroup.EditValue.ToString());

                //if (IsNeedTrackingCreate)
                //{
                //    // điều kiện nhiều tờ điều trị
                //    GridCheckMarksSelection gridCheckMarkBusiness = cboTracking.Properties.Tag as GridCheckMarksSelection;
                //    var lstCheck = intructionTimeSelecteds.Select(o => o.ToString().Substring(0, 8)).ToList();
                //    if (gridCheckMarkBusiness != null && gridCheckMarkBusiness.SelectedCount > 0)
                //    {
                //        List<string> lstTrackingTimeDupicate = new List<string>();
                //        serviceReqSDO.TrackingInfos = new List<TrackingInfoSDO>();
                //        string mgsKhongNamTrongNgayChiDinh = "";
                //        string mgsTrungNgay = "";
                //        foreach (TrackingAdo rv in gridCheckMarkBusiness.Selection)
                //        {
                //            if (rv != null && !lstCheck.Exists(o => o == rv.TRACKING_TIME.ToString().Substring(0, 8)))
                //            {
                //                mgsKhongNamTrongNgayChiDinh += rv.TrackingTimeStr.Substring(0, 10) + ",";
                //            }
                //            else if (rv != null && lstTrackingTimeDupicate.Exists(o => o == rv.TRACKING_TIME.ToString().Substring(0, 8)))
                //            {
                //                mgsTrungNgay += rv.TrackingTimeStr.Substring(0, 10) + ",";
                //            }
                //            else
                //            {
                //                lstTrackingTimeDupicate.Add(rv.TRACKING_TIME.ToString().Substring(0, 8));
                //                TrackingInfoSDO sdo = new TrackingInfoSDO();
                //                sdo.TrackingId = rv.ID;
                //                sdo.IntructionTime = Convert.ToInt64(intructionTimeSelecteds.Where(o => o.ToString().Substring(0, 8) == rv.TRACKING_TIME.ToString().Substring(0, 8)).FirstOrDefault());
                //                serviceReqSDO.TrackingInfos.Add(sdo);
                //            }
                //        }
                //        if (!string.IsNullOrEmpty(mgsKhongNamTrongNgayChiDinh))
                //        {
                //            MessageBox.Show(string.Format("Tờ điều trị ngày {0} không nằm trong ngày chỉ định", mgsKhongNamTrongNgayChiDinh), "Thông báo");
                //            isDupicate = true;
                //            return;
                //        }
                //        if (!string.IsNullOrEmpty(mgsTrungNgay))
                //        {
                //            MessageBox.Show(string.Format("Ngày {0} có nhiều hơn 1 tờ điều trị", mgsTrungNgay), "Thông báo");
                //            isDupicate = true;
                //            return;
                //        }

                //    }// nếu chỉ có 1 tờ điều trị
                //    //else if (this.cboTracking.EditValue != null && !string.IsNullOrEmpty(this.cboTracking.EditValue.ToString()))
                //    //{
                //    //    serviceReqSDO.TrackingId = Inventec.Common.TypeConvert.Parse.ToInt64(this.cboTracking.EditValue.ToString());
                //    //}
                //    //else
                //    //{
                //    //    serviceReqSDO.TrackingId = null;
                //    //}
                //}

                //serviceReqSDO.IsNotRequireFee = (chkIsNotRequireFee.CheckState == CheckState.Checked) ? (short?)1 : null;
                //serviceReqSDO.IsInformResultBySms = (chkIsInformResultBySms.CheckState == CheckState.Checked);
                //serviceReqSDO.IsEmergency = (chkIsEmergency.CheckState == CheckState.Checked);

                if (dataSereServModel != null && dataSereServModel.Count > 0)
                {
                    foreach (var item in dataSereServModel)
                    {
                        ServiceReqDetailSDO sdo = new ServiceReqDetailSDO();
                        sdo.EkipInfos = new List<EkipSDO>();
                        sdo.Amount = item.AMOUNT;
                        sdo.PatientTypeId = item.PATIENT_TYPE_ID;
                        sdo.RoomId = item.TDL_EXECUTE_ROOM_ID;
                        sdo.ServiceId = item.SERVICE_ID;
                        sdo.ParentId = null;
                        sdo.MultipleExecute = item.NumberOfTimes;
                        sdo.InstructionNote = item.InstructionNote;
                        sdo.IsExpend = (item.IsExpend == true ? 1 : (short?)null);
                        sdo.IsOutParentFee = (item.IsOutKtcFee == true ? 1 : (short?)null);
                        sdo.ShareCount = item.ShareCount;
                        sdo.UserPrice = item.AssignSurgPriceEdit;
                        sdo.UserPackagePrice = item.AssignPackagePriceEdit;
                        // Thêm thông tin bảo lãnh
                        //if (item.IsGuarantee)
                        //    sdo.IsGuaranteed = true;
                        if (HisConfigCFG.ServicePatyForServicePackage != "1")
                        {
                            sdo.PackageId = item.PackagePriceId;
                        }
                        if (item.OTHER_PAY_SOURCE_ID.HasValue)
                            sdo.OtherPaySourceId = item.OTHER_PAY_SOURCE_ID;
                        if (HisConfigCFG.IsSetPrimaryPatientType == "1"
                            || HisConfigCFG.IsSetPrimaryPatientType == "2")
                        {
                            sdo.PrimaryPatientTypeId = item.PRIMARY_PATIENT_TYPE_ID;
                        }
                        if (item.IsNoDifference.HasValue)
                            sdo.IsNoHeinDifference = item.IsNoDifference.Value;
                        if (item.SERVICE_CONDITION_ID.HasValue)
                            sdo.ServiceConditionId = item.SERVICE_CONDITION_ID.Value;
                        if (item.SereServEkipADO != null && item.SereServEkipADO.listEkipUser != null && item.SereServEkipADO.listEkipUser.Count() > 0)
                        {
                            foreach (var ekip in item.SereServEkipADO.listEkipUser)
                            {
                                EkipSDO ekipSdo = new EkipSDO();
                                ekipSdo.ExecuteRoleId = ekip.EXECUTE_ROLE_ID;
                                ekipSdo.LoginName = ekip.LOGINNAME;
                                ekipSdo.UserName = ekip.USERNAME;
                                sdo.EkipInfos.Add(ekipSdo);
                            }
                        }
                        sdo.BedId = item.BedId;
                        sdo.BedFinishTime = item.BedFinishTime;
                        sdo.BedStartTime = item.BedStartTime;
                        sdo.IsNotUseBhyt = item.IsNotUseBhyt;
                        sdo.AssignNumOrder = item.AssignNumOrder;
                        if (item.TEST_SAMPLE_TYPE_ID > 0)
                            sdo.SampleTypeCode = item.TEST_SAMPLE_TYPE_CODE;
                        serviceReqSDO.ServiceReqDetails.Add(sdo);
                    }
                }

                serviceReqSDO.RequestRoomId = GetRoomId();
                serviceReqSDO.ManualRequestRoomId = GetManualRequestRoom();

                if (serviceReqSDO.RequestRoomId == 0)
                    Inventec.Common.Logging.LogSystem.Warn("Khong xac dinh du lieu phong lam viec trong module, chuc nang goi module nay khong truyen vao phong lam viec. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentModule), currentModule));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private long GetRoomId()
        {
            long roomId = 0;
            try
            {
                roomId = (this.currentModule != null ? this.currentModule.RoomId : 0);
                //Inventec.Common.Logging.LogSystem.Debug("Combo nguoi chi dinh khong gia tri ==> se lay phong dang lam viec gan vao RequestRoomId:" + Inventec.Common.Logging.LogUtil.TraceData("roomId", roomId) + Inventec.Common.Logging.LogUtil.TraceData("isAssignInPttt", isAssignInPttt) + Inventec.Common.Logging.LogUtil.TraceData("HisConfigCFG.SetRequestRoomByBedRoomWhenBeingInSurgery", HisConfigCFG.SetRequestRoomByBedRoomWhenBeingInSurgery));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return roomId;
        }

        private bool GetManualRequestRoom()
        {
            bool isManualRequestRoom = false;
            try
            {
                isManualRequestRoom = (this.examRegisterRoomId > 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return isManualRequestRoom;
        }

        private bool ServiceAttachForServicePrimary(ref AssignServiceSDO result, long pTypeId)
        {
            bool valid = true;
            string messErr = "";
            List<string> serviceErrs = new List<string>();
            try
            {
                //qtcode
                List<string> bhytWarnings = new List<string>();
                List<ServiceReqDetailSDO> serviceReqDetailSDOTemp = new List<ServiceReqDetailSDO>();
                List<V_HIS_SERVICE_FOLLOW> serviceFollows = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW>().Where(o => new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN }.Exists(p => p != o.FOLLOW_TYPE_ID) && (string.IsNullOrEmpty(o.TREATMENT_TYPE_IDS) || ("," + o.TREATMENT_TYPE_IDS + ",").Contains("," + currentTreatment.TDL_TREATMENT_TYPE_ID + ","))).ToList();
                if (result.ServiceReqDetails != null && result.ServiceReqDetails.Count > 0
                    && serviceFollows != null && serviceFollows.Count > 0)
                {
                    List<long> serviceIds = result.ServiceReqDetails.Select(o => o.ServiceId).Distinct().ToList();
                    long defaultPatientTypeId = pTypeId;
                    List<long> allowPatientTypeIds = this.currentPatientTypeAllows != null ? this.currentPatientTypeAllows
                        .Where(o => o.PATIENT_TYPE_ID == defaultPatientTypeId)
                        .Select(o => o.PATIENT_TYPE_ALLOW_ID).ToList() : null;
                    //qtcode
                    Dictionary<long, List<V_HIS_SERVICE_FOLLOW>> followByServiceId = new Dictionary<long, List<V_HIS_SERVICE_FOLLOW>>();
                    foreach (ServiceReqDetailSDO sdo in result.ServiceReqDetails) // duyệt từng thằng dịch vụ mình chọn 
                    {
                        List<V_HIS_SERVICE_FOLLOW> follows = serviceFollows.Where(o => o.SERVICE_ID == sdo.ServiceId).ToList();
                        // lọc ra các dịch vụ đi kèm có service_id = id của thằng dịch vụ mình chọn 
                        if (follows != null && follows.Count > 0)
                        {

                            foreach (V_HIS_SERVICE_FOLLOW f in follows)
                            {
                                bool isBhyt = (defaultPatientTypeId == HisConfigCFG.PatientTypeId__BHYT);
                                bool isDrugOrMaterial = (f.FOLLOW_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC ||
                                                        f.FOLLOW_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT);
                                //qtcode
                                if (isBhyt && isDrugOrMaterial)
                                {
                                    if (!followByServiceId.ContainsKey(sdo.ServiceId))
                                    {
                                        followByServiceId[sdo.ServiceId] = new List<V_HIS_SERVICE_FOLLOW>();
                                    }
                                    followByServiceId[sdo.ServiceId].Add(f);
                                    continue; // Bỏ qua việc thêm dịch vụ đi kèm này vào SDO
                                }
                                //qtcode
                                bool hasServicePaty = BranchDataWorker.DicServicePatyInBranch.ContainsKey(f.FOLLOW_ID) ? BranchDataWorker.HasServicePatyWithListPatientType(f.FOLLOW_ID, new List<long>() { defaultPatientTypeId }) : false;
                                long? patientTypeId = null;
                                if (hasServicePaty)
                                {
                                    patientTypeId = defaultPatientTypeId;
                                }
                                else
                                {
                                    V_HIS_SERVICE_PATY otherServicePaty = BranchDataWorker.ServicePatyWithListPatientType(f.FOLLOW_ID, allowPatientTypeIds).FirstOrDefault();
                                    var patientTypeAll = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList();

                                    patientTypeId = otherServicePaty != null ? (long?)otherServicePaty.PATIENT_TYPE_ID : null;

                                    var patientTypeIdPlus = patientTypeAll.Where(k => k.BASE_PATIENT_TYPE_ID != null && allowPatientTypeIds.Contains(k.BASE_PATIENT_TYPE_ID.Value)).ToList();
                                    if (patientTypeIdPlus != null && patientTypeIdPlus.Count > 0 && (otherServicePaty != null && !String.IsNullOrEmpty(otherServicePaty.INHERIT_PATIENT_TYPE_IDS) && patientTypeIdPlus.Exists(k => k.ID != patientTypeId)))
                                    {
                                        patientTypeId = patientTypeIdPlus.First().ID;
                                    }
                                    Inventec.Common.Logging.LogSystem.Debug("ServiceAttachForServicePrimary____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => otherServicePaty), otherServicePaty)
                                         + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientTypeIdPlus), patientTypeIdPlus)
                                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientTypeId), patientTypeId));
                                    //patientTypeId = otherServicePaty != null ? new Nullable<long>(otherServicePaty.PATIENT_TYPE_ID) : null;
                                }

                                if (patientTypeId.HasValue)
                                {
                                    var serviceIdErrs = serviceIds.Where(o => o == f.FOLLOW_ID).ToList();

                                    if (serviceIdErrs != null && serviceIdErrs.Count > 0)
                                    {
                                        foreach (var sve in serviceIdErrs)
                                        {
                                            var svs = ServiceAllADOs.Where(o => o.ID == sdo.ServiceId).FirstOrDefault();
                                            var svsFL = lstService.Where(o => o.ID == f.FOLLOW_ID).FirstOrDefault();
                                            if (svs != null && svsFL != null && !serviceErrs.Contains(string.Format(ResourceMessage.DichVuADaDuocTietLapDinhKemDichVuB, svs.SERVICE_NAME, svsFL.SERVICE_NAME)))
                                            {
                                                serviceErrs.Add(string.Format(ResourceMessage.DichVuADaDuocTietLapDinhKemDichVuB, svs.SERVICE_NAME, svsFL.SERVICE_NAME));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ServiceReqDetailSDO attach = new ServiceReqDetailSDO();
                                        attach.ServiceId = f.FOLLOW_ID;
                                        attach.Amount = f.AMOUNT;
                                        attach.PatientTypeId = patientTypeId.Value;
                                        attach.IsExpend = f.IS_EXPEND;
                                        if (HisConfigCFG.IsSetPrimaryPatientType == "2")
                                        {
                                            attach.PrimaryPatientTypeId = sdo.PrimaryPatientTypeId;
                                        }
                                        serviceReqDetailSDOTemp.Add(attach);
                                    }
                                }
                                else
                                {
                                    Inventec.Common.Logging.LogSystem.Debug("Tim thay V_HIS_SERVICE_FOLLOW theo service " + sdo.ServiceId + " nhung khong co doi tuong thanh toan hop le__bo qua khong them vao danh sach dv se chi dinh____Chi tiet____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => f), f));
                                }
                            }
                        }
                    }
                    // cảnh báo BHYT
                    if (followByServiceId.Count > 0)
                    {//followByServiceId là dict có id là id của dịch vụ mà mình chọn và giá trị là các dịch vụ đi kèm 
                        foreach (var serviceGroup in followByServiceId)
                        {
                            long serviceId = serviceGroup.Key;
                            var follows = serviceGroup.Value; // các giá trị dịch vụ đi kèm
                            var service = lstService.FirstOrDefault(o => o.ID == serviceId);
                            if (service == null) continue;

                            string serviceName = service.SERVICE_NAME; // tên dịch vụ chính 
                            var followNames = follows.Select(f => string.Format("[{0}] {1}", f.AMOUNT, f.FOLLOW_NAME)).ToList();
                            string warning = string.Format("{0} phải đi kèm {1}.", serviceName, string.Join(", ", followNames));
                            bhytWarnings.Add(warning);
                        }

                        if (bhytWarnings.Count > 0)
                        {
                            string warningMessage = string.Format("{0}. Bạn có muốn tiếp tục không?", string.Join("; ", bhytWarnings));
                            var rs = MessageBox.Show(
                                warningMessage,
                                Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(
                                    Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao
                                ),
                                MessageBoxButtons.YesNo
                            );
                            if (rs == DialogResult.No)
                            {
                                return false;
                            }
                        }
                    }
                    if (serviceReqDetailSDOTemp != null && serviceReqDetailSDOTemp.Count > 0)
                    {
                        result.ServiceReqDetails.AddRange(serviceReqDetailSDOTemp);
                    }
                }

                if (!VerifyCheckFeeWhileAssign(serviceReqDetailSDOTemp))
                {
                    return false;
                }

                if (serviceErrs != null && serviceErrs.Count > 0)
                {
                    messErr = String.Join("\r\n", serviceErrs);
                    // #25501- Bổ sung cấu hình hệ thống "HIS.Desktop.Plugins.AssignService.IsAllowingChooseServiceWhichInAttachments": "1: Cho phép chỉ định dịch vụ trùng với 1 dịch vụ đi kèm với 1 dịch vụ đã chọn trước đó. 0: Không cho phép".
                    //- Sửa chức năng "Chỉ định DVKT":
                    //+ Khi cấu hình trên được bật, thì khi chỉ định DVKT, nếu chỉ định dịch vụ trùng với 1 dịch vụ đi kèm với dịch vụ đã chọn trước đó, thì hiển thị cảnh báo "Dịch vụ B đã được thiết lập đính kèm dịch vụ A. Bạn có muốn tiếp tục không?". Nếu người dùng chọn "Không" thì dừng xử lý, nếu người dùng chọn "có" thì vẫn thực hiện lưu.
                    //+ Khi cấu hình tắt, thì xử lý như hiện tại: hiển thị cảnh báo và không cho phép lưu.
                    if (HisConfigCFG.IsAllowingChooseServiceWhichInAttachments)
                    {
                        messErr += ". Bạn có muốn tiếp tục không?";
                        if (MessageBox.Show(messErr, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                        {
                            valid = false;
                        }
                    }
                    else
                    {
                        valid = false;
                        MessageManager.Show(messErr);
                    }
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.IsAllowingChooseServiceWhichInAttachments), HisConfigCFG.IsAllowingChooseServiceWhichInAttachments) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceErrs), serviceErrs));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
        public List<long> USE_TIME { get; set; }
        private void SaveServiceReqCombo(AssignServiceSDO serviceReqSDO, bool issaveandprint, bool printTH, bool isSaveAndShow, bool isSign = false, bool isPrintPreview = false, bool IsPatientSelect = false, string patientName = null, string treatmentCode = null)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                WaitingManager.Show();
                serviceReqSDO.InstructionTime = intructionTimeSelecteds.First();
                serviceReqSDO.InstructionTimes = intructionTimeSelecteds;//TODO

                serviceReqSDO.UseTimes = this.USE_TIME;
                //Trường hợp chỉ định từ màn hình xử lý pttt, cập nhật dữ liệu cùng kíp, khác kíp tương ứng
                long sereservid = this.GetSereServInKip();
                if (sereservid > 0)
                {
                    foreach (var item in serviceReqSDO.ServiceReqDetails)
                    {
                        item.ParentId = sereservid;
                        item.EkipId = (this.currentSereServInEkip != null ? this.currentSereServInEkip.EKIP_ID : null);
                    }
                }

                if (this.serviceReqComboResultSDO != null && dicSessionCode != null && dicSessionCode.Count > 0 && dicSessionCode.ContainsKey(serviceReqSDO.TreatmentId) && !String.IsNullOrEmpty(dicSessionCode[serviceReqSDO.TreatmentId]) && this.actionType == GlobalVariables.ActionEdit)
                {
                    serviceReqSDO.SessionCode = dicSessionCode[serviceReqSDO.TreatmentId];
                    Inventec.Common.Logging.LogSystem.Debug("Sua chi dinh SessionCode =" + serviceReqComboResultSDO.SessionCode);
                    if (HisConfigCFG.AutoDeleteEmrDocumentWhenEditReq == "1" && dSignedList != null && dSignedList.Count > 0 && dSignedList.ContainsKey(serviceReqSDO.TreatmentId) && dSignedList[serviceReqSDO.TreatmentId] != null && dSignedList[serviceReqSDO.TreatmentId].Count > 0)
                    {

                        WaitingManager.Hide();
                        if (DevExpress.XtraEditors.XtraMessageBox.Show("Y lệnh đã tồn tại văn bản ký, hệ thống sẽ tự động xóa văn bản ký hiện tại. Bạn có muốn tiếp tục?", HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo) != DialogResult.Yes)
                            return;
                        List<DocumentSignedUpdateIGSysResultDTO> lst = new List<DocumentSignedUpdateIGSysResultDTO>();
                        foreach (var item in dSignedList[serviceReqSDO.TreatmentId])
                        {
                            CommonParam paramEmr = new CommonParam();
                            bool apiResult = new BackendAdapter(paramEmr).Post<bool>("api/EmrDocument/DeleteByCode", ApiConsumers.EmrConsumer, item.DocumentCode, paramEmr);
                            if (apiResult)
                            {
                                lst.Add(item);
                            }
                            else
                            {
                                #region Hien thi message thong bao
                                MessageManager.Show(this, paramEmr, apiResult);
                                #endregion
                            }
                        }
                        foreach (var item in lst)
                        {
                            dSignedList[serviceReqSDO.TreatmentId].Remove(item);
                        }
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug("Luu chi dinh____Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqSDO), serviceReqSDO));
                //Gọi api chỉ định dv
                var rs = new BackendAdapter(param).Post<HisServiceReqListResultSDO>("api/HisServiceReq/AssignServiceByInstructionTimes", ApiConsumers.MosConsumer, serviceReqSDO, ProcessLostToken, param);
                Inventec.Common.Logging.LogSystem.Info("this.serviceReqComboResultSDO: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));

                if (rs != null)
                {
                    this.totalGuaranteeOriginal = this.totalGuaranteePrice_1;
                    this.serviceReqComboResultSDO = rs;
                    dicSessionCode[serviceReqComboResultSDO.ServiceReqs[0].TREATMENT_ID] = serviceReqComboResultSDO.SessionCode;
                    dicServiceReqList[serviceReqComboResultSDO.ServiceReqs[0].TREATMENT_ID] = serviceReqComboResultSDO;
                    Inventec.Common.Logging.LogSystem.Debug("Chi dinh dich vu. Du lieu chi tiet dich vụ gui len api: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqSDO.ServiceReqDetails), serviceReqSDO.ServiceReqDetails));

                    // distint để tránh lặp #27825
                    if (this.serviceReqComboResultSDO.ServiceReqs != null && this.serviceReqComboResultSDO.ServiceReqs.Count > 0)
                    {
                        this.serviceReqComboResultSDO.ServiceReqs = this.serviceReqComboResultSDO.ServiceReqs.GroupBy(x => x.ID).Select(x => x.FirstOrDefault()).ToList();
                    }

                    if (this.serviceReqComboResultSDO.SereServs != null && this.serviceReqComboResultSDO.SereServs.Count > 0)
                    {
                        this.serviceReqComboResultSDO.SereServs = this.serviceReqComboResultSDO.SereServs.GroupBy(x => x.ID).Select(x => x.FirstOrDefault()).ToList();
                    }

                    if (this.serviceReqComboResultSDO.SereServExts != null && this.serviceReqComboResultSDO.SereServExts.Count > 0)
                    {
                        this.serviceReqComboResultSDO.SereServExts = this.serviceReqComboResultSDO.SereServExts.GroupBy(x => x.ID).Select(x => x.FirstOrDefault()).ToList();
                    }

                    if (this.serviceReqComboResultSDO.SereServRations != null && this.serviceReqComboResultSDO.SereServRations.Count > 0)
                    {
                        this.serviceReqComboResultSDO.SereServRations = this.serviceReqComboResultSDO.SereServRations.GroupBy(x => x.ID).Select(x => x.FirstOrDefault()).ToList();
                    }

                    //Gọi delegate xử lý ở module thực hiện gọi module chỉ định sau khi chỉ định thành công
                    //Truyền vào đầu vào là kết quả api trả về
                    if (this.processDataResult != null)
                        this.processDataResult(this.serviceReqComboResultSDO);

                    //Gọi delegate xử lý cập nhật bệnh phụ tại module thực hiện gọi module chỉ định, truyền vào các giá trị "bệnh chính", "bệnh phụ",... đã nhập trên form chỉ định
                    if (this.processRefeshIcd != null)
                        this.processRefeshIcd(this.serviceReqComboResultSDO.ServiceReqs[0].ICD_CODE, this.serviceReqComboResultSDO.ServiceReqs[0].ICD_NAME, this.serviceReqComboResultSDO.ServiceReqs[0].ICD_SUB_CODE, this.serviceReqComboResultSDO.ServiceReqs[0].ICD_TEXT);

                    success = true;
                    this.toggleSwitchDataChecked.EditValue = true;

                    this.actionType = GlobalVariables.ActionEdit;
                    this.SetEnableButtonControl(this.actionType);
                    this.isSaveAndPrint = issaveandprint;
                    //this.RefeshSereServInTreatmentData();
                    //qtcode
                    if (serviceReqComboResultSDO != null && serviceReqComboResultSDO.SereServs != null)
                    {
                        var sereServs = serviceReqComboResultSDO.SereServs;
                        var services = BackendDataWorker.Get<V_HIS_SERVICE>()
                            .Where(o => sereServs.Select(s => s.SERVICE_ID).Contains(o.ID))
                            .ToList();

                        // Sắp xếp theo thứ tự lưu dịch vụ
                        var orderedSereServs = serviceReqSDO.ServiceReqDetails
                            .Where(d => sereServs.Any(s => s.SERVICE_ID == d.ServiceId))
                            .Select(d => sereServs.FirstOrDefault(s => s.SERVICE_ID == d.ServiceId))
                            .Where(s => s != null)
                            .ToList();

                        var printGroups = orderedSereServs
                            .Join(services,
                                ss => ss.SERVICE_ID,
                                s => s.ID,
                                (ss, s) => new { SereServ = ss, Service = s })
                            .Where(o => !string.IsNullOrEmpty(o.Service.ATTACH_ASSIGN_PRINT_TYPE_CODE))
                            .GroupBy(o => o.Service.ATTACH_ASSIGN_PRINT_TYPE_CODE)
                            .ToList();

                        if (printGroups.Any())
                        {
                            HIS.Desktop.Plugins.Library.FormMedicalRecord.MediRecordMenuPopupProcessor processor = new HIS.Desktop.Plugins.Library.FormMedicalRecord.MediRecordMenuPopupProcessor();
                            foreach (var group in printGroups)
                            {
                                var printTypeCode = group.Key;
                                var ado = new Library.FormMedicalRecord.Base.EmrInputADO
                                {
                                    TreatmentId = this.treatmentId,
                                    PatientId = this.currentHisTreatment?.PATIENT_ID ?? 0,
                                    roomId = this.currentModule.RoomId
                                };


                                if (currentTreatment?.EMR_COVER_TYPE_ID != null)
                                {
                                    ado.EmrCoverTypeId = currentTreatment.EMR_COVER_TYPE_ID.Value;
                                }
                                else
                                {
                                    var data = BackendDataWorker.Get<HIS_EMR_COVER_CONFIG>()
                                        .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                                            && o.ROOM_ID == this.currentModule.RoomId
                                            && o.TREATMENT_TYPE_ID == (currentHisPatientTypeAlter?.TREATMENT_TYPE_ID ?? 0))
                                        .ToList();
                                    if (data != null && data.Count > 0)
                                    {
                                        if (data.Count == 1)
                                        {
                                            ado.EmrCoverTypeId = data.FirstOrDefault().EMR_COVER_TYPE_ID;
                                        }
                                        else
                                        {
                                            ado.lstEmrCoverTypeId = data.Select(o => o.EMR_COVER_TYPE_ID).ToList();
                                        }
                                    }
                                    else
                                    {
                                        var departmentId = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO
                                            .FirstOrDefault(o => o.RoomId == this.currentModule.RoomId)?.DepartmentId ?? 0;
                                        var dataConfig = BackendDataWorker.Get<HIS_EMR_COVER_CONFIG>()
                                            .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                                                && o.DEPARTMENT_ID == departmentId
                                                && o.TREATMENT_TYPE_ID == (currentHisPatientTypeAlter?.TREATMENT_TYPE_ID ?? 0))
                                            .ToList();
                                        if (dataConfig != null && dataConfig.Count > 0)
                                        {
                                            if (dataConfig.Count == 1)
                                            {
                                                ado.EmrCoverTypeId = dataConfig.FirstOrDefault().EMR_COVER_TYPE_ID;
                                            }
                                            else
                                            {
                                                ado.lstEmrCoverTypeId = dataConfig.Select(o => o.EMR_COVER_TYPE_ID).ToList();
                                            }
                                        }
                                    }
                                }

                                long emrCoverTypeId = ado.EmrCoverTypeId ?? 0;
                                long emrCoverTypeIdSend = emrCoverTypeId <= 0 ? 0 : emrCoverTypeId;
                                processor.FormOpenEmr(emrCoverTypeIdSend, ado, printTypeCode);
                            }
                        }
                    }
                    //Nếu mở từ tiếp đón chưa có icd và có nhập icd thì cập nhật Icd để in ra
                    //Comment do code gây lỗi, không biết lý do code sử dụng hàm này
                    //this.UpdateIcdToCurrentHisTreatment();

                    MPS.ProcessorBase.PrintConfig.PreviewType? previewType = null;
                    if (isSign)
                    {
                        //if (isSaveAndPrint)
                        //{
                        //    previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow;
                        //}
                        //else 
                            if (isPrintPreview)
                        {
                            previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintPreview;
                            isSaveAndShow = true;
                        }
                        else
                            previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow;
                    }
                    else
                    {
                        //if (isSaveAndPrint)
                        //{
                        //    previewType = MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow;
                        //}
                        //else
                        if (isPrintPreview)
                        {
                            previewType = MPS.ProcessorBase.PrintConfig.PreviewType.Show;
                            isSaveAndShow = true;
                        }
                    }
                    //InTamUng(isSaveAndShow, previewType);
                    Inventec.Common.Logging.LogSystem.Debug("SaveServiceReqCombo____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => previewType), previewType)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isSaveAndPrint), isSaveAndPrint)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isSign), isSign)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isPrintPreview), isPrintPreview)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstLoaiPhieu), lstLoaiPhieu)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => printTH), printTH));
                    //Nếu click nút lưu in => tự động gọi hàm xử lý in ngay
                    if (isPrintPreview || isSign)
                    {
                        if (workingAssignServiceADO.OpenFromBedRoomPartial && this.patientSelectProcessor != null && this.ucPatientSelect != null && this.patientSelectProcessor.GetSelectedRows(this.ucPatientSelect).Count > 1 && serviceReqComboResultSDO.ServiceReqs != null && serviceReqComboResultSDO.ServiceReqs.Count > 0)
                        {
                            LoadDataToCurrentTreatmentData(serviceReqComboResultSDO.ServiceReqs[0].TREATMENT_ID, serviceReqComboResultSDO.ServiceReqs[0].INTRUCTION_TIME);
                        }

                        UpdateIcdToTreatment(this.currentHisTreatment);

                        if (this.lstLoaiPhieu != null && this.lstLoaiPhieu.Count > 0)
                        {
                            var checkHDBN = this.lstLoaiPhieu.FirstOrDefault(o => o.Check == true && o.ID == "gridView7_2");

                            var checkYCDV = this.lstLoaiPhieu.FirstOrDefault(o => o.Check == true && o.ID == "gridView7_1");

                            var checkQR = this.lstLoaiPhieu.FirstOrDefault(o => o.Check == true && o.ID == "gridView7_3");

                            var checkTH = this.lstLoaiPhieu.FirstOrDefault(o => o.Check == true && o.ID == "gridView7_4");

                            if (checkHDBN != null)
                            {
                                if (!isPrinted) InTamUng(isSaveAndShow, previewType);
                                InPhieuHuoangDanBenhNhan(isSaveAndShow, previewType);

                            }

                            if (checkYCDV != null)
                            {
                                if (!isPrinted) InTamUng(isSaveAndShow, previewType);
                                InPhieuYeuCauDichVu(isSaveAndShow, previewType);
                            }

                            if (checkQR != null)
                            {
                                if (!isPrinted) InTamUng(isSaveAndShow, previewType);
                                InYeuCauThanhToanQR(isSaveAndPrint, isSign, isPrintPreview);
                            }

                            if (checkTH != null)
                            {
                                //if (chkSign.Checked == true) previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow;
                                if (!isPrinted) InTamUng(isSaveAndShow, previewType);
                                DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__IN__PHIEU_YEU_CAU_CHI_DINH_TONG_HOP__MPS000037, isSaveAndShow, previewType);
                            }
                        }

                        //foreach (var item in this.lstLoaiPhieu)
                        //{
                        //    if (item.Check)
                        //    {
                        //        if (item.ID == "gridView7_1")
                        //        {
                        //            InPhieuYeuCauDichVu(isSaveAndShow, previewType);
                        //        }
                        //        if (item.ID == "gridView7_2")
                        //        {
                        //            InPhieuHuoangDanBenhNhan(isSaveAndShow);
                        //        }
                        //    }
                        //}

                        if (printTH)
                        {
                            DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__IN__PHIEU_YEU_CAU_CHI_DINH_TONG_HOP__MPS000037, isSaveAndShow, previewType);
                        }
                    }

                }
                else
                {
                    ListMessError.Add("Bệnh nhân " + patientName + " (" + treatmentCode + ") :" + param.GetMessage());

                }
                WaitingManager.Hide();

                if (!IsPatientSelect)
                {
                    #region Show message
                    MessageManager.Show(this, param, success);
                    #endregion

                    #region Process has exception
                    SessionManager.ProcessTokenLost(param);
                    #endregion

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private long GetSereServInKip()
        {
            long result = 0;
            try
            {
                if (this.currentSereServ != null)
                    result = this.currentSereServ.ID;

                if (this.currentSereServInEkip != null)
                    result = this.currentSereServInEkip.ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void UpdateIcdToTreatment(HisTreatmentWithPatientTypeInfoSDO hisTreatmentWithPatientTypeInfoSDO)
        {
            try
            {

                var icdValue = UcIcdGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                if (icdValue != null)
                {
                    hisTreatmentWithPatientTypeInfoSDO.ICD_CODE = icdValue.ICD_CODE;
                    if (!string.IsNullOrEmpty(icdValue.ICD_CODE))
                    {
                        hisTreatmentWithPatientTypeInfoSDO.ICD_CODE = icdValue.ICD_CODE;
                    }
                    hisTreatmentWithPatientTypeInfoSDO.ICD_NAME = icdValue.ICD_NAME;
                }

                var icdValueCause = UcIcdCauseGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                if (icdValueCause != null)
                {
                    hisTreatmentWithPatientTypeInfoSDO.ICD_CAUSE_CODE = icdValueCause.ICD_CODE;
                    if (!string.IsNullOrEmpty(icdValueCause.ICD_CODE))
                    {
                        hisTreatmentWithPatientTypeInfoSDO.ICD_CAUSE_CODE = icdValueCause.ICD_CODE;
                    }
                    hisTreatmentWithPatientTypeInfoSDO.ICD_CAUSE_NAME = icdValueCause.ICD_NAME;
                }

                var subIcd = UcSecondaryIcdGetValue() as HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO;
                if (subIcd != null)
                {
                    hisTreatmentWithPatientTypeInfoSDO.ICD_SUB_CODE = subIcd.ICD_SUB_CODE;
                    hisTreatmentWithPatientTypeInfoSDO.ICD_TEXT = subIcd.ICD_TEXT;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal object UcIcdCauseGetValue()
        {
            object result = null;
            try
            {
                IcdInputADO outPut = new IcdInputADO();
                if (txtIcdCodeCause.ErrorText == "")
                {
                    if (chkEditIcdCause.Checked)
                        outPut.ICD_NAME = txtIcdMainTextCause.Text;
                    else
                        outPut.ICD_NAME = cboIcdsCause.Text;

                    if (!String.IsNullOrEmpty(txtIcdCodeCause.Text))
                    {
                        outPut.ICD_CODE = txtIcdCodeCause.Text;
                    }
                }
                else
                    outPut = null;
                result = outPut;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void InTamUng(bool isSaveAndShow, MPS.ProcessorBase.PrintConfig.PreviewType? previewType)
        {
            try
            {
                var countPrintConfig = this.lstLoaiPhieu.Where(s => s.Check == true).Distinct().ToList().Count;
                // điều kiện in : có ít nhất 1 cấu hình và có giao dịch tạm ứng /// nếu có tạm ứng thì in trước
                if (serviceReqComboResultSDO.SereServDeposits != null && serviceReqComboResultSDO.SereServDeposits.Count > 0 && countPrintConfig > 0)
                {
                    this.IsSaveAndShowMps000102 = isSaveAndShow;
                    this.PreviewTypeMps000102 = previewType;
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), LocalStorage.LocalData.GlobalVariables.TemnplatePathFolder);
                    richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000102, ProcessPrintMps000102);
                    isPrinted = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool ProcessPrintMps000102(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (serviceReqComboResultSDO != null && serviceReqComboResultSDO.SereServDeposits != null && serviceReqComboResultSDO.SereServDeposits.Count > 0 && serviceReqComboResultSDO.Transactions != null && serviceReqComboResultSDO.Transactions.Count > 0)
                {
                    V_HIS_TRANSACTION transactionPrint = new V_HIS_TRANSACTION();
                    List<HIS_SERE_SERV_DEPOSIT> ssDepositPrint = new List<HIS_SERE_SERV_DEPOSIT>();
                    transactionPrint = serviceReqComboResultSDO.Transactions.FirstOrDefault(o => o.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU);
                    if (transactionPrint == null)
                        return result;

                    ssDepositPrint = serviceReqComboResultSDO.SereServDeposits.Where(o => o.DEPOSIT_ID == transactionPrint.ID).ToList();

                    //chỉ định chưa có thời gian ra viện nên chưa cso số ngày điều trị
                    long? totalDay = null;
                    string departmentName = "";

                    //sử dụng DepositedSereServs để hiển thị thêm dịch vụ thanh toán cha
                    List<V_HIS_SERE_SERV> sereServs = new List<V_HIS_SERE_SERV>();
                    if (serviceReqComboResultSDO.DepositedSereServs != null && serviceReqComboResultSDO.DepositedSereServs.Count > 0)
                    {
                        sereServs = serviceReqComboResultSDO.DepositedSereServs.Where(o => ssDepositPrint.Exists(e => e.SERE_SERV_ID == o.ID)).ToList();
                    }

                    var SERVICE_REPORT_ID__HIGHTECH = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__DVKTC;

                    var sereServHitechs = sereServs.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID == SERVICE_REPORT_ID__HIGHTECH).ToList();
                    var sereServHitechADOs = PriceBHYTSereServAdoProcess(sereServHitechs);

                    //các sereServ trong nhóm vật tư
                    var SERVICE_REPORT__MATERIAL_VTTT_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT;
                    var sereServVTTTs = sereServs.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID == SERVICE_REPORT__MATERIAL_VTTT_ID && o.IS_OUT_PARENT_FEE != null).ToList();
                    var sereServVTTTADOs = PriceBHYTSereServAdoProcess(sereServVTTTs);

                    var sereServNotHitechs = sereServs.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID != SERVICE_REPORT_ID__HIGHTECH).ToList();

                    var servicePatyPrpos = lstService;

                    //Cộng các sereServ trong gói vào dv ktc
                    foreach (var sereServHitech in sereServHitechADOs)
                    {
                        List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO> sereServVTTTInKtcADOs = new List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();
                        var sereServVTTTInKtcs = sereServs.Where(o => o.PARENT_ID == sereServHitech.ID && o.IS_OUT_PARENT_FEE == null).ToList();
                        sereServVTTTInKtcADOs = PriceBHYTSereServAdoProcess(sereServVTTTInKtcs);
                        if (sereServHitech.PRICE_POLICY != null)
                        {
                            var servicePatyPrpo = servicePatyPrpos.Where(o => o.ID == sereServHitech.SERVICE_ID && o.BILL_PATIENT_TYPE_ID == sereServHitech.PATIENT_TYPE_ID && o.PACKAGE_PRICE == sereServHitech.PRICE_POLICY).ToList();
                            if (servicePatyPrpo != null && servicePatyPrpo.Count > 0)
                            {
                                sereServHitech.VIR_PRICE = sereServHitech.PRICE;
                            }
                        }
                        else
                            sereServHitech.VIR_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_PRICE);

                        sereServHitech.VIR_HEIN_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_HEIN_PRICE);
                        sereServHitech.VIR_PATIENT_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_HEIN_PRICE);

                        decimal totalHeinPrice = 0;
                        foreach (var sereServVTTTInKtcADO in sereServVTTTInKtcADOs)
                        {
                            totalHeinPrice += sereServVTTTInKtcADO.AMOUNT * sereServVTTTInKtcADO.PRICE_BHYT;
                        }
                        sereServHitech.PRICE_BHYT += totalHeinPrice;
                        sereServHitech.HEIN_LIMIT_PRICE += sereServVTTTInKtcADOs.Sum(o => o.HEIN_LIMIT_PRICE);

                        sereServHitech.VIR_TOTAL_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_PRICE);
                        sereServHitech.VIR_TOTAL_HEIN_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                        sereServHitech.VIR_TOTAL_PATIENT_PRICE = sereServHitech.VIR_TOTAL_PRICE - sereServHitech.VIR_TOTAL_HEIN_PRICE;
                        sereServHitech.SERVICE_UNIT_NAME = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(o => o.ID == sereServHitech.TDL_SERVICE_UNIT_ID).SERVICE_UNIT_NAME;
                    }

                    //Lọc các sereServ nằm không nằm trong dịch vụ ktc và vật tư thay thế
                    //
                    var sereServDeleteADOs = new List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();
                    foreach (var sereServVTTTADO in sereServVTTTADOs)
                    {
                        var sereServADODelete = sereServHitechADOs.Where(o => o.ID == sereServVTTTADO.PARENT_ID).ToList();
                        if (sereServADODelete.Count == 0)
                        {
                            sereServDeleteADOs.Add(sereServVTTTADO);
                        }
                    }

                    foreach (var sereServDelete in sereServDeleteADOs)
                    {
                        sereServVTTTADOs.Remove(sereServDelete);
                    }
                    var sereServVTTTIds = sereServVTTTADOs.Select(o => o.ID);
                    sereServNotHitechs = sereServNotHitechs.Where(o => !sereServVTTTIds.Contains(o.ID)).ToList();
                    var sereServNotHitechADOs = PriceBHYTSereServAdoProcess(sereServNotHitechs);

                    string ratio_text = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentHisPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentHisPatientTypeAlter.HEIN_CARD_NUMBER, currentHisPatientTypeAlter.LEVEL_CODE, currentHisPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100) + "";

                    MPS.Processor.Mps000102.PDO.PatientADO patientAdo = new MPS.Processor.Mps000102.PDO.PatientADO(this.patientPrint);

                    if (sereServNotHitechADOs != null && sereServNotHitechADOs.Count > 0)
                    {
                        sereServNotHitechADOs = sereServNotHitechADOs.OrderBy(o => o.TDL_SERVICE_NAME).ToList();
                    }

                    if (sereServHitechADOs != null && sereServHitechADOs.Count > 0)
                    {
                        sereServHitechADOs = sereServHitechADOs.OrderBy(o => o.TDL_SERVICE_NAME).ToList();
                    }

                    if (sereServVTTTADOs != null && sereServVTTTADOs.Count > 0)
                    {
                        sereServVTTTADOs = sereServVTTTADOs.OrderBy(o => o.TDL_SERVICE_NAME).ToList();
                    }

                    V_HIS_SERVICE_REQ firsExamRoom = new V_HIS_SERVICE_REQ();
                    if (this.currentHisTreatment.TDL_FIRST_EXAM_ROOM_ID.HasValue)
                    {
                        var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentHisTreatment.TDL_FIRST_EXAM_ROOM_ID);
                        if (room != null)
                        {
                            firsExamRoom.EXECUTE_ROOM_NAME = room.ROOM_NAME;
                        }
                    }

                    MPS.Processor.Mps000102.PDO.Mps000102PDO mps000102RDO = new MPS.Processor.Mps000102.PDO.Mps000102PDO(
                            patientAdo,
                            currentHisPatientTypeAlter,
                            departmentName,

                            sereServNotHitechADOs,
                            sereServHitechADOs,
                            sereServVTTTADOs,

                            null,//bản tin chuyển khoa, mps lấy ramdom thời gian vào khoa khi chỉ định tạm thời chưa cần
                            this.treatmentPrint,

                            BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>(),
                            transactionPrint,
                            ssDepositPrint,
                            totalDay,
                            ratio_text,
                            firsExamRoom
                            );
                    WaitingManager.Hide();

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.currentHisTreatment != null ? this.currentHisTreatment.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                    if (this.PreviewTypeMps000102.HasValue)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000102RDO, this.PreviewTypeMps000102.Value, printerName) { EmrInputADO = inputADO });
                    }
                    else if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2 || !this.IsSaveAndShowMps000102)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000102RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000102RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO> PriceBHYTSereServAdoProcess(List<V_HIS_SERE_SERV> sereServs)
        {
            List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO> sereServADOs = new List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();
            try
            {
                foreach (var item in sereServs)
                {
                    MPS.Processor.Mps000102.PDO.SereServGroupPlusADO sereServADO = new MPS.Processor.Mps000102.PDO.SereServGroupPlusADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>(sereServADO, item);

                    if (sereServADO.PATIENT_TYPE_ID != HisConfigCFG.PatientTypeId__BHYT)
                    {
                        sereServADO.PRICE_BHYT = 0;
                    }
                    else
                    {
                        if (sereServADO.HEIN_LIMIT_PRICE != null && sereServADO.HEIN_LIMIT_PRICE > 0)
                            sereServADO.PRICE_BHYT = (item.HEIN_LIMIT_PRICE ?? 0);
                        else
                            sereServADO.PRICE_BHYT = item.VIR_PRICE_NO_ADD_PRICE ?? 0;
                    }

                    sereServADOs.Add(sereServADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return sereServADOs;
        }
        Library.PrintServiceReq.PrintServiceReqProcessor PrintServiceReqProcessor;
        private bool DelegateRunPrinter(string printTypeCode, bool isSaveAndShow, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType)
        {
            bool result = false;
            try
            {
                // get bedLog
                //CommonParam param = new CommonParam();
                //MOS.Filter.HisBedLogViewFilter bedLogViewFilter = new MOS.Filter.HisBedLogViewFilter();
                //bedLogViewFilter.DEPARTMENT_IDs = this.serviceReqComboResultSDO.ServiceReqs.Select(o => o.REQUEST_DEPARTMENT_ID).ToList();
                //bedLogViewFilter.TREATMENT_ID = this.currentHisTreatment.ID;
                //var bedLogs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumer.ApiConsumers.MosConsumer, bedLogViewFilter, param);

                if (PreviewType.HasValue)
                {
                    PrintServiceReqProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(serviceReqComboResultSDO, currentHisTreatment, null, currentModule != null ? currentModule.RoomId : 0, PreviewType.Value);
                    PrintServiceReqProcessor.IsView = isSaveAndShow;
                }
                else
                {

                    PrintServiceReqProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(serviceReqComboResultSDO, currentHisTreatment, null, currentModule != null ? currentModule.RoomId : 0);
                    PrintServiceReqProcessor.IsView = isSaveAndShow;
                }

                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__SERVICE_REQ_REGISTER:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__YEU_CAU_KHAM_CHUYEN_KHOA__MPS000071:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__IN__PHIEU_YEU_CAU_CHI_DINH_TONG_HOP__MPS000037:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_XET_NGHIEM__MPS000026:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_CHUAN_DOAN_HINH_ANH__MPS000028:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_THAM_DO_CHUC_NANG__MPS000038:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_NOI_SOI__MPS000029:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_SIEU_AM__MPS000030:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_THU_THUAT__MPS000031:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_PHAU_THUAT__MPS000036:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_DICH_VU_KHAC__MPS000040:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_GIUONG__MPS000042:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_PHUC_HOI_CHUC_NANG__MPS000053:
                        InPhieuYeuCauDichVu(printTypeCode);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void InPhieuYeuCauDichVu(string printTypeCode)
        {
            try
            {
                if (PrintServiceReqProcessor != null)
                {
                    PrintServiceReqProcessor.Print(printTypeCode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void ProcessSaveData(bool isSaveAndPrint, bool printTH, bool isSaveAndShow, bool isSign = false, bool isPrintDocumentSigned = false)
        {
            try
            {
                if (!ValidForSave()) return;

                if (this.gridViewServiceProcess.IsEditing)
                    this.gridViewServiceProcess.CloseEditor();

                if (this.gridViewServiceProcess.FocusedRowModified)
                    this.gridViewServiceProcess.UpdateCurrentRow();

                bool isValid = true;
                List<DataGridAdo> serviceCheckeds__Send = this.DataGridAdo.FindAll(o => o.IsChecked);
                if (serviceTypeIdRequired != null && serviceTypeIdRequired.Count > 0)
                {
                    var serviceTypeInGrid = serviceCheckeds__Send.Select(o => new { o.TDL_SERVICE_NAME, o.SERVICE_TYPE_ID, o.TEST_SAMPLE_TYPE_ID }).ToList();
                    var lstServiceName = serviceTypeInGrid.Where(item => serviceTypeIdRequired.Exists(o => o == item.SERVICE_TYPE_ID) && item.TEST_SAMPLE_TYPE_ID <= 0).Select(o => o.TDL_SERVICE_NAME);
                    if (lstServiceName != null && lstServiceName.Count() > 0)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Dịch vụ {0} bắt buộc chọn Loại mẫu bệnh phẩm xét nghiệm", String.Join(", ", lstServiceName.ToList())), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
                        return;
                    }
                }
                if (ucIcdYhct != null)
                    isValid = isValid && (bool)icdYhctProcessor.ValidationIcd(ucIcdYhct);
                if (ucSecondaryIcdYhct != null)
                    isValid = isValid && subIcdYhctProcessor.GetValidate(ucSecondaryIcdYhct);
                isValid = isValid && this.Valid(serviceCheckeds__Send);
                isValid = isValid && this.CheckIcd(new List<V_HIS_TREATMENT_BED_ROOM> { new V_HIS_TREATMENT_BED_ROOM() { TREATMENT_ID = currentTreatment.ID, ICD_CODE = txtIcdCode.Text.Trim(), ICD_SUB_CODE = txtIcdSubCode.Text.Trim() } });
                bool isValidICD = true;
                if (HisConfigCFG.IsIcdServiceHasRequireCheckPatientBHYT && !this.CheckPatientTypeBHYT(new List<V_HIS_TREATMENT_BED_ROOM> { new V_HIS_TREATMENT_BED_ROOM() { TDL_PATIENT_TYPE_ID = currentTreatment.TDL_PATIENT_TYPE_ID } }))
                {
                    isValidICD = false;
                }
                if (isValidICD)
                {
                    List<HIS_ICD_SERVICE> icdServicePhacDos = null;
                    List<HIS_ICD> icdFromUc = GetIcdCodeListFromUcIcd();
                    if (icdFromUc != null && icdFromUc.Count > 0)
                    {
                        MOS.Filter.HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
                        icdServiceFilter.ICD_CODE__EXACTs = icdFromUc.Select(o => o.ICD_CODE).Distinct().ToList();
                        icdServicePhacDos = new BackendAdapter(null).Get<List<HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumer.ApiConsumers.MosConsumer, icdServiceFilter, null);

                        //isValid = isValid && ValidServiceIcdForIcdSelected(icdServices, serviceCheckeds__Send);
                        Inventec.Common.Logging.LogSystem.Debug("Valid3:" + isValid);
                        isValid = isValid && ValidServiceIcdForServiceSelected(icdFromUc, icdServicePhacDos, serviceCheckeds__Send);
                        Inventec.Common.Logging.LogSystem.Debug("Valid4:" + isValid);
                        if (!isValid && HisConfigCFG.IcdServiceHasCheck == "4")
                            return;
                        if (isValid && HisConfigCFG.IcdServiceHasCheck == "5")
                        {
                            icdFromUc = GetIcdCodeListFromUcIcd();
                            icdServiceFilter = new HisIcdServiceFilter();
                            icdServiceFilter.ICD_CODE__EXACTs = icdFromUc.Select(o => o.ICD_CODE).Distinct().ToList();
                            icdServicePhacDos = new BackendAdapter(null).Get<List<HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumer.ApiConsumers.MosConsumer, icdServiceFilter, null);
                        }
                    }
                    else if (HisConfigCFG.IcdServiceHasCheck == "3" && serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0)
                    {
                        MOS.Filter.HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
                        icdServiceFilter.SERVICE_IDs = serviceCheckeds__Send.Select(o => o.SERVICE_ID).Distinct().ToList();
                        icdServicePhacDos = new BackendAdapter(new CommonParam()).Get<List<HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumer.ApiConsumers.MosConsumer, icdServiceFilter, null);

                        if (icdServicePhacDos != null && icdServicePhacDos.Count > 0 && icdFromUc != null && icdFromUc.Count > 0)
                        {
                            icdServicePhacDos = icdServicePhacDos.Where(o => !icdFromUc.Select(p => p.ICD_CODE).Contains(o.ICD_CODE)).ToList();
                        }
                        if (icdServicePhacDos != null && icdServicePhacDos.Count > 0)
                        {
                            frmMissingIcd frmWaringConfigIcdService = new frmMissingIcd(icdFromUc, serviceCheckeds__Send, this.currentModule, icdServicePhacDos, getDataFromMissingIcdDelegate);
                            frmWaringConfigIcdService.ShowDialog();
                            if (!isYes)
                                isValid = false;
                        }
                    }
                }
                List<string> lstIcd = new List<string>();
                if (!string.IsNullOrEmpty(txtIcdCode.Text))
                {
                    var arrIcdCode = txtIcdCode.Text.Trim().Split(';');
                    foreach (var item in arrIcdCode)
                    {
                        if (!string.IsNullOrEmpty(item))
                            lstIcd.Add(item);
                    }
                }
                List<string> lstSubIcd = new List<string>();
                if (!string.IsNullOrEmpty(txtIcdSubCode.Text))
                {
                    var arrIcdCode = txtIcdSubCode.Text.Trim().Split(';');
                    foreach (var item in arrIcdCode)
                    {
                        if (!string.IsNullOrEmpty(item))
                            lstSubIcd.Add(item);
                    }
                }
                string EmptyMessage = null;
                isValid = isValid && ValidGenderServiceAllow(serviceCheckeds__Send);
                Inventec.Common.Logging.LogSystem.Debug("Valid5__ValidGenderServiceAllow:" + isValid);
                isValid = isValid && ValidSereServWithMinDuration(serviceCheckeds__Send);
                Inventec.Common.Logging.LogSystem.Debug("Valid6.1__ValidSereServWithMinDuration:" + isValid);
                isValid = isValid && ValidSereServWithCondition(serviceCheckeds__Send);
                Inventec.Common.Logging.LogSystem.Debug("Valid7__ValidSereServWithCondition:" + isValid);
                isValid = isValid && CheckMaxPatientbyDayOption(serviceCheckeds__Send);
                Inventec.Common.Logging.LogSystem.Debug("Valid8__ValidSereServWithCondition:" + isValid);
                if (lstIcd.Count > 0 || lstSubIcd.Count > 0)
                    isValid = isValid && checkContraindicated(lstIcd, lstSubIcd, icdServicePhacDos, serviceCheckeds__Send);
                Inventec.Common.Logging.LogSystem.Debug("Valid9__ValidSereServWithCondition:" + isValid);
                isValid = isValid && ValidSereServWithOtherPaySource(serviceCheckeds__Send);
                Inventec.Common.Logging.LogSystem.Debug("Valid10__ValidSereServWithOtherPaySource:" + isValid);
                isValid = isValid && ValidCheckTreatmentTypeBed(serviceCheckeds__Send, ref EmptyMessage);
                Inventec.Common.Logging.LogSystem.Debug("Valid11__ValidCheckTreatmentTypeBed:" + isValid);
                isValid = isValid && ValidSereServWithBed(serviceCheckeds__Send);
                Inventec.Common.Logging.LogSystem.Debug("Valid12__ValidSereServWithBed:" + isValid);
                //isValid = isValid && WarningAlertWarningFeeProcess(serviceCheckeds__Send);
                //Inventec.Common.Logging.LogSystem.Debug("Valid7__WarningAlertWarningFeeProcess:" + isValid);
                isValid = isValid && CheckIcdByRoom();
                Inventec.Common.Logging.LogSystem.Debug("Valid13__CheckIcdByRoom:" + isValid);
                isValid = isValid && ValidFeeForExamTreatment();
                Inventec.Common.Logging.LogSystem.Debug("Valid14__ValidFeeForExamTreatment:" + isValid);
                //isValid = isValid && CheckMaxAmount(serviceCheckeds__Send);
                isValid = isValid && ValidICD();
                Inventec.Common.Logging.LogSystem.Debug("Valid15__CheckMaxAmount:" + isValid);
                if (this.USE_TIME != null && this.USE_TIME.Count > 0)
                {
                    var exits = serviceCheckeds__Send.Where(s => s.SERVICE_TYPE_ID == 1 || s.SERVICE_TYPE_ID == 12);
                    if (exits.Any())
                    {
                        MessageBox.Show(this, "Dịch vụ loại khám và dịch vụ loại khác không cho phép dự trù");
                        isValid = false;
                        return;
                    }

                }
                if (HisConfigCFG.IsCheckDepartmentInTimeWhenPresOrAssign && this.currentWorkingRoom != null && currentWorkingRoom.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG)
                {
                    isValid = isValid && CheckTimeInDepartment(this.intructionTimeSelecteds);
                }

                ValidConsultationReqiured(serviceCheckeds__Send, this.treatmentId);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceCheckeds__Send), serviceCheckeds__Send));
                if (isValid)
                {
                    ChangeLockButtonWhileProcess(false);
                    AssignServiceSDO serviceReqSDO = new AssignServiceSDO();
                    serviceReqSDO.ServiceReqDetails = new List<ServiceReqDetailSDO>();
                    
                    bool isDupicate = false;
                    this.ProcessServiceReqSDO(serviceReqSDO, serviceCheckeds__Send, ref isDupicate, treatmentId, true);
                    if (isDupicate)
                    {
                        this.ChangeLockButtonWhileProcess(true);
                        return;
                    }
                    this.ProcessServiceReqSDOForIcd(serviceReqSDO);
                    //Cập nhật với trường hợp có dịch vụ đính kèm của các dịch vụ đã chọn chỉ định
                    if (this.ServiceAttachForServicePrimary(ref serviceReqSDO, this.currentHisPatientTypeAlter.PATIENT_TYPE_ID))
                    {
                        this.SaveServiceReqCombo(serviceReqSDO, isSaveAndPrint, printTH, isSaveAndShow, isSign, isPrintDocumentSigned);
                        if (isSaveAndPrint)
                        {
                            long isClosedForm = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY_HIS_DESKTOP_ASSIGN_SERVICE_CLOSED_FORM_AFTER_PRINT);
                            if (isClosedForm == 1)
                            {
                                this.Dispose();
                                this.Close();
                            }
                        }
                        this.RefeshServiceDatasourceAfterSave(serviceCheckeds__Send);
                    }
                    this.isCheckAssignServiceSimultaneityOption = false;
                    this.ChangeLockButtonWhileProcess(true);
                }
            }
            catch (Exception ex)
            {
                this.ChangeLockButtonWhileProcess(true);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region bắt lỗi mã ICD khi lưu
        bool ValidForSave()
        {
            bool valid = true;
            try
            {
                if (!String.IsNullOrEmpty(this.txtIcdCode.Text))
                {
                    var listData = this.currentIcds.Where(o => o.ICD_CODE.Contains(this.txtIcdCode.Text)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == this.txtIcdCode.Text).ToList() : listData) : null;
                    if (result == null || result.Count <= 0)
                    {
                        txtIcdCode.DoValidate();
                        //MessageBox.Show("Mã ICD bạn nhập không hợp lệ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        valid = false;
                        return valid;
                    }
                    else
                    {
                        txtIcdCode.ErrorText = "";
                        dxValidationProviderControl.RemoveControlError(txtIcdCode);
                    }
                }

                this.dxValidationProviderControl.RemoveControlError(txtIcdCode);
                this.positionHandleControl = -1;
                valid = dxValidationProviderControl.Validate() && valid;
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
        #endregion

        private bool ValidGenderServiceAllow(List<DataGridAdo> serviceCheckeds__Send)
        {
            bool valid = true;
            try
            {
                if (this.currentHisTreatment != null)
                {
                    // check giới tính

                    var genderCheck = GetDiffGender(serviceCheckeds__Send, this.currentHisTreatment.TDL_PATIENT_GENDER_ID);
                    if (genderCheck != null && genderCheck.Count() > 0)
                    {
                        string gender = genderCheck.FirstOrDefault().GENDER_ID == 1 ? "nữ" : "nam";

                        MessageManager.Show(ResourceMessage.DichVuKhongChiDinhChoGioiTinh + " " + gender + ": " + String.Join("; ", genderCheck.Select(o => o.TDL_SERVICE_NAME).ToArray()));
                        return false;
                    }

                    // check tuổi từ - đến (DVKT)
                    var ageDate = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentHisTreatment.TDL_PATIENT_DOB);
                    //int age = DateTime.Now.Year - int.Parse(this.currentHisTreatment.TDL_PATIENT_DOB.ToString().Substring(0, 4));
                    TimeSpan timeSpan2 = System.DateTime.Now.Date - ageDate.Value.Date;
                    long ticks = timeSpan2.Ticks;
                    System.DateTime dateTime = new System.DateTime(ticks);
                    int ageMonth = (dateTime.Year - 1) * 12 + dateTime.Month - 1;
                    //Inventec.Common.Logging.LogSystem.Debug("age: " + age);
                    var checkAge = serviceCheckeds__Send.Where(o => (o.AGE_FROM.HasValue && o.AGE_FROM > ageMonth) || (o.AGE_TO.HasValue && o.AGE_TO < ageMonth));

                    if (checkAge != null && checkAge.Count() > 0)
                    {
                        MessageManager.Show(ResourceMessage.DoTuoiCuaBNKhongPhuHopVoiDieuKienCuaDV + String.Join("; ", checkAge.Select(o => o.TDL_SERVICE_NAME).ToArray()) + ResourceMessage._VuiLongChonDVKhac);
                        return false;
                    }

                    // check dịch vụ giường với diện điều trị là khám, điều trị ngoại trú, điều trị ban ngày
                    //if (this.currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM || this.currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY || this.currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
                    //{
                    //    var treatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == this.currentHisTreatment.TDL_TREATMENT_TYPE_ID);
                    //    var dichVuGiuong = serviceCheckeds__Send.Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G).ToList();
                    //    if (dichVuGiuong != null && dichVuGiuong.Count() > 0 && treatmentType != null)
                    //    {
                    //        if (MessageBox.Show(ResourceMessage.DienDieuTriCuaBNLa + treatmentType.TREATMENT_TYPE_NAME + ResourceMessage._BanCoMuonChiDinhGiuong, MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    //            return false;
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        private bool ValidSereServWithMinDuration(List<DataGridAdo> serviceCheckeds__Send)
        {
            bool valid = true;
            try
            {
                List<HIS_SERE_SERV> sereServMinDurations = getSereServWithMinDuration(serviceCheckeds__Send, this.currentTreatment.PATIENT_ID);
                if (sereServMinDurations != null && sereServMinDurations.Count > 0)
                {
                    string sereServMinDurationStr = "";
                    foreach (var item in sereServMinDurations)
                    {
                        sereServMinDurationStr += item.TDL_SERVICE_CODE + " - " + item.TDL_SERVICE_NAME + " - " +
                           Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(item.TDL_INTRUCTION_TIME) +
                           " (" + item.TDL_SERVICE_REQ_CODE +
                           "); ";
                    }
                    if (HisConfigCFG.IsSereServMinDurationAlert == 1)
                    {
                        if (MessageBox.Show(string.Format(ResourceMessage.SereServMinDurationAlert__BanCoMuonChuyenDoiDTTTSangVienPhi, sereServMinDurationStr), MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            foreach (var sv in serviceCheckeds__Send)
                            {
                                //Thực hiện tự động chuyển đổi đối tượng sang viện phí                     
                                if (sereServMinDurations.Any(o => o.SERVICE_ID == sv.SERVICE_ID))
                                {
                                    sv.PATIENT_TYPE_ID = HisConfigCFG.PatientTypeId__VP;
                                }
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (HisConfigCFG.IsSereServMinDurationAlert == 2)
                    {
                        if (MessageBox.Show(string.Format(ResourceMessage.DichVuCoThoiGianChiDinhNamTrongKhoangThoiGianKhongChoPhep, sereServMinDurationStr), MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (HisConfigCFG.IsSereServMinDurationAlert == 0 || (HisConfigCFG.IsSereServMinDurationAlert != 1 && HisConfigCFG.IsSereServMinDurationAlert != 2))
                        {
                            if (MessageBox.Show(string.Format(ResourceMessage.DichVuCoThoiGianChiDinhNamTrongKhoangThoiGianKhongChoPhep, sereServMinDurationStr), MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        private bool CheckIcdByRoom()
        {
            bool valid = true;
            if (this.requestRoom.IS_ALLOW_NO_ICD != 1)
                return valid;

            try
            {
                var icdValue = UcIcdGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                if ((icdValue != null && String.IsNullOrEmpty(icdValue.ICD_CODE)) && String.IsNullOrEmpty(txtProvisionalDiagnosis.Text))
                {
                    if (MessageBox.Show(ResourceMessage.ChuaNhapChanDoanChinhVaChanDoanSoBo, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                    {
                        valid = false;
                        if (String.IsNullOrEmpty(icdValue.ICD_CODE))
                        {
                            txtIcdCode.Focus();
                            txtIcdCode.SelectAll();
                        }
                        else
                        {
                            txtProvisionalDiagnosis.Focus();
                            txtProvisionalDiagnosis.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        private bool ValidFeeForExamTreatment()
        {
            bool result = true;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("qtcode canhbao");
                if ((HisConfigCFG.WarningOverTotalPatientPrice__IsCheck == "2" || HisConfigCFG.WarningOverTotalPatientPrice__IsCheck == "3") && this.currentHisPatientTypeAlter != null && this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM && (this.currentHisTreatment != null && string.IsNullOrEmpty(this.currentHisTreatment.GUARANTEE_CODE)))
                {
                    Inventec.Common.Logging.LogSystem.Debug("qtcode vao canhbao");
                    decimal tmp = 0;
                    decimal tongtienBHYT = GetDefaultSerServTotalPrice(ref tmp, HisConfigCFG.PatientTypeId__BHYT);
                    decimal totalPrice = GetDefaultSerServTotalPrice(ref tmp);
                    decimal checkPrice = this.transferTreatmentFee + totalPrice - tongtienBHYT;

                    if (checkPrice > 0 && MessageBox.Show(String.Format(ResourceMessage.BenhNhanDangThieuVienPhi,
                        Inventec.Common.Number.Convert.NumberToString(checkPrice, ConfigApplications.NumberSeperator)), "Cảnh báo",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.No)
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool CheckTimeInDepartment(List<long> listTime)
        {
            bool result = true;
            try
            {
                V_HIS_ROOM currentWorkingRoom = null;
                currentWorkingRoom = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().First(o => o.ID == this.currentModule.RoomId);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listTime), listTime));

                List<HIS_DEPARTMENT_TRAN> curremtTrans = null;
                if (this.ListDepartmentTranCheckTime != null && this.ListDepartmentTranCheckTime.Count > 0)
                {
                    curremtTrans = this.ListDepartmentTranCheckTime.Where(o => o.DEPARTMENT_ID == currentWorkingRoom.DEPARTMENT_ID && o.DEPARTMENT_IN_TIME.HasValue).ToList();
                }

                List<HIS_CO_TREATMENT> currentCo = null;
                if (this.ListCoTreatmentCheckTime != null && this.ListCoTreatmentCheckTime.Count > 0)
                {
                    currentCo = this.ListCoTreatmentCheckTime.Where(o => o.DEPARTMENT_ID == currentWorkingRoom.DEPARTMENT_ID && o.START_TIME.HasValue).ToList();
                }

                foreach (var intructionTime in listTime)
                {
                    bool hasTran = false;

                    List<string> times = new List<string>();
                    if (curremtTrans != null && curremtTrans.Count > 0)
                    {
                        curremtTrans = curremtTrans.OrderBy(o => o.DEPARTMENT_IN_TIME ?? 0).ToList();

                        long fromTime = 0;
                        long toTime = 0;

                        foreach (var item in curremtTrans)
                        {
                            fromTime = item.DEPARTMENT_IN_TIME ?? 0;
                            toTime = long.MaxValue;
                            HIS_DEPARTMENT_TRAN nextTran = this.ListDepartmentTranCheckTime.FirstOrDefault(o => o.PREVIOUS_ID == item.ID);
                            if (nextTran != null)
                            {
                                toTime = nextTran.DEPARTMENT_IN_TIME ?? long.MaxValue;
                            }

                            hasTran = hasTran || (fromTime <= intructionTime && intructionTime <= toTime);

                            times.Add(string.Format("từ {0}{1}", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(fromTime),
                            (toTime > 0 && toTime != long.MaxValue) ? " đến " + Inventec.Common.DateTime.Convert.TimeNumberToTimeString(toTime) : ""));
                        }
                    }

                    if (!hasTran && times.Count > 0 && currentCo != null && currentCo.Count > 0)
                    {
                        times.Clear();
                    }

                    if (!hasTran && currentCo != null && currentCo.Count > 0)
                    {
                        currentCo = currentCo.OrderBy(o => o.START_TIME ?? 0).ToList();

                        long fromTime = 0;
                        long toTime = 0;

                        foreach (var item in currentCo)
                        {
                            fromTime = item.START_TIME ?? 0;
                            toTime = item.FINISH_TIME ?? long.MaxValue;

                            hasTran = hasTran || (fromTime <= intructionTime && intructionTime <= toTime);

                            times.Add(string.Format("từ {0}{1}", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(fromTime),
                            (toTime > 0 && toTime != long.MaxValue) ? " đến " + Inventec.Common.DateTime.Convert.TimeNumberToTimeString(toTime) : ""));
                        }
                    }

                    if (!hasTran)
                    {
                        XtraMessageBox.Show(string.Format(ResourceMessage.ThoiGianYLenhKhongThuocKhoangThoiGianTrongKhoa,
                           string.Join(",", times)),
                            MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                        this.isNotLoadWhileChangeInstructionTimeInFirst = true;
                        this.isNotLoadWhileChangeInstructionTimeInFirst = false;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ValidConsultationReqiured(List<DataGridAdo> serviceCheckeds__Send, long treatmentId)
        {
            try
            {
                List<HIS_DEBATE> lstHisDebate = new List<HIS_DEBATE>();
                List<V_HIS_SERVICE> lstServiceWarn = new List<V_HIS_SERVICE>();
                V_HIS_ROOM currentWorkingRoom = null;
                string message = "";

                currentWorkingRoom = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().First(o => o.ID == this.currentModule.RoomId);

                if (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0)
                {
                    CommonParam param = new CommonParam();

                    var lstHisService = lstService.Where(o => o.MUST_BE_CONSULTED == 1 && serviceCheckeds__Send.Select(p => p.SERVICE_ID).ToList().Exists(p => p == o.ID)).ToList();

                    if (lstHisService != null && lstHisService.Count > 0 && currentWorkingRoom != null)
                    {
                        HisDebateFilter DebateFilter = new HisDebateFilter();
                        DebateFilter.DEPARTMENT_ID = currentWorkingRoom.DEPARTMENT_ID;
                        DebateFilter.SERVICE_IDs = lstHisService.Select(o => o.ID).ToList();
                        DebateFilter.TREATMENT_ID = treatmentId;

                        lstHisDebate = new BackendAdapter(param).Get<List<HIS_DEBATE>>(HisRequestUriStore.HIS_DEBATE_GET, ApiConsumers.MosConsumer, DebateFilter, param);

                        if (lstHisDebate != null && lstHisDebate.Count > 0)
                        {
                            foreach (var itemS in lstHisService)
                            {
                                var check = lstHisDebate.FirstOrDefault(o => o.SERVICE_ID == itemS.ID);
                                if (check == null)
                                {
                                    lstServiceWarn.Add(itemS);
                                }
                            }
                        }
                        else
                        {
                            lstServiceWarn = lstHisService;
                        }
                    }
                }

                if (lstServiceWarn != null && lstServiceWarn.Count > 0)
                {
                    message = String.Format(ResourceMessage.KhoaChiDinhChuaTaoBienBanHoiChuan, String.Join(",", lstServiceWarn.Select(o => o.SERVICE_NAME).ToList()));

                    frmServiceDebateConfirm frm = new frmServiceDebateConfirm(this.currentModule, lstServiceWarn, lstHisDebate, message, treatmentId);
                    frm.ShowDialog();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessServiceReqSDOForIcd(AssignServiceSDO serviceReqSDO)
        {
            try
            {
                var icdValue = UcIcdGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                if (icdValue != null)
                {
                    serviceReqSDO.IcdCode = icdValue.ICD_CODE;
                    if (!string.IsNullOrEmpty(icdValue.ICD_CODE))
                    {
                        serviceReqSDO.IcdCode = icdValue.ICD_CODE;
                    }
                    serviceReqSDO.IcdName = icdValue.ICD_NAME;
                }

                var icdValueCause = UcIcdCauseGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                if (icdValueCause != null)
                {
                    serviceReqSDO.IcdCauseCode = icdValueCause.ICD_CODE;
                    if (!string.IsNullOrEmpty(icdValueCause.ICD_CODE))
                    {
                        serviceReqSDO.IcdCauseCode = icdValueCause.ICD_CODE;
                    }
                    serviceReqSDO.IcdCauseName = icdValueCause.ICD_NAME;
                }

                var subIcd = UcSecondaryIcdGetValue() as HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO;
                if (subIcd != null)
                {
                    serviceReqSDO.IcdSubCode = subIcd.ICD_SUB_CODE;
                    serviceReqSDO.IcdText = subIcd.ICD_TEXT;
                }


                var icdTranditional = this.icdYhctProcessor.GetValue(this.ucIcdYhct);
                if (icdTranditional != null && icdTranditional is IcdInputADO)
                {
                    serviceReqSDO.TraditionalIcdCode = ((IcdInputADO)icdTranditional).ICD_CODE;
                    serviceReqSDO.TraditionalIcdName = ((IcdInputADO)icdTranditional).ICD_NAME;
                }
                var subIcdTranditional = subIcdYhctProcessor.GetValue(ucSecondaryIcdYhct);
                if (subIcdTranditional != null && subIcdTranditional is SecondaryIcdDataADO)
                {
                    serviceReqSDO.TraditionalIcdSubCode = ((SecondaryIcdDataADO)subIcdTranditional).ICD_SUB_CODE;
                    serviceReqSDO.TraditionalIcdText = ((SecondaryIcdDataADO)subIcdTranditional).ICD_TEXT;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RefeshServiceDatasourceAfterSave(List<DataGridAdo> serviceCheckeds__Send)
        {
            try
            {
                foreach (var sv in serviceCheckeds__Send)
                {
                    var sv1 = this.ServiceIsleafADOs.Where(o => o.SERVICE_ID == sv.SERVICE_ID).FirstOrDefault();
                    sv1.PATIENT_TYPE_ID = sv.PATIENT_TYPE_ID;
                }

                gridControlServiceProcess.RefreshDataSource();

                //gridViewServiceProcess.BeginUpdate();
                //gridViewServiceProcess.GridControl.DataSource = serviceCheckeds__Send;
                //gridViewServiceProcess.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                if (this.DataGridAdo != null)
                {
                    foreach (var item in this.DataGridAdo)
                    {
                        item.IsChecked = false;
                    }
                }

                int[] selectedRows = gridViewServiceProcess.GetSelectedRows();
                foreach (int rowHandle in selectedRows)
                {
                    if (rowHandle >= 0)
                    {
                        var item = gridViewServiceProcess.GetRow(rowHandle) as DataGridAdo;
                        if (item != null)
                            item.IsChecked = true;
                    }
                }

                // Refresh grid để hiển thị thay đổi (nếu cần)
                gridViewServiceProcess.RefreshData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            var data = gridViewServiceProcess.GetRow(e.RowHandle) as HisBedADO;
            if (data != null && data.IsKey == 2)
            {
                e.Appearance.ForeColor = Color.Red;  
            }
        }

        private void InitGridLookUpEditBed()
        {
            try
            {
                // 1. Tô màu đỏ cho giường đầy
                repositoryItemGridLookUpEdit1View.RowStyle += RepositoryItemGridLookUpEdit1View_RowStyle;

                // 2. Chặn không cho chọn giường đầy
                repositoryItemGridLookUpEditBed.EditValueChanging += RepositoryItemGridLookUpEditBed_EditValueChanging;

                // 3. (Tùy chọn) Hiển thị tooltip
                repositoryItemGridLookUpEdit1View.RowCellStyle += RepositoryItemGridLookUpEdit1View_RowCellStyle;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RepositoryItemGridLookUpEdit1View_RowStyle(object sender, RowStyleEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view != null && e.RowHandle >= 0)
                {
                    var data = view.GetRow(e.RowHandle) as HisBedADO;
                    if (data != null && data.IsKey == 2)
                    {
                        e.Appearance.BackColor = Color.LightCoral;  // Màu đỏ nhạt
                        e.Appearance.ForeColor = Color.DarkRed;     // Chữ đỏ đậm
                                                                    // Hoặc dùng: e.Appearance.BackColor = Color.FromArgb(255, 200, 200);
                    }
                    else if (data != null && data.IsKey == 1)
                    {
                        e.Appearance.BackColor = Color.Blue;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // Event 2: Chặn không cho chọn giường đầy
        private void RepositoryItemGridLookUpEditBed_EditValueChanging(object sender, ChangingEventArgs e)
        {
            try
            {
                var repo = sender as RepositoryItemGridLookUpEdit;
                if (repo != null && repo.View != null)
                {
                    var data = repo.View.GetFocusedRow() as HisBedADO;
                    if (data != null && data.IsKey == 2)
                    {
                        e.Cancel = true; // Hủy việc chọn
                        XtraMessageBox.Show(
                            "Giường này đã đầy, không thể chọn!",
                            "Thông báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // Event 3 (Tùy chọn): Thêm text "(Đầy)" vào hiển thị
        private void RepositoryItemGridLookUpEdit1View_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view != null && e.RowHandle >= 0)
                {
                    var data = view.GetRow(e.RowHandle) as HisBedADO;
                    if (data != null && data.IsKey == 2)
                    {
                        e.Appearance.BackColor = Color.LightCoral;
                        e.Appearance.ForeColor = Color.DarkRed;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItembtnEditDonGia_TextDisable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                DataGridAdo ssADO = (DataGridAdo)gridViewServiceProcess.GetFocusedRow();
                if (ssADO != null)
                {
                    frmPriceEdit frmPriceEdit = new frmPriceEdit(ssADO, UpdateSurgPrice, PriceEditType.EditTypeSurgPrice, GetPriceBySurg);
                    frmPriceEdit.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateSurgPrice(DataGridAdo data)
        {
            try
            {
                if (this.gridViewServiceProcess.IsEditing)
                    this.gridViewServiceProcess.CloseEditor();

                if (this.gridViewServiceProcess.FocusedRowModified)
                    this.gridViewServiceProcess.UpdateCurrentRow();

                this.gridControlServiceProcess.RefreshDataSource();

                this.SetDefaultSerServTotalPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEditOtherPaySource_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                this.currentRowSereServADO = (DataGridAdo)gridViewServiceProcess.GetFocusedRow();
                if (this.currentRowSereServADO != null && this.currentRowSereServADO.IsChecked)
                {
                    if (e.Button.Kind == ButtonPredefines.Down || e.Button.Kind == ButtonPredefines.DropDown)
                    {
                        ButtonEdit editor = sender as ButtonEdit;
                        Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                        popupControlContainerOtherPaySource.ShowPopup(new Point(buttonPosition.X + 532, buttonPosition.Bottom + 160));

                        var dataOtherPaySources = BackendDataWorker.Get<HIS_OTHER_PAY_SOURCE>();
                        List<HIS_OTHER_PAY_SOURCE> dataOtherPaySourceTmps = new List<HIS_OTHER_PAY_SOURCE>();
                        dataOtherPaySources = (dataOtherPaySources != null && dataOtherPaySources.Count > 0) ? dataOtherPaySources.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;
                        if (dataOtherPaySources != null && dataOtherPaySources.Count > 0)
                        {
                            var workingPatientType = currentPatientTypes.Where(t => t.ID == this.currentRowSereServADO.PATIENT_TYPE_ID).FirstOrDefault();

                            if (workingPatientType != null && !String.IsNullOrEmpty(workingPatientType.OTHER_PAY_SOURCE_IDS))
                            {
                                dataOtherPaySourceTmps = dataOtherPaySources.Where(o => ("," + workingPatientType.OTHER_PAY_SOURCE_IDS + ",").Contains("," + o.ID + ",")).ToList();
                            }
                            else
                            {
                                dataOtherPaySourceTmps.AddRange(dataOtherPaySources);
                            }

                        }

                        gridControlOtherPaySource.DataSource = null;
                        gridControlOtherPaySource.DataSource = dataOtherPaySourceTmps;
                        gridControlOtherPaySource.Focus();

                        int focusRow = 0;
                        if (this.currentRowSereServADO.OTHER_PAY_SOURCE_ID > 0 && dataOtherPaySourceTmps != null && dataOtherPaySourceTmps.Count > 0)
                        {

                            for (int i = 0; i < dataOtherPaySourceTmps.Count; i++)
                            {
                                if (dataOtherPaySourceTmps[i].ID == this.currentRowSereServADO.OTHER_PAY_SOURCE_ID)
                                {
                                    focusRow = i;
                                }
                            }
                        }
                        gridViewOtherPaySource.FocusedRowHandle = focusRow;
                    }
                    else if (e.Button.Kind == ButtonPredefines.Delete)
                    {
                        this.currentRowSereServADO.OTHER_PAY_SOURCE_ID = null;
                        this.currentRowSereServADO.OTHER_PAY_SOURCE_CODE = "";
                        this.currentRowSereServADO.OTHER_PAY_SOURCE_NAME = "";
                        this.gridControlServiceProcess.RefreshDataSource();

                        if (this.gridViewServiceProcess.IsEditing)
                            this.gridViewServiceProcess.CloseEditor();

                        if (this.gridViewServiceProcess.FocusedRowModified)
                            this.gridViewServiceProcess.UpdateCurrentRow();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
    public class BankInfo
    {
        public BankInfo() { }
        public string BANK { get; set; }
        public string VALUE { get; set; }
    }
}

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
using DevExpress.Data;
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
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Advise;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Base;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ChooseICD;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Worker;
using HIS.Desktop.Plugins.Library.FormMedicalRecord;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using HIS.UC.PatientSelect;
using HIS.UC.PeriousExpMestList;
using HIS.UC.SecondaryIcd;
using HIS.UC.SecondaryIcd.ADO;
using HIS.UC.TreatmentFinish;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.PopupLoader;
using Inventec.Common.Logging;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Plugins.Library.CheckIcd;
using System.Globalization;
using HIS.Desktop.Plugins.AssignPrescriptionPK.MessageBoxForm;
using System.Threading;
using HIS.UC.Icd;
using HIS.UC.Icd.ADO;
using DevExpress.XtraGrid;
using System.ComponentModel.DataAnnotations;
using static MPS.ProcessorBase.PrintConfig;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription
{
    public partial class frmAssignPrescription : FormBase
    {
        #region Reclare variable
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();

        long? serviceReqParentId;
        long treatmentId = 0;
        long? expMestTemplateId;
        string treatmentCode;
        int actionBosung = 0;
        int positionHandle = 0;
        int positionHandle__DuongDung = 0;
        internal int positionHandleControl = -1;
        internal int actionType = 0;
        internal int actionTypePrint = 0;
        internal bool isMultiDateState = false;
        internal List<long> intructionTimeSelecteds = new List<long>();
        internal List<DateTime?> intructionTimeSelected = new List<DateTime?>();
        DateTime timeSelested;

        internal List<long> UseTimeSelecteds = new List<long>();
        internal List<DateTime?> UseTimeSelected = new List<DateTime?>();
        internal long UseTime { get; set; }

        internal bool isMultiDateStateForMedi = false;
        internal List<long> intructionTimeSelectedsForMedi = new List<long>();
        internal List<DateTime?> intructionTimeSelectedForMedi = new List<DateTime?>();
        DateTime timeSelestedForMedi;
        internal int idRow = 1;
        internal long InstructionTime { get; set; }
        public List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN> ListMedicineTypeAcin { get; set; }
        public List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE> ListMedicineTypeOld { get; set; }

        internal bool limitHeinMedicinePrice = false;
        internal V_HIS_SERE_SERV currentSereServ { get; set; }
        internal V_HIS_SERE_SERV currentSereServInEkip { get; set; }
        V_HIS_SERVICE Service__Main;
        public HIS_SERVICE_REQ serviceReqMain;
        decimal currentExpendInServicePackage;
        HIS.Desktop.ADO.AssignPrescriptionADO.DelegateProcessDataResult processDataResult;
        HIS.Desktop.ADO.AssignPrescriptionADO.DelegateProcessRefeshIcd processRefeshIcd;
        HIS.Desktop.ADO.AssignPrescriptionADO.DelegateProcessWhileAutoTreatmentEnd processWhileAutoTreatmentEnd;
        bool isInKip;
        long patientId;
        string patientName;
        internal long patientDob;
        string genderName;
        internal bool isAutoCheckExpend = false;
        internal const int stepRow = 1;
        internal decimal totalPriceBHYT = 0;
        decimal totalHeinByTreatment = 0;
        decimal totalHeinPriceByTreatment = 0;
        bool IsSignExamTreatmentFn = false;
        bool IsPrintExamTreatmentFn = false;
        internal HisTreatmentWithPatientTypeInfoSDO currentTreatmentWithPatientType { get; set; }
        internal MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER currentHisPatientTypeAlter = null;
        List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> currentPatientTypeWithPatientTypeAlter = null;
        List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> currentPatientTypes = null;
        HIS_MEDICINE_TYPE_TUT medicineTypeTutSelected;
        HIS_EXP_MEST_MATERIAL materialTypeTutSelected;
        List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> sereServsInTreatmentRaw = new List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>();
        internal List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> sereServWithTreatment = new List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>();
        internal List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> sereServWithMultilTreatment = new List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>();
        internal List<HIS_SERVICE_REQ_METY> serviceReqMetyInDay;
        internal List<V_HIS_SERVICE_REQ_METY> serviceReqMetyViewInDay;
        internal List<HIS_SERVICE_REQ_MATY> serviceReqMatyInDay;
        internal List<HIS_SERVICE_REQ_METY> serviceReqMetyInBatch;
        internal List<HIS_SERVICE_REQ_METY> serviceReqMetyInBatchWithMultilTreatment;
        internal List<HIS_SERVICE_REQ_MATY> serviceReqMatyInBatch;
        internal List<MediMatyTypeADO> mediMatyTypeADOBKs;
        internal List<MediMatyTypeADO> mediMatyTypeADOs;
        internal List<MediMatyTypeADO> mediMatyTypeADOsAlertInTreatment = new List<MediMatyTypeADO>();
        internal List<MediMatyTypeADO> mediMatyTypeADOsBlockInTreatment = new List<MediMatyTypeADO>();
        List<MedicineMaterialTypeComboADO> currentMediMateTypeComboADOs;
        internal List<D_HIS_MEDI_STOCK_2> mediMatyTypeAvailables;
        internal MediMatyTypeADO currentMedicineTypeADOForEdit;
        internal List<DMediStock1ADO> mediStockD1ADOs;
        internal List<V_HIS_MEDICINE_TYPE> currentMedicineTypes;
        internal List<V_HIS_MATERIAL_TYPE> currentMaterialTypes;
        internal HIS_ICD icdChoose { get; set; }
        public Inventec.Desktop.Common.Modules.Module currentModule;
        MOS.SDO.WorkPlaceSDO currentWorkPlace;
        List<MOS.EFMODEL.DataModels.V_HIS_SERVICE> serviceInPackages;
        List<V_HIS_SERVICE_PACKAGE> servicePackageByServices;
        OutPatientPresResultSDO outPrescriptionResultSDOs;
        InPatientPresResultSDO inPrescriptionResultSDOs;
        List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_7> currentPrescriptions;
        List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> currentMediStock = null;
        internal List<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK> currentMediStockNhaThuocSelecteds = null;
        List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> currentMediStockByHeaderCard;
        List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> currentMediStockByNotInHeaderCard;
        List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> currentWorkingMestRooms;
        AssignPrescriptionEditADO assignPrescriptionEditADO;
        MOS.EFMODEL.DataModels.HIS_SERVICE_REQ icdExam;

        int lastRowHandle = -1;
        ToolTipControlInfo lastInfo = null;
        GridColumn lastColumn = null;
        string[] periodSeparators = new string[] { "," };
        internal long oldExpMestId;
        internal HIS_EXP_MEST oldExpMest;
        internal HIS_SERVICE_REQ oldServiceReq;
        internal HIS_SERVICE_REQ ServiceReqEye;
        List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_BEAN_1> listMedicineBeanForEdits = new List<V_HIS_MEDICINE_BEAN_1>();
        List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_BEAN_1> listMaterialBeanForEdits = new List<V_HIS_MATERIAL_BEAN_1>();
        CommonParam paramCommon;

        internal PeriousExpMestListProcessor periousExpMestListProcessor;
        internal UserControl ucPeriousExpMestList;
        internal TreatmentFinishProcessor treatmentFinishProcessor;
        internal UserControl ucTreatmentFinish;
        internal PatientSelectProcessor patientSelectProcessor;
        internal UserControl ucPatientSelect;
        HisTreatmentBedRoomLViewFilter treatmentBedRoomLViewFilterInput;

        int theRequiredWidth = 900, theRequiredHeight = 130;
        bool isShowContainerMediMaty = false;
        bool isShowContainerTutorial = false;
        bool isShowContainerHTU = false;
        bool isShowContainerMediMatyForChoose = false;
        bool isShow = true;

        bool isStopEventChangeMultiDate;
        bool IsObligatoryTranferMediOrg = false;
        bool IsAcceptWordNotInData = false;
        string[] icdSeparators = new string[] { ";" };
        bool isAutoCheckIcd;
        string _TextIcdName = "";
        string _TextIcdNameCause = "";

        List<HIS_ICD> currentIcds;
        List<HIS_ICD> currentTranditionalIcds;
        List<TrackingADO> trackingADOs { get; set; }
        bool isInitTracking;
        List<HIS_ALLERGENIC> allergenics { get; set; }

        internal bool isMediMatyIsOutStock = false;

        //Bien luu thong tin don thuoc cu
        string provisionalDiagnosis;
        List<HIS_SERVICE_REQ> serviceReqPrints { get; set; }
        List<HIS_SERVICE_REQ> serviceReqPrintAlls { get; set; }
        List<HIS_EXP_MEST> expMestPrints { get; set; }
        List<HIS_EXP_MEST_MEDICINE> expMestMedicinePrints { get; set; }
        List<HIS_EXP_MEST_MATERIAL> expMestMaterialPrints { get; set; }
        List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineEditPrints { get; set; }
        List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialEditPrints { get; set; }
        List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_METY> serviceReqMetys { get; set; }
        List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_MATY> serviceReqMatys { get; set; }
        MOS.EFMODEL.DataModels.V_HIS_ROOM requestRoom;
        HIS_DHST currentDhst;
        bool isNotLoadMediMatyByMediStockInitForm;
        bool IsHandlerWhileOpionGroupSelectedIndexChanged;
        bool isNotLoadWhileChangeInstructionTimeInFirst;
        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        const string moduleLink = "HIS.Desktop.Plugins.AssignPrescriptionPK";
        internal List<MOS.EFMODEL.DataModels.HIS_TRACKING> Listtrackings { get; set; }
        MOS.Filter.HisServiceReqView7Filter currentPrescriptionFilter;
        List<V_HIS_MEDI_STOCK> mediStockAllows;
        int numberDisplaySeperateFormatAmount = 0;
        long ContructorIntructionTime;

        bool isNotShowfrmEyeInfo = false;
        bool vlStateChkEyeInfo = false;
        bool chkNotShowExpMestTypeDTT = true;
        AdviseFormADO currentAdviseFormADO;
        bool isConfirmYes = false;

        bool navBarChongChiDinhInfoState = true;
        bool navBarDHSTInfoState = true;
        object resultDataPrescription;
        internal PopupMenu _Menu = null;
        List<HIS_EMR_COVER_CONFIG> LstEmrCoverConfig;
        List<HIS_EMR_COVER_CONFIG> LstEmrCoverConfigDepartment;
        HIS_TREATMENT treatmentData;
        MediRecordMenuPopupProcessor emrMenuPopupProcessor = null;

        internal List<HIS_SERVICE_CONDITION> workingServiceConditions;

        List<V_HIS_SERVICE_METY> serviceMetyByServices;
        List<V_HIS_SERVICE_MATY> serviceMatyByServices;

        List<HIS.Desktop.Plugins.AssignPrescriptionPK.ADO.IcdADO> icdSubcodeAdoChecks;
        HIS.Desktop.Plugins.AssignPrescriptionPK.ADO.IcdADO subIcdPopupSelect;
        bool isNotProcessWhileChangedTextSubIcd;

        GridHitInfo downHitInfo = null;
        internal bool bIsSelectMultiPatientProcessing = false;

        internal V_HIS_TREATMENT VHistreatment;

        CommonParam paramSaveList;
        bool successSaveList;
        EpaymentDepositResultSDO epaymentDepositResultSDO;

        List<HIS_DEPARTMENT_TRAN> lstDepartmentTran = new List<HIS_DEPARTMENT_TRAN>();
        List<HIS_CO_TREATMENT> lstCoTreatment = new List<HIS_CO_TREATMENT>();

        bool NoEdit = true;
        private List<string> lstModuleLinkApply;
        public string msgTuVong { get; set; }
        internal List<HIS_EXP_MEST_REASON> lstExpMestReasons;
        List<V_HIS_SERVICE> lstService;
        //Height ReloadModuleByInputData
        private Size OldSizeUcSelectpatient = new Size(0, 0);
        bool IsValidForSave = true;
        internal List<OutPatientPresADO> lstOutPatientPres = new List<OutPatientPresADO>();
        bool IsContinue = false;
        TuberCulosisADO tuberCulosisADO;
        bool IsSucessTuberCulosis;
        internal decimal? PresAmount;
        List<long> ConfigIds = new List<long>();
        List<ConfigADO> lstConfig;
        DateTime dteCommonParam { get; set; }
        long dteTreatmentFinishIntructionTime { get; set; }
        bool? IsOpen { get; set; }
        bool IsStateCase1Dhst { get; set; }
        Size sizeListPatient { get; set; }
        Size sizeExpMest { get; set; }
        Size sizeInforPatient { get; set; }
        CheckIcdManager checkIcdManager { get; set; }
        HIS_TREATMENT currentTreatment { get; set; }
        List<HIS_OBEY_CONTRAINDI> ObeyContraindiEdit { get; set; }
        List<HIS_OBEY_CONTRAINDI> ObeyContraindiSave { get; set; }

        public List<string> icdsWarning = new List<string>();
        public bool IsWarned;
        internal List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> SereServInDay { get; set; }
        bool IsMultilPatient;
        internal List<HIS_MEDICINE_SERVICE> medicineService { get; set; }
        internal List<HIS_MEDICINE_SERVICE> medicineServiceTest { get; set; }
        internal List<V_HIS_SERE_SERV_TEIN_1> sereServTeinResultTest { get; set; }
        internal List<V_HIS_SERE_SERV_TEIN_1> sereServTeinKidney { get; set; }
        internal bool IsSaveOverResultReasonTest = false;
        HIS_DHST dhst;
        List<HIS_DHST> dhstlist { get; set; }
        Dictionary<string, long> dicMediMateAssignPres { get; set; }
        long ServiceReqIdPrevios { get; set; }
        bool PrescriptionPrevious { get; set; }
        decimal transferTotal { get; set; }
        List<V_HIS_SERVICE_REQ_7> serviceReqPreExpmestAll { get; set; }
        internal string TreatmentMethod { get; set; }
        List<HIS_EXP_MEST_MEDICINE> ListExpMestMedicineAntibioticRequired = new List<HIS_EXP_MEST_MEDICINE>();
        List<HIS_ANCILLARY_SERV_PATY> ListAncillaryServPaty = new List<HIS_ANCILLARY_SERV_PATY>();
        List<HIS_EXP_MEST> ListExpMestResult { get; set; }
        private System.Windows.Forms.Timer timerReloadTreatmentFinishTime { get; set; }
        bool isCheckAssignServiceSimultaneityOption = false;

        internal IcdProcessor icdYhctProcessor;
        internal UserControl ucIcdYhct;
        internal SecondaryIcdProcessor subIcdYhctProcessor;
        internal UserControl ucSecondaryIcdYhct;
        internal List<AlertLogADO> AlertLogsSdo = new List<AlertLogADO>();
        decimal? chiSoMLCT;
        List<HIS_MEDICINE_TYPE_TUT> medicineTypeTutFilters { get; set; }
        HIS_MEDICINE_TYPE_TUT medicineTypehtu { get; set; }
        System.Windows.Forms.Timer timerInitFormAssignPrescription { get; set; }
        bool shouldAutoUpdateMemHtu = false;
        #endregion

        #region Construct
        public frmAssignPrescription(Inventec.Desktop.Common.Modules.Module module, AssignPrescriptionADO data, HisTreatmentBedRoomLViewFilter treatmentBedRoomLViewFilter = null)
            : base(module)
        {
            try
            {
                LogSystem.Debug("frmAssignPrescription InitializeComponent.1");
                InitializeComponent();
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
                this.treatmentBedRoomLViewFilterInput = treatmentBedRoomLViewFilter;
                this.actionType = data.AssignPrescriptionEditADO != null ? GlobalVariables.ActionEdit : GlobalVariables.ActionAdd;
                this.actionTypePrint = data.AssignPrescriptionEditADO != null ? GlobalVariables.ActionEdit : GlobalVariables.ActionAdd;
                this.currentModule = module;
                this.processDataResult = data.DgProcessDataResult;
                this.processRefeshIcd = data.DgProcessRefeshIcd;
                this.processWhileAutoTreatmentEnd = data.DlgWhileAutoTreatmentEnd;
                this.treatmentId = data.TreatmentId;
                this.expMestTemplateId = data.ExpMestTemplateId;
                this.treatmentCode = data.TreatmentCode;
                if (data.ServiceReqId > 0)
                    this.serviceReqParentId = data.ServiceReqId;
                this.isInKip = data.IsInKip;
                GlobalStore.IsCabinet = data.IsCabinet;
                GlobalStore.IsTreatmentIn = data.IsExecutePTTT;
                GlobalStore.IsExecutePTTT = data.IsExecutePTTT;
                this.patientName = data.PatientName;
                this.patientDob = data.PatientDob;
                this.genderName = data.GenderName;
                this.patientId = data.PatientId;
                if (data.Tracking != null)
                {
                    this.Listtrackings = new List<HIS_TRACKING>();
                    this.Listtrackings.Add(data.Tracking);
                }

                if (this.isInKip)
                    this.currentSereServInEkip = data.SereServ;
                else
                    this.currentSereServ = data.SereServ;
                this.isAutoCheckExpend = data.IsAutoCheckExpend;
                this.assignPrescriptionEditADO = data.AssignPrescriptionEditADO;
                this.icdExam = data.IcdExam;
                this.currentDhst = data.Dhst;
                this.sereServsInTreatmentRaw = data.SereServsInTreatment;
                this.provisionalDiagnosis = data.ProvisionalDiagnosis;
                this.ContructorIntructionTime = data.IntructionTime;
                Resources.ResourceLanguageManager.LanguagefrmAssignPrescription = new ResourceManager("HIS.Desktop.Plugins.AssignPrescriptionPK.Resources.Lang", typeof(HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription.frmAssignPrescription).Assembly);
                this.InitAssignPresctiptionType();
                this.InitDataForPrescriptionEdit();
                HisConfigCFG.LoadConfig();
                Inventec.Common.Logging.LogSystem.Debug(
                    Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data.TreatmentId), data.TreatmentId)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ContructorIntructionTime), ContructorIntructionTime)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentModule.RoomId), currentModule.RoomId)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentModule.RoomTypeId), currentModule.RoomTypeId)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.InPatientPrescription__ShowRoundAvailableAmount), HisConfigCFG.InPatientPrescription__ShowRoundAvailableAmount)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.IsUsingWarningHeinFee), HisConfigCFG.IsUsingWarningHeinFee)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.IsWarningOverTotalPatientPrice), HisConfigCFG.IsWarningOverTotalPatientPrice)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.IsWarringUseDayAndExpTimeBHYT), HisConfigCFG.IsWarringUseDayAndExpTimeBHYT)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.IsDontPresExpiredTime), HisConfigCFG.IsDontPresExpiredTime)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.isPrescriptionSplitOutMediStock), HisConfigCFG.isPrescriptionSplitOutMediStock)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.IsNotAllowingExpendWithoutHavingParent), HisConfigCFG.IsNotAllowingExpendWithoutHavingParent)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.BlockingInteractiveGrade), HisConfigCFG.BlockingInteractiveGrade)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.ManyDayPrescriptionOption), HisConfigCFG.ManyDayPrescriptionOption)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.IsWarningOddConvertAmount), HisConfigCFG.IsWarningOddConvertAmount)
                    );

                //InitMultipleThread();
                SetDataText();
                if (!GlobalStore.IsCabinet && (HisConfigCFG.icdServiceHasCheck == 1 || HisConfigCFG.icdServiceHasCheck == 2))
                {
                    this.lciPDDT.Enabled = true;
                }
                else
                {
                    this.lciPDDT.Enabled = false;
                }
                LogSystem.Debug("frmAssignPrescription InitializeComponent.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitUcIcdYhct()
        {
            try
            {
                icdYhctProcessor = new HIS.UC.Icd.IcdProcessor();
                HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
                ado.DelegateNextFocus = NextForcusOutYhct;
                ado.IsUCCause = false;
                ado.Width = 440;
                ado.LabelTextSize = 95;
                ado.Height = 24;
                ado.DataIcds = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_TRADITIONAL == 1).ToList();
                ado.AutoCheckIcd = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<String>("HIS.Desktop.Plugins.AutoCheckIcd") == "1";
                ado.LblIcdMain = "CĐ YHCT:";
                ado.ToolTipsIcdMain = "Chẩn đoán y học cổ truyền";
                ucIcdYhct = (UserControl)icdYhctProcessor.Run(ado);

                if (ucIcdYhct != null)
                {
                    this.pnIcdTranditional.Controls.Add(ucIcdYhct);
                    ucIcdYhct.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void NextForcusOutYhct()
        {
            try
            {
                if (subIcdYhctProcessor != null && ucSecondaryIcdYhct != null && ucSecondaryIcdYhct.Visible == true)
                {
                    ModuleControlProcess controlProcess = new ModuleControlProcess(true);
                    ModuleControls = controlProcess.GetControls(ucSecondaryIcdYhct);
                    int count = 0;
                    foreach (var itemCtrl in ModuleControls)
                    {
                        if (itemCtrl.ControlName == "txtIcdSubCode")
                        {
                            if (itemCtrl.IsVisible)
                            {
                                count = count + 1;
                            }
                        }
                        else if (itemCtrl.ControlName == "txtIcdText")
                        {
                            if (itemCtrl.IsVisible)
                            {
                                count = count + 1;
                            }
                        }
                    }

                    if (count > 0)
                    {
                        subIcdYhctProcessor.FocusControl(ucSecondaryIcdYhct);
                    }
                }
                else
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitUcSecondaryIcdYhct()
        {
            try
            {
                var icdYhct = BackendDataWorker.Get<V_HIS_ICD>().Where(o => o.IS_TRADITIONAL == 1).ToList();
                subIcdYhctProcessor = new SecondaryIcdProcessor(new CommonParam(), icdYhct);
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.DelegateGetIcdMain = GetIcdMainCodeYhct;
                ado.hisTreatment = this.currentTreatment;
                ado.Width = 440;
                ado.TextSize = 95;
                ado.Height = 24;
                ado.TextLblIcd = "CĐ YHCT phụ:";
                ado.TootiplciIcdSubCode = "Chẩn đoán y học cổ truyền phụ";
                ado.TextNullValue = "Nhấn F1 để chọn bệnh";
                ado.ViewHisIcds = icdYhct;
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                ucSecondaryIcdYhct = (UserControl)subIcdYhctProcessor.Run(ado);

                if (ucSecondaryIcdYhct != null)
                {
                    this.pnSubIcdTranditional.Controls.Add(ucSecondaryIcdYhct);
                    ucSecondaryIcdYhct.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private string GetIcdMainCodeYhct()
        {
            string mainCode = "";
            try
            {
                if (this.icdYhctProcessor != null && this.ucIcdYhct != null)
                {
                    var icdValue = this.icdYhctProcessor.GetValue(this.ucIcdYhct);
                    if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                    {
                        mainCode = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return mainCode;
        }

        private void LoadDataFromRam()
        {
            try
            {
                lstService = BackendDataWorker.Get<V_HIS_SERVICE>().ToList();
                this.lstExpMestReasons = new List<HIS_EXP_MEST_REASON>();
                this.lstExpMestReasons = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EXP_MEST_REASON>().Where(o => o.IS_ACTIVE == GlobalVariables.CommonNumberTrue).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                //StopTimer(currentModule.ModuleLink, "SetTextLanguage");
                ////Khoi tao doi tuong resource
                //Resources.ResourceLanguageManager.LanguagefrmAssignPrescription = new ResourceManager("HIS.Desktop.Plugins.AssignPrescriptionPK.Resources.Lang", typeof(frmAssignPrescription).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.bar1.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.barbtnSaveShortcut.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.barbtnSaveShortcut.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.barbtnSaveAndPrintShortcut.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.barbtnSaveAndPrintShortcut.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.barbtnPrintShortcut.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.barbtnPrintShortcut.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.barbtnNew.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.barbtnNew.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.bbtnF2.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.bbtnF2.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.bbtnBoSung.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.bbtnBoSung.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.bbtnF3.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.bbtnF3.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.barbtnF4.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.barbtnF4.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.bbtnF5.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.bbtnF5.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.bbtnF6.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.bbtnF6.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.bbtnF7.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.bbtnF7.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControl6.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.chkPDDT.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.chkPDDT.Properties.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());

                this.layoutControl10.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControl10.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.chkSignForDDT.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.chkSignForDDT.Properties.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.chkSignForDTT.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.chkSignForDTT.Properties.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.chkSignForDPK.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.chkSignForDPK.Properties.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciForchkSignForDDT.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciForchkSignForDDT.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciForchkSignForDTT.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciForchkSignForDTT.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciForchkSignForDPK.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciForchkSignForDPK.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.layoutControl9.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControl9.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.labelControl13.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.labelControl13.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.labelControl5.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.labelControl5.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.labelControl12.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.labelControl12.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.labelControl14.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.labelControl14.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.labelControl10.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.labelControl10.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());

                this.btnTestResult.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnTestResult.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.cboHtu.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignPrescription.cboHtu.Properties.NullText", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.labelControl6.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.labelControl6.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.labelControl3.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.labelControl3.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.labelControl1.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.labelControl1.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.labelControl9.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.labelControl9.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.labelControl8.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.labelControl8.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.btnConnectBloodPressure.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnConnectBloodPressure.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lcgDHST.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lcgDHST.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciExecuteTime.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciExecuteTime.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciPulse.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciPulse.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciBloodPressure.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciBloodPressure.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciWeight.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciWeight.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciHeight.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciHeight.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciNote.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciNote.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                //this.lciBMIDisplay.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciBMIDisplay.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                //this.lciLeatherArea.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciLeatherArea.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.layoutControlItem32.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControlItem32.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.layoutControlItem32.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControlItem32.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciTemperature.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciTemperature.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciSpo2.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciSpo2.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciBreathRate.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciBreathRate.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciChest.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciChest.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciBelly.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciBelly.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciBMI.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciBMI.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.btnAddTutorial.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnAddTutorial.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.btnCreateVBA.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnCreateVBA.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.layoutControl8.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControl8.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.grcDelete__MedicinePage.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcDelete__MedicinePage.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.grcDelete__MedicinePage.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcDelete__MedicinePage.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.grcNumOrder.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcNumOrder.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.grcManuMedicineName__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcManuMedicineName__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.grcConcentra__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcConcentra__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.grcMaHoatChat.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcMaHoatChat.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.grcUnit__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcUnit__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.grcUnit__TabMedicine.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcUnit__TabMedicine.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.grcAmount__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcAmount__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.grcAmount__TabMedicine.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcAmount__TabMedicine.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.grcPatientType__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcPatientType__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.grcExpend__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcExpend__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.grcExpend__TabMedicine.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcExpend__TabMedicine.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.grcIsExpendType.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcIsExpendType.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.grcIsExpendType.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcIsExpendType.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.grcIsOutKtcFee__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcIsOutKtcFee__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.grcIsOutKtcFee__TabMedicine.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcIsOutKtcFee__TabMedicine.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.grcTutorial__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcTutorial__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.grcTutorial__TabMedicine.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcTutorial__TabMedicine.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.gridColumnManyDate.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.gridColumnManyDate.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.gridColumnEquipment.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.gridColumnEquipment.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.grcTotalPrice__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcTotalPrice__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.grcMediStockExpMest__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcMediStockExpMest__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.grcTocDoTruyen.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcTocDoTruyen.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.gridColumnSERVICE_CONDITION_NAME.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.gridColumnSERVICE_CONDITION_NAME.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.gridColumnSERVICE_CONDITION_NAME.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.gridColumnSERVICE_CONDITION_NAME.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.grcUseForm__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcUseForm__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.repositoryItemcboMedicineUseForm.NullText = Inventec.Common.Resource.Get.Value("frmAssignPrescription.repositoryItemcboMedicineUseForm.NullText", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.grcKHBHYT__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcKHBHYT__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.grcUseTimeTo__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcUseTimeTo__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.grcPrice__TabMedicine.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcPrice__TabMedicine.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.grcLoaiHaoPhi.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.grcLoaiHaoPhi.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.repositoryItemGridLookupEditExpendType_Enable.NullText = Inventec.Common.Resource.Get.Value("frmAssignPrescription.repositoryItemGridLookupEditExpendType_Enable.NullText", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.repositoryItemcboPatientType_TabMedicine.NullText = Inventec.Common.Resource.Get.Value("frmAssignPrescription.repositoryItemcboPatientType_TabMedicine.NullText", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.repositoryItemcboPatientType_TabMedicine_GridLookUp.NullText = Inventec.Common.Resource.Get.Value("frmAssignPrescription.repositoryItemcboPatientType_TabMedicine_GridLookUp.NullText", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.repositoryItemcboPatientType_TabMedicine_GridLookUp__Disable.NullText = Inventec.Common.Resource.Get.Value("frmAssignPrescription.repositoryItemcboPatientType_TabMedicine_GridLookUp__Disable.NullText", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.repositoryItemGridLookUpEditEquipmentSet__Enabled.NullText = Inventec.Common.Resource.Get.Value("frmAssignPrescription.repositoryItemGridLookUpEditEquipmentSet__Enabled.NullText", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.repositoryItemGridLookUpEditEquipmentSet__Disabled.NullText = Inventec.Common.Resource.Get.Value("frmAssignPrescription.repositoryItemGridLookUpEditEquipmentSet__Disabled.NullText", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.repositoryItemGridLookupEditExpendType_Disable.NullText = Inventec.Common.Resource.Get.Value("frmAssignPrescription.repositoryItemGridLookupEditExpendType_Disable.NullText", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.repositoryItemGridLookupEditExpendTypeHasValue_Enable.NullText = Inventec.Common.Resource.Get.Value("frmAssignPrescription.repositoryItemGridLookupEditExpendTypeHasValue_Enable.NullText", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.navBarControlChongChiDinhInfo.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.navBarControlChongChiDinhInfo.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.nbgMail.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.nbgMail.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.layoutControlPrintAssignPrescriptionExt.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControlPrintAssignPrescriptionExt.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.chkPreviewBeforePrint.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.chkPreviewBeforePrint.Properties.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.chkEyeInfo.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.chkEyeInfo.Properties.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.chkPrint.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.chkPrint.Properties.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControl5.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.chkMultiIntructionTimeForMedi.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.chkMultiIntructionTimeForMedi.Properties.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.layoutControlItem24.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControlItem24.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.layoutControlItem24.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControlItem24.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.chkShowLo.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.chkShowLo.Properties.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());

                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControl1.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControl7.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.txtTocDoTho.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.txtTocDoTho.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciSang.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciSang.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciTrua.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciTrua.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciToi.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciToi.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciChieu.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciChieu.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciFortxtThoiGianTho.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciFortxtThoiGianTho.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciFortxtTocDoTho.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciFortxtTocDoTho.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciFortxtTocDoTho.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciFortxtTocDoTho.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.cboMedicineUseForm.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignPrescription.cboMedicineUseForm.Properties.NullText", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.btnAddTutorial.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnAddTutorial.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnAdd.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciSoLuongNgay.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciSoLuongNgay.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciTocDoTruyen.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciTocDoTruyen.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciHuongDan.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciHuongDan.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciHtu.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciHtu.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciMedicineUseForm.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciMedicineUseForm.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciAmount.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciAmount.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciFortxtPreviousUseDay.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciFortxtPreviousUseDay.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciFortxtPreviousUseDay.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciFortxtPreviousUseDay.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.chkPreKidneyShift.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.chkPreKidneyShift.Properties.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.chkHomePres.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.chkHomePres.Properties.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lcCause.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lcCause.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.chkEditIcdCause.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.chkEditIcdCause.Properties.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciIcdTextCause.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciIcdTextCause.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciIcdTextCause.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciIcdTextCause.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.cboNhaThuoc.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignPrescription.cboNhaThuoc.Properties.NullText", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.cboPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignPrescription.cboPatientType.Properties.NullText", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.btnDichVuHenKham.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnDichVuHenKham.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.btnDichVuHenKham.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnDichVuHenKham.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.cboEquipment.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignPrescription.cboEquipment.Properties.NullText", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.btnBoSungPhacDo.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnBoSungPhacDo.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.btnBoSungPhacDo.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnBoSungPhacDo.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.cboPhieuDieuTri.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignPrescription.cboPhieuDieuTri.Properties.NullText", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.txtMediMatyForPrescription.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmAssignPrescription.txtMediMatyForPrescription.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.layoutControlPrintAssignPrescription.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControlPrintAssignPrescription.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControl2.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.chkMultiIntructionTime.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.chkMultiIntructionTime.Properties.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciDateEditor.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciDateEditor.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciDateEditor.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciDateEditor.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControl3.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lcgPrevousPrescription.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lcgPrevousPrescription.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControl4.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciDob.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciDob.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciPatientName.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciPatientName.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciHeinCardNumberInfo.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciHeinCardNumberInfo.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciGenderName.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciGenderName.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciPatientTypeName.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciPatientTypeName.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciTreatmentTypeName.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciTreatmentTypeName.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciTreatmentTypeName.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciTreatmentTypeName.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.layoutControl15.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControl15.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.txtIcdText.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmAssignPrescription.txtIcdText.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciIcdSubCode.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciIcdSubCode.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciIcdSubCode.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciIcdSubCode.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.layoutControl13.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControl13.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.chkEditIcd.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.chkEditIcd.Properties.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciIcdText.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciIcdText.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciIcdText.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciIcdText.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.txtUnitOther.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmAssignPrescription.txtUnitOther.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.txtMedicineTypeOther.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmAssignPrescription.txtMedicineTypeOther.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnNew.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.btnNew.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnNew.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.btnShowDetail.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnShowDetail.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.btnShowDetail.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnShowDetail.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.txtAdvise.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmAssignPrescription.txtAdvise.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.btnSaveAndPrint.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnSaveAndPrint.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.btnSaveAndPrint.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnSaveAndPrint.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.cboExpMestTemplate.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignPrescription.cboExpMestTemplate.Properties.NullText", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.cboMediStockExport.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignPrescription.cboMediStockExport.Properties.NullText", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.btnSaveTemplate.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnSaveTemplate.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.btnSaveTemplate.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnSaveTemplate.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnSave.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.btnSave.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.btnSave.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.cboUser.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignPrescription.cboUser.Properties.NullText", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciMediStockExpMest.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciMediStockExpMest.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciLadder.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciLadder.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lcitxtLoginName.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lcitxtLoginName.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciExpMestTemplate.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciExpMestTemplate.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciPhieuDieuTri.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciPhieuDieuTri.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciEquipment.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciEquipment.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciPatientType.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciPatientType.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControlItem18.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciHomePres.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciHomePres.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciForchkPreKidneyShift.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciForchkPreKidneyShift.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciForspinKidneyCount.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciForspinKidneyCount.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciForspinKidneyCount.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciForspinKidneyCount.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciForchkShowLo.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciForchkShowLo.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciForchkShowLo.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciForchkShowLo.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciFortxtProvisionalDiagnosis.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciFortxtProvisionalDiagnosis.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciPrintAssignPrescription.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciPrintAssignPrescription.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciForchkThongTinMat.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciForchkThongTinMat.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciForlblDaDong.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciForlblDaDong.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciTongTien.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciTongTien.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.layoutControlItem26.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControlItem26.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.layoutControlItem29.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControlItem29.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciLoiDanBacSi.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciLoiDanBacSi.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciForlblConThua.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciForlblConThua.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciForlblChiPhiBNPhaiTra.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciForlblChiPhiBNPhaiTra.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.layoutControlItem28.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControlItem28.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.layoutControlItem20.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControlItem20.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControlItem20.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciPDDT.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciPDDT.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciPDDT.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciPDDT.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciInteractionReason.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciInteractionReason.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.licUseTime.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.licUseTime.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.lciExpMestReason.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciExpMestReason.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.customGridViewWithFilterMultiColumn3.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("frmAssignPrescription.customGridViewWithFilterMultiColumn3.OptionsFind.FindNullPrompt", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.customGridViewWithFilterMultiColumn2.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("frmAssignPrescription.customGridViewWithFilterMultiColumn2.OptionsFind.FindNullPrompt", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.layoutControl21.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.layoutControl21.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.gridColumn17.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.gridColumn18.Caption = Inventec.Common.Resource.Get.Value("frmAssignPrescription.gridColumn18.Caption", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.cboExpMestReason.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignPrescription.cboExpMestReason.Properties.NullText", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                //this.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.rdOpionGroup.Properties.Items[0].Description = Inventec.Common.Resource.Get.Value("frmAssignPrescription.rdoItem1.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.rdOpionGroup.Properties.Items[1].Description = Inventec.Common.Resource.Get.Value("frmAssignPrescription.rdoItem2.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                this.rdOpionGroup.Properties.Items[2].Description = Inventec.Common.Resource.Get.Value("frmAssignPrescription.rdoItem3.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        void InitControlState()
        {
            try
            {
                pbOpen.Image = global::HIS.Desktop.Plugins.AssignPrescriptionPK.Properties.Resources.arrow_down;
                pbClose.Image = global::HIS.Desktop.Plugins.AssignPrescriptionPK.Properties.Resources.arrow_up;
                isNotLoadWhileChangeControlStateInFirst = true;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                //this.lciLoiDanBacSi.MinSize = new Size(lciLoiDanBacSi.Width, lciLoiDanBacSi.Height);
                //this.lciIcdTranditional.MinSize = new Size(lciForchkPreKidneyShift.Width + lciForchkPreKidneyShift.Width + lciForspinKidneyCount.Width, lciIcdTranditional.Height);
                sizeInforPatient = new Size(layoutControlItem21.Width, layoutControlItem21.Height - 10);
                sizeListPatient = new Size(layoutControlItem15.Width, layoutControlItem15.Height);
                sizeExpMest = new Size(lciUCBottomPanel.Width, lciUCBottomPanel.Height);
                bool IsValueArrowUp = false;
                bool IsValueArrowDown = false;
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstan.chkPrint)
                        {
                            chkPrint.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstan.chkSign)
                        {
                            chkSignForDPK.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstan.chkSignForDTT)
                        {
                            chkSignForDTT.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstan.chkSignForDDT)
                        {
                            chkSignForDDT.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstan.chkEyeInfo)
                        {
                            this.vlStateChkEyeInfo = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstan.chkNotShowExpMestTypeDTT)
                        {
                            this.chkNotShowExpMestTypeDTT = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstan.chkPreviewBeforePrint)
                        {
                            chkPreviewBeforePrint.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstan.lcgChongChiDinhInfo)
                        {
                            this.navBarChongChiDinhInfoState = item.VALUE == "1";
                            navBarControlChongChiDinhInfo.OptionsNavPane.NavPaneState = this.navBarChongChiDinhInfoState ? DevExpress.XtraNavBar.NavPaneState.Expanded : DevExpress.XtraNavBar.NavPaneState.Collapsed;

                            //System.Threading.Thread.Sleep(100);
                        }
                        else if (item.KEY == pbClose.Name)
                        {
                            if (item.VALUE == "1")
                            {
                                IsOpen = false;
                                IsValueArrowUp = true;
                                pbClose.Image = null;
                                pbOpen.Image = global::HIS.Desktop.Plugins.AssignPrescriptionPK.Properties.Resources.arrow_down;
                            }
                        }
                        else if (item.KEY == pbOpen.Name)
                        {
                            if (item.VALUE == "1")
                            {
                                IsOpen = true;
                                IsValueArrowDown = true;
                                pbClose.Image = global::HIS.Desktop.Plugins.AssignPrescriptionPK.Properties.Resources.arrow_up;
                                pbOpen.Image = null;
                            }
                        }
                        else if (item.KEY == ControlStateConstan.lcgDHSTInfo)
                        {
                            this.navBarDHSTInfoState = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstan.AdviseFormData)
                        {
                            try
                            {
                                if (!String.IsNullOrEmpty(item.VALUE))
                                {
                                    if (currentAdviseFormADO == null)
                                        currentAdviseFormADO = new AdviseFormADO();

                                    var arrAdviseForm = item.VALUE.Split(new string[] { "|" }, StringSplitOptions.None);
                                    if (arrAdviseForm != null && arrAdviseForm.Count() > 2)
                                    {
                                        currentAdviseFormADO.IncludeMaterial = arrAdviseForm[0] == "1" ? (bool?)true : null;
                                        string strMedicineUseFormIds = arrAdviseForm[1];
                                        if (!String.IsNullOrEmpty(strMedicineUseFormIds) && strMedicineUseFormIds.Trim() != ",")
                                        {
                                            var arrMedicineUseFormIds = strMedicineUseFormIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                            currentAdviseFormADO.MedicineUseFormIds = (arrMedicineUseFormIds != null && arrMedicineUseFormIds.Count > 0) ? (from m in arrMedicineUseFormIds select Inventec.Common.TypeConvert.Parse.ToInt64(m)).ToList() : null;
                                        }
                                        string strExpMestTypeIds = arrAdviseForm[2];
                                        if (!String.IsNullOrEmpty(strExpMestTypeIds) && strExpMestTypeIds.Trim() != ",")
                                        {
                                            var arrExpMestTypeIds = strExpMestTypeIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                            currentAdviseFormADO.ExpMestTypeIds = (arrExpMestTypeIds != null && arrExpMestTypeIds.Count > 0) ? (from m in arrExpMestTypeIds select Inventec.Common.TypeConvert.Parse.ToInt64(m)).ToList() : null;
                                        }
                                        currentAdviseFormADO.AutoGetHomePres = arrAdviseForm[3] == "1" ? (bool?)true : null;
                                    }
                                }
                            }
                            catch (Exception ex1)
                            {
                                Inventec.Common.Logging.LogSystem.Warn(ex1);
                            }
                        }
                        else if (item.KEY == ControlStateConstan.chkPDDT)
                        {
                            chkPDDT.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == gridControlConfig.Name)
                        {
                            if (!string.IsNullOrEmpty(item.VALUE))
                            {
                                var lstStr = item.VALUE.Split(';').ToList();
                                foreach (var str in lstStr)
                                {
                                    ConfigIds.Add(Int64.Parse(str));
                                }
                            }
                        }
                    }
                    if (IsOpen == null)
                    {
                        IsStateCase1Dhst = true;
                        if (!this.navBarDHSTInfoState)
                        {
                            this.layoutControlItemDHSTInfo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                            layoutControlItem15.Size = new Size(layoutControlItem15.Width, layoutControlItem15.Height + layoutControlItemDHSTInfo.Height);
                            pbClose.Image = global::HIS.Desktop.Plugins.AssignPrescriptionPK.Properties.Resources.arrow_up;
                            pbOpen.Image = null;
                        }
                        else if (this.navBarDHSTInfoState && this.layoutControlItemDHSTInfo.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                        {
                            this.layoutControlItemDHSTInfo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                            layoutControlItem15.Size = new Size(layoutControlItem15.Width, layoutControlItem15.Height - layoutControlItemDHSTInfo.Height);
                        }
                    }
                    else
                    {
                        IsStateCase1Dhst = false;
                        if (IsValueArrowUp && IsValueArrowDown)
                        {
                            pbOpen.Image = global::HIS.Desktop.Plugins.AssignPrescriptionPK.Properties.Resources.arrow_down;
                            pbClose.Image = global::HIS.Desktop.Plugins.AssignPrescriptionPK.Properties.Resources.arrow_up;
                        }
                        VisibleDhst();
                    }
                }
                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Private method
        private void CheckAssignServiceSimultaneityOption()
        {

            try
            {
                if (this.actionType == GlobalVariables.ActionAdd)
                {
                    List<MediMatyTypeADO> serviceCheckeds__Send = this.mediMatyTypeADOs;
                    this.btnSave.Enabled = btnSaveAndPrint.Enabled = (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0);
                }
                else if (this.actionType == GlobalVariables.ActionEdit)
                {
                    this.btnSave.Enabled = btnSaveAndPrint.Enabled = true;
                }
                else
                {
                    this.btnSave.Enabled = this.btnAdd.Enabled = btnSaveAndPrint.Enabled = false;
                }
                isCheckAssignServiceSimultaneityOption = false;
                if ((HisConfigCFG.ASSIGN_SERVICE_SIMULTANEITY_OPTION != "1" && HisConfigCFG.ASSIGN_SERVICE_SIMULTANEITY_OPTION != "2") || cboUser.EditValue == null || intructionTimeSelecteds == null || intructionTimeSelecteds.Count == 0)
                    return;
                CommonParam param = new CommonParam();
                HisServiceReqCheckSereTimesSDO sdo = new HisServiceReqCheckSereTimesSDO();
                sdo.TreatmentId = treatmentId;
                sdo.Loginnames = new List<string> { cboUser.EditValue.ToString() };
                sdo.SereTimes = intructionTimeSelecteds;
                var CheckSereTimes = new BackendAdapter(param).Post<bool>("api/HisServiceReq/CheckSereTimes", ApiConsumers.MosConsumer, sdo, ProcessLostToken, param);
                if (!CheckSereTimes)
                {
                    if (HisConfigCFG.ASSIGN_SERVICE_SIMULTANEITY_OPTION == "1")
                    {
                        isCheckAssignServiceSimultaneityOption = true;
                        btnSave.Enabled = btnSaveAndPrint.Enabled = false;
                        MessageManager.Show(this, param, CheckSereTimes);
                    }
                    else if (HisConfigCFG.ASSIGN_SERVICE_SIMULTANEITY_OPTION == "2")
                    {
                        if (XtraMessageBox.Show(param.GetMessage(), "Thông báo", MessageBoxButtons.YesNo) != DialogResult.Yes)
                        {
                            isCheckAssignServiceSimultaneityOption = true;
                            btnSave.Enabled = btnSaveAndPrint.Enabled = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void SetDataText()
        {
            try
            {
                if (this.currentModule != null)
                {
                    this.currentWorkPlace = WorkPlace.GetWorkPlace(this.currentModule);
                    if (this.currentWorkPlace == null)
                        LogSystem.Warn("Get current WorkPlace theo phòng làm việc hiện tại không có dữ liệu, RoomId = " + GetRoomId() + " | RoomTypeId = " + GetRoomTypeId());

                    if (!String.IsNullOrEmpty(this.currentModule.text))
                        this.Text = this.currentModule.text;
                }
                if (GlobalStore.IsCabinet)
                {
                    if (GlobalStore.IsTreatmentIn)
                    {
                        this.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.Text.Cabinet.IsTreatmentIn", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    }
                    else
                    {
                        this.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.Text.Cabinet.IsTreatmentOut", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    }
                }
                else if (GlobalStore.IsTreatmentIn)
                    this.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.Text.IsTreatmentIn", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                else
                    this.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.Text.IsTreatmentOut", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #region Old

        #endregion
        #region Load
        private void frmAssignPrescription_Load(object sender, EventArgs e)
        {
            try
            {
                LogSystem.Debug("frmAssignPrescription_Load Starting.... 1");
                this.SetCaptionByLanguageKeyNew();
                LogSystem.Debug("frmAssignPrescription_Load. 1");
                WaitingManager.Show();
                InitTimerReloadTreatmentFinishTime();
                this.LoadVHisTreatment();
                LogSystem.Debug("frmAssignPrescription_Load. 2");
                InitMultipleThread();
                LogSystem.Debug("frmAssignPrescription_Load. 3");
                LoadDataFromRam();
                LogSystem.Debug("frmAssignPrescription_Load. 4");
                this.AddBarManager(this.barManager1);
                this.isNotLoadWhileChangeInstructionTimeInFirst = true;
                this.isInitTracking = true;
                this.dicMediMateAssignPres = new Dictionary<string, long>();
                this.gridControlServiceProcess.ToolTipController = this.tooltipService;
                this.ResetDataForm();
                LogSystem.Debug("frmAssignPrescription_Load. 6");
                this.SetDefaultData();
                LogSystem.Debug("frmAssignPrescription_Load. 7");
                this.SetDefaultUC();

                LogSystem.Debug("frmAssignPrescription_Load. 8");
                LogSystem.Debug("frmAssignPrescription_Load. 9");
                
                this.timerInitForm.Interval = 500;//Fix
                this.timerInitForm.Enabled = true;
                this.timerInitForm.Start();

                this.timerInitFormAssignPrescription = new System.Windows.Forms.Timer();
                this.timerInitFormAssignPrescription.Tick += new System.EventHandler(this.timerInitFormAssignPrescription_Tick);
                this.timerInitFormAssignPrescription.Interval = 200;//Fix
                this.timerInitFormAssignPrescription.Enabled = true;
                this.timerInitFormAssignPrescription.Start();

                WaitingManager.Hide();
                LogSystem.Debug("frmAssignPrescription_Load. 10");
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitFormAssignPrescriptionAsync()
        {
            try
            {
                LogSystem.Debug("InitFormAssignPrescriptionAsync. 1");
                this.LoadDataToPatientInfo();
                LogSystem.Debug("InitFormAssignPrescriptionAsync. 2");
                this.isNotLoadMediMatyByMediStockInitForm = true;
                this.ReSetDataInputAfterAdd__MedicinePage();
                LogSystem.Debug("InitFormAssignPrescriptionAsync. 3");
                LogSystem.Debug("InitFormAssignPrescriptionAsync. 4");
                this.LoadAncillaryServpaty();
                LogSystem.Debug("InitFormAssignPrescriptionAsync. 5");
                this.FillDataToControlsForm();
                this.InitComboExpMestReason();
                this.InitComborepositoryItemCustomGridLookUpReasion();
                LogSystem.Debug("InitFormAssignPrescriptionAsync. 6");
                this.VisibleButton(this.actionBosung);
                this.LoadDefaultTabpageMedicine();
                this.InitDataByServicePackage();
                this.InitDataByServiceMetyMaty();
                this.InitDataByExpMestTemplate();
                LogSystem.Debug("InitFormAssignPrescriptionAsync. 7");
                this.LoadPrescriptionForEdit();
                this.SetEnableButtonControl(this.actionType);
                this.LoadDataDhstToControl();
                this.isNotLoadMediMatyByMediStockInitForm = false;
                this.IsHandlerWhileOpionGroupSelectedIndexChanged = false;
                this.isNotLoadWhileChangeInstructionTimeInFirst = false;
                this.InitMenuToButtonPrint();
                LogSystem.Debug("InitFormAssignPrescriptionAsync. 8");
                this.InitDefaultFocus();
                this.gridControlServiceProcess.DragOver += new System.Windows.Forms.DragEventHandler(this.gridControlServiceProcess_DragOver);
                this.gridControlServiceProcess.DragDrop += new System.Windows.Forms.DragEventHandler(this.gridControlServiceProcess_DragDrop);
                this.gridViewServiceProcess.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gridViewServiceProcess_MouseMove);
                //ReloadValidate();

                LogSystem.Debug("InitFormAssignPrescriptionAsync. 9");

                ModuleList();
                VisibleColumnPreAmount();
                CheckAssignServiceSimultaneityOption();
                LogSystem.Debug("InitFormAssignPrescriptionAsync. 10");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private async Task ReloadValidate()
        //{
        //    try
        //    {
        //        int heightUCTop = 0;
        //        int heightUCBottom = 0;

        //        if ((GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet) || GlobalStore.IsExecutePTTT)
        //        {
        //            this.InitUCPatientSelect();
        //            this.InitUCPeriousExpMestList();
        //            heightUCBottom = lciUCBottomPanel.Height + 20;
        //            heightUCTop = lciUCTopPanel.Height - 20;


        //            if (!string.IsNullOrWhiteSpace(HisConfigCFG.MODULELINKS))
        //            {
        //                currentModule
        //                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionPK").FirstOrDefault();

        //                if (moduleData != null)
        //                {
        //                    var allowedModules = HisConfigCFG.MODULELINKS.Split(',');

        //                    if (allowedModules.Contains(moduleData.ModuleLink))
        //                    {
        //                        layoutControlGroup9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
        //                    }
        //                    else
        //                    {
        //                        layoutControlGroup9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
        //                        lciForchkSignForDPK.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
        //                        lciForchkSignForDTT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
        //                        lciForchkSignForDDT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            this.InitUCPeriousExpMestList();
        //            this.InitUcTreatmentFinish();
        //            heightUCBottom = lciUCBottomPanel.Height + 20;
        //            heightUCTop = lciUCTopPanel.Height - 20;

        //            if (GlobalStore.IsCabinet)
        //            {

        //                if (!string.IsNullOrWhiteSpace(HisConfigCFG.MODULELINKS))
        //                {
        //                    currentModule
        //                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionPK").FirstOrDefault();

        //                    if (moduleData != null)
        //                    {
        //                        var allowedModules = HisConfigCFG.MODULELINKS.Split(',');

        //                        if (allowedModules.Contains(moduleData.ModuleLink))
        //                        {
        //                            layoutControlGroup9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
        //                        }
        //                        else
        //                        {
        //                            layoutControlGroup9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
        //                            lciForchkSignForDPK.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
        //                            lciForchkSignForDTT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
        //                            lciForchkSignForDDT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (!string.IsNullOrWhiteSpace(HisConfigCFG.MODULELINKS))
        //                {
        //                    currentModule
        //                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionPK").FirstOrDefault();

        //                    if (moduleData != null)
        //                    {
        //                        var allowedModules = HisConfigCFG.MODULELINKS.Split(',');

        //                        if (allowedModules.Contains(moduleData.ModuleLink))
        //                        {
        //                            layoutControlGroup9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
        //                        }
        //                        else
        //                        {
        //                            layoutControlGroup9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
        //                            lciForchkSignForDPK.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
        //                            lciForchkSignForDTT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
        //                            lciForchkSignForDDT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        this.lciUCBottomPanel.Height = heightUCBottom;
        //        this.lciUCTopPanel.Height = heightUCTop;
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}
        private void InitTimerReloadTreatmentFinishTime()
        {
            try
            {
                timerReloadTreatmentFinishTime = new System.Windows.Forms.Timer();
                timerReloadTreatmentFinishTime.Interval = 1000;
                timerReloadTreatmentFinishTime.Tick += TimerReloadTreatmentFinishTime_Tick;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TimerReloadTreatmentFinishTime_Tick(object sender, EventArgs e)
        {
            try
            {       
                if (dteCommonParam != null && dteCommonParam != DateTime.MinValue)
                    dteCommonParam = dteCommonParam.AddSeconds(1);
                if (this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0)
                    dteTreatmentFinishIntructionTime = Int64.Parse(Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.intructionTimeSelecteds.OrderBy(o => o).First()).Value.AddSeconds(1).ToString("yyyyMMddHHmmss"));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void LoadAncillaryServpaty()
        {
            try
            {
                if (currentSereServ != null || currentSereServInEkip != null)
                {
                    CommonParam param = new CommonParam();
                    HisSereServFilter filter = new HisSereServFilter();
                    filter.SERVICE_REQ_ID = this.serviceReqParentId;
                    var SereServ = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, filter, param);
                    if (SereServ != null && SereServ.Count > 0)
                    {
                        HisAncillaryServPatyFilter Afilter = new HisAncillaryServPatyFilter();
                        Afilter.SERVICE_ID = currentSereServ != null ? currentSereServ.SERVICE_ID : currentSereServInEkip.SERVICE_ID;
                        ListAncillaryServPaty = new BackendAdapter(param).Get<List<HIS_ANCILLARY_SERV_PATY>>("api/HisAncillaryServPaty/Get", ApiConsumers.MosConsumer, Afilter, ProcessLostToken, param);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetListEMMedicineAcinInteractive()
        {
            try
            {
                ListMedicineTypeAcin = null;
                ListMedicineTypeOld = null;
                if (HisConfigCFG.AcinInteractiveOption != "1" && HisConfigCFG.AcinInteractiveOption != "2" && HisConfigCFG.AcinInteractiveOption != "3")
                    return;
                CommonParam param = new CommonParam();
                HisExpMestMedicineViewFilter searchMedicineFilter = new HisExpMestMedicineViewFilter();
                if (HisConfigCFG.AcinInteractiveOption == "1")
                {
                    searchMedicineFilter.TDL_INTRUCTION_TIME_FROM = Int64.Parse(InstructionTime.ToString().Substring(0, 8) + "000000");
                    searchMedicineFilter.TDL_INTRUCTION_TIME_TO = Int64.Parse(InstructionTime.ToString().Substring(0, 8) + "235959");
                    searchMedicineFilter.TDL_TREATMENT_ID = treatmentId;
                }
                else
                {
                    searchMedicineFilter.USE_TIME_TO_FROM = Int64.Parse(this.intructionTimeSelecteds.OrderByDescending(o => o).Last().ToString().Substring(0, 8) + "000000");
                    //if (currentTreatmentWithPatientType == null || currentTreatmentWithPatientType.ID <= 0)
                    //    this.currentTreatmentWithPatientType = this.LoadDataToCurrentTreatmentData(treatmentId, this.InstructionTime);
                    searchMedicineFilter.TDL_PATIENT_ID = VHistreatment.PATIENT_ID;
                }
                ListMedicineTypeOld = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, searchMedicineFilter, ProcessLostToken, param);

                Inventec.Common.Logging.LogSystem.Debug("GetListEMMedicineAcinInteractive___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListMedicineTypeOld), ListMedicineTypeOld));
                if (ListMedicineTypeOld != null && ListMedicineTypeOld.Count > 0)
                    ListMedicineTypeAcin = GetMedicineTypeAcinByMedicineType(ListMedicineTypeOld.Where(o => o.AMOUNT > (o.TH_AMOUNT ?? 0)).Select(o => o.MEDICINE_TYPE_ID).ToList());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void VisibleColumnPreAmount()
        {
            try
            {
                if (!HisConfigCFG.IsShowPresAmount)
                    gc_PreAmount__TabMedicine.VisibleIndex = -1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ModuleList()
        {
            try
            {
                if (!string.IsNullOrEmpty(HisConfigCFG.ModuleLinkApply))
                {
                    lstModuleLinkApply = HisConfigCFG.ModuleLinkApply.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        List<MediMatyTypeADO> mediMatyTypeADOSubPres = new List<MediMatyTypeADO>();

        private async Task LoadSubPrescription()
        {
            try
            {
                if (this.serviceReqMain != null && this.serviceReqMain.IS_MAIN_EXAM == 1)
                {
                    if (HisConfigCFG.IsUsingSubPrescriptionMechanism == "1" && this.actionType == GlobalVariables.ActionAdd)
                    {
                        V_HIS_SERVICE_REQ_7 serviceReq7 = new V_HIS_SERVICE_REQ_7();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERVICE_REQ_7>(serviceReq7, this.serviceReqMain);
                        //Đơn phụ
                        List<HIS_SERVICE_REQ_METY> lstExpMestMety = new List<HIS_SERVICE_REQ_METY>();
                        List<HIS_SERVICE_REQ_MATY> lstExpMestMaty = new List<HIS_SERVICE_REQ_MATY>();

                        mediMatyTypeADOSubPres = new List<MediMatyTypeADO>();

                        CommonParam param = new CommonParam();
                        HisServiceReqMetyFilter expMestMetyFilter = new HisServiceReqMetyFilter();
                        expMestMetyFilter.TDL_TREATMENT_ID = VHistreatment.ID;

                        lstExpMestMety = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_METY>>(RequestUriStore.HIS_SERVICE_REQ_METY__GET, ApiConsumers.MosConsumer, expMestMetyFilter, ProcessLostToken, param);

                        HisServiceReqMatyFilter expMestMatyFilter = new HisServiceReqMatyFilter();
                        expMestMatyFilter.TDL_TREATMENT_ID = VHistreatment.ID;
                        lstExpMestMaty = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_MATY>>(RequestUriStore.HIS_SERVICE_REQ_MATY__GET, ApiConsumers.MosConsumer, expMestMatyFilter, ProcessLostToken, param);


                        if (lstExpMestMety != null && lstExpMestMety.Count > 0)
                        {
                            lstExpMestMety = lstExpMestMety.Where(o => o.IS_SUB_PRES == 1 && o.MEDICINE_TYPE_ID != null).ToList();

                            var q1 = (from m in lstExpMestMety
                                      select new MediMatyTypeADO(m, this.intructionTimeSelecteds.First(), serviceReq7)).ToList();
                            if (q1 != null && q1.Count > 0)
                                mediMatyTypeADOSubPres.AddRange(q1);
                        }

                        if (lstExpMestMaty != null && lstExpMestMaty.Count > 0)
                        {
                            lstExpMestMaty = lstExpMestMaty.Where(o => o.IS_SUB_PRES == 1 && o.MATERIAL_TYPE_ID != null).ToList();

                            var q1 = (from m in lstExpMestMaty
                                      select new MediMatyTypeADO(m, false)).ToList();
                            if (q1 != null && q1.Count > 0)
                                mediMatyTypeADOSubPres.AddRange(q1);
                        }
                        if (this.mediMatyTypeAvailables == null || this.mediMatyTypeAvailables.Count == 0)
                            this.InitDataMetyMatyTypeInStockD(this.currentMediStock);
                        //Check trong kho
                        this.ProcessDataMediStock(mediMatyTypeADOSubPres);
                        if (!CheckMaterialReusableOrIdentityManager() || !CheckValidMaterial(true) || !CheckMedicineGroupTuberCulosis(true))
                            return;
                        this.ProcessInstructionTimeMediForEdit();
                        if (this.ProcessCheckAllergenicByPatientAfterChoose()
                            && this.ProcessCheckContraindicaterWarningOptionAfterChoose())
                        {
                            this.ProcessMergeDuplicateRowForListProcessing();
                            this.ProcessAddListRowDataIntoGridWithTakeBean();
                            this.ReloadDataAvaiableMediBeanInCombo();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadVHisTreatment()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                filter.ID = this.treatmentId;
                var vtreatmentDatas = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, param);

                this.VHistreatment = vtreatmentDatas != null ? vtreatmentDatas.FirstOrDefault() : new V_HIS_TREATMENT();
                dteCommonParam = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(param.Now) ?? DateTime.Now;
                this.patientDob = VHistreatment.TDL_PATIENT_DOB;
                timerReloadTreatmentFinishTime.Start();

                //currentTreatment = GetTreatment(this.treatmentId);
                AutoMapper.Mapper.CreateMap<V_HIS_TREATMENT, HIS_TREATMENT>();
                currentTreatment = AutoMapper.Mapper.Map<HIS_TREATMENT>(this.VHistreatment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void timerInitFormAssignPrescription_Tick(object sender, EventArgs e)
        {
            try
            {
                LogSystem.Debug("timerInitFormAssignPrescription_Tick 1...");
                timerInitFormAssignPrescription.Stop();
                this.InitTabIndex();
                this.ValidateForm();
                this.ValidateBosung();
                this.InitControlByConfig();
                this.VisibleExecuteGroupByConfig();
                this.VisibleColumnInGridControlService();

                InitFormAssignPrescriptionAsync();

                LogSystem.Debug("timerInitFormAssignPrescription_Tick 2...");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timerInitForm_Tick(object sender, EventArgs e)
        {
            try
            {
                LogSystem.Debug("timerInitForm_Tick 1...");
                this.timerInitForm.Stop();

                if (this.treatmentFinishProcessor != null && this.ucTreatmentFinish != null)
                {
                    this.treatmentFinishProcessor.UpdateTreatmentData(this.ucTreatmentFinish, this.currentTreatmentWithPatientType);
                }
                InitListConfig();
                EnableButtonConfig();
                this.InitComboEquipment();
                this.InitComboExpMestTemplate();
                this.CheckAppoinmentEarly();//Hien thi thong bao den som thoi gian hen kham
                this.LoadDataTracking(false);
                this.LoadAllergenic(this.currentTreatmentWithPatientType.PATIENT_ID);
                this.InitDataServiceReqAllInDay();
                this.ThreadLoadDonThuocCu(serviceReqAllInDays);
                this.FillDataToComboPriviousExpMest(this.currentTreatmentWithPatientType);
                this.InitMedicineTypeAcinInfo();
                this.InitCheckIcdManager();
                LogSystem.Debug("timerInitForm_Tick__Du lieu thuoc/vat tu____ " + (mediStockD1ADOs != null ? mediStockD1ADOs.Count : 0));
                LogSystem.Debug("timerInitForm_Tick. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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

        private void frmAssignPrescription_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                lblPhatSinh.Focus();
                if (pbClose.Image != null && pbOpen.Image != null)
                {
                    SaveKeyImage(pbClose.Name, true);
                    SaveKeyImage(pbOpen.Name, true);
                }
                else if (pbClose.Image == null && pbOpen.Image != null)
                {
                    SaveKeyImage(pbClose.Name, true);
                    SaveKeyImage(pbOpen.Name, false);
                }
                else
                {
                    SaveKeyImage(pbClose.Name, false);
                    SaveKeyImage(pbOpen.Name, true);
                }
                SaveKeyDHST();
                if (this.actionType == GlobalVariables.ActionAdd || this.actionType == GlobalVariables.ActionEdit)
                {
                    var mediMatyTypeNotEdits = this.mediMatyTypeADOs != null ? this.mediMatyTypeADOs.Where(o => o.IsEdit == false) : null;
                    if (mediMatyTypeNotEdits != null && mediMatyTypeNotEdits.Count() > 0 && btnSave.Enabled == true && !this.NoEdit)
                    {
                        DialogResult myResult;
                        myResult = MessageBox.Show(ResourceMessage.CanhBaoThuocChuaLuuTatForm, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (myResult != DialogResult.Yes)
                        {
                            e.Cancel = true;
                            return;
                        }
                    }
                    ReleaseAllMediByUser();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDefaultFocus()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("InitDefaultFocus.1");
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.AutoFocusToAdvise), HisConfigCFG.AutoFocusToAdvise));
                if (HisConfigCFG.AutoFocusToAdvise)
                {
                    txtAdvise.Focus();
                    txtAdvise.SelectAll();
                }
                else
                    this.UcIcdFocusComtrol();
                Inventec.Common.Logging.LogSystem.Debug("InitDefaultFocus.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitAssignPresctiptionType()
        {
            try
            {
                if (!GlobalStore.IsTreatmentIn)
                    //Set mặc định trường "là nội trú" hay không theo loại phòng
                    GlobalStore.IsTreatmentIn = (this.currentModule.RoomTypeId == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG);

                //Kiểm tra nếu sửa đơn thuốc thì sẽ set lại giá trị của trường "là nội trú" theo phòng yêu cầu của đơn cũ
                //Và set giá trị trường "là tủ trực" nếu đơn cũ là đơn tủ trực
                bool? isFromTypeOut = IsFromTypeOut();
                bool? isFromTypeTT = IsFromTypeTTByEditor();
                if (isFromTypeOut.HasValue && isFromTypeTT.HasValue)
                {
                    if (!GlobalStore.IsTreatmentIn) // Nếu được truyền ở bên ngoài thì không phải kiểm tra
                    {
                        if (isFromTypeOut == true)
                        {
                            GlobalStore.IsTreatmentIn = false;
                        }
                        else
                        {
                            GlobalStore.IsTreatmentIn = true;
                        }
                    }

                    if (!GlobalStore.IsCabinet) // Nếu được truyền ở bên ngoài thì không phải kiểm tra
                    {
                        if (isFromTypeTT.HasValue)
                        {
                            GlobalStore.IsCabinet = isFromTypeTT.Value;
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

        #region Button

        private void btnTestResult_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SumaryTestResults").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.SumaryTestResults'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(treatmentId);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId));
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDichVuHenKham_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AppointmentService").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AppointmentService");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    listArgs.Add(treatmentId);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnBoSungPhacDo_Click(object sender, EventArgs e)
        {
            try
            {
                //Lay danh sach icd
                string icdCode = "";
                var icdValue = this.UcIcdGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                if (icdValue != null)
                {
                    icdCode = icdValue.ICD_CODE;
                }

                var subIcd = this.UcSecondaryIcdGetValue() as SecondaryIcdDataADO;
                if (subIcd != null)
                {
                    icdCode += subIcd.ICD_SUB_CODE;
                }

                if (String.IsNullOrEmpty(icdCode))
                {
                    MessageBox.Show(ResourceMessage.KhongTimThayThongTinICD, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                                    MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (this.mediMatyTypeADOs == null || this.mediMatyTypeADOs.Count == 0)
                {
                    MessageBox.Show(ResourceMessage.VuiLongKeThuocVatTu, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                                    MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }

                string[] icdCodeArr = icdCode.Split(';');
                var mediMatyTypeAllows = this.mediMatyTypeADOs.Where(o => o.SERVICE_ID > 0).ToList();


                CommonParam param = new CommonParam();
                HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
                icdServiceFilter.ICD_CODE__EXACTs = icdCodeArr.ToList();
                List<HIS_ICD_SERVICE> icdServices = new BackendAdapter(param)
                .Get<List<MOS.EFMODEL.DataModels.HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumers.MosConsumer, icdServiceFilter, param);
                icdServices = icdServices != null ? icdServices.Where(o => o.IS_INDICATION == 1).ToList() : null;
                List<long> serviceIdTmps = icdServices.Where(o => o.SERVICE_ID > 0).Select(o => o.SERVICE_ID.Value).ToList();
                List<long> acingrIdTmps = icdServices.Where(o => o.ACTIVE_INGREDIENT_ID > 0).Select(o => o.ACTIVE_INGREDIENT_ID.Value).ToList();
                List<long> serviceNotConfigIds = mediMatyTypeAllows.Where(o => !serviceIdTmps.Contains(o.SERVICE_ID)).Select(o => o.SERVICE_ID).ToList();
                List<HIS_ACTIVE_INGREDIENT> activeIngredientNotConfigs = null;
                List<long> metyIds = mediMatyTypeAllows.Where(o => (o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM)).Select(t => t.ID).ToList();

                var medicineTypeAcinF1s = ValidAcinInteractiveWorker.currentMedicineTypeAcins.Where(o => metyIds.Contains(o.MEDICINE_TYPE_ID)).ToList();
                if (medicineTypeAcinF1s != null && medicineTypeAcinF1s.Count > 0)
                {
                    var acinIgrIds = medicineTypeAcinF1s.Select(o => o.ACTIVE_INGREDIENT_ID).ToList();
                    var acgrNotConfigIds = (acingrIdTmps != null && acingrIdTmps.Count > 0 ? (acingrIdTmps.Where(o => !acinIgrIds.Contains(o))) : null).ToList();
                    if (acgrNotConfigIds != null && acgrNotConfigIds.Count > 0)
                    {
                        activeIngredientNotConfigs = BackendDataWorker.Get<HIS_ACTIVE_INGREDIENT>().Where(o => acgrNotConfigIds.Contains(o.ID)).ToList();
                    }
                }

                if (serviceNotConfigIds == null || serviceNotConfigIds.Count == 0)
                {
                    MessageBox.Show(ResourceMessage.KhongTimThayThuocVatTuDuocCauHinhChanDoanICD, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                                    MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }

                List<HIS_ICD> icds = this.currentIcds.Where(o => icdCodeArr.Contains(o.ICD_CODE)).Distinct().ToList();
                if (icds == null || icds.Count == 0)
                {
                    LogSystem.Debug("Khong tim thay ICD");
                    return;
                }

                if (icds.Count == 1)
                {
                    icdChoose = icds[0];
                }
                else
                {
                    //Mo form chon icd
                    icdChoose = new HIS_ICD();
                    frmChooseICD frm = new frmChooseICD(icds, refeshChooseIcd);
                    frm.ShowDialog();
                }

                if (icdChoose == null || icdChoose.ID == 0)
                    return;

                List<object> listObj = new List<object>();
                listObj.Add(icdChoose);
                listObj.Add(serviceNotConfigIds);
                listObj.Add(activeIngredientNotConfigs);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.ServiceIcd", currentModule.RoomId, currentModule.RoomTypeId, listObj);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAddTutorial_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.medicineTypeTutSelected == null)
                    this.medicineTypeTutSelected = new HIS_MEDICINE_TYPE_TUT();

                if (this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                {
                    this.medicineTypeTutSelected.MEDICINE_TYPE_ID = (this.currentMedicineTypeADOForEdit.ID);
                    ProcessSaveMedicineTypeTut();
                }
                //else if (this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                //{
                //    ProcessSaveMaterialTypeTut();
                //}
                else
                {
                    LogSystem.Warn("Huong dan su dung thuoc chi luu lai khi lam viec o tab thuoc - vat tu va tab thuoc ngoai kho");
                    return;
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessSaveMedicineTypeTut()
        {
                  
            WaitingManager.Show();
            if (memHtu.Text != null)
                this.medicineTypeTutSelected.HTU_TEXT = memHtu.Text;
            else
                this.medicineTypeTutSelected.HTU_TEXT = null;


            if (cboMedicineUseForm.EditValue != null)
                this.medicineTypeTutSelected.MEDICINE_USE_FORM_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboMedicineUseForm.EditValue ?? 0).ToString());
            else
                this.medicineTypeTutSelected.MEDICINE_USE_FORM_ID = null;

            if (DataHtuList != null && DataHtuList.Count > 0 && DataHtuList.Exists(o => o.IsChecked))
                this.medicineTypeTutSelected.HTU_ID = DataHtuList.FirstOrDefault(o => o.IsChecked).ID;
            else
                this.medicineTypeTutSelected.HTU_ID = null;

            if (spinSoLuongNgay.Value > 0)
                this.medicineTypeTutSelected.DAY_COUNT = Convert.ToInt64(spinSoLuongNgay.Value);
            else
                this.medicineTypeTutSelected.DAY_COUNT = null;

            if (!String.IsNullOrEmpty(spinSang.Text))
                this.medicineTypeTutSelected.MORNING = spinSang.Text;
            else
                this.medicineTypeTutSelected.MORNING = null;

            if (!String.IsNullOrEmpty(spinTrua.Text))
                this.medicineTypeTutSelected.NOON = spinTrua.Text;
            else
                this.medicineTypeTutSelected.NOON = null;

            if (!String.IsNullOrEmpty(spinChieu.Text))
                this.medicineTypeTutSelected.AFTERNOON = spinChieu.Text;
            else
                this.medicineTypeTutSelected.AFTERNOON = null;

            if (!String.IsNullOrEmpty(spinToi.Text))
                this.medicineTypeTutSelected.EVENING = spinToi.Text;
            else
                this.medicineTypeTutSelected.EVENING = null;

            this.medicineTypeTutSelected.TUTORIAL = txtTutorial.Text;
            this.medicineTypeTutSelected.HTU_TEXT = memHtu.Text;

            this.medicineTypeTutSelected.LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

            CommonParam param = new CommonParam();
            bool success = true;
            this.medicineTypeTutSelected = new BackendAdapter(param).Post<HIS_MEDICINE_TYPE_TUT>(RequestUriStore.HIS_MEDICINE_TYPE_TUT_CREATE, ApiConsumers.MosConsumer, this.medicineTypeTutSelected, ProcessLostToken, param);
            if (this.medicineTypeTutSelected == null || this.medicineTypeTutSelected.ID == 0)
            {
                success = false;
            }
            MessageManager.ShowAlert(this, param, success);
            this.RefeshDataMedicineTutorial(this.medicineTypeTutSelected);

            var medicineTypeTuts = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE_TUT>();
            string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            List<HIS_MEDICINE_TYPE_TUT> medicineTypeTutFilters = medicineTypeTuts.OrderByDescending(o => o.MODIFY_TIME).Where(o => o.MEDICINE_TYPE_ID == this.currentMedicineTypeADOForEdit.ID && o.LOGINNAME == loginName).ToList();
            this.RebuildTutorialWithInControlContainer(medicineTypeTutFilters);
            this.RebuildHtuWithInControlContainer(medicineTypeTutFilters);



            WaitingManager.Hide();
        }

        internal bool IsSelectMultiPatient()
        {
            bool isMulti = false;
            try
            {
                if (GlobalStore.IsTreatmentIn && this.patientSelectProcessor != null && this.ucPatientSelect != null)
                {
                    var listPatientSelecteds = this.patientSelectProcessor.GetSelectedRows(this.ucPatientSelect);
                    if (listPatientSelecteds != null && listPatientSelecteds.Count > 1)
                    {
                        isMulti = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return isMulti;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            LogTheadInSessionInfo(NewClick, !GlobalStore.IsCabinet ? "NewPrescription" : "NewMedicalStore");
        }

        private void NewClick()
        {
            try
            {
                WaitingManager.Show();


                ThreadLoadDonThuocCu();
                this.isCheckAssignServiceSimultaneityOption = false;
                if (this.actionType == GlobalVariables.ActionAdd)
                    this.ReleaseAllMediByUser();
                this.oldServiceReq = null;
                this.txtInteractionReason.Text = "";
                this.dicMediMateAssignPres = new Dictionary<string, long>();
                long? trackingId = null;
                if (this.cboPhieuDieuTri.EditValue != null)
                    trackingId = Int64.Parse(this.cboPhieuDieuTri.EditValue.ToString());
                this.SetDefaultData();
                this.SetDefaultUC();
                if (HisConfigCFG.IsDefaultTracking)
                    this.cboPhieuDieuTri.EditValue = trackingId;
                this.ReloadUcTreatmentFinish();
                this.ReSetDataInputAfterAdd__MedicinePage();
                this.ReSetChongCHiDinhInfoControl__MedicinePage();
                this.sereServsInTreatmentRaw = null;
                this.LoadSereServTotalHeinPriceWithTreatment(this.treatmentId);
                this.LoadDataSereServWithTreatment(this.currentTreatmentWithPatientType, 0);
                //this.LoadTotalSereServByHeinWithTreatment();
                this.CheckWarningOverTotalPatientPrice();
                this.InitComboMediStockAllow(0);
                this.cboMediStockExport.ShowPopup();
                this.cboMediStockExport.ClosePopup();
                this.ResetFocusMediMaty(true);
                this.InitComboExpMestReason();
                this.LoadDefaultTabpageMedicine();
                this.InitMedicineTypeAcinInfo();
                //this.LoadDefaultDrugStore();
                this.OpionGroupSelectedChangedAsync();
                this.UseTimeSelecteds = new List<long>();
                this.UseTimeSelected = new List<DateTime?>();
                this.txtUseTime.Text = "";
                this.dtUseTime.EditValue = null;
                this.lstOutPatientPres = new List<OutPatientPresADO>();
                InitDataServiceReqAllInDay();
                this.EnableButtonConfig();
                this.CheckAssignServiceSimultaneityOption();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnShowDetail_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ServiceReqList").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ServiceReqList'");
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.ServiceReqList' is not plugins");
                MOS.EFMODEL.DataModels.HIS_TREATMENT treatment = new MOS.EFMODEL.DataModels.HIS_TREATMENT();//treatmentId, intructionTime, serviceReqParentId ?? 0
                treatment.ID = treatmentId;
                List<object> listArgs = new List<object>();
                listArgs.Add(treatment);
                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, GetRoomId(), GetRoomTypeId()), listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");

                WaitingManager.Hide();
                ((Form)extenceInstance).Show();
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
            }
        }

        private void btnSereservInTreatmentPreview_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SereservInTreatment").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.SereservInTreatment'");
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.SereservInTreatment' is not plugins");

                SereservInTreatmentADO sereservInTreatmentADO = new SereservInTreatmentADO(treatmentId, this.intructionTimeSelecteds.OrderByDescending(o => o).First(), serviceReqParentId ?? 0);
                List<object> listArgs = new List<object>();
                listArgs.Add(sereservInTreatmentADO);
                listArgs.Add(this.currentModule);
                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(moduleData, listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");

                WaitingManager.Hide();
                ((Form)extenceInstance).Show();
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSaveTemplate__MedicinePage_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                frmHisExpMestTemplateCreate frm = new frmHisExpMestTemplateCreate(mediMatyTypeADOs, RefeshExpMestTemplate, this.currentModule);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessSaveForListSelect(HIS.Desktop.Plugins.AssignPrescriptionPK.SAVETYPE sType)
        {
            try
            {
                this.bIsSelectMultiPatientProcessing = false;

                if (this.gridViewServiceProcess.IsEditing)
                    this.gridViewServiceProcess.CloseEditor();

                if (this.gridViewServiceProcess.FocusedRowModified)
                    this.gridViewServiceProcess.UpdateCurrentRow();

                this.mediMatyTypeADOs = this.gridViewServiceProcess.DataSource as List<MediMatyTypeADO>;
                if (mediMatyTypeADOs == null || mediMatyTypeADOs.Count == 0)
                    return;
                var NotHasSerialNumber = GetMaterialReusableOrIdentityManager().Where(maty => string.IsNullOrEmpty(maty.SERIAL_NUMBER)).ToList();
                if (NotHasSerialNumber.Count > 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Vật tư đích danh - TSD: {0} không có số seri", string.Join(", ", NotHasSerialNumber.Select(o => o.MEDICINE_TYPE_NAME))), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
                    return;
                }
                if (!CheckValidMaterial(true))
                    return;
                if (!GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet && intructionTimeSelecteds != null && intructionTimeSelecteds.Count > 1 && mediMatyTypeADOs.Exists(o => o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Với đơn phòng khám, chỉ cho phép kê đơn nhiều ngày với thuốc/vật tư mua ngoài", HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
                    return;
                }

                IsMultilPatient = IsSelectMultiPatient();
                if ((intructionTimeSelecteds != null && intructionTimeSelected.Count > 1) || (mediMatyTypeADOs != null && mediMatyTypeADOs.Count > 0 && (mediMatyTypeADOs.Select(o => o.IntructionTime).Distinct().ToList().Count > 1 || mediMatyTypeADOs.Select(o => o.IntructionTime).Distinct().First() != InstructionTime)) || IsMultilPatient || (this.oldServiceReq != null && this.oldServiceReq.ID > 0))
                {
                    IsSaveOverResultReasonTest = true;
                    List<long> treatmentIds = new List<long>();
                    if (IsMultilPatient)
                    {
                        var listPatientSelecteds = this.patientSelectProcessor.GetSelectedRows(this.ucPatientSelect);
                        treatmentIds.AddRange(listPatientSelecteds.Select(o => o.TREATMENT_ID).ToList());
                    }
                    else
                        treatmentIds.Add(treatmentId);
                    if (!GetOverReason(ref mediMatyTypeADOs, treatmentIds, this.intructionTimeSelecteds))
                        return;
                }
                if (GlobalStore.IsTreatmentIn && this.patientSelectProcessor != null && this.ucPatientSelect != null)
                {
                    if (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet && !CheckMaxInPrescriptionInBatchMultilPatientWhenSave(this.mediMatyTypeADOs))
                    {
                        return;
                    }
                    var listPatientSelecteds = this.patientSelectProcessor.GetSelectedRows(this.ucPatientSelect);
                    if (listPatientSelecteds != null && listPatientSelecteds.Count > 1)
                    {
                        var myResult = DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.BanDangChonKeDonChoNBenhNhanBanCoChacMuonThucHien, listPatientSelecteds.Count), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (myResult != System.Windows.Forms.DialogResult.Yes)
                        {
                            Inventec.Common.Logging.LogSystem.Debug("Trường hợp kê đơn nội trú chọn nhiều bệnh nhân để kê, người dùng chọn không kê=> dừng không xử lý gì tiếp____" + Inventec.Common.Logging.LogUtil.TraceData("listPatientSelecteds.Count", listPatientSelecteds.Count));
                            return;
                        }

                        this.bIsSelectMultiPatientProcessing = true;
                        this.paramSaveList = new CommonParam();
                        this.paramSaveList.Messages = new List<string>();
                        this.successSaveList = true;

                        this.ProcessSaveData(sType);
                        this.actionType = GlobalVariables.ActionAdd;
                        this.actionBosung = GlobalVariables.ActionAdd;
                        this.SetEnableButtonControl(this.actionType);
                        if (!IsValidForSave)
                            return;
                        var listProcessPatientSelecteds = listPatientSelecteds.Where(o => o.TREATMENT_ID != this.treatmentId).ToList();

                        foreach (var item in listProcessPatientSelecteds)
                        {
                            PatientSelectedChange(item, false);
                            this.ChangeLockButtonWhileProcess(true);

                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.TREATMENT_ID), item.TREATMENT_ID));
                            //- Nguồn khác
                            //- Kết hợp BH
                            //- Kiểm tra điều kiện DV(HIS_SERVICE_CONDITION)
                            foreach (var medi in this.mediMatyTypeADOs)
                            {
                                medi.AmountAlert = null;
                                medi.ErrorMessageAmount = "";
                                medi.ErrorMessageAmountAlert = "";
                                medi.ErrorMessageAmountHasRound = "";
                                medi.ErrorMessageIsAssignDay = "";
                                medi.ErrorMessageMedicineUseForm = "";
                                medi.ErrorMessageMediMatyBean = "";
                                medi.ErrorMessagePatientTypeId = "";
                                medi.ErrorMessageTutorial = "";
                                medi.ErrorMessageOddPres = "";
                                medi.ErrorTypeAmount = ErrorType.None;
                                medi.ErrorTypeAmountAlert = ErrorType.None;
                                medi.ErrorTypeAmountHasRound = ErrorType.None;
                                medi.ErrorTypeIsAssignDay = ErrorType.None;
                                medi.ErrorTypeMedicineUseForm = ErrorType.None;
                                medi.ErrorTypeMediMatyBean = ErrorType.None;
                                medi.ErrorTypePatientTypeId = ErrorType.None;
                                medi.ErrorTypeTutorial = ErrorType.None;
                                medi.ErrorTypeOddPres = ErrorType.None;
                                medi.IsKHBHYT = false;
                                medi.OTHER_PAY_SOURCE_ID = null;
                                medi.OTHER_PAY_SOURCE_CODE = null;
                                medi.OTHER_PAY_SOURCE_NAME = null;

                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("currentHisPatientTypeAlter.PATIENT_TYPE_ID", currentHisPatientTypeAlter.PATIENT_TYPE_ID)
                                    + Inventec.Common.Logging.LogUtil.TraceData("item.TREATMENT_ID", item.TREATMENT_ID));
                                HIS_PATIENT_TYPE patientTypeDefault = ChoosePatientTypeDefaultlServiceOther(currentHisPatientTypeAlter.PATIENT_TYPE_ID, medi.SERVICE_ID, medi.SERVICE_TYPE_ID);
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientTypeDefault), patientTypeDefault)
                                    + Inventec.Common.Logging.LogUtil.TraceData("medi.PATIENT_TYPE_ID", medi.PATIENT_TYPE_ID));
                                if (patientTypeDefault != null)
                                {
                                    medi.PATIENT_TYPE_ID = patientTypeDefault.ID;
                                    FillDataOtherPaySourceDataRow(medi);
                                }
                                else
                                {
                                    medi.PATIENT_TYPE_ID = null;
                                }

                                UpdateExpMestReasonInDataRow(medi);
                            }

                            gridViewServiceProcess.BeginUpdate();
                            gridViewServiceProcess.GridControl.DataSource = this.mediMatyTypeADOs;
                            gridViewServiceProcess.EndUpdate();
                            currentTreatment = GetTreatment(item.TREATMENT_ID);
                            this.ProcessSaveData(sType);

                            this.actionType = GlobalVariables.ActionAdd;
                            this.actionBosung = GlobalVariables.ActionAdd;
                            this.SetEnableButtonControl(this.actionType);
                        }
                        string message = "";
                        if (!this.successSaveList)
                        {
                            message += ResourceMessage.CacBenhNhanSaukeDonThatBai;
                            message += "<br>";
                            message += String.Join("", this.paramSaveList.Messages);

                            if (!String.IsNullOrEmpty(message))
                            {
                                MessageManager.Show(message);
                            }
                        }
                        else
                        {
                            this.actionType = GlobalVariables.ActionView;
                            this.actionBosung = GlobalVariables.ActionAdd;
                            this.SetEnableButtonControl(this.actionType);
                            message = MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKQXLYCCuaFrontendThanhCong);
                            MessageManager.Show(this, new CommonParam(), true);
                        }
                    }
                    else
                        this.ProcessSaveData(sType);
                }
                else
                    this.ProcessSaveData(sType);
                this.LoadDataSereServWithTreatment(this.currentTreatmentWithPatientType, 0);
                IsSaveOverResultReasonTest = false;
                this.bIsSelectMultiPatientProcessing = false;
            }
            catch (Exception ex)
            {
                IsSaveOverResultReasonTest = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal bool GetOverReason(MediMatyTypeADO mediMatyType, bool IsGrid = false, bool IsUpdateGrid = false)
        {
            bool result = true;
            try
            {
                if (mediMatyType.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU     
                    || mediMatyType.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM
                    || mediMatyType.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD)
                {
                    return result;
                }     
                CommonParam param = new CommonParam();
                HisMedicineServiceFilter filter = new HisMedicineServiceFilter();
                filter.MEDICINE_TYPE_ID = mediMatyType.ID;
                medicineService = new BackendAdapter(param).Get<List<HIS_MEDICINE_SERVICE>>("api/HisMedicineService/Get", ApiConsumers.MosConsumer, filter, param);
                if (medicineService != null && medicineService.Count > 0)
                {
                    CreateThreadOverReasonAdd(new List<long>() { treatmentId });

                    decimal AmountInDay = GetAmountInDay(mediMatyType);
                    #region 119139 V+
                    if (sereServTeinResultTest != null && sereServTeinResultTest.Count > 0)
                    {
                        List<HIS_MEDICINE_SERVICE> medicineService = new List<HIS_MEDICINE_SERVICE>();
                        var lstSereServTein = GetTein1(sereServTeinResultTest);
                        foreach (var ssTein in lstSereServTein)
                        {
                            var medicineServiceCheck = this.medicineServiceTest.Where(o => o.TEST_INDEX_ID == ssTein.TEST_INDEX_ID).Where(o => (o.VALUE_SERVICE_FROM ?? decimal.MinValue) <= ConvertToDecimal(ssTein.VALUE) && ConvertToDecimal(ssTein.VALUE) < (o.VALUE_SERVICE_TO ?? decimal.MaxValue) && (o.AMOUNT_INDAY_FROM ?? decimal.MinValue) < ((IsGrid ? mediMatyType.AMOUNT / (mediMatyType.UseDays ?? 1) : GetAmount() / (this.spinSoLuongNgay.Value != 0 ? this.spinSoLuongNgay.Value : 1)) + AmountInDay) && ((IsGrid ? mediMatyType.AMOUNT / (mediMatyType.UseDays ?? 1) : GetAmount() / (this.spinSoLuongNgay.Value != 0 ? this.spinSoLuongNgay.Value : 1)) + AmountInDay) <= (o.AMOUNT_INDAY_TO ?? decimal.MaxValue)).ToList();
                            if (medicineServiceCheck != null && medicineServiceCheck.Count > 0)
                            {
                                medicineService.AddRange(medicineServiceCheck);
                            }
                        }
                        if (medicineService != null && medicineService.Count > 0)
                        {
                            frmOverReason frm = new frmOverReason(medicineService, "bổ sung", (o) =>
                            {
                                if (mediMatyType.dicTreatmentOverResultTestReason == null)
                                    mediMatyType.dicTreatmentOverResultTestReason = new Dictionary<long, List<TreatmentOverReason>>();
                                if (!mediMatyType.dicTreatmentOverResultTestReason.ContainsKey(InstructionTime))
                                    mediMatyType.dicTreatmentOverResultTestReason[InstructionTime] = new List<TreatmentOverReason>() { o };
                                mediMatyType.OVER_RESULT_TEST_REASON = o.overReason;
                                mediMatyType.OVER_REASON_ID = o.overReasonId;
                                mediMatyType.IsEditOverResultTestReason = true;
                            }, (o) =>
                            {
                                mediMatyType.IsNoPrescription = !o;
                                result = o;
                            }, mediMatyType, IsUpdateGrid, false, treatmentId);
                            frm.ShowDialog();
                        }
                        else
                        {
                            mediMatyType.dicTreatmentOverResultTestReason = new Dictionary<long, List<TreatmentOverReason>>();
                            mediMatyType.OVER_RESULT_TEST_REASON = null;
                            mediMatyType.OVER_REASON_ID = null;
                            mediMatyType.IsEditOverResultTestReason = false;
                        }
                    }
                    #endregion

                    #region  119125 V+
                    if (sereServTeinKidney != null && sereServTeinKidney.Count > 0)
                    {
                        List<HIS_MEDICINE_SERVICE> medicineService = new List<HIS_MEDICINE_SERVICE>();
                        var lstSereServTein = GetTein1(sereServTeinKidney);
                        foreach (var ssTein in lstSereServTein)
                        {
                            if (this.dhst == null || dhst.ID == 0)
                                this.medicineService = this.medicineService.Where(o => o.DATA_TYPE == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_SERVICE.DATA_TYPE__EGFR).ToList();

                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.medicineService.Where(o => o.TEST_INDEX_ID == ssTein.TEST_INDEX_ID).Where(o => o.DATA_TYPE != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_SERVICE.DATA_TYPE__SERVICE)), this.medicineService.Where(o => o.TEST_INDEX_ID == ssTein.TEST_INDEX_ID).Where(o => o.DATA_TYPE != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_SERVICE.DATA_TYPE__SERVICE)));
                            var medicineServiceCheck = this.medicineService.Where(o => o.TEST_INDEX_ID == ssTein.TEST_INDEX_ID).Where(o => o.DATA_TYPE != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_SERVICE.DATA_TYPE__SERVICE).Where(o => (o.VALUE_SERVICE_FROM ?? decimal.MinValue) <= GetValueFromDataType(o.DATA_TYPE, ConvertToDecimal(ssTein.VALUE), dhst, chiSoMLCT) && GetValueFromDataType(o.DATA_TYPE, ConvertToDecimal(ssTein.VALUE), dhst, chiSoMLCT) < (o.VALUE_SERVICE_TO ?? decimal.MaxValue) && (o.AMOUNT_INDAY_FROM ?? 0) < ((IsGrid ? mediMatyType.AMOUNT / (mediMatyType.UseDays ?? 1) : GetAmount() / (this.spinSoLuongNgay.Value != 0 ? this.spinSoLuongNgay.Value : 1)) + AmountInDay) && ((IsGrid ? mediMatyType.AMOUNT / (mediMatyType.UseDays ?? 1) : GetAmount() / (this.spinSoLuongNgay.Value != 0 ? this.spinSoLuongNgay.Value : 1)) + AmountInDay) <= (o.AMOUNT_INDAY_TO ?? decimal.MaxValue)).ToList();
                            if (medicineServiceCheck != null && medicineServiceCheck.Count > 0)
                            {
                                medicineService.AddRange(medicineServiceCheck);
                            }
                        }
                        if (medicineService != null && medicineService.Count > 0)
                        {
                            var minOverDose = medicineService.OrderBy(o => o.AMOUNT_INDAY_FROM ?? 0).ThenByDescending(o => o.ID);
                            decimal minType = 0;
                            short? DataType = null;
                            bool IsFirst = true;
                            foreach (var item in minOverDose)
                            {
                                var value = GetValueFromDataType(item.DATA_TYPE, ConvertToDecimal(lstSereServTein.FirstOrDefault(p => p.TEST_INDEX_ID == item.TEST_INDEX_ID).VALUE), dhst, chiSoMLCT);
                                if (IsFirst)
                                {
                                    minType = value;
                                    IsFirst = false;
                                }
                                if (minType > value)
                                {
                                    minType = value;
                                }
                                DataType = item.DATA_TYPE;
                            }
                            frmOverReason frm = new frmOverReason(medicineService, string.Format("kê thuốc {0}", mediMatyType.MEDICINE_TYPE_NAME), (o) =>
                            {
                                if (mediMatyType.dicTreatmentOverKidneyReason == null)
                                    mediMatyType.dicTreatmentOverKidneyReason = new Dictionary<long, List<TreatmentOverReason>>();
                                if (!mediMatyType.dicTreatmentOverKidneyReason.ContainsKey(InstructionTime))
                                    mediMatyType.dicTreatmentOverKidneyReason[InstructionTime] = new List<TreatmentOverReason>() { o };
                                mediMatyType.OVER_KIDNEY_REASON = o.overReason;
                                mediMatyType.OVER_REASON_ID = o.overReasonId;
                                mediMatyType.IsEditOverKidneyReason = true;
                            }, (o) =>
                            {
                                mediMatyType.IsNoPrescription = !o;
                                result = o;
                            }, mediMatyType, IsUpdateGrid, true, treatmentId, (sdo) =>
                            {
                                if (DataType == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_SERVICE.DATA_TYPE__EGFR)
                                {
                                    sdo.Egfr = minType.ToString();
                                }
                                else if (DataType == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_SERVICE.DATA_TYPE__CRCL)
                                {
                                    sdo.Clcr = minType.ToString();
                                }
                                sdo.CreatininValue = chisotestIndexCreatinin > 0 ? chisotestIndexCreatinin.ToString() : null;
                                if (lciARCPCR.Text.ToLower().Contains("uacr"))
                                {
                                    sdo.Uacr = lblACRPCR.Text;
                                }
                                else if (lciARCPCR.Text.ToLower().Contains("upcr"))
                                {
                                    sdo.Upcr = lblACRPCR.Text;
                                }
                                sdo.IntructionTime = intructionTimeSelecteds.OrderByDescending(t => t).First();
                                AlertLogADO ado = new AlertLogADO(sdo, mediMatyType);
                                AlertLogsSdo.Add(ado);
                            }, minType, minOverDose.FirstOrDefault(), AmountInDay);
                            frm.ShowDialog();
                        }
                        else
                        {
                            mediMatyType.dicTreatmentOverKidneyReason = new Dictionary<long, List<TreatmentOverReason>>();
                            mediMatyType.OVER_KIDNEY_REASON = null;
                            mediMatyType.OVER_REASON_ID = null;
                            mediMatyType.IsEditOverKidneyReason = false;
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private decimal GetAmountInDay(MediMatyTypeADO mediMatyType)
        {
            decimal AmountInDay = 0;
            try
            {
                if (sereServWithTreatment != null && sereServWithTreatment.Count > 0)
                {
                    var ss = this.sereServWithTreatment.Where(o => o.SERVICE_ID == mediMatyType.SERVICE_ID
                 && o.TDL_INTRUCTION_TIME.ToString().Substring(0, 8) == intructionTimeSelecteds.OrderByDescending(t => t).First().ToString().Substring(0, 8)).ToList();
                    if (oldServiceReq != null && oldServiceReq.ID > 0)
                        ss = ss.Where(o => o.SERVICE_REQ_ID != oldServiceReq.ID).ToList();
                    foreach (var item in ss)
                    {
                        int number = 1;
                        if (item.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                        {
                            var emmList = GetExpMestMedicineByExpMestId(item.EXP_MEST_MEDICINE_ID ?? 0);
                            if (emmList != null && emmList.Count > 0)
                            {
                                var emm = emmList.FirstOrDefault();
                                if (emm.USE_TIME_TO.HasValue)
                                {
                                    number = (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(emm.USE_TIME_TO.Value).Value.Date - Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.TypeConvert.Parse.ToInt64((InstructionTime.ToString().Substring(0, 8) + "000000"))).Value.Date).Days + 1;
                                }
                            }
                        }
                        AmountInDay += item.AMOUNT / number;
                    }
                }
                if (serviceReqMetyInDay != null && serviceReqMetyInDay.Count > 0)
                {
                    var ss = this.serviceReqMetyInDay.Where(o => o.MEDICINE_TYPE_ID == mediMatyType.ID);
                    if (oldServiceReq != null && oldServiceReq.ID > 0)
                        ss = ss.Where(o => o.SERVICE_REQ_ID != oldServiceReq.ID).ToList();
                    AmountInDay += ss.Sum(o => o.AMOUNT / (o.USE_TIME_TO.HasValue ? (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(o.USE_TIME_TO.Value).Value.Date - Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.TypeConvert.Parse.ToInt64((InstructionTime.ToString().Substring(0, 8) + "000000"))).Value.Date).Days + 1 : 1));
                }
                if (mediMatyTypeADOs != null && mediMatyTypeADOs.Count > 0)
                    AmountInDay += this.mediMatyTypeADOs.Where(o => o.SERVICE_ID == mediMatyType.SERVICE_ID && o.PrimaryKey != mediMatyType.PrimaryKey).Sum(o => o.AMOUNT / (o.UseDays ?? 1) ?? 0);
            }
            catch (Exception ex)
            {
                AmountInDay = 0;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return AmountInDay;
        }
        private decimal GetAmountInDaySave(MediMatyTypeADO medi, long treatId, long itime, bool IsShowPopup = true)
        {
            decimal AmountInDay = 0;
            try
            {
                if (IsShowPopup)
                {
                    if (sereServWithMultilTreatment != null && sereServWithMultilTreatment.Count > 0)
                    {
                        var ss = this.sereServWithMultilTreatment.Where(o => o.SERVICE_ID == medi.SERVICE_ID && o.TDL_INTRUCTION_TIME.ToString().Substring(0, 8) == itime.ToString().Substring(0, 8) && o.TDL_TREATMENT_ID == treatId).ToList();
                        if (oldServiceReq != null && oldServiceReq.ID > 0)
                            ss = ss.Where(o => o.SERVICE_REQ_ID != oldServiceReq.ID).ToList();
                        foreach (var item in ss)
                        {
                            int number = 1;
                            if (item.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                            {
                                var emmList = GetExpMestMedicineByExpMestId(item.EXP_MEST_MEDICINE_ID ?? 0);
                                if (emmList != null && emmList.Count > 0)
                                {
                                    var emm = emmList.FirstOrDefault();
                                    if (emm.USE_TIME_TO.HasValue)
                                    {
                                        number = (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(emm.USE_TIME_TO.Value).Value.Date - Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.TypeConvert.Parse.ToInt64((InstructionTime.ToString().Substring(0, 8) + "000000"))).Value.Date).Days + 1;
                                    }
                                }
                            }
                            AmountInDay += item.AMOUNT / number;
                        }
                    }
                    if (serviceReqMetyViewInDay != null && serviceReqMetyViewInDay.Count > 0)
                    {
                        var ss = this.serviceReqMetyViewInDay.Where(o => o.MEDICINE_TYPE_ID == medi.ID && o.TREATMENT_ID == treatId && o.INTRUCTION_DATE.ToString().Substring(0, 8) == itime.ToString().Substring(0, 8));
                        if (oldServiceReq != null && oldServiceReq.ID > 0)
                            ss = ss.Where(o => o.SERVICE_REQ_ID != oldServiceReq.ID).ToList();
                        AmountInDay += ss.Sum(o => o.AMOUNT / (o.USE_TIME_TO.HasValue ? (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(o.USE_TIME_TO.Value).Value.Date - Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.TypeConvert.Parse.ToInt64((InstructionTime.ToString().Substring(0, 8) + "000000"))).Value.Date).Days + 1 : 1));
                    }
                    if (mediMatyTypeADOs != null && mediMatyTypeADOs.Count > 0)
                        AmountInDay += this.mediMatyTypeADOs.Where(o => o.SERVICE_ID == medi.SERVICE_ID && o.PrimaryKey != medi.PrimaryKey).Sum(o => (o.AMOUNT ?? 0) / (o.UseDays ?? 1));
                }
                else
                {
                    if (sereServWithMultilTreatment != null && sereServWithMultilTreatment.Count > 0)
                    {
                        var ss = this.sereServWithMultilTreatment.Where(o =>
o.SERVICE_ID == medi.SERVICE_ID && o.TDL_INTRUCTION_TIME.ToString().Substring(0, 8) == itime.ToString().Substring(0, 8) && o.SERVICE_REQ_ID != (this.actionType == GlobalVariables.ActionEdit ? this.assignPrescriptionEditADO.ServiceReq.ID : ServiceReqIdPrevios) && o.TDL_TREATMENT_ID == treatId).ToList();
                        foreach (var item in ss)
                        {
                            int number = 1;
                            if (item.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                            {
                                var emmList = GetExpMestMedicineByExpMestId(item.EXP_MEST_MEDICINE_ID ?? 0);
                                if (emmList != null && emmList.Count > 0)
                                {
                                    var emm = emmList.FirstOrDefault();
                                    if (emm.USE_TIME_TO.HasValue)
                                    {
                                        number = (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(emm.USE_TIME_TO.Value).Value.Date - Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.TypeConvert.Parse.ToInt64((InstructionTime.ToString().Substring(0, 8) + "000000"))).Value.Date).Days + 1;
                                    }
                                }
                            }
                            AmountInDay += item.AMOUNT / number;
                        }
                    }
                    if (serviceReqMetyViewInDay != null && serviceReqMetyViewInDay.Count > 0)
                    {
                        var ss = this.serviceReqMetyViewInDay.Where(o => o.MEDICINE_TYPE_ID == medi.ID && o.TREATMENT_ID == treatId && o.INTRUCTION_DATE.ToString().Substring(0, 8) == itime.ToString().Substring(0, 8) && o.SERVICE_REQ_ID != (assignPrescriptionEditADO != null && assignPrescriptionEditADO.ServiceReq != null ? this.assignPrescriptionEditADO.ServiceReq.ID : oldServiceReq.ID));
                        AmountInDay += ss.Sum(o => o.AMOUNT / (o.USE_TIME_TO.HasValue ? (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(o.USE_TIME_TO.Value).Value.Date - Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.TypeConvert.Parse.ToInt64((InstructionTime.ToString().Substring(0, 8) + "000000"))).Value.Date).Days + 1 : 1));
                    }
                }
            }
            catch (Exception ex)
            {
                AmountInDay = 0;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return AmountInDay;
        }

        private List<V_HIS_SERE_SERV_TEIN_1> GetTein1(List<V_HIS_SERE_SERV_TEIN_1> lstObj)
        {
            List<V_HIS_SERE_SERV_TEIN_1> lstSereServTein = new List<V_HIS_SERE_SERV_TEIN_1>();
            try
            {
                var GroupTein = lstObj.Where(o => !string.IsNullOrEmpty(o.VALUE)).OrderByDescending(o => o.TDL_INTRUCTION_TIME).GroupBy(o => o.TEST_INDEX_ID);
                foreach (var item in GroupTein)
                {
                    lstSereServTein.Add(item.First());
                }
            }
            catch (Exception ex)
            {
                lstSereServTein = new List<V_HIS_SERE_SERV_TEIN_1>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return lstSereServTein;
        }

        protected bool GetOverReason(ref List<MediMatyTypeADO> mediMatyType, List<long> treatmentIds, List<long> intructionTimeSelecteds, bool IsShowPopup = true)
        {
            bool result = true;
            try
            {
                if (mediMatyType.Any(m =>
                    m.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU
                    || m.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM
                    || m.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD))
                {
                    return result;
                }
                CommonParam param = new CommonParam();
                HisMedicineServiceFilter filter = new HisMedicineServiceFilter();
                filter.MEDICINE_TYPE_IDs = mediMatyType.Select(o => o.ID).ToList();
                medicineService = new BackendAdapter(param).Get<List<HIS_MEDICINE_SERVICE>>("api/HisMedicineService/Get", ApiConsumers.MosConsumer, filter, param);
                if (medicineService != null && medicineService.Count > 0)
                {
                    CreateThreadOverReasonSave(treatmentIds);
                    if (sereServTeinResultTest != null && sereServTeinResultTest.Count > 0)
                    {
                        foreach (var itime in intructionTimeSelecteds)
                        {
                            var mediResultTest = medicineService.Where(o => o.DATA_TYPE == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_SERVICE.DATA_TYPE__SERVICE).ToList();
                            foreach (var medi in mediMatyType)
                            {
                                if (medi.dicTreatmentOverResultTestReason == null)
                                    medi.dicTreatmentOverResultTestReason = new Dictionary<long, List<TreatmentOverReason>>();
                                decimal AmountInDay = 0;
                                var mediSer = mediResultTest.Where(o => o.MEDICINE_TYPE_ID == medi.ID).ToList();
                                foreach (var treatId in treatmentIds)
                                {
                                    AmountInDay = GetAmountInDaySave(medi, treatId, itime, IsShowPopup);
                                    var ssTeinList = sereServTeinResultTest.Where(o => o.TDL_TREATMENT_ID == treatId && mediSer.Exists(p => p.TEST_INDEX_ID == o.TEST_INDEX_ID)).ToList();
                                    if (ssTeinList != null && ssTeinList.Count > 0)
                                    {
                                        var lstSereServTein = GetTein1(ssTeinList);
                                        List<HIS_MEDICINE_SERVICE> medicine = new List<HIS_MEDICINE_SERVICE>();
                                        foreach (var ssTein in lstSereServTein)
                                        {
                                            var medicineCheck = mediSer.Where(o => ssTein.TEST_INDEX_ID == o.TEST_INDEX_ID).Where(o => (o.VALUE_SERVICE_FROM ?? decimal.MinValue) <= ConvertToDecimal(ssTein.VALUE) && ConvertToDecimal(ssTein.VALUE) < (o.VALUE_SERVICE_TO ?? decimal.MaxValue) && (o.AMOUNT_INDAY_FROM ?? 0) < (medi.AMOUNT / (medi.UseDays ?? 1) + AmountInDay) && (medi.AMOUNT / (medi.UseDays ?? 1) + AmountInDay) <= (o.AMOUNT_INDAY_TO ?? decimal.MaxValue)).ToList();
                                            if (medicineCheck != null && medicineCheck.Count > 0)
                                                medicine.AddRange(medicineCheck);
                                        }
                                        if (medicine != null && medicine.Count > 0)
                                        {
                                            if (IsShowPopup)
                                            {
                                                var minOverDose = medicine.OrderBy(o => o.AMOUNT_INDAY_FROM ?? 0).ThenByDescending(o => o.ID);
                                                decimal minType = 0;
                                                short? DataType = null;
                                                bool IsFirst = true;
                                                foreach (var item in minOverDose)
                                                {
                                                    var value = GetValueFromDataType(item.DATA_TYPE, ConvertToDecimal(lstSereServTein.FirstOrDefault(p => p.TEST_INDEX_ID == item.TEST_INDEX_ID).VALUE), dhst, chiSoMLCT);
                                                    if (IsFirst)
                                                    {
                                                        minType = value;
                                                        IsFirst = false;
                                                    }
                                                    if (minType > value)
                                                    {
                                                        minType = value;
                                                    }
                                                    DataType = item.DATA_TYPE;
                                                }
                                                frmOverReason frm = new frmOverReason(medicine, string.Format("kê thuốc {0}", medi.MEDICINE_TYPE_NAME), (o) =>
                                                {
                                                    if (!medi.dicTreatmentOverResultTestReason.ContainsKey(itime))
                                                        medi.dicTreatmentOverResultTestReason[itime] = new List<TreatmentOverReason>() { o };
                                                    else
                                                        medi.dicTreatmentOverResultTestReason[itime].Add(o);
                                                }, (o) =>
                                                {
                                                    if (!o)
                                                    {
                                                        medi.IsNoPrescription = true;
                                                    }
                                                    else
                                                    {
                                                        medi.IsNoPrescription = false;
                                                    }
                                                }, medi, false, false, treatmentId, null, minType, minOverDose.FirstOrDefault(), AmountInDay);
                                                frm.ShowDialog();
                                            }
                                            else
                                            {
                                                medi.IsEditOverResultTestReason = true;
                                            }
                                        }
                                        else
                                        {
                                            medi.OVER_RESULT_TEST_REASON = null;
                                            medi.OVER_REASON_ID = null;
                                        }
                                    }

                                }
                            }
                        }
                    }
                    if (sereServTeinKidney != null && sereServTeinKidney.Count > 0)
                    {
                        bool breakAll = false;
                        foreach (var itime in intructionTimeSelecteds)
                        {
                            if (breakAll)
                                break;
                            var mediKidney = medicineService.Where(o => o.DATA_TYPE != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_SERVICE.DATA_TYPE__SERVICE).ToList();
                            foreach (var medi in mediMatyType)
                            {
                                if (!result)
                                { breakAll = true; break; }
                                if (medi.dicTreatmentOverKidneyReason == null)
                                    medi.dicTreatmentOverKidneyReason = new Dictionary<long, List<TreatmentOverReason>>();
                                decimal AmountInDay = 0;
                                var mediSer = mediKidney.Where(o => o.MEDICINE_TYPE_ID == medi.ID).ToList();
                                foreach (var treatId in treatmentIds)      
                                {
                                    AmountInDay = GetAmountInDaySave(medi, treatId, itime, IsShowPopup);
                                    var ssTeinList = sereServTeinKidney.Where(o => !string.IsNullOrEmpty(o.VALUE)).Where(o => o.TDL_TREATMENT_ID == treatId && mediSer.Exists(p => p.TEST_INDEX_ID == o.TEST_INDEX_ID)).ToList();
                                    HIS_DHST dhst = null;
                                    if (IsShowPopup && dhstlist != null && dhstlist.Count > 0 && dhstlist.FirstOrDefault(o => o.TREATMENT_ID == treatId) != null)
                                    {
                                        dhstlist = dhstlist.Where(o => o.TREATMENT_ID == treatId).OrderByDescending(o => o.EXECUTE_TIME).ThenByDescending(o => o.ID).ToList();
                                        var firstDhst = dhstlist[0];
                                        if (firstDhst.WEIGHT.HasValue && firstDhst.HEIGHT.HasValue && firstDhst.TEMPERATURE.HasValue && firstDhst.BREATH_RATE.HasValue && firstDhst.CHEST.HasValue && firstDhst.BELLY.HasValue && firstDhst.BLOOD_PRESSURE_MAX.HasValue && firstDhst.BLOOD_PRESSURE_MIN.HasValue && firstDhst.PULSE.HasValue && firstDhst.SPO2.HasValue)
                                        {
                                            dhst = firstDhst;
                                        }
                                        else
                                        {
                                            dhst = firstDhst;
                                            foreach (var item in dhstlist)
                                            {
                                                if (dhst != null && dhst.WEIGHT.HasValue && !dhst.HEIGHT.HasValue && item.HEIGHT.HasValue)
                                                {
                                                    dhst.HEIGHT = item.HEIGHT;
                                                }
                                                else if (dhst != null && dhst.HEIGHT.HasValue && !dhst.WEIGHT.HasValue && item.WEIGHT.HasValue)
                                                {
                                                    dhst.WEIGHT = item.WEIGHT;
                                                }
                                                else if (dhst != null && !dhst.HEIGHT.HasValue && !dhst.WEIGHT.HasValue)
                                                {
                                                    dhst.WEIGHT = item.WEIGHT;
                                                    dhst.HEIGHT = item.HEIGHT;
                                                }
                                                MapInformationDhstEmpty(item);
                                            }
                                        }
                                    }
                                    else
                                        dhst = this.dhst;
                                    if (ssTeinList != null && ssTeinList.Count > 0)
                                    {
                                        var lstSereServTein = GetTein1(ssTeinList);
                                        List<HIS_MEDICINE_SERVICE> medicine = new List<HIS_MEDICINE_SERVICE>();
                                        foreach (var ssTein in lstSereServTein)
                                        {
                                            var medicineCheck = mediSer.Where(o => ssTein.TEST_INDEX_ID == o.TEST_INDEX_ID).ToList();
                                            if (dhst == null || dhst.ID == 0)
                                                medicineCheck = medicineCheck.Where(o => o.DATA_TYPE == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_SERVICE.DATA_TYPE__EGFR).ToList();
                                            medicineCheck = medicineCheck.Where(o => (o.VALUE_SERVICE_FROM ?? decimal.MinValue) <= GetValueFromDataType(o.DATA_TYPE, ConvertToDecimal(ssTein.VALUE), dhst, chiSoMLCT) && GetValueFromDataType(o.DATA_TYPE, ConvertToDecimal(ssTein.VALUE), dhst, chiSoMLCT) < (o.VALUE_SERVICE_TO ?? decimal.MaxValue) && (o.AMOUNT_INDAY_FROM ?? 0) < (medi.AMOUNT / (medi.UseDays ?? 1) + AmountInDay) && (medi.AMOUNT / (medi.UseDays ?? 1) + AmountInDay) <= (o.AMOUNT_INDAY_TO ?? decimal.MaxValue)).ToList();
                                            if (medicineCheck != null && medicineCheck.Count > 0)
                                                medicine.AddRange(medicineCheck);
                                        }

                                        if (medicine != null && medicine.Count > 0)
                                        {
                                            if (IsShowPopup)
                                            {
                                                var minOverDose = medicine.OrderBy(o => o.AMOUNT_INDAY_FROM ?? 0).ThenByDescending(o => o.ID);
                                                decimal minType = 0;
                                                short? DataType = null;
                                                bool IsFirst = true;
                                                foreach (var item in minOverDose)
                                                {
                                                    var value = GetValueFromDataType(item.DATA_TYPE, ConvertToDecimal(lstSereServTein.FirstOrDefault(p => p.TEST_INDEX_ID == item.TEST_INDEX_ID).VALUE), dhst, chiSoMLCT);
                                                    if (IsFirst)
                                                    {
                                                        minType = value;
                                                        IsFirst = false;
                                                    }
                                                    if (minType > value)
                                                    {
                                                        minType = value;
                                                    }
                                                    DataType = item.DATA_TYPE;
                                                }
                                                frmOverReason frm = new frmOverReason(medicine, string.Format("kê thuốc {0}", medi.MEDICINE_TYPE_NAME), (o) =>
                                                {
                                                    if (!medi.dicTreatmentOverKidneyReason.ContainsKey(itime))
                                                        medi.dicTreatmentOverKidneyReason[itime] = new List<TreatmentOverReason>() { o };
                                                    else
                                                        medi.dicTreatmentOverKidneyReason[itime].Add(o);
                                                }, (o) =>
                                                {
                                                    result = o;
                                                }, medi, false, true, treatmentId, (sdo) =>
                                                {
                                                    if (DataType == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_SERVICE.DATA_TYPE__EGFR)
                                                    {
                                                        sdo.Egfr = minType.ToString();
                                                    }
                                                    else if (DataType == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_SERVICE.DATA_TYPE__CRCL)
                                                    {
                                                        sdo.Clcr = minType.ToString();
                                                    }
                                                    sdo.CreatininValue = chisotestIndexCreatinin > 0 ? chisotestIndexCreatinin.ToString() : null;
                                                    if (lciARCPCR.Text.ToLower().Contains("uacr"))
                                                    {
                                                        sdo.Uacr = lblACRPCR.Text;
                                                    }
                                                    else if (lciARCPCR.Text.ToLower().Contains("upcr"))
                                                    {
                                                        sdo.Upcr = lblACRPCR.Text;
                                                    }
                                                    sdo.IntructionTime = itime;
                                                    AlertLogADO ado = new AlertLogADO(sdo, medi);
                                                    AlertLogsSdo.Add(ado);
                                                }, minOverDose.Select(o => GetValueFromDataType(o.DATA_TYPE, ConvertToDecimal(lstSereServTein.FirstOrDefault(p => p.TEST_INDEX_ID == o.TEST_INDEX_ID).VALUE), dhst, chiSoMLCT)).Min(), minOverDose.FirstOrDefault(), AmountInDay);
                                                frm.ShowDialog();
                                            }
                                            else
                                            {
                                                medi.IsEditOverKidneyReason = true;
                                            }
                                        }    
                                        else
                                        {
                                            medi.OVER_KIDNEY_REASON = null;
                                            medi.OVER_REASON_ID = null;
                                        }
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
            return result;
        }

        private void CreateThreadOverReasonAdd(List<long> treatmentIds)
        {
            Thread threadSsKidney = new Thread(new ParameterizedThreadStart(LoadDataSsKidney));
            Thread threadSsResult = new Thread(new ParameterizedThreadStart(LoadDataSsResultAdd));
            try
            {
                threadSsKidney.Start(treatmentIds);
                threadSsResult.Start(treatmentIds);
                threadSsKidney.Join();
                threadSsResult.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadSsKidney.Abort();
                threadSsResult.Abort();
            }
        }

        private void LoadDataSsResultAdd(object obj)
        {
            try
            {
                sereServTeinResultTest = null;
                medicineServiceTest = null;
                var mediResultTest = medicineService.Where(o => o.DATA_TYPE == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_SERVICE.DATA_TYPE__SERVICE).ToList();
                if (mediResultTest == null || mediResultTest.Count == 0)
                {
                    return;
                }
                List<string> lstIcd = new List<string>();
                var icdValue = UcIcdGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                if (icdValue != null && !string.IsNullOrEmpty(icdValue.ICD_CODE))
                {
                    lstIcd.Add(icdValue.ICD_CODE.Replace(";", ""));
                }

                var icdCauseValue = UcIcdCauseGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                if (icdCauseValue != null && !string.IsNullOrEmpty(icdCauseValue.ICD_CODE))
                {
                    lstIcd.Add(icdCauseValue.ICD_CODE.Replace(";", ""));
                }

                var subIcd = UcSecondaryIcdGetValue() as SecondaryIcdDataADO;
                if (subIcd != null && !string.IsNullOrEmpty(subIcd.ICD_SUB_CODE))
                {
                    lstIcd.AddRange(subIcd.ICD_SUB_CODE.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList());
                }
                medicineServiceTest = mediResultTest.Where(o => o.ICD_CODE.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList().Exists(k => lstIcd.Exists(p => p.Equals(k)))).ToList();
                if (medicineServiceTest != null && medicineServiceTest.Count > 0)
                {
                    HisSereServTeinView1Filter tifilter = new HisSereServTeinView1Filter();
                    tifilter.TEST_INDEX_IDs = medicineServiceTest.Select(o => o.TEST_INDEX_ID ?? 0).ToList();
                    tifilter.TREATMENT_IDs = obj as List<long>;
                    CommonParam param = new CommonParam();
                    sereServTeinResultTest = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_TEIN_1>>("api/HisSereServTein/GetView1", ApiConsumers.MosConsumer, tifilter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThreadOverReasonSave(List<long> treatmentIds)
        {
            Thread threadSsKidney = new Thread(new ParameterizedThreadStart(LoadDataSsKidney));
            Thread threadSsResult = new Thread(new ParameterizedThreadStart(LoadDataSsResult));
            Thread threadDhst = new Thread(new ParameterizedThreadStart(LoadDataDhst));
            Thread threadSsWithMultilTreatment = new Thread(new ParameterizedThreadStart(LoadSsWithMultilTreatment));
            Thread threadSsMety = new Thread(new ParameterizedThreadStart(LoadSsMety));
            try
            {
                threadSsKidney.Start(treatmentIds);
                threadSsResult.Start(treatmentIds);
                threadDhst.Start(treatmentIds);
                threadSsWithMultilTreatment.Start(treatmentIds);

                threadSsMety.Start(treatmentIds);
                threadDhst.Join();
                threadSsKidney.Join();
                threadSsResult.Join();
                threadSsWithMultilTreatment.Join();
                threadSsMety.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadSsKidney.Abort();
                threadSsResult.Abort();
                threadDhst.Abort();
                threadSsWithMultilTreatment.Abort();
                threadSsMety.Abort();
            }
        }


        private void LoadSsWithMultilTreatment(object obj)
        {
            try
            {
                if (sereServWithMultilTreatment != null && sereServWithMultilTreatment.Count > 0)
                    return;
                Inventec.Common.Logging.LogSystem.Debug("LoadDataSereServWithMutilTreatment.1");

                Inventec.Common.Logging.LogSystem.Debug("LoadDataSereServWithMutilTreatment.2");
                CommonParam param = new CommonParam();

                List<long> setyAllowsIds = new List<long>();
                setyAllowsIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT);
                setyAllowsIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC);
                Inventec.Common.Logging.LogSystem.Debug("LoadDataSereServWithMutilTreatment.3");
                HisSereServFilter hisSereServFilter = new HisSereServFilter();
                hisSereServFilter.TREATMENT_IDs = obj as List<long>;
                hisSereServFilter.TDL_SERVICE_TYPE_IDs = setyAllowsIds;
                this.sereServWithMultilTreatment = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumerNoStore, hisSereServFilter, ProcessLostToken, param);
                Inventec.Common.Logging.LogSystem.Debug("LoadDataSereServWithMutilTreatment.4");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadSsMety(object obj)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceReqMetyViewFilter expMestMetyFilter = new HisServiceReqMetyViewFilter();
                expMestMetyFilter.TREATMENT_IDs = obj as List<long>;
                expMestMetyFilter.MEDICINE_TYPE_IDs = mediMatyTypeADOs.Where(o => o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM).Select(o => o.ID).ToList();
                expMestMetyFilter.INTRUCTION_DATE_FROM = Int64.Parse(intructionTimeSelecteds.OrderByDescending(o => o).LastOrDefault().ToString().Substring(0, 8) + "000000");
                expMestMetyFilter.INTRUCTION_DATE_TO = Int64.Parse(intructionTimeSelecteds.OrderByDescending(o => o).FirstOrDefault().ToString().Substring(0, 8) + "000000");
                this.serviceReqMetyViewInDay = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ_METY>>("api/HisServiceReqMety/GetView", ApiConsumers.MosConsumer, expMestMetyFilter, ProcessLostToken, param);
                if (this.serviceReqMetyViewInDay == null)
                    this.serviceReqMetyViewInDay = new List<V_HIS_SERVICE_REQ_METY>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataDhst(object obj)
        {
            try
            {
                var mediResultTest = medicineService.Where(o => o.DATA_TYPE != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_SERVICE.DATA_TYPE__SERVICE).ToList();
                if (mediResultTest == null || mediResultTest.Count == 0)
                {
                    dhstlist = null;
                    return;
                }
                HisDhstFilter dhstFilter = new HisDhstFilter();
                dhstFilter.TREATMENT_IDs = obj as List<long>;
                dhstFilter.ORDER_FIELD = "EXECUTE_TIME";
                dhstFilter.ORDER_DIRECTION = "DESC";
                CommonParam param = new CommonParam();
                dhstlist = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDHST/Get", ApiConsumers.MosConsumer, dhstFilter, param);
                if (dhstlist != null && dhstlist.Count > 0)
                    dhstlist = dhstlist.Where(o => o.WEIGHT.HasValue).OrderByDescending(o => o.EXECUTE_TIME).ThenByDescending(o => o.ID).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataSsResult(object obj)
        {
            try
            {
                var mediResultTest = medicineService.Where(o => o.DATA_TYPE == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_SERVICE.DATA_TYPE__SERVICE).ToList();
                if (mediResultTest == null || mediResultTest.Count == 0)
                {
                    sereServTeinResultTest = null;
                    return;
                }
                HisSereServTeinView1Filter tifilter = new HisSereServTeinView1Filter();
                tifilter.TEST_INDEX_IDs = mediResultTest.Select(o => o.TEST_INDEX_ID ?? 0).ToList();
                tifilter.TREATMENT_IDs = obj as List<long>;
                CommonParam param = new CommonParam();
                sereServTeinResultTest = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_TEIN_1>>("api/HisSereServTein/GetView1", ApiConsumers.MosConsumer, tifilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataSsKidney(object obj)
        {
            try
            {
                var mediKidney = medicineService.Where(o => o.DATA_TYPE != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_SERVICE.DATA_TYPE__SERVICE).ToList();
                if (mediKidney == null || mediKidney.Count == 0)
                {
                    sereServTeinKidney = null;
                    return;
                }
                HisSereServTeinView1Filter tifilter = new HisSereServTeinView1Filter();
                tifilter.TEST_INDEX_IDs = mediKidney.Select(o => o.TEST_INDEX_ID ?? 0).ToList();
                tifilter.TREATMENT_IDs = obj as List<long>;
                CommonParam param = new CommonParam();
                sereServTeinKidney = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_TEIN_1>>("api/HisSereServTein/GetView1", ApiConsumers.MosConsumer, tifilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        internal decimal GetValueFromDataType(short? type, decimal value, HIS_DHST dhst, decimal? chiSoMLCT)
        {
            decimal result = 0;
            try
            {
                switch (type)
                {
                    case IMSys.DbConfig.HIS_RS.HIS_MEDICINE_SERVICE.DATA_TYPE__EGFR:
                        //Công thức eGFR 
                        if (HisConfigCFG.ASSIGNPRESCRIPTION_EGFROPTION == "1")
                        {
                            result = 186 * (decimal)Math.Pow((double)(chiSoMLCT * value), (double)(-1.154)) * (decimal)Math.Pow(Inventec.Common.DateTime.Calculation.Age(patientDob),
                                (double)(-0.203)) * (genderName.ToLower().Equals("nữ") ? (decimal)0.742 : 1);
                        }
                        else
                        {
                            result = 175 * (decimal)Math.Pow((double)(chiSoMLCT * value), (double)(-1.154)) * (decimal)Math.Pow(Inventec.Common.DateTime.Calculation.Age(patientDob),
                                (double)(-0.203)) * (genderName.ToLower().Equals("nữ") ? (decimal)0.742 : 1);
                        }

                        break;
                    case IMSys.DbConfig.HIS_RS.HIS_MEDICINE_SERVICE.DATA_TYPE__CRCL:
                        //Công thức CrCl
                        result = ((140 - Inventec.Common.DateTime.Calculation.Age(patientDob)) * (dhst != null && dhst.ID > 0 ? dhst.WEIGHT * (genderName.ToLower().Equals("nữ") ? (decimal)0.85 : 1) : 1)) / (72 * chiSoMLCT * value) ?? 0;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            Inventec.Common.Logging.LogSystem.Warn(result + "__VALUE" + value);
            return result;
        }

        internal decimal ConvertToDecimal(string value)
        {
            decimal result = 0;
            try
            {
                if (string.IsNullOrEmpty(value))
                    return 0;
                CultureInfo culture = new CultureInfo("en-US");
                if (value.Contains(","))
                    culture = new CultureInfo("fr-FR");
                result = Convert.ToDecimal(value, culture);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }   
            return result;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //ProcessSaveForListSelect(HIS.Desktop.Plugins.AssignPrescriptionPK.SAVETYPE.SAVE);

                LogTheadInSessionInfo(() => ProcessSaveForListSelect(HIS.Desktop.Plugins.AssignPrescriptionPK.SAVETYPE.SAVE), !GlobalStore.IsCabinet ? "SavePrescription" : "SaveMedicalStore");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSaveAndPrint_Click(object sender, EventArgs e)
        {
            try
            {
                //ProcessSaveForListSelect(HIS.Desktop.Plugins.AssignPrescriptionPK.SAVETYPE.SAVE_PRINT_NOW);
                LogTheadInSessionInfo(() => ProcessSaveForListSelect(HIS.Desktop.Plugins.AssignPrescriptionPK.SAVETYPE.SAVE_PRINT_NOW), !GlobalStore.IsCabinet ? "SaveAndPrintPrescription" : "SaveAndPrintMedicalStore");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Kê đơn phòng khám khi click vào nút lưu sẽ xử lý gọi api server => tạo/tách/ sửa/ xóa bean của thuốc luôn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_TabMedicine_Click(object sender, EventArgs e)
        {
            LockByKeyConfig();
            LogTheadInSessionInfo(AddTabMedicine, !GlobalStore.IsCabinet ? "AddPrescription" : "AddMedicalStore");
        }

        private void AddTabMedicine()
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionView || !btnAdd.Enabled)
                {
                    LogSystem.Warn("btnAdd_TabMedicine_Click => thao tac khong hop le. actionType = " + this.actionType + " Trang thai = " + btnAdd.Enabled);
                    return;
                }
                this.NoEdit = false;
                if (HisConfigCFG.CheckSoNgay && this.spinSoLuongNgay.Value != 0 && !string.IsNullOrEmpty(this.spinAmount.Text))
                {
                    var amountSTCT = (this.GetValueSpin(this.spinSang.Text) + this.GetValueSpin(this.spinTrua.Text) + this.GetValueSpin(this.spinChieu.Text) + this.GetValueSpin(this.spinToi.Text));
                    var amountNeedCheck = (double)this.spinSoLuongNgay.Value * (amountSTCT > 0 ? amountSTCT : 1);

                    double valueAmount = 0;
                    var vl = spinAmount.Text.Trim();
                    vl = vl.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                    vl = vl.Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                    if (vl.Contains("/"))
                    {
                        var arrNumber = vl.Split('/');
                        if (arrNumber != null && arrNumber.Count() > 1)
                        {
                            valueAmount = Convert.ToDouble(arrNumber[0]) / Convert.ToDouble(arrNumber[1]);
                        }
                    }
                    else
                    {
                        valueAmount = Convert.ToDouble(vl);
                    }

                    if (amountNeedCheck != valueAmount && MessageBox.Show("Số ngày * (Sáng + Trưa + Chiều + Tối) khác Số lượng. Bạn có muốn tiếp tục?", "Cảnh báo", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(amountNeedCheck + "__" + valueAmount + " __Bo sung thuoc/vat tu that bai do khac so luong____");
                        return;
                    }
                }
                bool valid = true;
                this.positionHandleControl = -1;
                var selectedOpionGroup = GetSelectedOpionGroup();
                Inventec.Common.Logging.LogSystem.Debug("btnAdd_TabMedicine_Click.0____selectedOpionGroup=" + selectedOpionGroup);
                Inventec.Common.Logging.LogSystem.Debug("btnAdd_TabMedicine_Click.1____valid=" + valid);
                //valid = valid && (selectedOpionGroup == 1 ? dxValidProviderBoXung.Validate() : true);
                valid = valid && dxValidProviderBoXung.Validate();
                Inventec.Common.Logging.LogSystem.Debug("btnAdd_TabMedicine_Click.2____valid=" + valid);
                valid = valid && (selectedOpionGroup != 3 ? CheckAllergenicByPatient() : true);
                Inventec.Common.Logging.LogSystem.Debug("btnAdd_TabMedicine_Click.3____valid=" + valid);
                Inventec.Common.Logging.LogSystem.Debug("btnAdd_TabMedicine_Click.4____valid=" + valid);
                //valid = valid && CheckContraidication();
                valid = valid && (selectedOpionGroup != 3 ? CheckMaxInPrescription(currentMedicineTypeADOForEdit, (decimal)this.GetValueSpinHasAround(this.spinAmount.Text)) : true);
                var _amount = this.spinSoLuongNgay.Value != 0 ? (decimal)this.GetValueSpinHasAround(this.spinAmount.Text) / this.spinSoLuongNgay.Value : (decimal)this.GetValueSpinHasAround(this.spinAmount.Text);
                valid = valid && (selectedOpionGroup != 3 ? CheckMaxInPrescriptionInDay(currentMedicineTypeADOForEdit, _amount) : true);
                //valid = valid && (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet) ? CheckMaxInPrescriptionInBatchWithMultilPatient(currentMedicineTypeADOForEdit, _amount) : CheckMaxInPrescriptionInBatch(currentMedicineTypeADOForEdit, _amount);
                valid = valid && (selectedOpionGroup != 3 ? CheckOddPrescription(currentMedicineTypeADOForEdit, (decimal)this.GetValueSpinHasAround(this.spinAmount.Text)) : true);
                Inventec.Common.Logging.LogSystem.Debug("btnAdd_TabMedicine_Click.5____valid=" + valid);
                valid = valid && CheckGenderMediMaty(currentMedicineTypeADOForEdit);
                Inventec.Common.Logging.LogSystem.Debug("btnAdd_TabMedicine_Click.6____valid=" + valid);
                valid = valid && CheckMaMePackage(currentMedicineTypeADOForEdit);
                Inventec.Common.Logging.LogSystem.Debug("btnAdd_TabMedicine_Click.7____valid=" + valid);
                //valid = valid && (selectedOpionGroup != 3 ? CheckOddConvertUnit(currentMedicineTypeADOForEdit, (decimal)this.GetValueSpin(this.spinAmount.Text)) : true);
                valid = valid && (selectedOpionGroup == 3 ? dxValidationProviderMaterialTypeTSD.Validate() : true);

                valid = valid && (selectedOpionGroup == 2 ? CheckAmoutMediMaty(currentMedicineTypeADOForEdit) : true);

                valid = valid && (selectedOpionGroup == 1 ? CheckMedicineGroupWarning() : true);
                valid = valid && CheckMedicineGroupTuberCulosis(false);
                Inventec.Common.Logging.LogSystem.Debug("btnAdd_TabMedicine_Click.8____valid=" + valid);
                if (!valid)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Bo sung thuoc/vat tu that bai____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => valid), valid));
                    return;
                }

                if (this.mediMatyTypeADOs == null)
                    this.mediMatyTypeADOs = new List<MediMatyTypeADO>();

                if (CheckMediMatyType(this.mediMatyTypeADOs) == false)
                {
                    return;
                }

                switch (this.actionBosung)
                {
                    case GlobalVariables.ActionAdd:
                        AddMediMatyClickHandler();
                        break;
                    case GlobalVariables.ActionEdit:
                        UpdateMediMatyClickHandler();
                        break;
                    default:
                        LogSystem.Warn("btnAdd_TabMedicine_Click => thao tac khong hop le. actionBosung = " + this.actionBosung);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Co loi xay ra khi bo sung thuoc/vat tu", ex);
            }
        }

        private Boolean CheckMediMatyType(List<MediMatyTypeADO> LstMediMatyTypeADO)
        {
            try
            {
                if (HisConfigCFG.ConnectDrugInterventionInfo == "1")
                {

                    HIS.Desktop.Plugins.Library.DrugInterventionInfo.ADO.InputCheckDataADO InputCheckData = new Library.DrugInterventionInfo.ADO.InputCheckDataADO();

                    InputCheckData.HisServiceReq = new HIS_SERVICE_REQ();

                    InputCheckData.HisServiceReq.INTRUCTION_TIME = this.InstructionTime;
                    InputCheckData.HisServiceReq.ICD_CODE = txtIcdCode.Text;
                    InputCheckData.HisServiceReq.ICD_NAME = cboIcds.Text;
                    InputCheckData.HisServiceReq.ICD_CAUSE_CODE = txtIcdCodeCause.Text;
                    InputCheckData.HisServiceReq.ICD_CAUSE_NAME = cboIcdsCause.Text;
                    InputCheckData.HisServiceReq.ICD_SUB_CODE = txtIcdSubCode.Text;
                    InputCheckData.HisServiceReq.ICD_TEXT = txtIcdText.Text;
                    InputCheckData.HisServiceReq.EXECUTE_DEPARTMENT_ID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetDepartmentId(this.currentModule.RoomTypeId);
                    InputCheckData.HisServiceReq.EXECUTE_ROOM_ID = this.currentModule.RoomId;

                    InputCheckData.ListMediMateCheck = new List<Library.DrugInterventionInfo.ADO.MediMateCheckADO>();

                    Library.DrugInterventionInfo.ADO.MediMateCheckADO mediMate = new Library.DrugInterventionInfo.ADO.MediMateCheckADO();

                    Inventec.Common.Mapper.DataObjectMapper.Map<Library.DrugInterventionInfo.ADO.MediMateCheckADO>(mediMate, this.currentMedicineTypeADOForEdit);

                    InputCheckData.ListMediMateCheck.Add(mediMate);

                    if (LstMediMatyTypeADO != null && LstMediMatyTypeADO.Count > 0)
                    {
                        foreach (var item in LstMediMatyTypeADO)
                        {
                            Library.DrugInterventionInfo.ADO.MediMateCheckADO ado = new Library.DrugInterventionInfo.ADO.MediMateCheckADO();

                            Inventec.Common.Mapper.DataObjectMapper.Map<Library.DrugInterventionInfo.ADO.MediMateCheckADO>(ado, item);

                            InputCheckData.ListMediMateCheck.Add(ado);
                        }
                    }

                    HIS.Desktop.Plugins.Library.DrugInterventionInfo.DrugInterventionInfoProcessor checkData = new Library.DrugInterventionInfo.DrugInterventionInfoProcessor(HisConfigCFG.ConnectionInfo, VHistreatment);
                    HIS.Desktop.Plugins.Library.DrugInterventionInfo.ADO.OutputResultADO result = checkData.CheckPrescription(InputCheckData);

                    if (result != null && result.AlertLevel == Library.DrugInterventionInfo.ADO.AlertSeverityLevel.Contraindicated)
                    {
                        MessageBox.Show(result.Message, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return false;
                    }
                    else if (result != null && result.AlertLevel == Library.DrugInterventionInfo.ADO.AlertSeverityLevel.Warning)
                    {
                        if (MessageBox.Show(result.Message, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                return true;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        internal bool CheckConditionService(MediMatyTypeADO sereServADO)
        {
            bool success = true;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("CheckConditionService. 1.12.1");
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServADO.PATIENT_TYPE_ID), sereServADO.PATIENT_TYPE_ID)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServADO.SERVICE_ID), sereServADO.SERVICE_ID));
                //var dataCondition = BranchDataWorker.ServicePatyWithListPatientType(sereServADO.SERVICE_ID, new List<long> { (sereServADO.PATIENT_TYPE_ID > 0 ? sereServADO.PATIENT_TYPE_ID ?? 0 : this.currentHisPatientTypeAlter != null ? this.currentHisPatientTypeAlter.PATIENT_TYPE_ID : 0) });
                long patientTypeId = (sereServADO.PATIENT_TYPE_ID > 0 ? sereServADO.PATIENT_TYPE_ID ?? 0 : this.currentHisPatientTypeAlter != null ? this.currentHisPatientTypeAlter.PATIENT_TYPE_ID : 0);
                var dataCondition = (workingServiceConditions != null && patientTypeId == HisConfigCFG.PatientTypeId__BHYT) ? workingServiceConditions.Where(o => o.SERVICE_ID == sereServADO.SERVICE_ID).ToList() : null;
                if (dataCondition != null && dataCondition.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("CheckConditionService. 1.12.2");
                    List<HIS_SERVICE_CONDITION> dataConditionTmps = new List<HIS_SERVICE_CONDITION>();
                    foreach (var item in dataCondition)
                    {
                        if (dataConditionTmps.Count == 0 || !dataConditionTmps.Exists(t => t.SERVICE_CONDITION_NAME == item.SERVICE_CONDITION_NAME && t.HEIN_RATIO == item.HEIN_RATIO))
                        {
                            dataConditionTmps.Add(item);
                        }
                    }
                    dataCondition.Clear();
                    dataCondition.AddRange(dataConditionTmps);
                    Inventec.Common.Logging.LogSystem.Debug("CheckConditionService. 1.12.4");
                    GridViewInfo info = gridViewServiceProcess.GetViewInfo() as GridViewInfo;
                    Inventec.Common.Logging.LogSystem.Debug("CheckConditionService. 1.12.5");
                    GridCellInfo cellInfo = info.GetGridCellInfo(gridViewServiceProcess.FocusedRowHandle, gridColumnSERVICE_CONDITION_NAME);
                    Inventec.Common.Logging.LogSystem.Debug("CheckConditionService. 1.12.6");
                    //TODO
                    Rectangle buttonPosition = cellInfo != null ? cellInfo.Bounds : default(Rectangle);
                    popupControlContainerCondition.ShowPopup(new Point(buttonPosition.X + 0, buttonPosition.Bottom + 288));

                    gridControlCondition.DataSource = null;
                    gridControlCondition.DataSource = dataCondition;
                    gridControlCondition.Focus();
                    gridViewCondition.FocusedRowHandle = 0;
                    Inventec.Common.Logging.LogSystem.Debug("CheckConditionService. 1.12.7");
                }
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Warn(exx);
            }
            return success;
        }

        private bool CheckAllergenicByPatient()
        {
            bool result = true;
            try
            {
                Inventec.Common.Logging.LogSystem.Info("CheckAllergenicByPatient.1");
                if (this.currentMedicineTypeADOForEdit != null)
                {
                    if (allergenics == null || allergenics.Count == 0)
                        return result;
                    HIS_ALLERGENIC allergencic = allergenics.FirstOrDefault(o => o.MEDICINE_TYPE_ID == this.currentMedicineTypeADOForEdit.ID);
                    string messageError = "";
                    if (allergencic != null)
                    {
                        string messageErrorRow = String.Format("{0} {1}: {2}.", "-", currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME, allergencic.CLINICAL_EXPRESSION);
                        if (allergencic.IS_SURE == 1)
                        {
                            Inventec.Common.Logging.LogSystem.Info("CheckAllergenicByPatient.2");
                            messageError = Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag(ResourceMessage.CanhBaoDiUngThuoc, Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            messageError += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag(messageErrorRow, Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            messageError = Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag(messageError, Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br) + ResourceMessage.BanCoMuonTiepTuc.Replace("{0} {1}. ", "");
                        }
                        else if (allergencic.IS_DOUBT == 1)
                        {
                            Inventec.Common.Logging.LogSystem.Info("CheckAllergenicByPatient.3");
                            messageError = Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag(ResourceMessage.BenhNhanNghiNgoDiUngVoiThuocXBanCoMuonTiepTuc, Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            messageError += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag(messageErrorRow, Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            messageError = Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag(messageError, Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br) + ResourceMessage.BanCoMuonTiepTuc.Replace("{0} {1}. ", "");

                        }

                        if (!String.IsNullOrEmpty(messageError))
                        {
                            DialogResult myResult;
                            myResult = XtraMessageBox.Show(messageError, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question, DefaultBoolean.True);
                            if (myResult != DialogResult.Yes)
                            {
                                result = false;
                            }
                        }
                    }
                }
                Inventec.Common.Logging.LogSystem.Info("CheckAllergenicByPatient.4");
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckContraidication()
        {
            bool result = true;
            try
            {
                if (this.currentMedicineTypeADOForEdit != null && !String.IsNullOrEmpty(this.currentMedicineTypeADOForEdit.CONTRAINDICATION))
                {
                    DialogResult myResult;
                    myResult = MessageBox.Show(String.Format(ResourceMessage.BanCoMuonTiepTuc, this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME, this.currentMedicineTypeADOForEdit.CONTRAINDICATION), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (myResult != DialogResult.Yes)
                    {
                        result = false;
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

        private void btnTomLuocVienPhi_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrHospitalFees").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.AggrHospitalFees'");
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.AggrHospitalFees' is not plugins");

                List<object> listArgs = new List<object>();
                listArgs.Add(treatmentId);
                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, GetRoomId(), GetRoomTypeId()), listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");

                WaitingManager.Hide();
                ((Form)extenceInstance).Show();
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnF2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.ResetFocusMediMaty(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnF3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.spinAmount.Focus();
                this.spinAmount.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barbtnF4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.patientSelectProcessor != null && this.ucPatientSelect != null)
                {
                    this.patientSelectProcessor.FocusSearchTextbox(this.ucPatientSelect);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnBoSung_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionAdd
                    || this.actionType == GlobalVariables.ActionEdit)
                {
                    Inventec.Common.Logging.LogSystem.Warn("bbtnBoSung_ItemClick");
                    this.btnAdd.Focus();
                    this.btnAdd_TabMedicine_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barbtnClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barbtnSaveShortcut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave.Focus();
                if ((this.actionType == GlobalVariables.ActionAdd || this.actionType == GlobalVariables.ActionEdit) && this.btnSave.Enabled && this.btnSave.Visible)
                {
                    this.btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barbtnSaveAndPrintShortcut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSaveAndPrint.Focus();
                if ((this.actionType == GlobalVariables.ActionAdd || this.actionType == GlobalVariables.ActionEdit) && this.btnSaveAndPrint.Enabled && this.btnSaveAndPrint.Visible)
                {
                    this.btnSaveAndPrint_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barbtnPrintShortcut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionView && ((this.lciPrintAssignPrescription.Enabled && this.lciPrintAssignPrescription.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always) || (this.lciPrintAssignPrescriptionExt.Enabled && this.lciPrintAssignPrescriptionExt.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)))
                {
                    var printTypeCode = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.SAVE_PRINT_MPS_DEFAULT);
                    if (String.IsNullOrEmpty(printTypeCode))
                    {
                        printTypeCode = MPS.Processor.Mps000118.PDO.Mps000118PDO.PrintTypeCode;
                    }
                    PrescriptionSavePrintShowHasClickSave(printTypeCode, false, MPS.ProcessorBase.PrintConfig.PreviewType.Show);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barbtnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.btnNew.Enabled && this.btnNew.Visible)
                    this.btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

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

        private void popupControlContainerMediMaty_CloseUp(object sender, EventArgs e)
        {
            try
            {
                isShow = true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void cboPhieuDieuTri_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboPhieuDieuTri.EditValue != null)
                    {
                        cboPhieuDieuTri.Properties.Buttons[1].Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhieuDieuTri_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPhieuDieuTri.Properties.Buttons[1].Visible = false;
                    cboPhieuDieuTri.EditValue = null;
                }
                else if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TrackingCreate").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TrackingCreate");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(this.treatmentId);
                        listArgs.Add((Action<HIS_TRACKING>)this.ProcessAfterChangeTrackingTime);//TODO
                        if (this.currentDhst != null)
                        {
                            listArgs.Add(this.currentDhst);
                        }
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();

                        this.LoadDataTracking();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void refeshChooseIcd(object data)
        {
            try
            {
                if (data != null)
                {
                    icdChoose = new HIS_ICD();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_ICD>(icdChoose, data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTutorial_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.DropDown)
                {
                    isShowContainerTutorial = !isShowContainerTutorial;
                    if (isShowContainerTutorial)
                    {
                        Rectangle buttonBounds = new Rectangle(txtTutorial.Bounds.X, txtTutorial.Bounds.Y, txtTutorial.Bounds.Width, txtTutorial.Bounds.Height);
                        popupControlContainerTutorial.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 25));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboEquipment_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.cboEquipment.Properties.Buttons[1].Visible = false;
                    this.cboEquipment.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEquipment_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboEquipment.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_EQUIPMENT_SET data = (cboEquipment.Properties.DataSource as List<HIS_EQUIPMENT_SET>).SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboEquipment.EditValue ?? "0").ToString()));
                        if (data != null)
                        {
                            this.cboEquipment.Properties.Buttons[1].Visible = true;
                            this.ProcessChoiceEquipmentSet(data);
                            this.ResetFocusMediMaty(true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTutorial_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinSang_Leave(object sender, EventArgs e)
        {
            this.CalculateAmount();
            this.SetHuongDanFromSoLuongNgay();
        }

        private void spinTrua_Leave(object sender, EventArgs e)
        {
            this.CalculateAmount();
            this.SetHuongDanFromSoLuongNgay();
        }

        private void spinChieu_Leave(object sender, EventArgs e)
        {
            this.CalculateAmount();
            this.SetHuongDanFromSoLuongNgay();
        }

        private void spinToi_Leave(object sender, EventArgs e)
        {
            this.CalculateAmount();
            this.SetHuongDanFromSoLuongNgay();
        }

        private void spinSoNgay_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.treatmentFinishProcessor != null)
                    this.treatmentFinishProcessor.Reload(this.ucTreatmentFinish, this.GetDateADO());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboNhaThuoc_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                GridLookUpEdit editor = sender as GridLookUpEdit;
                editor.Properties.Buttons[1].Visible = (editor.EditValue != null);
                if (HisConfigCFG.IsAutoCreateSaleExpMest || HisConfigCFG.IsDrugStoreComboboxOption)
                {
                    if (editor.EditValue == null)
                    {
                        this.currentMediStockNhaThuocSelecteds = new List<V_HIS_MEDI_STOCK>();
                        RebuildMedicineTypeWithInControlContainer();
                    }
                    else
                    {
                        var nt = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.ID == (long)editor.EditValue).FirstOrDefault();
                        if (nt != null)
                        {
                            this.currentMediStockNhaThuocSelecteds = new List<V_HIS_MEDI_STOCK>();
                            this.currentMediStockNhaThuocSelecteds.Add(nt);
                        }
                        RebuildNhaThuocMediMatyWithInControlContainer();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNhaThuoc_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.ResetComboNhaThuoc();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNhaThuoc_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.MEDI_STOCK_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNhaThuoc_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    WaitingManager.Show();

                    if (this.currentMediStockNhaThuocSelecteds != null && this.currentMediStockNhaThuocSelecteds.Count > 0)
                        RebuildNhaThuocMediMatyWithInControlContainer();
                    else
                        if (HisConfigCFG.OutStockListItemInCaseOfNoStockChosenOption == "2") //#26617
                        RebuildNhaThuocMediMatyWithInControlContainerWithConfig();
                    else
                        RebuildMedicineTypeWithInControlContainer();


                    Inventec.Common.Logging.LogSystem.Debug("cboNhaThuoc_Closed. 2" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentMediStockNhaThuocSelecteds), this.currentMediStockNhaThuocSelecteds));
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtLoginName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        this.cboUser.EditValue = null;
                        this.FocusShowpopup(this.cboUser, true);
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>()
                            .Where(o => o.LOGINNAME.ToUpper().Contains(searchCode.ToUpper())).ToList();
                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.LOGINNAME.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboUser.EditValue = searchResult[0].LOGINNAME;
                            this.txtLoginName.Text = searchResult[0].LOGINNAME;

                            CheckAssignServiceSimultaneityOption();
                            this.ResetFocusMediMaty(true);
                        }
                        else
                        {
                            this.cboUser.EditValue = null;
                            this.FocusShowpopup(this.cboUser, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboUser_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                GridLookUpEdit cbo = sender as GridLookUpEdit;
                if (cbo != null && cbo.ContainsFocus)
                    this.OpionGroupSelectedChanged();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtLoginName_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void cboUser_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboUser.EditValue != null)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME == (this.cboUser.EditValue ?? "").ToString());
                        if (data != null)
                        {
                            this.txtLoginName.Text = data.LOGINNAME;
                            CheckAssignServiceSimultaneityOption();
                        }
                    }

                    this.ResetFocusMediMaty(true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboUser_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboUser.EditValue != null)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME == (this.cboUser.EditValue ?? "").ToString());
                        if (data != null)
                        {
                            this.txtLoginName.Text = data.LOGINNAME;

                            CheckAssignServiceSimultaneityOption();
                            this.ResetFocusMediMaty(true);
                        }
                    }
                }
                else
                {
                    this.cboUser.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void DelegateSelectMultiDate(List<DateTime?> datas, DateTime time)
        {
            try
            {
                //this.InitComboTracking(cboPhieuDieuTri);
                this.intructionTimeSelecteds = this.UcDateGetValue();
                this.isMultiDateState = this.UcDateGetChkMultiDateState();
                ChangeIntructionTime(time);
                this.GetListEMMedicineAcinInteractive();
                EnableCheckTemporaryPres();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangeIntructionTime(DateTime intructTime)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ChangeIntructionTime. 1");
                if (this.isNotLoadWhileChangeInstructionTimeInFirst)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isNotLoadWhileChangeInstructionTimeInFirst), isNotLoadWhileChangeInstructionTimeInFirst));
                    return;
                }

                this.isMultiDateState = this.UcDateGetChkMultiDateState();
                if (!this.isMultiDateState)
                {
                    this.intructionTimeSelected = new List<DateTime?>();
                    this.intructionTimeSelected.Add(intructTime);
                    this.intructionTimeSelecteds = (this.intructionTimeSelected.Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o.Value.ToString("yyyyMMdd") + timeSelested.ToString("HHmm") + "00")).OrderByDescending(o => o).ToList());
                }

                this.InstructionTime = this.intructionTimeSelecteds.OrderByDescending(o => o).First();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => intructionTimeSelectedForMedi), intructionTimeSelectedForMedi) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => intructionTimeSelectedsForMedi), intructionTimeSelectedsForMedi));
                this.currentTreatmentWithPatientType = this.LoadDataToCurrentTreatmentData(treatmentId, this.InstructionTime);
                this.LoadDataSereServWithTreatment(this.currentTreatmentWithPatientType, this.InstructionTime);
                this.LoadTotalSereServByHeinWithTreatment();
                this.PatientTypeWithTreatmentView7();
                this.InitComboRepositoryPatientType(this.repositoryItemcboPatientType_TabMedicine_GridLookUp, this.currentPatientTypeWithPatientTypeAlter);
                this.FillTreatmentInfo__PatientType();
                this.cboPhieuDieuTri.EditValue = null;
                this.LoadDataTracking();

                this.InitComboMediStockAllow(0);
                InitDataServiceReqAllInDay();
                if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0)
                {
                    DateTime dtUseTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(licUseTime.Visible && this.UseTime > 0 ? this.UseTime : this.InstructionTime).Value;
                    foreach (var item in this.mediMatyTypeADOs)
                    {
                        if (item.UseDays > 0)
                        {
                            DateTime dtUseTimeTo = dtUseTime.AddDays((double)(item.UseDays.Value - 1));
                            item.UseTimeTo = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtUseTimeTo);
                        }
                        if (!item.IsMultiDateState && item.IntructionTimeSelecteds != null && item.IntructionTimeSelecteds.Count > 0)
                        {
                            item.IntructionTimeSelecteds = this.intructionTimeSelecteds;
                        }
                    }
                    if (CheckMediMatyType(this.mediMatyTypeADOs) == false)
                    {
                        return;
                    }
                    this.RefeshResourceGridMedicine();
                }
                if (HisConfigCFG.IsDontPresExpiredTime)
                {
                    this.OpionGroupSelectedChangedAsync();
                }
                LoadMLCT();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.intructionTimeSelecteds), this.intructionTimeSelecteds));
                Inventec.Common.Logging.LogSystem.Debug("ChangeIntructionTime. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediStockExport_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("cboMediStockExport_EditValueChanged.1");
                if (this.isNotLoadMediMatyByMediStockInitForm)
                {
                    this.isNotLoadMediMatyByMediStockInitForm = false;
                    return;
                }
                if (IsHandlerWhileOpionGroupSelectedIndexChanged)
                {
                    return;
                }

                if (cboMediStockExport.EditValue != null && isMediMatyIsOutStock)
                {
                    List<V_HIS_MEST_ROOM> lst = cboMediStockExport.Properties.DataSource as List<V_HIS_MEST_ROOM>;
                    this.currentMediStock = new List<V_HIS_MEST_ROOM>();
                    if (lst != null && lst.Count > 0)
                    {
                        this.currentMediStock = lst.Where(o => o.MEDI_STOCK_ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMediStockExport.EditValue.ToString())).ToList();
                    }
                }

                if (!cboMediStockExport.IsPopupOpen)
                    LoadDataToGridMetyMatyTypeInStock();
                Inventec.Common.Logging.LogSystem.Debug("cboMediStockExport_EditValueChanged.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediStockExport_TabMedicine_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.ResetFocusMediMaty(false);
                    this.currentMediStock = new List<V_HIS_MEST_ROOM>();
                    GridCheckMarksSelection gridCheckMark = cboMediStockExport.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                        gridCheckMark.ClearSelection(cboMediStockExport.Properties.View);
                    this.cboMediStockExport.EditValue = null;
                    //this.cboMediStockExport.Properties.Buttons[1].Visible = false;
                    this.gridControlServiceProcess.DataSource = null;
                    this.mediStockD1ADOs = new List<DMediStock1ADO>();
                    this.mediMatyTypeAvailables = new List<D_HIS_MEDI_STOCK_2>();
                    this.idRow = 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediStockExport_TabMedicine_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    WaitingManager.Show();
                    this.ProcessChangeMediStock();
                    this.spinSoNgay.Focus();
                    this.spinSoNgay.SelectAll();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediStockExport_TabMedicine_KeyUp(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    WaitingManager.Show();
                    this.ProcessChangeMediStock();
                    this.spinSoNgay.Focus();
                    this.spinSoNgay.SelectAll();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessChangeMediStock()
        {
            try
            {
                this.InitDataMetyMatyTypeInStockD();
                if (rdOpionGroup.SelectedIndex == 0)
                {
                    this.RebuildMediMatyWithInControlContainer();
                }

                if (this.currentMediStock == null || this.currentMediStock.Count == 0)
                    this.idRow = 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediStockExport_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {

                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.MEDI_STOCK_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediStockExport__SelectionChange(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("cboMediStockExport__SelectionChange.1");
                WaitingManager.Show();
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<V_HIS_MEST_ROOM> mestRoomSelectedNews = new List<V_HIS_MEST_ROOM>();
                    foreach (MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.MEDI_STOCK_NAME.ToString());
                            mestRoomSelectedNews.Add(rv);
                        }
                    }
                    this.currentMediStock = new List<V_HIS_MEST_ROOM>();
                    this.currentMediStock.AddRange(mestRoomSelectedNews);
                }
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentMediStock), currentMediStock) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sb), sb.ToString()));
                this.cboMediStockExport.Text = sb.ToString();

                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Debug("cboMediStockExport__SelectionChange.2");
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTemplate_Medicine_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    this.LoadTemplateMedicineCombo(this.cboExpMestTemplate, this.txtExpMestTemplateCode, strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadTemplateMedicineCombo(GridLookUpEdit cboTemplateMedicine, TextEdit txtTemplateMedicineCode, string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboTemplateMedicine.Properties.Buttons[1].Visible = false;
                    cboTemplateMedicine.EditValue = null;
                    this.FocusShowpopup(cboTemplateMedicine, false);
                }
                else
                {
                    List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE> lstData = (cboExpMestTemplate.Properties.DataSource as List<HIS_EXP_MEST_TEMPLATE>);
                    var data = lstData.Where(o => o.EXP_MEST_TEMPLATE_CODE.ToLower().Contains(searchCode.ToLower())).ToList();
                    var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.EXP_MEST_TEMPLATE_CODE.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                    if (searchResult != null && searchResult.Count == 1)
                    {
                        cboTemplateMedicine.Properties.Buttons[1].Visible = true;
                        cboTemplateMedicine.EditValue = searchResult[0].ID;
                        txtTemplateMedicineCode.Text = searchResult[0].EXP_MEST_TEMPLATE_CODE;
                        this.ProcessChoiceExpMestTemplate(searchResult[0]);
                    }
                    else
                    {
                        cboTemplateMedicine.Properties.Buttons[1].Visible = false;
                        cboTemplateMedicine.EditValue = null;
                        this.FocusShowpopup(cboTemplateMedicine, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTemplate_Medicine_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.cboExpMestTemplate.Properties.Buttons[1].Visible = false;
                    this.cboExpMestTemplate.EditValue = null;
                    this.txtExpMestTemplateCode.Text = "";
                    this.txtExpMestTemplateCode.Focus();
                    this.txtExpMestTemplateCode.SelectAll();
                }
                else if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    List<object> listObj = new List<object>();

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisExpMestTemplate", currentModule.RoomId, currentModule.RoomTypeId, listObj);
                    this.InitComboExpMestTemplate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTemplate_Medicine_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboExpMestTemplate.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE data = (cboExpMestTemplate.Properties.DataSource as List<HIS_EXP_MEST_TEMPLATE>).SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboExpMestTemplate.EditValue ?? "0").ToString()));
                        if (data != null)
                        {
                            this.txtExpMestTemplateCode.Text = data.EXP_MEST_TEMPLATE_CODE;
                            this.cboExpMestTemplate.Properties.Buttons[1].Visible = true;
                            this.ProcessChoiceExpMestTemplate(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTemplate_Medicine_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboExpMestTemplate.EditValue != null)
                    {
                        WaitingManager.Show();
                        HIS_EXP_MEST_TEMPLATE data = (cboExpMestTemplate.Properties.DataSource as List<HIS_EXP_MEST_TEMPLATE>).SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboExpMestTemplate.EditValue ?? "0").ToString()));
                        if (data != null)
                        {
                            this.txtExpMestTemplateCode.Text = data.EXP_MEST_TEMPLATE_CODE;
                            this.cboExpMestTemplate.Properties.Buttons[1].Visible = true;
                            this.ProcessChoiceExpMestTemplate(data);
                        }
                        WaitingManager.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExpMestTemplate_Leave(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.cboExpMestTemplate.Text.Trim()) && this.cboExpMestTemplate.EditValue != null)
                {
                    this.cboExpMestTemplate.Properties.Buttons[1].Visible = false;
                    this.cboExpMestTemplate.EditValue = null;
                    this.txtExpMestTemplateCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtLadder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.txtAdvise.Focus();
                    this.txtAdvise.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDuongDung__Medicine_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboMedicineUseForm.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboMedicineUseForm.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            this.cboMedicineUseForm.Properties.Buttons[1].Visible = true;
                            this.SetHuongDanFromSoLuongNgay();
                        }
                    }
                    this.FocusMedicineUseForm();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDuongDung__Medicine_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.cboMedicineUseForm.Properties.Buttons[1].Visible = false;
                    this.cboMedicineUseForm.EditValue = null;
                    this.FocusShowpopup(this.cboMedicineUseForm, false);
                }
                else if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    WaitingManager.Show();
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisMedicineUseForm").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.HisMedicineUseForm'");
                    if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.HisMedicineUseForm' is not plugins");

                    List<object> listArgs = new List<object>();
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, GetRoomId(), GetRoomTypeId()), listArgs);
                    if (extenceInstance == null) throw new NullReferenceException("Khoi tao moduleData that bai. extenceInstance = null");

                    WaitingManager.Hide();
                    ((Form)extenceInstance).ShowDialog();

                    BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM>();
                    this.InitComboMedicineUseForm(null);
                }
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDuongDung__Medicine_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (Inventec.Common.TypeConvert.Parse.ToInt64((this.cboMedicineUseForm.EditValue ?? "").ToString()) > 0)
                    {
                        this.cboMedicineUseForm.Properties.Buttons[1].Visible = true;
                        this.SetHuongDanFromSoLuongNgay();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMedicineUseForm_Leave(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.cboMedicineUseForm.Text) && this.cboMedicineUseForm.EditValue != null)
                {
                    this.cboMedicineUseForm.EditValue = null;
                    this.cboMedicineUseForm.Properties.Buttons[1].Visible = false;
                    this.SetHuongDanFromSoLuongNgay();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinUseDays_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.btnAdd.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtLoiDanBacSi_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Tab)
                {
                    if (HisConfigCFG.AutoFocusToAdvise)
                    {
                        txtMediMatyForPrescription.Focus();
                        txtMediMatyForPrescription.SelectAll();
                        e.Handled = true;
                    }
                    else
                    {
                        this.btnSave.Focus();
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinSoLuongNgay_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (lciSang.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    {
                        this.spinSang.SelectAll();
                        this.spinSang.Focus();
                    }
                    else
                    {
                        this.txtTocDoTho.SelectAll();
                        this.txtTocDoTho.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinSoLuongNgay_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.spinSoLuongNgay.Value > 0)
                {
                    if (this.spinSoNgay.Value < this.spinSoLuongNgay.Value)
                    {
                        this.spinSoNgay.Value = this.spinSoLuongNgay.Value;
                    }
                    this.CalculateAmount();
                    this.SetHuongDanFromSoLuongNgay();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinSoNgay_Leave(object sender, EventArgs e)
        {
            try
            {
                if (this.spinSoNgay.Value > 0)
                {
                    this.spinSoLuongNgay.Value = this.spinSoNgay.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinSoNgay_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.spinSoLuongNgay.Value = this.spinSoNgay.Value;
                    rdOpionGroup.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtChiaLam_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //if (!String.IsNullOrEmpty(spinTrua.Text))
                this.CalculateAmount();
                if (!IsSetByMedicineTut)
                    this.SetHuongDanFromSoLuongNgay();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinToi_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.cboHtu.Focus();
                    this.cboHtu.SelectAll();
                    ShowHtuPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinToi_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //if (!String.IsNullOrEmpty(spinToi.Text))
                this.CalculateAmount();
                if (!IsSetByMedicineTut)
                    this.SetHuongDanFromSoLuongNgay();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinToi_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = ResourceMessage.TruongSoLuongNhapSaiDinhDang;
                e.ExceptionMode = ExceptionMode.DisplayError;
            }
            catch (Exception ex)
            {       
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinToi_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                this.SpinValidating(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinToi_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                this.SpinKeyPress(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinChieu_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //if (!String.IsNullOrEmpty(spinChieu.Text))
                this.CalculateAmount();
                if (!IsSetByMedicineTut)
                    this.SetHuongDanFromSoLuongNgay();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinChieu_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.spinToi.SelectAll();
                    this.spinToi.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinChieu_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = ResourceMessage.TruongSoLuongNhapSaiDinhDang;
                e.ExceptionMode = ExceptionMode.DisplayError;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinChieu_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                this.SpinValidating(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinChieu_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                this.SpinKeyPress(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinTrua_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.spinChieu.SelectAll();
                    this.spinChieu.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinTrua_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = ResourceMessage.TruongSoLuongNhapSaiDinhDang;
                e.ExceptionMode = ExceptionMode.DisplayError;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinTrua_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                this.SpinValidating(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinTrua_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                this.SpinKeyPress(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinSang_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.spinTrua.SelectAll();
                    this.spinTrua.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinSang_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //if (!String.IsNullOrEmpty(spinSang.Text))
                this.CalculateAmount();
                if (!IsSetByMedicineTut)
                    this.SetHuongDanFromSoLuongNgay();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinSang_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                this.SpinValidating(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinSang_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = ResourceMessage.TruongSoLuongNhapSaiDinhDang;
                e.ExceptionMode = ExceptionMode.DisplayError;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinSang_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                this.SpinKeyPress(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHtu_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                //if (e.CloseMode == PopupCloseMode.Normal)
                {
                    var dataSource = gridViewHtu.DataSource as List<HtuADO>;
                    DataHtuList.ForEach(o => o.IsChecked = dataSource.Exists(p => p.IsChecked && p.ID == o.ID));
                    if (DataHtuList != null && DataHtuList.Count > 0 && DataHtuList.Exists(o => o.IsChecked))
                    {
                        cboHtu.Text = string.Join(", ", DataHtuList.Where(o => o.IsChecked).Select(o => o.HTU_NAME));
                    }
                    else
                        cboHtu.Text = null;
                    if (string.IsNullOrEmpty(cboHtu.Text))
                        this.cboHtu.Properties.Buttons[1].Visible = true;
                    this.SetHuongDanFromSoLuongNgay();

                    if (this.cboMedicineUseForm.Enabled && this.cboMedicineUseForm.EditValue == null)
                    {
                        this.FocusShowpopup(this.cboMedicineUseForm, false);
                    }
                    else
                    {
                        this.spinAmount.Focus();
                        this.spinAmount.SelectAll();
                    }
                    popupContainerHtu.HidePopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHtu_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(cboHtu.Text))
                        this.cboHtu.Properties.Buttons[1].Visible = true;
                    this.SetHuongDanFromSoLuongNgay();

                    if (this.cboMedicineUseForm.Enabled && this.cboMedicineUseForm.EditValue == null)
                    {
                        this.FocusShowpopup(this.cboMedicineUseForm, false);
                    }
                    else
                    {
                        this.spinAmount.Focus();
                        this.spinAmount.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHtu_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    DataHtuList.ForEach(o => o.IsChecked = false);
                    this.cboHtu.Text = null;
                    this.cboHtu.Properties.Buttons[1].Visible = false;
                    this.SetHuongDanFromSoLuongNgay();

                    if (this.medicineTypeTutSelected == null)
                    {
                        this.memHtu.Text = this.cboMedicineUseForm.Text + " " + this.cboHtu.Text;
                    }
                }
                else if (e.Button.Kind == ButtonPredefines.Plus)
                {
                    WaitingManager.Show();
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisHtu").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.HisHtu'");
                    if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.HisHtu' is not plugins");

                    List<object> listArgs = new List<object>();
                    var md = HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, GetRoomId(), GetRoomTypeId());
                    md.ModuleTypeId = Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM;    
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(md, listArgs);
                    if (extenceInstance == null) throw new NullReferenceException("Khoi tao moduleData that bai. extenceInstance = null");

                    WaitingManager.Hide();
                    if (extenceInstance is Form)
                    {    
                        ((Form)extenceInstance).ShowDialog();
                    }
                    else if (extenceInstance is UserControl)
                    {
                        HIS.Desktop.ModuleExt.TabControlBaseProcess.TabCreating(SessionManager.GetTabControlMain(), md.ExtensionInfo.Code, md.text, (System.Windows.Forms.UserControl)extenceInstance, md);
                    }

                    BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_HTU>();
                    this.InitComboHtu(null);

                    if (this.medicineTypeTutSelected == null)
                    {
                        this.memHtu.Text = this.cboMedicineUseForm.Text + " " + this.cboHtu.Text;
                    }
                }
                else if (e.Button.Kind == ButtonPredefines.Combo)
                {
                    ShowHtuPopup();
                }
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ShowHtuPopup()
        {
            try
            {
                if (!string.IsNullOrEmpty(cboHtu.Text))
                {
                    if (DataHtuListShow != null && DataHtuListShow.Count > 0)
                    {
                        foreach (var item in DataHtuListShow)
                        {
                            if (DataHtuList.Exists(o => o.ID == item.ID && o.IsChecked))
                                item.IsChecked = true;
                            else
                                item.IsChecked = false;
                        }
                    }
                    DataHtuListShow = DataHtuListShow.OrderByDescending(o => o.IsChecked ? 1 : 0).ToList();
                }
                else
                {
                    DataHtuListShow.ForEach(o => o.IsChecked = false);
                }

                gridControlHtu.DataSource = null;
                gridControlHtu.DataSource = DataHtuList;
                Rectangle buttonBounds = new Rectangle(cboHtu.Bounds.X, cboHtu.Bounds.Y, cboHtu.Bounds.Width, cboHtu.Bounds.Height);
                popupContainerHtu.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 245));
                gridControlHtu.Focus();
                gridViewHtu.FocusedColumn = gridColumn30;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboHtu_Leave(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.cboHtu.Text))
                {
                    this.cboHtu.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinAmount_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = ResourceMessage.TruongSoLuongNhapSaiDinhDang;
                e.ExceptionMode = ExceptionMode.DisplayError;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinAmount_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                this.SpinValidating(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                this.SpinAmountKeyPress(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinAmount_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    long chidinhnhanh = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__CHI_DINH_NHANH_THUOC_VAT_TU);
                    if (chidinhnhanh == 1)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("spinAmount_KeyDown");
                        this.btnAdd.Focus();
                        e.Handled = true;
                    }
                    else
                    {
                        this.txtTutorial.Focus();
                        this.txtTutorial.SelectionStart = this.txtTutorial.Text.Length + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMedicineTypeOther_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.txtUnitOther.Focus();
                    this.txtUnitOther.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtUnitOther_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.spinAmount.Focus();
                    this.spinAmount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMedicineTypeOther_Leave(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Warn("txtMedicineTypeOther_Leave");
                this.btnAdd.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void rdOpionGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => IsHandlerWhileOpionGroupSelectedIndexChanged), IsHandlerWhileOpionGroupSelectedIndexChanged));
                SetEnableControlMedicine();
                if (!IsHandlerWhileOpionGroupSelectedIndexChanged)
                    OpionGroupSelectedChanged();
                ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OpionGroupSelectedChanged()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("OpionGroupSelectedChanged.1");
                var selectedOpionGroup = GetSelectedOpionGroup();
                if (selectedOpionGroup == 1)//rdOpionGroup.EditorContainsFocus && 
                {
                    this.ResetComboNhaThuoc();
                    this.lciForchkShowLo.Enabled = HisConfigCFG.IsAllowAssignPresByPackage;
                    this.theRequiredWidth = 1030;
                    this.theRequiredHeight = 200;
                    this.RebuildMediMatyWithInControlContainer();
                }
                else if (selectedOpionGroup == 2)
                {
                    this.lciForchkShowLo.Enabled = false;
                    cboPatientType.EditValue = null;
                    cboPatientType.Properties.DataSource = null;
                    this.InitComboNhaThuoc();
                    this.theRequiredWidth = 1030;
                    this.theRequiredHeight = 200;
                    if (this.currentMediStockNhaThuocSelecteds == null || this.currentMediStockNhaThuocSelecteds.Count == 0)
                        if (HisConfigCFG.OutStockListItemInCaseOfNoStockChosenOption == "2") //#26617
                            RebuildNhaThuocMediMatyWithInControlContainerWithConfig();
                        else
                            RebuildMedicineTypeWithInControlContainer();
                    else
                        RebuildNhaThuocMediMatyWithInControlContainer();
                }
                else if (selectedOpionGroup == 3)
                {
                    this.ResetComboNhaThuoc();
                    this.lciForchkShowLo.Enabled = false;
                    this.theRequiredWidth = 1030;
                    this.theRequiredHeight = 200;
                    this.RebuildMediMatyWithInControlContainer(true);
                }
                else
                {
                    this.lciForchkShowLo.Enabled = false;
                    this.ResetComboNhaThuoc();
                }
                Inventec.Common.Logging.LogSystem.Debug("OpionGroupSelectedChanged.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OpionGroupSelectedChangedAsync()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("OpionGroupSelectedChangedAsync.1");
                var selectedOpionGroup = GetSelectedOpionGroup();
                if (selectedOpionGroup == 1)
                {
                    this.ResetComboNhaThuoc();
                    this.lciForchkShowLo.Enabled = HisConfigCFG.IsAllowAssignPresByPackage;
                    this.theRequiredWidth = 1030;
                    this.theRequiredHeight = 200;
                    this.RebuildMediMatyWithInControlContainerAsync();
                }
                else if (selectedOpionGroup == 2)
                {
                    this.lciForchkShowLo.Enabled = false;
                    this.cboPatientType.EditValue = null;
                    this.cboPatientType.Properties.DataSource = null;
                    this.InitComboNhaThuoc();
                    this.theRequiredWidth = 850;
                    this.theRequiredHeight = 200;
                    if (cboNhaThuoc.EditValue == null || (this.currentMediStockNhaThuocSelecteds == null || this.currentMediStockNhaThuocSelecteds.Count == 0))
                        RebuildMedicineTypeWithInControlContainer();
                    else
                        RebuildNhaThuocMediMatyWithInControlContainerAsync();
                }
                else if (selectedOpionGroup == 3)
                {
                    this.ResetComboNhaThuoc();
                    this.lciForchkShowLo.Enabled = false;
                    this.theRequiredWidth = 1030;
                    this.theRequiredHeight = 200;
                    this.RebuildMediMatyWithInControlContainerAsync(true);
                }
                else
                {
                    this.lciForchkShowLo.Enabled = false;
                    this.cboNhaThuoc.EditValue = null;
                }
                Inventec.Common.Logging.LogSystem.Debug("OpionGroupSelectedChangedAsync.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetComboNhaThuoc()
        {
            try
            {
                this.cboNhaThuoc.EditValue = null;
                this.cboNhaThuoc.Properties.Buttons[1].Visible = false;
                this.currentMediStockNhaThuocSelecteds = new List<V_HIS_MEDI_STOCK>();
                GridCheckMarksSelection gridCheckMark = cboNhaThuoc.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboNhaThuoc.Properties.View);
                    this.cboNhaThuoc.ShowPopup();
                    this.cboNhaThuoc.ClosePopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTutorial_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    memHtu.Focus();
                    if (e.KeyCode == Keys.Enter)
                        e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMediMatyForPrescription_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtMediMatyForPrescription.Text))
                {
                    txtMediMatyForPrescription.Refresh();
                    if (isShowContainerMediMatyForChoose)
                    {
                        gridViewMediMaty.ActiveFilter.Clear();
                    }
                    else
                    {
                        if (!isShowContainerMediMaty)
                        {
                            isShowContainerMediMaty = true;
                        }

                        //Filter data
                        gridViewMediMaty.ActiveFilterString = String.Format("[MEDICINE_TYPE_NAME] Like '%{0}%' OR [MEDICINE_TYPE_CODE] Like '%{0}%' OR [ACTIVE_INGR_BHYT_NAME] Like '%{0}%' OR [MEDICINE_TYPE_NAME__UNSIGN] Like '%{0}%' OR [MEDICINE_TYPE_CODE__UNSIGN] Like '%{0}%' OR [ACTIVE_INGR_BHYT_NAME__UNSIGN] Like '%{0}%' OR [PARENT_NAME] Like '%{0}%'" + (GetSelectedOpionGroup() == 3 ? " OR [SERIAL_NUMBER] Like '%{0}%'" : ""), txtMediMatyForPrescription.Text);
                        //+ " OR [CONCENTRA] Like '%" + txtMediMatyForPrescription.Text + "%'"
                        //+ " OR [MEDI_STOCK_NAME] Like '%" + txtMediMatyForPrescription.Text + "%'";
                        gridViewMediMaty.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                        gridViewMediMaty.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                        gridViewMediMaty.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                        gridViewMediMaty.FocusedRowHandle = 0;
                        gridViewMediMaty.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
                        gridViewMediMaty.OptionsFind.HighlightFindResults = true;

                        Rectangle buttonBounds = new Rectangle(txtMediMatyForPrescription.Bounds.X, txtMediMatyForPrescription.Bounds.Y, txtMediMatyForPrescription.Bounds.Width, txtMediMatyForPrescription.Bounds.Height);
                        if (isShow)
                        {
                            popupControlContainerMediMaty.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 25));
                            isShow = false;
                        }

                        this.SetDataIntoChongCHiDinhInfo();

                        txtMediMatyForPrescription.Focus();
                    }
                    isShowContainerMediMatyForChoose = false;
                }
                else
                {
                    gridViewMediMaty.ActiveFilter.Clear();
                    this.currentMedicineTypeADOForEdit = null;
                    if (!isShowContainerMediMaty)
                    {
                        popupControlContainerMediMaty.HidePopup();
                    }
                }
                this.ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMediMatyForPrescription_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.DropDown)
                {
                    isShowContainerMediMaty = !isShowContainerMediMaty;
                    if (isShowContainerMediMaty)
                    {
                        Rectangle buttonBounds = new Rectangle(txtMediMatyForPrescription.Bounds.X, txtMediMatyForPrescription.Bounds.Y, txtMediMatyForPrescription.Bounds.Width, txtMediMatyForPrescription.Bounds.Height);
                        popupControlContainerMediMaty.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 25));

                        if (this.currentMedicineTypeADOForEdit != null)
                        {
                            int rowIndex = 0;
                            var listDatas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MediMatyTypeADO>>(Newtonsoft.Json.JsonConvert.SerializeObject(this.gridControlMediMaty.DataSource));
                            for (int i = 0; i < listDatas.Count; i++)
                            {
                                if (listDatas[i].SERVICE_ID == this.currentMedicineTypeADOForEdit.SERVICE_ID && listDatas[i].MEDI_STOCK_ID == this.currentMedicineTypeADOForEdit.MEDI_STOCK_ID)
                                {
                                    rowIndex = i;
                                    break;
                                }
                            }
                            gridViewMediMaty.FocusedRowHandle = rowIndex;
                            this.SetDataIntoChongCHiDinhInfo();
                        }
                    }
                    else
                    {
                        //popupControlContainerMediMaty.HidePopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMediMatyForPrescription_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var data = txtMediMatyForPrescription.Text.Trim().ToLower();
                    var selectedOpionGroup = GetSelectedOpionGroup();
                    var medicineTypeADOForEdit = this.gridViewMediMaty.GetFocusedRow();
                    if (medicineTypeADOForEdit != null)
                    {
                        isShowContainerMediMaty = false;
                        isShowContainerMediMatyForChoose = true;
                        popupControlContainerMediMaty.HidePopup();
                        if (selectedOpionGroup == 1)
                        {
                            MetyMatyTypeInStock_RowClick(medicineTypeADOForEdit);
                        }
                        else if (selectedOpionGroup == 2)
                        {
                            MedicineType_RowClick(medicineTypeADOForEdit);
                        }
                        else
                        {
                            VisibleInputControl(true);
                            MaterialTypeTSD_RowClick(medicineTypeADOForEdit);
                        }
                        this.SetDataIntoChongCHiDinhInfo(medicineTypeADOForEdit);
                        if (selectedOpionGroup == 3)
                        {
                            if (gridControlMediMaty.DataSource is List<DMediStock1ADO> && (dMediStock1s != null && dMediStock1s.FirstOrDefault(o => o.ID == ((DMediStock1ADO)medicineTypeADOForEdit).ID) != null || mediStockD1ADOs != null && mediStockD1ADOs.FirstOrDefault(o => o.ID == ((DMediStock1ADO)medicineTypeADOForEdit).ID) != null))
                            {
                                if (dMediStock1s.Where(o => o.MEDICINE_TYPE_NAME.ToLower().Contains(data) || o.MEDICINE_TYPE_CODE.ToLower().Contains(data) || (o.ACTIVE_INGR_BHYT_NAME ?? "").ToLower().Contains((data ?? "")) || (o.MEDICINE_TYPE_NAME__UNSIGN ?? "").ToLower().Contains((data ?? "")) || (o.MEDICINE_TYPE_CODE__UNSIGN ?? "").ToLower().Contains((data ?? "")) || (o.ACTIVE_INGR_BHYT_NAME__UNSIGN ?? "").ToLower().Contains((data ?? "")) || (o.PARENT_NAME ?? "").ToLower().Contains((data ?? "")) || (o.SERIAL_NUMBER ?? "").ToLower().Contains((data ?? ""))).Count() == 1 ||
                                    mediStockD1ADOs.Where(o => o.MEDICINE_TYPE_NAME.ToLower().Contains(data) || o.MEDICINE_TYPE_CODE.ToLower().Contains(data) || (o.ACTIVE_INGR_BHYT_NAME ?? "").ToLower().Contains((data ?? "")) || (o.MEDICINE_TYPE_NAME__UNSIGN ?? "").ToLower().Contains((data ?? "")) || (o.MEDICINE_TYPE_CODE__UNSIGN ?? "").ToLower().Contains((data ?? "")) || (o.ACTIVE_INGR_BHYT_NAME__UNSIGN ?? "").ToLower().Contains((data ?? "")) || (o.PARENT_NAME ?? "").ToLower().Contains((data ?? "")) || (o.SERIAL_NUMBER ?? "").ToLower().Contains((data ?? ""))).Count() == 1)
                                    btnAdd.PerformClick();
                            }
                        }
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    int rowHandlerNext = 0;
                    int countInGridRows = gridViewMediMaty.RowCount;
                    if (countInGridRows > 1)
                    {
                        rowHandlerNext = 1;
                    }

                    Rectangle buttonBounds = new Rectangle(txtMediMatyForPrescription.Bounds.X, txtMediMatyForPrescription.Bounds.Y, txtMediMatyForPrescription.Bounds.Width, txtMediMatyForPrescription.Bounds.Height);
                    popupControlContainerMediMaty.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 25));
                    gridViewMediMaty.Focus();
                    gridViewMediMaty.FocusedRowHandle = rowHandlerNext;
                    System.Threading.Thread.Sleep(100);
                    this.SetDataIntoChongCHiDinhInfo();
                }
                else if (e.Control && e.KeyCode == Keys.A)
                {
                    txtMediMatyForPrescription.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataIntoChongCHiDinhInfo(object medicineTypeADOChoice = null)
        {
            Inventec.Common.Logging.LogSystem.Debug("SetDataIntoChongCHiDinhInfo____gridViewMediMaty.RowCount:" + gridViewMediMaty.RowCount);

            var medicineTypeADOForEdit = medicineTypeADOChoice != null ? medicineTypeADOChoice : this.gridViewMediMaty.GetFocusedRow();
            var currentMedicineTypeTmp = new MediMatyTypeADO();
            Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(currentMedicineTypeTmp, medicineTypeADOForEdit);
            Inventec.Common.Logging.LogSystem.Debug("SetDataIntoChongCHiDinhInfo____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentMedicineTypeTmp), currentMedicineTypeTmp));
            if (currentMedicineTypeTmp != null && (!String.IsNullOrEmpty(currentMedicineTypeTmp.CONTRAINDICATION) || !String.IsNullOrEmpty(currentMedicineTypeTmp.DESCRIPTION)))
            {
                if (!navBarControlChongChiDinhInfo.OptionsNavPane.IsAnimationInProgress && navBarControlChongChiDinhInfo.OptionsNavPane.NavPaneState == DevExpress.XtraNavBar.NavPaneState.Collapsed)
                {
                    navBarControlChongChiDinhInfo.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Expanded;
                }

                this.FillDataIntoDescriptionInfo(currentMedicineTypeTmp);
            }
            else
            {
                this.txtThongTinChongChiDinhThuoc.Text = "";
            }
        }

        private void FillDataIntoDescriptionInfo(MediMatyTypeADO currentMedicineTypeTmp)
        {
            try
            {
                string txtDesInfo = "";
                this.txtThongTinChongChiDinhThuoc.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
                Inventec.Common.Logging.LogSystem.Debug("FillDataIntoDescriptionInfo____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentMedicineTypeTmp.CONTRAINDICATION), currentMedicineTypeTmp.CONTRAINDICATION)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentMedicineTypeTmp.DESCRIPTION), currentMedicineTypeTmp.DESCRIPTION));
                if (!String.IsNullOrEmpty(currentMedicineTypeTmp.CONTRAINDICATION))
                {
                    if (!String.IsNullOrEmpty(currentMedicineTypeTmp.DESCRIPTION))
                    {
                        txtDesInfo = "<b><color=red>Chống chỉ định:</color></b><br>";
                    }
                    txtDesInfo += "<b>" + currentMedicineTypeTmp.CONTRAINDICATION + "</b>";
                }
                if (!String.IsNullOrEmpty(currentMedicineTypeTmp.DESCRIPTION))
                {
                    if (!String.IsNullOrEmpty(txtDesInfo))
                    {
                        txtDesInfo += "<br>";
                        txtDesInfo += "<br><b><color=red>Ghi chú:</color></b><br>";
                    }
                    txtDesInfo += "<b>" + currentMedicineTypeTmp.DESCRIPTION + "</b>";
                }
                this.txtThongTinChongChiDinhThuoc.Text = txtDesInfo;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void VisibleInputControl(bool is4Control)
        {
            try
            {
                lciSang.Visibility = lciTrua.Visibility = lciChieu.Visibility = lciToi.Visibility = is4Control ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciFortxtThoiGianTho.Visibility = lciFortxtTocDoTho.Visibility = is4Control ? DevExpress.XtraLayout.Utils.LayoutVisibility.Never : DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPreKidneyShift_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                spinKidneyCount.Enabled = chkPreKidneyShift.Checked;
                if (!chkPreKidneyShift.Checked)
                {
                    spinKidneyCount.EditValue = null;
                    lciForspinKidneyCount.AppearanceItemCaption.ForeColor = Color.Black;
                }
                else
                {
                    lciForspinKidneyCount.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Grid control
        private void gridControlServiceProcess_DoubleClick(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                this.cboHtu.Text = "";
                this.currentMedicineTypeADOForEdit = (MediMatyTypeADO)this.gridViewServiceProcess.GetFocusedRow();
                if (this.currentMedicineTypeADOForEdit != null)
                {
                    var selectedOpionGroup = GetSelectedOpionGroup();      

                    this.actionBosung = GlobalVariables.ActionEdit;
                    isShowContainerMediMatyForChoose = true;
                    if (this.currentMedicineTypeADOForEdit.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                        || this.currentMedicineTypeADOForEdit.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
                    {
                        this.SetControlSoLuongNgayNhapChanLe(this.currentMedicineTypeADOForEdit);
                        if (selectedOpionGroup != 1)
                            rdOpionGroup.SelectedIndex = 0;
                    }
                    else if (this.currentMedicineTypeADOForEdit.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD)
                    {
                        MessageManager.Show(ResourceMessage.VatTuTSDKhongChoPhepSua);
                        return;
                    }
                    else
                    {
                        if (rdOpionGroup.Properties.Items.Count > 1 && rdOpionGroup.Properties.Items[1].Enabled)
                        {
                            if (selectedOpionGroup != 2)
                                rdOpionGroup.SelectedIndex = 1;
                        }
                        else if (selectedOpionGroup != 1)
                            rdOpionGroup.SelectedIndex = 0;
                    }
                    this.currentMedicineTypeADOForEdit = (MediMatyTypeADO)this.gridViewServiceProcess.GetFocusedRow();

                    if (selectedOpionGroup == 1 && (this.serviceReqMain != null && this.serviceReqMain.IS_MAIN_EXAM != 1) && (HisConfigCFG.IsUsingSubPrescriptionMechanism == "1") && !GlobalStore.IsCabinet)
                    {
                        this.currentMedicineTypeADOForEdit.IS_SUB_PRES = 1;

                        if (this.oldServiceReq != null && this.serviceReqMain != null && this.serviceReqMain.IS_SUB_PRES != 1)
                        {
                            this.currentMedicineTypeADOForEdit.IS_SUB_PRES = null;
                        }
                    }

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentMedicineTypeADOForEdit), currentMedicineTypeADOForEdit));
                    if (this.currentMedicineTypeADOForEdit.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC)
                    {
                        this.txtMediMatyForPrescription.Text = "";
                        this.txtUnitOther.Text = this.currentMedicineTypeADOForEdit.SERVICE_UNIT_NAME;
                        this.txtMedicineTypeOther.Text = currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME;
                        if ((currentMedicineTypeADOForEdit.AMOUNT ?? 0) > 0)
                            this.spinPrice.Value = currentMedicineTypeADOForEdit.TotalPrice / currentMedicineTypeADOForEdit.AMOUNT.Value;
                    }
                    else
                    {
                        this.txtMediMatyForPrescription.Text = this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME;
                    }

                    this.VisibleInputControl(!(currentMedicineTypeADOForEdit.IS_OXYGEN == GlobalVariables.CommonNumberTrue));

                    this.lciTocDoTruyen.Enabled = (this.currentMedicineTypeADOForEdit.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC
                        || this.currentMedicineTypeADOForEdit.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                        || this.currentMedicineTypeADOForEdit.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM) ? true : false;

                    this.cboMedicineUseForm.EditValue = this.currentMedicineTypeADOForEdit.MEDICINE_USE_FORM_ID;
                    if (currentMedicineTypeADOForEdit.HTU_IDs != null && currentMedicineTypeADOForEdit.HTU_IDs.Count > 0)
                    {
                        if (DataHtuList != null && DataHtuList.Count > 0)
                        {
                            DataHtuList.ForEach(o =>
                            {
                                o.IsChecked = currentMedicineTypeADOForEdit.HTU_IDs.Exists(p => p == o.ID);
                            });
                            if (string.Join(", ", DataHtuList.Where(o => o.IsChecked).Select(o => o.HTU_NAME)) != null)
                            {
                                this.cboHtu.Text = string.Join(", ", DataHtuList.Where(o => o.IsChecked).Select(o => o.HTU_NAME));
                            }
                            else
                            {
                                this.cboHtu.Text = "";
                            }
                            this.cboHtu.Properties.Buttons[1].Visible = true;
                        }
                    }
                    else
                        this.cboHtu.Properties.Buttons[1].Visible = false;
                    this.InstructionTime = intructionTimeSelecteds.OrderByDescending(o => o).First();
                    if (this.currentMedicineTypeADOForEdit != null && currentMedicineTypeADOForEdit.UseTimeTo.HasValue && (InstructionTime > 0 || UseTime > 0) && !this.currentMedicineTypeADOForEdit.UseDays.HasValue)
                    {
                        System.DateTime dtUseTimeTo = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentMedicineTypeADOForEdit.UseTimeTo ?? 0).Value;
                        System.DateTime dtInstructionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.UseTime > 0 ? this.UseTime : this.InstructionTime).Value;
                        TimeSpan diff__Day = (dtUseTimeTo - dtInstructionTime);
                        this.currentMedicineTypeADOForEdit.UseDays = diff__Day.Days + 1;
                    }
                    this.spinSoLuongNgay.EditValue = this.currentMedicineTypeADOForEdit.UseDays;
                    this.memHtu.Text = this.currentMedicineTypeADOForEdit.HTU_TEXT;
                    if (!navBarControlChongChiDinhInfo.OptionsNavPane.IsAnimationInProgress && navBarControlChongChiDinhInfo.OptionsNavPane.NavPaneState == DevExpress.XtraNavBar.NavPaneState.Collapsed)
                    {
                        navBarControlChongChiDinhInfo.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Expanded;
                        //navBarControlChongChiDinhInfo.Update();
                    }

                    this.FillDataIntoDescriptionInfo(this.currentMedicineTypeADOForEdit);

                    if (this.currentMedicineTypeADOForEdit.PREVIOUS_USING_COUNT.HasValue && this.currentMedicineTypeADOForEdit.PREVIOUS_USING_COUNT > 0)
                        this.txtPreviousUseDay.Text = this.currentMedicineTypeADOForEdit.PREVIOUS_USING_COUNT + "";
                    else
                        this.txtPreviousUseDay.Text = "";

                    if (!String.IsNullOrEmpty(this.currentMedicineTypeADOForEdit.BREATH_SPEED))
                        this.txtTocDoTho.EditValue = this.currentMedicineTypeADOForEdit.BREATH_SPEED;
                    else
                        this.txtTocDoTho.EditValue = null;
                    if (!String.IsNullOrEmpty(this.currentMedicineTypeADOForEdit.BREATH_TIME))
                        this.txtThoiGianTho.EditValue = this.currentMedicineTypeADOForEdit.BREATH_TIME;
                    else
                        this.txtThoiGianTho.EditValue = null;

                    if (!String.IsNullOrEmpty(this.currentMedicineTypeADOForEdit.Sang))
                        this.spinSang.EditValue = this.currentMedicineTypeADOForEdit.Sang;
                    else
                        this.spinSang.EditValue = null;
                    if (!String.IsNullOrEmpty(this.currentMedicineTypeADOForEdit.Trua))
                        this.spinTrua.EditValue = this.currentMedicineTypeADOForEdit.Trua;
                    else
                        this.spinTrua.EditValue = null;
                    if (!String.IsNullOrEmpty(this.currentMedicineTypeADOForEdit.Chieu))
                        this.spinChieu.EditValue = this.currentMedicineTypeADOForEdit.Chieu;
                    else
                        this.spinChieu.EditValue = null;
                    if (!String.IsNullOrEmpty(this.currentMedicineTypeADOForEdit.Toi))
                        this.spinToi.EditValue = this.currentMedicineTypeADOForEdit.Toi;
                    else
                        this.spinToi.EditValue = null;
                    //Tự động hiển thi số lượng là phân số nếu AMOUNT là số thập phân
                    //Vd: AMOUNT = 0.25 --> spinAmount.Text = 1/4
                    //Ngược lại nếu là số nguyên thì hiển thị giữ nguyên giá trị     
                    this.spinAmount.EditValue = ConvertNumber.ConvertDecToFracByConfig((double)(this.currentMedicineTypeADOForEdit.AMOUNT ?? 0));
                    this.txtTutorial.Text = this.currentMedicineTypeADOForEdit.TUTORIAL;
                    this.memHtu.Text = this.currentMedicineTypeADOForEdit.HTU_TEXT;
                    this.spinTocDoTruyen.EditValue = this.currentMedicineTypeADOForEdit.Speed;
                    this.btnAdd.Enabled = true;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidProviderBoXung, this.dxErrorProvider1);
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidProviderBoXung__DuongDung, this.dxErrorProvider1);
                    this.VisibleButton(this.actionBosung);
                    this.spinAmount.Focus();
                    this.spinAmount.SelectAll();

                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    var medicineTypeTuts = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE_TUT>();
                    List<HIS_MEDICINE_TYPE_TUT> medicineTypeTutFilters = medicineTypeTuts.OrderByDescending(o => o.MODIFY_TIME).Where(o => o.MEDICINE_TYPE_ID == currentMedicineTypeADOForEdit.ID && o.LOGINNAME == loginName).ToList();
                    this.RebuildTutorialWithInControlContainer(medicineTypeTutFilters);
                    this.RebuildHtuWithInControlContainer(medicineTypeTutFilters);

                    if (HisConfigCFG.ManyDayPrescriptionOption == 2
                        //&& this.ucDateForMediProcessor != null
                        //&& this.ucDateForMedi != null
                        && this.currentMedicineTypeADOForEdit.IntructionTimeSelecteds != null
                        && this.currentMedicineTypeADOForEdit.IntructionTimeSelecteds.Count > 0)
                    {
                        if (this.isMultiDateState != this.currentMedicineTypeADOForEdit.IsMultiDateState)
                        {
                            // HIS.UC.DateEditor.ADO.DateInputOnlyCheckADO dateInputOnlyCheckADO = new HIS.UC.DateEditor.ADO.DateInputOnlyCheckADO();
                            //dateInputOnlyCheckADO.IsChecked = this.currentMedicineTypeADOForEdit.IsMultiDateState;

                            chkMultiIntructionTime.Checked = this.currentMedicineTypeADOForEdit.IsMultiDateState;

                            //this.ucDateProcessor.SetValue(this.ucDate, dateInputOnlyCheckADO);
                            this.isMultiDateState = chkMultiIntructionTime.Checked;
                        }

                        this.lciForpnlUCDateForMedi.Visibility = this.isMultiDateState ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        this.emptySpaceItem4.Visibility = this.isMultiDateState ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                        if (this.isMultiDateState)
                        {
                            UC.DateEditor.ADO.DateInputHasCheckADO dateInputForMediADO = new UC.DateEditor.ADO.DateInputHasCheckADO();
                            dateInputForMediADO.Dates = new List<DateTime?>();
                            dateInputForMediADO.IsMultiDayChecked = this.currentMedicineTypeADOForEdit.IsMultiDateState;
                            dateInputForMediADO.IsVisibleMultiDate = true;

                            foreach (var itemDate in this.currentMedicineTypeADOForEdit.IntructionTimeSelecteds)
                            {
                                DateTime? itemDateCV = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(itemDate);
                                if (itemDateCV != null && itemDateCV.Value != DateTime.MinValue)
                                {
                                    dateInputForMediADO.Dates.Add(itemDateCV);
                                    dateInputForMediADO.Time = itemDateCV.Value;
                                }
                            }
                            UcDateSetValueHasCheck(dateInputForMediADO);

                            //this.ucDateForMediProcessor.SetValue(this.ucDateForMedi, dateInputForMediADO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlServiceProcess_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Tab || e.KeyCode == Keys.Enter)
                {
                    DevExpress.XtraGrid.GridControl grid = sender as DevExpress.XtraGrid.GridControl;
                    DevExpress.XtraGrid.Views.Grid.GridView view = grid.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    if ((e.Modifiers == Keys.None && view.IsLastRow && view.FocusedColumn.VisibleIndex == view.VisibleColumns.Count - 1) || (e.Modifiers == Keys.Shift && view.IsFirstRow && (view.FocusedColumn.VisibleIndex == 0)))
                    {
                        if (view.IsEditing)
                            view.CloseEditor();
                        this.btnSave.Focus();
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("gridViewServiceProcess_CellValueChanged.1");
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                var mediMatyTypeADO = (MediMatyTypeADO)this.gridViewServiceProcess.GetFocusedRow();
                if (mediMatyTypeADO != null)
                {
                    //Nếu Cột HP không cho phép chỉnh sửa, khi nhấn vào cột sẽ không load lại các dữ liệu của dòng.
                    if (e.Column.FieldName == "IsExpend" && mediMatyTypeADO.IsDisableExpend && gridViewServiceProcess.FocusedColumn == grcExpend__TabMedicine)
                    {
                        return;
                    }
                    this.NoEdit = false;
                    mediMatyTypeADO.IsEdit = false;

                    if ((mediMatyTypeADO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC) && mediMatyTypeADO.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT && (mediMatyTypeADO.MEDICINE_USE_FORM_ID ?? 0) <= 0)
                    {
                        mediMatyTypeADO.ErrorMessageMedicineUseForm = ResourceMessage.BenhNhanDoiTuongTTBhytBatBuocPhaiNhapDuongDung;
                        mediMatyTypeADO.ErrorTypeMedicineUseForm = ErrorType.Warning;
                    }
                    else
                    {
                        mediMatyTypeADO.ErrorMessageMedicineUseForm = "";
                        mediMatyTypeADO.ErrorTypeMedicineUseForm = ErrorType.None;
                    }

                    if (mediMatyTypeADO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC && String.IsNullOrEmpty(mediMatyTypeADO.TUTORIAL.Trim()) && !HisConfigCFG.IsNotAutoGenerateTutorial)
                    {
                        mediMatyTypeADO.ErrorMessageTutorial = ResourceMessage.DoiTuongBHYTBatBuocPhaiNhapHDSD;
                        mediMatyTypeADO.ErrorTypeTutorial = ErrorType.Warning;
                    }
                    else
                    {
                        mediMatyTypeADO.ErrorMessageTutorial = "";
                        mediMatyTypeADO.ErrorTypeTutorial = ErrorType.None;
                    }

                    if (mediMatyTypeADO.PATIENT_TYPE_ID <= 0)
                    {
                        mediMatyTypeADO.ErrorMessagePatientTypeId = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                        mediMatyTypeADO.ErrorTypePatientTypeId = ErrorType.Warning;
                    }
                    else
                    {
                        mediMatyTypeADO.ErrorMessagePatientTypeId = "";
                        mediMatyTypeADO.ErrorTypePatientTypeId = ErrorType.None;
                    }
                    if ((mediMatyTypeADO.AMOUNT <= 0 || mediMatyTypeADO.AMOUNT == null) && (currentMediStockNhaThuocSelecteds == null || currentMediStockNhaThuocSelecteds.Count == 0 || (currentMediStockNhaThuocSelecteds != null && currentMediStockNhaThuocSelecteds.Count > 0 && mediMatyTypeADO.IS_OUT_HOSPITAL != GlobalVariables.CommonNumberTrue)))
                    {
                        mediMatyTypeADO.ErrorMessageAmount = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc);
                        mediMatyTypeADO.ErrorTypeAmount = ErrorType.Warning;
                    }
                    else
                    {
                        mediMatyTypeADO.ErrorMessageAmount = "";
                        mediMatyTypeADO.ErrorTypeAmount = ErrorType.None;
                    }
                    if (!String.IsNullOrEmpty(mediMatyTypeADO.ODD_WARNING_CONTENT) && (mediMatyTypeADO.AMOUNT != (int)mediMatyTypeADO.AMOUNT))
                    {
                        if (String.IsNullOrWhiteSpace(mediMatyTypeADO.ODD_PRES_REASON))
                        {
                            mediMatyTypeADO.ErrorMessageOddPres = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc);
                            mediMatyTypeADO.ErrorTypeOddPres = ErrorType.Warning;
                        }
                        else if (Encoding.UTF8.GetByteCount(mediMatyTypeADO.ODD_PRES_REASON.Trim()) > 2000)
                        {
                            mediMatyTypeADO.ErrorMessageOddPres = ResourceMessage.TruongDuLieuVuotQuaKyTuChoPhep;
                            mediMatyTypeADO.ErrorTypeOddPres = ErrorType.Warning;
                        }
                        else
                        {
                            mediMatyTypeADO.ErrorMessageOddPres = "";
                            mediMatyTypeADO.ErrorTypeOddPres = ErrorType.None;
                        }
                    }
                    else
                    {
                        mediMatyTypeADO.ErrorMessageOddPres = "";
                        mediMatyTypeADO.ErrorTypeOddPres = ErrorType.None;
                    }

                    //Trường hợp khi lấy dữ liệu khi Lưu thì sẽ không kiểm tra lại Validate 2 ô này.
                    if (!((intructionTimeSelecteds != null && intructionTimeSelected.Count > 1) || (mediMatyTypeADOs != null && mediMatyTypeADOs.Count > 0 && (mediMatyTypeADOs.Select(o => o.IntructionTime).Distinct().ToList().Count > 1 || mediMatyTypeADOs.Select(o => o.IntructionTime).Distinct().First() != InstructionTime)) || IsMultilPatient || (this.oldServiceReq != null && this.oldServiceReq.ID > 0)))
                    {
                        if (e.Column.FieldName == "OVER_RESULT_TEST_REASON")
                        {
                            if (mediMatyTypeADO.IsEditOverResultTestReason && string.IsNullOrEmpty(mediMatyTypeADO.OVER_RESULT_TEST_REASON.Trim()))
                            {
                                if (String.IsNullOrWhiteSpace(mediMatyTypeADO.OVER_RESULT_TEST_REASON.Trim()))
                                {
                                    mediMatyTypeADO.ErrorMessageOverResultTestReason = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc);
                                    mediMatyTypeADO.ErrorTypeOverResultTestReason = ErrorType.Warning;
                                }
                                else if (Encoding.UTF8.GetByteCount(mediMatyTypeADO.OVER_RESULT_TEST_REASON.Trim()) > 2000)
                                {
                                    mediMatyTypeADO.ErrorMessageOverResultTestReason = ResourceMessage.TruongDuLieuVuotQuaKyTuChoPhep;
                                    mediMatyTypeADO.ErrorTypeOverResultTestReason = ErrorType.Warning;
                                }
                                else
                                {
                                    mediMatyTypeADO.ErrorMessageOverResultTestReason = "";
                                    mediMatyTypeADO.ErrorTypeOverResultTestReason = ErrorType.None;
                                }
                            }
                            else
                            {
                                mediMatyTypeADO.ErrorMessageOverResultTestReason = "";
                                mediMatyTypeADO.ErrorTypeOverResultTestReason = ErrorType.None;
                            }
                        }

                        if (e.Column.FieldName == "OVER_KIDNEY_REASON")
                        {
                            if (mediMatyTypeADO.IsEditOverKidneyReason && string.IsNullOrEmpty(mediMatyTypeADO.OVER_KIDNEY_REASON.Trim()))
                            {
                                if (String.IsNullOrWhiteSpace(mediMatyTypeADO.OVER_KIDNEY_REASON.Trim()))
                                {
                                    mediMatyTypeADO.ErrorMessageOverKidneyReason = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc);
                                    mediMatyTypeADO.ErrorTypeOverKidneyReason = ErrorType.Warning;
                                }
                                else if (Encoding.UTF8.GetByteCount(mediMatyTypeADO.OVER_KIDNEY_REASON.Trim()) > 2000)
                                {
                                    mediMatyTypeADO.ErrorMessageOverKidneyReason = ResourceMessage.TruongDuLieuVuotQuaKyTuChoPhep;
                                    mediMatyTypeADO.ErrorTypeOverKidneyReason = ErrorType.Warning;
                                }
                                else
                                {
                                    mediMatyTypeADO.ErrorMessageOverKidneyReason = "";
                                    mediMatyTypeADO.ErrorTypeOverKidneyReason = ErrorType.None;
                                }
                            }
                            else
                            {
                                mediMatyTypeADO.ErrorMessageOverKidneyReason = "";
                                mediMatyTypeADO.ErrorTypeOverKidneyReason = ErrorType.None;
                            }
                        }
                    }
                    this.ValidRowChange(mediMatyTypeADO);
                    //mediMatyTypeADO.UpdateAutoRoundUpByConvertUnitRatioInDataRow(mediMatyTypeADO);//TODO
                    if (e.RowHandle >= 0)
                    {
                        if (e.Column.FieldName == "AMOUNT" || e.Column.FieldName == "PATIENT_TYPE_ID" || (e.Column.FieldName == "PRES_AMOUNT" && !GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet && HisConfigCFG.IsShowPresAmount && VHistreatment != null && VHistreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM))
                        {
                            if (e.Column.FieldName == "PRES_AMOUNT" && !GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet && HisConfigCFG.IsShowPresAmount && VHistreatment != null && VHistreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                            {
                                if (mediMatyTypeADO.CONVERT_RATIO.HasValue && mediMatyTypeADO.CONVERT_RATIO.Value > 1)
                                    mediMatyTypeADO.AMOUNT = (Math.Ceiling((mediMatyTypeADO.PRES_AMOUNT ?? 0) / mediMatyTypeADO.CONVERT_RATIO.Value)) * (mediMatyTypeADO.CONVERT_RATIO.Value);
                                else
                                    mediMatyTypeADO.AMOUNT = Math.Ceiling(mediMatyTypeADO.PRES_AMOUNT ?? 0);
                            }
                            if (e.Column.FieldName == "AMOUNT")
                            {
                                mediMatyTypeADO.PRES_AMOUNT = mediMatyTypeADO.AMOUNT;
                            }

                            bool rsMaxPresInBatch = true;
                            if (e.Column.FieldName == "AMOUNT")
                            {
                                IsCellChangeAmount = true;
                                bool rsMaxPres = CheckMaxInPrescription(mediMatyTypeADO, mediMatyTypeADO.AMOUNT);
                                bool rsMaxPresInDay = false;
                                bool rsCheckOddPres = false;
                                if (rsMaxPres)
                                {
                                    mediMatyTypeADO.EXCEED_LIMIT_IN_PRES_REASON = this.reasonMaxPrescription;
                                    rsMaxPresInDay = CheckMaxInPrescriptionInDay(mediMatyTypeADO, mediMatyTypeADO.UseDays != 0 ? mediMatyTypeADO.AMOUNT / mediMatyTypeADO.UseDays : mediMatyTypeADO.AMOUNT);
                                    if (rsMaxPresInDay)
                                    {
                                        mediMatyTypeADO.EXCEED_LIMIT_IN_DAY_REASON = this.reasonMaxPrescriptionDay;
                                        rsCheckOddPres = CheckOddPrescription(mediMatyTypeADO, mediMatyTypeADO.AMOUNT ?? 0);
                                        if (rsCheckOddPres)
                                        {
                                            mediMatyTypeADO.ODD_PRES_REASON = this.reasonOddPrescription;
                                            mediMatyTypeADO.ErrorMessageOddPres = "";
                                            mediMatyTypeADO.ErrorTypeOddPres = ErrorType.None;
                                        }
                                    }
                                }

                                if (mediMatyTypeADO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                                {
                                    if (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                                    {
                                        rsMaxPresInBatch = CheckMaxInPrescriptionInBatchWithMultilPatient(mediMatyTypeADO, mediMatyTypeADO.UseDays != 0 ? mediMatyTypeADO.AMOUNT / mediMatyTypeADO.UseDays : mediMatyTypeADO.AMOUNT);

                                    }
                                    else
                                    {
                                        rsMaxPresInBatch = CheckMaxInPrescriptionInBatch(mediMatyTypeADO, mediMatyTypeADO.UseDays != 0 ? mediMatyTypeADO.AMOUNT / mediMatyTypeADO.UseDays : mediMatyTypeADO.AMOUNT);
                                    }
                                    if (rsMaxPresInBatch)
                                    {
                                        var ItemRemove = mediMatyTypeADOsAlertInTreatment.LastOrDefault(p => p.PrimaryKey == mediMatyTypeADO.PrimaryKey && string.IsNullOrEmpty(p.EXCEED_LIMIT_IN_BATCH_REASON));
                                        if (ItemRemove != null)
                                        {
                                            RemoveItem(mediMatyTypeADO);
                                            return;
                                        }
                                        else
                                        {
                                            var alert = this.mediMatyTypeADOsAlertInTreatment.LastOrDefault(o => o.PrimaryKey == mediMatyTypeADO.PrimaryKey);
                                            var amountPres = (mediMatyTypeADO.NUMBER_PRESCIPTION_IN_TREATMENT ?? 0) + mediMatyTypeADO.AMOUNT + mediMatyTypeADOs.Where(o => o.ID == mediMatyTypeADO.ID && o.PrimaryKey != mediMatyTypeADO.PrimaryKey).Sum(o => o.UseDays != 0 ? o.AMOUNT / o.UseDays : o.AMOUNT);
                                            if (alert != null)
                                            {
                                                if ((amountPres > mediMatyTypeADO.ALERT_MAX_IN_TREATMENT && string.IsNullOrEmpty(alert.EXCEED_LIMIT_IN_BATCH_REASON)) || amountPres <= mediMatyTypeADO.ALERT_MAX_IN_TREATMENT)
                                                {
                                                    mediMatyTypeADOsAlertInTreatment.Remove(alert);
                                                    mediMatyTypeADO.IsAlertInTreatPresciption = false;
                                                    mediMatyTypeADO.EXCEED_LIMIT_IN_BATCH_REASON = null;
                                                }
                                                else
                                                {
                                                    mediMatyTypeADO.IsAlertInTreatPresciption = true;
                                                    mediMatyTypeADO.EXCEED_LIMIT_IN_BATCH_REASON = alert.EXCEED_LIMIT_IN_BATCH_REASON;
                                                }
                                            }
                                            else
                                            {
                                                mediMatyTypeADO.IsAlertInTreatPresciption = false;
                                                mediMatyTypeADO.EXCEED_LIMIT_IN_BATCH_REASON = null;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        RemoveItem(mediMatyTypeADO);
                                        return;
                                    }
                                }
                                decimal oldAmount = Inventec.Common.TypeConvert.Parse.ToDecimal(view.ActiveEditor.OldEditValue.ToString());
                                if (!rsMaxPres || !rsMaxPresInDay || !rsCheckOddPres || !rsMaxPresInBatch || !WarningOddConvertWorker.CheckWarningOddConvertAmount(mediMatyTypeADO, mediMatyTypeADO.AMOUNT)
                                || !WarningOddConvertWorker.CheckWarningOddConvertAmount(mediMatyTypeADO, (mediMatyTypeADO.AMOUNT ?? 0), null))
                                {
                                    SetOldAmountMediMaty(oldAmount, mediMatyTypeADO.ID, mediMatyTypeADO.PrimaryKey);
                                }
                                if (intructionTimeSelecteds != null && intructionTimeSelected.Count < 2 && !IsSelectMultiPatient())
                                {
                                    if (!GetOverReason(mediMatyTypeADO, true))
                                    {
                                        mediMatyTypeADO.AMOUNT = oldAmount;
                                    }
                                }
                            }

                            if (e.Column.FieldName == "PATIENT_TYPE_ID")
                            {
                                mediMatyTypeADO.SERVICE_CONDITION_ID = null;
                                mediMatyTypeADO.SERVICE_CONDITION_NAME = "";
                                CheckConditionService(mediMatyTypeADO);
                                this.UpdateExpMestReasonInDataRow(mediMatyTypeADO);
                            }

                            if ((mediMatyTypeADO.AmountAlert ?? 0) <= 0
                        && (mediMatyTypeADO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                            || mediMatyTypeADO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU))
                            {
                                List<MediMatyTypeADO> mediMatyTypeADOTemps = new List<MediMatyTypeADO>();
                                if (mediMatyTypeADO != null && mediMatyTypeADO.IsStent == true
                                    && mediMatyTypeADO.AMOUNT > 1)
                                {
                                    mediMatyTypeADOTemps = MediMatyProcessor.MakeMaterialSingleStent(mediMatyTypeADO);
                                }
                                else
                                {
                                    mediMatyTypeADOTemps.Add(mediMatyTypeADO);
                                }

                                bool success = true;
                                CommonParam param = new CommonParam();
                                foreach (var item in mediMatyTypeADOTemps)
                                {
                                    if (!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet)
                                    {
                                        var lstOutPatientPresTmp = lstOutPatientPres.Where(o => o.PrimaryKey != item.PrimaryKey).ToList();
                                        if (mediMatyTypeADOTemps.IndexOf(item) != 0)
                                        {
                                            var objTemp = new
                                            {
                                                MedicineBean1Result = item.MedicineBean1Result,
                                                MaterialBean1Result = item.MaterialBean1Result,
                                                ExpMestDetailIds = item.ExpMestDetailIds
                                            };
                                            item.MedicineBean1Result = null;
                                            item.MaterialBean1Result = null;
                                            item.ExpMestDetailIds = null;
                                            if (!TakeOrReleaseBeanWorker.TakeForCreateBean(this.intructionTimeSelecteds, this.oldExpMestId, item, true, param, this.UseTimeSelecteds, lstOutPatientPresTmp, true))
                                            {
                                                item.IsNotTakeBean = true;
                                                item.MedicineBean1Result = objTemp.MedicineBean1Result;
                                                item.MaterialBean1Result = objTemp.MaterialBean1Result;
                                                item.ExpMestDetailIds = objTemp.ExpMestDetailIds;
                                                MessageManager.Show(this, param, false);
                                                success = false;
                                                break;
                                            }
                                            else
                                            {
                                                lstOutPatientPres = lstOutPatientPresTmp;
                                            }
                                        }
                                        else
                                        {
                                            decimal amount = (item.AMOUNT ?? 0);
                                            if (!TakeOrReleaseBeanWorker.TakeForUpdateBean(this.intructionTimeSelecteds, this.oldExpMestId, item, amount, true, param, this.UseTimeSelecteds, lstOutPatientPresTmp, true))
                                            {
                                                item.IsNotTakeBean = true;
                                                if (e.Column.FieldName == "AMOUNT")
                                                {
                                                    decimal oldAmount = Inventec.Common.TypeConvert.Parse.ToDecimal(view.ActiveEditor.OldEditValue.ToString());
                                                    SetOldAmountMediMaty(oldAmount, item.ID, item.PrimaryKey);
                                                }
                                                else if (e.Column.FieldName == "PATIENT_TYPE_ID")
                                                {
                                                    long oldPatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(view.ActiveEditor.OldEditValue.ToString());
                                                    SetOldPatientTypeMediMaty(oldPatientTypeId, item.ID, item.PrimaryKey);
                                                }

                                                MessageManager.Show(this, param, false);
                                                success = false;
                                                break;
                                            }
                                            else
                                            {
                                                lstOutPatientPres = lstOutPatientPresTmp;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        item.TotalPrice = CalculatePrice(item);
                                    }
                                    var primaryKeyOld = item.PrimaryKey;
                                    item.PrimaryKey = (mediMatyTypeADO.SERVICE_ID + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                                    if (rsMaxPresInBatch)
                                    {
                                        mediMatyTypeADOsAlertInTreatment.ForEach(o =>
                                        {
                                            if (o.PrimaryKey == primaryKeyOld)
                                                o.PrimaryKey = item.PrimaryKey;
                                        });
                                    }
                                    foreach (var itemTakeBean in lstOutPatientPres)
                                    {
                                        if (itemTakeBean.PrimaryKey == primaryKeyOld)
                                            itemTakeBean.PrimaryKey = item.PrimaryKey;
                                    }
                                    item.IsNotTakeBean = false;
                                }

                                //Takebean thành công tất cả thì xóa dòng cũ đi , add 2 bean mới vào
                                //Stent mới có trường hợp mediMatyTypeADOTemps >1
                                if (success && mediMatyTypeADOTemps.Count > 1)
                                {
                                    mediMatyTypeADOs.Remove(mediMatyTypeADO);
                                    mediMatyTypeADOs.AddRange(mediMatyTypeADOTemps);
                                }
                            }

                        }

                        if (e.Column.FieldName == "TUTORIAL")
                        {
                            if (!String.IsNullOrEmpty(mediMatyTypeADO.TUTORIAL) && Encoding.UTF8.GetByteCount(mediMatyTypeADO.TUTORIAL) > 1000)
                            {
                                mediMatyTypeADO.ErrorMessageTutorial = "Vượt quá ký tự cho phép";
                                mediMatyTypeADO.ErrorTypeTutorial = ErrorType.Warning;
                            }
                        }

                        if (e.Column.FieldName == "IsExpend" && mediMatyTypeADO.IsExpend == false)
                        {
                            mediMatyTypeADO.IsExpendType = false;
                        }

                        if (e.Column.FieldName == "NUM_ORDER_STR")
                        {
                            var NumOrderSTr = this.gridViewServiceProcess.EditingValue.ToString();
                            string str1 = "(";
                            long? NumOrder = null;
                            if (str1.Contains(NumOrderSTr))
                            {
                                string[] arrListStr = NumOrderSTr.Split('(');
                                NumOrder = long.Parse(arrListStr[0]);
                            }
                            else
                            {
                                NumOrder = long.Parse(NumOrderSTr);
                            }
                            mediMatyTypeADO.NUM_ORDER = NumOrder;
                            int rowHandleFocus = gridViewServiceProcess.FocusedRowHandle;
                            gridViewServiceProcess.GridControl.BeginUpdate();
                            gridViewServiceProcess.GridControl.DataSource = mediMatyTypeADOs.OrderBy(o => o.NUM_ORDER).ToList();
                            gridViewServiceProcess.GridControl.EndUpdate();
                            gridViewServiceProcess.FocusedRowHandle = rowHandleFocus;
                        }
                    }
                    mediMatyTypeADO.IsNotShowMessage = false;
                    if (mediMatyTypeADO.IsAllowOdd == false && mediMatyTypeADO.AMOUNT != null && mediMatyTypeADO.AMOUNT > 0 && (mediMatyTypeADO.AMOUNT.ToString().Contains(".") || mediMatyTypeADO.AMOUNT.ToString().Contains(",")) && !mediMatyTypeADO.AMOUNT.ToString().Split(new string[] { ".", "," }, StringSplitOptions.RemoveEmptyEntries)[1].Equals("0"))
                    {
                        mediMatyTypeADO.ErrorMessageAmount = ResourceMessage.ThuocVatTuKhongDuocPhepKeLe;
                        mediMatyTypeADO.ErrorTypeAmount = ErrorType.Warning;
                        mediMatyTypeADO.IsNotShowMessage = true;
                    }
                    mediMatyTypeADO.BK_AMOUNT = mediMatyTypeADO.AMOUNT;

                    this.SetTotalPrice__TrongDon();
                    this.ReloadDataAvaiableMediBeanInCombo();
                    if (this.currentMedicineTypeADOForEdit != null && mediMatyTypeADO.ID == this.currentMedicineTypeADOForEdit.ID)
                    {
                        this.currentMedicineTypeADOForEdit = mediMatyTypeADO;

                        if (GetSelectedOpionGroup() == 1 && (this.serviceReqMain != null && this.serviceReqMain.IS_MAIN_EXAM != 1) && (HisConfigCFG.IsUsingSubPrescriptionMechanism == "1") && !GlobalStore.IsCabinet)
                        {
                            this.currentMedicineTypeADOForEdit.IS_SUB_PRES = 1;
                            if (this.oldServiceReq != null && this.serviceReqMain != null && this.serviceReqMain.IS_SUB_PRES != 1)
                            {
                                this.currentMedicineTypeADOForEdit.IS_SUB_PRES = null;
                            }
                        }
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("gridViewServiceProcess_CellValueChanged.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                IsCellChangeAmount = false;
            }
        }
        private void gridViewMediMaty_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                try
                {
                    if (e.RowHandle < 0)
                        return;
                    bool IsAssignPresed = (bool)(gridViewMediMaty.GetRowCellValue(e.RowHandle, "IsExistAssignPres") ?? "");
                    if (e.Column.FieldName == "IsAssignPresed")
                    {
                        if (IsAssignPresed)
                        {
                            e.RepositoryItem = repPicIsAssignPresed;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                MediMatyTypeADO data = null;
                if (e.RowHandle > -1)
                {
                    data = (MediMatyTypeADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "PATIENT_TYPE_ID")
                    {
                        //if (this.actionType == GlobalVariables.ActionEdit)
                        // e.RepositoryItem = this.repositoryItemcboPatientType_TabMedicine_GridLookUp__Disable;
                        //else
                        //{
                        if ((data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD))
                            e.RepositoryItem = this.repositoryItemcboPatientType_TabMedicine_GridLookUp;
                        else
                            e.RepositoryItem = this.TextEditPatient_Type_Disable;
                        //}
                    }
                    else if (e.Column.FieldName == "EQUIPMENT_SET_ID")
                    {
                        if (data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
                        {
                            e.RepositoryItem = this.repositoryItemGridLookUpEditEquipmentSet__Enabled;
                        }
                        else
                        {
                            e.RepositoryItem = this.repositoryItemGridLookUpEditEquipmentSet__Disabled;
                        }
                    }
                    else if (e.Column.FieldName == "IsExpend")
                    {
                        //#16421 để key cấu hình giá trị 1: Không cho phép check hao phí với thuốc/vật tư không đính kèm
                        if (data.IsDisableExpend || ((data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD) && ((HisConfigCFG.IsNotAllowingExpendWithoutHavingParent && ((data.SereServParentId ?? 0) > 0 || GetSereServInKip() > 0)) || !HisConfigCFG.IsNotAllowingExpendWithoutHavingParent)))
                            e.RepositoryItem = this.repositoryItemChkIsExpend__MedicinePage;
                        else
                            e.RepositoryItem = this.repositoryItemChkIsExpend__MedicinePage_Disable;
                    }
                    else if (e.Column.FieldName == "IsExpendType")
                    {
                        if ((data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU) && data.IsExpend
                           && ((data.SereServParentId ?? 0) <= 0 && GetSereServInKip() <= 0)//TODO//Chỉ cho phép check khi có check "Hao phí", và ko có thông tin "dịch vụ cha"
                            )
                        {
                            e.RepositoryItem = this.repositoryItemchkIsExpendType_Enable;
                        }
                        else
                        {
                            e.RepositoryItem = this.repositoryItemchkIsExpendType_Disable;
                        }
                    }
                    else if (e.Column.FieldName == "IsKHBHYT")
                    {
                        if (this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT && (data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU || data.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT))
                            e.RepositoryItem = this.repositoryItemChkIsKH__MedicinePage;
                        else
                            e.RepositoryItem = this.repositoryItemChkIsKH_Disable__MedicinePage;
                    }
                    else if (e.Column.FieldName == "MEDICINE_USE_FORM_ID")
                    {
                        if ((data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC))
                            e.RepositoryItem = this.repositoryItemcboMedicineUseForm;
                        else
                            e.RepositoryItem = this.TextEditPatient_Type_Disable;
                    }
                    else if (e.Column.FieldName == "AMOUNT")
                    {
                        if (data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD
                            || HisConfigCFG.SplitOffset == GlobalVariables.CommonStringTrue || (data.IS_SPLIT_COMPENSATION.HasValue && data.IS_SPLIT_COMPENSATION == 1))
                            e.RepositoryItem = repositoryItemSpinAmount_Disable__MedicinePage;
                        else if (((data.IsAllowOdd.HasValue && data.IsAllowOdd.Value == true))
                            //&& (GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet)
                            )
                            e.RepositoryItem = this.repositoryItemSpinAmount_Le_MedicinePage;
                        else
                            e.RepositoryItem = this.repositoryItemSpinAmount__MedicinePage;
                    }
                    else if (e.Column.FieldName == "PRES_AMOUNT")
                    {
                        if (data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD
                            || HisConfigCFG.SplitOffset == GlobalVariables.CommonStringTrue || (data.IS_SPLIT_COMPENSATION.HasValue && data.IS_SPLIT_COMPENSATION == 1))
                            e.RepositoryItem = repositoryItemSpinAmount_Disable__MedicinePage;
                        else if (((data.IsAllowOdd.HasValue && data.IsAllowOdd.Value == true))
                            //&& (GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet)
                            )
                            e.RepositoryItem = this.repositoryItemSpinAmount_Le_MedicinePage;
                        else
                            e.RepositoryItem = this.repositoryItemSpinPreAmount__MedicinePage;
                    }
                    else if (e.Column.FieldName == "HDSD")
                    {
                        if (HisConfigCFG.SplitOffset == GlobalVariables.CommonStringTrue || (data.IS_SPLIT_COMPENSATION.HasValue && data.IS_SPLIT_COMPENSATION == 1))
                        {
                            e.RepositoryItem = this.TextEditPatient_Type_Disable;
                        }
                    }
                    else if (e.Column.FieldName == "IsOutKtcFee")
                    {
                        e.RepositoryItem = this.repositoryItemChkOutKtcFee_Enable_TabMedicine;
                    }
                    else if (e.Column.FieldName == "PrescriptionDays")
                    {
                        if (this.actionType == GlobalVariables.ActionAdd)
                            e.RepositoryItem = this.repositoryItemButtonEditManyDayPres;
                        else
                            e.RepositoryItem = this.TextEditPatient_Type_Disable;
                    }
                    else if (e.Column.FieldName == "EXCEED_LIMIT_IN_PRES_REASON")
                    {
                        if (!String.IsNullOrEmpty(data.EXCEED_LIMIT_IN_PRES_REASON) || (data.ALERT_MAX_IN_PRESCRIPTION != null && !CheckMaxInPrescriptionForMemoReason(data, data.AMOUNT)))
                            e.RepositoryItem = this.memoReasonMaxPrescription;
                        else
                            e.RepositoryItem = this.TextEditPatient_Type_Disable;
                    }
                    else if (e.Column.FieldName == "EXCEED_LIMIT_IN_DAY_REASON")
                    {
                        if (!String.IsNullOrEmpty(data.EXCEED_LIMIT_IN_DAY_REASON) || (data.ALERT_MAX_IN_DAY != null && !CheckMaxInPrescriptionInDayForMemoReason(data, data.AMOUNT)))
                            e.RepositoryItem = this.memoReasonMaxPrescription;
                        else
                            e.RepositoryItem = this.TextEditPatient_Type_Disable;
                    }
                    else if (e.Column.FieldName == "EXCEED_LIMIT_IN_BATCH_REASON")
                    {
                        if (data.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                        {
                            if (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                            {
                                if (data.ALERT_MAX_IN_TREATMENT.HasValue && data.IsAlertInTreatPresciption && ((mediMatyTypeADOsAlertInTreatment != null && mediMatyTypeADOsAlertInTreatment.Count > 0 && mediMatyTypeADOsAlertInTreatment.Exists(o => o.PrimaryKey == data.PrimaryKey)) || !String.IsNullOrEmpty(data.EXCEED_LIMIT_IN_BATCH_REASON)))
                                {
                                    e.RepositoryItem = this.repELInBatchCliEnable;
                                }
                                else
                                    e.RepositoryItem = this.repELInBatchCliDisable;
                            }
                            else
                            {
                                if (data.ALERT_MAX_IN_TREATMENT != null && data.NUMBER_PRESCIPTION_IN_TREATMENT == null)
                                    data.NUMBER_PRESCIPTION_IN_TREATMENT = GetAmountPrescriptionInBatch(data, currentTreatment.PATIENT_ID) ?? 0;
                                if (!String.IsNullOrEmpty(data.EXCEED_LIMIT_IN_BATCH_REASON) && data.ALERT_MAX_IN_TREATMENT != null && (data.NUMBER_PRESCIPTION_IN_TREATMENT > data.ALERT_MAX_IN_TREATMENT || data.AMOUNT > data.ALERT_MAX_IN_TREATMENT))
                                {
                                    e.RepositoryItem = this.memoReasonMaxPrescription;
                                }
                                else
                                    e.RepositoryItem = this.TextEditPatient_Type_Disable;
                            }
                        }
                        else
                            e.RepositoryItem = this.TextEditPatient_Type_Disable;
                    }
                    else if (e.Column.FieldName == "ODD_PRES_REASON")
                    {
                        if (!String.IsNullOrEmpty(data.ODD_PRES_REASON) || (!String.IsNullOrEmpty(data.ODD_WARNING_CONTENT) && (data.AMOUNT != (int)data.AMOUNT)))
                            e.RepositoryItem = this.memoReasonMaxPrescription;
                        else
                            e.RepositoryItem = this.TextEditPatient_Type_Disable;
                    }
                    else if (e.Column.FieldName == "OVER_RESULT_TEST_REASON")
                    {
                        if (data.IsEditOverResultTestReason)
                            e.RepositoryItem = this.memoReasonMaxPrescription;
                        else
                            e.RepositoryItem = this.TextEditPatient_Type_Disable;
                    }
                    else if (e.Column.FieldName == "OVER_KIDNEY_REASON")
                    {
                        if (data.IsEditOverKidneyReason)
                            e.RepositoryItem = this.memoReasonMaxPrescription;
                        else
                            e.RepositoryItem = this.TextEditPatient_Type_Disable;
                    }
                    else if (e.Column.FieldName == "INFORMATION_MEDICINE")
                    {
                        if ((data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC))
                            e.RepositoryItem = this.repInForMedicineEnable;
                        else
                            e.RepositoryItem = this.repInForMedicineDisable;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MediMatyTypeADO data_ManuMedicineADO = (MediMatyTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data_ManuMedicineADO != null)
                    {

                        if (e.Column.FieldName == "NUM_ORDER_STR")
                        {

                            if (data_ManuMedicineADO.MIXED_INFUSION != null)
                            {
                                e.Value = data_ManuMedicineADO.NUM_ORDER + "(" + data_ManuMedicineADO.MIXED_INFUSION + ")";
                            }
                            else
                            {
                                e.Value = data_ManuMedicineADO.NUM_ORDER;
                            }
                        }

                        if (e.Column.FieldName == "UseTimeToDisplay")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data_ManuMedicineADO.UseTimeTo ?? 0);
                        }
                        else if (e.Column.FieldName == "PrescriptionDays")
                        {
                            if (HisConfigCFG.ManyDayPrescriptionOption == 2
                               && data_ManuMedicineADO.IntructionTimeSelecteds != null
                               && data_ManuMedicineADO.IntructionTimeSelecteds.Count > 0
                               )
                            {
                                e.Value = data_ManuMedicineADO.IsMultiDateState ? GetDateFromManyDayPres(data_ManuMedicineADO.IntructionTimeSelecteds) : Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data_ManuMedicineADO.IntructionTimeSelecteds.First());
                            }
                        }
                        else if (e.Column.FieldName == "TotalPriceDisplay")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString(data_ManuMedicineADO.TotalPrice, ConfigApplications.NumberSeperator);
                        }
                        else if (e.Column.FieldName == "SERVICE_UNIT_NAME_DISPLAY")
                        {
                            if ((data_ManuMedicineADO.IsUseOrginalUnitForPres ?? false) == false
                                && !String.IsNullOrEmpty(data_ManuMedicineADO.CONVERT_UNIT_CODE)
                                && !String.IsNullOrEmpty(data_ManuMedicineADO.CONVERT_UNIT_NAME))
                            {
                                e.Value = data_ManuMedicineADO.CONVERT_UNIT_NAME;
                            }
                            else
                            {
                                e.Value = data_ManuMedicineADO.SERVICE_UNIT_NAME;
                            }
                        }
                        else if (e.Column.FieldName == "EXPIRED_DATE_DISPLAY")
                        {
                            if ((data_ManuMedicineADO.IsAssignPackage ?? false) == true)
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data_ManuMedicineADO.EXPIRED_DATE ?? 0);
                            }
                        }
                        else if (e.Column.FieldName == "TDL_PACKAGE_NUMBER_DISPLAY")
                        {
                            if ((data_ManuMedicineADO.IsAssignPackage ?? false) == true)
                            {
                                e.Value = data_ManuMedicineADO.TDL_PACKAGE_NUMBER;
                            }
                        }
                        else if (e.Column.FieldName == "REGISTER_NUMBER_DISPLAY")
                        {
                            if ((data_ManuMedicineADO.IsAssignPackage ?? false) == true)
                            {
                                e.Value = data_ManuMedicineADO.REGISTER_NUMBER;
                            }
                        }
                    }
                    else
                    {
                        e.Value = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                downHitInfo = null;
                GridView view = sender as GridView;
                try
                {
                    Inventec.Common.Logging.LogSystem.Debug("gridViewServiceProcess_MouseDown.1");
                    GridHitInfo hitInfo = view.CalcHitInfo(new Point(e.X, e.Y));
                    if (Control.ModifierKeys != Keys.None)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("gridViewServiceProcess_MouseDown.2");
                        //return;
                    }
                    else if (e.Button == MouseButtons.Left && hitInfo.InRow && hitInfo.RowHandle != DevExpress.XtraGrid.GridControl.NewItemRowHandle)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("gridViewServiceProcess_MouseDown.3");
                        downHitInfo = hitInfo;
                    }
                }
                catch (Exception exx)
                {
                    Inventec.Common.Logging.LogSystem.Warn(exx);
                }

                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;

                            int rowHandle = gridViewServiceProcess.GetVisibleRowHandle(hi.RowHandle);
                            var dataRow = (MediMatyTypeADO)gridViewServiceProcess.GetRow(rowHandle);
                            if (dataRow != null)
                            {
                                if (hi.Column.FieldName == "IsExpend" && (dataRow.IsDisableExpend || ((dataRow.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || dataRow.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU || dataRow.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD) && ((HisConfigCFG.IsNotAllowingExpendWithoutHavingParent && ((dataRow.SereServParentId ?? 0) > 0 || GetSereServInKip() > 0)) || !HisConfigCFG.IsNotAllowingExpendWithoutHavingParent))))
                                //&& (HisConfigCFG.IsNotAllowingExpendWithoutHavingParent && (dataRow.SereServParentId ?? 0) <= 0 && GetSereServInKip() <= 0))//Không cho phép check hao phí với thuốc/vật tư không đính kèm//Không cho phép check hao phí với thuốc/vật tư không đính kèm
                                {
                                    if (dataRow.IsDisableExpend)
                                        DevExpress.XtraEditors.XtraMessageBox.Show("Thuốc/vật tư " + dataRow.MEDICINE_TYPE_NAME + " được kê tại kho hao phí", HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
                                    Inventec.Common.Logging.LogSystem.Debug("gridViewServiceProcess_MouseDown.return__FieldName:IsExpend");
                                    return;
                                }
                                if (hi.Column.FieldName == "IsExpendType")//TODO//Chỉ cho phép check khi có check "Hao phí", và ko có thông tin "dịch vụ cha"
                                {
                                    bool valid = ((dataRow.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || dataRow.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU) && dataRow.IsExpend
                            && ((dataRow.SereServParentId ?? 0) <= 0 && GetSereServInKip() <= 0));
                                    if (!valid)
                                    {
                                        Inventec.Common.Logging.LogSystem.Debug("gridViewServiceProcess_MouseDown.return__FieldName:IsExpendType____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => valid), valid));
                                        return;
                                    }
                                }
                            }

                            view.ShowEditor();
                            CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {
                                checkEdit.Checked = !checkEdit.Checked;
                                view.CloseEditor();
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                        else if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit) || hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit))
                        {
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("hi.Column.FieldName", hi.Column.FieldName)
                                + Inventec.Common.Logging.LogUtil.TraceData("hi.RowHandle", hi.RowHandle));
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;

                            int rowHandle = gridViewServiceProcess.GetVisibleRowHandle(hi.RowHandle);
                            var dataRow = (MediMatyTypeADO)gridViewServiceProcess.GetRow(rowHandle);
                            if (dataRow != null)
                            {
                                view.ShowEditor();
                                if (hi.Column.FieldName == "SERVICE_CONDITION_NAME")
                                {
                                    ButtonEdit editor = view.ActiveEditor as ButtonEdit;
                                    ShowPopupContainerForServiceCondition(editor, dataRow);
                                }
                                else if (hi.Column.FieldName == "PATIENT_TYPE_ID")
                                {
                                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                                    if (editor != null)
                                    {
                                        editor.ShowPopup();
                                    }
                                }
                                //else if (hi.Column.FieldName == "OTHER_PAY_SOURCE_NAME")
                                //{
                                //    ButtonEdit editor = view.ActiveEditor as ButtonEdit;
                                //    ShowPopupContainerForOtherPaySource(editor, dataRow);
                                //}
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

        //Begin drag drop

        private void gridViewServiceProcess_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                //Inventec.Common.Logging.LogSystem.Debug("gridViewServiceProcess_MouseMove.1");
                GridView view = sender as GridView;
                if (e.Button == MouseButtons.Left && downHitInfo != null)
                {
                    //Inventec.Common.Logging.LogSystem.Debug("gridViewServiceProcess_MouseMove.2");
                    Size dragSize = SystemInformation.DragSize;
                    Rectangle dragRect = new Rectangle(new Point(downHitInfo.HitPoint.X - dragSize.Width / 2,
                        downHitInfo.HitPoint.Y - dragSize.Height / 2), dragSize);

                    if (!dragRect.Contains(new Point(e.X, e.Y)))
                    {
                        //Inventec.Common.Logging.LogSystem.Debug("gridViewServiceProcess_MouseMove.3");
                        view.GridControl.DoDragDrop(downHitInfo, DragDropEffects.All);
                        downHitInfo = null;
                    }
                }
                //Inventec.Common.Logging.LogSystem.Debug("gridViewServiceProcess_MouseMove.4");
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Warn(exx);
            }
        }

        private void gridControlServiceProcess_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("gridControlServiceProcess_DragOver.1");
                if (e.Data.GetDataPresent(typeof(GridHitInfo)))
                {
                    Inventec.Common.Logging.LogSystem.Debug("gridControlServiceProcess_DragOver.2");
                    GridHitInfo downHitInfo = e.Data.GetData(typeof(GridHitInfo)) as GridHitInfo;
                    if (downHitInfo == null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("gridControlServiceProcess_DragOver.3");
                        return;
                    }
                    Inventec.Common.Logging.LogSystem.Debug("gridControlServiceProcess_DragOver.4");
                    DevExpress.XtraGrid.GridControl grid = sender as DevExpress.XtraGrid.GridControl;
                    GridView view = grid.MainView as GridView;
                    GridHitInfo hitInfo = view.CalcHitInfo(grid.PointToClient(new Point(e.X, e.Y)));
                    if (hitInfo.InRow && hitInfo.RowHandle != downHitInfo.RowHandle && hitInfo.RowHandle != DevExpress.XtraGrid.GridControl.NewItemRowHandle)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("gridControlServiceProcess_DragOver.5");
                        e.Effect = DragDropEffects.Move;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("gridControlServiceProcess_DragOver.6");
                        e.Effect = DragDropEffects.None;
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("gridControlServiceProcess_DragOver.7");
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Warn(exx);
            }
        }

        private void gridControlServiceProcess_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("gridControlServiceProcess_DragDrop.1");
                DevExpress.XtraGrid.GridControl grid = sender as DevExpress.XtraGrid.GridControl;
                GridView view = grid.MainView as GridView;
                GridHitInfo srcHitInfo = e.Data.GetData(typeof(GridHitInfo)) as GridHitInfo;
                GridHitInfo hitInfo = view.CalcHitInfo(grid.PointToClient(new Point(e.X, e.Y)));
                int sourceRow = srcHitInfo.RowHandle;
                int targetRow = hitInfo.RowHandle;
                Inventec.Common.Logging.LogSystem.Debug("gridControlServiceProcess_DragDrop.2");
                MoveRow(sourceRow, targetRow);
                Inventec.Common.Logging.LogSystem.Debug("gridControlServiceProcess_DragDrop.3");
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Warn(exx);
            }
        }

        private void MoveRow(int sourceRow, int targetRow)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("MoveRow.1");
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sourceRow), sourceRow)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => targetRow), targetRow)
                    );
                if (sourceRow == targetRow) return;
                Inventec.Common.Logging.LogSystem.Debug("MoveRow.2");
                GridView view = gridViewServiceProcess;

                int rowHandletargetRow1 = gridViewServiceProcess.GetVisibleRowHandle(targetRow);
                var datatargetRow1 = (MediMatyTypeADO)gridViewServiceProcess.GetRow(rowHandletargetRow1);

                int rowHandletargetRow2 = gridViewServiceProcess.GetVisibleRowHandle(targetRow + 1);
                var datatargetRow2 = (MediMatyTypeADO)gridViewServiceProcess.GetRow(rowHandletargetRow2);

                int rowHandledragRow = gridViewServiceProcess.GetVisibleRowHandle(sourceRow);
                var datadragRow = (MediMatyTypeADO)gridViewServiceProcess.GetRow(rowHandledragRow);


                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rowHandletargetRow1), rowHandletargetRow1)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => datatargetRow1), datatargetRow1)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rowHandletargetRow2), rowHandletargetRow2)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => datatargetRow2), datatargetRow2)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rowHandledragRow), rowHandledragRow)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => datadragRow), datadragRow)
                    );
                long? num_order_New = 0;
                long? val1 = datatargetRow1.NUM_ORDER;
                if (datatargetRow2 == null)
                    num_order_New = val1 + 1;
                else
                {
                    long? val2 = datatargetRow2.NUM_ORDER;
                    num_order_New = (val1 + val2) / 2;
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => num_order_New), num_order_New));
                foreach (var item in this.mediMatyTypeADOs)
                {
                    if (item.NUM_ORDER >= num_order_New && item.NUM_ORDER < datadragRow.NUM_ORDER)
                    {
                        item.NUM_ORDER += 1;
                    }
                }
                if (datadragRow != null)
                {
                    datadragRow.NUM_ORDER = num_order_New;
                }

                if (CheckMediMatyType(this.mediMatyTypeADOs) == false)
                {
                    return;
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("datadragRow.NUM_ORDER", datadragRow.NUM_ORDER));
                gridControlServiceProcess.DataSource = null;
                gridControlServiceProcess.DataSource = this.mediMatyTypeADOs.OrderBy(o => o.NUM_ORDER).ToList();
                Inventec.Common.Logging.LogSystem.Debug("MoveRow.3");
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Warn(exx);
            }
        }
        ///End drag drop


        private void gridViewServiceProcess_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                MediMatyTypeADO data = view.GetFocusedRow() as MediMatyTypeADO;
                if (view.FocusedColumn.FieldName == "PATIENT_TYPE_ID" && view.ActiveEditor is GridLookUpEdit && (data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD))
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (data != null)
                    {
                        this.FillDataIntoPatientTypeCombo(data, editor);
                        editor.EditValue = data.PATIENT_TYPE_ID;
                    }
                }
                else if (view.FocusedColumn.FieldName == "IsKHBHYT" && view.ActiveEditor is CheckEdit && (data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU))
                {
                    CheckEdit editor = view.ActiveEditor as CheckEdit;
                    editor.ReadOnly = true;
                    // Kiểm tra các điều kiện: 
                    //1. Đối tượng BN = BHYT
                    //2. Loại hình thanh toán !=BHYT
                    //3. Dịch vụ đó có giá bán = BHYT
                    //4. Dịch vụ đó có giá bán BHYT<giá bán của loại đối tượng TT (xemlai...)
                    if (this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
                        && data.PATIENT_TYPE_ID != this.currentHisPatientTypeAlter.PATIENT_TYPE_ID
                        //    && this.servicePatyAllows != null
                        )
                    {
                        //    var isExistsService = this.servicePatyAllows[data.SERVICE_ID].Any(o => o.PATIENT_TYPE_ID == this.currentHisPatientTypeAlter.PATIENT_TYPE_ID);
                        //    if (isExistsService)
                        //    {
                        editor.ReadOnly = false;
                    }
                    //}//TODO
                }
                else if (view.FocusedColumn.FieldName == "EQUIPMENT_SET_ID" && view.ActiveEditor is GridLookUpEdit
                    && (data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM))
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (data != null && data.EQUIPMENT_SET_ID != null)
                    {
                        editor.EditValue = data.EQUIPMENT_SET_ID;
                        editor.Properties.Buttons[1].Visible = true;
                        editor.ButtonClick += reposityButtonClick;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_ShowingEditor(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                MediMatyTypeADO data = view.GetFocusedRow() as MediMatyTypeADO;
                if (data == null) return;

                if (view.FocusedColumn.FieldName == "IsExpendType")
                {
                    if ((data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || data.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
                        && data.IsExpend
                        && ((data.SereServParentId ?? 0) <= 0 && GetSereServInKip() <= 0))
                    {
                        //Nothing
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("gridViewServiceProcess_ShowingEditorFieldName:IsExpendType.Cancel");
                        e.Cancel = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void reposityButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridLookUpEdit editor = sender as GridLookUpEdit;
                    if (editor != null)
                    {
                        editor.EditValue = null;
                        editor.Properties.Buttons[1].Visible = false;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_CustomRowColumnError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                if (e.ColumnName == "AMOUNT"
                    || e.ColumnName == "PATIENT_TYPE_ID"
                    || e.ColumnName == "MEDICINE_USE_FORM_ID"
                    || e.ColumnName == "TUTORIAL"
                    || e.ColumnName == "MEDICINE_TYPE_NAME"
                    || e.ColumnName == "ODD_PRES_REASON"
                    || e.ColumnName == "OVER_RESULT_TEST_REASON"
                    || e.ColumnName == "OVER_KIDNEY_REASON")
                {
                    this.gridViewServiceProcess_CustomRowError(sender, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_CustomRowError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                var index = this.gridViewServiceProcess.GetDataSourceRowIndex(e.RowHandle);
                if (index < 0)
                {
                    e.Info.ErrorType = ErrorType.None;
                    e.Info.ErrorText = "";
                    return;
                }
                var listDatas = this.gridControlServiceProcess.DataSource as List<MediMatyTypeADO>;
                var row = listDatas[index];
                if (e.ColumnName == "AMOUNT")
                {
                    if (row.ErrorTypeAmount == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypeAmount);
                        e.Info.ErrorText = (string)(row.ErrorMessageAmount);
                    }
                    else if ((row.AmountAlert ?? 0) > 0)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypeAmount);
                        e.Info.ErrorText = ResourceMessage.SoLuongXuatLonHonSpoLuongKhadungTrongKho;
                    }
                    else if (row.ErrorTypeAmountHasRound == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypeAmountHasRound);
                        e.Info.ErrorText = (string)(row.ErrorMessageAmountHasRound);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                else if (e.ColumnName == "PATIENT_TYPE_ID")
                {
                    if (row.ErrorTypePatientTypeId == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypePatientTypeId);
                        e.Info.ErrorText = (string)(row.ErrorMessagePatientTypeId);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                else if (e.ColumnName == "MEDICINE_USE_FORM_ID")
                {
                    if (row.ErrorTypeMedicineUseForm == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypeMedicineUseForm);
                        e.Info.ErrorText = (string)(row.ErrorMessageMedicineUseForm);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                else if (e.ColumnName == "TUTORIAL")
                {
                    if (row.ErrorTypeTutorial == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypeTutorial);
                        e.Info.ErrorText = (string)(row.ErrorMessageTutorial);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                else if (e.ColumnName == "MEDICINE_TYPE_NAME")
                {
                    ErrorType errorType = (ErrorType)(ErrorType.None);
                    string errorText = "";

                    if (row.ErrorTypeIsAssignDay == ErrorType.Warning)
                    {
                        errorType = (ErrorType)(row.ErrorTypeIsAssignDay);
                        errorText += (string)(row.ErrorMessageIsAssignDay);
                    }
                    if (row.ErrorTypeMediMatyBean == ErrorType.Warning)
                    {
                        errorType = (ErrorType)(row.ErrorTypeMediMatyBean);
                        errorText += ((String.IsNullOrEmpty(errorText) ? "" : "; ") + (string)(row.ErrorMessageMediMatyBean));
                    }

                    e.Info.ErrorType = errorType;
                    e.Info.ErrorText = errorText;
                }
                else if (e.ColumnName == "ODD_PRES_REASON")
                {
                    if (row.ErrorTypeOddPres == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypeOddPres);
                        e.Info.ErrorText = (string)(row.ErrorMessageOddPres);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                else if (e.ColumnName == "OVER_RESULT_TEST_REASON")
                {
                    if (row.ErrorTypeOverResultTestReason == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypeOverResultTestReason);
                        e.Info.ErrorText = (string)(row.ErrorMessageOverResultTestReason);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                else if (e.ColumnName == "OVER_KIDNEY_REASON")
                {
                    if (row.ErrorTypeOverKidneyReason == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypeOverKidneyReason);
                        e.Info.ErrorText = (string)(row.ErrorMessageOverKidneyReason);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    e.Info.ErrorType = (ErrorType)(ErrorType.None);
                    e.Info.ErrorText = "";
                }
                catch { }

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                var index = this.gridViewServiceProcess.GetDataSourceRowIndex(e.RowHandle);
                if (index < 0) return;

                var listDatas = this.gridControlServiceProcess.DataSource as List<MediMatyTypeADO>;
                var dataRow = listDatas[index];
                if (dataRow != null)
                {
                    if (dataRow.IS_SUB_PRES == 1)
                    {
                        e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Italic);
                    }

                    //Thuốc đã hết hoặc không còn trong kho
                    if ((dataRow.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || dataRow.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU || dataRow.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD)
                        && this.mediMatyTypeADOs != null
                        && this.mediMatyTypeADOs.Count > 0
                        && (dataRow.AmountAlert ?? 0) > 0)
                    {
                        e.Appearance.ForeColor = System.Drawing.Color.Red;

                        //var mety = this.mediMatyTypeADOs.FirstOrDefault(o => o.SERVICE_ID == dataRow.SERVICE_ID);
                        //if (mety != null && (mety.AMOUNT ?? 0) <= (dataRow.ALERT_MIN_IN_STOCK ?? 0))
                        //{
                        //    //So luong thuoc ton kho nho hon canh bao ton kho cua thuoc thi boi do mau chu dong thuoc
                        //    e.Appearance.ForeColor = System.Drawing.Color.Red;
                        //    e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Italic);
                        //}
                    }
                    //Thuoc trong danh muc & vat tu trong danh muc && thuoc tu tuc hien thi mau xanh
                    else if (dataRow.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM || dataRow.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM)
                    {
                        e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Regular);
                        e.Appearance.ForeColor = System.Drawing.Color.Green;
                    }
                    else if (dataRow.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC)
                    {
                        e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Regular);
                        e.Appearance.ForeColor = System.Drawing.Color.Blue;
                    }

                    if ((dataRow.IS_STAR_MARK ?? 0) == 1)
                    {
                        if (dataRow.IS_SUB_PRES == 1)
                        {
                            e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Bold;
                            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, e.Appearance.FontStyleDelta);
                        }
                        else
                        {
                            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);
                        }

                    }

                    if (dataRow.IS_MIXED_MAIN == 1)
                    {
                        if (e.Column.FieldName == "NUM_ORDER_STR")
                        {
                            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlServiceProcess_DataSourceChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.treatmentFinishProcessor != null && !IsNotReloadTreatmentFinish)
                    this.treatmentFinishProcessor.Reload(this.ucTreatmentFinish, this.GetDateADO());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("gridViewServiceProcess_PopupMenuShowing.1");
                //if (GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet || GlobalStore.IsExecutePTTT)
                if (GlobalStore.IsExecutePTTT)
                    return;
                Inventec.Common.Logging.LogSystem.Info("gridViewServiceProcess_PopupMenuShowing.2");
                GridHitInfo hitInfo = e.HitInfo;
                if (hitInfo.InRowCell)
                {
                    Inventec.Common.Logging.LogSystem.Info("gridViewServiceProcess_PopupMenuShowing.3");
                    int visibleRowHandle = this.gridViewServiceProcess.GetVisibleRowHandle(hitInfo.RowHandle);
                    int[] selectedRows = this.gridViewServiceProcess.GetSelectedRows();
                    if (selectedRows != null && selectedRows.Length > 0)// && selectedRows.Contains(visibleRowHandle)
                    {
                        Inventec.Common.Logging.LogSystem.Info("gridViewServiceProcess_PopupMenuShowing.4");
                        this.InitMenu();
                    }
                }
                Inventec.Common.Logging.LogSystem.Info("gridViewServiceProcess_PopupMenuShowing.5");
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        private void RemoveItem(MediMatyTypeADO mediMatyTypeADO)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("gridViewServiceProcess_RowCellClick. REMOVE_SELECED_ROW");
                WaitingManager.Show();
                if (mediMatyTypeADO != null && TakeOrReleaseBeanWorker.ProcessDeleteRowMediMaty(this.intructionTimeSelecteds, mediMatyTypeADO))
                {
                    bool isReloadAvaible = (mediMatyTypeADO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                            || mediMatyTypeADO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU
                            || mediMatyTypeADO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD);
                    if (this.gridViewServiceProcess.FocusedRowHandle == this.gridViewServiceProcess.DataRowCount - 1)
                    {
                        this.idRow = this.idRow - stepRow;
                        if (this.idRow <= 0) this.idRow = 1;
                    }
                    this.gridViewServiceProcess.BeginUpdate();
                    this.gridViewServiceProcess.DeleteRow(this.gridViewServiceProcess.FocusedRowHandle);
                    this.gridViewServiceProcess.EndUpdate();

                    this.mediMatyTypeADOs.Remove(mediMatyTypeADO);
                    if (this.mediMatyTypeADOsAlertInTreatment != null && mediMatyTypeADOsAlertInTreatment.Count > 0 && mediMatyTypeADOsAlertInTreatment.Exists(o => o.PrimaryKey == mediMatyTypeADO.PrimaryKey))
                        mediMatyTypeADOsAlertInTreatment = mediMatyTypeADOsAlertInTreatment.Where(o => o.PrimaryKey != mediMatyTypeADO.PrimaryKey).ToList();
                    if (isReloadAvaible)
                        this.ReloadDataAvaiableMediBeanInCombo();

                    if (currentMedicineTypeADOForEdit != null &&
                        mediMatyTypeADO.PrimaryKey == currentMedicineTypeADOForEdit.PrimaryKey)
                    {
                        this.actionBosung = GlobalVariables.ActionAdd;
                        this.VisibleButton(this.actionBosung);
                        this.ReSetDataInputAfterAdd__MedicinePage();
                        txtMediMatyForPrescription.Text = "";
                        currentMedicineTypeADOForEdit = null;
                    }

                    var check = this.mediMatyTypeADOs.Where(o => o.MIXED_INFUSION == mediMatyTypeADO.MIXED_INFUSION).ToList();
                    if (check != null && check.Count() == 1)
                    {
                        this.mediMatyTypeADOs.FirstOrDefault(o => o.ID == check.FirstOrDefault().ID).MIXED_INFUSION = null;
                        this.mediMatyTypeADOs.FirstOrDefault(o => o.ID == check.FirstOrDefault().ID).IS_MIXED_MAIN = null;
                        this.mediMatyTypeADOs.FirstOrDefault(o => o.ID == check.FirstOrDefault().ID).TUTORIAL_INFUSION = null;
                    }
                    GetServiceTick(mediMatyTypeADO.SERVICE_ID, mediMatyTypeADO.IdRowPopupGrid, true);
                    this.gridControlServiceProcess.RefreshDataSource();
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("Remove row in grid fail or Call release bean fail. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mediMatyTypeADO), mediMatyTypeADO));
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void gridViewServiceProcess_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "REMOVE_SELECED_ROW")
                {
                    RemoveItem((MediMatyTypeADO)this.gridViewServiceProcess.GetFocusedRow());
                }
                else if (e.Column.FieldName == "INFORMATION_MEDICINE")
                {
                    var data = (MediMatyTypeADO)this.gridViewServiceProcess.GetFocusedRow();
                    if (data.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                        repInForMedicineEnable_ButtonClick(null, null);
                }
                else if (e.Column.FieldName == "INFORMATION_MEDICINE")
                {
                    var data = (MediMatyTypeADO)this.gridViewServiceProcess.GetFocusedRow();
                    if (data.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                        repELInBatchCliEnable_ButtonClick(null, null);
                }
                else
                {
                    var mediMatyTypeADO = (MediMatyTypeADO)this.gridViewServiceProcess.GetFocusedRow();
                    if (mediMatyTypeADO != null && (!String.IsNullOrEmpty(mediMatyTypeADO.CONTRAINDICATION) || !String.IsNullOrEmpty(mediMatyTypeADO.DESCRIPTION)))
                    {
                        if (!navBarControlChongChiDinhInfo.OptionsNavPane.IsAnimationInProgress && navBarControlChongChiDinhInfo.OptionsNavPane.NavPaneState == DevExpress.XtraNavBar.NavPaneState.Collapsed)
                        {
                            navBarControlChongChiDinhInfo.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Expanded;
                            //navBarControlChongChiDinhInfo.Update();
                        }
                        this.FillDataIntoDescriptionInfo(mediMatyTypeADO);
                    }
                    else
                    {
                        this.txtThongTinChongChiDinhThuoc.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridViewMediMaty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    Inventec.Common.Logging.LogSystem.Debug("gridViewMediMaty_KeyDown.1");
                    var medicineTypeADOForEdit = this.gridViewMediMaty.GetFocusedRow();
                    if (medicineTypeADOForEdit != null)
                    {
                        isShowContainerMediMaty = false;
                        isShowContainerMediMatyForChoose = true;
                        popupControlContainerMediMaty.HidePopup();
                        var selectedOpionGroup = GetSelectedOpionGroup();
                        if (selectedOpionGroup == 1)
                        {
                            MetyMatyTypeInStock_RowClick(medicineTypeADOForEdit);
                        }
                        else if (selectedOpionGroup == 2)
                        {
                            MedicineType_RowClick(medicineTypeADOForEdit);
                        }
                        else
                        {
                            MaterialTypeTSD_RowClick(medicineTypeADOForEdit);
                        }
                        Inventec.Common.Logging.LogSystem.Debug("gridViewMediMaty_KeyDown.2");
                        this.SetDataIntoChongCHiDinhInfo(medicineTypeADOForEdit);
                        Inventec.Common.Logging.LogSystem.Debug("gridViewMediMaty_KeyDown.3");
                    }
                    Inventec.Common.Logging.LogSystem.Debug("gridViewMediMaty_KeyDown.4");
                }
                else if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
                {
                    //Inventec.Common.Logging.LogSystem.Info("gridViewMediMaty_KeyDown.1____" + "FocusedRowHandle:" + this.gridViewMediMaty.FocusedRowHandle);
                    this.gridViewMediMaty.Focus();
                    this.gridViewMediMaty.FocusedRowHandle = this.gridViewMediMaty.FocusedRowHandle;
                    int rowHandle = gridViewMediMaty.GetVisibleRowHandle(this.gridViewMediMaty.FocusedRowHandle);
                    if (e.KeyCode == Keys.Down)
                    {
                        rowHandle += 1;
                    }
                    else if (e.KeyCode == Keys.Up)
                    {
                        rowHandle -= 1;
                    }
                    var medicineTypeADOForEdit = gridViewMediMaty.GetRow(rowHandle);
                    //Inventec.Common.Logging.LogSystem.Info("gridViewMediMaty_KeyDown.2____" + "FocusedRowHandle:" + this.gridViewMediMaty.FocusedRowHandle + "___rowHandle:" + rowHandle + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeADOForEdit), medicineTypeADOForEdit));
                    this.SetDataIntoChongCHiDinhInfo(medicineTypeADOForEdit);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediMaty_RowClick(object sender, RowClickEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("gridViewMediMaty_RowClick.1");
                    var medicineTypeADOForEdit = this.gridViewMediMaty.GetFocusedRow();
                    if (medicineTypeADOForEdit != null)
                    {
                        popupControlContainerMediMaty.HidePopup();
                        isShowContainerMediMaty = false;
                        isShowContainerMediMatyForChoose = true;
                        var selectedOpionGroup = GetSelectedOpionGroup();
                        if (selectedOpionGroup == 1)
                        {
                            MetyMatyTypeInStock_RowClick(medicineTypeADOForEdit);
                        }
                        else if (selectedOpionGroup == 2)
                        {
                            MedicineType_RowClick(medicineTypeADOForEdit);
                        }
                        else if (selectedOpionGroup == 3)
                        {
                            MaterialTypeTSD_RowClick(medicineTypeADOForEdit);
                        }

                        var currentMedicineTypeTmp = new MediMatyTypeADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(currentMedicineTypeTmp, medicineTypeADOForEdit);
                        Inventec.Common.Logging.LogSystem.Debug("gridViewMediMaty_RowClick.2");
                        if (currentMedicineTypeTmp != null && (!String.IsNullOrEmpty(currentMedicineTypeTmp.CONTRAINDICATION) || !String.IsNullOrEmpty(currentMedicineTypeTmp.DESCRIPTION)))
                        {
                            Inventec.Common.Logging.LogSystem.Debug("gridViewMediMaty_RowClick.3");
                            if (!navBarControlChongChiDinhInfo.OptionsNavPane.IsAnimationInProgress && navBarControlChongChiDinhInfo.OptionsNavPane.NavPaneState == DevExpress.XtraNavBar.NavPaneState.Collapsed)
                            {
                                navBarControlChongChiDinhInfo.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Expanded;
                            }
                            Inventec.Common.Logging.LogSystem.Debug("gridViewMediMaty_RowClick.4");
                            this.FillDataIntoDescriptionInfo(currentMedicineTypeTmp);
                            Inventec.Common.Logging.LogSystem.Debug("gridViewMediMaty_RowClick.5");
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Debug("gridViewMediMaty_RowClick.6");
                            this.txtThongTinChongChiDinhThuoc.Text = "";
                        }
                        if (selectedOpionGroup == 3)
                        {
                            btnAdd.PerformClick();
                        }
                    }
                    Inventec.Common.Logging.LogSystem.Debug("gridViewMediMaty_RowClick.7");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediMaty_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                //Inventec.Common.Logging.LogSystem.Debug("gridViewMediMaty_MouseMove.1");
                if (navBarControlChongChiDinhInfo.OptionsNavPane.IsAnimationInProgress)
                {
                    //Inventec.Common.Logging.LogSystem.Debug("gridViewMediMaty_MouseMove.2");
                    return;
                }
                GridView view = sender as GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if (hi.InRowCell)
                {
                    view.FocusedRowHandle = hi.RowHandle;
                    view.FocusedColumn = hi.Column;

                    int rowHandle = gridViewMediMaty.GetVisibleRowHandle(hi.RowHandle);
                    var dataRow = gridViewMediMaty.GetRow(rowHandle);

                    var currentMedicineTypeTmp = new MediMatyTypeADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(currentMedicineTypeTmp, dataRow);
                    //Inventec.Common.Logging.LogSystem.Debug("gridViewMediMaty_MouseMove.3");
                    if (currentMedicineTypeTmp != null && (!String.IsNullOrEmpty(currentMedicineTypeTmp.CONTRAINDICATION) || !String.IsNullOrEmpty(currentMedicineTypeTmp.DESCRIPTION)))
                    {
                        if (!navBarControlChongChiDinhInfo.OptionsNavPane.IsAnimationInProgress && navBarControlChongChiDinhInfo.OptionsNavPane.NavPaneState == DevExpress.XtraNavBar.NavPaneState.Collapsed)
                        {
                            navBarControlChongChiDinhInfo.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Expanded;
                        }
                        //Inventec.Common.Logging.LogSystem.Debug("gridViewMediMaty_MouseMove.4");
                        this.FillDataIntoDescriptionInfo(currentMedicineTypeTmp);
                        //Inventec.Common.Logging.LogSystem.Debug("gridViewMediMaty_MouseMove.5");
                    }
                    else
                    {
                        this.txtThongTinChongChiDinhThuoc.Text = "";
                    }
                }
                //Inventec.Common.Logging.LogSystem.Debug("gridViewMediMaty_MouseMove.6");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMediMaty_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex] != null && ((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex] is DMediStock1ADO)
                    {
                        DMediStock1ADO data = (DMediStock1ADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                        if (data != null)
                        {
                            //#22134
                            //- Sửa lại cột "giá nhập" --> "giá bán". Trường này lấy dữ liệu từ: LAST_EXP_PRICE, LAST_EXP_VAT_RATIO. Theo công thức:
                            //"giá bán" = LAST_EXP_PRICE * (1 + LAST_EXP_VAT_RATIO).
                            //- Sửa ở combobox "tồn thuốc", và "combobox" kê thuốc ngoài kho.
                            //- Lưu ý: cần check trường hợp null, nếu cả 2 trường null thì hiển thị trống. Còn 1 trong 2 trường null, thì trường nào null thì coi giá trị trường đấy = 0.
                            if (e.Column.FieldName == "IMP_PRICE_DISPLAY")
                            {
                                if (data.LAST_EXP_PRICE.HasValue || data.LAST_EXP_VAT_RATIO.HasValue)
                                {
                                    decimal? priceRaw = (data.LAST_EXP_PRICE ?? 0) * (1 + (data.LAST_EXP_VAT_RATIO ?? 0));
                                    priceRaw = (data.CONVERT_RATIO.HasValue && data.CONVERT_RATIO > 0) ? priceRaw / data.CONVERT_RATIO.Value : priceRaw;

                                    //if (data.REMAIN_REUSE_COUNT.HasValue && data.REMAIN_REUSE_COUNT > 0)
                                    //{
                                    //    priceRaw = priceRaw / data.REMAIN_REUSE_COUNT;
                                    //}
                                    e.Value = Inventec.Common.Number.Convert.NumberToString(priceRaw ?? 0, ConfigApplications.NumberSeperator);
                                }
                            }
                            else if (e.Column.FieldName == "AMOUNT_DISPLAY")
                            {
                                if (data.AMOUNT.HasValue)
                                {
                                    e.Value = Inventec.Common.Number.Convert.NumberToStringRoundAuto(data.AMOUNT ?? 0, 6);
                                }
                            }
                            else if (e.Column.FieldName == "IMP_VAT_RATIO_DISPLAY")
                            {
                                e.Value = data.IMP_VAT_RATIO * 100;
                            }
                            else if (e.Column.FieldName == "SERVICE_UNIT_NAME_DISPLAY")
                            {
                                if (
                                    (data.IsUseOrginalUnitForPres ?? false) == false &&//TODO
                                    !String.IsNullOrEmpty(data.CONVERT_UNIT_CODE)
                                    && !String.IsNullOrEmpty(data.CONVERT_UNIT_NAME))
                                {
                                    e.Value = data.CONVERT_UNIT_NAME;
                                }
                                else
                                {
                                    e.Value = data.SERVICE_UNIT_NAME;
                                }
                            }
                            else if (e.Column.FieldName == "USE_REMAIN_COUNT_DISPLAY")
                            {
                                if (!String.IsNullOrEmpty(data.CONVERT_UNIT_CODE)
                                        && !String.IsNullOrEmpty(data.CONVERT_UNIT_NAME))
                                {
                                    e.Value = data.CONVERT_UNIT_NAME;
                                }
                                else
                                {
                                    e.Value = data.SERVICE_UNIT_NAME;
                                }
                            }
                            else if (e.Column.FieldName == "EXPIRED_DATE_DISPLAY")
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)(data.EXPIRED_DATE ?? 0));
                            }
                            else if (e.Column.FieldName == "IsAssignPresed")
                            {
                                if (data.IsExistAssignPres)
                                    e.Value = imageList1.Images[0];
                                else
                                    e.Value = null;
                            }
                        }
                    }
                    if (((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex] != null && ((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex] is MedicineMaterialTypeComboADO)
                    {
                        MedicineMaterialTypeComboADO data = (MedicineMaterialTypeComboADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                        if (data != null)
                        {
                            if (e.Column.FieldName == "IMP_PRICE_DISPLAY")
                            {
                                if (data.LAST_EXP_PRICE.HasValue || data.LAST_EXP_VAT_RATIO.HasValue)
                                {
                                    decimal? priceRaw = (data.LAST_EXP_PRICE ?? 0) * (1 + (data.LAST_EXP_VAT_RATIO ?? 0));
                                    priceRaw = (data.CONVERT_RATIO.HasValue && data.CONVERT_RATIO > 0) ? priceRaw / data.CONVERT_RATIO.Value : priceRaw;
                                    e.Value = Inventec.Common.Number.Convert.NumberToString(priceRaw ?? 0, ConfigApplications.NumberSeperator);
                                }
                            }
                            else if (e.Column.FieldName == "AMOUNT_DISPLAY")
                            {
                                if (data.AMOUNT.HasValue)
                                {
                                    e.Value = Inventec.Common.Number.Convert.NumberToStringRoundAuto(data.AMOUNT ?? 0, 6);
                                }
                            }
                            else if (e.Column.FieldName == "IMP_VAT_RATIO_DISPLAY")
                            {
                                e.Value = data.IMP_VAT_RATIO * 100;
                            }
                            else if (e.Column.FieldName == "SERVICE_UNIT_NAME_DISPLAY")
                            {
                                if (!String.IsNullOrEmpty(data.CONVERT_UNIT_CODE)
                                        && !String.IsNullOrEmpty(data.CONVERT_UNIT_NAME))
                                {
                                    e.Value = data.CONVERT_UNIT_NAME;
                                }
                                else
                                {
                                    e.Value = data.SERVICE_UNIT_NAME;
                                }
                            }
                            else if (e.Column.FieldName == "USE_REMAIN_COUNT_DISPLAY")
                            {
                                if (!String.IsNullOrEmpty(data.CONVERT_UNIT_CODE)
                                        && !String.IsNullOrEmpty(data.CONVERT_UNIT_NAME))
                                {
                                    e.Value = data.CONVERT_UNIT_NAME;
                                }
                                else
                                {
                                    e.Value = data.SERVICE_UNIT_NAME;
                                }
                            }
                            else if (e.Column.FieldName == "IsAssignPresed")
                            {
                                if (data.IsExistAssignPres)
                                    e.Value = imageList1.Images[0];
                                else
                                    e.Value = null;
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

        private void gridViewMediMaty_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                DMediStock1ADO dMediStock = gridViewMediMaty.GetRow(e.RowHandle) as DMediStock1ADO;
                if (dMediStock != null)
                {
                    if ((dMediStock.IS_STAR_MARK ?? 0) == 1)
                    {
                        e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);
                    }
                    if (IsFullHeinInfo(dMediStock) && !String.IsNullOrEmpty(HisConfigCFG.BhytColorCode))
                    {
                        e.Appearance.ForeColor = GetColor(HisConfigCFG.BhytColorCode);
                    }
                }

                MedicineMaterialTypeComboADO medicineMaterialTypeComboADO = gridViewMediMaty.GetRow
(e.RowHandle) as MedicineMaterialTypeComboADO;
                if (medicineMaterialTypeComboADO != null)
                {
                    if ((medicineMaterialTypeComboADO.IS_STAR_MARK ?? 0) == 1)
                    {
                        e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);
                    }
                    if (IsFullHeinInfo(medicineMaterialTypeComboADO) && !String.IsNullOrEmpty(HisConfigCFG.BhytColorCode))
                    {
                        e.Appearance.ForeColor = GetColor(HisConfigCFG.BhytColorCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        Color GetColor(string colorCode)
        {
            try
            {
                if (!String.IsNullOrEmpty(colorCode))
                {
                    return System.Drawing.ColorTranslator.FromHtml(colorCode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return Color.Red;
        }

        private void gridViewTutorial_RowClick(object sender, RowClickEventArgs e)
        {
            try
            {
                HIS_MEDICINE_TYPE_TUT medicineTypeTut = gridViewTutorial.GetFocusedRow() as HIS_MEDICINE_TYPE_TUT;
                if (medicineTypeTut != null)
                {
                    popupControlContainerTutorial.HidePopup();
                    isShowContainerTutorial = false;
                    Tutorial_RowClick(medicineTypeTut);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTutorial_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var medicineTypeADOForEdit = this.gridViewMediMaty.GetFocusedRow();
                    if (medicineTypeADOForEdit != null)
                    {
                        isShowContainerMediMaty = false;
                        isShowContainerMediMatyForChoose = true;
                        popupControlContainerMediMaty.HidePopup();
                        var selectedOpionGroup = GetSelectedOpionGroup();
                        if (selectedOpionGroup == 1)
                        {
                            MetyMatyTypeInStock_RowClick(medicineTypeADOForEdit);
                        }
                        else if (selectedOpionGroup == 2)
                        {
                            MedicineType_RowClick(medicineTypeADOForEdit);
                        }
                        else
                        {
                            MaterialTypeTSD_RowClick(medicineTypeADOForEdit);
                        }
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    this.gridViewMediMaty.Focus();
                    this.gridViewMediMaty.FocusedRowHandle = this.gridViewMediMaty.FocusedRowHandle;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Nếu takebean thất bại thì cho số lượng về giá trị cũ
        /// </summary>
        /// <param name="amount">Số lượng cũ</param>
        /// <param name="mediMateId"></param>
        /// <param name="privateKey"></param>
        private void SetOldAmountMediMaty(decimal amount, long mediMateId, string privateKey)
        {
            try
            {
                if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0)
                {
                    foreach (var item in mediMatyTypeADOs)
                    {
                        if (item.ID == mediMateId && privateKey == item.PrimaryKey)
                        {
                            item.AMOUNT = amount;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetOldPatientTypeMediMaty(long patientTypeId, long mediMateId, string privateKey)
        {
            try
            {
                if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0)
                {
                    foreach (var item in mediMatyTypeADOs)
                    {
                        if (item.ID == mediMateId && privateKey == item.PrimaryKey)
                        {
                            HIS_PATIENT_TYPE patientType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == patientTypeId);
                            if (patientType == null)
                                throw new Exception("Sua DTTT takebean that bai. Khong lay duoc DTTT cu");

                            item.PATIENT_TYPE_ID = patientType.ID;
                            item.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                            item.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;

                            this.UpdateExpMestReasonInDataRow(item);

                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ShowAlert(CommonParam param)
        {
            try
            {
                if ((param.Messages != null && param.Messages.Count > 0)
                                                    || (param.BugCodes != null && param.BugCodes.Count > 0))
                {
                    MessageManager.ShowAlert(this, param, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void tooltipService_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {    
            try
            {
                if (e.Info == null && e.SelectedControl == this.gridControlServiceProcess)
                {
                    string text = "";
                    DevExpress.XtraGrid.Views.Grid.GridView view = this.gridControlServiceProcess.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)        
                    {
                        if (this.lastRowHandle != info.RowHandle || this.lastColumn != info.Column)     
                        {
                            this.lastColumn = info.Column;
                            this.lastRowHandle = info.RowHandle;
                            bool IsAssignDay = Inventec.Common.TypeConvert.Parse.ToBoolean((view.GetRowCellValue(this.lastRowHandle, "IsAssignDay") ?? "false").ToString());
                            string ErrorMessageIsAssignDay = (view.GetRowCellValue(lastRowHandle, "ErrorMessageIsAssignDay") ?? "").ToString();
                            string ErrorMessageMediMatyBean = (view.GetRowCellValue(lastRowHandle, "ErrorMessageMediMatyBean") ?? "").ToString();
                            int DataType = Inventec.Common.TypeConvert.Parse.ToInt32((view.GetRowCellValue(this.lastRowHandle, "DataType") ?? "").ToString());
                            decimal AmountAlert = Inventec.Common.TypeConvert.Parse.ToDecimal((view.GetRowCellValue(this.lastRowHandle, "AmountAlert") ?? "").ToString());
                            int IsSubPres = Inventec.Common.TypeConvert.Parse.ToInt32((view.GetRowCellValue(this.lastRowHandle, "IS_SUB_PRES") ?? "").ToString());

                            //Gán toolip cảnh báo thuốc đa kê trong ngày
                            if (!String.IsNullOrEmpty(ErrorMessageIsAssignDay))
                            {
                                if (DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                                    || DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM
                                    || DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC)
                                {
                                    text = ResourceMessage.CanhBaoThuocDaKeTrongNgay;
                                }
                            }

                            if ((DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                                   || DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM
                                   || DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC)
                                && IsSubPres == 1)
                            {
                                text += ";" + ResourceMessage.ThuocVatTuKhongChiemKhaDung;
                            }

                            //Gán tooltip cảnh báo thuốc đã hết hoặc không còn trong kho
                            if ((DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU))
                                if (AmountAlert > 0)
                                {
                                    text += ";" + ResourceMessage.SoLuongXuatLonHonSpoLuongKhadungTrongKho;
                                }
                                else if (!String.IsNullOrEmpty(ErrorMessageMediMatyBean))
                                {
                                    text = ErrorMessageMediMatyBean;
                                }
                            //else if (DataType == THUOC_DM || DataType == VATTU_DM || DataType == THUOC_TUTUC)
                            //{
                            //    text += ResourceMessage.SoLuongXuatLonHonSpoLuongKhadungTrongKho;
                            //}
                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
                else if (e.Info == null && e.SelectedControl == this.gridControlMediMaty)
                {
                    string text = "";
                    DevExpress.XtraGrid.Views.Grid.GridView view = this.gridControlMediMaty.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (this.lastRowHandle != info.RowHandle || this.lastColumn != info.Column)
                        {
                            this.lastColumn = info.Column;
                            this.lastRowHandle = info.RowHandle;
                            bool IsExistAssignPres = Inventec.Common.TypeConvert.Parse.ToBoolean((view.GetRowCellValue(this.lastRowHandle, "IsExistAssignPres") ?? "false").ToString());
                            if (IsExistAssignPres)
                                text += "Thuốc/vật tư đã kê";
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

        void DelegateSelectMultiDateManyDayPres(List<DateTime?> datas, DateTime time)
        {
            try
            {
                List<long> insTimes = new List<long>();
                var mediMatyTypeADO = (MediMatyTypeADO)this.gridViewServiceProcess.GetFocusedRow();
                if (datas != null && datas.Count > 0)
                {
                    for (int i = 0, j = datas.Count; i < j; i++)
                    {
                        var dt = new DateTime(datas[i].Value.Year, datas[i].Value.Month, datas[i].Value.Day, time.Hour, time.Minute, 0);
                        insTimes.Add(Inventec.Common.TypeConvert.Parse.ToInt64(dt.ToString("yyyyMMddHHmm") + "00"));
                    }

                }
                mediMatyTypeADO.IntructionTimeSelecteds = insTimes;
                mediMatyTypeADO.IsMultiDateState = true;

                if (this.gridViewServiceProcess.IsEditing)
                    this.gridViewServiceProcess.CloseEditor();

                if (this.gridViewServiceProcess.FocusedRowModified)
                    this.gridViewServiceProcess.UpdateCurrentRow();

                this.gridControlServiceProcess.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void DelegateSelectSingleDayPres(DateTime date)
        {
            try
            {
                List<long> insTimes = new List<long>();
                insTimes.Add(Inventec.Common.TypeConvert.Parse.ToInt64(date.ToString("yyyyMMddHHmm") + "00"));

                var mediMatyTypeADO = (MediMatyTypeADO)this.gridViewServiceProcess.GetFocusedRow();
                mediMatyTypeADO.IntructionTimeSelecteds = insTimes;
                mediMatyTypeADO.IsMultiDateState = false;

                if (this.gridViewServiceProcess.IsEditing)
                    this.gridViewServiceProcess.CloseEditor();

                if (this.gridViewServiceProcess.FocusedRowModified)
                    this.gridViewServiceProcess.UpdateCurrentRow();

                this.gridControlServiceProcess.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEditManyDayPres_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                List<DateTime?> datas = new List<DateTime?>();
                var mediMatyTypeADO = (MediMatyTypeADO)this.gridViewServiceProcess.GetFocusedRow();

                if (mediMatyTypeADO.IntructionTimeSelecteds != null && mediMatyTypeADO.IntructionTimeSelecteds.Count > 0)
                {
                    for (int i = 0, j = mediMatyTypeADO.IntructionTimeSelecteds.Count; i < j; i++)
                    {
                        var dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(mediMatyTypeADO.IntructionTimeSelecteds[i]) ?? DateTime.Now;
                        datas.Add(dt);
                    }

                    if (mediMatyTypeADO.IsMultiDateState)
                    {
                        frmMultiIntructonTime frmMultiIntructonTime = new frmMultiIntructonTime(datas, datas[0].Value, DelegateSelectMultiDateManyDayPres);
                        frmMultiIntructonTime.ShowDialog();
                    }
                    else
                    {
                        frmChoiceSingleDate frmChoiceSingleDate = new frmChoiceSingleDate(datas[0], DelegateSelectSingleDayPres);
                        frmChoiceSingleDate.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemGridLookupEditExpendTypeHasValue_Enable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    //var mediMatyTypeADO = (MediMatyTypeADO)this.gridViewServiceProcess.GetFocusedRow();
                    //mediMatyTypeADO.ExpendTypeId = null;

                    //if (this.gridViewServiceProcess.IsEditing)
                    //    this.gridViewServiceProcess.CloseEditor();

                    //if (this.gridViewServiceProcess.FocusedRowModified)
                    //    this.gridViewServiceProcess.UpdateCurrentRow();

                    //this.gridControlServiceProcess.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TreatmentFinishCheckChanged(bool isChecked)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("TreatmentFinishCheckChanged.1");
                if (isChecked)
                {
                    Inventec.Common.Logging.LogSystem.Debug("TreatmentFinishCheckChanged.2");
                    //if (this.lciUCBottomPanel.Height < 270)
                    //{
                    int heightUCBottom = this.lciUCBottomPanel.Height;
                    int heightUCTop = this.lciUCTopPanel.Height;
                    int heightPlus = 270 - heightUCBottom;

                    //this.lciUCBottomPanel.MinSize = new System.Drawing.Size(0, 270);
                    //this.lciUCBottomPanel.MaxSize = new System.Drawing.Size(0, 270);
                    this.lciUCBottomPanel.MinSize = new Size();
                    this.lciUCBottomPanel.MaxSize = new Size();
                    //this.lciUCTopPanel.Height = heightUCTop - heightPlus;
                    if (layoutControlItemDHSTInfo.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                        this.lciUCBottomPanel.Height = sizeExpMest.Height + layoutControlItemDHSTInfo.Size.Height;
                    else
                        this.lciUCBottomPanel.Height = sizeExpMest.Height;
                    //}

                    if (!GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet && this.currentTreatmentWithPatientType.NEED_SICK_LEAVE_CERT.HasValue && this.currentTreatmentWithPatientType.NEED_SICK_LEAVE_CERT.Value == 1)
                    {
                        this.treatmentFinishProcessor.InitNeedSickLeaveCert(this.ucTreatmentFinish, true);
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("TreatmentFinishCheckChanged.3");
                    int heightUCTop = this.lciUCTopPanel.Height;

                    this.lciUCBottomPanel.MinSize = new System.Drawing.Size(0, 80);
                    this.lciUCBottomPanel.MaxSize = new System.Drawing.Size(0, 80);
                    this.lciUCTopPanel.Height = heightUCTop + 204;
                    this.lciUCBottomPanel.Height = 80;
                }
                Inventec.Common.Logging.LogSystem.Debug("TreatmentFinishCheckChanged.4");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AutoTreatmentFinish__Checked()
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionView)
                {
                    btnSave.Enabled = btnSaveAndPrint.Enabled = isCheckAssignServiceSimultaneityOption ? false : true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion


        private void txtThoiGianTho_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = ResourceMessage.TruongSoLuongNhapSaiDinhDang;
                e.ExceptionMode = ExceptionMode.DisplayError;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTocDoTho_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = ResourceMessage.TruongSoLuongNhapSaiDinhDang;
                e.ExceptionMode = ExceptionMode.DisplayError;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtThoiGianTho_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                this.SpinKeyPress(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtThoiGianTho_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.txtTutorial.SelectAll();
                    this.txtTutorial.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTocDoTho_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtThoiGianTho.SelectAll();
                    txtThoiGianTho.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtThoiGianTho_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                this.SpinValidating(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTocDoTho_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                this.SpinValidating(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtThoiGianTho_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.CalculateAmount();
                this.SetHuongDanFromSoLuongNgay();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTocDoTho_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.CalculateAmount();
                this.SetHuongDanFromSoLuongNgay();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkHomePres_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (lciForchkThongTinMat.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    chkEyeInfo.Enabled = chkHomePres.Checked;

                    //+ TH1: Kê đơn mới: khi người dùng tích "Đơn mang về" tự động enable checkbox và sét trạng thái gần nhất cho nó.
                    //+ TH2: Sửa đơn và đơn cũ không được tích "Đơn mang về": khi người tích "Đơn mang về" tự động enable checkbox và sét trạng thái gần nhất cho nó.
                    //+ TH3: Sửa đơn và đơn cũ đã được tích "Đơn mang về": chỉ enable checkbox và trạng thái là không tích (Người dùng muốn sửa thì sẽ tự tích vào để chọn sửa).
                    if (chkHomePres.Checked)
                    {
                        if (this.oldServiceReq == null || this.oldServiceReq.ID == 0)
                        {
                            chkEyeInfo.Checked = this.vlStateChkEyeInfo;
                        }
                        else
                        {
                            isNotShowfrmEyeInfo = true;
                            if (this.oldServiceReq.IS_HOME_PRES.HasValue && this.oldServiceReq.IS_HOME_PRES == 1)
                            {
                                chkEyeInfo.Checked = false;
                            }
                            else
                            {
                                chkEyeInfo.Checked = this.vlStateChkEyeInfo;
                            }
                            isNotShowfrmEyeInfo = false;
                        }
                        if (this.currentAdviseFormADO != null && this.currentAdviseFormADO.AutoGetHomePres == true)
                        {
                            bbtnF6_ItemClick(null, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkThongTinMat_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (lciForchkThongTinMat.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                    return;

                try
                {
                    if (isNotLoadWhileChangeControlStateInFirst)
                    {
                        return;
                    }
                    //WaitingManager.Show();

                    HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.chkEyeInfo && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                    if (csAddOrUpdate != null)
                    {
                        csAddOrUpdate.VALUE = (chkEyeInfo.Checked ? "1" : "");
                    }
                    else
                    {
                        csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                        csAddOrUpdate.KEY = ControlStateConstan.chkEyeInfo;
                        csAddOrUpdate.VALUE = (chkEyeInfo.Checked ? "1" : "");
                        csAddOrUpdate.MODULE_LINK = moduleLink;
                        if (this.currentControlStateRDO == null)
                            this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                        this.currentControlStateRDO.Add(csAddOrUpdate);
                    }
                    this.controlStateWorker.SetData(this.currentControlStateRDO);
                    //WaitingManager.Hide();
                }
                catch (Exception ex)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }

                this.vlStateChkEyeInfo = chkEyeInfo.Checked;

                if (chkEyeInfo.Checked)
                {
                    if (!isNotShowfrmEyeInfo)
                    {
                        if (oldServiceReq != null && oldServiceReq.ID > 0 && ServiceReqEye == null)
                        {
                            //CommonParam param = new CommonParam();
                            //HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                            //serviceReqFilter.ID = oldServiceReq.ID;
                            //var ServiceReqEyes = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(RequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumerNoStore, serviceReqFilter, ProcessLostToken, param);
                            //ServiceReqEye = ServiceReqEyes != null && ServiceReqEyes.Count > 0 ? ServiceReqEyes.FirstOrDefault() : null;

                            ServiceReqEye = new HIS_SERVICE_REQ();
                            ServiceReqEye.TREAT_EYE_TENSION_RIGHT = oldServiceReq.TREAT_EYE_TENSION_RIGHT;
                            ServiceReqEye.TREAT_EYE_TENSION_LEFT = oldServiceReq.TREAT_EYE_TENSION_LEFT;
                            ServiceReqEye.TREAT_EYESIGHT_RIGHT = oldServiceReq.TREAT_EYESIGHT_RIGHT;
                            ServiceReqEye.TREAT_EYESIGHT_LEFT = oldServiceReq.TREAT_EYESIGHT_LEFT;
                            ServiceReqEye.TREAT_EYESIGHT_GLASS_RIGHT = oldServiceReq.TREAT_EYESIGHT_GLASS_RIGHT;
                            ServiceReqEye.TREAT_EYESIGHT_GLASS_LEFT = oldServiceReq.TREAT_EYESIGHT_GLASS_LEFT;
                            ServiceReqEye.ID = oldServiceReq.ID;
                            ServiceReqEye.IS_HOME_PRES = oldServiceReq.IS_HOME_PRES;
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => oldServiceReq), oldServiceReq) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ServiceReqEye), ServiceReqEye));
                        }
                        frmEyeInfo frmEyeInfo = new frmEyeInfo(ServiceReqEye, ServiceReqEyeUpdateInfo);
                        frmEyeInfo.ShowDialog();
                    }
                }
                else
                {
                    this.ServiceReqEye = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkShowLo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!HisConfigCFG.IsAllowAssignPresByPackage)
                    return;

                var selectedOpionGroup = GetSelectedOpionGroup();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => selectedOpionGroup), selectedOpionGroup) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => chkShowLo.Checked), chkShowLo.Checked));
                if (selectedOpionGroup == 1)
                {
                    this.theRequiredWidth = 1030;
                    this.theRequiredHeight = 200;
                    this.RebuildMediMatyWithInControlContainer();

                    if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0)
                    {
                        bool isRefesh = false;
                        foreach (var item in this.mediMatyTypeADOs)
                        {
                            if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD)
                            {
                                isRefesh = true;
                                if (chkShowLo.Checked)
                                {
                                    if (item.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC && (item.MAME_ID ?? 0) > 0)// && item.MedicineBean1Result != null && item.MedicineBean1Result.Count > 0
                                    {
                                        item.IsAssignPackage = true;
                                        Inventec.Common.Logging.LogSystem.Debug("Chuyen doi thuoc: theo loai -> theo lo thanh cong____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.MAME_ID), item.MAME_ID) + "___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.MEDICINE_TYPE_NAME), item.MEDICINE_TYPE_NAME) + "___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.TDL_PACKAGE_NUMBER), item.TDL_PACKAGE_NUMBER));
                                    }
                                    else if (item.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT && (item.MAME_ID ?? 0) > 0)// && item.MaterialBean1Result != null && item.MaterialBean1Result.Count > 0
                                    {
                                        item.IsAssignPackage = true;
                                        Inventec.Common.Logging.LogSystem.Debug("Chuyen doi vat tu: theo loai -> theo lo thanh cong____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.MAME_ID), item.MAME_ID) + "___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.MEDICINE_TYPE_NAME), item.MEDICINE_TYPE_NAME) + "___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.TDL_PACKAGE_NUMBER), item.TDL_PACKAGE_NUMBER));
                                    }
                                    else
                                    {
                                        Inventec.Common.Logging.LogSystem.Debug("Chuyen doi hien thi theo lo <-> theo loai: du lieu loai dich vu - MedicineBean1Result (MaterialBean1Result) khong hop le____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item));
                                    }
                                }
                                else
                                {
                                    item.IsAssignPackage = null;
                                }
                            }
                        }

                        if (isRefesh)
                        {
                            this.ProcessMergeDuplicateRowForListProcessingForCheckChangePackage(chkShowLo.Checked);
                            if (CheckMediMatyType(this.mediMatyTypeADOs) == false)
                            {
                                return;
                            }
                            this.RefeshResourceGridMedicine();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPrint_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                isNotLoadWhileChangeControlStateInFirst = true;
                if (chkPrint.Checked)
                    chkPreviewBeforePrint.Checked = !chkPrint.Checked;
                isNotLoadWhileChangeControlStateInFirst = false;
                ChangeCheckPrintAndPreview();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPreviewBeforePrint_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                isNotLoadWhileChangeControlStateInFirst = true;
                if (chkPreviewBeforePrint.Checked)
                    chkPrint.Checked = !chkPreviewBeforePrint.Checked;
                isNotLoadWhileChangeControlStateInFirst = false;

                ChangeCheckPrintAndPreview();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkSign_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.chkSign && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkSignForDPK.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstan.chkSign;
                    csAddOrUpdate.VALUE = (chkSignForDPK.Checked ? "1" : "");
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

        private void chkSignForDDT_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.chkSignForDDT && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkSignForDDT.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstan.chkSignForDDT;
                    csAddOrUpdate.VALUE = (chkSignForDDT.Checked ? "1" : "");
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

        private void chkSignForDTT_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.chkSignForDTT && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkSignForDTT.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstan.chkSignForDTT;
                    csAddOrUpdate.VALUE = (chkSignForDTT.Checked ? "1" : "");
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

        private void ChangeCheckPrintAndPreview()
        {
            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdatePrintDocumentSigned = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.chkPreviewBeforePrint && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdatePrintDocumentSigned != null)
                {
                    csAddOrUpdatePrintDocumentSigned.VALUE = (chkPreviewBeforePrint.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdatePrintDocumentSigned = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdatePrintDocumentSigned.KEY = ControlStateConstan.chkPreviewBeforePrint;
                    csAddOrUpdatePrintDocumentSigned.VALUE = (chkPreviewBeforePrint.Checked ? "1" : "");
                    csAddOrUpdatePrintDocumentSigned.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdatePrintDocumentSigned);
                }

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.chkPrint && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrint.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstan.chkPrint;
                    csAddOrUpdate.VALUE = (chkPrint.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdatePDDT = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.chkPDDT && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdatePDDT != null)
                {
                    csAddOrUpdatePDDT.VALUE = (chkPDDT.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdatePDDT = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdatePDDT.KEY = ControlStateConstan.chkPDDT;
                    csAddOrUpdatePDDT.VALUE = (chkPDDT.Checked ? "1" : "");
                    csAddOrUpdatePDDT.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdatePDDT);
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdatePrintDocumentSigned), csAddOrUpdatePrintDocumentSigned)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));

                this.controlStateWorker.SetData(this.currentControlStateRDO);


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region UCSecondaryIcd
        internal bool ShowPopupIcdChoose()
        {
            try
            {
                WaitingManager.Show();
                frmSecondaryIcd FormSecondaryIcd = new frmSecondaryIcd(stringIcds, this.txtIcdSubCode.Text, this.txtIcdText.Text, (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize, BackendDataWorker.Get<V_HIS_ICD>().Where(o => o.IS_TRADITIONAL != 1 && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList(), currentTreatment);
                WaitingManager.Hide();
                FormSecondaryIcd.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
            return true;
        }

        private void txtIcdSubCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!ProccessorByIcdCode((sender as DevExpress.XtraEditors.TextEdit).Text.Trim()))
                    {
                        e.Handled = true;
                        return;
                    }
                    ReloadIcdSubContainerByCodeChanged();

                    customGridViewSubIcdName.ActiveFilter.Clear();
                    //ShowPopupContainerIcsSub();
                    txtIcdText.Focus();
                    txtIcdText.SelectionStart = txtIcdText.Text.Length;
                    //txtIcdText.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdSubCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    WaitingManager.Show();
                    frmSecondaryIcd FormSecondaryIcd = new frmSecondaryIcd(stringIcds, this.txtIcdSubCode.Text, this.txtIcdText.Text, (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize, BackendDataWorker.Get<V_HIS_ICD>().Where(o => o.IS_TRADITIONAL != 1 && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList(), currentTreatment);

                    WaitingManager.Hide();
                    FormSecondaryIcd.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdText_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //if (!isShow)
                    //{
                    //    this.subIcdPopupSelect = this.customGridViewSubIcdName.GetFocusedRow() as HIS.Desktop.Plugins.ExamServiceReqExecute.ADO.IcdADO;
                    //    if (this.customGridViewSubIcdName.RowCount == 1 && this.subIcdPopupSelect != null && !this.subIcdPopupSelect.IsChecked)
                    //    {
                    //        this.subIcdPopupSelect.IsChecked = true;
                    //        this.customGridControlSubIcdName.RefreshDataSource();
                    //    }
                    isShowContainerMediMaty = false;
                    isShowContainerMediMatyForChoose = true;
                    popupControlContainerSubIcdName.HidePopup();

                    //    SetCheckedSubIcdsToControl();

                    //    txtIcdText.Focus();
                    //    txtIcdText.SelectionStart = txtIcdText.Text.Length;
                    //}
                    //else
                    //{
                    //    isShowContainerMediMaty = false;
                    //    isShowContainerMediMatyForChoose = true;
                    //    popupControlContainerSubIcdName.HidePopup();
                    //}

                    UcIcdNextForcusOut();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    int rowHandlerNext = 0;
                    //int countInGridRows = customGridViewSubIcdName.RowCount;
                    //if (countInGridRows > 1)
                    //{
                    //    rowHandlerNext = 1;
                    //}

                    ShowPopupContainerIcsSub();
                    customGridViewSubIcdName.Focus();
                    customGridViewSubIcdName.FocusedRowHandle = rowHandlerNext;
                }
                else if (e.KeyCode == Keys.Shift || e.KeyCode == Keys.ShiftKey)
                {
                    customGridViewSubIcdName.ActiveFilter.Clear();
                    ShowPopupContainerIcsSub();
                    txtIcdText.Focus();
                    txtIcdText.SelectionStart = txtIcdText.Text.Length;
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdText_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    WaitingManager.Show();
                    frmSecondaryIcd FormSecondaryIcd = new frmSecondaryIcd(stringIcds, this.txtIcdSubCode.Text, this.txtIcdText.Text, (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize, BackendDataWorker.Get<V_HIS_ICD>().Where(o => o.IS_TRADITIONAL != 1 && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList(), currentTreatment);
                    WaitingManager.Hide();
                    FormSecondaryIcd.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void stringIcds(string icdCode, string icdName)
        {
            try
            {
                this.isNotProcessWhileChangedTextSubIcd = true;
                //if (!string.IsNullOrEmpty(icdCode))
                //{
                txtIcdSubCode.Text = icdCode;
                //}
                //if (!string.IsNullOrEmpty(icdName))
                //{
                txtIcdText.Text = icdName;
                //}
                ReloadIcdSubContainerByCodeChanged();
                this.isNotProcessWhileChangedTextSubIcd = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCheckedIcdsToControl(string icdCodes, string icdNames)
        {
            try
            {
                string icdName__Olds = (txtIcdText.Text == txtIcdText.Properties.NullValuePrompt ? "" : txtIcdText.Text);
                txtIcdText.Text = ProcessIcdNameChanged(icdName__Olds, icdNames);
                if (icdNames.Equals(IcdUtil.seperator))
                {
                    txtIcdText.Text = "";
                }
                if (icdCodes.Equals(IcdUtil.seperator))
                {
                    txtIcdSubCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string ProcessIcdNameChanged(string oldIcdNames, string newIcdNames)
        {
            //Thuat toan xu ly khi thay doi lai danh sach icd da chon
            //1. Gan danh sach cac ten icd dang chon vao danh sach ket qua
            //2. Tim kiem trong danh sach icd cu, neu ten icd do dang co trong danh sach moi thi bo qua, neu
            //   Neu icd do khong xuat hien trogn danh sach dang chon & khong tim thay ten do trong danh sach icd hien thi ra
            //   -> icd do da sua doi
            //   -> cong vao chuoi ket qua
            string result = "";
            try
            {
                result = newIcdNames;

                if (!String.IsNullOrEmpty(oldIcdNames))
                {
                    var arrNames = oldIcdNames.Split(new string[] { IcdUtil.seperator }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrNames != null && arrNames.Length > 0)
                    {
                        foreach (var item in arrNames)
                        {
                            if (!String.IsNullOrEmpty(item)
                                && !newIcdNames.Contains(IcdUtil.AddSeperateToKey(item))
                                )
                            {
                                var checkInList = this.currentIcds.Where(o => IcdUtil.AddSeperateToKey(item).Equals(IcdUtil.AddSeperateToKey(o.ICD_NAME))).FirstOrDefault();
                                if (checkInList == null || checkInList.ID == 0)
                                {
                                    result += item + IcdUtil.seperator;
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
            return result;
        }

        private bool CheckIcdWrongCode(ref string strIcdNames, ref string strWrongIcdCodes)
        {
            bool valid = true;
            try
            {
                if (!String.IsNullOrEmpty(this.txtIcdSubCode.Text))
                {
                    strWrongIcdCodes = "";
                    List<string> arrWrongCodes = new List<string>();
                    List<string> lstIcdCodes = new List<string>();
                    List<string> lstIcdSubName = new List<string>();
                    List<string> arrIcdExtraCodes = this.txtIcdSubCode.Text.Split(this.icdSeparators, StringSplitOptions.RemoveEmptyEntries).Where(o => !string.IsNullOrEmpty(o)).Select(o => o.Trim()).Distinct().Where(o => !string.IsNullOrEmpty(o)).ToList();
                    if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
                    {
                        foreach (var itemCode in arrIcdExtraCodes)
                        {
                            var icdByCode = this.currentIcds.FirstOrDefault(o => o.ICD_CODE.ToLower() == itemCode.ToLower());
                            if (icdByCode != null && icdByCode.ID > 0)
                            {
                                string messErr = null;
                                if (!checkIcdManager.ProcessCheckIcd(null, icdByCode.ICD_CODE, ref messErr, false))
                                {
                                    XtraMessageBox.Show(messErr, "Thông báo", MessageBoxButtons.OK);
                                    continue;
                                }
                                strIcdNames += (IcdUtil.seperator + icdByCode.ICD_NAME);
                                lstIcdCodes.Add(icdByCode.ICD_CODE);
                                lstIcdSubName.Add(icdByCode.ICD_NAME);
                            }
                            else
                            {
                                arrWrongCodes.Add(itemCode);
                                strWrongIcdCodes += (IcdUtil.seperator + itemCode);
                            }
                        }
                        strIcdNames += IcdUtil.seperator;
                        isNotProcessWhileChangedTextSubIcd = true;
                        if (lstIcdCodes != null && lstIcdCodes.Count > 0)
                        {
                            this.txtIcdSubCode.Text = String.Join(";", lstIcdCodes);
                            this.txtIcdText.Text = String.Join(";", lstIcdSubName);
                        }
                        else
                        {

                            this.txtIcdSubCode.Text = null;
                            this.txtIcdText.Text = null;
                        }
                        if (!String.IsNullOrEmpty(strWrongIcdCodes))
                        {
                            valid = false;
                            this.SetCheckedIcdsToControl(this.txtIcdSubCode.Text, this.txtIcdText.Text);
                            XtraMessageBox.Show(String.Format(Resources.ResourceMessage.KhongTimThayIcdTuongUngVoiCacMaSau, string.Join(",", arrWrongCodes)), "Thông báo", MessageBoxButtons.OK);
                            ShowPopupIcdChoose();
                        }
                        isNotProcessWhileChangedTextSubIcd = false;
                    }
                }
                else
                {
                    txtIcdText.Text = this.txtIcdSubCode.Text = null;
                    txtIcdText.Focus();
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private bool ProccessorByIcdCode(string currentValue)
        {
            bool valid = true;
            try
            {
                string strIcdNames = "";
                string strWrongIcdCodes = "";
                if (!CheckIcdWrongCode(ref strIcdNames, ref strWrongIcdCodes))
                {
                    valid = false;
                    Inventec.Common.Logging.LogSystem.Warn("Ma icd nhap vao khong ton tai trong danh muc. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => strWrongIcdCodes), strWrongIcdCodes));
                }
                //this.SetCheckedIcdsToControl(this.txtIcdSubCode.Text, strIcdNames);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private void ReloadIcdSubContainerByCodeChanged()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ReloadIcdSubContainerByCodeChanged.1");
                string[] codes = this.txtIcdSubCode.Text.Split(IcdUtil.seperator.ToCharArray());
                this.icdSubcodeAdoChecks = (from m in this.currentIcds.ToList() select new ADO.IcdADO(m, codes)).ToList();
                customGridControlSubIcdName.DataSource = null;
                customGridControlSubIcdName.DataSource = this.icdSubcodeAdoChecks;
                Inventec.Common.Logging.LogSystem.Debug("ReloadIcdSubContainerByCodeChanged.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdText_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isNotProcessWhileChangedTextSubIcd)
                {
                    Inventec.Common.Logging.LogSystem.Debug("txtIcdText_TextChanged____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isNotProcessWhileChangedTextSubIcd), isNotProcessWhileChangedTextSubIcd));
                    return;
                }
                if (!String.IsNullOrEmpty(txtIcdText.Text))
                {
                    string strIcdSubText = "";

                    txtIcdText.Refresh();
                    if (txtIcdText.Text.LastIndexOf(";") > -1)
                    {
                        strIcdSubText = txtIcdText.Text.Substring(txtIcdText.Text.LastIndexOf(";")).Replace(";", "");
                    }
                    else
                        strIcdSubText = txtIcdText.Text;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => strIcdSubText), strIcdSubText)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isShowContainerMediMatyForChoose), isShowContainerMediMatyForChoose)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isShowContainerMediMaty), isShowContainerMediMaty)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isShow), isShow));
                    if (isShowContainerMediMatyForChoose)
                    {
                        customGridViewSubIcdName.ActiveFilter.Clear();
                    }
                    else
                    {
                        if (!isShowContainerMediMaty)
                        {
                            isShowContainerMediMaty = true;
                        }

                        //Filter data
                        customGridViewSubIcdName.ActiveFilterString = String.Format("[ICD_NAME_UNSIGN] Like '%{0}%'", Inventec.Common.String.Convert.UnSignVNese(strIcdSubText).ToLower());
                        customGridViewSubIcdName.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                        customGridViewSubIcdName.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                        customGridViewSubIcdName.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                        customGridViewSubIcdName.FocusedRowHandle = 0;
                        customGridViewSubIcdName.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
                        customGridViewSubIcdName.OptionsFind.HighlightFindResults = true;

                        if (isShow)
                        {
                            ShowPopupContainerIcsSub();
                            isShow = false;
                        }

                        txtIcdText.Focus();
                    }
                    isShowContainerMediMatyForChoose = false;
                }
                else
                {
                    customGridViewSubIcdName.ActiveFilter.Clear();
                    this.subIcdPopupSelect = null;
                    if (!isShowContainerMediMaty)
                    {
                        popupControlContainerSubIcdName.HidePopup();
                    }
                    else
                    {
                        customGridViewSubIcdName.FocusedRowHandle = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdText_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                //if (e.KeyChar == ((char)System.Windows.Forms.Keys.ControlKey | (char)System.Windows.Forms.Keys.A))
                //{
                //    txtIcdText.Focus();
                //    txtIcdText.SelectAll();
                //}
                //else if (e.KeyChar == ((char)System.Windows.Forms.Keys.Delete) || e.KeyChar == ((char)System.Windows.Forms.Keys.Back))
                //{
                //    this.isNotProcessWhileChangedTextSubIcd = true;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowPopupContainerIcsSub()
        {
            try
            {
                popupControlContainerSubIcdName.Width = 600;
                popupControlContainerSubIcdName.Height = 250;

                Rectangle buttonBounds = new Rectangle(panelControlSubIcd.Bounds.X, panelControlSubIcd.Bounds.Y, panelControlSubIcd.Bounds.Width, panelControlSubIcd.Bounds.Height);
                popupControlContainerSubIcdName.ShowPopup(new Point(buttonBounds.X + 300, buttonBounds.Bottom + 15));
                Inventec.Common.Logging.LogSystem.Debug("buttonBounds.X + 300=" + buttonBounds.X + 300 + ", buttonBounds.Bottom + 15=" + (buttonBounds.Bottom + 15));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCheckedSubIcdsToControl()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("SetCheckedSubIcdsToControl.1");
                this.isNotProcessWhileChangedTextSubIcd = true;
                string strIcdSubText = "";
                if (txtIcdText.Text.LastIndexOf(";") > -1)
                {
                    strIcdSubText = txtIcdText.Text.Substring(txtIcdText.Text.LastIndexOf(";")).Replace(";", "");
                }
                else
                    strIcdSubText = txtIcdText.Text;

                string icdNames = null;// IcdUtil.seperator;
                string icdCodes = null;// IcdUtil.seperator;
                string icdName__Olds = txtIcdText.Text;
                var checkList = this.icdSubcodeAdoChecks.Where(o => o.IsChecked == true).ToList();
                int count = 0;
                foreach (var item in checkList)
                {
                    count++;
                    string messErr = null;
                    if (!checkIcdManager.ProcessCheckIcd(null, item.ICD_CODE, ref messErr, false))
                    {
                        XtraMessageBox.Show(messErr, "Thông báo", MessageBoxButtons.OK);
                        item.IsChecked = false;
                        continue;
                    }
                    if (count == checkList.Count)
                    {
                        icdCodes += item.ICD_CODE;
                        icdNames += item.ICD_NAME;
                    }
                    else
                    {
                        icdCodes += item.ICD_CODE + IcdUtil.seperator;
                        icdNames += item.ICD_NAME + IcdUtil.seperator;
                    }
                }

                string newtxtIcdText = ProcessIcdNameChanged(icdName__Olds, icdNames);

                txtIcdText.Text = newtxtIcdText;
                txtIcdSubCode.Text = icdCodes;
                Inventec.Common.Logging.LogSystem.Debug("SetCheckedSubIcdsToControl.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void customGridViewSubIcdName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.subIcdPopupSelect = this.customGridViewSubIcdName.GetFocusedRow() as HIS.Desktop.Plugins.AssignPrescriptionPK.ADO.IcdADO;
                    if (this.subIcdPopupSelect != null)
                    {
                        isShowContainerMediMaty = false;
                        isShowContainerMediMatyForChoose = true;

                        this.subIcdPopupSelect.IsChecked = !this.subIcdPopupSelect.IsChecked;
                        this.customGridControlSubIcdName.RefreshDataSource();

                        SetCheckedSubIcdsToControl();
                        popupControlContainerSubIcdName.HidePopup();

                        txtIcdText.Focus();
                        txtIcdText.SelectionStart = txtIcdText.Text.Length;
                    }
                }
                //if (e.KeyCode == Keys.Space)
                //{
                //    this.subIcdPopupSelect = this.customGridViewSubIcdName.GetFocusedRow() as HIS.Desktop.Plugins.ExamServiceReqExecute.ADO.IcdADO;
                //    if (this.subIcdPopupSelect != null)
                //    {
                //        int currentFocusRow = customGridViewSubIcdName.FocusedRowHandle;

                //        this.subIcdPopupSelect.IsChecked = !this.subIcdPopupSelect.IsChecked;
                //        this.customGridControlSubIcdName.RefreshDataSource();
                //        this.customGridViewSubIcdName.FocusedRowHandle = currentFocusRow;
                //        SetCheckedSubIcdsToControl();
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void customGridControlSubIcdName_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                this.subIcdPopupSelect = this.customGridViewSubIcdName.GetFocusedRow() as HIS.Desktop.Plugins.AssignPrescriptionPK.ADO.IcdADO;
                if (this.subIcdPopupSelect != null)
                {
                    isShowContainerMediMaty = false;
                    isShowContainerMediMatyForChoose = true;

                    this.subIcdPopupSelect.IsChecked = !this.subIcdPopupSelect.IsChecked;
                    this.customGridControlSubIcdName.RefreshDataSource();

                    SetCheckedSubIcdsToControl();
                    popupControlContainerSubIcdName.HidePopup();

                    txtIcdText.Focus();
                    txtIcdText.SelectionStart = txtIcdText.Text.Length;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void customGridViewSubIcdName_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "IsChecked")
                {
                    SetCheckedSubIcdsToControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void customGridViewSubIcdName_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    Inventec.Common.Logging.LogSystem.Debug("customGridViewSubIcdName_MouseDown.1");
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InRowCell)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("customGridViewSubIcdName_MouseDown.2");
                        if (hi.Column.FieldName == "IsChecked" && hi.Column.RealColumnEdit != null && hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            Inventec.Common.Logging.LogSystem.Debug("customGridViewSubIcdName_MouseDown.3");
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {
                                checkEdit.Checked = !checkEdit.Checked;
                                view.CloseEditor();
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                            Inventec.Common.Logging.LogSystem.Debug("customGridViewSubIcdName_MouseDown.4");
                        }
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("customGridViewSubIcdName_MouseDown.5");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void popupControlContainerSubIcdName_CloseUp(object sender, EventArgs e)
        {
            try
            {
                isShow = true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        #endregion

        #region UcIcd
        private void txtIcdCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadIcdCombo(txtIcdCode.Text.ToUpper());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadIcdCombo(string searchCode)
        {
            try
            {
                bool showCbo = true;
                if (!String.IsNullOrEmpty(searchCode))
                {
                    var listData = currentIcds.Where(o => o.ICD_CODE.Contains(searchCode)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == searchCode).ToList() : listData) : null;
                    if (result != null && result.Count > 0)
                    {
                        showCbo = false;
                        txtIcdCode.Text = result.First().ICD_CODE;
                        txtIcdMainText.Text = result.First().ICD_NAME;
                        cboIcds.EditValue = listData.First().ID;
                        chkEditIcd.Checked = (chkEditIcd.Enabled ? this.isAutoCheckIcd : false);
                        if (result.First().IS_LATENT_TUBERCULOSIS == 1)
                            btnLatentTuberCulosis.Enabled = true;
                        else
                            btnLatentTuberCulosis.Enabled = false;
                        string messErr = null;
                        if (!checkIcdManager.ProcessCheckIcd(txtIcdCode.Text.Trim(), txtIcdSubCode.Text.Trim(), ref messErr, false))
                        {
                            XtraMessageBox.Show(messErr, "Thông báo", MessageBoxButtons.OK);
                            if (CheckIcdManager.IcdCodeError.Equals(txtIcdCode.Text))
                            {
                                txtIcdCode.Text = txtIcdMainText.Text = null;
                                cboIcds.EditValue = null;
                            }
                            return;
                        }
                        if (chkEditIcd.Checked)
                        {
                            txtIcdMainText.Focus();
                            txtIcdMainText.SelectAll();
                        }
                        else
                        {
                            cboIcds.Focus();
                            cboIcds.SelectAll();
                        }
                    }
                }

                if (showCbo)
                {
                    cboIcds.Focus();
                    cboIcds.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdMainText_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    chkEditIcd.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcds_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (cboIcds.EditValue != null)
                        this.ChangecboChanDoanTD();
                    else if (this.IsAcceptWordNotInData && this.IsObligatoryTranferMediOrg && !string.IsNullOrEmpty(this._TextIcdName))
                        this.ChangecboChanDoanTD_V2_GanICDNAME(this._TextIcdName);
                    else
                        SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangecboChanDoanTD()
        {
            try
            {
                txtIcdCode.ErrorText = "";
                dxValidationProviderControl.RemoveControlError(txtIcdCode);
                MOS.EFMODEL.DataModels.HIS_ICD icd = currentIcds.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboIcds.EditValue ?? 0).ToString()));
                if (icd != null)
                {
                    txtIcdCode.Text = icd.ICD_CODE;
                    txtIcdMainText.Text = icd.ICD_NAME;
                    chkEditIcd.Checked = (chkEditIcd.Enabled ? this.isAutoCheckIcd : false);
                    string messErr = null;
                    if (!checkIcdManager.ProcessCheckIcd(txtIcdCode.Text.Trim(), txtIcdSubCode.Text.Trim(), ref messErr, false))
                    {
                        XtraMessageBox.Show(messErr, "Thông báo", MessageBoxButtons.OK);
                        if (CheckIcdManager.IcdCodeError.Equals(txtIcdCode.Text))
                        {
                            txtIcdCode.Text = txtIcdMainText.Text = null;
                            cboIcds.EditValue = null;
                        }
                        return;
                    }
                    if (icd.IS_LATENT_TUBERCULOSIS == 1)
                        btnLatentTuberCulosis.Enabled = true;
                    else
                        btnLatentTuberCulosis.Enabled = false;
                    if (chkEditIcd.Checked)
                    {
                        this.NextForcusSubIcd();
                    }
                    else if (chkEditIcd.Enabled)
                    {
                        chkEditIcd.Focus();
                    }
                    else
                    {
                        this.NextForcusSubIcd();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangecboChanDoanTD_V2_GanICDNAME(string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                    return;
                if (HisConfigCFG.AutoCheckIcd != "2")
                {
                    this.chkEditIcd.Enabled = true;
                    this.chkEditIcd.Checked = true;
                }
                this.txtIcdMainText.Text = text;
                this.txtIcdMainText.Focus();
                this.txtIcdMainText.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcds_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control & e.KeyCode == Keys.A)
                {
                    cboIcds.ClosePopup();
                    cboIcds.SelectAll();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    cboIcds.ClosePopup();
                    if (cboIcds.EditValue != null)
                        this.ChangecboChanDoanTD();
                }
                else
                    cboIcds.ShowPopup();
                e.Handled = true;
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

        private void chkEditIcd_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtIcdMainText.Text != null)
                    {
                        //this.data.DelegateRefeshIcdMainText(txtIcdMainText.Text);
                    }
                    if (cboIcds.EditValue != null)
                    {
                        //var hisIcd = BackendDataWorker.Get<HIS_ICD>().Where(p => p.ID == (long)cboIcds.EditValue).FirstOrDefault();
                        //this.data.DelegateRefeshIcd(hisIcd);
                    }
                    NextForcusSubIcd();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdCode_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = Resources.ResourceMessage.IcdKhongDung;
                AutoValidate = AutoValidate.EnableAllowFocusChange;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdCode_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                var search = ((DevExpress.XtraEditors.TextEdit)sender).Text;
                if (!String.IsNullOrEmpty(search))
                {
                    var listData = this.currentIcds.Where(o => o.ICD_CODE.Contains(search)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == search).ToList() : listData) : null;
                    if (result == null || result.Count <= 0)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        txtIcdCode.ErrorText = "";
                        dxValidationProviderControl.RemoveControlError(txtIcdCode);
                        ValidationICD(10, 500, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcds_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    if (!cboIcds.Properties.Buttons[1].Visible)
                        return;
                    this._TextIcdName = "";
                    cboIcds.EditValue = null;
                    txtIcdCode.Text = "";
                    btnLatentTuberCulosis.Enabled = false;
                    txtIcdMainText.Text = "";
                    cboIcds.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcds_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(cboIcds.Text))
                {
                    cboIcds.EditValue = null;
                    txtIcdMainText.Text = "";
                    chkEditIcd.Checked = false;
                }
                else
                {
                    this._TextIcdName = cboIcds.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcds_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("cboIcds_EditValueChanged.1");
                HIS_ICD icd = null;
                if (this.cboIcds.EditValue != null)
                    icd = this.currentIcds.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboIcds.EditValue.ToString()));
                //if (this.isExecuteValueChanged && refeshIcd != null)
                //{
                //    Inventec.Common.Logging.LogSystem.Debug("cboIcds_EditValueChanged.2");
                //    this.refeshIcd(icd);
                //}

                if (icd != null)
                {
                    if (icd != null && icd.IS_REQUIRE_CAUSE == 1)
                    {
                        this.LoadRequiredCause(true);
                    }
                    else
                        this.LoadRequiredCause(false);
                }
                else
                {
                    this.LoadRequiredCause(false);
                }

                Inventec.Common.Logging.LogSystem.Debug("cboIcds_EditValueChanged.3");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region UcIcdCause
        private void txtIcdCodeCause_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadIcdComboCause(txtIcdCodeCause.Text.ToUpper());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadIcdComboCause(string searchCode)
        {
            try
            {
                bool showCbo = true;
                if (!String.IsNullOrEmpty(searchCode))
                {
                    var listData = currentIcds.Where(o => o.ICD_CODE.Contains(searchCode)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == searchCode).ToList() : listData) : null;
                    if (result != null && result.Count > 0)
                    {
                        showCbo = false;
                        txtIcdCodeCause.Text = result.First().ICD_CODE;
                        txtIcdMainTextCause.Text = result.First().ICD_NAME;
                        cboIcdsCause.EditValue = listData.First().ID;
                        chkEditIcdCause.Checked = (chkEditIcdCause.Enabled ? this.isAutoCheckIcd : false);

                        if (chkEditIcdCause.Checked)
                        {
                            txtIcdMainTextCause.Focus();
                            txtIcdMainTextCause.SelectAll();
                        }
                        else
                        {
                            cboIcdsCause.Focus();
                            cboIcdsCause.SelectAll();
                        }
                    }
                }

                if (showCbo)
                {
                    cboIcdsCause.Focus();
                    cboIcdsCause.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdMainTextCause_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    chkEditIcdCause.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcdsCause_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (cboIcdsCause.EditValue != null)
                        this.ChangecboChanDoanTDCause();
                    else if (this.IsAcceptWordNotInData && this.IsObligatoryTranferMediOrg && !string.IsNullOrEmpty(this._TextIcdNameCause))
                        this.ChangecboChanDoanTD_V2_GanICDNAMECause(this._TextIcdNameCause);
                    else
                        SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangecboChanDoanTDCause()
        {
            try
            {
                //cboIcdsCause.Properties.Buttons[1].Visible = true;
                MOS.EFMODEL.DataModels.HIS_ICD icd = currentIcds.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboIcdsCause.EditValue ?? 0).ToString()));
                if (icd != null)
                {
                    txtIcdCodeCause.Text = icd.ICD_CODE;
                    txtIcdMainTextCause.Text = icd.ICD_NAME;
                    chkEditIcdCause.Checked = (chkEditIcdCause.Enabled ? this.isAutoCheckIcd : false);
                    if (chkEditIcdCause.Checked)
                    {
                        this.NextForcusSubIcdCause();
                    }
                    else if (chkEditIcdCause.Enabled)
                    {
                        chkEditIcdCause.Focus();
                    }
                    else
                    {
                        this.NextForcusSubIcdCause();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangecboChanDoanTD_V2_GanICDNAMECause(string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                    return;
                if (HisConfigCFG.AutoCheckIcd != "2")
                {
                    this.chkEditIcdCause.Enabled = true;
                    this.chkEditIcdCause.Checked = true;
                }
                this.txtIcdMainTextCause.Text = text;
                this.txtIcdMainTextCause.Focus();
                this.txtIcdMainTextCause.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcdsCause_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control & e.KeyCode == Keys.A)
                {
                    cboIcdsCause.ClosePopup();
                    cboIcdsCause.SelectAll();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    cboIcdsCause.ClosePopup();
                    if (cboIcdsCause.EditValue != null)
                        this.ChangecboChanDoanTDCause();
                }
                else
                    cboIcdsCause.ShowPopup();
                e.Handled = true;
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

        private void chkEditIcdCause_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtIcdMainTextCause.Text != null)
                    {
                        //this.data.DelegateRefeshIcdMainText(txtIcdMainText.Text);
                    }
                    if (cboIcdsCause.EditValue != null)
                    {
                        //var hisIcd = BackendDataWorker.Get<HIS_ICD>().Where(p => p.ID == (long)cboIcds.EditValue).FirstOrDefault();
                        //this.data.DelegateRefeshIcd(hisIcd);
                    }
                    NextForcusSubIcdCause();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdCodeCause_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = Resources.ResourceMessage.IcdKhongDung;
                AutoValidate = AutoValidate.EnableAllowFocusChange;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdCodeCause_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                var search = ((DevExpress.XtraEditors.TextEdit)sender).Text;
                if (!String.IsNullOrEmpty(search))
                {
                    var listData = this.currentIcds.Where(o => o.ICD_CODE.Contains(search)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == search).ToList() : listData) : null;
                    if (result == null || result.Count <= 0)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        txtIcdCodeCause.ErrorText = "";
                        dxValidationProviderControl.RemoveControlError(txtIcdCodeCause);
                        ValidationICDCause(10, 500, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcdsCause_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    if (!cboIcdsCause.Properties.Buttons[1].Visible)
                        return;
                    this._TextIcdNameCause = "";
                    cboIcdsCause.EditValue = null;
                    txtIcdCodeCause.Text = "";
                    txtIcdMainTextCause.Text = "";
                    cboIcdsCause.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcdsCause_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(cboIcdsCause.Text))
                {
                    cboIcdsCause.EditValue = null;
                    txtIcdMainTextCause.Text = "";
                    chkEditIcdCause.Checked = false;
                }
                else
                {
                    this._TextIcdNameCause = cboIcdsCause.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region UcDate
        private void dtInstructionTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //Thay đổi ngày chỉ định, phải load lại đối tượng thanh toán của BN tương ứng với ngày đó
                if (!this.isNotLoadWhileChangeInstructionTimeInFirst)
                {
                    this.ChangeIntructionTimeEditor(this.dtInstructionTime.DateTime);

                    if (HisConfigCFG.IsCheckDepartmentInTimeWhenPresOrAssign && currentWorkPlace.RoomTypeId == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG)
                    {
                        CheckTimeInDepartment();
                    }
                }
                EnableCheckTemporaryPres();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void EnableCheckTemporaryPres()
        {
            try
            {
                if (InstructionTime > Int64.Parse(DateTime.Now.ToString("yyyyMMddHHmm00")))
                {
                    chkTemporayPres.Enabled = true;
                }
                else
                {
                    chkTemporayPres.Enabled = false;
                    chkTemporayPres.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtInstructionTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    timeIntruction.Focus();
                    timeIntruction.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtInstructionTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    timeIntruction.Focus();
                    timeIntruction.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtInstructionTime_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Glyph)
                {
                    frmMultiIntructonTime frmChooseIntructionTime = new frmMultiIntructonTime(intructionTimeSelected, timeSelested, SelectMultiIntructionTime);
                    frmChooseIntructionTime.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkMultiIntructionTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //NextForcusUCDate();
                    dtUseTime.Focus();
                    dtUseTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkMultiIntructionTime_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isStopEventChangeMultiDate)
                {
                    return;
                }

                this.txtInstructionTime.Visible = this.chkMultiIntructionTime.Checked;
                this.dtInstructionTime.Visible = !this.chkMultiIntructionTime.Checked;

                if (this.chkMultiIntructionTime.Checked)
                {
                    this.timeIntruction.EditValue = DateTime.Now.ToString("HH:mm");
                    string strTimeDisplay = DateTime.Now.ToString("dd/MM");
                    this.txtInstructionTime.Text = strTimeDisplay;
                    this.licUseTime.Enabled = false;
                    if (!GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                    {
                        rdOpionGroup.Properties.Items[0].Enabled = false;
                        rdOpionGroup.SelectedIndex = 1;
                        rdOpionGroup.Properties.Items[2].Enabled = false;
                    }
                }
                else
                {
                    this.licUseTime.Enabled = true;
                    if (!GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                    {
                        rdOpionGroup.Properties.Items[0].Enabled = true;
                        rdOpionGroup.Properties.Items[2].Enabled = true;
                    }
                    this.ChangeIntructionTimeEditor(this.dtInstructionTime.DateTime);
                }
                this.UseTimeSelecteds = new List<long>();
                this.UseTimeSelected = new List<DateTime?>();
                this.txtUseTime.Text = "";
                this.dtUseTime.EditValue = null;
                this.DelegateMultiDateChanged();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timeIntruction_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.chkMultiIntructionTime.Enabled || lcichkMultiDate.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    {
                        this.chkMultiIntructionTime.Focus();
                    }
                    else
                    {
                        this.NextForcusUCDate();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timeIntruction_Leave(object sender, EventArgs e)
        {
            try
            {
                //Thay đổi ngày chỉ định, phải load lại đối tượng thanh toán của BN tương ứng với ngày đó
                if (!this.isNotLoadWhileChangeInstructionTimeInFirst)
                {
                    var InstructionTimeOld = this.InstructionTime;
                    this.ChangeIntructionTimeEditor(this.dtInstructionTime.DateTime);

                    if (HisConfigCFG.IsCheckDepartmentInTimeWhenPresOrAssign && currentWorkPlace.RoomTypeId == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG)
                    {
                        long timeNew = Inventec.Common.TypeConvert.Parse.ToInt64(this.dtInstructionTime.DateTime.ToString("yyyyMMdd") + this.timeSelested.ToString("HHmm") + "00");
                        if (InstructionTimeOld != timeNew)
                        {
                            CheckTimeInDepartment();
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

        #region UcDateForMedi
        private void dtInstructionTimeForMedi_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //Thay đổi ngày chỉ định, phải load lại đối tượng thanh toán của BN tương ứng với ngày đó
                if (!this.isNotLoadWhileChangeInstructionTimeInFirst)
                {
                    this.ChangeIntructionTimeForMedi(this.dtInstructionTimeForMedi.DateTime);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtInstructionTimeForMedi_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    timeIntructionForMedi.Focus();
                    timeIntructionForMedi.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtInstructionTimeForMedi_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    timeIntructionForMedi.Focus();
                    timeIntructionForMedi.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtInstructionTimeForMedi_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Glyph)
                {
                    frmMultiIntructonTime frmChooseIntructionTime = new frmMultiIntructonTime(intructionTimeSelectedForMedi, timeSelestedForMedi, SelectMultiIntructionTimeForMedi);
                    frmChooseIntructionTime.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkMultiIntructionTimeForMedi_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    NextForcusUCDateForMedi();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkMultiIntructionTimeForMedi_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isStopEventChangeMultiDate)
                {
                    return;
                }

                this.txtInstructionTimeForMedi.Visible = this.chkMultiIntructionTimeForMedi.Checked;
                this.dtInstructionTimeForMedi.Visible = !this.chkMultiIntructionTimeForMedi.Checked;

                if (this.chkMultiIntructionTimeForMedi.Checked)
                {
                    this.timeIntructionForMedi.EditValue = DateTime.Now.ToString("HH:mm");
                    string strTimeDisplay = DateTime.Now.ToString("dd/MM");
                    this.txtInstructionTimeForMedi.Text = strTimeDisplay;
                }
                //this.DelegateMultiDateChanged();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        void ServiceReqEyeUpdateInfo(HIS_SERVICE_REQ serviceReqEye)
        {
            this.ServiceReqEye = serviceReqEye;
            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ServiceReqEye), ServiceReqEye));
        }

        /// <summary>
        /// Sửa chức năng "Kê đơn" (kê đơn phòng khám, kê đơn tủ trực, kê đơn điều trị):
        ///Bổ sung tính năng, lấy lời dặn theo các thuốc đã kê, cụ thể:
        ///- Ở textbox "Lời dặn", bổ sung textholder như hình đính kèm (ke_don.png): "F6 để lấy lời dặn từ đơn đã kê. (F7 để thiết lập điều kiện lấy lời dặn)
        ///- Khi người dùng nhấn nút "F7" thì mở ra popup "Thiết lập điều kiện lấy lời dặn":
        ///+ Giao diện như hình đính kèm
        ///+ Trạng thái của các control (checkbox và combobox), mặc định hiển thị theo lần chọn trước đó
        ///+ Combobox "Đường dùng" hiển thị dữ liệu từ danh mục "Đường dùng" - chỉ lấy các bản ghi chưa khóa (HIS_MEDICINE_USE_FORM có IS_ACTIVE = 1)
        ///+ Combobox "Loại đơn", hiển thị theo danh mục "Loại xuất" (his_exp_mest_type), và chỉ hiển thị các loại xuất: "Đơn tủ trực", "Đơn phòng khám", "Đơn điều trị" (có ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT, IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DPK, IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT)

        ///- Khi người dùng nhấn F6:
        ///+ Kiểm tra xem đã có thông tin điều kiện lấy lời dặn chưa (có check 1 trong 2 checkbox "Thuốc", "Vật tư" trong chức năng thiết lập điều kiện), nếu đã có, thực hiện truy vấn lấy các lời dặn của các thuốc đã kê cho BN, theo điều kiện đã nhập, và tự động điền vào textbox "Lời dặn" theo định dạng:
        ///"Dùng tiếp thuốc cũ:
        ///Thuốc số 1 - HDSD thuốc số 1
        ///Thuốc số 2 - HDSD thuốc số 2"
        ///(để đảm bảo hiệu năng, server sẽ cung cấp API riêng)

        ///+ Nếu chưa có thông tin điều kiện lấy lời dặn, thì mở ra popup "Thiết lập điều kiện lấy lời dặn", sau khi người dùng thiết lập, nhấn nút "Lưu" thì tự động xử lý lấy thông tin lời dặn như trên.

        ///- Khi người dùng check vào "Đơn mang về":
        ///+ Kiểm tra, nếu thông tin thiết lập có check "Tự động lấy với đơn mang về" thì xử lý tiếp tương tự như khi người dùng nhấn F6 (mô tả ở trên)
        ///+ Nếu không check "Tự động lấy với đơn mang về" thì xử lý như cũ.

        ///- Sửa lại các phím tắt F5, F6, F7:
        ///Cho phép người dùng không focus vào textbox "Lời dặn" vẫn có thể nhấn các phím tắt này
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bbtnF5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                WaitingManager.Show();

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TextLibrary").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TextLibrary'");
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.TextLibrary' is not plugins");

                List<object> listArgs = new List<object>();
                listArgs.Add("loidanbacsi");
                listArgs.Add(this.currentModule);
                listArgs.Add((HIS.Desktop.Common.DelegateDataTextLib)ProcessDataTextLib);
                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(moduleData, listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                WaitingManager.Hide();
                ((Form)extenceInstance).ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnF6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //Lấy lời dặn từ đơn đã kê
                if (currentAdviseFormADO != null)
                {
                    SelectAdviseForm(currentAdviseFormADO);
                }
                else
                {
                    frmAdvise frmAdvise = new frmAdvise(currentAdviseFormADO, SelectAdviseForm);
                    frmAdvise.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnF7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //Thiết lập điều kiện lấy lời dặn
                frmAdvise frmAdvise = new frmAdvise(currentAdviseFormADO, SelectAdviseForm);
                frmAdvise.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SelectAdviseForm(AdviseFormADO adviseForm)
        {
            try
            {
                WaitingManager.Show();
                this.currentAdviseFormADO = adviseForm;
                this.SaveStateAdviseFormData();

                CommonParam paramCommon = new CommonParam();
                HisExpMestTutorialFilter filter = new MOS.Filter.HisExpMestTutorialFilter();
                filter.TREATMENT_ID = this.treatmentId;
                filter.INCLUDE_MATERIAL = this.currentAdviseFormADO.IncludeMaterial.HasValue && this.currentAdviseFormADO.IncludeMaterial.Value;
                if (this.currentAdviseFormADO.MedicineUseFormIds != null && this.currentAdviseFormADO.MedicineUseFormIds.Count > 0)
                    filter.MEDICINE_USE_FORM_IDs = this.currentAdviseFormADO.MedicineUseFormIds;
                if (this.currentAdviseFormADO.ExpMestTypeIds != null && this.currentAdviseFormADO.ExpMestTypeIds.Count > 0)
                    filter.EXP_MEST_TYPE_IDs = this.currentAdviseFormADO.ExpMestTypeIds;
                List<ExpMestTutorialSDO> rsData = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<ExpMestTutorialSDO>>("api/HisExpMest/GetExpMestTutorial", ApiConsumers.MosConsumer, filter, paramCommon);
                Inventec.Common.Logging.LogSystem.Debug("SelectAdviseForm____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rsData), rsData));
                //"Dùng tiếp thuốc cũ:
                //Thuốc số 1 - HDSD thuốc số 1
                //Thuốc số 2 - HDSD thuốc số 2"
                this.currentAdviseFormADO.AdviseResult = "";//TODO
                if (rsData != null && rsData.Count > 0)
                {
                    this.currentAdviseFormADO.AdviseResult = "Dùng tiếp thuốc cũ:\r\n";
                    foreach (var item in rsData)
                    {
                        this.currentAdviseFormADO.AdviseResult += String.Format("{0} - {1}\r\n", item.ItemName, item.Tutorial);
                    }
                }
                this.txtAdvise.Text = this.currentAdviseFormADO.AdviseResult;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SaveStateAdviseFormData()
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst || this.currentAdviseFormADO == null)
                {
                    return;
                }

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.AdviseFormData && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = String.Format("{0}|{1}|{2}|{3}", ((this.currentAdviseFormADO.IncludeMaterial != null && this.currentAdviseFormADO.IncludeMaterial.Value) ? "1" : "0"), ((this.currentAdviseFormADO.MedicineUseFormIds != null && this.currentAdviseFormADO.MedicineUseFormIds.Count > 0) ? String.Join(",", this.currentAdviseFormADO.MedicineUseFormIds) : ""), ((this.currentAdviseFormADO.ExpMestTypeIds != null && this.currentAdviseFormADO.ExpMestTypeIds.Count > 0) ? String.Join(",", this.currentAdviseFormADO.ExpMestTypeIds) : ""), ((this.currentAdviseFormADO.AutoGetHomePres != null && this.currentAdviseFormADO.AutoGetHomePres.Value) ? "1" : "0"));
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstan.AdviseFormData;
                    csAddOrUpdate.VALUE = String.Format("{0}|{1}|{2}|{3}", ((this.currentAdviseFormADO.IncludeMaterial != null && this.currentAdviseFormADO.IncludeMaterial.Value) ? "1" : "0"), ((this.currentAdviseFormADO.MedicineUseFormIds != null && this.currentAdviseFormADO.MedicineUseFormIds.Count > 0) ? String.Join(",", this.currentAdviseFormADO.MedicineUseFormIds) : ""), ((this.currentAdviseFormADO.ExpMestTypeIds != null && this.currentAdviseFormADO.ExpMestTypeIds.Count > 0) ? String.Join(",", this.currentAdviseFormADO.ExpMestTypeIds) : ""), ((this.currentAdviseFormADO.AutoGetHomePres != null && this.currentAdviseFormADO.AutoGetHomePres.Value) ? "1" : "0"));
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                bool success = this.controlStateWorker.SetData(this.currentControlStateRDO);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentAdviseFormADO), currentAdviseFormADO)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => success), success));
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveStateNotShowExpMestTypeDTTCheckedChanged(bool isNotShowExpMestTypeDTT)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("SaveStateNotShowExpMestTypeDTTCheckedChanged.1");
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                this.chkNotShowExpMestTypeDTT = isNotShowExpMestTypeDTT;
                Inventec.Common.Logging.LogSystem.Debug("SaveStateNotShowExpMestTypeDTTCheckedChanged.2");
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.chkNotShowExpMestTypeDTT && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (isNotShowExpMestTypeDTT ? "1" : "0");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstan.chkNotShowExpMestTypeDTT;
                    csAddOrUpdate.VALUE = (isNotShowExpMestTypeDTT ? "1" : "0");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                bool success = this.controlStateWorker.SetData(this.currentControlStateRDO);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => success), success));
                Inventec.Common.Logging.LogSystem.Debug("SaveStateNotShowExpMestTypeDTTCheckedChanged.3");
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void navBarControlChongChiDinhInfo_NavPaneStateChanged(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("SaveStatelcgChongChiDinhInfoExpandChanged.0");
                Inventec.Common.Logging.LogSystem.Debug("SaveStatelcgChongChiDinhInfoExpandChanged.1");
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                this.navBarChongChiDinhInfoState = !this.navBarChongChiDinhInfoState;
                Inventec.Common.Logging.LogSystem.Debug("SaveStatelcgChongChiDinhInfoExpandChanged.2");
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.lcgChongChiDinhInfo && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (this.navBarChongChiDinhInfoState ? "1" : "0");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstan.lcgChongChiDinhInfo;
                    csAddOrUpdate.VALUE = (this.navBarChongChiDinhInfoState ? "1" : "0");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                bool success = this.controlStateWorker.SetData(this.currentControlStateRDO);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => success), success));
                Inventec.Common.Logging.LogSystem.Debug("SaveStatelcgChongChiDinhInfoExpandChanged.3");

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCreateVBA_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnCreateVBA.Enabled)
                {
                    LogTheadInSessionInfo(CreateEMRVBAOnClick, !GlobalStore.IsCabinet ? "CreateMedicalRecordsFromPrescription" : "CreateMedicalRecordsFromMedicalStore");
                    //CreateEMRVBAOnClick();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPreviousUseDay_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                this.SpinKeyPressNoSeperate(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPreviousUseDay_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //this.FocusShowpopup(this.cboHtu, true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region DHST
        private void navBarControlDHST_NavPaneStateChanged(object sender, EventArgs e)
        {

        }

        private void navBarControlDHST_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("SaveStatelcgDHSTInfoExpandChanged.0");
                Inventec.Common.Logging.LogSystem.Debug("SaveStatelcgDHSTInfoExpandChanged.1");
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                this.navBarDHSTInfoState = !this.navBarDHSTInfoState;
                Inventec.Common.Logging.LogSystem.Debug("SaveStatelcgDHSTInfoExpandChanged.2");
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.lcgDHSTInfo && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (this.navBarDHSTInfoState ? "1" : "0");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstan.lcgDHSTInfo;
                    csAddOrUpdate.VALUE = (this.navBarDHSTInfoState ? "1" : "0");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                bool success = this.controlStateWorker.SetData(this.currentControlStateRDO);
                Inventec.Common.Logging.LogSystem.Debug("SaveStatelcgDHSTInfoExpandChanged.3");

                if (!this.navBarDHSTInfoState && this.layoutControlItemDHSTInfo.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    this.layoutControlItemDHSTInfo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem15.Size = new Size(layoutControlItem15.Width, layoutControlItem15.Height + layoutControlItemDHSTInfo.Height);
                }
                else if (this.navBarDHSTInfoState && this.layoutControlItemDHSTInfo.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                {
                    this.layoutControlItemDHSTInfo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem15.Size = new Size(layoutControlItem15.Width, layoutControlItem15.Height - layoutControlItemDHSTInfo.Height);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinHeight_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                DHSTFillDataToBmiAndLeatherArea();
                LoadMLCT();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinWeight_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                DHSTFillDataToBmiAndLeatherArea();
                LoadMLCT();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataDhstToControl()
        {
            try
            {
                if (this.currentTreatmentWithPatientType != null)
                {
                    //WaitingManager.Show();
                    HisDhstFilter dhstFilter = new HisDhstFilter();
                    dhstFilter.TREATMENT_ID = this.currentTreatmentWithPatientType.ID;
                    dhstFilter.ORDER_FIELD = "EXECUTE_TIME";
                    dhstFilter.ORDER_DIRECTION = "DESC";
                    CommonParam param = new CommonParam();
                    var listDHST = await new BackendAdapter(param)
                .GetAsync<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDHST/Get", ApiConsumers.MosConsumer, dhstFilter, param);
                    if (listDHST != null && listDHST.Count > 0)
                    {
                        listDHST = listDHST.OrderByDescending(o => o.EXECUTE_TIME).ThenByDescending(o => o.ID).ToList();
                        var firstDhst = listDHST[0];
                        if (firstDhst.WEIGHT.HasValue && firstDhst.HEIGHT.HasValue && firstDhst.TEMPERATURE.HasValue && firstDhst.BREATH_RATE.HasValue && firstDhst.CHEST.HasValue && firstDhst.BELLY.HasValue && firstDhst.BLOOD_PRESSURE_MAX.HasValue && firstDhst.BLOOD_PRESSURE_MIN.HasValue && firstDhst.PULSE.HasValue && firstDhst.SPO2.HasValue)
                        {
                            dhst = firstDhst;
                        }
                        else
                        {
                            dhst = firstDhst;
                            foreach (var item in listDHST)
                            {
                                if (dhst != null && dhst.WEIGHT.HasValue && !dhst.HEIGHT.HasValue && item.HEIGHT.HasValue)
                                {
                                    dhst.HEIGHT = item.HEIGHT;
                                }
                                else if (dhst != null && dhst.HEIGHT.HasValue && !dhst.WEIGHT.HasValue && item.WEIGHT.HasValue)
                                {
                                    dhst.WEIGHT = item.WEIGHT;
                                }
                                else if (dhst != null && !dhst.HEIGHT.HasValue && !dhst.WEIGHT.HasValue)
                                {
                                    dhst.WEIGHT = item.WEIGHT;
                                    dhst.HEIGHT = item.HEIGHT;
                                }
                                MapInformationDhstEmpty(item);
                            }
                        }
                    }
                    else
                        dhst = null;
                    //WaitingManager.Hide();
                    this.DHSTSetValue(dhst);
                    Inventec.Common.Logging.LogSystem.Debug("Get dhst from treatment");
                }

                //việc 20260
                if (HisConfigCFG.IsCheckDepartmentInTimeWhenPresOrAssign)
                {
                    //WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    HisDepartmentTranFilter TranFilter = new HisDepartmentTranFilter();
                    TranFilter.TREATMENT_ID = this.treatmentId;
                    TranFilter.ORDER_DIRECTION = "ASC";
                    TranFilter.ORDER_FIELD = "DEPARTMENT_IN_TIME";

                    this.lstDepartmentTran = await new BackendAdapter(param).GetAsync<List<HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/Get", ApiConsumers.MosConsumer, TranFilter, param);

                    HisCoTreatmentFilter CoTreatmentFilter = new HisCoTreatmentFilter();
                    CoTreatmentFilter.TDL_TREATMENT_ID = this.treatmentId;
                    CoTreatmentFilter.ORDER_DIRECTION = "ASC";
                    CoTreatmentFilter.ORDER_FIELD = "START_TIME";

                    this.lstCoTreatment = await new BackendAdapter(param).GetAsync<List<HIS_CO_TREATMENT>>("api/HisCoTreatment/Get", ApiConsumers.MosConsumer, CoTreatmentFilter, param);

                    //WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                //WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void MapInformationDhstEmpty(HIS_DHST item)
        {
            try
            {
                if (!dhst.TEMPERATURE.HasValue && item.TEMPERATURE.HasValue)
                    dhst.TEMPERATURE = item.TEMPERATURE;
                if (!dhst.BREATH_RATE.HasValue && item.BREATH_RATE.HasValue)
                    dhst.BREATH_RATE = item.BREATH_RATE;
                if (!dhst.CHEST.HasValue && item.CHEST.HasValue)
                    dhst.CHEST = item.CHEST;
                if (!dhst.BELLY.HasValue && item.BELLY.HasValue)
                    dhst.BELLY = item.BELLY;
                if (!dhst.BLOOD_PRESSURE_MAX.HasValue && item.BLOOD_PRESSURE_MAX.HasValue)
                    dhst.BLOOD_PRESSURE_MAX = item.BLOOD_PRESSURE_MAX;
                if (!dhst.BLOOD_PRESSURE_MIN.HasValue && item.BLOOD_PRESSURE_MIN.HasValue)
                    dhst.BLOOD_PRESSURE_MIN = item.BLOOD_PRESSURE_MIN;
                if (!dhst.PULSE.HasValue && item.PULSE.HasValue)
                    dhst.PULSE = item.PULSE;
                if (!dhst.SPO2.HasValue && item.SPO2.HasValue)
                    dhst.SPO2 = item.SPO2;
                if (!dhst.URINE.HasValue && item.URINE.HasValue)
                    dhst.URINE = item.URINE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        //việc 20260
        private bool CheckTimeInDepartment()
        {
            bool result = true;
            try
            {
                List<HIS_DEPARTMENT_TRAN> curremtTrans = null;
                if (this.lstDepartmentTran != null && this.lstDepartmentTran.Count > 0)
                {
                    curremtTrans = this.lstDepartmentTran.Where(o => o.DEPARTMENT_ID == currentWorkPlace.DepartmentId && o.DEPARTMENT_IN_TIME.HasValue).ToList();
                }

                List<HIS_CO_TREATMENT> currentCo = null;
                if (this.lstCoTreatment != null && this.lstCoTreatment.Count > 0)
                {
                    currentCo = this.lstCoTreatment.Where(o => o.DEPARTMENT_ID == currentWorkPlace.DepartmentId && o.START_TIME.HasValue).ToList();
                }

                foreach (var intructionTime in this.intructionTimeSelecteds)
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
                            HIS_DEPARTMENT_TRAN nextTran = this.lstDepartmentTran.FirstOrDefault(o => o.PREVIOUS_ID == item.ID);
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
                        XtraMessageBox.Show(string.Format(ResourceMessage.ThoiGianYLenhKhongNamTrongThoiGianBNHienDienTaiKhoa, string.Join(", ", times)));
                        this.dtInstructionTime.Focus();
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

        private void btnConnectBloodPressure_Click(object sender, EventArgs e)
        {
            try
            {
                HIS_DHST data = HIS.Desktop.Plugins.Library.ConnectBloodPressure.ConnectBloodPressureProcessor.GetData();
                if (data != null)
                {
                    if (data.EXECUTE_TIME != null)
                        dtExecuteTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.EXECUTE_TIME ?? 0) ?? DateTime.Now;
                    else
                        dtExecuteTime.EditValue = DateTime.Now;

                    if (data.BLOOD_PRESSURE_MAX.HasValue)
                    {
                        spinBloodPressureMax.EditValue = data.BLOOD_PRESSURE_MAX;
                    }
                    if (data.BLOOD_PRESSURE_MIN.HasValue)
                    {
                        spinBloodPressureMin.EditValue = data.BLOOD_PRESSURE_MIN;
                    }
                    if (data.BREATH_RATE.HasValue)
                    {
                        spinBreathRate.EditValue = data.BREATH_RATE;
                    }
                    if (data.HEIGHT.HasValue)
                    {
                        spinHeight.EditValue = data.HEIGHT;
                    }
                    if (data.CHEST.HasValue)
                    {
                        spinChest.EditValue = data.CHEST;
                    }
                    if (data.BELLY.HasValue)
                    {
                        spinBelly.EditValue = data.BELLY;
                    }
                    if (data.PULSE.HasValue)
                    {
                        spinPulse.EditValue = data.PULSE;
                    }
                    if (data.TEMPERATURE.HasValue)
                    {
                        spinTemperature.EditValue = data.TEMPERATURE;
                    }
                    if (data.WEIGHT.HasValue)
                    {
                        spinWeight.EditValue = data.WEIGHT;
                    }
                    if (!String.IsNullOrWhiteSpace(data.NOTE))
                    {
                        txtNote.Text = data.NOTE;
                    }
                    if (data.SPO2.HasValue)
                        spinSPO2.Value = (data.SPO2.Value * 100);
                    else
                        spinSPO2.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DHSTSetValue(HIS_DHST dhst)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("DHSTSetValue.1");
                if (dhst != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("DHSTSetValue.2");
                    if (dhst.EXECUTE_TIME != null)
                        dtExecuteTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dhst.EXECUTE_TIME ?? 0) ?? DateTime.Now;
                    else
                        dtExecuteTime.EditValue = DateTime.Now;
                    spinBloodPressureMax.EditValue = dhst.BLOOD_PRESSURE_MAX;
                    spinBloodPressureMin.EditValue = dhst.BLOOD_PRESSURE_MIN;
                    spinBreathRate.EditValue = dhst.BREATH_RATE;
                    spinHeight.EditValue = dhst.HEIGHT;
                    spinChest.EditValue = dhst.CHEST;
                    spinBelly.EditValue = dhst.BELLY;
                    spinPulse.EditValue = dhst.PULSE;
                    spinTemperature.EditValue = dhst.TEMPERATURE;
                    spinWeight.EditValue = dhst.WEIGHT;
                    if (dhst.SPO2.HasValue)
                        spinSPO2.Value = (dhst.SPO2.Value * 100);
                    else
                        spinSPO2.EditValue = null;
                    txtNote.Text = dhst.NOTE;

                    LoadMLCT();
                }
                Inventec.Common.Logging.LogSystem.Debug("DHSTSetValue.3");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<V_HIS_SERE_SERV_TEIN_1> SereServTeinData { get; set; }
        public bool IsCellChangeAmount { get; private set; }

        decimal chisotestIndexCreatinin;
        private void LoadMLCT()
        {
            try
            {
                string strIsToCalculateEgfr = "";
                if (spinWeight.EditValue == null)
                {
                    strIsToCalculateEgfr = "";
                }
                else
                {
                    List<long> ACRPCRList = new List<long>() { IMSys.DbConfig.HIS_RS.TEST_INDEX_TYPE.ALBUMIN_NIEU, IMSys.DbConfig.HIS_RS.TEST_INDEX_TYPE.PROTEIN_NIEU, IMSys.DbConfig.HIS_RS.TEST_INDEX_TYPE.CREATININ_NIEU };
                    var TestIndexData = BackendDataWorker.Get<HIS_TEST_INDEX>().ToList();
                    if (TestIndexData != null && TestIndexData.Count > 0)
                    {
                        if (SereServTeinData == null || SereServTeinData.Count == 0 || !SereServTeinData.Exists(o => o.TDL_TREATMENT_ID == treatmentId))
                        {
                            CommonParam param = new CommonParam();
                            HisSereServTeinView1Filter filter = new HisSereServTeinView1Filter();
                            filter.TREATMENT_IDs = new List<long>() { treatmentId };
                            filter.TEST_INDEX_IDs = TestIndexData.Where(p => ACRPCRList.Exists(o => o == p.TEST_INDEX_TYPE) || p.IS_TO_CALCULATE_EGFR == 1).Select(o => o.ID).ToList();
                            SereServTeinData = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_TEIN_1>>("/api/HisSereServTein/GetView1", ApiConsumers.MosConsumer, filter, param);
                            if (SereServTeinData != null && SereServTeinData.Count > 0)
                            {
                                var SereServTestType = SereServTeinData.Where(p => ACRPCRList.Exists(o => o == p.TEST_INDEX_TYPE)).ToList();
                                if (SereServTestType.Exists(o => o.TEST_INDEX_TYPE == IMSys.DbConfig.HIS_RS.TEST_INDEX_TYPE.CREATININ_NIEU && !string.IsNullOrEmpty(o.VALUE)) && SereServTestType.Exists(o => (o.TEST_INDEX_TYPE == IMSys.DbConfig.HIS_RS.TEST_INDEX_TYPE.ALBUMIN_NIEU || o.TEST_INDEX_TYPE == IMSys.DbConfig.HIS_RS.TEST_INDEX_TYPE.PROTEIN_NIEU) && !string.IsNullOrEmpty(o.VALUE)))
                                {
                                    var ListNotNullvalue = SereServTestType.Where(o => (o.TEST_INDEX_TYPE == IMSys.DbConfig.HIS_RS.TEST_INDEX_TYPE.ALBUMIN_NIEU || o.TEST_INDEX_TYPE == IMSys.DbConfig.HIS_RS.TEST_INDEX_TYPE.PROTEIN_NIEU) && !string.IsNullOrEmpty(o.VALUE)).OrderByDescending(o => o.MODIFY_TIME).ThenBy(o => o.TEST_INDEX_TYPE).ToList().FirstOrDefault();
                                    if (ListNotNullvalue != null)
                                    {
                                        var testIndex = TestIndexData.FirstOrDefault(o => o.ID == (ListNotNullvalue.TEST_INDEX_ID ?? 0));
                                        decimal chiso;
                                        string ssTeinVL = ListNotNullvalue.VALUE.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                                         .Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ssTeinVL), ssTeinVL));
                                        if (Decimal.TryParse(ssTeinVL, out chiso))
                                        {
                                            if (testIndex.CONVERT_RATIO_TYPE.HasValue)
                                                chiso *= (testIndex.CONVERT_RATIO_TYPE ?? 0);
                                            var Creatinin = SereServTestType.Where(o => o.TEST_INDEX_TYPE == IMSys.DbConfig.HIS_RS.TEST_INDEX_TYPE.CREATININ_NIEU && !string.IsNullOrEmpty(o.VALUE)).OrderByDescending(o => o.MODIFY_TIME).ToList().FirstOrDefault();
                                            var testIndexCreatinin = TestIndexData.FirstOrDefault(o => o.ID == (Creatinin.TEST_INDEX_ID ?? 0));
                                            string ssTeintestIndexCreatininVL = Creatinin.VALUE.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                                             .Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ssTeintestIndexCreatininVL), ssTeintestIndexCreatininVL));
                                            if (Decimal.TryParse(ssTeintestIndexCreatininVL, out chisotestIndexCreatinin))
                                            {
                                                if (testIndexCreatinin.CONVERT_RATIO_TYPE.HasValue)
                                                    chisotestIndexCreatinin *= (testIndexCreatinin.CONVERT_RATIO_TYPE ?? 0);
                                                lciARCPCR.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                                                lciARCPCR.Text = ListNotNullvalue.TEST_INDEX_TYPE == IMSys.DbConfig.HIS_RS.TEST_INDEX_TYPE.ALBUMIN_NIEU ? "uACR:" : "uPCR:";
                                                lblACRPCR.Text = (chiso / chisotestIndexCreatinin).ToString();
                                            }
                                        }
                                    }

                                }
                            }
                        }
                        TestIndexData = TestIndexData.Where(o => o.IS_TO_CALCULATE_EGFR == 1).ToList();
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("dữ liệu SereServTeinData: " + Inventec.Common.Logging.LogUtil.GetMemberName(() => SereServTeinData), SereServTeinData));
                        if (SereServTeinData != null && SereServTeinData.Count > 0 && InstructionTime > 0 && TestIndexData != null && TestIndexData.Count > 0)
                        {
                            var DataSereServTeins = SereServTeinData.Where(o => TestIndexData.Exists(p => p.ID == o.TEST_INDEX_ID) && !String.IsNullOrEmpty(o.VALUE) && o.TDL_INTRUCTION_TIME < InstructionTime).OrderByDescending(o => o.TDL_INTRUCTION_TIME).ThenByDescending(o => o.ID).ToList();
                            if (DataSereServTeins != null && DataSereServTeins.Count > 0)
                            {
                                var DataSereServTein = DataSereServTeins.FirstOrDefault();
                                var testIndex = TestIndexData.FirstOrDefault(o => o.ID == (DataSereServTein.TEST_INDEX_ID ?? 0));
                                if (testIndex != null)
                                {
                                    decimal chiso;
                                    string ssTeinVL = DataSereServTein.VALUE.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                                     .Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ssTeinVL), ssTeinVL));
                                    if (Decimal.TryParse(ssTeinVL, out chiso) && currentTreatmentWithPatientType != null)
                                    {
                                        if (testIndex.CONVERT_RATIO_MLCT.HasValue)
                                            chiso *= (testIndex.CONVERT_RATIO_MLCT ?? 0);
                                        chiSoMLCT = testIndex.CONVERT_RATIO_MLCT ?? 0;
                                        strIsToCalculateEgfr = Inventec.Common.Calculate.Calculation.MucLocCauThanCrCleGFR(this.currentTreatmentWithPatientType.TDL_PATIENT_DOB, spinWeight.Value, spinHeight.Value, chiso, this.currentTreatmentWithPatientType.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE).ToString();
                                    }
                                }
                            }
                        }
                    }
                }
                lblIsToCalculateEgfr.Text = strIsToCalculateEgfr;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => strIsToCalculateEgfr), strIsToCalculateEgfr));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DHSTFillDataToBmiAndLeatherArea()
        {
            try
            {
                decimal bmi = 0;
                if (spinHeight.Value == 0 || spinWeight.Value == 0)
                {
                    lblBMI.Text = "";
                    lblLeatherArea.Text = "";
                    //lblBmiDisplayText.Text = "";
                    return;
                }

                bmi = (spinWeight.Value) / ((spinHeight.Value / 100) * (spinHeight.Value / 100));

                double leatherArea = 0.007184 * Math.Pow((double)spinHeight.Value, 0.725) * Math.Pow((double)spinWeight.Value, 0.425);
                lblBMI.Text = Math.Round(bmi, 2) + "";
                lblLeatherArea.Text = Math.Round(leatherArea, 2) + "";
                //if (bmi < 16)
                //{
                //    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("UCDHST.SKINNY.III", ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                //}
                //else if (16 <= bmi && bmi < 17)
                //{
                //    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("UCDHST.SKINNY.II", ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                //}
                //else if (17 <= bmi && bmi < (decimal)18.5)
                //{
                //    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("UCDHST.SKINNY.I", ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                //}
                //else if ((decimal)18.5 <= bmi && bmi < 25)
                //{
                //    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("UCDHST.BMIDISPLAY.NORMAL", ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                //}
                //else if (25 <= bmi && bmi < 30)
                //{
                //    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("UCDHST.BMIDISPLAY.OVERWEIGHT", ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                //}
                //else if (30 <= bmi && bmi < 35)
                //{
                //    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("UCDHST.BMIDISPLAY.OBESITY.I", ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                //}
                //else if (35 <= bmi && bmi < 40)
                //{
                //    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("UCDHST.BMIDISPLAY.OBESITY.II", ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                //}
                //else if (40 < bmi)
                //{
                //    lblBmiDisplayText.Text = Inventec.Common.Resource.Get.Value("UCDHST.BMIDISPLAY.OBESITY.III", ResourceLanguageManager.LanguagefrmAssignPrescription, LanguageManager.GetCulture());
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void gridControlCondition_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("gridControlCondition_Click.1");
                HIS_SERVICE_CONDITION conditionRow = (HIS_SERVICE_CONDITION)gridViewCondition.GetFocusedRow();
                if (conditionRow != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("gridControlCondition_Click.2");
                    popupControlContainerCondition.HidePopup();
                    MediMatyTypeADO ssADO = (MediMatyTypeADO)gridViewServiceProcess.GetFocusedRow();
                    if (ssADO != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("gridControlCondition_Click.3");
                        ssADO.SERVICE_CONDITION_ID = conditionRow.ID;
                        ssADO.SERVICE_CONDITION_NAME = conditionRow.SERVICE_CONDITION_NAME;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ssADO.SERVICE_CONDITION_ID), ssADO.SERVICE_CONDITION_ID)
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ssADO.SERVICE_CONDITION_NAME), ssADO.SERVICE_CONDITION_NAME));
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

        private void repositoryItemButtonCondition_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var currentRowSereServADO = (MediMatyTypeADO)gridViewServiceProcess.GetFocusedRow();
                //this.currentMedicineTypeADOForEdit = (MediMatyTypeADO)this.gridViewServiceProcess.GetFocusedRow();
                Inventec.Common.Logging.LogSystem.Debug("repositoryItemButtonCondition_ButtonClick:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentRowSereServADO), currentRowSereServADO)
                    + ",e.Button.Kind == ButtonPredefines.Down=" + (e.Button.Kind == ButtonPredefines.Down));
                if (currentRowSereServADO != null)
                {
                    if (e.Button.Kind == ButtonPredefines.Down)
                    {
                        ButtonEdit editor = sender as ButtonEdit;
                        ShowPopupContainerForServiceCondition(editor, currentRowSereServADO);
                        //Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                        //popupControlContainerCondition.ShowPopup(new Point(buttonPosition.X + 0, buttonPosition.Bottom + 288));

                        //var dataCondition = workingServiceConditions != null ? workingServiceConditions.Where(o => o.SERVICE_ID == currentRowSereServADO.SERVICE_ID).ToList() : null;
                        //if (dataCondition != null && dataCondition.Count > 0)
                        //{
                        //    List<HIS_SERVICE_CONDITION> dataConditionTmps = new List<HIS_SERVICE_CONDITION>();
                        //    foreach (var item in dataCondition)
                        //    {
                        //        if (dataConditionTmps.Count == 0 || !dataConditionTmps.Exists(t => t.ID == item.ID))
                        //        {
                        //            dataConditionTmps.Add(item);
                        //        }
                        //    }
                        //    dataCondition.Clear();
                        //    dataCondition.AddRange(dataConditionTmps);
                        //}

                        //Inventec.Common.Logging.LogSystem.Debug("ServiceID=" + currentRowSereServADO.SERVICE_ID + "MEDICINE_TYPE_CODE=" + currentRowSereServADO.MEDICINE_TYPE_CODE + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataCondition), dataCondition));

                        //gridControlCondition.DataSource = null;
                        //gridControlCondition.DataSource = dataCondition;
                        //gridControlCondition.Focus();
                        //if (currentRowSereServADO.SERVICE_CONDITION_ID > 0 && dataCondition != null && dataCondition.Count > 0)
                        //{
                        //    int focusRow = 0;
                        //    for (int i = 0; i < dataCondition.Count; i++)
                        //    {
                        //        if (dataCondition[i].ID == currentRowSereServADO.SERVICE_CONDITION_ID)
                        //        {
                        //            focusRow = i;
                        //        }
                        //    }
                        //    gridViewCondition.FocusedRowHandle = focusRow;
                        //}
                        //else
                        //{
                        //    gridViewCondition.FocusedRowHandle = 0;
                        //}
                    }
                    else if (e.Button.Kind == ButtonPredefines.Delete)
                    {
                        currentRowSereServADO.SERVICE_CONDITION_ID = null;
                        currentRowSereServADO.SERVICE_CONDITION_NAME = "";
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

        private void gridViewCondition_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var conditionRow = (HIS_SERVICE_CONDITION)this.gridViewCondition.GetFocusedRow();
                    if (conditionRow != null)
                    {
                        popupControlContainerCondition.HidePopup();
                        MediMatyTypeADO ssADO = (MediMatyTypeADO)gridViewServiceProcess.GetFocusedRow();
                        if (ssADO != null)
                        {
                            ssADO.SERVICE_CONDITION_ID = conditionRow.ID;
                            ssADO.SERVICE_CONDITION_NAME = conditionRow.SERVICE_CONDITION_NAME;
                            this.gridControlServiceProcess.RefreshDataSource();

                            if (this.gridViewServiceProcess.IsEditing)
                                this.gridViewServiceProcess.CloseEditor();

                            if (this.gridViewServiceProcess.FocusedRowModified)
                                this.gridViewServiceProcess.UpdateCurrentRow();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewCondition_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (((IList)((BaseView)sender).DataSource) != null && ((IList)((BaseView)sender).DataSource).Count > 0)
                    {
                        MediMatyTypeADO ssADO = (MediMatyTypeADO)gridViewServiceProcess.GetFocusedRow();
                        if (ssADO != null)
                        {
                            List<HIS_SERVICE_CONDITION> servicePatieDatas = ((List<HIS_SERVICE_CONDITION>)((BaseView)sender).DataSource);

                            HIS_SERVICE_CONDITION oneServiceSDO = (HIS_SERVICE_CONDITION)servicePatieDatas[e.ListSourceRowIndex];
                            if (oneServiceSDO != null)
                            {
                                if (e.Column.FieldName == "HEIN_RATIO_DISPLAY")
                                {
                                    e.Value = oneServiceSDO.HEIN_RATIO.HasValue ? (decimal?)Inventec.Common.Number.Convert.NumberToNumberRoundMax4((decimal)((oneServiceSDO.HEIN_RATIO ?? 0) * 100)) : null;
                                }
                            }
                            else
                            {
                                e.Value = null;
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

        private void ShowPopupContainerForServiceCondition(ButtonEdit editor, MediMatyTypeADO currentRowSereServADO)
        {
            try
            {
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainerCondition.ShowPopup(new Point(buttonPosition.X + 0, buttonPosition.Bottom + 298));

                var dataCondition = workingServiceConditions != null ? workingServiceConditions.Where(o => o.SERVICE_ID == currentRowSereServADO.SERVICE_ID).ToList() : null;
                if (dataCondition != null && dataCondition.Count > 0)
                {
                    List<HIS_SERVICE_CONDITION> dataConditionTmps = new List<HIS_SERVICE_CONDITION>();
                    foreach (var item in dataCondition)
                    {
                        if (dataConditionTmps.Count == 0 || !dataConditionTmps.Exists(t => t.ID == item.ID))
                        {
                            dataConditionTmps.Add(item);
                        }
                    }
                    dataCondition.Clear();
                    dataCondition.AddRange(dataConditionTmps);
                }

                Inventec.Common.Logging.LogSystem.Debug("ServiceID=" + currentRowSereServADO.SERVICE_ID + "MEDICINE_TYPE_CODE=" + currentRowSereServADO.MEDICINE_TYPE_CODE + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataCondition), dataCondition));

                gridControlCondition.DataSource = null;
                gridControlCondition.DataSource = dataCondition;
                gridControlCondition.Focus();
                if (currentRowSereServADO.SERVICE_CONDITION_ID > 0 && dataCondition != null && dataCondition.Count > 0)
                {
                    int focusRow = 0;
                    for (int i = 0; i < dataCondition.Count; i++)
                    {
                        if (dataCondition[i].ID == currentRowSereServADO.SERVICE_CONDITION_ID)
                        {
                            focusRow = i;
                        }
                    }
                    gridViewCondition.FocusedRowHandle = focusRow;
                }
                else
                {
                    gridViewCondition.FocusedRowHandle = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ShowPopupContainerForOtherPaySource(ButtonEdit editor, MediMatyTypeADO currentRowMediMatyTypeADO)
        {
            try
            {
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainerOtherPaySource.ShowPopup(new Point(buttonPosition.X + 0, buttonPosition.Bottom + 298));

                var dataOtherPaySources = BackendDataWorker.Get<HIS_OTHER_PAY_SOURCE>();
                List<HIS_OTHER_PAY_SOURCE> dataConditionTmps = new List<HIS_OTHER_PAY_SOURCE>();
                dataOtherPaySources = (dataOtherPaySources != null && dataOtherPaySources.Count > 0) ? dataOtherPaySources.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;
                if (dataOtherPaySources != null && dataOtherPaySources.Count > 0)
                {
                    var workingPatientType = currentPatientTypes.Where(t => t.ID == currentRowMediMatyTypeADO.PATIENT_TYPE_ID).FirstOrDefault();

                    if (workingPatientType != null && !String.IsNullOrEmpty(workingPatientType.OTHER_PAY_SOURCE_IDS))
                    {
                        dataConditionTmps = dataOtherPaySources.Where(o => ("," + workingPatientType.OTHER_PAY_SOURCE_IDS + ",").Contains("," + o.ID + ",")).ToList();

                        if (currentRowMediMatyTypeADO.OTHER_PAY_SOURCE_ID == null && dataConditionTmps != null && dataConditionTmps.Count == 1)
                        {
                            currentRowMediMatyTypeADO.OTHER_PAY_SOURCE_ID = dataConditionTmps[0].ID;
                            currentRowMediMatyTypeADO.OTHER_PAY_SOURCE_CODE = dataConditionTmps[0].OTHER_PAY_SOURCE_CODE;
                            currentRowMediMatyTypeADO.OTHER_PAY_SOURCE_NAME = dataConditionTmps[0].OTHER_PAY_SOURCE_NAME;
                        }
                    }
                    else
                    {
                        dataConditionTmps.AddRange(dataOtherPaySources);
                    }

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => workingPatientType), workingPatientType)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataConditionTmps), dataConditionTmps));
                }

                gridControlOtherPaySource.DataSource = null;
                gridControlOtherPaySource.DataSource = dataConditionTmps;
                gridControlOtherPaySource.Focus();

                int focusRow = 0;
                if (currentRowMediMatyTypeADO.OTHER_PAY_SOURCE_ID > 0 && dataConditionTmps != null && dataConditionTmps.Count > 0)
                {
                    for (int i = 0; i < dataConditionTmps.Count; i++)
                    {
                        if (dataConditionTmps[i].ID == currentRowMediMatyTypeADO.OTHER_PAY_SOURCE_ID)
                        {
                            focusRow = i;
                        }
                    }
                }
                gridViewOtherPaySource.FocusedRowHandle = focusRow;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhieuDieuTri_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboPhieuDieuTri.Properties.Buttons[1].Visible = (cboPhieuDieuTri.EditValue != null);
                if (this.isInitTracking)
                {
                    return;
                }

                if (cboPhieuDieuTri.EditValue != null)
                {
                    var trackingData = this.trackingADOs != null ? this.trackingADOs.FirstOrDefault(o => o.ID == (long)cboPhieuDieuTri.EditValue) : null;
                    if (trackingData != null)
                    {
                        this.Listtrackings = new List<HIS_TRACKING>();
                        HIS_TRACKING tracking = new HIS_TRACKING();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRACKING>(tracking, trackingData);
                        this.Listtrackings.Add(tracking);
                    }
                    else
                    {
                        Listtrackings = null;
                    }
                }
                else
                {
                    Listtrackings = null;
                }

                if (!HisConfigCFG.IsServiceReqIcdOption && HisConfigCFG.TrackingCreate__UpdateTreatmentIcd == "1")
                {
                    this.LoadIcdDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlOtherPaySource_Click(object sender, EventArgs e)
        {
            try
            {
                HIS_OTHER_PAY_SOURCE conditionRow = (HIS_OTHER_PAY_SOURCE)gridViewOtherPaySource.GetFocusedRow();
                if (conditionRow != null)
                {
                    popupControlContainerOtherPaySource.HidePopup();
                    var currentRowMediMatyTypeADO = (MediMatyTypeADO)gridViewServiceProcess.GetFocusedRow();
                    if (currentRowMediMatyTypeADO != null)
                    {
                        currentRowMediMatyTypeADO.OTHER_PAY_SOURCE_ID = conditionRow.ID;
                        currentRowMediMatyTypeADO.OTHER_PAY_SOURCE_NAME = conditionRow.OTHER_PAY_SOURCE_NAME;

                        this.UpdateExpMestReasonInDataRow(currentRowMediMatyTypeADO);

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

        private void gridViewOtherPaySource_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var conditionRow = (HIS_OTHER_PAY_SOURCE)this.gridViewOtherPaySource.GetFocusedRow();
                    if (conditionRow != null)
                    {
                        popupControlContainerOtherPaySource.HidePopup();
                        var currentRowMediMatyTypeADO = (MediMatyTypeADO)gridViewServiceProcess.GetFocusedRow();
                        if (currentRowMediMatyTypeADO != null)
                        {
                            currentRowMediMatyTypeADO.OTHER_PAY_SOURCE_ID = conditionRow.ID;
                            currentRowMediMatyTypeADO.OTHER_PAY_SOURCE_NAME = conditionRow.OTHER_PAY_SOURCE_NAME;
                            this.gridControlServiceProcess.RefreshDataSource();

                            if (this.gridViewServiceProcess.IsEditing)
                                this.gridViewServiceProcess.CloseEditor();

                            if (this.gridViewServiceProcess.FocusedRowModified)
                                this.gridViewServiceProcess.UpdateCurrentRow();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonOtherPaySource_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var currentRowMediMatyTypeADO = (MediMatyTypeADO)gridViewServiceProcess.GetFocusedRow();
                if (currentRowMediMatyTypeADO != null && currentRowMediMatyTypeADO.PATIENT_TYPE_ID > 0)
                {
                    if (e.Button.Kind == ButtonPredefines.Down || e.Button.Kind == ButtonPredefines.DropDown)
                    {
                        ButtonEdit editor = sender as ButtonEdit;
                        ShowPopupContainerForOtherPaySource(editor, currentRowMediMatyTypeADO);

                    }
                    else if (e.Button.Kind == ButtonPredefines.Delete)
                    {
                        currentRowMediMatyTypeADO.OTHER_PAY_SOURCE_ID = null;
                        currentRowMediMatyTypeADO.OTHER_PAY_SOURCE_CODE = "";
                        currentRowMediMatyTypeADO.OTHER_PAY_SOURCE_NAME = "";

                        this.UpdateExpMestReasonInDataRow(currentRowMediMatyTypeADO);

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

        private void chkPDDT_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                LoadDataToGridMetyMatyTypeInStock();
                ChangeCheckPrintAndPreview();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public List<HIS_ICD_SERVICE> geticdServices()
        {
            List<HIS_ICD_SERVICE> icdServices = new List<HIS_ICD_SERVICE>();
            try
            {
                //Lay danh sach icd
                string icdCode = "";
                var icdValue = this.UcIcdGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                if (icdValue != null)
                {
                    icdCode = icdValue.ICD_CODE;
                }

                var subIcd = this.UcSecondaryIcdGetValue() as SecondaryIcdDataADO;
                if (subIcd != null)
                {
                    icdCode += subIcd.ICD_SUB_CODE;
                }

                string[] icdCodeArr = icdCode.Split(';');
                var mediMatyTypeAllows = this.mediMatyTypeADOs.Where(o => o.SERVICE_ID > 0).ToList();

                CommonParam param = new CommonParam();
                HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
                icdServiceFilter.ICD_CODE__EXACTs = icdCodeArr.ToList();
                icdServices = new BackendAdapter(param)
               .Get<List<MOS.EFMODEL.DataModels.HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumers.MosConsumer, icdServiceFilter, param);

                icdServices = icdServices != null ? icdServices.Where(o => o.SERVICE_ID != null || o.ACTIVE_INGREDIENT_ID != null).ToList() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return icdServices;
        }

        private bool CheckMultiIntructionTime()
        {
            bool result = true;
            try
            {
                if (this.chkMultiIntructionTime.Checked)
                {
                    List<string> NotWithinDay = new List<string>();
                    List<string> MoreThanOneTracking = new List<string>();
                    List<string> Notracking = new List<string>();

                    List<string> intructionDateSelectedProcess = new List<string>();
                    foreach (var item in this.intructionTimeSelecteds)
                    {
                        string intructionDate = item.ToString().Substring(0, 8);
                        intructionDateSelectedProcess.Add(intructionDate);
                    }

                    if (this.Listtrackings != null && this.Listtrackings.Count > 0)
                    {
                        foreach (var itemT in this.Listtrackings)
                        {
                            var checkWithDay = intructionDateSelectedProcess.Contains(itemT.TRACKING_TIME.ToString().Substring(0, 8));

                            if (!checkWithDay)
                            {
                                // var day = String.Format("{0:dd/MM}", Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(itemT.TRACKING_TIME));
                                var day = itemT.TRACKING_TIME.ToString().Substring(6, 2) + "/" + itemT.TRACKING_TIME.ToString().Substring(4, 2);
                                NotWithinDay.Add(day);
                            }
                        }
                        var trackings = this.Listtrackings.GroupBy(o => o.TRACKING_TIME.ToString().Substring(0, 8));

                        foreach (var item in trackings)
                        {
                            if (item.ToList().Count > 1)
                            {
                                // var day = String.Format("{0:dd/MM}", Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item.FirstOrDefault().TRACKING_TIME));
                                var day = item.FirstOrDefault().TRACKING_TIME.ToString().Substring(6, 2) + "/" + item.FirstOrDefault().TRACKING_TIME.ToString().Substring(4, 2);
                                MoreThanOneTracking.Add(day);
                            }
                        }

                        foreach (var itemDate in intructionDateSelectedProcess)
                        {
                            var tra = this.Listtrackings.Where(o => o.TRACKING_TIME.ToString().Substring(0, 8) == itemDate).ToList();

                            if (tra == null || tra.Count <= 0)
                            {
                                var day = itemDate.Substring(6, 2) + "/" + itemDate.Substring(4, 2);
                                Notracking.Add(day);
                            }
                        }
                    }

                    if (NotWithinDay != null && NotWithinDay.Count > 0)
                    {
                        XtraMessageBox.Show(string.Format(ResourceMessage.NgayKhongNamTrongNgaykeDon, string.Join(", ", NotWithinDay.Distinct())));
                        return false;
                    }

                    if (MoreThanOneTracking != null && MoreThanOneTracking.Count > 0)
                    {
                        XtraMessageBox.Show(string.Format(ResourceMessage.NgayCoNhieuHon1ToDieuTri, string.Join(", ", MoreThanOneTracking.Distinct())));
                        return false;
                    }

                    if (Notracking != null && Notracking.Count > 0)
                    {
                        if (XtraMessageBox.Show(string.Format(ResourceMessage.NgayChuaCoToDieuTri, string.Join(", ", Notracking.Distinct())), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo) == DialogResult.No)
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

        private void cboPhieuDieuTri_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string ToDieuTri = "";
                if (this.Listtrackings != null && this.Listtrackings.Count > 0)
                {
                    foreach (var item in this.Listtrackings)
                    {
                        ToDieuTri += Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(item.TRACKING_TIME);

                        if (this.chkMultiIntructionTime.Checked)
                        {
                            ToDieuTri += ", ";
                        }
                    }
                }

                e.DisplayText = ToDieuTri;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtUseTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    NextForcusUCDate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtUseTime_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Glyph)
                {
                    frmMultiIntructonTime frmChooseIntructionTime = new frmMultiIntructonTime(UseTimeSelected, null, SelectMultiUseTime);
                    frmChooseIntructionTime.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void DelegateSelectMultiDateUseTime(List<DateTime?> datas, DateTime time)
        {
            try
            {
                this.UseTimeSelecteds = this.UcDateGetValueUseTime();

                ChangeIntructionTimeUseTime(time);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangeIntructionTimeUseTime(DateTime UseTime)
        {
            try
            {
                if (!this.txtUseTime.Visible)
                {
                    this.UseTimeSelected = new List<DateTime?>();
                    this.UseTimeSelected.Add(UseTime);
                    this.UseTimeSelecteds = (this.UseTimeSelected.Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o.Value.ToString("yyyyMMdd") + "000000")).OrderByDescending(o => o).ToList());
                }

                this.UseTime = this.UseTimeSelecteds.OrderByDescending(o => o).First();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtUseTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.ChangeUseTimeEditor(this.dtUseTime.DateTime);

                if (dtUseTime.EditValue == null)
                {
                    this.chkMultiIntructionTime.Enabled = true;
                }
                else
                {
                    this.chkMultiIntructionTime.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExpMestReason_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboExpMestReason.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExpMestReason_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode != Keys.Enter)
                {
                    cboExpMestReason.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCustomGridLookUpReasion_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    var cbo = sender as GridLookUpEdit;

                    if (cbo != null && cbo.EditValue != null)
                    {
                        var row = (MediMatyTypeADO)gridViewServiceProcess.GetFocusedRow();

                        long ExpMestReasonId = (long)cbo.EditValue;

                        var ExpMestReason = this.lstExpMestReasons.FirstOrDefault(o => o.ID == ExpMestReasonId);

                        if (ExpMestReason != null)
                        {
                            row.EXP_MEST_REASON_ID = ExpMestReason.ID;
                            row.EXP_MEST_REASON_CODE = ExpMestReason.EXP_MEST_REASON_CODE;
                            row.EXP_MEST_REASON_NAME = ExpMestReason.EXP_MEST_REASON_CODE;
                        }
                        else
                        {
                            row.EXP_MEST_REASON_ID = null;
                            row.EXP_MEST_REASON_CODE = "";
                            row.EXP_MEST_REASON_NAME = "";
                        }

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

        private void repositoryItemCustomGridLookUpReasion_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var currentRowSereServADO = (MediMatyTypeADO)gridViewServiceProcess.GetFocusedRow();

                if (currentRowSereServADO != null)
                {
                    if (e.Button.Kind == ButtonPredefines.Delete)
                    {
                        currentRowSereServADO.EXP_MEST_REASON_ID = null;
                        currentRowSereServADO.EXP_MEST_REASON_CODE = "";
                        currentRowSereServADO.EXP_MEST_REASON_NAME = "";

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
        private void btnLatentTuberCulosis_Click(object sender, EventArgs e)
        {
            try
            {
                ShowFrmTub();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowFrmTub()
        {
            try
            {
                tuberCulosisADO = new TuberCulosisADO();
                tuberCulosisADO.MediOrgCode = VHistreatment.TUBERCULOSIS_ISSUED_ORG_CODE;
                tuberCulosisADO.MediOrgName = VHistreatment.TUBERCULOSIS_ISSUED_ORG_NAME;
                tuberCulosisADO.TuberCulosisTime = VHistreatment.TUBERCULOSIS_ISSUED_DATE;
                ChooseMediStock.frmTuberCulosis frm = new ChooseMediStock.frmTuberCulosis(tuberCulosisADO, GetTuberCulosis, VHistreatment.ID);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void VisibleLayoutSubIcd(bool IsVisble)
        {
            try
            {
                layoutControlItem35.Visibility = IsVisble && !GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCheckIcd_Click(object sender, EventArgs e)
        {
            try
            {
                #region Lấy toàn bộ các CĐ của hsđt
                CommonParam param = new CommonParam();
                HisServiceReqFilter filter = new HisServiceReqFilter();
                filter.TREATMENT_ID = treatmentId;
                var result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, param);
                List<HIS_ICD> lstHisIcd = new List<HIS_ICD>();
                foreach (var item in result)
                {
                    if (!string.IsNullOrEmpty(item.ICD_CODE))
                    {
                        HIS_ICD icd = new HIS_ICD();
                        icd.ICD_CODE = item.ICD_CODE;
                        icd.ICD_NAME = item.ICD_NAME;
                        lstHisIcd.Add(icd);
                    }

                    if (!string.IsNullOrEmpty(item.ICD_SUB_CODE) || !string.IsNullOrEmpty(item.ICD_TEXT))
                    {
                        string[] arrIcd = !string.IsNullOrEmpty(item.ICD_SUB_CODE) ? item.ICD_SUB_CODE.Split(';') : null;
                        string[] arrIcdName = !string.IsNullOrEmpty(item.ICD_TEXT) ? item.ICD_TEXT.Split(';') : null;
                        if (arrIcd != null && arrIcd.Count() > 0)
                        {
                            for (int i = 0; i < arrIcd.Length; i++)
                            {
                                HIS_ICD icd = new HIS_ICD();
                                if (!string.IsNullOrEmpty(arrIcd[i]))
                                {
                                    icd.ICD_CODE = arrIcd[i].Trim();
                                }
                                try
                                {
                                    icd.ICD_NAME = arrIcdName != null ? arrIcdName[i].Trim() : null;
                                    lstHisIcd.Add(icd);
                                }
                                catch (Exception ex)
                                {
                                    lstHisIcd.Add(icd);
                                }
                            }
                        }

                        if (arrIcdName != null && arrIcdName.Count() > 0 && arrIcd != null && arrIcd.Count() > 0 && arrIcd.Length < arrIcdName.Length)
                        {
                            for (int i = arrIcd.Length; i < arrIcdName.Length; i++)
                            {
                                HIS_ICD icd = new HIS_ICD();
                                icd.ICD_NAME = arrIcdName[i].Trim();
                                lstHisIcd.Add(icd);

                            }
                        }
                        else if (arrIcdName != null && arrIcdName.Count() > 0)
                        {
                            for (int i = 0; i < arrIcdName.Length; i++)
                            {
                                HIS_ICD icd = new HIS_ICD();
                                icd.ICD_NAME = arrIcdName[i].Trim();
                                lstHisIcd.Add(icd);

                            }
                        }
                    }
                }
                #endregion
                #region Phân loại các ICD
                foreach (var item in lstHisIcd)
                {
                    if (!string.IsNullOrEmpty(item.ICD_CODE) && !string.IsNullOrEmpty(item.ICD_NAME))
                    {
                        var icd = currentIcds.FirstOrDefault(o => o.ICD_CODE == item.ICD_CODE);
                        if (icd != null && icd.ICD_NAME == item.ICD_NAME)
                        {
                            item.IS_ACTIVE = 1;
                        }
                        else
                        {
                            item.IS_ACTIVE = 2;
                        }
                    }
                    if (string.IsNullOrEmpty(item.ICD_NAME))
                    {
                        item.IS_ACTIVE = 3;
                    }
                    if (string.IsNullOrEmpty(item.ICD_CODE))
                    {
                        item.IS_ACTIVE = 4;
                    }
                }
                List<IcdCheckADO> lstIcdNew = new List<IcdCheckADO>();
                foreach (var item in lstHisIcd)
                {
                    if (!string.IsNullOrEmpty(item.ICD_CODE) || !string.IsNullOrEmpty(item.ICD_NAME))

                        lstIcdNew.Add(new IcdCheckADO() { ICD_CODE = item.ICD_CODE, ICD_NAME = item.ICD_NAME, IS_ACTIVE = item.IS_ACTIVE });
                }
                lstIcdNew = lstIcdNew.OrderBy(o => o.IS_ACTIVE).ThenBy(o => o.ICD_CODE).Distinct(new Compare()).ToList();
                //lstIcdNew = lstIcdNew.OrderBy(o=>o.ICD_CODE).Distinct(new Compare()).ToList();
                #endregion
                SecondaryIcdDataADO subIcd = new SecondaryIcdDataADO();
                string icdSubCodeQ = null;
                string icdSubNameQ = null;
                string icdCode = null;
                string icdName = null;
                icdCode = txtIcdCode.Text.Trim();
                icdName = txtIcdMainText.Text.Trim();
                List<IcdCheckADO> lstIcd = lstIcdNew.Where(o => o.ICD_CODE != icdCode && o.ICD_NAME != icdName).ToList();
                icdSubCodeQ = String.Join(";", lstIcd.Select(o => o.ICD_CODE).Distinct()) + ";";
                icdSubNameQ = String.Join(";", lstIcd.Select(o => o.ICD_NAME).Distinct()) + ";";
                var lstIcdActive = lstIcdNew.Where(o => o.ICD_CODE == icdCode).ToList();
                foreach (var item in lstIcdActive)
                {
                    if (item.ICD_NAME == icdName)
                        continue;
                    icdSubNameQ += item.ICD_NAME + ";";
                }
                #region Xử lý dấu ; 
                if (!string.IsNullOrEmpty(icdSubCodeQ) && (icdSubCodeQ.StartsWith(";") || icdSubCodeQ.EndsWith(";")))
                {
                    List<string> lstTmp = new List<string>();
                    var arr = icdSubCodeQ.Split(';');
                    foreach (var item in arr)
                    {
                        if (!string.IsNullOrEmpty(item))
                            lstTmp.Add(item);
                    }
                    icdSubCodeQ = string.Join(";", lstTmp);
                }
                if (!string.IsNullOrEmpty(icdSubNameQ) && (icdSubNameQ.StartsWith(";") || icdSubNameQ.EndsWith(";")))
                {
                    List<string> lstTmp = new List<string>();
                    var arr = icdSubNameQ.Split(';');
                    foreach (var item in arr)
                    {
                        if (!string.IsNullOrEmpty(item))
                            lstTmp.Add(item);
                    }
                    icdSubNameQ = string.Join(";", lstTmp);
                }
                #endregion
                isNotProcessWhileChangedTextSubIcd = true;
                LoadDataToIcdSub(icdSubCodeQ, icdSubNameQ);
                isNotProcessWhileChangedTextSubIcd = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            try
            {
                ShowPopupContainerForConfig(btnConfig);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ShowPopupContainerForConfig(SimpleButton editor)
        {
            try
            {
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainer1.ShowPopup(new Point(buttonPosition.X, buttonPosition.Bottom - 100));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitListConfig()
        {
            try
            {
                List<string> ListName = new List<string>() { "Không hiển thị đơn không lấy ở đơn thuốc TH" };
                lstConfig = new List<ConfigADO>();
                for (int i = 0; i < ListName.Count; i++)
                {
                    lstConfig.Add(new ConfigADO() { ID = i + 1, NAME = ListName[i], IsChecked = ConfigIds.Exists(o => o == (i + 1)) });
                }
                gridControlConfig.DataSource = null;
                gridControlConfig.DataSource = lstConfig;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void popupControlContainer1_CloseUp(object sender, EventArgs e)
        {
            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == gridControlConfig.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                lstConfig = gridControlConfig.DataSource as List<ConfigADO>;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstConfig), lstConfig));
                var lstSelect = lstConfig.Where(o => o.IsChecked).Select(o => o.ID);
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = String.Join(";", lstSelect);
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = gridControlConfig.Name;
                    csAddOrUpdate.VALUE = String.Join(";", lstSelect);
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableButtonConfig()
        {
            try
            {
                btnConfig.Enabled = lciPrintAssignPrescription.Enabled;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repCheckConfig_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckEdit chk = sender as CheckEdit;
                foreach (var item in lstConfig)
                {
                    if (item.ID == ((ConfigADO)gridViewConfig.GetFocusedRow()).ID)
                        item.IsChecked = chk.Checked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemChkIsExpend__MedicinePage_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckEdit chk = sender as CheckEdit;
                var currentRowSereServADO = (MediMatyTypeADO)gridViewServiceProcess.GetFocusedRow();
                if (currentRowSereServADO.IsDisableExpend)
                {
                    chk.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void pbClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (pbClose.Image == null)
                    return;

                IsOpen = false;
                if (IsStateCase1Dhst)
                {
                    if (!this.navBarDHSTInfoState && this.layoutControlItemDHSTInfo.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                    {
                        pbClose.Image = global::HIS.Desktop.Plugins.AssignPrescriptionPK.Properties.Resources.arrow_up;
                        pbOpen.Image = global::HIS.Desktop.Plugins.AssignPrescriptionPK.Properties.Resources.arrow_down;
                        navBarDHSTInfoState = true;
                    }
                    else
                    {
                        pbClose.Image = null;
                        pbOpen.Image = global::HIS.Desktop.Plugins.AssignPrescriptionPK.Properties.Resources.arrow_down;
                        IsStateCase1Dhst = false;
                    }
                }
                else
                {
                    if (pbOpen.Image == null)
                    {
                        pbClose.Image = global::HIS.Desktop.Plugins.AssignPrescriptionPK.Properties.Resources.arrow_up;
                        pbOpen.Image = global::HIS.Desktop.Plugins.AssignPrescriptionPK.Properties.Resources.arrow_down;
                        IsStateCase1Dhst = true;
                    }
                    else
                    {
                        pbClose.Image = null;
                        pbOpen.Image = global::HIS.Desktop.Plugins.AssignPrescriptionPK.Properties.Resources.arrow_down;
                        IsStateCase1Dhst = false;
                    }
                }
                VisibleDhst();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void pbOpen_Click(object sender, EventArgs e)
        {
            try
            {
                if (pbOpen.Image == null)
                    return;
                IsOpen = true;
                if (IsStateCase1Dhst)
                {
                    if (!this.navBarDHSTInfoState && this.layoutControlItemDHSTInfo.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                    {
                        pbClose.Image = global::HIS.Desktop.Plugins.AssignPrescriptionPK.Properties.Resources.arrow_up;
                        pbOpen.Image = global::HIS.Desktop.Plugins.AssignPrescriptionPK.Properties.Resources.arrow_down;
                        navBarDHSTInfoState = true;
                    }
                    else
                    {
                        pbClose.Image = global::HIS.Desktop.Plugins.AssignPrescriptionPK.Properties.Resources.arrow_up;
                        pbOpen.Image = null;
                        IsStateCase1Dhst = false;
                    }
                }
                else
                {
                    if (pbClose.Image == null)
                    {
                        pbClose.Image = global::HIS.Desktop.Plugins.AssignPrescriptionPK.Properties.Resources.arrow_up;
                        pbOpen.Image = global::HIS.Desktop.Plugins.AssignPrescriptionPK.Properties.Resources.arrow_down;
                        IsStateCase1Dhst = true;
                    }
                    else
                    {
                        pbClose.Image = global::HIS.Desktop.Plugins.AssignPrescriptionPK.Properties.Resources.arrow_up;
                        pbOpen.Image = null;
                        IsStateCase1Dhst = false;
                    }
                }
                VisibleDhst();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SaveKeyImage(string keyName, bool IsOpen)
        {
            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == keyName && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = IsOpen ? "1" : "0";
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = keyName;
                    csAddOrUpdate.VALUE = IsOpen ? "1" : "0";
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SaveKeyDHST()
        {
            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.lcgDHSTInfo && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = layoutControlItemDHSTInfo.Visible ? "1" : "0";
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstan.lcgDHSTInfo;
                    csAddOrUpdate.VALUE = layoutControlItemDHSTInfo.Visible ? "1" : "0";
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void VisibleDhst()
        {
            try
            {
                if (pbClose.Image == null && pbOpen.Image != null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("CASE 1");
                    this.layoutControlItemDHSTInfo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.lciUCBottomPanel.Size = new Size(lciUCBottomPanel.Width, lciUCBottomPanel.Height + layoutControlItemDHSTInfo.Height);
                    this.layoutControlItem15.Size = sizeListPatient;
                }
                else if (pbClose.Image != null && pbOpen.Image == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("CASE 2");
                    this.layoutControlItemDHSTInfo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.layoutControlItem15.Size = new Size(layoutControlItem15.Width, layoutControlItem15.Height + layoutControlItemDHSTInfo.Height);
                    this.lciUCBottomPanel.Size = sizeExpMest;
                }
                else if (pbClose.Image != null && pbOpen.Image != null && IsOpen == false)
                {
                    Inventec.Common.Logging.LogSystem.Warn("CASE 3");
                    this.layoutControlItemDHSTInfo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.layoutControlItem15.Size = sizeListPatient;// new Size(layoutControlItem15.Width, layoutControlItem15.Height - layoutControlItemDHSTInfo.Height);
                }
                else if (pbClose.Image != null && pbOpen.Image != null && IsOpen == true)
                {
                    Inventec.Common.Logging.LogSystem.Warn("CASE 4");
                    this.layoutControlItemDHSTInfo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.lciUCBottomPanel.Size = sizeExpMest;// new Size(lciUCBottomPanel.Width, lciUCBottomPanel.Height - layoutControlItemDHSTInfo.Height);
                }
                this.layoutControlItem21.Size = sizeInforPatient;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkTemporayPres_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkTemporayPres.Checked)
                {
                    cboPhieuDieuTri.EditValue = null;
                    cboPhieuDieuTri.Enabled = false;
                    dxValidationProviderControl.SetValidationRule(cboPhieuDieuTri, null);
                    this.lciPhieuDieuTri.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                }
                else
                {
                    cboPhieuDieuTri.Enabled = true;
                    if (HisConfigCFG.IsTrackingRequired)
                    {
                        ValidationSingleControl(cboPhieuDieuTri, dxValidationProviderControl, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc), ValidTracking);
                        this.lciPhieuDieuTri.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void timeIntruction_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {

            try
            {
                if (e.Info == null && e.SelectedControl == this.rdOpionGroup)
                {
                    RadioGroup rg = e.SelectedControl as RadioGroup;
                    if (rg == null) return;
                    for (int i = 0; i < rdOpionGroup.Properties.Items.Count; i++)
                    {
                        Rectangle rect = rdOpionGroup.GetItemRectangle(i);
                        if (rect.Contains(e.ControlMousePosition) && i == 2)
                        {
                            e.Info = new ToolTipControlInfo(i, "Vật tư quản lý đích danh theo số seri hoặc vật tư tái sử dụng");
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void GetTuberCulosis(TuberCulosisADO obj)
        {
            try
            {
                tuberCulosisADO = obj;
                if (obj != null)
                {
                    VHistreatment.TUBERCULOSIS_ISSUED_ORG_CODE = tuberCulosisADO.MediOrgCode;
                    VHistreatment.TUBERCULOSIS_ISSUED_ORG_NAME = tuberCulosisADO.MediOrgName;
                    VHistreatment.TUBERCULOSIS_ISSUED_DATE = tuberCulosisADO.TuberCulosisTime;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repInForMedicineEnable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                var currentRowSereServADO = (MediMatyTypeADO)gridViewServiceProcess.GetFocusedRow();

                if (currentRowSereServADO != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisProductInfo").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisProductInfo");
                    if (moduleData != null && moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(new HIS.Desktop.ADO.ProductInfoADO() { MedicineTypeId = currentRowSereServADO.ID, ProductInfoOpen = 0 });//TODO
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void repELInBatchCliEnable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var currentRowSereServADO = (MediMatyTypeADO)gridViewServiceProcess.GetFocusedRow();

                if (currentRowSereServADO != null)
                {
                    List<MediMatyTypeADO> lst = new List<MediMatyTypeADO>();
                    if (mediMatyTypeADOsAlertInTreatment != null && mediMatyTypeADOsAlertInTreatment.Count > 0 && mediMatyTypeADOsAlertInTreatment.Exists(o => o.PrimaryKey == currentRowSereServADO.PrimaryKey))
                    {
                        lst = mediMatyTypeADOsAlertInTreatment.Where(o => o.PrimaryKey == currentRowSereServADO.PrimaryKey).ToList();
                    }
                    else
                    {
                        if (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                        {
                            var listPatientSelecteds = this.patientSelectProcessor.GetSelectedRows(this.ucPatientSelect);
                            foreach (var item in listPatientSelecteds)
                            {
                                MediMatyTypeADO ado = currentRowSereServADO;
                                ado.PATIENT_NAME_BY_TREATMENT_CODE = item.TDL_PATIENT_NAME + "_" + item.TREATMENT_CODE;
                                ado.IsAlertInTreatPresciption = true;
                                lst.Add(ado);
                            }
                        }
                        else
                        {
                            currentRowSereServADO.IsAlertInTreatPresciption = true;
                            currentRowSereServADO.PATIENT_NAME_BY_TREATMENT_CODE = currentTreatment.TDL_PATIENT_NAME + "_" + currentTreatment.TREATMENT_CODE;
                            lst.Add(currentRowSereServADO);
                        }

                    }
                    frmMutlilPatientReasonInBatch frm = new frmMutlilPatientReasonInBatch(lst, GetAlertListInTreatment);
                    frm.ShowDialog();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridControlHtu_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    if (this.gridViewHtu.IsEditing)
                        this.gridViewHtu.CloseEditor();

                    if (this.gridViewHtu.FocusedRowModified)
                        this.gridViewHtu.UpdateCurrentRow();
                    var dt = gridViewHtu.GetFocusedRow() as HtuADO;
                    dt.IsChecked = !dt.IsChecked;
                    gridControlHtu.RefreshDataSource();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    cboHtu_Closed(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewHtu_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void memHtu_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnAdd.Select();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void popupContainerHtu_Leave(object sender, EventArgs e)
        {

            try
            {
                if (DataHtuList != null && DataHtuList.Count > 0 && DataHtuList.Exists(o => o.IsChecked))
                {
                    cboHtu.Text = string.Join(", ", DataHtuList.Where(o => o.IsChecked).Select(o => o.HTU_NAME));
                }
                else
                    cboHtu.Text = null;
                if (string.IsNullOrEmpty(cboHtu.Text))
                    this.cboHtu.Properties.Buttons[1].Visible = true;
                this.SetHuongDanFromSoLuongNgay();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void repCheck_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var dt = gridViewHtu.GetFocusedRow() as HtuADO;
                CheckEdit chk = sender as CheckEdit;
                foreach (var item in DataHtuList)
                {
                    if (item.ID == dt.ID)
                        item.IsChecked = chk.Checked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboHtu_EditValueChanged(object sender, EventArgs e)
        {
            this.CalculateAmount();
            if (this.medicineTypeTutSelected == null)
            {
                this.memHtu.Text = this.cboMedicineUseForm.Text + " " + this.cboHtu.Text;
            }
            if (!IsSetByMedicineTut)
                this.SetHuongDanFromSoLuongNgay();
        }

        private void gridControlMediMaty_Click(object sender, EventArgs e)
        {

        }

        private void gridViewHtuText_RowClick(object sender, RowClickEventArgs e)
        {
            try
            {
                medicineTypehtu = gridViewHtuText.GetFocusedRow() as HIS_MEDICINE_TYPE_TUT;
                if (medicineTypehtu != null)
                {
                    popupControlContainerHtuText.HidePopup();
                    isShowContainerHTU = false;
                    HtuText_RowClick(medicineTypehtu);
                }
                

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewHtuText_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var medicineTypeADOForEdit = this.gridViewMediMaty.GetFocusedRow();
                    if (medicineTypeADOForEdit != null)
                    {
                        isShowContainerMediMaty = false;
                        isShowContainerMediMatyForChoose = true;
                        popupControlContainerMediMaty.HidePopup();
                        var selectedOpionGroup = GetSelectedOpionGroup();
                        if (selectedOpionGroup == 1)
                        {
                            MetyMatyTypeInStock_RowClick(medicineTypeADOForEdit);
                        }
                        else if (selectedOpionGroup == 2)
                        {
                            MedicineType_RowClick(medicineTypeADOForEdit);
                        }
                        else
                        {
                            MaterialTypeTSD_RowClick(medicineTypeADOForEdit);
                        }
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    this.gridViewMediMaty.Focus();
                    this.gridViewMediMaty.FocusedRowHandle = this.gridViewMediMaty.FocusedRowHandle;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void memHtu_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.DropDown)
                {
                    isShowContainerHTU = !isShowContainerHTU;
                    if (isShowContainerHTU)
                    {
                        Rectangle buttonBounds = new Rectangle(memHtu.Bounds.X, memHtu.Bounds.Y, memHtu.Bounds.Width, memHtu.Bounds.Height);
                        popupControlContainerHtuText.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 25));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void memHtu_EditValueChanged(object sender, EventArgs e)
        {
            string expected = cboMedicineUseForm.Text + " " + cboHtu.Text;     

            if (memHtu.Text != expected)
            {
                shouldAutoUpdateMemHtu = false;     
            }
        }

        internal bool CheckValidMaterial(bool IsCheckList = false)
        {
            bool result = true;
            try
            {
                if ((currentMedicineTypeADOForEdit != null && (currentMedicineTypeADOForEdit.IS_REUSABLE == 1 || currentMedicineTypeADOForEdit.IS_IDENTITY_MANAGEMENT == 1) || (IsCheckList && mediMatyTypeADOs != null && mediMatyTypeADOs.Count > 0 && mediMatyTypeADOs.Exists(o => o.IS_REUSABLE == 1 || o.IS_IDENTITY_MANAGEMENT == 1))) && ((GlobalStore.IsTreatmentIn && this.patientSelectProcessor != null && this.ucPatientSelect != null && this.patientSelectProcessor.GetSelectedRows(this.ucPatientSelect).Count > 1) || (this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 1) || (this.UseTimeSelecteds != null && this.UseTimeSelecteds.Count > 1)))
                {
                    XtraMessageBox.Show("Vật tư đích danh - TSD không cho phép kê nhiều ngày, dự trù, kê nhiều bệnh nhân");
                    if (IsCheckList)
                        mediMatyTypeADOs = new List<MediMatyTypeADO>();
                    result = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}

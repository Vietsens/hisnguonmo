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
using DevExpress.XtraBars.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.ApplicationFont;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignNoneMediService.ADO;
using HIS.Desktop.Plugins.AssignNoneMediService.Config;
using HIS.Desktop.Plugins.AssignNoneMediService.Resources;
using HIS.Desktop.Plugins.Library.CheckIcd;
using HIS.Desktop.Plugins.Library.PrintServiceReq;
using HIS.Desktop.Print;
using HIS.Desktop.Utilities;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utilities.Extentions;
using HIS.Desktop.Utility;
using HIS.UC.DateEditor;
using HIS.UC.Icd;
using HIS.UC.Icd.ADO;
using HIS.UC.PatientSelect;
using HIS.UC.SecondaryIcd;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Controls.PopupLoader;
using Inventec.Common.Logging;
using Inventec.Common.RichEditor.Base;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Common.Modules;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MPS.ProcessorBase.PrintConfig;

namespace HIS.Desktop.Plugins.AssignNoneMediService.AssignService
{
    public partial class frmAssignService : HIS.Desktop.Utility.FormBase
    {
        #region Reclare variable
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        CheckIcdManager checkIcdManager { get; set; }
        string[] periodSeparators = new string[] { "," };
        string[] icdSeparators = new string[] { ";" };
        const string commonString__true = "1";
        long? serviceReqParentId;
        int positionHandleControl = -1;
        int actionType = 0;
        long treatmentId = 0;
        long previusTreatmentId = 0;
        long? examRegisterRoomId;
        internal bool isMultiDateState = false;
        internal List<long> intructionTimeSelecteds = new List<long>();
        internal List<DateTime?> intructionTimeSelected = new List<DateTime?>();
        DateTime timeSelested;
        internal long InstructionTime { get; set; }
        bool isInitUcDate;

        V_HIS_SERE_SERV currentSereServ { get; set; }
        V_HIS_SERE_SERV currentSereServInEkip { get; set; }
        HIS.Desktop.ADO.AssignServiceADO.DelegateProcessDataResult processDataResult;
        HIS.Desktop.ADO.AssignServiceADO.DelegateProcessRefeshIcd processRefeshIcd;
        bool isInKip;
        bool isAssignInPttt;
        string patientName;
        long patientDob;
        string genderName;
        bool isAutoEnableEmergency;
        bool isPriority;
        string provisionalDiagnosis;

        HisTreatmentWithPatientTypeInfoSDO currentHisTreatment { get; set; }
        MOS.EFMODEL.DataModels.HIS_SERVICE_REQ serviceReqMain { get; set; }
        MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER currentHisPatientTypeAlter = null;
        List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> currentPatientTypeWithPatientTypeAlter = null;
        List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_6> currentPreServiceReqs;
        HisNoneMediServiceReqResultSDO serviceReqComboResultSDO { get; set; }

        HideCheckBoxHelper hideCheckBoxHelper__Service;
        BindingList<ServiceADO> records;
        List<SereServADO> ServiceIsleafADOs = new List<SereServADO>();
        List<ServiceADO> ServiceParentADOs;
        List<ServiceADO> ServiceParentADOForGridServices;
        List<ServiceADO> ServiceAllADOs;
        ServiceADO SereServADO__Main;

        List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> sereServWithTreatment = new List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>();
        List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> sereServsInTreatment = new List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>();
        List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> sereServsInTreatmentRaw = new List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>();
        Inventec.Desktop.Common.Modules.Module currentModule;

        Dictionary<long, List<V_HIS_SERVICE_PATY>> servicePatyInBranchs;
        Dictionary<long, V_HIS_SERVICE> dicServices;
        List<HIS_ICD_SERVICE> icdServicePhacDos { get; set; }
        List<L_HIS_ROOM_COUNTER_1> hisRoomCounters;

        bool isNotUseBhyt;

        decimal currentExpendInServicePackage = 0;
        bool isSaveAndPrint = false;

        int groupType__ServiceTypeName = 1;
        int groupType__PtttGroupName = 2;
        bool notSearch;

        bool isStopEventChangeMultiDate;
        bool IsObligatoryTranferMediOrg = false;
        bool IsAcceptWordNotInData = false;
        bool isAutoCheckIcd;
        string _TextIcdName = "";
        string _TextIcdNameCause = "";

        List<HIS_ICD> currentIcds;

        List<HIS_PATIENT_TYPE> currentPatientTypes;
        List<V_HIS_PATIENT_TYPE_ALLOW> currentPatientTypeAllows;
        HIS_DHST currentDhst;
        bool IscheckAllTreeService = false;
        bool isYes = false;
        decimal totalHeinByTreatment = 0;
        decimal totalHeinPriceByTreatment = 0;
        decimal totalHeinPriceByTreatmentBK = 0;
        List<HIS_ROOM_TIME> roomTimes;
        List<MOS.EFMODEL.DataModels.HIS_EXRO_ROOM> exroRooms;
        MOS.EFMODEL.DataModels.V_HIS_ROOM requestRoom;
        HIS_DEPARTMENT currentDepartment = null;
        long[] serviceTypeIdAllows;
        List<long> patientTypeIdAls;
        List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> currentExecuteRooms;
        List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> allDataExecuteRooms;
        List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_SAME> currentServiceSames;
        MOS.EFMODEL.DataModels.HIS_SERVICE_REQ icdExam;
        bool isNotHandlerWhileChangeToggetSwith;
        bool isHandlerResetServiceStateCheckeds;
        bool isHandlerResetServiceStateCheckedForTreeNodes;
        bool isProcessingAfternodeChecked;
        bool isNotEventByChangeServiceParent;
        bool isRunInitEventForGridServieProcess;
        bool isNotLoadWhileChangeInstructionTimeInFirst;
        bool isNotLoadWhileChangeControlStateInFirst;
        bool isUseTrackingInputWhileChangeTrackingTime;
        bool IsFirstloadConditionService;
        bool IsFirstloadForm;
        List<HIS_SERVICE_CONDITION> lstConditionService;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.AssignNoneMediService";
        List<V_HIS_ROOM> assRoomsWorks = null;
        long ContructorIntructionTime;

        SereServADO currentRowSereServADO;
        List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY> serviceConditions;
        int lastRowHandle = -1;
        ToolTipControlInfo lastInfo = null;
        GridColumn lastColumn = null;
        bool isInitTracking;
        HIS.Desktop.ADO.AssignServiceADO workingAssignServiceADO;

        V_HIS_TREATMENT_FEE treatmentPrint;
        V_HIS_PATIENT patientPrint;
        HIS_TREATMENT currentTreatment;

        List<SereServADO> serviceDeleteWhileSelectSeviceGroups;
        bool isShowContainerMediMaty = false;
        bool isShowContainerMediMatyForChoose = false;
        bool isShow = true;

        int popupHeight = 400;
        bool statecheckColumn;

        MOS.EFMODEL.DataModels.HIS_TRACKING tracking { get; set; }
        List<HIS_TRACKING> trackings;

        decimal transferTreatmentFeeBK = 0;
        decimal transferTreatmentFee = 0;
        HIS_PATIENT_TYPE patientTypeByPT;

        List<HIS.Desktop.Plugins.AssignNoneMediService.ADO.IcdADO> icdSubcodeAdoChecks;
        HIS.Desktop.Plugins.AssignNoneMediService.ADO.IcdADO subIcdPopupSelect;
        bool isNotProcessWhileChangedTextSubIcd;
        List<V_HIS_TREATMENT_BED_ROOM> TreatmentBedRooms = new List<V_HIS_TREATMENT_BED_ROOM>();
        bool IsTreatmentInBedRoom;
        V_HIS_ROOM currentWorkingRoom = null;

        string PatientKskCode = null;
        HIS_SERE_SERV hisSereServForGetPatientType = null;
        List<V_HIS_SERVICE> lstService = null;

        private Dictionary<string, string> dicValidIcd = new Dictionary<string, string>();
        Dictionary<long, List<HIS_PATIENT_TYPE>> dicPatientType = new Dictionary<long, List<HIS_PATIENT_TYPE>>();
        public enum TypeButton
        {
            SAVE,
            SAVE_AND_PRINT,
            EDIT
        }
        List<string> ListMessError = new List<string>();
        private List<string> lstModuleLinkApply;
        private bool IsFirstLoad = false;
        bool IsActionKey = false;
        Dictionary<long?, List<string>> dicMaxAmount = new Dictionary<long?, List<string>>();
        Dictionary<long, HisServiceReqListResultSDO> dicServiceReqList = new Dictionary<long, HisServiceReqListResultSDO>();
        bool assignMulti = false;

        Dictionary<long, string> dicSessionCode = new Dictionary<long, string>();
        DateTime dteCommonParam { get; set; }
        List<long> serviceTypeIdRequired { get; set; }
        int indexServiceType = 0;

        internal IcdProcessor icdYhctProcessor { get; set; }
        internal UserControl ucIcdYhct { get; set; }
        internal SecondaryIcdProcessor subIcdYhctProcessor { get; set; }
        internal UserControl ucSecondaryIcdYhct { get; set; }
        List<HIS_PATIENT_TYPE_ROOM> PatientTypeRooms { get; set; }
        #endregion

        #region Construct

        public frmAssignService(Inventec.Desktop.Common.Modules.Module module, HIS.Desktop.ADO.AssignServiceADO dataADO)
            : base(module)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("frmAssignService.Init .1");
                InitializeComponent();
                try
                {
                    //string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    //this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }

                this.actionType = GlobalVariables.ActionAdd;
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.AssignNoneMediService.Resources.Lang", typeof(HIS.Desktop.Plugins.AssignNoneMediService.AssignService.frmAssignService).Assembly);
                this.currentModule = module;
                HisConfigCFG.LoadConfig();
                this.InitUC();
                this.workingAssignServiceADO = dataADO;
                if (dataADO != null)
                {
                    this.processDataResult = dataADO.DgProcessDataResult;
                    this.processRefeshIcd = dataADO.DgProcessRefeshIcd;
                    this.treatmentId = dataADO.TreatmentId;
                    this.previusTreatmentId = dataADO.PreviusTreatmentId;
                    this.serviceReqParentId = dataADO.ServiceReqId;
                    this.isInKip = dataADO.IsInKip;
                    this.isAssignInPttt = dataADO.IsAssignInPttt;
                    if (this.isInKip)
                        this.currentSereServInEkip = dataADO.SereServ;
                    else
                        this.currentSereServ = dataADO.SereServ;

                    this.provisionalDiagnosis = dataADO.ProvisionalDiagnosis;
                    this.icdExam = dataADO.IcdExam;
                    this.patientName = dataADO.PatientName;
                    this.patientDob = dataADO.PatientDob;
                    this.genderName = dataADO.GenderName;
                    this.currentDhst = dataADO.Dhst;
                    this.isAutoEnableEmergency = dataADO.IsAutoEnableEmergency;
                    this.isPriority = dataADO.IsPriority;
                    this.tracking = dataADO.Tracking;
                    this.ContructorIntructionTime = dataADO.IntructionTime;
                    this.examRegisterRoomId = dataADO.ExamRegisterRoomId;
                    this.isNotUseBhyt = dataADO.IsNotUseBhyt.HasValue && dataADO.IsNotUseBhyt.Value;
                }

                if (this.currentModule != null)
                {
                    currentWorkingRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                }
                //Inventec.Common.Logging.LogSystem.Info("frmAssignService.Init .2___sereServsInTreatmentRaw.count=" + (sereServsInTreatmentRaw != null ? sereServsInTreatmentRaw.Count : 0));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        #region Private method
        private void LoadSource()
        {

            try
            {
                CommonParam commonParam = new CommonParam();
                HisNoneMediServiceFilter filter = new HisNoneMediServiceFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var apiResult = new BackendAdapter(commonParam).Get<List<MOS.EFMODEL.DataModels.HIS_NONE_MEDI_SERVICE>>("api/HisNoneMediService/Get", ApiConsumers.MosConsumer, filter, commonParam);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiResult), apiResult));
                if (apiResult != null)
                {
                    foreach (var item in apiResult)
                    {
                        ServiceIsleafADOs.Add(new SereServADO() { TDL_SERVICE_CODE = item.NONE_MEDI_SERVICE_CODE, TDL_SERVICE_NAME = item.NONE_MEDI_SERVICE_NAME, AMOUNT = 1, ACTUAL_PRICE = item.PRICE ?? 0, HEIN_RATIO = (item.VAT_RATIO ?? 0) * 100, ID = item.ID, SERVICE_UNIT_NAME = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(o=> o.ID == item.SERVICE_UNIT_ID).SERVICE_UNIT_NAME  });
                    }
                }
                ServiceIsleafADOs = ServiceIsleafADOs.OrderBy(o => o.TDL_SERVICE_CODE).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                ado.LabelTextSize = 90;
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
                ado.Width = 440;
                ado.TextSize = 90;
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


        bool isCheckAssignServiceSimultaneityOption = false;
        bool isCallingApi = false;
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


        #region Load data
        private void InitUC()
        {
            try
            {
                this.isAutoCheckIcd = (HisConfigCFG.AutoCheckIcd == GlobalVariables.CommonStringTrue);
                this.currentIcds = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).OrderBy(o => o.ICD_CODE).ToList();

                UCIcdInit();
                UcDateInit();
                V_HIS_ROOM currentRoom = null;

                if (this.currentModule != null && this.currentModule.RoomId > 0)
                {
                    currentRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                }
                if (HisConfigCFG.ObligateIcd == GlobalVariables.CommonStringTrue && (currentRoom != null && currentRoom.IS_ALLOW_NO_ICD != 1))
                {
                    ValidationICD(10, 500, true);
                }
                else
                {
                    ValidationSingleControlWithMaxLength(txtIcdCode, false, 10);
                    ValidationSingleControlWithMaxLength(txtIcdMainText, false, 500);
                }

                InitControlState();

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
                isNotLoadWhileChangeControlStateInFirst = true;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstant.chkPrint)
                        {
                            chkPrint.Checked = item.VALUE == "1";
                            if (chkPrint.Checked)
                                chkPrintDocumentSigned.Checked = false;
                        }
                        else if (item.KEY == ControlStateConstant.chkSign)
                        {
                            chkSign.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstant.chkPrintDocumentSigned)
                        {
                            chkPrintDocumentSigned.Checked = item.VALUE == "1";
                            if (chkPrintDocumentSigned.Checked)
                                chkPrint.Checked = false;
                        }
                    }
                }
                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmAssignService_Load(object sender, EventArgs e)
        {
            try
            {
                LogSystem.Debug("frmAssignService_Load => Starting...");
                currentTreatment = GetTreatment(treatmentId);
                LoadHisServiceFromRam();
                this.IsFirstloadForm = true;
                this.isInitTracking = true;
                this.requestRoom = GetRequestRoom(this.currentModule.RoomId);
                this.IsFirstloadConditionService = true;
                this.isNotLoadWhileChangeInstructionTimeInFirst = true;
                gridViewServiceProcess.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;//ẩn panel filter editor mặc định của grid khi gõ tìm kiếm ở các ô
                this.LoadIcdDefault();
                InitUcIcdYhct();
                InitUcSecondaryIcdYhct();
                this.SetDefaultData(true);
                this.ProcessInitEventForGridServieProcess();

                this.LoadDataToAssignRoom();
                if (this.treatmentId > 0)
                {
                    this.FillAllPatientInfoSelectedInForm();
                    DateTime intructTime = (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.intructionTimeSelecteds.First()) ?? DateTime.Now);
                    this.LoadTotalSereServByHeinWithTreatmentAsync(this.treatmentId);
                    this.InitComboUser();
                    this.LoadTreatmentInfo__PatientType();
                    IsFirstLoad = true;
                    this.InitDefaultFocus();
                }
                LoadSource();
                this.gridControlServiceProcess.DataSource = null;
                this.gridControlServiceProcess.DataSource = ServiceIsleafADOs;
                this.gridControlServiceProcess.ToolTipController = this.tooltipService;
                this.isNotLoadWhileChangeInstructionTimeInFirst = false;
                this.AddBarManager(this.barManager1);

                this.SetEnableButtonControl(this.actionType);
                this.InitCheckIcdManager();
                ModuleList();
                IsFirstloadForm = false;

                string configValue = HisConfigCFG.IsAllowSignaturePrint;

                if (!string.IsNullOrWhiteSpace(configValue))
                {
                    var allowedModules = configValue
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .ToList();

                    if (allowedModules.Contains("HIS.Desktop.Plugins.AssignNoneMediService"))
                    {
                        layoutControlItem18.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                    else
                    {
                        layoutControlItem18.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    }
                }
                else
                {
                    layoutControlItem18.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
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

        private void timerInitForm_Tick(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        private void FillAllPatientInfoSelectedInForm()
        {
            try
            {
                LogSystem.Debug("FillAllPatientInfoSelectedInForm => 1");
                this.LoadDataToCurrentTreatmentData(treatmentId, this.intructionTimeSelecteds.FirstOrDefault());
                this.SetDateUc();
                LogSystem.Debug("FillAllPatientInfoSelectedInForm => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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


        #endregion

        #region Control editor

        private void toggleSwitchDataChecked_Toggled(object sender, EventArgs e)
        {
            try
            {
                lblCaptionFortoggleSwitchDataChecked.Text = ((this.toggleSwitchDataChecked.EditValue ?? "").ToString().ToLower() != "true") ? toggleSwitchDataChecked.Properties.OffText : toggleSwitchDataChecked.Properties.OnText;
                if (isNotHandlerWhileChangeToggetSwith)
                {
                    //isNotHandlerWhileChangeToggetSwith = false;
                    return;
                }

                //Inventec.Common.Logging.LogSystem.Debug("toggleSwitchDataChecked_Toggled. 1");
                ToggleSwitch toggleSwitch = sender as ToggleSwitch;
                if (toggleSwitch != null)
                {
                    //Inventec.Common.Logging.LogSystem.Debug("toggleSwitchDataChecked_Toggled. 2");
                    WaitingManager.Show();
                    this.LoadDataToGrid(false);
                    WaitingManager.Hide();
                    //Inventec.Common.Logging.LogSystem.Debug("toggleSwitchDataChecked_Toggled. 3");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetOneService(SereServADO item)
        {
            try
            {
                item.AMOUNT = 1;
                item.PRICE = 0;
                item.VAT_RATIO = 0;
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

        private void gridViewServiceProcess_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    SereServADO data = (SereServADO)gridViewServiceProcess.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "IsChecked")
                    {
                        e.RepositoryItem = this.repositoryItemchkIsChecked;
                    }
                    else if (e.Column.FieldName == "AMOUNT")
                    {
                        if (data.IsChecked)
                            e.RepositoryItem = this.repSpin;
                        else
                            e.RepositoryItem = this.repSpinDis;

                    }
                    else if (e.Column.FieldName == "PRICE")
                    {
                        if (data.IsChecked)
                            e.RepositoryItem = this.repSpinPrice;
                        else
                            e.RepositoryItem = this.repSpinPriceDis;

                    }
                    else if (e.Column.FieldName == "VAT_RATIO")
                    {
                        if (data.IsChecked)
                            e.RepositoryItem = this.repSpinVat;
                        else
                            e.RepositoryItem = this.repSpinVatDis;

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
                }
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

        private void gridViewServiceProcess_ColumnFilterChanged(object sender, EventArgs e)
        {
            try
            {
                if (IsClosingForm)
                    return;
                if ((
                   gridViewServiceProcess.FocusedColumn == grcServiceCode_TabService ||
                   gridViewServiceProcess.FocusedColumn == grcServiceName_TabService)
                   && !string.IsNullOrEmpty(gridViewServiceProcess.GetFocusedDisplayText())
                   && gridViewServiceProcess.FocusedRowHandle == DevExpress.XtraGrid.GridControl.AutoFilterRowHandle)
                {
                    toggleSwitchDataChecked.IsOn = false;
                }

                if (gridViewServiceProcess.RowCount == 2)
                {
                    var sereServADO = (SereServADO)this.gridViewServiceProcess.GetRow(0);
                    if (sereServADO != null)
                    {
                        sereServADO.IsChecked = true;

                        this.gridControlServiceProcess.RefreshDataSource();
                        this.SetEnableButtonControl(this.actionType);

                        gridViewServiceProcess.ActiveEditor.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void gridViewServiceProcess_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.gridViewServiceProcess.FocusedRowHandle = GridControl.AutoFilterRowHandle;

                    if (this.gridViewServiceProcess.FocusedColumn != this.grcServiceCode_TabService
                        && this.gridViewServiceProcess.FocusedColumn != this.grcServiceName_TabService)
                    {
                        this.gridViewServiceProcess.FocusedColumn = this.grcServiceCode_TabService;
                        this.gridViewServiceProcess.ClearColumnsFilter();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewServiceProcess_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                if (this.currentHisPatientTypeAlter != null
                        && this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                {
                    var index = this.gridViewServiceProcess.GetDataSourceRowIndex(e.RowHandle);
                    if (index < 0) return;

                    var listDatas = this.gridControlServiceProcess.DataSource as List<SereServADO>;
                    var dataRow = listDatas[index];
                    if (dataRow != null && dataRow.PATIENT_TYPE_ID > 0
                        && dataRow.PATIENT_TYPE_ID != HisConfigCFG.PatientTypeId__BHYT)
                    {
                        //Đối tượng điều trị là BHYT, nhưng do ko có chính sách giá theo BHYT nên khi tích chọn dịch vụ, sẽ hiển thị màu tím.
                        //Có chính sách giá nhưng là đối tượng khác, không phải BHYT ==> màu tím

                        var bFindservice = (BranchDataWorker.DicServicePatyInBranch != null
                            && BranchDataWorker.DicServicePatyInBranch.ContainsKey(dataRow.SERVICE_ID)) ? BranchDataWorker.HasServicePatyWithListPatientType(dataRow.SERVICE_ID, new List<long>() { this.currentHisPatientTypeAlter.PATIENT_TYPE_ID }) : false;
                        if (!bFindservice)
                            e.Appearance.ForeColor = System.Drawing.Color.Violet;
                    }

                    if (dataRow != null)
                    {
                        var bFindservice = !String.IsNullOrWhiteSpace(dataRow.TDL_HEIN_SERVICE_BHYT_CODE) && (BranchDataWorker.DicServicePatyInBranch != null
                           && BranchDataWorker.DicServicePatyInBranch.ContainsKey(dataRow.SERVICE_ID)) ? BranchDataWorker.HasServicePatyWithListPatientType(dataRow.SERVICE_ID, new List<long>() { HisConfigCFG.PatientTypeId__BHYT }) : false;
                        if (bFindservice && !String.IsNullOrEmpty(HisConfigCFG.BhytColorCode))
                        {
                            e.Appearance.ForeColor = GetColor(HisConfigCFG.BhytColorCode);
                        }
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

        private void tooltipService_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
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

        private void SetEnableButtonControl(int actionType)
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionAdd && serviceReqComboResultSDO == null)
                {
                    List<SereServADO> serviceCheckeds__Send = this.ServiceIsleafADOs.FindAll(o => o.IsChecked);
                    this.btnSave.Enabled = this.btnSaveAndPrint.Enabled = (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0);

                }
                else
                {
                    this.btnSave.Enabled = this.btnSaveAndPrint.Enabled = false;
                }
                BtnPrint.Enabled = serviceReqComboResultSDO != null;

                //hiển thị ảnh checkbox
                List<SereServADO> data = null;
                if (gridViewServiceProcess.DataSource != null)
                {
                    data = (List<SereServADO>)gridViewServiceProcess.DataSource;
                }

                if (data != null && data.Count == data.Count(o => o.IsChecked) && this.imageCollection1.Images.Count > 1)
                {
                    this.grcChecked_TabService.Image = this.imageCollection1.Images[1];
                }
                else if (data != null && data.Exists(o => o.IsChecked) && this.imageCollection1.Images.Count > 2)
                {
                    this.grcChecked_TabService.Image = this.imageCollection1.Images[2];
                }
                else
                {
                    this.grcChecked_TabService.Image = this.imageCollection1.Images[0];
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
                        this.FocusShowpopup(cboUser, true);
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>()
                            .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME.ToUpper().Contains(searchCode.ToUpper())).ToList();

                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.LOGINNAME.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboUser.EditValue = searchResult[0].LOGINNAME;
                            this.txtLoginName.Text = searchResult[0].LOGINNAME;
                            this.FocusWhileSelectedUser();
                        }
                        else
                        {
                            this.cboUser.EditValue = null;
                            this.FocusShowpopup(cboUser, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusWhileSelectedUser()
        {
            try
            {
                this.gridControlServiceProcess.Focus();
                this.gridViewServiceProcess.FocusedRowHandle = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboUser_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboUser.EditValue != null)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME == ((this.cboUser.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            this.txtLoginName.Text = data.LOGINNAME;
                        }
                    }
                    this.FocusWhileSelectedUser();
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
                        ACS.EFMODEL.DataModels.ACS_USER data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME == ((this.cboUser.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            this.FocusWhileSelectedUser();
                            this.txtLoginName.Text = data.LOGINNAME;
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

        private void DelegateSelectMultiDate(List<DateTime?> datas, DateTime time)
        {
            try
            {
                this.intructionTimeSelecteds = (this.intructionTimeSelected.Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o.Value.ToString("yyyyMMdd") + timeSelested.ToString("HHmm") + "00")).OrderByDescending(o => o).ToList());
                this.isMultiDateState = chkMultiIntructionTime.Checked;

                ChangeIntructionTime(time);
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
                if (HisConfigCFG.IsUsingServerTime == commonString__true
                    && this.currentHisTreatment != null)
                {
                    return;
                }
                LogSystem.Debug("ChangeIntructionTime => 1");
                this.LoadDataToCurrentTreatmentData(treatmentId, this.intructionTimeSelecteds.FirstOrDefault());
                this.SetDateUc();
                this.LoadTreatmentInfo__PatientType();
                this.notSearch = false;
                LogSystem.Debug("ChangeIntructionTime => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private Rectangle GetClientRectangle(Control control, ref int heightPlus)
        {
            Rectangle bounds = default(Rectangle);
            if (control != null)
            {
                bounds = control.Bounds;
                if (control.Parent != null && !(control is UserControl))
                {
                    heightPlus += bounds.Y;
                    return GetClientRectangle(control.Parent, ref heightPlus);
                }
            }
            return bounds;
        }

        private Rectangle GetAllClientRectangle(Control control, ref int heightPlus)
        {
            Rectangle bounds = default(Rectangle);
            if (control != null)
            {
                bounds = control.Bounds;
                if (control.Parent != null)
                {
                    heightPlus += bounds.Y;
                    return GetAllClientRectangle(control.Parent, ref heightPlus);
                }
            }
            return bounds;
        }
        private void txtDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtLoginName.Focus();
                    txtLoginName.SelectAll();
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
                    chkPrintDocumentSigned.Checked = !chkPrint.Checked;
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
                //chkPrintDocumentSigned.Enabled = chkSign.Checked;
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                //if (chkSign.Checked == false)
                //{
                //    chkPrintDocumentSigned.Checked = false;
                //}

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.chkSign && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkSign.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.chkSign;
                    csAddOrUpdate.VALUE = (chkSign.Checked ? "1" : "");
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

        private void chkPrintDocumentSigned_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                isNotLoadWhileChangeControlStateInFirst = true;
                if (chkPrintDocumentSigned.Checked)
                    chkPrint.Checked = !chkPrintDocumentSigned.Checked;
                isNotLoadWhileChangeControlStateInFirst = false;
                ChangeCheckPrintAndPreview();
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
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdatePrintDocumentSigned = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.chkPrintDocumentSigned && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdatePrintDocumentSigned != null)
                {
                    csAddOrUpdatePrintDocumentSigned.VALUE = (chkPrintDocumentSigned.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdatePrintDocumentSigned = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdatePrintDocumentSigned.KEY = ControlStateConstant.chkPrintDocumentSigned;
                    csAddOrUpdatePrintDocumentSigned.VALUE = (chkPrintDocumentSigned.Checked ? "1" : "");
                    csAddOrUpdatePrintDocumentSigned.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdatePrintDocumentSigned);
                }

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.chkPrint && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrint.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.chkPrint;
                    csAddOrUpdate.VALUE = (chkPrint.Checked ? "1" : "");
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

        private void cboAssignRoom_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.cboAssignRoom.EditValue = null;
                    this.cboAssignRoom.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAssignRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboAssignRoom.EditValue != null)
                    {
                        var data = this.assRoomsWorks.FirstOrDefault(o => o.ID == (long)this.cboAssignRoom.EditValue);
                        if (data != null)
                        {
                            this.cboAssignRoom.Properties.Buttons[1].Visible = true;
                            txtAssignRoomCode.Text = data.ROOM_CODE;
                        }
                    }
                    this.txtLoginName.Focus();
                    this.txtLoginName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAssignRoom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboAssignRoom.EditValue != null)
                    {
                        var data = this.assRoomsWorks.FirstOrDefault(o => o.ID == (long)this.cboAssignRoom.EditValue);
                        if (data != null)
                        {
                            this.cboAssignRoom.Properties.Buttons[1].Visible = true;
                            txtAssignRoomCode.Text = data.ROOM_CODE;

                            this.txtLoginName.Focus();
                            this.txtLoginName.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAssignRoomCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        this.cboAssignRoom.EditValue = null;
                        this.cboAssignRoom.Properties.Buttons[1].Visible = false;
                        this.FocusShowpopup(cboAssignRoom, true);
                    }
                    else
                    {
                        var data = this.assRoomsWorks
                            .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.ROOM_CODE.ToUpper().Contains(searchCode.ToUpper())).ToList();

                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.ROOM_CODE.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboAssignRoom.EditValue = searchResult[0].ID;
                            this.cboAssignRoom.Properties.Buttons[1].Visible = true;
                            this.txtAssignRoomCode.Text = searchResult[0].ROOM_CODE;
                            this.txtLoginName.Focus();
                            this.txtLoginName.SelectAll();
                        }
                        else
                        {
                            this.cboAssignRoom.EditValue = null;
                            this.cboAssignRoom.Properties.Buttons[1].Visible = false;
                            this.FocusShowpopup(cboAssignRoom, true);
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

        #region Button
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveWithGridpatientSelect(TypeButton.SAVE, chkPrint.Checked, false, false, chkSign.Checked, chkPrintDocumentSigned.Checked);
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
        private void SaveWithGridpatientSelect(TypeButton type, bool isSaveAndPrint, bool printTH, bool isSaveAndShow, bool isSign = false, bool isPrintDocumentSigned = false)
        {
            try
            {
                assignMulti = false;
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

        private void btnSaveAndPrint_Click(object sender, EventArgs e)
        {
            try
            {
                SaveWithGridpatientSelect(TypeButton.SAVE_AND_PRINT, true, false, false);
                //if (LblBtnPrint.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && BtnPrint.Enabled)
                //{
                //    PrintServiceReqProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(serviceReqComboResultSDO, currentHisTreatment, null, currentModule != null ? currentModule.RoomId : 0);
                //    InPhieuYeuCauDichVu(true);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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

        private void barbtnSaveShortcut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.btnSave.Enabled)
                    this.btnSave_Click(null, null);
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
                if (this.btnSaveAndPrint.Enabled)
                    this.btnSaveAndPrint_Click(null, null);
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

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.isCheckAssignServiceSimultaneityOption = false;
                this.SetDefaultData();
                this.LoadIcdDefault();
                foreach (var item in this.ServiceIsleafADOs)
                {
                    item.AssignNumOrder = null;
                    item.PRICE = 0;
                    item.VAT_RATIO = 0;
                    item.AMOUNT = 1;
                    item.IsChecked = false;
                    item.ShareCount = null;
                    item.PATIENT_TYPE_ID = 0;
                    item.PATIENT_TYPE_CODE = "";
                    item.PATIENT_TYPE_NAME = "";
                    item.TDL_EXECUTE_ROOM_ID = 0;
                    item.IsExpend = false;
                    item.IsOutKtcFee = false;
                    item.IsKHBHYT = false;
                    item.InstructionNote = "";
                    item.SERVICE_GROUP_ID_SELECTEDs = null;
                    item.SERVICE_CONDITION_ID = null;
                    item.SERVICE_CONDITION_NAME = "";
                    item.AssignPackagePriceEdit = null;
                    item.AssignSurgPriceEdit = null;
                    item.IsNoDifference = false;
                    item.ErrorMessageAmount = "";
                    item.ErrorMessageIsAssignDay = "";
                    item.ErrorMessagePatientTypeId = "";
                    item.ErrorTypeAmount = ErrorType.None;
                    item.ErrorTypeIsAssignDay = ErrorType.None;
                    item.ErrorTypePatientTypeId = ErrorType.None;
                    item.PRIMARY_PATIENT_TYPE_ID = null;
                    item.IsNotChangePrimaryPaty = false;
                    item.PackagePriceId = null;
                    item.SERVICE_CONDITION_ID = null;
                    item.SERVICE_CONDITION_NAME = null;

                    item.OTHER_PAY_SOURCE_ID = null;
                    item.OTHER_PAY_SOURCE_CODE = null;
                    item.OTHER_PAY_SOURCE_NAME = null;
                    item.BedFinishTime = null;
                    item.BedId = null;
                    item.BedStartTime = null;
                    item.TEST_SAMPLE_TYPE_ID = 0;
                    item.TEST_SAMPLE_TYPE_CODE = null;
                    item.TEST_SAMPLE_TYPE_NAME = null;
                    item.SereServEkipADO = null;
                    item.NumberOfTimes = 1;
                }

                this.isNotHandlerWhileChangeToggetSwith = true;
                if ((this.toggleSwitchDataChecked.EditValue ?? "").ToString().ToLower() == "true")
                    this.toggleSwitchDataChecked.EditValue = false;
                this.isNotHandlerWhileChangeToggetSwith = false;

                this.gridControlServiceProcess.DataSource = null;

                this.gridControlServiceProcess.DataSource = this.ServiceIsleafADOs;
                this.gridControlServiceProcess.RefreshDataSource();
                this.gridViewServiceProcess.ClearColumnsFilter();
                this.SetEnableButtonControl(this.actionType);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void barbtnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnShowDetail_Click(object sender, EventArgs e)
        {
            try
            {
                frmDetail frmDetail = new frmDetail(this.serviceReqComboResultSDO, currentHisTreatment, this.currentModule);
                frmDetail.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BarSavePrint_PrintTH_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //try
            //{
            //    if (LciBtnSavePrintNPrintTH.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && this.BtnSavePrintNPrintTH.Enabled)
            //        BtnSavePrintNPrintTH_Click(null, null);
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }


        private void barBtnSaveNShow_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //if (LciBtnSaveNShow.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && this.BtnSaveNShow.Enabled)
                //{
                //    BtnSaveNShow_Click(null, null);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (LblBtnPrint.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && BtnPrint.Enabled)
                {
                    MPS.ProcessorBase.PrintConfig.PreviewType previewType = MPS.ProcessorBase.PrintConfig.PreviewType.Show;
                    if (chkSign.Checked)
                    {
                        if (isSaveAndPrint)
                        {
                            previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow;
                        }
                        else if (chkPrintDocumentSigned.Checked)
                        {
                            previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintPreview;
                        }
                        else
                            previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow;
                    }
                    else
                    {
                        if (isSaveAndPrint)
                        {
                            previewType = MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow;
                        }
                        else if (chkPrintDocumentSigned.Checked)
                        {
                            previewType = MPS.ProcessorBase.PrintConfig.PreviewType.Show;
                        }
                    }
                    var proc = new HIS.Desktop.Plugins.Library.PrintServiceReq.PrintServiceReqProcessor(this.serviceReqComboResultSDO, this.currentHisTreatment, previewType);
                    proc.InPhieuChiDinhNgoaiKhamBenh("Mps000502");
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void txtIcdSubCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtIcdSubCode.Text))
                    {
                        if (!ProccessorByIcdCode((sender as DevExpress.XtraEditors.TextEdit).Text.Trim()))
                        {
                            e.Handled = true;
                        }
                        else
                        {
                            ReloadIcdSubContainerByCodeChanged();
                            txtIcdText.Focus();
                            txtIcdText.SelectionStart = txtIcdText.Text.Length;
                            txtIcdText.SelectAll();
                        }
                    }
                    else
                    {

                        txtIcdText.Focus();

                    }
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
                    isShowContainerMediMaty = false;
                    isShowContainerMediMatyForChoose = true;
                    popupControlContainerSubIcdName.HidePopup();

                }
                else if (e.KeyCode == Keys.Down)
                {
                    int rowHandlerNext = 0;
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
                txtIcdSubCode.Text = icdCode;
                txtIcdText.Text = icdName;
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
                                var checkInList = this.currentIcds.Where(o => o.IS_TRADITIONAL != 1 &&
                                    IcdUtil.AddSeperateToKey(item).Equals(IcdUtil.AddSeperateToKey(o.ICD_NAME))).FirstOrDefault();
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
                            var icdByCode = this.currentIcds.Where(o => o.IS_TRADITIONAL != 1).ToList().FirstOrDefault(o => o.ICD_CODE.ToLower() == itemCode.ToLower());
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


                            //MessageManager.Show(String.Format(Resources.ResourceMessage.KhongTimThayIcdTuongUngVoiCacMaSau, strWrongIcdCodes));
                            //int startPositionWarm = 0;
                            //int lenghtPositionWarm = this.txtIcdSubCode.Text.Length - 1;
                            //if (arrWrongCodes != null && arrWrongCodes.Count > 0)
                            //{
                            //    startPositionWarm = this.txtIcdSubCode.Text.IndexOf(arrWrongCodes[0]);
                            //    lenghtPositionWarm = arrWrongCodes[0].Length;
                            //}
                            //this.txtIcdSubCode.Focus();
                            //this.txtIcdSubCode.Select(startPositionWarm, lenghtPositionWarm);
                            //valid = false;
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
                    Inventec.Common.Logging.LogSystem.Debug("Ma icd nhap vao khong ton tai trong danh muc. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => strWrongIcdCodes), strWrongIcdCodes));
                }
                //this.SetCheckedIcdsToControl(this.txtIcdSubCode.Text, strIcdNames);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
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
                popupControlContainerSubIcdName.ShowPopup(new Point(buttonBounds.X + 300, buttonBounds.Bottom + 22));
                Inventec.Common.Logging.LogSystem.Debug("buttonBounds.X + 300=" + buttonBounds.X + 300 + ", buttonBounds.Bottom + 22=" + (buttonBounds.Bottom + 22));
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
                //if (!String.IsNullOrEmpty(strIcdSubText))
                //{
                //    txtIcdText.Text = newtxtIcdText.Substring(0, newtxtIcdText.LastIndexOf(IcdUtil.seperator + strIcdSubText + IcdUtil.seperator) + 1);
                //}
                //if (icdNames.Equals(IcdUtil.seperator))
                //{
                //    txtIcdText.Text = "";
                //}
                //if (icdCodes.Equals(IcdUtil.seperator))
                //{
                //    txtIcdSubCode.Text = "";
                //}
                this.isNotProcessWhileChangedTextSubIcd = false;
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
                    this.subIcdPopupSelect = this.customGridViewSubIcdName.GetFocusedRow() as HIS.Desktop.Plugins.AssignNoneMediService.ADO.IcdADO;
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
                this.subIcdPopupSelect = this.customGridViewSubIcdName.GetFocusedRow() as HIS.Desktop.Plugins.AssignNoneMediService.ADO.IcdADO;
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
                        string messErr = null;
                        if (!checkIcdManager.ProcessCheckIcd(txtIcdCode.Text.Trim(), txtIcdSubCode.Text.Trim(), ref messErr, false))
                        {
                            XtraMessageBox.Show(messErr, "Thông báo", MessageBoxButtons.OK);
                            if (Desktop.Plugins.Library.CheckIcd.CheckIcdManager.IcdCodeError.Equals(txtIcdCode.Text))
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

                cboIcds.Properties.Buttons[1].Visible = true;
                if (currentIcds != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("currentIcds count " + currentIcds.Count);
                }

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
                        if (Desktop.Plugins.Library.CheckIcd.CheckIcdManager.IcdCodeError.Equals(txtIcdCode.Text))
                        {
                            txtIcdCode.Text = txtIcdMainText.Text = null;
                            cboIcds.EditValue = null;
                        }
                        return;
                    }
                    if (chkEditIcd.Checked)
                    {

                    }
                    else if (chkEditIcd.Enabled)
                    {
                        chkEditIcd.Focus();
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
                    //if (!cboIcds.Properties.Buttons[1].Visible)
                    //    return;
                    this._TextIcdName = "";
                    cboIcds.EditValue = null;
                    txtIcdCode.Text = "";
                    txtIcdMainText.Text = "";
                    cboIcds.Properties.Buttons[1].Visible = false;
                    txtIcdCode.ErrorText = "";
                    dxValidationProviderControl.RemoveControlError(txtIcdCode);
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
                HIS_ICD icd = null;
                if (this.cboIcds.EditValue != null)
                    icd = this.currentIcds.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboIcds.EditValue.ToString()));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                }
                if (HisConfigCFG.IsCheckDepartmentInTimeWhenPresOrAssign)
                {
                    this.intructionTimeSelected = new List<DateTime?>();
                    this.InstructionTime = Inventec.Common.TypeConvert.Parse.ToInt64(this.dtInstructionTime.DateTime.ToString("yyyyMMdd") + this.timeSelested.ToString("HHmm") + "00");
                    this.intructionTimeSelected.Add(Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.InstructionTime));
                    this.intructionTimeSelecteds = new List<long>();
                    this.intructionTimeSelecteds.Add(this.InstructionTime);
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
                    NextForcusUCDate();
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

                this.timeIntruction.EditValue = DateTime.Now.ToString("HH:mm");
                if (this.chkMultiIntructionTime.Checked)
                {
                    string strTimeDisplay = DateTime.Now.ToString("dd/MM");
                    this.txtInstructionTime.Text = strTimeDisplay;
                }
                else
                {
                    this.dtInstructionTime.EditValue = DateTime.Now;
                }
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
                    this.ChangeIntructionTimeEditor(this.dtInstructionTime.DateTime);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void cboAssignRoom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView7_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void bbtnEditCtrlU_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
        }

        bool IsClosingForm = false;
        private void frmAssignService_FormClosing(object sender, FormClosingEventArgs e)
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
            }
            return data;
        }
        private void gridControlServiceProcess_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void gridView14_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (((IList)((BaseView)sender).DataSource) != null && ((IList)((BaseView)sender).DataSource).Count > 0)
                    {
                        V_HIS_SERVICE_REQ_6 oneServiceSDO = (V_HIS_SERVICE_REQ_6)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                        if (oneServiceSDO != null)
                        {
                            if (e.Column.FieldName == "INTRUCTION_TIME_str")
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(oneServiceSDO.INTRUCTION_TIME);
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
        V_HIS_SERVICE_REQ_6 rowdataAssignOld { get; set; }
        private void gridView14_RowCellClick(object sender, RowCellClickEventArgs e)
        {

            try
            {
                rowdataAssignOld = (MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_6)gridView14.GetFocusedRow();

                if (rowdataAssignOld != null)
                {
                    this.btnSave.Focus();
                }
                popupControlContainer3.HidePopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        #endregion

        private void repSpin_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                var sereServADO = (SereServADO)this.gridViewServiceProcess.GetFocusedRow();
                var spn = sender as SpinEdit;
                if (spn != null)
                {
                    sereServADO.AMOUNT = spn.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repSpinPrice_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                var sereServADO = (SereServADO)this.gridViewServiceProcess.GetFocusedRow();
                var spn = sender as SpinEdit;
                if (spn != null)
                {
                    sereServADO.PRICE = spn.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repSpinVat_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                var sereServADO = (SereServADO)this.gridViewServiceProcess.GetFocusedRow();
                var spn = sender as SpinEdit;
                if (spn != null)
                {
                    sereServADO.VAT_RATIO = spn.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridControlServiceProcess_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                GridHitInfo hitInfo = gridViewServiceProcess.CalcHitInfo(e.Location);

                if (hitInfo.InColumn && hitInfo.Column == grcChecked_TabService)
                {
                    if(grcChecked_TabService.Image != imageCollection1.Images[1])
                        ServiceIsleafADOs.ForEach(o => { o.IsChecked = true; o.PRICE = o.ACTUAL_PRICE ?? 0; o.VAT_RATIO = o.HEIN_RATIO ?? 0; });
                    else if (grcChecked_TabService.Image == imageCollection1.Images[1])
                        ServiceIsleafADOs.ForEach(o => { o.IsChecked = false; o.PRICE = 0; o.VAT_RATIO = 0;  });
                    this.gridControlServiceProcess.RefreshDataSource();
                    this.SetEnableButtonControl(this.actionType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void repositoryItemchkIsChecked_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var sereServADO = ServiceIsleafADOs.FirstOrDefault(o=>o.ID == ((SereServADO)this.gridViewServiceProcess.GetFocusedRow()).ID);
                CheckEdit chk = sender as CheckEdit;
                sereServADO.IsChecked = chk.Checked;
                if (!sereServADO.IsChecked)
                {
                    ResetOneService(sereServADO);
                }
                else
                {
                    sereServADO.PRICE = sereServADO.ACTUAL_PRICE ?? 0;
                    sereServADO.VAT_RATIO = sereServADO.HEIN_RATIO ?? 0;
                }
                this.gridControlServiceProcess.RefreshDataSource();
                this.SetEnableButtonControl(this.actionType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

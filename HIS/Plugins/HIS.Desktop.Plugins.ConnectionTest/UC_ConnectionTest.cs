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
using LIS.EFMODEL.DataModels;
using DevExpress.XtraGrid.Columns;
using DevExpress.Utils;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using DevExpress.XtraGrid.Views.Grid;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.Controls.Session;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.ConfigApplication;
using MOS.Filter;
using Inventec.Common.Controls.EditorLoader;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using LIS.Filter;
using HIS.Desktop.Plugins.ConnectionTest.ADO;
using LIS.SDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.ConnectionTest.Config;
using HIS.Desktop.Utilities.Extentions;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using System.Globalization;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraBars;
using DevExpress.Utils.Menu;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraGrid;
using HIS.Desktop.Plugins.ConnectionTest.Sda.SdaEventLogCreate;
using HIS.Desktop.EventLog;
using System.Drawing.Text;
using HIS.Desktop.Plugins.ConnectionTest.ConnectionTest;
using AutoMapper;
using HIS.Desktop.Utility;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.ViewInfo;
using EMR.EFMODEL.DataModels;
using EMR.TDO;
using System.IO;
using Inventec.Common.SignLibrary;
using ACS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.ConnectionTest.Validation;
using MPS.ProcessorBase.Core;
using HIS.Desktop.Common;
using System.Diagnostics;
using EMR.Filter;
using MOS.SDO;
using HIS.Desktop.Plugins.ConnectionTest.Resources;
using ACS.SDO;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraPrinting;
using MOS.TDO;
using System.Threading;
using EMR.SDO;
using Inventec.Desktop.CustomControl.CustomGrid;
using System.Timers;

namespace HIS.Desktop.Plugins.ConnectionTest
{
    public partial class UC_ConnectionTest : UserControlBase
    {
        #region Declare
        List<LisSampleADO> lstSampleAll { get; set; }
        private bool IsCallApi = false;
        private string ConfigMachine__Warning = "1";
        private string ConfigMachine__Block = "2";
        private long Id_All = 9999999999;

        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentBySessionControlStateRDO;

        V_HIS_ROOM room = null;
        Inventec.Desktop.Common.Modules.Module currentModule;
        internal List<LisSampleADO> currentSample { get; set; }
        internal LisSampleADO rowSample { get; set; }
        int lastRowHandle = -1;
        internal V_LIS_RESULT currentLisResult { get; set; }
        GridColumn lastColumn = null;
        ToolTipControlInfo lastInfo = null;

        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int limit = 0;
        bool IsLoginameAddmin;
        NumberStyles style;
        internal List<V_LIS_RESULT> _LisResults = new List<V_LIS_RESULT>();
        internal HIS_SERVICE_REQ currentServiceReq = new HIS_SERVICE_REQ();
        internal List<TestLisResultADO> lstCheckPrint = new List<TestLisResultADO>();
        internal List<TestLisResultADO> listCheckPrint = new List<TestLisResultADO>();
        internal List<LIS_MACHINE> _Machines { get; set; }
        Inventec.Desktop.CustomControl.CustomGrid.HideCheckBoxHelper hideCheckBoxHelper__Service;
        BindingList<TestLisResultADO> records;
        public PRINT_OPTION PrintOption { get; set; }
        List<ACS.EFMODEL.DataModels.ACS_CONTROL> controlAcs = new List<ACS.EFMODEL.DataModels.ACS_CONTROL>();
        List<V_HIS_TEST_INDEX> currentTestIndexs = null;
        List<V_HIS_TEST_INDEX_RANGE> testIndexRangeAll;
        //public string ReturnResultWarningTime = "";
        long genderId = 0;
        List<TestLisResultADO> lstLisResultADOs;
        BarManager baManager = null;
        PopupMenuProcessor popupMenuProcessor = null;
        bool isInit = true;
        List<LIS_SAMPLE_TYPE> SampleTypeAllList = null;
        int positionHandle = -1;

        private TreeList treeList;
        private TreeListColumn columnToMerge;

        bool isNotLoadWhileChangeControlStateInFirst;
        string ModuleLinkName = "HIS.Desktop.Plugins.ConnectionTest";

        const string BtnHuyDuyet = "HIS000028";//hủy mẫu

        SignConfigADO SignConfigData;
        string SignApproveList_FileName;//chọn mẫu 1 lần
        List<string> ApproveListError = new List<string>();
        bool HasTemplate;
        HIS_TREATMENT currentTreatment;
        HIS_PATIENT currentPatient;
        HIS_PATIENT_TYPE_ALTER currentPatientTypeAlter;
        long currentPatientGenderId;
        List<V_LIS_RESULT> lstResultPrint = new List<V_LIS_RESULT>();
        List<V_HIS_SERVICE> lstHisService = new List<V_HIS_SERVICE>();

        List<V_LIS_RESULT> lstResultHH = new List<V_LIS_RESULT>();
        List<V_LIS_RESULT> lstResultVS = new List<V_LIS_RESULT>();
        List<V_LIS_RESULT> lstResultMD = new List<V_LIS_RESULT>();
        List<V_LIS_RESULT> lstResultSH = new List<V_LIS_RESULT>();
        List<V_LIS_RESULT> lstResultXNT = new List<V_LIS_RESULT>();
        List<V_LIS_RESULT> lstResultXNGPB = new List<V_LIS_RESULT>();
        List<V_LIS_RESULT> lstResultXNNT = new List<V_LIS_RESULT>();
        Dictionary<long, List<V_LIS_RESULT>> dicServiceTest;
        Dictionary<string, string> dicSignApproveList = new Dictionary<string, string>();

        long simpleTime;
        bool isReturn;
        DateTime currentTimer;
        DateTime currentTimerLM;
        TimerSDO timeSync { get; set; }
        bool IsLoadGridSampleAfterSave;

        List<HIS_DEPARTMENT> lstDepart;
        List<HIS_DEPARTMENT> _StatusSelecteds;
        List<V_HIS_ROOM> _StatusSelectedRooms;
        List<long> lstIDDepart = new List<long>();
        List<long> lstIDRoom = new List<long>();
        #endregion

        #region Contructor
        public UC_ConnectionTest()
        {
            InitializeComponent();
            LisConfigCFG.LoadConfig();
            HisConfigCFG.LoadConfig();
            PacsCFG.LoadConfig();
            LciGroupEmrDocument.Expanded = false;//mặc định lần đâu là ẩn đi
        }

        public UC_ConnectionTest(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                LisConfigCFG.LoadConfig();
                HisConfigCFG.LoadConfig();
                PacsCFG.LoadConfig();
                LciGroupEmrDocument.Expanded = false;//mặc định lần đâu là ẩn đi
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        System.Windows.Forms.Timer timer3 = new System.Windows.Forms.Timer();
        private void UC_ConnectionTest_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetCaptionByLanguageKey();
                if (HisConfigCFG.IsRequiredSampled)
                {
                    chkKhongHienThiChuaLayMau.ForeColor = Color.Gray;
                    chkKhongHienThiChuaLayMau.Checked = true;
                }
                LoadDataToCombo();
                CreateThreadGetService();
                LoadDefaulRoom();
                LoadDataToDepartRoom();
                InitCboStt();
                VisibleColumnSample();
                this.testIndexRangeAll = BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>();
                LoadSampleType();
                ValidateControl();
                RegisterTimer(currentModule.ModuleLink, "timerReloadMachineCounter", timerReloadMachineCounter.Interval, timerReloadMachineCounter_Tick);
                StartTimer(currentModule.ModuleLink, "timerReloadMachineCounter");
                timerReloadMachineCounter.Start();
                timerReloadMachineCounter_Tick();

                LoadUser();
                this.InitControlState();
                GetControlAcs();

                if (cboKskContract.Visible)
                {
                    LoadDataToComboKSK();
                }
                LoadDefaultData();
                this.gridControlSample.ToolTipController = this.toolTipControllerGrid;
                this.treeListSereServTein.ToolTipController = this.toolTipController1;
                GetNewestBarcode();
                FillDataToGridControl();
                LoadCboMachine();
                LoadDataToComboServiceResult();
                LoadCboServiceResult();
                TreeListHeaderMerger(treeListSereServTein, treeListColDescription);
                SetImageToButtonEditSTT();
                IsLoginameAddmin = CheckEmployIsAdmin();
                style = NumberStyles.Any;
                this.isInit = false;
                DateKQ.DateTime = DateTime.Now;

                cboServiceResult.EditValue = Id_All;
                EnabledLableApproveList(false);
                DateKQ.ToolTip = ConvertStringTime(DateKQ);
                DateLM.ToolTip = ConvertStringTime(DateLM);
                GetTimeSystem();
                EnableControlWarning();
                RegisterTimer(currentModule.ModuleLink, "timer1", timer1.Interval, timer1_Tick);
                StartTimer(currentModule.ModuleLink, "timer1");
                RegisterTimer(currentModule.ModuleLink, "timer2", timer2.Interval, timer2_Tick);
                StartTimer(currentModule.ModuleLink, "timer2");
                timer3.Interval = 200; 
                RegisterTimer(currentModule.ModuleLink, "timer3", timer3.Interval, timer3_Tick);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableControlWarning()
        {
            try
            {
                string config = HisConfigs.Get<string>("HIS.Desktop.Plugins.ConnectionTest.IsRequiredMachine");
                if (config == "1")
                {
                    chkCon.Checked = true;
                    chkCon.Enabled = chkWarn.Enabled = false;
                }
                else if (config == "2")
                {
                    chkWarn.Checked = true;
                    chkCon.Enabled = chkWarn.Enabled = false;
                }
                else
                {
                    chkCon.Enabled = chkWarn.Enabled = true;
                    chkCon.Checked = chkWarn.Checked = false;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToDepartRoom()
        {
            try
            {
                InitCheck(cboDepart, SelectionGrid__Status);
                InitCombo(cboDepart, lstDepart, "DEPARTMENT_NAME", "DEPARTMENT_CODE", "Khoa");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember, string title)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;

                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);
                col2.VisibleIndex = 1;
                col2.Width = 220;
                col2.Caption = title;
                cbo.Properties.PopupFormWidth = 250;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    //gridCheckMark.SelectAll(cbo.Properties.DataSource);
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        private void loadDepartment()
        {
            try
            {
                lstDepart = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == 1).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void GetTimeSystem()
        {
            try
            {
                timeSync = new BackendAdapter(new CommonParam()).Get<TimerSDO>(AcsRequestUriStore.ACS_TIMER__SYNC, ApiConsumers.AcsConsumer, 1, new CommonParam());
                currentTimer = currentTimerLM = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(timeSync.LocalTime) ?? DateTime.Now;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void timer1_Tick()
        {
            try
            {
                if (DateKQ.SelectionStart > 0)
                {
                    StopTimer(currentModule.ModuleLink, "timer1");
                }
                else
                {
                    currentTimer = currentTimer.AddSeconds(1);
                    if ((rowSample != null && !rowSample.RESULT_TIME.HasValue) || rowSample == null)
                    {
                        DateKQ.DateTime = currentTimer;
                    }
                }
                DateKQ.ToolTip = ConvertStringTime(DateKQ);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void timer2_Tick()
        {
            try
            {
                if (DateLM.SelectionStart > 0)
                {
                    StopTimer(currentModule.ModuleLink, "timer2");
                }
                else
                {
                    currentTimerLM = currentTimerLM.AddSeconds(1);
                    if ((rowSample != null && !rowSample.SAMPLE_TIME.HasValue) || rowSample == null)
                    {
                        DateLM.DateTime = currentTimerLM;
                    }
                }
                DateLM.ToolTip = ConvertStringTime(DateLM);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void timer3_Tick()
        {
            try
            {
                StopTimer(currentModule.ModuleLink, "timer3");
                txtTreatmentCode.Focus();
                txtTreatmentCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        string ConvertStringTime(DateEdit dte)
        {
            string rs = null;
            try
            {
                if (dte.EditValue != null && dte.DateTime != DateTime.MinValue)
                {
                    var timeNum = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dte.DateTime).ToString();
                    rs = timeNum.Substring(6, 2) + "/" + timeNum.Substring(4, 2) + "/" + timeNum.Substring(0, 4) + " " + timeNum.Substring(8, 2) + ":" + timeNum.Substring(10, 2) + ":" + timeNum.Substring(12, 2);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return rs;
        }

        private void CreateThreadGetService()
        {
            try
            {
                System.Threading.Thread thread = new System.Threading.Thread(ThreadGetService);
                System.Threading.Thread loadDepart = new System.Threading.Thread(loadDepartment);

                thread.Start();
                loadDepart.Start();

                loadDepart.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ThreadGetService()
        {
            try
            {
                lstHisService = BackendDataWorker.Get<V_HIS_SERVICE>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitControlState()
        {
            try
            {

                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();

                this.currentBySessionControlStateRDO = controlStateWorker.GetDataBySession(ControlStateConstant.MODULE_LINK);
                if (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0)
                {

                    foreach (var item_ in this.currentBySessionControlStateRDO)
                    {
                        if (item_.KEY == cboUserKQ.Name)
                        {
                            if (!String.IsNullOrEmpty(item_.VALUE))
                            {
                                cboUserKQ.EditValue = item_.VALUE;
                            }
                        }
                    }
                }
                else
                {
                    cboUserKQ.EditValue = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                    txtUserKQ.Text = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                }



                this.currentControlStateRDO = controlStateWorker.GetData(ControlStateConstant.MODULE_LINK);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstant.CHECK_PRINT_NOW)
                        {
                            checkPrintNow.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstant.CHECK_RETURN_RESULT)
                        {
                            chkReturnResult.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstant.CHECK_SIGN)
                        {
                            chkSign.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == "chkSignProcess")
                        {
                            chkSignProcess.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstant.CHECK_ORDER_BY_BARCODE)
                        {
                            chkOrderByBarcode.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstant.CHECK_ORDER_BY_EMERGENCY_PRIORITY)
                        {
                            chkOrderByEmergencyPriority.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstant.CHECK_SHOW_SAMPLE_GROUP)
                        {
                            chkShowSampleGroup.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstant.COMBO_CONFIG_MACHINE)
                        {
                            if (item.VALUE == this.ConfigMachine__Warning)
                            {
                                chkWarn.Checked = true;
                            }
                            else if (item.VALUE == this.ConfigMachine__Block)
                            {
                                chkCon.Checked = true;
                            }
                        }
                        else if (item.KEY == LciGroupEmrDocument.Name)
                        {
                            LciGroupEmrDocument.Expanded = item.VALUE == "1";
                        }
                        else if (item.KEY == chkSignApproveList.Name)
                        {
                            chkSignApproveList.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == btnApproveListResult.Name)
                        {
                            SignConfigData = !String.IsNullOrWhiteSpace(item.VALUE) ? Newtonsoft.Json.JsonConvert.DeserializeObject<SignConfigADO>(item.VALUE) : null;
                        }
                        else if (item.KEY == ControlStateConstant.CHECK_PRINT_PREVIEW)
                        {
                            chkPrintPreview.Checked = item.VALUE == "1";
                        }




                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadSampleType()
        {
            try
            {
                LIS.Filter.LisSampleTypeFilter filter = new LisSampleTypeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.LIS_RS.COMMON.IS_ACTIVE__TRUE;
                this.SampleTypeAllList = new BackendAdapter(new CommonParam()).Get<List<LIS_SAMPLE_TYPE>>("api/LisSampleType/Get", ApiConsumers.LisConsumer, filter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateControl()
        {
            try
            {
                //ValidationMaxLength(txtNote, 500); 
                ValidationDateKQ();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidationDateKQ()
        {
            try
            {
                Validation.ValidateTimeResult rule = new Validation.ValidateTimeResult();
                rule.dteKq = DateKQ;
                rule.dteLm = DateLM;
                this.dxValidationProviderEditorInfo.SetValidationRule(DateKQ, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void ValidationSampleTime(long intructionTime)
        {
            try
            {
                Validation.ValidateSampleTime rule = new Validation.ValidateSampleTime();
                rule.dtTime = DateLM;
                rule.intructionTime = intructionTime;
                this.dxValidationProviderEditorInfo.SetValidationRule(DateLM, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidationMaxLength(BaseControl memoEdit, int? maxLength)
        {
            try
            {
                Validation.ValidateMaxLength maxLengthValid = new Validation.ValidateMaxLength();
                maxLengthValid.memoEdit = memoEdit;
                maxLengthValid.maxLength = maxLength;
                maxLengthValid.ErrorText = "Dữ liệu vượt quá độ dài cho phép " + maxLength;
                maxLengthValid.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProviderEditorInfo.SetValidationRule(memoEdit, maxLengthValid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void VisibleColumnSample()
        {
            try
            {
                if (LisConfigCFG.SHOW_BUTTON_APPROVE == "1")
                {
                    gc_ApproveResult.Visible = true;
                    gc_ApproveSample.Visible = true;
                    gc_Reject.Visible = true;
                }
                else
                {
                    gc_ApproveResult.Visible = LisConfigCFG.MUST_APPROVE_RESULT == "1";
                    gc_ApproveSample.Visible = false;
                    gc_Reject.Visible = false;
                }

                if (LisConfigCFG.MUST_TICK_RUN_AGAIN == "1")
                {
                    treeListColumn_ReRun.Visible = true;
                }
                else
                {
                    treeListColumn_ReRun.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        bool CheckEmployIsAdmin()
        {
            bool result = false;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisEmployeeFilter hisEmployeeFilter = new HisEmployeeFilter();
                hisEmployeeFilter.LOGINNAME__EXACT = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var employee = new BackendAdapter(param).Get<List<HIS_EMPLOYEE>>("api/HisEmployee/Get", ApiConsumer.ApiConsumers.MosConsumer, hisEmployeeFilter, param).FirstOrDefault();
                if (employee != null && employee.IS_ADMIN == 1)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        void SetImageToButtonEditSTT()
        {
            try
            {
                ButtonEdit_ChuaCoKQSTT.Buttons[0].Image = imageList1.Images[0];
                ButtonEdit_DaCoKQSTT.Buttons[0].Image = imageList1.Images[1];
                ButtonEdit_DaTraKQSTT.Buttons[0].Image = imageList1.Images[2];
                ButtonEdit_DangChayXN.Buttons[0].Image = imageList1.Images[3];
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Gridview Sample
        private void toolTipControllerGrid_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlSample)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlSample.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;
                            string text = "";
                            var isEmergency = ((LisSampleADO)view.GetRow(lastRowHandle)).IS_EMERGENCY == 1;
                            var isPriority = ((LisSampleADO)view.GetRow(lastRowHandle)).PRIORITY == 1;
                            if (isEmergency)
                            {
                                text = "Bệnh nhân cấp cứu";
                            }
                            else if (isPriority)
                            {
                                text = "Bệnh nhân ưu tiên";
                            }
                            else if (info.Column.FieldName == "STATUS")
                            {
                                var busyCount = ((LisSampleADO)view.GetRow(lastRowHandle)).SAMPLE_STT_ID;
                                if (busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM)
                                {
                                    text = "Chưa lấy mẫu";
                                }
                                else if (busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM)
                                {
                                    text = "Đã lấy mẫu";
                                }
                                else if (busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ)
                                {
                                    text = "Có kết quả";
                                }
                                else if (busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                                {
                                    text = "Đã trả kết quả";
                                }
                                else if (busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                                {
                                    text = "Từ chối mẫu";
                                }
                                else if (busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN)
                                {
                                    text = "Chấp nhận mẫu";
                                }
                                else if (busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ)
                                {
                                    text = "Duyệt kết quả";
                                }
                            }

                            if (info.Column.FieldName == "PRINT_COUNT_IC")
                            {
                                var printCount = ((LisSampleADO)view.GetRow(lastRowHandle)).PRINT_COUNT;
                                if (printCount > 0)
                                {
                                    text = string.Format("Đã in {0} lần", printCount);
                                }
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

        private void gridViewSample_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "KET_QUA" || e.Column.FieldName == "DUYET" || e.Column.FieldName == "UPDATE_BARCODE_TIME" || e.Column.FieldName == "NUM_ORDER" || e.Column.FieldName == "REJECT" || e.Column.FieldName == "APPROVE_SAMPLE" || e.Column.FieldName == "APPROVE_RESULT" || e.Column.FieldName == "PRINT_BARCODE")
                {
                    ClickColumnItem();
                    return;
                }
                RowClick();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void RowClick()
        {

            try
            {
                WaitingManager.Show();
                rowSample = null;
                rowSample = (LisSampleADO)gridViewSample.GetFocusedRow();

                LoadLisResult(rowSample);
                LoadDataToGridTestResult2();
                SetDataToCommon(rowSample);
                DataLoadUser(rowSample);
                //chỉ load khi mở vùng xem văn bản
                if (LciGroupEmrDocument.Expanded)
                {
                    ViewSample_Click(rowSample);
                }

                if (rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                {
                    btnSave.Enabled = false;
                }
                else
                {
                    btnSave.Enabled = true;
                }
                positionHandle = -1;
                dxValidationProviderEditorInfo.RemoveControlError(DateKQ);
                dxValidationProviderEditorInfo.RemoveControlError(DateLM);
                ValidationSampleTime(rowSample.INTRUCTION_TIME ?? 0);
                CheckConfigSignWarningOption(rowSample);

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void ClickColumnItem()
        {
            try
            {
                rowSample = null;
                rowSample = (LisSampleADO)gridViewSample.GetFocusedRow();
                var columnFocus = gridViewSample.FocusedColumn;
                var sampleStt = rowSample.SAMPLE_STT_ID;
                if (columnFocus.FieldName == "KET_QUA")
                {
                    if (LisConfigCFG.MUST_APPROVE_RESULT == "1")
                    {
                        if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ)// if (sampleStt == 1 || sampleStt == 2)
                        {
                            TraKetQuaE_Click(null, null);
                        }
                        else if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                        {
                            if (((controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnHuyTraKQ) != null) || IsLoginameAddmin))
                            {
                                HuyTraKQE_Click(null, null);
                            }
                        }
                    }
                    else
                    {
                        if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM
                           || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ
                            || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ
                            || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN)// if (sampleStt == 1 || sampleStt == 2)
                        {
                            TraKetQuaE_Click(null, null);
                        }
                        else if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ
                            && ((controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnHuyTraKQ) != null) || IsLoginameAddmin))
                        {
                            HuyTraKQE_Click(null, null);
                        }
                    }
                }
                else if (columnFocus.FieldName == "DUYET")
                {
                    bool alowUnsample = controlAcs.Exists(o => o.CONTROL_CODE == BtnHuyDuyet);
                    if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM
                        || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                    {
                        DuyetE_Click(null, null);
                    }
                    else if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM
                        || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN)
                    {
                        if (alowUnsample)
                            HuyDuyetE_Click(null, null);
                    }
                    else if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                    {
                    }
                    else if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ)
                    {
                        if (LisConfigCFG.MUST_APPROVE_RESULT == "1" || !alowUnsample)
                        {
                        }
                        else
                        {
                            HuyDuyetE_Click(null, null);
                        }
                    }
                    else
                    {
                        if (alowUnsample)
                        {
                            HuyDuyetE_Click(null, null);
                        }
                    }
                }
                else if (columnFocus.FieldName == "UPDATE_BARCODE_TIME")
                {
                    if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM
                            || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM
                            || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN
                            || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                    {
                        repositoryItemBtnUpdateBarcodeTime_Enable_ButtonClick(null, null);
                    }
                }
                else if (columnFocus.FieldName == "NUM_ORDER")
                {
                    if (rowSample != null && rowSample.NUM_ORDER.HasValue)
                    {

                    }
                    else if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM)
                    {
                        repositoryItemBtnUpdateNumOrder_ButtonClick(null, null);
                    }
                }
                else if (columnFocus.FieldName == "REJECT")
                {
                    if ((rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM
                           || rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN))
                    {
                        repositoryTuChoiMauE_ButtonClick(null, null);
                    }
                }
                else if (columnFocus.FieldName == "APPROVE_SAMPLE")
                {
                    if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM)
                    {
                        repositoryChapNhanMauE_ButtonClick(null, null);
                    }
                    else if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN)
                    {
                        repositoryHuyChapNhan_ButtonClick(null, null);
                    }
                }
                else if (columnFocus.FieldName == "APPROVE_RESULT")
                {
                    if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ
                            || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN)
                    {
                        repositoryDuyetKetQuaE_ButtonClick(null, null);
                    }
                    else if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ)
                    {
                        repositoryHuyDuyetKetQuaE_ButtonClick(null, null);
                    }
                }
                else if (columnFocus.FieldName == "PRINT_BARCODE")
                {
                    InBarcode_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void CheckConfigSignWarningOption(LisSampleADO row)
        {
            try
            {
                WaitingManager.Hide();
                if ((row.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM || row.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM) && (HisConfigCFG.SignWarningOption == "1" || HisConfigCFG.SignWarningOption == "2"))
                {
                    EmrDocumentCountByHisCodeFilter filter = new EmrDocumentCountByHisCodeFilter();
                    filter.HisCode = string.Format("SERVICE_REQ_CODE:{0}", row.SERVICE_REQ_CODE);
                    filter.TreatmentCodeExact = row.TREATMENT_CODE;
                    filter.HasSigner = true;
                    filter.NotReject = true;
                    var data = new Inventec.Common.Adapter.BackendAdapter(param).Get<long?>("api/EmrDocument/CountByHisCode", ApiConsumer.ApiConsumers.EmrConsumer, filter, param);
                    if (data == null || data <= 0)
                    {
                        if (HisConfigCFG.SignWarningOption == "1")
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.YLenhChuaCoVanBan, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                            btnSave.Enabled = false;
                        }
                        else
                        {
                            if (XtraMessageBox.Show(Resources.ResourceMessage.YLenhChuaCoVanBanQues, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                btnSave.Enabled = true;
                            }
                            else
                            {
                                btnSave.Enabled = false;
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

        private void LoadLisResult(LisSampleADO data)
        {
            try
            {
                CommonParam param = new CommonParam();
                _LisResults = new List<V_LIS_RESULT>();
                if (data != null)
                {
                    LIS.Filter.LisResultViewFilter resultFilter = new LisResultViewFilter();
                    resultFilter.SAMPLE_ID = data.ID;
                    _LisResults = new BackendAdapter(param).Get<List<V_LIS_RESULT>>("api/LisResult/GetView", ApiConsumer.ApiConsumers.LisConsumer, resultFilter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataToCommon(LisSampleADO data)
        {
            try
            {
                if (data != null)
                {

                    lblPatientName.Text = data.LAST_NAME + " " + data.FIRST_NAME;
                    if (data.IS_HAS_NOT_DAY_DOB == 1)
                    {
                        lblDob.Text = string.Format("{0} ({1})", data.DOB.ToString().Substring(0, 4), MPS.AgeUtil.CalculateFullAge(data.DOB ?? 0));
                    }
                    else
                        lblDob.Text = string.Format("{0} ({1})", Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DOB ?? 0), MPS.AgeUtil.CalculateFullAge(data.DOB ?? 0));

                    lblGenderName.Text = (data.GENDER_CODE == "01" ? "Nữ" : "Nam");
                    lblPatientCode.Text = data.PATIENT_CODE;
                    txtNote.Text = data.NOTE;
                    txtDescription.Text = data.DESCRIPTION;
                    txtConclude.Text = data.CONCLUDE;
                    txtAdd.Text = data.ADDRESS;
                    lblCancelReason.Text = data.CANCEL_REASON;
                    lblRejectReason.Text = data.REJECT_REASON;
                    if (!String.IsNullOrEmpty(data.SERVICE_REQ_CODE))
                    {
                        CommonParam param = new CommonParam();
                        HisServiceReqFilter filter = new HisServiceReqFilter();
                        filter.SERVICE_REQ_CODE__EXACT = data.SERVICE_REQ_CODE;
                        var GetServiceReq = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, param);
                        if (GetServiceReq != null && GetServiceReq.Count > 0)
                        {
                            lblICDName.Text = GetServiceReq.FirstOrDefault().ICD_NAME;
                            lblICDText.Text = GetServiceReq.FirstOrDefault().ICD_TEXT;
                        }
                        else
                        {
                            lblICDName.Text = "";
                            lblICDText.Text = "";
                        }
                    }
                    else
                    {
                        lblICDName.Text = "";
                        lblICDText.Text = "";
                    }
                }
                else
                {
                    lblPatientName.Text = "";
                    lblDob.Text = "";
                    lblGenderName.Text = "";
                    lblPatientCode.Text = "";
                    txtNote.Text = "";
                    txtDescription.Text = "";
                    txtConclude.Text = "";
                    lblICDName.Text = "";
                    lblICDText.Text = "";
                    lblCancelReason.Text = "";
                    lblRejectReason.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSample_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var sampleStt = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewSample.GetRowCellValue(e.RowHandle, "SAMPLE_STT_ID")).ToString());
                    var data = (LisSampleADO)gridViewSample.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "KET_QUA")
                    {
                        if (LisConfigCFG.MUST_APPROVE_RESULT == "1")
                        {
                            if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ)// if (sampleStt == 1 || sampleStt == 2)
                            {
                                e.RepositoryItem = TraKetQuaE;
                            }
                            else if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                            {
                                if (((controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnHuyTraKQ) != null) || IsLoginameAddmin))
                                {
                                    e.RepositoryItem = HuyTraKQE;
                                }
                                else
                                {
                                    e.RepositoryItem = HuyTraKQD;
                                }
                            }
                            else
                            {
                                e.RepositoryItem = TraKetQuaD;
                            }
                        }
                        else
                        {
                            if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM
                               || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ
                                || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ
                                || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN)// if (sampleStt == 1 || sampleStt == 2)
                            {
                                e.RepositoryItem = TraKetQuaE;
                            }
                            else if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ
                                && ((controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnHuyTraKQ) != null) || IsLoginameAddmin))
                            {
                                e.RepositoryItem = HuyTraKQE;
                            }
                            else
                            {
                                e.RepositoryItem = TraKetQuaD;
                            }
                        }
                    }
                    else if (e.Column.FieldName == "DUYET")
                    {
                        bool alowUnsample = controlAcs.Exists(o => o.CONTROL_CODE == BtnHuyDuyet);
                        if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM
                            || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                        {
                            e.RepositoryItem = DuyetE;
                        }
                        else if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM
                            || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN)
                        {
                            if (alowUnsample)
                                e.RepositoryItem = HuyDuyetE;
                            else
                                e.RepositoryItem = HuyDuyetD;
                        }
                        else if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                        {
                            e.RepositoryItem = HuyDuyetD;
                        }
                        else if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ)
                        {
                            if (LisConfigCFG.MUST_APPROVE_RESULT == "1" || !alowUnsample)
                            {
                                e.RepositoryItem = HuyDuyetD;
                            }
                            else
                            {
                                e.RepositoryItem = HuyDuyetE;
                            }
                        }
                        else
                        {
                            if (alowUnsample)
                            {
                                e.RepositoryItem = HuyDuyetE;
                            }
                            else
                            {
                                e.RepositoryItem = HuyDuyetD;
                            }
                        }
                    }
                    else if (e.Column.FieldName == "UPDATE_BARCODE_TIME")
                    {
                        if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM
                            || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM
                            || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN
                            || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                        {
                            e.RepositoryItem = repositoryItemBtnUpdateBarcodeTime_Enable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemBtnUpdateBarcodeTime_Disable;
                        }
                    }
                    else if (e.Column.FieldName == "NUM_ORDER")
                    {
                        if (data != null && data.NUM_ORDER.HasValue)
                        {
                            e.RepositoryItem = repositoryItemSpinNumOrder;
                        }
                        else if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM)
                        {
                            e.RepositoryItem = repositoryItemBtnUpdateNumOrder;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemBtnUpdateNumOrderDisable;
                        }
                    }
                    else if (e.Column.FieldName == "BARCODE")
                    {
                        if (LisConfigCFG.IS_AUTO_CREATE_BARCODE != "1" && LisConfigCFG.IsAutoSampleAfterEnterBarcode)
                        {
                            if (sampleStt != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM && sampleStt != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                            {
                                e.RepositoryItem = ItemTextEdit_BarCodeEnable;
                            }
                            else if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                            {
                                e.RepositoryItem = ItemTextEdit_BarCodeEnable;
                            }
                            else
                            {
                                e.RepositoryItem = ItemTextEdit_BarCode_Disable;
                            }
                        }
                        else if (LisConfigCFG.IS_AUTO_CREATE_BARCODE != "1" && sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM)
                        {
                            e.RepositoryItem = ItemTextEdit_BarCodeEnable;
                        }
                        else
                        {
                            e.RepositoryItem = ItemTextEdit_BarCode_Disable;
                        }
                    }
                    else if (e.Column.FieldName == "REJECT")
                    {
                        if ((data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM
                            || data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN))
                        {
                            e.RepositoryItem = repositoryTuChoiMauE;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryTuChoiMauD;
                        }
                    }
                    else if (e.Column.FieldName == "APPROVE_SAMPLE")
                    {
                        if (data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM)
                        {
                            e.RepositoryItem = repositoryChapNhanMauE;
                        }
                        else if (data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN)
                        {
                            e.RepositoryItem = repositoryHuyChapNhan;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryChapNhanMauD;
                        }
                    }
                    else if (e.Column.FieldName == "APPROVE_RESULT")
                    {
                        if (data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ
                            || data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN)
                        {
                            e.RepositoryItem = repositoryDuyetKetQuaE;
                        }
                        else if (data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ)
                        {
                            e.RepositoryItem = repositoryHuyDuyetKetQuaE;
                        }
                        else if (data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                        {
                            e.RepositoryItem = repositoryHuyDuyetKetQuaD;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryDuyetKetQuaD;
                        }
                    }
                    else if (e.Column.FieldName == "PRINT_COUNT_IC")
                    {
                        if ((data.PRINT_COUNT != null && data.PRINT_COUNT > 0))
                        {
                            e.RepositoryItem = repCountPrint;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSample_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    LisSampleADO data = (LisSampleADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "STATUS")
                        {
                            //Chua lấy mẫu: mau trang
                            //Đã lấy mẫu: mau vang
                            //Đã có kết quả: mau cam
                            //Đã trả kết quả: mau do
                            long statusId = data.SAMPLE_STT_ID;
                            if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM)
                            {
                                e.Value = imageListIcon.Images[0];
                            }
                            else if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM)
                            {
                                e.Value = imageListIcon.Images[1];
                            }
                            else if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ)
                            {
                                e.Value = imageListIcon.Images[2];
                            }
                            else if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                            {
                                e.Value = imageListIcon.Images[3];
                            }
                            else if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                            {
                                e.Value = imageListIcon.Images[5];
                            }
                            else if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN)
                            {
                                e.Value = imageListIcon.Images[11];
                            }
                            else if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ)
                            {
                                e.Value = imageListIcon.Images[10];
                            }
                        }
                        else if (e.Column.FieldName == "PATIENT_NAME")
                        {
                            e.Value = data.LAST_NAME + " " + data.FIRST_NAME;
                        }
                        else if (e.Column.FieldName == "SAMPLE_LOGINNAME_STR")
                        {
                            e.Value = data.SAMPLE_USERNAME ?? null;
                        }
                        else if (e.Column.FieldName == "APPROVAL_LOGINNAME_STR")
                        {
                            e.Value = data.APPROVAL_USERNAME ?? null;
                        }
                        else if (e.Column.FieldName == "APPROVAL_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.APPROVAL_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "APPOINTMENT_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.APPOINTMENT_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "DOB_STR")
                        {
                            if (data.DOB.HasValue)
                            {
                                if (data.IS_HAS_NOT_DAY_DOB == 1)
                                {
                                    e.Value = data.DOB.ToString().Substring(0, 4);
                                }
                                else
                                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DOB ?? 0);
                            }
                            else
                                e.Value = "";
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "TDL_GENDER_NAME")
                        {
                            e.Value = (data.GENDER_CODE == "01" ? "Nữ" : "Nam");
                        }
                        else if (e.Column.FieldName == "SAMPLE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.SAMPLE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "RESULT_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.RESULT_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "INTRUCTION_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.INTRUCTION_TIME ?? 0);
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

        #region Method
        internal void FillDataToGridControl()
        {
            try
            {
                txtSearchKey.Focus();
                txtSearchKey.SelectAll();

                InitRestoreLayoutGridViewFromXml(gridViewSample);

                this.treeListSereServTein.DataSource = null;
                int pageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }

                FillDataToGridSample(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridSample, param, pageSize, gridControlSample);

                var dataSource = (List<LisSampleADO>)gridControlSample.DataSource;
                if (dataSource != null && dataSource.Count > 0)
                {
                    if (dataSource.Count == 1)
                    {
                        gridViewSample.FocusedRowHandle = 0;
                        RowClick();
                    }
                    else if (rowSample != null)
                    {
                        var checkSample = dataSource.FirstOrDefault(o => o.ID == rowSample.ID);
                        if (checkSample == null)
                        {
                            SetDataToCommon(null);
                        }
                    }
                }
                else
                {
                    SetDataToCommon(null);
                }

                if (IsSearchByTreatmentCode && lstSampleAll.Exists(o => string.IsNullOrEmpty(o.BARCODE)))
                {
                    FocusCellByRowHandle(gridViewSample, lstSampleAll.IndexOf(lstSampleAll.FirstOrDefault(o => string.IsNullOrEmpty(o.BARCODE))), gridColumnBarCode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void GetNewestBarcode()
        {
            try
            {
                if (dtBarcodeTimefrom.EditValue != null && dtBarcodeTimefrom.DateTime != DateTime.MinValue && dtBarcodeTimeTo.EditValue != null && dtBarcodeTimeTo.DateTime != DateTime.MinValue)
                {
                    CommonParam paramCommon = new CommonParam();
                    List<long> filter = new List<long>();

                    long fromTime = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtBarcodeTimefrom.EditValue).ToString("yyyyMMdd") + "000000");
                    filter.Add(fromTime);

                    long toTime = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtBarcodeTimeTo.EditValue).ToString("yyyyMMdd") + "235959");
                    filter.Add(toTime);
                    var result = new BackendAdapter(paramCommon).Get<LIS_SAMPLE>("api/LisSample/GetByBarcodeLatest", ApiConsumer.ApiConsumers.LisConsumer, filter, paramCommon);
                    if (result != null)
                    {
                        lblNewestBarcode.Text = result.BARCODE;
                    }
                    else
                    {
                        lblNewestBarcode.Text = "00";
                    }
                }
                else
                {
                    lblNewestBarcode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private bool IsSearchByTreatmentCode = false;
        internal void FillDataToGridSample(object param)
        {
            try
            {
                InitRestoreLayoutGridViewFromXml(gridViewSample);
                WaitingManager.Show();
                startPage = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);

                gridControlSample.DataSource = null;
                IsSearchByTreatmentCode = false;
                LisSampleViewFilter lisSampleFilter = new LisSampleViewFilter();
                if (room == null)
                {
                    room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentModule.RoomId);
                }
                if (room != null)
                {
                    lisSampleFilter.EXECUTE_ROOM_CODE__EXACT = room.ROOM_CODE;
                }
                if (!String.IsNullOrEmpty(txtSearchKey.Text))
                {
                    lisSampleFilter.KEY_WORD = txtSearchKey.Text.Trim();
                }

                if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                    }
                    IsSearchByTreatmentCode = true;
                    lisSampleFilter.TREATMENT_CODE__EXACT = code;
                    txtTreatmentCode.Text = code;
                }

                lisSampleFilter.IS_ANTIBIOTIC_RESISTANCE = false;
                if (!String.IsNullOrEmpty(txtSERVICE_REQ_CODE__EXACT.Text))
                {
                    string code = txtSERVICE_REQ_CODE__EXACT.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                    }
                    lisSampleFilter.SERVICE_REQ_CODE__EXACT = code;
                    txtSERVICE_REQ_CODE__EXACT.Text = code;
                }
                else if (!String.IsNullOrWhiteSpace(txtPatientCode.Text))
                {
                    string code = txtPatientCode.Text.Trim();
                    if (code.Length < 10)
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                    }
                    lisSampleFilter.PATIENT_CODE__EXACT = code;
                    txtPatientCode.Text = code;
                }

                if (dtBarcodeTimefrom != null && dtBarcodeTimefrom.DateTime != DateTime.MinValue)
                    lisSampleFilter.BARCODE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtBarcodeTimefrom.EditValue).ToString("yyyyMMddHHmm") + "00");
                if (dtBarcodeTimeTo != null && dtBarcodeTimeTo.DateTime != DateTime.MinValue)
                    lisSampleFilter.BARCODE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtBarcodeTimeTo.EditValue).ToString("yyyyMMddHHmm") + "59");
                if (dtResultTimeFrom != null && dtResultTimeFrom.DateTime != DateTime.MinValue)
                    lisSampleFilter.RESULT_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtResultTimeFrom.EditValue).ToString("yyyyMMddHHmm") + "00");
                if (dtResultTimeTo != null && dtResultTimeTo.DateTime != DateTime.MinValue)
                    lisSampleFilter.RESULT_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtResultTimeTo.EditValue).ToString("yyyyMMddHHmm") + "59");
                if (!chkOrderByEmergencyPriority.Checked)
                {
                    if (chkOrderByBarcode.Checked)
                    {
                        lisSampleFilter.ORDER_FIELD = "VIR_AGG_BARCODE";
                        lisSampleFilter.ORDER_DIRECTION = "ASC";

                        lisSampleFilter.ORDER_FIELD1 = "IS_AGGREGATION";
                        lisSampleFilter.ORDER_DIRECTION1 = "ASC";

                        lisSampleFilter.ORDER_FIELD2 = "BARCODE";
                        lisSampleFilter.ORDER_DIRECTION2 = "ASC";
                    }
                    else
                    {
                        lisSampleFilter.ORDER_FIELD = "BARCODE_TIME";
                        lisSampleFilter.ORDER_DIRECTION = "DESC";
                    }
                }
                if (chkAppointment.Checked)
                {
                    lisSampleFilter.ORDER_FIELD = "APPOINTMENT_TIME";
                    lisSampleFilter.ORDER_DIRECTION = "ASC";
                }
                List<long> lstTestServiceReqSTT = new List<long>();

                if (cboServiceResult.EditValue != null)
                {
                    long id = Convert.ToInt64(cboServiceResult.EditValue);
                    if (id != Id_All)
                        lisSampleFilter.SERVICE_RESULT_ID = id;
                }

                if (!chkShowSampleGroup.Checked)
                {
                    lisSampleFilter.HAS_AGGREGATE = false;
                }

                //Tất cả 0
                //Chưa lấy mẫu 1
                //Đã lấy mẫu 2
                //Đã có kết quả
                //Đã trả kết quả
                //Filter yeu cau chua lấy mẫu
                if (cboFind.EditValue != null)
                {
                    // tat ca
                    if ((long)cboFind.EditValue == 0 && chkKhongHienThiChuaLayMau.Checked)
                    {
                        List<long> sampleSttIds = new List<long>();
                        sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM);
                        sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ);
                        sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ);
                        sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN);
                        sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI);
                        sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ);

                        lisSampleFilter.SAMPLE_STT_IDs = sampleSttIds;
                    }
                    //Chưa lấy mẫu
                    else if ((long)cboFind.EditValue == 1)
                    {
                        lisSampleFilter.SAMPLE_STT_IDs = new List<long>()
                            {
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM,
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI
                            };
                    }
                    //Đã lấy mẫu
                    else if ((long)cboFind.EditValue == 2)
                    {
                        lisSampleFilter.SAMPLE_STT_IDs = new List<long>()
                            {
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM,
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN
                            };
                    }
                    //Có kết quả và đã duyệt
                    else if ((long)cboFind.EditValue == 3)
                    {
                        lisSampleFilter.SAMPLE_STT_IDs = new List<long>()
                            {
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ,
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ
                            };
                    }//đã trả kết quả
                    else if ((long)cboFind.EditValue == 4)
                    {
                        lisSampleFilter.SAMPLE_STT_ID = IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ;
                    }
                    else if ((long)cboFind.EditValue == 999) //Chưa trả kq
                    {
                        List<long> sampleSttIds = new List<long>();
                        sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM);
                        sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ);
                        sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN);
                        sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI);
                        sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ);
                        if (!chkKhongHienThiChuaLayMau.Checked)
                        {
                            sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM);//chưa lay mau
                        }
                        lisSampleFilter.SAMPLE_STT_IDs = sampleSttIds;
                    }//Có kết quả và chưa duyệt
                    else if ((long)cboFind.EditValue == 998)
                    {
                        lisSampleFilter.SAMPLE_STT_ID = IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ;
                    }
                    else if ((long)cboFind.EditValue == 5)
                    {
                        lisSampleFilter.SAMPLE_STT_ID = IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN;
                    }
                    //Tất cả
                    else
                    {
                        lisSampleFilter.SAMPLE_STT_ID = null;
                    }
                }

                if (cboKskContract.Visible && cboKskContract.EditValue != null)
                {
                    lisSampleFilter.KSK_CONTRACT_CODE__EXACT = cboKskContract.EditValue.ToString();
                }

                if (cboSttEmr.SelectedIndex == 1) //Chưa ký
                {
                    lisSampleFilter.EMR_RESULT_DOCUMENT_STT_ID = 1;
                }
                else if (cboSttEmr.SelectedIndex == 2) // Đang ký
                {
                    lisSampleFilter.EMR_RESULT_DOCUMENT_STT_ID = 2;
                }
                else if (cboSttEmr.SelectedIndex == 3) // Hoàn thành
                {
                    lisSampleFilter.EMR_RESULT_DOCUMENT_STT_ID = 3;
                }
                else if (cboSttEmr.SelectedIndex == 4) // Từ chối
                {
                    lisSampleFilter.EMR_RESULT_DOCUMENT_STT_ID = 4;
                }

                if (!string.IsNullOrEmpty(cboDepart.Text))
                {
                    var lstSelectDepartCode = _StatusSelecteds.Select(o => o.DEPARTMENT_CODE).ToList();
                    lisSampleFilter.REQUEST_DEPARTMENT_CODEs = lstSelectDepartCode;
                }

                if (!string.IsNullOrEmpty(cboRoom.Text))
                {
                    var lstSelectRoomCode = _StatusSelectedRooms.Select(o => o.ROOM_CODE).ToList();
                    lisSampleFilter.REQUEST_ROOM_CODEs = lstSelectRoomCode;
                }


                var apiResult = new ApiResultObject<List<V_LIS_SAMPLE>>();

                apiResult = new BackendAdapter(paramCommon).GetRO<List<V_LIS_SAMPLE>>("api/LisSample/GetView", ApiConsumer.ApiConsumers.LisConsumer, lisSampleFilter, paramCommon);
                gridControlSample.DataSource = null;
                lstSampleAll = new List<LisSampleADO>();
                if (apiResult != null)
                {
                    WaitingManager.Hide();
                    var data = (List<V_LIS_SAMPLE>)apiResult.Data;
                    if (data != null)
                    {
                        lstSampleAll = (from r in data select new LisSampleADO(r)).ToList();
                        if (chkOrderByEmergencyPriority.Checked)
                        {
                            if (chkOrderByBarcode.Checked)
                            {
                                lstSampleAll = lstSampleAll.OrderByDescending(o => o.IS_EMERGENCY ?? 0).ThenByDescending(o => o.PRIORITY ?? 0).ThenBy(o => o.BARCODE ?? "").ThenByDescending(o => o.INTRUCTION_TIME).ToList();
                            }
                            else
                            {

                                lstSampleAll = lstSampleAll.OrderByDescending(o => o.IS_EMERGENCY ?? 0).ThenByDescending(o => o.PRIORITY ?? 0).ThenByDescending(o => o.INTRUCTION_TIME).ThenByDescending(o => o.BARCODE_TIME).ToList();
                            }
                        }
                        gridControlSample.DataSource = lstSampleAll;
                        rowCount = (lstSampleAll == null ? 0 : lstSampleAll.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    #region Process has exception
                    SessionManager.ProcessTokenLost((CommonParam)param);
                    #endregion
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadDataToComboKSK()
        {
            try
            {
                List<KskContract> contractADOList = new List<KskContract>();
                var kskContractList = BackendDataWorker.Get<HIS_KSK_CONTRACT>();
                foreach (var item in kskContractList)
                {
                    KskContract ado = new KskContract();
                    Inventec.Common.Mapper.DataObjectMapper.Map<KskContract>(ado, item);
                    if (item.CONTRACT_DATE.HasValue)
                    {
                        ado.KskContractDate = Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.CONTRACT_DATE.Value);
                    }
                    contractADOList.Add(ado);
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("KSK_CONTRACT_CODE", "Mã", 50, 1));
                columnInfos.Add(new ColumnInfo("KskContractDate", "Ngày", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("KSK_CONTRACT_CODE", "KSK_CONTRACT_CODE", columnInfos, true, 50);
                ControlEditorLoader.Load(cboKskContract, contractADOList, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToCombo()
        {
            try
            {
                List<HIS.Desktop.Plugins.ConnectionTest.ComboADO> status = new List<HIS.Desktop.Plugins.ConnectionTest.ComboADO>();
                status.Add(new HIS.Desktop.Plugins.ConnectionTest.ComboADO(999, "Chưa trả kết quả"));
                status.Add(new HIS.Desktop.Plugins.ConnectionTest.ComboADO(0, "Tất cả"));
                if (!chkKhongHienThiChuaLayMau.Checked)
                {
                    status.Add(new HIS.Desktop.Plugins.ConnectionTest.ComboADO(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM, "Chưa lấy mẫu"));
                }
                status.Add(new HIS.Desktop.Plugins.ConnectionTest.ComboADO(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM, "Đã lấy mẫu"));
                status.Add(new HIS.Desktop.Plugins.ConnectionTest.ComboADO(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ, "Có kết quả và đã duyệt"));
                status.Add(new HIS.Desktop.Plugins.ConnectionTest.ComboADO(998, "Có kết quả và chưa duyệt"));
                status.Add(new HIS.Desktop.Plugins.ConnectionTest.ComboADO(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ, "Trả kết quả"));
                status.Add(new HIS.Desktop.Plugins.ConnectionTest.ComboADO(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN, "Chấp nhận mẫu"));
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "Trạng thái", 50, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, true, 50);
                ControlEditorLoader.Load(cboFind, status, controlEditorADO);

                cboFind.EditValue = status[0].id;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToComboServiceResult()
        {
            try
            {
                List<LIS_SERVICE_RESULT> lstServiceResult = BackendDataWorker.Get<LIS_SERVICE_RESULT>();
                if (lstServiceResult == null) lstServiceResult = new List<LIS_SERVICE_RESULT>();
                lstServiceResult = lstServiceResult.Where(o => o.IS_ACTIVE == (short)1).ToList();
                LIS_SERVICE_RESULT ss = new LIS_SERVICE_RESULT();
                ss.ID = Id_All;
                ss.SERVICE_RESULT_CODE = "00";
                ss.SERVICE_RESULT_NAME = "Tất cả";
                lstServiceResult.Insert(0, ss);
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_RESULT_NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_RESULT_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(cboServiceResult, lstServiceResult, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCboServiceResult()
        {
            try
            {
                List<LIS_SERVICE_RESULT> lstServiceResult = BackendDataWorker.Get<LIS_SERVICE_RESULT>();
                lstServiceResult = lstServiceResult != null ? lstServiceResult.Where(o => o.IS_ACTIVE == (short)1).ToList() : null;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_RESULT_NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_RESULT_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(this.repositoryItemGridLookUp_ServiceResult, lstServiceResult, controlEditorADO);
                ControlEditorLoader.Load(this.repositoryItemGridLookUp_ServiceResult_Disable, lstServiceResult, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void LoadDefaultData()
        {
            try
            {
                txtSearchKey.Focus();
                txtSearchKey.SelectAll();
                dtBarcodeTimefrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0);
                dtBarcodeTimeTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0);
                gridControlSample.DataSource = null;
                treeListSereServTein.DataSource = null;
                cboRoom.Properties.Buttons[1].Visible = false;
                cboDepart.Properties.Buttons[1].Visible = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region UpdateStt
        private void UpdateStt(long sampleSTT)
        {
            try
            {
                WaitingManager.Show();
                bool result = false;
                CommonParam param = new CommonParam();
                LIS.SDO.UpdateSampleSttSDO updateStt = new LIS.SDO.UpdateSampleSttSDO();
                var row = (LisSampleADO)gridViewSample.GetFocusedRow();
                if (row != null)
                {
                    updateStt.Id = row.ID;
                    updateStt.SampleSttId = sampleSTT;

                    var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("/api/LisSample/UpdateStt", ApiConsumer.ApiConsumers.LisConsumer, updateStt, param);
                    if (curentSTT != null)
                    {
                        result = true;
                        FillDataToGridControl();
                        WaitingManager.Hide();
                    }
                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, result);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
        #endregion

        #region Event Button Sample
        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                GetNewestBarcode();
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InBarcode_Click(object sender, EventArgs e)
        {
            try
            {
                onClickBtnPrintBarCode();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LayMauBenhPham(LisSampleADO sample)
        {
            try
            {
                CommonParam param = new CommonParam();
                if (sample != null && (sample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM
                    || sample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI))
                {
                    MOS.Filter.HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                    treatmentFilter.TREATMENT_CODE__EXACT = sample.TREATMENT_CODE;
                    var treatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();
                    if (treatment != null && treatment.IS_PAUSE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        XtraMessageBox.Show("Hồ sơ đã kết thúc điều trị. Không cho phép thực hiện lấy mẫu.", "Thông báo");
                        return;
                    }
                    if (LisConfigCFG.SHOW_FORM_SAMPLE_INFO == "1")
                    {
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "LIS.Desktop.Plugins.SampleInfo").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'LIS.Desktop.Plugins.SampleInfo'");
                        if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'LIS.Desktop.Plugins.SampleInfo' is not plugins");
                        List<object> listArgs = new List<object>();
                        listArgs.Add(sample);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();

                        rowSample = null;
                        FillDataToGridControl();
                        gridViewSample.RefreshData();
                    }
                    else
                    {
                        //CommonParam param = new CommonParam();
                        bool result = false;
                        WaitingManager.Show();
                        LisSampleSampleSDO sdo = new LisSampleSampleSDO();
                        sdo.SampleId = sample.ID;
                        sdo.RequestRoomCode = room.ROOM_CODE;
                        sdo.Barcode = sample.BARCODE;
                        var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/Sample", ApiConsumer.ApiConsumers.LisConsumer, sdo, param);
                        if (curentSTT != null)
                        {
                            rowSample = sample;
                            rowSample.SAMPLE_STT_ID = curentSTT.SAMPLE_STT_ID;
                            rowSample.SAMPLE_TYPE_ID = curentSTT.SAMPLE_TYPE_ID;
                            rowSample.SAMPLE_TIME = curentSTT.SAMPLE_TIME;
                            rowSample.SAMPLE_LOGINNAME = curentSTT.SAMPLE_LOGINNAME;
                            rowSample.SAMPLE_USERNAME = curentSTT.SAMPLE_USERNAME;
                            FillDataToGridControl();
                            result = true;
                            gridViewSample.RefreshData();
                        }

                        WaitingManager.Hide();
                        #region Show message
                        MessageManager.Show(this.ParentForm, param, result);
                        #endregion

                        #region Process has exception
                        SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DuyetE_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (LisSampleADO)gridViewSample.GetFocusedRow();
                LayMauBenhPham(row);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void HuyDuyetE_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var row = (LisSampleADO)gridViewSample.GetFocusedRow();
                if (row != null && (row.SAMPLE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ))
                {
                    frmUnSample frm = new frmUnSample(row, obj =>
                    {
                        if (obj != null)
                        {
                            var raw = lstSampleAll.FirstOrDefault(o => o.ID == row.ID);
                            raw.SAMPLE_STT_ID = obj.SAMPLE_STT_ID;
                            raw.SAMPLE_TIME = obj.SAMPLE_TIME;
                            raw.SAMPLE_LOGINNAME = obj.SAMPLE_LOGINNAME;
                            raw.SAMPLE_USERNAME = obj.SAMPLE_USERNAME;
                            raw.APPOINTMENT_TIME = obj.APPOINTMENT_TIME;
                            raw.SAMPLE_ORDER = obj.SAMPLE_ORDER;
                            raw.IS_SAMPLE_ORDER_REQUEST = obj.IS_SAMPLE_ORDER_REQUEST;
                            raw.CANCEL_REASON = obj.CANCEL_REASON;
                            gridControlSample.RefreshDataSource();
                            gridViewSample.FocusedRowHandle = gridViewSample.FocusedRowHandle - 1;
                            gridViewSample.FocusedRowHandle = gridViewSample.FocusedRowHandle + 1;
                        }
                    }, room.ROOM_CODE);
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }
        //Load lại Data khi nhấn nút lưu + check trả kết quả sẽ dẫn tới lấy sai chỉ số tương ứng với mẫu.
        private bool IsNotLoadData = false;
        private bool ReturnResult(ref CommonParam param, ref LIS_SAMPLE curentSTT)
        {
            bool result = false;
            try
            {
                long? resultTime = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(DateKQ.EditValue).ToString("yyyyMMddHHmmss"));
                if (rowSample != null && rowSample.RESULT_APPROVAL_TIME != null && rowSample.RESULT_APPROVAL_TIME > resultTime)
                {
                    XtraMessageBox.Show(String.Format("Thời gian duyệt kết quả {0} phải nhỏ hơn thời gian trả kết quả {1}", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rowSample.RESULT_APPROVAL_TIME ?? 0), Inventec.Common.DateTime.Convert.TimeNumberToTimeString(resultTime ?? 0)), "Thông báo");
                    return false;
                }
                var dataChildNullValue = lstLisResultADOs != null && lstLisResultADOs.Count() > 0 ?
                    lstLisResultADOs.Where(o => (o.IS_PARENT != 1 || (o.IS_PARENT == 1 && o.HAS_ONE_CHILD == 1)) && String.IsNullOrWhiteSpace(o.VALUE_RANGE)).ToList()
                    : null;

                if (LisConfigCFG.IS_ALLOW_SAVE_WHEN_NOT_FULL_RESULT == "2" && dataChildNullValue != null && dataChildNullValue.Count() > 0
                     && MessageBox.Show(String.Format("Chỉ số ({0}) chưa có kết quả, bạn có muốn trả kết quả không?", String.Join("; ", dataChildNullValue.Select(o => o.TEST_INDEX_NAME))), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return false;
                }
                else if (LisConfigCFG.IS_ALLOW_SAVE_WHEN_NOT_FULL_RESULT == "0" && dataChildNullValue != null && dataChildNullValue.Count() > 0)
                {
                    MessageBox.Show(String.Format("Chỉ số ({0}) chưa có kết quả", String.Join("; ", dataChildNullValue.Select(o => o.TEST_INDEX_NAME))), "Thông báo");
                    return false;
                }
                else if (LisConfigCFG.IS_ALLOW_SAVE_WHEN_NOT_FULL_RESULT == "3" && !this.CheckOption3(dataChildNullValue))
                {
                    return false;
                }
                result = true;
                LisSampleReturnResultSDO lisSampleReturnResultSDO = new LIS.SDO.LisSampleReturnResultSDO();
                lisSampleReturnResultSDO.SampleId = rowSample.ID;
                lisSampleReturnResultSDO.ResultUsername = cboUserKQ.Text.ToString();
                lisSampleReturnResultSDO.ResultLoginname = cboUserKQ.EditValue.ToString();
                lisSampleReturnResultSDO.ResultTime = resultTime;
                WaitingManager.Show();
                curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("/api/LisSample/ReturnResult", ApiConsumer.ApiConsumers.LisConsumer, lisSampleReturnResultSDO, param);
                if (curentSTT != null)
                {
                    rowSample.SAMPLE_STT_ID = IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ;
                    if (!IsNotLoadData)
                    {
                        FillDataToGridControl();
                        IsLoadGridSampleAfterSave = false;
                        LoadLisResult(rowSample);
                    }
                    string message = string.Format("Trả kết quả toàn phần thành công. SERVICE_REQ_CODE: {0}. BARCODE: {1}", rowSample.SERVICE_REQ_CODE, rowSample.BARCODE);
                    string login = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    SdaEventLogCreate eventlog = new SdaEventLogCreate();
                    eventlog.Create(login, null, true, message);
                }
            }
            catch (Exception ex)
            {
                result = false;
                curentSTT = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void TraKetQuaE_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;
                bool success = false;
                CommonParam param = new CommonParam();
                LisSampleADO row = (LisSampleADO)gridViewSample.GetFocusedRow();
                if (row != rowSample)
                {
                    rowSample = row;

                    LoadLisResult(rowSample);
                    LoadDataToGridTestResult2();
                    SetDataToCommon(rowSample);

                }
                if (rowSample != null)
                {
                    IsNotLoadData = false;
                    LIS_SAMPLE result = null;
                    if (!ReturnResult(ref param, ref result)) return;
                    if (result != null)
                    {
                        success = true;
                    }
                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private static void SaveSuccessLog(string loginName, string action)
        {
            try
            {
                string message = string.Format("{0} SERVICE_REQ_CODE: {1}. BARCODE: {2}. ", action, GlobalVariables.APPLICATION_CODE);
                SdaEventLogCreate eventlog = new SdaEventLogCreate();
                eventlog.Create(loginName, null, true, message);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void HuyTraKQE_Click(object sender, EventArgs e)
        {
            try
            {
                rowSample = (LisSampleADO)gridViewSample.GetFocusedRow();
                if (HisConfigCFG.AutoDeleteEmrDocumentWhenEditReq == "1" && rowSample != null)
                {
                    List<V_EMR_DOCUMENT> listData = new List<V_EMR_DOCUMENT>();
                    string hisCode = "SERVICE_REQ_CODE:" + rowSample.SERVICE_REQ_CODE;
                    CommonParam paramCommon = new CommonParam();
                    var emrFilter = new EMR.Filter.EmrDocumentViewFilter();
                    emrFilter.TREATMENT_CODE__EXACT = rowSample.TREATMENT_CODE;
                    emrFilter.IS_DELETE = false;
                    emrFilter.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_RESULT;
                    var documents = new BackendAdapter(paramCommon).Get<List<V_EMR_DOCUMENT>>("api/EmrDocument/GetView", ApiConsumers.EmrConsumer, emrFilter, paramCommon);
                    if (documents != null && documents.Count > 0)
                    {
                        var serviceDoc = documents.Where(o => !String.IsNullOrWhiteSpace(o.HIS_CODE) && o.HIS_CODE.Contains(hisCode)).ToList();
                        if (serviceDoc != null && serviceDoc.Count > 0)
                        {
                            listData.AddRange(serviceDoc);
                        }
                    }
                    if (listData != null && listData.Count > 0)
                    {
                        if (XtraMessageBox.Show(ResourceMessage.YLenhDaTonTaiVanBanKy, "Thông báo", MessageBoxButtons.YesNo) != DialogResult.Yes)
                            return;

                        CommonParam param = new CommonParam();
                        bool apiResult = new BackendAdapter(param).Post<bool>("api/EmrDocument/DeleteList", ApiConsumers.EmrConsumer, listData.Select(o => o.ID).ToList(), param);
                        if (!apiResult)
                        {
                            MessageManager.Show(this.ParentForm, param, apiResult);
                            return;
                        }

                    }
                }
                UpdateStt(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemBtnUpdateBarcodeTime_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (LisSampleADO)gridViewSample.GetFocusedRow();
                if (data != null)
                {
                    if (data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ
                        || data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ
                        || data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ)
                    {
                        return;
                    }
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    bool success = false;
                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/UpdateBarcodeTime", ApiConsumers.LisConsumer, data.ID, param);
                    if (rs != null)
                    {
                        data.SAMPLE_ORDER = rs.SAMPLE_ORDER;
                        success = true;
                    }
                    WaitingManager.Hide();
                    if (success)
                    {
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                    else
                    {
                        MessageManager.Show(param, success);
                    }

                    SessionManager.ProcessTokenLost(param);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearchKey_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGridControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSERVICE_REQ_CODE__EXACT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGridControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void SetCaptionByLanguageKey()
        {
            try
            {
                if (this.currentModule != null && !String.IsNullOrEmpty(this.currentModule.text))
                {
                    this.Text = this.currentModule.text;
                }
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ConnectionTest.Resources.Lang", typeof(UC_ConnectionTest).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSignProcess.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.chkSignProcess.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSignProcess.ToolTip = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.chkSignProcess.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnOKForResultDescription.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.btnOKForResultDescription.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancelForResultDescription.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.btnCancelForResultDescription.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabDocumentReq.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.xtraTabDocumentReq.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabDocumentResult.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.xtraTabDocumentResult.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnOKForNote.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.btnOKForNote.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancelForNote.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.btnCancelForNote.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListSereServTein.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.treeListSereServTein.OptionsFind.FindNullPrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn6.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.treeListColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemPictureEdit2.NullText = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.repositoryItemPictureEdit2.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIndexCode.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.grdColIndexCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIndexName.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.grdColIndexName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn_ReRun.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.treeListColumn_ReRun.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn_ReRun.ToolTip = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.treeListColumn_ReRun.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.treeListColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.ToolTip = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.treeListColumn3.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdCollDonvitinh.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.grdCollDonvitinh.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdCollDonvitinh.ToolTip = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.grdCollDonvitinh.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColVallue.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.grdColVallue.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.treeListColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColValueNormal.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.grdColValueNormal.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColDescription.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.treeListColDescription.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumnButtonForNote.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.treeListColumnButtonForNote.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListOldValue.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.treeListOldValue.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumnForOldValue.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.treeListColumnForOldValue.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColResultDescription.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.treeListColResultDescription.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumnForButtonResultDesciption.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.treeListColumnForButtonResultDesciption.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListModifier.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.treeListModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GridLookUpEdit__Machine.NullText = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.GridLookUpEdit__Machine.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GridLookUpEdit__Machine_Disable.NullText = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.GridLookUpEdit__Machine_Disable.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemCheckRerun_Enable.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.repositoryItemCheckRerun_Enable.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemCheckRerun_Disable.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.repositoryItemCheckRerun_Disable.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemGridLookUp_ServiceResult.NullText = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.repositoryItemGridLookUp_ServiceResult.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemGridLookUp_ServiceResult_Disable.NullText = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.repositoryItemGridLookUp_ServiceResult_Disable.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancelForValueRange.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.btnCancelForValueRange.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnOKForValueRange.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.btnOKForValueRange.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciGroupEmrDocument.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.LciGroupEmrDocument.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.layoutControlItem21.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem23.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.layoutControlItem23.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem32.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.layoutControlItem32.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPrintPreview.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.chkPrintPreview.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblApproveResultError.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.lblApproveResultError.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblApproveResultSuccess.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.lblApproveResultSuccess.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnApproveListResult.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.btnApproveListResult.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnApproveListResult.ToolTip = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.btnApproveListResult.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSignApproveList.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.chkSignApproveList.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCreateSigner.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.btnCreateSigner.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCreateSigner.ToolTip = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.btnCreateSigner.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkOrderByBarcode.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.chkOrderByBarcode.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkShowSampleGroup.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.chkShowSampleGroup.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboServiceResult.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.cboServiceResult.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnKhongThucHien.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.btnKhongThucHien.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSign.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.chkSign.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnInTachTheoNhom.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.btnInTachTheoNhom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboKskContract.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.cboKskContract.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboKskContract.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.cboKskContract.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkPrintNow.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.checkPrintNow.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkReturnResult.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.chkReturnResult.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtPatientCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.txtPatientCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSERVICE_REQ_CODE__EXACT.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.txtSERVICE_REQ_CODE__EXACT.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientName.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.lciPatientName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDob.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.lciDob.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciGenderName.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.lciGenderName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientCode.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.lciPatientCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboFind.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.cboFind.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchKey.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.txtSearchKey.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_CheckApprovalList.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gc_CheckApprovalList.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_Reject.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gc_Reject.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_Reject.ToolTip = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gc_Reject.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_ApproveSample.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gc_ApproveSample.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_ApproveSample.ToolTip = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gc_ApproveSample.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_ApproveResult.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gc_ApproveResult.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_ApproveResult.ToolTip = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gc_ApproveResult.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn18.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnBarCode.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumnBarCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn20.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn27.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn27.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn24.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn24.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn22.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn22.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn26.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn26.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn25.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn25.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn23.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn23.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnGenderName.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumnGenderName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn21.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn21.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn19.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn19.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboUserKQ.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.cboUserKQ.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNewestBarcode.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.lciNewestBarcode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciResultTimeFrom.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.lciResultTimeFrom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPrintNow.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.lciPrintNow.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciReturnResult.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.lciReturnResult.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSignApproveList.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.lciSignApproveList.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciApproveListSuccess.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.lciApproveListSuccess.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciApproveResultError.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.lciApproveResultError.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPrintPreview.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.lciPrintPreview.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSign.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.lciSign.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem29.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.layoutControlItem29.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem29.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.layoutControlItem29.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem31.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.layoutControlItem31.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem31.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.layoutControlItem31.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem33.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.layoutControlItem33.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void GetControlAcs()
        {
            try
            {
                CommonParam param = new CommonParam();
                ACS.SDO.AcsTokenLoginSDO tokenLoginSDOForAuthorize = new ACS.SDO.AcsTokenLoginSDO();
                tokenLoginSDOForAuthorize.LOGIN_NAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                tokenLoginSDOForAuthorize.APPLICATION_CODE = GlobalVariables.APPLICATION_CODE;
                var acsAuthorize = new BackendAdapter(param).Get<ACS.SDO.AcsAuthorizeSDO>(HIS.Desktop.ApiConsumer.AcsRequestUriStore.ACS_TOKEN__AUTHORIZE, HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, tokenLoginSDOForAuthorize, param);
                if (acsAuthorize != null)
                {
                    controlAcs.AddRange(acsAuthorize.ControlInRoles.ToList());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessMaxMixValue(TestLisResultADO ti, V_HIS_TEST_INDEX_RANGE testIndexRange)
        {
            try
            {
                Decimal minValue = 0, maxValue = 0, value = 0;
                if (ti != null && testIndexRange != null)
                {
                    ti.DESCRIPTION = "";
                    if (!String.IsNullOrWhiteSpace(testIndexRange.MIN_VALUE))
                    {
                        if (Decimal.TryParse((testIndexRange.MIN_VALUE ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out minValue))
                        {
                            ti.MIN_VALUE = minValue;
                        }
                        else
                        {
                            ti.MIN_VALUE = null;
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(testIndexRange.MAX_VALUE))
                    {
                        if (Decimal.TryParse((testIndexRange.MAX_VALUE ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out maxValue))
                        {
                            ti.MAX_VALUE = maxValue;
                        }
                        else
                        {
                            ti.MAX_VALUE = null;
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(ti.VALUE_RANGE))
                    {
                        if (Decimal.TryParse((ti.VALUE_RANGE ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out value))
                        {
                            ti.VALUE = value;
                        }
                        else
                        {
                            ti.VALUE = null;
                        }
                    }

                    ti.IS_NORMAL = null;
                    ti.IS_HIGHER = null;
                    ti.IS_LOWER = null;

                    if (!String.IsNullOrEmpty(testIndexRange.NORMAL_VALUE))
                    {
                        ti.DESCRIPTION = testIndexRange.NORMAL_VALUE;
                        if (!String.IsNullOrWhiteSpace(ti.DESCRIPTION) && !String.IsNullOrWhiteSpace(ti.VALUE_RANGE) && ti.VALUE_RANGE.ToUpper() == ti.DESCRIPTION.ToUpper())
                        {
                            ti.IS_NORMAL = true;
                        }
                        else if (!String.IsNullOrWhiteSpace(ti.DESCRIPTION) && !String.IsNullOrWhiteSpace(ti.VALUE_RANGE) && ti.VALUE_RANGE.ToUpper() != ti.DESCRIPTION.ToUpper())
                        {
                            ti.IS_LOWER = true;
                            ti.IS_HIGHER = true;
                        }
                    }
                    else
                    {
                        if (testIndexRange.IS_ACCEPT_EQUAL_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_MAX == null)
                        {
                            if (testIndexRange.MIN_VALUE != null)
                            {
                                ti.DESCRIPTION += testIndexRange.MIN_VALUE + "<= ";
                            }

                            ti.DESCRIPTION += "X";

                            if (testIndexRange.MAX_VALUE != null)
                            {
                                ti.DESCRIPTION += " < " + testIndexRange.MAX_VALUE;
                            }

                            if (ti.VALUE != null && ti.MIN_VALUE != null && ti.MIN_VALUE <= ti.VALUE && ti.MAX_VALUE != null && ti.VALUE < ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (ti.VALUE != null && ti.MIN_VALUE != null && ti.VALUE < ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (ti.VALUE != null && ti.MAX_VALUE != null && ti.MAX_VALUE <= ti.VALUE)
                            {
                                ti.IS_HIGHER = true;
                            }
                        }
                        else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_MAX == 1)
                        {
                            if (testIndexRange.MIN_VALUE != null)
                            {
                                ti.DESCRIPTION += testIndexRange.MIN_VALUE + "<= ";
                            }

                            ti.DESCRIPTION += "X";

                            if (testIndexRange.MAX_VALUE != null)
                            {
                                ti.DESCRIPTION += " <= " + testIndexRange.MAX_VALUE;
                            }

                            if (ti.VALUE != null && ti.MIN_VALUE != null && ti.MIN_VALUE <= ti.VALUE && ti.MAX_VALUE != null && ti.VALUE <= ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (ti.VALUE != null && ti.MIN_VALUE != null && ti.VALUE < ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (ti.VALUE != null && ti.MAX_VALUE != null && ti.MAX_VALUE < ti.VALUE)
                            {
                                ti.IS_HIGHER = true;
                            }
                        }
                        else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_MAX == 1)
                        {
                            if (testIndexRange.MIN_VALUE != null)
                            {
                                ti.DESCRIPTION += testIndexRange.MIN_VALUE + "< ";
                            }

                            ti.DESCRIPTION += "X";

                            if (testIndexRange.MAX_VALUE != null)
                            {
                                ti.DESCRIPTION += " <= " + testIndexRange.MAX_VALUE;
                            }

                            if (ti.VALUE != null && ti.MIN_VALUE != null && ti.MIN_VALUE < ti.VALUE && ti.MAX_VALUE != null && ti.VALUE <= ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (ti.VALUE != null && ti.MIN_VALUE != null && ti.VALUE < ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (ti.VALUE != null && ti.MAX_VALUE != null && ti.MAX_VALUE < ti.VALUE)
                            {
                                ti.IS_HIGHER = true;
                            }
                        }
                        else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_MAX == null)
                        {
                            if (testIndexRange.MIN_VALUE != null)
                            {
                                ti.DESCRIPTION += testIndexRange.MIN_VALUE + "< ";
                            }

                            ti.DESCRIPTION += "X";

                            if (testIndexRange.MAX_VALUE != null)
                            {
                                ti.DESCRIPTION += " < " + testIndexRange.MAX_VALUE;
                            }
                            if (ti.VALUE != null && ti.MIN_VALUE != null && ti.MIN_VALUE < ti.VALUE && ti.MAX_VALUE != null && ti.VALUE < ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (ti.VALUE != null && ti.MIN_VALUE != null && ti.VALUE <= ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (ti.VALUE != null && ti.MAX_VALUE != null && ti.MAX_VALUE <= ti.VALUE)
                            {
                                ti.IS_HIGHER = true;
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

        private long LoadGenderId()
        {
            long genderId = 0;
            try
            {
                CommonParam param = new CommonParam();
                if (rowSample != null && !String.IsNullOrWhiteSpace(rowSample.GENDER_CODE))
                {
                    genderId = rowSample.GENDER_CODE == "01" ? 1 : 2;
                }
                else if (rowSample != null && !String.IsNullOrWhiteSpace(rowSample.PATIENT_CODE))
                {
                    HisPatientFilter patientFilter = new HisPatientFilter();
                    patientFilter.PATIENT_CODE = rowSample.PATIENT_CODE;
                    var patients = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, param);
                    if (patients != null && patients.Count > 0)
                    {
                        genderId = patients.FirstOrDefault().GENDER_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                genderId = 0;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return genderId;
        }

        private void LoadDataToGridTestResult2()
        {
            try
            {
                CommonParam param = new CommonParam();

                List<V_HIS_TEST_INDEX_RANGE> testIndexRanges = null;
                var testIndex = BackendDataWorker.Get<V_HIS_TEST_INDEX>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                if (_LisResults != null && _LisResults.Count > 0)
                {
                    _LisResults = _LisResults.OrderBy(o => o.SERVICE_NUM_ORDER).ToList();

                    currentTestIndexs = new List<V_HIS_TEST_INDEX>();
                    var serviceCodes = _LisResults.Select(o => o.SERVICE_CODE).Distinct().ToList();
                    currentTestIndexs = testIndex.Where(o => serviceCodes.Contains(o.SERVICE_CODE)).ToList();
                    if (currentTestIndexs != null && currentTestIndexs.Count > 0 && testIndex != null && testIndex.Count > 0)
                    {
                        var testIndexCodes = currentTestIndexs.Select(o => o.TEST_INDEX_CODE).Distinct().ToList();
                        testIndexRanges = BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>().Where(o => testIndexCodes.Contains(o.TEST_INDEX_CODE)).ToList();
                    }
                }

                // lay lis_sample
                this.rowSample = (LisSampleADO)gridViewSample.GetFocusedRow();
                genderId = LoadGenderId();
                lstLisResultADOs = new List<TestLisResultADO>();
                if (currentTestIndexs != null && currentTestIndexs.Count > 0)
                {
                    var groupListResult = _LisResults.GroupBy(o => o.SERVICE_CODE).ToList();

                    List<HIS_SERE_SERV> dataSereServs = new List<HIS_SERE_SERV>();
                    if (this.rowSample != null && !String.IsNullOrEmpty(this.rowSample.SERVICE_REQ_CODE))
                    {
                        HisSereServFilter ssFilter = new HisSereServFilter();
                        // lay sereServ tu y lenh
                        ssFilter.TDL_SERVICE_REQ_CODE_EXACT = this.rowSample.SERVICE_REQ_CODE;
                        dataSereServs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, null);
                    }

                    foreach (var group in groupListResult)
                    {
                        TestLisResultADO hisSereServTeinSDO = new TestLisResultADO();
                        var fistGroup = group.FirstOrDefault();
                        if (fistGroup != null)
                        {
                            hisSereServTeinSDO.IS_PARENT = 1;
                            hisSereServTeinSDO.TEST_INDEX_CODE = fistGroup.SERVICE_CODE;
                            hisSereServTeinSDO.TEST_INDEX_NAME = fistGroup.SERVICE_NAME;
                            hisSereServTeinSDO.SERVICE_CODE = fistGroup.SERVICE_CODE;
                            hisSereServTeinSDO.SERVICE_NAME = fistGroup.SERVICE_NAME;
                            hisSereServTeinSDO.SERVICE_RESULT_ID = fistGroup.SERVICE_RESULT_ID;
                            hisSereServTeinSDO.ID = fistGroup.ID;
                            hisSereServTeinSDO.IS_NO_EXECUTE = fistGroup.IS_NO_EXECUTE;
                            hisSereServTeinSDO.PARENT_ID = ".";
                            hisSereServTeinSDO.MODIFIER = "";
                            hisSereServTeinSDO.CHILD_ID = fistGroup.ID + ".";
                            hisSereServTeinSDO.SERVICE_NUM_ORDER = fistGroup.SERVICE_NUM_ORDER;
                            hisSereServTeinSDO.IS_RUN_AGAIN = fistGroup.IS_RUN_AGAIN;
                            hisSereServTeinSDO.IS_RUNNING = fistGroup.IS_RUNNING;
                            hisSereServTeinSDO.RERUN = fistGroup.IS_RUN_AGAIN == 1;
                            // lay sereServ
                            if (dataSereServs != null && dataSereServs.Count() > 0)
                            {
                                var ss = dataSereServs.FirstOrDefault(o => o.TDL_SERVICE_CODE == fistGroup.SERVICE_CODE);
                                if (ss != null)
                                {
                                    hisSereServTeinSDO.PATIENT_TYPE_ID_BY_SERE_SERV = (long?)ss.PATIENT_TYPE_ID;
                                    hisSereServTeinSDO.IS_CONFIRM_NO_EXCUTE = ss.IS_CONFIRM_NO_EXCUTE;
                                }
                                else
                                {
                                    hisSereServTeinSDO.PATIENT_TYPE_ID_BY_SERE_SERV = null;
                                    hisSereServTeinSDO.IS_CONFIRM_NO_EXCUTE = null;
                                }
                            }
                        }

                        //Lay machine_id
                        var lstResultItem = group.ToList();
                        lstResultItem = lstResultItem.OrderBy(o => o.ID).ThenBy(p => p.SERVICE_NAME).ToList();
                        if (this._LisResults != null
                            && this._LisResults.Count > 0
                            && lstResultItem != null
                            && lstResultItem.Count > 0)
                        {
                            var machineNotNull = this._LisResults.FirstOrDefault(p => p.SERVICE_CODE == hisSereServTeinSDO.SERVICE_CODE && p.MACHINE_ID != null);
                            var machineByLisResult = machineNotNull != null ? machineNotNull : this._LisResults.FirstOrDefault(p => p.SERVICE_CODE == hisSereServTeinSDO.SERVICE_CODE);
                            if (machineByLisResult != null)
                            {
                                hisSereServTeinSDO.MACHINE_ID = machineByLisResult.MACHINE_ID.HasValue ? machineByLisResult.MACHINE_ID : machineByLisResult.SERVICE_MACHINE_ID;
                                hisSereServTeinSDO.MACHINE_ID_OLD = machineByLisResult.MACHINE_ID.HasValue ? machineByLisResult.MACHINE_ID : machineByLisResult.SERVICE_MACHINE_ID; ;
                            }
                        }
                        var testIndFist = currentTestIndexs.FirstOrDefault(o => o.TEST_INDEX_CODE == lstResultItem[0].TEST_INDEX_CODE);
                        //LogSystem.Debug("lstResultItem.Count " + lstResultItem.Count + " ; IS_NOT_SHOW_SERVICE: " + testIndFist.IS_NOT_SHOW_SERVICE + " ; testIndFist: " + LogUtil.TraceData("testIndFist", testIndFist));
                        if (lstResultItem != null
                            && lstResultItem.Count == 1
                            && testIndFist != null && testIndFist.IS_NOT_SHOW_SERVICE == 1)
                        {
                            hisSereServTeinSDO.HAS_ONE_CHILD = 1;
                            hisSereServTeinSDO.CHILD_ID = lstResultItem[0].ID + "." + lstResultItem[0].ID;
                            hisSereServTeinSDO.MODIFIER = lstResultItem[0].MODIFIER;
                            hisSereServTeinSDO.TEST_INDEX_CODE = lstResultItem[0].TEST_INDEX_CODE;
                            hisSereServTeinSDO.TEST_INDEX_NAME = lstResultItem[0].TEST_INDEX_NAME;
                            hisSereServTeinSDO.TEST_INDEX_UNIT_NAME = testIndFist.TEST_INDEX_UNIT_NAME;
                            hisSereServTeinSDO.IS_IMPORTANT = testIndFist.IS_IMPORTANT;
                            hisSereServTeinSDO.SAMPLE_SERVICE_ID = lstResultItem[0].SAMPLE_SERVICE_ID;
                            hisSereServTeinSDO.SAMPLE_SERVICE_STT_ID = lstResultItem[0].SAMPLE_SERVICE_STT_ID;
                            hisSereServTeinSDO.MODIFIER = lstResultItem[0].MODIFIER;
                            hisSereServTeinSDO.VALUE_RANGE = ProcessValue(lstResultItem[0]);
                            hisSereServTeinSDO.LIS_RESULT_ID = lstResultItem[0].ID;
                            hisSereServTeinSDO.ID = lstResultItem[0].ID;
                            hisSereServTeinSDO.SAMPLE_ID = lstResultItem[0].SAMPLE_ID;
                            hisSereServTeinSDO.SAMPLE_SERVICE_ID = lstResultItem[0].SAMPLE_SERVICE_ID;
                            hisSereServTeinSDO.SAMPLE_SERVICE_STT_CODE = lstResultItem[0].SAMPLE_SERVICE_STT_CODE;
                            hisSereServTeinSDO.SAMPLE_SERVICE_STT_NAME = lstResultItem[0].SAMPLE_SERVICE_STT_NAME;
                            hisSereServTeinSDO.MACHINE_ID_OLD = lstResultItem[0].MACHINE_ID.HasValue ? lstResultItem[0].MACHINE_ID : lstResultItem[0].SERVICE_MACHINE_ID;
                            hisSereServTeinSDO.MACHINE_ID = lstResultItem[0].MACHINE_ID.HasValue ? lstResultItem[0].MACHINE_ID : lstResultItem[0].SERVICE_MACHINE_ID;
                            hisSereServTeinSDO.NOTE = lstResultItem[0].DESCRIPTION;
                            hisSereServTeinSDO.SERVICE_NUM_ORDER = lstResultItem[0].SERVICE_NUM_ORDER;
                            hisSereServTeinSDO.OLD_VALUE = lstResultItem[0].OLD_VALUE;
                            hisSereServTeinSDO.SAMPLE_STT_ID = lstResultItem[0].SAMPLE_STT_ID;
                            hisSereServTeinSDO.IS_RUN_AGAIN = lstResultItem[0].IS_RUN_AGAIN;
                            hisSereServTeinSDO.IS_RUNNING = lstResultItem[0].IS_RUNNING;
                            hisSereServTeinSDO.RESULT_DESCRIPTION = lstResultItem[0].RESULT_DESCRIPTION;
                            hisSereServTeinSDO.IS_NOT_SHOW_SERVICE = testIndFist.IS_NOT_SHOW_SERVICE;
                        }
                        lstLisResultADOs.Add(hisSereServTeinSDO);

                        if (lstResultItem != null
                            && (lstResultItem.Count > 1
                            || (lstResultItem.Count == 1
                            && testIndFist != null && testIndFist.IS_NOT_SHOW_SERVICE != 1))
                            )
                        {
                            foreach (var ssTein in lstResultItem)
                            {
                                var testIndChild = currentTestIndexs.FirstOrDefault(o => o.TEST_INDEX_CODE == ssTein.TEST_INDEX_CODE);
                                TestLisResultADO hisSereServTein = new TestLisResultADO();
                                hisSereServTein.HAS_ONE_CHILD = 0;
                                Inventec.Common.Mapper.DataObjectMapper.Map<TestLisResultADO>(hisSereServTein, ssTein);
                                hisSereServTein.IS_PARENT = 0;
                                hisSereServTein.RESULT_DESCRIPTION = ssTein.RESULT_DESCRIPTION;
                                if (testIndChild != null)
                                {
                                    hisSereServTein.IS_IMPORTANT = testIndChild.IS_IMPORTANT;
                                    hisSereServTein.TEST_INDEX_UNIT_NAME = testIndChild.TEST_INDEX_UNIT_NAME;
                                    hisSereServTein.NUM_ORDER = testIndChild.NUM_ORDER;
                                }
                                else
                                {
                                    hisSereServTein.NUM_ORDER = null;
                                }
                                hisSereServTein.CHILD_ID = ssTein.ID + "." + ssTein.ID;
                                hisSereServTein.ID = ssTein.ID;
                                hisSereServTein.PARENT_ID = hisSereServTeinSDO.CHILD_ID;
                                hisSereServTein.TEST_INDEX_CODE = ssTein.TEST_INDEX_CODE;
                                hisSereServTein.TEST_INDEX_NAME = ssTein.TEST_INDEX_NAME;
                                hisSereServTein.MODIFIER = "";
                                hisSereServTein.MODIFIER = ssTein.MODIFIER;
                                hisSereServTein.VALUE_RANGE = ProcessValue(ssTein);
                                hisSereServTein.LIS_RESULT_ID = ssTein.ID;
                                hisSereServTein.MACHINE_ID = ssTein.MACHINE_ID.HasValue ? ssTein.MACHINE_ID : ssTein.SERVICE_MACHINE_ID;
                                hisSereServTein.MACHINE_ID_OLD = ssTein.MACHINE_ID.HasValue ? ssTein.MACHINE_ID : ssTein.SERVICE_MACHINE_ID;
                                hisSereServTein.SAMPLE_ID = ssTein.SAMPLE_ID;
                                hisSereServTein.SAMPLE_SERVICE_ID = ssTein.SAMPLE_SERVICE_ID;
                                hisSereServTein.SAMPLE_SERVICE_STT_CODE = ssTein.SAMPLE_SERVICE_STT_CODE;
                                hisSereServTein.SAMPLE_SERVICE_STT_ID = ssTein.SAMPLE_SERVICE_STT_ID;
                                hisSereServTein.SAMPLE_SERVICE_STT_NAME = ssTein.SAMPLE_SERVICE_STT_NAME;
                                hisSereServTein.SERVICE_NUM_ORDER = ssTein.SERVICE_NUM_ORDER;
                                hisSereServTein.OLD_VALUE = ssTein.OLD_VALUE;
                                hisSereServTein.DESCRIPTION = "";
                                hisSereServTeinSDO.SAMPLE_SERVICE_ID = ssTein.SAMPLE_SERVICE_ID;
                                hisSereServTeinSDO.SAMPLE_SERVICE_STT_ID = ssTein.SAMPLE_SERVICE_STT_ID;
                                hisSereServTeinSDO.SAMPLE_STT_ID = ssTein.SAMPLE_STT_ID;
                                hisSereServTeinSDO.IS_RUN_AGAIN = ssTein.IS_RUN_AGAIN;
                                hisSereServTeinSDO.IS_RUNNING = ssTein.IS_RUNNING;
                                hisSereServTein.NOTE = ssTein.DESCRIPTION;
                                lstLisResultADOs.Add(hisSereServTein);
                            }
                        }
                    }
                }
                // gán test index range
                if (lstLisResultADOs != null && lstLisResultADOs.Count > 0)
                {
                    foreach (var hisSereServTeinSDO in lstLisResultADOs)
                    {
                        V_HIS_TEST_INDEX_RANGE testIndexRange = new V_HIS_TEST_INDEX_RANGE();
                        testIndexRange = GetTestIndexRange(this.rowSample.DOB ?? 0, this.genderId, hisSereServTeinSDO.TEST_INDEX_CODE, ref this.testIndexRangeAll);
                        if (testIndexRange != null)
                        {
                            ProcessMaxMixValue(hisSereServTeinSDO, testIndexRange);
                        }
                    }
                }

                if (lstLisResultADOs != null && lstLisResultADOs.Count() > 0)
                {
                    lstLisResultADOs = lstLisResultADOs.OrderBy(o => o.SERVICE_NUM_ORDER)
                    .ThenByDescending(p => p.NUM_ORDER).ToList();
                }
                this.ProcessAutoSelectMachine(lstLisResultADOs);
                int stt = 1;
                lstLisResultADOs.ForEach(o => o.STT = o.IS_PARENT == 1 ? (int?)stt++ : null);
                // treeList
                records = new BindingList<TestLisResultADO>(lstLisResultADOs);
                this.treeListSereServTein.RefreshDataSource();
                this.treeListSereServTein.DataSource = null;
                this.treeListSereServTein.DataSource = records;
                this.treeListSereServTein.KeyFieldName = "CHILD_ID";
                this.treeListSereServTein.ParentFieldName = "PARENT_ID";
                this.treeListSereServTein.ExpandAll();

                this.treeListSereServTein.CheckAll();
                lstCheckPrint = lstLisResultADOs;

                btnPrint.Enabled = true;
                btnInTachTheoNhom.Enabled = true;
                btnKhongThucHien.Enabled = true;

                this.hideCheckBoxHelper__Service = new Inventec.Desktop.CustomControl.CustomGrid.HideCheckBoxHelper(this.treeListSereServTein);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string ProcessValue(V_LIS_RESULT data)
        {
            string result = "";
            try
            {
                if (data != null)
                {
                    if (!string.IsNullOrEmpty(data.VALUE))
                    {
                        result = data.VALUE;
                    }
                    else
                    {
                        var testIndex = BackendDataWorker.Get<V_HIS_TEST_INDEX>().FirstOrDefault(s => s.TEST_INDEX_CODE == data.TEST_INDEX_CODE && s.DEFAULT_VALUE != null);
                        if (testIndex != null)
                        {
                            result = testIndex.DEFAULT_VALUE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        List<List<TestLisResultADO>> SplitList(List<TestLisResultADO> me, int size = 44)
        {
            var list = new List<List<TestLisResultADO>>();
            try
            {
                for (int i = 0; i < me.Count; i += size)
                    list.Add(me.GetRange(i, Math.Min(size, me.Count - i)));
            }
            catch (Exception ex)
            {
                list = new List<List<TestLisResultADO>>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return list;
        }

        V_HIS_TEST_INDEX_RANGE GetTestIndexRange(long dob, long genderCode, string testIndexId, ref List<V_HIS_TEST_INDEX_RANGE> testIndexRanges)
        {
            MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX_RANGE testIndexRange = new V_HIS_TEST_INDEX_RANGE();
            try
            {
                if (testIndexRanges != null && testIndexRanges.Count > 0)
                {
                    double age = 0;
                    List<V_HIS_TEST_INDEX_RANGE> query = new List<V_HIS_TEST_INDEX_RANGE>();
                    var ageDate = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dob);
                    foreach (var item in testIndexRanges)
                    {
                        if (item.TEST_INDEX_CODE == testIndexId)
                        {
                            if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__YEAR) // Năm
                            {
                                age = (DateTime.Now - (ageDate ?? DateTime.Now)).TotalDays / 365;
                            }
                            else if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__MONTH) // Tháng
                            {
                                age = (DateTime.Now - (ageDate ?? DateTime.Now)).TotalDays / 30;
                            }
                            else if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__DAY) // Ngày
                            {
                                age = (DateTime.Now - (ageDate ?? DateTime.Now)).TotalDays;
                            }
                            else if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__HOUR) // Giờ
                            {
                                age = (DateTime.Now - (ageDate ?? DateTime.Now)).TotalHours;
                            }
                            if (((item.AGE_FROM.HasValue && item.AGE_FROM.Value <= age) || !item.AGE_FROM.HasValue)
                                && ((item.AGE_TO.HasValue && item.AGE_TO.Value >= age) || !item.AGE_TO.HasValue))
                            {
                                query.Add(item);
                            }
                        }
                    }
                    HIS_GENDER gender = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(p => p.ID == genderCode);
                    if (gender != null && gender.ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE)
                    {
                        query = query.Where(o => o.IS_MALE == 1).ToList();
                    }
                    else if (gender != null && gender.ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
                    {
                        query = query.Where(o => o.IS_FEMALE == 1).ToList();
                    }
                    testIndexRange = query.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                testIndexRange = new V_HIS_TEST_INDEX_RANGE();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return testIndexRange;
        }

        private bool SaveValue(ref CommonParam param, ref bool print, ref bool hasConfirmUser, ref bool resultConfirmUser)
        {
            bool result = false;
            try
            {
                positionHandle = -1;
                IsCallApi = false;
                treeListSereServTein.PostEditor();
                List<TestLisResultADO> dataGrid = new List<TestLisResultADO>();
                var AllCheckNodes = treeListSereServTein.GetAllCheckedNodes();

                if (AllCheckNodes == null || AllCheckNodes.Count() == 0)
                    return result;
                foreach (var item in AllCheckNodes)
                {
                    var data = (TestLisResultADO)this.treeListSereServTein.GetDataRecordByNode(item);
                    dataGrid.Add(data);
                }

                // bỏ is_no_execute
                dataGrid = dataGrid.Where(o => o.IS_NO_EXECUTE != 1).ToList();
                LisSampleResultSDO lisSampleResultSDO = new LIS.SDO.LisSampleResultSDO();

                List<UpdateResultValueSDO> updateAdo = new List<UpdateResultValueSDO>();
                var dataParent = dataGrid.Where(p => p.IS_PARENT == 1).ToList();
                List<TestLisResultADO> listNotValues = new List<TestLisResultADO>();
                bool IsFullValues = true;
                if (!String.IsNullOrWhiteSpace(txtNote.Text) && Encoding.UTF8.GetByteCount(txtNote.Text.Trim()) > 500)
                {
                    param.Messages.Add("Ghi chú có giá trị vượt quá độ dài cho phép [500 kí tự].");
                    if (xtraTabControl.SelectedTabPageIndex == 0)
                    {
                        txtNote.Focus();
                        txtNote.SelectAll();
                    }
                    return result;
                }
                string warningMaxlength = "";
                if (!String.IsNullOrWhiteSpace(txtDescription.Text) && Encoding.UTF8.GetByteCount(txtDescription.Text.Trim()) > 1000)
                {
                    warningMaxlength += "Nhận xét";
                }
                if (!String.IsNullOrWhiteSpace(txtConclude.Text) && Encoding.UTF8.GetByteCount(txtConclude.Text.Trim()) > 1000)
                {
                    if (String.IsNullOrEmpty(warningMaxlength))
                        warningMaxlength += "Kết luận";
                    else
                        warningMaxlength += ", Kết luận";
                }
                if (!String.IsNullOrEmpty(warningMaxlength))
                {
                    param.Messages.Add("Thông tin " + warningMaxlength + " vượt quá 1000 ký tự.");
                    if (xtraTabControl.SelectedTabPageIndex == 1)
                    {
                        txtDescription.Focus();
                        txtDescription.SelectAll();
                    }
                    else if (xtraTabControl.SelectedTabPageIndex == 2)
                    {
                        txtConclude.Focus();
                        txtConclude.SelectAll();
                    }
                    return result;
                }
                if (dataGrid != null && dataGrid.Count > 0)
                {

                    if (!CheckMachine(dataParent))
                    {
                        return result;
                    }
                    // check dich vu
                    List<string> ListErrSampleTime = new List<string>();
                    foreach (var item in dataParent)
                    {
                        var service = BackendDataWorker.Get<HIS_SERVICE>().FirstOrDefault(o => o.SERVICE_CODE == item.SERVICE_CODE);
                        if (rowSample.SAMPLE_TIME != null)
                        {
                            // 61459
                            if (string.IsNullOrEmpty(service.MIN_PROC_TIME_EXCEPT_PATY_IDS) || !service.MIN_PROC_TIME_EXCEPT_PATY_IDS.Split(',').Contains(item.PATIENT_TYPE_ID_BY_SERE_SERV.ToString()))
                            {
                                TimeSpan time = DateKQ.DateTime - (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Convert.ToInt64(rowSample.SAMPLE_TIME.ToString().Substring(0, 12) + "00"));
                                double timeCheck = time.TotalMinutes;

                                if (timeCheck < service.MIN_PROCESS_TIME)
                                {
                                    ListErrSampleTime.Add(string.Format("{0} ít hơn {1} phút", item.SERVICE_CODE, service.MIN_PROCESS_TIME));
                                }
                            }
                            if (string.IsNullOrEmpty(service.MAX_PROC_TIME_EXCEPT_PATY_IDS) || !service.MAX_PROC_TIME_EXCEPT_PATY_IDS.Split(',').Contains(item.PATIENT_TYPE_ID_BY_SERE_SERV.ToString()))
                            {
                                TimeSpan time = DateKQ.DateTime - (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Convert.ToInt64(rowSample.SAMPLE_TIME.ToString().Substring(0, 12) + "00"));
                                double timeCheck = time.TotalMinutes;
                                if (timeCheck > service.MAX_PROCESS_TIME)
                                {
                                    ListErrSampleTime.Add(string.Format("{0} lớn hơn {1} phút", item.SERVICE_CODE, service.MAX_PROCESS_TIME));
                                }
                            }
                        }

                        // TODO
                    }
                    if (ListErrSampleTime.Count() > 0)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Bệnh nhân có thời gian thực hiện xét nghiệm dịch vụ:\r\n" + string.Join("\r\n", ListErrSampleTime), "Thông báo");
                        return result;
                    }
                    WaitingManager.Show();
                    foreach (var item in dataGrid)
                    {
                        if (item.LIS_RESULT_ID == null || item.LIS_RESULT_ID == 0)
                            continue;
                        UpdateResultValueSDO ado = new UpdateResultValueSDO();
                        ado.ResultId = item.LIS_RESULT_ID.Value;
                        ado.Value = item.VALUE_RANGE;
                        if (String.IsNullOrWhiteSpace(ado.Value))
                        {
                            listNotValues.Add(item);
                            IsFullValues = false;
                        }
                        ado.MachineId = item.MACHINE_ID_OLD;
                        ado.Description = item.NOTE;
                        ado.ResultDescription = item.RESULT_DESCRIPTION;
                        if (dataParent != null && dataParent.Count > 0)
                        {
                            var _parent = dataParent.FirstOrDefault(p => p.CHILD_ID == item.PARENT_ID);
                            if (_parent != null && _parent.MACHINE_ID != _parent.MACHINE_ID_OLD)
                            {
                                ado.MachineId = _parent.MACHINE_ID;
                            }
                        }

                        updateAdo.Add(ado);
                    }

                    if (updateAdo == null || updateAdo.Count <= 0)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.ChuaChonChiSoIn, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                        print = false;
                        return false;
                    }

                    bool showMessage = true;

                    if (updateAdo != null && updateAdo.Count > 0 &&
                        (IsFullValues
                        || LisConfigCFG.ALLOW_SAVE_WHEN_EMPTY == "1"
                        || this.ConfirmUser(listNotValues, ref showMessage, ref hasConfirmUser, ref resultConfirmUser))
                        || (LisConfigCFG.ALLOW_SAVE_WHEN_EMPTY == "3" && this.CheckOption3(listNotValues)))
                    {

                        var sample = (LisSampleADO)gridViewSample.GetFocusedRow();
                        lisSampleResultSDO.ResultValues = updateAdo;
                        lisSampleResultSDO.Note = txtNote.Text.Trim();
                        lisSampleResultSDO.Description = txtDescription.Text.Trim();
                        lisSampleResultSDO.Conclude = txtConclude.Text.Trim();
                        lisSampleResultSDO.SampleTime = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(DateLM.EditValue).ToString("yyyyMMddHHmm00"));
                        if (string.IsNullOrEmpty(sample.SAMPLE_LOGINNAME) && string.IsNullOrEmpty(sample.SAMPLE_USERNAME))
                        {
                            lisSampleResultSDO.SampleLoginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                            lisSampleResultSDO.SampleUsername = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                        }
                        else
                        {
                            lisSampleResultSDO.SampleLoginname = sample.SAMPLE_LOGINNAME;
                            lisSampleResultSDO.SampleUsername = sample.SAMPLE_USERNAME;
                        }
                        lisSampleResultSDO.SampleId = sample.ID;
                        if (HisConfigCFG.CHECK_VALUE_MAXLENGTH_OPTION == "1")
                        {
                            var dataGridMaxlengthValue = dataGrid.Where(o => !String.IsNullOrEmpty(o.VALUE_RANGE) && Encoding.UTF8.GetByteCount(o.VALUE_RANGE) > 50);
                            if (dataGridMaxlengthValue != null && dataGridMaxlengthValue.Count() > 0)
                            {
                                WaitingManager.Hide();
                                var indexName = dataGridMaxlengthValue.Select(o => o.TEST_INDEX_NAME).ToList();
                                MessageBox.Show("Chỉ số " + string.Join(",", indexName) + " có giá trị vượt quá độ dài cho phép 50 ký tự.", "Thông báo");
                                return result;
                            }



                            //param.Messages.Add(String.Format("({0}) có giá trị vượt qúa độ dài cho phép [50 kí tự].", String.Join(", ", dataGridMaxlengthValue.Select(o => o.TEST_INDEX_NAME).ToList())));
                            //return result;
                        }
                        if (HisConfigCFG.CHECK_VALUE_MAXLENGTH_OPTION == "2")
                        {
                            var dataGridMaxlengthValue = dataGrid.Where(o => !String.IsNullOrEmpty(o.VALUE_RANGE) && Encoding.UTF8.GetByteCount(o.VALUE_RANGE) > 50);

                            if (dataGridMaxlengthValue != null && dataGridMaxlengthValue.Count() > 0)
                            {
                                WaitingManager.Hide();
                                var indexName = dataGridMaxlengthValue.Select(o => o.TEST_INDEX_NAME).ToList();
                                if (MessageBox.Show("Chỉ số " + string.Join(",", indexName) + " có giá trị vượt quá độ dài cho phép 50 ký tự. Bạn có muốn tiếp tục thực hiện không", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    IsCallApi = true;
                                    var rs = new BackendAdapter(param).Post<bool>("api/LisSample/UpdateResult", ApiConsumers.LisConsumer, lisSampleResultSDO, param);
                                    if (rs)
                                    {
                                        print = true;
                                        result = true;
                                        UpdateDataForSavePrintSuccess();
                                        //toggleSwitchSelectAll_EditValueChanged(null, null);

                                        string testIndexStr = "";

                                        var dataGridFilter = dataGrid.Where(o => o.LIS_RESULT_ID != null && o.LIS_RESULT_ID > 0).ToList();
                                        foreach (var item in dataGridFilter)
                                        {
                                            testIndexStr += item.TEST_INDEX_NAME + " - " + item.VALUE_RANGE + "; ";
                                        }

                                        string message = string.Format("Lưu kết quả xét nghiệm. SERVICE_REQ_CODE: {0}. BARCODE: {1}.  TEST_INDEX_NAME - VALUE: [{2}]", rowSample.SERVICE_REQ_CODE, rowSample.BARCODE, testIndexStr);
                                        string login = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                                        SdaEventLogCreate eventlog = new SdaEventLogCreate();
                                        eventlog.Create(login, null, true, message);
                                    }
                                    WaitingManager.Hide();
                                }
                            }
                            else
                            {
                                IsCallApi = true;
                                var rs = new BackendAdapter(param).Post<bool>("api/LisSample/UpdateResult", ApiConsumers.LisConsumer, lisSampleResultSDO, param);
                                if (rs)
                                {
                                    print = true;
                                    result = true;
                                    UpdateDataForSavePrintSuccess();
                                    //toggleSwitchSelectAll_EditValueChanged(null, null);

                                    string testIndexStr = "";

                                    var dataGridFilter = dataGrid.Where(o => o.LIS_RESULT_ID != null && o.LIS_RESULT_ID > 0).ToList();
                                    foreach (var item in dataGridFilter)
                                    {
                                        testIndexStr += item.TEST_INDEX_NAME + " - " + item.VALUE_RANGE + "; ";
                                    }

                                    string message = string.Format("Lưu kết quả xét nghiệm. SERVICE_REQ_CODE: {0}. BARCODE: {1}.  TEST_INDEX_NAME - VALUE: [{2}]", rowSample.SERVICE_REQ_CODE, rowSample.BARCODE, testIndexStr);
                                    string login = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                                    SdaEventLogCreate eventlog = new SdaEventLogCreate();
                                    eventlog.Create(login, null, true, message);

                                }
                            }
                        }
                        else
                        {
                            IsCallApi = true;
                            var rs = new BackendAdapter(param).Post<bool>("api/LisSample/UpdateResult", ApiConsumers.LisConsumer, lisSampleResultSDO, param);
                            if (rs)
                            {
                                print = true;
                                result = true;
                                UpdateDataForSavePrintSuccess();
                                //toggleSwitchSelectAll_EditValueChanged(null, null);

                                string testIndexStr = "";

                                var dataGridFilter = dataGrid.Where(o => o.LIS_RESULT_ID != null && o.LIS_RESULT_ID > 0).ToList();
                                foreach (var item in dataGridFilter)
                                {
                                    testIndexStr += item.TEST_INDEX_NAME + " - " + item.VALUE_RANGE + "; ";
                                }

                                string message = string.Format("Lưu kết quả xét nghiệm. SERVICE_REQ_CODE: {0}. BARCODE: {1}.  TEST_INDEX_NAME - VALUE: [{2}]", rowSample.SERVICE_REQ_CODE, rowSample.BARCODE, testIndexStr);
                                string login = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                                SdaEventLogCreate eventlog = new SdaEventLogCreate();
                                eventlog.Create(login, null, true, message);
                            }
                            WaitingManager.Hide();
                        }
                    }
                    else if ((updateAdo == null || updateAdo.Count <= 0) && IsFullValues)
                    {
                        print = true;
                    }
                    else
                    {
                        result = false;
                        WaitingManager.Hide();
                        if (showMessage && LisConfigCFG.ALLOW_SAVE_WHEN_EMPTY != "3")
                            param.Messages.Add(String.Format(Resources.ResourceMessage.ChiSoChuaNhapGiaTri, String.Join(", ", listNotValues.Select(o => o.TEST_INDEX_NAME).ToList())));
                        Inventec.Common.Logging.LogSystem.Info(String.Format("({0}) chưa nhập giá trị cho chỉ số.", String.Join(", ", listNotValues.Select(o => o.TEST_INDEX_NAME).ToList())));
                    }
                }
                else
                {
                    WaitingManager.Hide();
                    param.Messages.Add("Chưa nhập giá trị cho chỉ số hoặc không thay đổi để lưu kết quả");
                    Inventec.Common.Logging.LogSystem.Info("Chưa nhập giá trị cho chỉ số hoặc không thay đổi");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void GetSampleTime(long sampleTime)
        {
            try
            {
                this.simpleTime = sampleTime;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        private void getValueFromDlg(bool rs)
        {
            try
            {
                this.isReturn = rs;
                if (this.isReturn) FillDataToGridControl();
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
                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;
                rowSample = (LisSampleADO)gridViewSample.GetFocusedRow();
                if (HisConfigCFG.AllowToEnterSampleTime)
                {
                    if (rowSample.SAMPLE_TIME == null)
                    {
                        frmAddTimeSample frm = new frmAddTimeSample(getValueFromDlg, rowSample);
                        frm.ShowDialog();
                        if (this.isReturn) return;
                    }
                }
                long? sample_time = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateLM.DateTime);
                long? result_time = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateKQ.DateTime);
                var serviceReq = GetServiceReq();
                if ((HisConfigCFG.StartTimeMustBeGreaterThanInstructionTime == "1" || HisConfigCFG.StartTimeMustBeGreaterThanInstructionTime == "2") && result_time < serviceReq.INTRUCTION_TIME)
                {
                    XtraMessageBox.Show("Thời gian trả kết quả không được nhỏ hơn thời gian y lệnh.", "Thông báo");
                    return;
                }
                if (rowSample.APPROVAL_TIME != null && sample_time > rowSample.APPROVAL_TIME)
                {
                    XtraMessageBox.Show(String.Format("Thời gian lấy mẫu {0} phải nhỏ hơn thời gian duyệt mẫu {1}", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(sample_time ?? 0), Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rowSample.APPROVAL_TIME ?? 0)), "Thông báo");
                    return;
                }
                if (rowSample.APPROVAL_TIME != null && result_time < rowSample.APPROVAL_TIME)
                {
                    XtraMessageBox.Show(String.Format("Thời gian duyệt mẫu {0} phải nhỏ hơn thời gian trả kết quả {1}", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rowSample.APPROVAL_TIME ?? 0), Inventec.Common.DateTime.Convert.TimeNumberToTimeString(result_time ?? 0)), "Thông báo");
                    return;
                }

                if (chkSignProcess.Checked && (SignConfigData == null || SignConfigData.listSign == null || SignConfigData.listSign.Count == 0))
                {
                    if (XtraMessageBox.Show("Bạn chưa thiết lập luồng ký. Bạn có muốn thiết lập luồng ký không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        SignConfigData = new SignConfigADO();
                        HIS.Desktop.Plugins.ConnectionTest.AddSigner.frmSignerAdd frmAddSigner = new HIS.Desktop.Plugins.ConnectionTest.AddSigner.frmSignerAdd(SignConfigData.listSign, UpdateAfterAddSignThread, SignConfigData.IsSignParanel);
                        frmAddSigner.ShowDialog(this.ParentForm);
                    }
                }


                treeListSereServTein.PostEditor();
                btnSave.Focus();
                if (chkReturnResult.Checked)
                {
                    //LisSampleADO row = (LisSampleADO)gridViewSample.GetFocusedRow();
                    //DateTime inKQ = DateKQ.DateTime;
                    //DateTime inINTRUCTION = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(row.INTRUCTION_TIME ?? 0) ?? DateTime.Now;
                    //if (inKQ < inINTRUCTION)
                    //{
                    //    //ValidateTimeControl(true);
                    //}
                }
                if (!String.IsNullOrWhiteSpace(txtNote.Text) && Encoding.UTF8.GetByteCount(txtNote.Text) > 500)
                {
                    MessageManager.Show("Ghi chú vượt quá độ dài cho phép [500 kí tự]");
                    if (xtraTabControl.SelectedTabPageIndex == 1)
                    {
                        txtNote.Focus();
                        txtNote.SelectAll();
                    }
                    return;
                }
                string warningMaxlength = "";
                if (!String.IsNullOrWhiteSpace(txtDescription.Text) && Encoding.UTF8.GetByteCount(txtDescription.Text.Trim()) > 1000)
                {
                    warningMaxlength += "Nhận xét";
                }
                if (!String.IsNullOrWhiteSpace(txtConclude.Text) && Encoding.UTF8.GetByteCount(txtConclude.Text.Trim()) > 1000)
                {
                    if (String.IsNullOrEmpty(warningMaxlength))
                        warningMaxlength += "Kết luận";
                    else
                        warningMaxlength += ", Kết luận";
                }
                if (!String.IsNullOrEmpty(warningMaxlength))
                {
                    MessageManager.Show("Thông tin " + warningMaxlength + " vượt quá 1000 ký tự.");
                    if (xtraTabControl.SelectedTabPageIndex == 1)
                    {
                        txtDescription.Focus();
                        txtDescription.SelectAll();
                    }
                    else if (xtraTabControl.SelectedTabPageIndex == 2)
                    {
                        txtConclude.Focus();
                        txtConclude.SelectAll();
                    }
                    return;
                }
                List<TestLisResultADO> dataGrid = new List<TestLisResultADO>();
                var AllCheckNodes = treeListSereServTein.GetAllCheckedNodes();
                foreach (var item in AllCheckNodes)
                {
                    var data = (TestLisResultADO)this.treeListSereServTein.GetDataRecordByNode(item);
                    dataGrid.Add(data);
                }
                dataGrid = dataGrid.Where(o => o.IS_NO_EXECUTE != 1).ToList();
                List<UpdateResultValueSDO> updateAdo = new List<UpdateResultValueSDO>();
                var dataParent = dataGrid.Where(p => p.IS_PARENT == 1).ToList();

                // check dich vu
                List<string> ListErrSampleTime = new List<string>();
                Dictionary<string, string> ListErrService = new Dictionary<string, string>();
                // check may
                Dictionary<string, string> ListWarningMachine = new Dictionary<string, string>();
                foreach (var item in dataParent)
                {
                    if (item.MACHINE_ID != null)
                    {
                        var checkData = CheckConfigWarningMachine(item.MACHINE_ID ?? 0, item);
                        if (checkData != null && !String.IsNullOrEmpty(checkData.MACHINE_NAME) && !ListWarningMachine.ContainsKey(checkData.MACHINE_NAME))
                        {
                            ListWarningMachine.Add(checkData.MACHINE_NAME, checkData.MAX_SERVICE_PER_DAY.ToString());
                        }
                    }
                    //
                    var service = BackendDataWorker.Get<HIS_SERVICE>().FirstOrDefault(o => o.SERVICE_CODE == item.SERVICE_CODE);
                    if (rowSample.SAMPLE_TIME != null)
                    {
                        // 61459
                        if (string.IsNullOrEmpty(service.MIN_PROC_TIME_EXCEPT_PATY_IDS) || !service.MIN_PROC_TIME_EXCEPT_PATY_IDS.Split(',').Contains(item.PATIENT_TYPE_ID_BY_SERE_SERV.ToString()))
                        {
                            TimeSpan time = DateKQ.DateTime - (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Convert.ToInt64(rowSample.SAMPLE_TIME.ToString().Substring(0, 12) + "00"));
                            double timeCheck = time.TotalMinutes;

                            if (timeCheck < service.MIN_PROCESS_TIME)
                            {
                                ListErrSampleTime.Add(string.Format("{0} ít hơn {1} phút", item.SERVICE_CODE, service.MIN_PROCESS_TIME));
                            }
                        }
                        if (string.IsNullOrEmpty(service.MAX_PROC_TIME_EXCEPT_PATY_IDS) || !service.MAX_PROC_TIME_EXCEPT_PATY_IDS.Split(',').Contains(item.PATIENT_TYPE_ID_BY_SERE_SERV.ToString()))
                        {
                            TimeSpan time = DateKQ.DateTime - (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Convert.ToInt64(rowSample.SAMPLE_TIME.ToString().Substring(0, 12) + "00"));
                            double timeCheck = time.TotalMinutes;
                            if (timeCheck > service.MAX_PROCESS_TIME)
                            {
                                ListErrSampleTime.Add(string.Format("{0} lớn hơn {1} phút", item.SERVICE_CODE, service.MAX_PROCESS_TIME));
                            }
                        }
                    }
                    if (service.MAX_TOTAL_PROCESS_TIME != null && service.MAX_TOTAL_PROCESS_TIME > 0 && (string.IsNullOrEmpty(service.TOTAL_TIME_EXCEPT_PATY_IDS) || !service.TOTAL_TIME_EXCEPT_PATY_IDS.Split(',').Contains(item.PATIENT_TYPE_ID_BY_SERE_SERV.ToString())))
                    {
                        TimeSpan time = DateKQ.DateTime - (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.INTRUCTION_TIME);
                        double timeCheck = time.TotalMinutes;
                        if (timeCheck > service.MAX_TOTAL_PROCESS_TIME)
                        {
                            if (!String.IsNullOrEmpty(item.SERVICE_NAME) && !ListErrService.ContainsKey(item.SERVICE_NAME))
                            {
                                ListErrService.Add(item.SERVICE_NAME, service.MAX_TOTAL_PROCESS_TIME.ToString());
                            }
                        }
                    }

                    // TODO
                }
                if (ListErrSampleTime.Count() > 0)
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bệnh nhân có thời gian thực hiện xét nghiệm dịch vụ:\r\n" + string.Join("\r\n", ListErrSampleTime), "Thông báo");
                    return;
                }
                if (ListErrService.Count() > 0)
                {
                    WaitingManager.Hide();
                    string msg = "";
                    var ListErrServiceGroup = ListErrService.GroupBy(o => o.Value);
                    if (HisConfigCFG.ProcessTimeMustBeGreaterThanTotalProcessTime == "1")
                    {
                        foreach (var item in ListErrServiceGroup)
                        {
                            msg += string.Format("Không cho phép trả kết quả dịch vụ {0} sau {1} phút tính từ thời điểm ra y lệnh {2}\r\n", string.Join(",", item.Select(o => o.Key).ToList()), item.First().Value, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(serviceReq.INTRUCTION_TIME));
                        }
                        DevExpress.XtraEditors.XtraMessageBox.Show(msg, "Thông báo");
                        return;
                    }
                    else if (HisConfigCFG.ProcessTimeMustBeGreaterThanTotalProcessTime == "2")
                    {
                        foreach (var item in ListErrServiceGroup)
                        {
                            msg += string.Format("Trả kết quả dịch vụ {0} vượt quá {1} phút tính từ thời điểm ra y lệnh {2}\r\n", string.Join(",", item.Select(o => o.Key).ToList()), item.First().Value, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(serviceReq.INTRUCTION_TIME));
                        }
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(msg + "Bạn có muốn tiếp tục không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            return;
                        }
                    }
                }
                if (ListWarningMachine.Count() > 0)
                {
                    WaitingManager.Hide();
                    string msg = "";
                    var ListWarningMachineGroup = ListWarningMachine.GroupBy(o => o.Value);
                    foreach (var item in ListWarningMachineGroup)
                    {
                        msg += string.Format("Máy {0} vượt quá số lượng dịch vụ xử lý tối đa trong ngày ({1})\r\n", string.Join(",", item.Select(o => o.Key).ToList()), item.First().Value);
                    }
                    if (HisConfigCFG.MaxServicePerDayWarningOption == "1")
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("{0}. Bạn có muốn tiếp tục?", msg), "Thông báo",
                          MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            return;
                        }
                    }
                    else if (HisConfigCFG.MaxServicePerDayWarningOption == "2")
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(msg, "Thông báo");
                        return;
                    }
                }
                // 50262

                List<TestLisResultADO> listNotValues = new List<TestLisResultADO>();
                bool IsFullValues = true;
                if (dataGrid != null && dataGrid.Count > 0)
                {
                    IsNotLoadData = true;
                    LisSampleResultSDO lisSampleResultSDO = new LisSampleResultSDO();
                    var dataGridMaxlengthDes = dataGrid.Where(o => !String.IsNullOrEmpty(o.NOTE) && Encoding.UTF8.GetByteCount(o.NOTE) > 200);
                    if (dataGridMaxlengthDes != null && dataGridMaxlengthDes.Count() > 0)
                    {
                        MessageManager.Show(String.Join(", ", dataGridMaxlengthDes.Select(o => o
                            .TEST_INDEX_NAME).ToList()) + " ghi chú vượt quá độ dài cho phép [200 kí tự]");
                        return;
                    }

                    var dataGridMaxlengthResultDescription = dataGrid.Where(o => !String.IsNullOrEmpty(o.RESULT_DESCRIPTION) && Encoding.UTF8.GetByteCount(o.RESULT_DESCRIPTION) > 1000);
                    if (dataGridMaxlengthResultDescription != null && dataGridMaxlengthResultDescription.Count() > 0)
                    {
                        MessageManager.Show(String.Join(", ", dataGridMaxlengthResultDescription.Select(o => o
                            .TEST_INDEX_NAME).ToList()) + " mô tả vượt quá độ dài cho phép [1000 kí tự]");
                        return;
                    }

                    if (!CheckMachine(dataParent))
                    {
                        return;
                    }
                    WaitingManager.Show();



                    foreach (var item in dataGrid)
                    {

                        if (item.LIS_RESULT_ID == null || item.LIS_RESULT_ID == 0)
                            continue;
                        UpdateResultValueSDO ado = new UpdateResultValueSDO();
                        ado.ResultId = item.LIS_RESULT_ID.Value;
                        ado.Value = item.VALUE_RANGE;

                        ado.MachineId = item.MACHINE_ID;
                        ado.Description = item.NOTE;
                        ado.ResultDescription = item.RESULT_DESCRIPTION;
                        if (dataParent != null && dataParent.Count > 0)
                        {
                            var _parent = dataParent.FirstOrDefault(p => p.CHILD_ID == item.PARENT_ID);
                            if (_parent != null && _parent.MACHINE_ID != _parent.MACHINE_ID_OLD)
                            {
                                ado.MachineId = _parent.MACHINE_ID;
                                item.MACHINE_ID = _parent.MACHINE_ID;
                            }
                        }
                        if (String.IsNullOrWhiteSpace(ado.Value))
                        {
                            listNotValues.Add(item);
                            IsFullValues = false;
                        }

                        updateAdo.Add(ado);
                    }

                    CommonParam param = new CommonParam();
                    bool success = false;

                    if (updateAdo == null || updateAdo.Count <= 0)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.ChuaChonChiSoLuuKetQua, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                        return;
                    }

                    bool showMessage = true;
                    bool hasConfirmUser = false;
                    bool resultConfirmUser = false;

                    if (IsFullValues
                        || LisConfigCFG.ALLOW_SAVE_WHEN_EMPTY == "1"
                        || this.ConfirmUser(listNotValues, ref showMessage, ref hasConfirmUser, ref resultConfirmUser)
                        || (LisConfigCFG.ALLOW_SAVE_WHEN_EMPTY == "3" && this.CheckOption3(listNotValues)))
                    {
                        var sample = (LisSampleADO)gridViewSample.GetFocusedRow();
                        lisSampleResultSDO.ResultValues = updateAdo;
                        lisSampleResultSDO.Note = txtNote.Text.Trim();
                        lisSampleResultSDO.Description = txtDescription.Text.Trim();
                        lisSampleResultSDO.Conclude = txtConclude.Text.Trim();
                        lisSampleResultSDO.SampleId = sample.ID;
                        lisSampleResultSDO.SampleTime = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(DateLM.EditValue).ToString("yyyyMMddHHmm00"));
                        if (string.IsNullOrEmpty(sample.SAMPLE_LOGINNAME) && string.IsNullOrEmpty(sample.SAMPLE_USERNAME))
                        {
                            lisSampleResultSDO.SampleLoginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                            lisSampleResultSDO.SampleUsername = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                        }
                        else
                        {
                            lisSampleResultSDO.SampleLoginname = sample.SAMPLE_LOGINNAME;
                            lisSampleResultSDO.SampleUsername = sample.SAMPLE_USERNAME;
                        }
                        if (HisConfigCFG.CHECK_VALUE_MAXLENGTH_OPTION == "1")
                        {
                            var dataGridMaxlengthValue = dataGrid.Where(o => !String.IsNullOrEmpty(o.VALUE_RANGE) && Encoding.UTF8.GetByteCount(o.VALUE_RANGE) > 50);
                            if (dataGridMaxlengthValue != null && dataGridMaxlengthValue.Count() > 0)
                            {
                                WaitingManager.Hide();
                                var indexName = dataGridMaxlengthValue.Select(o => o.TEST_INDEX_NAME).ToList();
                                MessageBox.Show("Chỉ số " + string.Join(",", indexName) + " có giá trị vượt quá độ dài cho phép 50 ký tự.", "Thông báo");
                                return;
                            }
                        }

                        if (HisConfigCFG.CHECK_VALUE_MAXLENGTH_OPTION == "2")
                        {
                            var dataGridMaxlengthValue = dataGrid.Where(o => !String.IsNullOrEmpty(o.VALUE_RANGE) && Encoding.UTF8.GetByteCount(o.VALUE_RANGE) > 50);
                            if (dataGridMaxlengthValue != null && dataGridMaxlengthValue.Count() > 0)
                            {
                                WaitingManager.Hide();
                                var indexName = dataGridMaxlengthValue.Select(o => o.TEST_INDEX_NAME).ToList();
                                if (MessageBox.Show("Chỉ số " + string.Join(",", indexName) + " có giá trị vượt quá độ dài cho phép 50 ký tự. Bạn có muốn tiếp tục thực hiện không", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    #region
                                    IsLoadGridSampleAfterSave = true;
                                    var rs = new BackendAdapter(param).Post<bool>("api/LisSample/UpdateResult", ApiConsumers.LisConsumer, lisSampleResultSDO, param);
                                    if (rs)
                                    {
                                        success = true;
                                        UpdateDataForSaveSuccess();

                                        string testIndexStr = "";

                                        var dataGridFilter = dataGrid.Where(o => o.LIS_RESULT_ID != null && o.LIS_RESULT_ID > 0).ToList();
                                        foreach (var item in dataGridFilter)
                                        {
                                            testIndexStr += item.TEST_INDEX_NAME + " - " + item.VALUE_RANGE + "; ";
                                        }

                                        string message = string.Format("Lưu kết quả xét nghiệm. SERVICE_REQ_CODE: {0}. BARCODE: {1}.  TEST_INDEX_NAME - VALUE: [{2}]", rowSample.SERVICE_REQ_CODE, rowSample.BARCODE, testIndexStr);
                                        string login = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                                        SdaEventLogCreate eventlog = new SdaEventLogCreate();
                                        eventlog.Create(login, null, true, message);

                                        if (rowSample != null && chkReturnResult.CheckState == CheckState.Checked)
                                        {
                                            CommonParam paramReturn = new CommonParam();
                                            LIS_SAMPLE returnResult = null;
                                            if (ReturnResult(ref paramReturn, ref returnResult))
                                            {
                                                if (returnResult == null)
                                                {
                                                    paramReturn.Messages.Add("Tự động trả kết quả thất bại");
                                                    param.Messages.AddRange(paramReturn.Messages);
                                                }
                                                Inventec.Common.Mapper.DataObjectMapper.Map<LisSampleADO>(rowSample, returnResult);
                                            }
                                        }

                                        #region Process has exception
                                        SessionManager.ProcessTokenLost(param);
                                        #endregion

                                    }
                                    WaitingManager.Hide();
                                    MessageManager.Show(this.ParentForm, param, success);
                                    #endregion
                                }
                            }
                            else
                            {
                                IsLoadGridSampleAfterSave = true;
                                var rs = new BackendAdapter(param).Post<bool>("api/LisSample/UpdateResult", ApiConsumers.LisConsumer, lisSampleResultSDO, param);
                                if (rs)
                                {
                                    success = true;
                                    UpdateDataForSaveSuccess();

                                    string testIndexStr = "";

                                    var dataGridFilter = dataGrid.Where(o => o.LIS_RESULT_ID != null && o.LIS_RESULT_ID > 0).ToList();
                                    foreach (var item in dataGridFilter)
                                    {
                                        testIndexStr += item.TEST_INDEX_NAME + " - " + item.VALUE_RANGE + "; ";
                                    }

                                    string message = string.Format("Lưu kết quả xét nghiệm. SERVICE_REQ_CODE: {0}. BARCODE: {1}.  TEST_INDEX_NAME - VALUE: [{2}]", rowSample.SERVICE_REQ_CODE, rowSample.BARCODE, testIndexStr);
                                    string login = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                                    SdaEventLogCreate eventlog = new SdaEventLogCreate();
                                    eventlog.Create(login, null, true, message);

                                    if (rowSample != null && chkReturnResult.CheckState == CheckState.Checked)
                                    {
                                        CommonParam paramReturn = new CommonParam();
                                        LIS_SAMPLE returnResult = null;
                                        if (ReturnResult(ref paramReturn, ref returnResult))
                                        {
                                            if (returnResult == null)
                                            {
                                                paramReturn.Messages.Add("Tự động trả kết quả thất bại");
                                                param.Messages.AddRange(paramReturn.Messages);
                                            }
                                            Inventec.Common.Mapper.DataObjectMapper.Map<LisSampleADO>(rowSample, returnResult);
                                        }
                                    }

                                    #region Process has exception
                                    SessionManager.ProcessTokenLost(param);
                                    #endregion

                                }
                                WaitingManager.Hide();
                                MessageManager.Show(this.ParentForm, param, success);
                            }


                        }
                        else
                        {
                            IsLoadGridSampleAfterSave = true;
                            var rs = new BackendAdapter(param).Post<bool>("api/LisSample/UpdateResult", ApiConsumers.LisConsumer, lisSampleResultSDO, param);
                            if (rs)
                            {
                                success = true;
                                UpdateDataForSaveSuccess();

                                string testIndexStr = "";

                                var dataGridFilter = dataGrid.Where(o => o.LIS_RESULT_ID != null && o.LIS_RESULT_ID > 0).ToList();
                                foreach (var item in dataGridFilter)
                                {
                                    testIndexStr += item.TEST_INDEX_NAME + " - " + item.VALUE_RANGE + "; ";
                                }

                                string message = string.Format("Lưu kết quả xét nghiệm. SERVICE_REQ_CODE: {0}. BARCODE: {1}.  TEST_INDEX_NAME - VALUE: [{2}]", rowSample.SERVICE_REQ_CODE, rowSample.BARCODE, testIndexStr);
                                string login = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                                SdaEventLogCreate eventlog = new SdaEventLogCreate();
                                eventlog.Create(login, null, true, message);

                                if (rowSample != null && chkReturnResult.CheckState == CheckState.Checked)
                                {
                                    CommonParam paramReturn = new CommonParam();
                                    LIS_SAMPLE returnResult = null;
                                    if (ReturnResult(ref paramReturn, ref returnResult))
                                    {
                                        if (returnResult == null)
                                        {
                                            paramReturn.Messages.Add("Tự động trả kết quả thất bại");
                                            param.Messages.AddRange(paramReturn.Messages);
                                        }
                                        Inventec.Common.Mapper.DataObjectMapper.Map<LisSampleADO>(rowSample, returnResult);
                                    }
                                }

                                #region Process has exception
                                SessionManager.ProcessTokenLost(param);
                                #endregion

                            }
                            WaitingManager.Hide();
                            MessageManager.Show(this.ParentForm, param, success);
                        }

                    }
                    else
                    {
                        WaitingManager.Hide();
                        if (showMessage && LisConfigCFG.ALLOW_SAVE_WHEN_EMPTY != "3")
                            MessageManager.Show(String.Format(Resources.ResourceMessage.ChiSoChuaNhapGiaTri, String.Join(", ", listNotValues.Select(o => o.TEST_INDEX_NAME).ToList())));
                    }

                    bool isPrint = checkPrintNow.Checked || chkSign.Checked || chkPrintPreview.Checked || chkSignProcess.Checked;

                    if (success)
                    {
                        gridControlSample.RefreshDataSource();
                        if (!isPrint)
                        {
                            if (IsLoadGridSampleAfterSave)
                                FillDataToGridControl();
                            return;
                        }
                        listCheckPrint = GetListCheckPrint();
                        this.PrintOption = PRINT_OPTION.IN;
                        if (!HisConfigCFG.PRINT_TEST_RESULT)
                        {
                            SetDataToPrint(false);
                            PrintProcess(PrintTypeKXN.IN_KET_QUA_XET_NGHIEM);
                        }
                        else
                        {
                            SetDataToPrint(true);
                            if (lstResultPrint != null && lstResultPrint.Count > 0)
                            {
                                PrintProcess(PrintTypeKXN.IN_KET_QUA_XET_NGHIEM);
                            }
                            if (lstResultHH != null && lstResultHH.Count > 0)
                            {
                                PrintProcess(PrintTypeKXN.IN_HUYET_HOC);
                            }
                            if (lstResultVS != null && lstResultVS.Count > 0)
                            {
                                PrintProcess(PrintTypeKXN.IN_VI_SINH);
                            }
                            if (lstResultMD != null && lstResultMD.Count > 0)
                            {
                                PrintProcess(PrintTypeKXN.IN_MIEN_DICH);
                            }
                            if (lstResultSH != null && lstResultSH.Count > 0)
                            {
                                PrintProcess(PrintTypeKXN.IN_SINH_HOA);
                            }
                            if (lstResultXNT != null && lstResultXNT.Count > 0)
                            {
                                PrintProcess(PrintTypeKXN.IN_XET_NGHIEM_TEST);
                            }
                            if (lstResultXNGPB != null && lstResultXNGPB.Count > 0)
                            {
                                PrintProcess(PrintTypeKXN.IN_XET_NGHIEM_GIAI_PHAU_BENH);
                            }
                            if (lstResultXNNT != null && lstResultXNNT.Count > 0)
                            {
                                PrintProcess(PrintTypeKXN.IN_XET_NGHIEM_NUOC_TIEU);
                            }
                        }
                    }
                    if (IsLoadGridSampleAfterSave)
                        FillDataToGridControl();
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa nhập giá trị cho chỉ số hoặc không thay đổi", "Thông báo");
                    return;
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ConfirmUser(List<TestLisResultADO> listNotValues, ref bool showMessage, ref bool hasConfirmUser, ref bool resultConfirmUser)
        {
            bool result = false;
            try
            {
                if (hasConfirmUser)
                {
                    return resultConfirmUser;
                }
                WaitingManager.Hide();
                if (LisConfigCFG.ALLOW_SAVE_WHEN_EMPTY == "2")
                {
                    showMessage = false;
                    result = XtraMessageBox.Show(String.Format(Resources.ResourceMessage.ChiSoChuaNhapGiaTriBanCoMuonTiepTuc, String.Join(", ", listNotValues.Select(o => o.TEST_INDEX_NAME).ToList())), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo, DefaultBoolean.True) == DialogResult.Yes;
                    hasConfirmUser = true;
                    resultConfirmUser = result;
                }
                WaitingManager.Show();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        private bool CheckOption3(List<TestLisResultADO> listNotValues)
        {
            bool result = false;
            try
            {
                WaitingManager.Show();
                if (listNotValues != null && listNotValues.Count > 0)
                {
                    List<string> listNotValueResult = new List<string>();
                    var listTestIndexMap = BackendDataWorker.Get<V_LIS_TEST_INDEX_MAP>();
                    foreach (var item in listNotValues)
                    {
                        if (item.MACHINE_ID.HasValue)
                        {
                            if (!listTestIndexMap.Exists(o => o.TEST_INDEX_CODE == item.TEST_INDEX_CODE))
                                listNotValueResult.Add(item.TEST_INDEX_NAME);
                            else
                            {
                                var test = listTestIndexMap.FirstOrDefault(o => o.TEST_INDEX_CODE == item.TEST_INDEX_CODE && o.MACHINE_ID == item.MACHINE_ID);
                                if (test != null)
                                    listNotValueResult.Add(item.TEST_INDEX_NAME);
                            }

                        }
                        else
                        {
                            listNotValueResult.Add(item.TEST_INDEX_NAME);
                        }
                    }
                    if (listNotValueResult != null && listNotValueResult.Count > 0)
                    {
                        WaitingManager.Hide();
                        result = false;
                        XtraMessageBox.Show(String.Format(Resources.ResourceMessage.ChiSoChuaNhapGiaTri, String.Join(", ", listNotValueResult)), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    }
                    else
                        result = true;

                }
                else
                    result = true;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private bool CheckMachine(List<TestLisResultADO> parrentData)
        {
            try
            {
                if (!chkWarn.Checked && !chkCon.Checked)
                    return true;

                List<TestLisResultADO> notMachines = parrentData != null ? parrentData.Where(o => !o.MACHINE_ID.HasValue).ToList() : null;
                if (chkWarn.Checked && notMachines != null && notMachines.Count > 0)
                {
                    string names = String.Join(", ", notMachines.Select(s => s.SERVICE_NAME).ToList());
                    string notify = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao);
                    if (XtraMessageBox.Show(String.Format(Resources.ResourceMessage.DichVuChuaChonMayTraKetQuaBanCoMuonTiepTuc, names), notify, MessageBoxButtons.YesNo, DefaultBoolean.True) != DialogResult.Yes)
                    {
                        return false;
                    }
                }
                else if (chkCon.Checked && notMachines != null && notMachines.Count > 0)
                {
                    string names = String.Join(", ", notMachines.Select(s => s.SERVICE_NAME).ToList());
                    string notify = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao);
                    XtraMessageBox.Show(String.Format(Resources.ResourceMessage.DichVuChuaChonMayTraKetQua, names), notify, DefaultBoolean.True);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private void UpdateDataForSavePrintSuccess()
        {
            try
            {
                LisSampleFilter samFilter = new LisSampleFilter();
                samFilter.ID = rowSample.ID;
                var datas = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<LIS_SAMPLE>>("api/LisSample/Get", ApiConsumers.LisConsumer, samFilter, null);
                if (datas == null || datas.Count != 1)
                {
                    FillDataToGridControl();
                    IsLoadGridSampleAfterSave = false;
                    throw new Exception("Khong lay duoc LIS_SAMPLE theo id: " + samFilter.ID);
                }
                rowSample.SAMPLE_STT_ID = datas[0].SAMPLE_STT_ID;
                gridControlSample.RefreshDataSource();
                LoadLisResult(rowSample);
                if (rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                {
                    btnSave.Enabled = false;
                }
                else
                {
                    btnSave.Enabled = true;
                }
                //btnPrint.Enabled = false;
                //btnInTachTheoNhom.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateDataForSaveSuccess()
        {
            try
            {
                dxValidationProviderEditorInfo.RemoveControlError(DateKQ);
                dxValidationProviderEditorInfo.RemoveControlError(DateLM);
                LisSampleFilter samFilter = new LisSampleFilter();
                samFilter.ID = rowSample.ID;
                var datas = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<LIS_SAMPLE>>("api/LisSample/Get", ApiConsumers.LisConsumer, samFilter, null);
                if (datas == null || datas.Count != 1)
                {
                    FillDataToGridControl();
                    IsLoadGridSampleAfterSave = false;
                    throw new Exception("Khong lay duoc LIS_SAMPLE theo id: " + samFilter.ID);
                }
                rowSample.SAMPLE_STT_ID = datas[0].SAMPLE_STT_ID;
                gridControlSample.RefreshDataSource();
                LoadLisResult(rowSample);
                LoadDataToGridTestResult2();
                if (rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                {
                    btnSave.Enabled = false;
                }
                else
                {
                    btnSave.Enabled = true;
                }
                //btnPrint.Enabled = false;
                //btnInTachTheoNhom.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessSaveAndReturnValue(ref bool print, ref bool hasConfirmUser, ref bool resultConfirmUser)
        {
            try
            {
                rowSample = (LisSampleADO)gridViewSample.GetFocusedRow();
                long? sample_time = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateLM.DateTime);
                long? result_time = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateKQ.DateTime);
                if (rowSample.APPROVAL_TIME != null && sample_time > rowSample.APPROVAL_TIME)
                {
                    XtraMessageBox.Show(String.Format("Thời gian lấy mẫu {0} phải nhỏ hơn thời gian duyệt mẫu {1}", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(sample_time ?? 0), Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rowSample.APPROVAL_TIME ?? 0)), "Thông báo");
                    return;
                }
                if (rowSample.APPROVAL_TIME != null && result_time < rowSample.APPROVAL_TIME)
                {
                    XtraMessageBox.Show(String.Format("Thời gian duyệt mẫu {0} phải nhỏ hơn thời gian trả kết quả {1}", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rowSample.APPROVAL_TIME ?? 0), Inventec.Common.DateTime.Convert.TimeNumberToTimeString(result_time ?? 0)), "Thông báo");
                    return;
                }
                if ((HisConfigCFG.AUTO_RETURN_RESULT_BEFORE_PRINT == "1" || chkReturnResult.CheckState == CheckState.Checked)
                    && ((LisConfigCFG.MUST_APPROVE_RESULT != "1" && rowSample.SAMPLE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                    || (LisConfigCFG.MUST_APPROVE_RESULT == "1" && rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ)))
                {
                    CommonParam param = new CommonParam();
                    param.Messages = new List<string>();
                    bool success = false;
                    bool save = SaveValue(ref param, ref print, ref hasConfirmUser, ref resultConfirmUser);
                    if (save)
                    {
                        success = ProcessReturnResult(ref param);
                        if (!success)
                        {
                            param.Messages.Add("Tự động trả kết quả thất bại");
                        }
                    }
                    WaitingManager.Hide();
                    if (IsCallApi)
                        MessageManager.Show(this.ParentForm, param, success);
                }
                else
                {
                    CommonParam param = new CommonParam();
                    param.Messages = new List<string>();
                    bool success = SaveValue(ref param, ref print, ref hasConfirmUser, ref resultConfirmUser);
                    if (IsCallApi)
                        MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;
                bool hasConfirmUser = false;
                bool resultConfirmUser = false;
                ProcessPrint(ref hasConfirmUser, ref resultConfirmUser);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private List<TestLisResultADO> GetListCheckPrint() //lấy ra các dữ liệu được check để in, dùng để lưu lại dữ liệu được check trước khi gọi hàm load lại grid để tránh trường hợp in nhầm dữ liệu
        {
            List<TestLisResultADO> lst = new List<TestLisResultADO>();
            try
            {
                if (treeListSereServTein.AllNodesCount > 0)
                {
                    var TreeListCheck = treeListSereServTein.GetAllCheckedNodes();
                    foreach (var item in TreeListCheck)
                    {
                        lst.Add((TestLisResultADO)this.treeListSereServTein.GetDataRecordByNode(item));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return lst;
        }
        private void ProcessPrint(ref bool hasConfirmUser, ref bool resultConfirmUser)
        {
            try
            {
                bool print = false;
                listCheckPrint = GetListCheckPrint();
                ProcessSaveAndReturnValue(ref print, ref hasConfirmUser, ref resultConfirmUser);
                this.PrintOption = PRINT_OPTION.IN;
                if (print)
                {
                    if (!HisConfigCFG.PRINT_TEST_RESULT)
                    {
                        SetDataToPrint(false);
                        PrintProcess(PrintTypeKXN.IN_KET_QUA_XET_NGHIEM);
                    }
                    else
                    {
                        SetDataToPrint(true);
                        if (lstResultPrint != null && lstResultPrint.Count > 0)
                        {
                            PrintProcess(PrintTypeKXN.IN_KET_QUA_XET_NGHIEM);
                        }
                        if (lstResultHH != null && lstResultHH.Count > 0)
                        {
                            PrintProcess(PrintTypeKXN.IN_HUYET_HOC);
                        }
                        if (lstResultVS != null && lstResultVS.Count > 0)
                        {
                            PrintProcess(PrintTypeKXN.IN_VI_SINH);
                        }
                        if (lstResultMD != null && lstResultMD.Count > 0)
                        {
                            PrintProcess(PrintTypeKXN.IN_MIEN_DICH);
                        }
                        if (lstResultSH != null && lstResultSH.Count > 0)
                        {
                            PrintProcess(PrintTypeKXN.IN_SINH_HOA);
                        }
                        if (lstResultXNT != null && lstResultXNT.Count > 0)
                        {
                            PrintProcess(PrintTypeKXN.IN_XET_NGHIEM_TEST);
                        }
                        if (lstResultXNGPB != null && lstResultXNGPB.Count > 0)
                        {
                            PrintProcess(PrintTypeKXN.IN_XET_NGHIEM_GIAI_PHAU_BENH);
                        }
                        if (lstResultXNNT != null && lstResultXNNT.Count > 0)
                        {
                            PrintProcess(PrintTypeKXN.IN_XET_NGHIEM_NUOC_TIEU);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ProcessReturnResult(ref CommonParam param)
        {
            bool result = false;
            try
            {
                if (this.lstLisResultADOs == null || this.lstLisResultADOs.Count() == 0)
                {
                    param.Messages.Add("không có chỉ số để trả kết quả");
                    return result;
                }

                if (lstCheckPrint == null || lstCheckPrint.Count() == 0)
                {
                    param.Messages.Add("không có chỉ số để trả kết quả");
                    return result;
                }

                var lstLisResultProcessADOs = this.lstLisResultADOs.Where(o => o.IS_PARENT == 0 || (o.IS_PARENT == 1 && o.HAS_ONE_CHILD == 1)).ToList();

                if (lstLisResultProcessADOs == null || lstLisResultProcessADOs.Count() == 0)
                    return result;

                // bỏ những chỉ số không thực hiện
                lstLisResultProcessADOs = lstLisResultProcessADOs.Where(o => o.IS_NO_EXECUTE != 1).ToList();

                if (lstLisResultProcessADOs == null || lstLisResultProcessADOs.Count() == 0)
                {
                    return result;
                }

                // kiểm tra chỉ số có giá trị
                var checkTiNullValues = lstLisResultProcessADOs.Where(o => String.IsNullOrWhiteSpace(o.VALUE_RANGE));

                if (checkTiNullValues != null && checkTiNullValues.Count() > 0 && LisConfigCFG.IS_ALLOW_SAVE_WHEN_NOT_FULL_RESULT == "2")
                {
                    if (MessageBox.Show(String.Format("Chỉ số ({0}) chưa có kết quả, bạn có muốn trả kết quả không?", String.Join("; ", checkTiNullValues.Select(o => o.TEST_INDEX_NAME))), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        result = SaveResult(ref param);
                    }
                    else
                    {
                        result = true;
                        return result;
                    }
                }
                else if (checkTiNullValues != null && checkTiNullValues.Count() > 0 && LisConfigCFG.IS_ALLOW_SAVE_WHEN_NOT_FULL_RESULT == "0")
                {
                    param.Messages.Add(String.Format("Chỉ số ({0}) chưa có kết quả để trả kết quả", String.Join("; ", checkTiNullValues.Select(o => o.TEST_INDEX_NAME))));
                    return result;
                }
                else if (LisConfigCFG.IS_ALLOW_SAVE_WHEN_NOT_FULL_RESULT == "3" && !this.CheckOption3(checkTiNullValues.ToList()))
                {
                    return result;
                }
                else
                {
                    result = SaveResult(ref param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        bool SaveResult(ref CommonParam param)
        {
            bool success = false;
            try
            {
                rowSample = (LisSampleADO)gridViewSample.GetFocusedRow();

                if (rowSample != null)
                {
                    long? resultTime = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(DateKQ.EditValue).ToString("yyyyMMddHHmmss"));
                    if (rowSample.RESULT_APPROVAL_TIME != null && rowSample.RESULT_APPROVAL_TIME > resultTime)
                    {
                        XtraMessageBox.Show(String.Format("Thời gian duyệt kết quả {0} phải nhỏ hơn thời gian trả kết quả {1}", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rowSample.RESULT_APPROVAL_TIME ?? 0), Inventec.Common.DateTime.Convert.TimeNumberToTimeString(resultTime ?? 0)), "Thông báo");
                        return false;
                    }
                    LisSampleReturnResultSDO lisSampleReturnResultSDO = new LIS.SDO.LisSampleReturnResultSDO();
                    lisSampleReturnResultSDO.SampleId = rowSample.ID;
                    lisSampleReturnResultSDO.ResultUsername = cboUserKQ.Text.ToString();
                    lisSampleReturnResultSDO.ResultLoginname = cboUserKQ.EditValue.ToString();
                    lisSampleReturnResultSDO.ResultTime = resultTime;
                    var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("/api/LisSample/ReturnResult", ApiConsumer.ApiConsumers.LisConsumer, lisSampleReturnResultSDO, param);
                    if (curentSTT != null)
                    {
                        rowSample.SAMPLE_STT_ID = IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ;
                        LisSampleADO sample = new LisSampleADO(rowSample);
                        FillDataToGridControl();
                        rowSample = new LisSampleADO(sample);
                        IsLoadGridSampleAfterSave = false;
                        success = true;
                        LoadLisResult(rowSample);
                        string message = string.Format("Trả kết quả toàn phần thành công. SERVICE_REQ_CODE: {0}. BARCODE: {1}", rowSample.SERVICE_REQ_CODE, rowSample.BARCODE);
                        string login = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        SdaEventLogCreate eventlog = new SdaEventLogCreate();
                        eventlog.Create(login, null, true, message);

                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        public void SAVE()
        {
            try
            {
                if (btnSave.Enabled)
                {
                    btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void PRINT()
        {
            try
            {
                if (btnPrint.Enabled)
                {
                    btnPrint_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SEARCH()
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FocusF1()
        {
            try
            {
                txtSERVICE_REQ_CODE__EXACT.Focus();
                txtSERVICE_REQ_CODE__EXACT.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FocusF2()
        {
            try
            {
                txtPatientCode.Focus();
                txtPatientCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnUpdateNumOrder_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (LIS_SAMPLE)gridViewSample.GetFocusedRow();
                if (data != null)
                {
                    WaitingManager.Show();
                    bool success = false;
                    CommonParam param = new CommonParam();
                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/UpdateNumOrder", ApiConsumers.LisConsumer, data, param);
                    if (rs != null)
                    {
                        success = true;
                        CommonParam paramGet = new CommonParam(startPage, limit);
                        FillDataToGridSample(paramGet);
                    }
                    WaitingManager.Hide();
                    if (success)
                    {
                        MessageManager.ShowAlert(this.ParentForm, param, success);
                    }
                    else
                    {
                        MessageManager.Show(param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearchKey_Click(object sender, EventArgs e)
        {
            try
            {
                txtSearchKey.Focus();
                txtSearchKey.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCboMachine()
        {
            try
            {
                LIS.Filter.LisMachineFilter filter = new LisMachineFilter();
                this._Machines = new List<LIS_MACHINE>();
                this._Machines = new BackendAdapter(new CommonParam()).Get<List<LIS_MACHINE>>("api/LisMachine/Get", ApiConsumers.LisConsumer, filter, null);
                List<LisMachineAdo> machineSources = new List<LisMachineAdo>();
                if (_Machines != null && _Machines.Count() > 0)
                {
                    machineSources = (from n in _Machines select new LisMachineAdo(n, GlobalVariables.MachineCounterSdos != null ? GlobalVariables.MachineCounterSdos.FirstOrDefault(o => o.MACHINE_CODE == n.MACHINE_CODE) : null)).ToList();
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MACHINE_CODE", "Mã máy", 150, 1));
                columnInfos.Add(new ColumnInfo("MACHINE_NAME", "Tên máy", 250, 2));
                columnInfos.Add(new ColumnInfo("TOTAL_PROCESSED_SERVICE_TEIN", "Đã xử lý", 100, 3));
                columnInfos.Add(new ColumnInfo("MAX_SERVICE_PER_DAY", "Tối đa", 100, 4));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MACHINE_NAME", "ID", columnInfos, true, 450);
                ControlEditorLoader.Load(this.GridLookUpEdit__Machine, machineSources, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        List<LisMachineAdo> machineSources = new List<LisMachineAdo>();
        private void FillDataMachineCombo(TestLisResultADO data, DevExpress.XtraEditors.GridLookUpEdit cbo)
        {
            try
            {
                List<LIS_MACHINE> dataMachines = new List<LIS_MACHINE>();
                if (this._Machines != null && this._Machines.Count > 0 && this.rowSample != null)
                {
                    dataMachines = this._Machines.Where(o => o.EXECUTE_ROOM_CODE == null || o.EXECUTE_ROOM_CODE == this.rowSample.EXECUTE_ROOM_CODE).ToList();
                }
                if (dataMachines != null && dataMachines.Count() > 0)
                {
                    machineSources = (from n in dataMachines select new LisMachineAdo(n, GlobalVariables.MachineCounterSdos != null ? GlobalVariables.MachineCounterSdos.FirstOrDefault(o => o.MACHINE_CODE == n.MACHINE_CODE) : null)).ToList();
                    machineSources = machineSources.Where(o => o.IS_ACTIVE == IMSys.DbConfig.LIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MACHINE_CODE", "Mã máy", 150, 1));
                columnInfos.Add(new ColumnInfo("MACHINE_NAME", "Tên máy", 250, 2));
                columnInfos.Add(new ColumnInfo("TOTAL_PROCESSED_SERVICE_TEIN", "Đã xử lý", 100, 3));
                columnInfos.Add(new ColumnInfo("MAX_SERVICE_PER_DAY", "Tối đa", 100, 4));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MACHINE_NAME", "ID", columnInfos, true, 450);
                ControlEditorLoader.Load(cbo, machineSources, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeList1_CustomDrawNodeCell(object sender, DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e)
        {
            try
            {
                TestLisResultADO testLisResultADO = new TestLisResultADO();
                var data = this.treeListSereServTein.GetDataRecordByNode(e.Node);
                if (data != null && data is TestLisResultADO)
                {
                    testLisResultADO = (TestLisResultADO)data;
                    if (testLisResultADO.IS_PARENT == 1 && testLisResultADO.IS_NO_EXECUTE == 1)
                    {
                        e.Appearance.FontStyleDelta = FontStyle.Strikeout;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeList1_CustomNodeCellEdit(object sender, DevExpress.XtraTreeList.GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                TestLisResultADO testLisResultADO = new TestLisResultADO();
                var data = this.treeListSereServTein.GetDataRecordByNode(e.Node);
                if (data != null && data is TestLisResultADO)
                {
                    testLisResultADO = (TestLisResultADO)data;
                }
                if (e.Column.FieldName == "MACHINE_ID")
                {
                    if (((TestLisResultADO)data).IS_PARENT == 1)
                    {
                        e.RepositoryItem = GridLookUpEdit__Machine;
                    }
                    else
                    {
                        e.RepositoryItem = GridLookUpEdit__Machine_Disable;
                    }
                }
                else if (e.Column.FieldName == "VALUE_RANGE_DISPLAY" && data != null)
                {
                    if (HisConfigCFG.IS_SHOWING_RESULT_GENERAL == "1")
                    {
                        if (testLisResultADO.IS_NO_EXECUTE == 1
                            || testLisResultADO.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ
                             || testLisResultADO.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ)
                        {
                            if (testLisResultADO.IS_PARENT == 1 && testLisResultADO.HAS_ONE_CHILD == 0)
                            {
                                e.RepositoryItem = repositoryItemGridLookUp_ServiceResult_Disable;
                            }
                            else
                            {
                                e.RepositoryItem = TextEditValueRange__Disable;
                            }
                        }
                        else if ((testLisResultADO.IS_PARENT == 1 && testLisResultADO.HAS_ONE_CHILD == 0))
                        {
                            e.RepositoryItem = repositoryItemGridLookUp_ServiceResult;
                        }
                        else
                        {
                            e.RepositoryItem = TextEditValueRange__Enable;
                        }
                    }
                    else
                    {
                        if ((testLisResultADO.IS_PARENT == 1 && testLisResultADO.HAS_ONE_CHILD == 0) || testLisResultADO.IS_NO_EXECUTE == 1
                        || testLisResultADO.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ
                         || testLisResultADO.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ)
                        {
                            e.RepositoryItem = TextEditValueRange__Disable;
                            e.Column.OptionsColumn.AllowEdit = false;
                        }
                        else
                        {
                            e.RepositoryItem = TextEditValueRange__Enable;
                            e.Column.OptionsColumn.AllowEdit = true;
                        }
                    }

                }
                else if (e.Column.FieldName == "ButtonForVL" && data != null)
                {
                    if ((testLisResultADO.IS_PARENT == 1 && testLisResultADO.HAS_ONE_CHILD == 0) || testLisResultADO.IS_NO_EXECUTE == 1
                        || testLisResultADO.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ
                         || testLisResultADO.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ)
                    {
                        e.RepositoryItem = TextEditValueRange__Disable;
                    }
                    else
                    {
                        e.RepositoryItem = repositoryItembtnValueRangeShow;
                        e.Column.OptionsColumn.AllowEdit = true;
                    }
                }
                else if (e.Column.FieldName == "OLD_VALUE" && data != null)
                {
                    if ((testLisResultADO.IS_PARENT == 1 && testLisResultADO.HAS_ONE_CHILD == 0) || testLisResultADO.IS_NO_EXECUTE == 1)
                    {
                        e.RepositoryItem = TextEditValueRange__Disable;
                        e.Column.OptionsColumn.AllowEdit = false;
                    }
                    else
                    {
                        e.RepositoryItem = repositoryItemText__OldValue;
                        e.Column.OptionsColumn.AllowEdit = true;
                    }
                }
                else if (e.Column.FieldName == "NOTE" && data != null)
                {
                    if (testLisResultADO.IS_PARENT == 1 && testLisResultADO.HAS_ONE_CHILD == 0)
                    {
                        e.RepositoryItem = TextEditNote__Disable;
                        e.Column.OptionsColumn.AllowEdit = false;
                    }
                    else
                    {
                        e.RepositoryItem = TextEditNote__Enable;
                        e.Column.OptionsColumn.AllowEdit = true;
                    }
                }
                else if (e.Column.FieldName == "ColumnButtonForNote" && data != null)
                {
                    if ((testLisResultADO.IS_PARENT == 1 && testLisResultADO.HAS_ONE_CHILD == 0) || testLisResultADO.IS_NO_EXECUTE == 1
                        || testLisResultADO.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ
                         || testLisResultADO.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ)
                    {
                        e.RepositoryItem = TextEditNote__Disable;
                    }
                    else
                    {
                        e.RepositoryItem = repositoryItembtnNoteShow;
                        e.Column.OptionsColumn.AllowEdit = true;
                    }
                }
                else if (e.Column.FieldName == "ColumnForOldValue" && data != null)
                {
                    if ((testLisResultADO.IS_PARENT == 1 && testLisResultADO.HAS_ONE_CHILD == 0) || testLisResultADO.IS_NO_EXECUTE == 1
                        || testLisResultADO.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ
                         || testLisResultADO.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ)
                    {
                        e.RepositoryItem = TextEditValueRange__Disable;
                    }
                    else
                    {
                        e.RepositoryItem = repositoryItembtnOldValueShow;
                        e.Column.OptionsColumn.AllowEdit = true;
                    }
                }
                else if (e.Column.FieldName == "IMAGE")
                {
                    if (testLisResultADO.IS_PARENT != 1 && testLisResultADO.IS_RUNNING == 1)
                    {
                        e.RepositoryItem = ButtonEdit_DangChayXN;
                    }
                    else if (testLisResultADO.IS_PARENT != 1 && testLisResultADO.SAMPLE_SERVICE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_SERVICE_STT.ID__CHUA_CO_KQ)
                    {
                        e.RepositoryItem = ButtonEdit_ChuaCoKQSTT;
                    }
                    else if (testLisResultADO.IS_PARENT != 1 && testLisResultADO.SAMPLE_SERVICE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_SERVICE_STT.ID__DA_CO_KQ)
                    {
                        e.RepositoryItem = ButtonEdit_DaCoKQSTT;
                    }
                    else if (testLisResultADO.IS_PARENT != 1 && testLisResultADO.SAMPLE_SERVICE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_SERVICE_STT.ID__DA_TRA_KQ)
                    {
                        e.RepositoryItem = ButtonEdit_DaTraKQSTT;
                    }
                    else if (testLisResultADO.IS_PARENT == 1 && testLisResultADO.SAMPLE_SERVICE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_SERVICE_STT.ID__DA_TRA_KQ)
                    {
                        e.RepositoryItem = ButtonEdit_TraKetQua;
                    }
                }
                else if (e.Column.FieldName == "RERUN")
                {
                    if (testLisResultADO.IS_PARENT != 1)
                    {
                        e.RepositoryItem = TextEditValueRange__Disable;
                    }
                    else if (testLisResultADO.IS_PARENT == 1 &&
                        (testLisResultADO.IS_RUNNING == 1
                        || testLisResultADO.SAMPLE_SERVICE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_SERVICE_STT.ID__DA_CO_KQ))
                    {
                        e.RepositoryItem = repositoryItemCheckRerun_Disable;
                    }
                    else
                    {
                        e.RepositoryItem = repositoryItemCheckRerun_Enable;
                    }
                }
                else if (e.Column.FieldName == "ColumnButtonForResultDescription" && data != null)
                {
                    if ((testLisResultADO.IS_PARENT == 1 && testLisResultADO.HAS_ONE_CHILD == 0) || testLisResultADO.IS_NO_EXECUTE == 1
                        || testLisResultADO.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ
                         || testLisResultADO.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ)
                    {
                        e.RepositoryItem = TextEditResultRange__Disable;
                    }
                    else
                    {
                        e.RepositoryItem = repositoryItembtnResultDescription;
                        e.Column.OptionsColumn.AllowEdit = true;
                    }
                }
                else if (e.Column.FieldName == "RESULT_DESCRIPTION" && data != null)
                {
                    if (testLisResultADO.IS_PARENT == 1 && testLisResultADO.HAS_ONE_CHILD == 0)
                    {
                        e.RepositoryItem = TextEditResultRange__Disable;
                        e.Column.OptionsColumn.AllowEdit = false;
                    }
                    else
                    {
                        e.RepositoryItem = TextEditResultRange__Enable;
                        e.Column.OptionsColumn.AllowEdit = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeList1_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "VALUE_RANGE_DISPLAY")
                {
                    LogSystem.Debug("VALUE_RANGE_DISPLAY");
                    var data = this.treeListSereServTein.GetDataRecordByNode(e.Node);
                    if (data != null && data is TestLisResultADO && ((TestLisResultADO)data).LIS_RESULT_ID > 0 && !string.IsNullOrEmpty(((TestLisResultADO)data).VALUE_RANGE))
                    {
                        ((TestLisResultADO)data).Item_Edit_Value = 1;
                    }


                }
                else if (e.Column.FieldName == "NOTE")
                {
                    var data = this.treeListSereServTein.GetDataRecordByNode(e.Node);
                    if (data != null && data is TestLisResultADO && ((TestLisResultADO)data).LIS_RESULT_ID > 0)
                    {
                        ((TestLisResultADO)data).Item_Edit_Value = 1;
                    }
                }
                else if (e.Column.FieldName == "RERUN")
                {
                    var data = (TestLisResultADO)this.treeListSereServTein.GetDataRecordByNode(e.Node);
                    if (data != null && data.IS_PARENT == 1)
                    {
                        WaitingManager.Show();
                        bool success = false;
                        CommonParam param = new CommonParam();
                        LisSampleServiceSDO sdo = new LisSampleServiceSDO();
                        sdo.Id = data.SAMPLE_SERVICE_ID ?? 0;
                        sdo.IsRunAgain = data.RERUN.HasValue ? data.RERUN.Value : false;
                        var rs = new BackendAdapter(param).Post<LIS_SAMPLE_SERVICE>("api/LisSampleService/UpdateRetest", ApiConsumers.LisConsumer, sdo, param);
                        if (rs != null)
                        {
                            data.IS_RUNNING = rs.IS_RUNNING;
                            data.IS_RUN_AGAIN = rs.IS_RUN_AGAIN;
                            data.RERUN = rs.IS_RUN_AGAIN == 1;
                            success = true;
                        }
                        else
                        {
                            data.RERUN = data.IS_RUN_AGAIN == 1;
                        }
                        treeListSereServTein.RefreshDataSource();
                        WaitingManager.Hide();
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessCheckNormal(ref TestLisResultADO hisSereServTeinSDO)
        {
            try
            {
                V_HIS_TEST_INDEX_RANGE testIndexRange = new V_HIS_TEST_INDEX_RANGE();
                testIndexRange = GetTestIndexRange(this.rowSample.DOB ?? 0, this.genderId, hisSereServTeinSDO.TEST_INDEX_CODE, ref this.testIndexRangeAll);
                if (testIndexRange != null)
                {
                    AssignNormal(ref hisSereServTeinSDO, testIndexRange);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AssignNormal(ref TestLisResultADO ti, V_HIS_TEST_INDEX_RANGE testIndexRange)
        {
            try
            {
                decimal value = 0;
                if (ti != null && testIndexRange != null)
                {
                    ti.IS_NORMAL = null;
                    ti.IS_HIGHER = null;
                    ti.IS_LOWER = null;
                    if (!String.IsNullOrEmpty(testIndexRange.NORMAL_VALUE))
                    {
                        if (!String.IsNullOrWhiteSpace(ti.DESCRIPTION) && !String.IsNullOrWhiteSpace(ti.VALUE_RANGE) && ti.VALUE_RANGE.ToUpper() == ti.DESCRIPTION.ToUpper())
                        {
                            ti.IS_NORMAL = true;
                        }
                        else if (!String.IsNullOrWhiteSpace(ti.DESCRIPTION) && !String.IsNullOrWhiteSpace(ti.VALUE_RANGE) && ti.VALUE_RANGE.ToUpper() != ti.DESCRIPTION.ToUpper())
                        {
                            ti.IS_LOWER = true;
                            ti.IS_HIGHER = true;
                        }
                    }
                    else
                    {
                        if (!Decimal.TryParse((ti.VALUE_RANGE ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out value))
                            return;

                        if (testIndexRange.IS_ACCEPT_EQUAL_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_MAX == null)
                        {
                            if (!String.IsNullOrWhiteSpace(ti.VALUE_RANGE) && ti.MIN_VALUE != null && ti.MIN_VALUE <= value && ti.MAX_VALUE != null && value < ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (!String.IsNullOrWhiteSpace(ti.VALUE_RANGE) && ti.MIN_VALUE != null && value < ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (!String.IsNullOrWhiteSpace(ti.VALUE_RANGE) && ti.MAX_VALUE != null && ti.MAX_VALUE <= value)
                            {
                                ti.IS_HIGHER = true;
                            }
                        }
                        else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_MAX == 1)
                        {
                            if (!String.IsNullOrWhiteSpace(ti.VALUE_RANGE) && ti.MIN_VALUE != null && ti.MIN_VALUE <= value && ti.MAX_VALUE != null && value <= ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (!String.IsNullOrWhiteSpace(ti.VALUE_RANGE) && ti.MIN_VALUE != null && Decimal.Parse((ti.VALUE_RANGE.Trim()).Replace('.', ','), style, LanguageManager.GetCulture()) < ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (!String.IsNullOrWhiteSpace(ti.VALUE_RANGE) && ti.MAX_VALUE != null && ti.MAX_VALUE < Decimal.Parse((ti.VALUE_RANGE.Trim()).Replace('.', ','), style, LanguageManager.GetCulture()))
                            {
                                ti.IS_HIGHER = true;
                            }
                        }
                        else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_MAX == 1)
                        {
                            if (!String.IsNullOrWhiteSpace(ti.VALUE_RANGE) && ti.MIN_VALUE != null && ti.MIN_VALUE < value && ti.MAX_VALUE != null && value <= ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (!String.IsNullOrWhiteSpace(ti.VALUE_RANGE) && ti.MIN_VALUE != null && value < ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (!String.IsNullOrWhiteSpace(ti.VALUE_RANGE) && ti.MAX_VALUE != null && ti.MAX_VALUE < value)
                            {
                                ti.IS_HIGHER = true;
                            }
                        }
                        else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_MAX == null)
                        {
                            if (!String.IsNullOrWhiteSpace(ti.VALUE_RANGE) && ti.MIN_VALUE != null && ti.MIN_VALUE < value && ti.MAX_VALUE != null && value < ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (!String.IsNullOrWhiteSpace(ti.VALUE_RANGE) && ti.MIN_VALUE != null && value <= ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (!String.IsNullOrWhiteSpace(ti.VALUE_RANGE) && ti.MAX_VALUE != null && ti.MAX_VALUE <= value)
                            {
                                ti.IS_HIGHER = true;
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

        private void treeList1_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = (TestLisResultADO)this.treeListSereServTein.GetDataRecordByNode(e.Node);
                if (data != null)
                {
                    if (e.Column.FieldName == "VALUE_RANGE")
                    {
                        ProcessCheckNormal(ref data);
                    }

                    if (data.IS_PARENT == 1 || data.HAS_ONE_CHILD == 1)
                    {
                        e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
                    }

                    if (data.IS_LOWER == true && data.IS_HIGHER == true)
                    {
                        e.Appearance.ForeColor = Color.Green;
                    }
                    else if (data.IS_LOWER == true)
                    {
                        e.Appearance.ForeColor = Color.Blue;
                    }
                    else if (data.IS_HIGHER == true)
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }
                    else
                    {
                        e.Appearance.ForeColor = Color.Black;
                    }

                    if (e.Column.FieldName == "TEST_INDEX_NAME" && data.IS_IMPORTANT > 0)
                    {
                        e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
                        e.Appearance.ForeColor = Color.Black;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeList1_AfterCheckNode(object sender, NodeEventArgs e)
        {
            try
            {
                treeListSereServTein.FocusedNode = e.Node;
                lstCheckPrint = new List<TestLisResultADO>();
                if (treeListSereServTein.AllNodesCount > 0)
                {
                    var TreeListCheck = treeListSereServTein.GetAllCheckedNodes();
                    foreach (var item in TreeListCheck)
                    {
                        lstCheckPrint.Add((TestLisResultADO)this.treeListSereServTein.GetDataRecordByNode(item));
                    }
                }
                if (rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                {
                    btnSave.Enabled = false;
                }
                else
                {
                    btnSave.Enabled = true;
                }
                if (lstCheckPrint.Count > 0)
                {
                    btnPrint.Enabled = true;
                    btnInTachTheoNhom.Enabled = true;
                    btnKhongThucHien.Enabled = true;
                }
                else
                {
                    btnKhongThucHien.Enabled = false;
                    btnPrint.Enabled = false;
                    btnInTachTheoNhom.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeList1_BeforeCheckNode(object sender, CheckNodeEventArgs e)
        {
            try
            {
                treeListSereServTein.FocusedNode = e.Node;
                e.State = (e.PrevState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonEdit_TraKetQua_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var dataFocus = this.treeListSereServTein.GetDataRecordByNode(this.treeListSereServTein.FocusedNode);
                if (dataFocus != null)
                {

                    TestLisResultADO testLisResultADO = (TestLisResultADO)dataFocus;
                    List<TestLisResultADO> dataChildOfFocus = new List<TestLisResultADO>();
                    var AllCheckNodes = treeListSereServTein.GetAllCheckedNodes();

                    foreach (var item in AllCheckNodes)
                    {
                        var data = (TestLisResultADO)this.treeListSereServTein.GetDataRecordByNode(item);
                        if (data.PARENT_ID == testLisResultADO.CHILD_ID || (data.HAS_ONE_CHILD == 1 && data.CHILD_ID == testLisResultADO.CHILD_ID))
                        {
                            dataChildOfFocus.Add(data);
                        }
                    }

                    if (testLisResultADO != null
                        && testLisResultADO.SAMPLE_SERVICE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_SERVICE_STT.ID__DA_TRA_KQ)
                    {
                        var dataChildNullValue = dataChildOfFocus.Where(o => String.IsNullOrWhiteSpace(o.VALUE_RANGE)).ToList();

                        if (LisConfigCFG.IS_ALLOW_SAVE_WHEN_NOT_FULL_RESULT == "2" && dataChildNullValue != null && dataChildNullValue.Count() > 0
                            && MessageBox.Show(String.Format("Chỉ số ({0}) chưa có kết quả, bạn có muốn trả kết quả không?", String.Join("; ", dataChildNullValue.Select(o => o.TEST_INDEX_NAME))), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            return;
                        }
                        else if (LisConfigCFG.IS_ALLOW_SAVE_WHEN_NOT_FULL_RESULT == "0" && dataChildNullValue != null && dataChildNullValue.Count() > 0)
                        {
                            MessageBox.Show(String.Format("Chỉ số ({0}) chưa có kết quả", String.Join("; ", dataChildNullValue.Select(o => o.TEST_INDEX_NAME))), "Thông báo");
                            return;
                        }
                        else if (LisConfigCFG.IS_ALLOW_SAVE_WHEN_NOT_FULL_RESULT == "3" && !this.CheckOption3(dataChildNullValue))
                        {
                            return;
                        }
                        WaitingManager.Show();
                        bool success = false;
                        CommonParam param = new CommonParam();

                        List<long> lisResultIds = new List<long>();
                        var dataChilds = dataChildOfFocus.Where(p => p.LIS_RESULT_ID.HasValue).ToList();
                        lisResultIds.AddRange(dataChilds.Select(o => o.LIS_RESULT_ID.Value).Distinct().ToList());

                        var rs = new BackendAdapter(param).Post<List<LIS_SAMPLE_SERVICE>>("api/LisSampleService/ReturnResult", ApiConsumers.LisConsumer, lisResultIds, param);
                        if (rs != null)
                        {
                            success = true;
                            FillDataToGridControl();
                            if (lstSampleAll.Count > 1)
                                RowClick();

                            string testIndexStr = "";
                            foreach (var item in dataChilds)
                            {
                                testIndexStr += item.TEST_INDEX_NAME + " - " + item.VALUE_RANGE + "; ";
                            }

                            string message = string.Format("Trả kết quả từng phần. SERVICE_REQ_CODE: {0}. BARCODE: {1}.  TEST_INDEX_NAME - VALUE: [{2}]", rowSample.SERVICE_REQ_CODE, rowSample.BARCODE, testIndexStr);
                            string login = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                            SdaEventLogCreate eventlog = new SdaEventLogCreate();
                            eventlog.Create(login, null, true, message);
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                    else
                    {
                        MessageManager.Show(String.Format("({0}) chưa nhập giá trị cho chỉ số.", testLisResultADO.TEST_INDEX_NAME));
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TextEditValueRange__Enable_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    treeListSereServTein.PostEditor();
                    TreeListNode node = treeListSereServTein.FocusedNode;
                    if (node != null && node.NextNode != null)
                    {
                        if (node.NextVisibleNode != null && node.NextVisibleNode.HasChildren)
                        {
                            treeListSereServTein.FocusedNode = node.NextVisibleNode.Nodes[0];
                        }
                        else if (node.NextVisibleNode.NextNode != null)
                        {
                            if (node.NextVisibleNode.NextVisibleNode != null
                                && node.NextVisibleNode.NextVisibleNode.HasChildren)
                            {
                                treeListSereServTein.FocusedNode = node.NextVisibleNode.NextVisibleNode.Nodes[0];
                            }
                            else
                            {
                                treeListSereServTein.FocusedNode = node.NextNode;//.NextVisibleNode.NextNode;
                            }
                        }
                        else
                            treeListSereServTein.FocusedNode = node.NextNode;
                    }
                    else if (node != null && node.NextVisibleNode != null)
                    {
                        if (node.NextVisibleNode.HasChildren)
                        {
                            treeListSereServTein.FocusedNode = node.NextVisibleNode.Nodes[0];
                        }
                        else if (node.NextVisibleNode.NextNode != null)
                        {
                            if (node.NextVisibleNode.NextVisibleNode != null && node.NextVisibleNode.NextVisibleNode.HasChildren)
                            {
                                if (node.NextVisibleNode.NextVisibleNode.NextVisibleNode != null
                                    && node.NextVisibleNode.NextVisibleNode.NextVisibleNode.HasChildren)
                                {
                                    treeListSereServTein.FocusedNode = node.NextVisibleNode.NextVisibleNode.NextVisibleNode.Nodes[0];
                                }
                                else
                                    treeListSereServTein.FocusedNode = node.NextVisibleNode;
                            }
                            else if (node.NextVisibleNode.NextVisibleNode != null)
                            {
                                treeListSereServTein.FocusedNode = node.NextVisibleNode.NextVisibleNode;
                            }
                            else
                            {
                                treeListSereServTein.FocusedNode = node.NextVisibleNode;
                            }
                        }
                    }
                    treeListSereServTein.FocusedColumn = treeListSereServTein.Columns[grdColVallue.FieldName];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemText__Description_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    treeListSereServTein.PostEditor();
                    TreeListNode node = treeListSereServTein.FocusedNode;
                    if (node != null && node.NextNode != null)
                    {
                        if (node.NextVisibleNode != null && node.NextVisibleNode.HasChildren)
                        {
                            treeListSereServTein.FocusedNode = node.NextVisibleNode.Nodes[0];
                        }
                        else if (node.NextVisibleNode.NextNode != null)
                        {
                            if (node.NextVisibleNode.NextVisibleNode != null
                                && node.NextVisibleNode.NextVisibleNode.HasChildren)
                            {
                                treeListSereServTein.FocusedNode = node.NextVisibleNode.NextVisibleNode.Nodes[0];
                            }
                            else
                            {
                                treeListSereServTein.FocusedNode = node.NextNode;//.NextVisibleNode.NextNode;
                            }
                        }
                        else
                            treeListSereServTein.FocusedNode = node.NextNode;
                    }
                    else if (node != null && node.NextVisibleNode != null)
                    {
                        if (node.NextVisibleNode.HasChildren)
                        {
                            treeListSereServTein.FocusedNode = node.NextVisibleNode.Nodes[0];
                        }
                        else if (node.NextVisibleNode.NextNode != null)
                        {
                            if (node.NextVisibleNode.NextVisibleNode != null && node.NextVisibleNode.NextVisibleNode.HasChildren)
                            {
                                if (node.NextVisibleNode.NextVisibleNode.NextVisibleNode != null
                                    && node.NextVisibleNode.NextVisibleNode.NextVisibleNode.HasChildren)
                                {
                                    treeListSereServTein.FocusedNode = node.NextVisibleNode.NextVisibleNode.NextVisibleNode.Nodes[0];
                                }
                                else
                                    treeListSereServTein.FocusedNode = node.NextVisibleNode;
                            }
                            else if (node.NextVisibleNode.NextVisibleNode != null)
                            {
                                treeListSereServTein.FocusedNode = node.NextVisibleNode.NextVisibleNode;
                            }
                            else
                            {
                                treeListSereServTein.FocusedNode = node.NextVisibleNode;
                            }
                        }
                    }
                    treeListSereServTein.FocusedColumn = treeListSereServTein.Columns[treeListColDescription.FieldName];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnInTachTheoNhom_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;
                bool print = false;
                bool hasConfirmUser = false;
                bool resultConfirmUser = false;
                listCheckPrint = GetListCheckPrint();
                ProcessSaveAndReturnValue(ref print, ref hasConfirmUser, ref resultConfirmUser);
                this.PrintOption = PRINT_OPTION.IN_TACH_THEO_NHOM;
                if (print)
                {
                    SetDataToPrint(false);
                    PrintProcess(PrintTypeKXN.IN_KET_QUA_XET_NGHIEM);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSample_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "BARCODE")
                {
                    var focus = (LisSampleADO)gridViewSample.GetFocusedRow();
                    if (LisConfigCFG.IS_AUTO_CREATE_BARCODE != "1" && LisConfigCFG.IsAutoSampleAfterEnterBarcode)
                    {
                        if (focus.SAMPLE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM && focus.SAMPLE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                        {
                            UpdateBarcode(focus);
                        }
                        else if (focus.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM || focus.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                        {
                            LayMauBenhPham(focus);
                        }
                    }
                    else if (LisConfigCFG.IS_AUTO_CREATE_BARCODE != "1" && focus.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM)
                    {
                        UpdateBarcode(focus);
                    }
                }
                else if (e.Column.Name == gc_CheckApprovalList.Name)
                {
                    btnApproveListResult.Focus();
                    gridViewSample.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateBarcode(LisSampleADO sample)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool result = false;
                LisSampleUpdateBarcodeSDO input = new LisSampleUpdateBarcodeSDO();
                input.SampleId = sample.ID;
                input.Barcode = sample.BARCODE;

                WaitingManager.Show();
                var rs = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/UpdateBarcode", ApiConsumers.LisConsumer, input, param);

                if (rs != null)
                {
                    sample.BARCODE = rs.BARCODE;
                    lstSampleAll.FirstOrDefault(o => o.ID == rs.ID).BARCODE = rs.BARCODE;
                    result = true;
                    lblNewestBarcode.Text = sample.BARCODE;
                }
                else
                {
                    result = false;
                    if (IsSearchByTreatmentCode)
                    {
                        FocusCellByRowHandle(gridViewSample, lstSampleAll.IndexOf(lstSampleAll.FirstOrDefault(o => o.ID == sample.ID)), gridColumnBarCode);
                    }
                    else
                    {
                        FillDataToGridControl();
                    }
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, result);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSample_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (LisSampleADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "BARCODE")
                        {
                            if (LisConfigCFG.IS_AUTO_CREATE_BARCODE == "1" || data.SAMPLE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM)
                            {
                                e.Appearance.BackColor = Color.LightGray;
                            }
                        }

                        if (HisConfigCFG.WARNING_TIME_RETURN_RESULT != "0"
                            && data.APPOINTMENT_TIME.HasValue && data.APPOINTMENT_TIME < Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now)
                            && (data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM || data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ || data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN))
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                        else if (HisConfigCFG.WARNING_TIME_RETURN_RESULT != "0"
                            && data.APPOINTMENT_TIME.HasValue && data.APPOINTMENT_TIME - (Convert.ToInt64(HisConfigCFG.WARNING_TIME_RETURN_RESULT == "" ? "0" : HisConfigCFG.WARNING_TIME_RETURN_RESULT) * 100) < Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now)
                            && (data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM || data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ || data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN))
                        {
                            e.Appearance.ForeColor = Color.Orange;
                        }
                        else
                        {
                            e.Appearance.ForeColor = Color.Black;
                        }
                        if (data.IS_EMERGENCY == 1)
                        {
                            e.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
                        }
                        else if (data.PRIORITY == 1)
                        {
                            e.Appearance.ForeColor = Color.Green;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeListSereServTein_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
        {
            try
            {
                TreeList tree = sender as TreeList;
                if (ModifierKeys == Keys.None
                    && tree.State == TreeListState.Regular)
                {
                    Point pt = tree.PointToClient(MousePosition);
                    TreeListHitInfo info = tree.CalcHitInfo(pt);
                    if (info.HitInfoType == HitInfoType.Cell || info.HitInfoType == HitInfoType.RowIndicator)
                        tree.FocusedNode = info.Node;

                    else if (info.HitInfoType == HitInfoType.Column)
                    {
                        DXMenuItem menuItem = new DXMenuItem("Hide This Column", new EventHandler(this.MenuItemClick_AnCot));
                        menuItem.Tag = info.Column;
                        e.Menu.Items.Add(menuItem);
                    }

                    if (tree.FocusedNode != null)
                    {
                        TestLisResultADO data = (TestLisResultADO)this.treeListSereServTein.GetDataRecordByNode(this.treeListSereServTein.FocusedNode);
                        if (data.IS_PARENT != 1)
                        {
                            return;
                        }
                        if (data.IS_NO_EXECUTE == 1)
                        {
                            // bỏ không thực hiện
                            DXMenuItem menuItem = new DXMenuItem("Bỏ không thực hiện", new EventHandler(this.MenuItemClick_BoKhongThucHien), imageCollection1.Images[9]);
                            menuItem.Tag = info.Column;
                            e.Menu.Items.Add(menuItem);
                        }
                        else
                        {
                            DXMenuItem menuItem = new DXMenuItem("Không thực hiện", new EventHandler(this.MenuItemClick_KhongThucHien), imageCollection1.Images[10]);
                            menuItem.Tag = info.Column;
                            e.Menu.Items.Add(menuItem);
                        }
                        var AllCheckNodes = treeListSereServTein.GetAllCheckedNodes();
                        if (AllCheckNodes != null && AllCheckNodes.Count > 0 && AllCheckNodes.FirstOrDefault(o => o.Id == this.treeListSereServTein.FocusedNode.Id) != null)
                        {
                            DXMenuItem menuMachine = new DXMenuItem("Chọn máy kết quả", new EventHandler(MenuItemClick_ChonMayKq), imageCollection1.Images[11]);
                            menuMachine.Tag = info.Column;
                            e.Menu.Items.Add(menuMachine);
                        }

                        if (this.rowSample.SAMPLE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                        {
                            if (data.IS_CONFIRM_NO_EXCUTE == 1)
                            {
                                DXMenuItem menuItem = new DXMenuItem("Hủy xác nhận không thực hiện", new EventHandler(this.MenuItemClick_HuyXacNhanKhongThucHien), imageCollection1.Images[12]);
                                menuItem.Tag = info.Column;
                                e.Menu.Items.Add(menuItem);
                            }
                            else
                            {
                                DXMenuItem menuItem = new DXMenuItem("Xác nhận không thực hiện", new EventHandler(this.MenuItemClick_XacNhanKhongThucHien), imageCollection1.Images[12]);
                                menuItem.Tag = info.Column;
                                e.Menu.Items.Add(menuItem);
                            }
                        }
                        if (data.IS_RUNNING == 1)
                        {
                            DXMenuItem menuItem = new DXMenuItem("Chạy lại", new EventHandler(this.MenuItemClick_ChayLai), imageCollection1.Images[7]);
                            menuItem.Tag = info.Column;
                            e.Menu.Items.Add(menuItem);
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MenuItemClick_ChonMayKq(object sender, EventArgs e)
        {
            try
            {
                List<LIS_MACHINE> dataMachines = new List<LIS_MACHINE>();
                if (this._Machines != null && this._Machines.Count > 0 && this.rowSample != null)
                {
                    dataMachines = this._Machines.Where(o => o.EXECUTE_ROOM_CODE == null || o.EXECUTE_ROOM_CODE == this.rowSample.EXECUTE_ROOM_CODE).ToList();
                }
                if (dataMachines != null && dataMachines.Count() > 0)
                {
                    machineSources = (from n in dataMachines select new LisMachineAdo(n, GlobalVariables.MachineCounterSdos != null ? GlobalVariables.MachineCounterSdos.FirstOrDefault(o => o.MACHINE_CODE == n.MACHINE_CODE) : null)).ToList();
                    machineSources = machineSources.Where(o => o.IS_ACTIVE == IMSys.DbConfig.LIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                }
                frmMachine frm = new frmMachine(machineSources, (machineId) =>
                {
                    if (machineId == null)
                        return;
                    var AllCheckNodes = treeListSereServTein.GetAllCheckedNodes();
                    foreach (var item in AllCheckNodes)
                    {
                        var data = (TestLisResultADO)this.treeListSereServTein.GetDataRecordByNode(item);
                        if (data.IS_PARENT == 1)
                        {
                            data.MACHINE_ID_OLD = data.MACHINE_ID;
                            data.MACHINE_ID = machineId;
                            SaveMachineCount(data, machineId ?? 0);
                        }
                    }
                    treeListSereServTein.RefreshDataSource();
                });
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void SaveMachineCount(TestLisResultADO data, long machineId)
        {
            try
            {

                var checkData = CheckConfigWarningMachine(machineId, data);
                if (checkData != null)
                {
                    if (HisConfigCFG.MaxServicePerDayWarningOption == "1")
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Máy {0} vượt quá số lượng dịch vụ xử lý tối đa trong ngày ({1}). Bạn có muốn tiếp tục?", checkData.MACHINE_NAME, checkData.MAX_SERVICE_PER_DAY), "Thông báo",
                          MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            data.MACHINE_ID = null;
                            treeListSereServTein.RefreshNode(treeListSereServTein.FocusedNode);
                            return;
                        }
                    }
                    else if (HisConfigCFG.MaxServicePerDayWarningOption == "2")
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Máy {0} vượt quá số lượng dịch vụ xử lý tối đa trong ngày ({1})", checkData.MACHINE_NAME, checkData.MAX_SERVICE_PER_DAY), "Thông báo");
                        data.MACHINE_ID = null;
                        treeListSereServTein.RefreshNode(treeListSereServTein.FocusedNode);
                        return;
                    }

                };
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                LisSampleServiceMachineSDO sdo = new LisSampleServiceMachineSDO();
                sdo.MachineId = machineId;
                sdo.SampleServiceId = data.SAMPLE_SERVICE_ID.HasValue ? data.SAMPLE_SERVICE_ID.Value : 0;
                var rs = new BackendAdapter(param).Post<LIS_SAMPLE_SERVICE>("api/LisSampleService/UpdateMachine", ApiConsumers.LisConsumer, sdo, param);
                if (rs != null)
                {
                    success = true;
                }

                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void MenuItemClick_ChayLai(object sender, EventArgs e)
        {
            try
            {
                TestLisResultADO data = (TestLisResultADO)this.treeListSereServTein.GetDataRecordByNode(this.treeListSereServTein.FocusedNode);

                if (MessageBox.Show(String.Format("Bạn có xác nhận chạy lại dịch vụ {0} không?", data.SERVICE_NAME), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    CommonParam param = new CommonParam();
                    bool success = false;
                    WaitingManager.Show();
                    var result = new BackendAdapter(param).Post<LIS_SAMPLE_SERVICE>("api/LisSampleService/Unrunning", ApiConsumers.LisConsumer, data.SAMPLE_SERVICE_ID, param);
                    if (result != null)
                    {
                        success = true;
                        RowClick();
                    }
                    WaitingManager.Hide();
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MenuItemClick_AnCot(object sender, EventArgs e)
        {
            try
            {
                var menu = sender as DXMenuItem;
                if (menu != null && menu.Tag != null)
                {
                    var col = menu.Tag as TreeListColumn;
                    treeListSereServTein.Columns[col.FieldName].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MenuItemClick_BoKhongThucHien(object sender, EventArgs e)
        {
            try
            {
                TestLisResultADO data = (TestLisResultADO)this.treeListSereServTein.GetDataRecordByNode(this.treeListSereServTein.FocusedNode);

                var ChildData = this.lstLisResultADOs.Where(o => o.PARENT_ID == data.CHILD_ID || (data.HAS_ONE_CHILD == 1 && o.CHILD_ID == data.CHILD_ID));

                if (MessageBox.Show(String.Format("Bạn có xác nhận dịch vụ {0} bỏ không thực hiện không?", data.SERVICE_NAME), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    UpdateSampleServiceNoExecuteSDO updateSampleServiceNoExecuteSDO = new LIS.SDO.UpdateSampleServiceNoExecuteSDO();
                    updateSampleServiceNoExecuteSDO.ServiceCodes = new List<string>();
                    updateSampleServiceNoExecuteSDO.ServiceCodes.Add(data.SERVICE_CODE);
                    updateSampleServiceNoExecuteSDO.ServiceReqCode = rowSample.SERVICE_REQ_CODE;
                    updateSampleServiceNoExecuteSDO.IsNoExecute = false;

                    CommonParam param = new CommonParam();
                    bool success = false;
                    WaitingManager.Show();
                    var result = new BackendAdapter(param).Post<bool>("api/LisSampleService/UpdateNoExecute", ApiConsumers.LisConsumer, updateSampleServiceNoExecuteSDO, param);
                    if (result)
                    {
                        success = true;
                        RowClick();
                    }
                    WaitingManager.Hide();
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MenuItemClick_KhongThucHien(object sender, EventArgs e)
        {
            try
            {
                TestLisResultADO data = (TestLisResultADO)this.treeListSereServTein.GetDataRecordByNode(this.treeListSereServTein.FocusedNode);

                var ChildData = this.lstLisResultADOs.Where(o => o.PARENT_ID == data.CHILD_ID || (data.HAS_ONE_CHILD == 1 && o.CHILD_ID == data.CHILD_ID));

                var childHasValue = ChildData.Where(o => !String.IsNullOrWhiteSpace(o.VALUE_RANGE));
                if (childHasValue != null && childHasValue.Count() > 0)
                {
                    string message = String.Join("; ", childHasValue.Select(o => o.TEST_INDEX_NAME));
                    MessageManager.Show(String.Format("Các chỉ số đã có giá trị: {0} ", message));
                    return;
                }

                if (MessageBox.Show(String.Format("Bạn có xác nhận dịch vụ {0} không thực hiện không?", data.SERVICE_NAME), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    UpdateSampleServiceNoExecuteSDO updateSampleServiceNoExecuteSDO = new LIS.SDO.UpdateSampleServiceNoExecuteSDO();
                    updateSampleServiceNoExecuteSDO.ServiceCodes = new List<string>();
                    updateSampleServiceNoExecuteSDO.ServiceCodes.Add(data.SERVICE_CODE);
                    updateSampleServiceNoExecuteSDO.ServiceReqCode = rowSample.SERVICE_REQ_CODE;
                    updateSampleServiceNoExecuteSDO.IsNoExecute = true;

                    CommonParam param = new CommonParam();
                    bool success = false;
                    WaitingManager.Show();
                    var result = new BackendAdapter(param).Post<bool>("api/LisSampleService/UpdateNoExecute", ApiConsumers.LisConsumer, updateSampleServiceNoExecuteSDO, param);
                    if (result)
                    {
                        success = true;
                        RowClick();
                    }
                    WaitingManager.Hide();
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void MenuItemClick_XacNhanKhongThucHien(object sender, EventArgs e)
        {
            try
            {
                TestLisResultADO data = (TestLisResultADO)this.treeListSereServTein.GetDataRecordByNode(this.treeListSereServTein.FocusedNode);

                if (MessageBox.Show(String.Format("“Bạn có muốn xác nhận không thực hiện dịch vụ {0} không?", data.SERVICE_NAME), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    HisTestConfirmNoExcuteTDO hisTestConfirmNoExcuteTDO = new HisTestConfirmNoExcuteTDO();
                    hisTestConfirmNoExcuteTDO.ServiceReqCode = this.rowSample.SERVICE_REQ_CODE;
                    hisTestConfirmNoExcuteTDO.TestServiceTypeList = new List<HisTestServiceTypeTDO>();
                    HisTestServiceTypeTDO hisTestServiceTypeTDO = new HisTestServiceTypeTDO();
                    hisTestServiceTypeTDO.TestServiceTypeCode = data.TEST_INDEX_CODE;
                    hisTestServiceTypeTDO.ConfirmNoExcute = true;
                    hisTestConfirmNoExcuteTDO.TestServiceTypeList.Add(hisTestServiceTypeTDO);

                    CommonParam param = new CommonParam();
                    bool success = false;
                    WaitingManager.Show();
                    var result = new BackendAdapter(param).Post<bool>("api/HisTestServiceReq/ConfirmNoExcute", ApiConsumers.MosConsumer, hisTestConfirmNoExcuteTDO, param);
                    if (result)
                    {
                        success = true;
                        RowClick();
                    }
                    WaitingManager.Hide();
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void MenuItemClick_HuyXacNhanKhongThucHien(object sender, EventArgs e)
        {
            try
            {
                TestLisResultADO data = (TestLisResultADO)this.treeListSereServTein.GetDataRecordByNode(this.treeListSereServTein.FocusedNode);

                if (MessageBox.Show(String.Format("Bạn có muốn hủy xác nhận không thực hiện dịch vụ {0} không?", data.SERVICE_NAME), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    HisTestConfirmNoExcuteTDO hisTestConfirmNoExcuteTDO = new HisTestConfirmNoExcuteTDO();
                    hisTestConfirmNoExcuteTDO.ServiceReqCode = this.rowSample.SERVICE_REQ_CODE;
                    hisTestConfirmNoExcuteTDO.TestServiceTypeList = new List<HisTestServiceTypeTDO>();
                    HisTestServiceTypeTDO hisTestServiceTypeTDO = new HisTestServiceTypeTDO();
                    hisTestServiceTypeTDO.TestServiceTypeCode = data.TEST_INDEX_CODE;
                    hisTestServiceTypeTDO.ConfirmNoExcute = false;
                    hisTestConfirmNoExcuteTDO.TestServiceTypeList.Add(hisTestServiceTypeTDO);

                    CommonParam param = new CommonParam();
                    bool success = false;
                    WaitingManager.Show();
                    var result = new BackendAdapter(param).Post<bool>("api/HisTestServiceReq/ConfirmNoExcute", ApiConsumers.MosConsumer, hisTestConfirmNoExcuteTDO, param);
                    if (result)
                    {
                        success = true;
                        RowClick();
                    }
                    WaitingManager.Hide();
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListSereServTein_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                TreeList tree = sender as TreeList;
                Point pt = new Point(e.X, e.Y);
                TreeListHitInfo hit = tree.CalcHitInfo(pt);
                if (hit.Column != null && hit.Column.VisibleIndex == 0)
                {
                    DevExpress.XtraTreeList.ViewInfo.ColumnInfo info = tree.ViewInfo.ColumnsInfo[hit.Column];
                    Rectangle checkRect = new Rectangle(info.Bounds.Left + 3, info.Bounds.Top + 3, 12, 12);
                    if (checkRect.Contains(pt))
                    {
                        hit.Column.OptionsColumn.AllowSort = false;
                        EmbeddedCheckBoxChecked(tree);
                    }
                    else
                    {
                        hit.Column.OptionsColumn.AllowSort = true;
                    }
                }
                else if (hit.Column != null && hit.Column.Name == treeListColumn_ReRun.Name)
                {
                    DevExpress.XtraTreeList.ViewInfo.ColumnInfo info = tree.ViewInfo.ColumnsInfo[hit.Column];
                    Rectangle checkRect = new Rectangle(info.Bounds.Left + 3, info.Bounds.Top + 3, 12, 12);
                    if (checkRect.Contains(pt))
                    {
                        hit.Column.OptionsColumn.AllowSort = false;
                        bool checkAll = false;
                        List<TestLisResultADO> listResultCheck = EmbeddedCheckBoxCheckedReRun(tree, ref checkAll);
                        if (checkAll && listResultCheck != null && listResultCheck.Count > 0)
                        {
                            if (XtraMessageBox.Show("Bạn có xác nhận chạy lại tất cả dịch vụ trong phiếu không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                                return;
                            WaitingManager.Show();
                            bool success = false;
                            CommonParam param = new CommonParam();
                            List<long> listSampleServiceIds = listResultCheck.Select(o => o.SAMPLE_SERVICE_ID ?? 0).ToList();
                            var rs = new BackendAdapter(param).Post<List<LIS_SAMPLE_SERVICE>>("api/LisSampleService/UnrunningList", ApiConsumers.LisConsumer, listSampleServiceIds, param);
                            if (rs != null && rs.Count > 0)
                            {
                                success = true;
                                RowClick();
                            }
                            treeListSereServTein.RefreshDataSource();
                            WaitingManager.Hide();
                            MessageManager.Show(this.ParentForm, param, success);
                        }
                    }
                    else
                    {
                        hit.Column.OptionsColumn.AllowSort = true;
                    }
                }
                else if (e.Button == MouseButtons.Right && ModifierKeys == Keys.None
                    && tree.State == TreeListState.Regular && (hit.HitInfoType == HitInfoType.Cell || hit.HitInfoType == HitInfoType.RowIndicator))
                {
                    tree.FocusedNode = hit.Node;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridControlSample_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {
                ColumnView view = (sender as GridControl).FocusedView as ColumnView;
                if (view == null) return;

                if ((e.KeyCode == Keys.P || e.KeyCode == Keys.S || e.KeyCode == Keys.F) && e.Control || e.KeyCode == Keys.F1 || e.KeyCode == Keys.F2 || e.Control)
                    return;
                rowSample = null;

                if (e.KeyCode == Keys.Down)
                {
                    rowSample = (LisSampleADO)view.GetRow(view.FocusedRowHandle + 1);
                }
                else if (e.KeyCode == Keys.Up)
                {
                    rowSample = (LisSampleADO)view.GetRow(view.FocusedRowHandle - 1);
                }
                else if (e.KeyCode == Keys.Enter && IsSearchByTreatmentCode)
                {
                    rowSample = (LisSampleADO)gridViewSample.GetFocusedRow();
                    if (lstSampleAll.Exists(o => string.IsNullOrEmpty(o.BARCODE) && o.ID != rowSample.ID))
                    {
                        FocusCellByRowHandle(gridViewSample, lstSampleAll.IndexOf(lstSampleAll.FirstOrDefault(o => string.IsNullOrEmpty(o.BARCODE) && o.ID != rowSample.ID)), gridColumnBarCode);
                    }else
                    {
                        StartTimer(currentModule.ModuleLink, "timer3");
                    }
                    return;
                }
                else
                {
                    return;
                }

                if (rowSample == null)
                    return;

                if (view.ActiveEditor != null) return;//Prevent record deletion when an in-place editor is invoked
                WaitingManager.Show();
                LoadLisResult(rowSample);
                LoadDataToGridTestResult2();
                SetDataToCommon(rowSample);
                if (rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                {
                    btnSave.Enabled = false;
                }
                else
                {
                    btnSave.Enabled = true;
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryTuChoiMauE_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var row = (LisSampleADO)gridViewSample.GetFocusedRow();
                if (row != null && (row.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM
                    || row.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN))
                {
                    frmReasonReject frm = new frmReasonReject(row, (obj) =>
                    {
                        if (obj != null)
                        {
                            var sampleRow = lstSampleAll.FirstOrDefault(o => o.ID == row.ID);
                            sampleRow.SAMPLE_STT_ID = obj.SAMPLE_STT_ID;
                            sampleRow.APPROVAL_TIME = obj.APPROVAL_TIME;
                            sampleRow.APPROVAL_LOGINNAME = obj.APPROVAL_LOGINNAME;
                            sampleRow.APPROVAL_USERNAME = obj.APPROVAL_USERNAME;
                            sampleRow.IS_SAMPLE_ORDER_REQUEST = obj.IS_SAMPLE_ORDER_REQUEST;
                            sampleRow.REJECT_REASON = obj.REJECT_REASON;
                            sampleRow.SAMPLE_ORDER = obj.SAMPLE_ORDER;
                            gridControlSample.RefreshDataSource();
                            gridViewSample.FocusedRowHandle = gridViewSample.FocusedRowHandle - 1;
                            gridViewSample.FocusedRowHandle = gridViewSample.FocusedRowHandle + 1;
                        }
                    });
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryChapNhanMauE_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (LisSampleADO)gridViewSample.GetFocusedRow();
                if (row == null || row.SAMPLE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM) return;

                if (LisConfigCFG.ALLOW_TO_EDIT_APPROVE_TIME == "1")
                {
                    frmChapNhanMau frm = new frmChapNhanMau((obj) =>
                    {
                        if (obj != null)
                        {
                            var sampleRow = lstSampleAll.FirstOrDefault(o => o.ID == row.ID);
                            sampleRow.SAMPLE_STT_ID = obj.SAMPLE_STT_ID;
                            sampleRow.APPROVAL_TIME = obj.APPROVAL_TIME;
                            sampleRow.APPROVAL_LOGINNAME = obj.APPROVAL_LOGINNAME;
                            sampleRow.APPROVAL_USERNAME = obj.APPROVAL_USERNAME;
                            sampleRow.IS_SAMPLE_ORDER_REQUEST = obj.IS_SAMPLE_ORDER_REQUEST;
                            sampleRow.REJECT_REASON = obj.REJECT_REASON;
                            sampleRow.SAMPLE_ORDER = obj.SAMPLE_ORDER;
                            gridControlSample.RefreshDataSource();
                            gridViewSample.FocusedRowHandle = gridViewSample.FocusedRowHandle - 1;
                            gridViewSample.FocusedRowHandle = gridViewSample.FocusedRowHandle + 1;
                        }
                    }, row);
                    frm.ShowDialog();
                }
                else
                {
                    if (row.SAMPLE_TIME == null || LisConfigCFG.WARNING_APPROVE_TIME == 0) return;

                    TimeSpan time = DateTime.Now - (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Convert.ToInt64(row.SAMPLE_TIME.ToString().Substring(0, 12) + "00"));
                    if (time.TotalMinutes > LisConfigCFG.WARNING_APPROVE_TIME)
                    {
                        if (XtraMessageBox.Show(String.Format("Bệnh nhân có thời gian duyệt mẫu xét nghiệm lớn hơn thời gian lấy mẫu {0} phút. Bạn có muốn tiếp tục?", LisConfigCFG.WARNING_APPROVE_TIME), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                            return;
                    }

                    bool success = false;
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    LisSampleApproveSDO sdo = new LisSampleApproveSDO();
                    sdo.SampleId = row.ID;
                    var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/Approve", ApiConsumer.ApiConsumers.LisConsumer, sdo, param);
                    if (curentSTT != null)
                    {
                        success = true;
                        var sampleRow = lstSampleAll.FirstOrDefault(o => o.ID == row.ID);
                        sampleRow.SAMPLE_STT_ID = curentSTT.SAMPLE_STT_ID;
                        sampleRow.APPROVAL_TIME = curentSTT.APPROVAL_TIME;
                        sampleRow.APPROVAL_LOGINNAME = curentSTT.APPROVAL_LOGINNAME;
                        sampleRow.APPROVAL_USERNAME = curentSTT.APPROVAL_USERNAME;
                        sampleRow.IS_SAMPLE_ORDER_REQUEST = curentSTT.IS_SAMPLE_ORDER_REQUEST;
                        sampleRow.REJECT_REASON = curentSTT.REJECT_REASON;
                        sampleRow.SAMPLE_ORDER = curentSTT.SAMPLE_ORDER;
                        gridControlSample.RefreshDataSource();
                        gridViewSample.FocusedRowHandle = gridViewSample.FocusedRowHandle - 1;
                        gridViewSample.FocusedRowHandle = gridViewSample.FocusedRowHandle + 1;
                    }
                    MessageManager.Show(this.ParentForm, param, success);
                    WaitingManager.Hide();
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryDuyetKetQuaE_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool result = false;
                bool IsShowMessage = true;
                CommonParam param = new CommonParam();
                var row = (LisSampleADO)gridViewSample.GetFocusedRow();
                if (rowSample == null || row.ID != rowSample.ID)
                {
                    rowSample = row;
                    LoadLisResult(rowSample);
                    LoadDataToGridTestResult2();
                    SetDataToCommon(rowSample);
                }
                if (row != null && (row.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ
                    || row.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN))
                {

                    if (LisConfigCFG.SHOW_BUTTON_APPROVE == "1" && LisConfigCFG.IS_REQUIRED_APPROVE_SAMPLE)
                    {
                        if (row.APPROVAL_TIME == null)
                        {
                            WaitingManager.Hide();
                            MessageBox.Show("Y lệnh chưa chấp nhận mẫu. Không được phép duyệt kết quả.", "Thông báo");
                            gridViewSample.FocusedColumn = gc_ApproveSample;
                            return;
                        }
                    }
                    var dataChildNullValue = lstLisResultADOs != null && lstLisResultADOs.Count() > 0 ?
                        lstLisResultADOs.Where(o => (o.IS_PARENT != 1 || (o.IS_PARENT == 1 && o.HAS_ONE_CHILD == 1)) && String.IsNullOrWhiteSpace(o.VALUE_RANGE)).ToList()
                        : null;

                    if (LisConfigCFG.IS_ALLOW_SAVE_WHEN_NOT_FULL_RESULT == "2" && dataChildNullValue != null && dataChildNullValue.Count() > 0
                        && MessageBox.Show(String.Format("Chỉ số ({0}) chưa có kết quả, bạn có muốn duyệt kết quả không?", String.Join("; ", dataChildNullValue.Select(o => o.TEST_INDEX_NAME))), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        return;
                    }
                    else if (LisConfigCFG.IS_ALLOW_SAVE_WHEN_NOT_FULL_RESULT == "0" && dataChildNullValue != null && dataChildNullValue.Count() > 0)
                    {
                        MessageBox.Show(String.Format("Chỉ số ({0}) chưa có kết quả", String.Join("; ", dataChildNullValue.Select(o => o.TEST_INDEX_NAME))), "Thông báo");
                        return;
                    }
                    else if (LisConfigCFG.IS_ALLOW_SAVE_WHEN_NOT_FULL_RESULT == "3" && !this.CheckOption3(dataChildNullValue))
                        return;

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => row), row));
                    if (LisConfigCFG.ALLOW_TO_EDIT_APPROVE_RESULT_TIME)
                    {
                        WaitingManager.Hide();
                        frmApproveResult frm = new frmApproveResult(row, (obj) =>
                        {
                            if (obj != null)
                            {
                                var sampleRow = lstSampleAll.FirstOrDefault(o => o.ID == row.ID);
                                sampleRow.SAMPLE_STT_ID = obj.SAMPLE_STT_ID;
                                sampleRow.RESULT_APPROVAL_TIME = obj.RESULT_APPROVAL_TIME;
                                sampleRow.RESULT_APPROVAL_LOGINNAME = obj.RESULT_APPROVAL_LOGINNAME;
                                sampleRow.RESULT_APPROVAL_USERNAME = obj.RESULT_APPROVAL_USERNAME;
                                sampleRow.IS_SAMPLE_ORDER_REQUEST = obj.IS_SAMPLE_ORDER_REQUEST;
                                gridControlSample.RefreshDataSource();
                                gridViewSample.FocusedRowHandle = gridViewSample.FocusedRowHandle - 1;
                                gridViewSample.FocusedRowHandle = gridViewSample.FocusedRowHandle + 1;
                                result = true;
                            }
                        }, (IsShow) => IsShowMessage = IsShow);
                        frm.ShowDialog();
                    }
                    else
                    {
                        WaitingManager.Show();
                        LisSampleApproveResultSDO sdo = new LisSampleApproveResultSDO();
                        sdo.SampleId = row.ID;
                        var obj = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/ApproveResult", ApiConsumer.ApiConsumers.LisConsumer, sdo, param);
                        if (obj != null)
                        {
                            var sampleRow = lstSampleAll.FirstOrDefault(o => o.ID == row.ID);
                            sampleRow.SAMPLE_STT_ID = obj.SAMPLE_STT_ID;
                            sampleRow.RESULT_APPROVAL_TIME = obj.RESULT_APPROVAL_TIME;
                            sampleRow.RESULT_APPROVAL_LOGINNAME = obj.RESULT_APPROVAL_LOGINNAME;
                            sampleRow.RESULT_APPROVAL_USERNAME = obj.RESULT_APPROVAL_USERNAME;
                            sampleRow.IS_SAMPLE_ORDER_REQUEST = obj.IS_SAMPLE_ORDER_REQUEST;
                            gridControlSample.RefreshDataSource();
                            gridViewSample.FocusedRowHandle = gridViewSample.FocusedRowHandle - 1;
                            gridViewSample.FocusedRowHandle = gridViewSample.FocusedRowHandle + 1;
                            result = true;
                        }
                        WaitingManager.Hide();
                    }
                }
                WaitingManager.Hide();
                #region Show message
                if (IsShowMessage)
                    MessageManager.Show(this.ParentForm, param, result);
                #endregion

                if (result && LisConfigCFG.IS_PRINT_WHEN_APPROVE_RESULT == "1")
                {
                    this.PrintOption = PRINT_OPTION.IN;
                    listCheckPrint = GetListCheckPrint();
                    if (!HisConfigCFG.PRINT_TEST_RESULT)
                    {
                        SetDataToPrint(false);
                        PrintProcess(PrintTypeKXN.IN_KET_QUA_XET_NGHIEM);
                    }
                    else
                    {
                        SetDataToPrint(true);

                        //Inventec.Common.Logging.LogSystem.Debug("lstResultPrint" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstResultPrint.Select(o => o.SERVICE_CODE).ToList()), lstResultPrint.Select(o => o.SERVICE_CODE).ToList()));
                        //Inventec.Common.Logging.LogSystem.Debug("lstResultHH" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstResultHH.Select(o => o.SERVICE_CODE).ToList()), lstResultHH.Select(o => o.SERVICE_CODE).ToList()));
                        //Inventec.Common.Logging.LogSystem.Debug("lstResultVS" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstResultVS.Select(o => o.SERVICE_CODE).ToList()), lstResultVS.Select(o => o.SERVICE_CODE).ToList()));
                        //Inventec.Common.Logging.LogSystem.Debug("lstResultMD" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstResultMD.Select(o => o.SERVICE_CODE).ToList()), lstResultMD.Select(o => o.SERVICE_CODE).ToList()));
                        //Inventec.Common.Logging.LogSystem.Debug("lstResultSH" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstResultSH.Select(o => o.SERVICE_CODE).ToList()), lstResultSH.Select(o => o.SERVICE_CODE).ToList()));
                        //Inventec.Common.Logging.LogSystem.Debug("lstCheckPrint" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstCheckPrint.Select(o => o.SERVICE_CODE).ToList()), lstCheckPrint.Select(o => o.SERVICE_CODE).ToList()));


                        if (lstResultPrint != null && lstResultPrint.Count > 0)
                        {
                            PrintProcess(PrintTypeKXN.IN_KET_QUA_XET_NGHIEM);
                        }
                        if (lstResultHH != null && lstResultHH.Count > 0)
                        {
                            PrintProcess(PrintTypeKXN.IN_HUYET_HOC);
                        }
                        if (lstResultVS != null && lstResultVS.Count > 0)
                        {
                            PrintProcess(PrintTypeKXN.IN_VI_SINH);
                        }
                        if (lstResultMD != null && lstResultMD.Count > 0)
                        {
                            PrintProcess(PrintTypeKXN.IN_MIEN_DICH);
                        }
                        if (lstResultSH != null && lstResultSH.Count > 0)
                        {
                            PrintProcess(PrintTypeKXN.IN_SINH_HOA);
                        }
                        if (lstResultXNT != null && lstResultXNT.Count > 0)
                        {
                            PrintProcess(PrintTypeKXN.IN_XET_NGHIEM_TEST);
                        }
                        if (lstResultXNGPB != null && lstResultXNGPB.Count > 0)
                        {
                            PrintProcess(PrintTypeKXN.IN_XET_NGHIEM_GIAI_PHAU_BENH);
                        }
                        if (lstResultXNNT != null && lstResultXNNT.Count > 0)
                        {
                            PrintProcess(PrintTypeKXN.IN_XET_NGHIEM_NUOC_TIEU);
                        }
                    }
                }

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryHuyDuyetKetQuaE_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool result = false;
                CommonParam param = new CommonParam();
                var row = (LisSampleADO)gridViewSample.GetFocusedRow();
                if (row != null && (row.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ))
                {
                    var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/UnapproveResult", ApiConsumer.ApiConsumers.LisConsumer, row.ID, param);
                    if (curentSTT != null)
                    {
                        var sampleRow = lstSampleAll.FirstOrDefault(o => o.ID == row.ID);
                        sampleRow.SAMPLE_STT_ID = curentSTT.SAMPLE_STT_ID;
                        sampleRow.APPROVAL_TIME = curentSTT.APPROVAL_TIME;
                        sampleRow.APPROVAL_LOGINNAME = curentSTT.APPROVAL_LOGINNAME;
                        sampleRow.APPROVAL_USERNAME = curentSTT.APPROVAL_USERNAME;
                        sampleRow.IS_SAMPLE_ORDER_REQUEST = curentSTT.IS_SAMPLE_ORDER_REQUEST;
                        gridControlSample.RefreshDataSource();
                        gridViewSample.FocusedRowHandle = gridViewSample.FocusedRowHandle - 1;
                        gridViewSample.FocusedRowHandle = gridViewSample.FocusedRowHandle + 1;
                        WaitingManager.Hide();
                        result = true;
                    }
                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, result);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSample_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                GridHitInfo hi = e.HitInfo;
                if (hi.InRowCell)
                {
                    var row = (LisSampleADO)gridViewSample.GetFocusedRow();
                    if (this.baManager == null)
                    {
                        this.baManager = new BarManager();
                        this.baManager.Form = this;
                    }
                    if (row != null)
                    {
                        List<LisSampleADO> data = null;
                        if (gridControlSample != null)
                        {
                            data = (List<LisSampleADO>)gridControlSample.DataSource;
                        }
                        List<LisSampleADO> listCheck = data.Where(o => o.IsCheck).ToList();
                        //if (listCheck != null && listCheck.Count > 0)
                        {
                            this.popupMenuProcessor = new PopupMenuProcessor(row, this.barManager1, MouseRightItemClick, listCheck != null && listCheck.Count > 0);
                            this.popupMenuProcessor.InitMenu();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MouseRightItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                var row = (LisSampleADO)gridViewSample.GetFocusedRow();
                if ((e.Item is BarButtonItem) && row != null)
                {
                    var type = (PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case PopupMenuProcessor.ItemType.CapNhatTinhTrangMau:
                            this.CapNhatTinhTrangMau(row);
                            break;
                        case PopupMenuProcessor.ItemType.LichSuXetNghiem:
                            this.LichSuXetNghiemCuaBenhNha(row);
                            break;
                        case PopupMenuProcessor.ItemType.TaoEmr:
                            this.DongBoEmr(row);
                            break;
                        case PopupMenuProcessor.ItemType.PrintEmr:
                            this.InVanBanEmr(row);
                            break;
                        case PopupMenuProcessor.ItemType.CapNhatBarcode:
                            this.CapNhatBarcode(row);
                            break;
                        case PopupMenuProcessor.ItemType.InGopBarcode:
                            this.InGopBarcode(row);
                            break;
                        case PopupMenuProcessor.ItemType.AttachTestFile:
                            this.AttachTestFile(row);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AttachTestFile(LisSampleADO row)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AttackFile").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.AttackFile'");
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.AttackFile' is not plugins");
                List<object> listArgs = new List<object>();
                listArgs.Add(new HIS.Desktop.ADO.EmrAttackFileADO() { TreatmentCode = row.TREATMENT_CODE, HisCode = string.Format("TREATMENT_CODE:{0} SERVICE_REQ_CODE:{1} BAR_CODE:{2}",row.TREATMENT_CODE,row.SERVICE_REQ_CODE, row.BARCODE)});
                listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                ((Form)extenceInstance).ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InVanBanEmr(LisSampleADO row)
        {
            try
            {
                if (row != null && !String.IsNullOrWhiteSpace(row.EMR_RESULT_DOCUMENT_CODE))
                {
                    CommonParam param = new CommonParam();
                    var apiResult = GetEmrDocumentFile(row, false, false, false, ref param);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        var currDocument = apiResult.FirstOrDefault();
                        if (!String.IsNullOrWhiteSpace(currDocument.Base64Data))
                        {
                            Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(row != null ? row.TREATMENT_CODE : "", MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096, this.currentModule.RoomId);

                            SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();
                            var signNow = libraryProcessor.SignAndShowPrintPreview(currDocument.Base64Data, Inventec.Common.SignLibrary.FileType.Pdf, inputADO);
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Error("Không tìm thấy văn bản đã ký");
                        }
                    }
                    else
                    {
                        MessageManager.Show(this.ParentForm, param, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private List<EmrDocumentFileSDO> GetEmrDocumentFile(LisSampleADO row, bool? IsMerge, bool? IsShowPatientSign, bool? IsShowWatermark, ref CommonParam paramCommon)
        {
            EmrDocumentDownloadFileSDO sdo = new EmrDocumentDownloadFileSDO();
            var emrFilter = new EMR.Filter.EmrDocumentViewFilter();
            emrFilter.DOCUMENT_CODE__EXACT = row.EMR_RESULT_DOCUMENT_CODE;
            sdo.EmrDocumentViewFilter = emrFilter;
            sdo.IsMerge = IsMerge;
            sdo.IsShowPatientSign = IsShowPatientSign;
            sdo.IsShowWatermark = IsShowWatermark;
            sdo.RoomCode = room != null ? room.ROOM_CODE : null;
            sdo.DepartmentCode = room != null ? room.DEPARTMENT_CODE : null;
            return new BackendAdapter(paramCommon).Post<List<EmrDocumentFileSDO>>("api/EmrDocument/DownloadFile", ApiConsumers.EmrConsumer, sdo, paramCommon);
        }
        private void DongBoEmr(LisSampleADO row)
        {
            try
            {
                if (row != null)
                {
                    LisSampleSyncEmrSDO sdo = new LisSampleSyncEmrSDO();
                    sdo.SampleId = row.ID;
                    sdo.WorkingBranchCode = room.BRANCH_CODE;
                    sdo.WorkingBranchName = room.BRANCH_NAME;
                    sdo.WorkingDepartmentCode = room.DEPARTMENT_CODE;
                    sdo.WorkingDepartmentName = room.DEPARTMENT_NAME;
                    sdo.WorkingRoomCode = room.ROOM_CODE;
                    sdo.WorkingRoomName = room.ROOM_NAME;

                    CommonParam param = new CommonParam();
                    var apiResult = new BackendAdapter(param).Post<bool>("api/LisSample/SyncEmr", ApiConsumer.ApiConsumers.LisConsumer, sdo, param);
                    MessageManager.Show(this.ParentForm, param, apiResult);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InGopBarcode(LisSampleADO row)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                LisSampleBarcodeCreateSDO lisSampleBarcodeCreateSdo = new LisSampleBarcodeCreateSDO();
                List<LisSampleADO> data = null;
                if (gridControlSample != null)
                {
                    data = (List<LisSampleADO>)gridControlSample.DataSource;
                }

                if (data == null || data.Count <= 0)
                {
                    return;
                }

                List<LisSampleADO> listCheck = data.Where(o => o.IsCheck).ToList();
                if (listCheck == null || listCheck.Count <= 0)
                {
                    listCheck.Add(row);
                }
                if (listCheck != null && listCheck.Count > 0)
                {
                    PrintProcess(PrintType.IN_GOP_BARCODE);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void CapNhatBarcode(LisSampleADO row)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                LisSampleBarcodeCreateSDO lisSampleBarcodeCreateSdo = new LisSampleBarcodeCreateSDO();
                List<LisSampleADO> data = null;
                if (gridControlSample != null)
                {
                    data = (List<LisSampleADO>)gridControlSample.DataSource;
                }

                if (data == null || data.Count <= 0)
                {
                    return;
                }

                List<LisSampleADO> listCheck = data.Where(o => o.IsCheck).ToList();
                if (listCheck == null || listCheck.Count <= 0)
                {
                    listCheck.Add(row);
                }
                if (listCheck != null && listCheck.Count > 0)
                {
                    if (listCheck.GroupBy(o => o.TREATMENT_CODE).Count() > 1)
                    {
                        XtraMessageBox.Show("Các y lệnh không thuộc 1 hồ sơ điều trị");
                        return;
                    }
                    List<LisSampleADO> listSample = listCheck.Where(o => o.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ || o.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ || o.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ).ToList();
                    if (listSample != null && listSample.Count > 0)
                    {
                        XtraMessageBox.Show(String.Format("Y lệnh {0} đã có kết quả", String.Join(", ", listSample.Select(o => o.SERVICE_REQ_CODE).ToList())));
                        return;
                    }

                    WaitingManager.Show();
                    lisSampleBarcodeCreateSdo.SampleIds = listCheck.Select(o => o.ID).ToList();
                    var result = new BackendAdapter(param).Post<List<LIS_SAMPLE>>("api/LisSample/BarcodeCreate", ApiConsumer.ApiConsumers.LisConsumer, lisSampleBarcodeCreateSdo, param);
                    if (result != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        gridViewSample.RefreshData();
                    }

                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CapNhatTinhTrangMau(LisSampleADO data)
        {
            try
            {
                frmUpdateCondition frm = new frmUpdateCondition(this.currentModule, data);
                frm.ShowDialog();
                FillDataToGridControl();
                gridViewSample.RefreshData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LichSuXetNghiemCuaBenhNha(LisSampleADO data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "LIS.Desktop.Plugins.TestHistory").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'LIS.Desktop.Plugins.TestHistory'");
                    if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'LIS.Desktop.Plugins.TestHistory' is not plugins");
                    List<object> listArgs = new List<object>();
                    listArgs.Add(data.PATIENT_CODE);
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

        private void GridLookUpEdit__Machine_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                var data = (TestLisResultADO)this.treeListSereServTein.GetDataRecordByNode(treeListSereServTein.FocusedNode);
                GridLookUpEdit grd = sender as GridLookUpEdit;

                if (data != null && grd != null && grd.EditValue != null)
                {
                    long machineId = (long)grd.EditValue;
                    var checkData = CheckConfigWarningMachine(machineId, data);
                    if (checkData != null)
                    {
                        if (HisConfigCFG.MaxServicePerDayWarningOption == "1")
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Máy {0} vượt quá số lượng dịch vụ xử lý tối đa trong ngày ({1}). Bạn có muốn tiếp tục?", checkData.MACHINE_NAME, checkData.MAX_SERVICE_PER_DAY), "Thông báo",
                              MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                            {
                                data.MACHINE_ID = null;
                                treeListSereServTein.RefreshNode(treeListSereServTein.FocusedNode);
                                return;
                            }
                        }
                        else if (HisConfigCFG.MaxServicePerDayWarningOption == "2")
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Máy {0} vượt quá số lượng dịch vụ xử lý tối đa trong ngày ({1})", checkData.MACHINE_NAME, checkData.MAX_SERVICE_PER_DAY), "Thông báo");
                            data.MACHINE_ID = null;
                            treeListSereServTein.RefreshNode(treeListSereServTein.FocusedNode);
                            return;
                        }

                    };
                    WaitingManager.Show();
                    bool success = false;
                    CommonParam param = new CommonParam();
                    LisSampleServiceMachineSDO sdo = new LisSampleServiceMachineSDO();
                    sdo.MachineId = machineId;
                    sdo.SampleServiceId = data.SAMPLE_SERVICE_ID.HasValue ? data.SAMPLE_SERVICE_ID.Value : 0;
                    var rs = new BackendAdapter(param).Post<LIS_SAMPLE_SERVICE>("api/LisSampleService/UpdateMachine", ApiConsumers.LisConsumer, sdo, param);
                    if (rs != null)
                    {
                        success = true;
                    }

                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HisMachineCounterSDO CheckConfigWarningMachine(long machineId, TestLisResultADO data)
        {
            HisMachineCounterSDO rs = null;
            try
            {
                var machineLis = this._Machines.FirstOrDefault(o => o.ID == machineId);
                if (machineLis != null
                    && (HisConfigCFG.PatientTypeOption != "1" || (HisConfigCFG.PatientTypeOption == "1" && data.PATIENT_TYPE_ID_BY_SERE_SERV == HisConfigCFG.PatientTypeId__BHYT)) && GlobalVariables.MachineCounterSdos != null && GlobalVariables.MachineCounterSdos.Count() > 0)
                {
                    var machineHis = GlobalVariables.MachineCounterSdos.FirstOrDefault(o => o.MACHINE_CODE == machineLis.MACHINE_CODE);
                    if (machineHis.TOTAL_PROCESSED_SERVICE_TEIN >= machineHis.MAX_SERVICE_PER_DAY)
                    {
                        rs = machineHis;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return rs;
        }

        private void btnKhongThucHien_Click(object sender, EventArgs e)
        {
            try
            {
                treeListSereServTein.PostEditor();
                btnKhongThucHien.Focus();

                List<TestLisResultADO> dataGrid = new List<TestLisResultADO>();
                List<TestLisResultADO> dataGridChild = new List<TestLisResultADO>();
                var AllCheckNodes = treeListSereServTein.GetAllCheckedNodes();
                foreach (var item in AllCheckNodes)
                {
                    var data = (TestLisResultADO)this.treeListSereServTein.GetDataRecordByNode(item);
                    dataGrid.Add(data);
                }

                var dataGridParent = dataGrid.Where(o => o.IS_PARENT == 1
                    && (!o.IS_NO_EXECUTE.HasValue || o.IS_NO_EXECUTE != 1)).ToList();
                if (dataGridParent == null || dataGridParent.Count() == 0)
                {
                    MessageManager.Show(String.Format("Bạn chưa chọn dịch vụ nào"));
                    return;
                }
                foreach (var item in dataGridParent)
                {
                    var ChildData = this.lstLisResultADOs.Where(o => o.PARENT_ID == item.CHILD_ID || (item.HAS_ONE_CHILD == 1 && o.CHILD_ID == item.CHILD_ID));
                    if (ChildData != null && ChildData.Count() > 0)
                        dataGridChild.AddRange(ChildData);
                }
                var childHasValue = dataGridChild.Where(o => !String.IsNullOrWhiteSpace(o.VALUE_RANGE));
                if ((childHasValue != null && childHasValue.Count() > 0))
                {
                    string message = String.Join("; ", childHasValue.Select(o => o.TEST_INDEX_NAME));
                    MessageManager.Show(String.Format("Các chỉ số đã có giá trị: {0}", message));
                    return;
                }

                if (MessageBox.Show(String.Format("Bạn có xác nhận dịch vụ {0} không thực hiện không?", String.Join("; ", dataGridParent.Select(o => o.SERVICE_NAME))), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    UpdateSampleServiceNoExecuteSDO updateSampleServiceNoExecuteSDO = new LIS.SDO.UpdateSampleServiceNoExecuteSDO();
                    updateSampleServiceNoExecuteSDO.ServiceCodes = new List<string>();
                    updateSampleServiceNoExecuteSDO.ServiceCodes.AddRange(dataGridParent.Select(o => o.SERVICE_CODE));
                    updateSampleServiceNoExecuteSDO.ServiceReqCode = rowSample.SERVICE_REQ_CODE;
                    updateSampleServiceNoExecuteSDO.IsNoExecute = true;

                    CommonParam param = new CommonParam();
                    bool success = false;
                    WaitingManager.Show();
                    var result = new BackendAdapter(param).Post<bool>("api/LisSampleService/UpdateNoExecute", ApiConsumers.LisConsumer, updateSampleServiceNoExecuteSDO, param);
                    if (result)
                    {
                        success = true;
                        RowClick();
                    }
                    WaitingManager.Hide();
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkPrintNow_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isInit)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.CHECK_PRINT_NOW && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (checkPrintNow.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.CHECK_PRINT_NOW;
                    csAddOrUpdate.VALUE = (checkPrintNow.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();

                if (checkPrintNow.Checked)
                {
                    chkPrintPreview.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkReturnResult_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isInit)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.CHECK_RETURN_RESULT && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkReturnResult.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.CHECK_RETURN_RESULT;
                    csAddOrUpdate.VALUE = (chkReturnResult.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkSign_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isInit)
                {
                    return;
                }
                if (chkSign.Checked)
                    chkSignProcess.Checked = !chkSign.Checked;

                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.CHECK_SIGN && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkSign.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.CHECK_SIGN;
                    csAddOrUpdate.VALUE = (chkSign.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGridControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessAutoSelectMachine(List<TestLisResultADO> listData)
        {
            try
            {
                if (listData == null || listData.Count <= 0 || !listData.Any(a => a.IS_PARENT == 1 && !a.MACHINE_ID.HasValue))
                {
                    return;
                }

                if (!chkWarn.Checked && !chkCon.Checked)
                {
                    return;
                }
                if (this._Machines == null || this._Machines.Count == 0)
                {
                    return;
                }
                List<TestLisResultADO> autoAdos = listData.Where(o => o.PARENT_ID == "." && !o.MACHINE_ID.HasValue).ToList();
                List<string> serviceCodes = autoAdos.Select(s => s.SERVICE_CODE).ToList();

                List<V_HIS_SERVICE> services = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => serviceCodes.Contains(o.SERVICE_CODE)).ToList();

                if (services == null || services.Count <= 0)
                {
                    LogSystem.Warn("KHong lay duoc V_HIS_SERVICE theo ServiceCodes: " + String.Join(", ", serviceCodes));
                    return;
                }

                List<HIS_MACHINE> hisMachines = BackendDataWorker.Get<HIS_MACHINE>();

                if (hisMachines == null || hisMachines.Count <= 0)
                {
                    LogSystem.Warn("KHong lay duoc HIS_MACHINE");
                    return;
                }

                List<HIS_SERVICE_MACHINE> serviceMachines = BackendDataWorker.Get<HIS_SERVICE_MACHINE>().Where(o => services.Any(a => a.ID == o.SERVICE_ID)).ToList();

                foreach (TestLisResultADO ado in autoAdos)
                {
                    V_HIS_SERVICE s = services.FirstOrDefault(o => o.SERVICE_CODE == ado.SERVICE_CODE);
                    if (s == null)
                    {
                        LogSystem.Warn("Khong lay duoc V_HIS_SERVICE theo ServiceCodes: " + ado.SERVICE_CODE);
                        continue;
                    }

                    List<HIS_SERVICE_MACHINE> sm = serviceMachines != null ? serviceMachines.Where(o => o.SERVICE_ID == s.ID).ToList() : null;
                    if (sm == null || sm.Count <= 0)
                    {
                        LogSystem.Debug("Khong co thong tin HIS_SERVICE_MACHINE voi serviceCode: " + s.SERVICE_CODE);
                        continue;
                    }

                    List<HIS_MACHINE> m = hisMachines.Where(o => sm.Any(a => a.MACHINE_ID == o.ID)).ToList();

                    List<LIS_MACHINE> selectes = this._Machines.Where(o => m != null && m.Any(a => a.MACHINE_CODE == o.MACHINE_CODE)).ToList();

                    if (selectes != null && selectes.Count == 1)
                    {
                        ado.MACHINE_ID = selectes[0].ID;
                        List<TestLisResultADO> childs = listData.Where(o => o.PARENT_ID == ado.CHILD_ID).ToList();
                        if (childs.Count > 0)
                        {
                            childs.ForEach(o => o.MACHINE_ID = ado.MACHINE_ID);
                        }
                    }
                    else
                    {
                        LogSystem.Debug("Selectes ListMachine is empty. ServiceCode: " + s.SERVICE_CODE);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItembtnValueRangeShow_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                TestLisResultADO testLisResultADO = new TestLisResultADO();
                var data = this.treeListSereServTein.GetDataRecordByNode(this.treeListSereServTein.FocusedNode);
                if (data != null && data is TestLisResultADO)
                {
                    testLisResultADO = (TestLisResultADO)data;
                }
                if (testLisResultADO.VALUE_RANGE != null && testLisResultADO.VALUE_RANGE != "")
                {
                    txtValueRangeIntoPopup.Text = testLisResultADO.VALUE_RANGE;
                }
                else
                {
                    txtValueRangeIntoPopup.Text = "";
                }
                ButtonEdit editor = sender as ButtonEdit;
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainerRangeValue.ShowPopup(new Point(buttonPosition.X + 532, buttonPosition.Bottom + 200));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItembtnNoteShow_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                TestLisResultADO testLisResultADO = new TestLisResultADO();
                var data = this.treeListSereServTein.GetDataRecordByNode(this.treeListSereServTein.FocusedNode);
                if (data != null && data is TestLisResultADO)
                {
                    testLisResultADO = (TestLisResultADO)data;
                }
                if (testLisResultADO.NOTE != null && testLisResultADO.NOTE != "")
                {
                    txtNoteIntoPopup.Text = testLisResultADO.NOTE;
                }
                else
                {
                    txtNoteIntoPopup.Text = "";
                }
                ButtonEdit editor = sender as ButtonEdit;
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainerNote.ShowPopup(new Point(buttonPosition.X + 532, buttonPosition.Bottom + 200));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnOKForValueRange_Click(object sender, EventArgs e)
        {
            try
            {
                TestLisResultADO testLisResultADO = new TestLisResultADO();
                var data = this.treeListSereServTein.GetDataRecordByNode(this.treeListSereServTein.FocusedNode);
                if (data != null && data is TestLisResultADO)
                {
                    testLisResultADO = (TestLisResultADO)data;
                }

                if (txtValueRangeIntoPopup.Text != null && txtValueRangeIntoPopup.Text != "")
                {
                    testLisResultADO.VALUE_RANGE = txtValueRangeIntoPopup.Text;
                }
                else
                {
                    testLisResultADO.VALUE_RANGE = null;
                }

                treeListSereServTein.RefreshDataSource();
                popupControlContainerRangeValue.HidePopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnOKForNote_Click(object sender, EventArgs e)
        {
            try
            {
                TestLisResultADO testLisResultADO = new TestLisResultADO();
                var data = this.treeListSereServTein.GetDataRecordByNode(this.treeListSereServTein.FocusedNode);
                if (data != null && data is TestLisResultADO)
                {
                    testLisResultADO = (TestLisResultADO)data;
                }
                if (txtNoteIntoPopup.Text != null && txtNoteIntoPopup.Text != "")
                {
                    testLisResultADO.NOTE = txtNoteIntoPopup.Text;
                }
                else
                {
                    testLisResultADO.NOTE = null;
                }
                treeListSereServTein.RefreshDataSource();
                popupControlContainerNote.HidePopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCancelForValueRange_Click(object sender, EventArgs e)
        {
            txtValueRangeIntoPopup.Text = "";
            popupControlContainerRangeValue.HidePopup();
        }

        private void btnCancelForNote_Click(object sender, EventArgs e)
        {
            txtNoteIntoPopup.Text = "";
            popupControlContainerNote.HidePopup();
        }

        private void treeListSereServTein_CustomDrawColumnHeader(object sender, CustomDrawColumnHeaderEventArgs e)
        {
            try
            {
                if (e.Column != null && e.Column.VisibleIndex == 0)
                {
                    Rectangle checkRect = new Rectangle(e.Bounds.Left + 3, e.Bounds.Top + 3, 12, 12);
                    DevExpress.XtraTreeList.ViewInfo.ColumnInfo info = (DevExpress.XtraTreeList.ViewInfo.ColumnInfo)e.ObjectArgs;
                    if (info.CaptionRect.Left < 30)
                        info.CaptionRect = new Rectangle(new Point(info.CaptionRect.Left + 15, info.CaptionRect.Top), info.CaptionRect.Size);
                    e.Painter.DrawObject(info);

                    DrawCheckBox(e.Cache, repositoryItemCheckAll, checkRect, IsAllSelected(sender as TreeList));
                    e.Handled = true;
                }
                else if (e.Column != null && e.Column.Name == treeListColumn_ReRun.Name)
                {
                    Rectangle checkRect = new Rectangle(e.Bounds.Left + 4, e.Bounds.Top + 4, 12, 12);
                    DevExpress.XtraTreeList.ViewInfo.ColumnInfo info = (DevExpress.XtraTreeList.ViewInfo.ColumnInfo)e.ObjectArgs;
                    if (info.CaptionRect.Left < 30)
                        info.CaptionRect = new Rectangle(new Point(info.CaptionRect.Left + 15, info.CaptionRect.Top), info.CaptionRect.Size);
                    e.Painter.DrawObject(info);

                    DrawCheckBox(e.Cache, repositoryItemCheckAll, checkRect, IsAllSelectedReRun(sender as TreeList));
                    e.Handled = true;
                }
                else
                {
                    TreeListColumn nextColumn = treeListColumn1;
                    if (nextColumn == null) return;
                    if (e.Column == nextColumn) { e.Handled = true; return; }
                    if (e.Column != grdColVallue) return;
                    Rectangle r = e.ObjectArgs.Bounds;
                    r.Width = r.Width + nextColumn.VisibleWidth;
                    e.ObjectArgs.Bounds = r;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void TreeListHeaderMerger(TreeList treeList, TreeListColumn columnToMerge)
        {
            try
            {
                this.treeList = treeList;
                this.columnToMerge = columnToMerge;
                columnToMerge.OptionsColumn.AllowMove = false;
                TreeListColumn col = GetNextColumn();
                if (col.OptionsColumn != null) GetNextColumn().OptionsColumn.AllowMove = false;
                treeList.CustomDrawColumnHeader += treeList_CustomDrawColumnHeader;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public TreeListColumn GetNextColumn()
        {
            try
            {
                if (columnToMerge.VisibleIndex == treeList.VisibleColumns.Count - 1) return null;
                return treeList.VisibleColumns[columnToMerge.VisibleIndex + 1];
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        public void treeList_CustomDrawColumnHeader(object sender, CustomDrawColumnHeaderEventArgs e)
        {
            try
            {
                TreeListColumn nextColumn = GetNextColumn();
                if (nextColumn == null) return;
                if (e.Column == nextColumn) { e.Handled = true; return; }
                if (e.Column != columnToMerge) return;
                Rectangle r = e.ObjectArgs.Bounds;
                r.Width = r.Width + nextColumn.VisibleWidth;
                e.ObjectArgs.Bounds = r;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItembtnOldValueShow_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                TestLisResultADO testLisResultADO = new TestLisResultADO();
                var data = this.treeListSereServTein.GetDataRecordByNode(this.treeListSereServTein.FocusedNode);
                if (data != null && data is TestLisResultADO)
                {
                    testLisResultADO = (TestLisResultADO)data;
                }
                if (testLisResultADO.OLD_VALUE != null && testLisResultADO.OLD_VALUE != "")
                {
                    txtOldValueIntoPopup.Text = testLisResultADO.OLD_VALUE;
                }
                else
                {
                    txtOldValueIntoPopup.Text = "";
                }
                ButtonEdit editor = sender as ButtonEdit;
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainerOldValue.ShowPopup(new Point(buttonPosition.X + 532, buttonPosition.Bottom + 200));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProviderEditorInfo_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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

        private void xtraTabDocument_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                if (rowSample != null && !String.IsNullOrWhiteSpace(rowSample.SERVICE_REQ_CODE))
                {
                    Inventec.Common.Logging.LogSystem.Info("xtraTabDocument_TabIndexChanged");
                    ProcessLoadDocumentBySereServ(rowSample);
                }
                else
                {
                    this.ucViewEmrDocumentReq.ReloadDocument(new List<V_EMR_DOCUMENT>());
                    this.ucViewEmrDocumentResult.ReloadDocument(new List<V_EMR_DOCUMENT>());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessLoadDocumentBySereServ(LisSampleADO data)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("ProcessLoadDocumentBySereServ");
                WaitingManager.Show();
                List<V_EMR_DOCUMENT> listData = new List<V_EMR_DOCUMENT>();
                if (data != null)
                {
                    string hisCode = "SERVICE_REQ_CODE:" + data.SERVICE_REQ_CODE;
                    CommonParam paramCommon = new CommonParam();
                    var emrFilter = new EMR.Filter.EmrDocumentViewFilter();
                    emrFilter.TREATMENT_CODE__EXACT = data.TREATMENT_CODE;
                    emrFilter.IS_DELETE = false;
                    if (xtraTabDocument.SelectedTabPage == xtraTabDocumentReq)
                    {
                        emrFilter.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_ASSIGN;
                    }
                    else if (xtraTabDocument.SelectedTabPage == xtraTabDocumentResult)
                    {
                        emrFilter.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_RESULT;
                    }

                    var documents = new BackendAdapter(paramCommon).Get<List<V_EMR_DOCUMENT>>("api/EmrDocument/GetView", ApiConsumers.EmrConsumer, emrFilter, paramCommon);
                    if (documents != null && documents.Count > 0)
                    {
                        var serviceDoc = documents.Where(o => !String.IsNullOrWhiteSpace(o.HIS_CODE) && o.HIS_CODE.Contains(hisCode)).ToList();
                        if (serviceDoc != null && serviceDoc.Count > 0)
                        {
                            listData.AddRange(serviceDoc);
                        }
                    }
                }

                if (xtraTabDocument.SelectedTabPage == xtraTabDocumentReq)
                {
                    this.ucViewEmrDocumentReq.ReloadDocument(listData);
                }
                else if (xtraTabDocument.SelectedTabPage == xtraTabDocumentResult)
                {
                    this.ucViewEmrDocumentResult.ReloadDocument(listData);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void layoutControl3_GroupExpandChanged(object sender, DevExpress.XtraLayout.Utils.LayoutGroupEventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                string name = e.Group.Name;
                string value = "";

                if (e.Group.Name == LciGroupEmrDocument.Name)
                {
                    value = LciGroupEmrDocument.Expanded ? "1" : null;
                }

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == name && o.MODULE_LINK == ModuleLinkName).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = value;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = name;
                    csAddOrUpdate.VALUE = value;
                    csAddOrUpdate.MODULE_LINK = ModuleLinkName;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }

                if (this.controlStateWorker != null)
                    this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ViewSample_Click(LisSampleADO data)
        {
            try
            {
                if (data != null && !String.IsNullOrWhiteSpace(rowSample.SERVICE_REQ_CODE))
                {
                    ProcessLoadDocumentBySereServ(data);
                }
                else
                {
                    this.ucViewEmrDocumentReq.ReloadDocument(new List<V_EMR_DOCUMENT>());
                    this.ucViewEmrDocumentResult.ReloadDocument(new List<V_EMR_DOCUMENT>());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeListSereServTein_CustomUnboundColumnData(object sender, TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                TestLisResultADO data = (TestLisResultADO)this.treeListSereServTein.GetDataRecordByNode(e.Node);
                if (e.Column.FieldName == "VALUE_RANGE_DISPLAY" && data != null)
                {
                    if (HisConfigCFG.IS_SHOWING_RESULT_GENERAL == "1")
                    {
                        if (data.IS_PARENT == 1 && data.HAS_ONE_CHILD == 0)
                        {
                            e.Value = data.SERVICE_RESULT_ID;
                        }
                        else
                        {
                            e.Value = data.VALUE_RANGE;
                        }
                    }
                    else
                    {
                        e.Value = data.VALUE_RANGE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemGridLookUp_ServiceResult_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                var data = (TestLisResultADO)this.treeListSereServTein.GetDataRecordByNode(treeListSereServTein.FocusedNode);
                GridLookUpEdit grd = sender as GridLookUpEdit;

                if (data != null && grd != null)
                {
                    long? serviceResultId = null;
                    if (grd.EditValue != null)
                    {
                        serviceResultId = (long)grd.EditValue;
                    }

                    Inventec.Common.Logging.LogSystem.Debug("repositoryItemGridLookUp_ServiceResult serviceResultId: " + serviceResultId);
                    WaitingManager.Show();
                    bool success = false;
                    CommonParam param = new CommonParam();
                    UpdateServiceResultSDO sdo = new UpdateServiceResultSDO();
                    sdo.ServiceResultId = serviceResultId;
                    sdo.SampleServiceId = data.SAMPLE_SERVICE_ID.HasValue ? data.SAMPLE_SERVICE_ID.Value : 0;
                    var rs = new BackendAdapter(param).Post<V_LIS_SAMPLE>("api/LisSampleService/UpdateServiceResult", ApiConsumers.LisConsumer, sdo, param);
                    if (rs != null)
                    {
                        success = true;
                        if (rowSample != null)
                        {
                            rowSample.SERVICE_RESULT_CODE = rs.SERVICE_RESULT_CODE;
                            rowSample.SERVICE_RESULT_ID = rs.SERVICE_RESULT_ID;
                            rowSample.SERVICE_RESULT_NAME = rs.SERVICE_RESULT_NAME;
                        }
                        data.SERVICE_RESULT_ID = serviceResultId;
                        this.treeListSereServTein.RefreshDataSource();
                        this.gridControlSample.RefreshDataSource();
                    }

                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemGridLookUp_ServiceResult_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    GridLookUpEdit grd = sender as GridLookUpEdit;
                    grd.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TextEditValueRange__Enable_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                LogSystem.Debug("TextEditValueRange__Enable_EditValueChanged.1");
                TestLisResultADO testLisResultADO = new TestLisResultADO();
                var data = this.treeListSereServTein.GetDataRecordByNode(this.treeListSereServTein.FocusedNode);
                if (data != null && data is TestLisResultADO)
                {
                    LogSystem.Debug("TextEditValueRange__Enable_EditValueChanged.1.1");
                    TextEdit txt = sender as TextEdit;

                    testLisResultADO = (TestLisResultADO)data;

                    if (txt.Text != null && txt.Text != "")
                    {
                        testLisResultADO.VALUE_RANGE = txt.Text;
                    }
                    else
                    {
                        testLisResultADO.VALUE_RANGE = null;
                    }
                }

                LogSystem.Debug("TextEditValueRange__Enable_EditValueChanged.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkOrderByBarcode_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isInit)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.CHECK_ORDER_BY_BARCODE && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkOrderByBarcode.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.CHECK_ORDER_BY_BARCODE;
                    csAddOrUpdate.VALUE = (chkOrderByBarcode.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                FillDataToGridControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkShowSampleGroup_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isInit)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.CHECK_SHOW_SAMPLE_GROUP && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkShowSampleGroup.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.CHECK_SHOW_SAMPLE_GROUP;
                    csAddOrUpdate.VALUE = (chkShowSampleGroup.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                FillDataToGridControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSample_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column != null && e.Column.Name == gc_CheckApprovalList.Name)
                {
                    Rectangle checkRect = new Rectangle(e.Bounds.Left + 3, e.Bounds.Top + 3, 12, 12);
                    if (e.Info.CaptionRect.Left < 30)
                        e.Info.CaptionRect = new Rectangle(new Point(e.Info.CaptionRect.Left + 15, e.Info.CaptionRect.Top), e.Info.CaptionRect.Size);
                    e.Painter.DrawObject(e.Info);

                    DrawCheckBox(e.Cache, repositoryItemCheckAll, checkRect, IsAllSelected(sender as GridView));
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSample_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                GridView grid = sender as GridView;
                Point pt = new Point(e.X, e.Y);
                GridHitInfo hit = grid.CalcHitInfo(pt);
                if (hit.InColumn && hit.Column != null && hit.Column.Name == gc_CheckApprovalList.Name)
                {
                    EmbeddedCheckBoxChecked(grid);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCreateSigner_Click(object sender, EventArgs e)
        {
            try
            {
                if (SignConfigData == null)
                {
                    SignConfigData = new SignConfigADO();
                }

                HIS.Desktop.Plugins.ConnectionTest.AddSigner.frmSignerAdd frmAddSigner = new HIS.Desktop.Plugins.ConnectionTest.AddSigner.frmSignerAdd(SignConfigData.listSign, UpdateAfterAddSignThread, SignConfigData.IsSignParanel);
                frmAddSigner.ShowDialog(this.ParentForm);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateAfterAddSignThread(List<SignTDO> signs, bool signParanel)
        {
            try
            {
                //if (signs != null && signs.Count > 0)
                //{
                if (SignConfigData == null)
                {
                    SignConfigData = new SignConfigADO();
                }

                SignConfigData.listSign = signs;
                SignConfigData.IsSignParanel = signParanel;

                string value = Newtonsoft.Json.JsonConvert.SerializeObject(SignConfigData);
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == btnApproveListResult.Name && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = value;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = btnApproveListResult.Name;
                    csAddOrUpdate.VALUE = value;
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkSignApproveList_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isInit)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkSignApproveList.Name && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkSignApproveList.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkSignApproveList.Name;
                    csAddOrUpdate.VALUE = (chkSignApproveList.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                FillDataToGridControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnApproveListResult_Click(object sender, EventArgs e)
        {
            try
            {
                dicSignApproveList = new Dictionary<string, string>();
                List<LisSampleADO> data = null;
                if (gridControlSample != null)
                {
                    data = (List<LisSampleADO>)gridControlSample.DataSource;
                }

                if (data == null || data.Count <= 0)
                {
                    return;
                }

                List<LisSampleADO> listCheck = data.Where(o => o.IsCheck).ToList();
                if (listCheck == null || listCheck.Count <= 0)
                {
                    XtraMessageBox.Show("Bạn chưa chọn mẫu bệnh phẩm nào");
                    return;
                }

                if (!listCheck.Exists(o => o.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ))
                {
                    XtraMessageBox.Show("Bạn chưa chọn mẫu bệnh phẩm đã có kết quả nào");
                    return;
                }

                if (listCheck.Exists(o => o.SAMPLE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ))
                {
                    XtraMessageBox.Show("Chỉ cho phép duyệt với các mẫu đã có kết quả");
                    return;
                }

                if (!CheckSignApproveList())
                {
                    return;
                }
                EnabledLableApproveList(true);
                this.lblApproveResultError.Text = "";
                this.lblApproveResultSuccess.Text = "0/" + listCheck.Count;
                int success = 0;
                int error = 0;
                ApproveListError = new List<string>();
                WaitingManager.Show();
                foreach (var item in listCheck)
                {
                    CommonParam param = new CommonParam();
                    LIS_SAMPLE curentSTT = null;
                    frmApproveResult frm = new frmApproveResult(item, (result) =>
                    {
                        curentSTT = result;
                    });
                    frm.ShowDialog();
                    if (curentSTT != null)
                    {
                        string errorMessage = "";
                        if (chkSignApproveList.Checked)//có ký và ký thất bại
                        {
                            if (curentSTT.IS_SENT_EMR != 1)
                            {
                                error += 1;
                                ApproveListError.Add(string.Format("Mẫu XN có mã {0} chưa khởi tạo hồ sơ EMR", item.BARCODE));
                            }
                            else if (ProcessSign(curentSTT, ref errorMessage))
                            {
                                error += 1;
                                ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", item.BARCODE, errorMessage));
                            }
                            else
                            {
                                success += 1;
                            }
                        }
                        else
                        {
                            success += 1;
                        }
                    }
                    else
                    {
                        ApproveListError.Add(string.Format("Mẫu XN có mã {0} duyệt thất bại. {1}", item.BARCODE, param.GetMessage()));
                        error += 1;
                    }

                    if (error > 0)
                    {
                        lciBtnApproveError.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    }

                    this.lblApproveResultError.Text = error.ToString();
                    this.lblApproveResultSuccess.Text = string.Format("{0}/{1}", success, listCheck.Count);
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void CreateThreadLoadData(HIS_SERVICE_REQ param)
        {
            Thread tSereServ = new Thread(new ParameterizedThreadStart(GetSereServ));
            Thread tTreatmentBedRoom = new Thread(new ParameterizedThreadStart(GetTreatmentBedRoom));
            Thread tSereServTein = new Thread(new ParameterizedThreadStart(GetSereServTein));
            Thread tHisDhst = new Thread(new ParameterizedThreadStart(GetHisDhst));
            try
            {
                tSereServ.Start(param);
                tTreatmentBedRoom.Start(param);
                tSereServTein.Start(param);
                tHisDhst.Start(param);
                tSereServ.Join();
                tTreatmentBedRoom.Join();
                tSereServTein.Join();
                tHisDhst.Join();
            }
            catch (Exception ex)
            {
                tSereServ.Abort();
                tTreatmentBedRoom.Abort();
                tSereServTein.Abort();
                tHisDhst.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        List<V_HIS_TREATMENT_BED_ROOM> TreatmentBedRoomList = new List<V_HIS_TREATMENT_BED_ROOM>();
        private void GetTreatmentBedRoom(object obj)
        {
            try
            {
                HIS_SERVICE_REQ data = (HIS_SERVICE_REQ)obj;
                HisTreatmentBedRoomViewFilter filter = new HisTreatmentBedRoomViewFilter();
                filter.TREATMENT_ID = data.TREATMENT_ID;
                TreatmentBedRoomList = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetView", ApiConsumers.MosConsumer, filter, null);
                if (TreatmentBedRoomList != null && TreatmentBedRoomList.Count > 0)
                {
                    TreatmentBedRoomList = TreatmentBedRoomList.Where(o => o.ADD_TIME <= data.INTRUCTION_TIME && o.ROOM_ID == data.REQUEST_ROOM_ID).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        List<HIS_SERE_SERV> SereServList = new List<HIS_SERE_SERV>();
        List<V_HIS_SERE_SERV_TEIN_1> lstSereServTein1 = new List<V_HIS_SERE_SERV_TEIN_1>();
        List<HIS_DHST> hisdhst = new List<HIS_DHST>();
        private void GetSereServ(object obj)
        {
            try
            {
                HIS_SERVICE_REQ data = (HIS_SERVICE_REQ)obj;
                HisSereServFilter filter = new HisSereServFilter();
                filter.TDL_SERVICE_REQ_CODE_EXACT = data.SERVICE_REQ_CODE;
                SereServList = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, filter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void GetSereServTein(object obj)
        {
            try
            {
                lstSereServTein1.Clear();
                HIS_SERVICE_REQ data = (HIS_SERVICE_REQ)obj;
                var TestIndexDataMLCT = BackendDataWorker.Get<HIS_TEST_INDEX>().Where(o => o.IS_TO_CALCULATE_EGFR == 1 || o.TEST_INDEX_TYPE.HasValue).ToList();
                if (TestIndexDataMLCT != null && TestIndexDataMLCT.Count > 0)
                {

                    if (TestIndexDataMLCT != null && TestIndexDataMLCT.Count > 0)
                    {
                        CommonParam paramv1 = new CommonParam();
                        HisSereServTeinView1Filter view1filter = new HisSereServTeinView1Filter();
                        view1filter.TREATMENT_IDs = new List<long> { data.TREATMENT_ID };
                        view1filter.TEST_INDEX_IDs = TestIndexDataMLCT.Select(s => s.ID).ToList();
                        lstSereServTein1 = new BackendAdapter(paramv1).Get<List<V_HIS_SERE_SERV_TEIN_1>>("/api/HisSereServTein/GetView1", ApiConsumers.MosConsumer, view1filter, paramv1);
                        if (lstSereServTein1 != null && lstSereServTein1.Count > 0)
                        {
                            lstSereServTein1 = lstSereServTein1.Where(o => o.TDL_INTRUCTION_TIME <= data.INTRUCTION_TIME).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void GetHisDhst(object obj)
        {
            try
            {
                hisdhst.Clear();
                HIS_SERVICE_REQ data = (HIS_SERVICE_REQ)obj;
                MOS.Filter.HisDhstFilter DhstFilter = new HisDhstFilter();
                DhstFilter.TREATMENT_ID = data.TREATMENT_ID;
                hisdhst = new BackendAdapter(new CommonParam()).Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumer.ApiConsumers.MosConsumer, DhstFilter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        /// <summary>
        /// xử lý in từng dòng
        /// gọi thư viện in xử lý lưu ra memorystream
        /// truyền memorystream vào thư viện ký
        /// </summary>
        /// <param name="curentSTT">mẫu</param>
        /// <param name="errorMessage">thông báo lỗi</param>
        /// <returns>true: có lỗi</returns>
        private bool ProcessSign(LIS_SAMPLE curentSTT, ref string errorMessage)
        {
            bool result = true;
            try
            {
                if (curentSTT == null)
                {
                    errorMessage = "Lỗi mẫu XN";
                    return result;
                }

                V_LIS_SAMPLE samplePrint = new V_LIS_SAMPLE();
                List<V_LIS_RESULT> lstResultPrint = new List<V_LIS_RESULT>();
                List<V_HIS_TEST_INDEX> currentTestIndexs = new List<V_HIS_TEST_INDEX>();
                long genderId = curentSTT.GENDER_CODE == "01" ? 1 : 2;
                CommonParam param = new CommonParam();
                LisSampleViewFilter sampleFilter = new LisSampleViewFilter();
                sampleFilter.ID = curentSTT.ID;
                var apiResult = new BackendAdapter(param).Get<List<V_LIS_SAMPLE>>("api/LisSample/GetView", ApiConsumers.LisConsumer, sampleFilter, param);
                if (apiResult != null && apiResult.Count > 0)
                {
                    samplePrint = apiResult.FirstOrDefault();
                    HisServiceReqFilter filter = new HisServiceReqFilter();
                    filter.SERVICE_REQ_CODE__EXACT = samplePrint.SERVICE_REQ_CODE;
                    var GetServiceReq = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, param);
                    if (GetServiceReq != null && GetServiceReq.Count > 0)
                    {
                        CreateThreadLoadData(GetServiceReq.FirstOrDefault());
                    }
                }
                else
                {
                    errorMessage = "Không có thông tin mẫu XN";
                    return result;
                }

                LisResultViewFilter resultFilter = new LisResultViewFilter();
                resultFilter.SAMPLE_ID = curentSTT.ID;
                var apiLisResutl = new BackendAdapter(param).Get<List<V_LIS_RESULT>>("api/LisResult/GetView", ApiConsumers.LisConsumer, resultFilter, param);
                if (apiLisResutl != null && apiLisResutl.Count > 0)
                {
                    lstResultPrint.AddRange(apiLisResutl);
                }

                var testIndex = BackendDataWorker.Get<V_HIS_TEST_INDEX>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                var serviceCodes = lstResultPrint.Select(o => o.SERVICE_CODE).Distinct().ToList();
                currentTestIndexs = testIndex.Where(o => serviceCodes.Contains(o.SERVICE_CODE)).ToList();
                List<LIS_SAMPLE_TYPE> listSampleType = new List<LIS_SAMPLE_TYPE>();
                List<HIS_TEST_SAMPLE_TYPE> listTestSampleType = new List<HIS_TEST_SAMPLE_TYPE>();
                if ((PacsCFG.MosLisInterGrationVersion == "1" && PacsCFG.MosLisInterGrationOption == "1") || (PacsCFG.MosLisInterGrationVersion == "2" && PacsCFG.MosLisInterGrationType == "1"))
                {
                    listSampleType = BackendDataWorker.Get<LIS_SAMPLE_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                }
                else
                {
                    listTestSampleType = BackendDataWorker.Get<HIS_TEST_SAMPLE_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                }
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = null;
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (!HisConfigCFG.PRINT_TEST_RESULT)
                {
                    MPS.Processor.Mps000096.PDO.Mps000096PDO mps000096RDO = new MPS.Processor.Mps000096.PDO.Mps000096PDO(
                         null,
                         null,
                         samplePrint,
                         null,
                         currentTestIndexs,
                         lstResultPrint,
                         BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                         genderId,
                         BackendDataWorker.Get<V_HIS_SERVICE>(),
                         null,
                         TreatmentBedRoomList != null && TreatmentBedRoomList.Count > 0 ? TreatmentBedRoomList.FirstOrDefault() : null,
                         SereServList,
                         listSampleType,
                         listTestSampleType);
                    inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(curentSTT.TREATMENT_CODE, MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096, this.currentModule.RoomId);
                    PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096, dicSignApproveList[MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096], mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                    SetUpSign(inputADO, PrintData, curentSTT, ref result, ref errorMessage);
                }
                else
                {
                    List<V_LIS_RESULT> lstResultHH = new List<V_LIS_RESULT>();
                    List<V_LIS_RESULT> lstResultVS = new List<V_LIS_RESULT>();
                    List<V_LIS_RESULT> lstResultMD = new List<V_LIS_RESULT>();
                    List<V_LIS_RESULT> lstResultSH = new List<V_LIS_RESULT>();
                    List<V_LIS_RESULT> lstResultPrintTemp = new List<V_LIS_RESULT>();
                    foreach (var item in lstResultPrint)
                    {
                        var check = lstHisService.FirstOrDefault(o => o.SERVICE_CODE == item.SERVICE_CODE);
                        if (check != null)
                        {
                            if (check.TEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__HH)
                            {
                                lstResultHH.Add(item);
                            }
                            else if (check.TEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__VS)
                            {
                                lstResultVS.Add(item);
                            }
                            else if (check.TEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__MD)
                            {
                                lstResultMD.Add(item);
                            }
                            else if (check.TEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__SH)
                            {
                                lstResultSH.Add(item);
                            }
                            else
                            {
                                lstResultPrintTemp.Add(item);
                            }
                        }

                    }
                    if (lstResultPrintTemp != null && lstResultPrintTemp.Count > 0)
                    {
                        MPS.Processor.Mps000096.PDO.Mps000096PDO mps000096RDO = new MPS.Processor.Mps000096.PDO.Mps000096PDO(
                        null,
                        null,
                        samplePrint,
                        null,
                        currentTestIndexs.Where(o => lstResultPrintTemp.Select(p => p.SERVICE_CODE).Distinct().ToList().Contains(o.SERVICE_CODE)).ToList(),
                        lstResultPrintTemp,
                        BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                        genderId,
                        BackendDataWorker.Get<V_HIS_SERVICE>(),
                         null,
                         TreatmentBedRoomList != null && TreatmentBedRoomList.Count > 0 ? TreatmentBedRoomList.FirstOrDefault() : null,
                         SereServList,
                         listSampleType,
                         listTestSampleType);
                        inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(curentSTT.TREATMENT_CODE, MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096, this.currentModule.RoomId);
                        PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096, dicSignApproveList[MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096], mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                        SetUpSign(inputADO, PrintData, curentSTT, ref result, ref errorMessage);
                    }
                    if (lstResultHH != null && lstResultHH.Count > 0)
                    {
                        MPS.Processor.Mps000456.PDO.Mps000456PDO mps000456RDO = new MPS.Processor.Mps000456.PDO.Mps000456PDO(
                        null,
                        null,
                        samplePrint,
                        null,
                        currentTestIndexs.Where(o => lstResultHH.Select(p => p.SERVICE_CODE).Distinct().ToList().Contains(o.SERVICE_CODE)).ToList(),
                        lstResultHH,
                        BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                        genderId,
                        BackendDataWorker.Get<V_HIS_SERVICE>());
                        inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(curentSTT.TREATMENT_CODE, MPS.Processor.Mps000456.PDO.PrintTypeCode.Mps000456, this.currentModule.RoomId);
                        PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000456.PDO.PrintTypeCode.Mps000456, dicSignApproveList[MPS.Processor.Mps000456.PDO.PrintTypeCode.Mps000456], mps000456RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                        SetUpSign(inputADO, PrintData, curentSTT, ref result, ref errorMessage);

                    }
                    if (lstResultVS != null && lstResultVS.Count > 0)
                    {
                        MPS.Processor.Mps000457.PDO.Mps000457PDO mps000457RDO = new MPS.Processor.Mps000457.PDO.Mps000457PDO(
                        null,
                        null,
                        samplePrint,
                        null,
                        currentTestIndexs.Where(o => lstResultVS.Select(p => p.SERVICE_CODE).Distinct().ToList().Contains(o.SERVICE_CODE)).ToList(),
                        lstResultVS,
                        BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                        genderId,
                        BackendDataWorker.Get<V_HIS_SERVICE>());
                        inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(curentSTT.TREATMENT_CODE, MPS.Processor.Mps000457.PDO.PrintTypeCode.Mps000457, this.currentModule.RoomId);
                        PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000457.PDO.PrintTypeCode.Mps000457, dicSignApproveList[MPS.Processor.Mps000457.PDO.PrintTypeCode.Mps000457], mps000457RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                        SetUpSign(inputADO, PrintData, curentSTT, ref result, ref errorMessage);
                    }
                    if (lstResultMD != null && lstResultMD.Count > 0)
                    {
                        MPS.Processor.Mps000458.PDO.Mps000458PDO mps000458RDO = new MPS.Processor.Mps000458.PDO.Mps000458PDO(
                       null,
                       null,
                       samplePrint,
                       null,
                       currentTestIndexs.Where(o => lstResultVS.Select(p => p.SERVICE_CODE).Distinct().ToList().Contains(o.SERVICE_CODE)).ToList(),
                       lstResultVS,
                       BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                       genderId,
                       BackendDataWorker.Get<V_HIS_SERVICE>());
                        inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(curentSTT.TREATMENT_CODE, MPS.Processor.Mps000458.PDO.PrintTypeCode.Mps000458, this.currentModule.RoomId);
                        PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000458.PDO.PrintTypeCode.Mps000458, dicSignApproveList[MPS.Processor.Mps000458.PDO.PrintTypeCode.Mps000458], mps000458RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                        SetUpSign(inputADO, PrintData, curentSTT, ref result, ref errorMessage);
                    }
                    if (lstResultSH != null && lstResultSH.Count > 0)
                    {
                        MPS.Processor.Mps000459.PDO.Mps000459PDO mps000459RDO = new MPS.Processor.Mps000459.PDO.Mps000459PDO(
                      null,
                      null,
                      samplePrint,
                      null,
                      currentTestIndexs.Where(o => lstResultSH.Select(p => p.SERVICE_CODE).Distinct().ToList().Contains(o.SERVICE_CODE)).ToList(),
                      lstResultSH,
                      BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                      genderId,
                      BackendDataWorker.Get<V_HIS_SERVICE>());
                        inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(curentSTT.TREATMENT_CODE, MPS.Processor.Mps000459.PDO.PrintTypeCode.Mps000459, this.currentModule.RoomId);
                        PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000459.PDO.PrintTypeCode.Mps000459, dicSignApproveList[MPS.Processor.Mps000459.PDO.PrintTypeCode.Mps000459], mps000459RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");

                        SetUpSign(inputADO, PrintData, curentSTT, ref result, ref errorMessage);
                    }

                }


            }
            catch (Exception ex)
            {
                result = true;
                errorMessage = ex.Message;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void SetUpSign(Inventec.Common.SignLibrary.ADO.InputADO inputADO, MPS.ProcessorBase.Core.PrintData PrintData, LIS_SAMPLE curentSTT, ref bool result, ref string errorMessage)
        {
            try
            {
                using (PrintData.saveMemoryStream = new MemoryStream())
                {
                    PrintData.EmrInputADO = inputADO;
                    MPS.MpsPrinter.Run(PrintData);

                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => PrintData), PrintData));
                    //sau hàm Run sẽ có thêm thông tin Inventec.Common.SignLibrary.ADO.InputADO trong PrintData
                    Inventec.Common.SignLibrary.ADO.InputADO signEmrInputADO = (Inventec.Common.SignLibrary.ADO.InputADO)PrintData.EmrInputADO;
                    string base64File = "";
                    using (MemoryStream pdfStream = new MemoryStream())
                    {
                        PrintData.saveMemoryStream.Position = 0;
                        ConvertExcelToPdfByFlexCel(PrintData.saveMemoryStream, pdfStream);
                        pdfStream.Position = 0;
                        base64File = System.Convert.ToBase64String(Inventec.Common.SignLibrary.Utils.StreamToByte(pdfStream));
                    }
                    signEmrInputADO.IsSign = true;
                    if (this.SignConfigData != null)
                    {
                        //signEmrInputADO.IsMultiSign = this.SignConfigData.IsSignParanel;
                        if (SignConfigData.listSign != null && SignConfigData.listSign.Count > 0)
                        {
                            signEmrInputADO.IsSignConfig = true;
                            signEmrInputADO.SignerConfigs = new List<Inventec.Common.SignLibrary.DTO.SignerConfigDTO>();
                            bool isMe = false;
                            if (!SignConfigData.listSign.Exists(o => o.Loginname == "%me%"))
                            {
                                Inventec.Common.SignLibrary.DTO.SignerConfigDTO dto = new Inventec.Common.SignLibrary.DTO.SignerConfigDTO();
                                isMe = true;
                                dto.NumOrder = 1;
                                dto.Loginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                                signEmrInputADO.SignerConfigs.Add(dto);
                            }
                            foreach (var item in SignConfigData.listSign)
                            {
                                Inventec.Common.SignLibrary.DTO.SignerConfigDTO dto = new Inventec.Common.SignLibrary.DTO.SignerConfigDTO();
                                dto.NumOrder = isMe ? item.NumOrder + 1 : item.NumOrder;
                                dto.Loginname = item.Loginname;
                                signEmrInputADO.SignerConfigs.Add(dto);
                            }
                        }
                    }

                    SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("base64File:___", base64File));
                    // Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("signEmrInputADO:___", signEmrInputADO));


                    var signNowResult = libraryProcessor.SignNow(base64File, Inventec.Common.SignLibrary.FileType.Pdf, signEmrInputADO, true);
                    if (signNowResult != null)
                    {
                        errorMessage = !String.IsNullOrWhiteSpace(signNowResult.Message) ? signNowResult.Message : "Tạo văn bản thất bại";
                        result = !signNowResult.Success;
                        if (!String.IsNullOrWhiteSpace(signNowResult.DocumentCode))
                        {
                            CommonParam paramUpdate = new CommonParam();
                            curentSTT.EMR_RESULT_DOCUMENT_CODE = signNowResult.DocumentCode;
                            var apiresult = new BackendAdapter(paramUpdate).Post<LIS_SAMPLE>("api/LisSample/Update", ApiConsumer.ApiConsumers.LisConsumer, curentSTT, paramUpdate);
                            if (apiresult == null)
                            {
                                Inventec.Common.Logging.LogSystem.Error("Lỗi cập nhật thông tin văn bản điện tử của mẫu: " + curentSTT.BARCODE + ". Mã văn bản: " + signNowResult.DocumentCode);
                                result = true;
                                errorMessage += " " + paramUpdate.GetMessage();
                            }
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Error("Lỗi không có thông tin văn bản điện tử: " + curentSTT.BARCODE + ". Mã văn bản: " + signNowResult.DocumentCode);
                            result = true;
                            errorMessage += "Không có thông tin văn bản điện tử";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetUpSignAndPrint(Inventec.Common.SignLibrary.ADO.InputADO inputADO, MPS.ProcessorBase.Core.PrintData PrintData, LIS_SAMPLE curentSTT, ref bool result, ref string errorMessage)
        {
            try
            {
                using (PrintData.saveMemoryStream = new MemoryStream())
                {
                    PrintData.EmrInputADO = inputADO;
                    MPS.MpsPrinter.Run(PrintData);

                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => PrintData), PrintData));
                    //sau hàm Run sẽ có thêm thông tin Inventec.Common.SignLibrary.ADO.InputADO trong PrintData
                    Inventec.Common.SignLibrary.ADO.InputADO signEmrInputADO = (Inventec.Common.SignLibrary.ADO.InputADO)PrintData.EmrInputADO;


                    //string tempPdfFile = Utils.GenerateTempFileWithin();
                    //PrintData.saveMemoryStream.Position = 0;
                    //Inventec.Common.Integrate.FileConvert.ExcelToPdf(PrintData.saveMemoryStream, null, null, tempPdfFile);
                    //string base64File = System.Convert.ToBase64String(Inventec.Common.SignLibrary.Utils.FileToByte(tempPdfFile));
                    string base64File = "";
                    using (MemoryStream pdfStream = new MemoryStream())
                    {
                        PrintData.saveMemoryStream.Position = 0;
                        ConvertExcelToPdfByFlexCel(PrintData.saveMemoryStream, pdfStream);
                        pdfStream.Position = 0;
                        base64File = System.Convert.ToBase64String(Inventec.Common.SignLibrary.Utils.StreamToByte(pdfStream));
                    }
                    signEmrInputADO.IsSign = true;
                    if (this.SignConfigData != null)
                    {
                        //signEmrInputADO.IsMultiSign = this.SignConfigData.IsSignParanel;
                        if (SignConfigData.listSign != null && SignConfigData.listSign.Count > 0)
                        {
                            signEmrInputADO.IsSignConfig = true;
                            signEmrInputADO.SignerConfigs = new List<Inventec.Common.SignLibrary.DTO.SignerConfigDTO>();
                            bool isMe = false;
                            if (!SignConfigData.listSign.Exists(o => o.Loginname == "%me%"))
                            {
                                Inventec.Common.SignLibrary.DTO.SignerConfigDTO dto = new Inventec.Common.SignLibrary.DTO.SignerConfigDTO();
                                isMe = true;
                                dto.NumOrder = 1;
                                dto.Loginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                                signEmrInputADO.SignerConfigs.Insert(0, dto);
                            }
                            foreach (var item in SignConfigData.listSign)
                            {
                                Inventec.Common.SignLibrary.DTO.SignerConfigDTO dto = new Inventec.Common.SignLibrary.DTO.SignerConfigDTO();
                                dto.NumOrder = isMe ? item.NumOrder + 1 : item.NumOrder;
                                dto.Loginname = item.Loginname;
                                signEmrInputADO.SignerConfigs.Add(dto);
                            }
                        }
                    }

                    SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("base64File:___", base64File));
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("signEmrInputADO:___", signEmrInputADO.SignerConfigs));


                    var signNowResult = libraryProcessor.SignAndPrintNow(base64File, Inventec.Common.SignLibrary.FileType.Pdf, signEmrInputADO);
                    if (signNowResult != null)
                    {
                        errorMessage = !String.IsNullOrWhiteSpace(signNowResult.Message) ? signNowResult.Message : "Tạo văn bản thất bại";
                        result = !signNowResult.Success;
                        if (!String.IsNullOrWhiteSpace(signNowResult.DocumentCode))
                        {
                            CommonParam paramUpdate = new CommonParam();
                            curentSTT.EMR_RESULT_DOCUMENT_CODE = signNowResult.DocumentCode;
                            var apiresult = new BackendAdapter(paramUpdate).Post<LIS_SAMPLE>("api/LisSample/Update", ApiConsumer.ApiConsumers.LisConsumer, curentSTT, paramUpdate);
                            if (apiresult == null)
                            {
                                Inventec.Common.Logging.LogSystem.Error("Lỗi cập nhật thông tin văn bản điện tử của mẫu: " + curentSTT.BARCODE + ". Mã văn bản: " + signNowResult.DocumentCode);
                                result = true;
                                errorMessage += " " + paramUpdate.GetMessage();
                            }
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Error("Lỗi không có thông tin văn bản điện tử: " + curentSTT.BARCODE + ". Mã văn bản: " + signNowResult.DocumentCode);
                            result = true;
                            errorMessage += "Không có thông tin văn bản điện tử";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetUpSignAndPrintPreview(Inventec.Common.SignLibrary.ADO.InputADO inputADO, MPS.ProcessorBase.Core.PrintData PrintData, LIS_SAMPLE curentSTT, ref bool result, ref string errorMessage)
        {
            try
            {
                using (PrintData.saveMemoryStream = new MemoryStream())
                {
                    PrintData.EmrInputADO = inputADO;
                    MPS.MpsPrinter.Run(PrintData);
                    Inventec.Common.SignLibrary.ADO.InputADO signEmrInputADO = (Inventec.Common.SignLibrary.ADO.InputADO)PrintData.EmrInputADO;

                    string base64File = "";
                    using (MemoryStream pdfStream = new MemoryStream())
                    {
                        PrintData.saveMemoryStream.Position = 0;
                        ConvertExcelToPdfByFlexCel(PrintData.saveMemoryStream, pdfStream);
                        pdfStream.Position = 0;
                        base64File = System.Convert.ToBase64String(Inventec.Common.SignLibrary.Utils.StreamToByte(pdfStream));
                    }
                    signEmrInputADO.IsSign = true;
                    if (this.SignConfigData != null)
                    {
                        if (SignConfigData.listSign != null && SignConfigData.listSign.Count > 0)
                        {
                            signEmrInputADO.IsSignConfig = true;
                            signEmrInputADO.SignerConfigs = new List<Inventec.Common.SignLibrary.DTO.SignerConfigDTO>();
                            bool isMe = false;
                            if (!SignConfigData.listSign.Exists(o => o.Loginname == "%me%"))
                            {
                                Inventec.Common.SignLibrary.DTO.SignerConfigDTO dto = new Inventec.Common.SignLibrary.DTO.SignerConfigDTO();
                                isMe = true;
                                dto.NumOrder = 1;
                                dto.Loginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                                signEmrInputADO.SignerConfigs.Insert(0, dto);
                            }
                            foreach (var item in SignConfigData.listSign)
                            {
                                Inventec.Common.SignLibrary.DTO.SignerConfigDTO dto = new Inventec.Common.SignLibrary.DTO.SignerConfigDTO();
                                dto.NumOrder = isMe ? item.NumOrder + 1 : item.NumOrder;
                                dto.Loginname = item.Loginname;
                                signEmrInputADO.SignerConfigs.Add(dto);
                            }

                        }
                    }

                    SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("signEmrInputADO:___", signEmrInputADO));


                    var signNowResult = libraryProcessor.SignAndShowPrintPreview(base64File, Inventec.Common.SignLibrary.FileType.Pdf, signEmrInputADO);
                    if (signNowResult != null)
                    {
                        errorMessage = !String.IsNullOrWhiteSpace(signNowResult.Message) ? signNowResult.Message : "Tạo văn bản thất bại";
                        result = !signNowResult.Success;
                        if (!String.IsNullOrWhiteSpace(signNowResult.DocumentCode))
                        {
                            CommonParam paramUpdate = new CommonParam();
                            curentSTT.EMR_RESULT_DOCUMENT_CODE = signNowResult.DocumentCode;
                            var apiresult = new BackendAdapter(paramUpdate).Post<LIS_SAMPLE>("api/LisSample/Update", ApiConsumer.ApiConsumers.LisConsumer, curentSTT, paramUpdate);
                            if (apiresult == null)
                            {
                                Inventec.Common.Logging.LogSystem.Error("Lỗi cập nhật thông tin văn bản điện tử của mẫu: " + curentSTT.BARCODE + ". Mã văn bản: " + signNowResult.DocumentCode);
                                result = true;
                                errorMessage += " " + paramUpdate.GetMessage();
                            }
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Error("Lỗi không có thông tin văn bản điện tử: " + curentSTT.BARCODE + ". Mã văn bản: " + signNowResult.DocumentCode);
                            result = true;
                            errorMessage += "Không có thông tin văn bản điện tử";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetUpSignProcess(Inventec.Common.SignLibrary.ADO.InputADO inputADO, MPS.ProcessorBase.Core.PrintData PrintData, LIS_SAMPLE curentSTT, ref bool result, ref string errorMessage, ref long isPrint)
        {
            try
            {
                using (PrintData.saveMemoryStream = new MemoryStream())
                {
                    Inventec.Common.Logging.LogSystem.Info("Begin SetUpSignProcess");
                    bool isMe = false;
                    PrintData.EmrInputADO = inputADO;
                    MPS.MpsPrinter.Run(PrintData);
                    Inventec.Common.SignLibrary.ADO.InputADO signEmrInputADO = (Inventec.Common.SignLibrary.ADO.InputADO)PrintData.EmrInputADO;
                    string base64File = "";
                    //MemoryStream pdfStream = new MemoryStream();
                    //PrintData.saveMemoryStream.Position = 0;
                    //ConvertExcelToPdfByFlexCel(PrintData.saveMemoryStream, pdfStream);
                    //pdfStream.Position = 0;
                    //base64File = System.Convert.ToBase64String(Inventec.Common.SignLibrary.Utils.StreamToByte(pdfStream));
                    using (MemoryStream pdfStream = new MemoryStream())
                    {
                        PrintData.saveMemoryStream.Position = 0;
                        ConvertExcelToPdfByFlexCel(PrintData.saveMemoryStream, pdfStream);
                        pdfStream.Position = 0;
                        base64File = System.Convert.ToBase64String(Inventec.Common.SignLibrary.Utils.StreamToByte(pdfStream));
                    }
                    signEmrInputADO.IsSign = true;


                    if (this.SignConfigData != null)
                    {
                        if (SignConfigData.listSign != null && SignConfigData.listSign.Count > 0)
                        {
                            signEmrInputADO.IsSignConfig = true;
                            signEmrInputADO.SignerConfigs = new List<Inventec.Common.SignLibrary.DTO.SignerConfigDTO>();
                            foreach (var item in SignConfigData.listSign)
                            {
                                Inventec.Common.SignLibrary.DTO.SignerConfigDTO dto = new Inventec.Common.SignLibrary.DTO.SignerConfigDTO();
                                dto.NumOrder = item.NumOrder;
                                if (item.Loginname == "%me%")
                                {
                                    isMe = true;
                                    dto.Loginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                                }
                                else
                                {
                                    dto.Loginname = item.Loginname;
                                }

                                signEmrInputADO.SignerConfigs.Add(dto);
                            }
                        }
                    }
                    SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();

                    if (checkPrintNow.Checked)
                    {
                        Inventec.Common.Logging.LogSystem.Info("Begin SetUpSignProcess SignAndPrintNow");
                        if (isMe)
                        {
                            var signNowResult = libraryProcessor.SignAndPrintNow(base64File, Inventec.Common.SignLibrary.FileType.Pdf, signEmrInputADO);
                            if (signNowResult != null)
                            {
                                errorMessage = !String.IsNullOrWhiteSpace(signNowResult.Message) ? signNowResult.Message : "Tạo văn bản thất bại";
                                result = !signNowResult.Success;
                                if (!String.IsNullOrWhiteSpace(signNowResult.DocumentCode))
                                {
                                    CommonParam paramUpdate = new CommonParam();
                                    curentSTT.EMR_RESULT_DOCUMENT_CODE = signNowResult.DocumentCode;
                                    var apiresult = new BackendAdapter(paramUpdate).Post<LIS_SAMPLE>("api/LisSample/Update", ApiConsumer.ApiConsumers.LisConsumer, curentSTT, paramUpdate);
                                    if (apiresult == null)
                                    {
                                        Inventec.Common.Logging.LogSystem.Error("Lỗi cập nhật thông tin văn bản điện tử của mẫu: " + curentSTT.BARCODE + ". Mã văn bản: " + signNowResult.DocumentCode);
                                        result = true;
                                        errorMessage += " " + paramUpdate.GetMessage();
                                    }
                                }
                                else
                                {
                                    Inventec.Common.Logging.LogSystem.Error("Lỗi không có thông tin văn bản điện tử: " + curentSTT.BARCODE + ". Mã văn bản: " + signNowResult.DocumentCode);
                                    result = true;
                                    errorMessage += "Không có thông tin văn bản điện tử";
                                }
                            }
                        }
                        else
                        {
                            result = libraryProcessor.CreateDocument(signEmrInputADO, base64File);
                            isPrint = 1;
                        }
                    }
                    else if (chkPrintPreview.Checked)
                    {
                        if (isMe)
                        {
                            Inventec.Common.Logging.LogSystem.Info("Begin SetUpSignProcess SignAndShowPrintPreview");
                            var signNowResult = libraryProcessor.SignAndShowPrintPreview(base64File, Inventec.Common.SignLibrary.FileType.Pdf, signEmrInputADO);
                            if (signNowResult != null)
                            {
                                errorMessage = !String.IsNullOrWhiteSpace(signNowResult.Message) ? signNowResult.Message : "Tạo văn bản thất bại";
                                result = !signNowResult.Success;
                                if (!String.IsNullOrWhiteSpace(signNowResult.DocumentCode))
                                {
                                    CommonParam paramUpdate = new CommonParam();
                                    curentSTT.EMR_RESULT_DOCUMENT_CODE = signNowResult.DocumentCode;
                                    var apiresult = new BackendAdapter(paramUpdate).Post<LIS_SAMPLE>("api/LisSample/Update", ApiConsumer.ApiConsumers.LisConsumer, curentSTT, paramUpdate);
                                    if (apiresult == null)
                                    {
                                        Inventec.Common.Logging.LogSystem.Error("Lỗi cập nhật thông tin văn bản điện tử của mẫu: " + curentSTT.BARCODE + ". Mã văn bản: " + signNowResult.DocumentCode);
                                        result = true;
                                        errorMessage += " " + paramUpdate.GetMessage();
                                    }
                                }
                                else
                                {
                                    Inventec.Common.Logging.LogSystem.Error("Lỗi không có thông tin văn bản điện tử: " + curentSTT.BARCODE + ". Mã văn bản: " + signNowResult.DocumentCode);
                                    result = true;
                                    errorMessage += "Không có thông tin văn bản điện tử";
                                }
                            }
                        }
                        else
                        {
                            result = libraryProcessor.CreateDocument(signEmrInputADO, base64File);
                            isPrint = 2;
                        }
                    }
                    else if (chkSign.Checked || isMe)
                    {
                        Inventec.Common.Logging.LogSystem.Info("Begin SetUpSignProcess SignNow");
                        var signNowResult = libraryProcessor.SignNow(base64File, Inventec.Common.SignLibrary.FileType.Pdf, signEmrInputADO);
                        if (signNowResult != null)
                        {
                            errorMessage = !String.IsNullOrWhiteSpace(signNowResult.Message) ? signNowResult.Message : "Tạo văn bản thất bại";
                            result = !signNowResult.Success;
                            if (!String.IsNullOrWhiteSpace(signNowResult.DocumentCode))
                            {
                                CommonParam paramUpdate = new CommonParam();
                                curentSTT.EMR_RESULT_DOCUMENT_CODE = signNowResult.DocumentCode;
                                var apiresult = new BackendAdapter(paramUpdate).Post<LIS_SAMPLE>("api/LisSample/Update", ApiConsumer.ApiConsumers.LisConsumer, curentSTT, paramUpdate);
                                if (apiresult == null)
                                {
                                    Inventec.Common.Logging.LogSystem.Error("Lỗi cập nhật thông tin văn bản điện tử của mẫu: " + curentSTT.BARCODE + ". Mã văn bản: " + signNowResult.DocumentCode);
                                    result = true;
                                    errorMessage += " " + paramUpdate.GetMessage();
                                }
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Error("Lỗi không có thông tin văn bản điện tử: " + curentSTT.BARCODE + ". Mã văn bản: " + signNowResult.DocumentCode);
                                result = true;
                                errorMessage += "Không có thông tin văn bản điện tử";
                            }
                        }
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Info("Begin SetUpSignProcess CreateDocument");
                        result = libraryProcessor.CreateDocument(signEmrInputADO, base64File);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EnabledLableApproveList(bool enable)
        {
            try
            {
                lciApproveListSuccess.Visibility = enable ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciApproveResultError.Visibility = enable ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciBtnApproveError.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Nếu check ký thì xử lý kiểm tra thiết lập ký và thiết lập vị trí ký
        /// true : tiếp tục xử lý
        /// false: dừng xử lý
        /// </summary>
        /// <returns></returns>
        private bool CheckSignApproveList()
        {
            bool result = true;
            try
            {
                if (chkSignApproveList.Checked)
                {
                    if (this.SignConfigData == null || this.SignConfigData.listSign == null || this.SignConfigData.listSign.Count <= 0)
                    {
                        XtraMessageBox.Show("Chưa thiết lập ký. Vui lòng nhấn nút \"Thiết lập ký\" để thiết lập luồng ký cho phiếu trả kết quả xét nghiệm");
                        result = false;
                        btnCreateSigner.Focus();
                    }
                    else
                    {
                        if (!HisConfigCFG.PRINT_TEST_RESULT)
                        {
                            if (CheckTemplateMps(MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096))
                            {
                                XtraMessageBox.Show("Mẫu trả kết quả xét nghiệm chưa thiết lập vị trí ký");
                                result = false;
                            }
                        }
                        else
                        {
                            List<string> lst = new List<string>();
                            if (CheckTemplateMps("Mps000456"))
                            {
                                lst.Add("huyết học");
                                result = false;
                            }
                            if (CheckTemplateMps("Mps000457"))
                            {
                                lst.Add("vi sinh");
                                result = false;
                            }
                            if (CheckTemplateMps("Mps000458"))
                            {
                                lst.Add("miễn sinh");
                                result = false;
                            }
                            if (CheckTemplateMps("Mps000459"))
                            {
                                lst.Add("sinh hóa");
                                result = false;
                            }
                            if (CheckTemplateMps(MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096))
                            {
                                lst.Add("kết quả xét nghiệm khác");
                                result = false;
                            }
                            if (lst != null && lst.Count > 0)
                            {
                                XtraMessageBox.Show(String.Format("Mẫu trả kết quả xét nghiệm: {0} chưa thiết lập vị trí ký", string.Join(", ", lst)));
                            }
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

        private bool CheckTemplateMps(string prtCode)
        {
            bool result = true;
            try
            {
                this.HasTemplate = false;
                this.SignApproveList_FileName = "";
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                richEditorMain.RunPrintTemplate(prtCode, DelegateCheckPrinter);

                while (!HasTemplate)
                {
                    System.Threading.Thread.Sleep(100);
                }

                //kiểm tra thiết lập vị trí ký
                if (!String.IsNullOrWhiteSpace(dicSignApproveList[prtCode]))
                {
                    FlexCel.XlsAdapter.XlsFile xls = new FlexCel.XlsAdapter.XlsFile(true);
                    xls.Open(this.dicSignApproveList[prtCode]);//đọc template

                    List<string> commentData = new List<string>();//lưu dữ liệu thiết lập comment vị trí ký
                    bool haskey = false;
                    for (int i = 1; i <= xls.RowCount; i++)
                    {
                        for (int j = 1; j <= xls.ColCount; j++)
                        {
                            var commentDataRow = xls.GetComment(i, j);
                            if (commentDataRow != null && !String.IsNullOrWhiteSpace(commentDataRow.Value))
                            {
                                commentData.Add(commentDataRow.Value);
                            }

                            if (!haskey)
                            {
                                var value = xls.GetCellValue(i, j);
                                if (value != null && value.ToString().Contains("SINGLE_KEY__COMMENT_SIGN"))
                                {
                                    haskey = true;
                                }
                            }
                        }
                    }

                    //Có commentData bắt đầu bằng $ hoặc chứa SINGLE_KEY__COMMENT_SIGN
                    if ((commentData.Count > 0 && commentData.Exists(o => o.Trim().StartsWith("$") || o.Contains("SINGLE_KEY__COMMENT_SIGN"))) || haskey)
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

        private bool DelegateCheckPrinter(string printCode, string fileName)
        {
            bool result = true;
            try
            {
                this.SignApproveList_FileName = fileName;

                if (dicSignApproveList != null && dicSignApproveList.Count > 0 && dicSignApproveList.ContainsKey(printCode))
                {
                    dicSignApproveList[printCode] = fileName;
                }
                else
                {
                    this.dicSignApproveList.Add(printCode, fileName);
                }
                HasTemplate = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void btnApproveError_Click(object sender, EventArgs e)
        {
            try
            {
                if (ApproveListError == null || ApproveListError.Count <= 0)
                {
                    return;
                }

                txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);

                Rectangle screenRect = Screen.GetBounds(Bounds);
                popupControlContainerOldValue.ShowPopup(new Point(screenRect.Width / 2 - popupControlContainerOldValue.Width / 2, screenRect.Height / 2 - popupControlContainerOldValue.Height / 2));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ConvertExcelToPdfByFlexCel(MemoryStream excelStream, MemoryStream pdfStream)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ConvertExcelToPdfByFlexCel.2.1");
                FlexCel.Render.FlexCelPdfExport flexCelPdfExport1 = new FlexCel.Render.FlexCelPdfExport();

                flexCelPdfExport1.FontEmbed = FlexCel.Pdf.TFontEmbed.Embed;
                flexCelPdfExport1.PageLayout = FlexCel.Pdf.TPageLayout.None;
                flexCelPdfExport1.PageSize = null;
                FlexCel.Pdf.TPdfProperties tPdfProperties1 = new FlexCel.Pdf.TPdfProperties();
                tPdfProperties1.Author = null;
                tPdfProperties1.Creator = null;
                tPdfProperties1.Keywords = null;
                tPdfProperties1.Subject = null;
                tPdfProperties1.Title = null;
                flexCelPdfExport1.Properties = tPdfProperties1;
                flexCelPdfExport1.Workbook = new FlexCel.XlsAdapter.XlsFile();
                flexCelPdfExport1.Workbook.Open(excelStream);
                Inventec.Common.Logging.LogSystem.Debug("ConvertExcelToPdfByFlexCel.2.2");
                int SaveSheet = flexCelPdfExport1.Workbook.ActiveSheet;
                try
                {
                    flexCelPdfExport1.BeginExport(pdfStream);
                    flexCelPdfExport1.PageLayout = FlexCel.Pdf.TPageLayout.None;
                    flexCelPdfExport1.ExportSheet();
                    flexCelPdfExport1.EndExport();
                }
                finally
                {
                    flexCelPdfExport1.Workbook.ActiveSheet = SaveSheet;
                }
                pdfStream.Position = 0;

                Inventec.Common.Logging.LogSystem.Debug("ConvertExcelToPdfByFlexCel.2.3");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitCboStt()
        {
            try
            {
                cboSttEmr.Properties.Items.AddRange(new object[] { "Tất cả", "Chưa ký", "Đang ký", "Hoàn thành", "Từ chối" });
                cboSttEmr.SelectedIndex = 0;
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItembtnResultDescription_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                TestLisResultADO testLisResultADO = new TestLisResultADO();
                var data = this.treeListSereServTein.GetDataRecordByNode(this.treeListSereServTein.FocusedNode);
                if (data != null && data is TestLisResultADO)
                {
                    testLisResultADO = (TestLisResultADO)data;
                }
                if (testLisResultADO.RESULT_DESCRIPTION != null && testLisResultADO.RESULT_DESCRIPTION != "")
                {
                    txtResultDescription.Text = testLisResultADO.RESULT_DESCRIPTION;
                }
                else
                {
                    txtResultDescription.Text = "";
                }
                ButtonEdit editor = sender as ButtonEdit;
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                string screenWidth = Screen.PrimaryScreen.Bounds.Width.ToString();
                string screenHeight = Screen.PrimaryScreen.Bounds.Height.ToString();

                if (Int64.Parse(screenWidth) > 1600)
                {
                    popupControlResultDescription.Height = 200;
                    popupControlResultDescription.Width = 500;
                }
                else
                {
                    popupControlResultDescription.Height = 200;
                    popupControlResultDescription.Width = 350;
                }
                popupControlResultDescription.ShowPopup(new Point(buttonPosition.X + 532, buttonPosition.Bottom + 200));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnOKForResultDescription_Click(object sender, EventArgs e)
        {
            try
            {
                TestLisResultADO testLisResultADO = new TestLisResultADO();
                var data = this.treeListSereServTein.GetDataRecordByNode(this.treeListSereServTein.FocusedNode);
                if (data != null && data is TestLisResultADO)
                {
                    testLisResultADO = (TestLisResultADO)data;
                }
                if (txtResultDescription.Text != null && txtResultDescription.Text != "")
                {
                    testLisResultADO.RESULT_DESCRIPTION = txtResultDescription.Text;
                }
                else
                {
                    testLisResultADO.RESULT_DESCRIPTION = null;
                }
                treeListSereServTein.RefreshDataSource();
                popupControlResultDescription.HidePopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCancelForResultDescription_Click(object sender, EventArgs e)
        {
            txtResultDescription.Text = "";
            popupControlResultDescription.HidePopup();
        }

        private void TextEditResultRange__Enable_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                LogSystem.Debug("TextEditResultRange__Enable_EditValueChange.1");
                TestLisResultADO testLisResultADO = new TestLisResultADO();
                var data = this.treeListSereServTein.GetDataRecordByNode(this.treeListSereServTein.FocusedNode);
                if (data != null && data is TestLisResultADO)
                {
                    LogSystem.Debug("TextEditResultRange__Enable_EditValueChange.1.1");
                    TextEdit txt = sender as TextEdit;

                    testLisResultADO = (TestLisResultADO)data;

                    if (txt.Text != null && txt.Text != "")
                    {
                        testLisResultADO.RESULT_DESCRIPTION = txt.Text;
                    }
                    else
                    {
                        testLisResultADO.RESULT_DESCRIPTION = null;
                    }
                }

                LogSystem.Debug("TextEditResultRange__Enable_EditValueChange.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TextEditResultRange__Enable_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    treeListSereServTein.PostEditor();
                    TreeListNode node = treeListSereServTein.FocusedNode;
                    if (node != null && node.NextNode != null)
                    {
                        if (node.NextVisibleNode != null && node.NextVisibleNode.HasChildren)
                        {
                            treeListSereServTein.FocusedNode = node.NextVisibleNode.Nodes[0];
                        }
                        else if (node.NextVisibleNode.NextNode != null)
                        {
                            if (node.NextVisibleNode.NextVisibleNode != null
                                && node.NextVisibleNode.NextVisibleNode.HasChildren)
                            {
                                treeListSereServTein.FocusedNode = node.NextVisibleNode.NextVisibleNode.Nodes[0];
                            }
                            else
                            {
                                treeListSereServTein.FocusedNode = node.NextNode;//.NextVisibleNode.NextNode;
                            }
                        }
                        else
                            treeListSereServTein.FocusedNode = node.NextNode;
                    }
                    else if (node != null && node.NextVisibleNode != null)
                    {
                        if (node.NextVisibleNode.HasChildren)
                        {
                            treeListSereServTein.FocusedNode = node.NextVisibleNode.Nodes[0];
                        }
                        else if (node.NextVisibleNode.NextNode != null)
                        {
                            if (node.NextVisibleNode.NextVisibleNode != null && node.NextVisibleNode.NextVisibleNode.HasChildren)
                            {
                                if (node.NextVisibleNode.NextVisibleNode.NextVisibleNode != null
                                    && node.NextVisibleNode.NextVisibleNode.NextVisibleNode.HasChildren)
                                {
                                    treeListSereServTein.FocusedNode = node.NextVisibleNode.NextVisibleNode.NextVisibleNode.Nodes[0];
                                }
                                else
                                    treeListSereServTein.FocusedNode = node.NextVisibleNode;
                            }
                            else if (node.NextVisibleNode.NextVisibleNode != null)
                            {
                                treeListSereServTein.FocusedNode = node.NextVisibleNode.NextVisibleNode;
                            }
                            else
                            {
                                treeListSereServTein.FocusedNode = node.NextVisibleNode;
                            }
                        }
                    }
                    treeListSereServTein.FocusedColumn = treeListSereServTein.Columns[grdColVallue.FieldName];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPrintPreview_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isInit)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.CHECK_PRINT_PREVIEW && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrintPreview.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.CHECK_PRINT_PREVIEW;
                    csAddOrUpdate.VALUE = (chkPrintPreview.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();

                if (chkPrintPreview.Checked)
                {
                    checkPrintNow.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TextEditNote__Enable_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                LogSystem.Debug("TextEditNote__Enable_EditValueChanged.1");
                TestLisResultADO testLisResultADO = new TestLisResultADO();
                var data = this.treeListSereServTein.GetDataRecordByNode(this.treeListSereServTein.FocusedNode);
                if (data != null && data is TestLisResultADO)
                {
                    LogSystem.Debug("TextEditNote__Enable_EditValueChanged.1.1");
                    TextEdit txt = sender as TextEdit;

                    testLisResultADO = (TestLisResultADO)data;

                    if (txt.Text != null && txt.Text != "")
                    {
                        testLisResultADO.NOTE = txt.Text;
                    }
                    else
                    {
                        testLisResultADO.NOTE = null;
                    }
                }

                LogSystem.Debug("TextEditNote__Enable_EditValueChanged.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TextEditNote__Enable_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    treeListSereServTein.PostEditor();
                    TreeListNode node = treeListSereServTein.FocusedNode;
                    if (node != null && node.NextNode != null)
                    {
                        if (node.NextVisibleNode != null && node.NextVisibleNode.HasChildren)
                        {
                            treeListSereServTein.FocusedNode = node.NextVisibleNode.Nodes[0];
                        }
                        else if (node.NextVisibleNode.NextNode != null)
                        {
                            if (node.NextVisibleNode.NextVisibleNode != null
                                && node.NextVisibleNode.NextVisibleNode.HasChildren)
                            {
                                treeListSereServTein.FocusedNode = node.NextVisibleNode.NextVisibleNode.Nodes[0];
                            }
                            else
                            {
                                treeListSereServTein.FocusedNode = node.NextNode;//.NextVisibleNode.NextNode;
                            }
                        }
                        else
                            treeListSereServTein.FocusedNode = node.NextNode;
                    }
                    else if (node != null && node.NextVisibleNode != null)
                    {
                        if (node.NextVisibleNode.HasChildren)
                        {
                            treeListSereServTein.FocusedNode = node.NextVisibleNode.Nodes[0];
                        }
                        else if (node.NextVisibleNode.NextNode != null)
                        {
                            if (node.NextVisibleNode.NextVisibleNode != null && node.NextVisibleNode.NextVisibleNode.HasChildren)
                            {
                                if (node.NextVisibleNode.NextVisibleNode.NextVisibleNode != null
                                    && node.NextVisibleNode.NextVisibleNode.NextVisibleNode.HasChildren)
                                {
                                    treeListSereServTein.FocusedNode = node.NextVisibleNode.NextVisibleNode.NextVisibleNode.Nodes[0];
                                }
                                else
                                    treeListSereServTein.FocusedNode = node.NextVisibleNode;
                            }
                            else if (node.NextVisibleNode.NextVisibleNode != null)
                            {
                                treeListSereServTein.FocusedNode = node.NextVisibleNode.NextVisibleNode;
                            }
                            else
                            {
                                treeListSereServTein.FocusedNode = node.NextVisibleNode;
                            }
                        }
                    }
                    treeListSereServTein.FocusedColumn = treeListSereServTein.Columns[grdColVallue.FieldName];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private async Task LoadUser()
        {
            try
            {

                List<ACS_USER> listResult = BackendDataWorker.Get<ACS_USER>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                // Get listemployee
                List<HIS_EMPLOYEE> listHisEmployee;
                if (BackendDataWorker.IsExistsKey<HIS_EMPLOYEE>())
                {
                    listHisEmployee = BackendDataWorker.Get<HIS_EMPLOYEE>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic dynamicFilter = new System.Dynamic.ExpandoObject();
                    listHisEmployee = await new BackendAdapter(paramCommon).GetAsync<List<HIS_EMPLOYEE>>("api/HisEmployee/Get", ApiConsumers.MosConsumer, dynamicFilter, paramCommon);

                    if (listHisEmployee != null) BackendDataWorker.UpdateToRam(typeof(HIS_EMPLOYEE), listHisEmployee, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                var listLoginNameEmployee = listHisEmployee != null ? listHisEmployee.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Select(p => p.LOGINNAME).ToList() : null;

                if (listLoginNameEmployee != null && listLoginNameEmployee.Count > 0)
                {
                    listResult = listResult.Where(o => listLoginNameEmployee.Contains(o.LOGINNAME)).ToList();
                }
                MOS.Filter.HisUserRoomFilter filter = new HisUserRoomFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                // filter.ROOM_ID = currentModule.RoomId;
                Inventec.Common.Logging.LogSystem.Debug("currentServiceReq.EXECUTE_ROOM_ID: " + currentServiceReq.EXECUTE_ROOM_ID);
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 400);

                ControlEditorLoader.Load(cboUserKQ, listResult, controlEditorADO);
                //cboUserKQ.EditValue =  Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                //txtUserKQ.Text = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private async Task DataLoadUser(LisSampleADO sample)
        {

            try
            {
                ACS.EFMODEL.DataModels.ACS_USER commune = new ACS_USER();
                if (sample.RESULT_LOGINNAME != null)
                {
                    cboUserKQ.EditValue = sample.RESULT_LOGINNAME;
                    commune = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == cboUserKQ.EditValue.ToString());

                    if (commune != null)
                    {
                        txtUserKQ.Text = commune.LOGINNAME;
                    }
                }
                else
                {
                    this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();

                    this.currentBySessionControlStateRDO = controlStateWorker.GetDataBySession(ControlStateConstant.MODULE_LINK);
                    if (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0)
                    {

                        foreach (var item_ in this.currentBySessionControlStateRDO)
                        {
                            if (item_.KEY == cboUserKQ.Name)
                            {
                                if (!String.IsNullOrEmpty(item_.VALUE))
                                {
                                    cboUserKQ.EditValue = item_.VALUE;
                                }
                            }
                        }
                    }
                    else
                    {
                        cboUserKQ.EditValue = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                        txtUserKQ.Text = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    }
                }
                GetTimeSystem();
                if (sample.RESULT_TIME != null)
                {
                    DateKQ.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(sample.RESULT_TIME ?? 0) ?? DateTime.Now;
                }
                else
                {
                    DateKQ.EditValue = currentTimer;
                }
                if (sample.SAMPLE_TIME != null)
                {
                    DateLM.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(sample.SAMPLE_TIME ?? 0) ?? DateTime.Now;
                }
                else
                {
                    DateLM.EditValue = currentTimerLM;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void cboUserKQ_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboUserKQ.EditValue != null && cboUserKQ.EditValue != cboUserKQ.OldEditValue)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER commune = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == cboUserKQ.EditValue.ToString());
                        if (commune != null)
                        {
                            txtUserKQ.Text = commune.LOGINNAME;
                        }
                        WaitingManager.Show();
                        HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0) ? this.currentBySessionControlStateRDO.Where(o => o.KEY == cboUserKQ.Name && o.MODULE_LINK == ModuleLinkName).FirstOrDefault() : null;
                        if (csAddOrUpdate != null)
                        {
                            csAddOrUpdate.VALUE = cboUserKQ.EditValue.ToString();
                        }
                        else
                        {
                            csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                            csAddOrUpdate.KEY = cboUserKQ.Name;
                            csAddOrUpdate.VALUE = cboUserKQ.EditValue.ToString();
                            csAddOrUpdate.MODULE_LINK = ModuleLinkName;
                            if (this.currentBySessionControlStateRDO == null)
                                this.currentBySessionControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();

                            this.currentBySessionControlStateRDO.Add(csAddOrUpdate);
                        }
                        this.controlStateWorker.SetDataBySession(this.currentBySessionControlStateRDO);
                        WaitingManager.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboUserKQ_EditValueChanged(object sender, EventArgs e)
        {

            try
            {
                if (cboUserKQ.EditValue != null && cboUserKQ.EditValue != cboUserKQ.OldEditValue)
                {
                    ACS.EFMODEL.DataModels.ACS_USER commune = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == cboUserKQ.EditValue.ToString());
                    if (commune != null)
                    {
                        txtUserKQ.Text = commune.LOGINNAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }


        }

        private void txtUserKQ_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!string.IsNullOrEmpty(txtUserKQ.Text))
                {
                    cboUserKQ.EditValue = txtUserKQ.Text;
                }
                cboUserKQ.Focus();
            }
        }

        private void chkSignProcess_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkSignProcess.Checked)
                    chkSign.Checked = !chkSignProcess.Checked;

                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkSignProcess.Name && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkSignProcess.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkSignProcess.Name;
                    csAddOrUpdate.VALUE = (chkSignProcess.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListSereServTein_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {

        }
        private void timerReloadMachineCounter_Tick()
        {
            try
            {

                LogSystem.Debug("timerReloadMachineCounter_Tick.1");
                Task ts = Task.Factory.StartNew(ReloadMachineCounter);
                //ReloadMachineCounter();
                LogSystem.Debug("timerReloadMachineCounter_Tick.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ReloadMachineCounter()
        {
            try
            {
                if (HisConfigCFG.MaxServicePerDayWarningOption == "1" || HisConfigCFG.MaxServicePerDayWarningOption == "2")
                {
                    LogSystem.Debug("ReloadMachineCounter. 1");
                    LogSystem.Debug("ReloadMachineCounter. 2");
                    HisMachineCounterFilter filter = new HisMachineCounterFilter();
                    List<HisMachineCounterSDO> sdos = new BackendAdapter(new CommonParam()).Get<List<HisMachineCounterSDO>>("api/HisMachine/GetCounter", ApiConsumers.MosConsumer, filter, null);
                    LogSystem.Debug("ReloadMachineCounter. 3");
                    GlobalVariables.MachineCounterSdos = sdos;
                    LogSystem.Debug("ReloadMachineCounter. 5");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListSereServTein_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                TreeList tree = sender as TreeList;
                if (tree.FocusedNode != null)
                {
                    TestLisResultADO data = (TestLisResultADO)this.treeListSereServTein.GetDataRecordByNode(this.treeListSereServTein.FocusedNode);
                    if (tree.FocusedColumn.FieldName == "MACHINE_ID" && tree.ActiveEditor is GridLookUpEdit && data.IS_PARENT == 1)
                    {
                        treeListSereServTein.ClearColumnErrors();
                        GridLookUpEdit editor = tree.ActiveEditor as GridLookUpEdit;
                        FillDataMachineCombo(data, editor);
                        //editor.EditValue = data != null ? data.MACHINE_ID : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        void FocusCellByRowHandle(ColumnView view, int rowHandle, GridColumn column)
        {
            if (view == null || !view.IsDataRow(rowHandle)) return;
            view.FocusedColumn = column;
            view.FocusedRowHandle = rowHandle;
            view.ShowEditor();
        }

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
           
        }

        private void chkKhongHienThiChuaLayMau_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (HisConfigCFG.IsRequiredSampled && !chkKhongHienThiChuaLayMau.Checked)
                {
                    chkKhongHienThiChuaLayMau.Checked = true;
                }
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DateKQ_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (DateKQ.EditValue != null && DateKQ.DateTime != DateTime.MinValue)
                    currentTimer = DateKQ.DateTime;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DateKQ_Leave(object sender, EventArgs e)
        {
            try
            {
                currentTimer = DateKQ.DateTime;
                DateKQ.SelectionStart = 0;
                StartTimer(currentModule.ModuleLink, "timer1");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkWarn_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkWarn.Checked)
                    chkCon.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkCon_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkCon.Checked)
                    chkWarn.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void UpdateRadio(string value)
        {
            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.COMBO_CONFIG_MACHINE && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = value;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.COMBO_CONFIG_MACHINE;
                    csAddOrUpdate.VALUE = value;
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DateLM_Leave(object sender, EventArgs e)
        {
            try
            {
                currentTimerLM = DateLM.DateTime;
                DateLM.SelectionStart = 0;
                StartTimer(currentModule.ModuleLink, "timer2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DateLM_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (DateLM.EditValue != null && DateLM.DateTime != DateTime.MinValue)
                    currentTimerLM = DateLM.DateTime;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryHuyChapNhan_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (LisSampleADO)gridViewSample.GetFocusedRow();
                if (data != null && data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    bool success = false;
                    LisSampleApproveSDO sdo = new LisSampleApproveSDO();
                    sdo.SampleId = data.ID;
                    sdo.WorkingRoomId = currentModule.RoomId;
                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/UnApprove", ApiConsumers.LisConsumer, sdo, param);
                    if (rs != null)
                    {
                        var sampleRow = lstSampleAll.FirstOrDefault(o => o.ID == data.ID);
                        sampleRow.SAMPLE_STT_ID = rs.SAMPLE_STT_ID;
                        sampleRow.APPROVAL_TIME = rs.APPROVAL_TIME;
                        sampleRow.APPROVAL_LOGINNAME = rs.APPROVAL_LOGINNAME;
                        sampleRow.APPROVAL_USERNAME = rs.APPROVAL_USERNAME;
                        sampleRow.IS_SAMPLE_ORDER_REQUEST = rs.IS_SAMPLE_ORDER_REQUEST;
                        gridControlSample.RefreshDataSource();
                        gridViewSample.FocusedRowHandle = gridViewSample.FocusedRowHandle - 1;
                        gridViewSample.FocusedRowHandle = gridViewSample.FocusedRowHandle + 1;
                        success = true;
                    }
                    WaitingManager.Hide();
                    if (success)
                    {
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                    else
                    {
                        MessageManager.Show(param, success);
                    }

                    SessionManager.ProcessTokenLost(param);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListSereServTein_GetSelectImage(object sender, GetSelectImageEventArgs e)
        {
            //try
            //{
            //    var data = (TestLisResultADO)this.treeListSereServTein.GetDataRecordByNode(e.Node);
            //    if (data.IS_PARENT == 1 && data.SAMPLE_SERVICE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_SERVICE_STT.ID__DA_TRA_KQ)
            //    {
            //        e.NodeImageIndex = 4;
            //    }
            //    else 
            //        e.NodeImageIndex = -1;
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
            e.NodeImageIndex = -1;
        }

        private void treeListSereServTein_GetStateImage(object sender, GetStateImageEventArgs e)
        {
            try
            {
                var data = (TestLisResultADO)this.treeListSereServTein.GetDataRecordByNode(e.Node);
                if ((data.IS_PARENT == 1 && data.IS_NOT_SHOW_SERVICE == 1))
                {
                    if (data.IS_RUNNING == 1)
                    {
                        e.NodeImageIndex = 3;
                    }
                    else if (data.SAMPLE_SERVICE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_SERVICE_STT.ID__CHUA_CO_KQ)
                    {
                        e.NodeImageIndex = 0;
                    }
                    else if (data.SAMPLE_SERVICE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_SERVICE_STT.ID__DA_CO_KQ)
                    {
                        e.NodeImageIndex = 1;
                    }
                    else if (data.SAMPLE_SERVICE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_SERVICE_STT.ID__DA_TRA_KQ)
                    {
                        e.NodeImageIndex = 2;
                    }
                }
                else
                    e.NodeImageIndex = -1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                //if (e.SelectedControl is DevExpress.XtraTreeList.TreeList)
                {
                    DevExpress.Utils.ToolTipControlInfo info = null;
                    TreeListHitInfo hi = treeListSereServTein.CalcHitInfo(e.ControlMousePosition);
                    var o = hi.Node;
                    var data = (TestLisResultADO)treeListSereServTein.GetDataRecordByNode(o);
                    if (hi.HitInfoType == HitInfoType.SelectImage)
                    {
                        string text = "";
                        if (data.IS_PARENT == 1 && data.SAMPLE_SERVICE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_SERVICE_STT.ID__DA_TRA_KQ)
                        {
                            text = "Trả kết quả";
                        }
                        info = new DevExpress.Utils.ToolTipControlInfo(o, text);
                        e.Info = info;
                    }
                    else if (hi.HitInfoType == HitInfoType.StateImage)
                    {
                        string text = "";
                        if (data.IS_PARENT != 1 || (data.IS_PARENT == 1 && data.IS_NOT_SHOW_SERVICE == 1))
                        {
                            if (data.IS_RUNNING == 1)
                            {
                                text = "Đang chạy xét nghiệm";
                            }
                            else if (data.SAMPLE_SERVICE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_SERVICE_STT.ID__CHUA_CO_KQ)
                            {
                                text = "Chưa có kết quả";
                            }
                            else if (data.SAMPLE_SERVICE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_SERVICE_STT.ID__DA_CO_KQ)
                            {
                                text = "Đã có kết quả";
                            }
                            else if (data.SAMPLE_SERVICE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_SERVICE_STT.ID__DA_TRA_KQ)
                            {
                                text = "Đã trả kết quả";
                            }
                        }
                        info = new DevExpress.Utils.ToolTipControlInfo(o, text);
                        e.Info = info;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListSereServTein_SelectImageClick(object sender, NodeClickEventArgs e)
        {
            try
            {
                var data = (TestLisResultADO)treeListSereServTein.GetDataRecordByNode(e.Node);
                if (data != null && data.IS_PARENT == 1 && data.SAMPLE_SERVICE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_SERVICE_STT.ID__DA_TRA_KQ)
                {
                    ButtonEdit_TraKetQua_ButtonClick(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void lkbAmTinh_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                SetTextValueRange(sender as LinkLabel);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void lkbDuongTinh_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                SetTextValueRange(sender as LinkLabel);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetTextValueRange(LinkLabel lbl)
        {

            try
            {
                TestLisResultADO testLisResultADO = new TestLisResultADO();
                var data = this.treeListSereServTein.GetDataRecordByNode(this.treeListSereServTein.FocusedNode);
                if (data != null && data is TestLisResultADO)
                {
                    testLisResultADO = (TestLisResultADO)data;
                }
                testLisResultADO.VALUE_RANGE = lbl.Text;

                treeListSereServTein.RefreshDataSource();
                popupControlContainerRangeValue.HidePopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void chkOrderByEmergencyPriority_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isInit)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.CHECK_ORDER_BY_EMERGENCY_PRIORITY && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkOrderByEmergencyPriority.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.CHECK_ORDER_BY_EMERGENCY_PRIORITY;
                    csAddOrUpdate.VALUE = (chkOrderByEmergencyPriority.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                FillDataToGridControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAppointment_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepart_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
                {
                    cboDepart.ShowPopup();
                }
                else if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboDepart.Properties.Buttons[1].Visible = false;
                    cboDepart.EditValue = null;
                    GridCheckMarksSelection gridCheckMark = cboDepart.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboDepart.Properties.View);
                    }
                    this.cboDepart.Focus();
                    cboDepart.Refresh();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                cbo.Properties.View.Columns.Clear();
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                cbo.Properties.View.OptionsView.GroupDrawMode = DevExpress.XtraGrid.Views.Grid.GroupDrawMode.Office;
                cbo.Properties.View.OptionsView.HeaderFilterButtonShowMode = DevExpress.XtraEditors.Controls.FilterButtonShowMode.SmartTag;
                cbo.Properties.View.OptionsView.ShowAutoFilterRow = true;
                cbo.Properties.View.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDepart_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                //cboDepart.Properties.View.OptionsView.Reset();
                //LoadDataToDepartRoom();
                cboDepart.Properties.View.ClearColumnsFilter();

                if (_StatusSelecteds != null && _StatusSelecteds.Count > 0)
                {
                    cboDepart.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboDepart.Properties.Buttons[1].Visible = false;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SelectionGrid__Status(object sender, EventArgs e)
        {
            try
            {
                _StatusSelecteds = new List<HIS_DEPARTMENT>();
                lstIDDepart.Clear();
                foreach (HIS_DEPARTMENT rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                    {
                        lstIDDepart.Add(rv.ID);
                        _StatusSelecteds.Add(rv);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__StatusRoom(object sender, EventArgs e)
        {
            try
            {
                _StatusSelectedRooms = new List<V_HIS_ROOM>();
                foreach (V_HIS_ROOM rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _StatusSelectedRooms.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRoom_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
                {
                    cboRoom.ShowPopup();
                }
                else if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {

                    cboRoom.EditValue = null;
                    GridCheckMarksSelection gridCheckMark = cboRoom.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboRoom.Properties.View);
                    }
                    this.cboRoom.Focus();
                    cboRoom.Text = "";

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepart_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string statusName = "";
                if (_StatusSelecteds != null && _StatusSelecteds.Count > 0)
                {
                    foreach (var item in _StatusSelecteds)
                    {
                        statusName += item.DEPARTMENT_NAME + ", ";
                    }
                }

                e.DisplayText = statusName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboDepart_Validated(object sender, EventArgs e)
        {
            try
            {
                LoadDefaulRoom();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDefaulRoom()
        {
            try
            {
                List<V_HIS_ROOM> lstRoom;
                if (lstIDDepart != null && lstIDDepart.Count > 0)
                {
                    lstRoom = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.IS_ACTIVE == 1 && (o.ROOM_TYPE_ID == 1 || o.ROOM_TYPE_ID == 3 || o.ROOM_TYPE_ID == 4) && lstIDDepart.Contains(o.DEPARTMENT_ID)).ToList();
                }
                else
                {
                    lstRoom = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.IS_ACTIVE == 1 && (o.ROOM_TYPE_ID == 1 || o.ROOM_TYPE_ID == 3 || o.ROOM_TYPE_ID == 4)).ToList();
                }
                InitCheck(cboRoom, SelectionGrid__StatusRoom);
                InitCombo(cboRoom, lstRoom, "ROOM_NAME", "ROOM_CODE", "Phòng");

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRoom_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string statusName = "";
                if (_StatusSelectedRooms != null && _StatusSelectedRooms.Count > 0)
                {
                    foreach (var item in _StatusSelectedRooms)
                    {
                        statusName += item.ROOM_NAME + ", ";
                    }
                }

                e.DisplayText = statusName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                //LoadDefaulRoom();

                cboRoom.Properties.View.ClearColumnsFilter();
                if (_StatusSelectedRooms != null && _StatusSelectedRooms.Count > 0)
                {
                    cboRoom.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboRoom.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDepart_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(cboDepart.Text))
                {
                    cboDepart.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboDepart.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRoom_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(cboRoom.Text))
                {
                    cboRoom.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboRoom.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTreatmentCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtTreatmentCode.Text))
                    {
                        FillDataToGridControl();
                    }
                    else
                    {
                        dtBarcodeTimefrom.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

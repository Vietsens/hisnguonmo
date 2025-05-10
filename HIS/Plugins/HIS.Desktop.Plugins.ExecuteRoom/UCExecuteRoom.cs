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
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExecuteRoom.ADO;
using HIS.Desktop.Plugins.ExecuteRoom.Base;
using HIS.Desktop.Plugins.ExecuteRoom.Resources;
using HIS.Desktop.Utility;
using HIS.UC.TreeSereServ7V2;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Plugins.ExecuteRoom;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Inventec.Common.Controls.EditorLoader;
using System.Text;
using HIS.Desktop.Utilities.Extensions;
using DevExpress.XtraEditors.Repository;
using Inventec.Common.Logging;
using HIS.Desktop.Plugins.Library.OtherTreatmentHistory.Base;
using HIS.Desktop.Plugins.Library.OtherTreatmentHistory;
using DevExpress.Utils.Menu;
using HIS.Desktop.LocalStorage.BackendData;
using System.Threading;
using HIS.Desktop.Plugins.Library.FormMedicalRecord;
using System.Resources;


namespace HIS.Desktop.Plugins.ExecuteRoom
{
    public partial class UCExecuteRoom : UserControlBase
    {
        #region derlare

        internal long roomId;
        internal long roomTypeId;

        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;

        internal ServiceReqADO currentHisServiceReq { get; set; }
        internal ServiceReqADO serviceReqRightClick { get; set; }
        internal List<ServiceReqADO> serviceReqs { get; set; }
        internal List<SereServADO> sereServ7s { get; set; }
        internal V_HIS_EXECUTE_ROOM executeRoom { get; set; }
        //bool check = false;
        bool isFirstTime = true;
        int rowCount = 0;
        int dataTotal = 0;
        int numPageSize;
        int lastRowHandle = -1;
        bool needHandleOnClick = true;
        GridColumn lastColumn = null;
        ToolTipControlInfo lastInfo = null;
        List<SereServ6ADO> sereServ6s { get; set; }
        V_HIS_PATIENT_TYPE_ALTER currentPatientTypeAlter { get; set; }
        TreeSereServ7V2Processor ssTreeProcessor { get; set; }
        TreeSereServ7V2Processor p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16;
        UserControl ucTreeSereServ7 { get; set; }
        UserControl u1, u2, u3, u4, u5, u6, u7, u8, u9, u10, u11, u12, u13, u14, u15, u16;
        ExecuteRoomPopupMenuProcessor executeRoomPopupMenuProcessor;
        CPA.WCFClient.CallPatientClient.CallPatientClientManager clienttManager = null;

        List<L_HIS_SERVICE_REQ> ServiceReqCurrentTreatment { get; set; }
        List<ADOserserv7> SereServCurrentTreatment { get; set; }
        List<HIS_PATIENT_TYPE> selectedPatientTypeList { get; set; }
        List<HIS_PATIENT_TYPE> patientTypeList { get; set; }

        bool isInit = true;
        long timeCount = 0;
        long maxTimeReload = 0;

        GridColumn lastColumnSS = null;
        int lastRowHandleSS = -1;
        ToolTipControlInfo lastInfoSS = null;
        string ModuleLinkName = "HIS.Desktop.Plugins.ExecuteRoom";

        internal string typeCodeFind__KeyWork_InDate = "Ngày";//Set lại giá trị trong resource
        internal string typeCodeFind_InDate = "Ngày";//Set lại giá trị trong resource
        internal string typeCodeFind__InMonth = "Tháng";//Set lại giá trị trong resource
        internal string typeCodeFind_RangeDate = "Khoảng ngày";
        List<V_HIS_SERVICE> serviceSelecteds;
        List<V_HIS_SERVICE> listServices;

        bool isNotLoadWhileChangeControlStateInFirst;
        bool CPALoad = false;

        BarManager baManagerMenu = null;
        V_HIS_SERE_SERV_6 _sereServRowMenu = null;
        PopupMenuProcessor popupMenuProcessor = null;
        Inventec.Desktop.Common.Modules.Module currentModule;
        List<ServiceReqADO> CheckServiceExecuteGroup = new List<ServiceReqADO>();

        MediRecordMenuPopupProcessor emrMenuPopupProcessor = null;

        List<long> CheckListCPA = new List<long>();
        List<V_HIS_EXECUTE_ROOM> lstExecuteRoom { get; set; }

        private List<HIS_PRIORITY_TYPE> lstPatientPrioty { get; set; }

        public List<HIS_SERVICE_ROOM> lstServiceRoom { get; private set; }
        List<HIS_PATIENT_TYPE> lstPatientType { get; set; }
        List<HIS_DESK> deskList { get; set; }
        EpaymentDepositResultSDO epaymentDepositResultSDO;
        V_HIS_TREATMENT_4 currentTreatment4;
        bool chkModule = false;
        #region IsClick
        bool isEventPopupMenuShowing = false;
        long treatmentId = 0;
        #endregion

        private const string HIS000021 = "HIS000021";

        #endregion

        public UCExecuteRoom()
        {
            InitializeComponent();
        }

        public UCExecuteRoom(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.currentModule = moduleData;
                this.roomId = moduleData.RoomId;
                this.roomTypeId = moduleData.RoomTypeId;               
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCExecuteRoom_Load(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("UCExecuteRoom_Load.1");
                GetDataFromRam();
                Inventec.Common.Logging.LogSystem.Debug("UCExecuteRoom_Load.2");
                HisConfigCFG.LoadConfig();
                ShowCheckBoxIsResult();
                //this.typeCodeFind = typeCodeFind__KeyWork;
                Inventec.Common.Logging.LogSystem.Debug("UCExecuteRoom_Load.3");
                InitTypeFind();
                InitComboSucKhoe();
                LoadActionButtonRefesh(true);
                Inventec.Common.Logging.LogSystem.Debug("UCExecuteRoom_Load.4");
                this.InitControlState();
                InitLanguage();
                Inventec.Common.Logging.LogSystem.Debug("UCExecuteRoom_Load.5");
                AddUctoPanel();
                Inventec.Common.Logging.LogSystem.Debug("UCExecuteRoom_Load.5.1");
                HideControl();
                //AddOrtherUc();//bỏ đi, không load toàn bộ giao diện lên nếu chưa hiển thị ra, chuyển sang dạng lazyload từng phần cần trước, phần nào dùng đến thì load sau
                Inventec.Common.Logging.LogSystem.Debug("UCExecuteRoom_Load.5.2");
                InitComboBoxEditStatus();
                Inventec.Common.Logging.LogSystem.Debug("UCExecuteRoom_Load.6");
                LoadDefaultData();
                LoadDepartmentAndRoom();
                LoadDataToPatientTypeCombo();
                Inventec.Common.Logging.LogSystem.Debug("UCExecuteRoom_Load.7");
                this.InitGridCheckMarksSelectionPatientType();

                GridCheckMarksSelection gridCheckMark = cboPatientType.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(patientTypeList);
                }
                // Init combo service req type
                InitServiceReqTypeCheck();
                InitComboServiceReqType();
                Inventec.Common.Logging.LogSystem.Debug("UCExecuteRoom_Load.8");
                LoadDefaultScreenSaver();
               
               
                Inventec.Common.Logging.LogSystem.Debug("UCExecuteRoom_Load.9");
                RegisterTimer(currentModule.ModuleLink, "timerAutoReload", timerAutoReload.Interval, timerAutoReload_Tick);
                FillDataToGridControl();
                Inventec.Common.Logging.LogSystem.Debug("UCExecuteRoom_Load.10");
                gridControlServiceReq.ToolTipController = toolTipController1;
                InitEnableControl();
                gridControlSereServServiceReq.ToolTipController = toolTipController2;
                Inventec.Common.Logging.LogSystem.Debug("UCExecuteRoom_Load.11");
                LoadServiceReqCount(true, 0);
                this.isInit = false;
                //RegisterTimer(currentModule.ModuleLink, "timerDoubleClick", timerDoubleClick.Interval, timer1_Tick);
                Inventec.Common.Logging.LogSystem.Debug("UCExecuteRoom_Load.12");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCExecuteRoom
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ExecuteRoom.Resources.Lang", typeof(UCExecuteRoom).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ckKQCLS.Properties.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.ckKQCLS.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ckKQCLS.ToolTip = Inventec.Common.Resource.Get.Value("UCExecuteRoom.ckKQCLS.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnNotEnter.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnNotEnter.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnNotEnter.ToolTip = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnNotEnter.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnMissCall.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnMissCall.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnMissCall.ToolTip = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnMissCall.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkScreenSaver.Properties.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.chkScreenSaver.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRecallPatient.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnRecallPatient.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCallPatient.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnCallPatient.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtStepNumber.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCExecuteRoom.txtStepNumber.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtGateNumber.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCExecuteRoom.txtGateNumber.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnServiceReqList.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnServiceReqList.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnUnStart.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnUnStart.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnTreatmentHistory.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnTreatmentHistory.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRoomTran.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnRoomTran.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDepositReq.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnDepositReq.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnBordereau.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnBordereau.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnExecute.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnExecute.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage1.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.xtraTabPage1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage2.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.xtraTabPage2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl14.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.labelControl14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl13.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.labelControl13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl12.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.labelControl12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl11.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.labelControl11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl10.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.labelControl10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl9.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.labelControl9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtBMI.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.txtBMI.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem75.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem75.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem76.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem76.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem77.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem77.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem79.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem79.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem80.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem80.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem81.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem81.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem82.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem82.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem45.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem45.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem46.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem46.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem47.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem47.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem52.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem52.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem57.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem57.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem57.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem57.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem58.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem58.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem58.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem58.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem59.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem59.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem61.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem61.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem61.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem61.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem63.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem63.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem63.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem63.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem65.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem65.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage18.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.xtraTabPage18.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage4.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.xtraTabPage4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage5.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.xtraTabPage5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage6.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.xtraTabPage6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage7.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.xtraTabPage7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage8.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.xtraTabPage8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage9.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.xtraTabPage9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage10.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.xtraTabPage10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage11.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.xtraTabPage11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage12.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.xtraTabPage12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage3.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.xtraTabPage3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage13.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.xtraTabPage13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage14.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.xtraTabPage14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage15.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.xtraTabPage15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage16.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.xtraTabPage16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage17.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.xtraTabPage17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabDocumentReq.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.xtraTabDocumentReq.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabDocumentResult.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.xtraTabDocumentResult.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnStt.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumnStt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.repositoryItemPictureEdit3.NullText = Inventec.Common.Resource.Get.Value("UCExecuteRoom.repositoryItemPictureEdit3.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnInstructionNote.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumnInstructionNote.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnSoPhieu.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumnSoPhieu.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnSereServServiceReqBlock.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumnSereServServiceReqBlock.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.s.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.s.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem20.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem33.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem33.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem34.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem34.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPhoneNumber.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.lciPhoneNumber.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExamEndType.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCExecuteRoom.lciExamEndType.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExamEndType.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.lciExamEndType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExamTreatmentEndType.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.lciExamTreatmentEndType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExamTreatmentResult.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.lciExamTreatmentResult.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcgPatientInfo.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.lcgPatientInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcgTreeSereServ.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.lcgTreeSereServ.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcgServiceReq.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.lcgServiceReq.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.LciGroupEmrDocument.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.LciGroupEmrDocument.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkCPA.Properties.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.chkCPA.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkCPA.ToolTip = Inventec.Common.Resource.Get.Value("UCExecuteRoom.chkCPA.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboSucKhoe.Properties.NullText = Inventec.Common.Resource.Get.Value("UCExecuteRoom.cboSucKhoe.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboServiceRoom.Properties.NullText = Inventec.Common.Resource.Get.Value("UCExecuteRoom.cboServiceRoom.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsResult.Properties.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.chkIsResult.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCodeFind.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnCodeFind.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCExecuteRoom.cboPatientType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefesh.ToolTip = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnRefesh.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTRANGTHAI_IMG.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.grdColTRANGTHAI_IMG.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.repositoryItemPictureEdit1.NullText = Inventec.Common.Resource.Get.Value("UCExecuteRoom.repositoryItemPictureEdit1.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.repositoryItemPictureEdit5.NullText = Inventec.Common.Resource.Get.Value("UCExecuteRoom.repositoryItemPictureEdit5.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPRIORIRY_DISPLAY.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.grdColPRIORIRY_DISPLAY.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPRIORIRY_DISPLAY.ToolTip = Inventec.Common.Resource.Get.Value("UCExecuteRoom.grdColPRIORIRY_DISPLAY.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.repositoryItemPictureEdit2.NullText = Inventec.Common.Resource.Get.Value("UCExecuteRoom.repositoryItemPictureEdit2.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.grdColNUM_ORDER.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.grdColNUM_ORDER.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnBusyCount.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumnBusyCount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColServiceReqCode.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.grdColServiceReqCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColVirPatientName.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.grdColVirPatientName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPatientCode.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.grdColPatientCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColDob.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.grdColDob.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnGender.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumnGender.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnTreatmentCode.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumnTreatmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnInstructionTime.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumnInstructionTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnServiceReqBlock.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumnServiceReqBlock.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_PatientClassify.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gc_PatientClassify.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_PatientClassify.ToolTip = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gc_PatientClassify.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Gc_MedisoftHistory.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.Gc_MedisoftHistory.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColFinishTime.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.grdColFinishTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPatientType.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.grdColPatientType.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.repositoryItemPictureEdit6.NullText = Inventec.Common.Resource.Get.Value("UCExecuteRoom.repositoryItemPictureEdit6.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.repositoryItemPictureEdit4.NullText = Inventec.Common.Resource.Get.Value("UCExecuteRoom.repositoryItemPictureEdit4.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.btnFind.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchKey.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCExecuteRoom.txtSearchKey.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIntructionDateTo.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.lciIntructionDateTo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem40.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem40.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem40.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem40.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem54.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem54.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem35.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem35.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem35.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem35.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRequestAndMaxRequest.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCExecuteRoom.lciRequestAndMaxRequest.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRequestAndMaxRequest.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.lciRequestAndMaxRequest.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem42.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem42.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem53.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem53.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem53.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.layoutControlItem53.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcCkhCPA.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.lcCkhCPA.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                typeCodeFind__KeyWork_InDate = Inventec.Common.Resource.Get.Value("UcViewEmrDocument.typeCodeFind__KeyWork_InDate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                typeCodeFind_InDate = Inventec.Common.Resource.Get.Value("UcViewEmrDocument.typeCodeFind_InDate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                typeCodeFind__InMonth = Inventec.Common.Resource.Get.Value("UcViewEmrDocument.typeCodeFind__InMonth.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                typeCodeFind_RangeDate = Inventec.Common.Resource.Get.Value("UcViewEmrDocument.typeCodeFind_RangeDate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDaKe.Properties.Items.AddRange(new object[] {
             Inventec.Common.Resource.Get.Value("UCExecuteRoom.cboFind.All", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()),
            Inventec.Common.Resource.Get.Value("UCExecuteRoom.cboDaKe1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()),
             Inventec.Common.Resource.Get.Value("UCExecuteRoom.cboDaKe2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture())});

                this.cboInDebt.Properties.Items.AddRange(new object[] {
               Inventec.Common.Resource.Get.Value("UCExecuteRoom.cboFind.All", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()),
            Inventec.Common.Resource.Get.Value("UCExecuteRoom.cboInDebt1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()),
             Inventec.Common.Resource.Get.Value("UCExecuteRoom.cboInDebt2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture())});

                this.cboTreatmentType.Properties.Items.AddRange(new object[] {
             Inventec.Common.Resource.Get.Value("UCExecuteRoom.cboFind.All", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()),
             Inventec.Common.Resource.Get.Value("UCExecuteRoom.cboTreatmentType1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()),
            Inventec.Common.Resource.Get.Value("UCExecuteRoom.cboTreatmentType2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()),
             Inventec.Common.Resource.Get.Value("UCExecuteRoom.cboTreatmentType3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture())});
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetDataFromRam()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("GetDataFromRam.1");
                lstPatientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().ToList();
                patientTypeList = lstPatientType.Where(o => o.IS_NOT_USE_FOR_PATIENT != 1).OrderBy(o => o.PATIENT_TYPE_NAME).ToList();
                lstExecuteRoom = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().Where(o => o.ROOM_ID == this.roomId).ToList();
                lstPatientPrioty = BackendDataWorker.Get<HIS_PRIORITY_TYPE>().ToList();
                lstServiceRoom = BackendDataWorker.Get<HIS_SERVICE_ROOM>();
                deskList = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DESK>();
                Inventec.Common.Logging.LogSystem.Debug("GetDataFromRam.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ShowCheckBoxIsResult()
        {
            try
            {
                var roomExecute = lstExecuteRoom.Where(o => o.IS_TEST == 1).FirstOrDefault();
                if (roomExecute != null)
                {
                    chkIsResult.Visible = true;
                    chkIsResult.Enabled = true;
                    layoutControlItem40.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    chkIsResult.Enabled = false;
                    chkIsResult.Visible = false;
                    layoutControlItem40.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitTypeFind()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemInDateCode = new DXMenuItem(typeCodeFind__KeyWork_InDate, new EventHandler(btnCodeFind_Click));
                itemInDateCode.Tag = "InDate";
                menu.Items.Add(itemInDateCode);

                DXMenuItem itemInMonth = new DXMenuItem(typeCodeFind__InMonth, new EventHandler(btnCodeFind_Click));
                itemInMonth.Tag = "InMonth";
                menu.Items.Add(itemInMonth);

                DXMenuItem itemRangeDate = new DXMenuItem(typeCodeFind_RangeDate, new EventHandler(btnCodeFind_Click));
                itemRangeDate.Tag = "RangeDate";
                menu.Items.Add(itemRangeDate);

                btnCodeFind.DropDownControl = menu;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboSucKhoe()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisKskContractViewFilter filter = new HisKskContractViewFilter();
                filter.IS_ACTIVE = 1;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "CREATE_TIME";
                var listKskContract = new BackendAdapter(param).Get<List<HIS_KSK_CONTRACT>>("api/HisKskContract/GetView", ApiConsumers.MosConsumer, filter, param).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("KSK_CONTRACT_CODE", Inventec.Common.Resource.Get.Value("UCExecuteRoom.cboContract.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), 100, 1));
                // columnInfos.Add(new ColumnInfo("WORK_PLACE_NAME", "Tên công ty", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("KSK_CONTRACT_CODE", "ID", columnInfos, true, 100);
                ControlEditorLoader.Load(cboSucKhoe, listKskContract, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void FormatDtIntructionDate()
        {
            try
            {
                if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_InDate || this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_RangeDate)
                {
                    dtIntructionDate.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.Default;
                    dtIntructionDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtIntructionDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
                    dtIntructionDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtIntructionDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
                    dtIntructionDate.Properties.EditMask = "dd/MM/yyyy";
                    dtIntructionDate.Properties.Mask.EditMask = "dd/MM/yyyy";
                    dtIntructionDate.Properties.ShowClear = this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_RangeDate;
                }
                else if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind__InMonth)
                {
                    dtIntructionDate.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.YearView;
                    dtIntructionDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtIntructionDate.Properties.DisplayFormat.FormatString = "MM/yyyy";
                    dtIntructionDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtIntructionDate.Properties.EditFormat.FormatString = "MM/yyyy";
                    dtIntructionDate.Properties.EditMask = "MM/yyyy";
                    dtIntructionDate.Properties.Mask.EditMask = "MM/yyyy";
                    dtIntructionDate.Properties.ShowClear = false;
                }

                if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_RangeDate)
                {
                    lciIntructionDateTo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciNext.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciPreview.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                else
                {
                    lciIntructionDateTo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciNext.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciPreview.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCodeFind_Click(object sender, EventArgs e)
        {
            try
            {
                var btnMenuCodeFind = sender as DXMenuItem;
                btnCodeFind.Text = btnMenuCodeFind.Caption;
                this.typeCodeFind__KeyWork_InDate = btnMenuCodeFind.Caption;

                FormatDtIntructionDate();

                if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_InDate || this.typeCodeFind__KeyWork_InDate == this.typeCodeFind__InMonth)
                {
                    lciIntructionDateTo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                else
                {
                    lciIntructionDateTo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }

                dtIntructionDate.DateTime = DateTime.Now;
                dtIntructionDateTo.DateTime = DateTime.Now;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDefaultData()
        {
            try
            {
                btnCodeFind.Text = typeCodeFind__KeyWork_InDate;
                FormatDtIntructionDate();
                //dtCreatefrom.Properties.VistaDisplayMode = DefaultBoolean.True;
                //dtCreatefrom.Properties.VistaEditTime = DefaultBoolean.True;
                //dtCreateTo.Properties.VistaDisplayMode = DefaultBoolean.True;
                //dtCreateTo.Properties.VistaEditTime = DefaultBoolean.True;
                //dtCreatefrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0);
                //dtCreateTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0);
                dtIntructionDate.DateTime = DateTime.Now;
                dtIntructionDateTo.DateTime = DateTime.Now;
                cboFind.SelectedIndex = 0;
                cboTreatmentType.SelectedIndex = 0;
                cboDaKe.SelectedIndex = 0;
                cboInDebt.SelectedIndex = 0;
                lciExamTreatmentEndType.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciExamTreatmentResult.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciPhoneNumber.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciExamEndType.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                string configKeyCallPatientCPA = ConfigApplicationWorker.Get<string>(AppConfigKey.CONFIG_KEY__DANG_KY_TIEP_DON__GOI_BENH_NHAN_BANG_PHAN_MEM_CPA);
                if (configKeyCallPatientCPA != "1")
                {
                    txtGateNumber.Enabled = false;
                    txtStepNumber.Enabled = false;
                    btnRecallPatient.Enabled = false;
                }
                else
                {
                    txtGateNumber.Enabled = true;
                    txtStepNumber.Enabled = true;
                    btnRecallPatient.Enabled = true;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitGridCheckMarksSelectionPatientType()
        {
            try
            {
                this.cboPatientType.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboServiceGroup_CustomDisplayText);
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboPatientType.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(cboServiceGroup__SelectionChange);
                cboPatientType.Properties.Tag = gridCheck;
                cboPatientType.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboPatientType.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                    gridCheckMark.ClearSelection(cboPatientType.Properties.View);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceGroup_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.PATIENT_TYPE_NAME.ToString());
                }
                if (selectedPatientTypeList == null || selectedPatientTypeList.Count == 0 || patientTypeList.Count == gridCheckMark.Selection.Count)
                {
                    e.DisplayText = Inventec.Common.Resource.Get.Value("UCExecuteRoom.cboFind.All", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                }
                else
                {
                    e.DisplayText = sb.ToString();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceGroup__SelectionChange(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_PATIENT_TYPE> sgSelectedNews = new List<HIS_PATIENT_TYPE>();
                    foreach (MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.PATIENT_TYPE_NAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this.selectedPatientTypeList = new List<HIS_PATIENT_TYPE>();
                    this.selectedPatientTypeList.AddRange(sgSelectedNews);
                }

                //this.cboServiceGroup.Text = sb.ToString();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitControlState()
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(ModuleLinkName);
                LciGroupEmrDocument.Expanded = false;//mặc định lần đâu là ẩn đi
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == cboInDebt.Name)
                        {
                            cboInDebt.SelectedIndex = Convert.ToInt32(item.VALUE);
                        }
                        else if (item.KEY == chkScreenSaver.Name)
                        {
                            chkScreenSaver.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == LciGroupEmrDocument.Name)
                        {
                            LciGroupEmrDocument.Expanded = item.VALUE == "1";
                        }
                        else if (item.KEY == lcgPatientInfo.Name)
                        {
                            lcgPatientInfo.Expanded = item.VALUE == "1";
                        }
                        else if (item.KEY == lcgServiceReq.Name)
                        {
                            lcgServiceReq.Expanded = item.VALUE == "1";
                        }
                        else if (item.KEY == lcgTreeSereServ.Name)
                        {
                            lcgTreeSereServ.Expanded = item.VALUE == "1";
                        }
                        else if (item.KEY == txtGateNumber.Name)
                        {
                            txtGateNumber.Text = item.VALUE;
                        }
                        else if (item.KEY == txtStepNumber.Name)
                        {
                            txtStepNumber.Text = item.VALUE;
                        }
                        else if (item.KEY == ckKQCLS.Name)
                        {
                            ckKQCLS.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkCPA.Name)
                        {
                            chkCPA.Checked = item.VALUE == "1";
                        }
                    }
                }

                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                isNotLoadWhileChangeControlStateInFirst = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField("PATIENT_TYPE_CODE");
                col2.VisibleIndex = 1;
                col2.Width = 100;
                col2.Caption = "Mã đối tượng";
                DevExpress.XtraGrid.Columns.GridColumn col3 = cbo.Properties.View.Columns.AddField("PATIENT_TYPE_NAME");
                col3.VisibleIndex = 2;
                col3.Width = 200;
                col3.Caption = "Tên đối tượng";

                cbo.Properties.PopupFormWidth = 320;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = false;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadDataToPatientTypeCombo()
        {
            try
            {
                InitCombo(cboPatientType, patientTypeList, "PATIENT_TYPE_NAME", "ID");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateFindControl())
                {
                    return;
                }

                btnFind.Enabled = false;
                currentHisServiceReq = null;
                LoadPatientFromServiceReq(null);
                LoadSereServServiceReq(null);
                LoadTreeListSereServChild(null);
                FillDataToGridControl();
                InitEnableControl();
                btnFind.Enabled = true;
                LoadServiceReqCount(true, 0);
                LoadSereServCount();

                // mở màn hình xử lý khi nhập mã điều trị và chỉ có 1 bản ghi serviceReq
                if (serviceReqs != null && serviceReqs.Count == 1 && !String.IsNullOrWhiteSpace(txtServiceReqCode.Text))
                {
                    string configKeyCallPatientCPA = ConfigApplicationWorker.Get<string>(AppConfigKey.CONFIG_KEY__DANG_KY_TIEP_DON__GOI_BENH_NHAN_BANG_PHAN_MEM_CPA);
                    if (chkCPA.Checked == true)
                    {
                        List<ServiceReqADO> dataFind = new List<ServiceReqADO>();
                        dataFind = (List<ServiceReqADO>)gridViewServiceReq.DataSource;
                        if (dataFind != null && dataFind.Count == 1)
                        {
                            txtServiceReqCode.Focus();
                            txtServiceReqCode.SelectAll();
                        }
                        else
                        {
                            DoubleClickGridServiceReq();
                            txtServiceReqCode.Focus();
                            txtServiceReqCode.SelectAll();
                        }
                    }
                    else
                    {
                        DoubleClickGridServiceReq();
                        txtServiceReqCode.Focus();
                        txtServiceReqCode.SelectAll();
                    }
                }
                if (chkCPA.Checked == true)
                {
                    List<ServiceReqADO> dataFind = new List<ServiceReqADO>();
                    dataFind = (List<ServiceReqADO>)gridViewServiceReq.DataSource;
                    if (dataFind != null && dataFind.Count == 1)
                    {
                        gridViewServiceReq.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceReq_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ServiceReqADO dataRow = (ServiceReqADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];



                    if (e.Column.FieldName == "CallPatient")
                    {
                        if (dataRow.status == 11)
                        {
                            e.Value = imageListIcon.Images[11];

                        }
                        else if (dataRow.status == 12)
                        {
                            e.Value = imageListIcon.Images[12];

                        }
                        else if (dataRow.status == 13)
                        {
                            e.Value = imageListIcon.Images[13];

                        }
                        else
                        {
                            e.Value = imageListIcon.Images[14];
                        }

                    }
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + ((ucPaging1.pagingGrid.CurrentPage - 1) * ucPaging1.pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "TRANGTHAI_IMG")
                    {
                        //Chua xu ly: mau trang
                        //dang xu ly: mau vang
                        //Da ket thuc: mau den

                        long statusId = dataRow.SERVICE_REQ_STT_ID;
                        if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                        {
                            e.Value = imageListIcon.Images[0];
                        }
                        else if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                        {
                            e.Value = imageListIcon.Images[1];
                        }
                        else if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                        {
                            e.Value = imageListIcon.Images[4];
                        }
                        else
                        {
                            e.Value = imageListIcon.Images[0];
                        }
                    }
                    else if (e.Column.FieldName == "BUSY_COUNT_DISPLAY")
                    {
                        if (dataRow.IS_WAIT_CHILD.HasValue && dataRow.IS_WAIT_CHILD.Value == 1)
                        {
                            if (dataRow.IS_WAIT_CHILD.Value == 1)
                            {
                                e.Value = imageListIcon.Images[6];
                            }
                            else
                            {
                                e.Value = imageListIcon.Images[7];
                            }
                        }
                        else if (
                            dataRow.HAS_CHILD == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                            && ((dataRow.IS_WAIT_CHILD.HasValue && dataRow.IS_WAIT_CHILD.Value != 1) || !dataRow.IS_WAIT_CHILD.HasValue)
                            && dataRow.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL
                            )
                        {
                            e.Value = imageListIcon.Images[9];
                        }
                    }
                    else if (e.Column.FieldName == "TREATMENT_TYPE_ICON")
                    {
                        if (dataRow.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
                            e.Value = imageListIcon.Images[8];
                        else
                            e.Value = null;
                    }
                    else if (e.Column.FieldName == "PRIORIRY_DISPLAY")
                    {
                        decimal priority = (dataRow.PRIORITY ?? 0);
                        if ((priority == 1))
                        {
                            e.Value = imageListPriority.Images[0];
                        }
                    }
                    else if (e.Column.FieldName == "REQUEST_DATE_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.INTRUCTION_TIME);
                    }
                    //else if (e.Column.FieldName == "DOB_DISPLAY")
                    //{
                    //    e.Value = dataRow.TDL_PATIENT_DOB > 0 ? dataRow.TDL_PATIENT_DOB.ToString().Substring(0, 4) : null;
                    //}
                    else if (e.Column.FieldName == "INSTRUCTION_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.INTRUCTION_TIME);
                    }
                    //else if (e.Column.FieldName == "REQUEST_DEPARTMENT_DISPLAY")
                    //{
                    //    HIS_DEPARTMENT department = departments.FirstOrDefault(o => o.ID == dataRow.REQUEST_DEPARTMENT_ID);
                    //    e.Value = department != null ? department.DEPARTMENT_NAME : null;
                    //}
                    //else if (e.Column.FieldName == "REQUEST_ROOM_DISPLAY")
                    //{
                    //    V_HIS_ROOM room = rooms.FirstOrDefault(o => o.ID == dataRow.REQUEST_ROOM_ID);
                    //    e.Value = room != null ? room.ROOM_NAME : null;
                    //}

                    else if (e.Column.FieldName == "FINISH_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.FINISH_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "PATIENT_TYPE_NAME")
                    {
                        var PatientTypeName = lstPatientType.FirstOrDefault(o => o.ID == dataRow.TDL_PATIENT_TYPE_ID);
                        e.Value = PatientTypeName != null ? PatientTypeName.PATIENT_TYPE_NAME : null;
                    }
                    else if (e.Column.FieldName == "START_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.START_TIME ?? 0);
                    }

                    if (e.Column.FieldName == "EXAM_END_TYPE_STR")
                    {
                        if (dataRow.EXAM_END_TYPE == 1 && executeRoom.IS_EXAM == 1)
                        {
                            e.Value = imageListIcon.Images[10];

                        }
                        else
                            e.Value = null;
                    }
                }
                else
                {
                    e.Value = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewServiceReq_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Column.Caption == "Selection")
                    return;
                if (e.Column.FieldName == "CallPatient1")
                {

                }
                if (e.Column.FieldName == "CallPatient")
                {
                    CallPatient_gridViewServiceReq_ButtonClick();
                }
                else if (e.Column.FieldName == "PatientMissed")
                {
                    PatientMissed_gridViewServiceReq_ButtonClick();
                }
                else if (e.Column.FieldName == Gc_MedisoftHistory.FieldName)
                {
                    ServiceReqADO row = (ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    if (row != null)
                    {
                        InitDataADO ado = new InitDataADO();
                        ado.ProviderType = ProviderType.Medisoft;

                        ado.Patient = GetPatient(row);
                        if (ado.Patient != null)
                        {
                            ado.PatientId = ado.Patient.ID;
                        }

                        OtherTreatmentHistoryProcessor history = new OtherTreatmentHistoryProcessor(ado);
                        if (history != null)
                        {
                            history.Run(Library.OtherTreatmentHistory.Enum.XemCanLamSan);
                        }
                        //gridViewServiceReq.OptionsSelection.EnableAppearanceFocusedCell = true;
                        //gridViewServiceReq.OptionsSelection.EnableAppearanceFocusedRow = true;
                    }
                }
                else
                {
                    needHandleOnClick = true;
                    timerDoubleClick.Start();
                    if (e.Clicks == 1)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("gridViewServiceReq.ActiveEditor___");
                        if (gridViewServiceReq.ActiveEditor != null)
                        {
                            gridViewServiceReq.ActiveEditor.SelectAll();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DoubleClickGridServiceReq()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("gridViewServiceReq_RowCellClick. 6");
                gridViewServiceReq.OptionsSelection.EnableAppearanceFocusedCell = true;
                gridViewServiceReq.OptionsSelection.EnableAppearanceFocusedRow = true;
                currentHisServiceReq = (ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                //this.currentPatientTypeAlter = null;
                needHandleOnClick = false;
                btnExecuteByDoubleClick(null, null);

                Inventec.Common.Logging.LogSystem.Debug("gridViewServiceReq_RowCellClick. 7");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentHisServiceReq != null)
                {
                    if (currentHisServiceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                    {
                        CancelFinish(currentHisServiceReq);
                        return;
                    }
                    LoadModuleExecuteService(currentHisServiceReq);

                    CreateThreadCallPatientRefresh();

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentHisServiceReq), currentHisServiceReq));

                    InitEnableControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnExecuteByDoubleClick(object sender, EventArgs e)
        {
            try
            {
                LoadModuleExecuteService(currentHisServiceReq);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReLoadServiceReq(object data)
        {
            try
            {
                if (data != null && typeof(HIS_SERVICE_REQ) == data.GetType())
                {
                    HIS_SERVICE_REQ serviceReq = data as HIS_SERVICE_REQ;
                    ServiceReqADO serviceReqTemp = null;
                    foreach (var item in serviceReqs)
                    {
                        if (item.ID == serviceReq.ID)
                        {
                            Inventec.Common.Mapper.DataObjectMapper.Map<ServiceReqADO>(item, serviceReq);
                            serviceReqTemp = new ServiceReqADO(serviceReq);
                            break;
                        }
                    }

                    if (cboFind.SelectedIndex != 4 && serviceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                    {
                        serviceReqs.Remove(serviceReqTemp);
                    }
                    gridControlServiceReq.RefreshDataSource();
                    LoadServiceReqCount(true, 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAssignBlood_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisAssignBlood").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.SurgServiceReqExecute");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();

                    long intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                    AssignBloodADO assignBlood = new AssignBloodADO(this.currentHisServiceReq.TREATMENT_ID, intructionTime, this.currentHisServiceReq.ID);
                    moduleData.RoomId = roomId;
                    moduleData.RoomTypeId = roomTypeId;
                    listArgs.Add(assignBlood);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtSearchKey_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
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
                if (e.Info == null && e.SelectedControl == gridControlServiceReq)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlServiceReq.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;
                            string text = "";

                            if (info.Column.FieldName == "CallPatient")
                            {
                                long status = Inventec.Common.TypeConvert.Parse.ToInt64((view.GetRowCellValue(lastRowHandle, "status") ?? "").ToString());
                                if (status == 11)
                                {
                                    text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.ToolTipControl.DangGoiBenhNhan", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                                }
                                else if (status == 12)
                                {
                                    text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.ToolTipControl.GoiBenhNhanNho", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                                }
                                else if (status == 13)
                                {
                                    text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.ToolTipControl.DangGoiBenhNhanNho", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                                }
                                else
                                {
                                    text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.ToolTipControl.GoiBenhNhan", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                                }
                            }

                            if (info.Column.FieldName == "TRANGTHAI_IMG")
                            {
                                long serviceReqSTTId = Inventec.Common.TypeConvert.Parse.ToInt64((view.GetRowCellValue(lastRowHandle, "SERVICE_REQ_STT_ID") ?? "").ToString());
                                if (serviceReqSTTId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                                {
                                    text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.ToolTipControl.CXL", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                                }
                                else if (serviceReqSTTId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                                {
                                    text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.ToolTipControl.DXL", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                                }
                                else if (serviceReqSTTId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                                {
                                    text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.ToolTipControl.KT", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                                }

                            }
                            else if (info.Column.FieldName == "PRIORIRY_DISPLAY")
                            {
                                text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.ToolTipControl.PRIORIRY", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                            }
                            else if (info.Column.FieldName == "TREATMENT_TYPE_ICON")
                            {
                                long treatmentTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((view.GetRowCellValue(lastRowHandle, "TDL_TREATMENT_TYPE_ID") ?? "").ToString());
                                if (treatmentTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
                                    text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.ToolTipControl.TreatmentType_DtNgoaiTru", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());

                            }
                            else if (info.Column.FieldName == "BUSY_COUNT_DISPLAY")
                            {
                                string busyCount = (view.GetRowCellValue(lastRowHandle, "IS_WAIT_CHILD") ?? "").ToString();
                                if (busyCount == "1")
                                    text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.ToolTipControl.UnFinishCLS", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                                else
                                    if (busyCount == "0")
                                    text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.ToolTipControl.FinishCLS", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                                string hasChild = (view.GetRowCellValue(lastRowHandle, "HAS_CHILD") ?? "").ToString();
                                Inventec.Common.Logging.LogSystem.Debug(" busyCount: " + busyCount);
                                Inventec.Common.Logging.LogSystem.Debug(" hasChild: " + hasChild);
                                if (busyCount != "1" && hasChild == "1")
                                {
                                    text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.ToolTipControl.HaveResultCLS", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                                }
                            }
                            if (info.Column.FieldName == "EXAM_END_TYPE_STR")
                            {
                                text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.ToolTipControl.Kham", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
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

        private void btnAssignService_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignService").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignService");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    long intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;

                    AssignServiceADO assignServiceADO = new AssignServiceADO(currentHisServiceReq.TREATMENT_ID, intructionTime, this.currentHisServiceReq.ID);
                    moduleData.RoomId = roomId;
                    moduleData.RoomTypeId = roomTypeId;
                    listArgs.Add(assignServiceADO);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnAssignPrescription_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescription").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescription");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    long intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                    AssignPrescriptionADO assignPrescription = new AssignPrescriptionADO(currentHisServiceReq.TREATMENT_ID, intructionTime, this.currentHisServiceReq.ID);
                    moduleData.RoomId = roomId;
                    moduleData.RoomTypeId = roomTypeId;
                    listArgs.Add(assignPrescription);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnBordereau_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.Bordereau").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.Bordereau");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    moduleData.RoomId = roomId;
                    moduleData.RoomTypeId = roomTypeId;
                    listArgs.Add(currentHisServiceReq.TREATMENT_ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnBedRoomIn_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisBedRoomIn").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisBedRoomIn");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    listArgs.Add(currentHisServiceReq.TREATMENT_ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "SAR.Desktop.Plugins.SarPrintList").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = SAR.Desktop.Plugins.SarPrintList");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    SarPrintADO sarPrint = new SarPrintADO();
                    sarPrint.JSON_PRINT_ID = currentHisServiceReq.JSON_PRINT_ID;
                    //sarPrint.JsonPrintResult = JsonPrintResult;
                    listArgs.Add(sarPrint);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void JsonPrintResult(object data)
        {
            throw new NotImplementedException();
        }

        private void btnRoomTran_Click(object sender, EventArgs e)
        {
            try
            {
                //Kiem tra xem dich vu da bat dau hay chua

                if (currentHisServiceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                {
                    DialogResult myResult;
                    myResult = MessageBox.Show(String.Format(ResourceMessage.BanCoMuonHuyBatDauKhong), String.Format(ResourceMessage.XacNhanHuyBatDau), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (myResult == DialogResult.Yes)
                    {
                        if (!this.UnStartEvent())
                            return;
                    }
                }


                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ChangeExamRoomProcess").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ChangeExamRoomProcess");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    AutoMapper.Mapper.CreateMap<ServiceReqADO, L_HIS_SERVICE_REQ>();
                    L_HIS_SERVICE_REQ serviceReq = AutoMapper.Mapper.Map<ServiceReqADO, L_HIS_SERVICE_REQ>(this.currentHisServiceReq);
                    listArgs.Add(serviceReq);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSummaryInforTreatmentRecords_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SummaryInforTreatmentRecords").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.SummaryInforTreatmentRecords");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    listArgs.Add(this.currentHisServiceReq.TREATMENT_ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnTransaction_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.Transaction").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.Transaction");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAggrHospitalFees_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrHospitalFees").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AggrHospitalFees");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    listArgs.Add(this.currentHisServiceReq.TREATMENT_ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnTreatmentHistory_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentHistory").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TreatmentHistory");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();

                    HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                    treatmentFilter.ID = this.currentHisServiceReq.TREATMENT_ID;
                    V_HIS_TREATMENT treatment = new BackendAdapter(new CommonParam())
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, treatmentFilter, new CommonParam()).FirstOrDefault();
                    if (treatment != null)
                    {
                        TreatmentHistoryADO treatmentHistory = new TreatmentHistoryADO();
                        treatmentHistory.patientId = treatment.PATIENT_ID;
                        treatmentHistory.patient_code = treatment.TDL_PATIENT_CODE;
                        listArgs.Add(treatmentHistory);
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnTreatmentHistory2_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentHistory").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TreatmentHistory");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();

                    HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                    treatmentFilter.ID = this.currentHisServiceReq.TREATMENT_ID;
                    V_HIS_TREATMENT treatment = new BackendAdapter(new CommonParam())
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, treatmentFilter, new CommonParam()).FirstOrDefault();
                    if (treatment != null)
                    {
                        TreatmentHistoryADO treatmentHistory = new TreatmentHistoryADO();
                        treatmentHistory.treatmentId = treatment.ID;
                        treatmentHistory.treatment_code = treatment.TREATMENT_CODE;
                        listArgs.Add(treatmentHistory);
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDepositReq_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.RequestDeposit").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.RequestDeposit");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    listArgs.Add(this.currentHisServiceReq.TREATMENT_ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceReq_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                GridHitInfo hi = e.HitInfo;
                if (hi.InRowCell)
                {


                    int rowHandle = gridViewServiceReq.GetVisibleRowHandle(hi.RowHandle);
                    currentHisServiceReq = serviceReqRightClick = (ServiceReqADO)gridViewServiceReq.GetRow(rowHandle);
                    gridViewServiceReq.OptionsSelection.EnableAppearanceFocusedCell = true;
                    gridViewServiceReq.OptionsSelection.EnableAppearanceFocusedRow = true;
                    if (barManager1 == null)
                    {
                        barManager1 = new BarManager();
                        barManager1.Form = this;
                    }
                    // List<HIS.Desktop.Plugins.ExecuteRoom.ADO.ServiceReqADO> CheckServiceExecuteGroup = new List<HIS.Desktop.Plugins.ExecuteRoom.ADO.ServiceReqADO>();
                    CheckServiceExecuteGroup = new List<ServiceReqADO>();
                    if (gridViewServiceReq.GetSelectedRows().Count() > 0)
                    {

                        int select = gridViewServiceReq.GetSelectedRows().Count();
                        var data = gridViewServiceReq.GetSelectedRows();
                        if (data != null && data.Count() > 0)
                        {
                            foreach (var i in data)
                            {
                                var row = (HIS.Desktop.Plugins.ExecuteRoom.ADO.ServiceReqADO)gridViewServiceReq.GetRow(i);
                                CheckServiceExecuteGroup.Add(row);
                            }
                        }
                    }
                    if (this.emrMenuPopupProcessor == null) this.emrMenuPopupProcessor = new Library.FormMedicalRecord.MediRecordMenuPopupProcessor();
                    executeRoomPopupMenuProcessor = new ExecuteRoomPopupMenuProcessor(serviceReqRightClick, ExecuteRoomMouseRight_Click, barManager1, roomId, CheckServiceExecuteGroup, this.emrMenuPopupProcessor);
                    executeRoomPopupMenuProcessor.InitMenu();
                    isEventPopupMenuShowing = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnUnStart_Click(object sender, EventArgs e)
        {
            try
            {
                LogTheadInSessionInfo(() => btnUnStart_Click_Action(sender, e), "btnUnStart_Click");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnUnStart_Click_Action(object sender, EventArgs e)
        {
            try
            {
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                CommonParam param = new CommonParam();
                bool success = false;
                WaitingManager.Show();
                var serviceReq = new BackendAdapter(param)
                    .Post<MOS.EFMODEL.DataModels.L_HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_UNSTART, ApiConsumers.MosConsumer, currentHisServiceReq.ID, param);
                if (serviceReq != null && serviceReq.ID > 0)
                {
                    long dtFrom = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd") + "000000");
                    long dtTo = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd") + "232359");

                    if (dtFrom <= serviceReq.INTRUCTION_TIME && serviceReq.INTRUCTION_TIME <= dtTo)
                    {
                        if (HisConfigCFG.RequestLimitWarningOption == "2")
                        {
                            if (desk != null && currentHisServiceReq != null && currentHisServiceReq.EXE_DESK_ID == desk.ID)
                            {
                                LoadServiceReqCount(false, -1);
                            }
                        }
                        else
                        {
                            if (currentHisServiceReq != null && !String.IsNullOrWhiteSpace(currentHisServiceReq.EXECUTE_LOGINNAME))
                            {
                                if (currentHisServiceReq.EXECUTE_LOGINNAME.Equals(loginName))
                                {
                                    LoadServiceReqCount(false, -1);
                                }
                            }
                        }

                    }
                    success = true;
                    btnUnStart.Enabled = false;
                    //Reload data

                    foreach (var item in serviceReqs)
                    {
                        if (item.ID == currentHisServiceReq.ID)
                        {
                            item.SERVICE_REQ_STT_ID = serviceReq.SERVICE_REQ_STT_ID;
                            currentHisServiceReq.SERVICE_REQ_STT_ID = serviceReq.SERVICE_REQ_STT_ID;
                        }
                    }

                    gridControlServiceReq.RefreshDataSource();

                    LoadSereServCount();
                }

                WaitingManager.Hide();

                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool UnStartEvent()
        {
            bool result = false;
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                WaitingManager.Show();
                var serviceReq = new BackendAdapter(param)
                    .Post<MOS.EFMODEL.DataModels.L_HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_UNSTART, ApiConsumers.MosConsumer, currentHisServiceReq.ID, param);
                if (serviceReq != null && serviceReq.ID > 0)
                {
                    result = true;
                    success = true;
                    btnUnStart.Enabled = false;
                    //Reload data

                    foreach (var item in serviceReqs)
                    {
                        if (item.ID == currentHisServiceReq.ID)
                        {
                            item.SERVICE_REQ_STT_ID = serviceReq.SERVICE_REQ_STT_ID;
                            currentHisServiceReq.SERVICE_REQ_STT_ID = serviceReq.SERVICE_REQ_STT_ID;
                        }
                    }

                    gridControlServiceReq.RefreshDataSource();
                }

                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void btnServiceReqList_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ServiceReqList").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ServiceReqList");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    HIS_TREATMENT treatment = new HIS_TREATMENT();
                    treatment.ID = currentHisServiceReq.TREATMENT_ID;
                    listArgs.Add(treatment);
                    listArgs.Add(currentHisServiceReq.TREATMENT_ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSereServServiceReq_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                SereServ6ADO sereServ = (SereServ6ADO)gridViewSereServServiceReq.GetRow(e.RowHandle);
                if (sereServ != null && sereServ.IS_NO_EXECUTE != null)
                {
                    e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
                }
                else if (sereServ != null && sereServ.SereServExt != null && sereServ.SereServExt.NUMBER_OF_FILM.HasValue && sereServ.SereServExt.NUMBER_OF_FILM.Value > 0)
                {
                    e.Appearance.ForeColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceReq_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                ServiceReqADO serviceReq = (ServiceReqADO)gridViewServiceReq.GetRow(e.RowHandle);
                if (serviceReq != null)
                {
                    if (serviceReq.PRIORITY != null && serviceReq.PRIORITY == 1)
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);

                    if (!string.IsNullOrWhiteSpace(serviceReq.DISPLAY_COLOR))
                    {
                        List<int> parentBackColorCodes = GetColorValues(serviceReq.DISPLAY_COLOR);
                        if (parentBackColorCodes != null && parentBackColorCodes.Count >= 3)
                        {
                            e.Appearance.ForeColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                        }
                    }
                    else if (!String.IsNullOrEmpty(serviceReq.TDL_HEIN_CARD_NUMBER)) //nambg
                        e.Appearance.ForeColor = System.Drawing.Color.Blue; //nambg
                    else if (serviceReq.TDL_PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__KSK)
                        e.Appearance.ForeColor = System.Drawing.Color.Green;
                    //if (serviceReq.IS_CHRONIC.HasValue && serviceReq.IS_CHRONIC == 1)
                    //    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Italic);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<int> GetColorValues(string rgbCode)
        {
            List<int> result = new List<int>();
            try
            {
                if (!String.IsNullOrWhiteSpace(rgbCode))
                {
                    result = new List<int>();
                    string[] Codes = rgbCode.Split(',');
                    foreach (var item in Codes)
                    {
                        result.Add(Inventec.Common.TypeConvert.Parse.ToInt32(item));
                    }

                    if (result.Count < 3)
                    {
                        int rsCount = result.Count;
                        while (rsCount < 4)
                        {
                            rsCount++;
                            result.Add(0);
                        }
                    }
                }
                else
                {
                    result.Add(0);
                    result.Add(0);
                    result.Add(0);
                }
            }
            catch (Exception ex)
            {
                //màu đen
                result = new List<int>();
                result.Add(0);
                result.Add(0);
                result.Add(0);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void txtGateNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtStepNumber.Focus();
                    txtStepNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCallPatient_Click(object sender, EventArgs e)
        {
            try
            {
                LogTheadInSessionInfo(() => btnCallPatient_Click_Action(sender, e), "btnCallPatient_Click");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCallPatient_Click_Action(object sender, EventArgs e)
        {
            try
            {
                if (CheckListCPA != null && CheckListCPA.Count > 0)
                {
                    ThreadCallPatientRefresh(CheckListCPA);
                }
                string configKeyCallPatientCPA = ConfigApplicationWorker.Get<string>(AppConfigKey.CONFIG_KEY__DANG_KY_TIEP_DON__GOI_BENH_NHAN_BANG_PHAN_MEM_CPA);
                if (chkCPA.Checked == true && configKeyCallPatientCPA != "1")
                {
                    Thread t2 = new Thread(delegate ()
                    {
                        List<ServiceReqADO> listSelection = new List<ServiceReqADO>();
                        var rowHandles = gridViewServiceReq.GetSelectedRows();
                        if (rowHandles != null && rowHandles.Count() > 0)
                        {
                            foreach (var i in rowHandles)
                            {
                                var row = (ServiceReqADO)gridViewServiceReq.GetRow(i);
                                if (row != null)
                                {
                                    listSelection.Add(row);
                                }
                            }
                        }
                        var executeRoom = lstExecuteRoom.FirstOrDefault(o => o.ROOM_ID == roomId);
                        string executeRoomName = executeRoom != null ? executeRoom.EXECUTE_ROOM_NAME : null;
                        long[] id_ = new long[listSelection.Count];
                        for (int runs = 0; runs < listSelection.Count; runs++)
                        {
                            id_[runs] = listSelection[runs].ID;
                        }
                        CheckListCPA = id_.ToList();
                        CreateThreadCallPatientCountServiceGrid(id_);
                        CallPatientChkCPA(executeRoom, listSelection);

                    });
                    t2.Start();

                }
                else
                {
                    CreateThreadCallPatient();
                }

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
                LogTheadInSessionInfo(() => btnRecallPatient_Click_Action(sender, e), "btnRecallPatient_Click");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRecallPatient_Click_Action(object sender, EventArgs e)
        {
            try
            {
                if (!btnCallPatient.Enabled)
                    return;
                if (String.IsNullOrWhiteSpace(txtGateNumber.Text) || String.IsNullOrEmpty(txtStepNumber.Text))
                    return;
                string configKeyCallPatientCPA = ConfigApplicationWorker.Get<string>(AppConfigKey.CONFIG_KEY__DANG_KY_TIEP_DON__GOI_BENH_NHAN_BANG_PHAN_MEM_CPA);
                if (configKeyCallPatientCPA == "1")
                {
                    if (CheckListCPA != null && CheckListCPA.Count > 0)
                    {
                        ThreadCallPatientRefresh(CheckListCPA);
                    }
                    if (this.clienttManager == null)
                        this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager();
                    this.clienttManager.RecallNumOrderString(txtGateNumber.Text.Trim(), int.Parse(txtStepNumber.Text));
                    long[] id = this.clienttManager.GetCurrentPatientCall(txtGateNumber.Text.Trim(), false);
                    Inventec.Common.Logging.LogSystem.Info("This.clienttManager.GetCurrentPatientCall ____Goi F7: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => id), id));
                    txtCallPatientCPA.ForeColor = Color.Black;
                    lcCkhCPA.AppearanceItemCaption.ForeColor = Color.Black;
                    foreach (var item in id)
                    {
                        var data = serviceReqs.FirstOrDefault(o => o.ID == item);
                        if (data != null)
                        {
                            if (data.CALL_COUNT.HasValue)
                            {
                                if (data.CALL_COUNT >= 1)
                                {
                                    data.status = 13;
                                }
                                else
                                {
                                    data.status = 11;
                                }
                            }
                            else
                            {
                                data.status = 11;
                            }
                        }
                        gridControlServiceReq.RefreshDataSource();
                    }

                    CheckListCPA = id.ToList();
                    CreateThreadCallPatientCountService(id);

                    //Thread t2 = new Thread(delegate()
                    //{
                    //    CreateThreadCallPatientCountService(id);
                    //    Thread.Sleep(10000);

                    //    this.BeginInvoke(new MethodInvoker(delegate
                    //    {
                    //        //FillDataToGridControl();
                    //    }));
                    //});
                    //t2.Start();
                }
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
                LogTheadInSessionInfo(timer1_Tick_Action, "SelectServiceReq");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timer1_Tick_Action()
        {
            timerDoubleClick.Stop();
            if (needHandleOnClick)
            {
                if (!isEventPopupMenuShowing)
                {
                    sereServ7s = null;
                    gridViewServiceReq.OptionsSelection.EnableAppearanceFocusedCell = true;
                    gridViewServiceReq.OptionsSelection.EnableAppearanceFocusedRow = true;
                    currentHisServiceReq = (ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    LoadDataToPanelRight(currentHisServiceReq);
                    InitEnableControl();
                    SetTextButtonExecute(currentHisServiceReq);

                }
                isEventPopupMenuShowing = false;
            }
            needHandleOnClick = true;
        }

        private void cboFind_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (CPALoad == true)
                {
                    btnFind_Click(null, null);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSereServServiceReq_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                SereServ6ADO sereServ6 = gridViewSereServServiceReq.GetFocusedRow() as SereServ6ADO;
                if (sereServ6 != null)
                {
                    if (sereServ6.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA
                    || sereServ6.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS
                    || sereServ6.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN
                    || sereServ6.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA)
                    {
                        this._sereServRowMenu = (V_HIS_SERE_SERV_6)gridViewSereServServiceReq.GetFocusedRow();
                        this.frmShow();
                    }
                    //this.PacsCallModuleProccess(sereServ6);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSereServServiceReq_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    SereServ6ADO dataRow = (SereServ6ADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow == null) return;
                    if (dataRow.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA
                    || dataRow.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS
                    || dataRow.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN
                    || dataRow.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA)
                    {
                        if (e.Column.FieldName == "SO_PHIEU")
                        {
                            e.Value = this.GetSoPhieu(dataRow.TDL_SERVICE_REQ_CODE, dataRow.TDL_SERVICE_CODE);
                        }
                    }

                    if (e.Column.FieldName == "HasExt")
                    {
                        if (dataRow.SereServExt != null && (!String.IsNullOrWhiteSpace(dataRow.SereServExt.NOTE) || !String.IsNullOrWhiteSpace(dataRow.SereServExt.CONCLUDE) || dataRow.SereServExt.BEGIN_TIME != null))
                        {
                            e.Value = imageListIcon.Images[1];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timerAutoReload_Tick()
        {
            try
            {

                TimeRemainDisplay();
                if (timeCount == maxTimeReload)
                {
                    StopTimer(currentModule.ModuleLink, "timerAutoReload");
                    btnFind_Click(null, null);
                    return;
                }
                timeCount = timeCount + 1000;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TimeRemainDisplay()
        {
            try
            {
                long timeRemain = maxTimeReload / 1000 - ((timeCount) / 1000);
                if (timeRemain > 0)
                {
                    lblAutoReload.Text = String.Format("{0}", timeRemain);
                    lblAutoReload.ToolTip = String.Format("Tự động tải lại sau {0} giây", timeRemain);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkAutoReload_CheckedChanged(object sender, EventArgs e)
        {
            CheckEdit editor = sender as CheckEdit;
            if (editor != null && editor.EditorContainsFocus)
            {
                btnFind_Click(null, null);
            }
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                switch ((EnumUtil.REFESH_ENUM)btnRefesh.Tag)
                {
                    case EnumUtil.REFESH_ENUM.ON:
                        btnRefesh.Image = imageListRefesh.Images[0];
                        btnRefesh.Tag = EnumUtil.REFESH_ENUM.OFF;
                        btnFind_Click(null, null);
                        break;
                    default:
                        btnRefesh.Image = imageListRefesh.Images[1];
                        btnRefesh.Tag = EnumUtil.REFESH_ENUM.ON;
                        StopTimer(currentModule.ModuleLink, "timerAutoReload");
                        timeCount = 0;
                        lblAutoReload.Text = "";
                        break;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CreateThreadCallPatientRefresh()
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(CallPatientNewThreadRefresh));
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

        private void CallPatientNewThreadRefresh()
        {
            try
            {
                Refesh_();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Refesh_()
        {

            string configKeyCallPatientCPA = ConfigApplicationWorker.Get<string>(AppConfigKey.CONFIG_KEY__DANG_KY_TIEP_DON__GOI_BENH_NHAN_BANG_PHAN_MEM_CPA);
            if (configKeyCallPatientCPA == "1")
            {
                if (this.clienttManager == null)
                    this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager();
                List<CPA.WCFClient.CallPatientClient.ADO.CallPatientInfoADO> listData = new List<CPA.WCFClient.CallPatientClient.ADO.CallPatientInfoADO>();

                if (serviceReqs != null && serviceReqs.Count() > 0)
                {
                    foreach (var item in serviceReqs)
                    {
                        CPA.WCFClient.CallPatientClient.ADO.CallPatientInfoADO CallPatientInfoADO_ = new CPA.WCFClient.CallPatientClient.ADO.CallPatientInfoADO();
                        CallPatientInfoADO_.Dob = item.TDL_PATIENT_DOB;
                        CallPatientInfoADO_.NumOrder = item.NUM_ORDER ?? 0;
                        CallPatientInfoADO_.ServiceReqId = item.ID;
                        CallPatientInfoADO_.PatientName = item.TDL_PATIENT_NAME;
                        if (item.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL || (item.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL && item.IS_WAIT_CHILD != 1 && item.HAS_CHILD == 1))
                        {
                            //if (item.IS_WAIT_CHILD != 1 && item.HAS_CHILD == 1)
                            //{
                            if (item.CALL_COUNT > 0) //item.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL && 
                            {
                                CallPatientInfoADO_.PatientType = 2;
                            }
                            else
                            {
                                CallPatientInfoADO_.PatientType = 1;
                            }
                        }
                        else
                        {

                            CallPatientInfoADO_.PatientType = 0;
                        }
                        if (item.IS_WAIT_CHILD == null && item.HAS_CHILD == 1)
                        {
                            CallPatientInfoADO_.Note = "Có KQ";
                            CallPatientInfoADO_.ExecuteTime = item.RESULTING_TIME ?? 0;
                        }
                        else
                        {
                            CallPatientInfoADO_.Note = "";
                            CallPatientInfoADO_.ExecuteTime = item.INTRUCTION_TIME;
                        }


                        if (item.PRIORITY_TYPE_ID.HasValue)
                        {
                            //TH ưu tiên: <Tên đối tượng ưu tiên>"
                            CallPatientInfoADO_.PriorityNote = lstPatientPrioty.FirstOrDefault(o => o.ID == item.PRIORITY_TYPE_ID).PRIORITY_TYPE_NAME;
                        }
                        listData.Add(CallPatientInfoADO_);
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("Refresh__________Data:  " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listData), listData));
                this.clienttManager.UpdateListPatientInfoCalling(txtGateNumber.Text.Trim(), listData);
            }
        }
        private void gridViewSereServServiceReq_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                GridHitInfo hi = e.HitInfo;
                this._sereServRowMenu = null;
                if (hi.InRowCell)
                {
                    this._sereServRowMenu = (SereServ6ADO)gridViewSereServServiceReq.GetFocusedRow();
                    if (this.baManagerMenu == null)
                    {
                        this.baManagerMenu = new BarManager();
                        this.baManagerMenu.Form = this;
                    }

                    //if (this._sereServRowMenu != null && currentHisServiceReq != null && currentHisServiceReq.SERVICE_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                    //{
                    this.popupMenuProcessor = new PopupMenuProcessor(this._sereServRowMenu, this.baManagerMenu, MouseRightItemClick, currentHisServiceReq);
                    this.popupMenuProcessor.InitMenu();
                    //}
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
                if ((e.Item is BarButtonItem) && this._sereServRowMenu != null)
                {
                    var type = (PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case PopupMenuProcessor.ItemType.NhapThongTinPhim:
                            this.frmShow();
                            break;
                        case PopupMenuProcessor.ItemType.KeThuocVatTu:
                            this.frmShowKeThuocVatTu();
                            break;
                        case PopupMenuProcessor.ItemType.DoiDichVu:
                            this.frmShowDoiDichVu();
                            break;
                        case PopupMenuProcessor.ItemType.ChonMayXuLy:
                            this.frmShowChonMayXuLy();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void frmShowChonMayXuLy()
        {
            try
            {
                if (gridViewSereServServiceReq.GetSelectedRows() == null || gridViewSereServServiceReq.GetSelectedRows().Length == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn dịch vụ.");
                    return;
                }
                List<long> sereServId = new List<long>();
                foreach (var item in gridViewSereServServiceReq.GetSelectedRows())
                {
                    var row = ((SereServ6ADO)gridViewSereServServiceReq.GetRow(item));
                    sereServId.Add(row.ID);
                }
                if (currentHisServiceReq != null)
                {
                    frmMachine frm = new frmMachine(GetResult, new List<long>() { currentHisServiceReq.ID }, sereServId);
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetResult(bool isSuccess)
        {
            try
            {
                if (isSuccess)
                {
                    LoadSereServServiceReq(currentHisServiceReq);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmShowDoiDichVu()
        {
            try
            {
                if (currentHisServiceReq != null)
                {
                    var row = (SereServ6ADO)gridViewSereServServiceReq.GetFocusedRow();
                    if (row != null)
                    {
                        var formDoiDv = new ReqChangeService.SwapService(currentModuleBase, currentHisServiceReq, row, lstPatientType);
                        if (formDoiDv != null)
                        {
                            formDoiDv.ShowDialog();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmShow()
        {
            try
            {
                this.frmShowKeThuocVatTu();
                //HIS.Desktop.Plugins.ExecuteRoom.Design.frmNumberFilmInput frm = new HIS.Desktop.Plugins.ExecuteRoom.Design.frmNumberFilmInput(_sereServRowMenu, (Inventec.Desktop.Plugins.ExecuteRoom.Delegate.RefeshDataExt)UpdateStatus);
                //frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmShowKeThuocVatTu()
        {
            try
            {
                V_HIS_SERE_SERV sereServInput = new V_HIS_SERE_SERV();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV>(sereServInput, _sereServRowMenu);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServInput), sereServInput));

                List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ> serviceReqDonEdits = new List<HIS_SERVICE_REQ>();
                int PRESCRIPTION_TYPE_ID__CLS = 3;
                HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                CommonParam param = new CommonParam();
                serviceReqFilter.TREATMENT_ID = currentHisServiceReq.TREATMENT_ID;
                serviceReqFilter.PARENT_ID = sereServInput.SERVICE_REQ_ID;
                var serviceReqDons = new BackendAdapter(param)
                  .Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, serviceReqFilter, param);

                serviceReqDons = serviceReqDons != null ? serviceReqDons.Where(o => o.PRESCRIPTION_TYPE_ID == PRESCRIPTION_TYPE_ID__CLS && o.SERVICE_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT).ToList() : null;

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("serviceReqDons.count", serviceReqDons != null ? serviceReqDons.Count : 0) + Inventec.Common.Logging.LogUtil.TraceData("currentHisServiceReq.TREATMENT_ID", currentHisServiceReq.TREATMENT_ID) + Inventec.Common.Logging.LogUtil.TraceData("currentHisServiceReq.SERVICE_REQ_ID", currentHisServiceReq.ID));

                if (serviceReqDons != null && serviceReqDons.Count > 0)
                {
                    foreach (var itemSR in serviceReqDons)
                    {
                        var expMedius = GetExpMestMedicineByExpMestId(itemSR.ID);
                        var expMadeus = GetExpMestMaterialByExpMestId(itemSR.ID);

                        bool isExistsMedi = (expMedius != null && expMedius.Count > 0 && expMedius.Any(o => o.SERE_SERV_PARENT_ID == sereServInput.ID));
                        bool isExistsMate = (expMadeus != null && expMadeus.Count > 0 && expMadeus.Any(o => o.SERE_SERV_PARENT_ID == sereServInput.ID));
                        if (isExistsMedi || isExistsMate)
                        {
                            serviceReqDonEdits.Add(itemSR);
                        }
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("serviceReqDonEdits.count", serviceReqDonEdits != null ? serviceReqDonEdits.Count : 0));
                if (serviceReqDonEdits == null || serviceReqDonEdits.Count == 0 || (serviceReqDonEdits != null && serviceReqDonEdits.Count == 1))
                {
                    CallAssignPrescriptionCLS(serviceReqDonEdits, sereServInput);
                }
                else
                {
                    #region Nhiều hơn 1 y lệnh
                    DialogResult quesMessage = DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.DichVuDaDuocKeVoiYLenh, serviceReqDonEdits.OrderByDescending(o => o.ID).FirstOrDefault().SERVICE_REQ_CODE), "Thông báo", MessageBoxButtons.YesNoCancel);
                    switch (quesMessage)
                    {
                        case DialogResult.None:
                        case DialogResult.Cancel:
                            break;
                        case DialogResult.Yes:
                            CallAssignPrescriptionCLS(new List<HIS_SERVICE_REQ>() { serviceReqDonEdits.OrderByDescending(o => o.ID).FirstOrDefault() }, sereServInput);
                            break;
                        case DialogResult.No:
                            CallAssignPrescriptionCLS(null, sereServInput);
                            break;
                        default:
                            break;
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CallAssignPrescriptionCLS(List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ> serviceReqDonEdits, V_HIS_SERE_SERV sereServInput)
        {
            try
            {
                if (serviceReqDonEdits != null && serviceReqDonEdits.Count == 1)
                {
                    #region Sua don thuoc
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionCLS").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionCLS");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        HIS.Desktop.ADO.AssignPrescriptionADO assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionADO(currentHisServiceReq.TREATMENT_ID, currentHisServiceReq.INTRUCTION_TIME, currentHisServiceReq.ID, sereServInput);

                        var pres = serviceReqDonEdits[0];
                        assignServiceADO.IsCabinet = true;
                        assignServiceADO.TreatmentCode = pres.TDL_TREATMENT_CODE;
                        assignServiceADO.GenderName = pres.TDL_PATIENT_GENDER_NAME;
                        assignServiceADO.PatientDob = pres.TDL_PATIENT_DOB;
                        assignServiceADO.PatientName = pres.TDL_PATIENT_NAME;
                        assignServiceADO.PatientId = pres.TDL_PATIENT_ID;
                        assignServiceADO.TreatmentId = currentHisServiceReq.TREATMENT_ID;
                        assignServiceADO.IsAutoCheckExpend = true;
                        assignServiceADO.DgProcessDataResult = DlgProcessDataResult;
                        AssignPrescriptionEditADO assignEditADO = null;

                        HisExpMestFilter expfilter = new HisExpMestFilter();
                        expfilter.SERVICE_REQ_ID = pres.ID;
                        var expMests = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumer.ApiConsumers.MosConsumer, expfilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                        if (expMests != null && expMests.Count == 1)
                        {
                            var expMest = expMests.FirstOrDefault();
                            assignEditADO = new AssignPrescriptionEditADO(pres, expMest, null);
                        }
                        else
                        {
                            assignEditADO = new AssignPrescriptionEditADO(pres, null, null);
                        }
                        assignServiceADO.AssignPrescriptionEditADO = assignEditADO;

                        listArgs.Add(assignServiceADO);
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();
                    }
                    #endregion
                }
                else if (serviceReqDonEdits == null || serviceReqDonEdits.Count == 0)
                {
                    #region Ke don thuoc
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionCLS").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionCLS");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        AssignPrescriptionADO assignPrescription = new AssignPrescriptionADO(currentHisServiceReq.TREATMENT_ID, currentHisServiceReq.INTRUCTION_TIME, currentHisServiceReq.ID, sereServInput);
                        assignPrescription.IsCabinet = true;
                        assignPrescription.TreatmentCode = currentHisServiceReq.TDL_TREATMENT_CODE;
                        assignPrescription.TreatmentId = currentHisServiceReq.TREATMENT_ID;
                        assignPrescription.HeinCardnumber = currentHisServiceReq.TDL_HEIN_CARD_NUMBER;
                        assignPrescription.GenderName = currentHisServiceReq.TDL_PATIENT_GENDER_NAME;
                        assignPrescription.PatientName = currentHisServiceReq.TDL_PATIENT_NAME;
                        assignPrescription.PatientDob = currentHisServiceReq.TDL_PATIENT_DOB;
                        assignPrescription.IsAutoCheckExpend = true;
                        assignPrescription.DgProcessDataResult = DlgProcessDataResult;

                        listArgs.Add(assignPrescription);
                        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.AssignPrescriptionCLS", this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId, listArgs);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MEDICINE> GetExpMestMedicineByExpMestId(long serviceReqId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExpMestMedicineFilter searchMedicineFilter = new HisExpMestMedicineFilter();
                searchMedicineFilter.ORDER_DIRECTION = "ASC";
                searchMedicineFilter.ORDER_FIELD = "ID";
                searchMedicineFilter.TDL_SERVICE_REQ_ID = serviceReqId;
                return new BackendAdapter(param).Get<List<HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GET, ApiConsumers.MosConsumer, searchMedicineFilter, ProcessLostToken, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        private List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_MATERIAL> GetExpMestMaterialByExpMestId(long serviceReqId)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExpMestMaterialFilter searchMaterialFilter = new HisExpMestMaterialFilter();
                searchMaterialFilter.ORDER_DIRECTION = "ASC";
                searchMaterialFilter.ORDER_FIELD = "ID";
                searchMaterialFilter.TDL_SERVICE_REQ_ID = serviceReqId;
                return new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GET, ApiConsumers.MosConsumer, searchMaterialFilter, ProcessLostToken, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        private void DlgProcessDataResult(object data)
        {
            try
            {
                if (data == null)
                {
                    throw new ArgumentNullException("Du lieu ket qua truyen tu chuc nang ke don sang null: data is null");
                }

                if (currentHisServiceReq != null && currentHisServiceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                {
                    foreach (var item in serviceReqs)
                    {
                        if (item.ID == currentHisServiceReq.ID)
                        {
                            item.SERVICE_REQ_STT_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL;
                            currentHisServiceReq.SERVICE_REQ_STT_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL;
                        }
                    }

                    gridControlServiceReq.RefreshDataSource();
                    SetTextButtonExecute(currentHisServiceReq);

                    //Chưa kết thúc 0
                    //Tất cả 1
                    //Chưa xử lý 2
                    //Đang xử lý 3
                    //Kết thúc 4
                    if (cboFind.SelectedIndex == 2)
                    {

                        LoadServiceReqCount(false, -1);
                        serviceReqs.Remove(currentHisServiceReq);
                    }
                    else if (cboFind.SelectedIndex == 0 || cboFind.SelectedIndex == 1)
                    {

                        LoadServiceReqCount(false, 1);
                    }
                    LoadSereServCount();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateStatus(HIS_SERE_SERV_EXT ado)
        {
            try
            {
                bool isStart = false;
                if (currentHisServiceReq != null
                    && currentHisServiceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                    StartEvent(ref isStart, currentHisServiceReq);
                else
                    isStart = true;

                if (isStart == false)
                    return;
                btnUnStart.Enabled = true;

                string uri = (ado != null && ado.ID > 0) ? "/api/HisSereServExt/Update" : "/api/HisSereServExt/Create";
                CommonParam param = new CommonParam();
                bool success = false;
                var result = new BackendAdapter(param).Post<HIS_SERE_SERV_EXT>(uri, ApiConsumers.MosConsumer, ado, param);
                if (result != null)
                {
                    success = true;
                    foreach (var item in sereServ6s)
                    {
                        if (item.ID == result.SERE_SERV_ID)
                        {
                            item.SereServExt = result;
                            break;
                        }
                    }

                    gridControlSereServServiceReq.RefreshDataSource();
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void toolTipController2_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            if (e.Info == null && e.SelectedControl == gridControlSereServServiceReq)
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = gridControlSereServServiceReq.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                if (info.InRowCell)
                {
                    if (lastRowHandleSS != info.RowHandle || lastColumnSS != info.Column)
                    {
                        lastColumnSS = info.Column;
                        lastRowHandleSS = info.RowHandle;
                        string text = "";
                        if (info.Column.FieldName == "HasExt")
                        {

                            var ext = view.GetRowCellValue(lastRowHandleSS, "SereServExt");
                            if (ext != null)
                            {
                                if (ext.GetType() == typeof(SereServ6ADO))
                                {
                                    var dataRow = (SereServ6ADO)ext;
                                    if (!String.IsNullOrWhiteSpace(dataRow.SereServExt.NOTE) || !String.IsNullOrWhiteSpace(dataRow.SereServExt.CONCLUDE) || dataRow.SereServExt.BEGIN_TIME != null)
                                    {
                                        text = "Đã xử lý";
                                    }
                                }
                            }
                        }
                        lastInfoSS = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                    }
                    e.Info = lastInfoSS;
                }
            }
        }

        private void gridViewPatientType_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {

        }

        private void gridViewPatientType_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                HIS_PATIENT_TYPE patientType = (HIS_PATIENT_TYPE)gridViewPatientType.GetRow(e.RowHandle);
                if (patientType != null)
                {
                    //if (patientType.ID == HisConfigCFG.PatientTypeId__KSK)
                    //    e.Appearance.ForeColor = System.Drawing.Color.Blue; 
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timerReloadMachineCounter_Tick()
        {
            try
            {
                LogSystem.Debug("timerReloadMachineCounter_Tick.1");
                ReloadMachineCounter();
                LogSystem.Debug("timerReloadMachineCounter_Tick.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {

            try
            {

                //p16 = new TreeSereServ7V2Processor();
                //u16 = (UserControl)p16.Run(InitTreeSereServ(true,true));
                //if (u14 != null)
                //{
                //    xtraScrollableControl17.Controls.Add(u16);
                //    u16.Dock = DockStyle.Fill;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void xtraTabControl1_TabIndexChanged(object sender, EventArgs e)
        {

            try
            {
                //p16 = new TreeSereServ7V2Processor();
                //u16 = (UserControl)p16.Run(InitTreeSereServ(true,true));
                //if (u14 != null)
                //{
                //    xtraScrollableControl17.Controls.Add(u16);
                //    u16.Dock = DockStyle.Fill;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void timerDoubleClick_Tick(object sender, EventArgs e)
        {
            try
            {
                timer1_Tick();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HIS_PATIENT GetPatient(L_HIS_SERVICE_REQ row)
        {
            HIS_PATIENT result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                treatmentFilter.ID = row.TREATMENT_ID;
                var treatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, param);
                if (treatment != null && treatment.Count > 0)
                {
                    HisPatientFilter patientFilter = new HisPatientFilter();
                    patientFilter.ID = treatment.First().PATIENT_ID;
                    var patient = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, patientFilter, param);
                    if (patient != null && patient.Count > 0)
                    {
                        result = patient.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void lblServiceReqCount_Click(object sender, EventArgs e)
        {

        }

        private void txtBedCodeBedName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewServiceReq_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                DoubleClickGridServiceReq();
                CreateThreadCallPatientRefresh();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemTextEdit_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                DoubleClickGridServiceReq();
                CreateThreadCallPatientRefresh();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtGateNumber_Leave(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == txtGateNumber.Name && o.MODULE_LINK == ModuleLinkName).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = txtGateNumber.Text;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = txtGateNumber.Name;
                    csAddOrUpdate.VALUE = txtGateNumber.Text;
                    csAddOrUpdate.MODULE_LINK = ModuleLinkName;
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

        private void txtStepNumber_Leave(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == txtStepNumber.Name && o.MODULE_LINK == ModuleLinkName).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = txtStepNumber.Text; ;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = txtStepNumber.Name;
                    csAddOrUpdate.VALUE = txtStepNumber.Text;
                    csAddOrUpdate.MODULE_LINK = ModuleLinkName;
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

        private void cboInDebt_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (isInit)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == cboInDebt.Name && o.MODULE_LINK == this.ModuleLinkName).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (cboInDebt.SelectedIndex != null ? cboInDebt.SelectedIndex.ToString() : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = cboInDebt.Name;
                    csAddOrUpdate.VALUE = (cboInDebt.SelectedIndex != null ? cboInDebt.SelectedIndex.ToString() : "");
                    csAddOrUpdate.MODULE_LINK = this.ModuleLinkName;
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

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPreviewIntructionDate_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtIntructionDate.EditValue != null && dtIntructionDate.DateTime != DateTime.MinValue && !String.IsNullOrWhiteSpace(btnCodeFind.Text))
                {
                    var currentdate = dtIntructionDate.DateTime;
                    if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_InDate)
                        dtIntructionDate.EditValue = currentdate.AddDays(-1);
                    else
                        dtIntructionDate.EditValue = currentdate.AddMonths(-1);

                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNextIntructionDate_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtIntructionDate.EditValue != null && dtIntructionDate.DateTime != DateTime.MinValue && !String.IsNullOrWhiteSpace(btnCodeFind.Text))
                {
                    var currentdate = dtIntructionDate.DateTime;
                    if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_InDate)
                        dtIntructionDate.EditValue = currentdate.AddDays(1);
                    else
                        dtIntructionDate.EditValue = currentdate.AddMonths(1);

                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnViewAccessionNumber_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (SereServ6ADO)gridViewSereServServiceReq.GetFocusedRow();
                if (row != null)
                {
                    ImageCode.ImageCodeView view = new ImageCode.ImageCodeView(row.ID.ToString());
                    view.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkScreenSaver_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkScreenSaver.Name && o.MODULE_LINK == ModuleLinkName).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkScreenSaver.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkScreenSaver.Name;
                    csAddOrUpdate.VALUE = (chkScreenSaver.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ModuleLinkName;
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

        /// <summary>
        /// mở màn hình mặc định
        /// </summary>
        private void LoadDefaultScreenSaver()
        {
            try
            {
                if (chkScreenSaver.Checked)
                {
                
                    List<object> _listObj = new List<object>();
                    _listObj.Add(true);

                    WaitingManager.Hide();
                     var SCREEN_SAVER = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.roomId);
                 
                 //   var SCREEN_SAVER = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().FirstOrDefault(o => o.ID == roomId);

                    if (SCREEN_SAVER != null)
                    {
                        if (!string.IsNullOrEmpty(SCREEN_SAVER.SCREEN_SAVER_MODULE_LINK))
                        {
                            
                            HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(SCREEN_SAVER.SCREEN_SAVER_MODULE_LINK, this.roomId, this.roomTypeId, _listObj);
                            //HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(SCREEN_SAVER.SCREEN_SAVER_MODULE_LINK,GlobalVariables.ROOM_ID_FOR_WAITING_SCREEN, this.roomTypeId,  
                            //listObj
                   

                        }
                        else
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.KhongCoManHinhCho, string.Join(",", SCREEN_SAVER.ROOM_NAME)));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LciGroupEmrDocument_Click(object sender, EventArgs e)
        {
        }

        private void xtraTabDocument_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                if (TreeClickData != null && !String.IsNullOrWhiteSpace(TreeClickData.TDL_SERVICE_REQ_CODE))
                {
                    Inventec.Common.Logging.LogSystem.Info("xtraTabDocument_TabIndexChanged");
                    ProcessLoadDocumentBySereServ(TreeClickData);
                }
                else
                {
                    this.ucViewEmrDocumentReq.ReloadDocument(new List<EMR.SDO.EmrDocumentFileSDO>());
                    this.ucViewEmrDocumentResult.ReloadDocument(new List<EMR.SDO.EmrDocumentFileSDO>());
                }
            }
            catch (Exception ex)
            {
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

                if (e.Group.Name == lcgPatientInfo.Name)
                {
                    value = lcgPatientInfo.Expanded ? "1" : null;
                }
                else if (e.Group.Name == lcgServiceReq.Name)
                {
                    value = lcgServiceReq.Expanded ? "1" : null;
                }
                else if (e.Group.Name == lcgTreeSereServ.Name)
                {
                    value = lcgTreeSereServ.Expanded ? "1" : null;
                }
                else if (e.Group.Name == LciGroupEmrDocument.Name)
                {
                    value = LciGroupEmrDocument.Expanded ? "1" : null;
                }

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == name && o.MODULE_LINK == ModuleLinkName).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
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
                this.controlStateWorker.SetData(this.currentControlStateRDO);


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void gridViewSereServServiceReq_Click(object sender, EventArgs e)
        {
            try
            {
                SereServ6ADO sereServ6 = gridViewSereServServiceReq.GetFocusedRow() as SereServ6ADO;
                if (sereServ6 != null && !String.IsNullOrWhiteSpace(sereServ6.TDL_SERVICE_REQ_CODE))
                {
                    V_HIS_SERE_SERV_7 data = new V_HIS_SERE_SERV_7();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV_7>(data, sereServ6);
                    TreeClickData = data;

                    ProcessLoadDocumentBySereServ(data);
                }
                else
                {
                    this.ucViewEmrDocumentReq.ReloadDocument(new List<EMR.SDO.EmrDocumentFileSDO>());
                    this.ucViewEmrDocumentResult.ReloadDocument(new List<EMR.SDO.EmrDocumentFileSDO>());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitServiceReqTypeCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboServiceRoom.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__ServiceReqType);
                cboServiceRoom.Properties.Tag = gridCheck;
                cboServiceRoom.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboServiceRoom.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboServiceRoom.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__ServiceReqType(object sender, EventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                serviceSelecteds = new List<V_HIS_SERVICE>();
                foreach (MOS.EFMODEL.DataModels.V_HIS_SERVICE rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                    {
                        if (sb.ToString().Length > 0) { sb.Append(", "); }
                        sb.Append(rv.SERVICE_NAME);
                        serviceSelecteds.Add(rv);
                    }
                }
                cboServiceRoom.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void InitComboServiceReqType()
        {
            try
            {
                var datas = lstServiceRoom;
                if (datas != null)
                {
                    var listServiceIds = datas.Where(o => o.ROOM_ID == this.roomId).Select(p => p.SERVICE_ID).ToList();

                    listServices = BackendDataWorker.Get<V_HIS_SERVICE>();

                    var lstService = listServices.Where(o => listServiceIds.Any(p => p == o.ID) && o.IS_ACTIVE == 1).ToList();

                    cboServiceRoom.Properties.DataSource = lstService;
                    cboServiceRoom.Properties.DisplayMember = "SERVICE_NAME";
                    cboServiceRoom.Properties.ValueMember = "ID";
                    DevExpress.XtraGrid.Columns.GridColumn col1 = cboServiceRoom.Properties.View.Columns.AddField("SERVICE_CODE");
                    col1.VisibleIndex = 1;
                    col1.Width = 100;
                    col1.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.cboServiceRoom1.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                    DevExpress.XtraGrid.Columns.GridColumn col2 = cboServiceRoom.Properties.View.Columns.AddField("SERVICE_NAME");
                    col2.VisibleIndex = 2;
                    col2.Width = 600;
                    col2.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoom.cboServiceRoom2.Text", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                    cboServiceRoom.Properties.PopupFormWidth = 200;
                    cboServiceRoom.Properties.View.OptionsView.ShowColumnHeaders = true;
                    cboServiceRoom.Properties.View.OptionsSelection.MultiSelect = true;
                    cboServiceRoom.Properties.PopupFormWidth = 500;
                    cboServiceRoom.Properties.ImmediatePopup = true;


                    GridCheckMarksSelection gridCheckMark = cboServiceRoom.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.SelectAll(cboServiceRoom.Properties.View);
                    }


                    //GridCheckMarksSelection gridCheckMark = cboServiceRoom.Properties.Tag as GridCheckMarksSelection;
                    //if (gridCheckMark != null)
                    //{
                    //    gridCheckMark.ClearSelection(cboServiceRoom.Properties.View);
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboServiceRoom_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (MOS.EFMODEL.DataModels.V_HIS_SERVICE rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.SERVICE_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    btnFind.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }



        private void cboSucKhoe_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    btnFind.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDaKe_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    btnFind.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnMissCall_Click(object sender, EventArgs e)
        {
            try
            {
                LogTheadInSessionInfo(() => btnMissCall_Click_Action(sender, e), "btnMissCall_Click");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void btnMissCall_Click_Action(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                string configKeyCallPatientCPA = ConfigApplicationWorker.Get<string>(AppConfigKey.CONFIG_KEY__DANG_KY_TIEP_DON__GOI_BENH_NHAN_BANG_PHAN_MEM_CPA);
                if (configKeyCallPatientCPA == "1")
                {
                    if (CheckListCPA != null && CheckListCPA.Count > 0)
                    {
                        ThreadCallPatientRefresh(CheckListCPA);
                    }
                    if (this.clienttManager == null)
                        this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager();
                    long[] id = this.clienttManager.GetCurrentPatientCall(txtGateNumber.Text.Trim(), true);
                    Inventec.Common.Logging.LogSystem.Info("This.clienttManager.GetCurrentPatientCall ____Goi nho: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => id), id));
                    txtCallPatientCPA.ForeColor = Color.Red;
                    lcCkhCPA.AppearanceItemCaption.ForeColor = Color.Red;
                    foreach (var item in id)
                    {
                        var data = serviceReqs.FirstOrDefault(o => o.ID == item);
                        if (data != null)
                        {
                            if (data.CALL_COUNT.HasValue)
                            {
                                if (data.CALL_COUNT >= 1)
                                {
                                    data.status = 13;
                                }
                                else
                                {
                                    data.status = 11;
                                }
                            }
                            else
                            {
                                data.status = 11;
                            }
                        }
                        gridControlServiceReq.RefreshDataSource();
                    }

                    CreateThreadCallPatientCountService(id);
                    CheckListCPA = id.ToList();
                    // ThreadCallPatientRefresh(id.ToList());
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNotEnter_Click(object sender, EventArgs e)
        {
            try
            {
                LogTheadInSessionInfo(() => btnNotEnter_Click_Action(sender, e), "btnNotEnter_Click");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnNotEnter_Click_Action(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (CheckListCPA != null && CheckListCPA.Count > 0)
                {
                    ThreadCallPatientRefresh(CheckListCPA);
                }
                long[] id = callPatientNho();
                Inventec.Common.Logging.LogSystem.Info("This.clienttManager.GetCurrentPatientCall ____Benh nhan khong vao: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => id), id));


                var data_ = (List<HIS.Desktop.Plugins.ExecuteRoom.ADO.ServiceReqADO>)gridViewServiceReq.DataSource;

                var data = data_.FirstOrDefault(o => o.ID == id.FirstOrDefault());
                CPA.WCFClient.CallPatientClient.ADO.CallPatientInfoADO CallPatientInfoADO_ = new CPA.WCFClient.CallPatientClient.ADO.CallPatientInfoADO();
                CallPatientInfoADO_.Dob = data.TDL_PATIENT_DOB;
                CallPatientInfoADO_.NumOrder = data.NUM_ORDER ?? 0;
                CallPatientInfoADO_.ServiceReqId = data.ID;
                CallPatientInfoADO_.PatientName = data.TDL_PATIENT_NAME;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Gọi bệnh nhân______" + Inventec.Common.Logging.LogUtil.GetMemberName(() => CallPatientInfoADO_), CallPatientInfoADO_));
                this.clienttManager.UpdatePatientToMissingCall(txtGateNumber.Text.Trim(), CallPatientInfoADO_);
                txtCallPatientCPA.ForeColor = Color.Black;
                lcCkhCPA.AppearanceItemCaption.ForeColor = Color.Black;
                foreach (var item in id)
                {

                    if (serviceReqs.FirstOrDefault(o => o.ID == item) != null)
                    {
                        if (serviceReqs.FirstOrDefault(o => o.ID == item).CALL_COUNT.HasValue)
                        {
                            if (serviceReqs.FirstOrDefault(o => o.ID == item).CALL_COUNT >= 1)
                            {
                                serviceReqs.FirstOrDefault(o => o.ID == item).status = 13;
                            }
                            else
                            {
                                serviceReqs.FirstOrDefault(o => o.ID == item).status = 11;
                            }
                        }
                        else
                        {
                            serviceReqs.FirstOrDefault(o => o.ID == item).status = 11;
                        }
                    }
                    gridControlServiceReq.RefreshDataSource();
                }
                CreateThreadCallPatientCountService(id);
                CheckListCPA = id.ToList();
                //ThreadCallPatientRefresh(id.ToList());
                //Thread t2 = new Thread(delegate()
                //{
                //    CreateThreadCallPatientCountService(id);
                //    Thread.Sleep(10000);
                //    this.BeginInvoke(new MethodInvoker(delegate
                //    {
                //        //FillDataToGridControl();
                //    }));
                //});
                //t2.Start(); 
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        long[] ID_;
        private long[] callPatientNho()
        {

            try
            {
                if (txtGateNumber.Text != null)
                {
                    string configKeyCallPatientCPA = ConfigApplicationWorker.Get<string>(AppConfigKey.CONFIG_KEY__DANG_KY_TIEP_DON__GOI_BENH_NHAN_BANG_PHAN_MEM_CPA);
                    if (configKeyCallPatientCPA == "1")
                    {

                        if (this.clienttManager == null)
                            this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager();
                        ID_ = this.clienttManager.GetCurrentPatientCall(txtGateNumber.Text.Trim(), false);
                        Inventec.Common.Logging.LogSystem.Debug("ID__________ID___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ID_), ID_));
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return ID_;
        }

        private void cboSucKhoe_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboSucKhoe.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtGateNumber_TextChanged(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtStepNumber_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void ckKQCLS_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ckKQCLS.Name && o.MODULE_LINK == ModuleLinkName).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (ckKQCLS.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ckKQCLS.Name;
                    csAddOrUpdate.VALUE = (ckKQCLS.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ModuleLinkName;
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

        private void gridViewServiceReq_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {

            try
            {
                if (e.RowHandle >= 0)
                {
                    ServiceReqADO pData = (ServiceReqADO)gridViewServiceReq.GetRow(e.RowHandle);
                    //if (e.Column.FieldName == "CallPatient1") // sửa
                    //{
                    //    if (pData.CALL_COUNT.HasValue)
                    //    {
                    //        if (pData.CALL_COUNT > 1)
                    //        {
                    //            e.RepositoryItem = repositoryItembtnGoiNho;
                    //        }
                    //        else
                    //        {
                    //            e.RepositoryItem = repositoryItemButton_CallPatient;
                    //        }

                    //    }
                    //    else
                    //    {
                    //        e.RepositoryItem = repositoryItemButton_CallPatient;
                    //    }

                    //}
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void chkCPA_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkCPA.Name && o.MODULE_LINK == ModuleLinkName).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkCPA.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkCPA.Name;
                    csAddOrUpdate.VALUE = (chkCPA.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ModuleLinkName;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                //FillDataToGridControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

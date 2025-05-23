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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.LibraryHein.Bhyt.HeinTreatmentType;
using Inventec.Common.Controls.EditorLoader;
using System.Threading;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.LocalStorage.SdaConfig;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using System.Globalization;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraEditors;
using HIS.UC.Icd;
using HIS.UC.SecondaryIcd;
using HIS.UC.SecondaryIcd.ADO;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Plugins.TreatmentFinish.CloseTreatment;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt;
using HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt.Base;
using DevExpress.Utils.Menu;
using HIS.Desktop.Plugins.TreatmentFinish.Config;
using HIS.Desktop.Plugins.TreatmentFinish.ADO;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.Plugins.TreatmentFinish.Validation;
using MOS.SDO;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid; 
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using EMR.SDO;
using DevExpress.XtraBars;
using HIS.Desktop.Plugins.Library.FormMedicalRecord;
using HIS.Desktop.Plugins.Library.TreatmentEndTypeExt;
using HIS.Desktop.Plugins.Library.TreatmentEndTypeExt.Data;
using HIS.Desktop.Plugins.TreatmentFinish.FormWarning;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.LibraryMessage;
using HIS.UC.UCCauseOfDeath.ADO;
using ACS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.Library.CheckHeinGOV;
using Inventec.Common.QrCodeBHYT;
using Inventec.Common.Logging;
using HIS.UC.Death.ADO;
using System.Resources;
using Inventec.UC.Login.Base;
using HIS.UC.Icd.ADO;
using IcdADO = HIS.Desktop.Plugins.TreatmentFinish.ADO.IcdADO;

namespace HIS.Desktop.Plugins.TreatmentFinish
{
    public partial class FormTreatmentFinish : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        HisTreatmentFinishSDO hisTreatmentFinishSDO;
        internal MOS.SDO.WorkPlaceSDO WorkPlaceSDO; 
        internal long treatmentId = 0;
        internal MOS.EFMODEL.DataModels.HIS_TREATMENT currentHisTreatment = null;
        //internal MOS.EFMODEL.DataModels.HIS_TREATMENT currentHisTreatment_ = null;
        internal int positionHandle = -1;
        internal MOS.SDO.HisTreatmentFinishSDO currentTreatmentFinishSDO = null;
        internal MOS.SDO.HisTreatmentFinishSDO hisTreatmentFinishSDO_process { get; set; }
        internal CloseTreatment.FormDeath FormDeath { get; set; }
        internal CloseTreatment.FormTTCDHienThiTrenGiayRaVien FormTTCDHienThiTrenGiayRaVien_ { get; set; }
        internal CloseTreatment.FormTransfer FormTransfer { get; set; }
        internal CloseTreatment.FormAppointment FormAppointment { get; set; }
        internal List<MOS.EFMODEL.DataModels.HIS_ICD> listIcd;
        internal HIS.Desktop.Common.RefeshReference RefeshReference = null;
        internal long TreatmentFinishTime { get; set; }
        internal List<ADO.DoctorADO> listDoctors { get; set; }
        internal List<ADO.DoctorADO> listUser { get; set; }
        HIS_DHST saveDHST;

        internal IcdProcessor icdProcessor;
        internal UserControl ucIcd;
        internal SecondaryIcdProcessor subIcdProcessor;
        internal UserControl ucSecondaryIcd;
        internal string AutoCheckIcd = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<String>("HIS.Desktop.Plugins.AutoCheckIcd");
        internal string CheckIcdWhenSave = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<String>("HIS.Desktop.Plugins.CheckIcdWhenSave");


        internal CultureInfo cultureLang = LanguageManager.GetCulture();

        private HIS_TREATMENT hisTreatmentResult;
        private V_HIS_PATIENT_TYPE_ALTER patientTypeAlter;
        private List<ModuleControlADO> ModuleControls { get; set; }
        private HIS_TREATMENT prosCD;
        private bool isInit = true;
        internal Inventec.Desktop.Common.Modules.Module module;

        internal HIS.UC.Icd.IcdProcessor IcdCauseProcessor { get; set; }
        internal UserControl ucIcdCause;
        private V_HIS_PATIENT_TYPE_ALTER hisPatientTypeAlterApplied;
        // program
        List<ProgramADO> ProgramADOList { get; set; }
        List<HIS_PATIENT_PROGRAM> PatientPrograms { get; set; }
        List<V_HIS_DATA_STORE> DataStores { get; set; }

        internal IcdProcessor icdYhctProcessor;
        internal UserControl ucIcdYhct;
        internal SecondaryIcdProcessor subIcdYhctProcessor;
        internal UserControl ucSecondaryIcdYhct;

        private List<HIS_DEPARTMENT_TRAN> ListDepartmentTran;
        private List<HIS_SERE_SERV> SereServCheck;
        private List<HIS_SERE_SERV> SereServTreatment;
        internal List<PrintConfigADO> printConfigADOs = null;
        internal List<PrintConfigADO> printConfigADOLocalStores = null;
        internal List<WarningADO> warningADOs = null;
        bool _isSkipWarningForSave = false;

        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        const string moduleLink = "HIS.Desktop.Plugins.TreatmentFinish";

        List<HIS_EMR_COVER_CONFIG> LstEmrCoverConfig;
        List<HIS_EMR_COVER_CONFIG> LstEmrCoverConfigDepartment;
        List<HIS_TREATMENT_END_TYPE> LstHisTreatmentEndType;
        MediRecordMenuPopupProcessor emrMenuPopupProcessor = null;
        BarManager _BarManager = null;
        internal PopupMenu _Menu = null;
        private TreatmentEndTypeExtData currentTreatmentEndTypeExt;
        private bool isLoadTreatmentInFormTreatmentEndTypeExt;
        System.Windows.Forms.Timer timerInitForm;
        Thread t_;
        List<HIS_SERVICE_REQ> serviceReqsSA { get; set; }
        List<HIS_SERVICE_REQ> examServiceReqs;
        List<V_HIS_BED_LOG> BedLogs { get; set; }
        CauseOfDeathADO causeResult { get; set; }
        public ResultDataADO ResultDataADO { get; set; }
        public List<V_HIS_EMPLOYEE> HisEmployees;
        public List<ACS_USER> AcsUsers;
        public List<HIS_TREATMENT_END_TYPE> hisTreatmentEndTypes;
        public List<MOS.EFMODEL.DataModels.HIS_TREATMENT_RESULT> hisTreatmentResults;
        public List<MOS.EFMODEL.DataModels.HIS_TREATMENT_END_TYPE_EXT> hisTreatmentEndTypeExts;
        public List<MOS.EFMODEL.DataModels.V_HIS_ROOM> hisRooms;
        public List<MOS.EFMODEL.DataModels.HIS_DEPARTMENT> hisDepartments;
        public List<MOS.EFMODEL.DataModels.HIS_BRANCH> hisBranchs;
        public List<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE> hisTreatmentTypes;
        HIS_PATIENT currentPatient;
        HIS_TREATMENT_EXT currentTreatmentExt = new HIS_TREATMENT_EXT();
        public List<HIS_CAREER> careers;
        public bool InPhieuHenKham { get; set; }
        public bool XemTruocKhiIn { get; set; }
        public bool KyPhieuHenKham { get; set; }
        bool isFinished = false;
        #endregion

        #region Construct
        public FormTreatmentFinish()
        {
            InitializeComponent();
        }

        public FormTreatmentFinish(HIS.Desktop.ADO.TreatmentLogADO ado, HIS.Desktop.Common.RefeshReference refresh, Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            try
            {
                InitializeComponent();
                SetIcon();
                if (ado.RoomId > 0)
                {
                    WorkPlaceSDO = WorkPlace.WorkPlaceSDO.Where(o => o.RoomId == ado.RoomId).FirstOrDefault();
                }
                else
                {
                    WorkPlaceSDO = WorkPlace.WorkPlaceSDO.Where(o => o.RoomId == module.RoomId).FirstOrDefault();
                }

                this.module = module;
                this.treatmentId = ado.TreatmentId;
                this.Text = module.text;
                if (refresh != null)
                {
                    this.RefeshReference = refresh;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public FormTreatmentFinish(long _treatmentId, Inventec.Desktop.Common.Modules.Module module, Common.RefeshReference refresh)
            : base(module)
        {
            try
            {
                InitializeComponent();
                SetIcon();
                this.module = module;
                this.treatmentId = _treatmentId;
                this.Text = module.text;

                WorkPlaceSDO = WorkPlace.WorkPlaceSDO.Where(o => o.RoomId == module.RoomId).FirstOrDefault();
                if (refresh != null)
                {
                    this.RefeshReference = refresh;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method
        HIS_TRACKING tracking { get; set; }
        private void CreateThreadGetData1()
        {
            var treatment = new Thread(new ThreadStart(LoadTreatment));
            var trachking = new Thread(new ThreadStart(LoadTracking));
            var icd = new Thread(new ThreadStart(LoadIcd));
            try
            {
                icd.Start();
                treatment.Start();
                trachking.Start();

                icd.Join();
                treatment.Join();
                trachking.Join();
            }
            catch (Exception ex)
            {
                icd.Abort();
                treatment.Abort();
                trachking.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadIcd()
        {
            listIcd = BackendDataWorker.Get<HIS_ICD>().OrderBy(o => o.ICD_CODE).ToList();
        }

        private void LoadTracking()
        {
            if (treatmentId > 0)
            {
                CommonParam param = new CommonParam();
                HisTrackingFilter filter = new HisTrackingFilter();
                filter.TREATMENT_ID = treatmentId;
                var listTracking = new BackendAdapter(param).Get<List<HIS_TRACKING>>("api/HisTracking/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (listTracking != null && listTracking.Count > 0)
                {
                    tracking = listTracking.OrderByDescending(o => o.TRACKING_TIME).ThenByDescending(o => o.ID).ToList()[0];
                }
            }
        }

        #region load
        System.Windows.Forms.Timer time = new System.Windows.Forms.Timer();
        private void FormTreatmentFinish_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                Inventec.Common.Logging.LogSystem.Error("CreateThreadGetData 1");
                CreateThreadGetData1();
                InitUcIcdTotal();
                HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.LoadConfig();
                HIS.Desktop.Plugins.Library.RegisterConfig.BHXHLoginCFG.LoadConfig();
                Inventec.Common.Logging.LogSystem.Error("TreatmentFinish 1");
                LoadDataFromRam();
                InitComboHisHospitalizeReason();
                ValidatecboCareer();
                Config.ConfigKey.GetConfigKey();
                ProcessCheckMaterialInvoice();
                Inventec.Common.Logging.LogSystem.Error("CreateThreadGetData 2");
                CreateThreadGetData2();
                Inventec.Common.Logging.LogSystem.Error("TreatmentFinish 2");
                time.Interval = 500;
                time.Tick += Time_Tick;
                ThreadLoadSereServ();
                ThreadLoadDepartmentTran();

                SetEndOrder();

                SetDefaultValueControl();
                LoadDataToComboCareer();

                LoadDataEye(currentHisTreatment);
                LoadComboControls();
                LoadDataTreatmentExt();
                FillDataCurrentTreatment(currentHisTreatment);

                TaskLoadBedLog();
                TaskLoadServiceReqSA();
                EnableControlByTreatment();


                LoadClinicalNote();
                LoadSurgery();
                Inventec.Common.Logging.LogSystem.Error("TreatmentFinish 3");

                ValidateForm();

                LoadComboProgram(this.PatientPrograms, DataStores);

                SetupPrintConfig();
                InitPopupPrintConfig();

                InitControlState();
                Inventec.Common.Logging.LogSystem.Error("TreatmentFinish End");
                //
                chkOutHopitalCondition.MaximumSize = new System.Drawing.Size(txtDaysBedTreatment.Width, 0);
                ChkExpXml4210.MaximumSize = new System.Drawing.Size(txtDaysBedTreatment.Width, 0);

                time.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadDataTreatmentExt()
        {
            try
            {
                if (currentHisTreatment == null) return;
                List<HIS_TREATMENT_EXT> listTreatmentExt = null;
                MOS.Filter.HisTreatmentExtFilter filter = new MOS.Filter.HisTreatmentExtFilter();
                filter.TREATMENT_ID = currentHisTreatment.ID;
                listTreatmentExt = new BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT_EXT>>("api/HisTreatmentExt/Get", ApiConsumers.MosConsumer, filter, null);

                if (listTreatmentExt != null) currentTreatmentExt = listTreatmentExt.FirstOrDefault();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboCareer()
        {
            try
            {
                careers = BackendDataWorker.Get<HIS_CAREER>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("CAREER_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("CAREER_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("CAREER_NAME", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(cboCareer, careers, controlEditorADO);
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.TreatmentFinish.Resources.Lang", typeof(FormTreatmentFinish).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControlMain.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.layoutControlMain.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDeleteEndInfo.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.btnDeleteEndInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDeleteEndInfo.ToolTip = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.btnDeleteEndInfo.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtEndDeptSubsHead.ToolTip = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.txtEndDeptSubsHead.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemSave.Caption = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.barButtonItemSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtHospSubsDirector.ToolTip = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.txtHospSubsDirector.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCheckIcd.ToolTip = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.btnCheckIcd.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnKiemTraHoSo.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.btnKiemTraHoSo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSaveTextLib.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.btnSaveTextLib.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnListTextLib.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.btnListTextLib.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDepartmentOut.Properties.NullText = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.cboDepartmentOut.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCD.ToolTip = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.btnCD.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chKTaoHoSoMoi.Properties.Caption = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.chKTaoHoSoMoi.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chKTaoHoSoMoi.ToolTip = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.chKTaoHoSoMoi.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnGetQT.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.btnGetQT.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsEmergency.Properties.Caption = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.chkIsEmergency.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrintConfig.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.btnPrintConfig.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrintConfig.ToolTip = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.btnPrintConfig.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkOutHopitalCondition.Properties.Caption = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.chkOutHopitalCondition.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAppointInfo.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.btnAppointInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAppointInfo.ToolTip = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.btnAppointInfo.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblEyesightGlassLeft.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lblEyesightGlassLeft.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblEyesightGlassRight.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lblEyesightGlassRight.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblEyesightLeft.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lblEyesightLeft.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblEyesightRight.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lblEyesightRight.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblEyeTensionLeft.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lblEyeTensionLeft.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblEyeTensionRight.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lblEyeTensionRight.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnGetPT.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.btnGetPT.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ChkExpXml4210.Properties.Caption = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.ChkExpXml4210.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboProgram.Properties.NullText = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.cboProgram.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkCapSoLuuTruBA.Properties.Caption = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.chkCapSoLuuTruBA.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnChoiceResult.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.btnChoiceResult.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblKetQuaXetNghiem.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lblKetQuaXetNghiem.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.BtnEndCode.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.BtnEndCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTTExt.Properties.NullText = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.cboTTExt.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnBedHistory.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.btnBedHistory.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDHST.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.btnDHST.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSinhLaiSoChuyenVien.Properties.Caption = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.chkSinhLaiSoChuyenVien.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSinhLaiSoRaVien.Properties.Caption = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.chkSinhLaiSoRaVien.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDoctorUserName.Properties.NullText = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.cboDoctorUserName.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkChronic.Properties.Caption = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.chkChronic.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSaveTemp.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.btnSaveTemp.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtAdvised.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.txtAdvised.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtMethod.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.txtMethod.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTreatmentEndType.Properties.NullText = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.cboTreatmentEndType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboResult.Properties.NullText = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.cboResult.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dtNewTreatmentTime.ToolTip = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.dtNewTreatmentTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboHeadUser.Properties.NullText = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.cboHeadUser.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDirectorUser.Properties.NullText = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.cboDirectorUser.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboEndDeptSubsHead.Properties.NullText = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.cboEndDeptSubsHead.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboEndDeptSubsHead.ToolTip = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.cboEndDeptSubsHead.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboHospSubsDirector.Properties.NullText = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.cboHospSubsDirector.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboHospSubsDirector.ToolTip = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.cboHospSubsDirector.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTimeIn.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciTimeIn.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEndTime.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciEndTime.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEndTime.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciEndTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNumberOfDays.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciNumberOfDays.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMethod.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciMethod.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAdvised.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciAdvised.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEndOrder.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciEndOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciKetQuaXetNghiem.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciKetQuaXetNghiem.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSurgery.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciSurgery.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciChkCapSoLuuTruBA.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciChkCapSoLuuTruBA.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciChkCapSoLuuTruBA.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciChkCapSoLuuTruBA.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciClinical.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciClinical.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciChkChronic.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciChkChronic.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciStoreCode.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciStoreCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciExpXml4210.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.LciExpXml4210.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LciExpXml4210.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.LciExpXml4210.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcgEyeInfoGroup.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lcgEyeInfoGroup.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEyeTensionRight.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciEyeTensionRight.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEyeTensionLeft.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciEyeTensionLeft.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEyesightRight.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciEyesightRight.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEyesightRight.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciEyesightRight.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEyesightLeft.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciEyesightLeft.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEyesightLeft.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciEyesightLeft.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEyesightGlassLeft.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciEyesightGlassLeft.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEyesightGlassLeft.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciEyesightGlassLeft.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEyesightGlassRight.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciEyesightGlassRight.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEyesightGlassRight.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciEyesightGlassRight.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem25.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.layoutControlItem25.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem28.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.layoutControlItem28.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem37.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.layoutControlItem37.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEndDepartment.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciEndDepartment.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcDepartmentOut.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lcDepartmentOut.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcDepartmentOut.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lcDepartmentOut.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciOutPatientDateFrom.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciOutPatientDateFrom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDoctorName.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciDoctorName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTypeOfDischarge.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciTypeOfDischarge.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.layoutControlItem11.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientProgram.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciPatientProgram.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem27.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.layoutControlItem27.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem27.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.layoutControlItem27.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciChkEmergency.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciChkEmergency.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciChkOutHopitalCondition.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciChkOutHopitalCondition.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciChkOutHopitalCondition.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciChkOutHopitalCondition.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExtraEndCode.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciExtraEndCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciOutPatientDateTo.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciOutPatientDateTo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDaysBedTreatment.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciDaysBedTreatment.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDaysBedTreatment.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciDaysBedTreatment.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.emptyPhuongPhapDieuTri.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.emptyPhuongPhapDieuTri.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNewTreatmentTime.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciNewTreatmentTime.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNewTreatmentTime.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciNewTreatmentTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem34.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.layoutControlItem34.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem34.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.layoutControlItem34.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem40.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.layoutControlItem40.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem41.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.layoutControlItem41.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem43.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.layoutControlItem43.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem43.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.layoutControlItem43.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem46.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.layoutControlItem46.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem46.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.layoutControlItem46.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDeleteEndInfo.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciDeleteEndInfo.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDeleteEndInfo.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.lciDeleteEndInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormTreatmentFinish.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void Time_Tick(object sender, EventArgs e)
        {
            time.Stop();
            //#15631
            CheckWarningOverTotalPatientPrice();
        }
        private void CreateThreadGetData2()
        {
            try
            {
                var patient = new System.Threading.Thread(ThreadGetPatient);
                var patientProgram = new System.Threading.Thread(ThreadLoadPatientProgram);
                var dataStore = new System.Threading.Thread(LoadDataStore);
                var patientTypeAlter = new System.Threading.Thread(ThreadGetPatientTypeAlter);

                patient.Start();
                patientProgram.Start();
                dataStore.Start();
                patientTypeAlter.Start();

                patient.Join();
                patientProgram.Join();
                dataStore.Join();
                patientTypeAlter.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ThreadGetPatient()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientFilter filter = new MOS.Filter.HisPatientFilter();
                filter.ID = currentHisTreatment.PATIENT_ID;
                currentPatient = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadLoadPatientProgram()
        {
            try
            {
                if (this.currentHisTreatment != null)
                {
                    MOS.Filter.HisPatientProgramFilter patientProgramFilter = new HisPatientProgramFilter();
                    patientProgramFilter.PATIENT_ID = this.currentHisTreatment.PATIENT_ID;
                    this.PatientPrograms = new BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT_PROGRAM>>("api/HisPatientProgram/Get", ApiConsumer.ApiConsumers.MosConsumer, patientProgramFilter, null);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("LoadPatientProgram this.treatment NULL");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ThreadGetPatientTypeAlter()
        {
            try
            {
                if (patientTypeAlter == null && this.currentHisTreatment != null)
                {
                    patientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetLastByTreatmentId", ApiConsumers.MosConsumer, this.currentHisTreatment.ID, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataStore()
        {
            try
            {
                long branchId = -1;
                if (this.WorkPlaceSDO != null)
                {
                    branchId = this.WorkPlaceSDO.BranchId;
                }
                else
                {
                    var currentRoom = this.hisRooms.FirstOrDefault(o => o.ID == this.module.RoomId);
                    branchId = currentRoom.BRANCH_ID;
                }

                MOS.Filter.HisDataStoreViewFilter dataStoreFilter = new HisDataStoreViewFilter();
                dataStoreFilter.BRANCH_ID = branchId;
                this.DataStores = new BackendAdapter(new CommonParam()).Get<List<V_HIS_DATA_STORE>>("api/HisDataStore/GetView", ApiConsumer.ApiConsumers.MosConsumer, dataStoreFilter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadDataFromRam()
        {
            try
            {
                List<Task> Tasks = new List<Task>();
                Task taskEmp = new Task(() =>
                {
                    this.HisEmployees = BackendDataWorker.Get<V_HIS_EMPLOYEE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                });
                taskEmp.Start();
                Tasks.Add(taskEmp);
                Task taskAcsUser = new Task(() =>
                {
                    this.AcsUsers = BackendDataWorker.Get<ACS_USER>().ToList();
                });
                taskAcsUser.Start();
                Tasks.Add(taskAcsUser);
                Task.WaitAll(Tasks.ToArray());

                this.hisTreatmentEndTypes = Base.GlobalStore.HisTreatmentEndTypes;
                this.hisTreatmentResults = Base.GlobalStore.HisTreatmentResults;
                this.hisRooms = BackendDataWorker.Get<V_HIS_ROOM>().ToList();
                this.hisTreatmentEndTypeExts = Base.GlobalStore.TreatmentEndTypeExts;
                this.hisDepartments = BackendDataWorker.Get<HIS_DEPARTMENT>().ToList();
                this.hisBranchs = BackendDataWorker.Get<HIS_BRANCH>().ToList();
                this.hisTreatmentTypes = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EnableControlByTreatment()
        {
            try
            {
                emptyDepartment.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciOutPatientDateFrom.Enabled = false;
                lciOutPatientDateTo.Enabled = false;

                if (this.currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU
                    || this.currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU
                    || this.currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY)
                {
                    cboDepartmentOut.Enabled = true;
                    lcDepartmentOut.Enabled = true;
                }
                else
                {
                    cboDepartmentOut.Enabled = false;
                    lcDepartmentOut.Enabled = false;
                }

                if (this.currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU
                    || this.currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY)
                {
                    lciOutPatientDateFrom.Enabled = true;
                    lciOutPatientDateTo.Enabled = true;
                }
                BtnEndCode.Enabled = CheckTreatmentEndCode();

                if (this.currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM
                    || this.currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
                {
                    chkCapSoLuuTruBA.Enabled = true;
                }
                else
                {
                    chkCapSoLuuTruBA.Enabled = false;
                }

                if (this.currentHisTreatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN
                    || this.currentHisTreatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN
                    || this.currentHisTreatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN)
                {
                    btnAppointInfo.Enabled = true;
                }
                else
                {
                    btnAppointInfo.Enabled = false;
                }
                if (currentPatient == null || string.IsNullOrEmpty(currentPatient.HRM_EMPLOYEE_CODE))
                    txtKskCode.Enabled = false;
                if (this.currentHisTreatment.IS_PAUSE != 1 && this.currentHisTreatment.TREATMENT_END_TYPE_ID != null)
                {
                    btnDeleteEndInfo.Enabled = true;
                }
                else
                    btnDeleteEndInfo.Enabled = false;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            isNotLoadWhileChangeControlStateInFirst = true;
            try
            {
                this.controlStateWorker = new Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ChkExpXml4210.Name)
                        {
                            ChkExpXml4210.Checked = item.VALUE == "1";
                        }
                    }
                }

                if (Config.ConfigKey.IsAutoCheckExportXmlCollinear == "1")
                {
                    LciExpXml4210.Enabled = false;
                    ChkExpXml4210.Checked = true;
                }
                else if (Config.ConfigKey.IsAutoCheckExportXmlCollinear == "2")
                {
                    LciExpXml4210.Enabled = false;
                    ChkExpXml4210.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhileChangeControlStateInFirst = false;
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                if (!string.IsNullOrEmpty(ConfigKey.TimeToCreateNewTreatmentInNewMonth))
                {
                    try
                    {
                        var time = ConfigKey.TimeToCreateNewTreatmentInNewMonth;
                        var hour = Convert.ToInt32(time.Substring(0, 2));
                        var minutes = Convert.ToInt32(time.Substring(2, 2));
                        var miliseconds = Convert.ToInt32(time.Substring(4, 2));
                        var dt = DateTime.Now.AddMonths(1);
                        if (ConfigKey.TimeToCreateNewTreatmentInNewMonth.Length == 6)
                        {
                            //Ngày đầu tiên của tháng.
                            dtNewTreatmentTime.DateTime = new DateTime(dt.Year, dt.Month, 1, hour, minutes, miliseconds);
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }
                else
                {
                    dtNewTreatmentTime.DateTime = DateTime.Now;
                }
                dtEndTime.EditValue = DateTime.Now;
                dtEndTime.Focus();
                dtEndTime.SelectAll();
                if (Config.ConfigKey.IsShowDoctor == "1")
                {
                    lciDoctorName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciCboDoctor.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.txtDoctorLogginName.Text = ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    this.cboDoctorUserName.EditValue = (object)ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    cboDoctorUserName.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    lciDoctorName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciCboDoctor.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }



                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(Config.ConfigKey.SaveTemp) == "1")
                    btnSaveTemp.Visible = true;
                else
                    btnSaveTemp.Visible = false;

                btnDHST.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadComboControls()
        {
            try
            {
                LoadComboTreatmentEndTypeExt();
                LoadCboDoctor();
                LoadCboDepartmentOut();
                if (this.listUser == null)
                    this.listUser = new List<DoctorADO>();
                if (this.HisEmployees != null && this.HisEmployees.Count > 0)
                {
                    listUser.AddRange((from n in this.HisEmployees select new DoctorADO(n)).ToList());
                }
                else
                {
                    listUser.AddRange((from n in this.AcsUsers select new DoctorADO(n)).ToList());
                }
                LoadCboHeadUser(this.AcsUsers);
                LoadCboDirectorUser(this.AcsUsers);
                LoadCboEndDeptSubsHead(listUser);
                LoadCboHospSubsDirector(listUser);
                LoadDataToCbo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadCboHospSubsDirector(List<DoctorADO> acsUser)
        {
            try
            {
                Base.GlobalStore.LoadDataGridLookUpEdit(cboHospSubsDirector, "LOGINNAME", "", "USERNAME", "", "LOGINNAME", acsUser);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadCboEndDeptSubsHead(List<DoctorADO> acsUser)
        {
            try
            {
                Base.GlobalStore.LoadDataGridLookUpEdit(cboEndDeptSubsHead, "LOGINNAME", "", "USERNAME", "", "LOGINNAME", acsUser);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadDataToCbo()
        {
            try
            {
                string ma = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FIMISH_MA");
                string ten = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FIMISH_TEN");
                LstHisTreatmentEndType = new List<HIS_TREATMENT_END_TYPE>();
                if (patientTypeAlter != null)
                {
                    if (patientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                    {
                        LstHisTreatmentEndType = this.hisTreatmentEndTypes.Where(o => o.IS_FOR_OUT_PATIENT == 1).ToList();
                    }
                    else
                    {
                        LstHisTreatmentEndType = this.hisTreatmentEndTypes;
                    }
                }

                Base.GlobalStore.LoadDataGridLookUpEdit(cboTreatmentEndType, "TREATMENT_END_TYPE_CODE", ma, "TREATMENT_END_TYPE_NAME", ten, "ID", LstHisTreatmentEndType);

                Base.GlobalStore.LoadDataGridLookUpEdit(cboResult, "TREATMENT_RESULT_CODE", ma, "TREATMENT_RESULT_NAME", ten, "ID", this.hisTreatmentResults);
                if (cboResult.EditValue == null)
                {
                    if (ConfigKey.TreatmentResultDefault == "1")
                    {
                        cboResult.EditValue = this.hisTreatmentResults != null ? this.hisTreatmentResults.FirstOrDefault(o => o.TREATMENT_RESULT_NAME.ToLower() == "đỡ").ID : 0;
                    }
                }
                if (cboTreatmentEndType.EditValue == null)
                {
                    if (ConfigKey.SetDefaultTreatmentEndType == "1")
                    {
                        cboTreatmentEndType.EditValue = LstHisTreatmentEndType != null ? LstHisTreatmentEndType.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN).ID : 0;
                    }
                    else if (ConfigKey.SetDefaultTreatmentEndType == "2")
                    {
                        var heinRightRouteData = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteStore.GetByCode(this.patientTypeAlter.RIGHT_ROUTE_CODE);
                        if (!string.IsNullOrWhiteSpace(this.currentHisTreatment.TRANSFER_IN_MEDI_ORG_CODE)
                            && heinRightRouteData != null && heinRightRouteData.HeinRightRouteName == "Đúng tuyến")
                        {
                            cboTreatmentEndType.EditValue = LstHisTreatmentEndType != null ? LstHisTreatmentEndType.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN).ID : 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private async Task LoadCboDepartmentOut()
        {

            try
            {
                long branchId_ = -1;
                if (this.WorkPlaceSDO != null)
                {
                    branchId_ = this.WorkPlaceSDO.BranchId;
                }
                else
                {
                    var currentRoom = this.hisRooms.FirstOrDefault(o => o.ID == this.module.RoomId);
                    branchId_ = currentRoom.BRANCH_ID;
                }
                var listDepartment = this.hisDepartments.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.BRANCH_ID == branchId_).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(cboDepartmentOut, listDepartment.ToList(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private async Task LoadComboTreatmentEndTypeExt()
        {
            try
            {
                List<HIS_TREATMENT_END_TYPE_EXT> endTypeExt = new List<HIS_TREATMENT_END_TYPE_EXT>();
                if (this.currentHisTreatment.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE)
                {
                    endTypeExt = this.hisTreatmentEndTypeExts.Where(o => o.ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_DUONG_THAI).ToList();
                }
                else
                {
                    endTypeExt = this.hisTreatmentEndTypeExts;
                }
                if (lciOutPatientDateFrom.Enabled == true
                    && lciOutPatientDateTo.Enabled == true)
                {
                    if (dtOutPatientDateFrom.EditValue != null || dtOutPatientDateTo.EditValue != null)
                    {
                        endTypeExt = endTypeExt != null ? endTypeExt.Where(o => o.ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM
                                                                        && o.ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_DUONG_THAI).ToList() : null;
                    }
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TREATMENT_END_TYPE_EXT_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("TREATMENT_END_TYPE_EXT_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TREATMENT_END_TYPE_EXT_NAME", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(cboTTExt, endTypeExt, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadCboDoctor()
        {
            try
            {
                listDoctors = new List<ADO.DoctorADO>();
                if (this.HisEmployees != null && this.HisEmployees.Count > 0)
                {
                    foreach (var item in this.HisEmployees)
                    {
                        if (item.IS_DOCTOR.HasValue && item.IS_DOCTOR.Value == Base.GlobalStore.IS_TRUE)
                        {
                            var emp = this.HisEmployees.FirstOrDefault(o => o.LOGINNAME == item.LOGINNAME);
                            if (emp == null) continue;
                            listDoctors.Add(new ADO.DoctorADO(emp));
                        }
                    }
                    var Doctors = listDoctors.FirstOrDefault(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());
                    if (Doctors == null)
                    {
                        txtDoctorLogginName.Text = "";
                        cboDoctorUserName.EditValue = null;
                        cboDoctorUserName.Properties.Buttons[1].Visible = false;
                    }
                }
                else
                {
                    foreach (var item in this.AcsUsers)
                    {
                        listDoctors.Add(new ADO.DoctorADO(item));
                    }
                }

                Inventec.Common.Logging.LogSystem.Error("ListDoctor: " + listDoctors.Count);
                Base.GlobalStore.LoadDataGridLookUpEdit(cboDoctorUserName, "LOGINNAME", "", "USERNAME", "", "LOGINNAME", listDoctors);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadCboHeadUser(List<ACS_USER> acsUser)
        {
            try
            {
                Base.GlobalStore.LoadDataGridLookUpEdit(cboHeadUser, "LOGINNAME", "", "USERNAME", "", "USERNAME", acsUser);
                var department = this.hisDepartments.FirstOrDefault(o => o.ID == (currentHisTreatment.LAST_DEPARTMENT_ID ?? 0));
                if (department != null)
                {
                    if (CheckNeedSignInstead(department.HEAD_LOGINNAME))
                    {
                        this.layoutControlItem46.AppearanceItemCaption.ForeColor = Color.Brown;
                    }
                    txtHeadUser.Text = department.HEAD_LOGINNAME;
                    cboHeadUser.EditValue = department.HEAD_USERNAME;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool CheckNeedSignInstead(string login_name)
        {
            bool isSign = false;
            try
            {
                //var rs = HisEmployees.Where(s => s.LOGINNAME.Equals(login_name)).FirstOrDefault();
                HisEmployeeFilter emFilter = new HisEmployeeFilter();
                emFilter.LOGINNAME__EXACT = login_name;
                var rs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EMPLOYEE>>("/api/HisEmployee/GetView", ApiConsumers.MosConsumer, emFilter, new CommonParam());
                if (rs != null)
                {
                    if (rs.First().IS_NEED_SIGN_INSTEAD == 1) isSign = true;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return isSign;
        }
        private async Task LoadCboDirectorUser(List<ACS_USER> acsUser)
        {
            try
            {
                Base.GlobalStore.LoadDataGridLookUpEdit(cboDirectorUser, "LOGINNAME", "", "USERNAME", "", "USERNAME", acsUser);
                var department = this.hisDepartments.FirstOrDefault(o => o.ID == (currentHisTreatment.LAST_DEPARTMENT_ID ?? 0));
                if (department != null)
                {
                    var branch = this.hisBranchs.FirstOrDefault(o => o.ID == (department.BRANCH_ID));
                    if (branch != null)
                    {

                        if (CheckNeedSignInstead(branch.DIRECTOR_LOGINNAME))
                        {
                            this.layoutControlItem43.AppearanceItemCaption.ForeColor = Color.Brown;
                        }
                        txtDirectorUser.Text = branch.DIRECTOR_LOGINNAME;
                        cboDirectorUser.EditValue = branch.DIRECTOR_USERNAME;

                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadTreatment()
        {
            try
            {
                CommonParam param = new CommonParam();
                if (this.treatmentId > 0)
                {
                    MOS.Filter.HisTreatmentViewFilter filter = new MOS.Filter.HisTreatmentViewFilter();
                    filter.ID = treatmentId;

                    var result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_TREATMENT>>(ApiConsumer.HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (result != null && result.Count > 0)
                    {
                        currentHisTreatment = result.FirstOrDefault();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadDataEye(HIS_TREATMENT currentHisTreatment)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Warn("LoadDataEye 1");
                if (tracking != null)
                {
                    txtEyesightGlassLeft.Text = tracking.EYESIGHT_GLASS_LEFT;
                    txtEyesightGlassRight.Text = tracking.EYESIGHT_GLASS_RIGHT;
                    txtEyesightLeft.Text = tracking.EYESIGHT_LEFT;
                    txtEyesightRight.Text = tracking.EYESIGHT_RIGHT;
                    txtEyeTensionLeft.Text = tracking.EYE_TENSION_LEFT;
                    txtEyeTensionRight.Text = tracking.EYE_TENSION_RIGHT;
                }
                Inventec.Common.Logging.LogSystem.Warn("LoadDataEye 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task FillDataCurrentTreatment(HIS_TREATMENT data)
        {
            try
            {
                if (data != null)
                {
                    UC.Icd.ADO.IcdInputADO icd = new IcdInputADO();
                    icd.ICD_CODE = data.ICD_CODE;
                    icd.ICD_NAME = data.ICD_NAME;
                    if (ucIcd != null)
                    {
                        icdProcessor.Reload(ucIcd, icd);
                    }

                    SecondaryIcdDataADO subIcd = new SecondaryIcdDataADO();
                    subIcd.ICD_SUB_CODE = data.ICD_SUB_CODE;
                    subIcd.ICD_TEXT = data.ICD_TEXT;
                    if (ucSecondaryIcd != null)
                    {
                        subIcdProcessor.Reload(ucSecondaryIcd, subIcd);
                    }

                    if (data.OUT_TIME.HasValue)
                    {
                        dtEndTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.OUT_TIME.Value) ?? DateTime.MinValue;
                    }

                    if (data.CLINICAL_IN_TIME != null)//Thêm
                        dtTimeIn.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.CLINICAL_IN_TIME ?? 0) ?? DateTime.MinValue;
                    else //Thêm
                        dtTimeIn.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.IN_TIME) ?? DateTime.MinValue;
                    //Ngoại trú từ
                    if (data.OUTPATIENT_DATE_FROM.HasValue)
                        dtOutPatientDateFrom.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.OUTPATIENT_DATE_FROM.Value) ?? DateTime.MinValue;
                    else
                        dtOutPatientDateFrom.EditValue = null;
                    //Ngoại trú đến
                    if (data.OUTPATIENT_DATE_TO.HasValue)
                        dtOutPatientDateTo.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.OUTPATIENT_DATE_TO.Value) ?? DateTime.MinValue;
                    else
                        dtOutPatientDateTo.EditValue = null;

                    if (data.TREATMENT_RESULT_ID.HasValue)
                    {
                        var treatmentResult = this.hisTreatmentResults.FirstOrDefault(o => o.ID == data.TREATMENT_RESULT_ID.Value);
                        if (treatmentResult != null)
                        {
                            cboResult.EditValue = treatmentResult.ID;
                        }
                        else
                        {

                            cboResult.EditValue = null;

                        }
                    }
                    else
                    {
                        cboResult.EditValue = null;

                    }
                    if (!string.IsNullOrEmpty(data.TDL_PATIENT_CAREER_CODE))
                    {
                        var currentCareer = careers.Where(o => o.CAREER_CODE == data.TDL_PATIENT_CAREER_CODE).FirstOrDefault();
                        cboCareer.EditValue = currentCareer.ID;
                    }
                    else
                    {
                        cboCareer.EditValue = null;
                    }
                    cboTTExt.EditValue = data.TREATMENT_END_TYPE_EXT_ID;
                    if (data.TREATMENT_END_TYPE_EXT_ID != null)
                    {
                        if (data.TREATMENT_END_TYPE_EXT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM
                            || data.TREATMENT_END_TYPE_EXT_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_DUONG_THAI)
                        {
                            txtExtraEndCode.Text = data.EXTRA_END_CODE;
                            lciExtraEndCode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;//xuandv
                        }
                        else
                        {
                            lciExtraEndCode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        }
                    }

                    txtAdvised.Text = data.ADVISE;
                    txtMethod.Text = data.TREATMENT_METHOD;
                    txtEndOrder.Text = data.END_CODE;
                    txtSoChuyenVien.Text = data.OUT_CODE;
                    txtSurgery.Text = data.SURGERY;
                    txtMaBHXH.Text = data.TDL_SOCIAL_INSURANCE_NUMBER;
                    

                    if (!string.IsNullOrEmpty(data.ICD_CAUSE_CODE))
                    {
                        var dataIcd = BackendDataWorker.Get<HIS_ICD>().FirstOrDefault(p => p.ICD_CODE == data.ICD_CAUSE_CODE.Trim());
                        if (dataIcd != null)
                        {
                            LoadIcdCauseToControl(dataIcd.ICD_CODE, data.ICD_CAUSE_NAME);
                        }
                    }

                    if (!string.IsNullOrEmpty(data.TRADITIONAL_ICD_CODE))
                    {
                        UC.Icd.ADO.IcdInputADO icdYhct = new UC.Icd.ADO.IcdInputADO();
                        icdYhct.ICD_CODE = data.TRADITIONAL_ICD_CODE;
                        icdYhct.ICD_NAME = data.TRADITIONAL_ICD_NAME;
                        if (ucIcdYhct != null)
                        {
                            this.icdYhctProcessor.Reload(ucIcdYhct, icdYhct, Template.NoFocus);
                        }
                    }
                    if (ConfigKey.SetDefaultTreatmentEndType == "1")
                    {
                        cboTreatmentEndType.EditValue = LstHisTreatmentEndType != null ? LstHisTreatmentEndType.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN).ID : 0;
                    }
                    else if (ConfigKey.SetDefaultTreatmentEndType == "2")
                    {
                        var heinRightRouteData = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteStore.GetByCode(this.patientTypeAlter.RIGHT_ROUTE_CODE);
                        if (!string.IsNullOrWhiteSpace(this.currentHisTreatment.TRANSFER_IN_MEDI_ORG_CODE)
                            && heinRightRouteData != null && heinRightRouteData.HeinRightRouteName == "Đúng tuyến")
                        {
                            cboTreatmentEndType.EditValue = LstHisTreatmentEndType != null ? LstHisTreatmentEndType.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN).ID : 0;
                        }
                    }
                    if (data.TREATMENT_END_TYPE_ID.HasValue && (GetTreatmentEndType() == 0 || cboTreatmentEndType.EditValue == null))
                    {
                        this.isFinished = true;
                        var treatmentEndType = this.hisTreatmentEndTypes.FirstOrDefault(o => o.ID == data.TREATMENT_END_TYPE_ID);

                        if (treatmentEndType != null)
                        {
                            if (treatmentEndType.ID > 0)
                            {
                                cboTreatmentEndType.EditValue = treatmentEndType.ID;
                            }
                            else
                            {
                                cboTreatmentEndType.EditValue = null;

                            }
                            if (treatmentEndType.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET)
                            {
                                cboTTExt.Enabled = false;
                                cboTTExt.EditValue = null;
                            }
                            else
                            {
                                cboTTExt.Enabled = true;
                            }
                        }
                    }
                    else if (GetTreatmentEndType() > 0)
                    {
                        cboTreatmentEndType.EditValue = cboTreatmentEndType.EditValue;
                    }
                    else
                    {
                        cboTreatmentEndType.EditValue = null;
                    }

                    if (data.IS_CHRONIC.HasValue && data.IS_CHRONIC.Value == Base.GlobalStore.IS_TRUE)
                    {
                        chkChronic.Checked = true;
                        lciChkChronic.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                    else
                    {
                        chkChronic.Checked = false;
                        lciChkChronic.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    }

                    if (data.IS_APPROVE_FINISH.HasValue && data.IS_APPROVE_FINISH.Value == Base.GlobalStore.IS_TRUE)
                    {
                        chkOutHopitalCondition.Checked = true;
                    }
                    else
                    {
                        chkOutHopitalCondition.Checked = false;
                    }

                    if (!String.IsNullOrEmpty(data.DOCTOR_LOGINNAME))
                    {
                        txtDoctorLogginName.Text = data.DOCTOR_LOGINNAME;
                        cboDoctorUserName.EditValue = data.DOCTOR_USERNAME;
                    }

                    if (!string.IsNullOrEmpty(data.END_CODE) && Base.GlobalStore.END_ORDER_STR != "1")
                    {
                        chkSinhLaiSoRaVien.Enabled = true;
                    }

                    if (!string.IsNullOrEmpty(data.OUT_CODE))
                    {
                        chkSinhLaiSoChuyenVien.Enabled = true;
                    }

                    if (!string.IsNullOrEmpty(data.TRADITIONAL_ICD_SUB_CODE))
                    {
                        SecondaryIcdDataADO subYhctIcd = new SecondaryIcdDataADO();
                        subYhctIcd.ICD_SUB_CODE = data.TRADITIONAL_ICD_SUB_CODE;
                        subYhctIcd.ICD_TEXT = data.TRADITIONAL_ICD_TEXT;
                        if (ucSecondaryIcdYhct != null)
                        {
                            subIcdYhctProcessor.Reload(ucSecondaryIcdYhct, subYhctIcd);
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(data.EYE_TENSION_RIGHT)
                        || !String.IsNullOrWhiteSpace(data.EYE_TENSION_LEFT)
                        || !String.IsNullOrWhiteSpace(data.EYESIGHT_RIGHT)
                        || !String.IsNullOrWhiteSpace(data.EYESIGHT_LEFT)
                        || !String.IsNullOrWhiteSpace(data.EYESIGHT_GLASS_RIGHT)
                        || !String.IsNullOrWhiteSpace(data.EYESIGHT_GLASS_LEFT))
                    {
                        txtEyeTensionRight.Text = data.EYE_TENSION_RIGHT;
                        txtEyeTensionLeft.Text = data.EYE_TENSION_LEFT;
                        txtEyesightRight.Text = data.EYESIGHT_RIGHT;
                        txtEyesightLeft.Text = data.EYESIGHT_LEFT;
                        txtEyesightGlassRight.Text = data.EYESIGHT_GLASS_RIGHT;
                        txtEyesightGlassLeft.Text = data.EYESIGHT_GLASS_LEFT;
                    }

                    // bat dau code o day
                    //tq
                    if (!string.IsNullOrEmpty(data.END_DEPT_SUBS_HEAD_LOGINNAME) && !string.IsNullOrEmpty(data.END_DEPT_SUBS_HEAD_USERNAME))
                    {
                        txtEndDeptSubsHead.Text = data.END_DEPT_SUBS_HEAD_LOGINNAME;
                        cboEndDeptSubsHead.EditValue = data.END_DEPT_SUBS_HEAD_LOGINNAME;
                    }
                    else
                    {
                        if (Config.ConfigKey.ENDDEAPRTMENTSUBSHEADOPTIOIN == "1")
                        {
                            var USER = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                            if (USER != null)
                            {
                                txtEndDeptSubsHead.Text = USER;
                                cboEndDeptSubsHead.EditValue = USER;
                            }
                         }
                    }



                    // giam doc
                    if (!string.IsNullOrEmpty(data.HOSP_SUBS_DIRECTOR_LOGINNAME) && !string.IsNullOrEmpty(data.HOSP_SUBS_DIRECTOR_USERNAME))
                    {
                        txtHospSubsDirector.Text = data.HOSP_SUBS_DIRECTOR_LOGINNAME;
                        cboHospSubsDirector.EditValue = data.HOSP_SUBS_DIRECTOR_LOGINNAME;
                    }
                    else
                    {
                        var _vHisExecuteRooms = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().FirstOrDefault(p => p.ROOM_ID == module.RoomId);
                        if (_vHisExecuteRooms != null && _vHisExecuteRooms.HOSP_SUBS_DIRECTOR_LOGINNAME != null)
                        {
                            txtHospSubsDirector.Text = _vHisExecuteRooms.HOSP_SUBS_DIRECTOR_LOGINNAME;
                            cboHospSubsDirector.EditValue = _vHisExecuteRooms.HOSP_SUBS_DIRECTOR_LOGINNAME;
                        }
                        else
                        {
                            var depar = hisDepartments.FirstOrDefault(s => s.HOSP_SUBS_DIRECTOR_LOGINNAME != null);
                            if (depar != null)
                            {
                                txtHospSubsDirector.Text = _vHisExecuteRooms.HOSP_SUBS_DIRECTOR_LOGINNAME;
                                cboHospSubsDirector.EditValue = _vHisExecuteRooms.HOSP_SUBS_DIRECTOR_LOGINNAME;
                            }
                        }  
                    }
                    
                    // ket thu o day

                    //nếu có dữ liệu tại popup khác thì load dữ liệu cũ để không phải mở popup lưu lại
                    if (data.TREATMENT_END_TYPE_EXT_ID.HasValue || data.TREATMENT_END_TYPE_ID.HasValue)
                    {
                        ProcessDataForTreatmentFinishSDO(data);
                    }
                    if (!string.IsNullOrEmpty(data.HOSPITALIZE_REASON_CODE))
                    {
                        var lst = cboHosReason.Properties.DataSource as List<MOS.EFMODEL.DataModels.HIS_HOSPITALIZE_REASON>;
                        if (lst != null && lst.Count > 0 && lst.FirstOrDefault(o => o.HOSPITALIZE_REASON_CODE == data.HOSPITALIZE_REASON_CODE) != null)
                        {
                            cboHosReason.EditValue = lst.FirstOrDefault(o => o.HOSPITALIZE_REASON_CODE == data.HOSPITALIZE_REASON_CODE).ID;
                        }
                        else
                        {
                            txtHosReasonNt.Text = data.HOSPITALIZE_REASON_NAME;
                        }
                    }
                    else
                    {
                        txtHosReasonNt.Text = data.HOSPITALIZE_REASON_NAME;
                    }
                    FillDataToControlsForm();
                    LoadSoNgayDieuTri();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadIcdCauseToControl(string icdCode, string icdName)
        {
            try
            {
                UC.Icd.ADO.IcdInputADO icd = new UC.Icd.ADO.IcdInputADO();
                icd.ICD_CODE = icdCode;
                icd.ICD_NAME = icdName;
                if (this.ucIcdCause != null)
                {
                    this.IcdCauseProcessor.Reload(this.ucIcdCause, icd, Template.NoFocus);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrentPatientTypeAlter(long treatmentId, long intructionTime, ref MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER hisPatientTypeAlter)
        {
            try
            {
                if (hisPatientTypeAlterApplied == null)
                {
                    CommonParam param = new CommonParam();
                    HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
                    filter.TreatmentId = treatmentId;
                    if (intructionTime > 0)
                        filter.InstructionTime = intructionTime;
                    else
                        filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                    hisPatientTypeAlterApplied = new BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, filter, param);
                }

                hisPatientTypeAlter = hisPatientTypeAlterApplied;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToControlsForm()
        {
            try
            {
                btnPrint.Enabled = false;

                txtEndDepartment.Text = WorkPlaceSDO.DepartmentName;
                txtEndDepartment.ToolTip = WorkPlaceSDO.DepartmentName;
                cboDepartmentOut.EditValue = this.WorkPlaceSDO.DepartmentId;
                if (currentTreatmentFinishSDO != null && hisTreatmentFinishSDO_process == null)
                {
                    hisTreatmentFinishSDO_process = currentTreatmentFinishSDO;
                }
                txtStoreCode.Text = currentHisTreatment.STORE_CODE;
                txtKskCode.Text = currentHisTreatment.HRM_KSK_CODE;


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationComboProgram()
        {
            try
            {
                CboProgramValidationRule vali = new CboProgramValidationRule();
                vali.cboProgram = cboProgram;
                vali.chkDataStore = chkCapSoLuuTruBA;
                vali.IsMustSetProgram = ConfigKey.IsMustSetProgramWhenFinishingInPatient;
                vali.TreatmentTypeId = this.currentHisTreatment.TDL_TREATMENT_TYPE_ID;
                vali.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(cboProgram, vali);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetEndOrder()
        {
            try
            {
                if (Base.GlobalStore.END_ORDER_STR != "1")
                {
                    txtEndOrder.Enabled = false;
                }
                else
                {
                    txtEndOrder.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetReadOnly()
        {
            try
            {
                this.txtAdvised.ReadOnly = true;
                this.txtMethod.ReadOnly = true;
                this.cboResult.ReadOnly = true;
                this.txtSurgery.ReadOnly = true;
                this.cboTreatmentEndType.ReadOnly = true;
                this.cboTTExt.ReadOnly = true;
                this.dtEndTime.ReadOnly = true;
                this.txtDoctorLogginName.ReadOnly = true;
                this.cboDoctorUserName.ReadOnly = true;
                this.chkChronic.ReadOnly = true;
                this.icdProcessor.ReadOnly(ucIcd, true);
                this.subIcdProcessor.ReadOnly(ucSecondaryIcd, true);
                this.IcdCauseProcessor.ReadOnly(ucIcdCause, true, Template.NoFocus);
                this.txtEndOrder.ReadOnly = true;
                this.txtMaBHXH.ReadOnly = true;
                this.txtDauHieuLamSang.ReadOnly = true;
                this.txtKetQuaXetNghiem.ReadOnly = true;
                this.cboProgram.ReadOnly = true;
                this.icdYhctProcessor.ReadOnly(ucIcdYhct, true, Template.NoFocus);
                this.subIcdYhctProcessor.ReadOnly(ucSecondaryIcdYhct, true);
                this.txtEyesightGlassLeft.ReadOnly = true;
                this.txtEyesightGlassRight.ReadOnly = true;
                this.txtEyesightLeft.ReadOnly = true;
                this.txtEyesightRight.ReadOnly = true;
                this.txtEyeTensionLeft.ReadOnly = true;
                this.txtEyeTensionRight.ReadOnly = true;
                this.txtKskCode.ReadOnly = true;
                this.cboCareer.ReadOnly = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadSurgery()
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(currentHisTreatment.SURGERY))
                {
                    txtSurgery.Text = currentHisTreatment.SURGERY;
                }
                else
                {
                    ThreadLoadSereServ();
                    string sugrey = "";
                    if (this.SereServCheck != null && this.SereServCheck.Count > 0)
                    {
                        var sereServs = this.SereServCheck.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL).ToList();
                        if (sereServs != null && sereServs.Count() > 0)
                        {
                            sugrey = String.Join(", ", sereServs.Select(o => o.TDL_SERVICE_NAME).Distinct().ToArray());
                        }
                    }

                    this.txtSurgery.Text = sugrey;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadClinicalNote()
        {
            try
            {
                HIS_SERVICE_REQ examServiceReq = null;

                if (this.currentTreatmentExt != null && !String.IsNullOrWhiteSpace(this.currentTreatmentExt.CLINICAL_NOTE))
                {
                    txtDauHieuLamSang.Text = this.currentTreatmentExt.CLINICAL_NOTE;
                }
                else
                {
                    HisServiceReqFilter srFilter = new HisServiceReqFilter();
                    srFilter.TREATMENT_ID = currentHisTreatment.ID;
                    srFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                    srFilter.ORDER_DIRECTION = "ASC";
                    srFilter.ORDER_FIELD = "INTRUCTION_TIME";
                    examServiceReqs = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, srFilter, null);
                    examServiceReq = examServiceReqs != null ? examServiceReqs.FirstOrDefault() : new HIS_SERVICE_REQ();
                    if (examServiceReq != null)
                    {
                        txtDauHieuLamSang.Text = examServiceReq.PATHOLOGICAL_PROCESS ?? "";
                    }
                }

                if (this.currentTreatmentExt != null && !String.IsNullOrWhiteSpace(this.currentTreatmentExt.SUBCLINICAL_RESULT))
                {
                    txtKetQuaXetNghiem.Text = this.currentTreatmentExt.SUBCLINICAL_RESULT;
                }
                else
                {
                    if (examServiceReq == null)
                    {
                        HisServiceReqFilter srFilter = new HisServiceReqFilter();
                        srFilter.TREATMENT_ID = currentHisTreatment.ID;
                        srFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                        srFilter.ORDER_DIRECTION = "ASC";
                        srFilter.ORDER_FIELD = "INTRUCTION_TIME";
                        examServiceReqs = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, srFilter, null);
                        examServiceReq = examServiceReqs != null ? examServiceReqs.FirstOrDefault() : null;
                    }
                    if (examServiceReq != null)
                    {
                        txtKetQuaXetNghiem.Text = examServiceReq.SUBCLINICAL ?? "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region enter
        private void dtEndTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    LoadSoNgayDieuTri();
                    icdProcessor.FocusControl(ucIcd);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadSoNgayDieuTri()
        {
            try
            {
                if (dtEndTime.EditValue != null && dtTimeIn.EditValue != null)
                {
                    String enddatetime = Inventec.Common.TypeConvert.Parse.ToDateTime(dtEndTime.Text).ToString("yyyyMMddHHmmss");
                    long endTime = Inventec.Common.TypeConvert.Parse.ToInt64(enddatetime);

                    String indatetime = Inventec.Common.TypeConvert.Parse.ToDateTime(dtTimeIn.Text).ToString("yyyyMMddHHmm");
                    long inTime = Inventec.Common.TypeConvert.Parse.ToInt64(indatetime + "00");

                    if (inTime != 0 && endTime != 0)
                    {
                        txtDaysBedTreatment.Text = DayOfTreatment(inTime, endTime, GetTreatmentEndType(), GetTreatmentResult()).ToString();
                        this.TreatmentFinishTime = endTime;
                        //Số ngày điều trị
                        txtNumberOfDays.Text = HIS.Common.Treatment.Calculation.DayOfTreatment6556(inTime, this.currentHisTreatment.CLINICAL_IN_TIME, endTime, IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        public static long? DayOfTreatment(long? timeIn, long? timeOut, long? treatmentEndTypeId, long? treatmentResultId)
        {
            long? result = null;
            try
            {
                if (!timeIn.HasValue || !timeOut.HasValue || !treatmentEndTypeId.HasValue
                    || !treatmentResultId.HasValue || timeIn > timeOut)
                    return result;

                DateTime dtIn = TimeNumberToSystemDateTime(timeIn.Value) ?? DateTime.Now;
                DateTime dtOut = TimeNumberToSystemDateTime(timeOut.Value) ?? DateTime.Now;
                TimeSpan ts = new TimeSpan();
                ts = (TimeSpan)(dtOut - dtIn);

                //Cung 1 ngay
                if (timeIn.Value.ToString().Substring(0, 8) == timeOut.Value.ToString().Substring(0, 8))
                {
                    if (ts.TotalMinutes <= 1440 && ts.TotalMinutes > 240)
                    {
                        result = 1;
                    }
                    else if (ts.TotalMinutes <= 240)
                    {
                        result = 0;
                    }
                }
                else if (ts.TotalMinutes < 1440) //Khac 1 ngay
                {
                    result = 1;
                }
                else
                {
                    if (((treatmentResultId.Value == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__KTD 
                        || treatmentResultId.Value == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__NANG
                        ) 
                        &&
                        (treatmentEndTypeId.Value == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN ||
                         treatmentEndTypeId.Value == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN)) ||   
                         treatmentEndTypeId.Value == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET
                        )
                    {
                        result = (int)((TimeSpan)(dtOut.Date - dtIn.Date)).TotalDays + 1;
                    }
                    else if(treatmentResultId.Value == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__DO && treatmentEndTypeId.Value == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN)
                        result = (int)((TimeSpan)(dtOut.Date - dtIn.Date)).TotalDays + 1;
                    else
                        result = (int)((TimeSpan)(dtOut.Date - dtIn.Date)).TotalDays;
                }
            }
            catch (Exception ex)
            {
                result = 0;
            }
            return result;
        }

        private static System.DateTime? TimeNumberToSystemDateTime(long time)
        {
            System.DateTime? result = null;
            try
            {
                if (time > 0)
                {
                    result = System.DateTime.ParseExact(time.ToString(), "yyyyMMddHHmmss",
                                       System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }


        private void dtEndTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(dtEndTime.Text))
                {
                    if (e.KeyCode == Keys.Tab || e.KeyCode == Keys.Enter)
                    {
                        LoadSoNgayDieuTri();
                        icdProcessor.FocusControl(ucIcd);
                    }
                }
                else
                {
                    dtEndTime.Focus();
                    dtEndTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private long? GetTreatmentEndType()
        {
            long? result = null;
            try
            {
                if (cboTreatmentEndType.EditValue != null)
                {
                    result = Inventec.Common.TypeConvert.Parse.ToInt64(cboTreatmentEndType.EditValue.ToString());
                }
                else
                {
                    result = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }

            return result;
        }

        private long? GetTreatmentResult()
        {
            long? result = null;
            try
            {
                if (cboResult.EditValue != null)
                {
                    result = Inventec.Common.TypeConvert.Parse.ToInt64(cboResult.EditValue.ToString());
                }
                else
                {
                    result = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }

        /// <summary>
        /// Nếu người dùng chọn "Đồng ý" thì tiếp tục mở form kết thúc điều trị. Nếu người dùng chọn "Không", thì tắt form kết thúc điều trị.
        /// </summary>
        private void ProcessCheckMaterialInvoice()
        {
            try
            {
                if (Config.ConfigKey.IsNoMaterialInvoiceInfo)
                {
                    CommonParam param = new CommonParam();
                    List<MissingInvoiceInfoMaterialSDO> apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MissingInvoiceInfoMaterialSDO>>("api/HisTreatment/GetMissingInvoiceInfoMaterialByTreatmentId", ApiConsumers.MosConsumer, this.treatmentId, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiResult), apiResult));
                        string materialNames = string.Join(", ", apiResult.Select(s => s.MATERIAL_TYPE_NAME).Distinct().ToList());
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.CacVatTuThieuThongTinHoaDonBanCoMuonTiepTuc, materialNames), ResourceMessage.ThongBao, MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                        {
                            this.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessPrescription()
        {
            if (Config.CheckFinishTimeCFG.mustExportBeforeOutOfDepartmentWithStayInPatient)
            {
                if (patientTypeAlter == null)
                    patientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetLastByTreatmentId", ApiConsumers.MosConsumer, currentHisTreatment.ID, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                if (patientTypeAlter != null &&
                    (patientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU ||
                    patientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU))
                {
                    HisExpMestFilter filter = new HisExpMestFilter();
                    filter.TDL_TREATMENT_ID = currentHisTreatment.ID;
                    filter.EXP_MEST_STT_IDs = new List<long>(){
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST
                        };

                    List<HIS_EXP_MEST> prescriptions = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (prescriptions != null && prescriptions.Count > 0)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ChuaLinhHetThuoc, ResourceMessage.ThongBao, DevExpress.Utils.DefaultBoolean.True);
                        throw new Exception("Chua linh het thuoc");
                    }
                }
            }
        }

        private void dtEndTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                LoadSoNgayDieuTri();

                if (isInit)
                    return;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboResult_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboResult.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TREATMENT_RESULT data = this.hisTreatmentResults.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboResult.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtMaBHXH.Focus();
                            txtMaBHXH.SelectAll();
                            LoadSoNgayDieuTri();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboResult_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboResult.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TREATMENT_RESULT data = this.hisTreatmentResults.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboResult.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtMaBHXH.Focus();
                            txtMaBHXH.SelectAll();
                            LoadSoNgayDieuTri();
                        }
                    }
                }
                else if (e.KeyCode == Keys.Down) cboResult.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTreatmentEndType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboTreatmentEndType.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_TREATMENT_END_TYPE data = this.hisTreatmentEndTypes.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentEndType.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            ShowPopupEndType(data);
                            LoadSoNgayDieuTri();
                        }
                    }
                }
                else if (e.KeyCode == Keys.Down) cboTreatmentEndType.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowPopupEndType(MOS.EFMODEL.DataModels.HIS_TREATMENT_END_TYPE data)
        {
            try
            {
                dxValidationProvider.SetValidationRule(txtEndOrder, null);
                lciExtraEndCode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                if (hisTreatmentFinishSDO_process == null)
                    hisTreatmentFinishSDO_process = new MOS.SDO.HisTreatmentFinishSDO();

                txtMethod.Enabled = true;
                txtAdvised.Enabled = true;
                cboTTExt.Enabled = true;
                btnAppointInfo.Enabled = false;
                if (data.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET)
                {
                    var severeIllnessInfo = GetSevereIllnessInfo(currentHisTreatment.ID);
                    CauseOfDeathADO causeOfDeathADO = new CauseOfDeathADO();
                    causeOfDeathADO.Treatment = currentHisTreatment;
                    if (severeIllnessInfo != null)
                    {
                        causeOfDeathADO.SevereIllNessInfo = severeIllnessInfo;
                        causeOfDeathADO.ListEventsCausesDeath = GetListEventsCausesDeath(severeIllnessInfo.ID);
                    }
                    causeResult = null;
                    FormDeath = new CloseTreatment.FormDeath(currentHisTreatment, causeOfDeathADO, this.module, ActionGetCauseResult, TranPatiDataTreatmentFinish);
                    FormDeath.Form = this;
                    FormDeath.ShowDialog();
                    txtMethod.Enabled = false;
                    txtAdvised.Enabled = false;
                    cboTTExt.Enabled = false;
                    cboTTExt.EditValue = null;
                }
                else if (data.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN)
                {
                    var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == Config.ConfigKey.PatienTypeCode_BHYT);
                    if (ConfigKey.AllowManyOpeningOption == "4" && currentHisTreatment.TDL_PATIENT_TYPE_ID == patientType.ID)
                    {
                        HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                        treatmentFilter.PATIENT_ID = currentHisTreatment.PATIENT_ID;
                        treatmentFilter.TDL_PATIENT_TYPE_IDs = new List<long>() { patientType.ID };
                        treatmentFilter.IS_PAUSE = false;
                        treatmentFilter.TDL_TREATMENT_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY, IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU, IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU };
                        treatmentFilter.ID__NOT_EQUAL = this.currentHisTreatment.ID;


                        var treatmentOlds = new BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, null);
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("treatmentOlds__:", treatmentOlds));
                        if (treatmentOlds != null && treatmentOlds.Count() > 0)
                        {
                            XtraMessageBox.Show(string.Format(ResourceMessage.BNCoDotDieuTriNTCuChuaKetThucKhongChoPhepChuyen, String.Join(", ", treatmentOlds.Select(o => o.TREATMENT_CODE))), ResourceMessage.ThongBao, MessageBoxButtons.OK);
                            cboTreatmentEndType.EditValue = null;
                            return;
                        }
                    }

                    FormTransfer = new CloseTreatment.FormTransfer(this.module, currentHisTreatment);
                    FormTransfer.MyGetData = new CloseTreatment.FormTransfer.GetString(TranPatiDataTreatmentFinish);
                    
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("MyGetData", FormTransfer.MyGetData));
                    FormTransfer.Form = this; 
                    FormTransfer.ShowDialog(); 
                    cboTTExt.EditValue = null;
                }
                else if (data.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN)
                {
                    btnAppointInfo.Enabled = true;
                    var dataRoom = this.hisRooms.FirstOrDefault(o => o.ID == this.module.RoomId);

                    long dtTreatmentEnd = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtEndTime.DateTime) ?? 0;
                    FormAppointment = new CloseTreatment.FormAppointment(this.module, dtTreatmentEnd, dataRoom.IS_BLOCK_NUM_ORDER == 1 ? true : false);
                    FormAppointment.MyGetData = new CloseTreatment.FormAppointment.GetString(TranPatiDataTreatmentFinish);
                    FormAppointment.Form = this;
                    FormAppointment.ShowDialog();
                    cboTTExt.EditValue = null;
                }
                else if (data.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN ||
                    data.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN)
                {
                    if (txtEndOrder.Enabled)
                    {
                        lciEndOrder.AppearanceItemCaption.ForeColor = Color.Maroon;
                        ValidationEndOrder();
                    }

                    btnAppointInfo.Enabled = true;

                    cboTTExt.EditValue = null;

                    if (data.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN &&
                    Inventec.Common.TypeConvert.Parse.ToInt64((cboResult.EditValue ?? 0).ToString()) == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__NANG)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__NANG___:");
                        if (currentHisTreatment != null)
                        {
                            List<object> listArgs = new List<object>();
                            listArgs.Add(currentHisTreatment.ID);
                            listArgs.Add(true);
                            CallModule.Run(CallModule.InformationAllowGoHome, module.RoomId, module.RoomTypeId, listArgs);
                        }
                        else
                        {
                            throw new ArgumentNullException("Treatment is null");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ActionGetCauseResult(CauseOfDeathADO obj)
        {
            try
            {
                causeResult = obj;

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => causeResult), causeResult));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }
        private List<HIS_EVENTS_CAUSES_DEATH> GetListEventsCausesDeath(long _SsIllnessInfoId)
        {
            List<HIS_EVENTS_CAUSES_DEATH> rs = null;
            try
            {
                HisEventsCausesDeathFilter ft = new HisEventsCausesDeathFilter();
                ft.SEVERE_ILLNESS_INFO_ID = _SsIllnessInfoId;
                rs = new BackendAdapter(new CommonParam()).Get<List<HIS_EVENTS_CAUSES_DEATH>>("api/HisEventsCausesDeath/Get", ApiConsumers.MosConsumer, ft, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return rs;
        }

        private HIS_SEVERE_ILLNESS_INFO GetSevereIllnessInfo(long _TreatmentId)
        {
            HIS_SEVERE_ILLNESS_INFO rs = null;
            try
            {
                HisSevereIllnessInfoFilter ft = new HisSevereIllnessInfoFilter();
                ft.TREATMENT_ID = _TreatmentId;
                ft.IS_DEATH = true;
                rs = new BackendAdapter(new CommonParam()).Get<List<HIS_SEVERE_ILLNESS_INFO>>("api/HisSevereIllnessInfo/Get", ApiConsumers.MosConsumer, ft, null).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return rs;
        }

        private void txtDoctorLogginName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtDoctorLogginName.Text.Trim()))
                    {
                        string code = txtDoctorLogginName.Text.Trim().ToLower();
                        var listData = this.listDoctors.Where(o => o.LOGINNAME.ToLower().Contains(code)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.LOGINNAME.ToLower() == code).ToList() : listData) : null;
                        if (result != null && result.Count > 0)
                        {
                            showCbo = false;
                            txtDoctorLogginName.Text = result.First().LOGINNAME;
                            cboDoctorUserName.EditValue = result.First().LOGINNAME;
                            cboDoctorUserName.Properties.Buttons[1].Visible = true;
                            cboTreatmentEndType.Focus();
                        }
                    }
                    if (showCbo)
                    {
                        cboDoctorUserName.Focus();
                        cboDoctorUserName.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDoctorUserName_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    txtDoctorLogginName.Text = "";
                    cboDoctorUserName.EditValue = null;
                    cboDoctorUserName.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDoctorUserName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboDoctorUserName.EditValue != null)
                    {
                        var data = this.listDoctors.FirstOrDefault(o => o.LOGINNAME.ToLower() == cboDoctorUserName.EditValue.ToString().ToLower());
                        if (data != null)
                        {
                            txtDoctorLogginName.Text = data.LOGINNAME;
                            cboDoctorUserName.Properties.Buttons[1].Visible = true;
                        }
                        cboTreatmentEndType.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDoctorUserName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboDoctorUserName.EditValue != null)
                    {
                        var data = this.listDoctors.FirstOrDefault(o => o.LOGINNAME.ToLower() == cboDoctorUserName.EditValue.ToString().ToLower());
                        if (data != null)
                        {
                            txtDoctorLogginName.Text = data.LOGINNAME;
                            cboDoctorUserName.Properties.Buttons[1].Visible = true;
                        }
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    cboDoctorUserName.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMethod_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F5)
                {

                    List<object> listArgs = new List<object>();
                    listArgs.Add("phuongphapdieutri");
                    listArgs.Add((HIS.Desktop.Common.DelegateDataTextLib)ProcessDataTextLib_txtMethod);
                    if (this.module == null)
                    {
                        CallModule.Run(CallModule.TextLibrary, 0, 0, listArgs);
                    }
                    else
                    {
                        CallModule.Run(CallModule.TextLibrary, this.module.RoomId, this.module.RoomTypeId, listArgs);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataTextLib_txtMethod(MOS.EFMODEL.DataModels.HIS_TEXT_LIB textLib)
        {
            try
            {
                txtMethod.Text = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region click


        private void btnSave_Click(object sender, EventArgs e)
        {
            LogTheadInSessionInfo(save, "btnSave_Click");
        }
        private bool CheckWarningUnfinishedServiceOption()
        {
            bool valid = true;
            try
            {
                if (ConfigKey.WarningUnfinishedServiceOption == "1" || ConfigKey.WarningUnfinishedServiceOption == "2")
                {
                    HisServiceReqFilter srFilter = new HisServiceReqFilter();
                    srFilter.TREATMENT_ID = treatmentId;
                    List<HIS_SERVICE_REQ> serviceReqs = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, srFilter, null);
                    if (serviceReqs != null && serviceReqs.Count > 0)
                    {
                        serviceReqs = serviceReqs.Where(o => o.SERVICE_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT && o.IS_NOT_REQUIRED_COMPLETE == 1).ToList();
                        if (serviceReqs != null && serviceReqs.Count > 0)
                        {
                            CommonParam param = new CommonParam();
                            HisSereServFilter ssFilter = new HisSereServFilter();
                            ssFilter.TREATMENT_ID = this.treatmentId;
                            ssFilter.SERVICE_REQ_IDs = serviceReqs.Select(o => o.ID).ToList();
                            var SereServCheck = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, param);
                            if (SereServCheck != null && SereServCheck.Count > 0)
                            {
                                if (ConfigKey.WarningUnfinishedServiceOption == "1")
                                {
                                    SereServCheck = SereServCheck.Where(o => o.IS_NO_EXECUTE != 1 && o.IS_DELETE != 1).ToList();
                                }
                                if (ConfigKey.WarningUnfinishedServiceOption == "2")
                                {
                                    SereServCheck = SereServCheck.Where(o => o.IS_NO_EXECUTE != 1 && o.IS_DELETE != 1 && o.PATIENT_TYPE_ID == BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(p => p.PATIENT_TYPE_CODE == Config.ConfigKey.PatienTypeCode_BHYT).ID).ToList();
                                }
                                if (SereServCheck != null && SereServCheck.Count > 0)
                                {
                                    List<string> lst = new List<string>();
                                    foreach (var item in SereServCheck)
                                    {
                                        lst.Add(String.Format("{0} ({1})", item.TDL_SERVICE_REQ_CODE, item.TDL_SERVICE_NAME));
                                    }
                                    if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.YLenhChuanHoanThanh, String.Join(",", lst)), ResourceMessage.ThongBao, MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                                    {
                                        valid = false;
                                    }
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
            return valid;
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

        private void ProcessHein(ref HisTreatmentFinishSDO sdo)
        {
            try
            {
                List<string> rs = new List<string>() { "000", "001", "002", "003", "004" };
                HIS_PATIENT_TYPE_ALTER obj = new HIS_PATIENT_TYPE_ALTER();

                if (ResultDataADO == null || ResultDataADO.ResultHistoryLDO == null || ResultDataADO.ResultHistoryLDO.maKetQua == "000")
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_PATIENT_TYPE_ALTER>(obj, patientTypeAlter);
                    sdo.patientTypeAlter = obj;
                }
                else if (ResultDataADO.ResultHistoryLDO.maKetQua == "001" || ResultDataADO.ResultHistoryLDO.maKetQua == "002")
                {
                    sdo.IsCreateNewTreatment = false;
                }
                else if (ResultDataADO.ResultHistoryLDO.maKetQua == "003" || ResultDataADO.ResultHistoryLDO.maKetQua == "004")
                {
                    if (!string.IsNullOrEmpty(ResultDataADO.ResultHistoryLDO.gtTheTuMoi))
                        patientTypeAlter.HEIN_CARD_FROM_TIME = Int64.Parse(ResultDataADO.ResultHistoryLDO.gtTheTuMoi.Split('/')[2] + ResultDataADO.ResultHistoryLDO.gtTheTuMoi.Split('/')[1] + ResultDataADO.ResultHistoryLDO.gtTheTuMoi.Split('/')[0] + "000000");
                    if (!string.IsNullOrEmpty(ResultDataADO.ResultHistoryLDO.gtTheDenMoi))
                        patientTypeAlter.HEIN_CARD_TO_TIME = Int64.Parse(ResultDataADO.ResultHistoryLDO.gtTheDenMoi.Split('/')[2] + ResultDataADO.ResultHistoryLDO.gtTheDenMoi.Split('/')[1] + ResultDataADO.ResultHistoryLDO.gtTheDenMoi.Split('/')[0] + "000000");
                    patientTypeAlter.HEIN_MEDI_ORG_CODE = ResultDataADO.ResultHistoryLDO.maDKBDMoi;
                    patientTypeAlter.HEIN_MEDI_ORG_NAME = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_ORG>().FirstOrDefault(o => o.MEDI_ORG_CODE == ResultDataADO.ResultHistoryLDO.maDKBDMoi).MEDI_ORG_NAME;
                    patientTypeAlter.HEIN_CARD_NUMBER = ResultDataADO.ResultHistoryLDO.maTheMoi;
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_PATIENT_TYPE_ALTER>(obj, patientTypeAlter);
                    sdo.patientTypeAlter = obj;
                }
                else if (ResultDataADO.ResultHistoryLDO.maKetQua == "9999")
                {
                    XtraMessageBox.Show(ResultDataADO.ResultHistoryLDO.message, ResourceMessage.ThongBao, MessageBoxButtons.OK);
                    sdo.IsCreateNewTreatment = false;
                }
                else if (!rs.Exists(o => o == ResultDataADO.ResultHistoryLDO.maKetQua) || !string.IsNullOrEmpty(ResultDataADO.ResultHistoryLDO.ketQuaDangKyTocken))
                {
                    if (XtraMessageBox.Show(ResultDataADO.ResultHistoryLDO.message + ". Bạn có muốn đổi đối tượng sang viện phí không?", ResourceMessage.ThongBao, MessageBoxButtons.YesNo, DevExpress.Utils.DefaultBoolean.True) == System.Windows.Forms.DialogResult.Yes)
                    {
                        patientTypeAlter.PATIENT_TYPE_ID = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == Config.ConfigKey.patientTypeCodeHospitalFee).ID;
                        patientTypeAlter.HEIN_CARD_FROM_TIME = null;
                        patientTypeAlter.HEIN_CARD_TO_TIME = null;
                        patientTypeAlter.HEIN_MEDI_ORG_CODE = null;
                        patientTypeAlter.HEIN_MEDI_ORG_NAME = null;
                        patientTypeAlter.HEIN_CARD_NUMBER = null;
                        patientTypeAlter.JOIN_5_YEAR = null;
                        patientTypeAlter.BHYT_URL = null;
                        patientTypeAlter.FREE_CO_PAID_TIME = null;
                        patientTypeAlter.RIGHT_ROUTE_CODE = null;
                        patientTypeAlter.RIGHT_ROUTE_TYPE_CODE = null;
                        patientTypeAlter.PAID_6_MONTH = null;
                        patientTypeAlter.HAS_BIRTH_CERTIFICATE = null;
                        patientTypeAlter.IS_NO_CHECK_EXPIRE = null;
                        patientTypeAlter.JOIN_5_YEAR_TIME = null;
                        patientTypeAlter.LEVEL_CODE = null;
                        patientTypeAlter.TT46_NOTE = null;
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_PATIENT_TYPE_ALTER>(obj, patientTypeAlter);
                        sdo.patientTypeAlter = obj;
                    }
                    else
                    {
                        sdo.IsCreateNewTreatment = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool ValidationUser()
        {
            bool isValid = true;
            try
            {
                if ((string.IsNullOrEmpty(txtEndDeptSubsHead.Text.Trim()) || cboEndDeptSubsHead.EditValue == null) && this.layoutControlItem46.AppearanceItemCaption.ForeColor == Color.Brown)
                {
                    dxErrorProvider1.SetError(txtEndDeptSubsHead, ResourceMessage.TruongDuLieuBatBuoc, ErrorType.Warning);
                    isValid = false;
                }
                if ((string.IsNullOrEmpty(txtHospSubsDirector.Text.Trim()) || cboHospSubsDirector.EditValue == null) && this.layoutControlItem43.AppearanceItemCaption.ForeColor == Color.Brown)
                {
                    dxErrorProvider1.SetError(txtHospSubsDirector, ResourceMessage.TruongDuLieuBatBuoc, ErrorType.Warning);
                    isValid = false;
                }

            }
            catch (Exception ex)
            {
                isValid = false;
                LogSystem.Error(ex);
            }
            return isValid;
        }
        private async void save()
        {
            try
            {
                if (!btnSave.Enabled) return;
                bool success = false;
                this.positionHandle = -1;
                if (cboTreatmentEndType.EditValue != null && Int64.Parse(cboTreatmentEndType.EditValue.ToString()) == 2)
                {
                    layoutControlItem25.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidateTextEdit(txtDauHieuLamSang);
                }
                bool valid = (bool)icdProcessor.ValidationIcd(ucIcd);
                valid = (bool)subIcdProcessor.GetValidate(ucSecondaryIcd) && valid;
                valid = IsValiICDCause() && valid;
                valid = dxValidationProvider.Validate() && valid;
                if (!ValidationUser()) return;
                if (!valid) return;
                bool IsContinue = true;
                IsContinue = IsContinue && CheckSA(true);
                IsContinue = IsContinue && CheckBedLog(true);
                if (!IsContinue)
                    return;

                GetValueUC();
                if (Inventec.Common.String.CountVi.Count(codeCheckCD) > 100)
                {
                    XtraMessageBox.Show("Mã chẩn đoán phụ nhập quá 100 ký tự.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (Inventec.Common.String.CountVi.Count(nameCheckCD) > 1500)
                {
                    XtraMessageBox.Show("Tên chẩn đoán phụ nhập quá 1500 ký tự.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (Inventec.Common.String.CountVi.Count(codeCheckCDYHCT) > 255)
                {
                    XtraMessageBox.Show("Mã chẩn đoán YHCT phụ nhập quá 255 ký tự.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!string.IsNullOrEmpty(codeCheckSubICD))
                {
                    var checkICDSubCode = codeCheckSubICD.Split(';').ToList();
                    if (checkICDSubCode != null && checkICDSubCode.Count > 12)
                    {
                        if (Config.ConfigKey.IsCheckSubIcdExceedLimit == "1")
                        {
                            XtraMessageBox.Show("Chẩn đoán phụ nhập quá 12 mã bệnh. Vui lòng kiểm tra lại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        else if (Config.ConfigKey.IsCheckSubIcdExceedLimit == "2")
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show("Chẩn đoán phụ nhập quá 12 mã bệnh. Bạn có muốn tiếp tục không?",
                               "Thông báo",
                              MessageBoxButtons.YesNo) == DialogResult.No)
                                return;
                        }
                    }
                }
                List<WarningADO> warningADONew = new List<WarningADO>();
                if ((ConfigKey.MustChooseSeviceExamOption == "1" || ConfigKey.MustChooseSeviceExamOption == "2") && !this.CheckMustChooseSeviceExamOption()) return;
                if (!this.CheckAssignServiceBed_ForSave(ValidationDataType.PopupMessage, ref warningADONew))
                {
                    return;
                }

                if (!this.Check_INTRUCTION_TIME_and_DEPARTMENT_IN_TIME_ForSave(ValidationDataType.PopupMessage, ref warningADONew))
                {
                    return;
                }

                if (!this.CheckWarnNotRequiredCompleteHasNoSample(ValidationDataType.PopupMessage, ref warningADONew))
                {
                    return;
                }

                if (!this.CheckSameHein_ForSave(ValidationDataType.PopupMessage, ref warningADONew))
                {
                    return;
                }

                if (!this.CheckRation_ForSave(ValidationDataType.PopupMessage, ref warningADONew))
                {
                    return;
                }

                if (!this.Check_UNSIGN_DOC_FINISH_OPTION_ForSave(ValidationDataType.PopupMessage, ref warningADONew))
                {
                    return;
                }

                if (!this.CheckDHST_ForSave(ValidationDataType.PopupMessage, ref warningADONew))
                {
                    return;
                }

                if (!this.CheckUnassignTrackingServiceReq_ForSave(ValidationDataType.PopupMessage, ref warningADONew))
                {
                    return;
                }

                if (!this.CheckWarningUnfinishedServiceOption())
                {
                    return;
                }

                if (!this.Check_IsAllowTreatmentFinishDepartmentIsActiveFee_ForSave())
                {
                    return;
                }

                HIS.Desktop.Plugins.Library.CheckIcd.CheckIcdManager check = new Desktop.Plugins.Library.CheckIcd.CheckIcdManager(null, currentHisTreatment);
                string message = null;
                if (CheckIcdWhenSave == "1" || CheckIcdWhenSave == "2")
                {
                    string icdCode = "", icdSubCode = "";
                    if (ucIcd != null)
                    {
                        var icdValue = icdProcessor.GetValue(ucIcd);
                        if (icdValue != null && icdValue is IcdInputADO)
                        {
                            icdCode = ((IcdInputADO)icdValue).ICD_CODE;
                        }
                    }
                    if (ucSecondaryIcd != null)
                    {
                        var subIcd = subIcdProcessor.GetValue(ucSecondaryIcd);
                        if (subIcd != null && subIcd is SecondaryIcdDataADO)
                        {
                            icdSubCode = ((SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                        }
                    }
                    if (!check.ProcessCheckIcd(icdCode, icdSubCode, ref message, true, true))
                    {
                        if (CheckIcdWhenSave == "1" && !String.IsNullOrEmpty(message))
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("{0}. Bạn có muốn tiếp tục không?", message),
                            "Thông báo",
                           MessageBoxButtons.YesNo) == DialogResult.No)
                                return;
                        }
                        if (CheckIcdWhenSave == "2" && !String.IsNullOrEmpty(message))
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(message, "Thông báo", MessageBoxButtons.OK);
                            return;
                        }
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("currentHisTreatment", currentHisTreatment));

                if (this.currentHisTreatment.TDL_TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU && chkCapSoLuuTruBA.Checked)
                {
                    var program = ProgramADOList.FirstOrDefault(o => o.ID == Convert.ToInt64(cboProgram.EditValue));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("program", program));
                    if (program != null && program.AUTO_CHANGE_TO_OUT_PATIENT == 1)
                    {
                        XtraMessageBox.Show(String.Format(ResourceMessage.ChuongTrinhBatBuocChonDienDieuTriNgoaiTruHoSoSeDuocTuDongCapNhatSangDienDieuTriNgoaiTru, program.PROGRAM_NAME), ResourceMessage.ThongBao);
                    }
                }

                Inventec.Common.Logging.LogSystem.Info("Save treatmentFinish 5");
                if (hisTreatmentFinishSDO_process == null) hisTreatmentFinishSDO_process = new MOS.SDO.HisTreatmentFinishSDO();
                var treatmentEndType = this.hisTreatmentEndTypes.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentEndType.EditValue ?? 0).ToString()));
                var treatmentEndTypeExt = this.hisTreatmentEndTypeExts.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboTTExt.EditValue ?? 0).ToString()));
                if (treatmentEndType != null)
                {
                    hisTreatmentFinishSDO_process.TreatmentEndTypeId = treatmentEndType.ID;
                }

                if (dtNewTreatmentTime.Enabled)
                    hisTreatmentFinishSDO_process.NewTreatmentInTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtNewTreatmentTime.DateTime);

                if (Config.ConfigKey.RequiredTreatmentMethodOption == "1" && this.currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU && (treatmentEndType.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN
                    || treatmentEndType.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN || treatmentEndType.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN
                    || treatmentEndType.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN || treatmentEndType.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CTCV))
                {
                    if (String.IsNullOrEmpty(txtMethod.Text) && String.IsNullOrEmpty(hisTreatmentFinishSDO_process.TreatmentMethod))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ChuaNhapPhuongPhapDieuTri, ResourceMessage.ThongBao);
                        return;
                    }
                    if (!String.IsNullOrEmpty(hisTreatmentFinishSDO_process.TreatmentMethod) && String.IsNullOrEmpty(txtMethod.Text))
                    {
                        txtMethod.Text = hisTreatmentFinishSDO_process.TreatmentMethod;
                    }
                    else
                    {
                        hisTreatmentFinishSDO_process.TreatmentMethod = txtMethod.Text;
                    }
                }
                else if (Config.ConfigKey.RequiredTreatmentMethodOption == "2" && ((treatmentEndTypeExt != null && treatmentEndTypeExt.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM) || ((this.currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU || this.currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU || this.currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY) && (treatmentEndType.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN
                    || treatmentEndType.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN || treatmentEndType.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN
                    || treatmentEndType.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN || treatmentEndType.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CTCV))))
                {

                    if (String.IsNullOrEmpty(txtMethod.Text) && String.IsNullOrEmpty(hisTreatmentFinishSDO_process.TreatmentMethod))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ChuaNhapPhuongPhapDieuTri, ResourceMessage.ThongBao);
                        return;
                    }
                    if (!String.IsNullOrEmpty(hisTreatmentFinishSDO_process.TreatmentMethod) && String.IsNullOrEmpty(txtMethod.Text))
                    {
                        txtMethod.Text = hisTreatmentFinishSDO_process.TreatmentMethod;
                    }
                    else
                    {
                        hisTreatmentFinishSDO_process.TreatmentMethod = txtMethod.Text;
                    }
                }

                if (treatmentEndTypeExt != null)
                {
                    hisTreatmentFinishSDO_process.TreatmentEndTypeExtId = treatmentEndTypeExt.ID;
                }

                hisTreatmentFinishSDO = new MOS.SDO.HisTreatmentFinishSDO();
                bool rs = await ProcessDataBeforeSaveAsync(this, true);
                if (rs)
                {
                    return;
                }
                if (hisTreatmentFinishSDO != null && Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentEndType.EditValue ?? "0").ToString()) == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN && (string.IsNullOrEmpty(hisTreatmentFinishSDO.ClinicalNote) || string.IsNullOrEmpty(hisTreatmentFinishSDO.TreatmentDirection) || string.IsNullOrEmpty(hisTreatmentFinishSDO.TreatmentMethod) && string.IsNullOrEmpty(hisTreatmentFinishSDO.TransportVehicle) || (string.IsNullOrEmpty(hisTreatmentFinishSDO.TransporterLoginnames) && string.IsNullOrEmpty(hisTreatmentFinishSDO.Transporter)) || string.IsNullOrEmpty(hisTreatmentFinishSDO.TransferOutMediOrgCode) || !hisTreatmentFinishSDO.TranPatiReasonId.HasValue || !hisTreatmentFinishSDO.TranPatiFormId.HasValue))
                {
                    XtraMessageBox.Show("Thiếu thông tin chuyển viện", "Thông báo");
                    return;
                }
                CommonParam param = new CommonParam();
                SaveTreatmentFinish(hisTreatmentFinishSDO, ref success, ref param);
                MessageManager.Show(this, param, success);
                if (success)
                {
                    Inventec.Common.Logging.LogSystem.Warn("WarningOption: " + ConfigKey.WarningOption);
                    HIS_TREATMENT HisTreatment = hisTreatmentResult;

                    if (ConfigKey.WarningOption == 1 && HisTreatment != null && HisTreatment.PROGRAM_ID != null && HisTreatment.EMR_COVER_TYPE_ID == null)
                    {
                        if (MessageBox.Show(ResourceMessage.ChuaDuocTaoVoBenhAn, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            VoBenhAn(HisTreatment);
                        }
                    }      
                    
                    if(AppointmentPrintOptionsStorageADO.InPhieuHenKham || AppointmentPrintOptionsStorageADO.KyPhieuHenKham || AppointmentPrintOptionsStorageADO.XemTruocKhiIn)
                    {
                        var menuItem = new DevExpress.Utils.Menu.DXMenuItem("Hẹn khám lại");
                        menuItem.Tag = ModuleTypePrint.HEN_KHAM_LAI;
                        PrintCloseTreatment_Click(menuItem, null);
                    }
                    else
                    {
                        RunAutoPrintByPrintConfig();
                    }
                   
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisTreatmentFinishSDO), hisTreatmentFinishSDO));
                    if (hisTreatmentFinishSDO.TreatmentEndTypeExtId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM)
                    {
                        ReLoadPrintTreatmentEndTypeExt(PrintTreatmentEndTypeExPrintType.TYPE.NGHI_OM);
                    }
                    else if (hisTreatmentFinishSDO.TreatmentEndTypeExtId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_DUONG_THAI)
                    {
                        ReLoadPrintTreatmentEndTypeExt(PrintTreatmentEndTypeExPrintType.TYPE.NGHI_DUONG_THAI);
                    }

                    this.btnDHST.Enabled = false;
                    this.btnDeleteEndInfo.Enabled = false;

                    SetPrintMenu(hisTreatmentFinishSDO.TreatmentEndTypeId, hisTreatmentFinishSDO.TreatmentEndTypeExtId, this.currentHisTreatment.TDL_TREATMENT_TYPE_ID);

                    BtnEndCode.Enabled = CheckTreatmentEndCode();
                }
                Inventec.Common.Logging.LogSystem.Info("Save treatmentFinish 6"); 
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private object GetPreviewTypeForMps10(bool inPhieuHenKham, bool kyPhieuHenKham, bool xemTruocKhiIn)
        {
            throw new NotImplementedException();
        }
        
        private bool CheckMustChooseSeviceExamOption()
        {
            bool rs = true;
            try
            {
                bool isTreatmentFinish = false;
                string serviceReqCode = "";
                if (examServiceReqs == null || examServiceReqs.Count == 0)
                {
                    HisServiceReqFilter srFilter = new HisServiceReqFilter();
                    srFilter.TREATMENT_ID = currentHisTreatment.ID;
                    srFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                    srFilter.HAS_EXECUTE = true; //IS_NO_EXECUTE != 1 && o.IS_DELETE != 1
                    examServiceReqs = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, srFilter, null);
                }
                if (examServiceReqs != null && examServiceReqs.Count > 0)
                {
                    var serviceReqs = examServiceReqs.Where(o => string.IsNullOrEmpty(o.TDL_SERVICE_IDS)).ToList();
                    if (serviceReqs != null && serviceReqs.Count > 0)
                    {
                        serviceReqCode = string.Join(", ", serviceReqs.Select(o => o.SERVICE_REQ_CODE));
                        isTreatmentFinish = true;
                    }
                }
                if (ConfigKey.MustChooseSeviceExamOption == "1")
                {
                    if (!string.IsNullOrEmpty(serviceReqCode) && (XtraMessageBox.Show(String.Format(ResourceMessage.YLenhThieuDichVuKhamBanCoMuonTiepTuc, serviceReqCode), ResourceMessage.ThongBao, MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes))
                        rs = false;
                }
                else if (ConfigKey.MustChooseSeviceExamOption == "2")
                {
                    if (!string.IsNullOrEmpty(serviceReqCode))
                    {
                        if (isTreatmentFinish)
                            XtraMessageBox.Show(String.Format("Y lệnh {0} thiếu dịch vụ khám. Không cho phép kết thúc điều trị.", serviceReqCode), ResourceMessage.ThongBao);
                        else
                            XtraMessageBox.Show(String.Format("Y lệnh {0} thiếu dịch vụ khám. Bạn không thể kết thúc y lệnh khám.", serviceReqCode), ResourceMessage.ThongBao);
                        rs = false;
                    }
                }
            }
            catch (Exception ex)
            {
                rs = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return rs;
        }

        /// <summary>
        /// kiểm tra văn bản
        /// nếu có hiển thị thông báo và chọn ok thì trả về true để không lưu
        /// nếu không thì trả về false để lưu.
        /// </summary>
        /// <returns></returns>
        private bool CheckEmrDocumentData(HIS_TREATMENT_TYPE treatmentType, ValidationDataType validationDataType, ref List<WarningADO> listWarningADO)
        {
            bool result = false;
            try
            {
                if (treatmentType == null || (treatmentType.UNSIGN_DOC_FINISH_OPTION != 1
                                            && treatmentType.UNSIGN_DOC_FINISH_OPTION != 2))
                {
                    return result;
                }
                CommonParam param = new CommonParam();
                var document = new BackendAdapter(param).Post<MediRecordCheckingResultSDO>("api/EmrDocument/MediRecordChecking", ApiConsumers.EmrConsumer, this.currentHisTreatment.TREATMENT_CODE, param);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("document", document));
                if (document != null)
                {
                    List<string> messages = new List<string>();
                    List<MissingDocumentADO> messagesADO = new List<MissingDocumentADO>();
                    if (document.SignatureMissingDocuments != null && document.SignatureMissingDocuments.Count > 0)
                    {
                        foreach (var doc in document.SignatureMissingDocuments)
                        {
                            MissingDocumentADO ado = new MissingDocumentADO();

                            if (doc.UN_SIGNERS != null && doc.UN_SIGNERS.Contains(","))
                            {
                                List<string> str = new List<string>();
                                var arr = doc.UN_SIGNERS.Split(',');
                                for (int i = 0; i < arr.Length; i++)
                                {
                                    if (!arr[i].Contains("#@!@#"))
                                        str.Add(arr[i]);
                                }
                                ado.loginName = string.Join(", ", str);
                                ado.documentName = doc.DOCUMENT_NAME;
                                messagesADO.Add(ado);
                            }
                            else
                            {
                                if ((doc.UN_SIGNERS != null && !doc.UN_SIGNERS.Contains("#@!@#")) || (doc.SIGNERS == null && doc.UN_SIGNERS == null))
                                {
                                    ado.loginName = doc.UN_SIGNERS;
                                    ado.documentName = doc.DOCUMENT_NAME;
                                    messagesADO.Add(ado);
                                }
                            }
                        }
                    }

                    if (document.MandatoryMissingDocuments != null && document.MandatoryMissingDocuments.Count > 0)
                    {
                        MissingDocumentADO ado = new MissingDocumentADO();
                        ado.documentName = String.Join(", ", document.MandatoryMissingDocuments);
                        messagesADO.Add(ado);
                    }

                    if (messagesADO.Count > 0)
                    {
                        string mess = "";
                        foreach (var item in messagesADO)
                        {
                            mess += "Văn bản " + item.documentName + (!string.IsNullOrEmpty(item.loginName) ? " có tài khoản " + item.loginName : " ") + " chưa hoàn thành chữ ký.\r\n";
                        }
                        if (validationDataType == ValidationDataType.PopupMessage)
                        {
                            mess += " \r\n";
                            mess += ResourceMessage.BanCoMuonThucHienKhong;
                            if (XtraMessageBox.Show(mess, ResourceMessage.ThongBao, MessageBoxButtons.YesNo, DevExpress.Utils.DefaultBoolean.True) == System.Windows.Forms.DialogResult.Yes)
                            {
                                //gọi module
                                List<object> listArgs = new List<object>();
                                listArgs.Add(this.currentHisTreatment.TREATMENT_CODE);
                                listArgs.Add(true);

                                if (this.module == null)
                                {
                                    CallModule.Run(CallModule.EmrDocument, 0, 0, listArgs);
                                }
                                else
                                {
                                    CallModule.Run(CallModule.EmrDocument, this.module.RoomId, this.module.RoomTypeId, listArgs);
                                }
                                result = true;
                            }
                            else if (treatmentType.UNSIGN_DOC_FINISH_OPTION == 1)   //Cảnh báo
                            {
                                result = false;
                            }
                            else if (treatmentType.UNSIGN_DOC_FINISH_OPTION == 2)   //Chặn
                            {
                                result = true;
                            }
                        }
                        else if (validationDataType == ValidationDataType.GetListMessage && listWarningADO != null)
                        {
                            WarningADO warning = new WarningADO();
                            if (treatmentType.UNSIGN_DOC_FINISH_OPTION == 1)   //Cảnh báo
                            {
                                warning.IsSkippable = true;
                            }
                            else if (treatmentType.UNSIGN_DOC_FINISH_OPTION == 2)   //Chặn
                            {
                                warning.IsSkippable = false;
                            }
                            warning.Description = mess;
                            listWarningADO.Add(warning);
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

        private async Task ReLoadPrintTreatmentEndTypeExt(PrintTreatmentEndTypeExPrintType.TYPE exPrintType)
        {
            try
            {
                PrintTreatmentEndTypeExtProcessor printTreatmentEndTypeExtProcessor = new PrintTreatmentEndTypeExtProcessor(this.treatmentId, ReloadMenuTreatmentEndTypeExt, CreateMenu.TYPE.DYNAMIC, this.currentModuleBase != null ? this.currentModuleBase.RoomId : 0);

                printTreatmentEndTypeExtProcessor.Print(exPrintType,
                    HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt.PrintTreatmentEndTypeExtProcessor.OPTION.INIT_MENU);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReloadMenuTreatmentEndTypeExt(object data)
        {
            try
            {
                DXMenuItem dXMenuItem = data as DXMenuItem;
                if (dXMenuItem != null)
                {
                    DXPopupMenu menu = btnPrint.DropDownControl as DXPopupMenu;
                    menu.Items.Add(dXMenuItem);
                    btnPrint.DropDownControl = menu;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool IsValiICDCause()
        {
            bool result = true;
            try
            {
                result = (bool)this.IcdCauseProcessor.ValidationIcd(this.ucIcdCause, Template.NoFocus);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void GetPatientTypeAlter()
        {
            try
            {
                if (patientTypeAlter == null)
                    patientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetLastByTreatmentId", ApiConsumers.MosConsumer, currentHisTreatment.ID, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSaveTemp_Click(object sender, EventArgs e)
        {
            LogTheadInSessionInfo(saveTemp, "btnSaveTemp_Click");
        }
        //sua lai viec 181736
        string codeCheckCD = "";
        string nameCheckCD = "";
        string codeCheckCDYHCT = "";
        string codeCheckSubICD = "";
        private void GetValueUC()
        {
            try
            {
                codeCheckCD = "";
                nameCheckCD = "";
                codeCheckCDYHCT = "";
                codeCheckSubICD = "";
                if (ucSecondaryIcd != null)
                {
                    var subIcd = subIcdProcessor.GetValue(ucSecondaryIcd);
                    if (subIcd != null && subIcd is SecondaryIcdDataADO)
                    {
                        codeCheckSubICD += ((SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                        codeCheckCD += ((SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                        nameCheckCD += ((SecondaryIcdDataADO)subIcd).ICD_TEXT;
                    }
                }
                if (ucIcd != null)
                {
                    var icdValue = icdProcessor.GetValue(ucIcd);
                    if (icdValue is IcdInputADO)
                    {
                        codeCheckCD += ((IcdInputADO)icdValue).ICD_CODE;
                        nameCheckCD += ((IcdInputADO)icdValue).ICD_NAME;
                    }
                }
                if (ucSecondaryIcdYhct != null)
                {
                    var subIcdYHCT = subIcdYhctProcessor.GetValue(ucSecondaryIcdYhct);
                    var icd = BackendDataWorker.Get<HIS_ICD>()
                        .Where(s => s.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && s.IS_TRADITIONAL == 1).ToList();
                    if (subIcdYHCT != null && subIcdYHCT is SecondaryIcdDataADO)
                    {
                        codeCheckCDYHCT = ((SecondaryIcdDataADO)subIcdYHCT).ICD_SUB_CODE;
                        if (!string.IsNullOrEmpty(codeCheckCDYHCT))
                        {
                            foreach (var item in codeCheckCDYHCT.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList())
                            {
                                if (!icd.Exists(o => o.ICD_CODE == item))
                                {
                                    MessageBox.Show("Chẩn đoán YHCT phụ không có trong danh mục");
                                    throw new InvalidOperationException("Chẩn đoán YHCT phụ không có trong danh mục"); // Ném ngoại lệ khi có lỗi
                                }
                            }
                        }

                    }
                }
                if (ucIcdYhct != null)
                {
                    var IcdYHCT = icdYhctProcessor.GetValue(ucIcdYhct, Template.NoFocus);
                    if (IcdYHCT != null && IcdYHCT is IcdInputADO)
                    {
                        codeCheckCDYHCT += ((IcdInputADO)IcdYHCT).ICD_CODE;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private async void saveTemp()
        {
            try
            {
                if (!btnSaveTemp.Visible) return;
                this.positionHandle = -1;

                bool valid = (bool)icdProcessor.ValidationIcd(ucIcd);
                valid = (bool)subIcdProcessor.GetValidate(ucSecondaryIcd) && valid;
                valid = this.IsValiICDCause();
                valid = dxValidationProvider.Validate() && valid;
                if (!valid) return;

                List<WarningADO> warningADONew = new List<WarningADO>();
                
                if (!this.CheckWarnNotRequiredCompleteHasNoSample(ValidationDataType.PopupMessage, ref warningADONew))
                {
                    return;
                }
                GetValueUC();
                if (Inventec.Common.String.CountVi.Count(codeCheckCD) > 100)
                {
                    XtraMessageBox.Show("Mã chẩn đoán phụ nhập quá 100 ký tự.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (Inventec.Common.String.CountVi.Count(nameCheckCD) > 1500)
                {
                    XtraMessageBox.Show("Tên chẩn đoán phụ nhập quá 1500 ký tự.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (Inventec.Common.String.CountVi.Count(codeCheckCDYHCT) > 255)
                {
                    XtraMessageBox.Show("Mã chẩn đoán YHCT phụ nhập quá 255 ký tự.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var checkICDSubCode = codeCheckSubICD.Split(';').ToList();
                if (checkICDSubCode != null && checkICDSubCode.Count > 12)
                {
                    if (Config.ConfigKey.IsCheckSubIcdExceedLimit == "1")
                    {
                        XtraMessageBox.Show("Chẩn đoán phụ nhập quá 12 mã bệnh. Vui lòng kiểm tra lại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else if (Config.ConfigKey.IsCheckSubIcdExceedLimit == "2")
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show("Chẩn đoán phụ nhập quá 12 mã bệnh. Bạn có muốn tiếp tục không?",
                           "Thông báo",
                          MessageBoxButtons.YesNo) == DialogResult.No)
                            return;
                    }
                }
                HIS.Desktop.Plugins.Library.CheckIcd.CheckIcdManager check = new Desktop.Plugins.Library.CheckIcd.CheckIcdManager(null, currentHisTreatment);
                string message = null;
                if (CheckIcdWhenSave == "1" || CheckIcdWhenSave == "2")
                {
                    string icdCode = "", icdSubCode = "";
                    if (ucIcd != null)
                    {
                        var icdValue = icdProcessor.GetValue(ucIcd);
                        if (icdValue != null && icdValue is IcdInputADO)
                        {
                            icdCode = ((IcdInputADO)icdValue).ICD_CODE;
                        }
                    }
                    if (ucSecondaryIcd != null)
                    {
                        var subIcd = subIcdProcessor.GetValue(ucSecondaryIcd);
                        if (subIcd != null && subIcd is SecondaryIcdDataADO)
                        {
                            icdSubCode = ((SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                        }
                    }
                    if (!check.ProcessCheckIcd(icdCode, icdSubCode, ref message, true))
                    {
                        if (CheckIcdWhenSave == "1" && !String.IsNullOrEmpty(message))
                        {
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("{0}. Bạn có muốn tiếp tục không?", message),
                            "Thông báo",
                           MessageBoxButtons.YesNo) == DialogResult.No)
                                return;
                        }
                        if (CheckIcdWhenSave == "2" && !String.IsNullOrEmpty(message))
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(message, "Thông báo", MessageBoxButtons.OK);
                            return;
                        }
                    }
                }
                bool success = false;
                if (hisTreatmentFinishSDO_process == null)
                    hisTreatmentFinishSDO_process = new MOS.SDO.HisTreatmentFinishSDO();
                var treatmentEndType = this.hisTreatmentEndTypes.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentEndType.EditValue ?? 0).ToString()));
                if (treatmentEndType != null)
                {
                    hisTreatmentFinishSDO_process.TreatmentEndTypeId = treatmentEndType.ID;
                    hisTreatmentFinishSDO_process.IsTemporary = true;
                }

                hisTreatmentFinishSDO = new MOS.SDO.HisTreatmentFinishSDO();
                bool rs = await ProcessDataBeforeSaveAsync(this, false);
                if (rs)
                {
                    return;
                }

                if (chkOutHopitalCondition.Checked == true)
                {
                    hisTreatmentFinishSDO.IsApproveFinish = true;
                    hisTreatmentFinishSDO.ApproveFinishNote = "Đủ điều kiện ra viện";
                }
                else
                {
                    hisTreatmentFinishSDO.IsApproveFinish = false;
                    hisTreatmentFinishSDO.ApproveFinishNote = null;
                }
                Inventec.Common.Logging.LogSystem.Info(" IsApproveFinish: " + hisTreatmentFinishSDO.IsApproveFinish);
                CommonParam param = new CommonParam();
                SaveTreatmentFinish(hisTreatmentFinishSDO, ref success, ref param);
                if (!string.IsNullOrEmpty(param.GetMessage()))
                    XtraMessageBox.Show(param.GetMessage(), "Thông báo");
                MessageManager.Show(this, new CommonParam(), true);
                if (success)
                {
                    SetPrintMenu(hisTreatmentFinishSDO.TreatmentEndTypeId, hisTreatmentFinishSDO.TreatmentEndTypeExtId, this.currentHisTreatment.TDL_TREATMENT_TYPE_ID);
                    BtnEndCode.Enabled = CheckTreatmentEndCode();
                    btnDeleteEndInfo.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private int TinhTuoi(long ngaySinh, DateTime hienTai)
        {
            int rs = 0;
            try
            {
                DateTime dtNgSinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ngaySinh) ?? new DateTime();

                TimeSpan diff = hienTai - dtNgSinh;
                long tongsogiay = diff.Ticks;
                if (tongsogiay < 0)
                {
                    rs = 0;
                }

                DateTime newDate = new DateTime(tongsogiay);
                rs = newDate.Year - 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return 0;
            }

            return rs;
        }

        private bool CheckDHST(long treatmentId, ref HIS_DHST dhstRef)
        {
            bool rs = false;
            try
            {
                dhstRef = null;
                CommonParam param = new CommonParam();
                HisDhstFilter filter = new HisDhstFilter();
                filter.TREATMENT_ID = treatmentId;

                var dhst = new BackendAdapter(param).Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, filter, param);
                if (dhst != null && dhst.Count > 0)
                {
                    dhstRef = dhst.FirstOrDefault(o => o.WEIGHT.HasValue);
                    rs = true;
                    if (dhstRef == null)
                    {
                        dhstRef = dhst.First();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }

            return rs;
        }
        #endregion
        #endregion

        #region Shotcut
        private void barButtonItemSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void txtEndOrder_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtEndOrder_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillEndOrder();
                    if (chkSinhLaiSoRaVien.Enabled)
                    {
                        chkSinhLaiSoRaVien.Focus();
                    }
                    else if (chkSinhLaiSoChuyenVien.Enabled)
                    {
                        chkSinhLaiSoChuyenVien.Focus();
                    }
                    else
                    {
                        chkCapSoLuuTruBA.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillEndOrder()
        {
            try
            {
                if (!String.IsNullOrEmpty(txtEndOrder.Text))
                {
                    string code = txtEndOrder.Text.Trim();
                    if (code.Length < 9 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000}", Convert.ToInt64(code));
                        txtEndOrder.Text = code;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else
                        return false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private void chkChronic_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ChkExpXml4210.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtEndOrder_Leave(object sender, EventArgs e)
        {
            try
            {
                FillEndOrder();
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
                if (currentHisTreatment != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(currentHisTreatment.ID);

                    if (this.module == null)
                    {
                        CallModule.Run(CallModule.AppointmentService, 0, 0, listArgs);
                    }
                    else
                    {
                        CallModule.Run(CallModule.AppointmentService, this.module.RoomId, this.module.RoomTypeId, listArgs);
                    }
                }
                else
                {
                    throw new ArgumentNullException("Treatment is null");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDHST_Click(object sender, EventArgs e)
        {
            try
            {
                frmDHST frm = new frmDHST(DelegateDHST, this.currentHisTreatment, this.saveDHST);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void DelegateDHST(object data)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnBedHistory_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentHisTreatment != null)
                {
                    var treatmentBedRooms = GetHisTreatmentBedRoom(currentHisTreatment.ID, null);
                    if (treatmentBedRooms == null || treatmentBedRooms.Count == 0)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.HoSoKhongCoTtVaoBuong, ResourceMessage.ThongBao);
                        return;
                    }

                    var bedRoom = BackendDataWorker.Get<V_HIS_BED_ROOM>().FirstOrDefault(o => o.ROOM_ID == module.RoomId && o.ROOM_TYPE_ID == module.RoomTypeId);

                    if (bedRoom != null)
                    {
                        var treatmentBedRoom = treatmentBedRooms.Where(o => o.TREATMENT_ID == currentHisTreatment.ID && o.BED_ROOM_ID == bedRoom.ID).ToList();
                        if (treatmentBedRoom != null && treatmentBedRoom.Count == 1)
                        {
                            List<object> listArgs = new List<object>();
                            listArgs.Add(treatmentBedRoom.FirstOrDefault());

                            if (this.module == null)
                            {
                                CallModule.Run(CallModule.BedHistory, 0, 0, listArgs);
                            }
                            else
                            {
                                CallModule.Run(CallModule.BedHistory, this.module.RoomId, this.module.RoomTypeId, listArgs);
                            }
                        }
                        else if (treatmentBedRoom != null && treatmentBedRoom.Count > 1)
                        {
                            frmCheckBedRoom frm = new frmCheckBedRoom(module, treatmentBedRoom);
                            frm.ShowDialog();
                        }
                        else
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.HoSoKhongCoTtVaoBuong + bedRoom.BED_ROOM_NAME + ResourceMessage.KhongChoGanGiuong, ResourceMessage.ThongBao);
                            return;
                        }
                    }
                    else
                    {
                        var bedRoomDepartment = BackendDataWorker.Get<V_HIS_BED_ROOM>().Where(o => o.DEPARTMENT_ID == WorkPlaceSDO.DepartmentId).ToList();
                        if (bedRoomDepartment != null && bedRoomDepartment.Count > 0)
                        {
                            var treatmentBedRoom = treatmentBedRooms.Where(o => o.TREATMENT_ID == currentHisTreatment.ID && bedRoomDepartment.Select(p => p.ID).Contains(o.BED_ROOM_ID)).ToList();

                            if (treatmentBedRoom != null && treatmentBedRoom.Count > 0)
                            {
                                frmCheckBedRoom frm = new frmCheckBedRoom(module, treatmentBedRoom);
                                frm.ShowDialog();
                            }
                            else
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.HoSoKhongCoTtVaoBuong + ResourceMessage.CuaKhoaHienTai + ResourceMessage.KhongChoGanGiuong, ResourceMessage.ThongBao);
                                return;
                            }
                        }
                        else
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.KhoaKhongCoBuong + ResourceMessage.KhongChoGanGiuong, ResourceMessage.ThongBao);
                            return;
                        }
                    }
                }
                else
                {
                    throw new ArgumentNullException("Treatment is null");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<V_HIS_TREATMENT_BED_ROOM> GetHisTreatmentBedRoom(long treatmentId, long? bedRoomId)
        {
            List<V_HIS_TREATMENT_BED_ROOM> rs = null;

            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentBedRoomFilter filter = new HisTreatmentBedRoomFilter();
                filter.TREATMENT_ID = treatmentId;
                filter.BED_ROOM_ID = bedRoomId;

                rs = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }

            return rs;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            btnPrint.ShowDropDown();
        }

        private void cboTTExt_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboTTExt.Properties.Buttons[1].Visible = (cboTTExt.EditValue != null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTTExt_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (cboTTExt.EditValue != null)
                    if (hisTreatmentFinishSDO_process == null)
                        hisTreatmentFinishSDO_process = new MOS.SDO.HisTreatmentFinishSDO();
                if (cboTTExt.EditValue != null)
                    hisTreatmentFinishSDO_process.TreatmentEndTypeExtId = Convert.ToInt64(cboTTExt.EditValue);
                else
                    hisTreatmentFinishSDO_process.TreatmentEndTypeExtId = null;

                HIS_TREATMENT_END_TYPE_EXT treatmentEndTypeExt = this.hisTreatmentEndTypeExts != null ? this.hisTreatmentEndTypeExts.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboTTExt.EditValue.ToString())) : null;

                if (treatmentEndTypeExt == null)
                    return;

                if (cboTTExt.EditValue == null)
                {
                    this.hisTreatmentFinishSDO_process.SickLeaveDay = null;
                    this.hisTreatmentFinishSDO_process.SickLeaveFrom = null;
                    this.hisTreatmentFinishSDO_process.SickLeaveTo = null;
                    this.hisTreatmentFinishSDO_process.MotherName = null;
                    this.hisTreatmentFinishSDO_process.FatherName = null;
                    this.hisTreatmentFinishSDO_process.PatientRelativeName = null;
                    this.hisTreatmentFinishSDO_process.PatientRelativeType = null;
                    this.hisTreatmentFinishSDO_process.PatientWorkPlace = null;
                    this.hisTreatmentFinishSDO_process.Babies = null;
                    this.hisTreatmentFinishSDO_process.NumOrderBlockId = null;
                    this.hisTreatmentFinishSDO_process.AppointmentSurgery = null;
                    this.hisTreatmentFinishSDO_process.Advise = null;
                    this.hisTreatmentFinishSDO_process.SurgeryAppointmentTime = null;
                    this.hisTreatmentFinishSDO_process.DocumentBookId = null;
                    this.hisTreatmentFinishSDO_process.TreatmentMethod = null;
                    this.hisTreatmentFinishSDO_process.EndTypeExtNote = null;
                    this.hisTreatmentFinishSDO_process.IsPregnancyTermination = null;
                    this.hisTreatmentFinishSDO_process.GestationalAge = null;
                    this.hisTreatmentFinishSDO_process.PregnancyTerminationReason = null;
                    this.hisTreatmentFinishSDO_process.PregnancyTerminationTime = null;
                }
                else
                {
                    long treatmentEndTypeExtId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTTExt.EditValue.ToString());

                    HisTreatmentFinishSDO treatmentFinishSDOExt = new HisTreatmentFinishSDO();
                    treatmentFinishSDOExt.TreatmentFinishTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtEndTime.DateTime) ?? 0;
                    treatmentFinishSDOExt.DocumentBookId = hisTreatmentFinishSDO_process != null ? hisTreatmentFinishSDO_process.DocumentBookId : null;
                    if (String.IsNullOrEmpty(hisTreatmentFinishSDO_process.SickLoginname))
                    {
                        treatmentFinishSDOExt.SickLoginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        treatmentFinishSDOExt.SickUsername = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                    }
                    else
                    {
                        treatmentFinishSDOExt.SickLoginname = hisTreatmentFinishSDO_process.SickLoginname;
                        treatmentFinishSDOExt.SickUsername = hisTreatmentFinishSDO_process.SickUsername;
                    }

                    var treatmentEndTypeExtProcessor = new TreatmentEndTypeExtProcessor(treatmentEndTypeExt, ReloadDataTreatmentEndTypeExt);
                    List<object> args = new List<object>();
                    args.Add(this.treatmentId);
                    if (isLoadTreatmentInFormTreatmentEndTypeExt)
                        args.Add(GetTreatmentEndTypeExt());
                    args.Add(treatmentFinishSDOExt);
                    args.Add(this.module);
                    args.Add(true);

                    Inventec.Common.Logging.LogSystem.Debug("cboTreatmentEndTypeExt_Closed____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isLoadTreatmentInFormTreatmentEndTypeExt), isLoadTreatmentInFormTreatmentEndTypeExt));

                    treatmentEndTypeExtProcessor.Run(args);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private TreatmentEndTypeExtData GetTreatmentEndTypeExt()
        {
            TreatmentEndTypeExtData data = null;
            try
            {
                if (this.hisTreatmentFinishSDO_process != null &&
                    (this.hisTreatmentFinishSDO_process.SickLeaveDay.HasValue
                    || !String.IsNullOrEmpty(this.hisTreatmentFinishSDO_process.MotherName)
                    || !String.IsNullOrEmpty(this.hisTreatmentFinishSDO_process.FatherName)
                    || !String.IsNullOrEmpty(this.hisTreatmentFinishSDO_process.PatientRelativeName)
                    || !String.IsNullOrEmpty(this.hisTreatmentFinishSDO_process.PatientRelativeType)
                    || !String.IsNullOrEmpty(this.hisTreatmentFinishSDO_process.PatientWorkPlace)
                    || (this.hisTreatmentFinishSDO_process.WorkPlaceId.HasValue && this.hisTreatmentFinishSDO_process.WorkPlaceId.Value > 0)
                    || this.hisTreatmentFinishSDO_process.SickLeaveFrom.HasValue
                    || this.hisTreatmentFinishSDO_process.SickLeaveTo.HasValue
                    || this.hisTreatmentFinishSDO_process.DocumentBookId.HasValue
                    || !String.IsNullOrEmpty(this.hisTreatmentFinishSDO_process.SickLoginname)
                    ))
                {
                    data = new TreatmentEndTypeExtData();
                    data.SickLeaveDay = this.hisTreatmentFinishSDO_process.SickLeaveDay;
                    data.SickHeinCardNumber = this.hisTreatmentFinishSDO_process.SickHeinCardNumber;
                    data.MotherName = this.hisTreatmentFinishSDO_process.MotherName;
                    data.FatherName = this.hisTreatmentFinishSDO_process.FatherName;
                    data.PatientRelativeName = this.hisTreatmentFinishSDO_process.PatientRelativeName;
                    data.PatientRelativeType = this.hisTreatmentFinishSDO_process.PatientRelativeType;
                    data.PatientWorkPlace = this.hisTreatmentFinishSDO_process.PatientWorkPlace;
                    data.SickLeaveFrom = this.hisTreatmentFinishSDO_process.SickLeaveFrom;
                    data.SickLeaveTo = this.hisTreatmentFinishSDO_process.SickLeaveTo;
                    data.WorkPlaceId = this.hisTreatmentFinishSDO_process.WorkPlaceId;
                    data.Loginname = this.hisTreatmentFinishSDO_process.SickLoginname;
                    data.Username = this.hisTreatmentFinishSDO_process.SickUsername;
                    data.DocumentBookId = this.hisTreatmentFinishSDO_process.DocumentBookId;
                    data.TreatmentMethod = this.hisTreatmentFinishSDO_process.TreatmentMethod;
                    data.EndTypeExtNote = this.hisTreatmentFinishSDO_process.EndTypeExtNote;
                    data.IsPregnancyTermination = this.hisTreatmentFinishSDO_process.IsPregnancyTermination;
                    data.GestationalAge = this.hisTreatmentFinishSDO_process.GestationalAge;
                    data.PregnancyTerminationReason = this.hisTreatmentFinishSDO_process.PregnancyTerminationReason;
                    data.PregnancyTerminationTime = this.hisTreatmentFinishSDO_process.PregnancyTerminationTime;
                }
                else if (this.hisTreatmentFinishSDO_process.SurgeryAppointmentTime.HasValue
                     || !String.IsNullOrEmpty(this.hisTreatmentFinishSDO_process.Advise)
                     || !String.IsNullOrEmpty(this.hisTreatmentFinishSDO_process.AppointmentSurgery))
                {
                    data = new TreatmentEndTypeExtData();
                    data.SurgeryAppointmentTime = this.hisTreatmentFinishSDO_process.SurgeryAppointmentTime;
                    data.Advise = this.hisTreatmentFinishSDO_process.Advise;
                    data.AppointmentSurgery = this.hisTreatmentFinishSDO_process.AppointmentSurgery;
                }

                Inventec.Common.Logging.LogSystem.Debug("GetTreatmentEndTypeExt____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return data;
        }

        private void ReloadDataTreatmentEndTypeExt(object data)
        {
            try
            {
                if (this.hisTreatmentFinishSDO_process == null)
                    this.hisTreatmentFinishSDO_process = new HisTreatmentFinishSDO();

                if (data != null)
                {
                    if (data is TreatmentEndTypeExtData)
                    {
                        this.currentTreatmentEndTypeExt = (TreatmentEndTypeExtData)data;
                        this.hisTreatmentFinishSDO_process.TreatmentEndTypeExtId = this.currentTreatmentEndTypeExt.TreatmentEndTypeExtId;
                        if (this.hisTreatmentFinishSDO_process.TreatmentEndTypeExtId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__HEN_MO)
                        {
                            this.hisTreatmentFinishSDO_process.AppointmentSurgery = this.currentTreatmentEndTypeExt.AppointmentSurgery;
                            this.hisTreatmentFinishSDO_process.Advise = this.currentTreatmentEndTypeExt.Advise;
                            this.hisTreatmentFinishSDO_process.SurgeryAppointmentTime = this.currentTreatmentEndTypeExt.SurgeryAppointmentTime;
                        }
                        else
                        {
                            this.hisTreatmentFinishSDO_process.SickLeaveDay = this.currentTreatmentEndTypeExt.SickLeaveDay;
                            this.hisTreatmentFinishSDO_process.SickLeaveFrom = this.currentTreatmentEndTypeExt.SickLeaveFrom;
                            this.hisTreatmentFinishSDO_process.SickLeaveTo = this.currentTreatmentEndTypeExt.SickLeaveTo;
                            this.hisTreatmentFinishSDO_process.MotherName = this.currentTreatmentEndTypeExt.MotherName;
                            this.hisTreatmentFinishSDO_process.FatherName = this.currentTreatmentEndTypeExt.FatherName;
                            this.hisTreatmentFinishSDO_process.PatientRelativeName = this.currentTreatmentEndTypeExt.PatientRelativeName;
                            this.hisTreatmentFinishSDO_process.PatientRelativeType = this.currentTreatmentEndTypeExt.PatientRelativeType;
                            this.hisTreatmentFinishSDO_process.PatientWorkPlace = this.currentTreatmentEndTypeExt.PatientWorkPlace;
                            this.hisTreatmentFinishSDO_process.SickHeinCardNumber = this.currentTreatmentEndTypeExt.SickHeinCardNumber;
                            this.hisTreatmentFinishSDO_process.SickLoginname = this.currentTreatmentEndTypeExt.Loginname;
                            this.hisTreatmentFinishSDO_process.SickUsername = this.currentTreatmentEndTypeExt.Username;
                            this.hisTreatmentFinishSDO_process.WorkPlaceId = this.currentTreatmentEndTypeExt.WorkPlaceId;
                            this.hisTreatmentFinishSDO_process.SocialInsuranceNumber = this.currentTreatmentEndTypeExt.SocialInsuranceNumber;
                            this.hisTreatmentFinishSDO_process.TreatmentMethod = this.currentTreatmentEndTypeExt.TreatmentMethod;
                            this.hisTreatmentFinishSDO_process.EndTypeExtNote = this.currentTreatmentEndTypeExt.EndTypeExtNote;
                            this.hisTreatmentFinishSDO_process.IsPregnancyTermination = this.currentTreatmentEndTypeExt.IsPregnancyTermination;
                            this.hisTreatmentFinishSDO_process.GestationalAge = this.currentTreatmentEndTypeExt.GestationalAge;
                            this.hisTreatmentFinishSDO_process.PregnancyTerminationReason = this.currentTreatmentEndTypeExt.PregnancyTerminationReason;
                            this.hisTreatmentFinishSDO_process.PregnancyTerminationTime = this.currentTreatmentEndTypeExt.PregnancyTerminationTime;
                            if (this.currentTreatmentEndTypeExt.Babes != null && this.currentTreatmentEndTypeExt.Babes.Count > 0)
                            {
                                this.hisTreatmentFinishSDO_process.Babies = new List<HisBabySDO>();
                                foreach (var item in this.currentTreatmentEndTypeExt.Babes)
                                {
                                    HisBabySDO hisBabySDO = new HisBabySDO();
                                    hisBabySDO.FatherName = item.FatherName;
                                    if (item.BornTimeDt.HasValue)
                                    {
                                        hisBabySDO.BornTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(item.BornTimeDt.Value);
                                    }
                                    hisBabySDO.GenderId = item.GenderId;
                                    hisBabySDO.Weight = item.Weight;
                                    this.hisTreatmentFinishSDO_process.Babies.Add(hisBabySDO);
                                }
                            }

                            this.hisTreatmentFinishSDO_process.DocumentBookId = this.currentTreatmentEndTypeExt.DocumentBookId;
                        }
                        this.isLoadTreatmentInFormTreatmentEndTypeExt = true;
                    }
                    else
                    {
                        this.currentTreatmentEndTypeExt = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTTExt_Properties_ButtonPressed(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

        }

        private void cboTTExt_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTTExt.EditValue = null;
                    hisTreatmentFinishSDO_process.TreatmentEndTypeExtId = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BtnEndCode_Click(object sender, EventArgs e)
        {
            try
            {
                if (!BtnEndCode.Enabled) return;

                bool hasEndCode = false;

                if (this.hisTreatmentResult != null && !String.IsNullOrWhiteSpace(this.hisTreatmentResult.END_CODE))
                {
                    hasEndCode = true;
                }
                else if (this.currentHisTreatment != null && !String.IsNullOrWhiteSpace(this.currentHisTreatment.END_CODE))
                {
                    hasEndCode = true;
                }

                if (!hasEndCode || (hasEndCode && DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.HoSoDaCoSoRaVien, ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes))
                {
                    //goi api cấp số
                    CommonParam param = new CommonParam();
                    bool success = false;
                    MOS.SDO.HisTreatmentSetEndCodeSDO sdo = new MOS.SDO.HisTreatmentSetEndCodeSDO();
                    sdo.RequestRoomId = this.currentModuleBase.RoomId;
                    sdo.TreatmentId = this.currentHisTreatment.ID;

                    hisTreatmentResult = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TREATMENT>("api/HisTreatment/SetEndCode", ApiConsumer.ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    WaitingManager.Hide();
                    if (hisTreatmentResult != null)
                    {
                        success = true;

                        txtEndOrder.Text = hisTreatmentResult.END_CODE;
                        txtSoChuyenVien.Text = hisTreatmentResult.OUT_CODE;
                        txtExtraEndCode.Text = hisTreatmentResult.EXTRA_END_CODE;
                        txtMaBHXH.Text = hisTreatmentResult.TDL_SOCIAL_INSURANCE_NUMBER;
                        if (RefeshReference != null)
                        {
                            this.RefeshReference();
                        }
                    }

                    #region Show message
                    MessageManager.Show(this, param, success);
                    #endregion

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Nút này chỉ enable trong trường hợp hồ sơ đã có thông tin "thời gian ra viện"
        //check có mã hay chưa lúc tạo
        private bool CheckTreatmentEndCode()
        {
            bool result = false;
            try
            {
                if (Base.GlobalStore.END_ORDER_STR != "1")
                {
                    if (this.hisTreatmentResult != null && this.hisTreatmentResult.OUT_TIME.HasValue)
                    {
                        result = true;
                    }
                    else if (this.currentHisTreatment != null && this.currentHisTreatment.OUT_TIME.HasValue)
                    {
                        result = true;
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

        private void btnChoiceResult_Click(object sender, EventArgs e)
        {
            LogTheadInSessionInfo(choiceResult, "btnChoiceResult_Click");
        }

        private void choiceResult()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ContentSubclinical").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ContentSubclinical");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.treatmentId);
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)SelectDataResult);
                    listArgs.Add(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId));
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SelectDataResult(object data)
        {
            try
            {
                if (data != null && data is string)
                {
                    string dienBien = data as string;
                    txtKetQuaXetNghiem.Text = dienBien;
                }
                else
                {
                    txtKetQuaXetNghiem.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadComboProgram(List<HIS_PATIENT_PROGRAM> patientPrograms, List<V_HIS_DATA_STORE> dataStores)
        {
            try
            {
                var programs = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PROGRAM>().Where(o => o.IS_ACTIVE == 1 && (o.TREATMENT_TYPE_ID == null || o.TREATMENT_TYPE_ID == currentHisTreatment.TDL_TREATMENT_TYPE_ID)).ToList();
                this.ProgramADOList = new List<ProgramADO>();
                foreach (var item in programs)
                {
                    ProgramADO program = new ProgramADO();
                    program.ID = item.ID;
                    program.PROGRAM_NAME = item.PROGRAM_NAME;
                    program.PROGRAM_CODE = item.PROGRAM_CODE;
                    program.DATA_STORE_ID = item.DATA_STORE_ID;
                    program.AUTO_CHANGE_TO_OUT_PATIENT = item.AUTO_CHANGE_TO_OUT_PATIENT;
                    var check = patientPrograms != null
                        ? patientPrograms.FirstOrDefault(o => o.PROGRAM_ID == item.ID)
                        : null;

                    if (check != null)
                        program.SelectPatient = true;
                    else
                        program.SelectPatient = false;

                    ProgramADOList.Add(program);
                }

                ProgramADOList = ProgramADOList.Where(o => !o.DATA_STORE_ID.HasValue
                    || (o.DATA_STORE_ID.HasValue && dataStores != null && dataStores.Select(p => p.ID).Contains(o.DATA_STORE_ID.Value))).ToList();

                ProgramADOList = ProgramADOList != null
                    ? ProgramADOList.OrderByDescending(o => o.SelectPatient).ThenBy(p => p.PROGRAM_NAME).ToList()
                    : ProgramADOList;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PROGRAM_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("PROGRAM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PROGRAM_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboProgram, ProgramADOList, controlEditorADO);
                SetDafaultComboProram();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDafaultComboProram()
        {
            try
            {
                chKTaoHoSoMoi.Enabled = currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU;
                if (chkCapSoLuuTruBA.CheckState == CheckState.Checked && this.PatientPrograms != null && this.PatientPrograms.Count == 1)
                {
                    var program = this.ProgramADOList.FirstOrDefault(o => o.ID == this.PatientPrograms[0].PROGRAM_ID);

                    if (program != null)
                    {
                        cboProgram.EditValue = program.ID;
                    }
                    else
                    {
                        cboProgram.EditValue = null;
                    }
                }
                else if (this.currentHisTreatment != null && ProgramADOList.Any(p => p.ID == this.currentHisTreatment.PROGRAM_ID))
                {
                    cboProgram.EditValue = this.currentHisTreatment.PROGRAM_ID;
                }
                else
                {
                    cboProgram.EditValue = null;
                }

                if ((chkCapSoLuuTruBA.Visible && chkCapSoLuuTruBA.CheckState == System.Windows.Forms.CheckState.Checked) || (ConfigKey.IsMustSetProgramWhenFinishingInPatient && this.currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU))
                {
                    lciPatientProgram.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
                else
                {
                    lciPatientProgram.AppearanceItemCaption.ForeColor = Color.Black;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkCapSoLuuTruBA_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                LoadComboProgram(this.PatientPrograms, this.DataStores);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProgram_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboProgram.EditValue = null;
                    e.Button.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProgram_EditValueChanged(object sender, EventArgs e)
        {
            if (cboProgram.EditValue != null)
            {
                dxValidationProvider.RemoveControlError(cboProgram);
                cboProgram.Properties.Buttons[1].Visible = true;
                if (currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                    chKTaoHoSoMoi.Enabled = true;
            }
            else
            {
                if (currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                    chKTaoHoSoMoi.Enabled = false;
            }
        }

        private void btnGetPT_Click(object sender, EventArgs e)
        {
            LogTheadInSessionInfo(getPT, "btnGetPT_Click");
        }
        private void getPT()
        {
            try
            {
                string serviceNameList = "";
                if (this.SereServCheck != null && this.SereServCheck.Count > 0)
                {
                    var sereServList = this.SereServCheck.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT).ToList();
                    if (sereServList != null && sereServList.Count > 0)
                    {
                        serviceNameList = String.Join(";", sereServList.Select(o => o.TDL_SERVICE_NAME));
                    }
                }

                txtMethod.Text = serviceNameList;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void txtEyeTensionRight_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtEyeTensionLeft.Enabled && txtEyeTensionLeft.Visible)
                    {
                        txtEyeTensionLeft.Focus();
                        txtEyeTensionLeft.SelectAll();
                    }
                    else
                    {
                        SendKeys.Send("{TAB}");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtEyeTensionLeft_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtEyesightRight.Enabled && txtEyesightRight.Visible)
                    {
                        txtEyesightRight.Focus();
                        txtEyesightRight.SelectAll();
                    }
                    else { SendKeys.Send("{TAB}"); }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtEyesightRight_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(txtEyesightRight.Text) || checkDigit(txtEyesightRight.Text))
                {
                    lblEyesightRight.Text = "/10";
                }
                else
                {
                    lblEyesightRight.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtEyesightRight_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtEyesightLeft.Enabled && txtEyesightLeft.Visible)
                    {
                        txtEyesightLeft.Focus();
                        txtEyesightLeft.SelectAll();
                    }
                    else
                    {
                        SendKeys.Send("{TAB}");
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtEyesightLeft_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(txtEyesightLeft.Text) || checkDigit(txtEyesightLeft.Text))
                {
                    lblEyesightLeft.Text = "/10";
                }
                else
                {
                    lblEyesightLeft.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtEyesightLeft_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtEyesightGlassRight.Enabled && txtEyesightGlassRight.Visible)
                    {
                        txtEyesightGlassRight.Focus();
                        txtEyesightGlassRight.SelectAll();
                    }
                    else { SendKeys.Send("{TAB}"); }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtEyesightGlassRight_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(txtEyesightGlassRight.Text) || checkDigit(txtEyesightGlassRight.Text))
                {
                    lblEyesightGlassRight.Text = "/10";
                }
                else
                {
                    lblEyesightGlassRight.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtEyesightGlassRight_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtEyesightGlassLeft.Enabled && txtEyesightGlassLeft.Visible)
                    {
                        txtEyesightGlassLeft.Focus();
                        txtEyesightGlassLeft.SelectAll();
                    }
                    else
                    {
                        SendKeys.Send("{TAB}");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtEyesightGlassLeft_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(txtEyesightGlassLeft.Text) || checkDigit(txtEyesightGlassLeft.Text))
                {
                    lblEyesightGlassLeft.Text = "/10";
                }
                else
                {
                    lblEyesightGlassLeft.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtEyesightGlassLeft_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtAdvised.Enabled && txtAdvised.Visible)
                    {
                        txtAdvised.Focus();
                        txtAdvised.SelectAll();
                    }
                    else { SendKeys.Send("{TAB}"); }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAppointInfo_Click(object sender, EventArgs e)
        {
            try
            {
                var dataRoom = this.hisRooms.FirstOrDefault(o => o.ID == this.module.RoomId);
                long dtTreatmentEnd = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtEndTime.DateTime) ?? 0;
                FormAppointment = new CloseTreatment.FormAppointment(this.module, dtTreatmentEnd, dataRoom.IS_BLOCK_NUM_ORDER == 1 ? true : false);
                FormAppointment.MyGetData = new CloseTreatment.FormAppointment.GetString(TranPatiDataTreatmentFinish);
                FormAppointment.Form = this;
                FormAppointment.ShowDialog();
                cboTTExt.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrintConfig_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessShowpopupControlContainerPrintConfig();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewContainerPrintConfig_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("gridViewContainerPrintConfig_KeyDown.1");
                ColumnView View = (ColumnView)gridControlContainerPrintConfig.FocusedView;
                if (e.KeyCode == Keys.Space)
                {
                    if (this.gridViewContainerPrintConfig.IsEditing)
                        this.gridViewContainerPrintConfig.CloseEditor();

                    if (this.gridViewContainerPrintConfig.FocusedRowModified)
                        this.gridViewContainerPrintConfig.UpdateCurrentRow();

                    var rawPrintConfig = (PrintConfigADO)this.gridViewContainerPrintConfig.GetFocusedRow();

                    Inventec.Common.Logging.LogSystem.Debug("gridViewContainerPrintConfig_KeyDown. FocusedRowHandle=" + View.FocusedRowHandle + "|" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rawPrintConfig), rawPrintConfig));
                    if (rawPrintConfig != null && rawPrintConfig.IsAutoPrint)
                    {
                        rawPrintConfig.IsAutoPrint = false;
                    }
                    else if (rawPrintConfig != null)
                    {
                        rawPrintConfig.IsAutoPrint = true;
                    }
                    gridControlContainerPrintConfig.RefreshDataSource();
                    ProcessStorePrintConfigIntoLocal();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    var rawPrintConfig = (PrintConfigADO)this.gridViewContainerPrintConfig.GetFocusedRow();
                    if ((printConfigADOs != null && !printConfigADOs.Any(o => o.IsAutoPrint)) && rawPrintConfig != null)
                    {
                        rawPrintConfig.IsAutoPrint = true;
                        gridControlContainerPrintConfig.RefreshDataSource();
                        ProcessStorePrintConfigIntoLocal();
                    }
                    popupControlContainerPrintConfig.HidePopup();
                }
                Inventec.Common.Logging.LogSystem.Debug("gridViewContainerPrintConfig_KeyDown.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewContainerPrintConfig_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.Column.FieldName == "IsAutoPrint" && hi.InRowCell)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("hi.InRowCell");
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            Inventec.Common.Logging.LogSystem.Debug("gridViewContainerPrintConfig_MouseDown.1");
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
                            gridControlContainerPrintConfig.RefreshDataSource();
                            ProcessStorePrintConfigIntoLocal();
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                        Inventec.Common.Logging.LogSystem.Debug("gridViewContainerPrintConfig_MouseDown.2");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTTExt_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtEndOrder.Visible && txtEndOrder.Enabled)
                    {
                        txtEndOrder.Focus();
                        txtEndOrder.SelectAll();
                    }
                    else if (chkSinhLaiSoRaVien.Visible && chkSinhLaiSoRaVien.Enabled)
                    {
                        chkSinhLaiSoRaVien.Focus();
                    }
                    else if (chkSinhLaiSoChuyenVien.Visible && chkSinhLaiSoChuyenVien.Enabled)
                    {
                        chkSinhLaiSoChuyenVien.Focus();
                    }
                    else if (chkCapSoLuuTruBA.Visible && chkCapSoLuuTruBA.Enabled)
                    {
                        chkCapSoLuuTruBA.Focus();
                    }
                    else if (cboProgram.Enabled && cboProgram.Visible)
                    {
                        cboProgram.Focus();
                    }
                    else
                    {
                        SendKeys.Send("{TAB}");
                    }
                }
                else if (e.KeyCode == Keys.Down)
                    cboTTExt.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkSinhLaiSoRaVien_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (chkSinhLaiSoChuyenVien.Enabled)
                    {
                        chkSinhLaiSoChuyenVien.Focus();
                    }
                    else if (chkCapSoLuuTruBA.Enabled)
                    {
                        chkCapSoLuuTruBA.Focus();
                    }
                    else
                    {
                        cboProgram.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkSinhLaiSoChuyenVien_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkCapSoLuuTruBA.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkCapSoLuuTruBA_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboProgram.Enabled && cboProgram.Visible)
                    {
                        cboProgram.Focus();
                    }
                    else
                    {
                        SendKeys.Send("{TAB}");
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProgram_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (chkChronic.Visible && chkChronic.Enabled)
                    {
                        chkChronic.Focus();
                    }
                    else if (ChkExpXml4210.Visible && ChkExpXml4210.Enabled)
                    {
                        ChkExpXml4210.Focus();
                    }
                    else if (chkOutHopitalCondition.Visible && chkOutHopitalCondition.Enabled)
                    {
                        chkOutHopitalCondition.Focus();
                    }
                    else
                    {
                        SendKeys.Send("{TAB}");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChkExpXml4210_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (chkOutHopitalCondition.Enabled && chkOutHopitalCondition.Visible)
                    {
                        chkOutHopitalCondition.Focus();
                    }
                    else
                    {
                        SendKeys.Send("{TAB}");
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkOutHopitalCondition_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtMethod.Enabled && txtMethod.Visible)
                    {
                        txtMethod.Focus();
                        txtMethod.SelectAll();
                    }
                    else { SendKeys.Send("{TAB}"); }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Mở vỏ bệnh án
        /// --> Lấy tất cả dữ liệu HIS_EMR_COVER_CONFIG thỏa mãn ROOM_ID theo phòng đang làm việc, TREATMENT_TYPE_ID theo diện điều trị của hồ sơ.
        ///---> Trường hợp chỉ có 1 bản ghi thì truyền vào thư viện generate menu VBA loại vỏ bệnh án (EmrCoverTypeId) như hiện tại.
        ///---> Trường hợp có nhiều hơn 1 bản ghi thì truyền vào thư viện generate menu VBA danh sách loại vỏ bệnh án lấy được đấy.
        ///---> Trường hợp không có dữ liệu thì tiếp tục lấy HIS_EMR_COVER_CONFIG thỏa mãn DEPARTMENT_ID theo khoa đang làm việc, TREATMENT_TYPE_ID theo diện điều trị của hồ sơ.
        ///---> Trường hợp chỉ có 1 bản ghi thì truyền vào thư viện generate menu VBA loại vỏ bệnh án (EmrCoverTypeId) như hiện tại.
        ///---> Trường hợp có nhiều hơn 1 bản ghi thì truyền vào thư viện generate menu VBA danh sách loại vỏ bệnh án lấy được đấy.
        /// </summary>
        private void VoBenhAn(HIS_TREATMENT treatment)
        {
            try
            {
                if (barManager1 == null)
                {
                    barManager1 = new BarManager();
                    barManager1.Form = this;
                }
                this._BarManager = barManager1;
                if (this._BarManager == null)
                {
                    return;
                }
                if (this._Menu == null)
                    this._Menu = new PopupMenu(this._BarManager);
                this._Menu.ItemLinks.Clear();
                if (this.emrMenuPopupProcessor == null) this.emrMenuPopupProcessor = new Library.FormMedicalRecord.MediRecordMenuPopupProcessor();

                HIS.Desktop.Plugins.Library.FormMedicalRecord.Base.EmrInputADO emrInputAdo = new Library.FormMedicalRecord.Base.EmrInputADO();
                emrInputAdo.TreatmentId = treatment.ID;
                emrInputAdo.PatientId = treatment.PATIENT_ID;
                if (LstEmrCoverConfig != null && LstEmrCoverConfig.Count > 0)
                {
                    if (LstEmrCoverConfig.Count == 1)
                    {
                        emrInputAdo.EmrCoverTypeId = LstEmrCoverConfig.FirstOrDefault().EMR_COVER_TYPE_ID;
                    }
                    else
                    {
                        emrInputAdo.lstEmrCoverTypeId = new List<long>();
                        emrInputAdo.lstEmrCoverTypeId = LstEmrCoverConfig.Select(o => o.EMR_COVER_TYPE_ID).ToList();
                    }
                }
                else
                {
                    if (LstEmrCoverConfigDepartment != null && LstEmrCoverConfigDepartment.Count > 0)
                    {
                        if (LstEmrCoverConfigDepartment.Count == 1)
                        {
                            emrInputAdo.EmrCoverTypeId = LstEmrCoverConfigDepartment.FirstOrDefault().EMR_COVER_TYPE_ID;
                        }
                        else
                        {
                            emrInputAdo.lstEmrCoverTypeId = new List<long>();
                            emrInputAdo.lstEmrCoverTypeId = LstEmrCoverConfigDepartment.Select(o => o.EMR_COVER_TYPE_ID).ToList();
                        }
                    }
                }

                Inventec.Common.Logging.LogSystem.Info("emrInputAdo: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => emrInputAdo), emrInputAdo));

                this.emrMenuPopupProcessor.InitMenuButton(this._Menu, this._BarManager, emrInputAdo);
                this._Menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void panelControlCauseIcd_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ChkExpXml4210_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ChkExpXml4210.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (ChkExpXml4210.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ChkExpXml4210.Name;
                    csAddOrUpdate.VALUE = (ChkExpXml4210.Checked ? "1" : "");
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMaBHXH_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDoctorLogginName.Focus();
                    txtDoctorLogginName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnGetQT_Click(object sender, EventArgs e)
        {
            LogTheadInSessionInfo(getQT, "btnGetQT_Click");
        }
        private void getQT()
        {
            try
            {
                frmDienBienCLS frm = new frmDienBienCLS(module, (HIS.Desktop.Common.DelegateSelectData)dataResult, this.currentHisTreatment);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void dataResult(object data)
        {
            try
            {
                if (data != null && data is string)
                {
                    string dt = data as string;

                    txtDauHieuLamSang.Text = dt;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        // Tạo bệnh án mới đối với đối tượng ngoại trú và nội trú

        private void btnCD_Click(object sender, EventArgs e)
        {

            try
            {

                this.Invoke(new Action(() =>
                {
                    t_ = new Thread(() =>
                    {
                        this.Invoke(new Action(() =>
                        {
                            this.FormTTCDHienThiTrenGiayRaVien_ = new CloseTreatment.FormTTCDHienThiTrenGiayRaVien(currentHisTreatment, (HIS.Desktop.Common.DelegateSelectData)dataResult_);
                            this.FormTTCDHienThiTrenGiayRaVien_.ShowDialog();
                        }));
                    });
                }));
                t_.IsBackground = true;
                t_.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void dataResult_(object data)
        {
            try
            {
                if (data != null && data is HIS_TREATMENT)
                {
                    prosCD = new HIS_TREATMENT();
                    prosCD = data as HIS_TREATMENT;
                    currentHisTreatment.SHOW_ICD_CODE = prosCD.SHOW_ICD_CODE;
                    currentHisTreatment.SHOW_ICD_NAME = prosCD.SHOW_ICD_NAME;
                    currentHisTreatment.SHOW_ICD_SUB_CODE = prosCD.SHOW_ICD_SUB_CODE;
                    currentHisTreatment.SHOW_ICD_TEXT = prosCD.SHOW_ICD_TEXT;
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => prosCD), prosCD));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtOutPatientDateFrom_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    dxValidationProvider_ForOutPatientDateFromTo.Validate();
                    LoadComboTreatmentEndTypeExt();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtOutPatientDateTo_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    dxValidationProvider_ForOutPatientDateFromTo.Validate();
                    LoadComboTreatmentEndTypeExt();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtOutPatientDateFrom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                dxValidationProvider_ForOutPatientDateFromTo.Validate();
                LoadComboTreatmentEndTypeExt();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtOutPatientDateTo_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                dxValidationProvider_ForOutPatientDateFromTo.Validate();
                LoadComboTreatmentEndTypeExt();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartmentOut_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDepartmentOut.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnSaveTextLib_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == CallModule.TextLibrary).FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink :" + CallModule.TextLibrary);
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module '" + CallModule.TextLibrary + "' is not plugins");

                HIS.Desktop.ADO.TextLibraryInfoADO textLibraryInfoADO = new Desktop.ADO.TextLibraryInfoADO();
                textLibraryInfoADO.Hashtag = "loidanbacsi";
                textLibraryInfoADO.Content = txtAdvised.Text;
                textLibraryInfoADO.IsFillHashtag = true;
                textLibraryInfoADO.IsFillContent = true;

                List<object> listArgs = new List<object>();
                listArgs.Add(this.module);
                listArgs.Add(textLibraryInfoADO);

                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(moduleData, listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("ModuleData is null");

                WaitingManager.Hide();
                ((Form)extenceInstance).ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnListTextLib_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == CallModule.TextLibrary).FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink :" + CallModule.TextLibrary);
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module '" + CallModule.TextLibrary + "' is not plugins");

                HIS.Desktop.ADO.TextLibraryInfoADO textLibraryInfoADO = new Desktop.ADO.TextLibraryInfoADO();
                textLibraryInfoADO.IsFindTemplate = true;
                textLibraryInfoADO.IsNotSaveTemplate = true;
                textLibraryInfoADO.Hashtag = "loidanbacsi";
                textLibraryInfoADO.Content = txtAdvised.Text;

                List<object> listArgs = new List<object>();
                listArgs.Add(this.module);
                listArgs.Add(textLibraryInfoADO);
                listArgs.Add((HIS.Desktop.Common.DelegateDataTextLib)ProcessDataTextLib_txtAdvised);

                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(moduleData, listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("ModuleData is null");

                WaitingManager.Hide();
                ((Form)extenceInstance).ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataTextLib_txtAdvised(MOS.EFMODEL.DataModels.HIS_TEXT_LIB textLib)
        {
            try
            {
                txtAdvised.Text = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAdvised_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F5)
                {
                    WaitingManager.Show();

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == CallModule.TextLibrary).FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink :" + CallModule.TextLibrary);
                    if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module '" + CallModule.TextLibrary + "' is not plugins");

                    HIS.Desktop.ADO.TextLibraryInfoADO textLibraryInfoADO = new Desktop.ADO.TextLibraryInfoADO();
                    textLibraryInfoADO.IsFindTemplate = true;
                    textLibraryInfoADO.Hashtag = "loidanbacsi";
                    textLibraryInfoADO.Content = txtAdvised.Text;

                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.module);
                    listArgs.Add(textLibraryInfoADO);
                    listArgs.Add((HIS.Desktop.Common.DelegateDataTextLib)ProcessDataTextLib_txtAdvised);

                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("ModuleData is null");

                    WaitingManager.Hide();
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnKiemTraHoSo_Click(object sender, EventArgs e)
        {
            LogTheadInSessionInfo(kiemTraHoSo, "btnKiemTraHoSo_Click");
        }

        private async void kiemTraHoSo()
        {
            try
            {
                this.warningADOs = new List<WarningADO>();
                bool IsContinue = true;
                IsContinue = IsContinue && CheckSA(false);
                IsContinue = IsContinue && CheckBedLog(false);
                bool rs = await ProcessValidateServerAsync(this.warningADOs);
                if (!rs)
                {
                    return;
                }

                if (!this.CheckAssignServiceBed_ForSave(ValidationDataType.GetListMessage, ref this.warningADOs))
                {
                    return;
                }

                if (!this.Check_INTRUCTION_TIME_and_DEPARTMENT_IN_TIME_ForSave(ValidationDataType.GetListMessage, ref this.warningADOs))
                {
                    return;
                }

                if (!this.CheckWarnNotRequiredCompleteHasNoSample(ValidationDataType.GetListMessage, ref this.warningADOs))
                {
                    return;
                }

                if (!this.CheckSameHein_ForSave(ValidationDataType.GetListMessage, ref this.warningADOs))
                {
                    return;
                }

                if (!this.CheckRation_ForSave(ValidationDataType.GetListMessage, ref this.warningADOs))
                {
                    return;
                }

                if (!this.Check_UNSIGN_DOC_FINISH_OPTION_ForSave(ValidationDataType.GetListMessage, ref this.warningADOs))
                {
                    return;
                }

                if (!this.CheckDHST_ForSave(ValidationDataType.GetListMessage, ref this.warningADOs))
                {
                    return;
                }

                if (!this.CheckUnassignTrackingServiceReq_ForSave(ValidationDataType.GetListMessage, ref this.warningADOs))
                {
                    return;
                }

                GetValueUC();
                if (Inventec.Common.String.CountVi.Count(codeCheckCD) > 100)
                {
                    XtraMessageBox.Show("Mã chẩn đoán phụ nhập quá 100 ký tự.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (Inventec.Common.String.CountVi.Count(nameCheckCD) > 1500)
                {
                    XtraMessageBox.Show("Tên chẩn đoán phụ nhập quá 1500 ký tự.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (Inventec.Common.String.CountVi.Count(codeCheckCDYHCT) > 255)
                {
                    XtraMessageBox.Show("Mã chẩn đoán YHCT phụ nhập quá 255 ký tự.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var checkICDSubCode = codeCheckSubICD.Split(';').ToList();
                if (checkICDSubCode != null && checkICDSubCode.Count > 12)
                {
                    if (Config.ConfigKey.IsCheckSubIcdExceedLimit == "1")
                    {
                        XtraMessageBox.Show("Chẩn đoán phụ nhập quá 12 mã bệnh. Vui lòng kiểm tra lại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else if (Config.ConfigKey.IsCheckSubIcdExceedLimit == "2")
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show("Chẩn đoán phụ nhập quá 12 mã bệnh. Bạn có muốn tiếp tục không?",
                           "Thông báo",
                          MessageBoxButtons.YesNo) == DialogResult.No)
                            return;
                    }
                }

                frmWarning form = new frmWarning(DelegateCheckSkipMethod, this.warningADOs, this._isSkipWarningForSave);
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void DelegateCheckSkipMethod(bool isCheck)
        {
            try
            {
                this._isSkipWarningForSave = isCheck;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task<bool> ProcessValidateServerAsync(List<WarningADO> listWarningADO)
        {
            bool success = true;
            try
            {
                this.positionHandle = -1;

                bool valid = (bool)icdProcessor.ValidationIcd(ucIcd);
                valid = (bool)subIcdProcessor.GetValidate(ucSecondaryIcd) && valid;
                valid = IsValiICDCause() && valid;
                valid = dxValidationProvider.Validate() && valid;
                if (!valid) return false;

                Inventec.Common.Logging.LogSystem.Info("Save treatmentFinish 5");
                if (hisTreatmentFinishSDO_process == null) hisTreatmentFinishSDO_process = new MOS.SDO.HisTreatmentFinishSDO();
                var treatmentEndType = this.hisTreatmentEndTypes.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentEndType.EditValue ?? 0).ToString()));

                var treatmentEndTypeExt = this.hisTreatmentEndTypeExts.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboTTExt.EditValue ?? 0).ToString()));
                if (treatmentEndType != null)
                {
                    hisTreatmentFinishSDO_process.TreatmentEndTypeId = treatmentEndType.ID;
                }

                if (Config.ConfigKey.RequiredTreatmentMethodOption == "1" && this.currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU && (treatmentEndType.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN
                    || treatmentEndType.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN || treatmentEndType.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN
                    || treatmentEndType.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN || treatmentEndType.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CTCV))
                {
                    if (String.IsNullOrEmpty(txtMethod.Text))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ChuaNhapPhuongPhapDieuTri, ResourceMessage.ThongBao);
                        return false;
                    }
                    else
                    {
                        hisTreatmentFinishSDO_process.TreatmentMethod = txtMethod.Text;
                    }
                }
                else if (Config.ConfigKey.RequiredTreatmentMethodOption == "2" && ((treatmentEndTypeExt != null && treatmentEndTypeExt.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM) || ((this.currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU || this.currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU || this.currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY) && (treatmentEndType.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN
                    || treatmentEndType.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN || treatmentEndType.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN
                    || treatmentEndType.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN || treatmentEndType.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CTCV))))
                {
                    if (String.IsNullOrEmpty(txtMethod.Text))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ChuaNhapPhuongPhapDieuTri, ResourceMessage.ThongBao);
                        return false;
                    }
                    else
                    {
                        hisTreatmentFinishSDO_process.TreatmentMethod = txtMethod.Text;
                    }
                }
                if (treatmentEndTypeExt != null)
                {
                    hisTreatmentFinishSDO_process.TreatmentEndTypeExtId = treatmentEndTypeExt.ID;
                }

                hisTreatmentFinishSDO = new MOS.SDO.HisTreatmentFinishSDO();
                bool rs = await ProcessDataBeforeSaveAsync(this, true);
                if (rs)
                {
                    return false;
                }                     

                if (!string.IsNullOrEmpty(txtMaBHXH.Text))
                {
                    hisTreatmentFinishSDO_process.SocialInsuranceNumber = txtMaBHXH.Text;
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisTreatmentFinishSDO_process), hisTreatmentFinishSDO_process));

                CommonParam param = new CommonParam();
                WaitingManager.Show();
                //Validate các nghiệp vụ phía server
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisTreatmentFinishSDO), hisTreatmentFinishSDO));
                bool apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisTreatment/TreatmentFinishCheck", ApiConsumer.ApiConsumers.MosConsumer, hisTreatmentFinishSDO, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                WaitingManager.Hide();
                if (apiResult == false)
                {
                    WarningADO warning = new WarningADO();
                    warning.IsSkippable = false;
                    warning.Description = param.GetMessage();
                    listWarningADO.Add(warning);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                success = false;
            }
            return success;
        }

        private async Task TaskLoadServiceReqSA()
        {
            try
            {
                Action myaction = () =>
                {
                    HisServiceReqFilter srFilter = new HisServiceReqFilter();
                    srFilter.TREATMENT_ID = currentHisTreatment.ID;
                    srFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__AN;
                    serviceReqsSA = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, srFilter, null);
                    if (serviceReqsSA != null && serviceReqsSA.Count > 0)
                    {
                        serviceReqsSA = serviceReqsSA.Where(o => o.IS_FOR_HOMIE != 1).ToList();
                    }
                };
                Task task = new Task(myaction);
                task.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private async Task TaskLoadBedLog()
        {
            try
            {
                if (currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                    return;
                Action myaction = () =>
                {
                    HisBedLogViewFilter filter = new HisBedLogViewFilter();
                    filter.TREATMENT_ID = currentHisTreatment.ID;
                    BedLogs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumers.MosConsumer, filter, null);
                };
                Task task = new Task(myaction);
                task.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GroupCount(ref List<string> lstName)
        {
            try
            {
                List<long?> lstRationTimeIds = new List<long?>();
                decimal amountTreat = Inventec.Common.TypeConvert.Parse.ToDecimal(txtNumberOfDays.Text);
                foreach (var SA in serviceReqsSA.Where(o => o.RATION_TIME_ID != null).GroupBy(info => info.RATION_TIME_ID)
                        .Select(group => new
                        {
                            Key = group.Key,
                            Count = group.Count()
                        })
                        .OrderBy(x => x.Count))
                {
                    if (SA.Count > amountTreat)
                    {
                        lstRationTimeIds.Add(SA.Key);
                    }
                }


                lstName = BackendDataWorker.Get<HIS_RATION_TIME>().Where(o => lstRationTimeIds.Exists(p => p == o.ID)).Select(o => o.RATION_TIME_NAME).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckBedLog(bool IsButtonSave)
        {
            bool IsContinue = true;
            try
            {
                var OUT_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtEndTime.DateTime) ?? 0;
                if (BedLogs == null || BedLogs.Count == 0 || !BedLogs.Exists(o => o.FINISH_TIME == null || o.FINISH_TIME > OUT_TIME || o.START_TIME > OUT_TIME))
                    return IsContinue;
                if (IsButtonSave)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.TonTaiLichSuGiuongChuaCoThongTinThoiGianKetThucHoacThoiGianBatDauThoiGianKetThucLonHonThoiGianRaVienBanCoMuonTiepTuc, ResourceMessage.ThongBao, MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                    {
                        IsContinue = false;
                    }
                }
                else
                {
                    if (warningADOs != null)
                    {
                        WarningADO warning = new WarningADO();
                        warning.IsSkippable = true;
                        warning.Description = ResourceMessage.TonTaiLichSuGiuongChuaCoThongTinThoiGianKetThucHoacThoiGianBatDauThoiGianKetThucLonHonThoiGianRaVien;
                        warningADOs.Add(warning);
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return IsContinue;
        }

        private bool CheckSA(bool IsButtonSave)
        {
            bool IsContinue = true;
            try
            {
                List<string> lstNameSA = new List<string>();
                GroupCount(ref lstNameSA);
                if (lstNameSA != null && lstNameSA.Count > 0)
                {
                    if (Config.ConfigKey.CheckingRationOption == "1")
                    {
                        if (IsButtonSave)
                        {
                            if (!this._isSkipWarningForSave && DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.BuaAnCoSoLuongLonHonSoNgayDieuTriBanCoMuonTiepTucKhong, String.Join(", ", lstNameSA)), ResourceMessage.ThongBao, MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                            {
                                IsContinue = false;
                            }
                        }
                        else
                        {
                            if (warningADOs != null)
                            {
                                WarningADO warning = new WarningADO();
                                warning.IsSkippable = true;
                                warning.Description = String.Format(ResourceMessage.BuaAnCoSoLuongLonHonSoNgayDieuTri, String.Join(", ", lstNameSA));
                                warningADOs.Add(warning);
                            }
                        }
                    }
                    else if (!IsButtonSave && Config.ConfigKey.CheckingRationOption == "2")
                    {
                        if (warningADOs != null)
                        {
                            WarningADO warning = new WarningADO();
                            warning.IsSkippable = false;
                            warning.Description = String.Format(ResourceMessage.BuaAnCoSoLuongLonHonSoNgayDieuTri, String.Join(", ", lstNameSA));
                            warningADOs.Add(warning);
                        }
                    }
                    else if (Config.ConfigKey.CheckingRationOption == "2")
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.BuaAnCoSoLuongLonHonSoNgayDieuTri, String.Join(", ", lstNameSA)), ResourceMessage.ThongBao, MessageBoxButtons.OK);
                        IsContinue = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return IsContinue;
        }

        private void chKTaoHoSoMoi_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                ValidationSingleControl(dtNewTreatmentTime, chKTaoHoSoMoi.Checked);
                dtNewTreatmentTime.Enabled = chKTaoHoSoMoi.Checked;
                lciNewTreatmentTime.AppearanceItemCaption.ForeColor = chKTaoHoSoMoi.Checked ? Color.Maroon : Color.Black;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidationSingleControl(BaseEdit control, bool isValid)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();

                if (isValid)
                {
                    validRule.editor = control;
                    validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    validRule.ErrorType = ErrorType.Warning;
                }
                dxValidationProvider.SetValidationRule(control, validRule);
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
                CommonParam param = new CommonParam();
                HisServiceReqFilter filter = new HisServiceReqFilter();
                filter.TREATMENT_ID = treatmentId;
                var result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
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
                foreach (var item in lstHisIcd)
                {
                    if (!string.IsNullOrEmpty(item.ICD_CODE) && !string.IsNullOrEmpty(item.ICD_NAME))
                    {
                        var icd = listIcd.FirstOrDefault(o => o.ICD_CODE == item.ICD_CODE);
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
                List<IcdADO> lstIcdNew = new List<IcdADO>();
                foreach (var item in lstHisIcd)
                {
                    if (!string.IsNullOrEmpty(item.ICD_CODE) || !string.IsNullOrEmpty(item.ICD_NAME))

                        lstIcdNew.Add(new IcdADO() { ICD_CODE = item.ICD_CODE, ICD_NAME = item.ICD_NAME, IS_ACTIVE = item.IS_ACTIVE });
                }
                lstIcdNew = lstIcdNew.OrderBy(o => o.IS_ACTIVE).ThenBy(o => o.ICD_CODE).Distinct(new Compare()).ToList();
                SecondaryIcdDataADO subIcd = new SecondaryIcdDataADO();
                string icdSubCodeQ = null;
                string icdSubNameQ = null;
                string icdCode = null;
                string icdName = null;
                if (ucIcd != null)
                {
                    var icdValue = icdProcessor.GetValue(ucIcd);
                    if (icdValue != null && icdValue is IcdInputADO)
                    {
                        icdCode = ((IcdInputADO)icdValue).ICD_CODE;
                        icdName = ((IcdInputADO)icdValue).ICD_NAME;
                    }
                    List<IcdADO> lstIcd = lstIcdNew.Where(o => o.ICD_CODE != icdCode && o.ICD_NAME != icdName).ToList();
                    icdSubCodeQ = String.Join(";", lstIcd.Select(o => o.ICD_CODE).Distinct()) + ";";
                    icdSubNameQ = String.Join(";", lstIcd.Select(o => o.ICD_NAME).Distinct()) + ";";
                }
                var lstIcdActive = lstIcdNew.Where(o => o.ICD_CODE == icdCode).ToList();
                foreach (var item in lstIcdActive)
                {
                    if (item.ICD_NAME == icdName)
                        continue;
                    icdSubNameQ += item.ICD_NAME + ";";
                }
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
                subIcd.ICD_SUB_CODE = icdSubCodeQ;
                subIcd.ICD_TEXT = icdSubNameQ;
                if (ucSecondaryIcd != null)
                {
                    subIcdProcessor.Reload(ucSecondaryIcd, subIcd);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEndDeptSubsHead_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboEndDeptSubsHead.EditValue != null)
                    {
                        var data = this.listUser.FirstOrDefault(o => o.LOGINNAME.ToLower() == cboEndDeptSubsHead.EditValue.ToString().ToLower());
                        if (data != null)
                        {
                            txtEndDeptSubsHead.Text = data.LOGINNAME;
                            cboEndDeptSubsHead.Properties.Buttons[1].Visible = true;
                        }
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    cboEndDeptSubsHead.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboHospSubsDirector_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                dxErrorProvider1.SetError(txtHospSubsDirector, "", ErrorType.None);
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboHospSubsDirector.EditValue != null)
                    {
                        var data = this.listUser.FirstOrDefault(o => o.LOGINNAME.ToLower() == cboHospSubsDirector.EditValue.ToString().ToLower());
                        if (data != null)
                        {
                            txtHospSubsDirector.Text = data.LOGINNAME;
                            cboHospSubsDirector.Properties.Buttons[1].Visible = true;
                        }
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    cboHospSubsDirector.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboEndDeptSubsHead_Closed(object sender, ClosedEventArgs e)
        {
            try
            {

                dxErrorProvider1.SetError(txtEndDeptSubsHead, "", ErrorType.None);

                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboEndDeptSubsHead.EditValue != null)
                    {
                        var data = this.listUser.FirstOrDefault(o => o.LOGINNAME.ToLower() == cboEndDeptSubsHead.EditValue.ToString().ToLower());
                        if (data != null)
                        {
                            txtEndDeptSubsHead.Text = data.LOGINNAME;
                            cboEndDeptSubsHead.Properties.Buttons[1].Visible = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboHospSubsDirector_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboHospSubsDirector.EditValue != null)
                    {
                        var data = this.listUser.FirstOrDefault(o => o.LOGINNAME.ToLower() == cboHospSubsDirector.EditValue.ToString().ToLower());
                        if (data != null)
                        {
                            txtHospSubsDirector.Text = data.LOGINNAME;
                            cboHospSubsDirector.Properties.Buttons[1].Visible = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtEndDeptSubsHead_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtEndDeptSubsHead.Text.Trim()))
                    {
                        string code = txtEndDeptSubsHead.Text.Trim().ToLower();
                        var listData = this.listUser.Where(o => o.LOGINNAME.ToLower().Contains(code)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.LOGINNAME.ToLower() == code).ToList() : listData) : null;
                        if (result != null && result.Count > 0)
                        {
                            showCbo = false;
                            txtEndDeptSubsHead.Text = result.First().LOGINNAME;
                            cboEndDeptSubsHead.EditValue = result.First().LOGINNAME;
                            cboEndDeptSubsHead.Properties.Buttons[1].Visible = true;
                        }
                    }
                    if (showCbo)
                    {
                        cboEndDeptSubsHead.Focus();
                        cboEndDeptSubsHead.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtHospSubsDirector_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtHospSubsDirector.Text.Trim()))
                    {
                        string code = txtHospSubsDirector.Text.Trim().ToLower();
                        var listData = this.listUser.Where(o => o.LOGINNAME.ToLower().Contains(code)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.LOGINNAME.ToLower() == code).ToList() : listData) : null;
                        if (result != null && result.Count > 0)
                        {
                            showCbo = false;
                            txtHospSubsDirector.Text = result.First().LOGINNAME;
                            cboHospSubsDirector.EditValue = result.First().LOGINNAME;
                            cboHospSubsDirector.Properties.Buttons[1].Visible = true;
                        }
                    }
                    if (showCbo)
                    {
                        cboHospSubsDirector.Focus();
                        cboHospSubsDirector.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboEndDeptSubsHead_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    txtEndDeptSubsHead.Text = null;
                    cboEndDeptSubsHead.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboHospSubsDirector_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    txtHospSubsDirector.Text = null;
                    cboHospSubsDirector.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTreatmentEndType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                // lần đầu khi đã kết thúc thì không tự mở form
                if (this.isFinished)
                {
                    this.isFinished = false;
                    return;
                }
                if (cboTreatmentEndType.EditValue != null)
                {
                    MOS.EFMODEL.DataModels.HIS_TREATMENT_END_TYPE data = this.hisTreatmentEndTypes.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentEndType.EditValue ?? 0).ToString()));
                    if (data != null)
                    {
                        ShowPopupEndType(data);
                        LoadSoNgayDieuTri();
                        if (cboTTExt.Visible == true)
                        {
                            cboTTExt.Focus();
                        }
                        else
                        {
                            SendKeys.Send("{TAB}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDeleteEndInfo_Click(object sender, EventArgs e)
        {
            try
            {
                if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn xóa thông tin ra viện không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    if (this.currentHisTreatment != null)
                    {
                        bool success = false;
                        CommonParam param = new CommonParam();
                        var result = new BackendAdapter(param).Post<HIS_TREATMENT>("api/HisTreatment/DeleteEndInfo", ApiConsumers.MosConsumer, currentHisTreatment.ID, param);
                        if (result != null)
                        {
                            success = true;
                            BackendDataWorker.Reset<HIS_TREATMENT>();
                            currentHisTreatment = result;
                            FillDataCurrentTreatment(currentHisTreatment);
                            EnableControlByTreatment();    

                        }
                        MessageManager.Show(this, param, success);
                    }
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void txtEndDeptSubsHead_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                dxErrorProvider1.SetError(txtEndDeptSubsHead, "", ErrorType.None);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtHospSubsDirector_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                dxErrorProvider1.SetError(txtHospSubsDirector, "", ErrorType.None);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtHosReasonNt_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboHosReason.EditValue = null;
                    txtHosReasonNt.Text = null;
                }
                else if (e.Button.Kind == ButtonPredefines.Combo)
                {
                    cboHosReason.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtHosReasonNt_DoubleClick(object sender, EventArgs e)
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

        private void cboHosReason_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboHosReason.Text = null;
                    cboHosReason.EditValue = null;
                }
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
                    txtHosReasonNt.Text = (cboHosReason.Properties.DataSource as List<MOS.EFMODEL.DataModels.HIS_HOSPITALIZE_REASON>).FirstOrDefault(o => o.ID == Int64.Parse(cboHosReason.EditValue.ToString())).HOSPITALIZE_REASON_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboHisHospitalizeReason()
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_HOSPITALIZE_REASON> datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_HOSPITALIZE_REASON>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                InitComboHisHospitalizeReason(datas);
                cboHosReason.EditValue = null;
                dxValidationProvider.SetValidationRule(txtHosReasonNt, null);
                if (currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU || currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                {
                    lciHosReason.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidateComboHosspitalizeReason();
                }
                else
                {
                    lciHosReason.Text = " ";
                    panel1.Controls.Clear();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboHisHospitalizeReason(List<HIS_HOSPITALIZE_REASON> data)
        {
            try
            {
                try
                {
                    cboHosReason.Properties.DataSource = data;
                    cboHosReason.Properties.DisplayMember = "HOSPITALIZE_REASON_NAME";
                    cboHosReason.Properties.ValueMember = "ID";

                    cboHosReason.Properties.View.OptionsView.GroupDrawMode = DevExpress.XtraGrid.Views.Grid.GroupDrawMode.Office;
                    cboHosReason.Properties.View.OptionsView.HeaderFilterButtonShowMode = DevExpress.XtraEditors.Controls.FilterButtonShowMode.SmartTag;
                    cboHosReason.Properties.View.OptionsView.ShowAutoFilterRow = true;
                    cboHosReason.Properties.View.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
                    cboHosReason.Properties.View.OptionsView.ShowDetailButtons = false;
                    cboHosReason.Properties.View.OptionsView.ShowGroupPanel = false;
                    cboHosReason.Properties.View.OptionsView.ShowIndicator = false;
                    cboHosReason.Properties.View.RowCellClick += View_RowCellClick;


                    DevExpress.XtraGrid.Columns.GridColumn column = cboHosReason.Properties.View.Columns.AddField("HOSPITALIZE_REASON_CODE");
                    column.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
                    column.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
                    column.VisibleIndex = 1;
                    column.Width = 150;
                    column.Caption = "Mã";

                    DevExpress.XtraGrid.Columns.GridColumn column1 = cboHosReason.Properties.View.Columns.AddField("HOSPITALIZE_REASON_NAME");
                    column1.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
                    column1.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
                    column1.VisibleIndex = 2;
                    column1.Width = 250;
                    column1.Caption = "Tên";
                    cboHosReason.Properties.View.OptionsView.ShowColumnHeaders = true;
                    cboHosReason.Properties.View.OptionsSelection.MultiSelect = true;
                    cboHosReason.Properties.ImmediatePopup = true;
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
        private void View_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                cboHosReason.EditValue = ((HIS_HOSPITALIZE_REASON)cboHosReason.Properties.View.GetFocusedRow()).ID;
                cboHosReason.ClosePopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void ValidateComboHosspitalizeReason()
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = this.txtHosReasonNt;
                validRule.ErrorText = "Trường dữ liệu bắt buộc";
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(this.txtHosReasonNt, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidatecboCareer()
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = this.cboCareer;
                validRule.ErrorText = "Trường dữ liệu bắt buộc";
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(this.cboCareer, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

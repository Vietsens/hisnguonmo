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
using ACS.SDO;
using DevExpress.Data;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Tile;
using DevExpress.XtraRichEdit.API.Native;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.ModuleExt;
using HIS.Desktop.Plugins.Library.FormOtherSereServ;
using HIS.Desktop.Plugins.ServiceExecute.ADO;
using HIS.Desktop.Plugins.ServiceExecute.Config;
using HIS.Desktop.Plugins.ServiceExecute.Validation;
using HIS.Desktop.Utilities;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Common.RichEditor.DAL;
using Inventec.Common.SignLibrary;
using Inventec.Common.SignLibrary.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.LibraryMessage;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using MPS.ProcessorBase.EmrBusiness;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Markup;

namespace HIS.Desktop.Plugins.ServiceExecute
{
    public partial class UCServiceExecute : UserControlBase
    {
        #region Declare
        private int ActionType = 0;
        /// <summary>
        /// được map từ HIS_SERVICE_REQ nên bị thiếu dữ liệu Name, code
        /// </summary>
        private V_HIS_SERVICE_REQ ServiceReqConstruct;
        private ADO.ServiceADO sereServ;
        private HIS_SERE_SERV_EXT sereServExt;
        private SAR.EFMODEL.DataModels.SAR_PRINT currentSarPrint;
        private HIS_SERVICE_REQ currentServiceReq;
        private Inventec.Desktop.Common.Modules.Module moduleData;
        private List<HIS_SERE_SERV_TEMP> listTemplate;
        public HIS_SERE_SERV_EXT currentSereServExt = new HIS_SERE_SERV_EXT();
        private Dictionary<string, object> dicParam;
        private Dictionary<string, Image> dicImage;
        private List<ADO.ImageADO> listImage;
        private ADO.ImageADO currentDataClick;
        private Common.DelegateRefresh RefreshData;
        private List<ADO.ServiceADO> listServiceADO;
        private string camMonikerString;

        private List<HIS_TEXT_LIB> listHisTextLib;

        private ADO.ServiceADO mainSereServ;
        private List<ADO.ServiceADO> listServiceADOForAllInOne = new List<ADO.ServiceADO>();

        //private List<HisEkipUserADO> ekipUserAdos;
        private string keySubclinicalInfoOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ServiceExecuteCFG.SubclinicalProcessingInformationOption);
        private string keySubclinicalMachineOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ServiceExecuteCFG.SubclinicalMachineOption);

        private Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExt = new Dictionary<long, HIS_SERE_SERV_EXT>(); //sereServId,HIS_SERE_SERV_EXT
        private Dictionary<long, SAR.EFMODEL.DataModels.SAR_PRINT> dicSarPrint = new Dictionary<long, SAR.EFMODEL.DataModels.SAR_PRINT>();//sereServExt.ID, SAR_PRINT

        private Dictionary<long, List<HisEkipUserADO>> dicEkipUser = new Dictionary<long, List<HisEkipUserADO>>(); //sereServId,List<HisEkipUserADO>
        private Dictionary<long, HIS_SERE_SERV_PTTT> dicSereServPttt = new Dictionary<long, HIS_SERE_SERV_PTTT>(); //sereServId,HIS_SERE_SERV_PTTT
        private Dictionary<long, List<V_HIS_SERE_SERV_SUIN>> dicSereServSuin = new Dictionary<long, List<V_HIS_SERE_SERV_SUIN>>(); //sereServId,HIS_SERE_SERV_PTTT
        private Dictionary<long, string> dicOldData = new Dictionary<long, string>();

        private bool isPressButtonSave;
        private ADO.PatientADO patient;
        private HisTreatmentWithPatientTypeInfoSDO TreatmentWithPatientTypeAlter;

        private string Loginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
        private string UserName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
        private string thoiGianKetThuc = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ServiceExecuteCFG.ThoiGianKetThuc);
        private string HideTimePrint = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ServiceExecuteCFG.HideTimePrint);
        private string ConnectPacsByFss = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ServiceExecuteCFG.ConnectPacsByFss);
        private string ConnectImageOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ServiceExecuteCFG.ConnectImageOption);

        private const int SERVICE_CODE__MAX_LENGTH = 6;
        private const int SERVICE_REQ_CODE__MAX_LENGTH = 9;
        private const string tempFolder = @"Img\Temp";
        private string[] fss = new string[] { "FSS\\" };

        private List<HIS_MACHINE> ListMachine { get; set; }
        private List<HIS_SERVICE_MACHINE> ListServiceMachine { get; set; }

        private int positionHandle = -1;
        private bool? isExecuter = null, isReadResult = null;


        private string positionProtect = "";

        private bool isWordFull = false;
        private UcWordFull wordFullDocument;
        private UcWord wordDocument;

        private GridColumn lastColumnSS = null;
        private int lastRowHandleSS = -1;
        private ToolTipControlInfo lastInfoSS = null;

        private bool isNotLoadWhileChangeControlStateInFirst;
        private HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        private List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentBySessionControlStateRDO;
        private const string moduleLink = "HIS.Desktop.Plugins.ServiceExecute";
        private const string trackBarZoomName = "trackBarZoom";
        private long trackBarZoom;

        private UcWords.UcTelerik UcTelerikDocument;
        private UcWords.UcTelerikFullWord UcTelerikFullDocument;

        protected string currentBussinessCode;

        private string SelectedFolderForSaveImage;

        private string FolderSaveImage = Application.StartupPath + @"\Img\CLS";
        internal enum RightButtonType
        {
            Copy,
            ChangeStt
        }
        enum ContainerClick
        {
            None,
            AutoCapture
        }

        ContainerClick currentContainerClick = ContainerClick.None;
        DateTime currentTimer;
        TimerSDO timeSync { get; set; }
        bool? WarningConfig;
        List<V_HIS_SERVICE> lstService { get; set; }
        List<HIS_DEPARTMENT> lstDepartment { get; set; }
        List<HIS_EXECUTE_ROLE> lstExecuteRole { get; set; }
        public bool IsPin { get; private set; }
        public bool IsLoadFromPin { get; private set; }
        List<HisEkipUserADO> ekipUserAdos { get; set; }
        List<V_HIS_BED_LOG> lstBedLogData { get; set; }
        List<HIS_DHST> lstDhstData { get; set; }
        #endregion

        #region Construct
        public UCServiceExecute(Inventec.Desktop.Common.Modules.Module moduleData, MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ serviceReq, HIS.Desktop.Common.DelegateRefresh RefreshData, bool? isExecuter, bool? isReadResult)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.moduleData = moduleData;
                this.ServiceReqConstruct = serviceReq;
                if (RefreshData != null)
                {
                    this.RefreshData = RefreshData;
                }
                this.isExecuter = isExecuter;
                this.isReadResult = isReadResult;
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                InitUcWord();

                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ServiceExecuteCFG.ShowImageCFG) == "1" &&
                    serviceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA)
                {
                    lcgImage.Expanded = false;
                }
                else
                {
                    lcgImage.Expanded = true;
                }

                //cấu hình in qua template khác sẽ hiển thị luôn còn không sẽ ẩn đi để tiết kiệm diện tích
                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ServiceExecuteCFG.OptionPrint) == "1")
                {
                    LcgPatientInfo.Expanded = true;
                }
                else
                {
                    LcgPatientInfo.Expanded = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcWord()
        {
            try
            {
                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ServiceExecuteCFG.OptionDescription) == "1")
                {
                    this.UcTelerikDocument = new UcWords.UcTelerik(EditorZoomChanged);
                    this.UcTelerikDocument.Dock = DockStyle.Fill;
                    this.panelDescription.Controls.Add(this.UcTelerikDocument);
                }
                else
                {
                    this.wordDocument = new UcWord(EditorZoomChanged);
                    this.panelDescription.Controls.Add(this.wordDocument);
                    this.wordDocument.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                //Đảm bảo luôn init document của devexpress
                this.wordDocument = new UcWord(EditorZoomChanged);
                this.panelDescription.Controls.Add(this.wordDocument);
                this.wordDocument.Dock = DockStyle.Fill;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method
        #region load
        private void UCServiceExecute_Load(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("UCServiceExecute_Load.1");
                if (!Directory.Exists(FolderSaveImage))
                    Directory.CreateDirectory(FolderSaveImage);

                GetDataFromRam();
                isNotLoadWhileChangeControlStateInFirst = true;
                LoadKeysFromlanguage();
                Inventec.Common.Logging.LogSystem.Debug("UCServiceExecute_Load.2");
                this.LoadExecuteRoleUser();
                timerLoadEkip.Enabled = true;
                Inventec.Common.Logging.LogSystem.Debug("UCServiceExecute_Load.3");
                RegisterTimer(moduleData.ModuleLink, "timerLoadEkip", timerLoadEkip.Interval, timerLoadEkip_Tick);
                StartTimer(moduleData.ModuleLink, "timerLoadEkip");
                SetDefaultValueControl();
                //InitCboMachineOption();
                InitControlState();
                InitDrButtonOther();
                CreateThreadLoadDataDefault();
                Inventec.Common.Logging.LogSystem.Debug("UCServiceExecute_Load.4");
                FillDataCombo();
                Inventec.Common.Logging.LogSystem.Debug("UCServiceExecute_Load.5");
                FillDataToGrid();
                Inventec.Common.Logging.LogSystem.Debug("UCServiceExecute_Load.6");
                ProcessPatientInfo();
                Inventec.Common.Logging.LogSystem.Debug("UCServiceExecute_Load.7");
                bool sereServFileResult = false;
                if (listServiceADO != null && listServiceADO.Count > 0)
                {
                    sereServFileResult = ProcessLoadSereServFile(listServiceADO.Select(s => s.ID).Distinct().ToList());
                }
                else if (this.sereServ == null || (!sereServFileResult && ConnectImageOption != "1" && this.sereServ != null && String.IsNullOrEmpty(this.sereServ.TDL_PACS_TYPE_CODE)))
                {
                    LoadDataImageLocal();
                }
                Inventec.Common.Logging.LogSystem.Debug("UCServiceExecute_Load.8");
                SetDisable();
                ValidNumberOfFilm();
                ValidBeginTime();
                ValidEndTime();
                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_SERVICE_REQ.IS_ALLOWING_PROCESSING_SUBCLINICAL_AFTER_LOCKING_TREATMENT") == "1" && TreatmentWithPatientTypeAlter != null && TreatmentWithPatientTypeAlter.IS_LOCK_FEE == 1)
                {
                    btnAssignService.Enabled = false;
                    btnTuTruc.Enabled = false;
                }
                SetEnableControlWithExecuterParam();
                Inventec.Common.Logging.LogSystem.Debug("UCServiceExecute_Load.9");
                LoadDataToCombo();
                EnableControlCamera(false);
                Inventec.Common.Logging.LogSystem.Debug("UCServiceExecute_Load.10");
                CheckValidPress();
                InitCameraDefault();
                isNotLoadWhileChangeControlStateInFirst = false;
                this.ProcessCustomizeUI();
                InitBarContentLibrary(this.listHisTextLib);
                GetTimeSystem();
                RegisterTimer(moduleData.ModuleLink, "timer1", timer1.Interval, timer1_Tick);
                StartTimer(moduleData.ModuleLink, "timer1");
                RegisterTimer(moduleData.ModuleLink, "timerDoubleClick", timerDoubleClick.Interval, timerDoubleClick_Tick);
                DisableFinishConfig();
                Inventec.Common.Logging.LogSystem.Debug("UCServiceExecute_Load.11");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataDefaultHisBedLog()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentServiceReq), currentServiceReq));
                CommonParam param = new CommonParam();
                Inventec.Common.Logging.LogSystem.Info("1. Begin api/HisBedLog/GetView ");
                HisBedLogViewFilter blFilter = new HisBedLogViewFilter();
                if (ServiceReqConstruct != null)
                {
                    blFilter.TREATMENT_ID = ServiceReqConstruct.TREATMENT_ID;
                }
                lstBedLogData = new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumer.ApiConsumers.MosConsumer, blFilter, param);
                Inventec.Common.Logging.LogSystem.Info("1. End api/HisBedLog/GetView ");

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataDefaultHisDHST()
        {
            try
            {
                CommonParam param = new CommonParam();
                Inventec.Common.Logging.LogSystem.Info("1. Begin api/HisDhst/Get ");
                HisDhstFilter dhFilter = new HisDhstFilter();
                if (ServiceReqConstruct != null)
                {
                    dhFilter.TREATMENT_ID = ServiceReqConstruct.TREATMENT_ID;
                }
                lstDhstData = new BackendAdapter(param).Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumer.ApiConsumers.MosConsumer, dhFilter, param);
                Inventec.Common.Logging.LogSystem.Info("1. End api/HisDhst/Get ");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private async Task LoadSuim(bool IsReloadCallApi = false)
        {

            try
            {
                if (sereServ == null)
                    return;
                if (!dicSereServSuin.ContainsKey(sereServ.ID) || IsReloadCallApi)
                    dicSereServSuin[sereServ.ID] = new List<V_HIS_SERE_SERV_SUIN>();
                else
                    return;
                List<V_HIS_SERE_SERV_SUIN> listHisssSuin = null;
                List<HIS_SUIM_SETY_SUIN> listHisSuimSety = null;
                List<V_HIS_SUIM_INDEX> listHisSuimIndex = null;
                List<Task> lstTask = new List<Task>();
                CommonParam param = new CommonParam();
                Task tSety = Task.Factory.StartNew(() =>
                {
                    HisSuimSetySuinFilter SetyFilter = new HisSuimSetySuinFilter();
                    SetyFilter.SUIM_SERVICE_TYPE_ID = sereServ.SERVICE_ID;
                    listHisSuimSety = new BackendAdapter(param).Get<List<HIS_SUIM_SETY_SUIN>>("api/HisSuimSetySuin/Get", ApiConsumers.MosConsumer, SetyFilter, null).ToList();
                    if (listHisSuimSety != null && listHisSuimSety.Count > 0)
                    {
                        HisSuimIndexViewFilter indexFilter = new HisSuimIndexViewFilter();
                        indexFilter.IDs = listHisSuimSety.Select(o => o.SUIM_INDEX_ID).ToList();
                        listHisSuimIndex = new BackendAdapter(param).Get<List<V_HIS_SUIM_INDEX>>("api/HisSuimIndex/GetView", ApiConsumers.MosConsumer, indexFilter, null).ToList();
                    }
                });
                lstTask.Add(tSety);

                Task tSuin = Task.Factory.StartNew(() =>
                {
                    HisSereServSuinViewFilter SereSuimFilter = new HisSereServSuinViewFilter();
                    SereSuimFilter.SERE_SERV_IDs = new List<long>() { sereServ.ID };
                    listHisssSuin = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_SUIN>>("api/HisSereServSuin/GetView", ApiConsumers.MosConsumer, SereSuimFilter, null).ToList();
                });
                lstTask.Add(tSuin);

                Task.WaitAll(lstTask.ToArray());
                if (listHisSuimSety != null && listHisSuimSety.Count > 0)
                {
                    foreach (var item in listHisSuimSety)
                    {
                        V_HIS_SERE_SERV_SUIN ss = new V_HIS_SERE_SERV_SUIN();
                        if (listHisssSuin != null && listHisssSuin.Count > 0)
                        {
                            ss = listHisssSuin.FirstOrDefault(o => o.SUIM_INDEX_ID == item.SUIM_INDEX_ID);
                        }
                        else
                        {
                            ss.SERE_SERV_ID = sereServ.ID;
                            var suimIndex = listHisSuimIndex.FirstOrDefault(o => o.ID == item.SUIM_INDEX_ID);
                            if (suimIndex != null)
                            {
                                ss.SUIM_INDEX_ID = suimIndex.ID;
                                ss.SUIM_INDEX_CODE = suimIndex.SUIM_INDEX_CODE;
                                ss.SUIM_INDEX_NAME = suimIndex.SUIM_INDEX_NAME;
                                ss.SUIM_INDEX_UNIT_CODE = suimIndex.SUIM_INDEX_UNIT_CODE;
                                ss.SUIM_INDEX_UNIT_NAME = suimIndex.SUIM_INDEX_UNIT_NAME;
                            }
                        }
                        if (ss != null && ss.SERE_SERV_ID > 0)
                            dicSereServSuin[sereServ.ID].Add(ss);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataFromRam()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("GetDataFromRam.1");
                lstService = BackendDataWorker.Get<V_HIS_SERVICE>().ToList();
                lstDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == 1).ToList();
                lstExecuteRole = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().Where(o => o.IS_ACTIVE == 1 && o.IS_DISABLE_IN_EKIP != 1).ToList().ToList();

                ListMachine = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MACHINE>();
                ListServiceMachine = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_MACHINE>();
                this.listHisTextLib = BackendDataWorker.Get<HIS_TEXT_LIB>();
                Inventec.Common.Logging.LogSystem.Debug("GetDataFromRam.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void DisableFinishConfig()
        {
            try
            {
                var employee = BackendDataWorker.Get<V_HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());
                if (Config.AppConfigKeys.IsAllowFinishWhenAccountIsDoctor && employee.IS_DOCTOR != 1)
                {
                    WarningConfig = true;
                    btnFinish.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitBarContentLibrary(List<HIS_TEXT_LIB> listText)
        {
            try
            {
                listText = listText.Where(o => !String.IsNullOrEmpty(o.HOT_KEY) && o.CREATOR == Loginname || o.IS_PUBLIC == 1 && o.LIB_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_LIB_TYPE.ID__TEXT).OrderBy(p => p.HOT_KEY).ToList();

                this.lciContentLibrary.Controls.Clear();
                this.xtraScrollableContentLibrary.Controls.Clear();
                var numberOfText = listText.Count;
                if (numberOfText == 0)
                {
                    layoutControlItem44.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                if (numberOfText > 0)
                {
                    int oldLocation = 0;
                    for (int i = 0; i < numberOfText; i++)
                    {
                        // Khoi tao labelcontrol
                        DevExpress.XtraEditors.LabelControl lblControl = new DevExpress.XtraEditors.LabelControl();
                        if (i % 2 == 0)
                            lblControl.ForeColor = Color.Blue;
                        lblControl.Font = new Font(lblControl.Font, FontStyle.Bold);
                        lblControl.Name = listText[i].HOT_KEY;
                        lblControl.Text = listText[i].HOT_KEY;
                        lblControl.Tag = listText[i];
                        lblControl.TabIndex = 4;
                        var keySize = lblControl.CalcBestSize();
                        lblControl.Location = new System.Drawing.Point((oldLocation), 0);
                        lblControl.Size = new System.Drawing.Size(keySize.Width, 13);
                        lblControl.StyleController = this.lciContentLibrary;

                        // Gan event cho label control
                        lblControl.Click += new System.EventHandler(this.labelControlSentText_Click);


                        oldLocation += keySize.Width + 10;
                        // add laybel control to layout
                        this.lciContentLibrary.Controls.Add(lblControl);
                        xtraScrollableContentLibrary.Controls.Add(lblControl);
                    }

                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void labelControlSentText_Click(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.LabelControl lblControl = sender as DevExpress.XtraEditors.LabelControl;
                HIS_TEXT_LIB data = lblControl.Tag as HIS_TEXT_LIB;

                if (data != null)
                {
                    string bitString = HIS.Desktop.Utility.TextLibHelper.BytesToString(data.CONTENT);
                    SendKeys.SendWait(bitString);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }

        }

        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentControlStateRDO), currentControlStateRDO));
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkForPreview.Name)
                        {
                            chkForPreview.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == ChkAutoFinish.Name)
                        {
                            ChkAutoFinish.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == trackBarZoomName)
                        {
                            trackBarZoom = Inventec.Common.TypeConvert.Parse.ToInt64(item.VALUE);
                            ProcessSetWordZoom(trackBarZoom);
                        }
                        else if (item.KEY == chkAttachImage.Name)
                        {
                            chkAttachImage.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkClose.Name)
                        {
                            chkClose.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkPrint.Name)
                        {
                            chkPrint.Checked = item.VALUE == "1";
                        }
                        //else if (item.KEY == cboMachineOption.Name)
                        //{
                        //    cboMachineOption.EditValue = Inventec.Common.TypeConvert.Parse.ToInt64(item.VALUE ?? "0");
                        //}
                        else if (item.KEY == chkSign.Name)
                        {
                            chkSign.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkSaveImageToFile.Name)
                        {
                            chkSaveImageToFile.Checked = false;
                            if (!String.IsNullOrWhiteSpace(item.VALUE))
                            {
                                var data = item.VALUE.Split('|');
                                if (data.Count() == 2)
                                {
                                    if (!String.IsNullOrWhiteSpace(data[1]))
                                    {
                                        chkSaveImageToFile.Checked = data[0] == "1";
                                        this.SelectedFolderForSaveImage = data[1];
                                    }
                                    else
                                    {
                                        chkSaveImageToFile.Checked = false;
                                        this.SelectedFolderForSaveImage = null;
                                    }
                                }
                            }
                        }
                        else if (item.KEY == chkUpper.Name)
                        {
                            chkUpper.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkAutoCapture.Name)
                        {
                            chkAutoCapture.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == spnTotalCapture.Name)
                        {
                            spnTotalCapture.Value = Int32.Parse(item.VALUE);
                        }
                        else if (item.KEY == spnTotalTimeToCapture.Name)
                        {
                            spnTotalTimeToCapture.Value = decimal.Parse(item.VALUE);
                        }
                        else if (item.KEY == chkKeTieuHao.Name)
                        {
                            chkKeTieuHao.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == xtraTabControl1.Name)
                        {
                            if (item.VALUE == "1")
                            {
                                xtraTabControl1.CustomHeaderButtons[0].Visible = true;
                                xtraTabControl1.CustomHeaderButtons[1].Visible = false;
                                IsPin = true;
                            }
                            else
                            {
                                xtraTabControl1.CustomHeaderButtons[0].Visible = false;
                                xtraTabControl1.CustomHeaderButtons[1].Visible = true;
                                IsPin = false;
                            }
                        }
                    }
                }
                else
                {
                    ProcessSetWordZoom(GetZoom());
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => trackBarZoom), trackBarZoom)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.wordDocument.txtDescription.ActiveView.ZoomFactor), this.wordDocument.txtDescription.ActiveView.ZoomFactor));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataSourceEkipUser()
        {
            try
            {
                InformationADO ado = new InformationADO();
                IsLoadFromPin = false;
                this.currentBySessionControlStateRDO = controlStateWorker.GetDataBySession(moduleLink);
                if (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0)
                {

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentBySessionControlStateRDO), currentBySessionControlStateRDO));
                    foreach (var item in this.currentBySessionControlStateRDO)
                    {
                        if (item.KEY == "InformationADO")
                        {
                            if (!string.IsNullOrEmpty(item.VALUE))
                            {
                                ado = JsonConvert.DeserializeObject<InformationADO>(item.VALUE);
                            }
                            else
                            {
                                xtraTabControl1.CustomHeaderButtons[0].Visible = false;
                                xtraTabControl1.CustomHeaderButtons[1].Visible = true;
                                IsPin = false;
                            }
                        }
                    }
                    if (ado != null)
                    {
                        //cboEkipUserTemp.EditValue = ado.EKIP_TEMP_ID;
                        //cboEkipDepartment.EditValue = ado.EKIP_DEPARTMENT_ID;
                        gridControlEkip.DataSource = null;
                        if (ado.ListEkipUser != null && ado.ListEkipUser.Count > 0)
                        {
                            IsLoadFromPin = true;
                            gridControlEkip.DataSource = ado.ListEkipUser;
                        }
                        else
                        {
                            List<HisEkipUserADO> ekipUserAdos = new List<HisEkipUserADO>();
                            HisEkipUserADO a = new HisEkipUserADO();
                            a.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                            ekipUserAdos.Add(a);
                            gridControlEkip.DataSource = ekipUserAdos;
                        }
                    }
                }
                else
                {
                    xtraTabControl1.CustomHeaderButtons[0].Visible = false;
                    xtraTabControl1.CustomHeaderButtons[1].Visible = true;
                    IsPin = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// 1. Nếu chỉ có 1 camera --> thì auto chọn camera khi click đúp xử lý cho BN (siêu âm/ nội soi), không phải ctrl D
        ///2. TH có 2 camera thì tự động auto chọn 1 camera và gọi hình luôn, ưu tiên Svideo
        ///3. TH có 2 camera, 1 camera bị chiếm chỗ thì tự động chọn cái còn lại và gọi hình luôn (VD: hiện tại CĐHA đang dùng cả medisoft và HISpro để đọc KQ SA, NS. Medisoft đã chiếm 1 cái rồi thì HIS tự động hiểu cái còn lại.
        /// </summary>
        private void InitCameraDefault()
        {
            try
            {
                if (!HIS.Desktop.Plugins.ServiceExecute.Config.AppConfigKeys.IsInitCameraDefault)
                {
                    return;
                }

                if (cboConnectionType.EditValue == null)
                {
                    return;
                }
                bool autoStart = false;
                List<Inventec.UC.ImageLib.Core.CameraDevice> camdivices = null;
                if ((int)cboConnectionType.EditValue == 2)
                {
                    camdivices = Inventec.UC.ImageLib.CameraDiviceProcessor.GetUsbCameraDevices();
                }
                else
                {
                    camdivices = Inventec.UC.ImageLib.CameraDiviceProcessor.GetSvideoCameraDevices();
                }
                if (camdivices == null || camdivices.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("InitCameraDefault: khong tim thay device camera nao tren may tinh");
                    return;
                }
                if (camdivices.Count == 1)
                {
                    autoStart = true;
                    camMonikerString = camdivices[0].MonikerString;
                }
                else
                {
                    autoStart = true;
                    camMonikerString = camdivices[0].MonikerString;
                }
                if (autoStart)
                {
                    EnableControlCamera(true);
                    StartCamera();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessPatientInfo()
        {
            try
            {
                if (TreatmentWithPatientTypeAlter != null)
                {
                    this.LblGender.Text = TreatmentWithPatientTypeAlter.TDL_PATIENT_GENDER_NAME;
                    this.LblHeinCardNumber.Text = string.Format("{0} ({1} - {2})", HeinCardHelper.SetHeinCardNumberDisplayByNumber(TreatmentWithPatientTypeAlter.HEIN_CARD_NUMBER), Inventec.Common.DateTime.Convert.TimeNumberToDateString(TreatmentWithPatientTypeAlter.HEIN_CARD_FROM_TIME), Inventec.Common.DateTime.Convert.TimeNumberToDateString(TreatmentWithPatientTypeAlter.HEIN_CARD_TO_TIME));
                    if (TreatmentWithPatientTypeAlter.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                        this.LblPatientDob.Text = TreatmentWithPatientTypeAlter.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    else
                        this.LblPatientDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(TreatmentWithPatientTypeAlter.TDL_PATIENT_DOB);

                    this.LblPatientName.Text = TreatmentWithPatientTypeAlter.TDL_PATIENT_NAME;
                    var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == TreatmentWithPatientTypeAlter.TDL_PATIENT_TYPE_ID);
                    this.LblPatientType.Text = patientType != null ? patientType.PATIENT_TYPE_NAME : "";
                    var treatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == TreatmentWithPatientTypeAlter.TDL_TREATMENT_TYPE_ID);
                    this.LblTreatmentType.Text = treatmentType != null ? treatmentType.TREATMENT_TYPE_NAME : "";
                    this.LblAddress.Text = TreatmentWithPatientTypeAlter.TDL_PATIENT_ADDRESS;

                    if (sereServExt != null && sereServExt.ID > 0)
                    {
                        this.LblExecuteName.Text = string.Format("{0} ({1})", sereServExt.SUBCLINICAL_RESULT_USERNAME, sereServExt.SUBCLINICAL_RESULT_LOGINNAME);
                    }
                    else
                        this.LblExecuteName.Text = string.Format("{0} ({1})", currentServiceReq.EXECUTE_USERNAME, currentServiceReq.EXECUTE_LOGINNAME);

                    List<string> ktv = new List<string>();
                    List<string> nurse = new List<string>();
                    foreach (var item in dicSereServExt)
                    {
                        if (!String.IsNullOrWhiteSpace(item.Value.SUBCLINICAL_PRES_LOGINNAME))
                        {
                            ktv.Add(string.Format("{0} ({1})", item.Value.SUBCLINICAL_PRES_USERNAME, item.Value.SUBCLINICAL_PRES_LOGINNAME));
                        }

                        if (!String.IsNullOrWhiteSpace(item.Value.SUBCLINICAL_NURSE_LOGINNAME))
                        {
                            nurse.Add(string.Format("{0} ({1})", item.Value.SUBCLINICAL_NURSE_USERNAME, item.Value.SUBCLINICAL_NURSE_LOGINNAME));
                        }
                    }

                    this.LblKtv.Text = ktv != null ? string.Join(",", ktv.Distinct().ToList()) : "";
                    this.LblNurse.Text = "";

                    if (nurse != null && nurse.Count > 0)
                    {
                        this.LblNurse.Text = string.Join(",", nurse.Distinct().ToList());
                    }
                    else
                    {
                        var workPlace = WorkPlace.GetWorkPlace(moduleData);
                        if (WorkPlace.WorkInfoSDO != null && !String.IsNullOrWhiteSpace(WorkPlace.WorkInfoSDO.NurseLoginName))
                        {
                            this.LblNurse.Text = string.Format("{0} ({1})", WorkPlace.WorkInfoSDO.NurseUserName, WorkPlace.WorkInfoSDO.NurseLoginName);
                        }
                    }

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => nurse), nurse));
                }
                else
                {
                    this.LblGender.Text = "";
                    this.LblHeinCardNumber.Text = "";
                    this.LblPatientDob.Text = "";
                    this.LblPatientName.Text = "";
                    this.LblPatientType.Text = "";
                    this.LblTreatmentType.Text = "";
                    this.LblAddress.Text = "";
                    this.LblExecuteName.Text = "";
                    this.LblKtv.Text = "";
                    this.LblNurse.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckValidPress()
        {
            try
            {
                if (listServiceADO != null && listServiceADO.Count > 0)
                {
                    bool isDisable = false;
                    if (listServiceADO.Exists(o => o.MustHavePressBeforeExecute))
                    {
                        var serviceName = listServiceADO.Where(o => o.MustHavePressBeforeExecute).Select(s => s.TDL_SERVICE_NAME).ToList();
                        DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.DichVuChuaKeThuocVatTu, string.Join(",", serviceName)));

                        Inventec.Common.Logging.LogSystem.Warn(string.Format(ResourceMessage.DichVuChuaKeThuocVatTu, string.Join(",", serviceName)) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listServiceADO), listServiceADO));
                        if (serviceName.Count == listServiceADO.Count)
                        {
                            isDisable = true;
                        }
                    }

                    if (isDisable)
                    {
                        SetEnableControl(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetEnableControlWithExecuterParam()
        {
            try
            {
                if (this.isReadResult.HasValue && this.isReadResult == true)
                {
                    btnAssignPrescription.Enabled = false;
                    btnTuTruc.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadKeysFromlanguage()
        {
            try
            {
                //layout
                this.tileViewColumn2.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.tileViewColumn2.Caption");
                this.tileViewColumnName.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.tileViewColumnName.Caption");
                this.tileViewColumnSTTImage.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.tileViewColumnSTTImage.Caption");
                this.tileViewColumn4.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.tileViewColumn4.Caption");
                this.layoutControl1.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.layoutControl1.Text");
                this.layoutControl5.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.layoutControl5.Text");
                this.btnKhaiBaoVTTH.ToolTip = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.btnKhaiBaoVTTH.ToolTip");
                this.chkKeTieuHao.Properties.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.chkKeTieuHao.Properties.Caption");
                this.chkKeTieuHao.ToolTip = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.chkKeTieuHao.ToolTip");
                this.bar1.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.bar1.Text");
                this.lciContentLibrary.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.lciContentLibrary.Text");
                this.chkAutoCapture.Properties.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.chkAutoCapture.Properties.Caption");
                this.chkAutoCapture.ToolTip = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.chkAutoCapture.ToolTip");
                this.btnShowConfig.ToolTip = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.btnShowConfig.ToolTip");
                this.btnSaveEkipTemp.ToolTip = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.btnSaveEkipTemp.ToolTip");
                this.cboEkipDepartment.Properties.NullText = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.cboEkipDepartment.Properties.NullText");
                this.xtraTabPageConclude.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.xtraTabPageConclude.Text");
                this.layoutControl2.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.layoutControl2.Text");
                this.txtConclude.Properties.NullValuePrompt = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.txtConclude.Properties.NullValuePrompt");
                this.chkUpper.Properties.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.chkUpper.Properties.Caption");
                this.chkUpper.ToolTip = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.chkUpper.ToolTip");
                this.txtNote.Properties.NullValuePrompt = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.txtNote.Properties.NullValuePrompt");
                this.xtraTabPageEkip.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.xtraTabPageEkip.Text");
                this.layoutControl3.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.layoutControl3.Text");
                this.gridColTitles.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.gridColTitles.Caption");
                this.repositoryItemCboPosition.NullText = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.repositoryItemCboPosition.NullText");
                this.gridColUserName.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.gridColUserName.Caption");
                this.repositoryItemCboUser.NullText = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.repositoryItemCboUser.NullText");
                this.gridColumn3.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.gridColumn3.Caption");
                this.repositoryItemCboDepartment.NullText = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.repositoryItemCboDepartment.NullText");
                this.gridColAdd.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.gridColAdd.Caption");
                this.gridColDelete.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.gridColDelete.Caption");
                this.chkSaveImageToFile.Properties.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.chkSaveImageToFile.Properties.Caption");
                this.chkSaveImageToFile.ToolTip = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.chkSaveImageToFile.ToolTip");
                this.chkSign.Properties.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.chkSign.Properties.Caption");
                this.chkSign.ToolTip = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.chkSign.ToolTip");
                this.cboEkipUserTemp.Properties.NullText = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.cboEkipUserTemp.Properties.NullText");
                this.chkClose.Properties.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.chkClose.Properties.Caption");
                this.chkClose.ToolTip = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.chkClose.ToolTip");
                this.chkPrint.Properties.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.chkPrint.Properties.Caption");
                this.chkPrint.ToolTip = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.chkPrint.ToolTip");
                this.dropDownButton.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.dropDownButton.Text");
                this.chkAttachImage.Properties.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.chkAttachImage.Properties.Caption");
                this.ChkAutoFinish.Properties.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.ChkAutoFinish.Properties.Caption");
                this.ChkAutoFinish.ToolTip = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.ChkAutoFinish.ToolTip");
                this.lblNumberOfImageSelected.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.lblNumberOfImageSelected.Text");
                this.txtServiceReqCode.Properties.NullValuePrompt = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.txtServiceReqCode.Properties.NullValuePrompt");
                this.chkForPreview.Properties.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.chkForPreview.Properties.Caption");
                this.chkForPreview.ToolTip = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.chkForPreview.ToolTip");
                this.layoutControl4.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.layoutControl4.Text");
                this.label1.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.label1.Text");
                this.btnBoQua.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.btnBoQua.Text");
                this.btnDongY.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.btnDongY.Text");
                this.layoutControlItem40.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.layoutControlItem40.Text");
                this.layoutControlItem41.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.layoutControlItem41.Text");
                this.simpleButton1.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.simpleButton1.Text");
                this.cboConnectionType.Properties.NullText = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.cboConnectionType.Properties.NullText");
                this.btnCamera.ToolTip = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.btnCamera.ToolTip");
                this.btnDeleteImage.ToolTip = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.btnDeleteImage.ToolTip");
                this.btnCapture.ToolTip = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.btnCapture.ToolTip");
                this.cboSizeOfFilm.Properties.NullText = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.cboSizeOfFilm.Properties.NullText");
                this.BtnChangeImage.ToolTip = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.BtnChangeImage.ToolTip");
                this.BtnChooseImage.ToolTip = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.BtnChooseImage.ToolTip");
                this.BtnEmr.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.BtnEmr.Text");
                this.BtnEmr.ToolTip = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.BtnEmr.ToolTip");
                this.CheckAllInOne.Properties.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.CheckAllInOne.Properties.Caption");
                this.btnTuTruc.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.btnTuTruc.Text");
                this.btnTuTruc.ToolTip = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.btnTuTruc.ToolTip");
                this.btnLoadImage.ToolTip = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.btnLoadImage.ToolTip");
                this.cboSereServTemp.Properties.NullText = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.cboSereServTemp.Properties.NullText");
                this.Gc_Stt.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.Gc_Stt.Caption");
                this.Gc_ImageStt.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.Gc_ImageStt.Caption");
                this.repositoryItemPictureStt.NullText = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.repositoryItemPictureStt.NullText");
                this.Gc_SendSancy.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.Gc_SendSancy.Caption");
                this.gridColumn1.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.gridColumn1.Caption");
                this.Gc_SereServTraKQ.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.Gc_SereServTraKQ.Caption");
                this.gridColumn18.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.gridColumn18.Caption");
                this.Gc_ServiceCode.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.Gc_ServiceCode.Caption");
                this.Gc_ServiceName.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.Gc_ServiceName.Caption");
                this.Gc_MachineId.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.Gc_MachineId.Caption");
                this.repositoryItemMachineId.NullText = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.repositoryItemMachineId.NullText");
                this.Gc_CreateTime.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.Gc_CreateTime.Caption");
                this.Gc_Creator.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.Gc_Creator.Caption");
                this.Gc_ModifyTime.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.Gc_ModifyTime.Caption");
                this.Gc_Modifier.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.Gc_Modifier.Caption");
                this.repositoryItemMachineHideDelete.NullText = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.repositoryItemMachineHideDelete.NullText");
                this.btnAssignPrescription.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.btnAssignPrescription.Text");
                this.btnAssignService.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.btnAssignService.Text");
                this.btnSave.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.btnSave.Text");
                this.btnFinish.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.btnFinish.Text");
                this.btnPrint.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.btnPrint.Text");
                this.tileViewItemElement1.Text = Resources.ResourceLanguageManager.GetValue("tileViewItemElement1.Text");
                this.tileViewItemElement2.Text = Resources.ResourceLanguageManager.GetValue("tileViewItemElement2.Text");
                this.tileViewItemElement3.Text = Resources.ResourceLanguageManager.GetValue("tileViewItemElement3.Text");
                this.tileViewIsChecked.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.tileViewIsChecked.Caption");
                this.Checked.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.Checked.Caption");
                this.gridColumnImage.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.gridColumnImage.Caption");
                this.gridColumn2.Caption = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.gridColumn2.Caption");
                this.lciSereServTempCode.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.lciSereServTempCode.Text");
                this.layoutControlGroup2.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.layoutControlGroup2.Text");
                this.lciMunberOfFilm.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.lciMunberOfFilm.Text");
                this.lcgImage.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.lcgImage.Text");
                this.layoutControlItem19.OptionsToolTip.ToolTip = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.layoutControlItem19.OptionsToolTip.ToolTip");
                this.layoutControlItem19.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.layoutControlItem19.Text");
                this.lciForlblNumberOfImageSelected.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.lciForlblNumberOfImageSelected.Text");
                this.layoutControlItem39.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.layoutControlItem39.Text");
                this.LciEndTime.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.LciEndTime.Text");
                this.layoutControlItem24.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.layoutControlItem24.Text");
                this.LciBeginTime.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.LciBeginTime.Text");
                this.lciEkipUserTemp.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.lciEkipUserTemp.Text");
                this.lciEkipDepartment.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.lciEkipDepartment.Text");
                this.LcgPatientInfo.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.LcgPatientInfo.Text");
                this.LciPatientName.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.LciPatientName.Text");
                this.LciPatientDob.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.LciPatientDob.Text");
                this.LciTreatmentType.OptionsToolTip.ToolTip = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.LciTreatmentType.OptionsToolTip.ToolTip");
                this.LciTreatmentType.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.LciTreatmentType.Text");
                this.LciPatientType.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.LciPatientType.Text");
                this.LciGender.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.LciGender.Text");
                this.LciHeinCardNumber.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.LciHeinCardNumber.Text");
                this.LciAddress.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.LciAddress.Text");
                this.LciExecuteName.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.LciExecuteName.Text");
                this.LciKtv.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.LciKtv.Text");
                this.layoutControlItem34.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.layoutControlItem34.Text");
                this.LciNurse.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.LciNurse.Text");
                this.layoutControlItem27.Text = Resources.ResourceLanguageManager.GetValue("UCServiceExecute.layoutControlItem27.Text");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThreadLoadDataDefault()
        {
            Thread serviceReq = new Thread(LoadCurrentServiceReq);
            Thread listTemplate = new Thread(ProcessLoadListTemplate);
            Thread dataFillTemplate = new Thread(ProcessDataForTemplate);
            Thread treatment = new Thread(LoadTreatmentWithPaty);
            Thread hisBedLog = new Thread(GetDataDefaultHisBedLog);
            Thread hisDHST = new Thread(GetDataDefaultHisDHST);
            try
            {
                serviceReq.Start();
                listTemplate.Start();
                dataFillTemplate.Start();
                treatment.Start();
                hisBedLog.Start();
                hisDHST.Start();

                serviceReq.Join();
                listTemplate.Join();
                dataFillTemplate.Join();
                treatment.Join();
                //hisbebDHST.Join();
            }
            catch (Exception ex)
            {
                serviceReq.Abort();
                listTemplate.Abort();
                dataFillTemplate.Abort();
                treatment.Abort();
                hisBedLog.Abort();
                hisDHST.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadTreatmentWithPaty()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("1. Begin LoadTreatmentWithPaty");
                CommonParam param = new CommonParam();

                MOS.Filter.HisTreatmentWithPatientTypeInfoFilter filter = new MOS.Filter.HisTreatmentWithPatientTypeInfoFilter();
                filter.TREATMENT_ID = this.ServiceReqConstruct.TREATMENT_ID;
                filter.INTRUCTION_TIME = this.ServiceReqConstruct.INTRUCTION_TIME;
                var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HisTreatmentWithPatientTypeInfoSDO>>(RequestUriStore.HIS_TREATMENT_GET_TREATMENT_WITH_PATIENT_TYPE_INFO_SDO, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (apiResult != null && apiResult.Count > 0)
                {
                    TreatmentWithPatientTypeAlter = apiResult.FirstOrDefault();
                }
                Inventec.Common.Logging.LogSystem.Info("1. End LoadTreatmentWithPaty");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCurrentServiceReq()
        {
            try
            {
                //load lại để lấy thông tin mới nhất cho template
                if (ServiceReqConstruct != null && currentServiceReq == null)
                {
                    Inventec.Common.Logging.LogSystem.Info("1. Begin LoadCurrentServiceReq ");
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceReqFilter reqFilter = new MOS.Filter.HisServiceReqFilter();
                    reqFilter.ID = ServiceReqConstruct.ID;
                    var result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(RequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumer.ApiConsumers.MosConsumer, reqFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (result != null && result.Count > 0)
                    {
                        currentServiceReq = result.FirstOrDefault();
                    }
                    Inventec.Common.Logging.LogSystem.Info("1. End LoadCurrentServiceReq ");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDisable()
        {
            try
            {
                //nếu hoàn thành sẽ khóa lại.
                //nếu không phải ng thực hiện sẽ khóa lại
                //nếu ko phải admin sẽ khóa lại
                //nếu hoàn thành và không phải ng xử lý và ko phải admin thì disable
                if (currentServiceReq != null &&
                    currentServiceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                //&& currentServiceReq.EXECUTE_LOGINNAME != Loginname && !HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(Loginname))
                {
                    SetEnableControl(false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetEnableControl(bool isEnabled)
        {
            try
            {
                this.btnPreYhct.Enabled = isEnabled;
                this.btnAssignPrescription.Enabled = isEnabled;
                this.btnAssignService.Enabled = isEnabled;
                this.btnCamera.Enabled = isEnabled;
                this.btnFinish.Enabled = isEnabled;
                this.btnLoadImage.Enabled = isEnabled;
                this.btnSave.Enabled = isEnabled;
                this.btnTuTruc.Enabled = isEnabled;
                this.cboSereServTemp.Enabled = isEnabled;
                this.txtSereServTempCode.Enabled = isEnabled;
                this.repositoryItemButtonServiceReqMaty.ReadOnly = !isEnabled;
                if (wordFullDocument != null)
                {
                    wordFullDocument.txtDescription.ReadOnly = !isEnabled;
                }

                if (wordDocument != null)
                {
                    wordDocument.txtDescription.ReadOnly = !isEnabled;
                }

                if (UcTelerikDocument != null)
                {
                    UcTelerikDocument.radRichTextEditor1.IsReadOnly = !isEnabled;
                }

                if (UcTelerikFullDocument != null)
                {
                    UcTelerikFullDocument.radRichTextEditor1.IsReadOnly = !isEnabled;
                }
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
                dicSarPrint = new Dictionary<long, SAR.EFMODEL.DataModels.SAR_PRINT>();
                dicSereServExt = new Dictionary<long, HIS_SERE_SERV_EXT>();
                txtConclude.Text = "";
                txtSereServTempCode.Text = "";
                cboSereServTemp.EditValue = null;
                cboSereServTemp.Properties.Buttons[1].Visible = false;
                dtEndTime.EditValue = null;
                dtBeginTime.EditValue = null;
                ClearDocument();
                txtSereServTempCode.Focus();
                txtSereServTempCode.SelectAll();
                this.ActionType = GlobalVariables.ActionAdd;
                btnPrint.Enabled = false;
                BtnEmr.Enabled = false;
                PACS.PacsCFG.Reload();
                Gc_SendSancy.VisibleIndex = -1;
                var numOfFilm = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ServiceExecuteCFG.NumberOfFilmCFG);
                if (ServiceReqConstruct != null && ServiceReqConstruct.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA && !string.IsNullOrEmpty(numOfFilm) && numOfFilm.Trim() == "1")
                    lciMunberOfFilm.AppearanceItemCaption.ForeColor = Color.Maroon;
                //lcgForDescriptionNote.Expanded = !HIS.Desktop.Plugins.ServiceExecute.Config.AppConfigKeys.IsHideConcludeAndNoteByDefault;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateRequireMemoEdit(MemoEdit memoEdit)
        {
            try
            {
                ValidateMemoEdit validate = new ValidateMemoEdit();
                validate.memoEdit = memoEdit;
                dxValidationProvider1.SetValidationRule(memoEdit, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                ReLoadSereServ();

                if (listServiceADO != null && listServiceADO.Count > 0)
                    SereServClickRow(listServiceADO[0]);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReLoadSereServ()
        {
            try
            {
                if (currentServiceReq == null) throw new ArgumentNullException("currentServiceReq is null");

                Inventec.Common.Logging.LogSystem.Debug("3.1");
                gridViewSereServ.BeginUpdate();
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisSereServFilter filter = new MOS.Filter.HisSereServFilter();
                filter.ORDER_FIELD = "SERVICE_NUM_ORDER";
                filter.ORDER_DIRECTION = "DESC";
                filter.SERVICE_REQ_ID = currentServiceReq.ID;
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).Get<List<HIS_SERE_SERV>>
                    (ApiConsumer.HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (apiResult != null && apiResult.Count > 0)
                {
                    listServiceADO = new List<ADO.ServiceADO>();
                    var listId = apiResult.Select(o => o.ID).ToList();
                    ProcessDicSereServExt(listId);

                    foreach (var item in apiResult)
                    {
                        ADO.ServiceADO ado = new ADO.ServiceADO(item);
                        var ext = dicSereServExt.ContainsKey(item.ID) ? dicSereServExt[item.ID] : null;
                        ado.MACHINE_ID = ChoseDataMachine(ado);
                        ado.SoPhieu = String.Format("{0}-{1}", ReduceForCode(item.TDL_SERVICE_REQ_CODE, SERVICE_REQ_CODE__MAX_LENGTH), ReduceForCode(item.TDL_SERVICE_CODE, SERVICE_CODE__MAX_LENGTH));
                        var service = lstService.FirstOrDefault(o => o.ID == item.SERVICE_ID);
                        if (ext != null)
                        {
                            ado.NUMBER_OF_FILM = ext.NUMBER_OF_FILM;
                            if (ext.MACHINE_ID.HasValue)
                            {
                                ado.MACHINE_ID = ext.MACHINE_ID.Value;
                            }
                            if (!String.IsNullOrWhiteSpace(ext.NOTE) || !String.IsNullOrWhiteSpace(ext.CONCLUDE) || ext.BEGIN_TIME != null)
                            {
                                ado.IsProcessed = true;
                            }
                        }
                        else
                        {
                            if (service != null && service.NUMBER_OF_FILM.HasValue)
                            {
                                ado.NUMBER_OF_FILM = (long)(Math.Round(service.NUMBER_OF_FILM.Value * item.AMOUNT, 0, MidpointRounding.AwayFromZero));
                            }
                        }

                        //có cấu hình bắt buộc chỉ định thuốc vật tư sẽ kiểm tra dịch vụ đó đã có thông tin film hay chưa (số lượng film)
                        //
                        if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ServiceExecuteCFG.LockExecuteCFG) == "1" && item.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA)
                        {
                            if (ext == null || !ext.NUMBER_OF_FILM.HasValue || ext.NUMBER_OF_FILM.Value <= 0)
                            {
                                ado.MustHavePressBeforeExecute = true;
                            }
                        }

                        listServiceADO.Add(ado);
                    }

                    CheckAllInOne.ReadOnly = apiResult.Count == 1 ? true : false;
                }

                gridViewSereServ.GridControl.DataSource = listServiceADO;

                Inventec.Common.Logging.LogSystem.Debug("3.2");
                gridViewSereServ.EndUpdate();
                Inventec.Common.Logging.LogSystem.Debug("3.3");
            }
            catch (Exception ex)
            {
                gridViewSereServ.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataCombo()
        {
            try
            {
                if (ConnectImageOption == "2")
                {
                    Gc_SendSancy.VisibleIndex = 1;
                }
                var workingRoomIds = WorkPlace.GetRoomIds();

                if (ListMachine != null && ListMachine.Count > 0)
                    ListMachine =
                        (from m in ListMachine
                         from n in workingRoomIds
                         where m.IS_ACTIVE == 1 && m.ROOM_IDS != null && ("," + m.ROOM_IDS + ",").Contains("," + n.ToString() + ",")
                         select m).Distinct().ToList();

                if (ListServiceMachine != null && ListServiceMachine.Count > 0)
                    ListServiceMachine = ListServiceMachine.Where(o => o.IS_ACTIVE == 1).ToList();

                ComboServiceMachine(repositoryItemMachineId);//máy
                InitComboSereServTemp(new List<HIS_SERE_SERV_TEMP>());
                ComboSizeFilm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataImageLocal()
        {
            try
            {
                //lấy ảnh từ chụp từ chức năng camera fill vào titleView
                this.listImage = new List<ADO.ImageADO>();
                List<Image> images = new List<Image>();

                if (GlobalVariables.dicImageCapture != null && GlobalVariables.dicImageCapture.ContainsKey(this.currentServiceReq.TDL_TREATMENT_CODE))
                {
                    images.AddRange(GlobalVariables.dicImageCapture[this.currentServiceReq.TDL_TREATMENT_CODE]);
                }

                if (images != null && images.Count > 0)
                {
                    images = images.OrderByDescending(o => o.Tag).ToList();
                    foreach (var item in images)
                    {
                        string result = null;
                        string text = item.Tag.ToString();
                        if (text != null && text.Length >= 9)
                            result = new StringBuilder().Append(text.Substring(0, 2)).Append(":").Append(text.Substring(2, 2)).Append(":").Append(text.Substring(4, 2)).Append(":").Append(text.Substring(6, 3)).ToString();

                        using (MemoryStream ms = new MemoryStream())
                        {
                            item.Save(ms, ImageFormat.Jpeg);

                            ADO.ImageADO image = new ADO.ImageADO();
                            image.FileName = result;
                            image.IsChecked = false;

                            image.streamImage = new MemoryStream();
                            ms.Position = 0;
                            ms.CopyTo(image.streamImage);
                            ms.Position = 0;
                            image.IMAGE_DISPLAY = Image.FromStream(ms);

                            listImage.Add(image);
                        }
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("listImage.Count", listImage.Count));
                ProcessLoadGridImage(listImage);

                string detail = "|";
                if (GlobalVariables.dicImageCapture != null && GlobalVariables.dicImageCapture.Count > 0)
                {
                    foreach (var dicImg in GlobalVariables.dicImageCapture)
                    {
                        detail += dicImg.Key + " - count = " + ((dicImg.Value != null && dicImg.Value.Count > 0) ? dicImg.Value.Count : 0) + "|";
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug("DelegateCaptureImage____" +
                                    Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => maxImage), maxImage)
                                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => detail), detail)
                                    + Inventec.Common.Logging.LogUtil.TraceData("GlobalVariables.listImage.Count", GlobalVariables.listImage.Count));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SereServClickRow(ADO.ServiceADO sereServ)
        {
            try
            {
                isPressButtonSave = false;
                //load tempalte lên wordEdit
                if (sereServ != null)
                {
                    this.sereServ = sereServ;
                    this.lblIntructionTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(this.sereServ.TDL_INTRUCTION_TIME);
                    if (ConnectImageOption == "1" && !String.IsNullOrEmpty(this.sereServ.TDL_PACS_TYPE_CODE))
                    {
                        btnCamera.Enabled = false;
                        ThreadLoadImageFromPacs();
                    }
                    else
                        btnCamera.Enabled = true;

                    txtSereServTempCode.Text = "";
                    cboSereServTemp.EditValue = null;
                    cboSereServTemp.Properties.Buttons[1].Visible = false;

                    Inventec.Common.Logging.LogSystem.Debug("3.2.1");

                    ProcessLoadSereServExt(sereServ, ref sereServExt);
                    ProcessLoadSereServExtDescriptionPrint(sereServExt);

                    Inventec.Common.Logging.LogSystem.Debug("3.2.2");

                    var result = lstService.FirstOrDefault(o => o.ID == sereServ.SERVICE_ID);
                    if (result != null && result.FILM_SIZE_ID > 0 && (sereServExt == null || sereServExt.FILM_SIZE_ID == null))
                    {
                        cboSizeOfFilm.EditValue = result.FILM_SIZE_ID;
                    }
                    else
                    {
                        cboSizeOfFilm.EditValue = sereServExt != null && sereServExt.FILM_SIZE_ID > 0 ? sereServExt.FILM_SIZE_ID : null;
                    }
                    if (sereServExt != null && sereServExt.NUMBER_OF_FILM.HasValue)
                    {
                        txtNumberOfFilm.Text = sereServExt.NUMBER_OF_FILM.ToString();
                    }
                    else if (sereServ.NUMBER_OF_FILM.HasValue)
                    {
                        txtNumberOfFilm.Text = sereServ.NUMBER_OF_FILM.ToString();
                    }
                    else
                    {
                        txtNumberOfFilm.Text = "";
                    }

                    if (sereServExt != null && sereServExt.ID > 0)
                    {
                        this.LblExecuteName.Text = string.Format("{0} ({1})", sereServExt.SUBCLINICAL_RESULT_USERNAME, sereServExt.SUBCLINICAL_RESULT_LOGINNAME);
                    }

                    if (GettxtDescription().Text == "")
                    {
                        ProcessLoadTemplate(sereServ);
                    }
                    else
                    {
                        var temp = listTemplate.Where(o => ConvertServiceStrToLong(o.SERVICE_IDS).Contains(sereServ.SERVICE_ID)).ToList();
                        var tempType = listTemplate.Where(o => o.SERVICE_TYPE_ID == sereServ.TDL_SERVICE_TYPE_ID ||
                            !o.SERVICE_TYPE_ID.HasValue).ToList();
                        if (temp != null && temp.Count > 0)
                            InitComboSereServTemp(temp);
                        else if (tempType != null && tempType.Count > 0)
                            InitComboSereServTemp(tempType);
                        else
                            InitComboSereServTemp(listTemplate);
                    }

                    //load thông tin kip
                    FillDataToGridEkip();
                    GetSereServPtttBySereServId();

                    UncheckImage();

                    if (sereServExt != null && dicSarPrint.ContainsKey(sereServExt.ID) && dicSarPrint[sereServExt.ID] != null && listTemplate != null)
                    {
                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dicSarPrint[sereServExt.ID]), dicSarPrint[sereServExt.ID]));
                        if (!String.IsNullOrWhiteSpace(dicSarPrint[sereServExt.ID].ADDITIONAL_INFO) && dicSarPrint[sereServExt.ID].ADDITIONAL_INFO.Contains("SERE_SERV_TEMP_CODE"))
                        {
                            string TEMP_CODE = dicSarPrint[sereServExt.ID].ADDITIONAL_INFO.Trim("SERE_SERV_TEMP_CODE:".ToCharArray());
                            var temp = listTemplate.FirstOrDefault(o => o.SERE_SERV_TEMP_CODE == TEMP_CODE);
                            if (temp != null)
                            {
                                txtSereServTempCode.Text = temp.SERE_SERV_TEMP_CODE;
                                cboSereServTemp.EditValue = temp.ID;
                                cboSereServTemp.Properties.Buttons[1].Visible = true;
                            }
                        }
                    }
                    Inventec.Common.Logging.LogSystem.Debug("3.2.3");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ComboServiceMachine(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit cbo)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MACHINE_CODE", "Mã máy", 150, 1));
                columnInfos.Add(new ColumnInfo("MACHINE_NAME", "Tên máy", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MACHINE_NAME", "ID", columnInfos, true, 250);
                ControlEditorLoader.Load(cbo, ListMachine, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ComboSizeFilm()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisFilmSizeFilter filter = new HisFilmSizeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_FILM_SIZE>>("api/HisFilmSize/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("FILM_SIZE_CODE", "Mã kích cỡ phim", 150, 1));
                columnInfos.Add(new ColumnInfo("FILM_SIZE_NAME", "Tên kích cỡ phim", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("FILM_SIZE_NAME", "ID", columnInfos, true, 350);
                ControlEditorLoader.Load(cboSizeOfFilm, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region ProcessLoad
        private void ProcessDicSereServExt(List<long> listId)
        {
            try
            {
                if (listId != null && listId.Count > 0)
                {
                    MOS.Filter.HisSereServExtFilter filter = new MOS.Filter.HisSereServExtFilter();
                    filter.SERE_SERV_IDs = listId;
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var result = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT>>(RequestUriStore.HIS_SERE_SERV_EXT_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (result != null && result.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            dicSereServExt[item.SERE_SERV_ID] = item;
                            if (!dicOldData.ContainsKey(item.ID))
                            {
                                dicOldData[item.ID] = item.DESCRIPTION;
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

        private void ProcessLoadListTemplate()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("1. Begin ProcessLoadListTemplate ");
                CommonParam param = new CommonParam();
                HisSereServTempFilter filter = new HisSereServTempFilter();
                filter.GENDER_ID__NULL_OR_EXACT = ServiceReqConstruct.TDL_PATIENT_GENDER_ID;
                filter.ROOM_ID__NULL_OR_EXACT = moduleData.RoomId;
                filter.IS_ACTIVE = 1;
                filter.ColumnParams = new List<string>();
                filter.ColumnParams.Add("ID");
                filter.ColumnParams.Add("SERE_SERV_TEMP_CODE");
                filter.ColumnParams.Add("SERE_SERV_TEMP_NAME");
                filter.ColumnParams.Add("CONCLUDE");
                filter.ColumnParams.Add("SERVICE_ID");
                filter.ColumnParams.Add("SERVICE_TYPE_ID");
                filter.ColumnParams.Add("GENDER_ID");
                filter.ColumnParams.Add("NOTE");
                filter.ColumnParams.Add("DESCRIPTION_TEXT");
                filter.ColumnParams.Add("SERVICE_IDS");
                filter.ColumnParams.Add("EMR_BUSINESS_CODES");
                filter.ColumnParams.Add("EMR_DOCUMENT_TYPE_CODE");
                filter.ColumnParams.Add("IS_AUTO_CHOOSE_BUSINESS");
                filter.ColumnParams.Add("EMR_DOCUMENT_GROUP_CODE");
                filter.ColumnParams.Add("ROOM_ID");
                filter.ColumnParams.Add("MODIFY_TIME");
                filter.ColumnParams.Add("EMR_COLUMN_MAPPING");
                listTemplate = new BackendAdapter(param).Get<List<HIS_SERE_SERV_TEMP>>("api/HisSereServTemp/GetDynamic", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                Inventec.Common.Logging.LogSystem.Info("1. End ProcessLoadListTemplate ");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessLoadSereServExt(HIS_SERE_SERV sereServ, ref HIS_SERE_SERV_EXT sereServExt)
        {
            try
            {
                if (dicSereServExt.ContainsKey(sereServ.ID))
                {
                    this.currentSereServExt = sereServExt = dicSereServExt[sereServ.ID];
                    txtConclude.Text = sereServExt.CONCLUDE;
                    txtNote.Text = sereServExt.NOTE;
                    if (string.IsNullOrEmpty(sereServExt.CONCLUDE) || string.IsNullOrEmpty(sereServExt.NOTE))
                    {
                        if (cboSereServTemp.EditValue != null)
                        {
                            var data = listTemplate.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboSereServTemp.EditValue ?? 0).ToString()));
                            if (data != null)
                            {
                                if (string.IsNullOrEmpty(sereServExt.CONCLUDE))
                                {
                                    txtConclude.Text = data.CONCLUDE;
                                }
                                if (string.IsNullOrEmpty(sereServExt.NOTE))
                                {
                                    txtNote.Text = data.NOTE;
                                }
                            }
                        }
                    }

                    if (sereServExt.BEGIN_TIME != null)
                    {
                        dtBeginTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(sereServExt.BEGIN_TIME ?? 0) ?? DateTime.Now;
                    }
                    else
                    {
                        if (Config.AppConfigKeys.StartTimeOption == "1")
                        {
                            dtBeginTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ServiceReqConstruct.INTRUCTION_TIME) ?? DateTime.Now;
                        }
                        else if (Config.AppConfigKeys.StartTimeOption == "2")
                        {
                            dtBeginTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ServiceReqConstruct.START_TIME ?? 0) ?? DateTime.Now;
                        }
                        else
                        {
                            dtBeginTime.DateTime = DateTime.Now;
                        }
                    }
                    //Khi mở form, nếu nút "Lưu" được enable (cho phép người dùng sửa kết quả), thì luôn mặc định "Thời gian kết thúc" theo thời gian hiện tại, cho phép người dùng sửa
                    if (sereServExt.END_TIME.HasValue)
                    {
                        dtEndTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(sereServExt.END_TIME ?? 0) ?? DateTime.Now;
                    }
                    else
                    {
                        dtEndTime.DateTime = currentTimer;
                    }
                    this.ActionType = GlobalVariables.ActionEdit;
                }
                else
                {
                    sereServExt = null;
                    ProcessLoadTemplate(sereServ);
                    if (cboSereServTemp.EditValue != null)
                    {
                        var data = listTemplate.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboSereServTemp.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtConclude.Text = data.CONCLUDE;
                            txtNote.Text = data.NOTE;
                        }
                    }

                    //txtConclude.Text = "";
                    if (dicSereServExt != null && dicSereServExt.Count > 0)
                    {
                        long endTime = dicSereServExt.Values.Max(o => o.END_TIME ?? 0);
                        long startTime = dicSereServExt.Values.Min(o => o.BEGIN_TIME ?? 99999999999999);

                        if (endTime > 0)
                        {
                            dtEndTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(endTime) ?? DateTime.Now;
                        }
                        else
                        {
                            dtEndTime.DateTime = currentTimer;
                        }
                        if (startTime > 0)
                            dtBeginTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(startTime) ?? DateTime.Now;
                        else
                            if (Config.AppConfigKeys.StartTimeOption == "1")
                        {
                            dtBeginTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ServiceReqConstruct.INTRUCTION_TIME) ?? DateTime.Now;
                        }
                        else if (Config.AppConfigKeys.StartTimeOption == "2")
                        {
                            dtBeginTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ServiceReqConstruct.START_TIME ?? 0) ?? DateTime.Now;
                        }
                        else
                        {
                            dtBeginTime.DateTime = DateTime.Now;
                        }
                    }
                    else
                    {
                        if (Config.AppConfigKeys.StartTimeOption == "1")
                        {
                            dtBeginTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ServiceReqConstruct.INTRUCTION_TIME) ?? DateTime.Now;
                        }
                        else if (Config.AppConfigKeys.StartTimeOption == "2")
                        {
                            dtBeginTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ServiceReqConstruct.START_TIME ?? 0) ?? DateTime.Now;
                        }
                        else
                        {
                            dtBeginTime.DateTime = DateTime.Now;
                        }
                        dtEndTime.DateTime = currentTimer;
                    }
                    //txtNote.Text = "";
                    ClearDocument();
                    this.ActionType = GlobalVariables.ActionAdd;
                }

                //TODO
                if (sereServ.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA)
                {
                    Gc_SereServTraKQ.Visible = true;
                    Gc_SereServTraKQ.VisibleIndex = 2;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        List<SereServFileADO> currentSsPdf = new List<SereServFileADO>();
        private bool ProcessLoadSereServFile(List<long> sereServId)
        {
            bool result = false;
            try
            {
                var currentSereServFiles = GetSereServFilesBySereServId(sereServId);
                if (currentSereServFiles != null && currentSereServFiles.Count > 0)
                {
                    var dataPdf = currentSereServFiles.Where(o => o.FILE_TYPE == 2).ToList();
                    if (dataPdf != null && dataPdf.Count > 0)
                    {
                        AutoMapper.Mapper.CreateMap<HIS_SERE_SERV_FILE, SereServFileADO>();
                        currentSsPdf = AutoMapper.Mapper.Map<List<SereServFileADO>>(dataPdf);
                    }
                    currentSereServFiles = currentSereServFiles.Where(o => o.FILE_TYPE == 1).ToList();
                    result = true;
                    this.listImage = new List<ADO.ImageADO>();
                    foreach (MOS.EFMODEL.DataModels.HIS_SERE_SERV_FILE item in currentSereServFiles)
                    {
                        System.IO.MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(item.URL);
                        if (stream != null && stream.Length > 0)
                        {
                            ADO.ImageADO tileNew = new ADO.ImageADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV_FILE>(tileNew, item);
                            tileNew.FileName = item.SERE_SERV_FILE_NAME + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                            tileNew.IsChecked = false;

                            tileNew.streamImage = new MemoryStream();
                            stream.Position = 0;
                            stream.CopyTo(tileNew.streamImage);
                            stream.Position = 0;
                            tileNew.IMAGE_DISPLAY = Image.FromStream(stream);
                            this.listImage.Add(tileNew);
                        }
                    }
                    ProcessLoadGridImage(this.listImage);
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<HIS_SERE_SERV_FILE> GetSereServFilesBySereServId(List<long> sereServId)
        {
            List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_FILE> result = null;
            try
            {
                if (sereServId != null && sereServId.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisSereServFileFilter filter = new MOS.Filter.HisSereServFileFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    filter.SERE_SERV_IDs = sereServId;
                    result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_FILE>>(RequestUriStore.HIS_SERE_SERV_FILE_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ProcessLoadSereServExtDescriptionPrint(HIS_SERE_SERV_EXT sereServExt)
        {
            try
            {
                if (sereServExt != null && sereServExt.ID > 0)
                {
                    if (!dicSarPrint.ContainsKey(sereServExt.ID) || dicSarPrint[sereServExt.ID] == null)
                    {
                        dicSarPrint[sereServExt.ID] = GetListPrintByDescriptionPrint(sereServExt);
                    }

                    //nếu có kết quả thì hiển thị ra, nếu check gộp thì sẽ không thay đổi nôj dung
                    if (dicSarPrint[sereServExt.ID] != null && dicSarPrint[sereServExt.ID].ID > 0)
                    {
                        ProcessSetRtfText(Utility.TextLibHelper.BytesToStringConverted(dicSarPrint[sereServExt.ID].CONTENT));
                        btnPrint.Enabled = true;
                        BtnEmr.Enabled = true;
                        this.positionProtect = "";
                        //TODO
                        if (!String.IsNullOrEmpty(sereServExt.DOC_PROTECTED_LOCATION))
                        {
                            positionProtect = sereServExt.DOC_PROTECTED_LOCATION;
                        }

                        ProcessWordProtected(true);
                        ProcessSelectDataForDescription(sereServExt.XML_DESCRIPTION_LOCATION);
                    }
                    else if (CheckAllInOne.Checked)
                    {
                        btnPrint.Enabled = true;
                        BtnEmr.Enabled = true;
                    }
                    else
                    {
                        ClearDocument();

                        btnPrint.Enabled = false;
                        BtnEmr.Enabled = false;
                    }

                    this.currentSarPrint = dicSarPrint[sereServExt.ID];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private SAR.EFMODEL.DataModels.SAR_PRINT GetListPrintByDescriptionPrint(HIS_SERE_SERV_EXT sereServExt)
        {
            SAR.EFMODEL.DataModels.SAR_PRINT result = null;
            try
            {
                List<long> printIds = GetListPrintIdBySereServ(sereServExt);
                if (printIds != null && printIds.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    SAR.Filter.SarPrintFilter filter = new SAR.Filter.SarPrintFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    filter.IDs = printIds;
                    filter.ORDER_DIRECTION = "DESC";
                    filter.ORDER_FIELD = "ID";
                    var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<SAR.EFMODEL.DataModels.SAR_PRINT>>(ApiConsumer.SarRequestUriStore.SAR_PRINT_GET, ApiConsumer.ApiConsumers.SarConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        result = apiResult.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private List<long> GetListPrintIdBySereServ(HIS_SERE_SERV_EXT item)
        {
            List<long> result = new List<long>();
            try
            {
                if (!String.IsNullOrEmpty(item.DESCRIPTION_SAR_PRINT_ID))
                {
                    var arrIds = item.DESCRIPTION_SAR_PRINT_ID.Split(',', ';');
                    if (arrIds != null && arrIds.Length > 0)
                    {
                        foreach (var id in arrIds)
                        {
                            long printId = Inventec.Common.TypeConvert.Parse.ToInt64(id);
                            if (printId > 0)
                            {
                                result.Add(printId);
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

        private List<long> ConvertServiceStrToLong(string SERVICE_IDS)
        {
            List<long> serviceIdTempList = new List<long>();
            try
            {
                if (String.IsNullOrEmpty(SERVICE_IDS))
                    return serviceIdTempList;

                List<string> serviceIdInTemp = new List<string>();

                serviceIdInTemp.AddRange(SERVICE_IDS.Split(',').ToList());

                foreach (var item in serviceIdInTemp)
                {
                    long output = 0;
                    long.TryParse(item, out output);
                    if (output > 0)
                    {
                        serviceIdTempList.Add(output);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return serviceIdTempList;
        }

        private void ProcessLoadTemplate(HIS_SERE_SERV data)
        {
            try
            {
                if (data != null && data.SERVICE_ID > 0 && listTemplate != null && listTemplate.Count > 0)
                {
                    var temp = listTemplate.Where(o => ConvertServiceStrToLong(o.SERVICE_IDS).Contains(data.SERVICE_ID)).ToList();

                    var tempType = listTemplate.Where(o => o.SERVICE_TYPE_ID == data.TDL_SERVICE_TYPE_ID ||
                            !o.SERVICE_TYPE_ID.HasValue).ToList();
                    if (temp != null && temp.Count > 0)
                    {
                        InitComboSereServTemp(temp);
                        LoadSereServTempCombo(temp.FirstOrDefault().SERE_SERV_TEMP_CODE);
                    }
                    else if (tempType != null && tempType.Count > 0)
                    {
                        InitComboSereServTemp(tempType);
                        if (tempType.Count == 1)
                        {
                            LoadSereServTempCombo(tempType.FirstOrDefault().SERE_SERV_TEMP_CODE);
                        }
                    }
                    else
                        InitComboSereServTemp(listTemplate);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboSereServTemp(List<HIS_SERE_SERV_TEMP> data)
        {
            try
            {
                cboSereServTemp.Properties.DataSource = data;
                cboSereServTemp.Properties.DisplayMember = "SERE_SERV_TEMP_NAME";
                cboSereServTemp.Properties.ValueMember = "ID";
                cboSereServTemp.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboSereServTemp.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboSereServTemp.Properties.ImmediatePopup = true;
                cboSereServTemp.ForceInitialize();
                cboSereServTemp.Properties.View.Columns.Clear();
                cboSereServTemp.Properties.PopupFormSize = new Size(400, 250);

                GridColumn aColumnCode = cboSereServTemp.Properties.View.Columns.AddField("SERE_SERV_TEMP_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 60;

                GridColumn aColumnName = cboSereServTemp.Properties.View.Columns.AddField("SERE_SERV_TEMP_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 340;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetSereServPtttBySereServId()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSereServPtttViewFilter hisSereServPtttFilter = new HisSereServPtttViewFilter();
                hisSereServPtttFilter.SERE_SERV_ID = this.sereServ.ID;
                var hisSereServPttts = new BackendAdapter(param).Get<List<HIS_SERE_SERV_PTTT>>("api/HisSereServPttt/Get", ApiConsumers.MosConsumer, hisSereServPtttFilter, param);
                if (hisSereServPttts != null && hisSereServPttts.Count > 0)
                {
                    this.dicSereServPttt[this.sereServ.ID] = hisSereServPttts.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataMachineCombo(ADO.ServiceADO data, GridLookUpEdit editor)
        {
            try
            {
                if (editor != null && ListMachine != null && ListServiceMachine != null && ListServiceMachine.Count > 0)
                {
                    var currentServiceMachine = ListServiceMachine.Where(o => o.SERVICE_ID == data.SERVICE_ID).Select(o => o.MACHINE_ID).ToList();
                    List<HisMachineCounterSDO> dataCombo = new List<HisMachineCounterSDO>();

                    if (((AppConfigKeys.IsPatientTypeOption == "1" && data.PATIENT_TYPE_ID == AppConfigKeys.PatientTypeId__BHYT) || AppConfigKeys.IsPatientTypeOption != "1")
                        && GlobalVariables.MachineCounterSdos != null && GlobalVariables.MachineCounterSdos.Count > 0)
                    {
                        dataCombo = GlobalVariables.MachineCounterSdos.Where(o => currentServiceMachine.Contains(o.ID) && ListMachine.Select(s => s.ID).Contains(o.ID)).ToList();
                    }
                    else if (currentServiceMachine != null && currentServiceMachine.Count > 0)
                    {
                        var machines = ListMachine.Where(o => currentServiceMachine.Contains(o.ID)).ToList();
                        foreach (var item in machines)
                        {
                            var sdo = new HisMachineCounterSDO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HisMachineCounterSDO>(sdo, item);
                            dataCombo.Add(sdo);
                        }
                    }

                    InitComboExecuteRoom(editor, dataCombo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboExecuteRoom(GridLookUpEdit editor, object dataCombo)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MACHINE_CODE", "Mã máy", 150, 1));
                columnInfos.Add(new ColumnInfo("MACHINE_NAME", "Tên máy", 250, 2));
                columnInfos.Add(new ColumnInfo("TOTAL_PROCESSED_SERVICE", "Đã xử lý", 100, 3));
                columnInfos.Add(new ColumnInfo("MAX_SERVICE_PER_DAY", "Tối đa", 100, 4));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MACHINE_NAME", "ID", columnInfos, true, 250);
                ControlEditorLoader.Load(editor, dataCombo, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private long? ChoseDataMachine(ADO.ServiceADO data)
        {
            long? result = null;
            try
            {
                var currentServiceMachine = ListServiceMachine.Where(o => o.SERVICE_ID == data.SERVICE_ID).Select(o => o.MACHINE_ID).ToList();
                List<HIS_MACHINE> dataCombo = (currentServiceMachine != null && currentServiceMachine.Count > 0) ? ListMachine.Where(o => currentServiceMachine.Contains(o.ID)).ToList() : null;
                if (dataCombo != null && dataCombo.Count > 0)
                {
                    //+ Nếu C ko có bản ghi nào, thì yêu cầu người dùng chọn máy từ danh sách A.
                    //+ Nếu C có bản ghi, thì mặc định chọn bản ghi đầu tiên của C, cho phép người dùng sửa
                    if (dataCombo.Count == 1)
                    {
                        result = dataCombo.First().ID;
                    }
                    else if (GlobalVariables.DicExecuteRoomMachine != null && GlobalVariables.DicExecuteRoomMachine.ContainsKey(moduleData.RoomId) && GlobalVariables.DicExecuteRoomMachine[moduleData.RoomId].Count > 0)
                    {
                        dataCombo = dataCombo.Where(o => GlobalVariables.DicExecuteRoomMachine[moduleData.RoomId].Contains(o.ID)).ToList();
                        result = dataCombo.First().ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        private void ProcessLoadGridImage(List<ADO.ImageADO> listImage)
        {
            try
            {
                cardControl.BeginUpdate();
                cardControl.DataSource = null;
                if (listImage != null && listImage.Count > 0)
                {
                    cardControl.DataSource = listImage;
                }
                cardControl.EndUpdate();
                lblNumberOfImageSelected.Text = (((listImage != null && listImage.Count > 0) ? listImage.Where(o => o.IsChecked).Count() : 0).ToString()) + ResourceMessage.TieuDeChonAnh;
            }
            catch (Exception ex)
            {
                cardControl.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        #endregion

        #region event
        private void cboSizeOfFilm_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboSizeOfFilm.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSizeOfFilm_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboSizeOfFilm.Properties.Buttons[1].Visible = cboSizeOfFilm.EditValue != null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSereServTempCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadSereServTempCombo(strValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadSereServTempCombo(string searchCode)
        {
            try
            {
                bool showCombo = true;
                if (!String.IsNullOrEmpty(searchCode) && this.listTemplate != null)
                {
                    //List<HIS_SERE_SERV_TEMP> listData = cboSereServTemp.Properties.DataSource as List<HIS_SERE_SERV_TEMP>;
                    var data = this.listTemplate.Where(o => o.SERE_SERV_TEMP_CODE.ToLower().Contains(searchCode.ToLower())).ToList();
                    var result = data != null ? (data.Count > 1 ? data.Where(o => o.SERE_SERV_TEMP_CODE.ToLower() == searchCode.ToLower()).ToList() : data) : null;
                    if (result != null && result.Count > 0)
                    {
                        showCombo = false;
                        cboSereServTemp.Properties.Buttons[1].Visible = true;
                        cboSereServTemp.EditValue = result.First().ID;
                        txtSereServTempCode.Text = result.First().SERE_SERV_TEMP_CODE;

                        Inventec.Common.Logging.LogSystem.Info("3.2.2.1");
                        ProcessChoiceSereServTempl(result.First());
                        Inventec.Common.Logging.LogSystem.Info("3.2.2.2");
                        ProcessFocusWord();
                    }
                }
                if (showCombo)
                {
                    cboSereServTemp.Properties.Buttons[1].Visible = false;
                    cboSereServTemp.EditValue = null;
                    cboSereServTemp.Focus();
                    cboSereServTemp.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSereServTemp_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    txtSereServTempCode.Text = "";
                    cboSereServTemp.EditValue = null;
                    ClearDocument();
                    cboSereServTemp.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSereServTemp_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboSereServTemp.EditValue != null)
                    {
                        var data = listTemplate.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboSereServTemp.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtSereServTempCode.Text = data.SERE_SERV_TEMP_CODE;
                            cboSereServTemp.Properties.Buttons[1].Visible = true;
                            ProcessChoiceSereServTempl(data);
                        }
                    }

                    ProcessFocusWord();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSereServTemp_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboSereServTemp.EditValue != null)
                    {
                        var data = listTemplate.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboSereServTemp.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtSereServTempCode.Text = data.SERE_SERV_TEMP_CODE;
                            cboSereServTemp.Properties.Buttons[1].Visible = true;
                            ProcessChoiceSereServTempl(data);
                        }
                    }

                    ProcessFocusWord();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cardView_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("cardView_Click");
                cardControl.BeginUpdate();
                var card = (ADO.ImageADO)cardView.GetFocusedRow();
                if (card != null)
                {
                    card.IsChecked = !card.IsChecked;
                }
                cardControl.EndUpdate();
            }
            catch (Exception ex)
            {
                cardControl.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void tileView1_ItemClick(object sender, DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("tileView1_ItemClick");
                e.Item.Checked = !e.Item.Checked;
                TileView tileView = sender as TileView;
                this.currentDataClick = (ADO.ImageADO)tileView.GetRow(e.Item.RowHandle);
                this.currentDataClick.IsChecked = e.Item.Checked;
                StartTimer(moduleData.ModuleLink, "timerDoubleClick");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void tileView1_ItemDoubleClick(object sender, DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("tileView1_ItemDoubleClick");
                StartTimer(moduleData.ModuleLink, "timerDoubleClick");
                this.currentDataClick = null;
                e.Item.Checked = !e.Item.Checked;

                // mở form xem ảnh
                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ServiceExecuteCFG.ImageViewOption) == "1")
                {
                    //check gộp sẽ truyền tất cả service_id gộp. ngược lại chỉ add 1 service_id của dịch vụ đang xử lý
                    List<long> serviceIds = new List<long>();
                    if (listServiceADOForAllInOne != null && listServiceADOForAllInOne.Count > 0)
                    {
                        serviceIds.AddRange(listServiceADOForAllInOne.Select(s => s.SERVICE_ID));
                    }
                    else if (sereServ != null)
                    {
                        serviceIds.Add(sereServ.SERVICE_ID);
                    }

                    var formView = new ViewImage.FormViewImageV2(this.moduleData, serviceIds, this.listImage, SaveImageData, lstService);
                    if (formView != null) formView.ShowDialog();
                }
                else
                {
                    var item = (ADO.ImageADO)tileView1.GetRow(e.Item.RowHandle);
                    Form formView = new ViewImage.FormViewImage(this.listImage, item);
                    if (formView != null) formView.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveImageData(List<ImageADO> obj)
        {
            try
            {
                if (obj != null)
                {
                    this.listImage = ProcessOrderImage(obj);
                    cardControl.RefreshDataSource();
                    lblNumberOfImageSelected.Text = (((listImage != null && listImage.Count > 0) ? listImage.Where(o => o.IsChecked).Count() : 0).ToString()) + ResourceMessage.TieuDeChonAnh;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void tileView1_ContextButtonClick(object sender, DevExpress.Utils.ContextItemClickEventArgs e)
        {
            try
            {
                if (e.Item.Name == "btnDelete")
                {
                    if (XtraMessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), ResourceMessage.ThongBao, MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        var dataItem = (DevExpress.XtraGrid.Views.Tile.TileViewItem)e.DataItem;
                        var item = (ADO.ImageADO)tileView1.GetRow(dataItem.RowHandle);
                        //nếu đã lưu thì gọi api xóa và check document
                        if (item.ID > 0)
                        {
                            CommonParam param = new CommonParam();
                            HIS_SERE_SERV_FILE data = new HIS_SERE_SERV_FILE();
                            data.ID = item.ID;
                            var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(RequestUriStore.HIS_SERE_SERV_FILE_DELETE, ApiConsumer.ApiConsumers.MosConsumer, param, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken);
                            //gọi api xóa thành công thì xóa ở danh sách và xóa document
                            if (apiResult)
                            {
                                tileView1.DeleteRow(dataItem.RowHandle);
                            }
                        }
                        else
                        {
                            tileView1.DeleteRow(dataItem.RowHandle);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void tileView1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Delete)
                {
                    TileView view = sender as TileView;
                    var checkedRows = view.GetCheckedRows();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => checkedRows), checkedRows));
                    if (checkedRows != null && checkedRows.Count() > 0)
                    {
                        var listImagedelete = this.listImage.Where(o => o.IsChecked == true).ToList();
                        string clientCode = currentServiceReq.TDL_TREATMENT_CODE;
                        List<Image> images = null;
                        if (GlobalVariables.dicImageCapture != null && GlobalVariables.dicImageCapture.ContainsKey(clientCode))
                        {
                            images = GlobalVariables.dicImageCapture[clientCode];
                            foreach (var item in listImagedelete)
                            {
                                try
                                {
                                    images.Remove(item.IMAGE_DISPLAY);
                                    GlobalVariables.listImage.Remove(item.IMAGE_DISPLAY);
                                }
                                catch (Exception exx)
                                {
                                    LogSystem.Warn(exx);
                                }
                            }
                            GlobalVariables.dicImageCapture[clientCode] = images;
                        }
                        this.listImage = this.listImage.Where(o => o.IsChecked == false).ToList();
                        this.listImage = ProcessOrderImage(this.listImage);
                        ProcessLoadGridImage(this.listImage);

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("listImage.Count", listImage != null ? listImage.Count : 0));

                        string detail = "|";
                        if (GlobalVariables.dicImageCapture != null && GlobalVariables.dicImageCapture.Count > 0)
                        {
                            foreach (var dicImg in GlobalVariables.dicImageCapture)
                            {
                                detail += dicImg.Key + " - count = " + ((dicImg.Value != null && dicImg.Value.Count > 0) ? dicImg.Value.Count : 0) + "|";
                            }
                        }

                        Inventec.Common.Logging.LogSystem.Debug("DelegateCaptureImage____" +
                                            Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => maxImage), maxImage)
                                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => detail), detail)
                                            + Inventec.Common.Logging.LogUtil.TraceData("GlobalVariables.listImage.Count", GlobalVariables.listImage.Count));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void tileView1_ItemRightClick(object sender, TileViewItemClickEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("tileView1_ItemRightClick");
                e.Item.Checked = !e.Item.Checked;
                this.currentDataClick = (ADO.ImageADO)tileView1.GetRow(e.Item.RowHandle);
                this.currentDataClick.IsChecked = e.Item.Checked;

                PopupMenu menu = new PopupMenu(this.barManager1);

                BarButtonItem itemCopy = new BarButtonItem(this.barManager1, ResourceMessage.CopyImage);
                itemCopy.Tag = RightButtonType.Copy;
                itemCopy.ItemClick += new ItemClickEventHandler(MouseRightClick);
                menu.AddItems(new BarItem[] { itemCopy });

                if (this.currentDataClick.IsChecked)
                {
                    BarButtonItem itemStt = new BarButtonItem(this.barManager1, ResourceMessage.ChangeStt);
                    itemStt.Tag = RightButtonType.ChangeStt;
                    itemStt.ItemClick += new ItemClickEventHandler(MouseRightClick);
                    menu.AddItems(new BarItem[] { itemStt });
                }

                menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timerDoubleClick_Tick()
        {
            try
            {
                StopTimer(moduleData.ModuleLink, "timerDoubleClick");
                if (this.currentDataClick != null)
                {
                    ProcessRefeshImageOrder(this.currentDataClick);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// - Khi người dùng nhập số thứ tự vào thì sẽ cho phép hình ảnh đó được chèn vào stt được nhập đó. Và sắp xếp lại số thứ tự:
        ///VD: i1 = A, i2 = B, i3 = C, i4 = D, i5 = E (i1,2,3,4,5: STT; A,B,C,D,E: Hình ảnh)
        ///TH1: Đổi B có STT = 4 ==> STT sẽ sắp xếp lại như sau: i1 = A, i2 = C, i3 = D, i4 = B, i5 = E (Các STT của C,D sẽ bị đẩy lên 1 đơn vị để B thay thế vào).
        ///TH2: Đổi D có STT = 2 ==> STT sẽ sắp xếp lại như sau: i1 = A, i2 = D, i3 = B, i4 = C, i5 = E (Các STT của B,C sẽ bị đẩy xuống 1 đơn vị để D chèn vào).
        /// </summary>
        /// <param name="sttNumber"></param>
        private void ProcessUpdateImageOrder(decimal sttNumber)
        {
            try
            {
                if (this.currentDataClick.STTImage == (int)sttNumber)
                    return;

                bool isChangePlus = this.currentDataClick.STTImage < (int)sttNumber ? true : false;

                if (this.listImage != null && this.listImage.Count > 0)
                {
                    this.listImage = ProcessOrderImage(this.listImage);

                    var imgDuplicate = this.listImage.Where(o => o.IsChecked && o.STTImage == (int)sttNumber).FirstOrDefault();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => imgDuplicate), imgDuplicate) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isChangePlus), isChangePlus));
                    int duplicateSttNumber = imgDuplicate != null ? (imgDuplicate.STTImage ?? 0) : 0;
                    int rawSttNumber = currentDataClick != null ? (currentDataClick.STTImage ?? 0) : 0;
                    int numC = 1;
                    foreach (var itemImg in this.listImage)
                    {
                        int maxStt = this.listImage.Where(o => o.IsChecked).Max(o => o.STTImage ?? 0);
                        if (itemImg.IsChecked)
                        {
                            if (isChangePlus)
                            {
                                if ((int)sttNumber > maxStt)
                                {
                                    if ((itemImg.STTImage ?? 0) > rawSttNumber)
                                    {
                                        itemImg.STTImage = itemImg.STTImage - 1;
                                    }
                                    else if ((itemImg.STTImage ?? 0) == rawSttNumber)
                                        itemImg.STTImage = maxStt;
                                }
                                else if (duplicateSttNumber > 0)
                                {
                                    if ((itemImg.STTImage ?? 0) == rawSttNumber)
                                    {
                                        itemImg.STTImage = duplicateSttNumber;
                                    }
                                    else if ((itemImg.STTImage ?? 0) > rawSttNumber && (itemImg.STTImage ?? 0) <= duplicateSttNumber)
                                    {
                                        itemImg.STTImage = itemImg.STTImage - 1;
                                    }
                                }
                            }
                            else
                            {
                                if (duplicateSttNumber > 0)
                                {
                                    if ((itemImg.STTImage ?? 0) == rawSttNumber)
                                    {
                                        itemImg.STTImage = duplicateSttNumber;
                                    }
                                    else if ((itemImg.STTImage ?? 0) < rawSttNumber && (itemImg.STTImage ?? 0) >= duplicateSttNumber)
                                    {
                                        itemImg.STTImage = itemImg.STTImage + 1;
                                    }
                                }
                            }

                            numC += 1;
                        }
                    }
                }

                cardControl.RefreshDataSource();
                lblNumberOfImageSelected.Text = (((listImage != null && listImage.Count > 0) ? listImage.Where(o => o.IsChecked).Count() : 0).ToString()) + ResourceMessage.TieuDeChonAnh;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessRefeshImageOrder(ADO.ImageADO currentData)
        {
            try
            {
                if (currentData.IsChecked)
                {
                    Inventec.Common.Logging.LogSystem.Debug("ProcessRefeshImageOrder.1");
                    List<int> listSTT = listImage != null ? listImage.Select(o => o.STTImage ?? 0).Distinct().ToList() : new List<int>();
                    listSTT = listSTT != null ? listSTT.OrderBy(o => o).ToList() : listSTT;
                    currentData.STTImage = 1;

                    if (listSTT != null && listSTT.Count() == 1)
                        currentData.STTImage = listSTT.Max() + 1;
                    else
                        for (int i = 0; i < listSTT.Count() - 1; i++)
                        {
                            if (listSTT[i] + 1 != listSTT[i + 1])
                            {
                                currentData.STTImage = listSTT[i] + 1;
                                break;
                            }
                            else
                                currentData.STTImage = listSTT.Max() + 1;
                        }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("ProcessRefeshImageOrder.2");
                    //bs chụp các ảnh a -> b -> c. stt sẽ tương ứng a=1, b=2, c=3. Khi bỏ chọn b thì làm lại stt như sau: a=1,c=2
                    currentData.STTImage = null;
                    if (listImage != null && listImage.Count > 0)
                    {
                        var listImageTemp = listImage.OrderByDescending(o => o.IsChecked).ThenBy(o => o.FileName).ToList();
                        int sttNew = 1;
                        foreach (var imgADO in listImageTemp)
                        {
                            if (imgADO.IsChecked)
                            {
                                imgADO.STTImage = sttNew;
                                sttNew += 1;
                            }
                        }
                    }
                }

                cardControl.RefreshDataSource();
                lblNumberOfImageSelected.Text = (((listImage != null && listImage.Count > 0) ? listImage.Where(o => o.IsChecked).Count() : 0).ToString()) + ResourceMessage.TieuDeChonAnh;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (CheckAllInOne.Checked && this.sereServ != null) InsertRow(this.sereServ);//cập nhật lại dữ liệu
                    var asereServ = (ADO.ServiceADO)gridViewSereServ.GetFocusedRow();
                    if (asereServ != null)
                    {
                        if (CheckAllInOne.Checked)
                            InsertRow(asereServ);
                        else
                            SereServClickRow(asereServ);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                if (CheckAllInOne.Checked && this.sereServ != null) InsertRow(this.sereServ);//cập nhật lại dữ liệu
                var asereServ = (ADO.ServiceADO)gridViewSereServ.GetRow(e.RowHandle);
                if (asereServ != null)
                {
                    if (CheckAllInOne.Checked)
                        InsertRow(asereServ);
                    else
                        SereServClickRow(asereServ);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                var data = view.GetFocusedRow() as ADO.ServiceADO;
                if (view.FocusedColumn.FieldName == "MACHINE_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (editor != null)
                    {
                        this.FillDataMachineCombo(data, editor);
                        if (editor.Name == repositoryItemMachineHideDelete.Name)
                        {
                            this.FillDataMachineCombo(data, repositoryItemMachineId.OwnerEdit);
                        }
                        else
                        {
                            this.FillDataMachineCombo(data, repositoryItemMachineHideDelete.OwnerEdit);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.Column.FieldName == "MACHINE_ID")
                {
                    long machineId = Inventec.Common.TypeConvert.Parse.ToInt64((view.GetRowCellValue(e.RowHandle, view.Columns["MACHINE_ID"]) ?? "").ToString());
                    if (machineId == 0)
                    {
                        e.RepositoryItem = repositoryItemMachineHideDelete;
                    }
                    else
                    {
                        e.RepositoryItem = repositoryItemMachineId;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var aSereServ = (ADO.ServiceADO)gridViewSereServ.GetFocusedRow();
                if (aSereServ != null)
                {
                    if (aSereServ == null) return;
                    if (ConnectImageOption != "1" || String.IsNullOrEmpty(aSereServ.TDL_PACS_TYPE_CODE)) return;
                    if (String.IsNullOrEmpty(ConfigSystems.URI_API_PACS)) return;

                    var images = LoadImageFromPacsService(aSereServ.SoPhieu);

                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.moduleData);

                    var currDirect = Directory.GetCurrentDirectory();
                    var DirectoryFolder = String.Format("{0}\\{1}", currDirect, tempFolder);
                    if (Directory.Exists(DirectoryFolder))
                    {
                        Directory.Delete(DirectoryFolder, true);
                    }
                    Directory.CreateDirectory(DirectoryFolder);

                    if (images != null && images.Count > 0)
                    {

                        foreach (var item in images)
                        {
                            string fileCreate = "";

                            if (ConnectPacsByFss == "1")
                            {
                                if (String.IsNullOrEmpty(ConfigSystems.URI_API_FSS_FOR_PACS)) return;
                                string direct = item.ImageDirectory.Split(fss, StringSplitOptions.None).LastOrDefault();
                                string fullDirect = direct + "\\" + item.ImageDcmFileName;

                                var jpg = Inventec.Fss.Client.FileDownload.GetFile(fullDirect, ConfigSystems.URI_API_FSS_FOR_PACS);

                                fileCreate = String.Format("{0}\\{1}", DirectoryFolder, Inventec.Common.DateTime.Get.Now() + "_" + item.ImageDcmFileName);
                                var fileStream = File.Create(fileCreate);
                                jpg.Seek(0, SeekOrigin.Begin);
                                jpg.CopyTo(fileStream);
                                fileStream.Close();
                            }
                            else
                            {
                                var s = ConfigSystems.URI_API_PACS.Split('/');
                                var ip = s[2].Split(':').FirstOrDefault();

                                string direct = item.ImageDirectory.Split(fss, StringSplitOptions.None).FirstOrDefault();
                                fileCreate = String.Format("{0}\\{1}", item.ImageDirectory.Replace(direct + fss[0], "\\\\" + ip + "\\"), item.ImageDcmFileName);
                            }

                            listArgs.Add(fileCreate);
                        }
                    }

                    PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisPacsOne", moduleData.RoomId, moduleData.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSereServ_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                GridHitInfo hi = e.HitInfo;
                if (hi.InRowCell)
                {
                    int rowHandle = gridViewSereServ.GetVisibleRowHandle(hi.RowHandle);

                    this.sereServ = (ADO.ServiceADO)gridViewSereServ.GetRow(rowHandle);

                    gridViewSereServ.OptionsSelection.EnableAppearanceFocusedCell = true;
                    gridViewSereServ.OptionsSelection.EnableAppearanceFocusedRow = true;
                    if (barManager1 == null)
                    {
                        barManager1 = new BarManager();
                        barManager1.Form = this;
                    }

                    PopupMenuProcessor popupMenuProcessor = new PopupMenuProcessor(sereServ, barManager1, ServiceMouseRightClick, (RefeshReference)BtnRefreshs);
                    popupMenuProcessor.InitMenu();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ServiceMouseRightClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem && this.sereServ != null)
                {
                    var bbtnItem = sender as BarButtonItem;
                    PopupMenuProcessor.ItemType type = (PopupMenuProcessor.ItemType)(e.Item.Tag);
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumerStore.SarConsumer, UriBaseStore.URI_API_SAR, LanguageManager.GetLanguage(), Inventec.Desktop.Common.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                    switch (type)
                    {
                        case PopupMenuProcessor.ItemType.EkipExecute:
                            EkipClsInfoClick();
                            break;
                        case PopupMenuProcessor.ItemType.SereServSuin:
                            SereServSuinClick();
                            break;
                        case PopupMenuProcessor.ItemType.PhieuKeKhaiThuocVatTuTieuHao:
                            onClickPhieuKeKhaiThuocVatTu(null, null);
                            break;
                        case PopupMenuProcessor.ItemType.PhieuPTTT:
                            onClickPhieuPTTT(null, null);
                            break;
                        case PopupMenuProcessor.ItemType.KeThuocVatTuTieuHao:
                            onClickKeThuocVatTuTieuHao(null, null);
                            break;
                        case PopupMenuProcessor.ItemType.KhaiBaoThuocVTTH:
                            onClickKhaiBaoThuocVTTH(null, null);
                            break;
                        case PopupMenuProcessor.ItemType.HuyPhieuThuocVatTuTieuHaoDaKe:
                            onClickHuyPhieuThuocVatTuTieuHaoDaKe(null, null);
                            break;
                        case PopupMenuProcessor.ItemType.XacNhanKhongThucHien:
                            onClickXacNhanKhongThucHien();
                            break;
                        case PopupMenuProcessor.ItemType.HuyXacNhanKhongThucHien:
                            onClickHuyXacNhanKhongThucHien();
                            break;
                        case PopupMenuProcessor.ItemType.HuyXuLy:
                            onClickHuyXuLy();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickHuyXuLy()
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                if (sereServ == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("sereServ is null");
                    return;
                }
                MOS.SDO.HisSereServDeleteConfirmNoExcuteSDO sdo = new HisSereServDeleteConfirmNoExcuteSDO();
                sdo.SereServId = sereServ.ID;
                sdo.WorkingRoomId = moduleData.RoomId;
                var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT>("api/HisSereServExt/Delete", ApiConsumer.ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
                if (result != null)
                {
                    if (dicSereServExt.ContainsKey(result.SERE_SERV_ID))
                    {
                        dicSereServExt[result.SERE_SERV_ID] = result;
                    }
                    if (listServiceADO != null && listServiceADO.Count > 0)
                    {
                        foreach (var item in listServiceADO)
                        {
                            if (item.ID == this.sereServ.ID)
                            {
                                var ext = dicSereServExt.ContainsKey(item.ID) ? dicSereServExt[item.ID] : null;
                                if (!String.IsNullOrWhiteSpace(ext.NOTE) || !String.IsNullOrWhiteSpace(ext.CONCLUDE) || ext.BEGIN_TIME != null)
                                {
                                    item.IsProcessed = true;
                                }
                                else
                                {
                                    item.IsProcessed = false;
                                }
                                break;
                            }
                        }
                    }
                    gridControlSereServ.DataSource = null;
                    gridControlSereServ.DataSource = listServiceADO;
                    success = true;
                }

                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickHuyXacNhanKhongThucHien()
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                if (sereServ == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("sereServ is null");
                    return;
                }
                MOS.SDO.HisSereServDeleteConfirmNoExcuteSDO sdo = new HisSereServDeleteConfirmNoExcuteSDO();
                sdo.SereServId = sereServ.ID;
                sdo.WorkingRoomId = moduleData.RoomId;
                var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_SERE_SERV>("api/HisSereServ/DeleteConfirmNoExcute", ApiConsumer.ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
                if (result != null)
                {
                    if (listServiceADO != null && listServiceADO.Count > 0)
                    {
                        foreach (var item in listServiceADO)
                        {
                            if (item.ID == this.sereServ.ID)
                            {
                                item.IS_CONFIRM_NO_EXCUTE = result.IS_CONFIRM_NO_EXCUTE;
                                item.CONFIRM_NO_EXCUTE_REASON = result.CONFIRM_NO_EXCUTE_REASON;
                                break;
                            }
                        }
                    }
                    gridControlSereServ.DataSource = null;
                    gridControlSereServ.DataSource = listServiceADO;
                    success = true;
                }

                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickXacNhanKhongThucHien()
        {
            try
            {
                frmMessage frm = new frmMessage(sereServ, GetResult, moduleData);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetResult(HIS_SERE_SERV obj)
        {
            try
            {
                if (obj == null)
                    return;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => obj), obj));
                if (listServiceADO != null && listServiceADO.Count > 0)
                {
                    foreach (var item in listServiceADO)
                    {
                        if (item.ID == this.sereServ.ID)
                        {
                            item.IS_CONFIRM_NO_EXCUTE = obj.IS_CONFIRM_NO_EXCUTE;
                            item.CONFIRM_NO_EXCUTE_REASON = obj.CONFIRM_NO_EXCUTE_REASON;
                            break;
                        }
                    }
                }
                gridControlSereServ.DataSource = null;
                gridControlSereServ.DataSource = listServiceADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private async void SereServSuinClick()
        {
            try
            {
                await this.LoadSuim();
                frmSereServSuin frm = new frmSereServSuin(listResult =>
                {
                    dicSereServSuin[sereServ.ID] = listResult;
                }, dicSereServSuin[sereServ.ID]);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void EkipClsInfoClick()
        {
            try
            {
                if (this.sereServ != null)
                {
                    if (this.sereServExt == null)
                    {
                        this.sereServExt = new HIS_SERE_SERV_EXT();
                        this.sereServExt.SERE_SERV_ID = sereServ.ID;
                    }

                    List<HisEkipUserADO> ekipUserAdos = gridControlEkip.DataSource as List<HisEkipUserADO>;

                    HIS_SERE_SERV_PTTT sereServPttt = new HIS_SERE_SERV_PTTT();
                    if (this.dicSereServPttt != null && this.dicSereServPttt.ContainsKey(this.sereServ.ID))
                    {
                        sereServPttt = this.dicSereServPttt[this.sereServ.ID];
                    }
                    HIS_SERE_SERV_EXT sereServExtForLoad = new HIS_SERE_SERV_EXT();
                    if (this.dicSereServExt != null && this.dicSereServExt.ContainsKey(this.sereServ.ID))
                    {
                        sereServExtForLoad = this.dicSereServExt[this.sereServ.ID];
                    }

                    if (keySubclinicalInfoOption == "1")
                    {
                        frmClsInfo frmClsInfo = new frmClsInfo(this.moduleData, this.sereServ, SaveSSPtttInfoClick, sereServPttt, sereServExtForLoad, this.ServiceReqConstruct, (RefeshReference)ClickServiceAction, lstService, lstExecuteRole);
                        frmClsInfo.ShowDialog();
                    }
                    else
                    {
                        var service = lstService.FirstOrDefault(o => o.ID == this.sereServ.SERVICE_ID);
                        if (service != null && service.PTTT_GROUP_ID.HasValue)
                        {
                            frmClsInfo frmClsInfo = new frmClsInfo(this.moduleData, this.sereServ, SaveSSPtttInfoClick, sereServPttt, sereServExtForLoad, this.ServiceReqConstruct, (RefeshReference)ClickServiceAction, lstService, lstExecuteRole);
                            frmClsInfo.ShowDialog();
                        }
                        else if (service != null)
                        {
                            WaitingManager.Hide();
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.DichVuChuaCoNhomPttt, service.SERVICE_NAME), ResourceMessage.ThongBao, System.Windows.Forms.MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK)
                                return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ClickServiceAction()
        {
            try
            {
                SereServClickRow(sereServ);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveSSPtttInfoClick(HIS_SERE_SERV_PTTT sereservPttt, HIS_SERE_SERV_EXT sereServExt, ServiceADO serviceADO)
        {
            try
            {
                var datas = gridControlSereServ.DataSource as List<ADO.ServiceADO>;
                if (serviceADO != null)
                {
                    foreach (var item in datas)
                    {
                        if (item.ID == serviceADO.ID)
                            item.lstEkipUser = serviceADO.lstEkipUser;
                    }
                    this.sereServ.lstEkipUser = serviceADO.lstEkipUser;
                }
                if (serviceADO.lstEkipUser != null && serviceADO.lstEkipUser.Count > 0)
                {
                    var ekipUserAdos = new List<HisEkipUserADO>();
                    foreach (var item in serviceADO.lstEkipUser)
                    {
                        HisEkipUserADO ekipAdo = new HisEkipUserADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HisEkipUserADO>(ekipAdo, item);
                        if (ekipUserAdos.Count == 0)
                        {
                            ekipAdo.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                        }
                        else
                        {
                            ekipAdo.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                        }
                        ekipUserAdos.Add(ekipAdo);
                    }
                    this.dicEkipUser[this.sereServ.ID] = ekipUserAdos;
                }

                this.dicSereServPttt[this.sereServ.ID] = sereservPttt;
                //this.dicEkipUser[this.sereServ.ID] = ekipUserADOs;

                //if (ekipUserADOs != null && ekipUserADOs.Count > 0)
                //{
                //    gridControlEkip.DataSource = null;
                //    gridControlEkip.DataSource = ekipUserADOs;
                //    gridControlEkip.RefreshDataSource();
                //}

                this.sereServExt = sereServExt;
                Inventec.Common.Logging.LogSystem.Debug("SaveSSPtttInfoClick sereservPttt MANNER " + Inventec.Common.Logging.LogUtil.TraceData("", sereservPttt.MANNER));
                Inventec.Common.Logging.LogSystem.Debug("SaveSSPtttInfoClick sereServExt INSTRUCTION_NOTE " + Inventec.Common.Logging.LogUtil.TraceData("", sereServExt.INSTRUCTION_NOTE));
                Inventec.Common.Logging.LogSystem.Debug("SaveSSPtttInfoClick sereServExt DESCRIPTION " + Inventec.Common.Logging.LogUtil.TraceData("", sereServExt.DESCRIPTION));
                if (this.sereServExt != null)
                {
                    if (this.dicSereServExt != null && this.dicSereServExt.ContainsKey(this.sereServExt.SERE_SERV_ID))
                    {
                        this.dicSereServExt[this.sereServExt.SERE_SERV_ID] = this.sereServExt;
                    }
                    else if (this.dicSereServExt != null)
                    {
                        this.dicSereServExt.Add(this.sereServExt.SERE_SERV_ID, this.sereServExt);
                    }

                    if (this.sereServExt.MACHINE_ID != this.sereServ.MACHINE_ID)
                    {
                        var serviceADOEdit = listServiceADO != null ? listServiceADO.Where(o => o.ID == this.sereServExt.SERE_SERV_ID).FirstOrDefault() : null;
                        if (serviceADOEdit != null)
                        {
                            serviceADOEdit.MACHINE_ID = this.sereServExt.MACHINE_ID;
                        }
                        gridControlSereServ.RefreshDataSource();
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.sereServExt), this.sereServExt));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BtnRefreshs()
        {
            try
            {
                //TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckAllInOne_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (CheckAllInOne.Checked)
                {
                    this.mainSereServ = this.sereServ;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemMachineId_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    var edit = sender as GridLookUpEdit;
                    edit.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtConclude_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                string currentValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                var count = Inventec.Common.String.CountVi.Count(currentValue);
                if (count != null && count > 1000)
                {
                    txtConclude.ErrorText = ResourceMessage.ChuoiKyTuQuaDai;
                    txtConclude.Focus();
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNote_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                string currentValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                var count = Inventec.Common.String.CountVi.Count(currentValue);
                if (count != null && count > 500)
                {
                    txtNote.ErrorText = ResourceMessage.ChuoiKyTuQuaDai;
                    txtNote.Focus();
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNumberOfFilm_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                string currentValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                if (!String.IsNullOrEmpty(currentValue))
                {
                    string newValue = "";
                    foreach (char item in currentValue)
                    {
                        if (char.IsDigit(item))
                        {
                            newValue += item;
                        }
                    }

                    if (newValue != "")
                        txtNumberOfFilm.Text = newValue;
                    else
                        txtNumberOfFilm.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNumberOfFilm_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == '\r')
                {
                    txtConclude.Focus();
                    txtConclude.SelectAll();
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

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
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

        private void BtnChooseImage_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "JPEG files (*.jpg,*.bmp,*.png)|*.jpg;*.jpeg;*.jpe;*.jfif;*.bmp;*.png";
                    ofd.FilterIndex = 0;
                    ofd.Multiselect = true;

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        if (listImage == null) listImage = new List<ADO.ImageADO>();

                        foreach (var file in ofd.FileNames)
                        {
                            Image img = Image.FromFile(file);
                            string fileName = file.Split('\\').LastOrDefault();

                            if (listImage.Exists(o => o.FileName == fileName)) continue;

                            ADO.ImageADO image = new ADO.ImageADO();
                            image.CREATOR = Loginname;
                            image.FileName = fileName;
                            image.IsChecked = false;
                            image.IMAGE_DISPLAY = img;
                            byte[] buff = System.IO.File.ReadAllBytes(file);
                            image.streamImage = new System.IO.MemoryStream(buff);

                            listImage.Add(image);
                        }
                    }
                    ProcessLoadGridImage(this.listImage);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnDeleteImage_Click(object sender, EventArgs e)
        {
            try
            {
                listImage = new List<ADO.ImageADO>();
                ProcessLoadGridImage(this.listImage);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnChangeImage_Click(object sender, EventArgs e)
        {
            try
            {
                if (!BtnChangeImage.Enabled) return;

                List<ADO.ImageADO> images = listImage != null ? listImage.Where(o => o.IsChecked == true).ToList() : null;
                if (images != null && images.Count > 0)
                {
                    if (images.Count == 1)
                    {
                        var img = images.First();
                        Control edit = null;
                        foreach (var item in this.panelDescription.Controls)
                        {
                            if (item is UcWord)
                            {
                                var control = (UcWord)item;
                                edit = control.txtDescription;
                                break;
                            }
                            else if (item is UcWordFull)
                            {
                                var control = (UcWordFull)item;
                                edit = control.txtDescription;
                                break;
                            }
                            else if (item is UcWords.UcTelerik)
                            {
                                var control = (UcWords.UcTelerik)item;
                                edit = control.radRichTextEditor1;
                                break;
                            }
                            else if (item is UcWords.UcTelerikFullWord)
                            {
                                var control = (UcWords.UcTelerikFullWord)item;
                                edit = control.radRichTextEditor1;
                                break;
                            }
                        }

                        ProcessChangeImage(edit, img);

                        cardControl.RefreshDataSource();
                        lblNumberOfImageSelected.Text = (((listImage != null && listImage.Count > 0) ? listImage.Where(o => o.IsChecked).Count() : 0).ToString()) + ResourceMessage.TieuDeChonAnh;
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.BanChiDuocChonMotAnh);
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.BanChuaChonAnh);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region click
        private void repositoryItemButtonServiceReqMaty_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (repositoryItemButtonServiceReqMaty.ReadOnly) return;

                List<object> listArgs = new List<object>();
                listArgs.Add(this.sereServ.ID);

                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisServiceReqMaty", this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            try
            {
                bool checkPacs = true;
                checkPacs = checkPacs && ConnectImageOption == "1";
                checkPacs = checkPacs && !String.IsNullOrEmpty(this.sereServ.TDL_PACS_TYPE_CODE);
                checkPacs = checkPacs && PACS.PacsCFG.PACS_ADDRESS != null && PACS.PacsCFG.PACS_ADDRESS.Count > 0;
                V_HIS_ROOM room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == moduleData.RoomId) ?? new V_HIS_ROOM();
                checkPacs = checkPacs && PACS.PacsCFG.PACS_ADDRESS.Exists(o => o.RoomCode == room.ROOM_CODE);
                if (checkPacs)
                    LoadImageFromPacs();
                else
                    LoadDataImageLocal();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAssignService_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnAssignService.Enabled) return;

                var patientType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == TreatmentWithPatientTypeAlter.PATIENT_TYPE_CODE);
                if (patientType != null)
                {
                    if (new HIS.Desktop.Plugins.Library.AlertHospitalFeeNotBHYT.AlertHospitalFeeNotBHYTManager().Run(TreatmentWithPatientTypeAlter.ID, patientType.ID, moduleData.RoomId))
                    {
                        //Mở module chỉ định
                        HIS.Desktop.ADO.AssignServiceADO ado = new HIS.Desktop.ADO.AssignServiceADO(currentServiceReq.TREATMENT_ID, currentServiceReq.INTRUCTION_TIME, currentServiceReq.ID);
                        List<object> listArgs = new List<object>();
                        listArgs.Add(ado);

                        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.AssignService", this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);

                        //chỉ định xong sẽ load lại dữ liệu
                        FillDataToGrid();
                    }
                }
                else
                {
                    //Mở module chỉ định
                    HIS.Desktop.ADO.AssignServiceADO ado = new HIS.Desktop.ADO.AssignServiceADO(currentServiceReq.TREATMENT_ID, currentServiceReq.INTRUCTION_TIME, currentServiceReq.ID);
                    List<object> listArgs = new List<object>();
                    listArgs.Add(ado);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.AssignService", this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);

                    //chỉ định xong sẽ load lại dữ liệu
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAssignPrescription_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnAssignPrescription.Enabled) return;

                List<object> listArgs = new List<object>();

                V_HIS_SERE_SERV sereServInput = new V_HIS_SERE_SERV();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV>(sereServInput, sereServ);
                AssignPrescriptionADO assignPrescription = new AssignPrescriptionADO(currentServiceReq.TREATMENT_ID, currentServiceReq.INTRUCTION_TIME, this.currentServiceReq.ID, sereServInput);

                assignPrescription.PatientDob = currentServiceReq.TDL_PATIENT_DOB;
                assignPrescription.PatientName = currentServiceReq.TDL_PATIENT_NAME;
                assignPrescription.GenderName = currentServiceReq.TDL_PATIENT_GENDER_NAME;
                assignPrescription.TreatmentCode = currentServiceReq.TDL_TREATMENT_CODE;
                assignPrescription.TreatmentId = currentServiceReq.TREATMENT_ID;

                if (TreatmentWithPatientTypeAlter.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU
                    || TreatmentWithPatientTypeAlter.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU
                    || (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ServiceExecuteCFG.PrescriptionTypeOption) == "1" && TreatmentWithPatientTypeAlter.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY))
                {
                    assignPrescription.IsExecutePTTT = true;
                    assignPrescription.IsAutoCheckExpend = true;
                }

                listArgs.Add(assignPrescription);

                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.AssignPrescriptionPK", this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnTrackingList_Click(object sender, EventArgs e)
        {
            try
            {
                if (sereServ != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(sereServ.TDL_TREATMENT_ID);
                    listArgs.Add(moduleData);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisTrackingList", this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSereServTempList_Click(object sender, EventArgs e)
        {
            try
            {
                List<object> listArgs = new List<object>();
                if (sereServ.SERVICE_ID > 0)
                {
                    listArgs.Add(sereServ.SERVICE_ID);
                }

                listArgs.Add(moduleData);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.SereServTemplate", this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnContentLibrary_Click(object sender, EventArgs e)
        {
            try
            {
                List<object> listArgs = new List<object>();
                if (sereServ.SERVICE_ID > 0)
                {
                    listArgs.Add(sereServ.SERVICE_ID);
                }

                listArgs.Add(moduleData);
                listArgs.Add((HIS.Desktop.Common.DelegateDataTextLib)ProcessDataTextLib);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.TextLibrary", this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataTextLib(HIS_TEXT_LIB textLib)
        {
            try
            {
                var alreadyExist = this.listHisTextLib.FirstOrDefault(o => o.ID == textLib.ID);
                if (alreadyExist != null)
                {
                    this.listHisTextLib.Remove(alreadyExist);
                }

                this.listHisTextLib.Add(textLib);
                InitBarContentLibrary(this.listHisTextLib);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                HIS_SERE_SERV sereServInput = new HIS_SERE_SERV();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV>(sereServInput, sereServ);
                HIS_SERE_SERV_EXT sereServExtInput = new HIS_SERE_SERV_EXT();
                if (dicSereServExt.ContainsKey(sereServInput.ID))
                {
                    sereServExtInput = dicSereServExt[sereServInput.ID];
                }
                if (sereServ.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA)
                {
                    PopupMenu menu = new PopupMenu(barManager1);
                    menu.ItemLinks.Clear();

                    FormOtherProcessor FormOtherProcessor = new FormOtherProcessor(sereServInput, sereServExtInput, RefeshReferenceFormOther);
                    var pmenus = FormOtherProcessor.GetBarButtonItem(barManager1);
                    List<BarItem> bItems = new List<BarItem>();
                    BarButtonItem itemPrint = new BarButtonItem(barManager1, "In");
                    itemPrint.ItemClick += new ItemClickEventHandler(this.PrintResult__ItemClick);
                    bItems.Add(itemPrint);

                    if (pmenus != null && pmenus.Count > 0)
                    {
                        foreach (var item in pmenus)
                        {
                            bItems.Add((BarItem)item);
                        }
                    }

                    menu.AddItems(bItems.ToArray());
                    menu.ShowPopup(Cursor.Position);
                }
                else
                {
                    PrintResult(false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintResult__ItemClick(object sender, ItemClickEventArgs e)
        {
            PrintResult(false);
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnFinish.Enabled) return;
                bool success = false;
                CommonParam param = new CommonParam();
                if (currentServiceReq == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("currentServiceReq is null");
                    return;
                }

                if (listServiceADO != null && listServiceADO.Count > 0)
                {
                    bool successFull = true;
                    foreach (var item in listServiceADO)
                    {
                        //tất cả các dịch vụ (mà không bị tick "ko thực hiện") đã được nhập thông tin mô tả (DESCRIPTION_SAR_PRINT_ID trong his_sere_serv_ext khác null)
                        if (item.IS_NO_EXECUTE != 1 && (!dicSereServExt.ContainsKey(item.ID) || String.IsNullOrWhiteSpace(dicSereServExt[item.ID].DESCRIPTION_SAR_PRINT_ID)))
                        {
                            successFull = false;
                            break;
                        }
                    }
                    if (!successFull)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ChuaXuLyHetDichVu);
                        txtServiceReqCode.Focus();
                        txtServiceReqCode.SelectAll();
                        return;
                    }

                    currentServiceReq.START_TIME = dicSereServExt.ToList().Min(o => o.Value.BEGIN_TIME);
                    currentServiceReq.FINISH_TIME = dicSereServExt.ToList().Max(o => o.Value.END_TIME);
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("currentServiceReq___:", currentServiceReq));

                var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>("api/HisServiceReq/FinishWithTime", ApiConsumer.ApiConsumers.MosConsumer, currentServiceReq, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (result != null)
                {
                    success = true;
                    if (this.RefreshData != null)
                    {
                        this.RefreshData(result);
                    }
                    btnFinish.Enabled = false;
                    btnSave.Enabled = false;
                    //btnSaveNClose.Enabled = false;
                    //BtnSaveNPrint.Enabled = false;
                    //btnPrint.Enabled = false;

                    txtServiceReqCode.Focus();
                    txtServiceReqCode.SelectAll();
                }

                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
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
                if (!btnSave.Enabled) return;
                btnSave.Focus();
                if (currentServiceReq == null || currentServiceReq.ID == 0)
                {
                    MessageManager.Show("Chưa chọn hồ sơ nào");
                    return;
                }
                if (HIS.Desktop.Plugins.ServiceExecute.Config.AppConfigKeys.IsRequiredConclude)
                {
                    ValidateRequireMemoEdit(txtConclude);
                }

                if (CheckEkip())
                {
                    return;
                }

                var lstEkipUser = ProcessEkipUser(new HIS_SERE_SERV());
                if (AppConfigKeys.IsSampleInfoOption == "1")
                {
                    var datas = gridControlSereServ.DataSource as List<ADO.ServiceADO>;
                    if (datas != null && datas.Count > 0 && datas.FirstOrDefault(o => o.PATIENT_TYPE_ID == AppConfigKeys.PatientTypeId__BHYT) != null
                        && (lstEkipUser == null || lstEkipUser.Count == 0))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Bắt buộc phải có thông tin kíp thực hiện",
                          ResourceMessage.ThongBao,
                          MessageBoxButtons.OK);
                        return;
                    }
                }
                else if (AppConfigKeys.IsSampleInfoOption == "2")
                {
                    if (lstEkipUser == null || lstEkipUser.Count == 0)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Bắt buộc phải có thông tin kíp thực hiện",
                           ResourceMessage.ThongBao,
                           MessageBoxButtons.OK);
                        return;
                    }
                }
                if (lstEkipUser != null && lstEkipUser.Count > 0)
                {
                    var groupExecuteRole = from p in lstEkipUser
                                           group p by new
                                           {
                                               p.EXECUTE_ROLE_ID
                                           } into g
                                           select new { Key = g.Key, CareDetail = g.ToList() };
                    var executeRole = BackendDataWorker.Get<HIS_EXECUTE_ROLE>();
                    if (groupExecuteRole != null && groupExecuteRole.Count() > 0)
                    {
                        List<string> roleListName = new List<string>();
                        foreach (var item in groupExecuteRole)
                        {
                            var checkRole = executeRole.FirstOrDefault(o => o.ID == item.CareDetail.First().EXECUTE_ROLE_ID);
                            if (item.CareDetail.Count > 1 && checkRole != null && checkRole.IS_SINGLE_IN_EKIP == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                            {
                                roleListName.Add(checkRole.EXECUTE_ROLE_NAME);
                            }
                        }
                        if (roleListName != null && roleListName.Count > 0)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Không cho phép nhập nhiều hơn 1 tài khoản đối với vai trò {0}.", string.Join(", ", roleListName)),
                           ResourceMessage.ThongBao,
                           MessageBoxButtons.OK);
                            return;
                        }
                    }
                }

                if (this.sereServ != null && this.sereServ.MACHINE_ID == null && keySubclinicalMachineOption != null)
                {
                    if (keySubclinicalMachineOption == "1")//cảnh báo
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.BanCoMuonTiepTucKhong, string.Format(ResourceMessage.DichVuChuaCoMay, sereServ.TDL_SERVICE_NAME)),
                        ResourceMessage.ThongBao,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
                    }
                    else if (keySubclinicalMachineOption == "2")//chặn
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.DichVuChuaCoMay, sereServ.TDL_SERVICE_NAME),
                           ResourceMessage.ThongBao,
                           MessageBoxButtons.OK);
                        return;
                    }
                    else if (keySubclinicalMachineOption == "3" && sereServ.PATIENT_TYPE_ID == AppConfigKeys.PatientTypeId__BHYT)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Dịch vụ {0} chưa có thông tin máy cận lâm sàng. Bạn có muốn tiếp tục không?", sereServ.TDL_SERVICE_NAME),
                          ResourceMessage.ThongBao,
                          MessageBoxButtons.YesNo) == DialogResult.No)
                            return;
                    }
                    else if (keySubclinicalMachineOption == "4" && sereServ.PATIENT_TYPE_ID == AppConfigKeys.PatientTypeId__BHYT)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Dịch vụ {0} chưa có thông tin máy cận lâm sàng.", sereServ.TDL_SERVICE_NAME),
                          ResourceMessage.ThongBao,
                          MessageBoxButtons.OK);
                        return;
                    }
                }
                //
                if (Config.AppConfigKeys.StartTimeMustBeGreaterThanInstructionTime == "1" || Config.AppConfigKeys.StartTimeMustBeGreaterThanInstructionTime == "2")
                {
                    var beginTime = dtBeginTime.EditValue != null ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtBeginTime.DateTime) : null;
                    var endTime = dtEndTime.EditValue != null ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtEndTime.DateTime) : null;
                    if (beginTime < currentServiceReq.INTRUCTION_TIME || endTime < currentServiceReq.INTRUCTION_TIME)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ThoiGianBatDauThoiGianKetThucKhongDuocNhoHonThoiGianYLenh,
                           ResourceMessage.ThongBao,
                           MessageBoxButtons.OK);
                        return;
                    }
                }
                var checkService = lstService.FirstOrDefault(o => o.ID == sereServ.SERVICE_ID);
                if (checkService != null)
                {
                    if (checkService.MAX_TOTAL_PROCESS_TIME != null && checkService.MAX_TOTAL_PROCESS_TIME > 0 && (string.IsNullOrEmpty(checkService.TOTAL_TIME_EXCEPT_PATY_IDS) || !("," + checkService.TOTAL_TIME_EXCEPT_PATY_IDS + ",").Contains("," + sereServ.PATIENT_TYPE_ID.ToString() + ",")))
                    {
                        TimeSpan diff = (TimeSpan)(dtEndTime.DateTime - Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Int64.Parse(ServiceReqConstruct.INTRUCTION_TIME.ToString().Substring(0, ServiceReqConstruct.INTRUCTION_TIME.ToString().Length - 2) + "00")));
                        if ((int)diff.TotalMinutes > checkService.MAX_TOTAL_PROCESS_TIME)
                        {
                            if (Config.AppConfigKeys.IsProcessTimeMustBeGreaterThanTotalProcessTime == "1")
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Không cho phép trả kết quả dịch vụ {0} sau {1} phút tính từ thời điểm ra y lệnh {2}", sereServ.TDL_SERVICE_NAME, checkService.MAX_TOTAL_PROCESS_TIME, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(ServiceReqConstruct.INTRUCTION_TIME)),
                           ResourceMessage.ThongBao,
                           MessageBoxButtons.OK);
                                return;
                            }
                            else if (Config.AppConfigKeys.IsProcessTimeMustBeGreaterThanTotalProcessTime == "2")
                            {
                                if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Trả kết quả dịch vụ {0} vượt quá {1} phút tính từ thời điểm ra y lệnh {2}.Bạn có muốn tiếp tục không?", sereServ.TDL_SERVICE_NAME, checkService.MAX_TOTAL_PROCESS_TIME, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(ServiceReqConstruct.INTRUCTION_TIME)),
                           ResourceMessage.ThongBao,
                           MessageBoxButtons.YesNo) == DialogResult.No)
                                    return;
                            }
                        }
                    }
                }

                List<string> lstLoginValid = new List<string>();
                List<MOS.EFMODEL.DataModels.HIS_EKIP_USER> ekipUsers = new List<MOS.EFMODEL.DataModels.HIS_EKIP_USER>();
                var dataGrid = gridViewEkip.DataSource as List<HisEkipUserADO>;
                if (dataGrid != null && dataGrid.Count() > 0)
                    foreach (var item in dataGrid)
                    {
                        MOS.EFMODEL.DataModels.HIS_EKIP_USER ekipUser = new HIS_EKIP_USER();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EKIP_USER>(ekipUser, item);
                        if (ekipUser != null && ekipUser.EXECUTE_ROLE_ID != 0)
                            ekipUsers.Add(ekipUser);
                    }

                if (HIS.Desktop.Plugins.ServiceExecute.Config.AppConfigKeys.IsAssignServiceSimulTaneityOption)
                {
                    LogSystem.Info("lấy dữ liệu gọi api------------------------------------------------------------------------");
                    HisSereServCheckExecuteTimesSDO inputSDO = new HisSereServCheckExecuteTimesSDO();
                    var login = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();//currentServiceReq.EXECUTE_LOGINNAME;
                    long beginTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtBeginTime.DateTime) ?? 0;
                    long endTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtEndTime.DateTime) ?? 0;
                    inputSDO.ExecuteTime = new ExecuteTime
                    {
                        BeginTime = beginTime,
                        EndTime = endTime
                    };
                    inputSDO.TreatmentId = currentServiceReq.TREATMENT_ID;
                    List<string> dsLogin = new List<string> { login };
                    if (ekipUsers != null && ekipUsers.Count > 0)
                    {
                        

                        LogSystem.Info("có ekip------------------------------------------------------------------------");
                        List<string> lstLogin = ekipUsers.Select(o => o.LOGINNAME).Distinct().ToList();
                        foreach (string acc in lstLogin)
                        {
                            if (acc != null)
                            {
                                lstLoginValid.Add(acc);
                            }

                        }
                        if (lstLoginValid.Count == 0)
                        {
                            lstLoginValid.Add(login);
                        }
                        inputSDO.Loginnames = lstLoginValid;
                    }
                    else
                    {
                        LogSystem.Info("ko có ekip------------------------------------------------------------------------");
                        //var USER = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        //string EXECUTE_LOGINNAME = ServiceReqConstruct.EXECUTE_LOGINNAME;

                        HIS_EXECUTE_ROLE hIS_EXECUTE_ROLE = new HIS_EXECUTE_ROLE();
                        MOS.EFMODEL.DataModels.HIS_EKIP_USER ekipUser = new HIS_EKIP_USER();
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData($"ServiceReqConstruct", ServiceReqConstruct.EXECUTE_LOGINNAME));
                        if (ServiceReqConstruct.EXECUTE_LOGINNAME != null)
                        {
                            ekipUser.LOGINNAME = ServiceReqConstruct.EXECUTE_LOGINNAME;
                        }
                        else
                        {
                            ekipUser.LOGINNAME = login;
                        }
                            
                        //List<MOS.EFMODEL.DataModels.HIS_EKIP_USER> ekipUserVirtual = new List<HIS_EKIP_USER>();
                        lstExecuteRole = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().Where(o => o.ALLOW_SIMULTANEITY != 1).ToList();
                        if (lstExecuteRole != null)
                        {
                            long lstExecuteRoleId = lstExecuteRole.Select(o => o.ID).FirstOrDefault();
                            ekipUser.EXECUTE_ROLE_ID = lstExecuteRoleId;
                        }
                        ekipUsers.Add(ekipUser);      

                        if (ekipUsers != null)
                        {
                            List<string> lstLogin = ekipUsers.Select(o => o.LOGINNAME).Distinct().ToList();
                            foreach (string acc in lstLogin)
                            {
                                if (acc != null)
                                {
                                    lstLoginValid.Add(acc);
                                }

                            }
                            if (lstLoginValid.Count == 0)
                            {
                                lstLoginValid.Add(login);
                            }   
                            inputSDO.Loginnames = lstLoginValid;
                            
                        }
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData($"ekipUserVirtual", ekipUsers));
                    }
                    
                    string message = "";
                    CommonParam paramCheckEx = new CommonParam();
                    bool suscess = new BackendAdapter(paramCheckEx).Post<bool>("api/HisSereServ/CheckExecuteTimes", ApiConsumers.MosConsumer, inputSDO, paramCheckEx);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData($"/api/HisSereServ/CheckExecuteTimes", inputSDO));
                    if (suscess == true)
                    {
                                         
                    }
                    else
                    {
                        message = string.Format("{0} Bạn có muốn tiếp tục?", paramCheckEx.GetMessage());
                        if (XtraMessageBox.Show(message, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                            return;
                    }

                }

                if (HIS.Desktop.Plugins.ServiceExecute.Config.AppConfigKeys.IsCheckSimulTaneityOption)
                {
                    HisSurgServiceReqUpdateListSDO InputSDO = new HisSurgServiceReqUpdateListSDO();
                    List<SurgUpdateSDO> surgUpdateSDOs = new List<SurgUpdateSDO>();
                    SurgUpdateSDO surgUpdate = new SurgUpdateSDO();
                    surgUpdate.SereServId = sereServ.ID;
                    if (ekipUsers.Count > 0 && ekipUsers != null)
                    {
                        surgUpdate.EkipUsers = ekipUsers;
                    }
                    else
                    {
                        surgUpdate.EkipUsers = null;
                        // lấy kíp theo tài khoản đăng nhập (bỏ do pmem chạy chậm)
                        //var resuilt = BackendDataWorker.Get<HIS_EKIP_USER>().Where(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()).FirstOrDefault();
                        //if (resuilt != null)
                        //{
                        //    List<HIS_EKIP_USER> userList = new List<HIS_EKIP_USER> { resuilt };
                        //    surgUpdate.EkipUsers = userList;
                        //}
                        //else
                        //{
                        //    surgUpdate.EkipUsers = null;
                        //}
                    }

                    if (sereServExt != null)
                    {
                        surgUpdate.SereServExt = sereServExt;
                    }
                    else
                    {
                        HIS_SERE_SERV_EXT dataExt = new HIS_SERE_SERV_EXT();
                        dataExt.BEGIN_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtBeginTime.DateTime);
                        dataExt.END_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtEndTime.DateTime);
                        dataExt.ID = 0;
                        surgUpdate.SereServExt = dataExt;
                    }
                    if (dicSereServPttt.ContainsKey(sereServ.ID))
                    {
                        surgUpdate.SereServPttt = dicSereServPttt[sereServ.ID];
                    }
                    surgUpdateSDOs.Add(surgUpdate);

                    InputSDO.SurgUpdateSDOs = surgUpdateSDOs;
                    //InputSDO.IsFinished = currentServiceReq.FINISH_TIME != null ? true : false;
                    if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_SERVICE_REQ.ALLOW_FINISH_WHEN_ACCOUNT_IS_DOCTOR") == "1" && BackendDataWorker.Get<HIS_EMPLOYEE>().Where(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()).FirstOrDefault().IS_DOCTOR != 1)
                    {
                        InputSDO.IsFinished = false;
                    }
                    else
                    {
                        if (dtEndTime.EditValue != null && ChkAutoFinish.Checked)
                        {
                            InputSDO.IsFinished = true;
                        }
                    }
                    string message = "";
                    CommonParam paramCheckSurg = new CommonParam();
                    Inventec.Common.Logging.LogSystem.Debug("____________SERE_SERV_ID: " + sereServ.ID);
                    Inventec.Common.Logging.LogSystem.Debug("____________LOGINAME:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ekipUsers), ekipUsers));
                    Inventec.Common.Logging.LogSystem.Debug("____________BEGIN_TIME: " + surgUpdate.SereServExt.BEGIN_TIME);
                    Inventec.Common.Logging.LogSystem.Debug("____________END_TIME: " + surgUpdate.SereServExt.END_TIME);

                    bool suscess = new BackendAdapter(paramCheckSurg).Post<bool>("api/HisServiceReq/CheckSurgSimultaneily", ApiConsumers.MosConsumer, InputSDO, paramCheckSurg);
                    if (suscess == true)
                    {

                    }
                    else
                    {
                        message = string.Format("{0} Bạn có muốn tiếp tục?", paramCheckSurg.GetMessage());
                        if (XtraMessageBox.Show(message, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                            return;
                    }
                }

                if (CheckAllInOne.Checked)
                {
                    InsertRow(this.sereServ);//cập nhật lại dữ liệu
                    SaveAllProcess(chkPrint.Checked, chkClose.Checked, chkForPreview.Checked);
                }
                else
                    SaveProcessor(chkPrint.Checked, chkClose.Checked, chkForPreview.Checked);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnTuTruc_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnTuTruc.Enabled) return;

                List<object> listArgs = new List<object>();
                V_HIS_SERE_SERV sereServInput = new V_HIS_SERE_SERV();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV>(sereServInput, sereServ);
                HIS.Desktop.ADO.AssignPrescriptionADO assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionADO(currentServiceReq.TREATMENT_ID, currentServiceReq.INTRUCTION_TIME, this.currentServiceReq.ID, sereServInput);
                assignServiceADO.IsCabinet = true;
                assignServiceADO.PatientDob = currentServiceReq.TDL_PATIENT_DOB;
                assignServiceADO.PatientName = currentServiceReq.TDL_PATIENT_NAME;
                assignServiceADO.GenderName = currentServiceReq.TDL_PATIENT_GENDER_NAME;
                assignServiceADO.TreatmentCode = currentServiceReq.TDL_TREATMENT_CODE;
                assignServiceADO.TreatmentId = currentServiceReq.TREATMENT_ID;

                assignServiceADO.IsAutoCheckExpend = true;

                listArgs.Add(assignServiceADO);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.AssignPrescriptionCLS", this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCamera_Click(object sender, EventArgs e)
        {
            try
            {
                EnableControlCamera(true);
                StartCamera();// nambg
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void _dlgRefreshData()
        {
            try
            {
                btnLoadImage_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<FileSDO> ProcessImageList(List<ADO.ImageADO> imageList)
        {
            List<FileSDO> result = null;
            try
            {
                if (imageList != null && imageList.Count > 0)
                {
                    result = new List<FileSDO>();
                    foreach (var item in imageList)
                    {
                        FileSDO file = new FileSDO();
                        file.FileName = item.FileName + ".jpg";
                        file.Content = converterImageToByte(item.streamImage);
                        file.BodyPartId = item.BODY_PART_ID;
                        file.Caption = item.CAPTION;
                        file.NumOrder = item.STTImage;
                        file.FileType = 1;
                        result.Add(file);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        private byte[] converterImageToByte(Stream stream)
        {
            byte[] xByte = null;
            try
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    stream.Position = 0;
                    stream.CopyTo(memory);
                    xByte = memory.ToArray();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return xByte;
        }

        private void SaveProcessor(bool printNow, bool isClose, bool isPrintPreview = false)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                isPressButtonSave = true;
                if (!dxValidationProvider1.Validate())
                {
                    xtraTabControl1.SelectedTabPage = xtraTabPageConclude;
                    return;
                }

                if (sereServ == null) return;

                if (String.IsNullOrWhiteSpace(this.SelectedFolderForSaveImage) && chkSaveImageToFile.Checked)
                {
                    if (XtraMessageBox.Show(ResourceMessage.BanChuaChonDuongDanLuuAnh, ResourceMessage.ThongBao, MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        SelectFolder();
                    }
                    else
                    {
                        chkSaveImageToFile.Checked = false;
                    }
                }

                if (!CheckMachine(sereServ))
                {
                    return;
                }

                Inventec.Desktop.Common.Message.WaitingManager.Show();
                CheckSereServExt(new List<long> { sereServ.ID });
                SaveAllImage();

                this.sereServExt = dicSereServExt.ContainsKey(sereServ.ID) ? dicSereServExt[sereServ.ID] : new HIS_SERE_SERV_EXT() { SERE_SERV_ID = sereServ.ID };
                if (!this.sereServExt.SUBCLINICAL_PRES_ID.HasValue && chkKeTieuHao.Checked == true)
                {
                    long idReturn = Library.MediStockExpend.MediStockExpendProcessor.GetMediStock(moduleData.RoomId, false) ?? 0;
                    Inventec.Common.Logging.LogSystem.Debug("idReturn___________" + idReturn);

                    if (idReturn <= 0 && DevExpress.XtraEditors.XtraMessageBox.Show("Không có thông tin kho thuốc/vật tư tiêu hao. Bạn có muốn tiếp tục lưu thông tin xử lý không?", "Thông báo",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        return;

                    CommonParam paramSereServ = new CommonParam();
                    ExpendPresSDO sdo = new ExpendPresSDO();
                    sdo.MediStockId = idReturn;
                    sdo.SereServId = sereServ.ID;
                    sdo.RequestRoomId = moduleData.RoomId;
                    sdo.ServiceReqId = this.ServiceReqConstruct.ID;
                    SubclinicalPresResultSDO dataSubclinicalPresResultSDO = new Inventec.Common.Adapter.BackendAdapter(paramSereServ).Post<SubclinicalPresResultSDO>("api/HisServiceReq/ExpPresCreateByConfig", ApiConsumer.ApiConsumers.MosConsumer, sdo, paramSereServ);
                    Inventec.Common.Logging.LogSystem.Debug("DL TRA VE" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataSubclinicalPresResultSDO), dataSubclinicalPresResultSDO));
                    if (dataSubclinicalPresResultSDO != null && dataSubclinicalPresResultSDO.SereServExt != null && dataSubclinicalPresResultSDO.SereServExt.ID > 0)
                    {
                        this.sereServExt = dataSubclinicalPresResultSDO.SereServExt;
                    }
                    else if (dataSubclinicalPresResultSDO == null && DevExpress.XtraEditors.XtraMessageBox.Show(paramSereServ.GetMessage() + "Phần mềm sẽ không tự động \"Kê phiếu thuốc/vật tư tiêu hao\" Bạn có muốn tiếp tục lưu thông tin xử lý không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        return;

                }
                #region Save
                this.sereServExt.NOTE = txtNote.Text.Trim();
                this.sereServExt.CONCLUDE = txtConclude.Text.Trim();
                if (!String.IsNullOrEmpty(txtNumberOfFilm.Text.Trim()))
                {
                    this.sereServExt.NUMBER_OF_FILM = long.Parse(txtNumberOfFilm.Text);
                }
                else
                {
                    this.sereServExt.NUMBER_OF_FILM = null;
                }
                this.sereServExt.FILM_SIZE_ID = cboSizeOfFilm.EditValue != null ? (long?)cboSizeOfFilm.EditValue : null;
                ProcessDescriptionContent();

                List<FileHolder> listFileHolder = new List<FileHolder>();

                //đã gán và kiểm tra theo datasource
                this.sereServExt.MACHINE_ID = sereServ.MACHINE_ID;
                var machine = ListMachine.FirstOrDefault(o => o.ID == sereServ.MACHINE_ID);
                if (machine != null)
                {
                    this.sereServExt.MACHINE_CODE = machine.MACHINE_CODE;
                    this.sereServExt.MACHINE_ID = machine.ID;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("MACHINE_ID: " + sereServ.MACHINE_ID);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListMachine), ListMachine));
                }

                if (dtBeginTime.EditValue != null)
                    this.sereServExt.BEGIN_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtBeginTime.DateTime);
                else
                    this.sereServExt.BEGIN_TIME = null;

                if (dtEndTime.EditValue != null)
                    this.sereServExt.END_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtEndTime.DateTime);
                else
                    this.sereServExt.END_TIME = null;

                string xmlDescriptionLocation = "";
                string description = "";
                string descriptionInFrmClsInfo = "";
                bool isChanged = CheckChanged_SereServExtDescription(this.sereServExt.ID, this.sereServExt.DESCRIPTION);
                Inventec.Common.Logging.LogSystem.Debug("SaveProcessor_CheckChanged_SereServExtDescription: " + isChanged.ToString());
                if (isChanged)
                    descriptionInFrmClsInfo = this.sereServExt.DESCRIPTION ?? "";
                this.GetRangeAndData(ref xmlDescriptionLocation, ref description);

                this.sereServExt.XML_DESCRIPTION_LOCATION = xmlDescriptionLocation;
                description += " " + descriptionInFrmClsInfo;
                int index = 0;
                if (description.Length > 1000)
                {
                    index = description.Length - 1000;
                }
                this.sereServExt.DESCRIPTION = Inventec.Common.String.CountVi.SubStringVi(description, index, 3000);

                HisSereServExtSDO data = new HisSereServExtSDO();
                data.HisSereServExt = this.sereServExt;
                data.HisEkipUsers = ProcessEkipUser(sereServ);
                if (dicSereServSuin != null && dicSereServSuin.Count > 0 && dicSereServSuin.ContainsKey(sereServ.ID) && dicSereServSuin[sereServ.ID].Count > 0)
                {
                    AutoMapper.Mapper.CreateMap<V_HIS_SERE_SERV_SUIN, HIS_SERE_SERV_SUIN>();
                    data.HisSereServSuins = AutoMapper.Mapper.Map<List<HIS_SERE_SERV_SUIN>>(dicSereServSuin[sereServ.ID]);
                }
                ProcessSereServPtttInfo(ref data, sereServ, currentServiceReq);

                if (this.listImage != null && this.listImage.Count > 0 && chkAttachImage.Checked)
                {
                    data.Files = ProcessImageList(this.listImage.Where(o => o.IsChecked).ToList());
                }
                if (this.currentSsPdf != null && this.currentSsPdf.Count > 0)
                {
                    if (data.Files == null)
                        data.Files = new List<FileSDO>();
                    data.Files.AddRange(ProcessPdfList(currentSsPdf));
                }

                if (!IsCheckDataSereServExt(data))
                    return;
                if (ProcessSereServExt__DescriptionPrint(param, sereServ))
                {
                    Inventec.Desktop.Common.Message.WaitingManager.Hide();
                    Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                    return;
                }


                Inventec.Common.Logging.LogSystem.Debug("INPUT DATA: ___ " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                MOS.SDO.HisSereServExtWithFileSDO apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post
                    <MOS.SDO.HisSereServExtWithFileSDO>
                    (this.sereServExt.ID == 0 ?
                    RequestUriStore.HIS_SERE_SERV_EXT_CREATE_SDO :
                    RequestUriStore.HIS_SERE_SERV_EXT_UPDATE_SDO,
                    ApiConsumer.ApiConsumers.MosConsumer, param, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken);
                #endregion
                //ẩn trước khi hiển thị thông báo 
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, apiResult != null);
                #endregion

                if (apiResult != null)
                {
                    LoadSuim(true);
                    this.sereServExt = apiResult.SereServExt;
                    if (dicSereServExt.ContainsKey(apiResult.SereServExt.SERE_SERV_ID))
                    {
                        this.sereServ.NUMBER_OF_FILM = apiResult.SereServExt.NUMBER_OF_FILM;
                        dicSereServExt[apiResult.SereServExt.SERE_SERV_ID] = apiResult.SereServExt;
                        if (dicSarPrint.ContainsKey(apiResult.SereServExt.ID))
                        {
                            dicSarPrint[apiResult.SereServExt.ID] = GetListPrintByDescriptionPrint(apiResult.SereServExt);
                        }
                    }
                    else
                    {
                        dicSereServExt.Add(apiResult.SereServExt.SERE_SERV_ID, apiResult.SereServExt);
                        dicSarPrint[apiResult.SereServExt.ID] = GetListPrintByDescriptionPrint(apiResult.SereServExt);
                    }
                    this.currentServiceReq.EXECUTE_LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    this.currentServiceReq.EXECUTE_USERNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                    this.sereServExt.MODIFY_TIME = Inventec.Common.DateTime.Get.Now();
                    if (dicSereServExt.ContainsKey(apiResult.SereServExt.SERE_SERV_ID))
                        dicSereServExt[apiResult.SereServExt.SERE_SERV_ID].MODIFY_TIME = Inventec.Common.DateTime.Get.Now();

                    if (apiResult.SereServPttt != null && dicSereServPttt.ContainsKey(apiResult.SereServPttt.SERE_SERV_ID))
                    {
                        dicSereServPttt[apiResult.SereServPttt.SERE_SERV_ID] = apiResult.SereServPttt;
                    }

                    ProcessPatientInfo();

                    success = true;
                    if (listServiceADO != null && listServiceADO.Count > 0)
                    {
                        foreach (var item in listServiceADO)
                        {
                            if (item.ID == this.sereServ.ID)
                            {
                                var ext = dicSereServExt.ContainsKey(item.ID) ? dicSereServExt[item.ID] : null;
                                if (!String.IsNullOrWhiteSpace(ext.NOTE) || !String.IsNullOrWhiteSpace(ext.CONCLUDE) || ext.BEGIN_TIME != null)
                                {
                                    item.IsProcessed = true;
                                }
                                if (!item.EKIP_ID.HasValue)
                                {
                                    item.EKIP_ID = LoadEkipId(item.ID);
                                }
                                break;
                            }
                        }
                    }
                    gridControlSereServ.DataSource = null;
                    gridControlSereServ.DataSource = listServiceADO;
                    btnPrint.Enabled = true;
                    BtnEmr.Enabled = true;
                    ProcessLoadSereServFile(listServiceADO.Select(s => s.ID).Distinct().ToList());
                    //ẩn trước khi lưu đóng tránh bị dừng pm
                    Inventec.Desktop.Common.Message.WaitingManager.Hide();
                    //lưu và ký và đóng
                    if (chkSign.Checked)
                    {
                        BtnEmr_Click(null, null);
                    }

                    if (printNow && !chkSign.Checked)
                    {
                        PrintResult(printNow && !isPrintPreview);
                    }
                    else if (!printNow && !chkSign.Checked && isClose)
                    {
                        PrintResult(printNow && !isPrintPreview);
                    }
                    if (isPrintPreview && !chkSign.Checked)
                    {
                        PreviewResult(isPrintPreview);
                    }
                }

                if (apiResult != null)
                {
                    if (isClose || ChkAutoFinish.Checked)
                    {
                        if (WarningConfig == true)
                        {
                            XtraMessageBox.Show("Bạn không là Bác sỹ nên không được phép kết thúc", "Thông báo");
                        }
                        else
                        {
                            btnFinish_Click(null, null);//chỉ kết thúc khi tất cả đã thực hiện
                        }
                    }

                    if (isClose)
                    {
                        TabControlBaseProcess.CloseSelectedTabPage(SessionManager.GetTabControlMain());
                    }
                }

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool IsCheckDataSereServExt(HisSereServExtSDO sdo)
        {
            try
            {
                CommonParam param = new CommonParam();
                var res = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisSereServExt/CheckData", ApiConsumer.ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (!res)
                {
                    Inventec.Desktop.Common.Message.WaitingManager.Hide();
                    Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, false);
                }
                return res;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return false;
        }
        private List<FileSDO> ProcessPdfList(List<SereServFileADO> currentSsPdf)
        {
            List<FileSDO> result = new List<FileSDO>();
            try
            {
                foreach (var item in currentSsPdf)
                {
                    if (item.ID <= 0)
                    {
                        FileSDO file = new FileSDO();
                        file.FileName = item.SERE_SERV_FILE_NAME;
                        file.Content = Utils.FileToByte(item.URL);
                        file.FileType = 2;
                        file.NumOrder = item.NUM_ORDER;
                        result.Add(file);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool CheckMachine(ServiceADO sereServ)
        {
            bool result = true;
            try
            {
                if (sereServ != null && sereServ.MACHINE_ID.HasValue)
                {
                    if ((Config.AppConfigKeys.IsMachineWarningOption == "1" || Config.AppConfigKeys.IsMachineWarningOption == "2") && ((AppConfigKeys.IsPatientTypeOption == "1" && sereServ.PATIENT_TYPE_ID == AppConfigKeys.PatientTypeId__BHYT) || AppConfigKeys.IsPatientTypeOption != "1") && GlobalVariables.MachineCounterSdos != null && GlobalVariables.MachineCounterSdos.Count > 0)
                    {
                        var machine = GlobalVariables.MachineCounterSdos.FirstOrDefault(o => o.ID == sereServ.MACHINE_ID);

                        HIS_SERE_SERV_EXT ext = dicSereServExt != null && dicSereServExt.ContainsKey(sereServ.ID) ? dicSereServExt[sereServ.ID] : null;
                        if (machine != null && (ext == null || (ext != null && ext.MACHINE_ID != machine.ID)) &&
                            machine.MAX_SERVICE_PER_DAY.HasValue && machine.TOTAL_PROCESSED_SERVICE.HasValue &&
                            machine.TOTAL_PROCESSED_SERVICE.Value >= machine.MAX_SERVICE_PER_DAY.Value)
                        {
                            string mess = string.Format(ResourceMessage.BanCoMuonTiepTucKhong, string.Format(ResourceMessage.CanhBaoMayVuotQuaSoluongXuLy, machine.MACHINE_NAME, machine.MAX_SERVICE_PER_DAY));
                            if (Config.AppConfigKeys.IsMachineWarningOption == "2")
                                mess = string.Format(ResourceMessage.CanhBaoMayVuotQuaSoluongXuLy, machine.MACHINE_NAME, machine.MAX_SERVICE_PER_DAY);
                            if ((Config.AppConfigKeys.IsMachineWarningOption == "1" && DevExpress.XtraEditors.XtraMessageBox.Show(mess, ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) || (Config.AppConfigKeys.IsMachineWarningOption == "2" && DevExpress.XtraEditors.XtraMessageBox.Show(mess, ResourceMessage.ThongBao, MessageBoxButtons.OK) == DialogResult.OK))
                            {
                                result = false;
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

        private bool CheckChanged_SereServExtDescription(long id, string description)
        {
            bool rs = false;
            try
            {
                if (String.IsNullOrEmpty(description))
                {
                    return false;
                }

                if (dicOldData.ContainsKey(id) && dicOldData[id] != description)
                {
                    rs = true;
                }

                //CommonParam param = new CommonParam();
                //MOS.Filter.HisSereServExtFilter filter = new HisSereServExtFilter();
                //filter.ID = id;
                //var result = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_EXT>>(RequestUriStore.HIS_SERE_SERV_EXT_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null).FirstOrDefault();
                //if (result != null && result.DESCRIPTION != description)
                //{
                //    rs = true;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                rs = false;
            }
            return rs;
        }

        private void SaveAllProcess(bool printNow, bool isClose, bool isPrintPreview = false)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                isPressButtonSave = true;
                if (!dxValidationProvider1.Validate())
                {
                    xtraTabControl1.SelectedTabPage = xtraTabPageConclude;
                    return;
                }

                MOS.SDO.HisSereServExtWithFileSDO apiResult = null;

                if (String.IsNullOrWhiteSpace(this.SelectedFolderForSaveImage) && chkSaveImageToFile.Checked)
                {
                    if (XtraMessageBox.Show(ResourceMessage.BanChuaChonDuongDanLuuAnh, ResourceMessage.ThongBao, MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        SelectFolder();
                    }
                    else
                    {
                        chkSaveImageToFile.Checked = false;
                    }
                }

                Inventec.Desktop.Common.Message.WaitingManager.Show();
                CheckSereServExt(listServiceADOForAllInOne.Select(s => s.ID).ToList());
                SaveAllImage();
                foreach (var sereServ in listServiceADOForAllInOne)
                {
                    if (sereServ == null) return;
                    var sereServExt = new MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT();

                    sereServExt = dicSereServExt.ContainsKey(sereServ.ID) ? dicSereServExt[sereServ.ID] : new HIS_SERE_SERV_EXT() { SERE_SERV_ID = sereServ.ID };

                    sereServExt.CONCLUDE = sereServ.conclude;
                    sereServExt.NOTE = sereServ.note;
                    sereServExt.NUMBER_OF_FILM = sereServ.NUMBER_OF_FILM;
                    sereServExt.FILM_SIZE_ID = cboSizeOfFilm.EditValue != null ? (long?)cboSizeOfFilm.EditValue : null;
                    if (dtBeginTime.EditValue != null)
                        sereServExt.BEGIN_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtBeginTime.DateTime);
                    else
                        sereServExt.BEGIN_TIME = null;

                    if (dtEndTime.EditValue != null)
                        sereServExt.END_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtEndTime.DateTime);
                    else
                        sereServExt.END_TIME = null;

                    //gán trực tiếp từ grid do đã tự động chọn trước đó
                    //danh sách máy xử lý bị mất thông tin
                    sereServExt.MACHINE_ID = sereServ.MACHINE_ID;

                    var machine = ListMachine.FirstOrDefault(o => o.ID == sereServ.MACHINE_ID);
                    if (machine != null)
                    {
                        sereServExt.MACHINE_CODE = machine.MACHINE_CODE;
                        sereServExt.MACHINE_ID = machine.ID;
                    }

                    string xmlDescriptionLocation = "";
                    string description = "";
                    string descriptionInFrmClsInfo = "";
                    if (this.dicSereServExt != null && this.dicSereServExt.ContainsKey(sereServ.ID))
                    {
                        bool isChanged = CheckChanged_SereServExtDescription(this.dicSereServExt[sereServ.ID].ID, this.dicSereServExt[sereServ.ID].DESCRIPTION);
                        Inventec.Common.Logging.LogSystem.Debug("SaveAllProcess_CheckChanged_SereServExtDescription: " + isChanged.ToString());
                        if (isChanged)
                            descriptionInFrmClsInfo = this.dicSereServExt[sereServ.ID].DESCRIPTION ?? "";
                    }
                    this.GetRangeAndData(ref xmlDescriptionLocation, ref description);

                    sereServExt.XML_DESCRIPTION_LOCATION = xmlDescriptionLocation;

                    description += " " + descriptionInFrmClsInfo;
                    int index = 0;
                    if (description.Length > 1000)
                    {
                        index = description.Length - 1000;
                    }
                    sereServExt.DESCRIPTION = Inventec.Common.String.CountVi.SubStringVi(description, index, 3000);

                    List<FileHolder> listFileHolder = new List<FileHolder>();

                    HisSereServExtSDO data = new HisSereServExtSDO();
                    data.HisSereServExt = sereServExt;
                    data.HisEkipUsers = ProcessEkipUser(sereServ);
                    ProcessSereServPtttInfo(ref data, sereServ, currentServiceReq);
                    if (dicSereServSuin != null && dicSereServSuin.Count > 0 && dicSereServSuin.ContainsKey(sereServ.ID) && dicSereServSuin[sereServ.ID].Count > 0)
                    {
                        AutoMapper.Mapper.CreateMap<V_HIS_SERE_SERV_SUIN, HIS_SERE_SERV_SUIN>();
                        data.HisSereServSuins = AutoMapper.Mapper.Map<List<HIS_SERE_SERV_SUIN>>(dicSereServSuin[sereServ.ID]);
                    }
                    if (this.listImage != null && this.listImage.Count > 0 && chkAttachImage.Checked)
                    {
                        data.Files = ProcessImageList(this.listImage.Where(o => o.IsChecked).ToList());
                    }
                    if (this.currentSsPdf != null && this.currentSsPdf.Count > 0)
                    {
                        if (data.Files == null)
                            data.Files = new List<FileSDO>();
                        data.Files.AddRange(ProcessPdfList(currentSsPdf));
                    }
                    if (!IsCheckDataSereServExt(data))
                        return;
                    if (ProcessSereServExt__DescriptionPrint(param, sereServ, sereServExt))
                    {
                        Inventec.Desktop.Common.Message.WaitingManager.Hide();
                        Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        return;
                    }

                    apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post
                        <MOS.SDO.HisSereServExtWithFileSDO>
                        (sereServExt.ID == 0 ?
                        RequestUriStore.HIS_SERE_SERV_EXT_CREATE_SDO :
                        RequestUriStore.HIS_SERE_SERV_EXT_UPDATE_SDO,
                        ApiConsumer.ApiConsumers.MosConsumer, param, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken);
                    Inventec.Common.Logging.LogSystem.Debug("Goi api: " + (sereServExt.ID == 0 ?
                        RequestUriStore.HIS_SERE_SERV_EXT_CREATE_SDO :
                        RequestUriStore.HIS_SERE_SERV_EXT_UPDATE_SDO) + "____Du lieu dau vao:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data) +
                       "____Du lieu dau ra:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiResult), apiResult));
                    //ẩn trước khi hiển thị thông báo 
                    Inventec.Desktop.Common.Message.WaitingManager.Hide();

                    if (apiResult != null)
                    {
                        this.sereServExt = apiResult.SereServExt;
                        if (dicSereServExt.ContainsKey(apiResult.SereServExt.SERE_SERV_ID))
                        {
                            sereServ.NUMBER_OF_FILM = apiResult.SereServExt.NUMBER_OF_FILM;
                            dicSereServExt[apiResult.SereServExt.SERE_SERV_ID] = apiResult.SereServExt;
                            if (dicSarPrint.ContainsKey(apiResult.SereServExt.ID))
                            {
                                dicSarPrint[apiResult.SereServExt.ID] = GetListPrintByDescriptionPrint(apiResult.SereServExt);
                            }
                        }
                        else
                        {
                            dicSereServExt.Add(apiResult.SereServExt.SERE_SERV_ID, apiResult.SereServExt);
                            dicSarPrint[apiResult.SereServExt.ID] = GetListPrintByDescriptionPrint(apiResult.SereServExt);
                        }

                        this.currentServiceReq.EXECUTE_LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        this.currentServiceReq.EXECUTE_USERNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                        this.sereServExt.MODIFY_TIME = Inventec.Common.DateTime.Get.Now();
                        if (dicSereServExt.ContainsKey(apiResult.SereServExt.SERE_SERV_ID))
                            dicSereServExt[apiResult.SereServExt.SERE_SERV_ID].MODIFY_TIME = Inventec.Common.DateTime.Get.Now();

                        if (apiResult.SereServPttt != null && dicSereServPttt.ContainsKey(apiResult.SereServPttt.SERE_SERV_ID))
                        {
                            dicSereServPttt[apiResult.SereServPttt.SERE_SERV_ID] = apiResult.SereServPttt;
                        }

                        ProcessPatientInfo();

                        success = true;
                        if (!sereServ.EKIP_ID.HasValue)
                        {
                            sereServ.EKIP_ID = LoadEkipId(sereServ.ID);
                        }

                        foreach (var ado in listServiceADO)
                        {
                            var ext = dicSereServExt.ContainsKey(ado.ID) ? dicSereServExt[ado.ID] : null;
                            if (!String.IsNullOrWhiteSpace(ext.NOTE) || !String.IsNullOrWhiteSpace(ext.CONCLUDE) || ext.BEGIN_TIME != null)
                            {
                                ado.IsProcessed = true;
                            }
                            if (ado.ID == sereServ.ID)
                            {
                                ado.EKIP_ID = sereServ.EKIP_ID;
                                break;
                            }
                        }
                        gridControlSereServ.DataSource = null;
                        gridControlSereServ.DataSource = listServiceADO;
                        btnPrint.Enabled = true;
                        BtnEmr.Enabled = true;
                    }
                    else
                    {
                        success = false;
                        break;
                    }
                }
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
                //lưu và ký và đóng
                if (success && chkSign.Checked)
                {
                    BtnEmr_Click(null, null);
                }

                if (success && printNow)
                {
                    PrintResult(printNow && !isPrintPreview);
                }
                else if (success && !printNow && !chkSign.Checked && isClose)
                {
                    Inventec.Desktop.Common.Message.WaitingManager.Hide();
                    PrintResult(printNow && !isPrintPreview);
                }

                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, apiResult != null);
                #endregion

                if (success && (isClose || ChkAutoFinish.Checked))
                {
                    if (WarningConfig == true)
                    {
                        XtraMessageBox.Show("Bạn không là Bác sỹ nên không được phép kết thúc", "Thông báo");
                    }
                    else
                    {
                        btnFinish_Click(null, null);//chỉ kết thúc khi tất cả đã thực hiện
                    }
                }

                if (success && isClose)
                {
                    TabControlBaseProcess.CloseSelectedTabPage(SessionManager.GetTabControlMain());
                }
                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //trước khi lưu get dữ liệu sereServExt để đảm bảo gọi đúng api
        private void CheckSereServExt(List<long> listSereServId)
        {
            try
            {
                if (listSereServId != null && listSereServId.Count > 0)
                {
                    bool isGetData = false;
                    foreach (var sereServId in listSereServId)
                    {
                        if (!dicSereServExt.ContainsKey(sereServId))
                        {
                            isGetData = true;
                            break;
                        }
                    }

                    if (isGetData)
                    {
                        ProcessDicSereServExt(listSereServId);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private long? LoadEkipId(long sereServId)
        {
            long? ekipId = null;
            try
            {
                if (sereServId > 0)
                {
                    MOS.Filter.HisSereServFilter filter = new MOS.Filter.HisSereServFilter();
                    filter.ID = sereServId;
                    var sereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>(RequestUriStore.HIS_SERE_SERV_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (sereServ != null && sereServ.Count == 1)
                    {
                        ekipId = sereServ.First().EKIP_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                ekipId = null;
            }
            return ekipId;
        }

        private void BtnEmr_Click(object sender, EventArgs e)
        {
            try
            {
                if (!BtnEmr.Enabled) return;
                Library.EmrGenerate.EmrGenerateProcessor generateProcessor = new Library.EmrGenerate.EmrGenerateProcessor();
                InputADO inputADO = generateProcessor.GenerateInputADOWithPrintTypeCode(currentServiceReq.TDL_TREATMENT_CODE, "", true, moduleData.RoomId);

                HIS_SERE_SERV_TEMP SereServTemp = null;
                if (sereServExt != null && dicSarPrint.ContainsKey(sereServExt.ID))
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dicSarPrint[sereServExt.ID]), dicSarPrint[sereServExt.ID]));
                    inputADO.DocumentTypeCode = dicSarPrint[sereServExt.ID].EMR_DOCUMENT_TYPE_CODE;
                    inputADO.DocumentGroupCode = dicSarPrint[sereServExt.ID].EMR_DOCUMENT_GROUP_CODE;
                    if (!String.IsNullOrWhiteSpace(dicSarPrint[sereServExt.ID].EMR_BUSINESS_CODES))
                    {
                        var codes = dicSarPrint[sereServExt.ID].EMR_BUSINESS_CODES.Split(';').ToList();
                        if (codes.Count() == 1)
                        {
                            inputADO.BusinessCode = codes[0];
                        }
                        else
                        {
                            var listBussiness = BackendDataWorker.Get<EMR.EFMODEL.DataModels.EMR_BUSINESS>().Where(o => codes.Contains(o.BUSINESS_CODE)).ToList();
                            MPS.ProcessorBase.EmrBusiness.frmChooseBusiness frmChooseBusiness = new MPS.ProcessorBase.EmrBusiness.frmChooseBusiness(ChooseBusinessClick, listBussiness);
                            frmChooseBusiness.ShowDialog();

                            inputADO.BusinessCode = currentBussinessCode;
                            dicSarPrint[sereServExt.ID].EMR_BUSINESS_CODES = currentBussinessCode;
                            CreateThreadUpdateBusinessCode(dicSarPrint[sereServExt.ID]);
                        }
                    }

                    inputADO.DocumentName = (String.Format("{0} (Mã điều trị: {1})", dicSarPrint[sereServExt.ID].TITLE, currentServiceReq.TDL_TREATMENT_CODE));
                    if (!String.IsNullOrWhiteSpace(dicSarPrint[sereServExt.ID].ADDITIONAL_INFO) && dicSarPrint[sereServExt.ID].ADDITIONAL_INFO.Contains("SERE_SERV_TEMP_CODE"))
                    {
                        string TEMP_CODE = dicSarPrint[sereServExt.ID].ADDITIONAL_INFO.Replace("SERE_SERV_TEMP_CODE:", "").Trim();
                        var temp = listTemplate.FirstOrDefault(o => o.SERE_SERV_TEMP_CODE == TEMP_CODE);
                        if (temp != null)
                        {
                            inputADO.DocumentName = (String.Format("{0} (Mã điều trị: {1})", temp.SERE_SERV_TEMP_NAME, currentServiceReq.TDL_TREATMENT_CODE));
                            SereServTemp = temp;
                        }
                    }

                    inputADO.HisCode = string.Format("SERVICE_REQ_CODE:{0} SER_SERV_ID:{1}", this.sereServ.TDL_SERVICE_REQ_CODE, sereServExt.SERE_SERV_ID);
                }
                else if (cboSereServTemp.EditValue != null)
                {
                    var temp = listTemplate.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboSereServTemp.EditValue.ToString()));
                    if (temp != null)
                    {
                        inputADO.DocumentTypeCode = temp.EMR_DOCUMENT_TYPE_CODE;
                        inputADO.DocumentGroupCode = temp.EMR_DOCUMENT_GROUP_CODE;
                        inputADO.BusinessCode = temp.EMR_BUSINESS_CODES;
                        SereServTemp = temp;
                    }
                }

                if (String.IsNullOrWhiteSpace(inputADO.DocumentTypeCode))
                {
                    inputADO.DocumentTypeCode = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ServiceExecuteCFG.EmrDocumentTypeCode);
                }

                if (!String.IsNullOrWhiteSpace(inputADO.DocumentName) && !String.IsNullOrWhiteSpace(cboSereServTemp.Text))
                {
                    inputADO.DocumentName = (String.Format("{0} (Mã điều trị: {1})", cboSereServTemp.Text, currentServiceReq.TDL_TREATMENT_CODE));//Tên văn bản cần tạo
                }

                if (String.IsNullOrWhiteSpace(inputADO.HisCode))
                {
                    inputADO.HisCode = string.Format("SERVICE_REQ_CODE:{0} SER_SERV_ID:{1}", this.sereServ.TDL_SERVICE_REQ_CODE, this.sereServ.ID);
                }

                inputADO.HisOrder = currentServiceReq.SERVICE_REQ_CODE;

                DevExpress.XtraRichEdit.RichEditControl printDocument = ProcessDocumentBeforePrint(GettxtDescription());
                if (printDocument == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("printDocument is null");
                    return;
                }

                ProcessColumnMaping(SereServTemp, ref inputADO);
                if (chkSign.Checked)
                {
                    string base64File = "";
                    string pdfAddFile = Utils.GenerateTempFileWithin();
                    string OutputFile = Utils.GenerateTempFileWithin();

                    List<string> joinStreams = new List<string>();
                    using (MemoryStream pdfData = new MemoryStream())
                    {
                        printDocument.ExportToPdf(pdfData);
                        pdfData.Position = 0;
                        Utils.ByteToFile(Utils.StreamToByte(pdfData), pdfAddFile);
                        joinStreams.Add(pdfAddFile);
                    }

                    if (currentSsPdf != null && currentSsPdf.Count > 0)
                    {
                        var dataPdf = currentSsPdf.OrderBy(o => o.NUM_ORDER).ToList();
                        foreach (var item in dataPdf)
                        {
                            var stream = Inventec.Fss.Client.FileDownload.GetFile(item.URL);

                            if (stream != null && stream.Length > 0)
                            {
                                stream.Position = 0;
                                string pdfFile = Utils.GenerateTempFileWithin();
                                Utils.ByteToFile(Utils.StreamToByte(stream), pdfFile);
                                joinStreams.Add(pdfFile);
                            }
                        }
                    }
                    Stream currentStream = File.Open(OutputFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

                    var pdfConcat = new iTextSharp.text.pdf.PdfConcatenate(currentStream);

                    var pages = new List<int>();

                    foreach (var file in joinStreams)
                    {
                        iTextSharp.text.pdf.PdfReader pdfReader = null;
                        pdfReader = new iTextSharp.text.pdf.PdfReader(file);
                        pages = new List<int>();
                        for (int i = 0; i <= pdfReader.NumberOfPages; i++)
                        {
                            pages.Add(i);
                        }
                        pdfReader.SelectPages(pages);
                        pdfConcat.AddPages(pdfReader);
                        pdfReader.Close();
                    }
                    try
                    {
                        pdfConcat.Close();
                    }
                    catch { }

                    foreach (var file in joinStreams)
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch { }
                    }
                    SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();

                    if (chkPrint.Checked)
                    {
                        var signNow = libraryProcessor.SignAndPrintNow(Utils.FileToBase64String(OutputFile), FileType.Pdf, inputADO);//truyền vào đường dẫn file cần ký, các định dạng hỗ trợ là: pdf,doc,docx,xls,xlsx,rdlc,...
                    }
                    else if (chkForPreview.Checked)
                    {
                        var signNow = libraryProcessor.SignAndShowPrintPreview(Utils.FileToBase64String(OutputFile), FileType.Pdf, inputADO);//truyền vào đường dẫn file cần ký, các định dạng hỗ trợ là: pdf,doc,docx,xls,xlsx,rdlc,...
                    }
                    else
                    {
                        var signNow = libraryProcessor.SignNow(Utils.FileToBase64String(OutputFile), FileType.Pdf, inputADO);//truyền vào đường dẫn file cần ký, các định dạng hỗ trợ là: pdf,doc,docx,xls,xlsx,rdlc,...
                    }
                    //if (!signNow.Success && !String.IsNullOrWhiteSpace(signNow.Message))
                    //{
                    //    XtraMessageBox.Show(signNow.Message);
                    //}
                }
                else
                {
                    String temFile = Path.GetTempFileName();
                    temFile = temFile.Replace(".tmp", ".pdf");
                    printDocument.ExportToPdf(temFile);
                    SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();
                    libraryProcessor.ShowPopup(temFile, inputADO);//truyền vào đường dẫn file cần ký, các định dạng hỗ trợ là: pdf,doc,docx,xls,xlsx,rdlc,...
                    File.Delete(temFile);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThreadUpdateBusinessCode(SAR.EFMODEL.DataModels.SAR_PRINT data)
        {
            Thread update = new Thread(UpdateBusinessCode);
            try
            {
                update.Start(data);
            }
            catch (Exception ex)
            {
                update.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateBusinessCode(object obj)
        {
            try
            {
                if (obj != null && obj is SAR.EFMODEL.DataModels.SAR_PRINT)
                {
                    var data = (SAR.EFMODEL.DataModels.SAR_PRINT)obj;
                    CommonParam param = new CommonParam();
                    this.currentSarPrint = new Inventec.Common.Adapter.BackendAdapter(param).Post<SAR.EFMODEL.DataModels.SAR_PRINT>(ApiConsumer.SarRequestUriStore.SAR_PRINT_UPDATE, ApiConsumer.ApiConsumers.SarConsumer, data, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChooseBusinessClick(EMR.EFMODEL.DataModels.EMR_BUSINESS dataBusiness)
        {
            try
            {
                if (dataBusiness != null)
                {
                    this.currentBussinessCode = dataBusiness != null ? dataBusiness.BUSINESS_CODE : "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAssignPaan_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentServiceReq != null)
                {

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPaan").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPaan");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();

                        listArgs.Add(currentServiceReq);
                        listArgs.Add(currentServiceReq.TREATMENT_ID);
                        listArgs.Add(currentServiceReq.ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Warn("HIS.Desktop.Plugins.ServiceExecute btnAssignPaan_Click currentServiceReq is null");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnChangeOption_Click(object sender, EventArgs e)
        {
            try
            {
                this.isWordFull = !this.isWordFull;
                string rtfTextCurrent = ProcessGetRtfTextFromUc();
                this.panelDescription.Controls.Clear();
                string positionDescriptionData = "";
                //luôn có 1 trong 2 control wordDocument hoặc UcTelerikDocument được khởi tạo
                //lấy vị trí select gán cho uc còn lại
                if (this.wordDocument != null)
                {
                    if (this.isWordFull)
                    {
                        positionDescriptionData = this.wordDocument.GetRange();
                        if (this.wordFullDocument == null)
                        {
                            this.wordFullDocument = new UcWordFull(EditorZoomChanged);
                            this.wordFullDocument.Dock = DockStyle.Fill;
                        }

                        this.wordFullDocument.txtDescription.RtfText = rtfTextCurrent;
                        this.panelDescription.Controls.Add(this.wordFullDocument);
                    }
                    else
                    {
                        this.wordDocument.txtDescription.RtfText = rtfTextCurrent;
                        this.panelDescription.Controls.Add(this.wordDocument);
                        if (this.wordFullDocument != null)
                        {
                            positionDescriptionData = this.wordFullDocument.GetRange();
                        }
                    }
                }
                else if (this.UcTelerikDocument != null)
                {
                    if (this.isWordFull)
                    {
                        positionDescriptionData = this.UcTelerikDocument.GetRange();
                        if (this.UcTelerikFullDocument == null)
                        {
                            this.UcTelerikFullDocument = new UcWords.UcTelerikFullWord(EditorZoomChanged);
                            this.UcTelerikFullDocument.Dock = DockStyle.Fill;
                        }

                        this.UcTelerikFullDocument.SetRtfText(rtfTextCurrent);
                        this.panelDescription.Controls.Add(this.UcTelerikFullDocument);
                    }
                    else
                    {
                        this.UcTelerikDocument.SetRtfText(rtfTextCurrent);
                        this.panelDescription.Controls.Add(this.UcTelerikDocument);
                        if (this.UcTelerikFullDocument != null)
                        {
                            positionDescriptionData = this.UcTelerikFullDocument.GetRange();
                        }
                    }
                }

                ProcessWordProtected(true);
                ProcessSelectDataForDescription(positionDescriptionData);
            }
            catch (Exception ex)
            {
                this.panelDescription.Controls.Clear();
                this.panelDescription.Controls.Add(this.wordDocument);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAddImage_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboSereServTemp.EditValue != null)
                {
                    var data = listTemplate.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboSereServTemp.EditValue ?? 0).ToString()));
                    if (data != null)
                    {
                        txtSereServTempCode.Text = data.SERE_SERV_TEMP_CODE;
                        cboSereServTemp.Properties.Buttons[1].Visible = true;
                        ProcessChoiceSereServTempl(data);
                    }
                }

                ProcessFocusWord();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            ViewImage.FormHistoryCLS form = new ViewImage.FormHistoryCLS(moduleData, currentServiceReq);
            form.ShowDialog();
        }

        #region Load Image From PACS
        private void LoadImageFromPacs()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("LoadImageFromPacs");
                if (this.sereServ == null) return;
                if (ConnectImageOption != "1" || String.IsNullOrEmpty(this.sereServ.TDL_PACS_TYPE_CODE)) return;
                if (String.IsNullOrEmpty(ConfigSystems.URI_API_PACS)) return;

                Inventec.Common.Logging.LogSystem.Debug("LoadImageFromPacs 2");
                //lấy ảnh từ máy xử lý đưa lên grid
                this.listImage = new List<ADO.ImageADO>();
                var images = LoadImageFromPacsService(sereServ.SoPhieu);

                if (images != null && images.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("DIC_SERVER_PACS:" + PACS.PacsCFG.DIC_SERVER_PACS.Count);
                    foreach (var item in images)
                    {
                        try
                        {
                            string result = item.GetHashCode().ToString();
                            if (!String.IsNullOrEmpty(item.ImageThumbFileName))
                                result = item.ImageThumbFileName.Split('.').FirstOrDefault();

                            ADO.ImageADO image = new ADO.ImageADO();
                            image.FileName = result;
                            image.IsChecked = false;

                            if (ConnectPacsByFss == "1")
                            {
                                if (String.IsNullOrEmpty(ConfigSystems.URI_API_FSS_FOR_PACS)) return;
                                string direct = item.ImageDirectory.Split(fss, StringSplitOptions.None).LastOrDefault();
                                string fullDirect = direct + "\\" + item.ImageThumbFileName;

                                Stream jpg = Inventec.Fss.Client.FileDownload.GetFile(fullDirect, ConfigSystems.URI_API_FSS_FOR_PACS);
                                image.streamImage = new MemoryStream();
                                jpg.Position = 0;
                                jpg.CopyTo(image.streamImage);
                                jpg.Position = 0;
                                image.IMAGE_DISPLAY = Image.FromStream(jpg);
                            }
                            else
                            {
                                var s = ConfigSystems.URI_API_PACS.Split('/');
                                var ip = s[2].Split(':').FirstOrDefault();
                                if (PACS.PacsCFG.DIC_SERVER_PACS != null && PACS.PacsCFG.DIC_SERVER_PACS.ContainsKey(ip))
                                {
                                    string direct = item.ImageDirectory.Split(fss, StringSplitOptions.None).FirstOrDefault();
                                    string file = String.Format("{0}\\{1}", item.ImageDirectory.Replace(direct + fss[0], "\\\\" + ip + "\\"), item.ImageThumbFileName);
                                    Inventec.Common.Logging.LogSystem.Info(file);
                                    image.IMAGE_DISPLAY = Image.FromFile(file);

                                    byte[] buff = System.IO.File.ReadAllBytes(file);
                                    image.streamImage = new System.IO.MemoryStream(buff);
                                }
                                Inventec.Common.Logging.LogSystem.Debug("item.ImageDirectory: " + item.ImageDirectory);
                            }

                            listImage.Add(image);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Warn("khong lay duoc thong tin anh");
                }
                ProcessLoadGridImage(this.listImage);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<PACS.ImagesADO> LoadImageFromPacsService(string soPhieu)
        {
            List<PACS.ImagesADO> result = null;
            try
            {
                CommonParam param = new CommonParam();
                PACS.ImageRequestADO layThongTinAnhInputADO = new PACS.ImageRequestADO();
                layThongTinAnhInputADO.SoPhieu = soPhieu;
                var resultData = new Inventec.Common.Adapter.BackendAdapter(param).PostWithouApiParam<PACS.ImageResponseADO>(RequestUriStore.PACS_SERIVCE__LAY_THONG_TIN_ANH, PACS.PacsApiConsumer.PacsConsumer, layThongTinAnhInputADO, null, param);
                if (resultData != null && resultData.TrangThai && resultData.Series != null)
                {
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultData), resultData));
                    result = new List<PACS.ImagesADO>();
                    foreach (var item in resultData.Series)
                    {
                        item.Images.ForEach(o => o.SeriesDateTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(item.SeriesDateTime)));
                        result.AddRange(item.Images);
                    }
                    result = result.OrderBy(o => o.SeriesDateTime).ToList();
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info("loi thong tin lay tư pacs");
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultData), resultData));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        private string ReduceForCode(string orderCode, int maxlength)
        {
            if (!string.IsNullOrWhiteSpace(orderCode) && orderCode.Length >= maxlength)
            {
                return orderCode.Substring(orderCode.Length - maxlength);
            }
            return orderCode;
        }

        private void ThreadLoadImageFromPacs()
        {
            Thread load = new Thread(FillImageWithThread);
            //load.Priority = ThreadPriority.Highest;
            try
            {
                load.Start();
            }
            catch (Exception ex)
            {
                load.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillImageWithThread()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { this.LoadImageFromPacs(); }));
                }
                else
                {
                    this.LoadImageFromPacs();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region TraKqSA_Form dong_FRD000006
        private void repositoryItembtnTraKqSA_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (ADO.ServiceADO)gridViewSereServ.GetFocusedRow();
                if (row == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.KhongXacDinhDuocMay);
                    return;
                }

                if (row.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA)
                {
                    return;
                }

                HIS_SERE_SERV sereServInput = new HIS_SERE_SERV();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV>(sereServInput, row);
                HIS_SERE_SERV_EXT sereServExtInput = new HIS_SERE_SERV_EXT();
                if (dicSereServExt.ContainsKey(sereServInput.ID))
                {
                    sereServExtInput = dicSereServExt[sereServInput.ID];
                }

                PopupMenu menu = new PopupMenu(barManager1);
                menu.ItemLinks.Clear();

                FormOtherProcessor FormOtherProcessor = new FormOtherProcessor(sereServInput, sereServExtInput, RefeshReferenceFormOther);
                var pmenus = FormOtherProcessor.GetBarButtonItem(barManager1);
                List<BarItem> bItems = new List<BarItem>();
                foreach (var item in pmenus)
                {
                    bItems.Add((BarItem)item);
                }

                menu.AddItems(bItems.ToArray());
                menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RefeshReferenceFormOther()
        {
            try
            {
                var datas = gridControlSereServ.DataSource as List<ADO.ServiceADO>;
                var listId = datas.Select(o => o.ID).ToList();
                this.dicSereServExt = new Dictionary<long, HIS_SERE_SERV_EXT>();
                ProcessDicSereServExt(listId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #endregion

        #region Public Method
        public void Save()
        {
            try
            {
                //ẩn nút sẽ ko bấm phím tắt đc
                if (btnSave.Enabled && layoutControlItem10.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void End()
        {
            try
            {
                if (btnFinish.Enabled && layoutControlItem9.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    btnFinish_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Print()
        {
            try
            {
                if (!btnPrint.Enabled) return;
                if (lciForbtnPrint.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never) return;
                PrintResult(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void AssignService()
        {
            try
            {
                if (btnAssignService.Enabled && layoutControlItem11.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    btnAssignService_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void CaptureImage1()
        {
            try
            {
                if (btnCamera.Enabled && layoutControlItem32.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    btnCamera_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void AssignPre()
        {
            try
            {
                if (btnAssignPrescription.Enabled && layoutControlItem12.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    btnAssignPrescription_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void ChupHinhClick()
        {
            if (btnCapture.Enabled && layoutControlItem29.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
            {
                btnCapture_Click(null, null);
            }
        }

        public void ChupHinhClick1()
        {
            if (btnCapture.Enabled && layoutControlItem29.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
            {
                btnCapture_Click(null, null);
            }
        }

        public void Search()
        {
            try
            {
                //ẩn nút sẽ ko bấm phím tắt đc
                txtServiceReqCode.Focus();
                txtServiceReqCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private List<HIS.Desktop.Plugins.ServiceExecute.ADO.ImageADO> ProcessOrderImage(List<HIS.Desktop.Plugins.ServiceExecute.ADO.ImageADO> images)
        {
            try
            {
                if (images != null && images.Count > 0)
                {
                    List<ADO.ImageADO> listImageTemp = new List<ADO.ImageADO>();
                    var listImageSTTOrder = images.Where(o => o.IsChecked).OrderBy(o => o.STTImage).ToList();
                    var listImageNoSTTOrder = images.Where(o => !o.IsChecked).OrderBy(o => o.FileName).ToList();
                    if (listImageSTTOrder != null && listImageSTTOrder.Count > 0)
                        listImageTemp.AddRange(listImageSTTOrder);
                    if (listImageNoSTTOrder != null && listImageNoSTTOrder.Count > 0)
                        listImageTemp.AddRange(listImageNoSTTOrder);
                    return listImageTemp;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return null;
        }

        private void ClearDocument()
        {
            try
            {
                if (this.wordFullDocument != null)
                {
                    this.wordFullDocument.txtDescription.Text = "";
                    try
                    {
                        this.wordFullDocument.txtDescription.Document.Unprotect();
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }

                if (this.wordDocument != null)
                {
                    this.wordDocument.txtDescription.Text = "";
                    try
                    {
                        this.wordDocument.txtDescription.Document.Unprotect();
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }

                if (this.UcTelerikDocument != null)
                {
                    this.UcTelerikDocument.SetRtfText("");
                    try
                    {
                        this.UcTelerikDocument.radRichTextEditor1.DeleteReadOnlyRange();
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }

                if (this.UcTelerikFullDocument != null)
                {
                    this.UcTelerikFullDocument.SetRtfText("");
                    try
                    {
                        this.UcTelerikFullDocument.radRichTextEditor1.DeleteReadOnlyRange();
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboConnectionType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboConnectionType.EditValue != null)
                    {
                        ApplicationCaptureTypeWorker.ChangeCaptureConnectType((int)cboConnectionType.EditValue);
                        StopClick();
                        EnableControlCamera(false);
                        cboConnectionType.Enabled = true;
                        btnCamera.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cardControl_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                int count = cardControl.Size.Width / 200;
                tileView1.OptionsTiles.ColumnCount = count > 0 ? count : 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSereServ_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    ADO.ServiceADO sereServ = (ADO.ServiceADO)gridViewSereServ.GetRow(e.RowHandle);
                    if (sereServ != null)
                    {
                        if (sereServ.NUMBER_OF_FILM.HasValue && sereServ.NUMBER_OF_FILM.Value > 0)
                        {
                            e.Appearance.ForeColor = System.Drawing.Color.Red;
                        }

                        if (sereServ.IS_NO_EXECUTE == 1)
                        {
                            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSereServ_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ADO.ServiceADO dataRow = (ADO.ServiceADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow == null) return;

                    if (e.Column.FieldName == "ImageStt")
                    {
                        if (!String.IsNullOrWhiteSpace(dicSereServExt[dataRow.ID].NOTE) || !String.IsNullOrWhiteSpace(dicSereServExt[dataRow.ID].CONCLUDE) || dicSereServExt[dataRow.ID].BEGIN_TIME != null)
                        {
                            e.Value = imageListStt.Images[1];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlSereServ)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlSereServ.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandleSS != info.RowHandle || lastColumnSS != info.Column)
                        {
                            lastColumnSS = info.Column;
                            lastRowHandleSS = info.RowHandle;
                            string text = "";
                            if (info.Column.FieldName == "ImageStt")
                            {
                                var ext = view.GetRowCellValue(lastRowHandleSS, "ID");
                                if (ext != null)
                                {
                                    if (ext.GetType() == typeof(long) && dicSereServExt.ContainsKey((long)ext))
                                    {
                                        var dataRow = dicSereServExt[(long)ext];
                                        if (!String.IsNullOrWhiteSpace(dataRow.NOTE) || !String.IsNullOrWhiteSpace(dataRow.CONCLUDE) || dataRow.BEGIN_TIME != null)
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkForPreview_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkForPreview.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkForPreview.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkForPreview.Name;
                    csAddOrUpdate.VALUE = (chkForPreview.Checked ? "1" : "");
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

        private void txtServiceReqCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ProcessSearchByServiceReqCode();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessSearchByServiceReqCode()
        {
            if (!String.IsNullOrEmpty(txtServiceReqCode.Text))
            {
                if (Inventec.Common.TypeConvert.Parse.ToInt64(txtServiceReqCode.Text) <= 0)
                {
                    MessageManager.Show(ResourceMessage.MaYLenhKhongHopLe);
                    txtServiceReqCode.Focus();
                    txtServiceReqCode.SelectAll();
                    return;
                }

                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqFilter reqFilter = new MOS.Filter.HisServiceReqFilter();
                reqFilter.SERVICE_REQ_CODE__EXACT = string.Format("{0:000000000000}", Convert.ToInt64(txtServiceReqCode.Text));
                reqFilter.EXECUTE_ROOM_ID = moduleData.RoomId;
                var result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(RequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumer.ApiConsumers.MosConsumer, reqFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (result != null && result.Count > 0)
                {
                    currentServiceReq = result.FirstOrDefault();
                    if (currentServiceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                    {
                        if (!StartEvent(currentServiceReq))
                        {
                            MessageManager.Show(ResourceMessage.KhongCapNhatDuocTrangThaiXuLyCuaYLenh);
                            Inventec.Common.Logging.LogSystem.Debug("Goi api cap nhat trang thai xu ly cua yeu cau that bai__Y lenh van dang o trang thai chua xu ly__Dung nghiep vu khong tiep tuc xu ly.");
                            return;
                        }
                    }
                    ServiceReqConstruct = new V_HIS_SERVICE_REQ();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERVICE_REQ>(ServiceReqConstruct, currentServiceReq);
                    txtServiceReqCode.Text = currentServiceReq.SERVICE_REQ_CODE;
                    SearchNewTreatmentServiceReqForShowForm();
                }
                else
                {
                    MessageManager.Show(String.Format(ResourceMessage.KhongTimThayDULieuTheoMaYLenh, reqFilter.SERVICE_REQ_CODE__EXACT));
                }
            }
        }

        private void SearchNewTreatmentServiceReqForShowForm()
        {
            try
            {
                WaitingManager.Show();
                CreateThreadLoadDataDefault();
                SetEnableControl(true);
                FillDataToGrid();
                ProcessPatientInfo();
                LoadDataImageLocal();
                SetDisable();
                ValidNumberOfFilm();
                ValidBeginTime();
                ValidEndTime();
                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_SERVICE_REQ.IS_ALLOWING_PROCESSING_SUBCLINICAL_AFTER_LOCKING_TREATMENT") == "1" && TreatmentWithPatientTypeAlter != null && TreatmentWithPatientTypeAlter.IS_LOCK_FEE == 1)
                {
                    btnAssignService.Enabled = false;
                    btnTuTruc.Enabled = false;
                }

                ReloadCameraAfterSearchByPatientThread();
                CheckValidPress();

                var formMain = HIS.Desktop.Controls.Session.SessionManager.GetFormMain();
                if (formMain != null)
                {
                    if (formMain.InvokeRequired)
                    {
                        formMain.Invoke(new MethodInvoker(delegate
                        {
                            ////Dong tat ca cac tab page dang mo, dong thoi mo 1 page default
                            MethodReloadTextTabpageExecuteServiceByPatientThread(formMain);
                        }));
                    }
                    else
                    {
                        ////Dong tat ca cac tab page dang mo, dong thoi mo 1 page default
                        MethodReloadTextTabpageExecuteServiceByPatientThread(formMain);
                    }
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ReloadCameraAfterSearchByPatientThread()
        {
            try
            {
                this.listImage = new List<ADO.ImageADO>();
                string clientCode = this.currentServiceReq.TDL_TREATMENT_CODE;
                if ((int)cboConnectionType.EditValue == 2)
                {
                    this.ucCameraDXC1.SetClientCode(clientCode);
                }
                else
                {
                    this.ucCamera1.SetClientCode(clientCode);
                }
                List<Image> images = new List<Image>();
                if (GlobalVariables.dicImageCapture != null && GlobalVariables.dicImageCapture.ContainsKey(clientCode))
                {
                    images = GlobalVariables.dicImageCapture[clientCode];
                    GlobalVariables.dicImageCapture[clientCode] = null;
                    GlobalVariables.dicImageCapture.Remove(clientCode);
                }
                try
                {
                    if (GlobalVariables.listImage.Count > 0)
                    {
                        foreach (var item in images)
                        {
                            GlobalVariables.listImage.Remove(item);
                        }
                    }
                }
                catch (Exception exx)
                {
                    LogSystem.Warn(exx);
                }
                ProcessLoadGridImage(this.listImage);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void MethodReloadTextTabpageExecuteServiceByPatientThread(object formMain)
        {
            try
            {
                Type type = formMain.GetType();
                if (type != null)
                {
                    MethodInfo methodInfo__ResetAllTabpageToDefault = type.GetMethod("ReloadTextTabpageExecuteServiceByPatient");
                    MOS.SDO.WorkPlaceSDO workPlace = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetWorkPlace(moduleData);

                    string headerPageText = currentServiceReq.SERVICE_REQ_CODE + " - " + currentServiceReq.TDL_PATIENT_NAME + (workPlace != null ? " (" + workPlace.RoomName + ")" : "");

                    if (methodInfo__ResetAllTabpageToDefault != null)
                        methodInfo__ResetAllTabpageToDefault.Invoke(formMain, new object[] { headerPageText });
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private bool StartEvent(HIS_SERVICE_REQ serviceReqInput)
        {
            bool isStart = false;
            try
            {
                //Bắt đầu
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                V_HIS_SERVICE_REQ serviceReqResult = new BackendAdapter(param)
                .Post<V_HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_START, ApiConsumers.MosConsumer, serviceReqInput.ID, param);
                WaitingManager.Hide();
                if (serviceReqResult == null)
                {
                    #region Show message
                    MessageManager.Show(param, null);
                    Inventec.Common.Logging.LogSystem.Debug("StartEvent fail:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqResult), serviceReqResult));
                    #endregion
                }
                else
                {
                    serviceReqInput.SERVICE_REQ_STT_ID = serviceReqResult.SERVICE_REQ_STT_ID;
                    isStart = true;
                    if (this.RefreshData != null)
                    {
                        this.RefreshData(serviceReqResult);
                    }
                }
            }
            catch (Exception ex)
            {
                isStart = false;
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return isStart;
        }

        void EditorZoomChanged(decimal zoom)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => zoom), zoom));
                trackBarZoom = (long)(zoom * 100);

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == trackBarZoomName && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = ((trackBarZoom).ToString());
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = trackBarZoomName;
                    csAddOrUpdate.VALUE = ((trackBarZoom).ToString());
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void repositoryItemMachineId_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    var asereServ = (ADO.ServiceADO)gridViewSereServ.GetFocusedRow();
                    var cbo = sender as GridLookUpEdit;
                    long machineId = Inventec.Common.TypeConvert.Parse.ToInt64((cbo.EditValue ?? "0").ToString());
                    Inventec.Common.Logging.LogSystem.Info(machineId.ToString());
                    ProcessDicExecuteRoomMachine(machineId);
                    if ((Config.AppConfigKeys.IsMachineWarningOption == "1" || Config.AppConfigKeys.IsMachineWarningOption == "2") && ((AppConfigKeys.IsPatientTypeOption == "1" && asereServ.PATIENT_TYPE_ID == AppConfigKeys.PatientTypeId__BHYT) || AppConfigKeys.IsPatientTypeOption != "1") && GlobalVariables.MachineCounterSdos != null && GlobalVariables.MachineCounterSdos.Count > 0)
                    {
                        var machine = GlobalVariables.MachineCounterSdos.FirstOrDefault(o => o.ID == machineId);

                        HIS_SERE_SERV_EXT ext = dicSereServExt != null && dicSereServExt.ContainsKey(asereServ.ID) ? dicSereServExt[asereServ.ID] : null;
                        if (machine != null && (ext == null || (ext != null && ext.MACHINE_ID != machine.ID)) &&
                            machine.MAX_SERVICE_PER_DAY.HasValue && machine.TOTAL_PROCESSED_SERVICE.HasValue &&
                            machine.TOTAL_PROCESSED_SERVICE.Value >= machine.MAX_SERVICE_PER_DAY.Value)
                        {
                            string mess = string.Format(ResourceMessage.BanCoMuonTiepTucKhong, string.Format(ResourceMessage.CanhBaoMayVuotQuaSoluongXuLy, machine.MACHINE_NAME, machine.MAX_SERVICE_PER_DAY));
                            if (Config.AppConfigKeys.IsMachineWarningOption == "2")
                                mess = string.Format(ResourceMessage.CanhBaoMayVuotQuaSoluongXuLy, machine.MACHINE_NAME, machine.MAX_SERVICE_PER_DAY);
                            if ((Config.AppConfigKeys.IsMachineWarningOption == "1" && DevExpress.XtraEditors.XtraMessageBox.Show(mess, ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) || (Config.AppConfigKeys.IsMachineWarningOption == "2" && DevExpress.XtraEditors.XtraMessageBox.Show(mess, ResourceMessage.ThongBao, MessageBoxButtons.OK) == DialogResult.OK))
                            {
                                cbo.EditValue = null;
                                cbo.ShowPopup();
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

        private void repositoryItemMachineHideDelete_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    var asereServ = (ADO.ServiceADO)gridViewSereServ.GetFocusedRow();
                    var cbo = sender as GridLookUpEdit;
                    long machineId = Inventec.Common.TypeConvert.Parse.ToInt64((cbo.EditValue ?? "0").ToString());
                    ProcessDicExecuteRoomMachine(machineId);
                    if ((Config.AppConfigKeys.IsMachineWarningOption == "1" || Config.AppConfigKeys.IsMachineWarningOption == "2") && ((AppConfigKeys.IsPatientTypeOption == "1" && asereServ.PATIENT_TYPE_ID == AppConfigKeys.PatientTypeId__BHYT) || AppConfigKeys.IsPatientTypeOption != "1") && GlobalVariables.MachineCounterSdos != null && GlobalVariables.MachineCounterSdos.Count > 0)
                    {
                        var machine = GlobalVariables.MachineCounterSdos.FirstOrDefault(o => o.ID == machineId);
                        HIS_SERE_SERV_EXT ext = dicSereServExt != null && dicSereServExt.ContainsKey(asereServ.ID) ? dicSereServExt[asereServ.ID] : null;
                        if (machine != null && (ext == null || (ext != null && ext.MACHINE_ID != machine.ID)) &&
                            machine.MAX_SERVICE_PER_DAY.HasValue && machine.TOTAL_PROCESSED_SERVICE.HasValue &&
                            machine.TOTAL_PROCESSED_SERVICE.Value >= machine.MAX_SERVICE_PER_DAY.Value)
                        {
                            string mess = string.Format(ResourceMessage.BanCoMuonTiepTucKhong, string.Format(ResourceMessage.CanhBaoMayVuotQuaSoluongXuLy, machine.MACHINE_NAME, machine.MAX_SERVICE_PER_DAY));
                            if (Config.AppConfigKeys.IsMachineWarningOption == "2")
                                mess = string.Format(ResourceMessage.CanhBaoMayVuotQuaSoluongXuLy, machine.MACHINE_NAME, machine.MAX_SERVICE_PER_DAY);
                            if ((Config.AppConfigKeys.IsMachineWarningOption == "1" && DevExpress.XtraEditors.XtraMessageBox.Show(mess, ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) || (Config.AppConfigKeys.IsMachineWarningOption == "2" && DevExpress.XtraEditors.XtraMessageBox.Show(mess, ResourceMessage.ThongBao, MessageBoxButtons.OK) == DialogResult.OK))
                            {
                                cbo.EditValue = null;
                                cbo.ShowPopup();
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

        private void ProcessDicExecuteRoomMachine(long machineId)
        {
            try
            {
                if (machineId > 0)
                {
                    if (GlobalVariables.DicExecuteRoomMachine == null)
                    {
                        GlobalVariables.DicExecuteRoomMachine = new Dictionary<long, List<long>>();
                    }

                    if (!GlobalVariables.DicExecuteRoomMachine.ContainsKey(moduleData.RoomId))
                    {
                        GlobalVariables.DicExecuteRoomMachine[moduleData.RoomId] = new List<long>();
                    }

                    if (GlobalVariables.DicExecuteRoomMachine[moduleData.RoomId].Contains(machineId))
                    {
                        GlobalVariables.DicExecuteRoomMachine[moduleData.RoomId].Remove(machineId);
                    }

                    GlobalVariables.DicExecuteRoomMachine[moduleData.RoomId].Add(machineId);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChkAutoFinish_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ChkAutoFinish.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (ChkAutoFinish.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ChkAutoFinish.Name;
                    csAddOrUpdate.VALUE = (ChkAutoFinish.Checked ? "1" : "");
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

        private bool CheckIsDisable()
        {
            bool result = false;
            try
            {
                if (listServiceADO != null && listServiceADO.Count > 0 && listServiceADO.Exists(o => o.MustHavePressBeforeExecute))
                {
                    var serviceName = listServiceADO.Where(o => o.MustHavePressBeforeExecute).Select(s => s.TDL_SERVICE_NAME).ToList();
                    DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.DichVuChuaKeThuocVatTu, string.Join(",", serviceName)));
                    if (serviceName.Count == listServiceADO.Count)
                    {
                        return true;
                    }
                }

                //nếu hoàn thành sẽ khóa lại.
                //nếu không phải ng thực hiện sẽ khóa lại
                //nếu ko phải admin sẽ khóa lại
                //nếu hoàn thành và không phải ng xử lý và ko phải admin thì disable
                if (currentServiceReq != null &&
                    currentServiceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                //&& currentServiceReq.EXECUTE_LOGINNAME != Loginname && !HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(Loginname))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                result = true;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ProcessChangeImageDev(DevExpress.XtraRichEdit.RichEditControl txtDescription, ADO.ImageADO imageADO)
        {
            try
            {
                if (txtDescription == null || imageADO == null) return;

                if (txtDescription.IsSelectionInTextBox)
                {
                    if (txtDescription.Document.Shapes != null && txtDescription.Document.Shapes.Count > 0)
                    {
                        SubDocument sd = txtDescription.Document.Selection.BeginUpdateDocument();
                        string currentRtf = sd.GetRtfText(sd.Range);
                        foreach (Shape txt in txtDescription.Document.Shapes)
                        {
                            if (txt.TextBox != null)
                            {
                                SubDocument textBoxDocument = txt.TextBox.Document;
                                //có ảnh mới đổi hoặc không có ảnh nhưng không chứa key
                                if (textBoxDocument.Images.Count <= 0 && textBoxDocument.GetText(textBoxDocument.Range).Contains("IMAGE_DATA_")) continue;

                                string tbTextValue = textBoxDocument.GetRtfText(textBoxDocument.Range);
                                //so sánh dữ liệu trong shape với khu vực Document.Selection
                                // giống nhau thì chèn ảnh
                                if (tbTextValue == currentRtf)
                                {
                                    textBoxDocument.Delete(textBoxDocument.Range);
                                    try
                                    {
                                        Image imgFill = ResizeImage(imageADO.IMAGE_DISPLAY, (int)(txt.Size.Width / 3) - 30, (int)(txt.Size.Height / 3) - 30);
                                        textBoxDocument.Images.Insert(textBoxDocument.Range.Start, imgFill);

                                        imageADO.IsChecked = false;
                                        imageADO.STTImage = null;
                                        break;
                                    }
                                    catch (Exception)
                                    { }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (txtDescription.Document.Selection != null)
                    {
                        var rangeImage = txtDescription.Document.Selection;
                        if (rangeImage.Start != rangeImage.End)
                        {
                            foreach (var item in txtDescription.Document.Images)
                            {
                                if (rangeImage.Contains(item.Range.Start) || rangeImage.Contains(item.Range.End))
                                {
                                    txtDescription.Document.Delete(rangeImage);
                                    txtDescription.Document.Images.Insert(rangeImage.Start, ResizeImage(imageADO.IMAGE_DISPLAY, 250, 140));

                                    imageADO.IsChecked = false;
                                    imageADO.STTImage = null;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.KhongXacDinhDuocVungChuaAnh);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessChangeImageTel(Telerik.WinControls.UI.RadRichTextEditor radRichTextEditor, ADO.ImageADO imageADO)
        {
            try
            {
                bool notImage = true;
                if (radRichTextEditor != null && imageADO != null)
                {
                    if (!radRichTextEditor.Document.Selection.IsEmpty && radRichTextEditor.Document.Selection.Ranges.Count == 1)
                    {
                        var ImageInline = radRichTextEditor.Document.Selection.Ranges.First.StartPosition.ToString();
                        if (ImageInline == "Telerik.WinForms.Documents.Model.ImageInline")
                        {
                            var stream = new System.IO.MemoryStream();
                            ResizeImage(imageADO.IMAGE_DISPLAY, 250, 140).Save(stream, ImageFormat.Jpeg);
                            stream.Position = 0;

                            Telerik.WinForms.Documents.DocumentPosition positionImageStart = new Telerik.WinForms.Documents.DocumentPosition(radRichTextEditor.Document.Selection.Ranges.First.StartPosition, true);
                            Telerik.WinForms.Documents.DocumentPosition positionImageEnd = new Telerik.WinForms.Documents.DocumentPosition(radRichTextEditor.Document.Selection.Ranges.First.EndPosition, true);

                            Telerik.WinForms.Documents.TextSearch.TextRange sRange = new Telerik.WinForms.Documents.TextSearch.TextRange(positionImageStart, positionImageEnd);
                            radRichTextEditor.Document.Selection.AddSelectionStart(sRange.StartPosition);
                            radRichTextEditor.Document.Selection.AddSelectionEnd(sRange.EndPosition);
                            radRichTextEditor.InsertImage(stream, "jpeg");
                            sRange.StartPosition.Dispose();
                            sRange.EndPosition.Dispose();
                            notImage = false;

                            imageADO.IsChecked = false;
                            imageADO.STTImage = null;
                        }
                    }
                }

                if (notImage)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.KhongXacDinhDuocVungChuaAnh);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private long GetZoom()
        {
            long result = 100;
            try
            {
                if (this.panelDescription != null && this.panelDescription.Controls != null)
                {
                    Control edit = null;
                    foreach (var item in this.panelDescription.Controls)
                    {
                        if (item is UcWord)
                        {
                            var control = (UcWord)item;
                            edit = control.txtDescription;
                            break;
                        }
                        else if (item is UcWordFull)
                        {
                            var control = (UcWordFull)item;
                            edit = control.txtDescription;
                            break;
                        }
                        else if (item is UcWords.UcTelerik)
                        {
                            var control = (UcWords.UcTelerik)item;
                            edit = control.radRichTextEditor1;
                            break;
                        }
                        else if (item is UcWords.UcTelerikFullWord)
                        {
                            var control = (UcWords.UcTelerikFullWord)item;
                            edit = control.radRichTextEditor1;
                            break;
                        }
                    }

                    result = WordProcess.zoomFactor(edit);
                }
            }
            catch (Exception ex)
            {
                result = 100;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void chkAttachImage_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkAttachImage.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkAttachImage.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkAttachImage.Name;
                    csAddOrUpdate.VALUE = (chkAttachImage.Checked ? "1" : "");
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

        private void chkClose_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkClose.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkClose.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkClose.Name;
                    csAddOrUpdate.VALUE = (chkClose.Checked ? "1" : "");
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

        private void chkPrint_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPrint.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrint.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPrint.Name;
                    csAddOrUpdate.VALUE = (chkPrint.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();

                if (true)
                {

                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitDrButtonOther()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemSereServTemp = new DXMenuItem(Resources.ResourceLanguageManager.GetValue("IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__BTN_SERE_SERV_TEMP_LIST"), new EventHandler(btnSereServTempList_Click));
                menu.Items.Add(itemSereServTemp);

                DXMenuItem itemTrackingList = new DXMenuItem(Resources.ResourceLanguageManager.GetValue("IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__BTN_TRACKING_LIST"), new EventHandler(btnTrackingList_Click));
                menu.Items.Add(itemTrackingList);

                DXMenuItem itemAssBlood = new DXMenuItem(Resources.ResourceLanguageManager.GetValue("IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__BTN_ASSIGN_PAAN_TOOLTIP"), new EventHandler(btnAssignPaan_Click));
                menu.Items.Add(itemAssBlood);

                DXMenuItem itemContentLibrary = new DXMenuItem(Resources.ResourceLanguageManager.GetValue("IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__BTN_SERE_SERV_CONTENT_LIBRARY"), new EventHandler(btnContentLibrary_Click));
                menu.Items.Add(itemContentLibrary);

                dropDownButton.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dropDownButton_Click(object sender, EventArgs e)
        {
            try
            {
                dropDownButton.ShowDropDown();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void cboMachineOption_EditValueChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (isNotLoadWhileChangeControlStateInFirst)
        //        {
        //            return;
        //        }
        //        WaitingManager.Show();
        //        HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == cboMachineOption.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
        //        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
        //        if (csAddOrUpdate != null)
        //        {
        //            csAddOrUpdate.VALUE = (cboMachineOption.EditValue ?? "0").ToString();
        //        }
        //        else
        //        {
        //            csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
        //            csAddOrUpdate.KEY = cboMachineOption.Name;
        //            csAddOrUpdate.VALUE = (cboMachineOption.EditValue ?? "0").ToString();
        //            csAddOrUpdate.MODULE_LINK = moduleLink;
        //            if (this.currentControlStateRDO == null)
        //                this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
        //            this.currentControlStateRDO.Add(csAddOrUpdate);
        //        }
        //        this.controlStateWorker.SetData(this.currentControlStateRDO);
        //        WaitingManager.Hide();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void InitCboMachineOption()
        //{
        //    try
        //    {
        //        List<ComboADO> combo = new List<ComboADO>();

        //        combo.Add(new ComboADO(1, Resources.ResourceLanguageManager.GetValue("IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__COMBO_MACHINE_OPTION_WARNING")));
        //        combo.Add(new ComboADO(2, Resources.ResourceLanguageManager.GetValue("IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__COMBO_MACHINE_OPTION_BLOCK")));

        //        List<ColumnInfo> columnInfos = new List<ColumnInfo>();
        //        columnInfos.Add(new ColumnInfo("statusName", "Loại", 50, 2));
        //        ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, true, 50);
        //        ControlEditorLoader.Load(cboMachineOption, combo, controlEditorADO);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void cboMachineOption_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
        //        {
        //            cboMachineOption.EditValue = null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void tileView1_ContextButtonCustomize(object sender, TileViewContextButtonCustomizeEventArgs e)
        {
            try
            {
                var item = (ADO.ImageADO)tileView1.GetRow(e.RowHandle);
                if ((item != null && item.CREATOR == Loginname && e.Item.Name == "btnDelete") || e.Item.Name == "btnEditCaption")
                {
                    e.Item.Visibility = DevExpress.Utils.ContextItemVisibility.Visible;
                }
                else
                {
                    e.Item.Visibility = DevExpress.Utils.ContextItemVisibility.Hidden;
                }
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
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkSign.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkSign.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkSign.Name;
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkSaveImageToFile_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                if (chkSaveImageToFile.Checked)
                {
                    SelectFolder();
                }
                else
                {
                    this.SelectedFolderForSaveImage = null;
                }

                SaveAllImage();

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkSaveImageToFile.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkSaveImageToFile.Checked ? string.Format("{0}|{1}", "1", this.SelectedFolderForSaveImage) : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkSaveImageToFile.Name;
                    csAddOrUpdate.VALUE = (chkSaveImageToFile.Checked ? string.Format("{0}|{1}", "1", this.SelectedFolderForSaveImage) : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
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

        private void chkPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!chkPrint.Checked)
                {
                    chkForPreview.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkForPreview_Click(object sender, EventArgs e)
        {
            try
            {
                if (!chkForPreview.Checked)
                {
                    chkPrint.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtConclude_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (chkUpper.Checked)
                {
                    e.KeyChar = char.Parse(e.KeyChar.ToString().ToUpper());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkUpper_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkUpper.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkUpper.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkUpper.Name;
                    csAddOrUpdate.VALUE = (chkUpper.Checked ? "1" : "");
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

        private void timerLoadEkip_Tick()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("timerLoadEkip_Tick. 1");
                this.ComboExecuteRole();
                this.ComboEkipTemp(cboEkipUserTemp);
                this.LoadComboDepartment(cboEkipDepartment);
                this.LoadDataToGridComboDepartment();
                this.ComboAcsUser();
                Inventec.Common.Logging.LogSystem.Debug("timerLoadEkip_Tick. 2");
                StopTimer(moduleData.ModuleLink, "timerLoadEkip");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboEkipUserTemp_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboEkipUserTemp.EditValue != null)
                    {
                        cboEkipUserTemp.Properties.Buttons[1].Visible = true;
                        var data = this.ekipTemps.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboEkipUserTemp.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            LoadGridEkipUserFromTemp(data.ID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewEkip_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                var data = (HisEkipUserADO)gridViewEkip.GetFocusedRow();
                if (e.Column.FieldName == "LOGINNAME")
                {
                    this.gridControlEkip.RefreshDataSource();
                    SetDepartment(data);
                }
                IsDataEkipUser = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewEkip_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "BtnDelete")
                {
                    int rowSelected = Convert.ToInt32(e.RowHandle);
                    int action = Inventec.Common.TypeConvert.Parse.ToInt32((gridViewEkip.GetRowCellValue(e.RowHandle, "Action") ?? "").ToString());
                    if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                    {
                        e.RepositoryItem = repositoryItemBtnAddEkip;
                    }
                    //else if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                    //{
                    //    e.RepositoryItem = repositoryItemBtnDeleteEkip;
                    //}
                }
                else if (e.Column.FieldName == "BtnDelete1")
                {
                    e.RepositoryItem = repositoryItemBtnDeleteEkip;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewEkip_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "USERNAME")
                    {
                        try
                        {
                            string loginname = (view.GetRowCellValue(e.ListSourceRowIndex, "LOGINNAME") ?? "").ToString();
                            ACS_USER data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS_USER>().SingleOrDefault(o => o.LOGINNAME == loginname);
                            e.Value = data.USERNAME;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi hien thi gia tri cot USERNAME", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewEkip_FocusedColumnChanged(object sender, FocusedColumnChangedEventArgs e)
        {
            try
            {
                if (e.FocusedColumn.FieldName == "LOGINNAME")
                {
                    gridViewEkip.ShowEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewEkip_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                HisEkipUserADO data = view.GetFocusedRow() as HisEkipUserADO;
                if (view.FocusedColumn.FieldName == "LOGINNAME" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    LoadUser(data, editor);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadUser(HisEkipUserADO data, GridLookUpEdit editor)
        {
            try
            {
                List<string> loginNames = new List<string>();
                if (data != null && data.EXECUTE_ROLE_ID > 0)
                {

                    var executeRoleUserTemps = executeRoleUsers != null ? executeRoleUsers.Where(o => o.EXECUTE_ROLE_ID == data.EXECUTE_ROLE_ID).ToList() : null;
                    if (executeRoleUserTemps != null && executeRoleUserTemps.Count > 0)
                    {
                        loginNames = executeRoleUserTemps.Select(o => o.LOGINNAME).Distinct().ToList();
                    }
                    if (data.LOGINNAME != null)
                    {
                        if (key == "1")
                        {
                            if (loginNames.Contains(data.LOGINNAME))
                            {
                                editor.EditValue = data.LOGINNAME;
                            }
                            else
                            {
                                editor.EditValue = null;
                                data.LOGINNAME = null;
                            }
                        }
                        else
                        {
                            editor.EditValue = data.LOGINNAME;
                        }



                        //editor.Text = loginNames.Contains(data.LOGINNAME) ? data.LOGINNAME : null;
                    }
                }

                //ComboAcsUser(editor, loginNames);

                //SetDepartment(data);
                //gridViewEkip.RefreshData();

                if (data != null && data.DEPARTMENT_ID > 0)
                {

                    var depaloginNames = BackendDataWorker.Get<V_HIS_EMPLOYEE>().Where(o => o.DEPARTMENT_ID == data.DEPARTMENT_ID || o.DEPARTMENT_ID == null).Select(i => i.LOGINNAME).ToList();
                    if (depaloginNames != null && depaloginNames.Count > 0)
                    {
                        if (loginNames.Count > 0)
                        {
                            loginNames = loginNames.Where(o => depaloginNames.Contains(o)).ToList();
                        }
                        else
                        {
                            loginNames = depaloginNames;
                        }
                    }
                    if (data.LOGINNAME != null)
                    {
                        if (key == "1")
                        {
                            if (loginNames.Contains(data.LOGINNAME))
                            {
                                editor.EditValue = data.LOGINNAME;
                            }
                            else
                            {
                                editor.EditValue = null;
                                data.LOGINNAME = null;
                            }
                        }
                        else
                        {
                            editor.EditValue = data.LOGINNAME;
                        }



                        //editor.Text = loginNames.Contains(data.LOGINNAME) ? data.LOGINNAME : null;
                    }
                }
                ComboAcsUser(editor, loginNames);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void repositoryItemBtnAddEkip_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                List<HisEkipUserADO> ekipUserAdoTemps = new List<HisEkipUserADO>();
                var ekipUsers = gridViewEkip.DataSource as List<HisEkipUserADO>;
                if (ekipUsers == null || ekipUsers.Count < 1)
                {
                    HisEkipUserADO ekipUserAdoTemp = new HisEkipUserADO();
                    ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                    ekipUserAdoTemps.Add(ekipUserAdoTemp);
                }
                else
                {
                    HisEkipUserADO participant = new HisEkipUserADO();
                    participant.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                    ekipUserAdoTemps.AddRange(ekipUsers);
                    ekipUserAdoTemps.Add(participant);
                }

                gridControlEkip.DataSource = null;
                gridControlEkip.DataSource = ekipUserAdoTemps;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemBtnDeleteEkip_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var ekipUsers = gridControlEkip.DataSource as List<HisEkipUserADO>;
                var ekipUser = (HisEkipUserADO)gridViewEkip.GetFocusedRow();
                if (ekipUser != null)
                {
                    if (ekipUsers.Count > 0)
                    {
                        ekipUsers.Remove(ekipUser);

                        if (ekipUsers.Count == 0)
                        {
                            HisEkipUserADO participant = new HisEkipUserADO();
                            participant.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                            ekipUsers.Add(participant);
                        }
                        else
                        {
                            var check = ekipUsers.Where(o => o.Action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd).ToList();
                            if (check != null && check.Count < 1)
                            {
                                ekipUsers.FirstOrDefault().Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                            }
                        }
                        gridControlEkip.DataSource = null;
                        gridControlEkip.DataSource = ekipUsers;
                    }
                    if (ekipUsers.Count == 0)
                    {
                        HisEkipUserADO participant = new HisEkipUserADO();
                        participant.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                        ekipUsers.Add(participant);
                        gridControlEkip.DataSource = null;
                        gridControlEkip.DataSource = ekipUsers;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEkipDepartment_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboEkipDepartment.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEkipDepartment_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboEkipDepartment.EditValue != null)
                    {
                        var dataEkipList = (List<HisEkipUserADO>)gridControlEkip.DataSource;
                        if (dataEkipList != null && dataEkipList.Count > 0)
                        {
                            Parallel.ForEach(dataEkipList.Where(f => f.ID >= 0), l => l.DEPARTMENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboEkipDepartment.EditValue.ToString())));
                        }

                        gridControlEkip.DataSource = null;
                        gridControlEkip.DataSource = dataEkipList;
                        gridControlEkip.RefreshDataSource();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEkipUserTemp_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                try
                {
                    if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                    {
                        cboEkipUserTemp.Properties.Buttons[1].Visible = false;
                        cboEkipUserTemp.EditValue = null;
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

        private void gridControlEkip_Leave(object sender, EventArgs e)
        {
            try
            {
                this.dicEkipUser[this.sereServ.ID] = gridControlEkip.DataSource as List<HisEkipUserADO>;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSaveEkipTemp_Click(object sender, EventArgs e)
        {
            try
            {
                var ekipUsers = gridControlEkip.DataSource as List<HisEkipUserADO>;
                bool hasInvalid = ekipUsers
                   .Where(o => String.IsNullOrEmpty(o.LOGINNAME)
                       || o.EXECUTE_ROLE_ID <= 0 || o.EXECUTE_ROLE_ID == null).FirstOrDefault() != null ? true : false;
                if (hasInvalid)
                {
                    MessageBox.Show("Thiếu thông tin kip thực hiện");
                    return;
                }

                var groupLoginname = ekipUsers.Where(o => !String.IsNullOrWhiteSpace(o.LOGINNAME)).GroupBy(o => o.LOGINNAME).ToList();
                if (groupLoginname != null && groupLoginname.Count > 0)
                {
                    List<string> messError = new List<string>();
                    foreach (var item in groupLoginname)
                    {
                        if (item.Count() > 1)
                        {
                            var lstExeRole = lstExecuteRole.Where(o => item.Select(s => s.EXECUTE_ROLE_ID).Contains(o.ID)).ToList();
                            messError.Add(string.Format(ResourceMessage.TaiKhoanDuocGanNhieuVaiTro, item.Key, string.Join(",", lstExeRole.Select(s => s.EXECUTE_ROLE_NAME))));
                        }
                    }

                    if (messError.Count > 0)
                    {
                        XtraMessageBox.Show(string.Join("\n", messError), ResourceMessage.ThongBao);
                        return;
                    }
                }

                EkipTemp.frmEkipTemp frm = new EkipTemp.frmEkipTemp(ekipUsers, RefeshDataEkipTemp, this.moduleData);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnShowConfig_Click(object sender, EventArgs e)
        {
            try
            {
                ShowHideOptionCamera();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAutoCapture_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                if (chkAutoCapture.Checked)
                {
                    CheckEdit editor = sender as CheckEdit;
                    if (spnTotalCapture.Font.Size > 11)
                    {

                        popupControlContainerTextEdit.Height = 70;
                        popupControlContainerTextEdit.Width = 500;
                    }
                    else
                    {
                        popupControlContainerTextEdit.Height = 56;
                        popupControlContainerTextEdit.Width = 391;
                    }

                    Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                    popupControlContainerTextEdit.ShowPopup(new System.Drawing.Point(buttonPosition.X - 200, buttonPosition.Bottom + 80));




                    this.currentContainerClick = ContainerClick.AutoCapture;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkAutoCapture.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkAutoCapture.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkAutoCapture.Name;
                    csAddOrUpdate.VALUE = (chkAutoCapture.Checked ? "1" : "");
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

        private void btnDongY_Click(object sender, EventArgs e)
        {
            try
            {

                switch (this.currentContainerClick)
                {
                    case ContainerClick.AutoCapture:
                        if (isNotLoadWhileChangeControlStateInFirst)
                        {
                            return;
                        }
                        if (Int32.Parse(spnTotalCapture.Value.ToString()) <= 0 || double.Parse(spnTotalTimeToCapture.Value.ToString()) <= 0)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Giá trị thiết lập không phù hợp!", "Thông báo", MessageBoxButtons.OK);

                        }
                        else
                        {
                            popupControlContainerTextEdit.HidePopup();
                            WaitingManager.Show();
                            HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateCapture = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == spnTotalCapture.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                            HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateTime = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == spnTotalTimeToCapture.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;

                            if (csAddOrUpdateCapture != null && csAddOrUpdateTime != null)
                            {
                                csAddOrUpdateCapture.VALUE = spnTotalCapture.Value.ToString();
                                csAddOrUpdateTime.VALUE = spnTotalTimeToCapture.Value.ToString();
                            }
                            else
                            {
                                csAddOrUpdateCapture = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                                csAddOrUpdateCapture.KEY = spnTotalCapture.Name;
                                csAddOrUpdateCapture.VALUE = spnTotalCapture.Value.ToString();
                                csAddOrUpdateCapture.MODULE_LINK = moduleLink;

                                csAddOrUpdateTime = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                                csAddOrUpdateTime.KEY = spnTotalTimeToCapture.Name;
                                csAddOrUpdateTime.VALUE = spnTotalTimeToCapture.Value.ToString();
                                csAddOrUpdateTime.MODULE_LINK = moduleLink;

                                if (this.currentControlStateRDO == null)
                                    this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                                this.currentControlStateRDO.Add(csAddOrUpdateCapture);
                                this.currentControlStateRDO.Add(csAddOrUpdateTime);
                            }
                            this.controlStateWorker.SetData(this.currentControlStateRDO);
                            WaitingManager.Hide();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnBoQua_Click(object sender, EventArgs e)
        {
            try
            {
                chkAutoCapture.Checked = false;
                this.currentContainerClick = ContainerClick.None;
                popupControlContainerTextEdit.HidePopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void tileView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                //ShowName
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (ADO.ImageADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "ShowName")
                        {
                            try
                            {
                                e.Value = !String.IsNullOrWhiteSpace(data.CAPTION) ? data.CAPTION : data.FileName;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void chkKeTieuHao_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkKeTieuHao.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkKeTieuHao.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkKeTieuHao.Name;
                    csAddOrUpdate.VALUE = (chkKeTieuHao.Checked ? "1" : "");
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

        private void btnKhaiBaoVTTH_Click(object sender, EventArgs e)
        {
            try
            {
                Library.MediStockExpend.MediStockExpendProcessor.GetMediStock(moduleData.RoomId, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemMachineId_ButtonClick_1(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    var edit = sender as GridLookUpEdit;
                    edit.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetTimeSystem()
        {
            try
            {
                timeSync = new BackendAdapter(new CommonParam()).Get<TimerSDO>(AcsRequestUriStore.ACS_TIMER__SYNC, ApiConsumers.AcsConsumer, 1, new CommonParam());
                currentTimer = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(timeSync.LocalTime) ?? DateTime.Now;
                Inventec.Common.Logging.LogSystem.Debug("currentTimer________" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => timeSync), timeSync));
                Inventec.Common.Logging.LogSystem.Debug("DATETIME________" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now)), Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now)));

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
                if (dtEndTime.SelectionStart > 0)
                {
                    StopTimer(moduleData.ModuleLink, "timer1");
                }
                else
                {
                    currentTimer = currentTimer.AddSeconds(1);
                    if (sereServExt != null && !sereServExt.END_TIME.HasValue || sereServExt == null)
                    {
                        dtEndTime.DateTime = currentTimer;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtEndTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (dtEndTime.EditValue != null && dtEndTime.DateTime != DateTime.MinValue)
                    currentTimer = dtEndTime.DateTime;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtEndTime_Leave(object sender, EventArgs e)
        {
            try
            {
                bool IsFocus = false;
                if (!Config.AppConfigKeys.IsAllowToEditFinishTimeGreaterThanCurrentTime)
                {
                    if (dtEndTime.EditValue != null && dtEndTime.DateTime != DateTime.MinValue && dtEndTime.DateTime > DateTime.Now)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show("Thời gian kết thúc lớn hơn thời gian hiện tại?", ResourceMessage.ThongBao, MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK)
                        {
                            IsFocus = true;
                            dtEndTime.DateTime = DateTime.Now;
                            IsFocus = false;
                        }
                    }
                }
                if (!IsFocus)
                {
                    currentTimer = dtEndTime.DateTime;
                    dtEndTime.SelectionStart = 0;
                    StartTimer(moduleData.ModuleLink, "timer1");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnStateForInformationUser_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

        }

        private void SavePin()
        {
            try
            {
                InformationADO ado = new InformationADO();
                //if (cboEkipUserTemp.EditValue != null)
                //    ado.EKIP_TEMP_ID = Int64.Parse(cboEkipUserTemp.EditValue.ToString());
                //if (cboEkipDepartment.EditValue != null)
                //    ado.EKIP_DEPARTMENT_ID = Int64.Parse(cboEkipDepartment.EditValue.ToString());
                var lstEkipUser = ProcessEkipUserAdo();

                if (IsDataEkipUser)
                    return;
                if (lstEkipUser != null && lstEkipUser.Count > 0)
                    ado.ListEkipUser = lstEkipUser;
                string textJson = JsonConvert.SerializeObject(ado);
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateValue = (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0) ? this.currentBySessionControlStateRDO.Where(o => o.KEY == "InformationADO" && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdateValue != null)
                {
                    csAddOrUpdateValue.VALUE = IsPin ? textJson : "";
                }
                else
                {
                    csAddOrUpdateValue = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdateValue.KEY = "InformationADO";
                    csAddOrUpdateValue.VALUE = IsPin ? textJson : "";
                    csAddOrUpdateValue.MODULE_LINK = moduleLink;
                    if (this.currentBySessionControlStateRDO == null)
                        this.currentBySessionControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentBySessionControlStateRDO.Add(csAddOrUpdateValue);
                }
                this.controlStateWorker.SetDataBySession(this.currentBySessionControlStateRDO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DisposeData()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("DisposeData");
                SavePin();
                ServiceReqConstruct = null;
                sereServ = null;
                sereServExt = null;
                currentSarPrint = null;
                currentServiceReq = null;
                listTemplate = null;
                currentSereServExt = null;
                dicParam = null;
                currentDataClick = null;
                RefreshData = null;
                listServiceADO = null;
                listHisTextLib = null;
                mainSereServ = null;
                listServiceADOForAllInOne = null;
                dicSereServExt = null;
                dicSarPrint = null;
                dicEkipUser = null;
                dicSereServPttt = null;
                TreatmentWithPatientTypeAlter = null;
                ListMachine = null;
                ListServiceMachine = null;
                sereServ = null;
                lstService = null;
                lstDepartment = null;
                lstExecuteRole = null;
                if (dicImage != null)
                {
                    try
                    {
                        foreach (var item in dicImage)
                        {
                            if (item.Value != null)
                            {
                                item.Value.Dispose();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }
                dicImage = null;

                if (listImage != null && listImage.Count > 0)
                {
                    try
                    {
                        foreach (var item in listImage)
                        {
                            if (item.IMAGE_DISPLAY != null)
                            {
                                item.IMAGE_DISPLAY.Dispose();
                            }

                            if (item.streamImage != null)
                            {
                                item.streamImage.Dispose();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }
                listImage = null;

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCameramax_Click(object sender, EventArgs e)
        {
            try
            {
                if ((int)cboConnectionType.EditValue == 2)
                {
                    this.ucCameraDXC1.Stop();
                }
                else
                {
                    this.ucCamera1.Stop();
                }
                frmCamera frm = new frmCamera(moduleData, lstService, listServiceADOForAllInOne, sereServ, listImage, listResult =>
                {
                    listImage = listResult;
                    ProcessLoadGridImage(listImage);
                    StartCamera();
                }, (int)cboConnectionType.EditValue, currentServiceReq, camMonikerString, Loginname, SelectedFolderForSaveImage, chkAutoCapture.Checked, Int32.Parse(spnTotalCapture.Value.ToString()));
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPdf_Click(object sender, EventArgs e)
        {

            try
            {
                frmAttackPdf frm = new frmAttackPdf(currentSsPdf, list =>
                {
                    currentSsPdf = list;
                    currentSsPdf.ForEach(s => { s.IsNow = false; });
                });
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void repositoryItemCboPosition_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                LookUpEdit edit = sender as LookUpEdit;
                if (edit == null) return;
                if (edit.EditValue != null)
                {
                    if ((edit.EditValue ?? 0).ToString() != (edit.OldEditValue ?? 0).ToString())
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = gridViewEkip as DevExpress.XtraGrid.Views.Grid.GridView;
                        var data = (HisEkipUserADO)gridViewEkip.GetFocusedRow();
                        data.EXECUTE_ROLE_ID = Convert.ToInt64(edit.EditValue);
                        GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                        if (editor != null)
                        {
                            LoadUser(data, editor);
                        }
                        else
                        {
                            GridLookUpEdit newEditor = new GridLookUpEdit();
                            LoadUser(data, newEditor);
                        }
                        if (data.LOGINNAME == null && (key == "1"))
                        {
                            view.SetRowCellValue(view.FocusedRowHandle, view.Columns["LOGINNAME"], null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPreYhct_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionYHCT").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionYHCT");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    V_HIS_SERE_SERV sereServInput = new V_HIS_SERE_SERV();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV>(sereServInput, sereServ);
                    AssignPrescriptionADO assignServiceADO = new AssignPrescriptionADO(currentServiceReq.TREATMENT_ID, currentServiceReq.INTRUCTION_TIME, this.currentServiceReq.ID, sereServInput);
                    var pres = currentServiceReq;
                    assignServiceADO.TreatmentCode = pres.TDL_TREATMENT_CODE;
                    assignServiceADO.GenderName = pres.TDL_PATIENT_GENDER_NAME;
                    assignServiceADO.PatientDob = pres.TDL_PATIENT_DOB;
                    assignServiceADO.PatientName = pres.TDL_PATIENT_NAME;
                    listArgs.Add(assignServiceADO);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();


                    //chỉ định xong sẽ load lại dữ liệu
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void xtraTabControl1_CustomHeaderButtonClick(object sender, DevExpress.XtraTab.ViewInfo.CustomHeaderButtonEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Glyph)
                {
                    if (e.Button.Index == 0)
                    {
                        xtraTabControl1.CustomHeaderButtons[0].Visible = false;
                        xtraTabControl1.CustomHeaderButtons[1].Visible = true;
                        IsPin = false;
                    }
                    else if (e.Button.Index == 1)
                    {
                        xtraTabControl1.CustomHeaderButtons[0].Visible = true;
                        xtraTabControl1.CustomHeaderButtons[1].Visible = false;
                        IsPin = true;
                    }
                    xtraTabControl1.Update();
                    HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == xtraTabControl1.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                    if (csAddOrUpdate != null)
                    {
                        csAddOrUpdate.VALUE = IsPin ? "1" : "";
                    }
                    else
                    {
                        csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                        csAddOrUpdate.KEY = xtraTabControl1.Name;
                        csAddOrUpdate.VALUE = IsPin ? "1" : "";
                        csAddOrUpdate.MODULE_LINK = moduleLink;
                        if (this.currentControlStateRDO == null)
                            this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                        this.currentControlStateRDO.Add(csAddOrUpdate);
                    }
                    this.controlStateWorker.SetData(this.currentControlStateRDO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }

    class EkipNameException : Exception
    {
        public EkipNameException() { }
    }
}

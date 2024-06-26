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
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

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
        private Dictionary<string, object> dicParam;
        private Dictionary<string, Image> dicImage;
        private List<ADO.ImageADO> listImage;
        private Common.DelegateRefresh RefreshData;
        private List<ADO.ImageADO> imageLoad;
        private List<ADO.ServiceADO> listServiceADO;
        string camMonikerString;

        private ADO.ServiceADO mainSereServ;
        private List<ADO.ServiceADO> listServiceADOForAllInOne = new List<ADO.ServiceADO>();

        private List<HisEkipUserADO> ekipUserAdos = new List<HisEkipUserADO>();

        private Dictionary<long, HIS_SERE_SERV_EXT> dicSereServExt = new Dictionary<long, HIS_SERE_SERV_EXT>(); //sereServId,HIS_SERE_SERV_EXT
        private Dictionary<long, SAR.EFMODEL.DataModels.SAR_PRINT> dicSarPrint = new Dictionary<long, SAR.EFMODEL.DataModels.SAR_PRINT>();//sereServExt.ID, SAR_PRINT

        private bool isPressButtonSave;
        private bool isPressButtonSaveNClose;
        private ADO.PatientADO patient;
        private HisTreatmentWithPatientTypeInfoSDO TreatmentWithPatientTypeAlter;

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
        //int positionFinded = 0;
        int positionHandle = -1;
        bool? isExecuter = null, isReadResult = null;
        ADO.ServiceADO currentServiceADO;
        V_HIS_SERE_SERV_PTTT sereServPTTT;
        string positionProtect = "";

        bool isWordFull = false;
        UcWordFull wordFullDocument;
        UcWord wordDocument;

        List<string> keyPrint = new List<string>() { "<#CONCLUDE_PRINT;>", "<#NOTE_PRINT;>", "<#DESCRIPTION_PRINT;>", "<#CURRENT_USERNAME_PRINT;>" };
        GridColumn lastColumnSS = null;
        int lastRowHandleSS = -1;
        ToolTipControlInfo lastInfoSS = null;

        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        const string moduleLink = "HIS.Desktop.Plugins.ServiceExecute";
        const string trackBarZoomName = "trackBarZoom";
        long trackBarZoom;

        bool isNotExecuteWhileChangedZoomEditor;
        #endregion

        #region Construct
        public UCServiceExecute()
        {
            InitializeComponent();
        }

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
                this.wordDocument = new UcWord(EditorZoomChanged);
                this.panelDescription.Controls.Add(this.wordDocument);
                this.wordDocument.Dock = DockStyle.Fill;

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
        #endregion

        #region Private method
        #region load
        private void UCServiceExecute_Load(object sender, EventArgs e)
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;
                LoadKeysFromlanguage();
                SetDefaultValueControl();
                InitControlState();
                CreateThreadLoadDataDefault();
                FillDataCombo();
                FillDataToGrid();
                ProcessPatientInfo();
                if (this.sereServ == null || (ConnectImageOption != "1" && this.sereServ != null && String.IsNullOrEmpty(this.sereServ.TDL_PACS_TYPE_CODE)))
                {
                    LoadDataImageLocal();
                }

                FillDataToInformationSurg();
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
                LoadDataToCombo();
                EnableControlCamera(false);
                InitRestoreLayoutGridViewFromXml(gridViewSereServ);

                CheckValidPress();
                InitCameraDefault();
                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void InitControlState()
        {
            try
            {
                //layoutControlItem21.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                //layoutControlItem27.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                //layoutControlItem30.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

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
                        else if (item.KEY == trackBarZoomName)
                        {
                            isNotExecuteWhileChangedZoomEditor = true;
                            trackBarZoom = Inventec.Common.TypeConvert.Parse.ToInt64(item.VALUE);
                            if (this.isWordFull)
                            {
                                this.wordFullDocument.txtDescription.ActiveView.ZoomFactor = (float)(Inventec.Common.TypeConvert.Parse.ToDecimal(item.VALUE) / 100);
                            }
                            else
                            {
                                this.wordDocument.txtDescription.ActiveView.ZoomFactor = (float)(Inventec.Common.TypeConvert.Parse.ToDecimal(item.VALUE) / 100);
                            }
                            isNotExecuteWhileChangedZoomEditor = false;
                        }
                    }
                }
                else
                {
                    isNotExecuteWhileChangedZoomEditor = true;
                    WordProcess.zoomFactor(GettxtDescription());
                    if (this.isWordFull)
                    {
                        trackBarZoom = (int)this.wordFullDocument.txtDescription.ActiveView.ZoomFactor * 100;
                    }
                    else
                    {
                        trackBarZoom = (int)this.wordDocument.txtDescription.ActiveView.ZoomFactor * 100;
                    }
                    isNotExecuteWhileChangedZoomEditor = false;
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => trackBarZoom), trackBarZoom)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.wordDocument.txtDescription.ActiveView.ZoomFactor), this.wordDocument.txtDescription.ActiveView.ZoomFactor));

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
                    //TODO
                    autoStart = true;
                    //if (camdivices[0].is)
                    //{

                    //}
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
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServExt.SUBCLINICAL_RESULT_USERNAME), sereServExt.SUBCLINICAL_RESULT_USERNAME) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServExt.SUBCLINICAL_RESULT_LOGINNAME), sereServExt.SUBCLINICAL_RESULT_LOGINNAME));
                    List<string> ktv = new List<string>();
                    foreach (var item in dicSereServExt)
                    {
                        if (!String.IsNullOrWhiteSpace(item.Value.SUBCLINICAL_PRES_LOGINNAME))
                        {
                            ktv.Add(string.Format("{0} ({1})", item.Value.SUBCLINICAL_PRES_USERNAME, item.Value.SUBCLINICAL_PRES_LOGINNAME));
                        }
                    }

                    this.LblKtv.Text = ktv != null ? string.Join(",", ktv.Distinct().ToList()) : "";
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
                this.btnAssignPrescription.Text = Resources.ResourceLanguageManager.GetValue(
                "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__BTN_ASSIGN_PRESCRIPTION");
                this.btnAssignService.Text = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__BTN_ASSIGN_SERVICE");
                this.btnFinish.Text = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__BTN_END");
                this.btnPrint.Text = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__BTN_PRINT");
                this.btnSave.Text = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__BTN_SAVE");
                this.btnSereServTempList.Text = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__BTN_SERE_SERV_TEMP_LIST");
                this.lciSereServTempCode.Text = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__LCI_SERE_SERV_TEMP_CODE");
                this.txtConclude.Properties.NullValuePrompt = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__TXT_CONCLUDE__NULL_VALUE");
                this.txtNote.Properties.NullValuePrompt = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__TXT_NOTE__NULL_VALUE");
                //this.btnTuTruc.Text = Resources.ResourceLanguageManager.GetValue(
                //"IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__BTN_TU_TRUC");
                this.btnLoadImage.Text = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__BTN_LOAD_IMAGE");
                this.btnSaveNClose.Text = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__BTN_SAVE_N_CLOSE");
                //this.btnServiceReqMaty.Text = Resources.ResourceLanguageManager.GetValue(
                //    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__BTN_SERVICE_REQ_MATY");
                this.btnCamera.Text = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__BTN_CAMERA");
                this.lciMunberOfFilm.Text = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__LCI_NUMBER_OF_FILM");
                this.layoutControlItem24.Text = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__LCI_SIZE_OF_FILM");
                this.CheckAllInOne.Text = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__CHECK_ALL_IN_ONE");
                //this.BtnDeleteImage.Text = Resources.ResourceLanguageManager.GetValue(
                //    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__BTN_DELETE_IMAGE");
                this.BtnChangeImage.Text = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__BTN_CHANGE_IMAGE");
                this.BtnChooseImage.Text = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__BTN_CHOOSE_IMAGE");
                //this.btnAddImage.Text = Resources.ResourceLanguageManager.GetValue(
                //   "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__BTN_ADD_IMAGE");
                this.btnTuTruc.Text = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__BTN_TU_TRUC");
                this.btnAssignPaan.Text = Resources.ResourceLanguageManager.GetValue(
                   "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__BTN_ASSIGN_PAAN_TEXT");
                this.btnAssignPaan.ToolTip = Resources.ResourceLanguageManager.GetValue(
                   "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__BTN_ASSIGN_PAAN_TOOLTIP");

                this.LciBeginTime.Text = Resources.ResourceLanguageManager.GetValue("IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__LCI_BEGIN_TIME");
                this.LciBeginTime.OptionsToolTip.ToolTip = Resources.ResourceLanguageManager.GetValue("IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__LCI_BEGIN_TIME_TOOL_TIP");
                this.LciEndTime.Text = Resources.ResourceLanguageManager.GetValue("IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__LCI_END_TIME");
                this.LciEndTime.OptionsToolTip.ToolTip = Resources.ResourceLanguageManager.GetValue("IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__LCI_END_TIME_TOOL_TIP");

                //gridView
                this.Gc_ServiceCode.Caption = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__GC_SERVICE_CODE");
                this.Gc_ServiceName.Caption = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__GC_SERVICE_NAME");
                //this.gridColumnName.Caption = Resources.ResourceLanguageManager.GetValue(
                //    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__GC_USER_NAME");
                //this.gridColumnRole.Caption = Resources.ResourceLanguageManager.GetValue(
                //    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__GC_ROLE");
                //this.repositoryItemButtonAdd.Buttons[0].ToolTip = Resources.ResourceLanguageManager.GetValue(
                //    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__GC_BTN_ADD");
                //this.repositoryItemButtonDelete.Buttons[0].ToolTip = Resources.ResourceLanguageManager.GetValue(
                //    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__GC_BTN_DELETE");
                this.repositoryItemButtonServiceReqMaty.Buttons[0].ToolTip = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__GC_BTN_SERVICE_REQ_MATY");
                this.Gc_MachineId.Caption = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__GC_MACHINE_ID");
                //this.repositoryItemButtonSendSancy.Buttons[0].ToolTip = Resources.ResourceLanguageManager.GetValue(
                //    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__GC_BTN_SEND_SANCY");

                this.BtnChangeOption.ToolTip = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__BTN_CHANGE_OPTION");
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
            //Thread Camera = new Thread(StartCamera);
            //dataFillTemplate.Priority = ThreadPriority.Highest;
            try
            {
                //Camera.Start();
                serviceReq.Start();
                listTemplate.Start();
                dataFillTemplate.Start();
                treatment.Start();

                //Camera.Join();
                serviceReq.Join();
                listTemplate.Join();
                dataFillTemplate.Join();
                treatment.Join();
            }
            catch (Exception ex)
            {
                //Camera.Abort();
                serviceReq.Abort();
                listTemplate.Abort();
                dataFillTemplate.Abort();
                treatment.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            //try
            //{
            //    LoadCurrentServiceReq();
            //    ProcessLoadListTemplate();
            //    ProcessDataForTemplate();
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
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
                    //if (this.currentServiceReq == null)
                    //    this.currentServiceReq = new HIS_SERVICE_REQ();

                    //Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(currentServiceReq, ServiceReqConstruct);

                    //if (!currentServiceReq.START_TIME.HasValue)
                    {
                        MOS.Filter.HisServiceReqFilter reqFilter = new MOS.Filter.HisServiceReqFilter();
                        reqFilter.ID = ServiceReqConstruct.ID;
                        var result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(RequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumer.ApiConsumers.MosConsumer, reqFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (result != null && result.Count > 0)
                        {
                            currentServiceReq = result.FirstOrDefault();
                        }
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
                string loginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                //nếu hoàn thành sẽ khóa lại.
                //nếu không phải ng thực hiện sẽ khóa lại
                //nếu ko phải admin sẽ khóa lại
                //nếu hoàn thành và không phải ng xử lý và ko phải admin thì disable
                if (currentServiceReq != null &&
                    currentServiceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT &&
                    currentServiceReq.EXECUTE_LOGINNAME != loginname && !HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(loginname))
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
                this.btnAssignPrescription.Enabled = isEnabled;
                this.btnAssignService.Enabled = isEnabled;
                this.btnCamera.Enabled = isEnabled;
                this.btnFinish.Enabled = isEnabled;
                this.btnLoadImage.Enabled = isEnabled;
                this.btnSave.Enabled = isEnabled;
                this.btnSaveNClose.Enabled = isEnabled;
                this.BtnSaveNPrint.Enabled = isEnabled;
                this.btnSereServTempList.Enabled = isEnabled;
                this.btnTuTruc.Enabled = isEnabled;
                this.cboSereServTemp.Enabled = isEnabled;
                this.txtSereServTempCode.Enabled = isEnabled;
                //this.txtDescription.ReadOnly = isEnabled;
                this.btnAssignPaan.Enabled = isEnabled;
                GettxtDescription().ReadOnly = !isEnabled;
                this.repositoryItemButtonServiceReqMaty.ReadOnly = !isEnabled;
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
                //txtDescription.Text = "";
                ClearDocument();
                txtSereServTempCode.Focus();
                txtSereServTempCode.SelectAll();
                this.ActionType = GlobalVariables.ActionAdd;
                //WordProcess.zoomFactor(GettxtDescription());
                btnPrint.Enabled = false;
                BtnEmr.Enabled = false;
                PACS.PacsCFG.Reload();
                Gc_SendSancy.VisibleIndex = -1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid()
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
                        ado.isSave = ext != null && ext.ID > 0;
                        ado.MACHINE_ID = (ext != null && ext.ID > 0 && ext.MACHINE_ID.HasValue) ? ext.MACHINE_ID.Value : ChoseDataMachine(ado);
                        ado.SoPhieu = String.Format("{0}-{1}", ReduceForCode(item.TDL_SERVICE_REQ_CODE, SERVICE_REQ_CODE__MAX_LENGTH), ReduceForCode(item.TDL_SERVICE_CODE, SERVICE_CODE__MAX_LENGTH));
                        var service = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == item.SERVICE_ID);
                        if (ext != null)
                        {
                            ado.NUMBER_OF_FILM = ext.NUMBER_OF_FILM;
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
                SereServClickRow(listServiceADO[0]);
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
                    //ListRoomMachine = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_ROOM_MACHINE>();
                    //if (ListRoomMachine != null && ListRoomMachine.Count > 0)
                    //{
                    //    ListRoomMachine = ListRoomMachine.Where(o => o.ROOM_ID == moduleData.RoomId).ToList();
                    //}
                }
                var workingRoomIds = WorkPlace.GetRoomIds();

                ListMachine = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MACHINE>();
                if (ListMachine != null && ListMachine.Count > 0)
                    ListMachine = ListMachine.Where(o => o.IS_ACTIVE == 1 && moduleData.RoomId == o.ROOM_ID).ToList();

                ListServiceMachine = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_MACHINE>();
                if (ListServiceMachine != null && ListServiceMachine.Count > 0)
                    ListServiceMachine = ListServiceMachine.Where(o => o.IS_ACTIVE == 1).ToList();

                //ComboAcsUser(repositoryItemCboName);//Họ và tên
                //ComboExecuteRole(repositoryItemCboRole);//Vai trò
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
                //else if (GlobalVariables.listImage != null && GlobalVariables.listImage.Count > 0)
                //{
                //    images.AddRange(GlobalVariables.listImage);
                //}

                if (images != null && images.Count > 0)
                {
                    images = images.OrderByDescending(o => o.Tag).ToList();
                    foreach (var item in images)
                    {
                        string result = null;
                        string text = item.Tag.ToString();
                        if (text != null && text.Length >= 9)
                            result = new StringBuilder().Append(text.Substring(0, 2)).Append(":").Append(text.Substring(2, 2)).Append(":").Append(text.Substring(4, 2)).Append(":").Append(text.Substring(6, 3)).ToString();

                        ADO.ImageADO image = new ADO.ImageADO();
                        image.FileName = result;
                        image.IsChecked = false;
                        image.IMAGE_DISPLAY = item;

                        listImage.Add(image);
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

        private void FillDataToInformationSurg()
        {
            try
            {
                if (ekipUserAdos == null || ekipUserAdos.Count == 0)
                {
                    ekipUserAdos = new List<HisEkipUserADO>();
                    HisEkipUserADO ekipUserAdoTemp = new HisEkipUserADO();
                    ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                    ekipUserAdos.Add(ekipUserAdoTemp);

                    //gridControlEkip.DataSource = ekipUserAdos;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

                    if (ConnectImageOption == "1" && !String.IsNullOrEmpty(this.sereServ.TDL_PACS_TYPE_CODE))
                    {
                        btnCamera.Enabled = false;
                        ThreadLoadImageFromPacs();
                    }
                    else
                        btnCamera.Enabled = true;

                    Inventec.Common.Logging.LogSystem.Debug("3.2.1");
                    ProcessLoadSereServExt(sereServ, ref sereServExt);
                    ProcessLoadSereServExtDescriptionPrint(sereServExt);

                    Inventec.Common.Logging.LogSystem.Debug("3.2.2");

                    cboSizeOfFilm.EditValue = sereServExt != null && sereServExt.FILM_SIZE_ID > 0 ? sereServExt.FILM_SIZE_ID : null;
                    txtNumberOfFilm.Text = sereServExt != null && sereServExt.NUMBER_OF_FILM.HasValue ? sereServ.NUMBER_OF_FILM.ToString() : null;
                    txtSereServTempCode.Text = "";
                    cboSereServTemp.EditValue = null;
                    cboSereServTemp.Properties.Buttons[1].Visible = false;

                    if (sereServExt != null && sereServExt.ID > 0)
                    {
                        this.LblExecuteName.Text = string.Format("{0} ({1})", sereServExt.SUBCLINICAL_RESULT_USERNAME, sereServExt.SUBCLINICAL_RESULT_LOGINNAME);
                    }

                    //if (txtDescription.Text == "")
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

                    if (sereServ.EKIP_ID.HasValue)
                        ProcessLoadEkip(sereServ);
                    GetSereServPtttBySereServId();
                    UncheckImage();
                    Inventec.Common.Logging.LogSystem.Debug("3.2.3");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ComboAcsUser(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit cbo)
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ComboExecuteRole(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit cbo)
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROLE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var result = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT>>(RequestUriStore.HIS_SERE_SERV_EXT_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (result != null && result.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            //if (!dicSereServExt.ContainsKey(item.SERE_SERV_ID))
                            dicSereServExt[item.SERE_SERV_ID] = item;
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
                //var paramCommon = new CommonParam();

                //var filter = new MOS.Filter.HisSereServTempFilter();
                //filter.IS_ACTIVE = 1;
                //listTemplate = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_TEMP>>(RequestUriStore.HIS_SERE_SERV_TEMP__GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                var temp = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERE_SERV_TEMP>();
                if (temp != null && temp.Count > 0)
                {
                    var lstTemp = temp.Where(o => o.IS_ACTIVE == 1 && (!o.GENDER_ID.HasValue || o.GENDER_ID == ServiceReqConstruct.TDL_PATIENT_GENDER_ID)).ToList();
                    if (lstTemp != null && lstTemp.Count > 0)
                    {
                        listTemplate = lstTemp;
                    }
                    else
                    {
                        listTemplate = temp.Where(o => o.IS_ACTIVE == 1).ToList();
                    }
                }
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
                    sereServExt = dicSereServExt[sereServ.ID];
                    txtConclude.Text = sereServExt.CONCLUDE;
                    txtNote.Text = sereServExt.NOTE;
                    if (sereServExt.BEGIN_TIME != null)
                        dtBeginTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(sereServExt.BEGIN_TIME ?? 0) ?? DateTime.Now;
                    else
                        dtBeginTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ServiceReqConstruct.INTRUCTION_TIME) ?? DateTime.Now;

                    //Khi mở form, nếu nút "Lưu" được enable (cho phép người dùng sửa kết quả), thì luôn mặc định "Thời gian kết thúc" theo thời gian hiện tại, cho phép người dùng sửa
                    bool isDisable = false;
                    if (listServiceADO.Exists(o => o.MustHavePressBeforeExecute))
                    {
                        var serviceName = listServiceADO.Where(o => o.MustHavePressBeforeExecute).Select(s => s.TDL_SERVICE_NAME).ToList();
                        DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.DichVuChuaKeThuocVatTu, string.Join(",", serviceName)));
                        if (serviceName.Count == listServiceADO.Count)
                        {
                            isDisable = true;
                        }
                    }

                    string loginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    //nếu hoàn thành sẽ khóa lại.
                    //nếu không phải ng thực hiện sẽ khóa lại
                    //nếu ko phải admin sẽ khóa lại
                    //nếu hoàn thành và không phải ng xử lý và ko phải admin thì disable
                    if (currentServiceReq != null &&
                        currentServiceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT &&
                        currentServiceReq.EXECUTE_LOGINNAME != loginname && !HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(loginname))
                    {
                        isDisable = true;
                    }

                    if (sereServExt.END_TIME != null && isDisable)
                        dtEndTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(sereServExt.END_TIME ?? 0) ?? DateTime.Now;
                    else
                        dtEndTime.DateTime = DateTime.Now;

                    this.ActionType = GlobalVariables.ActionEdit;
                }
                else
                {
                    sereServExt = null;
                    txtConclude.Text = "";
                    dtBeginTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ServiceReqConstruct.INTRUCTION_TIME) ?? DateTime.Now;
                    dtEndTime.DateTime = DateTime.Now;
                    txtNote.Text = "";
                    //txtDescription.Text = "";
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

        private void ProcessLoadSereServFile(HIS_SERE_SERV sereServ)
        {
            try
            {
                var currentSereServFiles = GetSereServFilesBySereServId(sereServ.ID);
                if (currentSereServFiles != null && currentSereServFiles.Count > 0)
                {
                    foreach (MOS.EFMODEL.DataModels.HIS_SERE_SERV_FILE item in currentSereServFiles)
                    {
                        System.IO.MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(item.URL);
                        imageLoad = new List<ADO.ImageADO>();
                        if (stream != null && stream.Length > 0)
                        {
                            ADO.ImageADO tileNew = new ADO.ImageADO();
                            tileNew.FileName = item.SERE_SERV_FILE_NAME + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                            tileNew.IsChecked = true;
                            tileNew.IMAGE_DISPLAY = Image.FromStream(stream);
                            imageLoad.Add(tileNew);
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

        private List<HIS_SERE_SERV_FILE> GetSereServFilesBySereServId(long sereServId)
        {
            List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_FILE> result = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServFileFilter filter = new MOS.Filter.HisSereServFileFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.SERE_SERV_ID = sereServId;
                result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_FILE>>(RequestUriStore.HIS_SERE_SERV_FILE_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
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
                        GettxtDescription().RtfText = Utility.TextLibHelper.BytesToStringConverted(dicSarPrint[sereServExt.ID].CONTENT);
                        //txtDescription.RtfText = Utility.TextLibHelper.BytesToStringConverted(dicSarPrint[sereServExt.ID].CONTENT);
                        btnPrint.Enabled = true;
                        BtnEmr.Enabled = true;
                        //this.positionFinded = 0;
                        this.positionProtect = "";
                        //TODO
                        if (!String.IsNullOrEmpty(sereServExt.DOC_PROTECTED_LOCATION))
                        {
                            //positionFinded = Inventec.Common.TypeConvert.Parse.ToInt32(sereServExt.DOC_PROTECTED_LOCATION);
                            positionProtect = sereServExt.DOC_PROTECTED_LOCATION;
                        }
                        WordProtectedProcess protectedProcess = new WordProtectedProcess();
                        protectedProcess.InitialProtected(GettxtDescription(), ref positionProtect);
                    }
                    else if (CheckAllInOne.Checked)
                    {
                        btnPrint.Enabled = true;
                        BtnEmr.Enabled = true;
                        //InsertRow();
                    }
                    else
                    {
                        //txtDescription.Text = "";
                        //try
                        //{
                        //    txtDescription.Document.Unprotect();
                        //}
                        //catch (Exception exx)
                        //{
                        //    Inventec.Common.Logging.LogSystem.Error(exx);
                        //}
                        ClearDocument();

                        btnPrint.Enabled = false;
                        BtnEmr.Enabled = false;
                    }
                    this.currentSarPrint = dicSarPrint[sereServExt.ID];
                    //WordProcess.zoomFactor(GettxtDescription());
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

        private void ProcessLoadEkip(HIS_SERE_SERV sereServ)
        {
            try
            {
                if (sereServ != null && sereServ.EKIP_ID.HasValue)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisEkipUserViewFilter hisEkipUserFilter = new MOS.Filter.HisEkipUserViewFilter();
                    hisEkipUserFilter.EKIP_ID = sereServ.EKIP_ID;
                    var lst = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_EKIP_USER>>(ApiConsumer.HisRequestUriStore.HIS_EKIP_USER_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, hisEkipUserFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    if (lst != null && lst.Count > 0)
                    {
                        this.ekipUserAdos = new List<HisEkipUserADO>();
                        foreach (var item in lst)
                        {
                            AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_EKIP_USER, HisEkipUserADO>();
                            var HisEkipUserProcessing = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_EKIP_USER, HisEkipUserADO>(item);
                            if (item != lst[0])
                            {
                                HisEkipUserProcessing.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                            }
                            this.ekipUserAdos.Add(HisEkipUserProcessing);
                        }
                    }
                    //gridControlEkip.BeginUpdate();
                    //gridControlEkip.DataSource = null;
                    //gridControlEkip.DataSource = this.ekipUserAdos;
                    //gridControlEkip.EndUpdate();
                }
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
                var hisSereServPttts = new BackendAdapter(param)
                  .Get<List<V_HIS_SERE_SERV_PTTT>>(HisRequestUriStore.HIS_SERE_SERV_PTTT_GETVIEW, ApiConsumers.MosConsumer, hisSereServPtttFilter, param);
                this.sereServPTTT = (hisSereServPttts != null && hisSereServPttts.Count > 0) ? hisSereServPttts.FirstOrDefault() : null;
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
                    List<HIS_MACHINE> dataCombo = (currentServiceMachine != null && currentServiceMachine.Count > 0) ? ListMachine.Where(o => currentServiceMachine.Contains(o.ID)).ToList() : null;
                    InitComboExecuteRoom(editor, dataCombo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboExecuteRoom(GridLookUpEdit editor, List<HIS_MACHINE> dataCombo)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MACHINE_CODE", "Mã máy", 150, 1));
                columnInfos.Add(new ColumnInfo("MACHINE_NAME", "Tên máy", 250, 2));
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
                if (dataCombo != null && dataCombo.Count == 1)
                {
                    result = dataCombo.First().ID;
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
                cardControl.DataSource = listImage;
                cardControl.EndUpdate();
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
                if (!String.IsNullOrEmpty(searchCode))
                {
                    var data = listTemplate.Where(o => o.SERE_SERV_TEMP_CODE.ToLower().Contains(searchCode.ToLower())).ToList();
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
                        //txtDescription.Focus();
                        GettxtDescription().Focus();
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
                    //txtDescription.Text = "";
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
                    GettxtDescription().Focus();
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
                    GettxtDescription().Focus();
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
                e.Item.Checked = !e.Item.Checked;
                TileView tileView = sender as TileView;
                var currentData = (ADO.ImageADO)tileView.GetRow(e.Item.RowHandle);
                listImage = (List<ADO.ImageADO>)tileView1.DataSource;
                if (e.Item.Checked)
                {
                    List<int> listSTT = listImage.Select(o => o.STTImage ?? 0).Distinct().ToList();
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
                    currentData.STTImage = null;
                }
                tileView1.RefreshData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void Image_ItemClick(object sender, TileItemEventArgs e)
        //{
        //    try
        //    {
        //        //chọn ảnh sẽ add trực tiếp vào txtDescription --- tạm thờ không add được vào text box nên chưa dùng
        //        tileView1.GetCheckedRows();
        //        Image image = e.Item.Image;

        //        e.Item.Checked = !e.Item.Checked;
        //        if (image != null)
        //        {
        //            if (e.Item.Checked == true)//thêm ảnh
        //            {
        //                txtDescription.Document.Images.Insert(txtDescription.Document.CaretPosition, image);
        //            }
        //            else // tìm ảnh đã thêm và xóa
        //            {
        //                try
        //                {
        //                    DevExpress.XtraRichEdit.API.Native.DocumentImageCollection images = txtDescription.Document.Images;
        //                    txtDescription.Document.BeginUpdate();
        //                    foreach (DevExpress.XtraRichEdit.API.Native.DocumentImage item in images)
        //                    {
        //                        if (item.Image.NativeImage == image)
        //                        {
        //                            txtDescription.Document.Delete(item.Range);
        //                            break;
        //                        }
        //                    }
        //                    txtDescription.Document.EndUpdate();
        //                }
        //                catch (Exception ex)
        //                {
        //                    txtDescription.Document.EndUpdate();
        //                    Inventec.Common.Logging.LogSystem.Error(ex);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

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
                        //editor.EditValue = data.MACHINE_ID;
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
                    long? machineId = Inventec.Common.TypeConvert.Parse.ToInt64((view.GetRowCellValue(e.RowHandle, view.Columns["MACHINE_ID"]) ?? "").ToString());
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

                    this.currentServiceADO = (ADO.ServiceADO)gridViewSereServ.GetRow(rowHandle);

                    gridViewSereServ.OptionsSelection.EnableAppearanceFocusedCell = true;
                    gridViewSereServ.OptionsSelection.EnableAppearanceFocusedRow = true;
                    if (barManager1 == null)
                    {
                        barManager1 = new BarManager();
                        barManager1.Form = this;
                    }

                    PopupMenuProcessor popupMenuProcessor = new PopupMenuProcessor(currentServiceADO, barManager1, ServiceMouseRightClick, (RefeshReference)BtnRefreshs);
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
                if (e.Item is BarButtonItem && this.currentServiceADO != null)
                {
                    var bbtnItem = sender as BarButtonItem;
                    PopupMenuProcessor.ItemType type = (PopupMenuProcessor.ItemType)(e.Item.Tag);
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumerStore.SarConsumer, UriBaseStore.URI_API_SAR, LanguageManager.GetLanguage(), Inventec.Desktop.Common.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                    switch (type)
                    {
                        case PopupMenuProcessor.ItemType.EkipExecute:
                            EkipClsInfoClick();
                            break;
                        case PopupMenuProcessor.ItemType.PhieuKeKhaiThuocVatTuTieuHao:
                            onClickPhieuKeKhaiThuocVatTu(null, null);
                            break;
                        case PopupMenuProcessor.ItemType.PhieuPTTT:
                            onClickPhieuPTTT(null, null);
                            break;
                    }
                }
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
                if (this.currentServiceADO != null)
                {
                    if (this.sereServExt == null)
                    {
                        this.sereServExt = new HIS_SERE_SERV_EXT();
                        this.sereServExt.SERE_SERV_ID = sereServ.ID;
                    }

                    frmClsInfo frmClsInfo = new frmClsInfo(this.moduleData, this.currentServiceADO, this.ekipUserAdos, SaveSSPtttInfoClick, this.sereServPTTT, this.sereServExt, this.ServiceReqConstruct);
                    frmClsInfo.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveSSPtttInfoClick(List<HisEkipUserADO> ekipUserADOs, V_HIS_SERE_SERV_PTTT sereservPttt, HIS_SERE_SERV_EXT sereServExt)
        {
            try
            {
                this.ekipUserAdos = ekipUserADOs;
                this.sereServPTTT = sereservPttt;
                this.sereServExt = sereServExt;
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

        private void ComboAcsUser(GridLookUpEdit cbo, List<string> loginNames)
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();

                List<ACS.EFMODEL.DataModels.ACS_USER> acsUserAlows = new List<ACS.EFMODEL.DataModels.ACS_USER>();
                if (loginNames != null && loginNames.Count > 0)
                {
                    acsUserAlows = data.Where(o => loginNames.Contains(o.LOGINNAME)).ToList();
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, acsUserAlows, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                    ofd.Filter = "JPEG files (*.jpg,*.bmp,*.png)|*.jpg;*.bmp;*.png";
                    ofd.FilterIndex = 0;
                    ofd.Multiselect = true;

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        if (listImage == null) listImage = new List<ADO.ImageADO>();

                        foreach (var file in ofd.FileNames)
                        {
                            Image img = Image.FromFile(file);
                            string fileName = file.Split('\\').LastOrDefault();
                            fileName = fileName.Split('.').FirstOrDefault();

                            if (listImage.Exists(o => o.FileName == fileName)) continue;

                            ADO.ImageADO image = new ADO.ImageADO();
                            image.FileName = fileName;
                            image.IsChecked = false;
                            image.IMAGE_DISPLAY = img;

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

        private void tileView1_ItemDoubleClick(object sender, DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventArgs e)
        {
            try
            {
                // mở form xem ảnh
                var item = (ADO.ImageADO)tileView1.GetFocusedRow();
                Form formView = new ViewImage.FormViewImage(this.listImage, item);
                if (formView != null) formView.ShowDialog();
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
                        var txtDescription = GettxtDescription();
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
                                                Image imgFill = ResizeImage(images.First().IMAGE_DISPLAY, (int)(txt.Size.Width / 3) - 30, (int)(txt.Size.Height / 3) - 30);
                                                textBoxDocument.Images.Insert(textBoxDocument.Range.Start, imgFill);
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
                                            txtDescription.Document.Images.Insert(rangeImage.Start, ResizeImage(images.First().IMAGE_DISPLAY, 250, 140));
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

                //xuandv new
                //CommonParam param = new CommonParam();
                //HisTreatmentFilter filter = new HisTreatmentFilter();
                //filter.ID = this.ServiceReqConstruct.TREATMENT_ID;

                //var rsApi = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                //if (rsApi != null && rsApi.Count > 0 && rsApi.FirstOrDefault().TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                //{
                //    assignPrescription.IsExecutePTTT = true;
                //}
                if (TreatmentWithPatientTypeAlter.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU
                    || TreatmentWithPatientTypeAlter.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
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

        private void btnSereServTempList_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSereServTempList.Enabled) return;
                //Goi module danh sach danh muc xu ly

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
                        if (item.isSave == false)
                        {
                            successFull = false;
                            break;
                        }
                    }
                    if (!successFull)
                    {
                        if (!isPressButtonSaveNClose)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ChuaXuLyHetDichVu);
                        }
                        return;
                    }
                }

                var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>(RequestUriStore.HIS_SERVICE_REQ_FINISH, ApiConsumer.ApiConsumers.MosConsumer, currentServiceReq.ID, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (result != null)
                {
                    success = true;
                    if (this.RefreshData != null)
                    {
                        this.RefreshData(result);
                    }
                    btnFinish.Enabled = false;
                    btnSave.Enabled = false;
                    btnSaveNClose.Enabled = false;
                    BtnSaveNPrint.Enabled = false;
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

                if (CheckAllInOne.Checked)
                {
                    InsertRow(this.sereServ);//cập nhật lại dữ liệu
                    SaveAllProcess(false, false, chkForPreview.Checked);
                }
                else
                    SaveProcessor(false, false, chkForPreview.Checked);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSaveNClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSaveNClose.Enabled) return;
                btnSaveNClose.Focus();
                isPressButtonSaveNClose = true;
                if (CheckAllInOne.Checked)
                {
                    InsertRow(this.sereServ);//cập nhật lại dữ liệu
                    SaveAllProcess(true, true, chkForPreview.Checked);
                }
                else
                    SaveProcessor(true, true, chkForPreview.Checked);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnSaveNPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!BtnSaveNPrint.Enabled) return;
                BtnSaveNPrint.Focus();
                if (CheckAllInOne.Checked)
                {
                    InsertRow(this.sereServ);//cập nhật lại dữ liệu
                    SaveAllProcess(true, false, chkForPreview.Checked);
                }
                else
                    SaveProcessor(true, false, chkForPreview.Checked);
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
                //if (!btnCamera.Enabled) return;

                ////List<object> listArgs = new List<object>();
                ////listArgs.Add(currentServiceReq);
                ////HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.Camera", this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
                ////btnLoadImage_Click(null, null);

                //HIS.Desktop.ModuleExt.TabControlBaseProcess.CloseCameraFormOpened();

                //Inventec.Desktop.Common.Modules.Module _moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.Camera").FirstOrDefault();
                //if (_moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.Camera");
                //if (_moduleData.IsPlugin && _moduleData.ExtensionInfo != null)
                //{
                //    //Truyền vào để xử lý kết camera qua cổng usb sử dụng thư viện DirectX.Capture
                //    string captureType = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ServiceExecuteCFG.CaptureType);
                //    List<object> listArgs = new List<object>();
                //    listArgs.Add(currentServiceReq);
                //    listArgs.Add(captureType);
                //    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(_moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId));
                //    listArgs.Add((HIS.Desktop.Common.DelegateRefreshData)_dlgRefreshData);
                //    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(_moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                //    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                //    ((Form)extenceInstance).Show();

                //}

                EnableControlCamera(true);
                StartCamera();// nambg
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

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

        private void SaveProcessor(bool printNow, bool isClose, bool isPrintPreview = false)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                isPressButtonSave = true;
                if (!dxValidationProvider1.Validate()) return;
                Inventec.Desktop.Common.Message.WaitingManager.Show();
                if (sereServ == null) return;

                if (!sereServ.isSave && this.sereServExt == null)
                {
                    this.sereServExt = new MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT();
                    this.sereServExt.SERE_SERV_ID = sereServ.ID;
                }
                else
                {
                    this.sereServExt = dicSereServExt.ContainsKey(sereServ.ID) ? dicSereServExt[sereServ.ID] : new HIS_SERE_SERV_EXT() { SERE_SERV_ID = sereServ.ID };
                }

                this.sereServExt.NOTE = txtNote.Text.Trim();
                this.sereServExt.CONCLUDE = txtConclude.Text.Trim();
                if (!String.IsNullOrEmpty(txtNumberOfFilm.Text))
                {
                    this.sereServExt.NUMBER_OF_FILM = long.Parse(txtNumberOfFilm.Text);
                }
                else
                {
                    this.sereServExt.NUMBER_OF_FILM = null;
                }
                sereServExt.FILM_SIZE_ID = cboSizeOfFilm.EditValue != null ? (long?)cboSizeOfFilm.EditValue : null;
                ProcessDescriptionContent();

                if (ProcessSereServExt__DescriptionPrint(param, sereServ))
                {
                    Inventec.Desktop.Common.Message.WaitingManager.Hide();
                    Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                    return;
                }

                List<FileHolder> listFileHolder = new List<FileHolder>();
                //ProcessSereServFileExecute(listFileHolder);

                var machine = ListMachine.FirstOrDefault(o => o.ID == sereServ.MACHINE_ID);
                if (machine != null)
                {
                    this.sereServExt.MACHINE_CODE = machine.MACHINE_CODE;
                    this.sereServExt.MACHINE_ID = machine.ID;
                }
                else
                {
                    this.sereServExt.MACHINE_CODE = null;
                    this.sereServExt.MACHINE_ID = null;
                }

                if (dtBeginTime.EditValue != null)
                    sereServExt.BEGIN_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtBeginTime.DateTime);
                else
                    sereServExt.BEGIN_TIME = null;

                if (dtEndTime.EditValue != null)
                    sereServExt.END_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtEndTime.DateTime);
                else
                    sereServExt.END_TIME = null;

                string des = GettxtDescription().Text;
                if (Inventec.Common.String.CountVi.Count(des) > 4000)
                {
                    sereServExt.DESCRIPTION = Inventec.Common.String.CountVi.SubStringVi(des, 4000);
                }
                else
                    sereServExt.DESCRIPTION = des;

                HisSereServExtSDO data = new HisSereServExtSDO();
                data.HisSereServExt = this.sereServExt;
                data.HisEkipUsers = ProcessEkipUser(sereServ);
                ProcessSereServPtttInfo(ref data);

                MOS.SDO.HisSereServExtWithFileSDO apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post
                    <MOS.SDO.HisSereServExtWithFileSDO>
                    (this.sereServExt.ID == 0 ?
                    RequestUriStore.HIS_SERE_SERV_EXT_CREATE_SDO :
                    RequestUriStore.HIS_SERE_SERV_EXT_UPDATE_SDO,
                    ApiConsumer.ApiConsumers.MosConsumer, param, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken);
                Inventec.Common.Logging.LogSystem.Debug("Goi api: " + (sereServExt.ID == 0 ?
                          RequestUriStore.HIS_SERE_SERV_EXT_CREATE_SDO :
                          RequestUriStore.HIS_SERE_SERV_EXT_UPDATE_SDO) + "____Du lieu dau vao:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data) +
                         "____Du lieu dau ra:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiResult), apiResult));
                if (apiResult != null)
                {
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
                    }

                    this.currentServiceReq.EXECUTE_LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    this.currentServiceReq.EXECUTE_USERNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                    this.sereServExt.MODIFY_TIME = Inventec.Common.DateTime.Get.Now();
                    if (dicSereServExt.ContainsKey(apiResult.SereServExt.SERE_SERV_ID))
                        dicSereServExt[apiResult.SereServExt.SERE_SERV_ID].MODIFY_TIME = Inventec.Common.DateTime.Get.Now();
                    ProcessPatientInfo();

                    success = true;
                    if (listServiceADO != null && listServiceADO.Count > 0)
                    {
                        foreach (var item in listServiceADO)
                        {
                            if (item.ID == this.sereServ.ID)
                            {
                                item.isSave = true;
                                if (!item.EKIP_ID.HasValue)
                                {
                                    item.EKIP_ID = LoadEkipId(item.ID);
                                }
                                break;
                            }
                        }
                    }

                    btnPrint.Enabled = true;
                    BtnEmr.Enabled = true;
                    this.sereServ.isSave = true;
                    //SereServClickRow(this.sereServ);
                    GetSereServPtttBySereServId();
                    //ẩn trước khi lưu đóng tránh bị dừng pm
                    Inventec.Desktop.Common.Message.WaitingManager.Hide();
                    //lưu và đóng
                    if (printNow && isClose)
                    {
                        Inventec.Desktop.Common.Message.WaitingManager.Hide();
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.BanCoMuonInKetQua,
                    ResourceMessage.ThongBao,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            PrintResult(printNow && !isPrintPreview);
                        }
                    }
                    else if (printNow)
                    {
                        PrintResult(printNow && !isPrintPreview);
                    }

                    if (isClose)
                    {
                        btnFinish_Click(null, null);//chỉ kết thúc khi tất cả đã thực hiện

                        TabControlBaseProcess.CloseSelectedTabPage(SessionManager.GetTabControlMain());
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiResult), apiResult));

                //ẩn trước khi hiển thị thông báo 
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                #endregion

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

        private void SaveAllProcess(bool printNow, bool isClose, bool isPrintPreview = false)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                isPressButtonSave = true;
                if (!dxValidationProvider1.Validate()) return;
                Inventec.Desktop.Common.Message.WaitingManager.Show();
                MOS.SDO.HisSereServExtWithFileSDO apiResult = null;

                foreach (var sereServ in listServiceADOForAllInOne)
                {
                    if (sereServ == null) return;
                    var sereServExt = new MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT();

                    if (!sereServ.isSave)
                    {
                        sereServExt.SERE_SERV_ID = sereServ.ID;
                    }
                    else
                    {
                        sereServExt = dicSereServExt.ContainsKey(sereServ.ID) ? dicSereServExt[sereServ.ID] : new HIS_SERE_SERV_EXT() { SERE_SERV_ID = sereServ.ID };
                    }

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

                    var machine = ListMachine.FirstOrDefault(o => o.ID == sereServ.MACHINE_ID);
                    if (machine != null)
                    {
                        sereServExt.MACHINE_CODE = machine.MACHINE_CODE;
                        sereServExt.MACHINE_ID = machine.ID;
                    }
                    else
                    {
                        sereServExt.MACHINE_CODE = null;
                        sereServExt.MACHINE_ID = null;
                    }

                    string des = GettxtDescription().Text;
                    if (Inventec.Common.String.CountVi.Count(des) > 4000)
                    {
                        sereServExt.DESCRIPTION = Inventec.Common.String.CountVi.SubStringVi(des, 4000);
                    }
                    else
                        sereServExt.DESCRIPTION = des;

                    if (ProcessSereServExt__DescriptionPrint(param, sereServ, sereServExt))
                    {
                        Inventec.Desktop.Common.Message.WaitingManager.Hide();
                        Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        return;
                    }
                    List<FileHolder> listFileHolder = new List<FileHolder>();

                    HisSereServExtSDO data = new HisSereServExtSDO();
                    data.HisSereServExt = sereServExt;
                    data.HisEkipUsers = ProcessEkipUser(sereServ);
                    ProcessSereServPtttInfo(ref data);

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
                        }

                        this.currentServiceReq.EXECUTE_LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        this.currentServiceReq.EXECUTE_USERNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                        this.sereServExt.MODIFY_TIME = Inventec.Common.DateTime.Get.Now();
                        if (dicSereServExt.ContainsKey(apiResult.SereServExt.SERE_SERV_ID))
                            dicSereServExt[apiResult.SereServExt.SERE_SERV_ID].MODIFY_TIME = Inventec.Common.DateTime.Get.Now();
                        ProcessPatientInfo();

                        success = true;

                        sereServ.isSave = true;
                        if (!sereServ.EKIP_ID.HasValue)
                        {
                            sereServ.EKIP_ID = LoadEkipId(sereServ.ID);
                        }

                        foreach (var ado in listServiceADO)
                        {
                            if (ado.ID == sereServ.ID)
                            {
                                ado.isSave = sereServ.isSave;
                                ado.EKIP_ID = sereServ.EKIP_ID;
                                break;
                            }
                        }

                        btnPrint.Enabled = true;
                        BtnEmr.Enabled = true;
                        GetSereServPtttBySereServId();
                    }
                    else
                    {
                        success = false;
                        break;
                    }
                }

                //lưu và đóng
                if (success && printNow && isClose)
                {
                    Inventec.Desktop.Common.Message.WaitingManager.Hide();
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.BanCoMuonInKetQua,
                ResourceMessage.ThongBao,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        PrintResult(printNow && !isPrintPreview);
                    }
                }
                else if (success && printNow)
                {
                    PrintResult(printNow && !isPrintPreview);
                }

                if (success && isClose)
                {
                    btnFinish_Click(null, null);//chỉ kết thúc khi tất cả đã thực hiện

                    TabControlBaseProcess.CloseSelectedTabPage(SessionManager.GetTabControlMain());
                }

                //ẩn trước khi hiển thị thông báo 
                Inventec.Desktop.Common.Message.WaitingManager.Hide();

                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                #endregion

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

                SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();

                SignType type = new SignType();
                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.Library.EmrGenerate.SignType") == "1")
                {
                    type = SignType.USB;
                }
                else if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.Library.EmrGenerate.SignType") == "2")
                {
                    type = SignType.HMS;
                }

                InputADO inputADO = new InputADO(null, false, null, type);
                inputADO.DTI = ConfigSystems.URI_API_ACS + "|" + ConfigSystems.URI_API_EMR + "|" + ConfigSystems.URI_API_FSS;
                inputADO.IsSave = false;
                inputADO.IsSign = true;//set true nếu cần gọi ký
                inputADO.IsReject = true;
                inputADO.IsPrint = false;
                inputADO.IsExport = false;
                inputADO.DlgOpenModuleConfig = OpenSignConfig;

                //Mở popup 
                inputADO.Treatment = new Inventec.Common.SignLibrary.DTO.TreatmentDTO();
                inputADO.Treatment.TREATMENT_CODE = currentServiceReq.TDL_TREATMENT_CODE;//mã hồ sơ điều trị
                inputADO.DocumentName = (String.Format("{0} (Mã điều trị: {1})", cboSereServTemp.Text, currentServiceReq.TDL_TREATMENT_CODE));//Tên văn bản cần tạo

                DevExpress.XtraRichEdit.RichEditControl printDocument = ProcessDocumentBeforePrint(GettxtDescription());
                if (printDocument == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("printDocument is null");
                    return;
                }

                String temFile = Path.GetTempFileName();
                temFile = temFile.Replace(".tmp", ".pdf");
                printDocument.ExportToPdf(temFile);

                libraryProcessor.ShowPopup(temFile, inputADO);//truyền vào đường dẫn file cần ký, các định dạng hỗ trợ là: pdf,doc,docx,xls,xlsx,rdlc,...

                File.Delete(temFile);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void OpenSignConfig(EMR.TDO.DocumentTDO obj)
        {
            try
            {
                if (obj != null)
                {
                    EMR.Filter.EmrDocumentFilter filter = new EMR.Filter.EmrDocumentFilter();
                    filter.DOCUMENT_CODE__EXACT = obj.DocumentCode;
                    var apiResult = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<EMR.EFMODEL.DataModels.EMR_DOCUMENT>>(EMR.URI.EmrDocument.GET, ApiConsumer.ApiConsumers.EmrConsumer, filter, SessionManager.ActionLostToken, null);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        List<object> _listObj = new List<object>();
                        _listObj.Add(apiResult.Max(o => o.ID));//truyền vào id lớn nhất;

                        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("EMR.Desktop.Plugins.EmrSign", moduleData.RoomId, moduleData.RoomTypeId, _listObj);
                    }
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
                if (!btnAssignPaan.Enabled) return;

                if (currentServiceReq != null)
                {

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPaan").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPaan");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(currentServiceReq.TREATMENT_ID);
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
                this.panelDescription.Controls.Clear();
                WordProtectedProcess protectedProcess = new WordProtectedProcess();
                if (this.isWordFull)
                {
                    if (this.wordFullDocument == null)
                    {
                        this.wordFullDocument = new UcWordFull(EditorZoomChanged);
                        this.wordFullDocument.Dock = DockStyle.Fill;
                    }

                    this.wordFullDocument.txtDescription.RtfText = this.wordDocument.txtDescription.RtfText;
                    this.panelDescription.Controls.Add(this.wordFullDocument);
                    protectedProcess.InitialProtected(this.wordFullDocument.txtDescription, ref positionProtect);
                    //WordProcess.zoomFactor(this.wordFullDocument.txtDescription);
                }
                else
                {
                    this.wordDocument.txtDescription.RtfText = this.wordFullDocument.txtDescription.RtfText;
                    this.panelDescription.Controls.Add(this.wordDocument);
                    protectedProcess.InitialProtected(this.wordDocument.txtDescription, ref positionProtect);
                    //WordProcess.zoomFactor(this.wordDocument.txtDescription);
                }
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
                GettxtDescription().Focus();
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

        #endregion

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
                //var resultData = new PACS.ApiConsumerRaw().PostRaw<PACS.ImageResponseADO>(RequestUriStore.PACS_SERIVCE__LAY_THONG_TIN_ANH, layThongTinAnhInputADO, 0);
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
            load.Priority = ThreadPriority.Highest;
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
                if (row == null)//|| !row.JSON_FORM_ID.HasValue
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

        public void SaveNPrint()
        {
            try
            {
                //ẩn nút sẽ ko bấm phím tắt đc
                if (BtnSaveNPrint.Enabled && layoutControlItem28.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    BtnSaveNPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        private void tileView1_ContextButtonClick(object sender, DevExpress.Utils.ContextItemClickEventArgs e)
        {
            try
            {
                var titleItem = e.DataItem as TileViewItem;
                TileView view = sender as TileView;
                var rowData = view.GetRow(titleItem.RowHandle) as ADO.ImageADO;
                view.DeleteRow(titleItem.RowHandle);
                this.listImage = view.DataSource as List<ADO.ImageADO>;
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
                        //view.DeleteRow(item);
                        //this.listImage = view.DataSource as List<ADO.ImageADO>;
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboConnectionType_EditValueChanged(object sender, EventArgs e)
        {

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

        private void UCServiceExecute_Resize(object sender, EventArgs e)
        {
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
                ADO.ServiceADO sereServ = (ADO.ServiceADO)gridViewSereServ.GetRow(e.RowHandle);
                if (sereServ != null && sereServ.NUMBER_OF_FILM.HasValue && sereServ.NUMBER_OF_FILM.Value > 0)
                {
                    e.Appearance.ForeColor = System.Drawing.Color.Red;
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
                        if (dicSereServExt.ContainsKey(dataRow.ID) && (!String.IsNullOrWhiteSpace(dicSereServExt[dataRow.ID].NOTE) || !String.IsNullOrWhiteSpace(dicSereServExt[dataRow.ID].CONCLUDE) || !String.IsNullOrWhiteSpace(dicSereServExt[dataRow.ID].DESCRIPTION_SAR_PRINT_ID)))
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
                                        if (!String.IsNullOrWhiteSpace(dataRow.NOTE) || !String.IsNullOrWhiteSpace(dataRow.CONCLUDE) || !String.IsNullOrWhiteSpace(dataRow.DESCRIPTION_SAR_PRINT_ID))
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
                //if (this.sereServ == null || (ConnectImageOption != "1" && this.sereServ != null && String.IsNullOrEmpty(this.sereServ.TDL_PACS_TYPE_CODE)))
                //{
                //    LoadDataImageLocal();
                //}
                SetDisable();
                ValidNumberOfFilm();
                ValidBeginTime();
                ValidEndTime();
                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_SERVICE_REQ.IS_ALLOWING_PROCESSING_SUBCLINICAL_AFTER_LOCKING_TREATMENT") == "1" && TreatmentWithPatientTypeAlter != null && TreatmentWithPatientTypeAlter.IS_LOCK_FEE == 1)
                {
                    btnAssignService.Enabled = false;
                    btnTuTruc.Enabled = false;
                }
                //FillDataToInformationSurg();
                //SetEnableControlWithExecuterParam();
                //LoadDataToCombo();
                ReloadCameraAfterSearchByPatientThread();
                //InitRestoreLayoutGridViewFromXml(gridViewSereServ);

                CheckValidPress();
                //InitCameraDefault();

                var formMain = HIS.Desktop.Controls.Session.SessionManager.GetFormMain();
                if (formMain != null)
                {
                    if (formMain.InvokeRequired)
                    {
                        formMain.Invoke(new MethodInvoker(delegate
                        {
                            //WaitingManager.Show();
                            ////Dong tat ca cac tab page dang mo, dong thoi mo 1 page default
                            MethodReloadTextTabpageExecuteServiceByPatientThread(formMain);

                            ////Reload lại thông tin phòng, chi nhánh,... trong vùng status bar trong formmain
                            //MethodInitStatusBarThread(formMain);
                            //WaitingManager.Hide();
                        }));
                    }
                    else
                    {
                        //WaitingManager.Show();
                        ////Dong tat ca cac tab page dang mo, dong thoi mo 1 page default
                        MethodReloadTextTabpageExecuteServiceByPatientThread(formMain);

                        ////Reload lại thông tin phòng, chi nhánh,... trong vùng status bar trong formmain
                        //MethodInitStatusBarThread(formMain);
                        //WaitingManager.Hide();
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
                isNotExecuteWhileChangedZoomEditor = true;
                trackBarZoom = (long)(zoom * 100);
                isNotExecuteWhileChangedZoomEditor = false;

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
    }

    class EkipNameException : Exception
    {
        public EkipNameException() { }
    }
}

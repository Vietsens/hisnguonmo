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
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraRichEdit.API.Native;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.ModuleExt;
using HIS.Desktop.Plugins.Library.FormOtherSereServ;
using HIS.Desktop.Plugins.TestServiceExecute.ADO;
using HIS.Desktop.Utilities;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.SignLibrary;
using Inventec.Common.SignLibrary.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TestServiceExecute
{
    public partial class UCServiceExecute : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare
        private int ActionType = 0;
        /// <summary>
        /// được map từ HIS_SERVICE_REQ nên bị thiếu dữ liệu Name, code
        /// </summary>
        private V_HIS_SERVICE_REQ ServiceReqConstruct;
        private ADO.ServiceADO sereServ;
        private MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT sereServExt;
        private SAR.EFMODEL.DataModels.SAR_PRINT currentSarPrint;
        private MOS.EFMODEL.DataModels.HIS_SERVICE_REQ currentServiceReq;
        private Inventec.Desktop.Common.Modules.Module moduleData;
        private List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_TEMP> listTemplate;
        private Dictionary<string, object> dicParam;
        private Dictionary<string, Image> dicImage;
        private List<ADO.ImageADO> listImage;
        private Common.DelegateRefresh RefreshData;
        private List<ADO.ImageADO> imageLoad;
        private List<ADO.ServiceADO> listServiceADO;

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
        List<long> SERVICE_TYPE_IDs = new List<long>()
        {
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN
        };

        int positionHandle = -1;
        int positionFinded = 0;
        NumberStyles style = NumberStyles.Any;
        #endregion

        #region Construct
        public UCServiceExecute()
        {
            InitializeComponent();
        }

        public UCServiceExecute(Inventec.Desktop.Common.Modules.Module moduleData, MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ serviceReq, HIS.Desktop.Common.DelegateRefresh RefreshData)
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
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
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
                LoadKeysFromlanguage();
                SetDefaultValueControl();
                CreateThreadLoadDataDefault();
                FillDataCombo();
                FillDataToGrid();
                //TODO
                //if (this.sereServ == null || (ConnectImageOption != "1" && this.sereServ != null && String.IsNullOrEmpty(this.sereServ.TDL_PACS_TYPE_CODE)))
                //{
                //    LoadDataImageLocal();
                //}

                //TODO
                //FillDataToInformationSurg();
                SetDisable();
                //ValidNumberOfFilm();
                ValidBeginTime();
                ValidEndTime();
                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_SERVICE_REQ.IS_ALLOWING_PROCESSING_SUBCLINICAL_AFTER_LOCKING_TREATMENT") == "1" && TreatmentWithPatientTypeAlter != null && TreatmentWithPatientTypeAlter.IS_LOCK_FEE == 1)
                {
                    btnAssignService.Enabled = false;
                    btnTuTruc.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                //this.btnLoadImage.Text = Resources.ResourceLanguageManager.GetValue(
                //    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__BTN_LOAD_IMAGE");
                this.btnSaveNClose.Text = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__BTN_SAVE_N_CLOSE");
                //this.btnServiceReqMaty.Text = Resources.ResourceLanguageManager.GetValue(
                //    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__BTN_SERVICE_REQ_MATY");
                //this.btnCamera.Text = Resources.ResourceLanguageManager.GetValue(
                //    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__BTN_CAMERA");
                this.lciMunberOfFilm.Text = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__LCI_NUMBER_OF_FILM");

                //this.CheckAllInOne.Text = Resources.ResourceLanguageManager.GetValue(
                //    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__CHECK_ALL_IN_ONE");
                this.BtnDeleteImage.Text = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__BTN_DELETE_IMAGE");
                this.BtnChangeImage.Text = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__BTN_CHANGE_IMAGE");
                this.BtnChooseImage.Text = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__BTN_CHOOSE_IMAGE");
                this.btnTuTruc.Text = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__BTN_TU_TRUC");

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
                this.repositoryItemButtonSendSancy.Buttons[0].ToolTip = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__GC_BTN_SEND_SANCY");
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
            //dataFillTemplate.Priority = ThreadPriority.Highest;
            try
            {
                serviceReq.Start();
                listTemplate.Start();
                dataFillTemplate.Start();
                treatment.Start();

                serviceReq.Join();
                listTemplate.Join();
                dataFillTemplate.Join();
                treatment.Join();
            }
            catch (Exception ex)
            {
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
                if (ServiceReqConstruct != null)
                {
                    Inventec.Common.Logging.LogSystem.Info("1. Begin LoadCurrentServiceReq ");
                    CommonParam param = new CommonParam();
                    if (this.currentServiceReq == null)
                        this.currentServiceReq = new HIS_SERVICE_REQ();

                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(currentServiceReq, ServiceReqConstruct);

                    if (!currentServiceReq.START_TIME.HasValue)
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
                if (currentServiceReq != null &&
                    currentServiceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                {
                    this.btnAssignPrescription.Enabled = false;
                    this.btnAssignService.Enabled = false;
                    //this.btnCamera.Enabled = false;
                    //this.btnLoadImage.Enabled = false;
                    this.btnFinish.Enabled = false;
                    this.btnSave.Enabled = false;
                    this.btnSaveNClose.Enabled = false;
                    this.btnSereServTempList.Enabled = false;
                    this.btnTuTruc.Enabled = false;
                    this.cboSereServTemp.Enabled = false;
                    this.txtSereServTempCode.Enabled = false;
                    this.txtDescription.ReadOnly = true;
                    this.repositoryItemButtonServiceReqMaty.ReadOnly = true;
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
                txtDescription.Text = "";
                txtSereServTempCode.Focus();
                txtSereServTempCode.SelectAll();
                this.ActionType = GlobalVariables.ActionAdd;
                zoomFactor();
                btnPrint.Enabled = false;
                BtnEmr.Enabled = false;
                //PACS.PacsCFG.Reload();//TODO
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
                        ado.MACHINE_ID = ext != null && ext.ID > 0 ? ext.MACHINE_ID : ChoseDataMachine(ado);//TODO
                        ado.SoPhieu = String.Format("{0}-{1}", ReduceForCode(item.TDL_SERVICE_REQ_CODE, SERVICE_REQ_CODE__MAX_LENGTH), ReduceForCode(item.TDL_SERVICE_CODE, SERVICE_CODE__MAX_LENGTH));
                        //var service = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == item.SERVICE_ID);
                        //if (ext != null)//TODO
                        //{
                        //    ado.NUMBER_OF_FILM = ext.NUMBER_OF_FILM;
                        //}
                        //else
                        //{
                        //    if (service != null && service.NUMBER_OF_FILM.HasValue)
                        //    {
                        //        ado.NUMBER_OF_FILM = (long)(Math.Round(service.NUMBER_OF_FILM.Value * item.AMOUNT, 0, MidpointRounding.AwayFromZero));
                        //    }
                        //}
                        listServiceADO.Add(ado);
                    }
                    //CheckAllInOne.ReadOnly = apiResult.Count == 1 ? true : false;
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

        private void CreateThreadGetDataGrid(ADO.ThreadSereServADO data)
        {
            Thread SereServ = new Thread(GetSereServ);
            Thread Bill = new Thread(GetBill);
            Thread Deposit = new Thread(GetDeposit);
            Thread Repay = new Thread(GetRepay);
            try
            {
                SereServ.Start(data);
                Bill.Start(data);
                Deposit.Start(data);
                Repay.Start(data);

                SereServ.Join();
                Bill.Join();
                Deposit.Join();
                Repay.Join();
            }
            catch (Exception ex)
            {
                SereServ.Abort();
                Bill.Abort();
                Deposit.Abort();
                Repay.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetSereServ(object obj)
        {
            try
            {
                if (obj != null)
                {
                    var data = (ADO.ThreadSereServADO)obj;
                    CommonParam paramCommon = new CommonParam();
                    MOS.Filter.HisSereServFilter filter = new MOS.Filter.HisSereServFilter();
                    filter.ORDER_FIELD = "SERVICE_NUM_ORDER";
                    filter.ORDER_DIRECTION = "DESC";
                    filter.SERVICE_REQ_ID = currentServiceReq.ID;
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var apiResult = new Inventec.Common.Adapter.BackendAdapter
                        (paramCommon).Get<List<HIS_SERE_SERV>>
                        (ApiConsumer.HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    if (apiResult != null)
                    {
                        apiResult = apiResult.Where(o => o.IS_NO_EXECUTE != 1).ToList();
                        data.ListHisSereServ = apiResult;
                        var listId = apiResult.Select(o => o.ID).ToList();
                        ProcessDicSereServExt(listId);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetBill(object obj)
        {
            try
            {
                if (obj != null)
                {
                    var data = (ADO.ThreadSereServADO)obj;
                    CommonParam paramCommon = new CommonParam();
                    MOS.Filter.HisSereServBillFilter filter = new MOS.Filter.HisSereServBillFilter();
                    filter.TDL_TREATMENT_ID = currentServiceReq.TREATMENT_ID;
                    var apiResult = new Inventec.Common.Adapter.BackendAdapter
                        (paramCommon).Get<List<HIS_SERE_SERV_BILL>>
                        (RequestUriStore.HIS_SERE_SERV_BILL_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        data.DictHisSersServBill = new Dictionary<long, List<HIS_SERE_SERV_BILL>>();
                        foreach (var item in apiResult)
                        {
                            if (!data.DictHisSersServBill.ContainsKey(item.SERE_SERV_ID))
                                data.DictHisSersServBill[item.SERE_SERV_ID] = new List<HIS_SERE_SERV_BILL>();
                            data.DictHisSersServBill[item.SERE_SERV_ID].Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDeposit(object obj)
        {
            try
            {
                if (obj != null)
                {
                    var data = (ADO.ThreadSereServADO)obj;
                    CommonParam paramCommon = new CommonParam();
                    MOS.Filter.HisSereServDepositFilter filter = new MOS.Filter.HisSereServDepositFilter();
                    filter.TDL_TREATMENT_ID = currentServiceReq.TREATMENT_ID;
                    var apiResult = new Inventec.Common.Adapter.BackendAdapter
                        (paramCommon).Get<List<HIS_SERE_SERV_DEPOSIT>>
                        (RequestUriStore.HIS_SERE_SERV_DEPOSIT_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        data.DictHisSereServDeposit = new Dictionary<long, List<HIS_SERE_SERV_DEPOSIT>>();
                        foreach (var item in apiResult)
                        {
                            if (!data.DictHisSereServDeposit.ContainsKey(item.SERE_SERV_ID))
                                data.DictHisSereServDeposit[item.SERE_SERV_ID] = new List<HIS_SERE_SERV_DEPOSIT>();
                            data.DictHisSereServDeposit[item.SERE_SERV_ID].Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetRepay(object obj)
        {
            try
            {
                if (obj != null)
                {
                    var data = (ADO.ThreadSereServADO)obj;
                    CommonParam paramCommon = new CommonParam();
                    MOS.Filter.HisSeseDepoRepayFilter filter = new MOS.Filter.HisSeseDepoRepayFilter();
                    filter.TDL_TREATMENT_ID = currentServiceReq.TREATMENT_ID;
                    var apiResult = new Inventec.Common.Adapter.BackendAdapter
                        (paramCommon).Get<List<HIS_SESE_DEPO_REPAY>>
                        (RequestUriStore.HIS_SESE_DEPO_REPAY_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        data.ListHisSeseDepoRepay = new Dictionary<long, List<HIS_SESE_DEPO_REPAY>>();
                        foreach (var item in apiResult)
                        {
                            if (!data.ListHisSeseDepoRepay.ContainsKey(item.SERE_SERV_DEPOSIT_ID))
                                data.ListHisSeseDepoRepay[item.SERE_SERV_DEPOSIT_ID] = new List<HIS_SESE_DEPO_REPAY>();
                            data.ListHisSeseDepoRepay[item.SERE_SERV_DEPOSIT_ID].Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void zoomFactor()
        {
            try
            {
                float zoom = 0;
                if (txtDescription.Document.Sections[0].Page.Landscape)
                    zoom = (float)(txtDescription.Width - 50) / (txtDescription.Document.Sections[0].Page.Height / 3);
                else
                    zoom = (float)(txtDescription.Width - 50) / (txtDescription.Document.Sections[0].Page.Width / 3);
                txtDescription.ActiveView.ZoomFactor = zoom;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataCombo()
        {
            try
            {
                //TODO
                //if (ConnectImageOption == "2")
                //{
                //    Gc_SendSancy.VisibleIndex = 1;
                //    //ListRoomMachine = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_ROOM_MACHINE>();
                //    //if (ListRoomMachine != null && ListRoomMachine.Count > 0)
                //    //{
                //    //    ListRoomMachine = ListRoomMachine.Where(o => o.ROOM_ID == moduleData.RoomId).ToList();
                //    //}
                //}

                //ListMachine = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MACHINE>();
                //if (ListMachine != null && ListMachine.Count > 0)
                //    ListMachine = ListMachine.Where(o => o.IS_ACTIVE == 1 && moduleData.RoomId == o.ROOM_ID).ToList();

                ////if (ListRoomMachine != null && ListRoomMachine.Count > 0)
                ////{
                ////    ListMachine = ListMachine.Where(o => ListRoomMachine.Select(s => s.MACHINE_ID).Contains(o.ID)).ToList();
                ////}

                //ListServiceMachine = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_MACHINE>();
                //if (ListServiceMachine != null && ListServiceMachine.Count > 0)
                //    ListServiceMachine = ListServiceMachine.Where(o => o.IS_ACTIVE == 1).ToList();

                //ComboAcsUser(repositoryItemCboName);//Họ và tên
                //ComboExecuteRole(repositoryItemCboRole);//Vai trò
                ComboServiceMachine(repositoryItemMachineId);//máy
                InitComboSereServTemp(new List<HIS_SERE_SERV_TEMP>());
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
                var images = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.listImage;
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
                ProcessLoadGridImage(listImage);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void FillDataToInformationSurg()
        //{
        //    try
        //    {
        //        if (ekipUserAdos == null || ekipUserAdos.Count == 0)
        //        {
        //            ekipUserAdos = new List<HisEkipUserADO>();
        //            HisEkipUserADO ekipUserAdoTemp = new HisEkipUserADO();
        //            ekipUserAdoTemp.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
        //            ekipUserAdos.Add(ekipUserAdoTemp);

        //            gridControlEkip.DataSource = ekipUserAdos;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void SereServClickRow(ADO.ServiceADO sereServ)
        {
            try
            {
                isPressButtonSave = false;
                //load tempalte lên wordEdit
                if (sereServ != null)
                {
                    this.sereServ = sereServ;
                    txtNumberOfFilm.Text = sereServ.NUMBER_OF_FILM.HasValue ? sereServ.NUMBER_OF_FILM.ToString() : null;
                    Inventec.Common.Logging.LogSystem.Debug("3.2.1");
                    ProcessLoadSereServExt(sereServ, ref sereServExt);
                    ProcessLoadSereServExtDescriptionPrint(sereServExt);
                    Inventec.Common.Logging.LogSystem.Debug("3.2.2");

                    txtSereServTempCode.Text = "";
                    cboSereServTemp.EditValue = null;
                    cboSereServTemp.Properties.Buttons[1].Visible = false;
                    if (txtDescription.Text == "")
                    {
                        Inventec.Common.Logging.LogSystem.Debug("3.2.2.1");
                        ProcessLoadTemplate(sereServ);
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("3.2.2.2");
                        var temp = listTemplate.Where(o => o.SERVICE_ID == sereServ.SERVICE_ID).ToList();
                        var tempType = listTemplate.Where(o => o.SERVICE_TYPE_ID == sereServ.TDL_SERVICE_TYPE_ID ||
                            !o.SERVICE_TYPE_ID.HasValue).ToList();
                        if (temp != null && temp.Count > 0)
                            InitComboSereServTemp(temp);
                        else if (tempType != null && tempType.Count > 0)
                            InitComboSereServTemp(tempType);
                        else
                            InitComboSereServTemp(listTemplate);
                    }

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

        //private void ValidNumberOfFilm()
        //{
        //    try
        //    {
        //        string key = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.TestServiceExecute.CĐHA.ValidNumberOfFilm");//Giá trị = 1 bắt buộc nhập số phim

        //        if (ServiceReqConstruct.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA && key.Trim() == "1")
        //        {
        //            lciMunberOfFilm.AppearanceItemCaption.ForeColor = Color.Maroon;
        //            Validation.FilmValidationRule FilmRule = new Validation.FilmValidationRule();
        //            FilmRule.txtNumberOfFilm = txtNumberOfFilm;
        //            FilmRule.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
        //            FilmRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
        //            this.dxValidationProvider1.SetValidationRule(txtNumberOfFilm, FilmRule);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void ValidBeginTime()
        {
            try
            {
                Validation.BeginTimeValidationRule valid = new Validation.BeginTimeValidationRule();
                valid.dtBeginTime = dtBeginTime;
                valid.dtEndTime = dtEndTime;
                valid.IntructionTime = ServiceReqConstruct.INTRUCTION_TIME;
                valid.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(dtBeginTime, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidEndTime()
        {
            try
            {
                Validation.EndTimeValidationRule valid = new Validation.EndTimeValidationRule();
                valid.dtEndTime = dtEndTime;
                valid.IntructionTime = ServiceReqConstruct.INTRUCTION_TIME;
                valid.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(dtEndTime, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region ProcessLoad
        private void ProcessTestServiceResultKey()
        {
            try
            {
                //-Thời gian lấy mẫu
                //-Thời gian duyệt mẫu
                //-Thời gian trả kết quả
                //-Người lấy mẫu
                //-Người duyệt mẫu
                //-Người trả kết quả

                //Liên quan tới ký số:
                //THời gian ban hành kết quả( thời gian ký)
                //Người ban hành kết quả.
                //Key:
                //- Mã máy xét nghiêm (tương ứng với các xét nghiệm máy đó làm)
                //- Tên máy xét nghiệm
                //- Key giá trị bình thường
                //- Key giá trị báo động (#17901)
                //- Key Cảnh báo Hight, Low                       

                //Quote #4                      
                //Bổ sung Barcode : 
                //- mã bệnh nhân
                //- Mã điều trị
                //- Mã y lệnh

                CommonParam param = new CommonParam();
                if (sereServ != null)
                {
                    var testIndexranges = BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>();
                    var testIndex = BackendDataWorker.Get<V_HIS_TEST_INDEX>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                    List<long> sereServIds = new List<long>();
                    sereServIds.Add(sereServ.ID);
                    HisSereServTeinViewFilter sereSerTeinFilter = new HisSereServTeinViewFilter();
                    sereSerTeinFilter.SERE_SERV_IDs = sereServIds;
                    sereSerTeinFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    sereSerTeinFilter.ORDER_FIELD = "NUM_ORDER";
                    sereSerTeinFilter.ORDER_DIRECTION = "DESC";

                    List<V_HIS_SERE_SERV_TEIN> lstSereServTein = new List<V_HIS_SERE_SERV_TEIN>();
                    lstSereServTein = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_TEIN>>(HisRequestUriStore.HIS_SERE_SERV_TEIN_GET, ApiConsumers.MosConsumer, sereSerTeinFilter, param);
                    if (lstSereServTein != null && lstSereServTein.Count > 0)
                    {
                        LisSampleViewFilter lisSampleFilter = new LisSampleViewFilter();
                        lisSampleFilter.SERVICE_REQ_CODE__EXACT = sereServ.TDL_SERVICE_REQ_CODE;
                        lisSampleFilter.TREATMENT_CODE__EXACT = sereServ.TDL_TREATMENT_CODE;
                        lisSampleFilter.ORDER_FIELD = "NUM_ORDER";
                        lisSampleFilter.ORDER_DIRECTION = "ASC";

                        //Lấy các mẫu trạng thái chưa trả kết quả
                        List<long> sampleSttIds = new List<long>();
                        sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM);
                        sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM);
                        sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ);
                        sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI);
                        sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN);
                        sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ);

                        lisSampleFilter.SAMPLE_STT_IDs = sampleSttIds;

                        List<V_LIS_RESULT> lstResultItem = null;

                        param = new CommonParam();
                        V_LIS_SAMPLE apiResultLisSample = new BackendAdapter(param).Get<List<V_LIS_SAMPLE>>("api/LisSample/GetView", ApiConsumer.ApiConsumers.LisConsumer, lisSampleFilter, param).FirstOrDefault();

                        if (apiResultLisSample == null) apiResultLisSample = new V_LIS_SAMPLE();
                        AddKeyIntoDictionaryPrint<V_LIS_SAMPLE>(apiResultLisSample, this.dicParam, false);

                        List<V_HIS_TEST_INDEX_RANGE> testIndexRanges = null;
                        List<V_HIS_TEST_INDEX> currentTestIndexs = null;

                        param = new CommonParam();
                        if (apiResultLisSample != null && apiResultLisSample.ID > 0)
                        {
                            LIS.Filter.LisResultViewFilter resultFilter = new LisResultViewFilter();
                            resultFilter.SAMPLE_ID = apiResultLisSample.ID;
                            resultFilter.ORDER_DIRECTION = "ASC";
                            resultFilter.ORDER_FIELD = "SERVICE_NUM_ORDER";
                            lstResultItem = new BackendAdapter(param).Get<List<V_LIS_RESULT>>("api/LisResult/GetView", ApiConsumer.ApiConsumers.LisConsumer, resultFilter, param);

                            currentTestIndexs = new List<V_HIS_TEST_INDEX>();
                            var serviceCodes = lstResultItem.Select(o => o.SERVICE_CODE).Distinct().ToList();
                            currentTestIndexs = testIndex.Where(o => serviceCodes.Contains(o.SERVICE_CODE)).ToList();
                            if (currentTestIndexs != null && currentTestIndexs.Count > 0 && testIndex != null && testIndex.Count > 0)
                            {
                                var testIndexCodes = currentTestIndexs.Select(o => o.TEST_INDEX_CODE).Distinct().ToList();
                                testIndexRanges = BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>().Where(o => testIndexCodes.Contains(o.TEST_INDEX_CODE)).ToList();
                            }
                        }

                        var testIndFist = currentTestIndexs.FirstOrDefault(o => o.TEST_INDEX_CODE == lstResultItem[0].TEST_INDEX_CODE);

                        List<TestLisResultADO> lstLisResultADOs = new List<TestLisResultADO>();
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
                                //hisSereServTein.HAS_ONE_CHILD = 0;
                                Inventec.Common.Mapper.DataObjectMapper.Map<TestLisResultADO>(hisSereServTein, ssTein);
                                hisSereServTein.IS_PARENT = 0;

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
                                //hisSereServTein.PARENT_ID = hisSereServTeinSDO.CHILD_ID;
                                hisSereServTein.TEST_INDEX_CODE = ssTein.TEST_INDEX_CODE;
                                hisSereServTein.TEST_INDEX_NAME = ssTein.TEST_INDEX_NAME;
                                hisSereServTein.MODIFIER = "";
                                //hisSereServTeinSDO.SAMPLE_SERVICE_ID = ssTein.SAMPLE_SERVICE_ID;
                                //hisSereServTeinSDO.SAMPLE_SERVICE_STT_ID = ssTein.SAMPLE_SERVICE_STT_ID;
                                hisSereServTein.MODIFIER = ssTein.MODIFIER;
                                hisSereServTein.VALUE_RANGE = ssTein.VALUE;
                                hisSereServTein.LIS_RESULT_ID = ssTein.ID;
                                hisSereServTein.MACHINE_ID = ssTein.MACHINE_ID;
                                hisSereServTein.MACHINE_ID_OLD = ssTein.MACHINE_ID;
                                hisSereServTein.SAMPLE_ID = ssTein.SAMPLE_ID;
                                hisSereServTein.SAMPLE_SERVICE_ID = ssTein.SAMPLE_SERVICE_ID;
                                hisSereServTein.SAMPLE_SERVICE_STT_CODE = ssTein.SAMPLE_SERVICE_STT_CODE;
                                hisSereServTein.SAMPLE_SERVICE_STT_ID = ssTein.SAMPLE_SERVICE_STT_ID;
                                hisSereServTein.SAMPLE_SERVICE_STT_NAME = ssTein.SAMPLE_SERVICE_STT_NAME;
                                hisSereServTein.SERVICE_NUM_ORDER = ssTein.SERVICE_NUM_ORDER;
                                hisSereServTein.OLD_VALUE = ssTein.OLD_VALUE;
                                hisSereServTein.DESCRIPTION = "";
                                //hisSereServTeinSDO.SAMPLE_STT_ID = ssTein.SAMPLE_STT_ID;

                                hisSereServTein.VALUE_HL = ssTein.VALUE;
                                hisSereServTein.VALUE = ssTein.VALUE;
                                hisSereServTein.VALUE_DEC = Inventec.Common.TypeConvert.Parse.ToDecimal(ssTein.VALUE);

                                hisSereServTein.NOTE = ssTein.DESCRIPTION;
                                lstLisResultADOs.Add(hisSereServTein);
                            }
                        }

                        if (lstLisResultADOs != null && lstLisResultADOs.Count > 0)
                        {
                            foreach (var hisSereServTeinSDO in lstLisResultADOs)
                            {
                                if (dicParam.ContainsKey(hisSereServTeinSDO.TEST_INDEX_CODE))
                                {
                                    dicParam[hisSereServTeinSDO.TEST_INDEX_CODE] = hisSereServTeinSDO.VALUE;
                                }
                                else
                                {
                                    V_HIS_TEST_INDEX_RANGE testIndexRange = new V_HIS_TEST_INDEX_RANGE();
                                    testIndexRange = GetTestIndexRange(this.currentServiceReq.TDL_PATIENT_DOB, this.currentServiceReq.TDL_PATIENT_GENDER_ID, hisSereServTeinSDO.TEST_INDEX_CODE, testIndexranges);
                                    if (testIndexRange != null)
                                    {
                                        ProcessMaxMixValue(hisSereServTeinSDO, testIndexRange);
                                    }

                                    dicParam.Add(hisSereServTeinSDO.TEST_INDEX_CODE, hisSereServTeinSDO.VALUE);
                                    dicParam.Add("MACHINE_CODE_" + hisSereServTeinSDO.TEST_INDEX_CODE, hisSereServTeinSDO.MACHINE_CODE);
                                    dicParam.Add("MACHINE_NAME_" + hisSereServTeinSDO.TEST_INDEX_CODE, hisSereServTeinSDO.MACHINE_NAME);

                                    dicParam.Add("VALUE_RANGE_" + hisSereServTeinSDO.TEST_INDEX_CODE, hisSereServTeinSDO.VALUE_RANGE);
                                    dicParam.Add("DESCRIPTION_" + hisSereServTeinSDO.TEST_INDEX_CODE, hisSereServTeinSDO.DESCRIPTION);
                                    dicParam.Add("NOTE_" + hisSereServTeinSDO.TEST_INDEX_CODE, hisSereServTeinSDO.NOTE);

                                    dicParam.Add("MIN_VALUE_" + hisSereServTeinSDO.TEST_INDEX_CODE, hisSereServTeinSDO.MIN_VALUE);
                                    dicParam.Add("MAX_VALUE_" + hisSereServTeinSDO.TEST_INDEX_CODE, hisSereServTeinSDO.MAX_VALUE);
                                    dicParam.Add("NORMAL_VALUE_" + hisSereServTeinSDO.TEST_INDEX_CODE, testIndexRange.NORMAL_VALUE);

                                    dicParam.Add("HIGH_OR_LOW_" + hisSereServTeinSDO.TEST_INDEX_CODE, hisSereServTeinSDO.HIGH_OR_LOW);
                                    dicParam.Add("VALUE_HL_" + hisSereServTeinSDO.TEST_INDEX_CODE, hisSereServTeinSDO.VALUE_HL);
                                    dicParam.Add("DESCRIPTION_HL_" + hisSereServTeinSDO.TEST_INDEX_CODE, hisSereServTeinSDO.DESCRIPTION_HL);
                                    dicParam.Add("WARNING_MIN_VALUE_" + hisSereServTeinSDO.TEST_INDEX_CODE, hisSereServTeinSDO.WARNING_MIN_VALUE);
                                    dicParam.Add("WARNING_MAX_VALUE_" + hisSereServTeinSDO.TEST_INDEX_CODE, hisSereServTeinSDO.WARNING_MAX_VALUE);
                                    dicParam.Add("WARNING_NOTE_" + hisSereServTeinSDO.TEST_INDEX_CODE, hisSereServTeinSDO.WARNING_NOTE);
                                    dicParam.Add("WARNING_DESCRIPTION_" + hisSereServTeinSDO.TEST_INDEX_CODE, hisSereServTeinSDO.WARNING_DESCRIPTION);
                                }
                            }
                        }

                        dicImage.Add("BARCODE_PATIENT_CODE", GenerateBarcode(currentServiceReq.TDL_PATIENT_CODE));
                        dicImage.Add("BARCODE_TREATMENT_CODE", GenerateBarcode(currentServiceReq.TDL_TREATMENT_CODE));
                        dicImage.Add("BARCODE_SERVICE_REQ_CODE", GenerateBarcode(currentServiceReq.SERVICE_REQ_CODE));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        V_HIS_TEST_INDEX_RANGE GetTestIndexRange(long dob, long? genderId, string testIndexId, List<V_HIS_TEST_INDEX_RANGE> testIndexRanges)
        {
            MOS.EFMODEL.DataModels.V_HIS_TEST_INDEX_RANGE testIndexRange = new V_HIS_TEST_INDEX_RANGE();
            try
            {
                if (testIndexRanges != null && testIndexRanges.Count > 0)
                {
                    long age = Inventec.Common.DateTime.Calculation.Age(dob);

                    var query = testIndexRanges.Where(o => o.TEST_INDEX_CODE == testIndexId
                            && ((o.AGE_FROM.HasValue && o.AGE_FROM.Value <= age) || !o.AGE_FROM.HasValue)
                            && ((o.AGE_TO.HasValue && o.AGE_TO.Value >= age) || !o.AGE_TO.HasValue));
                    if (genderId == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE)
                    {
                        query = query.Where(o => o.IS_MALE == 1);
                    }
                    else if (genderId == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
                    {
                        query = query.Where(o => o.IS_FEMALE == 1);
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

        private void ProcessMaxMixValue(TestLisResultADO ti, V_HIS_TEST_INDEX_RANGE testIndexRange)
        {
            try
            {
                Decimal minValue = 0, maxValue = 0, value = 0, warMax = 0, warMin = 0;
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
                            ti.VALUE_DEC = value;
                        }
                        else
                        {
                            ti.VALUE_DEC = null;
                        }
                    }

                    ti.IS_NORMAL = null;
                    ti.IS_HIGHER = null;
                    ti.IS_LOWER = null;

                    if (!String.IsNullOrWhiteSpace(testIndexRange.WARNING_MIN_VALUE))
                    {
                        if (Decimal.TryParse((testIndexRange.WARNING_MIN_VALUE ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out warMin))
                        {
                            ti.WARNING_MIN_VALUE = warMin;

                        }
                        else
                        {
                            ti.WARNING_MIN_VALUE = null;
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(testIndexRange.WARNING_MAX_VALUE))
                    {
                        if (Decimal.TryParse((testIndexRange.WARNING_MAX_VALUE ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out warMax))
                        {
                            ti.WARNING_MAX_VALUE = warMax;

                        }
                        else
                        {
                            ti.WARNING_MAX_VALUE = null;
                        }
                    }

                    ti.VALUE_HL = ti.VALUE;

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

                            if (ti.VALUE != null && ti.MIN_VALUE != null && ti.MIN_VALUE <= ti.VALUE_DEC && ti.MAX_VALUE != null && ti.VALUE_DEC < ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (ti.VALUE != null && ti.MIN_VALUE != null && ti.VALUE_DEC < ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (ti.VALUE != null && ti.MAX_VALUE != null && ti.MAX_VALUE <= ti.VALUE_DEC)
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

                            if (ti.VALUE != null && ti.MIN_VALUE != null && ti.MIN_VALUE <= ti.VALUE_DEC && ti.MAX_VALUE != null && ti.VALUE_DEC <= ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (ti.VALUE != null && ti.MIN_VALUE != null && ti.VALUE_DEC < ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (ti.VALUE != null && ti.MAX_VALUE != null && ti.MAX_VALUE < ti.VALUE_DEC)
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

                            if (ti.VALUE != null && ti.MIN_VALUE != null && ti.MIN_VALUE < ti.VALUE_DEC && ti.MAX_VALUE != null && ti.VALUE_DEC <= ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (ti.VALUE != null && ti.MIN_VALUE != null && ti.VALUE_DEC < ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (ti.VALUE != null && ti.MAX_VALUE != null && ti.MAX_VALUE < ti.VALUE_DEC)
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
                            if (ti.VALUE != null && ti.MIN_VALUE != null && ti.MIN_VALUE < ti.VALUE_DEC && ti.MAX_VALUE != null && ti.VALUE_DEC < ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (ti.VALUE != null && ti.MIN_VALUE != null && ti.VALUE_DEC <= ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (ti.VALUE != null && ti.MAX_VALUE != null && ti.MAX_VALUE <= ti.VALUE_DEC)
                            {
                                ti.IS_HIGHER = true;
                            }
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(testIndexRange.WARNING_MIN_VALUE))
                    {
                        if (Decimal.TryParse((testIndexRange.WARNING_MIN_VALUE ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out warMin))
                        {
                            ti.WARNING_MIN_VALUE = warMin;

                        }
                        else
                        {
                            ti.WARNING_MIN_VALUE = null;
                        }
                    }
                    if (!String.IsNullOrWhiteSpace(testIndexRange.WARNING_MAX_VALUE))
                    {
                        if (Decimal.TryParse((testIndexRange.WARNING_MAX_VALUE ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out warMax))
                        {
                            ti.WARNING_MAX_VALUE = warMax;

                        }
                        else
                        {
                            ti.WARNING_MAX_VALUE = null;
                        }
                    }

                    ti.VALUE_HL = ti.VALUE;

                    this.ProcessHighLowValue(ti, testIndexRange);
                    this.ProcessHighLowWarningValue(ti, testIndexRange);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessHighLowValue(TestLisResultADO ti, V_HIS_TEST_INDEX_RANGE testIndexRange)
        {
            if (!String.IsNullOrEmpty(testIndexRange.NORMAL_VALUE))
            {
                ti.DESCRIPTION_HL = testIndexRange.NORMAL_VALUE;
                if (!String.IsNullOrWhiteSpace(ti.DESCRIPTION_HL) && !String.IsNullOrWhiteSpace(ti.VALUE_RANGE) && ti.VALUE_RANGE.ToUpper() == ti.DESCRIPTION_HL.ToUpper())
                {
                    ti.HIGH_OR_LOW = "";
                }
                else
                {
                    ti.HIGH_OR_LOW = " ";
                }
            }
            else
            {
                ti.DESCRIPTION_HL = "";

                if (testIndexRange.IS_ACCEPT_EQUAL_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_MAX == null)
                {
                    if (testIndexRange.MIN_VALUE != null)
                    {
                        ti.DESCRIPTION_HL += testIndexRange.MIN_VALUE + "<= ";
                    }

                    ti.DESCRIPTION_HL += "X";

                    if (testIndexRange.MAX_VALUE != null)
                    {
                        ti.DESCRIPTION_HL += " < " + testIndexRange.MAX_VALUE;
                    }

                    if (ti.VALUE != null && ti.MIN_VALUE != null && ti.MIN_VALUE <= ti.VALUE_DEC && ti.MAX_VALUE != null && ti.VALUE_DEC < ti.MAX_VALUE)
                    {
                        ti.HIGH_OR_LOW = "";
                        ti.VALUE_HL = ti.VALUE + "";
                    }
                    else if (ti.VALUE != null && ti.MIN_VALUE != null && ti.VALUE_DEC < ti.MIN_VALUE)
                    {
                        ti.HIGH_OR_LOW = "L";
                        ti.VALUE_HL = ti.VALUE + "L";
                    }
                    else if (ti.VALUE != null && ti.MAX_VALUE != null && ti.MAX_VALUE <= ti.VALUE_DEC)
                    {
                        ti.HIGH_OR_LOW = "H";
                        ti.VALUE_HL = ti.VALUE + "H";
                    }
                    else
                    {
                        ti.HIGH_OR_LOW = "";
                        ti.VALUE_HL = ti.VALUE + "";
                    }
                }
                else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_MAX == 1)
                {
                    if (testIndexRange.MIN_VALUE != null)
                    {
                        ti.DESCRIPTION_HL += testIndexRange.MIN_VALUE + "<= ";
                    }

                    ti.DESCRIPTION_HL += "X";

                    if (testIndexRange.MAX_VALUE != null)
                    {
                        ti.DESCRIPTION_HL += " <= " + testIndexRange.MAX_VALUE;
                    }

                    if (ti.VALUE != null && ti.MIN_VALUE != null && ti.MIN_VALUE <= ti.VALUE_DEC && ti.MAX_VALUE != null && ti.VALUE_DEC <= ti.MAX_VALUE)
                    {
                        ti.HIGH_OR_LOW = "";
                        ti.VALUE_HL = ti.VALUE + "";
                    }
                    else if (ti.VALUE != null && ti.MIN_VALUE != null && ti.VALUE_DEC < ti.MIN_VALUE)
                    {
                        ti.HIGH_OR_LOW = "L";
                        ti.VALUE_HL = ti.VALUE + "L";
                    }
                    else if (ti.VALUE != null && ti.MAX_VALUE != null && ti.MAX_VALUE < ti.VALUE_DEC)
                    {
                        ti.HIGH_OR_LOW = "H";
                        ti.VALUE_HL = ti.VALUE + "H";
                    }
                    else
                    {
                        ti.HIGH_OR_LOW = "";
                        ti.VALUE_HL = ti.VALUE + "";
                    }
                }
                else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_MAX == 1)
                {
                    if (testIndexRange.MIN_VALUE != null)
                    {
                        ti.DESCRIPTION_HL += testIndexRange.MIN_VALUE + "< ";
                    }

                    ti.DESCRIPTION_HL += "X";

                    if (testIndexRange.MAX_VALUE != null)
                    {
                        ti.DESCRIPTION_HL += " <= " + testIndexRange.MAX_VALUE;
                    }

                    if (ti.VALUE != null && ti.MIN_VALUE != null && ti.MIN_VALUE < ti.VALUE_DEC && ti.MAX_VALUE != null && ti.VALUE_DEC <= ti.MAX_VALUE)
                    {
                        ti.HIGH_OR_LOW = "";
                        ti.VALUE_HL = ti.VALUE + "";
                    }
                    else if (ti.VALUE != null && ti.MIN_VALUE != null && ti.VALUE_DEC < ti.MIN_VALUE)
                    {
                        ti.HIGH_OR_LOW = "L";
                        ti.VALUE_HL = ti.VALUE + "L";
                    }
                    else if (ti.VALUE != null && ti.MAX_VALUE != null && ti.MAX_VALUE < ti.VALUE_DEC)
                    {
                        ti.HIGH_OR_LOW = "H";
                        ti.VALUE_HL = ti.VALUE + "H";
                    }
                    else
                    {
                        ti.HIGH_OR_LOW = "";
                        ti.VALUE_HL = ti.VALUE + "";
                    }
                }
                else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_MAX == null)
                {
                    if (testIndexRange.MIN_VALUE != null)
                    {
                        ti.DESCRIPTION_HL += testIndexRange.MIN_VALUE + "< ";
                    }

                    ti.DESCRIPTION_HL += "X";

                    if (testIndexRange.MAX_VALUE != null)
                    {
                        ti.DESCRIPTION_HL += " < " + testIndexRange.MAX_VALUE;
                    }

                    if (ti.VALUE != null && ti.MIN_VALUE != null && ti.MIN_VALUE < ti.VALUE_DEC && ti.MAX_VALUE != null && ti.VALUE_DEC < ti.MAX_VALUE)
                    {
                        ti.HIGH_OR_LOW = "";
                        ti.VALUE_HL = ti.VALUE + "";
                    }
                    else if (ti.VALUE != null && ti.MIN_VALUE != null && ti.VALUE_DEC <= ti.MIN_VALUE)
                    {
                        ti.HIGH_OR_LOW = "L";
                        ti.VALUE_HL = ti.VALUE + "L";
                    }
                    else if (ti.VALUE != null && ti.MAX_VALUE != null && ti.MAX_VALUE <= ti.VALUE_DEC)
                    {
                        ti.HIGH_OR_LOW = "H";
                        ti.VALUE_HL = ti.VALUE + "H";
                    }
                    else
                    {
                        ti.HIGH_OR_LOW = "";
                        ti.VALUE_HL = ti.VALUE + "";
                    }
                }
            }
        }

        private void ProcessHighLowWarningValue(TestLisResultADO ti, V_HIS_TEST_INDEX_RANGE testIndexRange)
        {
            ti.WARNING_DESCRIPTION = "";

            if (testIndexRange.IS_ACCEPT_EQUAL_WARNING_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_WARNING_MAX == null)
            {
                if (testIndexRange.WARNING_MIN_VALUE != null)
                {
                    ti.WARNING_DESCRIPTION += testIndexRange.WARNING_MIN_VALUE + "<= ";
                }

                //ti.WARNING_DESCRIPTION += " ";

                if (testIndexRange.WARNING_MAX_VALUE != null)
                {
                    ti.WARNING_DESCRIPTION += " < " + testIndexRange.WARNING_MAX_VALUE;
                }

                if (ti.VALUE != null && ti.WARNING_MIN_VALUE != null && ti.VALUE_DEC < ti.WARNING_MIN_VALUE)
                {
                    ti.WARNING_NOTE = "Báo động";
                }
                else if (ti.VALUE != null && ti.WARNING_MAX_VALUE != null && ti.WARNING_MAX_VALUE <= ti.VALUE_DEC)
                {
                    ti.WARNING_NOTE = "Báo động";
                }
            }
            else if (testIndexRange.IS_ACCEPT_EQUAL_WARNING_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_WARNING_MAX == 1)
            {
                if (testIndexRange.WARNING_MIN_VALUE != null)
                {
                    ti.WARNING_DESCRIPTION += testIndexRange.WARNING_MIN_VALUE + "<= ";
                }

                //ti.WARNING_DESCRIPTION += " ";

                if (testIndexRange.WARNING_MAX_VALUE != null)
                {
                    ti.WARNING_DESCRIPTION += " <= " + testIndexRange.WARNING_MAX_VALUE;
                }

                if (ti.VALUE != null && ti.WARNING_MIN_VALUE != null && ti.VALUE_DEC < ti.WARNING_MIN_VALUE)
                {
                    ti.WARNING_NOTE = "Báo động";
                }
                else if (ti.VALUE != null && ti.WARNING_MAX_VALUE != null && ti.WARNING_MAX_VALUE < ti.VALUE_DEC)
                {
                    ti.WARNING_NOTE = "Báo động";
                }
            }
            else if (testIndexRange.IS_ACCEPT_EQUAL_WARNING_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_WARNING_MAX == 1)
            {
                if (testIndexRange.WARNING_MIN_VALUE != null)
                {
                    ti.WARNING_DESCRIPTION += testIndexRange.WARNING_MIN_VALUE + "< ";
                }

                //ti.WARNING_DESCRIPTION += " ";

                if (testIndexRange.WARNING_MAX_VALUE != null)
                {
                    ti.WARNING_DESCRIPTION += " <= " + testIndexRange.WARNING_MAX_VALUE;
                }

                if (ti.VALUE != null && ti.WARNING_MIN_VALUE != null && ti.VALUE_DEC < ti.WARNING_MIN_VALUE)
                {
                    ti.WARNING_NOTE = "Báo động";
                }
                else if (ti.VALUE != null && ti.WARNING_MAX_VALUE != null && ti.WARNING_MAX_VALUE < ti.VALUE_DEC)
                {
                    ti.WARNING_NOTE = "Báo động";
                }
            }
            else if (testIndexRange.IS_ACCEPT_EQUAL_WARNING_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_WARNING_MAX == null)
            {
                if (testIndexRange.WARNING_MIN_VALUE != null)
                {
                    ti.WARNING_DESCRIPTION += testIndexRange.WARNING_MIN_VALUE + "< ";
                }

                //ti.WARNING_DESCRIPTION += " ";

                if (testIndexRange.WARNING_MAX_VALUE != null)
                {
                    ti.WARNING_DESCRIPTION += " < " + testIndexRange.WARNING_MAX_VALUE;
                }

                if (ti.VALUE != null && ti.WARNING_MIN_VALUE != null && ti.VALUE_DEC <= ti.WARNING_MIN_VALUE)
                {
                    ti.WARNING_NOTE = "Báo động";
                }
                else if (ti.VALUE != null && ti.WARNING_MAX_VALUE != null && ti.WARNING_MAX_VALUE <= ti.VALUE_DEC)
                {
                    ti.WARNING_NOTE = "Báo động";
                }
            }
        }

        Image GenerateBarcode(string rawData)
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcode = new Inventec.Common.BarcodeLib.Barcode();
                barcode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcode.Width = 120;
                barcode.Height = 40;
                barcode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcode.IncludeLabel = true;
                barcode.RawData = rawData;
                return barcode.Encode(barcode.EncodedType, barcode.RawData, barcode.Width, barcode.Height);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

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
                            if (!dicSereServExt.ContainsKey(item.SERE_SERV_ID))
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
                Inventec.Common.Logging.LogSystem.Debug("1. Begin ProcessLoadListTemplate ");
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
                    listTemplate = listTemplate.Where(o => (o.SERVICE_TYPE_ID ?? 0) == 0 || SERVICE_TYPE_IDs.Contains(o.SERVICE_TYPE_ID ?? 0)).ToList();
                }
                Inventec.Common.Logging.LogSystem.Debug("1. End ProcessLoadListTemplate ");
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

                    if (sereServExt.END_TIME != null)
                        dtEndTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(sereServExt.END_TIME ?? 0) ?? DateTime.Now;
                    else

                        this.ActionType = GlobalVariables.ActionEdit;
                }
                else
                {
                    sereServExt = null;
                    txtConclude.Text = "";
                    dtBeginTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ServiceReqConstruct.INTRUCTION_TIME) ?? DateTime.Now;
                    txtNote.Text = "";
                    txtDescription.Text = "";
                    this.ActionType = GlobalVariables.ActionAdd;
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
                        txtDescription.RtfText = Utility.TextLibHelper.BytesToStringConverted(dicSarPrint[sereServExt.ID].CONTENT);
                        btnPrint.Enabled = true;
                        BtnEmr.Enabled = true;

                        this.positionFinded = 0;
                        //TODO
                        if (!String.IsNullOrEmpty(sereServExt.DOC_PROTECTED_LOCATION))
                        {
                            positionFinded = Inventec.Common.TypeConvert.Parse.ToInt32(sereServExt.DOC_PROTECTED_LOCATION);
                        }
                        WordProtectedProcess protectedProcess = new WordProtectedProcess();
                        protectedProcess.InitialProtected(txtDescription, ref positionFinded);
                    }
                    else
                    {
                        txtDescription.Text = "";
                        btnPrint.Enabled = false;
                        BtnEmr.Enabled = false;
                    }

                    this.currentSarPrint = dicSarPrint[sereServExt.ID];
                    zoomFactor();
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

        private void ProcessLoadTemplate(HIS_SERE_SERV data)
        {
            try
            {
                if (data != null && data.SERVICE_ID > 0 && listTemplate != null && listTemplate.Count > 0)
                {
                    var temp = listTemplate.Where(o => o.SERVICE_ID == data.SERVICE_ID).ToList();
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

        //private void ProcessLoadEkip(HIS_SERE_SERV sereServ)
        //{
        //    try
        //    {
        //        if (sereServ != null && sereServ.EKIP_ID.HasValue)
        //        {
        //            CommonParam param = new CommonParam();
        //            MOS.Filter.HisEkipUserViewFilter hisEkipUserFilter = new MOS.Filter.HisEkipUserViewFilter();
        //            hisEkipUserFilter.EKIP_ID = sereServ.EKIP_ID;
        //            var lst = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_EKIP_USER>>(ApiConsumer.HisRequestUriStore.HIS_EKIP_USER_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, hisEkipUserFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

        //            if (lst != null && lst.Count > 0)
        //            {
        //                this.ekipUserAdos = new List<HisEkipUserADO>();
        //                foreach (var item in lst)
        //                {
        //                    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_EKIP_USER, HisEkipUserADO>();
        //                    var HisEkipUserProcessing = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_EKIP_USER, HisEkipUserADO>(item);
        //                    if (item != lst[0])
        //                    {
        //                        HisEkipUserProcessing.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
        //                    }
        //                    this.ekipUserAdos.Add(HisEkipUserProcessing);
        //                }
        //            }

        //            gridControlEkip.BeginUpdate();
        //            gridControlEkip.DataSource = null;
        //            gridControlEkip.DataSource = this.ekipUserAdos;
        //            gridControlEkip.EndUpdate();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

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

                        //Inventec.Common.Logging.LogSystem.Debug("3.2.2.1");
                        ProcessChoiceSereServTempl(result.First());
                        //Inventec.Common.Logging.LogSystem.Debug("3.2.2.2");
                        txtDescription.Focus();
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
                    txtDescription.Text = "";
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
                    txtDescription.Focus();
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
                    txtDescription.Focus();
                }
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
                    //if (CheckAllInOne.Checked && this.sereServ != null) InsertRow(this.sereServ);//cập nhật lại dữ liệu
                    var asereServ = (ADO.ServiceADO)gridViewSereServ.GetFocusedRow();
                    if (asereServ != null)
                    {
                        //if (CheckAllInOne.Checked)
                        //    InsertRow(asereServ);
                        //else
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
                //if (CheckAllInOne.Checked && this.sereServ != null) InsertRow(this.sereServ);//cập nhật lại dữ liệu
                var asereServ = (ADO.ServiceADO)gridViewSereServ.GetRow(e.RowHandle);
                if (asereServ != null)
                {
                    //if (CheckAllInOne.Checked)
                    //    InsertRow(asereServ);
                    //else
                    SereServClickRow(asereServ);
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Image_ItemClick(object sender, TileItemEventArgs e)
        {
            try
            {
                //chọn ảnh sẽ add trực tiếp vào txtDescription --- tạm thờ không add được vào text box nên chưa dùng
                tileView1.GetCheckedRows();
                Image image = e.Item.Image;

                e.Item.Checked = !e.Item.Checked;
                if (image != null)
                {
                    if (e.Item.Checked == true)//thêm ảnh
                    {
                        txtDescription.Document.Images.Insert(txtDescription.Document.CaretPosition, image);
                    }
                    else // tìm ảnh đã thêm và xóa
                    {
                        try
                        {
                            DevExpress.XtraRichEdit.API.Native.DocumentImageCollection images = txtDescription.Document.Images;
                            txtDescription.Document.BeginUpdate();
                            foreach (DevExpress.XtraRichEdit.API.Native.DocumentImage item in images)
                            {
                                if (item.Image.NativeImage == image)
                                {
                                    txtDescription.Document.Delete(item.Range);
                                    break;
                                }
                            }
                            txtDescription.Document.EndUpdate();
                        }
                        catch (Exception ex)
                        {
                            txtDescription.Document.EndUpdate();
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void CheckAllInOne_CheckedChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (CheckAllInOne.Checked)
        //        {
        //            this.mainSereServ = this.sereServ;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

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
                                //DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.KhongXacDinhDuocVungChuaAnh);
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
                HIS.Desktop.ADO.AssignPrescriptionADO assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionADO(currentServiceReq.TREATMENT_ID, 0, 0);

                assignServiceADO.PatientDob = currentServiceReq.TDL_PATIENT_DOB;
                assignServiceADO.PatientName = currentServiceReq.TDL_PATIENT_NAME;
                assignServiceADO.GenderName = currentServiceReq.TDL_PATIENT_GENDER_NAME;
                assignServiceADO.TreatmentCode = currentServiceReq.TDL_TREATMENT_CODE;
                assignServiceADO.TreatmentId = currentServiceReq.TREATMENT_ID;

                //xuandv new
                CommonParam param = new CommonParam();
                HisTreatmentFilter filter = new HisTreatmentFilter();
                filter.ID = this.ServiceReqConstruct.TREATMENT_ID;

                var rsApi = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (rsApi != null && rsApi.Count > 0 && rsApi.FirstOrDefault().TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                {
                    assignServiceADO.IsExecutePTTT = true;
                }
                listArgs.Add(assignServiceADO);

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
                List<long> SERVICE_TYPE_IDs = new List<long>()
                {
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN
                };
                listArgs.Add(SERVICE_TYPE_IDs);
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
                    foreach (var item in pmenus)
                    {
                        bItems.Add((BarItem)item);
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

        private void PrintResult(bool printNow)
        {
            try
            {
                if (!btnPrint.Enabled || sereServ.isNoPay) return;

                MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                filter.ID = ServiceReqConstruct.ID;
                var lstServiceReq = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>(RequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                long? finishTime = null;
                if (lstServiceReq != null && lstServiceReq.Count > 0)
                {
                    finishTime = lstServiceReq.FirstOrDefault().FINISH_TIME;
                }
                var printDocument = new DevExpress.XtraRichEdit.RichEditControl();
                printDocument.RtfText = txtDescription.RtfText;
                if (!String.IsNullOrWhiteSpace(thoiGianKetThuc))
                {
                    foreach (var section in printDocument.Document.Sections)
                    {
                        if (HideTimePrint != "1")
                        {
                            section.Margins.HeaderOffset = 50;
                            section.Margins.FooterOffset = 50;
                            var myHeader = section.BeginUpdateHeader(DevExpress.XtraRichEdit.API.Native.HeaderFooterType.Odd);
                            //xóa header nếu có dữ liệu
                            myHeader.Delete(myHeader.Range);

                            myHeader.InsertText(myHeader.CreatePosition(0),
                                String.Format(ResourceMessage.NgayIn, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
                            myHeader.Fields.Update();
                            section.EndUpdateHeader(myHeader);
                        }

                        string finishTimeStr = "";

                        if (finishTime.HasValue)
                        {
                            finishTimeStr = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(finishTime.Value);
                        }

                        var rangeSeperators = printDocument.Document.FindAll(
                            thoiGianKetThuc,
                            DevExpress.XtraRichEdit.API.Native.SearchOptions.CaseSensitive);

                        if (rangeSeperators != null && rangeSeperators.Length > 0)
                        {
                            for (int i = 0; i < rangeSeperators.Length; i++)
                                printDocument.Document.Replace(rangeSeperators[i], finishTimeStr);
                        }
                    }
                }

                if (printNow)
                {
                    printDocument.Print();
                }
                else if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    printDocument.Print();
                }
                else
                {
                    printDocument.ShowPrintPreview();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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
                    //btnPrint.Enabled = false;
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

                SaveProcessor(false);
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
                //if (CheckAllInOne.Checked)
                //{
                //    InsertRow(this.sereServ);//cập nhật lại dữ liệu
                //    SaveAllProcess(true);
                //}
                //else
                SaveProcessor(true);
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

                V_HIS_SERE_SERV sereServInput = new V_HIS_SERE_SERV();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV>(sereServInput, sereServ);
                List<object> listArgs = new List<object>();
                HIS.Desktop.ADO.AssignPrescriptionADO assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionADO(currentServiceReq.TREATMENT_ID, 0, currentServiceReq.ID, sereServInput);
                assignServiceADO.IsCabinet = true;
                assignServiceADO.PatientDob = currentServiceReq.TDL_PATIENT_DOB;
                assignServiceADO.PatientName = currentServiceReq.TDL_PATIENT_NAME;
                assignServiceADO.GenderName = currentServiceReq.TDL_PATIENT_GENDER_NAME;
                assignServiceADO.TreatmentCode = currentServiceReq.TDL_TREATMENT_CODE;
                assignServiceADO.TreatmentId = currentServiceReq.TREATMENT_ID;

                listArgs.Add(assignServiceADO);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.AssignPrescriptionCLS", this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

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

        private void SaveProcessor(bool printNow)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("SaveProcessor. 1");
                CommonParam param = new CommonParam();
                bool success = false;
                isPressButtonSave = true;
                if (!dxValidationProvider1.Validate()) return;
                WaitingManager.Show();
                if (sereServ == null) return;
                if (sereServ.isNoPay) return;//chưa thanh toán ko lưu được
                if (!sereServ.isSave)
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

                ProcessDescriptionContent();
                if (ProcessSereServExt__DescriptionPrint(param, sereServ))
                {
                    Inventec.Desktop.Common.Message.WaitingManager.Hide();
                    Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                    return;
                }
                List<FileHolder> listFileHolder = new List<FileHolder>();
                Inventec.Common.Logging.LogSystem.Debug("SaveProcessor. 2");
                var machine = ListMachine != null ? ListMachine.FirstOrDefault(o => o.ID == sereServ.MACHINE_ID) : null;
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

                HisSereServExtSDO data = new HisSereServExtSDO();
                data.HisSereServExt = this.sereServExt;
                Inventec.Common.Logging.LogSystem.Debug("SaveProcessor. 3");

                //if (this.listImage != null && this.listImage.Count > 0 && chkAttachImage.Checked)
                //{
                //    data.Files = ProcessImageList(this.listImage.Where(o => o.IsChecked).ToList());
                //}

                MOS.SDO.HisSereServExtWithFileSDO apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post
                    <MOS.SDO.HisSereServExtWithFileSDO>
                    (this.sereServExt.ID == 0 ?
                    RequestUriStore.HIS_SERE_SERV_EXT_CREATE_SDO :
                    RequestUriStore.HIS_SERE_SERV_EXT_UPDATE_SDO,
                    ApiConsumer.ApiConsumers.MosConsumer, param, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken);
                if (apiResult != null)
                {
                    if (dicSereServExt.ContainsKey(apiResult.SereServExt.SERE_SERV_ID))
                    {
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

                    success = true;
                    if (listServiceADO != null && listServiceADO.Count > 0)
                    {
                        foreach (var item in listServiceADO)
                        {
                            if (item.ID == this.sereServ.ID)
                            {
                                item.isSave = true;
                                break;
                            }
                        }
                    }
                    btnPrint.Enabled = true;
                    BtnEmr.Enabled = true;
                    this.sereServ.isSave = true;
                    //SereServClickRow(this.sereServ);

                    //ẩn trước khi lưu đóng tránh bị dừng pm
                    WaitingManager.Hide();
                    //lưu và đóng
                    if (printNow && apiResult != null)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.BanCoMuonInKetQua,
                    ResourceMessage.ThongBao,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            PrintResult(true);
                        }

                        btnFinish_Click(null, null);//chỉ kết thúc khi tất cả đã thực hiện

                        TabControlBaseProcess.CloseSelectedTabPage(SessionManager.GetTabControlMain());
                    }
                    Inventec.Common.Logging.LogSystem.Debug("SaveProcessor. 4");
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

        private void SaveAllProcess(bool printNow)
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
                    //if (sereServ.isNoPay) return;//chưa thanh toán ko lưu được
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
                    if (dtBeginTime.EditValue != null)
                        sereServExt.BEGIN_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtBeginTime.DateTime);
                    else
                        sereServExt.BEGIN_TIME = null;

                    if (dtEndTime.EditValue != null)
                        sereServExt.END_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtEndTime.DateTime);
                    else
                        sereServExt.END_TIME = null;

                    var machine = ListMachine != null ? ListMachine.FirstOrDefault(o => o.ID == sereServ.MACHINE_ID) : null;
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

                    if (ProcessSereServExt__DescriptionPrint(param, sereServ, sereServExt))
                    {
                        Inventec.Desktop.Common.Message.WaitingManager.Hide();
                        Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        return;
                    }
                    List<FileHolder> listFileHolder = new List<FileHolder>();

                    HisSereServExtSDO data = new HisSereServExtSDO();
                    data.HisSereServExt = sereServExt;

                    apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post
                        <MOS.SDO.HisSereServExtWithFileSDO>
                        (sereServExt.ID == 0 ?
                        RequestUriStore.HIS_SERE_SERV_EXT_CREATE_SDO :
                        RequestUriStore.HIS_SERE_SERV_EXT_UPDATE_SDO,
                        ApiConsumer.ApiConsumers.MosConsumer, param, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken);
                    if (apiResult != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiResult), apiResult));
                        if (dicSereServExt.ContainsKey(apiResult.SereServExt.SERE_SERV_ID))
                        {
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

                        success = true;

                        sereServ.isSave = true;

                        foreach (var ado in listServiceADO)
                        {
                            if (ado.ID == sereServ.ID)
                            {
                                ado.isSave = sereServ.isSave;
                                break;
                            }
                        }
                        btnPrint.Enabled = true;
                        BtnEmr.Enabled = true;
                    }
                }

                //ẩn trước khi lưu đóng tránh bị dừng pm
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
                //lưu và đóng
                if (printNow && apiResult != null)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.BanCoMuonInKetQua,
                ResourceMessage.ThongBao,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        PrintResult(true);
                    }

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

        private void gridView_DoubleClick(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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
                MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                filter.ID = ServiceReqConstruct.ID;
                var lstServiceReq = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>(RequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                long? finishTime = null;
                if (lstServiceReq != null && lstServiceReq.Count > 0)
                {
                    finishTime = lstServiceReq.FirstOrDefault().FINISH_TIME;
                }
                var printDocument = new DevExpress.XtraRichEdit.RichEditControl();
                printDocument.RtfText = txtDescription.RtfText;
                if (!String.IsNullOrWhiteSpace(thoiGianKetThuc))
                {
                    foreach (var section in printDocument.Document.Sections)
                    {
                        if (HideTimePrint != "1")
                        {
                            section.Margins.HeaderOffset = 50;
                            section.Margins.FooterOffset = 50;
                            var myHeader = section.BeginUpdateHeader(DevExpress.XtraRichEdit.API.Native.HeaderFooterType.Odd);
                            //xóa header nếu có dữ liệu
                            myHeader.Delete(myHeader.Range);

                            myHeader.InsertText(myHeader.CreatePosition(0),
                                String.Format(ResourceMessage.NgayIn, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
                            myHeader.Fields.Update();
                            section.EndUpdateHeader(myHeader);
                        }

                        string finishTimeStr = "";

                        if (finishTime.HasValue)
                        {
                            finishTimeStr = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(finishTime.Value);
                        }

                        var rangeSeperators = printDocument.Document.FindAll(
                            thoiGianKetThuc,
                            DevExpress.XtraRichEdit.API.Native.SearchOptions.CaseSensitive);

                        if (rangeSeperators != null && rangeSeperators.Length > 0)
                        {
                            for (int i = 0; i < rangeSeperators.Length; i++)
                                printDocument.Document.Replace(rangeSeperators[i], finishTimeStr);
                        }
                    }
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
        #endregion

        #region process word
        private void ProcessChoiceSereServTempl(MOS.EFMODEL.DataModels.HIS_SERE_SERV_TEMP data)
        {
            try
            {
                if (data != null)
                {
                    WaitingManager.Show();
                    txtConclude.Text = data.CONCLUDE;
                    txtDescription.RtfText = Utility.TextLibHelper.BytesToStringConverted(data.DESCRIPTION);
                    zoomFactor();
                    ProcessDescriptionContent();

                    this.positionFinded = 0;
                    WordProtectedProcess protectedProcess = new WordProtectedProcess();
                    protectedProcess.InitialProtected(txtDescription, ref positionFinded);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void ProcessSereServFileExecute(List<FileHolder> listFileHolder)
        {
            try
            {
                if (listImage == null || listImage.Count <= 0) return;
                List<ADO.ImageADO> imageItems = listImage.Where(o => o.IsChecked == true).ToList();
                if (imageItems != null && imageItems.Count > 0)
                {
                    for (int i = 0; i < imageItems.Count; i++)
                    {
                        FileHolder file = new FileHolder();
                        file.FileName = imageItems[i].FileName;
                        file.Content = new System.IO.MemoryStream();
                        imageItems[i].IMAGE_DISPLAY.Save(file.Content, System.Drawing.Imaging.ImageFormat.Png);
                        file.Content.Position = 0;
                        listFileHolder.Add(file);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool ProcessSereServExt__DescriptionPrint(CommonParam param, HIS_SERE_SERV sereServ)
        {
            bool result = true;
            try
            {
                WordProtectedProcess protectedProcess = new WordProtectedProcess();
                protectedProcess.InitialProtected(txtDescription, ref positionFinded);


                //Khi luu se lay anh duoc chon load vao noi dung word editor
                //noi dung nay luu vao sarprint & co truong json_description_id trong sereserv
                //data.DESCRIPTION_SAR_PRINT_ID = Encoding.UTF8.GetBytes(txtDescription.RtfText);
                //Xu ly txtDescription -> replace key data + image
                if (this.ActionType == GlobalVariables.ActionEdit && currentSarPrint != null && currentSarPrint.ID > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("ProcessSereServExt__DescriptionPrint. 1");

                    currentSarPrint.CONTENT = Encoding.UTF8.GetBytes(txtDescription.RtfText);
                    this.currentSarPrint = new Inventec.Common.Adapter.BackendAdapter(param).Post<SAR.EFMODEL.DataModels.SAR_PRINT>(ApiConsumer.SarRequestUriStore.SAR_PRINT_UPDATE, ApiConsumer.ApiConsumers.SarConsumer, currentSarPrint, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (currentSarPrint != null) result = false;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("ProcessSereServExt__DescriptionPrint. 2");
                    this.currentSarPrint = new SAR.EFMODEL.DataModels.SAR_PRINT();
                    SAR.EFMODEL.DataModels.SAR_PRINT dataSarPrint = new SAR.EFMODEL.DataModels.SAR_PRINT();
                    dataSarPrint.DESCRIPTION = sereServ.TDL_SERVICE_NAME;
                    dataSarPrint.TITLE = sereServ.TDL_SERVICE_NAME;
                    dataSarPrint.CONTENT = Encoding.UTF8.GetBytes(txtDescription.RtfText);
                    this.currentSarPrint = new Inventec.Common.Adapter.BackendAdapter(param).Post<SAR.EFMODEL.DataModels.SAR_PRINT>(ApiConsumer.SarRequestUriStore.SAR_PRINT_CREATE, ApiConsumer.ApiConsumers.SarConsumer, dataSarPrint, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (this.currentSarPrint != null && this.currentSarPrint.ID > 0)
                    {
                        this.sereServExt.DESCRIPTION_SAR_PRINT_ID = this.currentSarPrint.ID + "";
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }
                }

                this.sereServExt.DOC_PROTECTED_LOCATION = (positionFinded > 0 ? positionFinded + "" : "");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = true;
            }
            return result;
        }

        private bool ProcessSereServExt__DescriptionPrint(CommonParam param, ADO.ServiceADO sereServ, HIS_SERE_SERV_EXT sereServExt)
        {
            bool result = true;
            try
            {
                //Khi luu se lay anh duoc chon load vao noi dung word editor
                //noi dung nay luu vao sarprint & co truong json_description_id trong sereserv
                //data.DESCRIPTION_SAR_PRINT_ID = Encoding.UTF8.GetBytes(txtDescription.RtfText);
                //Xu ly txtDescription -> replace key data + image
                if (sereServ.isSave && !String.IsNullOrEmpty(sereServExt.DESCRIPTION_SAR_PRINT_ID))
                {
                    if (currentSarPrint == null)
                    {
                        currentSarPrint = new SAR.EFMODEL.DataModels.SAR_PRINT();
                        currentSarPrint.DESCRIPTION = sereServ.TDL_SERVICE_NAME;
                        currentSarPrint.TITLE = sereServ.TDL_SERVICE_NAME;
                    }
                    currentSarPrint.ID = Convert.ToInt64(sereServExt.DESCRIPTION_SAR_PRINT_ID);
                    currentSarPrint.CONTENT = Encoding.UTF8.GetBytes(txtDescription.RtfText);
                    this.currentSarPrint = new Inventec.Common.Adapter.BackendAdapter(param).Post<SAR.EFMODEL.DataModels.SAR_PRINT>(ApiConsumer.SarRequestUriStore.SAR_PRINT_UPDATE, ApiConsumer.ApiConsumers.SarConsumer, currentSarPrint, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (currentSarPrint != null) result = false;
                }
                else
                {
                    this.currentSarPrint = new SAR.EFMODEL.DataModels.SAR_PRINT();
                    SAR.EFMODEL.DataModels.SAR_PRINT dataSarPrint = new SAR.EFMODEL.DataModels.SAR_PRINT();
                    dataSarPrint.DESCRIPTION = sereServ.TDL_SERVICE_NAME;
                    dataSarPrint.TITLE = sereServ.TDL_SERVICE_NAME;
                    dataSarPrint.CONTENT = Encoding.UTF8.GetBytes(txtDescription.RtfText);
                    this.currentSarPrint = new Inventec.Common.Adapter.BackendAdapter(param).Post<SAR.EFMODEL.DataModels.SAR_PRINT>(ApiConsumer.SarRequestUriStore.SAR_PRINT_CREATE, ApiConsumer.ApiConsumers.SarConsumer, dataSarPrint, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (this.currentSarPrint != null && this.currentSarPrint.ID > 0)
                    {
                        sereServExt.DESCRIPTION_SAR_PRINT_ID = this.currentSarPrint.ID + "";
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = true;
            }
            return result;
        }

        private void ProcessDescriptionContent()
        {
            DevExpress.XtraRichEdit.API.Native.Document doc = txtDescription.Document;
            try
            {
                if (!String.IsNullOrEmpty(txtDescription.Text))
                {
                    ProcessDicParam();
                    ProcessTestServiceResultKey();

                    if (this.dicParam != null && this.dicParam.Count > 0)
                    {
                        Inventec.Common.RichEditor.ProcessTag.ProcessSingleTag singleTag = new Inventec.Common.RichEditor.ProcessTag.ProcessSingleTag();

                        doc.BeginUpdate();

                        Inventec.Common.RichEditor.ProcessTag.Store store = new Inventec.Common.RichEditor.ProcessTag.Store(doc);
                        Inventec.Common.RichEditor.ProcessTag.ProcessSingleTag processSingleTag = new Inventec.Common.RichEditor.ProcessTag.ProcessSingleTag();
                        Inventec.Common.RichEditor.ProcessTag.ProcessImageTag processImageTag = new Inventec.Common.RichEditor.ProcessTag.ProcessImageTag();

                        processSingleTag.ProcessData(store, this.dicParam);

                        if (this.dicImage != null && this.dicImage.Count > 0)
                            processImageTag.ProcessData(store, this.dicImage);

                        doc.EndUpdate();
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info("txtDescription.Text is empty");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                doc.EndUpdate();
            }
        }

        private void InsertRow(ADO.ServiceADO sereServ)
        {
            Document document = txtDescription.Document;
            try
            {
                //CheckAllInOne.ReadOnly = true;//đã tiến hành xử lý thì không cho đổi nữa

                //nếu trùng với biến toàn cục thì là cập nhật dữ liệu
                if (sereServ.ID == this.sereServ.ID)
                {
                    sereServ.note = txtNote.Text;
                    sereServ.conclude = txtConclude.Text;

                    if (!String.IsNullOrEmpty(txtNumberOfFilm.Text))
                        sereServ.NUMBER_OF_FILM = long.Parse(txtNumberOfFilm.Text);
                    else
                        sereServ.NUMBER_OF_FILM = null;
                }
                else
                {
                    txtConclude.Text = null;
                    txtNote.Text = null;
                    txtNumberOfFilm.Text = null;
                }
                ProcessUpdateConclude(sereServ);
                bool inserted = true;
                var isAdd = listServiceADOForAllInOne.Where(o => o.ID == sereServ.ID).ToList();
                if (isAdd == null || isAdd.Count <= 0)
                {
                    inserted = false;
                    listServiceADOForAllInOne.Add(sereServ);
                }

                //cập nhật danh sách
                foreach (var item in listServiceADOForAllInOne)
                {
                    if (item.ID == sereServ.ID)
                    {
                        item.conclude = sereServ.conclude;
                        item.note = sereServ.note;
                        item.MACHINE_ID = sereServ.MACHINE_ID;
                        item.NUMBER_OF_FILM = sereServ.NUMBER_OF_FILM;
                        break;
                    }
                }

                this.sereServ = sereServ;

                Table table = null;
                if (document.Tables != null && document.Tables.Count > 0)
                {
                    foreach (var item in document.Tables)
                    {
                        if (item.FirstRow.Cells.Count > 2)
                        {
                            table = item;
                            break;
                        }
                    }
                }
                if (table == null) return;
                int index = 1;
                document.BeginUpdate();
                //lấy ra vị trí của dịch vụ đang xử lý
                if (!inserted)
                {
                    var row = table.Rows.InsertAfter(table.Rows.Last.Index);
                    index = row.Index;
                    foreach (var item in listServiceADOForAllInOne)
                    {
                        if (item.ID == sereServ.ID)
                        {
                            item.tableIndex = row.Index;
                            break;
                        }
                    }
                }
                else if (mainSereServ.ID == sereServ.ID && (isAdd == null || isAdd.Count <= 0))
                {
                    var mainSs = listServiceADOForAllInOne.FirstOrDefault(o => o.ID == mainSereServ.ID);
                    int lastIndex = listServiceADOForAllInOne.Count > 0 ? listServiceADOForAllInOne.Min(o => o.tableIndex) - 1 : table.Rows.Last.Index;
                    mainSs.tableIndex = lastIndex > 0 ? lastIndex : table.Rows.Last.Index;
                }
                else
                {
                    index = sereServ.tableIndex;
                }
                // Insert Cells Values and format the columns
                // Xóa dữ liệu cũ và chèn lại
                document.Delete(table[index, 1].Range);
                document.InsertText(table[index, 1].Range.Start, sereServ.TDL_SERVICE_NAME);
                document.Delete(table[index, 2].Range);
                document.InsertText(table[index, 2].Range.Start, txtConclude.Text);
                document.EndUpdate();
            }
            catch (Exception ex)
            {
                document.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessUpdateConclude(ADO.ServiceADO sereServ)
        {
            try
            {
                //lọc theo đúng service để lấy ra kết luận gắn với mẫu tương ứng
                var listtemplate = listTemplate.Where(o => o.SERVICE_ID == sereServ.SERVICE_ID).ToList();
                if (listtemplate != null && listtemplate.Count > 0)
                {
                    var temp = listtemplate.OrderByDescending(o => o.MODIFY_TIME).FirstOrDefault();
                    if (String.IsNullOrWhiteSpace(txtConclude.Text))
                    {
                        txtConclude.Text = temp.CONCLUDE;
                    }
                }

                if (!String.IsNullOrEmpty(sereServ.conclude) || !String.IsNullOrEmpty(sereServ.note))
                {
                    txtConclude.Text = sereServ.conclude;
                    txtNote.Text = sereServ.note;
                }

                txtNumberOfFilm.Text = sereServ.NUMBER_OF_FILM.HasValue ? sereServ.NUMBER_OF_FILM.ToString() : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDicParam()
        {
            try
            {
                // chế biến dữ liệu thành các key đơn thêm vào biểu mẫu tương tự như mps excel

                this.dicParam = new Dictionary<string, object>();
                this.dicImage = new Dictionary<string, Image>();

                HIS.Desktop.Print.SetCommonKey.SetCommonSingleKey(this.dicParam);//commonkey
                if (currentServiceReq != null)
                {
                    dicParam.Add("INTRUCTION_TIME_FULL_STR",
                            GetCurrentTimeSeparateBeginTime(
                            Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                            currentServiceReq.INTRUCTION_TIME) ?? DateTime.Now));

                    dicParam.Add("INTRUCTION_DATE_FULL_STR",
                        Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(
                        currentServiceReq.INTRUCTION_TIME));

                    dicParam.Add("INTRUCTION_TIME_STR",
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(
                            currentServiceReq.INTRUCTION_TIME));

                    dicParam.Add("START_TIME_STR",
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(
                            currentServiceReq.START_TIME ?? 0));

                    dicParam.Add("START_TIME_FULL_STR",
                            GetCurrentTimeSeparateBeginTime(
                            Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                            currentServiceReq.START_TIME ?? 0) ?? DateTime.Now));

                    dicParam.Add("ICD_MAIN_TEXT", currentServiceReq.ICD_NAME);

                    dicParam.Add("NATIONAL_NAME", currentServiceReq.TDL_PATIENT_NATIONAL_NAME);
                    dicParam.Add("WORK_PLACE", currentServiceReq.TDL_PATIENT_WORK_PLACE_NAME);
                    dicParam.Add("ADDRESS", currentServiceReq.TDL_PATIENT_ADDRESS);
                    dicParam.Add("CAREER_NAME", currentServiceReq.TDL_PATIENT_CAREER_NAME);
                    dicParam.Add("PATIENT_CODE", currentServiceReq.TDL_PATIENT_CODE);
                    dicParam.Add("DISTRICT_CODE", currentServiceReq.TDL_PATIENT_DISTRICT_CODE);
                    dicParam.Add("GENDER_NAME", currentServiceReq.TDL_PATIENT_GENDER_NAME);
                    dicParam.Add("MILITARY_RANK_NAME", currentServiceReq.TDL_PATIENT_MILITARY_RANK_NAME);
                    dicParam.Add("VIR_ADDRESS", currentServiceReq.TDL_PATIENT_ADDRESS);
                    dicParam.Add("AGE", CalculatorAge(currentServiceReq.TDL_PATIENT_DOB, false));
                    dicParam.Add("STR_YEAR", currentServiceReq.TDL_PATIENT_DOB.ToString().Substring(0, 4));
                    dicParam.Add("VIR_PATIENT_NAME", currentServiceReq.TDL_PATIENT_NAME);

                    var executeRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentServiceReq.EXECUTE_ROOM_ID);
                    if (executeRoom != null)
                    {
                        dicParam.Add("EXECUTE_DEPARTMENT_CODE", executeRoom.DEPARTMENT_CODE);
                        dicParam.Add("EXECUTE_DEPARTMENT_NAME", executeRoom.DEPARTMENT_NAME);
                        dicParam.Add("EXECUTE_ROOM_CODE", executeRoom.ROOM_CODE);
                        dicParam.Add("EXECUTE_ROOM_NAME", executeRoom.ROOM_NAME);
                    }

                    var reqRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentServiceReq.REQUEST_ROOM_ID);
                    if (reqRoom != null)
                    {
                        dicParam.Add("REQUEST_DEPARTMENT_CODE", reqRoom.DEPARTMENT_CODE);
                        dicParam.Add("REQUEST_DEPARTMENT_NAME", reqRoom.DEPARTMENT_NAME);
                        dicParam.Add("REQUEST_ROOM_CODE", reqRoom.ROOM_CODE);
                        dicParam.Add("REQUEST_ROOM_NAME", reqRoom.ROOM_NAME);
                    }
                }

                if (TreatmentWithPatientTypeAlter != null)
                {
                    if (!String.IsNullOrEmpty(TreatmentWithPatientTypeAlter.HEIN_CARD_NUMBER))
                    {
                        dicParam.Add("HEIN_CARD_NUMBER_SEPARATE",
                            HeinCardHelper.SetHeinCardNumberDisplayByNumber(TreatmentWithPatientTypeAlter.HEIN_CARD_NUMBER));
                        dicParam.Add("STR_HEIN_CARD_FROM_TIME",
                            Inventec.Common.DateTime.Convert.TimeNumberToDateString(
                            TreatmentWithPatientTypeAlter.HEIN_CARD_FROM_TIME));
                        dicParam.Add("STR_HEIN_CARD_TO_TIME",
                            Inventec.Common.DateTime.Convert.TimeNumberToDateString(
                            TreatmentWithPatientTypeAlter.HEIN_CARD_TO_TIME));
                        dicParam.Add("HEIN_CARD_ADDRESS", TreatmentWithPatientTypeAlter.HEIN_CARD_ADDRESS);
                    }
                    else
                    {
                        dicParam.Add("HEIN_CARD_NUMBER_SEPARATE", "");
                        dicParam.Add("STR_HEIN_CARD_FROM_TIME", "");
                        dicParam.Add("STR_HEIN_CARD_TO_TIME", "");
                        dicParam.Add("HEIN_CARD_ADDRESS", "");
                    }
                    var patientType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == TreatmentWithPatientTypeAlter.PATIENT_TYPE_CODE);
                    if (patientType != null)
                        dicParam.Add("PATIENT_TYPE_NAME", patientType.PATIENT_TYPE_NAME);
                    else
                        dicParam.Add("PATIENT_TYPE_NAME", "");

                    var treatmentType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.TREATMENT_TYPE_CODE == TreatmentWithPatientTypeAlter.TREATMENT_TYPE_CODE);
                    if (treatmentType != null)
                        dicParam.Add("TREATMENT_TYPE_NAME", treatmentType.TREATMENT_TYPE_NAME);
                    else
                        dicParam.Add("TREATMENT_TYPE_NAME", "");

                    dicParam.Add("TREATMENT_ICD_CODE", TreatmentWithPatientTypeAlter.ICD_CODE);
                    dicParam.Add("TREATMENT_ICD_NAME", TreatmentWithPatientTypeAlter.ICD_NAME);
                    dicParam.Add("TREATMENT_ICD_SUB_CODE", TreatmentWithPatientTypeAlter.ICD_SUB_CODE);
                    dicParam.Add("TREATMENT_ICD_TEXT", TreatmentWithPatientTypeAlter.ICD_TEXT);

                    AddKeyIntoDictionaryPrint<HisTreatmentWithPatientTypeInfoSDO>(TreatmentWithPatientTypeAlter, this.dicParam, false);
                }
                else
                {
                    dicParam.Add("HEIN_CARD_NUMBER_SEPARATE", "");
                    dicParam.Add("STR_HEIN_CARD_FROM_TIME", "");
                    dicParam.Add("STR_HEIN_CARD_TO_TIME", "");
                    dicParam.Add("HEIN_CARD_ADDRESS", "");
                    var typeAlter = new HisTreatmentWithPatientTypeInfoSDO();
                    AddKeyIntoDictionaryPrint<HisTreatmentWithPatientTypeInfoSDO>(typeAlter, this.dicParam, false);
                }

                if (patient != null)
                    AddKeyIntoDictionaryPrint<ADO.PatientADO>(patient, this.dicParam, false);

                AddKeyIntoDictionaryPrint<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>(this.currentServiceReq, this.dicParam, true);
                AddKeyIntoDictionaryPrint<MOS.EFMODEL.DataModels.HIS_SERE_SERV>(this.sereServ, this.dicParam, false);
                AddKeyIntoDictionaryPrint<HIS_SERE_SERV_EXT>(this.sereServExt, this.dicParam, true);

                dicParam.Add("USER_NAME", UserName);
                if (!isPressButtonSave)
                {
                    dicParam.Remove("CONCLUDE");
                    dicParam.Remove("NOTE");
                }
                dicParam.Add("CONCLUDE_NEW", "<#CONCLUDE;>");
                dicParam.Add("NOTE_NEW", "<#NOTE;>");

                List<ADO.ImageADO> image = listImage != null ? listImage.Where(o => o.IsChecked == true).ToList() : null;
                dicImage = new Dictionary<string, Image>();
                if (image != null && image.Count > 0)
                {
                    for (int i = 0; i < image.Count; i++)
                    {
                        //fix cứng size ảnh thành 250x140 để add vào kết quả
                        dicImage.Add("IMAGE_DATA_" + (i + 1), ResizeImage(image[i].IMAGE_DISPLAY, 250, 140));
                        //image[i].IsChecked = false;
                    }

                    if (image.Count < 10)
                    {
                        for (int i = image.Count; i < 10; i++)
                        {
                            dicImage.Add("IMAGE_DATA_" + (i + 1), null);
                        }
                    }
                }
                else if (isPressButtonSave)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        dicImage.Add("IMAGE_DATA_" + (i + 1), null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataForTemplate()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("1. Begin ProcessDataForTemplate ");
                patient = GetPatientById(this.ServiceReqConstruct.TDL_PATIENT_ID);
                Inventec.Common.Logging.LogSystem.Info("1. End ProcessDataForTemplate ");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private ADO.PatientADO GetPatientById(long patientId)
        {
            ADO.PatientADO currentPatientADO = new ADO.PatientADO();
            MOS.EFMODEL.DataModels.V_HIS_PATIENT patient = new V_HIS_PATIENT();
            try
            {
                MOS.Filter.HisPatientViewFilter patientFilter = new MOS.Filter.HisPatientViewFilter();
                patientFilter.ID = patientId;
                CommonParam param = new CommonParam();
                var patients = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_PATIENT>>(ApiConsumer.HisRequestUriStore.HIS_PATIENT_GET, ApiConsumer.ApiConsumers.MosConsumer, patientFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (patients != null && patients.Count > 0)
                {
                    patient = patients.FirstOrDefault();
                    currentPatientADO = new ADO.PatientADO(patient);
                }
            }
            catch (Exception ex)
            {
                currentPatientADO = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return currentPatientADO;
        }

        private void UncheckImage()
        {
            try
            {
                if (listImage != null && listImage.Count > 0)
                {
                    List<ADO.ImageADO> image = listImage.Where(o => o.IsChecked == true).ToList();
                    if (image != null && image.Count > 0)
                    {
                        foreach (var item in image)
                        {
                            item.IsChecked = false;
                        }
                        cardControl.RefreshDataSource();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region library
        /// <summary>
        /// gán dữ liệu vào diction để fill data vào word
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="dicParamPlus"></param>
        /// <param name="autoOveride"> ghi đè dữ liệu</param>
        private void AddKeyIntoDictionaryPrint<T>(T data, Dictionary<string, object> dicParamPlus, bool autoOveride)
        {
            try
            {
                if (data != null)
                {
                    PropertyInfo[] pis = typeof(T).GetProperties();
                    if (pis != null && pis.Length > 0)
                    {
                        foreach (var pi in pis)
                        {
                            if (pi.GetGetMethod().IsVirtual) continue;

                            var searchKey = dicParamPlus.SingleOrDefault(o => o.Key == pi.Name);
                            if (String.IsNullOrEmpty(searchKey.Key))
                            {
                                dicParamPlus.Add(pi.Name, pi.GetValue(data));
                            }
                            else
                            {
                                if (autoOveride)
                                    dicParamPlus[pi.Name] = pi.GetValue(data);
                                else if (dicParamPlus[pi.Name] == null)
                                    dicParamPlus[pi.Name] = pi.GetValue(data);
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

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destImage = new Bitmap(width, height);

            using (Graphics graphics = Graphics.FromImage((Image)destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphics.DrawImage(image, 0, 0, width, height);
            }

            return destImage;
        }

        /// <summary>
        /// Hiển thị định dạng 23:59 Ngày 12 tháng 10 năm 2015
        /// </summary>
        /// <returns></returns>
        internal static string GetCurrentTimeSeparateBeginTime(System.DateTime now)
        {
            string result = "";
            try
            {
                if (now != DateTime.MinValue)
                {
                    string month = string.Format("{0:00}", now.Month);
                    string day = string.Format("{0:00}", now.Day);
                    string hour = string.Format("{0:00}", now.Hour);
                    string hours = string.Format("{0:00}", now.Hour);
                    string minute = string.Format("{0:00}", now.Minute);
                    string strNgay = "ngày";
                    string strThang = "tháng";
                    string strNam = "năm";
                    result = string.Format("{0}" + ":" + "{1} " + strNgay + " {2} " + strThang + " {3} " + strNam + " {4}", hours, minute, now.Day, now.Month, now.Year);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        private string CalculatorAge(long ageYearNumber, bool isHl7)
        {
            string tuoi = "";
            try
            {
                string caption__Tuoi = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__AGE");
                string caption__ThangTuoi = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__MONTH_OLDS");
                string caption__NgayTuoi = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__DAY_OLDS");
                string caption__GioTuoi = Resources.ResourceLanguageManager.GetValue(
                    "IVT_LANGUAGE_KEY__UC_SERE_SERV_EXECUTE__HOURS_OLDS");

                if (isHl7)
                {
                    caption__Tuoi = "T";
                    caption__ThangTuoi = "TH";
                    caption__NgayTuoi = "NT";
                    caption__GioTuoi = "GT";
                }

                if (ageYearNumber > 0)
                {
                    System.DateTime dtNgSinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ageYearNumber).Value;
                    if (dtNgSinh == System.DateTime.MinValue) throw new ArgumentNullException("dtNgSinh");

                    TimeSpan diff__hour = (System.DateTime.Now - dtNgSinh);
                    TimeSpan diff__month = (System.DateTime.Now.Date - dtNgSinh.Date);

                    //- Dưới 24h: tính chính xác đến giờ.
                    double hour = diff__hour.TotalHours;
                    if (hour < 24)
                    {
                        tuoi = ((int)hour + " " + caption__GioTuoi);
                    }
                    else
                    {
                        long tongsogiay__hour = diff__hour.Ticks;
                        System.DateTime newDate__hour = new System.DateTime(tongsogiay__hour);
                        int month__hour = ((newDate__hour.Year - 1) * 12 + newDate__hour.Month - 1);
                        if (month__hour == 0)
                        {
                            //Nếu Bn trên 24 giờ và dưới 1 tháng tuổi => hiển thị "xyz ngày tuổi"
                            tuoi = ((int)diff__month.TotalDays + " " + caption__NgayTuoi);
                        }
                        else
                        {
                            long tongsogiay = diff__month.Ticks;
                            System.DateTime newDate = new System.DateTime(tongsogiay);
                            int month = ((newDate.Year - 1) * 12 + newDate.Month - 1);
                            if (month == 0)
                            {
                                //Nếu Bn trên 24 giờ và dưới 1 tháng tuổi => hiển thị "xyz ngày tuổi"
                                tuoi = ((int)diff__month.TotalDays + " " + caption__NgayTuoi);
                            }
                            else
                            {
                                //- Dưới 72 tháng tuổi: tính chính xác đến tháng như hiện tại
                                if (month < 72)
                                {
                                    tuoi = (month + " " + caption__ThangTuoi);
                                }
                                //- Trên 72 tháng tuổi: tính chính xác đến năm: tuổi= năm hiện tại - năm sinh
                                else
                                {
                                    int year = System.DateTime.Now.Year - dtNgSinh.Year;
                                    tuoi = (year + " " + caption__Tuoi);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                tuoi = "";
            }
            return tuoi;
        }
        #endregion
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

        #region PACS SANCY
        //private HL7PACS CreateHL7PACS(ADO.ServiceADO data)
        //{
        //    try
        //    {
        //        HL7PACS hl7PACS = new HL7PACS();
        //        hl7PACS.tenPhanMemHIS = System.Configuration.ConfigurationManager.AppSettings["Inventec.Desktop.ApplicationCode"] + "_INVENTEC";
        //        hl7PACS.tenPhanMemPACS = "SANCY";
        //        hl7PACS.idChiDinh = data.ID;

        //        hl7PACS.hoTenBenhNhan = TreatmentWithPatientTypeAlter.TDL_PATIENT_NAME;

        //        int namSinh = 0;
        //        int.TryParse(TreatmentWithPatientTypeAlter.TDL_PATIENT_DOB.ToString().Substring(0, 4), out namSinh);
        //        DateTime dtNow = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.TDL_INTRUCTION_TIME) ?? DateTime.Now;
        //        int tuoi = dtNow.Year - namSinh;
        //        if (tuoi < 6) hl7PACS.hoTenBenhNhan += " " + CalculatorAge(TreatmentWithPatientTypeAlter.TDL_PATIENT_DOB, true);

        //        hl7PACS.gioiTinh = TreatmentWithPatientTypeAlter.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE ? MedilinkHL7.Define.AdministrativeSex.F : MedilinkHL7.Define.AdministrativeSex.M;
        //        hl7PACS.idBenhNhan = TreatmentWithPatientTypeAlter.TDL_PATIENT_CODE;
        //        hl7PACS.idDotVaoVien = Convert.ToDecimal(TreatmentWithPatientTypeAlter.TREATMENT_CODE);
        //        hl7PACS.diaChiBenhNhan = TreatmentWithPatientTypeAlter.TDL_PATIENT_ADDRESS;
        //        hl7PACS.ngaySinh = TreatmentWithPatientTypeAlter.TDL_PATIENT_DOB.ToString().Substring(0, 8);
        //        hl7PACS.soVaoVien_STTKham = !String.IsNullOrWhiteSpace(TreatmentWithPatientTypeAlter.IN_CODE) ? TreatmentWithPatientTypeAlter.IN_CODE : (currentServiceReq.NUM_ORDER.HasValue ? currentServiceReq.NUM_ORDER.ToString() : "1");
        //        hl7PACS.ngayVaoVien = TreatmentWithPatientTypeAlter.CLINICAL_IN_TIME.HasValue ? TreatmentWithPatientTypeAlter.CLINICAL_IN_TIME.ToString().Substring(0, 12) : TreatmentWithPatientTypeAlter.IN_TIME.ToString().Substring(0, 12);
        //        hl7PACS.maChanDoan = TreatmentWithPatientTypeAlter.ICD_CODE;
        //        hl7PACS.chanDoan = TreatmentWithPatientTypeAlter.ICD_NAME;
        //        if (!String.IsNullOrWhiteSpace(TreatmentWithPatientTypeAlter.HEIN_CARD_NUMBER))
        //        {
        //            hl7PACS.soTheBHYT = TreatmentWithPatientTypeAlter.HEIN_CARD_NUMBER;
        //            hl7PACS.ngayBatDauCoGiaTriCuaThe = TreatmentWithPatientTypeAlter.HEIN_CARD_FROM_TIME.ToString().Substring(0, 8);
        //            hl7PACS.ngayHetHanCuaThe = TreatmentWithPatientTypeAlter.HEIN_CARD_TO_TIME.ToString().Substring(0, 8);
        //            hl7PACS.maNoiDKKCBBHYT = TreatmentWithPatientTypeAlter.TDL_HEIN_MEDI_ORG_CODE;
        //            hl7PACS.tenNoiDKKCBBHYT = TreatmentWithPatientTypeAlter.TDL_HEIN_MEDI_ORG_NAME;
        //        }
        //        hl7PACS.noiLamViec = TreatmentWithPatientTypeAlter.TDL_PATIENT_WORK_PLACE;

        //        hl7PACS.doiTuongBenhNhan = TreatmentWithPatientTypeAlter.PATIENT_TYPE_CODE;

        //        LoaiBenhNhan loaiBN = TreatmentWithPatientTypeAlter.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU ? LoaiBenhNhan.I : LoaiBenhNhan.O;
        //        hl7PACS.loaiBenhNhan = loaiBN;

        //        hl7PACS.hoTenBacSiChiDinh = data.TDL_REQUEST_USERNAME;
        //        hl7PACS.maVienPhiChiDinh = data.TDL_SERVICE_CODE;
        //        hl7PACS.tenVienPhiChiDinh = data.TDL_SERVICE_NAME;
        //        hl7PACS.sttLayMau_ChiDinh = currentServiceReq.NUM_ORDER.HasValue ? currentServiceReq.NUM_ORDER.ToString() : "1";
        //        var room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == currentServiceReq.REQUEST_DEPARTMENT_ID);
        //        hl7PACS.tenKhoaPhong = room != null ? room.DEPARTMENT_NAME : "";
        //        hl7PACS.phong = "";
        //        hl7PACS.giuong = "";

        //        DoUuTien s_UuTien = DoUuTien.R;
        //        hl7PACS.doUuTien = s_UuTien;
        //        PhuongThucDiChuyen _transportMode = PhuongThucDiChuyen.WALK;
        //        hl7PACS.phuongThucDiChuyen = _transportMode;
        //        return hl7PACS;
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //    return null;
        //}

        private void repositoryItemButtonSendSancy_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                //var row = (ADO.ServiceADO)gridViewSereServ.GetFocusedRow();
                //if (row == null || !row.MACHINE_ID.HasValue)
                //{
                //    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.KhongXacDinhDuocMay);
                //    return;
                //}

                //var machine = this.ListMachine.FirstOrDefault(o => o.ID == row.MACHINE_ID);
                //if (String.IsNullOrWhiteSpace(machine.INTEGRATE_ADDRESS))
                //{
                //    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ChuaCauHinhDiaChiPACS);
                //    return;
                //}
                //string host = "";
                //int port = 8080;
                //var address = machine.INTEGRATE_ADDRESS.Trim().Split(':');
                //if (address.Count() > 0)
                //{
                //    host = address[0];
                //    if (address.Count() > 1 && !String.IsNullOrWhiteSpace(address[1]))
                //        port = Convert.ToInt32(address[1]);
                //}

                //CommonParam param = new CommonParam();
                //bool success = false;

                //HL7PACS hl7PACS = this.CreateHL7PACS(row);
                //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hl7PACS), hl7PACS));
                //Output output = SendSANCY.SendOrderToSancy(hl7PACS, host, port);

                //if (output != null)
                //{
                //    if (output.Status == Status.ThanhCong)
                //    {
                //        success = true;
                //    }
                //    else
                //    {
                //        param.Messages = new List<string>();
                //        if (!String.IsNullOrWhiteSpace(output.Error))
                //            param.Messages.Add(output.Error);
                //        if (!string.IsNullOrWhiteSpace(output.Message))
                //            param.Messages.Add(output.Message);
                //    }
                //}
                //else
                //{
                //    param.Messages = new List<string>();
                //    param.Messages.Add("Không gửi được thông tin sang server PACS");
                //}

                //#region Show message
                //Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                //#endregion
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
                if (btnSave.Enabled)
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
                if (btnFinish.Enabled)
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
                if (!btnPrint.Enabled || sereServ.isNoPay) return;
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
                if (btnAssignService.Enabled)
                    btnAssignService_Click(null, null);
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
                if (btnAssignPrescription.Enabled)
                    btnAssignPrescription_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void UCServiceExecute_Leave(object sender, EventArgs e)
        {

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
                        listArgs.Add(currentServiceReq.TREATMENT_ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
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

    }
}

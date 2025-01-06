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
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisAssignBlood.ADO;
using HIS.Desktop.Plugins.HisAssignBlood.Config;
using HIS.Desktop.Plugins.HisAssignBlood.Resources;
using HIS.Desktop.Plugins.HisAssignBlood.Sda.SdaEventLogCreate;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using HIS.UC.DateEditor;
using HIS.UC.Icd;
using HIS.UC.SecondaryIcd;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
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
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisAssignBlood
{
    public partial class frmHisAssignBlood : HIS.Desktop.Utility.FormBase
    {
        #region Reclare variable
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        const string commonString__true = "1";

        long? serviceReqParentId;//TODO
        int positionHandleBoXung = -1;
        int positionHandleControl = -1;
        int focusedRowHandle = -1;
        int actionType = 0;
        int actionBosung = 0;
        decimal totalPriceFinance = 0;
        double idRow = -1;

        long serviceReqId = 0;
        long treatmentId = 0;
        long intructionTime = 0;
        V_HIS_SERE_SERV currentSereServ { get; set; }
        HIS.Desktop.ADO.AssignBloodADO.DelegateProcessDataResult processDataResult;
        HIS.Desktop.ADO.AssignBloodADO.DelegateProcessRefeshIcd processRefeshIcd;
        Inventec.Desktop.Common.Modules.Module currentModule;
        string patientName;
        long patientDob;
        string genderName;

        List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> currentMestRoomByRooms;
        HisTreatmentWithPatientTypeInfoSDO CurrentHisTreatment { get; set; }
        MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ CurrentHisServiceReq { get; set; }
        MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER currentHisPatientTypeAlter = null;
        List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> currentPatientTypeWithPatientTypeAlter = null;
        MOS.EFMODEL.DataModels.V_HIS_PATIENT currentPatient;
        List<BloodTypeADO> ListBloodTypeADOProcess { get; set; }
        BloodTypeADO currentBloodTypeADOForEdit = new BloodTypeADO();
        BloodTypeADO currentBloodTypeADO { get; set; }
        BloodADO currentBloodType { get; set; }

        PatientBloodPresSDO HisPrescriptionSDO { get; set; }
        PatientBloodPresResultSDO currentHisPrescriptionSDOPrint { get; set; }
        List<PatientBloodPresResultSDO> HisPrescriptionSDOPrint { get; set; }
        List<BloodADO> bloodGroups = new List<BloodADO>();
        List<HIS_BLOOD_TYPE> lstBloodGroups = new List<HIS_BLOOD_TYPE>();
        bool isSaveAndPrint = false;
        Dictionary<long, List<V_HIS_SERVICE_PATY>> servicePatyInBranchs;
        List<TrackingAdo> listTracking_ForCboTracking = null;

        internal bool isMultiDateState = false;
        internal List<long> intructionTimeSelecteds = new List<long>();
        internal IcdProcessor icdProcessor;
        internal UserControl ucIcd;
        internal SecondaryIcdProcessor subIcdProcessor;
        internal UserControl ucSecondaryIcd;
        internal UCDateProcessor ucDateProcessor;
        internal UserControl ucDate;

        bool isInitForm = true;

        HIS_SERVICE_REQ _ServiceReqEdit = null;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.HisAssignBlood";
        MOS.EFMODEL.DataModels.HIS_TRACKING tracking { get; set; }
        bool isReturn;
		private List<V_HIS_EXECUTE_ROOM> allDataExecuteRooms;
		private V_HIS_ROOM requestRoom;
		private List<HIS_SERVICE> lstSerivce;
		private HIS_DEPARTMENT currentDepartment;
		private List<HIS_SERE_SERV> sereServsInTreatmentRaw;
		private List<HIS_SERE_SERV> sereServWithTreatment;
        private V_HIS_TREATMENT currentTreatment;
        PrintPopupMenuProcessor PrintPopupMenuProcessor;

        private List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> GroupStreamPrint;
        private long TotalPrint = 0;
        private long total108 = 0;
        private long expMestToPrint = 0;
        private string currentTypeCode108 = "";
        private string currentFileName108 = "";
        private bool CancelPrint;
        private long CountSereServPrinted = 0;
        V_HIS_SERVICE_REQ hisServiceReqPrint = new V_HIS_SERVICE_REQ();
        HIS_SERVICE_REQ _ServiceReqPrint = new HIS_SERVICE_REQ();
        private const int TIME_OUT_PRINT_MERGE = 1200;
        private bool IsMultilData = false;
        Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
        PatientBloodPresResultSDO HisPrescriptionSDOResultPrint { get; set; }
        #endregion

        #region Construct
        public frmHisAssignBlood(Inventec.Desktop.Common.Modules.Module moduleData, AssignBloodADO assignBloodADO)
            : base(moduleData)
        {
            try
            {
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

                this.actionType = GlobalVariables.ActionAdd;
                this.actionBosung = GlobalVariables.ActionAdd;
                this.treatmentId = assignBloodADO.TreatmentId;
                this.intructionTime = assignBloodADO.IntructionTime;
                this.currentSereServ = assignBloodADO.SereServ;
                this.processDataResult = assignBloodADO.DgProcessDataResult;
                this.processRefeshIcd = assignBloodADO.DgProcessRefeshIcd;
                this.currentModule = moduleData;
                this.patientName = assignBloodADO.PatientName;
                this.patientDob = assignBloodADO.PatientDob;
                this.genderName = assignBloodADO.GenderName;
                this.tracking = assignBloodADO.Tracking;
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisAssignBlood.Resources.Lang", typeof(HIS.Desktop.Plugins.HisAssignBlood.frmHisAssignBlood).Assembly);
                HisConfigCFG.LoadConfig();

                //LogSystem.Info("Construct => 1");
                List<Action> methods = new List<Action>();
                methods.Add(this.InitUcIcd);
                methods.Add(this.InitUcSecondaryIcd);
                methods.Add(this.InitUcDate);
                ThreadCustomManager.MultipleThreadWithJoin(methods);
                //LogSystem.Info("Construct => 2");
                //this.InitUcIcd();
                //this.InitUcSecondaryIcd();
                //this.InitUcDate();

                this.isInitForm = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmHisAssignBlood(Inventec.Desktop.Common.Modules.Module moduleData, AssignBloodADO assignBloodADO, HIS_SERVICE_REQ serviceReqEdit)
            : base(moduleData)
        {
            try
            {
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

                this.actionType = GlobalVariables.ActionAdd;
                this.actionBosung = GlobalVariables.ActionAdd;
                this.treatmentId = assignBloodADO.TreatmentId;
                this.intructionTime = assignBloodADO.IntructionTime;
                this.currentSereServ = assignBloodADO.SereServ;
                this.processDataResult = assignBloodADO.DgProcessDataResult;
                this.processRefeshIcd = assignBloodADO.DgProcessRefeshIcd;
                this.currentModule = moduleData;
                this.patientName = assignBloodADO.PatientName;
                this.patientDob = assignBloodADO.PatientDob;
                this.genderName = assignBloodADO.GenderName;
                this._ServiceReqEdit = serviceReqEdit;
                this.tracking = assignBloodADO.Tracking;
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisAssignBlood.Resources.Lang", typeof(HIS.Desktop.Plugins.HisAssignBlood.frmHisAssignBlood).Assembly);
                HisConfigCFG.LoadConfig();

                //LogSystem.Info("Construct => 1");
                List<Action> methods = new List<Action>();
                methods.Add(this.InitUcIcd);
                methods.Add(this.InitUcSecondaryIcd);
                methods.Add(this.InitUcDate);
                ThreadCustomManager.MultipleThreadWithJoin(methods);
                //LogSystem.Info("Construct => 2");

                this.isInitForm = true;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadServiceReqOld(long _serviceReqId)
        {
            try
            {
                this.ListBloodTypeADOProcess = new List<BloodTypeADO>();
                this.HisPrescriptionSDOPrint = new List<PatientBloodPresResultSDO>();
                PatientBloodPresResultSDO sdoEdit = new PatientBloodPresResultSDO();
                MOS.Filter.HisServiceReqFilter _serviceReqfilter = new MOS.Filter.HisServiceReqFilter();
                _serviceReqfilter.ID = _serviceReqId;
                var dataServiceReqs = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("/api/HisServiceReq/Get", ApiConsumers.MosConsumer, _serviceReqfilter, null);
                if (dataServiceReqs != null && dataServiceReqs.Count > 0)
                {
                    sdoEdit.ServiceReq = dataServiceReqs.First();
                    UC.DateEditor.ADO.DateInputADO dateInputADO = new UC.DateEditor.ADO.DateInputADO();
                    dateInputADO.Time = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dataServiceReqs.First().INTRUCTION_TIME) ?? DateTime.Now;
                    dateInputADO.Dates = new List<DateTime?>();
                    dateInputADO.Dates.Add(dateInputADO.Time);
                    ucDateProcessor.Reload(ucDate, dateInputADO);


                    HIS.UC.Icd.ADO.IcdInputADO icd = new HIS.UC.Icd.ADO.IcdInputADO();
                    icd.ICD_CODE = dataServiceReqs.First().ICD_CODE;
                    icd.ICD_NAME = dataServiceReqs.First().ICD_NAME;
                    if (ucIcd != null)
                    {
                        icdProcessor.Reload(ucIcd, icd);
                    }

                    HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO subIcd = new HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO();
                    subIcd.ICD_SUB_CODE = dataServiceReqs.First().ICD_SUB_CODE;
                    subIcd.ICD_TEXT = dataServiceReqs.First().ICD_TEXT;
                    if (ucSecondaryIcd != null)
                    {
                        subIcdProcessor.Reload(ucSecondaryIcd, subIcd);
                    }
                }

                MOS.Filter.HisExpMestFilter filter = new MOS.Filter.HisExpMestFilter();
                filter.SERVICE_REQ_ID = _serviceReqId;
                var dataExpMests = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("/api/HisExpMest/Get", ApiConsumers.MosConsumer, filter, null);
                if (dataExpMests != null && dataExpMests.Count > 0)
                {
                    this.cboMediStockExport_TabBlood.EditValue = dataExpMests.First().MEDI_STOCK_ID;
                    this.cboMediStockExport_TabBlood.Enabled = false;                   
                    this.btnNew.Enabled = false;
                    this.dropDownPrintBlood.Enabled = true;

                    sdoEdit.ExpMest = dataExpMests.First();
                    sdoEdit.Bloods = new List<HIS_EXP_MEST_BLTY_REQ>();

                    MOS.Filter.HisExpMestBltyReqViewFilter _expMestBltyFilter = new MOS.Filter.HisExpMestBltyReqViewFilter();
                    _expMestBltyFilter.EXP_MEST_IDs = dataExpMests.Select(p => p.ID).ToList();
                    var dataExpMestBltyReqs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_BLTY_REQ>>("/api/HisExpMestBltyReq/GetView", ApiConsumers.MosConsumer, _expMestBltyFilter, null);

                    if (dataExpMestBltyReqs != null && dataExpMestBltyReqs.Count > 0)
                    {
                        MOS.Filter.HisBloodTypeFilter bloodTypeFilter = new MOS.Filter.HisBloodTypeFilter();
                        bloodTypeFilter.IDs = dataExpMestBltyReqs.Select(p => p.BLOOD_TYPE_ID).ToList();
                        var dataBloodType = new BackendAdapter(new CommonParam()).Get<List<HIS_BLOOD_TYPE>>("/api/HisBloodType/Get", ApiConsumers.MosConsumer, bloodTypeFilter, null);
                       
                        foreach (var item in dataExpMestBltyReqs)
                        {
                            HIS_EXP_MEST_BLTY_REQ bltyPrint = new HIS_EXP_MEST_BLTY_REQ();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST_BLTY_REQ>(bltyPrint, item);
                            sdoEdit.Bloods.Add(bltyPrint);

                            BloodTypeADO ado = new BloodTypeADO();
                            ado.ID = item.BLOOD_TYPE_ID;
                            ado.AMOUNT = item.AMOUNT;
                            ado.BLOOD_TYPE_NAME = item.BLOOD_TYPE_NAME;
                            ado.BLOOD_TYPE_CODE = item.BLOOD_TYPE_CODE;
                            ado.BLOOD_ABO_ID = item.BLOOD_ABO_ID;
                            ado.BLOOD_RH_ID = item.BLOOD_RH_ID;
                            ado.PATIENT_TYPE_ID = item.PATIENT_TYPE_ID;
                            ado.IS_OUT_PARENT_FEE = item.IS_OUT_PARENT_FEE;
                            if (dataBloodType.First(o => o.ID == item.BLOOD_TYPE_ID) != null)
                            {
                                var currentService = lstSerivce.First(o => o.ID == dataBloodType.First(p => p.ID == item.BLOOD_TYPE_ID).SERVICE_ID);
                                if (currentService != null)
                                {
                                    ado.SERVICE_TYPE_ID = currentService.SERVICE_TYPE_ID;
                                    ado.SERVICE_ID = currentService.ID;
                                    ado.HEIN_LIMIT_PRICE_IN_TIME = currentService.HEIN_LIMIT_PRICE_IN_TIME;
                                    ado.HEIN_LIMIT_PRICE_INTR_TIME = currentService.HEIN_LIMIT_PRICE_INTR_TIME;
                                    ado.HEIN_LIMIT_PRICE_OLD = currentService.HEIN_LIMIT_PRICE_OLD;
                                    ado.HEIN_LIMIT_PRICE = currentService.HEIN_LIMIT_PRICE;
                                }
                            }

                            ado.PRICE = GetPriceByBloodType(ado);
                            ado.TOT_PRICE = ado.PRICE * ado.AMOUNT;
                            this.ListBloodTypeADOProcess.Add(ado);
                        }
                    }
                }
                this.HisPrescriptionSDOPrint.Add(sdoEdit);
                this.gridControlServiceProcess__TabBlood.DataSource = null;
                this.gridControlServiceProcess__TabBlood.DataSource = this.ListBloodTypeADOProcess;
                this.EnableAndDisableControlWithGirdcontrol();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcDate()
        {
            try
            {
                ucDateProcessor = new UCDateProcessor();
                HIS.UC.DateEditor.ADO.DateInitADO ado = new HIS.UC.DateEditor.ADO.DateInitADO();
                ado.DelegateNextFocus = NextForcusUCDate;
                ado.DelegateChangeIntructionTime = ChangeIntructionTime;
                ado.DelegateSelectMultiDate = DelegateSelectMultiDate;
                ado.Height = 24;
                ado.Width = 364;//284//TODO
                ado.IsVisibleMultiDate = false;
                ado.IsValidate = true;
                ado.LanguageInputADO = new UC.DateEditor.ADO.LanguageInputADO();
                ado.LanguageInputADO.TruongDuLieuBatBuoc = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                ado.LanguageInputADO.UCDate__CaptionlciDateEditor = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.lciTimeAssign.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.UCDate__CaptionchkMultiIntructionTime = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.chkMultiIntructionTime.Text", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.ChuaChonNgayChiDinh = ResourceMessage.ChuaChonNgayChiDinh;
                ado.LanguageInputADO.FormMultiChooseDate__CaptionCalendaInput = Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionCalendaInput", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.FormMultiChooseDate__CaptionText = Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionText", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.FormMultiChooseDate__CaptionTimeInput = Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionTimeInput", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                ado.LanguageInputADO.FormMultiChooseDate__CaptionBtnChoose = Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionBtnChoose", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                ucDate = (UserControl)ucDateProcessor.Run(ado);

                if (ucDate != null)
                {
                    ucDate.Enabled = HisConfigCFG.IsUsingServerTime != commonString__true;
                    this.pnlUCDate.Controls.Add(ucDate);
                    ucDate.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void NextForcusUCDate()
        {
            try
            {
                SendKeys.Send("{TAB}");
                //this.FocusShowpopup(this.cboServiceGroup, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcIcd()
        {
            try
            {
                icdProcessor = new HIS.UC.Icd.IcdProcessor();
                HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
                ado.DelegateNextFocus = NextForcusSubIcd;
                ado.Width = 660;
                ado.Height = 24;
                ado.IsColor = (HisConfigCFG.ObligateIcd == commonString__true);
                ado.DataIcds = BackendDataWorker.Get<HIS_ICD>().OrderBy(o => o.ICD_CODE).ToList();
                ado.AutoCheckIcd = HisConfigCFG.AutoCheckIcd == commonString__true;
                ucIcd = (UserControl)icdProcessor.Run(ado);

                if (ucIcd != null)
                {
                    this.panelControlIcd.Controls.Add(ucIcd);
                    ucIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void NextForcusSubIcd()
        {
            try
            {
                if (ucSecondaryIcd != null)
                {
                    subIcdProcessor.FocusControl(ucSecondaryIcd);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcSecondaryIcd()
        {
            try
            {
                subIcdProcessor = new SecondaryIcdProcessor(new CommonParam(), BackendDataWorker.Get<HIS_ICD>().OrderBy(o => o.ICD_CODE).ToList());
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.DelegateNextFocus = NextForcusOut;
                ado.Width = 660;
                ado.Height = 24;
                ado.TextLblIcd = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.lciIcdText_TabBlood.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                ado.TextNullValue = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.txtIcdExtraName_TabBlood.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                ucSecondaryIcd = (UserControl)subIcdProcessor.Run(ado);

                if (ucSecondaryIcd != null)
                {
                    this.panelControlSubIcd.Controls.Add(ucSecondaryIcd);
                    ucSecondaryIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void NextForcusOut()
        {
            try
            {
                icdProcessor.FocusControl(ucDate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Private method

        #region Load data
        /// <summary>
        /// Set lai tat cac cac label/caption/tooltip hien thi tren giao dien doc tu file resource ngon ngu
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                if (this.currentModule != null && !String.IsNullOrEmpty(this.currentModule.text))
                {
                    this.Text = this.currentModule.text;
                }

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBloodABO.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.cboBloodABO.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBloodRH.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.cboBloodRH.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAddBlood.Text = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.btnAddBlood.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewBloodType__BloodPage.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.gridViewBloodType__BloodPage.OptionsFind.FindNullPrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcAvailableAmount_TabBlood.Caption = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.grcAvailableAmount_TabBlood.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcBloodTypeName_TabBlood.Caption = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.grcBloodTypeName_TabBlood.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcServiceUnitNameName_TabBlood.Caption = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.grcServiceUnitNameName_TabBlood.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcTotalAmount_TabBlood.Caption = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.grcTotalAmount_TabBlood.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcBloodTypeCode_TabBlood.Caption = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.grcBloodTypeCode_TabBlood.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn41.Caption = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.gridColumn41.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcExpend.Caption = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.grcExpend.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExecuteGroup_TabBlood.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.cboExecuteGroup_TabBlood.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.chkIcdBlood.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.chkIcdBlood.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.cboIcdBlood.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.cboIcdBlood.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMediStockExport_TabBlood.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.cboMediStockExport_TabBlood.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dropDownPrintBlood.Text = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.dropDownPrintBlood.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSaveAndPrint.Text = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.btnSaveAndPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl8.Text = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.layoutControl8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcDelete__BloodPage.Caption = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.grcDelete__BloodPage.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcDelete__BloodPage.ToolTip = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.grcDelete__BloodPage.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcManuBloodCode__TabBlood.Caption = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.grcManuBloodCode__TabBlood.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcNumOrder.Caption = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.grcNumOrder.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcManuBloodName__TabBlood.Caption = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.grcManuBloodName__TabBlood.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcPatientType__TabBlood.Caption = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.grcPatientType__TabBlood.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcAmount__TabBlood.Caption = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.grcAmount__TabBlood.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcPrice__TabBlood.Caption = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.grcPrice__TabBlood.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcTotalPrice__TabBlood.Caption = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.grcTotalPrice__TabBlood.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcBloodABO__TabBlood.Caption = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.grcBloodABO__TabBlood.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemcboBloodABO.NullText = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.repositoryItemcboBloodABO.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcBloodRH__TabBlood.Caption = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.grcBloodRH__TabBlood.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemcboBloodRH.NullText = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.repositoryItemcboBloodRH.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcIsOutKtcFee__TabBlood.Caption = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.grcIsOutKtcFee__TabBlood.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcUnit__TabBlood.Caption = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.grcUnit__TabBlood.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcConcentra__TabBlood.Caption = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.grcConcentra__TabBlood.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcExpend__TabBlood.Caption = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.grcExpend__TabBlood.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcKHBHYT__TabBlood.Caption = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.grcKHBHYT__TabBlood.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcTutorial__TabBlood.Caption = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.grcTutorial__TabBlood.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcUseTimeTo__TabBlood.Caption = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.grcUseTimeTo__TabBlood.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemcboPatientType_TabBlood.NullText = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.repositoryItemcboPatientType_TabBlood.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemcboPatientType_TabBlood_GridLookUp.NullText = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.repositoryItemcboPatientType_TabBlood_GridLookUp.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTotalBloodMaterialPrice.Text = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.lciTotalBloodMaterialPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciWarehouseproduction.Text = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.lciWarehouseproduction.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem44.Text = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.layoutControlItem44.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientTypeName.Text = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.lciPatientTypeName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.lciTracking.Text = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.lciTracking.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnCtrlK.Caption = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.bbtnCtrlA.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnCtrlS.Caption = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.bbtnCtrlS.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.btnTomLuocVienPhi.Text = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.btnTomLuocVienPhi.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("frmHisAssignBlood.btnNew.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async void frmHisAssignBlood_Load(object sender, EventArgs e)
        {
            try
            {
              
                LogSystem.Debug("_____________" + intructionTime);
                LogSystem.Debug("frmHisAssignBlood_Load => Starting...");
                this.SetCaptionByLanguageKey();
                await this.CheckOverTotalPatientPrice();
                WaitingManager.Show();
                currentTreatment = LoadDataToCurrentTreatmentData(treatmentId);
                LoadDataSereServWithTreatment();
                this.InitControlState();
                this.SetDefaultData();
                this.InitCurrentMesstRoom();
                LogSystem.Debug("frmHisAssignBlood_Load => SetDefaultData...");
                this.FillAllPatientInfoSelectedInForm();
                this.LoadIcdDefault();
                this.InitTabIndex();
                LogSystem.Debug("frmHisAssignBlood_Load => Loaded default data");
                this.FillDataToControlsForm();
                LogSystem.Debug("frmHisAssignBlood_Load => Loaded fillDataToControlsForm");
                this.InitMenuToButtonPrint();
                LogSystem.Debug("frmHisAssignBlood_Load => Loaded InitMenuToButtonPrint");
                this.LoadDefaultUser();
                LogSystem.Debug("frmHisAssignBlood_Load => Loaded LoadIcdDefault, LoadDefaultUser");
                EnableCboTracking();
                this.InitDefaultFocus();

                this.lstSerivce = BackendDataWorker.Get<HIS_SERVICE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                this.allDataExecuteRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                this.requestRoom = GetRequestRoom(this.currentModule.RoomId);
                if (this._ServiceReqEdit != null && this._ServiceReqEdit.ID > 0)
                {
                    this.actionType = GlobalVariables.ActionEdit;
                    this.LoadServiceReqOld(this._ServiceReqEdit.ID);
                }
                this.isInitForm = false;
                WaitingManager.Hide();
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

        private async Task LoadDataSereServWithTreatment()
        {
            try
            {
                if (treatmentId > 0)
                {
                    this.RefeshSereServInTreatmentData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RefeshSereServInTreatmentData()
        {
            try
            {

                CommonParam param = new CommonParam();
                HisSereServFilter hisSereSFilter = new HisSereServFilter();
                hisSereSFilter.TREATMENT_ID = treatmentId;
                //hisSereServFilter.PATIENT_TYPE_ID = HisConfigCFG.PatientTypeId__BHYT;
                this.sereServsInTreatmentRaw = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, hisSereSFilter, param);

                DateTime intructionTime = (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.intructionTimeSelecteds.First()) ?? DateTime.Now);

                List<long> setyAllowsIds = new List<long>();
                setyAllowsIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU);
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
                    HisSereServView1Filter hisSereServFilter = new HisSereServView1Filter();
                    hisSereServFilter.TREATMENT_ID = treatmentId;
                    hisSereServFilter.INTRUCTION_TIME_FROM = INTRUCTION_TIME_FROM;
                    hisSereServFilter.INTRUCTION_TIME_TO = INTRUCTION_TIME_TO;
                    hisSereServFilter.NOT_IN_SERVICE_TYPE_IDs = setyAllowsIds;
                    this.sereServWithTreatment = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>(RequestUriStore.HIS_SERE_SERV_GETVIEW_1, ApiConsumers.MosConsumer, hisSereServFilter, ProcessLostToken, param);
                }
                else
                {
                    
                    this.sereServWithTreatment = this.sereServsInTreatmentRaw.Where(o =>
                        o.TDL_TREATMENT_ID == treatmentId
                        && (INTRUCTION_TIME_FROM == null || (INTRUCTION_TIME_FROM.HasValue && o.TDL_INTRUCTION_TIME >= INTRUCTION_TIME_FROM.Value))
                        && (INTRUCTION_TIME_TO == null || (INTRUCTION_TIME_TO.HasValue && o.TDL_INTRUCTION_TIME <= INTRUCTION_TIME_TO.Value))
                        && !setyAllowsIds.Contains(o.TDL_SERVICE_TYPE_ID)).ToList();
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }
        private void EnableCboTracking()
        {
            try
            {
                LogSystem.Info("(EnableCboTracking) id doi tuong dieu tri hien tai " + this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID);
                if (this.currentHisPatientTypeAlter != null && (this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU || this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU))
                {
                    cboTracking.Enabled = true;
                }
                else
                {
                    cboTracking.Enabled = false;
                    cboTracking.EditValue = null;
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
                if (HisConfigCFG.WarningOverTotalPatientPrice__IsCheck == "1")
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTreatmentFeeViewFilter hisTreatmentFeeViewFilter = new MOS.Filter.HisTreatmentFeeViewFilter();
                    hisTreatmentFeeViewFilter.IS_ACTIVE = 1;
                    hisTreatmentFeeViewFilter.ID = this.treatmentId;
                    Inventec.Common.Logging.LogSystem.Debug("begin call HisTreatment/GetFeeView");
                    var treatmentFees = await new BackendAdapter(param).GetAsync<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumer.ApiConsumers.MosConsumer, hisTreatmentFeeViewFilter, param);

                    if (treatmentFees != null && treatmentFees.Count > 0)
                    {
                        var treatmentFee = treatmentFees.FirstOrDefault();
                        if (treatmentFee.TDL_TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                        {
                            return;
                        }
                        decimal warningOverTotalCGF = Convert.ToInt64(HisConfigCFG.WarningOverTotalPatientPrice);
                        decimal totalReceiveMore = (treatmentFee.TOTAL_PATIENT_PRICE ?? 0) - (treatmentFee.TOTAL_DEPOSIT_AMOUNT ?? 0) - (treatmentFee.TOTAL_BILL_AMOUNT ?? 0) + (treatmentFee.TOTAL_BILL_TRANSFER_AMOUNT ?? 0) + (treatmentFee.TOTAL_REPAY_AMOUNT ?? 0);
                        if (totalReceiveMore > warningOverTotalCGF)
                        {
                            if (MessageBox.Show(String.Format(ResourceMessage.BenhNhanDangThieuVienPhi, Inventec.Common.Number.Convert.NumberToString(totalReceiveMore, ConfigApplications.NumberSeperator)), "Cảnh báo",
        MessageBoxButtons.YesNo, MessageBoxIcon.Question,
        MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.No)
                            {
                                this.Close();
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


        private void LoadDataToTrackingCombo()
        {
            this.listTracking_ForCboTracking = new List<TrackingAdo>();
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTrackingViewFilter trackingFilter = new MOS.Filter.HisTrackingViewFilter();
                //trackingFilter.TRACKING_TIME_FROM = Inventec.Common.DateTime.Get.StartDay();
                //trackingFilter.TRACKING_TIME_TO = Inventec.Common.DateTime.Get.EndDay();
                trackingFilter.TREATMENT_ID = this.treatmentId;
                trackingFilter.DEPARTMENT_ID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetDepartmentId(this.currentModule.RoomTypeId);
                var trackings = new BackendAdapter(param).Get<List<V_HIS_TRACKING>>("api/HisTracking/GetView", ApiConsumer.ApiConsumers.MosConsumer, trackingFilter, param);
                trackings = trackings.OrderByDescending(o => o.TRACKING_TIME).ToList();
                foreach (var tracking in trackings)
                {
                    var trackingAdo = new TrackingAdo(tracking);
                    this.listTracking_ForCboTracking.Add(trackingAdo);
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TrackingTimeStr", "", 150, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TrackingTimeStr", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(this.cboTracking, this.listTracking_ForCboTracking, controlEditorADO);
                // set default 
                if (listTracking_ForCboTracking != null && listTracking_ForCboTracking.Count > 0)
                {
                    SetDefautTrackingCombo(listTracking_ForCboTracking, HisConfigCFG.IsDefaultTracking);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetDefautTrackingCombo(List<TrackingAdo> result, string isDefaultTracking)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("SetDefautTrackingCombo____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tracking), tracking));

                if (result != null && result.Count > 0)
                {
                    if (this.tracking != null)// && !isChangeData
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Ben ngoai truyen vao to dieu tri____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tracking), tracking));

                        cboTracking.EditValue = this.tracking.ID;
                        if(HisConfigCFG.IsServiceReqIcdOption)
                        {
                            HIS.UC.Icd.ADO.IcdInputADO icd = new HIS.UC.Icd.ADO.IcdInputADO();
                            icd.ICD_CODE = tracking.ICD_CODE;
                            icd.ICD_NAME = this.tracking.ICD_NAME;
                            if (ucIcd != null)
                            {
                                icdProcessor.Reload(ucIcd, icd);
                            }

                            HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO subIcd = new HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO();
                            subIcd.ICD_SUB_CODE = this.tracking.ICD_SUB_CODE;
                            subIcd.ICD_TEXT = this.tracking.ICD_TEXT;
                            if (ucSecondaryIcd != null)
                            {
                                subIcdProcessor.Reload(ucSecondaryIcd, subIcd);
                            }
                        }    
                        Inventec.Common.Logging.LogSystem.Info("Truong hop co tracking ben ngoai truyen vao, gan vao doi tuong tracking de phuc vu cho nghiep vu load cac du lieu: + Mã bệnh chỉnh,+ Tên bệnh chính,+ Mã bệnh phụ,+ Tên bệnh phụ,+ Mã bệnh YHCT chính,+ Tên bệnh YHCT chính,+ Mã bệnh YHCT phụ,+ Tên bệnh YHCT phụ tu ban ghi tracking do");
                    }
                    else if (isDefaultTracking == "1")
                    {
                        ReloadDataTrackingCombo(true);
                    }
                }
                else
                {
                    cboTracking.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void ReloadDataTrackingCombo(bool isReload = false)
        {
            try
            {
                if (this.isInitForm == true && !isReload)
                    return;
                List<long> intructionTimeSelecteds = this.ucDateProcessor.GetValue(this.ucDate);
                if (intructionTimeSelecteds == null)
                    return;

                List<string> intructionDateSelecteds = new List<string>();
                foreach (var item in intructionTimeSelecteds)
                {
                    string intructionDate = item.ToString().Substring(0, 8);
                    intructionDateSelecteds.Add(intructionDate);
                }

                if (listTracking_ForCboTracking != null && listTracking_ForCboTracking.Count > 0)
                {
                    var result = this.listTracking_ForCboTracking.Where(o => intructionDateSelecteds.Contains(o.TRACKING_TIME.ToString().Substring(0, 8))).FirstOrDefault();

                    if (result != null)
                    {
                        cboTracking.EditValue = result.ID;
                    }
                    else
                    {
                        cboTracking.EditValue = null;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void InitDefaultFocus()
        {
            try
            {
                if (!this.ValidPatientTypeForAdd() || HisConfigCFG.AutoCheckIcd != commonString__true)
                    ucDateProcessor.FocusControl(ucDate);
                //else
                //    icdProcessor.FocusControl(ucIcd);
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

        private void DelegateSelectMultiDate(List<DateTime?> datas, DateTime time)
        {
            try
            {
                this.intructionTimeSelecteds = this.ucDateProcessor.GetValue(this.ucDate).OrderByDescending(o => o).ToList();
                this.isMultiDateState = this.ucDateProcessor.GetChkMultiDateState(this.ucDate);

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
                this.intructionTimeSelecteds = this.ucDateProcessor.GetValue(ucDate);
                //LogSystem.Info("intructionTimeSelecteds --------------" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => intructionTimeSelecteds), intructionTimeSelecteds));
                this.isMultiDateState = this.ucDateProcessor.GetChkMultiDateState(ucDate);
                this.ReloadDataTrackingCombo();
                this.CurrentHisTreatment = this.LoadDataToCurrentTreatmentData(treatmentId, this.intructionTimeSelecteds.FirstOrDefault());
                //LogSystem.Info("ChangeIntructionTime => Loaded LoadDataToCurrentTreatmentData info (Truy van thong tin can cu theo treatment_id truyen vao lay thong tin ho so dieu trị, chan doan chinh phu, doi tuong dieu tri hien tai, thong tin tuyen & dang ky kcb ban dau neu doi tuong dieu tri hien tai la bhyt ==> su dung HisTreatmentWithPatientTypeInfoSDO)");
                this.ProcessDataWithTreatmentWithPatientTypeInfo();
                //LogSystem.Info("ChangeIntructionTime => Loaded PatientType With ProcessDataWithTreatmentWithPatientTypeInfo info");
                this.LoadServicePaty();
                LogSystem.Debug("ChangeIntructionTime => LoadServicePaty...");
                this.InitComboRepositoryPatientType(this.currentPatientTypeWithPatientTypeAlter);
                this.LoadTreatmentInfo__PatientType();
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
                var patientTypeAllows = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW>();
                var patientTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>();
                if (patientTypeAllows != null && patientTypes != null)
                {
                    if (this.CurrentHisTreatment != null && this.CurrentHisTreatment.ID > 0)
                    {
                        var patientType = patientTypes.FirstOrDefault(o => o.PATIENT_TYPE_CODE == this.CurrentHisTreatment.PATIENT_TYPE_CODE);
                        if (patientType == null) throw new AggregateException("Khong lay duoc thong tin PatientType theo ma doi tuong (PATIENT_TYPE trong HisTreatmentWithPatientTypeInfoSDO).");

                        this.currentHisPatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                        this.currentHisPatientTypeAlter.PATIENT_TYPE_ID = patientType.ID;
                        this.currentHisPatientTypeAlter.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                        this.currentHisPatientTypeAlter.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;

                        var tt = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.TREATMENT_TYPE_CODE == this.CurrentHisTreatment.TREATMENT_TYPE_CODE);
                        this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID = (tt != null ? tt.ID : 0);
                        LogSystem.Info("(ProcessDataWithTreatmentWithPatientTypeInfo) currentHisPatientTypeAlter doi tuong dieu tri hien tai" + this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID);
                        LogSystem.Info("(ProcessDataWithTreatmentWithPatientTypeInfo) CurrentHisTreatment doi tuong dieu tri hien tai" + CurrentHisTreatment.TREATMENT_TYPE_CODE);
                        this.currentHisPatientTypeAlter.TREATMENT_TYPE_CODE = this.CurrentHisTreatment.TREATMENT_TYPE_CODE;
                        this.currentHisPatientTypeAlter.HEIN_MEDI_ORG_CODE = this.CurrentHisTreatment.HEIN_MEDI_ORG_CODE;
                        this.currentHisPatientTypeAlter.HEIN_CARD_FROM_TIME = this.CurrentHisTreatment.HEIN_CARD_FROM_TIME;
                        this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME = this.CurrentHisTreatment.HEIN_CARD_TO_TIME;
                        this.currentHisPatientTypeAlter.HEIN_CARD_NUMBER = this.CurrentHisTreatment.HEIN_CARD_NUMBER;
                        this.currentHisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE = this.CurrentHisTreatment.RIGHT_ROUTE_TYPE_CODE;
                        this.currentHisPatientTypeAlter.LEVEL_CODE = this.CurrentHisTreatment.LEVEL_CODE;
                        this.currentHisPatientTypeAlter.RIGHT_ROUTE_CODE = this.CurrentHisTreatment.RIGHT_ROUTE_CODE;

                        var patientTypeAllow = patientTypeAllows.Where(o => o.PATIENT_TYPE_ID == patientType.ID).Select(m => m.PATIENT_TYPE_ALLOW_ID).Distinct().ToList();
                        patientTypeAllow.Add(currentHisPatientTypeAlter.PATIENT_TYPE_ID);
                        this.currentPatientTypeWithPatientTypeAlter = ((patientTypeAllow != null && patientTypeAllow.Count > 0) ? patientTypes.Where(o => patientTypeAllow.Contains(o.ID)).ToList() : new List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>());
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
                MessageManager.Show(ResourceMessage.KhongTimThayDoiTuongThanhToanTrongThoiGianYLenh);
                Inventec.Common.Logging.LogSystem.Info("LoadDataToCurrentTreatmentData => khong lay duoc doi tuong benh nhan. Dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => treatmentId), treatmentId) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => intructionTimeSelecteds), intructionTimeSelecteds) + "____Dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => CurrentHisTreatment), CurrentHisTreatment));
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                DateTime itime = (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.intructionTimeSelecteds.First()) ?? DateTime.Now);
                ChangeIntructionTime(itime);
                this.currentPatient = this.LoadDataToCurrentPatientData(this.CurrentHisTreatment != null ? this.CurrentHisTreatment.PATIENT_ID : 0);
                //LogSystem.Info("ChangeIntructionTime => Loaded LoadDataToCurrentTreatmentData info (Truy van thong tin can cu theo treatment_id truyen vao lay thong tin ho so dieu trị, chan doan chinh phu, doi tuong dieu tri hien tai, thong tin tuyen & dang ky kcb ban dau neu doi tuong dieu tri hien tai la bhyt ==> su dung HisTreatmentWithPatientTypeInfoSDO)");
                //this.LoadDataSereServWithTreatment(this.CurrentHisTreatment, itime);
                //LogSystem.Info("ChangeIntructionTime => Loaded LoadDataSereServWithTreatment (Truy vấn danh sách các loại thuốc đã kê trong ngày, lấy từ view v_his_sere_serv_8)");
                //this.CreateThreadLoadDataSereServWithTreatment(this.CurrentHisTreatment);
                //LogSystem.Debug("FillAllPatientInfoSelectedInForm => Loaded CreateThreadLoadDataSereServWithTreatment (Truy vấn danh sách các loại thuốc đã kê trong ngày, lấy từ view v_his_sere_serv_8)");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadTreatmentInfo__PatientType()
        {
            try
            {
                //this.lblPatientName.Text = this.CurrentHisTreatment.TDL_PATIENT_NAME;
                //if (this.patientDob > 0)
                //    lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.CurrentHisTreatment.TDL_PATIENT_DOB);
                //this.lblGenderName.Text = this.CurrentHisTreatment.TDL_PATIENT_GENDER_NAME;

                if (this.currentHisPatientTypeAlter != null)
                {
                    this.lblPatientTypeName.Text = this.currentHisPatientTypeAlter.PATIENT_TYPE_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitBloodADO__RH__FromPatientInfo()
        {
            try
            {
                //this.cboBloodABO.EditValue = (this.currentPatient != null ? this.currentPatient.BLOOD_ABO_ID : null);
                //this.cboBloodABO.Properties.Buttons[1].Visible = (this.cboBloodABO.EditValue != null ? true : false);

                //this.cboBloodRH.EditValue = (this.currentPatient != null ? this.currentPatient.BLOOD_RH_ID : null);
                //this.cboBloodRH.Properties.Buttons[1].Visible = (this.cboBloodRH.EditValue != null ? true : false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetEnableButtonControlBlood()
        {
            try
            {
                btnAddBlood.Enabled = this.btnSave.Enabled = this.btnSaveAndPrint.Enabled = !(this.actionType == GlobalVariables.ActionView || this.actionType == GlobalVariables.ActionViewForEdit);
                this.dropDownPrintBlood.Enabled = (this.actionType == GlobalVariables.ActionView || this.actionType == GlobalVariables.ActionViewForEdit);
                this.gridViewServiceProcess__TabBlood.OptionsBehavior.Editable = !(this.actionType == GlobalVariables.ActionView || this.actionType == GlobalVariables.ActionViewForEdit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private long GetWorkingRoomId()
        {
            long roomId = 0;
            try
            {
                if (this.currentModule != null)
                {
                    roomId = this.currentModule.RoomId;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return roomId;
        }

        /// <summary>
        /// Init set tab index
        /// </summary>
        private void InitTabIndex()
        {
            try
            {
                //dicOrderTabIndexControl.Add("lkAccidentHurtTypeId", 0);
                //dicOrderTabIndexControl.Add("txtContent", 0);
                //dicOrderTabIndexControl.Add("lkTreatmentId", 0);
                //dicOrderTabIndexControl.Add("chkUseAlcohol", 0);
                //dicOrderTabIndexControl.Add("dtAccidentTime", 0);
                //dicOrderTabIndexControl.Add("lkAccidentCareId", 0);
                //dicOrderTabIndexControl.Add("lkAccidentResultId", 0);
                //dicOrderTabIndexControl.Add("lkAccidentHelmetId", 0);
                //dicOrderTabIndexControl.Add("lkAccidentVehicleId", 0);
                //dicOrderTabIndexControl.Add("lkAccidentPoisonId", 0);
                //dicOrderTabIndexControl.Add("lkAccidentLocationId", 0);
                //dicOrderTabIndexControl.Add("lkAccidentBodyPartId", 0);

                //dicOrderTabIndexControl.Add("txtPatientCode", 0);
                //dicOrderTabIndexControl.Add("txtPatientName", 1);

                if (dicOrderTabIndexControl != null && dicOrderTabIndexControl.Count > 0)
                {
                    foreach (KeyValuePair<string, int> itemOrderTab in dicOrderTabIndexControl)
                    {
                        SetTabIndexToControl(itemOrderTab, lcEditorInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool SetTabIndexToControl(KeyValuePair<string, int> itemOrderTab, DevExpress.XtraLayout.LayoutControl layoutControlEditor)
        {
            bool success = false;
            try
            {
                if (!layoutControlEditor.IsInitialized) return success;
                layoutControlEditor.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControlEditor.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null)
                        {
                            BaseEdit be = lci.Control as BaseEdit;
                            if (be != null)
                            {
                                //Cac control dac biet can fix khong co thay doi thuoc tinh enable
                                if (itemOrderTab.Key.Contains(be.Name))
                                {
                                    be.TabIndex = itemOrderTab.Value;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    layoutControlEditor.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return success;
        }

        /// <summary>
        /// Gan gia trị mac dinh cho cac control can khoi tao san gia tri
        /// </summary>
        private void SetDefaultData()
        {
            try
            {
                btnSave.Enabled = btnSaveAndPrint.Enabled = true;
                dropDownPrintBlood.Enabled = false;
                this.lblTotalBloodBloodPrices.Text = "";
                this.actionType = GlobalVariables.ActionAdd;

                //UC.DateEditor.ADO.DateInputADO dateInputADO = new UC.DateEditor.ADO.DateInputADO();
                //ucDateProcessor.Reload(ucDate, dateInputADO);

                DateTime now = DateTime.Now;
                if (this.intructionTime > 0)
                {
                    now = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.intructionTime) ?? DateTime.Now;
                }

                UC.DateEditor.ADO.DateInputADO dateInputADO = new UC.DateEditor.ADO.DateInputADO();

                dateInputADO.Time = now;
                dateInputADO.Dates = new List<DateTime?>();
                dateInputADO.Dates.Add(now);

                this.ucDateProcessor.Reload(this.ucDate, dateInputADO);

                this.intructionTimeSelecteds = ucDateProcessor.GetValue(ucDate);
                this.isMultiDateState = false;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void gridControlBloodType__BloodPage_Click(object sender, EventArgs e)
        {
            try
            {
                this.currentBloodType = (BloodADO)this.gridViewBloodType__BloodPage.GetFocusedRow();
                if (this.currentBloodType != null)
                {
                    this.focusedRowHandle = this.gridViewBloodType__BloodPage.FocusedRowHandle;
                    this.actionBosung = GlobalVariables.ActionAdd;
                    this.spinAmount__BloodPage.Value = 0;
                    this.InitBloodADO__RH__FromPatientInfo();
                    this.spinAmount__BloodPage.SelectAll();
                    this.spinAmount__BloodPage.Focus();                  
                    if(chkShowGroupBlood.Checked)
					{
                        this.cboBloodABO.Enabled = false;
                        this.cboBloodRH.Enabled = false;
                        this.cboBloodABO.EditValue = currentBloodType.BLOOD_ABO_ID;
                        this.cboBloodRH.EditValue = currentBloodType.BLOOD_RH_ID;
                    }
					else
					{
                        this.cboBloodABO.Enabled = true;
                        this.cboBloodRH.Enabled = true;
                        if(!string.IsNullOrEmpty(currentPatient.BLOOD_ABO_CODE))
						{
                            this.cboBloodABO.EditValue = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_ABO>().First(o => o.BLOOD_ABO_CODE == currentPatient.BLOOD_ABO_CODE).ID;
						}
						else
						{
                            this.cboBloodABO.EditValue = null;
						}
                        if (!string.IsNullOrEmpty(currentPatient.BLOOD_RH_CODE))
                        {
                            this.cboBloodRH.EditValue = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_RH>().First(o => o.BLOOD_RH_CODE == currentPatient.BLOOD_RH_CODE).ID;
                        }
                        else
                        {
                            this.cboBloodRH.EditValue = null;
                        }
                    }                        
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlBloodType__BloodPage_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.gridControlBloodType__BloodPage_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewBloodType__BloodPage_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    BloodADO bloodType = (BloodADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (bloodType != null)
                    {
                        if (e.Column.FieldName == "VOLUME_DISPLAY")
                        {
                            //e.Value = bloodType.VOLUME + " " + bloodType.SERVICE_UNIT_NAME;
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

        private void btnAddBlood_Click(object sender, EventArgs e)
        {
            try
            {
                if (actionType == GlobalVariables.ActionView)// || actionType == GlobalVariables.ActionViewForEdit)
                    return;

                if (actionBosung == GlobalVariables.ActionEdit)
                {
                    if (this.currentBloodTypeADOForEdit != null)
                    {
                        EditMedicineTypeClick(sender, e);
                    }
                }
                else
                {
                    AddMedicineTypeClick(sender, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAddBlood_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.actionBosung == GlobalVariables.ActionEdit)
                    {
                        if (this.currentBloodTypeADOForEdit != null)
                        {
                            this.EditMedicineTypeClick(sender, e);
                        }
                    }
                    else
                    {
                        this.AddMedicineTypeClick(sender, e);
                    }
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckValidForAdd()
        {
            bool valid = true;
            try
            {
                this.positionHandleBoXung = -1;
                string message = "";
                if (this.CurrentHisTreatment == null)
                {
                    message += (String.Format(ResourceMessage.KhongTimThayHoSoDieuTriCoId, treatmentId));
                    valid = false;
                }
                if (valid && this.currentPatient == null)
                {
                    message += (String.Format(ResourceMessage.KhongTimThayHoSoDieuTriCoId, this.CurrentHisTreatment.PATIENT_ID));
                    valid = false;
                }
                if (valid && this.currentHisPatientTypeAlter == null || this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == 0)
                {
                    message += (String.Format(ResourceMessage.KhongTimThayDoiTuongThanhToanTrongThoiGianYLenh, Inventec.Common.DateTime.Convert.TimeNumberToDateString(intructionTimeSelecteds.First())));
                    valid = false;
                }
                valid = valid && (this.actionBosung == GlobalVariables.ActionAdd || this.actionBosung == GlobalVariables.ActionEdit);
                valid = valid && (this.dxValidProviderBoXung__MedicinePage.Validate());
                valid = valid && (this.currentBloodType != null || this.currentBloodTypeADOForEdit != null);

                if (!String.IsNullOrEmpty(message))
                {
                    MessageManager.Show(message);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return valid;
        }

        private void RemoveRowInGrid(double idRow)
        {
            try
            {
                this.ListBloodTypeADOProcess.RemoveAll(o => o.ID == idRow);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AddMedicineTypeClick(object sender, EventArgs e)
        {
            try
            {
                bool valid = true;
                valid = this.CheckValidForAdd();
                if (valid)
                {
                    if (chkShowGroupBlood.Checked)
                    {
                        if (!string.IsNullOrEmpty(currentPatient.BLOOD_ABO_CODE))
                        {
                            var dtAbo = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_ABO>().First(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboBloodABO.EditValue ?? "0").ToString()));
                            HIS_BLOOD_RH dtRh = null;
                            if (cboBloodRH.EditValue !=null)
                                dtRh = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_RH>().First(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboBloodRH.EditValue ?? "0").ToString()));
                            if (dtRh == null ? currentPatient.BLOOD_ABO_CODE != dtAbo.BLOOD_ABO_CODE  : (currentPatient.BLOOD_ABO_CODE != dtAbo.BLOOD_ABO_CODE && currentPatient.BLOOD_RH_CODE != dtRh.BLOOD_RH_CODE))
                            {
                                if (HisConfigCFG.IsAllowToAssignDifferenceBloodOption)
                                {
                                    string message = null;
                                    if (dtRh != null)
                                        message = String.Format("Bệnh nhân có thông tin máu khác với thông tin máu đã chọn (nhóm máu: {0}, Rh: {1})", dtAbo.BLOOD_ABO_CODE, dtRh.BLOOD_RH_CODE);
									else
                                        message = String.Format("Bệnh nhân có thông tin máu khác với thông tin máu đã chọn (nhóm máu: {0})", dtAbo.BLOOD_ABO_CODE);
                                    DevExpress.XtraEditors.XtraMessageBox.Show(message, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                                    return;
                                }
                                else
                                {
                                    string message = null;
                                    if (dtRh != null)
                                        message = String.Format("Bệnh nhân có thông tin máu khác với thông tin máu đã chọn (nhóm máu: {0}, Rh: {1}). Bạn có muốn bổ sung không?", dtAbo.BLOOD_ABO_CODE, dtRh.BLOOD_RH_CODE);
                                    else
                                        message = String.Format("Bệnh nhân có thông tin máu khác với thông tin máu đã chọn (nhóm máu: {0}). Bạn có muốn bổ sung không?", dtAbo.BLOOD_ABO_CODE);
                                    if (DevExpress.XtraEditors.XtraMessageBox.Show(message, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) == DialogResult.No)
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                    }                
                    //Neu dang la sua du lieu da them vao truoc do thi dau tien phai remove du lieu cu khoi danh sach
                    //sau do moi bo xung them du lieu moi vao
                    if (this.actionBosung == GlobalVariables.ActionEdit)
                    {
                        this.RemoveRowInGrid(this.currentBloodType.BLOOD_TYPE_ID);
                    }

                    if (this.ListBloodTypeADOProcess == null)
                        this.ListBloodTypeADOProcess = new List<BloodTypeADO>();

                    var manuMedicineTypeSDO = this.ListBloodTypeADOProcess.FirstOrDefault(o => o.ID == this.currentBloodType.BLOOD_TYPE_ID);
                    if ((manuMedicineTypeSDO != null && manuMedicineTypeSDO.ID > 0))
                    {

                        if (MessageBox.Show(ResourceMessage.MauDaKeBanCoMuonThem,
                                     ResourceMessage.CanhBao,
                                     MessageBoxButtons.YesNo) == DialogResult.No)
                            return;
                        ////manuMedicineTypeSDO.AMOUNT += spinAmount__BloodPage.Value;
                    }
                    //else TODO
                    //{
                    this.currentBloodTypeADO = new BloodTypeADO();
                    this.currentBloodTypeADO.ID = this.currentBloodType.BLOOD_TYPE_ID;
                    this.currentBloodTypeADO.AMOUNT = spinAmount__BloodPage.Value;
                    this.currentBloodTypeADO.BLOOD_TYPE_CODE = this.currentBloodType.BLOOD_TYPE_CODE;
                    this.currentBloodTypeADO.BLOOD_TYPE_NAME = this.currentBloodType.BLOOD_TYPE_NAME;
                    //this.currentBloodTypeADO.SERVICE_UNIT_NAME = this.currentBloodType.SERVICE_UNIT_NAME;
                    this.currentBloodTypeADO.SERVICE_ID = this.currentBloodType.SERVICE_ID;
                    //Lấy đối tượng thanh toán của bệnh nhân làm đối tượng mặc định
                    MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType = this.ChoosePatientTypeDeffautlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, this.currentBloodType.SERVICE_ID);
                    if (patientType != null)
                    {
                        this.currentBloodTypeADO.PATIENT_TYPE_ID = patientType.ID;
                        this.currentBloodTypeADO.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                        this.currentBloodTypeADO.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;

                        this.currentBloodTypeADO.ErrorTypePatientTypeId = ErrorType.None;
                        this.currentBloodTypeADO.ErrorMessagePatientTypeId = "";
                    }
                    else
                    {
                        //MessageBox.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc));
                        //Inventec.Common.Logging.LogSystem.Info("patientType != null");
                        //return;
                        this.currentBloodTypeADO.PATIENT_TYPE_ID = null;
                        this.currentBloodTypeADO.ErrorTypePatientTypeId = ErrorType.Warning;
                        this.currentBloodTypeADO.ErrorMessagePatientTypeId = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc);
                    }

                    if (this.cboBloodABO.EditValue != null)
                    {
                        this.currentBloodTypeADO.BLOOD_ABO_ID = Inventec.Common.TypeConvert.Parse.ToInt64((this.cboBloodABO.EditValue ?? "0").ToString());
                    }
                    if (this.cboBloodRH.EditValue != null)
                    {
                        this.currentBloodTypeADO.BLOOD_RH_ID = Inventec.Common.TypeConvert.Parse.ToInt64((this.cboBloodRH.EditValue ?? "0").ToString());
                    }
					if (this.currentBloodType.SERVICE_ID > 0)
					{
						var dtService = lstSerivce.FirstOrDefault(o => o.ID == this.currentBloodType.SERVICE_ID);
						if (dtService != null)
						{
                            this.currentBloodTypeADO.SERVICE_TYPE_ID = dtService.SERVICE_TYPE_ID;
                            this.currentBloodTypeADO.HEIN_LIMIT_PRICE_IN_TIME = dtService.HEIN_LIMIT_PRICE_IN_TIME;
							this.currentBloodTypeADO.HEIN_LIMIT_PRICE_INTR_TIME = dtService.HEIN_LIMIT_PRICE_INTR_TIME;
							this.currentBloodTypeADO.HEIN_LIMIT_PRICE_OLD = dtService.HEIN_LIMIT_PRICE_OLD;
							this.currentBloodTypeADO.HEIN_LIMIT_PRICE = dtService.HEIN_LIMIT_PRICE;
						}
					}
                    currentBloodTypeADO.PRICE = GetPriceByBloodType(currentBloodTypeADO);
                    currentBloodTypeADO.TOT_PRICE = currentBloodTypeADO.PRICE * currentBloodTypeADO.AMOUNT;

                    this.ListBloodTypeADOProcess.Insert(0, this.currentBloodTypeADO);
                    //}

                    this.gridControlServiceProcess__TabBlood.DataSource = null;
                    this.gridControlServiceProcess__TabBlood.DataSource = this.ListBloodTypeADOProcess;
                    this.EnableAndDisableControlWithGirdcontrol();

                    this.ReSetDataInputAfterAdd(valid);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EditMedicineTypeClick(object sender, EventArgs e)
        {
            try
            {
                bool valid = true;
                valid = this.CheckValidForAdd();
                if (valid)
                {
                    for (int i = 0; i < this.ListBloodTypeADOProcess.Count; i++)
                    {
                        if (this.ListBloodTypeADOProcess[i].ID == this.currentBloodTypeADOForEdit.ID)
                        {
                            this.ListBloodTypeADOProcess[i].AMOUNT = this.spinAmount__BloodPage.Value;
                            MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType = this.ChoosePatientTypeDeffautlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, this.currentBloodType.SERVICE_ID);
                            if (patientType != null)
                            {
                                this.ListBloodTypeADOProcess[i].PATIENT_TYPE_ID = patientType.ID;
                                this.ListBloodTypeADOProcess[i].PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                                this.ListBloodTypeADOProcess[i].PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Info("patientType != null");
                            }

                            if (this.cboBloodABO.EditValue != null)
                            {
                                this.ListBloodTypeADOProcess[i].BLOOD_ABO_ID = Inventec.Common.TypeConvert.Parse.ToInt64((this.cboBloodABO.EditValue ?? "0").ToString());
                            }
                            if (this.cboBloodRH.EditValue != null)
                            {
                                this.ListBloodTypeADOProcess[i].BLOOD_RH_ID = Inventec.Common.TypeConvert.Parse.ToInt64((this.cboBloodRH.EditValue ?? "0").ToString());
                            }
                        }
                    }
                    this.gridControlServiceProcess__TabBlood.DataSource = null;
                    this.gridControlServiceProcess__TabBlood.DataSource = this.ListBloodTypeADOProcess;
                    this.spinAmount__BloodPage.Value = 0;
                    this.EnableAndDisableControlWithGirdcontrol();
                }

                this.ReSetDataInputAfterAdd(valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableAndDisableControlWithGirdcontrol()
        {
            try
            {
                if (this.gridViewServiceProcess__TabBlood.RowCount > 0)
                {
                    this.btnSave.Enabled = this.btnSaveAndPrint.Enabled = true;
                }
                else
                {
                    this.btnSave.Enabled = this.btnSaveAndPrint.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadMediStockExportCombo(string searchCode)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    this.cboMediStockExport_TabBlood.EditValue = null;
                    this.cboMediStockExport_TabBlood.Focus();
                    this.cboMediStockExport_TabBlood.SelectAll();
                    this.cboMediStockExport_TabBlood.ShowPopup();
                }
                else
                {
                    var datas = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().Where(o => o.MEDI_STOCK_CODE.ToUpper().Contains(searchCode.ToUpper())).ToList();
                    var searchResult = (datas != null && datas.Count > 0) ? (datas.Count == 1 ? datas : datas.Where(o => o.MEDI_STOCK_CODE.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                    if (searchResult != null && searchResult.Count == 1)
                    {
                        this.SelectOneMediStockExport(searchResult[0]);
                    }
                    else
                    {
                        this.cboMediStockExport_TabBlood.EditValue = null;
                        this.cboMediStockExport_TabBlood.Focus();
                        this.cboMediStockExport_TabBlood.SelectAll();
                        this.cboMediStockExport_TabBlood.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectOneMediStockExport(MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK dataOne)
        {
            try
            {
                this.cboMediStockExport_TabBlood.EditValue = dataOne.ID;

                this.LoadDataToGridBloodType(dataOne);

                this.gridControlServiceProcess__TabBlood.DataSource = null;
                this.EnableAndDisableControlWithGirdcontrol();

                this.txtKeyword.Focus();
                this.txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultMedicineMaterialTotalPrice()
        {
            try
            {
                List<BloodTypeADO> bloodTypes = (List<BloodTypeADO>)this.gridControlServiceProcess__TabBlood.DataSource;
                if (bloodTypes != null)
                {
                    List<BloodTypeADO> sereServSdoTotalPrices = new List<BloodTypeADO>();
                    foreach (var item in bloodTypes)
                    {
                        if (item.AMOUNT <= 0)
                        {
                            item.ErrorMessageAmount = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc);
                            item.ErrorTypeAmount = ErrorType.Warning;
                        }
                        else
                        {
                            item.ErrorMessageAmount = "";
                            item.ErrorTypeAmount = ErrorType.None;
                        }
                        if (item.PATIENT_TYPE_ID == null || item.PATIENT_TYPE_ID <= 0)
                        {
                            item.ErrorMessagePatientTypeId = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc);
                            item.ErrorTypePatientTypeId = ErrorType.Warning;
                        }
                        else
                        {
                            item.ErrorMessagePatientTypeId = "";
                            item.ErrorTypePatientTypeId = ErrorType.None;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlServiceProcess__TabBlood_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (actionType == GlobalVariables.ActionView || actionType == GlobalVariables.ActionViewForEdit)
                    return;

                this.currentBloodTypeADOForEdit = (BloodTypeADO)this.gridViewServiceProcess__TabBlood.GetFocusedRow();
                if (this.currentBloodTypeADOForEdit != null)
                {
                    this.idRow = this.currentBloodTypeADOForEdit.IdRow;
                    this.focusedRowHandle = this.gridViewBloodType__BloodPage.FocusedRowHandle;
                    this.spinAmount__BloodPage.Value = this.currentBloodTypeADOForEdit.AMOUNT ?? 0;
                    this.cboBloodABO.EditValue = this.currentBloodTypeADOForEdit.BLOOD_ABO_ID;
                    this.cboBloodABO.Properties.Buttons[1].Visible = (this.currentBloodTypeADOForEdit.BLOOD_ABO_ID > 0);
                    this.cboBloodRH.EditValue = this.currentBloodTypeADOForEdit.BLOOD_RH_ID;
                    this.cboBloodRH.Properties.Buttons[1].Visible = (this.currentBloodTypeADOForEdit.BLOOD_RH_ID > 0);
                    this.actionBosung = GlobalVariables.ActionEdit;
                    this.spinAmount__BloodPage.SelectAll();
                    this.spinAmount__BloodPage.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess__TabBlood_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                this.SetDefaultMedicineMaterialTotalPrice();
                var data = (BloodTypeADO)gridViewServiceProcess__TabBlood.GetFocusedRow();
                if (data != null)
                {
                    if (e.Column.FieldName == "PATIENT_TYPE_ID")
                    {
                        data.PRICE = GetPriceByBloodType(data);
                        data.TOT_PRICE = data.PRICE * data.AMOUNT;
                    }
                    if (e.Column.FieldName == "AMOUNT")
                    {
                        data.TOT_PRICE = data.PRICE * data.AMOUNT;
                    }
                }            
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess__TabBlood_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                BloodTypeADO data = null;
                if (e.RowHandle > -1)
                {
                    data = (BloodTypeADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "PATIENT_TYPE_ID")
                    {
                        e.RepositoryItem = this.repositoryItemcboPatientType_TabBlood_GridLookUp;
                    }
                    else if (e.Column.FieldName == "IsExpend")
                    {
                        e.RepositoryItem = this.repositoryItemChkIsExpend__BloodPage;
                    }
                    else if (e.Column.FieldName == "IsKHBHYT")
                    {
                        if (this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                        {
                            e.RepositoryItem = this.repositoryItemChkIsKH__BloodPage;
                        }
                        else
                        {
                            e.RepositoryItem = this.repositoryItemChkIsKH_Disable__BloodPage;
                        }
                    }
                    else if (e.Column.FieldName == "AMOUNT")
                    {
                        e.RepositoryItem = this.repositoryItemSpinAmount__BloodPage;
                    }
                    else if (e.Column.FieldName == "IsOutParentFee")
                    {
                        e.RepositoryItem = this.repositoryItemChkOutKtcFee_Enable_TabBlood;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess__TabBlood_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    BloodTypeADO bloodTypeADO = (BloodTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (bloodTypeADO != null)
                    {
                        if (e.Column.FieldName == "TOTAL_PRICE")
                        {
                            if (bloodTypeADO.PATIENT_TYPE_ID != 0)
                            {
                                var searchServicePrice = this.servicePatyInBranchs[bloodTypeADO.SERVICE_ID].Where(o => o.PATIENT_TYPE_ID == bloodTypeADO.PATIENT_TYPE_ID).OrderByDescending(m => m.MODIFY_TIME).ToList();
                                if (searchServicePrice != null && searchServicePrice.Count > 0)
                                {
                                    e.Value = Inventec.Common.Number.Convert.NumberToString(searchServicePrice[0].PRICE, ConfigApplications.NumberSeperator);
                                }
                            }
                        }
                        if (e.Column.FieldName == "PRICE_DISPLAY")
                        {
                            e.Value = bloodTypeADO.PRICE;
                        }
                        if (e.Column.FieldName == "TOT_PRICE_DISPLAY")
                        {
                            e.Value = bloodTypeADO.TOT_PRICE;
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

        private decimal? GetPriceByBloodType(BloodTypeADO bloodTypeADO)
        {
            decimal? resultData = null;
            decimal? heinLimitPrice = null;
            decimal? heinLimitRatio = null;
            try
            {
                long instructionTime = this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0 ? this.intructionTimeSelecteds.FirstOrDefault() : 0;
                if (bloodTypeADO.PATIENT_TYPE_ID != 0 && BranchDataWorker.DicServicePatyInBranch.ContainsKey(bloodTypeADO.SERVICE_ID) && instructionTime > 0)
                {
                    List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> dataCombo = new List<V_HIS_EXECUTE_ROOM>();
                    var serviceRoomViews = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>();
                    List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM> arrExcuteRoomCode = new List<V_HIS_SERVICE_ROOM>();
                    if (this.allDataExecuteRooms != null && this.allDataExecuteRooms.Count > 0 && serviceRoomViews != null && serviceRoomViews.Count > 0)
                    {
                        arrExcuteRoomCode = serviceRoomViews.Where(o => bloodTypeADO != null && o.SERVICE_ID == bloodTypeADO.SERVICE_ID).ToList();
                        dataCombo = ((arrExcuteRoomCode != null && arrExcuteRoomCode.Count > 0 && this.allDataExecuteRooms != null) ?
                            this.allDataExecuteRooms.Where(o => arrExcuteRoomCode.Select(p => p.ROOM_ID).Contains(o.ROOM_ID) && o.BRANCH_ID == this.requestRoom.BRANCH_ID).ToList()
                            : null);
                    }

                    var checkExecuteRoom = dataCombo != null && dataCombo.Count > 0 ? dataCombo.FirstOrDefault() : null;
                    if (checkExecuteRoom != null)
                    {
                        bloodTypeADO.TDL_EXECUTE_BRANCH_ID = checkExecuteRoom.BRANCH_ID;
                    }
                    else
                    {
                        bloodTypeADO.TDL_EXECUTE_BRANCH_ID = dataCombo != null && dataCombo.Count > 0 ? dataCombo.FirstOrDefault().BRANCH_ID : HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.GetCurrentBranchId();
                    }
                    long? intructionNumByType = null;

                    List<HIS_SERE_SERV> sameServiceType = this.sereServWithTreatment != null ? this.sereServWithTreatment.Where(o => o.TDL_SERVICE_TYPE_ID == bloodTypeADO.SERVICE_TYPE_ID).ToList() : null;
                    intructionNumByType = sameServiceType != null ? (long)sameServiceType.Count() + 1 : 1;

                    List<V_HIS_SERVICE_PATY> servicePaties = BranchDataWorker.ServicePatyWithListPatientType(bloodTypeADO.SERVICE_ID, new List<long> { bloodTypeADO.PATIENT_TYPE_ID ?? 0 });

                    V_HIS_SERVICE_PATY oneServicePatyPrice = MOS.ServicePaty.ServicePatyUtil.GetApplied(servicePaties, bloodTypeADO.TDL_EXECUTE_BRANCH_ID, currentModule.RoomId, this.requestRoom.ID, this.requestRoom.DEPARTMENT_ID, instructionTime, currentTreatment.IN_TIME, bloodTypeADO.SERVICE_ID, bloodTypeADO.PATIENT_TYPE_ID ?? 0, null);
                 

                    if (bloodTypeADO.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                    {
                        this.GetHeinLimitPrice(bloodTypeADO, instructionTime, currentTreatment.IN_TIME, ref heinLimitPrice, ref heinLimitRatio);

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

        private void GetHeinLimitPrice(BloodTypeADO hisService, long instructionTime, long inTime, ref decimal? heinLimitPrice, ref decimal? heinLimitRatio)
        {
            if (hisService.HEIN_LIMIT_PRICE.HasValue || hisService.HEIN_LIMIT_PRICE_OLD.HasValue)
            {
                if (hisService.HEIN_LIMIT_PRICE_IN_TIME.HasValue)
                {
                    heinLimitPrice = inTime < hisService.HEIN_LIMIT_PRICE_IN_TIME.Value ? hisService.HEIN_LIMIT_PRICE_OLD : hisService.HEIN_LIMIT_PRICE;
                }
                else if (hisService.HEIN_LIMIT_PRICE_INTR_TIME.HasValue)
                {
                    heinLimitPrice = instructionTime < hisService.HEIN_LIMIT_PRICE_INTR_TIME.Value ? hisService.HEIN_LIMIT_PRICE_OLD : hisService.HEIN_LIMIT_PRICE;
                }
                else
                {
                    heinLimitPrice = hisService.HEIN_LIMIT_PRICE;
                }
            }
        }

        private void gridViewServiceProcess__TabBlood_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
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
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess__TabBlood_CustomRowError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                var index = this.gridViewServiceProcess__TabBlood.GetDataSourceRowIndex(e.RowHandle);
                if (index < 0)
                {
                    e.Info.ErrorType = ErrorType.None;
                    e.Info.ErrorText = "";
                    return;
                }
                var listFunds = this.gridViewServiceProcess__TabBlood.DataSource as List<BloodTypeADO>;
                var row = listFunds[index];
                if (e.ColumnName == "AMOUNT")
                {
                    if (row.ErrorTypeAmount == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypeAmount);
                        e.Info.ErrorText = (string)(row.ErrorMessageAmount);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                if (e.ColumnName == "PATIENT_TYPE_ID")
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

        private void gridViewServiceProcess__TabBlood_CustomRowColumnError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                if (e.ColumnName == "AMOUNT" || e.ColumnName == "PATIENT_TYPE_ID")
                {
                    this.gridViewServiceProcess__TabBlood_CustomRowError(sender, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess__TabBlood_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                BloodTypeADO data = view.GetFocusedRow() as BloodTypeADO;
                if (view.FocusedColumn.FieldName == "PATIENT_TYPE_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (data != null)
                    {
                        this.FillDataIntoPatientTypeCombo(data.SERVICE_ID, editor);
                        editor.EditValue = data.PATIENT_TYPE_ID;
                    }
                }
                else if (view.FocusedColumn.FieldName == "IsKHBHYT" && view.ActiveEditor is CheckEdit)
                {
                    CheckEdit editor = view.ActiveEditor as CheckEdit;
                    // Kiểm tra các điều kiện: 
                    //1. Đối tượng BN = BHYT
                    //2. Loại hình thanh toán !=BHYT
                    //3. Dịch vụ đó có giá bán = BHYT
                    //4. Dịch vụ đó có giá bán BHYT<giá bán của loại đối tượng TT
                    if (this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                    {
                        if (data.PATIENT_TYPE_ID != this.currentHisPatientTypeAlter.PATIENT_TYPE_ID)
                        {
                            var service = this.servicePatyInBranchs[data.SERVICE_ID].Where(o => o.PATIENT_TYPE_ID == this.currentHisPatientTypeAlter.PATIENT_TYPE_ID).ToList();
                            if (service != null && service.Count > 0)
                            {
                                editor.ReadOnly = false;
                            }
                            else
                            {
                                editor.ReadOnly = true;
                            }
                        }
                        else
                        {
                            editor.ReadOnly = true;
                        }
                    }
                    else
                    {
                        editor.ReadOnly = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataIntoPatientTypeCombo(long serviceId, DevExpress.XtraEditors.GridLookUpEdit patientTypeCombo)
        {
            try
            {
                List<long> arrPatientTypeIds = new List<long>();
                // kiểm tra chính sách giá
                if (this.servicePatyInBranchs.ContainsKey(serviceId))
                {
                    var arrPatientTypeCode = this.servicePatyInBranchs[serviceId].Select(o => o.PATIENT_TYPE_CODE).Distinct().ToList();
                    if (arrPatientTypeCode != null && arrPatientTypeCode.Count > 0)
                    {
                        List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> dataCombo = this.currentPatientTypeWithPatientTypeAlter.Where(o => arrPatientTypeCode.Contains(o.PATIENT_TYPE_CODE)).ToList();
                        InitComboPatientType(patientTypeCombo, dataCombo);
                    }
                    else
                    {
                        InitComboPatientType(patientTypeCombo, null);
                    }
                }
                else
                {
                    InitComboPatientType(patientTypeCombo, null);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckValidGridBLoodType()
        {
            bool success = true;
            try
            {
                string message = "";

                var medicinesMaterials = this.gridViewServiceProcess__TabBlood.DataSource as List<BloodTypeADO>;
                if (medicinesMaterials != null && medicinesMaterials.Count > 0)
                {
                    foreach (var item in medicinesMaterials)
                    {
                        string error = String.Format(ResourceMessage.MessageMau, item.BLOOD_TYPE_NAME);
                        bool valid = true;
                        if (item.AMOUNT <= 0)
                        {
                            valid = false;
                            error += " " + ResourceMessage.KhongNhapSoLuong;
                        }
                        if (item.PATIENT_TYPE_ID == null || item.PATIENT_TYPE_ID <= 0)
                        {
                            valid = false;
                            error += " " + ResourceMessage.KhongCoDoiTuongThanhToan;
                        }
                        if (!valid)
                        {
                            message += error;
                            success = false;
                        }
                    }
                }

                if (!success)
                {
                    MessageManager.Show(message);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return success;
        }

        void ChangeLockButtonWhileProcess(bool isLock)
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionView)
                    return;

                this.btnSave.Enabled = isLock;
                this.btnSaveAndPrint.Enabled = isLock;
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
                this.isSaveAndPrint = true;
                this.SaveProcess(this.isSaveAndPrint);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_TabBlood_Click(object sender, EventArgs e)
        {
            try
            {
                this.isSaveAndPrint = false;
                this.SaveProcess(this.isSaveAndPrint);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.SetDefaultData();
                this.LoadIcdDefault();
                this.ListBloodTypeADOProcess = new List<BloodTypeADO>();
                this.gridControlServiceProcess__TabBlood.DataSource = null;
                MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK ms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMediStockExport_TabBlood.EditValue ?? "0").ToString()));
                this.LoadDataToGridBloodType(ms);
                this.EnableAndDisableControlWithGirdcontrol();
                this.SetEnableButtonControlBlood();
                this.ReSetDataInputAfterAdd(true);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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
                listArgs.Add(this.treatmentId);
                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");

                WaitingManager.Hide();
                ((Form)extenceInstance).Show();
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ShowDialog()
        {
            try
            {
                isReturn = false;
                XtraMessageBoxArgs args = new XtraMessageBoxArgs();
                args.Caption = "Message";
                args.Text = "Buttons in this message box show have custom images.";
                args.Buttons = new DialogResult[] { DialogResult.OK, DialogResult.Retry };
                args.Showing += Args_Showing;
                XtraMessageBox.Show(args).ToString();

                //Rectangle rect = Screen.GetWorkingArea(this);
                //popupControlContainer1 .ShowPopup(new Point(rect.Width / 2 - popupControlContainer1.Width / 2, rect.Height / 2 - popupControlContainer1.Height / 2));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void Args_Showing(object sender, XtraMessageShowingArgs e)
        {
            foreach (var control in e.Form.Controls)
            {
                SimpleButton button = control as SimpleButton;
                if (button != null)
                {

                    switch (button.DialogResult.ToString())
                    {
                        case ("OK"):
                            button.Text = "Tạo biên bản hội chẩn";
                            button.Width = 150;
                            button.Click += (ss, ee) =>
                            {
                                OpenModuleBB();
                            };
                            break;
                        case ("Retry"):
                            button.Text = "Đóng";
                            isReturn = true;
                            break;

                    }
                }
            }
        }

        private void OpenModuleBB()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.DebateDiagnostic").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.DebateDiagnostic");

                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    CommonParam param = new CommonParam();
                    //MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                    //filter.ORDER_DIRECTION = "DESC";
                    //filter.ORDER_FIELD = "MODIFY_TIME";
                    //HIS_SERVICE_REQ rs = new HIS_SERVICE_REQ();
                    //rs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();

                    List<object> listArgs = new List<object>();
                    //                    listArgs.Add(rs);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(currentModule, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveProcess(bool isPrintNow)
        {
            if (isReturn) return;
            CommonParam param = new CommonParam();
            bool success = false;
            bool valid = true;
            try
            {
                if (this.gridViewServiceProcess__TabBlood.IsEditing)
                    this.gridViewServiceProcess__TabBlood.CloseEditor();

                if (this.gridViewServiceProcess__TabBlood.FocusedRowModified)
                    this.gridViewServiceProcess__TabBlood.UpdateCurrentRow();

                this.HisPrescriptionSDO = new PatientBloodPresSDO();
                this.HisPrescriptionSDO.ExpMestBltyReqs = new List<HIS_EXP_MEST_BLTY_REQ>();
                this.ProcessDataInputApiAssignBlood();
                this.positionHandleControl = -1;
                if (!this.dxValidationProviderControl__MedicinePage.Validate())
                    return;
                if (!this.CheckValidGridBLoodType())
                {
                    this.gridControlServiceProcess__TabBlood.Focus();
                    return;
                }
                if (!(bool)icdProcessor.ValidationIcd(ucIcd))
                    return;

                this.ChangeLockButtonWhileProcess(false);

                valid = valid && (this.HisPrescriptionSDO != null && (this.HisPrescriptionSDO.ExpMestBltyReqs.Count > 0));
                if (valid)
                {
                    this.Activate();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.HisPrescriptionSDO.ExpMestBltyReqs), this.HisPrescriptionSDO.ExpMestBltyReqs));
                    if (this.HisPrescriptionSDO.ExpMestBltyReqs != null && this.HisPrescriptionSDO.ExpMestBltyReqs.Count > 0)
                    {
                        MOS.Filter.HisBloodTypeFilter blfilter = new MOS.Filter.HisBloodTypeFilter();
                        blfilter.IDs = this.HisPrescriptionSDO.ExpMestBltyReqs.Select(o => o.BLOOD_TYPE_ID).ToList();
                        lstBloodGroups = new BackendAdapter(param).Get<List<HIS_BLOOD_TYPE>>("api/HisBloodType/Get", ApiConsumers.MosConsumer, blfilter, ProcessLostToken, param);

                        if (lstBloodGroups != null && lstBloodGroups.Count > 0)
                        {
                            List<HIS_DEBATE> dataDebate = null;
                            List<string> strList = new List<string>();
                            List<HIS_SERVICE> listService = new List<HIS_SERVICE>();
                            MOS.Filter.HisServiceFilter filter = new MOS.Filter.HisServiceFilter();
                            filter.IDs = lstBloodGroups.Select(o => o.SERVICE_ID).ToList();
                            filter.MUST_BE_CONSULTED = 1;
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
                            var dataService = new BackendAdapter(param).Get<List<HIS_SERVICE>>("api/HisService/Get", ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataService), dataService));
                            long departmentID = 0;
                            if (cboMediStockExport_TabBlood.EditValue != null)
                            {
                                V_HIS_ROOM ms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().SingleOrDefault(o => o.ID == currentModule.RoomId);
                                departmentID = ms.DEPARTMENT_ID;
                            }

                            if (dataService != null && dataService.ToList().Count > 0)
                            {
                                MOS.Filter.HisDebateFilter dbFilter = new MOS.Filter.HisDebateFilter();
                                dbFilter.DEPARTMENT_ID = departmentID;
                                dbFilter.SERVICE_IDs = dataService.Select(o => o.ID).ToList();
                                dbFilter.TREATMENT_ID = treatmentId;
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dbFilter), dbFilter));
                                dataDebate = new BackendAdapter(param).Get<List<HIS_DEBATE>>("api/HisDebate/Get", ApiConsumers.MosConsumer, dbFilter, param);
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataDebate), dataDebate));
                                if (dataDebate != null && dataDebate.Count > 0)
                                {
                                    var lstIdServiceDebate = dataDebate.Select(o => o.SERVICE_ID).ToList();
                                    foreach (var item in dataService)
                                    {
                                        if (!lstIdServiceDebate.Contains(item.ID))
                                        {
                                            strList.Add(item.SERVICE_NAME);
                                            listService.Add(item);
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var item in dataService)
                                    {
                                        strList.Add(item.SERVICE_NAME);
                                        listService.Add(item);
                                    }
                                }
                                if (strList != null && strList.Count > 0)
                                {
                                    string message = String.Format("Khoa chỉ định chưa tạo Biên bản hội chẩn đối với (các) dịch vụ:\n {0}", String.Join(", ", strList));
                                   
                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listService), listService));

                                    frmServiceDebateConfirmNew frm = new frmServiceDebateConfirmNew(this.currentModule, listService, dataDebate, message, treatmentId);
                                    frm.ShowDialog();

                                }
                            }
                        }
                    }

                    WaitingManager.Show();
                    //Inventec.Common.Logging.LogSystem.Info("Du lieu dau vao truoc khi goi api (PatientBloodPresSDO)" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisPrescriptionSDO.Description), HisPrescriptionSDO.Description) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisPrescriptionSDO.ExpMestBltyReqs), HisPrescriptionSDO.ExpMestBltyReqs) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisPrescriptionSDO.InstructionTime), HisPrescriptionSDO.InstructionTime) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisPrescriptionSDO.MediStockId), HisPrescriptionSDO.MediStockId));

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisPrescriptionSDO), HisPrescriptionSDO));
                    if (this.actionType == GlobalVariables.ActionEdit && this._ServiceReqEdit != null && this._ServiceReqEdit.ID > 0)
                    {
                        HisPrescriptionSDO.Id = this._ServiceReqEdit.ID;
                        this.currentHisPrescriptionSDOPrint = new BackendAdapter(param).Post<PatientBloodPresResultSDO>(RequestUriStore.HIS_SERVICE_REQ__BLOOD_UPDATE, ApiConsumers.MosConsumer, HisPrescriptionSDO, ProcessLostToken, param);

                        this.HisPrescriptionSDOPrint = new List<PatientBloodPresResultSDO>();
                        if(currentHisPrescriptionSDOPrint!=null)
                            this.HisPrescriptionSDOPrint.Add(this.currentHisPrescriptionSDOPrint);
                    }
                    else
                        this.HisPrescriptionSDOPrint = new BackendAdapter(param).Post<List<PatientBloodPresResultSDO>>(RequestUriStore.HIS_SERVICE_REQ__BLOODPRESCREATE, ApiConsumers.MosConsumer, HisPrescriptionSDO, ProcessLostToken, param);

					Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisPrescriptionSDOPrint), HisPrescriptionSDOPrint));
                    if (this.HisPrescriptionSDOPrint != null && this.HisPrescriptionSDOPrint.Count >  0)
                    {
                        this.btnSave.Enabled = this.btnSaveAndPrint.Enabled = false;

                        success = true;
                        var ServiceReqBlood = HisPrescriptionSDOPrint.Where(o => o.ServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM).FirstOrDefault();
                        if (ServiceReqBlood != null)
                        {
                            if (this.processDataResult != null)
                                this.processDataResult(ServiceReqBlood);

                            //Gọi delegate xử lý cập nhật bệnh phụ tại module thực hiện gọi module chỉ định, truyền vào các giá trị "bệnh chính", "bệnh phụ",... đã nhập trên form chỉ định
                            if (this.processRefeshIcd != null)
                                this.processRefeshIcd(ServiceReqBlood.ServiceReq.ICD_CODE, ServiceReqBlood.ServiceReq.ICD_NAME, ServiceReqBlood.ServiceReq.ICD_SUB_CODE, ServiceReqBlood.ServiceReq.ICD_TEXT);
                        }
                        if (isPrintNow)
                        {
                            SetUpToPrint108(isPrintNow);
                        }
                        // ghi log
                        var bloodForProcessADOs = this.gridControlServiceProcess__TabBlood.DataSource as List<BloodTypeADO>;
                        var bloodGroup = bloodForProcessADOs.GroupBy(o => new { o.PATIENT_TYPE_ID, o.ID });
                        string testIndexStr = "";
                        foreach (var item in bloodGroup)
                        {
                            testIndexStr += item.First().BLOOD_TYPE_CODE + " - " + item.First().AMOUNT + "; ";
                        }

                        string message = "";
                        if (this.actionType == GlobalVariables.ActionEdit)
                        {
							foreach (var item in HisPrescriptionSDOPrint)
							{
                                if(item.ServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM)
								{
                                    message += string.Format("Sửa đơn máu. SERVICE_REQ_CODE: {0}. TDL_TREATMENT_CODE: {1}. [BLOOD_TYPE_CODE - AMOUNT]: [{2}].\r\n", item.ServiceReq.SERVICE_REQ_CODE, item.ServiceReq.TDL_TREATMENT_CODE, testIndexStr);
                                }                                    
							}
                        }
                        else
                        {
                            foreach (var item in HisPrescriptionSDOPrint)
                            {
                                if (item.ServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM)
                                {
                                    message = string.Format("Kê đơn máu. SERVICE_REQ_CODE: {0}. TDL_TREATMENT_CODE: {1}. [BLOOD_TYPE_CODE - AMOUNT]: [{2}]", item.ServiceReq.SERVICE_REQ_CODE, item.ServiceReq.TDL_TREATMENT_CODE, testIndexStr);
                                }
                            }
                           
                        }

                        string login = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        SdaEventLogCreate eventlog = new SdaEventLogCreate();
                        eventlog.Create(login, null, true, message);

                        this.actionType = GlobalVariables.ActionView;
                        this.actionBosung = GlobalVariables.ActionView;
                        this.ResetDataForm();

                    }

                    WaitingManager.Hide();
                    #region Show message
                    MessageManager.Show(this, param, success);
                    #endregion
                }
                else
                {
                    MessageManager.Show(ResourceMessage.DuLieuKhongHopHeVuiLongKiemTraLai);
                }
                this.ChangeLockButtonWhileProcess(true);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                this.ChangeLockButtonWhileProcess(true);
                Inventec.Common.Logging.LogSystem.Warn(ex);
                MessageManager.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBXuatHienExceptionChuaKiemDuocSoat));
            }
        }
        private void SetUpToPrint108(bool IsPrintNow)
		{
			try
			{
                
                if (HisPrescriptionSDOPrint == null && HisPrescriptionSDOPrint.Count == 0)
                {
                    MessageManager.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThongBaoDuLieuTrong));
                    return;
                }
                WaitingManager.Show();
                this.GroupStreamPrint = new List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo>();
                richEditorMain.SetActionCancelChooseTemplate(CancelChooseTemplate);


                CommonParam param = new CommonParam();
                if(HisPrescriptionSDOPrint.Where(o=>o.ServiceReq != null).ToList().Count > 1)
				{
                    IsMultilData = true;
				}                    
				foreach (var item in HisPrescriptionSDOPrint)
				{
                    this.HisPrescriptionSDOResultPrint = item;

                    bool result = false;
                    if (!string.IsNullOrEmpty(currentFileName108))
                    {
                        InPhieuYeuCauChiDinhMau108(currentTypeCode108, currentFileName108, ref result, this.HisPrescriptionSDOResultPrint);
                    }
                    else
                    {
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000108, DelegateRunPrinter);
                    }
                    
                }              
                this.TotalPrint = total108;
                WaitingManager.Hide();
                if(!this.isSaveAndPrint)
                    PrintMerge();
            }
			catch (Exception ex)
			{
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
		}
      
        //Lay Chan doan mac dinh: Lay chan doan cuoi cung trong cac xu ly dich vu Kham benh
        private void LoadIcdDefault()
        {
            try
            {
                if (this.CurrentHisTreatment != null)
                {
                    //Nếu hồ sơ chưa có thông tin ICD, và là hồ sơ đến khám theo loại là hẹn khám thì khi chỉ định dịch vụ, tự động hiển thị ICD của đợt điều trị trước, tương ứng với mã hẹn khám
                    if (string.IsNullOrEmpty(this.CurrentHisTreatment.ICD_CODE)
                         && !String.IsNullOrEmpty(this.CurrentHisTreatment.PREVIOUS_ICD_CODE))
                    {
                        HIS.UC.Icd.ADO.IcdInputADO icd = new HIS.UC.Icd.ADO.IcdInputADO();
                        icd.ICD_CODE = CurrentHisTreatment.PREVIOUS_ICD_CODE;
                        icd.ICD_NAME = this.CurrentHisTreatment.PREVIOUS_ICD_NAME;
                        if (ucIcd != null)
                        {
                            icdProcessor.Reload(ucIcd, icd);
                        }

                        HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO subIcd = new HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO();
                        subIcd.ICD_SUB_CODE = this.CurrentHisTreatment.PREVIOUS_ICD_SUB_CODE;
                        subIcd.ICD_TEXT = this.CurrentHisTreatment.PREVIOUS_ICD_TEXT;
                        if (ucSecondaryIcd != null)
                        {
                            subIcdProcessor.Reload(ucSecondaryIcd, subIcd);
                        }
                    }
                    else
                    {
                        HIS.UC.Icd.ADO.IcdInputADO icd = new HIS.UC.Icd.ADO.IcdInputADO();
                        icd.ICD_CODE = CurrentHisTreatment.ICD_CODE;
                        icd.ICD_NAME = this.CurrentHisTreatment.ICD_NAME;
                        if (ucIcd != null)
                        {
                            icdProcessor.Reload(ucIcd, icd);
                        }

                        HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO subIcd = new HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO();
                        subIcd.ICD_SUB_CODE = this.CurrentHisTreatment.ICD_SUB_CODE;
                        subIcd.ICD_TEXT = this.CurrentHisTreatment.ICD_TEXT;
                        if (ucSecondaryIcd != null)
                        {
                            subIcdProcessor.Reload(ucSecondaryIcd, subIcd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetDataForm()
        {
            try
            {
                this.positionHandleBoXung = -1;
                this.positionHandleControl = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidProviderBoXung__MedicinePage, this.dxErrorProvider1);
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProviderControl__MedicinePage, this.dxErrorProvider1);
                //this.ListBloodTypeADOProcess = new List<BloodTypeADO>();
                //this.gridControlServiceProcess__TabBlood.DataSource = null;
                //this.lblTotalBloodBloodPrices.Text = "";
                MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK ms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMediStockExport_TabBlood.EditValue ?? "0").ToString()));
                this.LoadDataToGridBloodType(ms);
                this.spinAmount__BloodPage.Value = 0;
                this.cboBloodABO.EditValue = null;
                this.cboBloodRH.EditValue = null;
                this.SetEnableButtonControlBlood();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReSetDataInputAfterAdd(bool valid)
        {
            try
            {
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidProviderBoXung__MedicinePage, this.dxErrorProvider1);
                if (valid)
                {
                    this.spinAmount__BloodPage.Value = 0;
                    this.InitBloodADO__RH__FromPatientInfo();
                    this.txtKeyword.Focus();
                    this.txtKeyword.SelectAll();
                }
                else
                {
                    this.spinAmount__BloodPage.Focus();
                    this.spinAmount__BloodPage.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessDataInputApiAssignBlood()
        {
            try
            {
                //this.HisPrescriptionSDO.ExpMest.EXP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST;
                //this.HisPrescriptionSDO.ExpMest.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DNT;//.HisExpMestTypeId__Pres
                //if (this.cboMediStockExport_TabBlood.EditValue != null)
                //    this.HisPrescriptionSDO.MediStockId = Inventec.Common.TypeConvert.Parse.ToInt64((this.cboMediStockExport_TabBlood.EditValue ?? "0").ToString());
                this.HisPrescriptionSDO.TreatmentId = CurrentHisTreatment.ID;
                if (this.cboMediStockExport_TabBlood.EditValue != null)
                    this.HisPrescriptionSDO.MediStockId = Inventec.Common.TypeConvert.Parse.ToInt64((this.cboMediStockExport_TabBlood.EditValue ?? "0").ToString());
                this.HisPrescriptionSDO.TreatmentId = CurrentHisTreatment.ID;
                if (this.cboTracking.EditValue != null)
                {
                    this.HisPrescriptionSDO.TrackingId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTracking.EditValue.ToString());
                }
                if (this.cboExecuteGroup_TabBlood.EditValue != null)
                {
                    //this.HisPrescriptionSDO.ExecuteGroupId = Inventec.Common.TypeConvert.Parse.ToInt64((this.cboExecuteGroup_TabBlood.EditValue ?? "0").ToString());
                }
                if (this.cboUser.EditValue != null)
                {
                    this.HisPrescriptionSDO.RequestLoginName = this.cboUser.EditValue.ToString();
                    this.HisPrescriptionSDO.RequestUserName = this.cboUser.Text;
                }
                if (this.currentSereServ != null)
                {
                    this.HisPrescriptionSDO.ParentServiceReqId = this.currentSereServ.SERVICE_REQ_ID;
                }
                var bloodForProcessADOs = this.gridControlServiceProcess__TabBlood.DataSource as List<BloodTypeADO>;

                if (bloodForProcessADOs != null)
                {
                    IEnumerable<IGrouping<object, BloodTypeADO>> bloodGroup = null;
                    bloodGroup = bloodForProcessADOs.GroupBy(o => new { o.PATIENT_TYPE_ID, o.ID });

                    foreach (var itemGroup in bloodGroup)
                    {
                        HIS_EXP_MEST_BLTY_REQ mety = new HIS_EXP_MEST_BLTY_REQ();
                        mety.AMOUNT = (long)(itemGroup.Sum(o => o.AMOUNT ?? 0));
                        var firstBlood = itemGroup.FirstOrDefault();
                        if (firstBlood != null)
                        {
                            mety.BLOOD_TYPE_ID = firstBlood.ID;
                            mety.BLOOD_ABO_ID = firstBlood.BLOOD_ABO_ID;
                            mety.BLOOD_RH_ID = firstBlood.BLOOD_RH_ID;
                            if (this.currentSereServ != null)
                            {
                                mety.SERE_SERV_PARENT_ID = this.currentSereServ.ID;
                            }
                            if (firstBlood.IsOutParentFee)
                                mety.IS_OUT_PARENT_FEE = 1;

                            if ((firstBlood.PATIENT_TYPE_ID ?? 0) > 0)
                            {
                                mety.PATIENT_TYPE_ID = firstBlood.PATIENT_TYPE_ID;
                            }
                            this.HisPrescriptionSDO.ExpMestBltyReqs.Add(mety);
                        }
                    }
                }

                this.HisPrescriptionSDO.InstructionTime = intructionTimeSelecteds.First();

                if (ucIcd != null)
                {
                    var icdValue = icdProcessor.GetValue(ucIcd);
                    if (icdValue != null && icdValue is HIS.UC.Icd.ADO.IcdInputADO)
                    {
                        this.HisPrescriptionSDO.IcdCode = ((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                        if (!string.IsNullOrEmpty(((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE))
                        {
                            this.HisPrescriptionSDO.IcdCode = ((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                        }
                        this.HisPrescriptionSDO.IcdName = ((HIS.UC.Icd.ADO.IcdInputADO)icdValue).ICD_NAME;
                    }
                }
                if (ucSecondaryIcd != null)
                {
                    var subIcd = subIcdProcessor.GetValue(ucSecondaryIcd);
                    if (subIcd != null && subIcd is HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO)
                    {
                        this.HisPrescriptionSDO.IcdSubCode = ((HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                        this.HisPrescriptionSDO.IcdText = ((HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO)subIcd).ICD_TEXT;
                    }
                }
                this.HisPrescriptionSDO.BloodLevel = cboBloodLevel.SelectedIndex != -1 ? (short?)(cboBloodLevel.SelectedIndex + 1) : null;
                this.HisPrescriptionSDO.RequestRoomId = this.currentModule.RoomId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Shortcut
        private void bbtnCtrlA_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.btnAddBlood.Enabled)
                    this.btnAddBlood_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSaveAndPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.btnSaveAndPrint.Enabled)
                    this.SaveProcess(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnCtrlS_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.btnSave.Enabled)
                    this.btnSave_TabBlood_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.btnNew.Enabled)
                    this.btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void cboTracking_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTracking.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dropDownPrintBlood_Click(object sender, EventArgs e)
        {
            dropDownPrintBlood.ShowDropDown();
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
                            .Where(o => o.LOGINNAME.ToUpper().Contains(searchCode.ToUpper())).ToList();

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
                this.gridControlServiceProcess__TabBlood.Focus();
                this.gridViewServiceProcess__TabBlood.FocusedRowHandle = 0;
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
                        ACS.EFMODEL.DataModels.ACS_USER data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME == ((this.cboUser.EditValue ?? "").ToString()));
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
                        ACS.EFMODEL.DataModels.ACS_USER data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME == ((this.cboUser.EditValue ?? "").ToString()));
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

        private void btnShowModule_Click(object sender, EventArgs e)
        {
            try
            {
                OpenModuleBB();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                popupControlContainer1.HidePopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

		private void chkShowGroupBlood_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
                MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK ms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMediStockExport_TabBlood.EditValue ?? "0").ToString()));
                this.LoadDataToGridBloodType(ms);
                if(chkShowGroupBlood.Checked)
				{
                    cboBloodABO.Enabled = false;
                    cboBloodRH.Enabled = false;
                    grcBloodABO__TabBlood.OptionsColumn.AllowEdit = false;
                    grcBloodRH__TabBlood.OptionsColumn.AllowEdit = false;

                }
				else
				{
                    cboBloodABO.Enabled = true;
                    cboBloodRH.Enabled = true;
                    grcBloodABO__TabBlood.OptionsColumn.AllowEdit = true;
                    grcBloodRH__TabBlood.OptionsColumn.AllowEdit = true;
                }                    
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkShowGroupBlood.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkShowGroupBlood.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkShowGroupBlood.Name;
                    csAddOrUpdate.VALUE = (chkShowGroupBlood.Checked ? "1" : "");
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
                        if (item.KEY == chkShowGroupBlood.Name)
                        {
                            chkShowGroupBlood.Checked = item.VALUE == "1";
                        }                      
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChiDinhXetNghiem(BloodTypeADO row)
        {
            try
            {
                if (row != null)
                {
                    WaitingManager.Show();
                        List<object> listArgs = new List<object>();
                        AssignServiceTestADO assignBloodADO = new AssignServiceTestADO(0, 0, 0, null);
                        assignBloodADO.TreatmentId = treatmentId;
                        assignBloodADO.GenderName = currentPatient.GENDER_NAME;
                        assignBloodADO.PatientDob = currentPatient.DOB;
                        assignBloodADO.PatientName = currentPatient.VIR_PATIENT_NAME;
                    if (this._ServiceReqEdit != null)
                    {
                        assignBloodADO.ServiceReqId = _ServiceReqEdit.ID;

                        MOS.Filter.HisExpMestFilter filter = new MOS.Filter.HisExpMestFilter();
                        filter.SERVICE_REQ_ID = _ServiceReqEdit.ID;
                        var dataExpMests = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("/api/HisExpMest/Get", ApiConsumers.MosConsumer, filter, null);
                        if(dataExpMests!=null && dataExpMests.Count > 0)
						{
                            assignBloodADO.ExpMestId = dataExpMests[0].ID;
                        }                            
                    }
                        listArgs.Add(assignBloodADO);
                        long keyconfig = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>("HIS.Desktop.Plugins.AssignServiceTestSelect");
                        if (keyconfig == 1)
                        {
                            CallModuleProcess("HIS.Desktop.Plugins.AssignServiceTest", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                        }
                        else
                        {
                            CallModuleProcess("HIS.Desktop.Plugins.AssignServiceTestMulti", this.currentModule.RoomId, this.currentModule.RoomId, listArgs);
                        }

                        WaitingManager.Hide();
                    
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

		private void CallModuleProcess(string _moduleLink, long _roomId, long _roomTypeId, List<object> _listObj)
        {
            HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(_moduleLink, _roomId, _roomTypeId, _listObj);
        }

		private void gridViewServiceProcess__TabBlood_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
		{
			try
			{
                //var row = (ADO.BloodTypeADO)gridViewServiceProcess__TabBlood.GetFocusedRow();
                //if (row != null)
                //{                   
                //    PrintPopupMenuProcessor = new PrintPopupMenuProcessor(RightMenu_Click, barManager1, row);                
                //    PrintPopupMenuProcessor.RightMenu();
                //}
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
		}

        private void RightMenu_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem)
                {
                    var bbtnItem = sender as BarButtonItem;
                    PrintPopupMenuProcessor.ModuleType type = (PrintPopupMenuProcessor.ModuleType)(e.Item.Tag);
                    var serviceClick = (ADO.BloodTypeADO)gridViewServiceProcess__TabBlood.GetFocusedRow();
                    switch (type)
                    {
                        case PrintPopupMenuProcessor.ModuleType.ChiDinhXetNghiem:
                            ChiDinhXetNghiem(serviceClick);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

		private void frmHisAssignBlood_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
                arrControlEnableNotChange = null;
                dicOrderTabIndexControl = null;
                currentSereServ = null;
                processDataResult = null;
                processRefeshIcd = null;
                currentMestRoomByRooms = null;
                CurrentHisTreatment = null;
                currentHisPatientTypeAlter = null;
                currentPatientTypeWithPatientTypeAlter = null;
                currentPatient = null;
                ListBloodTypeADOProcess = null;
                currentBloodTypeADOForEdit = null;
                currentBloodTypeADO = null;
                currentBloodType = null;
                HisPrescriptionSDO = null;
                currentHisPrescriptionSDOPrint = null;
                HisPrescriptionSDOPrint = null;
                bloodGroups = null;
                lstBloodGroups = null;
                servicePatyInBranchs = null;
                intructionTimeSelecteds = null;
                tracking = null;
                allDataExecuteRooms = null;
                requestRoom = null;
                lstSerivce = null;
                sereServsInTreatmentRaw = null;
                sereServWithTreatment = null;
                currentTreatment = null;
                GroupStreamPrint = null;
                hisServiceReqPrint = null;
                richEditorMain = null;
                Print.DisposePrint();
                for (int i = 0; i < this.Controls.Count; i++)
                {
                    try
                    {
                        this.Controls[i].Dispose();
                    }
                    catch (Exception exxx)
                    {
                        Inventec.Common.Logging.LogSystem.Error(exxx);
                    }
                }
                this.Dispose(true);
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
		}

        private void cboBloodLevel_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {
                if(cboBloodLevel.SelectedIndex == 0) {
                    chkShowGroupBlood.Checked = true;
                }
                chkShowGroupBlood_CheckedChanged(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
    }
}

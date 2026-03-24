using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraTab;
using EMR.EFMODEL.DataModels;
using EMR.SDO;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ApprovaleDebate.ADO;
using HIS.Desktop.Plugins.ApprovaleDebate.Resources;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace HIS.Desktop.Plugins.ApprovaleDebate.ApprovaleDebate
{
    public partial class frmApprovaleDebate : FormBase
    {
        private Common.RefeshReference delegateRefresh;
        internal Inventec.Desktop.Common.Modules.Module currentModule { get; set; }
        V_HIS_SPECIALIST_EXAM currentHisSpecialistExam;
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmApprovaleDebate
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ApprovaleDebate.Resources.Lang", typeof(frmApprovaleDebate).Assembly);
                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSave.Caption = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.bbtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtICDsubName.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.txtICDsubName.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtICDsubName.ToolTip = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.txtICDsubName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtICDsub.ToolTip = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.txtICDsub.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboICD_YHCT.Properties.NullText = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.cboICD_YHCT.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboICD_YHCT.ToolTip = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.cboICD_YHCT.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtICD_YHCT.ToolTip = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.txtICD_YHCT.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboEmployee.Properties.NullText = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.cboEmployee.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabToDieuTri.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabToDieuTri.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabCDHA.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabCDHA.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabXetNghiem.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabXetNghiem.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabThuocVatTuMau.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabThuocVatTuMau.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabSieuAmNoiSoi.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabSieuAmNoiSoi.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabPhauThuatThuThuat.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabPhauThuatThuThuat.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabGiaiPhauBenh.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabGiaiPhauBenh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.layoutControlItem5.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.layoutControlItem6.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.layoutControlItem7.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.layoutControlItem8.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmApprovaleDebate(Inventec.Desktop.Common.Modules.Module module, Common.RefeshReference delegateRefresh, V_HIS_SPECIALIST_EXAM specialist)
                    : base(module)
        {
            try
            {
                this.delegateRefresh = delegateRefresh;
                this.currentHisSpecialistExam = specialist;
                InitializeComponent();
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                this.currentModule = module;
                this.Text = module.text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void frmApprovaleDebate_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetCaptionByLanguageKey();
                this.AddUc();
                this.InitComboEmployee();
                this.InitComboICD_YHCT();
                this.ValidControl();
                if (this.currentHisSpecialistExam != null)
                {
                    this.txtYKienBacSi.Text = this.currentHisSpecialistExam.EXAM_EXECUTE_CONTENT;
                    this.cboEmployee.EditValue = this.currentHisSpecialistExam.EXAM_EXECUTE_LOGINNAME;
                    this.cboICD_YHCT.EditValue = this.currentHisSpecialistExam.ICD_CODE;
                    this.txtICDsub.Text = this.currentHisSpecialistExam.ICD_SUB_CODE;
                    this.txtICDsubName.Text = this.currentHisSpecialistExam.ICD_TEXT;
                    this.txtDienBien.Text = this.currentHisSpecialistExam.CONTENT;
                    this.txtPPXuLy.Text = this.currentHisSpecialistExam.MEDICAL_INSTRUCTION;
                    LogSystem.Debug("IS_APPROVAL: " + currentHisSpecialistExam.IS_APPROVAL);
                    btnSave.Enabled = (currentHisSpecialistExam.IS_APPROVAL == null || currentHisSpecialistExam.IS_APPROVAL == 2);
                    this.ProcessSelectEmployee();
                    //
                    this.LoadDataSereServByTreatmentId(this.currentHisSpecialistExam);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        UCTreeListService ucCDHA, ucXetNghiem, ucThuocVatTu, ucSieuAm, ucPTTT, ucGPB;
        UCTreeListTracking ucAll;
        private void AddUc()
        {
            try
            {
                ucAll = new UCTreeListTracking(imageCollection1, currentModule);
                tabToDieuTri.Controls.Add(ucAll);
                ucAll.Dock = DockStyle.Fill;

                //
                ucCDHA = new UCTreeListService(imageCollection1, currentModule);
                tabCDHA.Controls.Add(ucCDHA);
                ucCDHA.Dock = DockStyle.Fill;
                //
                ucXetNghiem = new UCTreeListService(imageCollection1, currentModule);
                tabXetNghiem.Controls.Add(ucXetNghiem);
                ucXetNghiem.Dock = DockStyle.Fill;
                //
                ucThuocVatTu = new UCTreeListService(imageCollection1, currentModule);
                tabThuocVatTuMau.Controls.Add(ucThuocVatTu);
                ucThuocVatTu.Dock = DockStyle.Fill;
                //
                ucSieuAm = new UCTreeListService(imageCollection1, currentModule);
                tabSieuAmNoiSoi.Controls.Add(ucSieuAm);
                ucSieuAm.Dock = DockStyle.Fill;
                //
                ucPTTT = new UCTreeListService(imageCollection1, currentModule);
                tabPhauThuatThuThuat.Controls.Add(ucPTTT);
                ucPTTT.Dock = DockStyle.Fill;
                //
                ucGPB = new UCTreeListService(imageCollection1, currentModule);
                tabGiaiPhauBenh.Controls.Add(ucGPB);
                ucGPB.Dock = DockStyle.Fill;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void cboEmployee_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (cboEmployee.EditValue != null && e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                cboEmployee.EditValue = null;
        }

        //private void InitComboEmployee()
        //{
        //    try
        //    {
        //        var data = BackendDataWorker.Get<V_HIS_EMPLOYEE>().Where(o => 
        //                        o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE 
        //                        && o.IS_DOCTOR == 1
        //                        && o.DEPARTMENT_ID == this.currentHisSpecialistExam.EXAM_EXECUTE_DEPARMENT_ID
        //                        ).ToList();
        //        List<ColumnInfo> columnInfos = new List<ColumnInfo>();
        //        columnInfos.Add(new ColumnInfo("LOGINNAME", "Tên đăng nhập", 150, 1));
        //        columnInfos.Add(new ColumnInfo("TDL_USERNAME", "Họ và tên", 250, 1));
        //        ControlEditorADO controlEditorADO = new ControlEditorADO("TDL_USERNAME", "LOGINNAME", columnInfos, false, 400);
        //        ControlEditorLoader.Load(cboEmployee, data, controlEditorADO);
        //        cboEmployee.Properties.ImmediatePopup = true;
        //        cboEmployee.Properties.PopupFormMinSize = new Size(400, cboEmployee.Properties.PopupFormMinSize.Height);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void LoadDataSereServByTreatmentId(V_HIS_SPECIALIST_EXAM currentHisServiceReq)
        {
            try
            {
                WaitingManager.Show();
                foreach (XtraTabPage item in this.xtraTabControl1.TabPages)
                {
                    item.PageVisible = false;
                }
                List<SereServADO> SereServADOs = new List<SereServADO>();
                List<DHisSereServ2> dataNew = new List<DHisSereServ2>();
                List<HIS_SERVICE_REQ> dataServiceReq = new List<HIS_SERVICE_REQ>();
                if (currentHisServiceReq != null && currentHisServiceReq.TREATMENT_ID > 0)
                {
                    CommonParam param = new CommonParam();
                    DHisSereServ2Filter _sereServ2Filter = new DHisSereServ2Filter();
                    _sereServ2Filter.TREATMENT_ID = currentHisServiceReq.TREATMENT_ID;
                    //_sereServ2Filter.INTRUCTION_DATE = Int64.Parse(currentHisServiceReq.InstructionDate.ToString().Substring(0, 8) + "000000");
                    dataNew = new BackendAdapter(param).Get<List<DHisSereServ2>>("api/HisSereServ/GetDHisSereServ2", ApiConsumers.MosConsumer, _sereServ2Filter, param);
                    if (dataNew != null && dataNew.Count > 0)
                    {
                        HisServiceReqFilter filter = new HisServiceReqFilter();
                        filter.IDs = dataNew.Select(o => o.SERVICE_REQ_ID ?? 0).ToList();
                        dataServiceReq = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, param);
                        var listRootByType = dataNew.OrderByDescending(o => o.TRACKING_TIME).GroupBy(o => o.TDL_SERVICE_TYPE_ID).ToList();
                        foreach (var types in listRootByType)
                        {
                            SereServADO ssRootType = new SereServADO();
                            #region Parent
                            ssRootType.CONCRETE_ID__IN_SETY = types.First().TDL_SERVICE_TYPE_ID + "";
                            var serviceType = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(p => p.ID == types.First().TDL_SERVICE_TYPE_ID);
                            long idSerReqType = 0;
                            long idDepartment = 0;
                            long idExecuteDepartment = 0;
                            short? IsTemporaryPres = 0;
                            if (dataServiceReq != null && dataServiceReq.Count > 0)
                            {
                                if (dataServiceReq.Where(o => o.ID == types.First().SERVICE_REQ_ID) != null && dataServiceReq.Where(o => o.ID == types.First().SERVICE_REQ_ID).ToList().Count > 0)
                                {
                                    idSerReqType = dataServiceReq.Where(o => o.ID == types.First().SERVICE_REQ_ID).FirstOrDefault().SERVICE_REQ_TYPE_ID;
                                    idDepartment = dataServiceReq.Where(o => o.ID == types.First().SERVICE_REQ_ID).FirstOrDefault().REQUEST_DEPARTMENT_ID;
                                    idExecuteDepartment = dataServiceReq.Where(o => o.ID == types.First().SERVICE_REQ_ID).FirstOrDefault().EXECUTE_DEPARTMENT_ID;
                                    IsTemporaryPres = dataServiceReq.Where(o => o.ID == types.First().SERVICE_REQ_ID).FirstOrDefault().IS_TEMPORARY_PRES;
                                }
                            }
                            ssRootType.TRACKING_TIME = types.First().TRACKING_TIME;
                            ssRootType.TDL_SERVICE_TYPE_ID = types.First().TDL_SERVICE_TYPE_ID;
                            ssRootType.SERVICE_CODE = serviceType != null ? serviceType.SERVICE_TYPE_NAME : null;
                            #endregion
                            SereServADOs.Add(ssRootType);
                            var listRootSety = types.GroupBy(g => g.SERVICE_REQ_ID).ToList();
                            foreach (var rootSety in listRootSety)
                            {
                                #region Child
                                SereServADO ssRootSety = new SereServADO();
                                ssRootSety.CONCRETE_ID__IN_SETY = ssRootType.CONCRETE_ID__IN_SETY + "_" + rootSety.First().SERVICE_REQ_ID;
                                //qtcode
                                if (rootSety.First().USE_TIME.HasValue)
                                {
                                    ssRootSety.REQUEST_DEPARTMENT_NAME = string.Format("Dự trù: {0}", Inventec.Common.DateTime.Convert.TimeNumberToDateString(rootSety.First().USE_TIME.Value));
                                }
                                //qtcode
                                ssRootSety.PARENT_ID__IN_SETY = ssRootType.CONCRETE_ID__IN_SETY;
                                ssRootSety.REQUEST_DEPARTMENT_ID = idDepartment;
                                ssRootSety.EXECUTE_DEPARTMENT_ID = idExecuteDepartment;
                                ssRootSety.SERVICE_REQ_TYPE_ID = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().FirstOrDefault(p => p.ID == idSerReqType) != null ?
                                BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().FirstOrDefault(p => p.ID == idSerReqType).ID : 0;
                                ssRootSety.TRACKING_TIME = rootSety.First().TRACKING_TIME;
                                ssRootSety.SERVICE_REQ_ID = rootSety.First().SERVICE_REQ_ID;
                                ssRootSety.SERVICE_REQ_STT_ID = rootSety.First().SERVICE_REQ_STT_ID;
                                ssRootSety.TDL_SERVICE_TYPE_ID = rootSety.First().TDL_SERVICE_TYPE_ID;
                                ssRootSety.SERVICE_CODE = rootSety.First().SERVICE_REQ_CODE;
                                ssRootSety.SERVICE_REQ_CODE = rootSety.First().SERVICE_REQ_CODE;
                                ssRootSety.IS_TEMPORARY_PRES = IsTemporaryPres;
                                if (dataServiceReq != null && dataServiceReq.Count > 0)
                                {
                                    var serviceReq = dataServiceReq.FirstOrDefault(o => o.ID == rootSety.First().SERVICE_REQ_ID) ?? new HIS_SERVICE_REQ();
                                    ssRootSety.SAMPLE_TIME = serviceReq.SAMPLE_TIME;
                                    ssRootSety.RECEIVE_SAMPLE_TIME = serviceReq.RECEIVE_SAMPLE_TIME;
                                }
                                ssRootSety.TDL_TREATMENT_ID = rootSety.First().TDL_TREATMENT_ID;
                                ssRootSety.PRESCRIPTION_TYPE_ID = rootSety.First().PRESCRIPTION_TYPE_ID;
                                ssRootSety.REQUEST_LOGINNAME = rootSety.First().REQUEST_LOGINNAME;
                                ssRootSety.REQUEST_DEPARTMENT_ID = rootSety.First().REQUEST_DEPARTMENT_ID ?? 0;
                                ssRootSety.SERVICE_NAME = String.Format("- {0} - {1}", rootSety.First().REQUEST_ROOM_NAME, rootSety.First().REQUEST_DEPARTMENT_NAME);
                                var time = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rootSety.First().TDL_INTRUCTION_TIME ?? 0);
                                ssRootSety.NOTE_ADO = time.Substring(0, time.Count() - 3);
                                //if ((rootSety.First().REQUEST_LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() || CheckLoginAdmin.IsAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()))
                                //    && (rootSety.First().SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL || HisConfigs.Get<string>("MOS.HIS_SERVICE_REQ.ALLOW_MODIFYING_OF_STARTED") == "1" || (HisConfigs.Get<string>("MOS.HIS_SERVICE_REQ.ALLOW_MODIFYING_OF_STARTED") == "2"
                                //    && ssRootSety.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH))
                                //    && rootSety.First().IS_NO_EXECUTE != 1)
                                //{
                                //    ssRootSety.IsEnableEdit = true;
                                //}
                                //if ((rootSety.First().REQUEST_LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() || CheckLoginAdmin.IsAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName())
                                //  || (rootSety.First().REQUEST_DEPARTMENT_ID == departmentId && ssRootSety.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH))
                                //  && rootSety.First().SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                                //{
                                //    ssRootSety.IsEnableDelete = true;
                                //}


                                SereServADOs.Add(ssRootSety);
                                #endregion
                                int d = 0;
                                foreach (var item in rootSety)
                                {
                                    d++;
                                    #region Child (+n)
                                    SereServADO ado = new SereServADO(item);
                                    ado.CONCRETE_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY + "_" + d;
                                    ado.PARENT_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY;
                                    if (!String.IsNullOrWhiteSpace(item.TUTORIAL))
                                    {
                                        ado.NOTE_ADO = string.Format("{0}. {1}", item.TUTORIAL, item.INSTRUCTION_NOTE);

                                    }
                                    else
                                    {
                                        ado.NOTE_ADO = string.Format("{0}", item.INSTRUCTION_NOTE);
                                    }
                                    ado.AMOUNT_SER = string.Format("{0} - {1}", item.AMOUNT, item.SERVICE_UNIT_NAME);
                                    ado.IS_TEMPORARY_PRES = IsTemporaryPres;
                                    SereServADOs.Add(ado);
                                    #endregion
                                }
                            }
                        }
                    }
                }
                if (SereServADOs != null && SereServADOs.Count > 0)
                {

                    SereServADOs = SereServADOs.OrderBy(o => o.PARENT_ID__IN_SETY).ThenBy(p => p.SERVICE_CODE).ThenBy(o => o.SERVICE_NAME).ToList();
                    try
                    {
                        CommonParam paramCommon = new CommonParam();
                        MOS.Filter.HisTrackingViewFilter trackingFilter = new MOS.Filter.HisTrackingViewFilter();
                        trackingFilter.TREATMENT_ID = currentHisServiceReq.TREATMENT_ID;
                        trackingFilter.ORDER_FIELD = "TRACKING_TIME";
                        trackingFilter.ORDER_DIRECTION = "DESC";
                        var resultTracking = new BackendAdapter(paramCommon).Get<List<HIS_TRACKING>>(HisRequestUriStore.HIS_TRACKING_GET, ApiConsumers.MosConsumer, trackingFilter, paramCommon);
                        if (resultTracking != null)
                        {
                            //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataNew), dataNew));
                            var Employees = BackendDataWorker.Get<V_HIS_EMPLOYEE>()/*.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)*/.ToList();
                            List<TrackingListADO> listTracking = (from a in resultTracking
                                                                  join b in dataNew on a.ID equals b.TRACKING_ID
                                                                  into AB
                                                                  from ab in AB.DefaultIfEmpty()
                                                                  join c in Employees on a.CREATOR equals c.LOGINNAME
                                                                  into AC
                                                                  from ac in AC.DefaultIfEmpty()
                                                                  select new
                                                                  {
                                                                      TRACKING_ID = a.ID,
                                                                      TRACKING_TIME = a.TRACKING_TIME,
                                                                      USER_NAME = ac?.TDL_USERNAME,
                                                                      DIPLOMA = ac?.DIPLOMA,
                                                                      CONTENT = a.CONTENT,
                                                                      SERVICE_NAME = ab?.SERVICE_NAME,
                                                                      SERVICE_REQ_CODE = ab?.SERVICE_REQ_CODE,
                                                                      AMOUNT = ab?.AMOUNT,
                                                                      SERVICE_UNIT_NAME = ab?.SERVICE_UNIT_NAME,
                                                                      ICD_CODE = a.ICD_CODE,
                                                                      ICD_NAME = a.ICD_NAME,
                                                                      ICD_SUB_CODE = a.ICD_SUB_CODE,
                                                                      ICD_TEXT = a.ICD_TEXT,
                                                                   })
                                                                  .GroupBy(g => g.TRACKING_ID)
                                                                  .Select((s, i) =>
                                                                  {
                                                                      var ret = s.First();
                                                                      return new TrackingListADO()
                                                                      {
                                                                          CONCRETE_ID__IN_SETY = (i + 1).ToString(),
                                                                          TRACKING_TIME = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(ret.TRACKING_TIME)
                                                                                        .Replace(" ", Environment.NewLine),
                                                                          USER_NAME = ret.USER_NAME + " - " + ret.DIPLOMA,
                                                                          CONTENT = ret.CONTENT,
                                                                          ICD_CODE = ret.ICD_CODE,
                                                                          ICD_NAME = ret.ICD_NAME,
                                                                          ICD_SUB_CODE = ret.ICD_SUB_CODE,
                                                                          ICD_TEXT = ret.ICD_TEXT,
                                                                          SERVICE = string.Join(Environment.NewLine, s.Where(w => !string.IsNullOrEmpty(w.SERVICE_NAME))
                                                                          .Select(ss => ss.SERVICE_REQ_CODE + " - " + ss.SERVICE_NAME + " x " + ss.AMOUNT + " " + ss.SERVICE_UNIT_NAME))
                                                                      };
                                                                  })
                                                                  .ToList();
                            Inventec.Common.Logging.LogSystem.Info("listTracking12: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listTracking), listTracking));
                            tabToDieuTri.PageVisible = true;
                            ucAll.ReLoad(treeView_Click, listTracking, currentHisSpecialistExam);
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }

                    // Tab Chẩn đoán hình ảnh: Dữ liệu lấy từ D_HIS_SERE_SERV_2 có TDL_SERVICE_TYPE_ID = 3
                    List<SereServADO> listCDHA = new List<SereServADO>();
                    listCDHA.AddRange(SereServADOs.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA));
                    ucCDHA.ReLoad(treeView_Click, listCDHA, this.currentHisSpecialistExam, Edit_Click, Delete_Click);
                    if (listCDHA.Any())
                    {
                        tabCDHA.PageVisible = true;
                        ucCDHA.tc_Number.Visible = false;
                        ucCDHA.tc_TdlMedicineConcentra.Visible = false;
                        ucCDHA.tc_RequestDepartmentName.Visible = false;
                    }
                    // Tab Xét nghiệm: Dữ liệu lấy từ D_HIS_SERE_SERV_2 có TDL_SERVICE_TYPE_ID = 2
                    List<SereServADO> listXetNghiem = new List<SereServADO>();
                    listXetNghiem.AddRange(SereServADOs.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN));
                    if (listXetNghiem.Any())
                    {
                        tabXetNghiem.PageVisible = true;
                        ucXetNghiem.tc_Number.Visible = false;
                        ucXetNghiem.tc_TdlMedicineConcentra.Visible = false;
                        ucXetNghiem.tc_RequestDepartmentName.Visible = false;
                    }
                    ucXetNghiem.ReLoad(treeView_Click, listXetNghiem, this.currentHisSpecialistExam, Edit_Click, Delete_Click);
                    // Tab thuốc/vt/máu: Dữ liệu lấy từ D_HIS_SERE_SERV_2 có TDL_SERVICE_TYPE_ID = 6,7, 14
                    List<SereServADO> listMediMate = new List<SereServADO>();
                    listMediMate.AddRange(SereServADOs.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU
                        ));
                    if (listMediMate.Any())
                    {
                        tabThuocVatTuMau.PageVisible = true;
                    }
                    ucThuocVatTu.ReLoad(treeView_Click, listMediMate, this.currentHisSpecialistExam, Edit_Click, Delete_Click);
                    //Tab siêu âm, nội soi: Dữ liệu lấy từ D_HIS_SERE_SERV_2 có TDL_SERVICE_TYPE_ID = 9,10 
                    List<SereServADO> listSieuAm = new List<SereServADO>();
                    listSieuAm.AddRange(SereServADOs.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA
                    || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS));
                    if (listSieuAm.Any())
                    {
                        tabSieuAmNoiSoi.PageVisible = true;
                        ucSieuAm.tc_Number.Visible = false;
                        ucSieuAm.tc_TdlMedicineConcentra.Visible = false;
                        ucSieuAm.tc_RequestDepartmentName.Visible = false;
                    }
                    ucSieuAm.ReLoad(treeView_Click, listSieuAm, this.currentHisSpecialistExam, Edit_Click, Delete_Click);
                    //  Tab phẫn thuật thủ thật: Dữ liệu lấy từ D_HIS_SERE_SERV_2 có TDL_SERVICE_TYPE_ID = 11 ,4
                    List<SereServADO> listPTTT = new List<SereServADO>();
                    listPTTT.AddRange(SereServADOs.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT
                    || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT));
                    if (listPTTT.Any())
                    {
                        tabPhauThuatThuThuat.PageVisible = true;
                        ucPTTT.tc_Number.Visible = false;
                        ucPTTT.tc_TdlMedicineConcentra.Visible = false;
                        ucPTTT.tc_RequestDepartmentName.Visible = false;
                    }
                    ucPTTT.ReLoad(treeView_Click, listPTTT, this.currentHisSpecialistExam, Edit_Click, Delete_Click);
                    //Tab giải phẫu bệnh: Dữ liệu lấy từ D_HIS_SERE_SERV_2 có TDL_SERVICE_TYPE_ID = 15
                    List<SereServADO> listGPT = new List<SereServADO>();
                    listGPT.AddRange(SereServADOs.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL));
                    if (listGPT.Any())
                    {
                        tabGiaiPhauBenh.PageVisible = true;
                        ucGPB.tc_Number.Visible = false;
                        ucGPB.tc_TdlMedicineConcentra.Visible = false;
                        ucGPB.tc_RequestDepartmentName.Visible = false;
                    }
                    ucGPB.ReLoad(treeView_Click, listGPT, this.currentHisSpecialistExam, Edit_Click, Delete_Click);
                    //
                    foreach (XtraTabPage tab in xtraTabControl1.TabPages.Where(w => w.PageVisible))
                    {
                        xtraTabControl1.SelectedTabPage = tab;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                WaitingManager.Hide();
            }
        }

        private void Edit_Click(SereServADO currentSS)
        {

        }

        private void Delete_Click(SereServADO data)
        {

        }
        DHisSereServ2 TreeClickData;

        private void treeView_Click(SereServADO data)
        {
            try
            {
                if (data != null)
                {
                    TreeClickData = data;
                    if (TreeClickData != null && !String.IsNullOrWhiteSpace(TreeClickData.SERVICE_REQ_CODE))
                    {
                        ProcessLoadDocumentBySereServ(TreeClickData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ProcessLoadDocumentBySereServ(DHisSereServ2 data)
        {

        }
        int positionHandleControl = -1;

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                SetMaxlength(txtYKienBacSi, 4000, false);
                ValidContent();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetMaxlength(BaseEdit control, int maxlenght, bool IsRequired)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxlenght;
                validate.IsRequired = IsRequired;
                validate.ErrorText = string.Format(ResourceMessage.NhapQuaMaxlength, maxlenght);
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidContent()
        {
            var spin = new LookupEditWithTextEditValidationRule();
            spin.editor = cboEmployee;
            spin.GetSelectedEmployees = () => this.EmployeeSelecteds; 
            this.dxValidationProvider1.SetValidationRule(cboEmployee, spin);

            ValidateNull controlEditNull = new ValidateNull();
            controlEditNull.textEdit = txtDienBien;
            controlEditNull.ErrorType = ErrorType.Warning;
            this.dxValidationProvider1.SetValidationRule(txtDienBien, controlEditNull);

            ValidateMaxLength controlEditMax = new ValidateMaxLength();
            controlEditMax.textEdit = txtPPXuLy;
            controlEditMax.maxLength = 4000;
            controlEditMax.ErrorType = ErrorType.Warning;
            this.dxValidationProvider1.SetValidationRule(txtPPXuLy, controlEditMax);
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave.PerformClick();
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
                if (!dxValidationProvider1.Validate())
                {
                    Inventec.Common.Logging.LogSystem.Info("dxValidationProvider1.Validate");
                    return;
                }

                positionHandleControl = -1;
                CommonParam param = new CommonParam();
                HIS_SPECIALIST_EXAM datamapper = new HIS_SPECIALIST_EXAM();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SPECIALIST_EXAM>(datamapper, currentHisSpecialistExam);
                //datamapper.EXAM_EXECUTE_LOGINNAME = cboEmployee.EditValue != null ? cboEmployee.EditValue.ToString() : null;
                //datamapper.EXAM_EXECUTE_USERNAME = cboEmployee.EditValue != null ? cboEmployee.Text.ToString() : null;
                if (this.EmployeeSelecteds != null && this.EmployeeSelecteds.Count > 0)
                {
                    datamapper.EXAM_EXECUTE_LOGINNAME = string.Join(", ",this.EmployeeSelecteds.Select(o => o.LOGINNAME.ToString()).ToList());
                    datamapper.EXAM_EXECUTE_USERNAME = string.Join(", ", this.EmployeeSelecteds.Select(o => o.TDL_USERNAME.ToString()).ToList());
                }
                datamapper.EXAM_EXECUTE_CONTENT = txtYKienBacSi.Text.Trim();
                datamapper.CONTENT = txtDienBien.Text.Trim();
                datamapper.MEDICAL_INSTRUCTION = txtPPXuLy.Text.Trim();
                datamapper.REJECT_APPROVAL_REASON = null;
                datamapper.IS_APPROVAL = 1;
                datamapper.ICD_CODE = cboICD_YHCT.EditValue != null? cboICD_YHCT.EditValue.ToString(): null;
                datamapper.ICD_NAME = cboICD_YHCT.Text;
                datamapper.ICD_SUB_CODE= txtICDsub.Text.Trim();
                datamapper.ICD_TEXT = txtICDsubName.Text.Trim();
                //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => datamapper), datamapper));
                var rs = new BackendAdapter(param).Post<HIS_SPECIALIST_EXAM>("api/HisSpecialistExam/Update", ApiConsumers.MosConsumer, datamapper, param);
                if (rs != null && this.delegateRefresh != null)
                {
                    currentHisSpecialistExam.EXAM_EXECUTE_LOGINNAME = datamapper.EXAM_EXECUTE_LOGINNAME;
                    currentHisSpecialistExam.EXAM_EXECUTE_USERNAME = datamapper.EXAM_EXECUTE_USERNAME;
                    currentHisSpecialistExam.EXAM_EXECUTE_CONTENT = datamapper.EXAM_EXECUTE_CONTENT;
                    currentHisSpecialistExam.IS_APPROVAL = datamapper.IS_APPROVAL;
                    currentHisSpecialistExam.REJECT_APPROVAL_REASON = datamapper.REJECT_APPROVAL_REASON;
                    currentHisSpecialistExam.ICD_CODE = datamapper.ICD_CODE;
                    currentHisSpecialistExam.ICD_NAME = datamapper.ICD_NAME;
                    currentHisSpecialistExam.ICD_SUB_CODE = datamapper.ICD_SUB_CODE;
                    currentHisSpecialistExam.ICD_TEXT = datamapper.ICD_TEXT;
                    currentHisSpecialistExam.CONTENT = datamapper.CONTENT;
                    currentHisSpecialistExam.MEDICAL_INSTRUCTION = datamapper.MEDICAL_INSTRUCTION;
                    this.delegateRefresh();
                }
                MessageManager.Show(this, param, rs != null);
                SessionManager.ProcessTokenLost(param);
                if (rs != null)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        List<HIS_EMPLOYEE> EmployeeSelecteds;
        List<HIS_EMPLOYEE> EmployeesDataSource;
        private void InitComboEmployee()
        {
            this.EmployeesDataSource = BackendDataWorker.Get<HIS_EMPLOYEE>().Where(o =>
                                o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                                && o.IS_DOCTOR == 1
                                && o.DEPARTMENT_ID == this.currentHisSpecialistExam.EXAM_EXECUTE_DEPARMENT_ID
                                ).ToList();
            this.InitCombo(cboEmployee,
                EmployeesDataSource,
                 "TDL_USERNAME",
                 "LOGINNAME",
                cboEmployee_MarksSelection,
                cboEmployee_CustomDisplayText
                );
        }
        private void InitCombo(
            GridLookUpEdit cbo,
            object data,
            string displayMember,
            string valueMember,
            GridCheckMarksSelection.SelectionChangedEventHandler eventHandlerMarksSelection,
            DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler eventHandlerCustomDisplayText
            )
        {
            try
            {
                // Marks selection
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventHandlerMarksSelection);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                //
                cbo.Properties.View.ColumnFilterChanged += (s, e) =>
                {
                    var view = s as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (view == null) return;

                    // Lấy filter text của cột đầu tiên có filter (hoặc tuỳ ý)
                    string filterText = null;
                    foreach (var col in view.Columns)
                    {
                        var column = col as DevExpress.XtraGrid.Columns.GridColumn;
                        if (column != null && !string.IsNullOrEmpty(column.FilterInfo?.Value as string))
                        {
                            filterText = column.FilterInfo.Value as string;
                            break;
                        }
                    }
                    // Gán filterText cho FindPanelText để highlight
                    view.ApplyFindFilter(!string.IsNullOrEmpty(filterText) ? $"\"{filterText}\"" : string.Empty);
                };
                // Combo properties
                cbo.Properties.Closed += (s, e) =>
                {
                    GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                    cbo.Properties.Buttons[1].Visible = gridCheckMark != null && gridCheckMark.Selection.Count > 0;
                    var view = cbo.Properties.View;
                    if (view != null)
                    {
                        view.ClearColumnsFilter();
                        view.ApplyFindFilter(string.Empty);
                    }
                };
                cbo.Properties.View.CustomDrawCell += View_CustomDrawCell_ShowPlaceholder;
                cbo.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(eventHandlerCustomDisplayText);
                cbo.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboProperties_ButtonClick);
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = displayMember;
                cbo.Properties.ValueMember = valueMember;
                if (cbo.Properties.View.Columns.Count > 0)
                {
                    var checkCol = cbo.Properties.View.Columns[0];
                    checkCol.Width = 30;
                    checkCol.MinWidth = 30;
                    checkCol.MaxWidth = 30;
                    checkCol.OptionsColumn.FixedWidth = true;
                }
                DevExpress.XtraGrid.Columns.GridColumn column1 = cbo.Properties.View.Columns.AddField(valueMember);
                column1.VisibleIndex = 1;
                column1.Width = 100;
                column1.Caption = "Mã";

                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(displayMember);
                col2.VisibleIndex = 2;
                col2.Width = 325;
                col2.Caption = "Tên";
                col2.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
                cbo.Properties.PopupFormWidth = 350;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                //cbo.Properties.View.OptionsView.ShowAutoFilterRow = true;
                cbo.Properties.View.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
                cbo.Properties.View.BestFitColumns();
                // Clear selection
                this.cboClearSelection(cbo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void View_CustomDrawCell_ShowPlaceholder(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (view == null) return;
            if (e.RowHandle == DevExpress.XtraGrid.GridControl.AutoFilterRowHandle)
            {
                var filterValue = view.GetRowCellValue(e.RowHandle, e.Column);
                if (filterValue == null || string.IsNullOrEmpty(filterValue.ToString()))
                {
                    e.DisplayText = "Từ khóa tìm kiếm ...";
                    e.Appearance.ForeColor = System.Drawing.Color.Gray;
                }
            }
        }

        private void cboClearSelection(GridLookUpEdit gridLookUpEdit)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = gridLookUpEdit.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(gridLookUpEdit.Properties.View);
                }
                if (gridLookUpEdit.Properties.Buttons.Count > 0)
                {
                    foreach (EditorButton item in gridLookUpEdit.Properties.Buttons)
                    {
                        if (item != null && item.Kind == ButtonPredefines.Delete)
                        {
                            item.Visible = false;
                        }
                    }
                }
                gridLookUpEdit.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProperties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    var cbo = sender as DevExpress.XtraEditors.GridLookUpEdit;
                    this.cboClearSelection(cbo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void cboEmployee_MarksSelection(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_EMPLOYEE> sgSelectedNews = new List<HIS_EMPLOYEE>();
                    foreach (HIS_EMPLOYEE rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.LOGINNAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this.EmployeeSelecteds = new List<HIS_EMPLOYEE>();
                    this.EmployeeSelecteds.AddRange(sgSelectedNews);
                    this.cboEmployee.Text = sb.ToString();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void cboEmployee_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null || gridCheckMark.Selection == null || gridCheckMark.Selection.Count == 0)
                {
                    e.DisplayText = "";
                    return;
                }
                foreach (HIS_EMPLOYEE rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }

                    sb.Append(rv.TDL_USERNAME.ToString());
                    if (sb.ToString().Length > 100)
                    {
                        break;
                    }
                }
                //if (EmployeeSelecteds != null && EmployeeSelecteds.Count == this.EmployeesDataSource.Count)
                //{
                //    sb = new StringBuilder("Tất cả");
                //}
                string text = sb.ToString();
                if (text.Length > 100)
                    text = text.Substring(0, 100) + "...";
                e.DisplayText = text;
                var g = sender as DevExpress.XtraEditors.GridLookUpEdit;
                g.Text = e.DisplayText;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ProcessSelectEmployee()
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cboEmployee.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboEmployee.Properties.View);
                }
                if (cboEmployee.Properties.Tag != null)
                {
                    List<HIS_EMPLOYEE> ds = cboEmployee.Properties.DataSource as List<HIS_EMPLOYEE>;
                    List<HIS_EMPLOYEE> selects = new List<HIS_EMPLOYEE>();
                    
                    foreach (HIS_EMPLOYEE item in ds.Where(w => this.currentHisSpecialistExam.EXAM_EXECUTE_LOGINNAME.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Any(a => a.ToLower() == w.LOGINNAME.ToLower())))
                    {
                        selects.Add(item);
                    }
                    gridCheckMark.SelectAll(selects);
                }
                if (gridCheckMark != null && cboEmployee.Properties.Buttons.Count > 1)
                {
                    cboEmployee.Properties.Buttons[1].Visible = gridCheckMark.Selection.Count > 0;
                }
                else
                {
                    this.cboClearSelection(cboEmployee);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        //icd -------------------
        private void InitComboICD_YHCT()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == 1 && o.IS_TRADITIONAL != 1).ToList();

                var cbo = this.cboICD_YHCT;

                cboICD_YHCT.ProcessNewValue -= cboICD_YHCT_ProcessNewValue;
                cboICD_YHCT.ProcessNewValue += cboICD_YHCT_ProcessNewValue;
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = nameof(HIS_ICD.ICD_NAME);
                cbo.Properties.ValueMember = nameof(HIS_ICD.ICD_CODE);
                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.Properties.View.OptionsView.RowAutoHeight = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();
                cbo.Properties.PopupFormSize = new System.Drawing.Size(400, 250);

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cbo.Properties.View.Columns.AddField(nameof(HIS_ICD.ICD_CODE));
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 70;
                aColumnCode.ColumnEdit = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cbo.Properties.View.Columns.AddField(nameof(HIS_ICD.ICD_NAME));
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 300;
                aColumnName.ColumnEdit = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
                cbo.Properties.View.OptionsView.ColumnAutoWidth = true;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
       
        private void cboICD_YHCT_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboICD_YHCT.Properties.Buttons[1].Visible = cboICD_YHCT.EditValue != null;
                if (cboICD_YHCT.EditValue != null)
                {
                    txtICD_YHCT.Text = cboICD_YHCT.EditValue.ToString();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboICD_YHCT_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    if (!cboICD_YHCT.Properties.Buttons[1].Visible)
                        return;
                    cboICD_YHCT.EditValue = null;
                    cboICD_YHCT.Properties.NullText = "";
                    cboICD_YHCT.Text = "";
                    txtICD_YHCT.Text = "";

                    cboICD_YHCT.DoValidate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void cboICD_YHCT_ProcessNewValue(object sender, DevExpress.XtraEditors.Controls.ProcessNewValueEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.DisplayValue as string))
            {
                e.Handled = true;
            }
        }

        private void txtICDsubName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    WaitingManager.Show();
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SecondaryIcd").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.SecondaryIcd'");
                    if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.SecondaryIcd' is not plugins");
                    HIS.Desktop.ADO.SecondaryIcdADO secondaryIcdADO = new HIS.Desktop.ADO.SecondaryIcdADO(GetStringIcds, txtICDsub.Text, txtICDsubName.Text);
                    List<object> listArgs = new List<object>();
                    listArgs.Add(secondaryIcdADO);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null"); WaitingManager.Hide();
                    ((Form)extenceInstance).Show(this);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtICD_YHCT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    string code = txtICD_YHCT.Text.Trim();
                    if (!string.IsNullOrEmpty(code))
                    {
                        var listData = BackendDataWorker.Get<HIS_ICD>()
                            .Where(o => o.IS_ACTIVE == 1 && o.IS_TRADITIONAL != 1 &&
                                        (o.ICD_CODE.IndexOf(code, StringComparison.OrdinalIgnoreCase) >= 0)).ToList();

                        var result = listData != null
                            ? (listData.Count > 1
                                ? listData.Where(o => o.ICD_CODE.Equals(code.Trim(), StringComparison.OrdinalIgnoreCase)).ToList()
                                : listData)
                            : null;

                        if (result != null && result.Count > 0)
                        {
                            showCbo = false;
                            var item = result.First();

                            txtICD_YHCT.Text = item.ICD_CODE;
                            cboICD_YHCT.EditValue = item.ICD_CODE;

                            SendKeys.Send("{TAB}");
                        }
                        else
                        {
                            cboICD_YHCT.EditValue = null;
                        }
                    }


                    // Nếu không tìm được thì show popup cho user chọn
                    if (showCbo)
                    {
                        cboICD_YHCT.Focus();
                        cboICD_YHCT.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //ICDsub---------------------------------

        private void GetStringIcds(string delegateIcdCodes, string delegateIcdNames)
        {
            try
            {
                if (!string.IsNullOrEmpty(delegateIcdNames))
                {
                    txtICDsubName.Text = delegateIcdNames;
                }
                if (!string.IsNullOrEmpty(delegateIcdCodes))
                {
                    txtICDsub.Text = delegateIcdCodes;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void txtCdPhu_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string seperate = ";";
                    string strIcdNames = "";
                    string strWrongIcdCodes = "";
                    string[] periodSeparators = new string[1];
                    periodSeparators[0] = seperate;
                    
                    string[] arrIcdExtraCodes = txtICDsub.Text.Split(periodSeparators, StringSplitOptions.RemoveEmptyEntries);
                    if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
                    {
                        var icdAlls = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>().Where(o => o.IS_ACTIVE == 1 && o.IS_TRADITIONAL != 1).ToList();
                        foreach (var itemCode in arrIcdExtraCodes)
                        {
                            var icdByCode = icdAlls.FirstOrDefault(o => o.ICD_CODE.ToLower() == itemCode.ToLower());
                            if (icdByCode != null && icdByCode.ID > 0)
                            {
                                strIcdNames += (seperate + icdByCode.ICD_NAME);
                            }
                            else
                            {
                                strWrongIcdCodes += (seperate + itemCode);
                            }
                        }
                        strIcdNames += seperate;
                        if (!String.IsNullOrEmpty(strWrongIcdCodes))
                        {
                            MessageManager.Show(String.Format("Không tìm thấy icd tương ứng với các mã sau: {0}", strWrongIcdCodes));
                        }
                    }
                    txtICDsubName.Text = strIcdNames;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void cboCdPhu_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                string seperate = ";";
                string strIcdNames = "";
                string strWrongIcdCodes = "";
                string[] periodSeparators = new string[1];
                periodSeparators[0] = seperate;
                string[] arrIcdExtraCodes = txtICDsub.Text.Split(periodSeparators, StringSplitOptions.RemoveEmptyEntries);
                if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
                {
                    var icdAlls = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>().Where(o => o.IS_ACTIVE == 1 && o.IS_TRADITIONAL != 1).ToList();
                    foreach (var itemCode in arrIcdExtraCodes)
                    {
                        var icdByCode = icdAlls.FirstOrDefault(o => o.ICD_CODE.ToLower() == itemCode.ToLower());
                        if (icdByCode != null && icdByCode.ID > 0)
                        {
                            strIcdNames += (seperate + icdByCode.ICD_NAME);
                        }
                        else
                        {
                            strWrongIcdCodes += (seperate + itemCode);
                        }
                    }
                    strIcdNames += seperate;
                    if (!String.IsNullOrEmpty(strWrongIcdCodes))
                    {
                        MessageManager.Show(String.Format("Không tìm thấy icd tương ứng với các mã sau: {0}", strWrongIcdCodes));
                    }
                }
                txtICDsubName.Text = strIcdNames;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

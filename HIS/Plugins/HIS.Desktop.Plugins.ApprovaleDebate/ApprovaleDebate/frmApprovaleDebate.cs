using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
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
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Plugins.ApprovaleDebate.ADO;
using MOS.SDO;
using Inventec.Core;
using MOS.Filter;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LocalStorage.HisConfig;
using EMR.SDO;
using HIS.Desktop.ADO;
using EMR.EFMODEL.DataModels;
using HIS.Desktop.Controls.Session;
using DevExpress.XtraTab;
using DevExpress.XtraEditors;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.Plugins.ApprovaleDebate.Resources;
using DevExpress.XtraEditors.ViewInfo;

namespace HIS.Desktop.Plugins.ApprovaleDebate.ApprovaleDebate
{
    public partial class frmApprovaleDebate : FormBase
    {
        private Common.RefeshReference delegateRefresh;
        internal Inventec.Desktop.Common.Modules.Module currentModule { get; set; }
        V_HIS_SPECIALIST_EXAM v_his_specialist_exam;
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
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSave.Caption = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.bbtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabToDieuTri.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabToDieuTri.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabCDHA.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabCDHA.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabXetNghiem.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabXetNghiem.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabThuocVatTuMau.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabThuocVatTuMau.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabSieuAmNoiSoi.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabSieuAmNoiSoi.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabPhauThuatThuThuat.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabPhauThuatThuThuat.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabGiaiPhauBenh.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabGiaiPhauBenh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                this.v_his_specialist_exam = specialist;
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
                this.ValidControl();
                if (this.v_his_specialist_exam != null)
                {
                    this.txtYKienBacSi.Text = this.v_his_specialist_exam.EXAM_EXECUTE_CONTENT;
                    this.cboEmployee.EditValue = this.v_his_specialist_exam.EXAM_EXECUTE_LOGINNAME;
                    //
                    this.LoadDataSereServByTreatmentId(this.v_his_specialist_exam);
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

        private void InitComboEmployee()
        {
            try
            {
                var data = BackendDataWorker.Get<V_HIS_EMPLOYEE>().Where(o => 
                                o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE 
                                && o.IS_DOCTOR == 1
                                && o.DEPARTMENT_ID == this.v_his_specialist_exam.EXAM_EXECUTE_DEPARMENT_ID
                                ).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "Tên đăng nhập", 150, 1));
                columnInfos.Add(new ColumnInfo("TDL_USERNAME", "Họ và tên", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TDL_USERNAME", "LOGINNAME", columnInfos, false, 400);
                ControlEditorLoader.Load(cboEmployee, data, controlEditorADO);
                cboEmployee.Properties.ImmediatePopup = true;
                cboEmployee.Properties.PopupFormMinSize = new Size(400, cboEmployee.Properties.PopupFormMinSize.Height);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

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
                                                                          SERVICE = string.Join(Environment.NewLine, s.Where(w => !string.IsNullOrEmpty(w.SERVICE_NAME))
                                                                          .Select(ss => ss.SERVICE_REQ_CODE + " - " + ss.SERVICE_NAME + " x " + ss.AMOUNT + " " + ss.SERVICE_UNIT_NAME))
                                                                      };
                                                                  })
                                                                  .ToList();
                            tabToDieuTri.PageVisible = true;
                            ucAll.ReLoad(treeView_Click, listTracking, v_his_specialist_exam);
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }

                    //
                    List<SereServADO> listCDHA = new List<SereServADO>();
                    listCDHA.AddRange(SereServADOs.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA));
                    ucCDHA.ReLoad(treeView_Click, listCDHA, this.v_his_specialist_exam, Edit_Click, Delete_Click);
                    if (listCDHA.Any())
                    {
                        tabCDHA.PageVisible = true;
                        ucCDHA.tc_Number.Visible = false;
                        ucCDHA.tc_TdlMedicineConcentra.Visible = false;
                        ucCDHA.tc_RequestDepartmentName.Visible = false;
                    }
                    //
                    List<SereServADO> listXetNghiem = new List<SereServADO>();
                    listXetNghiem.AddRange(SereServADOs.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN));
                    if (listXetNghiem.Any())
                    {
                        tabXetNghiem.PageVisible = true;
                        ucXetNghiem.tc_Number.Visible = false;
                        ucXetNghiem.tc_TdlMedicineConcentra.Visible = false;
                        ucXetNghiem.tc_RequestDepartmentName.Visible = false;
                    }
                    ucXetNghiem.ReLoad(treeView_Click, listXetNghiem, this.v_his_specialist_exam, Edit_Click, Delete_Click);
                    //
                    List<SereServADO> listMediMate = new List<SereServADO>();
                    listMediMate.AddRange(SereServADOs.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU
                        ));
                    if (listMediMate.Any())
                    {
                        tabThuocVatTuMau.PageVisible = true;
                    }
                    ucThuocVatTu.ReLoad(treeView_Click, listMediMate, this.v_his_specialist_exam, Edit_Click, Delete_Click);
                    //
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
                    ucSieuAm.ReLoad(treeView_Click, listSieuAm, this.v_his_specialist_exam, Edit_Click, Delete_Click);
                    //
                    List<SereServADO> listPTTT = new List<SereServADO>();
                    listPTTT.AddRange(SereServADOs.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH
                    || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN));
                    if (listPTTT.Any())
                    {
                        tabPhauThuatThuThuat.PageVisible = true;
                        ucPTTT.tc_Number.Visible = false;
                        ucPTTT.tc_TdlMedicineConcentra.Visible = false;
                        ucPTTT.tc_RequestDepartmentName.Visible = false;
                    }
                    ucPTTT.ReLoad(treeView_Click, listPTTT, this.v_his_specialist_exam, Edit_Click, Delete_Click);
                    //
                    List<SereServADO> listGPT = new List<SereServADO>();
                    listGPT.AddRange(SereServADOs.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL));
                    if (listGPT.Any())
                    {
                        tabGiaiPhauBenh.PageVisible = true;
                        ucGPB.tc_Number.Visible = false;
                        ucGPB.tc_TdlMedicineConcentra.Visible = false;
                        ucGPB.tc_RequestDepartmentName.Visible = false;
                    }
                    ucGPB.ReLoad(treeView_Click, listGPT, this.v_his_specialist_exam, Edit_Click, Delete_Click);
                    //
                    List<SereServADO> listOther = new List<SereServADO>();
                    listOther.AddRange(SereServADOs.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT
                        ));
                    //ucOrther.ReLoad(treeView_Click, listOther, this.RowCellClickBedRoom, Edit_Click, Delete_Click);


                    xtraTabControl1.SelectedTabPage = xtraTabControl1.TabPages[6];
                    xtraTabControl1.SelectedTabPage = xtraTabControl1.TabPages[5];
                    xtraTabControl1.SelectedTabPage = xtraTabControl1.TabPages[4];
                    xtraTabControl1.SelectedTabPage = xtraTabControl1.TabPages[3];
                    xtraTabControl1.SelectedTabPage = xtraTabControl1.TabPages[2];
                    xtraTabControl1.SelectedTabPage = xtraTabControl1.TabPages[1];
                    xtraTabControl1.SelectedTabPage = xtraTabControl1.TabPages[0];


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
                SetMaxlength(txtYKienBacSi, 4000, true);
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
                    return;
                }
                positionHandleControl = -1;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                v_his_specialist_exam.EXAM_EXECUTE_LOGINNAME = cboEmployee.EditValue != null ? cboEmployee.EditValue.ToString() : null;
                v_his_specialist_exam.EXAM_EXECUTE_USERNAME = cboEmployee.EditValue != null ? cboEmployee.Text.ToString() : null;
                v_his_specialist_exam.EXAM_EXECUTE_CONTENT = txtYKienBacSi.Text.Trim();
                v_his_specialist_exam.IS_APPROVAL = 1;
                var rs = new BackendAdapter(param).Post<HIS_SPECIALIST_EXAM>("api/HisSpecialistExam/Update", ApiConsumers.MosConsumer, v_his_specialist_exam, param);
                if (rs != null && this.delegateRefresh != null)
                {
                    this.delegateRefresh();
                }
                WaitingManager.Hide();
                MessageManager.Show(this, param, rs != null);
                SessionManager.ProcessTokenLost(param);
                if (rs != null)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

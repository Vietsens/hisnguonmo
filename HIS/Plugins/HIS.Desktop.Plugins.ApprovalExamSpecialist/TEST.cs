//using HIS.Desktop.LocalStorage.BackendData;
//using HIS.Desktop.Utility;
//using Inventec.Desktop.Common.LanguageManager;
//using MOS.EFMODEL.DataModels;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Resources;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using Inventec.Common.Controls.EditorLoader;
//using HIS.Desktop.Plugins.a2ApprovaleDebate.ADO;
//using MOS.SDO;
//using Inventec.Core;
//using MOS.Filter;
//using Inventec.Desktop.Common.Message;
//using Inventec.Common.Adapter;
//using HIS.Desktop.ApiConsumer;
//using HIS.Desktop.IsAdmin;
//using HIS.Desktop.LocalStorage.HisConfig;
//using EMR.SDO;
//using HIS.Desktop.ADO;
//using EMR.EFMODEL.DataModels;
//using HIS.Desktop.Controls.Session;
//using DevExpress.XtraTab;

//namespace HIS.Desktop.Plugins.a2ApprovaleDebate.ApprovaleDebate
//{
//    public partial class frmApprovaleDebate : FormBase
//    {
//        System.Globalization.CultureInfo cultureLang;
//        internal Inventec.Desktop.Common.Modules.Module currentModule { get; set; }
//        internal L_HIS_TREATMENT_BED_ROOM RowCellClickBedRoom { get; set; }
//        /// <summary>
//        ///Hàm xét ngôn ngữ cho giao diện frmApprovaleDebate
//        /// </summary>
//        private void SetCaptionByLanguageKey()
//        {
//            try
//            {
//                ////Khoi tao doi tuong resource
//                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.a2ApprovaleDebate.Resources.Lang", typeof(frmApprovaleDebate).Assembly);

//                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
//                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
//                this.tabToDieuTri.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabToDieuTri.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
//                this.tabCDHA.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabCDHA.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
//                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
//                this.cboEmployee.Properties.NullText = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.cboBacSiHoiChan.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
//                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
//                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
//                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
//                this.bbtnSave.Caption = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.bbtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
//                this.tabXetNghiem.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabXetNghiem.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
//                this.tabThuocVatTuMau.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabThuocVatTuMau.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
//                this.tabSieuAmNoiSoi.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabSieuAmNoiSoi.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
//                this.tabPhauThuatThuThuat.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabPhauThuatThuThuat.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
//                this.tabGiaiPhauBenh.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.tabGiaiPhauBenh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
//                this.Text = Inventec.Common.Resource.Get.Value("frmApprovaleDebate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
//            }
//            catch (Exception ex)
//            {
//                Inventec.Common.Logging.LogSystem.Warn(ex);
//            }
//        }

//        public frmApprovaleDebate(Inventec.Desktop.Common.Modules.Module module)
//                    : base(module)
//        {
//            try
//            {
//                InitializeComponent();
//                try
//                {
//                    cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
//                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
//                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
//                }
//                catch (Exception ex)
//                {
//                    Inventec.Common.Logging.LogSystem.Error(ex);
//                }

//                this.currentModule = module;
//                this.Text = module.text;
//            }
//            catch (Exception ex)
//            {
//                Inventec.Common.Logging.LogSystem.Error(ex);
//            }
//        }
//        private void frmApprovaleDebate_Load(object sender, EventArgs e)
//        {
//            try
//            {
//                AddUc();
//                //
//                InitComboEmployee();
//                //
//                string jsonString = @"{""ADD_TIME_STR"":""12/09/2024 10:32:09"",""PATIENT_CLASSIFY_NAME"":""Màu tím"",""DISPLAY_COLOR"":""192, 0, 192"",""ID"":6707,""TREATMENT_ID"":152715,""CO_TREATMENT_ID"":2182,""ADD_TIME"":20240912103209,""BED_ROOM_ID"":284,""REMOVE_TIME"":null,""TREATMENT_ROOM_ID"":null,""TDL_OBSERVED_TIME_FROM"":null,""TDL_OBSERVED_TIME_TO"":null,""PATIENT_ID"":127997,""TREATMENT_CODE"":""000000152638"",""TDL_PATIENT_FIRST_NAME"":""1"",""TDL_PATIENT_LAST_NAME"":""TK QUẢNG"",""TDL_PATIENT_NAME"":""TK QUẢNG 1 "",""TDL_PATIENT_DOB"":20040215000000,""TDL_PATIENT_GENDER_NAME"":""Nam"",""TDL_PATIENT_CODE"":""0000127858"",""TDL_PATIENT_ADDRESS"":""34 Trung Kính, Phường Yên Hoà, Quận Cầu Giấy, Hà Nội"",""TDL_HEIN_CARD_NUMBER"":null,""TDL_HEIN_MEDI_ORG_CODE"":null,""ICD_CODE"":""A00"",""ICD_NAME"":""Bệnh tả"",""ICD_TEXT"":null,""ICD_SUB_CODE"":null,""TDL_PATIENT_GENDER_ID"":2,""TDL_HEIN_MEDI_ORG_NAME"":null,""IS_PAUSE"":null,""IS_APPROVE_FINISH"":null,""APPROVE_FINISH_NOTE"":null,""TDL_PATIENT_CLASSIFY_ID"":2,""TDL_TREATMENT_TYPE_ID"":3,""EMR_COVER_TYPE_ID"":12,""CLINICAL_IN_TIME"":20240912100800,""CO_TREAT_DEPARTMENT_IDS"":""22"",""OUT_TIME"":null,""TDL_PATIENT_AVATAR_URL"":null,""LAST_DEPARTMENT_ID"":36,""TDL_PATIENT_UNSIGNED_NAME"":""TK QUANG 1 "",""TREATMENT_END_TYPE_ID"":null,""TREATMENT_METHOD"":""phương pháp điều trị"",""TDL_PATIENT_PHONE"":null,""TDL_HEIN_CARD_FROM_TIME"":null,""TDL_HEIN_CARD_TO_TIME"":null,""TDL_PATIENT_CCCD_NUMBER"":null,""TDL_PATIENT_CMND_NUMBER"":null,""TDL_PATIENT_PASSPORT_NUMBER"":null,""TDL_PATIENT_MOBILE"":null,""DOCTOR_LOGINNAME"":""quangln"",""DOCTOR_USERNAME"":""Lương Ngọc Quảng"",""PATIENT_TYPE_NAME"":""Khám Đích Danh Và Tái Khám"",""BED_NAME"":""Gường xịn"",""BED_CODE"":""G1"",""PATIENT_TYPE_CODE"":""05"",""BED_ROOM_NAME"":""buồng nội trú 2"",""TREATMENT_ROOM_CODE"":null,""TREATMENT_ROOM_NAME"":null,""LAST_DEPARTMENT_CODE"":""2"",""LAST_DEPARTMENT_NAME"":""Khoa Răng Hàm Mặt"",""NOTE"":null}";


//                RowCellClickBedRoom = Newtonsoft.Json.JsonConvert.DeserializeObject<L_HIS_TREATMENT_BED_ROOM>(jsonString);
//                if (RowCellClickBedRoom != null)
//                {
//                    LoadDataSereServByTreatmentId(RowCellClickBedRoom);
//                }
//            }
//            catch (Exception ex)
//            {
//                Inventec.Common.Logging.LogSystem.Error(ex);
//            }
//        }

//        UCTreeListService ucCDHA, ucXetNghiem, ucThuocVatTu, ucSieuAm, ucPTTT, ucGPB;
//        UCTreeListTracking ucAll;
//        private void AddUc()
//        {
//            try
//            {
//                ucAll = new UCTreeListTracking(imageCollection1, currentModule);
//                tabToDieuTri.Controls.Add(ucAll);
//                ucAll.Dock = DockStyle.Fill;

//                //
//                ucCDHA = new UCTreeListService(imageCollection1, currentModule);
//                tabCDHA.Controls.Add(ucCDHA);
//                ucCDHA.Dock = DockStyle.Fill;
//                //
//                ucXetNghiem = new UCTreeListService(imageCollection1, currentModule);
//                tabXetNghiem.Controls.Add(ucXetNghiem);
//                ucXetNghiem.Dock = DockStyle.Fill;
//                //
//                ucThuocVatTu = new UCTreeListService(imageCollection1, currentModule);
//                tabThuocVatTuMau.Controls.Add(ucThuocVatTu);
//                ucThuocVatTu.Dock = DockStyle.Fill;
//                //
//                ucSieuAm = new UCTreeListService(imageCollection1, currentModule);
//                tabSieuAmNoiSoi.Controls.Add(ucSieuAm);
//                ucSieuAm.Dock = DockStyle.Fill;
//                //
//                ucPTTT = new UCTreeListService(imageCollection1, currentModule);
//                tabPhauThuatThuThuat.Controls.Add(ucPTTT);
//                ucPTTT.Dock = DockStyle.Fill;
//                //
//                ucGPB = new UCTreeListService(imageCollection1, currentModule);
//                tabGiaiPhauBenh.Controls.Add(ucGPB);
//                ucGPB.Dock = DockStyle.Fill;

//            }
//            catch (Exception ex)
//            {
//                Inventec.Common.Logging.LogSystem.Error(ex);
//            }
//        }
//        private void cboEmployee_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
//        {
//            if (cboEmployee.EditValue != null && e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
//                cboEmployee.EditValue = null;
//        }

//        private void InitComboEmployee()
//        {
//            try
//            {
//                // HIS_SPECIALIST_EXAM.EXAM_EXECUTE_DEPARMENT_ID     HIS_EMPLOYEE.DEPARTMENT_ID 
//                var data = BackendDataWorker.Get<V_HIS_EMPLOYEE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_DOCTOR == 1).ToList();
//                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
//                columnInfos.Add(new ColumnInfo("LOGINNAME", "Tên đăng nhập", 150, 1));
//                columnInfos.Add(new ColumnInfo("TDL_USERNAME", "Họ và tên", 250, 1));
//                ControlEditorADO controlEditorADO = new ControlEditorADO("TDL_USERNAME", "LOGINNAME", columnInfos, false, 400);
//                ControlEditorLoader.Load(cboEmployee, data, controlEditorADO);
//                cboEmployee.Properties.ImmediatePopup = true;
//                cboEmployee.Properties.PopupFormMinSize = new Size(400, cboEmployee.Properties.PopupFormMinSize.Height);
//            }
//            catch (Exception ex)
//            {
//                Inventec.Common.Logging.LogSystem.Error(ex);
//            }
//        }

//        //private void LoadDataSereServByTreatmentId(ServiceReqGroupByDateADO currentHisServiceReq)
//        private void LoadDataSereServByTreatmentId(L_HIS_TREATMENT_BED_ROOM currentHisServiceReq)
//        {
//            try
//            {
//                foreach (XtraTabPage item in this.xtraTabControl1.TabPages)
//                {
//                    item.PageVisible = false;
//                }
//                List<SereServADO> SereServADOs = new List<SereServADO>();
//                List<DHisSereServ2> dataNew = new List<DHisSereServ2>();
//                List<HIS_SERVICE_REQ> dataServiceReq = new List<HIS_SERVICE_REQ>();
//                WaitingManager.Show();
//                if (currentHisServiceReq != null && currentHisServiceReq.TREATMENT_ID > 0)
//                {
//                    CommonParam param = new CommonParam();
//                    DHisSereServ2Filter _sereServ2Filter = new DHisSereServ2Filter();
//                    _sereServ2Filter.TREATMENT_ID = currentHisServiceReq.TREATMENT_ID;
//                    //_sereServ2Filter.INTRUCTION_DATE = Int64.Parse(currentHisServiceReq.InstructionDate.ToString().Substring(0, 8) + "000000");
//                    dataNew = new BackendAdapter(param).Get<List<DHisSereServ2>>("api/HisSereServ/GetDHisSereServ2", ApiConsumers.MosConsumer, _sereServ2Filter, param);
//                    if (dataNew != null && dataNew.Count > 0)
//                    {
//                        //if ((long)cboFilterByDepartment.EditValue == (long)0) //Theo khoa
//                        //{
//                        //    dataNew = dataNew.Where(o => o.REQUEST_DEPARTMENT_ID == this.DepartmentID).ToList();
//                        //}

//                        //if (!currentHisServiceReq.isParent)
//                        //{
//                        //    dataNew = dataNew.Where(o => o.TRACKING_ID == currentHisServiceReq.TRACKING_ID).ToList();
//                        //}
//                        HisServiceReqFilter filter = new HisServiceReqFilter();
//                        filter.IDs = dataNew.Select(o => o.SERVICE_REQ_ID ?? 0).ToList();
//                        dataServiceReq = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, param);
//                        var listRootByType = dataNew.OrderByDescending(o => o.TRACKING_TIME).GroupBy(o => o.TDL_SERVICE_TYPE_ID).ToList();

//                        //H
//                        //var department = currentModule != null ? BackendDataWorker.Get<HIS_ROOM>().FirstOrDefault(p => p.ID == currentModule.RoomId) : null;
//                        var department = currentModule != null ? new HIS_ROOM() : null;
//                        var departmentId = department != null ? department.DEPARTMENT_ID : 0;
//                        foreach (var types in listRootByType)
//                        {
//                            SereServADO ssRootType = new SereServADO();
//                            #region Parent
//                            ssRootType.CONCRETE_ID__IN_SETY = types.First().TDL_SERVICE_TYPE_ID + "";
//                            var serviceType = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(p => p.ID == types.First().TDL_SERVICE_TYPE_ID);
//                            long idSerReqType = 0;
//                            long idDepartment = 0;
//                            long idExecuteDepartment = 0;
//                            short? IsTemporaryPres = 0;
//                            if (dataServiceReq != null && dataServiceReq.Count > 0)
//                            {
//                                if (dataServiceReq.Where(o => o.ID == types.First().SERVICE_REQ_ID) != null && dataServiceReq.Where(o => o.ID == types.First().SERVICE_REQ_ID).ToList().Count > 0)
//                                {
//                                    idSerReqType = dataServiceReq.Where(o => o.ID == types.First().SERVICE_REQ_ID).FirstOrDefault().SERVICE_REQ_TYPE_ID;
//                                    idDepartment = dataServiceReq.Where(o => o.ID == types.First().SERVICE_REQ_ID).FirstOrDefault().REQUEST_DEPARTMENT_ID;
//                                    idExecuteDepartment = dataServiceReq.Where(o => o.ID == types.First().SERVICE_REQ_ID).FirstOrDefault().EXECUTE_DEPARTMENT_ID;
//                                    IsTemporaryPres = dataServiceReq.Where(o => o.ID == types.First().SERVICE_REQ_ID).FirstOrDefault().IS_TEMPORARY_PRES;
//                                }
//                            }
//                            ssRootType.TRACKING_TIME = types.First().TRACKING_TIME;
//                            ssRootType.TDL_SERVICE_TYPE_ID = types.First().TDL_SERVICE_TYPE_ID;
//                            ssRootType.SERVICE_CODE = serviceType != null ? serviceType.SERVICE_TYPE_NAME : null;
//                            #endregion
//                            SereServADOs.Add(ssRootType);
//                            var listRootSety = types.GroupBy(g => g.SERVICE_REQ_ID).ToList();
//                            foreach (var rootSety in listRootSety)
//                            {
//                                #region Child
//                                SereServADO ssRootSety = new SereServADO();
//                                ssRootSety.CONCRETE_ID__IN_SETY = ssRootType.CONCRETE_ID__IN_SETY + "_" + rootSety.First().SERVICE_REQ_ID;
//                                //qtcode
//                                if (rootSety.First().USE_TIME.HasValue)
//                                {
//                                    ssRootSety.REQUEST_DEPARTMENT_NAME = string.Format("Dự trù: {0}", Inventec.Common.DateTime.Convert.TimeNumberToDateString(rootSety.First().USE_TIME.Value));
//                                }
//                                //qtcode
//                                ssRootSety.PARENT_ID__IN_SETY = ssRootType.CONCRETE_ID__IN_SETY;
//                                ssRootSety.REQUEST_DEPARTMENT_ID = idDepartment;
//                                ssRootSety.EXECUTE_DEPARTMENT_ID = idExecuteDepartment;
//                                ssRootSety.SERVICE_REQ_TYPE_ID = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().FirstOrDefault(p => p.ID == idSerReqType) != null ?
//                                BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().FirstOrDefault(p => p.ID == idSerReqType).ID : 0;
//                                ssRootSety.TRACKING_TIME = rootSety.First().TRACKING_TIME;
//                                ssRootSety.SERVICE_REQ_ID = rootSety.First().SERVICE_REQ_ID;
//                                ssRootSety.SERVICE_REQ_STT_ID = rootSety.First().SERVICE_REQ_STT_ID;
//                                ssRootSety.TDL_SERVICE_TYPE_ID = rootSety.First().TDL_SERVICE_TYPE_ID;
//                                ssRootSety.SERVICE_CODE = rootSety.First().SERVICE_REQ_CODE;
//                                ssRootSety.SERVICE_REQ_CODE = rootSety.First().SERVICE_REQ_CODE;
//                                ssRootSety.IS_TEMPORARY_PRES = IsTemporaryPres;
//                                if (dataServiceReq != null && dataServiceReq.Count > 0)
//                                {
//                                    var serviceReq = dataServiceReq.FirstOrDefault(o => o.ID == rootSety.First().SERVICE_REQ_ID) ?? new HIS_SERVICE_REQ();
//                                    ssRootSety.SAMPLE_TIME = serviceReq.SAMPLE_TIME;
//                                    ssRootSety.RECEIVE_SAMPLE_TIME = serviceReq.RECEIVE_SAMPLE_TIME;
//                                }
//                                ssRootSety.TDL_TREATMENT_ID = rootSety.First().TDL_TREATMENT_ID;
//                                ssRootSety.PRESCRIPTION_TYPE_ID = rootSety.First().PRESCRIPTION_TYPE_ID;
//                                ssRootSety.REQUEST_LOGINNAME = rootSety.First().REQUEST_LOGINNAME;
//                                ssRootSety.REQUEST_DEPARTMENT_ID = rootSety.First().REQUEST_DEPARTMENT_ID ?? 0;
//                                ssRootSety.SERVICE_NAME = String.Format("- {0} - {1}", rootSety.First().REQUEST_ROOM_NAME, rootSety.First().REQUEST_DEPARTMENT_NAME);
//                                var time = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rootSety.First().TDL_INTRUCTION_TIME ?? 0);
//                                ssRootSety.NOTE_ADO = time.Substring(0, time.Count() - 3);
//                                if ((rootSety.First().REQUEST_LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() || CheckLoginAdmin.IsAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()))
//                                    && (rootSety.First().SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL || HisConfigs.Get<string>("MOS.HIS_SERVICE_REQ.ALLOW_MODIFYING_OF_STARTED") == "1" || (HisConfigs.Get<string>("MOS.HIS_SERVICE_REQ.ALLOW_MODIFYING_OF_STARTED") == "2"
//                                    && ssRootSety.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH))
//                                    && rootSety.First().IS_NO_EXECUTE != 1)
//                                {
//                                    ssRootSety.IsEnableEdit = true;
//                                }
//                                if ((rootSety.First().REQUEST_LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() || CheckLoginAdmin.IsAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName())
//                                  || (rootSety.First().REQUEST_DEPARTMENT_ID == departmentId && ssRootSety.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH))
//                                  && rootSety.First().SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
//                                {
//                                    ssRootSety.IsEnableDelete = true;
//                                }


//                                SereServADOs.Add(ssRootSety);
//                                #endregion
//                                int d = 0;
//                                foreach (var item in rootSety)
//                                {
//                                    d++;
//                                    #region Child (+n)
//                                    SereServADO ado = new SereServADO(item);
//                                    ado.CONCRETE_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY + "_" + d;
//                                    ado.PARENT_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY;
//                                    if (!String.IsNullOrWhiteSpace(item.TUTORIAL))
//                                    {
//                                        ado.NOTE_ADO = string.Format("{0}. {1}", item.TUTORIAL, item.INSTRUCTION_NOTE);

//                                    }
//                                    else
//                                    {
//                                        ado.NOTE_ADO = string.Format("{0}", item.INSTRUCTION_NOTE);
//                                    }
//                                    ado.AMOUNT_SER = string.Format("{0} - {1}", item.AMOUNT, item.SERVICE_UNIT_NAME);
//                                    ado.IS_TEMPORARY_PRES = IsTemporaryPres;
//                                    SereServADOs.Add(ado);
//                                    #endregion
//                                }
//                            }
//                        }
//                    }
//                    //this.LoadDataTrackingList(dataNew, currentHisServiceReq);
//                }
//                WaitingManager.Hide();
//                if (SereServADOs != null && SereServADOs.Count > 0)
//                {

//                    SereServADOs = SereServADOs.OrderBy(o => o.PARENT_ID__IN_SETY).ThenBy(p => p.SERVICE_CODE).ThenBy(o => o.SERVICE_NAME).ToList();
//                    try
//                    {
//                        WaitingManager.Show();
//                        CommonParam paramCommon = new CommonParam();
//                        MOS.Filter.HisTrackingViewFilter trackingFilter = new MOS.Filter.HisTrackingViewFilter();
//                        trackingFilter.TREATMENT_ID = currentHisServiceReq.TREATMENT_ID;
//                        trackingFilter.ORDER_FIELD = "TRACKING_TIME";
//                        trackingFilter.ORDER_DIRECTION = "DESC";
//                        var resultTracking = new BackendAdapter(paramCommon).Get<List<HIS_TRACKING>>(HisRequestUriStore.HIS_TRACKING_GET, ApiConsumers.MosConsumer, trackingFilter, paramCommon);
//                        if (resultTracking != null)
//                        {
//                            //var vHisTrackingList = (List<V_HIS_TRACKING>)result;
//                            //Inventec.Common.Logging.LogSystem.Info(Newtonsoft.Json.JsonConvert.SerializeObject(vHisTrackingList));
//                            var Employees = BackendDataWorker.Get<V_HIS_EMPLOYEE>()/*.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)*/.ToList();
//                            List<TrackingListADO> listTracking = (from a in resultTracking
//                                                                  join b in dataNew on a.ID equals b.TRACKING_ID
//                                                                  into AB
//                                                                  from ab in AB.DefaultIfEmpty()
//                                                                  join c in Employees on a.CREATOR equals c.LOGINNAME
//                                                                  into AC
//                                                                  from ac in AC.DefaultIfEmpty()
//                                                                  select new
//                                                                  {
//                                                                      TRACKING_ID = a.ID,
//                                                                      TRACKING_TIME = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(a.TRACKING_TIME)
//                                                                                        .Replace(" ", Environment.NewLine),
//                                                                      USER_NAME = ac?.DIPLOMA,
//                                                                      CONTENT = a.CONTENT,
//                                                                      SERVICE_NAME = ab?.SERVICE_NAME,
//                                                                      SERVICE_REQ_CODE = ab?.SERVICE_REQ_CODE,
//                                                                      AMOUNT = ab?.AMOUNT,
//                                                                      SERVICE_UNIT_NAME = ab?.SERVICE_UNIT_NAME,
//                                                                  })
//                                                                  .GroupBy(g => g.TRACKING_ID)
//                                                                  .Select((s, i) => new TrackingListADO()
//                                                                  {
//                                                                      CONCRETE_ID__IN_SETY = (i + 1).ToString(),
//                                                                      TRACKING_TIME = s.First().TRACKING_TIME,
//                                                                      USER_NAME = s.First().USER_NAME,
//                                                                      CONTENT = s.First().CONTENT,
//                                                                      SERVICE = string.Join(Environment.NewLine, s.Where(w => !string.IsNullOrEmpty(w.SERVICE_NAME))
//                                                                      .Select(ss => $"{ss.SERVICE_REQ_CODE} - {ss.SERVICE_NAME} x {ss.AMOUNT} {ss.SERVICE_UNIT_NAME}"))
//                                                                  })
//                                                                  .ToList();
//                            tabToDieuTri.PageVisible = true;
//                            ucAll.ReLoad(treeView_Click, listTracking, this.RowCellClickBedRoom);
//                        }
//                        WaitingManager.Hide();
//                    }
//                    catch (Exception ex)
//                    {
//                        WaitingManager.Hide();
//                        Inventec.Common.Logging.LogSystem.Error(ex);
//                    }

//                    //
//                    List<SereServADO> listCDHA = new List<SereServADO>();
//                    listCDHA.AddRange(SereServADOs.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA));
//                    ucCDHA.ReLoad(treeView_Click, listCDHA, this.RowCellClickBedRoom, Edit_Click, Delete_Click);
//                    if (listCDHA.Any())
//                    {
//                        tabCDHA.PageVisible = true;
//                        ucCDHA.tc_Number.Visible = false;
//                        ucCDHA.tc_TdlMedicineConcentra.Visible = false;
//                        ucCDHA.tc_RequestDepartmentName.Visible = false;
//                    }
//                    //
//                    List<SereServADO> listXetNghiem = new List<SereServADO>();
//                    listXetNghiem.AddRange(SereServADOs.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN));
//                    if (listXetNghiem.Any())
//                    {
//                        tabXetNghiem.PageVisible = true;
//                        ucXetNghiem.tc_Number.Visible = false;
//                        ucXetNghiem.tc_TdlMedicineConcentra.Visible = false;
//                        ucXetNghiem.tc_RequestDepartmentName.Visible = false;
//                    }
//                    ucXetNghiem.ReLoad(treeView_Click, listXetNghiem, this.RowCellClickBedRoom, Edit_Click, Delete_Click);
//                    //
//                    List<SereServADO> listMediMate = new List<SereServADO>();
//                    listMediMate.AddRange(SereServADOs.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
//                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
//                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU
//                        ));
//                    if (listMediMate.Any())
//                    {
//                        tabThuocVatTuMau.PageVisible = true;
//                    }
//                    ucThuocVatTu.ReLoad(treeView_Click, listMediMate, this.RowCellClickBedRoom, Edit_Click, Delete_Click);
//                    //
//                    List<SereServADO> listSieuAm = new List<SereServADO>();
//                    listSieuAm.AddRange(SereServADOs.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA
//                    || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS));
//                    if (listSieuAm.Any())
//                    {
//                        tabSieuAmNoiSoi.PageVisible = true;
//                        ucSieuAm.tc_Number.Visible = false;
//                        ucSieuAm.tc_TdlMedicineConcentra.Visible = false;
//                        ucSieuAm.tc_RequestDepartmentName.Visible = false;
//                    }
//                    ucSieuAm.ReLoad(treeView_Click, listSieuAm, this.RowCellClickBedRoom, Edit_Click, Delete_Click);
//                    //
//                    List<SereServADO> listPTTT = new List<SereServADO>();
//                    listPTTT.AddRange(SereServADOs.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH
//                    || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN));
//                    if (listPTTT.Any())
//                    {
//                        tabPhauThuatThuThuat.PageVisible = true;
//                        ucPTTT.tc_Number.Visible = false;
//                        ucPTTT.tc_TdlMedicineConcentra.Visible = false;
//                        ucPTTT.tc_RequestDepartmentName.Visible = false;
//                    }
//                    ucPTTT.ReLoad(treeView_Click, listPTTT, this.RowCellClickBedRoom, Edit_Click, Delete_Click);
//                    //
//                    List<SereServADO> listGPT = new List<SereServADO>();
//                    listGPT.AddRange(SereServADOs.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL));
//                    if (listGPT.Any())
//                    {
//                        tabGiaiPhauBenh.PageVisible = true;
//                        ucGPB.tc_Number.Visible = false;
//                        ucGPB.tc_TdlMedicineConcentra.Visible = false;
//                        ucGPB.tc_RequestDepartmentName.Visible = false;
//                    }
//                    ucGPB.ReLoad(treeView_Click, listGPT, this.RowCellClickBedRoom, Edit_Click, Delete_Click);
//                    //
//                    List<SereServADO> listOther = new List<SereServADO>();
//                    listOther.AddRange(SereServADOs.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN
//                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G
//                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH
//                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC
//                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN
//                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT
//                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT
//                        ));
//                    //ucOrther.ReLoad(treeView_Click, listOther, this.RowCellClickBedRoom, Edit_Click, Delete_Click);


//                    xtraTabControl1.SelectedTabPage = xtraTabControl1.TabPages[6];
//                    xtraTabControl1.SelectedTabPage = xtraTabControl1.TabPages[5];
//                    xtraTabControl1.SelectedTabPage = xtraTabControl1.TabPages[4];
//                    xtraTabControl1.SelectedTabPage = xtraTabControl1.TabPages[3];
//                    xtraTabControl1.SelectedTabPage = xtraTabControl1.TabPages[2];
//                    xtraTabControl1.SelectedTabPage = xtraTabControl1.TabPages[1];
//                    xtraTabControl1.SelectedTabPage = xtraTabControl1.TabPages[0];


//                }
//                //else
//                //{
//                //    ucAll.ReLoad(treeView_Click, null, this.RowCellClickBedRoom, Edit_Click, Delete_Click);
//                //    ucCDHA.ReLoad(treeView_Click, null, this.RowCellClickBedRoom, Edit_Click, Delete_Click);
//                //    ucMediMate.ReLoad(treeView_Click, null, this.RowCellClickBedRoom, Edit_Click, Delete_Click);
//                //    ucOrther.ReLoad(treeView_Click, null, this.RowCellClickBedRoom, Edit_Click, Delete_Click);
//                //}

//            }
//            catch (Exception ex)
//            {
//                WaitingManager.Hide();
//                Inventec.Common.Logging.LogSystem.Error(ex);
//            }
//        }


//        private void LoadDataTrackingList(List<DHisSereServ2> dataNew, L_HIS_TREATMENT_BED_ROOM currentHisServiceReq)
//        {
//            try
//            {
//                WaitingManager.Show();
//                CommonParam paramCommon = new CommonParam();
//                MOS.Filter.HisTrackingViewFilter trackingFilter = new MOS.Filter.HisTrackingViewFilter();
//                trackingFilter.TREATMENT_ID = currentHisServiceReq.TREATMENT_ID;
//                trackingFilter.ORDER_FIELD = "TRACKING_TIME";
//                trackingFilter.ORDER_DIRECTION = "DESC";
//                var resultTracking = new BackendAdapter(paramCommon).Get<List<HIS_TRACKING>>(HisRequestUriStore.HIS_TRACKING_GET, ApiConsumers.MosConsumer, trackingFilter, paramCommon);
//                if (resultTracking != null)
//                {
//                    //var vHisTrackingList = (List<V_HIS_TRACKING>)result;

//                    //Inventec.Common.Logging.LogSystem.Info(Newtonsoft.Json.JsonConvert.SerializeObject(vHisTrackingList));
//                    var Employees = BackendDataWorker.Get<V_HIS_EMPLOYEE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

//                    //e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME.Value);
//                    var trackingView = resultTracking
//                                    .Join(dataNew, a => a.ID, b => b.TRACKING_ID, (a, b) => new { a, b })
//                                    .Select((x, index) => new
//                                    {
//                                        CONCRETE_ID__IN_SETY = index + 1,
//                                        TRACKING_TIME = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(x.a.TRACKING_TIME),
//                                        CONTENT = x.a.CONTENT
//                                    });

//                }
//                WaitingManager.Hide();
//            }
//            catch (Exception ex)
//            {
//                WaitingManager.Hide();
//                Inventec.Common.Logging.LogSystem.Error(ex);
//            }
//        }


//        private List<SereServADO> GroupDataByTracking(List<DHisSereServ2> dataNew, List<HIS_SERVICE_REQ> dataServiceReq)
//        {
//            List<SereServADO> SereServADOs = new List<ADO.SereServADO>();
//            try
//            {
//                //H
//                //var departmentId = BackendDataWorker.Get<HIS_ROOM>().FirstOrDefault(p => p.ID == currentModule.RoomId).DEPARTMENT_ID;
//                long departmentId = 0;

//                var listRootByTracking = dataNew.OrderByDescending(o => o.TRACKING_TIME).GroupBy(o => o.TRACKING_TIME).ToList();
//                foreach (var tracking in listRootByTracking)
//                {
//                    #region GrandFather
//                    SereServADO ssRootTrackingTime = new ADO.SereServADO();
//                    ssRootTrackingTime.CONCRETE_ID__IN_SETY = tracking.First().TRACKING_TIME + "_";
//                    string dayHospitalize = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(tracking.First().TRACKING_TIME ?? 0);
//                    ssRootTrackingTime.SERVICE_CODE = !string.IsNullOrEmpty(dayHospitalize) ? (System.String.Format("{0:dd/MM/yyyy HH:mm}", dayHospitalize)).Substring(0, (System.String.Format("{0:dd/MM/yyyy HH:mm}", dayHospitalize)).Length - 3) : "Chưa tạo tờ điều trị";
//                    SereServADOs.Add(ssRootTrackingTime);
//                    int count = 0;
//                    #endregion
//                    var listRootType = tracking.GroupBy(g => g.TDL_SERVICE_TYPE_ID).ToList();
//                    foreach (var types in listRootType)
//                    {
//                        #region Parent
//                        count++;
//                        SereServADO ssRootType = new SereServADO();
//                        ssRootType.CONCRETE_ID__IN_SETY = ssRootTrackingTime.CONCRETE_ID__IN_SETY + "_" + types.First().TRACKING_TIME + "_" + count;
//                        ssRootType.PARENT_ID__IN_SETY = ssRootTrackingTime.CONCRETE_ID__IN_SETY;
//                        var serviceType = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(p => p.ID == types.First().TDL_SERVICE_TYPE_ID);
//                        long idSerReqType = 0;
//                        long idDepartment = 0;
//                        long idExecuteDepartment = 0;
//                        short? IsTemporaryPres = 0;
//                        if (dataServiceReq != null && dataServiceReq.Count > 0)
//                        {
//                            if (dataServiceReq.Where(o => o.ID == types.First().SERVICE_REQ_ID) != null && dataServiceReq.Where(o => o.ID == types.First().SERVICE_REQ_ID).ToList().Count > 0)
//                            {
//                                idSerReqType = dataServiceReq.Where(o => o.ID == types.First().SERVICE_REQ_ID).FirstOrDefault().SERVICE_REQ_TYPE_ID;
//                                idDepartment = dataServiceReq.Where(o => o.ID == types.First().SERVICE_REQ_ID).FirstOrDefault().REQUEST_DEPARTMENT_ID;
//                                idExecuteDepartment = dataServiceReq.Where(o => o.ID == types.First().SERVICE_REQ_ID).FirstOrDefault().EXECUTE_DEPARTMENT_ID;
//                                IsTemporaryPres = dataServiceReq.Where(o => o.ID == types.First().SERVICE_REQ_ID).FirstOrDefault().IS_TEMPORARY_PRES;
//                            }
//                        }
//                        ssRootType.TRACKING_TIME = types.First().TRACKING_TIME;
//                        ssRootType.TDL_SERVICE_TYPE_ID = types.First().TDL_SERVICE_TYPE_ID;
//                        ssRootType.SERVICE_CODE = serviceType != null ? serviceType.SERVICE_TYPE_NAME : null;
//                        #endregion
//                        SereServADOs.Add(ssRootType);
//                        var listRootSety = types.GroupBy(g => g.SERVICE_REQ_ID).ToList();
//                        foreach (var rootSety in listRootSety)
//                        {
//                            #region Child
//                            SereServADO ssRootSety = new SereServADO();
//                            ssRootSety.CONCRETE_ID__IN_SETY = ssRootType.CONCRETE_ID__IN_SETY + "_" + rootSety.First().SERVICE_REQ_ID;
//                            ssRootSety.PARENT_ID__IN_SETY = ssRootType.CONCRETE_ID__IN_SETY;
//                            ssRootSety.REQUEST_DEPARTMENT_ID = idDepartment;
//                            ssRootSety.EXECUTE_DEPARTMENT_ID = idExecuteDepartment;
//                            ssRootSety.SERVICE_REQ_TYPE_ID = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().FirstOrDefault(p => p.ID == idSerReqType) != null ?
//                            BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().FirstOrDefault(p => p.ID == idSerReqType).ID : 0;
//                            //qtcode
//                            if (rootSety.First().USE_TIME.HasValue)
//                            {
//                                ssRootSety.REQUEST_DEPARTMENT_NAME = string.Format("Dự trù: {0}", Inventec.Common.DateTime.Convert.TimeNumberToDateString(rootSety.First().USE_TIME.Value));
//                            }
//                            //qtcode
//                            ssRootSety.TRACKING_TIME = rootSety.First().TRACKING_TIME;
//                            ssRootSety.SERVICE_REQ_ID = rootSety.First().SERVICE_REQ_ID;
//                            ssRootSety.SERVICE_REQ_STT_ID = rootSety.First().SERVICE_REQ_STT_ID;
//                            ssRootSety.TDL_SERVICE_TYPE_ID = rootSety.First().TDL_SERVICE_TYPE_ID;
//                            ssRootSety.PRESCRIPTION_TYPE_ID = rootSety.First().PRESCRIPTION_TYPE_ID;
//                            ssRootSety.TDL_TREATMENT_ID = rootSety.First().TDL_TREATMENT_ID;
//                            ssRootSety.REQUEST_LOGINNAME = rootSety.First().REQUEST_LOGINNAME;
//                            ssRootSety.REQUEST_DEPARTMENT_ID = rootSety.First().REQUEST_DEPARTMENT_ID ?? 0;
//                            ssRootSety.SERVICE_CODE = rootSety.First().SERVICE_REQ_CODE;
//                            ssRootSety.SERVICE_REQ_CODE = rootSety.First().SERVICE_REQ_CODE;
//                            ssRootSety.IS_TEMPORARY_PRES = IsTemporaryPres;
//                            if (dataServiceReq != null && dataServiceReq.Count > 0)
//                            {
//                                var serviceReq = dataServiceReq.FirstOrDefault(o => o.ID == rootSety.First().SERVICE_REQ_ID) ?? new HIS_SERVICE_REQ();
//                                ssRootSety.SAMPLE_TIME = serviceReq.SAMPLE_TIME;
//                                ssRootSety.RECEIVE_SAMPLE_TIME = serviceReq.RECEIVE_SAMPLE_TIME;
//                            }
//                            ssRootSety.SERVICE_NAME = String.Format("- {0} - {1}", rootSety.First().REQUEST_ROOM_NAME, rootSety.First().REQUEST_DEPARTMENT_NAME);
//                            var time = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rootSety.First().TDL_INTRUCTION_TIME ?? 0);
//                            ssRootSety.NOTE_ADO = time.Substring(0, time.Count() - 3);

//                            if ((rootSety.First().REQUEST_LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() || CheckLoginAdmin.IsAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()))
//                                    && (rootSety.First().SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL || HisConfigs.Get<string>("MOS.HIS_SERVICE_REQ.ALLOW_MODIFYING_OF_STARTED") == "1" || (HisConfigs.Get<string>("MOS.HIS_SERVICE_REQ.ALLOW_MODIFYING_OF_STARTED") == "2"
//                                    && ssRootSety.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH))
//                                    && rootSety.First().IS_NO_EXECUTE != 1)
//                            {
//                                ssRootSety.IsEnableEdit = true;
//                            }
//                            if ((rootSety.First().REQUEST_LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() || CheckLoginAdmin.IsAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName())
//                              || (rootSety.First().REQUEST_DEPARTMENT_ID == departmentId && ssRootSety.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH))
//                              && rootSety.First().SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
//                            {
//                                ssRootSety.IsEnableDelete = true;
//                            }

//                            SereServADOs.Add(ssRootSety);
//                            #endregion
//                            int d = 0;
//                            foreach (var item in rootSety)
//                            {
//                                d++;
//                                #region Child (+n)
//                                SereServADO ado = new SereServADO(item);
//                                ado.IS_TEMPORARY_PRES = IsTemporaryPres;
//                                ado.CONCRETE_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY + "_" + d;
//                                ado.PARENT_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY;
//                                ado.child = 4;

//                                if (!String.IsNullOrWhiteSpace(item.TUTORIAL))
//                                {
//                                    ado.NOTE_ADO = string.Format("{0}. {1}", item.TUTORIAL, item.INSTRUCTION_NOTE);

//                                }
//                                else
//                                {
//                                    ado.NOTE_ADO = string.Format("{0}", item.INSTRUCTION_NOTE);
//                                }

//                                ado.AMOUNT_SER = string.Format("{0} - {1}", item.AMOUNT, item.SERVICE_UNIT_NAME);
//                                SereServADOs.Add(ado);
//                                #endregion
//                            }
//                        }
//                    }

//                }

//            }
//            catch (Exception ex)
//            {
//                SereServADOs = new List<ADO.SereServADO>();
//                WaitingManager.Hide();
//                Inventec.Common.Logging.LogSystem.Error(ex);
//            }
//            return SereServADOs;
//        }
//        private void Edit_Click(SereServADO currentSS)
//        {
//            //    try
//            //    {
//            //        if (currentSS != null)
//            //        {
//            //            CommonParam param = new CommonParam();
//            //            HisTreatmentFilter filter = new HisTreatmentFilter();
//            //            filter.ID = currentSS.TDL_TREATMENT_ID;
//            //            var dtTreatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param);

//            //            if (dtTreatment != null && dtTreatment.Count > 0)
//            //            {
//            //                if (dtTreatment[0].IS_ACTIVE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE || dtTreatment[0].IS_PAUSE == 1)
//            //                {
//            //                    DevExpress.XtraEditors.XtraMessageBox.Show(
//            //                   Resources.ResourceMessage.HoSoDieuTriDangTamKhoa,
//            //                   Resources.ResourceMessage.ThongBao,
//            //                   MessageBoxButtons.OK);
//            //                    return;
//            //                }
//            //            }
//            //            HisServiceReqFilter sfilter = new HisServiceReqFilter();
//            //            sfilter.ID = currentSS.SERVICE_REQ_ID;
//            //            var dtServiceReq = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, sfilter, param);

//            //            if (currentSS.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH)
//            //            {
//            //                WaitingManager.Show();
//            //                List<object> sendObj = new List<object>() { currentSS.SERVICE_REQ_ID };
//            //                CallModule("HIS.Desktop.Plugins.UpdateExamServiceReq", sendObj);
//            //                WaitingManager.Hide();
//            //            }
//            //            else if ((currentSS.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC || currentSS.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT) && IsCheckDepartmentTran())
//            //            {
//            //                AssignPrescriptionEditADO assignEditADO = null;
//            //                HisExpMestFilter expfilter = new HisExpMestFilter();
//            //                expfilter.SERVICE_REQ_ID = dtServiceReq != null && dtServiceReq.Count > 0 ? dtServiceReq[0].ID : 0;
//            //                var expMests = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumer.ApiConsumers.MosConsumer, expfilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
//            //                if (expMests != null && expMests.Count == 1)
//            //                {
//            //                    var expMest = expMests.FirstOrDefault();
//            //                    if (expMest.IS_NOT_TAKEN.HasValue && expMest.IS_NOT_TAKEN.Value == 1)
//            //                    {
//            //                        WaitingManager.Hide();
//            //                        MessageBox.Show(Resources.ResourceMessage.DonKhongLayKhongChoPhepSua);
//            //                        return;
//            //                    }
//            //                    assignEditADO = new AssignPrescriptionEditADO(dtServiceReq[0], expMest, FillDataApterSave);
//            //                }
//            //                else
//            //                {
//            //                    assignEditADO = new AssignPrescriptionEditADO(dtServiceReq[0], null, FillDataApterSave);
//            //                }

//            //                var assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionADO(currentSS.TDL_TREATMENT_ID ?? 0, 0, currentSS.SERVICE_REQ_ID ?? 0);
//            //                if (dtTreatment != null && dtTreatment.Count > 0)
//            //                {
//            //                    assignServiceADO.GenderName = dtTreatment[0].TDL_PATIENT_GENDER_NAME;
//            //                    assignServiceADO.PatientDob = dtTreatment[0].TDL_PATIENT_DOB;
//            //                    assignServiceADO.PatientName = dtTreatment[0].TDL_PATIENT_NAME;
//            //                }

//            //                assignServiceADO.AssignPrescriptionEditADO = assignEditADO;

//            //                List<object> sendObj = new List<object>() { assignServiceADO };

//            //                if (currentSS.PRESCRIPTION_TYPE_ID == 1)
//            //                {
//            //                    string moduleLink = "HIS.Desktop.Plugins.AssignPrescriptionPK";
//            //                    CallModule(moduleLink, sendObj);

//            //                }
//            //                else if (currentSS.PRESCRIPTION_TYPE_ID == 2)
//            //                {
//            //                    CallModule("HIS.Desktop.Plugins.AssignPrescriptionYHCT", sendObj);
//            //                }
//            //            }
//            //            else if (currentSS.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM && IsCheckDepartmentTran())
//            //            {

//            //                HIS.Desktop.ADO.AssignBloodADO assignBloodADO = new HIS.Desktop.ADO.AssignBloodADO(currentSS.TDL_TREATMENT_ID ?? 0, 0, 0);
//            //                if (dtTreatment != null && dtTreatment.Count > 0)
//            //                {
//            //                    assignBloodADO.PatientDob = dtTreatment[0].TDL_PATIENT_DOB;
//            //                    assignBloodADO.PatientName = dtTreatment[0].TDL_PATIENT_NAME;
//            //                    assignBloodADO.GenderName = dtTreatment[0].TDL_PATIENT_GENDER_NAME;
//            //                }
//            //                List<object> sendObj = new List<object>() { assignBloodADO, dtServiceReq };
//            //                CallModule("HIS.Desktop.Plugins.HisAssignBlood", sendObj);
//            //            }
//            //            else if (IsCheckDepartmentTran())
//            //            {
//            //                AssignServiceEditADO assignServiceEditADO = new AssignServiceEditADO(currentSS.SERVICE_REQ_ID ?? 0, dtServiceReq[0].INTRUCTION_TIME, (HIS.Desktop.Common.RefeshReference)RefreshClick);
//            //                List<object> sendObj = new List<object>() { assignServiceEditADO };
//            //                CallModule("HIS.Desktop.Plugins.AssignServiceEdit", sendObj);
//            //            }
//            //        }
//            //    }
//            //    catch (Exception ex)
//            //    {
//            //        Inventec.Common.Logging.LogSystem.Error(ex);
//            //    }
//            //}

//            //private void RefreshClick()
//            //{
//            //    try
//            //    {
//            //        WaitingManager.Show();
//            //        LoadDataSereServByTreatmentId(this.rowClickByDate);
//            //        WaitingManager.Hide();
//            //    }
//            //    catch (Exception ex)
//            //    {
//            //        Inventec.Common.Logging.LogSystem.Error(ex);
//            //    }
//            //}

//            //private void FillDataApterSave(object prescription)
//            //{
//            //    try
//            //    {
//            //        if (prescription != null)
//            //        {
//            //            WaitingManager.Show();
//            //            LoadDataSereServByTreatmentId(this.rowClickByDate);
//            //            WaitingManager.Hide();
//            //        }
//            //    }
//            //    catch (Exception ex)
//            //    {
//            //        Inventec.Common.Logging.LogSystem.Error(ex);
//            //    }
//        }

//        private void Delete_Click(SereServADO data)
//        {
//            //    try
//            //    {
//            //        if (data != null && HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HAS_CONNECTION_EMR") == "1")
//            //        {
//            //            CommonParam paramEmr = new CommonParam();
//            //            CommonParam param = new CommonParam();
//            //            bool success = false;

//            //            EMR.Filter.EmrDocumentFilter filter = new EMR.Filter.EmrDocumentFilter();
//            //            filter.TREATMENT_CODE__EXACT = this.treatmentCode;
//            //            filter.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_ASSIGN;
//            //            var resultEmrDocument = new BackendAdapter(paramEmr).Get<List<EMR_DOCUMENT>>("api/EmrDocument/Get", ApiConsumers.EmrConsumer, filter, paramEmr);
//            //            if (resultEmrDocument != null && resultEmrDocument.Count() > 0)
//            //            {
//            //                resultEmrDocument = resultEmrDocument.Where(o => o.IS_DELETE != 1).ToList();
//            //                var checkServiceReqCode = "SERVICE_REQ_CODE:" + data.SERVICE_CODE;
//            //                var resultEmrDocumentLast = new List<EMR_DOCUMENT>();
//            //                foreach (var item in resultEmrDocument)
//            //                {
//            //                    if (item.HIS_CODE != null && item.HIS_CODE.Contains(checkServiceReqCode))
//            //                    {
//            //                        resultEmrDocumentLast.Add(item);
//            //                    }
//            //                }
//            //                if (resultEmrDocumentLast.Count() > 0 && resultEmrDocumentLast != null)
//            //                {
//            //                    #region
//            //                    if (MessageBox.Show(Resources.ResourceMessage.YLenhDaTonTaiVanBanKy, Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
//            //                    {
//            //                        WaitingManager.Show();
//            //                        MOS.SDO.HisServiceReqSDO sdoHisServiceReq = new MOS.SDO.HisServiceReqSDO();
//            //                        sdoHisServiceReq.Id = data.SERVICE_REQ_ID;
//            //                        //H
//            //                        //sdoHisServiceReq.RequestRoomId = this.currentModule.RoomId;
//            //                        success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisServiceReq/Delete", ApiConsumers.MosConsumer, sdoHisServiceReq, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
//            //                        WaitingManager.Hide();
//            //                        if (success == true)
//            //                        {
//            //                            var result = false;
//            //                            foreach (var item in resultEmrDocumentLast)
//            //                            {
//            //                                result = new BackendAdapter(paramEmr).Post<bool>("api/EmrDocument/Delete", ApiConsumers.EmrConsumer, item.ID, paramEmr);
//            //                            }
//            //                            MessageManager.Show(this.ParentForm, paramEmr, result);
//            //                            LoadDataSereServByTreatmentId(this.rowClickByDate);
//            //                        }
//            //                        else
//            //                        {
//            //                            MessageManager.Show(this.ParentForm, param, success);
//            //                        }

//            //                        #region Process has exception
//            //                        SessionManager.ProcessTokenLost(param);
//            //                        #endregion
//            //                    }
//            //                    #endregion
//            //                }
//            //                else
//            //                {
//            //                    ProcessDeleteServiceReq(data);
//            //                }
//            //            }
//            //            else
//            //            {
//            //                ProcessDeleteServiceReq(data);
//            //            }
//            //        }
//            //        else
//            //        {
//            //            ProcessDeleteServiceReq(data);
//            //        }
//            //    }
//            //    catch (Exception ex)
//            //    {
//            //        WaitingManager.Hide();
//            //        Inventec.Common.Logging.LogSystem.Error(ex);
//            //    }
//        }
//        //private void ProcessDeleteServiceReq(ADO.SereServADO data)
//        //{
//        //    try
//        //    {
//        //        WaitingManager.Show();
//        //        bool success = false;
//        //        CommonParam paramCommon = new CommonParam();
//        //        MOS.SDO.HisServiceReqSDO sdo = new MOS.SDO.HisServiceReqSDO();
//        //        sdo.Id = data.SERVICE_REQ_ID;
//        //        //H
//        //        //sdo.RequestRoomId = this.currentModule.RoomId;
//        //        //
//        //        success = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Post<bool>("api/HisServiceReq/Delete", ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
//        //        if (success)
//        //        {
//        //            LoadDataSereServByTreatmentId(this.rowClickByDate);
//        //            if (data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G)
//        //            {
//        //                FillDataToGridTreatmentBedRoom();
//        //            }
//        //        }
//        //        WaitingManager.Hide();
//        //        #region Show message
//        //        MessageManager.Show(this.ParentForm, paramCommon, success);
//        //        #endregion

//        //        #region Process has exception
//        //        SessionManager.ProcessTokenLost(paramCommon);
//        //        #endregion
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        WaitingManager.Hide();
//        //        Inventec.Common.Logging.LogSystem.Error(ex);
//        //    }
//        //}
//        //private void FillDataToGridTreatmentBedRoom()
//        //{
//        //    try
//        //    {
//        //        //if (ucPaging1.pagingGrid != null)
//        //        //{
//        //        //    pageSize = ucPaging1.pagingGrid.PageSize;
//        //        //}
//        //        //else
//        //        //{
//        //        //    pageSize = (int)ConfigApplications.NumPageSize;
//        //        //}
//        //        //FillDataToGridTreatment(new CommonParam(0, (int)pageSize));
//        //        //CommonParam param = new CommonParam();
//        //        //param.Limit = rowCount;
//        //        //param.Count = dataTotal;
//        //        //ucPaging1.Init(FillDataToGridTreatment, param, pageSize, gridControlTreatmentBedRoom);
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        Inventec.Common.Logging.LogSystem.Error(ex);
//        //    }
//        //}

//        //private void CallModule(string moduleLink, List<object> data)
//        //{
//        //    try
//        //    {
//        //        CallModule callModule = new CallModule(moduleLink, this.wkRoomId, this.wkRoomTypeId, data);

//        //        WaitingManager.Hide();
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        WaitingManager.Hide();
//        //        Inventec.Common.Logging.LogSystem.Error(ex);
//        //    }
//        //}
//        DHisSereServ2 TreeClickData;

//        private void treeView_Click(SereServADO data)
//        {
//            try
//            {
//                if (data != null)
//                {
//                    TreeClickData = data;
//                    if (TreeClickData != null && !String.IsNullOrWhiteSpace(TreeClickData.SERVICE_REQ_CODE))
//                    {
//                        ProcessLoadDocumentBySereServ(TreeClickData);
//                    }
//                    else
//                    {
//                        //this.ucViewEmrDocumentReq.ReloadDocument(new List<V_EMR_DOCUMENT>());
//                        //this.ucViewEmrDocumentResult.ReloadDocument(new List<V_EMR_DOCUMENT>());
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Inventec.Common.Logging.LogSystem.Error(ex);
//            }
//        }
//        private void ProcessLoadDocumentBySereServ(DHisSereServ2 data)
//        {
//            try
//            {
//                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HAS_CONNECTION_EMR") != "1")
//                    return;
//                //WaitingManager.Show();
//                //List<EmrDocumentFileSDO> listData = new List<EmrDocumentFileSDO>();
//                //if (data != null)
//                //{
//                //    string hisCode = "SERVICE_REQ_CODE:" + data.SERVICE_REQ_CODE;
//                //    CommonParam paramCommon = new CommonParam();
//                //    listData = GetEmrDocumentFile(hisCode, true, null, null, ref paramCommon);
//                //    if (listData != null && listData.Count > 0)
//                //    {
//                //        listData = listData.Where(o => o.Extension.ToLower().Equals("pdf")).ToList();
//                //    }
//                //}

//                //if (xtraTabDocument.SelectedTabPage == xtraTabDocumentReq)
//                //{
//                //    this.ucViewEmrDocumentReq.ReloadDocument(listData);
//                //}
//                //else if (xtraTabDocument.SelectedTabPage == xtraTabDocumentResult)
//                //{
//                //    this.ucViewEmrDocumentResult.ReloadDocument(listData);
//                //}
//                WaitingManager.Hide();
//            }
//            catch (Exception ex)
//            {
//                WaitingManager.Hide();
//                Inventec.Common.Logging.LogSystem.Error(ex);
//            }
//        }
//        //private bool IsCheckDepartmentTran()
//        //{
//        //    bool result = true;
//        //    try
//        //    {
//        //        if (RowCellClickBedRoom != null)
//        //        {
//        //            CommonParam param = new CommonParam();
//        //            MOS.Filter.HisDepartmentTranFilter _departmentTranFilter = new HisDepartmentTranFilter();
//        //            _departmentTranFilter.TREATMENT_ID = RowCellClickBedRoom.TREATMENT_ID;
//        //            //H
//        //            //_departmentTranFilter.DEPARTMENT_ID = BackendDataWorker.Get<HIS_ROOM>().FirstOrDefault(p => p.ID == currentModule.RoomId).DEPARTMENT_ID;
//        //            //
//        //            var _DepartmentTrans = new BackendAdapter(param).Get<List<HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/Get", ApiConsumers.MosConsumer, _departmentTranFilter, param);
//        //            if (_DepartmentTrans != null && _DepartmentTrans.Count > 0)
//        //            {
//        //                long from = 0;
//        //                long to = 0;
//        //                //if (dtFrom.EditValue != null && dtFrom.DateTime != DateTime.MinValue)
//        //                //{
//        //                //    from = Convert.ToInt64(dtFrom.DateTime.ToString("yyyyMMdd") + "000000");
//        //                //}
//        //                //if (dtTo.EditValue != null && dtTo.DateTime != DateTime.MinValue)
//        //                //{
//        //                //    to = Convert.ToInt64(dtTo.DateTime.ToString("yyyyMMdd") + "235959");
//        //                //}
//        //                var department = from != 0 && to != 0 ? _DepartmentTrans.Where(o => from <= (o.DEPARTMENT_IN_TIME ?? 0) && (o.DEPARTMENT_IN_TIME ?? 0) <= to).ToList().FirstOrDefault() : _DepartmentTrans.OrderByDescending(o => o.DEPARTMENT_IN_TIME ?? 0).ToList().FirstOrDefault();
//        //                if (department != null)
//        //                {
//        //                    if (department.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE && DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("{0} đã khóa chi phí không cho phép thực hiện chỉ định, kê đơn", BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == department.DEPARTMENT_ID).DEPARTMENT_NAME),
//        //               HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongThongBaoTieuDeChoWaitDialogForm),
//        //               MessageBoxButtons.OK) == DialogResult.OK)
//        //                        return false;
//        //                }
//        //            }
//        //            if (string.IsNullOrEmpty(RowCellClickBedRoom.BED_CODE))
//        //            {
//        //                var treatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == RowCellClickBedRoom.TDL_TREATMENT_TYPE_ID).IS_REQUIRED_SERVICE_BED == 1;
//        //                if (treatmentType)
//        //                {
//        //                    DevExpress.XtraEditors.XtraMessageBox.Show("Bệnh nhân chưa chỉ định giường không cho phép kê đơn, chỉ định",
//        //               HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongThongBaoTieuDeChoWaitDialogForm));
//        //                    return false;
//        //                }
//        //            }
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        Inventec.Common.Logging.LogSystem.Error(ex);
//        //    }
//        //    return result;
//        //}

//        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
//        {
//            try
//            {
//                btnSave.PerformClick();
//            }
//            catch (Exception ex)
//            {
//                Inventec.Common.Logging.LogSystem.Error(ex);
//            }
//        }

//        private void btnSave_Click(object sender, EventArgs e)
//        {
//            try
//            {
//                if (!btnSave.Enabled)
//                {
//                    return;
//                }

//                //
//            }
//            catch (Exception ex)
//            {cái dịch vụ kia bốc nguyên của a hùng vào
//                Inventec.Common.Logging.LogSystem.Error(ex);
//            }
//        }
//    }
//}

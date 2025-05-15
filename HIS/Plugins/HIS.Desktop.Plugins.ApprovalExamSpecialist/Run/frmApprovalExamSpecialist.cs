using HIS.Desktop.Plugins.ApprovalExamSpecialist.ValidateRule;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common;
using Inventec.Core;
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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.ApprovalExamSpecialist.Base;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.ApprovalExamSpecialist.ADO;
using HIS.Desktop.LocalStorage.HisConfig;
using EMR.SDO;
using EMR.EFMODEL.DataModels;
using DevExpress.XtraTreeList;
namespace HIS.Desktop.Plugins.ApprovalExamSpecialist.Run
{
    public partial class frmApprovalExamSpecialist : FormBase
    {
        MOS.EFMODEL.DataModels.HIS_TRACKING currentVHisTracking = null;
        MOS.EFMODEL.DataModels.HIS_SPECIALIST_EXAM currentVHisSpecialist = null;
        private Inventec.Desktop.Common.Modules.Module currentModule;
        internal ServiceReqGroupByDateADO rowClickByDate { get; set; }
        internal long treatmentID;
        internal string treatmentCode;
        bool IsExpandList = true;

        long wkRoomId { get; set; }

        long wkRoomTypeId = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int start = 0;
        int limit = 0;
        int pageSize = 0;
        int pageIndex = 0;

        int lastRowHandle = -1;

        DHisSereServ2 TreeClickData;
        UCTreeListService ucCDHA, ucXN, ucDichVu, ucSieuAm, ucPhauThuat, ucGiaiPhau;

        V_HIS_SPECIALIST_EXAM currentSpecialistExam;    
        public frmApprovalExamSpecialist()
            : base(null)
        {
            InitializeComponent();
        }
        public frmApprovalExamSpecialist(Inventec.Desktop.Common.Modules.Module currentModule, long treatmentID)
           : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.treatmentID = treatmentID;
                this.currentModule = currentModule;
                this.wkRoomId = this.currentModule != null ? this.currentModule.RoomId : 0;
                this.wkRoomTypeId = this.currentModule != null ? this.currentModule.RoomTypeId : 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void frmApprovalExamSpecialist_Load(object sender, EventArgs e)
        {
            try
            {
                treeSereServ.StateImageList = imageCollection;
                AddUc();
                gridViewTreatment.FocusedRowHandle = -1;
                this.currentSpecialistExam = BackendDataWorker.Get<V_HIS_SPECIALIST_EXAM>().FirstOrDefault(o => o.TREATMENT_ID == this.currentModule.RoomId);
                SetDefaultValueControl();
                SetValidateRule();
                this.FillDataToGridTreatmentSpeacialist();

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            dtTrackingTime.DateTime = DateTime.Now;
            txtNoiDungKham.Text = "";
            txtYLenhKham.Text = "";
        }
        private void AddUc()
        {
            try
            {
                ucCDHA = new UCTreeListService(imageCollection1, currentModule);
                ucXN = new UCTreeListService(imageCollection1, currentModule);
                ucDichVu = new UCTreeListService(imageCollection1, currentModule);
                ucSieuAm = new UCTreeListService(imageCollection1, currentModule);
                ucPhauThuat = new UCTreeListService(imageCollection1, currentModule);
                ucGiaiPhau = new UCTreeListService(imageCollection1, currentModule);

                panelControl2.Controls.Add(ucCDHA);
                ucCDHA.Dock = DockStyle.Fill;

                panelControl3.Controls.Add(ucXN);
                ucXN.Dock = DockStyle.Fill;

                panelControl4.Controls.Add(ucDichVu);
                ucDichVu.Dock = DockStyle.Fill;

                panelControl5.Controls.Add(ucSieuAm);
                ucSieuAm.Dock = DockStyle.Fill;

                panelControl6.Controls.Add(ucPhauThuat);
                ucPhauThuat.Dock = DockStyle.Fill;

                panelControl7.Controls.Add(ucGiaiPhau);
                ucGiaiPhau.Dock = DockStyle.Fill;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetValidateRule()
        {
            ValidateMaxLength validateMaxLengthNoiDung = new ValidateMaxLength();
            validateMaxLengthNoiDung.textEdit = txtNoiDungKham;
            validateMaxLengthNoiDung.maxLength = 4000;
            validateMaxLengthNoiDung.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            validateMaxLengthNoiDung.isValidNull = true;
            dxValidationProviderEditorInfo.SetValidationRule(txtNoiDungKham, validateMaxLengthNoiDung);

            ValidateMaxLength validateMaxLengthYLenh = new ValidateMaxLength();
            validateMaxLengthYLenh.textEdit = txtYLenhKham;
            validateMaxLengthYLenh.maxLength = 4000;
            validateMaxLengthYLenh.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            validateMaxLengthYLenh.isValidNull = true;
            dxValidationProviderEditorInfo.SetValidationRule(txtYLenhKham, validateMaxLengthYLenh);

        }
        private void FillDataToGridTreatmentSpeacialist()
        {
            try
            {
                //Đã gọi hàm này chó đâu :)))))
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }
                FillDataToGridTreatment(new CommonParam(0, (int)pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridTreatment, param, pageSize, gridControl1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FillDataToGridTreatment(object param)
        {
            try
            {
                WaitingManager.Show();
                gridControl1.DataSource = null;
                this.pageIndex = 0;
                int start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                HisTrackingFilter trackingFilter = new HisTrackingFilter
                {
                    TREATMENT_ID = treatmentID
                };
                var trackings = new BackendAdapter(paramCommon).Get<List<HIS_TRACKING>>(
                    HisRequestUriStore.HIS_TRACKING_GET,
                    ApiConsumers.MosConsumer, trackingFilter, paramCommon
                ) ?? new List<HIS_TRACKING>();
                var empList = BackendDataWorker.Get<V_HIS_EMPLOYEE>()
                    .Where(e => e.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .ToList();
                var empDict = empList
                    .Where(e => !string.IsNullOrEmpty(e.LOGINNAME))
                    .ToDictionary(e => e.LOGINNAME, e => e);
                var sereServFilter = new MOS.Filter.HisSereServFilter
                {
                    TREATMENT_ID = treatmentID
                };
                var DHisSereServ2 = new BackendAdapter(paramCommon).Get<List<DHisSereServ2>>(
                    UriApi.HIS_SERE_SERV_2_GET,
                    ApiConsumers.MosConsumer, sereServFilter, paramCommon
                ) ?? new List<DHisSereServ2>();
                var displayList = (from t in trackings
                                   orderby t.TRACKING_TIME
                                   select new
                                   {
                                       DATE_TIME = t.TRACKING_TIME.ToString("dd/MM/yyyy HH:mm"),
                                       DOCTOR = empDict.ContainsKey(t.CREATOR)
                                           ? string.Format("{0} - {1}", empDict[t.CREATOR].DIPLOMA, empDict[t.CREATOR].TDL_USERNAME)
                                           : "",
                                       PROGRESS = t.CONTENT,
                                       MEDICAL_ORDER = string.Join("\n", DHisSereServ2
                                           .Where(s => s.TRACKING_ID == t.ID)
                                           .Select(s => string.Format("{0}: - {1} x {2} {3}",
                                               s.SERVICE_REQ_CODE,
                                               s.SERVICE_NAME,
                                               s.AMOUNT,
                                               s.SERVICE_UNIT_NAME)))
                                   }).ToList();
                gridControl1.BeginUpdate();
                gridControl1.DataSource = displayList;
                gridControl1.EndUpdate();

                gridViewTreatment.OptionsSelection.EnableAppearanceFocusedCell = false;
                gridViewTreatment.OptionsSelection.EnableAppearanceFocusedRow = false;
                gridViewTreatment.BestFitColumns();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
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
                    else
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void RefreshClick()
        {
            try
            {
                WaitingManager.Show();
                LoadDataSereServByTreatmentId(this.rowClickByDate);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FillDataApterSave(object prescription)
        {
            try
            {
                if (prescription != null)
                {
                    WaitingManager.Show();
                    LoadDataSereServByTreatmentId(this.rowClickByDate);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ProcessLoadDocumentBySereServ(DHisSereServ2 data)
        {
            try
            {
                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HAS_CONNECTION_EMR") != "1")
                    return;
                WaitingManager.Show();
                List<EmrDocumentFileSDO> listData = new List<EmrDocumentFileSDO>();
                if (data != null)
                {
                    string hisCode = "SERVICE_REQ_CODE:" + data.SERVICE_REQ_CODE;
                    CommonParam paramCommon = new CommonParam();
                    listData = GetEmrDocumentFile(hisCode, true, null, null, ref paramCommon);
                    if (listData != null && listData.Count > 0)
                    {
                        listData = listData.Where(o => o.Extension.ToLower().Equals("pdf")).ToList();
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private List<EmrDocumentFileSDO> GetEmrDocumentFile(string hiscode, bool? IsMerge, bool? IsShowPatientSign, bool? IsShowWatermark, ref CommonParam paramCommon)
        {
            EmrDocumentDownloadFileSDO sdo = new EmrDocumentDownloadFileSDO();
            var emrFilter = new EMR.Filter.EmrDocumentViewFilter();
            emrFilter.IS_DELETE = false;

            sdo.HisCode = hiscode;
            sdo.EmrDocumentViewFilter = emrFilter;
            sdo.IsMerge = IsMerge;
            sdo.IsShowPatientSign = IsShowPatientSign;
            sdo.IsShowWatermark = IsShowWatermark;
            var roomWorking = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentModule.RoomId);
            sdo.RoomCode = roomWorking != null ? roomWorking.ROOM_CODE : null;
            sdo.DepartmentCode = roomWorking != null ? roomWorking.DEPARTMENT_CODE : null;
            return new BackendAdapter(paramCommon).Post<List<EmrDocumentFileSDO>>("api/EmrDocument/DownloadFile", ApiConsumers.EmrConsumer, sdo, paramCommon);
        }

        private void LoadDataSereServByTreatmentId(ServiceReqGroupByDateADO currentHisServiceReq)
        {
            try
            {
                List<SereServADO> SereServADOs = new List<SereServADO>();
                List<DHisSereServ2> dataNew = new List<DHisSereServ2>();
                List<HIS_SERVICE_REQ> dataServiceReq = new List<HIS_SERVICE_REQ>();
                WaitingManager.Show();
                if (currentHisServiceReq != null && currentHisServiceReq.TreatmentId > 0)
                {
                    CommonParam param = new CommonParam();
                    DHisSereServ2Filter _sereServ2Filter = new DHisSereServ2Filter();
                    _sereServ2Filter.TREATMENT_ID = currentHisServiceReq.TreatmentId;
                    _sereServ2Filter.INTRUCTION_DATE = Int64.Parse(currentHisServiceReq.InstructionDate.ToString().Substring(0, 8) + "000000");
                    dataNew = new BackendAdapter(param).Get<List<DHisSereServ2>>("api/HisSereServ/GetDHisSereServ2", ApiConsumers.MosConsumer, _sereServ2Filter, param);
                    if (dataNew != null && dataNew.Count > 0)
                    {

                        if (!currentHisServiceReq.isParent)
                        {
                            dataNew = dataNew.Where(o => o.TRACKING_ID == currentHisServiceReq.TRACKING_ID).ToList();
                        }
                        HisServiceReqFilter filter = new HisServiceReqFilter();
                        filter.IDs = dataNew.Select(o => o.SERVICE_REQ_ID ?? 0).ToList();
                        dataServiceReq = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, param);
                        var listRootByType = dataNew.OrderByDescending(o => o.TRACKING_TIME).GroupBy(o => o.TDL_SERVICE_TYPE_ID).ToList();
                        var department = currentModule != null ? BackendDataWorker.Get<HIS_ROOM>().FirstOrDefault(p => p.ID == currentModule.RoomId) : null;
                        var departmentId = department != null ? department.DEPARTMENT_ID : 0;
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

                WaitingManager.Hide();
                if (SereServADOs != null && SereServADOs.Count > 0)
                {
                    SereServADOs = SereServADOs.OrderBy(o => o.PARENT_ID__IN_SETY).ThenBy(p => p.SERVICE_CODE).ThenBy(o => o.SERVICE_NAME).ToList();

                    #region CDHA

                    List<SereServADO> listCLS = new List<SereServADO>();
                    listCLS.AddRange(SereServADOs.Where(
                        o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA
                        ));

                    ucCDHA.ReLoad(treeView_Click, listCLS);

                    #endregion

                    #region XN

                    List<SereServADO> listXN = new List<SereServADO>();
                    listXN.AddRange(SereServADOs.Where(
                        o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN
                        ));

                    ucXN.ReLoad(treeView_Click, listXN);

                    #endregion

                    #region PTTT

                    List<SereServADO> listPTTT = new List<SereServADO>();
                    listPTTT.AddRange(SereServADOs.Where(
                        o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN
                        ));

                    ucXN.ReLoad(treeView_Click, listPTTT);

                    #endregion

                    #region Service

                    List<SereServADO> listMediMate = new List<SereServADO>();
                    listMediMate.AddRange(SereServADOs.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU
                        ));

                    ucDichVu.ReLoad(treeView_Click, listMediMate);

                    #endregion

                    #region GP

                    List<SereServADO> listGP = new List<SereServADO>();
                    listGP.AddRange(SereServADOs.Where(
                        o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL
                        ));

                    ucGiaiPhau.ReLoad(treeView_Click, listGP);

                    #endregion

                    #region SA,NS

                    List<SereServADO> listSANS = new List<SereServADO>();
                    listSANS.AddRange(SereServADOs.Where(
                        o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS
                        ));

                    ucGiaiPhau.ReLoad(treeView_Click, listSANS);

                    #endregion

                    #region reloadTabControl
                    IsExpandList = true;                                     

                    xtraTabControl1.SelectedTabPage = xtraTabControl1.TabPages[3];
                    xtraTabControl1.SelectedTabPage = xtraTabControl1.TabPages[2];
                    xtraTabControl1.SelectedTabPage = xtraTabControl1.TabPages[1];
                    xtraTabControl1.SelectedTabPage = xtraTabControl1.TabPages[0];
                    #endregion

                }
                else
                {
                    ucCDHA.ReLoad(treeView_Click, null);
                    ucXN.ReLoad(treeView_Click, null);
                    ucDichVu.ReLoad(treeView_Click, null);
                    ucSieuAm.ReLoad(treeView_Click, null);
                    ucPhauThuat.ReLoad(treeView_Click, null);
                    ucGiaiPhau.ReLoad(treeView_Click, null);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                HIS_SPECIALIST_EXAM exam = new HIS_SPECIALIST_EXAM();
                HIS_TRACKING tracking = new HIS_TRACKING();

                //bool isSpecialistExamSaved = ProcessSaveSpecialistExam(ref exam);
                //if (!isSpecialistExamSaved)
                //{
                //    MessageBox.Show("Lưu thông tin khám chuyên khoa không thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}

                bool isTrackingSaved = ProcessSaveTracking(ref tracking);
                if (!isTrackingSaved)
                {
                    MessageBox.Show("Lưu thông tin theo dõi không thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                MessageBox.Show("Lưu thông tin thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                MessageBox.Show("Có lỗi xảy ra khi lưu thông tin.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ProcessSaveSpecialistExam(ref HIS_SPECIALIST_EXAM exam)
        {
            try
            {
                if (currentVHisSpecialist == null)
                    return false;

                CommonParam param = new CommonParam();
                HisSpecialistExamFilter filter = new HisSpecialistExamFilter
                {
                    ID = currentVHisSpecialist.ID
                };

                var examList = new BackendAdapter(param).Get<List<HIS_SPECIALIST_EXAM>>(
                    "api/HisSpecialistExam/Get", ApiConsumers.MosConsumer, filter, param);

                if (examList != null && examList.Count > 0)
                {
                    HIS_SPECIALIST_EXAM hisSpecialistResult = examList.First();

                    dtTrackingTime.DateTime = hisSpecialistResult.EXAM_TIME > 0
                        ? Convert.ToDateTime(hisSpecialistResult.EXAM_TIME)
                        : DateTime.Now;

                    txtNoiDungKham.Text = hisSpecialistResult.EXAM_EXECUTE_CONTENT ?? "";
                    txtYLenhKham.Text = hisSpecialistResult.EXAM_EXCUTE ?? "";
                }

                // Lấy thông tin từ giao diện để lưu
                exam.EXAM_EXECUTE_CONTENT = txtNoiDungKham.Text?.Trim();
                exam.EXAM_EXCUTE = txtYLenhKham.Text?.Trim();
                exam.EXAM_TIME = dtTrackingTime.DateTime.Ticks;
                exam.IS_APPROVAL = 1;

                // Gọi API để lưu
                exam = new BackendAdapter(param).Post<HIS_SPECIALIST_EXAM>(
                    UriApi.HIS_SPEACIALIST_EXAM_CREATE,
                    ApiConsumers.MosConsumer,
                    exam,
                    param);

                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private bool ProcessSaveTracking(ref HIS_TRACKING tracking)
        {
            try
            {
                if (currentVHisTracking == null)
                    return false;

                CommonParam param = new CommonParam();
                HisTrackingFilter filter = new HisTrackingFilter
                {
                    ID = currentVHisTracking.ID
                };

                var trackingList = new BackendAdapter(param).Get<List<HIS_TRACKING>>(
                    "api/HisTracking/Get", ApiConsumers.MosConsumer, filter, param);

                if (trackingList != null && trackingList.Count > 0)
                {
                    HIS_TRACKING hisTrackingResult = trackingList.First();

                    dtTrackingTime.DateTime = hisTrackingResult.TRACKING_TIME > 0
                        ? Convert.ToDateTime(hisTrackingResult.TRACKING_TIME)
                        : DateTime.Now;

                    txtNoiDungKham.Text = hisTrackingResult.CONTENT ?? "";
                    txtYLenhKham.Text = hisTrackingResult.CONTENT ?? "";
                }

                // Lấy thông tin từ giao diện để lưu
                tracking.CONTENT = txtNoiDungKham.Text?.Trim();
                tracking.CONTENT = txtYLenhKham.Text?.Trim();
                tracking.TRACKING_TIME = dtTrackingTime.DateTime.Ticks;

                // Gọi API để lưu
                tracking = new BackendAdapter(param).Post<HIS_TRACKING>(
                    HisRequestUriStore.HIS_TRACKING_CREATE,
                    ApiConsumers.MosConsumer,
                    tracking,
                    param);

                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }
    }
}

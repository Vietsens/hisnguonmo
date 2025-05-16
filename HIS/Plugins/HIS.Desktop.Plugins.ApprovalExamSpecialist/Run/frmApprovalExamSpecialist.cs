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
using DevExpress.XtraTab;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
namespace HIS.Desktop.Plugins.ApprovalExamSpecialist.Run
{
    public partial class frmApprovalExamSpecialist : FormBase
    {
        MOS.EFMODEL.DataModels.HIS_TRACKING currentVHisTracking = null;
        MOS.EFMODEL.DataModels.HIS_SPECIALIST_EXAM currentVHisSpecialist = null;
        internal L_HIS_TREATMENT_BED_ROOM RowCellClickBedRoom { get; set; }
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
        public frmApprovalExamSpecialist(Inventec.Desktop.Common.Modules.Module currentModule, long treatmentID, V_HIS_SPECIALIST_EXAM currentSpecialistExam)
           : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.treatmentID = treatmentID;
                this.currentSpecialistExam = currentSpecialistExam;
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
                AddUc();
                gridViewTreatment.FocusedRowHandle = -1;
                SetDefaultValueControl();
                SetValidateRule();
                FillDataToGridTreatmentSpeacialist();

                //string jsonString = @"{""ADD_TIME_STR"":""12/09/2024 10:32:09"",""PATIENT_CLASSIFY_NAME"":""Màu tím"",""DISPLAY_COLOR"":""192, 0, 192"",""ID"":6707,""TREATMENT_ID"":152715,""CO_TREATMENT_ID"":2182,""ADD_TIME"":20240912103209,""BED_ROOM_ID"":284,""REMOVE_TIME"":null,""TREATMENT_ROOM_ID"":null,""TDL_OBSERVED_TIME_FROM"":null,""TDL_OBSERVED_TIME_TO"":null,""PATIENT_ID"":127997,""TREATMENT_CODE"":""000000152638"",""TDL_PATIENT_FIRST_NAME"":""1"",""TDL_PATIENT_LAST_NAME"":""TK QUẢNG"",""TDL_PATIENT_NAME"":""TK QUẢNG 1 "",""TDL_PATIENT_DOB"":20040215000000,""TDL_PATIENT_GENDER_NAME"":""Nam"",""TDL_PATIENT_CODE"":""0000127858"",""TDL_PATIENT_ADDRESS"":""34 Trung Kính, Phường Yên Hoà, Quận Cầu Giấy, Hà Nội"",""TDL_HEIN_CARD_NUMBER"":null,""TDL_HEIN_MEDI_ORG_CODE"":null,""ICD_CODE"":""A00"",""ICD_NAME"":""Bệnh tả"",""ICD_TEXT"":null,""ICD_SUB_CODE"":null,""TDL_PATIENT_GENDER_ID"":2,""TDL_HEIN_MEDI_ORG_NAME"":null,""IS_PAUSE"":null,""IS_APPROVE_FINISH"":null,""APPROVE_FINISH_NOTE"":null,""TDL_PATIENT_CLASSIFY_ID"":2,""TDL_TREATMENT_TYPE_ID"":3,""EMR_COVER_TYPE_ID"":12,""CLINICAL_IN_TIME"":20240912100800,""CO_TREAT_DEPARTMENT_IDS"":""22"",""OUT_TIME"":null,""TDL_PATIENT_AVATAR_URL"":null,""LAST_DEPARTMENT_ID"":36,""TDL_PATIENT_UNSIGNED_NAME"":""TK QUANG 1 "",""TREATMENT_END_TYPE_ID"":null,""TREATMENT_METHOD"":""phương pháp điều trị"",""TDL_PATIENT_PHONE"":null,""TDL_HEIN_CARD_FROM_TIME"":null,""TDL_HEIN_CARD_TO_TIME"":null,""TDL_PATIENT_CCCD_NUMBER"":null,""TDL_PATIENT_CMND_NUMBER"":null,""TDL_PATIENT_PASSPORT_NUMBER"":null,""TDL_PATIENT_MOBILE"":null,""DOCTOR_LOGINNAME"":""quangln"",""DOCTOR_USERNAME"":""Lương Ngọc Quảng"",""PATIENT_TYPE_NAME"":""Khám Đích Danh Và Tái Khám"",""BED_NAME"":""Gường xịn"",""BED_CODE"":""G1"",""PATIENT_TYPE_CODE"":""05"",""BED_ROOM_NAME"":""buồng nội trú 2"",""TREATMENT_ROOM_CODE"":null,""TREATMENT_ROOM_NAME"":null,""LAST_DEPARTMENT_CODE"":""2"",""LAST_DEPARTMENT_NAME"":""Khoa Răng Hàm Mặt"",""NOTE"":null}";


                //RowCellClickBedRoom = Newtonsoft.Json.JsonConvert.DeserializeObject<L_HIS_TREATMENT_BED_ROOM>(jsonString);
                if (this.currentSpecialistExam != null)
                {
                    LoadDataSereServByTreatmentId(this.currentSpecialistExam);
                }


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

                pcCDHA.Controls.Add(ucCDHA);
                ucCDHA.Dock = DockStyle.Fill;

                pcXN.Controls.Add(ucXN);
                ucXN.Dock = DockStyle.Fill;

                pcService.Controls.Add(ucDichVu);
                ucDichVu.Dock = DockStyle.Fill;

                pcSANS.Controls.Add(ucSieuAm);
                ucSieuAm.Dock = DockStyle.Fill;

                pcPTTT.Controls.Add(ucPhauThuat);
                ucPhauThuat.Dock = DockStyle.Fill;

                pcGP.Controls.Add(ucGiaiPhau);
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
                //quên :)) thagndq cái này liên quan gì đến thắng a, k truyền vào thì chả null
                HisTrackingFilter trackingFilter = new HisTrackingFilter
                {
                    TREATMENT_ID = currentSpecialistExam.TREATMENT_ID
                };
                List<HIS_TRACKING> trackings = new BackendAdapter(paramCommon).Get<List<HIS_TRACKING>>(
                    HisRequestUriStore.HIS_TRACKING_GET,
                    ApiConsumers.MosConsumer, trackingFilter, paramCommon
                );

                List<V_HIS_EMPLOYEE> empList = BackendDataWorker.Get<V_HIS_EMPLOYEE>()
                    .Where(e => e.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .ToList();
                Dictionary<string, V_HIS_EMPLOYEE> empDict = new Dictionary<string, V_HIS_EMPLOYEE>();
                foreach (var e in empList)
                {
                    if (!string.IsNullOrEmpty(e.LOGINNAME) && !empDict.ContainsKey(e.LOGINNAME))
                        empDict.Add(e.LOGINNAME, e);
                }

                MOS.Filter.HisSereServFilter sereServFilter = new MOS.Filter.HisSereServFilter
                {
                    TREATMENT_ID = currentSpecialistExam.TREATMENT_ID
                };
                List<DHisSereServ2> sereServList = new BackendAdapter(paramCommon).Get<List<DHisSereServ2>>(
                    UriApi.HIS_SERE_SERV_2_GET,
                    ApiConsumers.MosConsumer, sereServFilter, paramCommon
                );

                List<TreatmentNoteADO> noteList = new List<TreatmentNoteADO>();
                foreach (var tracking in trackings.OrderBy(t => t.TRACKING_TIME))
                {
                    V_HIS_EMPLOYEE emp = null;
                    if (!string.IsNullOrEmpty(tracking.CREATOR) && empDict.ContainsKey(tracking.CREATOR))
                    {
                        emp = empDict[tracking.CREATOR];
                    }
                    var SServ = sereServList.Where(o => o.TRACKING_ID == tracking.ID).ToList();
                    TreatmentNoteADO note = new TreatmentNoteADO(tracking, emp, SServ);
                    noteList.Add(note);
                }
                //Lại đi
                gridControl1.BeginUpdate();
                gridControl1.DataSource = noteList;
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
        private void LoadDataSereServByTreatmentId(V_HIS_SPECIALIST_EXAM currentHisServiceReq)
        {
            try
            {
                foreach (XtraTabPage item in this.xtraTabControl1.TabPages)
                {
                    item.PageVisible = false;
                }
                List<SereServADO> SereServADOs = new List<SereServADO>();
                List<DHisSereServ2> dataNew = new List<DHisSereServ2>();
                List<HIS_SERVICE_REQ> dataServiceReq = new List<HIS_SERVICE_REQ>();
                WaitingManager.Show();
                if (currentHisServiceReq != null && currentHisServiceReq.TREATMENT_ID > 0)
                {
                    CommonParam param = new CommonParam();
                    DHisSereServ2Filter _sereServ2Filter = new DHisSereServ2Filter();
                    _sereServ2Filter.TREATMENT_ID = currentHisServiceReq.TREATMENT_ID;
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

                    ucCDHA.ReLoad(treeView_Click, listCLS, this.currentSpecialistExam);

                    #endregion

                    #region XN

                    List<SereServADO> listXN = new List<SereServADO>();
                    listXN.AddRange(SereServADOs.Where(
                        o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN
                        ));

                    ucXN.ReLoad(treeView_Click, listXN, this.currentSpecialistExam);

                    #endregion

                    #region PTTT
                    //Quần què gì đây pttt a, sao lạ load vào tab xét nghiệm :)))                   
                    List<SereServADO> listPTTT = new List<SereServADO>();
                    listPTTT.AddRange(SereServADOs.Where(
                        o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN
                        ));

                    ucPhauThuat.ReLoad(treeView_Click, listPTTT, this.currentSpecialistExam);

                    #endregion

                    #region Service

                    List<SereServADO> listMediMate = new List<SereServADO>();
                    listMediMate.AddRange(SereServADOs.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU
                        ));

                    ucDichVu.ReLoad(treeView_Click, listMediMate, this.currentSpecialistExam);

                    #endregion

                    #region GP

                    List<SereServADO> listGP = new List<SereServADO>();
                    listGP.AddRange(SereServADOs.Where(
                        o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL
                        ));

                    ucGiaiPhau.ReLoad(treeView_Click, listGP, this.currentSpecialistExam);

                    #endregion

                    #region SA,NS

                    List<SereServADO> listSANS = new List<SereServADO>();
                    listSANS.AddRange(SereServADOs.Where(
                        o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS
                        ));

                    ucSieuAm.ReLoad(treeView_Click, listSANS, this.currentSpecialistExam);

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
                    ucCDHA.ReLoad(treeView_Click, null, this.currentSpecialistExam);
                    ucXN.ReLoad(treeView_Click, null, this.currentSpecialistExam);
                    ucDichVu.ReLoad(treeView_Click, null, this.currentSpecialistExam);
                    ucSieuAm.ReLoad(treeView_Click, null, this.currentSpecialistExam);
                    ucPhauThuat.ReLoad(treeView_Click, null, this.currentSpecialistExam);
                    ucGiaiPhau.ReLoad(treeView_Click, null, this.currentSpecialistExam);
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
            //try
            //{
            //    WaitingManager.Show();
            //    CommonParam param = new CommonParam();
            //    currentVHisSpecialist.EXAM_TIME = dtTrackingTime.D
            //    currentVHisSpecialist.EXAM_EXECUTE_USERNAME = cboEmployee.EditValue?.ToString();
            //    currentVHisSpecialist.EXAM_EXECUTE_CONTENT = txtYKienBacSi.Text.Trim();
            //    currentVHisSpecialist.IS_APPROVAL = 1;
            //    var rs = new BackendAdapter(param).Post<HIS_INFUSION_SUM>("api/HisSpecialistExam/Update", ApiConsumers.MosConsumer, currentVHisSpecialist, param);

            //    MessageManager.Show(this, param, rs != null);
            //    SessionManager.ProcessTokenLost(param);
            //    WaitingManager.Hide();
            //}
            //catch (Exception ex)
            //{
            //    WaitingManager.Hide();
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}

        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ApprovalExamSpecialist.Resources.Lang", typeof(frmApprovalExamSpecialist).Assembly);
                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmApprovalExamSpecialist.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmApprovalExamSpecialist.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridControl1.Text = Inventec.Common.Resource.Get.Value("frmApprovalExamSpecialist.tabToDieuTri.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.tabAll.Text = Inventec.Common.Resource.Get.Value("frmApprovalExamSpecialist.tabAll.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.pcXN.Text = Inventec.Common.Resource.Get.Value("frmApprovalExamSpecialist.pcXN.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.pcService.Text = Inventec.Common.Resource.Get.Value("frmApprovalExamSpecialist.pcService.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.pcSANS.Text = Inventec.Common.Resource.Get.Value("frmApprovalExamSpecialist.pcSANS.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.pcPTTT.Text = Inventec.Common.Resource.Get.Value("frmApprovalExamSpecialist.pcPTTT.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.pcGP.Text = Inventec.Common.Resource.Get.Value("frmApprovalExamSpecialist.pcGP.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmApprovalExamSpecialist.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmApprovalExamSpecialist.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmApprovalExamSpecialist.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}

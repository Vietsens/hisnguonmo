using DevExpress.Entity.Model.Metadata;
using DevExpress.XtraTab;
using EMR.SDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ApprovalExamSpecialist.ADO;
using HIS.Desktop.Plugins.ApprovalExamSpecialist.Base;
using HIS.Desktop.Plugins.ApprovalExamSpecialist.ValidateRule;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.SignLibrary.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
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
        private Common.RefeshReference delegateRefresher;
        public frmApprovalExamSpecialist()
            : base(null)
        {
            InitializeComponent();
        }
        public frmApprovalExamSpecialist(Inventec.Desktop.Common.Modules.Module currentModule, long treatmentID, V_HIS_SPECIALIST_EXAM currentSpecialistExam, Common.RefeshReference delegateRefresh)
           : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.delegateRefresher = delegateRefresh;

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
                this.KeyPreview = true;
                this.ActiveControl = gridControl1;
                this.SetCaptionByLanguageKey();
                this.InitComboDoctor();
                AddUc();
                gridViewTreatment.FocusedRowHandle = -1;
                SetDefaultValueControl();
                FillDataToGridTreatment();
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
            try
            {

                if (currentSpecialistExam.EXAM_TIME.HasValue)
                {
                    dtTrackingTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)currentSpecialistExam.EXAM_TIME); 
                    
                }
                else
                {
                    dtTrackingTime.DateTime = DateTime.Now;
                }
                txtNoiDungKham.Text = currentSpecialistExam.EXAM_EXECUTE_CONTENT;
                txtYLenhKham.Text = currentSpecialistExam.EXAM_EXCUTE;
                this.cboDoctor.EditValue = this.currentSpecialistExam.EXAM_EXECUTE_LOGINNAME;
                SetValidateNoiDungKham();
                SetValidateYLenhKham();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void AddUc()
        {
            try
            {
                ucCDHA = new UCTreeListService(imageCollection3, currentModule);
                ucXN = new UCTreeListService(imageCollection3, currentModule);
                ucDichVu = new UCTreeListService(imageCollection3, currentModule);
                ucSieuAm = new UCTreeListService(imageCollection3, currentModule);
                ucPhauThuat = new UCTreeListService(imageCollection3, currentModule);
                ucGiaiPhau = new UCTreeListService(imageCollection3, currentModule);

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
        private void SetValidateNoiDungKham()
        {
            ValidateMaxLength validateMaxLengthNoiDung = new ValidateMaxLength();
            validateMaxLengthNoiDung.textEdit = txtNoiDungKham;
            validateMaxLengthNoiDung.maxLength = 4000;
            validateMaxLengthNoiDung.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            validateMaxLengthNoiDung.isValidNull = true;
            dxValidationProviderEditorInfo.SetValidationRule(txtNoiDungKham, validateMaxLengthNoiDung);
        }
        private void SetValidateYLenhKham()
        {
            ValidateMaxLength validateMaxLengthYLenh = new ValidateMaxLength();
            validateMaxLengthYLenh.textEdit = txtYLenhKham;
            validateMaxLengthYLenh.maxLength = 4000;
            validateMaxLengthYLenh.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            validateMaxLengthYLenh.isValidNull = true;
            dxValidationProviderEditorInfo.SetValidationRule(txtYLenhKham, validateMaxLengthYLenh);
        }
        private void FillDataToGridTreatment()
        {
            try
            {
                WaitingManager.Show();
                gridControl1.DataSource = null;
                CommonParam paramCommon = new CommonParam();
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
                gridControl1.BeginUpdate();
                gridControl1.DataSource = noteList;
                gridControl1.EndUpdate();

                gridViewTreatment.OptionsSelection.EnableAppearanceFocusedCell = false;
                gridViewTreatment.OptionsSelection.EnableAppearanceFocusedRow = false;

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
                    listData = GetEmrDocumentFile(data.TDL_TREATMENT_ID ?? 0,hisCode, true, null, null, ref paramCommon);
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

        private void cboDoctor_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (cboDoctor.EditValue != null && e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboDoctor.EditValue = null;
                    cboDoctor.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        private void InitComboDoctor()
        {
            try
            {
                var data = BackendDataWorker.Get<V_HIS_EMPLOYEE>().Where(o =>
                                o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                                && o.IS_DOCTOR == 1                                
                                ).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "Tên đăng nhập", 150, 1));
                columnInfos.Add(new ColumnInfo("TDL_USERNAME", "Họ và tên", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TDL_USERNAME", "LOGINNAME", columnInfos, false, 400);
                ControlEditorLoader.Load(cboDoctor, data, controlEditorADO);
                cboDoctor.Properties.ImmediatePopup = true;
                cboDoctor.Properties.PopupFormMinSize = new Size(400, cboDoctor.Properties.PopupFormMinSize.Height);
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
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate("Mps000500", DeletegatePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool DeletegatePrintTemplate(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printCode)
                {
                    case "Mps000500":
                        Inphieuketquakhamchuyenkhoa(printCode, fileName, ref result);
                        break;
                    default:
                        break;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
        private void Inphieuketquakhamchuyenkhoa(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                HIS_SPECIALIST_EXAM datamapperExam = new HIS_SPECIALIST_EXAM();
                HIS_TREATMENT datamapperTreatment = new HIS_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SPECIALIST_EXAM>(datamapperExam, currentSpecialistExam);
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(datamapperTreatment, currentSpecialistExam);
                MPS.Processor.Mps000500.PDO.Mps000500PDO pdo = new MPS.Processor.Mps000500.PDO.Mps000500PDO(datamapperExam, datamapperTreatment);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.currentSpecialistExam.TREATMENT_CODE ?? ""), printTypeCode, currentModuleBase.RoomId);
                WaitingManager.Hide();
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {

                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmApprovalExamSpecialist_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                btnSave_Click(null, null);
                e.Handled = true;
            }
            if (e.Control && e.KeyCode == Keys.P)
            {
                btnPrint_Click(null, null);
                e.Handled = true;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SetValidateNoiDungKham();
                SetValidateYLenhKham();

                if (!dxValidationProviderEditorInfo.Validate())
                {
                    MessageBox.Show("Vui lòng kiểm tra lại nội dung khám và y lệnh khám.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                positionHandleControl = -1;
                CommonParam param = new CommonParam();
                HIS_SPECIALIST_EXAM datamapper = new HIS_SPECIALIST_EXAM();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SPECIALIST_EXAM>(datamapper, currentSpecialistExam);
                datamapper.EXAM_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTrackingTime.DateTime);
                datamapper.EXAM_EXECUTE_LOGINNAME = cboDoctor.EditValue != null ? cboDoctor.EditValue.ToString() : null;
                datamapper.EXAM_EXECUTE_USERNAME = cboDoctor.EditValue != null ? cboDoctor.Text.ToString() : null;
                datamapper.EXAM_EXECUTE_CONTENT = txtNoiDungKham.Text != null ? txtNoiDungKham.Text.Trim() : string.Empty;
                datamapper.EXAM_EXCUTE = txtYLenhKham.Text != null ? txtYLenhKham.Text.Trim() : string.Empty;
                datamapper.IS_APPROVAL = 1;
                datamapper.REJECT_APPROVAL_REASON = null;

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_SPECIALIST_EXAM>("api/HisSpecialistExam/Update", ApiConsumers.MosConsumer, datamapper, param);

                Inventec.Common.Logging.LogSystem.Debug("API Create Result: " + Inventec.Common.Logging.LogUtil.TraceData("DataA", rs));
                if (rs != null && this.delegateRefresher != null)
                {
                    this.delegateRefresher();
                    this.Close();
                }

                MessageManager.Show(this, param, rs != null);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<EmrDocumentFileSDO> GetEmrDocumentFile(long treatmentID, string hiscode, bool? IsMerge, bool? IsShowPatientSign, bool? IsShowWatermark, ref CommonParam paramCommon)
        {
            try
            {
                EmrDocumentDownloadFileSDO sdo = new EmrDocumentDownloadFileSDO();
                var emrFilter = new EMR.Filter.EmrDocumentViewFilter();
                emrFilter.IS_DELETE = false;
                emrFilter.TREATMENT_ID = treatmentID;
                sdo.HisCode = hiscode;
                sdo.EmrDocumentViewFilter = emrFilter;
                sdo.IsMerge = IsMerge;
                sdo.IsShowPatientSign = IsShowPatientSign;
                sdo.IsShowWatermark = IsShowWatermark;

                var roomWorking = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentModule.RoomId);
                sdo.RoomCode = roomWorking.ROOM_CODE;
                sdo.DepartmentCode = roomWorking.DEPARTMENT_CODE;
                //log
                Inventec.Common.Logging.LogSystem.Debug("API Create Result: " + Inventec.Common.Logging.LogUtil.TraceData("DataA", sdo));

                return new BackendAdapter(paramCommon).Post<List<EmrDocumentFileSDO>>("api/EmrDocument/DownloadFile", ApiConsumers.EmrConsumer, sdo, paramCommon);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
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
                    List<SereServADO> listPTTT = new List<SereServADO>();
                    listPTTT.AddRange(SereServADOs.Where(
                        o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT
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
        int positionHandleControl = -1;

        private void SetCaptionByLanguageKey()
        {
            try
            {
            //    ////Khoi tao doi tuong resource
            //    Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ApprovalExamSpecialist.Resources.Lang", typeof(frmApprovalExamSpecialist).Assembly);
            //    ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
            //    this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmApprovalExamSpecialist.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            //    this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmApprovalExamSpecialist.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            //    this.gridControl1.Text = Inventec.Common.Resource.Get.Value("frmApprovalExamSpecialist.tabToDieuTri.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            //    this.tabAll.Text = Inventec.Common.Resource.Get.Value("frmApprovalExamSpecialist.tabAll.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            //    this.pcXN.Text = Inventec.Common.Resource.Get.Value("frmApprovalExamSpecialist.pcXN.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            //    this.pcService.Text = Inventec.Common.Resource.Get.Value("frmApprovalExamSpecialist.pcService.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            //    this.pcSANS.Text = Inventec.Common.Resource.Get.Value("frmApprovalExamSpecialist.pcSANS.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            //    this.pcPTTT.Text = Inventec.Common.Resource.Get.Value("frmApprovalExamSpecialist.pcPTTT.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            //    this.pcGP.Text = Inventec.Common.Resource.Get.Value("frmApprovalExamSpecialist.pcGP.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            //    this.Text = Inventec.Common.Resource.Get.Value("frmApprovalExamSpecialist.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}

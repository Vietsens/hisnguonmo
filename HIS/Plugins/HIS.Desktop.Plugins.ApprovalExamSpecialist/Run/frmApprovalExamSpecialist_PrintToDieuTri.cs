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
using DevExpress.XtraEditors;
using EMR.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ApprovalExamSpecialist.Run
{
    /// <summary>
    /// Duyet kham chuyen khoa: sau khi luu, in to dieu tri (Mps000062) va ky so hoan thanh.
    /// Noi dung kham / y lenh kham da duoc backend ghi vao to dieu tri khi duyet
    /// (xem HisSpecialistExamUpdate.ProcessTracking), o day chi lay to do ra in va ky.
    /// </summary>
    public partial class frmApprovalExamSpecialist
    {
        internal enum PrintType
        {
            IN_TO_DIEU_TRI,
        }

        #region Du lieu phuc vu in to dieu tri (Mps000062)

        /// <summary>
        /// To dieu tri mang noi dung duyet kham chuyen khoa - to se duoc in va ky so.
        /// Duoc xac dinh lai sau moi lan luu thanh cong (xem GetTrackingToSign).
        /// </summary>
        private HIS_TRACKING trackingToSign { get; set; }

        private List<V_HIS_TRACKING> _TrackingPrintsProcesss;

        private MOS.EFMODEL.DataModels.HIS_TREATMENT treatment { get; set; }

        private List<V_HIS_TREATMENT_BED_ROOM> _TreatmentBedRooms { get; set; }

        private List<HIS_SERVICE_REQ> _ServiceReqsMps62 { get; set; }
        private Dictionary<long, HIS_SERVICE_REQ> dicServiceReqsMps62 { get; set; }

        private List<HIS_SERE_SERV> _SereServsMps62 { get; set; }
        private Dictionary<long, List<HIS_SERE_SERV>> dicSereServsMps62 { get; set; }

        private List<HIS_EXP_MEST> _ExpMestsMps62 { get; set; }
        private Dictionary<long, HIS_EXP_MEST> dicExpMestsMps62 { get; set; }

        private List<HIS_EXP_MEST_MEDICINE> _ExpMestMedicinesMps62 { get; set; }
        private Dictionary<long, List<HIS_EXP_MEST_MEDICINE>> dicExpMestMedicinesMps62 { get; set; }

        private List<HIS_EXP_MEST_MATERIAL> _ExpMestMaterialsMps62 { get; set; }
        private Dictionary<long, List<HIS_EXP_MEST_MATERIAL>> dicExpMestMaterialsMps62 { get; set; }

        private Dictionary<long, List<HIS_SERVICE_REQ_METY>> dicServiceReqMetysMps62 { get; set; }
        private Dictionary<long, List<HIS_SERVICE_REQ_MATY>> dicServiceReqMatysMps62 { get; set; }

        private List<HIS_SERE_SERV_EXT> _SereServExtsMps62 { get; set; }

        //suat an
        private List<V_HIS_SERE_SERV_RATION> _SereServRation { get; set; }

        //thuoc, vat tu tra lai
        private List<V_HIS_IMP_MEST_2> _MobaImpMests { get; set; }
        private List<V_HIS_IMP_MEST_MEDICINE> _ImpMestMedicines_TL { get; set; }
        private List<V_HIS_IMP_MEST_MATERIAL> _ImpMestMaterial_TL { get; set; }
        private List<V_HIS_IMP_MEST_BLOOD> _ImpMestBlood_TL { get; set; }

        private List<V_HIS_EXP_MEST_BLTY_REQ_2> HisExpMestBltyReq2 { get; set; }

        /// <summary>Benh vien khong dung vat tu => khong can doc vat tu trong kho.</summary>
        private bool _IsMaterial;

        /// <summary>Mau duoc chi dinh rieng (HIS_EXP_MEST_BLTY_REQ) chu khong qua dich vu MAU.</summary>
        private bool BloodPresOption;

        /// <summary>Doc thuoc/vat tu ngoai kho theo y lenh de PDO loc ra khoi to dieu tri.</summary>
        private bool IsNotShowOutMediAndMate;

        #endregion

        private void PrintProcess62(PrintType printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorPrint62 = new Inventec.Common.RichEditor.RichEditorStore(
                    ApiConsumers.SarConsumer,
                    HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(),
                    GlobalVariables.TemnplatePathFolder);
                switch (printType)
                {
                    case PrintType.IN_TO_DIEU_TRI:
                        richEditorPrint62.RunPrintTemplate("Mps000062", null, DelegateRunPrinter62, true);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinter62(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000062":
                        Mps000062(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void Mps000062(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                #region Khoi tao du lieu
                WaitingManager.Show();
                if (this.trackingToSign == null)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Warn("Mps000062. trackingToSign == null => bo qua in to dieu tri.");
                    return;
                }

                V_HIS_TRACKING currentTrackingForPrint = GetVHisTrackingByID(this.trackingToSign.ID);
                if (currentTrackingForPrint == null)
                {
                    // To dieu tri vua duoc Create khi duyet co the chua doc lai duoc tu V_HIS_TRACKING.
                    // Khong duoc de null lot vao _TrackingPrintsProcesss vi se NullReferenceException
                    // o cho doc SHEET_ORDER va lam hong ca luot in ma khong bao gi.
                    currentTrackingForPrint = new V_HIS_TRACKING();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_TRACKING>(currentTrackingForPrint, this.trackingToSign);
                    Inventec.Common.Logging.LogSystem.Warn("Mps000062. Khong doc duoc V_HIS_TRACKING theo ID: "
                        + this.trackingToSign.ID + " => in bang du lieu tu HIS_TRACKING.");
                }

                long treatmentId = this.currentSpecialistExam.TREATMENT_ID ?? 0;
                CommonParam param = new CommonParam();

                _TrackingPrintsProcesss = new List<V_HIS_TRACKING>();
                _TrackingPrintsProcesss.Add(currentTrackingForPrint);

                _TreatmentBedRooms = new List<V_HIS_TREATMENT_BED_ROOM>();

                _ServiceReqsMps62 = new List<HIS_SERVICE_REQ>();
                dicServiceReqsMps62 = new Dictionary<long, HIS_SERVICE_REQ>();

                _SereServsMps62 = new List<HIS_SERE_SERV>();
                dicSereServsMps62 = new Dictionary<long, List<HIS_SERE_SERV>>();

                _ExpMestsMps62 = new List<HIS_EXP_MEST>();
                dicExpMestsMps62 = new Dictionary<long, HIS_EXP_MEST>();

                _ExpMestMedicinesMps62 = new List<HIS_EXP_MEST_MEDICINE>();
                dicExpMestMedicinesMps62 = new Dictionary<long, List<HIS_EXP_MEST_MEDICINE>>();

                _ExpMestMaterialsMps62 = new List<HIS_EXP_MEST_MATERIAL>();
                dicExpMestMaterialsMps62 = new Dictionary<long, List<HIS_EXP_MEST_MATERIAL>>();

                dicServiceReqMetysMps62 = new Dictionary<long, List<HIS_SERVICE_REQ_METY>>();
                dicServiceReqMatysMps62 = new Dictionary<long, List<HIS_SERVICE_REQ_MATY>>();

                HisExpMestBltyReq2 = new List<V_HIS_EXP_MEST_BLTY_REQ_2>();

                this._SereServExtsMps62 = new List<HIS_SERE_SERV_EXT>();
                this._SereServRation = new List<V_HIS_SERE_SERV_RATION>();

                //thuoc, vat tu tra lai
                this._MobaImpMests = new List<V_HIS_IMP_MEST_2>();
                this._ImpMestMedicines_TL = new List<V_HIS_IMP_MEST_MEDICINE>();
                this._ImpMestMaterial_TL = new List<V_HIS_IMP_MEST_MATERIAL>();
                this._ImpMestBlood_TL = new List<V_HIS_IMP_MEST_BLOOD>();

                this.IsNotShowOutMediAndMate = (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.TrackingPrint.IsNotShowOutMediAndMate") == "1");
                this.BloodPresOption = (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.TrackingCreate.BloodPresOption") == "1");
                this._IsMaterial = (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.Tracking.IsMaterial") != "1");

                if (treatmentId > 0)
                {
                    CreateThreadLoadData62(treatmentId);
                }

                int start = 0;
                int count = (this._ServiceReqsMps62 != null) ? this._ServiceReqsMps62.Count : 0;
                while (count > 0)
                {
                    int limit = (count <= 100) ? count : 100;
                    var listSub = this._ServiceReqsMps62.Skip(start).Take(limit).ToList();
                    List<long> _serviceReqIds = listSub.Select(p => p.ID).Distinct().ToList();

                    CreateThreadByServiceReq62(_serviceReqIds);

                    start += 100;
                    count -= 100;
                }

                if (this._ExpMestsMps62 != null && this._ExpMestsMps62.Count > 0)
                {
                    int startExpMest = 0;
                    int countExpMest = this._ExpMestsMps62.Count;
                    while (countExpMest > 0)
                    {
                        int limit = (countExpMest <= 100) ? countExpMest : 100;
                        var listSub = this._ExpMestsMps62.Skip(startExpMest).Take(limit).ToList();
                        CreateThreadLoadDataExpMest62(listSub.Select(p => p.ID).Distinct().ToList());
                        startExpMest += 100;
                        countExpMest -= 100;
                    }
                }

                if (this._SereServsMps62 != null && this._SereServsMps62.Count > 0)
                {
                    int startSS = 0;
                    int countSS = this._SereServsMps62.Count;
                    while (countSS > 0)
                    {
                        int limit = (countSS <= 100) ? countSS : 100;
                        var listSub = this._SereServsMps62.Skip(startSS).Take(limit).ToList();
                        List<long> _sereServIds = listSub.Select(p => p.ID).Distinct().ToList();

                        //Get SERE_SERV_EXT
                        MOS.Filter.HisSereServExtFilter sereServExtFilter = new MOS.Filter.HisSereServExtFilter();
                        sereServExtFilter.SERE_SERV_IDs = _sereServIds;

                        var dataSS_EXTs = new BackendAdapter(param).Get<List<HIS_SERE_SERV_EXT>>("/api/HisSereServExt/Get", ApiConsumers.MosConsumer, sereServExtFilter, param);
                        if (dataSS_EXTs != null && dataSS_EXTs.Count > 0)
                        {
                            this._SereServExtsMps62.AddRange(dataSS_EXTs);
                        }

                        startSS += 100;
                        countSS -= 100;
                    }
                }
                #endregion

                #region Dau hieu sinh ton
                MOS.Filter.HisDhstFilter dhstFilter = new HisDhstFilter();
                dhstFilter.TRACKING_ID = this.trackingToSign.ID;
                var _Dhsts = new BackendAdapter(param).Get<List<HIS_DHST>>(HisRequestUriStore.HIS_DHST_GET, ApiConsumers.MosConsumer, dhstFilter, param);
                #endregion

                #region Danh sach cham soc
                List<HIS_CARE> _Cares = new List<HIS_CARE>();
                List<V_HIS_CARE_DETAIL> _CareDetails = new List<V_HIS_CARE_DETAIL>();

                MOS.Filter.HisCareFilter careFilter = new HisCareFilter();
                careFilter.TREATMENT_ID = treatmentId;
                careFilter.TRACKING_ID = this.trackingToSign.ID;
                var care = new BackendAdapter(param).Get<List<HIS_CARE>>(HisRequestUriStore.HIS_CARE_GET, ApiConsumers.MosConsumer, careFilter, param).FirstOrDefault();
                if (care != null)
                {
                    _Cares.Add(care);
                    MOS.Filter.HisCareDetailViewFilter careDetailFilter = new HisCareDetailViewFilter();
                    careDetailFilter.CARE_ID = care.ID;
                    var careDetail = new BackendAdapter(param).Get<List<V_HIS_CARE_DETAIL>>(HisRequestUriStore.HIS_CARE_DETAIL_GETVIEW, ApiConsumers.MosConsumer, careDetailFilter, param);
                    _CareDetails.AddRange(careDetail);
                }
                #endregion

                #region Thong tin khoa phong hien tai
                MOS.SDO.WorkPlaceSDO _workPlaceSDO = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.wkRoomId);
                MPS.Processor.Mps000062.PDO.Mps000062SingleKey singleKey = new MPS.Processor.Mps000062.PDO.Mps000062SingleKey(_workPlaceSDO);
                singleKey.LOGIN_NAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                singleKey.USER_NAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                #endregion

                #region Suat an
                MOS.Filter.HisSereServRationViewFilter filterRation = new HisSereServRationViewFilter();
                filterRation.TRACKING_ID = this.trackingToSign.ID;
                var ssRation = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_RATION>>("api/HisSereServRation/GetView", ApiConsumers.MosConsumer, filterRation, param);
                this._SereServRation.AddRange(ssRation);
                #endregion

                #region Mau chi dinh
                if (BloodPresOption)
                {
                    CommonParam param_ = new CommonParam();
                    MOS.Filter.HisExpMestBltyReqView2Filter filter = new HisExpMestBltyReqView2Filter();
                    filter.TDL_TREATMENT_ID = treatmentId;
                    filter.TRACKING_ID = this.trackingToSign.ID;
                    HisExpMestBltyReq2 = new BackendAdapter(param_).Get<List<V_HIS_EXP_MEST_BLTY_REQ_2>>("/api/HisExpMestBltyReq/GetView2", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param_);
                }
                #endregion

                #region Key cau hinh hien thi
                singleKey.IsShowMedicineLine = (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.Library.Bordereau.IsShowMedicineLine") == "1");
                singleKey.IsOrderByType = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.TrackingPrint.OderOption"));
                singleKey.keyVienTim = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.Tracking.IsNumberByMedicineType"));
                singleKey.UsedDayCountingOption = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.TrackingPrint.UsedDayCountingOption"));
                singleKey.UsedDayCountingFormatOption = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.TrackingPrint.UsedDayCountingFormatOption"));
                singleKey.UsedDayCountingOutStockOption = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.TrackingPrint.UsedDayCountingOutStockOption"));
                #endregion

                #region Dong thuoc / Dang bao che
                List<HIS_MEDICINE_LINE> listMedicineLine = BackendDataWorker.Get<HIS_MEDICINE_LINE>();
                List<HIS_DOSAGE_FORM> listDosage = BackendDataWorker.Get<HIS_DOSAGE_FORM>();
                #endregion

                #region Mps000062
                Inventec.Common.Logging.LogSystem.Debug("Begin EmrGenerateProcessor");
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor()
                    .GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : ""), printTypeCode, this.wkRoomId);
                Inventec.Common.Logging.LogSystem.Debug("end EmrGenerateProcessor");

                long keyPrintMerge = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.EmrDocument.IsPrintMerge"));
                if (keyPrintMerge == 1)
                {
                    string uniqueTime = "";
                    inputADO.MergeCode = String.Format("{0}_{1}_{2}", printTypeCode, uniqueTime, (treatment != null ? treatment.TREATMENT_CODE : ""));
                }

                if (_TrackingPrintsProcesss != null && _TrackingPrintsProcesss.Count > 0
                    && _TrackingPrintsProcesss[0] != null && _TrackingPrintsProcesss[0].SHEET_ORDER != null)
                {
                    inputADO.DocumentName += " " + "(" + _TrackingPrintsProcesss[0].SHEET_ORDER.ToString() + ")";
                }

                var PatientTypeAlter = new BackendAdapter(new CommonParam()).Get<HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetLastByTreatmentId", ApiConsumers.MosConsumer, treatmentId, null);

                V_HIS_BED_LOG selectedBedLog = null;
                MOS.Filter.HisBedLogViewFilter bedLogViewFilter = new MOS.Filter.HisBedLogViewFilter();
                bedLogViewFilter.TREATMENT_ID = treatmentId;
                var bedLogs = new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumers.MosConsumer, bedLogViewFilter, param);
                if (bedLogs != null)
                {
                    var minTrackingTime = this.trackingToSign.TRACKING_TIME;
                    var filteredBedLogs = bedLogs
                        .Where(o => o.BED_ID != 0
                            && (o.START_TIME <= minTrackingTime && (o.FINISH_TIME == null || minTrackingTime <= o.FINISH_TIME)))
                        .ToList();
                    selectedBedLog = filteredBedLogs
                        .OrderByDescending(o => o.START_TIME)
                        .ThenByDescending(o => o.ID)
                        .FirstOrDefault();
                }

                #region Khoa hoi chan
                // Khoa phong moi hoi chan luu o HIS_SPECIALIST_EXAM (INVITE_TYPE = 2), cot EXAM_EXECUTE_DEPARMENT_ID.
                List<V_HIS_SPECIALIST_EXAM> specialistExams = new List<V_HIS_SPECIALIST_EXAM>();
                MOS.Filter.HisSpecialistExamViewFilter specialistExamViewFilter = new MOS.Filter.HisSpecialistExamViewFilter();
                specialistExamViewFilter.TREATMENT_CODE = (treatment != null ? treatment.TREATMENT_CODE : "");
                specialistExamViewFilter.INVITE_TYPE = 2;
                var specialistExamViews = new BackendAdapter(param).Get<List<V_HIS_SPECIALIST_EXAM>>("api/HisSpecialistExam/GetView", ApiConsumers.MosConsumer, specialistExamViewFilter, param);
                if (specialistExamViews != null && specialistExamViews.Count > 0)
                    specialistExams = specialistExamViews;
                #endregion

                Inventec.Common.Logging.LogSystem.Debug("begin Mps000062PDO");
                MPS.Processor.Mps000062.PDO.Mps000062PDO mps000062RDO = new MPS.Processor.Mps000062.PDO.Mps000062PDO(
                    treatment,
                    _TreatmentBedRooms,
                    _TrackingPrintsProcesss,
                    _Dhsts,
                    dicServiceReqsMps62,
                    dicSereServsMps62,
                    dicExpMestsMps62,
                    dicExpMestMedicinesMps62,
                    dicExpMestMaterialsMps62,
                    dicServiceReqMetysMps62,
                    dicServiceReqMatysMps62,
                    _Cares,
                    _CareDetails,
                    singleKey,
                    BackendDataWorker.Get<HIS_ICD>(),
                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                    BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                    BackendDataWorker.Get<HIS_SERVICE_TYPE>(),
                    this._SereServExtsMps62,
                    BackendDataWorker.Get<HIS_MEDICINE_USE_FORM>(),
                    this._SereServRation,
                    this._MobaImpMests,
                    this._ImpMestMedicines_TL,
                    this._ImpMestMaterial_TL,
                    HisExpMestBltyReq2,
                    BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => this._SereServsMps62.Select(p => p.SERVICE_ID).Contains(o.ID)).ToList(),
                    this._ImpMestBlood_TL,
                    PatientTypeAlter,
                    listMedicineLine,
                    listDosage,
                    selectedBedLog
                    );
                mps000062RDO._SpecialistExams = specialistExams;
                Inventec.Common.Logging.LogSystem.Debug("End Mps000062PDO");

                WaitingManager.Hide();

                string saveFilePath = "";
                string ext = Path.GetExtension(fileName);
                if (ext == ".doc" || ext == ".docx")
                {
                    saveFilePath = GenerateTempFileWithin("", ".docx");
                }

                MPS.ProcessorBase.Core.PrintData PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000062RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, "", 1, saveFilePath) { EmrInputADO = inputADO };

                WaitingManager.Hide();

                #region Neu to dieu tri da co van ban ky thi xoa truoc khi ky lai
                EMR.Filter.EmrDocumentViewFilter documentViewFilter = new EMR.Filter.EmrDocumentViewFilter();
                documentViewFilter.TREATMENT_CODE__EXACT = (treatment != null ? treatment.TREATMENT_CODE : "");
                documentViewFilter.IS_DELETE = false;
                var documents = new BackendAdapter(new CommonParam()).Get<List<V_EMR_DOCUMENT>>("api/EmrDocument/GetView", ApiConsumers.EmrConsumer, documentViewFilter, null);
                if (documents != null && documents.Count() > 0)
                {
                    string checkServiceReqCode = "HIS_TRACKING:" + this.trackingToSign.ID;
                    var documentSigneds = documents.Where(o => o.DOCUMENT_TYPE_ID == 7
                        && !string.IsNullOrEmpty(o.HIS_CODE)
                        && o.HIS_CODE.Contains(checkServiceReqCode));
                    if (documentSigneds != null && documentSigneds.Count() > 0)
                    {
                        if (XtraMessageBox.Show("Tờ điều trị đã tồn tại văn bản ký, tiếp tục sẽ tự động Xóa văn bản ký hiện tại. Bạn có muốn tiếp tục?",
                            Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            WaitingManager.Hide();
                            return;
                        }
                        foreach (var item in documentSigneds)
                        {
                            new BackendAdapter(new CommonParam()).Post<bool>("api/EmrDocument/Delete", ApiConsumers.EmrConsumer, item.ID, null);
                        }
                    }
                }
                #endregion

                result = MPS.MpsPrinter.Run(PrintData);
                Inventec.Common.Logging.LogSystem.Debug("Mps000062. MpsPrinter.Run result: " + result + ", fileName: " + fileName);
                WaitingManager.Hide();
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error("Mps000062. Loi khi in to dieu tri. trackingId: "
                    + (this.trackingToSign != null ? this.trackingToSign.ID.ToString() : "null")
                    + ", fileName: " + fileName, ex);
            }
        }

        private static string GenerateTempFileWithin(string fileName, string _extension = "")
        {
            try
            {
                string extension = !String.IsNullOrEmpty(_extension) ? _extension : Path.GetExtension(fileName);
                string pathDic = Path.Combine(Path.Combine(Path.Combine(Inventec.Common.TemplaterExport.ApplicationLocationStore.ApplicationPathLocal, "temp"), DateTime.Now.ToString("ddMMyyyy")), "Templates");
                if (!Directory.Exists(pathDic))
                {
                    Directory.CreateDirectory(pathDic);
                }
                return Path.Combine(pathDic, Guid.NewGuid().ToString() + extension);
            }
            catch (IOException exception)
            {
                Inventec.Common.Logging.LogSystem.Warn("Error create temp file: " + exception.Message, exception);
                return String.Empty;
            }
        }

        private V_HIS_TRACKING GetVHisTrackingByID(long trackingID)
        {
            V_HIS_TRACKING result = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTrackingViewFilter filter = new MOS.Filter.HisTrackingViewFilter();
                filter.ID = trackingID;
                var apiResult = new BackendAdapter(param).Get<List<V_HIS_TRACKING>>(HisRequestUriStore.HIS_TRACKING_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                if (apiResult != null)
                    result = apiResult.FirstOrDefault();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void CreateThreadLoadData62(object param)
        {
            Thread threadTreatment = new Thread(new ParameterizedThreadStart(LoadDataTreatmentNewThread62));
            Thread threadTreatmentBedRoom = new Thread(new ParameterizedThreadStart(LoadDataTreatmentBedRoomNewThread62));
            Thread threadServiceReq = new Thread(new ParameterizedThreadStart(LoadDataServiceReqNewThread62));
            Thread threadMobaImpMests = new Thread(new ParameterizedThreadStart(LoadDataMobaImpMestsNewThread62));

            try
            {
                threadTreatment.Start(param);
                threadTreatmentBedRoom.Start(param);
                threadServiceReq.Start(param);
                threadMobaImpMests.Start(param);

                threadTreatment.Join();
                threadTreatmentBedRoom.Join();
                threadServiceReq.Join();
                threadMobaImpMests.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadTreatment.Abort();
                threadTreatmentBedRoom.Abort();
                threadServiceReq.Abort();
                threadMobaImpMests.Abort();
            }
        }

        private void LoadDataTreatmentNewThread62(object param)
        {
            try
            {
                LoadDataTreatment62((long)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataTreatment62(long treatmentId)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentFilter treatmentFilter = new MOS.Filter.HisTreatmentFilter();
                treatmentFilter.ID = treatmentId;
                var treatments = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, param);
                this.treatment = (treatments != null) ? treatments.FirstOrDefault() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataTreatmentBedRoomNewThread62(object param)
        {
            try
            {
                LoadDataTreatmentBedRoom62((long)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataTreatmentBedRoom62(long treatmentId)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentBedRoomViewFilter bedFilter = new HisTreatmentBedRoomViewFilter();
                bedFilter.TREATMENT_ID = treatmentId;
                bedFilter.ORDER_FIELD = "CREATE_TIME";
                bedFilter.ORDER_DIRECTION = "DESC";
                var treatmentBedRooms = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>(HisRequestUriStore.HIS_TREATMENT_BED_ROOM_GETVIEW, ApiConsumers.MosConsumer, bedFilter, param);
                if (treatmentBedRooms != null && treatmentBedRooms.Count > 0)
                {
                    if (this._TrackingPrintsProcesss != null && this._TrackingPrintsProcesss.Count() > 0)
                    {
                        foreach (var item in this._TrackingPrintsProcesss.Where(o => o != null))
                        {
                            var treatmentBedRoom = treatmentBedRooms.Where(o => o.ADD_TIME <= item.TRACKING_TIME && o.ROOM_ID == item.ROOM_ID).OrderByDescending(o => o.ADD_TIME).FirstOrDefault();
                            if (treatmentBedRoom != null)
                                _TreatmentBedRooms.Add(treatmentBedRoom);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataServiceReqNewThread62(object param)
        {
            try
            {
                LoadDataServiceReq62((long)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataServiceReq62(long treatmentId)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqFilter serviceReqFilterVT = new MOS.Filter.HisServiceReqFilter();
                serviceReqFilterVT.TREATMENT_ID = treatmentId;
                this._ServiceReqsMps62 = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, serviceReqFilterVT, param);
                if (this._ServiceReqsMps62 != null && this._ServiceReqsMps62.Count > 0)
                {
                    foreach (var item in this._ServiceReqsMps62)
                    {
                        if (!dicServiceReqsMps62.ContainsKey(item.ID))
                        {
                            dicServiceReqsMps62[item.ID] = item;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMobaImpMestsNewThread62(object param)
        {
            try
            {
                LoadDataMobaImpMests62((long)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMobaImpMests62(long treatmentId)
        {
            try
            {
                long keyViewMediMateTH = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.Tracking.IsMediMateTH"));
                if (keyViewMediMateTH != 1)
                    return;
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisImpMestView2Filter filter = new MOS.Filter.HisImpMestView2Filter();
                filter.TDL_TREATMENT_ID = treatmentId;
                filter.IMP_MEST_TYPE_IDs = new List<long>()
                {
                    IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DNTTL,
                    IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DTTTL,
                    IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DMTL,
                    IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DONKTL
                };
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                this._MobaImpMests = new BackendAdapter(paramCommon).Get<List<V_HIS_IMP_MEST_2>>("api/HisImpMest/GetView2", ApiConsumers.MosConsumer, filter, paramCommon);

                if (this._MobaImpMests != null && this._MobaImpMests.Count > 0)
                {
                    int start = 0;
                    int count = this._MobaImpMests.Count;
                    while (count > 0)
                    {
                        int limit = (count <= 100) ? count : 100;
                        var listSub = this._MobaImpMests.Skip(start).Take(limit).ToList();
                        List<long> _MobaImpMestsIds = listSub.Select(p => p.ID).Distinct().ToList();

                        MOS.Filter.HisImpMestMedicineViewFilter impMestMedicineViewFilter = new MOS.Filter.HisImpMestMedicineViewFilter();
                        impMestMedicineViewFilter.IMP_MEST_IDs = _MobaImpMestsIds;
                        var impMestMed = new BackendAdapter(paramCommon).Get<List<V_HIS_IMP_MEST_MEDICINE>>("api/HisImpMestMedicine/GetView", ApiConsumers.MosConsumer, impMestMedicineViewFilter, paramCommon);
                        if (impMestMed != null && impMestMed.Count > 0)
                        {
                            this._ImpMestMedicines_TL.AddRange(impMestMed);
                        }

                        if (!this._IsMaterial)
                        {
                            MOS.Filter.HisImpMestMaterialViewFilter impMestMaterialViewFilter = new MOS.Filter.HisImpMestMaterialViewFilter();
                            impMestMaterialViewFilter.IMP_MEST_IDs = _MobaImpMestsIds.ToList();
                            var impMestMart = new BackendAdapter(paramCommon).Get<List<V_HIS_IMP_MEST_MATERIAL>>("api/HisImpMestMaterial/GetView", ApiConsumers.MosConsumer, impMestMaterialViewFilter, paramCommon);
                            if (impMestMart != null && impMestMart.Count > 0)
                            {
                                this._ImpMestMaterial_TL.AddRange(impMestMart);
                            }
                        }

                        MOS.Filter.HisImpMestBloodViewFilter impMestBloodViewFilter = new MOS.Filter.HisImpMestBloodViewFilter();
                        impMestBloodViewFilter.IMP_MEST_IDs = _MobaImpMestsIds.ToList();
                        this._ImpMestBlood_TL = new BackendAdapter(paramCommon).Get<List<V_HIS_IMP_MEST_BLOOD>>("api/HisImpMestBlood/GetView", ApiConsumers.MosConsumer, impMestBloodViewFilter, paramCommon);

                        start += 100;
                        count -= 100;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Thuoc / vat tu trong kho
        /// </summary>
        private void CreateThreadLoadDataExpMest62(object param)
        {
            Thread threadMedicine = new Thread(new ParameterizedThreadStart(LoadDataExpMestMedicineNewThread62));
            Thread threadMaterial = new Thread(new ParameterizedThreadStart(LoadDataExpMestMaterialNewThread62));

            try
            {
                threadMedicine.Start(param);
                threadMaterial.Start(param);

                threadMedicine.Join();
                threadMaterial.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadMedicine.Abort();
                threadMaterial.Abort();
            }
        }

        private void LoadDataExpMestMedicineNewThread62(object param)
        {
            try
            {
                LoadDataExpMestMedicine62((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataExpMestMedicine62(List<long> _expMestIds)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMedicineFilter filter = new HisExpMestMedicineFilter();
                filter.EXP_MEST_IDs = _expMestIds;
                this._ExpMestMedicinesMps62 = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumers.MosConsumer, filter, param);
                if (this._ExpMestMedicinesMps62 != null && this._ExpMestMedicinesMps62.Count > 0)
                {
                    var dataGroups = this._ExpMestMedicinesMps62.Where(p => p.IS_NOT_PRES != 1).GroupBy(p => new { p.TDL_MEDICINE_TYPE_ID, p.EXP_MEST_ID, p.TUTORIAL }).Select(p => p.ToList()).ToList();
                    foreach (var item in dataGroups)
                    {
                        HIS_EXP_MEST_MEDICINE ado = new HIS_EXP_MEST_MEDICINE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST_MEDICINE>(ado, item[0]);
                        ado.AMOUNT = item.Sum(p => p.AMOUNT);
                        ado.TH_AMOUNT = item.Sum(p => p.TH_AMOUNT);
                        if (!dicExpMestMedicinesMps62.ContainsKey(ado.EXP_MEST_ID ?? 0))
                        {
                            dicExpMestMedicinesMps62[ado.EXP_MEST_ID ?? 0] = new List<HIS_EXP_MEST_MEDICINE>();
                        }
                        dicExpMestMedicinesMps62[ado.EXP_MEST_ID ?? 0].Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataExpMestMaterialNewThread62(object param)
        {
            try
            {
                LoadDataExpMestMaterialMps62((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataExpMestMaterialMps62(List<long> _expMestIds)
        {
            try
            {
                if (this._IsMaterial)
                    return;
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMaterialFilter filter = new HisExpMestMaterialFilter();
                filter.EXP_MEST_IDs = _expMestIds;
                this._ExpMestMaterialsMps62 = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/Get", ApiConsumers.MosConsumer, filter, param);

                long keyPrintDoNotShowExpendMaterial = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.Tracking.DoNotShowExpendMaterial"));
                if (keyPrintDoNotShowExpendMaterial == 1)
                {
                    if (_ExpMestMaterialsMps62 != null && _ExpMestMaterialsMps62.Count > 0)
                    {
                        _ExpMestMaterialsMps62 = _ExpMestMaterialsMps62.Where(o => o.IS_EXPEND != 1).ToList();
                    }
                }

                if (this._ExpMestMaterialsMps62 != null && this._ExpMestMaterialsMps62.Count > 0)
                {
                    var dataGroups = this._ExpMestMaterialsMps62.Where(p => p.IS_NOT_PRES != 1).GroupBy(p => new { p.TDL_MATERIAL_TYPE_ID, p.EXP_MEST_ID }).Select(p => p.ToList()).ToList();
                    foreach (var item in dataGroups)
                    {
                        HIS_EXP_MEST_MATERIAL ado = new HIS_EXP_MEST_MATERIAL();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST_MATERIAL>(ado, item[0]);
                        ado.AMOUNT = item.Sum(p => p.AMOUNT);
                        ado.TH_AMOUNT = item.Sum(p => p.TH_AMOUNT);
                        if (!dicExpMestMaterialsMps62.ContainsKey(ado.EXP_MEST_ID ?? 0))
                        {
                            dicExpMestMaterialsMps62[ado.EXP_MEST_ID ?? 0] = new List<HIS_EXP_MEST_MATERIAL>();
                        }
                        dicExpMestMaterialsMps62[ado.EXP_MEST_ID ?? 0].Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Thread EXP_MEST / SERVICE_REQ_METY / SERVICE_REQ_MATY / SERE_SERV
        /// </summary>
        private void CreateThreadByServiceReq62(object param)
        {
            Thread threadExpMest = new Thread(new ParameterizedThreadStart(LoadDataExpMestNewThread62));
            Thread threadServiceReqMety = new Thread(new ParameterizedThreadStart(LoadDataServiceReqMetyNewThread62));
            Thread threadServiceReqMaty = new Thread(new ParameterizedThreadStart(LoadDataServiceReqMatyNewThread62));
            Thread threadSereServ = new Thread(new ParameterizedThreadStart(LoadDataSereServNewThread62));

            try
            {
                threadExpMest.Start(param);
                threadServiceReqMety.Start(param);
                threadServiceReqMaty.Start(param);
                threadSereServ.Start(param);

                threadExpMest.Join();
                threadServiceReqMety.Join();
                threadServiceReqMaty.Join();
                threadSereServ.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadExpMest.Abort();
                threadServiceReqMety.Abort();
                threadServiceReqMaty.Abort();
                threadSereServ.Abort();
            }
        }

        private void LoadDataExpMestNewThread62(object param)
        {
            try
            {
                LoadDataExpMest62((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataExpMest62(List<long> _serviceReqIds)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestFilter expMestFilter = new HisExpMestFilter();
                expMestFilter.SERVICE_REQ_IDs = _serviceReqIds;
                var expMestDatas = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumers.MosConsumer, expMestFilter, param);
                if (expMestDatas == null || expMestDatas.Count <= 0)
                    return;

                foreach (var item in expMestDatas)
                {
                    dicExpMestsMps62[item.SERVICE_REQ_ID ?? 0] = item;
                }
                this._ExpMestsMps62.AddRange(expMestDatas);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //Thuoc ngoai kho
        private void LoadDataServiceReqMetyNewThread62(object param)
        {
            try
            {
                LoadDataServiceReqMety62((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataServiceReqMety62(List<long> _serviceReqIds)
        {
            try
            {
                if (!this.IsNotShowOutMediAndMate)
                    return;
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqMetyFilter metyFilter = new HisServiceReqMetyFilter();
                metyFilter.SERVICE_REQ_IDs = _serviceReqIds;
                var metyDatas = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_METY>>("api/HisServiceReqMety/Get", ApiConsumers.MosConsumer, metyFilter, param);
                if (metyDatas == null || metyDatas.Count <= 0)
                    return;

                foreach (var item in metyDatas)
                {
                    if (!dicServiceReqMetysMps62.ContainsKey(item.SERVICE_REQ_ID))
                    {
                        dicServiceReqMetysMps62[item.SERVICE_REQ_ID] = new List<HIS_SERVICE_REQ_METY>();
                    }
                    dicServiceReqMetysMps62[item.SERVICE_REQ_ID].Add(item);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //Vat tu ngoai kho
        private void LoadDataServiceReqMatyNewThread62(object param)
        {
            try
            {
                LoadDataServiceReqMaty62((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataServiceReqMaty62(List<long> _serviceReqIds)
        {
            try
            {
                if (!this.IsNotShowOutMediAndMate)
                    return;
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqMatyFilter matyFilter = new HisServiceReqMatyFilter();
                matyFilter.SERVICE_REQ_IDs = _serviceReqIds;
                var matyDatas = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_MATY>>("api/HisServiceReqMaty/Get", ApiConsumers.MosConsumer, matyFilter, param);
                if (matyDatas == null || matyDatas.Count <= 0)
                    return;

                foreach (var item in matyDatas)
                {
                    if (!dicServiceReqMatysMps62.ContainsKey(item.SERVICE_REQ_ID))
                    {
                        dicServiceReqMatysMps62[item.SERVICE_REQ_ID] = new List<HIS_SERVICE_REQ_MATY>();
                    }
                    dicServiceReqMatysMps62[item.SERVICE_REQ_ID].Add(item);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataSereServNewThread62(object param)
        {
            try
            {
                LoadDataSereServ62((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataSereServ62(List<long> _serviceReqIds)
        {
            try
            {
                if (_serviceReqIds == null || _serviceReqIds.Count <= 0)
                    return;
                List<V_HIS_SERVICE> hiservice = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.IS_NOT_SHOW_TRACKING == 1).ToList();
                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServFilter hisSereServFilterVT = new MOS.Filter.HisSereServFilter();
                hisSereServFilterVT.SERVICE_REQ_IDs = _serviceReqIds;
                var datas = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, hisSereServFilterVT, param);
                if (datas == null || datas.Count <= 0)
                    return;

                if (hiservice != null && hiservice.Count() > 0)
                {
                    datas = datas.Where(o => hiservice.All(p => p.ID != o.SERVICE_ID)).ToList();
                }
                if (BloodPresOption)
                {
                    datas = datas.Where(o => o.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU).ToList();
                }

                if (datas.Count > 0)
                {
                    this._SereServsMps62.AddRange(datas);
                    foreach (var item in datas)
                    {
                        if (!dicSereServsMps62.ContainsKey(item.SERVICE_REQ_ID ?? 0))
                        {
                            dicSereServsMps62[item.SERVICE_REQ_ID ?? 0] = new List<HIS_SERE_SERV>();
                        }
                        dicSereServsMps62[item.SERVICE_REQ_ID ?? 0].Add(item);
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

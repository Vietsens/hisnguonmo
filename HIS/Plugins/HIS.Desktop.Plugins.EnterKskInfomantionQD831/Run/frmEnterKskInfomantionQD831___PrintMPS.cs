/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * In / Lưu ký phiếu Hồ sơ QĐ831 -> Mps000519.
 * Dữ liệu LẤY THEO Y LỆNH ĐANG XỬ LÝ, ưu tiên dữ liệu ĐÃ LƯU (GetFull theo currentServiceReq.ID):
 *   - Gọi ngay sau Lưu  -> phản ánh dữ liệu vừa lưu.
 *   - Chỉ In (chưa lưu lại) -> phản ánh dữ liệu đã lưu trước đó của y lệnh.
 * PDO Mps000519: HIS_KSK_PROFILE + HIS_KSK_GENERAL + HIS_SERVICE_REQ + HIS_PATIENT + HIS_DHST
 *   + danh mục HIS_DISEASE_TYPE/DETAIL + kết quả HIS_DISEASE_DETAIL_RESULT
 *   + HIS_VACCINE_TYPE + HIS_HEALTH_VACCINATION + V_HIS_TREATMENT_4.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;

namespace HIS.Desktop.Plugins.EnterKskInfomantionQD831.Run
{
    public partial class frmEnterKskInfomantionQD831
    {
        // Mã loại tiền sử (HIS_DISEASE_TYPE.DISEASE_TYPE_CODE) dùng cho checklist QĐ831.
        private static readonly string[] KSK_DISEASE_TYPE_CODES = { "49", "50", "51", "52", "53" };

        /// <summary>In (hoặc lưu ký nếu IsSignEmr=true) phiếu Mps000519 theo y lệnh đang xử lý.</summary>
        private void PrintMps000519()
        {
            try
            {
                if (currentServiceReq == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn y lệnh để in.", "Thông báo",
                        System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                    return;
                }
                var richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(
                    ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(),
                    Inventec.Desktop.Common.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                richEditorMain.RunPrintTemplate("Mps000519", DelegateRunPrinter519);
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        private bool DelegateRunPrinter519(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (printTypeCode == "Mps000519")
                    LoadBieuMauPhieuMps000519(printTypeCode, fileName, ref result);
            }
            catch (Exception ex) { LogSystem.Error(ex); }
            return result;
        }

        private void LoadBieuMauPhieuMps000519(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                WaitingManager.Show();

                // 1) Dữ liệu đã lưu theo y lệnh đang xử lý (GetFull).
                HIS_KSK_PROFILE profile = null;
                HIS_KSK_GENERAL general = null;
                HIS_DHST dhst = null;
                HIS_PATIENT patient = null;
                var results = new List<HIS_DISEASE_DETAIL_RESULT>();
                var vaccinations = new List<HIS_HEALTH_VACCINATION>();

                var full = new BackendAdapter(param).Get<HisKskProfileFullSDO>(
                    "api/HisKskProfile/GetFull", ApiConsumers.MosConsumer,
                    new MOS.Filter.HisKskProfileFilter { SERVICE_REQ_ID = currentServiceReq.ID }, param);
                if (full != null)
                {
                    var exam = full.ExamHistory != null ? full.ExamHistory.FirstOrDefault() : null;
                    profile = (full.PatientInfo != null && full.PatientInfo.Profiles != null)
                        ? full.PatientInfo.Profiles.FirstOrDefault() : null;
                    if (profile == null && exam != null) profile = exam.HisKskProfile;
                    if (full.PatientInfo != null) patient = full.PatientInfo.Patient;
                    if (exam != null)
                    {
                        general = exam.HisKskGeneral;
                        dhst = exam.HisDhst;
                        if (exam.HisDiseaseDetailResults != null) results = exam.HisDiseaseDetailResults;
                        if (exam.HisHealthVaccinations != null && exam.HisHealthVaccinations.Count > 0)
                            vaccinations = exam.HisHealthVaccinations;
                    }
                    if (vaccinations.Count == 0 && full.Vaccination != null) vaccinations = full.Vaccination;
                }

                // 2) Sinh tồn theo DHST_ID nếu GetFull chưa kèm.
                if (dhst == null && general != null && general.DHST_ID != null && general.DHST_ID > 0)
                {
                    var dt = new BackendAdapter(param).Get<List<HIS_DHST>>(
                        "api/HisDhst/Get", ApiConsumers.MosConsumer,
                        new MOS.Filter.HisDhstFilter { ID = general.DHST_ID }, param);
                    if (dt != null && dt.Count > 0) dhst = dt[0];
                }

                // 3) Entity y lệnh + bệnh nhân + lượt điều trị (hành chính/barcode/avatar).
                HIS_SERVICE_REQ sreqEntity = FetchServiceReqEntity(currentServiceReq.ID, param);
                if (patient == null) patient = FetchPatientEntity(currentServiceReq.TDL_PATIENT_ID, param);
                V_HIS_TREATMENT_4 treatment = FetchTreatmentView4(currentServiceReq.TREATMENT_ID, param);

                // 4) Danh mục checklist (49/50/51/52/53) + chi tiết + loại vắc xin (cache RAM).
                var diseaseTypes = BackendDataWorker.Get<HIS_DISEASE_TYPE>()
                    .Where(t => t != null && t.IS_ACTIVE == 1
                        && KSK_DISEASE_TYPE_CODES.Contains((t.DISEASE_TYPE_CODE ?? "").Trim()))
                    .ToList();
                var typeIds = new HashSet<long>(diseaseTypes.Select(t => t.ID));
                var diseaseDetails = BackendDataWorker.Get<HIS_DISEASE_DETAIL>()
                    .Where(d => d != null && d.IS_ACTIVE == 1 && d.DISEASE_TYPE_ID != null && typeIds.Contains(d.DISEASE_TYPE_ID.Value))
                    .OrderBy(d => d.NUM_ORDER).ToList();
                var vaccineTypes = BackendDataWorker.Get<HIS_VACCINE_TYPE>()
                    .Where(v => v != null && v.IS_ACTIVE == 1)
                    .OrderBy(v => v.VACCINE_TYPE_CODE).ToList();

                WaitingManager.Hide();

                var rdo = new MPS.Processor.Mps000519.PDO.Mps000519PDO(
                    profile,
                    general,
                    sreqEntity,
                    patient,
                    dhst,
                    diseaseTypes,
                    diseaseDetails,
                    results,
                    vaccineTypes,
                    vaccinations,
                    treatment);

                PrintData519(printTypeCode, fileName, rdo, ref result);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private HIS_SERVICE_REQ FetchServiceReqEntity(long id, CommonParam param)
        {
            try
            {
                var list = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(
                    "api/HisServiceReq/Get", ApiConsumers.MosConsumer,
                    new MOS.Filter.HisServiceReqFilter { ID = id }, param);
                return (list != null && list.Count > 0) ? list[0] : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }

        private HIS_PATIENT FetchPatientEntity(long patientId, CommonParam param)
        {
            try
            {
                if (patientId <= 0) return null;
                var list = new BackendAdapter(param).Get<List<HIS_PATIENT>>(
                    "api/HisPatient/Get", ApiConsumers.MosConsumer,
                    new MOS.Filter.HisPatientFilter { ID = patientId }, param);
                return (list != null && list.Count > 0) ? list[0] : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }

        private V_HIS_TREATMENT_4 FetchTreatmentView4(long? treatmentId, CommonParam param)
        {
            try
            {
                if (treatmentId == null || treatmentId <= 0) return null;
                var list = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_4>>(
                    "api/HisTreatment/GetView4", ApiConsumers.MosConsumer,
                    new HisTreatmentView4Filter { ID = treatmentId }, null);
                return (list != null && list.Count > 0) ? list[0] : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }

        /// <summary>In/ký theo cấu hình (giống EnterKskV2.PrintData).</summary>
        private void PrintData519(string printTypeCode, string fileName, object data, ref bool result)
        {
            try
            {
                string printerName = "";
                if (GlobalVariables.dicPrinter != null && GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    printerName = GlobalVariables.dicPrinter[printTypeCode];

                if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(
                        printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                }
                else
                {
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor()
                        .GenerateInputADOWithPrintTypeCode(
                            currentServiceReq != null ? currentServiceReq.TREATMENT_CODE : "",
                            printTypeCode,
                            this.currentModule != null ? currentModule.RoomId : 0);
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(
                        printTypeCode, fileName, data,
                        IsSignEmr ? MPS.ProcessorBase.PrintConfig.PreviewType.EmrShow
                                  : MPS.ProcessorBase.PrintConfig.PreviewType.Show,
                        printerName) { EmrInputADO = inputADO });
                    IsSignEmr = false;
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }
    }
}

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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000517.PDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceReqList
{
    public partial class frmServiceReqList : HIS.Desktop.Utility.FormBase
    {
        // Print code biểu in KQ tổng hợp xét nghiệm (chưa có trong PrintTypeCodeStore chung).
        private const string PRINT_TYPE_CODE__MPS000517 = "Mps000517";

        /// <summary>
        /// Nút "In KQ tổng hợp XN".
        /// Kiểm tra 4 bước trước khi gọi biểu in Mps000517:
        ///  1. Có y lệnh xét nghiệm được tích chọn.
        ///  2. Các y lệnh đã chọn cùng 1 bệnh nhân (TDL_PATIENT_ID).
        ///  3. Không còn xét nghiệm chưa có kết quả (V_LIS_SAMPLE.RESULT_TIME != null).
        ///  4. Đạt tất cả điều kiện -> gọi in.
        /// </summary>
        private void OnClickInKQTongHopXN(object sender, EventArgs e)
        {
            try
            {
                // Lấy danh sách y lệnh xét nghiệm đã tích chọn trên grid.
                List<ADO.ServiceReqADO> selectedTestReqs = GetSelectedTestServiceReqs();

                // Bước 1: Chưa chọn y lệnh xét nghiệm nào.
                if (selectedTestReqs == null || selectedTestReqs.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        Resources.ResourceMessage.ChuaChonYLenhXetNghiem,
                        Resources.ResourceMessage.ThongBao,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Bước 2: Các y lệnh đã chọn phải cùng 1 bệnh nhân.
                bool samePatient = selectedTestReqs
                    .Select(o => o.TDL_PATIENT_ID)
                    .Distinct()
                    .Count() <= 1;
                if (!samePatient)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        Resources.ResourceMessage.CacYLenhKhongCungBenhNhan,
                        Resources.ResourceMessage.CanhBao,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Bước 3: Kiểm tra còn xét nghiệm chưa có kết quả không.
                string testReqWithoutResult = GetTestServiceReqWithoutResult(selectedTestReqs);
                if (!string.IsNullOrEmpty(testReqWithoutResult))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        string.Format(Resources.ResourceMessage.CoXetNghiemChuaCoKetQua, testReqWithoutResult),
                        Resources.ResourceMessage.CanhBao,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Bước 4: Đạt tất cả điều kiện -> gọi biểu in Mps000517.
                PrintKQTongHopXN(selectedTestReqs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Lấy các y lệnh XÉT NGHIỆM đã tích chọn trên grid danh sách y lệnh.
        /// </summary>
        private List<ADO.ServiceReqADO> GetSelectedTestServiceReqs()
        {
            List<ADO.ServiceReqADO> result = new List<ADO.ServiceReqADO>();
            try
            {
                if (gridControlServiceReq.DataSource == null)
                    return result;

                var listData = (List<ADO.ServiceReqADO>)gridControlServiceReq.DataSource;
                result = listData
                    .Where(o => o.isCheck
                        && o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN)
                    .ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Tải V_LIS_SAMPLE theo từng mã y lệnh đã chọn và xác định các y lệnh
        /// còn xét nghiệm chưa có kết quả (RESULT_TIME IS NULL).
        /// Trả về chuỗi mô tả (mỗi dòng 1 y lệnh) nếu có; rỗng nếu tất cả đã có kết quả.
        /// </summary>
        private string GetTestServiceReqWithoutResult(List<ADO.ServiceReqADO> selectedTestReqs)
        {
            string result = "";
            try
            {
                WaitingManager.Show();

                // Tập mã y lệnh -> phòng thực hiện, dùng để hiển thị cảnh báo.
                var distinctReqs = selectedTestReqs
                    .Where(o => !string.IsNullOrEmpty(o.SERVICE_REQ_CODE))
                    .GroupBy(o => o.SERVICE_REQ_CODE)
                    .Select(g => g.First())
                    .ToList();

                // Các mã y lệnh còn mẫu chưa có kết quả.
                HashSet<string> codeWithoutResult = new HashSet<string>();

                CommonParam param = new CommonParam();
                foreach (var req in distinctReqs)
                {
                    LisSampleViewFilter sampleFilter = new LisSampleViewFilter();
                    sampleFilter.SERVICE_REQ_CODE__EXACT = req.SERVICE_REQ_CODE;

                    var samples = new BackendAdapter(param).Get<List<V_LIS_SAMPLE>>(
                        RequestUriStore.LIS_SAMPLE_GETVIEW,
                        ApiConsumers.LisConsumer,
                        sampleFilter,
                        HIS.Desktop.Controls.Session.SessionManager.ActionLostToken,
                        param);

                    // Có mẫu chưa trả kết quả -> y lệnh này chưa hoàn tất.
                    if (samples != null && samples.Any(o => o.RESULT_TIME == null))
                    {
                        codeWithoutResult.Add(req.SERVICE_REQ_CODE);
                    }
                }

                WaitingManager.Hide();

                if (codeWithoutResult.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var req in distinctReqs)
                    {
                        if (!codeWithoutResult.Contains(req.SERVICE_REQ_CODE))
                            continue;
                        if (sb.Length > 0) sb.Append(Environment.NewLine);
                        sb.AppendFormat("- {0} - {1}", req.SERVICE_REQ_CODE, req.EXECUTE_ROOM_NAME);
                    }
                    result = sb.ToString();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Gọi biểu in KQ tổng hợp xét nghiệm (Mps000517).
        /// </summary>
        private void PrintKQTongHopXN(List<ADO.ServiceReqADO> selectedTestReqs)
        {
            try
            {
                this.listServiceReqPrintKQTongHopXN = selectedTestReqs;

                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(
                    ApiConsumers.SarConsumer,
                    HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(),
                    GlobalVariables.TemnplatePathFolder);

                richEditorMain.RunPrintTemplate(PRINT_TYPE_CODE__MPS000517, DelegateRunPrinterMps000517);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // Danh sách y lệnh phục vụ build dữ liệu in Mps000517.
        private List<ADO.ServiceReqADO> listServiceReqPrintKQTongHopXN;

        /// <summary>
        /// Callback dựng dữ liệu và gọi MpsPrinter cho biểu in Mps000517 (KQ tổng hợp XN đa mẫu).
        /// </summary>
        private bool DelegateRunPrinterMps000517(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                var selectedTestReqs = this.listServiceReqPrintKQTongHopXN;
                if (selectedTestReqs == null || selectedTestReqs.Count == 0)
                    return false;

                WaitingManager.Show();

                CommonParam param = new CommonParam();
                long treatmentId = selectedTestReqs.First().TREATMENT_ID;

                // 1. Điều trị + đối tượng BHYT
                HIS_TREATMENT currentTreatment = LoadTreatmentForPrint(treatmentId, param);
                HIS_PATIENT_TYPE_ALTER patientTypeAlter = LoadPatientTypeAlterForPrint(treatmentId, param);

                // 2. Danh sách y lệnh (kiểu gốc HIS_SERVICE_REQ) — map từ ADO đang có trên grid
                List<HIS_SERVICE_REQ> currentServiceReqs = selectedTestReqs
                    .Select(o =>
                    {
                        HIS_SERVICE_REQ req = new HIS_SERVICE_REQ();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(req, o);
                        return req;
                    })
                    .ToList();

                // 3. Mẫu bệnh phẩm + kết quả xét nghiệm
                List<V_LIS_SAMPLE> currentSamples = LoadSamplesForPrint(selectedTestReqs, param);
                List<V_LIS_RESULT> lisResults = LoadLisResultsForPrint(currentSamples, param);

                // 4. Danh mục phục vụ render (tra cứu từ cache RAM)
                HashSet<string> resultServiceCodes = new HashSet<string>(
                    lisResults.Where(o => !string.IsNullOrEmpty(o.SERVICE_CODE)).Select(o => o.SERVICE_CODE));

                List<V_HIS_TEST_INDEX> testIndexs = BackendDataWorker.Get<V_HIS_TEST_INDEX>()
                    .Where(o => resultServiceCodes.Contains(o.SERVICE_CODE)).ToList();
                List<V_HIS_TEST_INDEX_RANGE> testIndexRanges = BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>();
                List<V_HIS_SERVICE> listService = BuildServiceListForPrint(resultServiceCodes);

                long genderId = selectedTestReqs.First().TDL_PATIENT_GENDER_ID ?? 0;

                // 5. Dựng PDO Mps000517 (đa mẫu -> serviceParent = null, processor tự gom theo mẫu)
                Mps000517PDO pdo = new Mps000517PDO(
                    patientTypeAlter,
                    currentTreatment,
                    currentSamples,
                    currentServiceReqs,
                    testIndexs,
                    lisResults,
                    testIndexRanges,
                    genderId,
                    listService,
                    null);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printCode))
                    printerName = GlobalVariables.dicPrinter[printCode];

                WaitingManager.Hide();

                if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(
                        printCode, fileName, pdo,
                        MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                }
                else
                {
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor()
                        .GenerateInputADOWithPrintTypeCode(
                            currentTreatment != null ? currentTreatment.TREATMENT_CODE : "",
                            printCode,
                            currentModule != null ? currentModule.RoomId : 0);

                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(
                        printCode, fileName, pdo,
                        MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName)
                    { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        /// <summary>Lấy điều trị theo TREATMENT_ID.</summary>
        private HIS_TREATMENT LoadTreatmentForPrint(long treatmentId, CommonParam param)
        {
            try
            {
                MOS.Filter.HisTreatmentFilter filter = new MOS.Filter.HisTreatmentFilter();
                filter.ID = treatmentId;
                var data = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(
                    RequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, filter, param);
                return data != null ? data.FirstOrDefault() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        /// <summary>Lấy đối tượng BHYT cuối cùng của điều trị.</summary>
        private HIS_PATIENT_TYPE_ALTER LoadPatientTypeAlterForPrint(long treatmentId, CommonParam param)
        {
            try
            {
                return new BackendAdapter(param).Get<HIS_PATIENT_TYPE_ALTER>(
                    "api/HisPatientTypeAlter/GetLastByTreatmentId", ApiConsumers.MosConsumer, treatmentId, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        /// <summary>Tải tất cả mẫu bệnh phẩm theo mã y lệnh đã chọn.</summary>
        private List<V_LIS_SAMPLE> LoadSamplesForPrint(List<ADO.ServiceReqADO> selectedTestReqs, CommonParam param)
        {
            List<V_LIS_SAMPLE> result = new List<V_LIS_SAMPLE>();
            try
            {
                var distinctCodes = selectedTestReqs
                    .Where(o => !string.IsNullOrEmpty(o.SERVICE_REQ_CODE))
                    .Select(o => o.SERVICE_REQ_CODE)
                    .Distinct()
                    .ToList();

                foreach (var code in distinctCodes)
                {
                    LisSampleViewFilter sampleFilter = new LisSampleViewFilter();
                    sampleFilter.SERVICE_REQ_CODE__EXACT = code;
                    var samples = new BackendAdapter(param).Get<List<V_LIS_SAMPLE>>(
                        RequestUriStore.LIS_SAMPLE_GETVIEW, ApiConsumers.LisConsumer, sampleFilter, param);
                    if (samples != null && samples.Count > 0)
                        result.AddRange(samples);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>Tải kết quả xét nghiệm theo từng mẫu.</summary>
        private List<V_LIS_RESULT> LoadLisResultsForPrint(List<V_LIS_SAMPLE> samples, CommonParam param)
        {
            List<V_LIS_RESULT> result = new List<V_LIS_RESULT>();
            try
            {
                var sampleIds = samples.Select(o => o.ID).Distinct().ToList();
                foreach (var sampleId in sampleIds)
                {
                    LisResultViewFilter resultFilter = new LisResultViewFilter();
                    resultFilter.SAMPLE_ID = sampleId;
                    var lisResults = new BackendAdapter(param).Get<List<V_LIS_RESULT>>(
                        "api/LisResult/GetView", ApiConsumers.LisConsumer, resultFilter, param);
                    if (lisResults != null && lisResults.Count > 0)
                        result.AddRange(lisResults);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>Dựng danh mục dịch vụ (kèm dịch vụ cha) phục vụ render biểu in.</summary>
        private List<V_HIS_SERVICE> BuildServiceListForPrint(HashSet<string> resultServiceCodes)
        {
            List<V_HIS_SERVICE> result = new List<V_HIS_SERVICE>();
            try
            {
                var allService = BackendDataWorker.Get<V_HIS_SERVICE>();
                result = allService.Where(o => resultServiceCodes.Contains(o.SERVICE_CODE)).ToList();

                // Bổ sung dịch vụ cha (nếu có) — biểu mẫu cần để gom nhóm.
                var parentIds = new HashSet<long>(
                    result.Where(o => o.PARENT_ID.HasValue).Select(o => o.PARENT_ID.Value));
                if (parentIds.Count > 0)
                {
                    var existingIds = new HashSet<long>(result.Select(o => o.ID));
                    var parents = allService.Where(o => parentIds.Contains(o.ID) && !existingIds.Contains(o.ID)).ToList();
                    result.AddRange(parents);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}

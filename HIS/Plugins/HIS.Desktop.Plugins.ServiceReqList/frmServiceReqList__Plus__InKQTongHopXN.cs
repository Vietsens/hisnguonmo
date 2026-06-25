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
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
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
        /// Callback dựng dữ liệu và gọi MpsPrinter cho biểu in Mps000517.
        /// </summary>
        private bool DelegateRunPrinterMps000517(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                // TODO[Mps000517]: Biểu in Mps000517 (PDO + MPS Processor) hiện CHƯA tồn tại trong repo.
                // Khi MPS-side bổ sung MPS.Processor.Mps000517.PDO.Mps000517PDO, hoàn thiện đoạn dưới:
                //   1. Load dữ liệu KQ tổng hợp theo this.listServiceReqPrintKQTongHopXN
                //      (treatment, service req, sere serv tein / kết quả XN...).
                //   2. Khởi tạo Mps000517PDO từ dữ liệu trên.
                //   3. Lấy printerName từ GlobalVariables.dicPrinter[printCode].
                //   4. (Tùy chọn) tạo EMR InputADO qua EmrGenerateProcessor.GenerateInputADOWithPrintTypeCode.
                //   5. Gọi MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(
                //          printCode, fileName, pdo, previewType, printerName) { EmrInputADO = inputADO });
                //      previewType theo ConfigApplications.CheDoInChoCacChucNangTrongPhanMem.
                Inventec.Common.Logging.LogSystem.Warn(
                    "Bieu in Mps000517 chua duoc cau hinh (PDO/Processor chua ton tai). printCode="
                    + printCode + ", fileName=" + fileName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
    }
}

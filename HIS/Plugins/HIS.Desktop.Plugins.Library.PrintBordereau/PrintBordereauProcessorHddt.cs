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
using HIS.Desktop.Plugins.Library.PrintBordereau.ADO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintBordereau
{
    /// <summary>
    /// PTTK 2724 - mục 3.3: render bảng kê thanh toán ra PDF (base64) để TransactionBill /
    /// TransactionList đính kèm lên cổng hóa đơn điện tử VNPT.
    ///
    /// KHÔNG đụng tới các method/menu in bảng kê 6556 giấy hiện có — chỉ thêm method mới.
    /// </summary>
    public partial class PrintBordereauProcessor
    {
        /// <summary>Kết quả PDF base64 của lần render gần nhất (set trong callback render).</summary>
        private string hddtPdfBase64Result;

        /// <summary>
        /// Render bảng kê theo mẫu HDDT (printTypeCode đọc từ config
        /// MOS.HIS_TRANSACTION.AUTO_ATTACH_BORDEREAU_HDDT__VNPT, VD "Mps000321") ra PDF base64.
        /// Plugin set <see cref="ADO.BordereauInitData.HddtInfo"/> (số hóa đơn + ngày xuất) trước khi gọi.
        ///
        /// Kỹ thuật: MpsPrinter.Run với PreviewType.SaveFile (không hiển thị preview/in) → nhận
        /// stream Excel (template FlexCel) → convert sang PDF bằng Inventec.Common.FileConvert →
        /// trả base64. KHÔNG gọi SOAP (do Library.ElectronicBill lo).
        /// </summary>
        /// <param name="printTypeCode">Mã mẫu in HDDT (đọc từ config).</param>
        /// <param name="initData">Dữ liệu đầu vào (đã set HddtInfo). Null → dùng dữ liệu khởi tạo qua constructor.</param>
        /// <returns>PDF dạng base64; null nếu thất bại.</returns>
        public string RenderHddtBordereauToPdf(string printTypeCode, BordereauInitData initData = null)
        {
            string pdfBase64 = null;
            try
            {
                if (String.IsNullOrWhiteSpace(printTypeCode))
                {
                    Inventec.Common.Logging.LogSystem.Warn("RenderHddtBordereauToPdf: printTypeCode rong, bo qua dinh kem.");
                    return null;
                }

                if (initData != null)
                    this.BordereauInitData = initData;

                // Nạp dữ liệu giống luồng in bảng kê thường (SereServ, TreatmentFee, Bill, Deposit, ...)
                this.InitData(this.BordereauInitData);
                this.LoadData();
                this.LoadTransactionView(); // Mps000321 cần danh sách giao dịch

                this.hddtPdfBase64Result = null;

                // Lấy template HDDT từ SAR (đồng bộ), sau đó build PDO + xuất PDF trong callback
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(
                    HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer,
                    HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(),
                    HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                richEditorMain.RunPrintTemplate(printTypeCode, DelegateRenderHddtToPdf);

                pdfBase64 = this.hddtPdfBase64Result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return pdfBase64;
        }

        /// <summary>
        /// Callback sau khi template HDDT được tải về: build Mps000321PDO (tái dùng BuildPdo của
        /// behavior) rồi MpsPrinter.Run(SaveFile) → Excel stream → convert PDF → lưu base64.
        /// </summary>
        private bool DelegateRenderHddtToPdf(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                MpsBehavior.Mps000321.Mps000321Behavior behavior = new MpsBehavior.Mps000321.Mps000321Behavior(
                    this.roomId, this.PatientTypeAlter, this.SereServs, this.DepartmentTrans, this.TreatmentFees,
                    this.Treatment, this.Patient, this.Rooms, this.Services, this.HeinServiceTypes, this.TotalDayTreatment,
                    this.StatusTreatmentOut, this.DepartmentName, this.RoomName, this.UserNameReturnResult,
                    this.Transactions, this.PayOption, this.transReq, this.lstConfig);
                behavior.HddtInfo = this.HddtInfo; // PTTK 2724 - mục 3.3: forward HDDT info xuống PDO

                MPS.Processor.Mps000321.PDO.Mps000321PDO rdo = behavior.BuildPdo();
                if (rdo == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("RenderHddtBordereauToPdf: khong build duoc PDO. printCode=" + printCode);
                    return false;
                }

                using (MemoryStream excelStream = new MemoryStream())
                {
                    MPS.ProcessorBase.Core.PrintData printData = new MPS.ProcessorBase.Core.PrintData(
                        printCode, fileName, rdo,
                        MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "")
                    {
                        saveMemoryStream = excelStream
                    };

                    bool printed = MPS.MpsPrinter.Run(printData);
                    if (printed && printData.saveMemoryStream != null && printData.saveMemoryStream.Length > 0)
                    {
                        using (MemoryStream pdfStream = new MemoryStream())
                        {
                            printData.saveMemoryStream.Position = 0;
                            bool converted = Inventec.Common.FileConvert.Convert.ExcelToPdfUsingFlex(
                                printData.saveMemoryStream, null, pdfStream, null);
                            if (converted && pdfStream.Length > 0)
                            {
                                this.hddtPdfBase64Result = System.Convert.ToBase64String(pdfStream.ToArray());
                                result = true;
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Warn("RenderHddtBordereauToPdf: convert Excel->PDF that bai. printCode=" + printCode);
                            }
                        }
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Warn("RenderHddtBordereauToPdf: MpsPrinter.Run(SaveFile) that bai hoac stream rong. printCode=" + printCode);
                    }
                }
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

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
using HIS.Desktop.Plugins.Library.ElectronicBill;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Plugins.Library.PrintBordereau;
using HIS.Desktop.Plugins.Library.PrintBordereau.ADO;
using HIS.Desktop.Plugins.Library.PrintBordereau.Base;
using HIS.Desktop.Plugins.TransactionBill.Config;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TransactionBill
{
    public partial class frmTransactionBill : HIS.Desktop.Utility.FormBase
    {
        /// <summary>
        /// Đính kèm bảng kê thanh toán (PDF) vào HĐĐT VNPT sau khi tạo hóa đơn thành công.
        ///
        /// Điều kiện chạy (ngược lại = bỏ qua, zero overhead, multi-site safe):
        ///   - Config MOS.HIS_TRANSACTION.AUTO_ATTACH_BORDEREAU_HDDT__VNPT có giá trị (PrintTypeCode)
        ///   - Nhà cung cấp HĐĐT là VNPT
        ///
        /// Phân tầng: plugin chỉ ĐIỀU PHỐI.
        ///   - Render PDF bảng kê: Library.PrintBordereau.RenderHddtBordereauToPdf
        ///   - Gọi SOAP đính kèm: Library.ElectronicBill (ATTACH_BORDEREAU) → framework Inventec.Common.ElectronicBill
        ///   - Lưu trạng thái: api/HisTransaction/UpdateBordereauAttachInfo
        ///
        /// Chạy ĐỘC LẬP: mọi lỗi đính kèm KHÔNG ảnh hưởng hóa đơn đã tạo thành công.
        /// Thất bại → STATUS giữ null ("Chưa đính kèm") để hiện trong danh sách rà soát, user gửi lại sau.
        /// </summary>
        /// <param name="transaction">Giao dịch hiện hành (V_HIS_TRANSACTION — có EINVOICE_TYPE_ID; HIS_TRANSACTION KHÔNG có)</param>
        /// <param name="electronicBillResult">Kết quả tạo HĐĐT của luồng "Lưu ký"</param>
        internal void ProcessAttachBordereauHddtVnpt(V_HIS_TRANSACTION transaction, ElectronicBillResult electronicBillResult)
        {
            try
            {
                // 1) Điều kiện bật/tắt — TẮT thì không làm gì thêm
                if (string.IsNullOrWhiteSpace(HisConfigCFG.AutoAttachBordereauHddtVnpt)) return;
                if (transaction == null || electronicBillResult == null) return;
                if (transaction.EINVOICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__VNPT) return;

                string printTypeCode = HisConfigCFG.AutoAttachBordereauHddtVnpt;

                // 2) Yêu cầu Library.PrintBordereau render bảng kê -> PDF base64
                ReloadMenuOption reloadMenuBordereau = new ReloadMenuOption();
                reloadMenuBordereau.ReloadMenu = ReloadMenuNull;
                reloadMenuBordereau.Type = ReloadMenuOption.MenuType.DYNAMIC;
                reloadMenuBordereau.BordereauPrint = BordereauPrint.Type.MPS_BASE;

                BordereauInitData bordereauInitData = new BordereauInitData();
                bordereauInitData.HddtInfo = new HddtInfoADO
                {
                    InvoiceNumOrder = electronicBillResult.InvoiceNumOrder,
                    InvoiceTime = electronicBillResult.InvoiceTime
                };

                PrintBordereauProcessor bordereauProcessor = new PrintBordereauProcessor(
                    this.currentModule != null ? this.currentModule.RoomId : 0,
                    this.currentModule != null ? this.currentModule.RoomTypeId : 0,
                    this.currentTreatment != null ? this.currentTreatment.ID : 0,
                    this.currentTreatment != null ? this.currentTreatment.PATIENT_ID : 0,
                    bordereauInitData,
                    reloadMenuBordereau);

                string pdfBase64 = bordereauProcessor.RenderHddtBordereauToPdf(printTypeCode, bordereauInitData);
                if (string.IsNullOrEmpty(pdfBase64))
                {
                    // Không render được -> coi như thất bại đính kèm, giữ STATUS null
                    XtraMessageBoxThatBaiDinhKem();
                    return;
                }

                // 3) Yêu cầu Library.ElectronicBill (VNPT) đính kèm — SOAP do framework lo
                // ElectronicBillDataInput.Transaction yêu cầu kiểu HIS_TRANSACTION -> map từ view
                HIS_TRANSACTION transactionForAttach = new HIS_TRANSACTION();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(transactionForAttach, transaction);

                ElectronicBillDataInput attachInput = new ElectronicBillDataInput();
                attachInput.Transaction = transactionForAttach;
                attachInput.EinvoiceTypeId = transaction.EINVOICE_TYPE_ID;
                attachInput.InvoiceCode = electronicBillResult.InvoiceCode;
                attachInput.AttachFileBase64 = pdfBase64;
                attachInput.AttachFileName = "Bang ke thanh toan.pdf";
                attachInput.IsSignFileAttach = 0;

                ElectronicBillProcessor electronicBillProcessor = new ElectronicBillProcessor(attachInput);
                ElectronicBillResult attachResult = electronicBillProcessor.Run(ElectronicBillType.ENUM.ATTACH_BORDEREAU);

                // 4) Xử lý kết quả
                if (attachResult == null || !attachResult.Success)
                {
                    // Thất bại: KHÔNG gọi API -> STATUS giữ null ("Chưa đính kèm") cho danh sách rà soát
                    XtraMessageBoxThatBaiDinhKem();
                    return;
                }

                // Thành công -> lưu trạng thái đính kèm.
                // SDO HisTransactionBordereauAttachInfoSDO (backend) gồm: BordereauAttachStatus + IDs.
                // Dùng anonymous object để không phụ thuộc compile vào kiểu SDO (BE deserialize theo tên field).
                CommonParam param = new CommonParam();
                var sdo = new
                {
                    BordereauAttachStatus = (short)1,
                    IDs = new List<long> { transaction.ID }
                };
                new BackendAdapter(param).Post<bool>(
                    RequestUriStore.HIS_TRANSACTION_UPDATE_BORDEREAU_ATTACH_INFO,
                    ApiConsumers.MosConsumer,
                    sdo,
                    param);
            }
            catch (Exception ex)
            {
                // Đính kèm chạy độc lập — lỗi KHÔNG ảnh hưởng hóa đơn đã tạo thành công
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void XtraMessageBoxThatBaiDinhKem()
        {
            try
            {
                MessageBox.Show(Base.ResourceMessageLang.GuiThongTinBangKeThatBai);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

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
using HIS.Desktop.Plugins.Library.ElectronicBill;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Plugins.TransactionList.Config;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.TransactionList
{
    public partial class frmTransactionList : HIS.Desktop.Utility.FormBase
    {
        /// <summary>
        /// Handler menu chuot phai "Gui dinh kem bang ke": render bang ke + dinh kem vao HDDT da co + thong bao ket qua.
        /// </summary>
        private void MouseRight_GuiDinhKemBangKe(V_HIS_TRANSACTION transaction)
        {
            if (transaction == null) return;
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                // Loading bao TOAN BO luong (render + dinh kem SOAP + update) — GuiDinhKemBangKe khong tu Show/Hide nua
                WaitingManager.Show();
                success = GuiDinhKemBangKe(transaction, ref param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                WaitingManager.Hide();
            }
            MessageManager.Show(this, param, success);

            // Dinh kem xong -> refresh grid de cot "Dinh kem bang ke" cap nhat + menu an o lan phai chuot sau
            if (success)
            {
                FillDataToGridTransaction(new CommonParam(0, (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize));
            }
        }

        /// <summary>
        /// Render bang ke + dinh kem vao HDDT (VNPT) + luu trang thai dinh kem.
        /// Dung chung cho luong "Xuat lai HDDT" va menu "Gui dinh kem bang ke".
        /// Chi chay khi config AutoAttachBordereauHddtVnpt co gia tri, provider la VNPT, va da co INVOICE_CODE.
        /// Render do Library.PrintBordereau, dinh kem (SOAP) do Library.ElectronicBill.
        /// </summary>
        private bool GuiDinhKemBangKe(V_HIS_TRANSACTION transaction, ref CommonParam param)
        {
            bool result = false;
            try
            {
                if (transaction == null) return result;

                string printTypeCode = HisConfigCFG.AutoAttachBordereauHddtVnpt;
                if (string.IsNullOrEmpty(printTypeCode)) return result;
                if (transaction.EINVOICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__VNPT) return result;
                if (string.IsNullOrWhiteSpace(transaction.INVOICE_CODE)) return result;

                if (param == null) param = new CommonParam();

                long treatmentId = transaction.TREATMENT_ID ?? 0;
                long patientId = transaction.TDL_PATIENT_ID ?? 0;
                if (treatmentId <= 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("GuiDinhKemBangKe: giao dich khong co TREATMENT_ID, khong render duoc bang ke. TransactionCode=" + transaction.TRANSACTION_CODE);
                    param.Messages.Add(Resources.ResourceMessage.GuiThongTinBangKeThatBai);
                    return result;
                }

                long roomId = this.currentModule != null ? this.currentModule.RoomId : 0;
                long roomTypeId = this.currentModule != null ? this.currentModule.RoomTypeId : 0;

                // 1. Render bang ke ra PDF base64 (khong preview/in). Truyen HddtInfo (so HD + ngay) de template in "Kem theo so hoa don / Ngay".
                //    Sau UpdateInvoiceInfo, transaction.EINVOICE_NUM_ORDER/EINVOICE_TIME da mang gia tri dung cho ca 2 luong (xuat lai / gui lai).
                HIS.Desktop.Plugins.Library.PrintBordereau.ADO.BordereauInitData bordereauInitData =
                    new HIS.Desktop.Plugins.Library.PrintBordereau.ADO.BordereauInitData
                    {
                        HddtInfo = new HIS.Desktop.Plugins.Library.PrintBordereau.ADO.HddtInfoADO
                        {
                            InvoiceNumOrder = transaction.EINVOICE_NUM_ORDER,
                            InvoiceTime = transaction.EINVOICE_TIME
                        }
                    };
                HIS.Desktop.Plugins.Library.PrintBordereau.PrintBordereauProcessor bordereauProc =
                    new HIS.Desktop.Plugins.Library.PrintBordereau.PrintBordereauProcessor(roomId, roomTypeId, treatmentId, patientId, bordereauInitData, null);
                string pdfBase64 = bordereauProc.RenderHddtBordereauToPdf(printTypeCode);

                if (string.IsNullOrEmpty(pdfBase64))
                {
                    param.Messages.Add(Resources.ResourceMessage.GuiThongTinBangKeThatBai);
                    return result;
                }

                // 2. Dinh kem PDF vao HDDT qua thu vien ElectronicBill (ATTACH_BORDEREAU)
                ElectronicBillResult attachResult = AttachBordereauToInvoice(transaction, pdfBase64);
                if (attachResult == null || !attachResult.Success)
                {
                    param.Messages.Add(Resources.ResourceMessage.GuiThongTinBangKeThatBai);
                    if (attachResult != null && attachResult.Messages != null && attachResult.Messages.Count > 0)
                    {
                        param.Messages.AddRange(attachResult.Messages);
                    }
                    param.Messages = param.Messages.Distinct().ToList();
                    return result;
                }

                // 3. Luu trang thai dinh kem (BORDEREAU_ATTACH_STATUS = 1). Loi -> giu status null.
                CommonParam paramUpdate = new CommonParam();
                MOS.SDO.HisTransactionBordereauAttachInfoSDO sdo = new MOS.SDO.HisTransactionBordereauAttachInfoSDO();
                sdo.Ids = new List<long> { transaction.ID };
                sdo.BordereauAttachStatus = 1;
                bool apiResult = new BackendAdapter(paramUpdate).Post<bool>(
                    RequestUri.HIS_TRANSACTION_UPDATE_BORDEREAU_ATTACH_INFO, ApiConsumers.MosConsumer, sdo, paramUpdate);
                if (apiResult)
                {
                    transaction.BORDEREAU_ATTACH_STATUS = 1;
                    result = true;
                }
                else
                {
                    param.Messages.Add(Resources.ResourceMessage.GuiThongTinBangKeThatBai);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Xay dung ElectronicBillDataInput cho HDDT da phat hanh + goi thu vien dinh kem file bang ke (ATTACH_BORDEREAU).
        /// </summary>
        private ElectronicBillResult AttachBordereauToInvoice(V_HIS_TRANSACTION transaction, string pdfBase64)
        {
            ElectronicBillResult result = new ElectronicBillResult();
            try
            {
                CommonParam param = new CommonParam();

                V_HIS_TREATMENT_FEE currentTreatment = new V_HIS_TREATMENT_FEE();
                if (transaction.TREATMENT_ID.HasValue)
                {
                    HisTreatmentFeeViewFilter filter = new HisTreatmentFeeViewFilter();
                    filter.ID = transaction.TREATMENT_ID;
                    var treatment = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_FEE>>(
                        "api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, filter, param);
                    if (treatment != null && treatment.Count > 0)
                    {
                        currentTreatment = treatment.First();
                    }
                }
                else
                {
                    currentTreatment.ID = -1;
                    currentTreatment.PATIENT_ID = transaction.TDL_PATIENT_ID ?? -1;
                }

                ElectronicBillDataInput dataInput = new ElectronicBillDataInput();
                dataInput.Branch = BackendDataWorker.Get<HIS_BRANCH>()
                    .FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                dataInput.Treatment = currentTreatment;
                dataInput.SymbolCode = transaction.SYMBOL_CODE;
                dataInput.TemplateCode = transaction.TEMPLATE_CODE;
                dataInput.EinvoiceTypeId = transaction.EINVOICE_TYPE_ID;
                dataInput.InvoiceCode = transaction.INVOICE_CODE;
                dataInput.IsTransactionList = true;
                dataInput.AttachFileBase64 = pdfBase64;
                dataInput.AttachFileName = "Bang ke thanh toan.pdf";
                dataInput.IsSignFileAttach = 0;

                HIS_TRANSACTION tran = new HIS_TRANSACTION();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(tran, transaction);
                dataInput.Transaction = tran;

                ElectronicBillProcessor electronicBillProcessor = new ElectronicBillProcessor(dataInput);
                result = electronicBillProcessor.Run(ElectronicBillType.ENUM.ATTACH_BORDEREAU);
            }
            catch (Exception ex)
            {
                result.Success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}

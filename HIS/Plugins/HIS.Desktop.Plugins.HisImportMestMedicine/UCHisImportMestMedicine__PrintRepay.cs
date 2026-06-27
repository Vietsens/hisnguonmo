/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * 42727 - Logic in phiếu hoàn ứng (MPS000113) cho phiếu nhập đã có REPAY_ID.
 * Sao chép pattern từ HIS.Desktop.Plugins.TransactionList.frmTransactionList__Plus__Print.
 */
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.HisImportMestMedicine
{
    public partial class UCHisImportMestMedicine
    {
        // Cache transaction đang được in để callback delegate đọc lại
        private V_HIS_TRANSACTION transactionPrintRepay = null;

        // Entry point - in phiếu hoàn ứng từ phiếu nhập đã có REPAY_ID
        internal void PrintRepayByImpMest(MOS.EFMODEL.DataModels.V_HIS_IMP_MEST impMest)
        {
            try
            {
                if (impMest == null || (impMest.REPAY_ID ?? 0) <= 0)
                    return;

                WaitingManager.Show();

                // Load V_HIS_TRANSACTION theo REPAY_ID của phiếu nhập
                HisTransactionViewFilter filter = new HisTransactionViewFilter();
                filter.ID = impMest.REPAY_ID.Value;
                var listTrans = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>(
                    "api/HisTransaction/GetView", ApiConsumers.MosConsumer, filter, null);

                WaitingManager.Hide();

                if (listTrans == null || listTrans.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn(
                        "Khong tim thay V_HIS_TRANSACTION theo REPAY_ID="
                        + Inventec.Common.Logging.LogUtil.TraceData(
                            Inventec.Common.Logging.LogUtil.GetMemberName(() => impMest), impMest));
                    return;
                }

                this.transactionPrintRepay = listTrans.First();

                // Gọi MPS template MPS000113 - Phiếu thu hoàn ứng
                Inventec.Common.RichEditor.RichEditorStore richStore = new Inventec.Common.RichEditor.RichEditorStore(
                    ApiConsumers.SarConsumer,
                    ConfigSystems.URI_API_SAR,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(),
                    GlobalVariables.TemnplatePathFolder);
                richStore.RunPrintTemplate(
                    HIS.Desktop.Print.PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuHoanUng_MPS000113,
                    this.DelegatePrintRepay);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // Callback của RichEditorStore - build PDO + gọi MpsPrinter
        private bool DelegatePrintRepay(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (printTypeCode == HIS.Desktop.Print.PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuHoanUng_MPS000113)
                {
                    InPhieuThuHoanUng(printTypeCode, fileName, ref result);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        // Build Mps000113PDO + in / preview - sao chép từ TransactionList
        private void InPhieuThuHoanUng(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.transactionPrintRepay == null || !this.transactionPrintRepay.TREATMENT_ID.HasValue)
                    return;

                WaitingManager.Show();

                // 1. Reload chính xác V_HIS_TRANSACTION
                HisTransactionViewFilter repayFilter = new HisTransactionViewFilter();
                repayFilter.ID = this.transactionPrintRepay.ID;
                var listRepay = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>(
                    "api/HisTransaction/GetView", ApiConsumers.MosConsumer, repayFilter, null);
                if (listRepay == null || listRepay.Count != 1)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Warn("Khong lay duoc V_HIS_TRANSACTION theo Id: " + this.transactionPrintRepay.ID);
                    return;
                }
                V_HIS_TRANSACTION repay = listRepay.First();

                // 2. Bệnh nhân
                V_HIS_PATIENT patient = null;
                if (this.transactionPrintRepay.TDL_PATIENT_ID.HasValue)
                {
                    HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                    patientFilter.ID = this.transactionPrintRepay.TDL_PATIENT_ID;
                    var listPatient = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>(
                        HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_PATIENT_GETVIEW,
                        ApiConsumers.MosConsumer, patientFilter, null);
                    if (listPatient != null && listPatient.Count > 0)
                        patient = listPatient.First();
                }

                // 3. Đối tượng BHYT + mức hưởng
                decimal ratio = 0;
                V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt = new V_HIS_PATIENT_TYPE_ALTER();
                HIS.Desktop.Print.PrintGlobalStore.LoadCurrentPatientTypeAlter(this.transactionPrintRepay.TREATMENT_ID.Value, 0, ref patyAlterBhyt);
                if (patyAlterBhyt != null && !string.IsNullOrEmpty(patyAlterBhyt.HEIN_CARD_NUMBER))
                {
                    ratio = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(
                        patyAlterBhyt.HEIN_TREATMENT_TYPE_CODE,
                        patyAlterBhyt.HEIN_CARD_NUMBER,
                        patyAlterBhyt.LEVEL_CODE,
                        patyAlterBhyt.RIGHT_ROUTE_CODE, patyAlterBhyt.FACILITY_CLASS, patyAlterBhyt.FORMER_LEVEL_CODE, (long)(patyAlterBhyt.CLASSIFY_POINT ?? 0)) ?? 0;
                }

                // 4. Khoa cuối
                HisDepartmentTranLastFilter departLastFilter = new HisDepartmentTranLastFilter();
                departLastFilter.TREATMENT_ID = this.transactionPrintRepay.TREATMENT_ID.Value;
                departLastFilter.BEFORE_LOG_TIME = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                var departmentTran = new BackendAdapter(new CommonParam()).Get<V_HIS_DEPARTMENT_TRAN>(
                    "api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, departLastFilter, null);

                // 5. Treatment fee
                CommonParam paramTreatment = new CommonParam();
                HisTreatmentFeeViewFilter feeFilter = new HisTreatmentFeeViewFilter();
                feeFilter.ID = this.transactionPrintRepay.TREATMENT_ID;
                var treatmentFee = new BackendAdapter(paramTreatment).Get<List<V_HIS_TREATMENT_FEE>>(
                    "api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, feeFilter, paramTreatment);
                if (treatmentFee == null || treatmentFee.Count == 0)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Warn("Khong lay duoc V_HIS_TREATMENT_FEE: " + this.transactionPrintRepay.TREATMENT_ID);
                    return;
                }

                // 6. Tất cả giao dịch của điều trị (chưa hủy)
                HisTransactionViewFilter transFilter = new HisTransactionViewFilter();
                transFilter.TREATMENT_ID = this.transactionPrintRepay.TREATMENT_ID;
                transFilter.IS_CANCEL = false;
                List<V_HIS_TRANSACTION> transactions = new BackendAdapter(paramTreatment).Get<List<V_HIS_TRANSACTION>>(
                    "api/HisTransaction/GetView", ApiConsumers.MosConsumer, transFilter, paramTreatment);
                if (transactions == null) transactions = new List<V_HIS_TRANSACTION>();

                // 7. Build PDO + Print
                MPS.Processor.Mps000113.PDO.Mps000113PDO pdo = new MPS.Processor.Mps000113.PDO.Mps000113PDO(
                    repay,
                    patient,
                    ratio,
                    null,
                    departmentTran,
                    treatmentFee.First(),
                    transactions);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    printerName = GlobalVariables.dicPrinter[printTypeCode];

                Inventec.Common.SignLibrary.ADO.InputADO inputADO =
                    new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor()
                        .GenerateInputADOWithPrintTypeCode(
                            repay != null ? repay.TREATMENT_CODE : "",
                            printTypeCode,
                            this.currentModule != null ? this.currentModule.RoomId : 0);

                WaitingManager.Hide();

                MPS.ProcessorBase.PrintConfig.PreviewType previewType =
                    ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2
                        ? MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow
                        : MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog;

                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(
                    printTypeCode, fileName, pdo, previewType, printerName)
                {
                    EmrInputADO = inputADO
                });
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                WaitingManager.Hide();
            }
        }
    }
}

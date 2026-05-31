/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisPatientPackage.ADO;
using HIS.Desktop.Plugins.HisPatientPackage.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LibraryMessage;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Message = Inventec.Desktop.Common.LibraryMessage.Message;

namespace HIS.Desktop.Plugins.HisPatientPackage
{
    public partial class UcHisPatientPackage
    {
        // Mã loại phiếu in (SAR report) cho phiếu gói dịch vụ — đồng bộ với PatientPackageRegister.
        private const string PRINT_TYPE_CODE__MPS000514 = "Mps000514";

        /// <summary>
        /// In phiếu thông tin gói dịch vụ — pattern y hệt PatientPackageRegister:
        /// RichEditorStore.RunPrintTemplate → DelegatePrintMps000514 → MPS.MpsPrinter.Run.
        /// </summary>
        private void PrintProcess(PatientPackageADO row)
        {
            try
            {
                if (row == null) { ShowChonGoi(); return; }

                // Lưu lại context gói/bệnh nhân đang in để DelegatePrintMps000514 dùng.
                currentPrintRow = row;

                // Tải template phiếu in theo mã loại phiếu, rồi gọi delegate sinh dữ liệu + in.
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(
                    ApiConsumers.SarConsumer,
                    HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(),
                    HIS.Desktop.LocalStorage.LocalData.GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PRINT_TYPE_CODE__MPS000514, DelegatePrintMps000514);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Callback sinh dữ liệu + gọi MpsPrinter — y hệt PatientPackageRegister.</summary>
        private bool DelegatePrintMps000514(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                if (printCode == PRINT_TYPE_CODE__MPS000514)
                {
                    InPhieuGoiDichVu(printCode, fileName, ref result);
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// In phiếu gói dịch vụ (MPS000514). LẤY DỮ LIỆU MỚI NHẤT từ server theo ID gói đang chọn
        /// (HIS_PATIENT_PACKAGE + HIS_PATIENT_PACKAGE_DT + HIS_PATIENT) để in đúng số liệu mới nhất.
        /// </summary>
        private void InPhieuGoiDichVu(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (currentPrintRow == null) return;

                WaitingManager.Show();
                CommonParam param = new CommonParam();

                // Gói (data mới nhất theo ID).
                HisPatientPackageFilter pkgFilter = new HisPatientPackageFilter();
                pkgFilter.ID = currentPrintRow.ID;
                List<HIS_PATIENT_PACKAGE> pkgs = new BackendAdapter(param)
                    .Get<List<HIS_PATIENT_PACKAGE>>(
                        HisRequestUriStore.MOSHIS_HIS_PATIENT_PACKAGE_GET,
                        ApiConsumers.MosConsumer, pkgFilter, param);
                HIS_PATIENT_PACKAGE pkg = pkgs != null ? pkgs.FirstOrDefault() : null;

                // Chi tiết dịch vụ trong gói (data mới nhất).
                HisPatientPackageDtFilter dtFilter = new HisPatientPackageDtFilter();
                dtFilter.PATIENT_PACKAGE_ID = currentPrintRow.ID;
                dtFilter.IS_ACTIVE = 1;
                List<HIS_PATIENT_PACKAGE_DT> details = new BackendAdapter(param)
                    .Get<List<HIS_PATIENT_PACKAGE_DT>>(
                        HisRequestUriStore.MOSHIS_HIS_PATIENT_PACKAGE_DT_GET,
                        ApiConsumers.MosConsumer, dtFilter, param);

                // Bệnh nhân — view gói chỉ giữ PATIENT_ID, phải load thêm HIS_PATIENT cho PDO.
                HIS_PATIENT patient = null;
                if (currentPrintRow.PATIENT_ID > 0)
                {
                    HisPatientFilter patFilter = new HisPatientFilter();
                    patFilter.ID = currentPrintRow.PATIENT_ID;
                    List<HIS_PATIENT> patients = new BackendAdapter(param)
                        .Get<List<HIS_PATIENT>>(
                            HisRequestUriStore.MOSHIS_HIS_PATIENT_GET,
                            ApiConsumers.MosConsumer, patFilter, param);
                    patient = patients != null ? patients.FirstOrDefault() : null;
                }

                if (pkg == null || patient == null)
                {
                    WaitingManager.Hide();
                    XtraMessageBox.Show(
                        ResourceMessage.KhongLayDuocDuLieuGoiDeIn,
                        MessageUtil.GetMessage(Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                MPS.Processor.Mps000514.PDO.Mps000514PDO pdo = new MPS.Processor.Mps000514.PDO.Mps000514PDO(
                    patient,
                    pkg,
                    details ?? new List<HIS_PATIENT_PACKAGE_DT>());

                string printerName = "";
                var dicPrinter = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicPrinter;
                if (dicPrinter != null && dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = dicPrinter[printTypeCode];
                }

                WaitingManager.Hide();

                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(
                    printTypeCode, fileName, pdo,
                    MPS.ProcessorBase.PrintConfig.PreviewType.Show,
                    printerName));
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

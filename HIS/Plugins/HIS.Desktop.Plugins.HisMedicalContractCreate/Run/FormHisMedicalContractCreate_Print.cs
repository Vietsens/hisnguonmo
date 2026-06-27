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
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisMedicalContractCreate.Run
{
    public partial class FormHisMedicalContractCreate
    {
        private Inventec.Common.RichEditor.RichEditorStore richEditorPrint;

        // Dữ liệu phục vụ in phiếu hợp đồng (Mps000518)
        private V_HIS_MEDICAL_CONTRACT printContract;
        private List<V_HIS_MEDI_CONTRACT_METY> printListMety;
        private List<V_HIS_MEDI_CONTRACT_MATY> printListMaty;
        private HIS_SUPPLIER printSupplier;
        private List<V_HIS_MEDICINE_TYPE> printListMedicineType;
        private List<V_HIS_MATERIAL_TYPE> printListMaterialType;

        private void bbtnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPrint_Click(null, null);
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
                if (this.medicalContractId <= 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Vui lòng lưu hợp đồng trước khi in.", Resources.ResourceLanguageManager.ThongBao);
                    return;
                }

                PrintMedicalContract(this.medicalContractId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// In phiếu hợp đồng theo ID hợp đồng vừa tạo/sửa.
        /// Tải dữ liệu hợp đồng + chi tiết thuốc + chi tiết vật tư + danh mục nhà cung cấp
        /// rồi gọi hàm khởi tạo MPS000518 để in.
        /// </summary>
        private void PrintMedicalContract(long medicalContractId)
        {
            try
            {
                WaitingManager.Show();

                // 1. Hợp đồng - api/HisMedicalContract/GetView (ID = medicalContractId)
                CommonParam paramContract = new CommonParam();
                HisMedicalContractViewFilter contractFilter = new HisMedicalContractViewFilter();
                contractFilter.ID = medicalContractId;
                var contracts = new BackendAdapter(paramContract).Get<List<V_HIS_MEDICAL_CONTRACT>>("api/HisMedicalContract/GetView", ApiConsumers.MosConsumer, contractFilter, paramContract);
                V_HIS_MEDICAL_CONTRACT contract = (contracts != null) ? contracts.FirstOrDefault() : null;
                if (contract == null)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Warn("Khong tim thay hop dong de in, MEDICAL_CONTRACT_ID = " + medicalContractId);
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy hợp đồng để in.", Resources.ResourceLanguageManager.ThongBao);
                    return;
                }

                // 2. Chi tiết thuốc - api/HisMediContractMety/GetView (MEDICAL_CONTRACT_ID = medicalContractId)
                CommonParam paramMety = new CommonParam();
                HisMediContractMetyViewFilter metyFilter = new HisMediContractMetyViewFilter();
                metyFilter.MEDICAL_CONTRACT_ID = medicalContractId;
                var listMety = new BackendAdapter(paramMety).Get<List<V_HIS_MEDI_CONTRACT_METY>>("api/HisMediContractMety/GetView", ApiConsumers.MosConsumer, metyFilter, paramMety);

                // 3. Chi tiết vật tư - api/HisMediContractMaty/GetView (MEDICAL_CONTRACT_ID = medicalContractId)
                CommonParam paramMaty = new CommonParam();
                HisMediContractMatyViewFilter matyFilter = new HisMediContractMatyViewFilter();
                matyFilter.MEDICAL_CONTRACT_ID = medicalContractId;
                var listMaty = new BackendAdapter(paramMaty).Get<List<V_HIS_MEDI_CONTRACT_MATY>>("api/HisMediContractMaty/GetView", ApiConsumers.MosConsumer, matyFilter, paramMaty);

                // 4. Nhà cung cấp (HIS_SUPPLIER) lấy từ cache theo SUPPLIER_ID của hợp đồng
                HIS_SUPPLIER supplier = BackendDataWorker.Get<HIS_SUPPLIER>().FirstOrDefault(o => o.ID == contract.SUPPLIER_ID);

                this.printContract = contract;
                this.printSupplier = supplier;
                this.printListMety = listMety ?? new List<V_HIS_MEDI_CONTRACT_METY>();
                this.printListMaty = listMaty ?? new List<V_HIS_MEDI_CONTRACT_MATY>();

                // Danh mục thuốc/vật tư (lọc theo các ID xuất hiện trong hợp đồng) truyền vào MPS
                // để MPS bổ sung hoạt chất, hàm lượng, dạng BC, hãng SX, nước SX (view hợp đồng không trả về).
                var metyTypeIds = this.printListMety.Select(o => o.MEDICINE_TYPE_ID).Distinct().ToList();
                this.printListMedicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>()
                    .Where(o => metyTypeIds.Contains(o.ID)).ToList();
                var matyTypeIds = this.printListMaty.Select(o => o.MATERIAL_TYPE_ID).Distinct().ToList();
                this.printListMaterialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>()
                    .Where(o => matyTypeIds.Contains(o.ID)).ToList();

                richEditorPrint = new Inventec.Common.RichEditor.RichEditorStore(
                    ApiConsumers.SarConsumer,
                    HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(),
                    HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                richEditorPrint.RunPrintTemplate("Mps000518", DelegateRunPrinterMedicalContract);

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinterMedicalContract(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                WaitingManager.Show();
                switch (printCode)
                {
                    case "Mps000518":
                        // Hàm khởi tạo MPS000518 - truyền vào hợp đồng + nhà cung cấp + chi tiết thuốc + chi tiết vật tư
                        MPS.Processor.Mps000518.PDO.Mps000518PDO mps000518PDO = new MPS.Processor.Mps000518.PDO.Mps000518PDO(
                            this.printContract,
                            this.printSupplier,
                            this.printListMety,
                            this.printListMaty,
                            this.printListMedicineType,
                            this.printListMaterialType);

                        string printerName = "";
                        if (GlobalVariables.dicPrinter != null && GlobalVariables.dicPrinter.ContainsKey(printCode))
                        {
                            printerName = GlobalVariables.dicPrinter[printCode];
                        }

                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printCode, fileName, mps000518PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName));
                        break;
                    default:
                        break;
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
    }
}

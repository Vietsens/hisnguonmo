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
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.PharmacyCashier.Config;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.PharmacyCashier
{
    public partial class frmPharmacyCashier : FormBase
    {
        private void PrintPhieuXuatBanMps092()
        {
            try
            {
                if (this.resultSdo == null || this.resultSdo.ExpMest == null) return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate("Mps000092", DeletegatePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintHoaDonDichVuMps106()
        {
            try
            {
                if (this.resultSdo == null || this.resultSdo.ServiceInvoices == null || this.resultSdo.ServiceInvoices.Count <= 0) return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate("Mps000106", DeletegatePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintBienLaiDichVuMps343()
        {
            try
            {
                if (this.resultSdo == null || this.resultSdo.ServiceReciepts == null || this.resultSdo.ServiceReciepts.Count <= 0) return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate("Mps000343", DeletegatePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintHoaDonBanThuocMps344()
        {
            try
            {
                if (this.resultSdo == null || this.expMestBill == null) return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate("Mps000344", DeletegatePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DeletegatePrintTemplate(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000343":
                        InBienLaiDichVu(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000106":
                        InHoaDonDichVu(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000344":
                        InHoaDonBanThuoc(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000092":
                        InPhieuXuatBan(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void InHoaDonDichVu(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.resultSdo == null || this.resultSdo.ServiceInvoices == null || this.resultSdo.ServiceInvoices.Count <= 0) return;
                WaitingManager.Show();
                string Print106Type = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Print.TransactionDetail");
                HisTransactionViewFilter tranFilter = new HisTransactionViewFilter();
                tranFilter.IDs = this.resultSdo.ServiceInvoices.Select(s => s.ID).ToList();
                List<V_HIS_TRANSACTION> listTran = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, tranFilter, null);

                var listSereServ = new List<V_HIS_SERE_SERV>();
                HisSereServBillViewFilter ssBillFilter = new HisSereServBillViewFilter();
                ssBillFilter.BILL_IDs = this.resultSdo.ServiceInvoices.Select(s => s.ID).ToList();
                var hisSSBills = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_BILL>>("api/HisSereServBill/GetView", ApiConsumers.MosConsumer, ssBillFilter, null);
                if ((hisSSBills == null || hisSSBills.Count <= 0) && (this.resultSdo.InvoiceBillGoods == null || this.resultSdo.InvoiceBillGoods.Count <= 0))
                {
                    throw new Exception("Khong co InvoiceBillGoods va khong lay duoc SereServBill theo BillIds: " + this.resultSdo.ServiceInvoices.Select(s => s.ID).ToList());
                }

                if (Print106Type != "1")
                {
                    HisSereServViewFilter ssFilter = new HisSereServViewFilter();
                    ssFilter.IDs = hisSSBills.Select(s => s.SERE_SERV_ID).ToList();
                    ssFilter.TREATMENT_ID = this.resultSdo.ServiceInvoices.FirstOrDefault().TREATMENT_ID;
                    listSereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, ssFilter, null);
                }
                else
                {
                    HisSereServViewFilter ssFilter = new HisSereServViewFilter();
                    ssFilter.TREATMENT_ID = this.resultSdo.ServiceInvoices.FirstOrDefault().TREATMENT_ID;
                    listSereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, ssFilter, null);

                    if (listSereServ != null && listSereServ.Count > 0)
                    {
                        listSereServ = listSereServ.Where(o => o.IS_NO_PAY != 1 && o.IS_NO_EXECUTE != 1).ToList();
                        if (hisSSBills != null && hisSSBills.Count > 0)
                        {
                            listSereServ = listSereServ.Where(o => hisSSBills.Select(s => s.SERE_SERV_ID).Contains(o.ID) || o.VIR_TOTAL_PATIENT_PRICE == 0).ToList();
                        }
                        else
                        {
                            listSereServ = listSereServ.Where(o => o.VIR_TOTAL_PATIENT_PRICE == 0).ToList();
                        }
                    }
                }

                HisPatientTypeAlterViewAppliedFilter ptAlterAppFilter = new HisPatientTypeAlterViewAppliedFilter();
                ptAlterAppFilter.TreatmentId = this.resultSdo.ServiceInvoices.FirstOrDefault().TREATMENT_ID ?? 0;
                ptAlterAppFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, ptAlterAppFilter, null);

                // tính mức hưởng của thẻ
                string levelCode = HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT;
                string ratio_text = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentPatientTypeAlter.HEIN_CARD_NUMBER, currentPatientTypeAlter.LEVEL_CODE, currentPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100) + "";

                decimal totalDeposit = GetDepositAmount(this.resultSdo.ServiceInvoices.FirstOrDefault().TREATMENT_ID);
                HIS_TREATMENT treatment = GetTreatment(this.resultSdo.ServiceInvoices.FirstOrDefault().TREATMENT_ID);

                WaitingManager.Hide();
                foreach (V_HIS_TRANSACTION transaction in listTran)
                {
                    MPS.Processor.Mps000106.PDO.Mps000106ADO ado = new MPS.Processor.Mps000106.PDO.Mps000106ADO();
                    ado.PatientTypeBHYT = HisConfigCFG.PatientTypeId__BHYT;
                    ado.PatientTypeVP = HisConfigCFG.PATIENT_TYPE_ID__IS_FEE;

                    List<V_HIS_SERE_SERV_BILL> ssBills = hisSSBills != null ? hisSSBills.Where(o => o.BILL_ID == transaction.ID).ToList() : null;
                    List<V_HIS_SERE_SERV> sereServs = null;
                    if (Print106Type != "1")
                    {
                        sereServs = listSereServ != null ? listSereServ.Where(o => ssBills != null && ssBills.Any(a => a.SERE_SERV_ID == o.ID)).ToList() : null;
                    }
                    else
                    {
                        sereServs = listSereServ != null ? listSereServ.Where(o => (ssBills != null && ssBills.Any(a => a.SERE_SERV_ID == o.ID)) || o.VIR_TOTAL_PATIENT_PRICE == 0).ToList() : null;
                    }

                    List<HIS_BILL_GOODS> billGoods = this.resultSdo.InvoiceBillGoods != null ? this.resultSdo.InvoiceBillGoods.Where(o => o.BILL_ID == transaction.ID).ToList() : null;

                    MPS.Processor.Mps000106.PDO.Mps000106PDO rdo = new MPS.Processor.Mps000106.PDO.Mps000106PDO(transaction, sereServs, ssBills, billGoods, treatment, totalDeposit, 0, currentPatientTypeAlter, null, ratio_text, ado, BackendDataWorker.Get<HIS_DEPARTMENT>());
                    WaitingManager.Hide();

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((transaction != null ? transaction.TREATMENT_CODE : ""), printTypeCode, currentModuleBase.RoomId);

                    MPS.ProcessorBase.Core.PrintData printdata = null;
                    if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2
                        || checkIsPrintNow.Checked)
                    {
                        printdata = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog };
                    }
                    else
                    {
                        printdata = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog };
                    }

                    result = MPS.MpsPrinter.Run(printdata);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InBienLaiDichVu(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.resultSdo == null || this.resultSdo.ServiceReciepts == null || this.resultSdo.ServiceReciepts.Count <= 0) return;
                WaitingManager.Show();

                HisTransactionViewFilter tranFilter = new HisTransactionViewFilter();
                tranFilter.IDs = this.resultSdo.ServiceReciepts.Select(s => s.ID).ToList();
                List<V_HIS_TRANSACTION> listTran = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, tranFilter, null);

                if ((this.resultSdo.RecieptSereServBills == null || this.resultSdo.RecieptSereServBills.Count <= 0) && (this.resultSdo.RecieptBillGoods == null || this.resultSdo.RecieptBillGoods.Count <= 0))
                {
                    throw new Exception("Khong co InvoiceBillGoods vaSereServBill theo BillIds: " + this.resultSdo.ServiceReciepts.Select(s => s.ID).ToList());
                }

                WaitingManager.Hide();
                foreach (V_HIS_TRANSACTION transaction in listTran)
                {
                    MPS.Processor.Mps000106.PDO.Mps000106ADO ado = new MPS.Processor.Mps000106.PDO.Mps000106ADO();
                    ado.PatientTypeBHYT = HisConfigCFG.PatientTypeId__BHYT;
                    ado.PatientTypeVP = HisConfigCFG.PATIENT_TYPE_ID__IS_FEE;

                    List<HIS_SERE_SERV_BILL> ssBills = this.resultSdo.RecieptSereServBills != null ? this.resultSdo.RecieptSereServBills.Where(o => o.BILL_ID == transaction.ID).ToList() : null;
                    List<HIS_BILL_GOODS> billGoods = this.resultSdo.RecieptBillGoods != null ? this.resultSdo.RecieptBillGoods.Where(o => o.BILL_ID == transaction.ID).ToList() : null;

                    MPS.Processor.Mps000343.PDO.Mps000343PDO rdo = new MPS.Processor.Mps000343.PDO.Mps000343PDO(transaction, ssBills, billGoods, BackendDataWorker.Get<HIS_SERVICE_UNIT>());
                    WaitingManager.Hide();

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((transaction != null ? transaction.TREATMENT_CODE : ""), printTypeCode, currentModuleBase.RoomId);

                    MPS.ProcessorBase.Core.PrintData printdata = null;
                    if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2
                        || checkIsPrintNow.Checked)
                    {
                        printdata = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog };
                    }
                    else
                    {
                        printdata = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog };
                    }

                    result = MPS.MpsPrinter.Run(printdata);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InHoaDonBanThuoc(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.resultSdo == null || this.expMestBill == null) return;
                WaitingManager.Show();

                HisExpMestMedicineViewFilter medicineFilter = new HisExpMestMedicineViewFilter();
                medicineFilter.EXP_MEST_ID = resultSdo.ExpMest.ID;
                List<V_HIS_EXP_MEST_MEDICINE> medicines = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, medicineFilter, null);

                HisExpMestMaterialViewFilter materialFilter = new HisExpMestMaterialViewFilter();
                materialFilter.EXP_MEST_ID = resultSdo.ExpMest.ID;
                List<V_HIS_EXP_MEST_MATERIAL> materials = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, materialFilter, null);

                HisBillGoodsFilter bgFilter = new HisBillGoodsFilter();
                bgFilter.BILL_ID = this.expMestBill.ID;
                List<HIS_BILL_GOODS> billGoods = new BackendAdapter(new CommonParam()).Get<List<HIS_BILL_GOODS>>("api/HisBillGoods/Get", ApiConsumers.MosConsumer, bgFilter, null);

                MPS.Processor.Mps000344.PDO.Mps000344PDO rdo = new MPS.Processor.Mps000344.PDO.Mps000344PDO(this.expMestBill, new List<HIS_EXP_MEST>() { this.resultSdo.ExpMest }, medicines, materials, billGoods);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                MPS.ProcessorBase.Core.PrintData printdata = null;
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2
                    || checkIsPrintNow.Checked)
                {
                    printdata = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                }
                else
                {
                    printdata = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                }

                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(printdata);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuXuatBan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.resultSdo == null || this.resultSdo.ExpMest == null) return;
                WaitingManager.Show();

                CommonParam param = new CommonParam();

                HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                expMestFilter.ID = this.resultSdo.ExpMest.ID;
                List<V_HIS_EXP_MEST> expMests = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, expMestFilter, param);

                HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                expMestMedicineFilter.EXP_MEST_ID = this.resultSdo.ExpMest.ID;
                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetVIew", ApiConsumers.MosConsumer, expMestMedicineFilter, param);

                HisExpMestMaterialViewFilter expMestMaterialFilter = new HisExpMestMaterialViewFilter();
                expMestMaterialFilter.EXP_MEST_ID = this.resultSdo.ExpMest.ID;
                List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetVIew", ApiConsumers.MosConsumer, expMestMaterialFilter, param);

                HisImpMestFilter impMestFilter = new HisImpMestFilter();
                impMestFilter.MOBA_EXP_MEST_ID = this.resultSdo.ExpMest.ID;
                List<V_HIS_IMP_MEST> impMests = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetView", ApiConsumers.MosConsumer, impMestFilter, param);

                MPS.Processor.Mps000092.PDO.Mps000092PDO rdo = new MPS.Processor.Mps000092.PDO.Mps000092PDO(expMests, expMestMedicines, expMestMaterials, null, impMests);
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                MPS.ProcessorBase.Core.PrintData printdata = null;
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2
                    || checkIsPrintNow.Checked)
                {
                    printdata = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                }
                else
                {
                    printdata = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                }

                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(printdata);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickInHoaDonDichVu(object sender, EventArgs e)
        {
            try
            {
                this.PrintHoaDonDichVuMps106();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickInHoaDonBanThuoc(object sender, EventArgs e)
        {
            try
            {
                this.PrintHoaDonBanThuocMps344();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickInBienLaiBanThuoc(object sender, EventArgs e)
        {
            try
            {
                //this.PrintBienLaiThuocMps342();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickInPhieuXuatBan(object sender, EventArgs e)
        {
            try
            {
                this.PrintPhieuXuatBanMps092();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HIS_TREATMENT GetTreatment(long? treatmentId)
        {
            HIS_TREATMENT result = new HIS_TREATMENT();
            try
            {
                if (treatmentId.HasValue)
                {
                    HisTreatmentFilter filter = new HisTreatmentFilter();
                    filter.ID = treatmentId;
                    var apiresult = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, null);
                    if (apiresult != null && apiresult.Count > 0)
                    {
                        result = apiresult.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                result = new HIS_TREATMENT();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private decimal GetDepositAmount(long? treatmentId)
        {
            decimal result = 0;
            try
            {
                if (treatmentId.HasValue)
                {
                    HisTransactionFilter filter = new HisTransactionFilter();
                    filter.TREATMENT_ID = treatmentId;
                    filter.TRANSACTION_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU;
                    var apiresult = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_TRANSACTION>>("api/HisTransaction/Get", ApiConsumers.MosConsumer, filter, null);
                    if (apiresult != null && apiresult.Count > 0)
                    {
                        foreach (var item in apiresult)
                        {
                            if (item.IS_CANCEL != 1)
                            {
                                result += item.AMOUNT;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = 0;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void CallModuleShowPrintLog(string printTypeCode, string uniqueCode)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(printTypeCode) && !String.IsNullOrWhiteSpace(uniqueCode))
                {
                    //goi modul
                    HIS.Desktop.ADO.PrintLogADO ado = new HIS.Desktop.ADO.PrintLogADO(printTypeCode, uniqueCode);

                    List<object> listArgs = new List<object>();
                    listArgs.Add(ado);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("Inventec.Desktop.Plugins.PrintLog", currentModuleBase.RoomId, currentModuleBase.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}

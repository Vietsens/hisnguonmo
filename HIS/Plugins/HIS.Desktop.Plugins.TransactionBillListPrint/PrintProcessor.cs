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
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TransactionBillListPrint
{
    class PrintProcessor
    {
        internal const string PrintTypeCode__Mps000147 = "Mps000147";
        internal const string PrintTypeCode__Mps000148 = "Mps000148";
        internal const string PrintTypeCode__Mps000092 = "Mps000092";

        Inventec.Common.RichEditor.RichEditorStore richEditorMain;
        List<V_HIS_TRANSACTION> ListTransaction;
        List<V_HIS_TRANSACTION> ListMps147;
        List<V_HIS_TRANSACTION> ListMps148;
        bool PrintNow;

        public PrintProcessor(List<V_HIS_TRANSACTION> listData)
        {
            try
            {
                this.ListTransaction = listData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void Print(bool p, string printTypeCode)
        {
            try
            {
                if (ListTransaction != null && ListTransaction.Count > 0)
                {
                    PrintNow = p;

                    if (p)
                    {
                        CreateThreadPrintNow(printTypeCode);
                    }
                    else
                    {
                        RichEditPrintByCode(printTypeCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinter(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printCode)
                {
                    case PrintTypeCode__Mps000147:
                        ListMps147 = ListTransaction.Where(o => o.BILL_TYPE_ID == 2).ToList();
                        result = DelegateRunPrinterMps147(printCode, fileName);
                        break;
                    case PrintTypeCode__Mps000092:
                        result = DelegateRunPrinterMps92(printCode, fileName);
                        break;
                    case PrintTypeCode__Mps000148:
                        ListMps148 = ListTransaction.Where(o => o.BILL_TYPE_ID != 2).ToList();
                        result = DelegateRunPrinterMps148(printCode, fileName);
                        break;
                    default:
                        //MessageBox.Show("Dữ liệu mã biểu in không hợp lệ");
                        break;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool DelegateRunPrinterMps147(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("DelegateRunPrinterMps147. 1");
                if (ListMps147 != null && ListMps147.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("DelegateRunPrinterMps147. 2");
                    foreach (var transaction in ListMps147)
                    {
                        MPS.Processor.Mps000147.PDO.Mps000147PDO rdo = new MPS.Processor.Mps000147.PDO.Mps000147PDO(transaction);
                        PrintData(printCode, fileName, rdo, ref result);
                    }
                    Inventec.Common.Logging.LogSystem.Debug("DelegateRunPrinterMps147. 3");
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool DelegateRunPrinterMps92(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                List<string> tranCodeErrors = new List<string>();
                foreach (var transaction in ListTransaction)
                {
                    CommonParam param = new CommonParam();
                    List<V_HIS_EXP_MEST> expMests = null;
                    if (transaction.SALE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SALE_TYPE.ID__SALE_EXP)
                    {
                        if (!transaction.IS_CANCEL.HasValue || transaction.IS_CANCEL != 1)
                        {
                            HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                            expMestFilter.BILL_ID = transaction.ID;
                            expMests = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, expMestFilter, param);
                            if (expMests != null && expMests.Count > 0)
                            {
                                param = new CommonParam();
                                HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                                expMestMedicineFilter.EXP_MEST_IDs = expMests.Select(s => s.ID).ToList();
                                List<V_HIS_EXP_MEST_MEDICINE> listExpMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, expMestMedicineFilter, param);

                                param = new CommonParam();
                                HisExpMestMaterialViewFilter expMestMaterialFilter = new HisExpMestMaterialViewFilter();
                                expMestMaterialFilter.EXP_MEST_IDs = expMests.Select(s => s.ID).ToList();
                                List<V_HIS_EXP_MEST_MATERIAL> listExpMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, expMestMaterialFilter, param);


                                MPS.Processor.Mps000092.PDO.Mps000092PDO rdo = new MPS.Processor.Mps000092.PDO.Mps000092PDO(expMests, listExpMestMedicines, listExpMestMaterials, transaction);

                                PrintData(printCode, fileName, rdo, ref result);
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Debug("Giao dich khong co phieu xuat(exp_mest) tuong ung theo BILL_ID =" + transaction.ID + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => transaction), transaction));
                                tranCodeErrors.Add(transaction.TRANSACTION_CODE);
                            }
                        }
                        else
                        {
                            HisBillGoodsFilter billGoodFilter = new HisBillGoodsFilter();
                            billGoodFilter.BILL_ID = transaction.ID;
                            List<HIS_BILL_GOODS> billGoods = new BackendAdapter(param).Get<List<HIS_BILL_GOODS>>("api/HisBillGoods/Get", ApiConsumers.MosConsumer, billGoodFilter, param);
                            MPS.Processor.Mps000092.PDO.Mps000092PDO rdo = new MPS.Processor.Mps000092.PDO.Mps000092PDO(billGoods, transaction);

                            PrintData(printCode, fileName, rdo, ref result);
                        }
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Khong phai loai giao dich xuat ban BILL_ID =" + transaction.ID + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => transaction), transaction));
                        tranCodeErrors.Add(transaction.TRANSACTION_CODE);
                    }

                }
                if (tranCodeErrors != null && tranCodeErrors.Count > 0)
                {
                    string messgeErrors = String.Format("Dữ liệu chi tiết của các hóa đơn có mã: {0} đã bị hủy", String.Join(",", tranCodeErrors.ToArray()));
                    MessageManager.Show(messgeErrors);
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool DelegateRunPrinterMps148(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                if (ListMps148 != null && ListMps148.Count > 0)
                {
                    CommonParam param = new CommonParam();

                    List<HIS_SERE_SERV_BILL> listSSBill = new List<HIS_SERE_SERV_BILL>();

                    var skip = 0;

                    while (ListMps148.Count - skip > 0)
                    {
                        var lstdata = ListMps148.Skip(skip).Take(100).ToList();
                        skip += 100;
                        HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                        ssBillFilter.BILL_IDs = lstdata.Select(s => s.ID).ToList();

                        var lstSSBill = new BackendAdapter(param).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, param);
                        if (lstSSBill != null && lstSSBill.Count > 0) listSSBill.AddRange(lstSSBill);
                    }

                    List<HIS_SERE_SERV> listSereServ = new List<HIS_SERE_SERV>();
                    if (listSSBill != null && listSSBill.Count > 0)
                    {
                        var skip2 = 0;
                        while (listSSBill.Count - skip2 > 0)
                        {
                            var lstData = listSSBill.Skip(skip2).Take(100).ToList();
                            skip2 += 100;
                            HisSereServFilter filter = new HisSereServFilter();
                            filter.IS_INCLUDE_DELETED = true;
                            filter.IDs = lstData.Select(s => s.SERE_SERV_ID).ToList();
                            var lstss = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, filter, param);
                            if (lstss != null && lstss.Count > 0) listSereServ.AddRange(lstss);
                        }
                    }

                    foreach (var transaction in ListMps148)
                    {
                        var listSereServBill = listSSBill.Where(o => o.BILL_ID == transaction.ID).ToList();
                        if (listSereServBill == null || listSereServBill.Count <= 0) continue;
                        List<HIS_SERE_SERV> listSS = null;
                        if (transaction.IS_CANCEL == 1)
                        {
                            listSS = listSereServ.Where(o => listSereServBill.Select(s => s.SERE_SERV_ID).Contains(o.ID)).ToList();
                        }
                        else
                        {
                            listSS = listSereServ.Where(o => o.IS_DELETE != 1 && o.SERVICE_REQ_ID.HasValue && listSereServBill.Select(s => s.SERE_SERV_ID).Contains(o.ID)).ToList();
                        }
                        if (listSS == null || listSS.Count <= 0) continue;

                        MPS.Processor.Mps000148.PDO.Mps000148PDO rdo = new MPS.Processor.Mps000148.PDO.Mps000148PDO(transaction, listSereServBill, listSS, Config.HisConfigCFG.PatientTypeId__BHYT);

                        PrintData(printCode, fileName, rdo, ref result);
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void PrintData(string printTypeCode, string fileName, object data, ref bool result)
        {
            try
            {
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                if (PrintNow)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                }
                else if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThreadPrintNow(string printTypeCode)
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(RichEditPrint));
            //thread.Priority = ThreadPriority.Highest;
            try
            {
                thread.Start(printTypeCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                thread.Abort();
            }
        }

        private void RichEditPrint(object printTypeCode)
        {
            try
            {
                RichEditPrintByCode(printTypeCode.ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RichEditPrintByCode(string printTypeCode)
        {
            try
            {
                richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);

                switch (printTypeCode)
                {
                    case PrintTypeCode__Mps000092:
                        richEditorMain.RunPrintTemplate(printTypeCode, DelegateRunPrinter);
                        break;
                    case PrintTypeCode__Mps000147:
                    case PrintTypeCode__Mps000148:
                        richEditorMain.RunPrintTemplate(PrintTypeCode__Mps000147, DelegateRunPrinter);
                        richEditorMain.RunPrintTemplate(PrintTypeCode__Mps000148, DelegateRunPrinter);
                        break;
                    default:
                        MessageBox.Show("Dữ liệu mã biểu in không hợp lệ");
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

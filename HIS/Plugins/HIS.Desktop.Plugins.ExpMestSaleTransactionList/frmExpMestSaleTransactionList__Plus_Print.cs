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
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExpMestSaleTransactionList.Config;
using HIS.Desktop.Plugins.Library.ElectronicBill;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Print;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.DocumentViewer;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestSaleTransactionList
{
    public partial class frmExpMestSaleTransactionList : HIS.Desktop.Utility.FormBase
    {
        private string Print106Type = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Print.TransactionDetail");
        private string Print106Type_Expend = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Print.TransactionDetail_Expend");

        private void MouseRightItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem) && this.transactionPrint != null)
                {
                    var type = (PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case PopupMenuProcessor.ItemType.PhieuThuThanhToan:
                            this.PrintPhieuThuThanhToan();
                            break;
                        case PopupMenuProcessor.ItemType.PhieuTamUng:
                            this.PrintPhieuTamUng();
                            break;
                        case PopupMenuProcessor.ItemType.PhieuTamUngDichVuChiTiet:
                            this.PrintPhieuChiTietTamUngDichVu();
                            break;
                        case PopupMenuProcessor.ItemType.PhieuHoanUng:
                            this.PrintPhieuHoanUng();
                            break;
                        case PopupMenuProcessor.ItemType.HoaDonTTTheoYeuCauDichVu:
                            this.PrintHoaDonThanhToanTheoYeuCauDichVu();
                            break;
                        case PopupMenuProcessor.ItemType.HoaDonTTChiTietDichVu:
                            this.PrintHoaDonThanhToanChiTietDichVu();
                            break;
                        case PopupMenuProcessor.ItemType.PhieuChiDinh:
                            this.PrintPhieuChiDinh();
                            break;
                        case PopupMenuProcessor.ItemType.BienLaiPhiLePhi:
                            this.PrintBienLaiThuPhiLePhi();
                            break;
                        case PopupMenuProcessor.ItemType.PhieuThuPhiDichVu:
                            this.PrintPhieuThuPhiDichVu();
                            break;
                        case PopupMenuProcessor.ItemType.PhieuHoanUngDichVu:
                            this.PrintPhieuHoanUngDichVu();
                            break;
                        case PopupMenuProcessor.ItemType.PhieuTamUngGiuThe:
                            this.PrintPhieuTamUngVaGiuThe();
                            break;
                        case PopupMenuProcessor.ItemType.PhieuTamUngThongTinBhyt:
                            this.PrintPhieuTamUngCoThongTinBHYT();
                            break;
                        case PopupMenuProcessor.ItemType.HoaDonDienTu:
                            this.InHoaDonDienTu();
                            break;
                        case PopupMenuProcessor.ItemType.SuaSoBienLai:
                            this.SuaSoBienLai();
                            break;
                        case PopupMenuProcessor.ItemType.InBienLaiHuy:
                            this.PrintBienLaiHuyHoaDon();
                            break;
                        case PopupMenuProcessor.ItemType.InHoaDonXuatBan:
                            this.PrintHoaDonXuatBan();
                            break;
                        case PopupMenuProcessor.ItemType.KhoiPhucBienLai:
                            this.KhoiPhucBienLai();
                            break;
                        case PopupMenuProcessor.ItemType.HuyBienLai:
                            this.HuyBienLai();
                            break;
                        case PopupMenuProcessor.ItemType.HuyPhieuXuat:
                            this.HuyPhieuXuat();
                            break;
                        case PopupMenuProcessor.ItemType.TransactionEInvoice:
                            this.TransactionEInvoice();
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TransactionEInvoice()
        {
            try
            {
                if (this.transactionPrint == null) return;
                //không phải giao dịch thanh toán hoặc đã tạo hóa đơn điện tử thì bỏ qua.
                if (transactionPrint.TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT || !String.IsNullOrWhiteSpace(transactionPrint.INVOICE_CODE))
                {
                    return;
                }

                V_HIS_TRANSACTION tran = null;
                HisTransactionViewFilter filter = new HisTransactionViewFilter();
                filter.TRANSACTION_CODE__EXACT = transactionPrint.TRANSACTION_CODE;
                var listTran = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, filter, null);
                if (listTran != null && listTran.Count > 0)
                {
                    tran = listTran.FirstOrDefault();
                }

                List<object> listArgs = new List<object>();
                listArgs.Add(tran);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.TransactionEInvoice", currentModule.RoomId, currentModule.RoomTypeId, listArgs);
                LoadDataGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void KhoiPhucBienLai()
        {
            try
            {
                var transaction = GetTransaction(this.transactionPrint.TRANSACTION_CODE);
                if (transaction == null) return;

                bool success = false;
                CommonParam param = new CommonParam();

                HisTransactionUncancelSDO sdo = new HisTransactionUncancelSDO();
                sdo.TransactionId = transaction.ID;

                var room = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().FirstOrDefault(p => p.ID == this.transactionPrint.CASHIER_ROOM_ID);
                long roomId = room != null ? room.ROOM_ID : this.currentModule.RoomId;
                sdo.RequestRoomId = roomId;

                var rs = new BackendAdapter(param).Post<HIS_TRANSACTION>("api/HisTransaction/Uncancel", ApiConsumers.MosConsumer, sdo, param);
                if (rs != null)
                {
                    success = true;
                    this.LoadDataGrid();
                }
                MessageManager.Show(this.ParentForm, param, success);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void HuyBienLai()
        {
            try
            {
                if (!string.IsNullOrEmpty(this.transactionPrint.TRANSACTION_CODE))
                {
                    HuyBienLaiHoaDon(this.transactionPrint.TRANSACTION_CODE);
                }
                else
                {
                    DeleteExpMest(this.transactionPrint);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void HuyPhieuXuat()
        {
            try
            {
                DeleteExpMest(this.transactionPrint);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private V_HIS_TRANSACTION GetTransaction(string transactionCode)
        {
            V_HIS_TRANSACTION result = null;
            try
            {
                if (!string.IsNullOrEmpty(transactionCode))
                {
                    MOS.Filter.HisTransactionViewFilter filter = new HisTransactionViewFilter();
                    filter.TRANSACTION_CODE__EXACT = transactionCode.Trim();
                    result = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, filter, null).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void PrintHoaDonXuatBan()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

                store.RunPrintTemplate("Mps000339", this.DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintBienLaiHuyHoaDon()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

                store.RunPrintTemplate("Mps000337", this.DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SuaSoBienLai()
        {
            try
            {
                var transaction = GetTransaction(this.transactionPrint.TRANSACTION_CODE);
                if (transaction == null) return;
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionNumOrderUpdate").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TransactionNumOrderUpdate'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(transaction);
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("extenceInstance is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                    this.LoadDataGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InHoaDonDienTu()
        {
            try
            {
                var transaction = GetTransaction(this.transactionPrint.TRANSACTION_CODE);
                if (transaction == null) return;

                ElectronicBillDataInput dataInput = new ElectronicBillDataInput();
                dataInput.PartnerInvoiceID = Inventec.Common.TypeConvert.Parse.ToInt64(transaction.INVOICE_CODE);

                dataInput.InvoiceCode = transaction.INVOICE_CODE;
                dataInput.NumOrder = transaction.NUM_ORDER;
                dataInput.SymbolCode = transaction.SYMBOL_CODE;
                dataInput.TemplateCode = transaction.TEMPLATE_CODE;
                dataInput.TransactionTime = transaction.TRANSACTION_TIME;
                dataInput.Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                ElectronicBillProcessor electronicBillProcessor = new ElectronicBillProcessor(dataInput);
                ElectronicBillResult electronicBillResult = electronicBillProcessor.Run(ElectronicBillType.ENUM.GET_INVOICE_LINK);

                if (electronicBillResult == null || String.IsNullOrEmpty(electronicBillResult.InvoiceLink))
                {
                    MessageBox.Show("Không tìm thấy link hóa đơn điện tử");
                    return;
                }

                Inventec.Common.DocumentViewer.DocumentViewerManager viewManager = new Inventec.Common.DocumentViewer.DocumentViewerManager(ViewType.ENUM.Pdf);
                viewManager.Run(electronicBillResult.InvoiceLink);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void PrintPhieuThuThanhToan()
        {
            try
            {
                var transaction = GetTransaction(this.transactionPrint.TRANSACTION_CODE);
                if (transaction == null) return;

                Inventec.Common.RichEditor.RichEditorStore richStore = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                if (transaction != null && transaction.SALE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SALE_TYPE.ID__SALE_OTHER)
                {
                    richStore.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuThanhToanKhac_MPS000299, this.DelegateRunPrinter);
                }
                else if (HisConfigCFG.TransactionBillSelect == "2")
                {
                    if (HisConfigCFG.BILL_TWO_BOOK__OPTION == (int)HisConfigCFG.BILL_OPTION.HCM_115
                        || HisConfigCFG.BILL_TWO_BOOK__OPTION == (int)HisConfigCFG.BILL_OPTION.QBH_CUBA)
                    {
                        if (transaction != null && transaction.BILL_TYPE_ID == 2)
                        {
                            richStore.RunPrintTemplate("Mps000318", this.DelegateRunPrinter);
                        }
                        else
                        {
                            richStore.RunPrintTemplate("Mps000317", this.DelegateRunPrinter);
                        }
                    }
                    else
                    {
                        if (transaction != null && transaction.BILL_TYPE_ID == 2)
                        {
                            richStore.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuHoaDonThanhToan_MPS000147, this.DelegateRunPrinter);
                        }
                        else
                        {
                            richStore.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuBienLaiThanhToan_MPS000148, this.DelegateRunPrinter);
                        }
                    }
                }
                else
                {
                    richStore.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuThanhToan_MPS000111, this.DelegateRunPrinter);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintHoaDonThanhToanTheoYeuCauDichVu()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richStore = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                richStore.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__HoaDonTTTheoYeuCauDichVu_MPS000103, this.DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void PrintHoaDonThanhToanChiTietDichVu()
        {
            try
            {
                var transaction = GetTransaction(this.transactionPrint.TRANSACTION_CODE);
                if (transaction == null) return;

                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

                var patientTypeAlter = new HIS_PATIENT_TYPE_ALTER();
                if (transaction != null)
                {
                    var paramCommon = new CommonParam();

                    patientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetLastByTreatmentId", ApiConsumers.MosConsumer, transaction.TREATMENT_ID, paramCommon);
                }

                if (patientTypeAlter != null && patientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                {
                    store.RunPrintTemplate(MPS.Processor.Mps000259.PDO.Mps000259PDO.printTypeCode, this.DelegateRunPrinter);
                }
                else
                {
                    store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__HoaDonThanhToanChiTietDichVu_Mps000106, this.DelegateRunPrinter);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintBienLaiThuPhiLePhi()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richStore = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                richStore.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BienLaiThuPhiLePhi_MPS000114, this.DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintPhieuChiDinh()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richStore = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                richStore.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuChiDinhDuaVaoGiaoDichThanhToan_Mps000105, this.DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintPhieuThuPhiDichVu()
        {

        }

        private void PrintPhieuTamUng()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richStore = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                richStore.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuTamUng_MPS000112, this.DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintPhieuTamUngDichVu()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richStore = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                richStore.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000109, this.DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintPhieuChiTietTamUngDichVu()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richStore = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                richStore.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000102, this.DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintPhieuHoanUng()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richStore = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                richStore.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuHoanUng_MPS000113, this.DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintPhieuHoanUngDichVu()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richStore = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                richStore.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000110, this.DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintPhieuTamUngVaGiuThe()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richStore = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                richStore.RunPrintTemplate(MPS.Processor.Mps000171.PDO.PrintTypeCode.Mps000171, this.DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintPhieuTamUngCoThongTinBHYT()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richStore = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                richStore.RunPrintTemplate(MPS.Processor.Mps000172.PDO.PrintTypeCode.Mps000172, this.DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuThanhToan_MPS000111:
                        InPhieuThuThanhToan(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuHoaDonThanhToan_MPS000147:
                        InHoaDonThanhToanHaiSo(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuBienLaiThanhToan_MPS000148:
                        InBienLaiThanhToanHaiSo(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__HoaDonThanhToanChiTietDichVu_Mps000106:
                        InPhieuThuTTChiTietDichVu(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuTamUng_MPS000112:
                        InPhieuThuTamUng(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000109:
                        InPhieuThuTamUngDichVu(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000102:
                        InPhieuChiTietTamUngDichVu(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuHoanUng_MPS000113:
                        InPhieuThuHoanUng(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__HoaDonTTTheoYeuCauDichVu_MPS000103:
                        InHoaDonTTTheoYeuCauDichVu(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BienLaiThuPhiLePhi_MPS000114:
                        InBienLaiThuPhiLePhi(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuChiDinhDuaVaoGiaoDichThanhToan_Mps000105:
                        InPhieuChiDinhTheoGiaoDichThanhToan(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000110:
                        InPhieuHoanUngDichVu(printTypeCode, fileName, ref result);
                        break;
                    case MPS.Processor.Mps000171.PDO.PrintTypeCode.Mps000171:
                        InPhieuTamUngVaGiuThe(printTypeCode, fileName, ref result);
                        break;
                    case MPS.Processor.Mps000172.PDO.PrintTypeCode.Mps000172:
                        InPhieuTamUngCoThongTinBHYT(printTypeCode, fileName, ref result);
                        break;
                    case MPS.Processor.Mps000259.PDO.Mps000259PDO.printTypeCode:
                        InPhieuThuTTChiTietDichVuNgoaiTru(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuThanhToanKhac_MPS000299:
                        InPhieuThuThanhToanKhac(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000317":
                        InBienLaiThanhToanHaiSoHCM115(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000318":
                        InHoaDonThanhToanHaiSoHCM115(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000337":
                        InHoaDonHuyThanhToan(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000339":
                        InHoaDonXuatBan(printTypeCode, fileName, ref result);
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

        private void InPhieuThuTTChiTietDichVuNgoaiTru(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                var transaction = GetTransaction(this.transactionPrint.TRANSACTION_CODE);
                if (transaction == null) return;

                WaitingManager.Show();
                var listSereServ = new List<V_HIS_SERE_SERV>();

                HisSereServBillViewFilter ssBillFilter = new HisSereServBillViewFilter();
                ssBillFilter.BILL_ID = transaction.ID;
                var hisSSBills = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_BILL>>("api/HisSereServBill/GetView", ApiConsumers.MosConsumer, ssBillFilter, null);
                if (hisSSBills == null || hisSSBills.Count <= 0)
                {
                    throw new Exception("Khong lay duoc SereServBill theo BillId: " + transaction.ID);
                }

                if (Print106Type != "1")
                {
                    HisSereServViewFilter ssFilter = new HisSereServViewFilter();
                    ssFilter.IDs = hisSSBills.Select(s => s.SERE_SERV_ID).ToList();
                    ssFilter.TREATMENT_ID = transaction.TREATMENT_ID;
                    listSereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, ssFilter, null);

                    if (Print106Type_Expend == "1")
                    {
                        if (listSereServ != null && listSereServ.Count > 0)
                        {
                            HisSereServViewFilter ssFilter1 = new HisSereServViewFilter();
                            ssFilter1.TREATMENT_ID = transaction.TREATMENT_ID;
                            ssFilter1.IS_EXPEND = true;
                            var listSereServChild = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, ssFilter1, null);
                            if (listSereServChild != null && listSereServChild.Count > 0)
                            {
                                listSereServChild = listSereServChild.Where(o => !o.PARENT_ID.HasValue || (listSereServ.Select(s => s.ID).Contains(o.PARENT_ID.Value))).ToList();
                                if (listSereServChild != null && listSereServChild.Count > 0)
                                {
                                    listSereServ.AddRange(listSereServChild);
                                }
                            }
                        }
                    }
                }
                else
                {
                    HisSereServViewFilter ssFilter = new HisSereServViewFilter();
                    ssFilter.TREATMENT_ID = transaction.TREATMENT_ID;
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
                ptAlterAppFilter.TreatmentId = transaction.TREATMENT_ID ?? 0;
                ptAlterAppFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, ptAlterAppFilter, null);

                // tính mức hưởng của thẻ
                string levelCode = HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT;
                string ratio_text = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentPatientTypeAlter.HEIN_CARD_NUMBER, currentPatientTypeAlter.LEVEL_CODE, currentPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100) + "";

                if (listSereServ != null && listSereServ.Count > 0)
                {
                    decimal totalDeposit = GetDepositAmount(transaction.TREATMENT_ID);
                    HIS_TREATMENT treatment = GetTreatment(transaction.TREATMENT_ID);

                    HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                    patientFilter.ID = treatment.PATIENT_ID;
                    var patients = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, null);
                    V_HIS_PATIENT patient = new V_HIS_PATIENT();
                    if (patients != null && patients.Count > 0)
                    {
                        patient = patients.FirstOrDefault();
                    }
                    MPS.Processor.Mps000259.PDO.Mps000259ADO ado = new MPS.Processor.Mps000259.PDO.Mps000259ADO();
                    ado.PatientTypeBHYT = HisConfigCFG.PatientTypeId__BHYT;
                    ado.PatientTypeVP = HisConfigCFG.PatientTypeId__VP;

                    MPS.Processor.Mps000259.PDO.Mps000259PDO rdo = new MPS.Processor.Mps000259.PDO.Mps000259PDO(
                        transaction,
                        listSereServ,
                        hisSSBills,
                        treatment,
                        totalDeposit,
                        0,
                        currentPatientTypeAlter,
                        patient,
                        ratio_text,
                        ado,
                        BackendDataWorker.Get<HIS_DEPARTMENT>()
                        );
                    rdo.ShowExpend = Print106Type_Expend == "1";
                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, null));
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuThuTTChiTietDichVu(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                var transaction = GetTransaction(this.transactionPrint.TRANSACTION_CODE);
                if (transaction == null) return;

                if (transaction == null)
                    return;
                WaitingManager.Show();
                var listSereServ = new List<V_HIS_SERE_SERV>();
                HisSereServBillViewFilter ssBillFilter = new HisSereServBillViewFilter();
                ssBillFilter.BILL_ID = transaction.ID;
                var hisSSBills = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_BILL>>("api/HisSereServBill/GetView", ApiConsumers.MosConsumer, ssBillFilter, null);
                if (hisSSBills == null || hisSSBills.Count <= 0)
                {
                    throw new Exception("Khong lay duoc SereServBill theo BillId: " + transaction.ID);
                }

                if (Print106Type != "1")
                {
                    HisSereServViewFilter ssFilter = new HisSereServViewFilter();
                    ssFilter.IDs = hisSSBills.Select(s => s.SERE_SERV_ID).ToList();
                    ssFilter.TREATMENT_ID = transaction.TREATMENT_ID;
                    listSereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, ssFilter, null);

                    if (Print106Type_Expend == "1")
                    {
                        if (listSereServ != null && listSereServ.Count > 0)
                        {
                            HisSereServViewFilter ssFilter1 = new HisSereServViewFilter();
                            ssFilter1.TREATMENT_ID = transaction.TREATMENT_ID;
                            ssFilter1.IS_EXPEND = true;
                            var listSereServChild = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, ssFilter1, null);
                            if (listSereServChild != null && listSereServChild.Count > 0)
                            {
                                listSereServChild = listSereServChild.Where(o => !o.PARENT_ID.HasValue || (listSereServ.Select(s => s.ID).Contains(o.PARENT_ID.Value))).ToList();
                                if (listSereServChild != null && listSereServChild.Count > 0)
                                {
                                    listSereServ.AddRange(listSereServChild);
                                }
                            }
                        }
                    }
                }
                else
                {
                    HisSereServViewFilter ssFilter = new HisSereServViewFilter();
                    ssFilter.TREATMENT_ID = transaction.TREATMENT_ID;
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

                if (listSereServ != null && listSereServ.Count > 0)
                {
                    HisPatientTypeAlterViewAppliedFilter ptAlterAppFilter = new HisPatientTypeAlterViewAppliedFilter();
                    ptAlterAppFilter.TreatmentId = transaction.TREATMENT_ID ?? 0;
                    ptAlterAppFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                    var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, ptAlterAppFilter, null);

                    // tính mức hưởng của thẻ
                    string levelCode = HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT;
                    string ratio_text = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentPatientTypeAlter.HEIN_CARD_NUMBER, currentPatientTypeAlter.LEVEL_CODE, currentPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100) + "";
                    V_HIS_PATIENT patient = new V_HIS_PATIENT();
                    if (transaction.TDL_PATIENT_ID.HasValue)
                    {
                        HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                        patientFilter.ID = transaction.TDL_PATIENT_ID;
                        var patients = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, null);

                        if (patients != null && patients.Count > 0)
                        {
                            patient = patients.FirstOrDefault();
                        }
                    }

                    decimal totalDeposit = GetDepositAmount(transaction.TREATMENT_ID);
                    HIS_TREATMENT treatment = GetTreatment(transaction.TREATMENT_ID);
                    MPS.Processor.Mps000106.PDO.Mps000106ADO ado = new MPS.Processor.Mps000106.PDO.Mps000106ADO();
                    ado.PatientTypeBHYT = HisConfigCFG.PatientTypeId__BHYT;
                    ado.PatientTypeVP = HisConfigCFG.PatientTypeId__VP;

                    MPS.Processor.Mps000106.PDO.Mps000106PDO rdo = new MPS.Processor.Mps000106.PDO.Mps000106PDO(transaction, listSereServ, hisSSBills, treatment, totalDeposit, 0, currentPatientTypeAlter, patient, ratio_text, ado, BackendDataWorker.Get<HIS_DEPARTMENT>());
                    rdo.ShowExpend = Print106Type_Expend == "1";
                    WaitingManager.Hide();

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.transactionPrint != null ? this.transactionPrint.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode) && !String.IsNullOrEmpty(GlobalVariables.dicPrinter[printTypeCode]))
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuThuThanhToan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                var transaction = GetTransaction(this.transactionPrint.TRANSACTION_CODE);
                if (transaction == null) return;

                if (!transaction.TREATMENT_ID.HasValue)
                {
                    MessageManager.Show("Hóa đơn thanh toán xuất bán thuốc vật tư");
                    return;
                }
                if (transaction.IS_CANCEL == 1)
                {
                    MessageManager.Show("Giao dịch đã bị hủy");
                    return;
                }
                WaitingManager.Show();

                HisBillFundFilter billFundFilter = new HisBillFundFilter();
                billFundFilter.BILL_ID = transaction.ID;
                var listBillFund = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_BILL_FUND>>("api/HisBillFund/Get", ApiConsumers.MosConsumer, billFundFilter, null);

                HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                ssBillFilter.BILL_ID = transaction.ID;
                var hisSSBills = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, null);
                if (hisSSBills == null || hisSSBills.Count <= 0)
                {
                    throw new Exception("Khong lay duoc SSBill theo billId: " + transaction.BILL_TYPE_ID);
                }

                HisSereServFilter ssFilter = new HisSereServFilter();
                ssFilter.IDs = hisSSBills.Select(s => s.SERE_SERV_ID).ToList();
                var listSereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, null);

                HisPatientTypeAlterViewAppliedFilter patyAlterAppliedFilter = new HisPatientTypeAlterViewAppliedFilter();
                patyAlterAppliedFilter.InstructionTime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                patyAlterAppliedFilter.TreatmentId = transaction.TREATMENT_ID.Value;
                var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, patyAlterAppliedFilter, null);
                if (currentPatientTypeAlter == null)
                {
                    Inventec.Common.Logging.LogSystem.Info("Khong lay duoc PatientTypeAlterApplied: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.transactionPrint.TREATMENT_CODE), this.transactionPrint.TREATMENT_CODE));
                }

                HisDepartmentTranLastFilter departLastFilter = new HisDepartmentTranLastFilter();
                departLastFilter.TREATMENT_ID = transaction.TREATMENT_ID.Value;
                departLastFilter.BEFORE_LOG_TIME = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                var departmentTran = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, departLastFilter, null);

                HIS_PATIENT patient = new HIS_PATIENT();
                if (transaction.TDL_PATIENT_ID != null)
                {
                    HisPatientFilter patientFilter = new HisPatientFilter();
                    patientFilter.ID = transaction.TDL_PATIENT_ID;
                    var patients = new BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, null);

                    if (patients != null && patients.Count > 0)
                    {
                        patient = patients.FirstOrDefault();
                    }
                }

                WaitingManager.Hide();
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((transactionPrint != null ? transactionPrint.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                MPS.Processor.Mps000111.PDO.Mps000111PDO pdo = new MPS.Processor.Mps000111.PDO.Mps000111PDO(transaction, patient, listBillFund, listSereServ, departmentTran, currentPatientTypeAlter, GetId(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT")));

                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void InBienLaiThanhToanHaiSo(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                var transaction = GetTransaction(this.transactionPrint.TRANSACTION_CODE);
                if (transaction == null) return;

                if (!transaction.TREATMENT_ID.HasValue)
                {
                    MessageManager.Show("Hóa đơn thanh toán xuất bán thuốc vật tư");
                    return;
                }
                if (transaction.IS_CANCEL == 1)
                {
                    MessageManager.Show("Giao dịch đã bị hủy");
                    return;
                }

                HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                ssBillFilter.BILL_ID = transaction.ID;
                var listSSBill = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, null);
                if (listSSBill == null || listSSBill.Count <= 0)
                {
                    throw new Exception("Khong lay duoc SSBill theo billId: " + transaction.ID);
                }
                HisSereServFilter filter = new HisSereServFilter();
                filter.IDs = listSSBill.Select(s => s.SERE_SERV_ID).ToList();
                var listSereServ = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, filter, null);

                if (listSereServ == null || listSereServ.Count == 0)
                {
                    throw new NullReferenceException("Khong lay duoc SereServ theo resultRecieptBill.ID" + LogUtil.TraceData("transactionPrint", transaction));
                }

                MPS.Processor.Mps000148.PDO.Mps000148PDO rdo = new MPS.Processor.Mps000148.PDO.Mps000148PDO(transaction, listSSBill, listSereServ, HisConfigCFG.PatientTypeId__BHYT);

                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, null) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, null) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void InHoaDonThanhToanHaiSo(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                var transaction = GetTransaction(this.transactionPrint.TRANSACTION_CODE);
                if (transaction == null) return;

                if (!transaction.TREATMENT_ID.HasValue)
                {
                    MessageManager.Show("Hóa đơn thanh toán xuất bán thuốc vật tư");
                    return;
                }
                if (transaction.IS_CANCEL == 1)
                {
                    MessageManager.Show("Giao dịch đã bị hủy");
                    return;
                }

                MPS.Processor.Mps000147.PDO.Mps000147PDO rdo = new MPS.Processor.Mps000147.PDO.Mps000147PDO(transaction);
                WaitingManager.Hide();

                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, null) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                }
                else
                {
                    result = result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, null) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void InHoaDonThanhToanHaiSoHCM115(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                var transaction = GetTransaction(this.transactionPrint.TRANSACTION_CODE);
                if (transaction == null) return;

                if (!transaction.TREATMENT_ID.HasValue)
                {
                    MessageManager.Show("Hóa đơn thanh toán xuất bán thuốc vật tư");
                    return;
                }
                if (transaction.IS_CANCEL == 1)
                {
                    MessageManager.Show("Giao dịch đã bị hủy");
                    return;
                }

                MPS.Processor.Mps000318.PDO.Mps000318PDO rdo = new MPS.Processor.Mps000318.PDO.Mps000318PDO(transaction);
                WaitingManager.Hide();

                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, null) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                }
                else
                {
                    result = result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, null) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void InBienLaiThanhToanHaiSoHCM115(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                var transaction = GetTransaction(this.transactionPrint.TRANSACTION_CODE);
                if (transaction == null) return;

                if (!transaction.TREATMENT_ID.HasValue)
                {
                    MessageManager.Show("Hóa đơn thanh toán xuất bán thuốc vật tư");
                    return;
                }
                if (transaction.IS_CANCEL == 1)
                {
                    MessageManager.Show("Giao dịch đã bị hủy");
                    return;
                }
                MPS.Processor.Mps000317.PDO.Mps000317PDO rdo = new MPS.Processor.Mps000317.PDO.Mps000317PDO(transaction);
                WaitingManager.Hide();

                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, null) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                }
                else
                {
                    result = result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, null) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void InPhieuThuThanhToanKhac(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                var transaction = GetTransaction(this.transactionPrint.TRANSACTION_CODE);
                if (transaction == null) return;

                if (!transaction.TREATMENT_ID.HasValue)
                {
                    MessageManager.Show("Hóa đơn thanh toán xuất bán thuốc vật tư");
                    return;
                }
                if (transaction.IS_CANCEL == 1)
                {
                    MessageManager.Show("Giao dịch đã bị hủy");
                    return;
                }
                WaitingManager.Show();
                HisBillGoodsFilter billGoodFilter = new HisBillGoodsFilter();
                billGoodFilter.BILL_ID = transaction.ID;
                var listBillGoods = new BackendAdapter(new CommonParam()).Get<List<HIS_BILL_GOODS>>("api/HisBillGoods/Get", ApiConsumers.MosConsumer, billGoodFilter, null);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((transactionPrint != null ? transactionPrint.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                MPS.Processor.Mps000299.PDO.Mps000299PDO pdo = new MPS.Processor.Mps000299.PDO.Mps000299PDO(
                    transaction,
                    listBillGoods
                    );
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    MPS.ProcessorBase.Core.PrintData printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    MPS.ProcessorBase.Core.PrintData printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private static long GetId(string code)
        {
            long result = 0;
            //try
            //{
            //    var data = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == code);
            //    if (!(data != null && data.ID > 0)) throw new ArgumentNullException(code + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => code), code));
            //    result = data.ID;
            //}
            //catch (Exception ex)
            //{
            //    LogSystem.Debug(ex);
            //    result = 0;
            //}
            return result;
        }

        private void InPhieuThuTamUng(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                var transaction = GetTransaction(this.transactionPrint.TRANSACTION_CODE);
                if (transaction == null) return;

                if (!transaction.TREATMENT_ID.HasValue)
                {
                    MessageManager.Show("Hóa đơn thanh toán xuất bán thuốc vật tư");
                    return;
                }
                if (transaction.IS_CANCEL == 1)
                {
                    MessageManager.Show("Giao dịch đã bị hủy");
                    return;
                }
                WaitingManager.Show();
                V_HIS_PATIENT patient = null;

                HisTransactionViewFilter depositFilter = new HisTransactionViewFilter();
                depositFilter.ID = transaction.ID;
                var listDeposit = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, depositFilter, null);
                if (listDeposit == null || listDeposit.Count != 1)
                {
                    throw new Exception("Khong lay duoc V-HIS_DEPOSIT theo transactionId, TransactionCode:" + transaction.TRANSACTION_CODE);
                }

                var deposit = listDeposit.First();

                if (transaction.TDL_PATIENT_ID.HasValue)
                {
                    HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                    patientFilter.ID = transaction.TDL_PATIENT_ID;
                    var listPatient = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, patientFilter, null);
                    if (listPatient == null || listPatient.Count != 1)
                    {
                        throw new NullReferenceException("Get VHisPatient by TdlPatientId null or count != 1" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listPatient), listPatient));
                    }
                    patient = listPatient.First();
                }


                decimal ratio = 0;
                var PatyAlterBhyt = new V_HIS_PATIENT_TYPE_ALTER();
                PrintGlobalStore.LoadCurrentPatientTypeAlter(transaction.TREATMENT_ID.Value, 0, ref PatyAlterBhyt);
                if (PatyAlterBhyt != null && !String.IsNullOrEmpty(PatyAlterBhyt.HEIN_CARD_NUMBER))
                {
                    ratio = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(PatyAlterBhyt.HEIN_TREATMENT_TYPE_CODE, PatyAlterBhyt.HEIN_CARD_NUMBER, PatyAlterBhyt.LEVEL_CODE, PatyAlterBhyt.RIGHT_ROUTE_CODE) ?? 0;
                }

                HisDepartmentTranViewFilter departLastFilter = new HisDepartmentTranViewFilter();
                departLastFilter.TREATMENT_ID = transaction.TREATMENT_ID.Value;
                var departmentTrans = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/GetView", ApiConsumers.MosConsumer, departLastFilter, null);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((deposit != null ? deposit.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                MPS.Processor.Mps000112.PDO.Mps000112ADO ado = new MPS.Processor.Mps000112.PDO.Mps000112ADO();

                HisTransactionFilter depositCountFilter = new HisTransactionFilter();
                depositCountFilter.TREATMENT_ID = transaction.TREATMENT_ID;
                depositCountFilter.TRANSACTION_TIME_TO = transaction.TRANSACTION_TIME;
                depositCountFilter.TRANSACTION_TYPE_ID = transaction.TRANSACTION_TYPE_ID;
                var deposits = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_TRANSACTION>>("api/HisTransaction/Get", ApiConsumers.MosConsumer, depositCountFilter, null);
                if (deposits != null && deposits.Count > 0)
                {
                    ado.DEPOSIT_NUM_ORDER = deposits.Count;
                }

                MPS.Processor.Mps000112.PDO.Mps000112PDO rdo = new MPS.Processor.Mps000112.PDO.Mps000112PDO(deposit, patient, ratio, PatyAlterBhyt, departmentTrans, ado);
                MPS.ProcessorBase.Core.PrintData printData = null;
                WaitingManager.Hide();
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuThuTamUngDichVu(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                var transaction = GetTransaction(this.transactionPrint.TRANSACTION_CODE);
                if (transaction == null) return;

                if (!transaction.TREATMENT_ID.HasValue)
                {
                    MessageManager.Show("Hóa đơn thanh toán xuất bán thuốc vật tư");
                    return;
                }
                if (transaction.IS_CANCEL == 1)
                {
                    MessageManager.Show("Giao dịch đã bị hủy");
                    return;
                }
                WaitingManager.Show();
                HisTransactionView5Filter depositFilter = new HisTransactionView5Filter();
                depositFilter.ID = transaction.ID;
                var listDeposit = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION_5>>("api/HisTransaction/GetView5", ApiConsumers.MosConsumer, depositFilter, null);
                if (listDeposit == null || listDeposit.Count != 1)
                {
                    throw new Exception("Khong lay duoc V_HIS_DEPOSIT theo transactionId:" + transaction.ID);
                }

                MPS.Processor.Mps000109.PDO.PatientADO mpsPatientADO = new MPS.Processor.Mps000109.PDO.PatientADO();
                var mpsPatyAlterBhytADO = new V_HIS_PATIENT_TYPE_ALTER();
                PrintGlobalStore.LoadCurrentPatientTypeAlter(transaction.TREATMENT_ID.Value, 0, ref mpsPatyAlterBhytADO);

                HisTreatmentFeeViewFilter filterTreatmentFee = new HisTreatmentFeeViewFilter();
                filterTreatmentFee.ID = transaction.TREATMENT_ID.Value;

                var treatmentFee = new BackendAdapter(new CommonParam())
                  .Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, filterTreatmentFee, null).FirstOrDefault();

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((transaction != null ? transaction.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                MPS.Processor.Mps000109.PDO.Mps000109PDO rdo = new MPS.Processor.Mps000109.PDO.Mps000109PDO(
                    mpsPatientADO,
                    mpsPatyAlterBhytADO,
                    listDeposit.FirstOrDefault(),
                    treatmentFee
                    );

                WaitingManager.Hide();
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuChiTietTamUngDichVu(string printTypeCode, string fileName, ref bool result)
        {
            try
            {

                var transaction = GetTransaction(this.transactionPrint.TRANSACTION_CODE);
                if (transaction == null) return;

                if (!transaction.TREATMENT_ID.HasValue)
                {
                    MessageManager.Show("Hóa đơn thanh toán xuất bán thuốc vật tư");
                    return;
                }
                if (transaction.IS_CANCEL == 1)
                {
                    MessageManager.Show("Giao dịch đã bị hủy");
                    return;
                }
                WaitingManager.Show();
                MOS.Filter.HisSereServView12Filter sereServFilter = new MOS.Filter.HisSereServView12Filter();
                sereServFilter.TREATMENT_ID = transaction.TREATMENT_ID;

                MOS.Filter.HisSereServDepositFilter dereDetailFiter = new MOS.Filter.HisSereServDepositFilter();
                dereDetailFiter.DEPOSIT_ID = transaction.ID;
                dereDetailFiter.ORDER_DIRECTION = "TDL_SERVICE_NAME";
                dereDetailFiter.ORDER_FIELD = "ASC";
                var sereServDeposits = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumers.MosConsumer, dereDetailFiter, null);
                if (sereServDeposits == null || sereServDeposits.Count == 0)
                {
                    WaitingManager.Hide();
                    MessageBox.Show("Giao dịch tạm thu nội trú. Không có chi tiết dịch vụ");
                    return;
                }

                var sereServIds = sereServDeposits.Select(o => o.SERE_SERV_ID).ToList();
                sereServFilter.IDs = sereServIds;
                var sereServs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_12>>("api/HisSereServ/GetView12", ApiConsumers.MosConsumer, sereServFilter, null);

                foreach (var item in sereServs)
                {
                    var itemCheck = sereServDeposits.FirstOrDefault(o => o.SERE_SERV_ID == item.ID);
                    if (itemCheck != null)
                    {
                        item.AMOUNT = itemCheck.TDL_AMOUNT;
                    }
                }
                //Thông tin bệnh nhân
                V_HIS_PATIENT patient = new V_HIS_PATIENT();
                if (transaction.TDL_PATIENT_ID.HasValue)
                {
                    MOS.Filter.HisPatientViewFilter patientViewFilter = new HisPatientViewFilter();
                    patientViewFilter.ID = transaction.TDL_PATIENT_ID;
                    var patients = new BackendAdapter(null).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientViewFilter, null);
                    if (patients != null && patients.Count > 0)
                    {
                        patient = patients.FirstOrDefault();
                    }
                }

                //Thông tin bảo hiểm y tế
                long instructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                var patyAlterBhyt = new V_HIS_PATIENT_TYPE_ALTER();
                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientTypeAlterViewFilter patientTypeFilter = new HisPatientTypeAlterViewFilter();
                patientTypeFilter.TREATMENT_ID = transaction.TREATMENT_ID.Value;
                patientTypeFilter.PATIENT_TYPE_ID = sereServs.FirstOrDefault().PATIENT_TYPE_ID;
                var patientTypeAlters = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeALter/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientTypeFilter, param);
                if (patientTypeAlters != null && patientTypeAlters.Count > 0)
                {
                    patyAlterBhyt = patientTypeAlters.OrderByDescending(o => o.LOG_TIME).FirstOrDefault();
                }

                //
                HisDepartmentTranViewFilter filter = new HisDepartmentTranViewFilter();
                filter.TREATMENT_ID = transaction.TREATMENT_ID;
                List<V_HIS_DEPARTMENT_TRAN> departmentTrans = new BackendAdapter(new CommonParam()).Get<List<V_HIS_DEPARTMENT_TRAN>>("api/HisDepartMentTran/GetView", ApiConsumers.MosConsumer, filter, null);

                long? totalDay = null;

                HisTreatmentFeeViewFilter filterTreatmentFee = new HisTreatmentFeeViewFilter();
                filterTreatmentFee.ID = transaction.TREATMENT_ID.Value;
                var treatmentFee = new BackendAdapter(param)
                  .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, filterTreatmentFee, param).FirstOrDefault();

                if (treatmentFee != null && treatmentFee.TDL_PATIENT_TYPE_ID == Config.HisConfigCFG.PatientTypeId__BHYT)
                {
                    totalDay = HIS.Common.Treatment.Calculation.DayOfTreatment(treatmentFee.IN_TIME, treatmentFee.OUT_TIME, treatmentFee.TREATMENT_END_TYPE_ID, treatmentFee.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT);
                }
                else if (treatmentFee != null)
                {
                    totalDay = HIS.Common.Treatment.Calculation.DayOfTreatment(treatmentFee.IN_TIME, treatmentFee.OUT_TIME, treatmentFee.TREATMENT_END_TYPE_ID, treatmentFee.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI);
                }
                string departmentName = "";// = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetDepartmentNames();
                var SERVICE_REPORT_ID__HIGHTECH = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__DVKTC;

                var sereServHitechs = sereServs.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID == SERVICE_REPORT_ID__HIGHTECH).ToList();
                var sereServHitechADOs = PriceBHYTSereServAdoProcess(sereServHitechs);

                //các sereServ trong nhóm vật tư
                var SERVICE_REPORT__MATERIAL_VTTT_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT;
                var sereServVTTTs = sereServs.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID == SERVICE_REPORT__MATERIAL_VTTT_ID && o.IS_OUT_PARENT_FEE != null).ToList();
                var sereServVTTTADOs = PriceBHYTSereServAdoProcess(sereServVTTTs);

                var sereServNotHitechs = sereServs.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID != SERVICE_REPORT_ID__HIGHTECH).ToList();

                var servicePatyPrpos = BackendDataWorker.Get<V_HIS_SERVICE>();

                //Cộng các sereServ trong gói vào dv ktc
                foreach (var sereServHitech in sereServHitechADOs)
                {
                    List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO> sereServVTTTInKtcADOs = new List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();
                    var sereServVTTTInKtcs = sereServs.Where(o => o.PARENT_ID == sereServHitech.ID && o.IS_OUT_PARENT_FEE == null).ToList();
                    sereServVTTTInKtcADOs = PriceBHYTSereServAdoProcess(sereServVTTTInKtcs);
                    if (sereServHitech.PRICE_POLICY != null)
                    {
                        var servicePatyPrpo = servicePatyPrpos.Where(o => o.ID == sereServHitech.SERVICE_ID && o.BILL_PATIENT_TYPE_ID == sereServHitech.PATIENT_TYPE_ID && o.PACKAGE_PRICE == sereServHitech.PRICE_POLICY).ToList();
                        if (servicePatyPrpo != null && servicePatyPrpo.Count > 0)
                        {
                            sereServHitech.VIR_PRICE = sereServHitech.PRICE;
                        }
                    }
                    else
                        sereServHitech.VIR_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_PRICE);

                    sereServHitech.VIR_HEIN_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_HEIN_PRICE);
                    sereServHitech.VIR_PATIENT_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_HEIN_PRICE);

                    decimal totalHeinPrice = 0;
                    foreach (var sereServVTTTInKtcADO in sereServVTTTInKtcADOs)
                    {
                        totalHeinPrice += sereServVTTTInKtcADO.AMOUNT * sereServVTTTInKtcADO.PRICE_BHYT;
                    }
                    sereServHitech.PRICE_BHYT += totalHeinPrice;
                    sereServHitech.HEIN_LIMIT_PRICE += sereServVTTTInKtcADOs.Sum(o => o.HEIN_LIMIT_PRICE);

                    sereServHitech.VIR_TOTAL_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_PRICE);
                    sereServHitech.VIR_TOTAL_HEIN_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                    sereServHitech.VIR_TOTAL_PATIENT_PRICE = sereServHitech.VIR_TOTAL_PRICE - sereServHitech.VIR_TOTAL_HEIN_PRICE;
                    if (sereServVTTTInKtcs.Count > 0)
                    {
                        //sereServNotHitechs = sereServNotHitechs.Except(sereServVTTTInKtcs).ToList();
                    }
                    sereServHitech.SERVICE_UNIT_NAME = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(o => o.ID == sereServHitech.TDL_SERVICE_UNIT_ID).SERVICE_UNIT_NAME;
                }

                //Lọc các sereServ nằm không nằm trong dịch vụ ktc và vật tư thay thế
                //
                var sereServDeleteADOs = new List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();
                foreach (var sereServVTTTADO in sereServVTTTADOs)
                {
                    var sereServADODelete = sereServHitechADOs.Where(o => o.ID == sereServVTTTADO.PARENT_ID).ToList();
                    if (sereServADODelete.Count == 0)
                    {
                        sereServDeleteADOs.Add(sereServVTTTADO);
                    }
                }

                foreach (var sereServDelete in sereServDeleteADOs)
                {
                    sereServVTTTADOs.Remove(sereServDelete);
                }
                var sereServVTTTIds = sereServVTTTADOs.Select(o => o.ID);
                sereServNotHitechs = sereServNotHitechs.Where(o => !sereServVTTTIds.Contains(o.ID)).ToList();
                var sereServNotHitechADOs = PriceBHYTSereServAdoProcess(sereServNotHitechs);

                MOS.Filter.HisHeinServiceTypeFilter HeinServiceTypefilter = new MOS.Filter.HisHeinServiceTypeFilter();
                HeinServiceTypefilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                //HeinServiceTypefilter.ORDER_DIRECTION =""
                //    HeinServiceTypefilter.ORDER_FIELD = "ASC";
                var serviceReports = new BackendAdapter(new CommonParam()).Get<List<HIS_HEIN_SERVICE_TYPE>>(HisRequestUriStore.HIS_HEIN_SERVICE_TYPE_GET, ApiConsumers.MosConsumer, HeinServiceTypefilter, null);
                WaitingManager.Hide();

                V_HIS_PATIENT_TYPE_ALTER currentHisPatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                PrintGlobalStore.LoadCurrentPatientTypeAlter(transaction.TREATMENT_ID.Value, 0, ref currentHisPatientTypeAlter);
                //string levelCode = Inventec.Common.LocalStorage.SdaConfig.SdaConfigs.Get<string>(HIS.Desktop.LocalStorage.SdaConfigKey.ExtensionConfigKey.BHYT_HEIN_LEVEL_CODE);
                string ratio_text = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentHisPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, patyAlterBhyt.HEIN_CARD_NUMBER, patyAlterBhyt.LEVEL_CODE, patyAlterBhyt.RIGHT_ROUTE_CODE) ?? 0) * 100) + "";

                MPS.Processor.Mps000102.PDO.PatientADO patientAdo = new MPS.Processor.Mps000102.PDO.PatientADO(patient);

                if (sereServNotHitechADOs != null && sereServNotHitechADOs.Count > 0)
                {
                    sereServNotHitechADOs = sereServNotHitechADOs.OrderBy(o => o.TDL_SERVICE_NAME).ToList();
                }

                if (sereServHitechADOs != null && sereServHitechADOs.Count > 0)
                {
                    sereServHitechADOs = sereServHitechADOs.OrderBy(o => o.TDL_SERVICE_NAME).ToList();
                }

                if (sereServVTTTADOs != null && sereServVTTTADOs.Count > 0)
                {
                    sereServVTTTADOs = sereServVTTTADOs.OrderBy(o => o.TDL_SERVICE_NAME).ToList();
                }

                MPS.Processor.Mps000102.PDO.Mps000102PDO mps000102RDO = new MPS.Processor.Mps000102.PDO.Mps000102PDO(
                        patientAdo,
                        patyAlterBhyt,
                        departmentName,

                        sereServNotHitechADOs,
                        sereServHitechADOs,
                        sereServVTTTADOs,

                        departmentTrans,
                        treatmentFee,

                        serviceReports,
                        transaction,
                        sereServDeposits,
                        totalDay,
                        ratio_text,
                        FirstExamRoom(transaction.TREATMENT_ID.Value)
                        );
                WaitingManager.Hide();

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((transaction != null ? transaction.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode) && !String.IsNullOrEmpty(GlobalVariables.dicPrinter[printTypeCode]))
                {
                    if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000102RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000102RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                    }
                }
                else
                {
                    if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000102RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000102RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuThuHoanUng(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                var transaction = GetTransaction(this.transactionPrint.TRANSACTION_CODE);
                if (transaction == null) return;

                if (!transaction.TREATMENT_ID.HasValue)
                {
                    MessageManager.Show("Hóa đơn thanh toán xuất bán thuốc vật tư");
                    return;
                }
                if (transaction.IS_CANCEL == 1)
                {
                    MessageManager.Show("Giao dịch đã bị hủy");
                    return;
                }
                WaitingManager.Show();

                //BỎ repay
                //HisTransactionViewFilter repayFilter = new HisTransactionViewFilter();
                //repayFilter.ID = transaction.ID;
                //V_HIS_TRANSACTION repay = null;
                //var listRepay = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, repayFilter, null);
                //if (listRepay == null || listRepay.Count != 1)
                //{
                //    throw new Exception("Khong lay duoc V_HIS_REPAY theo transactionId: " + transaction.ID);
                //}
                //repay = listRepay.FirstOrDefault();
                V_HIS_PATIENT patient = null;

                if (transaction.TDL_PATIENT_ID.HasValue)
                {
                    HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                    patientFilter.ID = transaction.TDL_PATIENT_ID;
                    var listPatient = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, patientFilter, null);
                    if (listPatient == null || listPatient.Count != 1)
                    {
                        throw new NullReferenceException("Get VHisPatient by TdlPatientId null or count != 1" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listPatient), listPatient));
                    }
                    patient = listPatient.First();
                }

                decimal ratio = 0;
                var PatyAlterBhyt = new V_HIS_PATIENT_TYPE_ALTER();
                PrintGlobalStore.LoadCurrentPatientTypeAlter(transaction.TREATMENT_ID.Value, 0, ref PatyAlterBhyt);
                if (PatyAlterBhyt != null && !String.IsNullOrEmpty(PatyAlterBhyt.HEIN_CARD_NUMBER))
                {
                    ratio = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(PatyAlterBhyt.HEIN_TREATMENT_TYPE_CODE, PatyAlterBhyt.HEIN_CARD_NUMBER, PatyAlterBhyt.LEVEL_CODE, PatyAlterBhyt.RIGHT_ROUTE_CODE) ?? 0;
                }

                HisDepartmentTranLastFilter departLastFilter = new HisDepartmentTranLastFilter();
                departLastFilter.TREATMENT_ID = transaction.TREATMENT_ID.Value;
                departLastFilter.BEFORE_LOG_TIME = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                var departmentTran = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, departLastFilter, null);

                CommonParam paramtreatment = new CommonParam();
                HisTreatmentFeeViewFilter filterTreat = new HisTreatmentFeeViewFilter();
                filterTreat.ID = transaction.TREATMENT_ID;
                var TreatmentFee = new Inventec.Common.Adapter.BackendAdapter(paramtreatment).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, filterTreat, paramtreatment);

                HisTransactionViewFilter filterTran = new HisTransactionViewFilter();
                filterTran.TREATMENT_ID = transaction.TREATMENT_ID;
                filterTran.TRANSACTION_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT };
                filterTran.IS_CANCEL = false;
                List<V_HIS_TRANSACTION> transa = new BackendAdapter(paramtreatment).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, filterTran, paramtreatment);
                if (transa == null) transa = new List<V_HIS_TRANSACTION>();

                MPS.Processor.Mps000113.PDO.Mps000113PDO rdo = new MPS.Processor.Mps000113.PDO.Mps000113PDO(
                    transaction,
                    patient,
                    ratio,
                    null,
                    departmentTran,
                    TreatmentFee.First(),
                    transa
                    );
                MPS.ProcessorBase.Core.PrintData printData = null;
                WaitingManager.Hide();
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((transaction != null ? transaction.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InHoaDonTTTheoYeuCauDichVu(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                var transaction = GetTransaction(this.transactionPrint.TRANSACTION_CODE);
                if (transaction == null) return;
                //if (this.transactionPrint.IS_CANCEL == 1)
                //{
                //    MessageManager.Show(Base.ResourceMessageLang.GiaoDichDaBiHuy);
                //    return;
                //}
                WaitingManager.Show();
                //V_HIS_BILL bill = null;
                //HisBillViewFilter billFilter = new HisBillViewFilter();
                //billFilter.TRANSACTION_ID = this.transactionPrint.ID;
                //var listBill = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_BILL>>(HisRequestUriStore.HIS_BILL_GETVIEW, ApiConsumers.MosConsumer, billFilter, null);
                //if (listBill != null && listBill.Count == 1)
                //{
                //    bill = listBill.First();
                //}
                //else
                //{
                //    throw new NullReferenceException("Get VHisBill by TransactionId null or count != 1" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listBill), listBill));
                //}

                HisPatientTypeAlterViewAppliedFilter ptAlterAppFilter = new HisPatientTypeAlterViewAppliedFilter();
                ptAlterAppFilter.TreatmentId = transaction.TREATMENT_ID ?? 0;
                ptAlterAppFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, ptAlterAppFilter, null);

                // tính mức hưởng của thẻ
                string levelCode = HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT;
                string ratio_text = currentPatientTypeAlter != null ? ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentPatientTypeAlter.HEIN_CARD_NUMBER, currentPatientTypeAlter.LEVEL_CODE, currentPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100) + "" : "";
                V_HIS_PATIENT patient = new V_HIS_PATIENT();
                if (transaction.TDL_PATIENT_ID.HasValue)
                {
                    HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                    patientFilter.ID = transaction.TDL_PATIENT_ID;
                    var patients = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, null);

                    if (patients != null && patients.Count > 0)
                    {
                        patient = patients.FirstOrDefault();
                    }
                }

                HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                ssBillFilter.BILL_ID = transaction.ID;
                var hisSSBills = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, null);
                if (hisSSBills == null || hisSSBills.Count <= 0)
                {
                    throw new Exception("Khong lay duoc SSBill theo billId: " + transaction.BILL_TYPE_ID);
                }

                HisSereServViewFilter sereServFilter = new HisSereServViewFilter();
                sereServFilter.IDs = hisSSBills.Select(s => s.SERE_SERV_ID).ToList();
                var listSereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, sereServFilter, null);
                if (listSereServ != null && listSereServ.Count > 0)
                {
                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.transactionPrint != null ? this.transactionPrint.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                    var Groups = listSereServ.GroupBy(o => o.SERVICE_REQ_ID).ToList();
                    foreach (var group in Groups)
                    {
                        var listSub = group.ToList<V_HIS_SERE_SERV>();
                        MPS.Processor.Mps000103.PDO.Mps000103PDO pdo = new MPS.Processor.Mps000103.PDO.Mps000103PDO(
                            patient,
                            transaction,
                            listSub,
                            currentPatientTypeAlter,
                            ratio_text);
                        WaitingManager.Hide();


                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InBienLaiThuPhiLePhi(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                var transaction = GetTransaction(this.transactionPrint.TRANSACTION_CODE);
                if (transaction == null) return;
                //if (this.transactionPrint.IS_CANCEL == 1)
                //{
                //    MessageManager.Show(Base.ResourceMessageLang.GiaoDichDaBiHuy);
                //    return;
                //}
                WaitingManager.Show();
                V_HIS_PATIENT patient = null;

                if (transaction.TDL_PATIENT_ID.HasValue)
                {
                    HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                    patientFilter.ID = transaction.TDL_PATIENT_ID;
                    var listPatient = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, patientFilter, null);
                    if (listPatient != null && listPatient.Count == 1)
                    {
                        patient = listPatient.First();
                    }
                }

                HisPatientTypeAlterViewAppliedFilter ptAlterAppFilter = new HisPatientTypeAlterViewAppliedFilter();
                ptAlterAppFilter.TreatmentId = transaction.TREATMENT_ID ?? 0;
                ptAlterAppFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, ptAlterAppFilter, null);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.transactionPrint != null ? this.transactionPrint.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                MPS.Processor.Mps000114.PDO.Mps000114PDO rdo = new MPS.Processor.Mps000114.PDO.Mps000114PDO(
                    transaction,
                    patient,
                    0,
                    currentPatientTypeAlter);
                WaitingManager.Hide();
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuChiDinhTheoGiaoDichThanhToan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                var transaction = GetTransaction(this.transactionPrint.TRANSACTION_CODE);
                if (transaction == null) return;
                if (!transaction.TREATMENT_ID.HasValue)
                {
                    MessageManager.Show("Hóa đơn thanh toán xuất thuốc vật tư");
                    return;
                }
                //if (this.transactionPrint.IS_CANCEL == 1)
                //{
                //    MessageManager.Show(Base.ResourceMessageLang.GiaoDichDaBiHuy);
                //    return;
                //}
                WaitingManager.Show();
                //V_HIS_PATY_ALTER_BHYT patyAlterBhyt = null;
                HisPatientTypeAlterViewAppliedFilter ptAlterAppFilter = new HisPatientTypeAlterViewAppliedFilter();
                ptAlterAppFilter.TreatmentId = transaction.TREATMENT_ID.Value;
                ptAlterAppFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, ptAlterAppFilter, null);

                // tính mức hưởng của thẻ
                string levelCode = HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT;
                string ratio_text = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentPatientTypeAlter.HEIN_CARD_NUMBER, currentPatientTypeAlter.LEVEL_CODE, currentPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100) + "";
                V_HIS_PATIENT patient = new V_HIS_PATIENT();
                if (transaction.TDL_PATIENT_ID.HasValue)
                {
                    HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                    patientFilter.ID = transaction.TDL_PATIENT_ID;
                    var patients = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, null);

                    if (patients != null && patients.Count > 0)
                    {
                        patient = patients.FirstOrDefault();
                    }
                }

                HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                ssBillFilter.BILL_ID = transaction.ID;
                var hisSSBills = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, null);
                if (hisSSBills == null || hisSSBills.Count <= 0)
                {
                    throw new Exception("Khong lay duoc SSBill theo billId: " + transaction.BILL_TYPE_ID);
                }

                HisSereServViewFilter sereServFilter = new HisSereServViewFilter();
                sereServFilter.TREATMENT_ID = transaction.TREATMENT_ID;
                sereServFilter.IDs = hisSSBills.Select(s => s.SERE_SERV_ID).ToList();
                var listSereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, sereServFilter, null);
                if (listSereServ != null && listSereServ.Count > 0)
                {
                    var Groups = listSereServ.GroupBy(o => o.SERVICE_REQ_ID).ToList();
                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.transactionPrint != null ? this.transactionPrint.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                    foreach (var group in Groups)
                    {
                        var listSub = group.ToList<V_HIS_SERE_SERV>();
                        V_HIS_SERVICE_REQ serviceReq = null;
                        HisServiceReqViewFilter serviceReqFilter = new HisServiceReqViewFilter();
                        serviceReqFilter.ID = listSub.First().SERVICE_REQ_ID;
                        var listServiceReq = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW, ApiConsumers.MosConsumer, serviceReqFilter, null);
                        if (listServiceReq != null && listServiceReq.Count > 0)
                        {
                            serviceReq = listServiceReq.First();
                        }
                        MPS.Processor.Mps000105.PDO.Mps000105PDO rdo = new MPS.Processor.Mps000105.PDO.Mps000105PDO(
                            transaction,
                            listSub,
                            currentPatientTypeAlter,
                            serviceReq,
                            patient,
                            ratio_text);
                        WaitingManager.Hide();
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                    }
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuHoanUngDichVu(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                var transaction = GetTransaction(this.transactionPrint.TRANSACTION_CODE);
                if (transaction == null) return;
                if (transaction.IS_CANCEL == 1)
                {
                    MessageManager.Show("Giao dịch đã bị hủy");
                    return;
                }
                WaitingManager.Show();
                //var patientADO = PrintGlobalStore.getPatient(this.transactionPrint.TREATMENT_ID.Value);
                var patyAlterBhytADO = new V_HIS_PATIENT_TYPE_ALTER();
                PrintGlobalStore.LoadCurrentPatientTypeAlter(transaction.TREATMENT_ID.Value, 0, ref patyAlterBhytADO);
                //List<SereServGroupPlusADO> sereServNotHiTechs = new List<SereServGroupPlusADO>();

                //bỏ rebay
                HisTransactionViewFilter repayFilter = new HisTransactionViewFilter();
                repayFilter.ID = transaction.ID;
                var listRepay = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, repayFilter, null);
                if (listRepay == null || listRepay.Count != 1)
                {
                    throw new Exception("Khong lay duoc V_HIS_REPAY theo transactionId: " + transaction.ID);
                }
                var repay = listRepay.FirstOrDefault();

                MOS.Filter.HisSeseDepoRepayFilter hisSeseDepoRepayFilter = new MOS.Filter.HisSeseDepoRepayFilter();
                hisSeseDepoRepayFilter.REPAY_ID = repay.ID;
                var seseDepoRepays = new BackendAdapter(new CommonParam()).Get<List<HIS_SESE_DEPO_REPAY>>("api/HisSeseDepoRepay/Get", ApiConsumers.MosConsumer, hisSeseDepoRepayFilter, null).ToList();

                List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans = new BackendAdapter(new CommonParam()).Get<List<V_HIS_DEPARTMENT_TRAN>>(HisRequestUriStore.HIS_DEPARTMENT_TRAN_GETHOSPITALINOUT, ApiConsumers.MosConsumer, transaction.TREATMENT_ID, null);

                HisTreatmentFeeViewFilter filterTreatmentFee = new HisTreatmentFeeViewFilter();
                filterTreatmentFee.ID = transaction.TREATMENT_ID.Value;
                var treatmentFee = new BackendAdapter(new CommonParam())
                  .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, filterTreatmentFee, null).FirstOrDefault();

                long? totalDay = null;

                if (treatmentFee != null && treatmentFee.TDL_PATIENT_TYPE_ID == Config.HisConfigCFG.PatientTypeId__BHYT)
                {
                    totalDay = HIS.Common.Treatment.Calculation.DayOfTreatment(treatmentFee.IN_TIME, treatmentFee.OUT_TIME, treatmentFee.TREATMENT_END_TYPE_ID, treatmentFee.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT);
                }
                else
                {
                    totalDay = HIS.Common.Treatment.Calculation.DayOfTreatment(treatmentFee.IN_TIME, treatmentFee.OUT_TIME, treatmentFee.TREATMENT_END_TYPE_ID, treatmentFee.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI);
                }
                string departmentName = "";

                MPS.Processor.Mps000110.PDO.PatientADO mpsPatientADO = new MPS.Processor.Mps000110.PDO.PatientADO();

                //if (patientADO != null)
                //{
                //    Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000110.PDO.PatientADO>(mpsPatientADO, patientADO);
                //}
                HisDepartmentTranLastFilter departLastFilter = new HisDepartmentTranLastFilter();
                departLastFilter.TREATMENT_ID = transaction.TREATMENT_ID.Value;
                departLastFilter.BEFORE_LOG_TIME = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                var departmentTran = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, departLastFilter, null);
                AutoMapper.Mapper.CreateMap<V_HIS_TREATMENT_FEE, V_HIS_TREATMENT_1>();
                var treatments = AutoMapper.Mapper.Map<V_HIS_TREATMENT_FEE, V_HIS_TREATMENT_1>(treatmentFee);
                MPS.Processor.Mps000110.PDO.Mps000110PDO pdo = new MPS.Processor.Mps000110.PDO.Mps000110PDO(mpsPatientADO,
                    patyAlterBhytADO,
                    departmentName,
                    seseDepoRepays,
                    departmentTrans,
                    treatments,
                    repay,
                    totalDay,
                    departmentTran);
                MPS.ProcessorBase.Core.PrintData printData = null;

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatments != null ? treatments.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);
                WaitingManager.Hide();
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuTamUngVaGiuThe(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                var transaction = GetTransaction(this.transactionPrint.TRANSACTION_CODE);
                if (transaction == null) return;

                if (!transaction.TREATMENT_ID.HasValue)
                {
                    MessageManager.Show("Hóa đơn thanh toán xuất bán thuốc vật tư");
                    return;
                }
                if (transaction.IS_CANCEL == 1)
                {
                    MessageManager.Show("Giao dịch đã bị hủy");
                    return;
                }
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                HisTransactionViewFilter depositFilter = new HisTransactionViewFilter();
                depositFilter.ID = transaction.ID;
                var listDeposit = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, depositFilter, null);
                if (listDeposit == null || listDeposit.Count != 1)
                {
                    throw new Exception("Khong lay duoc V_HIS_DEPOSIT theo transactionId: " + transaction.ID);
                }

                var deposit = listDeposit.First();

                HisTreatmentFeeViewFilter filterTreatmentFee = new HisTreatmentFeeViewFilter();
                filterTreatmentFee.ID = transaction.TREATMENT_ID.Value;
                var treatmentFee = new BackendAdapter(new CommonParam())
                  .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, filterTreatmentFee, null).FirstOrDefault();

                HisPatientTypeAlterViewAppliedFilter appFilter = new HisPatientTypeAlterViewAppliedFilter();
                long nowTime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                if (treatmentFee != null && treatmentFee.IN_TIME > nowTime)
                {
                    appFilter.InstructionTime = treatmentFee.IN_TIME;
                }
                else
                {
                    appFilter.InstructionTime = nowTime;
                }
                appFilter.TreatmentId = transaction.TREATMENT_ID.Value;
                var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(param).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, appFilter, null);
                if (currentPatientTypeAlter == null)
                {
                    throw new Exception("Khong lay duoc thong tin doi tuong cua ho so dieu tri Id: " + transaction.TREATMENT_ID);
                }
                //HIS_PATY_ALTER_BHYT patyAlter = null;
                //if (currentPatientTypeAlter.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT)
                //{
                //    HisPatyAlterBhytFilter patyBhytFilter = new HisPatyAlterBhytFilter();
                //    patyBhytFilter.PATIENT_TYPE_ALTER_ID = currentPatientTypeAlter.ID;
                //    var listPatyAlterBhyt = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_PATY_ALTER_BHYT>>("api/HisPatyAlterBhyt/Get", ApiConsumers.MosConsumer, patyBhytFilter, null);
                //    if (listPatyAlterBhyt == null || listPatyAlterBhyt.Count != 1)
                //    {
                //        throw new Exception("Khong lay duoc thong tin the BHYT cua ho so dieu tri Id: " + this.transactionPrint.TREATMENT_ID);
                //    }
                //    patyAlter = listPatyAlterBhyt.FirstOrDefault();
                //}

                decimal ratio = 0;
                if (currentPatientTypeAlter != null)
                {
                    ratio = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentPatientTypeAlter.HEIN_CARD_NUMBER, currentPatientTypeAlter.LEVEL_CODE, currentPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0;
                }

                HisDepartmentTranLastFilter departLastFilter = new HisDepartmentTranLastFilter();
                departLastFilter.TREATMENT_ID = transaction.TREATMENT_ID.Value;
                //long nowTime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                if (treatmentFee != null && treatmentFee.IN_TIME > nowTime)
                {
                    departLastFilter.BEFORE_LOG_TIME = treatmentFee.IN_TIME;
                }
                else
                {
                    departLastFilter.BEFORE_LOG_TIME = nowTime;
                }
                var departmentTran = new Inventec.Common.Adapter.BackendAdapter(param).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, departLastFilter, param);

                HIS_PATIENT patient = new HIS_PATIENT();
                HisPatientFilter filter = new HisPatientFilter();
                filter.ID = transaction.TDL_PATIENT_ID;
                var patients = new BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                if (patients != null && patients.Count > 0)
                {
                    patient = patients.First();
                }

                MPS.Processor.Mps000171.PDO.Mps000171PDO pdo = new MPS.Processor.Mps000171.PDO.Mps000171PDO(deposit, currentPatientTypeAlter, departmentTran, ratio, patient);
                MPS.ProcessorBase.Core.PrintData printData = null;
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((deposit != null ? deposit.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);
                WaitingManager.Hide();
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuTamUngCoThongTinBHYT(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                var transaction = GetTransaction(this.transactionPrint.TRANSACTION_CODE);
                if (transaction == null) return;

                if (!transaction.TREATMENT_ID.HasValue)
                {
                    MessageManager.Show("Hóa đơn thanh toán xuất bán thuốc vật tư");
                    return;
                }
                if (transaction.IS_CANCEL == 1)
                {
                    MessageManager.Show("Giao dịch đã bị hủy");
                    return;
                }
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                HisTransactionViewFilter depositFilter = new HisTransactionViewFilter();
                depositFilter.ID = transaction.ID;
                var listDeposit = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, depositFilter, null);
                if (listDeposit == null || listDeposit.Count != 1)
                {
                    throw new Exception("Khong lay duoc V_HIS_DEPOSIT theo transactionId: " + transaction.ID);
                }

                var deposit = listDeposit.First();

                HisTreatmentFeeViewFilter filterTreatmentFee = new HisTreatmentFeeViewFilter();
                filterTreatmentFee.ID = transaction.TREATMENT_ID.Value;
                var treatmentFee = new BackendAdapter(new CommonParam())
                  .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, filterTreatmentFee, null).FirstOrDefault();

                HisPatientTypeAlterViewAppliedFilter appFilter = new HisPatientTypeAlterViewAppliedFilter();
                long nowTime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                if (treatmentFee != null && treatmentFee.IN_TIME > nowTime)
                {
                    appFilter.InstructionTime = treatmentFee.IN_TIME;
                }
                else
                {
                    appFilter.InstructionTime = nowTime;
                }
                appFilter.TreatmentId = transaction.TREATMENT_ID.Value;
                var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(param).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, appFilter, null);
                if (currentPatientTypeAlter == null)
                {
                    throw new Exception("Khong lay duoc thong tin doi tuong cua ho so dieu tri Id: " + transaction.TREATMENT_ID);
                }
                //HIS_PATY_ALTER_BHYT patyAlter = null;
                //if (currentPatientTypeAlter.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT)
                //{
                //    HisPatyAlterBhytFilter patyBhytFilter = new HisPatyAlterBhytFilter();
                //    patyBhytFilter.PATIENT_TYPE_ALTER_ID = currentPatientTypeAlter.ID;
                //    var listPatyAlterBhyt = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_PATY_ALTER_BHYT>>("api/HisPatyAlterBhyt/Get", ApiConsumers.MosConsumer, patyBhytFilter, null);
                //    if (listPatyAlterBhyt == null || listPatyAlterBhyt.Count != 1)
                //    {
                //        throw new Exception("Khong lay duoc thong tin the BHYT cua ho so dieu tri Id: " + this.transactionPrint.TREATMENT_ID);
                //    }
                //    patyAlter = listPatyAlterBhyt.FirstOrDefault();
                //}

                HisDepartmentTranViewFilter departLastFilter = new HisDepartmentTranViewFilter();
                departLastFilter.TREATMENT_ID = transaction.TREATMENT_ID ?? 0;
                departLastFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var departmentTrans = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/GetView", ApiConsumers.MosConsumer, departLastFilter, param);

                decimal ratio = 0;
                if (currentPatientTypeAlter != null)
                {
                    ratio = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentPatientTypeAlter.HEIN_CARD_NUMBER, currentPatientTypeAlter.LEVEL_CODE, currentPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0;
                }

                HIS_PATIENT patient = new HIS_PATIENT();
                HisPatientFilter filter = new HisPatientFilter();
                filter.ID = transaction.TDL_PATIENT_ID;
                var patients = new BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                if (patients != null && patients.Count > 0)
                {
                    patient = patients.First();
                }

                MPS.Processor.Mps000172.PDO.Mps000172PDO pdo = new MPS.Processor.Mps000172.PDO.Mps000172PDO(deposit, currentPatientTypeAlter, departmentTrans, ratio, patient);
                MPS.ProcessorBase.Core.PrintData printData = null;
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((deposit != null ? deposit.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);
                WaitingManager.Hide();
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InHoaDonHuyThanhToan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                var transaction = GetTransaction(this.transactionPrint.TRANSACTION_CODE);
                if (transaction == null) return;

                if (!transaction.TREATMENT_ID.HasValue)
                {
                    MessageManager.Show("Hóa đơn thanh toán xuất bán thuốc vật tư");
                    return;
                }

                MPS.Processor.Mps000337.PDO.Mps000337PDO rdo = new MPS.Processor.Mps000337.PDO.Mps000337PDO(transaction);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                }
                else
                {
                    result = result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public static List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO> PriceBHYTSereServAdoProcess(List<V_HIS_SERE_SERV_12> sereServs)
        {
            List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO> sereServADOs = new List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();

            try
            {
                foreach (var item in sereServs)
                {

                    MPS.Processor.Mps000102.PDO.SereServGroupPlusADO sereServADO = new MPS.Processor.Mps000102.PDO.SereServGroupPlusADO();
                    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_12, MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();
                    sereServADO = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_12, MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>(item);
                    var patientTypeCFG = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT");
                    if (sereServADO.PATIENT_TYPE_CODE != patientTypeCFG)
                    {
                        sereServADO.PRICE_BHYT = 0;
                    }
                    else
                    {
                        if (sereServADO.HEIN_LIMIT_PRICE != null && sereServADO.HEIN_LIMIT_PRICE > 0)
                            sereServADO.PRICE_BHYT = (item.HEIN_LIMIT_PRICE ?? 0);
                        else
                            sereServADO.PRICE_BHYT = item.VIR_PRICE_NO_ADD_PRICE ?? 0;
                    }

                    sereServADO.SERVICE_UNIT_NAME = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(o => o.ID == item.TDL_SERVICE_UNIT_ID).SERVICE_UNIT_NAME;

                    sereServADOs.Add(sereServADO);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return sereServADOs;
        }

        private V_HIS_SERVICE_REQ FirstExamRoom(long treatmentId)
        {

            V_HIS_SERVICE_REQ result = null;
            try
            {
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisServiceReqViewFilter serviceReqFilter = new HisServiceReqViewFilter();

                serviceReqFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;

                serviceReqFilter.TREATMENT_ID = treatmentId;

                var listServiceReq = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW, ApiConsumers.MosConsumer, serviceReqFilter, paramCommon);
                if (listServiceReq != null && listServiceReq.Count > 0)
                {
                    result = listServiceReq.OrderBy(o => o.CREATE_TIME).FirstOrDefault();
                }
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
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

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("Inventec.Desktop.Plugins.PrintLog", currentModule.RoomId, currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InHoaDonXuatBan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                var transaction = GetTransaction(this.transactionPrint.TRANSACTION_CODE);
                if (transaction == null) return;
                WaitingManager.Show();

                CommonParam param = new CommonParam();
                HisBillGoodsFilter goodsFilter = new HisBillGoodsFilter();
                goodsFilter.BILL_ID = transaction.ID;
                List<HIS_BILL_GOODS> billGoods = new BackendAdapter(param).Get<List<HIS_BILL_GOODS>>("api/HisBillGoods/Get", ApiConsumers.MosConsumer, goodsFilter, param);

                HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                expMestFilter.BILL_ID = transaction.ID;
                List<V_HIS_EXP_MEST> expMests = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, expMestFilter, param);

                HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                expMestMedicineFilter.EXP_MEST_IDs = expMests.Select(s => s.ID).ToList();
                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = new BackendAdapter(param)
                    .Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, expMestMedicineFilter, param);

                HisExpMestMaterialViewFilter expMestMaterialFilter = new HisExpMestMaterialViewFilter();
                expMestMaterialFilter.EXP_MEST_IDs = expMests.Select(s => s.ID).ToList();
                List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials = new BackendAdapter(param)
                    .Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, expMestMaterialFilter, param);

                MPS.Processor.Mps000339.PDO.Mps000339PDO rdo = new MPS.Processor.Mps000339.PDO.Mps000339PDO(transaction, billGoods, expMestMedicines, expMestMaterials);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                MPS.ProcessorBase.Core.PrintData printdata = null;
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    printdata = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                }
                else
                {
                    printdata = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                }

                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(printdata);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

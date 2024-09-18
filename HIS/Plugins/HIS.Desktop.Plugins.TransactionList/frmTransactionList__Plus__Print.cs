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
using AutoMapper;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.ElectronicBill;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Plugins.TransactionList.ADO;
using HIS.Desktop.Plugins.TransactionList.Config;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.DocumentViewer;
using Inventec.Common.LocalStorage.SdaConfig;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;

namespace HIS.Desktop.Plugins.TransactionList
{
    public partial class frmTransactionList : HIS.Desktop.Utility.FormBase
    {
        private string Print106Type = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Print.TransactionDetail");
        private string Print106Type_Expend = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Print.TransactionDetail_Expend");
        private HisServiceReqListResultSDO HisServiceReqListResultSDO { get; set; }
        const string invoiceTypeCreate__CreateInvoiceHIS = "2";

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
                        case PopupMenuProcessor.ItemType.ThayThe:
                            this.ThayThe();
                            break;
                        case PopupMenuProcessor.ItemType.InBienLaiHuy:
                            this.PrintBienLaiHuyHoaDon();
                            break;
                        case PopupMenuProcessor.ItemType.InHoaDonXuatBan:
                            this.PrintHoaDonXuatBan();
                            break;
                        case PopupMenuProcessor.ItemType.InHoaDonBanThuoc:
                            this.PrintHoaDonBanThuocMps344();
                            break;
                        case PopupMenuProcessor.ItemType.HuyHoaDonDienTu:
                            this.HuyHoaDonDienTu();
                            break;
                        case PopupMenuProcessor.ItemType.BangKe:
                            this.BangKeThanhToan();
                            break;
                        case PopupMenuProcessor.ItemType.InPhieuChotNo:
                            this.PrintPhieuChotNo();
                            break;
                        case PopupMenuProcessor.ItemType.InPhieuThuNo:
                            this.PrintPhieuThuCongNo();
                            break;
                        case PopupMenuProcessor.ItemType.MPS373_BKChiTietThuVP:
                            this.PrintMps373();
                            break;
                        case PopupMenuProcessor.ItemType.InPhieuHuyTamUng:
                            this.PrintPhieuHuyTamUng();
                            break;
                        case PopupMenuProcessor.ItemType.XuatHoaDonDienTu:
                            this.MouseRight_XuatHoaDonDienTu(this.transactionPrint);
                            break;
                        case PopupMenuProcessor.ItemType.TransactionEInvoice:
                            this.TransactionEInvoice();
                            break;
                        case PopupMenuProcessor.ItemType.ChuyenDoiHoaDonDienTu:
                            this.ChuyenDoiHoaDon();
                            break;
                        case PopupMenuProcessor.ItemType.Mps000439_BienBanDieuChinhThongTinHanhChinhHoaDon__:
                            this.BienBanDieuChinhThongTinHanhChinhHoaDon();
                            break;
                        case PopupMenuProcessor.ItemType.Mps000440_BienBanDieuChinhTangGiamTrenHoaDon__:
                            this.BienBanDieuChinhTangGiamTrenHoaDon();
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
        private void MouseRight_XuatHoaDonDienTu(V_HIS_TRANSACTION transactionBill)
        {
            try
            {
                if (transactionBill == null) return;

                if (HisConfigCFG.ElectronicBillExportOption == "1")
                {
                    if (transactionBill.CASHIER_LOGINNAME != this.loginName)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Giao dịch có mã {0} bạn không phải là người tạo nên sẽ không xuất được giao dịch này.", transactionBill.TRANSACTION_CODE), "Thông báo");
                        return;
                    }
                }
                if(transactionBill.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE && (MessageBox.Show("Giao dịch đang tạm khóa bạn có muốn tiếp tục", Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo) == DialogResult.No))
                {
                    return;
                }
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = this.XuatHoaDonDienTu(transactionBill, false, ref param);
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool CancelElectronicBill()
        {
            var result = true;
            try
            {
                ElectronicBillDataInput dataInput = new ElectronicBillDataInput();
                dataInput.PartnerInvoiceID = Inventec.Common.TypeConvert.Parse.ToInt64(transactionPrint.INVOICE_CODE);
                dataInput.InvoiceCode = transactionPrint.INVOICE_CODE;
                dataInput.NumOrder = transactionPrint.NUM_ORDER;
                dataInput.SymbolCode = transactionPrint.SYMBOL_CODE;
                dataInput.TemplateCode = transactionPrint.TEMPLATE_CODE;
                dataInput.TransactionTime = transactionPrint.EINVOICE_TIME ?? transactionPrint.TRANSACTION_TIME;
                dataInput.ENumOrder = transactionPrint.EINVOICE_NUM_ORDER;
                dataInput.EinvoiceTypeId = transactionPrint.EINVOICE_TYPE_ID;
                dataInput.TransactionCode = transactionPrint.TRANSACTION_CODE;
                dataInput.Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());

                ElectronicBillProcessor electronicBillProcessor = new ElectronicBillProcessor(dataInput);
                ElectronicBillResult electronicBillResult = electronicBillProcessor.Run(ElectronicBillType.ENUM.CANCEL_INVOICE);
                if (electronicBillResult != null && !electronicBillResult.Success)
                {
                    string mes = "";
                    if (electronicBillResult.Messages != null && electronicBillResult.Messages.Count > 0)
                    {
                        foreach (var item in electronicBillResult.Messages)
                        {
                            mes += item + ";";
                        }
                        XtraMessageBox.Show("Hủy hóa đơn điện tử thất bại." + mes + ". Vui lòng truy cập hệ thống hóa đơn điện tử để hủy giao dịch.");
                        result = false;
                    }
                    //DialogResult myResult;
                    //myResult = MessageBox.Show("Hủy hóa đơn điện tử thất bại." + mes + ". Bạn có muốn tiếp tục hủy giao dịch trên HIS?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    //if (myResult != DialogResult.OK)
                    //{
                    //    result = false;
                    //}
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void HuyHoaDonDienTu()
        {
            try
            {
                if (MessageBox.Show("Bạn có muốn hủy hóa đơn điện tử?", Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    if (CancelElectronicBill())
                    {
                        bool success = false;
                        WaitingManager.Show();
                        CommonParam param = new CommonParam();
                        CancelInvoiceSDO sdo = new CancelInvoiceSDO();
                        sdo.TransactionId = this.transactionPrint.ID;
                        sdo.CancelTime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                        var result = new BackendAdapter(param).Post<HIS_TRANSACTION>("api/HisTransaction/CancelInvoice", ApiConsumers.MosConsumer, sdo, param);
                        if (result != null)
                        {
                            success = true;
                            FillDataToGridTransaction(new CommonParam(start, limit));
                        }
                        WaitingManager.Hide();
                        if (success)
                        {
                            MessageManager.Show(this, param, success);
                        }
                        else
                        {
                            MessageManager.Show(param, success);
                        }
                    }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BienBanDieuChinhThongTinHanhChinhHoaDon()
        {
            if (this.transactionPrint == null) return;
            Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
            store.RunPrintTemplate("Mps000439", DelegateRunPrinter);
        }

        private void BienBanDieuChinhTangGiamTrenHoaDon()
        {
            if (this.transactionPrint == null) return;
            Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
            store.RunPrintTemplate("Mps000440", DelegateRunPrinter);
        }

        private void ChuyenDoiHoaDon()
        {
            try
            {
                if (transactionPrint.EINVOICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__VNPT && (transactionPrint.EINVOICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__VIETEL && HisConfigCFG.autoPrintType != "1"))
                {
                    return;
                }

                ElectronicBillDataInput dataInput = new ElectronicBillDataInput();
                dataInput.PartnerInvoiceID = Inventec.Common.TypeConvert.Parse.ToInt64(this.transactionPrint.INVOICE_CODE);

                dataInput.InvoiceCode = transactionPrint.INVOICE_CODE;
                dataInput.NumOrder = transactionPrint.NUM_ORDER;
                dataInput.SymbolCode = transactionPrint.SYMBOL_CODE;
                dataInput.TemplateCode = transactionPrint.TEMPLATE_CODE;
                dataInput.TransactionTime = transactionPrint.EINVOICE_TIME ?? transactionPrint.TRANSACTION_TIME;
                dataInput.ENumOrder = transactionPrint.EINVOICE_NUM_ORDER;
                dataInput.EinvoiceTypeId = transactionPrint.EINVOICE_TYPE_ID;

                HIS_TRANSACTION tran = new HIS_TRANSACTION();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(tran, transactionPrint);
                dataInput.Transaction = tran;
                V_HIS_TREATMENT_FEE treatment2 = new V_HIS_TREATMENT_FEE();
                if (transactionPrint.TREATMENT_ID.HasValue)
                {
                    MOS.Filter.HisTreatmentFeeViewFilter filter = new HisTreatmentFeeViewFilter();
                    filter.ID = transactionPrint.TREATMENT_ID;
                    treatment2 = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumer.ApiConsumers.MosConsumer, filter, null).FirstOrDefault();
                }
                else
                {
                    treatment2.TDL_PATIENT_ACCOUNT_NUMBER = transactionPrint.BUYER_ACCOUNT_NUMBER;
                    treatment2.TDL_PATIENT_ADDRESS = transactionPrint.BUYER_ADDRESS;
                    treatment2.TDL_PATIENT_PHONE = transactionPrint.BUYER_PHONE;
                    treatment2.TDL_PATIENT_TAX_CODE = transactionPrint.BUYER_TAX_CODE;
                    treatment2.TDL_PATIENT_WORK_PLACE = transactionPrint.BUYER_ORGANIZATION;
                    treatment2.TDL_PATIENT_NAME = transactionPrint.BUYER_NAME;
                    treatment2.ID = -1;//để các api trong thư viện không lấy được dữ liệu
                    treatment2.PATIENT_ID = -1;
                }

                string serviceConfig = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.DESKTOP.LIBRARY.ELECTRONIC_BILL.CONFIG");

                if (serviceConfig.Contains(ProviderType.VIETTEL) &&
                    XtraMessageBox.Show("Bạn có muốn lấy thông tin người chuyển đổi theo thông tin người lưu ký không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                {
                    dataInput.Converter = transactionPrint.CASHIER_USERNAME;
                }

                dataInput.Treatment = treatment2;
                dataInput.Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                ElectronicBillProcessor electronicBillProcessor = new ElectronicBillProcessor(dataInput);
                ElectronicBillResult electronicBillResult = electronicBillProcessor.Run(ElectronicBillType.ENUM.CONVERT_INVOICE);

                if (electronicBillResult == null || String.IsNullOrEmpty(electronicBillResult.InvoiceLink))
                {
                    if (electronicBillResult != null && electronicBillResult.Messages != null && electronicBillResult.Messages.Count > 0)
                    {
                        MessageBox.Show("Chuyển đổi hóa đơn điện tử thất bại. " + string.Join(". ", electronicBillResult.Messages));
                    }
                    else
                        MessageBox.Show("Chuyển đổi hóa đơn điện tử thất bại");
                    return;
                }

                DocumentViewerManager viewManager = new DocumentViewerManager(ViewType.ENUM.Pdf);
                InputADO ado = new InputADO();
                ado.DeleteWhenClose = true;
                ado.NumberOfCopy = HisConfigCFG.E_BILL__PRINT_NUM_COPY;
                ado.PrintPageSize = transactionPrint.EINVOICE_PAGE_SIZE;
                ado.URL = electronicBillResult.InvoiceLink;
                ViewType.Platform type = ViewType.Platform.Telerik;
                if (HisConfigCFG.PlatformOption > 0)
                {
                    type = (ViewType.Platform)(HisConfigCFG.PlatformOption - 1);
                }

                viewManager.Run(ado, type);
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
                if (transactionPrint.TRANSACTION_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT || !String.IsNullOrWhiteSpace(transactionPrint.INVOICE_CODE))
                {
                    return;
                }

                List<object> listArgs = new List<object>();
                listArgs.Add(transactionPrint);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.TransactionEInvoice", currentModule.RoomId, currentModule.RoomTypeId, listArgs);
                FillDataToGridTransaction(new CommonParam(start, limit));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintPhieuHuyTamUng()
        {
            try
            {
                if (this.transactionPrint == null) return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate("Mps000381", DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThayThe()
        {
            try
            {
                if (this.transactionPrint == null || !this.transactionPrint.TREATMENT_ID.HasValue)
                    return;

                List<object> listArgs = new List<object>();
                listArgs.Add(this.transactionPrint);
                V_HIS_TREATMENT_FEE treatment = GetTreatmentById(this.transactionPrint.TREATMENT_ID.Value);
                listArgs.Add(treatment);
                List<V_HIS_SERE_SERV_5> listSereServ = GetListSereServ5ByTreatmentId(this.transactionPrint.TREATMENT_ID.Value);
                listArgs.Add(listSereServ);
                V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = GetPatientTypeAlterByTreatmentId(this.transactionPrint.TREATMENT_ID.Value);
                listArgs.Add(patientTypeAlter);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.TransactionBill", currentModule.RoomId, currentModule.RoomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private V_HIS_PATIENT_TYPE_ALTER GetPatientTypeAlterByTreatmentId(long id)
        {
            V_HIS_PATIENT_TYPE_ALTER result = null;
            try
            {
                if (id <= 0)
                    return null;
                MOS.Filter.HisPatientTypeAlterViewFilter patientTypeAlterViewFilter = new HisPatientTypeAlterViewFilter();
                patientTypeAlterViewFilter.TREATMENT_ID = id;
                var patientTypeAlters = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientTypeAlterViewFilter, null);

                if (patientTypeAlters != null && patientTypeAlters.Count > 0)
                {
                    result = patientTypeAlters.OrderByDescending(o => o.LOG_TIME).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<V_HIS_SERE_SERV_5> GetListSereServ5ByTreatmentId(long id)
        {
            List<V_HIS_SERE_SERV_5> result = null;
            try
            {
                if (id <= 0)
                    return null;
                MOS.Filter.HisSereServView5Filter filter = new HisSereServView5Filter();
                filter.TDL_TREATMENT_ID = id;
                result = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private V_HIS_TREATMENT_FEE GetTreatmentById(long id)
        {
            V_HIS_TREATMENT_FEE result = null;
            try
            {
                if (id <= 0)
                    return null;
                MOS.Filter.HisTreatmentFeeViewFilter filter = new HisTreatmentFeeViewFilter();
                filter.ID = id;
                result = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumer.ApiConsumers.MosConsumer, filter, null).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void BangKeThanhToan()
        {
            try
            {
                if (this.transactionPrint == null || !this.transactionPrint.TREATMENT_ID.HasValue)
                    return;

                List<object> listArgs = new List<object>();
                listArgs.Add(transactionPrint.TREATMENT_ID);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.Bordereau", currentModule.RoomId, currentModule.RoomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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
                if (this.transactionPrint == null) return;
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionNumOrderUpdate").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TransactionNumOrderUpdate'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(transactionPrint);
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("extenceInstance is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                    FillDataToGridTransaction(new CommonParam(start, limit));
                }
                else
                {
                    MessageManager.Show(Base.ResourceMessageLang.ChucNangNayChuaDuocHoTroTrongPhienBanNay);
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
                ElectronicBillDataInput dataInput = new ElectronicBillDataInput();
                dataInput.PartnerInvoiceID = Inventec.Common.TypeConvert.Parse.ToInt64(this.transactionPrint.INVOICE_CODE);

                dataInput.InvoiceCode = transactionPrint.INVOICE_CODE;
                dataInput.NumOrder = transactionPrint.NUM_ORDER;
                dataInput.SymbolCode = transactionPrint.SYMBOL_CODE;
                dataInput.TemplateCode = transactionPrint.TEMPLATE_CODE;
                dataInput.TransactionTime = transactionPrint.EINVOICE_TIME ?? transactionPrint.TRANSACTION_TIME;
                dataInput.ENumOrder = transactionPrint.EINVOICE_NUM_ORDER;
                dataInput.EinvoiceTypeId = transactionPrint.EINVOICE_TYPE_ID;

                HIS_TRANSACTION tran = new HIS_TRANSACTION();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(tran, transactionPrint);
                dataInput.Transaction = tran;
                V_HIS_TREATMENT_FEE treatment2 = new V_HIS_TREATMENT_FEE();
                if (transactionPrint.TREATMENT_ID.HasValue)
                {
                    MOS.Filter.HisTreatmentFeeViewFilter filter = new HisTreatmentFeeViewFilter();
                    filter.ID = transactionPrint.TREATMENT_ID;
                    treatment2 = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumer.ApiConsumers.MosConsumer, filter, null).FirstOrDefault();
                }
                else
                {
                    treatment2.TDL_PATIENT_ACCOUNT_NUMBER = transactionPrint.BUYER_ACCOUNT_NUMBER;
                    treatment2.TDL_PATIENT_ADDRESS = transactionPrint.BUYER_ADDRESS;
                    treatment2.TDL_PATIENT_PHONE = transactionPrint.BUYER_PHONE;
                    treatment2.TDL_PATIENT_TAX_CODE = transactionPrint.BUYER_TAX_CODE;
                    treatment2.TDL_PATIENT_WORK_PLACE = transactionPrint.BUYER_ORGANIZATION;
                    treatment2.TDL_PATIENT_NAME = transactionPrint.BUYER_NAME;
                    treatment2.ID = -1;//để các api trong thư viện không lấy được dữ liệu
                    treatment2.PATIENT_ID = -1;
                }

                string serviceConfig = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.DESKTOP.LIBRARY.ELECTRONIC_BILL.CONFIG");

                if ((serviceConfig.Contains(ProviderType.VIETTEL) || transactionPrint.INVOICE_SYS == ProviderType.VIETTEL) &&
                    HisConfigCFG.autoPrintType != "1")
                {
                    if (XtraMessageBox.Show("Bạn có muốn lấy thông tin người chuyển đổi theo thông tin người lưu ký không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                    {
                        dataInput.Converter = transactionPrint.CASHIER_USERNAME;
                    }
                }

                dataInput.Transaction = new HIS_TRANSACTION();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(dataInput.Transaction, transactionPrint);

                dataInput.Treatment = treatment2;
                dataInput.Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                ElectronicBillProcessor electronicBillProcessor = new ElectronicBillProcessor(dataInput);
                ElectronicBillResult electronicBillResult = electronicBillProcessor.Run(ElectronicBillType.ENUM.GET_INVOICE_LINK);

                if (electronicBillResult == null || String.IsNullOrEmpty(electronicBillResult.InvoiceLink))
                {
                    if (electronicBillResult != null && electronicBillResult.Messages != null && electronicBillResult.Messages.Count > 0)
                    {
                        MessageBox.Show("Tải hóa đơn điện tử thất bại. " + string.Join(". ", electronicBillResult.Messages));
                    }
                    else
                        MessageBox.Show("Không tìm thấy link hóa đơn điện tử");
                    return;
                }

                Inventec.Common.DocumentViewer.DocumentViewerManager viewManager = new Inventec.Common.DocumentViewer.DocumentViewerManager(ViewType.ENUM.Pdf);
                InputADO ado = new InputADO();
                ado.DeleteWhenClose = true;
                ado.NumberOfCopy = HisConfigCFG.E_BILL__PRINT_NUM_COPY;
                ado.PrintPageSize = transactionPrint.EINVOICE_PAGE_SIZE;
                ado.URL = electronicBillResult.InvoiceLink;
                ViewType.Platform type = ViewType.Platform.Telerik;
                if (HisConfigCFG.PlatformOption > 0)
                {
                    type = (ViewType.Platform)(HisConfigCFG.PlatformOption - 1);
                }

                FillDataToGrid();
                viewManager.Run(ado, type);
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
                Inventec.Common.RichEditor.RichEditorStore richStore = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                if (this.transactionPrint != null && this.transactionPrint.SALE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SALE_TYPE.ID__SALE_OTHER)
                {
                    if (this.transactionPrint.BILL_TYPE_ID == 1)
                    {
                        richStore.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuThanhToanKhac_MPS000355, this.DelegateRunPrinter);
                    }
                    else
                    {
                        richStore.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuThanhToanKhac_MPS000299, this.DelegateRunPrinter);
                    }
                }
                else if (HisConfigCFG.TransactionBillSelect == "2")
                {
                    if (HisConfigCFG.BILL_TWO_BOOK__OPTION == (int)HisConfigCFG.BILL_OPTION.HCM_115
                        || HisConfigCFG.BILL_TWO_BOOK__OPTION == (int)HisConfigCFG.BILL_OPTION.QBH_CUBA)
                    {
                        if (this.transactionPrint != null && this.transactionPrint.BILL_TYPE_ID == 2)
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
                        if (this.transactionPrint != null && this.transactionPrint.BILL_TYPE_ID == 2)
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
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

                var patientTypeAlter = new HIS_PATIENT_TYPE_ALTER();
                if (transactionPrint != null)
                {
                    var paramCommon = new CommonParam();
                    patientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetLastByTreatmentId", ApiConsumers.MosConsumer, transactionPrint.TREATMENT_ID, paramCommon);
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

        private void PrintMps373()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

                var patientTypeAlter = new HIS_PATIENT_TYPE_ALTER();
                if (transactionPrint != null)
                {
                    var paramCommon = new CommonParam();

                    patientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetLastByTreatmentId", ApiConsumers.MosConsumer, transactionPrint.TREATMENT_ID, paramCommon);
                }

                if (patientTypeAlter != null && patientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                {
                    store.RunPrintTemplate(MPS.Processor.Mps000259.PDO.Mps000259PDO.printTypeCode, this.DelegateRunPrinter);
                }
                else
                {
                    store.RunPrintTemplate(MPS.Processor.Mps000373.PDO.Mps000373PDO.printTypeCode, this.DelegateRunPrinter);
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

        private void PrintBienLaiThuocMps342()
        {
            try
            {
                if (this.transactionPrint == null) return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate("Mps000342", DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintHoaDonDichVuMps343()
        {
            try
            {
                if (this.transactionPrint == null) return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate("Mps000343", DelegateRunPrinter);
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
                if (this.transactionPrint == null) return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate("Mps000344", DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintPhieuChotNo()
        {
            try
            {
                if (this.transactionPrint == null) return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate("Mps000369", DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintPhieuThuCongNo()
        {
            try
            {
                if (this.transactionPrint == null) return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate("Mps000370", DelegateRunPrinter);
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
                    case MPS.Processor.Mps000373.PDO.Mps000373PDO.printTypeCode:
                        InPhieuMps373(printTypeCode, fileName, ref result);
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
                    case "Mps000344":
                        InHoaDonBanThuoc(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000369":
                        InPhieuXacNhanCongNo(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000370":
                        InThuCongNo(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuThanhToanKhac_MPS000355:
                        InPhieuThuThanhToanHoaDon(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000381":
                        InHuyTamUng(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000439":
                        Mps000439_BienBanDieuChinhThongTinHanhChinhHoaDon__(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000440":
                        Mps000440_BienBanDieuChinhTangGiamTrenHoaDon__(printTypeCode, fileName, ref result);
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

        private bool DelegatePrint430(string printTypeCode, string fileName)
        {
            bool result = true;
            try
            {
                if (this.transactionPrint == null || !this.transactionPrint.TREATMENT_ID.HasValue)
                    return false;

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                V_HIS_TREATMENT Treatment = new V_HIS_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_TREATMENT>(Treatment, this.treatment);
                V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = null;
                // get patient_type_alter 
                MOS.Filter.HisPatientTypeAlterViewFilter patientTypeAlterViewFilter = new HisPatientTypeAlterViewFilter();
                patientTypeAlterViewFilter.TREATMENT_ID = Treatment.ID;
                var patientTypeAlters = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientTypeAlterViewFilter, param);

                if (patientTypeAlters != null && patientTypeAlters.Count > 0)
                {
                    patientTypeAlter = patientTypeAlters.OrderByDescending(o => o.LOG_TIME).FirstOrDefault();
                }

                //danh sách tất cả dịch vụ hoàn ứng.
                //trường hợp hoàn ứng 2 lần thì lần 2 sẽ ko có dịch vụ của lần 1
                MOS.Filter.HisSeseDepoRepayFilter hisSeseDepoRepayFilter = new MOS.Filter.HisSeseDepoRepayFilter();
                hisSeseDepoRepayFilter.TDL_TREATMENT_ID = Treatment.ID;
                hisSeseDepoRepayFilter.IS_CANCEL = false;
                var dereDetails = new BackendAdapter(param).Get<List<HIS_SESE_DEPO_REPAY>>("api/HisSeseDepoRepay/Get", ApiConsumers.MosConsumer, hisSeseDepoRepayFilter, param).ToList();

                MOS.Filter.HisSereServDepositFilter sereServDepositFilter = new HisSereServDepositFilter();
                sereServDepositFilter.TDL_TREATMENT_ID = Treatment.ID;
                sereServDepositFilter.IS_CANCEL = false;
                var sereServDeposits = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumer.ApiConsumers.MosConsumer, sereServDepositFilter, param);

                HisTransactionViewFilter tranFilter = new HisTransactionViewFilter();
                tranFilter.TREATMENT_ID = Treatment.ID;
                tranFilter.IS_CANCEL = false;
                List<V_HIS_TRANSACTION> listTransaction = new BackendAdapter(param).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumer.ApiConsumers.MosConsumer, tranFilter, param);

                HisSereServFilter ssFilter = new HisSereServFilter();
                ssFilter.TREATMENT_ID = Treatment.ID;
                List<HIS_SERE_SERV> listSereServ = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumer.ApiConsumers.MosConsumer, ssFilter, param);

                //tạm ứng dv 1, 2, 3, 4
                //hoàn ứng lần 1: hư dv 1 in ra còn dv 2, 3, 4
                //hoàn ưng lần 2: hư dv 2 in ra còn dv 3, 4
                //in hoàn ứng lần 1 vẫn hiển thị dv 2, 3, 4

                //lấy danh sách các phiếu tạm ứng bị hoàn ứng tương ứng với giao dịch đang in
                var repayDepo = sereServDeposits.Where(o => dereDetails.Exists(e => e.SERE_SERV_DEPOSIT_ID == o.ID && this.transactionPrint.ID == e.REPAY_ID)).ToList();
                var groupDeposit = repayDepo.GroupBy(o => o.DEPOSIT_ID).ToList();
                foreach (var item in groupDeposit)
                {
                    //lấy các giao dịch hoàn ứng tạo trước giao dịch hiện tại
                    List<V_HIS_TRANSACTION> listTranRepay = listTransaction.Where(o => o.TRANSACTION_TYPE_ID == this.transactionPrint.TRANSACTION_TYPE_ID && o.TRANSACTION_TIME <= this.transactionPrint.TRANSACTION_TIME).ToList();
                    List<HIS_SESE_DEPO_REPAY> ssRepay = dereDetails.Where(o => listTranRepay.Exists(e => e.ID == o.REPAY_ID)).ToList();

                    var ssDepositCheck = sereServDeposits.Where(o => !ssRepay.Exists(e => e.SERE_SERV_DEPOSIT_ID == o.ID) && o.DEPOSIT_ID == item.Key).ToList();
                    var ssDeposit = sereServDeposits.Where(o => o.DEPOSIT_ID == item.Key).ToList();
                    var tranDeposit = listTransaction.FirstOrDefault(o => o.ID == item.Key) ?? new V_HIS_TRANSACTION();

                    if (ssDepositCheck == null || ssDepositCheck.Count <= 0)
                    {
                        continue;
                    }

                    MPS.Processor.Mps000430.PDO.Mps000430PDO pdo430 = new MPS.Processor.Mps000430.PDO.Mps000430PDO(
                        Treatment,
                        patientTypeAlter,
                        ssDeposit,
                        ssRepay,
                        null,
                        new List<V_HIS_TRANSACTION> { this.transactionPrint, tranDeposit },
                        listSereServ,
                        BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>(),
                        BackendDataWorker.Get<V_HIS_ROOM>()
                        );

                    WaitingManager.Hide();
                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((Treatment != null ? Treatment.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                    WaitingManager.Hide();
                    if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo430, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                    }
                    else
                    {
                        MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo430, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void MPS000430()
        {
            var printType = BackendDataWorker.Get<SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == MPS.Processor.Mps000430.PDO.Mps000430PDO.printTypeCode);
            if (printType != null && printType.IS_ACTIVE == 1)
            {
                Inventec.Common.RichEditor.RichEditorStore richStore = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

                richStore.RunPrintTemplate(MPS.Processor.Mps000430.PDO.Mps000430PDO.printTypeCode, this.DelegatePrint430);
            }
        }

        private void InHuyTamUng(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (transactionPrint == null) return;
                if (!transactionPrint.TREATMENT_ID.HasValue) return;

                V_HIS_PATIENT patient = new V_HIS_PATIENT();
                CommonParam param = new CommonParam();
                HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = transactionPrint.TDL_PATIENT_ID ?? 0;
                var patients = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientFilter, param);
                if (patients != null && patients.Count > 0)
                {
                    patient = patients.FirstOrDefault();
                }

                V_HIS_TREATMENT_FEE treatmentFee = new V_HIS_TREATMENT_FEE();
                HisTreatmentFeeViewFilter feeFilter = new HisTreatmentFeeViewFilter();
                feeFilter.ID = transactionPrint.TREATMENT_ID ?? 0;
                var fees = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, feeFilter, param);
                if (fees != null && fees.Count > 0)
                {
                    treatmentFee = fees.FirstOrDefault();
                }

                MPS.Processor.Mps000381.PDO.Mps000381PDO rdo = new MPS.Processor.Mps000381.PDO.Mps000381PDO(transactionPrint, patient, treatmentFee);
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.transactionPrint != null ? this.transactionPrint.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                }
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

        private void Mps000439_BienBanDieuChinhThongTinHanhChinhHoaDon__(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.transactionPrint == null || !transactionPrint.TREATMENT_ID.HasValue)
                    return;
                WaitingManager.Show();
                HisTransactionViewFilter tranFilter = new HisTransactionViewFilter();
                tranFilter.ID = this.transactionPrint.ID;
                V_HIS_TRANSACTION vTran = null;
                var hisTranSaction = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, tranFilter, null);
                if (hisTranSaction == null)
                {
                    throw new Exception(" Không lấy được giá trị  " + this.transactionPrint.ID);
                }
                vTran = hisTranSaction.FirstOrDefault();
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }
                MPS.Processor.Mps000439.PDO.Mps000439PDO rdo = new MPS.Processor.Mps000439.PDO.Mps000439PDO(vTran);

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.treatment != null ? this.treatment.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);


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

                throw;
            }
        }

        private void Mps000440_BienBanDieuChinhTangGiamTrenHoaDon__(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.transactionPrint == null || !transactionPrint.TREATMENT_ID.HasValue)
                    return;
                WaitingManager.Show();
                HisTransactionViewFilter tranFilter = new HisTransactionViewFilter();
                tranFilter.ID = this.transactionPrint.ID;
                V_HIS_TRANSACTION vTran = null;
                var hisTranSaction = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, tranFilter, null);
                if (hisTranSaction == null)
                {
                    throw new Exception(" Không lấy được giá trị  " + this.transactionPrint.ID);
                }
                vTran = hisTranSaction.FirstOrDefault();
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }
                MPS.Processor.Mps000440.PDO.Mps000440PDO rdo = new MPS.Processor.Mps000440.PDO.Mps000440PDO(vTran);

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.treatment != null ? this.treatment.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

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

                throw;
            }
        }

        private void InPhieuThuTTChiTietDichVuNgoaiTru(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.transactionPrint == null)
                    return;
                WaitingManager.Show();
                var listSereServ = new List<V_HIS_SERE_SERV>();

                HisSereServBillViewFilter ssBillFilter = new HisSereServBillViewFilter();
                ssBillFilter.BILL_ID = this.transactionPrint.ID;
                var hisSSBills = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_BILL>>("api/HisSereServBill/GetView", ApiConsumers.MosConsumer, ssBillFilter, null);
                if (hisSSBills == null || hisSSBills.Count <= 0)
                {
                    throw new Exception("Khong lay duoc SereServBill theo BillId: " + this.transactionPrint.ID);
                }

                if (Print106Type != "1")
                {
                    HisSereServViewFilter ssFilter = new HisSereServViewFilter();
                    ssFilter.TREATMENT_ID = this.transactionPrint.TREATMENT_ID;
                    var listSereServApi = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, ssFilter, null);
                    if (listSereServApi != null && listSereServApi.Count > 0 && hisSSBills != null && hisSSBills.Count > 0)
                    {
                        listSereServ = listSereServApi.Where(o => hisSSBills.Select(p => p.SERE_SERV_ID).Contains(o.ID)).ToList();
                    }

                    if (Print106Type_Expend == "1")
                    {
                        if (listSereServ != null && listSereServ.Count > 0)
                        {
                            HisSereServViewFilter ssFilter1 = new HisSereServViewFilter();
                            ssFilter1.TREATMENT_ID = this.transactionPrint.TREATMENT_ID;
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
                    ssFilter.TREATMENT_ID = this.transactionPrint.TREATMENT_ID;
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
                ptAlterAppFilter.TreatmentId = this.transactionPrint.TREATMENT_ID ?? 0;
                ptAlterAppFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, ptAlterAppFilter, null);

                // tính mức hưởng của thẻ
                string levelCode = HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT;
                string ratio_text = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentPatientTypeAlter.HEIN_CARD_NUMBER, currentPatientTypeAlter.LEVEL_CODE, currentPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100) + "";

                if (listSereServ != null && listSereServ.Count > 0)
                {
                    decimal totalDeposit = GetDepositAmount(this.transactionPrint.TREATMENT_ID);
                    HIS_TREATMENT treatment = GetTreatment(this.transactionPrint.TREATMENT_ID);

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

                    MPS.Processor.Mps000259.PDO.Mps000259PDO rdo = new MPS.Processor.Mps000259.PDO.Mps000259PDO(this.transactionPrint, listSereServ, hisSSBills, treatment, totalDeposit, 0, currentPatientTypeAlter, patient, ratio_text, ado, BackendDataWorker.Get<HIS_DEPARTMENT>());
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
            finally
            {
                WaitingManager.Hide();
            }
        }

        private void InPhieuThuTTChiTietDichVu(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.transactionPrint == null)
                    return;
                WaitingManager.Show();
                var listSereServ = new List<V_HIS_SERE_SERV>();
                HisSereServBillViewFilter ssBillFilter = new HisSereServBillViewFilter();
                ssBillFilter.BILL_ID = this.transactionPrint.ID;
                var hisSSBills = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_BILL>>("api/HisSereServBill/GetView", ApiConsumers.MosConsumer, ssBillFilter, null);

                HisBillGoodsFilter goodFilter = new HisBillGoodsFilter();
                goodFilter.BILL_ID = this.transactionPrint.ID;
                List<HIS_BILL_GOODS> billGoods = new BackendAdapter(new CommonParam()).Get<List<HIS_BILL_GOODS>>("api/HisBillGoods/Get", ApiConsumers.MosConsumer, goodFilter, null);
                if ((hisSSBills == null || hisSSBills.Count <= 0) && (billGoods == null || billGoods.Count <= 0))
                {
                    throw new Exception("Khong lay duoc SereServBill va billGoods theo BillId: " + this.transactionPrint.ID);
                }

                if (Print106Type != "1")
                {
                    HisSereServViewFilter ssFilter = new HisSereServViewFilter();
                    ssFilter.TREATMENT_ID = this.transactionPrint.TREATMENT_ID;
                    var listSereServApi = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, ssFilter, null);
                    if (listSereServApi != null && listSereServApi.Count > 0 && hisSSBills != null && hisSSBills.Count > 0)
                    {
                        listSereServ = listSereServApi.Where(o => hisSSBills.Select(p => p.SERE_SERV_ID).Contains(o.ID)).ToList();
                    }

                    if (Print106Type_Expend == "1")
                    {
                        if (listSereServ != null && listSereServ.Count > 0)
                        {
                            HisSereServViewFilter ssFilter1 = new HisSereServViewFilter();
                            ssFilter1.TREATMENT_ID = this.transactionPrint.TREATMENT_ID;
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
                    ssFilter.TREATMENT_ID = this.transactionPrint.TREATMENT_ID;
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
                    ptAlterAppFilter.TreatmentId = this.transactionPrint.TREATMENT_ID ?? 0;
                    ptAlterAppFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                    var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, ptAlterAppFilter, null);

                    // tính mức hưởng của thẻ
                    string levelCode = HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT;
                    string ratio_text = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentPatientTypeAlter.HEIN_CARD_NUMBER, currentPatientTypeAlter.LEVEL_CODE, currentPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100) + "";
                    V_HIS_PATIENT patient = new V_HIS_PATIENT();
                    if (this.transactionPrint.TDL_PATIENT_ID.HasValue)
                    {
                        HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                        patientFilter.ID = this.transactionPrint.TDL_PATIENT_ID;
                        var patients = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, null);

                        if (patients != null && patients.Count > 0)
                        {
                            patient = patients.FirstOrDefault();
                        }
                    }

                    decimal totalDeposit = GetDepositAmount(this.transactionPrint.TREATMENT_ID);
                    HIS_TREATMENT treatment = GetTreatment(this.transactionPrint.TREATMENT_ID);
                    MPS.Processor.Mps000106.PDO.Mps000106ADO ado = new MPS.Processor.Mps000106.PDO.Mps000106ADO();
                    ado.PatientTypeBHYT = HisConfigCFG.PatientTypeId__BHYT;
                    ado.PatientTypeVP = HisConfigCFG.PatientTypeId__VP;

                    MPS.Processor.Mps000106.PDO.Mps000106PDO rdo = new MPS.Processor.Mps000106.PDO.Mps000106PDO(this.transactionPrint, listSereServ, hisSSBills, billGoods, treatment, totalDeposit, 0, currentPatientTypeAlter, patient, ratio_text, ado, BackendDataWorker.Get<HIS_DEPARTMENT>());
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
            finally
            {
                WaitingManager.Hide();
            }
        }

        private void InPhieuMps373(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.transactionPrint == null)
                    return;
                WaitingManager.Show();
                var listSereServ = new List<V_HIS_SERE_SERV>();
                HisSereServBillViewFilter ssBillFilter = new HisSereServBillViewFilter();
                ssBillFilter.BILL_ID = this.transactionPrint.ID;
                var hisSSBills = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_BILL>>("api/HisSereServBill/GetView", ApiConsumers.MosConsumer, ssBillFilter, null);

                HisBillGoodsFilter goodFilter = new HisBillGoodsFilter();
                goodFilter.BILL_ID = this.transactionPrint.ID;
                List<HIS_BILL_GOODS> billGoods = new BackendAdapter(new CommonParam()).Get<List<HIS_BILL_GOODS>>("api/HisBillGoods/Get", ApiConsumers.MosConsumer, goodFilter, null);
                if ((hisSSBills == null || hisSSBills.Count <= 0) && (billGoods == null || billGoods.Count <= 0))
                {
                    throw new Exception("Khong lay duoc SereServBill va billGoods theo BillId: " + this.transactionPrint.ID);
                }

                if (Print106Type != "1")
                {
                    HisSereServViewFilter ssFilter = new HisSereServViewFilter();
                    ssFilter.TREATMENT_ID = this.transactionPrint.TREATMENT_ID;
                    var listSereServApi = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, ssFilter, null);
                    if (listSereServApi != null && listSereServApi.Count > 0 && hisSSBills != null && hisSSBills.Count > 0)
                    {
                        listSereServ = listSereServApi.Where(o => hisSSBills.Select(p => p.SERE_SERV_ID).Contains(o.ID)).ToList();
                    }

                    if (Print106Type_Expend == "1")
                    {
                        if (listSereServ != null && listSereServ.Count > 0)
                        {
                            HisSereServViewFilter ssFilter1 = new HisSereServViewFilter();
                            ssFilter1.TREATMENT_ID = this.transactionPrint.TREATMENT_ID;
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
                    ssFilter.TREATMENT_ID = this.transactionPrint.TREATMENT_ID;
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
                    ptAlterAppFilter.TreatmentId = this.transactionPrint.TREATMENT_ID ?? 0;
                    ptAlterAppFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                    var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, ptAlterAppFilter, null);

                    // tính mức hưởng của thẻ
                    string levelCode = HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT;
                    string ratio_text = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentPatientTypeAlter.HEIN_CARD_NUMBER, currentPatientTypeAlter.LEVEL_CODE, currentPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100) + "";
                    V_HIS_PATIENT patient = new V_HIS_PATIENT();
                    if (this.transactionPrint.TDL_PATIENT_ID.HasValue)
                    {
                        HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                        patientFilter.ID = this.transactionPrint.TDL_PATIENT_ID;
                        var patients = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, null);

                        if (patients != null && patients.Count > 0)
                        {
                            patient = patients.FirstOrDefault();
                        }
                    }

                    decimal totalDeposit = GetDepositAmount(this.transactionPrint.TREATMENT_ID);
                    HIS_TREATMENT treatment = GetTreatment(this.transactionPrint.TREATMENT_ID);
                    MPS.Processor.Mps000373.PDO.Mps000373ADO ado = new MPS.Processor.Mps000373.PDO.Mps000373ADO();
                    ado.PatientTypeBHYT = HisConfigCFG.PatientTypeId__BHYT;
                    ado.PatientTypeVP = HisConfigCFG.PatientTypeId__VP;

                    MPS.Processor.Mps000373.PDO.Mps000373PDO rdo = new MPS.Processor.Mps000373.PDO.Mps000373PDO(this.transactionPrint, listSereServ, hisSSBills, billGoods, treatment, totalDeposit, 0, currentPatientTypeAlter, patient, ratio_text, ado, BackendDataWorker.Get<HIS_DEPARTMENT>());
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
            finally
            {
                WaitingManager.Hide();
            }
        }

        private void InPhieuThuThanhToan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.transactionPrint == null)
                    return;
                if (!this.transactionPrint.TREATMENT_ID.HasValue)
                {
                    MessageManager.Show(Base.ResourceMessageLang.HoaDonThanhToanXuatBanThuocVatTu);
                    return;
                }
                if (this.transactionPrint.IS_CANCEL == 1)
                {
                    MessageManager.Show(Base.ResourceMessageLang.GiaoDichDaBiHuy);
                    return;
                }
                WaitingManager.Show();

                HisBillFundFilter billFundFilter = new HisBillFundFilter();
                billFundFilter.BILL_ID = this.transactionPrint.ID;
                var listBillFund = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_BILL_FUND>>("api/HisBillFund/Get", ApiConsumers.MosConsumer, billFundFilter, null);

                HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                ssBillFilter.BILL_ID = this.transactionPrint.ID;
                var hisSSBills = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, null);
                if (hisSSBills == null || hisSSBills.Count <= 0)
                {
                    throw new Exception("Khong lay duoc SSBill theo billId: " + this.transactionPrint.BILL_TYPE_ID);
                }

                List<HIS_SERE_SERV> listSereServ = new List<HIS_SERE_SERV>();
                HisSereServFilter ssFilter = new HisSereServFilter();
                ssFilter.TREATMENT_ID = this.transactionPrint.TREATMENT_ID.Value;
                List<HIS_SERE_SERV> listSereServApi = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, null);

                if (listSereServApi != null && listSereServApi.Count > 0 && hisSSBills != null && hisSSBills.Count > 0)
                {
                    listSereServ = listSereServApi.Where(o => hisSSBills.Select(p => p.SERE_SERV_ID).Contains(o.ID)).ToList();
                }

                HisPatientTypeAlterViewAppliedFilter patyAlterAppliedFilter = new HisPatientTypeAlterViewAppliedFilter();
                patyAlterAppliedFilter.InstructionTime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                patyAlterAppliedFilter.TreatmentId = this.transactionPrint.TREATMENT_ID.Value;
                var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, patyAlterAppliedFilter, null);


                HisDepartmentTranLastFilter departLastFilter = new HisDepartmentTranLastFilter();
                departLastFilter.TREATMENT_ID = this.transactionPrint.TREATMENT_ID.Value;
                departLastFilter.BEFORE_LOG_TIME = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                var departmentTran = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, departLastFilter, null);

                V_HIS_PATIENT patient = new V_HIS_PATIENT();
                if (transactionPrint.TDL_PATIENT_ID != null)
                {
                    HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                    patientFilter.ID = this.transactionPrint.TDL_PATIENT_ID;
                    var patients = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, null);

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

                // Lay thong tin cac dich vu da tam ung khong bi huy
                List<HIS_SERE_SERV_DEPOSIT> listSereServDeposit = new List<HIS_SERE_SERV_DEPOSIT>();
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisSereServDepositFilter sereServDepositFilter = new HisSereServDepositFilter();
                sereServDepositFilter.TDL_TREATMENT_ID = transactionPrint.TREATMENT_ID;
                sereServDepositFilter.IS_CANCEL = false;
                listSereServDeposit = new BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumer.ApiConsumers.MosConsumer, sereServDepositFilter, paramCommon);

                List<HIS_SESE_DEPO_REPAY> listSeseDepoRepay = new List<HIS_SESE_DEPO_REPAY>();
                MOS.Filter.HisSeseDepoRepayFilter filterSeseDepoRepay = new MOS.Filter.HisSeseDepoRepayFilter();
                filterSeseDepoRepay.TDL_TREATMENT_ID = transactionPrint.TREATMENT_ID;
                filterSeseDepoRepay.IS_CANCEL = false;
                listSeseDepoRepay = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_SESE_DEPO_REPAY>>("api/HisSeseDepoRepay/Get", ApiConsumer.ApiConsumers.MosConsumer, filterSeseDepoRepay, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);

                HisTransactionViewFilter depositFilter = new HisTransactionViewFilter();
                depositFilter.TREATMENT_ID = this.transactionPrint.TREATMENT_ID.Value;
                var lstTran = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, depositFilter, null);

                LogSystem.Debug("dich vu da hoan ung bang " + listSeseDepoRepay.Count.ToString());
                List<HIS_SERE_SERV_DEPOSIT> finalListSereServDeposit = new List<HIS_SERE_SERV_DEPOSIT>();

                if (listSeseDepoRepay != null && listSeseDepoRepay.Count > 0)
                {
                    finalListSereServDeposit = listSereServDeposit.Where(o => listSeseDepoRepay.All(k => k.SERE_SERV_DEPOSIT_ID != o.ID)).ToList();
                }
                else
                {
                    finalListSereServDeposit = listSereServDeposit;
                }

                MPS.Processor.Mps000111.PDO.Mps000111PDO pdo = new MPS.Processor.Mps000111.PDO.Mps000111PDO(transactionPrint,
                    patient,
                    listBillFund,
                    listSereServ,
                    departmentTran,
                    currentPatientTypeAlter,
                    GetId(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT")),
                    null,
                    finalListSereServDeposit,
                    lstTran,
                    listSeseDepoRepay
                    );

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
            finally
            {
                WaitingManager.Hide();
            }
        }

        private void InBienLaiThanhToanHaiSo(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.transactionPrint == null)
                    return;
                if (!this.transactionPrint.TREATMENT_ID.HasValue)
                {
                    MessageManager.Show(Base.ResourceMessageLang.HoaDonThanhToanXuatBanThuocVatTu);
                    return;
                }
                if (this.transactionPrint.IS_CANCEL == 1)
                {
                    MessageManager.Show(Base.ResourceMessageLang.GiaoDichDaBiHuy);
                    return;
                }
                HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                ssBillFilter.BILL_ID = transactionPrint.ID;
                var listSSBill = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, null);
                if (listSSBill == null || listSSBill.Count <= 0)
                {
                    throw new Exception("Khong lay duoc SSBill theo billId: " + transactionPrint.ID);
                }
                List<HIS_SERE_SERV> listSereServ = new List<HIS_SERE_SERV>();
                HisSereServFilter ssFilter = new HisSereServFilter();
                ssFilter.TREATMENT_ID = this.transactionPrint.TREATMENT_ID.Value;
                List<HIS_SERE_SERV> listSereServApi = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, null);

                if (listSereServApi != null && listSereServApi.Count > 0 && listSSBill != null && listSSBill.Count > 0)
                {
                    listSereServ = listSereServApi.Where(o => listSSBill.Select(p => p.SERE_SERV_ID).Contains(o.ID)).ToList();
                }

                if (listSereServ == null || listSereServ.Count == 0)
                {
                    throw new NullReferenceException("Khong lay duoc SereServ theo resultRecieptBill.ID" + LogUtil.TraceData("transactionPrint", transactionPrint));
                }

                MPS.Processor.Mps000148.PDO.Mps000148PDO rdo = new MPS.Processor.Mps000148.PDO.Mps000148PDO(transactionPrint, listSSBill, listSereServ, HisConfigCFG.PatientTypeId__BHYT);

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
                if (this.transactionPrint == null)
                    return;
                if (!this.transactionPrint.TREATMENT_ID.HasValue)
                {
                    MessageManager.Show(Base.ResourceMessageLang.HoaDonThanhToanXuatBanThuocVatTu);
                    return;
                }
                if (this.transactionPrint.IS_CANCEL == 1)
                {
                    MessageManager.Show(Base.ResourceMessageLang.GiaoDichDaBiHuy);
                    return;
                }
                MPS.Processor.Mps000147.PDO.Mps000147PDO rdo = new MPS.Processor.Mps000147.PDO.Mps000147PDO(transactionPrint);
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
                if (this.transactionPrint == null)
                    return;
                if (!this.transactionPrint.TREATMENT_ID.HasValue)
                {
                    MessageManager.Show(Base.ResourceMessageLang.HoaDonThanhToanXuatBanThuocVatTu);
                    return;
                }
                if (this.transactionPrint.IS_CANCEL == 1)
                {
                    MessageManager.Show(Base.ResourceMessageLang.GiaoDichDaBiHuy);
                    return;
                }
                MPS.Processor.Mps000318.PDO.Mps000318PDO rdo = new MPS.Processor.Mps000318.PDO.Mps000318PDO(transactionPrint);
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
                if (this.transactionPrint == null)
                    return;
                if (!this.transactionPrint.TREATMENT_ID.HasValue)
                {
                    MessageManager.Show(Base.ResourceMessageLang.HoaDonThanhToanXuatBanThuocVatTu);
                    return;
                }
                if (this.transactionPrint.IS_CANCEL == 1)
                {
                    MessageManager.Show(Base.ResourceMessageLang.GiaoDichDaBiHuy);
                    return;
                }
                MPS.Processor.Mps000317.PDO.Mps000317PDO rdo = new MPS.Processor.Mps000317.PDO.Mps000317PDO(transactionPrint);
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
                if (this.transactionPrint == null)
                    return;
                if (this.transactionPrint.SALE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SALE_TYPE.ID__SALE_EXP)
                {
                    MessageManager.Show(Base.ResourceMessageLang.HoaDonThanhToanXuatBanThuocVatTu);
                    return;
                }
                if (this.transactionPrint.IS_CANCEL == 1)
                {
                    MessageManager.Show(Base.ResourceMessageLang.GiaoDichDaBiHuy);
                    return;
                }
                WaitingManager.Show();
                HisBillGoodsFilter billGoodFilter = new HisBillGoodsFilter();
                billGoodFilter.BILL_ID = this.transactionPrint.ID;
                var listBillGoods = new BackendAdapter(new CommonParam()).Get<List<HIS_BILL_GOODS>>("api/HisBillGoods/Get", ApiConsumers.MosConsumer, billGoodFilter, null);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((transactionPrint != null ? transactionPrint.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                MPS.Processor.Mps000299.PDO.Mps000299PDO pdo = new MPS.Processor.Mps000299.PDO.Mps000299PDO(
                    this.transactionPrint,
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
            finally
            {
                WaitingManager.Hide();
            }
        }

        private static long GetId(string code)
        {
            long result = 0;
            try
            {
                var data = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == code);
                result = data.ID;
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
                result = 0;
            }
            return result;
        }

        private void InPhieuThuTamUng(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.transactionPrint == null || !this.transactionPrint.TREATMENT_ID.HasValue)
                    return;
                if (this.transactionPrint.IS_CANCEL == 1)
                {
                    if (this.transactionPrint.CREATOR != this.loginName && this.transactionPrint.CANCEL_LOGINNAME != this.loginName && !CheckLoginAdmin.IsAdmin(this.loginName))
                    {
                        MessageManager.Show("Bạn không có quyền in giao dịch đã hủy");
                        return;
                    }
                }
                WaitingManager.Show();
                V_HIS_PATIENT patient = null;

                HisTransactionViewFilter depositFilter = new HisTransactionViewFilter();
                depositFilter.ID = this.transactionPrint.ID;
                var listDeposit = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, depositFilter, null);
                if (listDeposit == null || listDeposit.Count != 1)
                {
                    throw new Exception("Khong lay duoc V_HIS_DEPOSIT theo transactionId, TransactionCode:" + transactionPrint.TRANSACTION_CODE);
                }

                var deposit = listDeposit.First();

                if (this.transactionPrint.TDL_PATIENT_ID.HasValue)
                {
                    HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                    patientFilter.ID = this.transactionPrint.TDL_PATIENT_ID;
                    var listPatient = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, patientFilter, null);
                    patient = listPatient.First();
                }

                decimal ratio = 0;
                var PatyAlterBhyt = new V_HIS_PATIENT_TYPE_ALTER();
                PrintGlobalStore.LoadCurrentPatientTypeAlter(this.transactionPrint.TREATMENT_ID.Value, 0, ref PatyAlterBhyt);
                if (PatyAlterBhyt != null && !String.IsNullOrEmpty(PatyAlterBhyt.HEIN_CARD_NUMBER))
                {
                    ratio = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(PatyAlterBhyt.HEIN_TREATMENT_TYPE_CODE, PatyAlterBhyt.HEIN_CARD_NUMBER, PatyAlterBhyt.LEVEL_CODE, PatyAlterBhyt.RIGHT_ROUTE_CODE) ?? 0;
                }

                HisDepartmentTranViewFilter departLastFilter = new HisDepartmentTranViewFilter();
                departLastFilter.TREATMENT_ID = this.transactionPrint.TREATMENT_ID.Value;
                var departmentTrans = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/GetView", ApiConsumers.MosConsumer, departLastFilter, null);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((deposit != null ? deposit.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                MPS.Processor.Mps000112.PDO.Mps000112ADO ado = new MPS.Processor.Mps000112.PDO.Mps000112ADO();

                HisTransactionFilter depositCountFilter = new HisTransactionFilter();
                depositCountFilter.TREATMENT_ID = this.transactionPrint.TREATMENT_ID;
                depositCountFilter.TRANSACTION_TIME_TO = this.transactionPrint.TRANSACTION_TIME;
                var deposits = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_TRANSACTION>>("api/HisTransaction/Get", ApiConsumers.MosConsumer, depositCountFilter, null);
                if (deposits != null && deposits.Count > 0)
                {
                    ado.DEPOSIT_NUM_ORDER = deposits.Where(o => o.IS_CANCEL != 1 && o.IS_DELETE == 0 && o.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU).Count();
                    ado.DEPOSIT_SERVICE_NUM_ORDER = deposits.Where(o => o.TDL_SERE_SERV_DEPOSIT_COUNT != null && o.IS_CANCEL != 1 && o.IS_DELETE == 0).Count().ToString();
                }

                V_HIS_TREATMENT treatment = null;
                HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                filter.ID = this.transactionPrint.TREATMENT_ID;
                var treatmentList = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, null);
                if (treatmentList != null && treatmentList.Count > 0)
                    treatment = treatmentList.First();
                MPS.Processor.Mps000112.PDO.Mps000112PDO rdo =
                    new MPS.Processor.Mps000112.PDO.Mps000112PDO(deposit, null, ratio, PatyAlterBhyt, departmentTrans, ado, treatment, BackendDataWorker.Get<HIS_TREATMENT_TYPE>());
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
            finally
            {
                WaitingManager.Hide();
            }
        }

        private void InPhieuThuTamUngDichVu(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.transactionPrint == null || !transactionPrint.TREATMENT_ID.HasValue)
                    return;
                if (this.transactionPrint.IS_CANCEL == 1)
                {
                    MessageManager.Show(Base.ResourceMessageLang.GiaoDichDaBiHuy);
                    return;
                }
                WaitingManager.Show();
                HisTransactionView5Filter depositFilter = new HisTransactionView5Filter();
                depositFilter.ID = this.transactionPrint.ID;
                var listDeposit = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION_5>>("api/HisTransaction/GetView5", ApiConsumers.MosConsumer, depositFilter, null);
                if (listDeposit == null || listDeposit.Count != 1)
                {
                    throw new Exception("Khong lay duoc V_HIS_DEPOSIT theo transactionId:" + this.transactionPrint.ID);
                }

                MPS.Processor.Mps000109.PDO.PatientADO mpsPatientADO = new MPS.Processor.Mps000109.PDO.PatientADO();
                var mpsPatyAlterBhytADO = new V_HIS_PATIENT_TYPE_ALTER();
                PrintGlobalStore.LoadCurrentPatientTypeAlter(this.transactionPrint.TREATMENT_ID.Value, 0, ref mpsPatyAlterBhytADO);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.treatment != null ? this.treatment.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                MPS.Processor.Mps000109.PDO.Mps000109PDO rdo = new MPS.Processor.Mps000109.PDO.Mps000109PDO(
                    mpsPatientADO,
                    mpsPatyAlterBhytADO,
                    listDeposit.FirstOrDefault(),
                    this.treatment
                    );

                MPS.ProcessorBase.Core.PrintData printData = null;
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
            finally
            {
                WaitingManager.Hide();
            }
        }

        private void InPhieuChiTietTamUngDichVu(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.transactionPrint == null || !transactionPrint.TREATMENT_ID.HasValue)
                    return;
                if (this.transactionPrint.IS_CANCEL == 1)
                {
                    if (this.transactionPrint.CREATOR != this.loginName && this.transactionPrint.CANCEL_LOGINNAME != this.loginName && !CheckLoginAdmin.IsAdmin(this.loginName))
                    {
                        MessageManager.Show("Bạn không có quyền in giao dịch đã hủy");
                        return;
                    }
                }
                WaitingManager.Show();

                MOS.Filter.HisSereServDepositFilter dereDetailFiter = new MOS.Filter.HisSereServDepositFilter();
                dereDetailFiter.DEPOSIT_ID = this.transactionPrint.ID;
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

                List<V_HIS_SERE_SERV_12> sereServs = new List<V_HIS_SERE_SERV_12>();
                int skip = 0;
                while (sereServIds.Count - skip > 0)
                {
                    var listIds = sereServIds.Skip(skip).Take(100).ToList();
                    skip += 100;

                    MOS.Filter.HisSereServView12Filter sereServFilter = new MOS.Filter.HisSereServView12Filter();
                    sereServFilter.IDs = listIds;
                    var lstSs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_12>>("api/HisSereServ/GetView12", ApiConsumers.MosConsumer, sereServFilter, null);
                    if (lstSs != null && lstSs.Count > 0)
                    {
                        sereServs.AddRange(lstSs);
                    }
                }

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
                if (this.transactionPrint.TDL_PATIENT_ID.HasValue)
                {
                    MOS.Filter.HisPatientViewFilter patientViewFilter = new HisPatientViewFilter();
                    patientViewFilter.ID = this.transactionPrint.TDL_PATIENT_ID;
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
                patientTypeFilter.TREATMENT_ID = this.transactionPrint.TREATMENT_ID.Value;
                patientTypeFilter.PATIENT_TYPE_ID = sereServs.FirstOrDefault().PATIENT_TYPE_ID;
                var patientTypeAlters = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeALter/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientTypeFilter, param);

                if (patientTypeAlters != null && patientTypeAlters.Count > 0)
                {
                    patyAlterBhyt = patientTypeAlters.OrderByDescending(o => o.LOG_TIME).FirstOrDefault();
                }

                HisDepartmentTranViewFilter filter = new HisDepartmentTranViewFilter();
                filter.TREATMENT_ID = this.transactionPrint.TREATMENT_ID;
                List<V_HIS_DEPARTMENT_TRAN> departmentTrans = new BackendAdapter(new CommonParam()).Get<List<V_HIS_DEPARTMENT_TRAN>>("api/HisDepartMentTran/GetView", ApiConsumers.MosConsumer, filter, null);

                long? totalDay = null;
                HisTreatmentFeeViewFilter filterTreatmentFee = new HisTreatmentFeeViewFilter();
                filterTreatmentFee.ID = this.transactionPrint.TREATMENT_ID.Value;
                var listTreatment = new BackendAdapter(param)
                  .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, filterTreatmentFee, param);
                if (listTreatment != null && listTreatment.Count > 0)
                {
                    this.treatment = listTreatment.FirstOrDefault();
                }

                if (this.treatment != null && this.treatment.TDL_PATIENT_TYPE_ID == Config.HisConfigCFG.PatientTypeId__BHYT)
                {
                    totalDay = HIS.Common.Treatment.Calculation.DayOfTreatment(this.treatment.IN_TIME, this.treatment.OUT_TIME, this.treatment.TREATMENT_END_TYPE_ID, this.treatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT);
                }
                else if (this.treatment != null)
                {
                    totalDay = HIS.Common.Treatment.Calculation.DayOfTreatment(this.treatment.IN_TIME, this.treatment.OUT_TIME, this.treatment.TREATMENT_END_TYPE_ID, this.treatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI);
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
                var serviceReports = new BackendAdapter(new CommonParam()).Get<List<HIS_HEIN_SERVICE_TYPE>>(HisRequestUriStore.HIS_HEIN_SERVICE_TYPE_GET, ApiConsumers.MosConsumer, HeinServiceTypefilter, null);
                WaitingManager.Hide();

                V_HIS_PATIENT_TYPE_ALTER currentHisPatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                PrintGlobalStore.LoadCurrentPatientTypeAlter(this.transactionPrint.TREATMENT_ID.Value, 0, ref currentHisPatientTypeAlter);
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
                        this.treatment,

                        serviceReports,
                        this.transactionPrint,
                        sereServDeposits,
                        totalDay,
                        ratio_text,
                        FirstExamRoom(this.transactionPrint.TREATMENT_ID.Value)
                        );
                WaitingManager.Hide();

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.treatment != null ? this.treatment.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                MPS.ProcessorBase.Core.PrintData PrintData = null;
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
            finally
            {
                WaitingManager.Hide();
            }
        }

        private void InPhieuThuHoanUng(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.transactionPrint == null || !transactionPrint.TREATMENT_ID.HasValue)
                    return;
                if (this.transactionPrint.IS_CANCEL == 1)
                {
                    if (this.transactionPrint.CREATOR != this.loginName && this.transactionPrint.CANCEL_LOGINNAME != this.loginName && !CheckLoginAdmin.IsAdmin(this.loginName))
                    {
                        MessageManager.Show("Bạn không có quyền in giao dịch đã hủy");
                        return;
                    }
                }
                WaitingManager.Show();

                //BỎ repay
                HisTransactionViewFilter repayFilter = new HisTransactionViewFilter();
                repayFilter.ID = this.transactionPrint.ID;
                V_HIS_TRANSACTION repay = null;
                var listRepay = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, repayFilter, null);
                if (listRepay == null || listRepay.Count != 1)
                {
                    throw new Exception("Khong lay duoc V_HIS_REPAY theo transactionId: " + this.transactionPrint.ID);
                }
                repay = listRepay.FirstOrDefault();
                V_HIS_PATIENT patient = null;

                if (this.transactionPrint.TDL_PATIENT_ID.HasValue)
                {
                    HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                    patientFilter.ID = this.transactionPrint.TDL_PATIENT_ID;
                    var listPatient = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, patientFilter, null);
                    patient = listPatient.First();
                }

                decimal ratio = 0;
                var PatyAlterBhyt = new V_HIS_PATIENT_TYPE_ALTER();
                PrintGlobalStore.LoadCurrentPatientTypeAlter(this.transactionPrint.TREATMENT_ID.Value, 0, ref PatyAlterBhyt);
                if (PatyAlterBhyt != null && !String.IsNullOrEmpty(PatyAlterBhyt.HEIN_CARD_NUMBER))
                {
                    ratio = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(PatyAlterBhyt.HEIN_TREATMENT_TYPE_CODE, PatyAlterBhyt.HEIN_CARD_NUMBER, PatyAlterBhyt.LEVEL_CODE, PatyAlterBhyt.RIGHT_ROUTE_CODE) ?? 0;
                }

                HisDepartmentTranLastFilter departLastFilter = new HisDepartmentTranLastFilter();
                departLastFilter.TREATMENT_ID = this.transactionPrint.TREATMENT_ID.Value;
                departLastFilter.BEFORE_LOG_TIME = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                var departmentTran = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, departLastFilter, null);

                CommonParam paramtreatment = new CommonParam();
                HisTreatmentFeeViewFilter filterTreat = new HisTreatmentFeeViewFilter();
                filterTreat.ID = this.transactionPrint.TREATMENT_ID;
                var TreatmentFee = new Inventec.Common.Adapter.BackendAdapter(paramtreatment).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, filterTreat, paramtreatment);

                HisTransactionViewFilter filterTran = new HisTransactionViewFilter();
                filterTran.TREATMENT_ID = transactionPrint.TREATMENT_ID;
                //filterTran.TRANSACTION_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT };
                filterTran.IS_CANCEL = false;
                List<V_HIS_TRANSACTION> transa = new BackendAdapter(paramtreatment).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, filterTran, paramtreatment);
                if (transa == null) transa = new List<V_HIS_TRANSACTION>();

                MPS.Processor.Mps000113.PDO.Mps000113PDO rdo = new MPS.Processor.Mps000113.PDO.Mps000113PDO(
                    repay,
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

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((repay != null ? repay.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

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
            finally
            {
                WaitingManager.Hide();
            }
        }

        private void InHoaDonTTTheoYeuCauDichVu(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.transactionPrint == null)
                    return;

                WaitingManager.Show();

                HisPatientTypeAlterViewAppliedFilter ptAlterAppFilter = new HisPatientTypeAlterViewAppliedFilter();
                ptAlterAppFilter.TreatmentId = this.transactionPrint.TREATMENT_ID ?? 0;
                ptAlterAppFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, ptAlterAppFilter, null);

                // tính mức hưởng của thẻ
                string levelCode = HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT;
                string ratio_text = currentPatientTypeAlter != null ? ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentPatientTypeAlter.HEIN_CARD_NUMBER, currentPatientTypeAlter.LEVEL_CODE, currentPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100) + "" : "";
                V_HIS_PATIENT patient = new V_HIS_PATIENT();
                if (this.transactionPrint.TDL_PATIENT_ID.HasValue)
                {
                    HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                    patientFilter.ID = this.transactionPrint.TDL_PATIENT_ID;
                    var patients = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, null);

                    if (patients != null && patients.Count > 0)
                    {
                        patient = patients.FirstOrDefault();
                    }
                }

                HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                ssBillFilter.BILL_ID = this.transactionPrint.ID;
                var hisSSBills = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, null);
                if (hisSSBills == null || hisSSBills.Count <= 0)
                {
                    throw new Exception("Khong lay duoc SSBill theo billId: " + this.transactionPrint.BILL_TYPE_ID);
                }

                List<V_HIS_SERE_SERV> listSereServ = new List<V_HIS_SERE_SERV>();
                HisSereServViewFilter sereServFilter = new HisSereServViewFilter();
                sereServFilter.TREATMENT_ID = transactionPrint.TREATMENT_ID.Value;
                List<V_HIS_SERE_SERV> listSereServApi = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, sereServFilter, null);

                if (listSereServApi != null && listSereServApi.Count > 0 && hisSSBills != null && hisSSBills.Count > 0)
                {
                    listSereServ = listSereServApi.Where(o => hisSSBills.Select(p => p.SERE_SERV_ID).Contains(o.ID)).ToList();
                }
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
                        MPS.Processor.Mps000103.PDO.Mps000103PDO pdo = new MPS.Processor.Mps000103.PDO.Mps000103PDO(patient, this.transactionPrint, listSub, currentPatientTypeAlter, ratio_text);
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
            finally
            {
                WaitingManager.Hide();
            }
        }

        private void InBienLaiThuPhiLePhi(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.transactionPrint == null)
                    return;

                WaitingManager.Show();
                V_HIS_PATIENT patient = null;

                if (this.transactionPrint.TDL_PATIENT_ID.HasValue)
                {
                    HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                    patientFilter.ID = this.transactionPrint.TDL_PATIENT_ID;
                    var listPatient = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, patientFilter, null);
                    if (listPatient != null && listPatient.Count == 1)
                    {
                        patient = listPatient.First();
                    }
                }

                HisPatientTypeAlterViewAppliedFilter ptAlterAppFilter = new HisPatientTypeAlterViewAppliedFilter();
                ptAlterAppFilter.TreatmentId = this.transactionPrint.TREATMENT_ID ?? 0;
                ptAlterAppFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, ptAlterAppFilter, null);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.transactionPrint != null ? this.transactionPrint.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                MPS.Processor.Mps000114.PDO.Mps000114PDO rdo = new MPS.Processor.Mps000114.PDO.Mps000114PDO(this.transactionPrint, patient, 0, currentPatientTypeAlter);
                MPS.ProcessorBase.Core.PrintData printData = null;
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
            finally
            {
                WaitingManager.Hide();
            }
        }

        private void InPhieuChiDinhTheoGiaoDichThanhToan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.transactionPrint == null)
                    return;
                if (!this.transactionPrint.TREATMENT_ID.HasValue)
                {
                    MessageManager.Show(Base.ResourceMessageLang.HoaDonThanhToanXuatBanThuocVatTu);
                    return;
                }

                WaitingManager.Show();

                HisPatientTypeAlterViewAppliedFilter ptAlterAppFilter = new HisPatientTypeAlterViewAppliedFilter();
                ptAlterAppFilter.TreatmentId = this.transactionPrint.TREATMENT_ID.Value;
                ptAlterAppFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, ptAlterAppFilter, null);

                // tính mức hưởng của thẻ
                string levelCode = HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT;
                string ratio_text = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentPatientTypeAlter.HEIN_CARD_NUMBER, currentPatientTypeAlter.LEVEL_CODE, currentPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100) + "";
                V_HIS_PATIENT patient = new V_HIS_PATIENT();
                if (this.transactionPrint.TDL_PATIENT_ID.HasValue)
                {
                    HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                    patientFilter.ID = this.transactionPrint.TDL_PATIENT_ID;
                    var patients = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, null);

                    if (patients != null && patients.Count > 0)
                    {
                        patient = patients.FirstOrDefault();
                    }
                }

                HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                ssBillFilter.BILL_ID = this.transactionPrint.ID;
                var hisSSBills = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, null);
                if (hisSSBills == null || hisSSBills.Count <= 0)
                {
                    throw new Exception("Khong lay duoc SSBill theo billId: " + this.transactionPrint.BILL_TYPE_ID);
                }
                List<V_HIS_SERE_SERV> listSereServ = new List<V_HIS_SERE_SERV>();
                HisSereServViewFilter sereServFilter = new HisSereServViewFilter();
                sereServFilter.TREATMENT_ID = this.transactionPrint.TREATMENT_ID;

                var listSereServApi = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, sereServFilter, null);
                if (listSereServApi != null && listSereServApi.Count > 0 && hisSSBills != null && hisSSBills.Count > 0)
                {
                    listSereServ = listSereServApi.Where(o => hisSSBills.Select(p => p.SERE_SERV_ID).Contains(o.ID)).ToList();
                }
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
                        MPS.Processor.Mps000105.PDO.Mps000105PDO rdo = new MPS.Processor.Mps000105.PDO.Mps000105PDO(this.transactionPrint, listSub, currentPatientTypeAlter, serviceReq, patient, ratio_text);

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
            finally
            {
                WaitingManager.Hide();
            }
        }

        private void InPhieuHoanUngDichVu(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.transactionPrint == null || !this.transactionPrint.TREATMENT_ID.HasValue)
                    return;
                if (this.transactionPrint.IS_CANCEL == 1)
                {
                    if (this.transactionPrint.CREATOR != this.loginName && this.transactionPrint.CANCEL_LOGINNAME != this.loginName && !CheckLoginAdmin.IsAdmin(this.loginName))
                    {
                        MessageManager.Show("Bạn không có quyền in giao dịch đã hủy");
                        return;
                    }
                }
                WaitingManager.Show();

                var patyAlterBhytADO = new V_HIS_PATIENT_TYPE_ALTER();
                PrintGlobalStore.LoadCurrentPatientTypeAlter(this.transactionPrint.TREATMENT_ID.Value, 0, ref patyAlterBhytADO);

                //bỏ rebay
                HisTransactionViewFilter repayFilter = new HisTransactionViewFilter();
                repayFilter.ID = this.transactionPrint.ID;
                var listRepay = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, repayFilter, null);
                if (listRepay == null || listRepay.Count != 1)
                {
                    throw new Exception("Khong lay duoc V_HIS_REPAY theo transactionId: " + this.transactionPrint.ID);
                }
                var repay = listRepay.FirstOrDefault();

                MOS.Filter.HisSeseDepoRepayFilter hisSeseDepoRepayFilter = new MOS.Filter.HisSeseDepoRepayFilter();
                hisSeseDepoRepayFilter.REPAY_ID = repay.ID;
                var seseDepoRepays = new BackendAdapter(new CommonParam()).Get<List<HIS_SESE_DEPO_REPAY>>(RequestUri.HIS_SESE_DEPO_REPAY_GET, ApiConsumers.MosConsumer, hisSeseDepoRepayFilter, null).ToList();

                List<HIS_SERE_SERV_DEPOSIT> lstSsDeposit = new List<HIS_SERE_SERV_DEPOSIT>();
                List<V_HIS_TRANSACTION> lstTran = new List<V_HIS_TRANSACTION>();
                if (seseDepoRepays != null && seseDepoRepays.Count > 0)
                {
                    ProcessGetDepositBySsdIds(seseDepoRepays.Select(s => s.SERE_SERV_DEPOSIT_ID).Distinct().ToList(), ref lstSsDeposit, lstTran);
                }

                List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans = new BackendAdapter(new CommonParam()).Get<List<V_HIS_DEPARTMENT_TRAN>>(HisRequestUriStore.HIS_DEPARTMENT_TRAN_GETHOSPITALINOUT, ApiConsumers.MosConsumer, this.transactionPrint.TREATMENT_ID, null);

                if (this.treatment == null || this.treatment.ID != this.transactionPrint.TREATMENT_ID.Value)
                {
                    CommonParam param = new CommonParam();
                    HisTreatmentFeeViewFilter filterTreatmentFee = new HisTreatmentFeeViewFilter();
                    filterTreatmentFee.ID = this.transactionPrint.TREATMENT_ID.Value;
                    var listTreatment = new BackendAdapter(param)
                      .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, filterTreatmentFee, param);
                    if (listTreatment != null && listTreatment.Count > 0)
                    {
                        this.treatment = listTreatment.FirstOrDefault();
                    }
                }

                long? totalDay = null;

                if (this.treatment != null && this.treatment.TDL_PATIENT_TYPE_ID == Config.HisConfigCFG.PatientTypeId__BHYT)
                {
                    totalDay = HIS.Common.Treatment.Calculation.DayOfTreatment(this.treatment.IN_TIME, this.treatment.OUT_TIME, this.treatment.TREATMENT_END_TYPE_ID, this.treatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.BHYT);
                }
                else
                {
                    totalDay = HIS.Common.Treatment.Calculation.DayOfTreatment(this.treatment.IN_TIME, this.treatment.OUT_TIME, this.treatment.TREATMENT_END_TYPE_ID, this.treatment.TREATMENT_RESULT_ID, HIS.Common.Treatment.PatientTypeEnum.TYPE.THU_PHI);
                }
                string departmentName = "";

                MPS.Processor.Mps000110.PDO.PatientADO mpsPatientADO = new MPS.Processor.Mps000110.PDO.PatientADO();

                HisDepartmentTranLastFilter departLastFilter = new HisDepartmentTranLastFilter();
                departLastFilter.TREATMENT_ID = this.transactionPrint.TREATMENT_ID.Value;
                departLastFilter.BEFORE_LOG_TIME = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                var departmentTran = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, departLastFilter, null);
                AutoMapper.Mapper.CreateMap<V_HIS_TREATMENT_FEE, V_HIS_TREATMENT_1>();
                var treatments = AutoMapper.Mapper.Map<V_HIS_TREATMENT_FEE, V_HIS_TREATMENT_1>(this.treatment);
                MPS.Processor.Mps000110.PDO.Mps000110PDO pdo = new MPS.Processor.Mps000110.PDO.Mps000110PDO(mpsPatientADO,
                    patyAlterBhytADO,
                    departmentName,
                    seseDepoRepays,
                    departmentTrans,
                    treatments,
                    repay,
                    totalDay,
                    departmentTran,
                    lstSsDeposit,
                    lstTran);
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
                    MPS000430();
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                    MPS000430();
                }
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

        private void ProcessGetDepositBySsdIds(List<long> listIds, ref List<HIS_SERE_SERV_DEPOSIT> lstSsDeposit, List<V_HIS_TRANSACTION> lstTran)
        {
            try
            {
                if (listIds != null && listIds.Count > 0)
                {
                    if (lstSsDeposit == null)
                        lstSsDeposit = new List<HIS_SERE_SERV_DEPOSIT>();
                    if (lstTran == null)
                        lstTran = new List<V_HIS_TRANSACTION>();

                    int skip = 0;
                    while (listIds.Count - skip > 0)
                    {
                        var ids = listIds.Skip(skip).Take(100).ToList();
                        skip += 100;
                        CommonParam param = new CommonParam();
                        HisSereServDepositFilter deFilter = new HisSereServDepositFilter();
                        deFilter.IDs = ids;
                        var ssdApiresult = new BackendAdapter(param).Get<List<HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumers.MosConsumer, deFilter, param);
                        if (ssdApiresult != null && ssdApiresult.Count > 0)
                        {
                            lstSsDeposit.AddRange(ssdApiresult);
                        }
                    }

                    List<long> transDeposit = lstSsDeposit.Select(s => s.DEPOSIT_ID).Distinct().ToList();

                    skip = 0;
                    while (transDeposit.Count - skip > 0)
                    {
                        var ids = transDeposit.Skip(skip).Take(100).ToList();
                        skip += 100;
                        CommonParam param = new CommonParam();
                        HisTransactionViewFilter tranFilter = new HisTransactionViewFilter();
                        tranFilter.IDs = ids;
                        var tranApiresult = new BackendAdapter(param).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, tranFilter, param);
                        if (tranApiresult != null && tranApiresult.Count > 0)
                        {
                            lstTran.AddRange(tranApiresult);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuTamUngVaGiuThe(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.transactionPrint == null || !this.transactionPrint.TREATMENT_ID.HasValue)
                    return;
                if (this.transactionPrint.IS_CANCEL == 1)
                {
                    MessageManager.Show(Base.ResourceMessageLang.GiaoDichDaBiHuy);
                    return;
                }
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                HisTransactionViewFilter depositFilter = new HisTransactionViewFilter();
                depositFilter.ID = this.transactionPrint.ID;
                var listDeposit = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, depositFilter, null);
                if (listDeposit == null || listDeposit.Count != 1)
                {
                    throw new Exception("Khong lay duoc V_HIS_DEPOSIT theo transactionId: " + this.transactionPrint.ID);
                }

                var deposit = listDeposit.First();

                HisPatientTypeAlterViewAppliedFilter appFilter = new HisPatientTypeAlterViewAppliedFilter();
                long nowTime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                if (this.treatment.IN_TIME > nowTime)
                {
                    appFilter.InstructionTime = this.treatment.IN_TIME;
                }
                else
                {
                    appFilter.InstructionTime = nowTime;
                }
                appFilter.TreatmentId = this.transactionPrint.TREATMENT_ID.Value;
                var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(param).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, appFilter, null);
                if (currentPatientTypeAlter == null)
                {
                    throw new Exception("Khong lay duoc thong tin doi tuong cua ho so dieu tri Id: " + this.transactionPrint.TREATMENT_ID);
                }

                decimal ratio = 0;
                if (currentPatientTypeAlter != null)
                {
                    ratio = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentPatientTypeAlter.HEIN_CARD_NUMBER, currentPatientTypeAlter.LEVEL_CODE, currentPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0;
                }

                HisDepartmentTranLastFilter departLastFilter = new HisDepartmentTranLastFilter();
                departLastFilter.TREATMENT_ID = this.transactionPrint.TREATMENT_ID.Value;

                if (this.treatment.IN_TIME > nowTime)
                {
                    departLastFilter.BEFORE_LOG_TIME = this.treatment.IN_TIME;
                }
                else
                {
                    departLastFilter.BEFORE_LOG_TIME = nowTime;
                }
                var departmentTran = new Inventec.Common.Adapter.BackendAdapter(param).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, departLastFilter, param);

                HIS_PATIENT patient = new HIS_PATIENT();
                HisPatientFilter filter = new HisPatientFilter();
                filter.ID = this.treatment.PATIENT_ID;
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
                if (this.transactionPrint == null || !this.transactionPrint.TREATMENT_ID.HasValue)
                    return;
                if (this.transactionPrint.IS_CANCEL == 1)
                {
                    MessageManager.Show(Base.ResourceMessageLang.GiaoDichDaBiHuy);
                    return;
                }
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                HisTransactionViewFilter depositFilter = new HisTransactionViewFilter();
                depositFilter.ID = this.transactionPrint.ID;
                var listDeposit = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, depositFilter, null);
                if (listDeposit == null || listDeposit.Count != 1)
                {
                    throw new Exception("Khong lay duoc V_HIS_DEPOSIT theo transactionId: " + this.transactionPrint.ID);
                }

                var deposit = listDeposit.First();

                HisPatientTypeAlterViewAppliedFilter appFilter = new HisPatientTypeAlterViewAppliedFilter();
                long nowTime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                if (this.treatment.IN_TIME > nowTime)
                {
                    appFilter.InstructionTime = this.treatment.IN_TIME;
                }
                else
                {
                    appFilter.InstructionTime = nowTime;
                }
                appFilter.TreatmentId = this.transactionPrint.TREATMENT_ID.Value;
                var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(param).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, appFilter, null);
                if (currentPatientTypeAlter == null)
                {
                    throw new Exception("Khong lay duoc thong tin doi tuong cua ho so dieu tri Id: " + this.transactionPrint.TREATMENT_ID);
                }

                HisDepartmentTranViewFilter departLastFilter = new HisDepartmentTranViewFilter();
                departLastFilter.TREATMENT_ID = this.transactionPrint.TREATMENT_ID ?? 0;
                departLastFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var departmentTrans = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/GetView", ApiConsumers.MosConsumer, departLastFilter, param);

                decimal ratio = 0;
                if (currentPatientTypeAlter != null)
                {
                    ratio = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentPatientTypeAlter.HEIN_CARD_NUMBER, currentPatientTypeAlter.LEVEL_CODE, currentPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0;
                }

                HIS_PATIENT patient = new HIS_PATIENT();
                HisPatientFilter filter = new HisPatientFilter();
                filter.ID = this.treatment.PATIENT_ID;
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
                if (transactionPrint == null) return;
                if (!transactionPrint.TREATMENT_ID.HasValue)
                {
                    MessageManager.Show(Base.ResourceMessageLang.HoaDonThanhToanXuatBanThuocVatTu);
                    return;
                }

                MPS.Processor.Mps000337.PDO.Mps000337PDO rdo = new MPS.Processor.Mps000337.PDO.Mps000337PDO(transactionPrint);

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
                    Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>(sereServADO, item);

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
                if (transactionPrint == null) return;
                WaitingManager.Show();

                CommonParam param = new CommonParam();
                HisBillGoodsFilter goodsFilter = new HisBillGoodsFilter();
                goodsFilter.BILL_ID = this.transactionPrint.ID;
                List<HIS_BILL_GOODS> billGoods = new BackendAdapter(param).Get<List<HIS_BILL_GOODS>>("api/HisBillGoods/Get", ApiConsumers.MosConsumer, goodsFilter, param);

                HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                expMestFilter.BILL_ID = this.transactionPrint.ID;
                List<V_HIS_EXP_MEST> expMests = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, expMestFilter, param);

                HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                expMestMedicineFilter.EXP_MEST_IDs = expMests.Select(s => s.ID).ToList();
                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = new BackendAdapter(param)
                    .Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetVIew", ApiConsumers.MosConsumer, expMestMedicineFilter, param);

                HisExpMestMaterialViewFilter expMestMaterialFilter = new HisExpMestMaterialViewFilter();
                expMestMaterialFilter.EXP_MEST_IDs = expMests.Select(s => s.ID).ToList();
                List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials = new BackendAdapter(param)
                    .Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetVIew", ApiConsumers.MosConsumer, expMestMaterialFilter, param);

                HisExpMestFilter hisexpmestFilter = new HisExpMestFilter();
                hisexpmestFilter.BILL_ID = this.transactionPrint.ID;
                List<V_HIS_EXP_MEST> hisexpmest = new BackendAdapter(param)
                    .Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetVIew", ApiConsumers.MosConsumer, hisexpmestFilter, param);

                HisImpMestFilter hisimpmestFilter = new HisImpMestFilter();
                hisimpmestFilter.MOBA_EXP_MEST_IDs = hisexpmest.Select(o => o.ID).ToList();
                List<V_HIS_IMP_MEST> hisimpmest = new BackendAdapter(param)
                    .Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetVIew", ApiConsumers.MosConsumer, hisimpmestFilter, param);

                MPS.Processor.Mps000339.PDO.Mps000339PDO rdo = new MPS.Processor.Mps000339.PDO.Mps000339PDO(transactionPrint, billGoods, expMestMedicines, expMestMaterials, hisexpmest, hisimpmest);

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

        private void InThuCongNo(string printTransaction, string fileName, ref bool result)
        {
            try
            {
                if (this.transactionPrint == null)
                    return;
                WaitingManager.Show();

                HisTransactionFilter transactionFilter = new HisTransactionFilter();
                transactionFilter.DEBT_BILL_ID = transactionPrint.ID;
                CommonParam param = new CommonParam();
                var transaction = new BackendAdapter(param).Get<List<HIS_TRANSACTION>>("api/HisTransaction/Get", ApiConsumers.MosConsumer, transactionFilter, param);
                if (transaction == null || transaction.Count <= 0)
                {
                    throw new Exception("Khong lay duoc transaction theo TRANSACTION_ID: " + this.transactionPrint.ID);
                }

                List<HIS_SERE_SERV> listSereServ = new List<HIS_SERE_SERV>();
                HisSereServDebtViewFilter ssDebtFilter = new HisSereServDebtViewFilter();
                ssDebtFilter.DEBT_IDs = transaction.Select(s => s.ID).ToList();
                var hisSSDebts = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_DEBT>>("api/HisSereServDebt/GetView", ApiConsumers.MosConsumer, ssDebtFilter, null);
                if (hisSSDebts != null || hisSSDebts.Count > 0)
                {
                    HisSereServFilter ssFilter = new HisSereServFilter();
                    ssFilter.IDs = hisSSDebts.Select(s => s.SERE_SERV_ID).ToList();
                    listSereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, null);
                }

                List<HIS_DEBT_GOODS> listDebtGood = new List<HIS_DEBT_GOODS>();

                HisDebtGoodsFilter debtGoodFilter = new HisDebtGoodsFilter();
                debtGoodFilter.DEBT_IDs = transaction.Select(s => s.ID).ToList();
                listDebtGood = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_DEBT_GOODS>>("api/HisDebtGoods/Get", ApiConsumers.MosConsumer, debtGoodFilter, null);

                HisPatientTypeAlterViewAppliedFilter patyAlterAppliedFilter = new HisPatientTypeAlterViewAppliedFilter();
                patyAlterAppliedFilter.InstructionTime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                patyAlterAppliedFilter.TreatmentId = this.transactionPrint.TREATMENT_ID.Value;
                var patientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, patyAlterAppliedFilter, null);

                HisDepartmentTranLastFilter departLastFilter = new HisDepartmentTranLastFilter();
                departLastFilter.TREATMENT_ID = this.transactionPrint.TREATMENT_ID.Value;
                departLastFilter.BEFORE_LOG_TIME = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                var departmentTran = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, departLastFilter, null);

                HIS_PATIENT patient = new HIS_PATIENT();
                if (this.transactionPrint.TDL_PATIENT_ID.HasValue)
                {
                    HisPatientFilter filter = new HisPatientFilter();
                    filter.ID = this.transactionPrint.TDL_PATIENT_ID;
                    var patients = new BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                    if (patients != null && patients.Count > 0)
                    {
                        patient = patients.First();
                    }
                }

                MPS.Processor.Mps000370.PDO.Mps000370PDO pdo = new MPS.Processor.Mps000370.PDO.Mps000370PDO(
                    transactionPrint,
                    patient,
                    listSereServ,
                    departmentTran,
                    patientTypeAlter,
                    hisSSDebts,
                    BackendDataWorker.Get<HIS_SERVICE_TYPE>(),
                    listDebtGood
                    );

                MPS.ProcessorBase.Core.PrintData printData = null;

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTransaction))
                {
                    printerName = GlobalVariables.dicPrinter[printTransaction];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((transactionPrint != null ? transactionPrint.TREATMENT_CODE : ""), printTransaction, currentModule != null ? currentModule.RoomId : 0);
                WaitingManager.Hide();
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTransaction, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTransaction, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuXacNhanCongNo(string printTransaction, string fileName, ref bool result)
        {
            try
            {
                if (this.transactionPrint == null)
                    return;
                WaitingManager.Show();

                HisSereServDebtViewFilter ssDebtFilter = new HisSereServDebtViewFilter();
                ssDebtFilter.DEBT_ID = this.transactionPrint.ID;
                var hisSSDebts = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_DEBT>>("api/HisSereServDebt/GetView", ApiConsumers.MosConsumer, ssDebtFilter, null);
                if (hisSSDebts == null || hisSSDebts.Count <= 0)
                {
                    throw new Exception("Khong lay duoc hisSSDebts theo DEBT_ID: " + this.transactionPrint.ID);
                }
                List<HIS_SERE_SERV> listSereServ = new List<HIS_SERE_SERV>();
                HisSereServFilter ssFilter = new HisSereServFilter();

                ssFilter.TREATMENT_ID = this.transactionPrint.TREATMENT_ID;
                var listSereServApi = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, null);

                if (listSereServApi != null && listSereServApi.Count > 0 && hisSSDebts != null && hisSSDebts.Count > 0)
                {
                    listSereServ = listSereServApi.Where(o => hisSSDebts.Select(p => p.SERE_SERV_ID).Contains(o.ID)).ToList();
                }

                HisPatientTypeAlterViewAppliedFilter patyAlterAppliedFilter = new HisPatientTypeAlterViewAppliedFilter();
                patyAlterAppliedFilter.InstructionTime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                patyAlterAppliedFilter.TreatmentId = this.transactionPrint.TREATMENT_ID.Value;
                var patientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, patyAlterAppliedFilter, null);

                HisDepartmentTranLastFilter departLastFilter = new HisDepartmentTranLastFilter();
                departLastFilter.TREATMENT_ID = this.transactionPrint.TREATMENT_ID.Value;
                departLastFilter.BEFORE_LOG_TIME = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                var departmentTran = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, departLastFilter, null);

                HIS_PATIENT patient = new HIS_PATIENT();
                if (this.transactionPrint.TDL_PATIENT_ID.HasValue)
                {
                    HisPatientFilter filter = new HisPatientFilter();
                    filter.ID = this.transactionPrint.TDL_PATIENT_ID;
                    var patients = new BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                    if (patients != null && patients.Count > 0)
                    {
                        patient = patients.First();
                    }
                }

                MPS.Processor.Mps000369.PDO.Mps000369PDO pdo = new MPS.Processor.Mps000369.PDO.Mps000369PDO(
                    transactionPrint,
                    patient,
                    listSereServ,
                    departmentTran,
                    patientTypeAlter,
                    hisSSDebts,
                    BackendDataWorker.Get<HIS_SERVICE_TYPE>()
                    );

                MPS.ProcessorBase.Core.PrintData printData = null;

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTransaction))
                {
                    printerName = GlobalVariables.dicPrinter[printTransaction];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((transactionPrint != null ? transactionPrint.TREATMENT_CODE : ""), printTransaction, currentModule != null ? currentModule.RoomId : 0);
                WaitingManager.Hide();
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTransaction, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTransaction, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InHoaDonBanThuoc(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.transactionPrint == null) return;
                WaitingManager.Show();

                HisExpMestFilter expFilter = new HisExpMestFilter();
                expFilter.BILL_ID = this.transactionPrint.ID;
                List<HIS_EXP_MEST> expMests = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("api/HisEpxMest/Get", ApiConsumers.MosConsumer, expFilter, null);

                List<V_HIS_EXP_MEST_MEDICINE> medicines = null;
                List<V_HIS_EXP_MEST_MATERIAL> materials = null;
                if (expMests != null && expMests.Count > 0)
                {
                    HisExpMestMedicineViewFilter medicineFilter = new HisExpMestMedicineViewFilter();
                    medicineFilter.EXP_MEST_IDs = expMests.Select(s => s.ID).ToList();
                    medicines = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisEpxMestMedicine/GetView", ApiConsumers.MosConsumer, medicineFilter, null);

                    HisExpMestMaterialViewFilter materialFilter = new HisExpMestMaterialViewFilter();
                    materialFilter.EXP_MEST_IDs = expMests.Select(s => s.ID).ToList();
                    materials = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisEpxMestMaterial/GetView", ApiConsumers.MosConsumer, materialFilter, null);
                }

                Mapper.CreateMap<HisTransactionADO, HIS_TRANSACTION>();
                HIS_TRANSACTION bill = Mapper.Map<HIS_TRANSACTION>(this.transactionPrint);

                MPS.Processor.Mps000344.PDO.Mps000344PDO rdo = new MPS.Processor.Mps000344.PDO.Mps000344PDO(bill, expMests, medicines, materials, null);

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

        private void InPhieuThuThanhToanHoaDon(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.transactionPrint == null)
                    return;
                if (this.transactionPrint.SALE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SALE_TYPE.ID__SALE_EXP)
                {
                    MessageManager.Show(Base.ResourceMessageLang.HoaDonThanhToanXuatBanThuocVatTu);
                    return;
                }
                if (this.transactionPrint.IS_CANCEL == 1)
                {
                    MessageManager.Show(Base.ResourceMessageLang.GiaoDichDaBiHuy);
                    return;
                }
                WaitingManager.Show();
                HisBillGoodsFilter billGoodFilter = new HisBillGoodsFilter();
                billGoodFilter.BILL_ID = this.transactionPrint.ID;
                var listBillGoods = new BackendAdapter(new CommonParam()).Get<List<HIS_BILL_GOODS>>("api/HisBillGoods/Get", ApiConsumers.MosConsumer, billGoodFilter, null);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((transactionPrint != null ? transactionPrint.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                MPS.Processor.Mps000355.PDO.Mps000355PDO pdo = new MPS.Processor.Mps000355.PDO.Mps000355PDO(
                    this.transactionPrint,
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

        private bool XuatHoaDonDienTu(V_HIS_TRANSACTION transactionBill, bool isError, ref CommonParam param)
        {
            bool result = false;
            try
            {
                if (transactionBill == null) return result;
                //không phải giao dịch thanh toán hoặc đã tạo hóa đơn điện tử thì bỏ qua.
                if (transactionBill.TRANSACTION_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT || !String.IsNullOrWhiteSpace(transactionBill.INVOICE_CODE))
                {
                    return result;
                }

                if (param == null)
                {
                    param = new CommonParam();
                }

                ElectronicBillResult electronicBillResult = null;
                if (isError)//nếu là lỗi thì lấy thông tin
                {
                    electronicBillResult = GetEbillInfo(transactionBill);
                }

                //Tao hoa don dien thu ben thu3 
                if (electronicBillResult == null || !electronicBillResult.Success)
                {
                    electronicBillResult = TaoHoaDonDienTuBenThu3CungCap(transactionBill);
                }

                if (electronicBillResult == null || !electronicBillResult.Success)
                {
                    param.Messages.Add("Tạo hóa đơn điện tử thất bại");
                    if (electronicBillResult.Messages != null && electronicBillResult.Messages.Count > 0)
                    {
                        param.Messages.AddRange(electronicBillResult.Messages);
                    }

                    param.Messages = param.Messages.Distinct().ToList();
                }
                else
                {
                    //goi api update
                    CommonParam paramUpdate = new CommonParam();
                    HisTransactionInvoiceInfoSDO sdo = new HisTransactionInvoiceInfoSDO();
                    sdo.EinvoiceLoginname = electronicBillResult.InvoiceLoginname;
                    sdo.InvoiceCode = electronicBillResult.InvoiceCode;
                    sdo.InvoiceSys = electronicBillResult.InvoiceSys;
                    sdo.EinvoiceNumOrder = electronicBillResult.InvoiceNumOrder;
                    sdo.EInvoiceTime = electronicBillResult.InvoiceTime ?? 0;
                    sdo.Id = transactionBill.ID;
                    sdo.InvoiceLookupCode = electronicBillResult.InvoiceLookupCode;
                    var apiResult = new BackendAdapter(paramUpdate).Post<bool>("api/HisTransaction/UpdateInvoiceInfo", ApiConsumers.MosConsumer, sdo, paramUpdate);
                    if (apiResult)
                    {
                        transactionBill.INVOICE_CODE = electronicBillResult.InvoiceCode;
                        transactionBill.INVOICE_SYS = electronicBillResult.InvoiceSys;
                        transactionBill.EINVOICE_NUM_ORDER = electronicBillResult.InvoiceNumOrder;
                        transactionBill.EINVOICE_TIME = electronicBillResult.InvoiceTime;
                        transactionBill.EINVOICE_LOGINNAME = electronicBillResult.InvoiceLoginname;
                        transactionBill.INVOICE_LOOKUP_CODE = electronicBillResult.InvoiceLookupCode;
                        result = true;
                    }
                }

                //MessageManager.Show(this, param, success);
                //MessageBox.Show(param.BugCodes.ToString());
                //MessageBox.Show(param.Messages.ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private ElectronicBillResult TaoHoaDonDienTuBenThu3CungCap(V_HIS_TRANSACTION transaction)
        {
            ElectronicBillResult result = new ElectronicBillResult();
            try
            {
                //WaitingManager.Show();
                List<V_HIS_SERE_SERV_5> sereServ5s = null;

                var param = new CommonParam();
                HisSereServBillFilter ssbfilter = new HisSereServBillFilter();
                ssbfilter.BILL_ID = transaction.ID;
                var sereServBill = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssbfilter, param);
                if (sereServBill == null || sereServBill.Count <= 0)
                {
                    if (transaction.SALE_TYPE_ID == 1)
                    {
                        sereServ5s = ProcessSereServByExpMestForEBill(transaction);
                    }
                    else
                    {
                        HisBillGoodsFilter bgfilter = new HisBillGoodsFilter();
                        bgfilter.BILL_ID = transaction.ID;
                        var billgoods = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_BILL_GOODS>>("api/HisBillGoods/Get", ApiConsumers.MosConsumer, ssbfilter, param);
                        if (billgoods != null && billgoods.Count > 0)
                        {
                            sereServ5s = new List<V_HIS_SERE_SERV_5>();
                            int dem = 0;
                            foreach (var item in billgoods)
                            {
                                V_HIS_SERE_SERV_5 ssb = new V_HIS_SERE_SERV_5();

                                ssb.SERVICE_ID = item.NONE_MEDI_SERVICE_ID ?? item.MATERIAL_TYPE_ID ?? item.MEDICINE_TYPE_ID ?? dem;
                                ssb.MEDICINE_ID = item.MEDICINE_TYPE_ID;
                                ssb.MATERIAL_ID = item.MATERIAL_TYPE_ID;
                                ssb.OTHER_PAY_SOURCE_ID = item.NONE_MEDI_SERVICE_ID;

                                ssb.AMOUNT = item.AMOUNT;
                                ssb.VAT_RATIO = item.VAT_RATIO ?? 0;
                                ssb.TDL_SERVICE_CODE = "";
                                ssb.TDL_SERVICE_NAME = item.GOODS_NAME;
                                ssb.SERVICE_UNIT_NAME = item.GOODS_UNIT_NAME;
                                ssb.PRICE = item.PRICE - ((item.DISCOUNT ?? 0) / item.AMOUNT);
                                ssb.VIR_PRICE = item.PRICE - ((item.DISCOUNT ?? 0) / item.AMOUNT);
                                ssb.VIR_TOTAL_PATIENT_PRICE = ssb.VIR_PRICE * (1 + ssb.VAT_RATIO) * ssb.AMOUNT;
                                sereServ5s.Add(ssb);
                                dem++;
                            }
                        }
                    }
                }

                V_HIS_TREATMENT_FEE currentTreatment = new V_HIS_TREATMENT_FEE();
                if (transaction.TREATMENT_ID.HasValue)
                {
                    HisTreatmentFeeViewFilter filter = new HisTreatmentFeeViewFilter();
                    filter.ID = transaction.TREATMENT_ID;
                    var treatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, filter, param);
                    if (treatment != null && treatment.Count > 0)
                    {
                        currentTreatment = treatment.First();
                    }
                }
                else
                {
                    currentTreatment.TDL_PATIENT_ACCOUNT_NUMBER = transaction.BUYER_ACCOUNT_NUMBER;
                    currentTreatment.TDL_PATIENT_ADDRESS = String.IsNullOrWhiteSpace(transaction.BUYER_ADDRESS) ? transaction.TDL_PATIENT_ADDRESS : transaction.BUYER_ADDRESS;
                    currentTreatment.TDL_PATIENT_PHONE = transaction.BUYER_PHONE;
                    currentTreatment.TDL_PATIENT_TAX_CODE = transaction.BUYER_TAX_CODE;
                    currentTreatment.TDL_PATIENT_WORK_PLACE = String.IsNullOrWhiteSpace(transaction.BUYER_ORGANIZATION) ? transaction.TDL_PATIENT_WORK_PLACE : transaction.BUYER_ORGANIZATION;
                    currentTreatment.TDL_PATIENT_NAME = String.IsNullOrWhiteSpace(transaction.BUYER_NAME) ? transaction.TDL_PATIENT_NAME : transaction.BUYER_NAME;
                    currentTreatment.TDL_PATIENT_CODE = transaction.TDL_PATIENT_CODE;
                    currentTreatment.TDL_PATIENT_WORK_PLACE_NAME = transaction.TDL_PATIENT_WORK_PLACE_NAME;
                    currentTreatment.ID = -1;//để các api trong thư viện không lấy được dữ liệu
                    currentTreatment.PATIENT_ID = transaction.TDL_PATIENT_ID ?? -1;
                }

                ElectronicBillDataInput dataInput = new ElectronicBillDataInput();
                dataInput.Amount = transaction.AMOUNT;
                dataInput.Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                dataInput.Discount = transaction.EXEMPTION;
                dataInput.DiscountRatio = Math.Round((transaction.EXEMPTION ?? 0) / transaction.AMOUNT, 2, MidpointRounding.AwayFromZero) * 100;
                dataInput.PaymentMethod = transaction.PAY_FORM_NAME;
                dataInput.SereServs = sereServ5s;
                dataInput.SereServBill = sereServBill;
                dataInput.Treatment = currentTreatment;
                dataInput.Currency = "VND";
                dataInput.SymbolCode = transaction.SYMBOL_CODE;
                dataInput.TemplateCode = transaction.TEMPLATE_CODE;
                dataInput.TransactionTime = transaction.EINVOICE_TIME ?? transaction.TRANSACTION_TIME;
                dataInput.EinvoiceTypeId = transaction.EINVOICE_TYPE_ID;
                dataInput.IsTransactionList = true;

                HIS_TRANSACTION tran = new HIS_TRANSACTION();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(tran, transaction);

                dataInput.Transaction = tran;

                if (transaction.SALE_TYPE_ID == 1)
                {
                    ElectronicBillProcessor electronicBillProcessor = new ElectronicBillProcessor(dataInput, Library.ElectronicBill.Template.TemplateEnum.TYPE.TemplateNhaThuoc);
                    result = electronicBillProcessor.Run(ElectronicBillType.ENUM.CREATE_INVOICE);
                }
                else
                {
                    ElectronicBillProcessor electronicBillProcessor = new ElectronicBillProcessor(dataInput);
                    result = electronicBillProcessor.Run(ElectronicBillType.ENUM.CREATE_INVOICE);
                }
                //WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                result.Success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private ElectronicBillResult GetEbillInfo(V_HIS_TRANSACTION transaction)
        {
            ElectronicBillResult result = new ElectronicBillResult();
            try
            {
                //WaitingManager.Show();
                List<V_HIS_SERE_SERV_5> sereServ5s = null;

                var param = new CommonParam();
                HisSereServBillFilter ssbfilter = new HisSereServBillFilter();
                ssbfilter.BILL_ID = transaction.ID;
                var sereServBill = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssbfilter, param);
                if (sereServBill == null || sereServBill.Count <= 0)
                {
                    if (transaction.SALE_TYPE_ID == 1)
                    {
                        sereServ5s = ProcessSereServByExpMestForEBill(transaction);
                    }
                    else
                    {
                        HisBillGoodsFilter bgfilter = new HisBillGoodsFilter();
                        bgfilter.BILL_ID = transaction.ID;
                        var billgoods = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_BILL_GOODS>>("api/HisBillGoods/Get", ApiConsumers.MosConsumer, ssbfilter, param);
                        if (billgoods != null && billgoods.Count > 0)
                        {
                            sereServ5s = new List<V_HIS_SERE_SERV_5>();
                            int dem = 0;
                            foreach (var item in billgoods)
                            {
                                V_HIS_SERE_SERV_5 ssb = new V_HIS_SERE_SERV_5();

                                ssb.SERVICE_ID = item.NONE_MEDI_SERVICE_ID ?? item.MATERIAL_TYPE_ID ?? item.MEDICINE_TYPE_ID ?? dem;
                                ssb.MEDICINE_ID = item.MEDICINE_TYPE_ID;
                                ssb.MATERIAL_ID = item.MATERIAL_TYPE_ID;
                                ssb.OTHER_PAY_SOURCE_ID = item.NONE_MEDI_SERVICE_ID;

                                ssb.AMOUNT = item.AMOUNT;
                                ssb.VAT_RATIO = item.VAT_RATIO ?? 0;
                                ssb.TDL_SERVICE_CODE = "";
                                ssb.TDL_SERVICE_NAME = item.GOODS_NAME;
                                ssb.SERVICE_UNIT_NAME = item.GOODS_UNIT_NAME;
                                ssb.PRICE = item.PRICE - ((item.DISCOUNT ?? 0) / item.AMOUNT);
                                ssb.VIR_PRICE = item.PRICE - ((item.DISCOUNT ?? 0) / item.AMOUNT);
                                ssb.VIR_TOTAL_PATIENT_PRICE = ssb.VIR_PRICE * (1 + ssb.VAT_RATIO) * ssb.AMOUNT;
                                sereServ5s.Add(ssb);
                                dem++;
                            }
                        }
                    }
                }

                V_HIS_TREATMENT_FEE currentTreatment = new V_HIS_TREATMENT_FEE();
                if (transaction.TREATMENT_ID.HasValue)
                {
                    HisTreatmentFeeViewFilter filter = new HisTreatmentFeeViewFilter();
                    filter.ID = transaction.TREATMENT_ID;
                    var treatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, filter, param);
                    if (treatment != null && treatment.Count > 0)
                    {
                        currentTreatment = treatment.First();
                    }
                }
                else
                {
                    currentTreatment.TDL_PATIENT_ACCOUNT_NUMBER = transaction.BUYER_ACCOUNT_NUMBER;
                    currentTreatment.TDL_PATIENT_ADDRESS = String.IsNullOrWhiteSpace(transaction.BUYER_ADDRESS) ? transaction.TDL_PATIENT_ADDRESS : transaction.BUYER_ADDRESS;
                    currentTreatment.TDL_PATIENT_PHONE = transaction.BUYER_PHONE;
                    currentTreatment.TDL_PATIENT_TAX_CODE = transaction.BUYER_TAX_CODE;
                    currentTreatment.TDL_PATIENT_WORK_PLACE = String.IsNullOrWhiteSpace(transaction.BUYER_ORGANIZATION) ? transaction.TDL_PATIENT_WORK_PLACE : transaction.BUYER_ORGANIZATION;
                    currentTreatment.TDL_PATIENT_NAME = String.IsNullOrWhiteSpace(transaction.BUYER_NAME) ? transaction.TDL_PATIENT_NAME : transaction.BUYER_NAME;
                    currentTreatment.TDL_PATIENT_CODE = transaction.TDL_PATIENT_CODE;
                    currentTreatment.TDL_PATIENT_WORK_PLACE_NAME = transaction.TDL_PATIENT_WORK_PLACE_NAME;
                    currentTreatment.ID = -1;//để các api trong thư viện không lấy được dữ liệu
                    currentTreatment.PATIENT_ID = transaction.TDL_PATIENT_ID ?? -1;
                }

                ElectronicBillDataInput dataInput = new ElectronicBillDataInput();
                //lấy thông tin hóa đơn đã tạo thì sẽ chưa có INVOICE_CODE trong HIS_TRANSACTION. thông tin mã giao dịch đang sử dụng làm key để tạo hóa đơn.
                dataInput.InvoiceCode = transaction.TRANSACTION_CODE;
                dataInput.Amount = transaction.AMOUNT;
                dataInput.Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                dataInput.Discount = transaction.EXEMPTION;
                dataInput.DiscountRatio = Math.Round((transaction.EXEMPTION ?? 0) / transaction.AMOUNT, 2, MidpointRounding.AwayFromZero) * 100;
                dataInput.PaymentMethod = transaction.PAY_FORM_NAME;
                dataInput.SereServs = sereServ5s;
                dataInput.SereServBill = sereServBill;
                dataInput.Treatment = currentTreatment;
                dataInput.Currency = "VND";
                dataInput.SymbolCode = transaction.SYMBOL_CODE;
                dataInput.TemplateCode = transaction.TEMPLATE_CODE;
                dataInput.TransactionTime = transaction.EINVOICE_TIME ?? transaction.TRANSACTION_TIME;
                dataInput.EinvoiceTypeId = transaction.EINVOICE_TYPE_ID;
                dataInput.IsTransactionList = true;

                HIS_TRANSACTION tran = new HIS_TRANSACTION();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(tran, transaction);

                dataInput.Transaction = tran;

                if (transaction.SALE_TYPE_ID == 1)
                {
                    ElectronicBillProcessor electronicBillProcessor = new ElectronicBillProcessor(dataInput, Library.ElectronicBill.Template.TemplateEnum.TYPE.TemplateNhaThuoc);
                    result = electronicBillProcessor.Run(ElectronicBillType.ENUM.GET_INVOICE_INFO);
                }
                else
                {
                    ElectronicBillProcessor electronicBillProcessor = new ElectronicBillProcessor(dataInput);
                    result = electronicBillProcessor.Run(ElectronicBillType.ENUM.GET_INVOICE_INFO);
                }

                if (result != null && result.Success && String.IsNullOrWhiteSpace(result.InvoiceCode))
                {
                    result.InvoiceCode = transaction.TRANSACTION_CODE;
                }
                //WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                result.Success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<V_HIS_SERE_SERV_5> ProcessSereServByExpMestForEBill(V_HIS_TRANSACTION transaction)
        {
            List<V_HIS_SERE_SERV_5> result = null;
            try
            {
                string vatOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.TransactionList.ElectronicBill.VatOption");
                CommonParam param = new CommonParam();
                HisExpMestFilter expfilter = new HisExpMestFilter();
                expfilter.BILL_ID = transaction.ID;
                var listExpMests = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, expfilter, param);
                if (listExpMests != null && listExpMests.Count > 0)
                {
                    result = new List<V_HIS_SERE_SERV_5>();

                    HisExpMestMedicineViewFilter mediFilter = new HisExpMestMedicineViewFilter();
                    mediFilter.EXP_MEST_IDs = listExpMests.Select(s => s.ID).ToList();
                    var listExpMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, mediFilter, param);
                    if (listExpMestMedicines != null && listExpMestMedicines.Count > 0)
                    {
                        foreach (var item in listExpMestMedicines)
                        {
                            V_HIS_SERE_SERV_5 sereServBill = new V_HIS_SERE_SERV_5();

                            sereServBill.AMOUNT = item.AMOUNT;
                            sereServBill.TDL_SERVICE_CODE = item.MEDICINE_TYPE_CODE;
                            sereServBill.TDL_SERVICE_NAME = item.MEDICINE_TYPE_NAME;
                            sereServBill.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                            sereServBill.DISCOUNT = item.DISCOUNT;
                            sereServBill.MEDICINE_ID = item.MEDICINE_TYPE_ID;

                            sereServBill.VAT_RATIO = item.VAT_RATIO ?? 0;
                            if (vatOption == "1")
                            {
                                sereServBill.VAT_RATIO = item.IMP_VAT_RATIO;
                            }
                            else if (vatOption == "2")
                            {
                                sereServBill.VAT_RATIO = 0;
                            }

                            sereServBill.PRICE = (item.VIR_PRICE ?? 0) / (1 + sereServBill.VAT_RATIO);

                            sereServBill.VIR_TOTAL_PATIENT_PRICE = sereServBill.PRICE * sereServBill.AMOUNT;
                            var service = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == item.SERVICE_ID);
                            if (service != null)
                            {
                                sereServBill.TDL_SERVICE_TAX_RATE_TYPE = service.TAX_RATE_TYPE;
                            }

                            result.Add(sereServBill);
                        }
                    }

                    HisExpMestMaterialViewFilter mateFilter = new HisExpMestMaterialViewFilter();
                    mateFilter.EXP_MEST_IDs = listExpMests.Select(s => s.ID).ToList();
                    var listExpMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, mateFilter, param);
                    if (listExpMestMaterials != null && listExpMestMaterials.Count > 0)
                    {
                        foreach (var item in listExpMestMaterials)
                        {
                            V_HIS_SERE_SERV_5 sereServBill = new V_HIS_SERE_SERV_5();

                            sereServBill.AMOUNT = item.AMOUNT;
                            sereServBill.TDL_SERVICE_CODE = item.MATERIAL_TYPE_CODE;
                            sereServBill.TDL_SERVICE_NAME = item.MATERIAL_TYPE_NAME;
                            sereServBill.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                            sereServBill.DISCOUNT = item.DISCOUNT;
                            sereServBill.MATERIAL_ID = item.MATERIAL_TYPE_ID;

                            sereServBill.VAT_RATIO = item.VAT_RATIO ?? 0;
                            if (vatOption == "1")
                            {
                                sereServBill.VAT_RATIO = item.IMP_VAT_RATIO;
                            }
                            else if (vatOption == "2")
                            {
                                sereServBill.VAT_RATIO = 0;
                            }

                            sereServBill.PRICE = (item.VIR_PRICE ?? 0) / (1 + sereServBill.VAT_RATIO);

                            sereServBill.VIR_TOTAL_PATIENT_PRICE = sereServBill.PRICE * sereServBill.AMOUNT;
                            var service = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == item.SERVICE_ID);
                            if (service != null)
                            {
                                sereServBill.TDL_SERVICE_TAX_RATE_TYPE = service.TAX_RATE_TYPE;
                            }

                            result.Add(sereServBill);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}

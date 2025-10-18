using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraExport;
using DevExpress.XtraPrinting.Native;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.ModuleExt;
using HIS.Desktop.Plugins.AdjustmentTransaction.config;
using HIS.Desktop.Plugins.HIS.Desktop.Plugins.AdjustmentTransaction.Base;
using HIS.Desktop.Plugins.Library.ElectronicBill;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Plugins.Library.ElectronicBill.Data;
using HIS.Desktop.Plugins.Library.ElectronicBill.Template;
using HIS.Desktop.Plugins.TransactionBill.ADO;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using IMSys.DbConfig.HIS_RS;
using Inventec.Common.Adapter;
using Inventec.Common.DocumentViewer;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Fss.Client;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using MPS.ADO;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;

namespace HIS.Desktop.Plugins.AdjustmentTransaction.AdjustmentTransaction
{
    public partial class frmAdjustmentTransaction : FormBase
    {
        private string Print106Type = HisConfigs.Get<string>("HIS.Desktop.Print.TransactionDetail");
        private string Print106Type_Expend = HisConfigs.Get<string>("HIS.Desktop.Print.TransactionDetail_Expend");
        List<string> ErrorElectronicBill = new List<string>();
        bool CreatAgain = false;
        bool isPrintNow = false;
        bool isEmr = false;
        bool isnotPrintMPS000111 = false;
        public HisTransactionBillResultSDO TransactionBillResultSDO { get; private set; }
        
        private void SetEnableButtonSave(bool? enable)
        {
            try
            {
                btnSave.Enabled = enable ?? true;
                btnSaveAndSign.Enabled = enable ?? true;
                btnSavePrint.Enabled = enable ?? true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool? ProcessSave(ref CommonParam param, bool isLuuKy)      
         {
            Inventec.Common.Logging.LogSystem.Info("ProcessSave 1.1");
            this.isPrintNow = false;
            bool? success = false;
            try
            {
                TransactionBillResultSDO = null;
                long payFormId = 0;
                var payForm = this.payFormList.FirstOrDefault(o => o.PayFormId == cboPayForm.EditValue);
                if (payForm == null)
                    return success;
                payFormId = payForm.ID;

                var listData = this.ssTreeProcessor.GetListCheck(this.ucSereServTree);
                if (listData == null || listData.Count == 0)
                {
                    param.Messages.Add(HIS.Desktop.Plugins.AdjustmentTransaction.Base.ResourceMessageLang.NguoiDungChuChonDichVuDeThanhToan);
                    return success;
                }

                if (cboPayForm.EditValue == null)
                {
                    param.Messages.Add(HIS.Desktop.Plugins.AdjustmentTransaction.Base.ResourceMessageLang.ThieuTruongDuLieuBatBuoc);
                    return success;
                }

                if (cboAccountBook.EditValue == null)
                {
                    param.Messages.Add(HIS.Desktop.Plugins.AdjustmentTransaction.Base.ResourceMessageLang.ThieuTruongDuLieuBatBuoc);
                    return success;
                }

                if (txtReason.Text.Trim() == null || txtReason.Text.Trim() == "")
                {
                    param.Messages.Add(HIS.Desktop.Plugins.AdjustmentTransaction.Base.ResourceMessageLang.ThieuLyDoDieuChinh);
                    return success;
                }
                if (txtReason.Text.Trim().Length > 1000)
                {
                    param.Messages.Add(HIS.Desktop.Plugins.AdjustmentTransaction.Base.ResourceMessageLang.LyDoQuaKyTu);
                    return success;
                }

                if (dtTransactionTime.EditValue == null)
                {
                    param.Messages.Add(HIS.Desktop.Plugins.AdjustmentTransaction.Base.ResourceMessageLang.ThieuThoiGianGiaoDich);
                    return success;
                }

                HisAdjustmentBillSDO data = new HisAdjustmentBillSDO();
                if (currentTransaction != null)
                {
                    data.OriginalTransactionId = currentTransaction.ID;
                }
                data.RequestRoomId = currentModule.RoomId;
                data.TreatmentId = this.currentTransaction.TREATMENT_ID;

                data.Transaction = new MOS.EFMODEL.DataModels.HIS_TRANSACTION();
                Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_TRANSACTION>(data.Transaction, this.currentTransaction);
                LogSystem.Info("this.currentTransaction: " + LogUtil.TraceData("", this.currentTransaction));
                LogSystem.Info("data.Transaction: " + LogUtil.TraceData("", data.Transaction));
                data.Transaction.ID = 0;
                data.Transaction.INVOICE_CODE = null;
                data.Transaction.INVOICE_SYS = null;
                data.Transaction.EINVOICE_NUM_ORDER = null;
                data.Transaction.EINVOICE_TIME = null;
                data.Transaction.EINVOICE_LOGINNAME = null;
                var accountBook = ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                if (accountBook != null)
                {
                    data.Transaction.ACCOUNT_BOOK_ID = accountBook.ID;
                }

                if (accountBook != null && accountBook.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                {
                    data.Transaction.NUM_ORDER = (long)spinTongTuDen.Value;
                }

                data.Transaction.PAY_FORM_ID = payFormId;
                if (dtTransactionTime.EditValue != null && dtTransactionTime.DateTime != DateTime.MinValue)
                    data.Transaction.TRANSACTION_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtTransactionTime.EditValue).ToString("yyyyMMddHHmmss"));
                if (data.Transaction.EXEMPTION == 0)
                {
                    data.Transaction.EXEMPTION = null;
                }
                data.Transaction.ADJUSTMENT_REASON = txtReason.Text;
                data.Transaction.IS_ADJUSTMENT = 1;

                
                decimal AmountTransaction = 0;
                decimal ToyalPriceGoc = 0;
                List<HIS_SERE_SERV_BILL> hisSSBills = new List<HIS_SERE_SERV_BILL>();
                foreach (var item in listData)  
                {
                    HIS_SERE_SERV_BILL ssBill = new HIS_SERE_SERV_BILL();
                    ssBill.SERE_SERV_ID = item.ID;

                    if (adjustmentValues.TryGetValue(item.ID, out decimal adjustedPrice))
                    {
                        if (adjustedPrice > item.VIR_TOTAL_PATIENT_PRICE || (adjustedPrice * -1) > item.VIR_TOTAL_PATIENT_PRICE)
                        {
                            param.Messages.Add(HIS.Desktop.Plugins.AdjustmentTransaction.Base.ResourceMessageLang.VuotMax);
                            return success;
                        }
                        else if (item.TOTAL_BILL_AMOUNT == 0 && adjustedPrice < 0)
                        {
                            param.Messages.Add(HIS.Desktop.Plugins.AdjustmentTransaction.Base.ResourceMessageLang.VuotMax);
                            return success;
                        }
                    }
                    ssBill.PRICE = adjustedPrice;
                    AmountTransaction += ssBill.PRICE;
                    ToyalPriceGoc += item.TOTAL_BILL_AMOUNT ?? 0;
                    hisSSBills.Add(ssBill);
                }

                if (AmountTransaction == 0 )
                {
                    param.Messages.Add(HIS.Desktop.Plugins.AdjustmentTransaction.Base.ResourceMessageLang.DieuChinh);
                    return success;
                }
                if (AmountTransaction != 0)
                {
                    data.Transaction.AMOUNT = AmountTransaction;
                }
                
                if (data.Transaction.AMOUNT >= 0)
                {
                    data.Transaction.ADJUSTMENT_TYPE = 2; //AMOUNT dương        
                }
                else if (data.Transaction.AMOUNT < 0)
                {
                    data.Transaction.ADJUSTMENT_TYPE = 1; //AMOUNT âm
                }
                else
                {
                    data.Transaction.ADJUSTMENT_TYPE = 3;
                }
                data.SereServBills = hisSSBills;

                #region không tạo hóa đơn điện tử khi tiền bệnh nhân phải trả bằng 0     
                if (isLuuKy && HisConfigCFG.AllowToCreateNoPriceTransaction != "1")
                {
                    var listFund1 = bindingSource1.DataSource as List<VHisBillFundADO>;
                    decimal totalFund1 = 0;
                    decimal canthuAmount = 0;
                    if (listFund1 != null && listFund1.Count > 0)
                    {
                        totalFund1 = listFund1.Sum(o => o.AMOUNT);
                    }
                    string totalAmountBNTra = Inventec.Common.Number.Convert.NumberToString((ToyalPriceGoc - AmountTransaction), ConfigApplications.NumberSeperator);

                    if (totalAmountBNTra == "0")
                    {
                        param.Messages.Add(HIS.Desktop.Plugins.AdjustmentTransaction.Base.ResourceMessageLang.TienBenhNhanTraBangKhong);
                        isnotPrintMPS000111 = true;
                        Inventec.Common.Logging.LogSystem.Info("param123: ");
                    }
                }
                #endregion

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisAdjustmentBillResultSDO>
                    ("api/HisTransaction/AdjustmentBill", ApiConsumers.MosConsumer, data, param);

                if (rs != null)
                {
                    success = true;
                    HisTransactionViewFilter fl = new HisTransactionViewFilter();
                    fl.ID = rs.TransactionBill.ID;
                    var lstTransaction = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumer.ApiConsumers.MosConsumer, fl, null);
                    this.resultTranBill = lstTransaction.FirstOrDefault();

                    if (delegateRefreshData != null)
                    {
                        delegateRefreshData();
                    }

                    if (isLuuKy && AdjustmentTransactionConfig.InvoiceTypeCreate == invoiceTypeCreate__CreateInvoiceVnpt)
                    {
                        if (isnotPrintMPS000111 == false)
                        {
                            MOS.EFMODEL.DataModels.HIS_TRANSACTION tran = new MOS.EFMODEL.DataModels.HIS_TRANSACTION();
                            Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_TRANSACTION>(tran, resultTranBill);
                            //tran.HIS_BILL_FUND = data.Transaction.HIS_BILL_FUND;
                            //Tao hoa don dien thu ben thu3 
                            ElectronicBillResult electronicBillResult = TaoHoaDonDienTuBenThu3CungCap(tran);
                            if (electronicBillResult == null || !electronicBillResult.Success)
                            {
                                CreatAgain = true;

                                ErrorElectronicBill.Add("Tạo hóa đơn điện tử thất bại");
                                if (electronicBillResult.Messages != null && electronicBillResult.Messages.Count > 0)
                                {
                                    ErrorElectronicBill.AddRange(electronicBillResult.Messages.Distinct().ToList());
                                }

                                ErrorElectronicBill.Add("Bạn có muốn phát hành lại hóa đơn điện tử không?");

                                param.Messages.AddRange(ErrorElectronicBill);

                                //MessageManager.Show(this.ParentForm, param, success);
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
                                sdo.EInvoiceTime = electronicBillResult.InvoiceTime ?? (Inventec.Common.DateTime.Get.Now() ?? 0);
                                sdo.Id = resultTranBill.ID;
                                sdo.InvoiceLookupCode = electronicBillResult.InvoiceLookupCode;
                                var apiResult = new BackendAdapter(paramUpdate).Post<bool>("api/HisTransaction/UpdateInvoiceInfo", ApiConsumers.MosConsumer, sdo, paramUpdate);
                                if (apiResult)
                                {
                                    resultTranBill.INVOICE_CODE = electronicBillResult.InvoiceCode;
                                    resultTranBill.INVOICE_SYS = electronicBillResult.InvoiceSys;
                                    resultTranBill.EINVOICE_NUM_ORDER = electronicBillResult.InvoiceNumOrder;
                                    resultTranBill.EINVOICE_TIME = electronicBillResult.InvoiceTime;
                                    resultTranBill.EINVOICE_LOGINNAME = electronicBillResult.InvoiceLoginname;
                                }
                            }
                        }
                    }

                    Inventec.Desktop.Common.Message.WaitingManager.Hide();
                    Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, true);
                }
            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }
        private void RefreshSessionInfo()
        {
            try
            {
                LogSystem.Debug("GlobalVariables.RefreshSessionModule: " + (GlobalVariables.RefreshSessionModule != null).ToString());
                if (GlobalVariables.RefreshSessionModule != null)
                {
                    GlobalVariables.RefreshSessionModule();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            bool? success = false;
            try
            {
                if (!btnSave.Enabled)
                    return;

                SetEnableButtonSave(false);

                if (treatmentFee.TDL_TREATMENT_TYPE_ID != null)
                {
                    var treatmentType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == treatmentFee.TDL_TREATMENT_TYPE_ID);
                    if (treatmentFee.OUT_TIME.HasValue && (treatmentType.TRANS_TIME_OUT_TIME_OPTION == 1 || treatmentType.TRANS_TIME_OUT_TIME_OPTION == 2))
                    {
                        var transactionTime = Int64.Parse(Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTransactionTime.DateTime).ToString().Substring(0, 12));
                        var outTime = Int64.Parse(treatmentFee.OUT_TIME.ToString().Substring(0, 12));
                        if (treatmentFee.OUT_TIME.HasValue && transactionTime < outTime)
                        {
                            short type = 2;
                            MessageBoxButtons buttons = MessageBoxButtons.OK;
                            string message = string.Format(ResourceMessageLang.ThoiGianThanhToanNhoHonThoiGianRaVien, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(Int64.Parse(Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTransactionTime.DateTime).ToString())), Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(Int64.Parse(treatmentFee.OUT_TIME.ToString())));
                            if (treatmentType.TRANS_TIME_OUT_TIME_OPTION == 1)
                            {
                                type = 1;
                                message += " Bạn có muốn thực hiện điều trỉnh hóa đơn thanh toán không?";
                                buttons = MessageBoxButtons.YesNo;
                            }
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(message, ResourceMessageLang.ThongBao, buttons) == (type == 1 ? System.Windows.Forms.DialogResult.No : System.Windows.Forms.DialogResult.OK))
                                return;
                        }
                    }
                }

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                success = ProcessSave(ref param, false);
                WaitingManager.Hide();

                if (success == false)
                {
                    SetEnableButtonSave(true);
                    Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, false);
                }
                else
                {
                    if (chkPrintHddt.Checked)
                    {
                        this.onClickInHoaDonDienTu(null, null);
                    }
                }
                SessionManager.ProcessTokenLost(param);
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void onClickInHoaDonDienTu(object sender, EventArgs e)
        {
            try
            {
                if (this.resultTranBill == null || String.IsNullOrEmpty(this.resultTranBill.INVOICE_CODE))
                {
                    return;
                }

                ElectronicBillDataInput dataInput = new ElectronicBillDataInput();
                dataInput.PartnerInvoiceID = Inventec.Common.TypeConvert.Parse.ToInt64(this.resultTranBill.INVOICE_CODE);
                dataInput.InvoiceCode = this.resultTranBill.INVOICE_CODE;
                dataInput.NumOrder = this.resultTranBill.NUM_ORDER;
                dataInput.SymbolCode = this.resultTranBill.SYMBOL_CODE;
                dataInput.TemplateCode = this.resultTranBill.TEMPLATE_CODE;
                dataInput.TransactionTime = this.resultTranBill.EINVOICE_TIME ?? this.resultTranBill.TRANSACTION_TIME;
                dataInput.ENumOrder = this.resultTranBill.EINVOICE_NUM_ORDER;
                dataInput.EinvoiceTypeId = this.resultTranBill.EINVOICE_TYPE_ID;
                dataInput.Treatment = this.treatmentFee;
                dataInput.Transaction = new MOS.EFMODEL.DataModels.HIS_TRANSACTION();
                Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_TRANSACTION>(dataInput.Transaction, this.resultTranBill);
                dataInput.SereServs = new List<V_HIS_SERE_SERV_5>();
                dataInput.Branch = LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == LocalStorage.LocalData.WorkPlace.GetBranchId());
                ElectronicBillProcessor electronicBillProcessor = new ElectronicBillProcessor(dataInput);
                ElectronicBillResult electronicBillResult = null;

                electronicBillResult = electronicBillProcessor.Run(ElectronicBillType.ENUM.GET_INVOICE_LINK);

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

                DocumentViewerManager viewManager = new DocumentViewerManager(ViewType.ENUM.Pdf);
                InputADO ado = new InputADO();
                ado.DeleteWhenClose = true;
                ado.NumberOfCopy = HisConfigCFG.E_BILL__PRINT_NUM_COPY;
                ado.PrintPageSize = this.resultTranBill.EINVOICE_PAGE_SIZE;
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
        private void btnSavePrint_Click(object sender, EventArgs e)
        {
            bool success = false;
            try
            {
                PrintMps279 = false;
                this.positionHandleControl = -1;
                if (!btnSavePrint.Enabled)
                    return;
                SetEnableButtonSave(false);

                if (HisConfigCFG.AttachAssignPrintWarningOption == "1")
                {
                    Inventec.Common.Logging.LogSystem.Debug("HisConfigCFG.AttachAssignPrintWarningOption == 1");
                    MOS.EFMODEL.DataModels.HIS_TREATMENT treatment = GetTreatment(this.currentTransaction.TREATMENT_ID);
                    if (treatment != null && treatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                    {
                        CommonParam paramCommon = new CommonParam();
                        var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<string>>("api/HisServiceReq/GetAttachAssignPrint", ApiConsumers.MosConsumer, this.currentTransaction.TREATMENT_ID, paramCommon);
                        if (result != null && result.Count() > 0)
                        {
                            Inventec.Common.Logging.LogSystem.Debug("HisConfigCFG.AttachAssignPrintWarningOption == 1; result = " + result);
                            List<SAR_PRINT_TYPE> listSARPrintType = BackendDataWorker.Get<SAR_PRINT_TYPE>();
                            string strMessage = "";
                            foreach (var item in result)
                            {
                                strMessage += listSARPrintType.Where(o => o.PRINT_TYPE_CODE == item).Select(o => o.PRINT_TYPE_NAME).FirstOrDefault();
                                strMessage += ", ";
                            }
                            if (result != null && result.Count() > 0)
                            {
                                int index = strMessage.LastIndexOf(',');
                                strMessage.Remove(index, 1);

                                if (MessageBox.Show(String.Format("Bệnh nhân có các phiếu sau cần thu lại: {0}", strMessage), ResourceMessageLang.ThongBao, MessageBoxButtons.OK, MessageBoxIcon.Question) == DialogResult.OK)
                                {

                                }
                            }
                        }
                    }
                }

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                success = (bool)ProcessSave(ref param, false);
                WaitingManager.Hide();

                if (success == true)
                {
                    this.hienHoaDonNhap = false;
                    this.onClickPhieuThuThanhToan();
                }
                else
                {
                    SetEnableButtonSave(true);
                    Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnSaveAndSign_Click(object sender, EventArgs e)
        {
            bool? success = false;
            try
            {
                PrintMps279 = false;
                this.positionHandleControl = -1;
                if (!btnSaveAndSign.Enabled)
                    return;
                if (cboPayForm.EditValue != null && Int64.Parse(cboPayForm.EditValue.ToString()) == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR && MessageBox.Show("Thanh toán QR chưa thể tự động tạo hóa đơn điện tử bạn có muốn tiếp tục?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
                SetEnableButtonSave(false);
                if (String.IsNullOrEmpty(AdjustmentTransactionConfig.InvoiceTypeCreate))
                    return;

                if (!this.CheckHastInvoiceCancel())
                {
                    return;
                }
                    
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                ErrorElectronicBill = new List<string>();
                success = ProcessSave(ref param, true);
                param.Messages = param.Messages.Distinct().ToList();
                WaitingManager.Hide();

                if (success == true)
                {
                    this.hienHoaDonNhap = false;   
                    bool showResult = true;
                    if (CreatAgain && success == true)
                    {
                        string notification = "Xử lý thành công.";
                        notification += param.GetMessage();
                    }


                    if (!isnotPrintMPS000111)
                    {
                        //tự động in hóa đơn điện tử
                        if (chkPrintHddt.Checked)
                        {
                            int sleepTime = (int)(HisConfigCFG.ElectronicInvoicePublishingDelayTime * 1000);
                            Inventec.Common.Logging.LogSystem.Debug("SleepTime: " + sleepTime);
                            System.Threading.Thread.Sleep(sleepTime);
                            printPDFWithAcrobat();
                        }

                        if (!chkHideHddt.Checked)
                        {
                            if (AdjustmentTransactionConfig.InvoiceTypeCreate == invoiceTypeCreate__CreateInvoiceHIS)
                            {
                                //Chế độ HIS tự tạo hóa đơn điện tử & tự ký điện tử trên hóa đơn: sau khi tạo giao dịch trên hệ thống HIS thành công, tự tạo hóa đơn + ký điện tử trên hóa đơn lưu trên hệ thống HIS
                                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                                store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuThanhToan_MPS000111, InPhieuThuThanhToanKyDienTu);
                            }
                            else
                            {
                                //Nothing
                                if (!chkPrintHddt.Checked)
                                {
                                    int sleepTime = (int)(HisConfigCFG.ElectronicInvoicePublishingDelayTime * 1000);
                                    Inventec.Common.Logging.LogSystem.Debug("SleepTime: " + sleepTime);
                                    System.Threading.Thread.Sleep(sleepTime);
                                }
                                this.onClickInHoaDonDienTu(null, null);
                            }
                        }
                    }

                    //if (showResult)
                    //    MessageManager.Show(this, param, success);
                }
                else if (success == false)
                {
                    SetEnableButtonSave(true);
                    Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, false);
                }

                GeneratePopupMenu();

                Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private MOS.EFMODEL.DataModels.HIS_TREATMENT GetTreatment(long? treatmentId)
        {
            MOS.EFMODEL.DataModels.HIS_TREATMENT result = new MOS.EFMODEL.DataModels.HIS_TREATMENT();
            try
            {
                if (treatmentId.HasValue)
                {
                    HisTreatmentFilter filter = new HisTreatmentFilter();
                    filter.ID = treatmentId;
                    var apiresult = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, null);
                    if (apiresult != null && apiresult.Count > 0)
                    {
                        result = apiresult.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                result = new MOS.EFMODEL.DataModels.HIS_TREATMENT();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void onClickPhieuThuThanhToan()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                if (HisConfigCFG.TransactionDetail_PrintNow)
                {
                    this.isPrintNow = true;
                    onClickPhieuThuThanhToanChiTietDichVu(null, null);
                }
                else
                {
                    store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuThanhToan_MPS000111, InPhieuThuThanhToan);   
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickPhieuThuThanhToanChiTietDichVu(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

                var patientTypeAlter = new HIS_PATIENT_TYPE_ALTER();
                if (resultTranBill != null)
                {
                    var paramCommon = new CommonParam();
                    patientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetLastByTreatmentId", ApiConsumers.MosConsumer, resultTranBill.TREATMENT_ID, paramCommon);
                }

                if (patientTypeAlter != null && patientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                {
                    store.RunPrintTemplate(MPS.Processor.Mps000259.PDO.Mps000259PDO.printTypeCode, this.DeletegatePrintTemplate);
                }
                else
                {
                    store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__HoaDonThanhToanChiTietDichVu_Mps000106, this.DeletegatePrintTemplate);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuThuThanhToan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                DefaultDataPrintMps111();
                if (this.resultTranBill == null)
                {
                    decimal totalReceive = ((this.treatmentFee.TOTAL_DEPOSIT_AMOUNT ?? 0) + (this.treatmentFee.TOTAL_BILL_AMOUNT ?? 0) - (this.treatmentFee.TOTAL_BILL_TRANSFER_AMOUNT ?? 0) - (this.treatmentFee.TOTAL_BILL_FUND ?? 0) - (this.treatmentFee.TOTAL_REPAY_AMOUNT ?? 0)) - (this.treatmentFee.TOTAL_BILL_EXEMPTION ?? 0);

                    decimal totalReceiveMore = (this.treatmentFee.TOTAL_PATIENT_PRICE ?? 0) - totalReceive - (this.treatmentFee.TOTAL_BILL_FUND ?? 0) - (this.treatmentFee.TOTAL_BILL_EXEMPTION ?? 0);

                    if (HisConfigCFG.EnableSaveOption == "1" && totalReceiveMore <= 0)
                    {
                        #region
                        HisPatientTypeAlterViewAppliedFilter patyAlterAppliedFilter = new HisPatientTypeAlterViewAppliedFilter();
                        patyAlterAppliedFilter.InstructionTime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                        patyAlterAppliedFilter.TreatmentId = treatmentFee.ID;
                        var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, patyAlterAppliedFilter, null);
                        if (currentPatientTypeAlter == null)
                        {
                            Inventec.Common.Logging.LogSystem.Info("Khong lay duoc PatientTypeAlterApplied: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => treatmentFee.TREATMENT_CODE), treatmentFee.TREATMENT_CODE));
                        }
                        //
                        HisDepartmentTranLastFilter departLastFilter = new HisDepartmentTranLastFilter();
                        departLastFilter.TREATMENT_ID = treatmentFee.ID;
                        departLastFilter.BEFORE_LOG_TIME = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                        var departmentTran = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, departLastFilter, null);
                        //
                        //2
                        V_HIS_PATIENT patient = new V_HIS_PATIENT();

                        HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                        patientFilter.ID = treatmentFee.PATIENT_ID;
                        var patients = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, null);

                        if (patients != null && patients.Count > 0)
                        {
                            patient = patients.FirstOrDefault();
                        }

                        //
                        #endregion
                        WaitingManager.Hide();
                        string printerName = "";
                        if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                        {
                            printerName = GlobalVariables.dicPrinter[printTypeCode];
                        }

                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatmentFee != null ? treatmentFee.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                        MPS.Processor.Mps000111.PDO.Mps000111PDO pdo = new MPS.Processor.Mps000111.PDO.Mps000111PDO(null,
                            patient,
                            null,
                            null,
                            departmentTran,
                            currentPatientTypeAlter,
                            HisConfigCFG.PatientTypeId__BHYT,
                            null,
                            null,
                            null,
                            null
                            );

                        if (LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                        }
                        else
                        {
                            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                        }
                    }
                    else
                        return;
                }
                else
                {
                    WaitingManager.Show();
                    if (!LoadBillSereServBill())
                        return;
                    CreateThreadPrintMps111();

                    MPS.Processor.Mps000111.PDO.Mps000111PDO pdo = new MPS.Processor.Mps000111.PDO.Mps000111PDO(
                        resultTranBill,
                        patientsPrint,
                        listBillFundPrint,
                        listSereServPrint,
                        departmentTranPrint,
                        patientTypeAlterPrint,
                        HisConfigCFG.PatientTypeId__BHYT,
                        null,
                        listSereDepoPrint,
                        lstTranPrint,
                        lstSeseRepayPrint
                        );

                    MPS.ProcessorBase.Core.PrintData printData = null;

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((resultTranBill != null ? resultTranBill.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);
                    WaitingManager.Hide();
                    if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool InPhieuThuThanhToan(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                DefaultDataPrintMps111();
                if (this.resultTranBill == null)
                    return result;
                if (!LoadBillSereServBill())
                    return result;
                CreateThreadPrintMps111();

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((resultTranBill != null ? resultTranBill.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                MPS.Processor.Mps000111.PDO.Mps000111PDO pdo = new MPS.Processor.Mps000111.PDO.Mps000111PDO(
                    resultTranBill,
                    patientsPrint,
                    listBillFundPrint,
                    listSereServPrint,
                    departmentTranPrint,
                    patientTypeAlterPrint,
                    HisConfigCFG.PatientTypeId__BHYT,
                    listSereDepoPrint,
                    lstTranPrint,
                    lstSeseRepayPrint
                    );

                MPS.ProcessorBase.Core.PrintData printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                WaitingManager.Hide();
                if (isEmr)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintPreview, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void DefaultDataPrintMps111()
        {
            try
            {
                listBillFundPrint = new List<HIS_BILL_FUND>();
                hisSSBillsPrint = new List<HIS_SERE_SERV_BILL>();
                listSereServPrint = new List<HIS_SERE_SERV>();
                patientTypeAlterPrint = new V_HIS_PATIENT_TYPE_ALTER();
                departmentTranPrint = new V_HIS_DEPARTMENT_TRAN();
                patientsPrint = new V_HIS_PATIENT();
                lstTranPrint = new List<V_HIS_TRANSACTION>();
                lstSeseRepayPrint = new List<HIS_SESE_DEPO_REPAY>();
                listSereDepoPrint = new List<HIS_SERE_SERV_DEPOSIT>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool LoadBillSereServBill()
        {
            try
            {
                if (this.resultTranBill != null)
                {
                    HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                    ssBillFilter.BILL_ID = this.resultTranBill.ID;
                    hisSSBillsPrint = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, null);
                    if (hisSSBillsPrint == null || hisSSBillsPrint.Count <= 0)
                    {
                        throw new Exception("Khong lay duoc SereServBill theo BillId: " + this.resultTranBill.ID);
                    }

                    HisSereServFilter ssFilter = new HisSereServFilter();
                    ssFilter.TREATMENT_ID = this.resultTranBill.TREATMENT_ID.Value;
                    List<HIS_SERE_SERV> listSereServApi = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, null);

                    if (listSereServApi != null && listSereServApi.Count > 0 && hisSSBillsPrint != null && hisSSBillsPrint.Count > 0)
                    {
                        listSereServPrint = listSereServApi.Where(o => hisSSBillsPrint.Select(p => p.SERE_SERV_ID).Contains(o.ID)).ToList();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return false;
        }

        private void CreateThreadPrintMps111()
        {
            Thread ThreadLoadBillFund = new Thread(new ThreadStart(LoadBillFund));
            Thread ThreadLoadPatientTypeAlterViewApplied = new Thread(new ThreadStart(LoadBillPatientTypeAlterViewApplied));
            Thread ThreadLoadDepartmentTranLast = new Thread(new ThreadStart(LoadDepartmentTranLast));
            Thread ThreadLoadPatient = new Thread(new ThreadStart(LoadPatient));
            Thread ThreadLoadTransaction = new Thread(new ThreadStart(LoadTransaction));
            Thread ThreadLoadSeseDepoRepay = new Thread(new ThreadStart(LoadSeseDepoRepay));
            Thread ThreadLoadSereServDeposit = new Thread(new ThreadStart(LoadSereServDeposit));
            try
            {
                ThreadLoadBillFund.Start();
                ThreadLoadPatientTypeAlterViewApplied.Start();
                ThreadLoadDepartmentTranLast.Start();
                ThreadLoadPatient.Start();
                ThreadLoadTransaction.Start();
                ThreadLoadSeseDepoRepay.Start();
                ThreadLoadSereServDeposit.Start();
                ThreadLoadBillFund.Join();
                ThreadLoadPatientTypeAlterViewApplied.Join();
                ThreadLoadDepartmentTranLast.Join();
                ThreadLoadPatient.Join();
                ThreadLoadTransaction.Join();
                ThreadLoadSeseDepoRepay.Join();
                ThreadLoadSereServDeposit.Join();
            }
            catch (Exception ex)
            {
                ThreadLoadBillFund.Abort();
                ThreadLoadPatientTypeAlterViewApplied.Abort();
                ThreadLoadDepartmentTranLast.Abort();
                ThreadLoadPatient.Abort();
                ThreadLoadTransaction.Abort();
                ThreadLoadSeseDepoRepay.Abort();
                ThreadLoadSereServDeposit.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadBillFund()
        {
            try
            {
                if (this.resultTranBill != null)
                {
                    HisBillFundFilter billFundFilter = new HisBillFundFilter();
                    billFundFilter.BILL_ID = this.resultTranBill.ID;
                    listBillFundPrint = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_BILL_FUND>>("api/HisBillFund/Get", ApiConsumers.MosConsumer, billFundFilter, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadBillPatientTypeAlterViewApplied()
        {
            try
            {
                if (this.resultTranBill != null)
                {
                    HisPatientTypeAlterViewAppliedFilter patyAlterAppliedFilter = new HisPatientTypeAlterViewAppliedFilter();
                    patyAlterAppliedFilter.InstructionTime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    patyAlterAppliedFilter.TreatmentId = this.resultTranBill.TREATMENT_ID.Value;
                    patientTypeAlterPrint = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, patyAlterAppliedFilter, null);
                    if (patientTypeAlterPrint == null)
                    {
                        Inventec.Common.Logging.LogSystem.Info("Khong lay duoc PatientTypeAlterApplied: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.resultTranBill.TREATMENT_CODE), this.resultTranBill.TREATMENT_CODE));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDepartmentTranLast()
        {
            try
            {
                if (this.resultTranBill != null)
                {
                    HisDepartmentTranLastFilter departLastFilter = new HisDepartmentTranLastFilter();
                    departLastFilter.TREATMENT_ID = this.resultTranBill.TREATMENT_ID.Value;
                    departLastFilter.BEFORE_LOG_TIME = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    departmentTranPrint = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, departLastFilter, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPatient()
        {
            try
            {
                if (this.resultTranBill != null)
                {
                    V_HIS_PATIENT patient = new V_HIS_PATIENT();
                    if (this.resultTranBill.TDL_PATIENT_ID.HasValue)
                    {
                        HisPatientViewFilter filter = new HisPatientViewFilter();
                        filter.ID = this.resultTranBill.TDL_PATIENT_ID;
                        var patients = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                        if (patients != null && patients.Count > 0)
                        {
                            patientsPrint = patients.First();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadTransaction()
        {
            try
            {
                if (this.resultTranBill != null)
                {
                    HisTransactionViewFilter fl = new HisTransactionViewFilter();
                    fl.TREATMENT_ID = this.resultTranBill.TREATMENT_ID.Value;
                    lstTranPrint = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumer.ApiConsumers.MosConsumer, fl, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadSeseDepoRepay()
        {
            try
            {
                if (this.resultTranBill != null)
                {
                    HisSeseDepoRepayFilter x = new HisSeseDepoRepayFilter();
                    x.TDL_TREATMENT_ID = this.resultTranBill.TREATMENT_ID.Value;
                    x.IS_CANCEL = false;
                    lstSeseRepayPrint = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SESE_DEPO_REPAY>>("api/HisSeseDepoRepay/Get", ApiConsumers.MosConsumer, x, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadSereServDeposit()
        {
            try
            {
                if (this.resultTranBill != null)
                {
                    HisSereServDepositFilter defilter = new HisSereServDepositFilter();
                    defilter.TDL_TREATMENT_ID = this.resultTranBill.TREATMENT_ID.Value;
                    defilter.IS_CANCEL = false;
                    listSereDepoPrint = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumers.MosConsumer, defilter, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DeletegatePrintTemplate(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuThanhToan_MPS000111:
                        InPhieuThuThanhToan(printCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__HoaDonTTTheoYeuCauDichVu_MPS000103:
                        InPhieuThuTTTheoYeuCauDichVu(printCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__HoaDonThanhToanChiTietDichVu_Mps000106:
                        InPhieuThuTTChiTietDichVu(printCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BienLaiThuPhiLePhi_MPS000114:
                        InBienlaiThuPhiLePhi(printCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuChiDinhDuaVaoGiaoDichThanhToan_Mps000105:
                        InPhieuChiDinhDuaVaoGiaoDichThanhToan(printCode, fileName, ref result);
                        break;
                    case MPS.Processor.Mps000259.PDO.Mps000259PDO.printTypeCode:
                        InPhieuThuTTChiTietDichVuNgoaiTru(printCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuHoanUngThanhToanRaVien_Mps000361:
                        InPhieuHoanUngThanhToanRaVien(printCode, fileName, ref result);
                        break;
                    //case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuHoanUng_MPS000113:
                    //    InPhieuThuHoanUng(printCode, fileName, ref result);
                    //    break;
                    //case MPS.Processor.Mps000431.PDO.Mps000431PDO.printTypeCode:
                    //    InHoaDonNhap(printCode, fileName, ref result);
                    //    break;
                    case "Mps000479":
                        InMps479(printCode, fileName, ref result);
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

        private bool CheckHastInvoiceCancel()
        {
            bool result = false;
            try
            {
                if (this.currentTransaction != null && this.currentTransaction.TREATMENT_ID != null)
                {
                    HisTransactionFilter tFilter = new HisTransactionFilter();
                    tFilter.TREATMENT_ID = this.currentTransaction.TREATMENT_ID;
                    tFilter.TRANSACTION_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT;
                    tFilter.HAS_INVOICE_CODE = true;
                    tFilter.IS_CANCEL = true;
                    List<MOS.EFMODEL.DataModels.HIS_TRANSACTION> tranCancels = new BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_TRANSACTION>>("api/HisTransaction/Get", ApiConsumers.MosConsumer, tFilter, null);
                    if (tranCancels != null && tranCancels.Count > 0)
                    {
                        string invoices = String.Join("; ", tranCancels.Select(s => s.EINVOICE_NUM_ORDER).ToList());
                        if (XtraMessageBox.Show(String.Format(ResourceMessageLang.BenhNhanDaXuatHoaDonBanCoMuonXuatHoaDonMoi, invoices), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo, DevExpress.Utils.DefaultBoolean.True) != System.Windows.Forms.DialogResult.Yes)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public void printPDFWithAcrobat()
        {
            if (this.resultTranBill == null || String.IsNullOrEmpty(this.resultTranBill.INVOICE_CODE))
            {
                //MessageBox.Show("Hóa đơn chưa thanh toán hoặc chưa cấu hình hóa đơn điện tử.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ElectronicBillDataInput dataInput = new ElectronicBillDataInput();
            dataInput.PartnerInvoiceID = Inventec.Common.TypeConvert.Parse.ToInt64(this.resultTranBill.INVOICE_CODE);
            dataInput.InvoiceCode = this.resultTranBill.INVOICE_CODE;
            dataInput.NumOrder = this.resultTranBill.NUM_ORDER;
            dataInput.SymbolCode = this.resultTranBill.SYMBOL_CODE;
            dataInput.TemplateCode = this.resultTranBill.TEMPLATE_CODE;
            dataInput.TransactionTime = this.resultTranBill.EINVOICE_TIME ?? this.resultTranBill.TRANSACTION_TIME;
            dataInput.ENumOrder = this.resultTranBill.EINVOICE_NUM_ORDER;
            dataInput.EinvoiceTypeId = this.resultTranBill.EINVOICE_TYPE_ID;
            dataInput.Treatment = this.treatmentFee;
            dataInput.SereServs = new List<V_HIS_SERE_SERV_5>();
            dataInput.Branch = LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == LocalStorage.LocalData.WorkPlace.GetBranchId());
            ElectronicBillProcessor electronicBillProcessor = new ElectronicBillProcessor(dataInput);
            ElectronicBillResult electronicBillResult = null;

            electronicBillResult = electronicBillProcessor.Run(ElectronicBillType.ENUM.GET_INVOICE_LINK);

            if (electronicBillResult == null || String.IsNullOrEmpty(electronicBillResult.InvoiceLink))
            {
                MessageBox.Show("Không tìm thấy link hóa đơn điện tử");
                return;
            }
            //string output = Inventec.Common.SignLibrary.Utils.GenerateTempFileWithin();
            //InsertPageOne(electronicBillResult.InvoiceLink, output);
            //string Filepath = output;

            //System.Net.WebClient client = new System.Net.WebClient();
            //this.byteData = client.DownloadData(Filepath);
            //MemoryStream ms = new MemoryStream(this.byteData);

            //DevExpress.XtraPdfViewer.PdfViewer pdfViewer1 = new DevExpress.XtraPdfViewer.PdfViewer();
            //pdfViewer1.LoadDocument(ms);
            //DevExpress.Pdf.PdfPrinterSettings pdfPrinterSettings = new DevExpress.Pdf.PdfPrinterSettings();
            //pdfPrinterSettings.Settings.Copies = (short)(HisConfigCFG.E_BILL__PRINT_NUM_COPY > 0 ? HisConfigCFG.E_BILL__PRINT_NUM_COPY : 1);
            //pdfViewer1.Print(pdfPrinterSettings);


            DocumentViewerManager viewManager = new DocumentViewerManager(ViewType.ENUM.Pdf);
            InputADO ado = new InputADO();
            ado.DeleteWhenClose = true;
            ado.NumberOfCopy = HisConfigCFG.E_BILL__PRINT_NUM_COPY;
            ado.URL = electronicBillResult.InvoiceLink;
            ado.PrintPageSize = resultTranBill.EINVOICE_PAGE_SIZE;
            ViewType.Platform type = ViewType.Platform.Telerik;
            if (HisConfigCFG.PlatformOption > 0)
            {
                type = (ViewType.Platform)(HisConfigCFG.PlatformOption - 1);
            }

            viewManager.Print(ado, type);
        }

        private bool InPhieuThuThanhToanKyDienTu(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (this.resultTranBill == null)
                    return result;
                DefaultDataPrintMps111();
                if (!LoadBillSereServBill())
                    return result;
                CreateThreadPrintMps111();
                CommonParam param = new CommonParam();
                MemoryStream streamResult = new MemoryStream();

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((resultTranBill != null ? resultTranBill.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);


                MPS.Processor.Mps000111.PDO.Mps000111PDO pdo = new MPS.Processor.Mps000111.PDO.Mps000111PDO(
                    resultTranBill,
                    null,
                    listBillFundPrint,
                    listSereServPrint,
                    departmentTranPrint,
                    patientTypeAlterPrint,
                    HisConfigCFG.PatientTypeId__BHYT,
                    listSereDepoPrint,
                    lstTranPrint,
                    lstSeseRepayPrint);
                MPS.ProcessorBase.Core.PrintData printData;
                if (isEmr)
                {
                    printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, "", 1, streamResult) { EmrInputADO = inputADO };
                }
                else
                {
                    printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "", 1, streamResult) { EmrInputADO = inputADO };

                }

                result = MPS.MpsPrinter.Run(printData);

                if (result && printData.saveMemoryStream != null && printData.saveMemoryStream.Length > 0)
                {
                    result = false;
                    streamResult.Position = 0;
                    MemoryStream outStream = new MemoryStream();
                    //Gọi thư viện convert file excel đã qua xử lý về định dạng pdf
                    if (Inventec.Common.FileConvert.Convert.ExcelToPdfUsingFlex(printData.saveMemoryStream, "", outStream, ""))
                    {
                        outStream.Position = 0;
                        if (outStream != null && outStream.Length > 0)
                        {
                            //Gọi thư viện đọc chứng thư trên máy và thực hiện ký điện tử trên file pdf
                            //Trước khi ký sẽ thực hiện các xử lý mã hóa,...
                            Inventec.Ca.Processor processor = new Inventec.Ca.Processor();
                            string pdfContentBase64 = Convert.ToBase64String(ReadFully(outStream));
                            var pdfContentSigned = processor.SignPdfBase64(pdfContentBase64, "");

                            //Chuyển đổi chuỗi base64 về mảng byte
                            var base64EncodedBytes = System.Convert.FromBase64String(pdfContentSigned);
                            //Chuyển đổi mảng byte của fiel kết quả về dạng MemoryStream
                            MemoryStream outStreamResult = new MemoryStream(base64EncodedBytes);
                            outStreamResult.Position = 0;
                            //Gọi api fss upload file hóa đơn đã ký điện tử thành công
                            string fileNameUpload = this.resultTranBill.ACCOUNT_BOOK_CODE + "__" + this.resultTranBill.TRANSACTION_CODE + SIGNED_EXTENSION;
                            var fileUploadInfo = Inventec.Fss.Client.FileUpload.UploadFile(GlobalVariables.APPLICATION_CODE, "FILESIGNED", outStreamResult, fileNameUpload);
                            if (fileUploadInfo != null)
                            {
                                //Cập nhật lại trường FILE_URL, FILE_NAME của bảng Bill
                                this.resultTranBill.FILE_URL = fileUploadInfo.Url;
                                this.resultTranBill.FILE_NAME = fileNameUpload;
                                //Review
                                MOS.EFMODEL.DataModels.HIS_TRANSACTION updateFile = new MOS.EFMODEL.DataModels.HIS_TRANSACTION();
                                AutoMapper.Mapper.CreateMap<V_HIS_TRANSACTION, MOS.EFMODEL.DataModels.HIS_TRANSACTION>();
                                updateFile = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.HIS_TRANSACTION>(this.resultTranBill);
                                V_HIS_TRANSACTION rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<V_HIS_TRANSACTION>("api/HisTransaction/UpdateFile", ApiConsumers.MosConsumer, updateFile, param);
                                if (rs != null && !String.IsNullOrEmpty(rs.FILE_URL))
                                {
                                    Inventec.Common.Logging.LogSystem.Debug("Ky dien tu cho giao dich hoa don thanh toan thanh cong. TRANSACTION_CODE = " + this.resultTranBill.TRANSACTION_CODE + ", Fss_Url_Signed_File = " + fileUploadInfo.Url);
                                    result = true;
                                }
                                else
                                {
                                    Inventec.Common.Logging.LogSystem.Warn("Tao giao dich thanh toan thanh cong, tao va upload file pdf cho hoa don thanh toan thanh cong. Tuy nhien qua trinh cap nhat url cua file pdf vao bang BILL that bai.");
                                }
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Da thuc hien viec ky dien tu tren file pdf hoa don thanh toan xong, tuy nhien upload file ket qua len server that bai. Cac buoc xu ly tiep sau khong the thuc hien.");
                            }
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Convert file excel da xu ly về dinh dang pdf that bai. Ky dien tu that bai.");
                        }
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Xu ly ExcelToPdf that bai. Tao file pdf convert tu file excel da qua xu ly that bai, cac buoc xu ly tiep sau khong the thuc hien");
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Warn("Tao giao dich thanh toan thanh cong, tuy nhien xu ly tao file excel hoa don thanh toan that bai. Khong the thuc hien ky dien tu tren hoa don thanh toan.");
                }
                if (!result)
                {
                    param.Messages.Add(HIS.Desktop.Plugins.AdjustmentTransaction.Base.ResourceMessageLang.TaoThanhToanThanhCong_TuyNhienThucHienKyDienTuThatBai);
                    MessageManager.Show(param, result);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        private void GeneratePopupMenu()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();

                //if (this.hienHoaDonNhap && AdjustmentTransactionConfig.InvoiceTypeCreate == invoiceTypeCreate__CreateInvoiceVnpt)
                //{
                //    menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__BTN_DROP_DOWN__ITEM_HOA_DON_NHAP", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickHoaDonNhap)));
                //}
                //else
                {
                    menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__BTN_DROP_DOWN__ITEM_PHIEU_THU_THANH_TOAN", Base.ResourceLangManager.LanguageFrmAdjustmentTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickPhieuThuThanhToan)));

                    menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__BTN_DROP_DOWN__ITEM_PHIEU_THU_TT_THEO_YEU_CAU", Base.ResourceLangManager.LanguageFrmAdjustmentTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickPhieuThuThanhToanTheoYeuCau)));

                    menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__BTN_DROP_DOWN__ITEM_PHIEU_THU_TT_CHI_TIET_DICH_VU", Base.ResourceLangManager.LanguageFrmAdjustmentTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickPhieuThuThanhToanChiTietDichVu)));

                    menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__BTN_DROP_DOWN__ITEM_BIEN_LAI_PHI_LE_PHI", Base.ResourceLangManager.LanguageFrmAdjustmentTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickBienLaiThuPhiLePhi)));

                    menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__BTN_DROP_DOWN__ITEM_IN_PHIEU_CHI_DINH", Base.ResourceLangManager.LanguageFrmAdjustmentTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickPhieuChiDinh)));

                    menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__BTN_DROP_DOWN__ITEM_IN_HOA_DON_DIEN_TU", Base.ResourceLangManager.LanguageFrmAdjustmentTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickInHoaDonDienTu)));
                    if (resultTranBill != null && resultTranBill.EINVOICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__VNPT)
                    {
                        menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__BTN_DROP_DOWN__ITEM_CHUYEN_DOI_HOA_DON_DIEN_TU", Base.ResourceLangManager.LanguageFrmAdjustmentTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickChuyenDoiHoaDonDienTu)));
                    }

                    if (this.treatmentFee != null && this.treatmentFee.IS_PAUSE == 1)
                    {
                        menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__BTN_DROP_DOWN__ITEM_IN_HOAN_UNG_THANH_TOAN", Base.ResourceLangManager.LanguageFrmAdjustmentTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickInThanhToanHoanUng)));
                    }
                    menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__BTN_DROP_DOWN__PHIEU_THU_HOAN_UNG_MPS113", Base.ResourceLangManager.LanguageFrmAdjustmentTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickPhieuThuHoanUng)));
                }
                ddBtnPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void onClickHoaDonNhap(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
        //        store.RunPrintTemplate(MPS.Processor.Mps000431.PDO.Mps000431PDO.printTypeCode, DeletegatePrintTemplate);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void onClickPhieuThuThanhToan(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuThanhToan_MPS000111, DeletegatePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickPhieuThuThanhToanTheoYeuCau(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__HoaDonTTTheoYeuCauDichVu_MPS000103, DeletegatePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickBienLaiThuPhiLePhi(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BienLaiThuPhiLePhi_MPS000114, DeletegatePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickPhieuChiDinh(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuChiDinhDuaVaoGiaoDichThanhToan_Mps000105, DeletegatePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickChuyenDoiHoaDonDienTu(object sender, EventArgs e)
        {
            try
            {
                if (this.resultTranBill == null || String.IsNullOrEmpty(this.resultTranBill.INVOICE_CODE))
                {
                    return;
                }

                ElectronicBillDataInput dataInput = new ElectronicBillDataInput();
                dataInput.PartnerInvoiceID = Inventec.Common.TypeConvert.Parse.ToInt64(this.resultTranBill.INVOICE_CODE);
                dataInput.InvoiceCode = resultTranBill.INVOICE_CODE;
                dataInput.NumOrder = resultTranBill.NUM_ORDER;
                dataInput.SymbolCode = resultTranBill.SYMBOL_CODE;
                dataInput.TemplateCode = resultTranBill.TEMPLATE_CODE;
                dataInput.TransactionTime = resultTranBill.EINVOICE_TIME ?? resultTranBill.TRANSACTION_TIME;
                dataInput.ENumOrder = resultTranBill.EINVOICE_NUM_ORDER;
                dataInput.EinvoiceTypeId = resultTranBill.EINVOICE_TYPE_ID;
                dataInput.Treatment = this.treatmentFee;
                dataInput.SereServs = new List<V_HIS_SERE_SERV_5>();
                dataInput.Branch = LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == LocalStorage.LocalData.WorkPlace.GetBranchId());
                ElectronicBillProcessor electronicBillProcessor = new ElectronicBillProcessor(dataInput);
                ElectronicBillResult electronicBillResult = null;

                electronicBillResult = electronicBillProcessor.Run(ElectronicBillType.ENUM.CONVERT_INVOICE);

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
                ado.PrintPageSize = resultTranBill.EINVOICE_PAGE_SIZE;
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

        private void onClickInThanhToanHoanUng(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuHoanUngThanhToanRaVien_Mps000361, DeletegatePrintTemplate);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickPhieuThuHoanUng(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuHoanUng_MPS000113, DeletegatePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuThuTTTheoYeuCauDichVu(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.resultTranBill == null)
                    return;
                WaitingManager.Show();
                HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                ssBillFilter.BILL_ID = this.resultTranBill.ID;
                var hisSSBills = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, null);
                if (hisSSBills == null || hisSSBills.Count <= 0)
                {
                    throw new Exception("Khong lay duoc SereServBill theo BillId: " + this.resultTranBill.ID);
                }

                HisPatientTypeAlterViewAppliedFilter ptAlterAppFilter = new HisPatientTypeAlterViewAppliedFilter();
                ptAlterAppFilter.TreatmentId = this.currentTransaction.TREATMENT_ID ?? 0;
                ptAlterAppFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, ptAlterAppFilter, null);

                // tính mức hưởng của thẻ
                string levelCode = LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT;
                string ratio_text = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentPatientTypeAlter.HEIN_CARD_NUMBER, currentPatientTypeAlter.LEVEL_CODE, currentPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100) + "";

                HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = this.treatmentFee.PATIENT_ID;
                var patients = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, null);
                V_HIS_PATIENT patient = new V_HIS_PATIENT();
                if (patients != null && patients.Count > 0)
                {
                    patient = patients.FirstOrDefault();
                }

                HisSereServViewFilter ssFilter = new HisSereServViewFilter();
                ssFilter.IDs = hisSSBills.Select(s => s.SERE_SERV_ID).ToList();
                ssFilter.TREATMENT_ID = this.currentTransaction.TREATMENT_ID;
                var listSereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, ssFilter, null);
                WaitingManager.Hide();

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.resultTranBill != null ? this.resultTranBill.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                if (listSereServ != null && listSereServ.Count > 0)
                {
                    var Groups = listSereServ.GroupBy(o => o.SERVICE_REQ_ID).ToList();
                    foreach (var group in Groups)
                    {
                        var listSub = group.ToList<V_HIS_SERE_SERV>();
                        MPS.Processor.Mps000103.PDO.Mps000103PDO rdo = new MPS.Processor.Mps000103.PDO.Mps000103PDO(patient, this.resultTranBill, listSub, currentPatientTypeAlter, ratio_text);

                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, null) { EmrInputADO = inputADO });
                    }
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
                if (this.resultTranBill == null)
                    return;
                WaitingManager.Show();
                var listSereServ = new List<V_HIS_SERE_SERV>();
                HisSereServBillViewFilter ssBillFilter = new HisSereServBillViewFilter();
                ssBillFilter.BILL_ID = this.resultTranBill.ID;
                var hisSSBills = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_BILL>>("api/HisSereServBill/GetView", ApiConsumers.MosConsumer, ssBillFilter, null);
                if (hisSSBills == null || hisSSBills.Count <= 0)
                {
                    throw new Exception("Khong lay duoc SereServBill theo BillId: " + this.resultTranBill.ID);
                }

                if (Print106Type != "1")
                {
                    HisSereServViewFilter ssFilter = new HisSereServViewFilter();
                    ssFilter.IDs = hisSSBills.Select(s => s.SERE_SERV_ID).ToList(); ;
                    ssFilter.TREATMENT_ID = this.currentTransaction.TREATMENT_ID;
                    listSereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, ssFilter, null);

                    if (Print106Type_Expend == "1")
                    {
                        if (listSereServ != null && listSereServ.Count > 0)
                        {
                            HisSereServViewFilter ssFilter1 = new HisSereServViewFilter();
                            ssFilter1.TREATMENT_ID = this.currentTransaction.TREATMENT_ID;
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
                    ssFilter.TREATMENT_ID = this.currentTransaction.TREATMENT_ID;
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
                ptAlterAppFilter.TreatmentId = this.currentTransaction.TREATMENT_ID ?? 0;
                ptAlterAppFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, ptAlterAppFilter, null);

                // tính mức hưởng của thẻ
                string levelCode = LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT;
                string ratio_text = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentPatientTypeAlter.HEIN_CARD_NUMBER, currentPatientTypeAlter.LEVEL_CODE, currentPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100) + "";

                HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = this.treatmentFee.PATIENT_ID;
                var patients = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, null);
                V_HIS_PATIENT patient = new V_HIS_PATIENT();
                if (patients != null && patients.Count > 0)
                {
                    patient = patients.FirstOrDefault();
                }

                if (listSereServ != null && listSereServ.Count > 0)
                {
                    decimal totalDeposit = GetDepositAmount(this.currentTransaction.TREATMENT_ID);
                    MOS.EFMODEL.DataModels.HIS_TREATMENT treatment = GetTreatment(this.currentTransaction.TREATMENT_ID);

                    MPS.Processor.Mps000106.PDO.Mps000106ADO ado = new MPS.Processor.Mps000106.PDO.Mps000106ADO();
                    ado.PatientTypeBHYT = HisConfigCFG.PatientTypeId__BHYT;
                    ado.PatientTypeVP = HisConfigCFG.PatientTypeId__VP;

                    MPS.Processor.Mps000106.PDO.Mps000106PDO rdo = new MPS.Processor.Mps000106.PDO.Mps000106PDO(this.resultTranBill, listSereServ, hisSSBills, treatment, totalDeposit, totalCanThu, currentPatientTypeAlter, patient, ratio_text, ado, BackendDataWorker.Get<HIS_DEPARTMENT>());
                    rdo.ShowExpend = Print106Type_Expend == "1";
                    WaitingManager.Hide();
                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.resultTranBill != null ? this.resultTranBill.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode) && !String.IsNullOrEmpty(GlobalVariables.dicPrinter[printTypeCode]))
                    {
                        if (isPrintNow)
                        {
                            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                        }
                        else
                        {
                            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                        }
                    }
                    else
                    {
                        if (isPrintNow)
                        {
                            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                        }
                        else
                        {
                            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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
                    var apiresult = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_TRANSACTION>>("api/HisTransaction/Get", ApiConsumers.MosConsumer, filter, null);
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
                    PrintLogADO ado = new PrintLogADO(printTypeCode, uniqueCode);

                    List<object> listArgs = new List<object>();
                    listArgs.Add(ado);

                    PluginInstanceBehavior.ShowModule("Inventec.Desktop.Plugins.PrintLog", currentModule.RoomId, currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InBienlaiThuPhiLePhi(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.resultTranBill == null)
                    return;
                HisPatientTypeAlterViewAppliedFilter ptAlterAppFilter = new HisPatientTypeAlterViewAppliedFilter();
                ptAlterAppFilter.TreatmentId = this.currentTransaction.TREATMENT_ID ?? 0;
                ptAlterAppFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, ptAlterAppFilter, null);

                HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = this.treatmentFee.PATIENT_ID;
                var patients = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, null);
                V_HIS_PATIENT patient = new V_HIS_PATIENT();
                if (patients != null && patients.Count > 0)
                {
                    patient = patients.FirstOrDefault();
                }

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.resultTranBill != null ? this.resultTranBill.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                MPS.Processor.Mps000114.PDO.Mps000114PDO rdo = new MPS.Processor.Mps000114.PDO.Mps000114PDO(this.resultTranBill, patient, totalCanThu, currentPatientTypeAlter);
                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuChiDinhDuaVaoGiaoDichThanhToan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.resultTranBill == null)
                    return;
                WaitingManager.Show();
                //V_HIS_PATY_ALTER_BHYT patyAlter = null;

                HisPatientTypeAlterViewAppliedFilter ptAlterAppFilter = new HisPatientTypeAlterViewAppliedFilter();
                ptAlterAppFilter.TreatmentId = this.currentTransaction.TREATMENT_ID ?? 0;
                ptAlterAppFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, ptAlterAppFilter, null);

                HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                ssBillFilter.BILL_ID = this.resultTranBill.ID;
                var hisSSBills = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, null);
                if (hisSSBills == null || hisSSBills.Count <= 0)
                {
                    throw new Exception("Khong lay duoc SereServBill theo BillId: " + this.resultTranBill.ID);
                }

                // tính mức hưởng của thẻ
                string levelCode = LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT;
                string ratio_text = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentPatientTypeAlter.HEIN_CARD_NUMBER, currentPatientTypeAlter.LEVEL_CODE, currentPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100) + "";

                HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = this.treatmentFee.PATIENT_ID;
                var patients = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, null);
                V_HIS_PATIENT patient = new V_HIS_PATIENT();
                if (patients != null && patients.Count > 0)
                {
                    patient = patients.FirstOrDefault();
                }

                HisSereServViewFilter sereServFilter = new HisSereServViewFilter();
                sereServFilter.TREATMENT_ID = this.currentTransaction.TREATMENT_ID;
                sereServFilter.IDs = hisSSBills.Select(s => s.SERE_SERV_ID).ToList();
                var listSereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, sereServFilter, null);
                if (listSereServ != null && listSereServ.Count > 0)
                {
                    HisServiceReqViewFilter serviceReqFilter = new HisServiceReqViewFilter();
                    serviceReqFilter.IDs = listSereServ.Select(p => p.SERVICE_REQ_ID ?? 0).ToList();
                    var listServiceReqs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW, ApiConsumers.MosConsumer, serviceReqFilter, null);

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.resultTranBill != null ? this.resultTranBill.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                    bool AssignServicePrintTEST = (HisConfigs.Get<string>("HIS.Desktop.Plugins.AssignServicePrintTEST") == "1");
                    if (AssignServicePrintTEST)
                    {
                        //In tach theo phong xl
                        var _SeveServTests = listSereServ.Where(p => p.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN).ToList();
                        if (_SeveServTests != null && _SeveServTests.Count > 0)
                        {
                            listSereServ.RemoveAll(p => _SeveServTests.Contains(p));//

                            var Groups = _SeveServTests.GroupBy(o => o.SERVICE_REQ_ID).Select(p => p.ToList()).ToList();
                            foreach (var items in Groups)
                            {
                                V_HIS_SERVICE_REQ serviceReq = listServiceReqs.FirstOrDefault(p => p.ID == items.First().SERVICE_REQ_ID);
                                List<long> _ServiceIds = items.Select(p => p.SERVICE_ID).ToList();
                                var dataServices = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => _ServiceIds.Contains(p.ID)).ToList();
                                var _ServiceGroups = dataServices.GroupBy(p => p.PARENT_ID).Select(p => p.ToList()).ToList();
                                foreach (var item in _ServiceGroups)
                                {
                                    List<long> _ServicePrintIds = item.Select(p => p.ID).ToList();
                                    var dataPrints = items.Where(p => _ServicePrintIds.Contains(p.SERVICE_ID)).ToList();
                                    MPS.Processor.Mps000105.PDO.Mps000105PDO rdo = new MPS.Processor.Mps000105.PDO.Mps000105PDO(this.resultTranBill, dataPrints, currentPatientTypeAlter, serviceReq, patient, ratio_text);
                                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO });
                                }
                            }
                        }
                    }

                    bool AssignServicePrintCDHA = (HisConfigs.Get<string>("HIS.Desktop.Plugins.AssignServicePrintCDHA") == "1");
                    if (AssignServicePrintCDHA)
                    {
                        //In tach theo phong xl
                        var _SeveServCDHAs = listSereServ.Where(p => p.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA).ToList();
                        if (_SeveServCDHAs != null && _SeveServCDHAs.Count > 0)
                        {
                            listSereServ.RemoveAll(p => _SeveServCDHAs.Contains(p));//
                            var Groups = _SeveServCDHAs.GroupBy(o => o.SERVICE_REQ_ID).Select(p => p.ToList()).ToList();
                            foreach (var items in Groups)
                            {
                                V_HIS_SERVICE_REQ serviceReq = listServiceReqs.FirstOrDefault(p => p.ID == items.First().SERVICE_REQ_ID);
                                List<long> _ServiceIds = items.Select(p => p.SERVICE_ID).ToList();
                                var dataServices = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => _ServiceIds.Contains(p.ID)).ToList();
                                var _ServiceGroups = dataServices.GroupBy(p => p.PARENT_ID).Select(p => p.ToList()).ToList();
                                foreach (var item in _ServiceGroups)
                                {
                                    List<long> _ServicePrintIds = item.Select(p => p.ID).ToList();
                                    var dataPrints = items.Where(p => _ServicePrintIds.Contains(p.SERVICE_ID)).ToList();
                                    MPS.Processor.Mps000105.PDO.Mps000105PDO rdo = new MPS.Processor.Mps000105.PDO.Mps000105PDO(this.resultTranBill, dataPrints, currentPatientTypeAlter, serviceReq, patient, ratio_text);
                                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO });
                                }
                            }
                        }
                    }

                    if (listSereServ != null && listSereServ.Count > 0)
                    {
                        var Groups = listSereServ.GroupBy(o => o.SERVICE_REQ_ID).Select(p => p.ToList()).ToList();
                        foreach (var group in Groups)
                        {
                            V_HIS_SERVICE_REQ serviceReq = listServiceReqs.FirstOrDefault(p => p.ID == group.First().SERVICE_REQ_ID);
                            MPS.Processor.Mps000105.PDO.Mps000105PDO rdo = new MPS.Processor.Mps000105.PDO.Mps000105PDO(this.resultTranBill, group, currentPatientTypeAlter, serviceReq, patient, ratio_text);
                            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO });
                        }
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuThuTTChiTietDichVuNgoaiTru(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.resultTranBill == null)
                    return;
                WaitingManager.Show();
                var listSereServ = new List<V_HIS_SERE_SERV>();
                HisSereServBillViewFilter ssBillFilter = new HisSereServBillViewFilter();
                ssBillFilter.BILL_ID = this.resultTranBill.ID;
                var hisSSBills = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_BILL>>("api/HisSereServBill/GetView", ApiConsumers.MosConsumer, ssBillFilter, null);
                if (hisSSBills == null || hisSSBills.Count <= 0)
                {
                    throw new Exception("Khong lay duoc SereServBill theo BillId: " + this.resultTranBill.ID);
                }

                if (Print106Type != "1")
                {
                    HisSereServViewFilter ssFilter = new HisSereServViewFilter();
                    ssFilter.IDs = hisSSBills.Select(s => s.SERE_SERV_ID).ToList();
                    ssFilter.TREATMENT_ID = this.currentTransaction.TREATMENT_ID;
                    listSereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, ssFilter, null);

                    if (Print106Type_Expend == "1")
                    {
                        if (listSereServ != null && listSereServ.Count > 0)
                        {
                            HisSereServViewFilter ssFilter1 = new HisSereServViewFilter();
                            ssFilter1.TREATMENT_ID = this.currentTransaction.TREATMENT_ID;
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
                    ssFilter.TREATMENT_ID = this.currentTransaction.TREATMENT_ID;
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
                ptAlterAppFilter.TreatmentId = this.currentTransaction.TREATMENT_ID ?? 0;
                ptAlterAppFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, ptAlterAppFilter, null);

                // tính mức hưởng của thẻ
                string levelCode = LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT;
                string ratio_text = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentPatientTypeAlter.HEIN_CARD_NUMBER, currentPatientTypeAlter.LEVEL_CODE, currentPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100) + "";

                HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = this.treatmentFee.PATIENT_ID;
                var patients = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, null);
                V_HIS_PATIENT patient = new V_HIS_PATIENT();
                if (patients != null && patients.Count > 0)
                {
                    patient = patients.FirstOrDefault();
                }

                if (listSereServ != null && listSereServ.Count > 0)
                {
                    decimal totalDeposit = GetDepositAmount(currentTransaction.TREATMENT_ID);
                    MOS.EFMODEL.DataModels.HIS_TREATMENT treatment = GetTreatment(currentTransaction.TREATMENT_ID);

                    MPS.Processor.Mps000259.PDO.Mps000259ADO ado = new MPS.Processor.Mps000259.PDO.Mps000259ADO();
                    ado.PatientTypeBHYT = HisConfigCFG.PatientTypeId__BHYT;
                    ado.PatientTypeVP = HisConfigCFG.PatientTypeId__VP;

                    MPS.Processor.Mps000259.PDO.Mps000259PDO rdo = new MPS.Processor.Mps000259.PDO.Mps000259PDO(this.resultTranBill, listSereServ, hisSSBills, treatment, totalDeposit, totalCanThu, currentPatientTypeAlter, patient, ratio_text, ado, BackendDataWorker.Get<HIS_DEPARTMENT>());
                    rdo.ShowExpend = Print106Type_Expend == "1";
                    WaitingManager.Hide();

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                    if (isPrintNow)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuHoanUngThanhToanRaVien(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.treatmentFee == null)
                    return;
                WaitingManager.Show();
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }
                CommonParam paramtreatment = new CommonParam();
                HisTreatmentFeeViewFilter filterTreat = new HisTreatmentFeeViewFilter();
                filterTreat.ID = this.treatmentFee.ID;
                var TreatmentFee = new Inventec.Common.Adapter.BackendAdapter(paramtreatment).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, filterTreat, paramtreatment);

                CommonParam param1 = new CommonParam();
                HisDepartmentViewFilter filterDepar = new HisDepartmentViewFilter();
                filterDepar.ID = this.treatmentFee.END_DEPARTMENT_ID;
                var department = new BackendAdapter(param1).Get<List<V_HIS_DEPARTMENT>>("api/HisDepartment/GetView", ApiConsumers.MosConsumer, filterDepar, param1);

                HisTransactionViewFilter filterTran = new HisTransactionViewFilter();
                filterTran.TREATMENT_ID = treatmentFee.ID;
                filterTran.TRANSACTION_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT };
                filterTran.IS_CANCEL = false;
                List<V_HIS_TRANSACTION> transa = new BackendAdapter(param1).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, filterTran, param1);
                if (transa == null) transa = new List<V_HIS_TRANSACTION>();

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.treatmentFee != null ? this.treatmentFee.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                MPS.Processor.Mps000361.PDO.Mps000361PDO pdo = new MPS.Processor.Mps000361.PDO.Mps000361PDO(TreatmentFee.FirstOrDefault(), transa, department.FirstOrDefault());
                MPS.ProcessorBase.Core.PrintData printData = null;
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                }
                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(printData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
        }

        //private void InHoaDonNhap(string printTypeCode, string fileName, ref bool result)
        //{
        //    try
        //    {
        //        WaitingManager.Show();

        //        V_HIS_ACCOUNT_BOOK AccountBook = new V_HIS_ACCOUNT_BOOK();
        //        if (cboAccountBook.EditValue != null)
        //        {
        //            AccountBook = BackendDataWorker.Get<V_HIS_ACCOUNT_BOOK>().FirstOrDefault(o => o.ID == (long)cboAccountBook.EditValue);
        //        }

        //        V_HIS_TRANSACTION transaction = new V_HIS_TRANSACTION();

        //        transaction.BUYER_NAME = this.currentTransaction.BUYER_NAME;
        //        transaction.BUYER_TAX_CODE = this.currentTransaction.BUYER_TAX_CODE;
        //        transaction.BUYER_ACCOUNT_NUMBER = this.currentTransaction.BUYER_ACCOUNT_NUMBER;
        //        transaction.BUYER_ORGANIZATION = this.currentTransaction.BUYER_ORGANIZATION;
        //        transaction.BUYER_ADDRESS = this.currentTransaction.BUYER_ADDRESS;
        //        //transaction.BUYER_EMAIL = txtBuyerEmail.Text;
        //        if (dtTransactionTime.EditValue != null && dtTransactionTime.DateTime != DateTime.MinValue)
        //        {
        //            transaction.TRANSACTION_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dtTransactionTime.DateTime.ToString("yyyyMMdd") + "000000");
        //        }
        //        transaction.PAY_FORM_NAME = cboPayForm.Text;

        //        if (AccountBook != null)
        //        {
        //            transaction.SYMBOL_CODE = AccountBook.SYMBOL_CODE;
        //            transaction.TEMPLATE_CODE = AccountBook.TEMPLATE_CODE;
        //        }


        //        List<V_HIS_SERE_SERV_5> sereServBills = new List<V_HIS_SERE_SERV_5>();
        //        var sereServBillADOs = ssTreeProcessor.GetListCheck(this.ucSereServTree);

        //        if (sereServBillADOs != null && sereServBillADOs.Count > 0)
        //        {
        //            foreach (var item in sereServBillADOs)
        //            {
        //                V_HIS_SERE_SERV_5 sereServBill = new V_HIS_SERE_SERV_5();
        //                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV_5>(sereServBill, item);
        //                sereServBills.Add(sereServBill);
        //            }
        //        }

        //        MOS.EFMODEL.DataModels.HIS_TRANSACTION hisTransaction = new MOS.EFMODEL.DataModels.HIS_TRANSACTION();

        //        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_TRANSACTION>(hisTransaction, transaction);

        //        ElectronicBillDataInput dataInput = new ElectronicBillDataInput();

        //        dataInput.Amount = hisTransaction.AMOUNT;
        //        dataInput.Branch = LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == LocalStorage.LocalData.WorkPlace.GetBranchId());
        //        dataInput.Discount = hisTransaction.EXEMPTION;
        //        //dataInput.DiscountRatio = txtDiscountRatio.Value;
        //        dataInput.PaymentMethod = cboPayForm.Text;
        //        dataInput.SereServs = sereServBills;
        //        dataInput.Treatment = this.treatmentFee;
        //        dataInput.Currency = "VND";
        //        dataInput.Transaction = hisTransaction;
        //        var accountBook = ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
        //        if (accountBook != null)
        //        {
        //            dataInput.SymbolCode = accountBook.SYMBOL_CODE;
        //            dataInput.TemplateCode = accountBook.TEMPLATE_CODE;
        //            dataInput.EinvoiceTypeId = accountBook.EINVOICE_TYPE_ID;
        //        }

        //        if (dtTransactionTime.EditValue != null && dtTransactionTime.DateTime != DateTime.MinValue)
        //        {
        //            dataInput.TransactionTime = Convert.ToInt64(dtTransactionTime.DateTime.ToString("yyyyMMddHHmmss"));
        //        }

        //        long Template = long.Parse(AdjustmentTransactionConfig.InvoiceTemplateCreate);
        //        TemplateEnum.TYPE typ = TemplateEnum.TYPE.Template1;
        //        try
        //        {
        //            typ = (TemplateEnum.TYPE)Template;
        //        }
        //        catch (Exception)
        //        {
        //            typ = TemplateEnum.TYPE.Template1;
        //        }

        //        IRunTemplate iRunTemplate = TemplateFactory.MakeIRun(typ, dataInput);

        //        var listProduct = iRunTemplate.Run();

        //        List<MPS.Processor.Mps000431.PDO.ProductADO> lstProductADO = new List<MPS.Processor.Mps000431.PDO.ProductADO>();
        //        var lst = (List<ProductBase>)listProduct;
        //        foreach (var item in lst)
        //        {
        //            MPS.Processor.Mps000431.PDO.ProductADO ado = new MPS.Processor.Mps000431.PDO.ProductADO();
        //            Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000431.PDO.ProductADO>(ado, item);
        //            lstProductADO.Add(ado);
        //        }

        //        MPS.Processor.Mps000431.PDO.Mps000431PDO rdo = new MPS.Processor.Mps000431.PDO.Mps000431PDO(transaction, lstProductADO);

        //        WaitingManager.Hide();

        //        string printerName = "";
        //        if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
        //        {
        //            printerName = GlobalVariables.dicPrinter[printTypeCode];
        //        }

        //        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.treatmentFee != null ? this.treatmentFee.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

        //        if (isPrintNow)
        //        {
        //            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
        //        }
        //        else
        //        {
        //            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        WaitingManager.Hide();
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void InMps479(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExpMestFilter filter = new HisExpMestFilter();
                filter.TDL_TREATMENT_ID = treatmentFee.ID;
                filter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK;
                var data = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>>("api/HisExpMest/get", ApiConsumers.MosConsumer, filter, param);
                if (data != null && data.Count > 0)
                {

                    WaitingManager.Show();
                    foreach (var item in data)
                    {


                        MPS.Processor.Mps000479.PDO.Mps000479PDO rdo = new MPS.Processor.Mps000479.PDO.Mps000479PDO(item);

                        WaitingManager.Hide();

                        string printerName = "";
                        if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                        {
                            printerName = GlobalVariables.dicPrinter[printTypeCode];
                        }

                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.treatmentFee != null ? this.treatmentFee.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                        if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                        }
                        else
                        {
                            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private ElectronicBillResult TaoHoaDonDienTuBenThu3CungCap(MOS.EFMODEL.DataModels.HIS_TRANSACTION transaction)
        {
            ElectronicBillResult result = new ElectronicBillResult();
            try
            {
                List<V_HIS_SERE_SERV_5> sereServBills = new List<V_HIS_SERE_SERV_5>();
                var sereServBillADOs = ssTreeProcessor.GetListCheck(this.ucSereServTree);
                if (sereServBillADOs == null)
                {
                    result.Success = false;
                    LogSystem.Debug("Khong co dich vu thanh toan nao duoc chon!");
                    return result;
                }
                foreach (var item in sereServBillADOs)
                {
                    V_HIS_SERE_SERV_5 sereServBill = new V_HIS_SERE_SERV_5();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV_5>(sereServBill, item);
                    sereServBills.Add(sereServBill);
                }

                ElectronicBillDataInput dataInput = new ElectronicBillDataInput();
                dataInput.Amount = transaction.AMOUNT;
                dataInput.Branch = LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == LocalStorage.LocalData.WorkPlace.GetBranchId());
                dataInput.Discount = this.resultTranBill.EXEMPTION ?? 0;
                dataInput.DiscountRatio = ((this.resultTranBill.EXEMPTION / this.resultTranBill.AMOUNT) * 100) ?? 0;
                dataInput.PaymentMethod = cboPayForm.Text;
                dataInput.SereServs = sereServBills;
                dataInput.Treatment = this.treatmentFee;
                dataInput.Currency = "VND";
                dataInput.Transaction = transaction;
                var accountBook = ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                if (accountBook != null)
                {
                    dataInput.SymbolCode = accountBook.SYMBOL_CODE;
                    dataInput.TemplateCode = accountBook.TEMPLATE_CODE;
                    dataInput.EinvoiceTypeId = accountBook.EINVOICE_TYPE_ID;
                }

                if (dtTransactionTime.EditValue != null && dtTransactionTime.DateTime != DateTime.MinValue)
                {
                    dataInput.TransactionTime = Convert.ToInt64(dtTransactionTime.DateTime.ToString("yyyyMMddHHmmss"));
                }

                WaitingManager.Show();
                ElectronicBillProcessor electronicBillProcessor = new ElectronicBillProcessor(dataInput);
                result = electronicBillProcessor.Run(ElectronicBillType.ENUM.CREATE_INVOICE);
                WaitingManager.Hide();
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

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
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.ElectronicBill;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Plugins.TransactionBillTwoInOne.ADO;
using HIS.Desktop.Plugins.TransactionBillTwoInOne.Base;
using HIS.Desktop.Plugins.TransactionBillTwoInOne.Config;
using HIS.Desktop.Plugins.TransactionBillTwoInOne.Resources;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.DocumentViewer;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using WCF.Client;
using WCF;
using Inventec.WCF.JsonConvert;

namespace HIS.Desktop.Plugins.TransactionBillTwoInOne
{
    public partial class frmTransactionBillTwoInOne : HIS.Desktop.Utility.FormBase
    {

        enum ContainerClick
        {
            None,
            MoTaVienPhi,
            MoTaDichVu,
            LyDoVienPhi,
            LyDoDichVu
        }

        ContainerClick currentContainerClick = ContainerClick.None;
        bool isLuuKy = false;
        const string invoiceTypeCreate__CreateInvoiceVnpt = "1";
        const string invoiceTypeCreate__CreateInvoiceHIS = "2";

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSave.Enabled)
                    return;
                SetEnableButtonSave(false);
                ValidControl();
                positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                {
                    SetEnableButtonSave(true);
                    return;
                }
                ClearValidate();
                WaitingManager.Show();
                isSavePrint = false;
                isLuuKy = false;
                CommonParam param = new CommonParam();
                bool success = false;
                ProcessSave(ref param, ref success);
                WaitingManager.Hide();
                if (success)
                {
                    MessageManager.Show(this, param, success);
                }
                else
                {
                    MessageManager.Show(param, success);
                }
                SessionManager.ProcessTokenLost(param);
                btnNew.Focus();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSavePrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSavePrint.Enabled)
                    return;
                SetEnableButtonSave(false);
                ValidControl();
                positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                {
                    SetEnableButtonSave(true);
                    return;
                }
                ClearValidate();
                WaitingManager.Show();
                isSavePrint = true;
                isLuuKy = false;
                CommonParam param = new CommonParam();
                bool success = false;
                ProcessSave(ref param, ref success);
                if (success)
                {
                    this.onClickBienLaiThanhToan(null, null);
                    this.onClickHoaDonThanhToan(null, null);
                }
                WaitingManager.Hide();
                if (!success)
                {
                    MessageManager.Show(param, success);
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetEnableButtonSave(bool enable)
        {
            try
            {
                btnSave.Enabled = enable;
                btnSavePrint.Enabled = enable;
                BtnSaveAndSign.Enabled = enable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnNew.Enabled)
                    return;
                string FindTreatmentCode = txtSearch.Text;
                txtSearch.Text = "";
                btnSearch_Click(null, null);
                txtSearch.Text = FindTreatmentCode;
                txtSearch.Focus();
                txtSearch.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnConfigPrinter_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnConfigPrinter.Enabled)
                    return;
                var listPrintType = MPS.PrintConfig.PrintTypes.Where(o => o.PRINT_TYPE_CODE == PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuBienLaiThanhToan_MPS000148 || o.PRINT_TYPE_CODE == PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuHoaDonThanhToan_MPS000147).ToList();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ConfigPrinter").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ConfigPrinter'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(listPrintType);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("extenceInstance is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                    LoadConfigPrinter();
                }
                else
                {
                    MessageManager.Show(Base.ResourceMessageLang.ChucNangNayChuDuocHoTroTrongPhienBanNay);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
            btnNew.Focus();
        }

        private void bbtnRCNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnNew_Click(null, null);
        }

        private void bbtnRCConfigPrinter_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnConfigPrinter_Click(null, null);
        }

        private void bbtnRCSavePrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSavePrint_Click(null, null);
        }

        private void ProcessSave(ref CommonParam param, ref bool success)
        {
            try
            {
                //List<HisTransactionBillSDO> listSdo = new List<HisTransactionBillSDO>();
                HisTransactionBillTwoBookSDO billTwoBookSDO = new HisTransactionBillTwoBookSDO();
                V_HIS_ACCOUNT_BOOK recieptAccBook = null;
                V_HIS_ACCOUNT_BOOK invoiceAccBook = null;
                CARD.WCF.DCO.WcfSaleDCO saleDCO = null;
                if ((listInvoiceData == null || listInvoiceData.Count <= 0) && (listRecieptData == null || listRecieptData.Count <= 0))
                {
                    success = false;
                    param.Messages.Add(Base.ResourceMessageLang.NguoiDungChuaChonDichVuDeThanhToan);
                    return;
                }
                if (listRecieptData != null && listRecieptData.Count > 0 && !checkNotReciept.Checked)
                {
                    if (cboRecieptAccountBook.EditValue == null || cboPayForm.EditValue == null) return;
                    if (!GetSdoReciept(param, ref billTwoBookSDO, ref recieptAccBook, listRecieptData))
                        return;
                }

                if (listInvoiceData != null && listInvoiceData.Count > 0 && !checkNotInvoice.Checked)
                {
                    if (cboInvoiceAccountBook.EditValue == null || cboPayForm.EditValue == null) return;
                    if (!GetSdoInvoice(param, ref billTwoBookSDO, ref invoiceAccBook, listInvoiceData))
                        return;
                }

                billTwoBookSDO.TreatmentId = this.treatmentId.Value;
                billTwoBookSDO.RequestRoomId = this.currentModuleBase.RoomId;

                V_HIS_ACCOUNT_BOOK accountBookRepay = null;
                billTwoBookSDO.IsAutoRepay = checkIsAutoRepay.Checked;
                if (cboRepayAccountBook.EditValue != null)
                {
                    accountBookRepay = this.ListAccountBookRepay != null && this.ListAccountBookRepay.Count > 0 ? this.ListAccountBookRepay.FirstOrDefault(o => o.ID == Convert.ToInt64(cboRepayAccountBook.EditValue)) : null;
                    if (accountBookRepay != null)
                    {
                        billTwoBookSDO.RepayAccountBookId = accountBookRepay.ID;
                    }

                    if (accountBookRepay != null && accountBookRepay.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                    {
                        billTwoBookSDO.RepayNumOrder = (long)spinRepayNumOrder.Value;
                    }
                }

                if ((long)cboPayForm.EditValue == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__KEYPAY)
                {
                    bool hasPaymentKEYPAY = false;
                    if (!this.CheckPayFormKEYPAY(billTwoBookSDO, ref param, ref hasPaymentKEYPAY))
                    {
                        success = false;
                        return;
                    }
                    Inventec.Common.Logging.LogSystem.Info("this.CheckPayFormKEYPAY(billTwoBookSDO, ref param, ref hasPaymentKEYPAY): " + this.CheckPayFormKEYPAY(billTwoBookSDO, ref param, ref hasPaymentKEYPAY));

                    if (hasPaymentKEYPAY)
                    {
                        CommonParam checkParam = new CommonParam();
                        var check = new BackendAdapter(checkParam).Post<bool>("api/HisTransaction/CheckBillTwoBook", ApiConsumers.MosConsumer, billTwoBookSDO, checkParam);
                        if (!check)
                        {
                            param = checkParam;
                            success = false;
                            return;
                        }

                        HisTransReqBillTwoBookSDO reqSdo1 = this.MapTranToReq(billTwoBookSDO, null);

                        var resultData = new BackendAdapter(param).Post<List<HIS_TRANS_REQ>>("api/HisTransReq/CreateBillTwoBook", ApiConsumers.MosConsumer, reqSdo1, param);

                        Inventec.Common.Logging.LogSystem.Info("resultData: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultData), resultData));

                        if (resultData != null && resultData.Count > 0)
                        {
                            success = true;
                            param.Messages.Add(" Yêu cầu thanh toán đã được ghi nhận");

                            RefreshSessionInfo();

                            ddBtnPrint.Enabled = true;
                            btnSavePrint.Enabled = false;
                            btnSave.Enabled = false;
                        }
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info("không dùng keypay");
                    if ((long)cboPayForm.EditValue == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QUET_THE && chkConnectPos.Checked && decimal.Parse(lblCanThu.Text) > 0)
                    {                      
                        CommonParam checkParam = new CommonParam();
                        var check = new BackendAdapter(checkParam).Post<bool>("api/HisTransaction/CheckBillTwoBook", ApiConsumers.MosConsumer, billTwoBookSDO, checkParam);
                        if (!check)
                        {
                            param = checkParam;
                            success = false;
                            return;
                        }
                        OpenAppPOS();
                        WcfRequest wc = new WcfRequest();
                        if ((long)cboPayForm.EditValue == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QUET_THE && decimal.Parse(lblCanThu.Text) > 0)
                        {
                            wc.AMOUNT = (long)decimal.Parse(lblCanThu.Text);
                        }
                        wc.billId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);
                        wc.creator = creator;
                        var json = JsonConvert.Serialize<WcfRequest>(wc);
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => json), json));
                        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(json);
                        try
                        {
                            if (cll == null)
                            {
                                cll = new WcfClient();
                            }
                        }
                        catch (Exception ex)
                        {
                            success = false;
                            XtraMessageBox.Show("Kiểm tra lại cấu hình NetTcpBinding_IService1", "Thông báo");
                            Inventec.Common.Logging.LogSystem.Error(ex);
                            return;
                        }
                        var result = cll.Sale(System.Convert.ToBase64String(plainTextBytes));

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
                        if (result != null && result.RESPONSE_CODE == "00")
                        {
                            if (billTwoBookSDO.InvoiceTransaction != null)
                            {
                                Inventec.Common.Logging.LogSystem.Info("billTwoBookSDO.InvoiceTransaction");
                                billTwoBookSDO.InvoiceTransaction.POS_PAN = result.PAN;
                                billTwoBookSDO.InvoiceTransaction.POS_CARD_HOLDER = result.NAME;
                                billTwoBookSDO.InvoiceTransaction.POS_INVOICE = result.INVOICE.ToString();
                                billTwoBookSDO.InvoiceTransaction.POS_RESULT_JSON = JsonConvert.Serialize<WcfRequest>(result);
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => billTwoBookSDO.InvoiceTransaction), billTwoBookSDO.InvoiceTransaction));
                            }
                            if (billTwoBookSDO.RecieptTransaction != null)
                            {
                                Inventec.Common.Logging.LogSystem.Info("billTwoBookSDO.RecieptTransaction");
                                billTwoBookSDO.RecieptTransaction.POS_PAN = result.PAN;
                                billTwoBookSDO.RecieptTransaction.POS_CARD_HOLDER = result.NAME;
                                billTwoBookSDO.RecieptTransaction.POS_INVOICE = result.INVOICE.ToString();
                                billTwoBookSDO.RecieptTransaction.POS_RESULT_JSON = JsonConvert.Serialize<WcfRequest>(result);
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => billTwoBookSDO.RecieptTransaction), billTwoBookSDO.RecieptTransaction));
                            }
                        }
                        else
                        {
                            if (result != null)
                            {
                                Inventec.Common.Logging.LogSystem.Info("1_____________");
                                WaitingManager.Hide();
                                if (DevExpress.XtraEditors.XtraMessageBox.
                               Show("Lỗi thanh toán. " + "(Mã lỗi: " + result.ERROR + ")", "Thông báo", System.Windows.Forms.MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK)
                                    SetEnableButtonSave(true);
                                    return ;
                            }
                            Inventec.Common.Logging.LogSystem.Info("2_____________");
                            SetEnableButtonSave(true);
                            return;
                        }
                    }

                    bool hasPaymentCard = false;
                    if (!this.CheckPayFormCard(billTwoBookSDO, ref param, ref hasPaymentCard))
                    {
                        success = false;
   //                     return;
                    }

                    if (hasPaymentCard)
                    {
                        CommonParam checkParam = new CommonParam();
                        var check = new BackendAdapter(checkParam).Post<bool>("api/HisTransaction/CheckBillTwoBook", ApiConsumers.MosConsumer, billTwoBookSDO, checkParam);
                        if (!check)
                        {
                            param = checkParam;
                            success = false;
                            return;
                        }

                        decimal canThuthem = 0;

                        if (billTwoBookSDO.RecieptTransaction != null)
                        {
                            decimal tong = billTwoBookSDO.RecieptPayAmount;
                            decimal chietkhau = billTwoBookSDO.RecieptTransaction.EXEMPTION ?? 0;
                            decimal quy = 0;
                            if (billTwoBookSDO.RecieptTransaction.HIS_BILL_FUND != null && billTwoBookSDO.RecieptTransaction.HIS_BILL_FUND.Count > 0)
                            {
                                quy = billTwoBookSDO.RecieptTransaction.HIS_BILL_FUND.Sum(s => s.AMOUNT);
                            }
                            canThuthem += (tong - (chietkhau + quy));
                        }

                        if (billTwoBookSDO.InvoiceTransaction != null)
                        {
                            decimal tong = billTwoBookSDO.InvoicePayAmount;
                            decimal chietkhau = billTwoBookSDO.InvoiceTransaction.EXEMPTION ?? 0;
                            decimal quy = 0;
                            if (billTwoBookSDO.InvoiceTransaction.HIS_BILL_FUND != null && billTwoBookSDO.InvoiceTransaction.HIS_BILL_FUND.Count > 0)
                            {
                                quy = billTwoBookSDO.InvoiceTransaction.HIS_BILL_FUND.Sum(s => s.AMOUNT);
                            }
                            canThuthem += (tong - (chietkhau + quy));
                        }

                        HisTransReqBillTwoBookSDO reqSdo = this.MapTranToReq(billTwoBookSDO, saleDCO);
                        LogSystem.Debug(LogUtil.TraceData("reqSdo", reqSdo));
                        if (canThuthem > 0)
                        {
                            CARD.WCF.DCO.WcfSaleDCO SaleDCO = new CARD.WCF.DCO.WcfSaleDCO();
                            SaleDCO.Amount = canThuthem;
                            saleDCO = SaleCard(ref SaleDCO, param, recieptAccBook, invoiceAccBook);
                            // nếu gọi sang POS trả về false thì kết thúc
                            Inventec.Common.Logging.LogSystem.Debug("saleDCO output: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => saleDCO), saleDCO));
                            if (saleDCO == null || (saleDCO.ResultCode == null || !saleDCO.ResultCode.Equals("00")))
                            {
                                success = false;
                                if (saleDCO != null && saleDCO.IsCreateTransReqSuccess)
                                {
                                    reqSdo = this.MapTranToReq(billTwoBookSDO, saleDCO);

                                    List<HIS_TRANS_REQ> lstTransReq = new BackendAdapter(param).Post<List<HIS_TRANS_REQ>>("api/HisTransReq/CreateBillTwoBook", ApiConsumers.MosConsumer, reqSdo, param);
                                    if (lstTransReq != null && lstTransReq.Count > 0)
                                    {
                                        param.Messages.Add("Thẻ không đủ số dư. Yêu cầu thanh toán đã được ghi nhận");
                                        return;
                                    }
                                }

                                MappingErrorTHE mappingErrorTHE = new Config.MappingErrorTHE();
                                success = false;
                                Inventec.Common.Logging.LogSystem.Info("saleDCO: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => saleDCO), saleDCO));
                                //param.Messages.Add(ResourceMessageLang.ThanhToanTheThatBai);
                                if (saleDCO != null
                                    && !String.IsNullOrWhiteSpace(saleDCO.ResultCode)
                                    && mappingErrorTHE.dicMapping != null
                                    && mappingErrorTHE.dicMapping.Count > 0
                                    && mappingErrorTHE.dicMapping.ContainsKey(saleDCO.ResultCode))
                                {
                                    param.Messages.Add(mappingErrorTHE.dicMapping[saleDCO.ResultCode]);
                                }
                                else if (saleDCO != null && String.IsNullOrWhiteSpace(saleDCO.ResultCode))
                                {
                                    param.Messages.Add("Kiểm tra lại phần mềm kết nối thiết bị");
                                }
                                else if (saleDCO != null
                                    && !String.IsNullOrWhiteSpace(saleDCO.ResultCode)
                                    && mappingErrorTHE.dicMapping != null
                                    && mappingErrorTHE.dicMapping.Count > 0
                                    && !mappingErrorTHE.dicMapping.ContainsKey(saleDCO.ResultCode)
                                    )
                                {
                                    param.Messages.Add("Kiểm tra lại phần mềm kết nối thiết bị");
                                }
                                success = false;
                                return;
                            }
                            else
                            {
                                if (billTwoBookSDO.RecieptTransaction != null)
                                {
                                    billTwoBookSDO.RecieptTransaction.TIG_TRANSACTION_CODE = saleDCO.TransactionCode;
                                    billTwoBookSDO.RecieptTransaction.TIG_TRANSACTION_TIME = saleDCO.TransactionTime;
                                }
                                if (billTwoBookSDO.InvoiceTransaction != null)
                                {
                                    billTwoBookSDO.InvoiceTransaction.TIG_TRANSACTION_CODE = saleDCO.TransactionCode;
                                    billTwoBookSDO.InvoiceTransaction.TIG_TRANSACTION_TIME = saleDCO.TransactionTime;
                                }
                            }
                        }
                        else
                        {
                            WaitingManager.Hide();
                            param.Messages.Add("Số tiền phải thu bệnh nhân <= 0. Bạn không thể chọn hình thức thanh toán qua thẻ. ");
                            return;
                        }
                    }

                    LogSystem.Debug(LogUtil.TraceData("billTwoBookSDO", billTwoBookSDO));

                    var rs = new BackendAdapter(param).Post<List<V_HIS_TRANSACTION>>("api/HisTransaction/CreateBillTwoBook", ApiConsumers.MosConsumer, billTwoBookSDO, param);
                    if (rs != null && rs.Count > 0)
                    {
                        success = true;
                        btnSave.Enabled = false;
                        btnSavePrint.Enabled = false;
                        ddBtnPrint.Enabled = true;
                        var exits = rs.Where(s => s.PAY_FORM_ID == 8 && s.IS_ACTIVE == 0);
                        if(isLuuKy)
                        {
                            if (isCreateQRContinue && (long)cboPayForm.EditValue == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR)
                            {

                                this.listTranToQR = exits.ToList();
                                btnQR.Enabled = true;
                                CreateQR(exits.ToList(), false);
                            }
                        }
                        else
                        {

                            if (exits != null && (long)cboPayForm.EditValue == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR)
                            {
                                this.listTranToQR = exits.ToList();
                                btnQR.Enabled = true;
                                CreateQR(exits.ToList(), false);
                            }
                        }
                        
                        //AddLastAccountToLocal();
                        bool resetReceipt = false;
                        bool resetInvoice = false;
                        foreach (var item in rs)
                        {
                            if (isLuuKy && TransactionBillConfig.InvoiceTypeCreate == invoiceTypeCreate__CreateInvoiceVnpt && item.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT)
                            {
                                //Tao hoa don dien thu ben thu3
                                ElectronicBillResult electronicBillResult = null;
                                if ((long)cboPayForm.EditValue != IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR)
                                     electronicBillResult = TaoHoaDonDienTuBenThu3CungCap(item);
                                if ((long)cboPayForm.EditValue != IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR && (electronicBillResult == null || !electronicBillResult.Success))
                                {
                                    param.Messages.Add("Tạo hóa đơn điện tử thất bại");
                                    if (electronicBillResult.Messages != null && electronicBillResult.Messages.Count > 0)
                                    {
                                        param.Messages.AddRange(electronicBillResult.Messages);
                                    }

                                    param.Messages = param.Messages.Distinct().ToList();

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
                                    sdo.Id = item.ID;
                                    sdo.InvoiceLookupCode = electronicBillResult.InvoiceLookupCode;
                                    var apiResult = new BackendAdapter(paramUpdate).Post<bool>("api/HisTransaction/UpdateInvoiceInfo", ApiConsumers.MosConsumer, sdo, paramUpdate);
                                    {
                                        item.INVOICE_CODE = electronicBillResult.InvoiceCode;
                                        item.INVOICE_SYS = electronicBillResult.InvoiceSys;
                                        item.EINVOICE_NUM_ORDER = electronicBillResult.InvoiceNumOrder;
                                        item.EINVOICE_LOGINNAME = electronicBillResult.InvoiceLoginname;
                                        item.EINVOICE_TIME = electronicBillResult.InvoiceTime ?? (Inventec.Common.DateTime.Get.Now() ?? 0);
                                    }
                                }
                            }

                            if (item.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__HU)
                            {
                                if (accountBookRepay != null && accountBookRepay.IS_NOT_GEN_TRANSACTION_ORDER != 1)
                                    spinRepayNumOrder.Value = item.NUM_ORDER;
                                UpdateDictionaryNumOrderAccountBook(false, false, true);
                            }
                            else if (item.BILL_TYPE_ID == 1 || item.BILL_TYPE_ID == null)
                            {
                                resultRecieptBill = item;
                                resetReceipt = true;
                            }
                            else if (item.BILL_TYPE_ID == 2)
                            {
                                resultInvoiceBill = item;
                                resetInvoice = true;
                            }
                            else
                            {
                                LogSystem.Info("Khong xac dinh duoc ket qua Bill tra ve la bien lai hay hoa don Bill_Type_id: " + LogUtil.TraceData("item", item));
                            }
                        }
                        UpdateDictionaryNumOrderAccountBook(resetReceipt, resetInvoice, false);
                        this.RefreshSessionInfo();
                    }
                    else
                    {
                        SetEnableButtonSave(true);
                        if (saleDCO != null)
                        {
                            CARD.WCF.DCO.WcfVoidDCO WcfVoidDCO = new CARD.WCF.DCO.WcfVoidDCO();
                            WcfVoidDCO.Amount = this.totalPatientPrice;
                            WcfVoidDCO.TransactionCode = saleDCO.TransactionCode;
                            var resultWcf = VoidCard(ref WcfVoidDCO);
                            if (resultWcf == null || (resultWcf != null && !resultWcf.ResultCode.Equals("00")))
                            {
                                success = false;
                                Inventec.Common.Logging.LogSystem.Info("[result code]: " + resultWcf.ResultCode);
                                param.Messages.Add(ResourceMessageLang.HuyGiaoDichThanhToanTheThatBai + WcfVoidDCO.TransactionCode);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckPayFormCard(HisTransactionBillTwoBookSDO sdo, ref CommonParam param, ref bool hasPaymentCard)
        {
            if (sdo.RecieptTransaction != null && sdo.RecieptTransaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE)
            {
                if (sdo.InvoiceTransaction != null && sdo.InvoiceTransaction.PAY_FORM_ID != IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE)
                {
                    param.Messages.Add(ResourceMessageLang.KhongChoPhepHaiHoaDonCoHinhThucThanhToanKhacNhau);
                    return false;
                }

                hasPaymentCard = true;
            }

            if (sdo.InvoiceTransaction != null && sdo.InvoiceTransaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE)
            {
                if (sdo.RecieptTransaction != null && sdo.RecieptTransaction.PAY_FORM_ID != IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE)
                {
                    param.Messages.Add(ResourceMessageLang.KhongChoPhepHaiHoaDonCoHinhThucThanhToanKhacNhau);
                    return false;
                }
                hasPaymentCard = true;
            }
            return true;
        }

        private bool CheckPayFormKEYPAY(HisTransactionBillTwoBookSDO sdo, ref CommonParam param, ref bool hasPaymentKEYPAY)
        {
            if (sdo.RecieptTransaction != null && sdo.RecieptTransaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__KEYPAY)
            {
                if (sdo.InvoiceTransaction != null && sdo.InvoiceTransaction.PAY_FORM_ID != IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__KEYPAY)
                {
                    param.Messages.Add(ResourceMessageLang.KhongChoPhepHaiHoaDonCoHinhThucThanhToanKhacNhau);
                    return false;
                }

                hasPaymentKEYPAY = true;
            }

            if (sdo.InvoiceTransaction != null && sdo.InvoiceTransaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__KEYPAY)
            {
                if (sdo.RecieptTransaction != null && sdo.RecieptTransaction.PAY_FORM_ID != IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__KEYPAY)
                {
                    param.Messages.Add(ResourceMessageLang.KhongChoPhepHaiHoaDonCoHinhThucThanhToanKhacNhau);
                    return false;
                }
                hasPaymentKEYPAY = true;
            }
            return true;
        }

        // gọi WCF (phần mềm thẻ) để hủy giao dịch 
        CARD.WCF.DCO.WcfVoidDCO VoidCard(ref CARD.WCF.DCO.WcfVoidDCO VoidDCO)
        {
            CARD.WCF.DCO.WcfVoidDCO result = null;
            CommonParam param = new CommonParam();
            try
            {
                CARD.WCF.Client.TransactionClient.TransactionClientManager transactionClientManager = new CARD.WCF.Client.TransactionClient.TransactionClientManager();
                result = transactionClientManager.Void(VoidDCO);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        // gọi sang WCF thanh toán qua thẻ
        CARD.WCF.DCO.WcfSaleDCO SaleCard(ref CARD.WCF.DCO.WcfSaleDCO SaleDCO, CommonParam param, V_HIS_ACCOUNT_BOOK recieptAccBook, V_HIS_ACCOUNT_BOOK invoiceAccBook)
        {
            CARD.WCF.DCO.WcfSaleDCO result = null;
            try
            {
                // gọi api HisCard/Get để lấy về serviceCodes
                MOS.Filter.HisCardFilter cardFilter = new HisCardFilter();
                cardFilter.PATIENT_ID = this.treatment.PATIENT_ID;
                var HisCards = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_CARD>>("api/HisCard/Get", ApiConsumer.ApiConsumers.MosConsumer, cardFilter, param);
                if (HisCards != null && HisCards.Count > 0)
                {
                    SaleDCO.ServiceCodes = HisCards.Select(o => o.SERVICE_CODE).ToArray();
                }

                SaleDCO.ClientTrace = Guid.NewGuid().ToString();
                if (!HisConfig.IsUsingPaylater)
                {
                    SaleDCO.IsCreateReqWhenBalanceIsInsufficient = false;
                }
                else
                {
                    if (recieptAccBook != null && recieptAccBook.IS_NOT_GEN_TRANSACTION_ORDER.HasValue && recieptAccBook.IS_NOT_GEN_TRANSACTION_ORDER.Value == (short)1)
                        SaleDCO.IsCreateReqWhenBalanceIsInsufficient = false;
                    else if (invoiceAccBook != null && invoiceAccBook.IS_NOT_GEN_TRANSACTION_ORDER.HasValue && invoiceAccBook.IS_NOT_GEN_TRANSACTION_ORDER.Value == (short)1)
                        SaleDCO.IsCreateReqWhenBalanceIsInsufficient = false;
                    else
                        SaleDCO.IsCreateReqWhenBalanceIsInsufficient = true;
                }

                CARD.WCF.Client.TransactionClient.TransactionClientManager transactionClientManager = new CARD.WCF.Client.TransactionClient.TransactionClientManager();

                result = transactionClientManager.Sale(SaleDCO);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void RefreshSessionInfo()
        {
            try
            {
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

        private void UpdateDictionaryNumOrderAccountBook(bool resetReceipt, bool resetInvoice, bool resetRepay)
        {
            try
            {
                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook != null && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count > 0)
                {
                    if (resetInvoice)
                    {
                        long invoiceAccId = Convert.ToInt64(cboInvoiceAccountBook.EditValue);
                        if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.ContainsKey(invoiceAccId))
                            HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[invoiceAccId] = spinInvoiceNumOrder.Value;
                    }

                    if (resetReceipt)
                    {
                        long recieptAccId = Convert.ToInt64(cboRecieptAccountBook.EditValue);
                        if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.ContainsKey(recieptAccId))
                            HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[recieptAccId] = spinRecieptNumOrder.Value;
                    }
                    if (resetRepay)
                    {
                        long repayAccId = Convert.ToInt64(cboRepayAccountBook.EditValue);
                        if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.ContainsKey(repayAccId))
                            HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[repayAccId] = spinRepayNumOrder.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessAddLastAccount()
        {
            System.Threading.Thread add = new System.Threading.Thread(AddLastAccountToLocal);
            try
            {
                add.Start();
            }
            catch (Exception ex)
            {
                add.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AddLastAccountToLocal()
        {
            try
            {
                if (GlobalVariables.LastAccountBook == null) GlobalVariables.LastAccountBook = new List<V_HIS_ACCOUNT_BOOK>();

                var recieptAccountBook = listRecieptAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboRecieptAccountBook.EditValue));
                if (recieptAccountBook != null)
                {
                    //sổ chưa có sẽ add thêm vào
                    //xóa bỏ sổ cũ cùng loại
                    if (!GlobalVariables.LastAccountBook.Exists(o => o.ID == recieptAccountBook.ID))
                    {
                        var lstSameType = GlobalVariables.LastAccountBook.Where(o => o.IS_FOR_BILL == 1 && o.ID != recieptAccountBook.ID && o.BILL_TYPE_ID != 2).ToList();
                        if (lstSameType != null && lstSameType.Count > 0)
                        {
                            foreach (var item in lstSameType)
                            {
                                GlobalVariables.LastAccountBook.Remove(item);
                            }
                        }
                        GlobalVariables.LastAccountBook.Add(recieptAccountBook);
                    }
                }

                var invoiceAccountBook = listInvoiceAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboInvoiceAccountBook.EditValue));
                if (invoiceAccountBook != null)
                {
                    //sổ chưa có sẽ add thêm vào
                    //xóa bỏ sổ cũ cùng loại
                    if (!GlobalVariables.LastAccountBook.Exists(o => o.ID == invoiceAccountBook.ID))
                    {
                        var lstSameType = GlobalVariables.LastAccountBook.Where(o => o.IS_FOR_BILL == 1 && o.ID != invoiceAccountBook.ID && o.BILL_TYPE_ID == 2).ToList();
                        if (lstSameType != null && lstSameType.Count > 0)
                        {
                            foreach (var item in lstSameType)
                            {
                                GlobalVariables.LastAccountBook.Remove(item);
                            }
                        }
                        GlobalVariables.LastAccountBook.Add(invoiceAccountBook);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool GetSdoReciept(CommonParam param, ref HisTransactionBillTwoBookSDO billTwoBookSDO, ref V_HIS_ACCOUNT_BOOK accountBook, List<VHisSereServADO> listReciept)
        {
            try
            {
                decimal totalRecieptPrice = listReciept.Sum(o => (o.RecieptPrice ?? 0));
                if (spinRecieptDiscountPrice.EditValue != null)
                {
                    if (totalRecieptPrice > 0)
                    {
                        spinRecieptDiscountRatio.Value = (spinRecieptDiscountPrice.Value / totalRecieptPrice) * 100;
                        CalcuCanThu(false);
                    }
                }
                else if (spinRecieptDiscountRatio.EditValue != spinRecieptDiscountRatio.OldEditValue)
                {
                    spinRecieptDiscountPrice.Value = (spinRecieptDiscountRatio.Value * totalRecieptPrice) / 100;
                    CalcuCanThu(false);
                }
                billTwoBookSDO.RecieptTransaction = new HIS_TRANSACTION();
                billTwoBookSDO.RecieptTransaction.TREATMENT_ID = this.treatmentId.Value;
                billTwoBookSDO.RecieptTransaction.CASHIER_ROOM_ID = this.cashierRoom.ID;

                var payForm = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboPayForm.EditValue));
                if (payForm != null)
                {
                    billTwoBookSDO.RecieptTransaction.PAY_FORM_ID = payForm.ID;
                }

                accountBook = listRecieptAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboRecieptAccountBook.EditValue));
                if (accountBook != null)
                {
                    billTwoBookSDO.RecieptTransaction.ACCOUNT_BOOK_ID = accountBook.ID;
                    if (accountBook.IS_NOT_GEN_TRANSACTION_ORDER == 1
                        && spinRecieptNumOrder.Value >= 0
                        && HisConfig.OLD_SYSTEM__OPTION != "1")
                    {
                        billTwoBookSDO.RecieptTransaction.NUM_ORDER = (long)spinRecieptNumOrder.Value;
                    }
                }

                billTwoBookSDO.RecieptTransaction.AMOUNT = totalRecieptPrice;
                if (dtTransactionTime.EditValue != null && dtTransactionTime.DateTime != DateTime.MinValue)
                {
                    billTwoBookSDO.RecieptTransaction.TRANSACTION_TIME = Convert.ToInt64(dtTransactionTime.DateTime.ToString("yyyyMMddHHmmss"));
                }
                billTwoBookSDO.RecieptTransaction.EXEMPTION = Math.Round(spinRecieptDiscountPrice.Value, 4);
                billTwoBookSDO.RecieptTransaction.EXEMPTION_REASON = txtRecieptReason.Text;
                billTwoBookSDO.RecieptTransaction.DESCRIPTION = txtRecieptDescription.Text;
                billTwoBookSDO.RecieptTransaction.BUYER_ACCOUNT_NUMBER = txtBuyerAccountCode.Text;
                
                if (chkBuyerInfo.Checked)
                {
                    billTwoBookSDO.RecieptTransaction.BUYER_ADDRESS = txtBuyerAddress.Text;
                    billTwoBookSDO.RecieptTransaction.BUYER_NAME = txtBuyerName.Text;
                    billTwoBookSDO.RecieptTransaction.BUYER_ORGANIZATION = txtBuyerOrganization.Text;
                    billTwoBookSDO.RecieptTransaction.BUYER_TAX_CODE = txtBuyerTaxCode.Text;
                    //qtcode
                    billTwoBookSDO.RecieptTransaction.BUYER_EMAIL = txtBuyerEmail.Text;
                    //qtcode
                    billTwoBookSDO.RecieptTransaction.BUYER_TYPE = 1;
                }
                else if (chkOrganizationInfo.Checked)
                {
                    billTwoBookSDO.RecieptTransaction.BUYER_ADDRESS = txtBuyerAddress2.Text;
                    billTwoBookSDO.RecieptTransaction.BUYER_NAME = txtBuyerName2.Text;
                    billTwoBookSDO.RecieptTransaction.BUYER_ORGANIZATION = txtBuyerOrganization2.Text;
                    billTwoBookSDO.RecieptTransaction.BUYER_TAX_CODE = txtBuyerTaxCode2.Text;

                    billTwoBookSDO.RecieptTransaction.BUYER_TYPE = 2;
                }
                billTwoBookSDO.RecieptSereServBills = new List<HIS_SERE_SERV_BILL>();
                foreach (var item in listReciept)
                {
                    HIS_SERE_SERV_BILL ssBill = new HIS_SERE_SERV_BILL();
                    ssBill.SERE_SERV_ID = item.ID;
                    ssBill.PRICE = (item.RecieptPrice ?? 0);
                    billTwoBookSDO.RecieptSereServBills.Add(ssBill);
                }

                decimal totalRecieptFund = 0;
                var listRecieptFund = bindingSource1.DataSource as List<VHisBillFundADO>;
                if (listRecieptFund != null && listRecieptFund.Count > 0)
                {
                    billTwoBookSDO.RecieptTransaction.HIS_BILL_FUND = new List<HIS_BILL_FUND>();
                    foreach (var item in listRecieptFund)
                    {
                        if (item.AMOUNT > 0 && item.FUND_ID > 0)
                        {
                            HIS_BILL_FUND billFund = new HIS_BILL_FUND();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_BILL_FUND>(billFund, item);
                            billTwoBookSDO.RecieptTransaction.HIS_BILL_FUND.Add(billFund);
                        }
                    }
                    if (billTwoBookSDO.RecieptTransaction.HIS_BILL_FUND.Count == 0)
                    {
                        billTwoBookSDO.RecieptTransaction.HIS_BILL_FUND = null;
                    }
                    else
                    {
                        totalRecieptFund = billTwoBookSDO.RecieptTransaction.HIS_BILL_FUND.Sum(s => s.AMOUNT);
                    }
                }
                if (checkIsKC.CheckState == CheckState.Checked)
                {
                    if (totalHienDu >= (totalRecieptPrice - (spinRecieptDiscountPrice.Value + totalRecieptFund)))
                    {
                        billTwoBookSDO.RecieptTransaction.KC_AMOUNT = (totalRecieptPrice - (spinRecieptDiscountPrice.Value + totalRecieptFund));
                        billTwoBookSDO.RecieptPayAmount = 0;
                    }
                    else
                    {
                        billTwoBookSDO.RecieptTransaction.KC_AMOUNT = totalHienDu;
                        billTwoBookSDO.RecieptPayAmount = (totalRecieptPrice - (spinRecieptDiscountPrice.Value + totalRecieptFund)) - totalHienDu;
                    }
                    if (totalHienDu == 0)
                    {
                        billTwoBookSDO.RecieptTransaction.KC_AMOUNT = null;
                        billTwoBookSDO.RecieptPayAmount = (totalRecieptPrice - (spinRecieptDiscountPrice.Value + totalRecieptFund));
                    }
                }
                else
                {
                    billTwoBookSDO.RecieptTransaction.KC_AMOUNT = null;
                    billTwoBookSDO.RecieptPayAmount = (totalRecieptPrice - (spinRecieptDiscountPrice.Value + totalRecieptFund));
                }

                totalHienDu -= (billTwoBookSDO.RecieptTransaction.KC_AMOUNT ?? 0);
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private bool GetSdoInvoice(CommonParam param, ref HisTransactionBillTwoBookSDO billTwoBookSDO, ref V_HIS_ACCOUNT_BOOK accountBook, List<VHisSereServADO> listInvoice)
        {
            try
            {
                decimal totalInvoicePrice = listInvoice.Sum(o => o.InvoicePrice ?? 0);
                if (spinInvoiceDiscountPrice.EditValue != null)
                {
                    if (totalInvoicePrice > 0)
                    {
                        spinInvoiceDiscountRatio.Value = (spinInvoiceDiscountPrice.Value / totalInvoicePrice) * 100;
                        CalcuCanThu(false);
                    }
                }
                else if (spinInvoiceDiscountRatio.EditValue != spinInvoiceDiscountRatio.OldEditValue)
                {
                    spinInvoiceDiscountPrice.Value = (spinInvoiceDiscountRatio.Value * totalInvoicePrice) / 100;
                    CalcuCanThu(false);
                }
                billTwoBookSDO.InvoiceTransaction = new HIS_TRANSACTION();
                billTwoBookSDO.InvoiceTransaction.TREATMENT_ID = this.treatmentId.Value;
                billTwoBookSDO.InvoiceTransaction.CASHIER_ROOM_ID = this.cashierRoom.ID;

                var payForm = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboPayForm.EditValue));
                if (payForm != null)
                {
                    billTwoBookSDO.InvoiceTransaction.PAY_FORM_ID = payForm.ID;
                }

                accountBook = listInvoiceAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboInvoiceAccountBook.EditValue));
                if (accountBook != null)
                {
                    billTwoBookSDO.InvoiceTransaction.ACCOUNT_BOOK_ID = accountBook.ID;
                    if (accountBook.IS_NOT_GEN_TRANSACTION_ORDER == 1
                        && spinInvoiceNumOrder.Value >= 0
                        && HisConfig.OLD_SYSTEM__OPTION != "1")
                    {
                        billTwoBookSDO.InvoiceTransaction.NUM_ORDER = (long)spinInvoiceNumOrder.Value;
                    }
                }

                billTwoBookSDO.InvoiceTransaction.AMOUNT = totalInvoicePrice;
                if (dtTransactionTime.EditValue != null && dtTransactionTime.DateTime != DateTime.MinValue)
                {
                    billTwoBookSDO.InvoiceTransaction.TRANSACTION_TIME = Convert.ToInt64(dtTransactionTime.DateTime.ToString("yyyyMMddHHmm") + "00");
                }
                billTwoBookSDO.InvoiceTransaction.EXEMPTION = Math.Round(spinInvoiceDiscountPrice.Value, 4);
                billTwoBookSDO.InvoiceTransaction.EXEMPTION_REASON = txtInvoiceReason.Text;
                billTwoBookSDO.InvoiceTransaction.DESCRIPTION = txtInvoiceDescription.Text;
                billTwoBookSDO.InvoiceTransaction.BUYER_ACCOUNT_NUMBER = txtBuyerAccountCode.Text;
                
                if (chkBuyerInfo.Checked)
                {
                    billTwoBookSDO.InvoiceTransaction.BUYER_NAME = txtBuyerName.Text;
                    billTwoBookSDO.InvoiceTransaction.BUYER_ADDRESS = txtBuyerAddress.Text;
                    billTwoBookSDO.InvoiceTransaction.BUYER_ORGANIZATION = txtBuyerOrganization.Text;
                    billTwoBookSDO.InvoiceTransaction.BUYER_TAX_CODE = txtBuyerTaxCode.Text;

                    billTwoBookSDO.InvoiceTransaction.BUYER_TYPE = 1;
                }
                else if (chkOrganizationInfo.Checked)
                {
                    billTwoBookSDO.InvoiceTransaction.BUYER_NAME = txtBuyerName2.Text;
                    billTwoBookSDO.InvoiceTransaction.BUYER_ADDRESS = txtBuyerAddress2.Text;
                    billTwoBookSDO.InvoiceTransaction.BUYER_ORGANIZATION = txtBuyerOrganization2.Text;
                    billTwoBookSDO.InvoiceTransaction.BUYER_TAX_CODE = txtBuyerTaxCode2.Text;

                    billTwoBookSDO.InvoiceTransaction.BUYER_TYPE = 2;
                }
                billTwoBookSDO.InvoiceSereServBills = new List<HIS_SERE_SERV_BILL>();
                foreach (var item in listInvoice)
                {
                    HIS_SERE_SERV_BILL ssBill = new HIS_SERE_SERV_BILL();
                    ssBill.PRICE = item.InvoicePrice ?? 0;
                    ssBill.SERE_SERV_ID = item.ID;
                    billTwoBookSDO.InvoiceSereServBills.Add(ssBill);
                }
                if (checkIsKC.CheckState == CheckState.Checked)
                {
                    if (totalHienDu >= (totalInvoicePrice - spinInvoiceDiscountPrice.Value))
                    {
                        billTwoBookSDO.InvoiceTransaction.KC_AMOUNT = (totalInvoicePrice - spinInvoiceDiscountPrice.Value);
                        billTwoBookSDO.InvoicePayAmount = 0;
                    }
                    else
                    {
                        billTwoBookSDO.InvoiceTransaction.KC_AMOUNT = totalHienDu;
                        billTwoBookSDO.InvoicePayAmount = (totalInvoicePrice - spinInvoiceDiscountPrice.Value - totalHienDu);
                    }
                    if (totalHienDu == 0)
                    {
                        billTwoBookSDO.InvoiceTransaction.KC_AMOUNT = null;
                        billTwoBookSDO.InvoicePayAmount = (totalInvoicePrice - spinInvoiceDiscountPrice.Value);
                    }
                }
                else
                {
                    billTwoBookSDO.InvoiceTransaction.KC_AMOUNT = null;
                    billTwoBookSDO.InvoicePayAmount = (totalInvoicePrice - spinInvoiceDiscountPrice.Value);
                }
                totalHienDu -= (billTwoBookSDO.InvoiceTransaction.KC_AMOUNT ?? 0);
                billTwoBookSDO.RequestRoomId = this.currentModuleBase.RoomId;
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private HisTransReqBillTwoBookSDO MapTranToReq(HisTransactionBillTwoBookSDO billTwoBookSDO, CARD.WCF.DCO.WcfSaleDCO saleDCO)
        {
            HisTransReqBillTwoBookSDO req = new HisTransReqBillTwoBookSDO();
            req.TreatmentId = billTwoBookSDO.TreatmentId;
            req.RequestRoomId = billTwoBookSDO.RequestRoomId;
            if (billTwoBookSDO.RecieptTransaction != null)
            {
                req.RecieptTransReq = new HIS_TRANS_REQ();
                //req.RecieptTransReq.ACCOUNT_BOOK_ID = billTwoBookSDO.RecieptTransaction.ACCOUNT_BOOK_ID;
                req.RecieptTransReq.AMOUNT = billTwoBookSDO.RecieptTransaction.AMOUNT;
                //req.RecieptTransReq.BILL_TYPE_ID = billTwoBookSDO.RecieptTransaction.BILL_TYPE_ID;
                //req.RecieptTransReq.BUYER_ACCOUNT_NUMBER = billTwoBookSDO.RecieptTransaction.BUYER_ACCOUNT_NUMBER;
                //req.RecieptTransReq.BUYER_ADDRESS = billTwoBookSDO.RecieptTransaction.BUYER_ADDRESS;
                //req.RecieptTransReq.BUYER_NAME = billTwoBookSDO.RecieptTransaction.BUYER_NAME;
                //req.RecieptTransReq.BUYER_ORGANIZATION = billTwoBookSDO.RecieptTransaction.BUYER_ORGANIZATION;
                //req.RecieptTransReq.BUYER_TAX_CODE = billTwoBookSDO.RecieptTransaction.BUYER_TAX_CODE;
                //req.RecieptTransReq.CASHIER_LOGINNAME = billTwoBookSDO.RecieptTransaction.CASHIER_LOGINNAME;
                //req.RecieptTransReq.CASHIER_ROOM_ID = billTwoBookSDO.RecieptTransaction.CASHIER_ROOM_ID;
                //req.RecieptTransReq.CASHIER_USERNAME = billTwoBookSDO.RecieptTransaction.CASHIER_USERNAME;
                //req.RecieptTransReq.DESCRIPTION = billTwoBookSDO.RecieptTransaction.DESCRIPTION;
                //req.RecieptTransReq.EXEMPTION = billTwoBookSDO.RecieptTransaction.EXEMPTION;
                //req.RecieptTransReq.EXEMPTION_REASON = billTwoBookSDO.RecieptTransaction.EXEMPTION_REASON;
                //if (saleDCO != null)
                //{
                //    req.RecieptTransReq.EXPIRY_TIME = saleDCO != null ? saleDCO.ExpiryTime ?? 0 : 0;
                //}
                //else
                //{
                //    DateTime today = DateTime.Now;
                //    DateTime? newday = today.AddDays(3);
                //    long? TransactionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(newday);

                //    req.RecieptTransReq.EXPIRY_TIME = TransactionTime.Value;
                //}
                //req.RecieptTransReq.IS_DIRECTLY_BILLING = billTwoBookSDO.RecieptTransaction.IS_DIRECTLY_BILLING;
                //req.RecieptTransReq.PAY_FORM_ID = billTwoBookSDO.RecieptTransaction.PAY_FORM_ID;
                //req.RecieptTransReq.SALE_TYPE_ID = billTwoBookSDO.RecieptTransaction.SALE_TYPE_ID;
                //req.RecieptTransReq.SELLER_ACCOUNT_NUMBER = billTwoBookSDO.RecieptTransaction.SELLER_ACCOUNT_NUMBER;
                //req.RecieptTransReq.SELLER_ADDRESS = billTwoBookSDO.RecieptTransaction.SELLER_ADDRESS;
                //req.RecieptTransReq.SELLER_NAME = billTwoBookSDO.RecieptTransaction.SELLER_NAME;
                //req.RecieptTransReq.SELLER_PHONE = billTwoBookSDO.RecieptTransaction.SELLER_PHONE;
                //req.RecieptTransReq.SELLER_TAX_CODE = billTwoBookSDO.RecieptTransaction.SELLER_TAX_CODE;
                req.RecieptTransReq.TRANS_REQ_CODE = saleDCO != null ? saleDCO.RequestTransCode : "";
                req.RecieptTransReq.TREATMENT_ID = billTwoBookSDO.TreatmentId;

                req.RecieptPayAmount = billTwoBookSDO.RecieptPayAmount;
                req.RecieptSeseTransReqs = (from r in billTwoBookSDO.RecieptSereServBills select new HIS_SESE_TRANS_REQ() { SERE_SERV_ID = r.SERE_SERV_ID, PRICE = r.PRICE }).ToList();

            }

            if (billTwoBookSDO.InvoiceTransaction != null)
            {
                req.InvoiceTransReq = new HIS_TRANS_REQ();
                //req.InvoiceTransReq.ACCOUNT_BOOK_ID = billTwoBookSDO.InvoiceTransaction.ACCOUNT_BOOK_ID;
                req.InvoiceTransReq.AMOUNT = billTwoBookSDO.InvoiceTransaction.AMOUNT;
                //req.InvoiceTransReq.BILL_TYPE_ID = billTwoBookSDO.InvoiceTransaction.BILL_TYPE_ID;
                //req.InvoiceTransReq.BUYER_ACCOUNT_NUMBER = billTwoBookSDO.InvoiceTransaction.BUYER_ACCOUNT_NUMBER;
                //req.InvoiceTransReq.BUYER_ADDRESS = billTwoBookSDO.InvoiceTransaction.BUYER_ADDRESS;
                //req.InvoiceTransReq.BUYER_NAME = billTwoBookSDO.InvoiceTransaction.BUYER_NAME;
                //req.InvoiceTransReq.BUYER_ORGANIZATION = billTwoBookSDO.InvoiceTransaction.BUYER_ORGANIZATION;
                //req.InvoiceTransReq.BUYER_TAX_CODE = billTwoBookSDO.InvoiceTransaction.BUYER_TAX_CODE;
                //req.InvoiceTransReq.CASHIER_LOGINNAME = billTwoBookSDO.InvoiceTransaction.CASHIER_LOGINNAME;
                //req.InvoiceTransReq.CASHIER_ROOM_ID = billTwoBookSDO.InvoiceTransaction.CASHIER_ROOM_ID;
                //req.InvoiceTransReq.CASHIER_USERNAME = billTwoBookSDO.InvoiceTransaction.CASHIER_USERNAME;
                //req.InvoiceTransReq.DESCRIPTION = billTwoBookSDO.InvoiceTransaction.DESCRIPTION;
                //req.InvoiceTransReq.EXEMPTION = billTwoBookSDO.InvoiceTransaction.EXEMPTION;
                //req.InvoiceTransReq.EXEMPTION_REASON = billTwoBookSDO.InvoiceTransaction.EXEMPTION_REASON;
                //if (saleDCO != null)
                //{
                //    req.InvoiceTransReq.EXPIRY_TIME = saleDCO != null ? saleDCO.ExpiryTime ?? 0 : 0;
                //}
                //else
                //{
                //    DateTime today = DateTime.Now;
                //    DateTime? newday = today.AddDays(3);
                //    long? TransactionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(newday);

                //    req.InvoiceTransReq.EXPIRY_TIME = TransactionTime.Value;
                //}

                //req.InvoiceTransReq.IS_DIRECTLY_BILLING = billTwoBookSDO.InvoiceTransaction.IS_DIRECTLY_BILLING;
                //req.InvoiceTransReq.PAY_FORM_ID = billTwoBookSDO.InvoiceTransaction.PAY_FORM_ID;
                //req.InvoiceTransReq.SALE_TYPE_ID = billTwoBookSDO.InvoiceTransaction.SALE_TYPE_ID;
                //req.InvoiceTransReq.SELLER_ACCOUNT_NUMBER = billTwoBookSDO.InvoiceTransaction.SELLER_ACCOUNT_NUMBER;
                //req.InvoiceTransReq.SELLER_ADDRESS = billTwoBookSDO.InvoiceTransaction.SELLER_ADDRESS;
                //req.InvoiceTransReq.SELLER_NAME = billTwoBookSDO.InvoiceTransaction.SELLER_NAME;
                //req.InvoiceTransReq.SELLER_PHONE = billTwoBookSDO.InvoiceTransaction.SELLER_PHONE;
                //req.InvoiceTransReq.SELLER_TAX_CODE = billTwoBookSDO.InvoiceTransaction.SELLER_TAX_CODE;
                req.InvoiceTransReq.TRANS_REQ_CODE = saleDCO != null ? saleDCO.RequestTransCode : "";
                req.InvoiceTransReq.TREATMENT_ID = billTwoBookSDO.TreatmentId;

                req.InvoicePayAmount = billTwoBookSDO.InvoicePayAmount;
                req.InvoiceSeseTransReqs = (from r in billTwoBookSDO.InvoiceSereServBills select new HIS_SESE_TRANS_REQ() { SERE_SERV_ID = r.SERE_SERV_ID, PRICE = r.PRICE }).ToList();
            }
            return req;
        }

        private void SetBillSuccessControl(List<V_HIS_TRANSACTION> results)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GeneratePopupMenu()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_TWO_IN_ONE__BTN_DROP_DOWN__ITEM_BIEN_LAI_THANH_TOAN", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), new EventHandler(onClickBienLaiThanhToan)));

                menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_TWO_IN_ONE__BTN_DROP_DOWN__ITEM_HOA_DON_THANH_TOAN", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), new EventHandler(onClickHoaDonThanhToan)));

                menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_TWO_IN_ONE__BTN_DROP_DOWN__ITEM_CHI_TIET_THANH_TOAN", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), new EventHandler(onClickChiTietDichVu)));

                ddBtnPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickChiTietDichVu(object sender, EventArgs e)
        {
            try
            {
                if (this.resultRecieptBill == null)
                    return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__HoaDonThanhToanChiTietDichVu_Mps000106, delegatePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickBienLaiThanhToan(object sender, EventArgs e)
        {
            try
            {
                if (this.resultRecieptBill == null)
                    return;
                if (HisConfig.BILL_TWO_BOOK__OPTION == (int)HisConfig.BILL_OPTION.HCM_115
                    || HisConfig.BILL_TWO_BOOK__OPTION == (int)HisConfig.BILL_OPTION.QBH_CUBA)
                {
                    Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    store.RunPrintTemplate("Mps000317", delegatePrintTemplate);
                }
                else
                {
                    Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuBienLaiThanhToan_MPS000148, delegatePrintTemplate);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickHoaDonThanhToan(object sender, EventArgs e)
        {
            try
            {
                if (this.resultInvoiceBill == null)
                    return;
                if (HisConfig.BILL_TWO_BOOK__OPTION == (int)HisConfig.BILL_OPTION.HCM_115
                    || HisConfig.BILL_TWO_BOOK__OPTION == (int)HisConfig.BILL_OPTION.QBH_CUBA)
                {
                    Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    store.RunPrintTemplate("Mps000318", delegatePrintTemplate);
                }
                else
                {
                    Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuHoaDonThanhToan_MPS000147, delegatePrintTemplate);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool delegatePrintTemplate(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuHoaDonThanhToan_MPS000147:
                        InPhieuHoaDonThanhToan(ref result, printTypeCode, fileName);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuBienLaiThanhToan_MPS000148:
                        InPhieuBienLaiThanhToan(ref result, printTypeCode, fileName);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__HoaDonThanhToanChiTietDichVu_Mps000106:
                        InChitietThanhToanBienLai(ref result, printTypeCode, fileName);
                        break;
                    case "Mps000317":
                        InPhieuBienLaiThanhToanHcm115(ref result, printTypeCode, fileName);
                        break;
                    case "Mps000318":
                        InPhieuHoaDonThanhToanHcm115(ref result, printTypeCode, fileName);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void InChitietThanhToanHoaDon(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                if (this.resultInvoiceBill == null)
                    return;
                WaitingManager.Show();
                var listSereServ = new List<V_HIS_SERE_SERV>();
                HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                ssBillFilter.BILL_ID = this.resultInvoiceBill.ID;
                var hisSSBills = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, null);
                if (hisSSBills == null || hisSSBills.Count <= 0)
                {
                    throw new Exception("Khong lay duoc SereServBill theo BillId: " + this.resultInvoiceBill.ID);
                }

                HisSereServViewFilter ssFilter = new HisSereServViewFilter();
                ssFilter.IDs = hisSSBills.Select(s => s.SERE_SERV_ID).ToList(); ;
                ssFilter.TREATMENT_ID = this.treatmentId;
                listSereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, ssFilter, null);

                HisPatientTypeAlterViewAppliedFilter ptAlterAppFilter = new HisPatientTypeAlterViewAppliedFilter();
                ptAlterAppFilter.TreatmentId = this.treatmentId.Value;
                ptAlterAppFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, ptAlterAppFilter, null);

                // tính mức hưởng của thẻ
                string levelCode = HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT;
                string ratio_text = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentPatientTypeAlter.HEIN_CARD_NUMBER, currentPatientTypeAlter.LEVEL_CODE, currentPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100) + "";

                HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = this.treatment.PATIENT_ID;
                var patients = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, null);
                V_HIS_PATIENT patient = new V_HIS_PATIENT();
                if (patients != null && patients.Count > 0)
                {
                    patient = patients.FirstOrDefault();
                }

                if (listSereServ != null && listSereServ.Count > 0)
                {
                    decimal totalDeposit = GetDepositAmount(treatmentId);
                    HIS_TREATMENT treatment = GetTreatment(treatmentId);

                    MPS.Processor.Mps000106.PDO.Mps000106ADO ado = new MPS.Processor.Mps000106.PDO.Mps000106ADO();
                    ado.PatientTypeBHYT = HisConfig.PatientTypeId__BHYT;
                    ado.PatientTypeVP = HisConfig.PATIENT_TYPE_ID__IS_FEE;
                    decimal ctAmount = 0;
                    decimal.TryParse(lblCanThu.Text ?? "0", out ctAmount);
                    MPS.Processor.Mps000106.PDO.Mps000106PDO rdo = new MPS.Processor.Mps000106.PDO.Mps000106PDO(this.resultInvoiceBill, listSereServ, treatment, totalDeposit, ctAmount, currentPatientTypeAlter, patient, ratio_text, ado, BackendDataWorker.Get<HIS_DEPARTMENT>());

                    WaitingManager.Hide();
                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.resultInvoiceBill != null ? this.resultInvoiceBill.TREATMENT_CODE : ""), printTypeCode, this.currentModuleBase != null ? currentModuleBase.RoomId : 0);

                    if (isSavePrint || ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InChitietThanhToanBienLai(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                if (this.resultRecieptBill == null)
                    return;
                WaitingManager.Show();
                var listSereServ = new List<V_HIS_SERE_SERV>();
                HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                ssBillFilter.BILL_ID = this.resultRecieptBill.ID;
                var hisSSBills = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, null);
                if (hisSSBills == null || hisSSBills.Count <= 0)
                {
                    throw new Exception("Khong lay duoc SereServBill theo BillId: " + this.resultRecieptBill.ID);
                }

                HisSereServViewFilter ssFilter = new HisSereServViewFilter();
                ssFilter.IDs = hisSSBills.Select(s => s.SERE_SERV_ID).ToList(); ;
                ssFilter.TREATMENT_ID = this.treatmentId;
                listSereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, ssFilter, null);

                HisPatientTypeAlterViewAppliedFilter ptAlterAppFilter = new HisPatientTypeAlterViewAppliedFilter();
                ptAlterAppFilter.TreatmentId = this.treatmentId.Value;
                ptAlterAppFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, ptAlterAppFilter, null);

                // tính mức hưởng của thẻ
                string levelCode = HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT;
                string ratio_text = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentPatientTypeAlter.HEIN_CARD_NUMBER, currentPatientTypeAlter.LEVEL_CODE, currentPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100) + "";

                HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = this.treatment.PATIENT_ID;
                var patients = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, null);
                V_HIS_PATIENT patient = new V_HIS_PATIENT();
                if (patients != null && patients.Count > 0)
                {
                    patient = patients.FirstOrDefault();
                }

                if (listSereServ != null && listSereServ.Count > 0)
                {
                    decimal totalDeposit = GetDepositAmount(treatmentId);
                    HIS_TREATMENT treatment = GetTreatment(treatmentId);

                    MPS.Processor.Mps000106.PDO.Mps000106ADO ado = new MPS.Processor.Mps000106.PDO.Mps000106ADO();
                    ado.PatientTypeBHYT = HisConfig.PatientTypeId__BHYT;
                    ado.PatientTypeVP = HisConfig.PATIENT_TYPE_ID__IS_FEE;

                    decimal ctAmount = 0;
                    decimal.TryParse(lblCanThu.Text ?? "0", out ctAmount);
                    MPS.Processor.Mps000106.PDO.Mps000106PDO rdo = new MPS.Processor.Mps000106.PDO.Mps000106PDO(this.resultRecieptBill, listSereServ, treatment, totalDeposit, ctAmount, currentPatientTypeAlter, patient, ratio_text, ado, BackendDataWorker.Get<HIS_DEPARTMENT>());

                    WaitingManager.Hide();
                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.resultRecieptBill != null ? this.resultRecieptBill.TREATMENT_CODE : ""), printTypeCode, this.currentModuleBase != null ? currentModuleBase.RoomId : 0);

                    if (isSavePrint || ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                    }
                }

                InChitietThanhToanHoaDon(ref result, printTypeCode, fileName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuHoaDonThanhToan(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                if (resultInvoiceBill == null)
                    return;
                WaitingManager.Show();
                MPS.Processor.Mps000147.PDO.Mps000147PDO rdo = new MPS.Processor.Mps000147.PDO.Mps000147PDO(resultInvoiceBill);
                WaitingManager.Hide();
                if (dicPrinter.ContainsKey(printTypeCode) && !String.IsNullOrEmpty(dicPrinter[printTypeCode]))
                {
                    if (isSavePrint || ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, dicPrinter[printTypeCode]) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, dicPrinter[printTypeCode]) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                    }
                }
                else
                {
                    if (isSavePrint || ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, null) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                    }
                    else
                    {
                        result = result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, null) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuBienLaiThanhToan(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                if (resultRecieptBill == null)
                    return;
                HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                ssBillFilter.BILL_ID = resultRecieptBill.ID;
                var listSSBill = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, null);
                if (listSSBill == null || listSSBill.Count <= 0)
                {
                    throw new Exception("Khong lay duoc SSBill theo billId: " + resultInvoiceBill.ID);
                }
                HisSereServFilter filter = new HisSereServFilter();
                filter.IDs = listSSBill.Select(s => s.SERE_SERV_ID).ToList();
                var listSereServ = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, filter, null);

                if (listSereServ == null || listSereServ.Count == 0)
                {
                    throw new NullReferenceException("Khong lay duoc SereServ theo resultRecieptBill.ID" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultInvoiceBill), resultInvoiceBill));
                }
                MPS.Processor.Mps000148.PDO.Mps000148PDO rdo = new MPS.Processor.Mps000148.PDO.Mps000148PDO(resultRecieptBill, listSSBill, listSereServ, HisConfig.PatientTypeId__BHYT);
                if (dicPrinter.ContainsKey(printTypeCode) && !String.IsNullOrEmpty(dicPrinter[printTypeCode]))
                {
                    if (isSavePrint || ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, dicPrinter[printTypeCode]) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, dicPrinter[printTypeCode]) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                    }
                }
                else
                {
                    if (isSavePrint || ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, null) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, null) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuBienLaiThanhToanHcm115(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                if (resultRecieptBill == null)
                    return;
                MPS.Processor.Mps000317.PDO.Mps000317PDO rdo = new MPS.Processor.Mps000317.PDO.Mps000317PDO(resultRecieptBill);
                if (dicPrinter.ContainsKey(printTypeCode) && !String.IsNullOrEmpty(dicPrinter[printTypeCode]))
                {
                    if (isSavePrint || ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, dicPrinter[printTypeCode]) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, dicPrinter[printTypeCode]) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                    }
                }
                else
                {
                    if (isSavePrint || ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, null) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, null) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuHoaDonThanhToanHcm115(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                if (resultInvoiceBill == null)
                    return;
                WaitingManager.Show();
                MPS.Processor.Mps000318.PDO.Mps000318PDO rdo = new MPS.Processor.Mps000318.PDO.Mps000318PDO(resultInvoiceBill);
                WaitingManager.Hide();
                if (dicPrinter.ContainsKey(printTypeCode) && !String.IsNullOrEmpty(dicPrinter[printTypeCode]))
                {
                    if (isSavePrint || ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, dicPrinter[printTypeCode]) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, dicPrinter[printTypeCode]) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                    }
                }
                else
                {
                    if (isSavePrint || ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, null) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                    }
                    else
                    {
                        result = result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, null) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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

        private ElectronicBillResult TaoHoaDonDienTuBenThu3CungCap(V_HIS_TRANSACTION transaction)
        {
            ElectronicBillResult result = new ElectronicBillResult();
            try
            {
                if (transaction == null)
                {
                    return null;
                }
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("TaoHoaDonDienTuBenThu3CungCap()__transaction: ", transaction));
                HIS_TRANSACTION tran = new HIS_TRANSACTION();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(tran, transaction);

                ElectronicBillDataInput dataInput = new ElectronicBillDataInput();
                dataInput.Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                dataInput.Treatment = treatment;
                dataInput.Currency = "VND";
                dataInput.Transaction = tran;
                dataInput.Amount = transaction.AMOUNT;
                dataInput.Discount = transaction.EXEMPTION;
                dataInput.DiscountRatio = Math.Round((transaction.EXEMPTION ?? 0) / transaction.AMOUNT, 2, MidpointRounding.AwayFromZero) * 100;
                dataInput.PaymentMethod = transaction.PAY_FORM_NAME;
                dataInput.SymbolCode = transaction.SYMBOL_CODE;
                dataInput.TemplateCode = transaction.TEMPLATE_CODE;
                dataInput.TransactionTime = transaction.TRANSACTION_TIME;
                dataInput.EinvoiceTypeId = transaction.EINVOICE_TYPE_ID;
                List<V_HIS_TRANSACTION> vtran = new List<V_HIS_TRANSACTION>();
                vtran.Add(transaction);
                dataInput.ListTransaction = vtran;

                List<V_HIS_SERE_SERV_5> sereServ5s = new List<V_HIS_SERE_SERV_5>();

                var param = new CommonParam();
                HisSereServBillFilter ssbfilter = new HisSereServBillFilter();
                ssbfilter.BILL_ID = transaction.ID;
                var sereServBill = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssbfilter, param);
                if (sereServBill != null && sereServBill.Count > 0)
                {
                    int skip = 0;
                    List<long> ssIds = sereServBill.Select(s => s.SERE_SERV_ID).Distinct().ToList();
                    while (ssIds.Count - skip > 0)
                    {
                        var listId = ssIds.Skip(skip).Take(100).ToList();
                        skip += 100;

                        HisSereServView5Filter ss5filter = new HisSereServView5Filter();
                        ss5filter.IDs = listId;
                        var sereServ = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumers.MosConsumer, ss5filter, param);
                        if (sereServ != null && sereServ.Count > 0)
                        {
                            sereServ5s.AddRange(sereServ);
                        }
                    }

                    //Thành tiền là tiền trong chi tiết thanh toán.
                    //thanh toán 2 sổ 1 dịch vụ tách 2 sổ. số tiền khác nhau
                    foreach (var ss5 in sereServ5s)
                    {
                        var ssb = sereServBill.FirstOrDefault(f => f.SERE_SERV_ID == ss5.ID);
                        if (ssb != null)
                        {
                            ss5.VIR_TOTAL_PATIENT_PRICE = ssb.PRICE;
                        }
                    }
                }
                else
                {
                    HisBillGoodsFilter bgfilter = new HisBillGoodsFilter();
                    bgfilter.BILL_ID = transaction.ID;
                    var billgoods = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_BILL_GOODS>>("api/HisBillGoods/Get", ApiConsumers.MosConsumer, ssbfilter, param);
                    if (billgoods != null && billgoods.Count > 0)
                    {
                        int dem = 0;
                        foreach (var item in billgoods)
                        {
                            V_HIS_SERE_SERV_5 ssb = new V_HIS_SERE_SERV_5();

                            ssb.SERVICE_ID = item.NONE_MEDI_SERVICE_ID ?? item.MATERIAL_TYPE_ID ?? item.MEDICINE_TYPE_ID ?? dem;
                            ssb.AMOUNT = item.AMOUNT;
                            ssb.VAT_RATIO = item.VAT_RATIO ?? 0;
                            ssb.TDL_SERVICE_CODE = "";
                            ssb.TDL_SERVICE_NAME = item.GOODS_NAME;
                            ssb.SERVICE_UNIT_NAME = item.GOODS_UNIT_NAME;
                            //ssb.DISCOUNT = item.DISCOUNT;
                            //ssb.PRICE = item.PRICE;
                            ssb.VIR_PRICE = item.PRICE - ((item.DISCOUNT ?? 0) / item.AMOUNT);
                            ssb.VIR_TOTAL_PATIENT_PRICE = ssb.VIR_PRICE * (1 + ssb.VAT_RATIO) * ssb.AMOUNT;
                            sereServ5s.Add(ssb);
                            dem++;
                        }
                    }
                }

                dataInput.SereServs = sereServ5s;

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

        private void barBtnSaveAndSign_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnSaveAndSign_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public bool isCreateQRContinue = true;
        private void BtnSaveAndSign_Click(object sender, EventArgs e)
        {
            try
            {
                if (!BtnSaveAndSign.Enabled)
                    return;
                if((long)cboPayForm.EditValue == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR && MessageBox.Show(this,"Thanh toán QR chưa thể tự động tạo hóa đơn điện tử, bạn có muốn tiếp tục?","Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    isCreateQRContinue = false;
                    return;
                }
                SetEnableButtonSave(false);
                ValidControl();
                positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                {
                    SetEnableButtonSave(true);
                    return;
                }
                ClearValidate();
                if (!this.CheckHastInvoiceCancel())
                {
                    SetEnableButtonSave(true);
                    return;
                }
                WaitingManager.Show();
                isSavePrint = false;
                isLuuKy = true;
                CommonParam param = new CommonParam();
                bool success = false;
                ProcessSave(ref param, ref success);
                WaitingManager.Hide();
                if (success)
                {
                    if (!chkHideHddt.Checked)
                    {
                        int sleepTime = (int)(HisConfig.ElectronicInvoicePublishingDelayTime * 1000);
                        Inventec.Common.Logging.LogSystem.Debug("ElectronicInvoicePublishingDelayTime: " + sleepTime);
                        System.Threading.Thread.Sleep(sleepTime);
                        this.onClickInHoaDonDienTuBienLai(null, null);
                        this.onClickInHoaDonDienTuHoaDon(null, null);
                    }
                }

                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickInHoaDonDienTuBienLai(object sender, EventArgs e)
        {
            try
            {
                if (this.resultRecieptBill == null || String.IsNullOrEmpty(this.resultRecieptBill.INVOICE_CODE))
                    return;

                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.resultRecieptBill), this.resultRecieptBill));
                ElectronicBillDataInput dataInput = new ElectronicBillDataInput();
                dataInput.PartnerInvoiceID = Inventec.Common.TypeConvert.Parse.ToInt64(this.resultRecieptBill.INVOICE_CODE);
                dataInput.InvoiceCode = resultRecieptBill.INVOICE_CODE;
                dataInput.NumOrder = resultRecieptBill.NUM_ORDER;
                dataInput.SymbolCode = resultRecieptBill.SYMBOL_CODE;
                dataInput.TemplateCode = resultRecieptBill.TEMPLATE_CODE;
                dataInput.TransactionTime = resultRecieptBill.EINVOICE_TIME ?? resultRecieptBill.TRANSACTION_TIME;
                dataInput.ENumOrder = resultRecieptBill.EINVOICE_NUM_ORDER;
                dataInput.EinvoiceTypeId = resultRecieptBill.EINVOICE_TYPE_ID;
                HIS_TRANSACTION transaction = new HIS_TRANSACTION();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(transaction, this.resultRecieptBill);
                List<V_HIS_TRANSACTION> vtran = new List<V_HIS_TRANSACTION>();
                vtran.Add(resultRecieptBill);
                dataInput.ListTransaction = vtran;
                dataInput.Transaction = transaction;
                dataInput.Treatment = this.treatment;
                dataInput.SereServs = new List<V_HIS_SERE_SERV_5>();
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
                    {
                        MessageBox.Show("Không tìm thấy link hóa đơn điện tử");
                    }
                    return;
                }

                DocumentViewerManager viewManager = new DocumentViewerManager(ViewType.ENUM.Pdf);
                InputADO ado = new InputADO();
                ado.DeleteWhenClose = true;
                ado.NumberOfCopy = HisConfig.E_BILL__PRINT_NUM_COPY;
                ado.URL = electronicBillResult.InvoiceLink;
                ViewType.Platform type = ViewType.Platform.Telerik;
                if (HisConfig.PlatformOption > 0)
                {
                    type = (ViewType.Platform)(HisConfig.PlatformOption - 1);
                }

                viewManager.Run(ado, type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickInHoaDonDienTuHoaDon(object sender, EventArgs e)
        {
            try
            {
                if (this.resultInvoiceBill == null || String.IsNullOrEmpty(this.resultInvoiceBill.INVOICE_CODE))
                    return;

                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.resultInvoiceBill), this.resultInvoiceBill));
                ElectronicBillDataInput dataInput = new ElectronicBillDataInput();
                dataInput.PartnerInvoiceID = Inventec.Common.TypeConvert.Parse.ToInt64(this.resultInvoiceBill.INVOICE_CODE);
                dataInput.InvoiceCode = resultInvoiceBill.INVOICE_CODE;
                dataInput.NumOrder = resultInvoiceBill.NUM_ORDER;
                dataInput.SymbolCode = resultInvoiceBill.SYMBOL_CODE;
                dataInput.TemplateCode = resultInvoiceBill.TEMPLATE_CODE;
                dataInput.TransactionTime = resultInvoiceBill.EINVOICE_TIME ?? resultInvoiceBill.TRANSACTION_TIME;
                dataInput.ENumOrder = resultInvoiceBill.EINVOICE_NUM_ORDER;
                dataInput.EinvoiceTypeId = resultInvoiceBill.EINVOICE_TYPE_ID;
                HIS_TRANSACTION transaction = new HIS_TRANSACTION();
                List<V_HIS_TRANSACTION> vtran = new List<V_HIS_TRANSACTION>();
                vtran.Add(resultInvoiceBill);
                dataInput.ListTransaction = vtran;
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(transaction, this.resultInvoiceBill);
                dataInput.Transaction = transaction;
                dataInput.Treatment = this.treatment;
                dataInput.SereServs = new List<V_HIS_SERE_SERV_5>();
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
                    {
                        MessageBox.Show("Không tìm thấy link hóa đơn điện tử");
                    }
                    return;
                }

                DocumentViewerManager viewManager = new DocumentViewerManager(ViewType.ENUM.Pdf);
                InputADO ado = new InputADO();
                ado.DeleteWhenClose = true;
                ado.NumberOfCopy = HisConfig.E_BILL__PRINT_NUM_COPY;
                ado.URL = electronicBillResult.InvoiceLink;
                ViewType.Platform type = ViewType.Platform.Telerik;
                if (HisConfig.PlatformOption > 0)
                {
                    type = (ViewType.Platform)(HisConfig.PlatformOption - 1);
                }

                viewManager.Run(ado, type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckHastInvoiceCancel()
        {
            bool result = false;
            try
            {
                if (this.treatmentId != null)
                {
                    HisTransactionFilter tFilter = new HisTransactionFilter();
                    tFilter.TREATMENT_ID = this.treatmentId;
                    tFilter.TRANSACTION_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT;
                    tFilter.HAS_INVOICE_CODE = true;
                    tFilter.IS_CANCEL = true;
                    List<HIS_TRANSACTION> tranCancels = new BackendAdapter(new CommonParam()).Get<List<HIS_TRANSACTION>>("api/HisTransaction/Get", ApiConsumers.MosConsumer, tFilter, null);
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

        private void btnPuMoTaVienPhi_Click(object sender, EventArgs e)
        {
            try
            {
                SimpleButton editor = sender as SimpleButton;
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainer1.ShowPopup(new System.Drawing.Point(buttonPosition.X - 12, buttonPosition.Bottom + 320));
                this.currentContainerClick = ContainerClick.MoTaVienPhi;
                memoEdit1.Text = txtRecieptDescription.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnPuLyDoVienPhi_Click(object sender, EventArgs e)
        {
            try
            {
                SimpleButton editor = sender as SimpleButton;
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainer1.ShowPopup(new System.Drawing.Point(buttonPosition.X - 12, buttonPosition.Bottom + 330));
                this.currentContainerClick = ContainerClick.LyDoVienPhi;
                memoEdit1.Text = txtRecieptReason.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPuMoTaDichVu_Click(object sender, EventArgs e)
        {
            try
            {
                SimpleButton editor = sender as SimpleButton;
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainer1.ShowPopup(new System.Drawing.Point(buttonPosition.X + 600, buttonPosition.Bottom + 320));
                this.currentContainerClick = ContainerClick.MoTaDichVu;
                memoEdit1.Text = txtInvoiceDescription.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPuLyDoDichVu_Click(object sender, EventArgs e)
        {
            try
            {
                SimpleButton editor = sender as SimpleButton;
                Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                popupControlContainer1.ShowPopup(new System.Drawing.Point(buttonPosition.X + 600, buttonPosition.Bottom + 330));
                this.currentContainerClick = ContainerClick.LyDoDichVu;
                memoEdit1.Text = txtInvoiceReason.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                popupControlContainer1.HidePopup();
                switch (this.currentContainerClick)
                {
                    case ContainerClick.MoTaVienPhi:
                        txtRecieptDescription.Text = memoEdit1.Text;
                        break;
                    case ContainerClick.LyDoVienPhi:
                        txtRecieptReason.Text = memoEdit1.Text;
                        break;
                    case ContainerClick.MoTaDichVu:
                        txtInvoiceDescription.Text = memoEdit1.Text;
                        break;
                    case ContainerClick.LyDoDichVu:
                        txtInvoiceReason.Text = memoEdit1.Text;
                        break;
                    case ContainerClick.None:
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.currentContainerClick = ContainerClick.None;
                popupControlContainer1.HidePopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }



    }
}

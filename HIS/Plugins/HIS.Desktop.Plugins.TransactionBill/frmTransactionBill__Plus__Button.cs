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
using DevExpress.Utils.Menu;
using DevExpress.Utils.MVVM.Services;
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.Library.ElectronicBill;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Plugins.Library.ElectronicBill.Data;
using HIS.Desktop.Plugins.Library.ElectronicBill.Template;
using HIS.Desktop.Plugins.Library.PrintBordereau;
using HIS.Desktop.Plugins.Library.PrintBordereau.ADO;
using HIS.Desktop.Plugins.Library.PrintBordereau.Base;
using HIS.Desktop.Plugins.TransactionBill.ADO;
using HIS.Desktop.Plugins.TransactionBill.Base;
using HIS.Desktop.Plugins.TransactionBill.Config;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.DocumentViewer;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Fss.Client;
using Inventec.WCF.JsonConvert;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WCF;
using WCF.Client;


namespace HIS.Desktop.Plugins.TransactionBill
{
    public partial class frmTransactionBill : HIS.Desktop.Utility.FormBase
    {
        private string Print106Type = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Print.TransactionDetail");
        private string Print106Type_Expend = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Print.TransactionDetail_Expend");
        bool isPrintNow = false;
        bool isnotPrintMPS000111 = false;
        bool isEmr = false;
        byte[] byteData { get; set; }
        public HisTransactionBillResultSDO TransactionBillResultSDO { get; private set; }

        bool CreatAgain = false;
        WcfClient cll;

        List<string> ErrorElectronicBill = new List<string>();
        string nameFile = "";
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
                if (HisConfigCFG.AutoCreateDepositTransaction && decimal.Parse(lblReceiveAmount.Text) > 0 && cboDepositBook.Enabled && cboDepositBook.EditValue == null)
                {
                    XtraMessageBox.Show("Bắt buộc phải chọn sổ tạm ứng khi thanh toán", "Thông báo");
                    Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule validate = new Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule();
                    validate.editor = this.cboDepositBook;
                    validate.ErrorText = "Trường dữ liệu bắt buộc";
                    validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    dxValidationProvider1.SetValidationRule(this.cboDepositBook, validate);
                }
                if (!dxValidationProvider1.Validate() || this.treatmentId == null)
                {
                    return;
                }
                dxValidationProvider1.SetValidationRule(this.cboDepositBook, null);
                if (String.IsNullOrEmpty(TransactionBillConfig.InvoiceTypeCreate))
                    return;

                if (!this.CheckHastInvoiceCancel())
                {
                    return;
                }

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                gridViewBillFund.PostEditor();
                ErrorElectronicBill = new List<string>();
                success = ProcessSave(ref param, true);
                param.Messages = param.Messages.Distinct().ToList();
                WaitingManager.Hide();
                if (success == true)
                {
                    if (chkPrintBKBHNT.Checked)
                    {
                        Task ts = Task.Factory.StartNew(() =>
                        {
                            InBangKe_6556_BHYT_Mps000279();
                        });
                    }

                    this.hienHoaDonNhap = false;
                    bool showResult = true;

                    if (CreatAgain && success == true)
                    {
                        string notification = "Xử lý thành công.";
                        notification += param.GetMessage();

                        if (MessageBox.Show(notification, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            //xóa thông báo cũ để hiển thị thông báo của hóa đơn mới
                            foreach (var item in ErrorElectronicBill)
                            {
                                param.Messages.Remove(item);
                            }

                            HIS_TRANSACTION tranagain = new HIS_TRANSACTION();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(tranagain, resultTranBill);

                            ElectronicBillResult electronicBillResultagain = TaoHoaDonDienTuBenThu3CungCap(tranagain);
                            if (electronicBillResultagain == null || !electronicBillResultagain.Success)
                            {
                                param.Messages.Add("Tạo hóa đơn điện tử thất bại");
                                if (electronicBillResultagain.Messages != null && electronicBillResultagain.Messages.Count > 0)
                                {
                                    param.Messages.AddRange(electronicBillResultagain.Messages.Distinct().ToList());
                                }
                            }
                            else
                            {
                                //goi api update
                                CommonParam paramUpdate = new CommonParam();
                                HisTransactionInvoiceInfoSDO sdo = new HisTransactionInvoiceInfoSDO();
                                sdo.EinvoiceLoginname = electronicBillResultagain.InvoiceLoginname;
                                sdo.InvoiceCode = electronicBillResultagain.InvoiceCode;
                                sdo.InvoiceSys = electronicBillResultagain.InvoiceSys;
                                sdo.EinvoiceNumOrder = electronicBillResultagain.InvoiceNumOrder;
                                sdo.EInvoiceTime = electronicBillResultagain.InvoiceTime ?? (Inventec.Common.DateTime.Get.Now() ?? 0);
                                sdo.Id = resultTranBill.ID;
                                sdo.InvoiceLookupCode = electronicBillResultagain.InvoiceLookupCode;
                                var apiResult = new BackendAdapter(paramUpdate).Post<bool>("api/HisTransaction/UpdateInvoiceInfo", ApiConsumers.MosConsumer, sdo, paramUpdate);
                                if (apiResult)
                                {
                                    resultTranBill.INVOICE_CODE = electronicBillResultagain.InvoiceCode;
                                    resultTranBill.INVOICE_SYS = electronicBillResultagain.InvoiceSys;
                                    resultTranBill.EINVOICE_NUM_ORDER = electronicBillResultagain.InvoiceNumOrder;
                                    resultTranBill.EINVOICE_TIME = electronicBillResultagain.InvoiceTime;
                                    resultTranBill.EINVOICE_LOGINNAME = electronicBillResultagain.InvoiceLoginname;
                                }
                            }
                        }
                        else
                        {
                            showResult = false;
                        }
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
                            if (TransactionBillConfig.InvoiceTypeCreate == invoiceTypeCreate__CreateInvoiceHIS)
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

                    if (showResult)
                        MessageManager.Show(this, param, success);


                    if (cboPayForm.EditValue != null && Int64.Parse(cboPayForm.EditValue.ToString()) == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR)
                    {
                        btnQr.Enabled = true;
                        if (TransactionBillResultSDO.TransactionRepay != null && TransactionBillResultSDO.TransactionRepay.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR && TransactionBillResultSDO.TransactionRepay.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                            TransactionQr = TransactionBillResultSDO.TransactionRepay;
                        if (TransactionBillResultSDO.TransactionDeposit != null && TransactionBillResultSDO.TransactionDeposit.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR && TransactionBillResultSDO.TransactionDeposit.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                            TransactionQr = TransactionBillResultSDO.TransactionDeposit;
                        if (TransactionBillResultSDO.TransactionBill != null && TransactionBillResultSDO.TransactionBill.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR && TransactionBillResultSDO.TransactionBill.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                            TransactionQr = TransactionBillResultSDO.TransactionBill;
                        if(listConfig != null)
                        {
                            if (listConfig.Count > 1)
                                XtraMessageBox.Show("Vui lòng sử dụng nút tạo QR để thực hiện thanh toán", "Thông báo");
                            else
                                btnQr_Click(null, null);
                        }
                    }
                    if (success == true && chkAutoClose.CheckState == CheckState.Checked)
                    {
                        if (!chkPrintBKBHNT.Checked)
                            this.Close();
                        else
                        {
                            if (ListSereServTranfer != null && ListSereServTranfer.Count > 0)
                                timerClose.Interval = ListSereServTranfer.Count * timerClose.Interval;
                            timerClose.Start();
                        }
                    }
                    ddBtnPrint.Enabled = true;
                    panelMenuPrintBill.Enabled = true;
                    //this.Hide();
                }
                else if (success == false)
                {
                    MessageManager.Show(param, success);
                }

                GeneratePopupMenu();

                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                SetEnableButtonSave(!success);
            }
        }

        private void btnSavePrint_Click(object sender, EventArgs e)
        {
            bool? success = false;
            try
            {
                PrintMps279 = false;
                this.positionHandleControl = -1;
                if (!btnSavePrint.Enabled)
                    return;
                SetEnableButtonSave(false);
                if (HisConfigCFG.AutoCreateDepositTransaction && decimal.Parse(lblReceiveAmount.Text) > 0 && cboDepositBook.Enabled && cboDepositBook.EditValue == null)
                {
                    XtraMessageBox.Show("Bắt buộc phải chọn sổ tạm ứng khi thanh toán", "Thông báo");
                    Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule validate = new Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule();
                    validate.editor = this.cboDepositBook;
                    validate.ErrorText = "Trường dữ liệu bắt buộc";
                    validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    dxValidationProvider1.SetValidationRule(this.cboDepositBook, validate);
                }
                if (!dxValidationProvider1.Validate() || this.treatmentId == null)
                {
                    return;
                }
                dxValidationProvider1.SetValidationRule(this.cboDepositBook, null);
                if (HisConfigCFG.AttachAssignPrintWarningOption == "1")
                {
                    Inventec.Common.Logging.LogSystem.Debug("HisConfigCFG.AttachAssignPrintWarningOption == 1");
                    HIS_TREATMENT treatment = GetTreatment(this.treatmentId);
                    if (treatment != null && treatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                    {
                        CommonParam paramCommon = new CommonParam();
                        var result = new BackendAdapter(paramCommon).Get<List<string>>("api/HisServiceReq/GetAttachAssignPrint", ApiConsumers.MosConsumer, this.treatmentId, paramCommon);
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
                gridViewBillFund.PostEditor();
                success = ProcessSave(ref param);
                WaitingManager.Hide();
                if (success == true)
                {
                    if (chkPrintBKBHNT.Checked)
                    {
                        InBangKe_6556_BHYT_Mps000279();
                    }
                    this.hienHoaDonNhap = false;
                    this.onClickPhieuThuThanhToan();

                    if (cboPayForm.EditValue != null && Int64.Parse(cboPayForm.EditValue.ToString()) == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR)
                    {
                        btnQr.Enabled = true;
                        if (TransactionBillResultSDO.TransactionRepay != null && TransactionBillResultSDO.TransactionRepay.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR && TransactionBillResultSDO.TransactionRepay.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                            TransactionQr = TransactionBillResultSDO.TransactionRepay;
                        if (TransactionBillResultSDO.TransactionDeposit != null && TransactionBillResultSDO.TransactionDeposit.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR && TransactionBillResultSDO.TransactionDeposit.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                            TransactionQr = TransactionBillResultSDO.TransactionDeposit;
                        if (TransactionBillResultSDO.TransactionBill != null && TransactionBillResultSDO.TransactionBill.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR && TransactionBillResultSDO.TransactionBill.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                            TransactionQr = TransactionBillResultSDO.TransactionBill;
                        if (listConfig != null)
                        {
                            if (listConfig.Count > 1)
                                XtraMessageBox.Show("Vui lòng sử dụng nút tạo QR để thực hiện thanh toán", "Thông báo");
                            else
                                btnQr_Click(null, null);
                        }
                    }
                    if (chkAutoClose.CheckState == CheckState.Checked)
                    {
                        if (!chkPrintBKBHNT.Checked)
                            this.Close();
                        else
                        {
                            if (ListSereServTranfer != null && ListSereServTranfer.Count > 0)
                                timerClose.Interval = ListSereServTranfer.Count * timerClose.Interval;
                            timerClose.Start();
                        }
                    }
                }

                if (success == false)
                {
                    MessageManager.Show(param, success);
                }

                // SessionManager.ProcessTokenLost(param);
                GeneratePopupMenu();
                InitMenuToButtonPrint();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                SetEnableButtonSave(!success);
            }
        }

        private void InBangKe_6556_BHYT_Mps000279()
        {
            try
            {
                bool isBHYT = false;
                long patientTypeIdBHYT = HisConfigCFG.PatientTypeId__BHYT;
                var sereServ_BHYT = this.ListSereServTranfer.Where(o =>
                    o.PATIENT_TYPE_ID == patientTypeIdBHYT).ToList();

                if (sereServ_BHYT != null && sereServ_BHYT.Count > 0)
                {
                    isBHYT = true;
                }

                if (this.currentTreatment != null && this.currentTreatment.TDL_TREATMENT_TYPE_ID == 1 && isBHYT) ;
                {
                    if (this.currentTreatment == null)
                        return;

                    WaitingManager.Show();

                    ReloadMenuOption reloadMenuBordereau = new ReloadMenuOption();
                    reloadMenuBordereau.ReloadMenu = ReloadMenuNull;
                    reloadMenuBordereau.Type = ReloadMenuOption.MenuType.DYNAMIC;
                    reloadMenuBordereau.BordereauPrint = BordereauPrint.Type.MPS_BASE;
                    BordereauInitData bordereauInitData = new BordereauInitData();

                    AutoMapper.Mapper.CreateMap<V_HIS_TREATMENT_FEE, V_HIS_TREATMENT>();
                    bordereauInitData.Treatment = AutoMapper.Mapper.Map<V_HIS_TREATMENT>(this.currentTreatment);
                    AutoMapper.Mapper.CreateMap<V_HIS_SERE_SERV_5, HIS_SERE_SERV>();
                    var listHisSereServ = AutoMapper.Mapper.Map<List<HIS_SERE_SERV>>(this.ListSereServTranfer);
                    if (listHisSereServ != null)
                    {
                        bordereauInitData.SereServs = listHisSereServ.ToList();
                    }
                    else
                    {
                        bordereauInitData.SereServs = new List<HIS_SERE_SERV>();
                    }

                    bordereauInitData.PatientTypeAlter = resultPatientType;
                    HIS.Desktop.Plugins.Library.PrintBordereau.PrintBordereauProcessor processor = new PrintBordereauProcessor(this.currentModule != null ? this.currentModule.RoomId : 0, this.currentModule != null ? this.currentModule.RoomTypeId : 0, currentTreatment.ID, currentTreatment.PATIENT_ID, bordereauInitData, reloadMenuBordereau);

                    processor.Print("Mps000279",PrintOption.Value.PRINT_NOW);
                    PrintMps279 = true;
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void ReloadMenuNull(object data)
        {
            //do notthing
        }

        // kiểm tra hình thức thanh toán có phải là thanh toán qua thẻ không, nếu là thanh toán qua thẻ thì update màu sắc và validate textBox PIN
        void CheckPayFormThanhToanThe(PayFormADO payForm)
        {
            try
            {
                UpdatePINControl(false);
                if (cboPayForm.EditValue == null)
                    return;

                // check nếu hình thức thanh toán là thanh toán qua thẻ thì check textBox PIN
                if (payForm != null && payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE)
                {
                    UpdatePINControl(true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetList()
        {
            MOS.Filter.HisCardFilter cardFilter = new HisCardFilter();
            cardFilter.PATIENT_ID = this.currentTreatment.PATIENT_ID;
            hisCard = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_CARD>>("api/HisCard/Get", ApiConsumer.ApiConsumers.MosConsumer, cardFilter, null);

            MOS.Filter.HisPatientFilter PatientFilter = new HisPatientFilter();
            PatientFilter.ID = this.currentTreatment.PATIENT_ID;
            var hispatients = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, PatientFilter, null);
            hispatient = hispatients.FirstOrDefault();
            //hisCard = BackendDataWorker.Get<HIS_CARD>().Where(o => o.PATIENT_ID == this.currentTreatment.PATIENT_ID).ToList();
            //hispatient = BackendDataWorker.Get<V_HIS_PATIENT>().FirstOrDefault(o => o.ID == this.currentTreatment.PATIENT_ID);

        }

        private void CheckPayFormKEYPAY(PayFormADO payForm)
        {
            try
            {
                if (payForm != null && payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__KEYPAY)
                {
                    if (hisCard == null || hisCard.Count == 0)
                    {
                        if (hispatient == null || hispatient.REGISTER_CODE == null)
                        {
                            MessageManager.Show("Bệnh nhân không có thông tin thẻ Việt hoặc mã MS.Vui lòng chọn hình thức thanh toán khác");
                            cboPayForm.Focus();
                            cboPayForm.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        void CheckPayFormTienMatChuyenKhoan(PayFormADO payForm)
        {
            try
            {
                if (payForm != null && payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK)
                {
                    dxValidationProvider1.RemoveControlError(spinTransferAmount);
                    ValidControlTransferAmount(true);

                    this.lciTranferAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__LAYOUT_TRANSFER_AMOUNT_TEXT", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    this.lciTranferAmount.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__LAYOUT_TRANSFER_AMOUNT_TOOLTIP", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    lciTranferAmount.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciTranferAmount.Enabled = true;

                }
                else if (payForm != null && payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT)
                {
                    dxValidationProvider1.RemoveControlError(spinTransferAmount);
                    ValidControlTransferAmount(true);

                    this.lciTranferAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__LAYOUT_SWIPE_AMOUNT_TEXT", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    this.lciTranferAmount.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__LAYOUT_SWIPE_AMOUNT_TOOLTIP", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    lciTranferAmount.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciTranferAmount.Enabled = true;

                }
                else if (payForm != null && payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QUET_THE)
                {
                    dxValidationProvider1.RemoveControlError(spinTransferAmount);
                    ValidControlTransferAmount(false);
                    this.lciTranferAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__LAYOUT_TRANSFER_AMOUNT_TEXT", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    this.lciTranferAmount.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__LAYOUT_TRANSFER_AMOUNT_TOOLTIP", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    lciTranferAmount.AppearanceItemCaption.ForeColor = Color.Black;
                    lciTranferAmount.Enabled = false;

                }
                else
                {
                    dxValidationProvider1.RemoveControlError(spinTransferAmount);
                    ValidControlTransferAmount(false);
                    this.lciTranferAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__LAYOUT_TRANSFER_AMOUNT_TEXT", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    this.lciTranferAmount.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__LAYOUT_TRANSFER_AMOUNT_TOOLTIP", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    lciTranferAmount.AppearanceItemCaption.ForeColor = Color.Black;
                    lciTranferAmount.Enabled = false;
                }
                spinTransferAmount.EditValue = 0;
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
                PrintMps279 = false;
                this.positionHandleControl = -1;
                if (!btnSave.Enabled && !lciBtnSave.Enabled)
                    return;
                SetEnableButtonSave(false);
                if (HisConfigCFG.AutoCreateDepositTransaction && decimal.Parse(lblReceiveAmount.Text) > 0 && cboDepositBook.Enabled && cboDepositBook.EditValue == null)
                {
                    XtraMessageBox.Show("Bắt buộc phải chọn sổ tạm ứng khi thanh toán", "Thông báo");
                    Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule validate = new Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule();
                    validate.editor = this.cboDepositBook;
                    validate.ErrorText = "Trường dữ liệu bắt buộc";
                    validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    dxValidationProvider1.SetValidationRule(this.cboDepositBook, validate);
                }
                if (!dxValidationProvider1.Validate() || this.treatmentId == null)
                {
                    return;
                }
                dxValidationProvider1.SetValidationRule(this.cboDepositBook, null);
                if (currentTreatment.TDL_TREATMENT_TYPE_ID != null)
                {
                    var treatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == currentTreatment.TDL_TREATMENT_TYPE_ID);
                    if (currentTreatment.OUT_TIME.HasValue && (treatmentType.TRANS_TIME_OUT_TIME_OPTION == 1 || treatmentType.TRANS_TIME_OUT_TIME_OPTION == 2))
                    {
                        var transactionTime = Int64.Parse(Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTransactionTime.DateTime).ToString().Substring(0, 12));
                        var outTime = Int64.Parse(currentTreatment.OUT_TIME.ToString().Substring(0, 12));
                        if (currentTreatment.OUT_TIME.HasValue && transactionTime < outTime)
                        {
                            short type = 2;
                            MessageBoxButtons buttons = MessageBoxButtons.OK;
                            string message = string.Format(ResourceMessageLang.ThoiGianThanhToanNhoHonThoiGianRaVien, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(Int64.Parse(Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTransactionTime.DateTime).ToString())), Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(Int64.Parse(currentTreatment.OUT_TIME.ToString())));
                            if (treatmentType.TRANS_TIME_OUT_TIME_OPTION == 1)
                            {
                                type = 1;
                                message += " Bạn có muốn thực hiện thanh toán không?";
                                buttons = MessageBoxButtons.YesNo;
                            }
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(message, ResourceMessageLang.ThongBao, buttons) == (type == 1 ? System.Windows.Forms.DialogResult.No : System.Windows.Forms.DialogResult.OK))
                                return;
                        }
                    }
                }
                if (HisConfigCFG.AttachAssignPrintWarningOption == "1")
                {
                    Inventec.Common.Logging.LogSystem.Debug("HisConfigCFG.AttachAssignPrintWarningOption == 1");
                    if (currentTreatment != null && currentTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                    {
                        CommonParam paramCommon = new CommonParam();
                        var result = new BackendAdapter(paramCommon).Get<List<string>>("api/HisServiceReq/GetAttachAssignPrint", ApiConsumers.MosConsumer, this.treatmentId, paramCommon);
                        if (result != null && result.Count() > 0)
                        {
                            Inventec.Common.Logging.LogSystem.Debug(" result = " + result);
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
                gridViewBillFund.PostEditor();
                success = ProcessSave(ref param);
                WaitingManager.Hide();
                if (success == true)
                {
                    if (chkPrintBKBHNT.Checked)
                    {
                        InBangKe_6556_BHYT_Mps000279();
                    }
                    this.hienHoaDonNhap = false;
                    Inventec.Common.Logging.LogSystem.Debug("stopSave bill");

                    if (cboPayForm.EditValue != null && Int64.Parse(cboPayForm.EditValue.ToString()) == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR)
                    {
                        btnQr.Enabled = true;
                        if (TransactionBillResultSDO.TransactionRepay != null && TransactionBillResultSDO.TransactionRepay.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR && TransactionBillResultSDO.TransactionRepay.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                            TransactionQr = TransactionBillResultSDO.TransactionRepay;
                        if (TransactionBillResultSDO.TransactionDeposit != null && TransactionBillResultSDO.TransactionDeposit.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR && TransactionBillResultSDO.TransactionDeposit.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                            TransactionQr = TransactionBillResultSDO.TransactionDeposit;
                        if (TransactionBillResultSDO.TransactionBill != null && TransactionBillResultSDO.TransactionBill.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR && TransactionBillResultSDO.TransactionBill.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                            TransactionQr = TransactionBillResultSDO.TransactionBill;
                        if (listConfig != null)
                        {
                            if (listConfig.Count > 1)
                                XtraMessageBox.Show("Vui lòng sử dụng nút tạo QR để thực hiện thanh toán", "Thông báo");
                            else
                                btnQr_Click(null, null);
                        }
                    }
                    MessageManager.Show(this, param, success);
                    if (chkAutoClose.CheckState == CheckState.Checked)
                    {
                        if (!chkPrintBKBHNT.Checked)
                            this.Close();
                        else
                        {
                            if (ListSereServTranfer != null && ListSereServTranfer.Count > 0)
                                timerClose.Interval = ListSereServTranfer.Count * timerClose.Interval;
                            timerClose.Start();
                        }
                    }
                }
                else if (success == false)
                {
                    Inventec.Common.Logging.LogSystem.Debug("can not stopSave bill");
                    MessageManager.Show(param, success);
                }
                GeneratePopupMenu();
                InitMenuToButtonPrint();
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                SetEnableButtonSave(!success);
            }
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
        CARD.WCF.DCO.WcfSaleDCO SaleCard(ref CARD.WCF.DCO.WcfSaleDCO SaleDCO, CommonParam param)
        {
            CARD.WCF.DCO.WcfSaleDCO result = null;
            try
            {
                // gọi api HisCard/Get để lấy về serviceCodes
                MOS.Filter.HisCardFilter cardFilter = new HisCardFilter();
                cardFilter.PATIENT_ID = this.currentTreatment.PATIENT_ID;
                var HisCards = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_CARD>>("api/HisCard/Get", ApiConsumer.ApiConsumers.MosConsumer, cardFilter, param);
                if (HisCards != null && HisCards.Count > 0)
                {
                    SaleDCO.ServiceCodes = HisCards.Select(o => o.SERVICE_CODE).ToArray();
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

        private CARD.WCF.DCO.WcfRefundDCO RefundCard(ref CARD.WCF.DCO.WcfRefundDCO RepayDCO, CommonParam param)
        {
            CARD.WCF.DCO.WcfRefundDCO result = null;
            try
            {
                // gọi api HisCard/Get để lấy về serviceCodes
                MOS.Filter.HisCardFilter cardFilter = new HisCardFilter();
                cardFilter.PATIENT_ID = this.currentTreatment.PATIENT_ID;
                var HisCards = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_CARD>>("api/HisCard/Get", ApiConsumer.ApiConsumers.MosConsumer, cardFilter, param);
                if (HisCards != null && HisCards.Count > 0)
                {
                    RepayDCO.ServiceCodes = HisCards.Select(o => o.SERVICE_CODE).ToArray();
                }

                CARD.WCF.Client.TransactionClient.TransactionClientManager transactionClientManager = new CARD.WCF.Client.TransactionClient.TransactionClientManager();

                result = transactionClientManager.Refund(RepayDCO);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private HisTransReqBillSDO MapTranToReq(HisTransactionBillSDO billOneBookSDO)
        {
            HisTransReqBillSDO req = new HisTransReqBillSDO();
            req.PayAmount = billOneBookSDO.PayAmount;
            req.RequestRoomId = billOneBookSDO.RequestRoomId;
            if (billOneBookSDO.Transaction != null)
            {
                req.TransReq = new HIS_TRANS_REQ();
                //req.TransReq.ACCOUNT_BOOK_ID = billOneBookSDO.Transaction.ACCOUNT_BOOK_ID;
                req.TransReq.AMOUNT = billOneBookSDO.Transaction.AMOUNT;
                //req.TransReq.BILL_TYPE_ID = billOneBookSDO.Transaction.BILL_TYPE_ID;
                //req.TransReq.BUYER_ACCOUNT_NUMBER = billOneBookSDO.Transaction.BUYER_ACCOUNT_NUMBER;
                //req.TransReq.BUYER_ADDRESS = billOneBookSDO.Transaction.BUYER_ADDRESS;
                //req.TransReq.BUYER_NAME = billOneBookSDO.Transaction.BUYER_NAME;
                //req.TransReq.BUYER_ORGANIZATION = billOneBookSDO.Transaction.BUYER_ORGANIZATION;
                //req.TransReq.BUYER_TAX_CODE = billOneBookSDO.Transaction.BUYER_TAX_CODE;
                //req.TransReq.CASHIER_LOGINNAME = billOneBookSDO.Transaction.CASHIER_LOGINNAME;
                //req.TransReq.CASHIER_ROOM_ID = billOneBookSDO.Transaction.CASHIER_ROOM_ID;
                //req.TransReq.CASHIER_USERNAME = billOneBookSDO.Transaction.CASHIER_USERNAME;
                //req.TransReq.DESCRIPTION = billOneBookSDO.Transaction.DESCRIPTION;
                //req.TransReq.EXEMPTION = billOneBookSDO.Transaction.EXEMPTION;
                //req.TransReq.EXEMPTION_REASON = billOneBookSDO.Transaction.EXEMPTION_REASON;

                DateTime today = DateTime.Now;
                DateTime? newday = today.AddDays(3);
                long? TransactionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(newday);

                //req.TransReq.EXPIRY_TIME = TransactionTime.Value;

                //req.TransReq.IS_DIRECTLY_BILLING = billOneBookSDO.Transaction.IS_DIRECTLY_BILLING;
                //req.TransReq.PAY_FORM_ID = billOneBookSDO.Transaction.PAY_FORM_ID;
                //req.TransReq.SALE_TYPE_ID = billOneBookSDO.Transaction.SALE_TYPE_ID;
                //req.TransReq.SELLER_ACCOUNT_NUMBER = billOneBookSDO.Transaction.SELLER_ACCOUNT_NUMBER;
                //req.TransReq.SELLER_ADDRESS = billOneBookSDO.Transaction.SELLER_ADDRESS;
                //req.TransReq.SELLER_NAME = billOneBookSDO.Transaction.SELLER_NAME;
                //req.TransReq.SELLER_PHONE = billOneBookSDO.Transaction.SELLER_PHONE;
                //req.TransReq.SELLER_TAX_CODE = billOneBookSDO.Transaction.SELLER_TAX_CODE;
                req.TransReq.TRANS_REQ_CODE = "";
                req.TransReq.TREATMENT_ID = billOneBookSDO.Transaction.TREATMENT_ID.Value;

                req.SeseTransReqs = (from r in billOneBookSDO.SereServBills select new HIS_SESE_TRANS_REQ() { SERE_SERV_ID = r.SERE_SERV_ID, PRICE = r.PRICE }).ToList();
            }
            return req;
        }

        private bool? ProcessSave(ref CommonParam param, [Optional] bool isLuuKy)
        {
            Inventec.Common.Logging.LogSystem.Info("ProcessSave 1.1");
            this.isPrintNow = false;
            bool? success = false;
            try
            {
                TransactionBillResultSDO = null;
                TransactionQr = null;
                long payFormId = 0;
                CARD.WCF.DCO.WcfSaleDCO saleDCO = null;
                CARD.WCF.DCO.WcfRefundDCO refundDCO = null;

                Inventec.Common.Logging.LogSystem.Info("ProcessSave 1.2");
                var payForm = this.payFormList.FirstOrDefault(o => o.PayFormId == cboPayForm.EditValue);
                if (payForm == null)
                    return success;
                payFormId = payForm.ID;

                Inventec.Common.Logging.LogSystem.Info("ProcessSave 1.3");

                var listData = this.ssTreeProcessor.GetListCheck(this.ucSereServTree);
                if (listData == null || listData.Count == 0)
                {
                    param.Messages.Add(Base.ResourceMessageLang.NguoiDungChuChonDichVuDeThanhToan);
                    return success;
                }

                if (IsNeedAccountBook)
                {
                    if (cboAccountBook.EditValue == null)
                    {
                        param.Messages.Add(Base.ResourceMessageLang.ThieuTruongDuLieuBatBuoc);
                        return success;
                    }
                }

                if (cboPayForm.EditValue == null)
                {
                    param.Messages.Add(Base.ResourceMessageLang.ThieuTruongDuLieuBatBuoc);
                    return success;
                }

                if (txtDiscount.EditValue != null)
                {
                    this.totalDiscount = txtDiscount.Value;
                    if (this.totalPatientPrice > 0)
                    {
                        txtDiscountRatio.EditValue = (this.totalDiscount / this.totalPatientPrice) * 100;
                    }
                }
                else if (txtDiscountRatio.EditValue != null)
                {
                    var ratio = txtDiscountRatio.Value / 100;
                    this.totalDiscount = this.totalPatientPrice * ratio;
                    txtDiscount.Value = this.totalDiscount;
                }
                else
                {
                    this.totalDiscount = txtDiscount.Value;
                    if (this.totalPatientPrice > 0)
                    {
                        txtDiscountRatio.EditValue = (this.totalDiscount / this.totalPatientPrice) * 100;
                    }
                }

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
                    string totalAmountBNTra = Inventec.Common.Number.Convert.NumberToString(((totalPatientPrice - totalFund1 - this.totalDiscount)), ConfigApplications.NumberSeperator);

                    if (totalAmountBNTra == "0")
                    {
                        param.Messages.Add(Base.ResourceMessageLang.TienBenhNhanTraBangKhong);
                        isnotPrintMPS000111 = true;
                        Inventec.Common.Logging.LogSystem.Info("param123: ");
                    }
                }
                #endregion

                Inventec.Common.Logging.LogSystem.Info("ProcessSave 1.4");
                CalcuCanThu();

                Inventec.Common.Logging.LogSystem.Info("ProcessSave 1.5");

                HisTransactionBillSDO data = new HisTransactionBillSDO();
                if (currentTransaction != null)
                {
                    data.OriginalTransactionId = currentTransaction.ID;
                }
                data.ReplaceReason = txtReplaceReason.Text;
                data.Transaction = new HIS_TRANSACTION();
                data.Transaction.REPLACE_REASON = txtReplaceReason.Text;
                data.Transaction.TREATMENT_ID = this.treatmentId.Value;
                data.Transaction.CASHIER_ROOM_ID = this.cashierRoom.ID;
                data.Transaction.BUYER_NAME = txtBuyerName.Text.Trim();
                if (chkOther.Checked)
                {
                    data.Transaction.BUYER_ORGANIZATION = txtBuyerOrganization.Text.Trim();
                }
                else
                {
                    if (cboBuyerOrganization.EditValue != null)
                    {
                        data.Transaction.BUYER_WORK_PLACE_ID = Int64.Parse(cboBuyerOrganization.EditValue.ToString());
                        data.Transaction.BUYER_ORGANIZATION = dtWorkPlace.Where(o => o.ID == data.Transaction.BUYER_WORK_PLACE_ID).First().WORK_PLACE_NAME;
                    }
                }
                data.Transaction.BUYER_TAX_CODE = txtBuyerTaxCode.Text.Trim();
                data.Transaction.BUYER_ACCOUNT_NUMBER = txtBuyerAccountNumber.Text.Trim();
                data.Transaction.BUYER_ADDRESS = txtBuyerAddress.Text.Trim();
                data.Transaction.BUYER_EMAIL = txtBuyerEmail.Text.Trim();
                data.RequestRoomId = this.currentModule.RoomId;
                var accountBook = ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                if (accountBook != null)
                {
                    data.Transaction.ACCOUNT_BOOK_ID = accountBook.ID;
                }

                if (accountBook != null && accountBook.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                {
                    data.Transaction.NUM_ORDER = (long)spinTongTuDen.Value;
                }
                if (this.hisBankList != null && this.hisBankList.Count > 0 && payFormId == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QUET_THE)
                {
                    data.Transaction.PAY_FORM_ID = IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QUET_THE;
                    data.Transaction.BANK_ID = payForm.BANK_ID;
                }
                else
                {
                    data.Transaction.PAY_FORM_ID = payFormId;
                    data.Transaction.BANK_ID = null;
                }
                data.Transaction.AMOUNT = totalPatientPrice;
                if (dtTransactionTime.EditValue != null && dtTransactionTime.DateTime != DateTime.MinValue)
                    data.Transaction.TRANSACTION_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtTransactionTime.EditValue).ToString("yyyyMMddHHmm") + "00");
                data.Transaction.EXEMPTION = Math.Round(totalDiscount, 4);
                data.Transaction.EXEMPTION_REASON = txtReason.Text;
                data.Transaction.DESCRIPTION = txtDescription.Text;
                LogSystem.Info("IsDirectlyBilling: " + IsDirectlyBilling);
                if (this.IsDirectlyBilling.HasValue && this.IsDirectlyBilling == true)
                    data.Transaction.IS_DIRECTLY_BILLING = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                LogSystem.Info("IsDirectlyBilling: " + IsDirectlyBilling);
                List<HIS_SERE_SERV_BILL> hisSSBills = new List<HIS_SERE_SERV_BILL>();
                foreach (var item in listData)
                {
                    if (item.VIR_TOTAL_PATIENT_PRICE == 0 && HisConfigCFG.AllowToCreateNoPriceTransaction != "1")
                        continue;
                    HIS_SERE_SERV_BILL ssBill = new HIS_SERE_SERV_BILL();
                    ssBill.SERE_SERV_ID = item.ID;
                    ssBill.PRICE = item.VIR_TOTAL_PATIENT_PRICE ?? 0;
                    hisSSBills.Add(ssBill);
                }
                data.SereServBills = hisSSBills;
                var listFund = bindingSource1.DataSource as List<VHisBillFundADO>;
                if (listFund != null && listFund.Count > 0)
                {
                    data.Transaction.HIS_BILL_FUND = new List<HIS_BILL_FUND>();
                    foreach (var item in listFund)
                    {
                        if (item.AMOUNT > 0 && item.FUND_ID > 0)
                        {
                            HIS_BILL_FUND billFund = new HIS_BILL_FUND();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_BILL_FUND>(billFund, item);
                            data.Transaction.HIS_BILL_FUND.Add(billFund);
                        }
                    }
                    if (data.Transaction.HIS_BILL_FUND.Count == 0)
                    {
                        data.Transaction.HIS_BILL_FUND = null;
                    }
                    else
                    {
                        totalFund = data.Transaction.HIS_BILL_FUND.Sum(s => s.AMOUNT);
                    }
                }

                // nếu checkbox "có kết chuyển" bỏ check thì không tính số tiền hiện dư vào
                if (chkCoKetChuyen.CheckState == CheckState.Unchecked)
                {
                    data.PayAmount = (totalPatientPrice - (totalDiscount + totalFund));
                }
                else
                {
                    if (totalHienDu >= (totalPatientPrice - (totalDiscount + totalFund)))
                    {
                        data.Transaction.KC_AMOUNT = (totalPatientPrice - (totalDiscount + totalFund));
                        data.PayAmount = 0;
                    }
                    else
                    {
                        data.Transaction.KC_AMOUNT = totalHienDu;
                        data.PayAmount = (totalPatientPrice - (totalDiscount + totalFund)) - totalHienDu;
                    }
                }

                totalCanThu = data.PayAmount;
                if (totalHienDu == 0)
                {
                    data.Transaction.KC_AMOUNT = null;
                }
                if (payFormId == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK && spinTransferAmount.EditValue != null)
                {

                    if (spinTransferAmount.Value > data.Transaction.AMOUNT)
                    {
                        param.Messages.Add(String.Format("Số tiền chuyển khoản [{0}] lớn hơn số tiền cần thanh toán của bệnh nhân [{1}]", Inventec.Common.Number.Convert.NumberToStringRoundAuto(spinTransferAmount.Value, 2), Inventec.Common.Number.Convert.NumberToStringRoundAuto(data.Transaction.AMOUNT, 2)));
                        return false;
                    }
                    else
                    {
                        data.Transaction.TRANSFER_AMOUNT = spinTransferAmount.Value;
                    }

                }
                else if (payFormId == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT && spinTransferAmount.EditValue != null)
                {

                    if (spinTransferAmount.Value > data.Transaction.AMOUNT)
                    {
                        param.Messages.Add(String.Format("Số tiền quẹt thẻ [{0}] lớn hơn số tiền cần thanh toán của bệnh nhân [{1}]", Inventec.Common.Number.Convert.NumberToStringRoundAuto(spinTransferAmount.Value, 2), Inventec.Common.Number.Convert.NumberToStringRoundAuto(data.Transaction.AMOUNT, 2)));
                        return false;
                    }
                    else
                    {
                        data.Transaction.SWIPE_AMOUNT = spinTransferAmount.Value;
                    }
                }

                data.PayFormId = payFormId;
                data.TransactionTime = data.Transaction.TRANSACTION_TIME;
                V_HIS_ACCOUNT_BOOK accountBookRepay = null;
                V_HIS_ACCOUNT_BOOK accountBookDeposit = null;
                data.IsAutoRepay = chkAutoRepay.Checked;

                if (lblRepayAmount.Visible && frmTransactionBill.RepayAmount > 0)
                {
                    data.RepayAmount = RepayAmount;

                }

                if (cboAccountBookRepay.EditValue != null)
                {
                    accountBookRepay = this.ListAccountBookRepay != null && this.ListAccountBookRepay.Count > 0 ? this.ListAccountBookRepay.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBookRepay.EditValue)) : null;
                    if (accountBookRepay != null)
                    {
                        data.RepayAccountBookId = accountBookRepay.ID;
                    }

                    if (accountBookRepay != null && accountBookRepay.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                    {
                        data.RepayNumOrder = (long)spinNumOrderRepay.Value;
                    }
                }
                if (cboDepositBook.EditValue != null)
                {
                    accountBookDeposit = this.ListAccountBookDeposit != null && this.ListAccountBookDeposit.Count > 0 ? this.ListAccountBookDeposit.FirstOrDefault(o => o.ID == Convert.ToInt64(cboDepositBook.EditValue)) : null;
                    if (accountBookDeposit != null)
                    {
                        data.DepositAccountBookId = accountBookDeposit.ID;
                    }

                    if (accountBookDeposit != null && accountBookDeposit.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                    {
                        data.DepositNumOrder = (long)spnNumOrder.Value;
                    }
                }
                Inventec.Common.Logging.LogSystem.Info("ProcessSave 1.6");

                //thanh toán qua KEYPAY
                if (payFormId == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__KEYPAY)
                {
                    #region
                    CommonParam checkParam = new CommonParam();
                    var check = new BackendAdapter(checkParam).Post<bool>("api/HisTransaction/CheckBill", ApiConsumers.MosConsumer, data, checkParam);
                    if (!check)
                    {
                        param = checkParam;
                        success = false;
                        return success;
                    }

                    Inventec.Common.Logging.LogSystem.Info("ProcessSave 1.7");
                    HisTransReqBillSDO TransReqBillSDO = this.MapTranToReq(data);
                    TransReqBillSDO.PayAmount = (decimal)txtTotalAmount.EditValue;
                    TransReqBillSDO.RequestRoomId = this.currentModuleBase.RoomId;

                    Inventec.Common.Logging.LogSystem.Info("ProcessSave 1.8");

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => TransReqBillSDO), TransReqBillSDO));
                    var resultData = new BackendAdapter(param).Post<HIS_TRANS_REQ>("api/HisTransReq/CreateBill", ApiConsumers.MosConsumer, TransReqBillSDO, param);

                    Inventec.Common.Logging.LogSystem.Info("ProcessSave 1.9");


                    if (resultData != null)
                    {
                        success = true;
                        param.Messages.Add(" Yêu cầu thanh toán đã được ghi nhận");

                        RefreshSessionInfo();

                        ddBtnPrint.Enabled = true;
                        panelMenuPrintBill.Enabled = true;
                        btnSavePrint.Enabled = false;
                        btnSave.Enabled = false;
                        lciBtnSave.Enabled = false;
                        btnSaveAndSign.Enabled = false;
                    }
                    #endregion
                }
                else
                {
                    #region
                    if ((payFormId == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QUET_THE || payFormId == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT)
                    && chkConnectPOS.Checked == true &&
                    ((payFormId == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT && spinTransferAmount.Value > 0)
                    || (payFormId == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QUET_THE && decimal.Parse(lblReceiveAmount.Text) > 0)))
                    {
                        CommonParam checkParam = new CommonParam();
                        var check = new BackendAdapter(checkParam).Post<bool>("api/HisTransaction/CheckBill", ApiConsumers.MosConsumer, data, checkParam);
                        if (!check)
                        {
                            param = checkParam;
                            success = false;
                            return success;
                        }
                        OpenAppPOS();
                        WcfRequest wc = new WcfRequest(); // Khởi tạo data
                        if (payFormId == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT && spinTransferAmount.Value > 0)
                        {
                            wc.AMOUNT = (long)spinTransferAmount.Value; // Số tiền
                        }
                        else if (payFormId == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QUET_THE && decimal.Parse(lblReceiveAmount.Text) > 0)
                        {
                            wc.AMOUNT = (long)decimal.Parse(lblReceiveAmount.Text);
                        }
                        wc.billId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);
                        wc.creator = creator;
                        var json = JsonConvert.Serialize<WcfRequest>(wc);
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
                            chkConnectPOS.Checked = false;
                            XtraMessageBox.Show("Kiểm tra lại cấu hình NetTcpBinding_IService1", "Thông báo");
                            Inventec.Common.Logging.LogSystem.Error(ex);
                            return success;
                        }

                        var result = cll.Sale(System.Convert.ToBase64String(plainTextBytes));

                        if (result != null && result.RESPONSE_CODE == "00")
                        {

                            data.Transaction.POS_PAN = result.PAN;
                            data.Transaction.POS_CARD_HOLDER = result.NAME;
                            data.Transaction.POS_INVOICE = result.INVOICE.ToString();
                            data.Transaction.POS_RESULT_JSON = JsonConvert.Serialize<WcfRequest>(result);
                        }
                        else
                        {
                            if (result != null)
                            {
                                WaitingManager.Hide();
                                if (DevExpress.XtraEditors.XtraMessageBox.
                               Show(ResourceMessageLang.LoiThanhToan + "(Mã lỗi: " + result.ERROR + ")", ResourceMessageLang.ThongBao, System.Windows.Forms.MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK)
                                    return false;
                            }

                            return false;
                        }
                    }
                    #endregion
                    #region
                    // thanh toán qua thẻ
                    if (payFormId == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE)
                    {
                        // gọi WCF tab thẻ (POS)
                        decimal canThuThucTeThem = 0;
                        if (chkCoKetChuyen.CheckState == CheckState.Checked)
                        {
                            canThuThucTeThem = (totalPatientPrice - totalFund - this.totalDiscount) - totalHienDu;
                        }
                        else
                        {
                            canThuThucTeThem = (totalPatientPrice - totalFund - this.totalDiscount);
                        }

                        if (canThuThucTeThem != 0)
                        {
                            if (canThuThucTeThem < 0)
                            {
                                data.RepayAmount = -canThuThucTeThem;
                            }

                            Inventec.Common.Logging.LogSystem.Info("ProcessSave 1.12");
                            //kiểm tra trước khi thanh toán nếu là giao dịch qua thẻ
                            var check = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(UriStores.HIS_TRANSACTION_CHECK_BILL, ApiConsumers.MosConsumer, data, param);
                            if (!check)
                            {
                                return success;
                            }
                            else
                            {
                                if (canThuThucTeThem > 0)
                                {
                                    CARD.WCF.DCO.WcfSaleDCO SaleDCO = new CARD.WCF.DCO.WcfSaleDCO();
                                    SaleDCO.Amount = canThuThucTeThem;
                                    saleDCO = SaleCard(ref SaleDCO, param);
                                    // nếu gọi sang POS trả về false thì kết thúc
                                    if (saleDCO == null || (saleDCO.ResultCode == null || !saleDCO.ResultCode.Equals("00")))
                                    {
                                        MappingErrorTHE mappingErrorTHE = new Config.MappingErrorTHE();
                                        success = false;
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
                                        return success;
                                    }
                                    else
                                    {
                                        data.TigTransactionCode = saleDCO.TransactionCode;
                                        data.TigTransactionTime = saleDCO.TransactionTime;
                                        data.CardCode = saleDCO.TransServiceCode;
                                    }
                                }
                                else if (chkAutoRepay.Checked)
                                {
                                    CARD.WCF.DCO.WcfRefundDCO RefundDCO = new CARD.WCF.DCO.WcfRefundDCO();
                                    RefundDCO.Amount = -canThuThucTeThem;
                                    refundDCO = RefundCard(ref RefundDCO, param);
                                    // nếu gọi sang POS trả về false thì kết thúc
                                    if (refundDCO == null || (refundDCO.ResultCode == null || !refundDCO.ResultCode.Equals("00")))
                                    {
                                        MappingErrorTHE mappingErrorTHE = new Config.MappingErrorTHE();
                                        success = false;
                                        //param.Messages.Add(ResourceMessageLang.ThanhToanTheThatBai);
                                        if (refundDCO != null
                                            && !String.IsNullOrWhiteSpace(refundDCO.ResultCode)
                                            && mappingErrorTHE.dicMapping != null
                                            && mappingErrorTHE.dicMapping.Count > 0
                                            && mappingErrorTHE.dicMapping.ContainsKey(refundDCO.ResultCode))
                                        {
                                            param.Messages.Add(mappingErrorTHE.dicMapping[refundDCO.ResultCode]);
                                        }
                                        else if (refundDCO != null && String.IsNullOrWhiteSpace(refundDCO.ResultCode))
                                        {
                                            param.Messages.Add("Kiểm tra lại phần mềm kết nối thiết bị");
                                        }
                                        else if (refundDCO != null
                                            && !String.IsNullOrWhiteSpace(refundDCO.ResultCode)
                                            && mappingErrorTHE.dicMapping != null
                                            && mappingErrorTHE.dicMapping.Count > 0
                                            && !mappingErrorTHE.dicMapping.ContainsKey(refundDCO.ResultCode)
                                            )
                                        {
                                            param.Messages.Add("Kiểm tra lại phần mềm kết nối thiết bị");
                                        }
                                        return success;
                                    }
                                    else
                                    {
                                        data.TigTransactionCode = refundDCO.TransactionCode;
                                        data.TigTransactionTime = refundDCO.TransactionTime;
                                        data.CardCode = refundDCO.TransServiceCode;
                                    }
                                }
                                else
                                {
                                    WaitingManager.Hide();
                                    MessageManager.Show("Số tiền hoàn trả bệnh nhân lớn hơn 0. Bạn không thể chọn hình thức thanh toán qua thẻ.");
                                    return null;
                                }
                            }
                        }
                    }
                    #endregion

                    if (((txtTotalAmount.Value == 0 && (currentTreatment.TOTAL_DEPOSIT_AMOUNT ?? 0) == 0 && currentTreatment.IS_PAUSE == 1) ||
                        (txtTotalAmount.Value == 0 && (currentTreatment.TOTAL_DEPOSIT_AMOUNT ?? 0) > 0 && chkAutoRepay.Checked && currentTreatment.IS_PAUSE == 1)
                        ) && HisConfigCFG.AllowToCreateNoPriceTransaction != "1")
                    {

                        data.Transaction = null;
                        data.SereServBills = null;
                        //data.PAY_FORM_ID = payFormId;
                        data.TreatmentId = currentTreatment.ID;
                        //data.TRANSACTION_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(
                        //Convert.ToDateTime(dtTransactionTime.EditValue).ToString("yyyyMMddHHmm") + "00");
                        //data.CASHIER_LOGINAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        //data.CASHIER_USERNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                    }

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisTransactionBillResultSDO>(UriStores.HIS_TRANSACTION_CREATE_BILL, ApiConsumers.MosConsumer, data, param);

                    if (rs != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("HisTransaction/CreateBill rs != null");
                        TransactionBillResultSDO = rs;
                        success = true;
                        AddLastAccountToLocal();
                        this.resultTranBill = rs.TransactionBill;
                        if (rs.TransactionRepay != null && accountBookRepay != null && accountBookRepay.IS_NOT_GEN_TRANSACTION_ORDER != 1)
                            spinNumOrderRepay.Value = rs.TransactionRepay.NUM_ORDER;
                        if (rs.TransactionDeposit != null && accountBookDeposit != null)
                            spnNumOrder.Value = rs.TransactionDeposit.NUM_ORDER;
                        SetBillSuccessControl();
                        CalcuHienDu();
                        UpdateDictionaryNumOrderAccountBook(accountBook, spinTongTuDen.Value);
                        UpdateDictionaryNumOrderAccountBook(accountBookRepay, spinNumOrderRepay.Value);
                        UpdateDictionaryNumOrderAccountBook(accountBookDeposit, spnNumOrder.Value);
                        RefreshSessionInfo();

                        ddBtnPrint.Enabled = true;
                        panelMenuPrintBill.Enabled = true;
                        btnSavePrint.Enabled = false;
                        btnSave.Enabled = false;
                        lciBtnSave.Enabled = false;
                        btnSaveAndSign.Enabled = false;

                        if (isLuuKy && TransactionBillConfig.InvoiceTypeCreate == invoiceTypeCreate__CreateInvoiceVnpt)
                        {
                            if (isnotPrintMPS000111 == false)
                            {
                                HIS_TRANSACTION tran = new HIS_TRANSACTION();
                                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(tran, resultTranBill);
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

                        if (rs.TransactionRepay != null && chkInHoanUng.Checked)
                        {
                            printNowMps000113 = true;
                            onClickPhieuThuHoanUng(null, null);
                        }

                        if (chkPrintPrescription.Checked)
                        {
                            //nếu tự động đóng sẽ không tạo thread để in 
                            if (chkAutoClose.Checked)
                            {
                                PrintPrescription();
                            }
                            else
                            {
                                CreateThreadPrintPrescription();
                            }
                        }
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("HisTransaction/CreateBill rs == null");
                        btnSave.Enabled = true;
                        lciBtnSave.Enabled = true;

                        // nếu gọi MOS thất bại thì gọi WCF hủy giao dịch
                        if (payFormId == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE)
                        {
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

                            if (refundDCO != null)
                            {
                                CARD.WCF.DCO.WcfVoidDCO WcfVoidDCO = new CARD.WCF.DCO.WcfVoidDCO();
                                WcfVoidDCO.Amount = refundDCO.Amount;
                                WcfVoidDCO.TransactionCode = refundDCO.TransactionCode;
                                var resultWcf = VoidCard(ref WcfVoidDCO);
                                if (resultWcf == null || (resultWcf != null && !resultWcf.ResultCode.Equals("00")))
                                {
                                    success = false;
                                    Inventec.Common.Logging.LogSystem.Info("[result code]: " + resultWcf.ResultCode);
                                    param.Messages.Add(ResourceMessageLang.HuyGiaoDichHoanUngTheThatBai + WcfVoidDCO.TransactionCode);
                                }
                            }
                        }

                        //if (!String.IsNullOrWhiteSpace(data.Transaction.INVOICE_SYS) || !String.IsNullOrWhiteSpace(data.Transaction.INVOICE_CODE) || !String.IsNullOrWhiteSpace(data.Transaction.EINVOICE_NUM_ORDER))
                        //{
                        //    success = false;
                        //    Inventec.Common.Logging.LogSystem.Info(string.Format("Tao hoa don dien tu thanh cong. So hoa don: {0}, Ma: {1}", data.Transaction.EINVOICE_NUM_ORDER, data.Transaction.INVOICE_CODE));
                        //    param.Messages.Add(string.Format(ResourceMessageLang.TaoThanhCongHoaDonDienTu, data.Transaction.EINVOICE_NUM_ORDER));
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }
         
        public bool OpenAppPOS()
        {
            try
            {
                if (IsProcessOpen("WCF"))
                {
                    return true;
                }
                else
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();

                    startInfo.FileName = Application.StartupPath + @"\Integrate\POS.WCFService\WCF.exe";
                    nameFile = startInfo.FileName;
                    Inventec.Common.Logging.LogSystem.Info("FileName " + startInfo.FileName);
                    Process.Start(startInfo);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return false;
        }

        private bool IsProcessOpen(string name)
        {
            try
            {
                var processByNames = System.Diagnostics.Process.GetProcesses().Where(o => o.ProcessName.Contains(name)).ToList();
                if (processByNames != null && processByNames.Count >= 2)
                {
                    return true;
                }
                return false;
                //foreach (Process clsProcess in Process.GetProcesses())
                //{
                //    if (clsProcess.ProcessName.Contains(name))
                //    {
                //        return true;
                //    }
                //}               
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }


        }
        private void btnPosConfig_Click(object sender, EventArgs e)
        {
            try
            {
                OpenAppPOS();
                try
                {
                    cll = new WcfClient();
                }
                catch (Exception ex)
                {
                    chkConnectPOS.Checked = false;
                    XtraMessageBox.Show("Kiểm tra lại cấu hình NetTcpBinding_IService1", "Thông báo");
                    return;
                }
                cll.cauhinh();
            }
            catch (Exception ex)
            {
                chkConnectPOS.Checked = false;
                XtraMessageBox.Show("Cấu hình thất bại", "Thông báo");
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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

                var accountBook = ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                if (accountBook != null)
                {
                    //sổ chưa có sẽ add thêm vào
                    //xóa bỏ sổ cũ cùng loại
                    if (!GlobalVariables.LastAccountBook.Exists(o => o.ID == accountBook.ID))
                    {
                        var lstSameType = GlobalVariables.LastAccountBook.Where(o => o.IS_FOR_BILL == 1 && o.ID != accountBook.ID).ToList();// && o.BILL_TYPE_ID == accountBook.BILL_TYPE_ID).ToList();
                        if (lstSameType != null && lstSameType.Count > 0)
                        {
                            foreach (var item in lstSameType)
                            {
                                GlobalVariables.LastAccountBook.Remove(item);
                            }
                        }
                        GlobalVariables.LastAccountBook.Add(accountBook);
                    }
                }
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
                string FindTreatmentCode = txtFindTreatmentCode.Text;
                txtFindTreatmentCode.Text = "";
                btnSearch_Click(null, null);
                txtFindTreatmentCode.Text = FindTreatmentCode;
                txtFindTreatmentCode.Focus();
                txtFindTreatmentCode.SelectAll();
                this.hienHoaDonNhap = true;
                GeneratePopupMenu();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCSavePrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSavePrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
                btnNew.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCSaveSign_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSaveAndSign_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetBillSuccessControl()
        {
            try
            {
                if (this.resultTranBill != null)
                {
                    cboAccountBook.EditValue = this.resultTranBill.ACCOUNT_BOOK_ID;
                    txtTotalAmount.Value = this.resultTranBill.AMOUNT;
                    txtDiscount.Value = this.resultTranBill.EXEMPTION ?? 0;
                    if (this.resultTranBill.AMOUNT > 0)
                    {
                        txtDiscountRatio.Value = ((this.resultTranBill.EXEMPTION ?? 0) / this.resultTranBill.AMOUNT) * 100;
                    }
                }
                else
                {
                    throw new NullReferenceException("this.resultTranBill is null");
                }
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

                if (this.hienHoaDonNhap && TransactionBillConfig.InvoiceTypeCreate == invoiceTypeCreate__CreateInvoiceVnpt)
                {
                    menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__BTN_DROP_DOWN__ITEM_HOA_DON_NHAP", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickHoaDonNhap)));
                }
                else
                {
                    menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__BTN_DROP_DOWN__ITEM_PHIEU_THU_THANH_TOAN", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickPhieuThuThanhToan)));

                    menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__BTN_DROP_DOWN__ITEM_PHIEU_THU_TT_THEO_YEU_CAU", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickPhieuThuThanhToanTheoYeuCau)));

                    menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__BTN_DROP_DOWN__ITEM_PHIEU_THU_TT_CHI_TIET_DICH_VU", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickPhieuThuThanhToanChiTietDichVu)));

                    menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__BTN_DROP_DOWN__ITEM_BIEN_LAI_PHI_LE_PHI", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickBienLaiThuPhiLePhi)));

                    menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__BTN_DROP_DOWN__ITEM_IN_PHIEU_CHI_DINH", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickPhieuChiDinh)));

                    menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__BTN_DROP_DOWN__ITEM_IN_HOA_DON_DIEN_TU", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickInHoaDonDienTu)));
                    if (resultTranBill != null && resultTranBill.EINVOICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__VNPT)
                    {
                        menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__BTN_DROP_DOWN__ITEM_CHUYEN_DOI_HOA_DON_DIEN_TU", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickChuyenDoiHoaDonDienTu)));
                    }

                    if (this.currentTreatment != null && this.currentTreatment.IS_PAUSE == 1)
                    {
                        menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__BTN_DROP_DOWN__ITEM_IN_HOAN_UNG_THANH_TOAN", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickInThanhToanHoanUng)));
                    }
                    menu.Items.Add(new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__BTN_DROP_DOWN__PHIEU_THU_HOAN_UNG_MPS113", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickPhieuThuHoanUng)));
                }
                ddBtnPrint.DropDownControl = menu;
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
                dataInput.Treatment = this.currentTreatment;
                dataInput.SereServs = new List<V_HIS_SERE_SERV_5>();
                dataInput.Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
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

        private void onClickHoaDonNhap(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(MPS.Processor.Mps000431.PDO.Mps000431PDO.printTypeCode, DeletegatePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

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

        private void InPhieuThuHoanUng(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                // lấy giao dịch hoàn ứng mới nhất:
                MOS.Filter.HisTransactionViewFilter filter = new HisTransactionViewFilter();
                filter.TREATMENT_ID = this.treatmentId;
                filter.TRANSACTION_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__HU };
                var transactionList = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                if (transactionList == null || transactionList.Count == 0)
                {
                    MessageBox.Show("Hồ sơ điều trị chưa có giao dịch hoàn ứng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var transactionPrint = transactionList.OrderByDescending(o => o.TRANSACTION_TIME).FirstOrDefault();
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                if (transactionPrint.IS_CANCEL == 1)
                {
                    if (transactionPrint.CREATOR != loginName && transactionPrint.CANCEL_LOGINNAME != loginName && !CheckLoginAdmin.IsAdmin(loginName))
                    {
                        MessageManager.Show("Bạn không có quyền in giao dịch đã hủy");
                        return;
                    }
                }
                WaitingManager.Show();

                //BỎ repay
                //HisTransactionViewFilter repayFilter = new HisTransactionViewFilter();
                //repayFilter.ID = transactionPrint.ID;
                //V_HIS_TRANSACTION repay = null;
                //var listRepay = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, repayFilter, null);
                //if (listRepay == null || listRepay.Count != 1)
                //{
                //    throw new Exception("Khong lay duoc V_HIS_REPAY theo transactionId: " + transactionPrint.ID);
                //}
                //repay = listRepay.FirstOrDefault();
                V_HIS_PATIENT patient = null;

                if (transactionPrint.TDL_PATIENT_ID.HasValue)
                {
                    HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                    patientFilter.ID = transactionPrint.TDL_PATIENT_ID;
                    var listPatient = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, patientFilter, null);
                    if (listPatient == null || listPatient.Count != 1)
                    {
                        throw new NullReferenceException("Get VHisPatient by TdlPatientId null or count != 1" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listPatient), listPatient));
                    }
                    patient = listPatient.First();
                }

                decimal ratio = 0;
                var PatyAlterBhyt = new V_HIS_PATIENT_TYPE_ALTER();
                PrintGlobalStore.LoadCurrentPatientTypeAlter(transactionPrint.TREATMENT_ID.Value, 0, ref PatyAlterBhyt);
                if (PatyAlterBhyt != null && !String.IsNullOrEmpty(PatyAlterBhyt.HEIN_CARD_NUMBER))
                {
                    ratio = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(PatyAlterBhyt.HEIN_TREATMENT_TYPE_CODE, PatyAlterBhyt.HEIN_CARD_NUMBER, PatyAlterBhyt.LEVEL_CODE, PatyAlterBhyt.RIGHT_ROUTE_CODE) ?? 0;
                }

                HisDepartmentTranLastFilter departLastFilter = new HisDepartmentTranLastFilter();
                departLastFilter.TREATMENT_ID = transactionPrint.TREATMENT_ID.Value;
                departLastFilter.BEFORE_LOG_TIME = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                var departmentTran = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, departLastFilter, null);

                CommonParam paramtreatment = new CommonParam();
                HisTreatmentFeeViewFilter filterTreat = new HisTreatmentFeeViewFilter();
                filterTreat.ID = transactionPrint.TREATMENT_ID;
                var TreatmentFee = new Inventec.Common.Adapter.BackendAdapter(paramtreatment).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, filterTreat, paramtreatment);

                HisTransactionViewFilter filterTran = new HisTransactionViewFilter();
                filterTran.TREATMENT_ID = transactionPrint.TREATMENT_ID;
                //filterTran.TRANSACTION_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT };
                filterTran.IS_CANCEL = false;
                List<V_HIS_TRANSACTION> transa = new BackendAdapter(paramtreatment).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, filterTran, paramtreatment);
                if (transa == null) transa = new List<V_HIS_TRANSACTION>();

                MPS.Processor.Mps000113.PDO.Mps000113PDO rdo = new MPS.Processor.Mps000113.PDO.Mps000113PDO(
                    transactionPrint,
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

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((transactionPrint != null ? transactionPrint.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2 || printNowMps000113)
                {
                    //printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, null);
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                    if (result && chkAutoClose.CheckState == CheckState.Checked)
                        this.Close();
                }
                else
                {
                    //printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, null);
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
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
            dataInput.Treatment = this.currentTreatment;
            dataInput.SereServs = new List<V_HIS_SERE_SERV_5>();
            dataInput.Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
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
                dataInput.InvoiceCode = resultTranBill.INVOICE_CODE;
                dataInput.NumOrder = resultTranBill.NUM_ORDER;
                dataInput.SymbolCode = resultTranBill.SYMBOL_CODE;
                dataInput.TemplateCode = resultTranBill.TEMPLATE_CODE;
                dataInput.TransactionTime = resultTranBill.EINVOICE_TIME ?? resultTranBill.TRANSACTION_TIME;
                dataInput.ENumOrder = resultTranBill.EINVOICE_NUM_ORDER;
                dataInput.EinvoiceTypeId = resultTranBill.EINVOICE_TYPE_ID;
                dataInput.Treatment = this.currentTreatment;
                dataInput.Transaction = new HIS_TRANSACTION();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(dataInput.Transaction, resultTranBill);
                dataInput.SereServs = new List<V_HIS_SERE_SERV_5>();
                dataInput.Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
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
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuHoanUng_MPS000113:
                        InPhieuThuHoanUng(printCode, fileName, ref result);
                        break;
                    case MPS.Processor.Mps000431.PDO.Mps000431PDO.printTypeCode:
                        InHoaDonNhap(printCode, fileName, ref result);
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

        private void InHoaDonNhap(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();

                V_HIS_ACCOUNT_BOOK AccountBook = new V_HIS_ACCOUNT_BOOK();
                if (cboAccountBook.EditValue != null)
                {
                    AccountBook = BackendDataWorker.Get<V_HIS_ACCOUNT_BOOK>().FirstOrDefault(o => o.ID == (long)cboAccountBook.EditValue);
                }

                V_HIS_TRANSACTION transaction = new V_HIS_TRANSACTION();

                transaction.BUYER_NAME = txtBuyerName.Text;
                transaction.BUYER_TAX_CODE = txtBuyerTaxCode.Text;
                transaction.BUYER_ACCOUNT_NUMBER = txtBuyerAccountNumber.Text;
                transaction.BUYER_ORGANIZATION = txtBuyerOrganization.Text;
                transaction.BUYER_ADDRESS = txtBuyerAddress.Text;
                //transaction.BUYER_EMAIL = txtBuyerEmail.Text;
                if (dtTransactionTime.EditValue != null && dtTransactionTime.DateTime != DateTime.MinValue)
                {
                    transaction.TRANSACTION_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dtTransactionTime.DateTime.ToString("yyyyMMdd") + "000000");
                }
                transaction.PAY_FORM_NAME = cboPayForm.Text;

                if (AccountBook != null)
                {
                    transaction.SYMBOL_CODE = AccountBook.SYMBOL_CODE;
                    transaction.TEMPLATE_CODE = AccountBook.TEMPLATE_CODE;
                }


                List<V_HIS_SERE_SERV_5> sereServBills = new List<V_HIS_SERE_SERV_5>();
                var sereServBillADOs = ssTreeProcessor.GetListCheck(this.ucSereServTree);

                if (sereServBillADOs != null && sereServBillADOs.Count > 0)
                {
                    foreach (var item in sereServBillADOs)
                    {
                        V_HIS_SERE_SERV_5 sereServBill = new V_HIS_SERE_SERV_5();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV_5>(sereServBill, item);
                        sereServBills.Add(sereServBill);
                    }
                }

                HIS_TRANSACTION hisTransaction = new HIS_TRANSACTION();

                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(hisTransaction, transaction);

                ElectronicBillDataInput dataInput = new ElectronicBillDataInput();

                if (!string.IsNullOrEmpty(lblReceiveAmount.Text))
                {
                    dataInput.Amount = decimal.Parse(lblReceiveAmount.Text);
                }

                dataInput.Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                dataInput.Discount = txtDiscount.Value;
                dataInput.DiscountRatio = txtDiscountRatio.Value;
                dataInput.PaymentMethod = cboPayForm.Text;
                dataInput.SereServs = sereServBills;
                dataInput.Treatment = this.currentTreatment;
                dataInput.Currency = "VND";
                dataInput.Transaction = hisTransaction;
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

                long Template = long.Parse(TransactionBillConfig.InvoiceTemplateCreate);
                TemplateEnum.TYPE typ = TemplateEnum.TYPE.Template1;
                try
                {
                    typ = (TemplateEnum.TYPE)Template;
                }
                catch (Exception)
                {
                    typ = TemplateEnum.TYPE.Template1;
                }

                IRunTemplate iRunTemplate = TemplateFactory.MakeIRun(typ, dataInput);

                var listProduct = iRunTemplate.Run();

                List<MPS.Processor.Mps000431.PDO.ProductADO> lstProductADO = new List<MPS.Processor.Mps000431.PDO.ProductADO>();
                var lst = (List<ProductBase>)listProduct;
                foreach (var item in lst)
                {
                    MPS.Processor.Mps000431.PDO.ProductADO ado = new MPS.Processor.Mps000431.PDO.ProductADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000431.PDO.ProductADO>(ado, item);
                    lstProductADO.Add(ado);
                }

                MPS.Processor.Mps000431.PDO.Mps000431PDO rdo = new MPS.Processor.Mps000431.PDO.Mps000431PDO(transaction, lstProductADO);

                WaitingManager.Hide();

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.currentTreatment != null ? this.currentTreatment.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                if (isPrintNow)
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
                    ssFilter.TREATMENT_ID = this.treatmentId;
                    listSereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, ssFilter, null);

                    if (Print106Type_Expend == "1")
                    {
                        if (listSereServ != null && listSereServ.Count > 0)
                        {
                            HisSereServViewFilter ssFilter1 = new HisSereServViewFilter();
                            ssFilter1.TREATMENT_ID = this.treatmentId;
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
                    ssFilter.TREATMENT_ID = this.treatmentId;
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
                ptAlterAppFilter.TreatmentId = this.treatmentId.Value;
                ptAlterAppFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, ptAlterAppFilter, null);

                // tính mức hưởng của thẻ
                string levelCode = HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT;
                string ratio_text = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentPatientTypeAlter.HEIN_CARD_NUMBER, currentPatientTypeAlter.LEVEL_CODE, currentPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100) + "";

                HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = this.currentTreatment.PATIENT_ID;
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

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                    if (isPrintNow)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                        if (result && chkAutoClose.CheckState == CheckState.Checked)
                            this.Close();
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
                if (this.currentTreatment == null)
                    return;
                WaitingManager.Show();
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }
                CommonParam paramtreatment = new CommonParam();
                HisTreatmentFeeViewFilter filterTreat = new HisTreatmentFeeViewFilter();
                filterTreat.ID = this.currentTreatment.ID;
                var TreatmentFee = new Inventec.Common.Adapter.BackendAdapter(paramtreatment).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, filterTreat, paramtreatment);

                CommonParam param1 = new CommonParam();
                HisDepartmentViewFilter filterDepar = new HisDepartmentViewFilter();
                filterDepar.ID = this.currentTreatment.END_DEPARTMENT_ID;
                var department = new BackendAdapter(param1).Get<List<V_HIS_DEPARTMENT>>("api/HisDepartment/GetView", ApiConsumers.MosConsumer, filterDepar, param1);

                HisTransactionViewFilter filterTran = new HisTransactionViewFilter();
                filterTran.TREATMENT_ID = currentTreatment.ID;
                filterTran.TRANSACTION_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT };
                filterTran.IS_CANCEL = false;
                List<V_HIS_TRANSACTION> transa = new BackendAdapter(param1).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, filterTran, param1);
                if (transa == null) transa = new List<V_HIS_TRANSACTION>();

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.currentTreatment != null ? this.currentTreatment.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                MPS.Processor.Mps000361.PDO.Mps000361PDO pdo = new MPS.Processor.Mps000361.PDO.Mps000361PDO(TreatmentFee.FirstOrDefault(), transa, department.FirstOrDefault());
                MPS.ProcessorBase.Core.PrintData printData = null;
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                    if (result && chkAutoClose.CheckState == CheckState.Checked)
                        this.Close();
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

        private void InPhieuThuThanhToan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                DefaultDataPrintMps111();
                if (this.resultTranBill == null)
                {
                    decimal totalReceive = ((this.currentTreatment.TOTAL_DEPOSIT_AMOUNT ?? 0) + (this.currentTreatment.TOTAL_BILL_AMOUNT ?? 0) - (this.currentTreatment.TOTAL_BILL_TRANSFER_AMOUNT ?? 0) - (this.currentTreatment.TOTAL_BILL_FUND ?? 0) - (this.currentTreatment.TOTAL_REPAY_AMOUNT ?? 0)) - (this.currentTreatment.TOTAL_BILL_EXEMPTION ?? 0);

                    decimal totalReceiveMore = (this.currentTreatment.TOTAL_PATIENT_PRICE ?? 0) - totalReceive - (this.currentTreatment.TOTAL_BILL_FUND ?? 0) - (this.currentTreatment.TOTAL_BILL_EXEMPTION ?? 0);

                    if (HisConfigCFG.EnableSaveOption == "1" && totalReceiveMore <= 0)
                    {
                        #region
                        HisPatientTypeAlterViewAppliedFilter patyAlterAppliedFilter = new HisPatientTypeAlterViewAppliedFilter();
                        patyAlterAppliedFilter.InstructionTime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                        patyAlterAppliedFilter.TreatmentId = currentTreatment.ID;
                        var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, patyAlterAppliedFilter, null);
                        if (currentPatientTypeAlter == null)
                        {
                            Inventec.Common.Logging.LogSystem.Info("Khong lay duoc PatientTypeAlterApplied: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentTreatment.TREATMENT_CODE), currentTreatment.TREATMENT_CODE));
                        }
                        //
                        HisDepartmentTranLastFilter departLastFilter = new HisDepartmentTranLastFilter();
                        departLastFilter.TREATMENT_ID = currentTreatment.ID;
                        departLastFilter.BEFORE_LOG_TIME = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                        var departmentTran = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, departLastFilter, null);
                        //
                        //2
                        V_HIS_PATIENT patient = new V_HIS_PATIENT();

                        HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                        patientFilter.ID = currentTreatment.PATIENT_ID;
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

                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((currentTreatment != null ? currentTreatment.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

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

                        if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                            if (result && chkAutoClose.CheckState == CheckState.Checked)
                                this.Close();
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

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((resultTranBill != null ? resultTranBill.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);
                    WaitingManager.Hide();
                    if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                        if (result && chkAutoClose.CheckState == CheckState.Checked)
                            this.Close();
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
                ptAlterAppFilter.TreatmentId = this.treatmentId.Value;
                ptAlterAppFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, ptAlterAppFilter, null);

                // tính mức hưởng của thẻ
                string levelCode = HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT;
                string ratio_text = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentPatientTypeAlter.HEIN_CARD_NUMBER, currentPatientTypeAlter.LEVEL_CODE, currentPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100) + "";

                HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = this.currentTreatment.PATIENT_ID;
                var patients = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, null);
                V_HIS_PATIENT patient = new V_HIS_PATIENT();
                if (patients != null && patients.Count > 0)
                {
                    patient = patients.FirstOrDefault();
                }

                HisSereServViewFilter ssFilter = new HisSereServViewFilter();
                ssFilter.IDs = hisSSBills.Select(s => s.SERE_SERV_ID).ToList();
                ssFilter.TREATMENT_ID = this.treatmentId;
                var listSereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, ssFilter, null);
                WaitingManager.Hide();

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.resultTranBill != null ? this.resultTranBill.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

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
                    ssFilter.TREATMENT_ID = this.treatmentId;
                    listSereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, ssFilter, null);

                    if (Print106Type_Expend == "1")
                    {
                        if (listSereServ != null && listSereServ.Count > 0)
                        {
                            HisSereServViewFilter ssFilter1 = new HisSereServViewFilter();
                            ssFilter1.TREATMENT_ID = this.treatmentId;
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
                    ssFilter.TREATMENT_ID = this.treatmentId;
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
                ptAlterAppFilter.TreatmentId = this.treatmentId.Value;
                ptAlterAppFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, ptAlterAppFilter, null);

                // tính mức hưởng của thẻ
                string levelCode = HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT;
                string ratio_text = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentPatientTypeAlter.HEIN_CARD_NUMBER, currentPatientTypeAlter.LEVEL_CODE, currentPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100) + "";

                HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = this.currentTreatment.PATIENT_ID;
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

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.resultTranBill != null ? this.resultTranBill.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode) && !String.IsNullOrEmpty(GlobalVariables.dicPrinter[printTypeCode]))
                    {
                        if (isPrintNow)
                        {
                            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                            if (chkAutoClose.CheckState == CheckState.Checked && result)
                                this.Close();
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

        private void InBienlaiThuPhiLePhi(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.resultTranBill == null)
                    return;
                HisPatientTypeAlterViewAppliedFilter ptAlterAppFilter = new HisPatientTypeAlterViewAppliedFilter();
                ptAlterAppFilter.TreatmentId = this.treatmentId.Value;
                ptAlterAppFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, ptAlterAppFilter, null);

                HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = this.currentTreatment.PATIENT_ID;
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

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.resultTranBill != null ? this.resultTranBill.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

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
                ptAlterAppFilter.TreatmentId = this.treatmentId.Value;
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
                string levelCode = HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT;
                string ratio_text = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentPatientTypeAlter.HEIN_CARD_NUMBER, currentPatientTypeAlter.LEVEL_CODE, currentPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100) + "";

                HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = this.currentTreatment.PATIENT_ID;
                var patients = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, null);
                V_HIS_PATIENT patient = new V_HIS_PATIENT();
                if (patients != null && patients.Count > 0)
                {
                    patient = patients.FirstOrDefault();
                }

                HisSereServViewFilter sereServFilter = new HisSereServViewFilter();
                sereServFilter.TREATMENT_ID = this.treatmentId.Value;
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

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.resultTranBill != null ? this.resultTranBill.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                    bool AssignServicePrintTEST = (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.AssignServicePrintTEST") == "1");
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

                    bool AssignServicePrintCDHA = (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.AssignServicePrintCDHA") == "1");
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

        #region InVaCloseForm
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

        private void LoadSereServDeposit()
        {
            try
            {
                if(this.resultTranBill != null)
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
                            txtBuyerEmail.Text = patientsPrint.EMAIL;          
                        }
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

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((resultTranBill != null ? resultTranBill.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

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
                if (result && chkAutoClose.CheckState == CheckState.Checked)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private ElectronicBillResult TaoHoaDonDienTuBenThu3CungCap(HIS_TRANSACTION transaction)
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
                dataInput.Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                dataInput.Discount = txtDiscount.Value;
                dataInput.DiscountRatio = txtDiscountRatio.Value;
                dataInput.PaymentMethod = cboPayForm.Text;
                dataInput.SereServs = sereServBills;
                dataInput.Treatment = this.currentTreatment;
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

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((resultTranBill != null ? resultTranBill.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);


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
                            var fileUploadInfo = FileUpload.UploadFile(GlobalVariables.APPLICATION_CODE, "FILESIGNED", outStreamResult, fileNameUpload);
                            if (fileUploadInfo != null)
                            {
                                //Cập nhật lại trường FILE_URL, FILE_NAME của bảng Bill
                                this.resultTranBill.FILE_URL = fileUploadInfo.Url;
                                this.resultTranBill.FILE_NAME = fileNameUpload;
                                //Review
                                HIS_TRANSACTION updateFile = new HIS_TRANSACTION();
                                AutoMapper.Mapper.CreateMap<V_HIS_TRANSACTION, HIS_TRANSACTION>();
                                updateFile = AutoMapper.Mapper.Map<HIS_TRANSACTION>(this.resultTranBill);
                                V_HIS_TRANSACTION rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<V_HIS_TRANSACTION>(UriStores.HIS_TRANSACTION_UPDATE_FILE, ApiConsumers.MosConsumer, updateFile, param);
                                if (rs != null && !String.IsNullOrEmpty(rs.FILE_URL))
                                {
                                    Inventec.Common.Logging.LogSystem.Debug("Ky dien tu cho giao dich hoa don thanh toan thanh cong. TRANSACTION_CODE = " + this.resultTranBill.TRANSACTION_CODE + ", Fss_Url_Signed_File = " + fileUploadInfo.Url);
                                    result = true;
                                    if (chkAutoClose.CheckState == CheckState.Checked)
                                        this.Close();
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
                    param.Messages.Add(Base.ResourceMessageLang.TaoThanhToanThanhCong_TuyNhienThucHienKyDienTuThatBai);
                    MessageManager.Show(param, result);
                    if (chkAutoClose.CheckState == CheckState.Checked)
                        this.Close();
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
        #endregion

        private void SetEnableButtonSave(bool? enable)
        {
            try
            {
                btnSave.Enabled = enable ?? true;
                btnSaveAndSign.Enabled = enable ?? true;
                btnSavePrint.Enabled = enable ?? true;
                btnAddSignEmr.Enabled = enable ?? true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThreadPrintPrescription()
        {
            System.Threading.Thread printPrescription = new System.Threading.Thread(PrintPrescription);
            try
            {
                printPrescription.Start();
            }
            catch (Exception ex)
            {
                printPrescription.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintPrescription()
        {
            try
            {
                if (this.resultTranBill != null && this.resultTranBill.TREATMENT_ID.HasValue)
                {
                    CommonParam param = new CommonParam();
                    HisTreatmentFilter treaFilter = new HisTreatmentFilter();
                    treaFilter.ID = this.resultTranBill.TREATMENT_ID;
                    var treatments = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treaFilter, param);
                    if (treatments != null && treatments.Count > 0 && treatments.First().IS_ACTIVE == 0)
                    {
                        HisServiceReqFilter reqFilter = new HisServiceReqFilter();
                        reqFilter.TREATMENT_ID = this.resultTranBill.TREATMENT_ID;
                        reqFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK;
                        var listServiceReq = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, reqFilter, param);
                        if (listServiceReq != null && listServiceReq.Count > 0)
                        {
                            MOS.SDO.OutPatientPresResultSDO outPatientPresResultSDO = new OutPatientPresResultSDO();

                            outPatientPresResultSDO.ServiceReqs = listServiceReq;

                            param = new CommonParam();

                            //Get ServiceReqMety
                            HisServiceReqMetyFilter hisServiceReqMetyFilter = new HisServiceReqMetyFilter();
                            hisServiceReqMetyFilter.SERVICE_REQ_IDs = listServiceReq.Select(o => o.ID).ToList();
                            var listHisServiceReqMety = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_METY>>("api/HisServiceReqMety/Get", ApiConsumers.MosConsumer, hisServiceReqMetyFilter, param);
                            outPatientPresResultSDO.ServiceReqMeties = listHisServiceReqMety;

                            //Get ServiceReqMaty
                            HisServiceReqMatyFilter hisServiceReqMatyFilter = new HisServiceReqMatyFilter();
                            hisServiceReqMatyFilter.SERVICE_REQ_IDs = listServiceReq.Select(o => o.ID).ToList();
                            var listHisServiceReqMaty = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_MATY>>("api/HisServiceReqMaty/Get", ApiConsumers.MosConsumer, hisServiceReqMatyFilter, param);
                            outPatientPresResultSDO.ServiceReqMaties = listHisServiceReqMaty;

                            //Get ExpMest
                            HisExpMestFilter hisExpMestFilter = new HisExpMestFilter();
                            hisExpMestFilter.SERVICE_REQ_IDs = listServiceReq.Select(o => o.ID).ToList();
                            var listHisExpMest = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, hisExpMestFilter, param);
                            outPatientPresResultSDO.ExpMests = listHisExpMest;

                            //Get ExpMestMedicine
                            HisExpMestMedicineFilter hisExpMestMedicineFilter = new HisExpMestMedicineFilter();
                            hisExpMestMedicineFilter.TDL_SERVICE_REQ_IDs = listServiceReq.Select(o => o.ID).ToList();
                            var listHisExpMestMedicine = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumers.MosConsumer, hisExpMestMedicineFilter, param);
                            outPatientPresResultSDO.Medicines = listHisExpMestMedicine;

                            //Get ExpMestMaterial
                            HisExpMestMaterialFilter hisExpMestMaterialFilter = new HisExpMestMaterialFilter();
                            hisExpMestMaterialFilter.TDL_SERVICE_REQ_IDs = listServiceReq.Select(o => o.ID).ToList();
                            var listHisExpMestMaterial = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/Get", ApiConsumers.MosConsumer, hisExpMestMaterialFilter, param);
                            outPatientPresResultSDO.Materials = listHisExpMestMaterial;

                            List<MOS.SDO.OutPatientPresResultSDO> listOutPatientPresResultSDO = new List<OutPatientPresResultSDO> { outPatientPresResultSDO };

                            var PrintPresProcessor = new Library.PrintPrescription.PrintPrescriptionProcessor(listOutPatientPresResultSDO, this.currentModule);
                            PrintPresProcessor.Print("Mps000234", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAddSignEmr_Click(object sender, EventArgs e)
        {
            isEmr = true;
            bool? success = false;
            try
            {
                PrintMps279 = false;
                this.positionHandleControl = -1;
                if (!btnSavePrint.Enabled)
                    return;
                SetEnableButtonSave(false);
                if (HisConfigCFG.AutoCreateDepositTransaction && decimal.Parse(lblReceiveAmount.Text) > 0 && cboDepositBook.Enabled && cboDepositBook.EditValue == null)
                {
                    XtraMessageBox.Show("Bắt buộc phải chọn sổ tạm ứng khi thanh toán", "Thông báo");
                    Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule validate = new Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule();
                    validate.editor = this.cboDepositBook;
                    validate.ErrorText = "Trường dữ liệu bắt buộc";
                    validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    dxValidationProvider1.SetValidationRule(this.cboDepositBook, validate);
                }
                if (!dxValidationProvider1.Validate() || this.treatmentId == null)
                {
                    return;
                }
                dxValidationProvider1.SetValidationRule(this.cboDepositBook, null);
                if (HisConfigCFG.AttachAssignPrintWarningOption == "1")
                {
                    Inventec.Common.Logging.LogSystem.Debug("HisConfigCFG.AttachAssignPrintWarningOption == 1");
                    HIS_TREATMENT treatment = GetTreatment(this.treatmentId);
                    if (treatment != null && treatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                    {
                        CommonParam paramCommon = new CommonParam();
                        var result = new BackendAdapter(paramCommon).Get<List<string>>("api/HisServiceReq/GetAttachAssignPrint", ApiConsumers.MosConsumer, this.treatmentId, paramCommon);
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
                gridViewBillFund.PostEditor();
                success = ProcessSave(ref param);
                WaitingManager.Hide();
                if (success == true)
                {
                    if (chkPrintBKBHNT.Checked)
                    {
                        InBangKe_6556_BHYT_Mps000279();
                    }
                    this.hienHoaDonNhap = false;
                    this.onClickPhieuThuThanhToan();

                    if (cboPayForm.EditValue != null && Int64.Parse(cboPayForm.EditValue.ToString()) == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR)
                    {
                        btnQr.Enabled = true;
                        if (TransactionBillResultSDO.TransactionRepay != null && TransactionBillResultSDO.TransactionRepay.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR && TransactionBillResultSDO.TransactionRepay.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                            TransactionQr = TransactionBillResultSDO.TransactionRepay;
                        if (TransactionBillResultSDO.TransactionDeposit != null && TransactionBillResultSDO.TransactionDeposit.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR && TransactionBillResultSDO.TransactionDeposit.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                            TransactionQr = TransactionBillResultSDO.TransactionDeposit;
                        if (TransactionBillResultSDO.TransactionBill != null && TransactionBillResultSDO.TransactionBill.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR && TransactionBillResultSDO.TransactionBill.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                            TransactionQr = TransactionBillResultSDO.TransactionBill;
                        if (listConfig != null)
                        {
                            if (listConfig.Count > 1)
                                XtraMessageBox.Show("Vui lòng sử dụng nút tạo QR để thực hiện thanh toán", "Thông báo");
                            else
                                btnQr_Click(null, null);
                        }
                    }
                    if (chkAutoClose.CheckState == CheckState.Checked)
                    {
                        if (!chkPrintBKBHNT.Checked)
                            this.Close();
                        else
                        {
                            if (ListSereServTranfer != null && ListSereServTranfer.Count > 0)
                                timerClose.Interval = ListSereServTranfer.Count * timerClose.Interval;
                            timerClose.Start();
                        }
                    }
                }

                if (success == false)
                {
                    MessageManager.Show(param, success);
                }

                // SessionManager.ProcessTokenLost(param);
                GeneratePopupMenu();
                InitMenuToButtonPrint();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                SetEnableButtonSave(!success);
            }
        }
    }

}

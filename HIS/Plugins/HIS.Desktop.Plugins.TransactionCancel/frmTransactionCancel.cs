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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.Library.ElectronicBill;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Plugins.TransactionCancel.Base;
using HIS.Desktop.Plugins.TransactionCancel.Config;
using HIS.Desktop.Plugins.TransactionCancel.Validation;
using Inventec.Common.Adapter;
using Inventec.Common.Integrate.EditorLoader;
using Inventec.Common.LocalStorage.SdaConfig;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.WCF.JsonConvert;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WCF;
using WCF.Client;
using HIS.Desktop.Plugins.TransactionCancel.Config;
using DevExpress.XtraEditors.DXErrorProvider;

namespace HIS.Desktop.Plugins.TransactionCancel
{
    public partial class frmTransactionCancel : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule = null;

        V_HIS_TRANSACTION transaction;
        long transactionId;
        DelegateSelectData refreshDelegate = null;
        V_HIS_EXP_MEST_2 expMest = null;
        V_HIS_TRANSACTION TransactionCancelResult;
        private int positionHandleControl = -1;

        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.TransactionCancel";
        List<V_HIS_TRANSACTION> listTranPrint { get; set; }
        bool isVisileCancelHIS = false;

        public frmTransactionCancel(Inventec.Desktop.Common.Modules.Module module, V_HIS_TRANSACTION data)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.SetIcon();
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.transaction = data;
                this.currentModule = module;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmTransactionCancel(Inventec.Desktop.Common.Modules.Module module, V_HIS_TRANSACTION data, DelegateSelectData _selectData)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.SetIcon();
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.transaction = data;
                this.currentModule = module;
                this.refreshDelegate = _selectData;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmTransactionCancel(Inventec.Desktop.Common.Modules.Module module, long data, V_HIS_EXP_MEST_2 expMestData, DelegateSelectData _selectData)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.SetIcon();
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.transactionId = data;
                this.refreshDelegate = _selectData;
                this.currentModule = module;
                this.expMest = expMestData;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmTransactionDepositCancel_Load(object sender, EventArgs e)
        {
            try
            {
                HisConfigCFG.LoadConfig();
                this.LoadKeyFrmLanguage();
                this.LoadTransaction();
                LblBtnPrint.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                BtnPrint.Enabled = false;
                InitComboCancelReason();
                if (HisConfigCFG.TransactionBill__AllowWhenRequest == "1")
                {
                    this.Text = "Duyệt và hủy giao dịch";
                    lciCancelReqDepartment.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciCancelReqRoom.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciCancelReqUser.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciCancelReqReason.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciBtnReject.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciReasonRejectCancel.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    btnSave.Text = "Đồng ý (Ctrl S)";
                }
                lciCancelHIS.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciNotCancelBill.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                if (this.transaction != null)
                {
                    this.lblTransactionCode.Text = this.transaction.TRANSACTION_CODE;
                    this.lblQT.Text = Inventec.Common.Number.Convert.NumberToString(this.transaction.SWIPE_AMOUNT ?? 0);
                    this.lblCK.Text = Inventec.Common.Number.Convert.NumberToString(this.transaction.TRANSFER_AMOUNT ?? 0);

                    this.lblCancelReqDepartment.Text = this.transaction.CANCEL_REQ_DEPARTMENT_NAME;
                    this.lblCancelReqRoom.Text = this.transaction.CANCEL_REQ_ROOM_NAME;
                    this.lblCancelReqUser.Text = this.transaction.CANCEL_REQ_USERNAME;
                    this.lblCancelReqReason.Text = this.transaction.CANCEL_REQ_REASON;
                    if (!string.IsNullOrEmpty(transaction.CANCEL_REQ_REASON) && HisConfigCFG.TransactionBill__CashierRoomPaymentOption == "1")
                    {
                        txtOtherCancelReason.Text = transaction.CANCEL_REQ_REASON;
                    }
                    decimal? price = transaction.AMOUNT - (transaction.KC_AMOUNT ?? 0) - (transaction.TDL_BILL_FUND_AMOUNT ?? 0) - (transaction.EXEMPTION ?? 0);

                    this.lblAmount.Text = Inventec.Common.Number.Convert.NumberToString(price ?? 0, ConfigApplications.NumberSeperator);
                    if (this.expMest != null)
                    {
                        this.lblTreatmentCode.Text = this.expMest.TDL_TREATMENT_CODE;
                        this.lblVirPatientName.Text = this.expMest.TDL_PATIENT_NAME;
                    }
                    else
                    {
                        this.lblTreatmentCode.Text = this.transaction.TREATMENT_CODE;
                        this.lblVirPatientName.Text = this.transaction.TDL_PATIENT_NAME;
                    }
                    lblBillNumberPos.Text = this.transaction.POS_INVOICE;
                    dtCancelTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.Now() ?? 0);
                    dtCancelTime.Focus();
                    dtCancelTime.SelectAll();
                    if (this.transaction.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT || this.transaction.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU)
                    {
                        LblBtnPrint.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    }

                    if ((this.transaction.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT || this.transaction.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU)
                        && this.transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE)
                    {
                        isVisileCancelHIS = true;
                        lciCancelHIS.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        btnCancelHIS.ToolTip = "Chỉ thực hiện ghi nhận hủy giao dịch trên hệ thống phần mềm HIS. Người dùng thực hiện hoàn tiền cho bệnh nhân thông qua các hình thức khác (tiền mặt, chuyển khoản, …) chứ không sử dụng giao dịch qua hệ thống Một thẻ quốc gia";
                    }

                    if (!String.IsNullOrEmpty(transaction.INVOICE_CODE) && !String.IsNullOrEmpty(transaction.INVOICE_SYS))
                    {
                        lciNotCancelBill.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        InitControlState();
                    }

                    if (lciCancelHIS.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Always && lciNotCancelBill.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    {
                        chkNotCancelBill.Text = "Không tự động hủy HĐĐT";
                    }
                    if (HisConfigCFG.TransactionBill__Select == "2")
                    {
                        layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        lcicheckPOS.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                    else if (HisConfigCFG.TransactionBill__Select == "1")
                    {
                        if ((this.transaction.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT || this.transaction.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU) && !string.IsNullOrEmpty(this.transaction.POS_INVOICE))
                        {
                            layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                            lcicheckPOS.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                            InitControlState();
                        }
                        else
                        {
                            layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                            lcicheckPOS.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        }
                    }
                    if (this.transaction.SWIPE_AMOUNT == null)
                    {
                        lcicheckQT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                    if (this.transaction.TRANSFER_AMOUNT == null)
                    {
                        lciCheckCK.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.transaction), this.transaction));
                }
                else
                {
                    this.btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboCancelReason()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisCancelReasonFilter filter = new HisCancelReasonFilter();
                filter.IS_ACTIVE = 1;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "KSK_CONTRACT_CODE";
                var hisCancelReason = new BackendAdapter(param).Get<List<HIS_CANCEL_REASON>>("api/HisCancelReason/Get", ApiConsumers.MosConsumer, filter, param).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("CANCEL_REASON_CODE", "Mã lý do hủy", 100, 2));
                columnInfos.Add(new ColumnInfo("CANCEL_REASON_NAME", "Lý do hủy", 250, 3));
                ControlEditorADO controlEditorADO = new ControlEditorADO("CANCEL_REASON_NAME", "ID", columnInfos, true, 350);

                ControlEditorLoader.Load(cboCancelReason, hisCancelReason, controlEditorADO);
                cboCancelReason.Properties.View.OptionsView.ShowIndicator = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadTransaction()
        {
            try
            {
                if (this.transaction == null)
                {
                    this.transaction = GetViewTransaction(this.transactionId);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                dxValidationProvider1.SetValidationRule(txtRejectCancel, null);
                this.ValidControlcboCancelReason();
                this.ValidControlCancelTime();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlcboCancelReason()
        {
            try
            {
                ComboBoxCancelReasonValidation reasonRule = new ComboBoxCancelReasonValidation();
                reasonRule.comboBox = cboCancelReason;
                reasonRule.memoEdit = txtOtherCancelReason;
                dxValidationProvider1.SetValidationRule(cboCancelReason, reasonRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlCancelTime()
        {
            try
            {
                CancelTimeValidationRule reasonRule = new CancelTimeValidationRule();
                reasonRule.dtCancelTime = dtCancelTime;
                reasonRule.TransactionTime = this.transaction.TRANSACTION_TIME;
                dxValidationProvider1.SetValidationRule(dtCancelTime, reasonRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtCancelReason_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.ValidControl();
                this.positionHandleControl = -1;
                if (!this.dxValidationProvider1.Validate())
                {
                    return;
                }
                if (!this.btnSave.Enabled || this.transaction == null)
                    return;

                //khi xử lý khi check được hiển thị
                if (lciNotCancelBill.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && this.transaction.IS_CANCEL_EINVOICE != 1)
                {
                    if (chkNotCancelBill.Checked && XtraMessageBox.Show("Bạn có chắc không muốn tự động hủy hóa đơn điện tử không?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                    {
                        chkNotCancelBill.Focus();
                        return;
                    }
                    else if (!chkNotCancelBill.Checked && XtraMessageBox.Show("Bạn có chắc muốn tự động hủy hóa đơn điện tử không?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                    {
                        chkNotCancelBill.Focus();
                        return;
                    }
                }
                listTranPrint = new List<V_HIS_TRANSACTION>();
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                HisTransactionCancelSDO sdo = ProcessDataForSave();

                if (this.transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE && HisConfigCFG.TransactionBill__CashierRoomPaymentOption == "1" && !isVisileCancelHIS)
                {
                    //#29335               
                    // giao dịch thanh toán, tạm ứng, tạm ứng dịch vụ
                    if (this.transaction.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU
                        || this.transaction.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT)
                    {
                        if (String.IsNullOrWhiteSpace(this.transaction.TIG_VOID_CODE))
                        {
                            //kiểm tra trước khi thanh toán nếu là giao dịch qua thẻ
                            var check = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisTransaction/CheckCancel", ApiConsumers.MosConsumer, sdo, param);

                            if (!check)
                            {
                                WaitingManager.Hide();
                                MessageManager.Show(param, success);
                                return;
                            }

                            CARD.WCF.DCO.WcfVoidDCO WcfVoidDCO = new CARD.WCF.DCO.WcfVoidDCO();
                            WcfVoidDCO.Amount = this.transaction.AMOUNT;
                            WcfVoidDCO.VoidCode = this.transaction.TIG_TRANSACTION_CODE;
                            if (this.transaction.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__HU)
                            {
                                WcfVoidDCO.IsRepay = true;
                            }
                            var resultWcf = VoidCard(ref WcfVoidDCO);

                            if (resultWcf == null || resultWcf.ResultCode == null || !resultWcf.ResultCode.Equals("00"))
                            {
                                success = false;
                                // nếu giao dịch thất bại thì confirm người dùng
                                Inventec.Common.Logging.LogSystem.Info("resultWcf: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultWcf), resultWcf));
                                string message = "";
                                MappingErrorTHE mappingErrorTHE = new Config.MappingErrorTHE();
                                if (resultWcf != null
                                       && !String.IsNullOrWhiteSpace(resultWcf.ResultCode)
                                       && mappingErrorTHE.dicMapping != null
                                       && mappingErrorTHE.dicMapping.Count > 0
                                       && mappingErrorTHE.dicMapping.ContainsKey(resultWcf.ResultCode))
                                {
                                    message = mappingErrorTHE.dicMapping[resultWcf.ResultCode];
                                }
                                else if (resultWcf != null && String.IsNullOrWhiteSpace(resultWcf.ResultCode))
                                {
                                    message = ("Kiểm tra lại phần mềm kết nối thiết bị. ");
                                }
                                else if (resultWcf != null
                                    && !String.IsNullOrWhiteSpace(resultWcf.ResultCode)
                                    && mappingErrorTHE.dicMapping != null
                                    && mappingErrorTHE.dicMapping.Count > 0
                                    && !mappingErrorTHE.dicMapping.ContainsKey(resultWcf.ResultCode)
                                    )
                                {
                                    message = ("Kiểm tra lại phần mềm kết nối thiết bị. ");
                                }
                                WaitingManager.Hide();
                                if (MessageBox.Show(message + " " + ResourceMessageLang.HuyGiaoDichThanhToanTheThatBai,
                                             ResourceMessageLang.TieuDeThongBao,
                                             MessageBoxButtons.YesNo) == DialogResult.No)
                                {
                                    SessionManager.ProcessTokenLost(param);
                                    this.Close();
                                    return;
                                }
                            }
                        }
                        else
                        {
                            WaitingManager.Hide();
                            if (MessageBox.Show(ResourceMessageLang.GiaoDichDaDuocHuyQuaTHETruocDo,
                                            ResourceMessageLang.TieuDeThongBao,
                                            MessageBoxButtons.YesNo) == DialogResult.No)
                            {
                                SessionManager.ProcessTokenLost(param);
                                this.Close();
                                return;
                            }
                        }
                    }

                    if (this.transaction.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__HU)
                    {
                        WaitingManager.Hide();
                        if (MessageBox.Show(ResourceMessageLang.GiaoDichHoanUng1TheQuocGiaKhongTheHuyTiepTucKhong,
                                            ResourceMessageLang.TieuDeThongBao,
                                            MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            SessionManager.ProcessTokenLost(param);
                            this.Close();
                            return;
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(this.transaction.POS_INVOICE) && chkConnectionPOS.Checked == true)
                {
                    if (HisConfigCFG.TransactionBill__Select == "1")
                    {
                        OpenAppPOS();
                        WcfRequest wc = new WcfRequest(); // Khởi tạo data
                        if (transaction.SWIPE_AMOUNT != null && transaction.SWIPE_AMOUNT != 0)
                        {
                            wc.AMOUNT = (long)transaction.SWIPE_AMOUNT; // Số tiền
                        }
                        else if (transaction.AMOUNT != null && transaction.AMOUNT != 0)
                        {
                            wc.AMOUNT = (long)(transaction.AMOUNT - (transaction.KC_AMOUNT ?? 0) - (transaction.TDL_BILL_FUND_AMOUNT ?? 0) - (transaction.EXEMPTION ?? 0));
                        }
                        if (!string.IsNullOrEmpty(this.transaction.POS_INVOICE))
                        {
                            wc.INVOICE = Int64.Parse(this.transaction.POS_INVOICE); // Số hóa đơn
                        }
                        var json = JsonConvert.Serialize<WcfRequest>(wc);
                        //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => json), json));
                        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(json);
                        WcfClient cll = new WcfClient();
                        var result = cll.Void(System.Convert.ToBase64String(plainTextBytes));
                        //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
                        if (result != null && result.RESPONSE_CODE == "00")
                        {
                            this.transaction.POS_PAN = result.PAN;
                            this.transaction.POS_CARD_HOLDER = result.NAME;
                            this.transaction.POS_INVOICE = result.INVOICE.ToString();
                            this.transaction.POS_RESULT_JSON = JsonConvert.Serialize<WcfRequest>(result);
                            //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.transaction), this.transaction));
                            success = true;
                        }
                        else
                        {
                            if (result != null)
                            {
                                WaitingManager.Hide();
                                if (DevExpress.XtraEditors.XtraMessageBox.
                               Show("Giao dịch hủy không thành công" + "(Mã lỗi: " + result.ERROR + ")", ResourceMessageLang.TieuDeThongBao, System.Windows.Forms.MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK)
                                    success = false;
                            }
                            success = false;
                            return;
                        }
                    }
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TRANSACTION>("api/HisTransaction/Cancel", ApiConsumers.MosConsumer, sdo, param);
                if (rs != null)
                {
                    success = true;
                    if (this.refreshDelegate != null) this.refreshDelegate(true);
                    TransactionCancelResult = GetViewTransaction(rs.ID);
                    listTranPrint.Add(this.TransactionCancelResult);
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
                SessionManager.ProcessTokenLost(param);
                if (success)
                {
                    ProcessUpdateServiceReq();
                    if (this.transaction.IS_CANCEL_EINVOICE != 1)
                    {
                        this.CancelElectronicBill(sdo);
                    }
                    if (HisConfigCFG.TransactionBill__AutoCancel == "1" && this.transaction.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT && this.transaction.SALE_TYPE_ID == null)
                    {
                        AutoCancel(sdo);
                    }

                    btnCancelHIS.Enabled = false;
                    btnSave.Enabled = false;
                    if (LblBtnPrint.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    {
                        BtnPrint.Enabled = true;
                    }
                    else
                    {
                        this.Close();
                    }
                    btnReject.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HisTransactionCancelSDO ProcessDataForSave(bool? isInternal = null)
        {
            HisTransactionCancelSDO sdo = new HisTransactionCancelSDO();
            try
            {
                sdo.TransactionId = this.transaction.ID;

                if (!String.IsNullOrEmpty(this.txtOtherCancelReason.Text))
                    sdo.CancelReason = this.txtOtherCancelReason.Text;

                if (this.cboCancelReason.Text.Trim() != "")
                    sdo.CancelReasonId = (long)this.cboCancelReason.EditValue;
                if (this.transaction.TREATMENT_ID.HasValue && (!this.transaction.SALE_TYPE_ID.HasValue || this.transaction.SALE_TYPE_ID.Value == IMSys.DbConfig.HIS_RS.HIS_SALE_TYPE.ID__SALE_OTHER))
                {
                    sdo.RequestRoomId = this.currentModule.RoomId;
                }
                else
                {
                    V_HIS_CASHIER_ROOM cashier_room = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().FirstOrDefault(o => o.ID == this.transaction.CASHIER_ROOM_ID);
                    sdo.RequestRoomId = cashier_room.ROOM_ID;
                }

                if (dtCancelTime.EditValue != null && dtCancelTime.DateTime != DateTime.MinValue)
                {
                    sdo.CancelTime = Convert.ToInt64(dtCancelTime.DateTime.ToString("yyyyMMddHHmm") + "00");
                }

                sdo.IsInternal = isInternal;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return sdo;
        }

        private void ProcessUpdateServiceReq()
        {
            try
            {
                if (TransactionCancelResult != null && (TransactionCancelResult.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT || TransactionCancelResult.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU) && TransactionCancelResult.KC_AMOUNT == null)
                {
                    CommonParam param = new CommonParam();
                    List<long> serviceReqIds = new List<long>();
                    if (TransactionCancelResult.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT)
                    {
                        HisSereServBillFilter Bilfilter = new HisSereServBillFilter();
                        Bilfilter.BILL_ID = TransactionCancelResult.ID;
                        var rsBil = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, Bilfilter, param);
                        if (rsBil != null && rsBil.Count > 0)
                        {
                            rsBil = rsBil.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT).ToList();
                            if (rsBil != null && rsBil.Count > 0)
                            {
                                serviceReqIds = rsBil.Select(o => o.TDL_SERVICE_REQ_ID ?? 0).ToList();
                            }
                        }
                    }
                    else
                    {
                        HisSereServDepositFilter filter = new HisSereServDepositFilter();
                        filter.DEPOSIT_ID = TransactionCancelResult.ID;
                        var rs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumers.MosConsumer, filter, param);
                        if (rs != null && rs.Count > 0)
                        {
                            rs = rs.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT).ToList();
                            if (rs != null && rs.Count > 0)
                            {
                                serviceReqIds = rs.Select(o => o.TDL_SERVICE_REQ_ID ?? 0).ToList();
                            }
                        }
                    }
                    if (serviceReqIds != null && serviceReqIds.Count > 0)
                    {
                        HisExpMestFilter emFilter = new HisExpMestFilter();
                        emFilter.SERVICE_REQ_IDs = serviceReqIds;
                        emFilter.IS_NOT_TAKEN = true;
                        var expMest = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, emFilter, param);
                        if (expMest != null && expMest.Count > 0)
                        {
                            if (XtraMessageBox.Show(String.Format("Đơn thuốc {0} bệnh nhân không lấy. Bạn có muốn bỏ ra khỏi bảng kê và không tính tiền bệnh nhân đối với các đơn thuốc này không? (Lưu ý: việc bỏ ra khỏi bảng kê sẽ làm thay đổi hồ sơ và XML đẩy cổng BHYT)", string.Join(", ", expMest.Select(o => o.EXP_MEST_CODE))), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                HisSereServNoExecuteSDO sereServNoExecuteSDO = new HisSereServNoExecuteSDO();
                                sereServNoExecuteSDO.IsNoExecute = true;
                                sereServNoExecuteSDO.RequestRoomId = currentModule.RoomId;
                                sereServNoExecuteSDO.TreatmentId = transaction.TREATMENT_ID ?? 0;
                                sereServNoExecuteSDO.ServiceReqIds = serviceReqIds;
                                var ss = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<HIS_SERE_SERV>>("api/HisSereServ/UpdateNoExecute", ApiConsumers.MosConsumer, sereServNoExecuteSDO, param);
                                MessageManager.Show(param, ss != null);
                            }
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

        private void AutoCancel(HisTransactionCancelSDO sdo)
        {
            try
            {
                if (this.transaction == null || !this.transaction.TREATMENT_ID.HasValue)
                    return;

                List<long> TransactionTypeIds = new List<long>();
                TransactionTypeIds.Add(IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT);
                HisTransactionViewFilter filter = new HisTransactionViewFilter();
                filter.TREATMENT_ID = this.transaction.TREATMENT_ID;
                filter.IS_CANCEL = false;
                filter.TRANSACTION_TYPE_IDs = TransactionTypeIds;
                filter.SALE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SALE_TYPE.ID__SALE_OTHER;
                var listTransactionCancel = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, filter, null);

                if (listTransactionCancel != null)
                {
                    foreach (var item in listTransactionCancel)
                    {
                        sdo.TransactionId = item.ID;
                        var rsItem = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Post<HIS_TRANSACTION>("api/HisTransaction/Cancel", ApiConsumers.MosConsumer, sdo, null);
                        if (rsItem != null && item.ID != TransactionCancelResult.ID) listTranPrint.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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
                    Inventec.Common.Logging.LogSystem.Info("FileName " + startInfo.FileName);
                    Process.Start(startInfo);
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => startInfo), startInfo));
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
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains(name))
                {
                    return true;
                }
            }
            return false;
        }

        private V_HIS_TRANSACTION GetViewTransaction(long id)
        {
            V_HIS_TRANSACTION result = null;
            try
            {
                if (id > 0)
                {
                    CommonParam param = new CommonParam();
                    HisTransactionViewFilter filter = new HisTransactionViewFilter();
                    filter.ID = id;
                    List<V_HIS_TRANSACTION> trans = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, filter, param);
                    result = trans != null ? trans.FirstOrDefault() : null;
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool CancelElectronicBill(HisTransactionCancelSDO sdo)
        {
            bool result = true;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("CancelElectronicBill.Begin!");
                if (transaction != null && !String.IsNullOrEmpty(transaction.INVOICE_CODE) && !String.IsNullOrEmpty(transaction.INVOICE_SYS) && !chkNotCancelBill.Checked)
                {
                    ElectronicBillDataInput dataInput = new ElectronicBillDataInput();
                    dataInput.PartnerInvoiceID = Inventec.Common.TypeConvert.Parse.ToInt64(transaction.INVOICE_CODE);

                    if (!String.IsNullOrWhiteSpace(sdo.CancelReason))
                    {
                        dataInput.CancelReason = sdo.CancelReason;
                    }
                    else if (sdo.CancelReasonId.HasValue)
                    {
                        var reason = BackendDataWorker.Get<HIS_CANCEL_REASON>().FirstOrDefault(s => s.ID == sdo.CancelReasonId.Value);
                        if (reason != null)
                        {
                            dataInput.CancelReason = reason.CANCEL_REASON_NAME;
                        }
                    }

                    dataInput.CancelTime = sdo.CancelTime;

                    dataInput.InvoiceCode = transaction.INVOICE_CODE;
                    dataInput.NumOrder = transaction.NUM_ORDER;
                    dataInput.SymbolCode = transaction.SYMBOL_CODE;
                    dataInput.TemplateCode = transaction.TEMPLATE_CODE;
                    dataInput.TransactionTime = transaction.EINVOICE_TIME ?? transaction.TRANSACTION_TIME;
                    dataInput.ENumOrder = transaction.EINVOICE_NUM_ORDER;
                    dataInput.EinvoiceTypeId = transaction.EINVOICE_TYPE_ID;
                    dataInput.TransactionCode = transaction.TRANSACTION_CODE;

                    dataInput.Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                    ElectronicBillProcessor electronicBillProcessor = new ElectronicBillProcessor(dataInput);
                    ElectronicBillResult electronicBillResult = electronicBillProcessor.Run(ElectronicBillType.ENUM.CANCEL_INVOICE);

                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => electronicBillResult), electronicBillResult));
                    if (electronicBillResult != null && electronicBillResult.Success)
                    {
                        CommonParam param = new CommonParam();
                        CancelInvoiceSDO cancelInvoiceSDO = new CancelInvoiceSDO();
                        cancelInvoiceSDO.TransactionId = this.transaction.ID;
                        cancelInvoiceSDO.CancelTime = sdo.CancelTime;
                        var apiResult = new BackendAdapter(param).Post<HIS_TRANSACTION>("api/HisTransaction/CancelInvoice", ApiConsumers.MosConsumer, cancelInvoiceSDO, param);
                        if (apiResult != null)
                        {
                            if (apiResult.ID == this.transaction.ID)
                            {
                                this.transaction.IS_CANCEL_EINVOICE = apiResult.IS_CANCEL_EINVOICE;
                                this.transaction.CANCEL_TIME = apiResult.CANCEL_TIME;
                            }
                        }

                    }
                    else if (electronicBillResult != null && !electronicBillResult.Success)
                    {
                        string mes = "";
                        if (electronicBillResult.Messages != null && electronicBillResult.Messages.Count > 0)
                        {
                            foreach (var item in electronicBillResult.Messages)
                            {
                                mes += item + ";";
                            }
                            if (mes.Length > 0)
                                mes = mes.Remove(mes.Length - 1, 1);
                        }

                        XtraMessageBox.Show("Hủy hóa đơn điện tử thất bại." + mes + ". Vui lòng truy cập hệ thống hóa đơn điện tử để hủy giao dịch.");
                        if (XtraMessageBox.Show("Bạn có muốn hủy lại hóa đơn điện tử không?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            this.CancelElectronicBill(sdo);
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("Hủy hóa đơn điện tử thất bại.");
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("CancelElectronicBill.Ended!");
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void bbtnRCSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;
                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;
                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadKeyFrmLanguage()
        {
            try
            {
                var cul = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                var lang = Base.ResourceLangManager.LanguageFrmTransactionDepositCancel;

                //this.btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT_CANCEL__BTN_SAVE", lang, cul);

                this.layoutAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT_CANCEL__LAYOUT_AMOUNT", lang, cul);
                //this.layoutOtherCancelReason.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT_CANCEL__LAYOUT_CANCEL_REASON", lang, cul);
                this.layoutTransactionCode.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT_CANCEL__LAYOUT_TRANSACTION_CODE", lang, cul);
                this.layoutTreatmentCode.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT_CANCEL__LAYOUT_TREATMENT_CODE", lang, cul);
                this.layoutVirPatientName.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT_CANCEL__LAYOUT_VIR_PATIENT_NAME", lang, cul);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtCancelTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboCancelReason.Focus();
                    cboCancelReason.SelectAll();
                    if (cboCancelReason.Text.Trim() == "")
                        cboCancelReason.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BarBtnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!BtnPrint.Enabled) return;

                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore
                  (HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR,
                  Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(),
                  HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                if (this.transaction.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT)
                {
                    richEditorMain.RunPrintTemplate("Mps000337", DelegateRunPrinter);
                }
                else if (this.transaction.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU)
                {
                    richEditorMain.RunPrintTemplate("Mps000381", DelegateRunPrinter);
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
                    case "Mps000337":
                        if (this.listTranPrint != null && this.listTranPrint.Count() > 0)
                        {
                            foreach (var item in this.listTranPrint)
                            {
                                LoadBieuMauPhieuHuyThanhToan(printCode, fileName, ref result, item);
                            }
                        }
                        break;
                    case "Mps000381":
                        LoadBieuMauPhieuHuyTamUng(printCode, fileName, ref result);
                        break;
                    default:
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

        private void LoadBieuMauPhieuHuyTamUng(string printCode, string fileName, ref bool result)
        {
            try
            {
                if (TransactionCancelResult == null) return;
                if (!TransactionCancelResult.TREATMENT_ID.HasValue) return;

                V_HIS_PATIENT patient = new V_HIS_PATIENT();
                CommonParam param = new CommonParam();
                HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = TransactionCancelResult.TDL_PATIENT_ID ?? 0;
                var patients = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientFilter, param);
                if (patients != null && patients.Count > 0)
                {
                    patient = patients.FirstOrDefault();
                }

                V_HIS_TREATMENT_FEE treatmentFee = new V_HIS_TREATMENT_FEE();
                HisTreatmentFeeViewFilter feeFilter = new HisTreatmentFeeViewFilter();
                feeFilter.ID = TransactionCancelResult.TREATMENT_ID ?? 0;
                var fees = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, feeFilter, param);
                if (fees != null && fees.Count > 0)
                {
                    treatmentFee = fees.FirstOrDefault();
                }

                MPS.Processor.Mps000381.PDO.Mps000381PDO rdo = new MPS.Processor.Mps000381.PDO.Mps000381PDO(TransactionCancelResult, patient, treatmentFee);
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printCode))
                {
                    printerName = GlobalVariables.dicPrinter[printCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.TransactionCancelResult != null ? this.TransactionCancelResult.TREATMENT_CODE : ""), printCode, currentModule != null ? currentModule.RoomId : 0);

                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadBieuMauPhieuHuyThanhToan(string printCode, string fileName, ref bool result, V_HIS_TRANSACTION data)
        {
            try
            {
                if (data == null) return;
                if (!data.TREATMENT_ID.HasValue) return;

                MPS.Processor.Mps000337.PDO.Mps000337PDO rdo = new MPS.Processor.Mps000337.PDO.Mps000337PDO(data);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printCode))
                {
                    printerName = GlobalVariables.dicPrinter[printCode];
                }

                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCancelHIS_Click(object sender, EventArgs e)
        {
            try
            {
                ValidControl();
                this.positionHandleControl = -1;
                if (!this.dxValidationProvider1.Validate())
                {
                    return;
                }
                if (!this.btnCancelHIS.Visible || this.transaction == null || !isVisileCancelHIS)
                    return;
                if (MessageBox.Show(ResourceMessageLang.GiaoDichTTTUHU1THEQuocGiaHuyNoiBoBanCoMuonTiepTuc,
                                            ResourceMessageLang.TieuDeThongBao,
                                            MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;

                HisTransactionCancelSDO sdo = ProcessDataForSave(true);

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TRANSACTION>("api/HisTransaction/Cancel", ApiConsumers.MosConsumer, sdo, param);
                if (rs != null)
                {
                    success = true;
                    if (this.refreshDelegate != null) this.refreshDelegate(true);
                    TransactionCancelResult = GetViewTransaction(rs.ID);

                    if (this.transaction.IS_CANCEL_EINVOICE != 1)
                    {
                        this.CancelElectronicBill(sdo);
                    }
                }
                WaitingManager.Hide();

                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);

                if (success)
                {
                    btnSave.Enabled = false;
                    if (LblBtnPrint.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    {
                        BtnPrint.Enabled = true;
                    }
                    else
                    {
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {

            isNotLoadWhileChangeControlStateInFirst = true;
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentControlStateRDO), currentControlStateRDO));
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkNotCancelBill.Name)
                        {
                            chkNotCancelBill.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == chkConnectionPOS.Name)
                        {
                            chkConnectionPOS.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhileChangeControlStateInFirst = false;
        }

        private void chkNotCancelBill_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkNotCancelBill.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkNotCancelBill.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkNotCancelBill.Name;
                    csAddOrUpdate.VALUE = (chkNotCancelBill.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCancelReason_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboCancelReason.Properties.Buttons[1].Visible = false;
                    cboCancelReason.EditValue = null;
                    //txtOtherCancelReason.Text = "";
                    txtOtherCancelReason.Focus();
                    txtOtherCancelReason.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCancelReason_EditValueChanged(object sender, EventArgs e)
        {
            if (cboCancelReason.Text != "")
                cboCancelReason.Properties.Buttons[1].Visible = true;
        }

        private void cboCancelReason_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtOtherCancelReason.Focus();
                    txtOtherCancelReason.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkConnectionPOS_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkConnectionPOS.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    Inventec.Common.Logging.LogSystem.Info(" log 0");
                    csAddOrUpdate.VALUE = (chkConnectionPOS.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    Inventec.Common.Logging.LogSystem.Info("1");
                    csAddOrUpdate.KEY = chkConnectionPOS.Name;
                    Inventec.Common.Logging.LogSystem.Info("2");
                    csAddOrUpdate.VALUE = (chkConnectionPOS.Checked ? "1" : "");
                    Inventec.Common.Logging.LogSystem.Info("3");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                    Inventec.Common.Logging.LogSystem.Info("4");
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            try
            {
                ValidationRejectCancel(txtRejectCancel);
                this.positionHandleControl = -1;
                if (!this.dxValidationProvider1.Validate() || this.transaction == null)
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                HisTransactionRejectCancellationRequestSDO sdo = new HisTransactionRejectCancellationRequestSDO();
                sdo.RejectCancelReqReason = this.txtRejectCancel.Text;
                sdo.TransactionId = this.transaction.ID;
                sdo.WorkingRoomId = this.currentModule.RoomId;
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<V_HIS_TRANSACTION>("api/HisTransaction/RejectCancellationRequest", ApiConsumers.MosConsumer, sdo, param);
                if (rs != null)
                {
                    success = true;
                }
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);
                if (success)
                {
                    this.Close();
                }
            }

            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationRejectCancel(MemoEdit control)
        {
            try
            {
                dxValidationProvider1.SetValidationRule(cboCancelReason, null);
                ValidateRejectCancel validRule = new ValidateRejectCancel();
                validRule.memoEdit = control;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

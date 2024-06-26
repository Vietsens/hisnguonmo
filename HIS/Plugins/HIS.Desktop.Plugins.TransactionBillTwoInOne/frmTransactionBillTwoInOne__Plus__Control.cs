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
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.TransactionBillTwoInOne.ADO;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TransactionBillTwoInOne
{
    public partial class frmTransactionBillTwoInOne : HIS.Desktop.Utility.FormBase
    {
        #region Reciept (BienLai)
        private void txtRecieptAccountBookCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(txtRecieptAccountBookCode.Text))
                    {
                        var listData = listRecieptAccountBook.Where(o => o.ACCOUNT_BOOK_CODE.Contains(txtRecieptAccountBookCode.Text)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboRecieptAccountBook.EditValue = listData.First().ID;
                        }
                    }
                    if (!valid)
                    {
                        cboRecieptAccountBook.Focus();
                        cboRecieptAccountBook.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRecieptAccountBook_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (spinRecieptNumOrder.Enabled)
                    {
                        spinRecieptNumOrder.Focus();
                        spinRecieptNumOrder.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRecieptAccountBook_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtRecieptAccountBookCode.Text = "";
                spinRecieptNumOrder.EditValue = null;
                spinRecieptNumOrder.Enabled = false;
                lblSoBlvp.Text = "";
                cboRecieptAccountBook.Properties.Buttons[1].Visible = false;
                if (cboRecieptAccountBook.EditValue != null)
                {
                    cboRecieptAccountBook.Properties.Buttons[1].Visible = true;
                    var account = listRecieptAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboRecieptAccountBook.EditValue));
                    if (account != null)
                    {
                        txtRecieptAccountBookCode.Text = account.ACCOUNT_BOOK_CODE;
                        spinRecieptNumOrder.EditValue = setDataToDicNumOrderInAccountBook(account);
                        lblReceiptBook.Text = String.Format("{0} - {1}", account.ACCOUNT_BOOK_CODE, account.ACCOUNT_BOOK_NAME ?? "");
                        if (spinRecieptNumOrder.EditValue != null)
                        {
                            lblSoBlvp.Text = Inventec.Common.Number.Convert.NumberToString(spinRecieptNumOrder.Value, 0);
                        }
                        else
                        {
                            lblSoBlvp.Text = "";
                        }
                        if (account.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                        {
                            spinRecieptNumOrder.Enabled = true;
                        }
                        GlobalVariables.DefaultAccountBookBillTwoInOne_VP = new List<V_HIS_ACCOUNT_BOOK>();
                        GlobalVariables.DefaultAccountBookBillTwoInOne_VP.Add(account);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRecieptAccountBook_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboRecieptAccountBook.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinRecieptNumOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtRecieptDescription.Focus();
                    txtRecieptDescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPayForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(txtPayForm.Text))
                    {
                        var listData = BackendDataWorker.Get<HIS_PAY_FORM>().Where(o => o.PAY_FORM_CODE.Contains(txtPayForm.Text) && o.IS_ACTIVE == 1).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboPayForm.EditValue = listData.First().ID;
                        }
                    }
                    if (!valid)
                    {
                        cboPayForm.Focus();
                        cboPayForm.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPayForm_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    //txtRecieptDescription.Focus();
                    //txtRecieptDescription.SelectAll();

                    txtPayForm.Text = "";
                    HIS_PAY_FORM payForm = null;
                    if (cboPayForm.EditValue != null)
                    {
                        payForm = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboPayForm.EditValue) && o.IS_ACTIVE == 1);
                        if (payForm != null)
                        {
                            txtPayForm.Text = payForm.PAY_FORM_CODE;
                        }
                    }
                    CheckRecieptPayFormKEYPAY(payForm);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPayForm_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtPayForm.Text = "";
                var payForm = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboPayForm.EditValue) && o.IS_ACTIVE == 1);
                if (payForm != null)
                {
                    txtPayForm.Text = payForm.PAY_FORM_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void txtRecieptDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinRecieptDiscountPrice.Focus();
                    spinRecieptDiscountPrice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinRecieptDiscountPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinRecieptDiscountRatio.Focus();
                    spinRecieptDiscountRatio.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinRecieptDiscountPrice_EditValueChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    FormatControl(ConfigApplications.NumberSeperator, spinRecieptDiscountPrice);
            //    if (spinRecieptDiscountPrice.EditValue != spinRecieptDiscountPrice.OldEditValue)
            //    {
            //        decimal totalRecieptPrice = listRecieptData.Sum(o => (o.RecieptPrice ?? 0));
            //        if (totalRecieptPrice > 0)
            //        {
            //            spinRecieptDiscountRatio.Value = (spinRecieptDiscountPrice.Value / totalRecieptPrice) * 100;
            //            CalcuCanThu(true);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }
        private void spinRecieptDiscountPrice_Leave(object sender, EventArgs e)
        {
            try
            {
                FormatControl(ConfigApplications.NumberSeperator, spinRecieptDiscountPrice);
                if (spinRecieptDiscountPrice.EditValue != spinRecieptDiscountPrice.OldEditValue)
                {
                    decimal totalRecieptPrice = listRecieptData.Sum(o => (o.RecieptPrice ?? 0));
                    if (totalRecieptPrice > 0)
                    {
                        spinRecieptDiscountRatio.Value = (spinRecieptDiscountPrice.Value / totalRecieptPrice) * 100;
                        CalcuCanThu(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinRecieptDiscountRatio_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtRecieptReason.Focus();
                    txtRecieptReason.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinRecieptDiscountRatio_EditValueChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (spinRecieptDiscountRatio.EditValue != spinRecieptDiscountRatio.OldEditValue)
            //    {
            //        decimal totalRecieptPrice = listRecieptData.Sum(o => (o.RecieptPrice ?? 0));
            //        spinRecieptDiscountPrice.Value = (spinRecieptDiscountRatio.Value * totalRecieptPrice) / 100;
            //        CalcuCanThu(true);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }
        private void spinRecieptDiscountRatio_Leave(object sender, EventArgs e)
        {
            try
            {
                if (spinRecieptDiscountRatio.EditValue != spinRecieptDiscountRatio.OldEditValue)
                {
                    decimal totalRecieptPrice = listRecieptData.Sum(o => (o.RecieptPrice ?? 0));
                    spinRecieptDiscountPrice.Value = (spinRecieptDiscountRatio.Value * totalRecieptPrice) / 100;
                    CalcuCanThu(true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void txtRecieptReason_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtInvoiceAccountBookCode.Focus();
                    txtInvoiceAccountBookCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Invoice (HoaDon)
        private void txtInvoiceAccountBookCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(txtInvoiceAccountBookCode.Text))
                    {
                        var listData = listInvoiceAccountBook.Where(o => o.ACCOUNT_BOOK_CODE.Contains(txtInvoiceAccountBookCode.Text)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboInvoiceAccountBook.EditValue = listData.First().ID;
                        }
                    }
                    if (!valid)
                    {
                        cboInvoiceAccountBook.Focus();
                        cboInvoiceAccountBook.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboInvoiceAccountBook_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (spinInvoiceNumOrder.Enabled)
                    {
                        spinInvoiceNumOrder.Focus();
                        spinInvoiceNumOrder.SelectAll();
                    }
                    else
                    {
                        txtInvoiceDescription.Focus();
                        txtInvoiceDescription.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboInvoiceAccountBook_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtInvoiceAccountBookCode.Text = "";
                spinInvoiceNumOrder.EditValue = null;
                spinInvoiceNumOrder.Enabled = false;
                lblSoBldv.Text = "";
                cboInvoiceAccountBook.Properties.Buttons[1].Visible = false;
                if (cboInvoiceAccountBook.EditValue != null)
                {
                    cboInvoiceAccountBook.Properties.Buttons[1].Visible = true;
                    var account = listInvoiceAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboInvoiceAccountBook.EditValue));
                    if (account != null)
                    {
                        txtInvoiceAccountBookCode.Text = account.ACCOUNT_BOOK_CODE;
                        spinInvoiceNumOrder.EditValue = setDataToDicNumOrderInAccountBook(account);
                        lblInvoiceBook.Text = String.Format("{0} - {1}", account.ACCOUNT_BOOK_CODE, account.ACCOUNT_BOOK_NAME ?? "");
                        if (spinInvoiceNumOrder.EditValue != null)
                        {
                            lblSoBldv.Text = Inventec.Common.Number.Convert.NumberToString(spinInvoiceNumOrder.Value, 0);
                        }
                        else
                        {
                            lblSoBldv.Text = "";
                        }
                        if (account.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                        {
                            spinInvoiceNumOrder.Enabled = true;
                        }
                        GlobalVariables.DefaultAccountBookBillTwoInOne_DV = new List<V_HIS_ACCOUNT_BOOK>();
                        GlobalVariables.DefaultAccountBookBillTwoInOne_DV.Add(account);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboInvoiceAccountBook_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboInvoiceAccountBook.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void spinInvoiceNumOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                   if (e.KeyCode == Keys.Enter)
                {  
                    txtInvoiceDescription.Focus();
                    txtInvoiceDescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetList()
        {
            MOS.Filter.HisCardFilter cardFilter = new MOS.Filter.HisCardFilter();
            cardFilter.PATIENT_ID = this.treatment.PATIENT_ID;
            hisCard = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_CARD>>("api/HisCard/Get", ApiConsumer.ApiConsumers.MosConsumer, cardFilter, null);

            MOS.Filter.HisPatientFilter PatientFilter = new MOS.Filter.HisPatientFilter();
            PatientFilter.ID = this.treatment.PATIENT_ID;
            var hispatients = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, PatientFilter, null);
            hispatient = hispatients.FirstOrDefault();
            //hisCard = BackendDataWorker.Get<HIS_CARD>().Where(o => o.PATIENT_ID == this.treatment.PATIENT_ID).ToList();
            //hispatient = BackendDataWorker.Get<V_HIS_PATIENT>().FirstOrDefault(o => o.ID == this.treatment.PATIENT_ID);

            Inventec.Common.Logging.LogSystem.Info("PATIENT_ID: " + this.treatment.PATIENT_ID + " hisCard: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisCard), hisCard) + "hispatient: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hispatient), hispatient));
        }

        private void CheckRecieptPayFormKEYPAY(HIS_PAY_FORM payForm)
        {
            try
            {
                if (payForm != null && payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__KEYPAY)
                {
                    if (hisCard == null || hisCard.Count == 0)
                    {
                        if (hispatient == null || hispatient.REGISTER_CODE == null)
                        {
                            Inventec.Desktop.Common.Message.MessageManager.Show("Bệnh nhân không có thông tin thẻ Việt hoặc mã MS.Vui lòng chọn hình thức thanh toán khác");
                            this.cboPayForm.Focus();
                            this.cboPayForm.ShowPopup();
                        }
                    }
                    Inventec.Common.Logging.LogSystem.Info("hisCard: " +Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisCard), hisCard) + "hispatient.REGISTER_CODE: " + hispatient.REGISTER_CODE);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

      


        private void txtInvoiceDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinInvoiceDiscountPrice.Focus();
                    spinInvoiceDiscountPrice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinInvoiceDiscountPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinInvoiceDiscountRatio.Focus();
                    spinInvoiceDiscountRatio.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinInvoiceDiscountPrice_EditValueChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    FormatControl(ConfigApplications.NumberSeperator, spinInvoiceDiscountPrice);
            //    if (spinInvoiceDiscountPrice.EditValue != spinInvoiceDiscountPrice.OldEditValue)
            //    {
            //        decimal totalInvoicePrice = listInvoiceData.Sum(o => o.InvoicePrice ?? 0);
            //        if (totalInvoicePrice > 0)
            //        {
            //            spinInvoiceDiscountRatio.Value = (spinInvoiceDiscountPrice.Value / totalInvoicePrice) * 100;
            //            CalcuCanThu(true);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void spinInvoiceDiscountPrice_Leave(object sender, EventArgs e)
        {
            try
            {
                FormatControl(ConfigApplications.NumberSeperator, spinInvoiceDiscountPrice);
                if (spinInvoiceDiscountPrice.EditValue != spinInvoiceDiscountPrice.OldEditValue)
                {
                    decimal totalInvoicePrice = listInvoiceData.Sum(o => o.InvoicePrice ?? 0);
                    if (totalInvoicePrice > 0)
                    {
                        spinInvoiceDiscountRatio.Value = (spinInvoiceDiscountPrice.Value / totalInvoicePrice) * 100;
                        CalcuCanThu(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void spinInvoiceDiscountRatio_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtInvoiceReason.Focus();
                    txtInvoiceReason.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinInvoiceDiscountRatio_EditValueChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (spinInvoiceDiscountRatio.EditValue != spinInvoiceDiscountRatio.OldEditValue)
            //    {
            //        decimal totalInvoicePrice = listInvoiceData.Sum(o => o.InvoicePrice ?? 0);
            //        spinInvoiceDiscountPrice.Value = (spinInvoiceDiscountRatio.Value * totalInvoicePrice) / 100;
            //        CalcuCanThu(true);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void spinInvoiceDiscountRatio_Leave(object sender, EventArgs e)
        {
            try
            {
                if (spinInvoiceDiscountRatio.EditValue != spinInvoiceDiscountRatio.OldEditValue)
                {
                    decimal totalInvoicePrice = listInvoiceData.Sum(o => o.InvoicePrice ?? 0);
                    spinInvoiceDiscountPrice.Value = (spinInvoiceDiscountRatio.Value * totalInvoicePrice) / 100;
                    CalcuCanThu(true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtInvoiceReason_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void checkNotReciept_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                ProcessEnabelControlReciept(!checkNotReciept.Checked);
                ProcessDataByCheckNot();
                CalcuTotalPrice();
                CalcuCanThu(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkNotInvoice_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                ProcessEnabelControlInvoice(!checkNotInvoice.Checked);
                ProcessDataByCheckNot();
                CalcuTotalPrice();
                CalcuCanThu(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ProcessEnabelControlReciept(bool enable)
        {
            try
            {
                txtRecieptAccountBookCode.Enabled = enable;           
                cboRecieptAccountBook.Enabled = enable;
                txtRecieptDescription.Enabled = enable;
                txtRecieptReason.Enabled = enable;
                var data = listRecieptAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboRecieptAccountBook.EditValue));
                if (data != null)
                {
                    if (data.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                    {
                        spinRecieptNumOrder.Enabled = enable;
                    }
                    else
                    {
                        spinRecieptNumOrder.Enabled = false;
                    }
                }
                else
                {
                    spinRecieptNumOrder.Enabled = enable;
                }
                btnPuMoTaVienPhi.Enabled = enable;
                btnPuLyDoVienPhi.Enabled = enable;
                spinRecieptDiscountPrice.Enabled = enable;
                spinRecieptDiscountRatio.Enabled = enable;
                spinRecieptDiscountPrice.Value = 0;
                spinRecieptDiscountRatio.Value = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ProcessEnabelControlInvoice(bool enable)
        {
            try
            {
                txtInvoiceAccountBookCode.Enabled = enable;
                cboInvoiceAccountBook.Enabled = enable;
                txtInvoiceDescription.Enabled = enable;
                txtInvoiceReason.Enabled = enable;
                var data = listInvoiceAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboInvoiceAccountBook.EditValue));
                if (data != null)
                {
                    if (data.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                    {
                        spinInvoiceNumOrder.Enabled = enable;
                    }
                    else
                    {
                        spinInvoiceNumOrder.Enabled = false;
                    }
                }
                else
                {
                    spinInvoiceNumOrder.Enabled = enable;
                }
                btnPuLyDoDichVu.Enabled = enable;
                btnPuMoTaDichVu.Enabled = enable;
                spinInvoiceDiscountPrice.Enabled = enable;
                spinInvoiceDiscountRatio.Enabled = enable;
                spinInvoiceDiscountPrice.Value = 0;
                spinInvoiceDiscountRatio.Value = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkIsKC_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.CalcuCanThu(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Buyer Info

        private void txtBuyerName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBuyerTaxCode.Focus();
                    txtBuyerTaxCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBuyerTaxCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBuyerAccountCode.Focus();
                    txtBuyerAccountCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBuyerAccountCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBuyerOrganization.Focus();
                    txtBuyerOrganization.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBuyerOrganization_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBuyerAddress.Focus();
                    txtBuyerAddress.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBuyerAddress_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
        #endregion

        private decimal setDataToDicNumOrderInAccountBook(V_HIS_ACCOUNT_BOOK accountBook)
        {
            decimal result = (accountBook.CURRENT_NUM_ORDER ?? 0) + 1;
            try
            {
                if (accountBook != null)
                {
                    if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook == null || HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count == 0 || (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook != null && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count > 0 && !HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.ContainsKey(accountBook.ID)))
                    {
                        if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook == null)
                        {
                            HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook = new Dictionary<long, decimal>();
                        }

                        CommonParam param = new CommonParam();
                        MOS.Filter.HisAccountBookViewFilter hisAccountBookViewFilter = new MOS.Filter.HisAccountBookViewFilter();
                        hisAccountBookViewFilter.ID = accountBook.ID;
                        var accountBooks = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK>>(ApiConsumer.HisRequestUriStore.HIS_ACCOUNT_BOOK_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, hisAccountBookViewFilter, param);
                        if (accountBooks != null && accountBooks.Count > 0)
                        {
                            var accountBookNew = accountBooks.FirstOrDefault();
                            decimal num = 0;
                            if ((accountBookNew.CURRENT_NUM_ORDER ?? 0) > 0)
                            {
                                num = (accountBookNew.CURRENT_NUM_ORDER ?? 0);
                            }
                            else
                            {
                                num = (decimal)accountBookNew.FROM_NUM_ORDER - 1;
                            }
                            HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Add(accountBookNew.ID, num);
                            result = (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID]) + 1;
                        }
                    }
                    else
                    {
                        result = (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID]) + 1;
                    }
                }
                else
                {
                    result = (accountBook.CURRENT_NUM_ORDER ?? 0) + 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void InitComboPayForm(DevExpress.XtraEditors.LookUpEdit cbo, object lData)
        {
            try
            {
                cbo.Properties.DataSource = lData;
                cbo.Properties.DisplayMember = "PAY_FORM_NAME";
                cbo.Properties.ValueMember = "ID";
                cbo.Properties.ForceInitialize();
                cbo.Properties.Columns.Clear();
                cbo.Properties.Columns.Add(new LookUpColumnInfo("PAY_FORM_CODE", "", 50));
                cbo.Properties.Columns.Add(new LookUpColumnInfo("PAY_FORM_NAME", "", 150));
                cbo.Properties.ShowHeader = false;
                cbo.Properties.ImmediatePopup = true;
                cbo.Properties.DropDownRows = 10;
                cbo.Properties.PopupWidth = 200;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

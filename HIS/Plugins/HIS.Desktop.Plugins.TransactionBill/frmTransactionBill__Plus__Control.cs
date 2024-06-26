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
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.TransactionBill.ADO;
using HIS.UC.SereServTree;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TransactionBill
{
    public partial class frmTransactionBill : HIS.Desktop.Utility.FormBase
    {

        private void cboAccountBook_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboAccountBook.EditValue != null)
                    {
                        var account = ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                        if (account != null)
                        {
                            //txtAccountBookCode.Text = account.ACCOUNT_BOOK_CODE;
                            //SetDataToDicNumOrderInAccountBook(account);
                            //GlobalVariables.DefaultAccountBookTransactionBill = new List<V_HIS_ACCOUNT_BOOK>();
                            //GlobalVariables.DefaultAccountBookTransactionBill.Add(account);
                        }
                    }
                    else
                    {
                        spinTongTuDen.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPayFormCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    //if (!String.IsNullOrEmpty(txtPayFormCode.Text))
                    //{
                    //    var listData = BackendDataWorker.Get<HIS_PAY_FORM>().Where(o => o.PAY_FORM_CODE.Contains(txtPayFormCode.Text)).ToList();
                    //    if (listData != null && listData.Count == 1)
                    //    {
                    //        valid = true;
                    //        txtPayFormCode.Text = listData.First().PAY_FORM_CODE;
                    //        cboPayForm.EditValue = listData.First().ID;
                    //        //CheckPayFormThanhToanThe(listData.First());
                    //        CheckPayFormTienMatChuyenKhoan(listData.First());
                    //        if (listData.First().ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE)
                    //        {
                    //            txtPin.Focus();
                    //            txtPin.SelectAll();
                    //        }
                    //        else
                    //        {
                    //            dtTransactionTime.Focus();
                    //        }
                    //    }
                    //}
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

        private void cboPayForm_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    PayFormADO payForm = null;
                    if (cboPayForm.EditValue != null)
                    {
                        payForm = this.payFormList.FirstOrDefault(o => o.PayFormId == cboPayForm.EditValue);
                        if (payForm != null)
                        {
                            if (dtTransactionTime.Enabled)
                            {
                                dtTransactionTime.Focus();
                            }
                        }
                    }
                    CheckPayFormTienMatChuyenKhoan(payForm);
                    CheckPayFormKEYPAY(payForm);
                    if (payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE && chkAutoRepay.Checked)
                    {
                        chkCoKetChuyen.Checked = true;
                        chkCoKetChuyen.ReadOnly = true;
                        chkCoKetChuyen.Enabled = false;
                    }
                    else
                    {
                        chkCoKetChuyen.ReadOnly = false;
                        chkCoKetChuyen.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        

        private void txtDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkAutoRepay.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDiscount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtDiscount.EditValue != null)
                    {
                        this.totalDiscount = txtDiscount.Value;
                        if (this.totalPatientPrice > 0)
                        {
                            txtDiscountRatio.EditValue = (this.totalDiscount / this.totalPatientPrice) * 100;
                        }
                    }
                    else
                    {
                        this.totalDiscount = 0;
                        txtDiscountRatio.EditValue = null;
                    }
                    CalcuCanThu();

                    txtDiscountRatio.Focus();
                    txtDiscountRatio.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDiscountRatio_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtDiscountRatio.EditValue != null)
                    {
                        var ratio = txtDiscountRatio.Value / 100;
                        this.totalDiscount = this.totalPatientPrice * ratio;
                        txtDiscount.Value = this.totalDiscount;
                    }
                    else
                    {
                        this.totalDiscount = 0;
                        txtDiscount.Value = 0;
                        txtDiscount.EditValue = null;
                    }
                    CalcuCanThu();

                    txtReason.Focus();
                    txtReason.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtReason_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void spinAmountBNDua_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                FormatControl(ConfigApplications.NumberSeperator, spinAmountBNDua);
                this.CalcuCanThu();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTransactionTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (layoutTongTuDen.Enabled)
                    {
                        spinTongTuDen.Focus();
                        spinTongTuDen.SelectAll();
                    }
                    else if (lciTranferAmount.Enabled)
                    {
                        spinTransferAmount.Focus();
                        spinTransferAmount.SelectAll();
                    }
                    else
                    {
                        txtDiscount.Focus();
                        txtDiscount.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessNotTakenPrescription(ref bool check)
        {
            try
            {
                if (XtraMessageBox.Show("Bạn muốn xử lý Không Lấy Thuốc/Vật tư phòng khám?", Base.ResourceMessageLang.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                {
                    check = false;
                    return;
                }
                if (this.ListSereServ != null && this.ListSereServ.Any(a => a.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK
                    && (!a.IS_NO_EXECUTE.HasValue || a.IS_NO_EXECUTE.Value != 1)))
                {
                    WaitingManager.Show();
                    bool success = false;
                    CommonParam param = new CommonParam();
                    HisSereServNoExecuteSDO dataSdo = new HisSereServNoExecuteSDO();
                    dataSdo.RequestRoomId = this.currentModule.RoomId;
                    dataSdo.TreatmentId = treatmentId ?? 0;
                    dataSdo.ServiceReqIds = this.ListSereServ.Where(o => o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK && (!o.IS_NO_EXECUTE.HasValue || o.IS_NO_EXECUTE.Value != 1)).Select(s => s.SERVICE_REQ_ID ?? 0).Distinct().ToList();
                    dataSdo.IsNoExecute = true;

                    var rs = new BackendAdapter(param).Post<List<HIS_SERE_SERV>>("api/HisSereServ/UpdateNoExecute", ApiConsumers.MosConsumer, dataSdo, param);
                    if (rs != null && rs.Count > 0)
                    {
                        success = true;
                        isInit = true;
                        this.ListSereServNoExecute = new List<V_HIS_SERE_SERV_5>();
                        this.LoadDataToTreeSereServ(true);
                        this.CalcuTotalPrice();
                        this.ProcessFundForHCM();
                        this.CalcuHienDu();
                        this.CalcuCanThu();
                        isInit = false;
                        
                    }
                    else
                    {
                        check = false;
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessTakenPrescription(ref bool check)
        {
            try
            {
                if (XtraMessageBox.Show("Bạn muốn xử lý Lấy Thuốc/Vật tư phòng khám?", Base.ResourceMessageLang.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                {
                    check = true;
                    return;
                }
                if (this.ListSereServNoExecute != null && this.ListSereServNoExecute.Any(a => a.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK
                    && a.IS_NO_EXECUTE.HasValue && a.IS_NO_EXECUTE.Value == 1))
                {
                    WaitingManager.Show();
                    bool success = false;
                    CommonParam param = new CommonParam();
                    HisSereServNoExecuteSDO dataSdo = new HisSereServNoExecuteSDO();
                    dataSdo.RequestRoomId = this.currentModule.RoomId;
                    dataSdo.TreatmentId = treatmentId ?? 0;
                    dataSdo.ServiceReqIds = this.ListSereServNoExecute.Where(o => o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK && o.IS_NO_EXECUTE.HasValue && o.IS_NO_EXECUTE.Value == 1).Select(s => s.SERVICE_REQ_ID ?? 0).Distinct().ToList();
                    dataSdo.IsNoExecute = false;

                    var rs = new BackendAdapter(param).Post<List<HIS_SERE_SERV>>("api/HisSereServ/UpdateNoExecute", ApiConsumers.MosConsumer, dataSdo, param);
                    if (rs != null && rs.Count > 0)
                    {
                        success = true;
                        isInit = true;
                        this.LoadDataToTreeSereServ(true);
                       
                        this.CalcuTotalPrice();
                        this.ProcessFundForHCM();
                        this.CalcuHienDu();
                        this.CalcuCanThu();
                        isInit = false;
                        this.ListSereServNoExecute = new List<V_HIS_SERE_SERV_5>();
                    }
                    else
                    {
                        check = true;
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

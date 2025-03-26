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
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Transaction.Config;
using HIS.Desktop.Utility;
using Inventec.Core;
using Inventec.Desktop.Common.LibraryMessage;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Transaction
{
    public partial class UCTransaction : UserControlBase
    {

        private void btnDeposit_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnDeposit.Enabled || this.currentTreatment == null)
                    return;
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionDeposit").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TransactionDeposit'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    moduleData.RoomId = this.RoomId;
                    moduleData.RoomTypeId = this.RoomTypeId;
                    List<object> listArgs = new List<object>();
                    TransactionDepositADO ado = new TransactionDepositADO(this.currentTreatment, this.cashierRoom.ID);
                    listArgs.Add(ado);
                    listArgs.Add(moduleData);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("moduleData is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                    FillDataToControlBySelectTreatment(true);
                    txtFindTreatmentCode.Focus();
                    txtFindTreatmentCode.SelectAll();
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

        // kiểm tra đã có giao dịch thanh toán trước khi kết thúc điều trị hay chưa
        //+ HSDT đã kết thúc
        //+ thanh toán < phải thu bn
        bool CheckTransactionBillBeforeTreatmentEnd()
        {
            bool result = true;
            try
            {
                if (this.currentTreatment != null && this.currentTreatment.IS_PAUSE == 1)
                {
                    if ((this.currentTreatment.TOTAL_BILL_AMOUNT ?? 0) < (this.currentTreatment.TOTAL_PATIENT_PRICE ?? 0))
                    {
                        result = false;
                    }
                }
                else
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void btnRepay_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnRepay.Enabled || this.currentTreatment == null)
                    return;
                if (!CheckTransactionBillBeforeTreatmentEnd() && HisConfigCFG.IsNotBillCFG == "2")
                {
                    MessageBox.Show("Bệnh nhân ra viện nhưng chưa thanh toán");
                    return;
                }
                else if (!CheckTransactionBillBeforeTreatmentEnd() && HisConfigCFG.IsNotBillCFG == "1")
                {
                    if (MessageBox.Show("Bệnh nhân ra viện nhưng chưa thanh toán đủ chi phí, bạn có muốn hoàn ứng?", MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) == DialogResult.No)
                        return;
                }

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionRepay").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TransactionRepay'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    moduleData.RoomId = this.RoomId;
                    moduleData.RoomTypeId = this.RoomTypeId;
                    List<object> listArgs = new List<object>();
                    TransactionRepayADO ado = new TransactionRepayADO(this.currentTreatment.ID, this.cashierRoom.ID);
                    ado.PatientTypeAlter = this.lastPatientType;
                    ado.Treatment = this.currentTreatment;
                    listArgs.Add(ado);
                    listArgs.Add(moduleData);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("moduleData is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                    FillDataToControlBySelectTreatment(true);
                    txtFindTreatmentCode.Focus();
                    txtFindTreatmentCode.SelectAll();
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

        private void btnBill_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnBill.Enabled || this.currentTreatment == null)
                    return;

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionBillSelect").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TransactionBillSelect'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    moduleData.RoomId = this.RoomId;
                    moduleData.RoomTypeId = this.RoomTypeId;
                    List<object> listArgs = new List<object>();

                    listArgs.Add(this.currentTreatment);
                    listArgs.Add(this.listSereServ);
                    listArgs.Add(this.lastPatientType);
                    listArgs.Add(moduleData);
                    listArgs.Add(false);
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.RoomId, this.RoomTypeId), listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("extenceInstance is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                    FillDataToControlBySelectTreatment(true);
                    txtFindTreatmentCode.Focus();
                    txtFindTreatmentCode.SelectAll();
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

        private void btnLock_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnLock.Enabled || this.currentTreatment == null)
                    return;
                if (this.currentTreatment != null && this.currentTreatment.IS_LOCK_HEIN == 1)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu đã duyệt BHYT.", Base.ResourceMessageLang.TieuDeCuaSoThongBaoLaThongBao, MessageBoxButtons.OK);
                        return;
                }
                if (this.currentTreatment.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                    && btnLock.Text == Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__BTN_LOCK_FEE", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()))
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentLockFee").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TreatmentLockFee'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        moduleData.RoomId = this.RoomId;
                        moduleData.RoomTypeId = this.RoomTypeId;
                        List<object> listArgs = new List<object>();
                        listArgs.Add(this.currentTreatment.ID);
                        listArgs.Add(moduleData);
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, RoomId, RoomTypeId), listArgs);
                        if (extenceInstance == null)
                        {
                            throw new ArgumentNullException("moduleData is null");
                        }

                        ((Form)extenceInstance).ShowDialog();
                        FillDataToControlBySelectTreatment(true);
                        SetEnableButton(null);
                        txtFindTreatmentCode.Focus();
                        txtFindTreatmentCode.SelectAll();
                    }
                    else
                    {
                        MessageManager.Show(Base.ResourceMessageLang.ChucNangNayChuaDuocHoTroTrongPhienBanNay);
                    }
                }
                else if (this.currentTreatment.IS_ACTIVE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                    && btnLock.Text == Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__BTN_UNLOCK_FEE", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()))
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(Base.ResourceMessageLang.NguoiDungCoMuonMoKhoaVienPhi, Base.ResourceMessageLang.TieuDeCuaSoThongBaoLaThongBao, MessageBoxButtons.YesNo) != DialogResult.Yes)
                        return;

                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    bool success = false;
                    HisTreatmentLockSDO sdo = new HisTreatmentLockSDO();
                    sdo.TreatmentId = this.currentTreatment.ID;
                    sdo.RequestRoomId = this.RoomId;

                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TREATMENT>("api/HisTreatment/Unlock", ApiConsumers.MosConsumer, sdo, param);
                    if (rs != null)
                    {
                        success = true;
                        FillDataToControlBySelectTreatment(true);
                        SetEnableButton(null);
                    }
                    txtFindTreatmentCode.Focus();
                    txtFindTreatmentCode.SelectAll();
                    WaitingManager.Hide();
                    if (success)
                    {
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                    else
                    {
                        MessageManager.Show(param, success);
                    }
                    SessionManager.ProcessTokenLost(param);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnInvoice_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnInvoice.Enabled || this.currentTreatment == null)
                    return;
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.InvoiceCreateForTreatment").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.InvoiceCreateForTreatment'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(moduleData);
                    listArgs.Add(this.currentTreatment.ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("moduleData is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                    txtFindTreatmentCode.Focus();
                    txtFindTreatmentCode.SelectAll();
                    //FillDataToControlBySelectTreatment(true);
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

        private void btnDepositService_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnDepositService.Enabled || this.currentTreatment == null)
                    return;
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.DepositService").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.DepositService'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    moduleData.RoomId = this.RoomId;
                    moduleData.RoomTypeId = this.RoomTypeId;
                    List<object> listArgs = new List<object>();
                    DepositServiceADO ado = new DepositServiceADO();
                    ado.hisTreatment = this.currentTreatment;
                    ado.BRANCH_ID = WorkPlace.GetBranchId();
                    ado.CashierRoomId = this.cashierRoom.ID;
                    //if (this.listSereServ.Where(o => o.VIR_TOTAL_PATIENT_PRICE > 0) != null && this.listSereServ.Where(o => o.VIR_TOTAL_PATIENT_PRICE > 0).Count() > 0)
                    //{

                    ado.SereServs = this.listSereServ;
                    //} 
                    listArgs.Add(ado);
                    listArgs.Add(moduleData);
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, RoomId, RoomTypeId), listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("moduleData is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                    FillDataToControlBySelectTreatment(true);
                    txtFindTreatmentCode.Focus();
                    txtFindTreatmentCode.SelectAll();
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

        private void btnRapayService_Click(object sender, EventArgs e)
        {
            try
            {
                //thinhdt2
                // sua lai kiem tra nut
                if (!btnRepayService.Enabled || this.currentTreatment == null)
                    return;
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.RepayService").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.RepayService'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    moduleData.RoomId = this.RoomId;
                    moduleData.RoomTypeId = this.RoomTypeId;
                    List<object> listArgs = new List<object>();
                    RepayServiceADO ado = new RepayServiceADO();
                    ado.hisTreatment = this.currentTreatment;
                    ado.branchId = WorkPlace.GetBranchId();
                    ado.cashierRoomId = cashierRoom.ID;
                    ado.ListSereServ = this.listSereServ;
                    listArgs.Add(ado);
                    listArgs.Add(moduleData);
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, RoomId, RoomTypeId), listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("moduleData is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                    FillDataToControlBySelectTreatment(true);
                    txtFindTreatmentCode.Focus();
                    txtFindTreatmentCode.SelectAll();
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

        // kết chuyển
        private void btnTranfer_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnTranfer.Enabled || this.currentTreatment == null)
                    return;
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.BillTransferAccounting").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.BillTransferAccounting'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    moduleData.RoomId = this.RoomId;
                    moduleData.RoomTypeId = this.RoomTypeId;
                    List<object> listArgs = new List<object>();
                    BillTransferADO ado = new BillTransferADO(this.currentTreatment.ID, this.cashierRoom.ID);
                    listArgs.Add(ado);
                    listArgs.Add(moduleData);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("moduleData is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                    FillDataToControlBySelectTreatment(true);
                    txtFindTreatmentCode.Focus();
                    txtFindTreatmentCode.SelectAll();
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

        private void btnLockHistory_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnLockHistory.Enabled || this.currentTreatment == null)
                    return;
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentLockList").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TreatmentLockList'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    moduleData.RoomId = this.RoomId;
                    moduleData.RoomTypeId = this.RoomTypeId;
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentTreatment.ID);
                    listArgs.Add(moduleData);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("moduleData is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                    txtFindTreatmentCode.Focus();
                    txtFindTreatmentCode.SelectAll();
                }
                else
                {
                    MessageManager.Show(Base.ResourceMessageLang.ChucNangNayChuaDuocHoTroTrongPhienBanNay);
                }
                //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPaan").FirstOrDefault();
                //if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.AssignPaan'");
                //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                //{
                //    List<object> listArgs = new List<object>();
                //    listArgs.Add(this.currentTreatment.ID);
                //    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, RoomId, RoomTypeId), listArgs);
                //    if (extenceInstance == null)
                //    {
                //        throw new ArgumentNullException("moduleData is null");
                //    }

                //    ((Form)extenceInstance).ShowDialog();
                //}
                //else
                //{
                //    MessageManager.Show(Base.ResourceMessageLang.ChucNangNayChuaDuocHoTroTrongPhienBanNay);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnBordereau_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnBordereau.Enabled || this.currentTreatment == null)
                    return;
                this.OpenFormBordereau(this.currentTreatment);
                this.FillDataToControlBySelectTreatment(true);
                txtFindTreatmentCode.Focus();
                txtFindTreatmentCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnTranList_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnTranList.Enabled || this.currentTreatment == null)
                    return;
                OpenFormTransactionList();
                txtFindTreatmentCode.Focus();
                txtFindTreatmentCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnFind.Enabled)
                    return;
                WaitingManager.Show();
                FillDataToGridTreatment();
                //if(chkNoVP.CheckState == CheckState.Checked)
                //{
                //FillDataToControlByCheckIsInDebt();
                //}
                if (listTreatment != null && listTreatment.Count == 1)
                {
                    this.currentTreatment = listTreatment.First();
                    FillDataToControlBySelectTreatment(false, true);
                    SetEnableButton(null);
                }
                txtFindTreatmentCode.Focus();
                txtFindTreatmentCode.SelectAll();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void FillDataToControlByCheckIsInDebt()
        //{

        //    try
        //    {
        //        HisTreatmentFeeViewFilter feeFilter = new HisTreatmentFeeViewFilter();
        //        //feeFilter = new BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST_MEDICINE>>(RequestUriStore.HIS_EXP_MEST_MEDICINE_GET, ApiConsumer.ApiConsumers.MosConsumer, expMestMedicineFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}
        private void txtFindPatientCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (Char.IsDigit(e.KeyChar) || Char.IsControl(e.KeyChar))
                {
                }
                else
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void txtFindPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtFindPatientCode.Text))
                    {
                        positionHandleControl = -1;
                        if (!dxValidationProvider1.Validate())
                            return;
                        WaitingManager.Show();
                        FillDataToGridTreatment();
                        if (listTreatment != null && listTreatment.Count == 1)
                        {
                            this.currentTreatment = listTreatment.First();
                            FillDataToControlBySelectTreatment(false, true);
                            SetEnableButton(null);
                        }
                        txtFindPatientCode.SelectAll();
                        WaitingManager.Hide();
                    }
                    else
                    {
                        txtKeyword.Focus();
                        txtKeyword.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

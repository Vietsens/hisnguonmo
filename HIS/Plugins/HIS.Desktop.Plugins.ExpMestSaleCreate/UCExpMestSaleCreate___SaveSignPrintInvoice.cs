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
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using DevExpress.XtraEditors;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestSaleCreate
{
    /// <summary>
    /// Viec 3082 (29/08/2026): nut MOI "Luu ky in (Ctrl E / F11)" canh nut "Luu in".
    /// Enable/disable theo checkbox "Xuat bien lai/hoa don" (chkCreateBill) co san: tick -> enable, bo tick -> disable
    /// (va nut Luu in dang enable). KHONG dung key config, KHONG checkbox rieng. Nut "Luu in" cu giu nguyen.
    /// Bam "Luu ky in": Luu phieu (BE tao bill ngay khi luu vi dang tick Xuat bien lai/hoa don) -> mo form Xuat hoa don (F10)
    /// o che do tu dong "phat hanh cho bill da co": kiem tra ton -> phat hanh (ky) HDDT VNPT -> tu dong duyet/thuc xuat
    /// (neu chua hoan thanh) -> in thang hoa don -> form tu dong. Luot nay KHONG in phieu xuat ban.
    /// </summary>
    public partial class UCExpMestSaleCreate : UserControlBase
    {
        /// <summary>Marker truyen sang form hoa don — PHAI TRUNG Config.AUTO_ACTION__* ben plugin MedicineSaleBill</summary>
        private const string MEDICINE_SALE_BILL__AUTO_ACTION__SAVE_SIGN_PRINT = "AUTO_SAVE_SIGN_PRINT";
        private const string MEDICINE_SALE_BILL__AUTO_ACTION__ISSUE_EXISTING_BILL = "AUTO_ISSUE_EXISTING_BILL";
        private const string MEDICINE_SALE_BILL__AUTO_PARAM__TRANSACTION_ID = "TRANSACTION_ID=";
        private const string MEDICINE_SALE_BILL__MODULE_LINK = "HIS.Desktop.Plugins.MedicineSaleBill";
        private const string CONFIG_KEY__USING_FUNCTION_KEY = "HIS.Desktop.Plugins.ExpMestSaleCreate.IsUsingFunctionKeyInsteadOfCtrlKey";

        /// <summary>
        /// true trong luot bam "Luu ky in": sau khi luu phieu thanh cong thi mo form hoa don tu dong
        /// (thay cho in phieu xuat ban va thay cho nhanh config Show_MedicineSaleBill).
        /// </summary>
        private bool savePrintInvoice = false;

        /// <summary>Da gan cac su kien dong bo trang thai nut Luu ky in chua</summary>
        private bool isSaveSignPrintButtonWired = false;

        /// <summary>
        /// Khoi tao nut "Luu ky in": nhan phim tat, gan su kien dong bo (tick "Xuat bien lai/hoa don", Enabled cua Luu in,
        /// Enabled/Visible cua UC) va refresh ngay + refresh tre sau khi Load xong.
        /// Goi trong InitControlState.
        /// </summary>
        private void InitSaveSignPrintButton()
        {
            try
            {
                bool isUsingFunctionKey = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__USING_FUNCTION_KEY) == "1";
                btnSaveSignPrint.Text = isUsingFunctionKey ? "Lưu ký in (F11)" : "Lưu ký in (Ctrl E)";

                if (!isSaveSignPrintButtonWired)
                {
                    // Control.Enabled la trang thai HIEU DUNG (false khi form cha dang bi WaitingManager khoa luc load)
                    // va LayoutControl khong chac lan EnabledChanged tu form cha xuong tung nut -> bat nhieu moc + refresh tre.
                    chkCreateBill.CheckedChanged += new EventHandler(SaveSignPrintState_Changed);
                    btnSavePrint.EnabledChanged += new EventHandler(SaveSignPrintState_Changed);
                    this.EnabledChanged += new EventHandler(SaveSignPrintState_Changed);
                    this.VisibleChanged += new EventHandler(SaveSignPrintState_Changed);
                    isSaveSignPrintButtonWired = true;
                }
                RefreshSaveSignPrintButton();
                if (this.IsHandleCreated)
                {
                    this.BeginInvoke(new Action(RefreshSaveSignPrintButton));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveSignPrintState_Changed(object sender, EventArgs e)
        {
            RefreshSaveSignPrintButton();
        }

        /// <summary>
        /// Dieu kien enable nut "Luu ky in": tick "Xuat bien lai/hoa don" + nut Luu in dang enable.
        /// Khi UC/form cha dang bi disable (Enabled hieu dung = false) thi KHONG ket luan — cho lan refresh sau.
        /// </summary>
        private void RefreshSaveSignPrintButton()
        {
            try
            {
                if (!chkCreateBill.Checked)
                {
                    btnSaveSignPrint.Enabled = false;
                    return;
                }
                if (!this.Enabled)
                {
                    return;
                }
                btnSaveSignPrint.Enabled = btnSavePrint.Enabled;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Nut "Luu ky in": luu phieu (BE tao bill vi dang tick Xuat bien lai/hoa don, khong in phieu xuat ban)
        /// -> ProcessSave thay flag savePrintInvoice -> mo form hoa don tu dong. Flag reset trong finally.
        /// </summary>
        private void btnSaveSignPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSaveSignPrint.Enabled || !chkCreateBill.Checked)
                {
                    return;
                }
                this.savePrintInvoice = true;
                this.savePrint = false;
                try
                {
                    btnSave_Click(null, null);
                }
                finally
                {
                    this.savePrintInvoice = false;
                }
            }
            catch (Exception ex)
            {
                this.savePrintInvoice = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Phim tat Ctrl E / F11 (KeyboardWorker)</summary>
        public void BtnSaveSignPrintShortcut()
        {
            try
            {
                if (btnSaveSignPrint.Enabled)
                {
                    btnSaveSignPrint.Focus();
                    btnSaveSignPrint_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Mo form Xuat hoa don (HIS.Desktop.Plugins.MedicineSaleBill) o che do tu dong cho tung ket qua luu
        /// (1 benh nhan: resultSDO; nhieu benh nhan: ListResultSDO).
        /// - Co Transaction (bill da tao khi luu): marker ISSUE_EXISTING_BILL + TRANSACTION_ID -> form phat hanh HDDT cho bill do,
        ///   duyet/thuc xuat, in, tu dong. Hinh thuc QR: bo qua (ProcessSave da mo module QR).
        /// - Khong co Transaction (BE khong tao bill): marker SAVE_SIGN_PRINT -> form tu tao bill + phat hanh nhu F10.
        /// Callback EnableControlAfterSaveSaleBill giu nhu nut F10.
        /// </summary>
        private void OpenMedicineSaleBillAutoSignPrint()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == MEDICINE_SALE_BILL__MODULE_LINK).FirstOrDefault();
                if (moduleData == null || !moduleData.IsPlugin || moduleData.ExtensionInfo == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("OpenMedicineSaleBillAutoSignPrint: Not found module by ModuleLink = '" + MEDICINE_SALE_BILL__MODULE_LINK + "'");
                    XtraMessageBox.Show("Không tìm thấy chức năng Xuất hóa đơn (" + MEDICINE_SALE_BILL__MODULE_LINK + "). Phiếu đã lưu, vui lòng phát hành hóa đơn điện tử thủ công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                List<HisExpMestSaleListResultSDO> results = new List<HisExpMestSaleListResultSDO>();
                if (isTwoPatient && this.ListResultSDO != null && this.ListResultSDO.Count > 0)
                {
                    results.AddRange(this.ListResultSDO.Where(o => o != null));
                }
                else if (this.resultSDO != null)
                {
                    results.Add(this.resultSDO);
                }

                foreach (var rs in results)
                {
                    if (rs.ExpMestSdos == null || rs.ExpMestSdos.Count == 0)
                        continue;

                    List<string> autoActions = new List<string>();
                    List<long> expMestIds = null;
                    if (rs.Transaction != null)
                    {
                        if (rs.Transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR)
                        {
                            Inventec.Common.Logging.LogSystem.Info("Viec 3082: bill hinh thuc QR -> khong phat hanh HDDT tu dong. TransactionId = " + rs.Transaction.ID);
                            continue;
                        }
                        expMestIds = rs.ExpMestSdos.Where(p => p.ExpMest != null).Select(p => p.ExpMest.ID).Distinct().ToList();
                        autoActions.Add(MEDICINE_SALE_BILL__AUTO_ACTION__ISSUE_EXISTING_BILL);
                        autoActions.Add(MEDICINE_SALE_BILL__AUTO_PARAM__TRANSACTION_ID + rs.Transaction.ID);
                    }
                    else
                    {
                        // BE khong tao bill khi luu -> form tu tao bill + phat hanh (chi phieu chua co bill)
                        expMestIds = rs.ExpMestSdos.Where(p => p.ExpMest != null && !p.ExpMest.BILL_ID.HasValue).Select(p => p.ExpMest.ID).Distinct().ToList();
                        autoActions.Add(MEDICINE_SALE_BILL__AUTO_ACTION__SAVE_SIGN_PRINT);
                    }
                    if (expMestIds == null || expMestIds.Count == 0)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("OpenMedicineSaleBillAutoSignPrint: khong co phieu phu hop de mo form tu dong. "
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
                        continue;
                    }

                    Inventec.Common.Logging.LogSystem.Info("Viec 3082: Luu ky in -> mo form Xuat hoa don tu dong. Actions = " + string.Join(",", autoActions) + "; ExpMestIds = " + string.Join(",", expMestIds));
                    moduleData.RoomId = this.roomId;
                    moduleData.RoomTypeId = this.roomTypeId;
                    List<object> listArgs = new List<object>();
                    listArgs.Add(moduleData);
                    listArgs.Add(expMestIds);
                    listArgs.Add((DelegateSelectData)EnableControlAfterSaveSaleBill);
                    listArgs.Add(autoActions);
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.roomId, this.roomTypeId), listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("extenceInstance is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

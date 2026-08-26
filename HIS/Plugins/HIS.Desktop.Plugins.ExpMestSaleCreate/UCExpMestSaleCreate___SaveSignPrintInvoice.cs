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
using HIS.Desktop.Plugins.ExpMestSaleCreate.Base;
using HIS.Desktop.Utility;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestSaleCreate
{
    /// <summary>
    /// Viec 3082 (25/08/2026): checkbox "In" canh "Ky don nha thuoc".
    /// Tick "In" + bam "Luu in (F9)" -> Luu phieu -> mo form Xuat hoa don (F10) o CHE DO TU DONG:
    /// kiem tra ton -> tao bill + phat hanh (ky) HDDT -> tu dong duyet/thuc xuat phieu (neu chua hoan thanh)
    /// -> in thang hoa don -> form tu dong. KHONG in phieu xuat ban trong luot nay.
    /// Gate: key HIS.Desktop.Plugins.MedicineSaleBill.SaveSignPrintAutoExport = 1 (dung chung voi form hoa don).
    /// </summary>
    public partial class UCExpMestSaleCreate : UserControlBase
    {
        /// <summary>Marker truyen sang form hoa don qua args (List&lt;string&gt;) — PHAI TRUNG Config.AUTO_ACTION__SAVE_SIGN_PRINT ben plugin MedicineSaleBill</summary>
        private const string MEDICINE_SALE_BILL__AUTO_ACTION__SAVE_SIGN_PRINT = "AUTO_SAVE_SIGN_PRINT";
        private const string MEDICINE_SALE_BILL__MODULE_LINK = "HIS.Desktop.Plugins.MedicineSaleBill";
        private const string CONFIG_KEY__SAVE_SIGN_PRINT_AUTO_EXPORT = "HIS.Desktop.Plugins.MedicineSaleBill.SaveSignPrintAutoExport";

        /// <summary>
        /// true trong luot "Luu in" co tick "In": sau khi luu phieu thanh cong thi mo form hoa don tu dong
        /// (thay cho in phieu xuat ban va thay cho nhanh config Show_MedicineSaleBill).
        /// </summary>
        private bool savePrintInvoice = false;

        /// <summary>Key config 3082 dang bat?</summary>
        private bool IsSaveSignPrintAutoExportEnabled()
        {
            try
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__SAVE_SIGN_PRINT_AUTO_EXPORT) == "1";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>
        /// Hien/an checkbox "In" theo key config. Goi trong InitControlState (flag isNotLoadWhileChangeControlStateInFirst dang bat
        /// nen bo tick khi key tat khong ghi ControlState).
        /// </summary>
        private void SetPrintInvoiceCheckboxByConfig(bool isEnabled)
        {
            try
            {
                lciPrintInvoice.Visibility = isEnabled ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                if (!isEnabled)
                {
                    chkPrintInvoice.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Luot "Luu in" hien tai co chay chuoi HDDT (tick "In" + key bat) khong</summary>
        private bool IsSavePrintInvoiceMode()
        {
            try
            {
                return lciPrintInvoice.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                    && chkPrintInvoice.Checked
                    && IsSaveSignPrintAutoExportEnabled();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>Nho trang thai tick "In" giua cac phien (ControlState) — cung mau voi chkSign_CheckedChanged</summary>
        private void chkPrintInvoice_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (UCExpMestSaleCreate.currentControlStateRDO != null && UCExpMestSaleCreate.currentControlStateRDO.Count > 0)
                    ? UCExpMestSaleCreate.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.CHK_PRINT_INVOICE && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault()
                    : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrintInvoice.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.CHK_PRINT_INVOICE;
                    csAddOrUpdate.VALUE = (chkPrintInvoice.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (UCExpMestSaleCreate.currentControlStateRDO == null)
                        UCExpMestSaleCreate.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    UCExpMestSaleCreate.currentControlStateRDO.Add(csAddOrUpdate);
                }
                UCExpMestSaleCreate.controlStateWorker.SetData(UCExpMestSaleCreate.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Mo form Xuat hoa don (HIS.Desktop.Plugins.MedicineSaleBill) o che do tu dong cho tung ket qua luu
        /// (1 benh nhan: resultSDO; nhieu benh nhan: ListResultSDO). Form tu chay Luu ky + duyet/thuc xuat + in roi tu dong;
        /// neu co buoc fail thi form giu mo de nguoi dung xu ly tiep. Callback EnableControlAfterSaveSaleBill giu nhu nut F10.
        /// </summary>
        private void OpenMedicineSaleBillAutoSignPrint()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == MEDICINE_SALE_BILL__MODULE_LINK).FirstOrDefault();
                if (moduleData == null || !moduleData.IsPlugin || moduleData.ExtensionInfo == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("OpenMedicineSaleBillAutoSignPrint: Not found module by ModuleLink = '" + MEDICINE_SALE_BILL__MODULE_LINK + "'");
                    XtraMessageBox.Show("Không tìm thấy chức năng Xuất hóa đơn (" + MEDICINE_SALE_BILL__MODULE_LINK + "). Phiếu đã lưu, vui lòng bấm Xuất hóa đơn (F10) để xử lý thủ công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                    // Form hoa don chi tim phieu chua co bill (HAS_BILL_ID = false) -> bo qua phieu da co bill
                    List<long> expMestIds = rs.ExpMestSdos
                        .Where(p => p.ExpMest != null && !p.ExpMest.BILL_ID.HasValue)
                        .Select(p => p.ExpMest.ID).Distinct().ToList();
                    if (expMestIds.Count == 0)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("OpenMedicineSaleBillAutoSignPrint: phieu da co bill, khong mo form tu dong. "
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
                        continue;
                    }

                    Inventec.Common.Logging.LogSystem.Info("Viec 3082: Luu in + tick In -> mo form Xuat hoa don tu dong. ExpMestIds = " + string.Join(",", expMestIds));
                    moduleData.RoomId = this.roomId;
                    moduleData.RoomTypeId = this.roomTypeId;
                    List<object> listArgs = new List<object>();
                    listArgs.Add(moduleData);
                    listArgs.Add(expMestIds);
                    listArgs.Add((DelegateSelectData)EnableControlAfterSaveSaleBill);
                    listArgs.Add(new List<string> { MEDICINE_SALE_BILL__AUTO_ACTION__SAVE_SIGN_PRINT });
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

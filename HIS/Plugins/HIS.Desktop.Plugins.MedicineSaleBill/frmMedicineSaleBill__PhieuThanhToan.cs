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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.MedicineSaleBill
{
    /// <summary>
    /// Viec 59437: in phieu thanh toan nha thuoc theo bieu mau Mps000339 (Hoa don xuat ban), vien chon file mau "Phieu thanh toan".
    /// - O tich "In phieu thu" (chkPrintPaymentSlip): tich roi an Luu / Luu in -> tu dong in sau khi luu thanh cong.
    ///   Doc lap voi o "In bien lai" (chkPrintReceipt, BVNTP, Mps000092). Luu ky khong tu in.
    /// - Menu In -> "In phieu thu": in lai cho giao dich vua luu.
    /// Du lieu in dung chung InHoaDonXuatBan (case "Mps000339" trong deletePrintTemplate).
    /// </summary>
    public partial class frmMedicineSaleBill : HIS.Desktop.Utility.FormBase
    {
        private const string PRINT_TYPE_CODE__PHIEU_THANH_TOAN_NHA_THUOC = "Mps000339";

        private void InitPrintPaymentSlipControl()
        {
            try
            {
                this.lciPrintPaymentSlip.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MEDICINE_SALE_BILL__LCI_PRINT_PAYMENT_SLIP", Base.ResourceLangManager.LanguagefrmMedicineSaleBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.chkPrintPaymentSlip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_MEDICINE_SALE_BILL__CHK_PRINT_PAYMENT_SLIP__TOOLTIP", Base.ResourceLangManager.LanguagefrmMedicineSaleBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPrintPaymentSlip_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst || this.controlStateWorker == null || this.module == null)
                {
                    return;
                }
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPrintPaymentSlip.Name && o.MODULE_LINK == module.ModuleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrintPaymentSlip.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPrintPaymentSlip.Name;
                    csAddOrUpdate.VALUE = (chkPrintPaymentSlip.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = module.ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Che do in Mps000339 khi tu in sau khi luu; null = theo cau hinh che do in chung (menu In, Luu, Luu ky)</summary>
        private MPS.ProcessorBase.PrintConfig.PreviewType? paymentSlipPreviewType = null;

        /// <summary>
        /// Goi sau khi Luu / Luu in / Luu ky thanh cong: co tich "In hoa don xuat ban" (chkPrintPaymentSlip) thi in Mps000339
        /// cua giao dich vua tao. previewType: Luu in truyen PrintNow (in thang, khong xem truoc); null = theo cau hinh chung.
        /// </summary>
        private void PrintPaymentSlipAfterSave(MPS.ProcessorBase.PrintConfig.PreviewType? previewType = null)
        {
            try
            {
                if (!chkPrintPaymentSlip.Checked || this.transactionBillResult == null)
                    return;
                this.paymentSlipPreviewType = previewType;
                this.onClickInPhieuThanhToan(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                this.paymentSlipPreviewType = null;
            }
        }

        /// <summary>
        /// Viec 59437: menu In -> "In bien lai": in bien lai Mps000092 (vien chon mau Bien nhan thanh toan) theo che do in chung.
        /// Cung duong in voi o "In bien lai" (chkPrintReceipt, BVNTP) nhung khong phu thuoc o tich.
        /// </summary>
        private void onClickInBienLai(object sender, EventArgs e)
        {
            try
            {
                if (this.transactionBillResult == null)
                    return;
                this.onClickInPhieuXuatBan(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickInPhieuThanhToan(object sender, EventArgs e)
        {
            try
            {
                if (this.transactionBillResult == null)
                    return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PRINT_TYPE_CODE__PHIEU_THANH_TOAN_NHA_THUOC, deletePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

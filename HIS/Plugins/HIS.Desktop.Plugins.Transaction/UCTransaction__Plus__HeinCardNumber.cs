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
using HIS.Desktop.Plugins.Transaction.Config;
using HIS.Desktop.Utility;
using Inventec.Common.QrCodeBHYT;
using Inventec.Desktop.Common.Message;
using MOS.Filter;
using System;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Transaction
{
    /// <summary>
    /// vCong61015 (PTTK_61015): o "So the BHYT" tren hang o ma cua man Vien phi - nhap tay hoac quet the,
    /// tim theo so the day du (15/17 ky tu - bang dung) hoac 10 so cuoi (ket thuc bang), ket hop cac dieu kien loc hien co.
    /// Chi hoat dong khi khoa HIS.Desktop.Plugins.Transaction.IsShowHeinCardNumberFind = 1;
    /// khoa tat -> o an, khong dang ky xu ly gi them, man chay y nhu truoc.
    /// </summary>
    public partial class UCTransaction
    {
        private const int HEIN_CARD_NUMBER_FIND_MODE__NONE = 0;
        private const int HEIN_CARD_NUMBER_FIND_MODE__EXACT = 1;
        private const int HEIN_CARD_NUMBER_FIND_MODE__END_WITH = 2;

        /// <summary>
        /// Thu ngan da chu dong doi Ngay vao / Ngay ra. He thong tu dat lai ngay (mo man, doi che do Ngay vao / Ngay ra)
        /// thi xoa co (ResetCashierChangedDate). Chi dung cho luot tim theo so the BHYT.
        /// </summary>
        private bool isCashierChangedDate = false;

        /// <summary>Goi khi tai man (sau LoadConfig): an / hien o theo khoa; khoa bat moi theo doi thu ngan doi ngay.</summary>
        private void InitHeinCardNumberFind()
        {
            try
            {
                if (!HisConfigCFG.IsShowHeinCardNumberFind)
                {
                    lciHeinCardNumber.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    return;
                }
                lciHeinCardNumber.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                dtInTimeFrom.EditValueChanged += dtFilterTime_EditValueChanged__HeinCardNumber;
                dtInTimeTo.EditValueChanged += dtFilterTime_EditValueChanged__HeinCardNumber;
                dtOutTimeFrom.EditValueChanged += dtFilterTime_EditValueChanged__HeinCardNumber;
                dtOutTimeTo.EditValueChanged += dtFilterTime_EditValueChanged__HeinCardNumber;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtFilterTime_EditValueChanged__HeinCardNumber(object sender, EventArgs e)
        {
            try
            {
                this.isCashierChangedDate = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>He thong vua tu dat lai Ngay vao / Ngay ra -> xoa dau hieu "thu ngan da doi ngay".</summary>
        private void ResetCashierChangedDate()
        {
            this.isCashierChangedDate = false;
        }

        /// <summary>
        /// Luot tim hien tai co dung dieu kien so the BHYT khong: khoa bat, o So the co gia tri
        /// va 3 o ma (Ma dieu tri / Ma benh nhan / So vao vien) deu trong (giu nguyen thu tu uu tien hien co).
        /// </summary>
        private bool IsHeinCardNumberFindActive()
        {
            return HisConfigCFG.IsShowHeinCardNumberFind
                && !String.IsNullOrWhiteSpace(txtHeinCardNumber.Text)
                && String.IsNullOrEmpty(txtFindTreatmentCode.Text)
                && String.IsNullOrEmpty(txtFindPatientCode.Text)
                && String.IsNullOrEmpty(txtInCode.Text);
        }

        /// <summary>
        /// Cach tim theo chuoi da chuan hoa: 15 hoac 17 ky tu -> bang dung; dung 10 chu so -> ket thuc bang; khac -> khong hop le.
        /// </summary>
        private int GetHeinCardNumberFindMode(string heinCardNumber)
        {
            if (String.IsNullOrEmpty(heinCardNumber))
                return HEIN_CARD_NUMBER_FIND_MODE__NONE;
            if (heinCardNumber.Length == 15 || heinCardNumber.Length == 17)
                return HEIN_CARD_NUMBER_FIND_MODE__EXACT;
            if (heinCardNumber.Length == 10 && heinCardNumber.All(Char.IsDigit))
                return HEIN_CARD_NUMBER_FIND_MODE__END_WITH;
            return HEIN_CARD_NUMBER_FIND_MODE__NONE;
        }

        /// <summary>Chuoi trong o sau khi bo dau cach / gach noi va chuyen chu hoa (dung chung cho loc va kiem tra).</summary>
        private string GetHeinCardNumberNormalized()
        {
            return HeinCardHelper.TrimHeinCardNumber((txtHeinCardNumber.Text ?? "").Trim());
        }

        /// <summary>
        /// Chuan bi truoc khi tim theo so the BHYT (Enter trong o, nut Tim, Ctrl F):
        /// chuoi QR the BHYT (co '|') -> giai ma bang thu vien dung chung, lay so the va ghi lai vao o;
        /// bo dau cach / gach noi, chu hoa; sai do dai -> canh bao, con tro o lai o, KHONG tim.
        /// Tra ve true khi luot tim nay khong dung dieu kien so the (khong can lam gi) hoac chuoi hop le.
        /// </summary>
        private bool PrepareHeinCardNumberFind()
        {
            try
            {
                if (!this.IsHeinCardNumberFindActive())
                    return true;

                string input = txtHeinCardNumber.Text.Trim();
                string heinCardNumber = input;
                if (input.Contains("|"))
                {
                    ReadQrCodeHeinCard readQrCode = new ReadQrCodeHeinCard();
                    HeinCardData dataHein = readQrCode.ReadDataQrCode(input);
                    heinCardNumber = (dataHein != null && dataHein.HeinCardNumber != null) ? dataHein.HeinCardNumber : "";
                }
                heinCardNumber = HeinCardHelper.TrimHeinCardNumber(heinCardNumber);
                if (!String.IsNullOrEmpty(heinCardNumber) && heinCardNumber != input)
                {
                    txtHeinCardNumber.Text = heinCardNumber;
                }

                if (this.GetHeinCardNumberFindMode(heinCardNumber) == HEIN_CARD_NUMBER_FIND_MODE__NONE)
                {
                    MessageManager.Show(Base.ResourceMessageLang.SoTheBhytPhaiDu15KyTuHoac10SoCuoi);
                    txtHeinCardNumber.Focus();
                    txtHeinCardNumber.SelectAll();
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return true;
        }

        /// <summary>
        /// Gan dieu kien so the BHYT (bang dung / ket thuc bang) va sap xep theo thoi gian vao vien giam dan
        /// cho luot tim theo the. Tra ve true khi luot tim nay dung dieu kien so the
        /// (ben goi se bo qua Ngay vao / Ngay ra do he thong tu dat).
        /// </summary>
        private bool SetHeinCardNumberFilter(HisTreatmentFeeViewFilter treatFilter)
        {
            try
            {
                if (!this.IsHeinCardNumberFindActive())
                    return false;

                string heinCardNumber = this.GetHeinCardNumberNormalized();
                int findMode = this.GetHeinCardNumberFindMode(heinCardNumber);
                if (findMode == HEIN_CARD_NUMBER_FIND_MODE__NONE)
                {
                    return false;
                }
                // 2 truong loc moi nam trong ham rieng (NoInlining): may tram chay voi MOS.Filter.dll cu chua co 2 truong
                // van tim binh thuong o moi luot khac; chi luot tim theo the moi gap MissingMethodException (bat o day, bo qua dieu kien the)
                this.SetHeinCardNumberFilterFields(treatFilter, findMode, heinCardNumber);
                treatFilter.ORDER_FIELD = "IN_TIME";
                treatFilter.ORDER_DIRECTION = "DESC";
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return false;
        }

        /// <summary>
        /// Gan 2 truong loc so the (TDL_HEIN_CARD_NUMBER__EXACT / __END_WITH) cua MOS.Filter - tach rieng + NoInlining de chi JIT khi thuc su tim theo the:
        /// MOS.Filter.dll cu (chua co 2 truong) khong lam hong cac luot tim khac cua man Vien phi.
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        private void SetHeinCardNumberFilterFields(HisTreatmentFeeViewFilter treatFilter, int findMode, string heinCardNumber)
        {
            if (findMode == HEIN_CARD_NUMBER_FIND_MODE__EXACT)
            {
                treatFilter.TDL_HEIN_CARD_NUMBER__EXACT = heinCardNumber;
            }
            else if (findMode == HEIN_CARD_NUMBER_FIND_MODE__END_WITH)
            {
                treatFilter.TDL_HEIN_CARD_NUMBER__END_WITH = heinCardNumber;
            }
        }
        /// <summary>Sau luot tim chu dong theo so the BHYT (Enter / Tim / Ctrl F) ma khong co ho so -> thong bao.</summary>
        private void ShowHeinCardNumberNotFound()
        {
            try
            {
                if (!this.IsHeinCardNumberFindActive())
                    return;
                if (this.GetHeinCardNumberFindMode(this.GetHeinCardNumberNormalized()) == HEIN_CARD_NUMBER_FIND_MODE__NONE)
                    return;
                if (listTreatment == null || listTreatment.Count == 0)
                {
                    MessageManager.Show(Base.ResourceMessageLang.KhongTimThayHoSoCoSoTheBhytDaNhap);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Enter trong o So the BHYT (thiet bi doc ma ket thuc chuoi bang Enter): tim nhu bam Tim, con tro o lai o.</summary>
        private void txtHeinCardNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode != Keys.Enter)
                    return;
                if (String.IsNullOrWhiteSpace(txtHeinCardNumber.Text))
                {
                    txtKeyword.Focus();
                    txtKeyword.SelectAll();
                    return;
                }
                if (!this.PrepareHeinCardNumberFind())
                    return;
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
                txtHeinCardNumber.Focus();
                txtHeinCardNumber.SelectAll();
                WaitingManager.Hide();
                this.ShowHeinCardNumberNotFound();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

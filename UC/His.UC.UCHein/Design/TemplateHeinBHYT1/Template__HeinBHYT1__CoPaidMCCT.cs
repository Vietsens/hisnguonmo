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
using His.Bhyt.InsuranceExpertise.LDO;
using His.UC.UCHein.Base;
using His.UC.UCHein.Data;
using His.UC.UCHein.Utils;
using HIS.Desktop.Plugins.Library.CheckHeinGOV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace His.UC.UCHein.Design.TemplateHeinBHYT1
{
    public partial class Template__HeinBHYT1 : UserControl
    {
        /// <summary>Number of months of base salary that forms the exemption threshold.</summary>
        const int MCCT_BASE_SALARY_MONTHS = 6;

        /// <summary>
        /// Applies a BHXH co-payment lookup to the three related fields:
        /// accumulated amount, the six-months flag and the exemption date.
        ///
        /// Asks the user before overwriting anything that differs from what the form already holds,
        /// so a value typed from a paper certificate is never silently replaced.
        /// </summary>
        internal void SetCoPaidAccumulateFromGov(ResultMCCTADO resultMcct)
        {
            try
            {
                if (resultMcct == null)
                {
                    return;
                }

                if (!resultMcct.HasData)
                {
                    // 204 means "no record", not "zero" - anything already on the form stays.
                    Inventec.Common.Logging.LogSystem.Info(
                        "SetCoPaidAccumulateFromGov: cong BHXH khong tra ve du lieu cung chi tra, giu nguyen gia tri tren form."
                        + Inventec.Common.Logging.LogUtil.TraceData(
                            Inventec.Common.Logging.LogUtil.GetMemberName(() => resultMcct.MaKetQua), resultMcct.MaKetQua));
                    return;
                }

                CoPaidMcctADO ado = this.CalculateCoPaidMcct(resultMcct);
                if (ado == null || !ado.HasAccumulate)
                {
                    return;
                }

                if (this.HasCoPaidMcctDifference(ado))
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(
                            this.BuildCoPaidMcctConfirmMessage(ado),
                            MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        Inventec.Common.Logging.LogSystem.Info(
                            "SetCoPaidAccumulateFromGov: nguoi dung tu choi lay thong tin tu cong BHXH.");
                        return;
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info(
                        "SetCoPaidAccumulateFromGov: du lieu cong BHXH trung khop voi form, cap nhat khong hoi lai.");
                }

                this.FillCoPaidMcctToForm(ado);

                if (ado.IsMissingFreeCoPaidTime)
                {
                    // Warn now instead of letting ValidateCoPaidAccumulate block the save later on.
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        ResourceMessage.KhongXacDinhDuocThoiDiemMienCungChiTra,
                        MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    if (this.txtFreeCoPainTime.Enabled)
                    {
                        this.txtFreeCoPainTime.Focus();
                        this.txtFreeCoPainTime.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Derives the three form values from the gateway payload.
        ///
        /// tBNCCTLuyKe is already an accumulated figure - it carries every earlier episode of the
        /// year - so the answer is the maximum across the records, never their sum. Max also
        /// survives the gateway changing its sort order or returning an empty ngayRa.
        /// </summary>
        private CoPaidMcctADO CalculateCoPaidMcct(ResultMCCTADO resultMcct)
        {
            CoPaidMcctADO ado = new CoPaidMcctADO();
            try
            {
                if (resultMcct == null || resultMcct.DataCCT == null || resultMcct.DataCCT.Count == 0)
                {
                    return ado;
                }

                List<DataCCTLDO> records = resultMcct.DataCCT;
                ado.DataNote = resultMcct.GhiChu;

                decimal maxAccumulate = records.Max(o => o.tBNCCTLuyKe);
                decimal firstAccumulate = records[0].tBNCCTLuyKe;
                if (firstAccumulate != maxAccumulate)
                {
                    // The gateway documents a descending sort by ngayRa; if the first record is not
                    // the largest, that contract changed and the exemption date below needs a review.
                    Inventec.Common.Logging.LogSystem.Warn(
                        "CalculateCoPaidMcct: ban ghi dau tien khong phai luy ke lon nhat, co the cong BHXH da doi thu tu sap xep."
                        + Inventec.Common.Logging.LogUtil.TraceData(
                            Inventec.Common.Logging.LogUtil.GetMemberName(() => firstAccumulate), firstAccumulate)
                        + Inventec.Common.Logging.LogUtil.TraceData(
                            Inventec.Common.Logging.LogUtil.GetMemberName(() => maxAccumulate), maxAccumulate));
                }

                ado.CoPaidAccumulateAmount = (long)Math.Round(maxAccumulate, MidpointRounding.AwayFromZero);
                ado.HasAccumulate = true;
                ado.TotalMcctAmount = records.Sum(o => o.tBNCCTMCCT);

                MOS.EFMODEL.DataModels.HIS_BHYT_PARAM bhytParam = this.GetCurrentBhytParam();
                if (bhytParam == null || bhytParam.BASE_SALARY <= 0)
                {
                    // Without a base salary neither the flag nor the date can be decided;
                    // the amount alone is still worth filling in.
                    Inventec.Common.Logging.LogSystem.Warn(
                        "CalculateCoPaidMcct: khong lay duoc luong co so tu HIS_BHYT_PARAM, chi cap nhat so tien luy ke.");
                    return ado;
                }

                decimal limit = bhytParam.BASE_SALARY * MCCT_BASE_SALARY_MONTHS;
                ado.HasThreshold = true;
                ado.IsPaid6Month = ado.CoPaidAccumulateAmount > limit;

                // Exemption starts when the episode that first crossed the threshold ended.
                // Single pass keeping the earliest discharge date instead of sorting the list.
                DateTime crossingDate = DateTime.MaxValue;
                bool hasCrossing = false;
                foreach (DataCCTLDO item in records)
                {
                    if (item == null || item.tBNCCTLuyKe <= limit)
                    {
                        continue;
                    }

                    DateTime? dischargeDate = HeinUtils.ConvertDateStringToSystemDate(item.ngayRa);
                    if (dischargeDate == null)
                    {
                        continue;
                    }

                    if (!hasCrossing || dischargeDate.Value < crossingDate)
                    {
                        crossingDate = dischargeDate.Value;
                        hasCrossing = true;
                    }
                }

                if (hasCrossing)
                {
                    ado.FreeCoPaidTime = Inventec.Common.TypeConvert.Parse.ToInt64(crossingDate.ToString("yyyyMMdd"));
                }
                else if (ado.IsPaid6Month)
                {
                    ado.IsMissingFreeCoPaidTime = true;
                }

                // Muc Info de van ghi khi moi truong chay o muc Info - can BASE_SALARY va limit
                // de doi chieu ngay khi TDMC CT suy ra khong nhu mong doi.
                decimal baseSalary = bhytParam.BASE_SALARY;
                Inventec.Common.Logging.LogSystem.Info(
                    "CalculateCoPaidMcct____"
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => baseSalary), baseSalary)
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => limit), limit)
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => ado), ado));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return ado;
        }

        /// <summary>
        /// True when at least one of the three fields on the form differs from the gateway answer.
        /// Only then is the user asked to confirm.
        /// </summary>
        private bool HasCoPaidMcctDifference(CoPaidMcctADO ado)
        {
            bool result = false;
            try
            {
                if (ado == null)
                {
                    return false;
                }

                string currentAmountStr = new string((this.txtCoPaidAccumulate.Text ?? "").Where(Char.IsDigit).ToArray());
                long currentAmount = Inventec.Common.TypeConvert.Parse.ToInt64(currentAmountStr);
                if (currentAmount != ado.CoPaidAccumulateAmount)
                {
                    return true;
                }

                if (!ado.HasThreshold)
                {
                    return false;
                }

                if (this.chkPaid6Month.Checked != ado.IsPaid6Month)
                {
                    return true;
                }

                if (ado.FreeCoPaidTime.HasValue)
                {
                    DateTime? currentDate = HeinUtils.ConvertDateStringToSystemDate(this.txtFreeCoPainTime.Text.Trim());
                    long currentTime = currentDate.HasValue
                        ? Inventec.Common.TypeConvert.Parse.ToInt64(currentDate.Value.ToString("yyyyMMdd"))
                        : 0;
                    result = currentTime != ado.FreeCoPaidTime.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Writes the derived values onto the form.
        ///
        /// Order matters: the exemption date goes in before the six-months flag, otherwise
        /// ValidateCoPaidAccumulate refuses the save while the date field is still empty.
        ///
        /// The whole block runs behind the two guard flags: assigning txtFreeCoPainTime.Text
        /// raises txtDTMCChiTra_TextChanged, which would recompute both checkboxes and could
        /// pop the "phai co giay chung nhan khong cung chi tra" question at the user.
        /// </summary>
        private void FillCoPaidMcctToForm(CoPaidMcctADO ado)
        {
            bool previousFillingFlag = this.isFillingHeinDataFromDb;
            bool previousAutoCheckFlag = this.IsAutoCheck;
            try
            {
                if (ado == null || !ado.HasAccumulate)
                {
                    return;
                }

                this.isFillingHeinDataFromDb = true;
                this.IsAutoCheck = true;

                this.txtCoPaidAccumulate.Text = ado.CoPaidAccumulateAmount.ToString();

                if (!ado.HasThreshold)
                {
                    // No base salary: the amount is trustworthy, the conclusions are not.
                    return;
                }

                if (ado.FreeCoPaidTime.HasValue)
                {
                    // Same conversion the DB fill path uses, so both produce identical text.
                    string freeCoPaidText = Inventec.Common.DateTime.Convert
                        .TimeNumberToDateString(ado.FreeCoPaidTime.Value);
                    this.txtFreeCoPainTime.Text = freeCoPaidText;
                    this.dtFreeCoPainTime.EditValue = HeinUtils.ConvertDateStringToSystemDate(freeCoPaidText);
                }

                this.chkPaid6Month.Checked = ado.IsPaid6Month;

                Inventec.Common.Logging.LogSystem.Info(
                    "FillCoPaidMcctToForm____"
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => ado), ado));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                this.isFillingHeinDataFromDb = previousFillingFlag;
                this.IsAutoCheck = previousAutoCheckFlag;
            }
        }

        /// <summary>
        /// Builds the confirmation text: gateway value against the value currently on the form,
        /// followed by the cut-off timestamp so the user can judge how stale the figures are.
        /// </summary>
        private string BuildCoPaidMcctConfirmMessage(CoPaidMcctADO ado)
        {
            string result = "";
            try
            {
                string currentAmountStr = new string((this.txtCoPaidAccumulate.Text ?? "").Where(Char.IsDigit).ToArray());
                long currentAmount = Inventec.Common.TypeConvert.Parse.ToInt64(currentAmountStr);

                StringBuilder builder = new StringBuilder();
                builder.AppendLine(String.Format(
                    ResourceMessage.CungChiTraLuyKeTrenCongBHXH,
                    ado.CoPaidAccumulateAmount.ToString("#,##0"),
                    currentAmount.ToString("#,##0")));

                if (ado.HasThreshold)
                {
                    builder.AppendLine(String.Format(
                        ResourceMessage.DaCungChiTra06ThangLuongCoSo,
                        ado.IsPaid6Month ? ResourceMessage.Co : ResourceMessage.Khong));

                    string freeCoPaidText = ado.FreeCoPaidTime.HasValue
                        ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(ado.FreeCoPaidTime.Value)
                        : ResourceMessage.KhongXacDinh;
                    string currentFreeCoPaidText = String.IsNullOrEmpty(this.txtFreeCoPainTime.Text.Trim())
                        ? ResourceMessage.DangDeTrong
                        : this.txtFreeCoPainTime.Text.Trim();

                    builder.AppendLine(String.Format(
                        ResourceMessage.ThoiDiemMienCungChiTraTrenCong, freeCoPaidText, currentFreeCoPaidText));
                }

                if (!String.IsNullOrEmpty(ado.DataNote))
                {
                    builder.AppendLine();
                    builder.AppendLine(ado.DataNote);
                }

                builder.AppendLine();
                builder.Append(ResourceMessage.BanCoMuonLayThongTinTuCongBHXHKhong);
                result = builder.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = ResourceMessage.BanCoMuonLayThongTinTuCongBHXHKhong;
            }
            return result;
        }

        /// <summary>
        /// Manual lookup button on the accumulated co-payment field. The host owns the patient
        /// context, so the UC only raises the request and waits for the answer to come back
        /// through SetCoPaidAccumulateFromGov.
        /// </summary>
        private void txtCoPaidAccumulate_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (this.dlgCheckTienMCCT != null)
                {
                    this.dlgCheckTienMCCT();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Hides the manual lookup button when the host did not supply a handler.
        /// Called once the delegates have been wired.
        /// </summary>
        private void InitCoPaidMcctButton()
        {
            try
            {
                if (this.txtCoPaidAccumulate.Properties.Buttons.Count > 0)
                {
                    this.txtCoPaidAccumulate.Properties.Buttons[0].Visible = this.dlgCheckTienMCCT != null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

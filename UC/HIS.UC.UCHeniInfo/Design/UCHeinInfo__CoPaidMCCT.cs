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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.Library.CheckHeinGOV;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using HIS.UC.UCHeniInfo.ADO;
using HIS.UC.UCHeniInfo.Utils;
using Inventec.Common.Adapter;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HIS.UC.UCHeniInfo
{
    public partial class UCHeinInfo
    {
        /// <summary>Số tháng lương cơ sở tạo thành ngưỡng miễn cùng chi trả.</summary>
        const int MCCT_BASE_SALARY_MONTHS = 6;

        /// <summary>
        /// Cache danh sách tham số BHYT (HIS_BHYT_PARAM) — load 1 lần qua API.
        /// </summary>
        private static List<MOS.EFMODEL.DataModels.HIS_BHYT_PARAM> listBhytParamMcct;

        /// <summary>
        /// Lấy bản ghi HIS_BHYT_PARAM đang hiệu lực (TO_TIME == null).
        /// </summary>
        private MOS.EFMODEL.DataModels.HIS_BHYT_PARAM GetCurrentBhytParam()
        {
            try
            {
                if (listBhytParamMcct == null)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisBhytParamFilter filter = new MOS.Filter.HisBhytParamFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    listBhytParamMcct = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_BHYT_PARAM>>(
                        "api/HisBhytParam/Get", ApiConsumers.MosConsumer, filter, param);
                }

                if (listBhytParamMcct == null)
                    return null;

                // Phải chặn cả FROM_TIME: viện có thể khai sẵn bản ghi lương cơ sở của đợt
                // sắp tới mà TO_TIME vẫn null. Thiếu điều kiện này sẽ lấy nhầm mức lương
                // chưa tới hiệu lực, làm ngưỡng 06 tháng cao hơn thực tế.
                long nowNumber = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                MOS.EFMODEL.DataModels.HIS_BHYT_PARAM bhytParam = listBhytParamMcct
                    .Where(o => o.TO_TIME == null && (o.FROM_TIME ?? 0) <= nowNumber)
                    .OrderByDescending(o => o.FROM_TIME)
                    .FirstOrDefault();

                if (bhytParam == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn(
                        "GetCurrentBhytParam: khong co ban ghi HIS_BHYT_PARAM nao dang hieu luc."
                        + Inventec.Common.Logging.LogUtil.TraceData(
                            Inventec.Common.Logging.LogUtil.GetMemberName(() => listBhytParamMcct), listBhytParamMcct));
                }

                return bhytParam;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Áp kết quả tra cứu cùng chi trả từ cổng BHXH lên 3 trường liên quan:
        /// số lũy kế, cờ đủ 06 tháng lương cơ sở và thời điểm miễn cùng chi trả.
        ///
        /// Hỏi người dùng trước khi ghi đè bất kỳ giá trị nào đang khác trên form,
        /// nên số đã nhập tay theo giấy chứng nhận không bị thay thế âm thầm.
        /// </summary>
        public void SetCoPaidAccumulateFromGov(ResultMCCTADO resultMcct)
        {
            try
            {
                if (resultMcct == null)
                {
                    return;
                }

                if (!resultMcct.HasData)
                {
                    // Mã 204 nghĩa là "không có bản ghi", không phải "bằng 0"
                    // => giữ nguyên mọi giá trị đang có trên form.
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
                    // Cảnh báo ngay lúc điền, thay vì để người dùng bị chặn ở nút Lưu.
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
        /// Suy ra 3 giá trị từ dữ liệu cổng trả về.
        ///
        /// tBNCCTLuyKe đã là số cộng dồn — nó mang theo mọi đợt KCB trước đó trong năm —
        /// nên kết quả là giá trị lớn nhất trong danh sách, tuyệt đối không phải tổng.
        /// Dùng Max còn chống được việc cổng đổi thứ tự sắp xếp hoặc trả ngayRa rỗng.
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
                    // Cổng cam kết sắp xếp giảm dần theo ngayRa; nếu bản ghi đầu không phải
                    // số lớn nhất thì cam kết đó đã đổi, cần rà lại cách suy TDMC CT bên dưới.
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
                    // Không có lương cơ sở thì không kết luận được cờ 6 tháng lẫn ngày miễn;
                    // riêng số tiền lũy kế vẫn đáng điền.
                    Inventec.Common.Logging.LogSystem.Warn(
                        "CalculateCoPaidMcct: khong lay duoc luong co so tu HIS_BHYT_PARAM, chi cap nhat so tien luy ke.");
                    return ado;
                }

                decimal limit = bhytParam.BASE_SALARY * MCCT_BASE_SALARY_MONTHS;
                ado.HasThreshold = true;
                ado.IsPaid6Month = ado.CoPaidAccumulateAmount > limit;

                // Miễn cùng chi trả bắt đầu khi đợt KCB đầu tiên vượt ngưỡng kết thúc.
                // Duyệt 1 lượt giữ ngày ra viện nhỏ nhất, thay vì sắp xếp lại danh sách.
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
        /// True khi ít nhất một trong ba trường trên form khác kết quả cổng trả về.
        /// Chỉ khi đó mới hỏi người dùng xác nhận.
        ///
        /// Khi cấu hình IsNotAutoCheck5Y6M đang bật, cờ 6 tháng không được chương trình
        /// tự tick nên cũng không đưa vào phép so sánh.
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

                if (!HisConfigCFG.IsNotAutoCheck5Y6M && this.chkPaid6Month.Checked != ado.IsPaid6Month)
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
        /// Ghi các giá trị suy ra lên form.
        ///
        /// Thứ tự quan trọng: thời điểm miễn cùng chi trả phải vào trước cờ 6 tháng,
        /// nếu không hàm kiểm tra sẽ chặn lưu khi ô ngày còn trống.
        ///
        /// Toàn khối chạy sau hai cờ chặn: gán txtFreeCoPainTime.Text sẽ kích hoạt
        /// txtDTMCChiTra_TextChanged, làm chương trình tự tính lại hai checkbox và có thể
        /// bung câu hỏi về giấy chứng nhận không cùng chi trả.
        ///
        /// Cờ 6 tháng chỉ được đụng tới khi cấu hình IsNotAutoCheck5Y6M đang tắt —
        /// giữ đúng hành vi hiện tại của màn hình tiếp đón.
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
                    // Không có lương cơ sở: số tiền thì tin được, còn kết luận thì không.
                    return;
                }

                if (ado.FreeCoPaidTime.HasValue)
                {
                    string freeCoPaidText = Inventec.Common.DateTime.Convert
                        .TimeNumberToDateString(ado.FreeCoPaidTime.Value);
                    this.txtFreeCoPainTime.Text = freeCoPaidText;
                    this.dtFreeCoPainTime.EditValue = HeinUtils.ConvertDateStringToSystemDate(freeCoPaidText);
                }

                if (!HisConfigCFG.IsNotAutoCheck5Y6M)
                {
                    this.chkPaid6Month.Checked = ado.IsPaid6Month;
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info(
                        "FillCoPaidMcctToForm: cau hinh IsNotAutoCheck5Y6M dang bat, khong tu tick chkPaid6Month.");
                }

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
        /// Dựng nội dung hộp thoại xác nhận: giá trị cổng đặt cạnh giá trị đang có trên form,
        /// kèm mốc thời gian dữ liệu để người dùng biết số liệu cũ tới đâu.
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
        /// Kiểm tra số tiền cùng chi trả lũy kế: nếu vượt 06 tháng lương cơ sở
        /// (BASE_SALARY x 6) mà chưa nhập TDMC CT thì cảnh báo, chặn lưu.
        /// </summary>
        public bool ValidateCoPaidAccumulate()
        {
            bool valid = true;
            try
            {
                // Chỉ kiểm tra khi user có nhập số tiền lũy kế
                string coPaidAccumulateStr = new string((this.txtCoPaidAccumulate.Text ?? "").Where(Char.IsDigit).ToArray());
                if (String.IsNullOrEmpty(coPaidAccumulateStr))
                    return true;

                long coPaidAccumulate = Inventec.Common.TypeConvert.Parse.ToInt64(coPaidAccumulateStr);

                MOS.EFMODEL.DataModels.HIS_BHYT_PARAM bhytParam = this.GetCurrentBhytParam();
                if (bhytParam == null || bhytParam.BASE_SALARY <= 0)
                    return true;

                decimal limit = bhytParam.BASE_SALARY * MCCT_BASE_SALARY_MONTHS;

                // Vượt 06 tháng lương cơ sở và chưa nhập TDMC CT
                if (coPaidAccumulate > limit
                    && String.IsNullOrEmpty(this.txtFreeCoPainTime.Text.Trim()))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        ResourceMessage.SoTienLuyKeCungChiTraVuot06ThangLuongCoSo,
                        MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    if (this.txtFreeCoPainTime.Enabled)
                    {
                        this.txtFreeCoPainTime.Focus();
                        this.txtFreeCoPainTime.SelectAll();
                    }
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        /// <summary>
        /// Nút tra cứu thủ công trên ô cùng chi trả lũy kế. Ngữ cảnh bệnh nhân nằm ở host,
        /// nên UC chỉ phát yêu cầu và chờ kết quả quay lại qua SetCoPaidAccumulateFromGov.
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
        /// Gán handler tra cứu thủ công. Không gán thì nút tra cứu tự ẩn.
        /// </summary>
        public void SetDelegateCheckTienMCCT(DelegateCheckTienMCCT _dlgCheckTienMCCT)
        {
            try
            {
                this.dlgCheckTienMCCT = _dlgCheckTienMCCT;
                if (this.txtCoPaidAccumulate.Properties.Buttons.Count > 0)
                {
                    this.txtCoPaidAccumulate.Properties.Buttons[0].Visible = _dlgCheckTienMCCT != null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

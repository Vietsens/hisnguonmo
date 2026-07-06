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
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ImportBlood
{
    public partial class UCImportBloodPlus
    {
        // Cấu trúc chuỗi QR túi máu (sau Base64 decode), tách theo ký tự "|":
        // [0] Mã vạch | [1] Nhóm máu Rh | [2] (rỗng) | [3] TG đóng gói | [4] Hạn sử dụng | [5] Mã loại máu | [6] Tên loại máu | [7] Điều kiện bảo quản

        /// <summary>
        /// Xử lý khi người dùng quét mã QR túi máu và nhấn Enter.
        /// </summary>
        private void txtQrBloodBag_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.ProcessQrBloodBag(txtQrBloodBag.Text);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Base64 decode chuỗi QR, kiểm tra hợp lệ, đổ dữ liệu vào các trường và tự chọn loại máu theo mã loại máu.
        /// </summary>
        private void ProcessQrBloodBag(string qrInput)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(qrInput))
                    return;

                // 1. Base64 decode. Thất bại => dữ liệu không hợp lệ.
                string decoded = null;
                try
                {
                    decoded = Encoding.UTF8.GetString(Convert.FromBase64String(qrInput.Trim()));
                }
                catch (FormatException)
                {
                    decoded = null;
                }

                // 2. Decode thất bại hoặc không chứa ký tự "|" => thông báo và dừng.
                if (String.IsNullOrEmpty(decoded) || !decoded.Contains("|"))
                {
                    ShowQrInvalidMessage();
                    return;
                }

                // 3. Tách dữ liệu theo cấu trúc.
                string[] parts = decoded.Split('|');

                string bloodCode = parts.Length > 0 ? (parts[0] ?? "").Trim() : "";
                string bloodGroupRh = parts.Length > 1 ? (parts[1] ?? "").Trim() : "";
                string packingTimeStr = parts.Length > 3 ? (parts[3] ?? "").Trim() : "";
                string expiredDateStr = parts.Length > 4 ? (parts[4] ?? "").Trim() : "";
                string bloodTypeCode = parts.Length > 5 ? (parts[5] ?? "").Trim() : "";

                // 4. Tìm và chọn loại máu theo mã loại máu.
                WaitingManager.Show();
                bool found = SelectBloodTypeByCode(bloodTypeCode);

                // 5. Đổ dữ liệu vào các trường thông tin (sau khi chọn loại máu vì bước chọn reset control).
                FillControlByQrBloodBag(bloodCode, bloodGroupRh, packingTimeStr, expiredDateStr);

                if (found)
                {
                    txtBloodCode.Focus();
                    txtBloodCode.SelectAll();
                }
                else
                {
                    txtQrBloodBag.Focus();
                    txtQrBloodBag.SelectAll();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Tìm loại máu theo mã loại máu. Có => tự chọn, enable nút Thêm. Không có => disable nút Thêm.
        /// </summary>
        private bool SelectBloodTypeByCode(string bloodTypeCode)
        {
            try
            {
                V_HIS_BLOOD_TYPE bloodType = null;
                if (!String.IsNullOrEmpty(bloodTypeCode))
                {
                    bloodType = BackendDataWorker.Get<V_HIS_BLOOD_TYPE>()
                        .FirstOrDefault(o => o.IS_LEAF == 1 && o.IS_ACTIVE == 1
                            && o.BLOOD_TYPE_CODE != null
                            && o.BLOOD_TYPE_CODE.ToLower() == bloodTypeCode.ToLower());
                }

                if (bloodType != null)
                {
                    this.bloodTypeADO = new UC.BloodType.ADO.BloodTypeADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<UC.BloodType.ADO.BloodTypeADO>(this.bloodTypeADO, bloodType);
                    // Chọn loại máu: set currentBlood, enable nút Thêm, reset các control chi tiết.
                    this.ProcessChoiceBloodTypeADO(this.bloodTypeADO);
                    return true;
                }
                else
                {
                    // Không có loại máu tương ứng => bỏ chọn và disable nút Thêm.
                    this.currentBlood = null;
                    this.SetEnableButtonAdd(true);
                    this.SetControlValueByBloodType(true);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// Đổ dữ liệu từ QR vào các control: mã vạch, nhóm máu, Rh, thời gian đóng gói, hạn sử dụng.
        /// </summary>
        private void FillControlByQrBloodBag(string bloodCode, string bloodGroupRh, string packingTimeStr, string expiredDateStr)
        {
            try
            {
                // Nhóm máu + Rh: định dạng "<Nhóm máu> <Rh>" (VD: "O +").
                string aboCode = bloodGroupRh;
                string rhCode = "";
                if (!String.IsNullOrEmpty(bloodGroupRh))
                {
                    string[] tokens = bloodGroupRh.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length >= 2)
                    {
                        aboCode = tokens[0];
                        rhCode = tokens[tokens.Length - 1];
                    }
                    else if (bloodGroupRh.EndsWith("+") || bloodGroupRh.EndsWith("-"))
                    {
                        rhCode = bloodGroupRh.Substring(bloodGroupRh.Length - 1, 1);
                        aboCode = bloodGroupRh.Substring(0, bloodGroupRh.Length - 1).Trim();
                    }
                    else
                    {
                        aboCode = bloodGroupRh;
                    }
                }

                if (!String.IsNullOrEmpty(aboCode))
                {
                    var bloodAbo = BackendDataWorker.Get<HIS_BLOOD_ABO>()
                        .FirstOrDefault(o => o.BLOOD_ABO_CODE != null && o.BLOOD_ABO_CODE.ToLower() == aboCode.ToLower());
                    if (bloodAbo != null)
                        cboBloodAbo.EditValue = bloodAbo.ID;
                }

                if (!String.IsNullOrEmpty(rhCode))
                {
                    var bloodRh = BackendDataWorker.Get<HIS_BLOOD_RH>()
                        .FirstOrDefault(o => o.BLOOD_RH_CODE != null && o.BLOOD_RH_CODE.ToLower() == rhCode.ToLower());
                    if (bloodRh != null)
                        cboBloodRh.EditValue = bloodRh.ID;
                }

                // Thời gian đóng gói (set trước để không bị auto tính lại đè lên hạn sử dụng).
                DateTime? packingTime = Helpers.DateTimeUtil.ParseDateOrDateTime(packingTimeStr);
                if (packingTime.HasValue && packingTime.Value != DateTime.MinValue)
                {
                    dtPackingTime.EditValue = packingTime.Value;
                }

                // Hạn sử dụng (set sau cùng để ưu tiên giá trị theo QR).
                DateTime? expiredDate = Helpers.DateTimeUtil.ParseDateOrDateTime(expiredDateStr);
                if (expiredDate.HasValue && expiredDate.Value != DateTime.MinValue)
                {
                    dtExpiredDate.EditValue = expiredDate.Value;
                }

                // Mã vạch.
                txtBloodCode.Text = bloodCode;

                RemoveControlDxError1();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowQrInvalidMessage()
        {
            try
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    Base.ResourceMessageLang.DuLieuQrKhongHopLe,
                    Base.ResourceMessageLang.TieuDeCuaSoThongBaoLaThongBao,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtQrBloodBag.Focus();
                txtQrBloodBag.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

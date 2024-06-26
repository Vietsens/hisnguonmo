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
using HIS.UC.UCHeniInfo.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.UCHeniInfo.CustomValidateRule
{
    /// <summary>
    /// Tích cả 5 năm 6 tháng thì bắt buộc nhập ô TDMC chi trả, 
    /// chỉ tích 5 năm hoặc 6 tháng thì không cho nhập, 
    /// chọn TDMC chi trả trước mà không tích đủ 2 ô 5 năm 6 tháng thì lưu cảnh báo, không cho lưu ( vẫn cho phép tích trước hoặc chọn TDMC chi trả trước đều được)
    /// </summary>
    class TemplateHeinBHYT1__FreeCoPainTime__ValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.ButtonEdit txtFreeCoPainTime;
        internal DevExpress.XtraEditors.CheckEdit chkJoin5Year;
        internal DevExpress.XtraEditors.CheckEdit chkPaid6Month;
        public override bool Validate(Control control, object value)
        {
            bool valid = true;
            try
            {
                valid = valid && (txtFreeCoPainTime != null && chkJoin5Year != null && chkPaid6Month != null);
                if (valid)
                {
                    string inputDate = txtFreeCoPainTime.Text.Trim();
                    if (chkJoin5Year.Checked && chkPaid6Month.Checked && String.IsNullOrEmpty(inputDate))
                    {
                        this.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
                        return false;
                    }

                    if (!String.IsNullOrEmpty(inputDate))
                    {
                        if (inputDate.Length == 8)
                        {
                            inputDate = inputDate.Substring(0, 2) + "/" + inputDate.Substring(2, 2) + "/" + inputDate.Substring(4, 4);
                        }

                        var dateFreeCoPainTime = HeinUtils.ConvertDateStringToSystemDate(inputDate);
                        if (dateFreeCoPainTime == null || dateFreeCoPainTime.Value == DateTime.MinValue)
                        {
                            this.ErrorText = ResourceMessage.NguoiDungNhapNgayKhongHopLe;
                            return false;
                        }

                        //thời điểm miễn cùng chi trả phải cùng năm với năm hiện tại                    
                        if (dateFreeCoPainTime.Value.Year != DateTime.Now.Year)
                        {
                            this.ErrorText = ResourceMessage.ThoiDiemMienCungChiTraPhaiCungNamVoiNamHienTai;
                            return false;
                        }

                        if (!chkJoin5Year.Checked || !chkPaid6Month.Checked)
                        {
                            this.ErrorText = ResourceMessage.PhaiDatDu5Nam6ThangMoiCoTheChonDTMCCT;
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
    }
}

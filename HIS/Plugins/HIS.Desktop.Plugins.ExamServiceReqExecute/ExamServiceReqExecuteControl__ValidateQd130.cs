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
using HIS.Desktop.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    /// <summary>
    /// Bat buoc nhap thong tin kham benh ngoai tru theo QD130.
    /// Chi kich hoat khi key HIS.DESKTOP.EXAM.REQUIRED_FIELDS_QD130 = 1
    /// VA ho so co loai dieu tri Kham.
    ///
    /// Pham vi: CHI ba truong chua co bat ky co che bat buoc nao trong phan mem —
    /// Tien su benh, Kham toan than, Kham bo phan.
    /// Cac truong khac (Ly do kham, Qua trinh benh ly, Chan doan chinh,
    /// Dau hieu sinh ton, Phuong phap dieu tri, Tinh trang ket thuc dieu tri)
    /// deu da co key cau hinh hoac rang buoc san — KHONG dung toi.
    /// </summary>
    public partial class ExamServiceReqExecuteControl : UserControlBase
    {
        private bool isRequiredFieldsQd130;

        private void LoadQd130Config()
        {
            try
            {
                isRequiredFieldsQd130 = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKeys.REQUIRED_FIELDS_QD130) == "1";
            }
            catch (Exception ex)
            {
                isRequiredFieldsQd130 = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool IsApplyQd130()
        {
            try
            {
                return isRequiredFieldsQd130
                    && this.treatment != null
                    && this.treatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>
        /// To do nhan cua ba truong thuoc pham vi QD130.
        /// Nhan cac truong khac giu nguyen - co che san co tu lo.
        /// </summary>
        private void ApplyQd130Appearance()
        {
            try
            {
                if (!IsApplyQd130()) return;

                SetQd130CaptionRequired(lblCaptionPathologicalHistory);
                SetQd130CaptionRequired(lciKhamToanThan);
                SetQd130CaptionRequired(lblInformationExam);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetQd130CaptionRequired(DevExpress.XtraLayout.LayoutControlItem item)
        {
            try
            {
                if (item == null) return;
                item.AppearanceItemCaption.ForeColor = Color.Maroon;
                item.AppearanceItemCaption.Options.UseForeColor = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Thieu thi gom vao MOT thong bao, dua con tro ve o thieu dau tien va dung thao tac.
        /// </summary>
        private bool CheckQd130RequiredFields()
        {
            try
            {
                if (!IsApplyQd130()) return true;

                List<string> missing = new List<string>();
                Action focusFirst = null;

                AddQd130Missing(missing, ref focusFirst, "Tiền sử bệnh",
                    IsQd130Empty(txtPathologicalHistory), () => FocusQd130Edit(txtPathologicalHistory));

                AddQd130Missing(missing, ref focusFirst, "Khám toàn thân",
                    IsQd130Empty(txtKhamToanThan), () => FocusQd130Edit(txtKhamToanThan));

                AddQd130Missing(missing, ref focusFirst, "Khám bộ phận",
                    IsQd130Empty(txtKhamBoPhan), () => FocusQd130Edit(txtKhamBoPhan));

                if (missing.Count == 0) return true;

                XtraMessageBox.Show(
                    "Hồ sơ chưa đầy đủ thông tin bắt buộc: " + String.Join(", ", missing)
                        + ". Vui lòng bổ sung đầy đủ thông tin trước khi tiếp tục.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                if (focusFirst != null) focusFirst();
                return false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return true;
            }
        }

        private void AddQd130Missing(List<string> missing, ref Action focusFirst, string fieldName, bool isEmpty, Action focus)
        {
            if (!isEmpty) return;
            missing.Add(fieldName);
            if (focusFirst == null) focusFirst = focus;
        }

        private bool IsQd130Empty(BaseEdit control)
        {
            try
            {
                if (control == null) return false;
                return String.IsNullOrWhiteSpace(control.Text);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        private void FocusQd130Edit(BaseEdit control)
        {
            try
            {
                if (control == null) return;
                control.Focus();
                control.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

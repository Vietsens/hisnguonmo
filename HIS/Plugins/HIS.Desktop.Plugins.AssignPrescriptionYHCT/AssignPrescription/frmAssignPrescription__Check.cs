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
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Config;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        /// <summary>
        /// Áp dụng cấu hình IS_TRACKING_REQUIRED = 4 (RequiredSoftForMedicine) — chỉ cho điều trị
        /// nội trú hoặc cấp cứu. Khi điều kiện thoả mãn:
        ///   - Đánh dấu caption cboPhieuDieuTri màu Maroon (KHÔNG set Required cứng).
        ///   - Nếu bệnh nhân chưa có tờ điều trị nào → cảnh báo XtraMessageBox, KHÔNG chặn form.
        /// Validation thực sự (chặn lưu khi có thuốc) thực hiện ở CheckTrackingRequiredOption4().
        /// Gọi từ LoadDataTracking sau khi xử lý xong dữ liệu tờ điều trị.
        /// </summary>
        private void ApplyTrackingRequiredOption4()
        {
            try
            {
                if (HisConfigCFG.TrackingRequiredOption != (int)EnumAssignPrescription.TRACKING_REQUIRED_OPTION.RequiredSoftForMedicine)
                    return;

                if (this.Histreatment == null) return;

                bool isNoiTru = this.Histreatment.IN_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU;
                bool isCapCuu = this.Histreatment.IS_EMERGENCY == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                if (!isNoiTru && !isCapCuu) return;

                // Maroon — chỉ đánh dấu trường quan trọng, KHÔNG validate cứng
                this.lciPhieuDieuTri.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;

                // Cảnh báo nếu chưa có tờ điều trị nào
                if (this.trackingADOs == null || this.trackingADOs.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        Resources.ResourceMessage.BenhNhanChuaCoToDieuTri_KeDonVTMaKhongKeThuoc,
                        Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(
                            Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Validate cho cấu hình IS_TRACKING_REQUIRED = 4 (RequiredSoftForMedicine):
        ///   - Chỉ áp dụng khi điều trị nội trú HOẶC cấp cứu.
        ///   - Nếu cboPhieuDieuTri đã chọn → cho lưu bình thường.
        ///   - Nếu chưa chọn + đơn có ít nhất 1 thuốc (SERVICE_TYPE_ID == ID__THUOC) → chặn lưu + cảnh báo, focus combo.
        ///   - Nếu chưa chọn + đơn CHỈ có vật tư → cho lưu bình thường.
        /// Gọi trong ProcessSaveData chain trước khi gọi API.
        /// </summary>
        private bool CheckTrackingRequiredOption4()
        {
            try
            {
                if (HisConfigCFG.TrackingRequiredOption != (int)EnumAssignPrescription.TRACKING_REQUIRED_OPTION.RequiredSoftForMedicine)
                    return true;

                if (this.Histreatment == null) return true;

                bool isNoiTru = this.Histreatment.IN_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU;
                bool isCapCuu = this.Histreatment.IS_EMERGENCY == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                if (!isNoiTru && !isCapCuu) return true;

                if (cboPhieuDieuTri.EditValue != null) return true;

                List<MediMatyTypeADO> items = this.mediMatyTypeADOs;
                if (items == null || items.Count == 0) return true;

                bool hasMedicine = items.Any(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC);
                if (!hasMedicine) return true;

                DevExpress.XtraEditors.XtraMessageBox.Show(
                    Resources.ResourceMessage.KhongChoPhepKeDonCoThuocKhiChuaChonToDieuTri,
                    Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(
                        Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                cboPhieuDieuTri.Focus();
                return false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                // Fail-safe: không chặn save khi check method lỗi
                return true;
            }
        }
    }
}

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
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentFinish
{
    /// <summary>
    /// Tính năng "Mở phiếu, Vỏ bệnh án (VBA)":
    /// - Checkbox ChkMoPhieuVoBenhAn do người dùng tự tích, trạng thái lưu qua ControlState.
    /// - Sau khi lưu kết thúc điều trị thành công → mở các mẫu phiếu được khai báo
    ///   "Mở khi kết thúc điều trị" trong danh mục Phiếu vỏ bệnh án
    ///   (HIS_EMR_FORM.IS_OPEN_WHEN_TREATMENT_FINISH = 1).
    /// - Phần mềm EMR tự tách chuỗi mã phiếu theo dấu phẩy và mở lần lượt từng phiếu.
    /// - Hồ sơ chưa có vỏ bệnh án (EMR_COVER_TYPE_ID null) vẫn mở phiếu bình thường: không
    ///   cảnh báo, không hiển thị danh mục vỏ và không tự tạo vỏ.
    /// - Không có mẫu phiếu nào được tích thì không mở gì.
    /// </summary>
    public partial class FormTreatmentFinish
    {
        /// <summary>Lưu trạng thái checkbox khi người dùng thay đổi.</summary>
        private void ChkMoPhieuVoBenhAn_CheckedChanged(object sender, EventArgs e)
        {
            if (isNotLoadWhileChangeControlStateInFirst) return;
            try
            {
                SaveMoPhieuVoBenhAnState();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SaveMoPhieuVoBenhAnState()
        {
            try
            {
                if (this.controlStateWorker == null)
                    this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                if (this.currentControlStateRDO == null)
                    this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();

                string key = ChkMoPhieuVoBenhAn.Name;
                string value = ChkMoPhieuVoBenhAn.Checked ? "1" : "";

                var item = this.currentControlStateRDO.FirstOrDefault(
                    o => o.KEY == key && o.MODULE_LINK == moduleLink);
                if (item != null)
                {
                    item.VALUE = value;
                }
                else
                {
                    this.currentControlStateRDO.Add(new HIS.Desktop.Library.CacheClient.ControlStateRDO
                    {
                        KEY = key,
                        MODULE_LINK = moduleLink,
                        VALUE = value
                    });
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Mở vỏ bệnh án và các mẫu phiếu sau khi lưu kết thúc điều trị thành công.
        /// Gọi sau khi api/HisTreatment/Finish trả về kết quả khác null.
        /// </summary>
        private void OpenEmrAfterFinishIfNeeded()
        {
            try
            {
                if (ChkMoPhieuVoBenhAn == null || !ChkMoPhieuVoBenhAn.Checked)
                    return;

                HIS_TREATMENT treatment = this.hisTreatmentResult ?? this.currentHisTreatment;
                if (treatment == null)
                    return;

                //Khong canh bao, khong tao vo benh an: chi mo cac mau phieu duoc tich
                //"Mo khi ket thuc dieu tri" cho nguoi dung tu nhap lieu va tu tao phieu.
                OpenEmrCoverWithForms(treatment);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Mo vo benh an da luu cua nguoi benh, kem chuoi ma mau phieu duoc tich
        /// "Mo khi ket thuc dieu tri". Chuoi rong thi EMR chi mo vo benh an.
        /// </summary>
        private void OpenEmrCoverWithForms(HIS_TREATMENT treatment)
        {
            try
            {
                if (treatment == null)
                    return;

                string emrFormCodes = GetEmrFormCodesOpenWhenTreatmentFinish();
                if (String.IsNullOrWhiteSpace(emrFormCodes))
                {
                    LogSystem.Debug("OpenEmrCoverWithForms: khong co mau phieu nao duoc tich Mo khi ket thuc dieu tri.");
                    return;
                }

                HIS.Desktop.Plugins.Library.FormMedicalRecord.Base.EmrInputADO emrInputAdo
                    = new HIS.Desktop.Plugins.Library.FormMedicalRecord.Base.EmrInputADO();
                emrInputAdo.TreatmentId = treatment.ID;
                emrInputAdo.PatientId = treatment.PATIENT_ID;
                emrInputAdo.EmrCoverTypeId = treatment.EMR_COVER_TYPE_ID;
                emrInputAdo.TreatmentTypeId = treatment.TDL_TREATMENT_TYPE_ID;
                emrInputAdo.roomId = this.module != null ? (long?)this.module.RoomId : null;

                //Ho so chua co vo benh an thi truyen 0: thu vien tu goi LoadDataEmr voi loai vo = 0,
                //van mo duoc cac mau phieu theo chuoi ma truyen vao.
                long emrCoverTypeId = treatment.EMR_COVER_TYPE_ID ?? 0;

                LogSystem.Debug("OpenEmrCoverWithForms. EmrCoverTypeId: "
                    + emrCoverTypeId + ", MaPhieu: " + emrFormCodes);

                HIS.Desktop.Plugins.Library.FormMedicalRecord.MediRecordMenuPopupProcessor processor
                    = new HIS.Desktop.Plugins.Library.FormMedicalRecord.MediRecordMenuPopupProcessor();
                processor.FormOpenEmr(emrCoverTypeId, emrInputAdo, emrFormCodes);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Lay danh sach ma mau phieu duoc tich "Mo khi ket thuc dieu tri" trong danh muc
        /// Phieu vo benh an, noi lai thanh chuoi ngan cach boi dau phay.
        /// Khong co mau phieu nao thi tra ve chuoi rong.
        /// </summary>
        private string GetEmrFormCodesOpenWhenTreatmentFinish()
        {
            string result = "";
            try
            {
                var forms = BackendDataWorker.Get<HIS_EMR_FORM>();
                if (forms == null)
                    return result;

                var codes = forms
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                             && o.IS_OPEN_WHEN_TREATMENT_FINISH == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                             && !String.IsNullOrWhiteSpace(o.EMR_FORM_CODE))
                    .Select(o => o.EMR_FORM_CODE.Trim())
                    .ToList();

                if (codes != null && codes.Count > 0)
                    result = String.Join(",", codes);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                result = "";
            }
            return result;
        }
    }
}

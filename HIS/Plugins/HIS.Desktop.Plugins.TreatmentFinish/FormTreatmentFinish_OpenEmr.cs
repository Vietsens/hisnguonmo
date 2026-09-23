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
    /// - Sau khi lưu kết thúc điều trị thành công → mở vỏ bệnh án đã lưu của người bệnh,
    ///   kèm danh sách mã mẫu phiếu được khai báo "Mở khi kết thúc điều trị" trong danh mục
    ///   Phiếu vỏ bệnh án (HIS_EMR_FORM.IS_OPEN_WHEN_TREATMENT_FINISH = 1).
    /// - Phần mềm EMR tự tách chuỗi mã phiếu theo dấu phẩy và mở lần lượt từng phiếu.
    /// - Nếu hồ sơ chưa có vỏ bệnh án (EMR_COVER_TYPE_ID null) → cảnh báo rồi hiển thị danh
    ///   mục vỏ để người dùng tự chọn, KHÔNG tự suy ra loại vỏ theo phòng/khoa.
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

                //Ho so chua co vo benh an: canh bao roi hien danh muc vo de nguoi dung tu chon
                if (treatment.EMR_COVER_TYPE_ID == null || treatment.EMR_COVER_TYPE_ID <= 0)
                {
                    MessageBox.Show(
                        ResourceMessage.BenhNhanChuaDuocTaoVoBenhAnCanTao,
                        "",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    LoadEmrCoverConfigForChoose(treatment);
                    VoBenhAn(treatment);
                    return;
                }

                OpenEmrCoverWithForms(treatment);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Nap thiet lap vo benh an theo phong dang lam viec, khong co thi theo khoa, de thu
        /// hep danh muc vo cho nguoi dung chon. Chi dung cho truong hop ho so chua co vo.
        /// </summary>
        private void LoadEmrCoverConfigForChoose(HIS_TREATMENT treatment)
        {
            try
            {
                LstEmrCoverConfig = null;
                LstEmrCoverConfigDepartment = null;
                if (treatment == null || this.module == null)
                    return;

                var allConfigs = BackendDataWorker.Get<HIS_EMR_COVER_CONFIG>();
                if (allConfigs == null)
                    return;

                LstEmrCoverConfig = allConfigs
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                             && o.ROOM_ID == this.module.RoomId
                             && o.TREATMENT_TYPE_ID == treatment.TDL_TREATMENT_TYPE_ID)
                    .ToList();

                if (LstEmrCoverConfig != null && LstEmrCoverConfig.Count > 0)
                    return;

                var workPlace = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO
                    .FirstOrDefault(o => o.RoomId == this.module.RoomId);
                if (workPlace == null)
                    return;

                LstEmrCoverConfigDepartment = allConfigs
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                             && o.DEPARTMENT_ID == workPlace.DepartmentId
                             && o.TREATMENT_TYPE_ID == treatment.TDL_TREATMENT_TYPE_ID)
                    .ToList();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
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
                if (treatment == null || treatment.EMR_COVER_TYPE_ID == null)
                    return;

                HIS.Desktop.Plugins.Library.FormMedicalRecord.Base.EmrInputADO emrInputAdo
                    = new HIS.Desktop.Plugins.Library.FormMedicalRecord.Base.EmrInputADO();
                emrInputAdo.TreatmentId = treatment.ID;
                emrInputAdo.PatientId = treatment.PATIENT_ID;
                emrInputAdo.EmrCoverTypeId = treatment.EMR_COVER_TYPE_ID;
                emrInputAdo.TreatmentTypeId = treatment.TDL_TREATMENT_TYPE_ID;
                emrInputAdo.roomId = this.module != null ? (long?)this.module.RoomId : null;

                string emrFormCodes = GetEmrFormCodesOpenWhenTreatmentFinish();

                LogSystem.Debug("OpenEmrCoverWithForms. EmrCoverTypeId: "
                    + treatment.EMR_COVER_TYPE_ID + ", MaPhieu: " + emrFormCodes);

                HIS.Desktop.Plugins.Library.FormMedicalRecord.MediRecordMenuPopupProcessor processor
                    = new HIS.Desktop.Plugins.Library.FormMedicalRecord.MediRecordMenuPopupProcessor();
                processor.FormOpenEmr(treatment.EMR_COVER_TYPE_ID.Value, emrInputAdo, emrFormCodes);
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

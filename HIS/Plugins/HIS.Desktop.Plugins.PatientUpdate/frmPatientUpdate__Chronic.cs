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
using HIS.Desktop.Plugins.PatientUpdate.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.PatientUpdate
{
    /// <summary>
    /// Việc 58919 - khi bật MOS.HIS_TREATMENT.FINISH.CHRONIC_CHANGE_TREATMENT_TYPE_OPTION, ô "BN mãn tính"
    /// hiển thị và ghi theo cờ của hồ sơ điều trị đích (không theo cờ bệnh nhân).
    /// Hồ sơ đích chọn đúng như backend (api/HisPatient/UpdateSdo):
    /// - mở kèm hồ sơ (TreatmentId): hồ sơ đó;
    /// - mở từ bệnh nhân + tích "Sửa HSĐT mới nhất": hồ sơ có thời gian vào mới nhất, và hồ sơ này
    ///   phải chưa khoá / chưa tạm khoá viện phí (nếu không backend rơi sang đợt cũ hơn và từ chối).
    /// Hồ sơ đã khoá viện phí, tạm khoá, duyệt khoá BHYT hoặc không xác định được thì khoá ô.
    /// </summary>
    public partial class frmPatientUpdate
    {
        HIS_TREATMENT chronicTargetTreatment = null;
        bool isChronicTargetEditable = false;
        bool isChronicInitialized = false;

        private void LoadChronicByTreatment()
        {
            try
            {
                if (!Config.IsChronicChangeTreatmentType)
                {
                    return;
                }

                this.isChronicInitialized = true;
                this.chronicTargetTreatment = null;
                this.isChronicTargetEditable = false;
                string reason = null;

                if (this.TreatmentId > 0)
                {
                    HisTreatmentFilter filter = new HisTreatmentFilter();
                    filter.ID = this.TreatmentId;
                    List<HIS_TREATMENT> treatments = new BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, null);
                    this.chronicTargetTreatment = treatments != null ? treatments.FirstOrDefault() : null;
                }
                else if (this.currentPatient != null && chkUpdateNew.Checked)
                {
                    HisTreatmentFilter filter = new HisTreatmentFilter();
                    filter.PATIENT_ID = this.PatientId;
                    List<HIS_TREATMENT> treatments = new BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, null);
                    if (treatments != null && treatments.Count > 0)
                    {
                        treatments = treatments.OrderByDescending(o => o.IN_TIME).ToList();
                        HIS_TREATMENT latest = treatments[0];
                        HIS_TREATMENT firstOpen = treatments.FirstOrDefault(o => o.IS_TEMPORARY_LOCK != 1 && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                        if (firstOpen != null && firstOpen.ID == latest.ID)
                        {
                            this.chronicTargetTreatment = latest;
                        }
                        else
                        {
                            reason = "Hồ sơ điều trị mới nhất đã khoá (tạm khoá) viện phí, không cập nhật được cờ mãn tính.";
                        }
                    }
                }

                if (this.chronicTargetTreatment == null)
                {
                    if (reason == null)
                    {
                        reason = "Không xác định được hồ sơ điều trị để cập nhật cờ mãn tính (cần mở từ hồ sơ điều trị hoặc tích \"Sửa HSĐT mới nhất\").";
                    }
                }
                else if (this.chronicTargetTreatment.IS_ACTIVE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE || this.chronicTargetTreatment.IS_TEMPORARY_LOCK == 1)
                {
                    reason = "Hồ sơ điều trị đã khoá (tạm khoá) viện phí, không cập nhật được cờ mãn tính.";
                }
                else if (this.chronicTargetTreatment.IS_LOCK_HEIN == 1)
                {
                    reason = "Hồ sơ điều trị đã duyệt khoá BHYT, không cập nhật được cờ mãn tính.";
                }
                else
                {
                    this.isChronicTargetEditable = true;
                }

                chkBNManTinh.Checked = this.chronicTargetTreatment != null && this.chronicTargetTreatment.IS_CHRONIC == 1;
                chkBNManTinh.Enabled = this.isChronicTargetEditable;
                chkBNManTinh.ToolTip = this.isChronicTargetEditable
                    ? "Mãn tính của đợt điều trị " + this.chronicTargetTreatment.TREATMENT_CODE
                    : reason;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Chỉ gửi TreatmentIsChronic khi người dùng đổi ô và lần lưu có mang đúng hồ sơ đích.
        /// Không gửi thì backend giữ nguyên cờ hồ sơ - tránh làm hỏng cả lần lưu ở các trường hợp
        /// hồ sơ khoá BHYT / người dùng chọn "chỉ cập nhật thông tin bệnh nhân".
        /// </summary>
        private void ApplyTreatmentChronic(HisPatientUpdateChronicSDO sdo)
        {
            try
            {
                if (!Config.IsChronicChangeTreatmentType || sdo == null
                    || this.chronicTargetTreatment == null || !this.isChronicTargetEditable)
                {
                    return;
                }

                bool hasTargetTreatment = (sdo.TreatmentId.HasValue && sdo.TreatmentId.Value == this.chronicTargetTreatment.ID)
                    || (this.currentPatient != null && sdo.UpdateTreatment);
                if (!hasTargetTreatment)
                {
                    return;
                }

                bool oldValue = this.chronicTargetTreatment.IS_CHRONIC == 1;
                if (chkBNManTinh.Checked != oldValue)
                {
                    sdo.TreatmentIsChronic = chkBNManTinh.Checked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

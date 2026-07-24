/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Tab trẻ em dưới 6 tuổi — thông tin người đi cùng trẻ (mục Hành chính):
 * - CCCD người đi cùng  (txtAccompanyCccd8)  ← HIS_PATIENT.RELATIVE_CMND_NUMBER
 * - Số điện thoại liên hệ (txtAccompanyPhone8) ← HIS_PATIENT.RELATIVE_PHONE (ưu tiên) / RELATIVE_MOBILE
 * - Họ tên người đi cùng (txtAccompanyPersonName8): default HIS_PATIENT.RELATIVE_NAME nếu
 *   HIS_KSK_UNDER_SIX.ACCOMPANY_PERSON_NAME chưa có.
 * Khi Lưu tab <6t: nếu CCCD / SĐT khác dữ liệu HIS_PATIENT -> cập nhật HIS_PATIENT qua
 * api/HisPatient/UpdateSdo (giống HIS.Desktop.Plugins.PatientUpdate).
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        // HIS_PATIENT của lượt khám hiện tại (nạp khi mở tab trẻ <6t) — để so sánh khi Lưu.
        private HIS_PATIENT currentPatientUnderSix;

        /// <summary>
        /// Nạp CCCD + SĐT người đi cùng từ HIS_PATIENT; default họ tên người đi cùng nếu KSK chưa có.
        /// Gọi trong EnsureTabLoaded case 7 (sau khi FillTabByIndex đã đổ ACCOMPANY_PERSON_NAME).
        /// </summary>
        private void LoadAccompanyInfoUnderSix()
        {
            try
            {
                if (currentServiceReq == null || currentServiceReq.TDL_PATIENT_ID <= 0) return;
                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientFilter filter = new MOS.Filter.HisPatientFilter();
                filter.ID = currentServiceReq.TDL_PATIENT_ID;
                var patients = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, filter, null);
                currentPatientUnderSix = (patients != null && patients.Count > 0) ? patients[0] : null;
                if (currentPatientUnderSix == null) return;

                txtAccompanyCccd8.Text = currentPatientUnderSix.RELATIVE_CMND_NUMBER;
                txtAccompanyPhone8.Text = !string.IsNullOrWhiteSpace(currentPatientUnderSix.RELATIVE_PHONE)
                    ? currentPatientUnderSix.RELATIVE_PHONE
                    : currentPatientUnderSix.RELATIVE_MOBILE;

                // Default họ tên người đi cùng từ HIS_PATIENT.RELATIVE_NAME khi KSK chưa lưu tên.
                if (string.IsNullOrWhiteSpace(txtAccompanyPersonName8.Text)
                    && !string.IsNullOrWhiteSpace(currentPatientUnderSix.RELATIVE_NAME))
                    txtAccompanyPersonName8.Text = currentPatientUnderSix.RELATIVE_NAME;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Khi Lưu tab &lt;6t: nếu CCCD / SĐT nhập khác dữ liệu HIS_PATIENT thì cập nhật HIS_PATIENT
        /// (RELATIVE_CMND_NUMBER, RELATIVE_PHONE) qua api/HisPatient/UpdateSdo. Gọi sau khi lưu KSK thành công.
        /// </summary>
        private void SaveAccompanyPatientUnderSix()
        {
            try
            {
                if (currentServiceReq == null || currentServiceReq.TDL_PATIENT_ID <= 0) return;
                string newCccd = txtAccompanyCccd8.Text != null ? txtAccompanyCccd8.Text.Trim() : "";
                string newPhone = txtAccompanyPhone8.Text != null ? txtAccompanyPhone8.Text.Trim() : "";

                CommonParam param = new CommonParam();
                // Dùng HIS_PATIENT đã cache lúc mở tab (LoadAccompanyInfoUnderSix) — KHÔNG GET lại cho nhanh.
                // Chỉ GET dự phòng khi chưa có cache.
                HIS_PATIENT pat = currentPatientUnderSix;
                if (pat == null)
                {
                    MOS.Filter.HisPatientFilter filter = new MOS.Filter.HisPatientFilter();
                    filter.ID = currentServiceReq.TDL_PATIENT_ID;
                    var patients = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, filter, null);
                    if (patients == null || patients.Count == 0) return;
                    pat = patients[0];
                }

                string curCccd = pat.RELATIVE_CMND_NUMBER ?? "";
                string curPhone = !string.IsNullOrWhiteSpace(pat.RELATIVE_PHONE) ? pat.RELATIVE_PHONE : (pat.RELATIVE_MOBILE ?? "");
                bool changed = !string.Equals(newCccd, curCccd, StringComparison.Ordinal)
                            || !string.Equals(newPhone, curPhone, StringComparison.Ordinal);
                if (!changed) return;

                pat.RELATIVE_CMND_NUMBER = string.IsNullOrWhiteSpace(newCccd) ? null : newCccd;
                pat.RELATIVE_PHONE = string.IsNullOrWhiteSpace(newPhone) ? null : newPhone;

                MOS.SDO.HisPatientUpdateSDO sdo = new MOS.SDO.HisPatientUpdateSDO();
                sdo.HisPatient = pat;
                var res = new BackendAdapter(param).Post<HIS_PATIENT>("api/HisPatient/UpdateSdo", ApiConsumers.MosConsumer, sdo, param);
                if (res != null) currentPatientUnderSix = res;
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.MIMS.Integration.Models;
using Inventec.Common.Adapter;
using Inventec.Core;

namespace HIS.Desktop.MIMS.Integration.Core
{
    /// <summary>
    /// Đọc/ghi bản ghi HIS_MIMS_PATIENT_PROFILE (trạng thái phụ nữ mang thai / cho con bú)
    /// qua API MOS api/HisMimsPatientProfile/*. Dùng chung cho màn Xử trí khám,
    /// Sửa thông tin bệnh nhân và các form kê đơn.
    /// </summary>
    public static class MimsPatientProfileWorker
    {
        private const string URI_GET = "api/HisMimsPatientProfile/Get";
        private const string URI_CREATE = "api/HisMimsPatientProfile/Create";
        private const string URI_UPDATE = "api/HisMimsPatientProfile/Update";

        /// <summary>
        /// Lấy bản ghi trạng thái active mới nhất của bệnh nhân. Trả null nếu chưa có / lỗi.
        /// </summary>
        public static MimsPatientProfileRecord GetByPatientId(long patientId)
        {
            try
            {
                if (patientId <= 0) return null;
                CommonParam param = new CommonParam();
                MimsPatientProfileFilter filter = new MimsPatientProfileFilter();
                filter.PATIENT_ID = patientId;
                var data = new BackendAdapter(param).Get<List<MimsPatientProfileRecord>>(
                    URI_GET, ApiConsumers.MosConsumer, filter, param);
                if (data == null || data.Count == 0) return null;
                return data
                    .Where(o => o != null && o.IS_DELETE != 1 && o.IS_ACTIVE != 0)
                    .OrderByDescending(o => (o.MODIFY_TIME ?? o.CREATE_TIME ?? 0))
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Lưu bản ghi: ID &lt;= 0 → Create, ngược lại Update. Trả về bản ghi đã lưu hoặc null nếu lỗi.
        /// </summary>
        public static MimsPatientProfileRecord Save(MimsPatientProfileRecord record)
        {
            try
            {
                if (record == null || record.PATIENT_ID <= 0) return null;
                CommonParam param = new CommonParam();
                string uri = record.ID <= 0 ? URI_CREATE : URI_UPDATE;
                var result = new BackendAdapter(param).Post<MimsPatientProfileRecord>(
                    uri, ApiConsumers.MosConsumer, record, param);
                Inventec.Common.Logging.LogSystem.Debug(
                    "MimsPatientProfileWorker.Save - uri=" + uri
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => record), record)
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Build model PatientProfile gửi MIMS từ bản ghi DB + thông tin nhân khẩu.
        /// Trả null khi bản ghi không có cờ nào được tick (điều kiện if/else tối ưu:
        /// null → request MIMS giữ nguyên như hiện tại, không thêm payload).
        /// </summary>
        public static MimsPatientProfile ToRequestProfile(MimsPatientProfileRecord record, string genderCode, int? ageYear)
        {
            try
            {
                if (record == null) return null;
                bool isPregnant = record.IS_PREGNANT == 1;
                bool isNursing = record.IS_LACTATING == 1;
                if (!isPregnant && !isNursing) return null;

                MimsPatientProfile profile = new MimsPatientProfile();
                profile.GenderCode = genderCode;
                profile.AgeYear = ageYear;
                profile.IsPregnant = isPregnant;
                profile.PregnancyMonth = record.PREGNANT_MONTH.HasValue ? (int?)record.PREGNANT_MONTH.Value : null;
                profile.IsNursing = isNursing;
                return profile;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}

using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    /// <summary>
    /// Việc 62129: cảnh báo chuyển khám phòng khám chuyên đề khi bác sĩ nhập ICD.
    /// Thiết lập nằm trên phòng chuyên đề (HIS_EXECUTE_ROOM.ADD_EXAM_*), chỉ thông báo, không chặn.
    /// </summary>
    public partial class ExamServiceReqExecuteControl
    {
        const short ADD_EXAM_OPTION_MAIN = 1;
        const short ADD_EXAM_OPTION_SUB = 2;

        /// <summary>Mã không cần kiểm tra nữa: có sẵn khi mở hồ sơ hoặc đã kiểm tra khớp thiết lập. Null = chưa nạp xong hồ sơ.</summary>
        HashSet<string> addExamKnownIcdCodes;
        /// <summary>Phòng chuyên đề có thiết lập, lấy mới 1 lần/lượt mở màn hình (lấy khi cần lần đầu).</summary>
        List<V_HIS_EXECUTE_ROOM> addExamSpecialRooms;

        /// <summary>Ghi nhận các mã đang có trên màn hình là mã có sẵn (sau khi nạp hồ sơ / nạp lại chẩn đoán).</summary>
        private void AddExamWarningMarkCurrentIcdKnown()
        {
            try
            {
                if (addExamKnownIcdCodes == null)
                    addExamKnownIcdCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (string code in GetAddExamIcdCodes(txtIcdCode.Text).Concat(GetAddExamIcdCodes(txtIcdSubCode.Text)))
                    addExamKnownIcdCodes.Add(code);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Kiểm tra mã mới nhập ở chẩn đoán chính / kèm theo, đủ điều kiện thì hiện 1 thông báo.</summary>
        private void CheckAddExamWarning()
        {
            try
            {
                if (addExamKnownIcdCodes == null || this.moduleData == null || this.HisServiceReqView == null)
                    return;

                List<string> newMainCodes = GetAddExamIcdCodes(txtIcdCode.Text).Where(o => !addExamKnownIcdCodes.Contains(o)).ToList();
                List<string> newSubCodes = GetAddExamIcdCodes(txtIcdSubCode.Text).Where(o => !addExamKnownIcdCodes.Contains(o) && !newMainCodes.Contains(o, StringComparer.OrdinalIgnoreCase)).ToList();
                if (newMainCodes.Count == 0 && newSubCodes.Count == 0)
                    return;

                if (addExamSpecialRooms == null)
                    addExamSpecialRooms = LoadAddExamSpecialRooms();
                if (addExamSpecialRooms.Count == 0)
                    return;

                long currentRoomId = this.moduleData.RoomId;
                DateTime now = DateTime.Now;
                long today = (long)now.DayOfWeek + 1; // 1 = Chủ nhật
                int nowHHmm = now.Hour * 100 + now.Minute;

                // mã ICD -> các phòng chuyên đề khớp 4 điều kiện thiết lập
                Dictionary<string, List<V_HIS_EXECUTE_ROOM>> matches = new Dictionary<string, List<V_HIS_EXECUTE_ROOM>>(StringComparer.OrdinalIgnoreCase);
                foreach (V_HIS_EXECUTE_ROOM room in addExamSpecialRooms)
                {
                    if (room.ROOM_ID == currentRoomId)
                        continue;
                    if (!SplitAddExamValues(room.ADD_EXAM_WARNING_ROOM_IDS, ',').Contains(currentRoomId.ToString()))
                        continue;
                    if (!IsInAddExamTime(room.ADD_EXAM_TIMES, today, nowHHmm))
                        continue;
                    List<string> roomIcds = SplitAddExamValues(room.ADD_EXAM_ICD_CODES, ';');
                    List<string> codes = new List<string>();
                    if (room.ADD_EXAM_ICD_OPTION != ADD_EXAM_OPTION_SUB)
                        codes.AddRange(newMainCodes);
                    if (room.ADD_EXAM_ICD_OPTION != ADD_EXAM_OPTION_MAIN)
                        codes.AddRange(newSubCodes);
                    foreach (string code in codes.Where(o => roomIcds.Contains(o, StringComparer.OrdinalIgnoreCase)))
                    {
                        if (!matches.ContainsKey(code))
                            matches[code] = new List<V_HIS_EXECUTE_ROOM>();
                        matches[code].Add(room);
                    }
                }
                if (matches.Count == 0)
                    return;

                // Đã kiểm tra khớp thiết lập thì không nhắc lại trong lượt khám (kể cả khi bỏ qua vì đã có chỉ định khám).
                foreach (string code in matches.Keys)
                    addExamKnownIcdCodes.Add(code);

                // Bệnh nhân đã có chỉ định khám tại phòng chuyên đề trong đợt điều trị thì không báo phòng đó.
                List<long> examinedRoomIds = GetAddExamExaminedRoomIds();
                List<string> lines = new List<string>();
                foreach (var match in matches)
                {
                    List<string> roomNames = match.Value.Where(o => !examinedRoomIds.Contains(o.ROOM_ID)).Select(o => o.EXECUTE_ROOM_NAME).Distinct().ToList();
                    if (roomNames.Count == 0)
                        continue;
                    HIS_ICD icd = BackendDataWorker.Get<HIS_ICD>().FirstOrDefault(o => String.Equals(o.ICD_CODE, match.Key, StringComparison.OrdinalIgnoreCase));
                    lines.Add(String.Format("Chẩn đoán {0} – {1} thuộc danh mục của {2}.",
                        match.Key, icd != null ? icd.ICD_NAME : "", String.Join(", ", roomNames)));
                }
                if (lines.Count == 0)
                    return;

                lines.Add("Đề nghị bác sĩ xem xét chuyển khám.");
                XtraMessageBox.Show(String.Join("\r\n", lines), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<V_HIS_EXECUTE_ROOM> LoadAddExamSpecialRooms()
        {
            List<V_HIS_EXECUTE_ROOM> result = null;
            try
            {
                HisExecuteRoomViewFilter filter = new HisExecuteRoomViewFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                result = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXECUTE_ROOM>>("api/HisExecuteRoom/GetView", ApiConsumers.MosConsumer, filter, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return (result ?? new List<V_HIS_EXECUTE_ROOM>())
                .Where(o => !String.IsNullOrWhiteSpace(o.ADD_EXAM_ICD_CODES) && !String.IsNullOrWhiteSpace(o.ADD_EXAM_WARNING_ROOM_IDS) && !String.IsNullOrWhiteSpace(o.ADD_EXAM_TIMES))
                .ToList();
        }

        private List<long> GetAddExamExaminedRoomIds()
        {
            List<long> result = new List<long>();
            try
            {
                HisServiceReqFilter filter = new HisServiceReqFilter();
                filter.TREATMENT_ID = this.HisServiceReqView.TREATMENT_ID;
                filter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                List<HIS_SERVICE_REQ> serviceReqs = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, null);
                if (serviceReqs != null)
                    result = serviceReqs.Where(o => o.IS_DELETE != 1 && o.IS_NO_EXECUTE != 1).Select(o => o.EXECUTE_ROOM_ID).Distinct().ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>ADD_EXAM_TIMES dạng "thứ|HHmm|HHmm;..." (1 = Chủ nhật).</summary>
        private static bool IsInAddExamTime(string addExamTimes, long today, int nowHHmm)
        {
            foreach (string part in SplitAddExamValues(addExamTimes, ';'))
            {
                string[] items = part.Split('|');
                long day;
                int from, to;
                if (items.Length == 3 && long.TryParse(items[0], out day) && int.TryParse(items[1], out from) && int.TryParse(items[2], out to)
                    && day == today && from <= nowHHmm && nowHHmm < to)
                    return true;
            }
            return false;
        }

        private static List<string> GetAddExamIcdCodes(string text)
        {
            return SplitAddExamValues(text, ';');
        }

        private static List<string> SplitAddExamValues(string value, char separator)
        {
            if (String.IsNullOrWhiteSpace(value))
                return new List<string>();
            return value.Split(separator).Select(o => o.Trim()).Where(o => o.Length > 0).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }
    }
}

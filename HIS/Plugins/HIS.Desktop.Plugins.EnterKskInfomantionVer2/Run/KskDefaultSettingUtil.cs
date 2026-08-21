/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Đọc/ghi thiết lập "mặc định khám lâm sàng (dưới 6 tuổi)" vào ControlState (lưu theo MÁY).
 * Dùng chung cho frmAutoClsSetting (nơi cấu hình) và frmEnterKskInfomantionVer2 (nơi áp dụng)
 * nên tách ra util riêng, tránh 2 chỗ tự parse mỗi kiểu.
 *
 * Khuôn ControlStateWorker lấy từ frmAutoClsSetting.cs / ___UnderEighteenVaccineDefault.cs.
 *
 * ĐỊNH DẠNG lưu: "rdoStrabismus8=0:1;rdoSuckingReflex8=1:0"
 *  - Mỗi cặp: <tên RadioGroup> = <giá trị> : <1 nếu ô "Dùng" đang tích, 0 nếu không>.
 *    Dòng KHÔNG tích vẫn được lưu đầy đủ (tạm tắt, không phải xóa) — chỉ không đem áp vào form.
 *  - Thiếu phần ":x" (thiết lập lưu theo format cũ) → hiểu là ĐANG DÙNG, tương thích ngược.
 *  - CHỈ ghi tên RadioGroup + số, KHÔNG ghi caption tiếng Việt: ControlStateWorker nối câu SQL
 *    update bằng string (xem ControlStateWorker.cs) nên một dấu nháy đơn trong VALUE là hỏng lệnh.
 *  - Không lưu tên nhóm (GROUP_NAME): suy lại được từ catalog, và đổi tên nhóm trong Designer
 *    thì thiết lập đã lưu vẫn dùng được.
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using HIS.Desktop.Library.CacheClient;
using HIS.Desktop.Plugins.EnterKskInfomantionVer2.ADO;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    /// <summary>Tiện ích lưu/đọc mặc định khám lâm sàng trẻ dưới 6 tuổi theo máy.</summary>
    public static class KskDefaultSettingUtil
    {
        public const string MODULE_LINK = "HIS.Desktop.Plugins.EnterKskInfomantionVer2";

        /// <summary>Key chứa danh sách cặp "tên RadioGroup = giá trị mặc định".</summary>
        public const string KEY_ROWS = "UnderSixClinicalDefault";

        /// <summary>Key cờ "tự động điền khi mở bản ghi mới" ("1"/"0").</summary>
        public const string KEY_AUTO_APPLY = "UnderSixClinicalDefaultAuto";

        private const char PAIR_SEPARATOR = ';';
        private const char PAIR_ASSIGN = '=';

        /// <summary>Ngăn cách giữa giá trị và cờ "Dùng": "rdoStrabismus8=0:1".</summary>
        private const char USED_SEPARATOR = ':';

        /// <summary>Ký tự nối trong VALUE_KEY của <see cref="ADO.KskDefaultValueADO"/>: "rdoX8|1".</summary>
        public const char VALUE_KEY_SEPARATOR = '|';

        /// <summary>Ghép khóa cho ô "Giá trị mặc định" của lưới thiết lập.</summary>
        public static string BuildValueKey(string fieldName, long value)
        {
            return fieldName + VALUE_KEY_SEPARATOR + value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Tách số ra khỏi VALUE_KEY ("rdoStrabismus8|0" → 0). Trả null nếu khóa rỗng/sai định dạng.
        /// </summary>
        public static long? ParseValueFromKey(string valueKey)
        {
            try
            {
                if (string.IsNullOrEmpty(valueKey)) return null;
                int idx = valueKey.LastIndexOf(VALUE_KEY_SEPARATOR);
                if (idx < 0 || idx >= valueKey.Length - 1) return null;
                long value;
                if (long.TryParse(valueKey.Substring(idx + 1), NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
                    return value;
                return null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }

        /// <summary>
        /// Đọc thiết lập đã lưu ở máy này. <paramref name="rows"/> luôn khác null (rỗng nếu chưa cấu hình);
        /// mỗi dòng có FIELD_NAME + VALUE_KEY + IS_USED, GROUP_NAME để null (người gọi suy từ catalog).
        /// Trả về CẢ dòng không tích "Dùng" để lưới hiện lại đúng những gì đã lưu.
        /// </summary>
        public static void Load(out List<KskDefaultRowADO> rows, out bool autoApply)
        {
            rows = new List<KskDefaultRowADO>();
            autoApply = false;
            try
            {
                ControlStateWorker worker = new ControlStateWorker();
                List<ControlStateRDO> states = worker.GetData(MODULE_LINK) ?? new List<ControlStateRDO>();

                rows = ParseRaw(GetStateValue(states, KEY_ROWS));
                autoApply = GetStateValue(states, KEY_AUTO_APPLY) == "1";
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Bóc chuỗi lưu thành danh sách dòng. Tách riêng khỏi <see cref="Load"/> để test được
        /// định dạng mà không phải chạm vào ControlState thật. Cặp sai định dạng bị bỏ qua chứ
        /// không làm vỡ cả thiết lập.
        /// </summary>
        internal static List<KskDefaultRowADO> ParseRaw(string raw)
        {
            var rows = new List<KskDefaultRowADO>();
            if (string.IsNullOrEmpty(raw)) return rows;

            var seen = new Dictionary<string, KskDefaultRowADO>();
            foreach (string pair in raw.Split(new[] { PAIR_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries))
            {
                int idx = pair.IndexOf(PAIR_ASSIGN);
                if (idx <= 0 || idx >= pair.Length - 1) continue;
                string field = pair.Substring(0, idx).Trim();
                if (string.IsNullOrEmpty(field)) continue;

                // Phần sau '=' là "<giá trị>" hoặc "<giá trị>:<cờ dùng>".
                string tail = pair.Substring(idx + 1).Trim();
                bool isUsed = true;                                    // format cũ không có cờ → coi như đang dùng
                int u = tail.IndexOf(USED_SEPARATOR);
                if (u >= 0)
                {
                    isUsed = tail.Substring(u + 1).Trim() != "0";
                    tail = tail.Substring(0, u).Trim();
                }

                long value;
                if (!long.TryParse(tail, NumberStyles.Integer, CultureInfo.InvariantCulture, out value)) continue;

                // Trùng field thì cặp sau thắng, nhưng giữ nguyên vị trí dòng đã có.
                if (seen.ContainsKey(field))
                {
                    KskDefaultRowADO old = seen[field];
                    old.VALUE_KEY = BuildValueKey(field, value);
                    old.IS_USED = isUsed;
                    continue;
                }

                var row = new KskDefaultRowADO()
                {
                    FIELD_NAME = field,
                    VALUE_KEY = BuildValueKey(field, value),
                    IS_USED = isUsed
                };
                seen.Add(field, row);
                rows.Add(row);
            }
            return rows;
        }

        /// <summary>
        /// Ghi thiết lập xuống ControlState của máy này. Lưu MỌI dòng đã chọn đủ Nội dung + Giá trị,
        /// kể cả dòng không tích "Dùng" (cờ dùng ghi kèm) — bỏ tích không làm mất cấu hình.
        /// </summary>
        public static void Save(List<KskDefaultRowADO> rows, bool autoApply)
        {
            try
            {
                ControlStateWorker worker = new ControlStateWorker();
                List<ControlStateRDO> states = worker.GetData(MODULE_LINK) ?? new List<ControlStateRDO>();

                SetStateValue(states, KEY_ROWS, BuildRawValue(rows));
                SetStateValue(states, KEY_AUTO_APPLY, autoApply ? "1" : "0");

                worker.SetData(states);
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        /// <summary>Ghép danh sách dòng thành chuỗi lưu. Bỏ tên field lạ ký tự để chuỗi luôn an toàn với câu SQL nối tay.</summary>
        internal static string BuildRawValue(List<KskDefaultRowADO> rows)
        {
            StringBuilder sb = new StringBuilder();
            if (rows == null) return "";
            var written = new HashSet<string>();
            foreach (var row in rows)
            {
                if (row == null || string.IsNullOrEmpty(row.FIELD_NAME)) continue;
                if (!IsSafeFieldName(row.FIELD_NAME)) continue;
                long? value = ParseValueFromKey(row.VALUE_KEY);
                if (value == null) continue;                                   // chưa chọn giá trị → chưa có gì để lưu
                if (!written.Add(row.FIELD_NAME)) continue;                     // khai 1 field 2 lần → giữ dòng đầu
                if (sb.Length > 0) sb.Append(PAIR_SEPARATOR);
                sb.Append(row.FIELD_NAME)
                  .Append(PAIR_ASSIGN).Append(value.Value.ToString(CultureInfo.InvariantCulture))
                  .Append(USED_SEPARATOR).Append(row.IS_USED ? "1" : "0");
            }
            return sb.ToString();
        }

        /// <summary>Tên control hợp lệ = chữ/số/gạch dưới. Chặn nháy đơn và dấu phân tách lọt vào VALUE.</summary>
        private static bool IsSafeFieldName(string name)
        {
            return name.All(c => char.IsLetterOrDigit(c) || c == '_');
        }

        #region ===== Xuất / nhập JSON (cóp thiết lập CẢ 2 TAB sang máy khác) =====

        /// <summary>Nhãn nhận dạng file, chặn nhập nhầm file JSON của chức năng khác.</summary>
        public const string JSON_TYPE = "ENTER_KSK_VER2_SETTING";

        /// <summary>
        /// TYPE của bản đầu (chỉ có phần mặc định KSK, chưa có phần Tự động lấy CLS).
        /// Vẫn nhận để file đã xuất trước đó không thành rác.
        /// </summary>
        public const string JSON_TYPE_LEGACY = "KSK_UNDER_SIX_DEFAULT";

        /// <summary>Phiên bản cấu trúc file — 1 = chỉ ROWS, 2 = thêm AUTO_CLS.</summary>
        public const int JSON_VERSION = 2;

        /// <summary>
        /// Dựng JSON để xuất cả 2 tab. Phần mặc định ghi kèm caption (Mục / Nội dung / Giá trị) và
        /// phần CLS ghi kèm mã + tên dịch vụ cho người đọc hiểu được, nhưng khi NHẬP chỉ dùng
        /// FIELD_NAME + VALUE (mặc định) và ID/CODE (dịch vụ) — caption có thể đổi giữa các bản build.
        /// </summary>
        public static string BuildJson(KskSettingFileADO data,
                                      List<KskDefaultGroupADO> groups,
                                      List<KskDefaultFieldADO> fields,
                                      List<KskDefaultValueADO> values)
        {
            if (data == null) data = new KskSettingFileADO();

            var arrRows = new Newtonsoft.Json.Linq.JArray();
            if (data.ROWS != null)
            {
                foreach (var row in data.ROWS)
                {
                    if (row == null || string.IsNullOrEmpty(row.FIELD_NAME)) continue;
                    long? value = ParseValueFromKey(row.VALUE_KEY);
                    if (value == null) continue;

                    var field = fields == null ? null : fields.FirstOrDefault(o => o.FIELD_NAME == row.FIELD_NAME);
                    var val = values == null ? null : values.FirstOrDefault(o => o.VALUE_KEY == row.VALUE_KEY);
                    var group = (field == null || groups == null)
                        ? null : groups.FirstOrDefault(o => o.GROUP_NAME == field.GROUP_NAME);

                    var item = new Newtonsoft.Json.Linq.JObject();
                    item["IS_USED"] = row.IS_USED;
                    item["FIELD_NAME"] = row.FIELD_NAME;
                    item["VALUE"] = value.Value;
                    item["GROUP_CAPTION"] = group == null ? "" : group.GROUP_CAPTION;
                    item["FIELD_CAPTION"] = field == null ? "" : field.FIELD_CAPTION;
                    item["VALUE_CAPTION"] = val == null ? "" : val.VALUE_CAPTION;
                    arrRows.Add(item);
                }
            }

            var autoCls = new Newtonsoft.Json.Linq.JObject();
            autoCls["BLOOD"] = BuildServiceArray(data.AUTO_CLS_BLOOD);
            autoCls["URINE"] = BuildServiceArray(data.AUTO_CLS_URINE);
            autoCls["DIIM"] = BuildServiceArray(data.AUTO_CLS_DIIM);

            var root = new Newtonsoft.Json.Linq.JObject();
            root["TYPE"] = JSON_TYPE;
            root["VERSION"] = JSON_VERSION;
            root["EXPORT_TIME"] = DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            root["AUTO_APPLY"] = data.AUTO_APPLY;
            root["ROWS"] = arrRows;
            root["AUTO_CLS"] = autoCls;
            return root.ToString(Newtonsoft.Json.Formatting.Indented);
        }

        private static Newtonsoft.Json.Linq.JArray BuildServiceArray(List<KskServiceRefADO> services)
        {
            var arr = new Newtonsoft.Json.Linq.JArray();
            if (services == null) return arr;
            foreach (var sv in services)
            {
                if (sv == null) continue;
                var item = new Newtonsoft.Json.Linq.JObject();
                item["ID"] = sv.ID;
                item["CODE"] = sv.CODE ?? "";
                item["NAME"] = sv.NAME ?? "";
                arr.Add(item);
            }
            return arr;
        }

        /// <summary>
        /// Đọc file JSON đã xuất. Trả false + <paramref name="error"/> khi file không phải JSON,
        /// sai loại, hoặc rỗng cả 2 phần. KHÔNG đối chiếu catalog / danh mục dịch vụ ở đây —
        /// việc đó do form làm để còn báo được số dòng bị bỏ.
        /// </summary>
        public static bool TryParseJson(string json, out KskSettingFileADO data, out string error)
        {
            data = new KskSettingFileADO();
            error = null;
            try
            {
                if (string.IsNullOrWhiteSpace(json)) { error = "File rỗng."; return false; }

                Newtonsoft.Json.Linq.JObject root;
                try { root = Newtonsoft.Json.Linq.JObject.Parse(json); }
                catch (Exception exJson)
                {
                    LogSystem.Warn(exJson);
                    error = "File không phải JSON hợp lệ.";
                    return false;
                }

                string type = root["TYPE"] == null ? "" : root["TYPE"].ToString();
                if (type != JSON_TYPE && type != JSON_TYPE_LEGACY)
                {
                    error = "File không phải thiết lập của chức năng nhập KSK (TYPE = \"" + type + "\").";
                    return false;
                }

                if (root["AUTO_APPLY"] != null)
                {
                    bool b;
                    if (bool.TryParse(root["AUTO_APPLY"].ToString(), out b)) data.AUTO_APPLY = b;
                }

                var arr = root["ROWS"] as Newtonsoft.Json.Linq.JArray;
                if (arr != null)
                {
                    foreach (var tok in arr)
                    {
                        var item = tok as Newtonsoft.Json.Linq.JObject;
                        if (item == null) continue;

                        string field = item["FIELD_NAME"] == null ? null : item["FIELD_NAME"].ToString();
                        if (string.IsNullOrWhiteSpace(field)) continue;
                        field = field.Trim();
                        if (!IsSafeFieldName(field)) continue;

                        long value;
                        if (item["VALUE"] == null) continue;
                        if (!long.TryParse(item["VALUE"].ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out value)) continue;

                        bool isUsed = true;
                        if (item["IS_USED"] != null)
                        {
                            bool u;
                            if (bool.TryParse(item["IS_USED"].ToString(), out u)) isUsed = u;
                        }

                        data.ROWS.Add(new KskDefaultRowADO()
                        {
                            FIELD_NAME = field,
                            VALUE_KEY = BuildValueKey(field, value),
                            IS_USED = isUsed
                        });
                    }
                }

                var cls = root["AUTO_CLS"] as Newtonsoft.Json.Linq.JObject;
                if (cls != null)
                {
                    data.AUTO_CLS_BLOOD = ParseServiceArray(cls["BLOOD"] as Newtonsoft.Json.Linq.JArray);
                    data.AUTO_CLS_URINE = ParseServiceArray(cls["URINE"] as Newtonsoft.Json.Linq.JArray);
                    data.AUTO_CLS_DIIM = ParseServiceArray(cls["DIIM"] as Newtonsoft.Json.Linq.JArray);
                }

                if (data.ROWS.Count == 0 && !data.HasAutoCls)
                {
                    error = "File không có thiết lập nào đọc được.";
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                error = "Không đọc được file: " + ex.Message;
                return false;
            }
        }

        private static List<KskServiceRefADO> ParseServiceArray(Newtonsoft.Json.Linq.JArray arr)
        {
            var result = new List<KskServiceRefADO>();
            if (arr == null) return result;
            foreach (var tok in arr)
            {
                var item = tok as Newtonsoft.Json.Linq.JObject;
                if (item == null) continue;

                long id = 0;
                if (item["ID"] != null) long.TryParse(item["ID"].ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out id);
                string code = item["CODE"] == null ? "" : item["CODE"].ToString().Trim();

                // Không có cả ID lẫn CODE thì không cách nào khớp lại được -> bỏ.
                if (id <= 0 && string.IsNullOrEmpty(code)) continue;

                result.Add(new KskServiceRefADO()
                {
                    ID = id,
                    CODE = code,
                    NAME = item["NAME"] == null ? "" : item["NAME"].ToString()
                });
            }
            return result;
        }

        #endregion

        private static string GetStateValue(List<ControlStateRDO> states, string key)
        {
            var item = states.FirstOrDefault(o => o.KEY == key && o.MODULE_LINK == MODULE_LINK);
            return item != null ? item.VALUE : null;
        }

        private static void SetStateValue(List<ControlStateRDO> states, string key, string value)
        {
            var item = states.FirstOrDefault(o => o.KEY == key && o.MODULE_LINK == MODULE_LINK);
            if (item != null) item.VALUE = value;
            else states.Add(new ControlStateRDO() { KEY = key, VALUE = value, MODULE_LINK = MODULE_LINK });
        }
    }
}

/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using His.Ksk.QD2062.Base;
using Inventec.Common.IntegrationError;
using MOS.EFMODEL.DataModels;
using Newtonsoft.Json.Linq;

namespace HIS.Desktop.Plugins.KskSyncList
{
    /// <summary>
    /// Cổng CSDL khám sức khỏe Sở Y tế Cần Thơ (FaCare) — việc 62022. FaCare nhận hồ sơ ĐÃ lên cổng Bộ và ĐÃ ký số,
    /// nên cổng này chạy SAU mọi cổng khác của hồ sơ trong cùng lượt Đồng bộ:
    ///   - Hồ sơ lên cổng Bộ (BYT / HOC qua thư viện, hoặc Kho dữ liệu sức khỏe toàn dân) thành công trong lượt
    ///     -> visit.profile_sync_status = true, trạng thái DA_LIEN_THONG; ngược lại false, CHO_LIEN_THONG.
    ///   - Hồ sơ CHO_LIEN_THONG lên cổng Bộ thành công ở lượt sau -> gọi API xác nhận liên thông (ma_lk).
    ///   - Gửi lại cùng ma_lk thì FaCare TẠO THÊM lượt khám (không ghi đè) -> hồ sơ đã có trên cổng không gửi lại.
    /// Trạng thái lưu ở đoạn "FACARE:" của REGISTRATION_NO, mã yêu cầu ở đoạn "FACARE:" của TRANSACTION_CODE.
    /// </summary>
    internal partial class KskSyncProcessor
    {
        private const string FACARE_LABEL = "FACARE:";
        private const string FACARE_FIELD_SIGNATURE = "electronic_signature";

        /// <summary>Nhãn nguồn lỗi của cổng FaCare (cùng cách gắn nhãn các cổng KSK khác — vCong 62522).</summary>
        private static readonly IntegrationPartner KSK_PARTNER_FACARE = IntegrationPartner.HealthPortal("cổng CSDL KSK Sở Y tế Cần Thơ (FaCare)");

        /// <summary>Có đẩy cổng FaCare hay không — theo ô tích trong bảng Cài đặt.</summary>
        internal bool PushFacare { get; set; }

        /// <summary>Chuỗi cấu hình khóa MOS.HIS_KSK_SYNC.FACARE_CONNECTION_INFO theo chi nhánh (UI truyền vào).</summary>
        internal string FacareConnectionInfo { get; set; }

        /// <summary>Người dùng đã đồng ý gửi lại các hồ sơ đang CHUA_RO (hỏi 1 lần trên UI trước khi chạy).</summary>
        internal bool FacareResendUnknown { get; set; }

        /// <summary>Hồ sơ đã gửi FaCare nhưng chưa rõ kết quả (đoạn FACARE của REGISTRATION_NO = CHUA_RO).</summary>
        internal static List<V_HIS_KSK_SYNC> FindFacareUnknown(IEnumerable<V_HIS_KSK_SYNC> rows)
        {
            var list = new List<V_HIS_KSK_SYNC>();
            try
            {
                if (rows == null) return list;
                foreach (var r in rows)
                {
                    if (r == null) continue;
                    string st = GetFacareSegment(SafeString(GetProp(r, "REGISTRATION_NO")));
                    if (string.Equals(st, KskFacarePushResult.STATE_UNKNOWN, StringComparison.OrdinalIgnoreCase)) list.Add(r);
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return list;
        }

        /// <summary>
        /// Xử lý cổng FaCare cho 1 hồ sơ. linkedThisRun = hồ sơ lên cổng Bộ thành công trong lượt này.
        /// Lỗi ở đây chỉ làm phần FaCare thất bại, không ảnh hưởng kết quả các cổng khác.
        /// </summary>
        private KskFacarePushResult PushFacareOneRow(KskFacarePusher pusher, V_HIS_KSK_SYNC row, Qd1551KskInput input,
            Func<string, string> dataSigner, bool linkedThisRun, string macskcb, long syncTime)
        {
            string storedState = GetFacareSegment(SafeString(GetProp(row, "REGISTRATION_NO")));
            string storedRequestId = GetFacareSegment(SafeString(GetProp(row, "TRANSACTION_CODE")));
            bool edited = ToLong(GetProp(row, "SYNC_RESULT_TYPE")) == RESULT_EDITED;
            string treatmentCode = SafeString(GetProp(row, "TDL_TREATMENT_CODE"));
            string editedNote = edited ? " Cổng không nhận sửa qua API, sửa trực tiếp trên web FaCare." : "";
            try
            {
                // 1. Đã có trên cổng và đã ghi nhận liên thông Bộ: không gọi cổng.
                if (string.Equals(storedState, KskFacarePushResult.STATE_LINKED, StringComparison.OrdinalIgnoreCase))
                {
                    LogFacare(treatmentCode, "da co tren cong (DA_LIEN_THONG) -> khong gui lai");
                    return new KskFacarePushResult
                    {
                        Success = true,
                        State = KskFacarePushResult.STATE_LINKED,
                        Note = "FACARE: Hồ sơ đã có trên cổng FaCare, không gửi lại." + editedNote
                    };
                }

                // 2. Đã có trên cổng, chưa ghi nhận liên thông Bộ: không gửi lại; lên Bộ trong lượt này thì xác nhận.
                if (string.Equals(storedState, KskFacarePushResult.STATE_WAIT_LINK, StringComparison.OrdinalIgnoreCase))
                {
                    if (!linkedThisRun)
                    {
                        LogFacare(treatmentCode, "da co tren cong (CHO_LIEN_THONG), luot nay chua len cong Bo -> khong goi cong");
                        return new KskFacarePushResult
                        {
                            Success = true,
                            State = KskFacarePushResult.STATE_WAIT_LINK,
                            Note = "FACARE: Hồ sơ đã có trên cổng FaCare, chưa ghi nhận liên thông Bộ — không gửi lại." + editedNote
                        };
                    }
                    string maLk = (input != null && input.Treatment != null) ? SafeString(input.Treatment.TREATMENT_CODE) : treatmentCode;
                    string signId = BuildFacareRequestId("SIGN", treatmentCode, syncTime);
                    KskFacarePushResult u = pusher.UpdateStatus(maLk, signId);
                    LogFacare(treatmentCode, "xac nhan lien thong ma_lk=" + maLk + " -> " + (u.Success ? "thanh cong" : u.Message));
                    if (u.Success && edited) u.Note += editedNote;
                    return u;
                }

                // 3. Lần trước chưa rõ kết quả: người dùng không đồng ý gửi lại -> giữ CHUA_RO.
                if (string.Equals(storedState, KskFacarePushResult.STATE_UNKNOWN, StringComparison.OrdinalIgnoreCase)
                    && !this.FacareResendUnknown)
                {
                    return new KskFacarePushResult
                    {
                        Success = false,
                        State = KskFacarePushResult.STATE_UNKNOWN,
                        Message = "FACARE: Chưa gửi lại — lần gửi trước chưa rõ kết quả (mã yêu cầu "
                            + (storedRequestId ?? "trống") + ")."
                    };
                }

                // 4. Chưa có trên cổng: kiểm rồi gửi hồ sơ.
                string cfgError = pusher.ValidateConfig();
                if (cfgError != null) return KskFacarePushResult.NotSent(cfgError);
                if (input == null) return KskFacarePushResult.NotSent("FACARE: Không dựng được dữ liệu hồ sơ.");
                if (dataSigner == null)
                    return KskFacarePushResult.NotSent("FACARE: Không gửi hồ sơ — cổng FaCare chỉ nhận hồ sơ đã ký số. Tích Ký số rồi đồng bộ lại.");

                HIS_PATIENT p = input.Patient;
                if (p == null) return KskFacarePushResult.NotSent("FACARE: Không dựng được thông tin hành chính bệnh nhân.");
                if (string.IsNullOrWhiteSpace(p.CCCD_NUMBER) && string.IsNullOrWhiteSpace(p.PASSPORT_NUMBER))
                    return KskFacarePushResult.NotSent("FACARE: Không gửi hồ sơ — bệnh nhân chưa có số CCCD hoặc số hộ chiếu.");
                if (p.GENDER_ID != 1 && p.GENDER_ID != 2)
                    return KskFacarePushResult.NotSent("FACARE: Không gửi hồ sơ — giới tính phải là Nam hoặc Nữ.");

                string libJson = KskEnvelopeBuilder.Build(new List<Qd1551KskInput> { input }, macskcb, true);
                string formType;
                JObject medicalData = BuildFacareMedicalData(KskHccJsonConverter.ToKdlskJson(libJson), out formType);
                if (medicalData == null || string.IsNullOrEmpty(formType))
                    return KskFacarePushResult.NotSent("FACARE: Không dựng được bản tin hồ sơ.");

                long conclusionTime = ToLong(GetProp(row, "CONCLUSION_TIME"));
                string ageError = CheckFacareAge(formType, p.DOB, conclusionTime, SafeString(GetProp(row, "KSK_TYPE_NAME")));
                if (ageError != null) return KskFacarePushResult.NotSent(ageError);

                // Chữ ký số: ký bản tin XML QĐ 2062 của hồ sơ bằng đúng cách ký đang dùng cho cổng Bộ.
                string xml = FixXmlDeclarationUtf8(KskEnvelopeBuilder.Build(new List<Qd1551KskInput> { input }, macskcb, false));
                if (string.IsNullOrEmpty(xml)) return KskFacarePushResult.NotSent("FACARE: Không dựng được bản tin hồ sơ.");
                string signedXml = dataSigner(xml);
                if (string.IsNullOrEmpty(signedXml))
                    return KskFacarePushResult.NotSent("FACARE: Ký số thất bại (CKS_BENH_VIEN/CKS_NGUOI_KET_LUAN) — không gửi hồ sơ chưa ký.");

                var record = new JObject();
                record["patient"] = BuildFacarePatient(p, medicalData["thong_tin_hanh_chinh"] as JObject);
                record["visit"] = BuildFacareVisit(row, input, formType, linkedThisRun);
                record["medical_data"] = medicalData;
                // Vị trí và dạng giá trị của electronic_signature chốt theo bộ Postman của FaCare (chưa có — câu hỏi 13).
                // Tạm gửi cùng cấp patient/visit: Base64 của bản XML KHAMSUCKHOE đã ký (UTF-8).
                record[FACARE_FIELD_SIGNATURE] = Convert.ToBase64String(new UTF8Encoding(false).GetBytes(signedXml));
                var envelope = new JObject();
                envelope["records"] = new JArray(record);

                string bodyText = envelope.ToString(Newtonsoft.Json.Formatting.None);
                DumpDebug("BanTin_FACARE_json", MaskFacareSignature(envelope));
                byte[] body = new UTF8Encoding(false).GetBytes(bodyText);   // chuyển byte ĐÚNG 1 lần, ký và gửi mảng này

                string requestId = BuildFacareRequestId("IMPORT", treatmentCode, syncTime);
                KskFacarePushResult r = pusher.Import(body, requestId, linkedThisRun);
                LogFacare(treatmentCode, "gui ho so X-Request-Id=" + requestId + ", profile_sync_status=" + linkedThisRun
                    + " -> " + (r.Success ? r.State : (r.State + ": " + r.Message)));
                return r;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                // Hồ sơ đã có trên cổng: lỗi bất ngờ không được làm mất trạng thái đó.
                string msg = "FACARE: " + IntegrationErrorMessage.TagException(ex, KSK_PARTNER_FACARE);
                if (KskFacarePushResult.IsOnPortal(storedState))
                    return new KskFacarePushResult { Success = false, State = storedState, Message = msg };
                return KskFacarePushResult.NotSent(msg);
            }
        }

        private static void LogFacare(string treatmentCode, string text)
        {
            Inventec.Common.Logging.LogSystem.Info("FACARE: ho so treatment_code=" + treatmentCode + " " + text);
        }

        /// <summary>X-Request-Id: &lt;tiền tố&gt;-&lt;mã điều trị&gt;-&lt;yyyyMMddHHmmss lúc bấm Đồng bộ&gt;; chỉ giữ ký tự cổng nhận.</summary>
        private static string BuildFacareRequestId(string prefix, string treatmentCode, long syncTime)
        {
            var sb = new StringBuilder();
            foreach (char c in (treatmentCode ?? ""))
                if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '.' || c == '_' || c == ':' || c == '-')
                    sb.Append(c);
            string id = prefix + "-" + sb.ToString() + "-" + syncTime.ToString(CultureInfo.InvariantCulture);
            return (id.Length <= 128) ? id : id.Substring(0, 128);
        }

        /// <summary>
        /// medical_data = các khối của bản tin JSON QĐ 2062 (bản chữ thường như cổng Kho dữ liệu sức khỏe toàn dân),
        /// giữ nguyên tên trường và giá trị. Bỏ trường trống, khối trống, dòng CLS trống. Không lấy khối chukydonvi.
        /// formType = thẻ type của thong_tin_chung_ve_lan_kham (ChildUnder / Minor / Adult).
        /// </summary>
        private static JObject BuildFacareMedicalData(string kdlskJson, out string formType)
        {
            formType = null;
            if (string.IsNullOrWhiteSpace(kdlskJson)) return null;
            JObject root = JObject.Parse(kdlskJson);
            JToken files = root.SelectToken("khamsuckhoe.thongtinhoso.danhsachhoso.hoso.filehoso");
            if (files == null) return null;

            var md = new JObject();
            foreach (JToken f in (files is JArray ? (IEnumerable<JToken>)files : new[] { files }))
            {
                JObject content = (f is JObject) ? ((JObject)f)["noidungfile"] as JObject : null;
                if (content == null) continue;
                foreach (JProperty block in content.Properties())
                {
                    JToken v = PruneFacareEmpty(block.Value);
                    if (v != null) md[block.Name] = v;
                }
            }
            if (md.Count == 0) return null;
            JToken type = md.SelectToken("thong_tin_chung_ve_lan_kham.type");
            formType = (type != null && type.Type != JTokenType.Null) ? type.ToString().Trim() : null;

            // Chỉ gửi các khối thuộc mẫu khám như thư viện dựng cho cổng Bộ. Bộ dựng envelope của plugin lấp đủ
            // 12 khối (Xuất XML / HCC / Kho) — vd XML8 ở hồ sơ người lớn trùng 3 trường với XML12, mà FaCare báo
            // lỗi cả bản ghi khi trùng tên trường giữa các khối.
            string[] allowed;
            if (formType != null && FACARE_BLOCKS_BY_FORM.TryGetValue(formType, out allowed))
                foreach (string name in md.Properties().Select(x => x.Name).ToList())
                    if (Array.IndexOf(allowed, name) < 0) md.Remove(name);
            return md;
        }

        /// <summary>Khối medical_data theo mẫu (Qd1551KskMapper: MapTre6 / MapDuoi18 / MapTren18 + XML1, XML2, XML11).</summary>
        private static readonly Dictionary<string, string[]> FACARE_BLOCKS_BY_FORM =
            new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            { "ChildUnder", new[] { "thong_tin_hanh_chinh", "thong_tin_chung_ve_lan_kham", "danh_gia_dau_hieu_sinh_ton",
                "danh_gia_dinh_duong", "danh_gia_phat_trien_tinh_than_van_dong", "danh_gia_tiem_chung", "kham_lam_sang",
                "ket_luan_va_tu_van", "tien_su_benh_tat", "kham_can_lam_sang", "ket_luan" } },
            { "Minor", new[] { "thong_tin_hanh_chinh", "thong_tin_chung_ve_lan_kham", "kham_lam_sang", "tien_su_benh_tat",
                "kham_the_luc", "kham_can_lam_sang", "ket_luan" } },
            { "Adult", new[] { "thong_tin_hanh_chinh", "thong_tin_chung_ve_lan_kham", "kham_lam_sang", "tien_su_benh_tat",
                "kham_the_luc", "kham_can_lam_sang", "ket_luan" } },
        };

        /// <summary>Bỏ null, chuỗi trắng, đối tượng/mảng rỗng (đệ quy). Trả null nếu cả nút rỗng.</summary>
        private static JToken PruneFacareEmpty(JToken token)
        {
            if (token == null || token.Type == JTokenType.Null || token.Type == JTokenType.Undefined) return null;
            JObject obj = token as JObject;
            if (obj != null)
            {
                var result = new JObject();
                foreach (JProperty p in obj.Properties())
                {
                    JToken v = PruneFacareEmpty(p.Value);
                    if (v != null) result[p.Name] = v;
                }
                return (result.Count > 0) ? result : null;
            }
            JArray arr = token as JArray;
            if (arr != null)
            {
                var result = new JArray();
                foreach (JToken item in arr)
                {
                    JToken v = PruneFacareEmpty(item);
                    if (v != null) result.Add(v);
                }
                return (result.Count > 0) ? result : null;
            }
            if (token.Type == JTokenType.String && string.IsNullOrWhiteSpace(token.ToString())) return null;
            return token.DeepClone();
        }

        /// <summary>
        /// Khối patient: cùng giá trị khối hành chính (XML1) gửi Bộ Y tế. Ba trường người đi cùng (họ tên, CCCD,
        /// quan hệ với trẻ) FaCare chỉ đọc ở patient (bị loại khỏi khối hành chính) nên chép sang khi có.
        /// </summary>
        private static JObject BuildFacarePatient(HIS_PATIENT p, JObject x1)
        {
            var patient = new JObject();
            AddFacareText(patient, "so_cccd", FirstText(ReadText(x1, "so_cccd"), p.CCCD_NUMBER, p.PASSPORT_NUMBER));
            AddFacareText(patient, "ho_ten", FirstText(ReadText(x1, "ho_ten"), p.VIR_PATIENT_NAME));
            DateTime? dob = TimeNumberToDate(p.DOB);
            if (dob.HasValue) patient["ngay_sinh"] = dob.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            patient["gioi_tinh"] = (p.GENDER_ID == 2) ? 1 : 2;   // FaCare: 1 Nam, 2 Nữ (HIS_GENDER: 2 Nam, 1 Nữ)
            AddFacareText(patient, "dien_thoai", FirstText(ReadText(x1, "dien_thoai"), p.MOBILE, p.PHONE));
            AddFacareText(patient, "dia_chi", FirstText(ReadText(x1, "dia_chi"), p.VIR_ADDRESS));
            AddFacareText(patient, "matinh_cu_tru", FirstText(ReadText(x1, "matinh_cu_tru"), p.PROVINCE_CODE));
            AddFacareText(patient, "maxa_cu_tru", FirstText(ReadText(x1, "maxa_cu_tru"), p.COMMUNE_CODE));
            if (x1 != null)
                foreach (string f in new[] { "ho_ten_nguoi_di_cung", "so_cccd_nguoi_di_cung", "moi_quan_he_voi_tre" })
                {
                    JToken v = x1[f];
                    if (v != null) patient[f] = v.DeepClone();
                }
            return patient;
        }

        private JObject BuildFacareVisit(V_HIS_KSK_SYNC row, Qd1551KskInput input, string formType, bool linked)
        {
            var visit = new JObject();
            visit["form_type"] = formType;
            long inTime = ToLong(GetProp(row, "IN_TIME"));
            if (inTime <= 0 && input != null && input.Treatment != null) inTime = input.Treatment.IN_TIME;
            AddFacareText(visit, "ngay_vao", FormatFacareTime(inTime));
            AddFacareText(visit, "thoi_gian_hoan_thanh", FormatFacareTime(ToLong(GetProp(row, "CONCLUSION_TIME"))));
            AddFacareText(visit, "doctor_name", SafeString(GetProp(row, "CONCLUDER_USERNAME")));
            visit["profile_sync_status"] = linked;
            return visit;
        }

        /// <summary>
        /// Cổng đối chiếu form_type với tuổi bệnh nhân tại ngày hoàn thành khám (FaCare trả lời 10/11/2026).
        /// Kiểm trước để báo rõ lý do: ChildUnder dưới 6 tuổi, Minor từ 6 đến dưới 18, Adult từ 18.
        /// </summary>
        private static string CheckFacareAge(string formType, long dob, long conclusionTime, string kskTypeName)
        {
            DateTime? birth = TimeNumberToDate(dob);
            if (!birth.HasValue) return null;   // thiếu ngày sinh: để cổng báo
            DateTime at = TimeNumberToDate(conclusionTime) ?? DateTime.Now;
            int age = at.Year - birth.Value.Year;
            if (at.Month < birth.Value.Month || (at.Month == birth.Value.Month && at.Day < birth.Value.Day)) age--;

            string formName;
            bool ok;
            if (string.Equals(formType, "ChildUnder", StringComparison.OrdinalIgnoreCase)) { formName = "trẻ dưới 6 tuổi"; ok = age < 6; }
            else if (string.Equals(formType, "Minor", StringComparison.OrdinalIgnoreCase)) { formName = "từ 6 đến dưới 18 tuổi"; ok = age >= 6 && age < 18; }
            else if (string.Equals(formType, "Adult", StringComparison.OrdinalIgnoreCase)) { formName = "từ 18 tuổi"; ok = age >= 18; }
            else return null;
            if (ok) return null;
            return "FACARE: Không gửi hồ sơ — loại khám " + (string.IsNullOrWhiteSpace(kskTypeName) ? "(không rõ)" : kskTypeName.Trim())
                + " dùng mẫu " + formName + " nhưng bệnh nhân " + Math.Max(age, 0) + " tuổi tại ngày kết luận "
                + at.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + ".";
        }

        /// <summary>Bản sao thân gửi để ghi log: thay giá trị chữ ký số bằng độ dài.</summary>
        private static JObject MaskFacareSignature(JObject envelope)
        {
            try
            {
                JObject copy = (JObject)envelope.DeepClone();
                foreach (JToken rec in (copy["records"] as JArray) ?? new JArray())
                {
                    JObject o = rec as JObject;
                    if (o != null && o[FACARE_FIELD_SIGNATURE] != null)
                        o[FACARE_FIELD_SIGNATURE] = "<" + o[FACARE_FIELD_SIGNATURE].ToString().Length + " ky tu>";
                }
                return copy;
            }
            catch { return null; }
        }

        /// <summary>yyyyMMddHHmmss -> DateTime; không hợp lệ -> null.</summary>
        private static DateTime? TimeNumberToDate(long value)
        {
            if (value <= 0) return null;
            DateTime d;
            return DateTime.TryParseExact(value.ToString(CultureInfo.InvariantCulture), "yyyyMMddHHmmss",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out d) ? (DateTime?)d : null;
        }

        /// <summary>Giờ HIS (giờ Việt Nam) -> ISO 8601 có offset, ví dụ 2026-11-09T07:30:00+07:00.</summary>
        private static string FormatFacareTime(long value)
        {
            DateTime? d = TimeNumberToDate(value);
            return d.HasValue ? d.Value.ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture) + "+07:00" : null;
        }

        private static string ReadText(JObject o, string name)
        {
            if (o == null) return null;
            JToken v = o[name];
            return (v == null || v.Type == JTokenType.Null) ? null : v.ToString();
        }

        private static string FirstText(params string[] values)
        {
            foreach (string v in values)
                if (!string.IsNullOrWhiteSpace(v)) return v.Trim();
            return null;
        }

        private static void AddFacareText(JObject o, string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value)) o[name] = value.Trim();
        }

        #region đoạn "FACARE:" trong mã giao dịch / trạng thái
        /// <summary>Đoạn "FACARE:" trong chuỗi đã lưu; không có -> null.</summary>
        private static string GetFacareSegment(string stored)
        {
            if (string.IsNullOrEmpty(stored)) return null;
            foreach (string p in stored.Split(';'))
                if (p.StartsWith(FACARE_LABEL, StringComparison.OrdinalIgnoreCase))
                    return EmptyToNull(p.Substring(FACARE_LABEL.Length).Trim());
            return null;
        }

        /// <summary>Ghi/xóa đoạn "FACARE:" mà KHÔNG mất giá trị cổng khác (cùng cách làm với đoạn "KDLSK:").</summary>
        private static string SetFacareSegment(string existing, string value, bool isTxn)
        {
            if (string.IsNullOrEmpty(value) && GetFacareSegment(existing) == null) return existing;
            var parts = new List<string>();
            if (!string.IsNullOrEmpty(existing))
            {
                if (!HasGatewayLabel(existing) && IsVlgBareValue(existing, isTxn))
                    parts.Add("VLG:" + existing.Trim());
                else
                    foreach (string p in existing.Split(';'))
                        if (p.Length > 0 && !p.StartsWith(FACARE_LABEL, StringComparison.OrdinalIgnoreCase)) parts.Add(p);
            }
            if (!string.IsNullOrEmpty(value)) parts.Add(FACARE_LABEL + value);
            return (parts.Count > 0) ? string.Join(";", parts.ToArray()) : null;
        }

        /// <summary>
        /// Trạng thái FaCare ghi lại: hồ sơ đã có trên cổng (DA_LIEN_THONG / CHO_LIEN_THONG) chỉ được đổi từ
        /// CHO_LIEN_THONG sang DA_LIEN_THONG, không bị ghi đè bằng trạng thái khác.
        /// </summary>
        private static string MergeFacareState(string stored, string next)
        {
            if (string.IsNullOrEmpty(next)) return stored;
            if (!KskFacarePushResult.IsOnPortal(stored)) return next;
            if (string.Equals(stored, KskFacarePushResult.STATE_WAIT_LINK, StringComparison.OrdinalIgnoreCase)
                && string.Equals(next, KskFacarePushResult.STATE_LINKED, StringComparison.OrdinalIgnoreCase))
                return next;
            return stored;
        }

        /// <summary>Log 1 dòng cấu hình cổng FaCare (mật khẩu, khóa bí mật chỉ ghi có/không + độ dài).</summary>
        private static void LogFacareConfig(string rawValue, bool selected, KskFacareConfig cfg)
        {
            try
            {
                if (!LogConfigState("FACARE", KskFacareConfigParser.CONFIG_KEY, rawValue, selected, cfg == null)) return;
                Inventec.Common.Logging.LogSystem.Info(string.Format(
                    "Cau hinh FACARE ({0}): Username={1}; Password={2}; SecretKey={3}; BaseUrl={4}",
                    KskFacareConfigParser.CONFIG_KEY, Show(cfg.Username), Mask(cfg.Password), Mask(cfg.SecretKey), Show(cfg.BaseUrl)));
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }
        #endregion
    }
}

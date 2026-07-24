/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000519.ADO;
using MPS.Processor.Mps000519.PDO;
using MPS.ProcessorBase.Core;

namespace MPS.Processor.Mps000519
{
    /// <summary>
    /// Hồ sơ quản lý sức khỏe cá nhân theo QĐ 831 (A + B + C + D).
    /// Dữ liệu vào là các model object hiện có (xem Mps000519PDO). Sinh key:
    ///  - Prefix single key: SREQ_ (y lệnh), PATIENT_ (bệnh nhân), PROFILE_ (hồ sơ), GENERAL_ (khám/kết luận), DHST_ (sinh tồn).
    ///  - Object-tag + quan hệ: checklist tiền sử (DiseaseType/DiseaseDetail/DiseaseResult) để template lặp band.
    /// </summary>
    public class Mps000519Processor : AbstractProcessor
    {
        Mps000519PDO rdo;
        TreatmentAdo TreatmentAdos { get; set; }

        public Mps000519Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000519PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();

                TreatmentAdos = new TreatmentAdo();
                if (rdo.treatment != null)
                {
                    TreatmentAdo ado = new TreatmentAdo();
                    Inventec.Common.Mapper.DataObjectMapper.Map<TreatmentAdo>(ado, rdo.treatment);
                    TreatmentAdos = ado;
                }
                SetImageKey();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));

                // ---- Object-tag (band lặp / {Obj.FIELD}) ----
                objectTag.AddObjectData(store, "Treatment", new List<TreatmentAdo>() { TreatmentAdos });
                objectTag.AddObjectData(store, "KskProfile", new List<HIS_KSK_PROFILE>() { rdo.HisKskProfile ?? new HIS_KSK_PROFILE() });
                objectTag.AddObjectData(store, "KskGeneral", new List<HIS_KSK_GENERAL>() { rdo.HisKskGeneral ?? new HIS_KSK_GENERAL() });
                objectTag.AddObjectData(store, "Dhst", new List<HIS_DHST>() { rdo.HisDhst ?? new HIS_DHST() });

                // Checklist tiền sử: type -> detail -> result (template lặp {DiseaseResult.*}, {DiseaseResult.DiseaseDetail.NAME}...)
                objectTag.AddObjectData(store, "DiseaseType", rdo.DiseaseTypes ?? new List<HIS_DISEASE_TYPE>());
                objectTag.AddObjectData(store, "DiseaseDetail", rdo.DiseaseDetails ?? new List<HIS_DISEASE_DETAIL>());
                objectTag.AddObjectData(store, "DiseaseResult", rdo.DiseaseDetailResults ?? new List<HIS_DISEASE_DETAIL_RESULT>());
                objectTag.AddRelationship(store, "DiseaseDetail", "DiseaseType", "DISEASE_TYPE_ID", "ID");
                objectTag.AddRelationship(store, "DiseaseResult", "DiseaseDetail", "DISEASE_DETAIL_ID", "ID");

                // Mục C — Tiêm chủng: danh sách GỘP danh mục + đã lưu, band theo nhóm.
                // {Vaccine1.*} nhóm 1 (trẻ em), {Vaccine2.*} nhóm 2 (ngoài TCMR), {Vaccine3.*} nhóm 3 (UV thai).
                var vaccineRows = BuildVaccinationList();
                objectTag.AddObjectData(store, "Vaccine1", vaccineRows.Where(v => v.VACCINE_GROUP == 1).ToList());
                objectTag.AddObjectData(store, "Vaccine2", vaccineRows.Where(v => v.VACCINE_GROUP == 2).ToList());
                objectTag.AddObjectData(store, "Vaccine3", vaccineRows.Where(v => v.VACCINE_GROUP == 3).ToList());

                SetSingleKey();
                SetSignatureKeyImageByCFG();

                singleTag.ProcessData(store, singleValueDictionary);
                // Chỉ xử lý barcode khi thực sự có key barcode — tránh ArgumentNullException("dicData")
                // do ProcessBarCodeTag ném khi dicImage rỗng (template này không có tag <#BARCODE>).
                if (dicImage != null && dicImage.Count > 0)
                    barCodeTag.ProcessData(store, dicImage);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void SetSingleKey()
        {
            try
            {
                if (rdo.HisServiceReq != null)
                    AddObjectKeyIntoListkeyWithPrefix<HIS_SERVICE_REQ>(rdo.HisServiceReq, "SREQ_", false);
                if (rdo.HisPatient != null)
                    AddObjectKeyIntoListkeyWithPrefix<HIS_PATIENT>(rdo.HisPatient, "PATIENT_", false);
                if (rdo.HisKskProfile != null)
                    AddObjectKeyIntoListkeyWithPrefix<HIS_KSK_PROFILE>(rdo.HisKskProfile, "PROFILE_", false);
                if (rdo.HisKskGeneral != null)
                    AddObjectKeyIntoListkeyWithPrefix<HIS_KSK_GENERAL>(rdo.HisKskGeneral, "GENERAL_", false);
                if (rdo.HisDhst != null)
                    AddObjectKeyIntoListkeyWithPrefix<HIS_DHST>(rdo.HisDhst, "DHST_", false);

                SetConclusionKeysFromGeneral();
                SetDiseaseHistoryKeys();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Key kết luận phái sinh từ HIS_KSK_GENERAL: kết luận sức khỏe (1/2/3 -> cờ "x"),
        /// kết luận ICD (1/2/3 -> cờ "x" + mã/tên), văn bản kết luận, tư vấn, bác sĩ, thời gian kết luận.
        /// </summary>
        private void SetConclusionKeysFromGeneral()
        {
            var g = rdo.HisKskGeneral;

            long? hc = g != null ? N(g.HEALTH_CONCLUSION_TYPE) : null;
            SetSingleKey(new KeyValue(Mps000519ExtendSingleKey.CONCLUSION_NORMAL_X, hc == 1 ? "x" : ""));
            SetSingleKey(new KeyValue(Mps000519ExtendSingleKey.CONCLUSION_TB_RISK_X, hc == 2 ? "x" : ""));
            SetSingleKey(new KeyValue(Mps000519ExtendSingleKey.CONCLUSION_HEALTH_ISSUE_X, hc == 3 ? "x" : ""));

            long? ic = g != null ? N(g.CONCLUSION_ICD_TYPE) : null;
            SetSingleKey(new KeyValue(Mps000519ExtendSingleKey.CONCLUSION_ICD_NONE_X, ic == 1 ? "x" : ""));
            SetSingleKey(new KeyValue(Mps000519ExtendSingleKey.CONCLUSION_ICD_PRELIM_X, ic == 2 ? "x" : ""));
            SetSingleKey(new KeyValue(Mps000519ExtendSingleKey.CONCLUSION_ICD_FINAL_X, ic == 3 ? "x" : ""));
            SetSingleKey(new KeyValue(Mps000519ExtendSingleKey.CONCLUSION_ICD_CODE, g != null ? (g.CONCLUSION_ICD_CODE ?? "") : ""));
            SetSingleKey(new KeyValue(Mps000519ExtendSingleKey.CONCLUSION_ICD_NAME, g != null ? (g.CONCLUSION_ICD_NAME ?? "") : ""));

            SetSingleKey(new KeyValue(Mps000519ExtendSingleKey.DISEASES, g != null ? (g.DISEASES ?? "") : ""));
            SetSingleKey(new KeyValue(Mps000519ExtendSingleKey.TREATMENT_INSTRUCTION, g != null ? (g.TREATMENT_INSTRUCTION ?? "") : ""));
            SetSingleKey(new KeyValue(Mps000519ExtendSingleKey.CONCLUDER_USERNAME, g != null ? (g.CONCLUDER_USERNAME ?? "") : ""));
            SetSingleKey(new KeyValue(Mps000519ExtendSingleKey.CONCLUDER_LOGINNAME, g != null ? (g.CONCLUDER_LOGINNAME ?? "") : ""));

            long? concTime = g != null ? N(g.CONCLUSION_TIME) : null;
            SetSingleKey(new KeyValue(Mps000519ExtendSingleKey.CONCLUSION_TIME_STR,
                (concTime.HasValue && concTime.Value > 0)
                    ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(concTime.Value)
                    : ""));
        }

        /// <summary>
        /// Mục 3 (và 4/6) — Tiền sử bệnh tật, dị ứng. Sinh single key ĐIỀN SẴN theo danh mục
        /// (mỗi HIS_DISEASE_DETAIL = 1 ô cố định trên biểu QĐ831), thay vì band lặp:
        ///   - Cờ tích:  {TS_D&lt;typeCode&gt;_&lt;numOrder&gt;_X}   = "x" nếu IS_CHECK = 1
        ///   - Ghi rõ:   {TS_D&lt;typeCode&gt;_&lt;numOrder&gt;_TXT} = OTHER (ghi rõ / mô tả (+ người mắc))
        /// Khóa bám DISEASE_TYPE_CODE + NUM_ORDER nên ổn định giữa các cơ sở (không phụ thuộc ID danh mục).
        /// Nguồn dữ liệu lưu: HIS_DISEASE_DETAIL_RESULT (DISEASE_DETAIL_ID -&gt; IS_CHECK/OTHER),
        /// móc danh mục HIS_DISEASE_DETAIL -&gt; HIS_DISEASE_TYPE để lấy code.
        /// </summary>
        private void SetDiseaseHistoryKeys()
        {
            try
            {
                var types = rdo.DiseaseTypes ?? new List<HIS_DISEASE_TYPE>();
                var details = rdo.DiseaseDetails ?? new List<HIS_DISEASE_DETAIL>();
                var results = rdo.DiseaseDetailResults ?? new List<HIS_DISEASE_DETAIL_RESULT>();

                // DISEASE_TYPE_ID -> DISEASE_TYPE_CODE
                var codeByTypeId = new Dictionary<long, string>();
                foreach (var t in types)
                {
                    if (t == null || codeByTypeId.ContainsKey(t.ID)) continue;
                    codeByTypeId[t.ID] = (t.DISEASE_TYPE_CODE ?? "").Trim();
                }

                // DISEASE_DETAIL_ID -> kết quả đã lưu (dòng checklist của đợt khám)
                var resultByDetailId = new Dictionary<long, HIS_DISEASE_DETAIL_RESULT>();
                foreach (var r in results)
                {
                    long? did = N(r != null ? (object)r.DISEASE_DETAIL_ID : null);
                    if (r == null || !did.HasValue || resultByDetailId.ContainsKey(did.Value)) continue;
                    resultByDetailId[did.Value] = r;
                }

                // Với MỌI mục danh mục -> sinh key (ô không tích/để trống vẫn có key rỗng cho biểu cố định).
                foreach (var d in details)
                {
                    if (d == null) continue;
                    long? typeId = N(d.DISEASE_TYPE_ID);
                    string code = (typeId.HasValue && codeByTypeId.ContainsKey(typeId.Value)) ? codeByTypeId[typeId.Value] : "";
                    if (string.IsNullOrEmpty(code)) continue;

                    long num = N(d.NUM_ORDER) ?? 0;
                    string baseKey = Mps000519ExtendSingleKey.DISEASE_KEY_PREFIX + code + "_" + num; // vd TS_D49_1

                    HIS_DISEASE_DETAIL_RESULT res;
                    resultByDetailId.TryGetValue(d.ID, out res);
                    bool isCheck = res != null && (N(res.IS_CHECK) ?? 0) == 1;
                    string other = res != null ? (res.OTHER ?? "") : "";

                    string mota, nguoiMac;
                    SplitOther(other, out mota, out nguoiMac);

                    SetSingleKey(new KeyValue(baseKey + Mps000519ExtendSingleKey.FLAG_SUFFIX, isCheck ? "x" : ""));
                    SetSingleKey(new KeyValue(baseKey + Mps000519ExtendSingleKey.DISEASE_TEXT_SUFFIX, other));
                    SetSingleKey(new KeyValue(baseKey + Mps000519ExtendSingleKey.DISEASE_DESC_SUFFIX, mota));
                    SetSingleKey(new KeyValue(baseKey + Mps000519ExtendSingleKey.DISEASE_PERSON_SUFFIX, nguoiMac));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Mục C — Tiêm chủng. Trả về DANH SÁCH GỘP: mỗi loại vắc xin trong danh mục (HIS_VACCINE_TYPE)
        /// kèm trạng thái đã lưu tương ứng (HIS_HEALTH_VACCINATION, khớp theo VACCINE_CODE + nhóm).
        /// Danh mục là gốc (loại chưa nhập vẫn có dòng trống), sắp theo nhóm rồi ID (như màn nhập).
        /// </summary>
        private List<VaccinationAdo> BuildVaccinationList()
        {
            var result = new List<VaccinationAdo>();
            try
            {
                var types = (rdo.VaccineTypes ?? new List<HIS_VACCINE_TYPE>())
                    .Where(t => t != null && (t.IS_DELETE == null || t.IS_DELETE == 0))
                    .OrderBy(t => N(t.TYPE_VACCINE) ?? 0).ThenBy(t => t.ID)
                    .ToList();
                var saved = (rdo.HealthVaccinations ?? new List<HIS_HEALTH_VACCINATION>())
                    .Where(h => h != null && (h.IS_DELETE == null || h.IS_DELETE == 0))
                    .ToList();

                var sttByGroup = new Dictionary<int, int>();
                foreach (var t in types)
                {
                    int group = (int)(N(t.TYPE_VACCINE) ?? 0);
                    if (group <= 0) continue;
                    string code = (t.VACCINE_TYPE_CODE ?? "").Trim();

                    var h = saved.FirstOrDefault(x => (int)(N(x.VACCINE_GROUP) ?? 0) == group
                        && string.Equals((x.VACCINE_CODE ?? "").Trim(), code, StringComparison.OrdinalIgnoreCase));

                    bool has = h != null;
                    bool chua = has && (N(h.IS_NOT_VACCINATED) ?? 0) == 1;
                    string vtime = has ? NumToDateStr(N(h.VACCINATED_TIME)) : "";

                    int stt;
                    sttByGroup.TryGetValue(group, out stt);
                    stt++;
                    sttByGroup[group] = stt;

                    result.Add(new VaccinationAdo
                    {
                        VACCINE_GROUP = group,
                        VACCINE_CODE = code,
                        VACCINE_NAME = (t.VACCINE_TYPE_NAME ?? "").Trim(),
                        STT = stt,
                        NOT_VACCINATED_X = chua ? "x" : "",
                        VACCINATED_X = (has && !chua && !string.IsNullOrEmpty(vtime)) ? "x" : "",
                        VACCINATED_TIME_STR = vtime,
                        PREGNANCY_MONTH = (has && h.PREGNANCY_MONTH != null) ? h.PREGNANCY_MONTH.ToString() : "",
                        REACTION = has ? (h.REACTION ?? "") : "",
                        APPOINTMENT_TIME_STR = has ? NumToDateStr(N(h.APPOINTMENT_TIME)) : ""
                    });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>Số thời gian yyyyMMddHHmmss → "dd/MM/yyyy" (rỗng nếu không hợp lệ).</summary>
        private static string NumToDateStr(long? num)
        {
            if (!num.HasValue || num.Value <= 0) return "";
            string s = num.Value.ToString();
            if (s.Length < 8) return "";
            return s.Substring(6, 2) + "/" + s.Substring(4, 2) + "/" + s.Substring(0, 4);
        }

        internal void SetImageKey()
        {
            try
            {
                if (TreatmentAdos != null && !string.IsNullOrEmpty(TreatmentAdos.TDL_PATIENT_AVATAR_URL))
                    SetSingleImage(TreatmentAdos, TreatmentAdos.TDL_PATIENT_AVATAR_URL);

                // {IMG_AVATAR} lấy từ HIS_PATIENT.AVATAR_URL.
                if (rdo.HisPatient != null && !string.IsNullOrEmpty(rdo.HisPatient.AVATAR_URL))
                    SetSingleImage(Mps000519ExtendSingleKey.IMG_AVATAR, rdo.HisPatient.AVATAR_URL);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetSingleImage(TreatmentAdo key, string imageUrl)
        {
            try
            {
                MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(imageUrl);
                key.AVATAR = stream != null ? stream.ToArray() : null;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        public void SetSingleImage(string key, string imageUrl)
        {
            try
            {
                MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(imageUrl);
                SetSingleKey(new KeyValue(key, stream != null ? (object)stream.ToArray() : ""));
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Tách OTHER của checklist gia đình: "mô tả | Người mắc: X" → mota + nguoiMac.
        /// (Đối xứng với logic gộp JoinOther/SplitOther ở plugin nhập KSK.)
        /// </summary>
        private static void SplitOther(string other, out string mota, out string nguoiMac)
        {
            mota = other ?? "";
            nguoiMac = "";
            if (string.IsNullOrEmpty(other)) { mota = ""; return; }
            const string marker = "Người mắc:";
            int idx = other.IndexOf(marker, StringComparison.Ordinal);
            if (idx >= 0)
            {
                nguoiMac = other.Substring(idx + marker.Length).Trim();
                mota = other.Substring(0, idx).TrimEnd(' ', '|').Trim();
            }
        }

        /// <summary>Đọc số bất kể EF sinh short?/long?/decimal? → long?.</summary>
        private static long? N(object v)
        {
            if (v == null) return null;
            long r;
            return long.TryParse(v.ToString(), out r) ? (long?)r : (long?)null;
        }
    }
}

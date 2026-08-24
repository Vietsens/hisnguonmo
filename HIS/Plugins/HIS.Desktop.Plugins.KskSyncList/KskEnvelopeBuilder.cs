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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using His.Ksk.QD2062.Base;
using His.Ksk.QD2062.Builder;
using His.Ksk.QD2062.Models;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.KskSyncList
{
    /// <summary>
    /// Dung ban tin (envelope) KHAMSUCKHOE — XML hoac JSON — cho danh sach ho so, DIEN DU CA 12 KHOI
    /// XML1..XML12: khoi nao khong co du lieu thi day len TRONG (cac truong de gia tri mac dinh) thay vi
    /// bi bo qua nhu mac dinh cua thu vien.
    ///
    /// Ly do co lop nay: <c>CreateQd1551Main.BuildEnvelope</c> map roi serialize NGAY trong thu vien nen
    /// khong chen duoc khoi trong. Lop nay lam dung 3 buoc do bang API PUBLIC cua thu vien:
    ///   1. <c>Qd1551KskMapper.Build(input)</c>  -> KhamSucKhoeData (chi co khoi nao co du lieu nguon)
    ///   2. <c>FillEmptyBlocks(model)</c>        -> khoi null = tao doi tuong RONG (reflection)
    ///   3. <c>KhamSucKhoePackageBuilder.BuildEnvelope(models, macskcb, ngayLap, asJson)</c>
    ///
    /// Pham vi anh huong: file XML xuat ra (ExportXmlFiles) va ban tin day cong HCC (BuildHccPayload).
    /// Cong BYT/HSSK/HOC day qua <c>CreateQd1551Main.PushListMulti</c> — thu vien tu map ben trong nen
    /// KHONG chen duoc khoi trong (muon ap dung cho ca 3 cong do thi phai sua thu vien).
    /// </summary>
    internal static class KskEnvelopeBuilder
    {
        /// <summary>
        /// XML11 (can lam sang) la MANG. Thu vien chi xuat khoi nay khi mang co it nhat 1 dong, nen de
        /// khoi XML11 xuat hien khi ho so khong co CLS thi phai them 1 DONG RONG. Dat false neu cong tu
        /// choi dong CLS rong (luc do ho so khong co CLS se khong co khoi XML11).
        /// </summary>
        internal const bool ADD_EMPTY_XML11_ROW = true;

        /// <summary>
        /// Dung envelope cho danh sach ho so (SOLUONGHOSO = so ho so). macskcb = ma don vi cua cong dich
        /// (SenderId). asJson = true -> ban tin JSON, false -> XML. Tra "" khi khong dung duoc.
        /// </summary>
        internal static string Build(IEnumerable<Qd1551KskInput> inputs, string macskcb, bool asJson)
        {
            try
            {
                var models = new List<KhamSucKhoeData>();
                if (inputs != null)
                {
                    foreach (Qd1551KskInput input in inputs)
                    {
                        if (input == null) continue;
                        KhamSucKhoeData model = Qd1551KskMapper.Build(input);
                        if (model == null) continue;
                        EnsureKetLuan(model, input);          // XML12 thieu/sai phan loai -> nap tu ban ghi KSK
                        EnsureKetLuanVaTuVan(model, input);   // XML8 rong (thieu HIS_KSK_GENERAL) -> nap tu ban ghi KSK
                        FillEmptyBlocks(model);
                        models.Add(model);
                    }
                }
                if (models.Count == 0) return "";

                string ngayLap = DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                return new KhamSucKhoePackageBuilder().BuildEnvelope(models, macskcb ?? "", ngayLap, asJson);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return "";
            }
        }

        #region XML12 (ket luan) — nap bu tu ban ghi KSK khi thu vien khong dung duoc
        private const string PROP_KET_LUAN = "KetLuan";                 // KhamSucKhoeData -> Xml12ADO
        private const string F_PHAN_LOAI_SK = "PHAN_LOAI_SK";          // int, BAT BUOC thuoc 1..5
        private const string F_KET_LUAN_BENH = "KET_LUAN_BENH";
        private const string F_CAC_VAN_DE_SUC_KHOE = "CAC_VAN_DE_SUC_KHOE";
        private const string SRC_RANK_ID = "HEALTH_EXAM_RANK_ID";
        private const string SRC_ICD_CODE = "CONCLUSION_ICD_CODE";
        private const string SRC_RANK_DESCRIPTION = "HEALTH_EXAM_RANK_DESCRIPTION";

        /// <summary>
        /// XML12: thu vien chi dien PHAN_LOAI_SK khi quy doi duoc rank ra 1..5, nen ho so chua phan loai se
        /// ra 0 (cong tu choi: "phan_loai_sk thieu hoac khong thuoc 1..5"). Ham nay NAP BU tung truong con
        /// THIEU tu ban ghi KSK (uu tien HIS_KSK_GENERAL, roi ban ghi theo lua tuoi):
        ///   PHAN_LOAI_SK        &lt;- HEALTH_EXAM_RANK_ID (quy doi qua HIS_HEALTH_EXAM_RANK.HEALTH_EXAM_RANK_CODE)
        ///   KET_LUAN_BENH       &lt;- CONCLUSION_ICD_CODE
        ///   CAC_VAN_DE_SUC_KHOE &lt;- HEALTH_EXAM_RANK_DESCRIPTION
        /// KHONG ghi de gia tri thu vien da dien (chi dien vao cho dang rong / rank ngoai 1..5).
        /// Doc nguon bang reflection nen khong phu thuoc phien ban MOS.EFMODEL (truong co the nam o
        /// HIS_KSK_GENERAL hoac HIS_KSK_OVER_EIGHTEEN tuy phien ban).
        /// </summary>
        private static void EnsureKetLuan(KhamSucKhoeData model, Qd1551KskInput input)
        {
            try
            {
                if (model == null || input == null) return;
                PropertyInfo ketLuanProp = typeof(KhamSucKhoeData).GetProperty(PROP_KET_LUAN);
                if (ketLuanProp == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("KskEnvelopeBuilder: khong thay thuoc tinh "
                        + PROP_KET_LUAN + " tren KhamSucKhoeData -> bo qua nap bu XML12.");
                    return;
                }

                object ketLuan = ketLuanProp.GetValue(model, null);
                int currentRank = ToInt(GetProp(ketLuan, F_PHAN_LOAI_SK));
                bool needRank = !(currentRank >= 1 && currentRank <= 5);
                bool needIcd = string.IsNullOrWhiteSpace(SafeStr(GetProp(ketLuan, F_KET_LUAN_BENH)));
                bool needDesc = string.IsNullOrWhiteSpace(SafeStr(GetProp(ketLuan, F_CAC_VAN_DE_SUC_KHOE)));
                if (!needRank && !needIcd && !needDesc) return;

                // Nguon nap bu: ban ghi ket luan chung truoc, sau do ban ghi KSK theo lua tuoi.
                object[] sources = new object[] { input.General, input.OverEighteen, input.UnderEighteen, input.UnderSix };

                int rankCode = 0; string icdCode = null, rankDesc = null;
                string fromRank = null, fromIcd = null, fromDesc = null;
                foreach (object src in sources)
                {
                    if (src == null) continue;
                    if (needRank && rankCode <= 0)
                    {
                        int code = ResolveRankCode(ToLong(GetProp(src, SRC_RANK_ID)), input.HealthExamRanks);
                        if (code >= 1 && code <= 5) { rankCode = code; fromRank = src.GetType().Name; }
                    }
                    if (needIcd && string.IsNullOrEmpty(icdCode))
                    {
                        string v = SafeStr(GetProp(src, SRC_ICD_CODE));
                        if (!string.IsNullOrWhiteSpace(v)) { icdCode = v.Trim(); fromIcd = src.GetType().Name; }
                    }
                    if (needDesc && string.IsNullOrEmpty(rankDesc))
                    {
                        string v = SafeStr(GetProp(src, SRC_RANK_DESCRIPTION));
                        if (!string.IsNullOrWhiteSpace(v)) { rankDesc = v.Trim(); fromDesc = src.GetType().Name; }
                    }
                }
                if (rankCode <= 0 && icdCode == null && rankDesc == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("XML12: thieu du lieu ket luan va KHONG nap bu duoc"
                        + " (ban ghi KSK khong co " + SRC_RANK_ID + " / " + SRC_ICD_CODE + " / "
                        + SRC_RANK_DESCRIPTION + ") -> phan_loai_sk se la 0.");
                    return;
                }

                if (ketLuan == null)
                {
                    ketLuan = Activator.CreateInstance(ketLuanProp.PropertyType);
                    ketLuanProp.SetValue(model, ketLuan, null);
                }

                var filled = new List<string>();
                if (rankCode > 0) { SetProp(ketLuan, F_PHAN_LOAI_SK, rankCode); filled.Add(F_PHAN_LOAI_SK + "=" + rankCode + " (tu " + fromRank + ")"); }
                if (icdCode != null) { SetProp(ketLuan, F_KET_LUAN_BENH, icdCode); filled.Add(F_KET_LUAN_BENH + "=" + icdCode + " (tu " + fromIcd + ")"); }
                if (rankDesc != null) { SetProp(ketLuan, F_CAC_VAN_DE_SUC_KHOE, rankDesc); filled.Add(F_CAC_VAN_DE_SUC_KHOE + " (tu " + fromDesc + ")"); }
                if (filled.Count > 0)
                    Inventec.Common.Logging.LogSystem.Info("XML12: nap bu ket luan -> " + string.Join("; ", filled.ToArray()));
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        #region XML8 (ket luan va tu van) — nap bu khi thu vien khong dung duoc
        private const string PROP_KET_LUAN_TU_VAN = "KetLuanVaTuVan";   // KhamSucKhoeData -> Xml8ADO
        private const string F_BINH_THUONG = "BINH_THUONG";
        private const string F_NGUY_CO_MAC_LAO = "NGUY_CO_MAC_LAO";
        private const string F_VAN_DE_SUC_KHOE = "VAN_DE_SUC_KHOE";
        private const string F_KET_LUAN_BENH_8 = "KET_LUAN_BENH";
        private const string F_GHI_RO_VAN_DE = "GHI_RO_VAN_DE_SUC_KHOE";
        private const string F_HEN_KHAM_LAN_SAU = "HEN_KHAM_LAN_SAU";
        private const string F_CHUYEN_CSKCB = "CHUYEN_CSKCB";

        /// <summary>
        /// XML8: thu vien chi dung khoi nay khi CO ban ghi HIS_KSK_GENERAL (MapConclusionTre thoat ngay neu
        /// general == null), va lay: BINH_THUONG/NGUY_CO_MAC_LAO/VAN_DE_SUC_KHOE &lt;- HEALTH_CONCLUSION_TYPE
        /// (1/2/3); KET_LUAN_BENH + GHI_RO_VAN_DE_SUC_KHOE &lt;- DISEASES; HEN_KHAM_LAN_SAU &lt;- co
        /// TREATMENT_INSTRUCTION; CHUYEN_CSKCB &lt;- HIS_KSK_UNDER_SIX.IS_TRANSFER_MEDI_ORG.
        /// Ho so nhap thieu (chua co ban ghi ket luan chung) -> khoi XML8 RONG, cong coi nhu "khong co XML8".
        ///
        /// Ham nay NAP BU tung truong con thieu:
        ///   - KET_LUAN_BENH / GHI_RO_VAN_DE_SUC_KHOE: DISEASES -> CONCLUSION_ICD_NAME -> CONCLUSION_ICD_CODE
        ///                                              -> CLINICAL_OBSERVATION (ban ghi tre &lt;6)
        ///   - HEN_KHAM_LAN_SAU : co TREATMENT_INSTRUCTION -> 1
        ///   - CHUYEN_CSKCB     : IS_TRANSFER_MEDI_ORG000001896694 
        ///   - 3 co ket luan (khi CA BA deu = 0):
        ///       NGUY_CO_MAC_LAO = 1 neu IS_TB_CONTACT = 1
        ///       VAN_DE_SUC_KHOE = 1 neu co dau hieu bat thuong (SDD/thua can/thieu mau/coi xuong/phu dinh duong/
        ///                          nguy co tu ky, hoac phat trien tinh than-van dong KHONG binh thuong),
        ///                          hoac phan loai suc khoe 3..5
        ///       BINH_THUONG     = 1 cho cac truong hop con lai (phan loai 1..2 hoac khong co dau hieu bat thuong)
        /// KHONG ghi de gia tri thu vien da dien. 
        /// </summary>
        private static void EnsureKetLuanVaTuVan(KhamSucKhoeData model, Qd1551KskInput input)
        {
            try
            {
                if (model == null || input == null) return;
                PropertyInfo prop = typeof(KhamSucKhoeData).GetProperty(PROP_KET_LUAN_TU_VAN);
                if (prop == null) return;

                object block = prop.GetValue(model, null);
                object general = input.General;
                object child = input.UnderSix;

                // 1. Ket luan benh / ghi ro van de suc khoe
                string ketLuanBenh = SafeStr(GetProp(block, F_KET_LUAN_BENH_8));
                if (string.IsNullOrWhiteSpace(ketLuanBenh))
                    ketLuanBenh = FirstNonEmpty(GetProp(general, "DISEASES"), GetProp(general, "CONCLUSION_ICD_NAME"),
                                                GetProp(general, "CONCLUSION_ICD_CODE"), GetProp(child, "CLINICAL_OBSERVATION"));

                // 2. Hen kham lan sau / chuyen co so
                bool henKham = ToInt(GetProp(block, F_HEN_KHAM_LAN_SAU)) == 1
                            || !string.IsNullOrWhiteSpace(SafeStr(GetProp(general, "TREATMENT_INSTRUCTION")));
                int chuyenCskcb = ToInt(GetProp(block, F_CHUYEN_CSKCB));
                if (chuyenCskcb != 1) chuyenCskcb = (ToInt(GetProp(child, "IS_TRANSFER_MEDI_ORG")) == 1) ? 1 : 0;

                // 3. Ba co ket luan — chi suy khi thu vien chua dat co nao
                int binhThuong = ToInt(GetProp(block, F_BINH_THUONG));
                int nguyCoLao = ToInt(GetProp(block, F_NGUY_CO_MAC_LAO));
                int vanDeSucKhoe = ToInt(GetProp(block, F_VAN_DE_SUC_KHOE));
                if (binhThuong != 1 && nguyCoLao != 1 && vanDeSucKhoe != 1)
                {
                    if (ToInt(GetProp(child, "IS_TB_CONTACT")) == 1) nguyCoLao = 1;

                    int rank = ResolveRankCode(ToLong(GetProp(general, SRC_RANK_ID)), input.HealthExamRanks);
                    bool coBatThuong = HasAny(child, 1, "IS_MALNUTRITION", "IS_OVERWEIGHT", "IS_ANEMIA_SIGN",
                                                       "IS_RICKETS_SIGN", "IS_NUTRITIONAL_EDEMA", "AUTISM_RISK")
                                    || HasAny(child, 0, "MENTAL_DEV_NORMAL", "MOTOR_DEV_NORMAL")
                                    || (rank >= 3 && rank <= 5);
                    if (coBatThuong) vanDeSucKhoe = 1; else binhThuong = 1;
                }

                if (block == null)
                {
                    block = Activator.CreateInstance(prop.PropertyType);
                    prop.SetValue(model, block, null);
                }
                SetProp(block, F_BINH_THUONG, binhThuong);
                SetProp(block, F_NGUY_CO_MAC_LAO, nguyCoLao);
                SetProp(block, F_VAN_DE_SUC_KHOE, vanDeSucKhoe);
                SetProp(block, F_HEN_KHAM_LAN_SAU, henKham ? 1 : 0);
                SetProp(block, F_CHUYEN_CSKCB, chuyenCskcb);
                if (!string.IsNullOrWhiteSpace(ketLuanBenh))
                {
                    SetProp(block, F_KET_LUAN_BENH_8, ketLuanBenh);
                    if (string.IsNullOrWhiteSpace(SafeStr(GetProp(block, F_GHI_RO_VAN_DE))))
                        SetProp(block, F_GHI_RO_VAN_DE, ketLuanBenh);
                }

                Inventec.Common.Logging.LogSystem.Info(string.Format(
                    "XML8: ket luan va tu van -> BINH_THUONG={0}; NGUY_CO_MAC_LAO={1}; VAN_DE_SUC_KHOE={2};"
                    + " HEN_KHAM_LAN_SAU={3}; CHUYEN_CSKCB={4}; KET_LUAN_BENH=\"{5}\"",
                    binhThuong, nguyCoLao, vanDeSucKhoe, henKham ? 1 : 0, chuyenCskcb, SafeStr(ketLuanBenh)));
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Gia tri dau tien khac rong trong danh sach (null neu khong co).</summary>
        private static string FirstNonEmpty(params object[] values)
        {
            if (values == null) return null;
            foreach (object v in values)
            {
                string s = SafeStr(v).Trim();
                if (!string.IsNullOrEmpty(s)) return s;
            }
            return null;
        }

        /// <summary>Co bat ky truong nao trong danh sach mang gia tri chi dinh khong (bo qua truong null).</summary>
        private static bool HasAny(object source, int expected, params string[] fields)
        {
            if (source == null || fields == null) return false;
            foreach (string f in fields)
            {
                object v = GetProp(source, f);
                if (v != null && ToInt(v) == expected) return true;
            }
            return false;
        }
        #endregion

        /// <summary>
        /// Dung cho cong So Y te TP.HCM: mo lai hai ham quy doi phan loai suc khoe da co san.
        /// Bao ngoai chu KHONG doi muc truy cap cua ham goc, de 4 cong dang chay khong bi anh huong.
        /// </summary>
        internal static int ResolveRankCodeForSyt(long rankId, List<HIS_HEALTH_EXAM_RANK> ranks)
        {
            return ResolveRankCode(rankId, ranks);
        }

        /// <summary>Doc cap do phan loai 1..5 tu chuoi ("2" / "02" / "II" / "Loai 2" / "Loai III").</summary>
        internal static int ParseRankForSyt(string raw)
        {
            return ParseRank(raw);
        }

        /// <summary>
        /// Quy doi HEALTH_EXAM_RANK_ID -> ma phan loai suc khoe 1..5 qua danh muc HIS_HEALTH_EXAM_RANK
        /// (giong cach thu vien lam). Khong nap duoc danh muc ma ID vo tinh nam 1..5 -> dung luon ID.
        /// Tra 0 khi khong quy doi duoc.
        /// </summary>
        private static int ResolveRankCode(long rankId, List<HIS_HEALTH_EXAM_RANK> ranks)
        {
            if (rankId <= 0) return 0;

            string rawCode = null, rawName = null;
            bool foundInCatalog = false;
            if (ranks != null)
            {
                foreach (HIS_HEALTH_EXAM_RANK r in ranks)
                {
                    if (r == null || r.ID != rankId) continue;
                    rawCode = r.HEALTH_EXAM_RANK_CODE;
                    rawName = r.HEALTH_EXAM_RANK_NAME;
                    foundInCatalog = true;
                    break;
                }
            }

            int code = ParseRank(rawCode);                                  // "2" / "02" / "II" / "Loai 2"
            if (code == 0) code = ParseRank(rawName);                       // fallback theo TEN phan loai
            if (code == 0 && rankId >= 1 && rankId <= 5) code = (int)rankId; // ID trung ma phan loai

            if (code == 0)
                Inventec.Common.Logging.LogSystem.Warn(string.Format(
                    "XML12: KHONG quy doi duoc phan loai suc khoe -> phan_loai_sk = 0."
                    + " HEALTH_EXAM_RANK_ID={0}; {1}; ma danh muc=\"{2}\"; ten=\"{3}\"."
                    + " (cong yeu cau 1..5 — kiem tra HIS_HEALTH_EXAM_RANK.HEALTH_EXAM_RANK_CODE)",
                    rankId, foundInCatalog ? "co trong danh muc" : "KHONG thay trong danh muc (IS_ACTIVE?)",
                    SafeStr(rawCode), SafeStr(rawName)));
            else
                Inventec.Common.Logging.LogSystem.Info(string.Format(
                    "XML12: HEALTH_EXAM_RANK_ID={0} (ma danh muc \"{1}\") -> phan_loai_sk={2}",
                    rankId, foundInCatalog ? SafeStr(rawCode) : "(khong co trong danh muc)", code));
            return code;
        }

        /// <summary>
        /// Quy doi chuoi phan loai suc khoe ve so 1..5: "2"/"02" (so), "II" (so La Ma),
        /// hoac chuoi co chua so/so La Ma ("Loai 2", "LOAI II", "SK3"). Khong quy doi duoc -> 0.
        /// </summary>
        private static int ParseRank(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0;
            string s = value.Trim().ToUpperInvariant();

            int n;
            if (int.TryParse(s, out n)) return (n >= 1 && n <= 5) ? n : 0;

            int roman = RomanToNumber(s);
            if (roman > 0) return roman;

            System.Text.RegularExpressions.Match digit =
                System.Text.RegularExpressions.Regex.Match(s, "[1-5]");
            if (digit.Success) return int.Parse(digit.Value);

            System.Text.RegularExpressions.Match rm =
                System.Text.RegularExpressions.Regex.Match(s, "(?:^|[^A-Z])(I{1,3}|IV|V)(?:[^A-Z]|$)");
            if (rm.Success) return RomanToNumber(rm.Groups[1].Value);

            return 0;
        }

        /// <summary>So La Ma I..V -> 1..5 (khac -> 0).</summary>
        private static int RomanToNumber(string s)
        {
            switch (s)
            {
                case "I": return 1;
                case "II": return 2;
                case "III": return 3;
                case "IV": return 4;
                case "V": return 5;
                default: return 0;
            }
        }

        private static object GetProp(object obj, string name)
        {
            try
            {
                if (obj == null) return null;
                PropertyInfo p = obj.GetType().GetProperty(name);
                return (p != null && p.CanRead) ? p.GetValue(obj, null) : null;
            }
            catch { return null; }
        }

        private static void SetProp(object obj, string name, object value)
        {
            try
            {
                if (obj == null) return;
                PropertyInfo p = obj.GetType().GetProperty(name);
                if (p == null || !p.CanWrite) return;
                Type t = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                p.SetValue(obj, (value != null && !t.IsInstanceOfType(value)) ? Convert.ChangeType(value, t) : value, null);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn("XML12: khong set duoc " + name + ": " + ex.Message); }
        }

        private static string SafeStr(object o) { return (o == null) ? "" : o.ToString(); }

        private static int ToInt(object o)
        {
            try { return (o == null) ? 0 : Convert.ToInt32(o); } catch { return 0; }
        }

        private static long ToLong(object o)
        {
            try { return (o == null) ? 0 : Convert.ToInt64(o); } catch { return 0; }
        }
        #endregion

        /// <summary>
        /// Khoi (XML1..XML12) dang null -> tao doi tuong RONG de thu vien van xuat khoi do ra ban tin.
        /// Dung reflection tren KhamSucKhoeData nen KHONG can tham chieu 12 assembly XMLn:
        /// - Thuoc tinh kieu string (CKS_NGUOI_KET_LUAN / CKS_BENH_VIEN): GIU NGUYEN.
        /// - Thuoc tinh kieu List&lt;T&gt; (XML11): tao list; rong thi them 1 dong T rong (xem ADD_EMPTY_XML11_ROW).
        /// - Cac thuoc tinh khoi khac: Activator.CreateInstance khi dang null.
        /// Ghi log so khoi vua bo sung de doi chieu voi ban tin.
        /// </summary>
        private static void FillEmptyBlocks(KhamSucKhoeData model)
        {
            var added = new List<string>();
            foreach (PropertyInfo prop in typeof(KhamSucKhoeData).GetProperties())
            {
                try
                {
                    if (!prop.CanRead || !prop.CanWrite) continue;
                    Type type = prop.PropertyType;
                    if (type == typeof(string) || type.IsValueType) continue;   // CKS_* va cac kieu gia tri

                    object current = prop.GetValue(model, null);

                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        IList list = current as IList;
                        if (list == null)
                        {
                            list = (IList)Activator.CreateInstance(type);
                            prop.SetValue(model, list, null);
                        }
                        if (ADD_EMPTY_XML11_ROW && list.Count == 0)
                        {
                            list.Add(Activator.CreateInstance(type.GetGenericArguments()[0]));
                            added.Add(prop.Name + "[1 dong rong]");
                        }
                        continue;
                    }

                    if (current == null)
                    {
                        prop.SetValue(model, Activator.CreateInstance(type), null);
                        added.Add(prop.Name);
                    }
                }
                catch (Exception exProp)
                {
                    Inventec.Common.Logging.LogSystem.Warn("KskEnvelopeBuilder: khong tao duoc khoi rong cho "
                        + prop.Name + ": " + exProp.Message);
                }
            }

            if (added.Count > 0)
                Inventec.Common.Logging.LogSystem.Info("Ban tin KSK: bo sung " + added.Count
                    + " khoi TRONG (khong co du lieu nguon) -> " + string.Join(", ", added.ToArray()));
        }
    }
}

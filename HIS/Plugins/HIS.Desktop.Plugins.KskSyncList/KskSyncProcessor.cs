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
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.KskSyncList.ADO;
using His.Ksk.QD2062;
using His.Ksk.QD2062.Base;
using His.Ksk.QD2062.Builder;
using His.Ksk.QD2062.Transport.Model;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.KskSyncList
{
    /// <summary>
    /// Diem rap noi thu vien dong bo QD 1551 (His.Ksk.QD2062 - thiet ke BD_046, muc 3.4 PTTK_44350).
    ///
    /// Plugin: (1) map ban ghi V_HIS_KSK_SYNC -> mau phieu QD2062 (KskSyncModelMapper);
    /// (2) goi thu vien CreateQd1551Main (BuildPreview / PushList): build XML/JSON -> ky envelope
    /// SHA256RSA -> xac thuc OAuth2 -> POST /api/platform/data-sync/push;
    /// (3) map ket qua tung ho so -> KskSyncResultADO de UC luu qua api/HisKskSync/SaveSyncResult.
    /// </summary>
    internal class KskSyncProcessor
    {
        private readonly string connectionInfo;        // cong BYT (MOS.HIS_KSK_SYNC.CONNECTION_INFO)
        private readonly string hsskConnectionInfo;     // cong HSSK (MOS.HIS_KSK_SYNC.HSSK_HN_2062_CONNECTION_INFO)
        private readonly string hocConnectionInfo;      // cong HOC->TTYTQG (MOS.HIS_KSK_SYNC.HSSK_HOC_2062_CONNECTION_INFO)
        private readonly string hccConnectionInfo;      // cong HCC (MOS.HIS_KSK_SYNC.HSSK_HCC_2062_CONNECTION_INFO)
        private readonly bool pushByt;                  // co day cong BYT
        private readonly bool pushHssk;                 // co day cong HSSK
        private readonly bool pushHoc;                  // co day cong HOC
        private readonly bool pushHcc;                  // co day cong HCC
        private readonly bool sign;
        private readonly SettingSignADO signSetting;

        // Ket qua dong bo (khop KskSyncResultADO.SYNC_RESULT_TYPE): 2 = thanh cong, 3 = that bai.
        private const short RESULT_SUCCESS = 2;
        private const short RESULT_FAILED = 3;

        /// <summary>Ctor cu (chi cong BYT) — giu tuong thich cho preview / cac loi goi khac.</summary>
        internal KskSyncProcessor(string connectionInfo, bool sign, SettingSignADO signSetting)
            : this(connectionInfo, null, null, true, false, false, sign, signSetting)
        {
        }

        /// <summary>Ctor 2 cong (BYT + HSSK) — giu tuong thich cho cac loi goi cu.</summary>
        internal KskSyncProcessor(string connectionInfo, string hsskConnectionInfo, bool pushByt, bool pushHssk,
            bool sign, SettingSignADO signSetting)
            : this(connectionInfo, hsskConnectionInfo, null, pushByt, pushHssk, false, sign, signSetting)
        {
        }

        /// <summary>Ctor 3 cong (BYT + HSSK + HOC) — giu tuong thich cho cac loi goi cu.</summary>
        internal KskSyncProcessor(string connectionInfo, string hsskConnectionInfo, string hocConnectionInfo,
            bool pushByt, bool pushHssk, bool pushHoc, bool sign, SettingSignADO signSetting)
            : this(connectionInfo, hsskConnectionInfo, hocConnectionInfo, null,
                   pushByt, pushHssk, pushHoc, false, sign, signSetting)
        {
        }

        /// <summary>
        /// Ctor day da cong: chon day BYT (pushByt), HSSK (pushHssk), HOC->TTYTQG (pushHoc) va/hoac
        /// HCC (pushHcc). BYT/HSSK/HOC dung CHUNG 1 base64 (thu vien CreateQd1551Main.PushListMulti xu ly),
        /// chi khac API dang nhap + endpoint day + cach dong goi body.
        /// HCC dung base64 RIENG (mac dinh json/base64 theo tai lieu HCC) nen dung payload rieng roi day
        /// bang KskHccPusher — xem KskHccPusher de biet giao thuc.
        /// </summary>
        internal KskSyncProcessor(string connectionInfo, string hsskConnectionInfo, string hocConnectionInfo,
            string hccConnectionInfo, bool pushByt, bool pushHssk, bool pushHoc, bool pushHcc,
            bool sign, SettingSignADO signSetting)
        {
            this.connectionInfo = connectionInfo;
            this.hsskConnectionInfo = hsskConnectionInfo;
            this.hocConnectionInfo = hocConnectionInfo;
            this.hccConnectionInfo = hccConnectionInfo;
            this.pushByt = pushByt;
            this.pushHssk = pushHssk;
            this.pushHoc = pushHoc;
            this.pushHcc = pushHcc;
            this.sign = sign;
            this.signSetting = signSetting;
        }

        /// <summary>
        /// Xem truoc du lieu se day cua mot ho so (Scene 3): map -> mau phieu roi goi
        /// CreateQd1551Main.BuildPreview (khong ky, khong gui) -> chuoi XML/JSON.
        /// </summary>
        internal string BuildPreview(V_HIS_KSK_SYNC row)
        {
            try
            {
                if (row == null) return "";

                CreateQd1551Main main = new CreateQd1551Main(BuildConfig());
                List<V_HIS_KSK_SYNC> one = new List<V_HIS_KSK_SYNC> { row };
                List<Qd1551KskInput> inputs = BuildInputs(one);

                ResultADO result = main.BuildPreview(inputs);
                if (result == null)
                    return "Không tạo được dữ liệu xem trước.";
                if (!result.Success)
                    return string.IsNullOrEmpty(result.Message) ? "Không tạo được dữ liệu xem trước." : result.Message;

                return (result.Data != null && result.Data.Length > 0 && result.Data[0] != null)
                    ? result.Data[0].ToString()
                    : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return "Lỗi tạo dữ liệu xem trước: " + ex.Message;
            }
        }

        /// <summary>
        /// Xuat file XML cho danh sach ho so ra thu muc dirPath — MOI ho so 1 file (envelope KHAMSUCKHOE
        /// SOLUONGHOSO=1). Goi API nap du lieu chi tiet BATCH 1 lan (BuildInputs), roi build tung envelope.
        /// XML KHONG mask chu ky (dung BuildEnvelope). Tra so file xuat thanh cong; failed = so ho so loi.
        /// </summary>
        internal int ExportXmlFiles(IEnumerable<V_HIS_KSK_SYNC> rows, string dirPath, out int failed, out string error)
        {
            failed = 0; error = null;
            List<V_HIS_KSK_SYNC> rowList = (rows != null) ? rows.Where(r => r != null).ToList() : new List<V_HIS_KSK_SYNC>();
            if (rowList.Count == 0) return 0;
            int ok = 0;
            try
            {
                // File XML xuat ra dung KskEnvelopeBuilder de co DU 12 khoi (khoi thieu du lieu -> khoi trong).
                Qd1551Config exportConfig = BuildConfig();
                string exportMacskcb = (exportConfig != null) ? (exportConfig.SenderId ?? "") : "";
                bool exportAsJson = exportConfig != null && exportConfig.IsJson();
                List<Qd1551KskInput> inputs = BuildInputs(rowList);   // 1 lan nap batch (1:1 voi rowList)
                bool doSign = this.sign && this.signSetting != null;
                // Tich ky so nhung KHONG co cau hinh chung thu -> DUNG. Neu xuat tiep se ra file CKS_ trong
                // ma nguoi dung van tuong da ky (truoc day im lang di nhanh khong ky).
                if (this.sign && this.signSetting == null)
                {
                    error = "Bạn đã bật Ký số nhưng chưa cấu hình chứng thư/chữ ký số. "
                          + "Vui lòng cấu hình (bỏ tích rồi tích lại nút Ký số) trước khi xuất XML.";
                    return 0;
                }
                var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                // === KY SO BAT: gom ho so theo NGUOI KET LUAN, moi nhom -> 1 file (nhieu ho so cung nguoi),
                //     ky CKS_NGUOI_KET_LUAN 1 lan cho ca file. Chan neu co ho so thieu nguoi ket luan. ===
                if (doSign)
                {
                    // Chan: moi ho so PHAI co nguoi ket luan.
                    var missing = rowList.Where(r => string.IsNullOrEmpty(SafeString(GetProp(r, "CONCLUDER_LOGINNAME")))).ToList();
                    if (missing.Count > 0)
                    {
                        error = "Không thể ký số: có " + missing.Count + " hồ sơ CHƯA CÓ NGƯỜI KẾT LUẬN. "
                              + "Vui lòng kết luận đầy đủ trước khi ký/xuất. Mã điều trị: "
                              + string.Join(", ", missing.Select(r => SafeString(GetProp(r, "TDL_TREATMENT_CODE")))
                                                          .Where(x => !string.IsNullOrEmpty(x)).Take(30));
                        return 0;   // DUNG, khong xuat file nao
                    }

                    KskSyncSigner signer = new KskSyncSigner(this.signSetting);
                    var concSigners = FetchConcluderSigners(rowList);
                    LogSignScope(rowList, concSigners, "Xuat XML KSK");

                    // Gom CHI SO dong theo concluder_loginname (giu thu tu).
                    var groups = new Dictionary<string, List<int>>(StringComparer.OrdinalIgnoreCase);
                    var order = new List<string>();
                    for (int i = 0; i < rowList.Count; i++)
                    {
                        string cl = SafeString(GetProp(rowList[i], "CONCLUDER_LOGINNAME"));
                        List<int> idxs;
                        if (!groups.TryGetValue(cl, out idxs)) { idxs = new List<int>(); groups[cl] = idxs; order.Add(cl); }
                        idxs.Add(i);
                    }

                    foreach (string concLogin in order)
                    {
                        List<int> idxs = groups[concLogin];
                        try
                        {
                            var groupInputs = idxs.Where(i => i < inputs.Count && inputs[i] != null).Select(i => inputs[i]).ToList();
                            if (groupInputs.Count == 0) { failed += idxs.Count; continue; }
                            // 1 envelope chua TAT CA ho so cung nguoi ket luan (SOLUONGHOSO = so ho so nhom).
                            string xml = KskEnvelopeBuilder.Build(groupInputs, exportMacskcb, exportAsJson);
                            if (string.IsNullOrEmpty(xml)) { failed += idxs.Count; continue; }

                            // Ky CKS_NGUOI_KET_LUAN (chung thu nguoi ket luan cua nhom) -> roi ky CKS_BENH_VIEN.
                            EMR.EFMODEL.DataModels.EMR_SIGNER concEmr;
                            if (concSigners.TryGetValue(concLogin, out concEmr))
                                xml = signer.SignXmlByConcluder(xml, concEmr);
                            xml = signer.SignCksBenhVien(xml);
                            if (string.IsNullOrEmpty(xml)) { failed += idxs.Count; continue; }

                            string baseName = MakeGroupFileName(concLogin, idxs.Count);
                            string name = baseName; int k = 1;
                            while (used.Contains(name)) { name = baseName + "_" + (++k); }
                            used.Add(name);
                            System.IO.File.WriteAllText(System.IO.Path.Combine(dirPath, name + ".xml"), xml, new System.Text.UTF8Encoding(false));
                            ok += idxs.Count;   // dem theo so ho so trong file
                        }
                        catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); failed += idxs.Count; }
                    }
                    return ok;
                }

                // === KY SO TAT: giu hanh vi cu — 1 file / 1 ho so, khong ky. ===
                for (int i = 0; i < rowList.Count; i++)
                {
                    try
                    {
                        Qd1551KskInput inp = (i < inputs.Count) ? inputs[i] : null;
                        if (inp == null) { failed++; continue; }
                        string xml = KskEnvelopeBuilder.Build(new List<Qd1551KskInput> { inp }, exportMacskcb, exportAsJson);
                        if (string.IsNullOrEmpty(xml)) { failed++; continue; }

                        string baseName = MakeExportFileName(rowList[i]);
                        string name = baseName; int k = 1;
                        while (used.Contains(name)) { name = baseName + "_" + (++k); }
                        used.Add(name);
                        System.IO.File.WriteAllText(System.IO.Path.Combine(dirPath, name + ".xml"), xml, new System.Text.UTF8Encoding(false));
                        ok++;
                    }
                    catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); failed++; }
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); error = ex.Message; }
            return ok;
        }

        /// <summary>Ten file XML xuat theo NHOM nguoi ket luan: KSK_&lt;loginname&gt;_&lt;so ho so&gt;hs.</summary>
        private static string MakeGroupFileName(string concluderLogin, int count)
        {
            string s = "KSK_" + (string.IsNullOrEmpty(concluderLogin) ? "NOCONCLUDER" : concluderLogin) + "_" + count + "hs";
            foreach (char c in System.IO.Path.GetInvalidFileNameChars()) s = s.Replace(c, '_');
            return s;
        }

        /// <summary>Ten file XML xuat = MaBN_MaDot_MaHoSo (bo ky tu khong hop le); trong -> KSK_MaHoSo.</summary>
        private static string MakeExportFileName(V_HIS_KSK_SYNC row)
        {
            string pat = SafeString(GetProp(row, "TDL_PATIENT_CODE"));
            string tre = SafeString(GetProp(row, "TDL_TREATMENT_CODE"));
            string rid = SafeString(GetProp(row, "KSK_RECORD_ID"));
            string s = string.Join("_", new[] { pat, tre, rid }.Where(x => !string.IsNullOrEmpty(x)).ToArray());
            if (string.IsNullOrEmpty(s)) s = "KSK_" + rid;
            foreach (char c in System.IO.Path.GetInvalidFileNameChars()) s = s.Replace(c, '_');
            return string.IsNullOrEmpty(s) ? "KSK" : s;
        }

        /// <summary>
        /// Kiểm tra MỌI hồ sơ đều có người kết luận (CONCLUDER_LOGINNAME) — điều kiện để ký CKS_NGUOI_KET_LUAN
        /// theo nhóm. Trả false + thông báo (liệt kê mã điều trị thiếu) nếu có hồ sơ chưa có người kết luận.
        /// </summary>
        internal static bool AllHaveConcluder(IEnumerable<V_HIS_KSK_SYNC> rows, out string message)
        {
            message = null;
            if (rows == null) return true;
            var missing = rows.Where(r => r != null && string.IsNullOrEmpty(SafeString(GetProp(r, "CONCLUDER_LOGINNAME")))).ToList();
            if (missing.Count == 0) return true;
            message = "Không thể ký số: có " + missing.Count + " hồ sơ CHƯA CÓ NGƯỜI KẾT LUẬN. "
                    + "Vui lòng kết luận đầy đủ trước khi đồng bộ/xuất. Mã điều trị: "
                    + string.Join(", ", missing.Select(r => SafeString(GetProp(r, "TDL_TREATMENT_CODE")))
                                                .Where(x => !string.IsNullOrEmpty(x)).Take(30));
            return false;
        }

        /// <summary>Kết quả LƯU trạng thái của lần PushList gần nhất (UI đọc để báo).</summary>
        internal bool SaveAllOk { get; private set; }
        internal string SaveError { get; private set; }

        /// <summary>
        /// Nạp dữ liệu BATCH (nhiều hồ sơ), ĐẨY TỪNG HỒ SƠ 1 (mỗi hồ sơ 1 envelope, 1 mã giao dịch),
        /// gom kết quả rồi LƯU TRẠNG THÁI 1 LẦN (batch List&lt;HIS_KSK_SYNC&gt; — API cho phép nhiều hồ sơ).
        /// Đăng nhập dùng CHUNG (token cache 15 phút, tự login lại khi 401/CM_AUTH_EXPIRED) vì tái sử dụng
        /// 1 instance CreateQd1551Main.
        /// </summary>
        internal List<KskSyncResultADO> PushList(IEnumerable<V_HIS_KSK_SYNC> rows, long syncTime)
        {
            List<KskSyncResultADO> results = new List<KskSyncResultADO>();
            List<HIS_KSK_SYNC> saveList = new List<HIS_KSK_SYNC>();   // gom -> LƯU 1 LẦN (batch)
            this.SaveAllOk = true; this.SaveError = null;
            if (rows == null) return results;

            List<V_HIS_KSK_SYNC> rowList = rows.Where(r => r != null).ToList();
            if (rowList.Count == 0) return results;

            try
            {
                // Parse cấu hình cổng BYT (giữ bản parse để log; parse lỗi -> config rỗng như hành vi cũ).
                Qd1551Config bytConfig = Qd1551ConfigParser.Parse(this.connectionInfo, null);
                CreateQd1551Main main = new CreateQd1551Main(bytConfig ?? new Qd1551Config());   // 1 instance -> token cache dùng chung
                X509Certificate2 certificate = LoadCertificate();
                List<Qd1551KskInput> inputs = BuildInputs(rowList);            // nạp batch (nhiều hồ sơ)

                Qd1551Config hsskConfig = null;
                if (this.pushHssk && !string.IsNullOrWhiteSpace(this.hsskConnectionInfo))
                    hsskConfig = Qd1551ConfigParser.Parse(this.hsskConnectionInfo, null);

                HocConfig hocConfig = null;
                if (this.pushHoc && !string.IsNullOrWhiteSpace(this.hocConnectionInfo))
                    hocConfig = HocConfigParser.Parse(this.hocConnectionInfo);

                // Cổng HCC: cùng giao thức trục BYT nhưng data_type mặc định json/base64 -> payload RIÊNG,
                // dựng bằng 1 instance CreateQd1551Main theo cấu hình HCC; token cache trong 1 KskHccPusher.
                Qd1551Config hccConfig = BuildHccConfig();
                string hccMacskcb = (hccConfig != null) ? (hccConfig.SenderId ?? "") : "";
                KskHccPusher hccPusher = (hccConfig != null) ? new KskHccPusher(hccConfig) : null;

                // Các cổng do THƯ VIỆN đẩy (BYT/HSSK/HOC). Không có cổng nào -> KHÔNG gọi PushListMulti
                // (gọi rỗng sẽ trả về "thành công" giả vì không cổng nào đánh dấu thất bại).
                bool pushViaLibrary = this.pushByt || hsskConfig != null || hocConfig != null;
                int libGatewayCount = (this.pushByt ? 1 : 0) + (hsskConfig != null ? 1 : 0) + (hocConfig != null ? 1 : 0);
                string libSingleLabel = (libGatewayCount == 1)
                    ? (this.pushByt ? "BYT" : (hsskConfig != null ? "HSSK" : "HOC"))
                    : null;

                // Ghi log giá trị cấu hình TỪNG CỔNG vừa lấy được (mật khẩu / khóa bí mật đã mask).
                LogGatewayConfigs(bytConfig, hsskConfig, hocConfig, hccConfig);

                // Cổng ĐÃ CHỌN + CÓ chuỗi cấu hình nhưng PARSE LỖI (sai định dạng) -> KHÔNG bỏ qua âm thầm:
                // ghi nhận để đánh dấu hồ sơ thất bại kèm lý do (nếu không, hồ sơ vẫn "thành công" nhờ cổng khác).
                var configErrorList = new List<string>();
                if (this.pushHssk && !string.IsNullOrWhiteSpace(this.hsskConnectionInfo) && hsskConfig == null)
                    configErrorList.Add("HSSK: chuỗi cấu hình sai định dạng (MOS.HIS_KSK_SYNC.HSSK_HN_2062_CONNECTION_INFO)");
                if (this.pushHoc && !string.IsNullOrWhiteSpace(this.hocConnectionInfo) && hocConfig == null)
                    configErrorList.Add("HOC: chuỗi cấu hình sai định dạng (MOS.HIS_KSK_SYNC.HSSK_HOC_2062_CONNECTION_INFO)");
                if (this.pushHcc && !string.IsNullOrWhiteSpace(this.hccConnectionInfo) && hccConfig == null)
                    configErrorList.Add("HCC: chuỗi cấu hình sai định dạng (MOS.HIS_KSK_SYNC.HSSK_HCC_2062_CONNECTION_INFO)");
                string configError = (configErrorList.Count > 0) ? string.Join(" | ", configErrorList.ToArray()) : null;

                // KÝ SỐ (CKS_NGUOI_KET_LUAN + CKS_BENH_VIEN): TÍCH ký số là ký, KHÔNG phụ thuộc cổng nào được
                // chọn. Bản tin XML base64 dùng CHUNG cho BYT/HSSK/HOC nên ký 1 lần là cả 3 cổng đều có CKS_
                // (trước đây chỉ ký khi có BYT -> đẩy riêng HSSK/HOC ra bản tin KHÔNG chữ ký).
                // Ngoại lệ duy nhất: cổng HCC cấu hình json/base64 — chữ ký chèn theo THẺ XML nên không ký được.
                bool hccIsJson = hccConfig != null && hccConfig.IsJson();     // mac dinh json/base64 theo tai lieu HCC
                bool signXmlForHcc = hccConfig != null && !hccIsJson;
                bool doSign = this.sign && this.signSetting != null;
                KskSyncSigner signer = doSign ? new KskSyncSigner(this.signSetting) : null;
                Dictionary<string, EMR.EFMODEL.DataModels.EMR_SIGNER> concSigners =
                    doSign ? FetchConcluderSigners(rowList) : null;
                if (doSign)
                {
                    LogSignScope(rowList, concSigners, "Dong bo KSK");
                    if (hccIsJson)
                        Inventec.Common.Logging.LogSystem.Warn("Dong bo KSK: cong HCC cau hinh json/base64 -> KHONG ky duoc"
                            + " CKS_ (chu ky chen theo the XML). Ban tin day sang HCC se de trong CHUKYDONVI."
                            + " Doi cau hinh HCC sang xml/base64 neu can chu ky.");
                }

                // Ghi log CHỐT danh sách cổng thực sự đẩy của lần bấm này (để đối soát khi có nhiều cổng).
                var gateways = new List<string>();
                if (this.pushByt) gateways.Add("BYT");
                if (hsskConfig != null) gateways.Add("HSSK");
                if (hocConfig != null) gateways.Add("HOC");
                if (hccConfig != null) gateways.Add("HCC" + (hccIsJson ? "(json)" : "(xml)"));
                Inventec.Common.Logging.LogSystem.Info(string.Format(
                    "Dong bo KSK: {0} ho so -> cong: {1}; ky so: {2}{3}",
                    rowList.Count,
                    (gateways.Count > 0) ? string.Join(", ", gateways.ToArray()) : "(khong co cong nao)",
                    doSign ? "co" : "khong",
                    (configError != null) ? ("; LOI CAU HINH: " + configError) : ""));

                // ĐẨY 1 HỒ SƠ / LẦN; gom kết quả (hiển thị + entity lưu). LƯU 1 LẦN sau vòng lặp.
                for (int i = 0; i < rowList.Count; i++)
                {
                    KskSyncResultADO ado;
                    try
                    {
                        Qd1551KskInput inp = (i < inputs.Count) ? inputs[i] : null;
                        if (inp == null)
                        {
                            ado = BuildFailedResult(rowList[i], syncTime, "Không dựng được dữ liệu hồ sơ");
                        }
                        else
                        {
                            Func<string, string> dataSigner = null;
                            if (signer != null)
                            {
                                EMR.EFMODEL.DataModels.EMR_SIGNER concEmr = null;
                                string cl = SafeString(GetProp(rowList[i], "CONCLUDER_LOGINNAME"));
                                if (concSigners != null && !string.IsNullOrEmpty(cl)) concSigners.TryGetValue(cl, out concEmr);
                                EMR.EFMODEL.DataModels.EMR_SIGNER emrLocal = concEmr;
                                // Ký CKS_NGUOI_KET_LUAN (chứng thư người kết luận) TRƯỚC, rồi CKS_BENH_VIEN.
                                dataSigner = xml => signer.SignCksBenhVien(emrLocal != null ? signer.SignXmlByConcluder(xml, emrLocal) : xml);
                            }
                            // ĐẨY ĐÚNG 1 HỒ SƠ.
                            ResultADO r0 = null;
                            if (pushViaLibrary)
                            {
                                // dataSigner ap cho CHUOI XML DUNG CHUNG cua BYT/HSSK/HOC -> tich ky so la
                                // cong nao cung nhan ban tin da ky (khong con phu thuoc co tich BYT hay khong).
                                List<ResultADO> pr = main.PushListMulti(new List<Qd1551KskInput> { inp }, certificate,
                                    dataSigner, this.pushByt, hsskConfig, hocConfig);
                                r0 = (pr != null && pr.Count > 0) ? pr[0] : null;
                            }
                            // Cổng HCC (nếu chọn) — payload riêng theo data_type của cấu hình HCC.
                            KskHccPushResult hccResult = null;
                            if (hccPusher != null)
                                hccResult = hccPusher.Push(BuildHccPayload(hccMacskcb, inp, hccIsJson,
                                    signXmlForHcc ? dataSigner : null));

                            ado = BuildResultAdo(rowList[i], r0, hccResult, syncTime, libSingleLabel, configError);
                        }
                    }
                    catch (Exception exRow)
                    {
                        Inventec.Common.Logging.LogSystem.Error(exRow);
                        ado = BuildFailedResult(rowList[i], syncTime, exRow.Message);
                    }

                    results.Add(ado);
                    saveList.Add(BuildSyncEntity(rowList[i], ado));   // gom entity -> lưu batch cuối
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                // Lỗi chung: hồ sơ nào chưa có kết quả -> đánh dấu thất bại để vẫn lưu đủ trạng thái.
                for (int i = results.Count; i < rowList.Count; i++)
                {
                    var ado = BuildFailedResult(rowList[i], syncTime, ex.Message);
                    results.Add(ado);
                    saveList.Add(BuildSyncEntity(rowList[i], ado));
                }
            }

            // LƯU 1 LẦN toàn bộ trạng thái (batch) — input List<HIS_KSK_SYNC>, KHÔNG gọi từng dòng.
            if (saveList.Count > 0 && !SaveResults(saveList))
            {
                this.SaveAllOk = false;
                if (string.IsNullOrEmpty(this.SaveError)) this.SaveError = "Lưu trạng thái đồng bộ thất bại (xem log).";
            }
            return results;
        }

        /// <summary>
        /// Ánh xạ V_HIS_KSK_SYNC (view lưới) + kết quả đẩy -> HIS_KSK_SYNC (entity lưu). Backend upsert theo
        /// (KSK_TYPE_ID, KSK_RECORD_ID). Điền đủ trường: khóa + FK điều trị/y lệnh + kết quả đồng bộ.
        /// </summary>
        private static HIS_KSK_SYNC BuildSyncEntity(V_HIS_KSK_SYNC row, KskSyncResultADO ado)
        {
            var ent = new HIS_KSK_SYNC
            {
                KSK_TYPE_ID = (short)ToLong(GetProp(row, "KSK_TYPE_ID")),
                KSK_RECORD_ID = ToLong(GetProp(row, "KSK_RECORD_ID")),
                SYNC_RESULT_TYPE = ado.SYNC_RESULT_TYPE,
                SYNC_TIME = ado.SYNC_TIME,
                TRANSACTION_CODE = ado.TRANSACTION_CODE,
                SYNC_FAILD_REASON = ado.SYNC_FAILD_REASON,
                REGISTRATION_NO = ado.REGISTRATION_NO
            };
            long treaId = ToLong(GetProp(row, "TDL_TREATMENT_ID"));
            if (treaId > 0) ent.TDL_TREATMENT_ID = treaId;
            long sreqId = ToLong(GetProp(row, "SERVICE_REQ_ID"));
            if (sreqId > 0) ent.TDL_SERVICE_REQ_ID = sreqId;
            return ent;
        }

        /// <summary>
        /// Lưu trạng thái đồng bộ CHO NHIỀU HỒ SƠ trong 1 lần gọi (POST cả danh sách HIS_KSK_SYNC).
        /// Trả false nếu backend báo lỗi / lưu 0 dòng.
        /// </summary>
        private bool SaveResults(List<HIS_KSK_SYNC> list)
        {
            try
            {
                if (list == null || list.Count == 0) return true;
                var param = new CommonParam();
                int saved = new BackendAdapter(param).Post<int>("api/HisKskSync/SaveSyncResult",
                    ApiConsumers.MosConsumer, list,
                    HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                bool hasErr = param != null && param.Messages != null && param.Messages.Count > 0;
                if (saved <= 0 || hasErr)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Luu trang thai lo ho so KSK that bai: "
                        + (hasErr ? string.Join("; ", param.Messages) : "backend tra 0"));
                    return false;
                }
                return true;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); return false; }
        }

        /// <summary>
        /// Dung day du Qd1551KskInput cho danh sach ho so (cac dong tich chon). Goi API BATCH theo danh sach ID
        /// (SERVICE_REQ_IDs / TREATMENT_IDs / KSK_*_IDs) va SONG SONG (Task.WaitAll) — KHONG for goi tung ho so mot.
        /// Sau do index theo khoa va gan vao tung Qd1551KskInput. Chay tren tien trinh nen cua PushList.
        /// XML1/XML2 (hanh chinh + lan kham) do THU VIEN tu dung tu Patient/Treatment/KSK entity
        /// + MaCskcb/MaGtinCskcb/MaLoaiKcb — plugin khong con dung Admin1/Admin2 thu cong.
        /// </summary>
        // ===== TEMP FAKE: true = sinh XML tu DU LIEU GIA (test), KHONG doc DB. false = du lieu THAT (doc DB). =====
        internal const bool USE_FAKE_DATA = false;

        private List<Qd1551KskInput> BuildInputs(List<V_HIS_KSK_SYNC> rowList)
        {
            if (USE_FAKE_DATA) return BuildFakeInputsFor(rowList);   // TEMP FAKE — map theo SERVICE_REQ_ID cua dong

            var inputs = new List<Qd1551KskInput>();
            if (rowList == null || rowList.Count == 0) return inputs;

            List<long> serviceReqIds = rowList.Select(r => ToLong(GetProp(r, "SERVICE_REQ_ID"))).Where(x => x > 0).Distinct().ToList();
            List<long> treatmentIds = rowList.Select(r => ToLong(GetProp(r, "TDL_TREATMENT_ID"))).Where(x => x > 0).Distinct().ToList();

            // .NET Framework mac dinh CHI cho 2 ket noi HTTP/host -> nang gioi han cho cac call song song.
            if (System.Net.ServicePointManager.DefaultConnectionLimit < 20)
                System.Net.ServicePointManager.DefaultConnectionLimit = 20;

            // === DOT 1 (song song): 1 CALL GOP api/HisKskSync/GetKskData (input/output nhu EnterKskVer2) ===
            // HisKskDataSDO bung du du lieu KSC 1 luot cho CA LIST ho so: General/UnderSix/Under18/Over18/DHST/
            // Treatment + UneiVaty(tiem chung) + PeriodDriverDity(tien su) + VaccineType + DiseaseType.
            // Cac du lieu NGOAI pham vi KSK (CLS, doi tuong dieu tri) van goi rieng theo TREATMENT_IDs (song song).
            MOS.SDO.HisKskDataSDO sdo = null;
            List<V_HIS_SERE_SERV_2> clsSereServs = null;
            List<V_HIS_SERE_SERV_TEIN> clsTeins = null;
            List<V_HIS_SERE_SERV_SUIN> clsSuins = null;
            List<HIS_SERE_SERV_EXT> clsExts = null;
            List<V_HIS_PATIENT_TYPE_ALTER> patientTypeAlters = null;
            List<HIS_SERVICE_REQ> serviceReqs = null;

            var tasks = new List<System.Threading.Tasks.Task>();
            if (serviceReqIds.Count > 0 || treatmentIds.Count > 0)
                tasks.Add(System.Threading.Tasks.Task.Factory.StartNew(() =>
                    sdo = GetKskDataSdo(new MOS.Filter.HisKskDataFilter
                    {
                        SERVICE_REQ_IDs = serviceReqIds,
                        TREATMENT_IDs = treatmentIds,
                        IS_ACTIVE = 1
                    })));
            // Y lenh KSK (HIS_SERVICE_REQ): nguon cua LY_DO_VV (XML1) — o "Ly do kham" tren man nhap KSK
            // duoc luu tai HIS_SERVICE_REQ.HOSPITALIZATION_REASON (khong phai HIS_TREATMENT).
            if (serviceReqIds.Count > 0)
                tasks.Add(System.Threading.Tasks.Task.Factory.StartNew(() =>
                    serviceReqs = GetList<HIS_SERVICE_REQ>("api/HisServiceReq/Get", new HisServiceReqFilter { IDs = serviceReqIds })));

            if (treatmentIds.Count > 0)
            {
                // CLS (XML11): dich vu can lam sang DA THUC HIEN theo dot dieu tri (bo loai chuan nhu
                // TreatmentList: XN/CDHA/NS/SA/TDCN) + chi so xet nghiem (TEIN) + ket qua mo ta/ket luan (EXT). 
                // Lay tu VIEW V_HIS_SERE_SERV_2 (giong XML130) — de TDL_HEIN_SERVICE_BHYT_CODE/NAME (MA/TEN_DICH_VU)
                // duoc dien chuan theo BHYT. Loc loai dich vu CLS (CDHA/TDCN/XN) thuc hien trong BuildClsByTreatment.  
                tasks.Add(System.Threading.Tasks.Task.Factory.StartNew(() =>
                    clsSereServs = GetList<V_HIS_SERE_SERV_2>("api/HisSereServ/GetView2", new HisSereServView2Filter
                    {
                        TREATMENT_IDs = treatmentIds,
                        HAS_EXECUTE = true
                    })));
                tasks.Add(System.Threading.Tasks.Task.Factory.StartNew(() =>
                    clsExts = GetList<HIS_SERE_SERV_EXT>("api/HisSereServExt/Get", new HisSereServExtFilter { TDL_TREATMENT_IDs = treatmentIds })));
                // Dien doi tuong hien tai cua dot dieu tri — phuc vu suy MA_LOAI_KCB=100 (doi tuong KSK, nhu XML130).
                tasks.Add(System.Threading.Tasks.Task.Factory.StartNew(() =>
                    patientTypeAlters = GetList<V_HIS_PATIENT_TYPE_ALTER>("/api/HisPatientTypeAlter/GetView", new HisPatientTypeAlterViewFilter { TREATMENT_IDs = treatmentIds })));
            }
            if (tasks.Count > 0) System.Threading.Tasks.Task.WaitAll(tasks.ToArray());

            // Log SAU WaitAll (truoc do task chay nen chua gan xong -> sdo con null). 
            LogKskDataSdo(sdo, serviceReqIds, treatmentIds);

            // Bung du lieu KSK tu SDO (null-safe). Loi call gop -> tat ca null -> input rong (khong sai du lieu).
            List<HIS_KSK_GENERAL> generals = (sdo != null) ? sdo.HisKskGenerals : null;
            List<HIS_KSK_UNDER_SIX> underSixes = (sdo != null) ? sdo.HisKskUnderSixs : null;
            List<HIS_KSK_UNDER_EIGHTEEN> under18s = (sdo != null) ? sdo.HisKskUnderEighteens : null;
            List<HIS_KSK_OVER_EIGHTEEN> over18s = (sdo != null) ? sdo.HisKskOverEighteens : null;
            List<HIS_DHST> dhsts = (sdo != null) ? sdo.HisDhsts : null;
            List<HIS_TREATMENT> treatments = (sdo != null) ? sdo.HisTreatments : null;
            List<HIS_KSK_UNEI_VATY> vatys = (sdo != null) ? sdo.HisKskUneiVatys : null;
            List<HIS_PERIOD_DRIVER_DITY> ditys = (sdo != null) ? sdo.HisPeriodDriverDitys : null;
            List<HIS_VACCINE_TYPE> vaccineTypes = (sdo != null) ? sdo.HisVaccineTypes : null;
            List<HIS_DISEASE_TYPE> diseaseTypes = (sdo != null) ? sdo.HisDiseaseTypes : null;

            // === DOT 2 (phu thuoc treatments/KSK entity tu SDO): benh nhan (XML1) + ngoai tru man tinh
            //     (MA_LOAI_KCB 05/08) + chu ky dien tu bac si (emr_signer.SIGN_IMAGE theo LOGINNAMEs). ===
            List<HIS_PATIENT> patients = null;
            List<HIS_SERE_SERV> chronicSereServs = null;
            List<EMR.EFMODEL.DataModels.EMR_SIGNER> emrSigners = null;
            var tasks2 = new List<System.Threading.Tasks.Task>();
            List<long> patientIds = (treatments != null) ? treatments.Select(t => t.PATIENT_ID).Distinct().ToList() : new List<long>();
            List<string> loginnames = CollectLoginnames(underSixes, under18s, over18s, generals, dhsts, clsExts);
            List<long> chronicTrIds = (treatments != null)
                ? treatments.Where(t => t != null && (t.TDL_TREATMENT_TYPE_ID ?? 0) == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU && t.IS_CHRONIC == 1)
                            .Select(t => t.ID).Distinct().ToList()
                : new List<long>();
            // CLS (XML11) — lay theo BHYT giong XML130 XML4: chi so xet nghiem (TEIN) + chi so CDHA/TDCN (SUIN).
            // Loc theo SERE_SERV_IDs (mau proven tu ContentSubclinical; loc theo TREATMENT view GetView khong tra dung).
            List<long> clsSsIds = (clsSereServs != null)
                ? clsSereServs.Where(s => s != null).Select(s => s.ID).Distinct().ToList()
                : new List<long>();
            if (clsSsIds.Count > 0)
            {
                tasks2.Add(System.Threading.Tasks.Task.Factory.StartNew(() =>
                    clsTeins = GetList<V_HIS_SERE_SERV_TEIN>("api/HisSereServTein/GetView",
                        new HisSereServTeinViewFilter { SERE_SERV_IDs = clsSsIds, IS_ACTIVE = 1 })));
                tasks2.Add(System.Threading.Tasks.Task.Factory.StartNew(() =>
                    clsSuins = GetList<V_HIS_SERE_SERV_SUIN>("api/HisSereServSuin/GetView",
                        new HisSereServSuinViewFilter { SERE_SERV_IDs = clsSsIds, IS_ACTIVE = 1 })));
            }
            if (patientIds.Count > 0)
                tasks2.Add(System.Threading.Tasks.Task.Factory.StartNew(() =>
                    patients = GetList<HIS_PATIENT>("api/HisPatient/Get", new HisPatientFilter { IDs = patientIds })));
            if (chronicTrIds.Count > 0)
                tasks2.Add(System.Threading.Tasks.Task.Factory.StartNew(() =>
                    chronicSereServs = GetList<HIS_SERE_SERV>("api/HisSereServ/Get", new HisSereServFilter { TREATMENT_IDs = chronicTrIds })));
            if (loginnames.Count > 0)
                tasks2.Add(System.Threading.Tasks.Task.Factory.StartNew(() =>
                    emrSigners = GetList<EMR.EFMODEL.DataModels.EMR_SIGNER>("api/EmrSigner/Get",
                        new EMR.Filter.EmrSignerFilter { LOGINNAMEs = loginnames, IS_ACTIVE = 1 }, ApiConsumers.EmrConsumer)));
            if (tasks2.Count > 0) System.Threading.Tasks.Task.WaitAll(tasks2.ToArray());

            // Chu ky: loginname -> base64(SIGN_IMAGE) — thu vien (Qd1551SignResolver) dien vao cac the CKDT_.
            Dictionary<string, string> signImageByLogin = BuildSignMap(emrSigners);
            // CLS: dung List<Qd1551ClsRow> theo TREATMENT_ID (XN: 1 dong/chi so TEIN; CDHA/NS/SA/TDCN: 1 dong/dich vu).
            Dictionary<long, List<Qd1551ClsRow>> clsByTr = BuildClsByTreatment(clsSereServs, clsTeins, clsSuins, clsExts);

            List<HIS_HEALTH_EXAM_RANK> ranks = null;
            // Chi lay rank ACTIVE (giong EnterKskInfomantionVer2) — de thu tu/vi tri phan loai khop UI.
            try { ranks = BackendDataWorker.Get<HIS_HEALTH_EXAM_RANK>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }

            var genBySr = IndexBy(generals, g => g.SERVICE_REQ_ID);
            var u6BySr = IndexBy(underSixes, x => x.SERVICE_REQ_ID);
            var u18BySr = IndexBy(under18s, x => x.SERVICE_REQ_ID);
            var o18BySr = IndexBy(over18s, x => x.SERVICE_REQ_ID);
            var dhstById = IndexBy(dhsts, d => d.ID);            // DHST theo ID (chinh xac tung ban ghi KSK)
            var dhstByTr = GroupByKey(dhsts, d => d.TREATMENT_ID); // fallback theo dot dieu tri
            var treaById = IndexBy(treatments, t => t.ID);
            var sreqById = IndexBy(serviceReqs, s => s.ID);       // y lenh KSK theo SERVICE_REQ_ID (LY_DO_VV)
            var patById = IndexBy(patients, p => p.ID);
            var vatyByU18 = GroupByKey(vatys, v => v.KSK_UNDER_EIGHTEEN_ID);
            var dityByO18 = GroupByKey(ditys, d => d.KSK_OVER_EIGHTEEN_ID ?? 0);

            // Danh muc chi nhanh (cache local) — MA_CSKCB = HEIN_MEDI_ORG_CODE theo BRANCH_ID.
            Dictionary<long, HIS_BRANCH> branchById = null;
            try { branchById = IndexBy(BackendDataWorker.Get<HIS_BRANCH>(), b => b.ID); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            // Ma GTIN/GLN co so — SenderId trong CONNECTION_INFO (BYT); neu rong -> fallback SenderId cong
            // HSSK, roi cong HCC (deu la ma don vi 13 so).
            string maGtinCskcb = "";
            try
            {
                var cfg = BuildConfig();
                string sid = (cfg != null) ? cfg.SenderId : null;
                if (string.IsNullOrWhiteSpace(sid) && !string.IsNullOrWhiteSpace(this.hsskConnectionInfo))
                {
                    var h = Qd1551ConfigParser.Parse(this.hsskConnectionInfo, null);
                    if (h != null && !string.IsNullOrWhiteSpace(h.SenderId)) sid = h.SenderId;
                }
                if (string.IsNullOrWhiteSpace(sid) && !string.IsNullOrWhiteSpace(this.hccConnectionInfo))
                {
                    var c = KskHccConfigParser.Parse(this.hccConnectionInfo);   // MaCsyt = ma don vi 13 so
                    if (c != null && !string.IsNullOrWhiteSpace(c.SenderId)) sid = c.SenderId;
                }
                maGtinCskcb = sid ?? "";
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            // Ma doi tuong KSK (config) — dung cho quy tac MA_LOAI_KCB=100 nhu XML130.
            string keyKsk = "";
            try
            {
                var cfgKsk = BackendDataWorker.Get<HIS_CONFIG>().FirstOrDefault(o => o.KEY == "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.KSK");
                if (cfgKsk != null) keyKsk = cfgKsk.VALUE ?? "";
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            var alterByTr = GroupByKey(patientTypeAlters, a => a.TREATMENT_ID);
            var chronicServByTr = GroupByKey(chronicSereServs, x => x.TDL_TREATMENT_ID ?? 0);

            foreach (var row in rowList)
            {
                long sr = ToLong(GetProp(row, "SERVICE_REQ_ID"));
                long tr = ToLong(GetProp(row, "TDL_TREATMENT_ID"));

                HIS_KSK_GENERAL general = ValOrNull(genBySr, sr);
                HIS_KSK_UNDER_SIX underSix = ValOrNull(u6BySr, sr);
                HIS_KSK_UNDER_EIGHTEEN under18 = ValOrNull(u18BySr, sr);
                HIS_KSK_OVER_EIGHTEEN over18 = ValOrNull(o18BySr, sr);

                // DHST: uu tien lay DUNG ban ghi theo DHST_ID cua ho so KSK (khong lay nham sinh hieu cua lan do
                // khac cung TREATMENT). Neu ban ghi khong co DHST_ID -> fallback theo dot dieu tri.
                long dhstId = 0;
                if (underSix != null && underSix.DHST_ID.HasValue) dhstId = underSix.DHST_ID.Value;
                else if (under18 != null && under18.DHST_ID.HasValue) dhstId = under18.DHST_ID.Value;
                else if (over18 != null && over18.DHST_ID.HasValue) dhstId = over18.DHST_ID.Value;
                else if (general != null && general.DHST_ID.HasValue) dhstId = general.DHST_ID.Value;
                HIS_DHST dhstOne = ValOrNull(dhstById, dhstId);
                List<HIS_DHST> dhstForInput = (dhstOne != null) ? new List<HIS_DHST> { dhstOne } : ListOrNull(dhstByTr, tr);

                HIS_TREATMENT trea = ValOrNull(treaById, tr);
                HIS_BRANCH branch = (trea != null) ? ValOrNull(branchById, trea.BRANCH_ID) : null;

                Qd1551KskInput input = new Qd1551KskInput
                {
                    FormType = Qd1551FormMapper.ResolveFormType(ToLong(GetProp(row, "KSK_TYPE_ID"))),
                    // XML1/XML2: thu vien tu dung tu Patient + Treatment + KSK entity + 3 gia tri duoi day
                    Patient = (trea != null) ? ValOrNull(patById, trea.PATIENT_ID) : null,
                    MaCskcb = (branch != null) ? (branch.HEIN_MEDI_ORG_CODE ?? "") : "", // MA_CSKCB thật theo BRANCH_ID
                    MaGtinCskcb = maGtinCskcb,
                    MaLoaiKcb = ResolveMaLoaiKcb(trea, ListOrNull(chronicServByTr, tr), ListOrNull(alterByTr, tr), keyKsk),
                    General = general,
                    UnderSix = underSix,
                    UnderEighteen = under18,
                    OverEighteen = over18,
                    Dhst = dhstForInput,
                    Treatment = trea,
                    // LY_DO_VV (XML1) lay tu y lenh KSK; rong -> thu vien fallback ve Treatment (ho so cu).
                    ServiceReq = ValOrNull(sreqById, sr),
                    HealthExamRanks = ranks,
                    // Tiem chung 6-18 + danh muc vac-xin (mapper quy doi VACCINE_TYPE_CODE KSK01-07 -> the TIEM_CHUNG_*)
                    Vaccinations = (under18 != null) ? ListOrNull(vatyByU18, under18.ID) : null,
                    VaccineTypes = vaccineTypes,
                    // Tien su ban than >=18 (grid) + danh muc loai benh (mapper quy doi ma 01-22 -> the TSBT_*)
                    PersonalHistoryDity = (over18 != null) ? ListOrNull(dityByO18, over18.ID) : null,
                    DiseaseTypes = diseaseTypes,
                    // Chu ky dien tu bac si kham (CKDT_) + danh sach chi so CLS (XML11)
                    SignImageByLoginName = signImageByLogin,
                    ClsList = ListOrNull(clsByTr, tr)
                };
                inputs.Add(input);
                LogInputData(row, input);   // log nguon du lieu nap duoc -> biet khoi XML nao se sinh ra
            }
            return inputs;
        }

        /// <summary>
        /// Log ket qua call gop api/HisKskSync/GetKskData — PHAI goi SAU Task.WaitAll (goi truoc do thi
        /// task nen chua gan xong, sdo con null va cham vao sdo.XXX se nem NullReferenceException).
        /// In so ban ghi tung danh sach (null-safe) de biet ngay khoi XML nao se thieu du lieu nguon.
        /// </summary>
        private static void LogKskDataSdo(MOS.SDO.HisKskDataSDO sdo, List<long> serviceReqIds, List<long> treatmentIds)
        {
            try
            {
                if (sdo == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn(string.Format(
                        "GetKskData: KHONG co du lieu (sdo = null). SERVICE_REQ_IDs={0}; TREATMENT_IDs={1}."
                        + " Ca 2 danh sach rong -> khong goi API; nguoc lai -> API loi/tra null (xem log WebApiClient).",
                        Ids(serviceReqIds), Ids(treatmentIds)));
                    return;
                }
                Inventec.Common.Logging.LogSystem.Info(string.Format(
                    "GetKskData [SERVICE_REQ_IDs={0}; TREATMENT_IDs={1}]: General={2}; Duoi6={3}; Duoi18={4};"
                    + " Tren18={5}; DHST={6} -> XML3/XML10; Treatment={7}; TiemChung={8}; TienSuBenhTat={9};"
                    + " DmVacXin={10}; DmLoaiBenh={11}",
                    Ids(serviceReqIds), Ids(treatmentIds),
                    Count(sdo.HisKskGenerals), Count(sdo.HisKskUnderSixs), Count(sdo.HisKskUnderEighteens),
                    Count(sdo.HisKskOverEighteens), Count(sdo.HisDhsts), Count(sdo.HisTreatments),
                    Count(sdo.HisKskUneiVatys), Count(sdo.HisPeriodDriverDitys),
                    Count(sdo.HisVaccineTypes), Count(sdo.HisDiseaseTypes)));

                // Dump TOAN BO du lieu tra ve (chi khi bat DEBUG) — de soi tung ban ghi khi thieu du lieu.
                DumpDebug("GetKskData_sdo", sdo);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Ghi log DUMP day du 1 doi tuong (JSON qua LogUtil.TraceData) o muc DEBUG. Chi chay khi DEBUG bat
        /// (log rat dai). Loi serialize KHONG duoc lam hong luong day — nuot va canh bao.
        /// </summary>
        private static void DumpDebug(string name, object data)
        {
            try
            {
                if (!Inventec.Common.Logging.LogSystem.IsDebugEnabled()) return;
                Inventec.Common.Logging.LogSystem.Debug(LogUtil.TraceData(name, data));
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>So ban ghi cua danh sach; null -> "null" (phan biet ro "khong tra ve" vs "tra ve rong").</summary>
        private static string Count<T>(List<T> list)
        {
            return (list == null) ? "null" : list.Count.ToString();
        }

        /// <summary>Danh sach ID dang "1,2,3" (rong -> "(rong)").</summary>
        private static string Ids(List<long> ids)
        {
            if (ids == null || ids.Count == 0) return "(rong)";
            return string.Join(",", ids.Select(x => x.ToString()).ToArray());
        }

        /// <summary>
        /// Ghi log 1 dong / ho so: cac NGUON du lieu da nap duoc — quyet dinh khoi XML nao duoc sinh ra.
        /// Doi chieu nhanh khi cong bao thieu khoi:
        ///   XML1/XML2  &lt;- Patient + Treatment;      XML3 + XML10 &lt;- DHST (phai co CAN NANG);
        ///   XML7/XML9  &lt;- ban ghi KSK (General/Duoi6/Duoi18/Tren18);
        ///   XML11      &lt;- danh sach CLS;             phan_loai_sk &lt;- HEALTH_EXAM_RANK_ID (phai 1..5).
        /// </summary>
        private static void LogInputData(V_HIS_KSK_SYNC row, Qd1551KskInput input)
        {
            try
            {
                if (input == null) return;
                List<HIS_DHST> dhsts = input.Dhst;
                int dhstCount = (dhsts != null) ? dhsts.Count : 0;
                bool hasWeight = dhsts != null && dhsts.Exists(d => d != null && d.WEIGHT.HasValue);
                bool hasHeight = dhsts != null && dhsts.Exists(d => d != null && d.HEIGHT.HasValue);
                object rankId = GetProp(input.General, "HEALTH_EXAM_RANK_ID")
                             ?? GetProp(input.OverEighteen, "HEALTH_EXAM_RANK_ID")
                             ?? GetProp(input.UnderEighteen, "HEALTH_EXAM_RANK_ID");

                Inventec.Common.Logging.LogSystem.Info(string.Format(
                    "Du lieu KSK [MaDT={0}; SERVICE_REQ_ID={1}; KSK_RECORD_ID={2}]: FormType={3};"
                    + " Patient={4}; Treatment={5}; General={6}; Duoi6={7}; Duoi18={8}; Tren18={9};"
                    + " DHST={10} ban ghi (can nang: {11}; chieu cao: {12}) -> XML3/XML10;"
                    + " CLS={13} dong -> XML11; TiemChung={14}; TienSuBenhTat={15};"
                    + " HEALTH_EXAM_RANK_ID={16} -> phan_loai_sk; MaCskcb={17}; MaGtinCskcb={18}; MaLoaiKcb={19}",
                    SafeString(GetProp(row, "TDL_TREATMENT_CODE")),
                    ToLong(GetProp(row, "SERVICE_REQ_ID")),
                    ToLong(GetProp(row, "KSK_RECORD_ID")),
                    input.FormType,
                    YesNo(input.Patient), YesNo(input.Treatment), YesNo(input.General), YesNo(input.UnderSix),
                    YesNo(input.UnderEighteen), YesNo(input.OverEighteen),
                    dhstCount, YesNo(hasWeight), YesNo(hasHeight),
                    (input.ClsList != null) ? input.ClsList.Count : 0,
                    (input.Vaccinations != null) ? input.Vaccinations.Count : 0,
                    (input.PersonalHistoryDity != null) ? input.PersonalHistoryDity.Count : 0,
                    (rankId != null) ? rankId.ToString() : "(rong)",
                    Show(input.MaCskcb), Show(input.MaGtinCskcb), Show(input.MaLoaiKcb)));

                // Dump day du du lieu nguon cua ho so (DEBUG). KHONG dump SignImageByLoginName / cac danh muc
                // (HealthExamRanks, VaccineTypes, DiseaseTypes) vi rat dai va lap lai o moi ho so.
                DumpDebug("KskInput_" + ToLong(GetProp(row, "KSK_RECORD_ID")), new
                {
                    FormType = input.FormType.ToString(),
                    input.MaCskcb,
                    input.MaGtinCskcb,
                    input.MaLoaiKcb,
                    input.Patient,
                    input.Treatment,
                    input.General,
                    input.UnderSix,
                    input.UnderEighteen,
                    input.OverEighteen,
                    input.Dhst,
                    input.Vaccinations,
                    input.PersonalHistoryDity,
                    input.ClsList
                });
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>"co" khi doi tuong khac null / dieu kien dung; nguoc lai "KHONG".</summary>
        private static string YesNo(object value) { return (value != null) ? "co" : "KHONG"; }
        private static string YesNo(bool value) { return value ? "co" : "KHONG"; }

        /// <summary>
        /// TEMP FAKE — dung 1 Qd1551KskInput DU LIEU GIA (mau nguoi >=18 tuoi) de sinh XML thu,
        /// KHONG doc DB. Bat/tat bang USE_FAKE_DATA. Xoa method + co khi khong con test.
        /// </summary>
        // FAKE: 4 ho so — 3001/3002 NGUOI LON (>=18, co/khong CKDT_), 3003 TRE <6, 3004 NGUOI <18.
        internal const long FAKE_SR_HAS_CKDT = 3001L;
        internal const long FAKE_SR_NO_CKDT = 3002L;
        internal const long FAKE_SR_UNDER6 = 3003L;
        internal const long FAKE_SR_UNDER18 = 3004L;
        internal const string FAKE_CONCLUDER_LOGINNAME = "fakebs";
        // Ma co so (dung chung moi ho so fake).
        private const string FAKE_MA_CSKCB = "01816";            // ma 5 so (MA_CSKCB)
        private const string FAKE_MA_GTIN = "8934285005264";     // ma 13 so (MA_GTIN_CSKCB / MACSKCB)
        private const string FAKE_MA_LOAI_KCB = "01";

        private List<Qd1551KskInput> BuildFakeInputsFor(List<V_HIS_KSK_SYNC> rowList)
        {
            var byId = new Dictionary<long, Qd1551KskInput>
            {
                { FAKE_SR_HAS_CKDT, BuildFakeInput(FAKE_SR_HAS_CKDT, "NGUYỄN VĂN CÓ CKDT", true) },    // >=18, CKDT_ day
                { FAKE_SR_NO_CKDT,  BuildFakeInput(FAKE_SR_NO_CKDT,  "TRẦN THỊ KHÔNG CKDT", false) },  // >=18, CKDT_ rong
                { FAKE_SR_UNDER6,   BuildFakeInputUnder6(FAKE_SR_UNDER6, "LÊ BẢO AN (TRẺ <6)") },      // tre <6 (ChildUnder)
                { FAKE_SR_UNDER18,  BuildFakeInputUnder18(FAKE_SR_UNDER18, "PHẠM GIA HÂN (<18)") }     // nguoi <18 (Minor)
            };
            var result = new List<Qd1551KskInput>();
            if (rowList == null || rowList.Count == 0)   // preview/khong co dong -> tra ca 4 de test
            {
                result.Add(byId[FAKE_SR_HAS_CKDT]); result.Add(byId[FAKE_SR_NO_CKDT]);
                result.Add(byId[FAKE_SR_UNDER6]); result.Add(byId[FAKE_SR_UNDER18]);
                return result;
            }
            foreach (var row in rowList)
            {
                long sr = ToLong(GetProp(row, "SERVICE_REQ_ID"));
                Qd1551KskInput inp;
                if (!byId.TryGetValue(sr, out inp)) inp = BuildFakeInput(sr > 0 ? sr : FAKE_SR_HAS_CKDT, "NGUYỄN VĂN CÓ CKDT", true);
                result.Add(inp);
            }
            return result;
        }

        /// <summary>Dung 1 ho so KSK gia (mau >=18). hasCkdt=true -> co anh chu ky (CKDT_ day); false -> CKDT_ rong.</summary>
        private Qd1551KskInput BuildFakeInput(long serviceReqId, string patientName, bool hasCkdt)
        {
            const string BT = "Bình thường";
            // CCCD gia DUY NHAT moi lan day (theo thoi gian + ma ho so) -> tranh loi cong PS_CCCD_DUPLICATE_IN_6_MONTHS.
            string cccdFake = DateTime.Now.ToString("yyMMddHHmm") + ((serviceReqId % 100 + 100) % 100).ToString("00");
            HIS_PATIENT patient = new HIS_PATIENT
            {
                VIR_PATIENT_NAME = patientName,
                GENDER_ID = 1,
                DOB = 19900101000000L,
                ETHNIC_CODE = "01",
                CCCD_NUMBER = cccdFake,
                CCCD_DATE = 20200315L,
                CCCD_PLACE = "Cục CSDLQG về dân cư",
                BLOOD_ABO_CODE = "O",   // NHOM_MAU chi lay ABO (A/B/AB/O) — khong noi Rh
                VIR_ADDRESS = "Số 1, Xã An Minh, An Giang",
                PROVINCE_CODE = "91",
                COMMUNE_CODE = "31018",
                MOBILE = "0912345678",
                CAREER_CODE = "04",
                WORK_PLACE = "Công ty TNHH ABC"
            };
            HIS_TREATMENT treatment = new HIS_TREATMENT
            {
                ID = 1000,
                PATIENT_ID = 1,
                TREATMENT_CODE = "000026007788",
                IN_TIME = 20260709081500L,
                HOSPITALIZATION_REASON = "Khám sức khỏe định kỳ",
                TDL_PATIENT_CAREER_CODE = "0412"   // MA_NGHE_NGHIEP se cat con "04" (nhu XML130)
            };
            HIS_DHST dhst = new HIS_DHST
            {
                ID = 555,
                TREATMENT_ID = 1000,
                HEIGHT = 168m,
                WEIGHT = 60m,
                PULSE = 78L,
                BLOOD_PRESSURE_MAX = 120L,
                BLOOD_PRESSURE_MIN = 80L
            };
            HIS_KSK_OVER_EIGHTEEN oe = new HIS_KSK_OVER_EIGHTEEN
            {
                ID = 2000 + serviceReqId,
                SERVICE_REQ_ID = serviceReqId,
                DHST_ID = 555L,
                KSK_PATIENT_TYPES = "1;2",
                KSK_PAY_SOURCE = (short)2,
                HEALTH_EXAM_RANK_DESCRIPTION = "Đủ sức khỏe làm việc",
                DHST_RANK = 2L,
                // Tien su ban than (3 o text) — TSBT_MA_BENH_KHAC / TEN_THUOC / THAI_SAN
                PATHOLOGICAL_HISTORY = "Viêm dạ dày mạn",
                MEDICINE_USING = "Omeprazol 20mg",
                MATERNITY_HISTORY = "Không",
                // Noi khoa (ket qua text + phan loai _RANK)
                EXAM_CIRCULATION = BT, EXAM_CIRCULATION_RANK = 2L,
                EXAM_RESPIRATORY = BT, EXAM_RESPIRATORY_RANK = 2L,
                EXAM_DIGESTION = BT, EXAM_DIGESTION_RANK = 2L,
                EXAM_KIDNEY_UROLOGY = BT, EXAM_KIDNEY_UROLOGY_RANK = 2L,
                EXAM_OEND = BT, EXAM_OEND_RANK = 2L,
                EXAM_MUSCLE_BONE = BT, EXAM_MUSCLE_BONE_RANK = 2L,
                EXAM_NEUROLOGICAL = BT, EXAM_NEUROLOGICAL_RANK = 2L,
                EXAM_MENTAL = BT, EXAM_MENTAL_RANK = 2L,
                EXAM_SURGERY = BT, EXAM_SURGERY_RANK = 2L,
                EXAM_DERMATOLOGY = BT, EXAM_DERMATOLOGY_RANK = 2L,
                EXAM_OBSTETRIC = BT, EXAM_OBSTETRIC_RANK = 2L,
                // Mat
                EXAM_EYESIGHT_RIGHT = "10/10", EXAM_EYESIGHT_LEFT = "10/10",
                EXAM_EYESIGHT_GLASS_RIGHT = "10/10", EXAM_EYESIGHT_GLASS_LEFT = "10/10",
                EXAM_EYE_DISEASE = "Không", EXAM_EYE_RANK = 2L,
                // Tai mui hong
                // Tai (do suc nghe noi thuong/noi tham) — do dai toi da 10, dung gia tri khoang cach
                EXAM_ENT_LEFT_NORMAL = "5/5", EXAM_ENT_LEFT_WHISPER = "5/5",
                EXAM_ENT_RIGHT_NORMAL = "5/5", EXAM_ENT_RIGHT_WHISPER = "5/5",
                EXAM_ENT_DISEASE = "Không", EXAM_ENT_RANK = 2L,
                // Rang ham mat
                EXAM_STOMATOLOGY_UPPER = BT, EXAM_STOMATOLOGY_LOWER = BT,
                EXAM_STOMATOLOGY_DISEASE = "Không", EXAM_STOMATOLOGY_RANK = 2L,
                // CKDT_: loginname bac si kham -> tra base64 chu ky (fake) o SignImageByLoginName ben duoi
                EXAM_CIRCULATION_LOGINNAME = "fakebs",
                EXAM_RESPIRATORY_LOGINNAME = "fakebs",
                EXAM_DIGESTION_LOGINNAME = "fakebs",
                EXAM_KIDNEY_UROLOGY_LOGINNAME = "fakebs",
                EXAM_OEND_LOGINNAME = "fakebs",
                EXAM_MUSCLE_BONE_LOGINNAME = "fakebs",
                EXAM_NEUROLOGICAL_LOGINNAME = "fakebs",
                EXAM_MENTAL_LOGINNAME = "fakebs",
                EXAM_SURGERY_LOGINNAME = "fakebs",
                EXAM_DERMATOLOGY_LOGINNAME = "fakebs",
                EXAM_OBSTETRIC_LOGINNAME = "fakebs",
                EXAM_EYE_LOGINNAME = "fakebs",
                EXAM_ENT_LOGINNAME = "fakebs",
                EXAM_STOMATOLOGY_LOGINNAME = "fakebs"
            };
            HIS_KSK_GENERAL general = new HIS_KSK_GENERAL
            {
                ID = 4000 + serviceReqId,
                SERVICE_REQ_ID = serviceReqId,
                DHST_ID = 555L,
                HEALTH_CONCLUSION_TYPE = (short)1,
                HEALTH_EXAM_RANK_ID = 2L,                                     // -> PHAN_LOAI_SK = "2"
                DISEASES = "Không phát hiện bệnh lý cấp tính",                 // -> CAC_BENH_TAT_NEU_CO
                CONCLUSION_ICD_CODE = "Z00.0",                                // -> KET_LUAN_BENH (ma ICD)
                FAMILY_HISTORY_ICD_CODE = "I10",                              // -> TSGD_MA_BENH + co
                PERSONAL_HISTORY_ICD_CODE = "K29",                            // -> TSBT_MA_BENH + co
                TREATING_DISEASE_ICD_CODE = "K29",                            // -> co TSBT_DANG_DIEU_TRI_BENH
                OBSTETRIC_DISEASE_ICD_CODE = ""                               // nam gioi -> de trong
            };
            List<HIS_HEALTH_EXAM_RANK> ranks = new List<HIS_HEALTH_EXAM_RANK>
            {
                new HIS_HEALTH_EXAM_RANK { ID = 1, HEALTH_EXAM_RANK_CODE = "1" },
                new HIS_HEALTH_EXAM_RANK { ID = 2, HEALTH_EXAM_RANK_CODE = "2" },
                new HIS_HEALTH_EXAM_RANK { ID = 3, HEALTH_EXAM_RANK_CODE = "3" },
                new HIS_HEALTH_EXAM_RANK { ID = 4, HEALTH_EXAM_RANK_CODE = "4" },
                new HIS_HEALTH_EXAM_RANK { ID = 5, HEALTH_EXAM_RANK_CODE = "5" }
            };
            // Tien su ban than (grid) -> co TSBT_* : ma 5=tim, 7=tang huyet ap, 12=dai thao duong (DefaultTsbtByCode)
            List<HIS_DISEASE_TYPE> diseaseTypes = new List<HIS_DISEASE_TYPE>
            {
                new HIS_DISEASE_TYPE { ID = 5, DISEASE_TYPE_CODE = "5" },
                new HIS_DISEASE_TYPE { ID = 7, DISEASE_TYPE_CODE = "7" },
                new HIS_DISEASE_TYPE { ID = 12, DISEASE_TYPE_CODE = "12" }
            };
            List<HIS_PERIOD_DRIVER_DITY> ditys = new List<HIS_PERIOD_DRIVER_DITY>
            {
                new HIS_PERIOD_DRIVER_DITY { ID = 1, DISEASE_TYPE_ID = 5, IS_YES_NO = "1", KSK_OVER_EIGHTEEN_ID = 2000L },
                new HIS_PERIOD_DRIVER_DITY { ID = 2, DISEASE_TYPE_ID = 7, IS_YES_NO = "1", KSK_OVER_EIGHTEEN_ID = 2000L },
                new HIS_PERIOD_DRIVER_DITY { ID = 3, DISEASE_TYPE_ID = 12, IS_YES_NO = "1", KSK_OVER_EIGHTEEN_ID = 2000L }
            };
            List<Qd1551ClsRow> cls = new List<Qd1551ClsRow>
            {
                new Qd1551ClsRow { MA_DICH_VU = "03C3.1.89", TEN_DICH_VU = "Tổng phân tích tế bào máu ngoại vi", MA_CHI_SO = "H02", TEN_CHI_SO = "Huyết sắc tố", GIA_TRI = "130", DON_VI_DO = "g/L", MO_TA = "Trong giới hạn bình thường", KET_LUAN = "Bình thường" },
                new Qd1551ClsRow { MA_DICH_VU = "18.0068.0013", TEN_DICH_VU = "X Quang phổi thẳng", MA_CHI_SO = "X01", TEN_CHI_SO = "X Quang phổi thẳng", GIA_TRI = "Không", DON_VI_DO = "Không", MO_TA = "Không thấy tổn thương nhu mô phổi", KET_LUAN = "Bình thường" }
            };
            Qd1551KskInput input = new Qd1551KskInput
            {
                FormType = FormType.Tren18,
                Patient = patient,
                Treatment = treatment,
                OverEighteen = oe,
                General = general,
                Dhst = new List<HIS_DHST> { dhst },
                HealthExamRanks = ranks,
                PersonalHistoryDity = ditys,
                DiseaseTypes = diseaseTypes,
                ClsList = cls,
                // CKDT_ (fake): hasCkdt=true -> co anh chu ky cho "fakebs" (CKDT_ day); false -> khong map -> CKDT_ rong
                SignImageByLoginName = hasCkdt
                    ? new Dictionary<string, string> { { "fakebs", FakeSignImage.ABC_JPG_BASE64 } }
                    : new Dictionary<string, string>(),
                MaCskcb = FAKE_MA_CSKCB,           // ma 5 so (MA_CSKCB)
                MaGtinCskcb = FAKE_MA_GTIN,        // ma 13 so (MA_GTIN_CSKCB / MACSKCB)
                MaLoaiKcb = FAKE_MA_LOAI_KCB
            };
            return input;
        }

        /// <summary>CCCD/định danh giả DUY NHẤT mỗi lần đẩy (theo thời gian + mã hồ sơ) — tránh PS_CCCD_DUPLICATE.</summary>
        private static string FakeCccd(long serviceReqId)
        {
            return DateTime.Now.ToString("yyMMddHHmm") + ((serviceReqId % 100 + 100) % 100).ToString("00");
        }

        /// <summary>TEMP FAKE — 1 hồ sơ KSK TRẺ &lt;6 TUỔI (ChildUnder → XML dùng HIS_KSK_UNDER_SIX), đầy đủ dữ liệu.</summary>
        private Qd1551KskInput BuildFakeInputUnder6(long serviceReqId, string patientName)
        {
            HIS_PATIENT patient = new HIS_PATIENT
            {
                VIR_PATIENT_NAME = patientName,
                GENDER_ID = 1,
                DOB = 20220615000000L,                 // ~4 tuổi (tính đến 2026)
                ETHNIC_CODE = "01",
                CCCD_NUMBER = FakeCccd(serviceReqId),
                BLOOD_ABO_CODE = "O",
                VIR_ADDRESS = "Số 1, Xã An Minh, An Giang",
                PROVINCE_CODE = "91",
                COMMUNE_CODE = "31018",
                MOBILE = "0912345678"
            };
            // Hồ sơ điều trị + thông tin NGƯỜI ĐI CÙNG (XML1 trẻ): CCCD/điện thoại người nhà.
            HIS_TREATMENT treatment = new HIS_TREATMENT
            {
                ID = 1003, PATIENT_ID = 1, TREATMENT_CODE = "000026007790",
                IN_TIME = 20260709081500L, HOSPITALIZATION_REASON = "Khám sức khỏe định kỳ",
                TDL_RELATIVE_CMND_NUMBER = "079222333444",
                TDL_PATIENT_RELATIVE_MOBILE = "0987000111"
            };
            HIS_DHST dhst = new HIS_DHST { ID = 557, TREATMENT_ID = 1003, HEIGHT = 98m, WEIGHT = 15m, PULSE = 110L };
            HIS_KSK_UNDER_SIX us = new HIS_KSK_UNDER_SIX
            {
                ID = 2000 + serviceReqId, SERVICE_REQ_ID = serviceReqId, DHST_ID = 557L,
                KSK_PATIENT_TYPES = "1;2", KSK_PAY_SOURCE = (short)2,
                // XML1 trẻ
                IS_PREMATURE_BIRTH = 0, ACCOMPANY_PERSON_NAME = "Nguyễn Thị Mẹ", ACCOMPANY_RELATIONSHIP = 1,
                RESIDENCE = "Số 1, Xã An Minh, An Giang", IS_TB_CONTACT = 0,
                // XML3 sinh tồn
                TEMPERATURE = "36.8", TEMPERATURE_EVAL = 1, PULSE = "110", PULSE_EVAL = 1,
                RESPIRATORY_RATE = "30", RESPIRATORY_EVAL = 1,
                // XML4 dinh dưỡng (không bất thường -> DGDD_BINH_THUONG=1)
                BODY_LENGTH = "98", BODY_LENGTH_AGE_SD = "0", WEIGHT = "15", WEIGHT_AGE_SD = "0",
                HEAD_CIRCUMFERENCE = "48", HEAD_CIRC_EVAL = 1, ARM_CIRCUMFERENCE = "15",
                IS_NUTRITIONAL_EDEMA = 0, IS_ANEMIA_SIGN = 0, IS_RICKETS_SIGN = 0, IS_MALNUTRITION = 0, IS_OVERWEIGHT = 0,
                // XML5 phát triển
                MENTAL_DEV_NORMAL = 1, MOTOR_DEV_NORMAL = 1, AUTISM_RISK = 0,
                // XML6 tiêm chủng trẻ
                VACCINE_TB = 1, VACCINE_HEPB1 = 1, VACCINE_FULL_BY_AGE = 1,
                // XML7 khám lâm sàng trẻ (đầy đủ — 1 = bình thường theo dữ liệu mẫu)
                SKIN_COLOR = 1, PALM_EVAL = 1, FONTANEL = 1, HEAD_SHAPE = 1, NECK_MOTION = 1, HEAD_ABNORMAL_MASS = 0,
                EYE_POSITION = 1, EYELID_CONJUNCTIVA = 1, STRABISMUS = 0, PUPIL = 1, EAR_EARDRUM = 1, SOUND_RESPONSE = 1,
                EAR_SWELLING = 0, EAR_DISCHARGE = 0, NOSE_SHAPE = 1, RUNNY_NOSE = 0, STUFFY_NOSE = 0, THROAT = 1,
                MOUTH_SHAPE = 1, NEONATAL_TEETH = 0, TONGUE_SHAPE = 1, TONGUE_TIE = 0, ORAL_THRUSH = 0, SMALL_CHIN = 0,
                TOOTH_DECAY = 0, IRREGULAR_BREATH = 0, CHEST_RETRACTION = 0, ABNORMAL_BREATH_SOUND = 0, RESP_FAILURE_SIGN = 0,
                LUNG_AUSCULTATION = 1, APEX_POSITION = 1, PERIPHERAL_PULSE = 1, HEART_AUSCULTATION = 1, ABDOMEN_NAVEL = 1,
                HEPATOSPLENOMEGALY = 0, ABDOMEN_MASS = 0, ANUS = 1, GENITALIA = 1, ASYMMETRIC_MOVEMENT = 0,
                SUCKING_REFLEX = 1, GRASP_REFLEX = 1, MORO_REFLEX = 1, MUSCLE_TONE = 1, HIP_JOINT = 1, MUSCLE_REFLEX = 1,
                SPINE_CHECK = 1, LIMBS_JOINTS = 1, GAIT = 1,
                // XML8 chuyển cơ sở
                IS_TRANSFER_MEDI_ORG = 0
            };
            HIS_KSK_GENERAL general = new HIS_KSK_GENERAL
            {
                ID = 4000 + serviceReqId, SERVICE_REQ_ID = serviceReqId, DHST_ID = 557L,
                HEALTH_CONCLUSION_TYPE = (short)1,
                HEALTH_EXAM_RANK_ID = 1L,
                DISEASES = "Trẻ phát triển bình thường",
                CONCLUSION_ICD_CODE = "Z00.1",
                PERSONAL_HISTORY_ICD_CODE = ""            // trẻ: không tiền sử -> TSBT_MAC_BENH=0
            };
            Qd1551KskInput input = new Qd1551KskInput
            {
                FormType = FormType.Tre2_6Tuoi,           // -> ChildUnder
                Patient = patient,
                Treatment = treatment,
                UnderSix = us,
                General = general,
                Dhst = new List<HIS_DHST> { dhst },
                HealthExamRanks = BuildFakeRanks(),
                SignImageByLoginName = new Dictionary<string, string>(),
                MaCskcb = FAKE_MA_CSKCB,
                MaGtinCskcb = FAKE_MA_GTIN,
                MaLoaiKcb = FAKE_MA_LOAI_KCB
            };
            return input;
        }

        /// <summary>TEMP FAKE — 1 hồ sơ KSK NGƯỜI 6–&lt;18 TUỔI (Minor → XML dùng HIS_KSK_UNDER_EIGHTEEN), đầy đủ dữ liệu.</summary>
        private Qd1551KskInput BuildFakeInputUnder18(long serviceReqId, string patientName)
        {
            const string BT = "Bình thường";
            HIS_PATIENT patient = new HIS_PATIENT
            {
                VIR_PATIENT_NAME = patientName,
                GENDER_ID = 2,                            // nữ
                DOB = 20120310000000L,                    // ~14 tuổi
                ETHNIC_CODE = "01",
                CCCD_NUMBER = FakeCccd(serviceReqId),
                CCCD_DATE = 20240101L,
                CCCD_PLACE = "Cục CSDLQG về dân cư",
                BLOOD_ABO_CODE = "A",
                VIR_ADDRESS = "Số 2, Xã An Minh, An Giang",
                PROVINCE_CODE = "91",
                COMMUNE_CODE = "31018",
                MOBILE = "0913222333",
                CAREER_CODE = "00"
            };
            HIS_TREATMENT treatment = new HIS_TREATMENT
            {
                ID = 1004, PATIENT_ID = 1, TREATMENT_CODE = "000026007791",
                IN_TIME = 20260709081500L, HOSPITALIZATION_REASON = "Khám sức khỏe định kỳ",
                // Người giám hộ (XML1 mẫu 6–<18) 
                TDL_PATIENT_RELATIVE_NAME = "Phạm Văn Bố",
                TDL_RELATIVE_CMND_NUMBER = "079111222333",
                TDL_PATIENT_RELATIVE_MOBILE = "0987444555"
            };
            HIS_DHST dhst = new HIS_DHST
            {
                ID = 558, TREATMENT_ID = 1004, HEIGHT = 150m, WEIGHT = 42m,
                PULSE = 82L, BLOOD_PRESSURE_MAX = 110L, BLOOD_PRESSURE_MIN = 70L
            };
            HIS_KSK_UNDER_EIGHTEEN ue = new HIS_KSK_UNDER_EIGHTEEN
            {
                ID = 2000 + serviceReqId, SERVICE_REQ_ID = serviceReqId, DHST_ID = 558L,
                KSK_PATIENT_TYPES = "1;2", KSK_PAY_SOURCE = (short)2,
                DHST_RANK = 2L, HEALTH_EXAM_RANK_ID = 2L,
                // Nhi khoa (kết quả text + chữ ký người khám)
                EXAM_CIRCULATION = BT, EXAM_RESPIRATORY = BT, EXAM_DIGESTION = BT, EXAM_KIDNEY_UROLOGY = BT,
                EXAM_NEURO_MENTAL = BT, EXAM_MENTAL = BT, EXAM_CLINICAL_OTHER = BT,
                EXAM_CIRCULATION_LOGINNAME = "fakebs", EXAM_RESPIRATORY_LOGINNAME = "fakebs",
                EXAM_DIGESTION_LOGINNAME = "fakebs", EXAM_KIDNEY_UROLOGY_LOGINNAME = "fakebs",
                EXAM_NEURO_MENTAL_LOGINNAME = "fakebs", EXAM_MENTAL_LOGINNAME = "fakebs",
                EXAM_CLINICAL_OTHER_LOGINNAME = "fakebs",
                // Mắt
                EXAM_EYESIGHT_RIGHT = "10/10", EXAM_EYESIGHT_LEFT = "10/10",
                EXAM_EYESIGHT_GLASS_RIGHT = "10/10", EXAM_EYESIGHT_GLASS_LEFT = "10/10",
                EXAM_EYE_DISEASE = "Không", EXAM_EYE_RANK = 2L, EXAM_EYE_LOGINNAME = "fakebs",
                // Tai mũi họng
                EXAM_ENT_LEFT_NORMAL = "5/5", EXAM_ENT_LEFT_WHISPER = "5/5",
                EXAM_ENT_RIGHT_NORMAL = "5/5", EXAM_ENT_RIGHT_WHISPER = "5/5",
                EXAM_ENT_DISEASE = "Không", EXAM_ENT_RANK = 2L, EXAM_ENT_LOGINNAME = "fakebs",
                // Răng hàm mặt
                EXAM_STOMATOLOGY_UPPER = BT, EXAM_STOMATOLOGY_LOWER = BT,
                EXAM_STOMATOLOGY_DISEASE = "Không", EXAM_STOMATOLOGY_RANK = 2L, EXAM_STOMATOLOGY_LOGINNAME = "fakebs",
                // Tiền sử + sức khỏe
                PATHOLOGICAL_HISTORY = "Không", MEDICINE_USING = "", MATERNITY_HISTORY = "Không",
                PROBLEM_HEALTH = "Không", OBSTETRIC_ABNORMAL_CODES = 0
            };
            HIS_KSK_GENERAL general = new HIS_KSK_GENERAL
            {
                ID = 4000 + serviceReqId, SERVICE_REQ_ID = serviceReqId, DHST_ID = 558L,
                HEALTH_CONCLUSION_TYPE = (short)1,
                HEALTH_EXAM_RANK_ID = 2L,                                 // -> PHAN_LOAI_SK = "2"
                DISEASES = "Không phát hiện bệnh lý",
                CONCLUSION_ICD_CODE = "Z00.0",
                FAMILY_HISTORY_ICD_CODE = "", PERSONAL_HISTORY_ICD_CODE = "",
                TREATING_DISEASE_ICD_CODE = "", TREATING_DISEASE_ICD_NAME = "",
                OBSTETRIC_DISEASE_ICD_CODE = ""
            };
            // Tiêm chủng (XML9): BCG/BH-HG-UV/Sởi = đã tiêm (CONDITION_TYPE=1).
            List<HIS_VACCINE_TYPE> vaccineTypes = new List<HIS_VACCINE_TYPE>
            {
                new HIS_VACCINE_TYPE { ID = 1, VACCINE_TYPE_CODE = "KSK01" },
                new HIS_VACCINE_TYPE { ID = 2, VACCINE_TYPE_CODE = "KSK02" },
                new HIS_VACCINE_TYPE { ID = 3, VACCINE_TYPE_CODE = "KSK03" }
            };
            List<HIS_KSK_UNEI_VATY> vaccinations = new List<HIS_KSK_UNEI_VATY>
            {
                new HIS_KSK_UNEI_VATY { ID = 1, VACCINE_TYPE_ID = 1, CONDITION_TYPE = 1L },
                new HIS_KSK_UNEI_VATY { ID = 2, VACCINE_TYPE_ID = 2, CONDITION_TYPE = 1L },
                new HIS_KSK_UNEI_VATY { ID = 3, VACCINE_TYPE_ID = 3, CONDITION_TYPE = 1L }
            };
            Qd1551KskInput input = new Qd1551KskInput
            {
                FormType = FormType.Duoi18,               // -> Minor
                Patient = patient,
                Treatment = treatment,
                UnderEighteen = ue,
                General = general,
                Dhst = new List<HIS_DHST> { dhst },
                HealthExamRanks = BuildFakeRanks(),
                Vaccinations = vaccinations,
                VaccineTypes = vaccineTypes,
                SignImageByLoginName = new Dictionary<string, string> { { "fakebs", FakeSignImage.ABC_JPG_BASE64 } },
                MaCskcb = FAKE_MA_CSKCB,
                MaGtinCskcb = FAKE_MA_GTIN,
                MaLoaiKcb = FAKE_MA_LOAI_KCB
            };
            return input;
        }

        /// <summary>Danh mục phân loại sức khỏe fake (mã "1".."5") — quy đổi *_RANK/HEALTH_EXAM_RANK_ID sang mã.</summary>
        private static List<HIS_HEALTH_EXAM_RANK> BuildFakeRanks()
        {
            return new List<HIS_HEALTH_EXAM_RANK>
            {
                new HIS_HEALTH_EXAM_RANK { ID = 1, HEALTH_EXAM_RANK_CODE = "1" },
                new HIS_HEALTH_EXAM_RANK { ID = 2, HEALTH_EXAM_RANK_CODE = "2" },
                new HIS_HEALTH_EXAM_RANK { ID = 3, HEALTH_EXAM_RANK_CODE = "3" },
                new HIS_HEALTH_EXAM_RANK { ID = 4, HEALTH_EXAM_RANK_CODE = "4" },
                new HIS_HEALTH_EXAM_RANK { ID = 5, HEALTH_EXAM_RANK_CODE = "5" }
            };
        }


        /// <summary>
        /// Suy MA_LOAI_KCB tu loai dieu tri — port DUNG logic bo XML BHYT (XML130 Xml1Processor):
        /// KHAM=01; DTNOITRU: noi tru duoi 4h (OUT_TIME - CLINICAL_IN_TIME) =09, nguoc lai =03;
        /// DTBANNGAY=04; TYTXA=06; NHANTHUOC=07; DTNGOAITRU: khong man tinh =02, man tinh co DV
        /// ngoai kham/don (KH/DONDT/DONTT/DONK) =08 nguoc lai =05; mac dinh =10.
        /// Rieng KSK: neu MA_LOAI_KCB=01 va doi tuong hien tai la KSK (PATIENT_TYPE_CODE = config
        /// MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.KSK, khop TDL_PATIENT_TYPE_ID) -> "100".
        /// Cac MA LOAI KCB MOI theo QD 1804/QD-BYT: KHONG hardcode ID nhu XML130/TT12 — lay theo danh muc
        /// HIS_TREATMENT_TYPE.TREATMENT_TYPE_CODE (VARCHAR2(2) = dung ma BYT), vi HIS_TREATMENT_TYPE_SEQ
        /// bat dau tu 21 nen ID cac loai them moi khac nhau giua cac co so. CHI nhan ma trong 11..16
        /// (pham vi da duoc bo sung); ma ngoai khoang nay -> giu mac dinh "10" nhu cu.
        /// </summary>
        private static string ResolveMaLoaiKcb(HIS_TREATMENT t, List<HIS_SERE_SERV> allSereServs,
            List<V_HIS_PATIENT_TYPE_ALTER> alters, string keyKsk)
        {
            if (t == null) return "";
            string maLoaiKcb = "10";
            long type = t.TDL_TREATMENT_TYPE_ID ?? 0;
            if (type == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
            {
                maLoaiKcb = "01";
            }
            else if (type == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
            {
                if (t.OUT_TIME.HasValue && t.CLINICAL_IN_TIME.HasValue
                    && (t.OUT_TIME.Value - t.CLINICAL_IN_TIME.Value) > 0
                    && Inventec.Common.DateTime.Calculation.DifferenceTime(t.CLINICAL_IN_TIME.Value, t.OUT_TIME.Value,
                        Inventec.Common.DateTime.Calculation.UnitDifferenceTime.HOUR) < 4)
                    maLoaiKcb = "09";
                else
                    maLoaiKcb = "03";
            }
            else if (type == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY)
            {
                maLoaiKcb = "04";
            }
            else if (type == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__TYTXA)
            {
                maLoaiKcb = "06";
            }
            else if (type == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__NHANTHUOC)
            {
                maLoaiKcb = "07";
            }
            else if (type == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
            {
                if (t.IS_CHRONIC != 1)
                    maLoaiKcb = "02";
                else if (allSereServs != null && allSereServs.Count > 0)
                {
                    if (allSereServs.Exists(o => o != null
                        && o.TDL_SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH
                        && o.TDL_SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT
                        && o.TDL_SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT
                        && o.TDL_SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK))
                        maLoaiKcb = "08";
                    else
                        maLoaiKcb = "05";
                }
            }
            else
            {
                // Loai dieu tri NGOAI 6 loai goc -> ma loai KCB moi theo QD 1804/QD-BYT: lay tu danh muc
                // (TREATMENT_TYPE_CODE), khong so ID. CHI nhan 11..16; ma khac hoac danh muc trong
                // -> giu mac dinh "10" (khong tin ma tu do trong danh muc).
                string typeCode = ResolveTreatmentTypeCode(type);
                if (NEW_MA_LOAI_KCB_CODES.Contains(typeCode)) maLoaiKcb = typeCode;
            }
            // Kham suc khoe: dien dieu tri kham + doi tuong benh nhan la KSK -> 100 (nhu XML130).
            if (maLoaiKcb == "01" && !string.IsNullOrEmpty(keyKsk)
                && alters != null && alters.Count > 0
                && alters.Exists(o => o != null && o.PATIENT_TYPE_CODE == keyKsk && o.PATIENT_TYPE_ID == t.TDL_PATIENT_TYPE_ID))
            {
                maLoaiKcb = "100";
            }
            return maLoaiKcb;
        }

        /// <summary>
        /// Cac ma loai KCB moi duoc bo sung theo QD 1804/QD-BYT — CHI cac ma nay duoc lay tu danh muc
        /// loai dieu tri. Ma ngoai danh sach -> khong dung (giu mac dinh nhu logic cu).
        /// </summary>
        private static readonly string[] NEW_MA_LOAI_KCB_CODES = new string[] { "11", "12", "13", "14", "15", "16" };

        /// <summary>
        /// Ma loai KCB theo danh muc loai dieu tri: HIS_TREATMENT_TYPE.TREATMENT_TYPE_CODE (2 ky tu,
        /// dung ma BYT) theo TDL_TREATMENT_TYPE_ID. Doc tu cache RAM (BackendDataWorker) nen khong ton
        /// them call API. Khong tim thay / danh muc de trong -> "" (caller giu mac dinh).
        /// </summary>
        private static string ResolveTreatmentTypeCode(long treatmentTypeId)
        {
            try
            {
                if (treatmentTypeId <= 0) return "";
                var treatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>()
                    .FirstOrDefault(o => o != null && o.ID == treatmentTypeId);
                if (treatmentType == null || string.IsNullOrWhiteSpace(treatmentType.TREATMENT_TYPE_CODE)) return "";
                return treatmentType.TREATMENT_TYPE_CODE.Trim();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return ""; }
        }

        /// <summary>Goi API danh sach (Get) — MosConsumer. Loi -> null (khong chan cac call khac).</summary>
        private static List<T> GetList<T>(string uri, object filter)
        {
            return GetList<T>(uri, filter, ApiConsumers.MosConsumer);
        }

        /// <summary>Goi API danh sach (Get) theo consumer chi dinh (Mos/Emr...). Loi -> null.</summary>
        private static List<T> GetList<T>(string uri, object filter, Inventec.Common.WebApiClient.ApiConsumer consumer)
        {
            try
            {
                var param = new CommonParam();
                return new BackendAdapter(param).Get<List<T>>(uri, consumer, filter, param);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        /// <summary>
        /// Gom moi gia tri cot string ket thuc "LOGINNAME" (EXAM_*_LOGINNAME, CONCLUDER_LOGINNAME,
        /// EXECUTE_LOGINNAME, SUBCLINICAL_RESULT_LOGINNAME...) tu cac danh sach entity — dung tra emr_signer.
        /// </summary>
        private static List<string> CollectLoginnames(params System.Collections.IEnumerable[] lists)
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (lists != null)
                foreach (var list in lists)
                {
                    if (list == null) continue;
                    foreach (var item in list)
                    {
                        if (item == null) continue;
                        foreach (var p in item.GetType().GetProperties())
                        {
                            if (p.PropertyType != typeof(string) || !p.Name.EndsWith("LOGINNAME")) continue;
                            string v = null;
                            try { v = p.GetValue(item, null) as string; } catch { }
                            if (!string.IsNullOrEmpty(v)) set.Add(v.Trim());
                        }
                    }
                }
            return set.ToList();
        }

        /// <summary>emr_signer -> map loginname -> base64(SIGN_IMAGE). Khong co chu ky -> bo qua loginname do.</summary>
        private static Dictionary<string, string> BuildSignMap(List<EMR.EFMODEL.DataModels.EMR_SIGNER> signers)
        {
            if (signers == null || signers.Count == 0) return null;
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var s in signers)
            {
                if (s == null || string.IsNullOrEmpty(s.LOGINNAME)) continue;
                if (s.SIGN_IMAGE == null || s.SIGN_IMAGE.Length == 0) continue;
                // Nen anh neu base64 vuot 65000 ky tu (gioi han 1 the CKDT_ cua cong tiep nhan).
                // Nen 1 lan/bac si tai day — 1 bac si co the ky nhieu the CKDT_ (VD 8 the noi khoa).
                if (!map.ContainsKey(s.LOGINNAME))
                    map[s.LOGINNAME] = KskSignImageCompressor.ToBase64(s.SIGN_IMAGE, s.LOGINNAME);
            }
            return map.Count > 0 ? map : null;
        }

        /// <summary>
        /// Ghi log PHAM VI ky so cua lan bam nay — de doi soat khi mo file XML thay the CKS_ trong:
        ///   - CKS_BENH_VIEN: luon ky khi tich ky so (HSM hoac USB token).
        ///   - CKS_NGUOI_KET_LUAN: CHI ky duoc bang HSM cua nguoi ket luan (EMR_SIGNER co PCA_SERIAL).
        ///     Cau hinh ky so khong phai HSM, hoac nguoi ket luan chua khai chung thu HSM -> the do DE TRONG
        ///     (khong chan viec ky/xuat) => log ro ly do + liet ke loginname thieu chung thu.
        /// </summary>
        private void LogSignScope(List<V_HIS_KSK_SYNC> rowList,
            Dictionary<string, EMR.EFMODEL.DataModels.EMR_SIGNER> concSigners, string prefix)
        {
            try
            {
                if (this.signSetting == null) return;
                if (!this.signSetting.IsHsm)
                {
                    Inventec.Common.Logging.LogSystem.Info(prefix + ": ky so bang USB token -> chi ky CKS_BENH_VIEN;"
                        + " CKS_NGUOI_KET_LUAN DE TRONG (chi ky duoc bang HSM cua nguoi ket luan).");
                    return;
                }
                var missCert = new List<string>();
                if (rowList != null)
                    foreach (var row in rowList)
                    {
                        string cl = SafeString(GetProp(row, "CONCLUDER_LOGINNAME"));
                        if (string.IsNullOrEmpty(cl) || missCert.Contains(cl)) continue;
                        if (concSigners == null || !concSigners.ContainsKey(cl)) missCert.Add(cl);
                    }
                if (missCert.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Info(prefix + ": ky CKS_BENH_VIEN + CKS_NGUOI_KET_LUAN (HSM)"
                        + " cho toan bo nguoi ket luan.");
                    return;
                }
                Inventec.Common.Logging.LogSystem.Warn(prefix + ": " + missCert.Count + " nguoi ket luan CHUA KHAI"
                    + " CHUNG THU HSM (EMR_SIGNER.PCA_SERIAL) -> CKS_NGUOI_KET_LUAN cua cac ho so do DE TRONG"
                    + " (van ky CKS_BENH_VIEN). Loginname: " + string.Join(", ", missCert.Take(30).ToArray()));
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Fetch EMR_SIGNER cua NGUOI KET LUAN theo danh sach CONCLUDER_LOGINNAME (loginname) trong cac ho so.
        /// Tra map loginname -> EMR_SIGNER (chi lay ban co PCA_SERIAL — du chung thu HSM de ky). Dung khi ky
        /// CKS_NGUOI_KET_LUAN per-file trong ExportXmlFiles.
        /// </summary>
        private Dictionary<string, EMR.EFMODEL.DataModels.EMR_SIGNER> FetchConcluderSigners(List<V_HIS_KSK_SYNC> rowList)
        {
            var map = new Dictionary<string, EMR.EFMODEL.DataModels.EMR_SIGNER>(StringComparer.OrdinalIgnoreCase);
            try
            {
                if (rowList == null) return map;
                var logins = new List<string>();
                foreach (var row in rowList)
                {
                    string cl = SafeString(GetProp(row, "CONCLUDER_LOGINNAME"));
                    if (!string.IsNullOrEmpty(cl) && !logins.Contains(cl)) logins.Add(cl);
                }
                if (logins.Count == 0) return map;
                var signers = GetList<EMR.EFMODEL.DataModels.EMR_SIGNER>("api/EmrSigner/Get",
                    new EMR.Filter.EmrSignerFilter { LOGINNAMEs = logins, IS_ACTIVE = 1 }, ApiConsumers.EmrConsumer);
                if (signers != null)
                    foreach (var s in signers)
                        if (s != null && !string.IsNullOrEmpty(s.LOGINNAME) && !string.IsNullOrEmpty(s.PCA_SERIAL)
                            && !map.ContainsKey(s.LOGINNAME))
                            map[s.LOGINNAME] = s;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return map;
        }

        /// <summary>
        /// Dung danh sach dong CLS (Qd1551ClsRow) theo TREATMENT_ID: dich vu XN co chi so -> 1 dong/chi so
        /// (MA_CHI_SO/GIA_TRI/DON_VI_DO tu V_HIS_SERE_SERV_TEIN); dich vu khong co chi so (CDHA/NS/SA/TDCN)
        /// -> 1 dong/dich vu. MO_TA/KET_LUAN/bac si ket qua tu HIS_SERE_SERV_EXT.
        /// </summary>
        private static Dictionary<long, List<Qd1551ClsRow>> BuildClsByTreatment(
            List<V_HIS_SERE_SERV_2> sereServs, List<V_HIS_SERE_SERV_TEIN> teins,
            List<V_HIS_SERE_SERV_SUIN> suins, List<HIS_SERE_SERV_EXT> exts)
        {
            var result = new Dictionary<long, List<Qd1551ClsRow>>();
            if (sereServs == null || sereServs.Count == 0) return result;

            // Chi lay dich vu CLS theo LOAI DICH VU BHYT (HEIN_SERVICE_TYPE): CDHA / TDCN / XN.
            var allowedHein = new HashSet<long>
            {
                IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__CDHA,
                IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TDCN,
                IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__XN
            };

            // 3 nguon chi so nhu XML130 XML4: (1) TEIN (chi so XN), (2) SUIN (chi so CDHA/TDCN),
            // (3) khong co chi so -> 1 dong dich vu (GIA_TRI/DON_VI_DO rong).
            var teinBySs = GroupByKey(teins, t => t.SERE_SERV_ID);
            var suinBySs = GroupByKey(suins, s => s.SERE_SERV_ID);
            var extBySs = IndexBy(exts, e => e.SERE_SERV_ID);

            foreach (var ss in sereServs)
            {
                if (ss == null || ss.IS_NO_EXECUTE != null) continue;   // chi lay dich vu DA thuc hien
                if (!allowedHein.Contains(ss.TDL_HEIN_SERVICE_TYPE_ID ?? 0)) continue;   // chi CDHA/TDCN/XN
                long tr = ss.TDL_TREATMENT_ID ?? 0;
                if (tr <= 0) continue;

                HIS_SERE_SERV_EXT ext = ValOrNull(extBySs, ss.ID);
                string moTa = (ext != null) ? (ext.DESCRIPTION ?? "") : "";
                string ketLuan = (ext != null) ? (ext.CONCLUDE ?? "") : "";
                string bacSi = (ext != null) ? (ext.SUBCLINICAL_RESULT_LOGINNAME ?? "") : "";
                string maDichVu = ss.TDL_HEIN_SERVICE_BHYT_CODE ?? "";
                string tenDichVu = ss.TDL_HEIN_SERVICE_BHYT_NAME ?? "";

                List<Qd1551ClsRow> rows;
                if (!result.TryGetValue(tr, out rows)) { rows = new List<Qd1551ClsRow>(); result[tr] = rows; }

                // --- Nguon 1: TEIN (chi so xet nghiem) ---
                List<V_HIS_SERE_SERV_TEIN> ssTeins = ListOrNull(teinBySs, ss.ID);
                if (ssTeins != null && ssTeins.Count > 0)
                {
                    foreach (var tein in ssTeins)
                    {
                        if (tein == null) continue;
                        // BHYT (giong XML130 XML4): MA_CHI_SO/TEN_CHI_SO = mã/tên chỉ số BHYT (fallback kháng KS);
                        // GIA_TRI/DON_VI_DO/MO_TA theo chỉ số.
                        string maChiSo = !string.IsNullOrEmpty(tein.BHYT_CODE) ? tein.BHYT_CODE
                            : (tein.ANTIBIOTIC_RESISTANCE_CODE ?? "");
                        string tenChiSo = !string.IsNullOrEmpty(tein.BHYT_NAME) ? tein.BHYT_NAME
                            : (tein.ANTIBIOTIC_RESISTANCE_NAME ?? "");
                        string moTaTein = !string.IsNullOrEmpty(tein.RESULT_DESCRIPTION) ? tein.RESULT_DESCRIPTION : moTa;
                        rows.Add(new Qd1551ClsRow
                        {
                            MA_DICH_VU = maDichVu,
                            TEN_DICH_VU = tenDichVu,
                            MA_CHI_SO = maChiSo,
                            TEN_CHI_SO = tenChiSo,
                            GIA_TRI = tein.VALUE ?? "",
                            DON_VI_DO = tein.TEST_INDEX_UNIT_NAME ?? "",
                            MO_TA = moTaTein,
                            KET_LUAN = ketLuan,
                            LoginNameBacSi = bacSi
                        });
                    }
                    continue;
                }

                // --- Nguon 2: SUIN (chi so CDHA/TDCN) khi khong co TEIN ---
                List<V_HIS_SERE_SERV_SUIN> ssSuins = ListOrNull(suinBySs, ss.ID);
                if (ssSuins != null && ssSuins.Count > 0)
                {
                    foreach (var suin in ssSuins)
                    {
                        if (suin == null) continue;
                        string moTaSuin = !string.IsNullOrEmpty(suin.DESCRIPTION) ? suin.DESCRIPTION : moTa;
                        rows.Add(new Qd1551ClsRow
                        {
                            MA_DICH_VU = maDichVu,
                            TEN_DICH_VU = tenDichVu,
                            MA_CHI_SO = suin.SUIM_INDEX_CODE ?? "",
                            TEN_CHI_SO = suin.SUIM_INDEX_NAME ?? "",
                            GIA_TRI = suin.VALUE ?? "",
                            DON_VI_DO = suin.SUIM_INDEX_UNIT_NAME ?? "",
                            MO_TA = moTaSuin,
                            KET_LUAN = ketLuan,
                            LoginNameBacSi = bacSi
                        });
                    }
                    continue;
                }

                // --- Nguon 3: khong co chi so -> 1 dong dich vu (giong XML130 CLS khong chi so) ---
                rows.Add(new Qd1551ClsRow
                {
                    MA_DICH_VU = maDichVu,
                    TEN_DICH_VU = tenDichVu,
                    MA_CHI_SO = "",
                    TEN_CHI_SO = tenDichVu,
                    GIA_TRI = "",
                    DON_VI_DO = "",
                    MO_TA = moTa,
                    KET_LUAN = ketLuan,
                    LoginNameBacSi = bacSi
                });
            }
            return result;
        }

        /// <summary>Goi API danh sach kieu GetRO (co ApiResultObject) — dung cho HIS_TREATMENT_GET.</summary>
        private static List<T> GetListRO<T>(string uri, object filter)
        {
            try
            {
                var param = new CommonParam();
                ApiResultObject<List<T>> rs = new BackendAdapter(param).GetRO<List<T>>(uri, ApiConsumers.MosConsumer, filter, param);
                return (rs != null) ? rs.Data : null;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        /// <summary>
        /// Goi 1 call gop api/HisKskSync/GetKskData -> HisKskDataSDO (chua toan bo du lieu KSK cua CA LIST
        /// ho so theo SERVICE_REQ_IDs + TREATMENT_IDs). Loi -> null (BuildInputs coi nhu du lieu KSK rong). 
        /// </summary>
        private static MOS.SDO.HisKskDataSDO GetKskDataSdo(MOS.Filter.HisKskDataFilter filter)
        {
            try
            {
                var param = new CommonParam();
                var result = new BackendAdapter(param).Get<MOS.SDO.HisKskDataSDO>(
                    "api/HisKskSync/GetKskData", ApiConsumers.MosConsumer, filter, param);
                // API tra null / backend bao loi -> log RO ly do (truoc day nuot im lang, kho lan ra).
                if (result == null || (param.Messages != null && param.Messages.Count > 0))
                    Inventec.Common.Logging.LogSystem.Warn("GetKskData tra ve "
                        + ((result == null) ? "NULL" : "co du lieu") + "; backend messages: "
                        + ((param.Messages != null && param.Messages.Count > 0)
                            ? string.Join("; ", param.Messages.ToArray()) : "(khong co)"));
                return result;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        private static Dictionary<long, T> IndexBy<T>(List<T> list, Func<T, long> key)
        {
            var d = new Dictionary<long, T>();
            if (list != null)
                foreach (var x in list) { long k = key(x); if (k > 0 && !d.ContainsKey(k)) d[k] = x; }
            return d;
        }

        private static Dictionary<long, List<T>> GroupByKey<T>(List<T> list, Func<T, long> key)
        {
            var d = new Dictionary<long, List<T>>();
            if (list != null)
                foreach (var x in list)
                {
                    long k = key(x); if (k <= 0) continue;
                    List<T> l; if (!d.TryGetValue(k, out l)) { l = new List<T>(); d[k] = l; }
                    l.Add(x);
                }
            return d;
        }

        private static T ValOrNull<T>(Dictionary<long, T> d, long k) where T : class
        {
            T v; return (d != null && d.TryGetValue(k, out v)) ? v : null;
        }

        private static List<T> ListOrNull<T>(Dictionary<long, List<T>> d, long k)
        {
            List<T> v; return (d != null && d.TryGetValue(k, out v)) ? v : null;
        }

        #region build config / certificate / result
        /// <summary>
        /// Parse chuoi HIS_CONFIG (theo vien) -> Qd1551Config. branchCode null -> lay cau hinh dau tien.
        /// Toan bo thong tin (ke ca khoa bi mat ky checksum = truong cuoi) lay tu cau hinh he thong
        /// MOS.HIS_KSK_SYNC.CONNECTION_INFO — khong con gia tri fix cung trong code.
        /// </summary> 
        private Qd1551Config BuildConfig()
        {
            return Qd1551ConfigParser.Parse(this.connectionInfo, null) ?? new Qd1551Config();
        }

        /// <summary>
        /// Cau hinh cong HCC tu MOS.HIS_KSK_SYNC.HSSK_HCC_2062_CONNECTION_INFO. Dinh dang RIENG (cac truong
        /// cach '|', cung ho voi cong HOC) — xem KskHccConfigParser:
        ///   MaCsyt|Username|Password|ReceiverId|DataType|Version|TokenUrl|PushUrl|PrivateKey
        /// Tra null khi khong day cong HCC / chua cau hinh / chuoi sai dinh dang.
        /// </summary>
        private Qd1551Config BuildHccConfig()
        {
            if (!this.pushHcc || string.IsNullOrWhiteSpace(this.hccConnectionInfo)) return null;
            return KskHccConfigParser.Parse(this.hccConnectionInfo);
        }

        /// <summary>
        /// Dung payload base64 cho cong HCC: envelope khamsuckhoe cua DUNG 1 ho so (SOLUONGHOSO = 1) theo
        /// data_type cua cau hinh HCC.
        /// - JSON (mac dinh): thu vien xuat ban "JSON hoa" cua XML (ten khoa IN HOA, khong co lop boc)
        ///   -> chuyen sang dung cau truc tai lieu HCC muc 3.3 bang KskHccJsonConverter.
        /// - XML: ky CKS (neu bat ky so) nhu cong BYT.
        /// Tra "" khi khong dung duoc (KskHccPusher se bao that bai cho ho so do).
        /// </summary>
        private static string BuildHccPayload(string macskcb, Qd1551KskInput input,
            bool isJson, Func<string, string> dataSigner)
        {
            try
            {
                if (input == null) return "";
                // Dung envelope co DU 12 khoi (khoi thieu du lieu -> khoi trong) — xem KskEnvelopeBuilder.
                string content = KskEnvelopeBuilder.Build(new List<Qd1551KskInput> { input }, macskcb, isJson);
                if (string.IsNullOrEmpty(content)) return "";
                if (isJson)
                {
                    content = KskHccJsonConverter.ToHccJson(content);
                    if (string.IsNullOrEmpty(content)) return "";
                }
                else if (dataSigner != null)
                {
                    string signed = dataSigner(content);
                    if (!string.IsNullOrEmpty(signed)) content = signed;
                }
                LogPayloadBlocks(content, isJson);   // log cac khoi XMLn thuc su co trong ban tin day HCC
                // Dump NGUYEN VAN ban tin truoc khi base64 (DEBUG) — khoi phai giai base64 trong log de doi chieu.
                DumpDebug("BanTin_HCC_" + (isJson ? "json" : "xml"), content);
                return new DataProcessorBase().EncodeBase64(content);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return "";
            }
        }

        #region log cau hinh cong (mask thong tin bi mat)
        private const string CFG_KEY_BYT = "MOS.HIS_KSK_SYNC.CONNECTION_INFO";
        private const string CFG_KEY_HSSK = "MOS.HIS_KSK_SYNC.HSSK_HN_2062_CONNECTION_INFO";
        private const string CFG_KEY_HOC = "MOS.HIS_KSK_SYNC.HSSK_HOC_2062_CONNECTION_INFO";
        private const string CFG_KEY_HCC = "MOS.HIS_KSK_SYNC.HSSK_HCC_2062_CONNECTION_INFO";

        /// <summary>
        /// Ghi log gia tri cau hinh CUA TUNG CONG vua lay duoc (1 dong/cong) de doi soat khi bam Dong bo:
        /// cong nao chua cau hinh, cong nao co cau hinh nhung khong chon day, cong nao parse loi, cong nao
        /// lay duoc gi (URL / tai khoan / ma don vi / data_type...). MAT KHAU va KHOA BI MAT chi log
        /// co/khong + do dai — KHONG bao gio ghi gia tri thuc ra file log.
        /// </summary>
        private void LogGatewayConfigs(Qd1551Config bytConfig, Qd1551Config hsskConfig,
            HocConfig hocConfig, Qd1551Config hccConfig)
        {
            try
            {
                LogQd1551Config("BYT", CFG_KEY_BYT, this.connectionInfo, this.pushByt, bytConfig);
                LogQd1551Config("HSSK", CFG_KEY_HSSK, this.hsskConnectionInfo, this.pushHssk, hsskConfig);
                LogHocConfig(this.hocConnectionInfo, this.pushHoc, hocConfig);
                LogQd1551Config("HCC", CFG_KEY_HCC, this.hccConnectionInfo, this.pushHcc, hccConfig);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Log 1 dong cau hinh cong dung Qd1551Config (BYT / HSSK / HCC).</summary>
        private static void LogQd1551Config(string gateway, string configKey, string rawValue,
            bool selected, Qd1551Config cfg)
        {
            if (!LogConfigState(gateway, configKey, rawValue, selected, cfg == null)) return;
            Inventec.Common.Logging.LogSystem.Info(string.Format(
                "Cau hinh {0} ({1}): SenderId(ma don vi)={2}; Username={3}; Password={4}; BaseUrl={5};"
                + " LoginUri={6}; PushUri={7}; DataType={8}; ReceiverId={9}; Version={10}; TxnType={11};"
                + " MsgType={12}; PrivateKey={13}",
                gateway, configKey, Show(cfg.SenderId), Show(cfg.Username), Mask(cfg.Password),
                Show(cfg.BaseUrl), Show(cfg.LoginUri), Show(cfg.PushUri), Show(cfg.DataType),
                Show(cfg.ReceiverId), Show(cfg.Version), Show(cfg.TxnType), Show(cfg.MsgType),
                Mask(cfg.ChecksumPrivateKeyPem)));
        }

        /// <summary>Log 1 dong cau hinh cong HOC (HocConfig — cau truc rieng, co URL hieu luc).</summary>
        private static void LogHocConfig(string rawValue, bool selected, HocConfig cfg)
        {
            if (!LogConfigState("HOC", CFG_KEY_HOC, rawValue, selected, cfg == null)) return;
            Inventec.Common.Logging.LogSystem.Info(string.Format(
                "Cau hinh HOC ({0}): MaCsyt={1}; MaTinh={2}; Username={3}; Password={4}; ClientId={5};"
                + " GrantType={6}; TokenUrl={7}; PushUrl={8}; PrivateKey={9}",
                CFG_KEY_HOC, Show(cfg.MaCsyt), Show(cfg.MaTinh), Show(cfg.Username), Mask(cfg.Password),
                Show(cfg.ClientId), Show(cfg.EffectiveGrantType), Show(cfg.EffectiveTokenUrl),
                Show(cfg.EffectivePushUrl), Mask(cfg.ChecksumPrivateKeyPem)));
        }

        /// <summary>
        /// Log trang thai chung cua 1 cong (chua cau hinh / khong chon / parse loi). Tra true khi CAN log
        /// tiep chi tiet gia tri cau hinh.
        /// </summary>
        private static bool LogConfigState(string gateway, string configKey, string rawValue,
            bool selected, bool parsedNull)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                Inventec.Common.Logging.LogSystem.Info(string.Format(
                    "Cau hinh {0} ({1}): CHUA CAU HINH -> khong day cong nay.", gateway, configKey));
                return false;
            }
            if (!selected)
            {
                Inventec.Common.Logging.LogSystem.Info(string.Format(
                    "Cau hinh {0} ({1}): CO cau hinh (do dai {2}) nhung KHONG chon day.",
                    gateway, configKey, rawValue.Trim().Length));
                return false;
            }
            if (parsedNull)
            {
                Inventec.Common.Logging.LogSystem.Warn(string.Format(
                    "Cau hinh {0} ({1}): PARSE LOI / THIEU TRUONG BAT BUOC (do dai chuoi {2})"
                    + " -> khong day duoc cong nay.", gateway, configKey, rawValue.Trim().Length));
                return false;
            }
            return true;
        }

        /// <summary>Gia tri rong -> "(rong)". Cac gia tri KHONG bi mat duoc log nguyen van.</summary>
        private static string Show(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "(rong)" : value;
        }

        /// <summary>Mat khau / khoa bi mat: chi log co-khong + do dai, KHONG log gia tri thuc.</summary>
        private static string Mask(string value)
        {
            return string.IsNullOrEmpty(value) ? "(rong)" : ("***(len=" + value.Length + ")");
        }
        #endregion

        /// <summary>
        /// Log danh sach khoi ho so (LOAIHOSO = XML1..XML12) THUC SU co trong ban tin day cong HCC,
        /// doc truc tiep tu chuoi ban tin (JSON: "loaihoso":"XMLn"; XML: &lt;LOAIHOSO&gt;XMLn&lt;/LOAIHOSO&gt;).
        /// Khoi khong co du lieu nguon thi thu vien KHONG sinh ra — xem them log "Du lieu KSK ..." de biet ly do.
        /// </summary>
        private static void LogPayloadBlocks(string content, bool isJson)
        {
            try
            {
                if (string.IsNullOrEmpty(content)) return;
                var blocks = new List<string>();
                var matches = System.Text.RegularExpressions.Regex.Matches(content,
                    "(?:\"loaihoso\"\\s*:\\s*\"|<LOAIHOSO>)\\s*(XML\\d+)",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                foreach (System.Text.RegularExpressions.Match m in matches)
                {
                    string block = m.Groups[1].Value.ToUpperInvariant();
                    if (!blocks.Contains(block)) blocks.Add(block);
                }
                Inventec.Common.Logging.LogSystem.Info(string.Format(
                    "Ban tin day HCC ({0}): {1} khoi ho so -> {2}",
                    isJson ? "json/base64" : "xml/base64",
                    blocks.Count,
                    (blocks.Count > 0) ? string.Join(", ", blocks.ToArray()) : "(KHONG co khoi nao)"));
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Lay chung thu so (co private key) theo serial da chon o SettingSignInfo khi bat ky so.</summary>
        private X509Certificate2 LoadCertificate()
        {
            try
            {
                if (!this.sign || this.signSetting == null || string.IsNullOrEmpty(this.signSetting.SerialNumber))
                    return null;
                return Inventec.Common.SignFile.CertUtil.GetBySerial(this.signSetting.SerialNumber, requirePrivateKey: true, validOnly: false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>
        /// Gop ket qua cua cac cong THU VIEN day (BYT/HSSK/HOC — pushResult) va cong HCC (hccResult) thanh
        /// 1 dong trang thai cua ho so. Thanh cong = TAT CA cong da day deu thanh cong (giong PushListMulti).
        /// Ma giao dich / trang thai ghep dang "BYT:xxx;HSSK:yyy;HCC:zzz" (chi them tien to khi >1 cong).
        /// libSingleLabel = ten cong duy nhat do thu vien day (null neu thu vien day >1 cong -> da co tien to).
        /// configError != null: co cong da chon nhung chuoi cau hinh sai dinh dang -> ho so LUON that bai.
        /// </summary>
        private KskSyncResultADO BuildResultAdo(V_HIS_KSK_SYNC row, ResultADO pushResult,
            KskHccPushResult hccResult, long syncTime, string libSingleLabel, string configError)
        {
            KskSyncResultADO ado = NewResult(row, syncTime);
            PushResponse resp = ExtractResponse(pushResult);
            bool hasLib = pushResult != null;
            bool hasHcc = hccResult != null;
            bool libOk = hasLib && pushResult.Success;
            bool hccOk = hasHcc && hccResult.Success;
            bool success = (hasLib || hasHcc) && (!hasLib || libOk) && (!hasHcc || hccOk)
                        && string.IsNullOrEmpty(configError);

            ado.SYNC_RESULT_TYPE = success ? RESULT_SUCCESS : RESULT_FAILED;
            // Ma giao dich / trang thai: uu tien tu PushResponse cong BYT; fallback Data[2]/Data[3]
            // (chuoi do PushListMulti chuan hoa — dung cho cong HSSK/HOC, response khac kieu).
            string txn = (resp != null) ? resp.TxnId : null;
            string regState = (resp != null && resp.Data != null) ? resp.Data.DataState : null;
            if (pushResult != null && pushResult.Data != null)
            {
                if (string.IsNullOrEmpty(txn) && pushResult.Data.Length > 2) txn = pushResult.Data[2] as string;
                if (string.IsNullOrEmpty(regState) && pushResult.Data.Length > 3) regState = pushResult.Data[3] as string;
            }
            ado.TRANSACTION_CODE = JoinGatewayValue(txn, hasHcc ? hccResult.TxnCode : null, libSingleLabel);
            ado.REGISTRATION_NO = JoinGatewayValue(regState, hasHcc ? hccResult.State : null, libSingleLabel);
            if (!success)
            {
                var reasons = new List<string>();
                if (hasLib && !libOk)
                    reasons.Add(!string.IsNullOrEmpty(pushResult.Message) ? pushResult.Message : "Đồng bộ thất bại");
                if (hasHcc && !hccOk)
                    reasons.Add(!string.IsNullOrEmpty(hccResult.Message) ? hccResult.Message : "HCC: đồng bộ thất bại");
                if (!string.IsNullOrEmpty(configError)) reasons.Add(configError);
                if (!hasLib && !hasHcc && string.IsNullOrEmpty(configError))
                    reasons.Add("Chưa chọn cổng liên thông để đẩy");
                ado.SYNC_FAILD_REASON = string.Join(" | ", reasons);
            }
            return ado;
        }

        /// <summary>
        /// Ghep gia tri cua cac cong: giu nguyen chuoi cua thu vien (da co tien to khi >1 cong), them
        /// "HCC:" khi co ca 2 nguon. Chi 1 nguon -> tra gia tri tran (khong tien to) nhu truoc day.
        /// </summary>
        private static string JoinGatewayValue(string libValue, string hccValue, string libSingleLabel)
        {
            bool hasLib = !string.IsNullOrEmpty(libValue);
            bool hasHcc = !string.IsNullOrEmpty(hccValue);
            if (!hasLib && !hasHcc) return null;
            if (hasLib && !hasHcc) return libValue;
            if (!hasLib && hasHcc) return hccValue;
            // Ca 2 nguon: cong thu vien duy nhat thi bo sung tien to ten cong cho de doi soat.
            string left = string.IsNullOrEmpty(libSingleLabel) ? libValue : libSingleLabel + ":" + libValue;
            return left + ";HCC:" + hccValue;
        }

        private KskSyncResultADO BuildFailedResult(V_HIS_KSK_SYNC row, long syncTime, string reason)
        {
            KskSyncResultADO ado = NewResult(row, syncTime);
            ado.SYNC_RESULT_TYPE = RESULT_FAILED;
            ado.SYNC_FAILD_REASON = reason;
            return ado;
        }

        private static KskSyncResultADO NewResult(V_HIS_KSK_SYNC row, long syncTime)
        {
            return new KskSyncResultADO
            {
                KSK_TYPE_ID = ToLong(GetProp(row, "KSK_TYPE_ID")),
                KSK_RECORD_ID = ToLong(GetProp(row, "KSK_RECORD_ID")),
                PATIENT_CODE = SafeString(GetProp(row, "TDL_PATIENT_CODE")),
                KskTypeName = SafeString(GetProp(row, "KSK_TYPE_NAME")),
                SYNC_TIME = syncTime
            };
        }

        /// <summary>ResultADO.Data cua PushList = [PushResponse (hoac null), tag].</summary>
        private static PushResponse ExtractResponse(ResultADO r)
        {
            if (r == null || r.Data == null || r.Data.Length == 0) return null;
            return r.Data[0] as PushResponse;
        }

        private static V_HIS_KSK_SYNC ExtractTag(ResultADO r)
        {
            if (r == null || r.Data == null || r.Data.Length < 2) return null;
            return r.Data[1] as V_HIS_KSK_SYNC;
        }
        #endregion

        #region helper
        private static object GetProp(object obj, string name)
        {
            try
            {
                if (obj == null) return null;
                var p = obj.GetType().GetProperty(name);
                return p != null ? p.GetValue(obj, null) : null;
            }
            catch { return null; }
        }
        private static string SafeString(object o) { return o == null ? "" : o.ToString(); }
        private static long ToLong(object o)
        {
            try { return o == null ? 0 : Convert.ToInt64(o); }
            catch { return 0; }
        }
        #endregion
    }
}

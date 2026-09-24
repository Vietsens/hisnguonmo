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
        private readonly string vlgConnectionInfo;      // cong KDLYT Vinh Long (MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO)
        private readonly bool pushByt;                  // co day cong BYT
        private readonly bool pushHssk;                 // co day cong HSSK
        private readonly bool pushHoc;                  // co day cong HOC
        private readonly bool pushHcc;                  // co day cong HCC
        private readonly bool pushVlg;                  // co day cong KDLYT Vinh Long

        /// <summary>
        /// Dữ liệu nguồn để dựng bản tin cổng Sở Y tế TP.HCM, khóa = mã y lệnh KSK.
        /// Dựng cùng lúc với dữ liệu của 4 cổng cũ để không phải tải lại lần hai.
        /// </summary>
        private readonly Dictionary<long, KskSytHcmSource> sytSourceBySr
            = new Dictionary<long, KskSytHcmSource>();
        private readonly bool sign;
        private readonly SettingSignADO signSetting;

        // Ket qua dong bo (khop KskSyncResultADO.SYNC_RESULT_TYPE): 2 = thanh cong, 3 = that bai.
        private const short RESULT_SUCCESS = 2;
        private const short RESULT_FAILED = 3;
        // 4 = "Co chinh sua": backend danh dau khi ho so KSK bi sua SAU lan dong bo -> can day lai ban moi.
        private const short RESULT_EDITED = 4;

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

        /// <summary>Ctor 4 cong (BYT + HSSK + HOC + HCC) — giu tuong thich cho cac loi goi cu.</summary>
        internal KskSyncProcessor(string connectionInfo, string hsskConnectionInfo, string hocConnectionInfo,
            string hccConnectionInfo, bool pushByt, bool pushHssk, bool pushHoc, bool pushHcc,
            bool sign, SettingSignADO signSetting)
            : this(connectionInfo, hsskConnectionInfo, hocConnectionInfo, hccConnectionInfo, null,
                   pushByt, pushHssk, pushHoc, pushHcc, false, sign, signSetting)
        {
        }

        /// <summary>
        /// Ctor day da cong: chon day BYT (pushByt), HSSK (pushHssk), HOC->TTYTQG (pushHoc), HCC (pushHcc)
        /// va/hoac KDLYT Vinh Long (pushVlg). BYT/HSSK/HOC dung CHUNG 1 base64 (thu vien
        /// CreateQd1551Main.PushListMulti xu ly), chi khac API dang nhap + endpoint day + cach dong goi body.
        /// HCC dung base64 RIENG (mac dinh json/base64 theo tai lieu HCC) nen dung payload rieng roi day
        /// bang KskHccPusher — xem KskHccPusher de biet giao thuc.
        /// VLG (Cong tiep nhan Vinh Long) dung giao thuc RIENG hoan toan: token /api/xac-thuc/token +
        /// POST XML KHAMSUCKHOE truc tiep (khong base64) — xem KskVlgPusher de biet giao thuc.
        /// </summary>
        internal KskSyncProcessor(string connectionInfo, string hsskConnectionInfo, string hocConnectionInfo,
            string hccConnectionInfo, string vlgConnectionInfo, bool pushByt, bool pushHssk, bool pushHoc,
            bool pushHcc, bool pushVlg, bool sign, SettingSignADO signSetting)
        {
            this.connectionInfo = connectionInfo;
            this.hsskConnectionInfo = hsskConnectionInfo;
            this.hocConnectionInfo = hocConnectionInfo;
            this.hccConnectionInfo = hccConnectionInfo;
            this.vlgConnectionInfo = vlgConnectionInfo;
            this.pushByt = pushByt;
            this.pushHssk = pushHssk;
            this.pushHoc = pushHoc;
            this.pushHcc = pushHcc;
            this.pushVlg = pushVlg;
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
                            bool hasConcCert = concSigners.TryGetValue(concLogin, out concEmr);
                            Inventec.Common.Logging.LogSystem.Info(string.Format(
                                "CKS_NGUOI_KET_LUAN: file nhom nguoi ket luan {0} ({1} ho so): chung thu HSM={2}",
                                string.IsNullOrEmpty(concLogin) ? "(trong)" : concLogin, idxs.Count,
                                hasConcCert ? "co -> se ky" : "KHONG -> bo qua the"));
                            if (hasConcCert)
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
                // Cấu hình đã chọn theo BRANCH_ID ở KskBranchConfig.GetValue; ở đây chỉ CẢNH BÁO nếu
                // trường [0] BranchCode của chuỗi khác chi nhánh đang làm việc — KHÔNG chặn đẩy
                // (truyền mã chi nhánh vào Parse sẽ làm parser trả null -> mất cả cổng).
                KskBranchConfig.WarnIfBranchCodeMismatch(this.connectionInfo, "MOS.HIS_KSK_SYNC.CONNECTION_INFO");
                Qd1551Config bytConfig = Qd1551ConfigParser.Parse(this.connectionInfo, null);
                CreateQd1551Main main = new CreateQd1551Main(bytConfig ?? new Qd1551Config());   // 1 instance -> token cache dùng chung
                X509Certificate2 certificate = LoadCertificate();
                List<Qd1551KskInput> inputs = BuildInputs(rowList);            // nạp batch (nhiều hồ sơ)

                Qd1551Config hsskConfig = null;
                if (this.pushHssk && !string.IsNullOrWhiteSpace(this.hsskConnectionInfo))
                {
                    KskBranchConfig.WarnIfBranchCodeMismatch(this.hsskConnectionInfo,
                        "MOS.HIS_KSK_SYNC.HSSK_HN_2062_CONNECTION_INFO");
                    hsskConfig = Qd1551ConfigParser.Parse(this.hsskConnectionInfo, null);
                }

                HocConfig hocConfig = null;
                if (this.pushHoc && !string.IsNullOrWhiteSpace(this.hocConnectionInfo))
                    hocConfig = HocConfigParser.Parse(this.hocConnectionInfo);

                // Cổng HCC: cùng giao thức trục BYT nhưng data_type mặc định json/base64 -> payload RIÊNG,
                // dựng bằng 1 instance CreateQd1551Main theo cấu hình HCC; token cache trong 1 KskHccPusher.
                Qd1551Config hccConfig = BuildHccConfig();
                string hccMacskcb = (hccConfig != null) ? (hccConfig.SenderId ?? "") : "";
                KskHccPusher hccPusher = (hccConfig != null) ? new KskHccPusher(hccConfig) : null;

                // Cổng KDLYT Vĩnh Long — API V1.5 /api/platform/data-sync/push (cấu trúc trục Bộ Y tế, QĐ 2062
                // Phụ lục 02): token Kho /api/xac-thuc/token; token cache trong 1 KskVlgPusher dùng chung cả lô.
                // Mã 13 số (GTIN/GLN) dùng cho header.sender_id VÀ THONGTINDONVI/MACSKCB — y như luồng trục BYT
                // (kiểm chứng cổng dev 24/09/2026: Kho VALID, phân loại đúng mẫu phiếu). Thiếu mã -> chặn ở pre-gate.
                KskVlgConfig vlgConfig = BuildVlgConfig();
                string vlgGtin = (vlgConfig != null) ? ResolveVlgSenderGtin(vlgConfig, bytConfig) : null;
                string vlgMacskcb = vlgGtin ?? "";
                KskVlgPusher vlgPusher = (vlgConfig != null) ? new KskVlgPusher(vlgConfig, vlgGtin) : null;

                // Các cổng do THƯ VIỆN đẩy (BYT/HSSK/HOC). Không có cổng nào -> KHÔNG gọi PushListMulti
                // (gọi rỗng sẽ trả về "thành công" giả vì không cổng nào đánh dấu thất bại).
                bool pushViaLibrary = this.pushByt || hsskConfig != null || hocConfig != null;
                int libGatewayCount = (this.pushByt ? 1 : 0) + (hsskConfig != null ? 1 : 0) + (hocConfig != null ? 1 : 0);
                string libSingleLabel = (libGatewayCount == 1)
                    ? (this.pushByt ? "BYT" : (hsskConfig != null ? "HSSK" : "HOC"))
                    : null;

                // Ghi log giá trị cấu hình TỪNG CỔNG vừa lấy được (mật khẩu / khóa bí mật đã mask).
                LogGatewayConfigs(bytConfig, hsskConfig, hocConfig, hccConfig, vlgConfig);

                // Cổng ĐÃ CHỌN + CÓ chuỗi cấu hình nhưng PARSE LỖI (sai định dạng) -> KHÔNG bỏ qua âm thầm:
                // ghi nhận để đánh dấu hồ sơ thất bại kèm lý do (nếu không, hồ sơ vẫn "thành công" nhờ cổng khác).
                var configErrorList = new List<string>();
                if (this.pushHssk && !string.IsNullOrWhiteSpace(this.hsskConnectionInfo) && hsskConfig == null)
                    configErrorList.Add("HSSK: chuỗi cấu hình sai định dạng (MOS.HIS_KSK_SYNC.HSSK_HN_2062_CONNECTION_INFO)");
                if (this.pushHoc && !string.IsNullOrWhiteSpace(this.hocConnectionInfo) && hocConfig == null)
                    configErrorList.Add("HOC: chuỗi cấu hình sai định dạng (MOS.HIS_KSK_SYNC.HSSK_HOC_2062_CONNECTION_INFO)");
                if (this.pushHcc && !string.IsNullOrWhiteSpace(this.hccConnectionInfo) && hccConfig == null)
                    configErrorList.Add("HCC: chuỗi cấu hình sai định dạng (MOS.HIS_KSK_SYNC.HSSK_HCC_2062_CONNECTION_INFO)");
                if (this.pushVlg && !string.IsNullOrWhiteSpace(this.vlgConnectionInfo) && vlgConfig == null)
                    configErrorList.Add("VLG: chuỗi cấu hình sai định dạng (MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO)");
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
                if (this.PushSytHcm) gateways.Add("SYT-HCM");
                if (this.pushByt) gateways.Add("BYT");
                if (hsskConfig != null) gateways.Add("HSSK");
                if (hocConfig != null) gateways.Add("HOC");
                if (hccConfig != null) gateways.Add("HCC" + (hccIsJson ? "(json)" : "(xml)"));
                if (vlgConfig != null) gateways.Add("VLG(data-sync/push)");
                Inventec.Common.Logging.LogSystem.Info(string.Format(
                    "Dong bo KSK: {0} ho so -> cong: {1}; ky so: {2}{3}",
                    rowList.Count,
                    (gateways.Count > 0) ? string.Join(", ", gateways.ToArray()) : "(khong co cong nao)",
                    doSign ? "co" : "khong",
                    (configError != null) ? ("; LOI CAU HINH: " + configError) : ""));
                // Có cổng nào NGOÀI VLG được đẩy trong lô không (quyết định lỗi cấu hình VLG có chặn cả hồ sơ không).
                bool otherGatewaySelected = pushViaLibrary || hccPusher != null || this.PushSytHcm;
                bool dupBytVlg = vlgConfig != null && (this.pushByt || hocConfig != null);
                if (dupBytVlg)
                    Inventec.Common.Logging.LogSystem.Warn("Dong bo KSK: cong VLG (API V1.5) DA TU chuyen tiep ban tin sang"
                        + " Cong Bo Y te (receiver TTYQG); lo nay con tich them BYT/HOC -> Bo co the nhan 2 ban tin cho"
                        + " cung ho so. Vien dung VLG nen bo tich cong BYT/HOC.");

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
                            if (vlgPusher != null) MarkVlgNotSent(ado, rowList[i]);
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
                                Inventec.Common.Logging.LogSystem.Info(string.Format(
                                    "CKS_NGUOI_KET_LUAN: ho so {0} (treatment_code={1}): CONCLUDER_LOGINNAME={2};"
                                    + " chung thu HSM={3}",
                                    i + 1, SafeString(GetProp(rowList[i], "TREATMENT_CODE")),
                                    string.IsNullOrEmpty(cl) ? "(trong)" : cl,
                                    (emrLocal != null) ? "co -> se ky" : "KHONG -> bo qua the"));
                                // Ký CKS_NGUOI_KET_LUAN (chứng thư người kết luận) TRƯỚC, rồi CKS_BENH_VIEN.
                                dataSigner = xml => signer.SignCksBenhVien(emrLocal != null ? signer.SignXmlByConcluder(xml, emrLocal) : xml);
                            }
                            // ĐẨY ĐÚNG 1 HỒ SƠ.
                            // ===== VLG PRE-GATE: sai dữ liệu bắt buộc / vượt độ dài QĐ 2062 -> CHẶN CỨNG,
                            // KHÔNG đồng bộ lên BẤT KỲ cổng nào (không đẩy BYT/HSSK/HOC/HCC/SYT). Chỉ áp
                            // khi viện có chọn cổng VLG; viện không dùng VLG -> giữ nguyên luồng cũ.
                            string vlgBlockReason = null;
                            string vlgPayload = null;
                            bool vlgSignFailed = false;
                            // Kết quả RIÊNG cổng VLG đã biết trước khi đẩy (thiếu mã 13 số / lần gửi trước mất
                            // phản hồi đã vào Kho / chưa đối soát được) -> CHỈ bỏ qua lệnh đẩy VLG, các cổng
                            // khác vẫn đẩy bình thường. Khác vlgBlockReason (lỗi DỮ LIỆU -> chặn mọi cổng).
                            KskVlgPushResult vlgKnownResult = null;
                            bool vlgGtinOk = KskVlgConfigParser.IsGtin13(vlgGtin);
                            if (vlgPusher != null && !vlgGtinOk && !otherGatewaySelected)
                            {
                                // Chỉ đẩy VLG mà thiếu mã 13 số: không dựng/ký bản tin vô ích cho từng hồ sơ.
                                vlgKnownResult = KskVlgPushResult.Failure(vlgPusher.DescribeMissingGtin());
                            }
                            else if (vlgPusher != null)
                            {
                                vlgBlockReason = ValidateVlgInput(inp);   // thiếu CCCD / lý do khám / MA_LOAI_KCB > 2
                                if (vlgBlockReason == null)
                                {
                                    // Dựng bản tin (ký CKS_) rồi kiểm độ dài trên XML thực sự gửi đi
                                    // (kể cả giá trị thư viện tự sinh, giải mã NOIDUNGFILE base64).
                                    vlgPayload = BuildVlgPayload(vlgMacskcb, inp, dataSigner, out vlgSignFailed);
                                    if (vlgSignFailed)
                                        vlgBlockReason = "VLG: ký số thất bại (CKS_BENH_VIEN/CKS_NGUOI_KET_LUAN)"
                                            + " — không đẩy bản tin chưa ký. Kiểm tra cấu hình chứng thư / HSM / USB token.";
                                    else
                                    {
                                        // Thiếu trường bắt buộc (Đối tượng / Nguồn chi trả) + vượt độ dài QĐ 2062.
                                        var vlgReasons = new List<string>();
                                        string reqReason = KskVlgLengthRules.ValidateRequired(vlgPayload);
                                        if (reqReason != null) vlgReasons.Add(reqReason);
                                        string lenReason = KskVlgLengthRules.Validate(vlgPayload);
                                        if (lenReason != null) vlgReasons.Add(lenReason);
                                        if (vlgReasons.Count > 0) vlgBlockReason = string.Join(" | ", vlgReasons.ToArray());
                                    }
                                }
                                if (vlgBlockReason == null)
                                {
                                    if (!vlgGtinOk)
                                        vlgKnownResult = KskVlgPushResult.Failure(vlgPusher.DescribeMissingGtin());   // lỗi cấu hình VLG
                                    else
                                        vlgKnownResult = CheckPreviousVlgUnknown(vlgPusher, rowList[i]);
                                }
                            }

                            if (vlgBlockReason != null)
                            {
                                // Chặn cứng vì lỗi DỮ LIỆU: KHÔNG gọi push cổng nào. BuildResultAdo giữ
                                // TRANSACTION_CODE/REGISTRATION_NO cũ (không mất mã đối soát của lần trước).
                                ado = BuildResultAdo(rowList[i], null, null, KskVlgPushResult.Failure(vlgBlockReason),
                                    syncTime, libSingleLabel, configError, null);
                            }
                            else
                            {
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

                            // Cổng KDLYT Vĩnh Long (nếu chọn) — payload đã dựng + đã kiểm ở pre-gate trên;
                            // kết quả đã biết trước (vlgKnownResult) -> không gửi lại, dùng luôn để gộp.
                            KskVlgPushResult vlgResult = vlgKnownResult;
                            if (vlgPusher != null && vlgResult == null)
                                vlgResult = vlgPusher.Push(vlgPayload, SafeString(GetProp(rowList[i], "TDL_TREATMENT_CODE")));

                            // Cổng Sở Y tế TP.HCM (mẫu M3) — hàm này tự bọc try/catch nên lỗi ở
                            // cổng này KHÔNG làm hỏng kết quả của các cổng trên.
                            KskSytHcmPushResult sytResult = PushSytHcmOneRow(rowList[i]);

                            ado = BuildResultAdo(rowList[i], r0, hccResult, vlgResult, syncTime,
                                libSingleLabel, configError, sytResult);
                            if (dupBytVlg && ado.SYNC_RESULT_TYPE == RESULT_SUCCESS)
                                ado.SuccessNote = (string.IsNullOrEmpty(ado.SuccessNote) ? "" : (ado.SuccessNote + "; "))
                                    + "Lưu ý: đang tích cả cổng BYT/HOC và Vĩnh Long — Kho Vĩnh Long đã tự chuyển tiếp sang Bộ Y tế,"
                                    + " Bộ sẽ nhận 2 bản tin. Nên bỏ tích cổng BYT/HOC.";
                            }

                        }
                    }
                    catch (Exception exRow)
                    {
                        Inventec.Common.Logging.LogSystem.Error(exRow);
                        ado = BuildFailedResult(rowList[i], syncTime, exRow.Message);
                        MarkVlgNotSent(ado, rowList[i]);
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
                    MarkVlgNotSent(ado, rowList[i]);
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
        #region ===== Cổng thứ năm — Sở Y tế TP.HCM (mẫu M3) =====

        private const string CFG_KEY_SYT_HCM = "MOS.HIS_KSK_SYNC.SYT_HCM_CONNECTION_INFO";

        /// <summary>
        /// Có đẩy sang cổng Sở Y tế TP.HCM hay không — theo ô tích trong bảng Cài đặt.
        /// Đặt qua thuộc tính thay vì thêm tham số hàm khởi tạo, để không phải sửa 4 cổng cũ.
        /// </summary>
        internal bool PushSytHcm { get; set; }

        /// <summary>
        /// Bảng khai báo nối chỉ số cận lâm sàng, do màn hình đồng bộ truyền vào (lưu tại máy qua
        /// ControlState). Rỗng = chưa khai báo -> khối cận lâm sàng gửi rỗng.
        /// </summary>
        internal string SytClsMapJson { get; set; }

        private string sytHcmConnectionInfoCache;
        private bool sytHcmConfigRead = false;

        /// <summary>
        /// Cấu hình cổng đã tách sẵn, dùng cho CẢ đợt đẩy. Bản ghi cấu hình giống nhau ở mọi hồ sơ
        /// nên tách lại cho từng hồ sơ là làm không. Giữ luôn danh sách trường còn thiếu để câu
        /// thông báo cũng không phải dựng lại.
        /// </summary>
        private KskSytHcmConfig sytHcmCfgParsed;
        private string sytHcmCfgParsedFrom;
        private string sytHcmCfgMissing;

        private KskSytHcmConfig GetSytHcmConfigOnce(string raw)
        {
            if (sytHcmCfgParsedFrom == raw) return sytHcmCfgParsed;
            sytHcmCfgParsed = KskSytHcmConfig.Parse(raw);
            sytHcmCfgMissing = (sytHcmCfgParsed != null)
                ? sytHcmCfgParsed.DescribeMissing() : "khong doc duoc";
            sytHcmCfgParsedFrom = raw;
            return sytHcmCfgParsed;
        }

        /// <summary>
        /// Chuỗi cấu hình cổng Sở Y tế TP.HCM. Đọc thẳng từ nguồn để sửa cấu hình là ăn ngay,
        /// không phải khởi động lại chương trình.
        /// </summary>
        private string sytHcmConnectionInfo
        {
            get
            {
                if (sytHcmConfigRead) return sytHcmConnectionInfoCache;
                sytHcmConfigRead = true;
                try
                {
                    // Doc tuoi (bo cache) VA theo dung chi nhanh dang lam viec — nhieu co so chung 1 DB.
                    sytHcmConnectionInfoCache = KskBranchConfig.GetValueFresh(CFG_KEY_SYT_HCM);
                }
                catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
                return sytHcmConnectionInfoCache;
            }
        }

        /// <summary>
        /// Đẩy MỘT hồ sơ sang cổng Sở Y tế TP.HCM.
        ///
        /// Dữ liệu THẬT lấy từ cơ sở dữ liệu; RIÊNG khối cận lâm sàng đang dùng dữ liệu giả theo
        /// mẫu của Sở vì phần đọc theo bảng nối 34 chỉ số chưa xong — xem KskSytHcmFakeData.
        ///
        /// Chưa khai báo cấu hình -> không làm gì. Lỗi ở đây KHÔNG ảnh hưởng 4 cổng còn lại.
        /// </summary>
        private KskSytHcmPushResult PushSytHcmOneRow(V_HIS_KSK_SYNC row)
        {
            try
            {
                if (!this.PushSytHcm) return null;   // khong tich cong nay -> khong lam gi
                string raw = this.sytHcmConnectionInfo;
                if (string.IsNullOrWhiteSpace(raw)) return null;

                long sr = ToLong(GetProp(row, "SERVICE_REQ_ID"));
                KskSytHcmSource src = ValOrNull(sytSourceBySr, sr);
                if (src == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("SytHcm: y lenh " + sr
                        + " khong co du lieu ho so KSK tren 18 tuoi -> bo qua cong SYT TP.HCM");
                    return new KskSytHcmPushResult
                    {
                        Message = "Hồ sơ không phải Khám sức khỏe người từ 18 tuổi trở lên"
                    };
                }

                KskSytHcmConfig cfg = GetSytHcmConfigOnce(raw);
                if (cfg == null || !cfg.CanPush)
                {
                    string missingCfgFields = sytHcmCfgMissing;
                    Inventec.Common.Logging.LogSystem.Warn("SytHcm: cau hinh thieu truong ["
                        + missingCfgFields + "] -> khong day");
                    return new KskSytHcmPushResult
                    {
                        Message = "Cấu hình cổng Sở Y tế TP.HCM còn thiếu: " + missingCfgFields
                    };
                }

                // false = KHONG dung du lieu gia nua. Chua khai bao noi chi so can lam sang thi
                // khoi do gui RONG — dung voi thuc te, thay vi gui so lieu bia.
                object body = KskSytHcmBodyBuilder.Build(src, false);

                // Kiem TRUOC khi goi cong: liet ke MOT LUOT moi truong bat buoc con trong.
                // Cong chi che tung truong mot moi lan gui nen khong kiem truoc thi phai day rat nhieu lan.
                List<string> missing = KskSytHcmBodyBuilder.DescribeMissingRequired(body);
                if (missing != null && missing.Count > 0)
                {
                    string msg = "Hồ sơ còn thiếu " + missing.Count + " thông tin bắt buộc: "
                        + string.Join("; ", missing.ToArray());
                    Inventec.Common.Logging.LogSystem.Warn("SytHcm: y lenh " + sr + " -> " + msg);
                    return new KskSytHcmPushResult { Message = msg };
                }

                KskSytHcmPushResult r = KskSytHcmPusher.Push(cfg, body,
                    KskSytHcmBodyBuilder.IsElderlyForm(src));
                Inventec.Common.Logging.LogSystem.Info("SytHcm: y lenh " + sr + " -> "
                    + (r != null ? r.ToString() : "khong co ket qua"));
                return r;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return new KskSytHcmPushResult { Message = "Lỗi khi đẩy cổng Sở Y tế TP.HCM" };
            }
        }

        #endregion

        /// <summary>
        /// Kết quả chỉ số xét nghiệm của MỘT đợt điều trị.
        ///
        /// Kết quả được lấy một lượt cho cả lô hồ sơ nên phải lọc lại theo đợt điều trị, nếu không
        /// hồ sơ này sẽ mang kết quả của bệnh nhân khác trong cùng lượt đẩy. Đường liên kết:
        /// kết quả -> dịch vụ (SERE_SERV_ID) -> đợt điều trị (TDL_TREATMENT_ID).
        /// </summary>
        /// <summary>
        /// Kết quả chẩn đoán hình ảnh / siêu âm của đúng đợt điều trị (`HIS_SERE_SERV_EXT`).
        /// </summary>
        private static List<HIS_SERE_SERV_EXT> ExtsOfTreatment(
            List<HIS_SERE_SERV_EXT> exts, long treatmentId)
        {
            var rs = new List<HIS_SERE_SERV_EXT>();
            try
            {
                if (exts == null || treatmentId <= 0) return rs;
                foreach (var ex in exts)
                {
                    if (ex == null) continue;
                    if ((ex.TDL_TREATMENT_ID ?? 0) != treatmentId) continue;
                    rs.Add(ex);
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return rs;
        }

        /// <summary>
        /// Dịch vụ của đúng đợt điều trị. KHÔNG lọc theo loại dịch vụ BHYT như phần lấy kết quả
        /// xét nghiệm: chỉ tiêu có thể nối vào dịch vụ phẫu thuật - thủ thuật, mà loại đó nằm
        /// ngoài nhóm CDHA/TDCN/XN.
        /// </summary>
        private static List<V_HIS_SERE_SERV_2> SereServsOfTreatment(
            List<V_HIS_SERE_SERV_2> sereServs, long treatmentId)
        {
            var rs = new List<V_HIS_SERE_SERV_2>();
            try
            {
                if (sereServs == null || treatmentId <= 0) return rs;
                foreach (var ss in sereServs)
                {
                    if (ss == null) continue;
                    if ((ss.TDL_TREATMENT_ID ?? 0) != treatmentId) continue;
                    rs.Add(ss);
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return rs;
        }

        private static List<V_HIS_SERE_SERV_TEIN> TeinsOfTreatment(
            List<V_HIS_SERE_SERV_2> sereServs, List<V_HIS_SERE_SERV_TEIN> teins, long treatmentId)
        {
            var rs = new List<V_HIS_SERE_SERV_TEIN>();
            try
            {
                if (teins == null || teins.Count == 0 || treatmentId <= 0) return rs;

                var ssOfTrea = new HashSet<long>();
                if (sereServs != null)
                {
                    foreach (var ss in sereServs)
                    {
                        if (ss == null) continue;
                        if ((ss.TDL_TREATMENT_ID ?? 0) == treatmentId) ssOfTrea.Add(ss.ID);
                    }
                }
                if (ssOfTrea.Count == 0) return rs;

                foreach (var t in teins)
                {
                    if (t != null && ssOfTrea.Contains(t.SERE_SERV_ID)) rs.Add(t);
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return rs;
        }

        /// <summary>
        /// Bản ghi sinh hiệu đầu tiên của đợt điều trị — dùng khi hồ sơ KSK không gắn bản ghi nào.
        /// Sinh hiệu có thể được nhập ở màn hình khác của cùng đợt, bỏ qua thì cổng báo thiếu
        /// huyết áp / nhịp thở dù dữ liệu đã có trong cơ sở dữ liệu.
        /// </summary>
        private static HIS_DHST FirstDhstOfTreatment(List<HIS_DHST> list)
        {
            try
            {
                if (list == null || list.Count == 0) return null;
                foreach (var d in list) if (d != null) return d;
                return null;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        private static HIS_KSK_SYNC BuildSyncEntity(V_HIS_KSK_SYNC row, KskSyncResultADO ado)
        {
            var ent = new HIS_KSK_SYNC
            {
                KSK_TYPE_ID = (short)ToLong(GetProp(row, "KSK_TYPE_ID")),
                KSK_RECORD_ID = ToLong(GetProp(row, "KSK_RECORD_ID")),
                SYNC_RESULT_TYPE = ado.SYNC_RESULT_TYPE,
                SYNC_TIME = ado.SYNC_TIME,
                // Cot varchar2(100): ghep nhieu cong (vd "BYT:<txn 55>;VLG:MSG:<msg_id 51>") de vuot ->
                // ca lo luu that bai. FitSyncCode rut gon/cat truoc khi gui backend.
                TRANSACTION_CODE = FitSyncCode(ado.TRANSACTION_CODE),
                SYNC_FAILD_REASON = ado.SYNC_FAILD_REASON,
                REGISTRATION_NO = FitSyncCode(ado.REGISTRATION_NO)
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
        /// TRA CUU ket qua xu ly THAT tren cong KDLYT Vinh Long cho danh sach ho so va CAP NHAT trang thai HIS theo
        /// DUNG lan gui VLG gan nhat cua ho so (API V1.5 — 2 lop: Kho kiem tra + Cong Bo Y te):
        ///   1. Lan dong bo gan nhat CHUA gui duoc len Kho (REGISTRATION_NO doan VLG = VLG_CHUA_GUI) -> CHI hien thi,
        ///      KHONG nang/ha theo ban Kho dang giu (ban cu — ban sua chua toi Kho).
        ///   2. Doan VLG cua TRANSACTION_CODE = "MSG:&lt;msg_id&gt;" -> doi soat theo sender_id + msg_id; = ma theo doi
        ///      -> tra /lan-gui/trang-thai?tracking_id. Ket luan theo CHINH lan gui do:
        ///        Kho khong dat / loi ky thuat / Bo tu choi -> That bai (luu); Bo da nhan -> Da dong bo BYT_ACCEPTED (luu);
        ///        Kho DAT, Bo chua co -> Da dong bo KHO_DAT_CHO_BO (luu khi dang That bai); dang xu ly -> chi hien thi.
        ///      Ho so That bai chi duoc NANG khi lan gui Kho giu KHONG cu hon lan dong bo gan nhat (SYNC_TIME - 15 phut).
        ///   3. Khong xac dinh duoc lan gui (ho so cu / ma theo doi khong tra duoc) -> theo trang thai HO SO (nhu truoc),
        ///      cung quy tac khong nang ho so That bai theo lan gui cu hon.
        /// Ho so That bai vi cong KHAC -> khong nang, chi hien thi. Ghi REGISTRATION_NO / TRANSACTION_CODE chi thay doan
        /// "VLG:" (giu gia tri cong khac). Luu batch 1 lan qua api/HisKskSync/SaveSyncResult; token cache dung chung ca lo.
        /// </summary>
        internal List<KskSyncResultADO> UpdateVlgStatuses(IEnumerable<V_HIS_KSK_SYNC> rows)
        {
            var results = new List<KskSyncResultADO>();
            var saveList = new List<HIS_KSK_SYNC>();
            this.SaveAllOk = true; this.SaveError = null;
            if (rows == null) return results;
            List<V_HIS_KSK_SYNC> rowList = rows.Where(r => r != null).ToList();
            if (rowList.Count == 0) return results;

            try
            {
                KskVlgConfig vlgConfig = KskVlgConfigParser.Parse(this.vlgConnectionInfo);
                if (vlgConfig == null)
                {
                    foreach (var row in rowList)
                        results.Add(BuildFailedResult(row, NowTimeNumber(),
                            "VLG: chưa cấu hình / cấu hình sai định dạng (MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO)"));
                    return results;   // khong co gi de luu
                }

                KskVlgPusher pusher = new KskVlgPusher(vlgConfig);
                Inventec.Common.Logging.LogSystem.Info("Cap nhat KQ cong VLG: tra cuu " + rowList.Count + " ho so.");

                foreach (var row in rowList)
                {
                    KskSyncResultADO ado;
                    bool save = false;
                    try
                    {
                        ado = UpdateOneVlgStatus(pusher, row, out save);
                        if (ToLong(GetProp(row, "SYNC_RESULT_TYPE")) == RESULT_EDITED)
                        {
                            // "Co chinh sua": ket qua cong la cua BAN CU — KHONG ghi de (se mat tin hieu can day lai).
                            save = false;
                            string kq = (ado.SYNC_RESULT_TYPE == RESULT_SUCCESS) ? ado.SuccessNote : ado.SYNC_FAILD_REASON;
                            ado.SYNC_RESULT_TYPE = RESULT_EDITED;
                            ado.SuccessNote = null;
                            ado.SYNC_FAILD_REASON = CapReason("Hồ sơ đã sửa sau lần đồng bộ — cần đồng bộ lại để gửi bản mới"
                                + (string.IsNullOrWhiteSpace(kq) ? "" : (" | Kết quả bản trước: " + kq)));
                        }
                    }
                    catch (Exception exRow)
                    {
                        Inventec.Common.Logging.LogSystem.Error(exRow);
                        ado = BuildFailedResult(row, NowTimeNumber(), "VLG: lỗi tra cứu — " + exRow.Message);
                        save = false;
                    }
                    results.Add(ado);
                    if (save) saveList.Add(BuildSyncEntity(row, ado));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                for (int i = results.Count; i < rowList.Count; i++)
                    results.Add(BuildFailedResult(rowList[i], NowTimeNumber(), "VLG: lỗi tra cứu — " + ex.Message));
            }

            if (saveList.Count > 0 && !SaveResults(saveList))
            {
                this.SaveAllOk = false;
                if (string.IsNullOrEmpty(this.SaveError)) this.SaveError = "Lưu trạng thái đồng bộ thất bại (xem log).";
            }
            return results;
        }

        // Do lech dong ho cho phep giua may tram HIS (SYNC_TIME) va Kho (received_at) khi so "lan gui co moi khong".
        private const int VLG_CLOCK_SKEW_MINUTES = 15;

        /// <summary>Cap nhat 1 ho so theo dung lan gui VLG gan nhat — xem UpdateVlgStatuses. save = co ghi DB khong.</summary>
        private KskSyncResultADO UpdateOneVlgStatus(KskVlgPusher pusher, V_HIS_KSK_SYNC row, out bool save)
        {
            save = false;
            long rowSyncTime = ToLong(GetProp(row, "SYNC_TIME"));
            KskSyncResultADO ado = NewResult(row, rowSyncTime > 0 ? rowSyncTime : NowTimeNumber());
            // GIU ma dang luu — tra cuu khong duoc xoa / ghi de rong; khi ghi chi thay doan VLG.
            string curTxn = EmptyToNull(SafeString(GetProp(row, "TRANSACTION_CODE")));
            string curReg = EmptyToNull(SafeString(GetProp(row, "REGISTRATION_NO")));
            ado.TRANSACTION_CODE = curTxn;
            ado.REGISTRATION_NO = curReg;
            string maLk = SafeString(GetProp(row, "TDL_TREATMENT_CODE"));
            string curReason = SafeString(GetProp(row, "SYNC_FAILD_REASON")) ?? "";
            long curType = ToLong(GetProp(row, "SYNC_RESULT_TYPE"));
            bool rowFailed = curType == RESULT_FAILED;
            // Loi cua cong KHAC dang luu (BuildResultAdo xep ly do cong khac TRUOC doan "VLG:").
            string otherReason = OtherGatewayReason(curReason);
            bool failedByOtherGateway = rowFailed && !string.IsNullOrWhiteSpace(otherReason);
            string vlgTxn = GetVlgSegment(curTxn, true);
            string vlgReg = GetVlgSegment(curReg, false);
            DateTime? rowSyncAt = (rowSyncTime > 0)
                ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(rowSyncTime) : (DateTime?)null;

            // (1) Lan dong bo gan nhat CHUA gui duoc len Kho -> chi hien thi trang thai ban Kho dang giu (moi loai
            // trang thai — ke ca Da dong bo do lan sau chi day cong khac: doan VLG van la "chua gui ban moi").
            if (string.Equals(vlgReg, KskVlgBytResCode.CHUA_GUI, StringComparison.OrdinalIgnoreCase))
            {
                KskVlgStatusResult st0 = pusher.GetStatus(maLk);
                string kho = !st0.Ok ? ("chưa tra được: " + st0.FailReason)
                    : (st0.Found ? ("Kho đang giữ bản gửi trước (" + st0.ValidationStatus + ")") : "Kho chưa có hồ sơ");
                return VlgDisplay(ado, KeepType(curType), rowFailed ? curReason : "",
                    "VLG: lần đồng bộ gần nhất CHƯA gửi được lên Kho — " + kho + "; xử lý lỗi rồi đồng bộ lại");
            }

            // (2) Xac dinh DUNG lan gui gan nhat: "MSG:<msg_id>" -> doi soat; ma theo doi -> tra lan gui.
            KskVlgRequestInfo att = null;
            string msgId = MsgIdOf(vlgTxn);
            if (msgId != null)
            {
                VlgPrevLookup lk = LookupVlgMessage(pusher, maLk, msgId);
                if (!lk.Checked)
                    return VlgDisplay(ado, KeepType(curType), curReason,
                        "VLG: chưa đối soát được lần gửi gần nhất (" + lk.FailReason + ") — thử lại sau");
                if (!lk.Found)
                    return VlgDisplay(ado, KeepType(curType), curReason,
                        "VLG: lần gửi gần nhất CHƯA vào Kho dữ liệu — xử lý lỗi rồi đồng bộ lại");
                att = lk.Info;
                if (att != null && !string.IsNullOrEmpty(att.TrackingId))
                {
                    // Lay them loi chi tiet cua chinh lan gui (doi soat khong tra errors).
                    KskVlgMessageLookup tl = pusher.LookupTracking(att.TrackingId);
                    if (tl.Ok && tl.Found && tl.Info != null)
                    {
                        if (string.IsNullOrEmpty(tl.Info.MsgId)) tl.Info.MsgId = att.MsgId;
                        if (tl.Info.ReceivedAt == DateTime.MinValue) tl.Info.ReceivedAt = att.ReceivedAt;
                        att = tl.Info;
                    }
                }
            }
            else if (IsVlgTracking(vlgTxn))
            {
                KskVlgMessageLookup tl = pusher.LookupTracking(vlgTxn);
                if (!tl.Ok)
                    return VlgDisplay(ado, KeepType(curType), curReason,
                        "VLG: chưa tra được lần gửi " + vlgTxn + " (" + tl.FailReason + ") — thử lại sau");
                if (tl.Found) att = tl.Info;
                else if (rowFailed)
                    // Lan gui gan nhat (ma theo doi dang luu) KHONG con tren Kho -> khong nang theo ban khac cua ho so.
                    return VlgDisplay(ado, RESULT_FAILED, curReason,
                        "VLG: không tìm thấy lần gửi " + vlgTxn + " trên Kho — đồng bộ lại");
                // Da dong bo + 404 -> xet theo ho so ben duoi (ma theo doi cu / da don)
            }

            if (att != null)
            {
                // Ban tin xac dinh DUNG bang MSG:<msg_id> dang luu kem VLG_CHUA_RO = chinh byte HIS gui gan nhat
                // (CHUA_RO khong duoc mang sang khi ho so "Co chinh sua") -> khong ap moc SYNC_TIME.
                bool exactLatest = msgId != null && !string.IsNullOrEmpty(vlgReg)
                    && vlgReg.IndexOf(KskVlgBytResCode.CHUA_RO, StringComparison.OrdinalIgnoreCase) >= 0;
                return ApplyVlgAttempt(ado, att, curType, curReason, otherReason, failedByOtherGateway,
                    curTxn, curReg, vlgTxn, vlgReg, exactLatest ? (DateTime?)null : rowSyncAt, exactLatest, out save);
            }

            // (3) Khong xac dinh duoc lan gui -> theo trang thai HO SO (nhu truoc).
            return ApplyVlgHoSoLevel(pusher, ado, maLk, curType, curReason, otherReason, failedByOtherGateway,
                curTxn, curReg, rowSyncAt, out save);
        }

        /// <summary>Ket luan theo DUNG 1 lan gui (att) — xem UpdateVlgStatuses buoc 2.</summary>
        private KskSyncResultADO ApplyVlgAttempt(KskSyncResultADO ado, KskVlgRequestInfo att, long curType,
            string curReason, string otherReason, bool failedByOtherGateway, string curTxn, string curReg,
            string vlgTxn, string vlgReg, DateTime? rowSyncAt, bool exactLatest, out bool save)
        {
            save = false;
            bool rowFailed = curType == RESULT_FAILED;
            string trk = att.TrackingId;
            string label = "lần gửi " + (!string.IsNullOrEmpty(trk) ? trk : ("msg " + att.MsgId));
            string bo = (att.BytResCode ?? att.BytStatus) ?? "";
            bool legacy = string.Equals(att.SourceChannel, "LEGACY_API", StringComparison.OrdinalIgnoreCase);

            if (att.IsHocInvalid || att.IsHocTechnicalFailed || att.IsBytRejected || att.IsBytFailed)
            {
                string code, text;
                if (att.IsHocInvalid)
                {
                    code = "INVALID";
                    text = "VLG: " + label + " KHÔNG ĐẠT kiểm tra của Kho"
                        + (string.IsNullOrEmpty(att.ErrorSummary) ? "" : (" — " + att.ErrorSummary)) + " — sửa hồ sơ rồi đẩy lại";
                }
                else if (att.IsHocTechnicalFailed)
                {
                    code = att.HocStatus;
                    text = "VLG: Kho lỗi kỹ thuật khi xử lý " + label + " (" + att.HocStatus + ") — đồng bộ lại";
                }
                else
                {
                    code = !string.IsNullOrEmpty(att.BytResCode) ? att.BytResCode : (att.BytStatus ?? "BYT_REJECTED");
                    text = "VLG: Kho ĐẠT nhưng Cổng Bộ Y tế TỪ CHỐI / gửi Bộ thất bại (" + bo + ")"
                        + (string.IsNullOrEmpty(att.BytResMsg) ? "" : (": " + att.BytResMsg)) + " — sửa hồ sơ rồi đẩy lại";
                }
                ado.SYNC_RESULT_TYPE = RESULT_FAILED;
                ado.REGISTRATION_NO = SetVlgSegment(curReg, code, false);
                if (!string.IsNullOrEmpty(trk)) ado.TRANSACTION_CODE = SetVlgSegment(curTxn, trk, true);
                ado.SYNC_FAILD_REASON = CapReason(JoinReason(otherReason, text));
                save = true;
                return ado;
            }

            if (att.IsBytAccepted || att.IsHocProcessedValid)
            {
                bool accepted = att.IsBytAccepted;
                string ok = accepted
                    ? ("VLG: Kho ĐẠT kiểm tra và Cổng Bộ Y tế đã tiếp nhận (" + bo + ")")
                    : (legacy
                        ? "VLG: hồ sơ (gửi qua API cũ) ĐẠT kiểm tra (VALID)"
                        : ("VLG: Kho dữ liệu ĐẠT kiểm tra, đang chờ Cổng Bộ Y tế (" + (att.BytStatus ?? "chưa có kết quả")
                            + (string.IsNullOrEmpty(att.BytResCode) ? "" : (" " + att.BytResCode)) + ") — KHÔNG đẩy lại, bấm cập nhật lại sau"));
                if (failedByOtherGateway)
                    return VlgDisplay(ado, RESULT_FAILED, curReason, ok + " — vẫn giữ Thất bại do lỗi cổng khác chưa xử lý");
                if (rowFailed && !exactLatest && !IsAttemptCurrent(att.ReceivedAt, rowSyncAt))
                    return VlgDisplay(ado, RESULT_FAILED, curReason, "VLG: Kho chỉ có " + label + " gửi lúc "
                        + FormatKhoTime(att.ReceivedAt) + " — cũ hơn lần đồng bộ gần nhất; đồng bộ lại để gửi bản mới");

                string newReg = accepted ? (!string.IsNullOrEmpty(att.BytResCode) ? att.BytResCode : "BYT_ACCEPTED")
                    : (legacy ? "VALID" : "KHO_DAT_CHO_BO");
                ado.SYNC_RESULT_TYPE = RESULT_SUCCESS;
                ado.SuccessNote = ok;
                if (!rowFailed && !accepted && !legacy)
                    return ado;   // dang Da dong bo + van cho Bo -> chi hien thi, khong ghi DB
                ado.REGISTRATION_NO = SetVlgSegment(curReg, newReg, false);
                if (!string.IsNullOrEmpty(trk)) ado.TRANSACTION_CODE = SetVlgSegment(curTxn, trk, true);
                bool changed = rowFailed
                    || !string.Equals(vlgReg, newReg, StringComparison.OrdinalIgnoreCase)
                    || (!string.IsNullOrEmpty(trk) && !string.Equals(vlgTxn, trk, StringComparison.OrdinalIgnoreCase));
                save = changed;
                return ado;
            }

            // Kho dang xu ly / trang thai la -> chi hien thi.
            string wait = att.IsHocInFlight
                ? ("VLG: Kho đang xử lý " + label + " (" + att.HocStatus + ") — bấm cập nhật lại sau ít phút")
                : ("VLG: trạng thái " + label + " chưa xác định (" + (att.HocStatus ?? "") + " / " + (att.BytStatus ?? "") + ")");
            return VlgDisplay(ado, KeepType(curType), curReason, wait);
        }

        /// <summary>Ket luan theo trang thai HO SO (ho so khong xac dinh duoc lan gui) — xem UpdateVlgStatuses buoc 3.</summary>
        private KskSyncResultADO ApplyVlgHoSoLevel(KskVlgPusher pusher, KskSyncResultADO ado, string maLk, long curType,
            string curReason, string otherReason, bool failedByOtherGateway, string curTxn, string curReg,
            DateTime? rowSyncAt, out bool save)
        {
            save = false;
            bool rowFailed = curType == RESULT_FAILED;
            KskVlgStatusResult st = pusher.GetStatus(maLk);
            if (!st.Ok)
                return VlgDisplay(ado, RESULT_FAILED, rowFailed ? curReason : "", st.FailReason);
            if (!st.Found)
                return VlgDisplay(ado, RESULT_FAILED, rowFailed ? curReason : "",
                    "VLG: chưa có hồ sơ trên cổng (mã " + maLk + ") — hồ sơ chưa được đẩy?");
            if (st.IsInvalid || (st.IsValid && st.IsBytRejected))
            {
                string text = st.IsInvalid
                    ? ("VLG: hồ sơ KHÔNG ĐẠT kiểm tra của cổng" + (string.IsNullOrEmpty(st.ErrorSummary) ? "" : (" — " + st.ErrorSummary)))
                    : ("VLG: Kho dữ liệu ĐẠT kiểm tra nhưng Cổng Bộ Y tế TỪ CHỐI (" + (st.LatestBytResCode ?? st.LatestBytStatus) + ")"
                        + (string.IsNullOrEmpty(st.ErrorSummary) ? "" : (" — " + st.ErrorSummary)) + " — sửa hồ sơ rồi đẩy lại");
                ado.SYNC_RESULT_TYPE = RESULT_FAILED;
                ado.REGISTRATION_NO = SetVlgSegment(curReg, st.IsInvalid ? "INVALID"
                    : (!string.IsNullOrEmpty(st.LatestBytResCode) ? st.LatestBytResCode : "BYT_REJECTED"), false);
                ado.SYNC_FAILD_REASON = CapReason(JoinReason(otherReason, text));
                save = true;
                return ado;
            }
            if (st.IsValid)
            {
                bool legacy = string.Equals(st.LatestSourceChannel, "LEGACY_API", StringComparison.OrdinalIgnoreCase);
                bool waiting = !st.IsBytAccepted && !legacy;
                string ok = st.IsBytAccepted
                    ? ("VLG: Kho ĐẠT kiểm tra và Cổng Bộ Y tế đã tiếp nhận (" + (st.LatestBytResCode ?? st.LatestBytStatus) + ")")
                    : (waiting
                        ? ("VLG: Kho dữ liệu ĐẠT kiểm tra, đang chờ Cổng Bộ Y tế (" + (st.LatestBytStatus ?? "chưa có kết quả")
                            + (string.IsNullOrEmpty(st.LatestBytResCode) ? "" : (" " + st.LatestBytResCode)) + ") — KHÔNG đẩy lại, bấm cập nhật lại sau")
                        : "VLG: hồ sơ ĐẠT kiểm tra (VALID)");
                if (failedByOtherGateway)
                    return VlgDisplay(ado, RESULT_FAILED, curReason, ok + " — vẫn giữ Thất bại do lỗi cổng khác chưa xử lý");
                if (rowFailed && !IsAttemptCurrent(st.LatestReceivedAt, rowSyncAt))
                    return VlgDisplay(ado, RESULT_FAILED, curReason, "VLG: Kho chỉ có bản gửi lúc "
                        + FormatKhoTime(st.LatestReceivedAt) + " — cũ hơn lần đồng bộ gần nhất; đồng bộ lại để gửi bản mới");
                ado.SYNC_RESULT_TYPE = RESULT_SUCCESS;
                ado.SuccessNote = ok;
                if (waiting && !rowFailed) return ado;   // dang Da dong bo + cho Bo -> chi hien thi
                ado.REGISTRATION_NO = SetVlgSegment(curReg, st.IsBytAccepted ? "BYT_ACCEPTED" : (waiting ? "KHO_DAT_CHO_BO" : "VALID"), false);
                ado.TRANSACTION_CODE = SetVlgSegment(curTxn, st.LatestTrackingId, true);
                save = true;
                return ado;
            }
            // Cong dang xu ly (QUEUED/PROCESSING...) -> giu nguyen trang thai hien tai, chi hien thi.
            return VlgDisplay(ado, KeepType(curType), curReason,
                "VLG: cổng đang xử lý (" + (st.ValidationStatus ?? "chưa có kết quả") + ") — bấm cập nhật lại sau ít phút");
        }

        /// <summary>
        /// Lan gui Kho giu co phai lan dong bo GAN NHAT cua HIS khong: received_at &gt;= SYNC_TIME - 15 phut.
        /// Khong biet thoi diem -> false (khong nang ho so That bai theo lan gui khong xac dinh duoc).
        /// </summary>
        private static bool IsAttemptCurrent(DateTime receivedAt, DateTime? rowSyncAt)
        {
            if (receivedAt == DateTime.MinValue || !rowSyncAt.HasValue) return false;
            return receivedAt >= rowSyncAt.Value.AddMinutes(-VLG_CLOCK_SKEW_MINUTES);
        }

        private static string FormatKhoTime(DateTime t)
        {
            return (t == DateTime.MinValue) ? "(không rõ)" : t.ToString("dd/MM/yyyy HH:mm");
        }

        private static short KeepType(long curType)
        {
            return (curType == RESULT_SUCCESS) ? RESULT_SUCCESS : RESULT_FAILED;
        }

        /// <summary>Ket qua CHI hien thi (khong ghi DB): That bai -> ly do = ly do dang luu + ghi chu; Thanh cong -> ghi chu.</summary>
        private static KskSyncResultADO VlgDisplay(KskSyncResultADO ado, short type, string curReason, string text)
        {
            ado.SYNC_RESULT_TYPE = type;
            if (type == RESULT_SUCCESS) ado.SuccessNote = text;
            else ado.SYNC_FAILD_REASON = CapReason(string.IsNullOrWhiteSpace(curReason) ? text : (curReason + " | " + text));
            return ado;
        }

        private static string JoinReason(string otherReason, string vlgText)
        {
            return string.IsNullOrWhiteSpace(otherReason) ? vlgText : (otherReason + " | " + vlgText);
        }

        private static string CapReason(string reason)
        {
            if (reason == null) return null;
            return (reason.Length > 3900) ? reason.Substring(0, 3900) + "..." : reason;   // backend cat 4000 ky tu
        }

        /// <summary>
        /// Phan ly do cua cong KHAC (khong phai VLG) trong SYNC_FAILD_REASON: BuildResultAdo xep ly do cong khac
        /// TRUOC, doan VLG sau cung ("... | VLG: ..."). Bat dau bang "VLG:" -> chi VLG loi -> null.
        /// </summary>
        private static string OtherGatewayReason(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason)) return null;
            string t = reason.TrimStart();
            if (t.StartsWith("VLG:", StringComparison.OrdinalIgnoreCase)) return null;
            int i = t.IndexOf(" | VLG:", StringComparison.OrdinalIgnoreCase);
            return (i >= 0) ? t.Substring(0, i) : t;
        }

        /// <summary>Thoi diem hien tai dang so yyyyMMddHHmmss (kieu long cua he thong).</summary>
        private static long NowTimeNumber()
        {
            return Inventec.Common.TypeConvert.Parse.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

        private static string EmptyToNull(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? null : s;
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

            // Bang du lieu mau M3 (So Y te TP.HCM). TACH sang method rieng: HIS_KSK_SYT_HCM la type
            // cua MOS.EFMODEL ban moi — vien dung EFMODEL cu KHONG co type nay, neu ham nay tham chieu
            // truc tiep se loi TypeLoad khi JIT (chan het viec dong bo). Chi goi (JIT) khi da cau hinh SYT.
            System.Collections.IDictionary sytHcmByO18 = null;
            if (!string.IsNullOrWhiteSpace(this.sytHcmConnectionInfo))
            {
                try { sytHcmByO18 = LoadSytHcmByO18(over18s); }
                catch (Exception exSyt) { Inventec.Common.Logging.LogSystem.Warn(exSyt); }
            }

            // Danh muc chi nhanh (cache local) — MA_CSKCB = HEIN_MEDI_ORG_CODE theo BRANCH_ID.
            Dictionary<long, HIS_BRANCH> branchById = null;
            try { branchById = IndexBy(BackendDataWorker.Get<HIS_BRANCH>(), b => b.ID); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            // Ma GTIN/GLN co so — SenderId trong CONNECTION_INFO (BYT); neu rong -> fallback MaCsyt cong HOC
            // (trong BuildConfig), roi SenderId cong HSSK, roi cong HCC (deu la ma don vi 13 so).
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
                if (string.IsNullOrWhiteSpace(sid) && !string.IsNullOrWhiteSpace(this.vlgConnectionInfo))
                {
                    // Uu tien ma 13 so khai o truong 6 khoa VLG (MA_GTIN_CSKCB phai trung THONGTINDONVI/MACSKCB
                    // ban tin gui Kho/Bo); chi roi ve MaDonVi 5 so khi vien chua khai ma 13 so.
                    var v = KskVlgConfigParser.Parse(this.vlgConnectionInfo);
                    if (v != null && KskVlgConfigParser.IsGtin13((v.SenderGtin ?? "").Trim())) sid = v.SenderGtin.Trim();
                    else if (v != null && !string.IsNullOrWhiteSpace(v.MaDonVi)) sid = v.MaDonVi;
                }
                maGtinCskcb = sid ?? "";
                // Vien day VLG: MA_GTIN_CSKCB phai TRUNG header.sender_id + THONGTINDONVI/MACSKCB (ResolveVlgSenderGtin)
                // — SenderId cong BYT cu co the khong phai 13 so (vd 83009) nhung van dung truoc trong chuoi tren.
                if (this.pushVlg && !string.IsNullOrWhiteSpace(this.vlgConnectionInfo))
                {
                    var vcfg = KskVlgConfigParser.Parse(this.vlgConnectionInfo);
                    string g = (vcfg != null) ? ResolveVlgSenderGtin(vcfg, Qd1551ConfigParser.Parse(this.connectionInfo, null)) : null;
                    if (KskVlgConfigParser.IsGtin13(g) && !string.Equals(g, maGtinCskcb, StringComparison.Ordinal))
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Dong bo KSK: MA_GTIN_CSKCB tu cau hinh BYT/HSSK/HCC ('" + maGtinCskcb
                            + "') KHAC ma 13 so cong VLG ('" + g + "') -> dung ma VLG cho ban tin (trung header.sender_id).");
                        maGtinCskcb = g;
                    }
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            // Ma doi tuong KSK (config) — dung cho quy tac MA_LOAI_KCB=100 nhu XML130.
            string keyKsk = "";
            try
            {
                keyKsk = KskBranchConfig.GetValue("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.KSK") ?? "";
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

                // Nguon du lieu cho cong So Y te TP.HCM — dung tu chinh nhung gi vua lay o tren.
                try
                {
                    if (!string.IsNullOrWhiteSpace(this.sytHcmConnectionInfo) && over18 != null)
                    {
                        sytSourceBySr[sr] = new KskSytHcmSource
                        {
                            Patient = (trea != null) ? ValOrNull(patById, trea.PATIENT_ID) : null,
                            Treatment = trea,
                            ServiceReq = ValOrNull(sreqById, sr),
                            Over18 = over18,
                            General = general,
                            // Ho so KSK khong gan ban ghi sinh hieu -> lay ban ghi sinh hieu khac
                            // cua CUNG dot dieu tri, thay vi bo trong ca khoi kham the luc.
                            Dhst = dhstOne ?? FirstDhstOfTreatment(dhstForInput),
                            HisRanks = ranks,
                            Ditys = ListOrNull(dityByO18, over18.ID),
                            DiseaseTypes = diseaseTypes,
                            // Kết quả xét nghiệm của ĐÚNG đợt điều trị này — nguồn của khối cận lâm sàng.
                            ClsTeins = TeinsOfTreatment(clsSereServs, clsTeins,
                                (trea != null) ? trea.ID : 0),
                            // Dịch vụ của đợt điều trị — nguồn cho chỉ tiêu nối vào DỊCH VỤ
                            // (siêu âm, phẫu thuật - thủ thuật), xem ServiceResultValues.
                            ClsSereServs = SereServsOfTreatment(clsSereServs,
                                (trea != null) ? trea.ID : 0),
                            ClsExts = ExtsOfTreatment(clsExts, (trea != null) ? trea.ID : 0),
                            ClsMapJson = this.SytClsMapJson
                        };
                        if (sytHcmByO18 != null) AttachSytHcm(sytSourceBySr[sr], sytHcmByO18, over18.ID);
                    }
                }
                catch (Exception exSytSrc) { Inventec.Common.Logging.LogSystem.Warn(exSytSrc); }
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
        /// <summary>
        /// Nap bang HIS_KSK_SYT_HCM theo KSK >=18 (mau M3 So Y te TP.HCM). TACH RIENG khoi BuildInputs de
        /// type HIS_KSK_SYT_HCM (MOS.EFMODEL ban moi) chi bi JIT nap khi vien co cau hinh cong SYT TP.HCM;
        /// vien dung EFMODEL cu khong co type nay se KHONG loi TypeLoad khi dong bo cac cong khac.
        /// </summary>
        private System.Collections.IDictionary LoadSytHcmByO18(List<HIS_KSK_OVER_EIGHTEEN> over18s)
        {
            var map = new Dictionary<long, HIS_KSK_SYT_HCM>();
            if (over18s == null || over18s.Count == 0) return map;
            List<long> o18Ids = over18s.Where(x => x != null).Select(x => x.ID).Distinct().ToList();
            var sytList = GetList<HIS_KSK_SYT_HCM>("api/HisKskSytHcm/Get",
                new MOS.Filter.HisKskSytHcmFilter { KSK_OVER_EIGHTEEN_IDs = o18Ids });
            if (sytList != null)
                foreach (var x in sytList)
                    if (x != null && !map.ContainsKey(x.KSK_OVER_EIGHTEEN_ID))
                        map[x.KSK_OVER_EIGHTEEN_ID] = x;
            Inventec.Common.Logging.LogSystem.Info("SytHcm: nap bang du lieu mau M3 cho "
                + map.Count + "/" + o18Ids.Count + " ho so KSK tren 18 tuoi");
            return map;
        }

        /// <summary>Gan HIS_KSK_SYT_HCM vao source KSK — tach rieng vi ly do JIT type-load nhu LoadSytHcmByO18.</summary>
        private static void AttachSytHcm(KskSytHcmSource src, System.Collections.IDictionary sytHcmByO18, long over18Id)
        {
            if (src == null || sytHcmByO18 == null) return;
            if (sytHcmByO18.Contains(over18Id)) src.SytHcm = (HIS_KSK_SYT_HCM)sytHcmByO18[over18Id];
        }

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
        ///   - CKS_NGUOI_KET_LUAN: LUON ky bang HSM cua nguoi ket luan (EMR_SIGNER co PCA_SERIAL), KHONG
        ///     phu thuoc cau hinh USB token/HSM cua CKS_BENH_VIEN. Chua chon he thong HSM (HsmType=0), hoac
        ///     nguoi ket luan chua khai chung thu HSM -> the do DE TRONG (khong chan viec ky/xuat) => log ro
        ///     ly do + liet ke loginname thieu chung thu.
        /// </summary>
        private void LogSignScope(List<V_HIS_KSK_SYNC> rowList,
            Dictionary<string, EMR.EFMODEL.DataModels.EMR_SIGNER> concSigners, string prefix)
        {
            try
            {
                if (this.signSetting == null)
                {
                    Inventec.Common.Logging.LogSystem.Info(prefix + ": CKS_SCOPE: signSetting = null (chua cau hinh"
                        + " ky so) -> khong ky the CKS_ nao.");
                    return;
                }
                // Log CAU HINH KY SO dang dung (KHONG log password/secret key) — de biet vi sao re nhanh USB/HSM.
                Inventec.Common.Logging.LogSystem.Info(string.Format(
                    "{0}: CKS_SCOPE cau hinh ky so: IsHsm={1}; HsmType(Id)={2}; Name={3}; SerialNumber={4};"
                    + " so ho so={5}; so nguoi ket luan co chung thu HSM={6}",
                    prefix, this.signSetting.IsHsm, this.signSetting.Id,
                    string.IsNullOrEmpty(this.signSetting.Name) ? "(trong)" : this.signSetting.Name,
                    string.IsNullOrEmpty(this.signSetting.SerialNumber) ? "(trong)" : this.signSetting.SerialNumber,
                    (rowList != null) ? rowList.Count : 0,
                    (concSigners != null) ? concSigners.Count : 0));
                // CKS_NGUOI_KET_LUAN LUON ky bang HSM cua nguoi ket luan, khong phu thuoc cau hinh
                // USB token/HSM (cau hinh do chi quyet dinh cach ky CKS_BENH_VIEN). Dieu kien duy nhat:
                // da chon he thong HSM (HsmType > 0) + nguoi ket luan co EMR_SIGNER.PCA_SERIAL.
                if (this.signSetting.Id <= 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn(prefix + ": chua chon HE THONG HSM (HsmType=0) trong form"
                        + " cau hinh ky so -> chi ky CKS_BENH_VIEN, CKS_NGUOI_KET_LUAN DE TRONG.");
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
                    Inventec.Common.Logging.LogSystem.Info(prefix + ": ky CKS_BENH_VIEN (bang "
                        + (this.signSetting.IsHsm ? "HSM" : "USB token") + ") + CKS_NGUOI_KET_LUAN (bang HSM)"
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
                if (logins.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Info("CKS_NGUOI_KET_LUAN: khong ho so nao co"
                        + " CONCLUDER_LOGINNAME -> khong tra chung thu nguoi ket luan.");
                    return map;
                }
                var signers = GetList<EMR.EFMODEL.DataModels.EMR_SIGNER>("api/EmrSigner/Get",
                    new EMR.Filter.EmrSignerFilter { LOGINNAMEs = logins, IS_ACTIVE = 1 }, ApiConsumers.EmrConsumer);
                Inventec.Common.Logging.LogSystem.Info(string.Format(
                    "CKS_NGUOI_KET_LUAN: tra emr_signer cho {0} nguoi ket luan ({1}) -> api tra ve {2} ban ghi.",
                    logins.Count, string.Join(", ", logins.Take(30).ToArray()),
                    (signers != null) ? signers.Count : -1));
                if (signers != null)
                    foreach (var s in signers)
                    {
                        if (s == null || string.IsNullOrEmpty(s.LOGINNAME)) continue;
                        // Log DU/THIEU tung truong can de ky HSM (chi log CO/KHONG — khong log gia tri mat).
                        Inventec.Common.Logging.LogSystem.Info(string.Format(
                            "CKS_NGUOI_KET_LUAN: emr_signer {0}: PCA_SERIAL={1}; HSM_USER_CODE={2}; PASSWORD={3};"
                            + " SECRET_KEY={4}; CMND_NUMBER={5}",
                            s.LOGINNAME,
                            string.IsNullOrEmpty(s.PCA_SERIAL) ? "THIEU" : "co",
                            string.IsNullOrEmpty(s.HSM_USER_CODE) ? "THIEU" : "co",
                            string.IsNullOrEmpty(s.PASSWORD) ? "THIEU" : "co",
                            string.IsNullOrEmpty(s.SECRET_KEY) ? "THIEU" : "co",
                            string.IsNullOrEmpty(s.CMND_NUMBER) ? "THIEU" : "co"));
                        if (!string.IsNullOrEmpty(s.PCA_SERIAL) && !map.ContainsKey(s.LOGINNAME))
                            map[s.LOGINNAME] = s;
                    }
                foreach (var lg in logins)
                    if (!map.ContainsKey(lg))
                        Inventec.Common.Logging.LogSystem.Warn("CKS_NGUOI_KET_LUAN: nguoi ket luan " + lg
                            + " KHONG co emr_signer (IS_ACTIVE=1) kem PCA_SERIAL -> the CKS_NGUOI_KET_LUAN cua ho so"
                            + " do se DE TRONG.");
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
            var cfg = Qd1551ConfigParser.Parse(this.connectionInfo, null) ?? new Qd1551Config();
            // MACSKCB (envelope) + MA_GTIN_CSKCB: nếu KHÔNG có SenderId cổng BYT (vd chỉ cấu hình HOC),
            // dùng MaCsyt trong cấu hình HOC làm SenderId -> THONGTINDONVI/MACSKCB = MaCsyt (HOC).
            if (string.IsNullOrWhiteSpace(cfg.SenderId) && !string.IsNullOrWhiteSpace(this.hocConnectionInfo))
            {
                var hoc = HocConfigParser.Parse(this.hocConnectionInfo);
                if (hoc != null && !string.IsNullOrWhiteSpace(hoc.MaCsyt)) cfg.SenderId = hoc.MaCsyt;
            }
            return cfg;
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
        /// Cau hinh cong KDLYT Vinh Long tu MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO. Dinh dang RIENG
        /// (cac truong cach '|') — xem KskVlgConfigParser:
        ///   MaDonVi|Username|Password|TokenUrl|PushUrl|SenderGtin
        /// Tra null khi khong day cong VLG / chua cau hinh / chuoi sai dinh dang.
        /// </summary>
        private KskVlgConfig BuildVlgConfig()
        {
            if (!this.pushVlg || string.IsNullOrWhiteSpace(this.vlgConnectionInfo)) return null;
            return KskVlgConfigParser.Parse(this.vlgConnectionInfo);
        }

        /// <summary>
        /// Chong gui trung len Bo: lan dong bo truoc MAT PHAN HOI sau khi da gui (doan VLG cua REGISTRATION_NO chua
        /// VLG_CHUA_RO, doan VLG cua TRANSACTION_CODE = "MSG:&lt;msg_id&gt;") -> doi soat xem ban tin do da vao Kho chua.
        ///   - Ho so "Co chinh sua" (SYNC_RESULT_TYPE = 4) -> null: ban tin mat phan hoi la BAN CU, gui ban hien tai.
        ///   - Kho chua co                                  -> null: gui ban hien tai nhu binh thuong.
        ///   - Kho co, khong dat / loi / Bo tu choi / la    -> null: gui ban hien tai (ban cu hong).
        ///   - Kho co, Bo da nhan / Kho dang giu            -> ket qua thanh cong, KHONG gui lai.
        ///   - Khong doi soat duoc                          -> that bai, GIU dau VLG_CHUA_RO de lan sau doi soat lai.
        /// </summary>
        private KskVlgPushResult CheckPreviousVlgUnknown(KskVlgPusher pusher, V_HIS_KSK_SYNC row)
        {
            try
            {
                string prevReg = GetVlgSegment(SafeString(GetProp(row, "REGISTRATION_NO")), false);
                if (string.IsNullOrEmpty(prevReg)
                    || prevReg.IndexOf(KskVlgBytResCode.CHUA_RO, StringComparison.OrdinalIgnoreCase) < 0) return null;
                string prevMsgId = ExtractVlgMsgMarker(row);
                if (prevMsgId == null) return null;
                string maLk = SafeString(GetProp(row, "TDL_TREATMENT_CODE"));
                if (ToLong(GetProp(row, "SYNC_RESULT_TYPE")) == RESULT_EDITED)
                {
                    Inventec.Common.Logging.LogSystem.Info("VLG: ma dieu tri " + maLk + " — ho so da sua sau lan gui mat phan hoi (msg_id "
                        + prevMsgId + ") -> gui ban hien tai.");
                    return null;
                }

                VlgPrevLookup lk = LookupVlgMessage(pusher, maLk, prevMsgId);
                if (!lk.Checked)
                    return new KskVlgPushResult
                    {
                        Success = false,
                        Status = KskVlgBytResCode.CHUA_RO,       // giu dau de lan sau doi soat lai
                        TrackingId = "MSG:" + prevMsgId,
                        MsgId = prevMsgId,
                        Message = "VLG: lần gửi trước mất phản hồi và CHƯA đối soát được trên cổng (" + lk.FailReason
                            + ") — chưa gửi lại để tránh trùng; đồng bộ lại sau."
                    };
                if (!lk.Found)
                {
                    Inventec.Common.Logging.LogSystem.Info("VLG: ma dieu tri " + maLk + " — lan gui truoc (msg_id "
                        + prevMsgId + ") CHUA vao Kho -> gui lai.");
                    return null;
                }
                KskVlgPushResult known = KskVlgPusher.FromKnownRequest(lk.Info, lk.Info.MsgId ?? prevMsgId);
                if (known == null || !known.Success)
                {
                    Inventec.Common.Logging.LogSystem.Info("VLG: ma dieu tri " + maLk + " — lan gui truoc (msg_id "
                        + prevMsgId + ") da vao Kho nhung khong dat/loi/bi tu choi (" + (known != null ? known.Status : "")
                        + ") -> gui ban hien tai.");
                    return null;
                }
                Inventec.Common.Logging.LogSystem.Info("VLG: ma dieu tri " + maLk + " — lan gui truoc (msg_id "
                    + prevMsgId + ") DA vao Kho (" + known.Status + ") -> khong gui lai.");
                return known;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>Ket qua doi soat 1 lan gui HIS da ghi dau "MSG:&lt;msg_id&gt;".</summary>
        private sealed class VlgPrevLookup
        {
            internal bool Checked;       // co ket luan (ke ca "khong co")
            internal bool Found;         // Kho CO ban tin do
            internal KskVlgRequestInfo Info;
            internal string FailReason;
        }

        /// <summary>msg_id HIS da ghi o doan VLG cua TRANSACTION_CODE ("MSG:&lt;msg_id&gt;"). Khong co -> null.</summary>
        private static string ExtractVlgMsgMarker(V_HIS_KSK_SYNC row)
        {
            return MsgIdOf(GetVlgSegment(SafeString(GetProp(row, "TRANSACTION_CODE")), true));
        }

        // msg_id day du = GTIN 13 + yyMMdd 6 + UUID 32; 32 = chi con UUID (ban ghi cu da rut gon, khong suy duoc sender_id).
        private const int FULL_MSG_ID_LEN = 51;
        private const int TAIL_MSG_ID_LEN = 32;

        /// <summary>
        /// "MSG:&lt;id&gt;" -> id, CHI nhan id dung 51 (day du) hoac 32 (UUID) ky tu chu/so — manh vo do cat chuoi
        /// khong duoc dung (khop hau to nham ban tin khac). Khac -> null.
        /// </summary>
        private static string MsgIdOf(string vlgTxnSegment)
        {
            if (string.IsNullOrEmpty(vlgTxnSegment) || !vlgTxnSegment.StartsWith("MSG:", StringComparison.OrdinalIgnoreCase)) return null;
            string id = vlgTxnSegment.Substring(4).Trim();
            if (id.Length != FULL_MSG_ID_LEN && id.Length != TAIL_MSG_ID_LEN) return null;
            foreach (char c in id) if (!char.IsLetterOrDigit(c)) return null;
            return id;
        }

        /// <summary>
        /// Doi soat 1 ban tin: (1) API doi soat theo sender_id + msg_id (tai lieu V1.5 muc 5.8.1 — ket luan chac chan
        /// ca "co" lan "khong co"); khong goi duoc thi (2) tim trong requests[] cua ho so. requests[] BO QUA lan gui lai
        /// noi dung y het (kiem chung dev 24/09) nen voi msg_id day du, "khong thay" o (2) KHONG phai bang chung ->
        /// Checked = false (giu dau, doi soat lai sau). msg_id rut gon (khong goi duoc doi soat) thi (2) la duy nhat.
        /// </summary>
        private static VlgPrevLookup LookupVlgMessage(KskVlgPusher pusher, string maLk, string msgId)
        {
            var res = new VlgPrevLookup();
            bool full = msgId.Length >= FULL_MSG_ID_LEN && KskVlgConfigParser.IsGtin13(msgId.Substring(0, 13));
            if (full)
            {
                KskVlgMessageLookup ml = pusher.LookupMessage(msgId.Substring(0, 13), msgId);
                if (ml.Ok)
                {
                    res.Checked = true; res.Found = ml.Found; res.Info = ml.Info;
                    return res;
                }
                res.FailReason = ml.FailReason;
            }
            KskVlgStatusResult st = pusher.GetStatus(maLk);
            if (!st.Ok)
            {
                if (string.IsNullOrEmpty(res.FailReason)) res.FailReason = st.FailReason;
                return res;
            }
            KskVlgRequestInfo hit = st.Found ? st.FindRequest(msgId) : null;
            if (hit != null)
            {
                res.Checked = true; res.Found = true; res.Info = hit; res.FailReason = null;
                return res;
            }
            if (full)
            {
                res.Checked = false;
                res.FailReason = (res.FailReason ?? "đối soát không trả lời") + "; không thấy trong danh sách lần gửi của hồ sơ — chưa kết luận";
                return res;
            }
            res.Checked = true; res.Found = false; res.FailReason = null;
            return res;
        }

        private static readonly string[] GatewayLabels = { "BYT:", "HSSK:", "HOC:", "HCC:", "VLG:" };

        private static bool HasGatewayLabel(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            foreach (string p in value.Split(';'))
                foreach (string lb in GatewayLabels)
                    if (p.StartsWith(lb, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        // Ma theo doi cua Kho: KSKBYT-20260924040450544-1C65DCE249B24D96 (V1.5), KSK2062-.../KSK-... (V1.3).
        private static readonly System.Text.RegularExpressions.Regex VlgTrackingRegex =
            new System.Text.RegularExpressions.Regex(@"^KSK[A-Z0-9]*-\d{14,}-[0-9A-Za-z]+$",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        private static bool IsVlgTracking(string value)
        {
            return !string.IsNullOrEmpty(value) && VlgTrackingRegex.IsMatch(value.Trim());
        }

        private static readonly HashSet<string> VlgStatusCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            KskVlgBytResCode.CHUA_RO, KskVlgBytResCode.CHUA_GUI, "KHO_DA_NHAN", "KHO_DAT_CHO_BO", "BYT_ACCEPTED", "BYT_REJECTED",
            "VALID", "INVALID", "QUEUED", "ACCEPTED", "ACCEPTED_DUPLICATE", "ACCEPTED_WITH_WARNING",
            "VALIDATION_FAILED", "TECHNICAL_FAILED", "HOC_KHONG_RO",
            // ma rieng cua Kho khi tu choi o cong (tai lieu V1.5 muc 5.1, 6)
            "EMPTY_BODY", "INVALID_JSON", "INVALID_XML", "INVALID_XML_BASE64", "INVALID_XML_ENCODING", "UNAUTHORIZED",
            "FORBIDDEN", "PAYLOAD_TOO_LARGE", "RATE_LIMITED", "KSK_LEGACY_API_DISABLED", "ORG_MISMATCH", "MACSKCB_MISMATCH",
            "UNSUPPORTED_CONTENT_TYPE", "REQUEST_ID_CONFLICT", "NOT_FOUND"
        };

        /// <summary>Chuoi TRAN (khong nhan cong) co phai gia tri do cong VLG ghi khong.</summary>
        private static bool IsVlgBareValue(string value, bool isTxn)
        {
            if (string.IsNullOrEmpty(value)) return false;
            string v = value.Trim();
            if (isTxn) return v.StartsWith("MSG:", StringComparison.OrdinalIgnoreCase) || IsVlgTracking(v);
            return VlgStatusCodes.Contains(v)
                || v.StartsWith("HOC_", StringComparison.OrdinalIgnoreCase)
                || v.StartsWith("HTTP_", StringComparison.OrdinalIgnoreCase)
                || v.StartsWith("CM_", StringComparison.OrdinalIgnoreCase)
                || v.StartsWith("PS_", StringComparison.OrdinalIgnoreCase)
                || v.StartsWith("BYT_", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Doan gia tri cua cong VLG trong chuoi da luu: chuoi ghep co nhan -> noi dung sau "VLG:"; chuoi tran nhan
        /// dien duoc la gia tri VLG (ma theo doi KSK..., MSG:..., ma trang thai VLG) -> ca chuoi; con lai -> null.
        /// </summary>
        private static string GetVlgSegment(string stored, bool isTxn)
        {
            if (string.IsNullOrEmpty(stored)) return null;
            if (HasGatewayLabel(stored))
            {
                foreach (string p in stored.Split(';'))
                    if (p.StartsWith("VLG:", StringComparison.OrdinalIgnoreCase)) return EmptyToNull(p.Substring(4).Trim());
                return null;
            }
            return IsVlgBareValue(stored, isTxn) ? stored.Trim() : null;
        }

        /// <summary>
        /// Ghi gia tri cong VLG vao chuoi da luu ma KHONG xoa gia tri cong khac: chuoi ghep co nhan -> thay/them doan
        /// "VLG:"; chuoi tran cua VLG -> thay ca chuoi; chuoi tran cua cong KHAC -> giu, noi them ";VLG:...".
        /// </summary>
        private static string SetVlgSegment(string existing, string vlgValue, bool isTxn)
        {
            if (string.IsNullOrEmpty(vlgValue)) return existing;
            if (string.IsNullOrEmpty(existing)) return vlgValue;
            if (!HasGatewayLabel(existing))
                return IsVlgBareValue(existing, isTxn) ? vlgValue : (existing + ";VLG:" + vlgValue);
            var outParts = new List<string>();
            bool replaced = false;
            foreach (string p in existing.Split(';'))
            {
                if (p.StartsWith("VLG:", StringComparison.OrdinalIgnoreCase))
                {
                    if (!replaced) outParts.Add("VLG:" + vlgValue);
                    replaced = true;
                }
                else if (p.Length > 0) outParts.Add(p);
            }
            if (!replaced) outParts.Add("VLG:" + vlgValue);
            return string.Join(";", outParts.ToArray());
        }

        private const int SYNC_CODE_MAX_LEN = 100;   // HIS_KSK_SYNC.TRANSACTION_CODE / REGISTRATION_NO varchar2(100)
        private const int SEGMENT_MIN_KEEP = 8;       // giu toi thieu "BYT:" + vai ky tu khi phai cat doan cong khac

        /// <summary>
        /// Cat gia tri cho vua cot 100 ky tu — vuot thi ca LO luu that bai (ORA-12899 / EF MaxLength). Doan "VLG:"
        /// (dau chong gui trung MSG:&lt;msg_id&gt; / ma theo doi) GIU NGUYEN; cat bot doan cong KHAC (chi de hien thi)
        /// tu doan dai nhat. Van vuot (khong co doan VLG) -> cat duoi + canh bao.
        /// </summary>
        private static string FitSyncCode(string value)
        {
            if (value == null || value.Length <= SYNC_CODE_MAX_LEN) return value;
            var parts = new List<string>(value.Split(';'));
            int vi = parts.FindIndex(p => p.StartsWith("VLG:", StringComparison.OrdinalIgnoreCase));
            int over = value.Length - SYNC_CODE_MAX_LEN;
            if (vi >= 0)
            {
                while (over > 0)
                {
                    int j = -1;
                    for (int k = 0; k < parts.Count; k++)
                        if (k != vi && parts[k].Length > SEGMENT_MIN_KEEP && (j < 0 || parts[k].Length > parts[j].Length)) j = k;
                    if (j < 0) break;
                    int cut = Math.Min(over, parts[j].Length - SEGMENT_MIN_KEEP);
                    parts[j] = parts[j].Substring(0, parts[j].Length - cut);
                    over -= cut;
                }
            }
            string v = string.Join(";", parts.ToArray());
            Inventec.Common.Logging.LogSystem.Warn("HIS_KSK_SYNC: gia tri ma giao dich/trang thai dai " + value.Length
                + " ky tu, rut con " + Math.Min(v.Length, SYNC_CODE_MAX_LEN) + " (giu nguyen doan VLG): " + value);
            return (v.Length <= SYNC_CODE_MAX_LEN) ? v : v.Substring(0, SYNC_CODE_MAX_LEN);
        }

        /// <summary>
        /// Ma 13 so (GTIN/GLN) cua co so gui len cong VLG: truong 6 khoa VLG -> SenderId cong BYT -> HSSK -> HCC.
        /// Chi nhan gia tri DUNG 13 chu so; khong co -> null (pre-gate chan kem huong dan khai cau hinh).
        /// KHONG roi ve MaDonVi 5 so (Kho/Bo can ma dinh danh CSKCB 13 so — QD 2062 Phu luc 02 muc 5.2).
        /// </summary>
        private string ResolveVlgSenderGtin(KskVlgConfig vlgConfig, Qd1551Config bytConfig)
        {
            try
            {
                var candidates = new List<string>();
                if (vlgConfig != null) candidates.Add(vlgConfig.SenderGtin);
                if (bytConfig != null) candidates.Add(bytConfig.SenderId);
                if (!string.IsNullOrWhiteSpace(this.hsskConnectionInfo))
                {
                    var h = Qd1551ConfigParser.Parse(this.hsskConnectionInfo, null);
                    if (h != null) candidates.Add(h.SenderId);
                }
                if (!string.IsNullOrWhiteSpace(this.hccConnectionInfo))
                {
                    var c = KskHccConfigParser.Parse(this.hccConnectionInfo);
                    if (c != null) candidates.Add(c.SenderId);
                }
                foreach (string s in candidates)
                {
                    string v = (s ?? "").Trim();
                    if (KskVlgConfigParser.IsGtin13(v)) return v;
                }
                Inventec.Common.Logging.LogSystem.Warn("VLG: khong tim thay ma GTIN 13 so (truong 6 khoa VLG / SenderId"
                    + " cong BYT/HSSK/HCC) -> cac ho so day VLG se bi chan cho den khi khai cau hinh.");
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return null;
        }

        /// <summary>
        /// Kiem tra DU LIEU BAT BUOC cua cong VLG truoc khi day (kiem chung tren cong 11/08/2026 —
        /// thieu la VALIDATION_FAILED 100%). Tra null neu hop le; nguoc lai tra thong bao gop du cac loi
        /// de vien sua 1 lan (MA_LOAI_KCB toi da 2 ky tu; SO_CCCD + LY_DO_VV bat buoc).
        /// CHI ap dung nhanh VLG — cac cong khac giu nguyen (truc BYT van nhan MA_LOAI_KCB=100).
        /// </summary>
        private static string ValidateVlgInput(Qd1551KskInput input)
        {
            try
            {
                if (input == null) return null;   // khong dung duoc du lieu -> da co loi rieng

                // API nap du lieu loi tam thoi (GetList nuot exception tra null) -> Patient/ServiceReq null
                // du DB co du lieu. KHONG duoc quy thanh "chua nhap CCCD/ly do kham" (message sai su that,
                // luu DB lam nhan vien truy tim loi khong ton tai) — bao dung ban chat de thu lai.
                if (input.Treatment == null || input.Patient == null)
                    return "VLG: KHÔNG đẩy hồ sơ — không tải được dữ liệu hồ sơ/bệnh nhân từ hệ thống"
                         + " (mạng/backend chập chờn) — thử đồng bộ lại sau.";

                var reasons = new List<string>();

                string maLoaiKcb = input.MaLoaiKcb ?? "";
                if (maLoaiKcb.Trim().Length > 2)
                    reasons.Add("mã loại KCB '" + maLoaiKcb.Trim() + "' vượt 2 ký tự (cổng chỉ nhận tối đa 2)"
                        + " — tiếp đón cần chọn loại điều trị 'Khám sức khỏe định kỳ' (mã 15)"
                        + " hoặc 'Khám sàng lọc' (mã 16) thay vì Khám + đối tượng KSK");

                // Đặc tả trường SO_CCCD cho phép người nước ngoài ghi SỐ HỘ CHIẾU, nên chỉ chặn
                // khi bệnh nhân không có cả hai. Việc điền hộ chiếu vào SO_CCCD do thư viện QĐ 2062
                // lo (Qd1551KskMapper) — ở đây chỉ cần đừng chặn nhầm.
                string cccd = input.Patient.CCCD_NUMBER;
                if (string.IsNullOrWhiteSpace(cccd))
                    cccd = input.Patient.PASSPORT_NUMBER;
                if (string.IsNullOrWhiteSpace(cccd))
                    reasons.Add("bệnh nhân chưa có Số CCCD, cũng chưa có Số hộ chiếu"
                        + " (cổng bắt buộc SO_CCCD) — bổ sung ở thông tin hành chính bệnh nhân;"
                        + " người nước ngoài thì nhập Số hộ chiếu");

                if (input.ServiceReq == null)
                {
                    // Ly do kham KSK luu o HIS_SERVICE_REQ — khong tai duoc y lenh thi KHONG ket luan
                    // "chua nhap" (Treatment.HOSPITALIZATION_REASON thuong rong voi ho so KSK).
                    reasons.Add("không tải được dữ liệu y lệnh KSK (nguồn Lý do khám) — thử đồng bộ lại sau");
                }
                else
                {
                    string lyDoKham = input.ServiceReq.HOSPITALIZATION_REASON;
                    if (string.IsNullOrWhiteSpace(lyDoKham))
                        lyDoKham = input.Treatment.HOSPITALIZATION_REASON;
                    if (string.IsNullOrWhiteSpace(lyDoKham))
                        reasons.Add("chưa nhập Lý do khám (cổng bắt buộc LY_DO_VV)"
                            + " — nhập ở ô Lý do khám trên màn phiếu KSK");
                }

                if (reasons.Count == 0) return null;
                return "VLG: KHÔNG đẩy hồ sơ — " + string.Join("; ", reasons.ToArray()) + ".";
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        /// <summary>
        /// Sua declaration XML ve encoding="utf-8" (thu vien serialize StringWriter -> khai utf-16).
        /// Khong co declaration / khong khai encoding -> giu nguyen.
        /// </summary>
        private static string FixXmlDeclarationUtf8(string xml)
        {
            try
            {
                if (string.IsNullOrEmpty(xml) || !xml.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase))
                    return xml;
                int end = xml.IndexOf("?>", StringComparison.Ordinal);
                if (end < 0) return xml;
                string decl = xml.Substring(0, end + 2);
                string fixedDecl = System.Text.RegularExpressions.Regex.Replace(
                    decl, "encoding\\s*=\\s*([\"'])[^\"']*\\1", "encoding=\"utf-8\"",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                return (fixedDecl == decl) ? xml : fixedDecl + xml.Substring(end + 2);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return xml; }
        }

        /// <summary>
        /// Dung payload cho cong KDLYT Vinh Long: XML KHAMSUCKHOE cua DUNG 1 ho so (SOLUONGHOSO = 1) —
        /// KskVlgPusher base64 chuoi nay vao truong data cua envelope truc Bo (tai lieu V1.5 muc 5.1).
        /// macskcb = ma 13 so (THONGTINDONVI/MACSKCB) nhu luong truc BYT. Ky CKS_ (neu bat ky so) nhu cong
        /// BYT — ban tin la XML nen luon ky duoc, khong co han che nhu HCC json/base64.
        /// signFailed = true khi user DA TICH ky so nhung ky that bai (dataSigner tra rong — HSM loi,
        /// USB token khong ky duoc...) -> caller PHAI danh dau ho so that bai, KHONG duoc day ban tin
        /// CHUA KY len cong roi bao "thanh cong" (nhat quan voi ExportXmlFiles coi ky-null la failed).
        /// Tra "" khi khong dung duoc (KskVlgPusher se bao that bai cho ho so do).
        /// </summary>
        private static string BuildVlgPayload(string macskcb, Qd1551KskInput input,
            Func<string, string> dataSigner, out bool signFailed)
        {
            signFailed = false;
            try
            {
                if (input == null) return "";
                // Dung envelope co DU 12 khoi (khoi thieu du lieu -> khoi trong) — xem KskEnvelopeBuilder.
                string content = KskEnvelopeBuilder.Build(new List<Qd1551KskInput> { input }, macskcb, false);
                if (string.IsNullOrEmpty(content)) return "";
                // Thu vien serialize bang StringWriter -> declaration ghi encoding="utf-16", nhung VLG gui
                // bytes UTF-8 (Content-Type charset=utf-8) — parser chuan phia cong chieu theo declaration
                // se loi INVALID_XML. Chuan hoa ve utf-8 TRUOC khi ky (sau khi ky khong duoc sua noi dung).
                content = FixXmlDeclarationUtf8(content);
                // API V1.5 (data-sync/push) nhan NGUYEN ban tin chuan truc Bo: ngay 12 so yyyyMMddHHmm nhu
                // thu vien sinh (kiem chung cong dev 24/09/2026: Kho doc dung ngay sinh/ngay kham, phan loai
                // DU_18_TUOI_TRO_LEN, VALID). Khong con cat ngay ve 8 so nhu API V1.3 cu.
                if (dataSigner != null)
                {
                    string signed = dataSigner(content);
                    if (string.IsNullOrEmpty(signed)) { signFailed = true; return ""; }
                    content = signed;
                }
                // Dump NGUYEN VAN ban tin gui di (DEBUG) — VLG gui XML tho nen khong can giai base64.
                DumpDebug("BanTin_VLG_xml", content);
                return content;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return "";
            }
        }

        /// <summary>
        /// Dung payload base64 cho cong HCC: envelope khamsuckhoe cua DUNG 1 ho so (SOLUONGHOSO = 1) theo
        /// data_type cua cau hinh HCC.
        /// - JSON (mac dinh): thu vien xuat ban "JSON hoa" cua XML (ten khoa IN HOA, khong co lop boc)
        ///   -> chuyen sang dung cau truc tai lieu HCC muc 3.3 bang KskHccJsonConverter.
        /// - XML: ky CKS (neu bat ky so) nhu cong BYT.
        /// Tra "" khi khong dung duoc (KskHccPusher se bao that bai cho ho so do).
        /// </summary>
        /// <summary>
        /// Quy đổi GIOI_TINH trong bản tin XML cho HCC: HCC dùng 0=Nữ, 1=Nam (KHÁC QĐ2062 1=Nam, 2=Nữ).
        /// Chỉ đổi Nữ (2 -&gt; 0); Nam (1) giữ. Dùng cho nhánh xml/base64; nhánh json xử lý ở KskHccJsonConverter.
        /// </summary>
        private static string ConvertGioiTinhXmlForHcc(string xml)
        {
            if (string.IsNullOrEmpty(xml)) return xml;
            try
            {
                string result = System.Text.RegularExpressions.Regex.Replace(
                    xml, @"<GIOI_TINH>\s*2\s*</GIOI_TINH>", "<GIOI_TINH>0</GIOI_TINH>",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (!string.Equals(result, xml, StringComparison.Ordinal))
                    Inventec.Common.Logging.LogSystem.Info("KskSyncProcessor: HCC XML gioi_tinh 2 (Nữ QĐ2062) -> 0 (HCC).");
                return result;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return xml; }
        }

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
                    // ToHccJson tự quy đổi gioi_tinh (2 Nữ -> 0) theo domain HCC.
                    content = KskHccJsonConverter.ToHccJson(content);
                    if (string.IsNullOrEmpty(content)) return "";
                }
                else
                {
                    // XML: quy đổi gioi_tinh cho HCC (2 Nữ -> 0) TRƯỚC khi ký (đổi sau ký sẽ hỏng chữ ký).
                    content = ConvertGioiTinhXmlForHcc(content);
                    if (dataSigner != null)
                    {
                        string signed = dataSigner(content);
                        if (!string.IsNullOrEmpty(signed)) content = signed;
                    }
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
        private const string CFG_KEY_VLG = "MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO";

        /// <summary>
        /// Ghi log gia tri cau hinh CUA TUNG CONG vua lay duoc (1 dong/cong) de doi soat khi bam Dong bo:
        /// cong nao chua cau hinh, cong nao co cau hinh nhung khong chon day, cong nao parse loi, cong nao
        /// lay duoc gi (URL / tai khoan / ma don vi / data_type...). MAT KHAU va KHOA BI MAT chi log
        /// co/khong + do dai — KHONG bao gio ghi gia tri thuc ra file log.
        /// </summary>
        private void LogGatewayConfigs(Qd1551Config bytConfig, Qd1551Config hsskConfig,
            HocConfig hocConfig, Qd1551Config hccConfig, KskVlgConfig vlgConfig)
        {
            try
            {
                LogQd1551Config("BYT", CFG_KEY_BYT, this.connectionInfo, this.pushByt, bytConfig);
                LogQd1551Config("HSSK", CFG_KEY_HSSK, this.hsskConnectionInfo, this.pushHssk, hsskConfig);
                LogHocConfig(this.hocConnectionInfo, this.pushHoc, hocConfig);
                LogQd1551Config("HCC", CFG_KEY_HCC, this.hccConnectionInfo, this.pushHcc, hccConfig);
                LogVlgConfig(this.vlgConnectionInfo, this.pushVlg, vlgConfig);
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

        /// <summary>Log 1 dong cau hinh cong KDLYT Vinh Long (KskVlgConfig — cau truc rieng).</summary>
        private static void LogVlgConfig(string rawValue, bool selected, KskVlgConfig cfg)
        {
            if (!LogConfigState("VLG", CFG_KEY_VLG, rawValue, selected, cfg == null)) return;
            Inventec.Common.Logging.LogSystem.Info(string.Format(
                "Cau hinh VLG ({0}): MaDonVi={1}; Username={2}; Password={3}; TokenUrl={4}; PushUrl={5}; SenderGtin={6}",
                CFG_KEY_VLG, Show(cfg.MaDonVi), Show(cfg.Username), Mask(cfg.Password),
                Show(cfg.TokenUrl), Show(cfg.PushUrl), Show(cfg.SenderGtin)));
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
        /// Gop ket qua cua cac cong THU VIEN day (BYT/HSSK/HOC — pushResult), cong HCC (hccResult) va cong
        /// KDLYT Vinh Long (vlgResult) thanh 1 dong trang thai cua ho so. Thanh cong = TAT CA cong da day
        /// deu thanh cong (giong PushListMulti). LUU Y VLG: thanh cong = cong DA TIEP NHAN (status QUEUED,
        /// xu ly bat dong bo) — ket qua xu ly thuc te tra cuu sau bang tracking_id.
        /// Ma giao dich / trang thai ghep dang "BYT:xxx;HCC:yyy;VLG:zzz" (chi them tien to khi >1 cong).
        /// libSingleLabel = ten cong duy nhat do thu vien day (null neu thu vien day >1 cong -> da co tien to).
        /// configError != null: co cong da chon nhung chuoi cau hinh sai dinh dang -> ho so LUON that bai.
        /// </summary>
        private KskSyncResultADO BuildResultAdo(V_HIS_KSK_SYNC row, ResultADO pushResult,
            KskHccPushResult hccResult, KskVlgPushResult vlgResult, long syncTime,
            string libSingleLabel, string configError, KskSytHcmPushResult sytResult)
        {
            KskSyncResultADO ado = NewResult(row, syncTime);
            PushResponse resp = ExtractResponse(pushResult);
            bool hasLib = pushResult != null;
            bool hasHcc = hccResult != null;
            bool hasVlg = vlgResult != null;
            // Cổng Sở Y tế TP.HCM cũng tính vào kết quả chung, nếu không thì lượt CHỈ tích riêng cổng
            // này sẽ bị ghi nhầm là "chưa chọn cổng nào để đẩy".
            bool hasSyt = sytResult != null;

            bool libOk = hasLib && pushResult.Success;
            bool hccOk = hasHcc && hccResult.Success;
            bool vlgOk = hasVlg && vlgResult.Success;
            bool sytOk = hasSyt && sytResult.Success;

            bool success = (hasLib || hasHcc || hasVlg || hasSyt)
                        && (!hasLib || libOk) && (!hasHcc || hccOk)
                        && (!hasVlg || vlgOk) && (!hasSyt || sytOk)
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
            // VLG: X-HOC-Tracking-Id (hoac "MSG:"+msg_id) -> ma giao dich; header.res_code -> trang thai.
            string storedTxn = EmptyToNull(SafeString(GetProp(row, "TRANSACTION_CODE")));
            string storedReg = EmptyToNull(SafeString(GetProp(row, "REGISTRATION_NO")));
            string vlgTxn = hasVlg ? vlgResult.TrackingId : null;
            string vlgReg = hasVlg ? vlgResult.Status : null;
            bool vlgKeyed = !string.IsNullOrWhiteSpace(this.vlgConnectionInfo);   // vien CO khoa VLG
            if (hasVlg)
            {
                // Lan nay VLG KHONG gui duoc byte nao (chan truoc khi gui / thieu GTIN / latch / dang nhap loi):
                // giu DANH TINH lan gui VLG truoc (ma theo doi / MSG) + danh dau VLG_CHUA_GUI -> "Cap nhat KQ cong"
                // khong nang ho so theo ban cu. Lan truoc dang CHUA RO -> giu CHUA_RO de lan sau van doi soat.
                if (string.IsNullOrEmpty(vlgTxn)) vlgTxn = GetVlgSegment(storedTxn, true);
                if (string.IsNullOrEmpty(vlgReg)) vlgReg = VlgNotSentStatus(storedReg, ToLong(GetProp(row, "SYNC_RESULT_TYPE")));
            }
            else if (vlgKeyed)
            {
                // Lan nay KHONG day VLG ma ho so co doan VLG cua lan truoc -> giu nguyen doan do.
                vlgTxn = GetVlgSegment(storedTxn, true);
                vlgReg = GetVlgSegment(storedReg, false);
            }
            bool otherSource = hasLib || hasHcc;
            if (!hasVlg && !otherSource)
            {
                // Khong cong nao sinh ma giao dich/trang thai (chi SYT / chua chon cong) -> giu nguyen gia tri da luu.
                ado.TRANSACTION_CODE = storedTxn;
                ado.REGISTRATION_NO = storedReg;
            }
            else if (hasVlg && !otherSource)
            {
                // Chi VLG tham gia: giu nguyen doan cua cong khac da luu (neu co), chi thay doan VLG.
                ado.TRANSACTION_CODE = SetVlgSegment(storedTxn, vlgTxn, true);
                ado.REGISTRATION_NO = SetVlgSegment(storedReg, vlgReg, false);
            }
            else
            {
                // Nhieu cong: gan nhan theo cong THAM GIA (khong theo gia tri co/khong) de doan VLG luon co nhan.
                bool forceLabel = otherSource && (hasVlg || !string.IsNullOrEmpty(vlgTxn) || !string.IsNullOrEmpty(vlgReg));
                ado.TRANSACTION_CODE = JoinGatewayValue(txn, hasHcc ? hccResult.TxnCode : null, vlgTxn, libSingleLabel, forceLabel);
                ado.REGISTRATION_NO = JoinGatewayValue(regState, hasHcc ? hccResult.State : null, vlgReg, libSingleLabel, forceLabel);
            }
            // Lan day nay KHONG co ma giao dich/trang thai (bi chan truoc khi gui, mat mang...) -> GIU
            // gia tri da luu cua ho so (backend upsert ghi de nguyen cot — null se XOA tracking_id cua
            // lan day thanh cong truoc, mat ma doi soat voi tinh; nhat quan voi UpdateVlgStatuses).
            if (string.IsNullOrEmpty(ado.TRANSACTION_CODE))
                ado.TRANSACTION_CODE = EmptyToNull(SafeString(GetProp(row, "TRANSACTION_CODE")));
            if (string.IsNullOrEmpty(ado.REGISTRATION_NO))
                ado.REGISTRATION_NO = EmptyToNull(SafeString(GetProp(row, "REGISTRATION_NO")));
            // Ghi chu tren dialog ket qua khi VLG thanh cong: Kho da giu ban tin nhung co the CHUA chuyen Bo
            // (tam dung / qua gio / Bo loi luu) — noi ro de nhan vien KHONG day lai va biet bam "Cap nhat KQ
            // cong". Vien khong day VLG -> SuccessNote null, hien thi nhu cu.
            if (success && hasVlg && vlgOk)
            {
                var notes = new List<string>();
                if (!string.IsNullOrEmpty(vlgResult.Note)) notes.Add(vlgResult.Note);
                if (!string.IsNullOrEmpty(vlgResult.Warning))
                    notes.Add("VLG lưu ý: " + vlgResult.Warning);
                if (notes.Count > 0) ado.SuccessNote = string.Join("; ", notes.ToArray());
            }
            if (!success)
            {
                var reasons = new List<string>();
                if (hasLib && !libOk)
                    reasons.Add(!string.IsNullOrEmpty(pushResult.Message) ? pushResult.Message : "Đồng bộ thất bại");
                if (hasHcc && !hccOk)
                    reasons.Add(!string.IsNullOrEmpty(hccResult.Message) ? hccResult.Message : "HCC: đồng bộ thất bại");
                if (hasSyt && !sytOk)
                    reasons.Add("SYT TP.HCM: " + (!string.IsNullOrEmpty(sytResult.Message)
                        ? sytResult.Message : "đồng bộ thất bại"));
                if (!string.IsNullOrEmpty(configError)) reasons.Add(configError);
                // Ly do VLG xep CUOI: "Cap nhat KQ cong" doc phan truoc " | VLG:" la loi cong khac (OtherGatewayReason).
                if (hasVlg && !vlgOk)
                {
                    string vm = !string.IsNullOrEmpty(vlgResult.Message) ? vlgResult.Message : "VLG: đồng bộ thất bại";
                    if (!vm.TrimStart().StartsWith("VLG", StringComparison.OrdinalIgnoreCase)) vm = "VLG: " + vm;
                    reasons.Add(vm);
                }
                // Chi khi KHONG co cong nao duoc day (truoc day thieu !hasVlg + cau "if" treo: ho so chi day VLG
                // ma that bai luon bi noi them "Chua chon cong", ly do loi SYT bi nuot, configError ghi 2 lan).
                if (!hasLib && !hasHcc && !hasVlg && !hasSyt && string.IsNullOrEmpty(configError))
                    reasons.Add("Chưa chọn cổng liên thông để đẩy");
                ado.SYNC_FAILD_REASON = string.Join(" | ", reasons);
            }
            return ado;
        }

        /// <summary>
        /// Ghep gia tri cua cac cong: giu nguyen chuoi cua thu vien (da co tien to khi >1 cong), them
        /// "HCC:" / "VLG:" khi co tu 2 nguon tro len. Chi 1 nguon -> tra gia tri tran (khong tien to)
        /// nhu truoc day.
        /// </summary>
        private static string JoinGatewayValue(string libValue, string hccValue, string vlgValue, string libSingleLabel)
        {
            return JoinGatewayValue(libValue, hccValue, vlgValue, libSingleLabel, false);
        }

        /// <summary>forceLabel = true: luon gan nhan ("BYT:", "HCC:", "VLG:") ke ca khi chi 1 nguon co gia tri.</summary>
        private static string JoinGatewayValue(string libValue, string hccValue, string vlgValue, string libSingleLabel, bool forceLabel)
        {
            var parts = new List<string>();       // gia tri da ghep tien to (khi >1 nguon)
            var rawValues = new List<string>();   // gia tri tran (khi chi 1 nguon)
            if (!string.IsNullOrEmpty(libValue))
            {
                parts.Add(string.IsNullOrEmpty(libSingleLabel) ? libValue : libSingleLabel + ":" + libValue);
                rawValues.Add(libValue);
            }
            if (!string.IsNullOrEmpty(hccValue)) { parts.Add("HCC:" + hccValue); rawValues.Add(hccValue); }
            if (!string.IsNullOrEmpty(vlgValue)) { parts.Add("VLG:" + vlgValue); rawValues.Add(vlgValue); }
            if (parts.Count == 0) return null;
            if (parts.Count == 1) return forceLabel ? parts[0] : rawValues[0];
            return string.Join(";", parts.ToArray());
        }

        /// <summary>
        /// Trang thai VLG khi lan dong bo nay KHONG gui duoc: giu VLG_CHUA_RO neu dang chua ro (ban tin mat phan hoi van
        /// la ban HIS gui gan nhat) — TRU khi ho so "Co chinh sua" (4): ban mat phan hoi la ban CU -> VLG_CHUA_GUI de lan
        /// sau gui ban moi va "Cap nhat KQ cong" khong nang theo ban cu.
        /// </summary>
        private static string VlgNotSentStatus(string storedReg, long rowType)
        {
            if (rowType == RESULT_EDITED) return KskVlgBytResCode.CHUA_GUI;
            string prev = GetVlgSegment(storedReg, false);
            return (!string.IsNullOrEmpty(prev) && prev.IndexOf(KskVlgBytResCode.CHUA_RO, StringComparison.OrdinalIgnoreCase) >= 0)
                ? KskVlgBytResCode.CHUA_RO : KskVlgBytResCode.CHUA_GUI;
        }

        /// <summary>Loi truoc khi toi buoc gui VLG (khong dung duoc du lieu / exception) — danh dau lan nay CHUA gui len Kho.</summary>
        private void MarkVlgNotSent(KskSyncResultADO ado, V_HIS_KSK_SYNC row)
        {
            try
            {
                if (ado == null || !this.pushVlg || string.IsNullOrWhiteSpace(this.vlgConnectionInfo)) return;
                ado.REGISTRATION_NO = SetVlgSegment(ado.REGISTRATION_NO,
                    VlgNotSentStatus(ado.REGISTRATION_NO, ToLong(GetProp(row, "SYNC_RESULT_TYPE"))), false);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private KskSyncResultADO BuildFailedResult(V_HIS_KSK_SYNC row, long syncTime, string reason)
        {
            KskSyncResultADO ado = NewResult(row, syncTime);
            ado.SYNC_RESULT_TYPE = RESULT_FAILED;
            ado.SYNC_FAILD_REASON = reason;
            // Giu ma da luu (backend upsert ghi de nguyen cot — null se xoa tracking cu / dau VLG_CHUA_RO,
            // lam mat chong gui trung). Nhat quan voi BuildResultAdo.
            ado.TRANSACTION_CODE = EmptyToNull(SafeString(GetProp(row, "TRANSACTION_CODE")));
            ado.REGISTRATION_NO = EmptyToNull(SafeString(GetProp(row, "REGISTRATION_NO")));
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

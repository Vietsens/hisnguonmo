/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * Tải danh mục của Nền tảng KSK Sở Y tế TP.HCM về MÁY TRẠM (mẫu phiếu M4).
 *
 * Trình tự:
 *   1. Đọc bản ghi cấu hình kết nối. CHƯA khai báo -> thoát ngay, không làm gì cả
 *      (viện không dùng cổng Sở Y tế không chịu bất kỳ ảnh hưởng nào).
 *   2. Bản lưu tại máy còn hạn -> dùng luôn, KHÔNG gọi cổng.
 *   3. Gọi dịch vụ lấy phiếu truy cập, lấy phiếu rồi gọi lần lượt từng danh mục.
 *   4. Ghi ra tệp tại máy trạm.
 *
 * CHẠY TRÊN LUỒNG RIÊNG: gọi tới 26 dịch vụ qua mạng, để trên luồng giao diện sẽ treo
 * màn hình. Dùng đúng khuôn chạy nền có sẵn của màn hình này (LongRunning — xem Prefetch).
 *
 * BẢO MẬT: KHÔNG ghi nhật ký mật khẩu và phiếu truy cập; chỉ ghi số lượng mục tải được.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        #region ===== Khai báo =====

        private const string CFG_KEY__SYT_HCM_CONNECTION_INFO = "MOS.HIS_KSK_SYNC.SYT_HCM_CONNECTION_INFO";
        private const string CFG_KEY__SYT_HCM_CATEGORY_CODES = "MOS.HIS_KSK_SYNC.SYT_HCM_CATEGORY_CODES";

        /// <summary>Số giờ coi bản danh mục lưu tại máy là còn dùng được, quá thì tải lại.</summary>
        private const int SYT_CATALOG_TTL_HOURS = 24;

        /// <summary>Giới hạn chờ mỗi lần gọi dịch vụ (mili giây) — tránh treo luồng nền vô hạn.</summary>
        private const int SYT_HTTP_TIMEOUT_MS = 20000;

        /// <summary>
        /// 26 mã danh mục theo bộ yêu cầu mẫu của Sở Y tế. Dùng khi bản ghi cấu hình
        /// danh sách mã để trống — Sở thêm danh mục thì khai vào cấu hình, không phải sửa chương trình.
        /// </summary>
        private static readonly string[] SYT_CATALOG_CODES_DEFAULT = new string[]
        {
            "ICD", "DanToc", "NgheNghiepId", "GioiTinh", "NhomMau", "YeuToNhomMau", "Diadiemkham",
            "DiaChiHienTai_Tinh", "DiaChiHienTai_XaPhuong", "NoiCongTacHocTap", "CoSoKham_ChuaBenh",
            // Ba ma duoi day CHI CO o dac ta M3, ten khac M4 (M4: ChiTraChiTiet_NCT / YesNo /
            // TS_BanThan_MacBenh_DanhSachBenh). Giu ca hai ten de dung chung mot ban tai ve.
            "ChiTraChiTiet", "Yes_No", "TS_BanThan_MacBenh_NgheNghiep_DanhSachBenh",
            "ChiTra", "NCT_HTChiTraKhamSucKhoe", "ChiTraChiTiet_NCT", "M3_DoiTuongKham", "KetLuan_DeNghi",
            "KSKDK_PhanLoai_SK", "KSKDK_DM_ChuyenKhoa", "KSKDK_TamSoatOption", "KSKDK_TinhTrangSuyYeu",
            "KSKDK_DanhMucCauHoiKhac", "NenTangKSK_TinhTrangRang", "AmTinh_DuongTinh", "YesNo", "CoChua",
            "TS_BanThan_MacBenh_DanhSachBenh", "TS_GiaDinh_MacBenh_DanhSachBenh"
        };

        /// <summary>Một mục trong danh mục của cổng.</summary>
        public class SytCatalogItem
        {
            public string Id { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
        }

        /// <summary>Danh mục đã tải, khóa = mã danh mục. Chỉ đọc sau khi tải xong.</summary>
        private static readonly Dictionary<string, List<SytCatalogItem>> sytCatalogs
            = new Dictionary<string, List<SytCatalogItem>>();

        private static readonly object sytCatalogLock = new object();
        private static bool sytCatalogLoading = false;

        #endregion

        #region ===== Cấu hình kết nối =====

        /// <summary>
        /// Thông tin kết nối cổng Sở Y tế, đọc từ bản ghi cấu hình dạng các trường cách nhau bằng "|".
        /// Thứ tự: mã cơ sở | tài khoản | mật khẩu | địa chỉ dịch vụ xác thực | địa chỉ dịch vụ nghiệp vụ
        ///         | mã đơn vị gọi | khóa ký toàn vẹn
        /// </summary>
        private class SytConnectionInfo
        {
            public string BranchCode { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string AuthBaseUrl { get; set; }
            public string ApiBaseUrl { get; set; }
            public string ClientId { get; set; }

            public bool IsValid
            {
                get
                {
                    return !string.IsNullOrWhiteSpace(Username)
                        && !string.IsNullOrWhiteSpace(Password)
                        && !string.IsNullOrWhiteSpace(AuthBaseUrl)
                        && !string.IsNullOrWhiteSpace(ApiBaseUrl);
                }
            }
        }

        /// <summary>
        /// CỔNG GÁC AN TOÀN ĐA VIỆN — viện đã khai báo cấu hình cổng Sở Y tế TP.HCM hay chưa.
        ///
        /// Mọi phần riêng của cổng này (tab Khám lâm sàng HCM, các ô nhập bổ sung, đổ danh mục
        /// của cổng) đều phải hỏi qua đây trước. Viện chưa khai báo thì KHÔNG dựng gì, KHÔNG gọi
        /// mạng, KHÔNG thêm truy vấn nào — màn hình giữ nguyên như trước khi có tính năng này.
        ///
        /// Chỉ xét bản ghi cấu hình CÓ GIÁ TRỊ, không xét đủ trường hay chưa: khai báo sai một
        /// trường thì vẫn phải thấy giao diện để còn sửa, chứ không phải mất luôn tab.
        /// </summary>
        private static bool IsSytHcmDeclared()
        {
            try
            {
                return !string.IsNullOrWhiteSpace(GetSytConfigValue(CFG_KEY__SYT_HCM_CONNECTION_INFO));
            }
            catch (Exception ex) { LogSystem.Warn(ex); return false; }
        }

        /// <summary>Đọc và tách bản ghi cấu hình. Chưa khai báo hoặc sai định dạng -> trả null.</summary>
        private static SytConnectionInfo GetSytConnectionInfo()
        {
            try
            {
                string raw = GetSytConfigValue(CFG_KEY__SYT_HCM_CONNECTION_INFO);
                if (string.IsNullOrWhiteSpace(raw)) return null;

                string[] p = raw.Split('|');
                SytConnectionInfo info = new SytConnectionInfo();
                info.BranchCode = Field(p, 0);
                info.Username = Field(p, 1);
                info.Password = Field(p, 2);
                info.AuthBaseUrl = TrimUrl(Field(p, 3));
                info.ApiBaseUrl = TrimUrl(Field(p, 4));
                info.ClientId = Field(p, 5);

                // Chỉ khai một địa chỉ thì dùng chung cho cả hai dịch vụ.
                if (string.IsNullOrWhiteSpace(info.ApiBaseUrl)) info.ApiBaseUrl = info.AuthBaseUrl;

                return info.IsValid ? info : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }

        private static string Field(string[] parts, int i)
        {
            return (parts != null && i < parts.Length) ? parts[i].Trim() : "";
        }

        private static string TrimUrl(string url)
        {
            return string.IsNullOrEmpty(url) ? url : url.TrimEnd('/');
        }

        /// <summary>Bản chụp danh sách cấu hình vừa đọc thẳng từ nguồn, kèm thời điểm chụp.</summary>
        private static List<HIS_CONFIG> sytConfigSnapshot;
        private static DateTime sytConfigSnapshotTime = DateTime.MinValue;

        /// <summary>Số giây dùng lại bản chụp — đủ để một lượt mở màn hình không gọi lặp nhiều lần.</summary>
        private const int SYT_CONFIG_SNAPSHOT_SECONDS = 10;

        /// <summary>
        /// Đọc giá trị một bản ghi cấu hình (null nếu chưa khai báo).
        ///
        /// ĐỌC THẲNG TỪ NGUỒN, KHÔNG qua bộ nhớ đệm RAM: bộ nhớ đệm chỉ nạp một lần lúc khởi động
        /// nên sửa cấu hình xong phải khởi động lại chương trình mới thấy — rất khó dùng khi đang
        /// dò cấu hình. Vẫn giữ một bản chụp ngắn hạn để một lượt mở màn hình không gọi lặp.
        /// </summary>
        private static string GetSytConfigValue(string key)
        {
            try
            {
                List<HIS_CONFIG> list = sytConfigSnapshot;
                if (list == null
                    || (DateTime.Now - sytConfigSnapshotTime).TotalSeconds > SYT_CONFIG_SNAPSHOT_SECONDS)
                {
                    // isTranslate=false, isNotGetInCache=true, islock=false, isSaveToRam=false
                    // -> lay ban moi nhat, khong dung va cung khong ghi de bo nho dem chung.
                    list = BackendDataWorker.Get<HIS_CONFIG>(false, true, false, false);
                    if (list == null || list.Count == 0)
                    {
                        // Khong lay duoc ban moi thi quay ve bo nho dem, con hon khong co gi.
                        list = BackendDataWorker.Get<HIS_CONFIG>();
                    }
                    sytConfigSnapshot = list;
                    sytConfigSnapshotTime = DateTime.Now;
                }

                if (list == null) return null;
                foreach (var c in list)
                {
                    if (c != null && c.KEY == key) return c.VALUE;
                }
                return null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }

        /// <summary>Bỏ bản chụp cấu hình để lần đọc sau lấy lại từ nguồn ngay lập tức.</summary>
        private static void ResetSytConfigSnapshot()
        {
            sytConfigSnapshot = null;
            sytConfigSnapshotTime = DateTime.MinValue;
        }

        /// <summary>Danh sách mã danh mục cần tải — lấy từ cấu hình, để trống thì dùng danh sách mặc định.</summary>
        private static string[] GetSytCatalogCodes()
        {
            try
            {
                string raw = GetSytConfigValue(CFG_KEY__SYT_HCM_CATEGORY_CODES);
                if (string.IsNullOrWhiteSpace(raw)) return SYT_CATALOG_CODES_DEFAULT;
                var codes = raw.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                List<string> rs = new List<string>();
                foreach (string c in codes)
                {
                    string t = c.Trim();
                    if (t.Length > 0) rs.Add(t);
                }
                return rs.Count > 0 ? rs.ToArray() : SYT_CATALOG_CODES_DEFAULT;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return SYT_CATALOG_CODES_DEFAULT; }
        }

        #endregion

        #region ===== Nơi lưu tại máy trạm =====

        /// <summary>Tên thư mục con chứa danh mục của cổng, nằm trong thư mục chạy ứng dụng.</summary>
        private const string SYT_CATALOG_DIR_NAME = "SytHcmCatalog";

        private static string sytCatalogDirResolved;

        /// <summary>
        /// Thư mục lưu danh mục của cổng — ĐẶT TRONG THƯ MỤC CHẠY ỨNG DỤNG, cùng chỗ với Logs.
        ///
        /// VÌ SAO KHÔNG DÙNG THƯ MỤC RIÊNG CỦA NGƯỜI DÙNG WINDOWS: đã chốt là danh mục lưu cache
        /// THEO MÁY. Đặt trong thư mục riêng của người dùng thì mỗi người đăng nhập vào cùng máy lại
        /// tải lại một bản, và nhân viên triển khai không biết tìm ở đâu. Đặt cạnh Logs thì mở ra là thấy.
        ///
        /// Máy cài ở nơi không cho ghi (ví dụ Program Files) thì quay về thư mục riêng của người dùng,
        /// còn hơn là không cache được gì.
        /// </summary>
        private static string GetSytCatalogDir()
        {
            if (!string.IsNullOrEmpty(sytCatalogDirResolved)) return sytCatalogDirResolved;
            try
            {
                string appDir = HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath;
                string dir = Path.Combine(appDir, SYT_CATALOG_DIR_NAME);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                // Thử ghi một tệp nhỏ để biết chắc có quyền ghi, không chỉ dựa vào việc tạo được thư mục.
                string probe = Path.Combine(dir, "_ghi_thu.tmp");
                File.WriteAllText(probe, "1", Encoding.UTF8);
                File.Delete(probe);

                sytCatalogDirResolved = dir;
                LogSystem.Debug("SytCatalog: thu muc luu danh muc = " + dir);
                return sytCatalogDirResolved;
            }
            catch (Exception ex)
            {
                LogSystem.Warn("SytCatalog: khong ghi duoc vao thu muc ung dung ("
                    + ex.GetType().Name + ") -> dung thu muc rieng cua nguoi dung Windows");
            }

            try
            {
                string dir = Path.Combine(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "HIS"), Path.Combine("KskSytHcm", "Catalog"));
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                sytCatalogDirResolved = dir;
                return sytCatalogDirResolved;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }

        private static string GetSytCatalogFile(string code)
        {
            // Mã danh mục do Sở đặt, chỉ gồm chữ và gạch dưới — vẫn lọc để chắc chắn không lạc thư mục.
            StringBuilder safe = new StringBuilder();
            foreach (char ch in code)
            {
                if (char.IsLetterOrDigit(ch) || ch == '_' || ch == '-') safe.Append(ch);
            }
            return Path.Combine(GetSytCatalogDir(), safe.ToString() + ".json");
        }

        /// <summary>
        /// Dấu nhận của cấu hình đang dùng. Đổi địa chỉ hoặc tài khoản thì dấu nhận đổi theo,
        /// khi đó bỏ hết tệp danh mục đã lưu để tải lại — nếu không sẽ dùng nhầm danh mục
        /// tải từ địa chỉ cũ mà không hiểu vì sao dữ liệu không đổi.
        /// </summary>
        private static string BuildSytConfigStamp(SytConnectionInfo cfg)
        {
            if (cfg == null) return "";
            return (cfg.AuthBaseUrl ?? "") + "|" + (cfg.ApiBaseUrl ?? "") + "|" + (cfg.Username ?? "");
        }

        private static string GetSytStampFile()
        {
            return Path.Combine(GetSytCatalogDir(), "_config_stamp.txt");
        }

        /// <summary>Cấu hình đã đổi so với lần tải trước -> xóa hết tệp danh mục đã lưu.</summary>
        private static void DropSytCatalogsIfConfigChanged(SytConnectionInfo cfg)
        {
            try
            {
                string stamp = BuildSytConfigStamp(cfg);
                string f = GetSytStampFile();
                string old = File.Exists(f) ? File.ReadAllText(f, Encoding.UTF8) : null;
                if (old == stamp) return;

                int removed = 0;
                foreach (string file in Directory.GetFiles(GetSytCatalogDir(), "*.json"))
                {
                    try { File.Delete(file); removed++; }
                    catch (Exception exDel) { LogSystem.Warn(exDel); }
                }
                lock (sytCatalogLock) { sytCatalogs.Clear(); }
                File.WriteAllText(f, stamp, Encoding.UTF8);

                if (old != null)
                {
                    LogSystem.Warn("SytCatalog: CAU HINH DA DOI (dia chi hoac tai khoan) -> da bo "
                        + removed + " tep danh muc cu, se tai lai tu dau");
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Bản lưu tại máy còn trong thời hạn dùng lại.</summary>
        private static bool IsSytCatalogFresh(string code)
        {
            try
            {
                string f = GetSytCatalogFile(code);
                if (!File.Exists(f)) return false;
                return (DateTime.Now - File.GetLastWriteTime(f)).TotalHours < SYT_CATALOG_TTL_HOURS;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return false; }
        }

        private static List<SytCatalogItem> ReadSytCatalogFile(string code)
        {
            try
            {
                string f = GetSytCatalogFile(code);
                if (!File.Exists(f)) return null;
                string json = File.ReadAllText(f, Encoding.UTF8);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<SytCatalogItem>>(json);
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }

        private static void WriteSytCatalogFile(string code, List<SytCatalogItem> items)
        {
            try
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(items);
                File.WriteAllText(GetSytCatalogFile(code), json, Encoding.UTF8);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #endregion

        #region ===== Tải danh mục =====

        /// <summary>
        /// Nạp danh mục cổng Sở Y tế trên LUỒNG RIÊNG. Gọi được nhiều lần, lần sau tự bỏ qua.
        /// KHÔNG ném lỗi ra ngoài — hỏng phần này thì màn hình nhập vẫn dùng bình thường.
        /// </summary>
        private void StartLoadSytCatalogs()
        {
            try
            {
                // Mo man hinh -> doc lai cau hinh tu nguon, khong dung ban chup cu.
                ResetSytConfigSnapshot();

                lock (sytCatalogLock)
                {
                    if (sytCatalogLoading) return;
                    sytCatalogLoading = true;
                }

                System.Threading.Tasks.Task.Factory.StartNew(
                    () => { LoadSytCatalogsCore(); ApplySytCatalogSafe(); },
                    System.Threading.CancellationToken.None,
                    System.Threading.Tasks.TaskCreationOptions.LongRunning,
                    System.Threading.Tasks.TaskScheduler.Default);
            }
            catch (Exception ex)
            {
                sytCatalogLoading = false;
                LogSystem.Warn(ex);
            }
        }

        private static void LoadSytCatalogsCore()
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                string rawCfg = GetSytConfigValue(CFG_KEY__SYT_HCM_CONNECTION_INFO);
                if (string.IsNullOrWhiteSpace(rawCfg))
                {
                    LogSystem.Warn("SytCatalog: CHUA KHAI BAO ban ghi cau hinh "
                        + CFG_KEY__SYT_HCM_CONNECTION_INFO + " -> bo qua, giu nguyen danh muc cua HIS");
                    return;
                }

                SytConnectionInfo cfg = GetSytConnectionInfo();
                if (cfg == null)
                {
                    // Neu den day thi da khai bao nhung THIEU truong bat buoc — chi ro thieu gi.
                    string[] fp = rawCfg.Split('|');
                    List<string> missing = new List<string>();
                    if (string.IsNullOrWhiteSpace(Field(fp, 1))) missing.Add("tai khoan (truong 2)");
                    if (string.IsNullOrWhiteSpace(Field(fp, 2))) missing.Add("mat khau (truong 3)");
                    if (string.IsNullOrWhiteSpace(Field(fp, 3))) missing.Add("dia chi dich vu xac thuc (truong 4)");
                    LogSystem.Warn("SytCatalog: cau hinh THIEU TRUONG BAT BUOC ["
                        + string.Join(", ", missing.ToArray()) + "] — ban ghi dang co "
                        + fp.Length + " truong, can it nhat 4");
                    return;
                }

                // Ghi lai dia chi dang dung de doi chieu khi goi that bai. KHONG ghi tai khoan/mat khau.
                LogSystem.Warn("SytCatalog: bat dau tai — dia chi xac thuc=" + cfg.AuthBaseUrl
                    + " , dia chi nghiep vu=" + cfg.ApiBaseUrl);

                // Doi dia chi/tai khoan thi bo danh muc da tai tu cau hinh cu.
                DropSytCatalogsIfConfigChanged(cfg);

                string[] codes = GetSytCatalogCodes();

                // Đọc trước những danh mục còn hạn, chỉ tải phần thiếu hoặc quá hạn.
                List<string> needFetch = new List<string>();
                foreach (string code in codes)
                {
                    if (IsSytCatalogFresh(code))
                    {
                        var cached = ReadSytCatalogFile(code);
                        if (cached != null && cached.Count > 0)
                        {
                            lock (sytCatalogLock) { sytCatalogs[code] = cached; }
                            continue;
                        }
                    }
                    needFetch.Add(code);
                }

                if (needFetch.Count == 0)
                {
                    LogSystem.Debug("SytCatalog: dung ban luu tai may cho toan bo " + codes.Length + " danh muc");
                    return;
                }

                string token = GetSytAccessToken(cfg);
                if (string.IsNullOrEmpty(token))
                {
                    LogSystem.Warn("SytCatalog: khong lay duoc phieu truy cap, bo qua lan tai nay");
                    return;
                }

                // Tải SONG SONG: 29 danh mục nối đuôi nhau thì mỗi lượt phải chờ hết một vòng
                // gọi mạng, cộng lại mất 10-30 giây mới có danh mục để chọn. Kết quả không đổi,
                // chỉ khác thứ tự dòng ghi nhật ký.
                RaiseSytConnectionLimit(cfg.ApiBaseUrl);

                int ok = 0, fail = 0;
                int fetchThreads = GetSytFetchThreads();
                LogSystem.Debug("SytCatalog: tai bang " + fetchThreads + " luong (may co "
                    + Environment.ProcessorCount + " loi)");
                var opt = new System.Threading.Tasks.ParallelOptions();
                opt.MaxDegreeOfParallelism = fetchThreads;
                System.Threading.Tasks.Parallel.ForEach(needFetch, opt, code =>
                {
                    try
                    {
                        List<SytCatalogItem> items = FetchSytCatalog(cfg, token, code);
                        if (items != null && items.Count > 0)
                        {
                            lock (sytCatalogLock) { sytCatalogs[code] = items; }
                            WriteSytCatalogFile(code, items);
                            System.Threading.Interlocked.Increment(ref ok);
                            LogSystem.Debug("SytCatalog: " + code + " -> " + items.Count + " muc");
                        }
                        else
                        {
                            // Tải hỏng thì giữ bản cũ tại máy (nếu có) để còn dùng được.
                            var old = ReadSytCatalogFile(code);
                            if (old != null && old.Count > 0)
                            {
                                lock (sytCatalogLock) { sytCatalogs[code] = old; }
                            }
                            System.Threading.Interlocked.Increment(ref fail);
                        }
                    }
                    catch (Exception exOne)
                    {
                        // Một danh mục hỏng KHÔNG được làm chết cả lượt tải — Parallel.ForEach sẽ
                        // gói lỗi lại và dừng các danh mục còn lại nếu để lỗi thoát ra ngoài.
                        LogSystem.Warn(exOne);
                        System.Threading.Interlocked.Increment(ref fail);
                    }
                });

                LogSystem.Warn(string.Format(
                    "SytCatalog: KET QUA — tai {0} danh muc, thanh cong {1}, that bai {2}, het {3} ms",
                    needFetch.Count, ok, fail, sw.ElapsedMilliseconds));

                if (fail == 0) RemoveOrphanSytCatalogFiles(codes);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            finally
            {
                lock (sytCatalogLock) { sytCatalogLoading = false; }
            }
        }

        /// <summary>
        /// Lấy phiếu truy cập. Phiếu có hiệu lực 2 giờ nhưng ở đây chỉ dùng trong một lượt tải
        /// nên KHÔNG lưu lại — tránh để phiếu nằm trên đĩa.
        /// </summary>
        private static string GetSytAccessToken(SytConnectionInfo cfg)
        {
            try
            {
                string url = cfg.AuthBaseUrl + "/hin-auth/getToken";
                string body = Newtonsoft.Json.JsonConvert.SerializeObject(
                    new { username = cfg.Username, password = cfg.Password });

                string resp = HttpSyt(url, "POST", body, null);
                if (string.IsNullOrEmpty(resp)) return null;

                var o = Newtonsoft.Json.Linq.JObject.Parse(resp);
                var t = o["result"];
                if (t == null) return null;
                var at = t["access_token"];
                return (at != null) ? at.ToString() : null;
            }
            catch (WebException wex)
            {
                // KHONG ghi noi dung tra ve vi co the chua phieu truy cap.
                LogSystem.Warn("SytCatalog: LAY PHIEU TRUY CAP THAT BAI tai "
                    + cfg.AuthBaseUrl + "/hin-auth/getToken — " + DescribeWebError(wex));
                return null;
            }
            catch (Exception ex)
            {
                LogSystem.Warn("SytCatalog: loi khi lay phieu truy cap — " + ex.GetType().Name);
                return null;
            }
        }

        /// <summary>Gọi dịch vụ lấy một danh mục theo mã.</summary>
        private static List<SytCatalogItem> FetchSytCatalog(SytConnectionInfo cfg, string token, string code)
        {
            try
            {
                string url = cfg.ApiBaseUrl + "/hin-api-service/kskdk-danh-muc-service?code="
                           + Uri.EscapeDataString(code);
                string resp = HttpSyt(url, "GET", null, token);
                if (string.IsNullOrEmpty(resp)) return null;
                return ParseSytCatalog(resp);
            }
            catch (WebException wex)
            {
                LogSystem.Warn("SytCatalog: tai danh muc " + code + " THAT BAI — " + DescribeWebError(wex));
                return null;
            }
            catch (Exception ex)
            {
                LogSystem.Warn("SytCatalog: loi khi tai danh muc " + code + " — " + ex.GetType().Name);
                return null;
            }
        }

        /// <summary>
        /// Đọc danh sách mục từ kết quả trả về. Cổng có thể bọc trong "result" hoặc "data",
        /// và tên trường của mỗi mục chưa được Sở chốt nên dò theo nhiều tên thường gặp.
        /// </summary>
        private static List<SytCatalogItem> ParseSytCatalog(string json)
        {
            List<SytCatalogItem> rs = new List<SytCatalogItem>();
            try
            {
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JToken.Parse(json);
                Newtonsoft.Json.Linq.JToken arr = null;

                if (root is Newtonsoft.Json.Linq.JArray)
                {
                    arr = root;
                }
                else
                {
                    foreach (string k in new[] { "result", "data", "items", "Result", "Data" })
                    {
                        var t = root[k];
                        if (t == null) continue;
                        if (t is Newtonsoft.Json.Linq.JArray) { arr = t; break; }
                        foreach (string k2 in new[] { "data", "items", "list" })
                        {
                            var t2 = t[k2];
                            if (t2 is Newtonsoft.Json.Linq.JArray) { arr = t2; break; }
                        }
                        if (arr != null) break;
                    }
                }
                if (arr == null) return rs;

                foreach (var it in arr)
                {
                    SytCatalogItem x = new SytCatalogItem();
                    x.Id = PickField(it, "id", "Id", "ID", "value", "Value");
                    x.Code = PickField(it, "code", "Code", "ma", "MA");
                    x.Name = PickField(it, "name", "Name", "ten", "TEN", "text", "Text", "label");
                    if (!string.IsNullOrEmpty(x.Id) || !string.IsNullOrEmpty(x.Name)) rs.Add(x);
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            return rs;
        }

        private static string PickField(Newtonsoft.Json.Linq.JToken it, params string[] names)
        {
            foreach (string n in names)
            {
                var v = it[n];
                if (v != null && v.Type != Newtonsoft.Json.Linq.JTokenType.Null)
                {
                    string s = v.ToString();
                    if (!string.IsNullOrEmpty(s)) return s;
                }
            }
            return null;
        }

        /// <summary>
        /// Mô tả lỗi gọi dịch vụ cho dễ chẩn đoán: mã trạng thái nếu máy chủ có trả lời,
        /// ngược lại là loại lỗi mạng (không phân giải được tên miền, quá hạn chờ...).
        /// KHÔNG ghi nội dung trả về.
        /// </summary>
        private static string DescribeWebError(WebException wex)
        {
            try
            {
                HttpWebResponse res = wex.Response as HttpWebResponse;
                if (res != null)
                    return "ma trang thai " + (int)res.StatusCode + " " + res.StatusCode;

                if (wex.Status == WebExceptionStatus.TrustFailure)
                    return "chung thu so cua may chu khong duoc tin cay (TrustFailure)";
                if (wex.Status == WebExceptionStatus.SecureChannelFailure)
                    return "bat tay TLS that bai (SecureChannelFailure) — may chu doi giao thuc "
                         + "cao hon muc chuong trinh dang bat";
                if (wex.Status == WebExceptionStatus.NameResolutionFailure)
                    return "khong phan giai duoc ten mien (NameResolutionFailure) — kiem tra dia chi";
                if (wex.Status == WebExceptionStatus.Timeout)
                    return "qua han cho (Timeout)";
                return "khong nhan duoc tra loi (" + wex.Status + ")";
            }
            catch { return "loi mang"; }
        }

        /// <summary>Gọi dịch vụ. Trả null khi lỗi — nơi gọi tự xử lý, không ném ra luồng giao diện.</summary>
        /// <summary>Đã bật TLS 1.2 hay chưa — chỉ cần đặt một lần cho cả tiến trình.</summary>
        private static bool sytTlsEnabled = false;

        /// <summary>
        /// Bật TLS 1.2 cho các lệnh gọi ra ngoài.
        ///
        /// .NET Framework 4.5 mặc định chỉ bật SSL3 và TLS 1.0. Máy chủ của Sở Y tế yêu cầu
        /// TLS 1.2 nên bắt tay hỏng ngay, báo SecureChannelFailure mà chưa gửi được gì.
        ///
        /// Dùng phép HỢP để THÊM chứ không thay thế: giữ nguyên các giao thức chương trình
        /// đang bật cho những kết nối khác, tránh làm hỏng tích hợp sẵn có.
        /// </summary>
        /// <summary>
        /// Xóa tệp danh mục của những mã KHÔNG CÒN DÙNG.
        ///
        /// Tên tệp lấy theo mã danh mục, nên khi Sở đổi tên mã (mẫu M4 sang M3 đã đổi vài mã) thì
        /// tệp của mã cũ nằm lại vĩnh viễn: không ai tải nữa, cũng không ai xóa. Mỗi tệp chỉ vài KB
        /// nên không nặng máy, nhưng để lẫn thì về sau không biết tệp nào còn dùng.
        ///
        /// CHỈ dọn khi cả lượt tải KHÔNG có danh mục nào hỏng. Nếu đang hỏng mạng mà vẫn dọn thì có
        /// thể xóa mất bản lưu còn dùng được.
        /// Chỉ xét tệp .json — dấu nhận cấu hình là tệp .txt nên không bị chạm tới.
        /// </summary>
        private static void RemoveOrphanSytCatalogFiles(string[] codes)
        {
            try
            {
                string dir = GetSytCatalogDir();
                if (string.IsNullOrEmpty(dir) || codes == null || codes.Length == 0) return;

                var inUse = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (string code in codes)
                {
                    if (!string.IsNullOrWhiteSpace(code))
                        inUse.Add(Path.GetFileNameWithoutExtension(GetSytCatalogFile(code)));
                }

                int removed = 0;
                foreach (string file in Directory.GetFiles(dir, "*.json"))
                {
                    if (inUse.Contains(Path.GetFileNameWithoutExtension(file))) continue;
                    try { File.Delete(file); removed++; }
                    catch (Exception exDel) { LogSystem.Warn(exDel); }
                }
                if (removed > 0)
                    LogSystem.Warn("SytCatalog: da don " + removed
                        + " tep danh muc cua ma khong con dung");
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private static void EnsureSytTls()
        {
            try
            {
                if (sytTlsEnabled) return;
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
                sytTlsEnabled = true;
                LogSystem.Debug("SytCatalog: da bat TLS 1.2 cho ket noi ra ngoai");
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Số danh mục tải cùng lúc. .NET chỉ cho MỖI MÁY CHỦ 2 kết nối đồng thời, nên phải nâng
        /// giới hạn riêng cho máy chủ của Sở, nếu không đặt bao nhiêu luồng cũng chỉ chạy được 2.
        /// Nâng RIÊNG theo máy chủ, KHÔNG đổi giới hạn chung của phần mềm — đổi chung sẽ ảnh hưởng
        /// 4 cổng cũ và các viện khác.
        ///
        /// Để 5: đủ nhanh mà không dồn dập khiến cổng chặn.
        /// </summary>
        private const int SYT_PARALLEL_FETCH = 5;

        /// <summary>
        /// Số luồng tải THẬT — co theo số lõi của máy, không cố định 5.
        ///
        /// Nhiều viện dùng máy cũ 1-2 lõi và đường truyền yếu. Ở đó 5 kết nối cùng lúc phải tranh
        /// nhau CPU (giải mã TLS, đọc tệp danh mục ICD hơn 1 MB) và giành luồng của phần còn lại
        /// trong phần mềm, có khi còn chậm hơn và dễ hết thời gian chờ hơn là tải nối đuôi nhau.
        ///
        /// Máy 1 lõi -> 1 luồng, tức đúng y cách làm cũ. Máy 2 lõi -> 2. Máy khỏe -> tối đa 5.
        /// </summary>
        private static int GetSytFetchThreads()
        {
            try
            {
                int cores = Environment.ProcessorCount;
                if (cores < 1) cores = 1;
                return (cores < SYT_PARALLEL_FETCH) ? cores : SYT_PARALLEL_FETCH;
            }
            catch { return 1; }
        }

        private static void RaiseSytConnectionLimit(string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url)) return;
                int threads = GetSytFetchThreads();
                ServicePoint sp = ServicePointManager.FindServicePoint(new Uri(url));
                if (sp != null && sp.ConnectionLimit < threads)
                    sp.ConnectionLimit = threads;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private static string HttpSyt(string url, string method, string body, string bearerToken)
        {
            EnsureSytTls();
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = method;
            req.Timeout = SYT_HTTP_TIMEOUT_MS;
            req.ReadWriteTimeout = SYT_HTTP_TIMEOUT_MS;
            req.Accept = "application/json";
            if (!string.IsNullOrEmpty(bearerToken))
                req.Headers[HttpRequestHeader.Authorization] = "Bearer " + bearerToken;

            if (!string.IsNullOrEmpty(body))
            {
                req.ContentType = "application/json";
                byte[] data = Encoding.UTF8.GetBytes(body);
                req.ContentLength = data.Length;
                using (Stream s = req.GetRequestStream()) s.Write(data, 0, data.Length);
            }

            using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
            using (StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
            {
                return sr.ReadToEnd();
            }
        }

        #endregion

        #region ===== Dùng danh mục đã tải =====

        /// <summary>Lấy một danh mục đã tải. Chưa có thì trả danh sách rỗng, KHÔNG gọi mạng.</summary>
        private static List<SytCatalogItem> GetSytCatalog(string code)
        {
            lock (sytCatalogLock)
            {
                List<SytCatalogItem> rs;
                if (sytCatalogs.TryGetValue(code, out rs) && rs != null) return rs;
            }
            var fromFile = ReadSytCatalogFile(code);
            if (fromFile != null)
            {
                lock (sytCatalogLock) { sytCatalogs[code] = fromFile; }
                return fromFile;
            }
            return new List<SytCatalogItem>();
        }

        #endregion

        // TODO(FE): đổ danh mục vào 2 ô chọn ngoại lệ của mẫu M4 sau khi tải xong:
        //   - Địa điểm khám            -> GetSytCatalog("Diadiemkham")
        //   - Hình thức chi trả chi tiết -> GetSytCatalog("ChiTraChiTiet_NCT")
        //   Vì tải chạy trên luồng riêng, phải quay lại luồng giao diện (Invoke) trước khi gán
        //   nguồn dữ liệu cho ô chọn.
    }
}

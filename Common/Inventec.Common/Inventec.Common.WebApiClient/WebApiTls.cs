using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Inventec.Common.WebApiClient
{
    /// <summary>
    /// BUG-002: cau hinh TLS cho phia goi API.
    ///
    /// Giai quyet bai toan: he thong dung chung thu noi bo (self-signed CA) nen may tram
    /// khong tin chung thu do, ma cai CA vao Trusted Root cua hang tram may tram thi can
    /// quyen admin va rat kho van hanh.
    ///
    /// Cach lam: nap CA noi bo tu mot file cau hinh va coi do la mot "gốc tin cậy bo sung"
    /// CHI trong pham vi tien trinh nay - tuong duong viec cai CA vao Trusted Root nhung
    /// khong dung toi machine store va khong can quyen admin.
    ///
    /// Quy tac xac thuc (xem <see cref="Validate"/>):
    ///     1. Chung thu hop le theo Windows store  -> CHAP NHAN
    ///        (giu nguyen cac tich hop ben ngoai: Gmail SMTP, Napas, Vietinbank, HL7...)
    ///     2. Chung thu chain ve dung CA trong file cau hinh -> CHAP NHAN (host noi bo)
    ///     3. Con lai -> TU CHOI
    ///
    /// Cau hinh (appSettings) - Inventec.Common.WebApiClient.Tls.TrustedCaFile:
    ///
    ///   1. Duong dan tuyet doi ("C:\ProgramData\IMSys\his-root-ca.cer" hoac UNC)
    ///      -> dung dung file do.
    ///   2. Chi ten file / duong dan tuong doi ("his-root-ca.cer", "certs\ca.cer")
    ///      -> tim trong cac thu muc cua ung dung, xem <see cref="ProbeDirectories"/>.
    ///   3. DE TRONG -> tu tim file ten mac dinh (his-root-ca.cer / his-root-ca.crt)
    ///      trong cac thu muc do. Khong thay thi giu nguyen kiem tra mac dinh cua Windows.
    ///
    /// Nho vay moi noi trien khai chi can tha file CA canh ung dung la chay, khong phai
    /// sua cau hinh theo tung duong dan cai dat.
    ///
    /// LUU Y VAN HANH:
    ///   - File CA o day chi chua PUBLIC KEY, khong phai bi mat. Tuyet doi khong dong goi
    ///     file .pfx/.key (co private key) kem bo cai.
    ///   - AI GHI DUOC VAO THU MUC UNG DUNG THI THA DUOC FILE CA VAO DO va mao danh duoc
    ///     server. Vi vay ung dung phai duoc cai o noi chi admin ghi duoc (%ProgramFiles%,
    ///     hoac %ProgramData%\IMSys da siet ACL) - dieu nay dung ca khi khai bao duong dan
    ///     tuyet doi, vi ke ghi duoc vao thu muc ung dung cung sua duoc chinh file cau hinh.
    ///   - Moi lan nap CA deu ghi log Info kem duong dan + thumbprint de con doi soat khi
    ///     kiem tra an toan thong tin.
    /// </summary>
    public static class WebApiTls
    {
        private const string CFG_TRUSTED_CA_FILE = "Inventec.Common.WebApiClient.Tls.TrustedCaFile";

        //Ten file duoc tu dong tim khi cau hinh de trong.
        private static readonly string[] DEFAULT_CA_FILE_NAMES = new string[] { "his-root-ca.cer", "his-root-ca.crt" };

        //TLS 1.2. Viet bang so vi .NET 4.5 chua co hang so SecurityProtocolType.Tls12.
        private const SecurityProtocolType TLS_12 = (SecurityProtocolType)3072;

        private static readonly object LOCK = new object();
        private static bool initialized;
        private static X509Certificate2 trustedCa;

        /// <summary>
        /// Nap cau hinh va dang ky co che kiem tra chung thu. Goi nhieu lan cung chi chay mot lan.
        ///
        /// Da duoc goi san trong static constructor cua <see cref="ApiConsumer"/>, nen moi loi goi
        /// API qua ApiConsumer/ApiConsumerWrapper deu duoc phu.
        ///
        /// Ung dung desktop CAN goi tuong minh mot lan luc khoi dong: luong dang nhap
        /// (Inventec.Token.ClientSystem.ClientTokenManager) tu tao HttpClient rieng, khong di
        /// qua ApiConsumer, nen neu khong goi thi request dang nhap khong duoc phu.
        /// </summary>
        public static void Init()
        {
            if (initialized) return;

            lock (LOCK)
            {
                if (initialized) return;
                //Danh dau truoc: co loi thi cung khong thu lai o moi request
                initialized = true;

                try
                {
                    //.NET 4.5 mac dinh chi bat Ssl3|Tls(1.0) cho ket noi di ra. Thieu dong nay thi
                    //sau khi server chuyen sang TLS 1.2 moi ket noi server<->server se dut.
                    //Cong them chu khong ghi de, de khong tat giao thuc ma noi khac da bat.
                    ServicePointManager.SecurityProtocol |= TLS_12;

                    List<string> probeDirs = ProbeDirectories();
                    string configured = (ConfigurationManager.AppSettings[CFG_TRUSTED_CA_FILE] ?? "").Trim();
                    string caFile;
                    bool autoDiscovered = false;

                    if (configured.Length > 0)
                    {
                        caFile = ResolveConfiguredPath(configured, probeDirs);
                        if (caFile == null)
                        {
                            //Da khai bao ma khong tim thay => loi cau hinh, phai bao chu khong duoc im lang
                            throw new FileNotFoundException("Khong tim thay file CA khai bao tai '"
                                + CFG_TRUSTED_CA_FILE + "' = '" + configured + "'."
                                + " Da tim trong: " + string.Join(" | ", probeDirs.ToArray()), configured);
                        }
                    }
                    else
                    {
                        caFile = FindDefaultCaFile(probeDirs);
                        if (caFile == null)
                        {
                            LogSystem.Info("WebApiTls: chua cau hinh '" + CFG_TRUSTED_CA_FILE
                                + "' va khong tim thay file mac dinh (" + string.Join(", ", DEFAULT_CA_FILE_NAMES)
                                + ") trong: " + string.Join(" | ", probeDirs.ToArray())
                                + " => giu nguyen kiem tra chung thu mac dinh cua Windows.");
                            return;
                        }
                        autoDiscovered = true;
                    }

                    X509Certificate2 ca = new X509Certificate2(caFile);
                    trustedCa = ca;

                    if (ServicePointManager.ServerCertificateValidationCallback != null)
                    {
                        LogSystem.Warn("WebApiTls: da ton tai ServerCertificateValidationCallback khac va se bi thay the. "
                            + "Neu co tich hop nao dua vao callback cu thi phai kiem tra lai.");
                    }
                    ServicePointManager.ServerCertificateValidationCallback = Validate;

                    //Ghi day du de con doi soat khi kiem tra an toan thong tin: nap CA nao, tu dau,
                    //do cau hinh chi dinh hay do tu tim thay.
                    LogSystem.Info("WebApiTls: da nap CA noi bo lam goc tin cay bo sung."
                        + "____nguon=" + (autoDiscovered ? "tu tim canh ung dung" : "cau hinh " + CFG_TRUSTED_CA_FILE)
                        + "____file=" + caFile
                        + "____subject=" + ca.Subject
                        + "____thumbprint=" + ca.Thumbprint
                        + "____notAfter=" + ca.NotAfter.ToString("dd/MM/yyyy"));
                }
                catch (Exception ex)
                {
                    //Fail-closed: da khai bao file CA ma khong nap duoc thi KHONG dang ky callback.
                    //Hau qua: cac host noi bo dung chung thu tu ky se bi Windows tu choi ngay
                    //(loi ro rang, dung ngay) thay vi chay tiep trong trang thai khong duoc bao ve.
                    //Cac ket noi ra ngoai dung CA cong cong van hoat dong binh thuong.
                    trustedCa = null;
                    LogSystem.Error("WebApiTls: nap CA noi bo that bai => KHONG bat goc tin cay bo sung. "
                        + "Ket noi toi host dung chung thu noi bo se bi tu choi cho den khi sua cau hinh '"
                        + CFG_TRUSTED_CA_FILE + "'.", ex);
                }
            }
        }

        /// <summary>
        /// Cac thu muc se tim file CA, theo thu tu uu tien.
        ///
        /// Thu vien nay chay o hai moi truong khac han nhau nen khong the chi lay mot cho:
        ///   - Ung dung desktop: BaseDirectory chinh la thu muc chua file .exe.
        ///   - Web app tren IIS : BaseDirectory la thu muc goc cua site (noi dat Web.config),
        ///                        con DLL nam trong thu muc con "bin" (RelativeSearchPath).
        /// Nen ca hai deu duoc dua vao danh sach, cong them thu muc that su chua DLL nay.
        /// </summary>
        private static List<string> ProbeDirectories()
        {
            List<string> dirs = new List<string>();
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                AddProbeDir(dirs, baseDir);

                //Web app: "bin". RelativeSearchPath co the chua nhieu duong dan ngan cach bang ';'.
                string relative = AppDomain.CurrentDomain.RelativeSearchPath;
                if (!string.IsNullOrEmpty(relative) && !string.IsNullOrEmpty(baseDir))
                {
                    string[] parts = relative.Split(';');
                    for (int i = 0; i < parts.Length; i++)
                    {
                        string part = parts[i].Trim();
                        if (part.Length > 0) AddProbeDir(dirs, Path.Combine(baseDir, part));
                    }
                }

                AddProbeDir(dirs, GetAssemblyDirectory());
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return dirs;
        }

        private static void AddProbeDir(List<string> dirs, string dir)
        {
            try
            {
                if (string.IsNullOrEmpty(dir)) return;
                string full = Path.GetFullPath(dir);

                //Chuan hoa truoc khi so trung: BaseDirectory co dau phan cach o cuoi con thu muc
                //cua assembly thi khong, de nguyen se coi la hai thu muc khac nhau.
                full = full.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                if (full.Length == 0) return;
                //Rieng goc o dia ("C:") phai giu lai dau phan cach, khong thi Path.Combine hieu sai
                if (full.EndsWith(":")) full += Path.DirectorySeparatorChar;

                for (int i = 0; i < dirs.Count; i++)
                {
                    if (string.Equals(dirs[i], full, StringComparison.OrdinalIgnoreCase)) return;
                }
                dirs.Add(full);
            }
            catch (Exception ex)
            {
                //Duong dan khong hop le thi bo qua, khong duoc lam hong ca qua trinh khoi tao
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Thu muc that su chua DLL nay. Dung CodeBase chu khong dung Location: tren IIS assembly
        /// bi shadow-copy nen Location tro vao "Temporary ASP.NET Files", khong phai thu muc trien khai.
        /// </summary>
        private static string GetAssemblyDirectory()
        {
            try
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                if (string.IsNullOrEmpty(codeBase)) return null;
                Uri uri = new Uri(codeBase);
                if (!uri.IsFile) return null;
                return Path.GetDirectoryName(uri.LocalPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>
        /// Doi gia tri cau hinh thanh duong dan that. Duong dan tuyet doi (ke ca UNC) thi dung
        /// nguyen; duong dan tuong doi thi tim lan luot trong cac thu muc cua ung dung.
        /// Tra ve null neu khong tim thay.
        /// </summary>
        private static string ResolveConfiguredPath(string configured, List<string> probeDirs)
        {
            try
            {
                if (Path.IsPathRooted(configured))
                {
                    return File.Exists(configured) ? configured : null;
                }

                for (int i = 0; i < probeDirs.Count; i++)
                {
                    string candidate = Path.Combine(probeDirs[i], configured);
                    if (File.Exists(candidate)) return candidate;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return null;
        }

        /// <summary>
        /// Khong khai bao cau hinh: tim file ten mac dinh canh ung dung. Tra ve null neu khong co
        /// - day la truong hop binh thuong (backend da cai CA vao machine store), khong phai loi.
        /// </summary>
        private static string FindDefaultCaFile(List<string> probeDirs)
        {
            try
            {
                for (int i = 0; i < probeDirs.Count; i++)
                {
                    for (int j = 0; j < DEFAULT_CA_FILE_NAMES.Length; j++)
                    {
                        string candidate = Path.Combine(probeDirs[i], DEFAULT_CA_FILE_NAMES[j]);
                        if (File.Exists(candidate)) return candidate;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return null;
        }

        /// <summary>
        /// Chung thu do server tra ve co duoc chap nhan hay khong.
        /// Callback nay la process-global: no bat MOI ket noi TLS cua tien trinh, ke ca cac
        /// tich hop ben ngoai. Vi vay nhanh dau tien phai giu nguyen ket qua kiem tra chuan.
        /// </summary>
        private static bool Validate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            try
            {
                //1. Hop le theo Windows store: Gmail SMTP, Napas, Vietinbank, HL7... giu nguyen
                if (sslPolicyErrors == SslPolicyErrors.None) return true;

                if (certificate == null)
                {
                    LogSystem.Warn("WebApiTls: server khong gui chung thu => tu choi ket noi.");
                    return false;
                }

                //2. Sai ten mien/IP thi tu choi ngay, khong xet toi CA noi bo.
                //   X509Chain khong kiem tra ten, neu bo qua co nay thi mot chung thu do CA noi bo
                //   cap cho host A se dung duoc cho host B.
                if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateNameMismatch) != 0)
                {
                    LogSystem.Warn("WebApiTls: ten mien/IP khong khop chung thu => tu choi ket noi.____subject="
                        + certificate.Subject);
                    return false;
                }

                if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateNotAvailable) != 0)
                {
                    LogSystem.Warn("WebApiTls: khong doc duoc chung thu cua server => tu choi ket noi.");
                    return false;
                }

                //3. Chi con loi ve chuoi tin cay: chap nhan neu chain ve dung CA noi bo
                return ChainsToTrustedCa(certificate);
            }
            catch (Exception ex)
            {
                //Fail-closed: khong ket luan duoc thi tu choi
                LogSystem.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// Dung lai chuoi chung thu voi CA noi bo dong vai tro goc, va kiem tra chuoi that su
        /// ket thuc o dung CA do. Khong duoc chi dua vao ket qua Build(): voi
        /// AllowUnknownCertificateAuthority thi Build() van tra true cho MOI goc khong xac dinh.
        ///
        /// Neu file cau hinh tro thang vao chung thu cua server (thay vi CA) thi ham nay van dung:
        /// chuoi chi co mot phan tu va phan tu do phai trung thumbprint - tuc la pin truc tiep.
        /// </summary>
        private static bool ChainsToTrustedCa(X509Certificate certificate)
        {
            X509Certificate2 ca = trustedCa;
            if (ca == null) return false;

            X509Chain chain = new X509Chain();
            try
            {
                //CA noi bo khong phat hanh CRL/OCSP nen khong kiem tra thu hoi.
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                //Cho phep goc khong nam trong store; tinh hop le cua goc do kiem thu cong ben duoi.
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                chain.ChainPolicy.ExtraStore.Add(ca);

                X509Certificate2 server = new X509Certificate2(certificate);

                //Build() = false nghia la con loi khac ngoai "goc khong tin cay":
                //het han, chu ky sai, sai muc dich su dung...
                if (!chain.Build(server))
                {
                    LogSystem.Warn("WebApiTls: chung thu server khong hop le => tu choi ket noi.____subject="
                        + server.Subject + "____" + DescribeStatus(chain));
                    return false;
                }

                if (chain.ChainElements == null || chain.ChainElements.Count == 0) return false;

                X509Certificate2 root = chain.ChainElements[chain.ChainElements.Count - 1].Certificate;
                if (root == null || !string.Equals(root.Thumbprint, ca.Thumbprint, StringComparison.OrdinalIgnoreCase))
                {
                    LogSystem.Warn("WebApiTls: chuoi chung thu khong ket thuc o CA noi bo => tu choi ket noi.____subject="
                        + server.Subject + "____rootNhanDuoc=" + (root != null ? root.Subject : "null"));
                    return false;
                }

                //Den day goc da dung. Chi con duoc phep ton tai loi "goc khong nam trong store"
                //vi do chinh la truong hop binh thuong cua CA noi bo.
                foreach (X509ChainStatus status in chain.ChainStatus)
                {
                    if (status.Status != X509ChainStatusFlags.NoError
                        && status.Status != X509ChainStatusFlags.UntrustedRoot)
                    {
                        LogSystem.Warn("WebApiTls: chuoi ve dung CA noi bo nhung con loi khac => tu choi ket noi.____subject="
                            + server.Subject + "____" + DescribeStatus(chain));
                        return false;
                    }
                }

                return true;
            }
            finally
            {
                //X509Chain giu handle cua CryptoAPI, phai giai phong vi ham nay chay o moi ket noi TLS moi.
                chain.Reset();
            }
        }

        private static string DescribeStatus(X509Chain chain)
        {
            try
            {
                string result = "chainStatus=";
                foreach (X509ChainStatus status in chain.ChainStatus)
                {
                    result += status.Status.ToString() + "(" + status.StatusInformation + ") ";
                }
                return result;
            }
            catch
            {
                return "chainStatus=?";
            }
        }
    }
}

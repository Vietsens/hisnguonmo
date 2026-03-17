using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.Library.BankHub.Config;
using HIS.Desktop.Plugins.Library.BankHub.Helper;
using HIS.Desktop.Plugins.Library.BankHub.Models;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Common.Adapter;

namespace HIS.Desktop.Plugins.Library.BankHub.Popup
{
    public partial class BankLogin : HIS.Desktop.Utility.FormBase
    {
        // ---- Services ----
        private BankHubAuthService _authService;
        private PkceSession _pkceSession;
        // ---- Kết quả sau khi đăng nhập ----
        /// <summary>Token nhận được sau đăng nhập thành công. Null nếu chưa/thất bại.</summary>
        public TokenResponse TokenResult { get; private set; }
        private readonly string _bankCode;
        private readonly Action<HIS_BANK_OAUTH> _getToken;

        public BankLogin(string bankcode, Action<HIS_BANK_OAUTH> getToken)
        {
            _bankCode = bankcode;
            _getToken = getToken;

            EO.Base.Runtime.EnableEOWP = true;
            EO.WebBrowser.Runtime.AddLicense(WebLicense.license_code);
            InitializeComponent();
            webView1.BeforeNavigate += webView1_BeforeNavigate;
        }

        private void BankLogin_Load(object sender, EventArgs e)
        {
            try
            {
                _authService = new BankHubAuthService(HisConfigData.GetByConfig(_bankCode));

                // Bước 1: Tạo PKCE session
                _pkceSession = _authService.CreatePkceSession();

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _pkceSession), _pkceSession));
                // Load URL đăng nhập vào WebView
                webView1.Url = _pkceSession.LoginUrl;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                MessageBox.Show("Không thể khởi tạo đăng nhập:\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sự kiện quan trọng nhất: được gọi MỖI KHI WebView sắp điều hướng đến URL mới.
        /// Khi Keycloak redirect về redirect_uri kèm ?code=... thì bắt tại đây.
        /// </summary>
        void webView1_BeforeNavigate(object sender, EO.WebBrowser.BeforeNavigateEventArgs e)
        {
            string navigatedUrl = e.NewUrl;
            if (string.IsNullOrEmpty(navigatedUrl)) return;

            string error;
            string authCode = _authService.TryGetAuthorizationCode(navigatedUrl, _pkceSession, out error);

            if (!string.IsNullOrEmpty(error))
            {
                // Có lỗi từ Keycloak (VD: user nhấn Cancel trên trang đăng nhập)
                e.Cancel = true;
                InvokeOnUiThread(() =>
                {
                    MessageBox.Show("Đăng nhập thất bại:\n" + error,
                        "Lỗi xác thực", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                });
                return;
            }

            if (!string.IsNullOrEmpty(authCode))
            {
                // Bắt được authorization code → ngăn WebView điều hướng thật
                e.Cancel = true;

                InvokeOnUiThread(() =>
                {
                    ProcessAuthorizationCode(authCode);
                });
            }
        }

        private void ProcessAuthorizationCode(string authCode)
        {
            try
            {
                // Bước 3: Exchange code → token
                TokenResult = _authService.ExchangeCodeForToken(authCode, _pkceSession);
                if (TokenResult != null && !String.IsNullOrEmpty(TokenResult.access_token))
                {
                    CommonParam param = new CommonParam();
                    //gọi api lưu thông tin token
                    HIS_BANK_OAUTH auth = new HIS_BANK_OAUTH();
                    auth.LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    auth.ACCESS_TOKEN = TokenResult.access_token;
                    auth.REFRESH_TOKEN = TokenResult.refresh_token;
                    auth.ACCESS_TOKEN_EXPIRY = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(TokenResult.ExpiresAt);
                    auth.REFRESH_TOKEN_EXPIRY = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(TokenResult.RefreshExpiresAt);
                    auth.BANK_CODE = _bankCode;
                    HIS_BANK_OAUTH data = new BackendAdapter(param).Post<HIS_BANK_OAUTH>("/api/HisBankOauth/Create", ApiConsumers.MosConsumer, auth, param);
                    if (data != null)
                    {
                        if (_getToken != null)
                        {
                            _getToken(data);
                        }

                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
            catch (BankHubException ex)
            {
                MessageBox.Show("Không thể lấy access token:\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void InvokeOnUiThread(Action action)
        {
            if (this.InvokeRequired)
                this.Invoke(action);
            else
                action();
        }
    }
}

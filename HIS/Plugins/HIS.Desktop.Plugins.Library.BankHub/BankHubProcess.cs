using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.Library.BankHub.Config;
using HIS.Desktop.Plugins.Library.BankHub.PVCB;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Library.BankHub
{
    public class BankHubProcess
    {
        /// <summary>
        /// (vCong 62522) Thông báo lỗi đã phân loại nguồn (kết nối / ngân hàng / thiết lập tại viện...) của lần GetAccessToken gần nhất.
        /// Null nếu thành công hoặc người dùng tự đóng cửa sổ đăng nhập. Nơi gọi nhận token rỗng thì hiển thị chuỗi này.
        /// </summary>
        public static string LastErrorMessage { get; private set; }

        /// <summary>LastErrorMessage đã được hiện cho người dùng (VD cửa sổ đăng nhập MB Bank đã báo) → nơi gọi không hiện lại</summary>
        private static bool lastErrorShown;

        /// <summary>
        /// (vCong 62522) Gọi khi GetAccessToken trả rỗng: hiện lý do thật (đã phân loại nguồn lỗi) thay cho câu chung chung
        /// "Không thể kết nối đến hệ thống ngân hàng". Không hiện lại nếu cửa sổ đăng nhập đã báo.
        /// </summary>
        /// <summary>Đối tác ngân hàng theo mã (MBB → MB Bank, PVCB → PVcomBank) để dựng thông báo lỗi (vCong 62522)</summary>
        public static Inventec.Common.IntegrationError.IntegrationPartner GetPartner(string bankCode)
        {
            return HisConfigData.GetPartner(bankCode);
        }

        public static void ShowLastError()
        {
            if (lastErrorShown) return;
            string message = String.IsNullOrWhiteSpace(LastErrorMessage)
                ? "Không lấy được mã xác thực (token) ngân hàng nên chưa thể thực hiện. Vui lòng đăng nhập lại tài khoản ngân hàng hoặc thử lại sau."
                : LastErrorMessage;
            MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        public static bool CheckExpiry(string bankCode)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisBankOauthFilter filter = new HisBankOauthFilter();
                filter.LOGINNAME__EXACT = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                filter.ACCESS_TOKEN_EXPIRY_FROM = Inventec.Common.DateTime.Get.Now();
                List<HIS_BANK_OAUTH> data = new BackendAdapter(param).Get<List<HIS_BANK_OAUTH>>("/api/HisBankOauth/Get", ApiConsumers.MosConsumer, filter, param);
                if (data == null || data.Count == 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
            return true;
        }

        public static string GetAccessToken(string bankCode)
        {
            string accessToken = null;
            LastErrorMessage = null;
            lastErrorShown = false;
            Inventec.Common.IntegrationError.IntegrationPartner partner = HisConfigData.GetPartner(bankCode);
            try
            {
                CommonParam param = new CommonParam();
                HisBankOauthFilter filter = new HisBankOauthFilter();
                filter.LOGINNAME__EXACT = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                filter.BANK_CODE__EXACT = bankCode;
                filter.ACCESS_TOKEN_EXPIRY_FROM = Inventec.Common.DateTime.Get.Now();
                List<HIS_BANK_OAUTH> data = new BackendAdapter(param).Get<List<HIS_BANK_OAUTH>>("/api/HisBankOauth/Get", ApiConsumers.MosConsumer, filter, param);

                switch (bankCode)
                {
                    case "MBB":
                        if (data != null && data.Count > 0)
                        {
                            accessToken = data[0].ACCESS_TOKEN;
                        }
                        else
                        {
                            MessageBox.Show("Vui lòng đăng nhập trước khi tiếp tục!");
                            Popup.BankLogin bankLogin = new Popup.BankLogin(bankCode, (token) =>
                            {
                                accessToken = token.ACCESS_TOKEN;
                            });
                            bankLogin.ShowDialog();
                            if (String.IsNullOrEmpty(accessToken))
                            {
                                // (vCong 62522) cửa sổ đăng nhập đã hiện lỗi (nếu có); người dùng tự đóng → báo chưa đăng nhập
                                LastErrorMessage = bankLogin.ErrorMessage;
                                lastErrorShown = !String.IsNullOrEmpty(bankLogin.ErrorMessage);
                                if (String.IsNullOrEmpty(LastErrorMessage))
                                    LastErrorMessage = Inventec.Common.IntegrationError.IntegrationErrorMessage.BusinessError(partner,
                                        "Chưa đăng nhập tài khoản ngân hàng " + Inventec.Common.IntegrationError.IntegrationErrorMessage.GetName(partner) + " (cửa sổ đăng nhập đã đóng) nên chưa thể thực hiện");
                            }
                        }
                        break;
                    case "PVCB":
                        if (data != null && data.Count > 0)
                        {
                            accessToken = data[0].ACCESS_TOKEN;
                        }
                        else
                        {
                            var configPvcb = HisConfigData.GetByConfig(bankCode);
                            PvcbTokenManager tokenManager = new PvcbTokenManager(configPvcb.AuthUrl, configPvcb.ClientId, configPvcb.ClientSecret);

                            accessToken = Task.Run(() => tokenManager.GetAccessTokenAsync()).GetAwaiter().GetResult();
                            if (String.IsNullOrEmpty(accessToken))
                            {
                                LastErrorMessage = Inventec.Common.IntegrationError.IntegrationErrorMessage.LoginError(partner, null);
                            }
                            if (accessToken != null)
                            {
                                CommonParam paramOauth = new CommonParam();
                                //gọi api lưu thông tin token
                                HIS_BANK_OAUTH auth = new HIS_BANK_OAUTH();
                                auth.LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                                auth.ACCESS_TOKEN = accessToken;
                                auth.REFRESH_TOKEN = accessToken;
                                auth.BANK_LOGINNAME = configPvcb.ClientId;
                                auth.ACCESS_TOKEN_EXPIRY = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(tokenManager.GetExpires());
                                auth.REFRESH_TOKEN_EXPIRY = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(tokenManager.GetExpires());
                                auth.BANK_CODE = bankCode;
                                HIS_BANK_OAUTH hisBankOauth = new BackendAdapter(param).Post<HIS_BANK_OAUTH>("/api/HisBankOauth/Create", ApiConsumers.MosConsumer, auth, paramOauth);
                            }
                        }
                        break;
                    default:
                        // (vCong 62522) thay MessageBox "Ngân hàng không được hỗ trợ!" bằng LastErrorMessage để nơi gọi hiển thị 1 lần
                        LastErrorMessage = Inventec.Common.IntegrationError.IntegrationErrorMessage.ConfigError(partner,
                            string.Format("Ngân hàng mã \"{0}\" chưa được hỗ trợ kết nối BankHub (chỉ hỗ trợ MBB, PVCB): kiểm tra mã ngân hàng của cấu hình hoàn tiền/kiểm tra tài khoản", bankCode));
                        return null;
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                // (vCong 62522) trước đây chỉ log → nơi gọi chỉ báo "Không thể kết nối đến hệ thống ngân hàng"; nay giữ lý do thật
                if (String.IsNullOrEmpty(accessToken))
                    LastErrorMessage = Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(ex, partner, "lấy mã xác thực (token) ngân hàng");
            }
            return accessToken;
        }
    }
}

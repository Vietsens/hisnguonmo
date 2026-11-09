using HIS.Desktop.Plugins.Library.BankHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.BankHub.Config
{
    class HisConfigData
    {
        internal const string REFUND_BY_TRANSFER_MBB__CONFIG = "HIS.Desktop.Plugins.RefundByTransfer.MBBInfo";
        internal const string REFUND_BY_TRANSFER_PVCB__CONFIG = "HIS.Desktop.Plugins.RefundByTransfer.PVCBInfo";

        internal static BankHubAuthConfig GetByConfig(String bankCode)
        {
            BankHubAuthConfig result = null;
            if (!String.IsNullOrEmpty(bankCode))
            {
                if (bankCode == "MBB")
                {
                    string serviceConfig = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(REFUND_BY_TRANSFER_MBB__CONFIG);
                    // (vCong 62522) thiếu/sai cấu hình → báo THIẾT LẬP TẠI VIỆN thay vì NullReference/IndexOutOfRange
                    string[] strings = SplitConfig(serviceConfig, REFUND_BY_TRANSFER_MBB__CONFIG, 5, "SourceAccount|ClientId|RedirectUri|AuthUrl|TokenUrl", GetPartner(bankCode));

                    string clientId = strings[1];//"bvbachmai"
                    string redirectUri = strings[2];//"http://mostest.onelink.vn/mb-callback"           
                    string authUrl = strings[3];//"https://api.mbbank.com.vn/biz-kc/realms/bank-hub/protocol/openid-connect/auth"
                    string tokenUrl = strings[4];//"https://api.mbbank.com.vn/biz-kc/realms/bank-hub/protocol/openid-connect/token"  

                    result = BankHubAuthConfig.Production(authUrl, tokenUrl, clientId, redirectUri, "");
                    result.BankName = GetBankName(bankCode);
                }
                else if (bankCode == "PVCB")
                {
                    string serviceConfig = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(REFUND_BY_TRANSFER_PVCB__CONFIG);
                    string[] strings = SplitConfig(serviceConfig, REFUND_BY_TRANSFER_PVCB__CONFIG, 3, "ClientId|ClientSecret|TokenUrl", GetPartner(bankCode));

                    string clientId = strings[0];//"clientid"
                    string clientSecret = strings[1];//clientSecret
                    // PVCB dùng Client Credentials flow — chỉ có 1 URL là token endpoint, không có auth URL riêng
                    string tokenUrl = strings[2];//https://apis-uat.pvcombank.io/idp/oauth2/token

                    result = BankHubAuthConfig.Production("", tokenUrl, clientId, "", clientSecret);
                    result.BankName = GetBankName(bankCode);
                }
            }
            return result;
        }

        /// <summary>Tên ngân hàng hiển thị trong thông báo lỗi (vCong 62522)</summary>
        internal static string GetBankName(string bankCode)
        {
            switch (bankCode)
            {
                case "MBB": return "MB Bank";
                case "PVCB": return "PVcomBank";
                default: return String.IsNullOrEmpty(bankCode) ? "BankHub" : bankCode;
            }
        }

        internal static Inventec.Common.IntegrationError.IntegrationPartner GetPartner(string bankCode)
        {
            return Inventec.Common.IntegrationError.IntegrationPartner.Bank(GetBankName(bankCode));
        }

        /// <summary>Tách chuỗi cấu hình "a|b|c", thiếu/rỗng → ném lỗi THIẾT LẬP TẠI VIỆN nêu rõ key và định dạng (vCong 62522)</summary>
        private static string[] SplitConfig(string serviceConfig, string key, int minCount, string format, Inventec.Common.IntegrationError.IntegrationPartner partner)
        {
            if (String.IsNullOrWhiteSpace(serviceConfig))
                throw new Inventec.Common.IntegrationError.IntegrationException(Inventec.Common.IntegrationError.ErrorSource.Config, partner,
                    string.Format("Chưa khai báo cấu hình hệ thống {0} (định dạng {1})", key, format));
            string[] strings = serviceConfig.Split('|');
            if (strings.Length < minCount)
                throw new Inventec.Common.IntegrationError.IntegrationException(Inventec.Common.IntegrationError.ErrorSource.Config, partner,
                    string.Format("Cấu hình hệ thống {0} thiếu tham số: cần {1} phần ngăn cách bởi dấu | theo định dạng {2}, hiện có {3} phần", key, minCount, format, strings.Length));
            return strings;
        }
    }
}

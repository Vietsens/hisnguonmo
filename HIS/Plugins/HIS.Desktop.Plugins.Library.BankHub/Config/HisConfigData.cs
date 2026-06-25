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
                    string[] strings = serviceConfig.Split('|');

                    string clientId = strings[1];//"bvbachmai"
                    string redirectUri = strings[2];//"http://mostest.onelink.vn/mb-callback"           
                    string authUrl = strings[3];//"https://api.mbbank.com.vn/biz-kc/realms/bank-hub/protocol/openid-connect/auth"
                    string tokenUrl = strings[4];//"https://api.mbbank.com.vn/biz-kc/realms/bank-hub/protocol/openid-connect/token"  

                    result = BankHubAuthConfig.Production(authUrl, tokenUrl, clientId, redirectUri, "");
                }
                else if (bankCode == "PVCB")
                {
                    string serviceConfig = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(REFUND_BY_TRANSFER_PVCB__CONFIG);
                    string[] strings = serviceConfig.Split('|');

                    string clientId = strings[0];//"clientid"
                    string clientSecret = strings[1];//clientSecret
                    // PVCB dùng Client Credentials flow — chỉ có 1 URL là token endpoint, không có auth URL riêng
                    string tokenUrl = strings[2];//https://apis-uat.pvcombank.io/idp/oauth2/token

                    result = BankHubAuthConfig.Production("", tokenUrl, clientId, "", clientSecret);
                }
            }
            return result;
        }
    }
}

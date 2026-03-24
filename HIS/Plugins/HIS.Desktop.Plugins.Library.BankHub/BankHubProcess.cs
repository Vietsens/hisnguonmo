using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.Library.BankHub.Config;
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
        public static bool CheckExpiry()
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
            try
            {
                CommonParam param = new CommonParam();
                HisBankOauthFilter filter = new HisBankOauthFilter();
                filter.LOGINNAME__EXACT = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                filter.BANK_CODE__EXACT = bankCode;
                filter.ACCESS_TOKEN_EXPIRY_FROM = Inventec.Common.DateTime.Get.Now();
                List<HIS_BANK_OAUTH> data = new BackendAdapter(param).Get<List<HIS_BANK_OAUTH>>("/api/HisBankOauth/Get", ApiConsumers.MosConsumer, filter, param);
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
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return accessToken;
        }
    }
}
